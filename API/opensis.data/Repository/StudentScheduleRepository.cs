/***********************************************************************************
openSIS is a free student information system for public and non-public
schools from Open Solutions for Education, Inc.Website: www.os4ed.com.

Visit the openSIS product website at https://opensis.com to learn more.
If you have question regarding this software or the license, please contact
via the website.

The software is released under the terms of the GNU Affero General Public License as
published by the Free Software Foundation, version 3 of the License.
See https://www.gnu.org/licenses/agpl-3.0.en.html.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Copyright (c) Open Solutions for Education, Inc.

All rights reserved.
***********************************************************************************/

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.StudentSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace opensis.data.Repository
{
    public class StudentScheduleRepository : IStudentScheduleRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public StudentScheduleRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Add Student Course Section Schedule
        /// </summary>
        /// <param name="studentCourseSectionScheduleAddViewModel"></param>
        /// <returns></returns>
        public StudentCourseSectionScheduleAddViewModel AddStudentCourseSectionSchedule(StudentCourseSectionScheduleAddViewModel studentCourseSectionScheduleAddViewModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {                
                try
                {
                    //for insert in job fetch max id.
                    long? Id = 1;
                    var dataExits = this.context?.ScheduledJobs.Where(x => x.TenantId == studentCourseSectionScheduleAddViewModel.TenantId);
                    if (dataExits?.Any() == true)
                    {
                        var scheduledJobData = this.context?.ScheduledJobs.Where(x => x.TenantId == studentCourseSectionScheduleAddViewModel.TenantId).Max(x => x.JobId);
                        if (scheduledJobData != null)
                        {
                            Id = scheduledJobData + 1;
                        }
                    }


                    string conflictMessage = "All Student Scheduled Successfully";
                    studentCourseSectionScheduleAddViewModel._conflictFailure = false;
                    //if (studentCourseSectionScheduleAddViewModel.courseSectionList.Count > 0)
                    if (studentCourseSectionScheduleAddViewModel.courseSectionList?.Count > 0)
                    {
                        int restSeats = 0;
                        //List<StudentMaster> studentData = null;
                        List<StudentMaster>? studentData = null;

                        var studentScheduleViewData = this.context?.StudentScheduleView.Where(e => e.SchoolId == studentCourseSectionScheduleAddViewModel.SchoolId && e.TenantId == studentCourseSectionScheduleAddViewModel.TenantId).ToList();

                        //if (studentScheduleViewData.Count > 0)
                        if (studentScheduleViewData!= null && studentScheduleViewData.Any())
                        {
                            this.context?.StudentScheduleView.RemoveRange(studentScheduleViewData);
                        }

                        foreach (var courseSection in studentCourseSectionScheduleAddViewModel.courseSectionList)
                        {
                            courseSection.AcademicYear = Utility.GetCurrentAcademicYear(this.context!, courseSection.TenantId, courseSection.SchoolId);

                            var studentCourseSectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(c => c.SchoolId == courseSection.SchoolId && c.TenantId == courseSection.TenantId && c.CourseSectionId == courseSection.CourseSectionId && c.AcademicYear == courseSection.AcademicYear && c.IsDropped != true).ToList();

                            //if (studentCourseSectionScheduleData.Count > 0)
                            if (studentCourseSectionScheduleData!=null && studentCourseSectionScheduleData.Any())
                            {

                                restSeats = courseSection.Seats !=null? ((int)courseSection.Seats - studentCourseSectionScheduleData.Count):0;
                            }
                            else
                            {
                                restSeats = courseSection.Seats != null ? ((int)courseSection.Seats):0;
                            }

                            if (restSeats > 0)
                            {
                                if (studentCourseSectionScheduleAddViewModel.studentMasterList.Count > 0)
                                {
                                    if (restSeats < studentCourseSectionScheduleAddViewModel.studentMasterList.Count)
                                    {
                                        studentData = studentCourseSectionScheduleAddViewModel.studentMasterList.Take(restSeats).ToList();
                                        
                                        if (studentData != null && studentData.Any())
                                        {
                                            var restStudentCount = studentCourseSectionScheduleAddViewModel.studentMasterList.Count - studentData.Count;
                                            
                                            if (restStudentCount > 0)
                                            {
                                                var restStudentList = studentCourseSectionScheduleAddViewModel.studentMasterList.TakeLast(restStudentCount).ToList();
                                                
                                                if (restStudentList != null && restStudentList.Any())
                                                {
                                                    foreach (var restStudent in restStudentList)
                                                    {
                                                        var conflictStudent = new StudentScheduleView()
                                                        {
                                                            TenantId = restStudent.TenantId,
                                                            SchoolId = restStudent.SchoolId,
                                                            StudentId = restStudent.StudentId,
                                                            CourseId = courseSection.CourseId,
                                                            CourseSectionId = courseSection.CourseSectionId,
                                                            CourseSectionName = courseSection.CourseSectionName,
                                                            StudentInternalId = restStudent.StudentInternalId,
                                                            StudentName = restStudent.FirstGivenName + " " + restStudent.MiddleName + " " + restStudent.LastFamilyName,
                                                            Scheduled = false,
                                                            ConflictComment = "Seats Not Avalaible"
                                                        };
                                                        studentCourseSectionScheduleAddViewModel._conflictFailure = true;
                                                        this.context?.StudentScheduleView.Add(conflictStudent);

                                                        if (studentCourseSectionScheduleAddViewModel.studentMasterList.Count > 1)
                                                        {
                                                            conflictMessage = "Some Student could not be scheduled due to conflicts. Please find the detailed report below.";
                                                        }
                                                        else
                                                        {
                                                            conflictMessage = "Some courses cannot be scheduled to the student due to conflict";
                                                        }
                                                    }
                                                }
                                            }
                                        }                                        
                                    }
                                    else
                                    {
                                        studentData = studentCourseSectionScheduleAddViewModel.studentMasterList.ToList();
                                    }

                                    if (studentData!=null && studentData.Any())
                                    {
                                        //var studentScheduleViewData = this.context?.StudentScheduleView.Where(e => e.SchoolId == studentCourseSectionScheduleAddViewModel.SchoolId && e.TenantId == studentCourseSectionScheduleAddViewModel.TenantId).ToList();

                                        //if (studentScheduleViewData.Count > 0)
                                        //{
                                        //    this.context?.StudentScheduleView.RemoveRange(studentScheduleViewData);
                                        //}

                                        foreach (var student in studentData)
                                        {
                                            var studentCourseSectionSchedule = this.context?.StudentCoursesectionSchedule.FirstOrDefault(c => c.SchoolId == student.SchoolId && c.TenantId == student.TenantId && c.StudentId == student.StudentId && c.CourseSectionId == courseSection.CourseSectionId && c.AcademicYear == courseSection.AcademicYear && c.IsDropped != true);

                                            var studentEnrollmentData = this.context?.StudentEnrollment.FirstOrDefault(x => x.TenantId == student.TenantId && x.SchoolId == student.SchoolId && x.StudentId == student.StudentId && x.IsActive == true);

                                            if (studentCourseSectionSchedule != null)
                                            {
                                                var conflictStudent = new StudentScheduleView()
                                                {
                                                    TenantId = student.TenantId,
                                                    SchoolId = student.SchoolId,
                                                    StudentId = student.StudentId,
                                                    CourseId = courseSection.CourseId,
                                                    CourseSectionId = courseSection.CourseSectionId,
                                                    CourseSectionName = courseSection.CourseSectionName,
                                                    StudentInternalId = student.StudentInternalId,
                                                    StudentName = student.FirstGivenName +" "+ student.MiddleName +" "+ student.LastFamilyName,
                                                    Scheduled = false,
                                                    ConflictComment = "Student is already scheduled in the course section"
                                                };
                                                studentCourseSectionScheduleAddViewModel._conflictFailure = true;
                                                //this.context.StudentScheduleView.Add(conflictStudent);
                                                this.context?.StudentScheduleView.Add(conflictStudent);

                                                if (studentCourseSectionScheduleAddViewModel.studentMasterList.Count > 1)
                                                {
                                                    conflictMessage = "Some Student could not be scheduled due to conflicts. Please find the detailed report below."; 
                                                }
                                                else
                                                {
                                                    conflictMessage = "Some courses cannot be scheduled to the student due to conflict";
                                                }
                                            }

                                            //Student future enrollment checking
                                            else if (studentEnrollmentData != null && studentEnrollmentData.EnrollmentDate > courseSection.DurationStartDate)
                                            {
                                                var conflictStudent = new StudentScheduleView()
                                                {
                                                    TenantId = student.TenantId,
                                                    SchoolId = student.SchoolId,
                                                    StudentId = student.StudentId,
                                                    CourseId = courseSection.CourseId,
                                                    CourseSectionId = courseSection.CourseSectionId,
                                                    CourseSectionName = courseSection.CourseSectionName,
                                                    StudentInternalId = student.StudentInternalId,
                                                    StudentName = student.FirstGivenName + " " + student.MiddleName + " " + student.LastFamilyName,
                                                    Scheduled = false,
                                                    ConflictComment = "Student enrollment date is grater than scheduled course section date"
                                                };
                                                studentCourseSectionScheduleAddViewModel._conflictFailure = true;
                                                //this.context.StudentScheduleView.Add(conflictStudent);
                                                this.context?.StudentScheduleView.Add(conflictStudent);

                                                if (studentCourseSectionScheduleAddViewModel.studentMasterList.Count > 1)
                                                {
                                                    conflictMessage = "Some Student could not be scheduled due to conflicts. Please find the detailed report below.";
                                                }
                                                else
                                                {
                                                    conflictMessage = "Some courses cannot be scheduled to the student due to conflict";
                                                }
                                            }

                                            //Student alredy inactive enrollment checking
                                            else if (studentEnrollmentData == null || studentEnrollmentData.ExitDate < courseSection.DurationStartDate)
                                            {
                                                var conflictStudent = new StudentScheduleView()
                                                {
                                                    TenantId = student.TenantId,
                                                    SchoolId = student.SchoolId,
                                                    StudentId = student.StudentId,
                                                    CourseId = courseSection.CourseId,
                                                    CourseSectionId = courseSection.CourseSectionId,
                                                    CourseSectionName = courseSection.CourseSectionName,
                                                    StudentInternalId = student.StudentInternalId,
                                                    StudentName = student.FirstGivenName + " " + student.MiddleName + " " + student.LastFamilyName,
                                                    Scheduled = false,
                                                    ConflictComment = "Student Exit date is smaller than scheduled course section date"
                                                };
                                                studentCourseSectionScheduleAddViewModel._conflictFailure = true;
                                                this.context?.StudentScheduleView.Add(conflictStudent);

                                                if (studentCourseSectionScheduleAddViewModel.studentMasterList.Count > 1)
                                                {
                                                    conflictMessage = "Some Student could not be scheduled due to conflicts. Please find the detailed report below.";
                                                }
                                                else
                                                {
                                                    conflictMessage = "Some courses cannot be scheduled to the student due to conflict";
                                                }
                                            }
                                            else
                                            {
                                                var courseSectionAllData = this.context?.AllCourseSectionView.Where(c => c.TenantId == courseSection.TenantId && c.SchoolId == courseSection.SchoolId && c.CourseSectionId == courseSection.CourseSectionId).ToList();

                                                if (courseSectionAllData?.FirstOrDefault()?.AllowStudentConflict == true)
                                                {
                                                    var studentCourseScheduling = new StudentCoursesectionSchedule()
                                                    {
                                                        TenantId = courseSection.TenantId,
                                                        SchoolId = courseSection.SchoolId,
                                                        StudentId = student.StudentId,
                                                        CourseId = courseSection.CourseId,
                                                        CourseSectionId = courseSection.CourseSectionId,
                                                        StudentGuid = student.StudentGuid,
                                                        AlternateId = student.AlternateId,
                                                        StudentInternalId = student.StudentInternalId,
                                                        //FirstGivenName = student.FirstGivenName,
                                                        FirstGivenName = student.FirstGivenName??"",
                                                        MiddleName = student.MiddleName,
                                                        //LastFamilyName = student.LastFamilyName,
                                                        LastFamilyName = student.LastFamilyName ?? "",
                                                        FirstLanguageId = student.FirstLanguageId,
                                                        //GradeId = studentEnrollmentData.GradeId,
                                                        GradeId = studentEnrollmentData != null ? studentEnrollmentData.GradeId : null,
                                                        //AcademicYear = (decimal)courseSection.AcademicYear,
                                                        AcademicYear = courseSection.AcademicYear != null? (decimal)courseSection.AcademicYear:0 ,
                                                        GradeScaleId = courseSection.GradeScaleId,
                                                        CourseSectionName = courseSection.CourseSectionName,
                                                        CalendarId = courseSection.CalendarId,
                                                        CreatedBy = studentCourseSectionScheduleAddViewModel.CreatedBy,
                                                        CreatedOn = DateTime.UtcNow,
                                                        EffectiveStartDate = courseSection.DurationStartDate,
                                                        EffectiveDropDate = courseSection.DurationEndDate
                                                    };
                                                    //this.context.StudentCoursesectionSchedule.Add(studentCourseScheduling);
                                                    this.context?.StudentCoursesectionSchedule.Add(studentCourseScheduling);

                                                    var conflictStudent = new StudentScheduleView()
                                                    {
                                                        TenantId = student.TenantId,
                                                        SchoolId = student.SchoolId,
                                                        StudentId = student.StudentId,
                                                        CourseId = courseSection.CourseId,
                                                        CourseSectionId = courseSection.CourseSectionId,
                                                        CourseSectionName = courseSection.CourseSectionName,
                                                        StudentInternalId = student.StudentInternalId,
                                                        StudentName = student.FirstGivenName + " " + student.MiddleName + " " + student.LastFamilyName,
                                                        Scheduled = true,
                                                    };
                                                    this.context?.StudentScheduleView.Add(conflictStudent);

                                                    //this block for add this req as a job
                                                    var studentCoursesectionSchedule = new StudentCoursesectionSchedule
                                                    {
                                                        CourseSectionId = courseSection.CourseSectionId,
                                                        EffectiveStartDate = DateTime.UtcNow,
                                                        EffectiveDropDate = courseSection.DurationEndDate
                                                    };

                                                    var scheduledStudentDropModel = new ScheduledStudentDropModel
                                                    {
                                                        TenantId = student.TenantId,
                                                        SchoolId = student.SchoolId,
                                                        StudentId = student.StudentId
                                                    };                                                 scheduledStudentDropModel.studentCoursesectionScheduleList.Add(studentCoursesectionSchedule);

                                                    var scheduledJob = new ScheduledJob
                                                    {
                                                        TenantId = scheduledStudentDropModel.TenantId,
                                                        SchoolId = scheduledStudentDropModel.SchoolId,
                                                        JobId = (long)Id,
                                                        AcademicYear = courseSection.AcademicYear,
                                                        JobTitle = "DropStudentfromScheduledCourseSections",
                                                        JobScheduleDate = courseSection.DurationEndDate!.Value.AddDays(1),
                                                        ApiTitle = "GroupDropForScheduledStudent",
                                                        ControllerPath = scheduledStudentDropModel._tenantName + "/StudentSchedule",
                                                        TaskJson = JsonConvert.SerializeObject(scheduledStudentDropModel),
                                                        LastRunStatus = null,
                                                        LastRunTime = null,
                                                        IsActive = true,
                                                        CreatedBy = scheduledStudentDropModel.UpdatedBy,
                                                        CreatedOn = DateTime.UtcNow
                                                    };
                                                    this.context?.ScheduledJobs.Add(scheduledJob);
                                                    Id++;
                                                 
                                                }
                                                else
                                                {
                                                    //var courseSectionAllData = this.context?.AllCourseSectionView.Where(c => c.TenantId == courseSection.TenantId && c.SchoolId == courseSection.SchoolId && c.CourseSectionId == courseSection.CourseSectionId).ToList();


                                                    if (courseSectionAllData != null && courseSectionAllData.Any())
                                                    {
                                                        bool isPeriodConflict = false;

                                                        foreach (var courseSectionAll in courseSectionAllData)
                                                        {                                                            
                                                            var courseSectionData = this.context?.AllCourseSectionView.
                                                                                   //Join(this.context?.StudentCoursesectionSchedule,
                                                                                   Join(this.context.StudentCoursesectionSchedule,
                                                                                   acsv => acsv.CourseSectionId, scs => scs.CourseSectionId,
                                                                                   (acsv, scs) => new { acsv, scs }).AsEnumerable().Where(x => x.scs.TenantId == courseSection.TenantId && x.acsv.TenantId == courseSection.TenantId && x.scs.SchoolId == courseSection.SchoolId && x.acsv.SchoolId == courseSection.SchoolId && x.scs.StudentId == student.StudentId && x.acsv.DurationEndDate > courseSectionAll.DurationStartDate && x.scs.IsDropped != true && x.acsv.AllowStudentConflict != true
                                                                                   &&
                                                                                   (
                                                                                   //courseSectionAll.FixedPeriodId != null && ((x.acsv.FixedPeriodId == courseSectionAll.FixedPeriodId || x.acsv.VarPeriodId == courseSectionAll.FixedPeriodId || x.acsv.CalPeriodId == courseSectionAll.FixedPeriodId) && ((x.acsv.FixedDays != null && (Regex.IsMatch(courseSectionAll.FixedDays.ToLower(), x.acsv.FixedDays.ToLower(), RegexOptions.IgnoreCase))) || (x.acsv.VarDay != null && (courseSectionAll.FixedDays.ToLower().Contains(x.acsv.VarDay.ToLower()))) || (x.acsv.CalDay != null && (courseSectionAll.FixedDays.ToLower().Contains(x.acsv.CalDay.ToLower())))))
                                                                                   //||
                                                                                   //courseSectionAll.VarPeriodId != null && ((x.acsv.FixedPeriodId == courseSectionAll.VarPeriodId || x.acsv.VarPeriodId == courseSectionAll.VarPeriodId || x.acsv.CalPeriodId == courseSectionAll.VarPeriodId) && ((x.acsv.FixedDays != null && (Regex.IsMatch(courseSectionAll.VarDay.ToLower(), x.acsv.FixedDays.ToLower(), RegexOptions.IgnoreCase))) || (x.acsv.VarDay != null && (courseSectionAll.VarDay.ToLower().Contains(x.acsv.VarDay.ToLower()))) || (x.acsv.CalDay != null && (courseSectionAll.VarDay.ToLower().Contains(x.acsv.CalDay.ToLower())))))
                                                                                   //||
                                                                                   //courseSectionAll.CalPeriodId != null && ((x.acsv.FixedPeriodId == courseSectionAll.CalPeriodId || x.acsv.VarPeriodId == courseSectionAll.CalPeriodId || x.acsv.CalPeriodId == courseSectionAll.CalPeriodId) && ((x.acsv.FixedDays != null && (Regex.IsMatch(courseSectionAll.CalDay.ToLower(), x.acsv.FixedDays.ToLower(), RegexOptions.IgnoreCase))) || (x.acsv.VarDay != null && (courseSectionAll.CalDay.ToLower().Contains(x.acsv.VarDay.ToLower()))) || (x.acsv.CalDay != null && (courseSectionAll.CalDay.ToLower().Contains(x.acsv.CalDay.ToLower())))))
                                                                                   //||
                                                                                   courseSectionAll.FixedPeriodId != null && ((x.acsv.FixedPeriodId == courseSectionAll.FixedPeriodId || x.acsv.VarPeriodId == courseSectionAll.FixedPeriodId || x.acsv.CalPeriodId == courseSectionAll.FixedPeriodId) && ((x.acsv.FixedDays != null && (Regex.IsMatch((courseSectionAll.FixedDays??"").ToLower(), x.acsv.FixedDays.ToLower(), RegexOptions.IgnoreCase))) || (x.acsv.VarDay != null && ((courseSectionAll.FixedDays??"").ToLower().Contains(x.acsv.VarDay.ToLower()))) || (x.acsv.CalDay != null && ((courseSectionAll.FixedDays??"").ToLower().Contains(x.acsv.CalDay.ToLower())))))
                                                                                   ||
                                                                                   courseSectionAll.VarPeriodId != null && ((x.acsv.FixedPeriodId == courseSectionAll.VarPeriodId || x.acsv.VarPeriodId == courseSectionAll.VarPeriodId || x.acsv.CalPeriodId == courseSectionAll.VarPeriodId) && ((x.acsv.FixedDays != null && (Regex.IsMatch((courseSectionAll.VarDay??"").ToLower(), x.acsv.FixedDays.ToLower(), RegexOptions.IgnoreCase))) || (x.acsv.VarDay != null && ((courseSectionAll.VarDay??"").ToLower().Contains(x.acsv.VarDay.ToLower()))) || (x.acsv.CalDay != null && ((courseSectionAll.VarDay??"").ToLower().Contains(x.acsv.CalDay.ToLower())))))
                                                                                   ||
                                                                                   courseSectionAll.CalPeriodId != null && ((x.acsv.FixedPeriodId == courseSectionAll.CalPeriodId || x.acsv.VarPeriodId == courseSectionAll.CalPeriodId || x.acsv.CalPeriodId == courseSectionAll.CalPeriodId) && ((x.acsv.FixedDays != null && (Regex.IsMatch((courseSectionAll.CalDay??"").ToLower(), x.acsv.FixedDays.ToLower(), RegexOptions.IgnoreCase))) || (x.acsv.VarDay != null && ((courseSectionAll.CalDay??"").ToLower().Contains(x.acsv.VarDay.ToLower()))) || (x.acsv.CalDay != null && ((courseSectionAll.CalDay??"").ToLower().Contains(x.acsv.CalDay.ToLower())))))
                                                                                   ||
                                                                                    courseSectionAll.BlockPeriodId != null && (x.acsv.BlockPeriodId == courseSectionAll.BlockPeriodId && x.acsv.BlockRoomId == courseSectionAll.BlockRoomId && x.acsv.BlockId == courseSectionAll.BlockId)
                                                                                   )
                                                                                );

                                                            //if (courseSectionData.ToList().Count > 0)
                                                            if (courseSectionData?.ToList().Count > 0)
                                                            {
                                                                isPeriodConflict = true;
                                                                break;
                                                            }
                                                        }
                                                        if (!(bool)isPeriodConflict)
                                                        {
                                                            //this is for student already exist in cs or not.If exixt then update its value else insert student.
                                                            var studentCourseSectionScheduleExists = this.context?.StudentCoursesectionSchedule.FirstOrDefault(c => c.SchoolId == student.SchoolId && c.TenantId == student.TenantId && c.StudentId == student.StudentId && c.CourseSectionId == courseSection.CourseSectionId && c.AcademicYear == courseSection.AcademicYear && c.IsDropped == true && c.EffectiveDropDate!=null);
                                                            if (studentCourseSectionScheduleExists != null)
                                                            {
                                                                studentCourseSectionScheduleExists.IsDropped = null;
                                                                studentCourseSectionScheduleExists.EffectiveDropDate = null;
                                                            }
                                                            else
                                                            {
                                                                var studentCourseScheduling = new StudentCoursesectionSchedule()
                                                                {
                                                                    TenantId = courseSection.TenantId,
                                                                    SchoolId = courseSection.SchoolId,
                                                                    StudentId = student.StudentId,
                                                                    CourseId = courseSection.CourseId,
                                                                    CourseSectionId = courseSection.CourseSectionId,
                                                                    StudentGuid = student.StudentGuid,
                                                                    AlternateId = student.AlternateId,
                                                                    StudentInternalId = student.StudentInternalId,
                                                                    //FirstGivenName = student.FirstGivenName,
                                                                    FirstGivenName = student.FirstGivenName ?? "",
                                                                    MiddleName = student.MiddleName,
                                                                    //LastFamilyName = student.LastFamilyName,
                                                                    LastFamilyName = student.LastFamilyName ?? "",
                                                                    FirstLanguageId = student.FirstLanguageId,
                                                                    //GradeId = studentEnrollmentData.GradeId,
                                                                    GradeId = studentEnrollmentData != null ? studentEnrollmentData.GradeId : null,
                                                                    //AcademicYear = (decimal)courseSection.AcademicYear,
                                                                    AcademicYear = courseSection.AcademicYear !=null? (decimal)courseSection.AcademicYear:0,
                                                                    GradeScaleId = courseSection.GradeScaleId,
                                                                    CourseSectionName = courseSection.CourseSectionName,
                                                                    CalendarId = courseSection.CalendarId,
                                                                    CreatedBy = studentCourseSectionScheduleAddViewModel.CreatedBy,
                                                                    CreatedOn = DateTime.UtcNow,
                                                                    EffectiveStartDate = courseSection.DurationStartDate,
                                                                    EffectiveDropDate = courseSection.DurationEndDate

                                                                };
                                                                this.context?.StudentCoursesectionSchedule.Add(studentCourseScheduling);
                                                            }
                                                            var conflictStudent = new StudentScheduleView()
                                                            {
                                                                TenantId = student.TenantId,
                                                                SchoolId = student.SchoolId,
                                                                StudentId = student.StudentId,
                                                                CourseId = courseSection.CourseId,
                                                                CourseSectionId = courseSection.CourseSectionId,
                                                                CourseSectionName = courseSection.CourseSectionName,
                                                                StudentInternalId = student.StudentInternalId,
                                                                StudentName = student.FirstGivenName + " " + student.MiddleName + " " + student.LastFamilyName,
                                                                Scheduled = true,
                                                            };
                                                            this.context?.StudentScheduleView.Add(conflictStudent);

                                                            //this block for add this req as a job
                                                            var studentCoursesectionSchedule = new StudentCoursesectionSchedule
                                                            {
                                                                CourseSectionId = courseSection.CourseSectionId,
                                                                EffectiveStartDate = DateTime.UtcNow,
                                                                EffectiveDropDate = courseSection.DurationEndDate
                                                            };

                                                            var scheduledStudentDropModel = new ScheduledStudentDropModel
                                                            {
                                                                TenantId = student.TenantId,
                                                                SchoolId = student.SchoolId,
                                                                StudentId = student.StudentId,
                                                                _tenantName = studentCourseSectionScheduleAddViewModel._tenantName
                                                            }; scheduledStudentDropModel.studentCoursesectionScheduleList.Add(studentCoursesectionSchedule);

                                                            var scheduledJob = new ScheduledJob
                                                            {
                                                                TenantId = scheduledStudentDropModel.TenantId,
                                                                SchoolId = scheduledStudentDropModel.SchoolId,
                                                                JobId = (long)Id,
                                                                AcademicYear = courseSection.AcademicYear,
                                                                JobTitle = "DropStudentfromScheduledCourseSections",
                                                                JobScheduleDate = courseSection.DurationEndDate!.Value.AddDays(1),
                                                                ApiTitle = "GroupDropForScheduledStudent",
                                                                ControllerPath = scheduledStudentDropModel._tenantName + "/StudentSchedule",
                                                                TaskJson = JsonConvert.SerializeObject(scheduledStudentDropModel),
                                                                LastRunStatus = null,
                                                                LastRunTime = null,
                                                                IsActive = true,
                                                                CreatedBy = scheduledStudentDropModel.UpdatedBy,
                                                                CreatedOn = DateTime.UtcNow
                                                            };
                                                            this.context?.ScheduledJobs.Add(scheduledJob);
                                                            Id++;

                                                        }
                                                        else
                                                        {
                                                            var conflictStudent = new StudentScheduleView()
                                                            {
                                                                TenantId = student.TenantId,
                                                                SchoolId = student.SchoolId,
                                                                StudentId = student.StudentId,
                                                                CourseId = courseSection.CourseId,
                                                                CourseSectionId = courseSection.CourseSectionId,
                                                                CourseSectionName = courseSection.CourseSectionName,
                                                                StudentInternalId = student.StudentInternalId,
                                                                StudentName = student.FirstGivenName + " " + student.MiddleName + " " + student.LastFamilyName,
                                                                Scheduled = false,
                                                                ConflictComment = "There is a period conflict"
                                                            };
                                                            studentCourseSectionScheduleAddViewModel._conflictFailure = true;
                                                            this.context?.StudentScheduleView.Add(conflictStudent);
                                                            
                                                            if (studentCourseSectionScheduleAddViewModel.studentMasterList.Count > 1)
                                                            {
                                                                conflictMessage = "Some Student could not be scheduled due to conflicts. Please find the detailed report below.";
                                                            }
                                                            else
                                                            {
                                                                conflictMessage = "Some courses cannot be scheduled to the student due to conflict";
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        studentCourseSectionScheduleAddViewModel._failure = true;
                                                        studentCourseSectionScheduleAddViewModel._message = "Course Section Does Not Exist";
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    studentCourseSectionScheduleAddViewModel._message = "Select Atleast One Student";
                                    studentCourseSectionScheduleAddViewModel._failure = true;
                                    studentCourseSectionScheduleAddViewModel.ConflictMessage = null;
                                    return studentCourseSectionScheduleAddViewModel;
                                }
                            }
                            else
                            {
                                if (studentCourseSectionScheduleAddViewModel.studentMasterList.Count>0)
                                {
                                    foreach (var studentMaster in studentCourseSectionScheduleAddViewModel.studentMasterList)
                                    {
                                        var conflictStudent = new StudentScheduleView()
                                        {
                                            TenantId = studentMaster.TenantId,
                                            SchoolId = studentMaster.SchoolId,
                                            StudentId = studentMaster.StudentId,
                                            CourseId = courseSection.CourseId,
                                            CourseSectionId = courseSection.CourseSectionId,
                                            CourseSectionName = courseSection.CourseSectionName,
                                            StudentInternalId = studentMaster.StudentInternalId,
                                            StudentName = studentMaster.FirstGivenName + " " + studentMaster.MiddleName + " " + studentMaster.LastFamilyName,
                                            Scheduled = false,
                                            ConflictComment = "Seats Not Avalaible"
                                        };
                                        studentCourseSectionScheduleAddViewModel._conflictFailure = true;
                                        this.context?.StudentScheduleView.Add(conflictStudent);
                                        
                                        if (studentCourseSectionScheduleAddViewModel.studentMasterList.Count > 1)
                                        {
                                            conflictMessage = "Some Student could not be scheduled due to conflicts. Please find the detailed report below.";
                                        }
                                        else
                                        {
                                            conflictMessage = "Some courses cannot be scheduled to the student due to conflict";
                                        }
                                    }
                                }                               
                            }
                        }
                        //var studentScheduleViewData = this.context?.StudentScheduleView.Where(e => e.SchoolId == studentCourseSectionScheduleAddViewModel.SchoolId && e.TenantId == studentCourseSectionScheduleAddViewModel.TenantId).ToList();

                        //if (studentScheduleViewData.Count > 0)
                        //{
                        //    this.context?.StudentScheduleView.RemoveRange(studentScheduleViewData);
                        //}
                        this.context?.SaveChanges();
                        transaction?.Commit();
                        studentCourseSectionScheduleAddViewModel._message = "Student Schedule added successfully";
                        studentCourseSectionScheduleAddViewModel.ConflictMessage = conflictMessage;
                        studentCourseSectionScheduleAddViewModel._failure = false;
                    }
                    else
                    {
                        studentCourseSectionScheduleAddViewModel._message = "Select Atleast One Course Section";
                        studentCourseSectionScheduleAddViewModel.ConflictMessage = null;
                        studentCourseSectionScheduleAddViewModel._failure = true;
                        return studentCourseSectionScheduleAddViewModel;
                    }
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    studentCourseSectionScheduleAddViewModel._failure = true;
                    studentCourseSectionScheduleAddViewModel.ConflictMessage = null;
                    studentCourseSectionScheduleAddViewModel._message = es.Message;
                }
            }
            return studentCourseSectionScheduleAddViewModel;
        }

        /// <summary>
        /// Search Scheduled Student For Group Drop
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public ScheduleStudentListViewModel SearchScheduledStudentForGroupDrop_old(PageResult pageResult)
        {
            ScheduleStudentListViewModel scheduleStudentListView = new ScheduleStudentListViewModel();
            IQueryable<ScheduleStudentForView>? transactionIQ = null;
            try
            {
                var scheduledStudentData = this.context?.StudentCoursesectionSchedule.
                                    //Join(this.context?.StudentMaster,
                                    Join(this.context.StudentMaster,
                                    scs => scs.StudentId, sm => sm.StudentId,
                                    (scs, sm) => new { scs, sm }).Where(c => c.scs.TenantId == pageResult.TenantId && c.scs.SchoolId == pageResult.SchoolId && c.scs.CourseSectionId == pageResult.CourseSectionId && c.sm.SchoolId == pageResult.SchoolId && c.sm.TenantId == pageResult.TenantId && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? c.sm.IsActive !=false : true)).ToList().Select(ssv => new ScheduleStudentForView
                                    {
                                        SchoolId = ssv.sm.SchoolId,
                                        TenantId = ssv.sm.TenantId,
                                        StudentId = ssv.sm.StudentId,
                                        FirstGivenName = ssv.sm.FirstGivenName,
                                        LastFamilyName = ssv.sm.LastFamilyName,
                                        AlternateId = ssv.sm.AlternateId,
                                        StudentInternalId=ssv.sm.StudentInternalId,
                                        GradeLevel = this.context.Gradelevels.FirstOrDefault(c => c.TenantId == ssv.sm.TenantId && c.SchoolId == ssv.sm.SchoolId && c.GradeId == ssv.scs.GradeId)?.Title,
                                        Section = this.context.Sections.FirstOrDefault(c => c.TenantId == ssv.sm.TenantId && c.SchoolId == ssv.sm.SchoolId && c.SectionId == ssv.sm.SectionId)?.Name,
                                        GradeId= ssv.scs.GradeId,
                                        GradeScaleId= this.context.Grade.FirstOrDefault(c => c.TenantId == ssv.sm.TenantId && c.SchoolId == ssv.sm.SchoolId && c.GradeId == ssv.scs.GradeId)?.GradeScaleId,
                                        PhoneNumber = ssv.sm.MobilePhone,
                                        Action = ssv.scs.IsDropped,
                                        ScheduleDate = ssv.scs.CreatedOn,
                                        StudentPhoto=(pageResult.ProfilePhoto ==true)?ssv.sm.StudentPhoto:null
                                    }).ToList();

                if (scheduledStudentData!= null && scheduledStudentData.Any())
                {
                    if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                    {
                        transactionIQ = scheduledStudentData.AsQueryable();
                    }
                    else
                    {
                        if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                        {
                            string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                            //transactionIQ = scheduledStudentData.Where(x => x.FirstGivenName.ToLower().Contains(Columnvalue.ToLower()) || x.LastFamilyName.ToLower().Contains(Columnvalue.ToLower()) || x.GradeLevel.ToLower().Contains(Columnvalue.ToLower()) || x.ScheduleDate.ToString() == Columnvalue || (x.Section != null && x.Section.ToLower().Contains(Columnvalue.ToLower())) || (x.StudentInternalId != null && x.StudentInternalId.ToLower().Contains(Columnvalue.ToLower())) || (x.AlternateId != null && x.AlternateId.ToLower().Contains(Columnvalue.ToLower())) || (x.PhoneNumber != null && x.PhoneNumber.ToLower().Contains(Columnvalue.ToLower()))).AsQueryable();
                            transactionIQ = scheduledStudentData.Where(x => x.FirstGivenName!=null && x.FirstGivenName.Contains(Columnvalue) || x.LastFamilyName != null && x.LastFamilyName.Contains(Columnvalue) || x.GradeLevel != null && x.GradeLevel.Contains(Columnvalue) || x.ScheduleDate.ToString() == Columnvalue || (x.Section != null && x.Section.Contains(Columnvalue)) || (x.StudentInternalId != null && x.StudentInternalId.Contains(Columnvalue)) || (x.AlternateId != null && x.AlternateId.Contains(Columnvalue)) || (x.PhoneNumber != null && x.PhoneNumber.Contains(Columnvalue))).AsQueryable();
                        }
                        else
                        {
                            //transactionIQ = Utility.FilteredData(pageResult.FilterParams, scheduledStudentData).AsQueryable();
                            transactionIQ = Utility.FilteredData(pageResult.FilterParams!, scheduledStudentData).AsQueryable();
                        }
                        transactionIQ = transactionIQ.Distinct();
                    }
                    if (pageResult.SortingModel != null)
                    {
                        switch (pageResult.SortingModel?.SortColumn?.ToLower())
                        {
                            default:
                                //transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn, pageResult.SortingModel.SortDirection.ToLower());
                                transactionIQ = Utility.Sort(transactionIQ, pageResult?.SortingModel?.SortColumn!, pageResult?.SortingModel?.SortDirection!);
                                break;
                        }

                    }

                    int totalCount = transactionIQ.Count();
                    if (pageResult!.PageNumber > 0 && pageResult.PageSize > 0)
                    {
                        transactionIQ = transactionIQ.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                    }
                    scheduleStudentListView.scheduleStudentForView = transactionIQ.ToList();
                    scheduleStudentListView.TotalCount = totalCount;
                }
                else
                {
                    scheduleStudentListView.scheduleStudentForView = scheduledStudentData ?? new();
                    scheduleStudentListView._failure = false;
                    scheduleStudentListView._message = NORECORDFOUND;
                }

                scheduleStudentListView.TenantId = pageResult.TenantId;
                scheduleStudentListView.SchoolId = pageResult.SchoolId;
                scheduleStudentListView.CourseSectionId = pageResult.CourseSectionId;
                scheduleStudentListView.PageNumber = pageResult.PageNumber;
                scheduleStudentListView._pageSize = pageResult.PageSize;
                scheduleStudentListView._tenantName = pageResult._tenantName;
                scheduleStudentListView._token = pageResult._token;
                scheduleStudentListView._failure = false;
            }
            catch (Exception es)
            {
                scheduleStudentListView._failure = true;
                scheduleStudentListView._message = es.Message; ;
            }
            return scheduleStudentListView;

        }

        /// <summary>
        ///  Get Student List By Course Section
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public ScheduleStudentListViewModel GetStudentListByCourseSection(PageResult pageResult)
        {
            ScheduleStudentListViewModel scheduleStudentListView = new ScheduleStudentListViewModel();
            //IQueryable<ScheduleStudentForView> transactionIQ = null;
            IQueryable<ScheduleStudentForView>? transactionIQ = null;
            var scheduledStudentData = new List<ScheduleStudentForView>();
            try
            {
                /*var scheduledData = this.context?.StudentCoursesectionSchedule.
                                    //Join(this.context?.StudentMaster,
                                    Join(this.context.StudentMaster,
                                    scs => scs.StudentId, sm => sm.StudentId,
                                    (scs, sm) => new { scs, sm }).Where(c => c.scs.TenantId == pageResult.TenantId && c.scs.SchoolId == pageResult.SchoolId && (pageResult.CourseSectionId == null || c.scs.CourseSectionId == pageResult.CourseSectionId) && (pageResult.AcademicYear == null || c.scs.AcademicYear == pageResult.AcademicYear) && (pageResult.AttendanceDate == null || c.scs.EffectiveStartDate!.Value.Date <= pageResult.AttendanceDate.Value.Date) && c.sm.SchoolId == pageResult.SchoolId && c.sm.TenantId == pageResult.TenantId && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? c.sm.IsActive != false : true) && (pageResult.IsDropped == true ? c.scs.IsDropped != true : true)).ToList();*/

                var courseSectionData = this.context?.CourseSection.Include(x => x.SchoolYears).Include(x => x.Semesters).Include(x => x.Quarters).Include(x => x.ProgressPeriods).FirstOrDefault(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && (pageResult.CourseSectionIds == null || pageResult.CourseSectionIds.ToList().Count == 0 || pageResult.CourseSectionIds.Contains(x.CourseSectionId)) /*&& x.DurationEndDate < DateTime.Today.Date*/);

                var scheduledData = this.context?.StudentCoursesectionSchedule.
                                  //Join(this.context?.StudentMaster,
                                  Join(this.context.StudentMaster,
                                  scs => scs.StudentId, sm => sm.StudentId,
                                  (scs, sm) => new { scs, sm }).Where(c => c.scs.TenantId == pageResult.TenantId && c.scs.SchoolId == pageResult.SchoolId && c.sm.SchoolId == pageResult.SchoolId && c.sm.TenantId == pageResult.TenantId && /*(pageResult.CourseSectionId == null || c.scs.CourseSectionId == pageResult.CourseSectionId)*/(pageResult.CourseSectionIds == null || pageResult.CourseSectionIds.ToList().Count == 0 || pageResult.CourseSectionIds.Contains(c.scs.CourseSectionId)) && (pageResult.AcademicYear == null || c.scs.AcademicYear == pageResult.AcademicYear) && (pageResult.AttendanceDate != null ? pageResult.AttendanceDate.Value.Date >= c.scs.EffectiveStartDate!.Value.Date && pageResult.AttendanceDate.Value.Date <= c.scs.EffectiveDropDate!.Value.Date : (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? c.sm.IsActive != false : true) && (pageResult.AciveStudentInCourseSection == true ? (courseSectionData != null && (courseSectionData.DurationEndDate < DateTime.Today.Date && c.scs.EffectiveDropDate == courseSectionData.DurationEndDate) || (c.scs.IsDropped != true)) : true))).ToList();

                if (pageResult.StaffId != null)
                {
                    scheduledStudentData = scheduledData?.Join(this.context!.StaffCoursesectionSchedule,
                                    //scheduledData.Join(this.context?.StaffCoursesectionSchedule,
                                    studentcss => studentcss.scs.CourseSectionId, staffcss => staffcss.CourseSectionId,
                                    (studentcss, staffcss) => new { studentcss, staffcss }).Where(c => c.studentcss.scs.TenantId == pageResult.TenantId && c.studentcss.scs.SchoolId == pageResult.SchoolId && c.staffcss.SchoolId == pageResult.SchoolId && c.staffcss.TenantId == pageResult.TenantId && c.staffcss.StaffId == pageResult.StaffId && c.staffcss.IsDropped != true).ToList().Select(ssv => new ScheduleStudentForView
                                    {
                                        SchoolId = ssv.studentcss.sm.SchoolId,
                                        TenantId = ssv.staffcss.TenantId,
                                        StudentId = ssv.studentcss.scs.StudentId,
                                        StudentGuid = ssv.studentcss.sm.StudentGuid,
                                        FirstGivenName = ssv.studentcss.sm.FirstGivenName,
                                        MiddleName = ssv.studentcss.sm.MiddleName,
                                        LastFamilyName = ssv.studentcss.sm.LastFamilyName,
                                        AlternateId = ssv.studentcss.sm.AlternateId,
                                        StudentInternalId = ssv.studentcss.sm.StudentInternalId,
                                        CourseSectionId = ssv.studentcss.scs.CourseSectionId,
                                        AdmissionNumber = ssv.studentcss.sm.AdmissionNumber,
                                        RollNumber = ssv.studentcss.sm.RollNumber,
                                        Dob = ssv.studentcss.sm.Dob,
                                        Race = ssv.studentcss.sm.Race,
                                        Gender = ssv.studentcss.sm.Gender,
                                        Ethnicity = ssv.studentcss.sm.Ethnicity,
                                        MaritalStatus = ssv.studentcss.sm.MaritalStatus,
                                        CountryOfBirth = ssv.studentcss.sm.CountryOfBirth,
                                        Nationality = ssv.studentcss.sm.Nationality,
                                        FirstLanguageId = ssv.studentcss.sm.FirstLanguageId,
                                        SecondLanguageId = ssv.studentcss.sm.SecondLanguageId,
                                        ThirdLanguageId = ssv.studentcss.sm.ThirdLanguageId,
                                        HomeAddressLineOne = ssv.studentcss.sm.HomeAddressLineOne,
                                        HomeAddressLineTwo = ssv.studentcss.sm.HomeAddressLineTwo,
                                        HomeAddressCountry = ssv.studentcss.sm.HomeAddressCountry,
                                        HomeAddressState = ssv.studentcss.sm.HomeAddressState,
                                        HomeAddressCity = ssv.studentcss.sm.HomeAddressCity,
                                        HomeAddressZip = ssv.studentcss.sm.HomeAddressZip,
                                        BusNo = ssv.studentcss.sm.BusNo,
                                        HomePhone = ssv.studentcss.sm.HomePhone,
                                        MobilePhone = ssv.studentcss.sm.MobilePhone,
                                        PersonalEmail = ssv.studentcss.sm.PersonalEmail,
                                        SchoolEmail = ssv.studentcss.sm.SchoolEmail,
                                        GradeLevel = this.context.Gradelevels.FirstOrDefault(c => c.TenantId == ssv.studentcss.sm.TenantId && c.SchoolId == ssv.studentcss.sm.SchoolId && c.GradeId == ssv.studentcss.scs.GradeId)?.Title,
                                        GradeId = ssv.studentcss.scs.GradeId,
                                        IsActive = ssv.studentcss.sm.IsActive,
                                        CreatedOn = ssv.studentcss.scs.CreatedOn,
                                        CreatedBy = ssv.studentcss.scs.CreatedBy,
                                        UpdatedOn = ssv.studentcss.scs.UpdatedOn,
                                        UpdatedBy = ssv.studentcss.scs.UpdatedBy,
                                        IsDropped = ssv.studentcss.scs.IsDropped,
                                        SchoolName = this.context?.SchoolMaster.Where(x => x.SchoolId == ssv.studentcss.sm.SchoolId).Select(x => x.SchoolName).FirstOrDefault(),
                                        Section = this.context?.Sections.FirstOrDefault(c => c.TenantId == ssv.studentcss.sm.TenantId && c.SchoolId == ssv.studentcss.sm.SchoolId && c.SectionId == ssv.studentcss.sm.SectionId)?.Name,
                                        GradePostingEndDate = courseSectionData?.SchoolYears != null ? courseSectionData?.SchoolYears.PostEndDate : courseSectionData?.Semesters != null ? courseSectionData?.Semesters.PostEndDate : courseSectionData?.Quarters != null ? courseSectionData?.Quarters.PostEndDate : courseSectionData?.ProgressPeriods != null ? courseSectionData?.ProgressPeriods.PostEndDate : null,
                                    }).GroupBy(f => f.StudentId).Select(g => g.First()).ToList();
                }
                else
                {
                    //scheduledStudentData = scheduledData.Select(ssv => new ScheduleStudentForView
                    scheduledStudentData = scheduledData?.Select(ssv => new ScheduleStudentForView
                    {
                        SchoolId = ssv.sm.SchoolId,
                        TenantId = ssv.sm.TenantId,
                        StudentId = ssv.sm.StudentId,
                        StudentGuid = ssv.sm.StudentGuid,
                        FirstGivenName = ssv.sm.FirstGivenName,
                        LastFamilyName = ssv.sm.LastFamilyName,
                        AlternateId = ssv.sm.AlternateId,
                        StudentInternalId = ssv.sm.StudentInternalId,
                        AdmissionNumber = ssv.sm.AdmissionNumber,
                        RollNumber = ssv.sm.RollNumber,
                        GradeLevel = this.context?.Gradelevels.FirstOrDefault(c => c.TenantId == ssv.sm.TenantId && c.SchoolId == ssv.sm.SchoolId && c.GradeId == ssv.scs.GradeId)?.Title,
                        Section = this.context?.Sections.FirstOrDefault(c => c.TenantId == ssv.sm.TenantId && c.SchoolId == ssv.sm.SchoolId && c.SectionId == ssv.sm.SectionId)?.Name,
                        GradeId = ssv.scs.GradeId,
                        GradeScaleId = ssv.scs.GradeScaleId, /*this.context.Grade.FirstOrDefault(c => c.TenantId == ssv.sm.TenantId && c.SchoolId == ssv.sm.SchoolId && c.GradeId == ssv.scs.GradeId)?.GradeScaleId,*/
                        PhoneNumber = ssv.sm.MobilePhone,
                        Action = ssv.scs.IsDropped,
                        ScheduleDate = ssv.scs.EffectiveStartDate,
                        //StudentPhoto = (pageResult.ProfilePhoto == true) ? ssv.sm.StudentPhoto : null,
                        StudentPhoto = (pageResult.ProfilePhoto == true) ? ssv.sm.StudentThumbnailPhoto : null,
                        Dob = ssv.sm.Dob,
                        Gender = ssv.sm.Gender,
                        Race = ssv.sm.Race,
                        Ethnicity = ssv.sm.Ethnicity,
                        MaritalStatus = ssv.sm.MaritalStatus,
                        CountryOfBirth = ssv.sm.CountryOfBirth,
                        Nationality = ssv.sm.Nationality,
                        FirstLanguageId = ssv.sm.FirstLanguageId,
                        SecondLanguageId = ssv.sm.SecondLanguageId,
                        ThirdLanguageId = ssv.sm.ThirdLanguageId,
                        HomeAddressLineOne = ssv.sm.HomeAddressLineOne,
                        HomeAddressLineTwo = ssv.sm.HomeAddressLineTwo,
                        HomeAddressCountry = ssv.sm.HomeAddressCountry,
                        HomeAddressState = ssv.sm.HomeAddressState,
                        HomeAddressCity = ssv.sm.HomeAddressCity,
                        HomeAddressZip = ssv.sm.HomeAddressZip,
                        BusNo = ssv.sm.BusNo,
                        HomePhone = ssv.sm.HomePhone,
                        MobilePhone = ssv.sm.MobilePhone,
                        PersonalEmail = ssv.sm.PersonalEmail,
                        SchoolEmail = ssv.sm.SchoolEmail,
                        CourseSectionId = ssv.scs.CourseSectionId,
                        MiddleName = ssv.sm.MiddleName,
                        CreatedOn = ssv.scs.CreatedOn,
                        CreatedBy = ssv.scs.CreatedBy,
                        UpdatedOn = ssv.scs.UpdatedOn,
                        UpdatedBy = ssv.scs.UpdatedBy,
                        IsDropped = ssv.scs.IsDropped,
                        GradePostingEndDate = courseSectionData?.SchoolYears != null ? courseSectionData?.SchoolYears.PostEndDate : courseSectionData?.Semesters != null ? courseSectionData?.Semesters.PostEndDate : courseSectionData?.Quarters != null ? courseSectionData?.Quarters.PostEndDate : courseSectionData?.ProgressPeriods != null ? courseSectionData?.ProgressPeriods.PostEndDate : null,
                    }).GroupBy(f => f.StudentId).Select(g => g.First()).ToList();
                }

                //if (scheduledStudentData.Count() > 0)
                if (scheduledStudentData != null && scheduledStudentData.Any())
                {
                    if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                    {
                        transactionIQ = scheduledStudentData.AsQueryable();
                    }
                    else
                    {
                        if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                        {
                            string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;

                            //transactionIQ = scheduledStudentData.Where(x => x.FirstGivenName.ToLower().Contains(Columnvalue.ToLower()) ||
                            //x.LastFamilyName.ToLower().Contains(Columnvalue.ToLower()) ||
                            transactionIQ = scheduledStudentData.Where(x => x.FirstGivenName != null && x.FirstGivenName.Contains(Columnvalue) ||
                            x.LastFamilyName != null && x.LastFamilyName.Contains(Columnvalue) ||
                            (x.GradeLevel != null && x.GradeLevel.Contains(Columnvalue)) ||
                            (x.ScheduleDate != null && x.ScheduleDate.ToString() == Columnvalue) ||
                            (x.Section != null && x.Section.Contains(Columnvalue)) ||
                            (x.StudentInternalId != null && x.StudentInternalId.Contains(Columnvalue)) ||
                            (x.AlternateId != null && x.AlternateId.Contains(Columnvalue)) ||
                            (x.PhoneNumber != null && x.PhoneNumber.Contains(Columnvalue)) ||
                            (x.SchoolEmail != null && x.SchoolEmail.Contains(Columnvalue)) ||
                            (x.MobilePhone != null && x.MobilePhone.Contains(Columnvalue))).AsQueryable();
                        }
                        else
                        {
                            //transactionIQ = Utility.FilteredData(pageResult!.FilterParams, scheduledStudentData).AsQueryable();
                            transactionIQ = Utility.FilteredData(pageResult.FilterParams!, scheduledStudentData).AsQueryable();

                            //medical advance search
                            var studentGuids = transactionIQ.Select(s => s.StudentGuid).ToList();
                            if (studentGuids.Count > 0)
                            {
                                var filterStudentIds = Utility.MedicalAdvancedSearch(this.context!, pageResult.FilterParams!, pageResult.TenantId, pageResult.SchoolId, studentGuids);

                                if (filterStudentIds?.Count > 0)
                                {
                                    transactionIQ = transactionIQ.Where(x => filterStudentIds.Contains(x.StudentGuid));
                                }
                                else
                                {
                                    transactionIQ = null;
                                }
                            }
                        }
                        //transactionIQ = transactionIQ.Distinct();
                    }
                    if (transactionIQ != null)
                    {
                        if (pageResult.SortingModel != null)
                        {
                            switch (pageResult!.SortingModel!.SortColumn)
                            {
                                default:
                                    //transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn, pageResult.SortingModel.SortDirection);
                                    transactionIQ = Utility.Sort(transactionIQ, pageResult?.SortingModel?.SortColumn!, pageResult?.SortingModel?.SortDirection!);
                                    break;
                            }
                        }
                        //Advance Search for date range
                        if (pageResult?.DobStartDate != null && pageResult.DobEndDate != null)
                        {
                            var filterInDateRange = transactionIQ.Where(x => x.Dob >= pageResult.DobStartDate && x.Dob <= pageResult.DobEndDate);
                            if (filterInDateRange.Any())
                            {
                                transactionIQ = filterInDateRange;
                            }
                            else
                            {
                                transactionIQ = null;
                            }
                        }
                    }

                    //int totalCount = transactionIQ.Count();
                    int totalCount = transactionIQ != null ? transactionIQ.Count() : 0;
                    if (pageResult?.PageNumber > 0 && pageResult.PageSize > 0)
                    {
                        //transactionIQ = transactionIQ.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                        transactionIQ = transactionIQ?.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                    }
                    //scheduleStudentListView.scheduleStudentForView = transactionIQ.ToList();
                    scheduleStudentListView.scheduleStudentForView = transactionIQ!.ToList() ?? new();
                    scheduleStudentListView.TotalCount = totalCount;
                    scheduleStudentListView._failure = false;
                }
                else
                {
                    scheduleStudentListView.scheduleStudentForView = scheduledStudentData ?? new();
                    scheduleStudentListView._failure = true;
                    scheduleStudentListView._message = NORECORDFOUND;
                }

                scheduleStudentListView.TenantId = pageResult?.TenantId;
                scheduleStudentListView.SchoolId = pageResult?.SchoolId;
                scheduleStudentListView.CourseSectionId = pageResult?.CourseSectionId;
                scheduleStudentListView.StaffId = pageResult?.StaffId;
                scheduleStudentListView.AcademicYear = pageResult?.AcademicYear;
                scheduleStudentListView.PageNumber = pageResult?.PageNumber;
                scheduleStudentListView._pageSize = pageResult?.PageSize;
                scheduleStudentListView._tenantName = pageResult?._tenantName;
                scheduleStudentListView._token = pageResult?._token;
            }
            catch (Exception es)
            {
                scheduleStudentListView._failure = true;
                scheduleStudentListView._message = es.Message; ;
            }
            return scheduleStudentListView;

        }


        /// <summary>
        /// Group Drop For Scheduled Student
        /// </summary>
        /// <param name="scheduledStudentDropModel"></param>
        /// <returns></returns>
        //public ScheduledStudentDropModel GroupDropForScheduledStudent(ScheduledStudentDropModel scheduledStudentDropModel)
        //{
        //    try
        //    {
        //        List<StudentCoursesectionSchedule> studentCoursesectionScheduleList = new List<StudentCoursesectionSchedule>();
        //        var currentDate = DateTime.UtcNow.Date;

        //        if (scheduledStudentDropModel.studentCoursesectionScheduleList.Count > 0 )
        //        {
        //            if (!string.IsNullOrEmpty(scheduledStudentDropModel.StudentId.ToString()) && scheduledStudentDropModel.StudentId > 0)
        //            {
        //                foreach (var scheduledStudent in scheduledStudentDropModel.studentCoursesectionScheduleList)
        //                {
        //                    var studentData = this.context?.StudentCoursesectionSchedule.Include(c=>c.CourseSection).FirstOrDefault(x => x.SchoolId == scheduledStudentDropModel.SchoolId && x.TenantId == scheduledStudentDropModel.TenantId && x.StudentId == scheduledStudentDropModel.StudentId && x.CourseSectionId == scheduledStudent.CourseSectionId && x.IsDropped!=true);

        //                    //if (studentData != null)
        //                    if (studentData != null && studentData.CourseSection.DurationEndDate is not null)
        //                    {
        //                        if (studentData.CourseSection.DurationEndDate>= scheduledStudent.EffectiveDropDate && currentDate<= scheduledStudent.EffectiveDropDate)
        //                        {
        //                            if (scheduledStudent.EffectiveDropDate.Value.Date < studentData.CourseSection.DurationEndDate.Value.Date)
        //                            {
        //                                studentData.IsDropped = true;
        //                                studentData.EffectiveDropDate = scheduledStudent.EffectiveDropDate;
        //                            }
        //                            studentData.EffectiveStartDate = scheduledStudent.EffectiveStartDate;
        //                            studentData.UpdatedBy = scheduledStudentDropModel.UpdatedBy;
        //                            studentData.UpdatedOn = DateTime.UtcNow;
        //                            studentCoursesectionScheduleList.Add(studentData);
        //                        }
        //                        else
        //                        {
        //                            scheduledStudentDropModel._failure = true;
        //                            scheduledStudentDropModel._message = "Effective Drop Date Must Be Equal or Lower Than Course Section End Date And Equal or Greater Than Current Date";
        //                            return scheduledStudentDropModel;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        scheduledStudentDropModel._message = NORECORDFOUND;
        //                        scheduledStudentDropModel._failure = true;
        //                        return scheduledStudentDropModel;
        //                    }
        //                }
        //                scheduledStudentDropModel._message = "End/Drop Date Updated Successfully.";
        //            }
        //            else
        //            {
        //                if (scheduledStudentDropModel.EffectiveDropDate is null)
        //                {
        //                    return scheduledStudentDropModel;
        //                }
        //                if (!string.IsNullOrEmpty(scheduledStudentDropModel.EffectiveDropDate.ToString()) && scheduledStudentDropModel.EffectiveDropDate.Value.Date >= currentDate)
        //                {
        //                    foreach (var scheduledStudent in scheduledStudentDropModel.studentCoursesectionScheduleList)
        //                    {
        //                        var studentData = this.context?.StudentCoursesectionSchedule.Include(c=>c.CourseSection).FirstOrDefault(x => x.SchoolId == scheduledStudent.SchoolId && x.TenantId == scheduledStudent.TenantId && x.StudentId == scheduledStudent.StudentId && x.CourseSectionId == scheduledStudentDropModel.CourseSectionId);

        //                        if (studentData != null)
        //                        {
        //                            if (studentData.CourseSection.DurationEndDate>= scheduledStudentDropModel.EffectiveDropDate)
        //                            {
        //                                if (scheduledStudentDropModel.EffectiveDropDate.Value.Date < studentData.CourseSection.DurationEndDate.Value.Date)
        //                                {
        //                                    studentData.IsDropped = true;
        //                                    studentData.EffectiveDropDate = scheduledStudentDropModel.EffectiveDropDate;
        //                                }
        //                                //studentData.EffectiveStartDate = scheduledStudent.EffectiveStartDate;
        //                                studentData.UpdatedBy = scheduledStudentDropModel.UpdatedBy;
        //                                studentData.UpdatedOn = DateTime.UtcNow;
        //                                studentCoursesectionScheduleList.Add(studentData);
        //                            }
        //                            else
        //                            {
        //                                scheduledStudentDropModel._failure = true;
        //                                scheduledStudentDropModel._message = "Effective Drop Date Must Be Equal or Lower Than Course Section End Date";
        //                                return scheduledStudentDropModel;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            scheduledStudentDropModel._message = NORECORDFOUND;
        //                            scheduledStudentDropModel._failure = true;
        //                            return scheduledStudentDropModel;
        //                        }
        //                    }
        //                    scheduledStudentDropModel._message = "Selected students have been dropped from the course section.";
        //                }
        //                else
        //                {
        //                    scheduledStudentDropModel._message = "Effective Drop Date Must Be Equal or Greater Than Current Date";
        //                    scheduledStudentDropModel._failure = true;
        //                    return scheduledStudentDropModel;
        //                }
        //            }
        //            this.context?.StudentCoursesectionSchedule.UpdateRange(studentCoursesectionScheduleList);
        //            this.context?.SaveChanges();
        //            scheduledStudentDropModel._failure = false;
        //            //scheduledStudentDropModel._message = "Selected students have been dropped from the course section.";
        //        }
        //        else
        //        {
        //            scheduledStudentDropModel._message = "Select Atleast One Student";
        //            scheduledStudentDropModel._failure = true;
        //        }
        //    }
        //    catch (Exception es)
        //    {
        //        scheduledStudentDropModel._failure = true;
        //        scheduledStudentDropModel._message = es.Message;
        //    }
        //    return scheduledStudentDropModel;
        //}

        public ScheduledStudentDropModel GroupDropForScheduledStudent(ScheduledStudentDropModel scheduledStudentDropModel)
        {
            using var transaction = this.context?.Database.BeginTransaction();
            {
                try
                {
                    List<StudentCoursesectionSchedule> studentCoursesectionScheduleList = new List<StudentCoursesectionSchedule>();

                    List<StudentAttendance> studentAttendanceList = new List<StudentAttendance>();
                    List<StudentAttendanceComments> studentAttendanceCommentsList = new List<StudentAttendanceComments>();
                    List<StudentAttendanceHistory> studentAttendanceHistoryList = new List<StudentAttendanceHistory>();
                    List<StudentMissingAttendance> studentMissingAttendanceList = new List<StudentMissingAttendance>();
                    var currentDate = DateTime.UtcNow.Date;
                    bool? isPreviousDate = false;

                    if (scheduledStudentDropModel.studentCoursesectionScheduleList.Count > 0)
                    {
                        var reqPramInString = JsonConvert.SerializeObject(scheduledStudentDropModel);
                        var reqPram = JsonConvert.DeserializeObject<ScheduledStudentDropModel>(reqPramInString);

                        long? Id = 1;
                        var dataExits = this.context?.ScheduledJobs.Where(x => x.TenantId == scheduledStudentDropModel.TenantId);
                        if (dataExits?.Any() == true)
                        {
                            var scheduledJobData = this.context?.ScheduledJobs.Where(x => x.TenantId == scheduledStudentDropModel.TenantId).Max(x => x.JobId);
                            if (scheduledJobData != null)
                            {
                                Id = scheduledJobData + 1;
                            }
                        }

                        decimal? academicYear = Utility.GetCurrentAcademicYear(this.context!, scheduledStudentDropModel.TenantId, scheduledStudentDropModel.SchoolId);

                        var missingAttendancesList = this.context?.StudentMissingAttendances.Where(y => y.TenantId == scheduledStudentDropModel.TenantId && y.SchoolId == scheduledStudentDropModel.SchoolId).ToList();

                        //this blok for student 360 screen
                        if (!string.IsNullOrEmpty(scheduledStudentDropModel.StudentId.ToString()) && scheduledStudentDropModel.StudentId > 0)
                        {
                            var attendanceDataList = this.context?.StudentAttendance.Include(x => x.StudentAttendanceComments).Where(y => y.TenantId == scheduledStudentDropModel.TenantId && y.SchoolId == scheduledStudentDropModel.SchoolId && y.StudentId == scheduledStudentDropModel.StudentId).ToList();

                            var studentAttendanceHistoryDataList = this.context?.StudentAttendanceHistory.Where(y => y.TenantId == scheduledStudentDropModel.TenantId && y.SchoolId == scheduledStudentDropModel.SchoolId && y.StudentId == scheduledStudentDropModel.StudentId).ToList();


                            foreach (var scheduledStudent in scheduledStudentDropModel.studentCoursesectionScheduleList)
                            {
                                var studentData = this.context?.StudentCoursesectionSchedule.Include(c => c.StudentMaster).Include(c => c.CourseSection).FirstOrDefault(x => x.SchoolId == scheduledStudentDropModel.SchoolId && x.TenantId == scheduledStudentDropModel.TenantId && x.StudentId == scheduledStudentDropModel.StudentId && x.CourseSectionId == scheduledStudent.CourseSectionId && x.IsDropped != true);

                                var studentEnrollmentData = this.context?.StudentEnrollment.FirstOrDefault(x => x.SchoolId == scheduledStudentDropModel.SchoolId && x.TenantId == scheduledStudentDropModel.TenantId && x.StudentId == scheduledStudentDropModel.StudentId && x.IsActive == true);

                                if (studentData != null && studentData.CourseSection.DurationEndDate is not null && studentEnrollmentData != null)
                                {
                                    if (scheduledStudent.EffectiveDropDate == null)
                                    {
                                        scheduledStudent.EffectiveDropDate = studentData.CourseSection.DurationEndDate;//for sefty EffectiveDropDate null update we assign course section end date
                                    }

                                    if (scheduledStudent.EffectiveStartDate >= studentEnrollmentData.EnrollmentDate)
                                    {
                                        if (studentData.CourseSection.DurationEndDate >= scheduledStudent.EffectiveDropDate /*&& currentDate <= scheduledStudent.EffectiveDropDate*/ && studentData.CourseSection.DurationStartDate <= scheduledStudent.EffectiveDropDate)
                                        {
                                            if (scheduledStudent.EffectiveDropDate.Value.Date == currentDate)
                                            {
                                                //this blok for when user drop student instantly
                                                studentData.EffectiveDropDate = scheduledStudent.EffectiveDropDate;
                                                studentData.EffectiveStartDate = scheduledStudent.EffectiveStartDate;
                                                studentData.IsDropped = true;
                                                studentData.UpdatedBy = scheduledStudentDropModel.UpdatedBy;
                                                studentData.UpdatedOn = DateTime.UtcNow;
                                                studentCoursesectionScheduleList.Add(studentData);
                                            }
                                            else if (scheduledStudent.EffectiveDropDate.Value.Date < currentDate)
                                            {
                                                //this blok for when user drop student in previous date
                                                studentData.EffectiveDropDate = scheduledStudent.EffectiveDropDate;
                                                studentData.EffectiveStartDate = scheduledStudent.EffectiveStartDate;
                                                studentData.IsDropped = true;
                                                studentData.UpdatedBy = scheduledStudentDropModel.UpdatedBy;
                                                studentData.UpdatedOn = DateTime.UtcNow;
                                                studentCoursesectionScheduleList.Add(studentData);

                                                if (attendanceDataList?.Any() == true)
                                                {
                                                    var attendanceData = attendanceDataList.Where(x => x.CourseId == scheduledStudent.CourseId && x.CourseSectionId == scheduledStudent.CourseSectionId && x.AttendanceDate >= scheduledStudent.EffectiveDropDate).ToList();

                                                    if (attendanceData?.Any() == true)
                                                    {
                                                        studentAttendanceList.AddRange(attendanceData);

                                                        studentAttendanceCommentsList.AddRange(attendanceData.SelectMany(x => x.StudentAttendanceComments).ToList());
                                                    }
                                                }

                                                if (studentAttendanceHistoryDataList?.Any() == true)
                                                {
                                                    var studentAttendanceHistoryData = studentAttendanceHistoryDataList.Where(x => x.CourseId == scheduledStudent.CourseId && x.CourseSectionId == scheduledStudent.CourseSectionId && x.AttendanceDate >= scheduledStudent.EffectiveDropDate).ToList();

                                                    if (studentAttendanceHistoryData?.Any() == true)
                                                    {
                                                        studentAttendanceHistoryList.AddRange(studentAttendanceHistoryData);
                                                    }
                                                }

                                                //remove missing attendance
                                                var studentExistinSchedule = this.context?.StudentCoursesectionSchedule.FirstOrDefault(x => x.SchoolId == scheduledStudentDropModel.SchoolId && x.TenantId == scheduledStudentDropModel.TenantId && x.StudentId != scheduledStudentDropModel.StudentId && x.CourseSectionId == scheduledStudent.CourseSectionId && x.IsDropped != true);

                                                if (studentExistinSchedule == null)
                                                {
                                                    var ma = missingAttendancesList?.Where(x => x.CourseSectionId == scheduledStudent.CourseSectionId && x.MissingAttendanceDate >= scheduledStudent.EffectiveDropDate).ToList();
                                                    if (ma?.Any() == true)
                                                    {
                                                        studentMissingAttendanceList.AddRange(ma);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //this blok for when user drop student future date by scheduler
                                                studentData.EffectiveDropDate = scheduledStudent.EffectiveDropDate;
                                                studentData.EffectiveStartDate = scheduledStudent.EffectiveStartDate;
                                                studentData.UpdatedBy = scheduledStudentDropModel.UpdatedBy;
                                                studentData.UpdatedOn = DateTime.UtcNow;
                                                studentCoursesectionScheduleList.Add(studentData);

                                                //this block for add this req as a job
                                                reqPram!.studentCoursesectionScheduleList = reqPram.studentCoursesectionScheduleList.Where(x => x.CourseSectionId == scheduledStudent.CourseSectionId && x.EffectiveDropDate == scheduledStudent.EffectiveDropDate).ToList();

                                                var scheduledJob = new ScheduledJob
                                                {
                                                    TenantId = scheduledStudentDropModel.TenantId,
                                                    SchoolId = scheduledStudentDropModel.SchoolId,
                                                    JobId = (long)Id,
                                                    AcademicYear = academicYear,
                                                    JobTitle = "DropStudentfromScheduledCourseSections",
                                                    JobScheduleDate = scheduledStudent.EffectiveDropDate.Value.AddDays(1),
                                                    ApiTitle = "GroupDropForScheduledStudent",
                                                    ControllerPath = scheduledStudentDropModel._tenantName + "/StudentSchedule",
                                                    TaskJson = JsonConvert.SerializeObject(reqPram),
                                                    LastRunStatus = null,
                                                    LastRunTime = null,
                                                    IsActive = true,
                                                    CreatedBy = scheduledStudentDropModel.UpdatedBy,
                                                    CreatedOn = DateTime.UtcNow
                                                };
                                                this.context?.ScheduledJobs.Add(scheduledJob);
                                                Id++;
                                                reqPram = JsonConvert.DeserializeObject<ScheduledStudentDropModel>(reqPramInString);
                                            }
                                        }
                                        else
                                        {
                                            scheduledStudentDropModel._failure = true;
                                            scheduledStudentDropModel._message = "Effective drop date must be equal or lower than course section end date and equal or greater than course section start date";
                                            return scheduledStudentDropModel;
                                        }
                                    }
                                    else
                                    {
                                        scheduledStudentDropModel._failure = true;
                                        scheduledStudentDropModel._message = "Effective start date must be equal or greater than student enrollment start date";
                                        return scheduledStudentDropModel;
                                    }
                                }
                                else
                                {
                                    scheduledStudentDropModel._message = NORECORDFOUND;
                                    scheduledStudentDropModel._failure = true;
                                    return scheduledStudentDropModel;
                                }
                            }
                            scheduledStudentDropModel._message = "Updated successfully";
                        }
                        else //this blok for Group drop students screen
                        {
                            if (scheduledStudentDropModel.EffectiveDropDate is null)
                            {
                                return scheduledStudentDropModel;
                            }
                            if (!string.IsNullOrEmpty(scheduledStudentDropModel.EffectiveDropDate.ToString()) /*&& scheduledStudentDropModel.EffectiveDropDate.Value.Date >= currentDate*/)
                            {
                                var attendanceDataList = this.context?.StudentAttendance.Include(x => x.StudentAttendanceComments).Where(y => y.TenantId == scheduledStudentDropModel.TenantId && y.SchoolId == scheduledStudentDropModel.SchoolId && y.CourseSectionId == scheduledStudentDropModel.CourseSectionId).ToList();

                                var studentAttendanceHistoryDataList = this.context?.StudentAttendanceHistory.Where(y => y.TenantId == scheduledStudentDropModel.TenantId && y.SchoolId == scheduledStudentDropModel.SchoolId && y.CourseSectionId == scheduledStudentDropModel.CourseSectionId).ToList();

                                foreach (var scheduledStudent in scheduledStudentDropModel.studentCoursesectionScheduleList)
                                {
                                    var studentData = this.context?.StudentCoursesectionSchedule.Include(c => c.StudentMaster).Include(c => c.CourseSection).FirstOrDefault(x => x.SchoolId == scheduledStudent.SchoolId && x.TenantId == scheduledStudent.TenantId && x.StudentId == scheduledStudent.StudentId && x.CourseSectionId == scheduledStudentDropModel.CourseSectionId);

                                    if (studentData != null)
                                    {
                                        if (studentData.CourseSection.DurationEndDate >= scheduledStudentDropModel.EffectiveDropDate && studentData.CourseSection.DurationStartDate <= scheduledStudentDropModel.EffectiveDropDate)
                                        {
                                            if (currentDate == scheduledStudentDropModel.EffectiveDropDate.Value.Date)
                                            {
                                                //this blok for when user drop student instantly

                                                studentData.EffectiveDropDate = scheduledStudentDropModel.EffectiveDropDate;
                                                studentData.IsDropped = true;
                                                studentData.UpdatedBy = scheduledStudentDropModel.UpdatedBy;
                                                studentData.UpdatedOn = DateTime.UtcNow;
                                                studentCoursesectionScheduleList.Add(studentData);
                                            }
                                            else if (scheduledStudentDropModel.EffectiveDropDate.Value.Date < currentDate)
                                            {
                                                //this blok for when user drop student in previous date
                                                isPreviousDate = true;

                                                studentData.EffectiveDropDate = scheduledStudentDropModel.EffectiveDropDate;
                                                studentData.IsDropped = true;
                                                studentData.UpdatedBy = scheduledStudentDropModel.UpdatedBy;
                                                studentData.UpdatedOn = DateTime.UtcNow;
                                                studentCoursesectionScheduleList.Add(studentData);

                                                if (attendanceDataList?.Any() == true)
                                                {
                                                    var attendanceData = attendanceDataList.Where(x => x.StudentId == scheduledStudent.StudentId && x.AttendanceDate >= scheduledStudentDropModel.EffectiveDropDate).ToList();

                                                    if (attendanceData?.Any() == true)
                                                    {
                                                        studentAttendanceList.AddRange(attendanceData);

                                                        studentAttendanceCommentsList.AddRange(attendanceData.SelectMany(x => x.StudentAttendanceComments).ToList());
                                                    }
                                                }

                                                if (studentAttendanceHistoryDataList?.Any() == true)
                                                {
                                                    var studentAttendanceHistoryData = studentAttendanceHistoryDataList.Where(x => x.StudentId == scheduledStudent.StudentId && x.AttendanceDate >= scheduledStudentDropModel.EffectiveDropDate).ToList();

                                                    if (studentAttendanceHistoryData?.Any() == true)
                                                    {
                                                        studentAttendanceHistoryList.AddRange(studentAttendanceHistoryData);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //this blok for when user drop student future date by scheduler

                                                studentData.EffectiveDropDate = scheduledStudentDropModel.EffectiveDropDate;
                                                studentData.UpdatedBy = scheduledStudentDropModel.UpdatedBy;
                                                studentData.UpdatedOn = DateTime.UtcNow;
                                                studentCoursesectionScheduleList.Add(studentData);

                                                //this block for add this req as a job
                                                reqPram!.studentCoursesectionScheduleList = reqPram.studentCoursesectionScheduleList.Where(x => x.StudentId == scheduledStudent.StudentId).ToList();

                                                var scheduledJob = new ScheduledJob
                                                {
                                                    TenantId = scheduledStudentDropModel.TenantId,
                                                    SchoolId = scheduledStudentDropModel.SchoolId,
                                                    JobId = (long)Id,
                                                    AcademicYear = academicYear,
                                                    JobTitle = "GroupDropForScheduledStudent",
                                                    JobScheduleDate = scheduledStudentDropModel.EffectiveDropDate.Value.AddDays(1),
                                                    ApiTitle = "GroupDropForScheduledStudent",
                                                    ControllerPath = scheduledStudentDropModel._tenantName + "/StudentSchedule",
                                                    TaskJson = JsonConvert.SerializeObject(reqPram),
                                                    LastRunStatus = null,
                                                    LastRunTime = null,
                                                    IsActive = true,
                                                    CreatedBy = scheduledStudentDropModel.UpdatedBy,
                                                    CreatedOn = DateTime.UtcNow
                                                };
                                                this.context?.ScheduledJobs.Add(scheduledJob);
                                                Id++;
                                                reqPram = JsonConvert.DeserializeObject<ScheduledStudentDropModel>(reqPramInString);
                                            }
                                        }
                                        else
                                        {
                                            scheduledStudentDropModel._failure = true;
                                            scheduledStudentDropModel._message = "Effective drop date must be equal or lower than course section end date and equal or greater than course section start date";
                                            return scheduledStudentDropModel;
                                        }
                                    }
                                    else
                                    {
                                        scheduledStudentDropModel._message = NORECORDFOUND;
                                        scheduledStudentDropModel._failure = true;
                                        return scheduledStudentDropModel;
                                    }
                                }

                                scheduledStudentDropModel._message = "Selected students have been dropped from the course section.";
                            }
                            else
                            {
                                scheduledStudentDropModel._message = "Please pass effective drop date";
                                scheduledStudentDropModel._failure = true;
                                return scheduledStudentDropModel;
                            }
                        }

                        this.context?.StudentAttendanceComments.RemoveRange(studentAttendanceCommentsList);
                        this.context?.StudentAttendanceHistory.RemoveRange(studentAttendanceHistoryList);
                        this.context?.StudentAttendance.RemoveRange(studentAttendanceList);
                        this.context?.StudentMissingAttendances.RemoveRange(studentMissingAttendanceList);
                        this.context?.StudentCoursesectionSchedule.UpdateRange(studentCoursesectionScheduleList);

                        this.context?.SaveChanges();

                        if (isPreviousDate == true)
                        {
                            //remove missing attendance
                            var studentExistinSchedule = this.context?.StudentCoursesectionSchedule.FirstOrDefault(x => x.SchoolId == scheduledStudentDropModel.SchoolId && x.TenantId == scheduledStudentDropModel.TenantId && x.CourseSectionId == scheduledStudentDropModel.CourseSectionId && x.IsDropped != true);

                            if (studentExistinSchedule == null)
                            {
                                var ma = missingAttendancesList?.Where(x => x.CourseSectionId == scheduledStudentDropModel.CourseSectionId && x.MissingAttendanceDate >= scheduledStudentDropModel.EffectiveDropDate).ToList();
                                if (ma?.Any() == true)
                                {
                                    this.context?.StudentMissingAttendances.RemoveRange(ma);
                                    this.context?.SaveChanges();
                                }
                            }
                        }

                        transaction?.Commit();
                        scheduledStudentDropModel._failure = false;
                    }
                    else
                    {
                        scheduledStudentDropModel._message = "Select atleast one student";
                        scheduledStudentDropModel._failure = true;
                    }
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    scheduledStudentDropModel._failure = true;
                    scheduledStudentDropModel._message = es.Message;
                }
            }
            return scheduledStudentDropModel;
        }

        /// <summary>
        /// Student Schedule View Report
        /// </summary>
        /// <param name="studentScheduleReportViewModel"></param>
        /// <returns></returns>
        public StudentScheduleReportViewModel StudentScheduleReport(StudentScheduleReportViewModel studentScheduleReportViewModel)
        {
            StudentScheduleReportViewModel studentScheduleReportView = new StudentScheduleReportViewModel();
            try
            {
                var scheduleReport = this.context?.StudentScheduleView.Where(x => x.TenantId == studentScheduleReportViewModel.TenantId && x.SchoolId == studentScheduleReportViewModel.SchoolId).ToPivotTable(
                    item => item.CourseSectionName,
                    item => new { item.StudentId, item.StudentName, item.StudentInternalId },
                    items => items.Any() ? items.First().Scheduled + " | " + items.First().ConflictComment : null);

                studentScheduleReportView.ScheduleReport = scheduleReport;

                studentScheduleReportView.TenantId = studentScheduleReportViewModel.TenantId;
                studentScheduleReportView.SchoolId = studentScheduleReportViewModel.SchoolId;
            }
            catch (Exception es)
            {
                studentScheduleReportViewModel._failure = true;
                studentScheduleReportViewModel._message = es.Message;
            }

            return studentScheduleReportView;
        }

        /// <summary>
        /// Delete Student Schedule View Report
        /// </summary>
        /// <param name="studentCourseSectionScheduleAddViewModel"></param>
        /// <returns></returns>
        public StudentCourseSectionScheduleAddViewModel DeleteStudentScheduleReport(StudentCourseSectionScheduleAddViewModel studentCourseSectionScheduleAddViewModel)
        {
            try
            {
                var studentScheduleViewData = this.context?.StudentScheduleView.Where(e => e.SchoolId == studentCourseSectionScheduleAddViewModel.SchoolId && e.TenantId == studentCourseSectionScheduleAddViewModel.TenantId).ToList();

                if (studentScheduleViewData?.Count > 0)
                {
                    this.context?.StudentScheduleView.RemoveRange(studentScheduleViewData);
                    this.context?.SaveChanges();
                    studentCourseSectionScheduleAddViewModel._failure = false;
                    studentCourseSectionScheduleAddViewModel._message = "Student Schedule Report deleted successfullyy";
                }
                else
                {
                    studentCourseSectionScheduleAddViewModel._message = NORECORDFOUND;
                    studentCourseSectionScheduleAddViewModel._failure = true;
                }
            }
            catch (Exception es)
            {
                studentCourseSectionScheduleAddViewModel._failure = true;
                studentCourseSectionScheduleAddViewModel._message = es.Message;
            }
            return studentCourseSectionScheduleAddViewModel;
        }
        /// <summary>
        /// Scheduled Course Section For Student360
        /// </summary>
        /// <param name="student360ScheduleCourseSectionListViewModel"></param>
        /// <returns></returns>
        public Student360ScheduleCourseSectionListViewModel ScheduleCoursesForStudent360(Student360ScheduleCourseSectionListViewModel student360ScheduleCourseSectionListViewModel)
        {
            Student360ScheduleCourseSectionListViewModel student360ScheduleCourseSectionListView = new Student360ScheduleCourseSectionListViewModel();
            List<Student360ScheduleCourseSectionForView> student360ScheduleCourseSectionForViewList = new List<Student360ScheduleCourseSectionForView>();
            try
            {
                var StudentData = this.context?.StudentCoursesectionSchedule.Include(v => v.CourseSection.Course).Include(x => x.CourseSection).ThenInclude(y => y.SchoolYears).ThenInclude(s => s!.Semesters).ThenInclude(e=>e.Quarters).ThenInclude(c => c.StaffCoursesectionSchedule).Include(e=>e.StudentAttendance).Where(x => (student360ScheduleCourseSectionListViewModel.IsDropped==false)?  x.SchoolId == student360ScheduleCourseSectionListViewModel.SchoolId && x.TenantId == student360ScheduleCourseSectionListViewModel.TenantId && x.StudentId == student360ScheduleCourseSectionListViewModel.StudentId && x.IsDropped != true : x.SchoolId == student360ScheduleCourseSectionListViewModel.SchoolId && x.TenantId == student360ScheduleCourseSectionListViewModel.TenantId && x.StudentId == student360ScheduleCourseSectionListViewModel.StudentId).ToList();

                var staffCoursesectionScheduleData = this.context?.StaffCoursesectionSchedule.Include(d => d.StaffMaster).Select(v => new StaffCoursesectionSchedule() { SchoolId=v.SchoolId,TenantId=v.TenantId,CourseSectionId=v.CourseSectionId,IsDropped=v.IsDropped,StaffMaster=new StaffMaster() {
                    FirstGivenName=v.StaffMaster.FirstGivenName,
                    MiddleName=v.StaffMaster.MiddleName,
                    LastFamilyName=v.StaffMaster.LastFamilyName} 
                     }).ToList();
                var studentFinalGradeData = this.context?.StudentFinalGrade;

                //if (StudentData.Count > 0)
                if (StudentData?.Count > 0)
                {
                    foreach (var Student in StudentData)
                    {
                        //var staffData = staffCoursesectionScheduleData.Where(c => c.TenantId == Student.TenantId && c.SchoolId == Student.SchoolId && c.CourseSectionId == Student.CourseSectionId && c.IsDropped != true).Select(v => v.StaffMaster).ToList();
                        var staffData = staffCoursesectionScheduleData?.Where(c => c.TenantId == Student.TenantId && c.SchoolId == Student.SchoolId && c.CourseSectionId == Student.CourseSectionId && c.IsDropped != true).Select(v => v.StaffMaster).ToList();

                        //var studentInputFinalGradeData = studentFinalGradeData.FirstOrDefault(b => b.TenantId == Student.TenantId && b.SchoolId == Student.SchoolId && b.StudentId == Student.StudentId && b.CourseSectionId == Student.CourseSectionId);
                        var studentInputFinalGradeData = studentFinalGradeData?.FirstOrDefault(b => b.TenantId == Student.TenantId && b.SchoolId == Student.SchoolId && b.StudentId == Student.StudentId && b.CourseSectionId == Student.CourseSectionId);

                        var Student360ScheduleCourseSectionData = new Student360ScheduleCourseSectionForView()
                        {
                            TenantId = Student.TenantId,
                            SchoolId = Student.SchoolId,
                            CourseId = Student.CourseId,
                            CourseSectionId = Student.CourseSectionId,
                            CourseTitle = Student.CourseSection?.Course.CourseTitle,
                            CourseSectionName = Student.CourseSectionName,
                            CourseSectionDurationStartDate = Student.CourseSection?.DurationStartDate,
                            CourseSectionDurationEndDate = Student.CourseSection?.DurationEndDate,
                            YrMarkingPeriodId = Student.CourseSection?.YrMarkingPeriodId,
                            SmstrMarkingPeriodId = Student.CourseSection?.SmstrMarkingPeriodId,
                            QtrMarkingPeriodId = Student.CourseSection?.QtrMarkingPeriodId,
                            PrgrsprdMarkingPeriodId = Student.CourseSection?.PrgrsprdMarkingPeriodId,
                            EnrolledDate = Student.CreatedOn,
                            EffectiveStartDate = Student.EffectiveStartDate,
                            EffectiveDropDate = (Student.EffectiveDropDate == null) ? Student.CourseSection?.DurationEndDate : Student.EffectiveDropDate,
                            DayOfWeek = (Student.CourseSection?.StaffCoursesectionSchedule.Count > 0) ? Student.CourseSection.StaffCoursesectionSchedule.FirstOrDefault()?.MeetingDays : null,
                            staffMasterList = /*(staffData.Count > 0) ? staffData : null*/ staffData!,
                            CreatedBy = Student.CreatedBy,
                            CreatedOn = Student.CreatedOn,
                            UpdatedBy = Student.UpdatedBy,
                            UpdatedOn = Student.UpdatedOn,
                            IsDropped = Student.IsDropped,
                            IsAssociationship = (Student.StudentAttendance.Count > 0 || studentInputFinalGradeData != null) ? true : false,
                            studentAttendanceList = /*(Student.StudentAttendance.Count > 0)? Student.StudentAttendance.ToList():null*/ Student.StudentAttendance.ToList(),
                            SchoolYears = Student.CourseSection?.SchoolYears,
                            Semesters = Student.CourseSection?.Semesters,
                            Quarters = Student.CourseSection?.Quarters,
                            AttendanceTaken = Student.CourseSection?.AttendanceTaken
                        };

                        if (Student.CourseSection?.ScheduleType == "Fixed Schedule (1)")
                        {
                            var fixedData = this.context?.CourseFixedSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).FirstOrDefault(c => c.SchoolId == Student.SchoolId && c.TenantId == Student.TenantId && c.CourseSectionId == Student.CourseSectionId);

                            if (fixedData != null)
                            {
                                fixedData.BlockPeriod!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                fixedData.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                fixedData.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                fixedData.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                fixedData.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>();
                                fixedData.Rooms!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                fixedData.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                fixedData.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                fixedData.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                Student360ScheduleCourseSectionData.courseFixedSchedule = fixedData;
                                Student360ScheduleCourseSectionData.DayOfWeek = Student.CourseSection?.MeetingDays;
                            }
                        }
                        if (Student.CourseSection?.ScheduleType == "Variable Schedule (2)")
                        {
                            var variableData = this.context?.CourseVariableSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => c.SchoolId == Student.SchoolId && c.TenantId == Student.TenantId && c.CourseSectionId == Student.CourseSectionId).ToList();

                            if (variableData !=null && variableData.Any())
                            {
                                //variableData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = null!; x.BlockPeriod.CourseVariableSchedule = null; x.BlockPeriod.CourseCalendarSchedule = null; x.BlockPeriod.CourseBlockSchedule = null; x.BlockPeriod.StudentAttendance = null; x.Rooms.CourseFixedSchedule = null; x.Rooms.CourseVariableSchedule = null; x.Rooms.CourseCalendarSchedule = null; x.Rooms.CourseBlockSchedule = null; });
                                variableData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });

                                Student360ScheduleCourseSectionData.courseVariableScheduleList = variableData;
                            }
                        }
                        if (Student.CourseSection?.ScheduleType == "Calendar Schedule (3)")
                        {
                            var calenderData = this.context?.CourseCalendarSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => c.SchoolId == Student.SchoolId && c.TenantId == Student.TenantId && c.CourseSectionId == Student.CourseSectionId).ToList();

                            if (calenderData != null && calenderData.Any())
                            {
                                //calenderData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = null; x.BlockPeriod.CourseVariableSchedule = null; x.BlockPeriod.CourseCalendarSchedule = null; x.BlockPeriod.CourseBlockSchedule = null; x.BlockPeriod.StudentAttendance = null; x.Rooms.CourseFixedSchedule = null; x.Rooms.CourseVariableSchedule = null; x.Rooms.CourseCalendarSchedule = null; x.Rooms.CourseBlockSchedule = null; });
                                calenderData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });
                                Student360ScheduleCourseSectionData.courseCalendarScheduleList = calenderData;
                            }
                        }
                        if (Student.CourseSection?.ScheduleType == "Block Schedule (4)")
                        {
                            var blockData = this.context?.CourseBlockSchedule.Include(v => v.Block).Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => c.SchoolId == Student.SchoolId && c.TenantId == Student.TenantId && c.CourseSectionId == Student.CourseSectionId).ToList();

                            if (blockData!= null && blockData.Any())
                            {
                                //blockData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = null; x.BlockPeriod.CourseVariableSchedule = null; x.BlockPeriod.CourseCalendarSchedule = null; x.BlockPeriod.CourseBlockSchedule = null; x.BlockPeriod.StudentAttendance = null; x.Rooms.CourseFixedSchedule = null; x.Rooms.CourseVariableSchedule = null; x.Rooms.CourseCalendarSchedule = null; x.Rooms.CourseBlockSchedule = null; });
                                blockData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });

                                Student360ScheduleCourseSectionData.courseBlockScheduleList = blockData;

                                var bellScheduleList = new List<BellSchedule>();
                                foreach (var block in blockData)
                                {
                                    var bellScheduleData = this.context?.BellSchedule.Where(c => c.SchoolId == Student.SchoolId && c.TenantId == Student.TenantId && c.BlockId == block.BlockId && c.BellScheduleDate >= Student.CourseSection.DurationStartDate && c.BellScheduleDate <= Student.CourseSection.DurationEndDate).ToList();
                                    if (bellScheduleData != null )
                                    {
                                        bellScheduleList.AddRange(bellScheduleData);
                                    }
                                    
                                }

                                Student360ScheduleCourseSectionData.bellScheduleList = bellScheduleList;
                            }
                        }

                        if (Student360ScheduleCourseSectionData.studentAttendanceList.ToList().Count() > 0)
                        {
                            Student360ScheduleCourseSectionData.studentAttendanceList.ForEach(b =>
                            {
                                b.StudentCoursesectionSchedule = new();
                                b.StaffCoursesectionSchedule = new();
                                b.BlockPeriod = new();
                                //b.BlockPeriod.CourseFixedSchedule = null;
                                //b.BlockPeriod.CourseVariableSchedule = null; 
                                //b.BlockPeriod.CourseCalendarSchedule = null;
                                //b.BlockPeriod.CourseBlockSchedule = null;                               
                                //b.BlockPeriod.StudentAttendance = null; 
                                //b.BlockPeriod.SchoolMaster = null;                               
                            });
                        }
                        if (Student360ScheduleCourseSectionData != null)
                        {
                            if (Student360ScheduleCourseSectionData.SchoolYears != null)
                            {
                                Student360ScheduleCourseSectionData.SchoolYears.CourseSection = new HashSet<CourseSection>();
                               // Student360ScheduleCourseSectionData.SchoolYears.HonorRolls = new HashSet<HonorRolls>();
                                Student360ScheduleCourseSectionData.SchoolYears.Semesters = new HashSet<Semesters>();
                                Student360ScheduleCourseSectionData.SchoolYears.StaffCoursesectionSchedule = new HashSet<StaffCoursesectionSchedule>();
                                Student360ScheduleCourseSectionData.SchoolYears.StudentEffortGradeMaster = new HashSet<StudentEffortGradeMaster>();
                                Student360ScheduleCourseSectionData.SchoolYears.StudentFinalGrade = new HashSet<StudentFinalGrade>();
                            }
                            if (Student360ScheduleCourseSectionData.Quarters != null)
                            {
                                Student360ScheduleCourseSectionData.Quarters.CourseSection = new HashSet<CourseSection>();
                                Student360ScheduleCourseSectionData.Quarters.Semesters = new();
                                Student360ScheduleCourseSectionData.Quarters.StaffCoursesectionSchedule = new HashSet<StaffCoursesectionSchedule>();
                                Student360ScheduleCourseSectionData.Quarters.StudentEffortGradeMaster = new HashSet<StudentEffortGradeMaster>();
                                Student360ScheduleCourseSectionData.Quarters.StudentFinalGrade = new HashSet<StudentFinalGrade>();
                                Student360ScheduleCourseSectionData.Quarters.ProgressPeriods = new HashSet<ProgressPeriods>();
                            }
                            if (Student360ScheduleCourseSectionData.Semesters != null)
                            {
                                Student360ScheduleCourseSectionData.Semesters.CourseSection = new HashSet<CourseSection>();
                                Student360ScheduleCourseSectionData.Semesters.Quarters = new HashSet<Quarters>();
                                Student360ScheduleCourseSectionData.Semesters.StaffCoursesectionSchedule = new HashSet<StaffCoursesectionSchedule>();
                                Student360ScheduleCourseSectionData.Semesters.StudentEffortGradeMaster = new HashSet<StudentEffortGradeMaster>();
                                Student360ScheduleCourseSectionData.Semesters.StudentFinalGrade = new HashSet<StudentFinalGrade>();
                                Student360ScheduleCourseSectionData.Semesters.SchoolYears = new();
                            }

                        }
                        if (Student360ScheduleCourseSectionData != null)
                        {
                            student360ScheduleCourseSectionForViewList.Add(Student360ScheduleCourseSectionData);
                        }
                        
                    }

                    student360ScheduleCourseSectionForViewList.ForEach(c =>
                    {
                        c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, student360ScheduleCourseSectionListViewModel.TenantId, c.CreatedBy);
                        c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, student360ScheduleCourseSectionListViewModel.TenantId, c.UpdatedBy);
                    });

                    student360ScheduleCourseSectionListView.scheduleCourseSectionForView = student360ScheduleCourseSectionForViewList;
                    student360ScheduleCourseSectionListView._failure = false;
                    student360ScheduleCourseSectionListView.SchoolId = student360ScheduleCourseSectionListViewModel.SchoolId;
                    student360ScheduleCourseSectionListView.TenantId = student360ScheduleCourseSectionListViewModel.TenantId;
                    student360ScheduleCourseSectionListView.StudentId = student360ScheduleCourseSectionListViewModel.StudentId;
                    student360ScheduleCourseSectionListView.IsDropped = student360ScheduleCourseSectionListViewModel.IsDropped;
                    student360ScheduleCourseSectionListView._tenantName = student360ScheduleCourseSectionListViewModel._tenantName;
                    student360ScheduleCourseSectionListView._token = student360ScheduleCourseSectionListViewModel._token;
                    student360ScheduleCourseSectionListView._userName = student360ScheduleCourseSectionListViewModel._userName;
                }
                else
                {
                    student360ScheduleCourseSectionListView._failure = true;
                    student360ScheduleCourseSectionListView._message = NORECORDFOUND;
                    student360ScheduleCourseSectionListView.SchoolId = student360ScheduleCourseSectionListViewModel.SchoolId;
                    student360ScheduleCourseSectionListView.TenantId = student360ScheduleCourseSectionListViewModel.TenantId;
                    student360ScheduleCourseSectionListView.StudentId = student360ScheduleCourseSectionListViewModel.StudentId;
                    student360ScheduleCourseSectionListView.IsDropped = student360ScheduleCourseSectionListViewModel.IsDropped;
                    student360ScheduleCourseSectionListView._tenantName = student360ScheduleCourseSectionListViewModel._tenantName;
                    student360ScheduleCourseSectionListView._token = student360ScheduleCourseSectionListViewModel._token;
                    student360ScheduleCourseSectionListView._userName = student360ScheduleCourseSectionListViewModel._userName;
                }
            }
            catch (Exception es)
            {
                student360ScheduleCourseSectionListView._failure = true;
                student360ScheduleCourseSectionListView._message = es.Message;
                student360ScheduleCourseSectionListView.SchoolId = student360ScheduleCourseSectionListViewModel.SchoolId;
                student360ScheduleCourseSectionListView.TenantId = student360ScheduleCourseSectionListViewModel.TenantId;
                student360ScheduleCourseSectionListView.StudentId = student360ScheduleCourseSectionListViewModel.StudentId;
                student360ScheduleCourseSectionListView.IsDropped = student360ScheduleCourseSectionListViewModel.IsDropped;
                student360ScheduleCourseSectionListView._tenantName = student360ScheduleCourseSectionListViewModel._tenantName;
                student360ScheduleCourseSectionListView._token = student360ScheduleCourseSectionListViewModel._token;
                student360ScheduleCourseSectionListView._userName = student360ScheduleCourseSectionListViewModel._userName;
            }            
            return student360ScheduleCourseSectionListView;
        }

        /// <summary>
        /// Drop Scheduled Course Section For Student360
        /// </summary>
        /// <param name="scheduledStudentDropModel"></param>
        /// <returns></returns>
        public ScheduledStudentDropModel DropScheduledCourseSectionForStudent360(ScheduledStudentDropModel scheduledStudentDropModel)
        {
            try
            {
                var studentCourseSectionScheduleData = this.context?.StudentCoursesectionSchedule.Include(e => e.StudentAttendance).FirstOrDefault(r => r.TenantId == scheduledStudentDropModel.TenantId && r.SchoolId == scheduledStudentDropModel.SchoolId && r.CourseSectionId == scheduledStudentDropModel.CourseSectionId && r.StudentId == scheduledStudentDropModel.StudentId && r.IsDropped != true);

                //        var studentCourseSectionScheduleData = this.context?.StudentCoursesectionSchedule.
                //Join(this.context?.StudentFinalGrade, u => u.StudentId, uir => uir.StudentId,
                //(u, uir) => new { u, uir }).Join(this.context?.StudentAttendance, st => st.u.StudentId, SA => SA.StudentId,
                //(st, SA) => new { st, SA }).Where(c =>c.st.u.SchoolId == scheduledStudentDropModel.SchoolId && c.st.uir.SchoolId == scheduledStudentDropModel.SchoolId && c.SA.SchoolId == scheduledStudentDropModel.SchoolId && c.st.u.TenantId == scheduledStudentDropModel.TenantId && c.st.uir.TenantId == scheduledStudentDropModel.TenantId && c.SA.TenantId == scheduledStudentDropModel.TenantId && c.st.u.StudentId == scheduledStudentDropModel.StudentId && c.st.uir.StudentId == scheduledStudentDropModel.StudentId && c.SA.StudentId == scheduledStudentDropModel.StudentId && c.st.u.CourseSectionId == scheduledStudentDropModel.CourseSectionId && c.st.uir.CourseSectionId == scheduledStudentDropModel.CourseSectionId && c.SA.CourseSectionId == scheduledStudentDropModel.CourseSectionId)
                //.Select(m => new
                // {
                //    m.st.u,m.st.uir,m.SA
                // }).FirstOrDefault();

                if (studentCourseSectionScheduleData!=null)
                {
                   var studentFinalGradeData= this.context?.StudentFinalGrade.FirstOrDefault( r => r.TenantId == scheduledStudentDropModel.TenantId && r.SchoolId == scheduledStudentDropModel.SchoolId && r.CourseSectionId == scheduledStudentDropModel.CourseSectionId && r.StudentId == scheduledStudentDropModel.StudentId);


                    if (studentCourseSectionScheduleData.StudentAttendance.Count > 0|| studentFinalGradeData !=null)
                    {
                        scheduledStudentDropModel._failure = true;
                        scheduledStudentDropModel._message = "Deletion Disallowed Due To Association.";
                    }
                    else
                    {
                        studentCourseSectionScheduleData.IsDropped = true;
                        studentCourseSectionScheduleData.EffectiveDropDate = DateTime.UtcNow;
                        studentCourseSectionScheduleData.UpdatedBy = scheduledStudentDropModel.UpdatedBy;
                        studentCourseSectionScheduleData.UpdatedOn = DateTime.UtcNow;
                        //this.context.StudentCoursesectionSchedule.Update(studentCourseSectionScheduleData);
                        this.context?.StudentCoursesectionSchedule.Update(studentCourseSectionScheduleData);
                        this.context?.SaveChanges();
                        scheduledStudentDropModel._failure = false;
                        scheduledStudentDropModel._message = "Drop Course Section Successfully.";
                    }                    
                }
                else
                {
                    scheduledStudentDropModel._failure = true;
                    scheduledStudentDropModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                scheduledStudentDropModel._failure = true;
                scheduledStudentDropModel._message = es.Message;
            }
            return scheduledStudentDropModel;
        }

        /// <summary>
        /// Scheduled Course Section List For Student360
        /// </summary>
        /// <param name="student360ScheduleCourseSectionListViewModel"></param>
        /// <returns></returns>
        public Student360ScheduleCourseSectionListViewModel ScheduleCourseSectionListForStudent360(Student360ScheduleCourseSectionListViewModel student360ScheduleCourseSectionListViewModel)
        {
            Student360ScheduleCourseSectionListViewModel student360ScheduleCourseSectionListView = new Student360ScheduleCourseSectionListViewModel();
            List<Student360ScheduleCourseSectionForView> student360ScheduleCourseSectionForViewList = new List<Student360ScheduleCourseSectionForView>();
            try
            {
                //var studentdata = this.context?.CourseSection.Where(r => r.SchoolId == student360ScheduleCourseSectionListViewModel.SchoolId && r.TenantId == student360ScheduleCourseSectionListViewModel.TenantId && r.DurationStartDate.Value.Date >= student360ScheduleCourseSectionListViewModel.DurationStartDate.Value.Date && r.DurationEndDate.Value.Date <= student360ScheduleCourseSectionListViewModel.DurationEndDate.Value.Date).Select(e => e.CourseSectionId).ToList();

                var studentdata = this.context?.CourseSection.Where(r => r.SchoolId == student360ScheduleCourseSectionListViewModel.SchoolId && r.TenantId == student360ScheduleCourseSectionListViewModel.TenantId &&((student360ScheduleCourseSectionListViewModel.DurationStartDate!.Value.Date >= r.DurationStartDate.Value.Date && student360ScheduleCourseSectionListViewModel.DurationStartDate.Value.Date <= r.DurationEndDate.Value.Date)||(student360ScheduleCourseSectionListViewModel.DurationEndDate!.Value.Date >= r.DurationStartDate.Value.Date && student360ScheduleCourseSectionListViewModel.DurationEndDate.Value.Date <= r.DurationEndDate))).Select(e => e.CourseSectionId).ToList();

                //if (studentdata.ToList().Count > 0)
                if (studentdata?.ToList()!= null && studentdata.ToList().Any())
                {
                    var studentCourseSectionScheduleData = this.context?.StudentCoursesectionSchedule.Include(o => o.CourseSection).Include(c=>c.StudentAttendance).ThenInclude(v=>v.StudentAttendanceComments).Include(p=>p.CourseSection.SchoolCalendars).Where(e => e.TenantId == student360ScheduleCourseSectionListViewModel.TenantId && e.SchoolId == student360ScheduleCourseSectionListViewModel.SchoolId && e.StudentId== student360ScheduleCourseSectionListViewModel.StudentId && (studentdata == null || (studentdata.Contains(e.CourseSectionId))) && e.IsDropped !=true).ToList();

                    var allBlockData = this.context?.Block.Where(x => x.SchoolId == student360ScheduleCourseSectionListViewModel.SchoolId && x.TenantId == student360ScheduleCourseSectionListViewModel.TenantId).ToList();

                    if (studentCourseSectionScheduleData!=null && studentCourseSectionScheduleData.Any())
                    {
                        var attendanceCategoriesData = this.context?.AttendanceCodeCategories.Include(c=>c.AttendanceCode).Where(x => x.SchoolId == student360ScheduleCourseSectionListViewModel.SchoolId && x.TenantId == student360ScheduleCourseSectionListViewModel.TenantId).ToList();

                        foreach (var studentCourseSectionSchedule in studentCourseSectionScheduleData)
                        {
                            var Student360ScheduleCourseSection = new Student360ScheduleCourseSectionForView()
                            {
                                TenantId = studentCourseSectionSchedule.TenantId,
                                SchoolId = studentCourseSectionSchedule.SchoolId,
                                CourseId = studentCourseSectionSchedule.CourseId,
                                CourseSectionId = studentCourseSectionSchedule.CourseSectionId,
                                CourseSectionName = studentCourseSectionSchedule.CourseSectionName,
                                CourseSectionDurationStartDate = studentCourseSectionSchedule.CourseSection?.DurationStartDate,
                                CourseSectionDurationEndDate = studentCourseSectionSchedule.CourseSection?.DurationEndDate,
                                YrMarkingPeriodId = studentCourseSectionSchedule.CourseSection?.YrMarkingPeriodId,
                                SmstrMarkingPeriodId = studentCourseSectionSchedule.CourseSection?.SmstrMarkingPeriodId,
                                QtrMarkingPeriodId = studentCourseSectionSchedule.CourseSection?.QtrMarkingPeriodId,
                                PrgrsprdMarkingPeriodId= studentCourseSectionSchedule.CourseSection?.PrgrsprdMarkingPeriodId,
                                EnrolledDate = studentCourseSectionSchedule.CreatedOn,
                                EffectiveDropDate = (studentCourseSectionSchedule.EffectiveDropDate == null) ? studentCourseSectionSchedule.CourseSection?.DurationEndDate : studentCourseSectionSchedule.EffectiveDropDate,
                                CreatedBy = studentCourseSectionSchedule.CreatedBy,
                                CreatedOn = studentCourseSectionSchedule.CreatedOn,
                                UpdatedBy = studentCourseSectionSchedule.UpdatedBy,
                                UpdatedOn = studentCourseSectionSchedule.UpdatedOn,
                                IsDropped = studentCourseSectionSchedule.IsDropped,
                                AttendanceTaken = studentCourseSectionSchedule.CourseSection?.AttendanceTaken,
                                studentAttendanceList = (studentCourseSectionSchedule.StudentAttendance.Count() > 0) ? studentCourseSectionSchedule.StudentAttendance.Where(x => x.AttendanceDate.Date >= student360ScheduleCourseSectionListViewModel.DurationStartDate!.Value.Date && x.AttendanceDate.Date <= student360ScheduleCourseSectionListViewModel.DurationEndDate!.Value.Date).ToList() : studentCourseSectionSchedule.StudentAttendance.ToList(),
                                //SchoolYears = Student.CourseSection.SchoolYears,
                                //Semesters = Student.CourseSection.Semesters,
                                //Quarters = Student.CourseSection.Quarters
                                AttendanceCodeCategories = attendanceCategoriesData?.FirstOrDefault(z => z.AttendanceCategoryId == studentCourseSectionSchedule.CourseSection?.AttendanceCategoryId),
                                WeekDays= studentCourseSectionSchedule.CourseSection?.SchoolCalendars?.Days
                            };

                            if (studentCourseSectionSchedule.CourseSection?.ScheduleType == "Fixed Schedule (1)")
                            {
                                var fixedData = this.context?.CourseFixedSchedule.Include(v => v.BlockPeriod).FirstOrDefault(c => c.SchoolId == studentCourseSectionSchedule.SchoolId && c.TenantId == studentCourseSectionSchedule.TenantId && c.CourseSectionId == studentCourseSectionSchedule.CourseSectionId);

                                if (fixedData != null)
                                {
                                    fixedData.BlockPeriod!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>() ;
                                    fixedData.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                    fixedData.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                    fixedData.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                    fixedData.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>();
                                    Student360ScheduleCourseSection.courseFixedSchedule = fixedData;
                                    Student360ScheduleCourseSection.DayOfWeek = studentCourseSectionSchedule.CourseSection?.MeetingDays;

                                    Student360ScheduleCourseSection.FullDayMinutes = allBlockData?.FirstOrDefault(x => x.BlockId == fixedData.BlockId)?.FullDayMinutes;
                                    Student360ScheduleCourseSection.HalfDayMinutes = allBlockData?.FirstOrDefault(x => x.BlockId == fixedData.BlockId)?.HalfDayMinutes;
                                }
                            }
                            if (studentCourseSectionSchedule.CourseSection?.ScheduleType == "Variable Schedule (2)")
                            {
                                var variableData = this.context?.CourseVariableSchedule.Include(v => v.BlockPeriod).Where(c => c.SchoolId == studentCourseSectionSchedule.SchoolId && c.TenantId == studentCourseSectionSchedule.TenantId && c.CourseSectionId == studentCourseSectionSchedule.CourseSectionId).ToList();

                                if (variableData != null && variableData.Any())
                                {
                                    //variableData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = null; x.BlockPeriod.CourseVariableSchedule = null; x.BlockPeriod.CourseCalendarSchedule = null; x.BlockPeriod.CourseBlockSchedule = null; x.BlockPeriod.StudentAttendance = null; });
                                    variableData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); });

                                    Student360ScheduleCourseSection.courseVariableScheduleList = variableData;

                                    Student360ScheduleCourseSection.FullDayMinutes = allBlockData?.FirstOrDefault(x => x.BlockId == variableData.FirstOrDefault()?.BlockId)?.FullDayMinutes;
                                    Student360ScheduleCourseSection.HalfDayMinutes = allBlockData?.FirstOrDefault(x => x.BlockId == variableData.FirstOrDefault()?.BlockId)?.HalfDayMinutes;
                                }
                            }
                            if (studentCourseSectionSchedule.CourseSection?.ScheduleType == "Calendar Schedule (3)")
                            {
                                var calenderData = this.context?.CourseCalendarSchedule.Include(v => v.BlockPeriod).Where(c => c.SchoolId == studentCourseSectionSchedule.SchoolId && c.TenantId == studentCourseSectionSchedule.TenantId && c.CourseSectionId == studentCourseSectionSchedule.CourseSectionId).ToList();

                                if (calenderData != null && calenderData.Any())
                                {
                                    //calenderData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = null; x.BlockPeriod.CourseVariableSchedule = null; x.BlockPeriod.CourseCalendarSchedule = null; x.BlockPeriod.CourseBlockSchedule = null; x.BlockPeriod.StudentAttendance = null; });
                                    calenderData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); });
                                    Student360ScheduleCourseSection.courseCalendarScheduleList = calenderData;

                                    Student360ScheduleCourseSection.FullDayMinutes = allBlockData?.FirstOrDefault(x => x.BlockId == calenderData.FirstOrDefault()?.BlockId)?.FullDayMinutes;
                                    Student360ScheduleCourseSection.HalfDayMinutes = allBlockData?.FirstOrDefault(x => x.BlockId == calenderData.FirstOrDefault()?.BlockId)?.HalfDayMinutes;
                                }
                            }
                            if (studentCourseSectionSchedule.CourseSection?.ScheduleType == "Block Schedule (4)")
                            {
                                var blockData = this.context?.CourseBlockSchedule.Include(v=>v.Block).Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => c.SchoolId == studentCourseSectionSchedule.SchoolId && c.TenantId == studentCourseSectionSchedule.TenantId && c.CourseSectionId == studentCourseSectionSchedule.CourseSectionId).ToList();

                                if (blockData != null && blockData.Any())
                                {
                                    //blockData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = null; x.BlockPeriod.CourseVariableSchedule = null; x.BlockPeriod.CourseCalendarSchedule = null; x.BlockPeriod.CourseBlockSchedule = null; x.BlockPeriod.StudentAttendance = null; });
                                    blockData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); });
                                    Student360ScheduleCourseSection.courseBlockScheduleList = blockData;

                                    var bellScheduleList = new List<BellSchedule>();
                                    foreach (var block in blockData)
                                    {
                                        var bellScheduleData = this.context?.BellSchedule.Where(c => c.SchoolId == studentCourseSectionSchedule.SchoolId && c.TenantId == studentCourseSectionSchedule.TenantId && c.BlockId == block.BlockId && c.BellScheduleDate >= student360ScheduleCourseSectionListViewModel.DurationStartDate && c.BellScheduleDate <= student360ScheduleCourseSectionListViewModel.DurationEndDate).ToList();
                                        if (bellScheduleData != null)
                                        {
                                            bellScheduleList.AddRange(bellScheduleData);
                                        }
                                    }
                                    Student360ScheduleCourseSection.bellScheduleList = bellScheduleList;
                                }
                            }
                            if (Student360ScheduleCourseSection.AttendanceCodeCategories!=null)
                            {                               
                                Student360ScheduleCourseSection.AttendanceCodeCategories.CourseSection = new HashSet<CourseSection>();
                                Student360ScheduleCourseSection.AttendanceCodeCategories.AttendanceCode.ToList().ForEach(v => v.StudentAttendance = new HashSet<StudentAttendance>());
                            }                            

                            if ( Student360ScheduleCourseSection.studentAttendanceList!=null)
                            {
                                Student360ScheduleCourseSection.studentAttendanceList.ForEach(u => { u.AttendanceCodeNavigation.AttendanceCodeCategories = new(); u.BlockPeriod = new(); u.StudentCoursesectionSchedule = new(); });
                            }
                            student360ScheduleCourseSectionForViewList.Add(Student360ScheduleCourseSection);
                        }

                        student360ScheduleCourseSectionListView.scheduleCourseSectionForView = student360ScheduleCourseSectionForViewList;
                        student360ScheduleCourseSectionListView._failure = false;
                        student360ScheduleCourseSectionListView.SchoolId = student360ScheduleCourseSectionListViewModel.SchoolId;
                        student360ScheduleCourseSectionListView.TenantId = student360ScheduleCourseSectionListViewModel.TenantId;
                        student360ScheduleCourseSectionListView.DurationStartDate = student360ScheduleCourseSectionListViewModel.DurationStartDate;
                        student360ScheduleCourseSectionListView.DurationEndDate= student360ScheduleCourseSectionListViewModel.DurationEndDate;
                        student360ScheduleCourseSectionListView.StudentId = student360ScheduleCourseSectionListViewModel.StudentId;
                        student360ScheduleCourseSectionListView.IsDropped = student360ScheduleCourseSectionListViewModel.IsDropped;
                        student360ScheduleCourseSectionListView._tenantName = student360ScheduleCourseSectionListViewModel._tenantName;
                        student360ScheduleCourseSectionListView._token = student360ScheduleCourseSectionListViewModel._token;
                        student360ScheduleCourseSectionListView._userName = student360ScheduleCourseSectionListViewModel._userName;
                    }
                    else
                    {
                        student360ScheduleCourseSectionListView.scheduleCourseSectionForView = student360ScheduleCourseSectionForViewList;
                        student360ScheduleCourseSectionListView._failure = true;
                        student360ScheduleCourseSectionListView._message = NORECORDFOUND;
                        student360ScheduleCourseSectionListView.SchoolId = student360ScheduleCourseSectionListViewModel.SchoolId;
                        student360ScheduleCourseSectionListView.TenantId = student360ScheduleCourseSectionListViewModel.TenantId;
                        student360ScheduleCourseSectionListView.StudentId = student360ScheduleCourseSectionListViewModel.StudentId;
                    }
                }                
            }
            catch (Exception es)
            {
                student360ScheduleCourseSectionListView._failure = true;
                student360ScheduleCourseSectionListView._message = es.Message;
                student360ScheduleCourseSectionListView.SchoolId = student360ScheduleCourseSectionListViewModel.SchoolId;
                student360ScheduleCourseSectionListView.TenantId = student360ScheduleCourseSectionListViewModel.TenantId;
                student360ScheduleCourseSectionListView.StudentId = student360ScheduleCourseSectionListViewModel.StudentId;
                student360ScheduleCourseSectionListView._tenantName = student360ScheduleCourseSectionListViewModel._tenantName;
                student360ScheduleCourseSectionListView._token = student360ScheduleCourseSectionListViewModel._token;
                student360ScheduleCourseSectionListView._userName = student360ScheduleCourseSectionListViewModel._userName;
            }
            return student360ScheduleCourseSectionListView;
        }

        public ScheduledStudentDeleteViewModel GroupDeleteForScheduledStudent(ScheduledStudentDeleteViewModel scheduledStudentDeleteViewModel)
        {
            ScheduledStudentDeleteViewModel scheduledStudentDelete = new();
            using var transaction = this.context?.Database.BeginTransaction();
            {
                try
                {
                    scheduledStudentDelete._message = "All students deleted from the selected course section";

                    if (scheduledStudentDeleteViewModel.StudentIds?.Any() == true)
                    {
                        foreach (var studentId in scheduledStudentDeleteViewModel.StudentIds)
                        {
                            var gradebookGradeData = this.context?.GradebookGrades.FirstOrDefault(e => e.TenantId == scheduledStudentDeleteViewModel.TenantId && e.SchoolId == scheduledStudentDeleteViewModel.SchoolId && e.CourseSectionId == scheduledStudentDeleteViewModel.CourseSectionId && e.StudentId == studentId);

                            var studentAttendanceData = this.context?.StudentAttendance.FirstOrDefault(e => e.TenantId == scheduledStudentDeleteViewModel.TenantId && e.SchoolId == scheduledStudentDeleteViewModel.SchoolId && e.CourseSectionId == scheduledStudentDeleteViewModel.CourseSectionId && e.StudentId == studentId);

                            var studentFinalGradeData = this.context?.StudentFinalGrade.FirstOrDefault(e => e.TenantId == scheduledStudentDeleteViewModel.TenantId && e.SchoolId == scheduledStudentDeleteViewModel.SchoolId && e.CourseSectionId == scheduledStudentDeleteViewModel.CourseSectionId && e.StudentId == studentId);

                            var studentEffortGradeData = this.context?.StudentEffortGradeMaster.FirstOrDefault(e => e.TenantId == scheduledStudentDeleteViewModel.TenantId && e.SchoolId == scheduledStudentDeleteViewModel.SchoolId && e.CourseSectionId == scheduledStudentDeleteViewModel.CourseSectionId && e.StudentId == studentId);

                            if (gradebookGradeData != null || studentAttendanceData != null || studentFinalGradeData != null || studentEffortGradeData != null)
                            {
                                scheduledStudentDelete._failure = true;
                                scheduledStudentDelete._message = "Some students could not be deleted from the selected course section. They have association";
                            }
                            else
                            {
                                var studentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.FirstOrDefault(e => e.TenantId == scheduledStudentDeleteViewModel.TenantId && e.SchoolId == scheduledStudentDeleteViewModel.SchoolId && e.CourseSectionId == scheduledStudentDeleteViewModel.CourseSectionId && e.StudentId == studentId);

                                if (studentCoursesectionScheduleData != null)
                                {
                                    this.context?.StudentCoursesectionSchedule.Remove(studentCoursesectionScheduleData);
                                }
                            }
                        }
                    }
                    else
                    {
                        scheduledStudentDelete._failure = true;
                        scheduledStudentDelete._message = "Please select students";
                    }

                    this.context?.SaveChanges();
                    transaction?.Commit();
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    scheduledStudentDelete._failure = true;
                    scheduledStudentDelete._message = es.Message;
                }
                return scheduledStudentDelete;
            }
        }

        /// <summary>
        ///  Get Student List By Course Section who have no associationship
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public ScheduleStudentListViewModel GetUnassociatedStudentListByCourseSection(PageResult pageResult)
        {
            ScheduleStudentListViewModel scheduleStudentListView = new ScheduleStudentListViewModel();
            IQueryable<ScheduleStudentForView>? transactionIQ = null;
            var scheduledStudentData = new List<ScheduleStudentForView>();
            try
            {
                var scheduledData = this.context?.StudentCoursesectionSchedule.
                                   Join(this.context.StudentMaster,
                                   scs => scs.StudentId, sm => sm.StudentId,
                                   (scs, sm) => new { scs, sm }).Where(c => c.scs.TenantId == pageResult.TenantId && c.scs.SchoolId == pageResult.SchoolId && c.sm.SchoolId == pageResult.SchoolId && c.sm.TenantId == pageResult.TenantId && c.scs.CourseSectionId == pageResult.CourseSectionId).ToList();

                if (scheduledData?.Any() == true)
                {
                    List<int> studentIds = new List<int>();

                    foreach (var data in scheduledData)
                    {
                        var gradebookGradeData = this.context?.GradebookGrades.FirstOrDefault(e => e.TenantId == data.scs.TenantId && e.SchoolId == data.scs.SchoolId && e.CourseSectionId == data.scs.CourseSectionId && e.StudentId == data.scs.StudentId);

                        var studentAttendanceData = this.context?.StudentAttendance.FirstOrDefault(e => e.TenantId == data.scs.TenantId && e.SchoolId == data.scs.SchoolId && e.CourseSectionId == data.scs.CourseSectionId && e.StudentId == data.scs.StudentId);

                        var studentFinalGradeData = this.context?.StudentFinalGrade.FirstOrDefault(e => e.TenantId == data.scs.TenantId && e.SchoolId == data.scs.SchoolId && e.CourseSectionId == data.scs.CourseSectionId && e.StudentId == data.scs.StudentId);

                        var studentEffortGradeData = this.context?.StudentEffortGradeMaster.FirstOrDefault(e => e.TenantId == data.scs.TenantId && e.SchoolId == data.scs.SchoolId && e.CourseSectionId == data.scs.CourseSectionId && e.StudentId == data.scs.StudentId);

                        if (gradebookGradeData != null || studentAttendanceData != null || studentFinalGradeData != null || studentEffortGradeData != null)
                        {
                            studentIds.Add(data.scs.StudentId);
                        }
                    }

                    if (studentIds?.Any() == true)
                    {
                        scheduledData = scheduledData.Where(x => !studentIds.Contains(x.scs.StudentId)).ToList();
                    }

                    scheduledStudentData = scheduledData?.Select(ssv => new ScheduleStudentForView
                    {
                        SchoolId = ssv.sm.SchoolId,
                        TenantId = ssv.sm.TenantId,
                        StudentId = ssv.sm.StudentId,
                        StudentGuid = ssv.sm.StudentGuid,
                        FirstGivenName = ssv.sm.FirstGivenName,
                        LastFamilyName = ssv.sm.LastFamilyName,
                        AlternateId = ssv.sm.AlternateId,
                        StudentInternalId = ssv.sm.StudentInternalId,
                        AdmissionNumber = ssv.sm.AdmissionNumber,
                        RollNumber = ssv.sm.RollNumber,
                        GradeLevel = this.context?.Gradelevels.FirstOrDefault(c => c.TenantId == ssv.sm.TenantId && c.SchoolId == ssv.sm.SchoolId && c.GradeId == ssv.scs.GradeId)?.Title,
                        Section = this.context?.Sections.FirstOrDefault(c => c.TenantId == ssv.sm.TenantId && c.SchoolId == ssv.sm.SchoolId && c.SectionId == ssv.sm.SectionId)?.Name,
                        GradeId = ssv.scs.GradeId,
                        GradeScaleId = ssv.scs.GradeScaleId,
                        PhoneNumber = ssv.sm.MobilePhone,
                        Action = ssv.scs.IsDropped,
                        ScheduleDate = ssv.scs.EffectiveStartDate,
                        //StudentPhoto = (pageResult.ProfilePhoto == true) ? ssv.sm.StudentPhoto : null,
                        Dob = ssv.sm.Dob,
                        Gender = ssv.sm.Gender,
                        Race = ssv.sm.Race,
                        Ethnicity = ssv.sm.Ethnicity,
                        MaritalStatus = ssv.sm.MaritalStatus,
                        CountryOfBirth = ssv.sm.CountryOfBirth,
                        Nationality = ssv.sm.Nationality,
                        FirstLanguageId = ssv.sm.FirstLanguageId,
                        SecondLanguageId = ssv.sm.SecondLanguageId,
                        ThirdLanguageId = ssv.sm.ThirdLanguageId,
                        HomeAddressLineOne = ssv.sm.HomeAddressLineOne,
                        HomeAddressLineTwo = ssv.sm.HomeAddressLineTwo,
                        HomeAddressCountry = ssv.sm.HomeAddressCountry,
                        HomeAddressState = ssv.sm.HomeAddressState,
                        HomeAddressCity = ssv.sm.HomeAddressCity,
                        HomeAddressZip = ssv.sm.HomeAddressZip,
                        BusNo = ssv.sm.BusNo,
                        HomePhone = ssv.sm.HomePhone,
                        MobilePhone = ssv.sm.MobilePhone,
                        PersonalEmail = ssv.sm.PersonalEmail,
                        SchoolEmail = ssv.sm.SchoolEmail,
                        CourseSectionId = ssv.scs.CourseSectionId,
                        MiddleName = ssv.sm.MiddleName,
                        CreatedOn = ssv.scs.CreatedOn,
                        CreatedBy = ssv.scs.CreatedBy,
                        UpdatedOn = ssv.scs.UpdatedOn,
                        UpdatedBy = ssv.scs.UpdatedBy,
                        IsDropped = ssv.scs.IsDropped,
                    }).GroupBy(f => f.StudentId).Select(g => g.First()).ToList();

                    if (scheduledStudentData != null && scheduledStudentData.Any())
                    {
                        if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                        {
                            transactionIQ = scheduledStudentData.AsQueryable();
                        }
                        else
                        {
                            if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                            {
                                string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue.ToLower();

                                transactionIQ = scheduledStudentData.Where(x => x.FirstGivenName != null && x.FirstGivenName.ToLower().Contains(Columnvalue) || x.MiddleName != null && x.MiddleName.ToLower().Contains(Columnvalue) ||
                                x.LastFamilyName != null && x.LastFamilyName.ToLower().Contains(Columnvalue) ||
                                (x.GradeLevel != null && x.GradeLevel.Contains(Columnvalue)) ||
                                (x.ScheduleDate != null && x.ScheduleDate.ToString() == Columnvalue) ||
                                (x.Section != null && x.Section.Contains(Columnvalue)) ||
                                (x.StudentInternalId != null && x.StudentInternalId.Contains(Columnvalue)) ||
                                (x.AlternateId != null && x.AlternateId.Contains(Columnvalue)) ||
                                (x.PhoneNumber != null && x.PhoneNumber.Contains(Columnvalue)) ||
                                (x.SchoolEmail != null && x.SchoolEmail.Contains(Columnvalue)) ||
                                (x.MobilePhone != null && x.MobilePhone.Contains(Columnvalue))).AsQueryable();
                            }
                            else
                            {
                                transactionIQ = Utility.FilteredData(pageResult.FilterParams!, scheduledStudentData).AsQueryable();
                            }
                            //transactionIQ = transactionIQ.Distinct();
                        }

                        if (pageResult.SortingModel != null)
                        {
                            switch (pageResult!.SortingModel!.SortColumn)
                            {
                                default:
                                    transactionIQ = Utility.Sort(transactionIQ, pageResult?.SortingModel?.SortColumn!, pageResult?.SortingModel?.SortDirection!);
                                    break;
                            }
                        }
                        //Advance Search for date range
                        if (pageResult?.DobStartDate != null && pageResult.DobEndDate != null)
                        {
                            var filterInDateRange = transactionIQ.Where(x => x.Dob >= pageResult.DobStartDate && x.Dob <= pageResult.DobEndDate);
                            if (filterInDateRange.Any())
                            {
                                transactionIQ = filterInDateRange;
                            }
                            else
                            {
                                transactionIQ = null;
                            }
                        }

                        int totalCount = transactionIQ != null ? transactionIQ.Count() : 0;
                        if (pageResult?.PageNumber > 0 && pageResult.PageSize > 0)
                        {
                            transactionIQ = transactionIQ?.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                        }

                        if (transactionIQ!.ToList()?.Any() == true)
                        {
                            scheduleStudentListView.scheduleStudentForView = transactionIQ!.ToList();
                            scheduleStudentListView.TotalCount = totalCount;
                            scheduleStudentListView._failure = false;
                        }
                        else
                        {
                            scheduleStudentListView.scheduleStudentForView = new();
                            scheduleStudentListView._failure = true;
                            scheduleStudentListView._message = "Student deletion is not permitted due to transactional associations";
                        }
                    }
                    else
                    {
                        scheduleStudentListView.scheduleStudentForView = new();
                        scheduleStudentListView._failure = true;
                        scheduleStudentListView._message = "Student deletion is not permitted due to transactional associations";
                    }
                }
                else
                {
                    scheduleStudentListView.scheduleStudentForView = new();
                    scheduleStudentListView._failure = true;
                    scheduleStudentListView._message = NORECORDFOUND;
                }

                scheduleStudentListView.TenantId = pageResult?.TenantId;
                scheduleStudentListView.SchoolId = pageResult?.SchoolId;
                scheduleStudentListView.CourseSectionId = pageResult?.CourseSectionId;
                scheduleStudentListView.StaffId = pageResult?.StaffId;
                scheduleStudentListView.AcademicYear = pageResult?.AcademicYear;
                scheduleStudentListView.PageNumber = pageResult?.PageNumber;
                scheduleStudentListView._pageSize = pageResult?.PageSize;
                scheduleStudentListView._tenantName = pageResult?._tenantName;
                scheduleStudentListView._token = pageResult?._token;
            }
            catch (Exception es)
            {
                scheduleStudentListView._failure = true;
                scheduleStudentListView._message = es.Message; ;
            }
            return scheduleStudentListView;

        }
    }
}

﻿/***********************************************************************************
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
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.School;
using opensis.data.ViewModels.StaffSchedule;
using opensis.data.ViewModels.StudentPortal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.data.Repository
{
    public class StudentPortalRepository: IStudentPortalRepository
    {
        private readonly CRMContext? context;
        //private readonly CatalogDBContext catdbContext;
        private static readonly string NORECORDFOUND = "No Record Found";
        
        public StudentPortalRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }
        public ScheduledCourseSectionViewModelForStudent GetStudentDashboard(ScheduledCourseSectionViewModelForStudent scheduledCourseSectionViewModelForStudent)
        {
            ScheduledCourseSectionViewModelForStudent scheduledCourseSectionViewForStudent = new();
            List<StudentCoursesectionSchedule> studentCoursesectionScheduleList = new();
            List<AllCourseSectionView>? allCourseSectionVewList = new();
            //CourseFixedSchedule fixedData = new();
            List<CourseVariableSchedule>? variableData = new();
            List<CourseCalendarSchedule>? calenderData = new();
            List<CourseBlockSchedule>? blockData = new();

            scheduledCourseSectionViewForStudent.TenantId = scheduledCourseSectionViewModelForStudent.TenantId;
            scheduledCourseSectionViewForStudent._tenantName = scheduledCourseSectionViewModelForStudent._tenantName;
            scheduledCourseSectionViewForStudent.SchoolId = scheduledCourseSectionViewModelForStudent.SchoolId;
            scheduledCourseSectionViewForStudent.StudentId = scheduledCourseSectionViewModelForStudent.StudentId;
            scheduledCourseSectionViewForStudent._token = scheduledCourseSectionViewModelForStudent._token;
            scheduledCourseSectionViewForStudent._userName = scheduledCourseSectionViewModelForStudent._userName;
            scheduledCourseSectionViewForStudent.AllCourse = scheduledCourseSectionViewModelForStudent.AllCourse;

            try
            {
                var todayDate = DateTime.Today;

                var studentMasterData = this.context?.StudentMaster.Include(x => x.StudentEnrollment).Include(s => s.Sections).Where(x => x.TenantId == scheduledCourseSectionViewModelForStudent.TenantId && x.SchoolId == scheduledCourseSectionViewModelForStudent.SchoolId && x.StudentId == scheduledCourseSectionViewModelForStudent.StudentId).FirstOrDefault();
                if (studentMasterData != null)
                {
                    //fetch student details
                    scheduledCourseSectionViewForStudent.FirstGivenName = studentMasterData.FirstGivenName;
                    scheduledCourseSectionViewForStudent.MiddleName = studentMasterData.MiddleName;
                    scheduledCourseSectionViewForStudent.LastFamilyName = studentMasterData.LastFamilyName;
                    scheduledCourseSectionViewForStudent.StudentInternalId = studentMasterData.StudentInternalId;
                    scheduledCourseSectionViewForStudent.GradeLevel = studentMasterData.StudentEnrollment.FirstOrDefault(x => x.IsActive == true && x.SchoolId == scheduledCourseSectionViewModelForStudent.SchoolId)?.GradeLevelTitle;
                    scheduledCourseSectionViewForStudent.Section = studentMasterData.Sections?.Name;
                    scheduledCourseSectionViewForStudent.StudentPhoto = studentMasterData.StudentPhoto;

                    var scheduledCourseSectionData = this.context?.StudentCoursesectionSchedule.Include(x => x.CourseSection).Include(x => x.CourseSection.Course).Include(x => x.CourseSection.SchoolCalendars).Join(this.context.AllCourseSectionView,
                        scs => scs.CourseSectionId, acsv => acsv.CourseSectionId,
                        (scs, acsv) => new { scs, acsv }).Where(x => x.scs.TenantId == scheduledCourseSectionViewModelForStudent.TenantId && x.acsv.TenantId == scheduledCourseSectionViewModelForStudent.TenantId && x.scs.SchoolId == scheduledCourseSectionViewModelForStudent.SchoolId && x.acsv.SchoolId == scheduledCourseSectionViewModelForStudent.SchoolId && x.scs.StudentId == scheduledCourseSectionViewModelForStudent.StudentId && x.scs.IsDropped != true).ToList();

                    if (scheduledCourseSectionData != null && scheduledCourseSectionData.Any())
                    {

                        if (scheduledCourseSectionViewModelForStudent.AllCourse != true)
                        {
                            var coursesectionScheduleList = scheduledCourseSectionData.Where(x => x.acsv.DurationEndDate >= todayDate && ((x.acsv.FixedDays != null
                                && x.acsv.FixedDays.ToLower().Contains(todayDate.DayOfWeek.ToString().ToLower())) || (x.acsv.VarDay != null
                                && x.acsv.VarDay.ToLower().Contains(todayDate.DayOfWeek.ToString().ToLower())) || (x.acsv.CalDate != null && x.acsv.CalDate == todayDate) || x.acsv.BlockPeriodId != null)).ToList();

                            studentCoursesectionScheduleList = coursesectionScheduleList.Select(s => s.scs).Distinct().ToList();
                        }
                        else
                        {
                            if (scheduledCourseSectionViewModelForStudent.MarkingPeriodStartDate != null && scheduledCourseSectionViewModelForStudent.MarkingPeriodEndDate != null)
                            {
                                scheduledCourseSectionData = scheduledCourseSectionData.Where(x => x.scs.CourseSection.DurationBasedOnPeriod == false || ((scheduledCourseSectionViewModelForStudent.MarkingPeriodStartDate >= x.acsv.DurationStartDate && scheduledCourseSectionViewModelForStudent.MarkingPeriodStartDate <= x.acsv.DurationEndDate) && (scheduledCourseSectionViewModelForStudent.MarkingPeriodEndDate >= x.acsv.DurationStartDate && scheduledCourseSectionViewModelForStudent.MarkingPeriodEndDate <= x.acsv.DurationEndDate))).ToList();
                            }
                            else
                            {
                                scheduledCourseSectionViewForStudent._failure = true;
                                scheduledCourseSectionViewForStudent._message = "Please send Marking Period Start Date and Marking Period End Date";
                                return scheduledCourseSectionViewForStudent;
                            }
                        }
                    }
                    else
                    {
                        scheduledCourseSectionViewForStudent._failure = true;
                        scheduledCourseSectionViewForStudent._message = NORECORDFOUND;
                        return scheduledCourseSectionViewForStudent;
                    }

                    if (scheduledCourseSectionData.Any())
                    {
                        foreach (var scheduledCourseSection in scheduledCourseSectionData)
                        {
                            List<DateTime> holidayList = new();
                            //Calculate Holiday
                            var CalendarEventsData = this.context?.CalendarEvents.Where(e => e.TenantId == scheduledCourseSectionViewModelForStudent.TenantId && e.CalendarId == scheduledCourseSection.scs.CourseSection.CalendarId && (e.StartDate >= scheduledCourseSection.acsv.DurationStartDate && e.StartDate <= scheduledCourseSection.acsv.DurationEndDate || e.EndDate >= scheduledCourseSection.acsv.DurationStartDate && e.EndDate <= scheduledCourseSection.acsv.DurationEndDate) && e.IsHoliday == true && (e.SchoolId == scheduledCourseSectionViewModelForStudent.SchoolId || e.ApplicableToAllSchool == true)).ToList();
                            if (CalendarEventsData != null && CalendarEventsData.Any())
                            {
                                foreach (var calender in CalendarEventsData)
                                {
                                    if (calender.EndDate != null && calender.StartDate != null)
                                    {
                                        if (calender.EndDate.Value.Date > calender.StartDate.Value.Date)
                                        {
                                            var date = Enumerable.Range(0, 1 + (calender.EndDate.Value.Date - calender.StartDate.Value.Date).Days)
                                               .Select(i => calender.StartDate.Value.Date.AddDays(i))
                                               .ToList();
                                            holidayList.AddRange(date);
                                        }
                                        holidayList.Add(calender.StartDate.Value.Date);
                                    }

                                }
                            }
                            if (scheduledCourseSection.scs.CourseSection.ScheduleType == "Fixed Schedule (1)" || scheduledCourseSection.scs.CourseSection.ScheduleType == "Variable Schedule (2)" || scheduledCourseSection.scs.CourseSection.ScheduleType == "Block Schedule (4)")
                            {
                                CourseSectionViewList CourseSections = new CourseSectionViewList
                                {
                                    CourseTitle = scheduledCourseSection.scs.CourseSection.Course.CourseTitle
                                };

                                if (scheduledCourseSection.scs.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                {
                                    CourseSections.ScheduleType = "Fixed Schedule";
                                    CourseSections.MeetingDays = scheduledCourseSection.scs.CourseSection.MeetingDays;

                                    var fixedData = context?.CourseFixedSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).FirstOrDefault(c => c.SchoolId == scheduledCourseSection.scs.SchoolId && c.TenantId == scheduledCourseSection.scs.TenantId && c.CourseSectionId == scheduledCourseSection.scs.CourseSectionId);



                                    if (fixedData != null)
                                    {
                                        CourseSections.courseFixedSchedule = fixedData;
                                        CourseSections.HolidayList = holidayList;
                                    }
                                }
                                if (scheduledCourseSection.scs.CourseSection.ScheduleType == "Variable Schedule (2)")
                                {
                                    CourseSections.ScheduleType = "Variable Schedule";

                                    variableData = this.context?.CourseVariableSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => c.SchoolId == scheduledCourseSection.scs.SchoolId && c.TenantId == scheduledCourseSection.scs.TenantId && c.CourseSectionId == scheduledCourseSection.scs.CourseSectionId).ToList();


                                    if (variableData?.Any() == true)
                                    {
                                        var days = variableData.Select(s => s.Day);
                                        CourseSections.MeetingDays = String.Join("|", days);
                                        CourseSections.courseVariableSchedule = variableData;
                                        CourseSections.HolidayList = holidayList;
                                    }
                                }
                                if (scheduledCourseSection.scs.CourseSection.ScheduleType == "Block Schedule (4)")
                                {
                                    CourseSections.ScheduleType = "Block Schedule";

                                    blockData = this.context?.CourseBlockSchedule.Include(v => v.Block).Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => c.SchoolId == scheduledCourseSection.scs.SchoolId && c.TenantId == scheduledCourseSection.scs.TenantId && c.CourseSectionId == scheduledCourseSection.scs.CourseSectionId).ToList();

                                    if (blockData?.Any() == true)
                                    {
                                        CourseSections.MeetingDays = "Block Days";
                                        CourseSections.courseBlockSchedule = blockData;
                                        CourseSections.HolidayList = holidayList;

                                        var bellScheduleList = new List<BellSchedule>();
                                        foreach (var block in blockData)
                                        {
                                            var bellScheduleData = this.context?.BellSchedule.Where(c => c.SchoolId == scheduledCourseSection.scs.SchoolId && c.TenantId == scheduledCourseSection.scs.TenantId && c.BlockId == block.BlockId && c.BellScheduleDate >= scheduledCourseSection.acsv.DurationStartDate && c.BellScheduleDate <= scheduledCourseSection.acsv.DurationEndDate).ToList();
                                            if (bellScheduleData?.Any() == true)
                                            {
                                                bellScheduleList.AddRange(bellScheduleData);
                                            }
                                        }
                                        CourseSections.bellScheduleList = bellScheduleList;
                                    }
                                }
                                CourseSections.CalendarId = scheduledCourseSection.scs.CourseSection.CalendarId;
                                CourseSections.CourseId = scheduledCourseSection.scs.CourseId;
                                CourseSections.CourseGradeLevel = scheduledCourseSection.scs.CourseSection.Course.CourseGradeLevel;
                                CourseSections.CourseSectionId = scheduledCourseSection.scs.CourseSectionId;
                                CourseSections.GradeScaleType = scheduledCourseSection.scs.CourseSection.GradeScaleType;
                                CourseSections.AttendanceCategoryId = scheduledCourseSection.scs.CourseSection.AttendanceCategoryId;
                                CourseSections.GradeScaleId = scheduledCourseSection.scs.CourseSection.GradeScaleId;
                                CourseSections.StandardGradeScaleId = scheduledCourseSection.scs.CourseSection.StandardGradeScaleId;
                                CourseSections.CourseSectionName = scheduledCourseSection.scs.CourseSectionName;
                                CourseSections.YrMarkingPeriodId = scheduledCourseSection.acsv.YrMarkingPeriodId;
                                CourseSections.SmstrMarkingPeriodId = scheduledCourseSection.acsv.SmstrMarkingPeriodId;
                                CourseSections.QtrMarkingPeriodId = scheduledCourseSection.acsv.QtrMarkingPeriodId;
                                CourseSections.PrgrsprdMarkingPeriodId = scheduledCourseSection.acsv.PrgrsprdMarkingPeriodId;
                                CourseSections.DurationStartDate = scheduledCourseSection.acsv.DurationStartDate;
                                CourseSections.DurationEndDate = scheduledCourseSection.acsv.DurationEndDate;
                                CourseSections.MeetingDays = scheduledCourseSection.scs.CourseSection.MeetingDays;
                                CourseSections.AttendanceTaken = scheduledCourseSection.scs.CourseSection.AttendanceTaken;
                                CourseSections.WeekDays = scheduledCourseSection.scs.CourseSection.SchoolCalendars!.Days;

                                scheduledCourseSectionViewForStudent.courseSectionViewList.Add(CourseSections);

                            }
                            else
                            {
                                if (scheduledCourseSection.scs.CourseSection.ScheduleType == "Calendar Schedule (3)")
                                {
                                    CourseSectionViewList CourseSection = new CourseSectionViewList
                                    {
                                        ScheduleType = "Calendar Schedule"
                                    };

                                    calenderData = this.context?.CourseCalendarSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => (scheduledCourseSectionViewModelForStudent.AllCourse != true) ? c.SchoolId == scheduledCourseSection.scs.SchoolId && c.TenantId == scheduledCourseSection.scs.TenantId && c.CourseSectionId == scheduledCourseSection.scs.CourseSectionId && c.Date.Value.Date == todayDate.Date : c.SchoolId == scheduledCourseSection.scs.SchoolId && c.TenantId == scheduledCourseSection.scs.TenantId && c.CourseSectionId == scheduledCourseSection.scs.CourseSectionId).ToList();

                                    if (calenderData != null && calenderData.Any())
                                    {
                                        CourseSection.MeetingDays = "Calendar Days";
                                        CourseSection.courseCalendarSchedule = calenderData;
                                        CourseSection.HolidayList = holidayList;

                                        CourseSection.CourseTitle = scheduledCourseSection.scs.CourseSection.Course.CourseTitle;
                                        CourseSection.CalendarId = scheduledCourseSection.scs.CourseSection.CalendarId;
                                        CourseSection.CourseId = scheduledCourseSection.scs.CourseId;
                                        CourseSection.CourseGradeLevel = scheduledCourseSection.scs.CourseSection.Course.CourseGradeLevel;
                                        CourseSection.CourseSectionId = scheduledCourseSection.scs.CourseSectionId;
                                        CourseSection.GradeScaleType = scheduledCourseSection.scs.CourseSection.GradeScaleType;
                                        CourseSection.AttendanceCategoryId = scheduledCourseSection.scs.CourseSection.AttendanceCategoryId;
                                        CourseSection.GradeScaleId = scheduledCourseSection.scs.CourseSection.GradeScaleId;
                                        CourseSection.StandardGradeScaleId = scheduledCourseSection.scs.CourseSection.StandardGradeScaleId;
                                        CourseSection.CourseSectionName = scheduledCourseSection.scs.CourseSectionName;
                                        CourseSection.YrMarkingPeriodId = scheduledCourseSection.acsv.YrMarkingPeriodId;
                                        CourseSection.SmstrMarkingPeriodId = scheduledCourseSection.acsv.SmstrMarkingPeriodId;
                                        CourseSection.PrgrsprdMarkingPeriodId = scheduledCourseSection.acsv.PrgrsprdMarkingPeriodId;
                                        CourseSection.DurationStartDate = scheduledCourseSection.acsv.DurationStartDate;
                                        CourseSection.DurationEndDate = scheduledCourseSection.acsv.DurationEndDate;
                                        //CourseSection.MeetingDays = scheduledCourseSection.MeetingDays;
                                        CourseSection.AttendanceTaken = scheduledCourseSection.scs.CourseSection.AttendanceTaken;
                                        CourseSection.WeekDays = scheduledCourseSection.scs.CourseSection.SchoolCalendars!.Days;

                                        scheduledCourseSectionViewForStudent.courseSectionViewList.Add(CourseSection);

                                    }
                                }
                            }
                        }

                        //this block for assignment details
                        if (scheduledCourseSectionViewModelForStudent.MarkingPeriodStartDate != null && scheduledCourseSectionViewModelForStudent.MarkingPeriodEndDate != null)
                        {
                            int? markingPeriodId = null;
                            //var assignmentTypeIds=new List<int>();
                            var progressPeriodsData = this.context?.ProgressPeriods.Where(x => x.SchoolId == scheduledCourseSectionViewModelForStudent.SchoolId && x.TenantId == scheduledCourseSectionViewModelForStudent.TenantId && x.StartDate == scheduledCourseSectionViewModelForStudent.MarkingPeriodStartDate && x.EndDate == scheduledCourseSectionViewModelForStudent.MarkingPeriodEndDate).FirstOrDefault();

                            if (progressPeriodsData != null)
                            {
                                markingPeriodId = progressPeriodsData.MarkingPeriodId;
                            }
                            else
                            {
                                var quarterData = this.context?.Quarters.Where(x => x.SchoolId == scheduledCourseSectionViewModelForStudent.SchoolId && x.TenantId == scheduledCourseSectionViewModelForStudent.TenantId && x.StartDate == scheduledCourseSectionViewModelForStudent.MarkingPeriodStartDate && x.EndDate == scheduledCourseSectionViewModelForStudent.MarkingPeriodEndDate).FirstOrDefault();

                                if (quarterData != null)
                                {
                                    markingPeriodId = quarterData.MarkingPeriodId;
                                }
                                else
                                {
                                    var semesterData = this.context?.Semesters.Where(x => x.SchoolId == scheduledCourseSectionViewModelForStudent.SchoolId && x.TenantId == scheduledCourseSectionViewModelForStudent.TenantId && x.StartDate == scheduledCourseSectionViewModelForStudent.MarkingPeriodStartDate && x.EndDate == scheduledCourseSectionViewModelForStudent.MarkingPeriodEndDate).FirstOrDefault();

                                    if (semesterData != null)
                                    {
                                        markingPeriodId = semesterData.MarkingPeriodId;
                                    }
                                    else
                                    {
                                        var yearData = this.context?.SchoolYears.Where(x => x.SchoolId == scheduledCourseSectionViewModelForStudent.SchoolId && x.TenantId == scheduledCourseSectionViewModelForStudent.TenantId && x.StartDate == scheduledCourseSectionViewModelForStudent.MarkingPeriodStartDate && x.EndDate == scheduledCourseSectionViewModelForStudent.MarkingPeriodEndDate).FirstOrDefault();

                                        if (yearData != null)
                                        {
                                            markingPeriodId = yearData.MarkingPeriodId;
                                        }
                                    }
                                }
                            }

                            foreach (var courseSection in scheduledCourseSectionViewForStudent.courseSectionViewList)
                            {
                                var assignmentData = this.context?.Assignment.Include(s => s.AssignmentType).Where(c => c.SchoolId == scheduledCourseSectionViewModelForStudent.SchoolId && c.TenantId == scheduledCourseSectionViewModelForStudent.TenantId && c.CourseSectionId == courseSection.CourseSectionId && ((c.AssignmentType.PrgrsprdMarkingPeriodId != null && c.AssignmentType.PrgrsprdMarkingPeriodId == markingPeriodId) || (c.AssignmentType.QtrMarkingPeriodId != null && c.AssignmentType.QtrMarkingPeriodId == markingPeriodId) || (c.AssignmentType.SmstrMarkingPeriodId != null && c.AssignmentType.SmstrMarkingPeriodId == markingPeriodId) || (c.AssignmentType.YrMarkingPeriodId != null && c.AssignmentType.YrMarkingPeriodId == markingPeriodId) || (c.AssignmentType.PrgrsprdMarkingPeriodId == null && c.AssignmentType.QtrMarkingPeriodId == null && c.AssignmentType.SmstrMarkingPeriodId == null && c.AssignmentType.YrMarkingPeriodId == null))).Select(a => new AssignmentDetails { AssignmentTypeId = a.AssignmentTypeId, AssignmentId = a.AssignmentId, CourseSectionId = a.CourseSectionId, CourseSectionTitle = courseSection.CourseSectionName, AssignmentTypeTitle = a.AssignmentType.Title, AssignmentTitle = a.AssignmentTitle, DueDate = a.DueDate, AssignmentDate = a.AssignmentDate, AssignmentDescription = a.AssignmentDescription }).ToList();

                                if (assignmentData?.Any() == true)
                                {
                                    scheduledCourseSectionViewForStudent.AssignmentList.AddRange(assignmentData);
                                }
                            }
                        }

                        foreach (var courseSection in scheduledCourseSectionViewForStudent.courseSectionViewList)
                        {
                            if (courseSection.courseFixedSchedule != null)
                            {
                                courseSection.courseFixedSchedule.BlockPeriod!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                courseSection.courseFixedSchedule.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                courseSection.courseFixedSchedule.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                courseSection.courseFixedSchedule.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                courseSection.courseFixedSchedule.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>();
                                courseSection.courseFixedSchedule.BlockPeriod.StudentMissingAttendances = new List<StudentMissingAttendance>();
                                courseSection.courseFixedSchedule.Rooms!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                courseSection.courseFixedSchedule.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                courseSection.courseFixedSchedule.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                courseSection.courseFixedSchedule.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                            }
                            else if (courseSection.courseVariableSchedule?.Any() == true)
                            {
                                courseSection.courseVariableSchedule.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); x.BlockPeriod.StudentMissingAttendances = new HashSet<StudentMissingAttendance>(); x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });
                            }
                            else if (courseSection.courseCalendarSchedule?.Any() == true)
                            {
                                courseSection.courseCalendarSchedule.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); x.BlockPeriod.StudentMissingAttendances = new HashSet<StudentMissingAttendance>(); x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });
                            }
                            else if (courseSection.courseBlockSchedule?.Any() == true)
                            {
                                courseSection.courseBlockSchedule.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); x.BlockPeriod.StudentMissingAttendances = new HashSet<StudentMissingAttendance>(); x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });
                            }
                        }
                    }
                    else
                    {
                        scheduledCourseSectionViewForStudent._failure = true;
                        scheduledCourseSectionViewForStudent._message = NORECORDFOUND;
                        return scheduledCourseSectionViewForStudent;
                    }

                    if (scheduledCourseSectionViewModelForStudent.MembershipId != null)
                    {
                        var noticeList = this.context?.Notice.Where(x => x.TenantId == scheduledCourseSectionViewModelForStudent.TenantId && (x.SchoolId == scheduledCourseSectionViewModelForStudent.SchoolId || (x.SchoolId != scheduledCourseSectionViewModelForStudent.SchoolId && x.VisibleToAllSchool == true)) && x.Isactive == true && x.TargetMembershipIds.Contains((scheduledCourseSectionViewModelForStudent.MembershipId ?? 0).ToString()) && (x.ValidFrom <= todayDate && todayDate <= x.ValidTo)).OrderByDescending(x => x.ValidFrom).ToList();

                        if (noticeList?.Any() == true)
                        {
                            scheduledCourseSectionViewForStudent.NoticeList = noticeList;
                        }
                    }
                }
                else
                {
                    scheduledCourseSectionViewForStudent._failure = true;
                    scheduledCourseSectionViewForStudent._message = NORECORDFOUND;
                    return scheduledCourseSectionViewForStudent;
                }
            }
            catch (Exception ex)
            {
                scheduledCourseSectionViewForStudent._failure = true;
                scheduledCourseSectionViewForStudent._message = ex.Message;
            }
            return scheduledCourseSectionViewForStudent;
        }
    }
}

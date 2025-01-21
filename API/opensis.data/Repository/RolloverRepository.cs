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
using Newtonsoft.Json.Linq;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.Rollover;
using opensis.data.ViewModels.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
    public class RolloverRepository : IRolloverRepository
    {
        private readonly CRMContext? context;
        //private static readonly string NORECORDFOUND = "No Record Found";
        public RolloverRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Roll Over School
        /// </summary>
        /// <param name="courseCommentCategoryAddViewModel"></param>
        /// <returns></returns>
        public RolloverViewModel Rollover(RolloverViewModel rolloverViewModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                if (rolloverViewModel.SchoolRollover is null)
                {
                    return rolloverViewModel;
                }
                try
                {


                    var sessionCalendar = this.context?.SchoolCalendars.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.SessionCalendar == true).OrderByDescending(x => x.AcademicYear).FirstOrDefault();

                    if (rolloverViewModel.SchoolRollover.SchoolBeginDate <= sessionCalendar!.EndDate)
                    {
                        rolloverViewModel._failure = true;
                        rolloverViewModel._message = "School begin date should be greater than previous school year end date";

                        return rolloverViewModel;
                    }

                    //Create 365 days session calendar
                    var sessionCalendarStartDate = rolloverViewModel.SchoolRollover.SchoolBeginDate;
                    var dateAfterOneYear = sessionCalendarStartDate!.Value.Date.AddYears(+1);
                    var sessionCalendarEndDate = dateAfterOneYear!.AddDays(-1);

                    if (rolloverViewModel.SchoolRollover.SchoolBeginDate < sessionCalendarStartDate || rolloverViewModel.SchoolRollover.SchoolBeginDate > sessionCalendarEndDate || rolloverViewModel.SchoolRollover.SchoolEndDate < sessionCalendarStartDate || rolloverViewModel.SchoolRollover.SchoolEndDate > sessionCalendarEndDate)
                    {
                        rolloverViewModel._failure = true;
                        rolloverViewModel._message = "School begin date and end date should be between academic calendar's start date & end date";

                        return rolloverViewModel;
                    }
                    //*****//

                    var rolloverExist = this.context?.SchoolRollover.Where(x => x.ReenrollmentDate == rolloverViewModel.SchoolRollover.ReenrollmentDate && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.TenantId == rolloverViewModel.SchoolRollover.TenantId).ToList();
                    if (rolloverExist?.Any() == true)
                    {
                        rolloverViewModel._failure = true;
                        rolloverViewModel._message = "Rollover data already exist";
                    }
                    else
                    {
                        int? rolloverId = 1;

                        var maxRolloverId = this.context?.SchoolRollover.Where(x => x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.TenantId == rolloverViewModel.SchoolRollover.TenantId).OrderByDescending(x => x.RolloverId).FirstOrDefault();

                        if (maxRolloverId != null)
                        {
                            rolloverId = maxRolloverId.RolloverId + 1;
                        }

                        if (rolloverViewModel.SchoolRollover != null)
                        {
                            rolloverViewModel.SchoolRollover.RolloverId = (int)rolloverId;
                            rolloverViewModel.SchoolRollover.RolloverStatus = true;
                            rolloverViewModel.SchoolRollover.CreatedOn = DateTime.UtcNow;
                            this.context?.SchoolRollover.Add(rolloverViewModel.SchoolRollover);


                            //Insert Year
                            SchoolYears schoolYears = new();
                            int? yearId = 1;

                            var yearData = this.context?.SchoolYears.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).OrderByDescending(x => x.MarkingPeriodId).FirstOrDefault();

                            if (yearData != null)
                            {
                                yearId = yearData.MarkingPeriodId + 1;
                            }

                            schoolYears.TenantId = rolloverViewModel.SchoolRollover.TenantId;
                            schoolYears.SchoolId = rolloverViewModel.SchoolRollover.SchoolId;
                            schoolYears.MarkingPeriodId = (int)yearId;
                            schoolYears.AcademicYear = rolloverViewModel.SchoolRollover.SchoolBeginDate.HasValue == true ? Convert.ToDecimal(rolloverViewModel.SchoolRollover.SchoolBeginDate.Value.Year) : (decimal?)null;
                            schoolYears.Title = rolloverViewModel.FullYearName;
                            schoolYears.ShortName = rolloverViewModel.FullYearShortName;
                            schoolYears.StartDate = rolloverViewModel.SchoolRollover.SchoolBeginDate;
                            schoolYears.EndDate = rolloverViewModel.SchoolRollover.SchoolEndDate;
                            schoolYears.DoesGrades = rolloverViewModel.DoesGrades;
                            schoolYears.DoesExam = rolloverViewModel.DoesExam;
                            schoolYears.DoesComments = rolloverViewModel.DoesComments;
                            schoolYears.RolloverId = rolloverId;
                            schoolYears.CreatedOn = DateTime.UtcNow;
                            schoolYears.CreatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                            this.context?.SchoolYears.Add(schoolYears);

                            //Insert session calendar
                            SchoolCalendars schoolSessionCalendar = new();
                            int? calenderId = 1;

                            var calendarData = this.context?.SchoolCalendars.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).OrderByDescending(x => x.CalenderId).FirstOrDefault();

                            if (calendarData != null)
                            {
                                calenderId = calendarData.CalenderId + 1;
                            }

                            schoolSessionCalendar.TenantId = rolloverViewModel.SchoolRollover.TenantId;
                            schoolSessionCalendar.SchoolId = rolloverViewModel.SchoolRollover.SchoolId;
                            schoolSessionCalendar.CalenderId = (int)calenderId;
                            schoolSessionCalendar.Title = "Default Calendar";
                            schoolSessionCalendar.AcademicYear = sessionCalendarStartDate != null ? Convert.ToDecimal(sessionCalendarStartDate.Value.Year) : 0;
                            schoolSessionCalendar.DefaultCalender = true;
                            schoolSessionCalendar.SessionCalendar = true;
                            schoolSessionCalendar.Days = "12345";
                            schoolSessionCalendar.RolloverId = rolloverId;
                            schoolSessionCalendar.StartDate = sessionCalendarStartDate;
                            schoolSessionCalendar.EndDate = sessionCalendarEndDate;
                            schoolSessionCalendar.CreatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                            schoolSessionCalendar.CreatedOn = DateTime.UtcNow;

                            this.context?.SchoolCalendars.Add(schoolSessionCalendar);
                            //*****//

                            //Insert Subject
                            var subjectList = this.context?.Subject.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.AcademicYear == sessionCalendar!.AcademicYear).ToList();

                            if (subjectList?.Any() == true)
                            {
                                int? subjectId = 1;

                                var subjectData = subjectList.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).OrderByDescending(x => x.SubjectId).FirstOrDefault();

                                if (subjectData != null)
                                {
                                    subjectId = subjectData.SubjectId + 1;
                                }

                                List<Subject> subjects = new();

                                foreach (var subjectListData in subjectList)
                                {
                                    Subject subject = new();

                                    subject.TenantId = subjectListData.TenantId;
                                    subject.SchoolId = subjectListData.SchoolId;
                                    subject.SubjectId = (int)subjectId;
                                    subject.SubjectName = subjectListData.SubjectName;
                                    subject.RolloverId = rolloverId;
                                    subject.AcademicYear = rolloverViewModel.SchoolRollover.SchoolBeginDate != null ? Convert.ToDecimal(rolloverViewModel.SchoolRollover.SchoolBeginDate.Value.Year) : 0;
                                    subject.CreatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                                    subject.CreatedOn = DateTime.UtcNow;

                                    subjects.Add(subject);
                                    subjectId++;
                                }
                                this.context?.Subject.AddRange(subjects);
                            }

                            //Insert Course
                            var courseList = this.context?.Course.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.AcademicYear == sessionCalendar!.AcademicYear).ToList();

                            if (courseList?.Any() == true)
                            {
                                int? courseId = 1;

                                var courseData = courseList.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).OrderByDescending(x => x.CourseId).FirstOrDefault();

                                if (courseData != null)
                                {
                                    courseId = courseData.CourseId + 1;
                                }

                                int? courseCommentCategoryId = 1;

                                var courseCommentCategoryData = this.context?.CourseCommentCategory.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).OrderByDescending(x => x.CourseCommentId).FirstOrDefault();

                                if (courseCommentCategoryData != null)
                                {
                                    courseCommentCategoryId = courseCommentCategoryData.CourseCommentId + 1;
                                }

                                List<Course> courses = new();

                                foreach (var courseListData in courseList)
                                {
                                    Course course = new();

                                    course.TenantId = courseListData.TenantId;
                                    course.SchoolId = courseListData.SchoolId;
                                    course.CourseId = (int)courseId;
                                    course.CourseTitle = courseListData.CourseTitle;
                                    course.CourseShortName = courseListData.CourseShortName;
                                    course.CourseGradeLevel = courseListData.CourseGradeLevel;
                                    course.CourseProgram = courseListData.CourseProgram;
                                    course.CourseSubject = courseListData.CourseSubject;
                                    course.CourseCategory = courseListData.CourseCategory;
                                    course.CreditHours = courseListData.CreditHours;
                                    course.Standard = courseListData.Standard;
                                    course.StandardRefNo = courseListData.StandardRefNo;
                                    course.CourseDescription = courseListData.CourseDescription;
                                    course.IsCourseActive = courseListData.IsCourseActive;
                                    course.RolloverId = rolloverId;
                                    course.AcademicYear = rolloverViewModel.SchoolRollover.SchoolBeginDate != null ? Convert.ToDecimal(rolloverViewModel.SchoolRollover.SchoolBeginDate.Value.Year) : 0;
                                    course.CreatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                                    course.CreatedOn = DateTime.UtcNow;

                                    courses.Add(course);

                                    //Insert CourseCommentCategory
                                    var courseCommentCategoryList = this.context?.CourseCommentCategory.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.CourseId == courseListData.CourseId && x.AcademicYear == sessionCalendar!.AcademicYear).ToList();

                                    if (courseCommentCategoryList?.Any() == true)
                                    {
                                        List<CourseCommentCategory> courseCommentCategorys = new();

                                        foreach (var courseCommentCategoryListData in courseCommentCategoryList)
                                        {
                                            CourseCommentCategory courseCommentCategory = new();

                                            courseCommentCategory.TenantId = courseCommentCategoryListData.TenantId;
                                            courseCommentCategory.SchoolId = courseCommentCategoryListData.SchoolId;
                                            courseCommentCategory.CourseId = (int)courseId;
                                            courseCommentCategory.CourseCommentId = (int)courseCommentCategoryId;
                                            courseCommentCategory.CourseName = courseCommentCategoryListData.CourseName;
                                            courseCommentCategory.ApplicableAllCourses = courseCommentCategoryListData.ApplicableAllCourses;
                                            courseCommentCategory.Comments = courseCommentCategoryListData.Comments;
                                            courseCommentCategory.SortOrder = courseCommentCategoryListData.SortOrder;
                                            courseCommentCategory.RolloverId = rolloverId;
                                            courseCommentCategory.AcademicYear = rolloverViewModel.SchoolRollover.SchoolBeginDate != null ? Convert.ToDecimal(rolloverViewModel.SchoolRollover.SchoolBeginDate.Value.Year) : 0;
                                            courseCommentCategory.CreatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                                            courseCommentCategory.CreatedOn = DateTime.UtcNow;

                                            courseCommentCategorys.Add(courseCommentCategory);
                                            courseCommentCategoryId++;
                                        }
                                        this.context?.CourseCommentCategory.AddRange(courseCommentCategorys);
                                    }
                                    courseId++;
                                }
                                this.context?.Course.AddRange(courses);
                            }

                            //Insert GradeScale
                            var gradeScaleList = this.context?.GradeScale.Include(d => d.Grade).Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.AcademicYear == sessionCalendar!.AcademicYear).ToList();

                            if (gradeScaleList?.Any() == true)
                            {
                                int? gradeScaleId = 1;

                                var gradeScaleData = gradeScaleList.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).OrderByDescending(x => x.GradeScaleId).FirstOrDefault();

                                if (gradeScaleData != null)
                                {
                                    gradeScaleId = gradeScaleData.GradeScaleId + 1;
                                }

                                int? gradeId = 1;
                                var gradeData = this.context?.Grade.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).OrderByDescending(x => x.GradeId).FirstOrDefault();

                                if (gradeData != null)
                                {
                                    gradeId = gradeData.GradeId + 1;
                                }

                                List<GradeScale> gradeScales = new();

                                foreach (var gradeScaleListData in gradeScaleList)
                                {
                                    GradeScale gradeScale = new();

                                    gradeScale.TenantId = gradeScaleListData.TenantId;
                                    gradeScale.SchoolId = gradeScaleListData.SchoolId;
                                    gradeScale.GradeScaleId = (int)gradeScaleId;
                                    gradeScale.GradeScaleName = gradeScaleListData.GradeScaleName;
                                    gradeScale.GradeScaleValue = gradeScaleListData.GradeScaleValue;
                                    gradeScale.GradeScaleComment = gradeScaleListData.GradeScaleComment;
                                    gradeScale.CalculateGpa = gradeScaleListData.CalculateGpa;
                                    gradeScale.UseAsStandardGradeScale = gradeScaleListData.UseAsStandardGradeScale;
                                    gradeScale.SortOrder = gradeScaleListData.SortOrder;
                                    gradeScale.RolloverId = rolloverId;
                                    gradeScale.AcademicYear = rolloverViewModel.SchoolRollover.SchoolBeginDate != null ? Convert.ToDecimal(rolloverViewModel.SchoolRollover.SchoolBeginDate.Value.Year) : 0;
                                    gradeScale.CreatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                                    gradeScale.CreatedOn = DateTime.UtcNow;

                                    gradeScales.Add(gradeScale);

                                    //Insert Grade
                                    if (gradeScaleListData.Grade?.Any() == true)
                                    {
                                        List<Grade> grades = new();

                                        foreach (var grdData in gradeScaleListData.Grade)
                                        {
                                            Grade grade = new Grade();

                                            grade.TenantId = grdData.TenantId;
                                            grade.SchoolId = grdData.SchoolId;
                                            grade.GradeScaleId = (int)gradeScaleId;
                                            grade.GradeId = (int)gradeId;
                                            grade.Title = grdData.Title;
                                            grade.Breakoff = grdData.Breakoff;
                                            grade.WeightedGpValue = grdData.WeightedGpValue;
                                            grade.UnweightedGpValue = grdData.UnweightedGpValue;
                                            grade.Comment = grdData.Comment;
                                            grade.SortOrder = grdData.SortOrder;
                                            grade.CreatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                                            grade.CreatedOn = DateTime.UtcNow;

                                            grades.Add(grade);
                                            gradeId++;
                                        }
                                        this.context?.Grade.AddRange(grades);
                                    }
                                    gradeScaleId++;
                                }
                                this.context?.GradeScale.AddRange(gradeScales);
                            }

                            //Insert Rooms
                            var roomList = this.context?.Rooms.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.AcademicYear == sessionCalendar!.AcademicYear).ToList();

                            if (roomList?.Any() == true)
                            {
                                int? roomId = 1;

                                var roomData = roomList.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).OrderByDescending(x => x.RoomId).FirstOrDefault();

                                if (roomData != null)
                                {
                                    roomId = roomData.RoomId + 1;
                                }

                                List<Rooms> rooms = new();

                                foreach (var roomListData in roomList)
                                {
                                    Rooms room = new();

                                    room.TenantId = roomListData.TenantId;
                                    room.SchoolId = roomListData.SchoolId;
                                    room.RoomId = (int)roomId;
                                    room.Title = roomListData.Title;
                                    room.Capacity = roomListData.Capacity;
                                    room.Description = roomListData.Description;
                                    room.SortOrder = roomListData.SortOrder;
                                    room.IsActive = roomListData.IsActive;
                                    room.RolloverId = rolloverId;
                                    room.AcademicYear = rolloverViewModel.SchoolRollover.SchoolBeginDate != null ? Convert.ToDecimal(rolloverViewModel.SchoolRollover.SchoolBeginDate.Value.Year) : 0;
                                    room.CreatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                                    room.CreatedOn = DateTime.UtcNow;

                                    rooms.Add(room);
                                    roomId++;
                                }
                                this.context?.Rooms.AddRange(rooms);
                            }

                            //Insert Block
                            var blockList = this.context?.Block.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.AcademicYear == sessionCalendar!.AcademicYear).ToList();

                            if (blockList?.Any() == true)
                            {
                                int? blockId = 1;

                                var blockData = blockList.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).OrderByDescending(x => x.BlockId).FirstOrDefault();

                                if (blockData != null)
                                {
                                    blockId = blockData.BlockId + 1;
                                }

                                List<Block> blocks = new();

                                foreach (var blockListData in blockList)
                                {
                                    Block block = new();

                                    block.TenantId = blockListData.TenantId;
                                    block.SchoolId = blockListData.SchoolId;
                                    block.BlockId = (int)blockId;
                                    block.BlockTitle = blockListData.BlockTitle;
                                    block.BlockSortOrder = blockListData.BlockSortOrder;
                                    block.FullDayMinutes = blockListData.FullDayMinutes;
                                    block.HalfDayMinutes = blockListData.HalfDayMinutes;
                                    block.RolloverId = rolloverId;
                                    block.AcademicYear = rolloverViewModel.SchoolRollover.SchoolBeginDate != null ? Convert.ToDecimal(rolloverViewModel.SchoolRollover.SchoolBeginDate.Value.Year) : 0;
                                    block.CreatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                                    block.CreatedOn = DateTime.UtcNow;

                                    blocks.Add(block);

                                    //Insert BlockPeriod
                                    var blockPeriodList = this.context?.BlockPeriod.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.BlockId == blockListData.BlockId && x.AcademicYear == sessionCalendar!.AcademicYear).ToList();

                                    if (blockPeriodList?.Any() == true)
                                    {
                                        int? blockPeriodId = 1;

                                        var blockPeriodData = this.context?.BlockPeriod.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).OrderByDescending(x => x.PeriodId).FirstOrDefault();

                                        if (blockPeriodData != null)
                                        {
                                            blockPeriodId = blockPeriodData.PeriodId + 1;
                                        }

                                        List<BlockPeriod> blockPeriods = new();

                                        foreach (var blockPeriodListData in blockPeriodList)
                                        {
                                            BlockPeriod blockPeriod = new();

                                            blockPeriod.TenantId = blockPeriodListData.TenantId;
                                            blockPeriod.SchoolId = blockPeriodListData.SchoolId;
                                            blockPeriod.PeriodId = (int)blockPeriodId;
                                            blockPeriod.BlockId = (int)blockId;
                                            blockPeriod.PeriodTitle = blockPeriodListData.PeriodTitle;
                                            blockPeriod.PeriodShortName = blockPeriodListData.PeriodShortName;
                                            blockPeriod.PeriodStartTime = blockPeriodListData.PeriodStartTime;
                                            blockPeriod.PeriodEndTime = blockPeriodListData.PeriodEndTime;
                                            blockPeriod.PeriodSortOrder = blockPeriodListData.PeriodSortOrder;
                                            blockPeriod.CalculateAttendance = blockPeriodListData.CalculateAttendance;
                                            blockPeriod.RolloverId = rolloverId;
                                            blockPeriod.AcademicYear = rolloverViewModel.SchoolRollover.SchoolBeginDate != null ? Convert.ToDecimal(rolloverViewModel.SchoolRollover.SchoolBeginDate.Value.Year) : 0;
                                            blockPeriod.CreatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                                            blockPeriod.CreatedOn = DateTime.UtcNow;

                                            blockPeriods.Add(blockPeriod);
                                            blockPeriodId++;
                                        }
                                        this.context?.BlockPeriod.AddRange(blockPeriods);
                                    }
                                    blockId++;
                                }
                                this.context?.Block.AddRange(blocks);
                            }

                            //Insert StudentEnrollmentCode
                            var studentEnrollmentCodeList = this.context?.StudentEnrollmentCode.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.AcademicYear == sessionCalendar!.AcademicYear).ToList();

                            if (studentEnrollmentCodeList?.Any() == true)
                            {
                                int? studentEnrollmentCodeId = 1;

                                var studentEnrollmentCodeData = studentEnrollmentCodeList.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).OrderByDescending(x => x.EnrollmentCode).FirstOrDefault();

                                if (studentEnrollmentCodeData != null)
                                {
                                    studentEnrollmentCodeId = studentEnrollmentCodeData.EnrollmentCode + 1;
                                }

                                List<StudentEnrollmentCode> studentEnrollmentCodes = new();

                                foreach (var studentEnrollmentCodeListData in studentEnrollmentCodeList)
                                {
                                    StudentEnrollmentCode studentEnrollmentCode = new();

                                    studentEnrollmentCode.TenantId = studentEnrollmentCodeListData.TenantId;
                                    studentEnrollmentCode.SchoolId = studentEnrollmentCodeListData.SchoolId;
                                    studentEnrollmentCode.EnrollmentCode = (int)studentEnrollmentCodeId;
                                    studentEnrollmentCode.Title = studentEnrollmentCodeListData.Title;
                                    studentEnrollmentCode.ShortName = studentEnrollmentCodeListData.ShortName;
                                    studentEnrollmentCode.SortOrder = studentEnrollmentCodeListData.SortOrder;
                                    studentEnrollmentCode.Type = studentEnrollmentCodeListData.Type;
                                    studentEnrollmentCode.RolloverId = rolloverId;
                                    studentEnrollmentCode.AcademicYear = rolloverViewModel.SchoolRollover.SchoolBeginDate != null ? Convert.ToDecimal(rolloverViewModel.SchoolRollover.SchoolBeginDate.Value.Year) : 0;
                                    studentEnrollmentCode.CreatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                                    studentEnrollmentCode.CreatedOn = DateTime.UtcNow;

                                    studentEnrollmentCodes.Add(studentEnrollmentCode);
                                    studentEnrollmentCodeId++;
                                }
                                this.context?.StudentEnrollmentCode.AddRange(studentEnrollmentCodes);
                            }

                            //Insert HistoricalMarkingPeriod
                            var historicalMarkingPeriodList = this.context?.HistoricalMarkingPeriod.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).ToList();

                            if (historicalMarkingPeriodList?.Any() == true)
                            {
                                int? historicalMarkingPeriodId = 1;

                                var historicalMarkingPeriodData = historicalMarkingPeriodList.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).OrderByDescending(x => x.HistMarkingPeriodId).FirstOrDefault();

                                if (historicalMarkingPeriodData != null)
                                {
                                    historicalMarkingPeriodId = historicalMarkingPeriodData.HistMarkingPeriodId + 1;
                                }

                                List<HistoricalMarkingPeriod> historicalMarkingPeriods = new();

                                foreach (var historicalMarkingPeriodListData in historicalMarkingPeriodList)
                                {
                                    HistoricalMarkingPeriod historicalMarkingPeriod = new();

                                    historicalMarkingPeriod.TenantId = historicalMarkingPeriodListData.TenantId;
                                    historicalMarkingPeriod.SchoolId = historicalMarkingPeriodListData.SchoolId;
                                    historicalMarkingPeriod.HistMarkingPeriodId = (int)historicalMarkingPeriodId;
                                    historicalMarkingPeriod.Title = historicalMarkingPeriodListData.Title;
                                    historicalMarkingPeriod.GradePostDate = historicalMarkingPeriodListData.GradePostDate;
                                    historicalMarkingPeriod.DoesGrades = historicalMarkingPeriodListData.DoesGrades;
                                    historicalMarkingPeriod.DoesExam = historicalMarkingPeriodListData.DoesExam;
                                    historicalMarkingPeriod.DoesComments = historicalMarkingPeriodListData.DoesComments;
                                    historicalMarkingPeriod.RolloverId = rolloverId;
                                    historicalMarkingPeriod.AcademicYear = historicalMarkingPeriodListData.AcademicYear;
                                    historicalMarkingPeriod.CreatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                                    historicalMarkingPeriod.CreatedOn = DateTime.UtcNow;

                                    historicalMarkingPeriods.Add(historicalMarkingPeriod);
                                    historicalMarkingPeriodId++;
                                }
                                this.context?.HistoricalMarkingPeriod.AddRange(historicalMarkingPeriods);
                            }

                            //Insert AttendanceCodeCategories
                            var attendanceCodeCategorieList = this.context?.AttendanceCodeCategories.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.AcademicYear == sessionCalendar!.AcademicYear).ToList();

                            if (attendanceCodeCategorieList?.Any() == true)
                            {
                                int? attendanceCodeCategorieId = 1;

                                var attendanceCodeCategorieData = attendanceCodeCategorieList.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).OrderByDescending(x => x.AttendanceCategoryId).FirstOrDefault();

                                if (attendanceCodeCategorieData != null)
                                {
                                    attendanceCodeCategorieId = attendanceCodeCategorieData.AttendanceCategoryId + 1;
                                }

                                List<AttendanceCodeCategories> attendanceCodeCategories = new();

                                foreach (var attendanceCodeCategorieListData in attendanceCodeCategorieList)
                                {
                                    AttendanceCodeCategories attendanceCodeCategorie = new();

                                    attendanceCodeCategorie.TenantId = attendanceCodeCategorieListData.TenantId;
                                    attendanceCodeCategorie.SchoolId = attendanceCodeCategorieListData.SchoolId;
                                    attendanceCodeCategorie.AttendanceCategoryId = (int)attendanceCodeCategorieId;
                                    attendanceCodeCategorie.Title = attendanceCodeCategorieListData.Title;
                                    attendanceCodeCategorie.RolloverId = rolloverId;
                                    attendanceCodeCategorie.AcademicYear = rolloverViewModel.SchoolRollover.SchoolBeginDate != null ? Convert.ToDecimal(rolloverViewModel.SchoolRollover.SchoolBeginDate.Value.Year) : 0;
                                    attendanceCodeCategorie.CreatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                                    attendanceCodeCategorie.CreatedOn = DateTime.UtcNow;

                                    attendanceCodeCategories.Add(attendanceCodeCategorie);

                                    //Insert AttendanceCode
                                    var attendanceCodeList = this.context?.AttendanceCode.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.AttendanceCategoryId == attendanceCodeCategorieListData.AttendanceCategoryId && x.AcademicYear == sessionCalendar!.AcademicYear).ToList();

                                    if (attendanceCodeList?.Any() == true)
                                    {
                                        int? attendanceCodeId = 1;

                                        var attendanceCodeData = this.context?.AttendanceCode.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).OrderByDescending(x => x.AttendanceCode1).FirstOrDefault();

                                        if (attendanceCodeData != null)
                                        {
                                            attendanceCodeId = attendanceCodeData.AttendanceCode1 + 1;
                                        }

                                        List<AttendanceCode> attendanceCodes = new();

                                        foreach (var attendanceCodeeListData in attendanceCodeList)
                                        {
                                            AttendanceCode attendanceCode = new();

                                            attendanceCode.TenantId = attendanceCodeeListData.TenantId;
                                            attendanceCode.SchoolId = attendanceCodeeListData.SchoolId;
                                            attendanceCode.AttendanceCategoryId = (int)attendanceCodeCategorieId;
                                            attendanceCode.AttendanceCode1 = (int)attendanceCodeId;
                                            attendanceCode.Title = attendanceCodeeListData.Title;
                                            attendanceCode.ShortName = attendanceCodeeListData.ShortName;
                                            attendanceCode.Type = attendanceCodeeListData.Type;
                                            attendanceCode.StateCode = attendanceCodeeListData.StateCode;
                                            attendanceCode.DefaultCode = attendanceCodeeListData.DefaultCode;
                                            attendanceCode.AllowEntryBy = attendanceCodeeListData.AllowEntryBy;
                                            attendanceCode.SortOrder = attendanceCodeeListData.SortOrder;
                                            attendanceCode.RolloverId = rolloverId;
                                            attendanceCode.AcademicYear = rolloverViewModel.SchoolRollover.SchoolBeginDate != null ? Convert.ToDecimal(rolloverViewModel.SchoolRollover.SchoolBeginDate.Value.Year) : 0;
                                            attendanceCode.CreatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                                            attendanceCode.CreatedOn = DateTime.UtcNow;

                                            attendanceCodes.Add(attendanceCode);
                                            attendanceCodeId++;
                                        }
                                        this.context?.AttendanceCode.AddRange(attendanceCodes);
                                    }
                                    attendanceCodeCategorieId++;
                                }
                                this.context?.AttendanceCodeCategories.AddRange(attendanceCodeCategories);
                            }

                            //Insert HonorRolls
                            var honorRollsList = this.context?.HonorRolls.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.AcademicYear == sessionCalendar!.AcademicYear).ToList();

                            if (honorRollsList?.Any() == true)
                            {
                                int? honorRollsId = 1;

                                var honorRollsData = honorRollsList.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).OrderByDescending(x => x.HonorRollId).FirstOrDefault();

                                if (honorRollsData != null)
                                {
                                    honorRollsId = honorRollsData.HonorRollId + 1;
                                }

                                List<HonorRolls> honorRolls = new();

                                foreach (var honorRollsListData in honorRollsList)
                                {
                                    HonorRolls honorRoll = new();

                                    honorRoll.TenantId = honorRollsListData.TenantId;
                                    honorRoll.SchoolId = honorRollsListData.SchoolId;
                                    honorRoll.HonorRollId = (int)honorRollsId;
                                    honorRoll.HonorRoll = honorRollsListData.HonorRoll;
                                    honorRoll.Breakoff = honorRollsListData.Breakoff;
                                    honorRoll.RolloverId = rolloverId;
                                    honorRoll.AcademicYear = rolloverViewModel.SchoolRollover.SchoolBeginDate != null ? Convert.ToDecimal(rolloverViewModel.SchoolRollover.SchoolBeginDate.Value.Year) : 0;
                                    honorRoll.CreatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                                    honorRoll.CreatedOn = DateTime.UtcNow;

                                    honorRolls.Add(honorRoll);
                                    honorRollsId++;
                                }
                                this.context?.HonorRolls.AddRange(honorRolls);
                            }

                            //Insert Semester
                            List<Semesters> semesters = new();
                            if (rolloverViewModel.Semesters.Count > 0)
                            {
                                int? semesterId = 1;

                                var semesterData = this.context?.Semesters.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).OrderByDescending(x => x.MarkingPeriodId).FirstOrDefault();

                                if (semesterData != null)
                                {
                                    semesterId = semesterData.MarkingPeriodId + 1;
                                }

                                int? quarterId = 1;

                                var quarterData = this.context?.Quarters.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).OrderByDescending(x => x.MarkingPeriodId).FirstOrDefault();

                                if (quarterData != null)
                                {
                                    quarterId = quarterData.MarkingPeriodId + 1;
                                }

                                int? progressPeriodId = 1;

                                var progressPeriodData = this.context?.ProgressPeriods.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).OrderByDescending(x => x.MarkingPeriodId).FirstOrDefault();

                                if (progressPeriodData != null)
                                {
                                    progressPeriodId = quarterData?.MarkingPeriodId + 1;
                                }

                                foreach (var sem in rolloverViewModel.Semesters)
                                {
                                    if (sem.StartDate != null)
                                    {
                                        var semester = new Semesters()
                                        {
                                            TenantId = rolloverViewModel.SchoolRollover.TenantId,
                                            SchoolId = rolloverViewModel.SchoolRollover.SchoolId,
                                            MarkingPeriodId = (int)semesterId,
                                            YearId = (int)yearId,
                                            AcademicYear = schoolYears.AcademicYear,
                                            Title = sem.Title,
                                            ShortName = sem.ShortName,
                                            StartDate = sem.StartDate,
                                            EndDate = sem.EndDate,
                                            DoesGrades = sem.DoesGrades,
                                            DoesExam = sem.DoesExam,
                                            DoesComments = sem.DoesComments,
                                            RolloverId = rolloverId,
                                            CreatedOn = DateTime.UtcNow,
                                            CreatedBy = rolloverViewModel.SchoolRollover.CreatedBy
                                        };
                                        semesters.Add(semester);

                                        //Insert Quarter
                                        if (sem.Quarters.Count > 0)
                                        {
                                            List<Quarters> quarters = new();
                                            foreach (var qtr in sem.Quarters)
                                            {
                                                if (qtr.StartDate != null)
                                                {
                                                    var quarter = new Quarters()
                                                    {
                                                        TenantId = rolloverViewModel.SchoolRollover.TenantId,
                                                        SchoolId = rolloverViewModel.SchoolRollover.SchoolId,
                                                        MarkingPeriodId = (int)quarterId,
                                                        SemesterId = (int)semesterId,
                                                        AcademicYear = schoolYears.AcademicYear,
                                                        Title = qtr.Title,
                                                        ShortName = qtr.ShortName,
                                                        StartDate = qtr.StartDate,
                                                        EndDate = qtr.EndDate,
                                                        DoesGrades = qtr.DoesGrades,
                                                        DoesExam = qtr.DoesExam,
                                                        DoesComments = qtr.DoesComments,
                                                        RolloverId = rolloverId,
                                                        CreatedOn = DateTime.UtcNow,
                                                        CreatedBy = rolloverViewModel.SchoolRollover.CreatedBy
                                                    };
                                                    quarters.Add(quarter);

                                                    //Insert Progress Period
                                                    if (qtr.ProgressPeriods.Count > 0)
                                                    {
                                                        List<ProgressPeriods> progressPeriods = new();
                                                        foreach (var prgprd in qtr.ProgressPeriods)
                                                        {
                                                            if (prgprd.StartDate != null)
                                                            {
                                                                var progressPeriod = new ProgressPeriods()
                                                                {
                                                                    TenantId = rolloverViewModel.SchoolRollover.TenantId,
                                                                    SchoolId = rolloverViewModel.SchoolRollover.SchoolId,
                                                                    MarkingPeriodId = progressPeriodId != null ? (int)progressPeriodId : 0,
                                                                    QuarterId = (int)quarterId,
                                                                    AcademicYear = schoolYears.AcademicYear != null ? (decimal)schoolYears.AcademicYear : 0,
                                                                    Title = prgprd.Title,
                                                                    ShortName = prgprd.ShortName,
                                                                    StartDate = prgprd.StartDate,
                                                                    EndDate = prgprd.EndDate,
                                                                    DoesGrades = prgprd.DoesGrades,
                                                                    DoesExam = prgprd.DoesExam,
                                                                    DoesComments = prgprd.DoesComments,
                                                                    RolloverId = rolloverId,
                                                                    CreatedOn = DateTime.UtcNow,
                                                                    CreatedBy = rolloverViewModel.SchoolRollover.CreatedBy
                                                                };
                                                                progressPeriods.Add(progressPeriod);

                                                                progressPeriodId++;
                                                            }
                                                        }
                                                        this.context?.ProgressPeriods.AddRange(progressPeriods);
                                                    }
                                                    quarterId++;
                                                }
                                            }
                                            this.context?.Quarters.AddRange(quarters);
                                        }
                                        semesterId++;
                                    }
                                }
                                this.context?.Semesters.AddRange(semesters);
                            }

                            this.context?.SaveChanges();

                            //Reenroll Students
                            var studentMasterData = this.context?.StudentMaster.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.IsActive == true).ToList();
                            var GradelevelsData = this.context?.Gradelevels.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).ToList();

                            List<(int? schoolId, int maxStudentId)> idList = new List<(int? schoolId, int maxStudentId)>();

                            if (studentMasterData != null && studentMasterData.Any())
                            {
                                //for insert in job fetch max id
                                long? Id = 1;
                                var dataExits = this.context?.ScheduledJobs.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId);
                                if (dataExits?.Any() == true)
                                {
                                    var scheduledJobData = this.context?.ScheduledJobs.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId).Max(x => x.JobId);
                                    if (scheduledJobData != null)
                                    {
                                        Id = scheduledJobData + 1;
                                    }
                                }

                                foreach (var studentMaster in studentMasterData)
                                {
                                    int? EnrollmentId = 1;

                                    var enrollmentData = this.context?.StudentEnrollment.Where(x => x.StudentGuid == studentMaster.StudentGuid).OrderByDescending(x => x.EnrollmentId).FirstOrDefault();

                                    if (enrollmentData != null)
                                    {
                                        EnrollmentId = enrollmentData.EnrollmentId + 1;
                                    }

                                    var studentEnrollmentData = this.context?.StudentEnrollment.FirstOrDefault(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.StudentId == studentMaster.StudentId && x.IsActive == true && x.ExitCode == null);

                                    if (studentEnrollmentData != null)
                                    {
                                        var studentData = this.context?.StudentMaster.FirstOrDefault(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == studentEnrollmentData.SchoolId && x.StudentId == studentEnrollmentData.StudentId);

                                        if (studentData != null)
                                        {
                                            if (studentEnrollmentData.RollingOption?.ToLower() == "Next grade at current school".ToLower())
                                            {
                                                //Fetching enrollment code where student Rolled Over.
                                                var studentRollOver = this.context?.StudentEnrollmentCode.FirstOrDefault(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.Type == "Rolled Over");

                                                studentEnrollmentData.ExitDate = rolloverViewModel.SchoolRollover.ReenrollmentDate;
                                                studentEnrollmentData.ExitCode = studentRollOver?.Title;
                                                studentEnrollmentData.UpdatedOn = DateTime.UtcNow;
                                                studentEnrollmentData.UpdatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                                                studentEnrollmentData.IsActive = false;

                                                //Fetching all GradeLevel of school.
                                                var gradeLevelData = this.context?.Gradelevels.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).ToList();

                                                //Fetching enrolled GradeLevel of student.
                                                var enrolledGradeLevelData = gradeLevelData?.FirstOrDefault(x => x.GradeId == studentEnrollmentData.GradeId);

                                                //Fetching next GradeLevel for student.
                                                if (enrolledGradeLevelData?.NextGradeId != null)
                                                {
                                                    StudentEnrollment studentEnrollment = new();
                                                    studentEnrollment.SchoolId = rolloverViewModel.SchoolRollover.SchoolId;
                                                    studentEnrollment.TenantId = rolloverViewModel.SchoolRollover.TenantId;
                                                    studentEnrollment.StudentId = studentMaster.StudentId;
                                                    studentEnrollment.StudentGuid = studentMaster.StudentGuid;
                                                    studentEnrollment.EnrollmentId = (int)EnrollmentId;
                                                    studentEnrollment.EnrollmentDate = rolloverViewModel.SchoolRollover.ReenrollmentDate;
                                                    studentEnrollment.EnrollmentCode = studentRollOver?.Title;
                                                    studentEnrollment.SchoolName = studentEnrollmentData.SchoolName;
                                                    studentEnrollment.RolloverId = rolloverId;
                                                    //studentEnrollment.CalenderId = calenderId;
                                                    studentEnrollment.CalenderId = schoolSessionCalendar.CalenderId;
                                                    studentEnrollment.RollingOption = studentEnrollmentData.RollingOption;
                                                    studentEnrollment.UpdatedOn = DateTime.UtcNow;
                                                    studentEnrollment.UpdatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                                                    studentEnrollment.IsActive = true;

                                                    var nextGradeLevelData = gradeLevelData?.FirstOrDefault(x => x.GradeId == enrolledGradeLevelData.NextGradeId);

                                                    studentEnrollmentData.TransferredGrade = nextGradeLevelData?.Title;

                                                    studentEnrollment.GradeLevelTitle = nextGradeLevelData?.Title;
                                                    studentEnrollment.GradeId = nextGradeLevelData?.GradeId;
                                                    this.context?.StudentEnrollment.Add(studentEnrollment);
                                                }
                                                else
                                                {
                                                    var dropCode = this.context?.StudentEnrollmentCode.FirstOrDefault(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.Type == "Drop");
                                                    studentEnrollmentData.IsActive = true;
                                                    studentEnrollmentData.ExitCode = dropCode?.Title;
                                                    studentEnrollmentData.RollingOption = "Do not enroll after this school year";
                                                    studentEnrollmentData.RolloverId = rolloverId;

                                                    if (rolloverViewModel.SchoolRollover.ReenrollmentDate != null
                                               && rolloverViewModel.SchoolRollover.ReenrollmentDate.Value.Date <= DateTime.UtcNow.Date) //drop student in previous date
                                                    {
                                                        //Deactive student from student master
                                                        this.context?.StudentMaster.Where(x => x.StudentGuid == studentEnrollmentData.StudentGuid && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.TenantId == rolloverViewModel.SchoolRollover.TenantId).ToList().ForEach(x => x.IsActive = false);

                                                    }
                                                    else
                                                    {
                                                        var studentEnrollmentListModel = new StudentEnrollmentListModel { _tenantName = rolloverViewModel._tenantName, TenantId = rolloverViewModel.SchoolRollover.TenantId, SchoolId = rolloverViewModel.SchoolRollover.SchoolId, StudentGuid = studentData.StudentGuid, studentEnrollments = new List<StudentEnrollment>() { new StudentEnrollment { TenantId = rolloverViewModel.SchoolRollover.TenantId, SchoolId = rolloverViewModel.SchoolRollover.SchoolId, StudentId = studentMaster.StudentId, ExitCode = dropCode?.EnrollmentCode.ToString(), ExitDate = rolloverViewModel.SchoolRollover.ReenrollmentDate, StudentGuid = studentEnrollmentData.StudentGuid, EnrollmentId = studentEnrollmentData.EnrollmentId, UpdatedBy = rolloverViewModel.SchoolRollover.UpdatedBy } } };

                                                        //insert job if date today or in future
                                                        var scheduledJob = new ScheduledJob
                                                        {
                                                            TenantId = rolloverViewModel.SchoolRollover.TenantId,
                                                            SchoolId = rolloverViewModel.SchoolRollover.SchoolId,
                                                            JobId = (long)Id,
                                                            AcademicYear = rolloverViewModel._academicYear,
                                                            JobTitle = "StudentEnrollmentDropTransferStudent",
                                                            JobScheduleDate = rolloverViewModel.SchoolRollover.ReenrollmentDate!.Value.AddDays(1),
                                                            ApiTitle = "UpdateStudentEnrollment",
                                                            ControllerPath = studentEnrollmentListModel._tenantName + "/Rollover",
                                                            TaskJson = JsonConvert.SerializeObject(studentEnrollmentListModel),
                                                            LastRunStatus = null,
                                                            LastRunTime = null,
                                                            IsActive = true,
                                                            CreatedBy = rolloverViewModel.SchoolRollover.UpdatedBy,
                                                            CreatedOn = DateTime.UtcNow
                                                        };
                                                        this.context?.ScheduledJobs.Add(scheduledJob);
                                                        Id++;
                                                    }
                                                }

                                                //this.context?.SaveChanges();
                                            }
                                            else if (studentEnrollmentData.RollingOption?.ToLower() == "Retain".ToLower())
                                            {
                                                //Fetching enrollment code where student Drop.
                                                var studentRollOver = this.context?.StudentEnrollmentCode.FirstOrDefault(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.Type == "Drop");

                                                studentEnrollmentData.ExitDate = rolloverViewModel.SchoolRollover.ReenrollmentDate;
                                                studentEnrollmentData.ExitCode = studentRollOver?.Title;
                                                studentEnrollmentData.TransferredGrade = studentEnrollmentData.GradeLevelTitle;
                                                studentEnrollmentData.UpdatedOn = DateTime.UtcNow;
                                                studentEnrollmentData.UpdatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                                                studentEnrollmentData.IsActive = false;
                                                studentEnrollmentData.RolloverId = rolloverId;

                                                StudentEnrollment studentEnrollment = new();
                                                studentEnrollment.SchoolId = rolloverViewModel.SchoolRollover.SchoolId;
                                                studentEnrollment.TenantId = rolloverViewModel.SchoolRollover.TenantId;
                                                studentEnrollment.StudentId = studentMaster.StudentId;
                                                studentEnrollment.StudentGuid = studentMaster.StudentGuid;
                                                studentEnrollment.EnrollmentId = (int)EnrollmentId;
                                                studentEnrollment.EnrollmentDate = rolloverViewModel.SchoolRollover.ReenrollmentDate;
                                                studentEnrollment.EnrollmentCode = studentRollOver?.Title;
                                                studentEnrollment.SchoolName = studentEnrollmentData.SchoolName;
                                                studentEnrollment.GradeLevelTitle = studentEnrollmentData.GradeLevelTitle;
                                                studentEnrollment.GradeId = studentEnrollmentData.GradeId;
                                                //studentEnrollment.CalenderId = calenderId;
                                                studentEnrollment.CalenderId = schoolSessionCalendar.CalenderId;
                                                studentEnrollment.RollingOption = "Next grade at current school";
                                                studentEnrollment.UpdatedOn = DateTime.UtcNow;
                                                studentEnrollment.UpdatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                                                studentEnrollment.IsActive = true;
                                                studentEnrollment.RolloverId = rolloverId;

                                                this.context?.StudentEnrollment.Add(studentEnrollment);
                                                //this.context?.SaveChanges();
                                            }
                                            else if (studentEnrollmentData.RollingOption?.ToLower() == "Do not enroll after this school year".ToLower())
                                            {
                                                //Fetching enrollment code where student Drop Out.
                                                var studentRollOver = this.context?.StudentEnrollmentCode.FirstOrDefault(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.Type == "Drop");

                                                studentEnrollmentData.ExitDate = rolloverViewModel.SchoolRollover.ReenrollmentDate;
                                                studentEnrollmentData.ExitCode = studentRollOver?.Title;
                                                studentEnrollmentData.UpdatedOn = DateTime.UtcNow;
                                                studentEnrollmentData.UpdatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                                                studentEnrollmentData.IsActive = true;
                                                studentEnrollmentData.RolloverId = rolloverId;

                                                if (rolloverViewModel.SchoolRollover.ReenrollmentDate != null
                                                && rolloverViewModel.SchoolRollover.ReenrollmentDate.Value.Date <= DateTime.UtcNow.Date) //drop student in previous date
                                                {
                                                    //Deactive student from student master
                                                    this.context?.StudentMaster.Where(x => x.StudentGuid == studentEnrollmentData.StudentGuid && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId).ToList().ForEach(x => x.IsActive = false);

                                                }
                                                else
                                                {
                                                    var studentEnrollmentListModel = new StudentEnrollmentListModel { _tenantName = rolloverViewModel._tenantName, TenantId = rolloverViewModel.SchoolRollover.TenantId, SchoolId = rolloverViewModel.SchoolRollover.SchoolId, StudentGuid = studentData.StudentGuid, studentEnrollments = new List<StudentEnrollment>() { new StudentEnrollment { TenantId = rolloverViewModel.SchoolRollover.TenantId, SchoolId = rolloverViewModel.SchoolRollover.SchoolId, StudentId = studentMaster.StudentId, ExitCode = studentRollOver?.EnrollmentCode.ToString(), ExitDate = rolloverViewModel.SchoolRollover.ReenrollmentDate, StudentGuid = studentEnrollmentData.StudentGuid, EnrollmentId = studentEnrollmentData.EnrollmentId, UpdatedBy = rolloverViewModel.SchoolRollover.UpdatedBy } } };

                                                    //insert job if date today or in future
                                                    var scheduledJob = new ScheduledJob
                                                    {
                                                        TenantId = rolloverViewModel.SchoolRollover.TenantId,
                                                        SchoolId = rolloverViewModel.SchoolRollover.SchoolId,
                                                        JobId = (long)Id,
                                                        AcademicYear = rolloverViewModel._academicYear,
                                                        JobTitle = "StudentEnrollmentDropTransferStudent",
                                                        JobScheduleDate = rolloverViewModel.SchoolRollover.ReenrollmentDate!.Value.AddDays(1),
                                                        ApiTitle = "UpdateStudentEnrollment",
                                                        ControllerPath = studentEnrollmentListModel._tenantName + "/Rollover",
                                                        TaskJson = JsonConvert.SerializeObject(studentEnrollmentListModel),
                                                        LastRunStatus = null,
                                                        LastRunTime = null,
                                                        IsActive = true,
                                                        CreatedBy = rolloverViewModel.SchoolRollover.UpdatedBy,
                                                        CreatedOn = DateTime.UtcNow
                                                    };
                                                    this.context?.ScheduledJobs.Add(scheduledJob);
                                                    Id++;
                                                }
                                            }
                                            else if (studentEnrollmentData.RollingOption?.ToLower() == "Enrol to another school".ToLower())
                                            {
                                                if (studentEnrollmentData.EnrollOtherSchoolId != null)
                                                {
                                                    int MasterStudentId = 1;

                                                    var checkId = idList.OrderByDescending(x => x.maxStudentId).FirstOrDefault(s => s.schoolId == studentEnrollmentData.EnrollOtherSchoolId);

                                                    if (checkId.schoolId == null)
                                                    {
                                                        var studentDataForTransferredSchool = this.context?.StudentMaster.Where(x => x.SchoolId == studentEnrollmentData.EnrollOtherSchoolId && x.TenantId == rolloverViewModel.SchoolRollover.TenantId).OrderByDescending(x => x.StudentId).FirstOrDefault();

                                                        if (studentDataForTransferredSchool != null)
                                                        {
                                                            MasterStudentId = studentDataForTransferredSchool.StudentId + 1;
                                                        }

                                                        idList.Add((studentEnrollmentData.EnrollOtherSchoolId, MasterStudentId));
                                                    }
                                                    else
                                                    {
                                                        MasterStudentId = checkId.maxStudentId + 1;
                                                        idList.Add((studentEnrollmentData.EnrollOtherSchoolId, MasterStudentId));
                                                    }

                                                    var studentRollOverCode = this.context?.StudentEnrollmentCode.FirstOrDefault(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.Type == "Rolled Over");

                                                    var studentTransferCode = this.context?.StudentEnrollmentCode.FirstOrDefault(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.Type == "Drop (Transfer)");

                                                    //update existing enrollment
                                                    studentEnrollmentData.ExitDate = rolloverViewModel.SchoolRollover.ReenrollmentDate;
                                                    studentEnrollmentData.ExitCode = studentRollOverCode?.Title;
                                                    studentEnrollmentData.UpdatedOn = DateTime.UtcNow;
                                                    studentEnrollmentData.UpdatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                                                    studentEnrollmentData.IsActive = false;
                                                    studentEnrollmentData.RolloverId = rolloverId;

                                                    //fetching enrollment code where student enroll(transfer).
                                                    var studentTransferIn = this.context?.StudentEnrollmentCode.FirstOrDefault(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == studentEnrollmentData.EnrollOtherSchoolId && x.Type != null && x.Type.ToLower() == "Rolled Over".ToLower());

                                                    if (studentTransferIn != null)
                                                    {
                                                        if (rolloverViewModel.SchoolRollover.ReenrollmentDate != null
                                                       && rolloverViewModel.SchoolRollover.ReenrollmentDate.Value.Date > DateTime.UtcNow.Date)
                                                        {
                                                            var studentEnrollmentListModel = new StudentEnrollmentListModel { _tenantName = rolloverViewModel._tenantName, TenantId = rolloverViewModel.SchoolRollover.TenantId, SchoolId = rolloverViewModel.SchoolRollover.SchoolId, StudentGuid = studentData.StudentGuid, studentEnrollments = new List<StudentEnrollment>() { new StudentEnrollment { TenantId = rolloverViewModel.SchoolRollover.TenantId, SchoolId = rolloverViewModel.SchoolRollover.SchoolId, StudentId = studentMaster.StudentId, ExitCode = studentTransferCode?.EnrollmentCode.ToString(), ExitDate = rolloverViewModel.SchoolRollover.ReenrollmentDate, StudentGuid = studentEnrollmentData.StudentGuid, TransferredSchoolId = studentEnrollmentData.EnrollOtherSchoolId, EnrollmentId = studentEnrollmentData.EnrollmentId, UpdatedBy = rolloverViewModel.SchoolRollover.UpdatedBy } } };

                                                            //insert job if date today or in future
                                                            var scheduledJob = new ScheduledJob
                                                            {
                                                                TenantId = rolloverViewModel.SchoolRollover.TenantId,
                                                                SchoolId = rolloverViewModel.SchoolRollover.SchoolId,
                                                                JobId = (long)Id,
                                                                AcademicYear = rolloverViewModel._academicYear,
                                                                JobTitle = "StudentEnrollmentDropTransferStudent",
                                                                JobScheduleDate = rolloverViewModel.SchoolRollover.ReenrollmentDate!.Value.AddDays(1),
                                                                ApiTitle = "UpdateStudentEnrollment",
                                                                ControllerPath = studentEnrollmentListModel._tenantName + "/Rollover",
                                                                TaskJson = JsonConvert.SerializeObject(studentEnrollmentListModel),
                                                                LastRunStatus = null,
                                                                LastRunTime = null,
                                                                IsActive = true,
                                                                CreatedBy = rolloverViewModel.SchoolRollover.UpdatedBy,
                                                                CreatedOn = DateTime.UtcNow
                                                            };
                                                            this.context?.ScheduledJobs.Add(scheduledJob);
                                                            Id++;
                                                        }

                                                        //fetching student details from studentMaster table for the new school if exist previously
                                                        var checkStudentAlreadyExistInTransferredSchool = this.context?.StudentMaster.FirstOrDefault(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == studentEnrollmentData.EnrollOtherSchoolId && x.StudentGuid == studentEnrollmentData.StudentGuid);

                                                        if (checkStudentAlreadyExistInTransferredSchool != null)
                                                        {
                                                            if (rolloverViewModel.SchoolRollover.ReenrollmentDate != null
                                                           && rolloverViewModel.SchoolRollover.ReenrollmentDate.Value.Date <= DateTime.UtcNow.Date) //drop student in previous date
                                                            {
                                                                var studentOldSchool = this.context?.StudentMaster.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId != studentEnrollmentData.EnrollOtherSchoolId && x.StudentGuid == studentEnrollmentData.StudentGuid).ToList();
                                                                if (studentOldSchool?.Any() == true)
                                                                {
                                                                    studentOldSchool.ForEach(x => x.IsActive = false);
                                                                    checkStudentAlreadyExistInTransferredSchool.EnrollmentType = "Internal";
                                                                    checkStudentAlreadyExistInTransferredSchool.IsActive = true;
                                                                }
                                                            }

                                                            MasterStudentId = (int)checkStudentAlreadyExistInTransferredSchool.StudentId;
                                                        }
                                                        else
                                                        {
                                                            if (studentData != null)
                                                            {
                                                                if (rolloverViewModel.SchoolRollover.ReenrollmentDate != null
                                                            && rolloverViewModel.SchoolRollover.ReenrollmentDate.Value.Date <= DateTime.UtcNow.Date) //drop student in previous date
                                                                {
                                                                    var studentOldSchool = this.context?.StudentMaster.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.StudentGuid == studentEnrollmentData.StudentGuid).ToList();
                                                                    if (studentOldSchool?.Any() == true)
                                                                    {
                                                                        studentOldSchool.ForEach(x => x.IsActive = false);
                                                                    }
                                                                }


                                                                var checkInternalId = this.context?.StudentMaster.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == studentEnrollmentData.EnrollOtherSchoolId && x.StudentInternalId == studentData.StudentInternalId).ToList();

                                                                string? StudentInternalId = null;

                                                                if (checkInternalId?.Any() == true)
                                                                {
                                                                    StudentInternalId = MasterStudentId.ToString();
                                                                }
                                                                else
                                                                {
                                                                    StudentInternalId = studentData.StudentInternalId;
                                                                }

                                                                StudentMaster studentMasterAdd = new StudentMaster() { TenantId = studentData.TenantId, SchoolId = (int)studentEnrollmentData.EnrollOtherSchoolId!, StudentId = (int)MasterStudentId, AlternateId = studentData.AlternateId, DistrictId = studentData.DistrictId, StateId = studentData.StateId, AdmissionNumber = studentData.AdmissionNumber, RollNumber = studentData.RollNumber, Salutation = studentData.Salutation, FirstGivenName = studentData.FirstGivenName, MiddleName = studentData.MiddleName, LastFamilyName = studentData.LastFamilyName, Suffix = studentData.Suffix, PreferredName = studentData.PreferredName, PreviousName = studentData.PreviousName, SocialSecurityNumber = studentData.SocialSecurityNumber, OtherGovtIssuedNumber = studentData.OtherGovtIssuedNumber, StudentPhoto = studentData.StudentPhoto, StudentThumbnailPhoto = studentData.StudentThumbnailPhoto, Dob = studentData.Dob, Gender = studentData.Gender, Race = studentData.Race, Ethnicity = studentData.Ethnicity, MaritalStatus = studentData.MaritalStatus, CountryOfBirth = studentData.CountryOfBirth, Nationality = studentData.Nationality, FirstLanguageId = studentData.FirstLanguageId, SecondLanguageId = studentData.SecondLanguageId, ThirdLanguageId = studentData.ThirdLanguageId, HomePhone = studentData.HomePhone, MobilePhone = studentData.MobilePhone, PersonalEmail = studentData.PersonalEmail, SchoolEmail = studentData.SchoolEmail, Twitter = studentData.Twitter, Facebook = studentData.Facebook, Instagram = studentData.Instagram, Youtube = studentData.Youtube, Linkedin = studentData.Linkedin, HomeAddressLineOne = studentData.HomeAddressLineOne, HomeAddressLineTwo = studentData.HomeAddressLineTwo, HomeAddressCountry = studentData.HomeAddressCountry, HomeAddressState = studentData.HomeAddressState, HomeAddressCity = studentData.HomeAddressCity, HomeAddressZip = studentData.HomeAddressZip, BusNo = studentData.BusNo, SchoolBusPickUp = studentData.SchoolBusPickUp, SchoolBusDropOff = studentData.SchoolBusDropOff, MailingAddressSameToHome = studentData.MailingAddressSameToHome, MailingAddressLineOne = studentData.MailingAddressLineOne, MailingAddressLineTwo = studentData.MailingAddressLineTwo, MailingAddressCountry = studentData.MailingAddressCountry, MailingAddressState = studentData.MailingAddressState, MailingAddressCity = studentData.MailingAddressCity, MailingAddressZip = studentData.MailingAddressZip, StudentPortalId = studentData.StudentPortalId, AlertDescription = studentData.AlertDescription, CriticalAlert = studentData.CriticalAlert, Dentist = studentData.Dentist, DentistPhone = studentData.DentistPhone, InsuranceCompany = studentData.InsuranceCompany, InsuranceCompanyPhone = studentData.InsuranceCompanyPhone, MedicalFacility = studentData.MedicalFacility, MedicalFacilityPhone = studentData.MedicalFacilityPhone, PolicyHolder = studentData.PolicyHolder, PolicyNumber = studentData.PolicyNumber, PrimaryCarePhysician = studentData.PrimaryCarePhysician, PrimaryCarePhysicianPhone = studentData.PrimaryCarePhysicianPhone, Vision = studentData.Vision, VisionPhone = studentData.VisionPhone, Associationship = studentData.Associationship, EconomicDisadvantage = studentData.EconomicDisadvantage, Eligibility504 = studentData.Eligibility504, EstimatedGradDate = studentData.EstimatedGradDate, FreeLunchEligibility = studentData.FreeLunchEligibility, LepIndicator = studentData.LepIndicator, SectionId = null, SpecialEducationIndicator = studentData.SpecialEducationIndicator, StudentInternalId = StudentInternalId, UpdatedOn = DateTime.UtcNow, UpdatedBy = studentData.UpdatedBy, EnrollmentType = "Internal", IsActive = rolloverViewModel.SchoolRollover.ReenrollmentDate != null && rolloverViewModel.SchoolRollover.ReenrollmentDate.Value.Date < DateTime.UtcNow.Date ? true : false, StudentGuid = studentData.StudentGuid };

                                                                this.context?.StudentMaster.Add(studentMasterAdd);

                                                                if (studentMaster.Associationship != null)
                                                                {
                                                                    string[] StudentAssociate = studentMaster.Associationship.Split(" | ", StringSplitOptions.RemoveEmptyEntries);

                                                                    foreach (var studentAssociate in StudentAssociate)
                                                                    {
                                                                        string[] student = studentAssociate.Split("#", StringSplitOptions.RemoveEmptyEntries);
                                                                        var schoolId = Convert.ToInt32(student.ElementAt(1));
                                                                        var studentId = Convert.ToInt32(student.ElementAt(2));

                                                                        var sm = this.context?.StudentMaster.FirstOrDefault(x => x.TenantId == studentData.TenantId && x.SchoolId == schoolId && x.StudentId == studentId);
                                                                        if (sm != null && sm.Associationship != null)
                                                                        {
                                                                            var oldAssociate = rolloverViewModel.SchoolRollover.TenantId + "#" + studentEnrollmentData.SchoolId + "#" + studentEnrollmentData.StudentId;
                                                                            var newAssociate = studentMaster.TenantId + "#" + studentMaster.SchoolId + "#" + studentMaster.StudentId;
                                                                            var associate = sm.Associationship.Replace(oldAssociate, newAssociate);
                                                                            sm.Associationship = associate;
                                                                        }
                                                                    }
                                                                }

                                                                if (rolloverViewModel.SchoolRollover.ReenrollmentDate != null
                                                         && rolloverViewModel.SchoolRollover.ReenrollmentDate.Value.Date <= DateTime.UtcNow.Date)
                                                                {
                                                                    //Student Protal Access
                                                                    if (studentData.StudentPortalId != null)
                                                                    {
                                                                        var userMasterData = this.context?.UserMaster.FirstOrDefault(x => x.EmailAddress == studentData.StudentPortalId && x.TenantId == studentData.TenantId);
                                                                        if (userMasterData != null)
                                                                        {
                                                                            this.context?.UserMaster.Remove(userMasterData);

                                                                            UserMaster userMaster = new UserMaster();
                                                                            userMaster.TenantId = studentData.TenantId;
                                                                            userMaster.SchoolId = (int)studentEnrollmentData.EnrollOtherSchoolId;
                                                                            userMaster.UserId = (int)MasterStudentId;
                                                                            userMaster.Name = userMasterData.Name;
                                                                            userMaster.EmailAddress = userMasterData.EmailAddress;
                                                                            userMaster.PasswordHash = userMasterData.PasswordHash;
                                                                            userMaster.LangId = userMasterData.LangId;
                                                                            var membershipsId = this.context?.Membership.Where(x => x.SchoolId == (int)studentEnrollmentData.EnrollOtherSchoolId && x.TenantId == studentEnrollmentData.TenantId && x.Profile == "Student").Select(x => x.MembershipId).FirstOrDefault();
                                                                            userMaster.MembershipId = (int)membershipsId!;
                                                                            userMaster.UpdatedOn = DateTime.UtcNow;
                                                                            userMaster.UpdatedBy = rolloverViewModel.SchoolRollover.UpdatedBy;
                                                                            userMaster.IsActive = true;
                                                                            this.context?.UserMaster.Add(userMaster);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        //fetch default calender for enroll(transfer) school and save details in StudentEnrollment table.
                                                        int? calId = null;

                                                        var defaultCalender = this.context?.SchoolCalendars.FirstOrDefault(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == studentEnrollmentData.EnrollOtherSchoolId && x.StartDate <= rolloverViewModel.SchoolRollover.ReenrollmentDate && x.EndDate >= rolloverViewModel.SchoolRollover.ReenrollmentDate && x.SessionCalendar == true);

                                                        if (defaultCalender != null)
                                                        {
                                                            calId = defaultCalender.CalenderId;
                                                        }

                                                        var previousGradeId = GradelevelsData?.FirstOrDefault(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.GradeId == studentEnrollmentData.GradeId);

                                                        var nextGradeId = previousGradeId?.EquivalencyId + 1;
                                                        int? transferredGradeId = null;
                                                        string? transferredGradeTitle = null;
                                                        var transferredGrade = this.context?.Gradelevels.FirstOrDefault(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == (int)studentEnrollmentData.EnrollOtherSchoolId! && x.EquivalencyId == nextGradeId);

                                                        if (transferredGrade != null)
                                                        {
                                                            transferredGradeId = transferredGrade.GradeId;
                                                            transferredGradeTitle = transferredGrade.Title;
                                                        }
                                                        else
                                                        {
                                                            var transferredGrad = this.context?.Gradelevels.FirstOrDefault(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == (int)studentEnrollmentData.EnrollOtherSchoolId!);
                                                            transferredGradeId = transferredGrad?.GradeId;
                                                            transferredGradeTitle = transferredGrad?.Title;
                                                        }

                                                        var transferredSchoolName = this.context?.SchoolMaster.FirstOrDefault(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == (int)studentEnrollmentData.EnrollOtherSchoolId!)?.SchoolName;

                                                        StudentEnrollment studentEnrollment = new();
                                                        studentEnrollment.SchoolId = (int)studentEnrollmentData.EnrollOtherSchoolId!;
                                                        studentEnrollment.TenantId = rolloverViewModel.SchoolRollover.TenantId;
                                                        studentEnrollment.StudentId = MasterStudentId;
                                                        studentEnrollment.StudentGuid = studentMaster.StudentGuid;
                                                        studentEnrollment.EnrollmentId = (int)EnrollmentId;
                                                        studentEnrollment.EnrollmentDate = rolloverViewModel.SchoolRollover.ReenrollmentDate;
                                                        studentEnrollment.EnrollmentCode = studentRollOverCode?.Title;
                                                        studentEnrollment.SchoolName = transferredSchoolName;
                                                        studentEnrollment.GradeLevelTitle = transferredGradeTitle;
                                                        studentEnrollment.GradeId = transferredGradeId;
                                                        studentEnrollment.CalenderId = calId;
                                                        studentEnrollment.RollingOption = "Next grade at current school";
                                                        studentEnrollment.UpdatedOn = DateTime.UtcNow;
                                                        studentEnrollment.UpdatedBy = rolloverViewModel.SchoolRollover.CreatedBy;
                                                        studentEnrollment.IsActive = true;
                                                        studentEnrollment.RolloverId = rolloverId;

                                                        this.context?.StudentEnrollment.Add(studentEnrollment);
                                                        EnrollmentId++;

                                                        //this block for transfer associated parent
                                                        var parentAssociationshipData = this.context?.ParentAssociationship.Where(x => x.TenantId == rolloverViewModel.SchoolRollover.TenantId && x.SchoolId == rolloverViewModel.SchoolRollover.SchoolId && x.StudentId == studentEnrollmentData.StudentId).ToList();
                                                        if (parentAssociationshipData?.Any() == true)
                                                        {
                                                            foreach (var associationship in parentAssociationshipData)
                                                            {
                                                                //parentAssociationshipOldList.Add(associationship);

                                                                var associationshipNew = new ParentAssociationship
                                                                {
                                                                    TenantId = rolloverViewModel.SchoolRollover.TenantId,
                                                                    SchoolId = (int)studentEnrollmentData.EnrollOtherSchoolId,
                                                                    StudentId = MasterStudentId,
                                                                    ParentId = associationship.ParentId,
                                                                    Relationship = associationship.Relationship,
                                                                    Associationship = true,
                                                                    UpdatedBy = rolloverViewModel.SchoolRollover.UpdatedBy,
                                                                    UpdatedOn = DateTime.UtcNow,
                                                                    IsCustodian = associationship.IsCustodian,
                                                                    ContactType = associationship.ContactType,
                                                                    CreatedBy = associationship.CreatedBy,
                                                                    CreatedOn = associationship.CreatedOn
                                                                };
                                                                this.context?.ParentAssociationship.Add(associationshipNew);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            this.context?.SaveChanges();
                            rolloverViewModel._failure = false;
                            rolloverViewModel._message = "School rolled over successfully. Please login again";
                            transaction?.Commit();
                        }
                    }
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    rolloverViewModel._message = es.Message;
                    rolloverViewModel._failure = true;
                }
            }
            return rolloverViewModel;
        }
    }
}

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
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.CourseManager;
using opensis.data.ViewModels.StaffSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace opensis.data.Repository
{
    public class StaffScheduleRepository : IStaffScheduleRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public StaffScheduleRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Teacher Schedule View For Course Section
        /// </summary>
        /// <param name="staffScheduleViewModel"></param>
        /// <returns></returns>
        public StaffScheduleViewModel StaffScheduleViewForCourseSection(StaffScheduleViewModel staffScheduleViewModel)
        {
            StaffScheduleViewModel staffScheduleList = new StaffScheduleViewModel();
            try
            {
                if (staffScheduleViewModel.staffScheduleViewList.Count() > 0)
                {

                    foreach (StaffScheduleViewList teacherScheduleView in staffScheduleViewModel.staffScheduleViewList.ToList())
                    {
                        StaffScheduleViewList teacherSchedules = new StaffScheduleViewList();

                        var staffData = this.context?.StaffMaster.FirstOrDefault(x => x.TenantId == staffScheduleViewModel.TenantId /*&& x.SchoolId == staffScheduleViewModel.SchoolId*/ && x.StaffId == teacherScheduleView.StaffId);

                        if (staffData != null)
                        {
                            var courseSectionList = staffScheduleViewModel.courseSectionViewList.ToList();

                            foreach (var getCourseSection in courseSectionList)
                            {
                                string? concatDay = null;

                                var courseSectionData = this.context?.AllCourseSectionView.Where(x => x.TenantId == staffScheduleViewModel.TenantId && x.SchoolId == staffScheduleViewModel.SchoolId && x.CourseSectionId == getCourseSection.CourseSectionId);

                                if (courseSectionData != null)
                                {
                                    CourseSectionViewList CourseSections = new CourseSectionViewList();

                                    var calender = this.context?.SchoolCalendars.FirstOrDefault(x => x.TenantId == staffScheduleViewModel.TenantId && x.SchoolId == staffScheduleViewModel.SchoolId && x.CalenderId == courseSectionData.FirstOrDefault()!.CalendarId);

                                    if (calender != null)
                                    {
                                        CourseSections.WeekDays = calender.Days;
                                    }

                                    var staffSchedule = this.context?.StaffCoursesectionSchedule.Include(x => x.StaffMaster).Where(x => x.TenantId == staffScheduleViewModel.TenantId && x.SchoolId == staffScheduleViewModel.SchoolId && x.CourseSectionId == getCourseSection.CourseSectionId && x.IsDropped != true).ToList();

                                    if (staffSchedule != null && staffSchedule.Any())
                                    {
                                        foreach (var staff in staffSchedule)
                                        {
                                            var staffName = staff.StaffMaster.FirstGivenName + " " + staff.StaffMaster.MiddleName + " " + staff.StaffMaster.LastFamilyName;
                                            CourseSections.ScheduledStaff = CourseSections.ScheduledStaff != null ? CourseSections.ScheduledStaff + "|" + staffName : staffName;
                                        }
                                    }

                                    var variableScheduleData = courseSectionData.Where(x => x.TenantId == staffScheduleViewModel.TenantId && x.SchoolId == staffScheduleViewModel.SchoolId && x.CourseSectionId == getCourseSection.CourseSectionId && x.ScheduleType == "Variable Schedule (2)").ToList();

                                    if (variableScheduleData != null && variableScheduleData.Any())
                                    {
                                        CourseSections.ScheduleType = "Variable Schedule";
                                        foreach (var variableSchedule in variableScheduleData)
                                        {
                                            concatDay = concatDay != null ? concatDay + "|" + variableSchedule.VarDay : variableSchedule.VarDay;

                                            var variableList = this.context?.CourseVariableSchedule.Include(x => x.BlockPeriod).Include(x => x.Rooms).Where(x => x.TenantId == variableSchedule.TenantId && x.SchoolId == variableSchedule.SchoolId && x.CourseSectionId == variableSchedule.CourseSectionId).Select(s => new CourseVariableSchedule { TenantId = s.TenantId, SchoolId = s.SchoolId, CourseId = s.CourseId, CourseSectionId = s.CourseSectionId, GradeScaleId = s.GradeScaleId, Serial = s.Serial, Day = s.Day, RoomId = s.RoomId, TakeAttendance = s.TakeAttendance, PeriodId = s.PeriodId, BlockId = s.BlockId, CreatedBy = s.CreatedBy, CreatedOn = s.CreatedOn, UpdatedBy = s.UpdatedBy, UpdatedOn = s.UpdatedOn, Rooms = new Rooms { Title = s.Rooms!.Title }, BlockPeriod = new BlockPeriod { PeriodTitle = s.BlockPeriod!.PeriodTitle } }).ToList();
                                            //CourseSections.courseVariableSchedule = variableList;

                                            if (variableList != null)
                                            {
                                                CourseSections.courseVariableSchedule = variableList;
                                            }
                                        }
                                    }

                                    var fixedScheduleData = courseSectionData.Where(x => x.TenantId == staffScheduleViewModel.TenantId && x.SchoolId == staffScheduleViewModel.SchoolId && x.CourseSectionId == getCourseSection.CourseSectionId && x.ScheduleType == "Fixed Schedule (1)").FirstOrDefault();

                                    if (fixedScheduleData != null)
                                    {
                                        CourseSections.TakeAttendanceForFixedSchedule = fixedScheduleData.AttendanceTaken;
                                        CourseSections.ScheduleType = "Fixed Schedule";
                                        concatDay = fixedScheduleData.FixedDays;

                                        var fixedSchedule = this.context?.CourseFixedSchedule.Include(f => f.Rooms).Include(f => f.BlockPeriod).Where(x => x.TenantId == staffScheduleViewModel.TenantId && x.SchoolId == staffScheduleViewModel.SchoolId && x.CourseId == fixedScheduleData.CourseId && x.CourseSectionId == fixedScheduleData.CourseSectionId).Select(s => new CourseFixedSchedule { TenantId = s.TenantId, SchoolId = s.SchoolId, CourseId = s.CourseId, CourseSectionId = s.CourseSectionId, GradeScaleId = s.GradeScaleId, Serial = s.Serial, RoomId = s.RoomId, PeriodId = s.PeriodId, BlockId = s.BlockId, CreatedBy = s.CreatedBy, CreatedOn = s.CreatedOn, UpdatedBy = s.UpdatedBy, UpdatedOn = s.UpdatedOn, Rooms = new Rooms { Title = s.Rooms!.Title }, BlockPeriod = new BlockPeriod { PeriodTitle = s.BlockPeriod!.PeriodTitle } }).FirstOrDefault();

                                        CourseSections.courseFixedSchedule = fixedSchedule;

                                    }

                                    var calendarScheduleData = courseSectionData.Where(x => x.TenantId == staffScheduleViewModel.TenantId && x.SchoolId == staffScheduleViewModel.SchoolId && x.CourseSectionId == getCourseSection.CourseSectionId && x.ScheduleType == "Calendar Schedule (3)").ToList();

                                    if (calendarScheduleData.Count > 0)
                                    {
                                        CourseSections.ScheduleType = "Calendar Schedule";
                                        concatDay = "Calendar Days";

                                        foreach (var calendarSchedule in calendarScheduleData)
                                        {
                                            var calendarList = this.context?.CourseCalendarSchedule.Include(x => x.BlockPeriod).Include(x => x.Rooms).Where(x => x.TenantId == calendarSchedule.TenantId && x.SchoolId == calendarSchedule.SchoolId && x.CourseSectionId == calendarSchedule.CourseSectionId).Select(s => new CourseCalendarSchedule { TenantId = s.TenantId, SchoolId = s.SchoolId, CourseId = s.CourseId, CourseSectionId = s.CourseSectionId, GradeScaleId = s.GradeScaleId, Serial = s.Serial, Date = s.Date, RoomId = s.RoomId, TakeAttendance = s.TakeAttendance, PeriodId = s.PeriodId, BlockId = s.BlockId, CreatedBy = s.CreatedBy, CreatedOn = s.CreatedOn, UpdatedBy = s.UpdatedBy, UpdatedOn = s.UpdatedOn, Rooms = new Rooms { Title = s.Rooms!.Title }, BlockPeriod = new BlockPeriod { PeriodTitle = s.BlockPeriod!.PeriodTitle } }).ToList();

                                            //CourseSections.courseCalendarSchedule = calendarList;
                                            if (calendarList != null)
                                            {
                                                CourseSections.courseCalendarSchedule = calendarList;
                                            }
                                        }
                                    }

                                    var blockScheduleData = courseSectionData.Where(x => x.TenantId == staffScheduleViewModel.TenantId && x.SchoolId == staffScheduleViewModel.SchoolId && x.CourseSectionId == getCourseSection.CourseSectionId && x.ScheduleType == "Block Schedule (4)").ToList();

                                    if (blockScheduleData.Count > 0)
                                    {
                                        CourseSections.ScheduleType = "Block Schedule";
                                        concatDay = "Block Days";
                                        foreach (var blockSchedule in blockScheduleData)
                                        {

                                            var blockScheduleList = this.context?.CourseBlockSchedule.Include(x => x.Block).Include(x => x.BlockPeriod).Include(x => x.Rooms).Where(x => x.TenantId == blockSchedule.TenantId && x.SchoolId == blockSchedule.SchoolId && x.CourseSectionId == blockSchedule.CourseSectionId).Select(s => new CourseBlockSchedule { TenantId = s.TenantId, SchoolId = s.SchoolId, CourseId = s.CourseId, CourseSectionId = s.CourseSectionId, GradeScaleId = s.GradeScaleId, Serial = s.Serial, RoomId = s.RoomId, PeriodId = s.PeriodId, BlockId = s.BlockId, TakeAttendance = s.TakeAttendance, CreatedBy = s.CreatedBy, CreatedOn = s.CreatedOn, UpdatedBy = s.UpdatedBy, UpdatedOn = s.UpdatedOn, Rooms = new Rooms { Title = s.Rooms!.Title }, BlockPeriod = new BlockPeriod { PeriodTitle = s.BlockPeriod!.PeriodTitle }, Block = new Block { BlockTitle = s.Block!.BlockTitle } }).ToList();
                                            //CourseSections.courseBlockSchedule = blockScheduleList;
                                            if (blockScheduleList != null)
                                            {
                                                CourseSections.courseBlockSchedule = blockScheduleList;
                                            }
                                        }
                                    }

                                    var courseSection = courseSectionData.FirstOrDefault();
                                    if (courseSection != null)
                                    {
                                        CourseSections.CourseId = courseSection.CourseId;
                                        CourseSections.CourseSectionId = courseSection.CourseSectionId;
                                        CourseSections.CourseTitle = courseSection.CourseTitle;
                                        CourseSections.CourseSectionName = courseSection.CourseSectionName;
                                        CourseSections.DurationStartDate = courseSection.DurationStartDate;
                                        CourseSections.DurationEndDate = courseSection.DurationEndDate;
                                        CourseSections.YrMarkingPeriodId = courseSection.YrMarkingPeriodId;
                                        CourseSections.SmstrMarkingPeriodId = courseSection.SmstrMarkingPeriodId;
                                        CourseSections.QtrMarkingPeriodId = courseSection.QtrMarkingPeriodId;
                                        CourseSections.PrgrsprdMarkingPeriodId = courseSection.PrgrsprdMarkingPeriodId;
                                        CourseSections.MeetingDays = concatDay;
                                        CourseSections.AttendanceTaken = courseSection.AttendanceTaken;
                                        teacherSchedules.courseSectionViewList.Add(CourseSections);
                                    }
                                }
                            }

                            teacherSchedules.StaffId = staffData.StaffId;
                            teacherSchedules.StaffInternalId = staffData.StaffInternalId;
                            teacherSchedules.StaffFullName = staffData.FirstGivenName + " " + staffData.MiddleName + " " + staffData.LastFamilyName;
                            teacherSchedules.StaffEmail = staffData.LoginEmailAddress != null ? staffData.LoginEmailAddress : staffData.PersonalEmail;
                            teacherSchedules.HomeroomTeacher = staffData.HomeroomTeacher;
                            teacherSchedules.StaffGuid = staffData.StaffGuid;

                            staffScheduleList.staffScheduleViewList.Add(teacherSchedules);
                        }
                    }
                    staffScheduleList.TenantId = staffScheduleViewModel.TenantId;
                    staffScheduleList._tenantName = staffScheduleViewModel._tenantName;
                    staffScheduleList.SchoolId = staffScheduleViewModel.SchoolId;
                    staffScheduleList._token = staffScheduleViewModel._token;
                    staffScheduleList._failure = false;
                }
            }
            catch (Exception es)
            {
                staffScheduleList.staffScheduleViewList = null!;
                staffScheduleList._failure = true;
                staffScheduleList._message = es.Message;
            }
            return staffScheduleList;
        }

        /// <summary>
        ///  Add Staff Course Section Schedule
        /// </summary>
        /// <param name="staffScheduleViewModel"></param>
        /// <returns></returns>
        public StaffScheduleViewModel AddStaffCourseSectionSchedule(StaffScheduleViewModel staffScheduleViewModel)
        {
            try
            {
                if (staffScheduleViewModel.staffScheduleViewList.Count() > 0)
                {
                    foreach (var staffSchedule in staffScheduleViewModel.staffScheduleViewList.ToList())
                    {
                        var courseSectionList = staffSchedule.courseSectionViewList.ToList();

                        if (courseSectionList.Count > 0)
                        {
                            foreach (var CourseSection in courseSectionList)
                            {
                                var StaffScheduleDataAlreadyExist = this.context?.StaffCoursesectionSchedule.FirstOrDefault(x => x.TenantId == staffScheduleViewModel.TenantId && x.SchoolId == staffScheduleViewModel.SchoolId && x.StaffId == staffSchedule.StaffId && x.CourseId == CourseSection.CourseId && x.CourseSectionId == CourseSection.CourseSectionId);

                                if (StaffScheduleDataAlreadyExist != null)
                                {
                                    StaffScheduleDataAlreadyExist.IsDropped = null;
                                    StaffScheduleDataAlreadyExist.EffectiveDropDate = null;
                                }
                                else
                                {
                                    var primaryStaffinCourseSection = this.context?.StaffCoursesectionSchedule.FirstOrDefault(x => x.TenantId == staffScheduleViewModel.TenantId && x.SchoolId == staffScheduleViewModel.SchoolId && x.CourseId == CourseSection.CourseId && x.CourseSectionId == CourseSection.CourseSectionId && x.IsPrimaryStaff == true);

                                    var staffCoursesectionSchedule = new StaffCoursesectionSchedule()
                                    {
                                        TenantId = staffScheduleViewModel.TenantId,
                                        SchoolId = staffScheduleViewModel.SchoolId,
                                        StaffId = (int)staffSchedule.StaffId!,
                                        CourseId = (int)CourseSection.CourseId!,
                                        CourseSectionId = (int)CourseSection.CourseSectionId!,
                                        StaffGuid = (Guid)staffSchedule.StaffGuid!,
                                        CourseSectionName = CourseSection.CourseSectionName,
                                        YrMarkingPeriodId = CourseSection.YrMarkingPeriodId,
                                        SmstrMarkingPeriodId = CourseSection.SmstrMarkingPeriodId,
                                        QtrMarkingPeriodId = CourseSection.QtrMarkingPeriodId,
                                        PrgrsprdMarkingPeriodId = CourseSection.PrgrsprdMarkingPeriodId,
                                        DurationStartDate = CourseSection.DurationStartDate,
                                        DurationEndDate = CourseSection.DurationEndDate,
                                        MeetingDays = CourseSection.MeetingDays,
                                        CreatedBy = staffScheduleViewModel.CreatedBy,
                                        CreatedOn = DateTime.UtcNow,
                                        IsAssigned = true,
                                        AcademicYear = this.context?.CourseSection.FirstOrDefault(x => x.TenantId == staffScheduleViewModel.TenantId && x.SchoolId == staffScheduleViewModel.SchoolId && x.CourseSectionId == CourseSection.CourseSectionId)?.AcademicYear,
                                        IsPrimaryStaff = primaryStaffinCourseSection == null ? true : false
                                    };
                                    this.context?.StaffCoursesectionSchedule.Add(staffCoursesectionSchedule);
                                }
                            }
                            this.context?.SaveChanges();
                            staffScheduleViewModel._message = "Teacher scheduled successfully";
                            staffScheduleViewModel._failure = false;
                        }
                        else
                        {
                            staffScheduleViewModel._failure = true;
                            staffScheduleViewModel._message = "Select CourseSection For Teacher scheduled ";
                        }
                    }
                }
                else
                {
                    staffScheduleViewModel._failure = true;
                    staffScheduleViewModel._message = "Select Staff For Teacher scheduled ";
                }
            }
            catch (Exception es)
            {
                staffScheduleViewModel._failure = true;
                staffScheduleViewModel._message = es.Message;
            }
            return staffScheduleViewModel;
        }

        /// <summary>
        /// Check Availability Staff Course Section Schedule
        /// </summary>
        /// <param name="staffScheduleViewModel"></param>
        /// <returns></returns>
        public StaffScheduleViewModel CheckAvailabilityStaffCourseSectionSchedule(StaffScheduleViewModel staffScheduleViewModel)
        {
            try
            {
                if (staffScheduleViewModel.staffScheduleViewList?.Any() == true)
                {
                    staffScheduleViewModel._message = "No Conflict Detected";
                    List<int> conflictInInputCourseSection = new List<int>();

                    var courseSectionListCheck = staffScheduleViewModel.staffScheduleViewList.FirstOrDefault()!.courseSectionViewList.ToList();

                    if (courseSectionListCheck?.Any() == true)
                    {
                        //This Block For Checking Period,Day Conflict Between Input Course Sections
                        foreach (var courseSectionCheck in courseSectionListCheck)
                        {
                            var courseSectionCheckData = this.context?.AllCourseSectionView.Where(c => c.SchoolId == staffScheduleViewModel.SchoolId && c.TenantId == staffScheduleViewModel.TenantId && c.CourseSectionId == courseSectionCheck.CourseSectionId).ToList();
                            if (courseSectionCheckData?.Any()==true)
                            {
                                var courseSectionIds = courseSectionListCheck.Where(x => x.CourseSectionId != courseSectionCheck.CourseSectionId).Select(s => s.CourseSectionId);

                                if (courseSectionIds.Count() > 0)
                                {
                                    if (courseSectionCheckData.FirstOrDefault()!.AllowTeacherConflict != true)
                                    {
                                        var courseSectionInputData = this.context?.AllCourseSectionView.Where(c => c.SchoolId == staffScheduleViewModel.SchoolId && c.TenantId == staffScheduleViewModel.TenantId && courseSectionIds.Contains(c.CourseSectionId)).ToList();

                                        foreach (var courseSec in courseSectionCheckData)
                                        {
                                            //   courseSectionInputData = courseSectionInputData?.Where(x => x.TenantId == courseSec.TenantId && x.SchoolId == courseSec.SchoolId &&
                                            //     ((courseSec.FixedPeriodId != null && ((courseSec.FixedPeriodId == x.FixedPeriodId && (Regex.IsMatch(x.FixedDays.ToLower(), courseSec.FixedDays.ToLower(), RegexOptions.IgnoreCase))) || (courseSec.FixedPeriodId == x.VarPeriodId && courseSec.FixedDays.ToLower().Contains(x.VarDay.ToLower())) || (courseSec.FixedPeriodId == x.CalPeriodId && courseSec.FixedDays.ToLower().Contains(x.CalDay.ToLower())))) ||

                                            //(courseSec.VarPeriodId != null && ((courseSec.VarPeriodId == x.FixedPeriodId && x.FixedDays.ToLower().Contains(courseSec.VarDay.ToLower())) || (courseSec.VarPeriodId == x.VarPeriodId && courseSec.VarDay.ToLower() == x.VarDay.ToLower()) || (courseSec.VarPeriodId == x.CalPeriodId && courseSec.VarDay.ToLower() == x.CalDay.ToLower()))) ||

                                            //(courseSec.CalPeriodId != null && ((courseSec.CalPeriodId == x.FixedPeriodId && x.FixedDays.ToLower().Contains(courseSec.CalDay.ToLower())) || (courseSec.CalPeriodId == x.VarPeriodId && courseSec.CalDay.ToLower() == x.VarDay.ToLower()) || (courseSec.CalPeriodId == x.CalPeriodId && courseSec.CalDay.ToLower() == x.CalDay.ToLower()))) ||

                                            //(courseSec.BlockPeriodId != null && (courseSec.BlockPeriodId == x.BlockPeriodId && courseSec.BlockRoomId == x.BlockRoomId && courseSec.BlockId == x.BlockId))) && courseSec.DurationEndDate > x.DurationStartDate).ToList();

                                            courseSectionInputData = courseSectionInputData?.Where(x => x.TenantId == courseSec.TenantId && x.SchoolId == courseSec.SchoolId &&
                                                ((courseSec.FixedPeriodId != null && ((courseSec.FixedPeriodId == x.FixedPeriodId && (Regex.IsMatch((x.FixedDays ?? "").ToLower(), (courseSec.FixedDays ?? "").ToLower(), RegexOptions.IgnoreCase )))|| (courseSec.FixedPeriodId == x.VarPeriodId && (courseSec.FixedDays??"").ToLower().Contains((x.VarDay??"").ToLower())) || (courseSec.FixedPeriodId == x.CalPeriodId && (courseSec.FixedDays??"").ToLower().Contains((x.CalDay??"").ToLower())))) ||

                                           (courseSec.VarPeriodId != null && ((courseSec.VarPeriodId == x.FixedPeriodId && (x.FixedDays??"").ToLower().Contains((courseSec.VarDay??"").ToLower())) || (courseSec.VarPeriodId == x.VarPeriodId && (courseSec.VarDay??"").ToLower() == (x.VarDay??"").ToLower()) || (courseSec.VarPeriodId == x.CalPeriodId && (courseSec.VarDay??"").ToLower() == (x.CalDay??"").ToLower()))) ||

                                           (courseSec.CalPeriodId != null && ((courseSec.CalPeriodId == x.FixedPeriodId && (x.FixedDays??"").ToLower().Contains((courseSec.CalDay??"").ToLower())) || (courseSec.CalPeriodId == x.VarPeriodId && (courseSec.CalDay??"").ToLower() == (x.VarDay??"").ToLower()) || (courseSec.CalPeriodId == x.CalPeriodId && (courseSec.CalDay??"").ToLower() == (x.CalDay??"").ToLower()))) ||

                                           (courseSec.BlockPeriodId != null && (courseSec.BlockPeriodId == x.BlockPeriodId && courseSec.BlockRoomId == x.BlockRoomId && courseSec.BlockId == x.BlockId))) && courseSec.DurationEndDate > x.DurationStartDate && x.AllowTeacherConflict != true).ToList();

                                            if (courseSectionInputData?.Any() == true)
                                            {
                                                var ids = courseSectionInputData.Select(s => s.CourseSectionId).Distinct().ToList();
                                                conflictInInputCourseSection.Add(ids.First());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    } //end of input course section checking

                    foreach (var staff in staffScheduleViewModel.staffScheduleViewList.ToList())
                    {
                        var courseSectionList = staff.courseSectionViewList.ToList();

                        if (courseSectionList.Count() > 0)
                        {
                            foreach (var courseSection in courseSectionList)
                            {
                                var check = conflictInInputCourseSection.Contains((int)courseSection.CourseSectionId!);
                                if (check == true) //checking this courseSection is in conflict Input CourseSection or not
                                {
                                    staffScheduleViewModel._failure = true;
                                    //staffScheduleViewModel._message = "Staff already exits in course section";
                                    staffScheduleViewModel._message = "Conflict Detected";
                                    staff.ConflictStaff = true;
                                    courseSection.ConflictCourseSection = true;
                                }
                                else
                                {
                                    var checkStaffInCourseSection = this.context?.StaffCoursesectionSchedule.Where(x => x.TenantId == staffScheduleViewModel.TenantId && x.SchoolId == staffScheduleViewModel.SchoolId && x.StaffId == staff.StaffId && x.CourseId == courseSection.CourseId && x.CourseSectionId == courseSection.CourseSectionId && x.IsAssigned == true && x.IsDropped != true).ToList();

                                    if (checkStaffInCourseSection?.Any() == true)
                                    {
                                        staffScheduleViewModel._failure = true;
                                        //staffScheduleViewModel._message = "Staff already exits in course section";
                                        staffScheduleViewModel._message = "Conflict Detected";
                                        staff.ConflictStaff = true;
                                        courseSection.ConflictCourseSection = true;

                                    }
                                    else
                                    {
                                        var courseSectionAllData = this.context?.AllCourseSectionView.Where(c => c.SchoolId == staffScheduleViewModel.SchoolId && c.TenantId == staffScheduleViewModel.TenantId && c.CourseSectionId == courseSection.CourseSectionId).ToList();

                                        if (courseSectionAllData?.FirstOrDefault()!.AllowTeacherConflict != true)
                                        {
                                            if (courseSectionAllData?.Any() == true)
                                            {
                                                foreach (var courseSectionData in courseSectionAllData)
                                                {
                                                    var checkForConflict = this.context?.AllCourseSectionView.Join(this.context.StaffCoursesectionSchedule, acsv => acsv.CourseSectionId, scss => scss.CourseSectionId, (acsv, scss) => new { acsv, scss }).AsEnumerable().Where(c => c.acsv.TenantId == staffScheduleViewModel.TenantId && c.acsv.SchoolId == staffScheduleViewModel.SchoolId && c.scss.TenantId == staffScheduleViewModel.TenantId && c.scss.SchoolId == staffScheduleViewModel.SchoolId && c.scss.StaffId == staff.StaffId &&

                                    ((c.acsv.FixedPeriodId != null && ((c.acsv.FixedPeriodId == courseSectionData.FixedPeriodId &&( Regex.IsMatch((c.acsv.FixedDays ?? "").ToLower(), (courseSectionData.FixedDays ?? "").ToLower(), RegexOptions.IgnoreCase))) || (c.acsv.FixedPeriodId == courseSectionData.VarPeriodId && (c.acsv.FixedDays??"").ToLower().Contains((courseSectionData.VarDay??"").ToLower())) || (c.acsv.FixedPeriodId == courseSectionData.CalPeriodId && (c.acsv.FixedDays??"").ToLower().Contains((courseSectionData.CalDay??"").ToLower())))) ||

                                    (c.acsv.VarPeriodId != null && ((c.acsv.VarPeriodId == courseSectionData.FixedPeriodId && (courseSectionData.FixedDays??"").ToLower().Contains((c.acsv.VarDay??"").ToLower())) || (c.acsv.VarPeriodId == courseSectionData.VarPeriodId && (c.acsv.VarDay??"").ToLower() == (courseSectionData.VarDay??"").ToLower()) || (c.acsv.VarPeriodId == courseSectionData.CalPeriodId && (c.acsv.VarDay??"").ToLower() == (courseSectionData.CalDay??"").ToLower()))) ||

                                    (c.acsv.CalPeriodId != null && ((c.acsv.CalPeriodId == courseSectionData.FixedPeriodId && (courseSectionData.FixedDays??"").ToLower().Contains((c.acsv.CalDay??"").ToLower())) || (c.acsv.CalPeriodId == courseSectionData.VarPeriodId && (c.acsv.CalDay??"").ToLower() == (courseSectionData.VarDay??"").ToLower()) || (c.acsv.CalPeriodId == courseSectionData.CalPeriodId && (c.acsv.CalDay??"").ToLower() == (courseSectionData.CalDay??"").ToLower()))) ||

                                    (c.acsv.BlockPeriodId != null && (c.acsv.BlockPeriodId == courseSectionData.BlockPeriodId &&c.acsv.BlockRoomId==courseSectionData.BlockRoomId&& c.acsv.BlockId==courseSectionData.BlockId))) && c.acsv.DurationEndDate > courseSectionData.DurationStartDate && c.scss.IsDropped != true && c.acsv.AllowTeacherConflict != true).ToList();
                                    //                var checkForConflict = this.context?.AllCourseSectionView.Join(this.context.StaffCoursesectionSchedule, acsv => acsv.CourseSectionId, scss => scss.CourseSectionId, (acsv, scss) => new { acsv, scss }).AsEnumerable().Where(c => c.acsv.TenantId == staffScheduleViewModel.TenantId && c.acsv.SchoolId == staffScheduleViewModel.SchoolId && c.scss.StaffId == staff.StaffId &&

                                    //((c.acsv.FixedPeriodId != null && ((c.acsv.FixedPeriodId == courseSectionData.FixedPeriodId && (Regex.IsMatch(courseSectionData.FixedDays.ToLower(), c.acsv.FixedDays.ToLower(), RegexOptions.IgnoreCase))) || (c.acsv.FixedPeriodId == courseSectionData.VarPeriodId && c.acsv.FixedDays.ToLower().Contains(courseSectionData.VarDay.ToLower())) || (c.acsv.FixedPeriodId == courseSectionData.CalPeriodId && c.acsv.FixedDays.ToLower().Contains(courseSectionData.CalDay.ToLower())))) ||

                                    //(c.acsv.VarPeriodId != null && ((c.acsv.VarPeriodId == courseSectionData.FixedPeriodId && courseSectionData.FixedDays.ToLower().Contains(c.acsv.VarDay.ToLower())) || (c.acsv.VarPeriodId == courseSectionData.VarPeriodId && c.acsv.VarDay.ToLower() == courseSectionData.VarDay.ToLower()) || (c.acsv.VarPeriodId == courseSectionData.CalPeriodId && c.acsv.VarDay.ToLower() == courseSectionData.CalDay.ToLower()))) ||

                                    //(c.acsv.CalPeriodId != null && ((c.acsv.CalPeriodId == courseSectionData.FixedPeriodId && courseSectionData.FixedDays.ToLower().Contains(c.acsv.CalDay.ToLower())) || (c.acsv.CalPeriodId == courseSectionData.VarPeriodId && c.acsv.CalDay.ToLower() == courseSectionData.VarDay.ToLower()) || (c.acsv.CalPeriodId == courseSectionData.CalPeriodId && c.acsv.CalDay.ToLower() == courseSectionData.CalDay.ToLower()))) ||

                                    //(c.acsv.BlockPeriodId != null && (c.acsv.BlockPeriodId == courseSectionData.BlockPeriodId && c.acsv.BlockRoomId == courseSectionData.BlockRoomId && c.acsv.BlockId == courseSectionData.BlockId))) && c.acsv.DurationEndDate > courseSectionData.DurationStartDate && c.scss.IsDropped != true).ToList();
                                                    if (checkForConflict?.Any()==true)
                                                    {
                                                        staffScheduleViewModel._failure = true;
                                                        //staffScheduleViewModel._message = "Period and Room Conflict";
                                                        staffScheduleViewModel._message = "Conflict Detected";
                                                        staff.ConflictStaff = true;
                                                        courseSection.ConflictCourseSection = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            staffScheduleViewModel._failure = true;
                            staffScheduleViewModel._message = "Select CourseSection For Teacher scheduled";
                            return staffScheduleViewModel;
                        }
                    }
                }
                else
                {
                    staffScheduleViewModel._failure = true;
                    staffScheduleViewModel._message = "Select Staff For Teacher scheduled";
                    return staffScheduleViewModel;
                }
            }
            catch (Exception es)
            {
                staffScheduleViewModel._failure = true;
                staffScheduleViewModel._message = es.Message;
            }
            return staffScheduleViewModel;
        }

        /// <summary>
        /// Get All Scheduled Course Section For Staff
        /// </summary>
        /// <param name="scheduledCourseSectionViewModel"></param>
        /// <returns></returns>
        public ScheduledCourseSectionViewModel GetAllScheduledCourseSectionForStaff(ScheduledCourseSectionViewModel scheduledCourseSectionViewModel)
        {
            ScheduledCourseSectionViewModel scheduledCourseSectionView = new ScheduledCourseSectionViewModel();
            try
            {
                scheduledCourseSectionView.TenantId = scheduledCourseSectionViewModel.TenantId;
                scheduledCourseSectionView._tenantName = scheduledCourseSectionViewModel._tenantName;
                scheduledCourseSectionView.SchoolId = scheduledCourseSectionViewModel.SchoolId;
                scheduledCourseSectionView.StaffId = scheduledCourseSectionViewModel.StaffId;
                scheduledCourseSectionView._token = scheduledCourseSectionViewModel._token;

                var scheduledCourseSectionData = this.context?.StaffCoursesectionSchedule.Include(s => s.StaffMaster).Include(x => x.CourseSection).Include(x => x.CourseSection.Course).Include(x => x.CourseSection.SchoolCalendars).Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.StaffId == scheduledCourseSectionViewModel.StaffId && x.IsDropped != true).ToList();

                if (scheduledCourseSectionData?.Any() == true)
                {
                    if (scheduledCourseSectionViewModel.MarkingPeriodStartDate != null && scheduledCourseSectionViewModel.MarkingPeriodEndDate != null)
                    {    
                        //this code for filter CS by top right corner marking period.
                        scheduledCourseSectionData = scheduledCourseSectionData.Where(x => x.CourseSection.DurationBasedOnPeriod == false || ((scheduledCourseSectionViewModel.MarkingPeriodStartDate >= x.DurationStartDate && scheduledCourseSectionViewModel.MarkingPeriodStartDate <= x.DurationEndDate) && (scheduledCourseSectionViewModel.MarkingPeriodEndDate >= x.DurationStartDate && scheduledCourseSectionViewModel.MarkingPeriodEndDate <= x.DurationEndDate))).ToList();
                    }

                    List<int> csIds = new List<int> { };
                    csIds = scheduledCourseSectionData.Select(x => x.CourseSectionId).ToList();

                    var scheduledStaffDataForCourseSection = this.context?.StaffCoursesectionSchedule.Include(x => x.StaffMaster).Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.StaffId != scheduledCourseSectionViewModel.StaffId && (csIds == null || (csIds.Contains(x.CourseSectionId))) && x.IsDropped != true).ToList();

                    foreach (var scheduledCourseSection in scheduledCourseSectionData)
                    {
                        if (scheduledCourseSection.CourseSection.AcademicYear == scheduledCourseSectionViewModel.AcademicYear)
                        {
                            CourseSectionViewList CourseSections = new CourseSectionViewList();

                            //var scheduledStaffData = this.context?.StaffCoursesectionSchedule.Include(x => x.StaffMaster).Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.StaffId != scheduledCourseSection.StaffId&& x.CourseSectionId== scheduledCourseSection.CourseSectionId && x.IsDropped != true).ToList();

                            var scheduledStaffData = scheduledStaffDataForCourseSection?.Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();


                            if (scheduledStaffData?.Any() == true)
                            {
                                foreach (var scheduledStaff in scheduledStaffData)
                                {
                                    var staffName = scheduledStaff.StaffMaster.FirstGivenName + " " + scheduledStaff.StaffMaster.MiddleName + " " + scheduledStaff.StaffMaster.LastFamilyName;
                                    CourseSections.ScheduledStaff = CourseSections.ScheduledStaff != null ? CourseSections.ScheduledStaff + "|" + staffName : staffName;
                                }
                            }

                            CourseSections.WeekDays = scheduledCourseSection.CourseSection.SchoolCalendars?.Days;
                            CourseSections.CourseTitle = scheduledCourseSection.CourseSection.Course.CourseTitle;

                            if (scheduledCourseSection.CourseSection.ScheduleType == "Fixed Schedule (1)")
                            {
                                CourseSections.ScheduleType = "Fixed Schedule";
                            }
                            if (scheduledCourseSection.CourseSection.ScheduleType == "Variable Schedule (2)")
                            {
                                CourseSections.ScheduleType = "Variable Schedule";
                            }
                            if (scheduledCourseSection.CourseSection.ScheduleType == "Calendar Schedule (3)")
                            {
                                CourseSections.ScheduleType = "Calendar Schedule";
                            }
                            if (scheduledCourseSection.CourseSection.ScheduleType == "Block Schedule (4)")
                            {
                                CourseSections.ScheduleType = "Block Schedule";
                            }
                            CourseSections.CalendarId = scheduledCourseSection.CourseSection.CalendarId;
                            CourseSections.CourseId = scheduledCourseSection.CourseId;
                            CourseSections.CourseSectionId = scheduledCourseSection.CourseSectionId;
                            CourseSections.GradeScaleId = scheduledCourseSection.CourseSection.GradeScaleId;
                            CourseSections.StandardGradeScaleId = scheduledCourseSection.CourseSection.StandardGradeScaleId;
                            CourseSections.CourseSectionName = scheduledCourseSection.CourseSectionName;
                            CourseSections.YrMarkingPeriodId = scheduledCourseSection.YrMarkingPeriodId;
                            CourseSections.SmstrMarkingPeriodId = scheduledCourseSection.SmstrMarkingPeriodId;
                            CourseSections.QtrMarkingPeriodId = scheduledCourseSection.QtrMarkingPeriodId;
                            CourseSections.PrgrsprdMarkingPeriodId = scheduledCourseSection.PrgrsprdMarkingPeriodId;
                            CourseSections.DurationStartDate = scheduledCourseSection.DurationStartDate;
                            CourseSections.DurationEndDate = scheduledCourseSection.DurationEndDate;
                            CourseSections.MeetingDays = scheduledCourseSection.MeetingDays;
                            CourseSections.GradeScaleType = scheduledCourseSection.CourseSection.GradeScaleType;
                            CourseSections.UsedStandard = scheduledCourseSection.CourseSection.UseStandards;
                            CourseSections.IsPrimaryStaff = scheduledCourseSection.IsPrimaryStaff;

                            scheduledCourseSectionView.courseSectionViewList.Add(CourseSections);
                        }
                    }
                }
                else
                {
                    scheduledCourseSectionView._failure = true;
                    scheduledCourseSectionView._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                scheduledCourseSectionView.courseSectionViewList = null!;
                scheduledCourseSectionView._failure = true;
                scheduledCourseSectionView._message = es.Message;
            }
            return scheduledCourseSectionView;
        }

        /// <summary>
        /// Add Staff Course Section Re-Schedule
        /// </summary>
        /// <param name="staffScheduleViewModel"></param>
        /// <returns></returns>
        public StaffScheduleViewModel AddStaffCourseSectionReSchedule(StaffScheduleViewModel staffScheduleViewModel)
        {
            try
            {
                if (staffScheduleViewModel.staffScheduleViewList?.Any() == true)
                {
                    foreach (var staffSchedule in staffScheduleViewModel.staffScheduleViewList.ToList())
                    {
                        var courseSectionList = staffSchedule.courseSectionViewList.ToList();

                        if (courseSectionList.Count > 0)
                        {
                            var staffGuid = this.context?.StaffMaster.Where(x => x.StaffId == staffScheduleViewModel.staffScheduleViewList.FirstOrDefault()!.StaffId && x.SchoolId == staffScheduleViewModel.SchoolId && x.TenantId == staffScheduleViewModel.TenantId).FirstOrDefault()?.StaffGuid;

                            foreach (var courseSection in courseSectionList)
                            {
                                var StaffScheduleData = this.context?.StaffCoursesectionSchedule.FirstOrDefault(x => x.TenantId == staffScheduleViewModel.TenantId && x.SchoolId == staffScheduleViewModel.SchoolId && x.StaffId == staffScheduleViewModel.ExistingStaff && x.CourseSectionId == courseSection.CourseSectionId);

                                if (StaffScheduleData != null)
                                {
                                    StaffScheduleData.IsDropped = true;
                                    StaffScheduleData.EffectiveDropDate = DateTime.UtcNow;
                                    StaffScheduleData.IsPrimaryStaff = false;
                                }
                                else
                                {
                                    staffScheduleViewModel._failure = true;
                                    staffScheduleViewModel._message = "Existing staff does not in this CourseSection";
                                    return staffScheduleViewModel;
                                }
                                var StaffScheduleDataAlreadyExist = this.context?.StaffCoursesectionSchedule.FirstOrDefault(x => x.TenantId == staffScheduleViewModel.TenantId && x.SchoolId == staffScheduleViewModel.SchoolId && x.StaffId == staffSchedule.StaffId && x.CourseId == courseSection.CourseId && x.CourseSectionId == courseSection.CourseSectionId);
                                if (StaffScheduleDataAlreadyExist != null)
                                {
                                    StaffScheduleDataAlreadyExist.IsDropped = null;
                                    StaffScheduleDataAlreadyExist.EffectiveDropDate = null;
                                    StaffScheduleDataAlreadyExist.IsPrimaryStaff = true;
                                }
                                else
                                {
                                    var staffCoursesectionSchedule = new StaffCoursesectionSchedule()
                                    {
                                        TenantId = staffScheduleViewModel.TenantId,
                                        SchoolId = staffScheduleViewModel.SchoolId,
                                        StaffId = (int)staffSchedule.StaffId!,
                                        CourseId = (int)courseSection.CourseId!,
                                        CourseSectionId = (int)courseSection.CourseSectionId!,
                                        StaffGuid = (Guid)staffGuid!,
                                        CourseSectionName = courseSection.CourseSectionName,
                                        YrMarkingPeriodId = courseSection.YrMarkingPeriodId,
                                        SmstrMarkingPeriodId = courseSection.SmstrMarkingPeriodId,
                                        QtrMarkingPeriodId = courseSection.QtrMarkingPeriodId,
                                        PrgrsprdMarkingPeriodId = courseSection.PrgrsprdMarkingPeriodId,
                                        DurationStartDate = courseSection.DurationStartDate,
                                        DurationEndDate = courseSection.DurationEndDate,
                                        MeetingDays = courseSection.MeetingDays,
                                        CreatedBy = staffScheduleViewModel.CreatedBy,
                                        CreatedOn = DateTime.UtcNow,
                                        IsAssigned = true,
                                        AcademicYear = this.context?.CourseSection.FirstOrDefault(x => x.TenantId == staffScheduleViewModel.TenantId && x.SchoolId == staffScheduleViewModel.SchoolId && x.CourseSectionId == courseSection.CourseSectionId)?.AcademicYear,
                                        IsPrimaryStaff = true
                                    };
                                    this.context?.StaffCoursesectionSchedule.Add(staffCoursesectionSchedule);
                                }
                            }
                            this.context?.SaveChanges();
                            staffScheduleViewModel._message = "Staff  Re-Schedule In CourseSection Successfully";
                            staffScheduleViewModel._failure = false;
                        }
                        else
                        {
                            staffScheduleViewModel._failure = true;
                            staffScheduleViewModel._message = "Select CourseSection For Staff  Re-Schedule";
                        }
                    }
                }
                else
                {
                    staffScheduleViewModel._failure = true;
                    staffScheduleViewModel._message = "Select Staff For Staff  Re-Schedule";
                }
            }
            catch (Exception es)
            {
                staffScheduleViewModel._failure = true;
                staffScheduleViewModel._message = es.Message;
            }
            return staffScheduleViewModel;
        }

        /// <summary>
        /// Check Availability Staff Course Section Re-Schedule
        /// </summary>
        /// <param name="staffListViewModel"></param>
        /// <returns></returns>
        public StaffListViewModel checkAvailabilityStaffCourseSectionReSchedule(StaffListViewModel staffListViewModel)
        {
            try
            {
                if (staffListViewModel.CourseSectionsList.Count() > 0)
                {
                    staffListViewModel._message = "No Conflict Detected";

                    foreach (var courseSection in staffListViewModel.CourseSectionsList.ToList())
                    {
                        if(courseSection.StaffCoursesectionSchedule.Count() > 0)
                        {
                            var checkStaffInCourseSection = this.context?.StaffCoursesectionSchedule.Where(x => x.TenantId == staffListViewModel.TenantId && x.SchoolId == staffListViewModel.SchoolId && x.StaffId == staffListViewModel.ReScheduleStaffId
                                                    && x.CourseId == courseSection.CourseId && x.CourseSectionId == courseSection.CourseSectionId && x.IsAssigned == true && x.IsDropped != true).ToList();

                            if (checkStaffInCourseSection?.Any() == true)
                            {
                                staffListViewModel._failure = true;
                                staffListViewModel._message = "Conflict Detected";
                                var indexValue = staffListViewModel.CourseSectionsList.FindIndex(x => x.CourseSectionId == courseSection.CourseSectionId);
                                staffListViewModel.ConflictIndexNo = staffListViewModel.ConflictIndexNo != null ? staffListViewModel.ConflictIndexNo + "," + indexValue.ToString() : indexValue.ToString();

                            }
                            else
                            {
                                var checkAllowStaffConflict = this.context?.CourseSection.FirstOrDefault(x => x.TenantId == staffListViewModel.TenantId && x.SchoolId == staffListViewModel.SchoolId && x.CourseId == courseSection.CourseId && x.CourseSectionId == courseSection.CourseSectionId && x.AllowTeacherConflict == true);


                                if (checkAllowStaffConflict == null)
                                {
                                    var courseSectionAllData = this.context?.AllCourseSectionView.Where(c => c.SchoolId == staffListViewModel.SchoolId && c.TenantId == staffListViewModel.TenantId && c.CourseSectionId == courseSection.CourseSectionId).ToList();

                                    if (courseSectionAllData?.Any() == true)
                                    {
                                        foreach (var courseSectionData in courseSectionAllData)
                                        {
                                            //                var checkForConflict = this.context?.AllCourseSectionView.Join(this.context.StaffCoursesectionSchedule, acsv => acsv.CourseSectionId, scss => scss.CourseSectionId, (acsv, scss) => new { acsv, scss }).AsEnumerable().Where(c => c.acsv.TenantId == staffListViewModel.TenantId && c.acsv.SchoolId == staffListViewModel.SchoolId && c.scss.StaffId == staffListViewModel.ReScheduleStaffId &&

                                            //((c.acsv.FixedPeriodId != null && ((c.acsv.FixedPeriodId == courseSectionData.FixedPeriodId && (Regex.IsMatch(courseSectionData.FixedDays.ToLower(), c.acsv.FixedDays.ToLower(), RegexOptions.IgnoreCase))) || (c.acsv.FixedPeriodId == courseSectionData.VarPeriodId && c.acsv.FixedDays.ToLower().Contains(courseSectionData.VarDay.ToLower())) || (c.acsv.FixedPeriodId == courseSectionData.CalPeriodId && c.acsv.FixedDays.ToLower().Contains(courseSectionData.CalDay.ToLower())))) ||

                                            //(c.acsv.VarPeriodId != null && ((c.acsv.VarPeriodId == courseSectionData.FixedPeriodId && courseSectionData.FixedDays.ToLower().Contains(c.acsv.VarDay.ToLower())) || (c.acsv.VarPeriodId == courseSectionData.VarPeriodId && c.acsv.VarDay.ToLower() == courseSectionData.VarDay.ToLower()) || (c.acsv.VarPeriodId == courseSectionData.CalPeriodId && c.acsv.VarDay.ToLower() == courseSectionData.CalDay.ToLower()))) ||

                                            //(c.acsv.CalPeriodId != null && ((c.acsv.CalPeriodId == courseSectionData.FixedPeriodId && courseSectionData.FixedDays.ToLower().Contains(c.acsv.CalDay.ToLower())) || (c.acsv.CalPeriodId == courseSectionData.VarPeriodId && c.acsv.CalDay.ToLower() == courseSectionData.VarDay.ToLower()) || (c.acsv.CalPeriodId == courseSectionData.CalPeriodId && c.acsv.CalDay.ToLower() == courseSectionData.CalDay.ToLower()))) ||

                                            //(c.acsv.BlockPeriodId != null && (c.acsv.BlockPeriodId == courseSectionData.BlockPeriodId && c.acsv.BlockRoomId == courseSectionData.BlockRoomId && c.acsv.BlockId == courseSectionData.BlockId))) && c.acsv.DurationEndDate > courseSectionData.DurationStartDate && c.scss.IsDropped != true).ToList();

                                            var checkForConflict = this.context?.AllCourseSectionView.Join(this.context.StaffCoursesectionSchedule, acsv => acsv.CourseSectionId, scss => scss.CourseSectionId, (acsv, scss) => new { acsv, scss }).AsEnumerable().Where(c => c.acsv.TenantId == staffListViewModel.TenantId && c.acsv.SchoolId == staffListViewModel.SchoolId && c.scss.TenantId == staffListViewModel.TenantId && c.scss.SchoolId == staffListViewModel.SchoolId && c.scss.StaffId == staffListViewModel.ReScheduleStaffId &&

                                                   ((c.acsv.FixedPeriodId != null && ((c.acsv.FixedPeriodId == courseSectionData.FixedPeriodId && (Regex.IsMatch((courseSectionData.FixedDays??"").ToLower(),(c.acsv.FixedDays??"").ToLower(), RegexOptions.IgnoreCase))) || (c.acsv.FixedPeriodId == courseSectionData.VarPeriodId && (c.acsv.FixedDays ?? "").ToLower().Contains((courseSectionData.VarDay ?? "").ToLower())) || (c.acsv.FixedPeriodId == courseSectionData.CalPeriodId && (c.acsv.FixedDays ?? "").ToLower().Contains((courseSectionData.CalDay ?? "").ToLower())))) ||

                                                   (c.acsv.VarPeriodId != null && ((c.acsv.VarPeriodId == courseSectionData.FixedPeriodId && (courseSectionData.FixedDays ?? "").ToLower().Contains((c.acsv.VarDay ?? "").ToLower())) || (c.acsv.VarPeriodId == courseSectionData.VarPeriodId && (c.acsv.VarDay ?? "").ToLower() == (courseSectionData.VarDay ?? "").ToLower()) || (c.acsv.VarPeriodId == courseSectionData.CalPeriodId && (c.acsv.VarDay ?? "").ToLower() == (courseSectionData.CalDay ?? "").ToLower()))) ||

                                                   (c.acsv.CalPeriodId != null && ((c.acsv.CalPeriodId == courseSectionData.FixedPeriodId && (courseSectionData.FixedDays ?? "").ToLower().Contains((c.acsv.CalDay ?? "").ToLower())) || (c.acsv.CalPeriodId == courseSectionData.VarPeriodId && (c.acsv.CalDay ?? "").ToLower() == (courseSectionData.VarDay ?? "").ToLower()) || (c.acsv.CalPeriodId == courseSectionData.CalPeriodId && (c.acsv.CalDay ?? "").ToLower() == (courseSectionData.CalDay ?? "").ToLower()))) ||

                                                   (c.acsv.BlockPeriodId != null && (c.acsv.BlockPeriodId == courseSectionData.BlockPeriodId && c.acsv.BlockRoomId == courseSectionData.BlockRoomId && c.acsv.BlockId == courseSectionData.BlockId))) && c.acsv.DurationEndDate > courseSectionData.DurationStartDate && c.scss.IsDropped != true).ToList();

                                            if (checkForConflict?.Any() == true)
                                            {
                                                staffListViewModel._failure = true;
                                                staffListViewModel._message = "Conflict Detected";
                                                var indexValue = staffListViewModel.CourseSectionsList.FindIndex(x => x.CourseSectionId == courseSection.CourseSectionId);
                                                staffListViewModel.ConflictIndexNo = staffListViewModel.ConflictIndexNo != null ? staffListViewModel.ConflictIndexNo + "," + indexValue.ToString() : indexValue.ToString();

                                            }
                                        }
                                    }
                                }
                            }
                        }
                        
                    }
                }
                else
                {
                    staffListViewModel._failure = true;
                    staffListViewModel._message = "Select CourseSection For StaffCourseSection Re-Schedule";
                    return staffListViewModel;
                }
            }
            catch (Exception es)
            {
                staffListViewModel._failure = true;
                staffListViewModel._message = es.Message;
            }
            return staffListViewModel;
        }

        /// <summary>
        /// Add Staff Course Section Re-Schedule By Course Wise
        /// </summary>
        /// <param name="staffListViewModel"></param>
        /// <returns></returns>
        public StaffListViewModel AddStaffCourseSectionReScheduleByCourse(StaffListViewModel staffListViewModel)
        {
            try
            {
                if (staffListViewModel.CourseSectionsList.Count() > 0)
                {
                    var staffGuid = this.context?.StaffMaster.Where(x => x.StaffId == staffListViewModel.ReScheduleStaffId && x.SchoolId == staffListViewModel.SchoolId && x.TenantId == staffListViewModel.TenantId).FirstOrDefault()?.StaffGuid;

                    foreach (var courseSection in staffListViewModel.CourseSectionsList.ToList())
                    {
                        var StaffScheduleData = this.context?.StaffCoursesectionSchedule.FirstOrDefault(x => x.TenantId == staffListViewModel.TenantId && x.SchoolId == staffListViewModel.SchoolId && x.StaffId == courseSection.StaffCoursesectionSchedule.FirstOrDefault()!.StaffId && x.CourseSectionId == courseSection.CourseSectionId);

                        if (StaffScheduleData != null)
                        {
                            StaffScheduleData.IsDropped = true;
                            StaffScheduleData.EffectiveDropDate = DateTime.UtcNow;
                            StaffScheduleData.IsPrimaryStaff = false;
                        }
                        else
                        {
                            staffListViewModel._failure = true;
                            staffListViewModel._message = "Exixting Staff Does't in this CourseSection";
                            return staffListViewModel;
                        }
                        var StaffScheduleDataAlreadyExist = this.context?.StaffCoursesectionSchedule.FirstOrDefault(x => x.TenantId == staffListViewModel.TenantId && x.SchoolId == staffListViewModel.SchoolId && x.StaffId == staffListViewModel.ReScheduleStaffId && x.CourseId == courseSection.CourseId && x.CourseSectionId == courseSection.CourseSectionId);

                        if (StaffScheduleDataAlreadyExist != null)
                        {
                            StaffScheduleDataAlreadyExist.IsDropped = null;
                            StaffScheduleDataAlreadyExist.EffectiveDropDate = null;
                            StaffScheduleDataAlreadyExist.IsPrimaryStaff = true;
                        }
                        else
                        {
                            var staffCoursesectionSchedule = new StaffCoursesectionSchedule()
                            {
                                TenantId = (Guid)staffListViewModel.TenantId!,
                                SchoolId = (int)staffListViewModel.SchoolId!,
                                StaffId = (int)staffListViewModel.ReScheduleStaffId!,
                                CourseId = courseSection.CourseId,
                                CourseSectionId = courseSection.CourseSectionId,
                                StaffGuid = (Guid)staffGuid!,
                                CourseSectionName = courseSection.CourseSectionName,
                                YrMarkingPeriodId = courseSection.YrMarkingPeriodId,
                                SmstrMarkingPeriodId = courseSection.SmstrMarkingPeriodId,
                                QtrMarkingPeriodId = courseSection.QtrMarkingPeriodId,
                                PrgrsprdMarkingPeriodId = courseSection.PrgrsprdMarkingPeriodId,
                                DurationStartDate = courseSection.DurationStartDate,
                                DurationEndDate = courseSection.DurationEndDate,
                                MeetingDays = courseSection.StaffCoursesectionSchedule.FirstOrDefault()!.MeetingDays,
                                CreatedBy = staffListViewModel.CreatedBy,
                                CreatedOn = DateTime.UtcNow,
                                IsAssigned = true,
                                IsPrimaryStaff = true
                            };
                            this.context?.StaffCoursesectionSchedule.Add(staffCoursesectionSchedule);
                        }
                    }
                    this.context?.SaveChanges();
                    staffListViewModel._message = "Staff  Re-Schedule In CourseSection Successfully";
                    staffListViewModel._failure = false;
                }
                else
                {
                    staffListViewModel._failure = true;
                    staffListViewModel._message = "Select CourseSection For Staff  Re-Schedule";
                }
            }
            catch (Exception es)
            {
                staffListViewModel._failure = true;
                staffListViewModel._message = es.Message;
            }
            return staffListViewModel;
        }

        /// <summary>
        /// Remove Staff Course Section Schedule
        /// </summary>
        /// <param name="removestaffScheduleViewModel"></param>
        /// <returns></returns>

        public RemoveStaffScheduleViewModel RemoveStaffCourseSectionSchedule(RemoveStaffScheduleViewModel removeStaffScheduleViewModel)
        {
            if (removeStaffScheduleViewModel == null)
            {
                return removeStaffScheduleViewModel!;
            }
            try
            {
                var StaffScheduleCourseSectionData = this.context?.StaffCoursesectionSchedule.Where(x => x.TenantId == removeStaffScheduleViewModel.TenantId && x.SchoolId == removeStaffScheduleViewModel.SchoolId && x.StaffId == removeStaffScheduleViewModel.StaffId && x.CourseSectionId == removeStaffScheduleViewModel.CourseSectionId).FirstOrDefault();

                if (StaffScheduleCourseSectionData != null)
                {
                    StaffScheduleCourseSectionData.IsDropped = true;
                    StaffScheduleCourseSectionData.EffectiveDropDate = DateTime.UtcNow;
                    this.context?.SaveChanges();
                    removeStaffScheduleViewModel._failure = false;
                    removeStaffScheduleViewModel._message = "Schudule Staff Remove Successfully";
                }
                else
                {
                    removeStaffScheduleViewModel._failure = true;
                    removeStaffScheduleViewModel._message = "No Record Found";
                }
            }
            catch (Exception es)
            {
                removeStaffScheduleViewModel._failure = true;
                removeStaffScheduleViewModel._message = es.Message;
            }
            return removeStaffScheduleViewModel;
        }

        /// <summary>
		/// Get Unassociated Staff List By CourseSection
		/// </summary>
		/// <param name="staffListViewModel"></param>
		/// <returns></returns>
        public StaffListViewModel GetUnassociatedStaffListByCourseSection(StaffListViewModel staffListViewModel)
        {
            StaffListViewModel staffListView = new StaffListViewModel();
            try
            {
                staffListView.TenantId = staffListViewModel.TenantId;
                staffListView.SchoolId = staffListViewModel.SchoolId;
                staffListView.CourseId = staffListViewModel.CourseId;
                staffListView.CourseSectionId = staffListViewModel.CourseSectionId;
                staffListView._token = staffListViewModel._token;
                staffListView._tenantName = staffListViewModel._tenantName;

                List<int> staffIds = new List<int>();

                var StaffScheduleCourseSectionMasterData = this.context?.StaffCoursesectionSchedule.Where(x => x.TenantId == staffListViewModel.TenantId && x.SchoolId == staffListViewModel.SchoolId && x.CourseSectionId == staffListViewModel.CourseSectionId).ToList();

                if (StaffScheduleCourseSectionMasterData?.Any() == true)
                {
                    var StudentAttendanceMasterData = this.context?.StudentAttendance.Where(x => x.TenantId == staffListViewModel.TenantId && x.SchoolId == staffListViewModel.SchoolId && x.CourseSectionId == staffListViewModel.CourseSectionId);

                    var AssignmentMasterData = this.context?.Assignment.Where(x => x.TenantId == staffListViewModel.TenantId && x.SchoolId == staffListViewModel.SchoolId && x.CourseSectionId == staffListViewModel.CourseSectionId);

                    foreach (var scheduledStaff in StaffScheduleCourseSectionMasterData)
                    {
                        var StudentAttendanceData = StudentAttendanceMasterData?.Where(x => x.StaffId == scheduledStaff.StaffId).FirstOrDefault();
                        var AssignmentData = AssignmentMasterData?.Where(x => x.StaffId == scheduledStaff.StaffId).FirstOrDefault();

                        if (StudentAttendanceData == null && AssignmentData == null)
                        {
                            staffIds.Add(scheduledStaff.StaffId);
                        }
                    }

                    if (staffIds?.Any() == true)
                    {
                        var staffSchedule = this.context?.CourseSection.Include(x => x.SchoolYears).Include(x => x.Semesters).Include(x => x.Quarters).Include(x => x.StaffCoursesectionSchedule).ThenInclude(x => x.StaffMaster).Where(x => x.TenantId == staffListViewModel.TenantId && x.SchoolId == staffListViewModel.SchoolId && x.CourseId == staffListViewModel.CourseId && (staffListViewModel.CourseSectionId == null || x.CourseSectionId == staffListViewModel.CourseSectionId)).AsNoTracking().Select(e => new CourseSection
                        {
                            TenantId = e.TenantId,
                            SchoolId = e.SchoolId,
                            CourseId = e.CourseId,
                            CourseSectionId = e.CourseSectionId,
                            CourseSectionName = e.CourseSectionName,
                            ScheduleType = e.ScheduleType,
                            YrMarkingPeriodId = e.YrMarkingPeriodId,
                            SmstrMarkingPeriodId = e.SmstrMarkingPeriodId,
                            QtrMarkingPeriodId = e.QtrMarkingPeriodId,
                            DurationStartDate = e.DurationStartDate,
                            DurationEndDate = e.DurationEndDate,
                            ProgressPeriods = e.ProgressPeriods != null ? new ProgressPeriods { Title = e.ProgressPeriods.Title, StartDate = e.ProgressPeriods.StartDate, EndDate = e.ProgressPeriods.EndDate, ShortName = e.ProgressPeriods.ShortName } : null,
                            Quarters = e.Quarters != null ? new Quarters { Title = e.Quarters.Title, StartDate = e.Quarters.StartDate, EndDate = e.Quarters.EndDate, ShortName = e.Quarters.ShortName } : null,
                            Semesters = e.Semesters != null ? new Semesters { Title = e.Semesters.Title, StartDate = e.Semesters.StartDate, EndDate = e.Semesters.EndDate, ShortName = e.Semesters.ShortName } : null,
                            SchoolYears = e.SchoolYears != null ? new SchoolYears { Title = e.SchoolYears.Title, StartDate = e.SchoolYears.StartDate, EndDate = e.SchoolYears.EndDate, ShortName = e.SchoolYears.ShortName } : null,
                            StaffCoursesectionSchedule = e.StaffCoursesectionSchedule.Where(d => d.IsDropped != true && staffIds.Contains(d.StaffId)).Select(s => new StaffCoursesectionSchedule
                            {
                                TenantId = s.TenantId,
                                SchoolId = s.SchoolId,
                                StaffId = s.StaffId,
                                StaffGuid = s.StaffGuid,
                                CourseId = s.CourseId,
                                CourseSectionId = s.CourseSectionId,
                                CourseSectionName = s.CourseSectionName,
                                IsDropped = s.IsDropped,
                                MeetingDays = s.MeetingDays,
                                IsPrimaryStaff = s.IsPrimaryStaff,
                                StaffMaster = new StaffMaster
                                {
                                    TenantId = s.StaffMaster.TenantId,
                                    SchoolId = s.StaffMaster.SchoolId,
                                    StaffId = s.StaffMaster.StaffId,
                                    FirstGivenName = s.StaffMaster.FirstGivenName,
                                    MiddleName = s.StaffMaster.MiddleName,
                                    LastFamilyName = s.StaffMaster.LastFamilyName,
                                    StaffInternalId = s.StaffMaster.StaffInternalId,
                                    StaffThumbnailPhoto = staffListViewModel.CourseSectionId != null ? s.StaffMaster.StaffThumbnailPhoto : null,
                                    FirstLanguage = s.StaffMaster.FirstLanguage,
                                    SecondLanguage = s.StaffMaster.SecondLanguage,
                                    ThirdLanguage = s.StaffMaster.ThirdLanguage,
                                    FirstLanguageNavigation = s.StaffMaster.FirstLanguageNavigation,
                                    SecondLanguageNavigation = s.StaffMaster.SecondLanguageNavigation,
                                    ThirdLanguageNavigation = s.StaffMaster.ThirdLanguageNavigation,
                                    JobTitle = s.StaffMaster.JobTitle,
                                    PrimaryGradeLevelTaught = s.StaffMaster.PrimaryGradeLevelTaught,
                                    OtherGradeLevelTaught = s.StaffMaster.OtherGradeLevelTaught,
                                    PrimarySubjectTaught = s.StaffMaster.PrimarySubjectTaught,
                                    OtherSubjectTaught = s.StaffMaster.OtherSubjectTaught
                                }
                            }).ToList()
                        }).ToList();

                        if (staffSchedule?.Any() == true)
                        {
                            staffListView.CourseSectionsList = staffSchedule;
                            staffListView._failure = false;
                        }
                        else
                        {
                            staffListView._failure = true;
                            staffListView._message = "No staff found";
                        }
                    }
                    else
                    {
                        staffListView._failure = true;
                        staffListView._message = "Staff deletion is not permitted due to transactional associations";
                    }
                }
                else
                {
                    staffListView._failure = true;
                    staffListView._message = "No staff found";
                }
            }
            catch (Exception es)
            {
                staffListView.CourseSectionsList = null!;
                staffListView._failure = true;
                staffListView._message = es.Message;
            }

            return staffListView;
        }
    }
}

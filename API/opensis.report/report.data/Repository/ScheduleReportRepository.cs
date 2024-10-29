using Microsoft.EntityFrameworkCore;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.Period;
using opensis.report.report.data.Interface;
using opensis.report.report.data.ViewModels.ScheduleReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace opensis.report.report.data.Repository
{
    public class ScheduleReportRepository : IScheduleReportRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public ScheduleReportRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }


        /// <summary>
        /// Get Scheduled Add/Drop Report
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public AddDropScheduleReport GetScheduledAddDropReport(PageResult pageResult)
        {
            AddDropScheduleReport scheduleAddDrop = new();
            IQueryable<StudentScheduleData>? transactionIQ = null;
            try
            {
                scheduleAddDrop.TenantId = pageResult.TenantId;
                scheduleAddDrop.SchoolId = pageResult.SchoolId;
                scheduleAddDrop.FromDate = pageResult.DobStartDate;
                scheduleAddDrop.ToDate = pageResult.DobEndDate;
                scheduleAddDrop._token = pageResult._token;
                scheduleAddDrop._userName = pageResult._userName;
                scheduleAddDrop._tenantName = pageResult._tenantName;
                scheduleAddDrop.PageNumber = pageResult.PageNumber;
                scheduleAddDrop._pageSize = pageResult.PageSize;

                var schoolData = this.context?.SchoolMaster.Include(x => x.SchoolDetail).FirstOrDefault(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId);
                if (schoolData != null)
                {
                    scheduleAddDrop.SchoolName = schoolData.SchoolName;
                    scheduleAddDrop.StreetAddress1 = schoolData.StreetAddress1;
                    scheduleAddDrop.StreetAddress2 = schoolData.StreetAddress2;
                    scheduleAddDrop.Country = schoolData.Country;
                    scheduleAddDrop.City = schoolData.City;
                    scheduleAddDrop.State = schoolData.State;
                    scheduleAddDrop.District = schoolData.District;
                    scheduleAddDrop.Zip = schoolData.Zip;
                    scheduleAddDrop.SchoolLogo = schoolData.SchoolDetail.FirstOrDefault().SchoolLogo;

                }
        //        var studentScheduleAddDropdata = this.context?.StudentCoursesectionSchedule.
        //Join(this.context?.StaffCoursesectionSchedule, std => std.CourseSectionId, stf => stf.CourseSectionId,
        //(std, stf) => new { std, stf }).
        //Join(this.context?.Course, r => r.std.CourseId, cs => cs.CourseId, (r, cs) => new { r, cs }).Join(this.context?.StaffMaster, s => s.r.stf.StaffId, sm => sm.StaffId, (s, sm) => new { s, sm })
        //.Where(x => x.s.r.std.TenantId == pageResult.TenantId && x.s.r.std.SchoolId == pageResult.SchoolId && x.s.r.stf.TenantId == pageResult.TenantId && x.s.r.stf.SchoolId == pageResult.SchoolId && x.s.cs.TenantId == pageResult.TenantId && x.s.cs.SchoolId == pageResult.SchoolId && x.sm.TenantId == pageResult.TenantId && x.sm.SchoolId == pageResult.SchoolId && ((x.s.r.std.EffectiveStartDate >= pageResult.DobStartDate && x.s.r.std.EffectiveStartDate <= pageResult.DobEndDate) || (x.s.r.std.EffectiveDropDate >= pageResult.DobStartDate && x.s.r.std.EffectiveDropDate <= pageResult.DobEndDate)))
        //.Select(m => new StudentScheduleData
        //{
        //    TenantId = m.s.r.std.TenantId,
        //    SchoolId = m.s.r.std.SchoolId,
        //    StudentId = m.s.r.std.StudentId,
        //    StudentGuid = m.s.r.std.StudentGuid,
        //    StudentInternalId = m.s.r.std.StudentInternalId,
        //    CourseName = m.s.cs.CourseTitle,
        //    CourseSectionName = m.s.r.std.CourseSectionName,
        //    EnrolledDate = m.s.r.std.EffectiveStartDate,
        //    DropDate = m.s.r.std.EffectiveDropDate,
        //    StaffName = m.sm.FirstGivenName + " " + (m.s.r.std.MiddleName != null ? m.s.r.std.MiddleName + " " : "") + m.sm.LastFamilyName,
        //    StudentName = m.s.r.std.FirstGivenName + " " + (m.s.r.std.MiddleName != null ? m.s.r.std.MiddleName + " " : "") + m.s.r.std.LastFamilyName,
        //});

                var studentScheduleData = this.context?.StudentCoursesectionSchedule.Include(x => x.CourseSection).Include(x => x.CourseSection.StaffCoursesectionSchedule).ThenInclude(x => x.StaffMaster).Include(x => x.CourseSection.Course).Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && ((x.EffectiveStartDate >= pageResult.MarkingPeriodStartDate && x.EffectiveStartDate <= pageResult.MarkingPeriodEndDate) || (x.EffectiveDropDate >= pageResult.MarkingPeriodStartDate && x.EffectiveDropDate <= pageResult.MarkingPeriodEndDate)) && ((x.EffectiveStartDate > x.CourseSection.DurationStartDate && x.EffectiveDropDate < x.CourseSection.DurationEndDate) || (x.EffectiveStartDate == x.CourseSection.DurationStartDate && x.EffectiveDropDate < x.CourseSection.DurationEndDate) || (x.EffectiveStartDate > x.CourseSection.DurationStartDate && x.EffectiveDropDate == x.CourseSection.DurationEndDate))).Select(m => new StudentScheduleData
                  {
                    TenantId = m.TenantId,
                    SchoolId = m.SchoolId,
                    StudentId = m.StudentId,
                    StudentGuid = m.StudentGuid,
                    CourseId = m.CourseSection.Course.CourseId,
                    CourseSectionId = m.CourseSection.CourseSectionId,
                    StudentInternalId = m.StudentInternalId,
                    CourseName = m.CourseSection.Course.CourseTitle,
                    CourseSectionName = m.CourseSectionName,
                    EnrolledDate = m.EffectiveStartDate,
                    DropDate = m.EffectiveDropDate,
                    StudentName = m.FirstGivenName + " " + (m.MiddleName != null ? m.MiddleName + " " : "") + m.LastFamilyName,
                    StaffName = m.CourseSection.StaffCoursesectionSchedule.FirstOrDefault().StaffMaster.FirstGivenName != null ? (m.CourseSection.StaffCoursesectionSchedule.FirstOrDefault().StaffMaster.FirstGivenName + " " + (m.CourseSection.StaffCoursesectionSchedule.FirstOrDefault().StaffMaster.MiddleName != null ? m.CourseSection.StaffCoursesectionSchedule.FirstOrDefault().StaffMaster.MiddleName + " " : null) + m.CourseSection.StaffCoursesectionSchedule.FirstOrDefault().StaffMaster.LastFamilyName + " ") : null,
                });

                if (studentScheduleData?.Any() == true)
                {
                    if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                    {
                        transactionIQ = studentScheduleData;
                    }
                    else
                    {
                        string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                        //Columnvalue = Regex.Replace(Columnvalue, @"\s+", "");

                        if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                        {
                            transactionIQ = studentScheduleData.Where(x => x.StudentName != null && x.StudentName.ToLower().Contains(Columnvalue.ToLower()) || x.CourseName != null && x.CourseName.ToLower().Contains(Columnvalue.ToLower()) || x.CourseSectionName != null && x.CourseSectionName.ToLower().Contains(Columnvalue.ToLower()) || x.StaffName != null && x.StaffName.ToLower().Contains(Columnvalue.ToLower()) || x.StudentInternalId != null && x.StudentInternalId.ToLower().Contains(Columnvalue.ToLower()));
                        }
                    }

                    if (transactionIQ != null && transactionIQ.Any() == true)
                    {
                        if (pageResult.SortingModel != null)
                        {
                            transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn ?? "", (pageResult.SortingModel.SortDirection ?? "").ToLower());
                        }

                        int totalCount = transactionIQ.Count();

                        if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                        {
                            transactionIQ = transactionIQ.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                        }

                        scheduleAddDrop.studentCoursesectionScheduleList = transactionIQ.ToList();
                        scheduleAddDrop.TotalCount = totalCount;
                        scheduleAddDrop._failure = false;
                    }
                    else
                    {
                        scheduleAddDrop._failure = true;
                        scheduleAddDrop.studentCoursesectionScheduleList = new();
                        scheduleAddDrop._message = NORECORDFOUND;
                    }
                }
                else
                {
                    scheduleAddDrop._failure = true;
                    scheduleAddDrop.studentCoursesectionScheduleList = new();
                    scheduleAddDrop._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                scheduleAddDrop._failure = true;
                scheduleAddDrop._message = es.Message;
            }
            return scheduleAddDrop;

        }


        /// <summary>
        /// Scheduled CourseSection List
        /// </summary>
        /// <param name="courseSectionList"></param>
        /// <returns></returns>
        public ScheduleClassList ScheduledCourseSectionList(ScheduleClassList courseSectionList)
        {
            ScheduleClassList scheduleClass = new();
            scheduleClass.TenantId = courseSectionList.TenantId;
            scheduleClass.SchoolId = courseSectionList.SchoolId;
            scheduleClass._token = courseSectionList._token;
            scheduleClass._tenantName = courseSectionList._tenantName;
            try
            {
                var coursedata = this.context?.AllCourseSectionView.Where(x => x.TenantId == courseSectionList.TenantId && x.SchoolId == courseSectionList.SchoolId && x.AcademicYear == courseSectionList._academicYear && (courseSectionList.CourseId == null || x.CourseId == courseSectionList.CourseId) && (courseSectionList.CourseSubject == null || x.CourseSubject == courseSectionList.CourseSubject));

                if (coursedata != null && coursedata.Any())
                {
                    List<AllCourseSectionView> filteredPeriodCourseData = new();
                    var distinctCourseData = coursedata.Select(s => new AllCourseSectionView { TenantId = s.TenantId, SchoolId = s.SchoolId, CourseId = s.CourseId, CourseTitle = s.CourseTitle, CourseProgram = s.CourseProgram, CourseSubject = s.CourseSubject, AcademicYear = s.AcademicYear, CourseSectionId = s.CourseSectionId, CourseSectionName = s.CourseSectionName, YrMarkingPeriodId = s.YrMarkingPeriodId, SmstrMarkingPeriodId = s.SmstrMarkingPeriodId, QtrMarkingPeriodId = s.QtrMarkingPeriodId, PrgrsprdMarkingPeriodId = s.PrgrsprdMarkingPeriodId, IsActive = s.IsActive, DurationStartDate = s.DurationStartDate, DurationEndDate = s.DurationEndDate, Seats = s.Seats, CourseGradeLevel = s.CourseGradeLevel, FixedPeriodId = s.FixedPeriodId, VarPeriodId = s.VarPeriodId, CalPeriodId = s.CalPeriodId, BlockPeriodId = s.BlockPeriodId, GradeScaleId = s.GradeScaleId, AllowStudentConflict = s.AllowStudentConflict, AllowTeacherConflict = s.AllowTeacherConflict, ScheduleType = s.ScheduleType }).Distinct().ToList();

                    if (courseSectionList.MarkingPeriodStartDate != null && courseSectionList.MarkingPeriodEndDate != null)
                    {
                        distinctCourseData = distinctCourseData.Where(x => (x.YrMarkingPeriodId == null && x.SmstrMarkingPeriodId == null && x.QtrMarkingPeriodId == null && x.PrgrsprdMarkingPeriodId == null) || ((courseSectionList.MarkingPeriodStartDate >= x.DurationStartDate && courseSectionList.MarkingPeriodStartDate <= x.DurationEndDate) && (courseSectionList.MarkingPeriodEndDate >= x.DurationStartDate && courseSectionList.MarkingPeriodEndDate <= x.DurationEndDate))).ToList();
                    }

                    if (courseSectionList.BlockPeriodId != null)
                    {
                        filteredPeriodCourseData = distinctCourseData.Where(x => x.FixedPeriodId == courseSectionList.BlockPeriodId || x.VarPeriodId == courseSectionList.BlockPeriodId || x.CalPeriodId == courseSectionList.BlockPeriodId || x.BlockPeriodId == courseSectionList.BlockPeriodId).ToList();
                    }
                    else
                    {
                        filteredPeriodCourseData = distinctCourseData;
                    }


                    if (filteredPeriodCourseData.Any() == true)
                    {
                        List<CourseSectionForStaff> sectionList = new();
                        foreach (var CourseData in filteredPeriodCourseData)
                        {
                            List<string> staffName = new();
                            var staffSchedule = this.context?.StaffCoursesectionSchedule.Include(x => x.StaffMaster).Where(x => x.TenantId == courseSectionList.TenantId && x.SchoolId == courseSectionList.SchoolId && x.CourseSectionId == CourseData.CourseSectionId && x.CourseId == CourseData.CourseId && (courseSectionList.StaffId == null || x.StaffId == courseSectionList.StaffId) && x.IsDropped != false).ToList();
                            CourseSectionForStaff section = new();

                            if (staffSchedule.Any() == true && courseSectionList.StaffId != null)
                            {
                                foreach (var scheduleList in staffSchedule)
                                {
                                    staffName.Add($"{scheduleList.StaffMaster.FirstGivenName} {(scheduleList.StaffMaster.MiddleName == null ? "" : $"{scheduleList.StaffMaster.MiddleName} ")}{scheduleList.StaffMaster.LastFamilyName}");

                                    var totalStudent = this.context?.StudentCoursesectionSchedule.Include(s => s.StudentMaster).Where(x => x.TenantId == CourseData.TenantId && x.SchoolId == CourseData.SchoolId && x.CourseId == CourseData.CourseId && x.CourseSectionId == CourseData.CourseSectionId && x.IsDropped != true && x.StudentMaster.IsActive == true).ToList().Count;

                                    section.CourseSectionName = CourseData.CourseSectionName;
                                    section.SchoolId = CourseData.SchoolId;
                                    section.TenantId = CourseData.TenantId;
                                    section.ScheduledStudentCount = totalStudent;
                                    section.CourseSectionId = CourseData.CourseSectionId;
                                    section.CourseSubject = CourseData.CourseSubject;
                                    section.CourseTitle = CourseData.CourseTitle;
                                    section.CourseProgram = CourseData.CourseProgram;
                                    section.CourseId = CourseData.CourseId;
                                    section.StaffName = string.Join(", ", staffName);
                                    sectionList.Add(section);
                                }
                            }
                            if (courseSectionList.StaffId == null)
                            {
                                foreach (var scheduleList in staffSchedule)
                                {
                                    staffName.Add($"{scheduleList.StaffMaster.FirstGivenName} {(scheduleList.StaffMaster.MiddleName == null ? "" : $"{scheduleList.StaffMaster.MiddleName} ")}{scheduleList.StaffMaster.LastFamilyName}");
                                }

                                var totalStudent = this.context?.StudentCoursesectionSchedule.Include(s => s.StudentMaster).Where(x => x.TenantId == CourseData.TenantId && x.SchoolId == CourseData.SchoolId && x.CourseId == CourseData.CourseId && x.CourseSectionId == CourseData.CourseSectionId && x.IsDropped != true && x.StudentMaster.IsActive == true).ToList().Count;

                                section.CourseSectionName = CourseData.CourseSectionName;
                                section.SchoolId = CourseData.SchoolId;
                                section.TenantId = CourseData.TenantId;
                                section.ScheduledStudentCount = totalStudent;
                                section.CourseSectionId = CourseData.CourseSectionId;
                                section.CourseSubject = CourseData.CourseSubject;
                                section.CourseTitle = CourseData.CourseTitle;
                                section.CourseProgram = CourseData.CourseProgram;
                                section.CourseId = CourseData.CourseId;
                                section.StaffName = string.Join(", ", staffName);
                                sectionList.Add(section);
                            }
                        }

                        foreach (var data in sectionList)
                        {
                            if (!scheduleClass.CourseSectionViewList.Any(p => p.SchoolId == data.SchoolId && p.CourseSectionId == data.CourseSectionId))
                            {
                                scheduleClass.CourseSectionViewList.Add(data);
                            }
                        }

                        if (scheduleClass.CourseSectionViewList.Count == 0)
                        {
                            scheduleClass._failure = true;
                            scheduleClass._message = NORECORDFOUND;
                        }
                    }
                    else
                    {
                        scheduleClass._failure = true;
                        scheduleClass._message = NORECORDFOUND;
                    }
                }
                else
                {
                    scheduleClass._failure = true;
                    scheduleClass._message = NORECORDFOUND;
                }
            }
            catch (Exception ex)
            {
                scheduleClass._failure = true;
                scheduleClass._message = ex.Message;
            }

            return scheduleClass;
        }


        /// <summary>
        /// Scheduled CourseSection List
        /// </summary>
        /// <param name="courseSectionList"></param>
        /// <returns></returns>
        public StudentScheduledListModel GetStudentListByCourseSection(StudentScheduledListModel studentList)
        {
            StudentScheduledListModel studentScheduledListModel = new();

            studentScheduledListModel.TenantId = studentList.TenantId;
            studentScheduledListModel.SchoolId = studentList.SchoolId;
            studentScheduledListModel._tenantName = studentList._tenantName;
            studentScheduledListModel._token = studentList._token;
            studentScheduledListModel._tokenExpiry = studentList._tokenExpiry;
            List<CourseSectionForStaff> sectionList = new();
            int? studentCount = 0;
            foreach (var ids in studentList.courseIds)
            {
                var coursedata = this.context?.AllCourseSectionView.Where(x => x.TenantId == studentList.TenantId && x.SchoolId == studentList.SchoolId && x.CourseId == ids.CourseId && x.CourseSectionId == ids.CourseSectionId);

                if (coursedata != null && coursedata.Any())
                {
                    var distinctCourseData = coursedata.Select(s => new AllCourseSectionView { TenantId = s.TenantId, SchoolId = s.SchoolId, CourseId = s.CourseId, CourseTitle = s.CourseTitle, CourseProgram = s.CourseProgram, CourseSubject = s.CourseSubject, AcademicYear = s.AcademicYear, CourseSectionId = s.CourseSectionId, CourseSectionName = s.CourseSectionName, YrMarkingPeriodId = s.YrMarkingPeriodId, SmstrMarkingPeriodId = s.SmstrMarkingPeriodId, QtrMarkingPeriodId = s.QtrMarkingPeriodId, PrgrsprdMarkingPeriodId = s.PrgrsprdMarkingPeriodId, IsActive = s.IsActive, DurationStartDate = s.DurationStartDate, DurationEndDate = s.DurationEndDate, Seats = s.Seats, CourseGradeLevel = s.CourseGradeLevel, FixedPeriodId = s.FixedPeriodId, VarPeriodId = s.VarPeriodId, CalPeriodId = s.CalPeriodId, BlockPeriodId = s.BlockPeriodId, GradeScaleId = s.GradeScaleId, AllowStudentConflict = s.AllowStudentConflict, AllowTeacherConflict = s.AllowTeacherConflict, ScheduleType = s.ScheduleType }).Distinct().ToList();

                    if (distinctCourseData.Any() == true)
                    {
                        CourseSectionForStaff section = new();
                        foreach (var CourseData in distinctCourseData)
                        {
                            List<string> staffName = new();
                            var staffSchedule = this.context?.StaffCoursesectionSchedule.Include(x => x.StaffMaster).Where(x => x.TenantId == studentList.TenantId && x.SchoolId == studentList.SchoolId && x.CourseSectionId == CourseData.CourseSectionId && x.CourseId == CourseData.CourseId && x.IsDropped != false).ToList();

                            foreach (var scheduleList in staffSchedule)
                            {
                                staffName.Add($"{scheduleList.StaffMaster.FirstGivenName} { (scheduleList.StaffMaster.MiddleName == null ? "" : $"{scheduleList.StaffMaster.MiddleName} ")}{scheduleList.StaffMaster.LastFamilyName}");
                            }
                            List<StudentListForCoursection> studentListForCourseSectionReport = new();
                            var seduleStudents = this.context?.StudentCoursesectionSchedule.Include(s => s.StudentMaster).ThenInclude(x => x.StudentEnrollment).Where(x => x.TenantId == CourseData.TenantId && x.SchoolId == CourseData.SchoolId && x.CourseId == CourseData.CourseId && x.CourseSectionId == CourseData.CourseSectionId && x.IsDropped != true && x.StudentMaster.IsActive == true).ToList();

                            foreach (var e in seduleStudents)
                            {
                                StudentListForCoursection studentListForCoursection = new();
                                var studentView = new StudentViewForCoursection()
                                {

                                    StudentInternalId = e.StudentInternalId,
                                    StudentGuid = e.StudentGuid,
                                    AlternateId = e.AlternateId,
                                    DistrictId = e.StudentMaster.DistrictId,
                                    StateId = e.StudentMaster.StateId,
                                    AdmissionNumber = e.StudentMaster.AdmissionNumber,
                                    RollNumber = e.StudentMaster.RollNumber,
                                    Salutation = e.StudentMaster.Salutation,
                                    FirstGivenName = e.FirstGivenName,
                                    MiddleName = e.MiddleName,
                                    LastFamilyName = e.LastFamilyName,
                                    Suffix = e.StudentMaster.Suffix,
                                    PreferredName = e.StudentMaster.PreferredName,
                                    PreviousName = e.StudentMaster.PreviousName,
                                    SchoolEmail = e.StudentMaster.SchoolEmail,
                                    MobilePhone = e.StudentMaster.MobilePhone,
                                    SocialSecurityNumber = e.StudentMaster.SocialSecurityNumber,
                                    OtherGovtIssuedNumber = e.StudentMaster.OtherGovtIssuedNumber,
                                    Dob = e.StudentMaster.Dob,
                                    Gender = e.StudentMaster.Gender,
                                    Race = e.StudentMaster.Race,
                                    Ethnicity = e.StudentMaster.Ethnicity,
                                    MaritalStatus = e.StudentMaster.MaritalStatus,
                                    CountryOfBirth = e.StudentMaster.CountryOfBirth != null ? this.context!.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(e.StudentMaster.CountryOfBirth))!.Name : null,
                                    Nationality = e.StudentMaster.Nationality != null ? this.context!.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(e.StudentMaster.Nationality))!.Name : null,
                                    FirstLanguage = e.FirstLanguageId != null ? this.context!.Language.FirstOrDefault(x => x.LangId == Convert.ToInt32(e.FirstLanguageId))!.Locale : null,
                                    SectionId = e.StudentMaster.SectionId,
                                    SectionName = e.StudentMaster.SectionId != null ? this.context!.Sections.FirstOrDefault(x => x.SectionId == Convert.ToInt32(e.StudentMaster.SectionId) && x.SchoolId == e.StudentMaster.SchoolId && x.TenantId == e.StudentMaster.TenantId).Name : null,
                                    EstimatedGradDate = e.StudentMaster.EstimatedGradDate,
                                    GradeLevelTitle = e.GradeId != null ? this.context!.Gradelevels.FirstOrDefault(x => x.GradeId == Convert.ToInt32(e.GradeId) && x.SchoolId == e.StudentMaster.SchoolId && x.TenantId == e.StudentMaster.TenantId).Title : null,
                                    Twitter = e.StudentMaster.Twitter,
                                    Facebook = e.StudentMaster.Facebook,
                                    Instagram = e.StudentMaster.Instagram,
                                    Youtube = e.StudentMaster.Youtube,
                                    Linkedin = e.StudentMaster.Linkedin,
                                    HomeAddressLineOne = e.StudentMaster.HomeAddressLineOne,
                                    HomeAddressLineTwo = e.StudentMaster.HomeAddressLineTwo,
                                    HomeAddressCountry = e.StudentMaster.HomeAddressCountry != null ? this.context!.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(e.StudentMaster.HomeAddressCountry))!.Name : null,
                                    HomeAddressCity = e.StudentMaster.HomeAddressCity,
                                    HomeAddressState = e.StudentMaster.HomeAddressState,
                                    HomeAddressZip = e.StudentMaster.HomeAddressZip,
                                    MailingAddressSameToHome = e.StudentMaster.MailingAddressSameToHome,
                                    MailingAddressLineOne = e.StudentMaster.MailingAddressLineOne,
                                    MailingAddressLineTwo = e.StudentMaster.MailingAddressLineTwo,
                                    MailingAddressCountry = e.StudentMaster.MailingAddressCountry != null ? this.context!.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(e.StudentMaster.MailingAddressCountry))!.Name : null,
                                    MailingAddressCity = e.StudentMaster.MailingAddressCity,
                                    MailingAddressState = e.StudentMaster.MailingAddressState,
                                    MailingAddressZip = e.StudentMaster.MailingAddressZip,
                                    IsActive = e.StudentMaster.IsActive,
                                };

                                var customFields = this.context?.FieldsCategory.Where(x => x.TenantId == CourseData.TenantId && x.SchoolId == CourseData.SchoolId && x.Module == "Student").OrderByDescending(x => x.IsSystemCategory).ThenBy(x => x.SortOrder)
                        .Select(y => new FieldsCategory
                        {
                            TenantId = y.TenantId,
                            SchoolId = y.SchoolId,
                            CategoryId = y.CategoryId,
                            IsSystemCategory = y.IsSystemCategory,
                            Search = y.Search,
                            Title = y.Title,
                            Module = y.Module,
                            SortOrder = y.SortOrder,
                            Required = y.Required,
                            Hide = y.Hide,
                            UpdatedOn = y.UpdatedOn,
                            UpdatedBy = y.UpdatedBy,
                            CreatedBy = y.CreatedBy,
                            CreatedOn = y.CreatedOn,
                            CustomFields = y.CustomFields.Where(x => x.SystemField != true).Select(z => new CustomFields
                            {
                                TenantId = z.TenantId,
                                SchoolId = z.SchoolId,
                                CategoryId = z.CategoryId,
                                FieldId = z.FieldId,
                                Module = z.Module,
                                Type = z.Type,
                                Search = z.Search,
                                Title = z.Title,
                                SortOrder = z.SortOrder,
                                SelectOptions = z.SelectOptions,
                                SystemField = z.SystemField,
                                Required = z.Required,
                                Hide = z.Hide,
                                DefaultSelection = z.DefaultSelection,
                                UpdatedOn = z.UpdatedOn,
                                UpdatedBy = z.UpdatedBy,
                                CreatedBy = z.CreatedBy,
                                CreatedOn = z.CreatedOn,
                                CustomFieldsValue = z.CustomFieldsValue.Where(w => w.TargetId == e.StudentId).ToList()
                            }).OrderByDescending(x => x.SystemField).ThenBy(x => x.SortOrder).ToList()
                        }).ToList();

                                studentListForCoursection.FieldsCategoryList = customFields;
                                studentListForCoursection.StudentView = studentView;
                                studentListForCourseSectionReport.Add(studentListForCoursection);
                            }

                            int? seduleStudentCount = seduleStudents != null ? seduleStudents.Count() : null;
                            var availableSeat = CourseData.Seats - seduleStudentCount;
                            section.StudentLists = studentListForCourseSectionReport;
                            studentCount = studentCount + studentListForCourseSectionReport.Count;
                            section.CourseSectionName = CourseData.CourseSectionName;
                            section.SchoolId = CourseData.SchoolId;
                            section.TenantId = CourseData.TenantId;
                            section.ScheduledStudentCount = seduleStudentCount;
                            section.CourseSectionId = CourseData.CourseSectionId;
                            section.CourseSubject = CourseData.CourseSubject;
                            section.CourseTitle = CourseData.CourseTitle;
                            section.CourseSubject = CourseData.CourseSubject;
                            section.CourseId = CourseData.CourseId;
                            section.StaffName = string.Join(", ", staffName);
                            section.AvailableSeat = availableSeat;
                            section.TotalSeats = CourseData.Seats;

                            sectionList.Add(section);
                        }
                    }
                }
            }
            studentScheduledListModel.courseSectionForStaffs = sectionList;
            studentScheduledListModel.TotalStudents = studentCount;

            return studentScheduledListModel;
        }

        public SchoolwideScheduleReportViewModel GetSchoolwideScheduleReport(SchoolwideScheduleReportViewModel schoolwideScheduleViewModel)
        {
            SchoolwideScheduleReportViewModel schoolwideScheduleListModel = new();

            try
            {
                int daydiff = (schoolwideScheduleViewModel.EndDate.Date - schoolwideScheduleViewModel.StartDate.Date).Days;
                if (daydiff == 6 && schoolwideScheduleViewModel.StartDate.DayOfWeek.ToString().ToLower() == "sunday")
                {
                    //******PeriodData
                    var blockDataList = this.context?.Block.Join(this.context?.BlockPeriod, blk => new { blk.TenantId, blk.SchoolId, blk.BlockId }, blkp => new { blkp.TenantId, blkp.SchoolId, blkp.BlockId }, (blk, blkp) => new
                    {
                        blk.TenantId,
                        blk.SchoolId,
                        blk.BlockId,
                        blk.BlockTitle,
                        blk.BlockSortOrder,
                        blk.HalfDayMinutes,
                        blk.FullDayMinutes,
                        blk.AcademicYear,
                        blkp.PeriodId,
                        blkp.PeriodTitle,
                        blkp.PeriodShortName,
                        blkp.PeriodSortOrder,
                        blkp.PeriodStartTime,
                        blkp.PeriodEndTime
                    }).Where(x => x.TenantId == schoolwideScheduleViewModel.TenantId && x.SchoolId == schoolwideScheduleViewModel.SchoolId && x.AcademicYear == schoolwideScheduleViewModel.AcademicYear).OrderBy(x => x.BlockSortOrder);

                    var blockPeriodDataList = blockDataList.GroupBy(g => new
                    {
                        g.BlockId

                    }).Select(b => new GetBlockListForView()
                    {
                        BlockId = b.Key.BlockId,
                        BlockPeriod = b.Select(c => new BlockPeriod
                        {
                            PeriodId = c.PeriodId,
                            PeriodTitle = c.PeriodTitle,
                            PeriodShortName = c.PeriodShortName,
                            PeriodSortOrder = c.PeriodSortOrder,
                            PeriodStartTime = c.PeriodStartTime,
                            PeriodEndTime = c.PeriodEndTime
                        }).ToList()
                    }).ToList();
                    schoolwideScheduleListModel.BlockListForView = blockPeriodDataList;
                    //*******calendar view
                    var CalendarData = this.context?.SchoolCalendars.Where(x => x.TenantId == schoolwideScheduleViewModel.TenantId && x.SchoolId == schoolwideScheduleViewModel.SchoolId && x.AcademicYear == schoolwideScheduleViewModel.AcademicYear && x.DefaultCalender == true)
                        .Select(x => new CalendarDataView
                        {
                            CalendarId = x.CalenderId,
                            StartDate = x.StartDate,
                            EndDate = x.EndDate,
                            Days = x.Days
                        }).FirstOrDefault();

                    var CalendarHolidayList = this.context?.CalendarEvents.Where(x => x.TenantId == schoolwideScheduleViewModel.TenantId && x.SchoolId == schoolwideScheduleViewModel.SchoolId && x.AcademicYear == schoolwideScheduleViewModel.AcademicYear && x.IsHoliday == true && x.CalendarId == CalendarData.CalendarId);
                    CalendarData.CalendarEvents = CalendarHolidayList.ToList();
                    schoolwideScheduleListModel.CalendarDataView = CalendarData;
                    //********Course Data

                    var data = this.context?.StaffCoursesectionSchedule.Include(x => x.StaffMaster).Join(this.context?.AllCourseSectionView, scs => new { scs.TenantId, scs.SchoolId, scs.CourseId, scs.CourseSectionId },
                         acsv => new { acsv.TenantId, acsv.SchoolId, acsv.CourseId, acsv.CourseSectionId }, (scs, acsv) => new {
                             acsv.AcademicYear,
                             acsv.CourseId,
                             acsv.CourseTitle,
                             acsv.CourseSectionId,
                             acsv.CourseSectionName,
                             acsv.ScheduleType,
                             acsv.FixedDays,
                             acsv.FixedPeriodId,
                             acsv.VarDay,
                             acsv.VarPeriodId,
                             acsv.CalDate,
                             acsv.CalDay,
                             acsv.CalPeriodId,
                             scs.TenantId,
                             scs.SchoolId,
                             scs.DurationStartDate,
                             scs.DurationEndDate,
                             scs.StaffId,
                             scs.IsPrimaryStaff,
                             scs.StaffMaster.FirstGivenName,
                             scs.StaffMaster.MiddleName,
                             scs.StaffMaster.LastFamilyName
                         }).Where(a => a.TenantId == schoolwideScheduleViewModel.TenantId && a.SchoolId == schoolwideScheduleViewModel.SchoolId && a.IsPrimaryStaff == true && a.AcademicYear == schoolwideScheduleViewModel.AcademicYear
                          && ((a.DurationStartDate <= schoolwideScheduleViewModel.StartDate.Date
                          && a.DurationEndDate >= schoolwideScheduleViewModel.EndDate.Date)
                          || ((a.DurationStartDate >= schoolwideScheduleViewModel.StartDate.Date
       && a.DurationStartDate <= schoolwideScheduleViewModel.EndDate.Date) || (a.DurationEndDate >= schoolwideScheduleViewModel.StartDate.Date
       && a.DurationEndDate <= schoolwideScheduleViewModel.EndDate.Date)))

                          && (a.CalDate == null || a.CalDate >= schoolwideScheduleViewModel.StartDate.Date
       && a.CalDate <= schoolwideScheduleViewModel.EndDate.Date)
                          );
                    if (data != null && data.Any())
                    {
                        List<DateAndDayNameRange> dateAndDay = new();
                        if (schoolwideScheduleViewModel.EndDate.Date > schoolwideScheduleViewModel.StartDate.Date)
                        {
                            var dateDayName = Enumerable.Range(0, 1 + (schoolwideScheduleViewModel.EndDate.Date - schoolwideScheduleViewModel.StartDate.Date).Days)
                               .Select(i => new DateAndDayNameRange
                               {
                                   Date = schoolwideScheduleViewModel.StartDate.Date.AddDays(i),
                                   DayName = schoolwideScheduleViewModel.StartDate.Date.AddDays(i).DayOfWeek
                               })
                               .ToList();

                            dateAndDay.AddRange(dateDayName);
                        }
                        List<DayWithCourse> DayWiseCourseList = new();
                        foreach (var item in dateAndDay)
                        {
                            DayWithCourse dayWithCourse = new();
                            dayWithCourse.DayName = item.DayName.ToString();
                            dayWithCourse.Date = item.Date;
                            dayWithCourse.IsHoliday = CalendarHolidayList.Where(i => i.StartDate <= item.Date && i.EndDate >= item.Date).Select(i => i.IsHoliday).FirstOrDefault();

                            var dayWiseCoureseData = data.Where(a => a.VarDay == dayWithCourse.DayName || a.CalDay == dayWithCourse.DayName || (a.FixedDays != null && a.FixedDays.Contains(dayWithCourse.DayName))).OrderBy(c => c.CourseId).ToList();
                            //*check input-date with course section end-date**/
                            dayWiseCoureseData = dayWiseCoureseData.Where(x => x.DurationEndDate >= item.Date).ToList();

                            var resultData = dayWiseCoureseData.GroupBy(g => new
                            {
                                g.CourseId,
                                g.CourseTitle
                            }).Select(a => new ScheduleCourseViewModel
                            {
                                CourseId = a.Key.CourseId,
                                CourseName = a.Key.CourseTitle,
                                StaffListModels = a.GroupBy(s => new { s.StaffId, staffName = $"{s.FirstGivenName} {(s.MiddleName == null ? "" : $"{s.MiddleName} ")}{s.LastFamilyName}" })
                                .Select(a => new ScheduleStaffDetails
                                {
                                    StaffId = a.Key.StaffId,
                                    StaffName = a.Key.staffName,
                                    CourseSectionListModels = a.Select(b => new ScheduleCourseSectionViewModel
                                    {
                                        CourSectionId = b.CourseSectionId,
                                        CourseSectionName = b.CourseSectionName,
                                        ScheduleType = b.ScheduleType,
                                        DurationStartDate = b.DurationStartDate,
                                        DurationEndDate = b.DurationEndDate,
                                        PeriodId = (b.FixedPeriodId != null) ? b.FixedPeriodId : (b.VarPeriodId != null) ? b.VarPeriodId : b.CalPeriodId,
                                    }).ToList()
                                }).ToList()
                            }).ToList();

                            dayWithCourse.CourseListModel = resultData;
                            DayWiseCourseList.Add(dayWithCourse);

                        }
                        schoolwideScheduleListModel.DayWithCourseList = DayWiseCourseList;
                        schoolwideScheduleListModel._failure = false;

                    }
                    else
                    {
                        schoolwideScheduleListModel._failure = true;
                        schoolwideScheduleListModel._message = NORECORDFOUND;
                    }
                }
                else
                {
                    schoolwideScheduleListModel._failure = true;
                    schoolwideScheduleListModel._message = "Date range are not match";
                }



            }
            catch (Exception _ex)
            {
                schoolwideScheduleListModel._failure = true;
                schoolwideScheduleListModel._message = _ex.Message.ToString();
            }



            ///final set
            schoolwideScheduleListModel.TenantId = schoolwideScheduleViewModel.TenantId;
            schoolwideScheduleListModel._token = schoolwideScheduleViewModel._token;

            return schoolwideScheduleListModel;
        }

        /// <summary>
        /// Get Print Schedule Report
        /// </summary>
        /// <param name="printScheduleReportViewModel"></param>
        /// <returns></returns>
        public PrintScheduleReportViewModel GetPrintScheduleReport(PrintScheduleReportViewModel printScheduleReportViewModel)
        {
            PrintScheduleReportViewModel printScheduleReport = new();
            List<int> staffCourseSectionIds = new List<int>();
            printScheduleReport._tenantName = printScheduleReportViewModel._tenantName;
            printScheduleReport._token = printScheduleReportViewModel._token;
            printScheduleReport.TenantId = printScheduleReportViewModel.TenantId;
            printScheduleReport.SchoolId = printScheduleReportViewModel.SchoolId;
            try
            {
                var schoolData = this.context?.SchoolMaster.Include(d => d.SchoolDetail).Include(x => x.Gradelevels).FirstOrDefault(x => x.TenantId == printScheduleReportViewModel.TenantId && x.SchoolId == printScheduleReportViewModel.SchoolId);

                var ScheduledData = this.context?.StudentCoursesectionSchedule.Include(x => x.StudentMaster).Join(this.context?.AllCourseSectionView, scs => new { scs.TenantId, scs.SchoolId, scs.CourseId, scs.CourseSectionId },
                       acsv => new { acsv.TenantId, acsv.SchoolId, acsv.CourseId, acsv.CourseSectionId }, (scs, acsv) => new { scs, acsv }).Where(a => a.scs.TenantId == printScheduleReportViewModel.TenantId && a.scs.SchoolId == printScheduleReportViewModel.SchoolId && a.acsv.TenantId == printScheduleReportViewModel.TenantId && a.acsv.SchoolId == printScheduleReportViewModel.SchoolId && a.acsv.AcademicYear == printScheduleReportViewModel.AcademicYear && printScheduleReportViewModel.StudentGuids.Contains(a.scs.StudentGuid) && (printScheduleReportViewModel.CourseSectionIds != null && printScheduleReportViewModel.CourseSectionIds.Length > 0 ? printScheduleReportViewModel.CourseSectionIds.Contains(a.scs.CourseSectionId) : ((a.acsv.DurationStartDate <= printScheduleReportViewModel.MarkingPeriodStartDate.Value.Date && a.acsv.DurationEndDate >= printScheduleReportViewModel.MarkingPeriodEndDate.Value.Date) || ((a.acsv.DurationStartDate >= printScheduleReportViewModel.MarkingPeriodStartDate.Value.Date && a.acsv.DurationStartDate <= printScheduleReportViewModel.MarkingPeriodEndDate.Value.Date) || (a.acsv.DurationEndDate >= printScheduleReportViewModel.MarkingPeriodStartDate.Value.Date && a.acsv.DurationEndDate <= printScheduleReportViewModel.MarkingPeriodEndDate.Value.Date))) && (a.acsv.CalDate == null || a.acsv.CalDate >= printScheduleReportViewModel.MarkingPeriodStartDate.Value.Date && a.acsv.CalDate <= printScheduleReportViewModel.MarkingPeriodEndDate.Value.Date))).ToList();

                var staffScheduleData = this.context?.StaffCoursesectionSchedule.Include(s => s.StaffMaster).Where(x => printScheduleReportViewModel.StaffId != null ? x.TenantId == printScheduleReportViewModel.TenantId && x.SchoolId == printScheduleReportViewModel.SchoolId && x.StaffId == printScheduleReportViewModel.StaffId && x.IsDropped != true : x.TenantId == printScheduleReportViewModel.TenantId && x.SchoolId == printScheduleReportViewModel.SchoolId && x.IsPrimaryStaff == true);

                if (staffScheduleData != null && printScheduleReportViewModel.StaffId != null) //For staff potral
                {
                    staffCourseSectionIds = staffScheduleData.Select(s => s.CourseSectionId).ToList();
                    ScheduledData = ScheduledData.Where(x => staffCourseSectionIds.Contains(x.scs.CourseSectionId)).ToList();
                }

                if (ScheduledData?.Any() == true)
                {
                    var roomData = this.context?.Rooms.Where(x => x.TenantId == printScheduleReportViewModel.TenantId && x.SchoolId == printScheduleReportViewModel.SchoolId && x.AcademicYear == printScheduleReportViewModel.AcademicYear);
                    var periodData = this.context?.BlockPeriod.Where(x => x.TenantId == printScheduleReportViewModel.TenantId && x.SchoolId == printScheduleReportViewModel.SchoolId && x.AcademicYear == printScheduleReportViewModel.AcademicYear);

                    foreach (var studentGuid in printScheduleReportViewModel.StudentGuids)
                    {
                        StudentDetailsViewModel studentDetails = new StudentDetailsViewModel();
                        var studentScheduleData = ScheduledData.Where(x => x.scs.StudentGuid == studentGuid);
                        if (studentScheduleData?.Any() == true)
                        {
                            var studentData = studentScheduleData.FirstOrDefault().scs;
                            studentDetails.StudentPhoto = studentData.StudentMaster.StudentThumbnailPhoto;
                            studentDetails.FirstGivenName = studentData.StudentMaster.FirstGivenName;
                            studentDetails.MiddleName = studentData.StudentMaster.MiddleName;
                            studentDetails.LastFamilyName = studentData.StudentMaster.LastFamilyName;
                            studentDetails.StudentInternalId = studentData.StudentMaster.StudentInternalId;
                            studentDetails.Dob = studentData.StudentMaster.Dob;
                            studentDetails.GradeLevelTitle = schoolData.Gradelevels.FirstOrDefault(d => d.GradeId == studentData.GradeId)?.Title;
                            studentDetails.Gender = studentData.StudentMaster.Gender;
                            studentDetails.HomeAddressLineOne = studentData.StudentMaster.HomeAddressLineOne;
                            studentDetails.HomeAddressLineTwo = studentData.StudentMaster.HomeAddressLineTwo;
                            studentDetails.HomeAddressCity = studentData.StudentMaster.HomeAddressCity;
                            studentDetails.HomeAddressState = studentData.StudentMaster.HomeAddressState;
                            studentDetails.HomeAddressCountry = studentData.StudentMaster.HomeAddressCountry;
                            studentDetails.HomeAddressZip = studentData.StudentMaster.HomeAddressZip;

                            var courseData = studentScheduleData.Select(s => s.acsv).GroupBy(g => new
                            {
                                g.CourseId,
                                g.CourseTitle
                            });

                            foreach (var course in courseData)
                            {
                                CourseDetailsViewModel courseDetails = new();

                                courseDetails.CourseId = course.Key.CourseId;
                                courseDetails.CourseName = course.Key.CourseTitle;
                                var courseSectionIds = course.Where(s => s.CourseId == course.Key.CourseId).Select(s => s.CourseSectionId).Distinct();

                                foreach (var courseSectionId in courseSectionIds)
                                {
                                    CourseSectionDetailsViewModel courseSectionDetails = new CourseSectionDetailsViewModel();
                                    string[] days = { };

                                    var courseSectionData = course.Where(x => x.CourseSectionId == courseSectionId);
                                    var courseSection = courseSectionData.FirstOrDefault();
                                    courseSectionDetails.CourseSectionId = courseSectionId;
                                    courseSectionDetails.CourseSectionName = courseSection.CourseSectionName;

                                    var staffData = staffScheduleData.FirstOrDefault(x => x.CourseSectionId == courseSectionId);
                                    if (staffData != null)
                                    {
                                        courseSectionDetails.StaffId = staffData.StaffId;
                                        courseSectionDetails.StaffName = $"{staffData.StaffMaster.FirstGivenName} {(staffData.StaffMaster.MiddleName == null ? "" : $"{staffData.StaffMaster.MiddleName} ")}{staffData.StaffMaster.LastFamilyName}";
                                    }

                                    var startDate = printScheduleReportViewModel.MarkingPeriodStartDate != null ? printScheduleReportViewModel.MarkingPeriodStartDate.Value.Date : courseSection.DurationStartDate.Value.Date;
                                    var endDate = printScheduleReportViewModel.MarkingPeriodEndDate != null ? printScheduleReportViewModel.MarkingPeriodEndDate.Value.Date : courseSection.DurationEndDate.Value.Date;

                                    if (courseSection.ScheduleType == "Fixed Schedule (1)")
                                    {
                                        days = courseSection.FixedDays.Split("|");

                                        foreach (var day in days)
                                        {
                                            DayDetailsViewModel dayDetails = new DayDetailsViewModel();

                                            dayDetails.DayName = day;

                                            var dateList = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
                                                                    .Select(offset => startDate.AddDays(offset))
                                                                    .Where(d => day == d.DayOfWeek.ToString())
                                                                    .ToList();

                                            dayDetails.DatePeriodRoomDetailsViewModelList.AddRange(dateList.Select(x => new DatePeriodRoomDetailsViewModel { Date = x.Date, PeriodId = courseSection.FixedPeriodId, PeriodName = periodData.FirstOrDefault(x => x.PeriodId == courseSection.FixedPeriodId).PeriodTitle, RoomId = courseSection.FixedRoomId, RoomName = roomData.FirstOrDefault(x => x.RoomId == courseSection.FixedRoomId).Title }));

                                            courseSectionDetails.DayDetailsViewModelList.Add(dayDetails);
                                        }
                                    }
                                    else if (courseSection.ScheduleType == "Variable Schedule (2)")
                                    {
                                        foreach (var cs in courseSectionData)
                                        {
                                            DayDetailsViewModel dayDetails = new DayDetailsViewModel();

                                            dayDetails.DayName = cs.VarDay;

                                            var dateList = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
                                                                    .Select(offset => startDate.AddDays(offset))
                                                                    .Where(d => cs.VarDay == d.DayOfWeek.ToString())
                                                                    .ToList();

                                            dayDetails.DatePeriodRoomDetailsViewModelList.AddRange(dateList.Select(x => new DatePeriodRoomDetailsViewModel { Date = x.Date, PeriodId = cs.VarPeriodId, PeriodName = periodData.FirstOrDefault(x => x.PeriodId == cs.VarPeriodId).PeriodTitle, RoomId = cs.VarRoomId, RoomName = roomData.FirstOrDefault(x => x.RoomId == cs.VarRoomId).Title }));
                                            courseSectionDetails.DayDetailsViewModelList.Add(dayDetails);
                                        }
                                    }
                                    else if (courseSection.ScheduleType == "Calendar Schedule (3)")
                                    {
                                        courseSectionDetails.DayDetailsViewModelList = courseSectionData.GroupBy(x => x.CalDay).Select(s => new DayDetailsViewModel { DayName = s.Key, DatePeriodRoomDetailsViewModelList = s.Select(cs => new DatePeriodRoomDetailsViewModel { Date = cs.CalDate.Value.Date, PeriodId = cs.CalPeriodId, PeriodName = periodData.FirstOrDefault(x => x.PeriodId == cs.CalPeriodId).PeriodTitle, RoomId = cs.CalRoomId, RoomName = roomData.FirstOrDefault(x => x.RoomId == cs.CalRoomId).Title }).ToList() }).ToList();
                                    }
                                    else if (courseSection.ScheduleType == "Block Schedule (4)")
                                    {
                                        var blockData = this.context?.Block.Where(x => x.TenantId == printScheduleReportViewModel.TenantId && x.SchoolId == printScheduleReportViewModel.SchoolId && x.AcademicYear == printScheduleReportViewModel.AcademicYear);
                                        var bellScheduleData = this.context?.BellSchedule.Where(x => x.TenantId == printScheduleReportViewModel.TenantId && x.SchoolId == printScheduleReportViewModel.SchoolId && x.AcademicYear == printScheduleReportViewModel.AcademicYear && x.BellScheduleDate >= startDate && x.BellScheduleDate <= endDate);

                                        var blocks = courseSectionData.GroupBy(x => x.BlockId);
                                        foreach (var block in blocks)
                                        {
                                            DayDetailsViewModel dayDetails = new DayDetailsViewModel();

                                            dayDetails.DayName = blockData.FirstOrDefault(s => s.BlockId == block.Key)?.BlockTitle;
                                            var dateList = bellScheduleData.Where(s => s.BlockId == block.Key).Select(s => s.BellScheduleDate).ToList();
                                            var blockDetails = block.Select(s => new { s.BlockPeriodId, s.BlockRoomId });
                                            foreach (var cs in blockDetails)
                                            {
                                                dayDetails.DatePeriodRoomDetailsViewModelList.AddRange(dateList.Select(x => new DatePeriodRoomDetailsViewModel { Date = x.Date, PeriodId = cs.BlockPeriodId, PeriodName = periodData.FirstOrDefault(x => x.BlockId == block.Key && x.PeriodId == cs.BlockPeriodId).PeriodTitle, RoomId = cs.BlockRoomId, RoomName = roomData.FirstOrDefault(x => x.RoomId == cs.BlockRoomId).Title }));
                                            }
                                            courseSectionDetails.DayDetailsViewModelList.Add(dayDetails);
                                        }
                                    }
                                    courseDetails.CourseSectionDetailsViewModelList.Add(courseSectionDetails);
                                }
                                studentDetails.CourseDetailsViewModelList.Add(courseDetails);
                            }
                            printScheduleReport.StudentDetailsViewModelList.Add(studentDetails);
                        }
                    }

                    //school details
                    printScheduleReport.SchoolName = schoolData.SchoolName;
                    printScheduleReport.SchoolLogo = schoolData.SchoolDetail.First().SchoolThumbnailLogo;
                    printScheduleReport.StreetAddress1 = schoolData.StreetAddress1;
                    printScheduleReport.StreetAddress2 = schoolData.StreetAddress2;
                    printScheduleReport.State = schoolData.State;
                    printScheduleReport.District = schoolData.District;
                    printScheduleReport.City = schoolData.City;
                    printScheduleReport.Country = schoolData.Country;
                    printScheduleReport.Zip = schoolData.Zip;

                }
                else
                {
                    printScheduleReport._message = NORECORDFOUND;
                    printScheduleReport._failure = true;
                }
            }
            catch (Exception es)
            {
                printScheduleReport._message = es.Message;
                printScheduleReport._failure = true;
            }
            return printScheduleReport;
        }
    }
}

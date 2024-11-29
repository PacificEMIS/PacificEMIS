using opensis.data.Models;
using opensis.data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.data.ViewModels.ScheduleReport
{
    public class ScheduleClassList: CommonFields
    {
        public ScheduleClassList()
        {
            CourseSectionViewList = new List<CourseSectionForStaff>();
        }
        public List<CourseSectionForStaff> CourseSectionViewList { get; set; }
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int? CourseId { get; set; }
        public string? CourseSubject { get; set; }
        public string? CourseProgram { get; set; }
        public int? BlockPeriodId { get; set; }
        public int? StaffId { get; set; }
        public DateTime? MarkingPeriodStartDate { get; set; }
        public DateTime? MarkingPeriodEndDate { get; set; }
    }

    public class CourseSectionForStaff {
        public List<StudentListForCoursection>? StudentLists { get; set; }
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int CourseId { get; set; }
        public string? CourseTitle { get; set; }
        public string? CourseSubject { get; set; }
        public string? CourseProgram { get; set; }
        public int CourseSectionId { get; set; }
        public string? CourseSectionName { get; set; }
        public string? StaffName { get; set; }
        public int? ScheduledStudentCount { get; set; }
        public int? AvailableSeat { get; set; }
        public int? TotalSeats { get; set; }
    }

    public class StudentListForCoursection 
    {
        public StudentViewForCoursection StudentView { get; set; }
        public List<FieldsCategory>? FieldsCategoryList { get; set; }
    }
    public class StudentViewForCoursection
    {
       
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int StudentId { get; set; }
        public string? AlternateId { get; set; }
        public string? DistrictId { get; set; }
        public string? StateId { get; set; }
        public string? AdmissionNumber { get; set; }
        public string? RollNumber { get; set; }
        public string? Salutation { get; set; }
        public string? FirstGivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastFamilyName { get; set; }
        public string? Suffix { get; set; }
        public string? PreferredName { get; set; }
        public string? PreviousName { get; set; }
        public string? SocialSecurityNumber { get; set; }
        public string? OtherGovtIssuedNumber { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }
        public string? Race { get; set; }
        public string? Ethnicity { get; set; }
        public string? MaritalStatus { get; set; }
        public string? CountryOfBirth { get; set; }
        public string? Nationality { get; set; }
        public string? FirstLanguage { get; set; }
        public string? SecondLanguage { get; set; }
        public string? ThirdLanguage { get; set; }
        public string? HomePhone { get; set; }
        public string? MobilePhone { get; set; }
        public string? PersonalEmail { get; set; }
        public string? SchoolEmail { get; set; }
        public string? Twitter { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Youtube { get; set; }
        public string? Linkedin { get; set; }
        public string? HomeAddressLineOne { get; set; }
        public string? HomeAddressLineTwo { get; set; }
        public string? HomeAddressCity { get; set; }
        public string? HomeAddressState { get; set; }
        public string? HomeAddressZip { get; set; }
        public string? BusNo { get; set; }
        public bool? SchoolBusPickUp { get; set; }
        public bool? SchoolBusDropOff { get; set; }
        public bool? MailingAddressSameToHome { get; set; }
        public string? MailingAddressLineOne { get; set; }
        public string? MailingAddressLineTwo { get; set; }
        public string? MailingAddressCity { get; set; }
        public string? MailingAddressState { get; set; }
        public string? MailingAddressZip { get; set; }
        public string? HomeAddressCountry { get; set; }
        public string? MailingAddressCountry { get; set; }
        public int? SectionId { get; set; }
        public string? SectionName { get; set; }
        public string? StudentInternalId { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public string? EnrollmentType { get; set; }
        public bool? IsActive { get; set; }
        public Guid StudentGuid { get; set; }
        public int EnrollmentId { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public string? EnrollmentCode { get; set; }
        public int? CalenderId { get; set; }
        public int? GradeId { get; set; }
        public string? GradeLevelTitle { get; set; }
        public string? RollingOption { get; set; }
        public string? SchoolName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public string? StudentPortalId { get; set; }
        public bool? Eligibility504 { get; set; }
        public bool? EconomicDisadvantage { get; set; }
        public bool? FreeLunchEligibility { get; set; }
        public bool? SpecialEducationIndicator { get; set; }
        public bool? LepIndicator { get; set; }
        public DateTime? EstimatedGradDate { get; set; }
    }
}

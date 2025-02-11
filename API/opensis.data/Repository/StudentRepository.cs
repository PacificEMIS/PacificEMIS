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

using JSReport;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.Student;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace opensis.data.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public StudentRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Add Student
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public StudentAddViewModel AddStudent(StudentAddViewModel student)
        {
            if(student.studentMaster is null)
            {
                student._failure = true;
                return student;
            }
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    var checkMessage = Utility.checkDuplicate(this.context, student.studentMaster.TenantId, student.studentMaster.SchoolId, student.studentMaster.Salutation, student.studentMaster.FirstGivenName, student.studentMaster.MiddleName, student.studentMaster.LastFamilyName, student.studentMaster.Suffix, student.studentMaster.Dob, student.studentMaster.PersonalEmail, student.studentMaster.SocialSecurityNumber, "student", null);

                    if (checkMessage != null && student.allowDuplicate != true)
                    {
                        student._failure = true;
                        student.checkDuplicate = checkMessage;
                        student._message = "Duplicate Entry Found";
                        return student;
                    }
                    else
                    {
                        //int? MasterStudentId = Utility.GetMaxPK(this.context, new Func<StudentMaster, int>(x => x.StudentId));
                        int? MasterStudentId = 1;

                        var studentData = this.context?.StudentMaster.Where(x => x.SchoolId == student.studentMaster.SchoolId && x.TenantId == student.studentMaster.TenantId).OrderByDescending(x => x.StudentId).FirstOrDefault();

                        if (studentData != null)
                        {
                            MasterStudentId = studentData.StudentId + 1;
                        }

                        student.studentMaster.StudentId = (int)MasterStudentId;
                        Guid GuidId = Guid.NewGuid();
                        var GuidIdExist = this.context?.StudentMaster.FirstOrDefault(x => x.StudentGuid == GuidId);
                        if (GuidIdExist != null)
                        {
                            student._failure = true;
                            student._message = "Guid is already exist, Please try again.";
                            return student;
                        }
                        student.studentMaster.StudentGuid = GuidId;
                        student.studentMaster.IsActive = true;
                        student.studentMaster.EnrollmentType = "Internal";
                        student.studentMaster.CreatedOn = DateTime.UtcNow;

                        if (!string.IsNullOrEmpty(student.studentMaster.StudentInternalId))
                        {
                            bool checkInternalID = CheckInternalID(student.studentMaster.TenantId, student.studentMaster.StudentInternalId, student.studentMaster.SchoolId);
                            if (checkInternalID == false)
                            {
                                student.studentMaster = null;
                                student.fieldsCategoryList = new();
                                student._failure = true;
                                student._message = "Student ID Already Exist";
                                return student;
                            }
                        }
                        else
                        {
                            student.studentMaster.StudentInternalId = MasterStudentId.ToString();
                        }

                        var schoolName = this.context?.SchoolMaster.Where(x => x.TenantId == student.studentMaster.TenantId && x.SchoolId == student.studentMaster.SchoolId).Select(s => s.SchoolName).FirstOrDefault();

                        //Insert data into Enrollment table
                        int? calenderId = null;
                        string? enrollmentCode = null;

                        var defaultCalender = this.context?.SchoolCalendars.FirstOrDefault(x => x.TenantId == student.studentMaster.TenantId && x.SchoolId == student.studentMaster.SchoolId && x.AcademicYear.ToString() == student.AcademicYear && x.DefaultCalender == true);

                        if (defaultCalender != null)
                        {
                            calenderId = defaultCalender.CalenderId;
                        }

                        var enrollmentType = this.context?.StudentEnrollmentCode.FirstOrDefault(x => x.TenantId == student.studentMaster.TenantId && x.SchoolId == student.studentMaster.SchoolId && x.Type.ToLower() == "Add".ToLower());

                        if (enrollmentType != null)
                        {
                            enrollmentCode = enrollmentType.Title;
                        }

                        var gradeLevel = this.context?.Gradelevels.Where(x => x.SchoolId == student.studentMaster.SchoolId).OrderBy(x => x.GradeId).FirstOrDefault();

                        int? gradeId = null;
                        if (gradeLevel != null)
                        {
                            gradeId = gradeLevel.GradeId;
                        }

                        var schoolYearStartDate = this.context?.SchoolYears.Where(x => x.TenantId == student.studentMaster.TenantId && x.SchoolId == student.studentMaster.SchoolId && x.AcademicYear.ToString() == student.AcademicYear).Min(s=>s.StartDate);

                        var StudentEnrollmentData = new StudentEnrollment() { TenantId = student.studentMaster.TenantId, SchoolId = student.studentMaster.SchoolId, StudentId = student.studentMaster.StudentId, EnrollmentId = 1, SchoolName = schoolName, RollingOption = "Next grade at current school", EnrollmentCode = enrollmentCode, CalenderId = calenderId, GradeLevelTitle = gradeLevel?.Title, EnrollmentDate = schoolYearStartDate ?? DateTime.UtcNow, StudentGuid = GuidId, IsActive = true, GradeId = gradeId, CreatedBy = student.studentMaster.CreatedBy, CreatedOn = DateTime.UtcNow };

                        //Add student portal access
                        if (!string.IsNullOrWhiteSpace(student.PasswordHash) && !string.IsNullOrWhiteSpace(student.LoginEmail))
                        {
                            UserMaster userMaster = new UserMaster();

                            var decrypted = Utility.Decrypt(student.PasswordHash);
                            string passwordHash = Utility.GetHashedPassword(decrypted);

                            var loginInfo = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == student.studentMaster.TenantId && x.EmailAddress == student.LoginEmail);

                            if (loginInfo == null)
                            {
                                var membership = this.context?.Membership.FirstOrDefault(x => x.TenantId == student.studentMaster.TenantId && x.SchoolId == student.studentMaster.SchoolId && x.Profile == "Student");

                                userMaster.SchoolId = student.studentMaster.SchoolId;
                                userMaster.TenantId = student.studentMaster.TenantId;
                                userMaster.UserId = student.studentMaster.StudentId;
                                userMaster.LangId = 1;
                                userMaster.MembershipId = membership!=null?membership.MembershipId:0;
                                userMaster.EmailAddress = student.LoginEmail;
                                userMaster.PasswordHash = passwordHash;
                                userMaster.Name = student.studentMaster.FirstGivenName ?? "";
                                userMaster.IsActive = student.PortalAccess;
                                userMaster.CreatedBy = student.studentMaster.CreatedBy;
                                userMaster.CreatedOn = DateTime.UtcNow;
                                student.studentMaster.StudentPortalId = student.LoginEmail;
                                this.context?.UserMaster.Add(userMaster);
                                this.context?.SaveChanges();
                            }
                            else
                            {
                                student.studentMaster = null;
                                student.fieldsCategoryList = new();
                                student._failure = true;
                                student._message = "Student Login Email Already Exist";
                                return student;
                            }
                        }

                        this.context?.StudentMaster.Add(student.studentMaster);
                        this.context?.StudentEnrollment.Add(StudentEnrollmentData);
                        this.context?.SaveChanges();

                        if (student.fieldsCategoryList != null && student.fieldsCategoryList.ToList()?.Any()==true)
                        {
                            var fieldsCategory = student.fieldsCategoryList.FirstOrDefault(x => x.CategoryId == student.SelectedCategoryId);
                            if (fieldsCategory != null)
                            {
                                foreach (var customFields in fieldsCategory.CustomFields.ToList())
                                {
                                    if (customFields.CustomFieldsValue != null && customFields.CustomFieldsValue.ToList().Any())
                                    {
                                        var dataExits = this.context?.CustomFieldsValue.Any(x => x.TenantId == student.studentMaster.TenantId && x.SchoolId == student.studentMaster.SchoolId && x.CategoryId == customFields.CategoryId && x.FieldId == customFields.FieldId && x.Module == "Student" && x.TargetId == student.studentMaster.StudentId);

                                        if (dataExits != true)
                                        {
                                            customFields.CustomFieldsValue.FirstOrDefault()!.Module = "Student";
                                            customFields.CustomFieldsValue.FirstOrDefault()!.CategoryId = customFields.CategoryId;
                                            customFields.CustomFieldsValue.FirstOrDefault()!.FieldId = customFields.FieldId;
                                            customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldTitle = customFields.Title;
                                            customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldType = customFields.Type;
                                            customFields.CustomFieldsValue.FirstOrDefault()!.SchoolId = student.studentMaster.SchoolId;
                                            customFields.CustomFieldsValue.FirstOrDefault()!.TenantId = student.studentMaster.TenantId;
                                            customFields.CustomFieldsValue.FirstOrDefault()!.TargetId = student.studentMaster.StudentId;
                                            customFields.CustomFieldsValue.FirstOrDefault()!.CreatedBy = student.studentMaster.CreatedBy;
                                            customFields.CustomFieldsValue.FirstOrDefault()!.UpdatedBy = student.studentMaster.CreatedBy;
                                            customFields.CustomFieldsValue.FirstOrDefault()!.CreatedOn = DateTime.UtcNow;
                                            customFields.CustomFieldsValue.FirstOrDefault()!.UpdateOn = DateTime.UtcNow;
                                            this.context?.CustomFieldsValue.AddRange(customFields.CustomFieldsValue);
                                            this.context?.SaveChanges();
                                        }
                                    }
                                }

                            }
                        }
                        student._failure = false;
                        student._message = "Student added successfully";
                        transaction?.Commit();
                    }
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    student._failure = true;
                    student._message = es.Message;
                }
            }
            return student;
        }

        //Checking Internal Id
        private bool CheckInternalID(Guid TenantId, string InternalID, int SchoolId)
        {
            if (InternalID != null && InternalID != "")
            {
                var checkInternalId = this.context?.StudentMaster.Where(x => x.TenantId == TenantId && x.SchoolId == SchoolId && x.StudentInternalId == InternalID).ToList();
                if (checkInternalId?.Any()==true)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Update Student
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public StudentAddViewModel UpdateStudent(StudentAddViewModel student)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    var checkMessage = Utility.checkDuplicate(this.context, student.studentMaster!.TenantId, student.studentMaster.SchoolId, student.studentMaster.Salutation, student.studentMaster.FirstGivenName, student.studentMaster.MiddleName, student.studentMaster.LastFamilyName, student.studentMaster.Suffix, student.studentMaster.Dob, student.studentMaster.PersonalEmail, student.studentMaster.SocialSecurityNumber, "student", student.studentMaster.StudentGuid);

                    if (checkMessage != null && student.allowDuplicate != true)
                    {
                        student._failure = true;
                        student._message = "Duplicate Entry Found";
                        student.checkDuplicate = checkMessage;
                        return student;
                    }

                    else
                    {
                        var checkInternalId = this.context?.StudentMaster.Where(x => x.TenantId == student.studentMaster.TenantId && x.SchoolId == student.studentMaster.SchoolId && x.StudentInternalId == student.studentMaster.StudentInternalId && x.StudentInternalId != null && x.StudentGuid != student.studentMaster.StudentGuid).ToList();
                        if (checkInternalId?.Any()==true)
                        {
                            student.studentMaster = null;
                            student.fieldsCategoryList = new();
                            student._failure = true;
                            student._message = "Student ID Already Exist";
                        }
                        else
                        {
                            var studentUpdate = this.context?.StudentMaster.FirstOrDefault(x => x.TenantId == student.studentMaster.TenantId && x.SchoolId == student.studentMaster.SchoolId && x.StudentId == student.studentMaster.StudentId);
                            if (studentUpdate != null)
                            {
                                if (string.IsNullOrEmpty(student.studentMaster.StudentInternalId))
                                {
                                    student.studentMaster.StudentInternalId = studentUpdate.StudentInternalId;
                                }

                                //Add or Update student portal access
                                if (studentUpdate.StudentPortalId != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(student.LoginEmail))
                                    {
                                        if (studentUpdate.StudentPortalId != student.LoginEmail)
                                        {
                                            var loginInfo = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == student.studentMaster.TenantId && x.EmailAddress == student.LoginEmail);

                                            if (loginInfo != null)
                                            {
                                                student.studentMaster = null;
                                                student.fieldsCategoryList = new();
                                                student._failure = true;
                                                student._message = "Student Login Email Already Exist";
                                                return student;
                                            }
                                            else
                                            {
                                                var loginInfoData = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == student.studentMaster.TenantId && x.EmailAddress == studentUpdate.StudentPortalId);

                                                if (loginInfoData != null)
                                                {
                                                    var loginInfoDataForRemove = loginInfoData;

                                                    if (loginInfoDataForRemove != null)
                                                    {
                                                        this.context?.UserMaster.Remove(loginInfoDataForRemove);
                                                        this.context?.SaveChanges();
                                                    }

                                                    loginInfoData.EmailAddress = student.LoginEmail;
                                                    loginInfoData.IsActive = student.PortalAccess;
                                                    loginInfoData.UpdatedBy = student.studentMaster.UpdatedBy;
                                                    loginInfoData.UpdatedOn = DateTime.UtcNow;
                                                    this.context?.UserMaster.Add(loginInfoData);
                                                    this.context?.SaveChanges();

                                                    //Update StudentPortalId in Studentmaster table
                                                    student.studentMaster.StudentPortalId = student.LoginEmail;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var loginInfo = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == student.studentMaster.TenantId && x.EmailAddress == studentUpdate.StudentPortalId);

                                            if (loginInfo != null)
                                            {
                                                loginInfo.IsActive = student.PortalAccess;
                                                loginInfo.UpdatedBy = student.studentMaster.UpdatedBy;
                                                loginInfo.UpdatedOn = DateTime.UtcNow;
                                            }

                                            this.context?.SaveChanges();

                                            //Keep existing StudentPortalId in Studentmaster table
                                            student.studentMaster.StudentPortalId = student.LoginEmail;
                                        }
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(student.LoginEmail) && !string.IsNullOrWhiteSpace(student.PasswordHash))
                                    {
                                        var decrypted = Utility.Decrypt(student.PasswordHash);
                                        string passwordHash = Utility.GetHashedPassword(decrypted);

                                        UserMaster userMaster = new UserMaster();

                                        var loginInfo = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == student.studentMaster.TenantId && x.EmailAddress == student.LoginEmail);

                                        if (loginInfo == null)
                                        {
                                            var membership = this.context?.Membership.FirstOrDefault(x => x.TenantId == student.studentMaster.TenantId && x.SchoolId == student.studentMaster.SchoolId && x.Profile == "Student");

                                            userMaster.SchoolId = student.studentMaster.SchoolId;
                                            userMaster.TenantId = student.studentMaster.TenantId;
                                            userMaster.UserId = student.studentMaster.StudentId;
                                            userMaster.LangId = 1;
                                            userMaster.MembershipId = membership != null ? membership.MembershipId : 0;
                                            userMaster.EmailAddress = student.LoginEmail;
                                            userMaster.PasswordHash = passwordHash;
                                            userMaster.Name = student.studentMaster.FirstGivenName ?? "";
                                            userMaster.CreatedOn = DateTime.UtcNow;
                                            userMaster.CreatedBy = student.studentMaster.UpdatedBy;
                                            userMaster.IsActive = student.PortalAccess;

                                            this.context?.UserMaster.Add(userMaster);
                                            this.context?.SaveChanges();


                                            //Update StudentPortalId in Studentmaster table.
                                            //studentUpdate.StudentPortalId = student.LoginEmail;
                                            student.studentMaster.StudentPortalId = student.LoginEmail;
                                        }
                                        else
                                        {
                                            student.studentMaster = null;
                                            student.fieldsCategoryList = new();
                                            student._failure = true;
                                            student._message = "Student Login Email Already Exist";
                                            return student;
                                        }
                                    }
                                }

                                student.studentMaster.Associationship = studentUpdate.Associationship;
                                student.studentMaster.EnrollmentType = studentUpdate.EnrollmentType;
                                student.studentMaster.IsActive = studentUpdate.IsActive;
                                student.studentMaster.StudentGuid = studentUpdate.StudentGuid;
                                student.studentMaster.CreatedBy = studentUpdate.CreatedBy;
                                student.studentMaster.CreatedOn = studentUpdate.CreatedOn;
                                student.studentMaster.UpdatedOn = DateTime.UtcNow;
                                this.context?.Entry(studentUpdate).CurrentValues.SetValues(student.studentMaster);
                                this.context?.SaveChanges();

                            }

                            //this.context?.SaveChanges();

                            if (student.fieldsCategoryList != null && student.fieldsCategoryList.ToList()?.Any()==true)
                            {
                                var fieldsCategory = student.fieldsCategoryList.FirstOrDefault(x => x.CategoryId == student.SelectedCategoryId);
                                if (fieldsCategory != null)
                                {
                                    foreach (var customFields in fieldsCategory.CustomFields.ToList())
                                    {
                                        var customFieldValueData = this.context?.CustomFieldsValue.FirstOrDefault(x => x.TenantId == student.studentMaster.TenantId && x.SchoolId == student.studentMaster.SchoolId && x.CategoryId == customFields.CategoryId && x.FieldId == customFields.FieldId && x.Module == "Student" && x.TargetId == student.studentMaster.StudentId);
                                        if (customFieldValueData != null)
                                        {
                                            this.context?.CustomFieldsValue.RemoveRange(customFieldValueData);
                                        }
                                        if (customFields.CustomFieldsValue != null && customFields.CustomFieldsValue.ToList().Any())
                                        {
                                            customFields.CustomFieldsValue.FirstOrDefault()!.Module = "Student";
                                            customFields.CustomFieldsValue.FirstOrDefault()!.CategoryId = customFields.CategoryId;
                                            customFields.CustomFieldsValue.FirstOrDefault()!.FieldId = customFields.FieldId;
                                            customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldTitle = customFields.Title;
                                            customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldType = customFields.Type;
                                            customFields.CustomFieldsValue.FirstOrDefault()!.SchoolId = student.studentMaster.SchoolId;
                                            customFields.CustomFieldsValue.FirstOrDefault()!.TenantId = student.studentMaster.TenantId;
                                            customFields.CustomFieldsValue.FirstOrDefault()!.TargetId = student.studentMaster.StudentId;
                                            customFields.CustomFieldsValue.FirstOrDefault()!.CreatedBy = student.studentMaster.UpdatedBy;
                                            customFields.CustomFieldsValue.FirstOrDefault()!.UpdatedBy = student.studentMaster.UpdatedBy;
                                            customFields.CustomFieldsValue.FirstOrDefault()!.CreatedOn = DateTime.UtcNow;
                                            customFields.CustomFieldsValue.FirstOrDefault()!.UpdateOn = DateTime.UtcNow;
                                            this.context?.CustomFieldsValue.AddRange(customFields.CustomFieldsValue);
                                            this.context?.SaveChanges();
                                        }
                                    }
                                }
                            }

                            student._failure = false;
                            student._message = "Student Updated Successfully";
                        }
                        transaction?.Commit();
                    }
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    student.studentMaster = null;
                    student._failure = true;
                    student._message = ex.Message;

                }
            }
            return student;

        }

        /// <summary>
        /// Get All Student With Pagination,sorting,searching
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public StudentListModel GetAllStudentList(PageResult pageResult)
        {
            StudentListModel studentListModel = new StudentListModel();
            IQueryable<StudentListView>? transactionIQ = null;
            IQueryable<StudentListView>? studentDataList = null;
            int? totalCount = 0;
            IEnumerable<int?>? schoolAttachedId = null;

            var membershipData = this.context?.UserMaster.Include(x => x.Membership).FirstOrDefault(x => x.TenantId == pageResult.TenantId && x.EmailAddress == pageResult.EmailAddress);

            var activeSchools = this.context?.SchoolDetail.Where(x => x.Status == true).Select(x => x.SchoolId).ToList();

            if (membershipData != null)
            {
                //if (membershipData.Membership.ProfileType.ToLower() == "super administrator")
                if (String.Compare(membershipData.Membership.ProfileType, "super administrator", true) == 0)
                {
                    studentDataList = this.context?.StudentListView.Where(x => x.TenantId == pageResult.TenantId && (pageResult.SearchAllSchool == false || pageResult.SearchAllSchool == null ? x.SchoolId == pageResult.SchoolId : activeSchools!.Contains(x.SchoolId)) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.IsActive != false : true));
                }
                else
                {
                    schoolAttachedId = this.context?.StaffSchoolInfo.Where(x => x.TenantId == pageResult.TenantId && x.StaffId == membershipData.UserId && x.EndDate == null && activeSchools!.Contains(x.SchoolAttachedId)).ToList().Select(s => s.SchoolAttachedId);

                    studentDataList = this.context?.StudentListView.Where(x => x.TenantId == pageResult.TenantId && (pageResult.SearchAllSchool == false || pageResult.SearchAllSchool == null ? x.SchoolId == pageResult.SchoolId : schoolAttachedId!.Contains(x.SchoolId)) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.IsActive != false : true));

                }
            }

            //var studentDataList = this.context?.StudentListView.Where(x => x.TenantId == pageResult.TenantId && (pageResult.SearchAllSchool == false || pageResult.SearchAllSchool == null ? x.SchoolId == pageResult.SchoolId : true) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.IsActive != false : true));

            try
            {
                if (studentDataList?.Any() == true)
                {
                    if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                    {
                        transactionIQ = studentDataList;
                    }
                    else
                    {
                        string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                        var ColumnvalueForName = Regex.Replace(Columnvalue, @"\s+", "");
                        if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                        {
                            transactionIQ = studentDataList.Where(x => x.FirstGivenName != null && x.FirstGivenName.ToLower().Contains(ColumnvalueForName.ToLower()) || x.MiddleName != null && x.MiddleName.ToLower().Contains(ColumnvalueForName.ToLower()) || x.LastFamilyName != null && x.LastFamilyName.ToLower().Contains(ColumnvalueForName.ToLower()) || ((x.FirstGivenName ?? "").ToLower() + (x.MiddleName ?? "").ToLower() + (x.LastFamilyName ?? "").ToLower()).Contains(ColumnvalueForName.ToLower()) || ((x.FirstGivenName ?? "").ToLower() + (x.MiddleName ?? "").ToLower()).Contains(ColumnvalueForName.ToLower()) || ((x.FirstGivenName ?? "").ToLower() + (x.LastFamilyName ?? "").ToLower()).Contains(ColumnvalueForName.ToLower()) || ((x.MiddleName ?? "").ToLower() + (x.LastFamilyName ?? "").ToLower()).Contains(ColumnvalueForName.ToLower()) || x.StudentInternalId != null && x.StudentInternalId.ToLower().Contains(Columnvalue.ToLower()) || x.AlternateId != null && x.AlternateId.ToLower().Contains(Columnvalue.ToLower()) || x.HomePhone != null && x.HomePhone.ToLower().Contains(Columnvalue.ToLower()) || x.MobilePhone != null && x.MobilePhone.ToLower().Contains(Columnvalue.ToLower()) || x.PersonalEmail != null && x.PersonalEmail.ToLower().Contains(Columnvalue.ToLower()) || x.SchoolEmail != null && x.SchoolEmail.ToLower().Contains(Columnvalue.ToLower()) || x.GradeLevelTitle != null && x.GradeLevelTitle.ToLower().Contains(Columnvalue.ToLower()) || x.SectionName != null && x.SectionName.ToLower().Contains(Columnvalue.ToLower()) || x.Gender != null && x.Gender.ToLower().Contains(Columnvalue.ToLower()));
                        }
                        else
                        {
                            //normal advance search
                            transactionIQ = Utility.FilteredData(pageResult.FilterParams!, studentDataList).AsQueryable();

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
                    }

                    if (pageResult.DobStartDate != null && pageResult.DobEndDate != null)
                    {
                        var filterInDateRange = transactionIQ?.Where(x => x.Dob >= pageResult.DobStartDate && x.Dob <= pageResult.DobEndDate);
                        if (filterInDateRange?.Any() == true)
                        {
                            transactionIQ = filterInDateRange;
                        }
                        else
                        {
                            transactionIQ = null;
                        }
                    }

                    if (pageResult.FullName != null)
                    {
                        var studentName = pageResult.FullName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                        if (studentName.Length > 1)
                        {
                            var firstName = studentName.First();
                            var lastName = studentName.Last();
                            pageResult.FullName = null;

                            if (pageResult.FullName == null)
                            {
                                var nameSearch = transactionIQ?.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && (x.FirstGivenName ?? "").StartsWith(firstName.ToString()) && (x.LastFamilyName ?? "").StartsWith(lastName.ToString()));

                                transactionIQ = nameSearch;
                            }
                        }
                        else
                        {
                            var nameSearch = transactionIQ?.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && ((x.FirstGivenName ?? "").StartsWith(pageResult.FullName) || (x.LastFamilyName ?? "").StartsWith(pageResult.FullName))).AsQueryable();

                            transactionIQ = nameSearch;
                        }
                    }

                    if (transactionIQ != null)
                    {
                        if (pageResult.SortingModel != null)
                        {
                            transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn ?? "", (pageResult.SortingModel.SortDirection ?? "").ToLower());
                        }
                        else
                        {
                            transactionIQ = transactionIQ.OrderBy(s => s.LastFamilyName).ThenBy(c => c.FirstGivenName);
                        }
                    }

                    totalCount = transactionIQ != null ? transactionIQ?.Count() : 0;


                    if (totalCount > 0)
                    {
                        if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                        {
                            transactionIQ = transactionIQ!.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                        }

                        studentListModel.studentListViews = transactionIQ!.ToList();

                        studentListModel.studentListViews.ForEach(c =>
                        {
                            c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, c.CreatedBy);
                            c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, c.UpdatedBy);
                        });

                        studentListModel.TotalCount = totalCount;
                        studentListModel._message = "success";
                        studentListModel._failure = false;
                    }
                    else
                    {
                        studentListModel._message = NORECORDFOUND;
                        studentListModel._failure = true;
                    }

                    studentListModel.TenantId = pageResult.TenantId;
                    studentListModel.SchoolId = pageResult.SchoolId;
                    studentListModel.PageNumber = pageResult.PageNumber;
                    studentListModel._pageSize = pageResult.PageSize;
                    studentListModel._tenantName = pageResult._tenantName;
                    studentListModel._token = pageResult._token;
                }
            }
            catch (Exception es)
            {
                studentListModel._message = es.Message;
                studentListModel._failure = true;
            }
            return studentListModel;
        }

        //public StudentListModel GetAllStudentList(PageResult pageResult)
        //{

        //    StudentListModel studentListModel = new StudentListModel();
        //    IQueryable<StudentMaster> transactionIQ = null;


        //    //var studentDataList = this.context?.StudentMaster.Include(x => x.StudentEnrollment).Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.IsActive != false);

        //    var studentDataList = this.context?.StudentMaster.Include(x => x.StudentEnrollment).Include(x => x.Sections).Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.IsActive!=false : true)).AsNoTracking().Select(e => new StudentMaster
        //    {
        //        TenantId = e.TenantId,
        //        SchoolId = e.SchoolId,
        //        StudentId = e.StudentId,
        //        FirstGivenName = e.FirstGivenName,
        //        MiddleName = e.MiddleName,
        //        LastFamilyName = e.LastFamilyName,
        //        AlternateId = e.AlternateId,
        //        StudentInternalId = e.StudentInternalId,
        //        MobilePhone = e.MobilePhone,
        //        HomePhone = e.HomePhone,
        //        PersonalEmail = e.PersonalEmail,
        //        SchoolEmail = e.SchoolEmail,
        //        StudentGuid = e.StudentGuid,
        //        AdmissionNumber=e.AdmissionNumber,
        //        RollNumber=e.RollNumber,
        //        Dob=e.Dob,
        //        Gender=e.Gender,
        //        Race=e.Race,
        //        Ethnicity=e.Ethnicity,
        //        MaritalStatus=e.MaritalStatus,
        //        CountryOfBirth=e.CountryOfBirth,
        //        Nationality=e.Nationality,
        //        FirstLanguage=e.FirstLanguage,
        //        SecondLanguage=e.SecondLanguage,
        //        ThirdLanguage=e.ThirdLanguage,
        //        HomeAddressLineOne = e.HomeAddressLineOne,
        //        HomeAddressLineTwo = e.HomeAddressLineTwo,
        //        HomeAddressCity = e.HomeAddressCity,
        //        HomeAddressCountry = e.HomeAddressCountry,
        //        HomeAddressState = e.HomeAddressState,
        //        HomeAddressZip = e.HomeAddressZip,
        //        BusNo=e.BusNo,
        //        FirstLanguageId=e.FirstLanguageId,
        //        SecondLanguageId=e.SecondLanguageId,
        //        ThirdLanguageId=e.ThirdLanguageId,
        //        SectionId=e.SectionId,
        //        IsActive=e.IsActive,
        //        Sections = e.Sections,
        //        StudentEnrollment = e.StudentEnrollment.Where(d => d.IsActive == true).Select(s => new StudentEnrollment
        //        {
        //            EnrollmentDate = s.EnrollmentDate,
        //            GradeLevelTitle = s.GradeLevelTitle,
        //            TenantId = s.TenantId,
        //            SchoolId = s.SchoolId,
        //            StudentId = s.StudentId,
        //            EnrollmentId = s.EnrollmentId, 
        //            StudentGuid = s.StudentGuid,
        //            GradeId=s.GradeId,
        //        }).ToList()
        //    });

        //    try
        //    {
        //        if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
        //        {
        //            transactionIQ = studentDataList;
        //        }
        //        else
        //        {
        //            string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
        //            if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
        //            {
        //                transactionIQ = studentDataList.Where(x => x.FirstGivenName != null && x.FirstGivenName.ToLower().Contains(Columnvalue.ToLower()) ||
        //                                                            x.MiddleName != null && x.MiddleName.ToLower().Contains(Columnvalue.ToLower()) ||
        //                                                            x.LastFamilyName != null && x.LastFamilyName.ToLower().Contains(Columnvalue.ToLower()) ||
        //                                                            x.StudentInternalId != null && x.StudentInternalId.ToLower().Contains(Columnvalue.ToLower()) ||
        //                                                            x.AlternateId != null && x.AlternateId.Contains(Columnvalue) ||
        //                                                            x.HomePhone != null && x.HomePhone.Contains(Columnvalue) ||
        //                                                            x.MobilePhone != null && x.MobilePhone.Contains(Columnvalue) ||
        //                                                            x.PersonalEmail != null && x.PersonalEmail.Contains(Columnvalue) ||
        //                                                            x.SchoolEmail != null && x.SchoolEmail.Contains(Columnvalue));

        //                //for GradeLevel Searching
        //                var gradeLevelFilter = studentDataList.AsNoTracking().ToList().Where(x => x.StudentEnrollment.ToList()?.Any()==true ? x.StudentEnrollment.FirstOrDefault().GradeLevelTitle.ToLower().Contains(Columnvalue.ToLower()) : string.Empty.Contains(Columnvalue)).AsQueryable();

        //                if (gradeLevelFilter.ToList()?.Any()==true)
        //                {
        //                    transactionIQ = transactionIQ.AsNoTracking().ToList().Concat(gradeLevelFilter).AsQueryable();

        //                }

        //                //searching for section name
        //                var sectionId = this.context?.Sections.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.Name.ToLower().Contains(Columnvalue.ToLower())).Select(x => x.SectionId).ToList();

        //                if (sectionId.ToList()?.Any()==true)
        //                {
        //                    var sectionSearchData = studentDataList.Where(x => x.SchoolId == pageResult.SchoolId && sectionId.Contains((int)x.SectionId));

        //                    if (sectionSearchData.ToList()?.Any()==true)
        //                    {
        //                        transactionIQ = transactionIQ.AsNoTracking().ToList().Concat(sectionSearchData).AsQueryable();
        //                        transactionIQ = transactionIQ.GroupBy(x => x.StudentId).Select(g => g.First());
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (pageResult.FilterParams.Any(x => x.ColumnName.ToLower() == "gradeid"))
        //                {
        //                    var filterData = pageResult.FilterParams.Where(x => x.ColumnName.ToLower() == "gradeid").ToList();
        //                    if (filterData?.Any()==true)
        //                    {
        //                        var filterValues = filterData.Select(x => x.FilterValue).ToList();

        //                        var gradeLevelData = studentDataList.AsNoTracking().ToList().Where(x =>filterValues.Contains(x.StudentEnrollment.FirstOrDefault().GradeId.ToString())).AsQueryable();

        //                        if (gradeLevelData.ToList()?.Any()==true)
        //                        {
        //                            transactionIQ = gradeLevelData.AsNoTracking().ToList().AsQueryable();
        //                            var indexValue = pageResult.FilterParams.FindIndex(x => x.ColumnName.ToLower() == "gradeid");
        //                            pageResult.FilterParams.RemoveAt(indexValue);
        //                            if (pageResult.FilterParams?.Any()==true)
        //                            {
        //                                transactionIQ = Utility.FilteredData(pageResult.FilterParams, transactionIQ).AsQueryable();
        //                            }
        //                        }
        //                    }
        //                    //else
        //                    //{
        //                    //    var filterValue = Convert.ToInt32(pageResult.FilterParams.Where(x => x.ColumnName.ToLower() == "gradeid").Select(x => x.FilterValue).FirstOrDefault());

        //                    //    var gradeLevelData = studentDataList.AsNoTracking().ToList().Where(x => x.StudentEnrollment.FirstOrDefault().GradeId == filterValue).AsQueryable();

        //                    //    if (gradeLevelData.ToList()?.Any()==true)
        //                    //    {
        //                    //        transactionIQ = gradeLevelData.AsNoTracking().ToList().AsQueryable();
        //                    //        var indexValue = pageResult.FilterParams.FindIndex(x => x.ColumnName.ToLower() == "gradeid");
        //                    //        pageResult.FilterParams.RemoveAt(indexValue);
        //                    //        if (pageResult.FilterParams?.Any()==true)
        //                    //        {
        //                    //            transactionIQ = Utility.FilteredData(pageResult.FilterParams, transactionIQ).AsQueryable();
        //                    //        }
        //                    //    }
        //                    //}
        //                }
        //                else
        //                {
        //                    transactionIQ = Utility.FilteredData(pageResult.FilterParams, studentDataList).AsQueryable();
        //                }
        //            }  
        //        }

        //        if (pageResult.DobStartDate != null && pageResult.DobEndDate != null)
        //        {
        //            var filterInDateRange = transactionIQ.AsNoTracking().ToList().Where(x => x.Dob >= pageResult.DobStartDate && x.Dob <= pageResult.DobEndDate).AsQueryable();
        //            if (filterInDateRange.ToList()?.Any()==true)
        //            {
        //                transactionIQ = filterInDateRange;
        //            }
        //            else
        //            {
        //                transactionIQ = null;
        //            }
        //        }

        //        if (pageResult.FullName != null)
        //        {
        //            var studentName = pageResult.FullName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        //            if (studentName.Length > 1)
        //            {
        //                var firstName = studentName.First();
        //                var lastName = studentName.Last();
        //                pageResult.FullName = null;

        //                if (pageResult.FullName == null)
        //                {
        //                    var nameSearch = transactionIQ.AsNoTracking().ToList().Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.FirstGivenName.StartsWith(firstName.ToString()) && x.LastFamilyName.StartsWith(lastName.ToString())).AsQueryable();

        //                    transactionIQ = nameSearch;
        //                }
        //            }
        //            else
        //            {
        //                var nameSearch = transactionIQ.AsNoTracking().ToList().Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && (x.FirstGivenName.StartsWith(pageResult.FullName) || x.LastFamilyName.StartsWith(pageResult.FullName))).AsQueryable();

        //                transactionIQ = nameSearch;
        //            }
        //        }

        //        if (pageResult.SortingModel != null)
        //        {
        //            switch (pageResult.SortingModel.SortColumn.ToLower())
        //            {
        //                //For GradeLevel Sorting
        //                case "gradeleveltitle":

        //                    if (pageResult.SortingModel.SortDirection.ToLower() == "asc")
        //                    {

        //                        transactionIQ = transactionIQ.AsNoTracking().ToList().OrderBy(a => a.StudentEnrollment?.Any()==true ? a.StudentEnrollment.FirstOrDefault().GradeLevelTitle : null).AsQueryable();
        //                    }
        //                    else
        //                    {

        //                        transactionIQ = transactionIQ.AsNoTracking().ToList().OrderByDescending(a => a.StudentEnrollment?.Any()==true ? a.StudentEnrollment.FirstOrDefault().GradeLevelTitle : null).AsQueryable();
        //                    }
        //                    break;

        //                default:
        //                    transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn, pageResult.SortingModel.SortDirection.ToLower());
        //                    break;
        //            }

        //        }

        //        if (transactionIQ != null)
        //        {
        //            int? totalCount = transactionIQ.AsNoTracking().Count();
        //            if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
        //            {
        //                transactionIQ = transactionIQ.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
        //            }
        //            studentListModel.studentMaster = transactionIQ.ToList();
        //            studentListModel.TotalCount = totalCount;
        //        }
        //        else
        //        {
        //            studentListModel.TotalCount = 0;
        //        }

        //        studentListModel.TenantId = pageResult.TenantId;
        //        studentListModel.SchoolId = pageResult.SchoolId;
        //        studentListModel.PageNumber = pageResult.PageNumber;
        //        studentListModel._pageSize = pageResult.PageSize;
        //        studentListModel._tenantName = pageResult._tenantName;
        //        studentListModel._token = pageResult._token;
        //        studentListModel._failure = false;
        //    }
        //    catch (Exception es)
        //    {
        //        studentListModel._message = es.Message;
        //        studentListModel._failure = true;
        //        studentListModel._tenantName = pageResult._tenantName;
        //        studentListModel._token = pageResult._token;
        //    }
        //    return studentListModel;
        //}

        /// <summary>
        /// Add Student Document
        /// </summary>
        /// <param name="studentDocumentAddViewModel"></param>
        /// <returns></returns>
        public StudentDocumentAddViewModel AddStudentDocument(StudentDocumentAddViewModel studentDocumentAddViewModel)
        {
            try
            {
                int? MasterDocumentId = 0;

                if (studentDocumentAddViewModel.studentDocuments != null && studentDocumentAddViewModel.studentDocuments.ToList()?.Any()==true)
                {
                    MasterDocumentId = Utility.GetMaxPK(this.context, new Func<StudentDocuments, int>(x => x.DocumentId));

                    foreach (var studentDocument in studentDocumentAddViewModel.studentDocuments.ToList())
                    {
                        studentDocument.DocumentId = (int)MasterDocumentId!;
                        studentDocument.UploadedOn = DateTime.UtcNow;
                        studentDocument.CreatedOn = DateTime.UtcNow;
                        this.context?.StudentDocuments.Add(studentDocument);
                        MasterDocumentId++;
                    }

                    this.context?.SaveChanges();
                }

                studentDocumentAddViewModel._failure = false;
                studentDocumentAddViewModel._message = "Student Document added successfully";
            }
            catch (Exception es)
            {
                studentDocumentAddViewModel._failure = true;
                studentDocumentAddViewModel._message = es.Message;
            }
            return studentDocumentAddViewModel;
        }

        /// <summary>
        /// Update Student Document
        /// </summary>
        /// <param name="studentDocumentAddViewModel"></param>
        /// <returns></returns>
        public StudentDocumentAddViewModel UpdateStudentDocument(StudentDocumentAddViewModel studentDocumentAddViewModel)
        { 
          
            try
            {
                var data = studentDocumentAddViewModel.studentDocuments.FirstOrDefault();
                if (data != null) { 
                var studentDocumentUpdate = this.context?.StudentDocuments.FirstOrDefault(x => x.TenantId == data.TenantId && x.SchoolId == data.SchoolId && x.StudentId == data.StudentId && x.DocumentId == data.DocumentId);
                if (studentDocumentUpdate != null)
                {
                    data.UploadedOn = DateTime.UtcNow;
                    data.UpdatedOn = DateTime.UtcNow;
                    data.CreatedBy = studentDocumentUpdate.CreatedBy;
                    data.CreatedOn = studentDocumentUpdate.CreatedOn;
                    this.context?.Entry(studentDocumentUpdate).CurrentValues.SetValues(data);
                    this.context?.SaveChanges();
                    studentDocumentAddViewModel._failure = false;
                    studentDocumentAddViewModel._message = "Student Document Updated Successfully";
                    }
                    else
                    {
                        studentDocumentAddViewModel._failure = true;
                        studentDocumentAddViewModel._message = "Student Document Not Updated";
                    }
                }
            }
            catch (Exception es)
            {
                studentDocumentAddViewModel._failure = true;
                studentDocumentAddViewModel._message = es.Message;
            }
            return studentDocumentAddViewModel;
        }

        /// <summary>
        /// Get All Student Documents List
        /// </summary>
        /// <param name="studentDocumentListViewModel"></param>
        /// <returns></returns>
        public StudentDocumentListViewModel GetAllStudentDocumentsList(StudentDocumentListViewModel studentDocumentListViewModel)
        {
            StudentDocumentListViewModel studentDocumentsList = new StudentDocumentListViewModel();
            try
            {
                var StudentDocumentsAll = this.context?.StudentDocuments.Where(x => x.TenantId == studentDocumentListViewModel.TenantId && x.SchoolId == studentDocumentListViewModel.SchoolId && x.StudentId == studentDocumentListViewModel.StudentId).OrderByDescending(x=>x.DocumentId).ToList();

                if (StudentDocumentsAll?.Any()==true)
                {
                    studentDocumentsList._failure = false;
                    StudentDocumentsAll.ForEach(c =>
                    {
                        c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, studentDocumentListViewModel.TenantId, c.CreatedBy);
                        c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, studentDocumentListViewModel.TenantId, c.UpdatedBy);
                        c.UploadedBy = Utility.CreatedOrUpdatedBy(this.context, studentDocumentListViewModel.TenantId, c.UploadedBy); ;
                    });
                    studentDocumentsList.studentDocumentsList = StudentDocumentsAll;
                }
                else
                {
                    studentDocumentsList._failure = true;
                    studentDocumentsList._message = NORECORDFOUND;
                }

                
                studentDocumentsList._tenantName = studentDocumentListViewModel._tenantName;
                studentDocumentsList._token = studentDocumentListViewModel._token;
                studentDocumentsList.TenantId = studentDocumentListViewModel.TenantId;
                studentDocumentsList.SchoolId = studentDocumentListViewModel.SchoolId;
                studentDocumentsList.StudentId = studentDocumentListViewModel.StudentId;
            }
            catch (Exception es)
            {
                studentDocumentsList._message = es.Message;
                studentDocumentsList._failure = true;
                studentDocumentsList._tenantName = studentDocumentListViewModel._tenantName;
                studentDocumentsList._token = studentDocumentListViewModel._token;
            }
            return studentDocumentsList;
        }

        /// <summary>
        /// Delete Student Document
        /// </summary>
        /// <param name="studentDocumentAddViewModel"></param>
        /// <returns></returns>
        public StudentDocumentAddViewModel DeleteStudentDocument(StudentDocumentAddViewModel studentDocumentAddViewModel)
        {
            try
            {
                var data = studentDocumentAddViewModel.studentDocuments.FirstOrDefault();
                if (data != null)
                {
                    var studentDocumentDelete = this.context?.StudentDocuments.FirstOrDefault(x => x.TenantId == data.TenantId && x.SchoolId == data.SchoolId && x.StudentId == data.StudentId && x.DocumentId == data.DocumentId);
                    if (studentDocumentDelete != null)
                    {
                        this.context?.StudentDocuments.Remove(studentDocumentDelete);
                        this.context?.SaveChanges();
                        studentDocumentAddViewModel._failure = false;
                        studentDocumentAddViewModel._message = "Student Document deleted successfullyy";
                    }
                    else
                    {
                        studentDocumentAddViewModel._failure = true;
                        studentDocumentAddViewModel._message = "Student Document Not Deleted";
                    }
                    
                }
            }
            catch (Exception es)
            {
                studentDocumentAddViewModel._failure = true;
                studentDocumentAddViewModel._message = es.Message;
            }
            return studentDocumentAddViewModel;
        }

        /// <summary>
        /// Add Student Login Info
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public LoginInfoAddModel AddStudentLoginInfo(LoginInfoAddModel login)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(login.userMaster?.PasswordHash) && !string.IsNullOrWhiteSpace(login.userMaster.EmailAddress))
                {
                    var decrypted = Utility.Decrypt(login.userMaster.PasswordHash);
                    string passwordHash = Utility.GetHashedPassword(decrypted);

                    var loginInfo = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == login.userMaster.TenantId && x.SchoolId == login.userMaster.SchoolId && x.EmailAddress == login.userMaster.EmailAddress);

                    if (loginInfo == null)
                    {
                        var membership = this.context?.Membership.FirstOrDefault(x => x.TenantId == login.userMaster.TenantId && x.SchoolId == login.userMaster.SchoolId && x.Profile == "Student");

                        login.userMaster.UserId = login.StudentId;
                        login.userMaster.LangId = 1;
                        login.userMaster.MembershipId = membership!=null?membership.MembershipId:0;
                        login.userMaster.PasswordHash = passwordHash;
                        login.userMaster.UpdatedOn = DateTime.UtcNow;
                        login.userMaster.IsActive = true;

                        if (login.userMaster.UserSecretQuestions != null)
                        {
                            login.userMaster.UserSecretQuestions.UserId = login.StudentId;
                            login.userMaster.UserSecretQuestions.UpdatedOn = DateTime.UtcNow;
                        }

                        this.context?.UserMaster.Add(login.userMaster);
                        this.context?.SaveChanges();

                        //Update StudentPortalId in Studentmaster table.
                        var student = this.context?.StudentMaster.FirstOrDefault(x => x.TenantId == login.userMaster.TenantId && x.SchoolId == login.userMaster.SchoolId && x.StudentId == login.StudentId);
                        if (student != null)
                        {
                            student.StudentPortalId = login.userMaster.EmailAddress;

                            this.context?.SaveChanges();
                        }
                        
                    }
                }
                login._failure = false;
                login._message = "Student Login Info Added Succesfully";
            }
            catch (Exception es)
            {
                login._failure = true;
                login._message = es.Message;
            }

            return login;
        }

        /// <summary>
        /// Add Student Comment
        /// </summary>
        /// <param name="studentCommentAddViewModel"></param>
        /// <returns></returns>
        public StudentCommentAddViewModel AddStudentComment(StudentCommentAddViewModel studentCommentAddViewModel)
        {
            try
            {
                int? MasterCommentId = Utility.GetMaxPK(this.context, new Func<StudentComments, int>(x => x.CommentId));
                studentCommentAddViewModel.studentComments!.CommentId = (int)MasterCommentId!;
                studentCommentAddViewModel.studentComments.CreatedOn = DateTime.UtcNow;
                this.context?.StudentComments.Add(studentCommentAddViewModel.studentComments);
                this.context?.SaveChanges();
                studentCommentAddViewModel._failure = false;
                studentCommentAddViewModel._message = "Student Comment added successfully";
            }
            catch (Exception es)
            {
                studentCommentAddViewModel._failure = true;
                studentCommentAddViewModel._message = es.Message;
            }
            return studentCommentAddViewModel;
        }

        /// <summary>
        /// Update Student Comment
        /// </summary>
        /// <param name="studentCommentAddViewModel"></param>
        /// <returns></returns>
        public StudentCommentAddViewModel UpdateStudentComment(StudentCommentAddViewModel studentCommentAddViewModel)
        {
            try
            {
                var studentCommentUpdate = this.context?.StudentComments.FirstOrDefault(x => x.TenantId == studentCommentAddViewModel.studentComments!.TenantId && x.SchoolId == studentCommentAddViewModel.studentComments.SchoolId && x.StudentId == studentCommentAddViewModel.studentComments.StudentId && x.CommentId == studentCommentAddViewModel.studentComments.CommentId);

                if (studentCommentUpdate != null)
                {
                    studentCommentAddViewModel.studentComments!.UpdatedOn = DateTime.UtcNow;
                    studentCommentAddViewModel.studentComments.CreatedOn = studentCommentUpdate.CreatedOn;
                    studentCommentAddViewModel.studentComments.CreatedBy = studentCommentUpdate.CreatedBy;
                    this.context?.Entry(studentCommentUpdate).CurrentValues.SetValues(studentCommentAddViewModel.studentComments);
                    this.context?.SaveChanges();
                    studentCommentAddViewModel._failure = false;
                    studentCommentAddViewModel._message = "Student Comment Updated Successfully";
                }
                
            }
            catch (Exception es)
            {
                studentCommentAddViewModel._failure = true;
                studentCommentAddViewModel._message = es.Message;
            }
            return studentCommentAddViewModel;
        }

        /// <summary>
        /// Get All Student Comments List
        /// </summary>
        /// <param name="studentCommentListViewModel"></param>
        /// <returns></returns>
        public StudentCommentListViewModel GetAllStudentCommentsList(StudentCommentListViewModel studentCommentListViewModel)
        {
            StudentCommentListViewModel studentCommentsList = new StudentCommentListViewModel();
            try
            {
                var StudentCommentsAll = this.context?.StudentComments.Where(x => x.TenantId == studentCommentListViewModel.TenantId && x.SchoolId == studentCommentListViewModel.SchoolId && x.StudentId == studentCommentListViewModel.StudentId).OrderByDescending(x => x.CommentId).ToList();

                studentCommentsList._tenantName = studentCommentListViewModel._tenantName;
                studentCommentsList.TenantId = studentCommentListViewModel.TenantId;
                studentCommentsList.SchoolId = studentCommentListViewModel.SchoolId;
                studentCommentsList.StudentId = studentCommentListViewModel.StudentId;
                studentCommentsList._token = studentCommentListViewModel._token;

                if (StudentCommentsAll?.Any()==true)
                {
                    studentCommentsList._failure = false;
                    StudentCommentsAll.ForEach(c =>
                    {
                        c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, studentCommentListViewModel.TenantId, c.CreatedBy);
                        c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, studentCommentListViewModel.TenantId, c.UpdatedBy);
                    });
                    studentCommentsList.studentCommentsList = StudentCommentsAll;
                }
                else
                {
                    studentCommentsList._failure = true;
                    studentCommentsList._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                studentCommentsList._message = es.Message;
                studentCommentsList._failure = true;
                studentCommentsList._tenantName = studentCommentListViewModel._tenantName;
                studentCommentsList._token = studentCommentListViewModel._token;
            }
            return studentCommentsList;
        }

        /// <summary>
        /// Delete Student Comment
        /// </summary>
        /// <param name="studentCommentAddViewModel"></param>
        /// <returns></returns>
        public StudentCommentAddViewModel DeleteStudentComment(StudentCommentAddViewModel studentCommentAddViewModel)
        {
            try
            {
                var studentCommentDelete = this.context?.StudentComments.FirstOrDefault(x => x.TenantId == studentCommentAddViewModel.studentComments!.TenantId && x.SchoolId == studentCommentAddViewModel.studentComments.SchoolId && x.StudentId == studentCommentAddViewModel.studentComments.StudentId && x.CommentId == studentCommentAddViewModel.studentComments.CommentId);
                if (studentCommentDelete != null)
                {
                    this.context?.StudentComments.Remove(studentCommentDelete);
                    this.context?.SaveChanges();
                    studentCommentAddViewModel._failure = false;
                    studentCommentAddViewModel._message = "Student Comment deleted successfullyy";
                }
                
            }
            catch (Exception es)
            {
                studentCommentAddViewModel._failure = true;
                studentCommentAddViewModel._message = es.Message;
            }
            return studentCommentAddViewModel;
        }


        /// <summary>
        /// Add Student Enrollment
        /// </summary>
        /// <param name="studentEnrollmentAddView"></param>
        /// <returns></returns>
        public StudentEnrollmentListModel AddStudentEnrollment(StudentEnrollmentListModel studentEnrollmentListModel)
        {
            try
            {
                int? EnrollmentId = null;
                EnrollmentId = Utility.GetMaxPK(this.context, new Func<StudentEnrollment, int>(x => x.EnrollmentId));
                foreach (var studentEnrollment in studentEnrollmentListModel.studentEnrollments)
                {

                    studentEnrollment.EnrollmentId = (int)EnrollmentId!;
                    studentEnrollment.CalenderId = studentEnrollmentListModel.CalenderId;
                    studentEnrollment.RollingOption = studentEnrollmentListModel.RollingOption;
                    studentEnrollment.CreatedOn = DateTime.UtcNow;
                    this.context?.StudentEnrollment.AddRange(studentEnrollment);
                    EnrollmentId++;
                }
                this.context?.SaveChanges();
                studentEnrollmentListModel._failure = false;
                studentEnrollmentListModel._message = "Student Enrollment added successfully";
            }
            catch (Exception es)
            {
                studentEnrollmentListModel._failure = true;
                studentEnrollmentListModel._message = es.Message;
            }

            return studentEnrollmentListModel;
        }

        /// <summary>
        /// Update Student Enrollment
        /// </summary>
        /// <param name="studentEnrollmentListModel"></param>
        /// <returns></returns>
        public StudentEnrollmentListModel UpdateStudentEnrollment(StudentEnrollmentListModel studentEnrollmentListModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    List<ParentAssociationship> parentAssociationshipOldList = new List<ParentAssociationship>();
                    List<ParentAssociationship> parentAssociationshipNewList = new List<ParentAssociationship>();

                    var studentMasterData = this.context?.StudentMaster.Where(x => x.TenantId == studentEnrollmentListModel.TenantId && x.SchoolId == studentEnrollmentListModel.SchoolId && x.StudentGuid == studentEnrollmentListModel.StudentGuid).FirstOrDefault();
                    if (studentMasterData != null)
                    {
                        studentMasterData.SectionId = studentEnrollmentListModel.SectionId;
                        studentMasterData.EstimatedGradDate = studentEnrollmentListModel.EstimatedGradDate;
                        studentMasterData.Eligibility504 = studentEnrollmentListModel.Eligibility504;
                        studentMasterData.EconomicDisadvantage = studentEnrollmentListModel.EconomicDisadvantage;
                        studentMasterData.FreeLunchEligibility = studentEnrollmentListModel.FreeLunchEligibility;
                        studentMasterData.SpecialEducationIndicator = studentEnrollmentListModel.SpecialEducationIndicator;
                        studentMasterData.LepIndicator = studentEnrollmentListModel.LepIndicator;
                        this.context?.SaveChanges();
                    }

                    //Add CustomField Value for StudentEnrollment Category
                    if (studentEnrollmentListModel.fieldsCategoryList != null && studentEnrollmentListModel.fieldsCategoryList.ToList()?.Any() == true)
                    {
                        var fieldsCategory = studentEnrollmentListModel.fieldsCategoryList.FirstOrDefault(x => x.CategoryId == studentEnrollmentListModel.SelectedCategoryId);
                        if (fieldsCategory != null)
                        {
                            foreach (var customFields in fieldsCategory.CustomFields.ToList())
                            {
                                var customFieldValueData = this.context?.CustomFieldsValue.FirstOrDefault(x => x.TenantId == studentEnrollmentListModel.TenantId && x.SchoolId == studentEnrollmentListModel.SchoolId && x.CategoryId == customFields.CategoryId && x.FieldId == customFields.FieldId && x.Module == "Student" && x.TargetId == studentEnrollmentListModel.StudentId);
                                if (customFieldValueData != null)
                                {
                                    this.context?.CustomFieldsValue.RemoveRange(customFieldValueData);
                                }
                                if (customFields.CustomFieldsValue != null && customFields.CustomFieldsValue.ToList()?.Any() == true)
                                {
                                    var customFieldsValueData = customFields.CustomFieldsValue.FirstOrDefault();
                                    if (customFieldsValueData != null)
                                    {
                                        customFieldsValueData.Module = "Student";
                                        customFieldsValueData.CategoryId = customFields.CategoryId;
                                        customFieldsValueData.FieldId = customFields.FieldId;
                                        customFieldsValueData.CustomFieldTitle = customFields.Title;
                                        customFieldsValueData.CustomFieldType = customFields.Type;
                                        customFieldsValueData.SchoolId = studentEnrollmentListModel.SchoolId;
                                        customFieldsValueData.TenantId = (Guid)studentEnrollmentListModel.TenantId;
                                        customFieldsValueData.TargetId = studentEnrollmentListModel.StudentId;
                                        this.context?.CustomFieldsValue.AddRange(customFieldsValueData);
                                        this.context?.SaveChanges();
                                    }

                                }
                            }
                        }
                    }

                    //for insert in job fetch max id
                    long? Id = 1;
                    var dataExits = this.context?.ScheduledJobs.Where(x => x.TenantId == studentEnrollmentListModel.TenantId);
                    if (dataExits?.Any() == true)
                    {
                        var scheduledJobData = this.context?.ScheduledJobs.Where(x => x.TenantId == studentEnrollmentListModel.TenantId).Max(x => x.JobId);
                        if (scheduledJobData != null)
                        {
                            Id = scheduledJobData + 1;
                        }
                    }

                    //Enrollment Info Add & Update
                    int? EnrollmentId = 1;

                    var studentEnrollmentData = this.context?.StudentEnrollment.Where(x => x.StudentGuid == studentEnrollmentListModel.StudentGuid).OrderByDescending(x => x.EnrollmentId).FirstOrDefault();

                    if (studentEnrollmentData != null)
                    {
                        EnrollmentId = studentEnrollmentData.EnrollmentId + 1;
                    }

                    foreach (var studentEnrollmentList in studentEnrollmentListModel.studentEnrollments)
                    {
                        //Update Existing Enrollment Data
                        if (studentEnrollmentList.EnrollmentId > 0)
                        {
                            var studentEnrollmentUpdate = this.context?.StudentEnrollment.FirstOrDefault(x => x.TenantId == studentEnrollmentList.TenantId && x.SchoolId == studentEnrollmentList.SchoolId && x.StudentId == studentEnrollmentList.StudentId && x.EnrollmentId == studentEnrollmentList.EnrollmentId);
                            if (studentEnrollmentUpdate != null)
                            {
                                StudentEnrollment studentEnrollment = new StudentEnrollment();
                                if (studentEnrollmentList.ExitCode != null)
                                {
                                    //This block for Roll Over,Drop (Transfer),Enroll (Transfer)
                                    var studentExitCode = this.context?.StudentEnrollmentCode.FirstOrDefault(x => x.TenantId == studentEnrollmentList.TenantId && x.SchoolId == studentEnrollmentList.SchoolId && x.EnrollmentCode.ToString() == studentEnrollmentList.ExitCode); //fetching enrollemnt code type 

                                    if (studentExitCode != null && studentExitCode.Type!.ToLower() == "Drop (Transfer)".ToLower())
                                    {
                                        //This block for student drop(transfer) & enroll(transfer) new school

                                        if (studentEnrollmentList.ExitDate != null
                                               && studentEnrollmentList.ExitDate.Value.Date >= DateTime.UtcNow.Date)
                                        {
                                            //insert job if date today or in future
                                            var scheduledJob = new ScheduledJob
                                            {
                                                TenantId = studentEnrollmentList.TenantId,
                                                SchoolId = studentEnrollmentList.SchoolId,
                                                JobId = (long)Id,
                                                AcademicYear = studentEnrollmentList.AcademicYear,
                                                JobTitle = "StudentEnrollmentDropTransferStudent",
                                                JobScheduleDate = studentEnrollmentList.ExitDate!.Value.AddDays(1),
                                                ApiTitle = "UpdateStudentEnrollment",
                                                ControllerPath = studentEnrollmentListModel._tenantName + "/Student",
                                                TaskJson = JsonConvert.SerializeObject(studentEnrollmentListModel),
                                                LastRunStatus = null,
                                                LastRunTime = null,
                                                IsActive = true,
                                                CreatedBy = studentEnrollmentList.UpdatedBy,
                                                CreatedOn = DateTime.UtcNow
                                            };
                                            this.context?.ScheduledJobs.Add(scheduledJob);
                                            Id++;
                                            this.context?.SaveChanges();
                                        }

                                        //fetching enrollment code where student enroll(transfer).
                                        var studentTransferIn = this.context?.StudentEnrollmentCode.FirstOrDefault(x => x.TenantId == studentEnrollmentList.TenantId && x.SchoolId == studentEnrollmentList.TransferredSchoolId && x.Type.ToLower() == "Enroll (Transfer)".ToLower());

                                        if (studentTransferIn != null)
                                        {
                                            //update student's existing enrollment details 
                                            var studentEnrollmentCode = this.context?.StudentEnrollmentCode.FirstOrDefault(x => x.TenantId == studentEnrollmentList.TenantId && x.SchoolId == studentEnrollmentList.SchoolId && x.EnrollmentCode.ToString() == studentEnrollmentList.EnrollmentCode);

                                            studentEnrollmentUpdate.EnrollmentCode = studentEnrollmentCode?.Title;
                                            studentEnrollmentUpdate.EnrollmentDate = studentEnrollmentList.EnrollmentDate;
                                            studentEnrollmentUpdate.ExitCode = studentExitCode.Title;
                                            studentEnrollmentUpdate.ExitDate = studentEnrollmentList.ExitDate;
                                            studentEnrollmentUpdate.ExitReason = studentEnrollmentList.ExitReason;
                                            studentEnrollmentUpdate.TransferredGrade = studentEnrollmentList.TransferredGrade;
                                            studentEnrollmentUpdate.TransferredSchoolId = studentEnrollmentList.TransferredSchoolId;
                                            studentEnrollmentUpdate.SchoolTransferred = studentEnrollmentList.SchoolTransferred;
                                            studentEnrollmentUpdate.UpdatedOn = DateTime.UtcNow;
                                            studentEnrollmentUpdate.UpdatedBy = studentEnrollmentList.UpdatedBy;
                                            //studentEnrollmentUpdate.IsActive = studentEnrollmentList.ExitDate != null && studentEnrollmentList.ExitDate.Value.Date < DateTime.UtcNow.Date ? false : true;
                                            studentEnrollmentUpdate.IsActive = false;

                                            //fetching student details from studentMaster table for the new school if exist previously
                                            var checkStudentAlreadyExistInTransferredSchool = this.context?.StudentMaster.FirstOrDefault(x => x.TenantId == studentEnrollmentList.TenantId && x.SchoolId == studentEnrollmentList.TransferredSchoolId && x.StudentGuid == studentEnrollmentListModel.StudentGuid);

                                            if (checkStudentAlreadyExistInTransferredSchool != null)
                                            {
                                                //fetching student details from studentMaster table
                                                var studentData = this.context?.StudentMaster.FirstOrDefault(x => x.TenantId == studentEnrollmentListModel.TenantId && x.SchoolId == studentEnrollmentListModel.SchoolId && x.StudentId == studentEnrollmentListModel.StudentId);

                                                if (studentEnrollmentList.ExitDate != null
                                               && studentEnrollmentList.ExitDate.Value.Date < DateTime.UtcNow.Date) //drop student in previous date
                                                {
                                                    var studentOldSchool = this.context?.StudentMaster.Where(x => x.TenantId == studentEnrollmentListModel.TenantId && x.SchoolId != studentEnrollmentList.TransferredSchoolId && x.StudentGuid == studentEnrollmentListModel.StudentGuid).ToList();
                                                    if (studentOldSchool?.Any() == true)
                                                    {
                                                        studentOldSchool.ForEach(x => x.IsActive = false);

                                                        //this foreach for drop student from his scheduled course section
                                                        foreach (var studentSchool in studentOldSchool)
                                                        {
                                                            var studentCourseSection = context?.StudentCoursesectionSchedule.Where(x => x.SchoolId == studentSchool.SchoolId && x.TenantId == studentSchool.TenantId && x.StudentId == studentSchool.StudentId && x.IsDropped != true).ToList();
                                                            if (studentCourseSection?.Any() == true)
                                                            {
                                                                studentCourseSection.ForEach(s => { s.IsDropped = true; s.EffectiveDropDate = studentEnrollmentList.ExitDate; });
                                                            }
                                                        }
                                                    }
                                                    checkStudentAlreadyExistInTransferredSchool.IsActive = true;
                                                }

                                                //fetching all student's active details from studentMaster table
                                                /*var otherSchoolEnrollment = this.context?.StudentMaster.Where(x => x.TenantId == studentData!.TenantId && x.StudentGuid == studentData.StudentGuid).ToList();

                                                if (otherSchoolEnrollment?.Any() == true)
                                                {
                                                    foreach (var enrollmentData in otherSchoolEnrollment)
                                                    {
                                                        //this loop for update student's IsActive details and make it false for previos school
                                                        enrollmentData.IsActive = false;
                                                        this.context?.SaveChanges();
                                                    }
                                                }
                                                checkStudentAlreadyExistInTransferredSchool.IsActive = true;*/

                                                checkStudentAlreadyExistInTransferredSchool.EnrollmentType = "Internal";
                                                this.context?.SaveChanges();

                                                studentEnrollmentList.StudentId = (int)checkStudentAlreadyExistInTransferredSchool.StudentId;
                                            }
                                            else
                                            {
                                                //fetching student details from studentMaster table
                                                var studentData = this.context?.StudentMaster.FirstOrDefault(x => x.TenantId == studentEnrollmentListModel.TenantId && x.SchoolId == studentEnrollmentListModel.SchoolId && x.StudentId == studentEnrollmentListModel.StudentId);
                                                if (studentData != null)
                                                {
                                                    //fetching all student's active details from studentMaster table
                                                    /* var otherSchoolEnrollment = this.context?.StudentMaster.Where(x => x.TenantId == studentData.TenantId && x.StudentGuid == studentData.StudentGuid).ToList();
                                                     if (otherSchoolEnrollment?.Any()==true)
                                                     {
                                                         foreach (var enrollmentData in otherSchoolEnrollment)
                                                         {
                                                             //this loop for update student's IsActive details and make it false for previos school
                                                             enrollmentData.IsActive = false;
                                                             this.context?.SaveChanges();
                                                         }
                                                     }*/

                                                    if (studentEnrollmentList.ExitDate != null
                                             && studentEnrollmentList.ExitDate.Value.Date < DateTime.UtcNow.Date) //drop student in previous date
                                                    {
                                                        var studentOldSchool = this.context?.StudentMaster.Where(x => x.TenantId == studentEnrollmentListModel.TenantId && x.StudentGuid == studentEnrollmentListModel.StudentGuid).ToList();
                                                        if (studentOldSchool?.Any() == true)
                                                        {
                                                            studentOldSchool.ForEach(x => x.IsActive = false);

                                                            //this foreach for drop student from his scheduled course section
                                                            foreach (var studentSchool in studentOldSchool)
                                                            {
                                                                var studentCourseSection = context?.StudentCoursesectionSchedule.Where(x => x.SchoolId == studentSchool.SchoolId && x.TenantId == studentSchool.TenantId && x.StudentId == studentSchool.StudentId && x.IsDropped != true).ToList();
                                                                if (studentCourseSection?.Any() == true)
                                                                {
                                                                    studentCourseSection.ForEach(s => { s.IsDropped = true; s.EffectiveDropDate = studentEnrollmentList.ExitDate; });
                                                                }
                                                            }
                                                        }
                                                    }

                                                    //generate StudentId where student enroll(Transfer) & save data
                                                    int? MasterStudentId = 0;

                                                    var studentDataForTransferredSchool = this.context?.StudentMaster.Where(x => x.SchoolId == studentEnrollmentList.TransferredSchoolId && x.TenantId == studentEnrollmentList.TenantId).OrderByDescending(x => x.StudentId).FirstOrDefault();

                                                    if (studentDataForTransferredSchool != null)
                                                    {
                                                        MasterStudentId = studentDataForTransferredSchool.StudentId + 1;
                                                    }
                                                    else
                                                    {
                                                        MasterStudentId = 1;
                                                    }

                                                    var checkInternalId = this.context?.StudentMaster.Where(x => x.TenantId == studentEnrollmentList.TenantId && x.SchoolId == studentEnrollmentList.TransferredSchoolId && x.StudentInternalId == studentData.StudentInternalId).ToList();

                                                    string? StudentInternalId = null;

                                                    if (checkInternalId?.Any() == true)
                                                    {
                                                        StudentInternalId = MasterStudentId.ToString();
                                                    }
                                                    else
                                                    {
                                                        StudentInternalId = studentData.StudentInternalId;
                                                    }

                                                    //studentData.SchoolId = (int)studentEnrollmentList.TransferredSchoolId;
                                                    //studentData.StudentId = (int)MasterStudentId;
                                                    //studentData.EnrollmentType = "Internal";
                                                    //studentData.IsActive = true;
                                                    //studentData.LastUpdated = DateTime.UtcNow;                                                   
                                                    //this.context?.StudentMaster.Add(studentData);

                                                    StudentMaster studentMaster = new StudentMaster() { TenantId = studentData.TenantId, SchoolId = (int)studentEnrollmentList.TransferredSchoolId!, StudentId = (int)MasterStudentId, AlternateId = studentData.AlternateId, DistrictId = studentData.DistrictId, StateId = studentData.StateId, AdmissionNumber = studentData.AdmissionNumber, RollNumber = studentData.RollNumber, Salutation = studentData.Salutation, FirstGivenName = studentData.FirstGivenName, MiddleName = studentData.MiddleName, LastFamilyName = studentData.LastFamilyName, Suffix = studentData.Suffix, PreferredName = studentData.PreferredName, PreviousName = studentData.PreviousName, SocialSecurityNumber = studentData.SocialSecurityNumber, OtherGovtIssuedNumber = studentData.OtherGovtIssuedNumber, StudentPhoto = studentData.StudentPhoto, StudentThumbnailPhoto = studentData.StudentThumbnailPhoto, Dob = studentData.Dob, Gender = studentData.Gender, Race = studentData.Race, Ethnicity = studentData.Ethnicity, MaritalStatus = studentData.MaritalStatus, CountryOfBirth = studentData.CountryOfBirth, Nationality = studentData.Nationality, FirstLanguageId = studentData.FirstLanguageId, SecondLanguageId = studentData.SecondLanguageId, ThirdLanguageId = studentData.ThirdLanguageId, HomePhone = studentData.HomePhone, MobilePhone = studentData.MobilePhone, PersonalEmail = studentData.PersonalEmail, SchoolEmail = studentData.SchoolEmail, Twitter = studentData.Twitter, Facebook = studentData.Facebook, Instagram = studentData.Instagram, Youtube = studentData.Youtube, Linkedin = studentData.Linkedin, HomeAddressLineOne = studentData.HomeAddressLineOne, HomeAddressLineTwo = studentData.HomeAddressLineTwo, HomeAddressCountry = studentData.HomeAddressCountry, HomeAddressState = studentData.HomeAddressState, HomeAddressCity = studentData.HomeAddressCity, HomeAddressZip = studentData.HomeAddressZip, BusNo = studentData.BusNo, SchoolBusPickUp = studentData.SchoolBusPickUp, SchoolBusDropOff = studentData.SchoolBusDropOff, MailingAddressSameToHome = studentData.MailingAddressSameToHome, MailingAddressLineOne = studentData.MailingAddressLineOne, MailingAddressLineTwo = studentData.MailingAddressLineTwo, MailingAddressCountry = studentData.MailingAddressCountry, MailingAddressState = studentData.MailingAddressState, MailingAddressCity = studentData.MailingAddressCity, MailingAddressZip = studentData.MailingAddressZip, StudentPortalId = studentData.StudentPortalId, AlertDescription = studentData.AlertDescription, CriticalAlert = studentData.CriticalAlert, Dentist = studentData.Dentist, DentistPhone = studentData.DentistPhone, InsuranceCompany = studentData.InsuranceCompany, InsuranceCompanyPhone = studentData.InsuranceCompanyPhone, MedicalFacility = studentData.MedicalFacility, MedicalFacilityPhone = studentData.MedicalFacilityPhone, PolicyHolder = studentData.PolicyHolder, PolicyNumber = studentData.PolicyNumber, PrimaryCarePhysician = studentData.PrimaryCarePhysician, PrimaryCarePhysicianPhone = studentData.PrimaryCarePhysicianPhone, Vision = studentData.Vision, VisionPhone = studentData.VisionPhone, Associationship = studentData.Associationship, EconomicDisadvantage = studentData.EconomicDisadvantage, Eligibility504 = studentData.Eligibility504, EstimatedGradDate = studentData.EstimatedGradDate, FreeLunchEligibility = studentData.FreeLunchEligibility, LepIndicator = studentData.LepIndicator, SectionId = null, SpecialEducationIndicator = studentData.SpecialEducationIndicator, StudentInternalId = StudentInternalId, UpdatedOn = DateTime.UtcNow, UpdatedBy = studentData.UpdatedBy, EnrollmentType = "Internal", IsActive = studentEnrollmentList.ExitDate != null && studentEnrollmentList.ExitDate.Value.Date < DateTime.UtcNow.Date ? true : false, StudentGuid = studentData.StudentGuid };

                                                    this.context?.StudentMaster.Add(studentMaster);

                                                    if (studentEnrollmentList.ExitDate != null
                                             && studentEnrollmentList.ExitDate.Value.Date < DateTime.UtcNow.Date)
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
                                                                userMaster.SchoolId = (int)studentEnrollmentList.TransferredSchoolId;
                                                                userMaster.UserId = (int)MasterStudentId;
                                                                userMaster.Name = userMasterData.Name;
                                                                userMaster.EmailAddress = userMasterData.EmailAddress;
                                                                userMaster.PasswordHash = userMasterData.PasswordHash;
                                                                userMaster.LangId = userMasterData.LangId;
                                                                var membershipsId = this.context?.Membership.Where(x => x.SchoolId == (int)studentEnrollmentList.TransferredSchoolId && x.TenantId == studentEnrollmentList.TenantId && x.Profile == "Student").Select(x => x.MembershipId).FirstOrDefault();
                                                                userMaster.MembershipId = (int)membershipsId!;
                                                                userMaster.UpdatedOn = DateTime.UtcNow;
                                                                userMaster.UpdatedBy = studentEnrollmentList.UpdatedBy;
                                                                userMaster.IsActive = true;
                                                                this.context?.UserMaster.Add(userMaster);
                                                            }
                                                        }
                                                        this.context?.SaveChanges();
                                                    }
                                                    studentEnrollmentList.TenantId = studentEnrollmentList.TenantId;
                                                    studentEnrollmentList.SchoolId = (int)studentEnrollmentList.SchoolId;
                                                    studentEnrollmentList.StudentId = studentEnrollmentUpdate.StudentId;
                                                    studentEnrollmentList.EnrollmentId = (int)EnrollmentId;
                                                    studentEnrollmentList.EnrollmentDate = null;
                                                    studentEnrollmentList.EnrollmentCode = null;
                                                    studentEnrollmentList.ExitCode = studentExitCode.Title;
                                                    studentEnrollmentList.ExitDate = studentEnrollmentList.ExitDate;
                                                    studentEnrollmentList.SchoolName = studentEnrollmentUpdate.SchoolName;
                                                    studentEnrollmentList.SchoolTransferred = studentEnrollmentList.SchoolTransferred;
                                                    studentEnrollmentList.TransferredSchoolId = studentEnrollmentList.TransferredSchoolId;
                                                    studentEnrollmentList.GradeLevelTitle = null;
                                                    studentEnrollmentList.TransferredGrade = studentEnrollmentList.TransferredGrade;
                                                    studentEnrollmentList.CalenderId = studentEnrollmentUpdate.CalenderId;
                                                    studentEnrollmentList.RollingOption = "Next grade at current school";
                                                    studentEnrollmentList.UpdatedOn = DateTime.UtcNow;
                                                    studentEnrollmentList.IsActive = false;
                                                    this.context?.StudentEnrollment.AddRange(studentEnrollmentList);
                                                    this.context?.SaveChanges();
                                                    EnrollmentId++;

                                                    studentEnrollmentList.StudentId = (int)MasterStudentId;
                                                }
                                            }

                                            //fetch default calender for enroll(transfer) school and save details in StudentEnrollment table.
                                            int? calenderId = null;

                                            var defaultCalender = this.context?.SchoolCalendars.FirstOrDefault(x => x.TenantId == studentEnrollmentList.TenantId && x.SchoolId == studentEnrollmentList.TransferredSchoolId && x.AcademicYear == studentEnrollmentListModel.AcademicYear && x.DefaultCalender == true);

                                            if (defaultCalender != null)
                                            {
                                                calenderId = defaultCalender.CalenderId;
                                            }


                                            var transferredGradeId = this.context?.Gradelevels.AsEnumerable().FirstOrDefault(x => x.TenantId == studentEnrollmentList.TenantId && x.SchoolId == (int)studentEnrollmentList.TransferredSchoolId! && String.Compare(x.Title, studentEnrollmentList.TransferredGrade, true) == 0);

                                            studentEnrollmentList.TenantId = studentEnrollmentList.TenantId;
                                            studentEnrollmentList.SchoolId = (int)studentEnrollmentList.TransferredSchoolId!;
                                            studentEnrollmentList.EnrollmentId = (int)EnrollmentId;
                                            studentEnrollmentList.EnrollmentDate = studentEnrollmentList.ExitDate;
                                            studentEnrollmentList.EnrollmentCode = studentTransferIn.Title;
                                            studentEnrollmentList.ExitCode = null;
                                            studentEnrollmentList.ExitDate = null;
                                            studentEnrollmentList.SchoolName = studentEnrollmentList.SchoolTransferred;
                                            studentEnrollmentList.SchoolTransferred = null;
                                            studentEnrollmentList.TransferredSchoolId = null;
                                            studentEnrollmentList.GradeLevelTitle = studentEnrollmentList.TransferredGrade;
                                            studentEnrollmentList.GradeId = transferredGradeId != null ? transferredGradeId.GradeId : null;
                                            studentEnrollmentList.TransferredGrade = null;
                                            studentEnrollmentList.CalenderId = calenderId;
                                            studentEnrollmentList.RollingOption = "Next grade at current school";
                                            studentEnrollmentList.UpdatedOn = DateTime.UtcNow;
                                            studentEnrollmentList.UpdatedBy = studentEnrollmentList.UpdatedBy;
                                            studentEnrollmentList.IsActive = true;
                                            this.context?.StudentEnrollment.AddRange(studentEnrollmentList);
                                            EnrollmentId++;

                                            //this block for transfer associated parent
                                            var parentAssociationshipData = this.context?.ParentAssociationship.Where(x => x.TenantId == studentEnrollmentListModel.TenantId && x.SchoolId == studentEnrollmentListModel.SchoolId && x.StudentId == studentEnrollmentListModel.StudentId).ToList();
                                            if (parentAssociationshipData?.Any() == true)
                                            {
                                                foreach (var associationship in parentAssociationshipData)
                                                {
                                                    parentAssociationshipOldList.Add(associationship);

                                                    var associationshipNew = new ParentAssociationship
                                                    {
                                                        TenantId = studentEnrollmentList.TenantId,
                                                        SchoolId = studentEnrollmentList.SchoolId,
                                                        StudentId = studentEnrollmentList.StudentId,
                                                        ParentId = associationship.ParentId,
                                                        Relationship = associationship.Relationship,
                                                        Associationship = true,
                                                        UpdatedBy = studentEnrollmentList.UpdatedBy,
                                                        UpdatedOn = DateTime.UtcNow,
                                                        IsCustodian = associationship.IsCustodian,
                                                        ContactType = associationship.ContactType,
                                                        CreatedBy = associationship.CreatedBy,
                                                        CreatedOn = associationship.CreatedOn
                                                    };
                                                    parentAssociationshipNewList.Add(associationshipNew);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (studentExitCode.Type.ToLower() == "Drop".ToLower())
                                        {
                                            //This block update data for student's drop out from school
                                            studentEnrollmentUpdate.ExitCode = studentExitCode.Title;
                                            studentEnrollmentUpdate.ExitDate = studentEnrollmentList.ExitDate;
                                            studentEnrollmentUpdate.ExitReason = studentEnrollmentList.ExitReason;
                                            studentEnrollmentUpdate.TransferredGrade = studentEnrollmentList.GradeLevelTitle;
                                            studentEnrollmentUpdate.RollingOption = "Do not enroll after this school year";
                                            studentEnrollmentUpdate.UpdatedOn = DateTime.UtcNow;
                                            studentEnrollmentUpdate.UpdatedBy = studentEnrollmentList.UpdatedBy;
                                            studentEnrollmentUpdate.IsActive = true;

                                            if (studentEnrollmentList.ExitDate != null
                                                && studentEnrollmentList.ExitDate.Value.Date < DateTime.UtcNow.Date) //drop student in previous date
                                            {
                                                this.context?.StudentMaster.Where(x => x.StudentGuid == studentEnrollmentList.StudentGuid && x.SchoolId == studentEnrollmentList.SchoolId).ToList().ForEach(x => x.IsActive = false);

                                                var studentCourseSection = context?.StudentCoursesectionSchedule.Where(x => x.SchoolId == studentEnrollmentList.SchoolId && x.TenantId == studentEnrollmentList.TenantId && x.StudentGuid == studentEnrollmentList.StudentGuid && x.IsDropped != true).ToList();

                                                if (studentCourseSection?.Any() == true)
                                                {
                                                    studentCourseSection.ForEach(s => { s.IsDropped = true; s.EffectiveDropDate = studentEnrollmentList.ExitDate; });
                                                }
                                            }
                                            else
                                            {
                                                var scheduledJob = new ScheduledJob
                                                {
                                                    TenantId = studentEnrollmentList.TenantId,
                                                    SchoolId = studentEnrollmentList.SchoolId,
                                                    JobId = (long)Id,
                                                    AcademicYear = studentEnrollmentList.AcademicYear,
                                                    JobTitle = "StudentEnrollmentDropStudent",
                                                    JobScheduleDate = studentEnrollmentList.ExitDate!.Value.AddDays(1),
                                                    ApiTitle = "UpdateStudentEnrollment",
                                                    ControllerPath = studentEnrollmentListModel._tenantName + "/Student",
                                                    TaskJson = JsonConvert.SerializeObject(studentEnrollmentListModel),
                                                    LastRunStatus = null,
                                                    LastRunTime = null,
                                                    IsActive = true,
                                                    CreatedBy = studentEnrollmentList.UpdatedBy,
                                                    CreatedOn = DateTime.UtcNow
                                                };
                                                this.context?.ScheduledJobs.Add(scheduledJob);
                                                Id++;
                                            }
                                            this.context?.SaveChanges();
                                        }
                                        else
                                        {
                                            //This block save data for student's roll over
                                            studentEnrollmentUpdate.ExitCode = studentExitCode.Title;
                                            studentEnrollmentUpdate.ExitDate = studentEnrollmentList.ExitDate;
                                            studentEnrollmentUpdate.TransferredGrade = studentEnrollmentList.GradeLevelTitle;
                                            studentEnrollmentUpdate.UpdatedOn = DateTime.UtcNow;
                                            studentEnrollmentUpdate.UpdatedBy = studentEnrollmentList.UpdatedBy;
                                            studentEnrollmentUpdate.IsActive = false;

                                            studentEnrollment.EnrollmentId = (int)EnrollmentId;
                                            studentEnrollment.TenantId = studentEnrollmentList.TenantId;
                                            studentEnrollment.SchoolId = (int)studentEnrollmentList.SchoolId;
                                            studentEnrollment.StudentId = studentEnrollmentList.StudentId;
                                            studentEnrollment.CalenderId = studentEnrollmentListModel.CalenderId;
                                            studentEnrollment.SchoolName = studentEnrollmentList.SchoolName;
                                            studentEnrollment.EnrollmentDate = studentEnrollmentList.ExitDate;
                                            studentEnrollment.EnrollmentCode = studentExitCode.Title;
                                            studentEnrollment.GradeLevelTitle = studentEnrollmentList.GradeLevelTitle;
                                            studentEnrollment.GradeId = studentEnrollmentList.GradeId;
                                            studentEnrollment.RollingOption = studentEnrollmentListModel.RollingOption;
                                            studentEnrollment.UpdatedBy = studentEnrollmentList.UpdatedBy;
                                            studentEnrollment.UpdatedOn = DateTime.UtcNow;
                                            studentEnrollment.StudentGuid = studentEnrollmentUpdate.StudentGuid;
                                            studentEnrollment.IsActive = true;
                                            this.context?.StudentEnrollment.Add(studentEnrollment);

                                            //if (studentExitCode.Type.ToLower() == "Drop".ToLower() /*&& studentEnrollmentListModel.RollingOption.ToLower() == "Do not enroll after this school year".ToLower()*/)
                                            //{
                                            //    this.context?.StudentMaster.Where(x => x.StudentGuid == studentEnrollmentList.StudentGuid && x.SchoolId == studentEnrollmentList.SchoolId).ToList().ForEach(x => x.IsActive = false);
                                            //    //this.context?.StudentMaster.Where(x => x.StudentGuid == studentEnrollmentList.StudentGuid).ToList().ForEach(x => x.IsActive = false);
                                            //    this.context?.SaveChanges();
                                            //}

                                            EnrollmentId++;
                                        }
                                    }
                                }
                                else
                                {
                                    //This block for update existing enrollment details only
                                    var studentEnrollmentCode = this.context?.StudentEnrollmentCode.FirstOrDefault(x => x.TenantId == studentEnrollmentList.TenantId && x.SchoolId == studentEnrollmentList.SchoolId && x.EnrollmentCode.ToString() == studentEnrollmentList.EnrollmentCode);

                                    //Update grade of associated tables when grade update in student enrollment
                                    if (studentEnrollmentUpdate.GradeId != studentEnrollmentList.GradeId)
                                    {
                                        var academicYear = Utility.GetCurrentAcademicYear(this.context!, studentEnrollmentListModel.TenantId, studentEnrollmentListModel.SchoolId);

                                        var studentCourseSectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(x => x.TenantId == studentEnrollmentListModel.TenantId && x.SchoolId == studentEnrollmentListModel.SchoolId && x.StudentId == studentEnrollmentListModel.StudentId && x.AcademicYear == academicYear).ToList();

                                        if (studentCourseSectionScheduleData?.Any() != null)
                                        {
                                            studentCourseSectionScheduleData.ForEach(x => x.GradeId = studentEnrollmentList.GradeId);
                                        }

                                        var studentInputFinalGradeData = this.context?.StudentFinalGrade.Where(x => x.TenantId == studentEnrollmentListModel.TenantId && x.SchoolId == studentEnrollmentListModel.SchoolId && x.StudentId == studentEnrollmentListModel.StudentId && x.AcademicYear == academicYear).ToList();

                                        if (studentInputFinalGradeData?.Any() != null)
                                        {
                                            studentInputFinalGradeData.ForEach(x => x.GradeId = studentEnrollmentList.GradeId);
                                        }
                                    }

                                    studentEnrollmentUpdate.EnrollmentCode = studentEnrollmentCode != null ? studentEnrollmentCode.Title : null;
                                    studentEnrollmentUpdate.EnrollmentDate = studentEnrollmentList.EnrollmentDate;
                                    studentEnrollmentUpdate.GradeLevelTitle = studentEnrollmentList.GradeLevelTitle;
                                    studentEnrollmentUpdate.GradeId = studentEnrollmentList.GradeId;
                                    studentEnrollmentUpdate.RollingOption = studentEnrollmentListModel.RollingOption;
                                    studentEnrollmentUpdate.EnrollOtherSchoolId = studentEnrollmentListModel.EnrollOtherSchoolId;
                                    studentEnrollmentUpdate.CalenderId = studentEnrollmentListModel.CalenderId;
                                    studentEnrollmentUpdate.UpdatedOn = DateTime.UtcNow;
                                    studentEnrollmentUpdate.UpdatedBy = studentEnrollmentList.UpdatedBy;
                                }
                            }
                        }
                        else
                        {
                            //fetching student details from studentMaster table for the new school if exist previously
                            var checkStudentAlreadyExistInTransferredSchool = this.context?.StudentMaster.FirstOrDefault(x => x.TenantId == studentEnrollmentList.TenantId && x.SchoolId == studentEnrollmentList.SchoolId && x.StudentGuid == studentEnrollmentListModel.StudentGuid);

                            if (checkStudentAlreadyExistInTransferredSchool != null)
                            {
                                checkStudentAlreadyExistInTransferredSchool.IsActive = true;
                                checkStudentAlreadyExistInTransferredSchool.EnrollmentType = "External";
                                this.context?.SaveChanges();

                                studentEnrollmentList.StudentId = (int)checkStudentAlreadyExistInTransferredSchool.StudentId;
                            }
                            else
                            {
                                //This block for student new enrollment in another school as external school
                                var studentData = this.context?.StudentMaster.FirstOrDefault(x => x.TenantId == studentEnrollmentListModel.TenantId && x.SchoolId == studentEnrollmentListModel.SchoolId && x.StudentId == studentEnrollmentListModel.StudentId);

                                if (studentData != null)
                                {
                                    int? MasterStudentId = 0;

                                    var studentDataForNewSchool = this.context?.StudentMaster.Where(x => x.SchoolId == studentEnrollmentList.SchoolId && x.TenantId == studentEnrollmentList.TenantId).OrderByDescending(x => x.StudentId).FirstOrDefault();

                                    if (studentDataForNewSchool != null)
                                    {
                                        MasterStudentId = studentDataForNewSchool.StudentId + 1;
                                    }
                                    else
                                    {
                                        MasterStudentId = 1;
                                    }

                                    var checkInternalId = this.context?.StudentMaster.Where(x => x.TenantId == studentEnrollmentList.TenantId && x.SchoolId == studentEnrollmentList.SchoolId && x.StudentInternalId == studentData.StudentInternalId).ToList();

                                    string? StudentInternalId = null;

                                    if (checkInternalId?.Any() == true)
                                    {
                                        StudentInternalId = MasterStudentId.ToString();
                                    }
                                    else
                                    {
                                        StudentInternalId = studentData.StudentInternalId;
                                    }

                                    StudentMaster studentMaster = new StudentMaster() { TenantId = studentData.TenantId, SchoolId = studentEnrollmentList.SchoolId, StudentId = (int)MasterStudentId, AlternateId = studentData.AlternateId, DistrictId = studentData.DistrictId, StateId = studentData.StateId, AdmissionNumber = studentData.AdmissionNumber, RollNumber = studentData.RollNumber, Salutation = studentData.Salutation, FirstGivenName = studentData.FirstGivenName, MiddleName = studentData.MiddleName, LastFamilyName = studentData.LastFamilyName, Suffix = studentData.Suffix, PreferredName = studentData.PreferredName, PreviousName = studentData.PreviousName, SocialSecurityNumber = studentData.SocialSecurityNumber, OtherGovtIssuedNumber = studentData.OtherGovtIssuedNumber, StudentPhoto = studentData.StudentPhoto, StudentThumbnailPhoto = studentData.StudentThumbnailPhoto, Dob = studentData.Dob, Gender = studentData.Gender, Race = studentData.Race, Ethnicity = studentData.Ethnicity, MaritalStatus = studentData.MaritalStatus, CountryOfBirth = studentData.CountryOfBirth, Nationality = studentData.Nationality, FirstLanguageId = studentData.FirstLanguageId, SecondLanguageId = studentData.SecondLanguageId, ThirdLanguageId = studentData.ThirdLanguageId, HomePhone = studentData.HomePhone, MobilePhone = studentData.MobilePhone, PersonalEmail = studentData.PersonalEmail, SchoolEmail = studentData.SchoolEmail, Twitter = studentData.Twitter, Facebook = studentData.Facebook, Instagram = studentData.Instagram, Youtube = studentData.Youtube, Linkedin = studentData.Linkedin, HomeAddressLineOne = studentData.HomeAddressLineOne, HomeAddressLineTwo = studentData.HomeAddressLineTwo, HomeAddressCountry = studentData.HomeAddressCountry, HomeAddressState = studentData.HomeAddressState, HomeAddressCity = studentData.HomeAddressCity, HomeAddressZip = studentData.HomeAddressZip, BusNo = studentData.BusNo, SchoolBusPickUp = studentData.SchoolBusPickUp, SchoolBusDropOff = studentData.SchoolBusDropOff, MailingAddressSameToHome = studentData.MailingAddressSameToHome, MailingAddressLineOne = studentData.MailingAddressLineOne, MailingAddressLineTwo = studentData.MailingAddressLineTwo, MailingAddressCountry = studentData.MailingAddressCountry, MailingAddressState = studentData.MailingAddressState, MailingAddressCity = studentData.MailingAddressCity, MailingAddressZip = studentData.MailingAddressZip, StudentPortalId = studentData.StudentPortalId, AlertDescription = studentData.AlertDescription, CriticalAlert = studentData.CriticalAlert, Dentist = studentData.Dentist, DentistPhone = studentData.DentistPhone, InsuranceCompany = studentData.InsuranceCompany, InsuranceCompanyPhone = studentData.InsuranceCompanyPhone, MedicalFacility = studentData.MedicalFacility, MedicalFacilityPhone = studentData.MedicalFacilityPhone, PolicyHolder = studentData.PolicyHolder, PolicyNumber = studentData.PolicyNumber, PrimaryCarePhysician = studentData.PrimaryCarePhysician, PrimaryCarePhysicianPhone = studentData.PrimaryCarePhysicianPhone, Vision = studentData.Vision, VisionPhone = studentData.VisionPhone, Associationship = studentData.Associationship, EconomicDisadvantage = studentData.EconomicDisadvantage, Eligibility504 = studentData.Eligibility504, EstimatedGradDate = studentData.EstimatedGradDate, FreeLunchEligibility = studentData.FreeLunchEligibility, LepIndicator = studentData.LepIndicator, SectionId = null, SpecialEducationIndicator = studentData.SpecialEducationIndicator, StudentInternalId = StudentInternalId, UpdatedOn = DateTime.UtcNow, UpdatedBy = studentData.UpdatedBy, EnrollmentType = "External", IsActive = true, StudentGuid = studentData.StudentGuid };

                                    //studentData.SchoolId = studentEnrollmentList.SchoolId;
                                    //studentData.StudentId = (int)MasterStudentId;
                                    //studentData.EnrollmentType = "External";
                                    //studentData.IsActive = true;
                                    //studentData.LastUpdated = DateTime.UtcNow;
                                    //this.context?.StudentMaster.Add(studentData);
                                    this.context?.StudentMaster.Add(studentMaster);
                                    this.context?.SaveChanges();

                                    studentEnrollmentList.StudentId = (int)MasterStudentId;
                                }
                            }

                            var studentEnrollmentCode = this.context?.StudentEnrollmentCode.FirstOrDefault(x => x.TenantId == studentEnrollmentList.TenantId && x.SchoolId == studentEnrollmentList.SchoolId && x.EnrollmentCode.ToString() == studentEnrollmentList.EnrollmentCode);

                            int? calenderId = null;

                            var defaultCalender = this.context?.SchoolCalendars.FirstOrDefault(x => x.TenantId == studentEnrollmentList.TenantId && x.SchoolId == studentEnrollmentList.SchoolId && x.AcademicYear == studentEnrollmentListModel.AcademicYear && x.DefaultCalender == true);

                            if (defaultCalender != null)
                            {
                                calenderId = defaultCalender.CalenderId;
                            }

                            studentEnrollmentList.TenantId = studentEnrollmentList.TenantId;
                            studentEnrollmentList.SchoolId = studentEnrollmentList.SchoolId;

                            studentEnrollmentList.EnrollmentId = (int)EnrollmentId;
                            studentEnrollmentList.EnrollmentDate = studentEnrollmentList.EnrollmentDate;
                            studentEnrollmentList.EnrollmentCode = studentEnrollmentCode!.Title;
                            studentEnrollmentList.CalenderId = calenderId;
                            studentEnrollmentList.RollingOption = studentEnrollmentListModel.RollingOption;
                            studentEnrollmentList.EnrollOtherSchoolId = studentEnrollmentListModel.EnrollOtherSchoolId;
                            studentEnrollmentList.UpdatedOn = DateTime.UtcNow;
                            studentEnrollmentList.IsActive = true;
                            this.context?.StudentEnrollment.AddRange(studentEnrollmentList);
                            EnrollmentId++;
                        }
                    }
                    this.context?.ParentAssociationship.RemoveRange(parentAssociationshipOldList);
                    this.context?.SaveChanges();
                    this.context?.ParentAssociationship.AddRange(parentAssociationshipNewList);
                    this.context?.SaveChanges();
                    transaction?.Commit();
                    studentEnrollmentListModel._failure = false;
                    studentEnrollmentListModel._message = "Student Enrollment Updated Successfully";
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    studentEnrollmentListModel._failure = true;
                    studentEnrollmentListModel._message = es.Message;
                }
                return studentEnrollmentListModel;
            }
        }

        /// <summary>
        /// Get All Student Enrollment
        /// </summary>
        /// <param name="studentEnrollmentListViewModel"></param>
        /// <returns></returns>
        public StudentEnrollmentListViewModel GetAllStudentEnrollment_old(StudentEnrollmentListViewModel studentEnrollmentListViewModel)
        {
            StudentEnrollmentListViewModel studentEnrollmentListView = new StudentEnrollmentListViewModel();
            try
            {
                //fetch default calender id
                int? calenderId = null;

                var defaultCalender = this.context?.SchoolCalendars.FirstOrDefault(x => x.TenantId == studentEnrollmentListViewModel.TenantId && x.SchoolId == studentEnrollmentListViewModel.SchoolId && x.AcademicYear.ToString() == studentEnrollmentListViewModel.AcademicYear && x.DefaultCalender == true);

                if (defaultCalender != null)
                {
                    calenderId = defaultCalender.CalenderId;
                }

                var studentEnrollmentList = this.context?.StudentEnrollment.Where(x => x.TenantId == studentEnrollmentListViewModel.TenantId && x.StudentGuid == studentEnrollmentListViewModel.StudentGuid).OrderByDescending(x => x.EnrollmentId).ToList();

                if (studentEnrollmentList?.Any()==true)
                {
                    var studentEnrollment = studentEnrollmentList.Select(y => new StudentEnrollmentListForView
                    {
                        TenantId = y.TenantId,
                        SchoolId = y.SchoolId,
                        StudentId = y.StudentId,
                        GradeLevelTitle = y.GradeLevelTitle,
                        RollingOption = y.RollingOption,
                        EnrollOtherSchoolId = y.EnrollOtherSchoolId,
                        SchoolName = y.SchoolName,
                        UpdatedOn = y.UpdatedOn,
                        SchoolTransferred = y.SchoolTransferred,
                        TransferredGrade = y.TransferredGrade,
                        TransferredSchoolId = y.TransferredSchoolId,
                        UpdatedBy = y.UpdatedBy,
                        CreatedOn = y.CreatedOn,
                        CreatedBy = y.CreatedBy,
                        AcademicYear = this.context?.SchoolCalendars.FirstOrDefault(z => z.CalenderId == y.CalenderId && z.SchoolId == y.SchoolId)?.AcademicYear,
                        CalenderId = y.CalenderId,
                        EnrollmentCode = y.EnrollmentCode,
                        EnrollmentId = y.EnrollmentId,
                        EnrollmentDate = y.EnrollmentDate,
                        ExitCode = y.ExitCode,
                        ExitDate = y.ExitDate,
                        ExitReason = y.ExitReason,
                        StudentGuid = y.StudentGuid,
                        EnrollmentType = this.context?.StudentEnrollmentCode.FirstOrDefault(s => s.TenantId == y.TenantId && s.SchoolId == y.SchoolId && s.Title == y.EnrollmentCode)?.Type,
                        ExitType = this.context?.StudentEnrollmentCode.FirstOrDefault(s => s.TenantId == y.TenantId && s.SchoolId == y.SchoolId && s.Title == y.ExitCode)?.Type,
                        Type = this.context?.StudentMaster.FirstOrDefault(s => s.TenantId == y.TenantId && s.SchoolId == y.SchoolId && s.StudentId == y.StudentId)?.EnrollmentType,
                        GradeId = y.GradeId,
                        RolloverId = y.RolloverId,
                        //StartYear = y.EnrollmentDate != null ? this.context?.SchoolYears.FirstOrDefault(z => z.TenantId == y.TenantId && z.SchoolId == y.SchoolId && z.AcademicYear == y.EnrollmentDate.Value.Year)?.StartDate.Value.Year.ToString() : null,
                        //EndYear = y.EnrollmentDate != null ? this.context?.SchoolYears.FirstOrDefault(z => z.TenantId == y.TenantId && z.SchoolId == y.SchoolId && z.AcademicYear == y.EnrollmentDate.Value.Year)?.EndDate.Value.Year.ToString() : null,
                        StartYear = y.EnrollmentDate != null ? this.context?.SchoolCalendars.FirstOrDefault(z => z.TenantId == y.TenantId && z.SchoolId == y.SchoolId && z.SessionCalendar==true && z.StartDate <= y.EnrollmentDate && z.EndDate >= y.EnrollmentDate)?.StartDate!.Value.Year.ToString() : this.context?.SchoolCalendars.FirstOrDefault(z => z.TenantId == y.TenantId && z.SchoolId == y.SchoolId && z.SessionCalendar == true && z.StartDate <= y.ExitDate && z.EndDate >= y.ExitDate)?.StartDate!.Value.Year.ToString(),
                        EndYear = y.EnrollmentDate != null ? this.context?.SchoolCalendars.FirstOrDefault(z => z.TenantId == y.TenantId && z.SchoolId == y.SchoolId && z.SessionCalendar == true && z.StartDate <= y.EnrollmentDate && z.EndDate >= y.EnrollmentDate)?.EndDate!.Value.Year.ToString() : this.context?.SchoolCalendars.FirstOrDefault(z => z.TenantId == y.TenantId && z.SchoolId == y.SchoolId && z.SessionCalendar == true && z.StartDate <= y.ExitDate && z.EndDate >= y.ExitDate)?.EndDate!.Value.Year.ToString(),
                        IsActive = y.IsActive
                    }).ToList();
                    studentEnrollmentListView.studentEnrollmentListForView = studentEnrollment;
                    studentEnrollmentListView.RollingOption = studentEnrollmentList.FirstOrDefault(x => x.SchoolId == studentEnrollmentListViewModel.SchoolId)?.RollingOption;
                    studentEnrollmentListView.EnrollOtherSchoolId = studentEnrollmentList.FirstOrDefault(x => x.SchoolId == studentEnrollmentListViewModel.SchoolId)?.EnrollOtherSchoolId;

                    var studentMasterData = this.context?.StudentMaster.FirstOrDefault(x => x.TenantId == studentEnrollmentListViewModel.TenantId && x.SchoolId == studentEnrollmentListViewModel.SchoolId && x.StudentGuid == studentEnrollmentListViewModel.StudentGuid);

                    if (studentMasterData != null)
                    {
                        studentEnrollmentListView.SectionId = studentMasterData.SectionId;
                        studentEnrollmentListView.EstimatedGradDate = studentMasterData.EstimatedGradDate;
                        studentEnrollmentListView.Eligibility504 = studentMasterData.Eligibility504;
                        studentEnrollmentListView.EconomicDisadvantage = studentMasterData.EconomicDisadvantage;
                        studentEnrollmentListView.FreeLunchEligibility = studentMasterData.FreeLunchEligibility;
                        studentEnrollmentListView.SpecialEducationIndicator = studentMasterData.SpecialEducationIndicator;
                        studentEnrollmentListView.LepIndicator = studentMasterData.LepIndicator;
                    }

                    var fieldsCategory = this.context?.FieldsCategory.Where(x => x.TenantId == studentEnrollmentListViewModel.TenantId && x.SchoolId == studentEnrollmentListViewModel.SchoolId && x.Module == "Student").OrderByDescending(x => x.IsSystemCategory).ThenBy(x => x.SortOrder)
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
                           CustomFieldsValue = z.CustomFieldsValue.Where(w => w.TargetId == studentEnrollmentListViewModel.StudentId).ToList()
                       }).OrderByDescending(x => x.SystemField).ThenBy(x => x.SortOrder).ToList()
                   }).ToList();

                    if (fieldsCategory?.Any()==true)
                    {
                        studentEnrollmentListView.fieldsCategoryList = fieldsCategory;

                    }

                    studentEnrollmentListView._failure = false;
                }
                else
                {
                    studentEnrollmentListView._failure = true;
                    studentEnrollmentListView._message = NORECORDFOUND;
                }
                studentEnrollmentListView.StudentId = studentEnrollmentListViewModel.StudentId;
                studentEnrollmentListView._tenantName = studentEnrollmentListViewModel._tenantName;
                studentEnrollmentListView._token = studentEnrollmentListViewModel._token;
                studentEnrollmentListView.TenantId = studentEnrollmentListViewModel.TenantId;
                studentEnrollmentListView.CalenderId = calenderId;
                studentEnrollmentListView.AcademicYear = studentEnrollmentListViewModel.AcademicYear;
            }
            catch (Exception es)
            {
                studentEnrollmentListView._message = es.Message;
                studentEnrollmentListView._failure = true;
                studentEnrollmentListView._tenantName = studentEnrollmentListViewModel._tenantName;
                studentEnrollmentListView._token = studentEnrollmentListViewModel._token;
            }
            return studentEnrollmentListView;
        }

        public StudentEnrollmentListViewModel GetAllStudentEnrollment(StudentEnrollmentListViewModel studentEnrollmentListViewModel)
        {
            StudentEnrollmentListViewModel studentEnrollmentListView = new StudentEnrollmentListViewModel();

            studentEnrollmentListView._tenantName = studentEnrollmentListViewModel._tenantName;
            studentEnrollmentListView._token = studentEnrollmentListViewModel._token;
            studentEnrollmentListView.StudentId = studentEnrollmentListViewModel.StudentId;
            studentEnrollmentListView._tenantName = studentEnrollmentListViewModel._tenantName;
            studentEnrollmentListView._userName = studentEnrollmentListViewModel._userName;
            studentEnrollmentListView._token = studentEnrollmentListViewModel._token;
            studentEnrollmentListView.TenantId = studentEnrollmentListViewModel.TenantId;
            studentEnrollmentListView.AcademicYear = studentEnrollmentListViewModel.AcademicYear;
            try
            {
                studentEnrollmentListView.CalenderId = this.context?.SchoolCalendars.FirstOrDefault(x => x.TenantId == studentEnrollmentListViewModel.TenantId && x.SchoolId == studentEnrollmentListViewModel.SchoolId && x.AcademicYear.ToString() == studentEnrollmentListViewModel.AcademicYear && x.DefaultCalender == true)?.CalenderId;

                var studentEnrollmentList = this.context?.StudentEnrollment.Where(x => x.TenantId == studentEnrollmentListViewModel.TenantId && x.StudentGuid == studentEnrollmentListViewModel.StudentGuid).Select(y => new StudentEnrollmentListForView
                {
                    TenantId = y.TenantId,
                    SchoolId = y.SchoolId,
                    StudentId = y.StudentId,
                    RollingOption = y.RollingOption,
                    EnrollOtherSchoolId = y.EnrollOtherSchoolId,
                    SchoolName = y.SchoolName,
                    UpdatedOn = y.UpdatedOn,
                    SchoolTransferred = y.SchoolTransferred,
                    TransferredGrade = y.TransferredGrade,
                    TransferredSchoolId = y.TransferredSchoolId,
                    UpdatedBy = y.UpdatedBy,
                    CreatedOn = y.CreatedOn,
                    CreatedBy = y.CreatedBy,
                    CalenderId = y.CalenderId,
                    EnrollmentCode = y.EnrollmentCode,
                    EnrollmentId = y.EnrollmentId,
                    EnrollmentDate = y.EnrollmentDate,
                    ExitCode = y.ExitCode,
                    ExitDate = y.ExitDate,
                    StudentGuid = y.StudentGuid,
                    GradeId = y.GradeId,
                    RolloverId = y.RolloverId,
                    IsActive = y.IsActive
                }).OrderByDescending(x => x.EnrollmentId).ToList();

                if (studentEnrollmentList?.Any() == true)
                {
                    var studentMasterData = this.context?.StudentMaster.Where(x => x.TenantId == studentEnrollmentListViewModel.TenantId && x.SchoolId == studentEnrollmentListViewModel.SchoolId && x.StudentGuid == studentEnrollmentListViewModel.StudentGuid);

                    if (studentMasterData?.Any() == true)
                    {
                        var studentMaster = studentMasterData.FirstOrDefault()!;
                        studentEnrollmentListView.SectionId = studentMaster.SectionId;
                        studentEnrollmentListView.EstimatedGradDate = studentMaster.EstimatedGradDate;
                        studentEnrollmentListView.Eligibility504 = studentMaster.Eligibility504;
                        studentEnrollmentListView.EconomicDisadvantage = studentMaster.EconomicDisadvantage;
                        studentEnrollmentListView.FreeLunchEligibility = studentMaster.FreeLunchEligibility;
                        studentEnrollmentListView.SpecialEducationIndicator = studentMaster.SpecialEducationIndicator;
                        studentEnrollmentListView.LepIndicator = studentMaster.LepIndicator;
                    }

                    var studentEnrollmentCodeData = this.context?.StudentEnrollmentCode.Where(x => x.TenantId == studentEnrollmentListViewModel.TenantId).ToList();
                    var gradeLevelData = this.context?.Gradelevels.Where(x => x.TenantId == studentEnrollmentListViewModel.TenantId).ToList();
                    var schoolCalendarData = this.context?.SchoolCalendars.Where(x => x.TenantId == studentEnrollmentListViewModel.TenantId).ToList();

                    foreach (var studentEnrollment in studentEnrollmentList)
                    {
                        studentEnrollment.GradeLevelTitle = gradeLevelData?.FirstOrDefault(s => s.SchoolId == studentEnrollment.SchoolId && s.GradeId == studentEnrollment.GradeId)?.Title;
                        if (studentEnrollment.EnrollmentDate != null)
                        {
                            var calData = schoolCalendarData?.FirstOrDefault(z => z.SchoolId == studentEnrollment.SchoolId && z.SessionCalendar == true && z.StartDate <= studentEnrollment.EnrollmentDate && z.EndDate >= studentEnrollment.EnrollmentDate);
                            studentEnrollment.AcademicYear = calData?.AcademicYear;
                            studentEnrollment.StartYear = calData?.StartDate!.Value.Year.ToString();
                            studentEnrollment.EndYear = calData?.EndDate!.Value.Year.ToString();
                        }
                        else
                        {
                            var calData = schoolCalendarData?.FirstOrDefault(z => z.SchoolId == studentEnrollment.SchoolId && z.SessionCalendar == true && z.StartDate <= studentEnrollment.ExitDate && z.EndDate >= studentEnrollment.ExitDate);
                            studentEnrollment.AcademicYear = calData?.AcademicYear;
                            studentEnrollment.StartYear = calData?.StartDate!.Value.Year.ToString();
                            studentEnrollment.EndYear = calData?.EndDate!.Value.Year.ToString();
                        }

                        studentEnrollment.EnrollmentCodeId = studentEnrollmentCodeData?.FirstOrDefault(s => s.SchoolId == studentEnrollment.SchoolId && s.AcademicYear == studentEnrollment.AcademicYear && s.Title == studentEnrollment.EnrollmentCode)?.EnrollmentCode;
                        studentEnrollment.EnrollmentType = studentEnrollmentCodeData?.FirstOrDefault(s => s.SchoolId == studentEnrollment.SchoolId && s.AcademicYear == studentEnrollment.AcademicYear && s.Title == studentEnrollment.EnrollmentCode)?.Type;
                        studentEnrollment.ExitType = studentEnrollmentCodeData?.FirstOrDefault(s => s.SchoolId == studentEnrollment.SchoolId && s.Title == studentEnrollment.ExitCode && s.AcademicYear == studentEnrollment.AcademicYear && s.Type != "Add" && s.Type != "Enroll (Transfer)")?.Type;
                        studentEnrollment.ExitCodeId = studentEnrollmentCodeData?.FirstOrDefault(s => s.SchoolId == studentEnrollment.SchoolId && s.Title == studentEnrollment.ExitCode && s.AcademicYear == studentEnrollment.AcademicYear && s.Type != "Add" && s.Type != "Enroll (Transfer)")?.EnrollmentCode;
                        studentEnrollment.Type = studentMasterData?.FirstOrDefault(s => s.TenantId == studentEnrollment.TenantId && s.SchoolId == studentEnrollment.SchoolId && s.StudentId == studentEnrollment.StudentId)?.EnrollmentType;
                    }

                    studentEnrollmentListView.studentEnrollmentListForView = studentEnrollmentList;

                    studentEnrollmentListView.RollingOption = studentEnrollmentList.FirstOrDefault(x => x.SchoolId == studentEnrollmentListViewModel.SchoolId)?.RollingOption;
                    studentEnrollmentListView.EnrollOtherSchoolId = studentEnrollmentList.FirstOrDefault(x => x.SchoolId == studentEnrollmentListViewModel.SchoolId)?.EnrollOtherSchoolId;

                    var fieldsCategory = this.context?.FieldsCategory.Where(x => x.TenantId == studentEnrollmentListViewModel.TenantId && x.SchoolId == studentEnrollmentListViewModel.SchoolId && x.Module == "Student").OrderByDescending(x => x.IsSystemCategory).ThenBy(x => x.SortOrder)
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
                       CustomFields = y.CustomFields.Select(z => new CustomFields
                       {
                           TenantId = z.TenantId,
                           SchoolId = z.SchoolId,
                           CategoryId = z.CategoryId,
                           FieldName = z.FieldName,
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
                           CustomFieldsValue = z.CustomFieldsValue.Where(w => w.TargetId == studentEnrollmentListViewModel.StudentId).ToList()
                       }).OrderByDescending(x => x.SystemField).ThenBy(x => x.SortOrder).ToList()
                   }).ToList();

                    if (fieldsCategory?.Any() == true)
                    {
                        studentEnrollmentListView.fieldsCategoryList = fieldsCategory;
                    }

                    studentEnrollmentListView._failure = false;
                }
                else
                {
                    studentEnrollmentListView._failure = true;
                    studentEnrollmentListView._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                studentEnrollmentListView._message = es.Message;
                studentEnrollmentListView._failure = true;
            }
            return studentEnrollmentListView;
        }

        /// <summary>
        /// Get Student By Id
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public StudentAddViewModel ViewStudent(StudentAddViewModel student)
        {
            if(student.studentMaster is null)
            {
                return student;
            }
            StudentAddViewModel studentView = new StudentAddViewModel();
            try
            {

                var studentData = this.context?.StudentMaster.Include(x => x.StudentMedicalAlert).FirstOrDefault(x => x.TenantId == student.studentMaster.TenantId && x.SchoolId == student.studentMaster.SchoolId && x.StudentId == student.studentMaster.StudentId);
                if (studentData != null)
                {
                    studentView.studentMaster = studentData;
                    studentView.CurrentGradeLevel = this.context?.StudentEnrollment.Where(x => x.TenantId == studentData.TenantId && x.StudentGuid == studentData.StudentGuid && x.IsActive == true).Select(x => x.GradeLevelTitle).FirstOrDefault();

                    if (studentData.StudentPortalId != null)
                    {
                        var userMasterData = this.context?.UserMaster.FirstOrDefault(x => x.EmailAddress == studentData.StudentPortalId);

                        if (userMasterData != null)
                        {
                            studentView.LoginEmail = userMasterData.EmailAddress;
                            studentView.PortalAccess = userMasterData.IsActive;
                        }
                    }
                   var customFields = this.context?.FieldsCategory.Where(x => x.TenantId == student.studentMaster.TenantId && x.SchoolId == student.studentMaster.SchoolId && x.Module == "Student").OrderByDescending(x=>x.IsSystemCategory).ThenBy(x=>x.SortOrder)
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
                            CreatedOn =y.CreatedOn,
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
                                Hide=z.Hide,
                                DefaultSelection = z.DefaultSelection,
                                UpdatedOn = z.UpdatedOn,
                                UpdatedBy = z.UpdatedBy,
                                CreatedBy = z.CreatedBy,
                                CreatedOn = z.CreatedOn,
                                CustomFieldsValue = z.CustomFieldsValue.Where(w => w.TargetId == student.studentMaster.StudentId).ToList()
                            }).OrderByDescending(x=>x.SystemField).ThenBy(x=>x.SortOrder).ToList()
                        }).ToList();
                    studentView.fieldsCategoryList = customFields??new();
                    studentView._tenantName = student._tenantName;
                    studentView._token = student._token;
                }
                else
                {
                    studentView._tenantName = student._tenantName;
                    studentView._token = student._token;
                    studentView._failure = true;
                    studentView._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                studentView._tenantName = student._tenantName;
                studentView._token = student._token;
                studentView._failure = true;
                studentView._message = es.Message;
            }
            return studentView;
        }

        //    /// <summary>
        //    /// Delete Student
        //    /// </summary>
        //    /// <param name="student"></param>
        //    /// <returns></returns>

        //    public StudentAddViewModel DeleteStudent(StudentAddViewModel student)
        //    {
        //        try
        //        {
        //            var studentDel = this.context?.StudentEnrollment.FirstOrDefault(x => x.TenantId == student.studentEnrollment.TenantId && x.SchoolId == student.studentEnrollment.SchoolId && x.StudentId == student.studentEnrollment.StudentId);
        //            this.context?.StudentEnrollment.Remove(studentDel);
        //            this.context?.SaveChanges();
        //            student._failure = false;
        //            student._message = "Deleted";
        //        }
        //        catch (Exception es)
        //        {
        //            student._failure = true;
        //            student._message = es.Message;
        //        }
        //        return student;
        //    }

        //Get Full Address
        private static string ToFullAddress(string? Address1, string? Address2, string? City, string? State, string? Country, string? Zip)
        {
            string? address = "";
            if (!string.IsNullOrWhiteSpace(Address1))
            {


                return address == null
                      ? ""
                      : $"{Address1?.Trim()}{(!string.IsNullOrWhiteSpace(Address2) ? $", {Address2?.Trim()}" : string.Empty)}, {City?.Trim()}, {State?.Trim()} {Zip?.Trim()}";
            }
            return address;
        }

        /// <summary>
        /// Search Sibling For Student
        /// </summary>
        /// <param name="studentSiblingListViewModel"></param>
        /// <returns></returns>
        /*public SiblingSearchForStudentListModel SearchSiblingForStudent(SiblingSearchForStudentListModel studentSiblingListViewModel)
        {
            SiblingSearchForStudentListModel StudentSiblingList = new SiblingSearchForStudentListModel();
            try
            {
                int resultData;
                var studentData = (from student in this.context?.StudentMaster
                                   join enrollment in this.context?.StudentEnrollment on student.StudentId equals enrollment.StudentId
                                   where student.SchoolId == enrollment.SchoolId && student.TenantId == enrollment.TenantId && enrollment.GradeLevelTitle != null && enrollment.IsActive == true
                                   select new
                                   {
                                       student.TenantId,
                                       student.SchoolId,
                                       student.StudentId,
                                       student.HomeAddressLineOne,
                                       student.HomeAddressLineTwo,
                                       student.HomeAddressCountry,
                                       student.HomeAddressState,
                                       student.HomeAddressCity,
                                       student.HomeAddressZip,
                                       student.StudentInternalId,
                                       student.FirstGivenName,
                                       student.MiddleName,
                                       student.LastFamilyName,
                                       student.Dob,
                                       enrollment.GradeLevelTitle
                                   });
                if (studentData != null && studentData?.Any()==true)
                {
                    var StudentSibling = studentData.Where(x => x.FirstGivenName == studentSiblingListViewModel.FirstGivenName && x.LastFamilyName == studentSiblingListViewModel.LastFamilyName && x.TenantId == studentSiblingListViewModel.TenantId && (studentSiblingListViewModel.GradeLevelTitle == null || (x.GradeLevelTitle == studentSiblingListViewModel.GradeLevelTitle)) && (studentSiblingListViewModel.SchoolId == null || (x.SchoolId == studentSiblingListViewModel.SchoolId)) && (studentSiblingListViewModel.Dob == null || (x.Dob == studentSiblingListViewModel.Dob)) && (studentSiblingListViewModel.StudentInternalId == null || (x.StudentInternalId.ToLower().Trim() == studentSiblingListViewModel.StudentInternalId.ToLower().Trim()))).ToList();
                    if (StudentSibling?.Any()==true)
                    {
                        var siblingsOfStudent = StudentSibling.Select(s => new GetStudentForView
                        {
                            FirstGivenName = s.FirstGivenName,
                            LastFamilyName = s.LastFamilyName,
                            Dob = s.Dob,
                            StudentId = s.StudentId,
                            StudentInternalId = s.StudentInternalId,
                            SchoolId = s.SchoolId,
                            TenantId = s.TenantId,
                            SchoolName = this.context?.SchoolMaster.Where(x => x.SchoolId == s.SchoolId)?.Select(e => e.SchoolName).FirstOrDefault(),
                            Address = ToFullAddress(s.HomeAddressLineOne, s.HomeAddressLineTwo,
                        int.TryParse(s.HomeAddressCity, out resultData) == true ? this.context.City.Where(x => x.Id == Convert.ToInt32(s.HomeAddressCity)).FirstOrDefault().Name : s.HomeAddressCity,
                        int.TryParse(s.HomeAddressState, out resultData) == true ? this.context.State.Where(x => x.Id == Convert.ToInt32(s.HomeAddressState)).FirstOrDefault().Name : s.HomeAddressState,
                        int.TryParse(s.HomeAddressCountry, out resultData) == true ? this.context.Country.Where(x => x.Id == Convert.ToInt32(s.HomeAddressCountry)).FirstOrDefault().Name : string.Empty, s.HomeAddressZip),
                            GradeLevelTitle = s.GradeLevelTitle
                        }).ToList();
                        StudentSiblingList.getStudentForView = siblingsOfStudent;
                        StudentSiblingList._tenantName = studentSiblingListViewModel._tenantName;
                        StudentSiblingList._token = studentSiblingListViewModel._token;
                        StudentSiblingList._failure = false;
                    }
                    else
                    {
                        StudentSiblingList._failure = true;
                        StudentSiblingList._message = NORECORDFOUND;
                    }
                }
                else
                {
                    StudentSiblingList._failure = true;
                    StudentSiblingList._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                StudentSiblingList._message = es.Message;
                StudentSiblingList._failure = true;
                StudentSiblingList._tenantName = studentSiblingListViewModel._tenantName;
                StudentSiblingList._token = studentSiblingListViewModel._token;
            }
            return StudentSiblingList;
        }
        */
        public SiblingSearchForStudentListModel SearchSiblingForStudent(SiblingSearchForStudentListModel studentSiblingListViewModel)
        {
            SiblingSearchForStudentListModel StudentSiblingList = new SiblingSearchForStudentListModel();
            try
            {
                int resultData;

                var studentData = this.context?.StudentMaster
                    .Include(s => s.StudentEnrollment).Where(x => x.IsActive == true && x.FirstGivenName == studentSiblingListViewModel.FirstGivenName && x.LastFamilyName == studentSiblingListViewModel.LastFamilyName && x.TenantId == studentSiblingListViewModel.TenantId
                        && (studentSiblingListViewModel.SchoolId == null || (x.SchoolId == studentSiblingListViewModel.SchoolId)) && (studentSiblingListViewModel.Dob == null || (x.Dob == studentSiblingListViewModel.Dob)) && (string.IsNullOrEmpty(studentSiblingListViewModel.StudentInternalId) || (
                        (x.StudentInternalId ?? "").ToLower().Trim() == studentSiblingListViewModel.StudentInternalId.ToLower().Trim()))
                    && (string.IsNullOrEmpty(studentSiblingListViewModel.GradeLevelTitle) || (x.StudentEnrollment.Any(s => s.IsActive == true && s.GradeLevelTitle == studentSiblingListViewModel.GradeLevelTitle)))).Select(s => new GetStudentForView
                    {
                        FirstGivenName = s.FirstGivenName,
                        LastFamilyName = s.LastFamilyName,
                        Dob = s.Dob,
                        StudentId = s.StudentId,
                        StudentInternalId = s.StudentInternalId,
                        SchoolId = s.SchoolId,
                        TenantId = s.TenantId,
                        SchoolName = s.StudentEnrollment.Where(s => s.IsActive == true).Select(s => s.SchoolName).FirstOrDefault(),
                        Address = ToFullAddress(s.HomeAddressLineOne, s.HomeAddressLineTwo, s.HomeAddressCity, s.HomeAddressState,
                                   int.TryParse(s.HomeAddressCountry, out resultData) == true ? this.context.Country.Where(x => x.Id == Convert.ToInt32(s.HomeAddressCountry)).FirstOrDefault()!.Name : string.Empty, s.HomeAddressZip),
                        GradeLevelTitle = s.StudentEnrollment.Where(s => s.IsActive == true).Select(s => s.GradeLevelTitle).FirstOrDefault()
                    }).ToList();
                if (studentData != null && studentData?.Any() == true)
                {

                    StudentSiblingList.getStudentForView = studentData;
                    StudentSiblingList._tenantName = studentSiblingListViewModel._tenantName;
                    StudentSiblingList._token = studentSiblingListViewModel._token;
                    StudentSiblingList._failure = false;
                }
                else
                {
                    StudentSiblingList._failure = true;
                    StudentSiblingList._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                StudentSiblingList._message = es.Message;
                StudentSiblingList._failure = true;
                StudentSiblingList._tenantName = studentSiblingListViewModel._tenantName;
                StudentSiblingList._token = studentSiblingListViewModel._token;
            }
            return StudentSiblingList;
        }
        /// <summary>
        /// Association Sibling
        /// </summary>
        /// <param name="siblingAddUpdateForStudentModel"></param>
        /// <returns></returns>
        public SiblingAddUpdateForStudentModel AssociationSibling(SiblingAddUpdateForStudentModel siblingAddUpdateForStudentModel)
        {
            SiblingAddUpdateForStudentModel siblingAddUpdateForStudent = new SiblingAddUpdateForStudentModel();
            try
            {
                if (siblingAddUpdateForStudentModel.studentMaster!.StudentId > 0)
                {
                    var studentAssociateTo = this.context?.StudentMaster.FirstOrDefault(x => x.StudentId == siblingAddUpdateForStudentModel.studentMaster.StudentId && x.SchoolId == siblingAddUpdateForStudentModel.studentMaster.SchoolId && x.TenantId == siblingAddUpdateForStudentModel.studentMaster.TenantId);

                    var studentAssociateBy = this.context?.StudentMaster.FirstOrDefault(x => x.StudentId == siblingAddUpdateForStudentModel.StudentId && x.SchoolId == siblingAddUpdateForStudentModel.SchoolId && x.TenantId == siblingAddUpdateForStudentModel.TenantId);

                    if (studentAssociateTo != null && studentAssociateBy != null)
                    {
                        if (studentAssociateTo.Associationship != null)
                        {
                            var associationData = siblingAddUpdateForStudentModel.studentMaster.TenantId + "#" + siblingAddUpdateForStudentModel.SchoolId + "#" + siblingAddUpdateForStudentModel.StudentId;

                            var checkAssociation = studentAssociateTo.Associationship.Contains(associationData);
                            if (checkAssociation)
                            {
                                siblingAddUpdateForStudentModel._failure = true;
                                siblingAddUpdateForStudentModel._message = "Sibling already added";
                                return siblingAddUpdateForStudentModel;
                            }
                            else
                            {
                                studentAssociateTo.Associationship = studentAssociateTo.Associationship + " | " + associationData;
                            }
                        }
                        else
                        {
                            studentAssociateTo.Associationship = siblingAddUpdateForStudentModel.studentMaster.TenantId + "#" + siblingAddUpdateForStudentModel.SchoolId + "#" + siblingAddUpdateForStudentModel.StudentId;
                        }

                        if (studentAssociateBy.Associationship != null)
                        {
                            var associationData = siblingAddUpdateForStudentModel.studentMaster.TenantId + "#" + siblingAddUpdateForStudentModel.studentMaster.SchoolId + "#" + siblingAddUpdateForStudentModel.studentMaster.StudentId;

                            var checkAssociation = studentAssociateTo.Associationship.Contains(associationData);
                            if (checkAssociation)
                            {
                                siblingAddUpdateForStudentModel._failure = true;
                                siblingAddUpdateForStudentModel._message = "Sibling already added";
                                return siblingAddUpdateForStudentModel;
                            }
                            else
                            {
                                studentAssociateBy.Associationship = studentAssociateBy.Associationship + " | " + associationData;
                            }
                        }
                        else
                        {
                            studentAssociateBy.Associationship = siblingAddUpdateForStudentModel.studentMaster.TenantId + "#" + siblingAddUpdateForStudentModel.studentMaster.SchoolId + "#" + siblingAddUpdateForStudentModel.studentMaster.StudentId;
                        }

                        this.context?.SaveChanges();
                        siblingAddUpdateForStudentModel._failure = false;
                        siblingAddUpdateForStudentModel._message = "Sibling added successfully";
                    }
                    else
                    {
                        siblingAddUpdateForStudentModel._failure = true;
                        siblingAddUpdateForStudentModel._message = "Please select a valid student";
                    }
                }
            }
            catch (Exception es)
            {
                siblingAddUpdateForStudentModel._message = es.Message;
                siblingAddUpdateForStudentModel._failure = true;
                siblingAddUpdateForStudentModel._tenantName = siblingAddUpdateForStudentModel._tenantName;
                siblingAddUpdateForStudentModel._token = siblingAddUpdateForStudentModel._token;
            }
            return siblingAddUpdateForStudentModel;
        }

        /// <summary>
        /// Get All Sibling
        /// </summary>
        /// <param name="studentListModel"></param>
        /// <returns></returns>
        public StudentListModel ViewAllSibling(StudentListModel studentListModel)
        {
            StudentListModel studentList = new StudentListModel();
            try
            {
                var Associationship = studentListModel.TenantId + "#" + studentListModel.SchoolId + "#" + studentListModel.StudentId;
                if (Associationship != null)
                {
                    var studentAssociationship = this.context?.StudentMaster.Where(x => (x.Associationship??"").Contains(Associationship)).Include(x => x.SchoolMaster).Include(x => x.StudentEnrollment).ToList();
                    if (studentAssociationship?.Any() == true)
                    {
                        foreach (var studentData in studentAssociationship)
                        {
                            studentData.StudentEnrollment = studentData.StudentEnrollment.Where(x => x.IsActive == true && x.SchoolId == studentData.SchoolId && x.TenantId == studentData.TenantId && x.StudentId == studentData.StudentId).ToList();
                            studentData.StudentPhoto = null;

                            if (!studentListModel.IsShowPicture)
                            {
                                studentData.StudentThumbnailPhoto = null;
                            }
                        }

                        
                        studentList.studentMaster = studentAssociationship;
                        studentList._tenantName = studentListModel._tenantName;
                        studentList._token = studentListModel._token;
                        studentList._failure = false;
                    }
                    else
                    {
                        studentList._failure = true;
                        studentList._message = NORECORDFOUND;
                    }
                }
                else
                {
                    studentList._failure = true;
                    studentList._message = NORECORDFOUND;
                }
                
            }
            catch (Exception es)
            {
                studentList.studentMaster = new();
                studentList._message = es.Message;
                studentList._failure = true;
                studentList._tenantName = studentListModel._tenantName;
                studentList._token = studentListModel._token;
            }
            return studentList;
        }

        /// <summary>
        /// Remove Sibling
        /// </summary>
        /// <param name="siblingAddUpdateForStudentModel"></param>
        /// <returns></returns>
        public SiblingAddUpdateForStudentModel RemoveSibling(SiblingAddUpdateForStudentModel siblingAddUpdateForStudentModel)
        {
            try
            {
                string? StudentAssociateToAfterDel;
                string? StudentAssociateByAfterDel;
                var StudentAssociateTo = this.context?.StudentMaster.FirstOrDefault(x => x.StudentId == siblingAddUpdateForStudentModel.studentMaster!.StudentId && x.SchoolId == siblingAddUpdateForStudentModel.studentMaster.SchoolId);
                var StudentAssociateBy = this.context?.StudentMaster.FirstOrDefault(x => x.StudentId == siblingAddUpdateForStudentModel.StudentId && x.SchoolId == siblingAddUpdateForStudentModel.SchoolId);
                var StudentAssociateToDataDel = siblingAddUpdateForStudentModel.studentMaster!.TenantId + "#" + siblingAddUpdateForStudentModel.studentMaster.SchoolId + "#" + siblingAddUpdateForStudentModel.studentMaster.StudentId;
                var StudentAssociateByDataDel = siblingAddUpdateForStudentModel.studentMaster.TenantId + "#" + siblingAddUpdateForStudentModel.SchoolId + "#" + siblingAddUpdateForStudentModel.StudentId;

                if (StudentAssociateTo != null && StudentAssociateTo.Associationship != null)
                {
                    var AssociationshipToData = StudentAssociateTo.Associationship;

                    string[] StudentAssociateToWithSiblings = AssociationshipToData.Split(" | ", StringSplitOptions.RemoveEmptyEntries);

                    StudentAssociateToWithSiblings = StudentAssociateToWithSiblings.Where(w => w != StudentAssociateByDataDel).ToArray();

                    if (StudentAssociateToWithSiblings.Count() > 1)
                    {
                        StudentAssociateToAfterDel = string.Join(" | ", StudentAssociateToWithSiblings);
                    }
                    else if (StudentAssociateToWithSiblings.Count() == 1)
                    {
                        StudentAssociateToAfterDel = string.Concat(StudentAssociateToWithSiblings);
                    }
                    else
                    {
                        StudentAssociateToAfterDel = null;
                    }
                    StudentAssociateTo.Associationship = StudentAssociateToAfterDel;
                }

                if (StudentAssociateBy != null && StudentAssociateBy.Associationship != null)
                {
                    var AssociationshipByData = StudentAssociateBy.Associationship;

                    string[] StudentAssociateByWithSiblings = AssociationshipByData.Split(" | ", StringSplitOptions.RemoveEmptyEntries);

                    StudentAssociateByWithSiblings = StudentAssociateByWithSiblings.Where(w => w != StudentAssociateToDataDel).ToArray();

                    if (StudentAssociateByWithSiblings.Count() > 1)
                    {
                        StudentAssociateByAfterDel = string.Join(" | ", StudentAssociateByWithSiblings);
                    }
                    else if (StudentAssociateByWithSiblings.Count() == 1)
                    {
                        StudentAssociateByAfterDel = string.Concat(StudentAssociateByWithSiblings);
                    }
                    else
                    {
                        StudentAssociateByAfterDel = null;
                    }
                    StudentAssociateBy.Associationship = StudentAssociateByAfterDel;

                }
                this.context?.SaveChanges();
                siblingAddUpdateForStudentModel._message = "Sibling deleted successfullyy";
            }

            catch (Exception es)
            {
                siblingAddUpdateForStudentModel._failure = true;
                siblingAddUpdateForStudentModel._message = es.Message;
            }
            return siblingAddUpdateForStudentModel;
        }

        /// <summary>
        ///  Check Student InternalId Exist or Not
        /// </summary>
        /// <param name="checkStudentInternalIdViewModel"></param>
        /// <returns></returns>
        public CheckStudentInternalIdViewModel CheckStudentInternalId(CheckStudentInternalIdViewModel checkStudentInternalIdViewModel)
        {
            var checkInternalId = this.context?.StudentMaster.Where(x => x.TenantId == checkStudentInternalIdViewModel.TenantId && x.SchoolId == checkStudentInternalIdViewModel.SchoolId && x.StudentInternalId == checkStudentInternalIdViewModel.StudentInternalId).ToList();
            if (checkInternalId?.Any()==true)
            {
                checkStudentInternalIdViewModel.IsValidInternalId = false;
                checkStudentInternalIdViewModel._message = "Student Internal Id Already Exist";
            }
            else
            {
                checkStudentInternalIdViewModel.IsValidInternalId = true;
                checkStudentInternalIdViewModel._message = "Student Internal Id Is Valid";
            }
            return checkStudentInternalIdViewModel;
        }

        /// <summary>
        /// Add/Update Student Photo
        /// </summary>
        /// <param name="studentAddViewModel"></param>
        /// <returns></returns>
        public StudentAddViewModel AddUpdateStudentPhoto(StudentAddViewModel studentAddViewModel)
        {
            try
            {
                var studentUpdate = this.context?.StudentMaster.FirstOrDefault(x => x.TenantId == studentAddViewModel.studentMaster!.TenantId && x.SchoolId == studentAddViewModel.studentMaster.SchoolId && x.StudentId == studentAddViewModel.studentMaster.StudentId);
                if (studentUpdate != null)
                {
                    studentUpdate.UpdatedOn = DateTime.UtcNow;
                    studentUpdate.StudentPhoto = studentAddViewModel.studentMaster!.StudentPhoto;
                    studentUpdate.StudentThumbnailPhoto = studentAddViewModel.studentMaster!.StudentThumbnailPhoto;
                    studentUpdate.UpdatedBy = studentAddViewModel.studentMaster.UpdatedBy;
                    this.context?.SaveChanges();
                    studentAddViewModel._message = "Student Photo Updated Successfully";
                }
                else
                {
                    studentAddViewModel._failure = true;
                    studentAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                studentAddViewModel._failure = true;
                studentAddViewModel._message = es.Message;
            }
            return studentAddViewModel;
        }

        //public SearchStudentViewModel SearchStudentForSchedule(SearchStudentViewModel searchStudentViewModel)
        //{
        //    SearchStudentViewModel searchStudentView = new SearchStudentViewModel();
        //    IQueryable<StudentMaster> transactionIQ = null;
        //    try
        //    {
        //        var studentDataList = this.context?.StudentMaster.Include(x => x.StudentEnrollment).Where(x => x.TenantId == searchStudentViewModel.TenantId && x.SchoolId == searchStudentViewModel.SchoolId && (searchStudentViewModel.StudentId == null || (x.StudentId == searchStudentViewModel.StudentId)) && (searchStudentViewModel.AlternateId == null || (x.AlternateId == searchStudentViewModel.AlternateId)) && (searchStudentViewModel.SectionId == null || (x.SectionId == searchStudentViewModel.SectionId)) && (searchStudentViewModel.FirstLanguageId == null || (x.FirstLanguageId == searchStudentViewModel.FirstLanguageId))).Select(e => new StudentMaster
        //        {
        //            TenantId = e.TenantId,
        //            SchoolId = e.SchoolId,
        //            StudentId = e.StudentId,
        //            FirstGivenName = e.FirstGivenName,
        //            MiddleName = e.MiddleName,
        //            LastFamilyName = e.LastFamilyName,
        //            AlternateId = e.AlternateId,
        //            SectionId = e.SectionId,
        //            FirstLanguage = e.FirstLanguage,

        //            StudentEnrollment = e.StudentEnrollment.Where(d => d.IsActive == true).Select(s => new StudentEnrollment
        //            {
        //                GradeId = s.GradeId,

        //            }).ToList()
        //        });



        //        if (searchStudentViewModel.GradeId!=null)
        //        {
        //            var filterGradeLevelTitle = this.context?.StudentMaster.Include(x => x.StudentEnrollment).Where(x => x.StudentEnrollment.FirstOrDefault().GradeId == searchStudentViewModel.GradeId).AsQueryable();
        //            if (filterGradeLevelTitle.ToList()?.Any()==true)
        //            {
        //                transactionIQ = filterGradeLevelTitle;
        //            }
        //        }

        //        if (searchStudentViewModel.StudentName != null)
        //        {
        //            var name = searchStudentViewModel.StudentName.Split(" ");
        //            var firstName = name.First();
        //            var lastName = name.Last();
        //            searchStudentViewModel.StudentName = null;
        //            if (searchStudentViewModel.StudentName == null)
        //            {
        //                var a = this.context?.StudentMaster.Include(x => x.StudentEnrollment).Where(x => x.TenantId == searchStudentViewModel.TenantId && x.SchoolId == searchStudentViewModel.SchoolId && x.FirstGivenName.StartsWith(firstName.ToString()) && x.LastFamilyName.StartsWith(lastName.ToString()));
        //                transactionIQ = transactionIQ.Concat(a).Distinct();

        //            }
        //            else
        //            {
        //                var b = transactionIQ.Where(x => x.TenantId == searchStudentViewModel.TenantId && x.SchoolId == searchStudentViewModel.SchoolId && x.FirstGivenName.StartsWith(searchStudentViewModel.StudentName) || x.LastFamilyName.StartsWith(searchStudentViewModel.StudentName));
        //                transactionIQ = transactionIQ.Concat(b).Distinct();

        //            }
        //        }

        //        int totalCount = transactionIQ.Count();

        //        var xyz = transactionIQ.Select(e => new SearchStudentForView
        //        {
        //            TenantId = e.TenantId,
        //            SchoolId = e.SchoolId,
        //            StudentId = e.StudentId,
        //            StudentName = e.FirstGivenName +" " + e.LastFamilyName,
        //            AlternateId = e.AlternateId,
        //            GradeId = e.StudentEnrollment.FirstOrDefault().GradeId,
        //            SectionId = e.SectionId,
        //            FirstLanguageId = e.FirstLanguageId,
        //        }).Skip((searchStudentViewModel.PageNumber - 1) * searchStudentViewModel.PageSize).Take(searchStudentViewModel.PageSize);

        //        searchStudentView.searchStudentForView = xyz.ToList();


        //    }
        //    catch (Exception es)
        //    {
        //        searchStudentView._failure = true;
        //        searchStudentView._message = es.Message;
        //    }
        //    return searchStudentView;
        //}

        /// <summary>
        /// Search Student List For Reenroll
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public StudentListModel SearchStudentListForReenroll(PageResult pageResult)
        {
            StudentListModel studentListModel = new StudentListModel();
            List<StudentMaster> Student = new List<StudentMaster>();
            IQueryable<StudentMaster>? transactionIQ = null;
            try
            {
                var studentDataList = this.context?.StudentMaster.Include(y => y.SchoolMaster).Include(z => z.StudentEnrollment).Where(x => (pageResult.SchoolId > 0) ? x.SchoolId == pageResult.SchoolId && x.TenantId == pageResult.TenantId && x.IsActive == false : x.TenantId == pageResult.TenantId && x.IsActive == false).ToList();

                if (studentDataList?.Any() == true)
                {
                    List<Guid>? studentGuids = new();

                    foreach (var studentData in studentDataList)
                    {
                        if (!studentGuids.Contains(studentData.StudentGuid)) //for duplicate checking in search all school time
                        {
                            studentData.StudentEnrollment = studentData.StudentEnrollment.OrderByDescending(x => x.EnrollmentId).Take(1).ToList();
                            //var checkEnrolledStudent = this.context?.StudentEnrollment?.Where(x => x.TenantId == pageResult.TenantId && x.StudentGuid == studentData.StudentGuid).OrderByDescending(x => x.EnrollmentId).FirstOrDefault();

                            var checkEnrolledStudent = studentData.StudentEnrollment.FirstOrDefault();

                            if (checkEnrolledStudent != null && checkEnrolledStudent.ExitCode != null && checkEnrolledStudent.ExitCode == "Dropped Out" && checkEnrolledStudent.ExitDate != null && checkEnrolledStudent.ExitDate < DateTime.Today.Date)
                            {
                                Student.Add(studentData);
                                studentGuids.Add(studentData.StudentGuid);
                            }
                        }
                    }
                }

                if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                {
                    transactionIQ = Student.AsQueryable();
                }
                else
                {
                    string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                    if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                    {
                        transactionIQ = Student.AsQueryable().Where(x => x.FirstGivenName != null && x.FirstGivenName.ToLower().Contains(Columnvalue.ToLower()) ||
                                                                    x.MiddleName != null && x.MiddleName.ToLower().Contains(Columnvalue.ToLower()) ||
                                                                    x.LastFamilyName != null && x.LastFamilyName.ToLower().Contains(Columnvalue.ToLower()) ||
                                                                    x.StudentInternalId != null && x.StudentInternalId.ToLower().Contains(Columnvalue.ToLower()) ||
                                                                    x.MobilePhone != null && x.MobilePhone.Contains(Columnvalue) ||
                                                                    x.PersonalEmail != null && x.PersonalEmail.Contains(Columnvalue));

                        //for StudentEnrollment Searching

                        var studentEnrollmentFilter = Student.AsQueryable().AsNoTracking().ToList().Where(x => x.StudentEnrollment.ToList()?.Any() == true ? (x.StudentEnrollment.FirstOrDefault()?.GradeLevelTitle != null && (x.StudentEnrollment.FirstOrDefault()?.GradeLevelTitle ?? "").ToLower().Contains(Columnvalue.ToLower())) : string.Empty.Contains(Columnvalue)).AsQueryable();

                        if (studentEnrollmentFilter.ToList()?.Any() == true)
                        {
                            transactionIQ = transactionIQ.AsNoTracking().ToList().Concat(studentEnrollmentFilter).AsQueryable();
                        }

                        //for school name Searching

                        var schoolNameFilter = Student.Where(x => x.SchoolMaster != null ? (x.SchoolMaster.SchoolName != null && (x.SchoolMaster.SchoolName).ToLower().Contains(Columnvalue.ToLower())) : string.Empty.Contains(Columnvalue)).AsQueryable();

                        if (schoolNameFilter.ToList()?.Any() == true)
                        {
                            transactionIQ = transactionIQ.AsNoTracking().ToList().Concat(schoolNameFilter).AsQueryable();
                        }
                    }
                    else
                    {
                        if (pageResult.FilterParams!.Any(x => x.ColumnName.ToLower() == "enrollmentdate" || x.ColumnName.ToLower() == "exitdate" || x.ColumnName.ToLower() == "exitcode" || x.ColumnName.ToLower() == "gradeid"))
                        {

                            var enrollmentData = Student.AsQueryable();
                            foreach (var filterParam in pageResult.FilterParams!)
                            {
                                if (filterParam.ColumnName.ToLower() == "enrollmentdate" || filterParam.ColumnName.ToLower() == "exitdate" || filterParam.ColumnName.ToLower() == "exitcode" || filterParam.ColumnName.ToLower() == "gradeid")
                                {
                                    var columnName = filterParam.ColumnName;
                                    string filterValue = filterParam.FilterValue;

                                    if (filterValue != null)
                                    {
                                        if (columnName.ToLower() == "enrollmentdate")
                                        {
                                            enrollmentData = enrollmentData.AsQueryable().AsNoTracking().ToList().Where(x => x.StudentEnrollment.FirstOrDefault()!.EnrollmentDate == Convert.ToDateTime(filterValue)).AsQueryable();
                                        }
                                        if (columnName.ToLower() == "exitdate")
                                        {
                                            enrollmentData = enrollmentData.AsQueryable().AsNoTracking().ToList().Where(x => x.StudentEnrollment.FirstOrDefault()!.ExitDate == Convert.ToDateTime(filterValue)).AsQueryable();
                                        }
                                        if (columnName.ToLower() == "exitcode")
                                        {
                                            enrollmentData = enrollmentData.AsQueryable().AsNoTracking().ToList().Where(x =>
                                            String.Compare(x.StudentEnrollment.FirstOrDefault()!.ExitCode, filterValue, true) == 0
                                            ).AsQueryable();
                                        }
                                        if (columnName.ToLower() == "gradeid")
                                        {
                                            enrollmentData = enrollmentData.AsQueryable().AsNoTracking().ToList().Where(x => x.StudentEnrollment.FirstOrDefault()!.GradeId == Convert.ToInt32(filterValue)).AsQueryable();
                                        }
                                    }
                                }
                            }

                            pageResult.FilterParams.RemoveAll(x => x.ColumnName.ToLower() == "enrollmentdate" || x.ColumnName.ToLower() == "exitdate" || x.ColumnName.ToLower() == "exitcode" || x.ColumnName.ToLower() == "gradeid");

                            if (pageResult.FilterParams?.Any() == true)
                            {
                                transactionIQ = Utility.FilteredData(pageResult.FilterParams, enrollmentData).AsQueryable();
                            }
                            else
                            {
                                transactionIQ = enrollmentData;
                            }
                        }
                        else
                        {
                            transactionIQ = Utility.FilteredData(pageResult.FilterParams!, Student).AsQueryable();

                            //medical advance search
                            var studentGuids = transactionIQ.Select(s => s.StudentGuid).ToList();
                            if (studentGuids.Count > 0)
                            {
                                int? schoolId = pageResult.SearchAllSchool == true ? null : pageResult.SchoolId;
                                var filterStudentIds = Utility.MedicalAdvancedSearch(this.context!, pageResult.FilterParams!, pageResult.TenantId, schoolId, studentGuids);

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
                    }
                }

                

                if (transactionIQ != null)
                {
                    if (pageResult.SortingModel != null)
                    {
                        switch ((pageResult.SortingModel.SortColumn ?? "").ToLower())
                        {
                            //For Schoolname Sorting
                            case "schoolname":

                                if (pageResult.SortingModel.SortDirection.ToLower() == "asc")
                                {

                                    transactionIQ = transactionIQ.AsNoTracking().ToList().OrderBy(a => a.SchoolMaster != null ? a.SchoolMaster.SchoolName : null).AsQueryable();
                                }
                                else
                                {

                                    transactionIQ = transactionIQ.AsNoTracking().ToList().OrderByDescending(a => a.SchoolMaster != null ? a.SchoolMaster.SchoolName : null).AsQueryable();
                                }
                                break;

                            //For GradeLevel Sorting
                            case "gradeleveltitle":

                                if (pageResult.SortingModel.SortDirection.ToLower() == "asc")
                                {

                                    transactionIQ = transactionIQ.AsNoTracking().ToList().OrderBy(a => a.StudentEnrollment?.Any() == true ? a.StudentEnrollment.FirstOrDefault()?.GradeLevelTitle : null).AsQueryable();
                                }
                                else
                                {

                                    transactionIQ = transactionIQ.AsNoTracking().ToList().OrderByDescending(a => a.StudentEnrollment?.Any() == true ? a.StudentEnrollment.FirstOrDefault()?.GradeLevelTitle : null).AsQueryable();
                                }
                                break;

                            //For Student Enrollment Date Sorting
                            case "enrollmentdate":

                                if (pageResult.SortingModel.SortDirection.ToLower() == "asc")
                                {

                                    transactionIQ = transactionIQ.AsNoTracking().ToList().OrderBy(a => a.StudentEnrollment?.Any() == true ? a.StudentEnrollment.FirstOrDefault()?.EnrollmentDate : null).AsQueryable();
                                }
                                else
                                {

                                    transactionIQ = transactionIQ.AsNoTracking().ToList().OrderByDescending(a => a.StudentEnrollment?.Any() == true ? a.StudentEnrollment.FirstOrDefault()?.EnrollmentDate : null).AsQueryable();
                                }
                                break;

                            //For Student Enrollment Exit Date Sorting
                            case "exitdate":

                                if (pageResult.SortingModel.SortDirection.ToLower() == "asc")
                                {

                                    transactionIQ = transactionIQ.AsNoTracking().ToList().OrderBy(a => a.StudentEnrollment?.Any() == true ? a.StudentEnrollment.FirstOrDefault()?.ExitDate : null).AsQueryable();
                                }
                                else
                                {

                                    transactionIQ = transactionIQ.AsNoTracking().ToList().OrderByDescending(a => a.StudentEnrollment?.Any() == true ? a.StudentEnrollment.FirstOrDefault()?.ExitDate : null).AsQueryable();
                                }
                                break;

                            //For Student Enrollment Exit Code Sorting
                            case "exitcode":

                                if (pageResult.SortingModel.SortDirection.ToLower() == "asc")
                                {

                                    transactionIQ = transactionIQ.AsNoTracking().ToList().OrderBy(a => a.StudentEnrollment?.Any() == true ? a.StudentEnrollment.FirstOrDefault()?.ExitCode : null).AsQueryable();
                                }
                                else
                                {

                                    transactionIQ = transactionIQ.AsNoTracking().ToList().OrderByDescending(a => a.StudentEnrollment?.Any() == true ? a.StudentEnrollment.FirstOrDefault()?.ExitCode : null).AsQueryable();
                                }
                                break;

                            case "lastfamilyname":

                                if (pageResult.SortingModel.SortDirection?.ToLower() == "asc")
                                {
                                    transactionIQ = transactionIQ?.OrderBy(a => a.LastFamilyName).ThenBy(a => (a.PreferredName != null && a.PreferredName != "") ? a.PreferredName : a.FirstGivenName);
                                }
                                else
                                {
                                    transactionIQ = transactionIQ?.OrderByDescending(a => a.LastFamilyName).ThenByDescending(a => (a.PreferredName != null && a.PreferredName != "") ? a.PreferredName : a.FirstGivenName);
                                }
                                break;

                            default:
                                transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn ?? "", pageResult.SortingModel.SortDirection.ToLower());
                                break;
                        }
                    }
                    else
                    {
                        transactionIQ = transactionIQ.OrderBy(s => s.LastFamilyName).ThenBy(a => (a.PreferredName != null && a.PreferredName != "") ? a.PreferredName : a.FirstGivenName);
                    }
                }

                if (transactionIQ != null)
                {
                    int totalCount = transactionIQ.AsNoTracking().ToList().Count();
                    if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                    {
                        transactionIQ = transactionIQ.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                    }
                    studentListModel.studentMaster = transactionIQ.ToList();

                    studentListModel.studentMaster.ForEach(x => x.SchoolMaster.StudentMaster = new HashSet<StudentMaster>());

                    studentListModel.TotalCount = totalCount;
                }
                else
                {
                    studentListModel.TotalCount = 0;
                }

                studentListModel.TenantId = pageResult.TenantId;
                studentListModel.SchoolId = pageResult.SchoolId;
                studentListModel.PageNumber = pageResult.PageNumber;
                studentListModel._pageSize = pageResult.PageSize;
                studentListModel._tenantName = pageResult._tenantName;
                studentListModel._token = pageResult._token;
                studentListModel._failure = false;
            }
            catch (Exception es)
            {
                studentListModel._message = es.Message;
                studentListModel._failure = true;
                studentListModel._tenantName = pageResult._tenantName;
                studentListModel._token = pageResult._token;
            }
            return studentListModel;
        }

        /// <summary>
        /// Re enrollment For Student
        /// </summary>
        /// <param name="studentListModel"></param>
        /// <returns></returns>
        public StudentListModel ReenrollmentForStudent(StudentListModel studentListModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    if (studentListModel.studentMaster?.Any() == true)
                    {
                        bool atleastOneStudentReEnroll = false;
                        studentListModel._failure = false;
                        studentListModel._message = "Student re-enrollment added successfully";

                        studentListModel.AcademicYear = Utility.GetCurrentAcademicYear(this.context!, studentListModel.TenantId, studentListModel.SchoolId).ToString(); //fetch current academic year

                        int? calenderId = null;
                        var defaultCalender = this.context?.SchoolCalendars.FirstOrDefault(x => x.TenantId == studentListModel.TenantId && x.SchoolId == studentListModel.SchoolId && x.AcademicYear.ToString() == studentListModel.AcademicYear && x.DefaultCalender == true);

                        if (defaultCalender != null)
                        {
                            calenderId = defaultCalender.CalenderId;
                        }

                        var enrollmenttitle = this.context?.StudentEnrollmentCode.FirstOrDefault(x => x.TenantId == studentListModel.TenantId && x.SchoolId == studentListModel.SchoolId && x.EnrollmentCode == studentListModel.EnrollmentCode)?.Title;

                        foreach (var studentData in studentListModel.studentMaster)
                        {
                            var activeStudentEnrollment = this.context?.StudentEnrollment.Where(s => s.TenantId == studentListModel.TenantId && s.SchoolId == studentListModel.SchoolId && s.StudentGuid == studentData.StudentGuid).OrderByDescending(x => x.EnrollmentId).FirstOrDefault();

                            if (activeStudentEnrollment != null && (activeStudentEnrollment.ExitDate == null || activeStudentEnrollment.ExitDate > DateTime.Today.Date)) //this for students already enrolled in this school in future date
                            {
                                //studentListModel._failure = false;
                                //studentListModel._message = "One or more students already enrolled in this school";
                                studentListModel.ConflictStudentsName.Add($"{studentData.FirstGivenName} { (studentData.MiddleName == null ? " " : $"{studentData.MiddleName} ")}{studentData.LastFamilyName}");
                            }
                            else
                            {
                                var lastStudentEnrollment = this.context?.StudentEnrollment.Where(s => s.TenantId == studentListModel.TenantId && s.SchoolId == studentListModel.SchoolId && s.StudentGuid == studentData.StudentGuid && s.IsActive == true).FirstOrDefault();

                                if (lastStudentEnrollment != null && lastStudentEnrollment.ExitDate != null && lastStudentEnrollment.ExitDate.Value.Date > studentListModel.EnrollmentDate!.Value.Date) // this for students last enrolled exit date grater then now enrolled date
                                {
                                    studentListModel.ConflictStudentsName.Add($"{studentData.FirstGivenName} { (studentData.MiddleName == null ? " " : $"{studentData.MiddleName} ")}{studentData.LastFamilyName}");
                                }
                                else
                                {
                                    this.context?.StudentEnrollment.Where(x => x.StudentGuid == studentData.StudentGuid && x.IsActive == true && x.ExitDate != null).ToList().ForEach(x => { x.IsActive = false; });

                                    int? EnrollmentId = 1;

                                    var studentEnrollmentData = this.context?.StudentEnrollment.Where(x => x.StudentGuid == studentData.StudentGuid).OrderByDescending(x => x.EnrollmentId).FirstOrDefault();

                                    if (studentEnrollmentData != null)
                                    {
                                        EnrollmentId = studentEnrollmentData.EnrollmentId + 1;
                                    }

                                    var existingStudentData = this.context?.StudentMaster.FirstOrDefault(s => s.TenantId == studentListModel.TenantId && s.SchoolId == studentListModel.SchoolId && s.StudentGuid == studentData.StudentGuid); //check student exists in school or not.

                                    if (existingStudentData != null)
                                    {
                                        //if student re enroll today or previous date
                                        if (studentListModel.EnrollmentDate != null && studentListModel.EnrollmentDate.Value.Date <= DateTime.Today.Date)
                                        {
                                            existingStudentData.IsActive = true;
                                        }

                                        var StudentEnrollmentData = new StudentEnrollment()
                                        {
                                            TenantId = studentData.TenantId,
                                            SchoolId = (int)studentListModel.SchoolId!,
                                            StudentId = existingStudentData.StudentId,
                                            EnrollmentId = (int)EnrollmentId,
                                            EnrollmentCode = enrollmenttitle,
                                            EnrollmentDate = studentListModel.EnrollmentDate,
                                            GradeLevelTitle = studentListModel.GradeLevelTitle,
                                            GradeId = studentListModel.GradeId,
                                            UpdatedOn = DateTime.UtcNow,
                                            RollingOption = "Next Grade at Current School",
                                            StudentGuid = studentData.StudentGuid,
                                            IsActive = true,
                                            SchoolName = this.context?.SchoolMaster.FirstOrDefault(x => x.TenantId == studentListModel.TenantId && x.SchoolId == studentListModel.SchoolId)?.SchoolName,
                                            UpdatedBy = studentListModel.UpdatedBy,
                                            CalenderId = calenderId
                                        };
                                        this.context?.StudentEnrollment.Add(StudentEnrollmentData);
                                        atleastOneStudentReEnroll = true;
                                    }
                                    else
                                    {
                                        var studentInfo = this.context?.StudentMaster.FirstOrDefault(x => x.StudentGuid == studentData.StudentGuid);

                                        int? MasterStudentId = 1;

                                        var studentId = this.context?.StudentMaster.Where(x => x.SchoolId == studentListModel.SchoolId && x.TenantId == studentListModel.TenantId).OrderByDescending(x => x.StudentId).FirstOrDefault();

                                        if (studentId != null)
                                        {
                                            MasterStudentId = studentId.StudentId + 1;
                                        }

                                        var StudentMasterData = new StudentMaster()
                                        {
                                            TenantId = studentInfo!.TenantId,
                                            SchoolId = (int)studentListModel!.SchoolId!,
                                            StudentId = (int)MasterStudentId,
                                            AlternateId = studentInfo.AlternateId,
                                            DistrictId = studentInfo.DistrictId,
                                            StateId = studentInfo.StateId,
                                            AdmissionNumber = studentInfo.AdmissionNumber,
                                            RollNumber = studentInfo.RollNumber,
                                            Salutation = studentInfo.Salutation,
                                            FirstGivenName = studentInfo.FirstGivenName,
                                            MiddleName = studentInfo.MiddleName,
                                            LastFamilyName = studentInfo.LastFamilyName,
                                            Suffix = studentInfo.Suffix,
                                            PreferredName = studentInfo.PreferredName,
                                            PreviousName = studentInfo.PreviousName,
                                            SocialSecurityNumber = studentInfo.SocialSecurityNumber,
                                            OtherGovtIssuedNumber = studentInfo.OtherGovtIssuedNumber,
                                            StudentPhoto = studentInfo.StudentPhoto,
                                            StudentThumbnailPhoto = studentInfo.StudentThumbnailPhoto,
                                            Dob = studentInfo.Dob,
                                            Gender = studentInfo.Gender,
                                            Race = studentInfo.Race,
                                            Ethnicity = studentInfo.Ethnicity,
                                            MaritalStatus = studentInfo.MaritalStatus,
                                            CountryOfBirth = studentInfo.CountryOfBirth,
                                            Nationality = studentInfo.Nationality,
                                            FirstLanguageId = studentInfo.FirstLanguageId,
                                            SecondLanguageId = studentInfo.SecondLanguageId,
                                            ThirdLanguageId = studentInfo.ThirdLanguageId,
                                            HomePhone = studentInfo.HomePhone,
                                            MobilePhone = studentInfo.MobilePhone,
                                            PersonalEmail = studentInfo.PersonalEmail,
                                            SchoolEmail = studentInfo.SchoolEmail,
                                            Twitter = studentInfo.Twitter,
                                            Facebook = studentInfo.Facebook,
                                            Instagram = studentInfo.Instagram,
                                            Youtube = studentInfo.Youtube,
                                            Linkedin = studentInfo.Linkedin,
                                            HomeAddressLineOne = studentInfo.HomeAddressLineOne,
                                            HomeAddressLineTwo = studentInfo.HomeAddressLineTwo,
                                            HomeAddressCountry = studentInfo.HomeAddressCountry,
                                            HomeAddressState = studentInfo.HomeAddressState,
                                            HomeAddressCity = studentInfo.HomeAddressCity,
                                            HomeAddressZip = studentInfo.HomeAddressZip,
                                            BusNo = studentInfo.BusNo,
                                            SchoolBusPickUp = studentInfo.SchoolBusPickUp,
                                            SchoolBusDropOff = studentInfo.SchoolBusDropOff,
                                            MailingAddressSameToHome = studentInfo.MailingAddressSameToHome,
                                            MailingAddressLineOne = studentInfo.MailingAddressLineOne,
                                            MailingAddressLineTwo = studentInfo.MailingAddressLineTwo,
                                            MailingAddressCountry = studentInfo.MailingAddressCountry,
                                            MailingAddressState = studentInfo.MailingAddressState,
                                            MailingAddressCity = studentInfo.MailingAddressCity,
                                            MailingAddressZip = studentInfo.MailingAddressZip,
                                            StudentPortalId = studentInfo.StudentPortalId,
                                            AlertDescription = studentInfo.AlertDescription,
                                            CriticalAlert = studentInfo.CriticalAlert,
                                            Dentist = studentInfo.Dentist,
                                            DentistPhone = studentInfo.DentistPhone,
                                            InsuranceCompany = studentInfo.InsuranceCompany,
                                            InsuranceCompanyPhone = studentInfo.InsuranceCompanyPhone,
                                            MedicalFacility = studentInfo.MedicalFacility,
                                            MedicalFacilityPhone = studentInfo.MedicalFacilityPhone,
                                            PolicyHolder = studentInfo.PolicyHolder,
                                            PolicyNumber = studentInfo.PolicyNumber,
                                            PrimaryCarePhysician = studentInfo.PrimaryCarePhysician,
                                            PrimaryCarePhysicianPhone = studentInfo.PrimaryCarePhysicianPhone,
                                            Vision = studentInfo.Vision,
                                            VisionPhone = studentInfo.VisionPhone,
                                            Associationship = studentInfo.Associationship,
                                            EconomicDisadvantage = studentInfo.EconomicDisadvantage,
                                            Eligibility504 = studentInfo.Eligibility504,
                                            EstimatedGradDate = studentInfo.EstimatedGradDate,
                                            FreeLunchEligibility = studentInfo.FreeLunchEligibility,
                                            LepIndicator = studentInfo.LepIndicator,
                                            SectionId = null,
                                            SpecialEducationIndicator = studentInfo.SpecialEducationIndicator,
                                            StudentInternalId = studentInfo.StudentInternalId,
                                            UpdatedOn = DateTime.UtcNow,
                                            UpdatedBy = studentInfo.UpdatedBy,
                                            EnrollmentType = "Internal",
                                            IsActive = studentListModel.EnrollmentDate != null && studentListModel.EnrollmentDate.Value.Date <= DateTime.Today.Date ? true : false,
                                            StudentGuid = studentData.StudentGuid

                                        };
                                        this.context?.StudentMaster.Add(StudentMasterData);

                                        var StudentEnrollmentData = new StudentEnrollment()
                                        {
                                            TenantId = studentData.TenantId,
                                            SchoolId = (int)studentListModel.SchoolId,
                                            StudentId = (int)MasterStudentId,
                                            EnrollmentId = (int)EnrollmentId,
                                            EnrollmentCode = enrollmenttitle,
                                            EnrollmentDate = studentListModel.EnrollmentDate,
                                            GradeLevelTitle = studentListModel.GradeLevelTitle,
                                            UpdatedOn = DateTime.UtcNow,
                                            RollingOption = "Next Grade at Current School",
                                            StudentGuid = studentData.StudentGuid,
                                            IsActive = true,
                                            SchoolName = this.context?.SchoolMaster.FirstOrDefault(x => x.TenantId == studentListModel.TenantId && x.SchoolId == studentListModel.SchoolId)?.SchoolName,
                                            UpdatedBy = studentListModel.UpdatedBy,
                                            GradeId = studentListModel.GradeId,
                                            CalenderId = calenderId
                                        };
                                        this.context?.StudentEnrollment.Add(StudentEnrollmentData);
                                        atleastOneStudentReEnroll = true;
                                    }

                                    if (studentListModel.EnrollmentDate != null && studentListModel.EnrollmentDate.Value.Date <= DateTime.Today.Date)
                                    {
                                        //update StudentCoursesectionSchedule data if exits.
                                        var studentCourseSection = this.context?.StudentCoursesectionSchedule.Include(s => s.CourseSection).Where(x => x.TenantId == studentListModel.TenantId && x.SchoolId == studentListModel.SchoolId && x.StudentGuid == studentData.StudentGuid && x.IsDropped == true && x.AcademicYear.ToString() == studentListModel.AcademicYear).ToList();
                                        if (studentCourseSection?.Any() == true)
                                        {
                                            studentCourseSection.ForEach(s => { s.IsDropped = null; s.EffectiveDropDate = s.CourseSection.DurationEndDate; });
                                        }
                                    }

                                    this.context?.SaveChanges();
                                }
                            }
                        }

                        if (studentListModel.EnrollmentDate != null && studentListModel.EnrollmentDate.Value.Date > DateTime.Today.Date && atleastOneStudentReEnroll == true)
                        {
                            //for insert in job fetch max id.
                            long? Id = 1;
                            var dataExits = this.context?.ScheduledJobs.Where(x => x.TenantId == studentListModel.TenantId);
                            if (dataExits?.Any() == true)
                            {
                                var scheduledJobData = this.context?.ScheduledJobs.Where(x => x.TenantId == studentListModel.TenantId).Max(x => x.JobId);
                                if (scheduledJobData != null)
                                {
                                    Id = scheduledJobData + 1;
                                }
                            }

                            // if student re enroll future date & this block for add this req as a job
                            var scheduledJob = new ScheduledJob
                            {
                                TenantId = (Guid)studentListModel.TenantId!,
                                SchoolId = (int)studentListModel.SchoolId!,
                                JobId = (long)Id,
                                AcademicYear = defaultCalender?.AcademicYear,
                                JobTitle = "ReenrollmentForStudent",
                                JobScheduleDate = studentListModel.EnrollmentDate,
                                ApiTitle = "ReenrollmentForStudent",
                                ControllerPath = studentListModel._tenantName + "/Student",
                                TaskJson = JsonConvert.SerializeObject(studentListModel),
                                LastRunStatus = null,
                                LastRunTime = null,
                                IsActive = true,
                                CreatedBy = studentListModel.UpdatedBy,
                                CreatedOn = DateTime.UtcNow
                            };
                            this.context?.ScheduledJobs.Add(scheduledJob);
                        }

                        this.context?.SaveChanges();
                        transaction?.Commit();

                        if (studentListModel.ConflictStudentsName?.Any() == true)
                        {
                            studentListModel._failure = true;
                        }
                    }
                    else
                    {
                        studentListModel._failure = true;
                        studentListModel._message = "Atleast select one student";
                    }
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    studentListModel._message = es.Message;
                    studentListModel._failure = true;
                }
                return studentListModel;
            }
        }

        /// <summary>
        /// Add Student List
        /// </summary>
        /// <param name="studentListAddViewModel"></param>
        /// <returns></returns>
        public StudentListAddViewModel AddStudentList(StudentListAddViewModel studentListAddViewModel)
        {
            StudentListAddViewModel studentListAdd = new StudentListAddViewModel();
            int number;
            studentListAdd._tenantName = studentListAddViewModel._tenantName;
            studentListAdd._token = studentListAddViewModel._token;
            studentListAdd._userName = studentListAddViewModel._userName;

            if (studentListAddViewModel.studentAddViewModelList?.Any()==true)
            {
                studentListAdd._failure = false;
                studentListAdd._message = "Student added successfully";
                int? MasterStudentId = 1;

                var studentData = this.context?.StudentMaster.Where(x => x.SchoolId == studentListAddViewModel.SchoolId && x.TenantId == studentListAddViewModel.TenantId).OrderByDescending(x => x.StudentId).FirstOrDefault();

                if (studentData != null)
                {
                    MasterStudentId = studentData.StudentId + 1;
                }

                var academicYear = Utility.GetCurrentAcademicYear(this.context!, studentListAddViewModel.TenantId, studentListAddViewModel.SchoolId);

                var schoolYearStartDate = this.context?.SchoolYears.Where(x => x.TenantId == studentListAddViewModel.TenantId && x.SchoolId == studentListAddViewModel.SchoolId && x.AcademicYear == academicYear).Min(s => s.StartDate);

                int? indexNo = -1;
                foreach (var student in studentListAddViewModel.studentAddViewModelList)
                {
                    if (student.studentMaster != null)
                    {
                        indexNo++;
                        student.AcademicYear = academicYear.ToString();

                        //UserMaster userMaster = new UserMaster();
                        var StudentEnrollmentData = new StudentEnrollment();
                        using (var transaction = this.context?.Database.BeginTransaction())
                        {
                            try
                            {
                                DateTime? studentDob = null;
                                if (!string.IsNullOrEmpty(student.Dob))
                                {
                                    studentDob = Convert.ToDateTime(student.Dob);
                                }
                                var checkMessage = Utility.checkDuplicate(this.context, studentListAddViewModel.TenantId, studentListAddViewModel.SchoolId, student.studentMaster.Salutation, student.studentMaster.FirstGivenName, student.studentMaster.MiddleName, student.studentMaster.LastFamilyName, student.studentMaster.Suffix, studentDob, student.studentMaster.PersonalEmail, student.studentMaster.SocialSecurityNumber, "student", null);

                                if (checkMessage != null)
                                {
                                    studentListAdd.ConflictIndexNo = studentListAdd.ConflictIndexNo != null ? studentListAdd.ConflictIndexNo + "," + indexNo.ToString() : indexNo.ToString();
                                    student._message = "Duplicate Data Found";
                                    studentListAdd.studentAddViewModelList.Add(student);
                                    studentListAdd._failure = true;
                                    studentListAdd._message = "Student Rejected Due To Duplicate Data";
                                    continue;
                                }

                                student.studentMaster.TenantId = studentListAddViewModel.TenantId;
                                student.studentMaster.SchoolId = studentListAddViewModel.SchoolId;
                                student.studentMaster.StudentId = (int)MasterStudentId;
                                student.studentMaster.CreatedOn = DateTime.UtcNow;
                                student.studentMaster.CreatedBy = studentListAddViewModel.CreatedBy;
                                Guid GuidId = Guid.NewGuid();
                                var GuidIdExist = this.context?.StudentMaster.FirstOrDefault(x => x.StudentGuid == GuidId);

                                if (GuidIdExist != null)
                                {
                                    studentListAdd.ConflictIndexNo = studentListAdd.ConflictIndexNo != null ? studentListAdd.ConflictIndexNo + "," + indexNo.ToString() : indexNo.ToString();
                                    student._message = "GUID Already Exist";
                                    studentListAdd.studentAddViewModelList.Add(student);
                                    studentListAdd._failure = true;
                                    studentListAdd._message = "Student Rejected Due to Data Error";
                                    continue;
                                }

                                student.studentMaster.StudentGuid = GuidId;
                                student.studentMaster.IsActive = true;
                                student.studentMaster.EnrollmentType = "Internal";

                                if (!string.IsNullOrEmpty(student.studentMaster.StudentInternalId))
                                {
                                    bool checkInternalID = CheckInternalID(student.studentMaster.TenantId, student.studentMaster.StudentInternalId, student.studentMaster.SchoolId);
                                    if (checkInternalID == false)
                                    {
                                        studentListAdd.ConflictIndexNo = studentListAdd.ConflictIndexNo != null ? studentListAdd.ConflictIndexNo + "," + indexNo.ToString() : indexNo.ToString();
                                        student._message = "Student Id Already Exist";
                                        studentListAdd.studentAddViewModelList.Add(student);
                                        studentListAdd._failure = true;
                                        studentListAdd._message = "Student Rejected Due to Data Error";
                                        continue;
                                    }
                                }
                                else
                                {
                                    student.studentMaster.StudentInternalId = MasterStudentId.ToString();
                                }

                                if (student.FirstLanguageName != null)
                                {
                                    var checkItsId = Int32.TryParse(student.FirstLanguageName, out number);
                                    if (checkItsId)
                                    {
                                        var firstLanguageData = this.context?.Language.FirstOrDefault(x => x.LangId == Convert.ToInt32(student.FirstLanguageName));
                                        student.studentMaster.FirstLanguageId = firstLanguageData != null ? Convert.ToInt32(student.FirstLanguageName) : (int?)null;
                                    }
                                    else
                                    {
                                        var firstLanguageId = this.context?.Language.FirstOrDefault(x => x.Locale.ToLower() == student.FirstLanguageName.ToLower())?.LangId;
                                        student.studentMaster.FirstLanguageId = firstLanguageId != null ? firstLanguageId : null;
                                    }
                                }

                                if (student.SecondLanguageName != null)
                                {
                                    var checkItsId = Int32.TryParse(student.SecondLanguageName, out number);
                                    if (checkItsId)
                                    {
                                        var SecondLanguageData = this.context?.Language.FirstOrDefault(x => x.LangId == Convert.ToInt32(student.SecondLanguageName));
                                        student.studentMaster.SecondLanguageId = SecondLanguageData != null ? Convert.ToInt32(student.SecondLanguageName) : (int?)null;
                                    }
                                    else
                                    {
                                        var secondLanguageId = this.context?.Language.FirstOrDefault(x => x.Locale.ToLower() == student.SecondLanguageName.ToLower())?.LangId;
                                        student.studentMaster.SecondLanguageId = secondLanguageId != null ? secondLanguageId : null;
                                    }
                                }

                                if (student.ThirdLanguageName != null)
                                {
                                    var checkItsId = Int32.TryParse(student.ThirdLanguageName, out number);
                                    if (checkItsId)
                                    {
                                        var ThirdLanguageData = this.context?.Language.FirstOrDefault(x => x.LangId == Convert.ToInt32(student.ThirdLanguageName));
                                        student.studentMaster.ThirdLanguageId = ThirdLanguageData != null ? Convert.ToInt32(student.ThirdLanguageName) : (int?)null;
                                    }
                                    else
                                    {
                                        var thirdLanguageId = this.context?.Language.FirstOrDefault(x => x.Locale.ToLower() == student.ThirdLanguageName.ToLower())?.LangId;
                                        student.studentMaster.ThirdLanguageId = thirdLanguageId != null ? thirdLanguageId : null;
                                    }
                                }

                                if (student.CountryOfBirthName != null)
                                {
                                    var checkItsId = Int32.TryParse(student.CountryOfBirthName, out number);
                                    if (checkItsId)
                                    {
                                        var CountryOfBirthData = this.context?.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(student.CountryOfBirthName));
                                        student.studentMaster.CountryOfBirth = CountryOfBirthData != null ? Convert.ToInt32(student.CountryOfBirthName) : (int?)null;
                                    }
                                    else
                                    {
                                        var countryOfBirthId = this.context?.Country.FirstOrDefault(x => x.Name.ToLower() == student.CountryOfBirthName.ToLower())?.Id;
                                        student.studentMaster.CountryOfBirth = countryOfBirthId != null ? countryOfBirthId : null;
                                    }
                                }

                                if (student.NationalityName != null)
                                {
                                    var checkItsId = Int32.TryParse(student.NationalityName, out number);
                                    if (checkItsId)
                                    {
                                        var NationalityData = this.context?.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(student.NationalityName));
                                        student.studentMaster.Nationality = NationalityData != null ? Convert.ToInt32(student.NationalityName) : (int?)null;
                                    }
                                    else
                                    {
                                        var nationalityId = this.context?.Country.FirstOrDefault(x => x.Name.ToLower() == student.NationalityName.ToLower())?.Id;
                                        student.studentMaster.Nationality = nationalityId != null ? nationalityId : null;
                                    }
                                }

                                if (student.SectionName != null)
                                {
                                    var checkItsId = Int32.TryParse(student.SectionName, out number);
                                    if (checkItsId)
                                    {
                                        var SectionData = this.context?.Sections.FirstOrDefault(x => x.SectionId == Convert.ToInt32(student.SectionName) && x.SchoolId == studentListAddViewModel.SchoolId && x.TenantId == studentListAddViewModel.TenantId);
                                        student.studentMaster.SectionId = SectionData != null ? Convert.ToInt32(student.SectionName) : (int?)null;
                                    }
                                    else
                                    {
                                        var sectionId = this.context?.Sections.FirstOrDefault(x => x.Name.ToLower() == student.SectionName.ToLower() && x.TenantId == studentListAddViewModel.TenantId && x.SchoolId == studentListAddViewModel.SchoolId)?.SectionId;
                                        student.studentMaster.SectionId = sectionId != null ? sectionId : null;
                                    }
                                }

                                if (student.studentMaster.HomeAddressCountry != null)
                                {
                                    var checkItsId = Int32.TryParse(student.studentMaster.HomeAddressCountry, out number);
                                    if (checkItsId)
                                    {
                                        var CountryData = this.context?.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(student.studentMaster.HomeAddressCountry));
                                        student.studentMaster.HomeAddressCountry = CountryData != null ? CountryData.Id.ToString() : null;
                                    }
                                    else
                                    {
                                        var CountryData = this.context?.Country.FirstOrDefault(x => x.Name.ToLower() == student.studentMaster.HomeAddressCountry.ToLower());
                                        student.studentMaster.HomeAddressCountry = CountryData != null ? CountryData.Id.ToString() : null;
                                    }
                                }
                                if (student.studentMaster.MailingAddressCountry != null)
                                {
                                    var checkItsId = Int32.TryParse(student.studentMaster.MailingAddressCountry, out number);
                                    if (checkItsId)
                                    {
                                        var CountryData = this.context?.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(student.studentMaster.MailingAddressCountry));
                                        student.studentMaster.MailingAddressCountry = CountryData != null ? CountryData.Id.ToString() : null;
                                    }
                                    else
                                    {
                                        var CountryData = this.context?.Country.FirstOrDefault(x => x.Name.ToLower() == student.studentMaster.MailingAddressCountry.ToLower());
                                        student.studentMaster.MailingAddressCountry = CountryData != null ? CountryData.Id.ToString() : null;
                                    }
                                }


                                var schoolName = this.context?.SchoolMaster.Where(x => x.TenantId == student.studentMaster.TenantId && x.SchoolId == student.studentMaster.SchoolId).Select(s => s.SchoolName).FirstOrDefault();

                                //Insert data into Enrollment table
                                int? calenderId = null;
                                string? enrollmentCode = null;

                                var defaultCalender = this.context?.SchoolCalendars.FirstOrDefault(x => x.TenantId == student.studentMaster.TenantId && x.SchoolId == student.studentMaster.SchoolId && x.AcademicYear.ToString() == student.AcademicYear && x.DefaultCalender == true);

                                if (defaultCalender != null)
                                {
                                    calenderId = defaultCalender.CalenderId;
                                }

                                var enrollmentType = this.context?.StudentEnrollmentCode.FirstOrDefault(x => x.TenantId == student.studentMaster.TenantId && x.SchoolId == student.studentMaster.SchoolId && x.Type.ToLower() == "Add".ToLower());

                                if (enrollmentType != null)
                                {
                                    enrollmentCode = enrollmentType.Title;
                                }
                                if (student.CurrentGradeLevel != null)
                                {
                                    var gradeLevelData = this.context?.Gradelevels.FirstOrDefault(x => x.SchoolId == student.studentMaster.SchoolId && x.TenantId == student.studentMaster.TenantId && x.Title.ToLower() == student.CurrentGradeLevel.ToLower());

                                    if (gradeLevelData != null)
                                    {
                                        StudentEnrollmentData = new StudentEnrollment() { TenantId = student.studentMaster.TenantId, SchoolId = student.studentMaster.SchoolId, StudentId = student.studentMaster.StudentId, EnrollmentId = 1, SchoolName = schoolName, RollingOption = "Next grade at current school", EnrollmentCode = enrollmentCode, CalenderId = calenderId, GradeLevelTitle = student.CurrentGradeLevel, EnrollmentDate = DateTime.UtcNow, StudentGuid = GuidId, IsActive = true, GradeId = gradeLevelData.GradeId, CreatedBy = studentListAddViewModel.CreatedBy, CreatedOn = DateTime.UtcNow };
                                    }
                                    else
                                    {
                                        var gradeLevel = this.context?.Gradelevels.Where(x => x.SchoolId == student.studentMaster.SchoolId).OrderBy(x => x.GradeId).FirstOrDefault();

                                        int? gradeId = null;
                                        if (gradeLevel != null)
                                        {
                                            gradeId = gradeLevel.GradeId;
                                        }

                                        StudentEnrollmentData = new StudentEnrollment() { TenantId = student.studentMaster.TenantId, SchoolId = student.studentMaster.SchoolId, StudentId = student.studentMaster.StudentId, EnrollmentId = 1, SchoolName = schoolName, RollingOption = "Next grade at current school", EnrollmentCode = enrollmentCode, CalenderId = calenderId, GradeLevelTitle = gradeLevel?.Title, EnrollmentDate = schoolYearStartDate ?? DateTime.UtcNow, StudentGuid = GuidId, IsActive = true, GradeId = gradeId, CreatedBy = studentListAddViewModel.CreatedBy, CreatedOn = DateTime.UtcNow };
                                    }
                                }
                                else
                                {
                                    var gradeLevel = this.context?.Gradelevels.Where(x => x.SchoolId == student.studentMaster.SchoolId && x.TenantId == student.studentMaster.TenantId).OrderBy(x => x.GradeId).FirstOrDefault();

                                    int? gradeId = null;
                                    if (gradeLevel != null)
                                    {
                                        gradeId = gradeLevel.GradeId;
                                    }

                                    StudentEnrollmentData = new StudentEnrollment() { TenantId = student.studentMaster.TenantId, SchoolId = student.studentMaster.SchoolId, StudentId = student.studentMaster.StudentId, EnrollmentId = 1, SchoolName = schoolName, RollingOption = "Next grade at current school", EnrollmentCode = enrollmentCode, CalenderId = calenderId, GradeLevelTitle = gradeLevel?.Title, EnrollmentDate = schoolYearStartDate ?? DateTime.UtcNow, StudentGuid = GuidId, IsActive = true, GradeId = gradeId, CreatedBy = studentListAddViewModel.CreatedBy, CreatedOn = DateTime.UtcNow };
                                }

                                if (student.EnrollmentDate != null)
                                {
                                    StudentEnrollmentData.EnrollmentDate = Convert.ToDateTime(student.EnrollmentDate);
                                }
                                if (student.Dob != null)
                                {
                                    student.studentMaster.Dob = Convert.ToDateTime(student.Dob);
                                }
                                if (student.EstimatedGradDate != null)
                                {
                                    student.studentMaster.EstimatedGradDate = Convert.ToDateTime(student.EstimatedGradDate);
                                }

                                //Add student portal access
                                if (!string.IsNullOrWhiteSpace(student.PasswordHash) && !string.IsNullOrWhiteSpace(student.LoginEmail))
                                {
                                    UserMaster userMaster = new UserMaster();
                                    //var decrypted = Utility.Decrypt(crypted);
                                    string passwordHash = Utility.GetHashedPassword(student.PasswordHash);

                                    var loginInfo = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == student.studentMaster.TenantId && x.EmailAddress == student.LoginEmail);

                                    if (loginInfo == null)
                                    {
                                        var membership = this.context?.Membership.FirstOrDefault(x => x.TenantId == student.studentMaster.TenantId && x.SchoolId == student.studentMaster.SchoolId && x.Profile == "Student");

                                        userMaster.SchoolId = student.studentMaster.SchoolId;
                                        userMaster.TenantId = student.studentMaster.TenantId;
                                        userMaster.UserId = student.studentMaster.StudentId;
                                        userMaster.LangId = 1;
                                        userMaster.MembershipId = membership!.MembershipId;
                                        userMaster.EmailAddress = student.LoginEmail;
                                        userMaster.PasswordHash = passwordHash;
                                        userMaster.Name = student.studentMaster.FirstGivenName!;
                                        userMaster.CreatedOn = DateTime.UtcNow;
                                        userMaster.CreatedBy = studentListAddViewModel.CreatedBy;
                                        userMaster.IsActive = true;
                                        student.studentMaster.StudentPortalId = student.LoginEmail;
                                        this.context?.UserMaster.Add(userMaster);
                                        this.context?.SaveChanges();
                                        student.studentMaster.StudentPortalId = student.LoginEmail;
                                    }
                                    else
                                    {
                                        studentListAdd.ConflictIndexNo = studentListAdd.ConflictIndexNo != null ? studentListAdd.ConflictIndexNo + "," + indexNo.ToString() : indexNo.ToString();
                                        student._message = "Login Email Already Exist";
                                        studentListAdd.studentAddViewModelList.Add(student);
                                        studentListAdd._failure = true;
                                        studentListAdd._message = "Student Rejected Due to Data Error";
                                        continue;
                                    }
                                }

                                if (student.studentMaster.FirstGivenName == null || student.studentMaster.LastFamilyName == null)
                                {
                                    studentListAdd.ConflictIndexNo = studentListAdd.ConflictIndexNo != null ? studentListAdd.ConflictIndexNo + "," + indexNo.ToString() : indexNo.ToString();
                                    student._message = "FirstName or LastName is not Provided";
                                    studentListAdd.studentAddViewModelList.Add(student);
                                    studentListAdd._failure = true;
                                    studentListAdd._message = "Student Rejected Due to Data Error";
                                    continue;
                                }

                                this.context?.StudentMaster.Add(student.studentMaster);
                                this.context?.StudentEnrollment.Add(StudentEnrollmentData);
                                this.context?.SaveChanges();

                                if (student.fieldsCategoryList != null && student.fieldsCategoryList.ToList()?.Any() == true)
                                {
                                    //var fieldsCategory = student.fieldsCategoryList.FirstOrDefault(x => x.CategoryId == student.SelectedCategoryId);
                                    //if (fieldsCategory != null)
                                    //{
                                    foreach (var fieldsCategory in student.fieldsCategoryList.ToList())
                                    {
                                        foreach (var customFields in fieldsCategory.CustomFields.ToList())
                                        {
                                            if (customFields.CustomFieldsValue.ToList()?.Any() == true)
                                            {
                                                var dataExits = this.context?.CustomFieldsValue.Any(x => x.TenantId == studentListAddViewModel.TenantId && x.SchoolId == student.studentMaster.SchoolId && x.CategoryId == customFields.CategoryId && x.FieldId == customFields.FieldId && x.Module == "Student" && x.TargetId == student.studentMaster.StudentId);

                                                if (dataExits != true)
                                                {
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.TenantId = studentListAddViewModel.TenantId;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.Module = "Student";
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.CategoryId = customFields.CategoryId;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.FieldId = customFields.FieldId;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldTitle = customFields.Title;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldType = customFields.Type;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.SchoolId = student.studentMaster.SchoolId;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.TenantId = student.studentMaster.TenantId;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.TargetId = student.studentMaster.StudentId;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.CreatedBy = studentListAddViewModel.CreatedBy;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.CreatedOn = DateTime.UtcNow;
                                                    this.context?.CustomFieldsValue.AddRange(customFields.CustomFieldsValue);
                                                    this.context?.SaveChanges();
                                                }
                                            }

                                        }
                                    }
                                    //}
                                }
                                transaction?.Commit();
                                MasterStudentId++;
                            }

                            catch (Exception es)
                            {
                                transaction?.Rollback();
                                studentListAdd.studentAddViewModelList.Add(student);
                                studentListAdd.ConflictIndexNo = studentListAdd.ConflictIndexNo != null ? studentListAdd.ConflictIndexNo + "," + indexNo.ToString() : indexNo.ToString();
                                this.context?.StudentMaster.Remove(student.studentMaster);
                                this.context?.StudentEnrollment.Remove(StudentEnrollmentData);
                                //this.context?.UserMaster.Remove(userMaster);
                                studentListAdd._failure = true;
                                studentListAdd._message = es.Message;
                                continue;
                            }
                        }
                    }
                }
            }
            else
            {
                studentListAdd._failure = true;
                studentListAdd._message = "Please Import Student";
            }
            return studentListAdd;
        }

        /// <summary>
        /// Get Transcript For Students
        /// </summary>
        /// <param name="transcriptViewModel"></param>
        /// <returns></returns>
        //public TranscriptViewModel GetTranscriptForStudents(TranscriptViewModel transcriptViewModel)
        //{
        //    TranscriptViewModel transcriptView = new TranscriptViewModel();
        //    try
        //    {
        //        transcriptView.TenantId = transcriptViewModel.TenantId;
        //        transcriptView.SchoolId = transcriptViewModel.SchoolId;
        //        transcriptView._tenantName = transcriptViewModel._tenantName;
        //        transcriptView._token = transcriptViewModel._token;
        //        transcriptView._userName = transcriptViewModel._userName;

        //        if (transcriptViewModel.studentsDetailsForTranscripts?.Any()==true)
        //        {
        //            var schoolData = this.context?.SchoolMaster.Include(x => x.SchoolDetail).FirstOrDefault(x => x.TenantId == transcriptViewModel.TenantId && x.SchoolId == transcriptViewModel.SchoolId);

        //            foreach (var student in transcriptViewModel.studentsDetailsForTranscripts)
        //            {
        //                decimal? totalCreditAttempeted = 0.0m;
        //                decimal? totalCreditEarned = 0.0m;
        //                decimal? cumulativeGPValue = 0.0m;
        //                decimal? cumulativeCreditHours = 0.0m;

        //                var studentsDetailsForTranscript = new StudentsDetailsForTranscript();

        //                var studentMasterData = this.context?.StudentMaster.Include(x => x.StudentEnrollment).FirstOrDefault(x => x.TenantId == transcriptViewModel.TenantId && x.SchoolId == transcriptViewModel.SchoolId && x.StudentId == student.StudentId);

        //                if (studentMasterData != null)
        //                {
        //                    if (schoolData != null)
        //                    {
        //                        studentsDetailsForTranscript.SchoolName = schoolData.SchoolName;
        //                        studentsDetailsForTranscript.StreetAddress1 = schoolData.StreetAddress1;
        //                        studentsDetailsForTranscript.StreetAddress2 = schoolData.StreetAddress2;
        //                        studentsDetailsForTranscript.State = schoolData.State;
        //                        studentsDetailsForTranscript.City = schoolData.City;
        //                        studentsDetailsForTranscript.District = schoolData.District;
        //                        studentsDetailsForTranscript.Zip = schoolData.Zip;
        //                        studentsDetailsForTranscript.SchoolPicture = transcriptViewModel.SchoolLogo == true ? schoolData.SchoolDetail.FirstOrDefault()!.SchoolLogo : null;
        //                        studentsDetailsForTranscript.PrincipalName = schoolData.SchoolDetail.FirstOrDefault()!.NameOfPrincipal;
        //                    }

        //                    if (transcriptViewModel.GradeLagend == true)
        //                    {
        //                        var gradeDataList = this.context?.Grade.Where(x => x.TenantId == transcriptViewModel.TenantId && x.SchoolId == transcriptViewModel.SchoolId).Select(s => new Grade { Breakoff = s.Breakoff, Title = s.Title, UnweightedGpValue = s.UnweightedGpValue, WeightedGpValue = s.WeightedGpValue, Comment = s.Comment }).ToList();
        //                        if (gradeDataList?.Any()==true)
        //                        {
        //                            studentsDetailsForTranscript.gradeList = gradeDataList;
        //                        }
        //                    }

        //                    studentsDetailsForTranscript.StudentGuid = studentMasterData.StudentGuid;
        //                    studentsDetailsForTranscript.StudentId = studentMasterData.StudentId;
        //                    studentsDetailsForTranscript.StudentInternalId = studentMasterData.StudentInternalId;
        //                    studentsDetailsForTranscript.FirstGivenName = studentMasterData.FirstGivenName;
        //                    studentsDetailsForTranscript.MiddleName = studentMasterData.MiddleName;
        //                    studentsDetailsForTranscript.LastFamilyName = studentMasterData.LastFamilyName;
        //                    studentsDetailsForTranscript.StudentPhoto = transcriptViewModel.StudentPhoto == true ? studentMasterData.StudentPhoto : null;
        //                    studentsDetailsForTranscript.HomeAddressLineOne = studentMasterData.HomeAddressLineOne;
        //                    studentsDetailsForTranscript.HomeAddressLineTwo = studentMasterData.HomeAddressLineTwo;
        //                    studentsDetailsForTranscript.HomeAddressState = studentMasterData.HomeAddressState;
        //                    studentsDetailsForTranscript.HomeAddressCity = studentMasterData.HomeAddressCity;
        //                    studentsDetailsForTranscript.HomeAddressCountry = studentMasterData.HomeAddressCountry;
        //                    studentsDetailsForTranscript.HomeAddressZip = studentMasterData.HomeAddressZip;

        //                    if (!string.IsNullOrEmpty(transcriptViewModel.GradeLavels))
        //                    {
        //                        var gradeIds = transcriptViewModel.GradeLavels.Split(",");

        //                        foreach (var grade in gradeIds.ToList())
        //                        {
        //                            var gradeLevelDetailsForTranscript = new GradeLevelDetailsForTranscript();

        //                            var studentDataWithCurrentGrade = studentMasterData.StudentEnrollment.Where(x => x.GradeId == Convert.ToInt32(grade)).FirstOrDefault();

        //                            if (studentDataWithCurrentGrade != null)
        //                            {
        //                                var calenderData = this.context?.SchoolCalendars.FirstOrDefault(x => x.TenantId == transcriptViewModel.TenantId && x.SchoolId == studentDataWithCurrentGrade.SchoolId && x.CalenderId == studentDataWithCurrentGrade.CalenderId);

        //                                if (calenderData != null)
        //                                {
        //                                    gradeLevelDetailsForTranscript.SchoolYear = calenderData.StartDate!.Value.Date.Year + "-" + calenderData.EndDate!.Value.Date.Year;
        //                                }

        //                                gradeLevelDetailsForTranscript.GradeId = studentDataWithCurrentGrade.GradeId;
        //                                gradeLevelDetailsForTranscript.GradeLevelTitle = studentDataWithCurrentGrade.GradeLevelTitle;
        //                                gradeLevelDetailsForTranscript.SchoolName = studentDataWithCurrentGrade.SchoolName;

        //                                decimal? gPValue = 0.0m;
        //                                decimal? gPAValue = 0.0m;
        //                                decimal? creditAttemped = 0.0m;
        //                                decimal? creditEarned = 0.0m;

        //                                var reportCardData = this.context?.StudentFinalGrade.Include(x => x.StudentFinalGradeStandard).Where(x => x.TenantId == transcriptViewModel.TenantId && x.StudentId == student.StudentId && x.GradeId == Convert.ToInt32(grade)).ToList();
        //                                if (reportCardData?.Any()==true)
        //                                {
        //                                    foreach (var reportCard in reportCardData)
        //                                    {
        //                                        var reportCardDetailsForTranscript = new ReportCardDetailsForTranscript();

        //                                        var courseSectionData = this.context?.CourseSection.Include(x => x.Course).Include(x => x.GradeScale).ThenInclude(x => x!.Grade).FirstOrDefault(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && x.CourseId == reportCard.CourseId && x.CourseSectionId == reportCard.CourseSectionId);

        //                                        if (courseSectionData != null)
        //                                        {
        //                                            reportCardDetailsForTranscript.CourseCode = courseSectionData.Course.CourseShortName;
        //                                            reportCardDetailsForTranscript.CourseSectionName = courseSectionData.CourseSectionName;
        //                                            reportCardDetailsForTranscript.CreditHours = courseSectionData.CreditHours;
        //                                            reportCardDetailsForTranscript.Grade = reportCard.GradeObtained;
        //                                            if (courseSectionData.GradeScale != null)
        //                                            {
        //                                                //var gradeData = courseSectionData.GradeScale.Grade.FirstOrDefault(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && x.Title.ToLower() == reportCard.GradeObtained.ToLower() && x.GradeScaleId == reportCard.GradeScaleId);
        //                                                var gradeData = courseSectionData.GradeScale.Grade.FirstOrDefault(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && String.Compare(x.Title, reportCard.GradeObtained, true) == 0 && x.GradeScaleId == reportCard.GradeScaleId);

        //                                                if (gradeData != null)
        //                                                {
        //                                                    reportCardDetailsForTranscript.CreditEarned = reportCard.CreditEarned != null ? reportCard.CreditEarned : courseSectionData.CreditHours;

        //                                                    gPValue = courseSectionData.IsWeightedCourse != true ? reportCardDetailsForTranscript.CreditEarned * gradeData.UnweightedGpValue : reportCardDetailsForTranscript.CreditEarned * gradeData.WeightedGpValue;
        //                                                }
        //                                            }

        //                                            reportCardDetailsForTranscript.GPValue = gPValue; //gpValue=CreditEarned*(WeightedGpValue or UnweightedGpValue)
        //                                            creditAttemped += reportCardDetailsForTranscript.CreditHours;
        //                                            creditEarned += reportCardDetailsForTranscript.CreditEarned;
        //                                            gPAValue += reportCardDetailsForTranscript.GPValue; gradeLevelDetailsForTranscript.reportCardDetailsForTranscripts.Add(reportCardDetailsForTranscript);
        //                                        }
        //                                    }
        //                                }
        //                                gradeLevelDetailsForTranscript.CreditAttemped = creditAttemped; // Σ CreditHours of course sections
        //                                gradeLevelDetailsForTranscript.CreditEarned = creditEarned; // Σ CreditEarned of course sections
        //                                if (gPAValue > 0 && creditEarned > 0)
        //                                {
        //                                    gradeLevelDetailsForTranscript.GPA = gPAValue / creditEarned; // Σ gpValue of course sections / Σ CreditEarned of course sections
        //                                }                                       
        //                                totalCreditEarned += creditEarned;
        //                                totalCreditAttempeted += creditAttemped;
        //                                cumulativeGPValue += gPAValue;
        //                                cumulativeCreditHours += creditAttemped;
        //                                studentsDetailsForTranscript.gradeLevelDetailsForTranscripts.Add(gradeLevelDetailsForTranscript);
        //                            }
        //                        }
        //                    }
        //                    if (cumulativeGPValue > 0 && cumulativeCreditHours > 0)
        //                    {
        //                        studentsDetailsForTranscript.CumulativeGPA = cumulativeGPValue / cumulativeCreditHours;  // Σ gpValue of all course sections / Σ CreditHours of all course sections
        //                    }
        //                    studentsDetailsForTranscript.TotalCreditAttempeted = totalCreditAttempeted; //Σ CreditAttemped 
        //                    studentsDetailsForTranscript.TotalCreditEarned = totalCreditEarned;  //Σ CreditEarned

        //                    transcriptView.studentsDetailsForTranscripts.Add(studentsDetailsForTranscript);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            transcriptView._failure = true;
        //            transcriptView._message = "Select Student Please";
        //        }
        //    }
        //    catch (Exception es)
        //    {
        //        transcriptView._failure = false;
        //        transcriptView._message = es.Message;
        //    }
        //    return transcriptView;
        //}

        public TranscriptViewModel GetTranscriptForStudents(TranscriptViewModel transcriptViewModel)
        {
            TranscriptViewModel transcriptView = new TranscriptViewModel();
            var gradeDataList = new List<Grade>();
            try
            {
                transcriptView.TenantId = transcriptViewModel.TenantId;
                transcriptView.SchoolId = transcriptViewModel.SchoolId;
                transcriptView._tenantName = transcriptViewModel._tenantName;
                transcriptView._token = transcriptViewModel._token;
                transcriptView._userName = transcriptViewModel._userName;

                if (transcriptViewModel.studentsDetailsForTranscripts?.Any() == true)
                {
                    if (transcriptViewModel.GradeLagend == true)
                    {
                        //this block for grade details
                        var gradeScaleData = this.context?.GradeScale.Include(x => x.Grade).Where(x => x.TenantId == transcriptViewModel.TenantId && x.SchoolId == transcriptViewModel.SchoolId && x.AcademicYear == transcriptViewModel.AcademicYear && x.UseAsStandardGradeScale != true);

                        if (gradeScaleData?.Any() == true)
                        {
                            gradeDataList = gradeScaleData.SelectMany(x => x.Grade).Where(x => x.TenantId == transcriptViewModel.TenantId && x.SchoolId == transcriptViewModel.SchoolId).Select(s => new Grade { GradeScaleId = s.GradeScaleId, GradeId = s.GradeId, Breakoff = s.Breakoff, Title = s.Title, UnweightedGpValue = s.UnweightedGpValue, WeightedGpValue = s.WeightedGpValue, Comment = s.Comment }).ToList();
                        }
                    }

                    var schoolData = this.context?.SchoolMaster.Include(x => x.SchoolDetail).FirstOrDefault(x => x.TenantId == transcriptViewModel.TenantId && x.SchoolId == transcriptViewModel.SchoolId);

                    foreach (var student in transcriptViewModel.studentsDetailsForTranscripts)
                    {
                        decimal? totalCreditAttempeted = 0.0m;
                        decimal? totalCreditEarned = 0.0m;
                        decimal? cumulativeGPValue = 0.0m;
                        decimal? cumulativeCreditHours = 0.0m;

                        var studentsDetailsForTranscript = new StudentsDetailsForTranscript();

                        var studentMasterData = this.context?.StudentMaster.Include(x => x.StudentEnrollment).FirstOrDefault(x => x.TenantId == transcriptViewModel.TenantId && x.SchoolId == transcriptViewModel.SchoolId && x.StudentId == student.StudentId);

                        if (studentMasterData != null)
                        {
                            if (schoolData != null)
                            {
                                //this block for school details data
                                studentsDetailsForTranscript.SchoolName = schoolData.SchoolName;
                                studentsDetailsForTranscript.StreetAddress1 = schoolData.StreetAddress1;
                                studentsDetailsForTranscript.StreetAddress2 = schoolData.StreetAddress2;
                                studentsDetailsForTranscript.State = schoolData.State;
                                studentsDetailsForTranscript.City = schoolData.City;
                                studentsDetailsForTranscript.District = schoolData.District;
                                studentsDetailsForTranscript.Zip = schoolData.Zip;
                                studentsDetailsForTranscript.SchoolPicture = transcriptViewModel.SchoolLogo == true ? schoolData.SchoolDetail.FirstOrDefault()!.SchoolThumbnailLogo : null;
                                studentsDetailsForTranscript.PrincipalName = schoolData.SchoolDetail.FirstOrDefault()!.NameOfPrincipal;
                            }

                            if (transcriptViewModel.GradeLagend == true)
                            {
                                //this block for bind grade details if GradeLagend turned on from ui
                                if (gradeDataList?.Any() == true)
                                {
                                    studentsDetailsForTranscript.gradeList = gradeDataList;
                                }
                            }

                            //this are student details
                            studentsDetailsForTranscript.StudentGuid = studentMasterData.StudentGuid;
                            studentsDetailsForTranscript.StudentId = studentMasterData.StudentId;
                            studentsDetailsForTranscript.StudentInternalId = studentMasterData.StudentInternalId;
                            studentsDetailsForTranscript.FirstGivenName = studentMasterData.FirstGivenName;
                            studentsDetailsForTranscript.MiddleName = studentMasterData.MiddleName;
                            studentsDetailsForTranscript.LastFamilyName = studentMasterData.LastFamilyName;
                            studentsDetailsForTranscript.Dob = studentMasterData.Dob;
                            //studentsDetailsForTranscript.StudentPhoto = transcriptViewModel.StudentPhoto == true ? studentMasterData.StudentPhoto : null;
                            studentsDetailsForTranscript.StudentPhoto = transcriptViewModel.StudentPhoto == true ? studentMasterData.StudentThumbnailPhoto : null;
                            studentsDetailsForTranscript.HomeAddressLineOne = studentMasterData.HomeAddressLineOne;
                            studentsDetailsForTranscript.HomeAddressLineTwo = studentMasterData.HomeAddressLineTwo;
                            studentsDetailsForTranscript.HomeAddressState = studentMasterData.HomeAddressState;
                            studentsDetailsForTranscript.HomeAddressCity = studentMasterData.HomeAddressCity;
                            studentsDetailsForTranscript.HomeAddressCountry = studentMasterData.HomeAddressCountry;
                            studentsDetailsForTranscript.HomeAddressZip = studentMasterData.HomeAddressZip;

                            if (!string.IsNullOrEmpty(transcriptViewModel.GradeLavels))
                            {
                                //this block for regular grade level for student.
                                var gradeIds = transcriptViewModel.GradeLavels.Split(",");

                                //this loop for multiple grade.
                                foreach (var grade in gradeIds.ToList())
                                {
                                    var gradeLevelDetailsForTranscript = new GradeLevelDetailsForTranscript();
                                    var gradeLevelId = Convert.ToInt32(grade);

                                    var studentDataWithCurrentGrade = studentMasterData.StudentEnrollment.Where(x => x.GradeId == gradeLevelId).FirstOrDefault();

                                    if (studentDataWithCurrentGrade != null)
                                    {
                                        var calenderData = this.context?.SchoolCalendars.FirstOrDefault(x => x.TenantId == transcriptViewModel.TenantId && x.SchoolId == studentDataWithCurrentGrade.SchoolId && x.CalenderId == studentDataWithCurrentGrade.CalenderId);

                                        if (calenderData != null)
                                        {
                                            gradeLevelDetailsForTranscript.SchoolYear = calenderData.StartDate!.Value.Date.Year + "-" + calenderData.EndDate!.Value.Date.Year;
                                        }

                                        gradeLevelDetailsForTranscript.GradeId = studentDataWithCurrentGrade.GradeId;
                                        gradeLevelDetailsForTranscript.GradeLevelTitle = studentDataWithCurrentGrade.GradeLevelTitle;
                                        gradeLevelDetailsForTranscript.SchoolName = studentDataWithCurrentGrade.SchoolName;

                                        decimal? gPAValue = 0.0m;
                                        decimal? creditAttemped = 0.0m;
                                        decimal? creditEarned = 0.0m;

                                        var reportCardData = this.context?.StudentFinalGrade.Include(x => x.StudentFinalGradeStandard).Include(s => s.SchoolYears).Include(s => s.Semesters).Include(s => s.Quarters).Include(s => s.ProgressPeriod).Where(x => x.TenantId == transcriptViewModel.TenantId && x.StudentId == student.StudentId && x.GradeId == gradeLevelId && x.IsExamGrade != true).ToList();

                                        if (reportCardData?.Any() == true)
                                        {
                                            //for pp marking period
                                            var prgsReportCardData = reportCardData.Where(x => x.PrgrsprdMarkingPeriodId != null);

                                            if (prgsReportCardData.Any() == true)
                                            {
                                                var distinctPrgsIds = prgsReportCardData.Select(x => x.PrgrsprdMarkingPeriodId).Distinct().ToList().OrderBy(s => s!.Value);

                                                //this loop for multiple progress period
                                                foreach (var prgsId in distinctPrgsIds)
                                                {
                                                    var markingPeriodDetailsForTranscript = new MarkingPeriodDetailsForTranscript();
                                                    //decimal? prgsGPValue = 0.0m;
                                                    decimal? prgsGPAValue = 0.0m;
                                                    decimal? prgsCreditAttemped = 0.0m;
                                                    decimal? prgsCreditEarned = 0.0m;

                                                    var MPWiseReportCardData = prgsReportCardData.Where(s => s.PrgrsprdMarkingPeriodId == prgsId);//fetch data marking period wise

                                                    foreach (var reportCard in MPWiseReportCardData)
                                                    {
                                                        decimal? prgsGPValue = 0.0m;
                                                        var reportCardDetailsForTranscript = new ReportCardDetailsForTranscript();

                                                        var courseSectionData = this.context?.CourseSection.Include(x => x.Course).Include(x => x.GradeScale).ThenInclude(x => x!.Grade).FirstOrDefault(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && x.CourseId == reportCard.CourseId && x.CourseSectionId == reportCard.CourseSectionId);

                                                        if (courseSectionData != null)
                                                        {
                                                            reportCardDetailsForTranscript.CourseCode = courseSectionData.Course.CourseShortName;
                                                            reportCardDetailsForTranscript.CourseSectionName = courseSectionData.CourseSectionName;
                                                            reportCardDetailsForTranscript.CreditHours = reportCard.CreditAttempted != null ? reportCard.CreditAttempted : 0.0m;
                                                            reportCardDetailsForTranscript.CreditEarned = reportCard.CreditEarned != null ? reportCard.CreditEarned : 0.0m;
                                                            reportCardDetailsForTranscript.Grade = reportCard.GradeObtained;
                                                            if (courseSectionData.GradeScale != null)
                                                            {
                                                                var gradeData = courseSectionData.GradeScale.Grade.FirstOrDefault(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && String.Compare(x.Title, reportCard.GradeObtained, true) == 0 && x.GradeScaleId == reportCard.GradeScaleId);
                                                                if (gradeData != null)
                                                                {
                                                                    prgsGPValue = courseSectionData.IsWeightedCourse != true ? reportCardDetailsForTranscript.CreditEarned * gradeData.UnweightedGpValue : reportCardDetailsForTranscript.CreditEarned * gradeData.WeightedGpValue;
                                                                }
                                                            }
                                                            else if (courseSectionData.GradeScaleType == "Teacher_Scale")
                                                            {
                                                                var GradebookConfigurationGrade = this.context?.GradebookConfigurationGradescale.Where(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && x.CourseSectionId == reportCard.CourseSectionId && x.AcademicYear == transcriptViewModel.AcademicYear).ToList();
                                                                if (GradebookConfigurationGrade != null)
                                                                {
                                                                    var ConfigurationGrade = GradebookConfigurationGrade.FirstOrDefault(x => x.BreakoffPoints <= reportCard.PercentMarks);
                                                                    var gradeData = gradeDataList?.FirstOrDefault(x => x.GradeId == ConfigurationGrade?.GradeId && x.GradeScaleId == ConfigurationGrade.GradeScaleId);
                                                                    if (gradeData != null)
                                                                    {
                                                                        reportCardDetailsForTranscript.Grade = gradeData.Title;
                                                                        prgsGPValue = courseSectionData.IsWeightedCourse != true ? reportCardDetailsForTranscript.CreditEarned * gradeData.UnweightedGpValue : reportCardDetailsForTranscript.CreditEarned * gradeData.WeightedGpValue;
                                                                    }
                                                                }
                                                            }

                                                            reportCardDetailsForTranscript.GPValue = prgsGPValue;
                                                            prgsCreditAttemped += reportCardDetailsForTranscript.CreditHours;
                                                            prgsCreditEarned += reportCardDetailsForTranscript.CreditEarned;
                                                            prgsGPAValue += reportCardDetailsForTranscript.GPValue;
                                                            markingPeriodDetailsForTranscript.reportCardDetailsForTranscripts.Add(reportCardDetailsForTranscript);
                                                        }
                                                    }
                                                    creditAttemped += prgsCreditAttemped;
                                                    creditEarned += prgsCreditEarned;
                                                    gPAValue += prgsGPAValue;

                                                    markingPeriodDetailsForTranscript.MarkingPeriodTitle = MPWiseReportCardData.FirstOrDefault()!.ProgressPeriod?.Title;
                                                    markingPeriodDetailsForTranscript.CreditAttemped = prgsCreditAttemped;
                                                    markingPeriodDetailsForTranscript.CreditEarned = prgsCreditEarned;
                                                    if (prgsGPAValue > 0 && prgsCreditEarned > 0)
                                                    {
                                                        markingPeriodDetailsForTranscript.GPA = Math.Round((decimal)(prgsGPAValue / prgsCreditEarned), 3);
                                                    }
                                                    gradeLevelDetailsForTranscript.markingPeriodDetailsForTranscripts.Add(markingPeriodDetailsForTranscript);
                                                }
                                            }

                                            //for quater marking period
                                            var qrtReportCardData = reportCardData.Where(x => x.QtrMarkingPeriodId != null);

                                            if (qrtReportCardData.Any() == true)
                                            {
                                                var distinctQrtIds = qrtReportCardData.Select(x => x.QtrMarkingPeriodId).Distinct().ToList().OrderBy(s => s!.Value);

                                                //this loop for multiple quater
                                                foreach (var qtrId in distinctQrtIds)
                                                {
                                                    var markingPeriodDetailsForTranscript = new MarkingPeriodDetailsForTranscript();
                                                    //decimal? qtrGPValue = 0.0m;
                                                    decimal? qtrGPAValue = 0.0m;
                                                    decimal? qtrCreditAttemped = 0.0m;
                                                    decimal? qtrCreditEarned = 0.0m;

                                                    var MPWiseReportCardData = qrtReportCardData.Where(s => s.QtrMarkingPeriodId == qtrId);//fetch data marking period wise

                                                    foreach (var reportCard in MPWiseReportCardData)
                                                    {
                                                        decimal? qtrGPValue = 0.0m;
                                                        var reportCardDetailsForTranscript = new ReportCardDetailsForTranscript();

                                                        var courseSectionData = this.context?.CourseSection.Include(x => x.Course).Include(x => x.GradeScale).ThenInclude(x => x!.Grade).FirstOrDefault(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && x.CourseId == reportCard.CourseId && x.CourseSectionId == reportCard.CourseSectionId);

                                                        if (courseSectionData != null)
                                                        {
                                                            reportCardDetailsForTranscript.CourseCode = courseSectionData.Course.CourseShortName;
                                                            reportCardDetailsForTranscript.CourseSectionName = courseSectionData.CourseSectionName;
                                                            reportCardDetailsForTranscript.CreditHours = reportCard.CreditAttempted != null ? reportCard.CreditAttempted : 0.0m;
                                                            reportCardDetailsForTranscript.CreditEarned = reportCard.CreditEarned != null ? reportCard.CreditEarned : 0.0m;
                                                            reportCardDetailsForTranscript.Grade = reportCard.GradeObtained;
                                                            if (courseSectionData.GradeScale != null)
                                                            {
                                                                var gradeData = courseSectionData.GradeScale.Grade.FirstOrDefault(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && String.Compare(x.Title, reportCard.GradeObtained, true) == 0 && x.GradeScaleId == reportCard.GradeScaleId);
                                                                if (gradeData != null)
                                                                {
                                                                    qtrGPValue = courseSectionData.IsWeightedCourse != true ? reportCardDetailsForTranscript.CreditEarned * gradeData.UnweightedGpValue : reportCardDetailsForTranscript.CreditEarned * gradeData.WeightedGpValue;
                                                                }
                                                            }
                                                            else if (courseSectionData.GradeScaleType == "Teacher_Scale")
                                                            {
                                                                var GradebookConfigurationGrade = this.context?.GradebookConfigurationGradescale.Where(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && x.CourseSectionId == reportCard.CourseSectionId && x.AcademicYear == transcriptViewModel.AcademicYear).ToList();
                                                                if (GradebookConfigurationGrade != null)
                                                                {
                                                                    var ConfigurationGrade = GradebookConfigurationGrade.FirstOrDefault(x => x.BreakoffPoints <= reportCard.PercentMarks);
                                                                    var gradeData = gradeDataList?.FirstOrDefault(x => x.GradeId == ConfigurationGrade?.GradeId && x.GradeScaleId == ConfigurationGrade.GradeScaleId);
                                                                    if (gradeData != null)
                                                                    {
                                                                        reportCardDetailsForTranscript.Grade = gradeData.Title;
                                                                        qtrGPValue = courseSectionData.IsWeightedCourse != true ? reportCardDetailsForTranscript.CreditEarned * gradeData.UnweightedGpValue : reportCardDetailsForTranscript.CreditEarned * gradeData.WeightedGpValue;
                                                                    }
                                                                }
                                                            }

                                                            reportCardDetailsForTranscript.GPValue = qtrGPValue;
                                                            qtrCreditAttemped += reportCardDetailsForTranscript.CreditHours;
                                                            qtrCreditEarned += reportCardDetailsForTranscript.CreditEarned;
                                                            qtrGPAValue += reportCardDetailsForTranscript.GPValue;
                                                            markingPeriodDetailsForTranscript.reportCardDetailsForTranscripts.Add(reportCardDetailsForTranscript);
                                                        }
                                                    }
                                                    creditAttemped += qtrCreditAttemped;
                                                    creditEarned += qtrCreditEarned;
                                                    gPAValue += qtrGPAValue;

                                                    markingPeriodDetailsForTranscript.MarkingPeriodTitle = MPWiseReportCardData.FirstOrDefault()!.Quarters?.Title;
                                                    markingPeriodDetailsForTranscript.CreditAttemped = qtrCreditAttemped;
                                                    markingPeriodDetailsForTranscript.CreditEarned = qtrCreditEarned;
                                                    if (qtrGPAValue > 0 && qtrCreditEarned > 0)
                                                    {
                                                        markingPeriodDetailsForTranscript.GPA = Math.Round((decimal)(qtrGPAValue / qtrCreditEarned), 3);
                                                    }
                                                    gradeLevelDetailsForTranscript.markingPeriodDetailsForTranscripts.Add(markingPeriodDetailsForTranscript);
                                                }
                                            }

                                            //for semester marking period
                                            var smstrReportCardData = reportCardData.Where(x => x.SmstrMarkingPeriodId != null);

                                            if (smstrReportCardData.Any() == true)
                                            {
                                                var distinctSmstrIds = smstrReportCardData.Select(x => x.SmstrMarkingPeriodId).Distinct().ToList().OrderBy(s => s!.Value);

                                                foreach (var smstrId in distinctSmstrIds)
                                                {
                                                    var markingPeriodDetailsForTranscript = new MarkingPeriodDetailsForTranscript();
                                                    //decimal? smstrGPValue = 0.0m;
                                                    decimal? smstrGPAValue = 0.0m;
                                                    decimal? smstrCreditAttemped = 0.0m;
                                                    decimal? smstrCreditEarned = 0.0m;

                                                    var MPWiseReportCardData = smstrReportCardData.Where(s => s.SmstrMarkingPeriodId == smstrId);//fetch data marking period wise

                                                    //this loop for multiple semester
                                                    foreach (var reportCard in MPWiseReportCardData)
                                                    {
                                                        decimal? smstrGPValue = 0.0m;
                                                        var reportCardDetailsForTranscript = new ReportCardDetailsForTranscript();

                                                        var courseSectionData = this.context?.CourseSection.Include(x => x.Course).Include(x => x.GradeScale).ThenInclude(x => x!.Grade).FirstOrDefault(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && x.CourseId == reportCard.CourseId && x.CourseSectionId == reportCard.CourseSectionId);

                                                        if (courseSectionData != null)
                                                        {
                                                            reportCardDetailsForTranscript.CourseCode = courseSectionData.Course.CourseShortName;
                                                            reportCardDetailsForTranscript.CourseSectionName = courseSectionData.CourseSectionName;
                                                            reportCardDetailsForTranscript.CreditHours = reportCard.CreditAttempted != null ? reportCard.CreditAttempted : 0.0m;
                                                            reportCardDetailsForTranscript.CreditEarned = reportCard.CreditEarned != null ? reportCard.CreditEarned : 0.0m;
                                                            reportCardDetailsForTranscript.Grade = reportCard.GradeObtained;
                                                            if (courseSectionData.GradeScale != null)
                                                            {
                                                                var gradeData = courseSectionData.GradeScale.Grade.FirstOrDefault(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && String.Compare(x.Title, reportCard.GradeObtained, true) == 0 && x.GradeScaleId == reportCard.GradeScaleId);
                                                                if (gradeData != null)
                                                                {
                                                                    smstrGPValue = courseSectionData.IsWeightedCourse != true ? reportCardDetailsForTranscript.CreditEarned * gradeData.UnweightedGpValue : reportCardDetailsForTranscript.CreditEarned * gradeData.WeightedGpValue;
                                                                }
                                                            }
                                                            else if (courseSectionData.GradeScaleType == "Teacher_Scale")
                                                            {
                                                                var GradebookConfigurationGrade = this.context?.GradebookConfigurationGradescale.Where(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && x.CourseSectionId == reportCard.CourseSectionId && x.AcademicYear == transcriptViewModel.AcademicYear).ToList();
                                                                if (GradebookConfigurationGrade != null)
                                                                {
                                                                    var ConfigurationGrade = GradebookConfigurationGrade.FirstOrDefault(x => x.BreakoffPoints <= reportCard.PercentMarks);
                                                                    var gradeData = gradeDataList?.FirstOrDefault(x => x.GradeId == ConfigurationGrade?.GradeId && x.GradeScaleId == ConfigurationGrade.GradeScaleId);
                                                                    if (gradeData != null)
                                                                    {
                                                                        reportCardDetailsForTranscript.Grade = gradeData.Title;
                                                                        smstrGPValue = courseSectionData.IsWeightedCourse != true ? reportCardDetailsForTranscript.CreditEarned * gradeData.UnweightedGpValue : reportCardDetailsForTranscript.CreditEarned * gradeData.WeightedGpValue;
                                                                    }
                                                                }
                                                            }

                                                            reportCardDetailsForTranscript.GPValue = smstrGPValue;
                                                            smstrCreditAttemped += reportCardDetailsForTranscript.CreditHours;
                                                            smstrCreditEarned += reportCardDetailsForTranscript.CreditEarned;
                                                            smstrGPAValue += reportCardDetailsForTranscript.GPValue;
                                                            markingPeriodDetailsForTranscript.reportCardDetailsForTranscripts.Add(reportCardDetailsForTranscript);
                                                        }
                                                    }
                                                    creditAttemped += smstrCreditAttemped;
                                                    creditEarned += smstrCreditEarned;
                                                    gPAValue += smstrGPAValue;

                                                    markingPeriodDetailsForTranscript.MarkingPeriodTitle = MPWiseReportCardData.FirstOrDefault()!.Semesters?.Title;
                                                    markingPeriodDetailsForTranscript.CreditAttemped = smstrCreditAttemped;
                                                    markingPeriodDetailsForTranscript.CreditEarned = smstrCreditEarned;
                                                    if (smstrGPAValue > 0 && smstrCreditEarned > 0)
                                                    {
                                                        markingPeriodDetailsForTranscript.GPA = Math.Round((decimal)(smstrGPAValue / smstrCreditEarned), 3);
                                                    }
                                                    gradeLevelDetailsForTranscript.markingPeriodDetailsForTranscripts.Add(markingPeriodDetailsForTranscript);
                                                }
                                            }

                                            //for year marking period
                                            var yrReportCardData = reportCardData.Where(x => x.YrMarkingPeriodId != null);

                                            if (yrReportCardData.Any() == true)
                                            {
                                                var distinctYrIds = yrReportCardData.Select(x => x.YrMarkingPeriodId).Distinct().ToList().OrderBy(s => s!.Value);

                                                //this loop for multiple school year
                                                foreach (var yrId in distinctYrIds)
                                                {
                                                    var markingPeriodDetailsForTranscript = new MarkingPeriodDetailsForTranscript();
                                                    //decimal? yrGPValue = 0.0m;
                                                    decimal? yrGPAValue = 0.0m;
                                                    decimal? yrCreditAttemped = 0.0m;
                                                    decimal? yrCreditEarned = 0.0m;

                                                    var MPWiseReportCardData = yrReportCardData.Where(s => s.YrMarkingPeriodId == yrId);//fetch data marking period wise

                                                    foreach (var reportCard in MPWiseReportCardData)
                                                    {
                                                        decimal? yrGPValue = 0.0m;
                                                        var reportCardDetailsForTranscript = new ReportCardDetailsForTranscript();

                                                        var courseSectionData = this.context?.CourseSection.Include(x => x.Course).Include(x => x.GradeScale).ThenInclude(x => x!.Grade).FirstOrDefault(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && x.CourseId == reportCard.CourseId && x.CourseSectionId == reportCard.CourseSectionId);

                                                        if (courseSectionData != null)
                                                        {
                                                            reportCardDetailsForTranscript.CourseCode = courseSectionData.Course.CourseShortName;
                                                            reportCardDetailsForTranscript.CourseSectionName = courseSectionData.CourseSectionName;
                                                            reportCardDetailsForTranscript.CreditHours = reportCard.CreditAttempted != null ? reportCard.CreditAttempted : 0.0m;
                                                            reportCardDetailsForTranscript.CreditEarned = reportCard.CreditEarned != null ? reportCard.CreditEarned : 0.0m;
                                                            reportCardDetailsForTranscript.Grade = reportCard.GradeObtained;
                                                            if (courseSectionData.GradeScale != null)
                                                            {
                                                                var gradeData = courseSectionData.GradeScale.Grade.FirstOrDefault(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && String.Compare(x.Title, reportCard.GradeObtained, true) == 0 && x.GradeScaleId == reportCard.GradeScaleId);
                                                                if (gradeData != null)
                                                                {
                                                                    yrGPValue = courseSectionData.IsWeightedCourse != true ? reportCardDetailsForTranscript.CreditEarned * gradeData.UnweightedGpValue : reportCardDetailsForTranscript.CreditEarned * gradeData.WeightedGpValue;
                                                                }
                                                            }
                                                            else if (courseSectionData.GradeScaleType == "Teacher_Scale")
                                                            {
                                                                var GradebookConfigurationGrade = this.context?.GradebookConfigurationGradescale.Where(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && x.CourseSectionId == reportCard.CourseSectionId && x.AcademicYear == transcriptViewModel.AcademicYear).ToList();
                                                                if (GradebookConfigurationGrade != null)
                                                                {
                                                                    var ConfigurationGrade = GradebookConfigurationGrade.FirstOrDefault(x => x.BreakoffPoints <= reportCard.PercentMarks);
                                                                    var gradeData = gradeDataList?.FirstOrDefault(x => x.GradeId == ConfigurationGrade?.GradeId && x.GradeScaleId == ConfigurationGrade.GradeScaleId);
                                                                    if (gradeData != null)
                                                                    {
                                                                        reportCardDetailsForTranscript.Grade = gradeData.Title;
                                                                        yrGPValue = courseSectionData.IsWeightedCourse != true ? reportCardDetailsForTranscript.CreditEarned * gradeData.UnweightedGpValue : reportCardDetailsForTranscript.CreditEarned * gradeData.WeightedGpValue;
                                                                    }
                                                                }
                                                            }

                                                            reportCardDetailsForTranscript.GPValue = yrGPValue;
                                                            yrCreditAttemped += reportCardDetailsForTranscript.CreditHours;
                                                            yrCreditEarned += reportCardDetailsForTranscript.CreditEarned;
                                                            yrGPAValue += reportCardDetailsForTranscript.GPValue;
                                                            markingPeriodDetailsForTranscript.reportCardDetailsForTranscripts.Add(reportCardDetailsForTranscript);
                                                        }
                                                    }
                                                    creditAttemped += yrCreditAttemped;
                                                    creditEarned += yrCreditEarned;
                                                    gPAValue += yrGPAValue;

                                                    markingPeriodDetailsForTranscript.MarkingPeriodTitle = MPWiseReportCardData.FirstOrDefault()!.SchoolYears?.Title;
                                                    markingPeriodDetailsForTranscript.CreditAttemped = yrCreditAttemped;
                                                    markingPeriodDetailsForTranscript.CreditEarned = yrCreditEarned;
                                                    if (yrGPAValue > 0 && yrCreditEarned > 0)
                                                    {
                                                        markingPeriodDetailsForTranscript.GPA = Math.Round((decimal)(yrGPAValue / yrCreditEarned), 3);
                                                    }
                                                    gradeLevelDetailsForTranscript.markingPeriodDetailsForTranscripts.Add(markingPeriodDetailsForTranscript);
                                                }
                                            }

                                            totalCreditEarned += creditEarned;
                                            totalCreditAttempeted += creditAttemped;
                                            cumulativeGPValue += gPAValue;
                                            cumulativeCreditHours += creditAttemped;
                                            studentsDetailsForTranscript.gradeLevelDetailsForTranscripts.Add(gradeLevelDetailsForTranscript);
                                        }
                                    }
                                }

                                //this block for historical grade level for student 
                                List<int?> gradeLevelIds = new List<int?>();

                                var studentHistoricalGradeList = this.context?.HistoricalGrade.Include(s => s.HistoricalCreditTransfer).Where(x => x.TenantId == transcriptViewModel.TenantId && x.SchoolId == transcriptViewModel.SchoolId && x.StudentId == student.StudentId).ToList();

                                if (studentHistoricalGradeList?.Any() == true)
                                {
                                    gradeLevelIds = studentHistoricalGradeList.Select(x => x.EquivalencyId).ToList();

                                    var historicalMarkingPeriodData = this.context?.HistoricalMarkingPeriod.Where(x => x.TenantId == transcriptViewModel.TenantId && x.SchoolId == transcriptViewModel.SchoolId).ToList();

                                    var gradeEquivalencyData = this.context?.GradeEquivalency.ToList();

                                    //this loop for multiple grade.
                                    foreach (var gradeLevelId in gradeLevelIds)
                                    {
                                        var gradeLevelDetailsForTranscript = new GradeLevelDetailsForTranscript();

                                        var studentHistoricalGradeData = studentHistoricalGradeList.FirstOrDefault(x => x.TenantId == transcriptViewModel.TenantId && x.SchoolId == transcriptViewModel.SchoolId && x.EquivalencyId == gradeLevelId && x.StudentId == student.StudentId);

                                        if (studentHistoricalGradeData != null)
                                        {
                                            gradeLevelDetailsForTranscript.SchoolYear = historicalMarkingPeriodData?.FirstOrDefault(x => x.HistMarkingPeriodId == studentHistoricalGradeData.HistMarkingPeriodId)?.Title;
                                            gradeLevelDetailsForTranscript.GradeId = studentHistoricalGradeData.EquivalencyId;
                                            gradeLevelDetailsForTranscript.GradeLevelTitle = gradeEquivalencyData!.FirstOrDefault(x => x.EquivalencyId == studentHistoricalGradeData.EquivalencyId)?.GradeLevelEquivalency;
                                            gradeLevelDetailsForTranscript.SchoolName = studentHistoricalGradeData.SchoolName;

                                            decimal? gPAValue = 0.0m;
                                            decimal? creditAttemped = 0.0m;
                                            decimal? creditEarned = 0.0m;

                                            var historicalMarkingPeriodinGrade = historicalMarkingPeriodData?.Where(s => s.HistMarkingPeriodId == studentHistoricalGradeData.HistMarkingPeriodId).ToList();

                                            if (historicalMarkingPeriodinGrade?.Any() == true)
                                            {
                                                foreach (var historicalMarkingPeriod in historicalMarkingPeriodinGrade)
                                                {
                                                    var markingPeriodDetailsForTranscript = new MarkingPeriodDetailsForTranscript();

                                                    decimal? mpGPAValue = 0.0m;
                                                    decimal? mpCreditAttemped = 0.0m;
                                                    decimal? mpCreditEarned = 0.0m;

                                                    foreach (var historicalCredit in studentHistoricalGradeData.HistoricalCreditTransfer)
                                                    {
                                                        var reportCardDetailsForTranscript = new ReportCardDetailsForTranscript();

                                                        reportCardDetailsForTranscript.CourseCode = historicalCredit.CourseCode;
                                                        reportCardDetailsForTranscript.CourseSectionName = historicalCredit.CourseName;
                                                        reportCardDetailsForTranscript.CreditHours = historicalCredit.CreditAttempted;
                                                        reportCardDetailsForTranscript.CreditEarned = historicalCredit.CreditEarned;
                                                        reportCardDetailsForTranscript.Grade = historicalCredit.LetterGrade;
                                                        reportCardDetailsForTranscript.GPValue = historicalCredit.GpValue;

                                                        mpCreditAttemped += reportCardDetailsForTranscript.CreditHours;
                                                        mpCreditEarned += reportCardDetailsForTranscript.CreditEarned;
                                                        mpGPAValue += reportCardDetailsForTranscript.GPValue;
                                                        markingPeriodDetailsForTranscript.reportCardDetailsForTranscripts.Add(reportCardDetailsForTranscript);
                                                    }

                                                    creditAttemped += mpCreditAttemped;
                                                    creditEarned += mpCreditEarned;
                                                    gPAValue += mpGPAValue;

                                                    markingPeriodDetailsForTranscript.MarkingPeriodTitle = historicalMarkingPeriod.Title;
                                                    markingPeriodDetailsForTranscript.CreditAttemped = mpCreditAttemped;
                                                    markingPeriodDetailsForTranscript.CreditEarned = mpCreditEarned;
                                                    if (mpGPAValue > 0 && mpCreditEarned > 0)
                                                    {
                                                        markingPeriodDetailsForTranscript.GPA = Math.Round((decimal)(mpGPAValue / mpCreditEarned), 3);
                                                    }
                                                    gradeLevelDetailsForTranscript.markingPeriodDetailsForTranscripts.Add(markingPeriodDetailsForTranscript);
                                                }

                                                totalCreditEarned += creditEarned;
                                                totalCreditAttempeted += creditAttemped;
                                                cumulativeGPValue += gPAValue;
                                                cumulativeCreditHours += creditAttemped;
                                                studentsDetailsForTranscript.gradeLevelDetailsForTranscripts.Add(gradeLevelDetailsForTranscript);
                                            }
                                        }
                                    }
                                }

                                //this for calculate cumulativeGPValue
                                if (cumulativeGPValue > 0 && cumulativeCreditHours > 0)
                                {
                                    studentsDetailsForTranscript.CumulativeGPA = Math.Round((decimal)(cumulativeGPValue / cumulativeCreditHours), 3);  // Σ gpValue of all course sections / Σ CreditHours of all course sections
                                }
                                studentsDetailsForTranscript.TotalCreditAttempeted = totalCreditAttempeted; //Σ CreditAttemped 
                                studentsDetailsForTranscript.TotalCreditEarned = totalCreditEarned;  //Σ CreditEarned

                                transcriptView.studentsDetailsForTranscripts.Add(studentsDetailsForTranscript);
                            }
                            else
                            {
                                transcriptView._failure = true;
                                transcriptView._message = "Select Gradelevel Please";
                            }
                        }
                    }
                }
                else
                {
                    transcriptView._failure = true;
                    transcriptView._message = "Select Student Please";
                }
            }
            catch (Exception es)
            {
                transcriptView._failure = false;
                transcriptView._message = es.Message;
            }
            return transcriptView;
        }

        /// <summary>
        /// Add Transcript For Students
        /// </summary>
        /// <param name="transcriptAddViewModel"></param>
        /// <returns></returns>
        public TranscriptAddViewModel AddTranscriptForStudent(TranscriptAddViewModel transcriptAddViewModel)
        {
            TranscriptAddViewModel transcriptView = new TranscriptAddViewModel();
            try
            {
                int i = 0;
                long? ide = 1;
                if (transcriptAddViewModel.studentListForTranscript?.Any()==true)
                {
                    foreach (var student in transcriptAddViewModel.studentListForTranscript)
                    {
                        List<StudentTranscriptMaster> studentTranscriptMasterList = new List<StudentTranscriptMaster>();
                        List<StudentTranscriptDetail> studentTranscriptDetailsList = new List<StudentTranscriptDetail>();
                        decimal? totalCreditAttempeted = 0.0m;
                        decimal? totalCreditEarned = 0.0m;
                        decimal? cumulativeGPValue = 0.0m;
                        decimal? cumulativeCreditHours = 0.0m;

                        List<StudentTranscriptDetail> studentTranscriptDetails = new List<StudentTranscriptDetail>();

                        var existingStudentTranscriptMasterData = this.context?.StudentTranscriptMaster.Where(x => x.SchoolId == transcriptAddViewModel.SchoolId && x.TenantId == transcriptAddViewModel.TenantId && x.StudentId == student.StudentId).ToList();

                        if (existingStudentTranscriptMasterData?.Any()==true)
                        {
                            var existingStudentTranscriptDetailsData = this.context?.StudentTranscriptDetail.Where(x => x.SchoolId == transcriptAddViewModel.SchoolId && x.TenantId == transcriptAddViewModel.TenantId && x.StudentId == student.StudentId).ToList();
                            if (existingStudentTranscriptDetailsData?.Any()==true)
                            {
                                this.context?.StudentTranscriptDetail.RemoveRange(existingStudentTranscriptDetailsData);
                            }
                            this.context?.StudentTranscriptMaster.RemoveRange(existingStudentTranscriptMasterData);
                            this.context?.SaveChanges();
                        }
                        if (i == 0)
                        {
                            var idData = this.context?.StudentTranscriptDetail.Where(x => x.TenantId == transcriptAddViewModel.TenantId && x.SchoolId == transcriptAddViewModel.SchoolId).OrderByDescending(x => x.Id).FirstOrDefault();

                            if (idData != null)
                            {
                                ide = idData.Id + 1;
                            }
                        }

                        var studentData = this.context?.StudentMaster.Include(x => x.StudentEnrollment).FirstOrDefault(x => x.TenantId == transcriptAddViewModel.TenantId && x.SchoolId == transcriptAddViewModel.SchoolId && x.StudentId == student.StudentId);

                        if (studentData != null)
                        {
                            var gradeIds = (transcriptAddViewModel.GradeLavels??"").Split(",");

                            foreach (var grade in gradeIds.ToList())
                            {
                                decimal? gPValue = 0.0m;
                                decimal? sumOfGPValue = 0.0m;
                                decimal? creditAttemped = 0.0m;
                                decimal? creditEarned = 0.0m;
                                var calenderData = new SchoolCalendars();
                                decimal? GPA = 0.0m;
                                var studentDataWithCurrentGrade = studentData.StudentEnrollment.Where(x => x.GradeId == Convert.ToInt32(grade)).FirstOrDefault();

                                if (studentDataWithCurrentGrade != null)
                                {
                                    calenderData = this.context?.SchoolCalendars.FirstOrDefault(x => x.TenantId == transcriptAddViewModel.TenantId && x.SchoolId == studentDataWithCurrentGrade.SchoolId && x.CalenderId == studentDataWithCurrentGrade.CalenderId);

                                    var reportCardData = this.context?.StudentFinalGrade.Include(x => x.StudentFinalGradeStandard).Where(x => x.TenantId == transcriptAddViewModel.TenantId && x.StudentId == student.StudentId && x.GradeId == Convert.ToInt32(grade)).ToList();

                                    if (reportCardData?.Any()==true)
                                    {
                                        foreach (var reportCard in reportCardData)
                                        {
                                            var gradeData = new Grade();

                                            var courseSectionData = this.context?.CourseSection.Include(x => x.Course).FirstOrDefault(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && x.CourseId == reportCard.CourseId && x.CourseSectionId == reportCard.CourseSectionId);

                                            if (courseSectionData != null)
                                            {
                                                gradeData = this.context?.Grade.AsEnumerable().FirstOrDefault(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && String.Compare(x.Title, reportCard.GradeObtained, true) == 0 && x.GradeScaleId == reportCard.GradeScaleId);
                                                if (gradeData != null)
                                                {
                                                    if (reportCard.CreditEarned != null)
                                                    {
                                                        gPValue = courseSectionData.IsWeightedCourse != true ? reportCard.CreditEarned * gradeData.UnweightedGpValue : reportCard.CreditEarned * gradeData.WeightedGpValue;
                                                        //gpValue=CreditEarned*(WeightedGpValue or UnweightedGpValue)
                                                    }
                                                    else
                                                    {
                                                        gPValue = courseSectionData.IsWeightedCourse != true ? courseSectionData.CreditHours * gradeData.UnweightedGpValue : courseSectionData.CreditHours * gradeData.WeightedGpValue;
                                                        //gpValue=CreditEarned*(WeightedGpValue or UnweightedGpValue)
                                                    }

                                                }
                                                creditAttemped += reportCard.CreditAttempted != null ? reportCard.CreditAttempted : courseSectionData.CreditHours;
                                                creditEarned += reportCard.CreditEarned != null ? reportCard.CreditEarned : courseSectionData.CreditHours;
                                                sumOfGPValue += gPValue;
                                                //}
                                                //creditAttemped += reportCard.CreditAttempted != null ? reportCard.CreditAttempted : courseSectionData.CreditHours;
                                                //creditEarned += reportCard.CreditEarned != null ? reportCard.CreditEarned : courseSectionData.CreditHours;
                                                //sumOfGPValue += gPValue;

                                                var studentTranscriptDetail = new StudentTranscriptDetail()
                                                {
                                                    Id = (long)ide,
                                                    TenantId = (Guid)transcriptAddViewModel.TenantId!,
                                                    SchoolId = (int)transcriptAddViewModel.SchoolId!,
                                                    StudentId = student.StudentId,
                                                    CourseCode = courseSectionData.Course.CourseShortName,
                                                    CourseName = courseSectionData.CourseSectionName,
                                                    CreditHours = courseSectionData.CreditHours,
                                                    CreditEarned = reportCard.CreditEarned != null ? reportCard.CreditEarned : courseSectionData.CreditHours,
                                                    GpValue = gPValue,
                                                    Grade = reportCard.GradeObtained,
                                                    GradeTitle = studentDataWithCurrentGrade.GradeLevelTitle??"",
                                                    CreatedBy = transcriptAddViewModel.CreatedBy,
                                                    CreatedOn = DateTime.UtcNow

                                                };
                                                studentTranscriptDetailsList.Add(studentTranscriptDetail);
                                                ide++;
                                            }
                                        }
                                        this.context?.StudentTranscriptDetail.AddRange(studentTranscriptDetailsList);
                                        GPA = sumOfGPValue / creditEarned;
                                        totalCreditEarned += creditEarned;
                                        totalCreditAttempeted += creditAttemped;
                                        cumulativeGPValue += sumOfGPValue;
                                        cumulativeCreditHours += creditAttemped;
                                    }

                                    var CumulativeGPA = cumulativeGPValue / cumulativeCreditHours;

                                    var studentTranscriptMaster = new StudentTranscriptMaster
                                    {
                                        TenantId = (Guid)transcriptAddViewModel.TenantId!,
                                        SchoolId = (int)transcriptAddViewModel.SchoolId!,
                                        StudentId = (int)student.StudentId,
                                        StudentInternalId = studentData.StudentInternalId,
                                        CumulativeGpa = CumulativeGPA,
                                        TotalCreditAttempted = totalCreditAttempeted,
                                        TotalCreditEarned = totalCreditEarned,
                                        GeneratedOn = DateTime.UtcNow,
                                        CreatedBy = transcriptAddViewModel.CreatedBy,
                                        CreatedOn = DateTime.UtcNow,
                                        SchoolYear = calenderData?.AcademicYear.ToString(),
                                        SchoolName = studentDataWithCurrentGrade.SchoolName,
                                        GradeTitle = studentDataWithCurrentGrade.GradeLevelTitle??"",
                                        TotalGradeCreditEarned = creditEarned,
                                        CreditAttempted = creditAttemped,
                                        Gpa = GPA,
                                    };
                                    studentTranscriptMasterList.Add(studentTranscriptMaster);
                                }
                            }
                        }
                        this.context?.StudentTranscriptMaster.AddRange(studentTranscriptMasterList);
                        i++;
                    }
                    this.context?.SaveChanges();
                    transcriptAddViewModel._message = "added successfully";
                }
                else
                {
                    transcriptAddViewModel._failure = true;
                    transcriptAddViewModel._message = "Select Student Please";
                }
            }
            catch (Exception es)
            {
                transcriptAddViewModel._failure = false;
                transcriptAddViewModel._message = es.Message;
            }
            return transcriptAddViewModel;
        }

        /// <summary>
        /// Generate Pdf Transcript For Student
        /// </summary>
        /// <param name="transcriptAddViewModel"></param>
        /// <returns></returns>
        public async Task<TranscriptAddViewModel> GenerateTranscriptForStudent(TranscriptAddViewModel transcriptAddViewModel)
        {
            TranscriptAddViewModel transcriptView = new TranscriptAddViewModel();
            try
            {
                transcriptView.TenantId = transcriptAddViewModel.TenantId;
                transcriptView.SchoolId = transcriptAddViewModel.SchoolId;
                transcriptView._tenantName = transcriptAddViewModel._tenantName;
                transcriptView._userName = transcriptAddViewModel._userName;
                string? base64 = null;
                object data = new object();

                List<object> transcriptList = new List<object>();
                List<object> teacherCommentList = new List<object>();

                foreach (var student in transcriptAddViewModel.studentListForTranscript)
                {
                    var studentData = this.context?.StudentMaster.FirstOrDefault(x => x.SchoolId == transcriptAddViewModel.SchoolId && x.TenantId == transcriptAddViewModel.TenantId && x.StudentId == student.StudentId);

                    var schoolData = this.context?.SchoolMaster.Include(m => m.SchoolDetail).FirstOrDefault(x => x.SchoolId == transcriptAddViewModel.SchoolId && x.TenantId == transcriptAddViewModel.TenantId);

                    var studentTranscriptData = this.context?.StudentTranscriptMaster.Include(x => x.StudentTranscriptDetail).Where(x => x.SchoolId == transcriptAddViewModel.SchoolId && x.TenantId == transcriptAddViewModel.TenantId && x.StudentId == student.StudentId).ToList();

                    var gradeData = this.context?.Grade.Where(x => x.TenantId == transcriptAddViewModel.TenantId && x.SchoolId == transcriptAddViewModel.SchoolId).ToList();

                    if (studentData != null && schoolData != null && studentTranscriptData?.Any()==true)
                    {
                        List<object> transcriptDetailsList = new List<object>();
                        //studentData.StudentPhoto = transcriptAddViewModel.StudentPhoto == true ? studentData.StudentPhoto : null;
                        studentData.StudentPhoto = transcriptAddViewModel.StudentPhoto == true ? studentData.StudentThumbnailPhoto : null;
                        string? studentDob = studentData.Dob.HasValue == true ? studentData.Dob.Value.ToShortDateString() : null;
                        foreach (var studentTranscript in studentTranscriptData)
                        {
                            var studentTranscriptDetailsData = studentTranscript.StudentTranscriptDetail.Where(x => x.GradeTitle == studentTranscript.GradeTitle).ToList();

                            object gardeLevelWiseData = new
                            {
                                studentTranscript.GradeTitle,
                                studentTranscript.SchoolName,
                                studentTranscript.SchoolYear,
                                studentTranscript.CreditAttempted,
                                studentTranscript.TotalGradeCreditEarned,
                                studentTranscript.Gpa,
                                Details = studentTranscriptDetailsData
                            };
                            transcriptDetailsList.Add(gardeLevelWiseData);
                        }

                        object transcript = new
                        {
                            SchoolData = schoolData,
                            nameOfPrincipal = schoolData.SchoolDetail != null ? schoolData.SchoolDetail.FirstOrDefault()!.NameOfPrincipal : null,
                            TasnscriptdetailsData = transcriptDetailsList,
                            StudentData = studentData,
                            StudentDob = studentDob,
                            cumulativeGpa = studentTranscriptData.LastOrDefault()!.CumulativeGpa,
                            totalCreditEarned = studentTranscriptData.LastOrDefault()!.TotalCreditEarned,
                            totalCreditAttempted = studentTranscriptData.LastOrDefault()!.TotalCreditAttempted,
                            GradeDetails = transcriptAddViewModel.GradeLagend == true ? gradeData : null,
                        };
                        transcriptList.Add(transcript);
                    }
                }
                if (transcriptList != null)
                {
                    data = new
                    {
                        TotalData = transcriptList
                    };
                }

                GenerateTranscript _transcript = new GenerateTranscript();
                var message = await _transcript.Generate(data);

                bool isWindows = System.Runtime.InteropServices.RuntimeInformation
                                                .IsOSPlatform(OSPlatform.Windows);
                if (isWindows)
                {
                    using (var fileStream = new FileStream(@"ReportCard\\StudentTranscript.pdf", FileMode.Open))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            fileStream.CopyTo(memoryStream);
                            byte[] bytes = memoryStream.ToArray();
                            base64 = Convert.ToBase64String(bytes);
                            fileStream.Close();
                        }
                    }
                    transcriptView.TranscriptPdf = base64;
                }
                else
                {
                    using (var fileStream = new FileStream(@"ReportCard/StudentTranscript.pdf", FileMode.Open))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            fileStream.CopyTo(memoryStream);
                            byte[] bytes = memoryStream.ToArray();
                            base64 = Convert.ToBase64String(bytes);
                            fileStream.Close();
                        }
                    }
                    transcriptView.TranscriptPdf = base64;
                }
            }
            catch (Exception es)
            {
                transcriptView._message = es.Message;
                transcriptView._failure = true;
            }
            return transcriptView;
        }

        /// <summary>
        /// Add Student Medical Alert
        /// </summary>
        /// <param name="studentMedicalAlertAddViewModel"></param>
        /// <returns></returns>
        public StudentMedicalAlertAddViewModel AddStudentMedicalAlert(StudentMedicalAlertAddViewModel studentMedicalAlertAddViewModel)
        {
            try
            {
                int ide = 1;
                var studentMedicalAlertData = this.context?.StudentMedicalAlert.Where(x => x.TenantId == studentMedicalAlertAddViewModel.studentMedicalAlert!.TenantId && x.SchoolId == studentMedicalAlertAddViewModel.studentMedicalAlert.SchoolId).OrderByDescending(x => x.Id).FirstOrDefault();

                if (studentMedicalAlertData != null)
                {
                    ide = studentMedicalAlertData.Id + 1;
                }
                studentMedicalAlertAddViewModel.studentMedicalAlert!.Id = ide;
                studentMedicalAlertAddViewModel.studentMedicalAlert.CreatedOn = DateTime.UtcNow;
                this.context?.StudentMedicalAlert.Add(studentMedicalAlertAddViewModel.studentMedicalAlert);
                this.context?.SaveChanges();
                studentMedicalAlertAddViewModel._failure = false;
                studentMedicalAlertAddViewModel._message = "Student Medical Alert added successfully";
            }
            catch (Exception es)
            {
                studentMedicalAlertAddViewModel._message = es.Message;
                studentMedicalAlertAddViewModel._failure = true;
            }
            return studentMedicalAlertAddViewModel;
        }

        /// <summary>
        /// Update Student Medical Alert
        /// </summary>
        /// <param name="studentMedicalAlertAddViewModel"></param>
        /// <returns></returns>
        public StudentMedicalAlertAddViewModel UpdateStudentMedicalAlert(StudentMedicalAlertAddViewModel studentMedicalAlertAddViewModel)
        {
            try
            {
                var studentMedicalAlertData = this.context?.StudentMedicalAlert.FirstOrDefault(x => x.TenantId == studentMedicalAlertAddViewModel.studentMedicalAlert!.TenantId && x.SchoolId == studentMedicalAlertAddViewModel.studentMedicalAlert.SchoolId && x.StudentId == studentMedicalAlertAddViewModel.studentMedicalAlert.StudentId && x.Id == studentMedicalAlertAddViewModel.studentMedicalAlert.Id);

                if (studentMedicalAlertData != null)
                {
                    studentMedicalAlertAddViewModel.studentMedicalAlert!.CreatedOn = studentMedicalAlertData.CreatedOn;
                    studentMedicalAlertAddViewModel.studentMedicalAlert.CreatedBy = studentMedicalAlertData.CreatedBy;
                    studentMedicalAlertAddViewModel.studentMedicalAlert.UpdatedOn = DateTime.UtcNow;
                    this.context?.Entry(studentMedicalAlertData).CurrentValues.SetValues(studentMedicalAlertAddViewModel.studentMedicalAlert);
                    this.context?.SaveChanges();
                    studentMedicalAlertAddViewModel._failure = false;
                    studentMedicalAlertAddViewModel._message = "Student Medical Alert Updated Successfully";
                }
                else
                {
                    studentMedicalAlertAddViewModel._message = NORECORDFOUND;
                    studentMedicalAlertAddViewModel._failure = true;
                }

            }
            catch (Exception es)
            {
                studentMedicalAlertAddViewModel._message = es.Message;
                studentMedicalAlertAddViewModel._failure = true;
            }
            return studentMedicalAlertAddViewModel;
        }

        /// <summary>
        /// Delete Student Medical Alert
        /// </summary>
        /// <param name="studentMedicalAlertAddViewModel"></param>
        /// <returns></returns>
        public StudentMedicalAlertAddViewModel DeleteStudentMedicalAlert(StudentMedicalAlertAddViewModel studentMedicalAlertAddViewModel)
        {
            try
            {
                var studentMedicalAlertData = this.context?.StudentMedicalAlert.FirstOrDefault(x => x.TenantId == studentMedicalAlertAddViewModel.studentMedicalAlert!.TenantId && x.SchoolId == studentMedicalAlertAddViewModel.studentMedicalAlert.SchoolId && x.StudentId == studentMedicalAlertAddViewModel.studentMedicalAlert.StudentId && x.Id == studentMedicalAlertAddViewModel.studentMedicalAlert.Id);

                if (studentMedicalAlertData != null)
                {
                    this.context?.StudentMedicalAlert.Remove(studentMedicalAlertData);
                    this.context?.SaveChanges();
                    studentMedicalAlertAddViewModel._failure = false;
                    studentMedicalAlertAddViewModel._message = "Student Medical Alert deleted successfullyy";
                }
                else
                {
                    studentMedicalAlertAddViewModel._message = NORECORDFOUND;
                    studentMedicalAlertAddViewModel._failure = true;
                }
            }
            catch (Exception es)
            {
                studentMedicalAlertAddViewModel._message = es.Message;
                studentMedicalAlertAddViewModel._failure = true;
            }
            return studentMedicalAlertAddViewModel;
        }

        /// <summary>
        /// Add Student Medical Note
        /// </summary>
        /// <param name="studentMedicalAlertAddViewModel"></param>
        /// <returns></returns>
        public StudentMedicalNoteAddViewModel AddStudentMedicalNote(StudentMedicalNoteAddViewModel studentMedicalNoteAddViewModel)
        {
            try
            {
                int ide = 1;
                var studentMedicalNoteData = this.context?.StudentMedicalNote.Where(x => x.TenantId == studentMedicalNoteAddViewModel.studentMedicalNote!.TenantId && x.SchoolId == studentMedicalNoteAddViewModel.studentMedicalNote.SchoolId).OrderByDescending(x => x.Id).FirstOrDefault();

                if (studentMedicalNoteData != null)
                {
                    ide = studentMedicalNoteData.Id + 1;
                }
                studentMedicalNoteAddViewModel.studentMedicalNote!.Id = ide;
                studentMedicalNoteAddViewModel.studentMedicalNote.CreatedOn = DateTime.UtcNow;
                this.context?.StudentMedicalNote.Add(studentMedicalNoteAddViewModel.studentMedicalNote);
                this.context?.SaveChanges();
                studentMedicalNoteAddViewModel._failure = false;
                studentMedicalNoteAddViewModel._message = "Student Medical Note added successfully";
            }
            catch (Exception es)
            {
                studentMedicalNoteAddViewModel._message = es.Message;
                studentMedicalNoteAddViewModel._failure = true;
            }
            return studentMedicalNoteAddViewModel;
        }

        /// <summary>
        /// Update Student Medical Note
        /// </summary>
        /// <param name="studentMedicalAlertAddViewModel"></param>
        /// <returns></returns>
        public StudentMedicalNoteAddViewModel UpdateStudentMedicalNote(StudentMedicalNoteAddViewModel studentMedicalNoteAddViewModel)
        {
            try
            {
                var studentMedicalNoteData = this.context?.StudentMedicalNote.FirstOrDefault(x => x.TenantId == studentMedicalNoteAddViewModel.studentMedicalNote!.TenantId && x.SchoolId == studentMedicalNoteAddViewModel.studentMedicalNote.SchoolId && x.StudentId == studentMedicalNoteAddViewModel.studentMedicalNote.StudentId && x.Id == studentMedicalNoteAddViewModel.studentMedicalNote.Id);

                if (studentMedicalNoteData != null)
                {
                    studentMedicalNoteAddViewModel.studentMedicalNote!.CreatedOn = studentMedicalNoteData.CreatedOn;
                    studentMedicalNoteAddViewModel.studentMedicalNote.CreatedBy = studentMedicalNoteData.CreatedBy;
                    studentMedicalNoteAddViewModel.studentMedicalNote.UpdatedOn = DateTime.UtcNow;
                    this.context?.Entry(studentMedicalNoteData).CurrentValues.SetValues(studentMedicalNoteAddViewModel.studentMedicalNote);
                    this.context?.SaveChanges();
                    studentMedicalNoteAddViewModel._failure = false;
                    studentMedicalNoteAddViewModel._message = "Student Medical Note Updated Successfully";
                }
                else
                {
                    studentMedicalNoteAddViewModel._message = NORECORDFOUND;
                    studentMedicalNoteAddViewModel._failure = true;
                }
            }
            catch (Exception es)
            {
                studentMedicalNoteAddViewModel._message = es.Message;
                studentMedicalNoteAddViewModel._failure = true;
            }
            return studentMedicalNoteAddViewModel;
        }

        /// <summary>
        /// Delete Student Medical Note
        /// </summary>
        /// <param name="studentMedicalAlertAddViewModel"></param>
        /// <returns></returns>
        public StudentMedicalNoteAddViewModel DeleteStudentMedicalNote(StudentMedicalNoteAddViewModel studentMedicalNoteAddViewModel)
        {
            try
            {
                var studentMedicalNoteData = this.context?.StudentMedicalNote.FirstOrDefault(x => x.TenantId == studentMedicalNoteAddViewModel.studentMedicalNote!.TenantId && x.SchoolId == studentMedicalNoteAddViewModel.studentMedicalNote.SchoolId && x.StudentId == studentMedicalNoteAddViewModel.studentMedicalNote.StudentId && x.Id == studentMedicalNoteAddViewModel.studentMedicalNote.Id);

                if (studentMedicalNoteData != null)
                {
                    this.context?.StudentMedicalNote.Remove(studentMedicalNoteData);
                    this.context?.SaveChanges();
                    studentMedicalNoteAddViewModel._failure = false;
                    studentMedicalNoteAddViewModel._message = "Student Medical Note deleted successfullyy";
                }
                else
                {
                    studentMedicalNoteAddViewModel._message = NORECORDFOUND;
                    studentMedicalNoteAddViewModel._failure = true;
                }
            }
            catch (Exception es)
            {
                studentMedicalNoteAddViewModel._message = es.Message;
                studentMedicalNoteAddViewModel._failure = true;
            }
            return studentMedicalNoteAddViewModel;
        }

        /// <summary>
        /// Add Student Medical Immunization
        /// </summary>
        /// <param name="studentMedicalImmunizationAddViewModel"></param>
        /// <returns></returns>
        public StudentMedicalImmunizationAddViewModel AddStudentMedicalImmunization(StudentMedicalImmunizationAddViewModel studentMedicalImmunizationAddViewModel)
        {
            try
            {
                int ide = 1;
                var studentMedicalImmunizationData = this.context?.StudentMedicalImmunization.Where(x => x.TenantId == studentMedicalImmunizationAddViewModel.studentMedicalImmunization!.TenantId && x.SchoolId == studentMedicalImmunizationAddViewModel.studentMedicalImmunization.SchoolId).OrderByDescending(x => x.Id).FirstOrDefault();

                if (studentMedicalImmunizationData != null)
                {
                    ide = studentMedicalImmunizationData.Id + 1;
                }
                studentMedicalImmunizationAddViewModel.studentMedicalImmunization!.Id = ide;
                studentMedicalImmunizationAddViewModel.studentMedicalImmunization.CreatedOn = DateTime.UtcNow;
                this.context?.StudentMedicalImmunization.Add(studentMedicalImmunizationAddViewModel.studentMedicalImmunization);
                this.context?.SaveChanges();
                studentMedicalImmunizationAddViewModel._failure = false;
                studentMedicalImmunizationAddViewModel._message = "Student Medical Immunization added successfully";
            }
            catch (Exception es)
            {
                studentMedicalImmunizationAddViewModel._message = es.Message;
                studentMedicalImmunizationAddViewModel._failure = true;
            }
            return studentMedicalImmunizationAddViewModel;
        }

        /// <summary>
        /// Update Student Medical Immunization
        /// </summary>
        /// <param name="studentMedicalImmunizationAddViewModel"></param>
        /// <returns></returns>
        public StudentMedicalImmunizationAddViewModel UpdateStudentMedicalImmunization(StudentMedicalImmunizationAddViewModel studentMedicalImmunizationAddViewModel)
        {
            try
            {
                var studentMedicalImmunizationData = this.context?.StudentMedicalImmunization.FirstOrDefault(x => x.TenantId == studentMedicalImmunizationAddViewModel.studentMedicalImmunization!.TenantId && x.SchoolId == studentMedicalImmunizationAddViewModel.studentMedicalImmunization.SchoolId && x.StudentId == studentMedicalImmunizationAddViewModel.studentMedicalImmunization.StudentId && x.Id == studentMedicalImmunizationAddViewModel.studentMedicalImmunization.Id);

                if (studentMedicalImmunizationData != null)
                {
                    studentMedicalImmunizationAddViewModel.studentMedicalImmunization!.CreatedOn = studentMedicalImmunizationData.CreatedOn;
                    studentMedicalImmunizationAddViewModel.studentMedicalImmunization.CreatedBy = studentMedicalImmunizationData.CreatedBy;
                    studentMedicalImmunizationAddViewModel.studentMedicalImmunization.UpdatedOn = DateTime.UtcNow;
                    this.context?.Entry(studentMedicalImmunizationData).CurrentValues.SetValues(studentMedicalImmunizationAddViewModel.studentMedicalImmunization);
                    this.context?.SaveChanges();
                    studentMedicalImmunizationAddViewModel._failure = false;
                    studentMedicalImmunizationAddViewModel._message = "Student Medical Immunization Updated Successfully";
                }
                else
                {
                    studentMedicalImmunizationAddViewModel._message = NORECORDFOUND;
                    studentMedicalImmunizationAddViewModel._failure = true;
                }
            }
            catch (Exception es)
            {
                studentMedicalImmunizationAddViewModel._message = es.Message;
                studentMedicalImmunizationAddViewModel._failure = true;
            }
            return studentMedicalImmunizationAddViewModel;
        }

        /// <summary>
        /// Delete Student Medical Immunization
        /// </summary>
        /// <param name="studentMedicalImmunizationAddViewModel"></param>
        /// <returns></returns>
        public StudentMedicalImmunizationAddViewModel DeleteStudentMedicalImmunization(StudentMedicalImmunizationAddViewModel studentMedicalImmunizationAddViewModel)
        {
            try
            {
                var studentMedicalImmunizationData = this.context?.StudentMedicalImmunization.FirstOrDefault(x => x.TenantId == studentMedicalImmunizationAddViewModel.studentMedicalImmunization!.TenantId && x.SchoolId == studentMedicalImmunizationAddViewModel.studentMedicalImmunization.SchoolId && x.StudentId == studentMedicalImmunizationAddViewModel.studentMedicalImmunization.StudentId && x.Id == studentMedicalImmunizationAddViewModel.studentMedicalImmunization.Id);

                if (studentMedicalImmunizationData != null)
                {
                    this.context?.StudentMedicalImmunization.Remove(studentMedicalImmunizationData);
                    this.context?.SaveChanges();
                    studentMedicalImmunizationAddViewModel._failure = false;
                    studentMedicalImmunizationAddViewModel._message = "Student Medical Immunization deleted successfullyy";
                }
                else
                {
                    studentMedicalImmunizationAddViewModel._message = NORECORDFOUND;
                    studentMedicalImmunizationAddViewModel._failure = true;
                }
            }
            catch (Exception es)
            {
                studentMedicalImmunizationAddViewModel._message = es.Message;
                studentMedicalImmunizationAddViewModel._failure = true;
            }
            return studentMedicalImmunizationAddViewModel;
        }

        /// <summary>
        /// Add Student Medical Nurse Visit
        /// </summary>
        /// <param name="studentMedicalImmunizationAddViewModel"></param>
        /// <returns></returns>
        public StudentMedicalNurseVisitAddViewModel AddStudentMedicalNurseVisit(StudentMedicalNurseVisitAddViewModel studentMedicalNurseVisitAddViewModel)
        {
            try
            {
                int ide = 1;
                var studentMedicalNurseVisitData = this.context?.StudentMedicalNurseVisit.Where(x => x.TenantId == studentMedicalNurseVisitAddViewModel.studentMedicalNurseVisit!.TenantId && x.SchoolId == studentMedicalNurseVisitAddViewModel.studentMedicalNurseVisit.SchoolId).OrderByDescending(x => x.Id).FirstOrDefault();

                if (studentMedicalNurseVisitData != null)
                {
                    ide = studentMedicalNurseVisitData.Id + 1;
                }
                studentMedicalNurseVisitAddViewModel.studentMedicalNurseVisit!.Id = ide;
                studentMedicalNurseVisitAddViewModel.studentMedicalNurseVisit.CreatedOn = DateTime.UtcNow;
                this.context?.StudentMedicalNurseVisit.Add(studentMedicalNurseVisitAddViewModel.studentMedicalNurseVisit);
                this.context?.SaveChanges();
                studentMedicalNurseVisitAddViewModel._failure = false;
                studentMedicalNurseVisitAddViewModel._message = "Student Medical Nurse Visit added successfully";
            }
            catch (Exception es)
            {
                studentMedicalNurseVisitAddViewModel._message = es.Message;
                studentMedicalNurseVisitAddViewModel._failure = true;
            }
            return studentMedicalNurseVisitAddViewModel;
        }

        /// <summary>
        /// Update Student Medical Nurse Visit
        /// </summary>
        /// <param name="studentMedicalImmunizationAddViewModel"></param>
        /// <returns></returns>
        public StudentMedicalNurseVisitAddViewModel UpdateStudentMedicalNurseVisit(StudentMedicalNurseVisitAddViewModel studentMedicalNurseVisitAddViewModel)
        {
            try
            {
                var studentMedicalNurseVisitData = this.context?.StudentMedicalNurseVisit.FirstOrDefault(x => x.TenantId == studentMedicalNurseVisitAddViewModel.studentMedicalNurseVisit!.TenantId && x.SchoolId == studentMedicalNurseVisitAddViewModel.studentMedicalNurseVisit.SchoolId && x.StudentId == studentMedicalNurseVisitAddViewModel.studentMedicalNurseVisit.StudentId && x.Id == studentMedicalNurseVisitAddViewModel.studentMedicalNurseVisit.Id);

                if (studentMedicalNurseVisitData != null)
                {
                    studentMedicalNurseVisitAddViewModel.studentMedicalNurseVisit!.CreatedOn = studentMedicalNurseVisitData.CreatedOn;
                    studentMedicalNurseVisitAddViewModel.studentMedicalNurseVisit.CreatedBy = studentMedicalNurseVisitData.CreatedBy;
                    studentMedicalNurseVisitAddViewModel.studentMedicalNurseVisit.UpdatedOn = DateTime.UtcNow;
                    this.context?.Entry(studentMedicalNurseVisitData).CurrentValues.SetValues(studentMedicalNurseVisitAddViewModel.studentMedicalNurseVisit);
                    this.context?.SaveChanges();
                    studentMedicalNurseVisitAddViewModel._failure = false;
                    studentMedicalNurseVisitAddViewModel._message = "Student Medical Nurse Visit Updated Successfully";
                }
                else
                {
                    studentMedicalNurseVisitAddViewModel._message = NORECORDFOUND;
                    studentMedicalNurseVisitAddViewModel._failure = true;
                }
            }
            catch (Exception es)
            {
                studentMedicalNurseVisitAddViewModel._message = es.Message;
                studentMedicalNurseVisitAddViewModel._failure = true;
            }
            return studentMedicalNurseVisitAddViewModel;
        }

        /// <summary>
        /// Delete Student Medical Nurse Visit
        /// </summary>
        /// <param name="studentMedicalImmunizationAddViewModel"></param>
        /// <returns></returns>
        public StudentMedicalNurseVisitAddViewModel DeleteStudentMedicalNurseVisit(StudentMedicalNurseVisitAddViewModel studentMedicalNurseVisitAddViewModel)
        {
            try
            {
                var studentMedicalNurseVisitData = this.context?.StudentMedicalNurseVisit.FirstOrDefault(x => x.TenantId == studentMedicalNurseVisitAddViewModel.studentMedicalNurseVisit!.TenantId && x.SchoolId == studentMedicalNurseVisitAddViewModel.studentMedicalNurseVisit.SchoolId && x.StudentId == studentMedicalNurseVisitAddViewModel.studentMedicalNurseVisit.StudentId && x.Id == studentMedicalNurseVisitAddViewModel.studentMedicalNurseVisit.Id);

                if (studentMedicalNurseVisitData != null)
                {
                    this.context?.StudentMedicalNurseVisit.Remove(studentMedicalNurseVisitData);
                    this.context?.SaveChanges();
                    studentMedicalNurseVisitAddViewModel._failure = false;
                    studentMedicalNurseVisitAddViewModel._message = "Student Medical Nurse Visit deleted successfullyy";
                }
                else
                {
                    studentMedicalNurseVisitAddViewModel._message = NORECORDFOUND;
                    studentMedicalNurseVisitAddViewModel._failure = true;
                }
            }
            catch (Exception es)
            {
                studentMedicalNurseVisitAddViewModel._message = es.Message;
                studentMedicalNurseVisitAddViewModel._failure = true;
            }
            return studentMedicalNurseVisitAddViewModel;
        }

        /// <summary>
        /// Add Student Medical Provider
        /// </summary>
        /// <param name="studentMedicalProviderAddViewModel"></param>
        /// <returns></returns>
        public StudentMedicalProviderAddViewModel AddStudentMedicalProvider(StudentMedicalProviderAddViewModel studentMedicalProviderAddViewModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    int ide = 1;
                    var studentMedicalProviderData = this.context?.StudentMedicalProvider.Where(x => x.TenantId == studentMedicalProviderAddViewModel.studentMedicalProvider!.TenantId && x.SchoolId == studentMedicalProviderAddViewModel.studentMedicalProvider.SchoolId).OrderByDescending(x => x.Id).FirstOrDefault();

                    if (studentMedicalProviderData != null)
                    {
                        ide = studentMedicalProviderData.Id + 1;
                    }
                    studentMedicalProviderAddViewModel.studentMedicalProvider!.Id = ide;
                    studentMedicalProviderAddViewModel.studentMedicalProvider.CreatedOn = DateTime.UtcNow;
                    this.context?.StudentMedicalProvider.Add(studentMedicalProviderAddViewModel.studentMedicalProvider);
                    this.context?.SaveChanges();

                    if (studentMedicalProviderAddViewModel.fieldsCategoryList != null && studentMedicalProviderAddViewModel.fieldsCategoryList.ToList()?.Any()==true)
                    {
                        var fieldsCategory = studentMedicalProviderAddViewModel.fieldsCategoryList.FirstOrDefault(x => x.CategoryId == studentMedicalProviderAddViewModel.SelectedCategoryId);
                        if (fieldsCategory != null)
                        {
                            foreach (var customFields in fieldsCategory.CustomFields.ToList())
                            {
                                if (customFields.CustomFieldsValue != null && customFields.CustomFieldsValue.ToList()?.Any()==true)
                                {
                                    customFields.CustomFieldsValue.FirstOrDefault()!.Module = "Student";
                                    customFields.CustomFieldsValue.FirstOrDefault()!.CategoryId = customFields.CategoryId;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.FieldId = customFields.FieldId;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldTitle = customFields.Title;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldType = customFields.Type;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.SchoolId = studentMedicalProviderAddViewModel.studentMedicalProvider.SchoolId;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.TenantId = studentMedicalProviderAddViewModel.studentMedicalProvider.TenantId;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.TargetId = studentMedicalProviderAddViewModel.studentMedicalProvider.StudentId;
                                    this.context?.CustomFieldsValue.AddRange(customFields.CustomFieldsValue);
                                    this.context?.SaveChanges();
                                }
                            }
                        }
                    }

                    studentMedicalProviderAddViewModel._failure = false;
                    studentMedicalProviderAddViewModel._message = "Student Medical Provider added successfully";
                    transaction?.Commit();
                }

                catch (Exception es)
                {
                    transaction?.Rollback();
                    studentMedicalProviderAddViewModel._message = es.Message;
                    studentMedicalProviderAddViewModel._failure = true;
                }
            }
            return studentMedicalProviderAddViewModel;
        }

        /// <summary>
        /// Update Student Medical Provider
        /// </summary>
        /// <param name="studentMedicalProviderAddViewModel"></param>
        /// <returns></returns>
        public StudentMedicalProviderAddViewModel UpdateStudentMedicalProvider(StudentMedicalProviderAddViewModel studentMedicalProviderAddViewModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    var studentMedicalProviderData = this.context?.StudentMedicalProvider.FirstOrDefault(x => x.TenantId == studentMedicalProviderAddViewModel.studentMedicalProvider!.TenantId && x.SchoolId == studentMedicalProviderAddViewModel.studentMedicalProvider.SchoolId && x.StudentId == studentMedicalProviderAddViewModel.studentMedicalProvider.StudentId && x.Id == studentMedicalProviderAddViewModel.studentMedicalProvider.Id);

                    if (studentMedicalProviderData != null)
                    {
                        studentMedicalProviderAddViewModel.studentMedicalProvider!.CreatedOn = studentMedicalProviderData.CreatedOn;
                        studentMedicalProviderAddViewModel.studentMedicalProvider.CreatedBy = studentMedicalProviderData.CreatedBy;
                        studentMedicalProviderAddViewModel.studentMedicalProvider.UpdatedOn = DateTime.UtcNow;
                        this.context?.Entry(studentMedicalProviderData).CurrentValues.SetValues(studentMedicalProviderAddViewModel.studentMedicalProvider);
                        this.context?.SaveChanges();

                        if (studentMedicalProviderAddViewModel.fieldsCategoryList != null && studentMedicalProviderAddViewModel.fieldsCategoryList.ToList()?.Any()==true)
                        {
                            var fieldsCategory = studentMedicalProviderAddViewModel.fieldsCategoryList.FirstOrDefault(x => x.CategoryId == studentMedicalProviderAddViewModel.SelectedCategoryId);
                            if (fieldsCategory != null)
                            {
                                foreach (var customFields in fieldsCategory.CustomFields.ToList())
                                {
                                    var customFieldValueData = this.context?.CustomFieldsValue.FirstOrDefault(x => x.TenantId == studentMedicalProviderAddViewModel.studentMedicalProvider.TenantId && x.SchoolId == studentMedicalProviderAddViewModel.studentMedicalProvider.SchoolId && x.CategoryId == customFields.CategoryId && x.FieldId == customFields.FieldId && x.Module == "Student" && x.TargetId == studentMedicalProviderAddViewModel.studentMedicalProvider.StudentId);
                                    if (customFieldValueData != null)
                                    {
                                        this.context?.CustomFieldsValue.RemoveRange(customFieldValueData);
                                    }
                                    if (customFields.CustomFieldsValue != null && customFields.CustomFieldsValue.ToList()?.Any()==true)
                                    {
                                        customFields.CustomFieldsValue.FirstOrDefault()!.Module = "Student";
                                        customFields.CustomFieldsValue.FirstOrDefault()!.CategoryId = customFields.CategoryId;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.FieldId = customFields.FieldId;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldTitle = customFields.Title;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldType = customFields.Type;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.SchoolId = studentMedicalProviderAddViewModel.studentMedicalProvider.SchoolId;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.TenantId = studentMedicalProviderAddViewModel.studentMedicalProvider.TenantId;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.TargetId = studentMedicalProviderAddViewModel.studentMedicalProvider.StudentId;
                                        this.context?.CustomFieldsValue.AddRange(customFields.CustomFieldsValue);
                                        this.context?.SaveChanges();
                                    }
                                }
                            }
                        }
                        studentMedicalProviderAddViewModel._failure = false;
                        studentMedicalProviderAddViewModel._message = "Student Medical Provider Updated Successfully";
                        transaction?.Commit();
                    }
                    else
                    {
                        studentMedicalProviderAddViewModel._message = NORECORDFOUND;
                        studentMedicalProviderAddViewModel._failure = true;
                    }
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    studentMedicalProviderAddViewModel._message = es.Message;
                    studentMedicalProviderAddViewModel._failure = true;
                }
            }
            return studentMedicalProviderAddViewModel;
        }

        /// <summary>
        /// Delete Student Medical Provider
        /// </summary>
        /// <param name="studentMedicalProviderAddViewModel"></param>
        /// <returns></returns>
        public StudentMedicalProviderAddViewModel DeleteStudentMedicalProvider(StudentMedicalProviderAddViewModel studentMedicalProviderAddViewModel)
        {
            try
            {
                var studentMedicalProviderData = this.context?.StudentMedicalProvider.FirstOrDefault(x => x.TenantId == studentMedicalProviderAddViewModel.studentMedicalProvider!.TenantId && x.SchoolId == studentMedicalProviderAddViewModel.studentMedicalProvider.SchoolId && x.StudentId == studentMedicalProviderAddViewModel.studentMedicalProvider.StudentId && x.Id == studentMedicalProviderAddViewModel.studentMedicalProvider.Id);

                if (studentMedicalProviderData != null)
                {
                    this.context?.StudentMedicalProvider.Remove(studentMedicalProviderData);
                    this.context?.SaveChanges();
                    studentMedicalProviderAddViewModel._failure = false;
                    studentMedicalProviderAddViewModel._message = "Student Medical Provider deleted successfullyy";
                }
                else
                {
                    studentMedicalProviderAddViewModel._message = NORECORDFOUND;
                    studentMedicalProviderAddViewModel._failure = true;
                }
            }
            catch (Exception es)
            {
                studentMedicalProviderAddViewModel._message = es.Message;
                studentMedicalProviderAddViewModel._failure = true;
            }
            return studentMedicalProviderAddViewModel;
        }

        /// <summary>
        /// Get All Student Medical Info
        /// </summary>
        /// <param name="studentMedicalInfoViewModel"></param>
        /// <returns></returns>
        public StudentMedicalInfoViewModel GetAllStudentMedicalInfo(StudentMedicalInfoViewModel studentMedicalInfoViewModel)
        {
            StudentMedicalInfoViewModel studentMedicalInfoList = new StudentMedicalInfoViewModel();
            try
            {
                studentMedicalInfoList.TenantId = studentMedicalInfoViewModel.TenantId;
                studentMedicalInfoList.SchoolId = studentMedicalInfoViewModel.SchoolId;
                studentMedicalInfoList.StudentId = studentMedicalInfoViewModel.StudentId;
                studentMedicalInfoList._tenantName = studentMedicalInfoViewModel._tenantName;
                studentMedicalInfoList._userName = studentMedicalInfoViewModel._userName;

                var studentData = this.context?.StudentMaster.Include(x => x.StudentMedicalAlert).Include(x => x.StudentMedicalNote).Include(x => x.StudentMedicalImmunization).Include(x => x.StudentMedicalNurseVisit).Include(x => x.StudentMedicalProvider).FirstOrDefault(x => x.TenantId == studentMedicalInfoViewModel.TenantId && x.SchoolId == studentMedicalInfoViewModel.SchoolId && x.StudentId == studentMedicalInfoViewModel.StudentId);

                if (studentData != null)
                {
                    studentMedicalInfoList.studentMedicalAlertList = studentData.StudentMedicalAlert.ToList();
                    studentMedicalInfoList.studentMedicalNoteList = studentData.StudentMedicalNote.ToList();
                    studentMedicalInfoList.studentMedicalImmunizationList = studentData.StudentMedicalImmunization.ToList();
                    studentMedicalInfoList.studentMedicalNurseVisitList = studentData.StudentMedicalNurseVisit.ToList();
                    studentMedicalInfoList.studentMedicalProviderList = studentData.StudentMedicalProvider.ToList();

                    studentMedicalInfoList.studentMedicalAlertList.ForEach(x => x.StudentMaster = new());
                    studentMedicalInfoList.studentMedicalNoteList.ForEach(x => x.StudentMaster = new());
                    studentMedicalInfoList.studentMedicalImmunizationList.ForEach(x => x.StudentMaster = new());
                    studentMedicalInfoList.studentMedicalNurseVisitList.ForEach(x => x.StudentMaster = new());
                    studentMedicalInfoList.studentMedicalProviderList.ForEach(x => x.StudentMaster = new());

                    var fieldsCategories = this.context?.FieldsCategory.Where(x => x.TenantId == studentMedicalInfoViewModel.TenantId && x.SchoolId == studentMedicalInfoViewModel.SchoolId && x.Module == "Student").OrderByDescending(x => x.IsSystemCategory).ThenBy(x => x.SortOrder)
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
                           CreatedOn =y.CreatedOn,
                           CreatedBy= y.CreatedBy,
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
                               DefaultSelection = z.DefaultSelection,
                               UpdatedOn = z.UpdatedOn,
                               UpdatedBy = z.UpdatedBy,
                               CreatedOn = z.CreatedOn,
                               CreatedBy = z.CreatedBy,
                               CustomFieldsValue = z.CustomFieldsValue.Where(w => w.TargetId == studentMedicalInfoViewModel.StudentId).ToList()
                           }).OrderByDescending(x => x.SystemField).ThenBy(x => x.SortOrder).ToList()
                       }).ToList();

                    studentMedicalInfoList.fieldsCategoryList = fieldsCategories??new();
                }
                else
                {
                    studentMedicalInfoList._message = NORECORDFOUND;
                    studentMedicalInfoList._failure = true;
                }
            }
            catch (Exception es)
            {
                studentMedicalInfoList._message = es.Message;
                studentMedicalInfoList._failure = true;
            }
            return studentMedicalInfoList;
        }

        /// <summary>
        /// Assign General Info For Students
        /// </summary>
        /// <param name="studentListAddViewModel"></param>
        /// <returns></returns>
        public StudentAddViewModel AssignGeneralInfoForStudents(StudentAddViewModel studentAddViewModel)
        {
            //StudentAddViewModel studentGeneralInfoAssign = new StudentAddViewModel();
            if(studentAddViewModel.studentMaster is null)
            {
                return studentAddViewModel;
            }
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    if (studentAddViewModel.studentIds?.Any()==true)
                    {
                        foreach (var studentId in studentAddViewModel.studentIds)
                        {
                            var student = this.context?.StudentMaster.FirstOrDefault(x => x.TenantId == studentAddViewModel.studentMaster!.TenantId && x.SchoolId == studentAddViewModel.studentMaster.SchoolId && x.StudentId == studentId);

                            if (student != null)
                            {
                                if (studentAddViewModel.studentMaster.DistrictId != null)
                                {
                                    student.DistrictId = studentAddViewModel.studentMaster.DistrictId;
                                }

                                if (studentAddViewModel.studentMaster.StateId != null)
                                {
                                    student.StateId = studentAddViewModel.studentMaster.StateId;
                                }

                                if (studentAddViewModel.studentMaster.Dob != null)
                                {
                                    student.Dob = studentAddViewModel.studentMaster.Dob;
                                }

                                if (studentAddViewModel.studentMaster.Gender != null)
                                {
                                    student.Gender = studentAddViewModel.studentMaster.Gender;
                                }

                                if (studentAddViewModel.studentMaster.Race != null)
                                {
                                    student.Race = studentAddViewModel.studentMaster.Race;
                                }

                                if (studentAddViewModel.studentMaster.Ethnicity != null)
                                {
                                    student.Ethnicity = studentAddViewModel.studentMaster.Ethnicity;
                                }

                                if (studentAddViewModel.studentMaster.MaritalStatus != null)
                                {
                                    student.MaritalStatus = studentAddViewModel.studentMaster.MaritalStatus;
                                }

                                if (studentAddViewModel.studentMaster.CountryOfBirth != null)
                                {
                                    student.CountryOfBirth = studentAddViewModel.studentMaster.CountryOfBirth;
                                }

                                if (studentAddViewModel.studentMaster.Nationality != null)
                                {
                                    student.Nationality = studentAddViewModel.studentMaster.Nationality;
                                }

                                if (studentAddViewModel.studentMaster.FirstLanguageId != null && studentAddViewModel.studentMaster.FirstLanguageId > 0)
                                {
                                    student.FirstLanguageId = studentAddViewModel.studentMaster.FirstLanguageId;
                                }

                                if (studentAddViewModel.studentMaster.SecondLanguageId != null && studentAddViewModel.studentMaster.SecondLanguageId > 0)
                                {
                                    student.SecondLanguageId = studentAddViewModel.studentMaster.SecondLanguageId;
                                }

                                if (studentAddViewModel.studentMaster.ThirdLanguageId != null && studentAddViewModel.studentMaster.ThirdLanguageId > 0)
                                {
                                    student.ThirdLanguageId = studentAddViewModel.studentMaster.ThirdLanguageId;
                                }

                                student.UpdatedBy = studentAddViewModel.CreatedOrUpdatedBy;
                                student.UpdatedOn = DateTime.UtcNow;
                                this.context?.SaveChanges();

                                if (studentAddViewModel.fieldsCategoryList != null && studentAddViewModel.fieldsCategoryList.ToList()?.Any()==true)
                                {
                                    var fieldsCategory = studentAddViewModel.fieldsCategoryList.FirstOrDefault(x => x.CategoryId == studentAddViewModel.SelectedCategoryId);

                                    if (fieldsCategory != null)
                                    {
                                        foreach (var customFields in fieldsCategory.CustomFields.ToList())
                                        {
                                            var customFieldValueData = this.context?.CustomFieldsValue.FirstOrDefault(x => x.TenantId == studentAddViewModel.studentMaster.TenantId && x.SchoolId == studentAddViewModel.studentMaster.SchoolId && x.CategoryId == customFields.CategoryId && x.FieldId == customFields.FieldId && x.Module == "Student" && x.TargetId == studentId);

                                            if (customFieldValueData != null)
                                            {
                                                this.context?.CustomFieldsValue.RemoveRange(customFieldValueData);
                                            }

                                            if (customFields.CustomFieldsValue != null && customFields.CustomFieldsValue.ToList()?.Any()==true)
                                            {
                                                customFields.CustomFieldsValue.FirstOrDefault()!.Module = "Student";
                                                customFields.CustomFieldsValue.FirstOrDefault()!.CategoryId = customFields.CategoryId;
                                                customFields.CustomFieldsValue.FirstOrDefault()!.FieldId = customFields.FieldId;
                                                customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldTitle = customFields.Title;
                                                customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldType = customFields.Type;
                                                customFields.CustomFieldsValue.FirstOrDefault()!.SchoolId = studentAddViewModel.studentMaster.SchoolId;
                                                customFields.CustomFieldsValue.FirstOrDefault()!.TenantId = studentAddViewModel.studentMaster.TenantId;
                                                customFields.CustomFieldsValue.FirstOrDefault()!.TargetId = studentId;
                                                customFields.CustomFieldsValue.FirstOrDefault()!.UpdatedBy = studentAddViewModel.CreatedOrUpdatedBy;
                                                customFields.CustomFieldsValue.FirstOrDefault()!.UpdateOn = DateTime.UtcNow;
                                                this.context?.CustomFieldsValue.AddRange(customFields.CustomFieldsValue);
                                                this.context?.SaveChanges();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        transaction?.Commit();
                        studentAddViewModel._message = "General Info Succesfully Assigned For Selected Students";
                        studentAddViewModel._failure = false;
                    }
                    else
                    {
                        studentAddViewModel._message = "Please Select Atleast One Student";
                        studentAddViewModel._failure = true;
                    }
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    studentAddViewModel._message = es.Message;
                    studentAddViewModel._failure = true;
                }
            }
            return studentAddViewModel;
        }

        public StudentMedicalProviderAddViewModel AssignMedicalInfoForStudents(StudentMedicalProviderAddViewModel studentMedicalProviderAddViewModel)
        {
            StudentMedicalProviderAddViewModel studentMedicalInfoUpdateModel = new StudentMedicalProviderAddViewModel();

            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    List<CustomFieldsValue> customFieldsValueList = new List<CustomFieldsValue>();
                    List<StudentMedicalProvider> studentMedicalProviderList = new List<StudentMedicalProvider>();

                    if (studentMedicalProviderAddViewModel.studentMedicalProvider != null)
                    {
                        if (studentMedicalProviderAddViewModel.studentIds?.Any()==true)
                        {
                            int ide = 1;
                            var studentMedicalProviderData = this.context?.StudentMedicalProvider.Where(x => x.TenantId == studentMedicalProviderAddViewModel.studentMedicalProvider.TenantId && x.SchoolId == studentMedicalProviderAddViewModel.studentMedicalProvider.SchoolId).OrderByDescending(x => x.Id).FirstOrDefault();

                            if (studentMedicalProviderData != null)
                            {
                                ide = studentMedicalProviderData.Id + 1;
                            }

                            foreach (var studentId in studentMedicalProviderAddViewModel.studentIds)
                            {
                                var studentMedicalProviderUpdate = this.context?.StudentMedicalProvider.FirstOrDefault(x => x.TenantId == studentMedicalProviderAddViewModel.studentMedicalProvider.TenantId && x.SchoolId == studentMedicalProviderAddViewModel.studentMedicalProvider.SchoolId && x.StudentId == studentId);

                                if (studentMedicalProviderUpdate != null)
                                {
                                    if (studentMedicalProviderAddViewModel.studentMedicalProvider.PrimaryCarePhysician != null)
                                    {
                                        studentMedicalProviderUpdate.PrimaryCarePhysician = studentMedicalProviderAddViewModel.studentMedicalProvider.PrimaryCarePhysician;
                                    }

                                    if (studentMedicalProviderAddViewModel.studentMedicalProvider.PrimaryCarePhysician != null)
                                    {
                                        studentMedicalProviderUpdate.PrimaryCarePhysicianPhone = studentMedicalProviderAddViewModel.studentMedicalProvider.PrimaryCarePhysician;
                                    }

                                    if (studentMedicalProviderAddViewModel.studentMedicalProvider.InsuranceCompany != null)
                                    {
                                        studentMedicalProviderUpdate.InsuranceCompany = studentMedicalProviderAddViewModel.studentMedicalProvider.InsuranceCompany;
                                    }

                                    if (studentMedicalProviderAddViewModel.studentMedicalProvider.InsuranceCompanyPhone != null)
                                    {
                                        studentMedicalProviderUpdate.InsuranceCompanyPhone = studentMedicalProviderAddViewModel.studentMedicalProvider.InsuranceCompanyPhone;
                                    }

                                    if (studentMedicalProviderAddViewModel.studentMedicalProvider.PreferredMedicalFacility != null)
                                    {
                                        studentMedicalProviderUpdate.PreferredMedicalFacility = studentMedicalProviderAddViewModel.studentMedicalProvider.PreferredMedicalFacility;
                                    }

                                    if (studentMedicalProviderAddViewModel.studentMedicalProvider.PreferredMedicalFacilityPhone != null)
                                    {
                                        studentMedicalProviderUpdate.PreferredMedicalFacilityPhone = studentMedicalProviderAddViewModel.studentMedicalProvider.PreferredMedicalFacilityPhone;
                                    }

                                    if (studentMedicalProviderAddViewModel.studentMedicalProvider.DentistName != null)
                                    {
                                        studentMedicalProviderUpdate.DentistName = studentMedicalProviderAddViewModel.studentMedicalProvider.DentistName;
                                    }

                                    if (studentMedicalProviderAddViewModel.studentMedicalProvider.DentistPhone != null)
                                    {
                                        studentMedicalProviderUpdate.DentistPhone = studentMedicalProviderAddViewModel.studentMedicalProvider.DentistPhone;
                                    }

                                    if (studentMedicalProviderAddViewModel.studentMedicalProvider.VisionName != null)
                                    {
                                        studentMedicalProviderUpdate.VisionName = studentMedicalProviderAddViewModel.studentMedicalProvider.VisionName;
                                    }

                                    if (studentMedicalProviderAddViewModel.studentMedicalProvider.VisionProviderPhone != null)
                                    {
                                        studentMedicalProviderUpdate.VisionProviderPhone = studentMedicalProviderAddViewModel.studentMedicalProvider.VisionProviderPhone;
                                    }

                                    studentMedicalProviderUpdate.UpdatedOn = DateTime.UtcNow;
                                    studentMedicalProviderUpdate.UpdatedBy = studentMedicalProviderAddViewModel.studentMedicalProvider.UpdatedBy;
                                    //studentMedicalProviderList.Add(studentMedicalProviderUpdate);
                                    this.context?.SaveChanges();

                                    if (studentMedicalProviderAddViewModel.fieldsCategoryList != null && studentMedicalProviderAddViewModel.fieldsCategoryList.ToList()?.Any()==true)
                                    {
                                        var fieldsCategory = studentMedicalProviderAddViewModel.fieldsCategoryList.FirstOrDefault(x => x.CategoryId == studentMedicalProviderAddViewModel.SelectedCategoryId);

                                        if (fieldsCategory != null)
                                        {
                                            foreach (var customFields in fieldsCategory.CustomFields.ToList())
                                            {
                                                var customFieldValueData = this.context?.CustomFieldsValue.FirstOrDefault(x => x.TenantId == studentMedicalProviderAddViewModel.studentMedicalProvider.TenantId && x.SchoolId == studentMedicalProviderAddViewModel.studentMedicalProvider.SchoolId && x.CategoryId == customFields.CategoryId && x.FieldId == customFields.FieldId && x.Module == "Student" && x.TargetId == studentId);
                                                if (customFieldValueData != null)
                                                {
                                                    this.context?.CustomFieldsValue.RemoveRange(customFieldValueData);
                                                }
                                                if (customFields.CustomFieldsValue.ToList()?.Any()==true)
                                                {
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.Module = "Student";
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.CategoryId = customFields.CategoryId;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.FieldId = customFields.FieldId;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldTitle = customFields.Title;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldType = customFields.Type;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.SchoolId = studentMedicalProviderAddViewModel.studentMedicalProvider.SchoolId;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.TargetId = studentId;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.TenantId = studentMedicalProviderAddViewModel.studentMedicalProvider.TenantId;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.UpdatedBy = studentMedicalProviderAddViewModel.studentMedicalProvider.UpdatedBy;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.UpdateOn = DateTime.UtcNow;

                                                    this.context?.CustomFieldsValue.AddRange(customFields.CustomFieldsValue);
                                                    this.context?.SaveChanges();

                                                    //customFieldsValueList.AddRange(customFields.CustomFieldsValue);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var medicalProvider = new StudentMedicalProvider()
                                    {
                                        SchoolId = studentMedicalProviderAddViewModel.studentMedicalProvider.SchoolId,
                                        TenantId = studentMedicalProviderAddViewModel.studentMedicalProvider.TenantId,
                                        StudentId = studentId,
                                        CreatedBy = studentMedicalProviderAddViewModel.studentMedicalProvider.CreatedBy,
                                        CreatedOn = DateTime.UtcNow,
                                        PrimaryCarePhysician = studentMedicalProviderAddViewModel.studentMedicalProvider.PrimaryCarePhysician,
                                        PrimaryCarePhysicianPhone = studentMedicalProviderAddViewModel.studentMedicalProvider.PrimaryCarePhysicianPhone,
                                        InsuranceCompany = studentMedicalProviderAddViewModel.studentMedicalProvider.InsuranceCompany,
                                        InsuranceCompanyPhone = studentMedicalProviderAddViewModel.studentMedicalProvider.InsuranceCompanyPhone,
                                        PreferredMedicalFacility = studentMedicalProviderAddViewModel.studentMedicalProvider.PreferredMedicalFacility,
                                        PreferredMedicalFacilityPhone = studentMedicalProviderAddViewModel.studentMedicalProvider.PreferredMedicalFacilityPhone,
                                        DentistName = studentMedicalProviderAddViewModel.studentMedicalProvider.DentistName,
                                        DentistPhone = studentMedicalProviderAddViewModel.studentMedicalProvider.DentistPhone,
                                        VisionName = studentMedicalProviderAddViewModel.studentMedicalProvider.VisionName,
                                        VisionProviderPhone = studentMedicalProviderAddViewModel.studentMedicalProvider.VisionProviderPhone,
                                        Id = ide
                                    };
                                    //studentMedicalProviderList.Add(medicalProvider);
                                    this.context?.StudentMedicalProvider.Add(medicalProvider);
                                    this.context?.SaveChanges();
                                    ide++;

                                    if (studentMedicalProviderAddViewModel.fieldsCategoryList != null && studentMedicalProviderAddViewModel.fieldsCategoryList.ToList()?.Any()==true)
                                    {
                                        CustomFieldsValue customFieldsValue = new CustomFieldsValue();

                                        var fieldsCategory = studentMedicalProviderAddViewModel.fieldsCategoryList.FirstOrDefault(x => x.CategoryId == studentMedicalProviderAddViewModel.SelectedCategoryId);
                                        if (fieldsCategory != null)
                                        {
                                            foreach (var customFields in fieldsCategory.CustomFields.ToList())
                                            {
                                                if ( customFields.CustomFieldsValue.ToList()?.Any()==true)
                                                {
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.Module = "Student";
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.CategoryId = customFields.CategoryId;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.FieldId = customFields.FieldId;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldTitle = customFields.Title;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldType = customFields.Type;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.SchoolId = studentMedicalProviderAddViewModel.studentMedicalProvider.SchoolId;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.TenantId = studentMedicalProviderAddViewModel.studentMedicalProvider.TenantId;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.TargetId = studentId;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.CreatedBy = studentMedicalProviderAddViewModel.studentMedicalProvider.UpdatedBy;
                                                    customFields.CustomFieldsValue.FirstOrDefault()!.CreatedOn = DateTime.UtcNow;
                                                    this.context?.CustomFieldsValue.AddRange(customFields.CustomFieldsValue);
                                                    this.context?.SaveChanges();
                                                    //customFieldsValueList.AddRange(customFields.CustomFieldsValue);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            //this.context.CustomFieldsValue.AddRange(customFieldsValueList);
                            //this.context?.StudentMedicalProvider.AddRange(studentMedicalProviderList);
                            //this.context?.SaveChanges();
                            transaction?.Commit();
                            studentMedicalProviderAddViewModel._failure = false;
                            studentMedicalProviderAddViewModel._message = "Student Medical Info Assigned Successfully For Selected Students";
                        }
                        else
                        {
                            studentMedicalProviderAddViewModel._failure = false;
                            studentMedicalProviderAddViewModel._message = "Please Select Atleast One Student";
                        }
                    }
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    studentMedicalProviderAddViewModel._failure = true;
                    studentMedicalProviderAddViewModel._message = es.Message;
                }
            }
            return studentMedicalProviderAddViewModel;
        }

        public StudentCommentAddViewModel AssignCommentForStudents(StudentCommentAddViewModel studentCommentAddViewModel)
        {
            try
            {
                if (studentCommentAddViewModel.studentIds?.Any()==true)
                {
                    int? MasterCommentId = Utility.GetMaxPK(this.context, new Func<StudentComments, int>(x => x.CommentId));

                    foreach (var studentId in studentCommentAddViewModel.studentIds)
                    {
                        var studentComment = new StudentComments()
                        {
                            TenantId = studentCommentAddViewModel.TenantId,
                            SchoolId = studentCommentAddViewModel.SchoolId,
                            StudentId = studentId,
                            CommentId = (int)MasterCommentId!,
                            Comment = studentCommentAddViewModel.studentComments?.Comment?? "",
                            CreatedBy = studentCommentAddViewModel.CreatedBy,
                            CreatedOn = DateTime.UtcNow
                        };
                        this.context?.StudentComments.Add(studentComment);
                        MasterCommentId++;
                    }
                    this.context?.SaveChanges();
                    studentCommentAddViewModel._failure = false;
                    studentCommentAddViewModel._message = "Student Comment added successfully For Selected Students";
                }
                else
                {
                    studentCommentAddViewModel._failure = true;
                    studentCommentAddViewModel._message = "Please Select Atleast One Student";
                }
            }
            catch (Exception es)
            {
                studentCommentAddViewModel._failure = true;
                studentCommentAddViewModel._message = es.Message;
            }
            return studentCommentAddViewModel;
        }

        public StudentDocumentAddViewModel AssignDocumentForStudents(StudentDocumentAddViewModel studentDocumentAddViewModel)
        {
            try
            {
                int? MasterDocumentId = 0;
                if (studentDocumentAddViewModel.studentIds?.Any()==true)
                {
                    MasterDocumentId = Utility.GetMaxPK(this.context, new Func<StudentDocuments, int>(x => x.DocumentId));

                    foreach (var studentId in studentDocumentAddViewModel.studentIds)
                    {
                        if (studentDocumentAddViewModel.studentDocuments?.Any()==true)
                        {
                            foreach (var studentDocument in studentDocumentAddViewModel.studentDocuments)
                            {
                                var studentComment = new StudentDocuments()
                                {
                                    TenantId = studentDocumentAddViewModel.TenantId,
                                    SchoolId = studentDocumentAddViewModel.SchoolId,
                                    StudentId = studentId,
                                    DocumentId = (int)MasterDocumentId!,
                                    FileUploaded = studentDocument.FileUploaded,
                                    UploadedOn = DateTime.UtcNow,
                                    UploadedBy = studentDocumentAddViewModel.CreatedBy,
                                    Filename = studentDocument.Filename,
                                    Filetype = studentDocument.Filetype,
                                    CreatedOn = DateTime.UtcNow,
                                    CreatedBy = studentDocumentAddViewModel.CreatedBy

                                };
                                this.context?.StudentDocuments.Add(studentComment);
                                MasterDocumentId++;
                            }
                        }
                    }
                    this.context?.SaveChanges();
                    studentDocumentAddViewModel._failure = false;
                    studentDocumentAddViewModel._message = "Student Document added successfully for Selected Students";
                }
                else
                {
                    studentDocumentAddViewModel._failure = true;
                    studentDocumentAddViewModel._message = "Please Select Atleast One Student";
                }
            }
            catch (Exception es)
            {
                studentDocumentAddViewModel._failure = true;
                studentDocumentAddViewModel._message = es.Message;
            }
            return studentDocumentAddViewModel;
        }

        public StudentEnrollmentAssignModel AssignEnrollmentInfoForStudents(StudentEnrollmentAssignModel studentEnrollmentAssignModel)
        {
            try
            {
                if (studentEnrollmentAssignModel.studentIds?.Any() == true)
                {
                    foreach (var studentId in studentEnrollmentAssignModel.studentIds)
                    {
                        var studentMasterData = this.context?.StudentMaster.FirstOrDefault(x => x.TenantId == studentEnrollmentAssignModel.TenantId && x.SchoolId == studentEnrollmentAssignModel.SchoolId && x.StudentId == studentId);

                        if (studentMasterData != null)
                        {
                            if (studentEnrollmentAssignModel.SectionId != null)
                            {
                                studentMasterData.SectionId = studentEnrollmentAssignModel.SectionId;
                            }
                            if (studentEnrollmentAssignModel.EstimatedGradDate != null)
                            {
                                studentMasterData.EstimatedGradDate = studentEnrollmentAssignModel.EstimatedGradDate;
                            }
                            if (studentEnrollmentAssignModel.Eligibility504 != null)
                            {
                                studentMasterData.Eligibility504 = studentEnrollmentAssignModel.Eligibility504;
                            }
                            if (studentEnrollmentAssignModel.EconomicDisadvantage != null)
                            {
                                studentMasterData.EconomicDisadvantage = studentEnrollmentAssignModel.EconomicDisadvantage;
                            }
                            if (studentEnrollmentAssignModel.FreeLunchEligibility != null)
                            {
                                studentMasterData.FreeLunchEligibility = studentEnrollmentAssignModel.FreeLunchEligibility;
                            }
                            if (studentEnrollmentAssignModel.SpecialEducationIndicator != null)
                            {
                                studentMasterData.SpecialEducationIndicator = studentEnrollmentAssignModel.SpecialEducationIndicator;
                            }
                            if (studentEnrollmentAssignModel.LepIndicator != null)
                            {
                                studentMasterData.LepIndicator = studentEnrollmentAssignModel.LepIndicator;
                            }

                            studentMasterData.UpdatedBy = studentEnrollmentAssignModel.studentEnrollments?.UpdatedBy;
                            studentMasterData.UpdatedOn = DateTime.UtcNow;
                        }

                        var studentEnrollmentData = this.context?.StudentEnrollment.FirstOrDefault(x => x.TenantId == studentEnrollmentAssignModel.TenantId && x.SchoolId == studentEnrollmentAssignModel.SchoolId && x.StudentId == studentId && x.IsActive == true);

                        if (studentEnrollmentData != null)
                        {
                            if (studentEnrollmentAssignModel.studentEnrollments?.RollingOption != null)
                            {
                                studentEnrollmentData.RollingOption = studentEnrollmentAssignModel.studentEnrollments.RollingOption;
                            }
                            if (studentEnrollmentAssignModel.studentEnrollments?.EnrollOtherSchoolId != null)
                            {
                                studentEnrollmentData.EnrollOtherSchoolId = studentEnrollmentAssignModel.studentEnrollments.EnrollOtherSchoolId;
                            }
                            if (studentEnrollmentAssignModel.studentEnrollments?.CalenderId != null)
                            {
                                studentEnrollmentData.CalenderId = studentEnrollmentAssignModel.studentEnrollments.CalenderId;
                            }
                            if (studentEnrollmentAssignModel.studentEnrollments?.GradeId != null && studentEnrollmentAssignModel.studentEnrollments?.GradeLevelTitle != null)
                            {
                                studentEnrollmentData.GradeId = studentEnrollmentAssignModel.studentEnrollments.GradeId;
                                studentEnrollmentData.GradeLevelTitle = studentEnrollmentAssignModel.studentEnrollments.GradeLevelTitle;

                                //for update related table
                                var academicYear = Utility.GetCurrentAcademicYear(this.context!, studentEnrollmentAssignModel.TenantId, studentEnrollmentAssignModel.SchoolId);

                                var studentCourseSectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(x => x.TenantId == studentEnrollmentAssignModel.TenantId && x.SchoolId == studentEnrollmentAssignModel.SchoolId && x.StudentId == studentId && x.AcademicYear == academicYear).ToList();

                                if (studentCourseSectionScheduleData?.Any() != null)
                                {
                                    studentCourseSectionScheduleData.ForEach(x => x.GradeId = studentEnrollmentAssignModel.studentEnrollments.GradeId);
                                }

                                var studentInputFinalGradeData = this.context?.StudentFinalGrade.Where(x => x.TenantId == studentEnrollmentAssignModel.TenantId && x.SchoolId == studentEnrollmentAssignModel.SchoolId && x.StudentId == studentId && x.AcademicYear == academicYear).ToList();

                                if (studentInputFinalGradeData?.Any() != null)
                                {
                                    studentInputFinalGradeData.ForEach(x => x.GradeId = studentEnrollmentAssignModel.studentEnrollments.GradeId);
                                }
                            }

                            if (studentEnrollmentAssignModel.studentEnrollments?.EnrollmentDate != null)
                            {
                                studentEnrollmentData.EnrollmentDate = studentEnrollmentAssignModel.studentEnrollments.EnrollmentDate;
                            }

                            studentEnrollmentData.UpdatedBy = studentEnrollmentAssignModel.studentEnrollments?.UpdatedBy;
                            studentEnrollmentData.UpdatedOn = DateTime.UtcNow;
                        }

                        this.context?.SaveChanges();

                        studentEnrollmentAssignModel._message = "Student Enrollment Info is Updated Successfully";
                    }
                }
                else
                {
                    studentEnrollmentAssignModel._failure = true;
                    studentEnrollmentAssignModel._message = "Please Select Atleast One Student";
                }
            }
            catch (Exception es)
            {
                studentEnrollmentAssignModel._failure = true;
                studentEnrollmentAssignModel._message = es.Message;
            }
            return studentEnrollmentAssignModel;
        }

        /// <summary>
        /// GetAllStudentListByDateRange
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public StudentListModel GetAllStudentListByDateRange(PageResult pageResult)
        {
            StudentListModel studentListModel = new StudentListModel();
            IQueryable<StudentListView>? transactionIQ = null;
            IQueryable<StudentListView>? studentDataList = null;
            int? totalCount = 0;

            var membershipData = this.context?.UserMaster.Include(x => x.Membership).FirstOrDefault(x => x.TenantId == pageResult.TenantId && x.EmailAddress == pageResult.EmailAddress);

            var activeSchools = this.context?.SchoolDetail.Where(x => x.Status == true).Select(x => x.SchoolId).ToList();

            if (membershipData != null)
            {
                if (String.Compare(membershipData.Membership.ProfileType, "super administrator", true) == 0)
                {
                    var studentGuids = this.context?.StudentEnrollment.Where(x => x.TenantId == pageResult.TenantId && (pageResult.SearchAllSchool == false || pageResult.SearchAllSchool == null ? x.SchoolId == pageResult.SchoolId : activeSchools!.Contains(x.SchoolId)) && (x.ExitDate == null ? ((x.EnrollmentDate >= pageResult.MarkingPeriodStartDate && x.EnrollmentDate <= pageResult.MarkingPeriodEndDate) || x.EnrollmentDate <= pageResult.MarkingPeriodStartDate) : ((pageResult.MarkingPeriodStartDate >= x.EnrollmentDate && pageResult.MarkingPeriodStartDate <= x.ExitDate) || (pageResult.MarkingPeriodEndDate >= x.EnrollmentDate && pageResult.MarkingPeriodEndDate <= x.ExitDate)))).Select(x => x.StudentGuid).Distinct();

                    if (studentGuids?.Any() == true)
                    {
                        studentDataList = this.context?.StudentListView.Where(x => x.TenantId == pageResult.TenantId && studentGuids.Contains(x.StudentGuid) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.IsActive != false : true));
                    }
                }
                else
                {
                    var schoolAttachedId = this.context?.StaffSchoolInfo.Where(x => x.TenantId == pageResult.TenantId && x.StaffId == membershipData.UserId && x.EndDate == null && activeSchools!.Contains(x.SchoolAttachedId)).ToList().Select(s => s.SchoolAttachedId);

                    var studentGuids = this.context?.StudentEnrollment.Where(x => x.TenantId == pageResult.TenantId && (pageResult.SearchAllSchool == false || pageResult.SearchAllSchool == null ? x.SchoolId == pageResult.SchoolId : schoolAttachedId!.Contains(x.SchoolId)) && (x.ExitDate == null ? ((x.EnrollmentDate >= pageResult.MarkingPeriodStartDate && x.EnrollmentDate <= pageResult.MarkingPeriodEndDate) || x.EnrollmentDate <= pageResult.MarkingPeriodStartDate) : ((pageResult.MarkingPeriodStartDate >= x.EnrollmentDate && pageResult.MarkingPeriodStartDate <= x.ExitDate) || (pageResult.MarkingPeriodEndDate >= x.EnrollmentDate && pageResult.MarkingPeriodEndDate <= x.ExitDate)))).Select(x => x.StudentGuid).Distinct();

                    if (studentGuids?.Any() == true)
                    {
                        studentDataList = this.context?.StudentListView.Where(x => x.TenantId == pageResult.TenantId && studentGuids.Contains(x.StudentGuid) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.IsActive != false : true));

                    }
                }
                if (pageResult.SearchAllSchool != true)
                {
                    studentDataList = studentDataList?.Where(x => x.SchoolId == pageResult.SchoolId);
                }
            }

            try
            {
                if (studentDataList?.Any() == true)
                {
                    if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                    {
                        transactionIQ = studentDataList;
                    }
                    else
                    {
                        string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                        var ColumnvalueForName = Regex.Replace(Columnvalue, @"\s+", "");
                        if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                        {
                            transactionIQ = studentDataList.Where(x => x.FirstGivenName != null && x.FirstGivenName.ToLower().Contains(ColumnvalueForName.ToLower()) || x.MiddleName != null && x.MiddleName.ToLower().Contains(ColumnvalueForName.ToLower()) || x.LastFamilyName != null && x.LastFamilyName.ToLower().Contains(ColumnvalueForName.ToLower()) || ((x.FirstGivenName ?? "").ToLower() + (x.MiddleName ?? "").ToLower() + (x.LastFamilyName ?? "").ToLower()).Contains(ColumnvalueForName.ToLower()) || ((x.FirstGivenName ?? "").ToLower() + (x.MiddleName ?? "").ToLower()).Contains(ColumnvalueForName.ToLower()) || ((x.FirstGivenName ?? "").ToLower() + (x.LastFamilyName ?? "").ToLower()).Contains(ColumnvalueForName.ToLower()) || ((x.MiddleName ?? "").ToLower() + (x.LastFamilyName ?? "").ToLower()).Contains(ColumnvalueForName.ToLower()) || x.StudentInternalId != null && x.StudentInternalId.ToLower().Contains(Columnvalue.ToLower()) || x.AlternateId != null && x.AlternateId.ToLower().Contains(Columnvalue.ToLower()) || x.HomePhone != null && x.HomePhone.ToLower().Contains(Columnvalue.ToLower()) || x.MobilePhone != null && x.MobilePhone.ToLower().Contains(Columnvalue.ToLower()) || x.PersonalEmail != null && x.PersonalEmail.ToLower().Contains(Columnvalue.ToLower()) || x.SchoolEmail != null && x.SchoolEmail.ToLower().Contains(Columnvalue.ToLower()) || x.GradeLevelTitle != null && x.GradeLevelTitle.ToLower().Contains(Columnvalue.ToLower()) || x.SectionName != null && x.SectionName.ToLower().Contains(Columnvalue.ToLower()));
                        }
                        else
                        {
                            //normal advance search
                            transactionIQ = Utility.FilteredData(pageResult.FilterParams!, studentDataList).AsQueryable();

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
                    }

                    if (pageResult.DobStartDate != null && pageResult.DobEndDate != null)
                    {
                        var filterInDateRange = transactionIQ?.Where(x => x.Dob >= pageResult.DobStartDate && x.Dob <= pageResult.DobEndDate);
                        if (filterInDateRange?.Any() == true)
                        {
                            transactionIQ = filterInDateRange;
                        }
                        else
                        {
                            transactionIQ = null;
                        }
                    }

                    if (pageResult.FullName != null)
                    {
                        var studentName = pageResult.FullName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                        if (studentName.Length > 1)
                        {
                            var firstName = studentName.First();
                            var lastName = studentName.Last();
                            pageResult.FullName = null;

                            if (pageResult.FullName == null)
                            {
                                var nameSearch = transactionIQ?.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && (x.FirstGivenName ?? "").StartsWith(firstName.ToString()) && (x.LastFamilyName ?? "").StartsWith(lastName.ToString()));

                                transactionIQ = nameSearch;
                            }
                        }
                        else
                        {
                            var nameSearch = transactionIQ?.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && ((x.FirstGivenName ?? "").StartsWith(pageResult.FullName) || (x.LastFamilyName ?? "").StartsWith(pageResult.FullName))).AsQueryable();

                            transactionIQ = nameSearch;
                        }
                    }
                    if (transactionIQ != null)
                    {
                        if (pageResult.SortingModel != null)
                        {
                            transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn ?? "", (pageResult.SortingModel.SortDirection ?? "").ToLower());
                        }
                        else
                        {
                            transactionIQ = transactionIQ.OrderBy(s => s.LastFamilyName).ThenBy(c => c.FirstGivenName);
                        }
                    }

                    totalCount = transactionIQ != null ? transactionIQ?.Count() : 0;

                    if (totalCount > 0)
                    {
                        if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                        {
                            transactionIQ = transactionIQ!.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                        }

                        studentListModel.studentListViews = transactionIQ!.ToList();

                        studentListModel.studentListViews.ForEach(c =>
                        {
                            c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, c.CreatedBy);
                            c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, c.UpdatedBy);
                        });

                        studentListModel.TotalCount = totalCount;
                        studentListModel._message = "success";
                        studentListModel._failure = false;
                    }
                    else
                    {
                        studentListModel._message = NORECORDFOUND;
                        studentListModel._failure = true;
                    }

                    studentListModel.TenantId = pageResult.TenantId;
                    studentListModel.SchoolId = pageResult.SchoolId;
                    studentListModel.PageNumber = pageResult.PageNumber;
                    studentListModel._pageSize = pageResult.PageSize;
                    studentListModel._tenantName = pageResult._tenantName;
                    studentListModel._token = pageResult._token;
                }
            }
            catch (Exception es)
            {
                studentListModel._message = es.Message;
                studentListModel._failure = true;
            }
            return studentListModel;
        }

        public StudentDeleteViewModel DeleteStudent(StudentDeleteViewModel studentDeleteViewModel)
        {
            StudentDeleteViewModel studentDelete = new();
            studentDelete.TenantId = studentDeleteViewModel.TenantId;
            studentDelete.SchoolId = studentDeleteViewModel.SchoolId;
            studentDelete._tenantName = studentDeleteViewModel._tenantName;
            studentDelete._token = studentDeleteViewModel._token;
            studentDelete._userName = studentDeleteViewModel._userName;

            bool allSccuss = true;
            List<string> studentName = new();
            string? staffName = string.Empty;
            List<StudentMaster> studentMasters = new List<StudentMaster>();
            List<StudentComments> studentComments = new List<StudentComments>();
            List<StudentMedicalAlert> studentMedicalAlerts = new List<StudentMedicalAlert>();
            List<StudentMedicalNurseVisit> studentMedicalNurseVisits = new List<StudentMedicalNurseVisit>();
            List<StudentMedicalImmunization> studentMedicalImmunizations = new List<StudentMedicalImmunization>();
            List<StudentMedicalNote> studentMedicalNotes = new List<StudentMedicalNote>();
            List<StudentMedicalProvider> studentMedicalProviders = new List<StudentMedicalProvider>();
            List<StudentDocuments> studentDocuments = new List<StudentDocuments>();
            List<CustomFieldsValue> customFieldsValues = new List<CustomFieldsValue>();
            List<UserMaster> userMasters = new List<UserMaster>();
            List<ParentAssociationship> parentAssociationships = new List<ParentAssociationship>();
            List<ParentInfo> parentInfos = new List<ParentInfo>();
            List<ParentAddress> parentAddresses = new List<ParentAddress>();
            List<StudentCoursesectionSchedule> studentCoursesectionSchedules = new List<StudentCoursesectionSchedule>();
            try
            {
                if (studentDeleteViewModel.studentListViews?.Any() == true)
                {
                    var studentIds = studentDeleteViewModel.studentListViews.Select(s => s.StudentId).ToList();

                    var studentMasterData = this.context?.StudentMaster.Include(s => s.StudentEnrollment).Include(s => s.StudentComments).Include(s => s.StudentDocuments).Include(s => s.StudentMedicalAlert).Include(s => s.StudentMedicalImmunization).Include(s => s.StudentMedicalNote).Include(s => s.StudentMedicalNurseVisit).Include(s => s.StudentMedicalProvider).Include(s => s.StudentCoursesectionSchedule).Where(x => x.TenantId == studentDeleteViewModel.TenantId && x.SchoolId == studentDeleteViewModel.SchoolId && studentIds.Contains(x.StudentId)).ToList();

                    var customFieldsValueData = this.context?.CustomFieldsValue.Where(x => x.TenantId == studentDeleteViewModel.TenantId && x.SchoolId == studentDeleteViewModel.SchoolId && x.Module == "Student" && studentIds.Contains(x.TargetId)).ToList();

                    var userMasterData = this.context?.UserMaster.Where(x => x.TenantId == studentDeleteViewModel.TenantId && x.SchoolId == studentDeleteViewModel.SchoolId).ToList();

                    var parentAssociationshipAllData = this.context?.ParentAssociationship.Where(x => x.TenantId == studentDeleteViewModel.TenantId && x.SchoolId == studentDeleteViewModel.SchoolId).ToList();
                    var parentAssociationshipData = parentAssociationshipAllData?.Where(x => x.TenantId == studentDeleteViewModel.TenantId && x.SchoolId == studentDeleteViewModel.SchoolId && studentIds.Contains(x.StudentId)).ToList();
                    var parentIds = parentAssociationshipData?.Select(s => s.ParentId).ToList();
                    var parentInfoData = this.context?.ParentInfo.Include(s => s.ParentAddress).Where(x => x.TenantId == studentDeleteViewModel.TenantId && /*x.SchoolId == studentDeleteViewModel.SchoolId &&*/ parentIds != null && parentIds.Contains(x.ParentId)).ToList();

                    foreach (var student in studentDeleteViewModel.studentListViews)
                    {
                        var studentData = studentMasterData?.FirstOrDefault(s => s.StudentId == student.StudentId);
                        if (studentData != null)
                        {
                            bool hasAssociation = false;

                            if (studentData.StudentCoursesectionSchedule.Count == 0)
                            {
                                hasAssociation = false;
                            }
                            else
                            {
                                var studentDroppedCS = studentData.StudentCoursesectionSchedule.Where(x => x.IsDropped == true).ToList();
                                if (studentDroppedCS?.Any() == true)
                                {
                                    var studentDroppedCSIds = studentDroppedCS.Select(s => s.CourseSectionId).ToList();
                                    var StudentAttendanceData = this.context?.StudentAttendance.Where(x => x.TenantId == studentDeleteViewModel.TenantId && x.SchoolId == studentDeleteViewModel.SchoolId && x.StudentId == student.StudentId && studentDroppedCSIds.Contains(x.CourseSectionId)).FirstOrDefault();
                                    var FinalGradeData = this.context?.StudentFinalGrade.Where(x => x.TenantId == studentDeleteViewModel.TenantId && x.SchoolId == studentDeleteViewModel.SchoolId && x.StudentId == student.StudentId && studentDroppedCSIds.Contains((int)x.CourseSectionId)).FirstOrDefault();
                                    var GradebookGradeData = this.context?.GradebookGrades.Where(x => x.TenantId == studentDeleteViewModel.TenantId && x.SchoolId == studentDeleteViewModel.SchoolId && x.StudentId == student.StudentId && studentDroppedCSIds.Contains((int)x.CourseSectionId)).FirstOrDefault();
                                    if (StudentAttendanceData != null || FinalGradeData != null || GradebookGradeData != null)
                                    {
                                        hasAssociation = true;
                                        allSccuss = false;
                                        student.MailingAddressLineTwo = "Student has course section transactional association.";
                                        studentDelete.studentListViews.Add(student);
                                    }
                                    else
                                    {
                                        hasAssociation = false;
                                        studentCoursesectionSchedules.AddRange(studentDroppedCS);
                                    }

                                }
                                else
                                {
                                    hasAssociation = true;
                                    allSccuss = false;
                                    student.MailingAddressLineTwo = "Student has active course section.";
                                    studentDelete.studentListViews.Add(student);
                                }
                            }

                            if (!hasAssociation)
                            {
                                studentMasters.Add(studentData);
                                studentComments.AddRange(studentData.StudentComments);
                                studentDocuments.AddRange(studentData.StudentDocuments);
                                studentMedicalNurseVisits.AddRange(studentData.StudentMedicalNurseVisit);
                                studentMedicalAlerts.AddRange(studentData.StudentMedicalAlert);
                                studentMedicalProviders.AddRange(studentData.StudentMedicalProvider);
                                studentMedicalNotes.AddRange(studentData.StudentMedicalNote);
                                studentMedicalImmunizations.AddRange(studentData.StudentMedicalImmunization);

                                studentName.Add(studentData.FirstGivenName + " " + studentData.MiddleName + " " + studentData.LastFamilyName);

                                var UserMaster = userMasterData?.FirstOrDefault(x => x.TenantId == studentDeleteViewModel.TenantId && x.SchoolId == studentDeleteViewModel.SchoolId && x.EmailAddress == studentData.StudentPortalId);
                                if (UserMaster != null)
                                {
                                    userMasters.Add(UserMaster);
                                }

                                var customFieldsValue = customFieldsValueData?.Where(s => s.TargetId == student.StudentId).ToList();
                                if (customFieldsValue?.Any() == true)
                                {
                                    customFieldsValues.AddRange(customFieldsValue);
                                }

                                var parentAssociationship = parentAssociationshipData?.FirstOrDefault(s => s.StudentId == student.StudentId);
                                if (parentAssociationship != null)
                                {
                                    var parentOtherAssociationship = parentAssociationshipAllData?.FirstOrDefault(s => s.ParentId == parentAssociationship.ParentId && s.StudentId != student.StudentId);
                                    if (parentOtherAssociationship != null)
                                    {
                                        parentAssociationships.Add(parentAssociationship);
                                    }
                                    else
                                    {
                                        parentAssociationships.Add(parentAssociationship);
                                        var parentInfo = parentInfoData?.FirstOrDefault(s => s.ParentId == parentAssociationship.ParentId);
                                        if (parentInfo != null)
                                        {
                                            parentAddresses.AddRange(parentInfo.ParentAddress);
                                            parentInfos.Add(parentInfo);

                                            var userMaster = userMasterData?.FirstOrDefault(x => x.TenantId == studentDeleteViewModel.TenantId && x.SchoolId == studentDeleteViewModel.SchoolId && x.EmailAddress == parentInfo.LoginEmail);
                                            if (userMaster != null)
                                            {
                                                userMasters.Add(userMaster);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    this.context?.UserMaster.RemoveRange(userMasters);
                    this.context?.CustomFieldsValue.RemoveRange(customFieldsValues);
                    this.context?.ParentAssociationship.RemoveRange(parentAssociationships);
                    this.context?.ParentAddress.RemoveRange(parentAddresses);
                    this.context?.ParentInfo.RemoveRange(parentInfos);
                    this.context?.StudentComments.RemoveRange(studentComments);
                    this.context?.StudentMedicalAlert.RemoveRange(studentMedicalAlerts);
                    this.context?.StudentMedicalNurseVisit.RemoveRange(studentMedicalNurseVisits);
                    this.context?.StudentMedicalImmunization.RemoveRange(studentMedicalImmunizations);
                    this.context?.StudentMedicalNote.RemoveRange(studentMedicalNotes);
                    this.context?.StudentMedicalProvider.RemoveRange(studentMedicalProviders);
                    this.context?.StudentDocuments.RemoveRange(studentDocuments);
                    this.context?.StudentCoursesectionSchedule.RemoveRange(studentCoursesectionSchedules);
                    this.context?.StudentMaster.RemoveRange(studentMasters);

                    this.context?.SaveChanges();

                    if (allSccuss)
                    {
                        studentDelete._message = "Students deleted successfully";
                    }
                    else
                    {
                        studentDelete._message = "Students can not be deleted due to transactional associations";
                    }
                }
                else
                {
                    studentDelete._message = "Please select student";
                    studentDelete._failure = false;
                }
            }
            catch (Exception es)
            {
                studentDelete._message = es.Message;
                studentDelete._failure = false;
            }
            return studentDelete;
        }
    }
}




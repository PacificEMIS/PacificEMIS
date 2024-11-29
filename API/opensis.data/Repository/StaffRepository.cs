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
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.Staff;
using opensis.data.ViewModels.StaffSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace opensis.data.Repository
{
    public class StaffRepository : IStaffRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";

        public StaffRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Add Staff
        /// </summary>
        /// <param name="staffAddViewModel"></param>
        /// <returns></returns>
        public StaffAddViewModel AddStaff(StaffAddViewModel staffAddViewModel)
        {
            if (staffAddViewModel.staffMaster is null)
            {
                return staffAddViewModel;
            }
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    //int? staffId = Utility.GetMaxPK(this.context, new Func<StaffMaster, int>(x => x.StaffId));
                    int? staffId = 1;
                    var dataExits = this.context?.StaffMaster.Where(x => x.TenantId == staffAddViewModel.staffMaster.TenantId).Count();

                    if (dataExits > 0)
                    {
                        var staffData = this.context?.StaffMaster.Where(x => x.TenantId == staffAddViewModel.staffMaster.TenantId).Max(x => x.StaffId);
                        if (staffData != null)
                        {
                            staffId = staffData + 1;
                        }
                    }
                    staffAddViewModel.staffMaster.StaffId = (int)staffId;
                    Guid GuidId = Guid.NewGuid();
                    var GuidIdExist = this.context?.StaffMaster.FirstOrDefault(x => x.StaffGuid == GuidId);
                    if (GuidIdExist != null)
                    {
                        staffAddViewModel._failure = true;
                        staffAddViewModel._message = "Guid is already exist, Please try again.";
                        return staffAddViewModel;
                    }
                    staffAddViewModel.staffMaster.StaffGuid = GuidId;
                    staffAddViewModel.staffMaster.CreatedOn = DateTime.UtcNow;
                    staffAddViewModel.staffMaster.IsActive = true;

                    if (!string.IsNullOrEmpty(staffAddViewModel.staffMaster.StaffInternalId))
                    {
                        bool checkInternalID = CheckInternalID(staffAddViewModel.staffMaster.TenantId, staffAddViewModel.staffMaster.StaffInternalId);
                        if (checkInternalID == false)
                        {
                            staffAddViewModel.staffMaster = null;
                            //staffAddViewModel.fieldsCategoryList = null;
                            staffAddViewModel.fieldsCategoryList = new();
                            staffAddViewModel._failure = true;
                            staffAddViewModel._message = "Staff id already exist";
                            return staffAddViewModel;
                        }
                    }
                    else
                    {
                        staffAddViewModel.staffMaster.StaffInternalId = staffAddViewModel.staffMaster.StaffId.ToString();
                    }
                    //bool checkInternalID = CheckInternalID(staffAddViewModel.staffMaster.TenantId, staffAddViewModel.staffMaster.StaffInternalId);
                    //if (checkInternalID == true)
                    //{                       
                    //Add Staff Portal Access
                    if (!string.IsNullOrWhiteSpace(staffAddViewModel.PasswordHash) && !string.IsNullOrWhiteSpace(staffAddViewModel.staffMaster.LoginEmailAddress))
                    {
                        UserMaster userMaster = new UserMaster();

                        var decrypted = Utility.Decrypt(staffAddViewModel.PasswordHash);
                        string passwordHash = Utility.GetHashedPassword(decrypted);

                        var loginInfo = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.EmailAddress == staffAddViewModel.staffMaster.LoginEmailAddress);

                        if (loginInfo == null)
                        {
                            var membership = this.context?.Membership.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.SchoolId == staffAddViewModel.staffMaster.SchoolId && x.ProfileType == "Teacher");

                            userMaster.SchoolId = staffAddViewModel.staffMaster.SchoolId;
                            userMaster.TenantId = staffAddViewModel.staffMaster.TenantId;
                            userMaster.UserId = staffAddViewModel.staffMaster.StaffId;
                            userMaster.LangId = 1;
                            //userMaster.MembershipId = membership.MembershipId;
                            userMaster.MembershipId = membership!.MembershipId;
                            userMaster.EmailAddress = staffAddViewModel.staffMaster.LoginEmailAddress;
                            userMaster.PasswordHash = passwordHash;
                            //userMaster.Name = staffAddViewModel.staffMaster.FirstGivenName;
                            userMaster.Name = staffAddViewModel.staffMaster.FirstGivenName ?? "";
                            userMaster.IsActive = staffAddViewModel.staffMaster.PortalAccess;
                            userMaster.CreatedBy = staffAddViewModel.staffMaster.CreatedBy;
                            userMaster.CreatedOn = DateTime.UtcNow;

                            this.context?.UserMaster.Add(userMaster);
                            this.context?.SaveChanges();
                        }
                        else
                        {
                            staffAddViewModel.staffMaster = null;
                            staffAddViewModel._failure = true;
                            staffAddViewModel._message = "Staff login email already exist";
                            return staffAddViewModel;
                        }
                    }

                    this.context?.StaffMaster.Add(staffAddViewModel.staffMaster);
                    this.context?.SaveChanges();

                    //Insert data into StaffSchoolInfo table            
                    int? Id = Utility.GetMaxPK(this.context, new Func<StaffSchoolInfo, int>(x => (int)x.Id));
                    var schoolName = this.context?.SchoolMaster.Where(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.SchoolId == staffAddViewModel.staffMaster.SchoolId).Select(s => s.SchoolName).FirstOrDefault();
                    var StaffSchoolInfoData = new StaffSchoolInfo() { TenantId = staffAddViewModel.staffMaster.TenantId, SchoolId = staffAddViewModel.staffMaster.SchoolId, StaffId = staffAddViewModel.staffMaster.StaffId, SchoolAttachedId = staffAddViewModel.staffMaster.SchoolId,/* Id = (int)Id!*/Id=0, SchoolAttachedName = schoolName, StartDate = DateTime.UtcNow, CreatedOn = DateTime.UtcNow, CreatedBy = staffAddViewModel.staffMaster.CreatedBy, Profile = "Teacher", MembershipId = 4 };
                    this.context?.StaffSchoolInfo.Add(StaffSchoolInfoData);
                    this.context?.SaveChanges();

                    if (staffAddViewModel.fieldsCategoryList != null && staffAddViewModel.fieldsCategoryList.ToList().Count > 0)
                    {
                        var fieldsCategory = staffAddViewModel.fieldsCategoryList.FirstOrDefault(x => x.CategoryId == staffAddViewModel.SelectedCategoryId);
                        if (fieldsCategory != null)
                        {
                            foreach (var customFields in fieldsCategory.CustomFields.ToList())
                            {
                                if (customFields.CustomFieldsValue != null && customFields.CustomFieldsValue.ToList().Count > 0)
                                {
                                    customFields.CustomFieldsValue.FirstOrDefault()!.Module = "Staff";
                                    customFields.CustomFieldsValue.FirstOrDefault()!.CategoryId = customFields.CategoryId;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.FieldId = customFields.FieldId;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldTitle = customFields.Title;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldType = customFields.Type;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.TenantId = staffAddViewModel.staffMaster.TenantId;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.SchoolId = staffAddViewModel.staffMaster.SchoolId;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.TargetId = staffAddViewModel.staffMaster.StaffId;
                                    this.context?.CustomFieldsValue.AddRange(customFields.CustomFieldsValue);
                                    this.context?.SaveChanges();
                                }
                            }

                        }
                    }

                    staffAddViewModel._failure = false;
                    staffAddViewModel._message = "Staff added successfully";
                    //}
                    //else
                    //{
                    //    staffAddViewModel.staffMaster = null;
                    //    staffAddViewModel.fieldsCategoryList = null;
                    //    staffAddViewModel._failure = true;
                    //    staffAddViewModel._message = "Staff InternalID Already Exist";
                    //}
                    transaction?.Commit();

                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    staffAddViewModel._message = es.Message;
                    staffAddViewModel._failure = true;
                    staffAddViewModel._tenantName = staffAddViewModel._tenantName;
                    staffAddViewModel._token = staffAddViewModel._token;
                }
            }
            return staffAddViewModel;
        }

        //Checking Internal ID
        private bool CheckInternalID(Guid TenantId, string InternalID)
        {
            if (InternalID != null && InternalID != "")
            {
                var checkInternalId = this.context?.StaffMaster.Where(x => x.TenantId == TenantId && x.StaffInternalId == InternalID).ToList();
                if (checkInternalId!=null&& checkInternalId.Any())
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
        /// Get All Staff List
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public StaffListModel GetAllStaffList(PageResult pageResult)
        {
            StaffListModel staffListModel = new StaffListModel();
            IQueryable<StaffMaster>? transactionIQ = null;
            IQueryable<StaffMaster>? StaffMasterList = null;
          
            var membershipData = this.context?.UserMaster.Include(x => x.Membership).FirstOrDefault(x => x.TenantId == pageResult.TenantId && x.EmailAddress == pageResult.EmailAddress);

            var activeSchools = this.context?.SchoolDetail.Where(x => x.Status == true).Select(x => x.SchoolId).ToList();

            if (membershipData != null)
            {
                //if (membershipData.Membership.ProfileType?.ToLower() == "super administrator")
                if  (String.Compare(membershipData.Membership.ProfileType, "super administrator", true) == 0)
                {
                    if (pageResult.IsHomeRoomTeacher == true)
                    {
                        var schoolAttachedStaffId = this.context?.StaffSchoolInfo.Where(x => x.TenantId == pageResult.TenantId && x.Profile!.ToLower() == "Homeroom Teacher".ToLower() && (pageResult.SearchAllSchool == false || pageResult.SearchAllSchool == null ? x.SchoolAttachedId == pageResult.SchoolId : activeSchools!.Contains(x.SchoolAttachedId)) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date : true)).Select(s => s.StaffId).ToList();

                        StaffMasterList = this.context?.StaffMaster.Include(x => x.StaffSchoolInfo).Where(x => x.TenantId == pageResult.TenantId && (x.Profile ?? "").ToLower() != "super administrator" && schoolAttachedStaffId!.Contains(x.StaffId) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.IsActive != false : true));
                    }
                    else
                    {
                        var schoolAttachedStaffId = this.context?.StaffSchoolInfo.Where(x => x.TenantId == pageResult.TenantId && (pageResult.SearchAllSchool == false || pageResult.SearchAllSchool == null ? x.SchoolAttachedId == pageResult.SchoolId : activeSchools!.Contains(x.SchoolAttachedId)) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date : true)).Select(s => s.StaffId).ToList();

                        StaffMasterList = this.context?.StaffMaster.Include(x => x.StaffSchoolInfo).Where(x => x.TenantId == pageResult.TenantId && (x.Profile ?? "").ToLower() != "super administrator" && schoolAttachedStaffId!.Contains(x.StaffId) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.IsActive != false : true));
                    }

                    //StaffMasterList = this.context?.StaffMaster.Include(x => x.StaffSchoolInfo).Where(x => x.TenantId == pageResult.TenantId && x.Profile.ToLower() != "super administrator" && schoolAttachedStaffId.Contains(x.StaffId) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.IsActive != false : true));

                    
                    //StaffMasterList = this.context?.StaffMaster.Include(x => x.StaffSchoolInfo).Where(x => x.TenantId == pageResult.TenantId && !(String.Compare(x.Profile, "super administrator", true) == 0) && schoolAttachedStaffId!.Contains(x.StaffId) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.IsActive != false : true));
                }
                else
                {
                    var schoolAttachedId = this.context?.StaffSchoolInfo.Where(x => x.TenantId == pageResult.TenantId && x.StaffId == membershipData.UserId && x.EndDate == null && activeSchools!.Contains(x.SchoolAttachedId)).ToList().Select(s => s.SchoolAttachedId);

                    //var schoolAttachedStaffId = this.context?.StaffSchoolInfo.Where(x => x.TenantId == pageResult.TenantId && (pageResult.SearchAllSchool == false || pageResult.SearchAllSchool == null ? x.SchoolAttachedId == pageResult.SchoolId : schoolAttachedId.Contains(x.SchoolId)) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date : true)).Select(s => s.StaffId).ToList();
                    if (pageResult.IsHomeRoomTeacher == true)
                    {
                        var schoolAttachedStaffId = this.context?.StaffSchoolInfo.Where(x => x.TenantId == pageResult.TenantId && x.Profile!.ToLower() == "Homeroom Teacher".ToLower() && (pageResult.SearchAllSchool == false || pageResult.SearchAllSchool == null ? x.SchoolAttachedId == pageResult.SchoolId : schoolAttachedId!.Contains(x.SchoolId)) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date : true)).Select(s => s.StaffId).ToList();

                        StaffMasterList = this.context?.StaffMaster.Include(x => x.StaffSchoolInfo).Where(x => x.TenantId == pageResult.TenantId && (x.Profile ?? "").ToLower() != "super administrator" && schoolAttachedStaffId!.Contains(x.StaffId) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.IsActive != false : true));
                    }
                    else
                    {
                        var schoolAttachedStaffId = this.context?.StaffSchoolInfo.Where(x => x.TenantId == pageResult.TenantId && (pageResult.SearchAllSchool == false || pageResult.SearchAllSchool == null ? x.SchoolAttachedId == pageResult.SchoolId : schoolAttachedId!.Contains(x.SchoolId)) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date : true)).Select(s => s.StaffId).ToList();

                        StaffMasterList = this.context?.StaffMaster.Include(x => x.StaffSchoolInfo).Where(x => x.TenantId == pageResult.TenantId && (x.Profile ?? "").ToLower() != "super administrator" && schoolAttachedStaffId!.Contains(x.StaffId) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.IsActive != false : true));
                    }
                        
                    //StaffMasterList = this.context?.StaffMaster.Include(x => x.StaffSchoolInfo).Where(x => x.TenantId == pageResult.TenantId && !(String.Compare(x.Profile, "super administrator", true) == 0) && schoolAttachedStaffId!.Contains(x.StaffId) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.IsActive != false : true));
                }
            }

            //var StaffMasterList = this.context?.StaffMaster.Include(x => x.StaffSchoolInfo).Where(x => x.TenantId == pageResult.TenantId && x.Profile.ToLower() != "super administrator" && (pageResult.SearchAllSchool == false || pageResult.SearchAllSchool == null ? x.SchoolId == pageResult.SchoolId : true) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.StaffSchoolInfo.FirstOrDefault().EndDate == null || x.StaffSchoolInfo.FirstOrDefault().EndDate >= DateTime.UtcNow : true));

            try
            {
                if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                {

                    transactionIQ = StaffMasterList;
                }
                else
                {
                    if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                    {
                        string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                        var Searchvalue = Columnvalue.Trim();
                        Columnvalue = Regex.Replace(Columnvalue, @"\s+", "");

                        transactionIQ = StaffMasterList?.Where(x => x.FirstGivenName != null && x.FirstGivenName.Contains(Columnvalue) ||
                                                                    x.MiddleName != null && x.MiddleName.Contains(Columnvalue) ||
                                                                    x.LastFamilyName != null && x.LastFamilyName.Contains(Columnvalue) ||
                                                                    (x.FirstGivenName + x.MiddleName + x.LastFamilyName).Contains(Columnvalue) || (x.FirstGivenName + x.MiddleName).Contains(Columnvalue) || (x.FirstGivenName + x.LastFamilyName).Contains(Columnvalue) || (x.MiddleName + x.LastFamilyName).Contains(Columnvalue) ||
                                                                    x.StaffInternalId != null && x.StaffInternalId.Contains(Searchvalue) ||
                                                                    x.JobTitle != null && x.JobTitle.Contains(Searchvalue) ||
                                                                    x.SchoolEmail != null && x.SchoolEmail.Contains(Searchvalue) ||
                                                                    x.MobilePhone != null && x.MobilePhone.Contains(Searchvalue)
                                                                    || x.Gender != null && x.Gender.Contains(Searchvalue) || (x.StaffSchoolInfo.Count > 0 ? (x.StaffSchoolInfo.Any(x => (x.Profile ?? "").ToLower().Contains(Searchvalue.ToLower()) && x.SchoolAttachedId == pageResult.SchoolId)) : string.Empty.Contains(Searchvalue)));

                        //var ProfileFilter = StaffMasterList.Where(x => x.StaffSchoolInfo.FirstOrDefault() != null ? x.StaffSchoolInfo.FirstOrDefault().Profile.ToLower().Contains(Columnvalue.ToLower()) : string.Empty.Contains(Columnvalue));

                        //var ProfileFilter = StaffMasterList?.Where(x => x.StaffSchoolInfo.Count > 0 ? x.StaffSchoolInfo.Any(x => (x.Profile??"").ToLower().Contains(Columnvalue.ToLower()) && x.SchoolAttachedId==pageResult.SchoolId) : string.Empty.Contains(Columnvalue));

                        //if (ProfileFilter!=null && ProfileFilter.Any())
                        //{
                        //    transactionIQ = transactionIQ?.Concat(ProfileFilter);
                        //}
                    }
                    else
                    {
                        if (pageResult.FilterParams != null&& pageResult.FilterParams.Any(x => x.ColumnName.ToLower() == "profile"))
                        {
                            var filterValue = pageResult.FilterParams.Where(x => x.ColumnName.ToLower() == "profile").Select(x => x.FilterValue).FirstOrDefault();
                            //var profileFilterData = StaffMasterList.Where(x => x.StaffSchoolInfo.FirstOrDefault().Profile.ToLower() == filterValue.ToLower());
                            var profileFilterData = StaffMasterList?.Where(x => x.StaffSchoolInfo.Any(x =>( x.Profile??"").ToLower() == (filterValue??"").ToLower() && x.SchoolAttachedId == pageResult.SchoolId));

                            if (profileFilterData?.Any()==true)
                            {
                                //transactionIQ = transactionIQ.Concat(a);
                                StaffMasterList = profileFilterData;
                                var indexValue = pageResult.FilterParams.FindIndex(x => x.ColumnName.ToLower() == "profile");
                                pageResult.FilterParams.RemoveAt(indexValue);
                            }
                        }   

                        transactionIQ = Utility.FilteredData(pageResult.FilterParams!, StaffMasterList!).AsQueryable();
                    }
                    //transactionIQ = transactionIQ.Distinct();
                }

                if (pageResult.DobStartDate != null && pageResult.DobEndDate != null)
                {
                    var filterInDateRange = transactionIQ?.Where(x => x.Dob >= pageResult.DobStartDate && x.Dob <= pageResult.DobEndDate).AsQueryable();

                    if (filterInDateRange?.Any() == true)
                    {
                        transactionIQ = filterInDateRange;
                    }
                }

                if (pageResult.FullName != null)
                {
                    var staffName = pageResult.FullName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    if (staffName.Length > 1)
                    {
                        var firstName = staffName.First();
                        var lastName = staffName.Last();
                        //pageResult.FullName = null;

                        if (pageResult.FullName == null)
                        {
                            var nameSearch = transactionIQ?.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.FirstGivenName!.StartsWith(firstName.ToString()) && x.LastFamilyName!.StartsWith(lastName.ToString()));

                            //transactionIQ = transactionIQ.Concat(nameSearch);
                            transactionIQ = nameSearch;
                        }
                    }
                    else
                    {
                        var nameSearch = transactionIQ?.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && (x.FirstGivenName!.StartsWith(pageResult.FullName) || x.LastFamilyName!.StartsWith(pageResult.FullName)));

                        //transactionIQ = transactionIQ.Concat(nameSearch);
                        transactionIQ = nameSearch;
                    }
                }

                if (pageResult.SortingModel != null)
                {
                    switch (pageResult.SortingModel.SortColumn?.ToLower())
                    {
                        case "profile":

                            if (pageResult.SortingModel.SortDirection?.ToLower() == "asc")
                            {
                                transactionIQ = transactionIQ?.OrderBy(a => a.StaffSchoolInfo.Count > 0 ? a.StaffSchoolInfo.FirstOrDefault()!.Profile : null);
                            }
                            else
                            {
                                transactionIQ = transactionIQ?.OrderByDescending(a => a.StaffSchoolInfo.Count > 0 ? a.StaffSchoolInfo.FirstOrDefault()!.Profile : null);
                            }
                            break;
                        default:
                            transactionIQ = Utility.Sort(transactionIQ!, pageResult.SortingModel.SortColumn!, pageResult.SortingModel.SortDirection!.ToLower());
                            break;
                    }
                }
                else
                {
                    transactionIQ = transactionIQ?.OrderBy(e => e.LastFamilyName).ThenBy(c => c.FirstGivenName);
                }

                int totalCount = transactionIQ!=null? transactionIQ.Count():0;
                if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                {
                    transactionIQ = transactionIQ?.Select(e => new StaffMaster
                    {
                        TenantId = e.TenantId,
                        StaffId = e.StaffId,
                        SchoolId = e.SchoolId,
                        StaffInternalId = e.StaffInternalId,
                        FirstGivenName = e.FirstGivenName,
                        MiddleName = e.MiddleName,
                        LastFamilyName = e.LastFamilyName,
                        Profile = e.StaffSchoolInfo.Count > 0 ? e.StaffSchoolInfo.Where(x => (pageResult.SearchAllSchool == true || x.SchoolAttachedId == pageResult.SchoolId) && (pageResult.IncludeInactive == true || x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date)).FirstOrDefault()!.Profile : e.Profile,
                        JobTitle = e.JobTitle,
                        SchoolEmail = e.SchoolEmail,
                        LoginEmailAddress = e.LoginEmailAddress,
                        MobilePhone = e.MobilePhone,
                        HomeroomTeacher = e.HomeroomTeacher,
                        PrimaryGradeLevelTaught = e.PrimaryGradeLevelTaught,
                        OtherGradeLevelTaught = e.OtherGradeLevelTaught,
                        PrimarySubjectTaught = e.PrimarySubjectTaught,
                        OtherSubjectTaught = e.OtherSubjectTaught,
                        FirstLanguage = e.FirstLanguage,
                        SecondLanguage = e.SecondLanguage,
                        ThirdLanguage = e.ThirdLanguage,
                        StaffPhoto = pageResult.ProfilePhoto != null ? e.StaffThumbnailPhoto : null,
                        Dob = e.Dob,
                        Gender = e.Gender,
                        CreatedBy = e.CreatedBy,
                        CreatedOn = e.CreatedOn,
                        UpdatedBy = e.UpdatedBy,
                        UpdatedOn = e.UpdatedOn,
                        IsActive = e.IsActive,
                        StaffSchoolInfo = e.StaffSchoolInfo.Count > 0 ? e.StaffSchoolInfo.Select(s => new StaffSchoolInfo { StartDate = s.StartDate, EndDate = s.EndDate, SchoolAttachedId = s.SchoolAttachedId, SchoolId = s.SchoolId, SchoolAttachedName = s.SchoolAttachedName }).ToList() : new()
                    }).Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                }
                else
                {
                    transactionIQ = transactionIQ?.Select(e => new StaffMaster
                    {
                        TenantId = e.TenantId,
                        StaffId = e.StaffId,
                        SchoolId = e.SchoolId,
                        StaffInternalId = e.StaffInternalId,
                        FirstGivenName = e.FirstGivenName,
                        MiddleName = e.MiddleName,
                        LastFamilyName = e.LastFamilyName,
                        Profile = e.StaffSchoolInfo.Count > 0 ? e.StaffSchoolInfo.Where(x => (pageResult.SearchAllSchool == true || x.SchoolAttachedId == pageResult.SchoolId) && (pageResult.IncludeInactive == true || x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date)).FirstOrDefault()!.Profile : e.Profile,
                        JobTitle = e.JobTitle,
                        SchoolEmail = e.SchoolEmail,
                        LoginEmailAddress = e.LoginEmailAddress,
                        MobilePhone = e.MobilePhone,
                        HomeroomTeacher = e.HomeroomTeacher,
                        PrimaryGradeLevelTaught = e.PrimaryGradeLevelTaught,
                        OtherGradeLevelTaught = e.OtherGradeLevelTaught,
                        PrimarySubjectTaught = e.PrimarySubjectTaught,
                        OtherSubjectTaught = e.OtherSubjectTaught,
                        FirstLanguage = e.FirstLanguage,
                        SecondLanguage = e.SecondLanguage,
                        ThirdLanguage = e.ThirdLanguage,
                        StaffPhoto = pageResult.ProfilePhoto != null ? e.StaffThumbnailPhoto : null,
                        Dob = e.Dob,
                        Gender = e.Gender,
                        CreatedBy = e.CreatedBy,
                        CreatedOn = e.CreatedOn,
                        UpdatedBy = e.UpdatedBy,
                        UpdatedOn = e.UpdatedOn,
                        IsActive = e.IsActive,
                        StaffSchoolInfo = e.StaffSchoolInfo.Count > 0 ? e.StaffSchoolInfo.Select(s => new StaffSchoolInfo { StartDate = s.StartDate, EndDate = s.EndDate, SchoolAttachedId = s.SchoolAttachedId, SchoolId = s.SchoolId, SchoolAttachedName = s.SchoolAttachedName }).ToList() : new()
                    });
                }
                if(transactionIQ!=null && transactionIQ.Any())
                {
                    staffListModel.staffMaster = transactionIQ.ToList();
                }
                //staffListModel.staffMaster = transactionIQ.ToList();

                staffListModel.staffMaster.ForEach(e =>
                {
                    e.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, e.CreatedBy);
                    e.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, e.UpdatedBy);
                });
                
                //var staffList = transactionIQ.Select(s => new GetStaffListForView
                //{
                //    TenantId = s.TenantId,
                //    StaffId = s.StaffId,
                //    StaffInternalId = s.StaffInternalId,
                //    FirstGivenName = s.FirstGivenName,
                //    MiddleName = s.MiddleName,
                //    LastFamilyName = s.LastFamilyName,
                //    Profile = s.Profile,
                //    JobTitle = s.JobTitle,
                //    SchoolEmail = s.SchoolEmail,
                //    MobilePhone = s.MobilePhone,
                //}).ToList();

                staffListModel.TenantId = pageResult.TenantId;
                //staffListModel.getStaffListForView = staffList;
                //staffListModel.staffMaster = transactionIQ.ToList();
                staffListModel.TotalCount = totalCount;
                staffListModel.PageNumber = pageResult.PageNumber;
                staffListModel._pageSize = pageResult.PageSize;
                staffListModel._tenantName = pageResult._tenantName;
                staffListModel._token = pageResult._token;
                staffListModel._failure = false;
            }
            catch (Exception es)
            {
                staffListModel._message = es.Message;
                staffListModel._failure = true;
                staffListModel._tenantName = pageResult._tenantName;
                staffListModel._token = pageResult._token;
            }
            return staffListModel;
        }

        /// <summary>
        ///  Get Staff By Id
        /// </summary>
        /// <param name="staffAddViewModel"></param>
        /// <returns></returns>
        public StaffAddViewModel ViewStaff(StaffAddViewModel staffAddViewModel)
        {
            if (staffAddViewModel.staffMaster is null)
            {
                return staffAddViewModel;
            }
               StaffAddViewModel staffView = new StaffAddViewModel();
            try
            {
                var staffData = this.context?.StaffMaster.Include(x=>x.StaffSchoolInfo).FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId /*&& x.SchoolId==staffAddViewModel.staffMaster.SchoolId*/ &&  x.StaffId == staffAddViewModel.staffMaster.StaffId);
                if (staffData != null)
                {
                    if (staffData.StaffSchoolInfo.Count() > 0)
                    {
                       // staffData.Profile = staffData.StaffSchoolInfo.FirstOrDefault().Profile;
                        staffData.Profile = staffData.StaffSchoolInfo.FirstOrDefault()?.Profile;
                        List<int> ids = new List<int>();
                        foreach (var StaffSchoolInfo in staffData.StaffSchoolInfo)
                        {
                            if (StaffSchoolInfo.SchoolId == StaffSchoolInfo.SchoolAttachedId)
                            {
                                staffView.DefaultSchoolName = StaffSchoolInfo.SchoolAttachedName;
                            }
                            else
                            {
                                ids.Add((int)StaffSchoolInfo.SchoolAttachedId!);
                            }
                        }
                        staffView.ExternalSchoolIds = ids.ToArray();
                    }
                    staffData.StaffSchoolInfo = new HashSet<StaffSchoolInfo>();
                    staffView.staffMaster = staffData;

                    
                    if (staffAddViewModel.ExternalSchoolId != staffAddViewModel.staffMaster?.SchoolId)
                    {
                        var externalCustomFields = this.context?.FieldsCategory.Where(x => x.TenantId == staffAddViewModel.staffMaster!.TenantId && x.SchoolId == staffAddViewModel.ExternalSchoolId && x.Module == "Staff").OrderByDescending(x => x.IsSystemCategory).ThenBy(x => x.SortOrder)
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
                            CustomFields = y.CustomFields.Select(z => new CustomFields
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
                                CustomFieldsValue = z.CustomFieldsValue.Where(w => w.TargetId == staffAddViewModel.staffMaster!.StaffId).ToList()
                            }).OrderByDescending(x => x.SystemField).ThenBy(x => x.SortOrder).ToList()
                        }).ToList();

                        if(externalCustomFields?.Any()==true)
                        {
                            staffView.fieldsCategoryList = externalCustomFields;
                        }
                        
                    }
                    else
                    {
                        var customFields = this.context?.FieldsCategory.Where(x => x.TenantId == staffAddViewModel.staffMaster!.TenantId && x.SchoolId == staffAddViewModel.staffMaster.SchoolId && x.Module == "Staff").OrderByDescending(x => x.IsSystemCategory).ThenBy(x => x.SortOrder)
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
                            CustomFields = y.CustomFields.Select(z => new CustomFields
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
                                CustomFieldsValue = z.CustomFieldsValue.Where(w => w.TargetId == staffAddViewModel.staffMaster!.StaffId).ToList()
                            }).OrderByDescending(x => x.SystemField).ThenBy(x => x.SortOrder).ToList()
                        }).ToList();
                        if(customFields?.Any()==true)
                        {
                            staffView.fieldsCategoryList = customFields;
                        }
                        
                    }
                    
                    staffView._tenantName = staffAddViewModel._tenantName;
                    staffView._token = staffAddViewModel._token;
                }
                else
                {
                    staffView._tenantName = staffAddViewModel._tenantName;
                    staffView._token = staffAddViewModel._token;
                    staffView._failure = true;
                    staffView._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                staffView._tenantName = staffAddViewModel._tenantName;
                staffView._token = staffAddViewModel._token;
                staffView._failure = true;
                staffView._message = es.Message;
            }
            return staffView;
        }

        /// <summary>
        /// Update Staff
        /// </summary>
        /// <param name="staffAddViewModel"></param>
        /// <returns></returns>
        public StaffAddViewModel UpdateStaff(StaffAddViewModel staffAddViewModel)
        {
            if (staffAddViewModel.staffMaster != null)
            {
                using (var transaction = this.context?.Database.BeginTransaction())
                {
                    try
                    {
                        var checkInternalId = this.context?.StaffMaster.Where(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.StaffInternalId == staffAddViewModel.staffMaster.StaffInternalId && x.StaffInternalId != null && x.StaffId != staffAddViewModel.staffMaster.StaffId).ToList();
                        if (checkInternalId != null && checkInternalId.Any())
                        {
                            staffAddViewModel.staffMaster = null;
                            //staffAddViewModel.fieldsCategoryList = null;
                            staffAddViewModel._failure = true;
                            staffAddViewModel._message = "Staff id already exist";
                        }
                        else
                        {
                            var staffUpdate = this.context?.StaffMaster.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.StaffId == staffAddViewModel.staffMaster.StaffId);
                            if (staffUpdate != null)
                            {
                                if (string.IsNullOrEmpty(staffAddViewModel.staffMaster.StaffInternalId))
                                {
                                    staffAddViewModel.staffMaster.StaffInternalId = staffUpdate.StaffInternalId;
                                }

                                //Add or Update staff portal access
                                if (staffUpdate.LoginEmailAddress != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(staffAddViewModel.staffMaster.LoginEmailAddress))
                                    {
                                        if (staffUpdate.LoginEmailAddress != staffAddViewModel.staffMaster.LoginEmailAddress)
                                        {
                                            var loginInfo = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.EmailAddress == staffAddViewModel.staffMaster.LoginEmailAddress);

                                            if (loginInfo != null)
                                            {
                                                staffAddViewModel.staffMaster = null;
                                                staffAddViewModel._failure = true;
                                                staffAddViewModel._message = "Staff login email already exist";
                                                return staffAddViewModel;
                                            }
                                            else
                                            {
                                                var loginInfoData = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.EmailAddress == staffUpdate.LoginEmailAddress);

                                                var loginInfoDataForRemove = loginInfoData;

                                                if (loginInfoDataForRemove != null)
                                                {
                                                    this.context?.UserMaster.Remove(loginInfoDataForRemove);
                                                    this.context?.SaveChanges();
                                                }

                                                if (loginInfoData != null)
                                                {
                                                    loginInfoData.EmailAddress = staffAddViewModel.staffMaster.LoginEmailAddress;
                                                    loginInfoData.IsActive = staffAddViewModel.staffMaster.PortalAccess;
                                                    loginInfoData.UpdatedBy = staffAddViewModel.staffMaster.UpdatedBy;
                                                    loginInfoData.UpdatedOn = DateTime.UtcNow;
                                                    this.context?.UserMaster.Add(loginInfoData);
                                                    this.context?.SaveChanges();
                                                }
                                                    
                                                //loginInfoData.Name = staffAddViewModel.staffMaster.FirstGivenName;

                                              

                                                //Update Parent Login in ParentInfo table.
                                                //staffUpdate.LoginEmailAddress = staffAddViewModel.staffMaster.LoginEmailAddress;
                                            }
                                        }
                                        else
                                        {
                                            var loginInfo = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.EmailAddress == staffUpdate.LoginEmailAddress);
                                            if (loginInfo != null)
                                            {
                                                loginInfo.IsActive = staffAddViewModel.staffMaster.PortalAccess;
                                                loginInfo.UpdatedBy = staffAddViewModel.staffMaster.UpdatedBy;
                                                loginInfo.UpdatedOn = DateTime.UtcNow;
                                                this.context?.SaveChanges();
                                            }
                                                
                                            //loginInfo.Name = staffAddViewModel.staffMaster.FirstGivenName;

                                            
                                        }
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(staffAddViewModel.staffMaster.LoginEmailAddress) && !string.IsNullOrWhiteSpace(staffAddViewModel.PasswordHash))
                                    {
                                        var decrypted = Utility.Decrypt(staffAddViewModel.PasswordHash);
                                        string passwordHash = Utility.GetHashedPassword(decrypted);

                                        UserMaster userMaster = new UserMaster();

                                        var loginInfo = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.EmailAddress == staffAddViewModel.staffMaster.LoginEmailAddress);

                                        if (loginInfo == null)
                                        {
                                            var staffSchoolInfoData = this.context?.StaffSchoolInfo.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.StaffId == staffAddViewModel.staffMaster.StaffId);

                                            if (staffSchoolInfoData != null)
                                            {
                                                var membership = this.context?.Membership.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.SchoolId == staffAddViewModel.staffMaster.SchoolId && x.Profile == staffSchoolInfoData.Profile);

                                                userMaster.SchoolId = staffAddViewModel.staffMaster.SchoolId;
                                                userMaster.TenantId = staffAddViewModel.staffMaster.TenantId;
                                                userMaster.UserId = staffAddViewModel.staffMaster.StaffId;
                                                userMaster.LangId = 1;
                                                //userMaster.MembershipId = membership.MembershipId;
                                                userMaster.MembershipId = membership != null ? membership.MembershipId : 0;
                                                userMaster.EmailAddress = staffAddViewModel.staffMaster.LoginEmailAddress;
                                                userMaster.PasswordHash = passwordHash;
                                                //userMaster.Name = staffAddViewModel.staffMaster.FirstGivenName;
                                                userMaster.Name = staffAddViewModel.staffMaster.FirstGivenName != null ? staffAddViewModel.staffMaster.FirstGivenName : "";
                                                userMaster.IsActive = staffAddViewModel.staffMaster.PortalAccess;
                                                userMaster.CreatedBy = staffAddViewModel.staffMaster.UpdatedBy;
                                                userMaster.CreatedOn = DateTime.UtcNow;

                                                this.context?.UserMaster.Add(userMaster);
                                                this.context?.SaveChanges();
                                            }
                                            //Update LoginEmailAddress in StaffMaster table.
                                            //staffUpdate.LoginEmailAddress = staffAddViewModel.staffMaster.LoginEmailAddress;
                                        }
                                        else
                                        {
                                            staffAddViewModel.staffMaster = null;
                                            staffAddViewModel._failure = true;
                                            staffAddViewModel._message = "Staff login email already exist";
                                            return staffAddViewModel;
                                        }
                                    }
                                }

                                staffAddViewModel.staffMaster.StaffGuid = staffUpdate.StaffGuid;
                                staffAddViewModel.staffMaster.UpdatedOn = DateTime.UtcNow;
                                staffAddViewModel.staffMaster.CreatedOn = staffUpdate.CreatedOn;
                                staffAddViewModel.staffMaster.CreatedBy = staffUpdate.CreatedBy;
                                staffAddViewModel.staffMaster.IsActive = staffUpdate.IsActive;


                                this.context?.Entry(staffUpdate).CurrentValues.SetValues(staffAddViewModel.staffMaster);
                                this.context?.SaveChanges();

                                if (staffAddViewModel.fieldsCategoryList != null && staffAddViewModel.fieldsCategoryList.ToList().Count > 0)
                                {
                                    var fieldsCategory = staffAddViewModel.fieldsCategoryList.FirstOrDefault(x => x.CategoryId == staffAddViewModel.SelectedCategoryId);
                                    if (fieldsCategory != null)
                                    {
                                        foreach (var customFields in fieldsCategory.CustomFields.ToList())
                                        {
                                            var customFieldValueData = this.context?.CustomFieldsValue.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.SchoolId == staffAddViewModel.staffMaster.SchoolId && x.CategoryId == customFields.CategoryId && x.FieldId == customFields.FieldId && x.Module == "Staff" && x.TargetId == staffAddViewModel.staffMaster.StaffId);
                                            if (customFieldValueData != null)
                                            {
                                                this.context?.CustomFieldsValue.RemoveRange(customFieldValueData);
                                            }
                                            if (customFields.CustomFieldsValue != null && customFields.CustomFieldsValue.ToList().Count > 0)
                                            {
                                                customFields.CustomFieldsValue.FirstOrDefault()!.Module = "Staff";
                                                customFields.CustomFieldsValue.FirstOrDefault()!.CategoryId = customFields.CategoryId;
                                                customFields.CustomFieldsValue.FirstOrDefault()!.FieldId = customFields.FieldId;
                                                customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldTitle = customFields.Title;
                                                customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldType = customFields.Type;
                                                customFields.CustomFieldsValue.FirstOrDefault()!.SchoolId = staffAddViewModel.staffMaster.SchoolId;
                                                customFields.CustomFieldsValue.FirstOrDefault()!.TenantId = staffAddViewModel.staffMaster.TenantId;
                                                customFields.CustomFieldsValue.FirstOrDefault()!.TargetId = staffAddViewModel.staffMaster.StaffId;
                                                this.context?.CustomFieldsValue.AddRange(customFields.CustomFieldsValue);
                                                this.context?.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                staffAddViewModel._failure = false;
                                staffAddViewModel._message = "Staff updated successfully";
                                transaction?.Commit();
                            }
                            //transaction.Commit();
                        }
                    }

                    catch (Exception ex)
                    {
                        transaction?.Rollback();
                        staffAddViewModel.staffMaster = null;
                        staffAddViewModel._failure = true;
                        staffAddViewModel._message = ex.Message;
                    }
                }
            }
            return staffAddViewModel;
        }

        /// <summary>
        /// Check Staff Internal Id
        /// </summary>
        /// <param name="checkStaffInternalIdViewModel"></param>
        /// <returns></returns>
        public CheckStaffInternalIdViewModel CheckStaffInternalId(CheckStaffInternalIdViewModel checkStaffInternalIdViewModel)
        {
            var checkInternalId = this.context?.StaffMaster.Where(x => x.TenantId == checkStaffInternalIdViewModel.TenantId && x.StaffInternalId == checkStaffInternalIdViewModel.StaffInternalId).ToList();
            if (checkInternalId!=null&& checkInternalId.Any())
            {
                checkStaffInternalIdViewModel.IsValidInternalId = false;
                checkStaffInternalIdViewModel._message = "Staff id already exist";
            }
            else
            {
                checkStaffInternalIdViewModel.IsValidInternalId = true;
                checkStaffInternalIdViewModel._message = "Staff id is valid";
            }
            return checkStaffInternalIdViewModel;
        }

        /// <summary>
        /// Add Staff School Info
        /// </summary>
        /// <param name="staffSchoolInfoAddViewModel"></param>
        /// <returns></returns>
        public StaffSchoolInfoAddViewModel AddStaffSchoolInfo(StaffSchoolInfoAddViewModel staffSchoolInfoAddViewModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    var staffMaster = this.context?.StaffMaster.FirstOrDefault(x => x.TenantId == staffSchoolInfoAddViewModel.TenantId && x.StaffId == staffSchoolInfoAddViewModel.StaffId && x.SchoolId == staffSchoolInfoAddViewModel.SchoolId);
                    if (staffMaster != null)
                    {
                        //staffMaster.Profile = staffSchoolInfoAddViewModel.Profile;
                        staffMaster.JobTitle = staffSchoolInfoAddViewModel.JobTitle;
                        staffMaster.JoiningDate = staffSchoolInfoAddViewModel.JoiningDate;
                        staffMaster.EndDate = staffSchoolInfoAddViewModel.EndDate;
                        staffMaster.HomeroomTeacher = staffSchoolInfoAddViewModel.HomeroomTeacher;
                        staffMaster.PrimaryGradeLevelTaught = staffSchoolInfoAddViewModel.PrimaryGradeLevelTaught;
                        staffMaster.OtherGradeLevelTaught = staffSchoolInfoAddViewModel.OtherGradeLevelTaught;
                        staffMaster.PrimarySubjectTaught = staffSchoolInfoAddViewModel.PrimarySubjectTaught;
                        staffMaster.OtherSubjectTaught = staffSchoolInfoAddViewModel.OtherSubjectTaught;
                        this.context?.SaveChanges();
                    }

                    if (staffSchoolInfoAddViewModel.staffSchoolInfoList != null && staffSchoolInfoAddViewModel.staffSchoolInfoList.ToList().Count > 0)
                    {
                        int? Id = 0;
                        Id = Utility.GetMaxPK(this.context, new Func<StaffSchoolInfo, int>(x => x.Id));
                        foreach (var staffSchoolInfo in staffSchoolInfoAddViewModel.staffSchoolInfoList.ToList())
                        {
                            staffSchoolInfo.Id = (int)Id!;
                            staffSchoolInfo.CreatedOn = DateTime.UtcNow;
                            this.context?.StaffSchoolInfo.Add(staffSchoolInfo);
                            Id++;
                        }
                        this.context?.SaveChanges();
                    }

                    if (staffSchoolInfoAddViewModel.fieldsCategoryList != null && staffSchoolInfoAddViewModel.fieldsCategoryList.ToList().Count > 0)
                    {
                        var fieldsCategory = staffSchoolInfoAddViewModel.fieldsCategoryList.FirstOrDefault(x => x.CategoryId == staffSchoolInfoAddViewModel.SelectedCategoryId);
                        if (fieldsCategory != null)
                        {
                            foreach (var customFields in fieldsCategory.CustomFields.ToList())
                            {
                                if (customFields.CustomFieldsValue != null && customFields.CustomFieldsValue.ToList().Count > 0)
                                {
                                    customFields.CustomFieldsValue.FirstOrDefault()!.Module = "Staff";
                                    customFields.CustomFieldsValue.FirstOrDefault()!.CategoryId = customFields.CategoryId;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.FieldId = customFields.FieldId;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldTitle = customFields.Title;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldType = customFields.Type;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.TenantId = (Guid)staffSchoolInfoAddViewModel.TenantId!;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.SchoolId = (int)staffSchoolInfoAddViewModel.SchoolId!;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.TargetId = (int)staffSchoolInfoAddViewModel.StaffId!;
                                    this.context?.CustomFieldsValue.AddRange(customFields.CustomFieldsValue);
                                    this.context?.SaveChanges();
                                }
                            }

                        }
                    }

                    transaction?.Commit();
                    staffSchoolInfoAddViewModel._failure = false;
                    staffSchoolInfoAddViewModel._message = "Staff school info added successfully";
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    staffSchoolInfoAddViewModel._failure = true;
                    staffSchoolInfoAddViewModel._message = es.Message;
                }
            }
            return staffSchoolInfoAddViewModel;
        }

        /// <summary>
        /// Get Staff School Info
        /// </summary>
        /// <param name="staffSchoolInfoAddViewModel"></param>
        /// <returns></returns>
        public StaffSchoolInfoAddViewModel ViewStaffSchoolInfo(StaffSchoolInfoAddViewModel staffSchoolInfoAddViewModel)
        {

            StaffSchoolInfoAddViewModel staffSchoolInfoView = new StaffSchoolInfoAddViewModel();
            try
            {
                var staffMaster = this.context?.StaffMaster.FirstOrDefault(x => x.TenantId == staffSchoolInfoAddViewModel.staffSchoolInfoList.First().TenantId && x.StaffId == staffSchoolInfoAddViewModel.staffSchoolInfoList.First().StaffId);
                if (staffMaster != null)
                {
                    staffSchoolInfoView.Profile = staffMaster.Profile;
                    staffSchoolInfoView.JobTitle = staffMaster.JobTitle;
                    staffSchoolInfoView.JoiningDate = staffMaster.JoiningDate;
                    staffSchoolInfoView.EndDate = staffMaster.EndDate;
                    staffSchoolInfoView.HomeroomTeacher = staffMaster.HomeroomTeacher;
                    staffSchoolInfoView.PrimaryGradeLevelTaught = staffMaster.PrimaryGradeLevelTaught;
                    staffSchoolInfoView.PrimarySubjectTaught = staffMaster.PrimarySubjectTaught;
                    staffSchoolInfoView.OtherGradeLevelTaught = staffMaster.OtherGradeLevelTaught;
                    staffSchoolInfoView.OtherSubjectTaught = staffMaster.OtherSubjectTaught;
                    staffSchoolInfoView.TenantId = staffMaster.TenantId;
                    staffSchoolInfoView.SchoolId = staffMaster.SchoolId;
                    staffSchoolInfoView.StaffId = staffMaster.StaffId;
                    staffSchoolInfoView._failure = false;

                }
                else
                {
                    staffSchoolInfoView._failure = true;
                    staffSchoolInfoView._message = NORECORDFOUND;
                }
                var staffSchoolInfo = this.context?.StaffSchoolInfo.Where(x => x.TenantId == staffSchoolInfoAddViewModel.staffSchoolInfoList.First().TenantId && x.StaffId == staffSchoolInfoAddViewModel.staffSchoolInfoList.First().StaffId).ToList();
                if (staffSchoolInfo != null && staffSchoolInfo.Any())
                {
                    staffSchoolInfoView.staffSchoolInfoList = staffSchoolInfo;
                }

                //for fetch custom category details
                if (staffSchoolInfoAddViewModel.ExternalSchoolId != staffSchoolInfoAddViewModel.SchoolId)
                {
                    var externalCustomFields = this.context?.FieldsCategory.Where(x => x.TenantId == staffSchoolInfoAddViewModel.TenantId && x.SchoolId == staffSchoolInfoAddViewModel.ExternalSchoolId && x.Module == "Staff").OrderByDescending(x => x.IsSystemCategory).ThenBy(x => x.SortOrder)
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
                            CustomFieldsValue = z.CustomFieldsValue.Where(w => w.TargetId == staffSchoolInfoAddViewModel.staffSchoolInfoList.First().StaffId).ToList()
                        }).OrderByDescending(x => x.SystemField).ThenBy(x => x.SortOrder).ToList()
                    }).ToList();

                    if (externalCustomFields?.Any() == true)
                    {
                        staffSchoolInfoView.fieldsCategoryList = externalCustomFields;
                    }
                }
                else
                {
                    var customFields = this.context?.FieldsCategory.Where(x => x.TenantId == staffSchoolInfoAddViewModel.TenantId && x.SchoolId == staffSchoolInfoAddViewModel.SchoolId && x.Module == "Staff").OrderByDescending(x => x.IsSystemCategory).ThenBy(x => x.SortOrder)
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
                            CustomFieldsValue = z.CustomFieldsValue.Where(w => w.TargetId == staffSchoolInfoAddViewModel.staffSchoolInfoList.First().StaffId).ToList()
                        }).OrderByDescending(x => x.SystemField).ThenBy(x => x.SortOrder).ToList()
                    }).ToList();
                    if (customFields?.Any() == true)
                    {
                        staffSchoolInfoView.fieldsCategoryList = customFields;
                    }
                }
            }
            catch (Exception es)
            {
                staffSchoolInfoView._failure = true;
                staffSchoolInfoView._message = es.Message;
            }
            return staffSchoolInfoView;
        }

        /// <summary>
        /// Update Staff School Info
        /// </summary>
        /// <param name="staffSchoolInfoAddViewModel"></param>
        /// <returns></returns>
        public StaffSchoolInfoAddViewModel UpdateStaffSchoolInfo(StaffSchoolInfoAddViewModel staffSchoolInfoAddViewModel)
        {
            if(staffSchoolInfoAddViewModel.staffSchoolInfoList is null)
            {
                return staffSchoolInfoAddViewModel;
            }
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    var staffMaster = this.context?.StaffMaster.FirstOrDefault(x => x.TenantId == staffSchoolInfoAddViewModel.TenantId && x.StaffId == staffSchoolInfoAddViewModel.StaffId && x.SchoolId == staffSchoolInfoAddViewModel.SchoolId);
                    if (staffMaster != null)
                    {
                        staffMaster.Profile = staffSchoolInfoAddViewModel.Profile;
                        staffMaster.JobTitle = staffSchoolInfoAddViewModel.JobTitle;
                        staffMaster.JoiningDate = staffSchoolInfoAddViewModel.JoiningDate;
                        staffMaster.EndDate = staffSchoolInfoAddViewModel.EndDate;
                        staffMaster.HomeroomTeacher = staffSchoolInfoAddViewModel.HomeroomTeacher;
                        staffMaster.PrimaryGradeLevelTaught = staffSchoolInfoAddViewModel.PrimaryGradeLevelTaught;
                        staffMaster.OtherGradeLevelTaught = staffSchoolInfoAddViewModel.OtherGradeLevelTaught;
                        staffMaster.PrimarySubjectTaught = staffSchoolInfoAddViewModel.PrimarySubjectTaught;
                        staffMaster.OtherSubjectTaught = staffSchoolInfoAddViewModel.OtherSubjectTaught;

                        var userMaster = this.context?.UserMaster.Include(x => x.Membership).FirstOrDefault(x => x.TenantId == staffSchoolInfoAddViewModel.TenantId && x.UserId == staffSchoolInfoAddViewModel.StaffId && x.SchoolId == staffSchoolInfoAddViewModel.SchoolId);

                        if (userMaster != null)
                        {
                            var membership = this.context?.Membership.FirstOrDefault(x => x.TenantId == staffSchoolInfoAddViewModel.TenantId && x.SchoolId == staffSchoolInfoAddViewModel.SchoolId && x.Profile == staffSchoolInfoAddViewModel.staffSchoolInfoList.FirstOrDefault()!.Profile);

                            if (membership != null)
                            {
                                userMaster.MembershipId = membership.MembershipId;
                            }
                            else
                            {
                                userMaster.MembershipId = 4;
                            }

                            userMaster.UpdatedOn = DateTime.Now;
                            userMaster.UpdatedBy = staffSchoolInfoAddViewModel.staffSchoolInfoList.FirstOrDefault()!.UpdatedBy;
                        }

                        this.context?.SaveChanges();
                    }

                    if (staffSchoolInfoAddViewModel.staffSchoolInfoList != null && staffSchoolInfoAddViewModel.staffSchoolInfoList.ToList().Count > 0)
                    {
                        var staffSchoolInfoData = this.context?.StaffSchoolInfo.Where(x => x.TenantId == staffSchoolInfoAddViewModel.TenantId && x.StaffId == staffSchoolInfoAddViewModel.StaffId && x.SchoolId == staffSchoolInfoAddViewModel.SchoolId).ToList();
                       
                        if (staffSchoolInfoData!=null&& staffSchoolInfoData.Any())
                        {
                            this.context?.StaffSchoolInfo.RemoveRange(staffSchoolInfoData);
                            this.context?.SaveChanges();
                        }

                        if (staffSchoolInfoData?.FirstOrDefault()!.SchoolId != staffSchoolInfoAddViewModel.staffSchoolInfoList.FirstOrDefault()!.SchoolId)
                        {
                            staffMaster!.SchoolId = (int)staffSchoolInfoAddViewModel.staffSchoolInfoList.FirstOrDefault()!.SchoolId!;

                            var staffCertificateData = this.context?.StaffCertificateInfo.Where(x => x.TenantId == staffSchoolInfoAddViewModel.TenantId && x.StaffId == staffSchoolInfoAddViewModel.StaffId && x.SchoolId == staffSchoolInfoAddViewModel.SchoolId).ToList();

                            if (staffCertificateData!=null&& staffCertificateData.Any())
                            {
                                staffCertificateData.ForEach(x => x.SchoolId = (int)staffSchoolInfoAddViewModel.staffSchoolInfoList.FirstOrDefault()!.SchoolId!);
                            }
                        }

                        int? Id = 0;
                        Id = Utility.GetMaxPK(this.context, new Func<StaffSchoolInfo, int>(x => x.Id));
                        foreach (var staffSchoolInfo in staffSchoolInfoAddViewModel.staffSchoolInfoList.ToList())
                        {
                            staffSchoolInfo.Id = 0;
                            //staffSchoolInfo.Id = Id != null ? (int)Id : 0;
                            staffSchoolInfo.UpdatedOn = DateTime.UtcNow;
                            staffSchoolInfo.CreatedOn = DateTime.UtcNow;
                            staffSchoolInfo.CreatedBy = staffSchoolInfo.UpdatedBy;
                            staffSchoolInfo.StaffMaster = null;
                            this.context?.StaffSchoolInfo.Add(staffSchoolInfo);
                            Id++;
                        }
                        this.context?.SaveChanges();
                    }

                    if (staffSchoolInfoAddViewModel.fieldsCategoryList != null && staffSchoolInfoAddViewModel.fieldsCategoryList.ToList().Count > 0)
                    {
                        var fieldsCategory = staffSchoolInfoAddViewModel.fieldsCategoryList.FirstOrDefault(x => x.CategoryId == staffSchoolInfoAddViewModel.SelectedCategoryId);
                        if (fieldsCategory != null)
                        {
                            foreach (var customFields in fieldsCategory.CustomFields.ToList())
                            {
                                var customFieldValueData = this.context?.CustomFieldsValue.FirstOrDefault(x => x.TenantId == staffSchoolInfoAddViewModel.TenantId && x.SchoolId == staffSchoolInfoAddViewModel.SchoolId && x.CategoryId == customFields.CategoryId && x.FieldId == customFields.FieldId && x.Module == "Staff" && x.TargetId == staffSchoolInfoAddViewModel.StaffId);
                                if (customFieldValueData != null)
                                {
                                    this.context?.CustomFieldsValue.RemoveRange(customFieldValueData);
                                }
                                if (customFields.CustomFieldsValue != null && customFields.CustomFieldsValue.ToList().Count > 0)
                                {
                                    customFields.CustomFieldsValue.FirstOrDefault()!.Module = "Staff";
                                    customFields.CustomFieldsValue.FirstOrDefault()!.CategoryId = customFields.CategoryId;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.FieldId = customFields.FieldId;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldTitle = customFields.Title;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldType = customFields.Type;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.SchoolId = (int)staffSchoolInfoAddViewModel.SchoolId!;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.TenantId = (Guid)staffSchoolInfoAddViewModel.TenantId!;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.TargetId = (int)staffSchoolInfoAddViewModel.StaffId!;
                                    this.context?.CustomFieldsValue.AddRange(customFields.CustomFieldsValue);
                                    this.context?.SaveChanges();
                                }
                            }
                        }
                    }
                    transaction?.Commit();
                    staffSchoolInfoAddViewModel._failure = false;
                    staffSchoolInfoAddViewModel._message = "Staff school info updated successfully";
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    staffSchoolInfoAddViewModel._failure = true;
                    staffSchoolInfoAddViewModel._message = es.Message;
                }
            }
            return staffSchoolInfoAddViewModel;
        }
        
        /// <summary>
        /// Add Staff Certificate Info
        /// </summary>
        /// <param name="staffCertificateInfoAddViewModel"></param>
        /// <returns></returns>
        public StaffCertificateInfoAddViewModel AddStaffCertificateInfo(StaffCertificateInfoAddViewModel staffCertificateInfoAddViewModel)
        {
            if (staffCertificateInfoAddViewModel.staffCertificateInfo is null)
            {
                return staffCertificateInfoAddViewModel;
            }
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    int? Id = Utility.GetMaxPK(this.context, new Func<StaffCertificateInfo, int>(x => x.Id));
                    staffCertificateInfoAddViewModel.staffCertificateInfo.Id = (int)Id!;
                    staffCertificateInfoAddViewModel.staffCertificateInfo.CreatedOn = DateTime.UtcNow;
                    this.context?.StaffCertificateInfo.Add(staffCertificateInfoAddViewModel.staffCertificateInfo);
                    this.context?.SaveChanges();

                    if (staffCertificateInfoAddViewModel.fieldsCategoryList != null && staffCertificateInfoAddViewModel.fieldsCategoryList.ToList().Count > 0)
                    {
                        var fieldsCategory = staffCertificateInfoAddViewModel.fieldsCategoryList.FirstOrDefault(x => x.CategoryId == staffCertificateInfoAddViewModel.SelectedCategoryId);
                        if (fieldsCategory != null)
                        {
                            foreach (var customFields in fieldsCategory.CustomFields.ToList())
                            {
                                if (customFields.CustomFieldsValue != null && customFields.CustomFieldsValue.ToList().Count > 0)
                                {
                                    customFields.CustomFieldsValue.FirstOrDefault()!.Module = "Staff";
                                    customFields.CustomFieldsValue.FirstOrDefault()!.CategoryId = customFields.CategoryId;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.FieldId = customFields.FieldId;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldTitle = customFields.Title;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldType = customFields.Type;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.TenantId = (Guid)staffCertificateInfoAddViewModel.staffCertificateInfo.TenantId!;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.SchoolId = (int)staffCertificateInfoAddViewModel.staffCertificateInfo.SchoolId!;
                                    customFields.CustomFieldsValue.FirstOrDefault()!.TargetId = (int)staffCertificateInfoAddViewModel.staffCertificateInfo.StaffId!;
                                    this.context?.CustomFieldsValue.AddRange(customFields.CustomFieldsValue);
                                    this.context?.SaveChanges();
                                }
                            }
                        }
                    }
                    transaction?.Commit();
                    staffCertificateInfoAddViewModel._failure = false;
                    staffCertificateInfoAddViewModel._message = "Staff certificate added successfully";
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    staffCertificateInfoAddViewModel._message = es.Message;
                    staffCertificateInfoAddViewModel._failure = true;
                    staffCertificateInfoAddViewModel._tenantName = staffCertificateInfoAddViewModel._tenantName;
                    staffCertificateInfoAddViewModel._token = staffCertificateInfoAddViewModel._token;
                }
            }

            return staffCertificateInfoAddViewModel;
        }

        /// <summary>
        /// Get All Staff Certificate Info
        /// </summary>
        /// <param name="staffCertificateInfoListModel"></param>
        /// <returns></returns>
        public StaffCertificateInfoListModel GetAllStaffCertificateInfo(StaffCertificateInfoListModel staffCertificateInfoListModel)
        {
            StaffCertificateInfoListModel staffCertificateInfoListView = new StaffCertificateInfoListModel();
            //IQueryable<StaffCertificateInfo> transactionIQ = null;           

            try
            {
                var staffCertificateInfoList = this.context?.StaffCertificateInfo.Where(x => x.TenantId == staffCertificateInfoListModel.TenantId && x.SchoolId == staffCertificateInfoListModel.SchoolId && x.StaffId == staffCertificateInfoListModel.StaffId).ToList();

                //staffCertificateInfoListView.staffCertificateInfoList = staffCertificateInfoList;
                staffCertificateInfoListView._tenantName = staffCertificateInfoListModel._tenantName;
                staffCertificateInfoListView.TenantId = staffCertificateInfoListModel.TenantId;
                staffCertificateInfoListView.SchoolId = staffCertificateInfoListModel.SchoolId;
                staffCertificateInfoListView.StaffId = staffCertificateInfoListModel.StaffId;
                staffCertificateInfoListView._token = staffCertificateInfoListModel._token;

                if (staffCertificateInfoList != null && staffCertificateInfoList.Any())
                {
                    staffCertificateInfoListView.staffCertificateInfoList = staffCertificateInfoList;
                    staffCertificateInfoListView._failure = false;

                    //for fetch custom category details
                    if (staffCertificateInfoListModel.ExternalSchoolId != staffCertificateInfoListModel.SchoolId)
                    {
                        var externalCustomFields = this.context?.FieldsCategory.Where(x => x.TenantId == staffCertificateInfoListModel.TenantId && x.SchoolId == staffCertificateInfoListModel.ExternalSchoolId && x.Module == "Staff").OrderByDescending(x => x.IsSystemCategory).ThenBy(x => x.SortOrder)
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
                                CustomFieldsValue = z.CustomFieldsValue.Where(w => w.TargetId == staffCertificateInfoListModel.StaffId).ToList()
                            }).OrderByDescending(x => x.SystemField).ThenBy(x => x.SortOrder).ToList()
                        }).ToList();

                        if (externalCustomFields?.Any() == true)
                        {
                            staffCertificateInfoListView.fieldsCategoryList = externalCustomFields;
                        }
                    }
                    else
                    {
                        var customFields = this.context?.FieldsCategory.Where(x => x.TenantId == staffCertificateInfoListModel.TenantId && x.SchoolId == staffCertificateInfoListModel.SchoolId && x.Module == "Staff").OrderByDescending(x => x.IsSystemCategory).ThenBy(x => x.SortOrder)
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
                                CustomFieldsValue = z.CustomFieldsValue.Where(w => w.TargetId == staffCertificateInfoListModel.StaffId).ToList()
                            }).OrderByDescending(x => x.SystemField).ThenBy(x => x.SortOrder).ToList()
                        }).ToList();
                        if (customFields?.Any() == true)
                        {
                            staffCertificateInfoListView.fieldsCategoryList = customFields;
                        }
                    }
                }
                else
                {
                    staffCertificateInfoListView._failure = true;
                    staffCertificateInfoListView._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                staffCertificateInfoListView._message = es.Message;
                staffCertificateInfoListView._failure = true;
                staffCertificateInfoListView._tenantName = staffCertificateInfoListModel._tenantName;
                staffCertificateInfoListView._token = staffCertificateInfoListModel._token;
            }
            return staffCertificateInfoListView;
        }
        
        /// <summary>
        /// Update Staff Certificate Info
        /// </summary>
        /// <param name="staffCertificateInfoAddViewModel"></param>
        /// <returns></returns>
        public StaffCertificateInfoAddViewModel UpdateStaffCertificateInfo(StaffCertificateInfoAddViewModel staffCertificateInfoAddViewModel)
        {
            if(staffCertificateInfoAddViewModel.staffCertificateInfo is null)
            {
                return staffCertificateInfoAddViewModel;
            }
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    var staffCertificateInfoUpdate = this.context?.StaffCertificateInfo.FirstOrDefault(x => x.TenantId == staffCertificateInfoAddViewModel.staffCertificateInfo.TenantId && x.SchoolId == staffCertificateInfoAddViewModel.staffCertificateInfo.SchoolId && x.StaffId == staffCertificateInfoAddViewModel.staffCertificateInfo.StaffId && x.Id == staffCertificateInfoAddViewModel.staffCertificateInfo.Id);
                    if (staffCertificateInfoUpdate != null)
                    {
                        staffCertificateInfoAddViewModel.staffCertificateInfo.UpdatedOn = DateTime.UtcNow;
                        staffCertificateInfoAddViewModel.staffCertificateInfo.CreatedBy = staffCertificateInfoUpdate.CreatedBy;
                        staffCertificateInfoAddViewModel.staffCertificateInfo.CreatedOn = staffCertificateInfoUpdate.CreatedOn;
                        this.context?.Entry(staffCertificateInfoUpdate).CurrentValues.SetValues(staffCertificateInfoAddViewModel.staffCertificateInfo);
                        this.context?.SaveChanges();

                        if (staffCertificateInfoAddViewModel.fieldsCategoryList != null && staffCertificateInfoAddViewModel.fieldsCategoryList.ToList().Count > 0)
                        {
                            var fieldsCategory = staffCertificateInfoAddViewModel.fieldsCategoryList.FirstOrDefault(x => x.CategoryId == staffCertificateInfoAddViewModel.SelectedCategoryId);
                            if (fieldsCategory != null)
                            {
                                foreach (var customFields in fieldsCategory.CustomFields.ToList())
                                {
                                    var customFieldValueData = this.context?.CustomFieldsValue.FirstOrDefault(x => x.TenantId == staffCertificateInfoAddViewModel.staffCertificateInfo.TenantId && x.SchoolId == staffCertificateInfoAddViewModel.staffCertificateInfo.SchoolId && x.CategoryId == customFields.CategoryId && x.FieldId == customFields.FieldId && x.Module == "Staff" && x.TargetId == staffCertificateInfoAddViewModel.staffCertificateInfo.StaffId);
                                    if (customFieldValueData != null)
                                    {
                                        this.context?.CustomFieldsValue.RemoveRange(customFieldValueData);
                                    }
                                    if (customFields.CustomFieldsValue != null && customFields.CustomFieldsValue.ToList().Count > 0)
                                    {
                                        customFields.CustomFieldsValue.FirstOrDefault()!.Module = "Staff";
                                        customFields.CustomFieldsValue.FirstOrDefault()!.CategoryId = customFields.CategoryId;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.FieldId = customFields.FieldId;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldTitle = customFields.Title;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldType = customFields.Type;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.SchoolId = (int)staffCertificateInfoAddViewModel.staffCertificateInfo.SchoolId!;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.TenantId = (Guid)staffCertificateInfoAddViewModel.staffCertificateInfo.TenantId!;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.TargetId = (int)staffCertificateInfoAddViewModel.staffCertificateInfo.StaffId!;
                                        this.context?.CustomFieldsValue.AddRange(customFields.CustomFieldsValue);
                                        this.context?.SaveChanges();
                                    }
                                }
                            }
                        }
                        transaction?.Commit();
                        staffCertificateInfoAddViewModel._failure = false;
                        staffCertificateInfoAddViewModel._message = "Staff certificate updated successfully";
                    }
                    else
                    {
                        staffCertificateInfoAddViewModel.staffCertificateInfo = null;
                        staffCertificateInfoAddViewModel._failure = true;
                        staffCertificateInfoAddViewModel._message = NORECORDFOUND;
                    }
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    staffCertificateInfoAddViewModel._failure = true;
                    staffCertificateInfoAddViewModel._message = es.Message;
                }
            }
            return staffCertificateInfoAddViewModel;
        }
        
        /// <summary>
        /// Delete Staff Certificate Info
        /// </summary>
        /// <param name="staffCertificateInfoAddViewModel"></param>
        /// <returns></returns>
        public StaffCertificateInfoAddViewModel DeleteStaffCertificateInfo(StaffCertificateInfoAddViewModel staffCertificateInfoAddViewModel)
        {
            try
            {
                if (staffCertificateInfoAddViewModel.staffCertificateInfo != null)
                {
                    var staffCertificateInfoDelete = this.context?.StaffCertificateInfo.FirstOrDefault(x => x.TenantId == staffCertificateInfoAddViewModel.staffCertificateInfo.TenantId && x.SchoolId == staffCertificateInfoAddViewModel.staffCertificateInfo.SchoolId && x.StaffId == staffCertificateInfoAddViewModel.staffCertificateInfo.StaffId && x.Id == staffCertificateInfoAddViewModel.staffCertificateInfo.Id);
                    if (staffCertificateInfoDelete != null)
                    {
                        this.context?.StaffCertificateInfo.Remove(staffCertificateInfoDelete);
                        this.context?.SaveChanges();
                        staffCertificateInfoAddViewModel._failure = false;
                        staffCertificateInfoAddViewModel._message = "Staff certificate deleted successfullyy";
                    }
                    else
                    {
                        staffCertificateInfoAddViewModel.staffCertificateInfo = null;
                        staffCertificateInfoAddViewModel._failure = true;
                        staffCertificateInfoAddViewModel._message = NORECORDFOUND;
                    }
                }
            }
            catch (Exception es)
            {
                staffCertificateInfoAddViewModel._failure = true;
                staffCertificateInfoAddViewModel._message = es.Message;
            }
            return staffCertificateInfoAddViewModel;
        }

        /// <summary>
        /// Add or Update Staff Photo
        /// </summary>
        /// <param name="staffAddViewModel"></param>
        /// <returns></returns>
        public StaffAddViewModel AddUpdateStaffPhoto(StaffAddViewModel staffAddViewModel)
        {
            try
            {
                if (staffAddViewModel.staffMaster != null)
                {
                    var staffUpdate = this.context?.StaffMaster.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.StaffId == staffAddViewModel.staffMaster.StaffId);
                    if (staffUpdate != null)
                    {
                        staffUpdate.UpdatedOn = DateTime.UtcNow;
                        staffUpdate.StaffPhoto = staffAddViewModel.staffMaster.StaffPhoto;
                        staffUpdate.StaffThumbnailPhoto = staffAddViewModel.staffMaster.StaffThumbnailPhoto;
                        staffUpdate.UpdatedBy = staffAddViewModel.staffMaster.UpdatedBy;
                        this.context?.SaveChanges();
                        staffAddViewModel._message = "Staff photo updated successfully";
                    }
                    else
                    {
                        staffAddViewModel._failure = true;
                        staffAddViewModel._message = NORECORDFOUND;
                    }
                }
            }
            catch (Exception es)
            {
                staffAddViewModel._failure = true;
                staffAddViewModel._message = es.Message;
            }
            return staffAddViewModel;
        }

        /// <summary>
        /// Add Staff List
        /// </summary>
        /// <param name="staffListAddViewModel"></param>
        /// <returns></returns>
        public StaffListAddViewModel AddStaffList(StaffListAddViewModel staffListAddViewModel)
        {
            StaffListAddViewModel staffListAdd = new StaffListAddViewModel();
            staffListAdd._tenantName = staffListAddViewModel._tenantName;
            staffListAdd._token = staffListAddViewModel._token;
            staffListAdd._userName = staffListAddViewModel._userName;
            int number;
            if (staffListAddViewModel.StaffAddViewModelList.Count > 0)
            {
                staffListAdd._failure = false;
                staffListAdd._message = "Staff added successfully";

                //int? staffId = Utility.GetMaxPK(this.context, new Func<StaffMaster, int>(x => x.StaffId));
                int? staffId = 1;

                var dataExits = this.context?.StaffMaster.Where(x => x.TenantId == staffListAddViewModel.TenantId).Count();

                if (dataExits > 0)
                {
                    var staffData = this.context?.StaffMaster.Where(x => x.TenantId == staffListAddViewModel.TenantId).Max(x => x.StaffId);

                    if (staffData != null)
                    {
                        staffId = staffData + 1;
                    }
                }             
               
                int? indexNo = -1;

                foreach (var staff in staffListAddViewModel.StaffAddViewModelList)
                {
                    indexNo++;

                     //UserMaster userMaster = new UserMaster();
                    var StaffSchoolInfoData = new StaffSchoolInfo();
                    
                    using (var transaction = this.context?.Database.BeginTransaction())
                    {
                        try
                        {
                            if (staff.staffMaster != null)
                            {
                                staff.staffMaster.TenantId = staffListAddViewModel.TenantId;
                                staff.staffMaster.SchoolId = staffListAddViewModel.SchoolId;
                                staff.staffMaster.StaffId = (int)staffId;
                                staff.staffMaster.CreatedOn = DateTime.UtcNow;
                                staff.staffMaster.CreatedBy = staffListAddViewModel.CreatedBy;
                                staff.staffMaster.IsActive = true;
                                Guid GuidId = Guid.NewGuid();
                                var GuidIdExist = this.context?.StaffMaster.FirstOrDefault(x => x.StaffGuid == GuidId);

                                if (GuidIdExist != null)
                                {
                                    staffListAdd.ConflictIndexNo = staffListAdd.ConflictIndexNo != null ? staffListAdd.ConflictIndexNo + "," + indexNo.ToString() : indexNo.ToString();
                                    staff._message = "GUID already exist";
                                    staffListAdd.StaffAddViewModelList.Add(staff);
                                    staffListAdd._failure = true;
                                    staffListAdd._message = "Staff rejected due to data error";
                                    continue;
                                }

                                staff.staffMaster.StaffGuid = GuidId;

                                if (!string.IsNullOrEmpty(staff.staffMaster.StaffInternalId))
                                {
                                    bool checkInternalID = CheckInternalID(staff.staffMaster.TenantId, staff.staffMaster.StaffInternalId);
                                    if (checkInternalID == false)
                                    {
                                        staffListAdd.ConflictIndexNo = staffListAdd.ConflictIndexNo != null ? staffListAdd.ConflictIndexNo + "," + indexNo.ToString() : indexNo.ToString();
                                        staff._message = "Staff id already exist";
                                        staffListAdd.StaffAddViewModelList.Add(staff);
                                        staffListAdd._failure = true;
                                        staffListAdd._message = "Staff rejected due to data error";
                                        continue;
                                    }
                                }
                                else
                                {
                                    staff.staffMaster.StaffInternalId = staff.staffMaster.StaffId.ToString();
                                }

                                if (staff.FirstLanguageName != null)
                                {
                                    var checkItsId = Int32.TryParse(staff.FirstLanguageName, out number);
                                    if (checkItsId)
                                    {
                                        var firstLanguageData = this.context?.Language.FirstOrDefault(x => x.LangId == Convert.ToInt32(staff.FirstLanguageName));
                                        staff.staffMaster.FirstLanguage = firstLanguageData != null ? Convert.ToInt32(staff.FirstLanguageName) : (int?)null;
                                    }
                                    else
                                    {
                                        var firstLanguageId = this.context?.Language.FirstOrDefault(x => x.Locale.ToLower() == staff.FirstLanguageName.ToLower())?.LangId;
                                        staff.staffMaster.FirstLanguage = firstLanguageId != null ? firstLanguageId : null;
                                    }
                                }

                                if (staff.SecondLanguageName != null)
                                {
                                    var checkItsId = Int32.TryParse(staff.SecondLanguageName, out number);
                                    if (checkItsId)
                                    {
                                        var SecondLanguageData = this.context?.Language.FirstOrDefault(x => x.LangId == Convert.ToInt32(staff.SecondLanguageName));
                                        staff.staffMaster.SecondLanguage = SecondLanguageData != null ? Convert.ToInt32(staff.SecondLanguageName) : (int?)null;
                                    }
                                    else
                                    {
                                        var secondLanguageId = this.context?.Language.FirstOrDefault(x => x.Locale.ToLower() == staff.SecondLanguageName.ToLower())?.LangId;
                                        staff.staffMaster.SecondLanguage = secondLanguageId != null ? secondLanguageId : null;
                                    }
                                }

                                if (staff.ThirdLanguageName != null)
                                {
                                    var checkItsId = Int32.TryParse(staff.ThirdLanguageName, out number);
                                    if (checkItsId)
                                    {
                                        var ThirdLanguageData = this.context?.Language.FirstOrDefault(x => x.LangId == Convert.ToInt32(staff.ThirdLanguageName));
                                        staff.staffMaster.ThirdLanguage = ThirdLanguageData != null ? Convert.ToInt32(staff.ThirdLanguageName) : (int?)null;
                                    }
                                    else
                                    {
                                        var thirdLanguageId = this.context?.Language.FirstOrDefault(x => x.Locale.ToLower() == staff.ThirdLanguageName.ToLower())?.LangId;
                                        staff.staffMaster.ThirdLanguage = thirdLanguageId != null ? thirdLanguageId : null;
                                    }
                                }

                                if (staff.CountryOfBirthName != null)
                                {
                                    var checkItsId = Int32.TryParse(staff.CountryOfBirthName, out number);
                                    if (checkItsId)
                                    {
                                        var CountryOfBirthData = this.context?.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(staff.CountryOfBirthName));
                                        staff.staffMaster.CountryOfBirth = CountryOfBirthData != null ? Convert.ToInt32(staff.CountryOfBirthName) : (int?)null;
                                    }
                                    else
                                    {
                                        var countryOfBirthId = this.context?.Country.FirstOrDefault(x => x.Name.ToLower() == staff.CountryOfBirthName.ToLower())?.Id;
                                        staff.staffMaster.CountryOfBirth = countryOfBirthId != null ? countryOfBirthId : null;
                                    }
                                }

                                if (staff.NationalityName != null)
                                {
                                    var checkItsId = Int32.TryParse(staff.NationalityName, out number);
                                    if (checkItsId)
                                    {
                                        var NationalityData = this.context?.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(staff.NationalityName));
                                        staff.staffMaster.Nationality = NationalityData != null ? Convert.ToInt32(staff.NationalityName) : (int?)null;
                                    }
                                    else
                                    {
                                        var nationalityId = this.context?.Country.FirstOrDefault(x => x.Name.ToLower() == staff.NationalityName.ToLower())?.Id;
                                        staff.staffMaster.Nationality = nationalityId != null ? nationalityId : null;
                                    }
                                }


                                if (staff.staffMaster.HomeAddressCountry != null)
                                {
                                    var checkItsId = Int32.TryParse(staff.staffMaster.HomeAddressCountry, out number);
                                    if (checkItsId)
                                    {
                                        var CountryData = this.context?.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(staff.staffMaster.HomeAddressCountry));
                                        staff.staffMaster.HomeAddressCountry = CountryData?.Id.ToString();
                                    }
                                    else
                                    {
                                        var CountryData = this.context?.Country.FirstOrDefault(x => x.Name.ToLower() == staff.staffMaster.HomeAddressCountry.ToLower());
                                        staff.staffMaster.HomeAddressCountry = CountryData?.Id.ToString();
                                    }
                                }

                                if (staff.staffMaster.MailingAddressCountry != null)
                                {
                                    var checkItsId = Int32.TryParse(staff.staffMaster.MailingAddressCountry, out number);
                                    if (checkItsId)
                                    {
                                        var CountryData = this.context?.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(staff.staffMaster.MailingAddressCountry));
                                        staff.staffMaster.MailingAddressCountry = CountryData != null ? CountryData.Id.ToString() : null;
                                    }
                                    else
                                    {
                                        var CountryData = this.context?.Country.FirstOrDefault(x => x.Name.ToLower() == staff.staffMaster.MailingAddressCountry.ToLower());
                                        staff.staffMaster.MailingAddressCountry = CountryData != null ? CountryData.Id.ToString() : null;
                                    }
                                }


                                //Add Staff Portal Access
                                if (!string.IsNullOrWhiteSpace(staff.PasswordHash) && !string.IsNullOrWhiteSpace(staff.LoginEmail))
                                {
                                    UserMaster userMaster = new UserMaster();

                                    //var decrypted = Utility.Decrypt(staff.PasswordHash);
                                    string passwordHash = Utility.GetHashedPassword(staff.PasswordHash);

                                    var loginInfo = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == staff.staffMaster.TenantId && x.EmailAddress == staff.LoginEmail);

                                    if (loginInfo == null)
                                    {
                                        var membership = this.context?.Membership.FirstOrDefault(x => x.TenantId == staff.staffMaster.TenantId && x.SchoolId == staff.staffMaster.SchoolId && x.Profile == "Teacher");

                                        userMaster.SchoolId = staff.staffMaster.SchoolId;
                                        userMaster.TenantId = staff.staffMaster.TenantId;
                                        userMaster.UserId = staff.staffMaster.StaffId;
                                        userMaster.LangId = 1;
                                        //userMaster.MembershipId = membership.MembershipId;
                                        userMaster.MembershipId = membership != null ? membership.MembershipId : 0;
                                        userMaster.EmailAddress = staff.LoginEmail;
                                        userMaster.PasswordHash = passwordHash;
                                        //userMaster.Name = staff.staffMaster.FirstGivenName;
                                        userMaster.Name = staff.staffMaster.FirstGivenName!=null? staff.staffMaster.FirstGivenName:"" ;
                                        userMaster.IsActive = true;

                                        this.context?.UserMaster.Add(userMaster);
                                        this.context?.SaveChanges();
                                        staff.staffMaster.PortalAccess = true;
                                        staff.staffMaster.LoginEmailAddress = staff.LoginEmail;
                                    }
                                    else
                                    {
                                        staffListAdd.ConflictIndexNo = staffListAdd.ConflictIndexNo != null ? staffListAdd.ConflictIndexNo + "," + indexNo.ToString() : indexNo.ToString();
                                        staff._message = "Login email already exist";
                                        staffListAdd.StaffAddViewModelList.Add(staff);
                                        staffListAdd._failure = true;
                                        staffListAdd._message = "Staff rejected due to data error";
                                        continue;
                                    }
                                }

                                if (staff.staffMaster.FirstGivenName == null || staff.staffMaster.LastFamilyName == null)
                                {
                                    staffListAdd.ConflictIndexNo = staffListAdd.ConflictIndexNo != null ? staffListAdd.ConflictIndexNo + "," + indexNo.ToString() : indexNo.ToString();
                                    staff._message = "FirstName or LastName is not provided";
                                    staffListAdd.StaffAddViewModelList.Add(staff);
                                    staffListAdd._failure = true;
                                    staffListAdd._message = "Staff rejected due to data error";
                                    continue;
                                }

                                if (staff.Dob != null)
                                {
                                    staff.staffMaster.Dob = Convert.ToDateTime(staff.Dob);
                                }
                                if (staff.JoiningDate != null)
                                {
                                    staff.staffMaster.JoiningDate = Convert.ToDateTime(staff.JoiningDate);
                                }

                                this.context?.StaffMaster.Add(staff.staffMaster);
                                this.context?.SaveChanges();

                                //Insert data into StaffSchoolInfo table            
                                int? Id = Utility.GetMaxPK(this.context, new Func<StaffSchoolInfo, int>(x => (int)x.Id));
                                var schoolName = this.context?.SchoolMaster.Where(x => x.TenantId == staff.staffMaster.TenantId && x.SchoolId == staff.staffMaster.SchoolId).Select(s => s.SchoolName).FirstOrDefault();

                                var startDate = DateTime.UtcNow;
                                if (staff.StartDate != null)
                                {
                                    startDate = Convert.ToDateTime(staff.StartDate);
                                }

                                StaffSchoolInfoData = new StaffSchoolInfo() { TenantId = staff.staffMaster.TenantId, SchoolId = staff.staffMaster.SchoolId, StaffId = staff.staffMaster.StaffId, SchoolAttachedId = staff.staffMaster.SchoolId, Id = (int)Id!, SchoolAttachedName = schoolName, StartDate = startDate, CreatedOn = DateTime.UtcNow, CreatedBy = staff.staffMaster.CreatedBy, Profile = staff.Profile ?? "Teacher" };
                                this.context?.StaffSchoolInfo.Add(StaffSchoolInfoData);
                                this.context?.SaveChanges();

                                if (staff.fieldsCategoryList != null && staff.fieldsCategoryList.ToList().Count > 0)
                                {
                                    //var fieldsCategory = staff.fieldsCategoryList.FirstOrDefault(x => x.CategoryId == staff.SelectedCategoryId);
                                    //if (fieldsCategory != null)
                                    //{
                                    foreach (var fieldsCategory in staff.fieldsCategoryList.ToList())
                                    {
                                        foreach (var customFields in fieldsCategory.CustomFields.ToList())
                                        {
                                            if (customFields.CustomFieldsValue != null && customFields.CustomFieldsValue.ToList().Count > 0)
                                            {
                                                customFields.CustomFieldsValue.FirstOrDefault()!.Module = "Staff";
                                                customFields.CustomFieldsValue.FirstOrDefault()!.CategoryId = customFields.CategoryId;
                                                customFields.CustomFieldsValue.FirstOrDefault()!.TenantId = staffListAddViewModel.TenantId;
                                                customFields.CustomFieldsValue.FirstOrDefault()!.FieldId = customFields.FieldId;
                                                customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldTitle = customFields.Title;
                                                customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldType = customFields.Type;
                                                customFields.CustomFieldsValue.FirstOrDefault()!.SchoolId = staff.staffMaster.SchoolId;
                                                customFields.CustomFieldsValue.FirstOrDefault()!.TargetId = staff.staffMaster.StaffId;
                                                this.context?.CustomFieldsValue.AddRange(customFields.CustomFieldsValue);
                                                this.context?.SaveChanges();
                                            }
                                        }
                                    }
                                    //}
                                }
                                transaction?.Commit();
                                staffId++;
                            }
                        }
                        catch (Exception es)
                        {
                            transaction?.Rollback();
                            staffListAdd.StaffAddViewModelList.Add(staff);
                            staffListAdd.ConflictIndexNo = staffListAdd.ConflictIndexNo != null ? staffListAdd.ConflictIndexNo + "," + indexNo.ToString() : indexNo.ToString();
                            this.context?.StaffMaster.Remove(staff.staffMaster!);
                            this.context?.StaffSchoolInfo.Remove(StaffSchoolInfoData);
                            //this.context?.UserMaster.Remove(userMaster);
                            staffListAdd._failure = true;
                            staffListAdd._message = es.Message;

                            //staffId--;
                            continue;
                        }
                    }
                }
            }
            else
            {
                staffListAdd._failure = true;
                staffListAdd._message = "Please Import Staff";
            }
            return staffListAdd;
        }

        /// <summary>
        /// Get Scheduled Course Sections For Staff
        /// </summary>
        /// <param name="scheduledCourseSectionViewModel"></param>
        /// <returns></returns>
        public ScheduledCourseSectionViewModel GetScheduledCourseSectionsForStaff(ScheduledCourseSectionViewModel scheduledCourseSectionViewModel)
        {
            ScheduledCourseSectionViewModel scheduledCourseSectionView = new ScheduledCourseSectionViewModel();
            try
            {
                scheduledCourseSectionView.TenantId = scheduledCourseSectionViewModel.TenantId;
                scheduledCourseSectionView._tenantName = scheduledCourseSectionViewModel._tenantName;
                scheduledCourseSectionView.SchoolId = scheduledCourseSectionViewModel.SchoolId;
                scheduledCourseSectionView.StaffId = scheduledCourseSectionViewModel.StaffId;
                scheduledCourseSectionView._token = scheduledCourseSectionViewModel._token;
                scheduledCourseSectionView._userName = scheduledCourseSectionViewModel._userName;

                var scheduledCourseSectionDataList = this.context?.StaffCoursesectionSchedule.Include(x => x.CourseSection).ThenInclude(y => y.SchoolYears).ThenInclude(s => s!.Semesters).ThenInclude(e => e.Quarters).ThenInclude(f => f.ProgressPeriods).Include(n=>n.CourseSection.SchoolCalendars).Include(b=>b.CourseSection.Course).Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.StaffId == scheduledCourseSectionViewModel.StaffId && x.IsDropped != true).ToList();

                if (scheduledCourseSectionDataList!=null && scheduledCourseSectionDataList.Any())
                {
                    foreach (var scheduledCourseSectionData in scheduledCourseSectionDataList)
                    {
                        if (scheduledCourseSectionData.CourseSection.AcademicYear == scheduledCourseSectionViewModel.AcademicYear)
                        {
                            CourseSectionViewList CourseSections = new CourseSectionViewList();

                            if (scheduledCourseSectionData.CourseSection.ScheduleType == "Fixed Schedule (1)")
                            {
                                CourseSections.ScheduleType = "Fixed Schedule";

                                var courseFixedScheduleData = this.context?.CourseFixedSchedule.Include(c => c.BlockPeriod).Include(b => b.Rooms).FirstOrDefault(x => x.TenantId == scheduledCourseSectionData.TenantId && x.SchoolId == scheduledCourseSectionData.SchoolId && x.CourseSectionId == scheduledCourseSectionData.CourseSectionId);
                                if (courseFixedScheduleData != null)
                                {
                                    if (courseFixedScheduleData.BlockPeriod != null)
                                    {
                                        courseFixedScheduleData.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                        courseFixedScheduleData.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                        courseFixedScheduleData.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                        courseFixedScheduleData.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                    }
                                    if (courseFixedScheduleData.Rooms != null)
                                    {
                                        courseFixedScheduleData.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                        courseFixedScheduleData.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                        courseFixedScheduleData.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                        courseFixedScheduleData.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                    }
                                    CourseSections.courseFixedSchedule = courseFixedScheduleData;

                                }
                            }
                            if (scheduledCourseSectionData.CourseSection.ScheduleType == "Variable Schedule (2)")
                            {
                                CourseSections.ScheduleType = "Variable Schedule";

                                var courseVariableScheduleData = this.context?.CourseVariableSchedule.Include(c => c.BlockPeriod).Include(b => b.Rooms).Where(x => x.TenantId == scheduledCourseSectionData.TenantId && x.SchoolId == scheduledCourseSectionData.SchoolId && x.CourseSectionId == scheduledCourseSectionData.CourseSectionId).ToList();

                                if (courseVariableScheduleData != null && courseVariableScheduleData.Any())
                                {
                                    courseVariableScheduleData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });

                                    CourseSections.courseVariableSchedule = courseVariableScheduleData;
                                }
                            }
                            if (scheduledCourseSectionData.CourseSection.ScheduleType == "Calendar Schedule (3)")
                            {
                                CourseSections.ScheduleType = "Calendar Schedule";

                                var courseCalenderScheduleData = this.context?.CourseCalendarSchedule.Include(c => c.BlockPeriod).Include(b => b.Rooms).Where(x => x.TenantId == scheduledCourseSectionData.TenantId && x.SchoolId == scheduledCourseSectionData.SchoolId && x.CourseSectionId == scheduledCourseSectionData.CourseSectionId).ToList();

                                if (courseCalenderScheduleData != null && courseCalenderScheduleData.Any())
                                {
                                    courseCalenderScheduleData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });

                                    CourseSections.courseCalendarSchedule = courseCalenderScheduleData;
                                }
                            }
                            if (scheduledCourseSectionData.CourseSection.ScheduleType == "Block Schedule (4)")
                            {
                                CourseSections.ScheduleType = "Block Schedule";

                                var courseBlockScheduleData = this.context?.CourseBlockSchedule.Include(c => c.BlockPeriod).Include(b => b.Rooms).Where(x => x.TenantId == scheduledCourseSectionData.TenantId && x.SchoolId == scheduledCourseSectionData.SchoolId && x.CourseSectionId == scheduledCourseSectionData.CourseSectionId).ToList();

                                if (courseBlockScheduleData != null && courseBlockScheduleData.Any())
                                {
                                    courseBlockScheduleData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });

                                    CourseSections.courseBlockSchedule = courseBlockScheduleData;

                                    var bellScheduleList = new List<BellSchedule>();
                                    foreach (var block in courseBlockScheduleData)
                                    {
                                        var bellScheduleData = this.context?.BellSchedule.Where(c => c.SchoolId == scheduledCourseSectionData.SchoolId && c.TenantId == scheduledCourseSectionData.TenantId && c.BlockId == block.BlockId && c.BellScheduleDate >= scheduledCourseSectionData.CourseSection.DurationStartDate && c.BellScheduleDate <= scheduledCourseSectionData.CourseSection.DurationEndDate).ToList();
                                        if (bellScheduleData?.Any() == true)
                                        {
                                            bellScheduleList.AddRange(bellScheduleData);
                                        }

                                    }

                                    CourseSections.bellScheduleList = bellScheduleList;
                                }
                            }

                            CourseSections.CourseId = scheduledCourseSectionData.CourseId;
                            CourseSections.CourseSectionId = scheduledCourseSectionData.CourseSectionId;
                            CourseSections.CalendarId = scheduledCourseSectionData.CourseSection.CalendarId;
                            CourseSections.CourseSectionName = scheduledCourseSectionData.CourseSectionName;
                            CourseSections.CourseTitle = scheduledCourseSectionData.CourseSection.Course.CourseTitle;
                            CourseSections.CourseGradeLevel = scheduledCourseSectionData.CourseSection.Course.CourseGradeLevel;
                            CourseSections.YrMarkingPeriodId = scheduledCourseSectionData.YrMarkingPeriodId;
                            CourseSections.SmstrMarkingPeriodId = scheduledCourseSectionData.SmstrMarkingPeriodId;
                            CourseSections.QtrMarkingPeriodId = scheduledCourseSectionData.QtrMarkingPeriodId;
                            CourseSections.PrgrsprdMarkingPeriodId = scheduledCourseSectionData.PrgrsprdMarkingPeriodId;
                            CourseSections.DurationStartDate = scheduledCourseSectionData.DurationStartDate;
                            CourseSections.DurationEndDate = scheduledCourseSectionData.DurationEndDate;
                            CourseSections.MeetingDays = scheduledCourseSectionData.MeetingDays;
                            CourseSections.AttendanceCategoryId = scheduledCourseSectionData.CourseSection.AttendanceCategoryId;
                            CourseSections.AttendanceTaken = scheduledCourseSectionData.CourseSection.AttendanceTaken;
                            CourseSections.WeekDays = scheduledCourseSectionData.CourseSection.SchoolCalendars?.Days;
                            CourseSections.SchoolYears = scheduledCourseSectionData.CourseSection.SchoolYears;
                            CourseSections.Quarters = scheduledCourseSectionData.CourseSection.Quarters;
                            CourseSections.Semesters = scheduledCourseSectionData.CourseSection.Semesters;
                            CourseSections.DurationBasedOnPeriod = scheduledCourseSectionData.CourseSection.DurationBasedOnPeriod;

                            if (CourseSections != null)
                            {
                                if (CourseSections.SchoolYears != null)
                                {
                                    CourseSections.SchoolYears.CourseSection = new HashSet<CourseSection>();
                                    //CourseSections.SchoolYears.HonorRolls = new HashSet<HonorRolls>();
                                    CourseSections.SchoolYears.Semesters = new HashSet<Semesters>();
                                    CourseSections.SchoolYears.StaffCoursesectionSchedule = new HashSet<StaffCoursesectionSchedule>();
                                    CourseSections.SchoolYears.StudentEffortGradeMaster = new HashSet<StudentEffortGradeMaster>();
                                    CourseSections.SchoolYears.StudentFinalGrade = new HashSet<StudentFinalGrade>();
                                }
                                if (CourseSections.Quarters != null)
                                {
                                    CourseSections.Quarters.CourseSection = new HashSet<CourseSection>();
                                    CourseSections.Quarters.Semesters = null;
                                    CourseSections.Quarters.ProgressPeriods = new HashSet<ProgressPeriods>();
                                    CourseSections.Quarters.StaffCoursesectionSchedule = new HashSet<StaffCoursesectionSchedule>();
                                    CourseSections.Quarters.StudentEffortGradeMaster = new HashSet<StudentEffortGradeMaster>();
                                    CourseSections.Quarters.StudentFinalGrade = new HashSet<StudentFinalGrade>();
                                    CourseSections.Quarters.ProgressPeriods = new HashSet<ProgressPeriods>();
                                }
                                if (CourseSections.Semesters != null)
                                {
                                    CourseSections.Semesters.CourseSection = new HashSet<CourseSection>();
                                    CourseSections.Semesters.Quarters = new HashSet<Quarters>();
                                    CourseSections.Semesters.StaffCoursesectionSchedule = new HashSet<StaffCoursesectionSchedule>();
                                    CourseSections.Semesters.StudentEffortGradeMaster = new HashSet<StudentEffortGradeMaster>();
                                    CourseSections.Semesters.StudentFinalGrade = new HashSet<StudentFinalGrade>();
                                    CourseSections.Semesters.SchoolYears = null;
                                }
                                if (CourseSections.ProgressPeriods != null)
                                {
                                    CourseSections.ProgressPeriods.CourseSections = new HashSet<CourseSection>();
                                    CourseSections.ProgressPeriods.Quarters = new Quarters();
                                    CourseSections.ProgressPeriods.StaffCoursesectionSchedules = new HashSet<StaffCoursesectionSchedule>();
                                    CourseSections.ProgressPeriods.StudentEffortGradeMasters = new HashSet<StudentEffortGradeMaster>();
                                    CourseSections.ProgressPeriods.StudentFinalGrades = new HashSet<StudentFinalGrade>();
                                }
                            }
                            if (CourseSections != null)
                            {
                                scheduledCourseSectionView.courseSectionViewList.Add(CourseSections);
                                scheduledCourseSectionView._failure = false;
                            }

                        }
                    }
                }
            }
            catch (Exception es)
            {
                scheduledCourseSectionView._failure = true;
                scheduledCourseSectionView._message = es.Message;
            }
            return scheduledCourseSectionView;
        }

        /// <summary>
        /// GetAllStaffListByDateRange
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public StaffListModel GetAllStaffListByDateRange(PageResult pageResult)
        {
            StaffListModel staffListModel = new StaffListModel();
            IQueryable<StaffMaster>? transactionIQ = null;
            IQueryable<StaffMaster>? StaffMasterList = null;

            var membershipData = this.context?.UserMaster.Include(x => x.Membership).FirstOrDefault(x => x.TenantId == pageResult.TenantId && x.EmailAddress == pageResult.EmailAddress);

            var activeSchools = this.context?.SchoolDetail.Where(x => x.TenantId == pageResult.TenantId && x.Status == true).Select(x => x.SchoolId).ToList();

            if (membershipData != null)
            {
                if (String.Compare(membershipData.Membership.ProfileType, "super administrator", true) == 0)
                {
                    var schoolAttachedStaffId = this.context?.StaffSchoolInfo.Where(x => x.TenantId == pageResult.TenantId && (pageResult.SearchAllSchool == false || pageResult.SearchAllSchool == null ? x.SchoolAttachedId == pageResult.SchoolId : activeSchools!.Contains(x.SchoolAttachedId)) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date && ((x.StartDate >= pageResult.MarkingPeriodStartDate && x.StartDate <= pageResult.MarkingPeriodEndDate) || x.StartDate <= pageResult.MarkingPeriodStartDate) : /*((x.StartDate >= pageResult.MarkingPeriodStartDate && x.StartDate <= pageResult.MarkingPeriodEndDate) || x.StartDate <= pageResult.MarkingPeriodStartDate)*/ true)).Select(s => s.StaffId).ToList();

                    StaffMasterList = this.context?.StaffMaster.Include(x => x.StaffSchoolInfo).Where(x => x.TenantId == pageResult.TenantId && (x.Profile ?? "").ToLower() != "super administrator" && schoolAttachedStaffId!.Contains(x.StaffId) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.IsActive != false : true));

                }
                else
                {
                    var schoolAttachedId = this.context?.StaffSchoolInfo.Where(x => x.TenantId == pageResult.TenantId && x.StaffId == membershipData.UserId && x.EndDate == null && activeSchools!.Contains(x.SchoolAttachedId)).ToList().Select(s => s.SchoolAttachedId);

                    var schoolAttachedStaffId = this.context?.StaffSchoolInfo.Where(x => x.TenantId == pageResult.TenantId && (pageResult.SearchAllSchool == false || pageResult.SearchAllSchool == null ? x.SchoolAttachedId == pageResult.SchoolId : schoolAttachedId!.Contains(x.SchoolId)) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date && ((x.StartDate >= pageResult.MarkingPeriodStartDate && x.StartDate <= pageResult.MarkingPeriodEndDate) || x.StartDate <= pageResult.MarkingPeriodStartDate) : /*((x.StartDate >= pageResult.MarkingPeriodStartDate && x.StartDate <= pageResult.MarkingPeriodEndDate) || x.StartDate <= pageResult.MarkingPeriodStartDate)*/ true)).Select(s => s.StaffId).ToList();

                    StaffMasterList = this.context?.StaffMaster.Include(x => x.StaffSchoolInfo).Where(x => x.TenantId == pageResult.TenantId && (x.Profile ?? "").ToLower() != "super administrator" && schoolAttachedStaffId!.Contains(x.StaffId) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.IsActive != false : true));
                }
            }

            try
            {
                if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                {

                    transactionIQ = StaffMasterList;
                }
                else
                {
                    if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                    {
                        string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                        var Searchvalue = Columnvalue.Trim();
                        Columnvalue = Regex.Replace(Columnvalue, @"\s+", "");
                        transactionIQ = StaffMasterList?.Where(x => x.FirstGivenName != null && x.FirstGivenName.Contains(Columnvalue) ||
                                                                    x.MiddleName != null && x.MiddleName.Contains(Columnvalue) ||
                                                                    x.LastFamilyName != null && x.LastFamilyName.Contains(Columnvalue) ||
                                                                    (x.FirstGivenName + x.MiddleName + x.LastFamilyName).Contains(Columnvalue) || (x.FirstGivenName + x.MiddleName).Contains(Columnvalue) || (x.FirstGivenName + x.LastFamilyName).Contains(Columnvalue) || (x.MiddleName + x.LastFamilyName).Contains(Columnvalue) ||
                                                                    x.StaffInternalId != null && x.StaffInternalId.Contains(Searchvalue) ||
                                                                    x.JobTitle != null && x.JobTitle.Contains(Searchvalue) ||
                                                                    x.SchoolEmail != null && x.SchoolEmail.Contains(Searchvalue) ||
                                                                    x.MobilePhone != null && x.MobilePhone.Contains(Searchvalue) || (x.StaffSchoolInfo.Count > 0 ? (x.StaffSchoolInfo.Any(x => (x.Profile ?? "").ToLower().Contains(Searchvalue.ToLower()) && x.SchoolAttachedId == pageResult.SchoolId)) : string.Empty.Contains(Searchvalue)));

                    }
                    else
                    {
                        if (pageResult.FilterParams != null && pageResult.FilterParams.Any(x => x.ColumnName.ToLower() == "profile"))
                        {
                            var filterValue = pageResult.FilterParams.Where(x => x.ColumnName.ToLower() == "profile").Select(x => x.FilterValue).FirstOrDefault();
                            var profileFilterData = StaffMasterList?.Where(x => x.StaffSchoolInfo.Any(x => (x.Profile ?? "").ToLower() == (filterValue ?? "").ToLower() && x.SchoolAttachedId == pageResult.SchoolId));

                            if (profileFilterData?.Any() == true)
                            {
                                StaffMasterList = profileFilterData;
                                var indexValue = pageResult.FilterParams.FindIndex(x => x.ColumnName.ToLower() == "profile");
                                pageResult.FilterParams.RemoveAt(indexValue);
                            }
                        }

                        transactionIQ = Utility.FilteredData(pageResult.FilterParams!, StaffMasterList!).AsQueryable();
                    }
                }

                if (pageResult.DobStartDate != null && pageResult.DobEndDate != null)
                {
                    var filterInDateRange = transactionIQ?.Where(x => x.Dob >= pageResult.DobStartDate && x.Dob <= pageResult.DobEndDate).AsQueryable();

                    if (filterInDateRange?.Any() == true)
                    {
                        transactionIQ = filterInDateRange;
                    }
                }

                if (pageResult.FullName != null)
                {
                    var staffName = pageResult.FullName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    if (staffName.Length > 1)
                    {
                        var firstName = staffName.First();
                        var lastName = staffName.Last();

                        if (pageResult.FullName == null)
                        {
                            var nameSearch = transactionIQ?.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.FirstGivenName!.StartsWith(firstName.ToString()) && x.LastFamilyName!.StartsWith(lastName.ToString()));

                            transactionIQ = nameSearch;
                        }
                    }
                    else
                    {
                        var nameSearch = transactionIQ?.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && (x.FirstGivenName!.StartsWith(pageResult.FullName) || x.LastFamilyName!.StartsWith(pageResult.FullName)));

                        transactionIQ = nameSearch;
                    }
                }

                if (pageResult.SortingModel != null)
                {
                    switch (pageResult.SortingModel.SortColumn?.ToLower())
                    {
                        case "profile":

                            if (pageResult.SortingModel.SortDirection?.ToLower() == "asc")
                            {
                                transactionIQ = transactionIQ?.OrderBy(a => a.StaffSchoolInfo.Count > 0 ? a.StaffSchoolInfo.FirstOrDefault()!.Profile : null);
                            }
                            else
                            {
                                transactionIQ = transactionIQ?.OrderByDescending(a => a.StaffSchoolInfo.Count > 0 ? a.StaffSchoolInfo.FirstOrDefault()!.Profile : null);
                            }
                            break;
                        default:
                            transactionIQ = Utility.Sort(transactionIQ!, pageResult.SortingModel.SortColumn!, pageResult.SortingModel.SortDirection!.ToLower());
                            break;
                    }
                }
                else
                {
                    transactionIQ = transactionIQ?.OrderBy(e => e.LastFamilyName).ThenBy(c => c.FirstGivenName);
                }

                int totalCount = transactionIQ != null ? transactionIQ.Count() : 0;
                if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                {
                    transactionIQ = transactionIQ?.Select(e => new StaffMaster
                    {
                        TenantId = e.TenantId,
                        StaffId = e.StaffId,
                        SchoolId = e.SchoolId,
                        StaffInternalId = e.StaffInternalId,
                        StaffGuid = e.StaffGuid,
                        FirstGivenName = e.FirstGivenName,
                        MiddleName = e.MiddleName,
                        LastFamilyName = e.LastFamilyName,
                        Profile = e.StaffSchoolInfo.Count > 0 ? e.StaffSchoolInfo.Where(x => (pageResult.SearchAllSchool == true || x.SchoolAttachedId == pageResult.SchoolId) && (pageResult.IncludeInactive == true || x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date)).FirstOrDefault()!.Profile : e.Profile,
                        JobTitle = e.JobTitle,
                        SchoolEmail = e.SchoolEmail,
                        MobilePhone = e.MobilePhone,
                        HomeroomTeacher = e.HomeroomTeacher,
                        PrimaryGradeLevelTaught = e.PrimaryGradeLevelTaught,
                        OtherGradeLevelTaught = e.OtherGradeLevelTaught,
                        PrimarySubjectTaught = e.PrimarySubjectTaught,
                        OtherSubjectTaught = e.OtherSubjectTaught,
                        FirstLanguage = e.FirstLanguage,
                        SecondLanguage = e.SecondLanguage,
                        ThirdLanguage = e.ThirdLanguage,
                        StaffThumbnailPhoto = pageResult.ProfilePhoto != null ? e.StaffThumbnailPhoto : null,
                        CreatedBy = e.CreatedBy,
                        CreatedOn = e.CreatedOn,
                        UpdatedBy = e.UpdatedBy,
                        UpdatedOn = e.UpdatedOn,
                        IsActive = e.IsActive,
                        StaffSchoolInfo = e.StaffSchoolInfo.Count > 0 ? e.StaffSchoolInfo.Select(s => new StaffSchoolInfo { StartDate = s.StartDate, EndDate = s.EndDate, SchoolAttachedId = s.SchoolAttachedId, SchoolId = s.SchoolId, SchoolAttachedName = s.SchoolAttachedName }).ToList() : new()
                    }).Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                }
                else
                {
                    transactionIQ = transactionIQ?.Select(e => new StaffMaster
                    {
                        TenantId = e.TenantId,
                        StaffId = e.StaffId,
                        SchoolId = e.SchoolId,
                        StaffInternalId = e.StaffInternalId,
                        StaffGuid = e.StaffGuid,
                        FirstGivenName = e.FirstGivenName,
                        MiddleName = e.MiddleName,
                        LastFamilyName = e.LastFamilyName,
                        Profile = e.StaffSchoolInfo.Count > 0 ? e.StaffSchoolInfo.Where(x => (pageResult.SearchAllSchool == true || x.SchoolAttachedId == pageResult.SchoolId) && (pageResult.IncludeInactive == true || x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date)).FirstOrDefault()!.Profile : e.Profile,
                        JobTitle = e.JobTitle,
                        SchoolEmail = e.SchoolEmail,
                        MobilePhone = e.MobilePhone,
                        HomeroomTeacher = e.HomeroomTeacher,
                        PrimaryGradeLevelTaught = e.PrimaryGradeLevelTaught,
                        OtherGradeLevelTaught = e.OtherGradeLevelTaught,
                        PrimarySubjectTaught = e.PrimarySubjectTaught,
                        OtherSubjectTaught = e.OtherSubjectTaught,
                        FirstLanguage = e.FirstLanguage,
                        SecondLanguage = e.SecondLanguage,
                        ThirdLanguage = e.ThirdLanguage,
                        StaffThumbnailPhoto = pageResult.ProfilePhoto != null ? e.StaffThumbnailPhoto : null,
                        CreatedBy = e.CreatedBy,
                        CreatedOn = e.CreatedOn,
                        UpdatedBy = e.UpdatedBy,
                        UpdatedOn = e.UpdatedOn,
                        IsActive = e.IsActive,
                        StaffSchoolInfo = e.StaffSchoolInfo.Count > 0 ? e.StaffSchoolInfo.Select(s => new StaffSchoolInfo { StartDate = s.StartDate, EndDate = s.EndDate, SchoolAttachedId = s.SchoolAttachedId, SchoolId = s.SchoolId, SchoolAttachedName = s.SchoolAttachedName }).ToList() : new()
                    });
                }
                if (transactionIQ != null && transactionIQ.Any())
                {
                    staffListModel.staffMaster = transactionIQ.ToList();
                }

                staffListModel.staffMaster.ForEach(e =>
                {
                    e.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, e.CreatedBy);
                    e.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, e.UpdatedBy);
                });

                staffListModel.TenantId = pageResult.TenantId;
                staffListModel.TotalCount = totalCount;
                staffListModel.PageNumber = pageResult.PageNumber;
                staffListModel._pageSize = pageResult.PageSize;
                staffListModel._tenantName = pageResult._tenantName;
                staffListModel._token = pageResult._token;
                staffListModel._failure = false;
            }
            catch (Exception es)
            {
                staffListModel._message = es.Message;
                staffListModel._failure = true;
                staffListModel._tenantName = pageResult._tenantName;
                staffListModel._token = pageResult._token;
            }
            return staffListModel;
        }

        /// <summary>
        /// Delete Staff
        /// </summary>
        /// <param name="staffDeleteViewModel"></param>
        /// <returns></returns>
        public StaffDeleteViewModel DeleteStaff(StaffDeleteViewModel staffDeleteViewModel)
        {
            StaffDeleteViewModel staffDelete = new()
            {
                TenantId = staffDeleteViewModel.TenantId,
                SchoolId = staffDeleteViewModel.SchoolId,
                _userName = staffDeleteViewModel._userName,
                _tenantName = staffDeleteViewModel._tenantName,
                _token = staffDeleteViewModel._token
            };

            bool allSccuss = true;
            List<string> deletedStaffName = new();
            string? staffName = string.Empty;
            List<StaffMaster> staffMasters = new List<StaffMaster>();
            List<StaffCertificateInfo> staffCertificateInfos = new List<StaffCertificateInfo>();
            List<StaffSchoolInfo> staffSchoolInfos = new List<StaffSchoolInfo>();
            List<CustomFieldsValue> customFieldsValues = new List<CustomFieldsValue>();
            List<UserMaster> userMasters = new List<UserMaster>();
            List<StaffCoursesectionSchedule> staffCoursesectionSchedules = new List<StaffCoursesectionSchedule>();

            try
            {
                if (staffDeleteViewModel.staffMasters?.Any() == true)
                {
                    var staffIds = staffDeleteViewModel.staffMasters.Select(s => s.StaffId).ToList();

                    var staffMasterData = this.context?.StaffMaster.Include(s => s.StaffSchoolInfo).Include(s => s.StaffCertificateInfo).Include(s => s.StaffCoursesectionSchedule).Where(x => x.TenantId == staffDeleteViewModel.TenantId /*&& x.SchoolId == staffDeleteViewModel.SchoolId*/ && staffIds.Contains(x.StaffId)).ToList();

                    var customFieldsValueData = this.context?.CustomFieldsValue.Where(x => x.TenantId == staffDeleteViewModel.TenantId /*&& x.SchoolId == staffDeleteViewModel.SchoolId*/ && x.Module == "Staff" && staffIds.Contains(x.TargetId)).ToList();

                    var userMasterData = this.context?.UserMaster.Where(x => x.TenantId == staffDeleteViewModel.TenantId && x.SchoolId == staffDeleteViewModel.SchoolId).ToList();
                    var schoolMasterData = this.context?.SchoolMaster.Where(x => x.TenantId == staffDeleteViewModel.TenantId).Select(s => new { s.SchoolId, s.SchoolName }).ToList();

                    foreach (var staff in staffDeleteViewModel.staffMasters)
                    {
                        var staffData = staffMasterData?.FirstOrDefault(s => s.StaffId == staff.StaffId);
                        if (staffData != null)
                        {
                            var checkCourseSectionSchedule = staffData.StaffCoursesectionSchedule.Where(x => x.IsDropped != true).ToList();

                            if (/*staffData.StaffCoursesectionSchedule.Count == 0 &&*/ checkCourseSectionSchedule.Any() != true)
                            {
                                bool hasAssociation = false;
                                if (staffData.StaffCoursesectionSchedule.Count == 0)
                                {
                                    hasAssociation = false;
                                }
                                else
                                {
                                    var staffDroppedCS = staffData.StaffCoursesectionSchedule.Where(x => x.IsDropped == true).ToList();
                                    if (staffDroppedCS?.Any() == true)
                                    {
                                        var staffDroppedCSIds = staffDroppedCS.Select(s => s.CourseSectionId).ToList();

                                        var StudentAttendanceData = this.context?.StudentAttendance.Where(x => x.TenantId == staffDeleteViewModel.TenantId /*&& x.SchoolId == staffDeleteViewModel.SchoolId*/ && x.StaffId == staff.StaffId && staffDroppedCSIds.Contains(x.CourseSectionId)).FirstOrDefault();

                                        var AssignmentData = this.context?.Assignment.Where(x => x.TenantId == staffDeleteViewModel.TenantId /*&& x.SchoolId == staffDeleteViewModel.SchoolId*/ && x.StaffId == staff.StaffId && x.CourseSectionId != null && staffDroppedCSIds.Contains((int)x.CourseSectionId)).FirstOrDefault();

                                        if (StudentAttendanceData != null || AssignmentData != null)
                                        {
                                            hasAssociation = true;
                                            allSccuss = false;
                                            if (StudentAttendanceData != null)
                                            {
                                                var schoolName = schoolMasterData?.FirstOrDefault(s => s.SchoolId == StudentAttendanceData.SchoolId)?.SchoolName;
                                                staffDelete._message = "Staff record can not be deleted due to staff has attendance records in " + schoolName;
                                            }
                                            else if (AssignmentData != null)
                                            {
                                                var schoolName = schoolMasterData?.FirstOrDefault(s => s.SchoolId == AssignmentData.SchoolId)?.SchoolName;
                                                staffDelete._message = "Staff record can not be deleted due to staff has assignment records in " + schoolName;
                                            }

                                            staffDelete.staffMasters.Add(staff);
                                        }
                                        else
                                        {
                                            hasAssociation = false;
                                            staffCoursesectionSchedules.AddRange(staffDroppedCS);
                                        }
                                    }
                                    //else
                                    //{
                                    //    hasAssociation = true;
                                    //    allSccuss = false;
                                    //    staffDelete._message = "Staff record can not be deleted due to staff has active course section.";
                                    //    staffDelete.staffMasters.Add(staff);
                                    //}
                                }

                                if (!hasAssociation)
                                {
                                    staffMasters.Add(staffData);
                                    staffCertificateInfos.AddRange(staffData.StaffCertificateInfo);
                                    staffSchoolInfos.AddRange(staffData.StaffSchoolInfo);

                                    deletedStaffName.Add(staffData.FirstGivenName + " " + staffData.MiddleName + " " + staffData.LastFamilyName);


                                    var UserMaster = userMasterData?.FirstOrDefault(x => x.TenantId == staffDeleteViewModel.TenantId /*&& x.SchoolId == staffDeleteViewModel.SchoolId*/ && x.EmailAddress == staffData.LoginEmailAddress);
                                    if (UserMaster != null)
                                    {
                                        userMasters.Add(UserMaster);
                                    }

                                    var customFieldsValue = customFieldsValueData?.Where(s => s.TargetId == staff.StaffId).ToList();
                                    if (customFieldsValue?.Any() == true)
                                    {
                                        customFieldsValues.AddRange(customFieldsValue);
                                    }
                                }
                            }
                            else
                            {
                                allSccuss = false;
                                staffDelete.staffMasters.Add(staff);
                                if (checkCourseSectionSchedule != null && checkCourseSectionSchedule.Any())
                                {
                                    var schoolName = schoolMasterData?.FirstOrDefault(s => s.SchoolId == checkCourseSectionSchedule?.FirstOrDefault()?.SchoolId)?.SchoolName;
                                    staffDelete._message = "Staff record can not be deleted due to staff has course section " + checkCourseSectionSchedule?.FirstOrDefault()?.CourseSectionName + "  asociation in " + schoolName;
                                }
                            }
                        }
                        else
                        {
                            allSccuss = false;
                            staffDelete._message = "Staff record can not be deleted due to this school not a staff's home school.";
                            staffDelete.staffMasters.Add(staff);
                        }
                    }

                    if (allSccuss)
                    {
                        this.context?.UserMaster.RemoveRange(userMasters);
                        this.context?.CustomFieldsValue.RemoveRange(customFieldsValues);
                        this.context?.StaffCertificateInfo.RemoveRange(staffCertificateInfos);
                        this.context?.StaffCoursesectionSchedule.RemoveRange(staffCoursesectionSchedules);
                        this.context?.StaffSchoolInfo.RemoveRange(staffSchoolInfos);
                        this.context?.StaffMaster.RemoveRange(staffMasters);
                        this.context?.SaveChanges();

                        staffDelete._message = "Staff deleted successfully";
                    }
                }
                else
                {
                    staffDelete._message = "Please select staff";
                    staffDelete._failure = false;
                }
            }
            catch (Exception es)
            {
                staffDelete._failure = true;
                staffDelete._message = es.Message;
            }
            return staffDelete;
        }
    }
}

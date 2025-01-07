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

using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.IO;
using opensis.data.Helper;
using opensis.data.ViewModels.RoleBasedAccess;
using Newtonsoft.Json;
using opensis.catalogdb.Interface;
using opensis.catalogdb.Models;

namespace opensis.data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly CRMContext? context;
        private CatalogDBContext catdbContext;
        private static readonly string NORECORDFOUND = "No Record Found";
        private static string EMAILMESSAGE = "Email address is not registered in the system";
        private static string PASSWORDMESSAGE = "Login failed. Email and password combination is incorrect. Try again.";
        private static string INCORRECTLOGINATTEMPTMESSAGE = "Due to excessive incorrect login attempts your account has been deactivated. Please contact the school administration to reactivate your account";
        private static string INACTIVITYDAYSMESSAGE = "Due to your long absence from the system, as a measure of security your account has been deactivated. Please contact the school administration to reactivate your account";
        public UserRepository(IDbContextFactory dbContextFactory, ICatalogDBContextFactory catdbContextFactory)
        {
            this.context = dbContextFactory.Create();
            this.catdbContext = catdbContextFactory.Create();
        }

        /// <summary>
        ///  ValidateUserLogin method is used for authentcatred the login process
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>
        public LoginViewModel ValidateUserLogin(LoginViewModel objModel)
        {
            LoginViewModel ReturnModel = new LoginViewModel();
            string decrypted = "";
            try
            {
                UserMaster userMaster = new UserMaster();
                if (objModel.IsMobileLogin==true)
                {
                    decrypted = Utility.DecryptString(objModel.Password!);

                }
                else
                {
                    decrypted = Utility.Decrypt(objModel.Password!);

                }

                string passwordHash = Utility.GetHashedPassword(decrypted);
                ReturnModel._tenantName = objModel._tenantName;
                //var encryptedPassword = EncodePassword(objModel.Password);

                var user = this.context?.UserMaster.Include(x => x.Membership).Where(x => x.EmailAddress == objModel.Email && x.PasswordHash == passwordHash /*&& x.Membership.ProfileType != "Student" && x.Membership.ProfileType != "Parent"*/).FirstOrDefault();

                if (user!=null)
                {
                    objModel.MembershipId = user.Membership.MembershipId;
                    objModel.MembershipName = user.Membership.Profile;
                    objModel.Name = user.Name;
                }

                var correctEmailList = this.context?.UserMaster.Where(x => x.EmailAddress.Contains(objModel.Email!)).ToList();

                var correctPasswordList = this.context?.UserMaster.Where(x => x.PasswordHash == passwordHash).ToList();

                if (correctEmailList?.Count > 0)
                {
                    objModel.MembershipId = correctEmailList!.FirstOrDefault()!.MembershipId;
                    objModel.MembershipName = this.context?.Membership.Where(x => x.SchoolId== correctEmailList.FirstOrDefault()!.SchoolId && x.TenantId== correctEmailList.FirstOrDefault()!.TenantId && x.MembershipId==correctEmailList!.FirstOrDefault()!.MembershipId).FirstOrDefault()?.Profile;
                    objModel.Name = correctEmailList!.FirstOrDefault()!.Name;
                    //objModel.SchoolId = correctEmailList.FirstOrDefault()!.SchoolId;

                    var schoolDetails = this.context?.SchoolDetail.FirstOrDefault(x => x.SchoolId == correctEmailList.FirstOrDefault()!.LastUsedSchoolId);

                    //if (schoolDetails.Status != true)
                    if (schoolDetails?.Status != true)
                    {
                        objModel.SchoolId = correctEmailList.FirstOrDefault()!.SchoolId;
                    }
                    else
                    {
                        objModel.SchoolId = correctEmailList.FirstOrDefault()!.LastUsedSchoolId;
                    }

                    var userAccessLogData = this.context?.UserAccessLog.Where(x => x.Emailaddress == objModel.Email && x.Ipaddress == objModel.userAccessLog!.Ipaddress && x.LoginAttemptDate.Date == DateTime.UtcNow.Date).OrderByDescending(x => x.Id).FirstOrDefault();

                    if (user == null && correctEmailList!.Count > 0 && correctPasswordList!.Count == 0)
                    {
                        if (userAccessLogData != null && userAccessLogData.LoginFailureCount > 0)
                        {
                            userAccessLogData.LoginFailureCount = userAccessLogData.LoginFailureCount + 1;
                            this.context?.SaveChanges();
                        }
                        else
                        {
                            objModel.userAccessLog!.LoginStatus = false;
                            objModel.userAccessLog.LoginFailureCount = 1;
                            AddUserAccessLog(objModel);
                        }
                    }
                    if (user == null && correctEmailList!.Count > 0 && correctPasswordList!.Count > 0)
                    {
                        if (userAccessLogData != null && userAccessLogData.LoginFailureCount > 0)
                        {
                            userAccessLogData.LoginFailureCount = userAccessLogData.LoginFailureCount + 1;
                            this.context?.SaveChanges();
                        }
                        else
                        {
                            objModel.userAccessLog!.LoginStatus = false;
                            objModel.userAccessLog.LoginFailureCount = 1;
                            AddUserAccessLog(objModel);
                        }
                    }
                    if (user != null && correctEmailList!.Count > 0 && correctPasswordList!.Count > 0)
                    {
                        objModel.userAccessLog!.LoginStatus = true;
                        objModel.userAccessLog.LoginFailureCount = 0;
                        AddUserAccessLog(objModel);
                    }

                    if (objModel.MembershipId != 1)
                    {
                        var schoolPreferenceData = this.context?.SchoolPreference.FirstOrDefault(x => x.TenantId == correctEmailList.FirstOrDefault()!.TenantId && x.SchoolId == correctEmailList.FirstOrDefault()!.SchoolId);

                        if (schoolPreferenceData != null)
                        {

                            if (correctEmailList.FirstOrDefault()?.LoginFailureCount != null && schoolPreferenceData.MaxLoginFailure != null && correctEmailList.FirstOrDefault()?.LoginFailureCount >= schoolPreferenceData.MaxLoginFailure)
                            {
                                ReturnModel.UserId = null;
                                ReturnModel._failure = true;
                                ReturnModel._message = INCORRECTLOGINATTEMPTMESSAGE;

                                //objModel.SchoolId = correctEmailList.FirstOrDefault()!.SchoolId;

                                //if (ReturnModel._failure == true)
                                //{
                                //    objModel.userAccessLog!.LoginStatus = false;
                                //    objModel.userAccessLog.LoginFailureCount = correctEmailList.FirstOrDefault()?.LoginFailureCount;
                                //    AddUserAccessLog(objModel);
                                //}

                                return ReturnModel;
                            }

                            if (correctEmailList.FirstOrDefault()?.LoginAttemptDate != null && schoolPreferenceData.MaxInactivityDays != null && schoolPreferenceData.MaxInactivityDays != 0)
                            {
                                //int numberOfDays = (DateTime.UtcNow - correctEmailList.FirstOrDefault().LoginAttemptDate).Value.Days;
                                int numberOfDays = (DateTime.UtcNow - correctEmailList.FirstOrDefault()!.LoginAttemptDate)!.Value.Days;

                                if (numberOfDays >= schoolPreferenceData.MaxInactivityDays)
                                {
                                    ReturnModel.UserId = null;
                                    ReturnModel._failure = true;
                                    ReturnModel._message = INACTIVITYDAYSMESSAGE;

                                    //this block making staff inactive for MaxInactivityDays
                                    var staffData = this.context?.StaffMaster.FirstOrDefault(x => x.TenantId == correctEmailList.FirstOrDefault()!.TenantId && x.StaffId == correctEmailList.FirstOrDefault()!.UserId);
                                    if (staffData != null)
                                    {
                                        staffData.IsActive = false;
                                    }
                                    this.context?.SaveChanges();
                                    return ReturnModel;
                                }
                            }
                        }
                    }
                }

                if (user == null && correctEmailList?.Count > 0 && correctPasswordList?.Count == 0)
                {
                    ReturnModel.UserId = null;
                    ReturnModel._failure = true;
                    ReturnModel._message = PASSWORDMESSAGE;

                    if (correctEmailList.FirstOrDefault()?.LoginFailureCount != null)
                    {
                        correctEmailList.FirstOrDefault()!.LoginFailureCount = correctEmailList.FirstOrDefault()?.LoginFailureCount + 1;
                    }
                    else
                    {
                        correctEmailList.FirstOrDefault()!.LoginFailureCount = 1;
                    }                 
                }
                //else if (user == null && correctEmailList.Count == 0 && correctPasswordList.Count > 0)
                else if (user == null && correctEmailList?.Count == 0 && correctPasswordList?.Count > 0)
                {
                    ReturnModel.UserId = null;
                    ReturnModel._failure = true;
                    ReturnModel._message = EMAILMESSAGE;
                }
                //else if (user == null && correctEmailList.Count == 0 && correctPasswordList.Count == 0)
                else if (user == null && correctEmailList?.Count == 0 && correctPasswordList?.Count == 0)
                {
                    ReturnModel.UserId = null;
                    ReturnModel._failure = true;
                    ReturnModel._message = PASSWORDMESSAGE;
                }
                //else if (user == null && correctEmailList.Count > 0 && correctPasswordList.Count > 0)
                else if (user == null && correctEmailList?.Count > 0 && correctPasswordList?.Count > 0)
                {
                    ReturnModel.UserId = null;
                    ReturnModel._failure = true;
                    ReturnModel._message = PASSWORDMESSAGE;

                    if (correctEmailList.FirstOrDefault()?.LoginFailureCount != null)
                    {
                        correctEmailList.FirstOrDefault()!.LoginFailureCount = correctEmailList.FirstOrDefault()?.LoginFailureCount + 1;
                    }
                    else
                    {
                        correctEmailList.FirstOrDefault()!.LoginFailureCount = 1;
                    }
                }
                else
                {
                    ReturnModel.MembershipId = user?.Membership.MembershipId;
                    ReturnModel.MembershipType = user?.Membership.ProfileType;
                    ReturnModel.MembershipName = user?.Membership.Profile;

                    //if (user.Membership.ProfileType.ToLower() == "Student".ToLower())
                    if (user?.Membership?.ProfileType == "Student")
                    {
                        var userData = this.context?.StudentMaster.Where(x => x.TenantId == user.TenantId && x.SchoolId == user.SchoolId && x.StudentId == user.UserId).Select(x => new StudentMaster()
                        { StudentPhoto = x.StudentThumbnailPhoto, StudentGuid = x.StudentGuid, Suffix = x.Suffix, FirstGivenName = x.FirstGivenName, MiddleName = x.MiddleName, LastFamilyName = x.LastFamilyName }).FirstOrDefault();

                        if (userData != null)
                        {
                            ReturnModel.UserPhoto = userData.StudentPhoto;
                            ReturnModel.UserGuid = userData.StudentGuid.ToString();
                            ReturnModel.Suffix = userData.Suffix;
                            ReturnModel.FirstGivenName = userData.FirstGivenName;
                            ReturnModel.MiddleName = userData.MiddleName;
                            ReturnModel.LastFamilyName = userData.LastFamilyName;
                        }
                    }
                    //else if (user.Membership.ProfileType.ToLower() == "Parent".ToLower())
                    else if (user?.Membership?.ProfileType == "Parent")
                    {
                        var userData = this.context?.ParentInfo.Where(x => x.TenantId == user.TenantId && x.SchoolId == user.SchoolId && x.ParentId == user.UserId).Select(x => new ParentInfo()
                        { ParentPhoto = x.ParentThumbnailPhoto, Suffix = x.Suffix, ParentGuid = x.ParentGuid, Firstname = x.Firstname, Middlename = x.Middlename, Lastname = x.Lastname }).FirstOrDefault();

                        if (userData != null)
                        {
                            ReturnModel.UserPhoto = userData.ParentPhoto;
                            ReturnModel.UserGuid = userData.ParentGuid.ToString();
                            ReturnModel.Suffix = userData.Suffix;
                            ReturnModel.FirstGivenName = userData.Firstname;
                            ReturnModel.MiddleName = userData.Middlename;
                            ReturnModel.LastFamilyName = userData.Lastname;
                        }
                    }
                    else
                    {
                        if (user?.Membership?.ProfileType == "Super Administrator")
                        {
                            var userData = this.context?.StaffMaster.Where(x => x.TenantId == user!.TenantId /*&& x.SchoolId == user.SchoolId*/ && x.StaffId == user.UserId).Select(x => new StaffMaster()
                            {
                                StaffPhoto = x.StaffThumbnailPhoto,
                                Suffix = x.Suffix,
                                FirstGivenName = x.FirstGivenName,
                                MiddleName = x.MiddleName,
                                LastFamilyName = x.LastFamilyName,
                                StaffGuid = x.StaffGuid,
                            }).FirstOrDefault();

                            if (userData != null)
                            {
                                ReturnModel.UserPhoto = userData.StaffPhoto;
                                ReturnModel.Suffix = userData.Suffix;
                                ReturnModel.FirstGivenName = userData.FirstGivenName;
                                ReturnModel.MiddleName = userData.MiddleName;
                                ReturnModel.LastFamilyName = userData.LastFamilyName;
                                ReturnModel.UserGuid = userData.StaffGuid.ToString();
                            }
                        }
                        else
                        {
                            var staffDefaultData = this.context?.StaffMaster.Where(x => x.TenantId == objModel.TenantId && x.LoginEmailAddress == objModel.Email).Select(g => new StaffMaster()
                            {
                                StaffId = g.StaffId,
                                SchoolId = g.SchoolId,
                            }).FirstOrDefault();

                            if (staffDefaultData != null)
                            {
                                var activeSchool = this.context?.SchoolDetail.Where(x => x.SchoolId == staffDefaultData.SchoolId).Select(y => y.Status).FirstOrDefault();

                                if (activeSchool == false)
                                {
                                    ReturnModel.UserId = user?.UserId;
                                    ReturnModel.TenantId = user?.TenantId;
                                    ReturnModel.Email = user?.EmailAddress;
                                    ReturnModel.Name = user?.Name;
                                    ReturnModel.LastUsedSchoolId = user?.LastUsedSchoolId;
                                    //ReturnModel.MembershipName = user?.Membership.Profile;
                                    //ReturnModel.MembershipType = user?.Membership.ProfileType;
                                    //ReturnModel.MembershipId = user?.Membership.MembershipId;
                                    ReturnModel._failure = true;
                                    ReturnModel._message = "Your default school is inactive, please contact the Administrator";
                                    return ReturnModel;
                                }
                                else
                                {
                                    var staffSchoolInfoData = this.context?.StaffSchoolInfo.Include(s => s.Membership).Where(x => x.TenantId == objModel.TenantId && x.SchoolAttachedId == staffDefaultData!.SchoolId && x.StaffId == staffDefaultData.StaffId && (x.EndDate == null || x.EndDate.Value.Date >= DateTime.UtcNow.Date) && x.StartDate <= DateTime.UtcNow.Date).FirstOrDefault();

                                    if (staffSchoolInfoData != null)
                                    {
                                        var userData = this.context?.StaffMaster.Where(x => x.TenantId == user!.TenantId /*&& x.SchoolId == user.SchoolId*/ && x.StaffId == user.UserId).Select(x => new StaffMaster()
                                        {
                                            StaffPhoto = x.StaffThumbnailPhoto,
                                            Suffix = x.Suffix,
                                            FirstGivenName = x.FirstGivenName,
                                            MiddleName = x.MiddleName,
                                            LastFamilyName = x.LastFamilyName,
                                            StaffGuid = x.StaffGuid,
                                        }).FirstOrDefault();

                                        if (userData != null)
                                        {
                                            ReturnModel.UserPhoto = userData.StaffPhoto;
                                            ReturnModel.Suffix = userData.Suffix;
                                            ReturnModel.FirstGivenName = userData.FirstGivenName;
                                            ReturnModel.MiddleName = userData.MiddleName;
                                            ReturnModel.LastFamilyName = userData.LastFamilyName;
                                            ReturnModel.UserGuid = userData.StaffGuid.ToString();

                                            if (user?.LastUsedSchoolId != null)
                                            {
                                                var lastSchoolMembershipId = this.context?.StaffSchoolInfo.Include(s => s.Membership).Where(x => x.TenantId == objModel.TenantId && x.SchoolAttachedId == (int)user.LastUsedSchoolId && x.StaffId == staffDefaultData!.StaffId && (x.EndDate == null || x.EndDate.Value.Date >= DateTime.UtcNow.Date)).FirstOrDefault();

                                                if (lastSchoolMembershipId != null)
                                                {
                                                    ReturnModel.MembershipId = lastSchoolMembershipId.MembershipId;
                                                    ReturnModel.MembershipType = lastSchoolMembershipId.Membership?.ProfileType;
                                                    ReturnModel.MembershipName = lastSchoolMembershipId.Membership?.Profile;
                                                }
                                                else
                                                {
                                                    ReturnModel.LastUsedSchoolId = staffSchoolInfoData.SchoolId;
                                                    ReturnModel.MembershipType = staffSchoolInfoData.Membership?.ProfileType;
                                                    ReturnModel.MembershipName = staffSchoolInfoData.Membership?.Profile;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var staffSchoolInfoAllData = this.context?.StaffSchoolInfo.Include(s => s.Membership).Where(x => x.TenantId == objModel.TenantId && x.StaffId == staffDefaultData.StaffId).ToList();

                                        var schoolInfoData = staffSchoolInfoAllData?.Where(x => x.TenantId == objModel.TenantId && x.StaffId == staffDefaultData.StaffId && (x.EndDate == null || x.EndDate.Value.Date >= DateTime.UtcNow.Date) && x.StartDate <= DateTime.UtcNow.Date).FirstOrDefault();

                                        if (schoolInfoData != null)
                                        {
                                            var userData = this.context?.StaffMaster.Where(x => x.TenantId == user!.TenantId && x.StaffId == user.UserId).FirstOrDefault();

                                            if (userData != null)
                                            {
                                                ReturnModel.UserPhoto = userData.StaffPhoto;
                                                ReturnModel.Suffix = userData.Suffix;
                                                ReturnModel.FirstGivenName = userData.FirstGivenName;
                                                ReturnModel.MiddleName = userData.MiddleName;
                                                ReturnModel.LastFamilyName = userData.LastFamilyName;
                                                ReturnModel.UserGuid = userData.StaffGuid.ToString();

                                                ReturnModel.MembershipId = schoolInfoData.MembershipId;
                                                ReturnModel.MembershipType = schoolInfoData.Membership.ProfileType;
                                                ReturnModel.MembershipName = schoolInfoData.Membership?.Profile;

                                                //update
                                                userData.SchoolId = (int)schoolInfoData.SchoolAttachedId!;

                                                staffSchoolInfoAllData?.ForEach(s => s.SchoolId = schoolInfoData.SchoolAttachedId);

                                                //user.SchoolId = (int)schoolInfoData.SchoolAttachedId;
                                                //user.MembershipId = (int)schoolInfoData.MembershipId;
                                                //user.LastUsedSchoolId = schoolInfoData.SchoolAttachedId;

                                                var loginInfoData = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == objModel.TenantId && x.EmailAddress == objModel.Email);

                                                if (loginInfoData != null)
                                                {
                                                    var loginInfoDataForRemove = loginInfoData;
                                                    if (loginInfoDataForRemove != null)
                                                    {
                                                        this.context?.UserMaster.Remove(loginInfoDataForRemove);
                                                    }

                                                    UserMaster userMasters = new UserMaster();
                                                    //userMasters = loginInfoData;
                                                    userMasters.SchoolId = (int)schoolInfoData.SchoolAttachedId;
                                                    userMasters.MembershipId = (int)schoolInfoData.MembershipId;
                                                    userMasters.LastUsedSchoolId = schoolInfoData.SchoolAttachedId;
                                                    userMasters.TenantId = loginInfoData.TenantId;
                                                    userMasters.UserId = loginInfoData.UserId;
                                                    userMasters.Name = loginInfoData.Name;
                                                    userMasters.EmailAddress = loginInfoData.EmailAddress;
                                                    userMasters.PasswordHash = loginInfoData.PasswordHash;
                                                    userMasters.LangId = loginInfoData.LangId;     
                                                    userMasters.UpdatedOn = DateTime.UtcNow;
                                                    userMasters.UpdatedBy = loginInfoData.UpdatedBy;
                                                    userMasters.IsActive = true;
                                                    this.context?.UserMaster.Add(userMasters);
                                                }  
                                            }
                                        }
                                        else
                                        {
                                            ReturnModel.UserId = user?.UserId;
                                            ReturnModel.TenantId = user?.TenantId;
                                            ReturnModel.Email = user?.EmailAddress;
                                            ReturnModel.Name = user?.Name;
                                            ReturnModel.LastUsedSchoolId = user?.LastUsedSchoolId;
                                            //ReturnModel.MembershipName = user?.Membership.Profile;
                                            //ReturnModel.MembershipType = user?.Membership.ProfileType;
                                            //ReturnModel.MembershipId = user?.Membership.MembershipId;
                                            ReturnModel._failure = true;
                                            ReturnModel._message = "Your account is inactive, please contact your Administrator";
                                            return ReturnModel;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //ReturnModel.UserId = user.UserId;
                    //ReturnModel.TenantId = user.TenantId;
                    //ReturnModel.Email = user.EmailAddress;
                    //ReturnModel.Name = user.Name;
                    //ReturnModel.LastUsedSchoolId = user.LastUsedSchoolId;
                    //ReturnModel.MembershipName = user.Membership.Profile;
                    //ReturnModel.MembershipType = user.Membership.ProfileType;
                    //ReturnModel.MembershipId = user.Membership.MembershipId;
                    ReturnModel.UserId = user?.UserId;
                    ReturnModel.TenantId = user?.TenantId;
                    ReturnModel.Email = user?.EmailAddress;
                    ReturnModel.Name = user?.Name;
                    ReturnModel.LastUsedSchoolId = user?.LastUsedSchoolId;
                    //ReturnModel.MembershipName = user?.Membership.Profile;
                    //ReturnModel.MembershipType = user?.Membership.ProfileType;
                    //ReturnModel.MembershipId = user?.Membership.MembershipId;
                    ReturnModel._failure = false;
                    ReturnModel._message = "";

                    //if (user.LastUsedSchoolId != null && user.LastUsedSchoolId != 0)
                    if (user?.LastUsedSchoolId != null && user.LastUsedSchoolId != 0)
                    {
                        var schoolDetails = this.context?.SchoolDetail.FirstOrDefault(x => x.SchoolId == user.LastUsedSchoolId);

                        //if (schoolDetails.Status != true)
                        if (schoolDetails?.Status != true)
                        {
                            objModel.SchoolId = user.SchoolId;
                        }
                        else
                        {
                            objModel.SchoolId = user.LastUsedSchoolId;
                        }
                    }
                    else
                    {
                        //objModel.SchoolId = user.SchoolId;
                        objModel.SchoolId = user?.SchoolId;
                    }
                    ReturnModel.SchoolId = objModel.SchoolId;

                    //correctEmailList.FirstOrDefault().LoginFailureCount = 0;
                    //correctEmailList.FirstOrDefault().LoginAttemptDate = DateTime.UtcNow;
                    correctEmailList!.FirstOrDefault()!.LoginFailureCount = 0;
                    correctEmailList!.FirstOrDefault()!.LoginAttemptDate = DateTime.UtcNow;
                }

                //this block making staff inactive for MaximumLoginFailure
                //if (correctEmailList?.Count > 0)
                if (correctEmailList?.Count > 0)
                {
                    //var schoolPreferenceData = this.context?.SchoolPreference.FirstOrDefault(x => x.TenantId == correctEmailList.FirstOrDefault().TenantId && x.SchoolId == correctEmailList.FirstOrDefault().SchoolId);
                    var schoolPreferenceData = this.context?.SchoolPreference.FirstOrDefault(x => x.TenantId == correctEmailList.FirstOrDefault()!.TenantId && x.SchoolId == correctEmailList.FirstOrDefault()!.SchoolId);

                    if (schoolPreferenceData != null)
                    {
                        //if (correctEmailList.FirstOrDefault().LoginFailureCount >= schoolPreferenceData.MaxLoginFailure)
                        if (correctEmailList.FirstOrDefault()!.LoginFailureCount >= schoolPreferenceData.MaxLoginFailure)
                        {
                            //var staffData = this.context?.StaffMaster.FirstOrDefault(x => x.TenantId == correctEmailList.FirstOrDefault().TenantId && x.StaffId == correctEmailList.FirstOrDefault().UserId);
                            var staffData = this.context?.StaffMaster.FirstOrDefault(x => x.TenantId == correctEmailList.FirstOrDefault()!.TenantId && x.StaffId == correctEmailList.FirstOrDefault()!.UserId);
                            if (staffData != null)
                            {
                                staffData.IsActive = false;
                            }
                        }
                    }
                }
                this.context?.SaveChanges();

                //if (ReturnModel._failure == false)
                //{
                //    //objModel.userAccessLog.LoginStatus = true;
                //    if (objModel.userAccessLog != null)
                //    {

                //        objModel.userAccessLog.LoginStatus = true;
                //        objModel.userAccessLog.LoginFailureCount = 0;
                //        AddUserAccessLog(objModel);
                //    }

                //}
            }
            catch (Exception ex)
            {
                ReturnModel._failure = true;
                ReturnModel._message = ex.Message;
            }

            return ReturnModel;
        }

        /// <summary>
        /// Check User Login Email
        /// </summary>
        /// <param name="checkUserEmailAddressViewModel"></param>
        /// <returns></returns>
        public CheckUserEmailAddressViewModel CheckUserLoginEmail(CheckUserEmailAddressViewModel checkUserEmailAddressViewModel)
        {
            var checkEmailAddress = this.context?.UserMaster.Where(x => x.TenantId == checkUserEmailAddressViewModel.TenantId && x.EmailAddress == checkUserEmailAddressViewModel.EmailAddress).ToList();
            //if (checkEmailAddress.Count() > 0)
            if (checkEmailAddress?.Count() > 0)
            {
                checkUserEmailAddressViewModel.IsValidEmailAddress = false;
                checkUserEmailAddressViewModel._message = "User Login Email Address Already Exist";
            }
            else
            {
                checkUserEmailAddressViewModel.IsValidEmailAddress = true;
                checkUserEmailAddressViewModel._message = "User Login Email Address Is Valid";
            }
            return checkUserEmailAddressViewModel;
        }

        /// <summary>
        /// LogOut For User
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        public LoginViewModel LogOutForUser(LoginViewModel loginViewModel)
        {
            try
            {
                //var userMasterData = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == loginViewModel.TenantId && x.EmailAddress == loginViewModel.Email);
                //if (userMasterData != null)
                //{
                var loginSessionData = this.context?.LoginSession.FirstOrDefault(x => x.TenantId == loginViewModel.TenantId && x.EmailAddress == loginViewModel.Email && x.IsExpired == false);

                if (loginSessionData != null)
                {
                    //loginSessionData.ForEach(x => x.IsExpired = true);
                    loginSessionData.IsExpired = true;
                    //this.context.SaveChanges();
                    this.context?.SaveChanges();
                }
                loginViewModel._failure = false;
                loginViewModel._message = "User LogOut Successfully";
                // }
                //else
                //{
                //    loginViewModel._failure = true;
                //    loginViewModel._message = "User Login Email Is Not Valid";
                //}
            }
            catch (Exception ex)
            {
                loginViewModel._failure = true;
                loginViewModel._message = ex.Message;
            }
            return loginViewModel;
        }

        public bool AddLoginSession(LoginViewModel returnModel)
        {
            //string msg = null;
            string? msg = null;
            try
            {
                if (returnModel != null)
                {
                    int? ide = 1;
                    var loginSession = new LoginSession();
                    var LoginSessionId = this.context?.LoginSession.OrderByDescending(x => x.Id).FirstOrDefault();
                    if (LoginSessionId != null)
                    {
                        ide = LoginSessionId.Id + 1;
                    }
                    var loginSessionData = this.context?.LoginSession.FirstOrDefault(x => x.TenantId == returnModel.TenantId && x.EmailAddress == returnModel.Email && x.IsExpired == false);

                    if (loginSessionData != null)
                    {
                        loginSessionData.IsExpired = true;
                    }
                    loginSession.Id = (int)ide;
                    //loginSession.TenantId = (Guid)returnModel.TenantId;
                    //loginSession.SchoolId = (int)returnModel.SchoolId;
                    loginSession.TenantId = (Guid)returnModel.TenantId!;
                    loginSession.SchoolId = (int)returnModel.SchoolId!;
                    loginSession.EmailAddress = returnModel.Email!;
                    loginSession.Token = returnModel._token;
                    loginSession.IsExpired = false;
                    loginSession.LoginTime = DateTime.UtcNow;
                    loginSession.CreatedOn = DateTime.UtcNow;
                    loginSession.CreatedBy = returnModel.CreatedBy;
                    this.context?.LoginSession.Add(loginSession);
                    this.context?.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception es)
            {
                msg = es.Message;
                return false;
            }
        }

        private void AddUserAccessLog(LoginViewModel objModel)
        {
            try
            {
                long? Id = Utility.GetMaxLongPK(this.context, new Func<UserAccessLog, long>(x => x.Id));

                //objModel.userAccessLog.Id = (long)Id;
                objModel.userAccessLog!.Id = (long)Id!;

                //objModel.userAccessLog.TenantId = (Guid)objModel.TenantId;
                //objModel.userAccessLog.SchoolId = (int)objModel.SchoolId;
                //objModel.userAccessLog.Emailaddress = objModel.Email;
                objModel.userAccessLog.TenantId = (Guid)objModel.TenantId!;
                objModel.userAccessLog.SchoolId = (int)objModel.SchoolId!;
                objModel.userAccessLog.Emailaddress = objModel.Email!;
                objModel.userAccessLog.UserName = Utility.CreatedOrUpdatedByForAccessLog(this.context, objModel.TenantId, objModel.Email);
                objModel.userAccessLog.MembershipId = objModel.MembershipId;
                objModel.userAccessLog.Profile = objModel.MembershipName;
                //objModel.userAccessLog.LoginFailureCount = correctEmailList.FirstOrDefault().LoginFailureCount;
                //objModel.userAccessLog.Ipaddress = "ghtyytr";
                //objModel.userAccessLog.CreatedBy = objModel.Email;
                objModel.userAccessLog.LoginAttemptDate = DateTime.UtcNow;
                objModel.userAccessLog.CreatedOn = DateTime.UtcNow;
                this.context?.UserAccessLog.Add(objModel.userAccessLog);
                this.context?.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }                        
        }        

        public UserAccessLogListViewModel GetAllUserAccessLog(PageResult pageResult)
        {
            UserAccessLogListViewModel userAccessLogListView = new UserAccessLogListViewModel();
            //IQueryable<UserAccessLog> transactionIQ = null;
            IQueryable<UserAccessLog>? transactionIQ = null;
            try
            {
                if (pageResult.DobStartDate != null && pageResult.DobEndDate != null)
                {
                    var userAccessLogDataList = this.context?.UserAccessLog.Where(c => c.TenantId == pageResult.TenantId && c.SchoolId == pageResult.SchoolId && c.LoginAttemptDate.Date >= pageResult.DobStartDate.Value.Date && c.LoginAttemptDate.Date <= pageResult.DobEndDate.Value.Date);

                    if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                    {

                        transactionIQ = userAccessLogDataList;
                    }

                    else
                    {
                        if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                        {
                            string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                            //transactionIQ = userAccessLogDataList.Where(x => x.Emailaddress.ToLower().Contains(Columnvalue.ToLower()) || x.UserName.ToLower().Contains(Columnvalue.ToLower()) || x.Profile.ToLower().Contains(Columnvalue.ToLower()) || (x.LoginFailureCount.ToString() == Columnvalue) || x.Ipaddress.ToLower().Contains(Columnvalue.ToLower()));
                            transactionIQ = userAccessLogDataList?.Where(x => x.Emailaddress.Contains(Columnvalue) || x.UserName!.Contains(Columnvalue) || x.Profile!.Contains(Columnvalue) || (x.LoginFailureCount.ToString() == Columnvalue) || x.Ipaddress!.Contains(Columnvalue));
                        }
                    }
                    //transactionIQ = transactionIQ.OrderByDescending(s => s.LoginAttemptDate);
                    transactionIQ = transactionIQ!.OrderByDescending(s => s.LoginAttemptDate);
                    int totalCount = transactionIQ.Count();

                    if (pageResult.IsDelete==true)
                    {
                        if (totalCount>0)
                        {
                            this.context?.UserAccessLog.RemoveRange(transactionIQ);
                            this.context?.SaveChanges();
                            userAccessLogListView._failure = false;
                            userAccessLogListView._message = "Access log deleted successfullyy";
                            return userAccessLogListView;
                        }
                        else
                        {
                            userAccessLogListView._failure = true;
                            userAccessLogListView._message = NORECORDFOUND;
                            return userAccessLogListView;
                        }                        
                    }

                    if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                    {
                        transactionIQ = transactionIQ.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                    }

                    userAccessLogListView.TenantId = pageResult.TenantId;
                    userAccessLogListView.userAccessLogList = transactionIQ.ToList();
                    userAccessLogListView.TotalCount = totalCount;
                    userAccessLogListView.PageNumber = pageResult.PageNumber;
                    userAccessLogListView._pageSize = pageResult.PageSize;
                    userAccessLogListView._tenantName = pageResult._tenantName;
                    userAccessLogListView._token = pageResult._token;
                    userAccessLogListView._failure = false;
                }
                else
                {
                    userAccessLogListView._failure = true;
                    userAccessLogListView._message = "Please provide a date range";
                }                    
            }
            catch (Exception es)
            {
                userAccessLogListView._failure = true;
                userAccessLogListView._message = es.Message;
            }
            return userAccessLogListView;
        }





        public UserAccessLogListViewModel DeleteUserAccessLogs (UserAccessLogListViewModel userAccessLogListViewModel)
        {
            UserAccessLogListViewModel userAccessLogList = new UserAccessLogListViewModel();
            try
            {
                //if (userAccessLogListViewModel.UserAccessLogIds.Count>0)
                if (userAccessLogListViewModel.UserAccessLogIds?.Count > 0)
                {
                    var useraccessData = this.context?.UserAccessLog.Where(c => c.TenantId == userAccessLogListViewModel.TenantId && (userAccessLogListViewModel.UserAccessLogIds == null || (userAccessLogListViewModel.UserAccessLogIds.Contains(c.Id)))).ToList();

                    //if (useraccessData.Count>0)
                    if (useraccessData?.Count > 0)
                    {
                        this.context?.UserAccessLog.RemoveRange(useraccessData);
                        this.context?.SaveChanges();
                        userAccessLogList._failure = false;
                        userAccessLogList._message = "Access Log deleted successfullyy";
                    }
                    else
                    {
                        userAccessLogList._failure = true;
                        userAccessLogList._message = NORECORDFOUND;
                    }
                }
                else
                {
                    userAccessLogList._failure = true;
                    userAccessLogList._message = "Select Atleast One Access Log";
                }
            }
            catch (Exception es)
            {
                userAccessLogList._failure = true;
                userAccessLogList._message = es.Message;
            }
            return userAccessLogList;
        }
    }
}

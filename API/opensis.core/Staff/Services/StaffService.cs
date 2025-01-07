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

using opensis.core.helper;
using opensis.core.helper.Interfaces;
using opensis.core.Staff.Interfaces;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.Staff;
using opensis.data.ViewModels.StaffSchedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.core.Staff.Services
{
    public class StaffService : IStaffService
    {
        private static string SUCCESS = "success";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public IStaffRepository staffRepository;
        public ICheckLoginSession tokenManager;
        public StaffService(IStaffRepository staffRepository, ICheckLoginSession checkLoginSession)
        {
            this.staffRepository = staffRepository;
            this.tokenManager = checkLoginSession;
        }
      
        /// <summary>
        /// Add Staff
        /// </summary>
        /// <param name="StaffAddViewModel"></param>
        /// <returns></returns>
        public StaffAddViewModel AddStaff(StaffAddViewModel staffAddViewModel)
        {
            StaffAddViewModel staffInfoAddViewModel = new StaffAddViewModel();
            try
            {
                if (tokenManager.CheckToken(staffAddViewModel._tenantName + staffAddViewModel._userName, staffAddViewModel._token))
                {

                    staffInfoAddViewModel = this.staffRepository.AddStaff(staffAddViewModel);

                }
                else
                {
                    staffInfoAddViewModel._failure = true;
                    staffInfoAddViewModel._message = TOKENINVALID;

                }
            }
            catch (Exception es)
            {

                staffInfoAddViewModel._failure = true;
                staffInfoAddViewModel._message = es.Message;
            }
            return staffInfoAddViewModel;

        }

        /// <summary>
        /// Get All Staff List
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public StaffListModel GetAllStaffList(PageResult pageResult)
        {
            logger.Info("Method getAllStaffList called.");
            StaffListModel staffList = new StaffListModel();
            try
            {
                if (tokenManager.CheckToken(pageResult._tenantName+pageResult._userName, pageResult._token))
                {
                    staffList = this.staffRepository.GetAllStaffList(pageResult);
                    if (staffList.staffMaster.Count > 0)
                    {
                        staffList._message = SUCCESS;
                        staffList._failure = false;
                    }
                    else
                    {
                        staffList._message = "NO RECORD FOUND";
                        staffList._failure = true;
                    }
                    logger.Info("Method getAllStaffList end with success.");
                }

                else
                {
                    staffList._failure = true;
                    staffList._message = TOKENINVALID;
                    return staffList;
                }
            }
            catch (Exception ex)
            {
                staffList._message = ex.Message;
                staffList._failure = true;
                logger.Error("Method getAllStaffList end with error :" + ex.Message);
            }
            return staffList;
        }

        /// <summary>
        /// Get Staff By Id
        /// </summary>
        /// <param name="staffAddViewModel"></param>
        /// <returns></returns>
        public StaffAddViewModel ViewStaff(StaffAddViewModel staffAddViewModel)
        {
            StaffAddViewModel staffView = new StaffAddViewModel();
            if (tokenManager.CheckToken(staffAddViewModel._tenantName + staffAddViewModel._userName, staffAddViewModel._token))
            {
                staffView = this.staffRepository.ViewStaff(staffAddViewModel);
            }
            else
            {
                staffView._failure = true;
                staffView._message = TOKENINVALID;
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
            StaffAddViewModel staffUpdate = new StaffAddViewModel();
            if (tokenManager.CheckToken(staffAddViewModel._tenantName + staffAddViewModel._userName, staffAddViewModel._token))
            {
                staffUpdate = this.staffRepository.UpdateStaff(staffAddViewModel);
            }
            else
            {
                staffUpdate._failure = true;
                staffUpdate._message = TOKENINVALID;
            }
            return staffUpdate;
        }

        /// <summary>
        /// Check Staff Internal Id
        /// </summary>
        /// <param name="checkStaffInternalIdViewModel"></param>
        /// <returns></returns>
        public CheckStaffInternalIdViewModel CheckStaffInternalId(CheckStaffInternalIdViewModel checkStaffInternalIdViewModel)
        {
            CheckStaffInternalIdViewModel checkInternalId = new CheckStaffInternalIdViewModel();
            if (tokenManager.CheckToken(checkStaffInternalIdViewModel._tenantName + checkStaffInternalIdViewModel._userName, checkStaffInternalIdViewModel._token))
            {
                checkInternalId = this.staffRepository.CheckStaffInternalId(checkStaffInternalIdViewModel);
            }
            else
            {
                checkInternalId._failure = true;
                checkInternalId._message = TOKENINVALID;
            }
            return checkInternalId;
        }

        /// <summary>
        /// Add Staff School Info
        /// </summary>
        /// <param name="staffSchoolInfoAddViewModel"></param>
        /// <returns></returns>
        public StaffSchoolInfoAddViewModel AddStaffSchoolInfo(StaffSchoolInfoAddViewModel staffSchoolInfoAddViewModel)
        {
            StaffSchoolInfoAddViewModel staffSchoolInfoAdd = new StaffSchoolInfoAddViewModel();
            try
            {
                if (tokenManager.CheckToken(staffSchoolInfoAddViewModel._tenantName + staffSchoolInfoAddViewModel._userName, staffSchoolInfoAddViewModel._token))
                {
                    staffSchoolInfoAdd = this.staffRepository.AddStaffSchoolInfo(staffSchoolInfoAddViewModel);
                }
                else
                {
                    staffSchoolInfoAdd._failure = true;
                    staffSchoolInfoAdd._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                staffSchoolInfoAdd._failure = true;
                staffSchoolInfoAdd._message = es.Message;
            }
            return staffSchoolInfoAdd;
        }

        /// <summary>
        /// Get Staff School Info
        /// </summary>
        /// <param name="staffSchoolInfoAddViewModel"></param>
        /// <returns></returns>
        public StaffSchoolInfoAddViewModel ViewStaffSchoolInfo(StaffSchoolInfoAddViewModel staffSchoolInfoAddViewModel)
        {
            StaffSchoolInfoAddViewModel staffSchoolInfoView = new StaffSchoolInfoAddViewModel();
            if (tokenManager.CheckToken(staffSchoolInfoAddViewModel._tenantName + staffSchoolInfoAddViewModel._userName, staffSchoolInfoAddViewModel._token))
            {
                staffSchoolInfoView = this.staffRepository.ViewStaffSchoolInfo(staffSchoolInfoAddViewModel);
            }
            else
            {
                staffSchoolInfoView._failure = true;
                staffSchoolInfoView._message = TOKENINVALID;
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
            StaffSchoolInfoAddViewModel staffSchoolInfoUpdate = new StaffSchoolInfoAddViewModel();
            try
            {
                if (tokenManager.CheckToken(staffSchoolInfoAddViewModel._tenantName + staffSchoolInfoAddViewModel._userName, staffSchoolInfoAddViewModel._token))
                {
                    staffSchoolInfoUpdate = this.staffRepository.UpdateStaffSchoolInfo(staffSchoolInfoAddViewModel);
                }
                else
                {
                    staffSchoolInfoUpdate._failure = true;
                    staffSchoolInfoUpdate._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                staffSchoolInfoUpdate._failure = true;
                staffSchoolInfoUpdate._message = es.Message;
            }
            return staffSchoolInfoUpdate;
        }

        /// <summary>
        /// Add Staff Certificate Info
        /// </summary>
        /// <param name="staffCertificateInfoAddViewModel"></param>
        /// <returns></returns>
        public StaffCertificateInfoAddViewModel AddStaffCertificateInfo(StaffCertificateInfoAddViewModel staffCertificateInfoAddViewModel)
        {
            StaffCertificateInfoAddViewModel staffCertificateInfoAdd = new StaffCertificateInfoAddViewModel();
            try
            {
                if (tokenManager.CheckToken(staffCertificateInfoAddViewModel._tenantName + staffCertificateInfoAddViewModel._userName, staffCertificateInfoAddViewModel._token))
                {

                    staffCertificateInfoAdd = this.staffRepository.AddStaffCertificateInfo(staffCertificateInfoAddViewModel);

                }
                else
                {
                    staffCertificateInfoAdd._failure = true;
                    staffCertificateInfoAdd._message = TOKENINVALID;

                }
            }
            catch (Exception es)
            {

                staffCertificateInfoAdd._failure = true;
                staffCertificateInfoAdd._message = es.Message;
            }
            return staffCertificateInfoAdd;
        }
        
        /// <summary>
        /// Get All Staff Certificate Info
        /// </summary>
        /// <param name="staffCertificateInfoListModel"></param>
        /// <returns></returns>
        public StaffCertificateInfoListModel GetAllStaffCertificateInfo(StaffCertificateInfoListModel staffCertificateInfoListModel)
        {
            StaffCertificateInfoListModel staffCertificateInfoListView = new StaffCertificateInfoListModel();
            try
            {
                if (tokenManager.CheckToken(staffCertificateInfoListModel._tenantName + staffCertificateInfoListModel._userName, staffCertificateInfoListModel._token))
                {
                    staffCertificateInfoListView = this.staffRepository.GetAllStaffCertificateInfo(staffCertificateInfoListModel);
                }
                else
                {
                    staffCertificateInfoListView._failure = true;
                    staffCertificateInfoListView._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                staffCertificateInfoListView._failure = true;
                staffCertificateInfoListView._message = es.Message;
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
            StaffCertificateInfoAddViewModel staffCertificateInfoUpdate = new StaffCertificateInfoAddViewModel();
            try
            {
                if (tokenManager.CheckToken(staffCertificateInfoAddViewModel._tenantName + staffCertificateInfoAddViewModel._userName, staffCertificateInfoAddViewModel._token))
                {
                    staffCertificateInfoUpdate = this.staffRepository.UpdateStaffCertificateInfo(staffCertificateInfoAddViewModel);
                }
                else
                {
                    staffCertificateInfoUpdate._failure = true;
                    staffCertificateInfoUpdate._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                staffCertificateInfoUpdate._failure = true;
                staffCertificateInfoUpdate._message = es.Message;
            }
            return staffCertificateInfoUpdate;
        }
        
        /// <summary>
        /// Delete Staff Certificate Info
        /// </summary>
        /// <param name="staffCertificateInfoAddViewModel"></param>
        /// <returns></returns>
        public StaffCertificateInfoAddViewModel DeleteStaffCertificateInfo(StaffCertificateInfoAddViewModel staffCertificateInfoAddViewModel)
        {
            StaffCertificateInfoAddViewModel staffCertificateInfoDelete = new StaffCertificateInfoAddViewModel();
            try
            {
                if (tokenManager.CheckToken(staffCertificateInfoAddViewModel._tenantName + staffCertificateInfoAddViewModel._userName, staffCertificateInfoAddViewModel._token))
                {
                    staffCertificateInfoDelete = this.staffRepository.DeleteStaffCertificateInfo(staffCertificateInfoAddViewModel);
                }
                else
                {
                    staffCertificateInfoDelete._failure = true;
                    staffCertificateInfoDelete._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                staffCertificateInfoDelete._failure = true;
                staffCertificateInfoDelete._message = es.Message;
            }
            return staffCertificateInfoDelete;
        }

        /// <summary>
        /// Add or Update Staff Photo
        /// </summary>
        /// <param name="staffAddViewModel"></param>
        /// <returns></returns>
        public StaffAddViewModel AddUpdateStaffPhoto(StaffAddViewModel staffAddViewModel)
        {
            StaffAddViewModel staffPhotoUpdate = new StaffAddViewModel();
            if (tokenManager.CheckToken(staffAddViewModel._tenantName + staffAddViewModel._userName, staffAddViewModel._token))
            {
                staffPhotoUpdate = this.staffRepository.AddUpdateStaffPhoto(staffAddViewModel);
            }
            else
            {
                staffPhotoUpdate._failure = true;
                staffPhotoUpdate._message = TOKENINVALID;
            }
            return staffPhotoUpdate;
        }

        /// <summary>
        /// Add Staff List
        /// </summary>
        /// <param name="staffListAddViewModel"></param>
        /// <returns></returns>
        public StaffListAddViewModel AddStaffList(StaffListAddViewModel staffListAddViewModel)
        {
            StaffListAddViewModel staffListAdd = new StaffListAddViewModel();
            try
            {
                if (tokenManager.CheckToken(staffListAddViewModel._tenantName + staffListAddViewModel._userName, staffListAddViewModel._token))
                {
                    staffListAdd = this.staffRepository.AddStaffList(staffListAddViewModel);
                }
                else
                {
                    staffListAdd._failure = true;
                    staffListAdd._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                staffListAdd._failure = true;
                staffListAdd._message = es.Message;
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
            ScheduledCourseSectionViewModel staffCourseSectionView = new ScheduledCourseSectionViewModel();
            try
            {
                if (tokenManager.CheckToken(scheduledCourseSectionViewModel._tenantName + scheduledCourseSectionViewModel._userName, scheduledCourseSectionViewModel._token))
                {
                    staffCourseSectionView = this.staffRepository.GetScheduledCourseSectionsForStaff(scheduledCourseSectionViewModel);
                }
                else
                {
                    staffCourseSectionView._failure = true;
                    staffCourseSectionView._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                staffCourseSectionView._failure = true;
                staffCourseSectionView._message = es.Message;
            }
            return staffCourseSectionView;
        }

        public StaffListModel GetAllStaffListByDateRange(PageResult pageResult)
        {
            logger.Info("Method GetAllStaffListByDateRange called.");
            StaffListModel staffList = new StaffListModel();
            try
            {
                if (tokenManager.CheckToken(pageResult._tenantName + pageResult._userName, pageResult._token))
                {
                    staffList = this.staffRepository.GetAllStaffListByDateRange(pageResult);
                    if (staffList.staffMaster.Count > 0)
                    {
                        staffList._message = SUCCESS;
                        staffList._failure = false;
                    }
                    else
                    {
                        staffList._message = "NO RECORD FOUND";
                        staffList._failure = true;
                    }
                    logger.Info("Method getAllStaffList end with success.");
                }

                else
                {
                    staffList._failure = true;
                    staffList._message = TOKENINVALID;
                    return staffList;
                }
            }
            catch (Exception ex)
            {
                staffList._message = ex.Message;
                staffList._failure = true;
                logger.Error("Method GetAllStaffListByDateRange end with error :" + ex.Message);
            }
            return staffList;
        }

        /// <summary>
        /// Delete Staff
        /// </summary>
        /// <param name="staffDeleteViewModel"></param>
        /// <returns></returns>
        public StaffDeleteViewModel DeleteStaff(StaffDeleteViewModel staffDeleteViewModel)
        {
            StaffDeleteViewModel staffDelete = new();
            try
            {
                if (tokenManager.CheckToken(staffDeleteViewModel._tenantName + staffDeleteViewModel._userName, staffDeleteViewModel._token))
                {
                    staffDelete = this.staffRepository.DeleteStaff(staffDeleteViewModel);
                }
                else
                {
                    staffDelete._failure = true;
                    staffDelete._message = TOKENINVALID;
                }
            }
            catch (Exception ex)
            {
                staffDelete._message = ex.Message;
                staffDelete._failure = true;
            }
            return staffDelete;
        }
    }
}

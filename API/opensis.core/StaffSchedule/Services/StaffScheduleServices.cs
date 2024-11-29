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
using opensis.core.StaffSchedule.Interfaces;
using opensis.data.Interface;
using opensis.data.ViewModels.CourseManager;
using opensis.data.ViewModels.StaffSchedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.core.StaffSchedule.Services
{
    public class StaffScheduleServices : IStaffScheduleService
    {
        //private static string SUCCESS = "success";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public IStaffScheduleRepository staffScheduleRepository;
        public ICheckLoginSession tokenManager;
        public StaffScheduleServices(IStaffScheduleRepository staffScheduleRepository, ICheckLoginSession checkLoginSession)
        {
            this.staffScheduleRepository = staffScheduleRepository;
            this.tokenManager = checkLoginSession;
        }

        /// <summary>
        /// Teacher Schedule View For Course Section
        /// </summary>
        /// <param name="staffScheduleViewModel"></param>
        /// <returns></returns>
        public StaffScheduleViewModel StaffScheduleViewForCourseSection(StaffScheduleViewModel staffScheduleViewModel)
        {
            StaffScheduleViewModel staffSchedule = new StaffScheduleViewModel();
            try
            {
                if (tokenManager.CheckToken(staffScheduleViewModel._tenantName + staffScheduleViewModel._userName, staffScheduleViewModel._token))
                {
                    staffSchedule = this.staffScheduleRepository.StaffScheduleViewForCourseSection(staffScheduleViewModel);
                }
                else
                {
                    staffSchedule._failure = true;
                    staffSchedule._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                staffSchedule._failure = true;
                staffSchedule._message = es.Message;
            }
            return staffSchedule;
        }

        /// <summary>
        /// Add Staff Course Section Schedule
        /// </summary>
        /// <param name="staffScheduleViewModel"></param>
        /// <returns></returns>
        public StaffScheduleViewModel AddStaffCourseSectionSchedule(StaffScheduleViewModel staffScheduleViewModel)
        {
            StaffScheduleViewModel staffSchedule = new StaffScheduleViewModel();
            try
            {
                if (tokenManager.CheckToken(staffScheduleViewModel._tenantName + staffScheduleViewModel._userName, staffScheduleViewModel._token))
                {
                    staffSchedule = this.staffScheduleRepository.AddStaffCourseSectionSchedule(staffScheduleViewModel);
                }
                else
                {
                    staffSchedule._failure = true;
                    staffSchedule._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                staffSchedule._failure = true;
                staffSchedule._message = es.Message;
            }
            return staffSchedule;
        }

        /// <summary>
        /// Check Availability Staff Course Section Schedule
        /// </summary>
        /// <param name="staffScheduleViewModel"></param>
        /// <returns></returns>
        public StaffScheduleViewModel CheckAvailabilityStaffCourseSectionSchedule(StaffScheduleViewModel staffScheduleViewModel)
        {
            StaffScheduleViewModel staffSchedule = new StaffScheduleViewModel();
            try
            {
                if (tokenManager.CheckToken(staffScheduleViewModel._tenantName + staffScheduleViewModel._userName, staffScheduleViewModel._token))
                {
                    staffSchedule = this.staffScheduleRepository.CheckAvailabilityStaffCourseSectionSchedule(staffScheduleViewModel);
                }
                else
                {
                    staffSchedule._failure = true;
                    staffSchedule._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                staffSchedule._failure = true;
                staffSchedule._message = es.Message;
            }
            return staffSchedule;
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
                if (tokenManager.CheckToken(scheduledCourseSectionViewModel._tenantName + scheduledCourseSectionViewModel._userName, scheduledCourseSectionViewModel._token))
                {
                    scheduledCourseSectionView = this.staffScheduleRepository.GetAllScheduledCourseSectionForStaff(scheduledCourseSectionViewModel);
                }
                else
                {
                    scheduledCourseSectionView._failure = true;
                    scheduledCourseSectionView._message = TOKENINVALID;
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
        /// Add Staff Course Section ReSchedule
        /// </summary>
        /// <param name="staffScheduleViewModel"></param>
        /// <returns></returns>
        public StaffScheduleViewModel AddStaffCourseSectionReSchedule(StaffScheduleViewModel staffScheduleViewModel)
        {
            StaffScheduleViewModel staffSchedule = new StaffScheduleViewModel();
            try
            {
                if (tokenManager.CheckToken(staffScheduleViewModel._tenantName + staffScheduleViewModel._userName, staffScheduleViewModel._token))
                {
                    staffSchedule = this.staffScheduleRepository.AddStaffCourseSectionReSchedule(staffScheduleViewModel);
                }
                else
                {
                    staffSchedule._failure = true;
                    staffSchedule._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                staffSchedule._failure = true;
                staffSchedule._message = es.Message;
            }
            return staffSchedule;
        }

        /// <summary>
        /// Check Availability Staff Course Section ReSchedule
        /// </summary>
        /// <param name="staffListViewModel"></param>
        /// <returns></returns>
        public StaffListViewModel checkAvailabilityStaffCourseSectionReSchedule(StaffListViewModel staffListViewModel)
        {
            StaffListViewModel staffListView = new StaffListViewModel();
            try
            {
                if (tokenManager.CheckToken(staffListViewModel._tenantName + staffListViewModel._userName, staffListViewModel._token))
                {
                    staffListView = this.staffScheduleRepository.checkAvailabilityStaffCourseSectionReSchedule(staffListViewModel);
                }
                else
                {
                    staffListView._failure = true;
                    staffListView._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                staffListView._failure = true;
                staffListView._message = es.Message;
            }
            return staffListView;
        }

        /// <summary>
        ///  Add Staff Course Section Re-Schedule By Course Wise
        /// </summary>
        /// <param name="staffListViewModel"></param>
        /// <returns></returns>
        public StaffListViewModel AddStaffCourseSectionReScheduleByCourse(StaffListViewModel staffListViewModel)
        {
            StaffListViewModel staffListView = new StaffListViewModel();
            try
            {
                if (tokenManager.CheckToken(staffListViewModel._tenantName + staffListViewModel._userName, staffListViewModel._token))
                {
                    staffListView = this.staffScheduleRepository.AddStaffCourseSectionReScheduleByCourse(staffListViewModel);
                }
                else
                {
                    staffListView._failure = true;
                    staffListView._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                staffListView._failure = true;
                staffListView._message = es.Message;
            }
            return staffListView;
        }

        /// <summary>
        ///  Remove Staff Course
        /// </summary>
        /// <param name="removeStaffScheduleViewModel"></param>
        /// <returns></returns>
        public RemoveStaffScheduleViewModel RemoveStaffCourseSectionSchedule(RemoveStaffScheduleViewModel removeStaffScheduleViewModel)
        {
            RemoveStaffScheduleViewModel removeStaffScheduleView = new RemoveStaffScheduleViewModel();
            try
            {
                if (tokenManager.CheckToken(removeStaffScheduleViewModel._tenantName + removeStaffScheduleViewModel._userName, removeStaffScheduleViewModel._token))
                {
                    removeStaffScheduleView = this.staffScheduleRepository.RemoveStaffCourseSectionSchedule(removeStaffScheduleViewModel);
                }
                else
                {
                    removeStaffScheduleView._failure = true;
                    removeStaffScheduleView._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                removeStaffScheduleView._failure = true;
                removeStaffScheduleView._message = es.Message;
            }
            return removeStaffScheduleView;
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
                if (tokenManager.CheckToken(staffListViewModel._tenantName + staffListViewModel._userName, staffListViewModel._token))
                {
                    staffListView = this.staffScheduleRepository.GetUnassociatedStaffListByCourseSection(staffListViewModel);
                }
                else
                {
                    staffListView._failure = true;
                    staffListView._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                staffListView._failure = true;
                staffListView._message = es.Message;
            }
            return staffListView;
        }
    }
}

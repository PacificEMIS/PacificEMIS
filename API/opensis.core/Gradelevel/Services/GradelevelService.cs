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

using opensis.core.GradeLevel.Interfaces;
using opensis.core.helper;
using opensis.core.helper.Interfaces;
using opensis.data.Interface;
using opensis.data.ViewModels.Gradelevel;
using opensis.data.ViewModels.GradeLevel;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.core.GradeLevel.Services
{
    public class GradelevelService : IGradelevelService
    {
        //private static string SUCCESS = "success";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public IGradelevelRepository gradelevelRepository;
        public ICheckLoginSession tokenManager;
        public GradelevelService(IGradelevelRepository gradelevelRepository, ICheckLoginSession checkLoginSession)
        {
            this.gradelevelRepository = gradelevelRepository;
            this.tokenManager = checkLoginSession;
        }
        /// <summary>
        /// Add Grade Level
        /// </summary>
        /// <param name="gradelevel"></param>
        /// <returns></returns>
        public GradelevelViewModel AddGradelevel(GradelevelViewModel gradelevel)
        {
            GradelevelViewModel gradelevelViewModel = new GradelevelViewModel();
            try
            {
                if (tokenManager.CheckToken(gradelevel._tenantName + gradelevel._userName, gradelevel._token))
                {
                    gradelevelViewModel = this.gradelevelRepository.AddGradelevel(gradelevel);
                }
                else
                {
                    gradelevelViewModel._failure = true;
                    gradelevelViewModel._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                gradelevelViewModel._failure = true;
                gradelevelViewModel._message = es.Message;
            }
            return gradelevelViewModel;

        }
        
        /// <summary>
        /// Get Grade Level By Id
        /// </summary>
        /// <param name="gradelevel"></param>
        /// <returns></returns>
        public GradelevelViewModel ViewGradelevel(GradelevelViewModel gradelevel)
        {
            GradelevelViewModel gradelevelViewModel = new GradelevelViewModel();
            try
            {
                if (tokenManager.CheckToken(gradelevel._tenantName + gradelevel._userName, gradelevel._token))
                {
                    gradelevelViewModel = this.gradelevelRepository.ViewGradelevel(gradelevel);
                }
                else
                {
                    gradelevelViewModel._failure = true;
                    gradelevelViewModel._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                gradelevelViewModel._failure = true;
                gradelevelViewModel._message = es.Message;
            }
            return gradelevelViewModel;
        }
        
        /// <summary>
        /// Update Grade Level
        /// </summary>
        /// <param name="gradelevel"></param>
        /// <returns></returns>
        public GradelevelViewModel UpdateGradelevel(GradelevelViewModel gradelevel)
        {
            GradelevelViewModel gradelevelUpdate = new GradelevelViewModel();
            try
            {
                if (tokenManager.CheckToken(gradelevel._tenantName + gradelevel._userName, gradelevel._token))
                {
                    gradelevelUpdate = this.gradelevelRepository.UpdateGradelevel(gradelevel);
                }
                else
                {
                    gradelevelUpdate._failure = true;
                    gradelevelUpdate._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                gradelevelUpdate._failure = true;
                gradelevelUpdate._message = es.Message;
            }

            return gradelevelUpdate;
        }
        
        /// <summary>
        /// Delete Grade Level
        /// </summary>
        /// <param name="gradelevel"></param>
        /// <returns></returns>
        public GradelevelViewModel DeleteGradelevel(GradelevelViewModel gradelevel)
        {
            GradelevelViewModel gradelevelDelete = new GradelevelViewModel();
            try
            {
                if (tokenManager.CheckToken(gradelevel._tenantName + gradelevel._userName, gradelevel._token))
                {
                    gradelevelDelete = this.gradelevelRepository.DeleteGradelevel(gradelevel);
                }
                else
                {
                    gradelevelDelete._failure = true;
                    gradelevelDelete._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                gradelevelDelete._failure = true;
                gradelevelDelete._message = es.Message;
            }

            return gradelevelDelete;
        }
        
        /// <summary>
        /// Get All Grade Level
        /// </summary>
        /// <param name="gradelevel"></param>
        /// <returns></returns>
        public GradelevelListViewModel GetAllGradeLevels(GradelevelListViewModel gradelevel)
        {
            GradelevelListViewModel gradelevelList = new GradelevelListViewModel();
            try
            {
                if (tokenManager.CheckToken(gradelevel._tenantName + gradelevel._userName, gradelevel._token))
                {
                    gradelevelList = this.gradelevelRepository.GetAllGradeLevels(gradelevel);
                }
                else
                {
                    gradelevelList._failure = true;
                    gradelevelList._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                gradelevelList._failure = true;
                gradelevelList._message = es.Message;
            }

            return gradelevelList;
        }

        /// <summary>
        /// Get All Grade Equivalency
        /// </summary>
        /// <param name="gradeEquivalencyListModel"></param>
        /// <returns></returns>
        public GradeEquivalencyListViewModel GetAllGradeEquivalency(GradeEquivalencyListViewModel gradeEquivalencyListModel)
        {
            GradeEquivalencyListViewModel gradeEquivalencyListViewModel = new GradeEquivalencyListViewModel();
            try
            {
                //gradeEquivalencyListViewModel = this.gradelevelRepository.GetAllGradeEquivalency(gradeEquivalencyListModel);
                //return gradeEquivalencyListViewModel;
                if (tokenManager.CheckToken(gradeEquivalencyListModel._tenantName + gradeEquivalencyListModel._userName, gradeEquivalencyListModel._token))
                {
                    gradeEquivalencyListViewModel = this.gradelevelRepository.GetAllGradeEquivalency(gradeEquivalencyListModel);
                }
                else
                {
                    gradeEquivalencyListViewModel._failure = true;
                    gradeEquivalencyListViewModel._message = TOKENINVALID;
                }
            }
            catch (Exception ex)
            {

                gradeEquivalencyListViewModel._failure = true;
                gradeEquivalencyListViewModel._message = ex.Message;
                
            }
            return gradeEquivalencyListViewModel;

        }

        /// <summary>
        /// UpdateGradeLevelSortOrder
        /// </summary>
        /// <param name="gradelevelSortOrderViewModel"></param>
        /// <returns></returns>
        public GradelevelSortOrderViewModel UpdateGradeLevelSortOrder(GradelevelSortOrderViewModel gradelevelSortOrderViewModel)
        {
            GradelevelSortOrderViewModel gradelevelSortOrder = new();
            try
            {
                if (tokenManager.CheckToken(gradelevelSortOrderViewModel._tenantName + gradelevelSortOrderViewModel._userName, gradelevelSortOrderViewModel._token))
                {
                    gradelevelSortOrder = this.gradelevelRepository.UpdateGradeLevelSortOrder(gradelevelSortOrderViewModel);
                }
                else
                {
                    gradelevelSortOrder._failure = true;
                    gradelevelSortOrder._message = TOKENINVALID;
                }
            }
            catch (Exception ex)
            {
                gradelevelSortOrder._failure = true;
                gradelevelSortOrder._message = ex.Message;
            }
            return gradelevelSortOrder;
        }
    }
}

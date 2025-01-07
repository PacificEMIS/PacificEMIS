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
using opensis.data.ViewModels.Gradelevel;
using opensis.data.ViewModels.GradeLevel;
using opensis.data.ViewModels.School;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
    public class GradeLevelRepository : IGradelevelRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public GradeLevelRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Add Grade Level
        /// </summary>
        /// <param name="gradelevel"></param>
        /// <returns></returns>
        public GradelevelViewModel AddGradelevel(GradelevelViewModel gradelevel)
        {
            if(gradelevel.TblGradelevel is null)
            {
                return gradelevel;
            }
            try
            {
                var checkGradelevelTitle = this.context?.Gradelevels.AsEnumerable().Where(x => x.SchoolId == gradelevel.TblGradelevel.SchoolId && x.TenantId == gradelevel.TblGradelevel.TenantId && String.Compare(x.Title,gradelevel.TblGradelevel.Title,true)==0).FirstOrDefault();

                if (checkGradelevelTitle !=null)
                {
                    gradelevel._failure = true;
                    gradelevel._message = "Grade Level Title already exists";
                }
                else
                {
                    int? GradeLevelId = Utility.GetMaxPK(this.context, new Func<Gradelevels, int>(x => x.GradeId));
                    gradelevel.TblGradelevel.GradeId = (int)GradeLevelId!;
                    gradelevel.TblGradelevel.CreatedOn = DateTime.UtcNow;
                    // gradelevel.TblGradelevel.SchoolMaster = null;
                   // this.context?.Entry(GradeLevels).State = EntityState.Detached;
                    this.context?.Gradelevels.Add(gradelevel.TblGradelevel);
                    //context!.Entry(gradelevel.TblGradelevel.SchoolMaster).State = EntityState.Unchanged;
                    this.context?.SaveChanges();
                    gradelevel._failure = false;
                    gradelevel._message = "Grade Level Added Successsfully";
                }                
            }
            catch (Exception es)
            {
                gradelevel._failure = true;
                gradelevel._message = es.Message;
            }
            return gradelevel;

        }

        /// <summary>
        /// Get Grade Level By Id
        /// </summary>
        /// <param name="gradelevel"></param>
        /// <returns></returns>
        public GradelevelViewModel ViewGradelevel(GradelevelViewModel gradelevel)
        {
            if (gradelevel.TblGradelevel is null)
            {
                return gradelevel;
            }
            GradelevelViewModel gradelevelModel = new ();
            try
            {
                var Gradelevel = this.context?.Gradelevels.FirstOrDefault(x => x.TenantId == gradelevel.TblGradelevel.TenantId && x.SchoolId == gradelevel.TblGradelevel.SchoolId && x.GradeId == gradelevel.TblGradelevel.GradeId);
                if (Gradelevel != null)
                {
                    gradelevelModel.TblGradelevel = Gradelevel;
                    gradelevelModel._failure = false;
                }
                else
                {
                    gradelevelModel._failure = true;
                    gradelevelModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                gradelevelModel._failure = true;
                gradelevelModel._message = es.Message;
            }
            return gradelevelModel;

        }
        
        /// <summary>
        /// Update Grade Level
        /// </summary>
        /// <param name="gradelevel"></param>
        /// <returns></returns>
        public GradelevelViewModel UpdateGradelevel(GradelevelViewModel gradelevel)
        {
            if (gradelevel.TblGradelevel is null)
            {
                return gradelevel;
            }
            GradelevelViewModel gradelevelUpdate = new ();
            try
            {
                var GradeLevel = this.context?.Gradelevels.FirstOrDefault(x => x.TenantId == gradelevel.TblGradelevel.TenantId && x.SchoolId == gradelevel.TblGradelevel.SchoolId && x.GradeId == gradelevel.TblGradelevel.GradeId);

                if (GradeLevel != null)
                {
                    var checkGradelevelTitle = this.context?.Gradelevels.AsEnumerable().Where(x => x.SchoolId == gradelevel.TblGradelevel.SchoolId && x.TenantId == gradelevel.TblGradelevel.TenantId && x.GradeId != gradelevel.TblGradelevel.GradeId && String.Compare(x.Title , gradelevel.TblGradelevel.Title,true)==0).FirstOrDefault();

                    if (checkGradelevelTitle != null)
                    {
                        gradelevel._failure = true;
                        gradelevel._message = "Grade Level Title already exists";
                    }
                    else
                    {
                        if (GradeLevel.Title != gradelevel.TblGradelevel.Title)
                        {
                            var gradeTitleUsed = this.context?.GradeUsStandard.AsEnumerable().Where(x => x.SchoolId == gradelevel.TblGradelevel.SchoolId && x.TenantId == gradelevel.TblGradelevel.TenantId && String.Compare(x.GradeLevel,GradeLevel.Title,true)==0).ToList();

                            if (gradeTitleUsed?.Count>0)
                            {
                                gradeTitleUsed.ForEach(x => x.GradeLevel = gradelevel.TblGradelevel.Title);
                            }

                            var gradeTitleUsedInCourse = this.context?.Course.AsEnumerable().Where(x => x.SchoolId == gradelevel.TblGradelevel.SchoolId && x.TenantId == gradelevel.TblGradelevel.TenantId && String.Compare(x.CourseGradeLevel,GradeLevel.Title,true)==0).ToList();

                            if (gradeTitleUsedInCourse?.Any()==true)
                            {
                                gradeTitleUsedInCourse.ForEach(x => x.CourseGradeLevel = gradelevel.TblGradelevel.Title);
                            }

                            var gradeTitleUsedInStudentEnrollment = this.context?.StudentEnrollment.AsEnumerable().Where(x => x.SchoolId == gradelevel.TblGradelevel.SchoolId && x.TenantId == gradelevel.TblGradelevel.TenantId && String.Compare(x.GradeLevelTitle ,GradeLevel.Title,true)==0).ToList();

                            if (gradeTitleUsedInStudentEnrollment?.Any()==true)
                            {
                                gradeTitleUsedInStudentEnrollment.ForEach(x => x.GradeLevelTitle = gradelevel.TblGradelevel.Title);
                            }

                            var gradeTitleUsedInStaff = this.context?.StaffMaster.AsEnumerable().Where(x => x.SchoolId == gradelevel.TblGradelevel.SchoolId && x.TenantId == gradelevel.TblGradelevel.TenantId && String.Compare(x.PrimaryGradeLevelTaught,GradeLevel.Title,true)==0).ToList();

                            if (gradeTitleUsedInStaff?.Any()==true)
                            {
                                gradeTitleUsedInStaff.ForEach(x => x.PrimaryGradeLevelTaught = gradelevel.TblGradelevel.Title) ;
                            }

                            var StaffData = this.context?.StaffMaster.Where(x => x.SchoolId == gradelevel.TblGradelevel.SchoolId && x.TenantId == gradelevel.TblGradelevel.TenantId && x.OtherGradeLevelTaught!.Contains(GradeLevel.Title!=null?GradeLevel.Title:"")).ToList();

                            if (StaffData?.Any()==true)
                            {
                                foreach (var staff in StaffData)
                                {
                                    var otherGradeLevelTaught = (staff.OtherGradeLevelTaught??"").Split(",");
                                    otherGradeLevelTaught = otherGradeLevelTaught.Where(w => w != GradeLevel.Title).ToArray();
                                    var newOtherGradeLevelTaught = string.Join(",", otherGradeLevelTaught);
                                    newOtherGradeLevelTaught = newOtherGradeLevelTaught + "," + gradelevel.TblGradelevel.Title;
                                    staff.OtherSubjectTaught = newOtherGradeLevelTaught;
                                }
                            }

                        }

                        gradelevel.TblGradelevel.UpdatedOn = DateTime.Now;
                        gradelevel.TblGradelevel.CreatedBy = GradeLevel.CreatedBy;
                        gradelevel.TblGradelevel.CreatedOn = GradeLevel.CreatedOn;
                        this.context?.Entry(GradeLevel).CurrentValues.SetValues(gradelevel.TblGradelevel);
                        this.context?.SaveChanges();
                        gradelevel._failure = false;
                        gradelevel._message = "Grade Level Updated Successsfully";
                    }
                }
                else
                {
                    gradelevel.TblGradelevel = null;
                    gradelevel._failure = true;
                    gradelevel._message = NORECORDFOUND;
                }
            }
            catch(Exception es)
            {
                gradelevel._failure = true;
                gradelevel._message = es.Message;
            }
            return gradelevel;
        }
        
        /// <summary>
        /// Delete Grade Level
        /// </summary>
        /// <param name="gradelevel"></param>
        /// <returns></returns>
        public GradelevelViewModel DeleteGradelevel(GradelevelViewModel gradelevel)
        {
            if(gradelevel.TblGradelevel is null)
            {
                return gradelevel;
            }
            try
            {
                var LinkedGradeLevels = this.context?.Gradelevels.Where(x => x.TenantId == gradelevel.TblGradelevel.TenantId && x.SchoolId == gradelevel.TblGradelevel.SchoolId && x.NextGradeId == gradelevel.TblGradelevel.GradeId).ToList();

                var studentEnrollmentData = this.context?.StudentEnrollment.Where(x => x.TenantId == gradelevel.TblGradelevel.TenantId && x.SchoolId == gradelevel.TblGradelevel.SchoolId && x.GradeId == gradelevel.TblGradelevel.GradeId).ToList();
                if (LinkedGradeLevels?.Count>0 || studentEnrollmentData?.Count > 0)
                {
                    gradelevel.TblGradelevel = null;
                    gradelevel._failure = true;
                    gradelevel._message = "Grade Level cannot be deleted because it has its association";
                }
                else
                {
                    var GradeLevel = this.context?.Gradelevels.FirstOrDefault(x => x.TenantId == gradelevel.TblGradelevel.TenantId && x.SchoolId == gradelevel.TblGradelevel.SchoolId && x.GradeId == gradelevel.TblGradelevel.GradeId);
                    this.context?.Gradelevels.Remove(GradeLevel!);
                    this.context?.SaveChanges();
                    gradelevel._failure = false;
                    gradelevel._message = "Grade Level Deleted Successsfully";
                }
            }
            catch (Exception es)
            {
                gradelevel._failure = true;
                gradelevel._message = es.Message;
            }
            return gradelevel;
        }
        
        /// <summary>
        /// Get All Grade Level
        /// </summary>
        /// <param name="gradelevelList"></param>
        /// <returns></returns>
        public GradelevelListViewModel GetAllGradeLevels(GradelevelListViewModel gradelevelList)
        {
            GradelevelListViewModel gradelevelListModel = new ();
            try
            {

                var gradelevelsList = this.context?.Gradelevels.Include(x=>x.Equivalency)
                    .Where(x => x.TenantId == gradelevelList.TenantId && x.SchoolId==gradelevelList.SchoolId).OrderBy(x=>x.SortOrder).ToList();

                if (gradelevelsList?.Count > 0)
                {
                    var gradeLevels = from gradelevel in gradelevelsList
                                      select new GradeLevelView()
                                      {
                                          GradeId = gradelevel.GradeId,
                                          UpdatedOn = gradelevel.UpdatedOn,
                                          CreatedOn=gradelevel.CreatedOn,
                                          NextGrade = this.context?.Gradelevels.FirstOrDefault(x => x.GradeId == gradelevel.NextGradeId)?.Title,
                                          NextGradeId = gradelevel.NextGradeId,
                                          SchoolId = gradelevel.SchoolId,
                                          Title = gradelevel.Title,
                                          ShortName = gradelevel.ShortName,
                                          SortOrder = gradelevel.SortOrder,
                                          TenantId = gradelevel.TenantId,
                                          GradeLevelEquivalency = gradelevel.Equivalency?.GradeLevelEquivalency,
                                          //GradeLevelEquivalency = gradelevel.Equivalency != null? gradelevel.Equivalency.GradeLevelEquivalency : null,
                                          EquivalencyId = gradelevel.EquivalencyId,
                                          AgeRangeId = gradelevel.AgeRangeId,
                                          IscedCode = gradelevel.IscedCode,
                                          //GradeDescription = gradelevel.IscedGradeLevelNavigation != null ? gradelevel.IscedGradeLevelNavigation.GradeDescription : null,
                                          //AgeRange=gradelevel.AgeRange,
                                          //EducationalStage=gradelevel.EducationalStage,
                                          //GradeLevelEquivalency=gradelevel.GradeLevelEquivalency,
                                          CreatedBy=gradelevel.CreatedBy,
                                          UpdatedBy=gradelevel.UpdatedBy
                                      };                   
                    
                    gradelevelListModel.TableGradelevelList = gradeLevels.ToList();
                    if (gradelevelList.IsListView == true)
                    {
                        gradelevelListModel.TableGradelevelList.ForEach(e =>
                        {
                            e.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, gradelevelList.TenantId, e.CreatedBy);
                            e.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, gradelevelList.TenantId, e.UpdatedBy);
                        });
                    }
                    gradelevelListModel._tenantName = gradelevelList._tenantName;
                    gradelevelListModel._token = gradelevelList._token;
                    gradelevelListModel._failure = false;
                }
                else
                {
                    gradelevelListModel._tenantName = gradelevelList._tenantName;
                    gradelevelListModel._token = gradelevelList._token;
                    gradelevelListModel._failure = true;
                    gradelevelListModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                gradelevelListModel.TableGradelevelList = null!;
                gradelevelListModel._message = es.Message;
                gradelevelListModel._failure = true;
                gradelevelListModel._tenantName = gradelevelList._tenantName;
                gradelevelListModel._token = gradelevelList._token;
            }
            return gradelevelListModel;

        }
        
        /// <summary>
        /// Get All Grade Equivalency
        /// </summary>
        /// <param name="gradeEquivalencyList"></param>
        /// <returns></returns>
        public GradeEquivalencyListViewModel GetAllGradeEquivalency(GradeEquivalencyListViewModel gradeEquivalencyList)
        {
            GradeEquivalencyListViewModel gradeEquivalencyListModel = new ();
            try
            {
                var gradeEquivalency = this.context?.GradeEquivalency.ToList();
                
                //gradeEquivalencyListModel.GradeEquivalencyList = gradeEquivalency;
                gradeEquivalencyListModel._tenantName = gradeEquivalencyList._tenantName;
                gradeEquivalencyListModel._token = gradeEquivalencyList._token;

                if (gradeEquivalency?.Count > 0)
                {
                    gradeEquivalencyListModel.GradeEquivalencyList = gradeEquivalency;
                    gradeEquivalencyListModel._failure = false;
                }
                else
                {
                    gradeEquivalencyListModel._failure = true;
                    gradeEquivalencyListModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                gradeEquivalencyListModel._message = es.Message;
                gradeEquivalencyListModel._failure = true;
                gradeEquivalencyListModel._tenantName = gradeEquivalencyList._tenantName;
                gradeEquivalencyListModel._token = gradeEquivalencyList._token;
            }
            return gradeEquivalencyListModel;

        }

        /// <summary>
        /// UpdateGradeLevelSortOrder
        /// </summary>
        /// <param name="gradelevelSortOrderViewModel"></param>
        /// <returns></returns>
        public GradelevelSortOrderViewModel UpdateGradeLevelSortOrder(GradelevelSortOrderViewModel gradelevelSortOrderViewModel)
        {
            try
            {
                var gradelevelList = this.context?.Gradelevels.Where(a => a.TenantId == gradelevelSortOrderViewModel.TenantId && a.SchoolId == gradelevelSortOrderViewModel.SchoolId);

                if (gradelevelList?.Any() == true)
                {
                    var sortOrderValues = gradelevelSortOrderViewModel.SortOrderValues.OrderBy(c => c.Id);
                    foreach (var sortOrderValue in sortOrderValues)
                    {
                        var gradelevelData = gradelevelList.FirstOrDefault(d => d.TenantId == gradelevelSortOrderViewModel.TenantId && d.SchoolId == gradelevelSortOrderViewModel.SchoolId && d.GradeId == sortOrderValue.Id);

                        if (gradelevelData != null)
                        {
                            var oldGradelevelData = gradelevelData;
                            gradelevelData.SortOrder = sortOrderValue.SortOrder;
                            gradelevelData.UpdatedOn = DateTime.UtcNow;
                            gradelevelData.UpdatedBy = gradelevelSortOrderViewModel.UpdatedBy;
                            this.context?.Entry(oldGradelevelData).CurrentValues.SetValues(gradelevelData);
                        }
                    }
                    this.context?.SaveChanges();
                    gradelevelSortOrderViewModel._failure = false;
                    gradelevelSortOrderViewModel._message = "Sort order updated successfully";
                }
                else
                {
                    gradelevelSortOrderViewModel._failure = true;
                    gradelevelSortOrderViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                gradelevelSortOrderViewModel._message = es.Message;
                gradelevelSortOrderViewModel._failure = true;
            }
            return gradelevelSortOrderViewModel;
        }
    }
}

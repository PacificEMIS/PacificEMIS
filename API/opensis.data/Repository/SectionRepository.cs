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
using opensis.data.ViewModels.Section;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
    public class SectionRepository : ISectionRepositiory
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public SectionRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }
        /// <summary>
        ///Add Section
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public SectionAddViewModel AddSection(SectionAddViewModel section)
        {
            if (section.tableSections is null)
            {
                return section;
            }
            try
            {
                var checkSectionName = this.context?.Sections.AsEnumerable().Where(x => x.SchoolId == section.tableSections.SchoolId && x.TenantId == section.tableSections.TenantId && String.Compare(x.Name , section.tableSections.Name,true)==0
                 ).FirstOrDefault();

                if (checkSectionName != null)
                {
                    section._failure = true;
                    section._message = "Section Name already exists";
                }
                else
                {
                    //int? MasterSectionId = Utility.GetMaxPK(this.context, new Func<Sections, int>(x => x.SectionId));
                    int? MasterSectionId = 1;

                    var SectionData = this.context?.Sections.Where(x => x.SchoolId == section.tableSections.SchoolId && x.TenantId == section.tableSections.TenantId).OrderByDescending(x => x.SectionId).FirstOrDefault();

                    if (SectionData != null)
                    {
                        MasterSectionId = SectionData.SectionId + 1;
                    }

                    if(section.tableSections != null)
                    {
                        section.tableSections.SectionId = (int)MasterSectionId;
                        section.tableSections.CreatedOn = DateTime.UtcNow;
                        this.context?.Sections.Add(section.tableSections);
                        this.context?.SaveChanges();
                        section._failure = false;
                        section._message = "Section added successfully";
                    }
                }                
            }
            catch(Exception es)
            {
                section._failure = true;
                section._message = es.Message;
            }            
            return section;
        }

       /// <summary>
       /// Update Section
       /// </summary>
       /// <param name="section"></param>
       /// <returns></returns>
        public SectionAddViewModel UpdateSection(SectionAddViewModel section)
        {
            if (section.tableSections is null)
            {
                return section;
            }
            try
            {
                var sectionUpdate = this.context?.Sections.FirstOrDefault(x => x.TenantId == section.tableSections.TenantId && x.SchoolId == section.tableSections.SchoolId && x.SectionId == section.tableSections.SectionId);
                if (sectionUpdate!=null)
                {
                    var checkSectionName = this.context?.Sections.AsEnumerable().Where(x => x.SchoolId == section.tableSections.SchoolId && x.TenantId == section.tableSections.TenantId && x.SectionId!=section.tableSections.SectionId && String.Compare(x.Name, section.tableSections.Name, true) == 0).FirstOrDefault();

                    if (checkSectionName != null)
                    {
                        section._failure = true;
                        section._message = "Section Name already exists";
                    }
                    else
                    {
                        if(section.tableSections != null && sectionUpdate != null)
                        {
                            section.tableSections.UpdatedOn = DateTime.UtcNow;
                            section.tableSections.CreatedOn = sectionUpdate.CreatedOn;
                            section.tableSections.CreatedBy = sectionUpdate.CreatedBy;
                            this.context?.Entry(sectionUpdate).CurrentValues.SetValues(section.tableSections);
                            this.context?.SaveChanges();
                            section._failure = false;
                            section._message = "Section Updated Successfully";
                        }
                    }                    
                }
                else
                {
                    section.tableSections = null;
                    section._failure = true;
                    section._message = NORECORDFOUND;
                }                
            }
            catch (Exception ex)
            {
                section._failure = true;
                section._message = ex.Message;
            }
            return section;
        }

        /// <summary>
        /// Get Section By ID
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public SectionAddViewModel ViewSection(SectionAddViewModel section)
        {
            if (section.tableSections is null)
            {
                return section;
            }
            SectionAddViewModel sectionView = new SectionAddViewModel();
            try
            {
                var sectionById = this.context?.Sections.FirstOrDefault(x => x.TenantId == section.tableSections.TenantId && x.SchoolId == section.tableSections.SchoolId && x.SectionId== section.tableSections.SectionId);
                if (sectionById != null)
                {
                    sectionView.tableSections = sectionById;
                    return sectionView;
                }
                else
                {
                    sectionView._failure = true;
                    sectionView._message = NORECORDFOUND;
                    return sectionView;
                }
            }
            catch (Exception es)
            {
                sectionView._failure = true;
                sectionView._message = es.Message;
                return sectionView;
            }
        }

        /// <summary>
        /// Get All Section
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public SectionListViewModel GetAllsection(SectionListViewModel section)
        {
            SectionListViewModel sectionList = new SectionListViewModel();
            try
            {
                var sectionAll = this.context?.Sections.Where(x => x.TenantId == section.TenantId && x.SchoolId == section.SchoolId).OrderBy(x => x.SortOrder).ToList();

                if (sectionAll != null && sectionAll.Any())
                {
                    if (section.IsListView == true)
                    {
                        sectionAll.ForEach(c =>
                        {
                            c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, section.TenantId, c.CreatedBy != null ? c.CreatedBy : "");
                            c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, section.TenantId, c.UpdatedBy != null ? c.UpdatedBy : "");
                        });
                    }

                    sectionList.tableSectionsList = sectionAll;
                    sectionList._tenantName = section._tenantName;
                    sectionList._token = section._token;
                    sectionList._failure = false;
                }
                else
                {
                    sectionList._failure = true;
                    sectionList._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                sectionList._message = es.Message;
                sectionList._failure = true;
                sectionList._tenantName = section._tenantName;
                sectionList._token = section._token;
            }
            return sectionList;

        }
        
        /// <summary>
        /// Delete Section
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public SectionAddViewModel DeleteSection(SectionAddViewModel section)
        {
            if (section.tableSections is null)
            {
                return section;
            }
            try
            {
                var sectionDel = this.context?.Sections.FirstOrDefault(x => x.TenantId == section.tableSections.TenantId && x.SchoolId == section.tableSections.SchoolId && x.SectionId == section.tableSections.SectionId);

                if(sectionDel != null)
                {
                    this.context?.Sections.Remove(sectionDel);
                    this.context?.SaveChanges();
                    section._failure = false;
                    section._message = "Section deleted successfullyy";
                }
            }
            catch (Exception es)
            {
                section._failure = true;
                section._message = es.Message;
            }
            return section;
        }

        /// <summary>
        /// Update Section Sort Order
        /// </summary>
        /// <param name="sectionSortOrderViewModel"></param>
        /// <returns></returns>
        public SectionSortOrderViewModel UpdateSectionSortOrder(SectionSortOrderViewModel sectionSortOrderViewModel)
        {
            try
            {
                var sectionList = this.context?.Sections.Where(a => a.TenantId == sectionSortOrderViewModel.TenantId && a.SchoolId == sectionSortOrderViewModel.SchoolId);

                if (sectionList?.Any() == true)
                {
                    var sortOrderValues = sectionSortOrderViewModel.SortOrderValues.OrderBy(c => c.Id);
                    foreach (var sortOrderValue in sortOrderValues)
                    {
                        var sectionData = sectionList.FirstOrDefault(d => d.TenantId == sectionSortOrderViewModel.TenantId && d.SchoolId == sectionSortOrderViewModel.SchoolId && d.SectionId == sortOrderValue.Id);

                        if (sectionData != null)
                        {
                            var oldSectionData = sectionData;
                            sectionData.SortOrder = sortOrderValue.SortOrder;
                            sectionData.UpdatedOn = DateTime.UtcNow;
                            sectionData.UpdatedBy = sectionSortOrderViewModel.UpdatedBy;
                            this.context?.Entry(oldSectionData).CurrentValues.SetValues(sectionData);
                        }
                    }
                    this.context?.SaveChanges();
                    sectionSortOrderViewModel._failure = false;
                    sectionSortOrderViewModel._message = "Sort order updated successfully";
                }
                else
                {
                    sectionSortOrderViewModel._failure = true;
                    sectionSortOrderViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                sectionSortOrderViewModel._message = es.Message;
                sectionSortOrderViewModel._failure = true;
            }
            return sectionSortOrderViewModel;
        }

    }
}

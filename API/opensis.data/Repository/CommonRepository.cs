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

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using opensis.catalogdb.Interface;
using opensis.catalogdb.Models;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.CommonModel;
using opensis.data.ViewModels.Membership;
using opensis.data.ViewModels.StaffSchedule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace opensis.data.Repository
{
    public class CommonRepository : ICommonRepository
    {
        private readonly CRMContext? context;
        //private readonly CatalogDBContext catdbContext;
        private static readonly string NORECORDFOUND = "No Record Found";
       
       
        public CommonRepository(IDbContextFactory dbContextFactory/*, ICatalogDBContextFactory catdbContextFactory*/)
        {
            this.context = dbContextFactory.Create();
            //this.catdbContext = catdbContextFactory.Create();
            
        }

        /// <summary>
        /// Get All Cities By State
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public CityListModel GetAllCitiesByState(CityListModel city)
        {
            CityListModel cityListModel = new();
            try
            {
                var CityList = this.context?.City.Where(x => x.StateId == city.StateId).ToList();
                
                if (CityList !=null && CityList.Any())
                {
                    cityListModel.TableCity = CityList;
                }
                cityListModel._tenantName = city._tenantName;
                cityListModel._token = city._token;
                cityListModel._failure = false;
            }
            catch (Exception es)
            {
                cityListModel._message = es.Message;
                cityListModel._failure = true;
                cityListModel._tenantName = city._tenantName;
                cityListModel._token = city._token;
            }
            return cityListModel;

        }

        /// <summary>
        /// Get All States By Country
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public StateListModel GetAllStatesByCountry(StateListModel state)
        {
            StateListModel stateListModel = new();
            try
            {
                stateListModel.TableState = null;
                var StateList = this.context?.State.Where(x => x.CountryId == state.CountryId).ToList();
                
                if (StateList !=null && StateList.Any())
                {
                    stateListModel.TableState = StateList;
                }
                stateListModel._tenantName = state._tenantName;
                stateListModel._token = state._token;
                stateListModel._failure = false;
            }
            catch (Exception es)
            {
                stateListModel._message = es.Message;
                stateListModel._failure = true;
                stateListModel._tenantName = state._tenantName;
                stateListModel._token = state._token;
            }
            return stateListModel;

        }

        /// <summary>
        /// Get All Countries
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public CountryListModel GetAllCountries(PageResult pageResult)
        {
            CountryListModel countryListModel = new ();
            try
            {
                IQueryable<Country>? transactionIQ =null!;
                var CountryList = this.context?.Country;

                if (CountryList !=null && CountryList.Any())
                {
                    countryListModel._failure = false;
                    countryListModel._message = "success";

                    int? StateCount = this.context?.State.Count();
                    countryListModel.StateCount = StateCount;

                    if (pageResult.IsListView == true)
                    {
                        if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                        {
                            transactionIQ = CountryList;
                        }
                        else
                        {
                            string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;

                            if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                            {
                                transactionIQ = CountryList.Where(x => x.Name != null && x.Name.ToLower().Contains(Columnvalue.ToLower()) || x.CountryCode != null && x.CountryCode.ToLower().Contains(Columnvalue.ToLower()));
                            }
                        }
                        if (pageResult.SortingModel != null)
                        {
                            transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn??"", pageResult.SortingModel.SortDirection.ToLower());
                        }

                        int? totalCount = transactionIQ?.Count();
                        if (totalCount > 0)
                        {
                            if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                            {
                                transactionIQ = transactionIQ!.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                            }
                            if(transactionIQ !=null)
                            {
                                countryListModel.TableCountry = transactionIQ.ToList();

                                countryListModel.TableCountry.ForEach(c =>
                                {
                                    c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context!, pageResult.TenantId, c.CreatedBy!);
                                    c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context!, pageResult.TenantId, c.UpdatedBy!);
                                });
                                countryListModel.TotalCount = totalCount;
                            }
                                                    
                        }
                        else
                        {
                            countryListModel._failure = true;
                            countryListModel._message = NORECORDFOUND;
                        }
                    }
                    else
                    {
                        countryListModel.TableCountry = CountryList.ToList();
                    }                   
                    countryListModel.PageNumber = pageResult.PageNumber;
                    countryListModel._pageSize = pageResult.PageSize;
                }
                else
                {
                    countryListModel._failure = true;
                    countryListModel._message = NORECORDFOUND;
                }
                countryListModel._tenantName = pageResult._tenantName;
                countryListModel._token = pageResult._token;
                countryListModel.IsListView = pageResult.IsListView;
            }
            catch (Exception es)
            {
                countryListModel._message = es.Message;
                countryListModel._failure = true;
                countryListModel._tenantName = pageResult._tenantName;
                countryListModel._token = pageResult._token;
            }
            return countryListModel;

        }

        /// <summary>
        /// Add Language 
        /// </summary>
        /// <param name="languageAdd"></param>
        /// <returns></returns>
        public LanguageAddModel AddLanguage(LanguageAddModel languageAdd)
        {
            try
            {
                var getLanguage = this.context?.Language.AsEnumerable().FirstOrDefault(x =>String.Compare( x.Locale,languageAdd.Language!.Locale,true)==0);

                if (getLanguage != null)
                {
                    languageAdd._message = "This Language Name Already Exist";
                    languageAdd._failure = true;
                }
                else
                {
                    int? languageId = Utility.GetMaxPK(this.context!, new Func<Language, int>(x => x.LangId));
                    languageAdd.Language!.LangId = (int)languageId!;
                    languageAdd.Language.CreatedOn = DateTime.UtcNow;
                    this.context?.Language.Add(languageAdd.Language);
                    this.context?.SaveChanges();
                    languageAdd._failure = false;
                    languageAdd._message = "Language added successfully";
                }
            }
            catch (Exception es)
            {
                languageAdd._message = es.Message;
                languageAdd._failure = true;
                languageAdd._tenantName = languageAdd._tenantName;
                languageAdd._token = languageAdd._token;
            }
            return languageAdd;
        }

        /// <summary>
        /// Get Language By Id 
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public LanguageAddModel ViewLanguage(LanguageAddModel language)
        {
            try
            {
                LanguageAddModel languageAddModel = new ();
                var getLanguageValue = this.context?.Language.FirstOrDefault(x => x.LangId == language.Language!.LangId);
                if (getLanguageValue != null)
                {
                    languageAddModel.Language = getLanguageValue;
                    languageAddModel._failure = false;
                    return languageAddModel;
                }
                else
                {
                    languageAddModel._failure = true;
                    languageAddModel._message = NORECORDFOUND;
                    return languageAddModel;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Update Language
        /// </summary>
        /// <param name="languageUpdate"></param>
        /// <returns></returns>
        public LanguageAddModel UpdateLanguage(LanguageAddModel languageUpdate)
        {
            var getLanguage = this.context?.Language.AsEnumerable().Where(x => 
            String.Compare(x.Locale, languageUpdate.Language!.Locale,true)==0
            /*x.Locale!.ToLower() == languageUpdate.Language!.Locale!.ToLower()*/ && x.LangId != languageUpdate.Language.LangId).ToList();

            if (getLanguage !=null && getLanguage.Any())
            {
                languageUpdate._message = "This Language Name Already Exist";
                languageUpdate._failure = true;
            }
            else
            {
                var getLanguageData = this.context?.Language.FirstOrDefault(x => x.LangId == languageUpdate.Language!.LangId);
                if (getLanguageData != null)
                {
                    if (getLanguageData.Lcid != null)
                    {
                        if (getLanguageData.Lcid.ToLower() == "en-us".ToLower() || getLanguageData.Lcid.ToLower() == "fr-fr".ToLower() || getLanguageData.Lcid.ToLower() == "es-es".ToLower())
                        {
                            languageUpdate._message = "This Language is not editable";
                            languageUpdate._failure = true;
                            return languageUpdate;
                        }
                    }
                    languageUpdate.Language!.CreatedBy = getLanguageData.CreatedBy;
                    languageUpdate.Language.CreatedOn = getLanguageData.CreatedOn;
                    languageUpdate.Language.UpdatedOn = DateTime.Now;
                    languageUpdate.Language.Lcid = getLanguageData.Lcid;
                    this.context?.Entry(getLanguageData).CurrentValues.SetValues(languageUpdate.Language);
                    this.context?.SaveChanges();
                    languageUpdate._failure = false;
                    languageUpdate._message = "Language Updated Successfully";

                }
            }
            return languageUpdate;
        }

        /// <summary>
        /// Get All Language
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public LanguageListModel GetAllLanguage(PageResult pageResult)
        {
            LanguageListModel languageListModel = new();
            try
            {
                IQueryable<Language>? transactionIQ = null!;

                var languages = this.context?.Language;

                if (languages!=null && languages.Any())
                {
                    languageListModel._failure = false;
                    languageListModel._message = "success";

                    if (pageResult.IsListView == true)
                    {
                        if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                        {
                            transactionIQ = languages;
                        }
                        else
                        {
                            string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;

                            if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                            {
                                transactionIQ = languages.Where(x => x.Locale != null && x.Locale.ToLower().Contains(Columnvalue.ToLower()) || x.Lcid != null && x.Lcid.ToLower().Contains(Columnvalue.ToLower()) || x.LanguageCode != null && x.LanguageCode.ToLower().Contains(Columnvalue.ToLower()));
                            }
                        }
                        if (pageResult.SortingModel != null)
                        {
                            transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn??"", pageResult.SortingModel.SortDirection.ToLower());
                        }

                        int? totalCount = transactionIQ.Count();
                        if (totalCount > 0)
                        {
                            if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                            {
                                transactionIQ = transactionIQ.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                            }

                            languageListModel.TableLanguage = transactionIQ.ToList();

                            languageListModel.TableLanguage.ForEach(c =>
                            {
                                c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, c.CreatedBy);
                                c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, c.UpdatedBy);
                            });

                            languageListModel.TotalCount = totalCount;
                        }
                        else
                        {
                            languageListModel._failure = true;
                            languageListModel._message = NORECORDFOUND;
                        }
                    }
                    else
                    {
                        languageListModel.TableLanguage = languages.ToList();
                    }
                }
                else
                {
                    languageListModel._failure = true;
                    languageListModel._message = NORECORDFOUND;
                }
                languageListModel._tenantName = pageResult._tenantName;
                languageListModel._token = pageResult._token;
                languageListModel.PageNumber = pageResult.PageNumber;
                languageListModel._pageSize = pageResult.PageSize;
                languageListModel.IsListView = pageResult.IsListView;

            }
            catch (Exception es)
            {
                languageListModel._message = es.Message;
                languageListModel._failure = true;
            }
            return languageListModel;

        }

        /// <summary>
        /// Delete Language Value By Id
        /// </summary>
        /// <param name="languageAddModel"></param>
        /// <returns></returns>
        public LanguageAddModel DeleteLanguage(LanguageAddModel languageAddModel)
        {
            LanguageAddModel deleteLanguageModel = new();
            try
            {
                var getLanguageValue = this.context?.Language.FirstOrDefault(x => x.LangId == languageAddModel.Language!.LangId);

                if (getLanguageValue != null)
                {

                    var getStudentLanguageData = this.context?.StudentMaster.Where(x => x.FirstLanguageId == languageAddModel.Language!.LangId || x.SecondLanguageId == languageAddModel.Language.LangId || x.ThirdLanguageId == languageAddModel.Language.LangId).ToList();

                    if (getLanguageValue.Lcid != null && (getLanguageValue.Lcid.ToLower() == "en-us".ToLower() || getLanguageValue.Lcid.ToLower() == "fr-fr".ToLower() || getLanguageValue.Lcid.ToLower() == "es-es".ToLower()))
                    {
                        deleteLanguageModel._message = "This Language is not deletable";
                        deleteLanguageModel._failure = true;
                        return deleteLanguageModel;
                    }
                    if (getStudentLanguageData !=null && getStudentLanguageData.Any())
                    {
                        deleteLanguageModel._message = "Language cannot be deleted because it has its association";
                        deleteLanguageModel._failure = true;
                        return deleteLanguageModel;
                    }
                    var getStaffLanguageData = this.context?.StaffMaster.Where(x => x.FirstLanguage == languageAddModel.Language!.LangId || x.SecondLanguage == languageAddModel.Language.LangId || x.ThirdLanguage == languageAddModel.Language.LangId).ToList();
                    if (getStaffLanguageData !=null && getStaffLanguageData.Any())
                    {
                        deleteLanguageModel._message = "Language cannot be deleted because it has its association";
                        deleteLanguageModel._failure = true;
                        return deleteLanguageModel;
                    }
                    var getUserLanguageData = this.context?.UserMaster.Where(x => x.LangId == languageAddModel.Language!.LangId).ToList();
                    if (getUserLanguageData !=null && getUserLanguageData.Any())
                    {
                        deleteLanguageModel._message = "Language cannot be deleted because it has its association";
                        deleteLanguageModel._failure = true;
                        return deleteLanguageModel;
                    }

                    this.context?.Language.Remove(getLanguageValue);
                    this.context?.SaveChanges();
                    deleteLanguageModel._failure = false;
                    deleteLanguageModel._message = "Language deleted successfullyy";
                }
            }
            catch (Exception ex)
            {
                deleteLanguageModel._failure = true;
                deleteLanguageModel._message = ex.Message;
            }
            return deleteLanguageModel;
        }

        /// <summary>
        /// Add Dropdown Value
        /// </summary>
        /// <param name="dpdownValue"></param>
        /// <returns></returns>
        public DropdownValueAddModel AddDropdownValue(DropdownValueAddModel dpdownValue)
        {
            if (dpdownValue.DropdownValue is null)
            {
                dpdownValue._failure = true;
                return dpdownValue;
            }

            var getDpvalue = this.context?.DpdownValuelist.AsEnumerable().FirstOrDefault(x => x.TenantId == dpdownValue.DropdownValue.TenantId && (x.SchoolId == dpdownValue.DropdownValue.SchoolId || x.SchoolId == null) && String.Compare(x.LovName, dpdownValue.DropdownValue.LovName, true) == 0 && String.Compare(x.LovColumnValue, dpdownValue.DropdownValue!.LovColumnValue, true) == 0);

            if (getDpvalue != null)
            {
                dpdownValue._message = "This Title Already Exist";
                dpdownValue._failure = true;
            }
            else
            {
                //long? dpdownValueId = Utility.GetMaxLongPK(this.context, new Func<DpdownValuelist, long>(x => x.Id));
                long? dpdownValueId = 1;

                var dpValueAllData = this.context?.DpdownValuelist.Where(x => x.TenantId == dpdownValue.DropdownValue.TenantId);

                if (dpValueAllData != null)
                {
                    var dpValueData = dpValueAllData.OrderByDescending(x => x.Id).FirstOrDefault();
                    if (dpValueData != null)
                    {
                        dpdownValueId = dpValueData.Id + 1;
                    }
                    var dpDownSortData = dpValueAllData.Where(x => Equals(x.LovName, dpdownValue.DropdownValue.LovName)).OrderByDescending(x => x.SortOrder).FirstOrDefault();
                    if (dpDownSortData != null)
                    {
                        dpdownValue.DropdownValue.SortOrder = dpDownSortData.SortOrder + 1;
                    }
                }

                dpdownValue.DropdownValue!.Id = (long)dpdownValueId!;
                dpdownValue.DropdownValue.CreatedOn = DateTime.UtcNow;
                this.context?.DpdownValuelist.Add(dpdownValue.DropdownValue);
                this.context?.SaveChanges();
                dpdownValue._failure = false;
                dpdownValue._message = "Data added successfully";
            }
            return dpdownValue;
        }

        /// <summary>
        /// Update Dropdown Value
        /// </summary>
        /// <param name="dpdownValue"></param>
        /// <returns></returns>
        public DropdownValueAddModel UpdateDropdownValue(DropdownValueAddModel dpdownValue)
        {
            if (dpdownValue.DropdownValue is null)
            {
                return dpdownValue;
            }
            using var transaction = this.context?.Database.BeginTransaction();
            try
            {
                var getDpdownData = this.context?.DpdownValuelist.AsEnumerable().Where(x => x.TenantId == dpdownValue.DropdownValue.TenantId && (x.SchoolId == dpdownValue.DropdownValue.SchoolId || x.SchoolId == null) && x.Id != dpdownValue.DropdownValue.Id && String.Compare(x.LovName, dpdownValue.DropdownValue.LovName, true) == 0 && String.Compare(x.LovColumnValue, dpdownValue.DropdownValue!.LovColumnValue, true) == 0).FirstOrDefault();

                if (getDpdownData != null)
                {
                    dpdownValue._message = "This Title Already Exist";
                    dpdownValue._failure = true;
                }
                else
                {
                    var getDpdownValue = this.context?.DpdownValuelist.FirstOrDefault(x => x.TenantId == dpdownValue.DropdownValue.TenantId && x.SchoolId == dpdownValue.DropdownValue.SchoolId && x.Id == dpdownValue.DropdownValue.Id);

                    if (getDpdownValue != null)
                    {
                        if ((dpdownValue.DropdownValue.LovColumnValue ?? "").ToLower() != (getDpdownValue.LovColumnValue ?? "").ToLower())
                        {

                            switch (getDpdownValue.LovName)
                            {
                                case "Race":
                                    //var raceValueStaff = this.context?.StaffMaster.AsEnumerable().Where(x => x.TenantId == dpdownValue.DropdownValue.TenantId && x.SchoolId == dpdownValue.DropdownValue.SchoolId && String.Compare(x.Race, getDpdownValue.LovColumnValue, true) == 0).ToList();
                                    //var raceValueStudent = this.context?.StudentMaster.AsEnumerable().Where(x => x.TenantId == dpdownValue.DropdownValue.TenantId && x.SchoolId == dpdownValue.DropdownValue.SchoolId && String.Compare(x.Race, getDpdownValue.LovColumnValue, true) == 0).ToList();

                                    var raceValueStaff = this.context?.StaffMaster.Where(x => x.TenantId == dpdownValue.DropdownValue.TenantId && x.SchoolId == dpdownValue.DropdownValue.SchoolId && x.Race != null && (x.Race ?? "").ToLower() == (getDpdownValue.LovColumnValue ?? "").ToLower()).ToList();

                                    var raceValueStudent = this.context?.StudentMaster.Where(x => x.TenantId == dpdownValue.DropdownValue.TenantId && x.SchoolId == dpdownValue.DropdownValue.SchoolId && x.Race != null && (x.Race ?? "").ToLower() == (getDpdownValue.LovColumnValue ?? "").ToLower()).ToList();


                                    if (raceValueStaff != null && raceValueStaff.Any())
                                    {
                                        foreach (var staff in raceValueStaff)
                                        {
                                            staff.Race = dpdownValue.DropdownValue.LovColumnValue;
                                        }
                                        this.context?.StaffMaster.UpdateRange(raceValueStaff);
                                    }
                                    if (raceValueStudent != null && raceValueStudent.Any())
                                    {
                                        foreach (var student in raceValueStudent)
                                        {
                                            student.Race = dpdownValue.DropdownValue.LovColumnValue;
                                        }
                                        this.context?.StudentMaster.UpdateRange(raceValueStudent);
                                    }
                                    break;
                                case "School Level":
                                    var schoolLevelValue = this.context?.SchoolMaster.AsEnumerable().Where(x => x.TenantId == dpdownValue.DropdownValue.TenantId && x.SchoolId == dpdownValue.DropdownValue.SchoolId && String.Compare(x.SchoolLevel, getDpdownValue.LovColumnValue, true) == 0).ToList();

                                    if (schoolLevelValue != null && schoolLevelValue.Any())
                                    {
                                        foreach (var schoolLevel in schoolLevelValue)
                                        {
                                            schoolLevel.SchoolLevel = dpdownValue.DropdownValue.LovColumnValue;
                                        }
                                        this.context?.SchoolMaster.UpdateRange(schoolLevelValue);

                                    }

                                    break;
                                case "School Classification":
                                    var schoolClassificationValue = this.context?.SchoolMaster.AsEnumerable().Where(x => x.TenantId == dpdownValue.DropdownValue.TenantId && x.SchoolId == dpdownValue.DropdownValue.SchoolId && String.Compare(x.SchoolClassification, getDpdownValue.LovColumnValue, true) == 0).ToList();

                                    if (schoolClassificationValue != null && schoolClassificationValue.Any())
                                    {
                                        foreach (var schoolClassification in schoolClassificationValue)
                                        {
                                            schoolClassification.SchoolClassification = dpdownValue.DropdownValue.LovColumnValue;
                                        }
                                        this.context?.SchoolMaster.UpdateRange(schoolClassificationValue);
                                    }

                                    break;
                                case "Female Toilet Type":
                                    var femaleToiletTypeValue = this.context?.SchoolDetail.AsEnumerable().Where(x => x.TenantId == dpdownValue.DropdownValue.TenantId && x.SchoolId == dpdownValue.DropdownValue.SchoolId && String.Compare(x.FemaleToiletType, getDpdownValue.LovColumnValue, true) == 0).ToList();

                                    if (femaleToiletTypeValue != null && femaleToiletTypeValue.Any())
                                    {
                                        foreach (var femaleToilet in femaleToiletTypeValue)
                                        {
                                            femaleToilet.FemaleToiletType = dpdownValue.DropdownValue.LovColumnValue;
                                        }
                                        this.context?.SchoolDetail.UpdateRange(femaleToiletTypeValue);
                                    }

                                    break;
                                case "Male Toilet Type":
                                    var maleToiletTypeValue = this.context?.SchoolDetail.AsEnumerable().Where(x => x.TenantId == dpdownValue.DropdownValue.TenantId && x.SchoolId == dpdownValue.DropdownValue.SchoolId && String.Compare(x.MaleToiletType, getDpdownValue.LovColumnValue, true) == 0).ToList();

                                    if (maleToiletTypeValue != null && maleToiletTypeValue.Any())
                                    {
                                        foreach (var maleToilet in maleToiletTypeValue)
                                        {
                                            maleToilet.MaleToiletType = dpdownValue.DropdownValue.LovColumnValue;
                                        }
                                        this.context?.SchoolDetail.UpdateRange(maleToiletTypeValue);
                                    }

                                    break;
                                case "Common Toilet Type":
                                    var commonToiletTypeValue = this.context?.SchoolDetail.AsEnumerable().Where(x => x.TenantId == dpdownValue.DropdownValue.TenantId && x.SchoolId == dpdownValue.DropdownValue.SchoolId && String.Compare(x.ComonToiletType, getDpdownValue.LovColumnValue, true) == 0).ToList();

                                    if (commonToiletTypeValue != null && commonToiletTypeValue.Any())
                                    {
                                        foreach (var ComonToilet in commonToiletTypeValue)
                                        {
                                            ComonToilet.ComonToiletType = dpdownValue.DropdownValue.LovColumnValue;
                                        }
                                        this.context?.SchoolDetail.UpdateRange(commonToiletTypeValue);
                                    }

                                    break;
                                case "Ethnicity":
                                    //var ethnicityValueStaff = this.context?.StaffMaster.AsEnumerable().Where(x => x.TenantId == dpdownValue.DropdownValue.TenantId && x.SchoolId == dpdownValue.DropdownValue.SchoolId && String.Compare(x.Ethnicity, getDpdownValue.LovColumnValue, true) == 0).ToList();
                                    //var ethnicityValueStudent = this.context?.StudentMaster.AsEnumerable().Where(x => x.TenantId == dpdownValue.DropdownValue.TenantId && x.SchoolId == dpdownValue.DropdownValue.SchoolId && String.Compare(x.Ethnicity, getDpdownValue.LovColumnValue, true) == 0).ToList();

                                    var ethnicityValueStaff = this.context?.StaffMaster.Where(x => x.TenantId == dpdownValue.DropdownValue.TenantId && x.SchoolId == dpdownValue.DropdownValue.SchoolId && x.Ethnicity != null && (x.Ethnicity ?? "").ToLower() == (getDpdownValue.LovColumnValue ?? "").ToLower()).ToList();

                                    var ethnicityValueStudent = this.context?.StudentMaster.Where(x => x.TenantId == dpdownValue.DropdownValue.TenantId && x.SchoolId == dpdownValue.DropdownValue.SchoolId && x.Ethnicity != null && (x.Ethnicity ?? "").ToLower() == (getDpdownValue.LovColumnValue ?? "").ToLower()).ToList();

                                    if (ethnicityValueStaff != null && ethnicityValueStaff.Any())
                                    {
                                        foreach (var staff in ethnicityValueStaff)
                                        {
                                            staff.Ethnicity = dpdownValue.DropdownValue.LovColumnValue;
                                        }
                                        this.context?.StaffMaster.UpdateRange(ethnicityValueStaff);
                                    }
                                    if (ethnicityValueStudent != null && ethnicityValueStudent.Any())
                                    {
                                        foreach (var student in ethnicityValueStudent)
                                        {
                                            student.Ethnicity = dpdownValue.DropdownValue.LovColumnValue;
                                        }
                                        this.context?.StudentMaster.UpdateRange(ethnicityValueStudent);
                                    }
                                    break;
                            }
                        }

                        getDpdownValue.LovColumnValue = dpdownValue.DropdownValue.LovColumnValue;
                        getDpdownValue.UpdatedOn = DateTime.UtcNow;
                        getDpdownValue.UpdatedBy = dpdownValue.DropdownValue.UpdatedBy;
                        this.context?.SaveChanges();
                        dpdownValue._failure = false;
                        dpdownValue._message = "Data Updated Successfully";
                        transaction?.Commit();
                    }
                }
                return dpdownValue;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                dpdownValue.DropdownValue = null;
                dpdownValue._failure = true;
                dpdownValue._message = ex.Message;
                return dpdownValue;
            }
        }


        /// <summary>
        /// Get Dropdown Value By Id
        /// </summary>
        /// <param name="dpdownValue"></param>
        /// <returns></returns>
        public DropdownValueAddModel ViewDropdownValue(DropdownValueAddModel dpdownValue)
        {
            if (dpdownValue.DropdownValue is null)
            {
                return dpdownValue;
            }

            try
            {
                DropdownValueAddModel dropdownValueAddModel = new();
                var getDpdownValue = this.context?.DpdownValuelist.FirstOrDefault(x => x.TenantId == dpdownValue.DropdownValue.TenantId && x.SchoolId == dpdownValue.DropdownValue.SchoolId && x.Id == dpdownValue.DropdownValue.Id);
                if (getDpdownValue != null)
                {
                    dropdownValueAddModel.DropdownValue = getDpdownValue;
                    dropdownValueAddModel._tenantName = dpdownValue._tenantName;
                    dropdownValueAddModel._failure = false;
                    return dropdownValueAddModel;
                }
                else
                {
                    dropdownValueAddModel._failure = true;
                    dropdownValueAddModel._message = NORECORDFOUND;
                    return dropdownValueAddModel;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// Get All Dropdown Value
        /// </summary>
        /// <param name="dpdownList"></param>
        /// <returns></returns>
        public DropdownValueListModel GetAllDropdownValues(DropdownValueListModel dpdownList)
        {
           
            DropdownValueListModel dpdownListModel = new ();
            try
            {
                if (dpdownList.SchoolId != null && dpdownList.SchoolId > 0)
                {
                    var dropdownList = this.context?.DpdownValuelist.AsEnumerable().Where(x => x.TenantId == dpdownList.TenantId && (x.SchoolId == dpdownList.SchoolId || x.SchoolId == null) && String.Compare( x.LovName, dpdownList.LovName,true)==0).OrderBy(a => a.SortOrder).ToList();

                    if (dropdownList !=null && dropdownList.Any())
                    {
                        if (dpdownList.IsListView == true)
                        {
                            dropdownList.ForEach(c =>
                            {
                                c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, dpdownList.TenantId, c.CreatedBy);
                                c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, dpdownList.TenantId, c.UpdatedBy);
                            });
                        }
                        dpdownListModel.DropdownList = dropdownList;
                        dpdownListModel._failure = false;
                    }
                    else
                    {
                        dpdownListModel._failure = true;
                        dpdownListModel._message = NORECORDFOUND;
                    }

                   
                    dpdownListModel._tenantName = dpdownList._tenantName;
                    dpdownListModel._token = dpdownList._token;
                }
                else
                {
                    var dropdownList = this.context?.DpdownValuelist.AsEnumerable().Where(x => x.TenantId == dpdownList.TenantId && x.SchoolId == null && String.Compare(x.LovName, dpdownList.LovName, true) == 0).Select(x => new DpdownValuelist
                    {
                        TenantId = x.TenantId,
                        LovName = x.LovName,
                        LovColumnValue = x.LovColumnValue
                    }).Distinct().ToList();

                    if (dropdownList!=null && dropdownList.Any())
                    {
                        dpdownListModel.DropdownList = dropdownList;
                        dpdownListModel._failure = false;
                    }
                    else
                    {
                        dpdownListModel._failure = true;
                        dpdownListModel._message = NORECORDFOUND;
                    }

                    //dpdownListModel.DropdownList = dropdownList;
                    dpdownListModel._tenantName = dpdownList._tenantName;
                    dpdownListModel._token = dpdownList._token;
                }
            }
            catch (Exception es)
            {
                dpdownListModel._message = es.Message;
                dpdownListModel._failure = true;
                dpdownListModel._tenantName = dpdownList._tenantName;
                dpdownListModel._token = dpdownList._token;
            }
            return dpdownListModel;

        }

        /// <summary>
        /// Add Country
        /// </summary>
        /// <param name="countryAddModel"></param>
        /// <returns></returns>
        public CountryAddModel AddCountry(CountryAddModel countryAddModel)
        {
            if (countryAddModel.Country?.Name is null)
            {
                return countryAddModel;
            }
            try
            {
                var getCountry = this.context?.Country.AsEnumerable().FirstOrDefault(x => String.Compare(x.Name, countryAddModel.Country.Name,true)==0);

                if (getCountry != null)
                {
                    countryAddModel._message = "This Country Name Already Exist";
                    countryAddModel._failure = true;
                }
                else
                {
                    int? CountryId = Utility.GetMaxPK(this.context, new Func<Country, int>(x => x.Id));
                    countryAddModel.Country.Id = (int)CountryId!;
                    countryAddModel.Country.CreatedOn = DateTime.UtcNow;
                    this.context?.Country.Add(countryAddModel.Country);
                    this.context?.SaveChanges();
                    countryAddModel._failure = false;
                    countryAddModel._message = "Country added successfully";
                }
            }
            catch (Exception es)
            {
                countryAddModel._message = es.Message;
                countryAddModel._failure = true;
                countryAddModel._tenantName = countryAddModel._tenantName;
                countryAddModel._token = countryAddModel._token;
            }
            return countryAddModel;
        }

        /// <summary>
        /// Update Country
        /// </summary>
        /// <param name="countryAddModel"></param>
        /// <returns></returns>
        public CountryAddModel UpdateCountry(CountryAddModel countryAddModel)
        {
            if (countryAddModel.Country?.Name is null)
            {
                return countryAddModel;
            }
            try
            {
                var getCountry = this.context?.Country.AsEnumerable().Where(x => String.Compare(x.Name, countryAddModel.Country.Name,true)==0 && x.Id != countryAddModel.Country.Id).ToList();

                if (getCountry!=null && getCountry.Any())
                {
                    countryAddModel._message = "This Country Name Already Exist";
                    countryAddModel._failure = true;
                }
                else
                {
                    var getCountryData = this.context?.Country.FirstOrDefault(x => x.Id == countryAddModel.Country.Id);
                    if (getCountryData != null)
                    {
                        countryAddModel.Country.CreatedBy = getCountryData.CreatedBy;
                        countryAddModel.Country.CreatedOn = getCountryData.CreatedOn;
                        countryAddModel.Country.UpdatedOn = DateTime.Now;
                        this.context?.Entry(getCountryData).CurrentValues.SetValues(countryAddModel.Country);
                        this.context?.SaveChanges();
                        countryAddModel._failure = false;
                        countryAddModel._message = "Country Updated Successfully";
                    }
                }
            }
            catch (Exception es)
            {
                countryAddModel._message = es.Message;
                countryAddModel._failure = true;
                countryAddModel._tenantName = countryAddModel._tenantName;
                countryAddModel._token = countryAddModel._token;
            }
            return countryAddModel;
        }

        /// <summary>
        /// Delete Country
        /// </summary>
        /// <param name="countryDeleteModel"></param>
        /// <returns></returns>
        public CountryAddModel DeleteCountry(CountryAddModel countryDeleteModel)
        {
            if (countryDeleteModel.Country is null)
            {
                return countryDeleteModel;
            }
            CountryAddModel deleteCountryModel = new();
            try
            {
                var getCountryValue = this.context?.Country.FirstOrDefault(x => x.Id == countryDeleteModel.Country.Id);

                if (getCountryValue != null)
                {
                    var getPatrentCountryData = this.context?.ParentAddress.Where(x => x.Country == countryDeleteModel.Country.Id.ToString()).ToList();
                    if (getPatrentCountryData !=null && getPatrentCountryData.Any())
                    {
                        deleteCountryModel._message = "Country cannot be deleted because it has its association";
                        deleteCountryModel._failure = true;
                        return deleteCountryModel;
                    }
                    var getStudentCountryData = this.context?.StudentMaster.Where(x => x.CountryOfBirth == countryDeleteModel.Country.Id || x.HomeAddressCountry == countryDeleteModel.Country.Id.ToString() || x.MailingAddressCountry == countryDeleteModel.Country.Id.ToString()).ToList();
                    if (getStudentCountryData!= null && getStudentCountryData.Any())
                    {
                        deleteCountryModel._message = "Country cannot be deleted because it has its association";
                        deleteCountryModel._failure = true;
                        return deleteCountryModel;
                    }
                    var getStaffCountryData = this.context?.StaffMaster.Where(x => x.CountryOfBirth == countryDeleteModel.Country.Id || x.HomeAddressCountry == countryDeleteModel.Country.Id.ToString() || x.MailingAddressCountry == countryDeleteModel.Country.Id.ToString()).ToList();
                    if (getStaffCountryData!=null && getStaffCountryData.Any())
                    {
                        deleteCountryModel._message = "Country cannot be deleted because it has its association";
                        deleteCountryModel._failure = true;
                        return deleteCountryModel;
                    }
                    var getSchoolCountryData = this.context?.SchoolMaster.Where(x => x.Country == countryDeleteModel.Country.Id.ToString()).ToList();
                    if (getSchoolCountryData !=null && getSchoolCountryData.Any())
                    {
                        deleteCountryModel._message = "Country cannot be deleted because it has its association";
                        deleteCountryModel._failure = true;
                        return deleteCountryModel;
                    }
                    var getSateData = this.context?.State.Where(x => x.CountryId == countryDeleteModel.Country.Id).ToList();
                    if (getSateData!=null && getSateData.Any())
                    {
                        deleteCountryModel._message = "Country cannot be deleted because it has its association";
                        deleteCountryModel._failure = true;
                        return deleteCountryModel;
                    }

                    this.context?.Country.Remove(getCountryValue);
                    this.context?.SaveChanges();
                    deleteCountryModel._failure = false;
                    deleteCountryModel._message = "Country deleted successfullyy";

                }

            }
            catch (Exception ex)
            {
                deleteCountryModel._failure = true;
                deleteCountryModel._message = ex.Message;
            }
            return deleteCountryModel;
        }

        /// <summary>
        /// Delete Dropdown Value
        /// </summary>
        /// <param name="dpdownValue"></param>
        /// <returns></returns>
        public DropdownValueAddModel DeleteDropdownValue(DropdownValueAddModel dpdownValue)
        {
            if (dpdownValue.DropdownValue is null)
            {
                return dpdownValue;
            }
            DropdownValueAddModel dropdownValueAddModel = new();
            try
            {
                var getDpValue = this.context?.DpdownValuelist.FirstOrDefault(x => x.Id == dpdownValue.DropdownValue.Id);

                if (getDpValue != null)
                {

                    switch (getDpValue.LovName)
                    {
                        case "Race":

                            //var raceValueStaff = this.context?.StaffMaster.AsEnumerable().Where(x => x.TenantId == getDpValue.TenantId&& x.SchoolId == getDpValue.SchoolId&&  x.Race != null && String.Compare(x.Race, getDpValue.LovColumnValue, true) == 0).FirstOrDefault();
                            var raceValueStaff = this.context?.StaffMaster.FirstOrDefault(x => x.TenantId == getDpValue.TenantId && x.SchoolId == getDpValue.SchoolId && x.Race != null && (x.Race ?? "").ToLower() == (getDpValue.LovColumnValue ?? "").ToLower());

                            if (raceValueStaff != null)
                            {
                                dropdownValueAddModel._message = "Race cannot be deleted because it has its association";
                                dropdownValueAddModel._failure = true;
                                return dropdownValueAddModel;
                            }

                            //var raceValueStudent = this.context?.StudentMaster.AsEnumerable().FirstOrDefault(x => x.TenantId == getDpValue.TenantId && x.SchoolId == getDpValue.SchoolId && x.Race != null && String.Compare(x.Race, getDpValue.LovColumnValue, true) == 0);

                            var raceValueStudent = this.context?.StudentMaster.FirstOrDefault(x => x.TenantId == getDpValue.TenantId && x.SchoolId == getDpValue.SchoolId && x.Race != null && (x.Race ?? "").ToLower() == (getDpValue.LovColumnValue ?? "").ToLower());

                            if (raceValueStudent != null)
                            {
                                dropdownValueAddModel._message = "Race cannot be deleted because it has its association";
                                dropdownValueAddModel._failure = true;
                                return dropdownValueAddModel;
                            }


                            break;
                        case "School Level":
                            var schoolLevelValue = this.context?.SchoolMaster.AsEnumerable().Where(x => x.TenantId == getDpValue.TenantId && x.SchoolId == getDpValue.SchoolId && String.Compare(x.SchoolLevel, getDpValue.LovColumnValue, true) == 0).FirstOrDefault();

                            if (schoolLevelValue != null)
                            {
                                dropdownValueAddModel._message = "School Level cannot be deleted because it has its association";
                                dropdownValueAddModel._failure = true;
                                return dropdownValueAddModel;
                            }

                            break;
                        case "School Classification":
                            var schoolClassificationValue = this.context?.SchoolMaster.AsEnumerable().Where(x => x.TenantId == getDpValue.TenantId && x.SchoolId == getDpValue.SchoolId && x.SchoolClassification != null && String.Compare(x.SchoolClassification, getDpValue.LovColumnValue, true) == 0).FirstOrDefault();

                            if (schoolClassificationValue != null)
                            {
                                dropdownValueAddModel._message = "School Classification cannot be deleted because it has its association";
                                dropdownValueAddModel._failure = true;
                                return dropdownValueAddModel;
                            }

                            break;
                        case "Female Toilet Type":
                            var femaleToiletTypeValue = this.context?.SchoolDetail.AsEnumerable().Where(x => x.TenantId == getDpValue.TenantId && x.SchoolId == getDpValue.SchoolId && x.FemaleToiletType != null && String.Compare(x.FemaleToiletType, getDpValue.LovColumnValue, true) == 0).FirstOrDefault();

                            if (femaleToiletTypeValue != null)
                            {
                                dropdownValueAddModel._message = "Female Toilet Type cannot be deleted because it has its association";
                                dropdownValueAddModel._failure = true;
                                return dropdownValueAddModel;
                            }

                            break;
                        case "Male Toilet Type":
                            var maleToiletTypeValue = this.context?.SchoolDetail.AsEnumerable().Where(x => x.TenantId == getDpValue.TenantId && x.SchoolId == getDpValue.SchoolId && x.MaleToiletType != null && String.Compare(x.MaleToiletType, getDpValue.LovColumnValue, true) == 0).FirstOrDefault();

                            if (maleToiletTypeValue != null)
                            {
                                dropdownValueAddModel._message = "Male Toilet Type cannot be deleted because it has its association";
                                dropdownValueAddModel._failure = true;
                                return dropdownValueAddModel;
                            }

                            break;
                        case "Common Toilet Type":
                            var commonToiletTypeValue = this.context?.SchoolDetail.AsEnumerable().Where(x => x.TenantId == getDpValue.TenantId && x.SchoolId == getDpValue.SchoolId && x.ComonToiletType != null && String.Compare(x.ComonToiletType, getDpValue.LovColumnValue, true) == 0).FirstOrDefault();

                            if (commonToiletTypeValue != null)
                            {
                                dropdownValueAddModel._message = "Common Toilet Type cannot be deleted because it has its association";
                                dropdownValueAddModel._failure = true;
                                return dropdownValueAddModel;
                            }

                            break;
                        case "Ethnicity":
                            //var ethnicityValueStaff = this.context?.StaffMaster.AsEnumerable().Where(x => x.TenantId == getDpValue.TenantId && x.SchoolId == getDpValue.SchoolId && x.Ethnicity != null && String.Compare(x.Ethnicity, getDpValue.LovColumnValue, true) == 0).FirstOrDefault();
                            var ethnicityValueStaff = this.context?.StaffMaster.FirstOrDefault(x => x.TenantId == getDpValue.TenantId && x.SchoolId == getDpValue.SchoolId && x.Ethnicity != null && (x.Ethnicity ?? "").ToLower() == (getDpValue.LovColumnValue ?? "").ToLower());

                            if (ethnicityValueStaff != null)
                            {
                                dropdownValueAddModel._message = "Ethnicity cannot be deleted because it has its association";
                                dropdownValueAddModel._failure = true;
                                return dropdownValueAddModel;
                            }
                            //var ethnicityValueStudent = this.context?.StudentMaster.AsEnumerable().Where(x => x.TenantId == getDpValue.TenantId && x.SchoolId == getDpValue.SchoolId && x.Ethnicity != null && String.Compare(x.Ethnicity, getDpValue.LovColumnValue, true) == 0).FirstOrDefault();
                            var ethnicityValueStudent = this.context?.StudentMaster.FirstOrDefault(x => x.TenantId == getDpValue.TenantId && x.SchoolId == getDpValue.SchoolId && x.Ethnicity != null && (x.Ethnicity ?? "").ToLower() == (getDpValue.LovColumnValue ?? "").ToLower());

                            if (ethnicityValueStudent != null)
                            {
                                dropdownValueAddModel._message = "Ethnicity cannot be deleted because it has its association";
                                dropdownValueAddModel._failure = true;
                                return dropdownValueAddModel;
                            }

                            break;
                    }
                    this.context?.DpdownValuelist.Remove(getDpValue);
                    this.context?.SaveChanges();
                    dropdownValueAddModel._failure = false;
                    dropdownValueAddModel._message = "Data deleted successfullyy";
                }

            }
            catch (Exception es)
            {
                dropdownValueAddModel._failure = true;
                dropdownValueAddModel._message = es.Message;
            }
            return dropdownValueAddModel;

        }

        /// <summary>
        /// Get All Language For Login 
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public LanguageListModel GetAllLanguageForLogin(LanguageListModel language)
        {
            LanguageListModel languageListModel = new();
            try
            {
                //languageListModel.TableLanguage = null;

                var languages = this.context?.Language.Where(x =>x.Lcid!=null && x.Lcid.ToLower() == "en-us".ToLower() || x.Lcid != null && x.Lcid.ToLower() == "fr-fr".ToLower() || x.Lcid!=null && x.Lcid.ToLower() == "es-es".ToLower()).ToList();
                
                if (languages!=null && languages.Any())
                {
                    languageListModel.TableLanguage = languages;
                    languageListModel._failure = false;
                }
                else
                {
                    languageListModel._failure = true;
                    languageListModel._message = NORECORDFOUND;
                }
                languageListModel._tenantName = language._tenantName;
                languageListModel._token = language._token;

            }
            catch (Exception es)
            {
                languageListModel._message = es.Message;
                languageListModel._failure = true;
                languageListModel._tenantName = language._tenantName;
                languageListModel._token = language._token;
            }
            return languageListModel;

        }

        /// <summary>
        /// Dashboard View
        /// </summary>
        /// <param name="dashboardViewModel"></param>
        /// <returns></returns>
        public DashboardViewModel GetDashboardView(DashboardViewModel dashboardViewModel)
        {
            DashboardViewModel dashboardView = new ();
            try
            {
                var todayDate = DateTime.Today;
                dashboardView.TenantId = dashboardViewModel.TenantId;
                dashboardView.SchoolId = dashboardViewModel.SchoolId;
                dashboardView.AcademicYear = dashboardViewModel.AcademicYear;

                dashboardView.SuperAdministratorName = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == dashboardViewModel.TenantId && x.SchoolId == dashboardViewModel.SchoolId && x.MembershipId == dashboardViewModel.MembershipId)?.Name;

                dashboardView.SchoolName = this.context?.SchoolMaster.FirstOrDefault(x => x.TenantId == dashboardViewModel.TenantId && x.SchoolId == dashboardViewModel.SchoolId)?.SchoolName;

                dashboardView.TotalStudent = this.context?.StudentMaster.Where(x => x.TenantId == dashboardViewModel.TenantId && x.SchoolId == dashboardViewModel.SchoolId && x.IsActive == true).Count();

                //dashboardView.TotalParent = this.context?.ParentAssociationship.Where(x => x.TenantId == dashboardViewModel.TenantId && x.SchoolId == dashboardViewModel.SchoolId && x.Associationship == true).Select(s => new { s.TenantId, s.SchoolId, s.ParentId }).Distinct().ToList().Count();

                //dashboardView.TotalStaff = this.context?.StaffMaster.Where(x => x.TenantId == dashboardViewModel.TenantId && x.SchoolId == dashboardViewModel.SchoolId).Count();

                dashboardView.TotalStaff = this.context?.StaffSchoolInfo.Include(x => x.StaffMaster).Where(x => x.TenantId == dashboardViewModel.TenantId && x.SchoolAttachedId == dashboardViewModel.SchoolId && (x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date) && x.StaffMaster!.IsActive != false).Count();

                dashboardView.TotalParent = this.context?.ParentAssociationship.Where(x => x.TenantId == dashboardViewModel.TenantId && x.SchoolId == dashboardViewModel.SchoolId && x.Associationship == true).Select(x => x.ParentId).Distinct().Count();

                var notice = this.context?.Notice.Where(x => x.TenantId == dashboardViewModel.TenantId && (x.SchoolId == dashboardViewModel.SchoolId || (x.SchoolId != dashboardViewModel.SchoolId && x.VisibleToAllSchool == true)) && x.Isactive == true && (x.ValidFrom <= todayDate && todayDate <= x.ValidTo)).OrderByDescending(x => x.ValidFrom).ToList();
                if (notice?.Any() == true)
                {
                    dashboardView.NoticeList = notice;

                }

                dashboardView._tenantName = dashboardViewModel._tenantName;
                dashboardView._token = dashboardViewModel._token;
            }
            catch (Exception es)
            {
                dashboardView._failure = true;
                dashboardView._message = es.Message;
            }
            return dashboardView;
        }

        /// <summary>
        /// Get Release Number
        /// </summary>
        /// <param name="releaseNumberAddViewModel"></param>
        /// <returns></returns>
        public ReleaseNumberAddViewModel GetReleaseNumber(ReleaseNumberAddViewModel releaseNumberAddViewModel)
        {
            ReleaseNumberAddViewModel ReleaseNumberViewModel = new ();
            try
            {
                var releaseNumber = this.context?.ReleaseNumber.OrderByDescending(c => c.ReleaseDate).FirstOrDefault(c => c.SchoolId == releaseNumberAddViewModel.releaseNumber!.SchoolId && c.TenantId == releaseNumberAddViewModel.releaseNumber.TenantId);

                ReleaseNumberViewModel.releaseNumber = releaseNumber;

                if (releaseNumber != null)
                {
                    ReleaseNumberViewModel._failure = false;
                }
                else
                {
                    ReleaseNumberViewModel._message = NORECORDFOUND;
                    ReleaseNumberViewModel._failure = true;
                }
                ReleaseNumberViewModel._tenantName = releaseNumberAddViewModel._tenantName;
                ReleaseNumberViewModel._token = releaseNumberAddViewModel._token;
            }
            catch (Exception es)
            {
                ReleaseNumberViewModel._message = es.Message;
                ReleaseNumberViewModel._failure = true;
                ReleaseNumberViewModel._tenantName = releaseNumberAddViewModel._tenantName;
                ReleaseNumberViewModel._token = releaseNumberAddViewModel._token;
            }
            return ReleaseNumberViewModel;
        }

        /// <summary>
        /// Add Search Filter
        /// </summary>
        /// <param name="searchFilterAddViewModel"></param>
        /// <returns></returns>
        public SearchFilterAddViewModel AddSearchFilter(SearchFilterAddViewModel searchFilterAddViewModel)
        {
            if(searchFilterAddViewModel.SearchFilter is null)
            {
                return searchFilterAddViewModel;
            }
            try
            {
                var checkFilterName = this.context?.SearchFilter.AsEnumerable().Where(x => x.SchoolId == searchFilterAddViewModel.SearchFilter.SchoolId && x.TenantId == searchFilterAddViewModel.SearchFilter.TenantId && x.FilterName !=null && String.Compare(x.FilterName, searchFilterAddViewModel.SearchFilter.FilterName,true)==0 && String.Compare(x.Module, searchFilterAddViewModel.SearchFilter.Module,true)==0).FirstOrDefault();

                if (checkFilterName != null)
                {
                    searchFilterAddViewModel._failure = true;
                    searchFilterAddViewModel._message = "Filter Name already exists";
                }
                else
                {
                    int? FilterId = 1;

                    var searchFilterData = this.context?.SearchFilter.Where(x => x.SchoolId == searchFilterAddViewModel.SearchFilter.SchoolId && x.TenantId == searchFilterAddViewModel.SearchFilter.TenantId).OrderByDescending(x => x.FilterId).FirstOrDefault();

                    if (searchFilterData != null)
                    {
                        FilterId = searchFilterData.FilterId + 1;
                    }

                    searchFilterAddViewModel.SearchFilter.FilterId = (int)FilterId;
                    searchFilterAddViewModel.SearchFilter.CreatedOn = DateTime.UtcNow;
                    this.context?.SearchFilter.Add(searchFilterAddViewModel.SearchFilter);
                    this.context?.SaveChanges();
                    searchFilterAddViewModel._failure = false;
                    searchFilterAddViewModel._message = "Search Filter added successfully";
                }
            }
            catch (Exception es)
            {
                searchFilterAddViewModel._failure = true;
                searchFilterAddViewModel._message = es.Message;
            }
            return searchFilterAddViewModel;
        }

        /// <summary>
        /// Update Search Filter
        /// </summary>
        /// <param name="searchFilterAddViewModel"></param>
        /// <returns></returns>
        public SearchFilterAddViewModel UpdateSearchFilter(SearchFilterAddViewModel searchFilterAddViewModel)
        {
            if(searchFilterAddViewModel.SearchFilter?.FilterName is null)
            {
                return searchFilterAddViewModel;
            }
            try
            {
                var searchFilterUpdate = this.context?.SearchFilter.FirstOrDefault(x => x.TenantId == searchFilterAddViewModel.SearchFilter.TenantId && x.SchoolId == searchFilterAddViewModel.SearchFilter.SchoolId && x.FilterId == searchFilterAddViewModel.SearchFilter.FilterId);

                if (searchFilterUpdate != null)
                {
                    var checkFilterName = this.context?.SearchFilter.AsEnumerable().Where(x => x.SchoolId == searchFilterAddViewModel.SearchFilter.SchoolId && x.TenantId == searchFilterAddViewModel.SearchFilter.TenantId && x.FilterId != searchFilterAddViewModel.SearchFilter.FilterId && x.FilterName!=null && String.Compare(x.FilterName, searchFilterAddViewModel.SearchFilter.FilterName,true)==0 && String.Compare(x.Module, searchFilterAddViewModel.SearchFilter.Module,true)==0).FirstOrDefault();

                    if (checkFilterName != null)
                    {
                        searchFilterAddViewModel._failure = true;
                        searchFilterAddViewModel._message = "Filter Name already exists";
                    }
                    else
                    {
                        searchFilterAddViewModel.SearchFilter.FilterName = searchFilterUpdate.FilterName;
                        searchFilterAddViewModel.SearchFilter.CreatedBy = searchFilterUpdate.CreatedBy;
                        searchFilterAddViewModel.SearchFilter.CreatedOn = searchFilterUpdate.CreatedOn;
                        searchFilterAddViewModel.SearchFilter.UpdatedOn = DateTime.Now;
                        this.context?.Entry(searchFilterUpdate).CurrentValues.SetValues(searchFilterAddViewModel.SearchFilter);
                        this.context?.SaveChanges();
                        searchFilterAddViewModel._failure = false;
                        searchFilterAddViewModel._message = "Search Filter Updated Successfully";
                    }
                }
                else
                {
                    searchFilterAddViewModel._failure = true;
                    searchFilterAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {

                searchFilterAddViewModel._failure = true;
                searchFilterAddViewModel._message = es.Message;
            }
            return searchFilterAddViewModel;
        }

        /// <summary>
        /// Delete Search Filter
        /// </summary>
        /// <param name="searchFilterAddViewModel"></param>
        /// <returns></returns>
        public SearchFilterAddViewModel DeleteSearchFilter(SearchFilterAddViewModel searchFilterAddViewModel)
        {
            try
            {
                var searchFilterDelete = this.context?.SearchFilter.FirstOrDefault(x => x.TenantId == searchFilterAddViewModel.SearchFilter!.TenantId && x.SchoolId == searchFilterAddViewModel.SearchFilter.SchoolId && x.FilterId == searchFilterAddViewModel.SearchFilter.FilterId);

                if (searchFilterDelete != null)
                {
                    this.context?.SearchFilter.Remove(searchFilterDelete);
                    this.context?.SaveChanges();
                    searchFilterAddViewModel._failure = false;
                    searchFilterAddViewModel._message = "Search Filter deleted successfullyy";

                }
                else
                {
                    searchFilterAddViewModel._failure = true;
                    searchFilterAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {

                searchFilterAddViewModel._failure = true;
                searchFilterAddViewModel._message = es.Message;
            }
            return searchFilterAddViewModel;
        }

        /// <summary>
        /// Get All Search Filter
        /// </summary>
        /// <param name="searchFilterListViewModel"></param>
        /// <returns></returns>
        public SearchFilterListViewModel GetAllSearchFilter(SearchFilterListViewModel searchFilterListViewModel)
        {
            SearchFilterListViewModel searchFilterListModel = new ();
            try
            {

                var searchFilterList = this.context?.SearchFilter.AsEnumerable().Where(x => x.TenantId == searchFilterListViewModel.TenantId && x.SchoolId == searchFilterListViewModel.SchoolId && String.Compare(x.Module, searchFilterListViewModel.Module,true)==0).ToList();

                

                if (searchFilterList!=null && searchFilterList.Any())
                {
                    searchFilterListModel.SearchFilterList = searchFilterList;
                    searchFilterListModel._tenantName = searchFilterListViewModel._tenantName;
                    searchFilterListModel._token = searchFilterListViewModel._token;
                    searchFilterListModel._failure = false;
                }
                else
                {
                    searchFilterListModel._tenantName = searchFilterListViewModel._tenantName;
                    searchFilterListModel._token = searchFilterListViewModel._token;
                    searchFilterListModel._failure = true;
                    searchFilterListModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                searchFilterListModel._message = es.Message;
                searchFilterListModel._failure = true;
                searchFilterListModel._tenantName = searchFilterListViewModel._tenantName;
                searchFilterListModel._token = searchFilterListViewModel._token;
            }
            return searchFilterListModel;
        }

        /// <summary>
        /// Get All Grade Age Range
        /// </summary>
        /// <param name="gradeAgeRangeListViewModel"></param>
        /// <returns></returns>
        public GradeAgeRangeListViewModel GetAllGradeAgeRange(GradeAgeRangeListViewModel gradeAgeRangeListViewModel)
        {
            GradeAgeRangeListViewModel gradeAgeRangeListModel = new ();
            try
            {
                var gradeAgeRangeList = this.context?.GradeAgeRange.ToList();
                
                gradeAgeRangeListModel._tenantName = gradeAgeRangeListViewModel._tenantName;
                gradeAgeRangeListModel._token = gradeAgeRangeListViewModel._token;

                if (gradeAgeRangeList != null &&  gradeAgeRangeList.Any())
                {
                    gradeAgeRangeListModel.GradeAgeRangeList = gradeAgeRangeList;
                    gradeAgeRangeListModel._failure = false;
                }
                else
                {
                    gradeAgeRangeListModel._failure = true;
                    gradeAgeRangeListModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                gradeAgeRangeListModel._message = es.Message;
                gradeAgeRangeListModel._failure = true;
                gradeAgeRangeListModel._tenantName = gradeAgeRangeListViewModel._tenantName;
                gradeAgeRangeListModel._token = gradeAgeRangeListViewModel._token;
            }
            return gradeAgeRangeListModel;
        }

        /// <summary>
        /// Get All Grade Educational Stage
        /// </summary>
        /// <param name="gradeEducationalStageListViewModel"></param>
        /// <returns></returns>
        public GradeEducationalStageListViewModel GetAllGradeEducationalStage(GradeEducationalStageListViewModel gradeEducationalStageListViewModel)
        {
            GradeEducationalStageListViewModel gradeEducationalStageListModel = new ();
            try
            {
                var gradeEducationalStageList = this.context?.GradeEducationalStage.ToList();
                
                gradeEducationalStageListModel._tenantName = gradeEducationalStageListViewModel._tenantName;
                gradeEducationalStageListModel._token = gradeEducationalStageListViewModel._token;

                if (gradeEducationalStageList!=null && gradeEducationalStageList.Any())
                {
                    gradeEducationalStageListModel.GradeEducationalStageList = gradeEducationalStageList;
                    gradeEducationalStageListModel._failure = false;
                }
                else
                {
                    gradeEducationalStageListModel._failure = true;
                    gradeEducationalStageListModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                gradeEducationalStageListModel._message = es.Message;
                gradeEducationalStageListModel._failure = true;
                gradeEducationalStageListModel._tenantName = gradeEducationalStageListViewModel._tenantName;
                gradeEducationalStageListModel._token = gradeEducationalStageListViewModel._token;
            }
            return gradeEducationalStageListModel;
        }

        /// <summary>
        /// Get Dashboard View For Staff
        /// </summary>
        /// <param name="scheduledCourseSectionViewModel"></param>
        /// <returns></returns>
        public ScheduledCourseSectionViewModel GetDashboardViewForStaff_old(ScheduledCourseSectionViewModel scheduledCourseSectionViewModel)
        {
            ScheduledCourseSectionViewModel scheduledCourseSectionView = new();
            List<StaffCoursesectionSchedule> staffCoursesectionScheduleList = new();
            List<AllCourseSectionView>? allCourseSectionVewList = new();

            //CourseFixedSchedule fixedData = new();
            List<CourseVariableSchedule>? variableData = new();
            List<CourseCalendarSchedule>? calenderData = new();
            List<CourseBlockSchedule>? blockData = new();

            try
            {
                var todayDate = DateTime.Today;

                scheduledCourseSectionView.TenantId = scheduledCourseSectionViewModel.TenantId;
                scheduledCourseSectionView._tenantName = scheduledCourseSectionViewModel._tenantName;
                scheduledCourseSectionView.SchoolId = scheduledCourseSectionViewModel.SchoolId;
                scheduledCourseSectionView.StaffId = scheduledCourseSectionViewModel.StaffId;
                scheduledCourseSectionView._token = scheduledCourseSectionViewModel._token;
                scheduledCourseSectionView._userName = scheduledCourseSectionViewModel._userName;
                scheduledCourseSectionView.AllCourse = scheduledCourseSectionViewModel.AllCourse;

                var scheduledCourseSectionData = this.context?.StaffCoursesectionSchedule.Include(x => x.CourseSection).Include(x => x.CourseSection.Course).Include(x => x.CourseSection.SchoolCalendars).Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.StaffId == scheduledCourseSectionViewModel.StaffId && x.IsDropped != true).ToList();

                if (scheduledCourseSectionData != null && scheduledCourseSectionData.Any())
                {
                    if (scheduledCourseSectionViewModel.AllCourse != true)
                    {
                        staffCoursesectionScheduleList = scheduledCourseSectionData.Where(x => x.DurationEndDate >= todayDate && (x.MeetingDays.ToLower().Contains(todayDate.DayOfWeek.ToString().ToLower()) || x.MeetingDays == "Calendar Days" || x.MeetingDays == "Block Days")).ToList();
                    }
                    else
                    {

                        if(scheduledCourseSectionViewModel.MarkingPeriodStartDate != null && scheduledCourseSectionViewModel.MarkingPeriodEndDate != null)
                        {
                            staffCoursesectionScheduleList = scheduledCourseSectionData.Where(x => x.CourseSection.DurationBasedOnPeriod == false || ((scheduledCourseSectionViewModel.MarkingPeriodStartDate >= x.DurationStartDate && scheduledCourseSectionViewModel.MarkingPeriodStartDate <= x.DurationEndDate) && (scheduledCourseSectionViewModel.MarkingPeriodEndDate >= x.DurationStartDate && scheduledCourseSectionViewModel.MarkingPeriodEndDate <= x.DurationEndDate))).ToList();
                        }
                        else
                        {
                            scheduledCourseSectionView._failure = true;
                            scheduledCourseSectionView._message = "Please send Marking Period Start Date and Marking Period End Date";
                            return scheduledCourseSectionView;
                        }
                    }
                }
                else
                {
                    scheduledCourseSectionView._failure = true;
                    scheduledCourseSectionView._message = NORECORDFOUND;
                    return scheduledCourseSectionView;
                }


                if (staffCoursesectionScheduleList.Any())
                {
                    foreach (var scheduledCourseSection in staffCoursesectionScheduleList)
                    {
                        List<DateTime> holidayList = new();
                        //Calculate Holiday
                        var CalendarEventsData = this.context?.CalendarEvents.Where(e => e.TenantId == scheduledCourseSectionViewModel.TenantId && e.CalendarId == scheduledCourseSection.CourseSection.CalendarId && (e.StartDate >= scheduledCourseSection.DurationStartDate && e.StartDate <= scheduledCourseSection.DurationEndDate || e.EndDate >= scheduledCourseSection.DurationStartDate && e.EndDate <= scheduledCourseSection.DurationEndDate) && e.IsHoliday == true && (e.SchoolId == scheduledCourseSectionViewModel.SchoolId || e.ApplicableToAllSchool == true)).ToList();

                        if (CalendarEventsData != null && CalendarEventsData.Any())
                        {
                            foreach (var calender in CalendarEventsData)
                            {
                                if (calender.EndDate != null && calender.StartDate != null)
                                {
                                    if (calender.EndDate.Value.Date > calender.StartDate.Value.Date)
                                    {
                                        var date = Enumerable.Range(0, 1 + (calender.EndDate.Value.Date - calender.StartDate.Value.Date).Days)
                                           .Select(i => calender.StartDate.Value.Date.AddDays(i))
                                           .ToList();
                                        holidayList.AddRange(date);
                                    }
                                    holidayList.Add(calender.StartDate.Value.Date);
                                }

                            }
                        }

                        if (scheduledCourseSection.CourseSection.ScheduleType == "Fixed Schedule (1)" || scheduledCourseSection.CourseSection.ScheduleType == "Variable Schedule (2)" || scheduledCourseSection.CourseSection.ScheduleType == "Block Schedule (4)")
                        {
                            CourseSectionViewList CourseSections = new CourseSectionViewList
                            {
                                CourseTitle = scheduledCourseSection.CourseSection.Course.CourseTitle
                            };

                            if (scheduledCourseSection.CourseSection.ScheduleType == "Fixed Schedule (1)")
                            {
                                CourseSections.ScheduleType = "Fixed Schedule";

                                var fixedData = context?.CourseFixedSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).FirstOrDefault(c => c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId);

                                if (fixedData != null)
                                {
                                    fixedData.BlockPeriod!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                    fixedData.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                    fixedData.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                    fixedData.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                    fixedData.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>();
                                    fixedData.Rooms!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                    fixedData.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                    fixedData.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                    fixedData.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                    CourseSections.courseFixedSchedule = fixedData;
                                    CourseSections.HolidayList = holidayList;
                                }
                            }

                            if (scheduledCourseSection.CourseSection.ScheduleType == "Variable Schedule (2)")
                            {
                                CourseSections.ScheduleType = "Variable Schedule";

                                variableData = this.context?.CourseVariableSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

                                if (variableData?.Any() == true)
                                {
                                    variableData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });

                                    CourseSections.courseVariableSchedule = variableData;
                                    CourseSections.HolidayList = holidayList;
                                }
                            }
                            //if (scheduledCourseSection.CourseSection.ScheduleType == "Calendar Schedule (3)")
                            //{
                            //    CourseSections.ScheduleType = "Calendar Schedule";

                            //    calenderData = this.context?.CourseCalendarSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => (scheduledCourseSectionViewModel.AllCourse != true) ? c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId && c.Date.Value.Date == todayDate.Date : c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

                            //    if (calenderData.Any())
                            //    {
                            //        calenderData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = null; x.BlockPeriod.CourseVariableSchedule = null; x.BlockPeriod.CourseCalendarSchedule = null; x.BlockPeriod.CourseBlockSchedule = null; x.BlockPeriod.StudentAttendance = null; x.Rooms.CourseFixedSchedule = null; x.Rooms.CourseVariableSchedule = null; x.Rooms.CourseCalendarSchedule = null; x.Rooms.CourseBlockSchedule = null; });

                            //        CourseSections.courseCalendarSchedule = calenderData;
                            //    }
                            //    else
                            //    {
                            //        break;
                            //    }
                            //}
                            if (scheduledCourseSection.CourseSection.ScheduleType == "Block Schedule (4)")
                            {
                                CourseSections.ScheduleType = "Block Schedule";

                                blockData = this.context?.CourseBlockSchedule.Include(v => v.Block).Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

                                if (blockData?.Any() == true)
                                {
                                    blockData.ForEach(x =>
                                    { /*x.BlockPeriod.CourseFixedSchedule = null!; x.BlockPeriod.CourseVariableSchedule = null!; x.BlockPeriod.CourseCalendarSchedule = null!; x.BlockPeriod.CourseBlockSchedule = null!; x.BlockPeriod.StudentAttendance = null!; x.Rooms.CourseFixedSchedule = null!; x.Rooms.CourseVariableSchedule = null!; x.Rooms.CourseCalendarSchedule = null!; x.Rooms.CourseBlockSchedule = null!;*/
                                        x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                        x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                        x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                        x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                        x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>();
                                        x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                        x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                        x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                        x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                    });

                                    CourseSections.courseBlockSchedule = blockData;
                                    CourseSections.HolidayList = holidayList;

                                    var bellScheduleList = new List<BellSchedule>();
                                    foreach (var block in blockData)
                                    {
                                        var bellScheduleData = this.context?.BellSchedule.Where(c => c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.BlockId == block.BlockId && c.BellScheduleDate >= scheduledCourseSection.DurationStartDate && c.BellScheduleDate <= scheduledCourseSection.DurationEndDate).ToList();
                                        if (bellScheduleData?.Any() == true)
                                            bellScheduleList.AddRange(bellScheduleData);
                                    }
                                    CourseSections.bellScheduleList = bellScheduleList;
                                }
                            }

                            CourseSections.CalendarId = scheduledCourseSection.CourseSection.CalendarId;
                            CourseSections.CourseId = scheduledCourseSection.CourseId;
                            CourseSections.CourseGradeLevel = scheduledCourseSection.CourseSection.Course.CourseGradeLevel;
                            CourseSections.CourseSectionId = scheduledCourseSection.CourseSectionId;
                            CourseSections.GradeScaleType = scheduledCourseSection.CourseSection.GradeScaleType;
                            CourseSections.AttendanceCategoryId = scheduledCourseSection.CourseSection.AttendanceCategoryId;
                            CourseSections.GradeScaleId = scheduledCourseSection.CourseSection.GradeScaleId;
                            CourseSections.StandardGradeScaleId = scheduledCourseSection.CourseSection.StandardGradeScaleId;
                            CourseSections.CourseSectionName = scheduledCourseSection.CourseSectionName;
                            CourseSections.YrMarkingPeriodId = scheduledCourseSection.YrMarkingPeriodId;
                            CourseSections.SmstrMarkingPeriodId = scheduledCourseSection.SmstrMarkingPeriodId;
                            CourseSections.QtrMarkingPeriodId = scheduledCourseSection.QtrMarkingPeriodId;
                            CourseSections.PrgrsprdMarkingPeriodId = scheduledCourseSection.PrgrsprdMarkingPeriodId;
                            CourseSections.DurationStartDate = scheduledCourseSection.DurationStartDate;
                            CourseSections.DurationEndDate = scheduledCourseSection.DurationEndDate;
                            CourseSections.MeetingDays = scheduledCourseSection.MeetingDays;
                            CourseSections.AttendanceTaken = scheduledCourseSection.CourseSection.AttendanceTaken;
                            CourseSections.WeekDays = scheduledCourseSection.CourseSection.SchoolCalendars!.Days;

                            scheduledCourseSectionView.courseSectionViewList.Add(CourseSections);

                            //for assigmnent grade due date notification
                            var assignmentData = this.context?.Assignment.Include(x => x.GradebookGrades).Where(c => c.SchoolId == scheduledCourseSectionViewModel.SchoolId && c.TenantId == scheduledCourseSectionViewModel.TenantId && c.CourseSectionId == CourseSections.CourseSectionId).ToList();
                            if (assignmentData?.Any() == true)
                            {
                                DateTime? GradePostingEndDate = null;
                                var assignmentTitle = assignmentData.Select(s => s.AssignmentTitle).ToList();

                                if (CourseSections.YrMarkingPeriodId != null)
                                {
                                    var yrData = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseSections.YrMarkingPeriodId && x.PostStartDate <= todayDate);
                                    if (yrData != null)
                                    {
                                        GradePostingEndDate = yrData.PostEndDate;
                                    }
                                }

                                if (CourseSections.SmstrMarkingPeriodId != null)
                                {
                                    var smstrData = this.context?.Semesters.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseSections.SmstrMarkingPeriodId && x.PostStartDate <= todayDate);
                                    if (smstrData != null)
                                    {
                                        GradePostingEndDate = smstrData.PostEndDate;
                                    }
                                }

                                if (CourseSections.QtrMarkingPeriodId != null)
                                {
                                    var qtrData = this.context?.Quarters.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseSections.QtrMarkingPeriodId && x.PostStartDate <= todayDate);
                                    if (qtrData != null)
                                    {
                                        GradePostingEndDate = qtrData.PostEndDate;
                                    }
                                }

                                if (CourseSections.PrgrsprdMarkingPeriodId != null)
                                {
                                    var ppData = this.context?.ProgressPeriods.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseSections.PrgrsprdMarkingPeriodId && x.PostStartDate <= todayDate);
                                    if (ppData != null)
                                    {
                                        GradePostingEndDate = ppData.PostEndDate;
                                    }
                                }

                                if (GradePostingEndDate != null)
                                {
                                    foreach (var assignment in assignmentData)
                                    {
                                        if (assignment.GradebookGrades.Count == 0)
                                        {
                                            var notification = assignment.AssignmentTitle + " " + "Assignment Grades due on" + " " + GradePostingEndDate.Value.ToShortDateString();
                                            scheduledCourseSectionView.NotificationList!.Add(notification);
                                        }

                                    }

                                }
                            }
                        }
                        else
                        {
                            if (scheduledCourseSection.CourseSection.ScheduleType == "Calendar Schedule (3)")
                            {
                                CourseSectionViewList CourseSection = new CourseSectionViewList
                                {
                                    ScheduleType = "Calendar Schedule"
                                };

                                calenderData = this.context?.CourseCalendarSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => (scheduledCourseSectionViewModel.AllCourse != true) ? c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId && c.Date.Value.Date == todayDate.Date : c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

                                if (calenderData != null && calenderData.Any())
                                {
                                    calenderData.ForEach(x =>
                                    { /*x.BlockPeriod.CourseFixedSchedule = null!; x.BlockPeriod.CourseVariableSchedule = null!; x.BlockPeriod.CourseCalendarSchedule = null!; x.BlockPeriod.CourseBlockSchedule = null!; x.BlockPeriod.StudentAttendance = null!; x.Rooms.CourseFixedSchedule = null!; x.Rooms.CourseVariableSchedule = null!; x.Rooms.CourseCalendarSchedule = null!; x.Rooms.CourseBlockSchedule = null!;*/
                                        x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                        x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                        x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                        x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                        x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>();
                                        x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                        x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                        x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                        x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                    });

                                    CourseSection.courseCalendarSchedule = calenderData;
                                    CourseSection.HolidayList = holidayList;

                                    CourseSection.CourseTitle= scheduledCourseSection.CourseSection.Course.CourseTitle;
                                    CourseSection.CalendarId = scheduledCourseSection.CourseSection.CalendarId;
                                    CourseSection.CourseId = scheduledCourseSection.CourseId;
                                    CourseSection.CourseGradeLevel = scheduledCourseSection.CourseSection.Course.CourseGradeLevel;
                                    CourseSection.CourseSectionId = scheduledCourseSection.CourseSectionId;
                                    CourseSection.GradeScaleType = scheduledCourseSection.CourseSection.GradeScaleType;
                                    CourseSection.AttendanceCategoryId = scheduledCourseSection.CourseSection.AttendanceCategoryId;
                                    CourseSection.GradeScaleId = scheduledCourseSection.CourseSection.GradeScaleId;
                                    CourseSection.StandardGradeScaleId = scheduledCourseSection.CourseSection.StandardGradeScaleId;
                                    CourseSection.CourseSectionName = scheduledCourseSection.CourseSectionName;
                                    CourseSection.YrMarkingPeriodId = scheduledCourseSection.YrMarkingPeriodId;
                                    CourseSection.SmstrMarkingPeriodId = scheduledCourseSection.SmstrMarkingPeriodId;
                                    CourseSection.PrgrsprdMarkingPeriodId = scheduledCourseSection.PrgrsprdMarkingPeriodId;
                                    CourseSection.DurationStartDate = scheduledCourseSection.DurationStartDate;
                                    CourseSection.DurationEndDate = scheduledCourseSection.DurationEndDate;
                                    CourseSection.MeetingDays = scheduledCourseSection.MeetingDays;
                                    CourseSection.AttendanceTaken = scheduledCourseSection.CourseSection.AttendanceTaken;
                                    CourseSection.WeekDays = scheduledCourseSection.CourseSection.SchoolCalendars!.Days;

                                    scheduledCourseSectionView.courseSectionViewList.Add(CourseSection);

                                    //for assigmnent grade due date notification
                                    var assignmentData = this.context?.Assignment.Include(x => x.GradebookGrades).Where(c => c.SchoolId == scheduledCourseSectionViewModel.SchoolId && c.TenantId == scheduledCourseSectionViewModel.TenantId && c.CourseSectionId == CourseSection.CourseSectionId).ToList();
                                    if (assignmentData?.Any() == true)
                                    {
                                        DateTime? GradePostingEndDate = null;
                                        var assignmentTitle = assignmentData.Select(s => s.AssignmentTitle).ToList();

                                        if (CourseSection.YrMarkingPeriodId != null)
                                        {
                                            var yrData = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseSection.YrMarkingPeriodId && x.PostStartDate <= todayDate);
                                            if (yrData != null)
                                            {
                                                GradePostingEndDate = yrData.PostEndDate;
                                            }
                                        }

                                        if (CourseSection.SmstrMarkingPeriodId != null)
                                        {
                                            var smstrData = this.context?.Semesters.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseSection.SmstrMarkingPeriodId && x.PostStartDate <= todayDate);
                                            if (smstrData != null)
                                            {
                                                GradePostingEndDate = smstrData.PostEndDate;
                                            }
                                        }

                                        if (CourseSection.QtrMarkingPeriodId != null)
                                        {
                                            var qtrData = this.context?.Quarters.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseSection.QtrMarkingPeriodId && x.PostStartDate <= todayDate);
                                            if (qtrData != null)
                                            {
                                                GradePostingEndDate = qtrData.PostEndDate;
                                            }
                                        }

                                        if (CourseSection.PrgrsprdMarkingPeriodId != null)
                                        {
                                            var ppData = this.context?.ProgressPeriods.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseSection.PrgrsprdMarkingPeriodId && x.PostStartDate <= todayDate);
                                            if (ppData != null)
                                            {
                                                GradePostingEndDate = ppData.PostEndDate;
                                            }
                                        }

                                        if (GradePostingEndDate != null)
                                        {
                                            foreach (var assignment in assignmentData)
                                            {
                                                if (assignment.GradebookGrades.Count == 0)
                                                {
                                                    var notification = assignment.AssignmentTitle + " " + "Assignment Grades due on" + " " + GradePostingEndDate.Value.ToShortDateString();
                                                    scheduledCourseSectionView.NotificationList!.Add(notification);
                                                }

                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    scheduledCourseSectionView._failure = true;
                    scheduledCourseSectionView._message = NORECORDFOUND;
                    return scheduledCourseSectionView;
                }

                int count = 0;

                if (scheduledCourseSectionData.Any())
                {
                    allCourseSectionVewList = this.context?.AllCourseSectionView.Where(c => c.TenantId == scheduledCourseSectionViewModel.TenantId && c.SchoolId == scheduledCourseSectionViewModel.SchoolId).ToList();

                    foreach (var StaffScheduleData in scheduledCourseSectionData)
                    {
                        List<DateTime> holidayList = new List<DateTime>();

                        var allCourseSectionViewData = allCourseSectionVewList!.Where(v => v.CourseSectionId == StaffScheduleData.CourseSectionId && v.CourseId == StaffScheduleData.CourseId && (v.AttendanceTaken == true || v.TakeAttendanceCalendar == true || v.TakeAttendanceVariable == true || v.TakeAttendanceBlock == true)).ToList();

                        if (allCourseSectionViewData.Count > 0)
                        {
                            //Calculate Holiday
                            var CalendarEventsData = this.context?.CalendarEvents.Where(e => e.TenantId == scheduledCourseSectionViewModel.TenantId && e.CalendarId == allCourseSectionViewData.FirstOrDefault()!.CalendarId && (e.StartDate >= StaffScheduleData.DurationStartDate && e.StartDate <= StaffScheduleData.DurationEndDate || e.EndDate >= StaffScheduleData.DurationStartDate && e.EndDate <= StaffScheduleData.DurationEndDate) && e.IsHoliday == true && (e.SchoolId == scheduledCourseSectionViewModel.SchoolId || e.ApplicableToAllSchool == true)).ToList();

                            if (CalendarEventsData?.Any() == true)
                            {
                                foreach (var calender in CalendarEventsData)
                                {
                                    if (calender.EndDate != null && calender.StartDate != null)
                                    {
                                        if (calender.EndDate.Value.Date > calender.StartDate.Value.Date)
                                        {
                                            var date = Enumerable.Range(0, 1 + (calender.EndDate.Value.Date - calender.StartDate.Value.Date).Days)
                                               .Select(i => calender.StartDate.Value.Date.AddDays(i))
                                               .ToList();
                                            holidayList.AddRange(date);
                                        }
                                        holidayList.Add(calender.StartDate.Value.Date);
                                    }
                                }
                            }

                            if (StaffScheduleData.CourseSection.ScheduleType == "Block Schedule (4)")
                            {
                                var AllCourseSectionViewData = this.context?.AllCourseSectionView.Where(v => v.SchoolId == StaffScheduleData.SchoolId && v.TenantId == StaffScheduleData.TenantId && v.CourseSectionId == StaffScheduleData.CourseSectionId).ToList();
                                if (AllCourseSectionViewData?.Any() == true)
                                {
                                    foreach (var allCourseSectionVew in AllCourseSectionViewData)
                                    {
                                        var bellScheduleList = this.context?.BellSchedule.Where(v => v.SchoolId == StaffScheduleData.SchoolId && v.TenantId == StaffScheduleData.TenantId && v.BlockId == allCourseSectionVew.BlockId && v.BellScheduleDate >= StaffScheduleData.DurationStartDate && v.BellScheduleDate <= StaffScheduleData.DurationEndDate && v.BellScheduleDate <= DateTime.Today.Date && (!holidayList.Contains(v.BellScheduleDate))).ToList();

                                        if (bellScheduleList != null && bellScheduleList.Any())
                                        {
                                            foreach (var bellSchedule in bellScheduleList)
                                            {
                                                var StudentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(b => b.SchoolId == StaffScheduleData.SchoolId && b.TenantId == StaffScheduleData.TenantId && b.EffectiveStartDate.Value.Date <= bellSchedule.BellScheduleDate.Date && b.CourseSectionId == StaffScheduleData.CourseSectionId).ToList(); //check student's EffectiveStartDate in this course section

                                                if (StudentCoursesectionScheduleData != null && StudentCoursesectionScheduleData.Any())
                                                {
                                                    var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == StaffScheduleData.SchoolId && b.TenantId == StaffScheduleData.TenantId && b.AttendanceDate.Date == bellSchedule.BellScheduleDate && b.CourseSectionId == StaffScheduleData.CourseSectionId && b.CourseId == StaffScheduleData.CourseId && b.PeriodId == allCourseSectionVew.BlockPeriodId);

                                                    if (staffAttendanceData != null && !staffAttendanceData.Any())
                                                    {
                                                        count++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if ((StaffScheduleData.CourseSection.ScheduleType == "Fixed Schedule (1)" && StaffScheduleData.CourseSection.AttendanceTaken == true) || StaffScheduleData.CourseSection.ScheduleType == "Variable Schedule (2)")
                            {
                                List<DateTime> dateList = new List<DateTime>();
                                List<string> list = new List<string>();
                                string[] meetingDays = Array.Empty<string>();


                                DateTime start = (DateTime)StaffScheduleData.CourseSection.DurationStartDate!;
                                DateTime end = (DateTime)StaffScheduleData.CourseSection.DurationEndDate!;

                                //if (StaffScheduleData.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                //{
                                //    list = StaffScheduleData.CourseSection.MeetingDays.Split("|").ToList();

                                //    if (list.Any())
                                //    {
                                //        meetingDays = list.ToArray();
                                //    }
                                //}
                                //if (StaffScheduleData.CourseSection.ScheduleType == "Variable Schedule (2)")
                                //{
                                //    meetingDays = variableData.Select(c => c.Day).ToArray();
                                //}

                                meetingDays = (StaffScheduleData.MeetingDays ?? "").ToLower().Split("|");

                                bool allDays = meetingDays == null || !meetingDays.Any();

                                dateList = Enumerable.Range(0, 1 + end.Subtract(start).Days)
                                                      .Select(offset => start.AddDays(offset))
                                                      .Where(d => allDays || meetingDays!.Contains(d.DayOfWeek.ToString().ToLower()))
                                                      .ToList();

                                if (dateList.Any())
                                {
                                    dateList = dateList.Where(s => dateList.Any(secL => s.Date <= DateTime.Today.Date)).ToList();
                                    //Remove Holiday
                                    dateList = dateList.Where(x => !holidayList.Contains(x.Date)).ToList();
                                }

                                foreach (var date in dateList)
                                {
                                    var StudentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(b => b.SchoolId == StaffScheduleData.SchoolId && b.TenantId == StaffScheduleData.TenantId && b.EffectiveStartDate.Value.Date <= date && b.CourseSectionId == StaffScheduleData.CourseSectionId).ToList();  //check student's EffectiveStartDate in this course section 

                                    if (StudentCoursesectionScheduleData?.Any() == true)
                                    {
                                        if (StaffScheduleData.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                        {
                                            var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == StaffScheduleData.SchoolId && b.TenantId == StaffScheduleData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseSectionId == StaffScheduleData.CourseSectionId && b.PeriodId == allCourseSectionViewData.FirstOrDefault()!.FixedPeriodId);

                                            if (staffAttendanceData?.Any() != true)
                                            {
                                                count++;
                                            }
                                        }

                                        if (StaffScheduleData.CourseSection.ScheduleType == "Variable Schedule (2)")
                                        {
                                            var courseVariableScheduleData = allCourseSectionViewData.Where(e => e.VarDay.ToLower().Contains(date.DayOfWeek.ToString().ToLower()));

                                            if (courseVariableScheduleData != null)
                                            {
                                                foreach (var courseVariableSchedule in courseVariableScheduleData.ToList())
                                                {
                                                    var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == StaffScheduleData.SchoolId && b.TenantId == StaffScheduleData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseSectionId == StaffScheduleData.CourseSectionId && b.PeriodId == courseVariableSchedule.VarPeriodId);

                                                    if (staffAttendanceData?.Any() != true)
                                                    {
                                                        count++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var calendarDataList = allCourseSectionViewData.Where(c => c.CalDate <= DateTime.Today.Date && !holidayList.Contains(c.CalDate.Value.Date));

                                if (calendarDataList.Any())
                                {
                                    foreach (var calenderScheduleData in calendarDataList)
                                    {
                                        var StudentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(b => b.SchoolId == StaffScheduleData.SchoolId && b.TenantId == StaffScheduleData.TenantId && b.EffectiveStartDate.Value.Date <= calenderScheduleData.CalDate && b.CourseSectionId == StaffScheduleData.CourseSectionId).ToList(); //check student's EffectiveStartDate in this course section 

                                        if (StudentCoursesectionScheduleData?.Any() == true)
                                        {
                                            var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == StaffScheduleData.SchoolId && b.TenantId == StaffScheduleData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == calenderScheduleData.CalDate && b.CourseSectionId == StaffScheduleData.CourseSectionId && b.PeriodId == calenderScheduleData.CalPeriodId);

                                            if (staffAttendanceData?.Any() != true)
                                            {
                                                count++;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //scheduledCourseSectionView.MissingAttendanceCount = count;
                    }
                    scheduledCourseSectionView.MissingAttendanceCount = count;


                    //var membership = this.context?.Membership.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.Profile == "Teacher");

                    if (scheduledCourseSectionViewModel.MembershipId != null)
                    {
                        var noticeList = this.context?.Notice.Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.Isactive == true && x.TargetMembershipIds.Contains((scheduledCourseSectionViewModel.MembershipId ?? 0).ToString()) && (x.ValidFrom <= todayDate && todayDate <= x.ValidTo)).OrderByDescending(x => x.ValidFrom).ToList();

                        if (noticeList?.Any() == true)
                        {
                            scheduledCourseSectionView.NoticeList = noticeList;
                        }
                    }
                }
            }
            catch (Exception es)
            {
                // scheduledCourseSectionView.courseSectionViewList = null;
                scheduledCourseSectionView._failure = true;
                scheduledCourseSectionView._message = es.Message;
            }
            return scheduledCourseSectionView;
        }

        public ScheduledCourseSectionViewModel GetDashboardViewForStaff(ScheduledCourseSectionViewModel scheduledCourseSectionViewModel)
        {
            ScheduledCourseSectionViewModel scheduledCourseSectionView = new();
            List<StaffCoursesectionSchedule> staffCoursesectionScheduleList = new();
            List<AllCourseSectionView>? allCourseSectionVewList = new();

            //CourseFixedSchedule fixedData = new();
            List<CourseVariableSchedule>? variableData = new();
            List<CourseCalendarSchedule>? calenderData = new();
            List<CourseBlockSchedule>? blockData = new();

            try
            {
                int count = 0;
                var todayDate = DateTime.Today;

                scheduledCourseSectionView.TenantId = scheduledCourseSectionViewModel.TenantId;
                scheduledCourseSectionView._tenantName = scheduledCourseSectionViewModel._tenantName;
                scheduledCourseSectionView.SchoolId = scheduledCourseSectionViewModel.SchoolId;
                scheduledCourseSectionView.StaffId = scheduledCourseSectionViewModel.StaffId;
                scheduledCourseSectionView._token = scheduledCourseSectionViewModel._token;
                scheduledCourseSectionView._userName = scheduledCourseSectionViewModel._userName;
                scheduledCourseSectionView.AllCourse = scheduledCourseSectionViewModel.AllCourse;

                var scheduledCourseSectionData = this.context?.StaffCoursesectionSchedule.Include(x => x.CourseSection).Include(x => x.CourseSection.Course).Include(x => x.CourseSection.SchoolCalendars).Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.StaffId == scheduledCourseSectionViewModel.StaffId && x.IsDropped != true).ToList();
                

                if (scheduledCourseSectionData != null && scheduledCourseSectionData.Any())
                {
                    if (scheduledCourseSectionViewModel.AllCourse != true)
                    {
                        staffCoursesectionScheduleList = scheduledCourseSectionData.Where(x => x.DurationEndDate >= todayDate && x.DurationStartDate <= todayDate && (x.MeetingDays.ToLower().Contains(todayDate.DayOfWeek.ToString().ToLower()) || x.MeetingDays == "Calendar Days" || x.MeetingDays == "Block Days")).ToList();
                    }
                    else
                    {

                        if (scheduledCourseSectionViewModel.MarkingPeriodStartDate != null && scheduledCourseSectionViewModel.MarkingPeriodEndDate != null)
                        {
                            staffCoursesectionScheduleList = scheduledCourseSectionData.Where(x => x.CourseSection.DurationBasedOnPeriod == false || ((scheduledCourseSectionViewModel.MarkingPeriodStartDate >= x.DurationStartDate && scheduledCourseSectionViewModel.MarkingPeriodStartDate <= x.DurationEndDate) && (scheduledCourseSectionViewModel.MarkingPeriodEndDate >= x.DurationStartDate && scheduledCourseSectionViewModel.MarkingPeriodEndDate <= x.DurationEndDate))).ToList();
                        }
                        else
                        {
                            scheduledCourseSectionView._failure = true;
                            scheduledCourseSectionView._message = "Please send Marking Period Start Date and Marking Period End Date";
                            return scheduledCourseSectionView;
                        }
                    }
                }
                else
                {
                    scheduledCourseSectionView._failure = true;
                    scheduledCourseSectionView._message = NORECORDFOUND;
                    return scheduledCourseSectionView;
                }

                if (staffCoursesectionScheduleList.Any())
                {
                    foreach (var scheduledCourseSection in staffCoursesectionScheduleList)
                    {
                        List<DateTime> holidayList = new();
                        //Calculate Holiday
                        var CalendarEventsData = this.context?.CalendarEvents.Where(e => e.TenantId == scheduledCourseSectionViewModel.TenantId && e.CalendarId == scheduledCourseSection.CourseSection.CalendarId && (e.StartDate >= scheduledCourseSection.DurationStartDate && e.StartDate <= scheduledCourseSection.DurationEndDate || e.EndDate >= scheduledCourseSection.DurationStartDate && e.EndDate <= scheduledCourseSection.DurationEndDate) && e.IsHoliday == true && (e.SchoolId == scheduledCourseSectionViewModel.SchoolId || e.ApplicableToAllSchool == true)).ToList();

                        if (CalendarEventsData != null && CalendarEventsData.Any())
                        {
                            foreach (var calender in CalendarEventsData)
                            {
                                if (calender.EndDate != null && calender.StartDate != null)
                                {
                                    if (calender.EndDate.Value.Date > calender.StartDate.Value.Date)
                                    {
                                        var date = Enumerable.Range(0, 1 + (calender.EndDate.Value.Date - calender.StartDate.Value.Date).Days)
                                           .Select(i => calender.StartDate.Value.Date.AddDays(i))
                                           .ToList();
                                        holidayList.AddRange(date);
                                    }
                                    holidayList.Add(calender.StartDate.Value.Date);
                                }

                            }
                        }

                        if (scheduledCourseSection.CourseSection.ScheduleType == "Fixed Schedule (1)" || scheduledCourseSection.CourseSection.ScheduleType == "Variable Schedule (2)" || scheduledCourseSection.CourseSection.ScheduleType == "Block Schedule (4)")
                        {
                            CourseSectionViewList CourseSections = new CourseSectionViewList
                            {
                                CourseTitle = scheduledCourseSection.CourseSection.Course.CourseTitle
                            };

                            if (scheduledCourseSection.CourseSection.ScheduleType == "Fixed Schedule (1)")
                            {
                                CourseSections.ScheduleType = "Fixed Schedule";

                                var studentMissingAttendanceData = this.context?.StudentMissingAttendances.Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.CourseSectionId == scheduledCourseSection.CourseSectionId && x.MissingAttendanceDate >= scheduledCourseSection.DurationStartDate && x.MissingAttendanceDate <= scheduledCourseSection.DurationEndDate).ToList();

                                if (studentMissingAttendanceData != null && studentMissingAttendanceData.Any() == true)
                                {
                                    {
                                        count += studentMissingAttendanceData.Count;
                                    }
                                }

                                var fixedData = context?.CourseFixedSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).FirstOrDefault(c => c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId);

                                if (fixedData != null)
                                {
                                    //fixedData.BlockPeriod!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                    //fixedData.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                    //fixedData.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                    //fixedData.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                    //fixedData.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>();
                                    //fixedData.BlockPeriod.StudentMissingAttendances = new List<StudentMissingAttendance>();
                                    //fixedData.Rooms!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                    //fixedData.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                    //fixedData.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                    //fixedData.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                    CourseSections.courseFixedSchedule = fixedData;
                                    CourseSections.HolidayList = holidayList;
                                }
                            }

                            if (scheduledCourseSection.CourseSection.ScheduleType == "Variable Schedule (2)")
                            {
                                CourseSections.ScheduleType = "Variable Schedule";

                                var studentMissingAttendanceData = this.context?.StudentMissingAttendances.Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.CourseSectionId == scheduledCourseSection.CourseSectionId && x.MissingAttendanceDate >= scheduledCourseSection.DurationStartDate && x.MissingAttendanceDate <= scheduledCourseSection.DurationEndDate).ToList();

                                if (studentMissingAttendanceData != null && studentMissingAttendanceData.Any() == true)
                                {
                                    {
                                        count += studentMissingAttendanceData.Count;
                                    }
                                }

                                variableData = this.context?.CourseVariableSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

                                if (variableData?.Any() == true)
                                {
                                    //variableData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); x.BlockPeriod.StudentMissingAttendances = new HashSet<StudentMissingAttendance>(); x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });

                                    CourseSections.courseVariableSchedule = variableData;
                                    CourseSections.HolidayList = holidayList;
                                }
                            }

                            if (scheduledCourseSection.CourseSection.ScheduleType == "Block Schedule (4)")
                            {
                                CourseSections.ScheduleType = "Block Schedule";

                                var studentMissingAttendanceData = this.context?.StudentMissingAttendances.Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.CourseSectionId == scheduledCourseSection.CourseSectionId && x.MissingAttendanceDate >= scheduledCourseSection.DurationStartDate && x.MissingAttendanceDate <= scheduledCourseSection.DurationEndDate).ToList();

                                if (studentMissingAttendanceData != null && studentMissingAttendanceData.Any() == true)
                                {
                                    {
                                        count += studentMissingAttendanceData.Count;
                                    }
                                }

                                blockData = this.context?.CourseBlockSchedule.Include(v => v.Block).Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

                                if (blockData?.Any() == true)
                                {
                                    //blockData.ForEach(x =>
                                    //{
                                    //    x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                    //    x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                    //    x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                    //    x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                    //    x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>();
                                    //    x.BlockPeriod.StudentMissingAttendances = new HashSet<StudentMissingAttendance>();
                                    //    x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                    //    x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                    //    x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                    //    x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                    //});

                                    CourseSections.courseBlockSchedule = blockData;
                                    CourseSections.HolidayList = holidayList;

                                    var bellScheduleList = new List<BellSchedule>();
                                    foreach (var block in blockData)
                                    {
                                        var bellScheduleData = this.context?.BellSchedule.Where(c => c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.BlockId == block.BlockId && c.BellScheduleDate >= scheduledCourseSection.DurationStartDate && c.BellScheduleDate <= scheduledCourseSection.DurationEndDate).ToList();
                                        if (bellScheduleData?.Any() == true)
                                            bellScheduleList.AddRange(bellScheduleData);
                                    }
                                    CourseSections.bellScheduleList = bellScheduleList;
                                }
                            }

                            CourseSections.CalendarId = scheduledCourseSection.CourseSection.CalendarId;
                            CourseSections.CourseId = scheduledCourseSection.CourseId;
                            CourseSections.CourseGradeLevel = scheduledCourseSection.CourseSection.Course.CourseGradeLevel;
                            CourseSections.CourseSectionId = scheduledCourseSection.CourseSectionId;
                            CourseSections.GradeScaleType = scheduledCourseSection.CourseSection.GradeScaleType;
                            CourseSections.AttendanceCategoryId = scheduledCourseSection.CourseSection.AttendanceCategoryId;
                            CourseSections.GradeScaleId = scheduledCourseSection.CourseSection.GradeScaleId;
                            CourseSections.StandardGradeScaleId = scheduledCourseSection.CourseSection.StandardGradeScaleId;
                            CourseSections.CourseSectionName = scheduledCourseSection.CourseSectionName;
                            CourseSections.YrMarkingPeriodId = scheduledCourseSection.YrMarkingPeriodId;
                            CourseSections.SmstrMarkingPeriodId = scheduledCourseSection.SmstrMarkingPeriodId;
                            CourseSections.QtrMarkingPeriodId = scheduledCourseSection.QtrMarkingPeriodId;
                            CourseSections.PrgrsprdMarkingPeriodId = scheduledCourseSection.PrgrsprdMarkingPeriodId;
                            CourseSections.DurationStartDate = scheduledCourseSection.DurationStartDate;
                            CourseSections.DurationEndDate = scheduledCourseSection.DurationEndDate;
                            CourseSections.MeetingDays = scheduledCourseSection.MeetingDays;
                            CourseSections.AttendanceTaken = scheduledCourseSection.CourseSection.AttendanceTaken;
                            CourseSections.WeekDays = scheduledCourseSection.CourseSection.SchoolCalendars!.Days;

                            scheduledCourseSectionView.courseSectionViewList.Add(CourseSections);

                            //for assigmnent grade due date notification
                            var assignmentData = this.context?.Assignment.Include(x => x.GradebookGrades).Where(c => c.SchoolId == scheduledCourseSectionViewModel.SchoolId && c.TenantId == scheduledCourseSectionViewModel.TenantId && c.CourseSectionId == CourseSections.CourseSectionId).ToList();
                            if (assignmentData?.Any() == true)
                            {
                                DateTime? GradePostingEndDate = null;
                                var assignmentTitle = assignmentData.Select(s => s.AssignmentTitle).ToList();

                                if (CourseSections.YrMarkingPeriodId != null)
                                {
                                    var yrData = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseSections.YrMarkingPeriodId && x.PostStartDate <= todayDate);
                                    if (yrData != null)
                                    {
                                        GradePostingEndDate = yrData.PostEndDate;
                                    }
                                }

                                if (CourseSections.SmstrMarkingPeriodId != null)
                                {
                                    var smstrData = this.context?.Semesters.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseSections.SmstrMarkingPeriodId && x.PostStartDate <= todayDate);
                                    if (smstrData != null)
                                    {
                                        GradePostingEndDate = smstrData.PostEndDate;
                                    }
                                }

                                if (CourseSections.QtrMarkingPeriodId != null)
                                {
                                    var qtrData = this.context?.Quarters.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseSections.QtrMarkingPeriodId && x.PostStartDate <= todayDate);
                                    if (qtrData != null)
                                    {
                                        GradePostingEndDate = qtrData.PostEndDate;
                                    }
                                }

                                if (CourseSections.PrgrsprdMarkingPeriodId != null)
                                {
                                    var ppData = this.context?.ProgressPeriods.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseSections.PrgrsprdMarkingPeriodId && x.PostStartDate <= todayDate);
                                    if (ppData != null)
                                    {
                                        GradePostingEndDate = ppData.PostEndDate;
                                    }
                                }

                                if (GradePostingEndDate != null)
                                {
                                    foreach (var assignment in assignmentData)
                                    {
                                        if (assignment.GradebookGrades.Count == 0)
                                        {
                                            var notification = assignment.AssignmentTitle + " " + "Assignment Grades due on" + " " + GradePostingEndDate.Value.ToShortDateString();
                                            scheduledCourseSectionView.NotificationList!.Add(notification);
                                        }

                                    }

                                }
                            }
                        }
                        else
                        {
                            if (scheduledCourseSection.CourseSection.ScheduleType == "Calendar Schedule (3)")
                            {
                                CourseSectionViewList CourseSection = new CourseSectionViewList
                                {
                                    ScheduleType = "Calendar Schedule"
                                };

                                var studentMissingAttendanceData = this.context?.StudentMissingAttendances.Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.CourseSectionId == scheduledCourseSection.CourseSectionId && x.MissingAttendanceDate >= scheduledCourseSection.DurationStartDate && x.MissingAttendanceDate <= scheduledCourseSection.DurationEndDate).ToList();

                                if (studentMissingAttendanceData != null && studentMissingAttendanceData.Any() == true)
                                {
                                    {
                                        count += studentMissingAttendanceData.Count;
                                    }
                                }

                                calenderData = this.context?.CourseCalendarSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => (scheduledCourseSectionViewModel.AllCourse != true) ? c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId && c.Date.Value.Date == todayDate.Date : c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

                                if (calenderData != null && calenderData.Any())
                                {
                                    //calenderData.ForEach(x =>
                                    //{
                                    //    x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                    //    x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                    //    x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                    //    x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                    //    x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>();
                                    //    x.BlockPeriod.StudentMissingAttendances = new HashSet<StudentMissingAttendance>();
                                    //    x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                    //    x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                    //    x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                    //    x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                    //});

                                    CourseSection.courseCalendarSchedule = calenderData;
                                    CourseSection.HolidayList = holidayList;

                                    CourseSection.CourseTitle = scheduledCourseSection.CourseSection.Course.CourseTitle;
                                    CourseSection.CalendarId = scheduledCourseSection.CourseSection.CalendarId;
                                    CourseSection.CourseId = scheduledCourseSection.CourseId;
                                    CourseSection.CourseGradeLevel = scheduledCourseSection.CourseSection.Course.CourseGradeLevel;
                                    CourseSection.CourseSectionId = scheduledCourseSection.CourseSectionId;
                                    CourseSection.GradeScaleType = scheduledCourseSection.CourseSection.GradeScaleType;
                                    CourseSection.AttendanceCategoryId = scheduledCourseSection.CourseSection.AttendanceCategoryId;
                                    CourseSection.GradeScaleId = scheduledCourseSection.CourseSection.GradeScaleId;
                                    CourseSection.StandardGradeScaleId = scheduledCourseSection.CourseSection.StandardGradeScaleId;
                                    CourseSection.CourseSectionName = scheduledCourseSection.CourseSectionName;
                                    CourseSection.YrMarkingPeriodId = scheduledCourseSection.YrMarkingPeriodId;
                                    CourseSection.SmstrMarkingPeriodId = scheduledCourseSection.SmstrMarkingPeriodId;
                                    CourseSection.PrgrsprdMarkingPeriodId = scheduledCourseSection.PrgrsprdMarkingPeriodId;
                                    CourseSection.DurationStartDate = scheduledCourseSection.DurationStartDate;
                                    CourseSection.DurationEndDate = scheduledCourseSection.DurationEndDate;
                                    CourseSection.MeetingDays = scheduledCourseSection.MeetingDays;
                                    CourseSection.AttendanceTaken = scheduledCourseSection.CourseSection.AttendanceTaken;
                                    CourseSection.WeekDays = scheduledCourseSection.CourseSection.SchoolCalendars!.Days;

                                    scheduledCourseSectionView.courseSectionViewList.Add(CourseSection);

                                    //for assigmnent grade due date notification
                                    var assignmentData = this.context?.Assignment.Include(x => x.GradebookGrades).Where(c => c.SchoolId == scheduledCourseSectionViewModel.SchoolId && c.TenantId == scheduledCourseSectionViewModel.TenantId && c.CourseSectionId == CourseSection.CourseSectionId).ToList();
                                    if (assignmentData?.Any() == true)
                                    {
                                        DateTime? GradePostingEndDate = null;
                                        var assignmentTitle = assignmentData.Select(s => s.AssignmentTitle).ToList();

                                        if (CourseSection.YrMarkingPeriodId != null)
                                        {
                                            var yrData = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseSection.YrMarkingPeriodId && x.PostStartDate <= todayDate);
                                            if (yrData != null)
                                            {
                                                GradePostingEndDate = yrData.PostEndDate;
                                            }
                                        }

                                        if (CourseSection.SmstrMarkingPeriodId != null)
                                        {
                                            var smstrData = this.context?.Semesters.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseSection.SmstrMarkingPeriodId && x.PostStartDate <= todayDate);
                                            if (smstrData != null)
                                            {
                                                GradePostingEndDate = smstrData.PostEndDate;
                                            }
                                        }

                                        if (CourseSection.QtrMarkingPeriodId != null)
                                        {
                                            var qtrData = this.context?.Quarters.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseSection.QtrMarkingPeriodId && x.PostStartDate <= todayDate);
                                            if (qtrData != null)
                                            {
                                                GradePostingEndDate = qtrData.PostEndDate;
                                            }
                                        }

                                        if (CourseSection.PrgrsprdMarkingPeriodId != null)
                                        {
                                            var ppData = this.context?.ProgressPeriods.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseSection.PrgrsprdMarkingPeriodId && x.PostStartDate <= todayDate);
                                            if (ppData != null)
                                            {
                                                GradePostingEndDate = ppData.PostEndDate;
                                            }
                                        }

                                        if (GradePostingEndDate != null)
                                        {
                                            foreach (var assignment in assignmentData)
                                            {
                                                if (assignment.GradebookGrades.Count == 0)
                                                {
                                                    var notification = assignment.AssignmentTitle + " " + "Assignment Grades due on" + " " + GradePostingEndDate.Value.ToShortDateString();
                                                    scheduledCourseSectionView.NotificationList!.Add(notification);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    scheduledCourseSectionView.MissingAttendanceCount = count;

                    foreach (var courseSection in scheduledCourseSectionView.courseSectionViewList)
                    {
                        if (courseSection.courseFixedSchedule != null)
                        {
                            courseSection.courseFixedSchedule.BlockPeriod!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                            courseSection.courseFixedSchedule.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                            courseSection.courseFixedSchedule.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                            courseSection.courseFixedSchedule.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                            courseSection.courseFixedSchedule.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>();
                            courseSection.courseFixedSchedule.BlockPeriod.StudentMissingAttendances = new List<StudentMissingAttendance>();
                            courseSection.courseFixedSchedule.Rooms!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                            courseSection.courseFixedSchedule.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                            courseSection.courseFixedSchedule.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                            courseSection.courseFixedSchedule.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                        }
                        else if (courseSection.courseVariableSchedule?.Any() == true)
                        {
                            courseSection.courseVariableSchedule.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); x.BlockPeriod.StudentMissingAttendances = new HashSet<StudentMissingAttendance>(); x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });
                        }
                        else if (courseSection.courseCalendarSchedule?.Any() == true)
                        {
                            courseSection.courseCalendarSchedule.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); x.BlockPeriod.StudentMissingAttendances = new HashSet<StudentMissingAttendance>(); x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });
                        }
                        else if (courseSection.courseBlockSchedule?.Any() == true)
                        {
                            courseSection.courseBlockSchedule.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); x.BlockPeriod.StudentMissingAttendances = new HashSet<StudentMissingAttendance>(); x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });
                        }
                    }
                }
                else
                {
                    scheduledCourseSectionView._failure = true;
                    scheduledCourseSectionView._message = NORECORDFOUND;
                    return scheduledCourseSectionView;
                }

                if (scheduledCourseSectionViewModel.MembershipId != null)
                {
                    var noticeList = this.context?.Notice.Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && (x.SchoolId == scheduledCourseSectionViewModel.SchoolId || (x.SchoolId != scheduledCourseSectionViewModel.SchoolId && x.VisibleToAllSchool == true)) && x.Isactive == true && x.TargetMembershipIds.Contains((scheduledCourseSectionViewModel.MembershipId ?? 0).ToString()) && (x.ValidFrom <= todayDate && todayDate <= x.ValidTo)).OrderByDescending(x => x.ValidFrom).ToList();

                    if (noticeList?.Any() == true)
                    {
                        scheduledCourseSectionView.NoticeList = noticeList;
                    }
                }

            }
            catch (Exception es)
            {
                // scheduledCourseSectionView.courseSectionViewList = null;
                scheduledCourseSectionView._failure = true;
                scheduledCourseSectionView._message = es.Message;
            }
            return scheduledCourseSectionView;
        }

        //public ScheduledCourseSectionViewModel GetDashboardViewForStaff(ScheduledCourseSectionViewModel scheduledCourseSectionViewModel)
        //{
        //    ScheduledCourseSectionViewModel scheduledCourseSectionView = new();
        //    List<StaffCoursesectionSchedule> staffCoursesectionScheduleList = new();
        //    List<AllCourseSectionView>? allCourseSectionVewList = new();

        //    CourseFixedSchedule? fixedData = null;
        //    List<CourseVariableSchedule>? variableData = new();
        //    List<CourseCalendarSchedule>? calenderData = new();
        //    List<CourseBlockSchedule>? blockData = new();

        //    try
        //    {
        //        var todayDate = DateTime.Today;

        //        scheduledCourseSectionView.TenantId = scheduledCourseSectionViewModel.TenantId;
        //        scheduledCourseSectionView._tenantName = scheduledCourseSectionViewModel._tenantName;
        //        scheduledCourseSectionView.SchoolId = scheduledCourseSectionViewModel.SchoolId;
        //        scheduledCourseSectionView.StaffId = scheduledCourseSectionViewModel.StaffId;
        //        scheduledCourseSectionView._token = scheduledCourseSectionViewModel._token;
        //        scheduledCourseSectionView._userName = scheduledCourseSectionViewModel._userName;
        //        scheduledCourseSectionView.AllCourse = scheduledCourseSectionViewModel.AllCourse;

        //        var scheduledCourseSectionData = this.context?.StaffCoursesectionSchedule.Include(x => x.CourseSection).Include(x => x.CourseSection.Course).Include(x => x.CourseSection.SchoolCalendars).Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.StaffId == scheduledCourseSectionViewModel.StaffId && x.IsDropped != true).ToList();

        //        if (scheduledCourseSectionData != null && scheduledCourseSectionData.Any())
        //        {
        //            if (scheduledCourseSectionViewModel.AllCourse != true)
        //            {
        //                staffCoursesectionScheduleList = scheduledCourseSectionData.Where(x => x.DurationEndDate >= todayDate && (x.MeetingDays.ToLower().Contains(todayDate.DayOfWeek.ToString().ToLower()) || x.MeetingDays == "Calendar Days" || x.MeetingDays == "Block Days")).ToList();
        //            }
        //            else
        //            {
        //                staffCoursesectionScheduleList = scheduledCourseSectionData;
        //            }
        //        }
        //        else
        //        {
        //            scheduledCourseSectionView._failure = true;
        //            scheduledCourseSectionView._message = NORECORDFOUND;
        //            return scheduledCourseSectionView;
        //        }

        //        if (staffCoursesectionScheduleList.Any())
        //        {
        //            foreach (var scheduledCourseSection in staffCoursesectionScheduleList)
        //            {
        //                List<DateTime> holidayList = new();

        //                //Calculate Holiday List
        //                var CalendarEventsData = this.context?.CalendarEvents.Where(e => e.TenantId == scheduledCourseSectionViewModel.TenantId && e.CalendarId == scheduledCourseSection.CourseSection.CalendarId && (e.StartDate >= scheduledCourseSection.DurationStartDate && e.StartDate <= scheduledCourseSection.DurationEndDate || e.EndDate >= scheduledCourseSection.DurationStartDate && e.EndDate <= scheduledCourseSection.DurationEndDate) && e.IsHoliday == true && (e.SchoolId == scheduledCourseSectionViewModel.SchoolId || e.ApplicableToAllSchool == true)).ToList();

        //                if (CalendarEventsData != null && CalendarEventsData.Any())
        //                {
        //                    foreach (var calender in CalendarEventsData)
        //                    {
        //                        if (calender.EndDate != null && calender.StartDate != null)
        //                        {
        //                            if (calender.EndDate.Value.Date > calender.StartDate.Value.Date)
        //                            {
        //                                var date = Enumerable.Range(0, 1 + (calender.EndDate.Value.Date - calender.StartDate.Value.Date).Days)
        //                                   .Select(i => calender.StartDate.Value.Date.AddDays(i))
        //                                   .ToList();
        //                                holidayList.AddRange(date);
        //                            }
        //                            holidayList.Add(calender.StartDate.Value.Date);
        //                        }
        //                    }
        //                }

        //                CourseSectionViewList CourseSections = new CourseSectionViewList();

        //                if (scheduledCourseSection.CourseSection.ScheduleType == "Fixed Schedule (1)" || scheduledCourseSection.CourseSection.ScheduleType == "Variable Schedule (2)")
        //                {
        //                    if (scheduledCourseSection.CourseSection.ScheduleType == "Fixed Schedule (1)")
        //                    {
        //                        CourseSections.ScheduleType = "Fixed Schedule";

        //                        fixedData = this.context?.CourseFixedSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).FirstOrDefault(c => c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId);

        //                        if (fixedData != null)
        //                        {
        //                            fixedData.BlockPeriod!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); fixedData.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); fixedData.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); fixedData.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); fixedData.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); fixedData.Rooms!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); fixedData.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); fixedData.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); fixedData.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();

        //                            CourseSections.courseFixedSchedule = fixedData;
        //                            CourseSections.HolidayList = holidayList;
        //                        }
        //                    }

        //                    if (scheduledCourseSection.CourseSection.ScheduleType == "Variable Schedule (2)")
        //                    {
        //                        CourseSections.ScheduleType = "Variable Schedule";

        //                        variableData = this.context?.CourseVariableSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

        //                        if (variableData?.Any() == true)
        //                        {
        //                            variableData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });

        //                            CourseSections.courseVariableSchedule = variableData;
        //                            CourseSections.HolidayList = holidayList;
        //                        }
        //                    }
        //                }
        //                if (scheduledCourseSection.CourseSection.ScheduleType == "Calendar Schedule (3)")
        //                {
        //                    CourseSections.ScheduleType = "Calendar Schedule";

        //                    calenderData = this.context?.CourseCalendarSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => (scheduledCourseSectionViewModel.AllCourse != true) ? c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId && c.Date.Value.Date == todayDate.Date : c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

        //                    if (calenderData != null && calenderData.Any())
        //                    {
        //                        calenderData.ForEach(x =>
        //                        {
        //                            x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
        //                        });

        //                        CourseSections.courseCalendarSchedule = calenderData;
        //                        CourseSections.HolidayList = holidayList;
        //                    }
        //                }
        //                if (scheduledCourseSection.CourseSection.ScheduleType == "Block Schedule (4)")
        //                {
        //                    CourseSections.ScheduleType = "Block Schedule";

        //                    blockData = this.context?.CourseBlockSchedule.Include(v => v.Block).Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

        //                    if (blockData?.Any() == true)
        //                    {
        //                        blockData.ForEach(x =>
        //                        {
        //                            x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
        //                        });

        //                        CourseSections.courseBlockSchedule = blockData;
        //                        CourseSections.HolidayList = holidayList;

        //                        var bellScheduleList = new List<BellSchedule>();
        //                        foreach (var block in blockData)
        //                        {
        //                            var bellScheduleData = this.context?.BellSchedule.Where(c => c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.BlockId == block.BlockId && c.BellScheduleDate >= scheduledCourseSection.DurationStartDate && c.BellScheduleDate <= scheduledCourseSection.DurationEndDate).ToList();
        //                            if (bellScheduleData?.Any() == true)
        //                                bellScheduleList.AddRange(bellScheduleData);
        //                        }
        //                        CourseSections.bellScheduleList = bellScheduleList;
        //                    }
        //                }
        //                if (fixedData != null || variableData?.Any() == true || calenderData?.Any() == true || blockData?.Any() == true)
        //                {
        //                    CourseSections.CourseTitle = scheduledCourseSection.CourseSection.Course.CourseTitle;
        //                    CourseSections.CalendarId = scheduledCourseSection.CourseSection.CalendarId;
        //                    CourseSections.CourseId = scheduledCourseSection.CourseId;
        //                    CourseSections.CourseGradeLevel = scheduledCourseSection.CourseSection.Course.CourseGradeLevel;
        //                    CourseSections.CourseSectionId = scheduledCourseSection.CourseSectionId;
        //                    CourseSections.GradeScaleType = scheduledCourseSection.CourseSection.GradeScaleType;
        //                    CourseSections.AttendanceCategoryId = scheduledCourseSection.CourseSection.AttendanceCategoryId;
        //                    CourseSections.GradeScaleId = scheduledCourseSection.CourseSection.GradeScaleId;
        //                    CourseSections.StandardGradeScaleId = scheduledCourseSection.CourseSection.StandardGradeScaleId;
        //                    CourseSections.CourseSectionName = scheduledCourseSection.CourseSectionName;
        //                    CourseSections.YrMarkingPeriodId = scheduledCourseSection.YrMarkingPeriodId;
        //                    CourseSections.SmstrMarkingPeriodId = scheduledCourseSection.SmstrMarkingPeriodId;
        //                    CourseSections.QtrMarkingPeriodId = scheduledCourseSection.QtrMarkingPeriodId;
        //                    CourseSections.DurationStartDate = scheduledCourseSection.DurationStartDate;
        //                    CourseSections.DurationEndDate = scheduledCourseSection.DurationEndDate;
        //                    CourseSections.MeetingDays = scheduledCourseSection.MeetingDays;
        //                    CourseSections.AttendanceTaken = scheduledCourseSection.CourseSection.AttendanceTaken;
        //                    CourseSections.WeekDays = scheduledCourseSection.CourseSection.SchoolCalendars!.Days;

        //                    scheduledCourseSectionView.courseSectionViewList.Add(CourseSections);
        //                }

        //                //for assigmnent grade due date notification
        //                var assignmentData = this.context?.Assignment.Include(x => x.GradebookGrades).Where(c => c.SchoolId == scheduledCourseSectionViewModel.SchoolId && c.TenantId == scheduledCourseSectionViewModel.TenantId && c.CourseSectionId == CourseSections.CourseSectionId).ToList();
        //                if (assignmentData?.Any() == true)
        //                {
        //                    DateTime? GradePostingEndDate = null;
        //                    var assignmentTitle = assignmentData.Select(s => s.AssignmentTitle).ToList();

        //                    if (CourseSections.YrMarkingPeriodId != null)
        //                    {
        //                        var yrData = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseSections.YrMarkingPeriodId && x.PostStartDate <= todayDate);
        //                        if (yrData != null)
        //                        {
        //                            GradePostingEndDate = yrData.PostEndDate;
        //                        }
        //                    }

        //                    if (CourseSections.SmstrMarkingPeriodId != null)
        //                    {
        //                        var smstrData = this.context?.Semesters.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseSections.SmstrMarkingPeriodId && x.PostStartDate <= todayDate);
        //                        if (smstrData != null)
        //                        {
        //                            GradePostingEndDate = smstrData.PostEndDate;
        //                        }
        //                    }

        //                    if (CourseSections.QtrMarkingPeriodId != null)
        //                    {
        //                        var qtrData = this.context?.Quarters.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseSections.QtrMarkingPeriodId && x.PostStartDate <= todayDate);
        //                        if (qtrData != null)
        //                        {
        //                            GradePostingEndDate = qtrData.PostEndDate;
        //                        }
        //                    }

        //                    if (GradePostingEndDate != null)
        //                    {
        //                        foreach (var assignment in assignmentData)
        //                        {
        //                            if (assignment.GradebookGrades.Count == 0)
        //                            {
        //                                var notification = assignment.AssignmentTitle + " " + "Assignment Grades due on" + " " + GradePostingEndDate.Value.Date;
        //                                scheduledCourseSectionView.NotificationList!.Add(notification);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            scheduledCourseSectionView._failure = true;
        //            scheduledCourseSectionView._message = NORECORDFOUND;
        //            return scheduledCourseSectionView;
        //        }

        //        int count = 0;
        //        if (scheduledCourseSectionData.Any())
        //        {
        //            allCourseSectionVewList = this.context?.AllCourseSectionView.Where(c => c.TenantId == scheduledCourseSectionViewModel.TenantId && c.SchoolId == scheduledCourseSectionViewModel.SchoolId).ToList();

        //            foreach (var StaffScheduleData in scheduledCourseSectionData)
        //            {
        //                List<DateTime> holidayList = new List<DateTime>();

        //                var allCourseSectionViewData = allCourseSectionVewList!.Where(v => v.CourseSectionId == StaffScheduleData.CourseSectionId && v.CourseId == StaffScheduleData.CourseId && (v.AttendanceTaken == true || v.TakeAttendanceCalendar == true || v.TakeAttendanceVariable == true || v.TakeAttendanceBlock == true)).ToList();

        //                if (allCourseSectionViewData.Count > 0)
        //                {
        //                    //Calculate Holiday
        //                    var CalendarEventsData = this.context?.CalendarEvents.Where(e => e.TenantId == scheduledCourseSectionViewModel.TenantId && e.CalendarId == allCourseSectionViewData.FirstOrDefault()!.CalendarId && (e.StartDate >= StaffScheduleData.DurationStartDate && e.StartDate <= StaffScheduleData.DurationEndDate || e.EndDate >= StaffScheduleData.DurationStartDate && e.EndDate <= StaffScheduleData.DurationEndDate) && e.IsHoliday == true && (e.SchoolId == scheduledCourseSectionViewModel.SchoolId || e.ApplicableToAllSchool == true)).ToList();

        //                    if (CalendarEventsData?.Any() == true)
        //                    {
        //                        foreach (var calender in CalendarEventsData)
        //                        {
        //                            if (calender.EndDate != null && calender.StartDate != null)
        //                            {
        //                                if (calender.EndDate.Value.Date > calender.StartDate.Value.Date)
        //                                {
        //                                    var date = Enumerable.Range(0, 1 + (calender.EndDate.Value.Date - calender.StartDate.Value.Date).Days)
        //                                       .Select(i => calender.StartDate.Value.Date.AddDays(i))
        //                                       .ToList();
        //                                    holidayList.AddRange(date);
        //                                }
        //                                holidayList.Add(calender.StartDate.Value.Date);
        //                            }
        //                        }
        //                    }

        //                    if (StaffScheduleData.CourseSection.ScheduleType == "Block Schedule (4)")
        //                    {
        //                        foreach (var allCourseSectionVew in allCourseSectionViewData)
        //                        {
        //                            var bellScheduleList = this.context?.BellSchedule.Where(v => v.SchoolId == StaffScheduleData.SchoolId && v.TenantId == StaffScheduleData.TenantId && v.BlockId == allCourseSectionVew.BlockId && v.BellScheduleDate >= StaffScheduleData.DurationStartDate && v.BellScheduleDate <= StaffScheduleData.DurationEndDate && v.BellScheduleDate <= DateTime.Today.Date && (!holidayList.Contains(v.BellScheduleDate))).ToList();

        //                            if (bellScheduleList != null && bellScheduleList.Any())
        //                            {
        //                                foreach (var bellSchedule in bellScheduleList)
        //                                {
        //                                    var StudentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(b => b.SchoolId == StaffScheduleData.SchoolId && b.TenantId == StaffScheduleData.TenantId && b.EffectiveStartDate.Value.Date <= bellSchedule.BellScheduleDate.Date && b.CourseSectionId == StaffScheduleData.CourseSectionId).ToList(); //check student's EffectiveStartDate in this course section

        //                                    if (StudentCoursesectionScheduleData != null && StudentCoursesectionScheduleData.Any())
        //                                    {
        //                                        var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == StaffScheduleData.SchoolId && b.TenantId == StaffScheduleData.TenantId && b.AttendanceDate.Date == bellSchedule.BellScheduleDate && b.CourseSectionId == StaffScheduleData.CourseSectionId && b.CourseId == StaffScheduleData.CourseId && b.PeriodId == allCourseSectionVew.BlockPeriodId);

        //                                        if (staffAttendanceData != null && !staffAttendanceData.Any())
        //                                        {
        //                                            count++;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }

        //                    if ((StaffScheduleData.CourseSection.ScheduleType == "Fixed Schedule (1)" && StaffScheduleData.CourseSection.AttendanceTaken == true) || StaffScheduleData.CourseSection.ScheduleType == "Variable Schedule (2)")
        //                    {
        //                        List<DateTime> dateList = new List<DateTime>();
        //                        List<string> list = new List<string>();
        //                        string[] meetingDays = Array.Empty<string>();

        //                        DateTime start = (DateTime)StaffScheduleData.CourseSection.DurationStartDate!;
        //                        DateTime end = (DateTime)StaffScheduleData.CourseSection.DurationEndDate!;

        //                        meetingDays = (StaffScheduleData.MeetingDays ?? "").ToLower().Split("|");

        //                        bool allDays = meetingDays == null || !meetingDays.Any();

        //                        dateList = Enumerable.Range(0, 1 + end.Subtract(start).Days)
        //                                              .Select(offset => start.AddDays(offset))
        //                                              .Where(d => allDays || meetingDays!.Contains(d.DayOfWeek.ToString().ToLower()))
        //                                              .ToList();

        //                        if (dateList.Any())
        //                        {
        //                            dateList = dateList.Where(s => dateList.Any(secL => s.Date <= DateTime.Today.Date)).ToList();
        //                            //Remove Holiday
        //                            dateList = dateList.Where(x => !holidayList.Contains(x.Date)).ToList();
        //                        }

        //                        foreach (var date in dateList)
        //                        {
        //                            var StudentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(b => b.SchoolId == StaffScheduleData.SchoolId && b.TenantId == StaffScheduleData.TenantId && b.EffectiveStartDate.Value.Date <= date && b.CourseSectionId == StaffScheduleData.CourseSectionId).ToList();  //check student's EffectiveStartDate in this course section 

        //                            if (StudentCoursesectionScheduleData?.Any() == true)
        //                            {
        //                                if (StaffScheduleData.CourseSection.ScheduleType == "Fixed Schedule (1)")
        //                                {
        //                                    var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == StaffScheduleData.SchoolId && b.TenantId == StaffScheduleData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseSectionId == StaffScheduleData.CourseSectionId && b.PeriodId == allCourseSectionViewData.FirstOrDefault()!.FixedPeriodId);

        //                                    if (staffAttendanceData?.Any() != true)
        //                                    {
        //                                        count++;
        //                                    }
        //                                }

        //                                if (StaffScheduleData.CourseSection.ScheduleType == "Variable Schedule (2)")
        //                                {
        //                                    var courseVariableScheduleData = allCourseSectionViewData.Where(e => e.VarDay.ToLower().Contains(date.DayOfWeek.ToString().ToLower()));

        //                                    if (courseVariableScheduleData != null)
        //                                    {
        //                                        foreach (var courseVariableSchedule in courseVariableScheduleData.ToList())
        //                                        {
        //                                            var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == StaffScheduleData.SchoolId && b.TenantId == StaffScheduleData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseSectionId == StaffScheduleData.CourseSectionId && b.PeriodId == courseVariableSchedule.VarPeriodId);

        //                                            if (staffAttendanceData?.Any() != true)
        //                                            {
        //                                                count++;
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        var calendarDataList = allCourseSectionViewData.Where(c => c.CalDate <= DateTime.Today.Date && !holidayList.Contains(c.CalDate.Value.Date));

        //                        if (calendarDataList.Any())
        //                        {
        //                            foreach (var calenderScheduleData in calendarDataList)
        //                            {
        //                                var StudentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(b => b.SchoolId == StaffScheduleData.SchoolId && b.TenantId == StaffScheduleData.TenantId && b.EffectiveStartDate.Value.Date <= calenderScheduleData.CalDate && b.CourseSectionId == StaffScheduleData.CourseSectionId).ToList(); //check student's EffectiveStartDate in this course section 

        //                                if (StudentCoursesectionScheduleData?.Any() == true)
        //                                {
        //                                    var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == StaffScheduleData.SchoolId && b.TenantId == StaffScheduleData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == calenderScheduleData.CalDate && b.CourseSectionId == StaffScheduleData.CourseSectionId && b.PeriodId == calenderScheduleData.CalPeriodId);

        //                                    if (staffAttendanceData?.Any() != true)
        //                                    {
        //                                        count++;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            scheduledCourseSectionView.MissingAttendanceCount = count;

        //            if (scheduledCourseSectionViewModel.MembershipId != null)
        //            {
        //                var noticeList = this.context?.Notice.Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.Isactive == true && x.TargetMembershipIds.Contains((scheduledCourseSectionViewModel.MembershipId ?? 0).ToString()) && (x.ValidFrom <= todayDate && todayDate <= x.ValidTo)).OrderByDescending(x => x.ValidFrom).ToList();

        //                if (noticeList?.Any() == true)
        //                {
        //                    scheduledCourseSectionView.NoticeList = noticeList;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception es)
        //    {
        //        scheduledCourseSectionView._failure = true;
        //        scheduledCourseSectionView._message = es.Message;
        //    }
        //    return scheduledCourseSectionView;
        //}

        /// <summary>
        /// Get Dashboard View For Calendar View
        /// </summary>
        /// <param name="dashboardViewModel"></param>
        /// <returns></returns>
        public DashboardViewModel GetDashboardViewForCalendarView(DashboardViewModel dashboardViewModel)
        {
            DashboardViewModel dashboardView = new DashboardViewModel();

            try
            {
                List<CalendarEvents>? Events = new();

                var CalendarData = this.context?.SchoolCalendars.Where(x => x.TenantId == dashboardViewModel.TenantId && x.SchoolId == dashboardViewModel.SchoolId && x.AcademicYear == dashboardViewModel.AcademicYear).ToList();
                if (CalendarData?.Any() == true)
                {
                    dashboardView.SchoolCalendar = CalendarData.FirstOrDefault(x => x.DefaultCalender == true);
                    if (dashboardView.SchoolCalendar != null)
                    {
                        dashboardView.SchoolCalendar!.SchoolMaster = null!;
                    }

                    Events = this.context?.CalendarEvents.Where(x => x.TenantId == dashboardViewModel.TenantId && x.AcademicYear == dashboardViewModel.AcademicYear && (x.VisibleToMembershipId ?? "").Contains((dashboardViewModel.MembershipId ?? 0).ToString()) && ((x.StartDate <= DateTime.Today.Date && DateTime.Today.Date <= x.EndDate) || (x.StartDate >= DateTime.Today.Date)) && (x.SchoolId == dashboardViewModel.SchoolId || x.SystemWideEvent == true)).ToList();

                    //for fetch holiday
                    var holidays = this.context?.CalendarEvents.Where(x => x.TenantId == dashboardViewModel.TenantId && x.AcademicYear == dashboardViewModel.AcademicYear && ((x.StartDate <= DateTime.Today.Date && DateTime.Today.Date <= x.EndDate) || (x.StartDate >= DateTime.Today.Date)) && x.IsHoliday == true && (x.SchoolId == dashboardViewModel.SchoolId || x.ApplicableToAllSchool == true)).ToList();

                    if (holidays?.Any() == true)
                    {
                        Events?.AddRange(holidays);
                    }

                    if (Events != null && Events.Any())
                    {
                        var EventList = Events.OrderBy(x => x.StartDate).ToList();
                        foreach (var events in EventList)
                        {
                            var calenderEvent = new CalenderEventViewModel
                            {
                                TenantId = events.TenantId,
                                SchoolId = events.SchoolId,
                                CalendarId = events.CalendarId,
                                EventId = events.EventId,
                                AcademicYear = events.AcademicYear,
                                SchoolDate = events.SchoolDate,
                                Title = events.Title,
                                Description = events.Description,
                                VisibleToMembershipId = events.VisibleToMembershipId,
                                UpdatedBy = events.UpdatedBy,
                                UpdatedOn = events.UpdatedOn,
                                StartDate = events.StartDate,
                                EndDate = events.EndDate,
                                EventColor = events.EventColor,
                                SystemWideEvent = events.SystemWideEvent,
                                ApplicableToAllSchool = events.ApplicableToAllSchool,
                                IsHoliday = events.IsHoliday,
                                CreatedBy = events.CreatedBy,
                                CreatedOn = events.CreatedOn,
                                CalenderName = CalendarData.FirstOrDefault(x => x.TenantId == events.TenantId && x.SchoolId == events.SchoolId && x.CalenderId == events.CalendarId)?.Title
                            };
                            dashboardView.CalendarEventList.Add(calenderEvent);
                        }
                    }
                }
                dashboardView.TenantId = dashboardViewModel.TenantId;
                dashboardView.SchoolId = dashboardViewModel.SchoolId;
                dashboardView.AcademicYear = dashboardViewModel.AcademicYear;
                dashboardView._tenantName = dashboardViewModel._tenantName;
                dashboardView._userName = dashboardViewModel._userName;
                dashboardView._token = dashboardViewModel._token;
                dashboardView._failure = false;
            }
            catch (Exception es)
            {
                dashboardView._failure = true;
                dashboardView._message = es.Message;
            }
            return dashboardView;
        }

        /// <summary>
        /// Reset Password For User
        /// </summary>
        /// <param name="resetPasswordModel"></param>
        /// <returns></returns>
        public ResetPasswordModel ResetPasswordForUser(ResetPasswordModel resetPasswordModel)
        {
            ResetPasswordModel resetPassword = new ();
            try
            {
                var userMasterData = this.context?.UserMaster.Where(x => x.TenantId == resetPasswordModel.UserMaster!.TenantId && x.SchoolId == resetPasswordModel.UserMaster.SchoolId && x.UserId == resetPasswordModel.UserMaster.UserId && x.EmailAddress == resetPasswordModel.UserMaster.EmailAddress).FirstOrDefault();
                if (userMasterData != null)
                {
                    var decrypted = Utility.Decrypt(resetPasswordModel.UserMaster!.PasswordHash);
                    string passwordHash = Utility.GetHashedPassword(decrypted);

                    userMasterData.PasswordHash = passwordHash;
                    userMasterData.UpdatedOn = DateTime.UtcNow;

                    this.context?.SaveChanges();
                    resetPassword._failure = false;
                    resetPassword._message = "Password Updated Successfully";
                }
                else
                {
                    resetPassword.UserMaster = null;
                    resetPassword._failure = true;
                    resetPassword._message = "Invalid Email Address";
                }
            }
            catch (Exception es)
            {
                resetPassword._failure = true;
                resetPassword._message = es.Message;
            }
            return resetPassword;
        }

        /// <summary>
        /// Get All Field List
        /// </summary>
        /// <param name="excelHeaderModel"></param>
        /// <returns></returns>      
        public ModuleFieldListModel GetAllFieldList(ModuleFieldListModel excelHeaderModel)
        {
            ModuleFieldListModel excelHeader = new ModuleFieldListModel();
            try
            {
                List<int> categoryid = new List<int>();
                List<string> fieldName = new();
                if (excelHeaderModel.Module != null) { 
                if (excelHeaderModel.Module.ToLower() == "student")
                {
                    categoryid = new List<int> { 6, 7, 8, 9 };

                    fieldName = new List<string>
                    {
                        "socialSecurityNumber",
                        "otherGovtIssuedNumber",
                        "portalAccess",
                        "studentPhoto",
                        "calenderId",
                        "rollingOption",
                        "eligibility504",
                        "economicDisadvantage",
                        "freeLunchEligibility" ,
                        "specialEducationIndicator",
                        "lepIndicator",
                        "schoolId",
                        "enrollmentCode",
                        "exitDate",
                        "exitCode",
                        "busNo",
                        "schoolBusPickUp",
                        "schoolBusDropOff",
                        "mailingAddressSameToHome",
                        "mailingAddressLineOne",
                        "mailingAddressLineTwo",
                        "mailingAddressCountry",
                        "mailingAddressState",
                        "mailingAddressCity",
                        "mailingAddressZip",
                        "twitter",
                        "facebook",
                        "instagram",
                        "youtube",
                        "linkedin"
                    };
                }
                if (excelHeaderModel.Module.ToLower() == "staff")
                {
                    categoryid = new List<int> { 15 };

                    fieldName = new List<string>
                    {
                        "portalAccess",
                        "staffPhoto",
                        "schoolAttachedName",
                        "endDate",
                        "twitter",
                        "facebook",
                        "instagram",
                        "youtube",
                        "linkedin",
                        "mailingAddressSameToHome",
                        "mailingAddressLineOne",
                        "mailingAddressLineTwo",
                        "mailingAddressCountry",
                        "mailingAddressState",
                        "mailingAddressCity",
                        "mailingAddressZip",
                        "busNo",
                        "busPickup",
                        "busDropoff"
                    };
                }

                var customFieldTitleList = this.context?.CustomFields.Where(c => c.SchoolId == excelHeaderModel.SchoolId && c.TenantId == excelHeaderModel.TenantId && ((categoryid.Count == 0 || (!categoryid.Contains(c.CategoryId))) && ((fieldName.Count == 0 || (!fieldName.Contains(c.FieldName!)))) && c.Module.ToLower() == excelHeaderModel.Module.ToLower())).OrderBy(x => x.CategoryId).ThenBy(b => b.SortOrder).ToArray();

                if (customFieldTitleList !=null && customFieldTitleList.Any())
                {
                    excelHeader.CustomfieldTitle = customFieldTitleList;
                    excelHeader.SchoolId = excelHeaderModel.SchoolId;
                    excelHeader.TenantId = excelHeaderModel.TenantId;
                    excelHeader.Module = excelHeaderModel.Module;
                    excelHeader._tenantName = excelHeaderModel._tenantName;
                    excelHeader._token = excelHeaderModel._token;
                    excelHeader._userName = excelHeaderModel._userName;
                    excelHeader._failure = false;
                }
            }
            }
            catch (Exception es)
            {
                excelHeader._failure = true;
                excelHeader._message = es.Message;
            }
            return excelHeader;
        }

        /// <summary>
        /// Add/Update School Preference
        /// </summary>
        /// <param name="schoolPreferenceAddViewModel"></param>
        /// <returns></returns>
        public SchoolPreferenceAddViewModel AddUpdateSchoolPreference(SchoolPreferenceAddViewModel schoolPreferenceAddViewModel)

        {
            try
            {
                if (schoolPreferenceAddViewModel.SchoolPreference!.SchoolPreferenceId > 0)
                {
                    var schoolPreferenceUpdate = this.context?.SchoolPreference.FirstOrDefault(x => x.TenantId == schoolPreferenceAddViewModel.SchoolPreference.TenantId && x.SchoolId == schoolPreferenceAddViewModel.SchoolPreference.SchoolId && x.SchoolPreferenceId == schoolPreferenceAddViewModel.SchoolPreference.SchoolPreferenceId);

                    if (schoolPreferenceUpdate != null)
                    {
                        schoolPreferenceAddViewModel.SchoolPreference.UpdatedOn = DateTime.UtcNow;
                        schoolPreferenceAddViewModel.SchoolPreference.CreatedOn = schoolPreferenceUpdate.CreatedOn;
                        schoolPreferenceAddViewModel.SchoolPreference.CreatedBy = schoolPreferenceUpdate.CreatedBy;
                        this.context?.Entry(schoolPreferenceUpdate).CurrentValues.SetValues(schoolPreferenceAddViewModel.SchoolPreference);
                        this.context?.SaveChanges();
                        schoolPreferenceAddViewModel._failure = false;
                        schoolPreferenceAddViewModel._message = "School Preference Updated Successfully";
                    }
                    else
                    {
                        schoolPreferenceAddViewModel._failure = true;
                        schoolPreferenceAddViewModel._message = NORECORDFOUND;
                    }
                }
                else
                {
                    long? schoolPreferenceId = Utility.GetMaxLongPK(this.context, new Func<SchoolPreference, long>(x => x.SchoolPreferenceId));
                    schoolPreferenceAddViewModel.SchoolPreference.SchoolPreferenceId = (long)schoolPreferenceId!;
                    schoolPreferenceAddViewModel.SchoolPreference.CreatedOn = DateTime.UtcNow;
                    this.context?.SchoolPreference.Add(schoolPreferenceAddViewModel.SchoolPreference);
                    //context!.Entry(schoolPreferenceAddViewModel.SchoolPreference.SchoolMaster).State = EntityState.Unchanged;
                    this.context?.SaveChanges();
                    schoolPreferenceAddViewModel._failure = false;
                    schoolPreferenceAddViewModel._message = "School Preference added successfully";
                }
            }
            catch (Exception es)
            {
                schoolPreferenceAddViewModel._failure = true;
                schoolPreferenceAddViewModel._message = es.Message;
            }
            return schoolPreferenceAddViewModel;
        }

        /// <summary>
        /// Get School Preference
        /// </summary>
        /// <param name="schoolPreferenceAddViewModel"></param>
        /// <returns></returns>
        public SchoolPreferenceAddViewModel ViewSchoolPreference(SchoolPreferenceAddViewModel schoolPreferenceAddViewModel)
        {
            SchoolPreferenceAddViewModel schoolPreferenceViewModel = new SchoolPreferenceAddViewModel();
            try
            {
               

                var schoolPreferenceView = this.context?.SchoolPreference.FirstOrDefault(x => x.TenantId == schoolPreferenceAddViewModel.SchoolPreference!.TenantId && x.SchoolId == schoolPreferenceAddViewModel.SchoolPreference.SchoolId);

                if (schoolPreferenceView != null)
                {
                    schoolPreferenceViewModel.SchoolPreference = schoolPreferenceView;
                    schoolPreferenceViewModel._tenantName = schoolPreferenceAddViewModel._tenantName;
                    schoolPreferenceViewModel._token = schoolPreferenceAddViewModel._token;
                    schoolPreferenceViewModel._userName = schoolPreferenceAddViewModel._userName;
                    schoolPreferenceViewModel._failure = false;
                }
                else
                {
                    schoolPreferenceViewModel._failure = true;
                    schoolPreferenceViewModel._message = NORECORDFOUND;
                    schoolPreferenceViewModel._tenantName = schoolPreferenceAddViewModel._tenantName;
                    schoolPreferenceViewModel._token = schoolPreferenceAddViewModel._token;
                    schoolPreferenceViewModel._userName = schoolPreferenceAddViewModel._userName;
                }
            }
            catch (Exception es)
            {
                schoolPreferenceViewModel._failure = true;
                schoolPreferenceViewModel._message = es.Message;
                schoolPreferenceViewModel._tenantName = schoolPreferenceAddViewModel._tenantName;
                schoolPreferenceViewModel._token = schoolPreferenceAddViewModel._token;
                schoolPreferenceViewModel._userName = schoolPreferenceAddViewModel._userName;
            }
            return schoolPreferenceViewModel;
        }

        /// <summary>
        /// Get Missing Attendance Count For Dashboard View
        /// </summary>
        /// <param name="scheduledCourseSectionViewModel"></param>
        /// <returns></returns>
        public ScheduledCourseSectionViewModel GetMissingAttendanceCountForDashboardView_old(ScheduledCourseSectionViewModel scheduledCourseSectionViewModel)
        {
            List<AllCourseSectionView>? AllCourseSectionViewList = new ();
            ScheduledCourseSectionViewModel scheduledCourseSectionView = new ();
            List<DateTime> holidayList = new List<DateTime>();
            try
            {
                var scheduledCourseSectionData = this.context?.StaffCoursesectionSchedule.Include(x => x.CourseSection).Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.IsDropped != true ).ToList();

                if (scheduledCourseSectionData!=null && scheduledCourseSectionData.Any())
                {
                    int count = 0;
                    List<int> ID = new List<int>();

                    AllCourseSectionViewList = this.context?.AllCourseSectionView.Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.AcademicYear == scheduledCourseSectionViewModel.AcademicYear).ToList();
                    
                    foreach (var scheduledCourseSection in scheduledCourseSectionData)
                    {
                        if (!ID.Contains(scheduledCourseSection.CourseSectionId))
                        {
                            var AllCourseSectionViewData = AllCourseSectionViewList!.Where(v => v.SchoolId == scheduledCourseSectionViewModel.SchoolId && v.TenantId == scheduledCourseSectionViewModel.TenantId && v.CourseId == scheduledCourseSection.CourseId && v.CourseSectionId == scheduledCourseSection.CourseSectionId && (v.AttendanceTaken == true || v.TakeAttendanceCalendar == true || v.TakeAttendanceVariable == true || v.TakeAttendanceBlock == true)).ToList();

                            if (AllCourseSectionViewData.Any())
                            { 
                                //Calculate Holiday
                                var CalendarEventsData = this.context?.CalendarEvents.Where(e => e.TenantId == scheduledCourseSectionViewModel.TenantId && e.CalendarId == AllCourseSectionViewData.FirstOrDefault()!.CalendarId && (e.StartDate >= scheduledCourseSection.DurationStartDate && e.StartDate <= scheduledCourseSection.DurationEndDate || e.EndDate >= scheduledCourseSection.DurationStartDate && e.EndDate <= scheduledCourseSection.DurationEndDate) && e.IsHoliday == true && (e.SchoolId == scheduledCourseSectionViewModel.SchoolId || e.ApplicableToAllSchool == true)).ToList();

                                if (CalendarEventsData?.Any()==true)
                                {
                                    foreach (var calender in CalendarEventsData)
                                    {
                                        if (calender.EndDate!.Value.Date > calender.StartDate!.Value.Date)
                                        {
                                            var date = Enumerable.Range(0, 1 + (calender.EndDate.Value.Date - calender.StartDate.Value.Date).Days)
                                               .Select(i => calender.StartDate.Value.Date.AddDays(i))
                                               .ToList();
                                            holidayList.AddRange(date);
                                        }
                                        holidayList.Add(calender.StartDate.Value.Date);
                                    }
                                }

                                if (scheduledCourseSection.CourseSection.ScheduleType == "Block Schedule (4)")
                                {
                                    foreach (var allCourseSectionVew in AllCourseSectionViewData)
                                    {
                                        var bellScheduleList = this.context?.BellSchedule.Where(v => v.SchoolId == scheduledCourseSection.SchoolId && v.TenantId == scheduledCourseSection.TenantId && v.BlockId == allCourseSectionVew.BlockId && v.BellScheduleDate >= scheduledCourseSection.DurationStartDate && v.BellScheduleDate <= scheduledCourseSection.DurationEndDate && v.BellScheduleDate <= DateTime.Today.Date&&(!holidayList.Contains(v.BellScheduleDate))).ToList();

                                        if (bellScheduleList?.Any()==true)
                                        {
                                            foreach (var bellSchedule in bellScheduleList)
                                            {
                                                var StudentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(b => b.SchoolId == scheduledCourseSection.SchoolId && b.TenantId == scheduledCourseSection.TenantId && b.EffectiveStartDate.Value.Date <= bellSchedule.BellScheduleDate.Date && b.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList(); //check student's EffectiveStartDate in this course section

                                                if (StudentCoursesectionScheduleData?.Any()==true)
                                                {
                                                    var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == scheduledCourseSection.SchoolId && b.TenantId == scheduledCourseSection.TenantId && b.AttendanceDate.Date == bellSchedule.BellScheduleDate && b.CourseSectionId == scheduledCourseSection.CourseSectionId && b.CourseId == scheduledCourseSection.CourseId && b.PeriodId == allCourseSectionVew.BlockPeriodId);

                                                    if (staffAttendanceData?.Any()!=true)
                                                    {
                                                        count++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (scheduledCourseSection.CourseSection.ScheduleType == "Calendar Schedule (3)")
                                {
                                    var calenderScheduleList = AllCourseSectionViewData.Where(c => c.CalDate.Value.Date <= DateTime.Today.Date && (!holidayList.Contains(c.CalDate.Value.Date)));

                                    if (calenderScheduleList.Any())
                                    {
                                        foreach (var courseCalenderSchedule in calenderScheduleList)
                                        {
                                            var StudentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(b => b.SchoolId == scheduledCourseSection.SchoolId && b.TenantId == scheduledCourseSection.TenantId && b.EffectiveStartDate.Value.Date <= courseCalenderSchedule.CalDate!.Value.Date && b.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList(); //check student's EffectiveStartDate in this course section

                                            if (StudentCoursesectionScheduleData?.Any()==true)
                                            {
                                                var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == scheduledCourseSection.SchoolId && b.TenantId == scheduledCourseSection.TenantId /*&& b.StaffId == staffScheduleData.StaffId*/ && b.AttendanceDate.Date == courseCalenderSchedule.CalDate!.Value.Date && b.CourseSectionId == scheduledCourseSection.CourseSectionId && b.CourseId == scheduledCourseSection.CourseId && b.PeriodId == courseCalenderSchedule.CalPeriodId);

                                                if (staffAttendanceData?.Any()!=true)
                                                {
                                                    count++;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    DateTime start;
                                    DateTime end;
                                    List<DateTime> dateList = new ();
                                    string[] meetingDays = Array.Empty<string>();

                                    start = (DateTime)scheduledCourseSection.DurationStartDate!;
                                    end = (DateTime)scheduledCourseSection.DurationEndDate!;


                                    meetingDays = (scheduledCourseSection.MeetingDays??"").ToLower().Split("|");

                                    bool allDays = meetingDays == null || !meetingDays.Any();

                                    dateList = Enumerable.Range(0, 1 + end.Subtract(start).Days)
                                                          .Select(offset => start.AddDays(offset))
                                                          .Where(d => allDays || meetingDays!.Contains(d.DayOfWeek.ToString().ToLower()))
                                                          .ToList();

                                    if (dateList.Any())
                                    {
                                        dateList = dateList.Where(s => dateList.Any(secL => s.Date <= DateTime.Today.Date)).ToList();
                                        dateList = dateList.Where(x => !holidayList.Contains(x.Date)).ToList();
                                    }

                                    foreach (var date in dateList)
                                    {
                                        var StudentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(b => b.SchoolId == scheduledCourseSection.SchoolId && b.TenantId == scheduledCourseSection.TenantId && b.EffectiveStartDate.Value.Date <= date && b.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();  //check student's EffectiveStartDate in this course section 

                                        if (StudentCoursesectionScheduleData?.Any()==true)
                                        {
                                            if (scheduledCourseSection.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                            {
                                                var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == scheduledCourseSection.SchoolId && b.TenantId == scheduledCourseSection.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseId == scheduledCourseSection.CourseId && b.CourseSectionId == scheduledCourseSection.CourseSectionId && b.PeriodId == AllCourseSectionViewData.FirstOrDefault()!.FixedPeriodId);

                                                if (!staffAttendanceData?.Any()==true)
                                                {
                                                    count++;
                                                }
                                            }

                                            if (scheduledCourseSection.CourseSection.ScheduleType == "Variable Schedule (2)")
                                            {
                                                var courseVariableScheduleData = AllCourseSectionViewData.Where(e => e.VarDay.ToLower().Contains(date.DayOfWeek.ToString().ToLower()));

                                                if (courseVariableScheduleData != null)
                                                {
                                                    foreach (var courseVariableSchedule in courseVariableScheduleData.ToList())
                                                    {
                                                        var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == scheduledCourseSection.SchoolId && b.TenantId == scheduledCourseSection.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseId == scheduledCourseSection.CourseId && b.CourseSectionId == scheduledCourseSection.CourseSectionId && b.PeriodId == courseVariableSchedule.VarPeriodId);

                                                        if (staffAttendanceData?.Any()!=true)
                                                        {
                                                            count++;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            ID.Add(scheduledCourseSection.CourseSectionId);
                        }                        
                    }
                    scheduledCourseSectionView.MissingAttendanceCount = count;
                    scheduledCourseSectionView._failure = false;
                    scheduledCourseSectionView.SchoolId = scheduledCourseSectionViewModel.SchoolId;
                    scheduledCourseSectionView.TenantId = scheduledCourseSectionViewModel.TenantId;
                    scheduledCourseSectionView._tenantName = scheduledCourseSectionViewModel._tenantName;
                    scheduledCourseSectionView._token = scheduledCourseSectionViewModel._token;
                }
            }
            catch (Exception es)
            {
                scheduledCourseSectionView._failure = true;
                scheduledCourseSectionView._message = es.Message;
            }
            return scheduledCourseSectionView;
        }

        public ScheduledCourseSectionViewModel GetMissingAttendanceCountForDashboardView(ScheduledCourseSectionViewModel scheduledCourseSectionViewModel)
        {
            List<AllCourseSectionView>? AllCourseSectionViewList = new();
            ScheduledCourseSectionViewModel scheduledCourseSectionView = new();
            List<DateTime> holidayList = new List<DateTime>();
            try
            {
                var scheduledCourseSectionData = this.context?.StaffCoursesectionSchedule.Include(x => x.CourseSection).Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.IsDropped != true).ToList();

                if (scheduledCourseSectionData != null && scheduledCourseSectionData.Any())
                {
                    int count = 0;
                    List<int> ID = new List<int>();

                    AllCourseSectionViewList = this.context?.AllCourseSectionView.Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.AcademicYear == scheduledCourseSectionViewModel.AcademicYear).ToList();

                    foreach (var scheduledCourseSection in scheduledCourseSectionData)
                    {
                        if (!ID.Contains(scheduledCourseSection.CourseSectionId))
                        {
                            var AllCourseSectionViewData = AllCourseSectionViewList!.Where(v => v.SchoolId == scheduledCourseSectionViewModel.SchoolId && v.TenantId == scheduledCourseSectionViewModel.TenantId && v.CourseId == scheduledCourseSection.CourseId && v.CourseSectionId == scheduledCourseSection.CourseSectionId && (v.AttendanceTaken == true || v.TakeAttendanceCalendar == true || v.TakeAttendanceVariable == true || v.TakeAttendanceBlock == true)).ToList();

                            if (AllCourseSectionViewData.Any())
                            {
                                var studentMissingAttendanceData = this.context?.StudentMissingAttendances.Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.CourseSectionId == scheduledCourseSection.CourseSectionId && x.MissingAttendanceDate >= scheduledCourseSection.DurationStartDate && x.MissingAttendanceDate <= scheduledCourseSection.DurationEndDate).ToList();

                                if (studentMissingAttendanceData != null && studentMissingAttendanceData.Any() == true)
                                {
                                    count += studentMissingAttendanceData.Count;
                                }

                                ID.Add(scheduledCourseSection.CourseSectionId);
                            }
                        }
                    }
                    scheduledCourseSectionView.MissingAttendanceCount = count;
                    scheduledCourseSectionView._failure = false;
                    scheduledCourseSectionView.SchoolId = scheduledCourseSectionViewModel.SchoolId;
                    scheduledCourseSectionView.TenantId = scheduledCourseSectionViewModel.TenantId;
                    scheduledCourseSectionView._tenantName = scheduledCourseSectionViewModel._tenantName;
                    scheduledCourseSectionView._token = scheduledCourseSectionViewModel._token;
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
        /// Change Password For User
        /// </summary>
        /// <param name="changePasswordViewModel"></param>
        /// <returns></returns>
        public ChangePasswordViewModel ChangePasswordForUser(ChangePasswordViewModel changePasswordViewModel)
        {
            try
            {
                var oldPasswordDecrypted = Utility.Decrypt(changePasswordViewModel.CurrentPasswordHash!);
                string oldPasswordHash = Utility.GetHashedPassword(oldPasswordDecrypted);

                var userMasterData = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == changePasswordViewModel.TenantId && x.UserId == changePasswordViewModel.UserId && x.EmailAddress == changePasswordViewModel.EmailAddress && x.PasswordHash == oldPasswordHash);

                if (userMasterData != null)
                {
                    var newPasswordDecrypted = Utility.Decrypt(changePasswordViewModel.NewPasswordHash!);
                    string NewPasswordHash = Utility.GetHashedPassword(newPasswordDecrypted);

                    userMasterData.PasswordHash = NewPasswordHash;
                    userMasterData.UpdatedOn = DateTime.UtcNow;

                    this.context?.SaveChanges();
                    changePasswordViewModel._failure = false;
                    changePasswordViewModel._message = "Password Updated Successfully";
                }
                else
                {
                    changePasswordViewModel._failure = true;
                    changePasswordViewModel._message = "Invalid Email Address or Current Password";
                }
            }
            catch (Exception es)
            {
                changePasswordViewModel._failure = true;
                changePasswordViewModel._message = es.Message;
            }
            return changePasswordViewModel;
        }

        /// <summary>
        /// Active Deactive User
        /// </summary>
        /// <param name="activeDeactiveUserViewModel"></param>
        /// <returns></returns>
        public ActiveDeactiveUserViewModel ActiveDeactiveUser(ActiveDeactiveUserViewModel activeDeactiveUserViewModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(activeDeactiveUserViewModel.Module))
                {

                    if (activeDeactiveUserViewModel.Module.ToLower() == "student")
                    {
                        var StudentData = this.context?.StudentMaster.Include(x => x.StudentEnrollment).FirstOrDefault(e => e.TenantId == activeDeactiveUserViewModel.TenantId && e.SchoolId == activeDeactiveUserViewModel.SchoolId && e.StudentId == activeDeactiveUserViewModel.UserId);
                        if (StudentData != null)
                        {
                            if (activeDeactiveUserViewModel.IsActive == true)
                            {
                                var lastEnrollment = StudentData.StudentEnrollment.OrderByDescending(x => x.EnrollmentId).FirstOrDefault();
                                if ((lastEnrollment!.ExitDate != null && lastEnrollment.ExitDate < DateTime.Today.Date) || (lastEnrollment.EnrollmentDate > DateTime.Today.Date))
                                {
                                    activeDeactiveUserViewModel._failure = true;
                                    activeDeactiveUserViewModel._message = "Student dropped out from the school, so can not upadte the status";
                                    return activeDeactiveUserViewModel;
                                }
                                else
                                {
                                    StudentData.IsActive = activeDeactiveUserViewModel.IsActive;
                                }
                            }
                            else
                            {
                                StudentData.IsActive = activeDeactiveUserViewModel.IsActive;
                            }
                        }
                    }
                    else if (activeDeactiveUserViewModel.Module.ToLower() == "parent")
                    {
                        var ParentData = this.context?.ParentInfo.FirstOrDefault(e => e.TenantId == activeDeactiveUserViewModel.TenantId && e.SchoolId == activeDeactiveUserViewModel.SchoolId && e.ParentId == activeDeactiveUserViewModel.UserId);

                        if (ParentData != null)
                        {
                            ParentData.IsActive = activeDeactiveUserViewModel.IsActive;
                        }
                    }
                    else
                    {
                        var StaffData = this.context?.StaffMaster.Include(x => x.StaffSchoolInfo).FirstOrDefault(e => e.TenantId == activeDeactiveUserViewModel.TenantId && e.StaffId == activeDeactiveUserViewModel.UserId);

                        if (StaffData != null)
                        {
                            if (activeDeactiveUserViewModel.IsActive == true)
                            {

                                var staffSchoolInfo = StaffData.StaffSchoolInfo.FirstOrDefault(x => x.SchoolId == activeDeactiveUserViewModel.SchoolId);
                                if (staffSchoolInfo!.EndDate != null && staffSchoolInfo.EndDate < DateTime.Today.Date)
                                {
                                    activeDeactiveUserViewModel._failure = true;
                                    activeDeactiveUserViewModel._message = "Staff dropped out from the school, so can not upadte the status";
                                    return activeDeactiveUserViewModel;
                                }
                                else
                                {
                                    StaffData.IsActive = activeDeactiveUserViewModel.IsActive;
                                }
                            }
                            else
                            {
                                StaffData.IsActive = activeDeactiveUserViewModel.IsActive;
                            }
                        }
                    }

                    //update in user master table
                    if (activeDeactiveUserViewModel.IsActive == true && !string.IsNullOrEmpty(activeDeactiveUserViewModel.LoginEmail))
                    {
                        var userMasterData = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == activeDeactiveUserViewModel.TenantId && x.UserId == activeDeactiveUserViewModel.UserId && x.EmailAddress == activeDeactiveUserViewModel.LoginEmail);
                        if (userMasterData != null)
                        {
                            userMasterData.LoginFailureCount = 0;
                            userMasterData.LoginAttemptDate = DateTime.UtcNow;
                        }
                    }

                    this.context?.SaveChanges();
                    activeDeactiveUserViewModel._failure = false;
                    activeDeactiveUserViewModel._message = "updated successfully";
                }
                else
                {
                    activeDeactiveUserViewModel._failure = true;
                    activeDeactiveUserViewModel._message = "Please pass the module";
                }
            }
            catch (Exception es)
            {
                activeDeactiveUserViewModel._failure = true;
                activeDeactiveUserViewModel._message = es.Message;
            }
            return activeDeactiveUserViewModel;
        }

        /// <summary>
        /// Update Dropdown Value Sort Order
        /// </summary>
        /// <param name="dpValueSortOrderModel"></param>
        /// <returns></returns>
        public DropdownValueSortOrderModel UpdateDropdownValueSortOrder(DropdownValueSortOrderModel dpValueSortOrderModel)
        {
            try
            {
                var dpValueList = this.context?.DpdownValuelist.Where(a => a.TenantId == dpValueSortOrderModel.TenantId && a.SchoolId == dpValueSortOrderModel.SchoolId && Equals(a.LovName, dpValueSortOrderModel.LovName));
                if (dpValueList != null && dpValueList.Any())
                {
                    var sortOrderValues = dpValueSortOrderModel.SortOrderValues.OrderBy(c => c.Id);
                    foreach (var dpValue in sortOrderValues)
                    {

                        var dpDownValueData = dpValueList.FirstOrDefault(d => d.TenantId == dpValueSortOrderModel.TenantId && d.SchoolId == dpValueSortOrderModel.SchoolId && d.Id == dpValue.Id);

                        if (dpDownValueData != null)
                        {
                            var editDpDownData = dpDownValueData;
                            dpDownValueData.SortOrder = dpValue.SortOrder;
                            dpDownValueData.UpdatedOn = DateTime.UtcNow;
                            dpDownValueData.UpdatedBy = dpValueSortOrderModel.UpdatedBy;
                            this.context?.Entry(editDpDownData).CurrentValues.SetValues(dpDownValueData);
                        }

                    }
                    this.context?.SaveChanges();
                    dpValueSortOrderModel._failure = false;
                    dpValueSortOrderModel._message = dpValueSortOrderModel.LovName + " sort order updated successfully";
                }
                else
                {
                    dpValueSortOrderModel._failure = true;
                    dpValueSortOrderModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                dpValueSortOrderModel._message = es.Message;
                dpValueSortOrderModel._failure = true;
            }
            return dpValueSortOrderModel;
        }
    }
}

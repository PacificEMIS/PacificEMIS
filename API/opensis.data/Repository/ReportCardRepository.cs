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
using opensis.data.ViewModels.ReportCard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using opensis.data.ViewModels.CourseManager;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using JSReport;
using opensis.data.Helper;
using System.Runtime.InteropServices;

namespace opensis.data.Repository
{
    public class ReportCardRepository : IReportCardRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public ReportCardRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }
        /// <summary>
        /// Add Report Card Comments
        /// </summary>
        /// <param name="reportCardViewModel"></param>
        /// <returns></returns>

        //public ReportCardCommentsAddViewModel AddReportCardComments(ReportCardCommentsAddViewModel reportCardCommentsAddViewModel)
        //{
        //    using (var transaction = this.context.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            List<ReportCardComments> reportCardComments = new List<ReportCardComments>();

        //            int? id = 1;
        //            int? SortOrder = 1;

        //            if (reportCardCommentsAddViewModel.reportCardComments.Count > 0)
        //            {
        //                //var reportCardCommentData = this.context?.ReportCardComments.Where(x => x.TenantId == reportCardCommentsAddViewModel.TenantId && x.SchoolId == reportCardCommentsAddViewModel.SchoolId).OrderByDescending(x => x.Id).FirstOrDefault();

        //                //if (reportCardCommentData != null)
        //                //{
        //                //    id = Convert.ToInt32(reportCardCommentData.Id) + 1;
        //                //}

        //                //var reportCardCommentSortOrder = this.context?.ReportCardComments.Where(x => x.TenantId == reportCardCommentsAddViewModel.TenantId && x.SchoolId == reportCardCommentsAddViewModel.SchoolId && x.CourseCommentId == reportCardCommentsAddViewModel.CourseCommentId).OrderByDescending(x => x.Id).FirstOrDefault();

        //                //if (reportCardCommentSortOrder != null)
        //                //{
        //                //    SortOrder = reportCardCommentSortOrder.SortOrder + 1;
        //                //}

        //                //foreach (var reportComment in reportCardCommentsAddViewModel.reportCardComments.ToList())
        //                //{

        //                //    reportComment.TenantId = reportCardCommentsAddViewModel.TenantId;
        //                //    reportComment.SchoolId = reportCardCommentsAddViewModel.SchoolId;
        //                //    reportComment.CourseCommentId = reportCardCommentsAddViewModel.CourseCommentId;
        //                //    reportComment.CourseId = 1;
        //                //    reportComment.CourseSectionId = 1;
        //                //    reportComment.SortOrder = SortOrder;
        //                //    reportComment.Id = id;
        //                //    reportComment.CreatedOn = DateTime.UtcNow;
        //                //    reportCardComments.Add(reportComment);
        //                //    id++;
        //                //    SortOrder++;
        //                //}
        //                //reportCardCommentsAddViewModel._message = "Report Comments added succsesfully.";

        //                //this.context?.ReportCardComments.AddRange(reportCardComments);
        //                //this.context?.SaveChanges();
        //                //transaction.Commit();
        //                //reportCardCommentsAddViewModel._failure = false;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            reportCardCommentsAddViewModel._failure = true;
        //            reportCardCommentsAddViewModel._message = ex.Message;
        //        }
        //        return reportCardCommentsAddViewModel;
        //    }
        //}

        ///// <summary>
        ///// Update Report Card Comments
        ///// </summary>
        ///// <param name="reportCardViewModel"></param>
        ///// <returns></returns>

        //public ReportCardCommentsAddViewModel UpdateReportCardComments(ReportCardCommentsAddViewModel reportCardCommentsAddViewModel)
        //{
        //    try
        //    {
        //        List<ReportCardComments> reportCardComments = new List<ReportCardComments>();

        //        if (reportCardCommentsAddViewModel.reportCardComments.Count > 0)
        //        {
        //           /* var reportCommentDataExist = this.context?.ReportCardComments.Where(x => x.TenantId == reportCardCommentsAddViewModel.TenantId && x.SchoolId == reportCardCommentsAddViewModel.SchoolId && x.CourseCommentId == reportCardCommentsAddViewModel.CourseCommentId).ToList();

        //            if (reportCommentDataExist.Count > 0)
        //            {
        //                this.context?.ReportCardComments.RemoveRange(reportCommentDataExist);
        //                this.context?.SaveChanges();
        //            }

        //            int? id = 1;
        //            int? SortOrder = 1;

        //            var reportCardCommentData = this.context?.ReportCardComments.Where(x => x.TenantId == reportCardCommentsAddViewModel.TenantId && x.SchoolId == reportCardCommentsAddViewModel.SchoolId).OrderByDescending(x => x.Id).FirstOrDefault();

        //            if (reportCardCommentData != null)
        //            {
        //                id = Convert.ToInt32(reportCardCommentData.Id) + 1;
        //            }

        //            var reportCardCommentSortOrder = this.context?.ReportCardComments.Where(x => x.TenantId == reportCardCommentsAddViewModel.TenantId && x.SchoolId == reportCardCommentsAddViewModel.SchoolId && x.CourseCommentId == reportCardCommentsAddViewModel.CourseCommentId).OrderByDescending(x => x.SortOrder).FirstOrDefault();

        //            if (reportCardCommentSortOrder != null)
        //            {
        //                SortOrder = reportCardCommentSortOrder.SortOrder + 1;
        //            }

        //            foreach (var reportComment in reportCardCommentsAddViewModel.reportCardComments.ToList())
        //            {

        //                reportComment.TenantId = reportCardCommentsAddViewModel.TenantId;
        //                reportComment.SchoolId = reportCardCommentsAddViewModel.SchoolId;
        //                reportComment.CourseCommentId = reportCardCommentsAddViewModel.CourseCommentId;
        //                reportComment.CourseId = 1;
        //                reportComment.CourseSectionId = 1;

        //                reportComment.SortOrder = SortOrder;
        //                reportComment.Id = id;
        //                reportComment.UpdatedOn = DateTime.UtcNow;
        //                reportCardComments.Add(reportComment);
        //                id++;
        //                SortOrder++;
        //            }
        //            reportCardCommentsAddViewModel._message = "Report Comments updated successfully.";

        //            this.context?.ReportCardComments.AddRange(reportCardComments);
        //            this.context?.SaveChanges();
        //            //transaction.Commit();
        //            reportCardCommentsAddViewModel._failure = false;*/
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //transaction.Rollback();
        //        reportCardCommentsAddViewModel._failure = true;
        //        reportCardCommentsAddViewModel._message = ex.Message;
        //    }
        //    return reportCardCommentsAddViewModel;
        //}

        ///// <summary>
        ///// Delete Report Card Comments
        ///// </summary>
        ///// <param name="reportCardViewModel"></param>
        ///// <returns></returns>
        //public ReportCardCommentsAddViewModel DeleteReportCardComments(ReportCardCommentsAddViewModel reportCardCommentsAddViewModel)
        //{
        //    try
        //    {
        //       /* if (reportCardCommentsAddViewModel.reportCardComments.Count > 0)
        //        {
        //            var reportCardCommentDataExist = this.context?.ReportCardComments.FirstOrDefault(x => x.TenantId == reportCardCommentsAddViewModel.TenantId && x.SchoolId == reportCardCommentsAddViewModel.SchoolId && x.Id == reportCardCommentsAddViewModel.reportCardComments.FirstOrDefault().Id && x.CourseCommentId == reportCardCommentsAddViewModel.CourseCommentId);

        //            if (reportCardCommentDataExist != null)
        //            {
        //                this.context?.ReportCardComments.Remove(reportCardCommentDataExist);
        //                this.context?.SaveChanges();
        //                reportCardCommentsAddViewModel._failure = false;
        //                reportCardCommentsAddViewModel._message = "deleted successfullyy";
        //            }
        //            else
        //            {
        //                reportCardCommentsAddViewModel._failure = true;
        //                reportCardCommentsAddViewModel._message = NORECORDFOUND;
        //            }
        //        }*/
        //    }
        //    catch (Exception ex)
        //    {
        //        reportCardCommentsAddViewModel._failure = true;
        //        reportCardCommentsAddViewModel._message = ex.Message;
        //    }
        //    return reportCardCommentsAddViewModel;
        //}


        /// <summary>
        /// Add Course Comment Category
        /// </summary>
        /// <param name="courseCommentCategoryAddViewModel"></param>
        /// <returns></returns>
        //public CourseCommentCategoryAddViewModel AddCourseCommentCategory(CourseCommentCategoryAddViewModel courseCommentCategoryAddViewModel)
        //{
        //    try
        //    {
        //        List<CourseCommentCategory> courseCommentCategoryList = new List<CourseCommentCategory>();
        //        int i = 0;
        //        var distinctCourseData = courseCommentCategoryAddViewModel.courseCommentCategory.Select(s => new { s.CourseId, s.TenantId, s.SchoolId }).Distinct().ToList();

        //        int? courseCommentId = 1;

        //        foreach (var course in distinctCourseData.ToList())
        //        {
        //            var courseCommentCategoryDataExist = this.context?.CourseCommentCategory.Where(x => x.TenantId == course.TenantId && x.SchoolId == course.SchoolId && x.CourseId == course.CourseId).ToList();

        //            if (courseCommentCategoryDataExist != null && courseCommentCategoryDataExist.Any())
        //            {
        //                this.context?.CourseCommentCategory.RemoveRange(courseCommentCategoryDataExist);
        //                this.context?.SaveChanges();
        //            }

        //            var courseCommentCategoryData = courseCommentCategoryAddViewModel.courseCommentCategory.Where(x => x.CourseId == course.CourseId).ToList();

        //            int? sortOrder = 1;
        //            int? sortOrderForAllCourse = 1;

        //            if (i == 0)
        //            {
        //                var courseCommentCategoryDataForID = this.context?.CourseCommentCategory.Where(x => x.TenantId == course.TenantId && x.SchoolId == course.SchoolId).OrderByDescending(x => x.CourseCommentId).FirstOrDefault();

        //                if (courseCommentCategoryDataForID != null)
        //                {
        //                    courseCommentId = courseCommentCategoryDataForID.CourseCommentId + 1;
        //                }
        //            }

        //            foreach (var courseCommentCategory in courseCommentCategoryData)
        //            {
        //                courseCommentCategory.CourseCommentId = (int)courseCommentId;
        //                courseCommentCategory.SortOrder = courseCommentCategory.CourseId != null ? sortOrder : sortOrderForAllCourse;
        //                courseCommentCategory.CreatedOn = DateTime.UtcNow;
        //                courseCommentCategoryList.Add(courseCommentCategory);
        //                courseCommentId++;
        //                sortOrder++;
        //                sortOrderForAllCourse++;
        //            }
        //            i++;
        //        }

        //        this.context?.CourseCommentCategory.AddRange(courseCommentCategoryList);
        //        this.context?.SaveChanges();
        //        courseCommentCategoryAddViewModel._failure = false;
        //        courseCommentCategoryAddViewModel._message = "Course Comment Category added successfully";

        //    }
        //    catch (Exception es)
        //    {
        //        courseCommentCategoryAddViewModel._message = es.Message;
        //        courseCommentCategoryAddViewModel._failure = true;
        //    }
        //    return courseCommentCategoryAddViewModel;
        //}
        public CourseCommentCategoryAddViewModel AddCourseCommentCategory(CourseCommentCategoryAddViewModel courseCommentCategoryAddViewModel)
        {
            if (courseCommentCategoryAddViewModel.courseCommentCategory.Any() == false)
            {
                return courseCommentCategoryAddViewModel;
            }

            try
            {
                List<CourseCommentCategory> courseCommentCategoryList = new List<CourseCommentCategory>();
                int i = 0;
                var distinctCourseData = courseCommentCategoryAddViewModel.courseCommentCategory.Select(s => new { s.CourseId, s.TenantId, s.SchoolId }).Distinct().ToList();

                int? courseCommentId = 1;

                decimal? academicYear = Utility.GetCurrentAcademicYear(this.context!, courseCommentCategoryAddViewModel.courseCommentCategory.FirstOrDefault()!.TenantId, courseCommentCategoryAddViewModel.courseCommentCategory.FirstOrDefault()!.SchoolId);

                foreach (var course in distinctCourseData.ToList())
                {
                    int? sortOrder = 1;
                    var courseCommentCategoryDataForSortOrder = this.context?.CourseCommentCategory.Where(x => x.TenantId == course.TenantId && x.SchoolId == course.SchoolId && x.CourseId == course.CourseId).OrderByDescending(x => x.SortOrder).FirstOrDefault();
                    if (courseCommentCategoryDataForSortOrder != null)
                    {
                        sortOrder = courseCommentCategoryDataForSortOrder.SortOrder + 1;
                    }

                    var courseCommentCategoryData = courseCommentCategoryAddViewModel.courseCommentCategory.Where(x => x.CourseId == course.CourseId).ToList();

                    if (i == 0)
                    {
                        var courseCommentCategoryDataForID = this.context?.CourseCommentCategory.Where(x => x.TenantId == course.TenantId && x.SchoolId == course.SchoolId).OrderByDescending(x => x.CourseCommentId).FirstOrDefault();

                        if (courseCommentCategoryDataForID != null)
                        {
                            courseCommentId = courseCommentCategoryDataForID.CourseCommentId + 1;
                        }
                    }

                    foreach (var courseCommentCategory in courseCommentCategoryData)
                    {
                        if (courseCommentCategory.CourseCommentId > 0)
                        {
                            var courseCommentData = this.context?.CourseCommentCategory.FirstOrDefault(x => x.TenantId == courseCommentCategory.TenantId && x.SchoolId == courseCommentCategory.SchoolId && x.CourseId == courseCommentCategory.CourseId && x.CourseCommentId == courseCommentCategory.CourseCommentId);

                            if (courseCommentData != null)
                            {
                                courseCommentData.Comments = courseCommentCategory.Comments;
                                courseCommentData.UpdatedBy = courseCommentCategory.UpdatedBy;
                                courseCommentData.UpdatedOn = DateTime.UtcNow;
                            }        
                        }
                        else
                        {
                            courseCommentCategory.AcademicYear = academicYear;
                            courseCommentCategory.CourseCommentId = (int)courseCommentId;
                            courseCommentCategory.SortOrder = sortOrder;
                            courseCommentCategory.CreatedOn = DateTime.UtcNow;
                            courseCommentCategoryList.Add(courseCommentCategory);
                            courseCommentId++;
                            sortOrder++;
                        }
                    }
                    i++;
                }
                this.context?.CourseCommentCategory.AddRange(courseCommentCategoryList);
                this.context?.SaveChanges();
                courseCommentCategoryAddViewModel._failure = false;
                courseCommentCategoryAddViewModel._message = "Course Comment Category added successfully";
            }
            catch (Exception es)
            {
                courseCommentCategoryAddViewModel._message = es.Message;
                courseCommentCategoryAddViewModel._failure = true;
            }
            return courseCommentCategoryAddViewModel;
        }

        /// <summary>
        /// Delete Course Comment Category
        /// </summary>
        /// <param name="courseCommentCategoryDeleteViewModel"></param>
        /// <returns></returns>
        public CourseCommentCategoryDeleteViewModel DeleteCourseCommentCategory(CourseCommentCategoryDeleteViewModel courseCommentCategoryDeleteViewModel)
        {
            try
            {
                if (courseCommentCategoryDeleteViewModel.CourseCommentId != null)
                {
                    var courseCommentData = this.context?.CourseCommentCategory.Include(x => x.StudentFinalGradeComments).Where(x => x.TenantId == courseCommentCategoryDeleteViewModel.TenantId && x.SchoolId == courseCommentCategoryDeleteViewModel.SchoolId && x.CourseId == courseCommentCategoryDeleteViewModel.CourseId && x.CourseCommentId == courseCommentCategoryDeleteViewModel.CourseCommentId).FirstOrDefault();

                    if (courseCommentData != null)
                    {
                        if (courseCommentData.StudentFinalGradeComments.Any())
                        {
                            courseCommentCategoryDeleteViewModel._failure = true;
                            courseCommentCategoryDeleteViewModel._message = "Course Comments cannot be deleted because it has its association";
                        }
                        else
                        {
                            this.context?.CourseCommentCategory.Remove(courseCommentData);
                            this.context?.SaveChanges();
                            courseCommentCategoryDeleteViewModel._failure = false;
                            courseCommentCategoryDeleteViewModel._message = "Course Comments deleted successfullyy";
                        }

                    }
                }
                else
                {
                    var courseComments = this.context?.CourseCommentCategory.Include(x => x.StudentFinalGradeComments).Where(x => x.TenantId == courseCommentCategoryDeleteViewModel.TenantId && x.SchoolId == courseCommentCategoryDeleteViewModel.SchoolId && x.CourseId == courseCommentCategoryDeleteViewModel.CourseId).ToList();

                    if (courseComments != null && courseComments.Any())
                    {
                        var checkChild = courseComments.Where(x => x.StudentFinalGradeComments.Count > 0);
                        if (checkChild != null && checkChild.Any())
                        {
                            courseCommentCategoryDeleteViewModel._failure = true;
                            courseCommentCategoryDeleteViewModel._message = "Course Comments cannot be deleted because it has its association";
                        }
                        else
                        {
                            this.context?.CourseCommentCategory.RemoveRange(courseComments);
                            this.context?.SaveChanges();
                            courseCommentCategoryDeleteViewModel._failure = false;
                            courseCommentCategoryDeleteViewModel._message = "Course Comments deleted successfullyy";
                        }
                    }
                }
            }

            //var courseCommentData = this.context?.CourseCommentCategory.Include(x => x.StudentFinalGradeComments).Where(x => x.TenantId == courseCommentCategoryDeleteViewModel.TenantId && x.SchoolId == courseCommentCategoryDeleteViewModel.SchoolId && x.CourseId == courseCommentCategoryDeleteViewModel.CourseId && x.CourseCommentId == courseCommentCategoryDeleteViewModel.CourseCommentId).FirstOrDefault();

            //if (courseCommentData != null)
            //{
            //    if (courseCommentData.StudentFinalGradeComments.Any())
            //    {
            //        courseCommentCategoryDeleteViewModel._failure = true;
            //        courseCommentCategoryDeleteViewModel._message = "Course Comments cannot be deleted because it has its association";
            //    }
            //    else
            //    {
            //        this.context?.CourseCommentCategory.Remove(courseCommentData);
            //        this.context?.SaveChanges();
            //        courseCommentCategoryDeleteViewModel._failure = false;
            //        courseCommentCategoryDeleteViewModel._message = "Course Comments deleted successfullyy";
            //    }
            //}
            //else
            //{
            //    courseCommentCategoryDeleteViewModel._failure = true;
            //    courseCommentCategoryDeleteViewModel._message = NORECORDFOUND;
            //}

            catch (Exception es)
            {
                courseCommentCategoryDeleteViewModel._failure = true;
                courseCommentCategoryDeleteViewModel._message = es.Message;
            }
            return courseCommentCategoryDeleteViewModel;
        }

        /// <summary>
        /// Update Sort Order For Course Comment Category
        /// </summary>
        /// <param name="courseCommentCategorySortOrderViewModel"></param>
        /// <returns></returns>
        public CourseCommentCategorySortOrderViewModel UpdateSortOrderForCourseCommentCategory(CourseCommentCategorySortOrderViewModel courseCommentCategorySortOrderViewModel)
        {
            try
            {
                //if (courseCommentCategorySortOrderViewModel.CourseId > 0)
                //{
                    var SortOrderItem = new List<CourseCommentCategory>();

                    var targetSortOrderItem = this.context?.CourseCommentCategory.FirstOrDefault(x => x.SortOrder == courseCommentCategorySortOrderViewModel.PreviousSortOrder && x.SchoolId == courseCommentCategorySortOrderViewModel.SchoolId && x.TenantId == courseCommentCategorySortOrderViewModel.TenantId && x.CourseId == courseCommentCategorySortOrderViewModel.CourseId);

                    if (targetSortOrderItem != null)
                    {
                        targetSortOrderItem.SortOrder = courseCommentCategorySortOrderViewModel.CurrentSortOrder;
                        targetSortOrderItem.UpdatedBy = courseCommentCategorySortOrderViewModel.UpdatedBy;
                        targetSortOrderItem.UpdatedOn = DateTime.UtcNow;

                    if (courseCommentCategorySortOrderViewModel.PreviousSortOrder > courseCommentCategorySortOrderViewModel.CurrentSortOrder)
                        {
                            SortOrderItem = this.context?.CourseCommentCategory.Where(x => x.SortOrder >= courseCommentCategorySortOrderViewModel.CurrentSortOrder && x.SortOrder < courseCommentCategorySortOrderViewModel.PreviousSortOrder && x.SchoolId == courseCommentCategorySortOrderViewModel.SchoolId && x.TenantId == courseCommentCategorySortOrderViewModel.TenantId && x.CourseId == courseCommentCategorySortOrderViewModel.CourseId).ToList();

                            if (SortOrderItem != null && SortOrderItem.Any())
                            {
                                SortOrderItem.ForEach(x => { x.SortOrder = x.SortOrder + 1; x.UpdatedOn = DateTime.UtcNow; x.UpdatedBy = courseCommentCategorySortOrderViewModel.UpdatedBy; });
                            }
                        }

                        if (courseCommentCategorySortOrderViewModel.CurrentSortOrder > courseCommentCategorySortOrderViewModel.PreviousSortOrder)
                        {
                            SortOrderItem = this.context?.CourseCommentCategory.Where(x => x.SortOrder <= courseCommentCategorySortOrderViewModel.CurrentSortOrder && x.SortOrder > courseCommentCategorySortOrderViewModel.PreviousSortOrder && x.SchoolId == courseCommentCategorySortOrderViewModel.SchoolId && x.TenantId == courseCommentCategorySortOrderViewModel.TenantId && x.CourseId == courseCommentCategorySortOrderViewModel.CourseId).ToList();

                        if (SortOrderItem != null && SortOrderItem.Any())
                            {
                                SortOrderItem.ForEach(x => { x.SortOrder = x.SortOrder - 1; ; x.UpdatedOn = DateTime.UtcNow; x.UpdatedBy = courseCommentCategorySortOrderViewModel.UpdatedBy; });
                            }
                        }
                    }
                    this.context?.SaveChanges();
                    courseCommentCategorySortOrderViewModel._failure = false;
                //}
            }
            catch (Exception es)
            {
                courseCommentCategorySortOrderViewModel._message = es.Message;
                courseCommentCategorySortOrderViewModel._failure = true;
            }
            return courseCommentCategorySortOrderViewModel;
        }
         
        /// <summary>
        /// Get All Course Comment Category With Report Card Comments
        /// </summary>
        /// <param name="courseCommentCategoryListViewModel"></param>
        /// <returns></returns>
        public CourseCommentCategoryListViewModel GetAllCourseCommentCategory(CourseCommentCategoryListViewModel courseCommentCategoryListViewModel)
        {
            CourseCommentCategoryListViewModel courseCommentCategoryList = new CourseCommentCategoryListViewModel();
            try
            {
                courseCommentCategoryList.TenantId = courseCommentCategoryListViewModel.TenantId;
                courseCommentCategoryList.SchoolId = courseCommentCategoryListViewModel.SchoolId;
                courseCommentCategoryList._tenantName = courseCommentCategoryListViewModel._tenantName;
                courseCommentCategoryList._token = courseCommentCategoryListViewModel._token;
                courseCommentCategoryList._userName = courseCommentCategoryListViewModel._userName;

                var courseCommentCategoryData = this.context?.CourseCommentCategory.Where(x => x.TenantId == courseCommentCategoryListViewModel.TenantId && x.SchoolId == courseCommentCategoryListViewModel.SchoolId && x.AcademicYear== courseCommentCategoryListViewModel.AcademicYear).ToList();

                if (courseCommentCategoryData != null && courseCommentCategoryData.Any())
                {
                    if (courseCommentCategoryListViewModel.IsListView == true)
                    {
                        courseCommentCategoryData.ForEach(c =>
                        {
                            c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, courseCommentCategoryListViewModel.TenantId, c.CreatedBy);
                            c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, courseCommentCategoryListViewModel.TenantId, c.UpdatedBy);
                        });
                    }

                    courseCommentCategoryList.courseCommentCategories = courseCommentCategoryData;
                    courseCommentCategoryList._failure = false;
                }
                else
                {
                    courseCommentCategoryList._message = NORECORDFOUND;
                    courseCommentCategoryList._failure = true;
                }
            }
            catch (Exception es)
            {
                courseCommentCategoryList.courseCommentCategories = null!;
                courseCommentCategoryList._message = es.Message;
                courseCommentCategoryList._failure = true;
            }
            return courseCommentCategoryList;
        }

        /// <summary>
        /// Add Report Card
        /// </summary>
        /// <param name="reportCardViewModel"></param>
        /// <returns></returns>
        public ReportCardViewModel AddReportCard(ReportCardViewModel reportCardViewModel)
        {
            if (reportCardViewModel.MarkingPeriods is null)
            {
                return reportCardViewModel;
            }
            try
            {
                List<string> teacherComments = new List<string>();

                int i = 0;
                long? ide = 1;
                int teacherCommentsNo = 1;
                if (reportCardViewModel.studentsReportCardViewModelList.Count > 0)
                {
                    reportCardViewModel.AcademicYear= Utility.GetCurrentAcademicYear(this.context!, reportCardViewModel.TenantId, reportCardViewModel.SchoolId);

                    if (reportCardViewModel.TemplateType?.ToLower() == "default")
                    {

                        foreach (var student in reportCardViewModel.studentsReportCardViewModelList)
                        {
                            List<StudentReportCardMaster> studentReportCardMasterList = new List<StudentReportCardMaster>();
                            List<StudentReportCardDetail> studentReportCardDetailList = new List<StudentReportCardDetail>();

                            var existingStudentReportCardData = this.context?.StudentReportCardMaster.Where(x => x.SchoolId == reportCardViewModel.SchoolId && x.TenantId == reportCardViewModel.TenantId && x.StudentId == student.StudentId && x.SchoolYear == reportCardViewModel.AcademicYear.ToString()).ToList();

                            if (existingStudentReportCardData != null)
                            {
                                var existingStudentReportCardDetailsData = this.context?.StudentReportCardDetail.Where(x => x.SchoolId == reportCardViewModel.SchoolId && x.TenantId == reportCardViewModel.TenantId && x.StudentId == student.StudentId && x.SchoolYear == reportCardViewModel.AcademicYear.ToString()).ToList();
                                if (existingStudentReportCardDetailsData != null && existingStudentReportCardDetailsData.Any())
                                {
                                    this.context?.StudentReportCardDetail.RemoveRange(existingStudentReportCardDetailsData);
                                }
                                this.context?.StudentReportCardMaster.RemoveRange(existingStudentReportCardData);
                                this.context?.SaveChanges();
                            }
                            if (i == 0)
                            {
                                var idData = this.context?.StudentReportCardDetail.Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId).OrderByDescending(x => x.Id).FirstOrDefault();

                                if (idData != null)
                                {
                                    ide = idData.Id + 1;
                                }
                            }

                            int? absencesInDays = 0;

                            var studentData = this.context?.StudentMaster.Include(x => x.StudentEnrollment).FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId);

                            //var GradeLevelTitle = studentData.StudentEnrollment.Where(x => x.IsActive == true).Select(s => s.GradeLevelTitle).FirstOrDefault();

                            var GradeLevelTitle = studentData!.StudentEnrollment.Where(x => x.IsActive == true).Select(s => s.GradeLevelTitle).FirstOrDefault();

                            var markingPeriodsData = reportCardViewModel.MarkingPeriods.Split(",");
                            DateTime? startDate = null;
                            DateTime? endDate = null;
                            string MarkingPeriodTitle = null!;

                            foreach (var markingPeriod in markingPeriodsData)
                            {
                                List<DateTime> holidayList = new List<DateTime>();
                                int? Absences = 0;
                                int? ExcusedAbsences = 0;
                                int? QtrMarkingPeriodId = null;
                                int? SmstrMarkingPeriodId = null;
                                int? YrMarkingPeriodId = null;
                                int? PrgrsprdMarkingPeriodId = null;

                                if (markingPeriod != null)
                                {
                                    var markingPeriodid = markingPeriod.Split("_", StringSplitOptions.RemoveEmptyEntries);

                                    if (markingPeriodid.First() == "3")
                                    {
                                        PrgrsprdMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));

                                        var ppData = this.context?.ProgressPeriods.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == PrgrsprdMarkingPeriodId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                        //MarkingPeriodTitle = qtrData.Title;
                                        MarkingPeriodTitle = ppData!.Title!;
                                        startDate = ppData.StartDate;
                                        endDate = ppData.EndDate;
                                    }

                                    if (markingPeriodid.First() == "2")
                                    {
                                        QtrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));

                                        var qtrData = this.context?.Quarters.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == QtrMarkingPeriodId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                        //MarkingPeriodTitle = qtrData.Title;
                                        MarkingPeriodTitle = qtrData!.Title!;
                                        startDate = qtrData.StartDate;
                                        endDate = qtrData.EndDate;
                                    }

                                    if (markingPeriodid.First() == "1")
                                    {
                                        SmstrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));

                                        var smstrData = this.context?.Semesters.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == SmstrMarkingPeriodId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                        //MarkingPeriodTitle = smstrData.Title;
                                        MarkingPeriodTitle = smstrData!.Title!;
                                        startDate = smstrData.StartDate;
                                        endDate = smstrData.EndDate;
                                    }

                                    if (markingPeriodid.First() == "0")
                                    {
                                        YrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));

                                        var yrData = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == YrMarkingPeriodId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                        //MarkingPeriodTitle = yrData.Title;
                                        MarkingPeriodTitle = yrData!.Title!;
                                        startDate = yrData.StartDate;
                                        endDate = yrData.EndDate;
                                    }

                                    var studentAttendanceData = this.context?.StudentAttendance.Include(x => x.AttendanceCodeNavigation).Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId && x.AttendanceDate >= startDate && x.AttendanceDate <= endDate).ToList();

                                    if (studentAttendanceData != null && studentAttendanceData.Any())
                                    {
                                        Absences = studentAttendanceData.Where(x => x.AttendanceCodeNavigation.StateCode.ToLower() == "absent").Count();
                                        ExcusedAbsences = studentAttendanceData.Where(x => x.AttendanceCodeNavigation.StateCode.ToLower() == "excusedabsent").Count();
                                        var prasentData = studentAttendanceData.Where(x => x.AttendanceCodeNavigation.StateCode.ToLower() == "present");

                                        absencesInDays += Absences + ExcusedAbsences;
                                    }

                                    var reportCardData = this.context?.StudentFinalGrade.Include(x => x.StudentFinalGradeComments).Include(x => x.StudentFinalGradeStandard).Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId && x.AcademicYear == reportCardViewModel.AcademicYear && ((x.YrMarkingPeriodId != null && x.YrMarkingPeriodId == YrMarkingPeriodId) || (x.SmstrMarkingPeriodId != null && x.SmstrMarkingPeriodId == SmstrMarkingPeriodId) || (x.QtrMarkingPeriodId != null && x.QtrMarkingPeriodId == QtrMarkingPeriodId) || (x.PrgrsprdMarkingPeriodId != null && x.PrgrsprdMarkingPeriodId == PrgrsprdMarkingPeriodId))).ToList();

                                    decimal? gPaValue = 0.0m;
                                    decimal? CreditEarned = 0.0m;
                                    decimal? CreditHours = 0.0m;


                                    if (reportCardData != null && reportCardData.Any())
                                    {
                                        foreach (var reportCard in reportCardData)
                                        {
                                            var CourseSectionData = this.context?.CourseSection.Include(x => x.StaffCoursesectionSchedule).ThenInclude(x => x.StaffMaster).FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.CourseSectionId == reportCard.CourseSectionId && x.CourseId == reportCard.CourseId);

                                            var gradeData = this.context?.Grade.FirstOrDefault(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && x.Title == reportCard.GradeObtained && x.GradeScaleId == reportCard.GradeScaleId);

                                            if (gradeData != null)
                                            {
                                                //CreditHours = CourseSectionData.CreditHours;
                                                CreditHours = CourseSectionData!.CreditHours;
                                                CreditEarned = reportCard.CreditEarned != null ? reportCard.CreditEarned : CourseSectionData.CreditHours;
                                                gPaValue = CourseSectionData.IsWeightedCourse != true ? gradeData.UnweightedGpValue * (CreditHours / CreditEarned) : gradeData.WeightedGpValue * (CreditHours / CreditEarned);

                                            }

                                            var comments = reportCard.StudentFinalGradeComments.Select(x => x.CourseCommentId).ToList();
                                            var Comments = string.Join(",", comments.Select(x => x.ToString()).ToArray());

                                            var studentReportCardDetail = new StudentReportCardDetail()
                                            {
                                                Id = (long)ide,
                                                TenantId = reportCardViewModel.TenantId,
                                                SchoolId = reportCardViewModel.SchoolId,
                                                StudentId = student.StudentId,
                                                SchoolYear = reportCardViewModel.AcademicYear.ToString()!,
                                                GradeTitle = GradeLevelTitle!,
                                                MarkingPeriodTitle = MarkingPeriodTitle,
                                                CourseName = CourseSectionData!.CourseSectionName,
                                                Teacher = reportCardViewModel.TeacherName == true ? CourseSectionData.StaffCoursesectionSchedule.Count > 0 ? CourseSectionData.StaffCoursesectionSchedule.FirstOrDefault()!.StaffMaster.FirstGivenName + " " + CourseSectionData.StaffCoursesectionSchedule.FirstOrDefault()!.StaffMaster.MiddleName + " " + CourseSectionData.StaffCoursesectionSchedule.FirstOrDefault()!.StaffMaster.LastFamilyName : null : null,
                                                Grade = reportCardViewModel.Parcentage != true ? reportCard.GradeObtained : reportCard.GradeObtained + "(" + reportCard.PercentMarks + ")",
                                                Gpa = reportCardViewModel.GPA == true ? gPaValue : null,
                                                Comments = Comments,
                                                TeacherComments = reportCardViewModel.TeacherComments == true ? reportCard.TeacherComment != null ? (teacherCommentsNo++).ToString() : null : null,
                                                OverallTeacherComments = reportCard.TeacherComment,

                                                CreatedBy = reportCardViewModel.CreatedBy,
                                                CreatedOn = DateTime.UtcNow

                                            };
                                            studentReportCardDetailList.Add(studentReportCardDetail);
                                            ide++;

                                        }
                                        this.context?.StudentReportCardDetail.AddRange(studentReportCardDetailList);
                                    }

                                    double attendencePercent = 0;
                                    int prasentDay = 0;

                                    var calenderData = this.context?.SchoolCalendars.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.DefaultCalender == true && x.AcademicYear == reportCardViewModel.AcademicYear);

                                    var schoolYearData = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                    if (calenderData != null && schoolYearData != null)
                                    { 
                                      
                                        DateTime schoolYearStartDate = (DateTime)schoolYearData.StartDate!;
                                        DateTime schoolYearEndDate = (DateTime)schoolYearData.EndDate!;

                                        // Calculate Holiday
                                         var CalendarEventsData = this.context?.CalendarEvents.Where(e => e.TenantId == reportCardViewModel.TenantId && e.AcademicYear == reportCardViewModel.AcademicYear && (e.StartDate >= schoolYearStartDate && e.StartDate <= schoolYearEndDate || e.EndDate >= schoolYearStartDate && e.EndDate <= schoolYearEndDate) && e.IsHoliday == true && (e.SchoolId == reportCardViewModel.SchoolId || e.ApplicableToAllSchool == true)).ToList();

                                        if (CalendarEventsData != null && CalendarEventsData.Any())
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

                                        var daysValue = "0123456";
                                        var weekdays = calenderData.Days;
                                        //var WeekOffDays = Regex.Split(daysValue, weekdays);
                                        var WeekOffDays = Regex.Split(daysValue, weekdays!);
                                        var WeekOfflist = new List<string>();
                                        foreach (var WeekOffDay in WeekOffDays)
                                        {
                                            Days days = new Days();
                                            var Day = Enum.GetName(days.GetType(), Convert.ToInt32(WeekOffDay));
                                            //WeekOfflist.Add(Day);
                                            WeekOfflist.Add(Day!);
                                        }

                                        int workDays = 0;
                                        while (schoolYearStartDate != schoolYearEndDate)
                                        {
                                            if (!holidayList.Contains(schoolYearStartDate))
                                            {
                                                if (!WeekOfflist.Contains(schoolYearStartDate.DayOfWeek.ToString()))
                                                {
                                                    workDays++;
                                                }
                                                schoolYearStartDate = schoolYearStartDate.AddDays(1);
                                            }

                                        }

                                        var studentPrasentAttendanceData = this.context?.StudentAttendance.Include(x => x.AttendanceCodeNavigation).Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId && x.AttendanceDate >= calenderData.StartDate && x.AttendanceDate <= calenderData.EndDate).ToList();

                                        if (studentPrasentAttendanceData != null && studentPrasentAttendanceData.Any())
                                        {
                                            prasentDay = studentPrasentAttendanceData.Where(x => x.AttendanceCodeNavigation.StateCode.ToLower() == "present").Count();
                                        }
                                        var presentDays = workDays - absencesInDays;
                                        attendencePercent = (double)(presentDays * 100 / workDays);
                                        //attendencePercent = ((presentDays / workDays) * 100);
                                    }

                                    var studentReportCardMaster = new StudentReportCardMaster
                                    {
                                        TenantId = reportCardViewModel.TenantId,
                                        SchoolId = reportCardViewModel.SchoolId,
                                        StudentId = student.StudentId,
                                        SchoolYear = reportCardViewModel.AcademicYear.ToString()!,
                                        GradeTitle = GradeLevelTitle!,
                                        StudentInternalId = studentData.StudentInternalId,
                                        YodAbsence = reportCardViewModel.YearToDateDailyAbsences == true ? absencesInDays : null,
                                        YodAttendance = reportCardViewModel.YearToDateDailyAbsences == true ? Math.Round(attendencePercent, 2).ToString() + "%" : null,
                                        ReportGenerationDate = DateTime.UtcNow,
                                        Absences = reportCardViewModel.DailyAbsencesThisMarkingPeriod == true ? Absences : null,
                                        ExcusedAbsences = reportCardViewModel.DailyAbsencesThisMarkingPeriod == true ? ExcusedAbsences : null,
                                        MarkingPeriodTitle = MarkingPeriodTitle,
                                        CreatedBy = reportCardViewModel.CreatedBy,
                                        CreatedOn = DateTime.UtcNow
                                    };
                                    studentReportCardMasterList.Add(studentReportCardMaster);
                                }
                            }
                            this.context?.StudentReportCardMaster.AddRange(studentReportCardMasterList);
                            i++;
                        }
                    }
                    else
                    {
                        foreach (var student in reportCardViewModel.studentsReportCardViewModelList)
                        {
                            List<StudentReportCardMaster> studentReportCardMasterList = new List<StudentReportCardMaster>();
                            List<StudentReportCardDetail> studentReportCardDetailList = new List<StudentReportCardDetail>();

                            var existingStudentReportCardData = this.context?.StudentReportCardMaster.Where(x => x.SchoolId == reportCardViewModel.SchoolId && x.TenantId == reportCardViewModel.TenantId && x.StudentId == student.StudentId && x.SchoolYear == reportCardViewModel.AcademicYear.ToString()).ToList();

                            if (existingStudentReportCardData != null && existingStudentReportCardData.Any())
                            {
                                var existingStudentReportCardDetailsData = this.context?.StudentReportCardDetail.Where(x => x.SchoolId == reportCardViewModel.SchoolId && x.TenantId == reportCardViewModel.TenantId && x.StudentId == student.StudentId && x.SchoolYear == reportCardViewModel.AcademicYear.ToString()).ToList();

                                if (existingStudentReportCardDetailsData != null && existingStudentReportCardDetailsData.Any())
                                {
                                    this.context?.StudentReportCardDetail.RemoveRange(existingStudentReportCardDetailsData);
                                }
                                this.context?.StudentReportCardMaster.RemoveRange(existingStudentReportCardData);
                                this.context?.SaveChanges();
                            }
                            if (i == 0)
                            {
                                var idData = this.context?.StudentReportCardDetail.Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId).OrderByDescending(x => x.Id).FirstOrDefault();

                                if (idData != null)
                                {
                                    ide = idData.Id + 1;
                                }
                            }

                            var studentData = this.context?.StudentMaster.Include(x => x.StudentEnrollment).FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId);

                            //var GradeLevelTitle = studentData.StudentEnrollment.Where(x => x.IsActive == true).Select(s => s.GradeLevelTitle).FirstOrDefault();

                            var GradeLevelTitle = studentData!.StudentEnrollment.Where(x => x.IsActive == true).Select(s => s.GradeLevelTitle).FirstOrDefault();

                            var markingPeriodsData = reportCardViewModel.MarkingPeriods.Split(",");
                            DateTime? startDate = null;
                            DateTime? endDate = null;
                            string? MarkingPeriodTitle = null;

                            foreach (var markingPeriod in markingPeriodsData)
                            {
                                int? QtrMarkingPeriodId = null;
                                int? SmstrMarkingPeriodId = null;
                                int? YrMarkingPeriodId = null;
                                int? PrgrsprdMarkingPeriodId = null;

                                if (markingPeriod != null)
                                {
                                    var markingPeriodid = markingPeriod.Split("_", StringSplitOptions.RemoveEmptyEntries);

                                    if (markingPeriodid.First() == "3")
                                    {
                                        PrgrsprdMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));

                                        var ppData = this.context?.ProgressPeriods.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == PrgrsprdMarkingPeriodId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                        MarkingPeriodTitle = ppData!.ShortName;
                                        startDate = ppData.StartDate;
                                        endDate = ppData.EndDate;
                                    }

                                    if (markingPeriodid.First() == "2")
                                    {
                                        QtrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));

                                        var qtrData = this.context?.Quarters.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == QtrMarkingPeriodId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                        MarkingPeriodTitle = qtrData!.ShortName;
                                        startDate = qtrData.StartDate;
                                        endDate = qtrData.EndDate;
                                    }

                                    if (markingPeriodid.First() == "1")
                                    {
                                        SmstrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));

                                        var smstrData = this.context?.Semesters.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == SmstrMarkingPeriodId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                        MarkingPeriodTitle = smstrData!.ShortName;
                                        startDate = smstrData.StartDate;
                                        endDate = smstrData.EndDate;
                                    }

                                    if (markingPeriodid.First() == "0")
                                    {
                                        YrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));

                                        var yrData = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == YrMarkingPeriodId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                        MarkingPeriodTitle = yrData!.ShortName;
                                        startDate = yrData.StartDate;
                                        endDate = yrData.EndDate;
                                    }                                   

                                    var reportCardData = this.context?.StudentFinalGrade.Include(x => x.StudentFinalGradeComments).Include(x => x.StudentFinalGradeStandard).Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId && x.AcademicYear == reportCardViewModel.AcademicYear && ((x.YrMarkingPeriodId != null && x.YrMarkingPeriodId == YrMarkingPeriodId) || (x.SmstrMarkingPeriodId != null && x.SmstrMarkingPeriodId == SmstrMarkingPeriodId) || (x.QtrMarkingPeriodId != null && x.QtrMarkingPeriodId == QtrMarkingPeriodId) || (x.PrgrsprdMarkingPeriodId != null && x.PrgrsprdMarkingPeriodId == PrgrsprdMarkingPeriodId))).ToList();

                                    decimal? gPaValue = 0.0m;
                                    decimal? CreditEarned = 0.0m;
                                    decimal? CreditHours = 0.0m;

                                    if (reportCardData != null && reportCardData.Any())
                                    {
                                        foreach (var reportCard in reportCardData)
                                        {
                                            var CourseSectionData = this.context?.CourseSection.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.CourseSectionId == reportCard.CourseSectionId && x.CourseId == reportCard.CourseId);

                                            var gradeData = this.context?.Grade.FirstOrDefault(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && x.Title == reportCard.GradeObtained && x.GradeScaleId == reportCard.GradeScaleId);

                                            if (gradeData != null)
                                            {
                                                //CreditHours = CourseSectionData.CreditHours;
                                                CreditHours = CourseSectionData!.CreditHours;
                                                CreditEarned = reportCard.CreditEarned != null ? reportCard.CreditEarned : CourseSectionData.CreditHours;
                                                gPaValue = CourseSectionData.IsWeightedCourse != true ? gradeData.UnweightedGpValue * (CreditHours / CreditEarned) : gradeData.WeightedGpValue * (CreditHours / CreditEarned);

                                            }
                                            var studentReportCardDetail = new StudentReportCardDetail()
                                            {
                                                Id = (long)ide,
                                                TenantId = reportCardViewModel.TenantId,
                                                SchoolId = reportCardViewModel.SchoolId,
                                                StudentId = student.StudentId,
                                                SchoolYear = reportCardViewModel.AcademicYear.ToString()!,
                                                GradeTitle = GradeLevelTitle!,
                                                MarkingPeriodTitle = MarkingPeriodTitle,
                                                CourseName = CourseSectionData!.CourseSectionName,
                                                Grade = reportCardViewModel.Parcentage != true ? reportCard.GradeObtained : reportCard.GradeObtained + "(" + reportCard.PercentMarks + ")",
                                                Gpa = reportCardViewModel.GPA == true ? gPaValue : null,
                                                CreatedBy = reportCardViewModel.CreatedBy,
                                                CreatedOn = DateTime.UtcNow
                                            };
                                            studentReportCardDetailList.Add(studentReportCardDetail);
                                            ide++;
                                        }
                                        this.context?.StudentReportCardDetail.AddRange(studentReportCardDetailList);
                                    }

                                    var attendanceData = this.context?.AttendanceCodeCategories.Include(x => x.AttendanceCode).FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                    if(attendanceData!=null)
                                    {
                                        foreach (var Attendance in attendanceData.AttendanceCode.ToList())
                                        {
                                            var studentDailyAttendanceCount = this.context?.StudentDailyAttendance.Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId && x.AttendanceCode == Attendance.Title && x.AttendanceDate >= startDate && x.AttendanceDate <= endDate).ToList().Count;
                                          
                                        }
                                    }

                                    var studentReportCardMaster = new StudentReportCardMaster
                                    {
                                        TenantId = reportCardViewModel.TenantId,
                                        SchoolId = reportCardViewModel.SchoolId,
                                        StudentId = student.StudentId,
                                        SchoolYear = reportCardViewModel.AcademicYear.ToString()!,
                                        GradeTitle = GradeLevelTitle!,
                                        StudentInternalId = studentData.StudentInternalId,
                                        ReportGenerationDate = DateTime.UtcNow,
                                        MarkingPeriodTitle = MarkingPeriodTitle!,
                                        CreatedBy = reportCardViewModel.CreatedBy,
                                        CreatedOn = DateTime.UtcNow
                                    };
                                    studentReportCardMasterList.Add(studentReportCardMaster);
                                }
                            }
                            this.context?.StudentReportCardMaster.AddRange(studentReportCardMasterList);
                            i++;
                        }
                    }
                    this.context?.SaveChanges();
                    reportCardViewModel._message = "added successfully";
                }
                else
                {
                    reportCardViewModel._failure = true;
                    reportCardViewModel._message = "Select Student Please";
                }
            }
            catch (Exception es)
            {
                reportCardViewModel._failure = true;
                reportCardViewModel._message = es.Message;
            }
            return reportCardViewModel;
        }


        /// <summary>
        /// Generate Report Card
        /// </summary>
        /// <param name="reportCardViewModel"></param>
        /// <returns></returns>
        public async Task<ReportCardViewModel> GenerateReportCard(ReportCardViewModel reportCardViewModel)
        {
            ReportCardViewModel reportCardView = new ReportCardViewModel();
            try
            {
                reportCardView.TenantId = reportCardViewModel.TenantId;
                reportCardView.SchoolId = reportCardViewModel.SchoolId;
                reportCardView._tenantName = reportCardViewModel._tenantName;
                reportCardView._userName = reportCardViewModel._userName;
                string? base64 = null;
                object data = new object();

                List<object> reportCardList = new List<object>();
                List<object> teacherCommentList = new List<object>();

                if (reportCardViewModel?.TemplateType?.ToLower() == "default")
                {

                    foreach (var student in reportCardViewModel.studentsReportCardViewModelList)
                    {
                        var studentNameData = this.context?.StudentMaster.FirstOrDefault(x => x.SchoolId == reportCardViewModel.SchoolId && x.TenantId == reportCardViewModel.TenantId && x.StudentId == student.StudentId);

                        var schoolData = this.context?.SchoolMaster.FirstOrDefault(x => x.SchoolId == reportCardViewModel.SchoolId && x.TenantId == reportCardViewModel.TenantId);

                        var studentReportCardData = this.context?.StudentReportCardMaster.Include(x => x.StudentReportCardDetail).Where(x => x.SchoolId == reportCardViewModel.SchoolId && x.TenantId == reportCardViewModel.TenantId && x.SchoolYear == reportCardViewModel.AcademicYear.ToString() && x.StudentId == student.StudentId).ToList();

                        if (studentNameData != null && schoolData != null && studentReportCardData != null && studentReportCardData.Any())
                        {
                            List<object> reportDetailsList = new List<object>();
                            foreach (var studentReportCard in studentReportCardData)
                            {
                                var studentReportCardDetailsData = studentReportCard.StudentReportCardDetail.Where(x => x.MarkingPeriodTitle == studentReportCard.MarkingPeriodTitle).ToList();

                                //var teacherCommentsData = studentReportCardDetailsData.Where(x => x.OverallTeacherComments != null && x.TeacherComments != null).ToList().Select(x => new { x.TeacherComments, x.OverallTeacherComments });

                                var teacherCommentsData = studentReportCardDetailsData.Where(x => x.OverallTeacherComments != null && x.TeacherComments != null).ToList().Select(x => new { x?.TeacherComments, x?.OverallTeacherComments });

                                teacherCommentList.AddRange(teacherCommentsData);
                                object markingPeriodWiseData = new
                                {
                                    studentReportCard.MarkingPeriodTitle,
                                    studentReportCard.ExcusedAbsences,
                                    studentReportCard.Absences,
                                    Details = studentReportCardDetailsData,

                                };
                                reportDetailsList.Add(markingPeriodWiseData);
                            }

                            object reportCard = new
                            {
                                SchoolData = schoolData,
                                studentInternalId = studentReportCardData.FirstOrDefault()!.StudentInternalId,
                                gradeTitle = studentReportCardData.FirstOrDefault()!.GradeTitle,
                                yodAttendance = studentReportCardData.FirstOrDefault()!.YodAttendance,
                                yodAbsence = studentReportCardData.FirstOrDefault()!.YodAbsence,
                                ReportdetailsData = reportDetailsList,
                                StudentName = studentNameData,

                            };
                            reportCardList.Add(reportCard);
                        }
                    }

                    if (reportCardList != null)
                    {
                        var courseCommentCategoryData = this.context?.CourseCommentCategory.Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId).ToList();

                        data = new
                        {
                            TotalData = reportCardList,
                            CourseCommentCategoryData = courseCommentCategoryData,
                            TeacherCommentList = reportCardViewModel.TeacherComments == true ? teacherCommentList : null
                        };
                    }

                    GenerateReportCard _report = new GenerateReportCard();
                    var message = await _report.Generate(data);

                    bool isWindows = System.Runtime.InteropServices.RuntimeInformation
                                           .IsOSPlatform(OSPlatform.Windows);
                    if (isWindows)
                    {
                        using (var fileStream = new FileStream(@"ReportCard\\StudentReport.pdf", FileMode.Open))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                fileStream.CopyTo(memoryStream);
                                byte[] bytes = memoryStream.ToArray();
                                base64 = Convert.ToBase64String(bytes);
                                fileStream.Close();
                            }
                        }
                        reportCardView.ReportCardPdf = base64;
                    }
                    else
                    {
                        using (var fileStream = new FileStream(@"ReportCard/StudentReport.pdf", FileMode.Open))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                fileStream.CopyTo(memoryStream);
                                byte[] bytes = memoryStream.ToArray();
                                base64 = Convert.ToBase64String(bytes);
                                fileStream.Close();
                            }
                        }
                        reportCardView.ReportCardPdf = base64;
                    }
                }
                else
                {
                    if (reportCardViewModel!.studentsReportCardViewModelList is null)
                    {
                        return reportCardViewModel;
                    }
                    SchoolMaster schoolData = new SchoolMaster();
                    foreach (var student in reportCardViewModel.studentsReportCardViewModelList)
                    {
                        var studentData = this.context?.StudentMaster.Include(s => s.Sections).FirstOrDefault(x => x.SchoolId == reportCardViewModel.SchoolId && x.TenantId == reportCardViewModel.TenantId && x.StudentId == student.StudentId);

                        //schoolData = this.context?.SchoolMaster.Include(x => x.SchoolDetail).Include(x => x.GradeScale).ThenInclude(x => x.Grade).Include(s => s.AttendanceCodeCategories).ThenInclude(s => s.AttendanceCode).FirstOrDefault(x => x.SchoolId == reportCardViewModel.SchoolId && x.TenantId == reportCardViewModel.TenantId);

                        schoolData = this.context?.SchoolMaster.Include(x => x.SchoolDetail).Include(x => x.GradeScale).ThenInclude(x => x.Grade).Include(s => s.AttendanceCodeCategories).ThenInclude(s => s.AttendanceCode).FirstOrDefault(x => x.SchoolId == reportCardViewModel.SchoolId && x.TenantId == reportCardViewModel.TenantId)!;

                        var studentReportCardData = this.context?.StudentReportCardMaster.Include(x => x.StudentReportCardDetail).Where(x => x.SchoolId == reportCardViewModel.SchoolId && x.TenantId == reportCardViewModel.TenantId && x.SchoolYear == reportCardViewModel.AcademicYear.ToString() && x.StudentId == student.StudentId).ToList();

                        if (studentData != null && schoolData != null && studentReportCardData?.Any() == true)
                        {
                            List<object> reportDetailsList = new List<object>();
                            List<object> attendanceList = new List<object>();
                            int? count = studentReportCardData?.Count;
                            List<string> courseSectionList = new List<string>();
                            List<object> courseSectionWithGradeList = new List<object>();

                            foreach (var studentReportCard in studentReportCardData!)
                            {
                                var courseSection = studentReportCard.StudentReportCardDetail.Where(x => x.TenantId == reportCardViewModel.TenantId && x.StudentId == student.StudentId).Select(s => s.CourseName).ToList();
                                if(courseSection?.Any()==true)
                                {
                                    courseSectionList.AddRange(courseSection!);
                                    courseSectionList = courseSectionList.Distinct().ToList();
                                }
                            }

                            var reportCardDetailsList = this.context?.StudentReportCardDetail.Where(x => x.SchoolId == reportCardViewModel.SchoolId && x.TenantId == reportCardViewModel.TenantId && x.SchoolYear == reportCardViewModel.AcademicYear.ToString() && x.StudentId == student.StudentId).ToList();
                            foreach (var cs in courseSectionList)
                            {
                                List<object> csGradeMPWiseList = new List<object>();

                                foreach (var studentReportCard in studentReportCardData)
                                {
                                    var csGradeInMP = reportCardDetailsList!.FirstOrDefault(x => x.MarkingPeriodTitle == studentReportCard.MarkingPeriodTitle && x.CourseName==cs);

                                    if (csGradeInMP != null)
                                    {
                                        object csGradeMPWise = new
                                        {
                                            grade = csGradeInMP.Grade
                                        };
                                        csGradeMPWiseList.Add(csGradeMPWise);
                                    }
                                    else
                                    {
                                        object csGradeMPWise = new
                                        {
                                            grade = "N/A"
                                        };
                                        csGradeMPWiseList.Add(csGradeMPWise);
                                    }
                                }
                                object csDetails = new
                                {
                                    courseSectionTitle= cs,
                                    gradeList = csGradeMPWiseList
                                };
                                courseSectionWithGradeList.Add(csDetails);
                            }

                            //this block for GPA
                            List<object> gpaList = new List<object>();
                            foreach (var studentReportCard in studentReportCardData)
                            {
                                var gpa = studentReportCard.StudentReportCardDetail.Sum(x => x.Gpa);
                                var csCount = studentReportCard.StudentReportCardDetail.Count;
                                decimal? Gpavalue = 0.0m;
                                if (gpa > 0 && csCount > 0)
                                {
                                    Gpavalue = Math.Round((decimal)(gpa / csCount), 2);
                                }

                                object GPA = new
                                {
                                    gpaValue = Gpavalue
                                };
                                gpaList.Add(GPA);
                            }

                            foreach (var studentReportCard in studentReportCardData)
                            {
                                //var studentReportCardDetailsData = studentReportCard.StudentReportCardDetail.Where(x => x.MarkingPeriodTitle == studentReportCard.MarkingPeriodTitle).ToList();

                                object markingPeriodWiseData = new
                                {
                                    MarkingPeriod = studentReportCard.MarkingPeriodTitle,
                                    //Details = studentReportCardDetailsData,
                                };
                                reportDetailsList.Add(markingPeriodWiseData);                               
                            }
                           
                            //this block for attendance
                            if (schoolData.AttendanceCodeCategories.Count > 0)
                            {
                                foreach (var AttendanceCode in schoolData.AttendanceCodeCategories.FirstOrDefault()!.AttendanceCode)
                                {
                                    List<object> AttendanceCountList = new List<object>();
                                    var markingPeriodsData = reportCardViewModel.MarkingPeriods!.Split(",");
                                    DateTime? startDate = null;
                                    DateTime? endDate = null;
                                    string? MarkingPeriodTitle = null;

                                    foreach (var markingPeriod in markingPeriodsData)
                                    {
                                        int? QtrMarkingPeriodId = null;
                                        int? SmstrMarkingPeriodId = null;
                                        int? YrMarkingPeriodId = null;
                                        int? PrgrsprdMarkingPeriodId = null;

                                        if (markingPeriod != null)
                                        {
                                            var markingPeriodid = markingPeriod.Split("_", StringSplitOptions.RemoveEmptyEntries);

                                            if (markingPeriodid.First() == "3")
                                            {
                                                PrgrsprdMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));

                                                var ppData = this.context?.ProgressPeriods.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == PrgrsprdMarkingPeriodId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                                MarkingPeriodTitle = ppData!.ShortName;
                                                startDate = ppData.StartDate;
                                                endDate = ppData.EndDate;
                                            }

                                            if (markingPeriodid.First() == "2")
                                            {
                                                QtrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));

                                                var qtrData = this.context?.Quarters.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == QtrMarkingPeriodId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                                MarkingPeriodTitle = qtrData!.ShortName;
                                                startDate = qtrData.StartDate;
                                                endDate = qtrData.EndDate;
                                            }

                                            if (markingPeriodid.First() == "1")
                                            {
                                                SmstrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));

                                                var smstrData = this.context?.Semesters.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == SmstrMarkingPeriodId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                                MarkingPeriodTitle = smstrData!.ShortName;
                                                startDate = smstrData.StartDate;
                                                endDate = smstrData.EndDate;
                                            }

                                            if (markingPeriodid.First() == "0")
                                            {
                                                YrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));

                                                var yrData = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == YrMarkingPeriodId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                                MarkingPeriodTitle = yrData!.ShortName;
                                                startDate = yrData.StartDate;
                                                endDate = yrData.EndDate;
                                            }

                                            var studentDailyAttendanceCount = this.context?.StudentDailyAttendance.Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId && x.AttendanceCode == AttendanceCode.Title && x.AttendanceDate >= startDate && x.AttendanceDate <= endDate).ToList().Count;
                                            object attendanceCount = new
                                            {
                                                AttendanceCount = studentDailyAttendanceCount
                                            };
                                            AttendanceCountList.Add(attendanceCount);
                                        }
                                    }
                                    object Attendance = new
                                    {
                                        AttendanceCodeTitle = AttendanceCode.Title,
                                        AttendanceCountList
                                    };
                                    attendanceList.Add(Attendance);
                                }
                            }

                            object reportCard = new
                            {
                                SchoolData = schoolData,
                                gradeTitle = studentReportCardData != null ? studentReportCardData.FirstOrDefault()!.GradeTitle : null,
                                ReportdetailsData = reportDetailsList,
                                StudentData = studentData,
                                AttendanceCode = attendanceList,
                                CourseSectionWithGradeList=courseSectionWithGradeList,
                                GPAList= gpaList
                            };
                            reportCardList.Add(reportCard);
                        }
                    }

                    if (reportCardList != null)
                    {
                        data = new
                        {
                            TotalData = reportCardList,
                            GradeData = schoolData.GradeScale.Count > 0 ? schoolData.GradeScale.FirstOrDefault()!.Grade.ToList() : null,
                        };
                    }

                    if (String.Compare(reportCardViewModel.TemplateType, "chuuk", true) == 0)
                    {
                        GenerateChuukReportCard _report = new GenerateChuukReportCard();
                        var message = await _report.Generate(data);

                        bool isWindows = System.Runtime.InteropServices.RuntimeInformation
                                               .IsOSPlatform(OSPlatform.Windows);
                        if (isWindows)
                        {
                            using (var fileStream = new FileStream(@"ReportCard\\StudentChuukReportCard.pdf", FileMode.Open))
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    fileStream.CopyTo(memoryStream);
                                    byte[] bytes = memoryStream.ToArray();
                                    base64 = Convert.ToBase64String(bytes);
                                    fileStream.Close();
                                }
                            }
                            reportCardView.ReportCardPdf = base64;
                        }
                        else
                        {
                            using (var fileStream = new FileStream(@"ReportCard/StudentChuukReportCard.pdf", FileMode.Open))
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    fileStream.CopyTo(memoryStream);
                                    byte[] bytes = memoryStream.ToArray();
                                    base64 = Convert.ToBase64String(bytes);
                                    fileStream.Close();
                                }
                            }
                            reportCardView.ReportCardPdf = base64;
                        }
                    }

                    if (String.Compare(reportCardViewModel.TemplateType, "kosrae", true) == 0)
                    {
                        GenerateKosraeReportCard _report = new GenerateKosraeReportCard();
                        var message = await _report.Generate(data);

                        //if (message == "success")
                        //{
                        //    using (var fileStream = new FileStream(@"ReportCard\\StudentKosraeReportCard.pdf", FileMode.Open))
                        //    {
                        //        using (var memoryStream = new MemoryStream())
                        //        {
                        //            fileStream.CopyTo(memoryStream);
                        //            byte[] bytes = memoryStream.ToArray();
                        //            base64 = Convert.ToBase64String(bytes);
                        //            fileStream.Close();
                        //        }
                        //    }
                        //    reportCardView.ReportCardPdf = base64;
                        //}
                        //else
                        //{
                        //    reportCardView._message = "Problem occur!!! Please Try Again";
                        //    reportCardView._failure = true;
                        //}

                        bool isWindows = System.Runtime.InteropServices.RuntimeInformation
                                              .IsOSPlatform(OSPlatform.Windows);
                        if (isWindows)
                        {
                            using (var fileStream = new FileStream(@"ReportCard\\StudentKosraeReportCard.pdf", FileMode.Open))
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    fileStream.CopyTo(memoryStream);
                                    byte[] bytes = memoryStream.ToArray();
                                    base64 = Convert.ToBase64String(bytes);
                                    fileStream.Close();
                                }
                            }
                            reportCardView.ReportCardPdf = base64;
                        }
                        else
                        {
                            using (var fileStream = new FileStream(@"ReportCard/StudentKosraeReportCard.pdf", FileMode.Open))
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    fileStream.CopyTo(memoryStream);
                                    byte[] bytes = memoryStream.ToArray();
                                    base64 = Convert.ToBase64String(bytes);
                                    fileStream.Close();
                                }
                            }
                            reportCardView.ReportCardPdf = base64;
                        }
                    }

                    if (String.Compare(reportCardViewModel.TemplateType, "pohnpei", true) == 0)
                    {
                        GeneratePohnpeiReportCard _report = new GeneratePohnpeiReportCard();
                        var message = await _report.Generate(data);

                        bool isWindows = System.Runtime.InteropServices.RuntimeInformation
                                             .IsOSPlatform(OSPlatform.Windows);
                        if (isWindows)
                        {
                            using (var fileStream = new FileStream(@"ReportCard\\StudentPohnpeiReportCard.pdf", FileMode.Open))
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    fileStream.CopyTo(memoryStream);
                                    byte[] bytes = memoryStream.ToArray();
                                    base64 = Convert.ToBase64String(bytes);
                                    fileStream.Close();
                                }
                            }
                            reportCardView.ReportCardPdf = base64;
                        }
                        else
                        {
                            using (var fileStream = new FileStream(@"ReportCard/StudentPohnpeiReportCard.pdf", FileMode.Open))
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    fileStream.CopyTo(memoryStream);
                                    byte[] bytes = memoryStream.ToArray();
                                    base64 = Convert.ToBase64String(bytes);
                                    fileStream.Close();
                                }
                            }
                            reportCardView.ReportCardPdf = base64;
                        }
                    }

                    if (String.Compare(reportCardViewModel.TemplateType, "yap", true) == 0)
                    {
                        GenerateYapReportCard _report = new GenerateYapReportCard();
                        var message = await _report.Generate(data);

                        bool isWindows = System.Runtime.InteropServices.RuntimeInformation
                                            .IsOSPlatform(OSPlatform.Windows);
                        if (isWindows)
                        {
                            using (var fileStream = new FileStream(@"ReportCard\\StudentYapReportCard.pdf", FileMode.Open))
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    fileStream.CopyTo(memoryStream);
                                    byte[] bytes = memoryStream.ToArray();
                                    base64 = Convert.ToBase64String(bytes);
                                    fileStream.Close();
                                }
                            }
                            reportCardView.ReportCardPdf = base64;
                        }
                        else
                        {
                            using (var fileStream = new FileStream(@"ReportCard/StudentYapReportCard.pdf", FileMode.Open))
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    fileStream.CopyTo(memoryStream);
                                    byte[] bytes = memoryStream.ToArray();
                                    base64 = Convert.ToBase64String(bytes);
                                    fileStream.Close();
                                }
                            }
                            reportCardView.ReportCardPdf = base64;

                        }
                    }
                }
            }
            catch (Exception es)
            {
                reportCardView._message = es.Message;
                reportCardView._failure = true;
            }
            return reportCardView;
        }

        /// <summary>
        /// Get Report Card For Students
        /// </summary>
        /// <param name="reportCardViewModel"></param>
        /// <returns></returns>
        public ReportCardViewModel GetReportCardForStudents(ReportCardViewModel reportCardViewModel)
        {
            ReportCardViewModel reportCardView = new ReportCardViewModel();
            reportCardView._tenantName = reportCardViewModel._tenantName;
            reportCardView._token = reportCardViewModel._token;
            reportCardView.TenantId = reportCardViewModel.TenantId;
            reportCardView.SchoolId = reportCardViewModel.SchoolId;
            reportCardView.AcademicYear = reportCardViewModel.AcademicYear;
            reportCardView.TemplateType = reportCardViewModel.TemplateType;
            try
            {
                if (reportCardViewModel.studentsReportCardViewModelList.Count > 0)
                {
                    string? schoolYear = null;
                    var gradeDataList = new List<Grade>();

                    var schoolData = this.context?.SchoolMaster.Include(x => x.SchoolDetail).Include(x => x.SchoolYears).Include(x => x.GradeScale).ThenInclude(x => x.Grade).FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId);

                    if (schoolData != null)
                    {
                        var schoolYearData = schoolData.SchoolYears.FirstOrDefault(x => x.AcademicYear == reportCardViewModel.AcademicYear);
                        if (schoolYearData != null)
                        {
                            schoolYear = schoolYearData.StartDate!.Value.Year + "-" + schoolYearData.EndDate!.Value.Year;
                        }

                        //this block for fetch main grade
                        var gradeScaleData = schoolData.GradeScale.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.AcademicYear == reportCardViewModel.AcademicYear && x.UseAsStandardGradeScale != true);

                        if (gradeScaleData != null)
                        {
                            gradeDataList = gradeScaleData.Grade.OrderByDescending(s => s.Breakoff).Select(s => new Grade { GradeScaleId = s.GradeScaleId, GradeId = s.GradeId, Breakoff = s.Breakoff, Title = s.Title, UnweightedGpValue = s.UnweightedGpValue != null ? s.UnweightedGpValue : 0, WeightedGpValue = s.WeightedGpValue != null ? s.WeightedGpValue : 0, Comment = s.Comment }).ToList();
                        }
                    }

                    var studentMasterData = this.context?.StudentMaster.Include(x => x.StudentEnrollment).Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId).ToList();

                    var studentAttendanceMasterData = this.context?.StudentAttendance.Include(x => x.AttendanceCodeNavigation).Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId).ToList();

                    var EffortGradeLibraryCategoryData = this.context?.EffortGradeLibraryCategory.Include(s => s.EffortGradeLibraryCategoryItem).Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId).ToList();

                    if (String.Compare(reportCardViewModel.TemplateType, "default", true) == 0)
                    {
                        var courseCommentCategoryData = this.context?.CourseCommentCategory.Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId).Select(s => new CourseCommentCategory { TenantId = s.TenantId, SchoolId = s.SchoolId, CourseCommentId = s.CourseCommentId, Comments = s.Comments }).ToList();

                        //this loop for students
                        foreach (var student in reportCardViewModel.studentsReportCardViewModelList)
                        {
                            StudentsReportCardViewModel studentsReportCard = new StudentsReportCardViewModel();
                            List<string> teacherComments = new List<string>();
                            int teacherCommentsNo = 1;
                            int? absencesInDays = 0;
                            List<DateTime> holidayList = new List<DateTime>();
                            List<EffortGradeDetailsViewModel> effortGradeDetailList = new List<EffortGradeDetailsViewModel>();
                            List<StanderdsGradeDetailsViewModel> standerdsGradeDetailsList = new List<StanderdsGradeDetailsViewModel>();
                            List<StanderdsGradeDetailsViewModel> MarkingPeriodList = new List<StanderdsGradeDetailsViewModel>();

                            var studentData = studentMasterData!.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId);

                            var GradeLevelTitle = studentData!.StudentEnrollment.Where(x => x.IsActive == true).Select(s => s.GradeLevelTitle).FirstOrDefault();

                            studentsReportCard.SchoolName = schoolData!.SchoolName;
                            studentsReportCard.SchoolYear = schoolYear;
                            studentsReportCard.FirstGivenName = studentData.FirstGivenName;
                            studentsReportCard.MiddleName = studentData.MiddleName;
                            studentsReportCard.LastFamilyName = studentData.LastFamilyName;
                            studentsReportCard.StudentInternalId = studentData.StudentInternalId;
                            studentsReportCard.GradeTitle = GradeLevelTitle;
                            studentsReportCard.HomeAddressLineOne = schoolData.StreetAddress1;
                            studentsReportCard.HomeAddressLineTwo = schoolData.StreetAddress2;
                            studentsReportCard.HomeAddressCountry = schoolData.Country;
                            studentsReportCard.HomeAddressState = schoolData.State;
                            studentsReportCard.HomeAddressCity = schoolData.City;
                            studentsReportCard.HomeAddressZip = schoolData.Zip;

                            var markingPeriodsData = reportCardViewModel.MarkingPeriods!.Split(",");
                            DateTime? startDate = null;
                            DateTime? endDate = null;
                            string? MarkingPeriodTitle = null;
                            string? MarkingPeriodShortTitle = null;
                            string? SortId = null;

                            //this loop for marking periods
                            foreach (var markingPeriod in markingPeriodsData)
                            {
                                MarkingPeriodDetailsForDefaultTemplate markingPeriodDetailsForDefaultTemplates = new MarkingPeriodDetailsForDefaultTemplate();
                                StanderdsGradeDetailsViewModel MarkingPeriod = new();

                                int? Absences = 0;
                                int? ExcusedAbsences = 0;
                                int? QtrMarkingPeriodId = null;
                                int? SmstrMarkingPeriodId = null;
                                int? YrMarkingPeriodId = null;
                                int? PrgrsprdMarkingPeriodId = null;
                                bool? Exam = null;
                                bool? IsCustomDateRange = null;

                                if (markingPeriod != null)
                                {
                                    var markingPeriodid = markingPeriod.Split("_", StringSplitOptions.RemoveEmptyEntries);

                                    //now find the marking period id
                                    if (markingPeriodid.First() == "3")
                                    {
                                        PrgrsprdMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));

                                        var ppData = this.context?.ProgressPeriods.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == PrgrsprdMarkingPeriodId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                        MarkingPeriodTitle = ppData!.Title;
                                        MarkingPeriodShortTitle = ppData!.ShortName;
                                        startDate = ppData.StartDate;
                                        endDate = ppData.EndDate;
                                        MarkingPeriod.MarkingPeriodName = MarkingPeriodShortTitle;
                                        SortId = "3_" + ppData.MarkingPeriodId;
                                        MarkingPeriod.SortId = SortId;

                                        if (markingPeriodid.Last() == "E")
                                        {
                                            Exam = true;
                                            MarkingPeriodTitle = ppData!.Title + " Exam";
                                            MarkingPeriodShortTitle = ppData!.ShortName + " Exam";
                                            MarkingPeriod.MarkingPeriodName = MarkingPeriodShortTitle;
                                        }
                                        MarkingPeriodList.Add(MarkingPeriod);
                                    }

                                    else if (markingPeriodid.First() == "2")
                                    {
                                        QtrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));

                                        var qtrData = this.context?.Quarters.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == QtrMarkingPeriodId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                        MarkingPeriodTitle = qtrData!.Title;
                                        MarkingPeriodShortTitle = qtrData!.ShortName;
                                        startDate = qtrData.StartDate;
                                        endDate = qtrData.EndDate;
                                        MarkingPeriod.MarkingPeriodName = MarkingPeriodShortTitle;
                                        SortId = "2_" + qtrData.MarkingPeriodId;
                                        MarkingPeriod.SortId = SortId;

                                        if (markingPeriodid.Last() == "E")
                                        {
                                            Exam = true;
                                            MarkingPeriodTitle = qtrData!.Title + " Exam";
                                            MarkingPeriodShortTitle = qtrData!.ShortName + " Exam";
                                            MarkingPeriod.MarkingPeriodName = MarkingPeriodShortTitle;
                                        }
                                        MarkingPeriodList.Add(MarkingPeriod);
                                    }

                                    else if (markingPeriodid.First() == "1")
                                    {
                                        SmstrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));

                                        var smstrData = this.context?.Semesters.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == SmstrMarkingPeriodId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                        MarkingPeriodTitle = smstrData!.Title;
                                        MarkingPeriodShortTitle = smstrData!.ShortName;
                                        startDate = smstrData.StartDate;
                                        endDate = smstrData.EndDate;
                                        MarkingPeriod.MarkingPeriodName = MarkingPeriodShortTitle;
                                        SortId = "1_" + smstrData.MarkingPeriodId;
                                        MarkingPeriod.SortId = SortId;

                                        if (markingPeriodid.Last() == "E")
                                        {
                                            Exam = true;
                                            MarkingPeriodTitle = smstrData!.Title + " Exam";
                                            MarkingPeriodShortTitle = smstrData!.ShortName + " Exam";
                                            MarkingPeriod.MarkingPeriodName = MarkingPeriodShortTitle;
                                        }
                                        MarkingPeriodList.Add(MarkingPeriod);
                                    }

                                    else if (markingPeriodid.First() == "0")
                                    {
                                        YrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));

                                        var yrData = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == YrMarkingPeriodId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                        MarkingPeriodTitle = yrData!.Title;
                                        MarkingPeriodShortTitle = yrData!.ShortName;
                                        startDate = yrData.StartDate;
                                        endDate = yrData.EndDate;
                                        MarkingPeriod.MarkingPeriodName = MarkingPeriodShortTitle;
                                        SortId = "0_" + yrData.MarkingPeriodId;
                                        MarkingPeriod.SortId = SortId;

                                        if (markingPeriodid.Last() == "E")
                                        {
                                            Exam = true;
                                            MarkingPeriodTitle = yrData!.Title + " Exam";
                                            MarkingPeriodShortTitle = yrData!.ShortName + " Exam";
                                            MarkingPeriod.MarkingPeriodName = MarkingPeriodShortTitle;
                                        }
                                        MarkingPeriodList.Add(MarkingPeriod);
                                    }

                                    else if (markingPeriodid.First().ToLower() == "custom")
                                    {
                                        IsCustomDateRange = true;
                                        MarkingPeriodTitle = "Custom Date Range";
                                        MarkingPeriodShortTitle = "Custom";
                                        SortId = "5";
                                        MarkingPeriod.SortId = SortId;
                                        MarkingPeriod.MarkingPeriodName = MarkingPeriodShortTitle;
                                        MarkingPeriodList.Add(MarkingPeriod);
                                    }


                                    markingPeriodDetailsForDefaultTemplates.MarkingPeriodTitle = MarkingPeriodTitle;

                                    var studentAttendanceData = studentAttendanceMasterData!.Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId && x.AttendanceDate >= startDate && x.AttendanceDate <= endDate).ToList();

                                    if (studentAttendanceData.Count > 0)
                                    {
                                        Absences = studentAttendanceData.Where(x => x.AttendanceCodeNavigation.StateCode.ToLower() == "absent").Count();
                                        ExcusedAbsences = studentAttendanceData.Where(x => x.AttendanceCodeNavigation.StateCode.ToLower() == "excusedabsent").Count();
                                        absencesInDays += Absences + ExcusedAbsences;
                                    }
                                    var reportCardData = new List<StudentFinalGrade>();

                                    if (IsCustomDateRange == true)
                                    {
                                        reportCardData = this.context?.StudentFinalGrade.Include(x => x.StudentFinalGradeComments).Include(x => x.StudentFinalGradeStandard).Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId && x.AcademicYear == reportCardViewModel.AcademicYear && x.IsCustomMarkingPeriod == true).ToList();
                                    }
                                    else
                                    {
                                        if (Exam == true)
                                        {
                                            reportCardData = this.context?.StudentFinalGrade.Include(x => x.StudentFinalGradeComments).Include(x => x.StudentFinalGradeStandard).Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId && x.AcademicYear == reportCardViewModel.AcademicYear && ((x.YrMarkingPeriodId != null && x.YrMarkingPeriodId == YrMarkingPeriodId) || (x.SmstrMarkingPeriodId != null && x.SmstrMarkingPeriodId == SmstrMarkingPeriodId) || (x.QtrMarkingPeriodId != null && x.QtrMarkingPeriodId == QtrMarkingPeriodId) || (x.PrgrsprdMarkingPeriodId != null && x.PrgrsprdMarkingPeriodId == PrgrsprdMarkingPeriodId)) && x.IsExamGrade == true).ToList();
                                        }
                                        else
                                        {
                                            reportCardData = this.context?.StudentFinalGrade.Include(x => x.StudentFinalGradeComments).Include(x => x.StudentFinalGradeStandard).Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId && x.AcademicYear == reportCardViewModel.AcademicYear && ((x.YrMarkingPeriodId != null && x.YrMarkingPeriodId == YrMarkingPeriodId) || (x.SmstrMarkingPeriodId != null && x.SmstrMarkingPeriodId == SmstrMarkingPeriodId) || (x.QtrMarkingPeriodId != null && x.QtrMarkingPeriodId == QtrMarkingPeriodId) || (x.PrgrsprdMarkingPeriodId != null && x.PrgrsprdMarkingPeriodId == PrgrsprdMarkingPeriodId)) && x.IsExamGrade != true).ToList();
                                        }
                                    }

                                    decimal? gPaValue = 0.0m;
                                    decimal? CreditEarned = 0.0m;
                                    decimal? CreditHours = 0.0m;
                                    decimal? GradePoint = 0.0m;

                                    if (reportCardData?.Any() == true)
                                    {
                                        foreach (var reportCard in reportCardData)
                                        {
                                            CourseSectionGradeDetailsForDefaultTemplate courseSectionGradeDetailsForDefaultTemplate = new CourseSectionGradeDetailsForDefaultTemplate();

                                            var CourseSectionData = this.context?.CourseSection.Include(x => x.Course).Include(x => x.GradeScale).Include(x => x.StaffCoursesectionSchedule.Where(s => s.IsDropped != true && s.IsPrimaryStaff == true)).ThenInclude(x => x.StaffMaster).FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.CourseSectionId == reportCard.CourseSectionId && x.CourseId == reportCard.CourseId);

                                            if (CourseSectionData?.GradeScale != null)
                                            {
                                                var gradeData = CourseSectionData?.GradeScale?.Grade.AsEnumerable().Where(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && String.Compare(x.Title, reportCard.GradeObtained, true) == 0 && x.GradeScaleId == reportCard.GradeScaleId).FirstOrDefault();

                                                if (gradeData != null)
                                                {
                                                    if (gradeData.WeightedGpValue == null)
                                                    {
                                                        gradeData.WeightedGpValue = 0;
                                                    }
                                                    if (gradeData.UnweightedGpValue == null)
                                                    {
                                                        gradeData.UnweightedGpValue = 0;
                                                    }
                                                    CreditEarned = reportCard.CreditEarned;
                                                    GradePoint = CourseSectionData?.IsWeightedCourse != true ? (gradeData.UnweightedGpValue * CreditEarned) : gradeData.WeightedGpValue * CreditEarned;
                                                    gPaValue = CreditEarned > 0 && GradePoint > 0 ? (GradePoint / CreditEarned) : 0;
                                                }
                                            }
                                            else if (CourseSectionData?.GradeScaleType == "Teacher_Scale")
                                            {
                                                var GradebookConfigurationGrade = this.context?.GradebookConfigurationGradescale.Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.CourseSectionId == reportCard.CourseSectionId && x.AcademicYear == reportCardViewModel.AcademicYear).OrderByDescending(s => s.BreakoffPoints).ToList();
                                                if (GradebookConfigurationGrade != null)
                                                {
                                                    var ConfigurationGrade = GradebookConfigurationGrade.FirstOrDefault(x => x.BreakoffPoints <= reportCard.PercentMarks);
                                                    var gradeData = gradeDataList?.FirstOrDefault(x => x.GradeId == ConfigurationGrade?.GradeId && x.GradeScaleId == ConfigurationGrade.GradeScaleId);
                                                    if (gradeData != null)
                                                    {
                                                        CreditEarned = reportCard.CreditEarned;
                                                        GradePoint = CourseSectionData?.IsWeightedCourse != true ? (gradeData.UnweightedGpValue * CreditEarned) : gradeData.WeightedGpValue * CreditEarned;
                                                        gPaValue = CreditEarned > 0 && GradePoint > 0 ? (GradePoint / CreditEarned) : 0;
                                                    }
                                                }
                                            }
                                            else if (CourseSectionData?.GradeScaleType == "Numeric")
                                            {
                                                courseSectionGradeDetailsForDefaultTemplate.GradeObtained = reportCard.PercentMarks.ToString();
                                                courseSectionGradeDetailsForDefaultTemplate.PercentMarks = null;
                                                //    var gradeData = gradeDataList?.FirstOrDefault(x => x.Breakoff <= reportCard.PercentMarks);

                                                //    if (gradeData != null)
                                                //    {
                                                //        CreditEarned = reportCard.CreditEarned;
                                                //        GradePoint = CourseSectionData?.IsWeightedCourse != true ? (gradeData.UnweightedGpValue * CreditEarned) : gradeData.WeightedGpValue * CreditEarned;
                                                //        gPaValue = CreditEarned > 0 && GradePoint > 0 ? (GradePoint / CreditEarned) : 0;
                                                //    }

                                            }

                                            var comments = reportCard.StudentFinalGradeComments.Select(x => x.CourseCommentId).ToList();
                                            var Comments = string.Join(",", comments.Select(x => x.ToString()).ToArray());

                                            courseSectionGradeDetailsForDefaultTemplate.CourseSectionName = CourseSectionData?.CourseSectionName;
                                            courseSectionGradeDetailsForDefaultTemplate.StaffName = CourseSectionData?.StaffCoursesectionSchedule.Count > 0 ? CourseSectionData?.StaffCoursesectionSchedule.FirstOrDefault()!.StaffMaster.FirstGivenName + " " + CourseSectionData?.StaffCoursesectionSchedule.FirstOrDefault()!.StaffMaster.MiddleName + " " + CourseSectionData?.StaffCoursesectionSchedule.FirstOrDefault()!.StaffMaster.LastFamilyName : null;
                                            courseSectionGradeDetailsForDefaultTemplate.GradeObtained = reportCard.GradeObtained;
                                            courseSectionGradeDetailsForDefaultTemplate.PercentMarks = reportCard.PercentMarks;
                                            courseSectionGradeDetailsForDefaultTemplate.GPA = gPaValue;
                                            courseSectionGradeDetailsForDefaultTemplate.Comments = Comments;

                                            if (reportCard.TeacherComment != null)
                                            {
                                                courseSectionGradeDetailsForDefaultTemplate.TeacherComments = teacherCommentsNo++.ToString();
                                                teacherComments.Add(reportCard.TeacherComment);
                                            }
                                            markingPeriodDetailsForDefaultTemplates.courseSectionGradeDetailsForDefaultTemplates.Add(courseSectionGradeDetailsForDefaultTemplate);

                                            //this block for StandardGrade
                                            if (reportCardViewModel.StandardGrade == true)
                                            {
                                                var StudentFinalGradeStandardData = reportCard.StudentFinalGradeStandard.Where(s => s.StandardGradeScaleId > 0 && s.GradeObtained > 0);
                                                if (StudentFinalGradeStandardData?.Any() == true)
                                                {
                                                    foreach (var finalGradeStandardData in StudentFinalGradeStandardData)
                                                    {
                                                        StanderdsGradeDetailsViewModel standerdsGradeDetails = new();
                                                        standerdsGradeDetails.CourseSectionName = CourseSectionData?.CourseSectionName;
                                                        standerdsGradeDetails.StaffName = CourseSectionData?.StaffCoursesectionSchedule.Count > 0 ? CourseSectionData?.StaffCoursesectionSchedule.FirstOrDefault()!.StaffMaster.FirstGivenName + " " + CourseSectionData?.StaffCoursesectionSchedule.FirstOrDefault()!.StaffMaster.MiddleName + " " + CourseSectionData?.StaffCoursesectionSchedule.FirstOrDefault()!.StaffMaster.LastFamilyName : null;
                                                        standerdsGradeDetails.MarkingPeriodName = MarkingPeriodShortTitle;
                                                        standerdsGradeDetails.SortId = SortId;

                                                        var gradeUsStandardData = this.context?.GradeUsStandard.FirstOrDefault(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && x.GradeStandardId == finalGradeStandardData.StandardGradeScaleId);

                                                        standerdsGradeDetails.StandardRefNo = gradeUsStandardData?.StandardRefNo;
                                                        standerdsGradeDetails.StandardDetails = gradeUsStandardData?.StandardDetails;
                                                        standerdsGradeDetails.value = this.context?.Grade.FirstOrDefault(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && x.GradeId == finalGradeStandardData.GradeObtained)?.Title;

                                                        standerdsGradeDetailsList.Add(standerdsGradeDetails);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    markingPeriodDetailsForDefaultTemplates.Absences = Absences;
                                    markingPeriodDetailsForDefaultTemplates.ExcusedAbsences = ExcusedAbsences;
                                    //studentsReportCard.YearToDateAttendencePercent = Math.Round(attendencePercent, 2).ToString() + "%";
                                    //studentsReportCard.YearToDateAbsencesInDays = absencesInDays;
                                    //studentsReportCard.teacherCommentList = teacherComments;
                                    //studentsReportCard.courseCommentCategories = courseCommentCategoryData!;

                                    studentsReportCard.markingPeriodDetailsForDefaultTemplates.Add(markingPeriodDetailsForDefaultTemplates);

                                    //this block for effot grade
                                    if (reportCardViewModel.EffortGrade == true)
                                    {
                                        if (Exam != true)  //becz effort grade are not given for exam
                                        {
                                            var effortGradeData = this.context?.StudentEffortGradeMaster.Include(x => x.StudentEffortGradeDetail).Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId && x.AcademicYear == reportCardViewModel.AcademicYear && ((x.YrMarkingPeriodId != null && x.YrMarkingPeriodId == YrMarkingPeriodId) || (x.SmstrMarkingPeriodId != null && x.SmstrMarkingPeriodId == SmstrMarkingPeriodId) || (x.QtrMarkingPeriodId != null && x.QtrMarkingPeriodId == QtrMarkingPeriodId) || (x.PrgrsprdMarkingPeriodId != null && x.PrgrsprdMarkingPeriodId == PrgrsprdMarkingPeriodId))).FirstOrDefault();

                                            if (effortGradeData != null && EffortGradeLibraryCategoryData?.Any() == true)
                                            {
                                                effortGradeData.StudentEffortGradeDetail = effortGradeData.StudentEffortGradeDetail.Where(s => s.EffortCategoryId != null).ToList();

                                                var effortGradeDetails = effortGradeData.StudentEffortGradeDetail.Select(s => new EffortGradeDetailsViewModel
                                                {
                                                    CategoryName = EffortGradeLibraryCategoryData.FirstOrDefault(x => x.EffortCategoryId == s.EffortCategoryId)?.CategoryName,
                                                    MarkingPeriodName = MarkingPeriodShortTitle,
                                                    SortId = SortId,
                                                    EffortItemTitle = EffortGradeLibraryCategoryData.SelectMany(x => x.EffortGradeLibraryCategoryItem).FirstOrDefault(x => x.EffortCategoryId == s.EffortCategoryId && x.EffortItemId == s.EffortItemId)?.EffortItemTitle,
                                                    GradeScaleValue = s.EffortGradeScaleId
                                                });

                                                effortGradeDetailList.AddRange(effortGradeDetails);
                                            }
                                        }
                                    }
                                }
                            }

                            //this code for year to date percentage calculation
                            int workDays = 0;
                            decimal attendencePercent = 0;

                            var calenderData = this.context?.SchoolCalendars.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.DefaultCalender == true && x.AcademicYear == reportCardViewModel.AcademicYear);

                            var schoolYearData = schoolData.SchoolYears.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.AcademicYear == reportCardViewModel.AcademicYear);

                            if (calenderData != null && schoolYearData != null)
                            {
                                DateTime? schoolYearStartDate = schoolYearData.StartDate;
                                DateTime? schoolYearEndDate = schoolYearData.EndDate;

                                // Calculate Holiday
                                var CalendarEventsData = this.context?.CalendarEvents.Where(e => e.TenantId == reportCardViewModel.TenantId && e.AcademicYear == reportCardViewModel.AcademicYear && (e.StartDate >= schoolYearStartDate && e.StartDate <= schoolYearEndDate || e.EndDate >= schoolYearStartDate && e.EndDate <= schoolYearEndDate) && e.IsHoliday == true && (e.SchoolId == reportCardViewModel.SchoolId || e.ApplicableToAllSchool == true)).ToList();

                                if (CalendarEventsData?.Any() == true)
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

                                //var daysValue = "0123456";
                                //var weekdays = calenderData.Days;
                                //var WeekOffDays = Regex.Split(daysValue, weekdays!);
                                //var WeekOfflist = new List<string>();

                                //foreach (var WeekOffDay in WeekOffDays)
                                //{
                                //    Days days = new Days();
                                //    var Day = Enum.GetName(days.GetType(), Convert.ToInt32(WeekOffDay));
                                //    WeekOfflist.Add(Day!);
                                //}

                                //List<DateTime> allDates = new List<DateTime>();
                                //for (DateTime date = (DateTime)schoolYearStartDate!; date <= schoolYearEndDate; date = date.AddDays(1))
                                //    allDates.Add(date);

                                ////remove holidays & weekofdays
                                //foreach (var date in allDates)
                                //{
                                //    var Day = date.DayOfWeek.ToString();
                                //    if (!WeekOfflist.Contains(Day) && !holidayList.Contains(date))
                                //    {
                                //        workDays++;
                                //    }
                                //}

                                //fetch calender days & weekoff days
                                List<char> daysValue = new List<char> { '0', '1', '2', '3', '4', '5', '6' };
                                var calenderDays = calenderData.Days!.ToCharArray();
                                var WeekOffDays = daysValue.Except(calenderDays);
                                var WeekOfflist = new List<string>();
                                foreach (var WeekOffDay in WeekOffDays)
                                {
                                    Days days = new Days();
                                    var Day = Enum.GetName(days.GetType(), Convert.ToInt32(WeekOffDay.ToString()));
                                    WeekOfflist.Add(Day!);
                                }

                                //fetch all dates in this session calender
                                var allDates = Enumerable.Range(0, 1 + schoolYearEndDate!.Value.Date.Subtract(schoolYearStartDate!.Value.Date).Days).Select(d => schoolYearStartDate.Value.Date.AddDays(d)).ToList();

                                //remove holidays &weekoffdays
                                var wrokingDates = allDates.Where(s => !holidayList.Contains(s.Date) && !WeekOfflist.Contains(s.Date.DayOfWeek.ToString())).ToList();

                                workDays = wrokingDates.Count;

                                var studentAttendance = this.context?.StudentAttendance.Include(s => s.AttendanceCodeNavigation).Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId && x.AttendanceDate >= schoolYearStartDate && x.AttendanceDate <= schoolYearEndDate).ToList();

                                if (studentAttendance?.Count > 0 && workDays > 0)
                                {
                                    var studentDailyAttendanceCount = studentAttendance.Where(x => x.AttendanceCodeNavigation.StateCode.ToLower() == "present").GroupBy(c => new
                                    {
                                        c.StudentId,
                                        c.AttendanceDate
                                    }).Count();

                                    attendencePercent = ((Convert.ToDecimal(studentDailyAttendanceCount) / Convert.ToDecimal(workDays)) * 100);
                                }
                            }

                            studentsReportCard.YearToDateAttendencePercent = Math.Round(attendencePercent, 2).ToString() + "%";
                            studentsReportCard.YearToDateAbsencesInDays = absencesInDays;
                            studentsReportCard.teacherCommentList = teacherComments;
                            studentsReportCard.courseCommentCategories = courseCommentCategoryData!;


                            //this block for standerd grade
                            if (reportCardViewModel.StandardGrade == true)
                            {
                                var CSbygroupData = standerdsGradeDetailsList.GroupBy(G => G.CourseSectionName).ToList();

                                foreach (var csData in CSbygroupData)
                                {
                                    StanderdsGradeDetailsViewModel standerdsGradeDetails = new();
                                    standerdsGradeDetails.CourseSectionName = csData.Key;
                                    standerdsGradeDetails.StaffName = csData.First().StaffName;

                                    var standerdData = csData.GroupBy(x => x.StandardRefNo).ToList();
                                    foreach (var standerd in standerdData)
                                    {
                                        StanderdsGradeDetails standerdsGrade = new();

                                        standerdsGrade.StandardRefNo = standerd.Key;
                                        standerdsGrade.StandardDetails = standerd.First().StandardDetails;
                                        standerdsGrade.markingPeriodDetailsforStanderdsGrades = standerd.Select(s => new MarkingPeriodDetailsforStanderdsGrade { MarkingPeriodName = s.MarkingPeriodName, value = s.value, SortId = s.SortId }).ToList();

                                        var exceptMarkingPeriod = MarkingPeriodList.Select(x => new { x.MarkingPeriodName, x.SortId }).Except(standerdsGrade.markingPeriodDetailsforStanderdsGrades.Select(x => new { x.MarkingPeriodName, x.SortId })).Select(s => new MarkingPeriodDetailsforStanderdsGrade { MarkingPeriodName = s.MarkingPeriodName, SortId = s.SortId }).ToList();

                                        standerdsGrade.markingPeriodDetailsforStanderdsGrades.AddRange(exceptMarkingPeriod);
                                        standerdsGrade.markingPeriodDetailsforStanderdsGrades = standerdsGrade.markingPeriodDetailsforStanderdsGrades.OrderBy(s => s.SortId).ThenBy(s => s.MarkingPeriodName).ToList();
                                        standerdsGradeDetails.standerdsGradeDetails.Add(standerdsGrade);

                                    }
                                    studentsReportCard.standerdsGradeList.Add(standerdsGradeDetails);
                                }

                                var standerdGradeScale = this.context?.GradeScale.Include(x => x.Grade).FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.UseAsStandardGradeScale == true);

                                if (standerdGradeScale != null)
                                {
                                    studentsReportCard.standerdGradeScale = standerdGradeScale.Grade.Select(s => new Grade { Title = s.Title, Comment = s.Comment, GradeId = s.GradeId }).ToList();
                                }
                            }

                            //this block for effort grade
                            if (reportCardViewModel.EffortGrade == true)
                            {
                                var Categorydata = effortGradeDetailList.GroupBy(g => g.CategoryName).ToList();
                                foreach (var category in Categorydata)
                                {
                                    EffortGradeDetailsViewModel effortGradeDetailsViewModel = new();
                                    var itemData = category.GroupBy(g => g.EffortItemTitle).ToList();

                                    effortGradeDetailsViewModel.CategoryName = category.Key;
                                    foreach (var item in itemData)
                                    {
                                        EffortGradeItemDetails effortGradeItemDetails = new();

                                        effortGradeItemDetails.EffortItemTitle = item.Key;
                                        effortGradeItemDetails.markingPeriodDetailsforEffortGrades = item.Select(s => new MarkingPeriodDetailsforEffortGrade { MarkingPeriodName = s.MarkingPeriodName, GradeScaleValue = s.GradeScaleValue, SortId = s.SortId }).ToList();

                                        var index = MarkingPeriodList.FindIndex(s => s.MarkingPeriodName.ToLower() == "custom"); //effort does't generate for custom
                                        if (index > -1)
                                        {
                                            MarkingPeriodList.RemoveAt(index);
                                        }

                                        var exceptMarkingPeriod = MarkingPeriodList.Select(x => new { x.MarkingPeriodName, x.SortId }).Except(effortGradeItemDetails.markingPeriodDetailsforEffortGrades.Select(x => new { x.MarkingPeriodName, x.SortId })).Select(s => new MarkingPeriodDetailsforEffortGrade { MarkingPeriodName = s.MarkingPeriodName, SortId = s.SortId }).ToList();

                                        effortGradeItemDetails.markingPeriodDetailsforEffortGrades.AddRange(exceptMarkingPeriod);
                                        effortGradeItemDetails.markingPeriodDetailsforEffortGrades = effortGradeItemDetails.markingPeriodDetailsforEffortGrades.OrderBy(s => s.SortId).ThenBy(s => s.MarkingPeriodName).ToList();

                                        effortGradeDetailsViewModel.effortGradeItemDetails.Add(effortGradeItemDetails);

                                    }
                                    studentsReportCard.effortGradeList.Add(effortGradeDetailsViewModel);
                                }

                                var effortGradeScale = this.context?.EffortGradeScale.Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId).Select(s => new EffortGradeScale { GradeScaleValue = s.GradeScaleValue, GradeScaleComment = s.GradeScaleComment }).ToList();
                                if (effortGradeScale?.Any() == true)
                                {
                                    studentsReportCard.effortGradeScales = effortGradeScale;
                                }
                            }
                            reportCardView.studentsReportCardViewModelList.Add(studentsReportCard);
                        }
                    }

                    else if (String.Compare(reportCardViewModel.TemplateType, "rmi", true) == 0)
                    {
                        List<string?> Subjects = new List<string?> { "math", "science", "social studies", "health", "marshallese", "english" };

                        var BlockData = this.context?.Block.Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId);

                        foreach (var student in reportCardViewModel.studentsReportCardViewModelList)
                        {
                            StudentsReportCardViewModel studentsReportCard = new StudentsReportCardViewModel();
                            List<int?> QtrMarkingPeriodId = new List<int?>();
                            List<int?> SmstrMarkingPeriodId = new List<int?>();
                            List<int?> YrMarkingPeriodId = new List<int?>();
                            List<int?> PrgrsprdMarkingPeriodId = new List<int?>();
                            List<int?> QtrMarkingPeriodIdExam = new List<int?>();
                            List<int?> SmstrMarkingPeriodIdExam = new List<int?>();
                            List<int?> YrMarkingPeriodIdExam = new List<int?>();
                            List<int?> PrgrsprdMarkingPeriodIdExam = new List<int?>();
                            List<MarkingPeriodDetailsViewforRMIReport> markingPeriodList = new List<MarkingPeriodDetailsViewforRMIReport>();

                            var StudentDailyAttendanceData = this.context?.StudentDailyAttendance.Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId);

                            var AttendanceData = this.context?.StudentAttendance.Include(s => s.BlockPeriod).Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId);

                            var studentData = studentMasterData!.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId);

                            var GradeLevelTitle = studentData!.StudentEnrollment.Where(x => x.IsActive == true).Select(s => s.GradeLevelTitle).FirstOrDefault();

                            studentsReportCard.SchoolName = schoolData!.SchoolName;
                            studentsReportCard.SchoolLogo = schoolData!.SchoolDetail.FirstOrDefault()!.SchoolThumbnailLogo;
                            studentsReportCard.SchoolYear = schoolYear;
                            studentsReportCard.HomeAddressLineOne = schoolData.StreetAddress1;
                            studentsReportCard.HomeAddressLineTwo = schoolData.StreetAddress2;
                            studentsReportCard.HomeAddressCountry = schoolData.Country;
                            studentsReportCard.HomeAddressState = schoolData.State;
                            studentsReportCard.HomeAddressCity = schoolData.City;
                            studentsReportCard.HomeAddressZip = schoolData.Zip;
                            studentsReportCard.FirstGivenName = studentData.FirstGivenName;
                            studentsReportCard.MiddleName = studentData.MiddleName;
                            studentsReportCard.LastFamilyName = studentData.LastFamilyName;
                            studentsReportCard.StudentInternalId = studentData.StudentInternalId;
                            studentsReportCard.GradeTitle = GradeLevelTitle;

                            if (schoolData.GradeScale.Count > 0)
                            {
                                var gradeScaledata = schoolData.GradeScale.FirstOrDefault(x => x.UseAsStandardGradeScale != true);

                                if (gradeScaledata != null && gradeScaledata?.Grade.Count > 0)
                                {
                                    gradeScaledata?.Grade.ToList().ForEach(x => x.GradeScale = new());
                                    studentsReportCard.gradeList = gradeScaledata!.Grade.ToList();
                                }
                            }

                            var markingPeriodsData = reportCardViewModel.MarkingPeriods!.Split(",");

                            foreach (var markingPeriod in markingPeriodsData)
                            {
                                MarkingPeriodDetailsViewforRMIReport markingPeriodListView = new MarkingPeriodDetailsViewforRMIReport();
                                if (markingPeriod != null)
                                {
                                    var markingPeriodid = markingPeriod.Split("_", StringSplitOptions.RemoveEmptyEntries);

                                    if (markingPeriodid.First() == "3")
                                    {
                                        var Id = Int32.Parse(markingPeriodid.ElementAt(1));

                                        var ppData = this.context?.ProgressPeriods.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == Id && x.AcademicYear == reportCardViewModel.AcademicYear);
                                        markingPeriodListView.StartDate = ppData!.StartDate;
                                        markingPeriodListView.EndDate = ppData.EndDate;

                                        if (markingPeriodid.Last() == "E")
                                        {
                                            markingPeriodListView.MarkingPeriodName = ppData!.ShortName + " Exam";
                                            markingPeriodListView.SortId = "3_" + ppData.MarkingPeriodId + " Exam";
                                            PrgrsprdMarkingPeriodIdExam.Add(Id);
                                        }
                                        else
                                        {
                                            markingPeriodListView.MarkingPeriodName = ppData.ShortName;
                                            markingPeriodListView.SortId = "3_" + ppData.MarkingPeriodId;
                                            PrgrsprdMarkingPeriodId.Add(Id);
                                        }
                                        markingPeriodList.Add(markingPeriodListView);
                                    }

                                    else if (markingPeriodid.First() == "2")
                                    {
                                        var Id = Int32.Parse(markingPeriodid.ElementAt(1));

                                        var qtrData = this.context?.Quarters.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == Id && x.AcademicYear == reportCardViewModel.AcademicYear);
                                        markingPeriodListView.StartDate = qtrData!.StartDate;
                                        markingPeriodListView.EndDate = qtrData.EndDate;

                                        if (markingPeriodid.Last() == "E")
                                        {
                                            markingPeriodListView.MarkingPeriodName = qtrData!.ShortName + " Exam";
                                            markingPeriodListView.SortId = "2_" + qtrData.MarkingPeriodId + " Exam";
                                            QtrMarkingPeriodIdExam.Add(Id);
                                        }
                                        else
                                        {
                                            markingPeriodListView.MarkingPeriodName = qtrData.ShortName;
                                            markingPeriodListView.SortId = "2_" + qtrData.MarkingPeriodId;
                                            QtrMarkingPeriodId.Add(Id);
                                        }
                                        markingPeriodList.Add(markingPeriodListView);
                                    }

                                    else if (markingPeriodid.First() == "1")
                                    {
                                        var Id = Int32.Parse(markingPeriodid.ElementAt(1));

                                        var smstrData = this.context?.Semesters.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == Id && x.AcademicYear == reportCardViewModel.AcademicYear);
                                        markingPeriodListView.StartDate = smstrData!.StartDate;
                                        markingPeriodListView.EndDate = smstrData.EndDate;

                                        if (markingPeriodid.Last() == "E")
                                        {
                                            markingPeriodListView.MarkingPeriodName = smstrData!.ShortName + " Exam";
                                            markingPeriodListView.SortId = "1_" + smstrData.MarkingPeriodId + " Exam";
                                            SmstrMarkingPeriodIdExam.Add(Id);
                                        }
                                        else
                                        {
                                            markingPeriodListView.MarkingPeriodName = smstrData.ShortName;
                                            markingPeriodListView.SortId = "1_" + smstrData.MarkingPeriodId;
                                            SmstrMarkingPeriodId.Add(Id);
                                        }
                                        markingPeriodList.Add(markingPeriodListView);
                                    }

                                    else if (markingPeriodid.First() == "0")
                                    {
                                        var Id = Int32.Parse(markingPeriodid.ElementAt(1));

                                        var yrData = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == Id && x.AcademicYear == reportCardViewModel.AcademicYear);
                                        markingPeriodListView.StartDate = yrData!.StartDate;
                                        markingPeriodListView.EndDate = yrData.EndDate;

                                        if (markingPeriodid.Last() == "E")
                                        {
                                            markingPeriodListView.MarkingPeriodName = yrData.ShortName + " Exam";
                                            markingPeriodListView.SortId = "0_" + yrData.MarkingPeriodId + " Exam";
                                            YrMarkingPeriodIdExam.Add(Id);
                                        }
                                        else
                                        {
                                            markingPeriodListView.MarkingPeriodName = yrData!.ShortName;
                                            markingPeriodListView.SortId = "0_" + yrData.MarkingPeriodId;
                                            YrMarkingPeriodId.Add(Id);
                                        }
                                        markingPeriodList.Add(markingPeriodListView);
                                    }
                                }
                            }

                            //fetch all final grade data for student
                            var studentFinalGradeData = this.context?.StudentFinalGrade.Include(x => x.SchoolYears).Include(x => x.Semesters).Include(x => x.Quarters).Include(x => x.ProgressPeriod).Join(this.context?.CourseSection.Include(x => x.Course)!, sfg => sfg.CourseSectionId, cs => cs.CourseSectionId,
                           (sfg, cs) => new { sfg, cs }).Where(x => x.sfg.TenantId == reportCardViewModel.TenantId && x.sfg.SchoolId == reportCardViewModel.SchoolId && x.cs.TenantId == reportCardViewModel.TenantId && x.cs.SchoolId == reportCardViewModel.SchoolId && x.sfg.StudentId == student.StudentId && x.sfg.AcademicYear == reportCardViewModel.AcademicYear && Subjects.Contains(x.cs.Course.CourseSubject.ToLower()) &&

                           ((((x.sfg.YrMarkingPeriodId != null && YrMarkingPeriodId.Contains(x.sfg.YrMarkingPeriodId)) || (x.sfg.SmstrMarkingPeriodId != null && SmstrMarkingPeriodId.Contains(x.sfg.SmstrMarkingPeriodId)) || (x.sfg.QtrMarkingPeriodId != null && QtrMarkingPeriodId.Contains(x.sfg.QtrMarkingPeriodId)) || (x.sfg.PrgrsprdMarkingPeriodId != null && PrgrsprdMarkingPeriodId.Contains(x.sfg.PrgrsprdMarkingPeriodId))) && x.sfg.IsExamGrade != true) ||
                           (((x.sfg.YrMarkingPeriodId != null && YrMarkingPeriodIdExam.Contains(x.sfg.YrMarkingPeriodId)) || (x.sfg.SmstrMarkingPeriodId != null && SmstrMarkingPeriodIdExam.Contains(x.sfg.SmstrMarkingPeriodId)) || (x.sfg.QtrMarkingPeriodId != null && QtrMarkingPeriodIdExam.Contains(x.sfg.QtrMarkingPeriodId)) || (x.sfg.PrgrsprdMarkingPeriodId != null && PrgrsprdMarkingPeriodIdExam.Contains(x.sfg.PrgrsprdMarkingPeriodId))) && x.sfg.IsExamGrade == true))).ToList();

                            if (studentFinalGradeData?.Any() == true)
                            {
                                var subjectWiseData = studentFinalGradeData.GroupBy(x => x.cs.Course.CourseSubject).ToList();

                                foreach (var subject in subjectWiseData)
                                {
                                    SubjectDetailsForRMITemplate subjectDetailsForRMITemplate = new();
                                    subjectDetailsForRMITemplate.SubjectName = subject.Key;

                                    //fetch course section from subject
                                    var courseSectionWiseData = subject.GroupBy(x => x.sfg.CourseSectionId).ToList();

                                    foreach (var courseSection in courseSectionWiseData)
                                    {
                                        CourseSectionDetailsViewforRMIReport courseSectionDetailsView = new();

                                        var courseSectionData = courseSection.Select(s => s.cs).FirstOrDefault();

                                        courseSectionDetailsView.CourseSectionName = courseSectionData!.CourseSectionName;

                                        //fetch markingperiod name & bind in VM where final grade has for this course section.
                                        courseSectionDetailsView.markingPeriodDetailsViewforRMIReports = courseSection.Select(s => new MarkingPeriodDetailsViewforRMIReport { Percentage = reportCardViewModel.Parcentage == true ? s.sfg.PercentMarks : 0, Grade = s.sfg.GradeObtained, MarkingPeriodName = s.sfg.IsExamGrade == true ? (s.sfg.ProgressPeriod != null ? s.sfg.ProgressPeriod.ShortName + " Exam" : s.sfg.Quarters != null ? s.sfg.Quarters.ShortName + " Exam" : s.sfg.Semesters != null ? s.sfg.Semesters.ShortName + " Exam" : s.sfg.SchoolYears != null ? s.sfg.SchoolYears.ShortName + " Exam" : null) : (s.sfg.ProgressPeriod != null ? s.sfg.ProgressPeriod.ShortName : s.sfg.Quarters != null ? s.sfg.Quarters.ShortName : s.sfg.Semesters != null ? s.sfg.Semesters.ShortName : s.sfg.SchoolYears != null ? s.sfg.SchoolYears.ShortName : null), SortId = s.sfg.ProgressPeriod != null ? "3_" + s.sfg.ProgressPeriod.MarkingPeriodId : s.sfg.Quarters != null ? "2_" + s.sfg.Quarters.MarkingPeriodId : s.sfg.Semesters != null ? "1_" + s.sfg.Semesters.MarkingPeriodId : s.sfg.SchoolYears != null ? "0_" + s.sfg.SchoolYears.MarkingPeriodId : null }).ToList();

                                        //fetch markingperiod name & bind in VM where final grade has not for this course section.
                                        var exceptMarkingPeriod = markingPeriodList.Select(x => new { x.MarkingPeriodName, x.SortId }).Except(courseSectionDetailsView.markingPeriodDetailsViewforRMIReports.Select(x => new { x.MarkingPeriodName, x.SortId })).Select(s => new MarkingPeriodDetailsViewforRMIReport { MarkingPeriodName = s.MarkingPeriodName, SortId = s.SortId }).ToList();

                                        courseSectionDetailsView.markingPeriodDetailsViewforRMIReports.AddRange(exceptMarkingPeriod);
                                        courseSectionDetailsView.markingPeriodDetailsViewforRMIReports = courseSectionDetailsView.markingPeriodDetailsViewforRMIReports.OrderBy(s => s.SortId).ThenBy(s => s.MarkingPeriodName).ToList();
                                        subjectDetailsForRMITemplate.courseSectionDetailsViewforRMIReports.Add(courseSectionDetailsView);
                                    }
                                    studentsReportCard.subjectDetailsForRMITemplates.Add(subjectDetailsForRMITemplate);
                                }

                                //overall GPA calculation marking period waise
                                markingPeriodList = markingPeriodList.OrderBy(s => s.SortId).ThenBy(s => s.MarkingPeriodName).ToList();

                                //this loop for all marking period
                                foreach (var markingPeriod in markingPeriodList)
                                {
                                    MarkingPeriodDetailsViewforRMIReport markingPeriodListView = new MarkingPeriodDetailsViewforRMIReport();

                                    int? PPId = null;
                                    int? QtrId = null;
                                    int? SemId = null;
                                    int? FYId = null;
                                    DateTime? startDate = null;
                                    DateTime? endDate = null;

                                    if (markingPeriod != null)
                                    {
                                        startDate = markingPeriod.StartDate;
                                        endDate = markingPeriod.EndDate;
                                        var markingPeriodid = markingPeriod.SortId!.Split("_", StringSplitOptions.RemoveEmptyEntries);

                                        if (markingPeriodid.First() == "3")
                                        {
                                            PPId = Int32.Parse(markingPeriodid.ElementAt(1));
                                        }

                                        else if (markingPeriodid.First() == "2")
                                        {
                                            QtrId = Int32.Parse(markingPeriodid.ElementAt(1));
                                        }

                                        else if (markingPeriodid.First() == "1")
                                        {
                                            SemId = Int32.Parse(markingPeriodid.ElementAt(1));
                                        }

                                        else if (markingPeriodid.First() == "0")
                                        {
                                            FYId = Int32.Parse(markingPeriodid.ElementAt(1));
                                        }

                                        var reportCardData = studentFinalGradeData.Where(x => ((x.sfg.YrMarkingPeriodId != null && x.sfg.YrMarkingPeriodId == FYId) || (x.sfg.SmstrMarkingPeriodId != null && x.sfg.SmstrMarkingPeriodId == SemId) || (x.sfg.QtrMarkingPeriodId != null && x.sfg.QtrMarkingPeriodId == QtrId) || (x.sfg.PrgrsprdMarkingPeriodId != null && x.sfg.PrgrsprdMarkingPeriodId == PPId)) && (markingPeriodid.Last() == "E" ? x.sfg.IsExamGrade == true : x.sfg.IsExamGrade != true)).ToList();

                                        decimal? SumofCreditEarned = 0.0m;
                                        decimal? SumofGPaValue = 0.0m;

                                        if (reportCardData?.Any() == true)
                                        {
                                            foreach (var reportCard in reportCardData)
                                            {
                                                var gradeData = reportCard.cs.GradeScale?.Grade.AsEnumerable().Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && String.Compare(x.Title, reportCard.sfg.GradeObtained, true) == 0 && x.GradeScaleId == reportCard.sfg.GradeScaleId).FirstOrDefault();

                                                if (gradeData != null)
                                                {
                                                    var CreditEarned = reportCard.sfg.CreditEarned != null ? reportCard.sfg.CreditEarned : 0.0m;
                                                    var gPaValue = reportCard.cs.IsWeightedCourse != true ? gradeData.UnweightedGpValue * CreditEarned : gradeData.WeightedGpValue * CreditEarned;
                                                    SumofCreditEarned = SumofCreditEarned + CreditEarned;
                                                    SumofGPaValue = SumofGPaValue + gPaValue;
                                                }
                                            }
                                        }

                                        if (SumofGPaValue > 0 && SumofCreditEarned > 0)
                                        {
                                            markingPeriodListView.GPA = (Math.Round((decimal)(SumofGPaValue / SumofCreditEarned), 3)).ToString();
                                        }

                                        markingPeriodListView.MarkingPeriodName = markingPeriod.MarkingPeriodName;
                                        studentsReportCard.overallGPAList.Add(markingPeriodListView);

                                        //this block for attendance details marking period waise
                                        int PresentCount = 0;
                                        int AbsentCount = 0;
                                        //int HalfDayCount = 0;
                                        MarkingPeriodDetailsViewforRMIReport attendanceByMarkingPeriod = new();
                                        //var attendanceData = this.context?.AttendanceCodeCategories.Include(x => x.AttendanceCode).FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                        //if (attendanceData != null)
                                        //{
                                        var studentDailyAttendanceData = StudentDailyAttendanceData?.Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId && x.AttendanceDate >= startDate && x.AttendanceDate <= endDate).ToList();
                                        if (studentDailyAttendanceData?.Any() == true)
                                        {
                                            foreach (var dailyAttendance in studentDailyAttendanceData)
                                            {
                                                var StudentAttendanceData = AttendanceData?.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId && x.AttendanceDate == dailyAttendance.AttendanceDate);

                                                var block = BlockData?.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.BlockId == StudentAttendanceData!.BlockId
                                                );

                                                if (dailyAttendance.AttendanceMinutes >= block?.FullDayMinutes)
                                                {
                                                    PresentCount++;
                                                }
                                                //if (dailyAttendance.AttendanceMinutes >= block?.HalfDayMinutes && dailyAttendance.AttendanceMinutes < block?.FullDayMinutes)
                                                //{
                                                //    HalfDayCount++;
                                                //}
                                                if (dailyAttendance.AttendanceMinutes < block?.HalfDayMinutes)
                                                {
                                                    AbsentCount++;
                                                }
                                            }
                                            attendanceByMarkingPeriod.PresentCount = PresentCount;
                                            attendanceByMarkingPeriod.AbsencesCount = AbsentCount;
                                        }
                                        attendanceByMarkingPeriod.MarkingPeriodName = markingPeriod.MarkingPeriodName;
                                        studentsReportCard.attendanceDetailsViewforRMIReports.Add(attendanceByMarkingPeriod);
                                        //}
                                    }
                                }
                                studentsReportCard.markingPeriodDetailsViewforRMIReports = markingPeriodList;
                            }
                            reportCardView.studentsReportCardViewModelList.Add(studentsReportCard);
                        }
                    }

                    else
                    {
                        foreach (var student in reportCardViewModel.studentsReportCardViewModelList)
                        {
                            StudentsReportCardViewModel studentsReportCardViewModel = new StudentsReportCardViewModel();
                            List<EffortGradeDetailsViewModel> effortGradeDetailList = new List<EffortGradeDetailsViewModel>();
                            List<StanderdsGradeDetailsViewModel> standerdsGradeDetailsList = new List<StanderdsGradeDetailsViewModel>();
                            List<StanderdsGradeDetailsViewModel> MarkingPeriodList = new List<StanderdsGradeDetailsViewModel>();

                            var studentData = this.context?.StudentMaster.Include(x => x.Sections).Include(x => x.StudentEnrollment).FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId);

                            var GradeLevelTitle = studentData!.StudentEnrollment.Where(x => x.IsActive == true).Select(s => s.GradeLevelTitle).FirstOrDefault();

                            studentsReportCardViewModel.SchoolName = schoolData!.SchoolName;
                            //studentsReportCardViewModel.SchoolLogo = schoolData!.SchoolDetail.FirstOrDefault()!.SchoolLogo;
                            studentsReportCardViewModel.SchoolLogo = schoolData!.SchoolDetail.FirstOrDefault()!.SchoolThumbnailLogo;
                            studentsReportCardViewModel.SchoolYear = schoolYear;
                            studentsReportCardViewModel.StudentId = studentData.StudentId; studentsReportCardViewModel.StudentInternalId = studentData.StudentInternalId;
                            studentsReportCardViewModel.FirstGivenName = studentData.FirstGivenName;
                            studentsReportCardViewModel.MiddleName = studentData.MiddleName;
                            studentsReportCardViewModel.LastFamilyName = studentData.LastFamilyName;
                            studentsReportCardViewModel.Gender = studentData.Gender;
                            studentsReportCardViewModel.GradeTitle = GradeLevelTitle;
                            studentsReportCardViewModel.Section = studentData.Sections != null ? studentData.Sections.Name : null;
                            studentsReportCardViewModel.HomeAddressLineOne = studentData.HomeAddressLineOne;
                            studentsReportCardViewModel.HomeAddressLineTwo = studentData.HomeAddressLineTwo;
                            studentsReportCardViewModel.HomeAddressCountry = studentData.HomeAddressCountry;
                            studentsReportCardViewModel.HomeAddressState = studentData.HomeAddressState;
                            studentsReportCardViewModel.HomeAddressCity = studentData.HomeAddressCity;
                            studentsReportCardViewModel.HomeAddressZip = studentData.HomeAddressZip;

                            if (schoolData.GradeScale.Count > 0)
                            {
                                var gradeScaledata = schoolData.GradeScale.FirstOrDefault(x => x.UseAsStandardGradeScale != true);

                                if (gradeScaledata != null && gradeScaledata?.Grade.Count > 0)
                                {
                                    gradeScaledata?.Grade.ToList().ForEach(x => x.GradeScale = new());
                                    studentsReportCardViewModel.gradeList = gradeScaledata!.Grade.ToList();
                                }
                            }

                            var markingPeriodsData = reportCardViewModel.MarkingPeriods!.Split(",");
                            DateTime? startDate = null;
                            DateTime? endDate = null;
                            string? MarkingPeriodTitle = null;
                            string? SortId = null;

                            var StudentDailyAttendanceData = this.context?.StudentDailyAttendance.Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId);

                            var AttendanceData = this.context?.StudentAttendance.Include(s => s.BlockPeriod).Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId);

                            var BlockData = this.context?.Block.Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId);

                            foreach (var markingPeriod in markingPeriodsData)
                            {
                                MarkingPeriodDetailsForOtherTemplate markingPeriodDetailsForOtherTemplate = new MarkingPeriodDetailsForOtherTemplate();
                                StanderdsGradeDetailsViewModel MarkingPeriod = new();

                                int? QtrMarkingPeriodId = null;
                                int? SmstrMarkingPeriodId = null;
                                int? YrMarkingPeriodId = null;
                                int? PrgrsprdMarkingPeriodId = null;
                                bool? Exam = null;
                                bool? IsCustomDateRange = null;

                                if (markingPeriod != null)
                                {
                                    var markingPeriodid = markingPeriod.Split("_", StringSplitOptions.RemoveEmptyEntries);

                                    if (markingPeriodid.First() == "3")
                                    {
                                        PrgrsprdMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));

                                        var ppData = this.context?.ProgressPeriods.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == PrgrsprdMarkingPeriodId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                        MarkingPeriodTitle = ppData!.ShortName;
                                        startDate = ppData.StartDate;
                                        endDate = ppData.EndDate;
                                        SortId = "3_" + ppData.MarkingPeriodId;
                                        MarkingPeriod.SortId = SortId;
                                        MarkingPeriod.MarkingPeriodName = MarkingPeriodTitle;

                                        if (markingPeriodid.Last() == "E")
                                        {
                                            Exam = true;
                                            MarkingPeriodTitle = ppData!.ShortName + " Exam";
                                            MarkingPeriod.MarkingPeriodName = MarkingPeriodTitle;

                                        }
                                        MarkingPeriodList.Add(MarkingPeriod);
                                    }

                                    else if (markingPeriodid.First() == "2")
                                    {
                                        QtrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));

                                        var qtrData = this.context?.Quarters.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == QtrMarkingPeriodId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                        MarkingPeriodTitle = qtrData!.ShortName;
                                        startDate = qtrData.StartDate;
                                        endDate = qtrData.EndDate;
                                        SortId = "2_" + qtrData.MarkingPeriodId;
                                        MarkingPeriod.SortId = SortId;
                                        MarkingPeriod.MarkingPeriodName = MarkingPeriodTitle;

                                        if (markingPeriodid.Last() == "E")
                                        {
                                            Exam = true;
                                            MarkingPeriodTitle = qtrData!.ShortName + " Exam";
                                            MarkingPeriod.MarkingPeriodName = MarkingPeriodTitle;
                                        }
                                        MarkingPeriodList.Add(MarkingPeriod);
                                    }

                                    else if (markingPeriodid.First() == "1")
                                    {
                                        SmstrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));

                                        var smstrData = this.context?.Semesters.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == SmstrMarkingPeriodId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                        MarkingPeriodTitle = smstrData!.ShortName;
                                        startDate = smstrData.StartDate;
                                        endDate = smstrData.EndDate;
                                        SortId = "1_" + smstrData.MarkingPeriodId;
                                        MarkingPeriod.SortId = SortId;
                                        MarkingPeriod.MarkingPeriodName = MarkingPeriodTitle;

                                        if (markingPeriodid.Last() == "E")
                                        {
                                            Exam = true;
                                            MarkingPeriodTitle = smstrData!.ShortName + " Exam";
                                            MarkingPeriod.MarkingPeriodName = MarkingPeriodTitle;
                                        }
                                        MarkingPeriodList.Add(MarkingPeriod);
                                    }

                                    else if (markingPeriodid.First() == "0")
                                    {
                                        YrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));

                                        var yrData = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.MarkingPeriodId == YrMarkingPeriodId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                        MarkingPeriodTitle = yrData!.ShortName;
                                        startDate = yrData.StartDate;
                                        endDate = yrData.EndDate;
                                        SortId = "0_" + yrData.MarkingPeriodId;
                                        MarkingPeriod.SortId = SortId;
                                        MarkingPeriod.MarkingPeriodName = MarkingPeriodTitle;

                                        if (markingPeriodid.Last() == "E")
                                        {
                                            Exam = true;
                                            MarkingPeriodTitle = yrData!.ShortName + " Exam";
                                            MarkingPeriod.MarkingPeriodName = MarkingPeriodTitle;
                                        }
                                        MarkingPeriodList.Add(MarkingPeriod);
                                    }

                                    else if (markingPeriodid.First() == "Custom")
                                    {
                                        IsCustomDateRange = true;
                                        MarkingPeriodTitle = "Custom";
                                        SortId = "5";
                                        MarkingPeriod.SortId = SortId;
                                        MarkingPeriod.MarkingPeriodName = MarkingPeriodTitle;
                                        MarkingPeriodList.Add(MarkingPeriod);
                                    }

                                    markingPeriodDetailsForOtherTemplate.MarkingPeriodShortName = MarkingPeriodTitle;
                                    var reportCardData = new List<StudentFinalGrade>();

                                    if (IsCustomDateRange == true)
                                    {
                                        reportCardData = this.context?.StudentFinalGrade.Include(x => x.StudentFinalGradeComments).Include(x => x.StudentFinalGradeStandard).Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId && x.AcademicYear == reportCardViewModel.AcademicYear && x.IsCustomMarkingPeriod == true).ToList();
                                    }
                                    else
                                    {
                                        if (Exam == true)
                                        {
                                            reportCardData = this.context?.StudentFinalGrade.Include(x => x.StudentFinalGradeComments).Include(x => x.StudentFinalGradeStandard).Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId && x.AcademicYear == reportCardViewModel.AcademicYear && ((x.YrMarkingPeriodId != null && x.YrMarkingPeriodId == YrMarkingPeriodId) || (x.SmstrMarkingPeriodId != null && x.SmstrMarkingPeriodId == SmstrMarkingPeriodId) || (x.QtrMarkingPeriodId != null && x.QtrMarkingPeriodId == QtrMarkingPeriodId) || (x.PrgrsprdMarkingPeriodId != null && x.PrgrsprdMarkingPeriodId == PrgrsprdMarkingPeriodId)) && x.IsExamGrade == true).ToList();
                                        }
                                        else
                                        {
                                            reportCardData = this.context?.StudentFinalGrade.Include(x => x.StudentFinalGradeComments).Include(x => x.StudentFinalGradeStandard).Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId && x.AcademicYear == reportCardViewModel.AcademicYear && ((x.YrMarkingPeriodId != null && x.YrMarkingPeriodId == YrMarkingPeriodId) || (x.SmstrMarkingPeriodId != null && x.SmstrMarkingPeriodId == SmstrMarkingPeriodId) || (x.QtrMarkingPeriodId != null && x.QtrMarkingPeriodId == QtrMarkingPeriodId) || (x.PrgrsprdMarkingPeriodId != null && x.PrgrsprdMarkingPeriodId == PrgrsprdMarkingPeriodId)) && x.IsExamGrade != true).ToList();
                                        }
                                    }

                                    decimal? SumofGPaValue = 0.0m;
                                    decimal? CreditEarned = 0.0m;
                                    decimal? CreditHours = 0.0m;
                                    int CourseCount = 0;
                                    if (reportCardData?.Any() == true)
                                    {
                                        foreach (var reportCard in reportCardData)
                                        {
                                            decimal? gPaValue = 0.0m;
                                            CourseCount++;
                                            CourseSectionGradeDetailsForOtherTemplate courseSectionGradeDetailsForOtherTemplate = new CourseSectionGradeDetailsForOtherTemplate();

                                            var CourseSectionData = this.context?.CourseSection.Include(s => s.GradeScale).FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.CourseSectionId == reportCard.CourseSectionId && x.CourseId == reportCard.CourseId);

                                            var gradeData = CourseSectionData?.GradeScale?.Grade.AsEnumerable().Where(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && String.Compare(x.Title, reportCard.GradeObtained, true) == 0 && x.GradeScaleId == reportCard.GradeScaleId).FirstOrDefault();

                                            if (gradeData != null)
                                            {
                                                CreditHours = CourseSectionData?.CreditHours;
                                                CreditEarned = reportCard.CreditEarned != null ? reportCard.CreditEarned : CourseSectionData?.CreditHours;
                                                gPaValue = CourseSectionData?.IsWeightedCourse != true ? gradeData.UnweightedGpValue * (CreditHours / CreditEarned) : gradeData.WeightedGpValue * (CreditHours / CreditEarned);
                                                SumofGPaValue = SumofGPaValue + gPaValue;

                                            }

                                            courseSectionGradeDetailsForOtherTemplate.MarkingPeriodShortName = MarkingPeriodTitle;
                                            courseSectionGradeDetailsForOtherTemplate.CourseSectionName = CourseSectionData?.CourseSectionName;
                                            courseSectionGradeDetailsForOtherTemplate.Grade = reportCard.GradeObtained;
                                            courseSectionGradeDetailsForOtherTemplate.Percentage = reportCard.PercentMarks.ToString();
                                            markingPeriodDetailsForOtherTemplate.courseSectionGradeDetailsForOtherTemplates.Add(courseSectionGradeDetailsForOtherTemplate);

                                            //this block for StandardGrade
                                            if (reportCardViewModel.StandardGrade == true)
                                            {
                                                var StudentFinalGradeStandardData = reportCard.StudentFinalGradeStandard.Where(s => s.StandardGradeScaleId > 0 && s.GradeObtained > 0);
                                                if (StudentFinalGradeStandardData?.Any() == true)
                                                {
                                                    foreach (var finalGradeStandardData in StudentFinalGradeStandardData)
                                                    {
                                                        StanderdsGradeDetailsViewModel standerdsGradeDetails = new();
                                                        standerdsGradeDetails.CourseSectionName = CourseSectionData?.CourseSectionName;
                                                        standerdsGradeDetails.StaffName = CourseSectionData?.StaffCoursesectionSchedule.Count > 0 ? CourseSectionData?.StaffCoursesectionSchedule.FirstOrDefault()!.StaffMaster.FirstGivenName + " " + CourseSectionData?.StaffCoursesectionSchedule.FirstOrDefault()!.StaffMaster.MiddleName + " " + CourseSectionData?.StaffCoursesectionSchedule.FirstOrDefault()!.StaffMaster.LastFamilyName : null;
                                                        standerdsGradeDetails.MarkingPeriodName = MarkingPeriodTitle;
                                                        standerdsGradeDetails.SortId = SortId;

                                                        var gradeUsStandardData = this.context?.GradeUsStandard.FirstOrDefault(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && x.GradeStandardId == finalGradeStandardData.StandardGradeScaleId);

                                                        standerdsGradeDetails.StandardRefNo = gradeUsStandardData?.StandardRefNo;
                                                        standerdsGradeDetails.StandardDetails = gradeUsStandardData?.StandardDetails;
                                                        standerdsGradeDetails.value = this.context?.Grade.FirstOrDefault(x => x.TenantId == reportCard.TenantId && x.SchoolId == reportCard.SchoolId && x.GradeId == finalGradeStandardData.GradeObtained)?.Title;

                                                        standerdsGradeDetailsList.Add(standerdsGradeDetails);
                                                    }
                                                }
                                            }
                                        }

                                        if (SumofGPaValue > 0 && CourseCount > 0)
                                        {
                                            var Gpavalue = Math.Round((decimal)(SumofGPaValue / CourseCount), 2);
                                            markingPeriodDetailsForOtherTemplate.GPA = Gpavalue.ToString();
                                        }
                                    }

                                    //this block for attendance details for marking period not for custom date range
                                    if (IsCustomDateRange != true)
                                    {
                                        int PresentCount = 0;
                                        int AbsentCount = 0;
                                        int HalfDayCount = 0;

                                        var attendanceData = this.context?.AttendanceCodeCategories.Include(x => x.AttendanceCode).FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.AcademicYear == reportCardViewModel.AcademicYear);

                                        if (attendanceData != null)
                                        {
                                            var studentDailyAttendanceData = StudentDailyAttendanceData?.Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId && x.AttendanceDate >= startDate && x.AttendanceDate <= endDate).ToList();

                                            if (studentDailyAttendanceData?.Any() == true)
                                            {
                                                foreach (var dailyAttendance in studentDailyAttendanceData)
                                                {
                                                    var StudentAttendanceData = AttendanceData?.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId && x.AttendanceDate == dailyAttendance.AttendanceDate);

                                                    var block = BlockData?.FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.BlockId == StudentAttendanceData!.BlockId
                                                    );

                                                    if (dailyAttendance.AttendanceMinutes >= block?.FullDayMinutes)
                                                    {
                                                        PresentCount++;
                                                    }
                                                    if (dailyAttendance.AttendanceMinutes >= block?.HalfDayMinutes && dailyAttendance.AttendanceMinutes < block?.FullDayMinutes)
                                                    {
                                                        HalfDayCount++;
                                                    }
                                                    if (dailyAttendance.AttendanceMinutes < block?.HalfDayMinutes)
                                                    {
                                                        AbsentCount++;
                                                    }
                                                }
                                            }
                                            //this loop for multiple attendance code
                                            foreach (var Attendance in attendanceData.AttendanceCode.ToList())
                                            {
                                                AttendanceDetailsForOtherTemplate attendanceDetailsForOtherTemplate = new AttendanceDetailsForOtherTemplate();

                                                if (Attendance.StateCode!.ToLower() == "present")
                                                {
                                                    attendanceDetailsForOtherTemplate.AttendanceCount = PresentCount;
                                                }
                                                if (Attendance.StateCode!.ToLower() == "absent")
                                                {
                                                    attendanceDetailsForOtherTemplate.AttendanceCount = AbsentCount;
                                                }
                                                if (Attendance.StateCode!.ToLower() == "half day")
                                                {
                                                    attendanceDetailsForOtherTemplate.AttendanceCount = HalfDayCount;
                                                }

                                                attendanceDetailsForOtherTemplate.AttendanceTitle = Attendance.Title;
                                                attendanceDetailsForOtherTemplate.MarkingPeriodShortName = MarkingPeriodTitle;
                                                markingPeriodDetailsForOtherTemplate.attendanceDetailsForOtherTemplates.Add(attendanceDetailsForOtherTemplate);
                                            }
                                        }
                                    }
                                    studentsReportCardViewModel.markingPeriodDetailsForOtherTemplates.Add(markingPeriodDetailsForOtherTemplate);

                                    //this block for effot grade
                                    if (reportCardViewModel.EffortGrade == true)
                                    {
                                        if (Exam != true)  //becz effort grade are not given for exam
                                        {
                                            var effortGradeData = this.context?.StudentEffortGradeMaster.Include(x => x.StudentEffortGradeDetail).Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.StudentId == student.StudentId && x.AcademicYear == reportCardViewModel.AcademicYear && ((x.YrMarkingPeriodId != null && x.YrMarkingPeriodId == YrMarkingPeriodId) || (x.SmstrMarkingPeriodId != null && x.SmstrMarkingPeriodId == SmstrMarkingPeriodId) || (x.QtrMarkingPeriodId != null && x.QtrMarkingPeriodId == QtrMarkingPeriodId) || (x.PrgrsprdMarkingPeriodId != null && x.PrgrsprdMarkingPeriodId == PrgrsprdMarkingPeriodId))).FirstOrDefault();

                                            if (effortGradeData != null && EffortGradeLibraryCategoryData?.Any() == true)
                                            {
                                                effortGradeData.StudentEffortGradeDetail = effortGradeData.StudentEffortGradeDetail.Where(s => s.EffortCategoryId != null).ToList();

                                                var effortGradeDetails = effortGradeData.StudentEffortGradeDetail.Select(s => new EffortGradeDetailsViewModel
                                                {
                                                    CategoryName = EffortGradeLibraryCategoryData.FirstOrDefault(x => x.EffortCategoryId == s.EffortCategoryId)?.CategoryName,
                                                    MarkingPeriodName = MarkingPeriodTitle,
                                                    SortId = SortId,
                                                    EffortItemTitle = EffortGradeLibraryCategoryData.SelectMany(x => x.EffortGradeLibraryCategoryItem).FirstOrDefault(x => x.EffortCategoryId == s.EffortCategoryId && x.EffortItemId == s.EffortItemId)?.EffortItemTitle,
                                                    GradeScaleValue = s.EffortGradeScaleId
                                                });

                                                effortGradeDetailList.AddRange(effortGradeDetails);
                                            }
                                        }
                                    }

                                }
                            }

                            //this block for standerd grade
                            if (reportCardViewModel.StandardGrade == true)
                            {
                                var CSbygroupData = standerdsGradeDetailsList.GroupBy(G => G.CourseSectionName).ToList();

                                foreach (var csData in CSbygroupData)
                                {
                                    StanderdsGradeDetailsViewModel standerdsGradeDetails = new();
                                    standerdsGradeDetails.CourseSectionName = csData.Key;
                                    standerdsGradeDetails.StaffName = csData.First().StaffName;

                                    var standerdData = csData.GroupBy(x => x.StandardRefNo).ToList();
                                    foreach (var standerd in standerdData)
                                    {
                                        StanderdsGradeDetails standerdsGrade = new();

                                        standerdsGrade.StandardRefNo = standerd.Key;
                                        standerdsGrade.StandardDetails = standerd.First().StandardDetails;
                                        standerdsGrade.markingPeriodDetailsforStanderdsGrades = standerd.Select(s => new MarkingPeriodDetailsforStanderdsGrade { MarkingPeriodName = s.MarkingPeriodName, value = s.value, SortId = s.SortId }).ToList();

                                        var exceptMarkingPeriod = MarkingPeriodList.Select(x => new { x.MarkingPeriodName, x.SortId }).Except(standerdsGrade.markingPeriodDetailsforStanderdsGrades.Select(x => new { x.MarkingPeriodName, x.SortId })).Select(s => new MarkingPeriodDetailsforStanderdsGrade { MarkingPeriodName = s.MarkingPeriodName, SortId = s.SortId }).ToList();

                                        standerdsGrade.markingPeriodDetailsforStanderdsGrades.AddRange(exceptMarkingPeriod);
                                        standerdsGrade.markingPeriodDetailsforStanderdsGrades = standerdsGrade.markingPeriodDetailsforStanderdsGrades.OrderBy(s => s.SortId).ThenBy(s => s.MarkingPeriodName).ToList();
                                        standerdsGradeDetails.standerdsGradeDetails.Add(standerdsGrade);

                                    }
                                    studentsReportCardViewModel.standerdsGradeList.Add(standerdsGradeDetails);
                                }

                                var standerdGradeScale = this.context?.GradeScale.Include(x => x.Grade).FirstOrDefault(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId && x.UseAsStandardGradeScale == true);

                                if (standerdGradeScale != null)
                                {
                                    studentsReportCardViewModel.standerdGradeScale = standerdGradeScale.Grade.Select(s => new Grade { Title = s.Title, Comment = s.Comment, GradeId = s.GradeId }).ToList();
                                }
                            }

                            //this block for effort grade
                            if (reportCardViewModel.EffortGrade == true)
                            {
                                var Categorydata = effortGradeDetailList.GroupBy(g => g.CategoryName).ToList();
                                foreach (var category in Categorydata)
                                {
                                    EffortGradeDetailsViewModel effortGradeDetailsViewModel = new();
                                    var itemData = category.GroupBy(g => g.EffortItemTitle).ToList();

                                    effortGradeDetailsViewModel.CategoryName = category.Key;
                                    foreach (var item in itemData)
                                    {
                                        EffortGradeItemDetails effortGradeItemDetails = new();

                                        effortGradeItemDetails.EffortItemTitle = item.Key;
                                        effortGradeItemDetails.markingPeriodDetailsforEffortGrades = item.Select(s => new MarkingPeriodDetailsforEffortGrade { MarkingPeriodName = s.MarkingPeriodName, GradeScaleValue = s.GradeScaleValue, SortId = s.SortId }).ToList();

                                        var index = MarkingPeriodList.FindIndex(s => s.MarkingPeriodName.ToLower() == "custom");
                                        if (index > -1)
                                        {
                                            MarkingPeriodList.RemoveAt(index);
                                        }

                                        var exceptMarkingPeriod = MarkingPeriodList.Select(x => new { x.MarkingPeriodName, x.SortId }).Except(effortGradeItemDetails.markingPeriodDetailsforEffortGrades.Select(x => new { x.MarkingPeriodName, x.SortId })).Select(s => new MarkingPeriodDetailsforEffortGrade { MarkingPeriodName = s.MarkingPeriodName, SortId = s.SortId }).ToList();

                                        effortGradeItemDetails.markingPeriodDetailsforEffortGrades.AddRange(exceptMarkingPeriod);
                                        effortGradeItemDetails.markingPeriodDetailsforEffortGrades = effortGradeItemDetails.markingPeriodDetailsforEffortGrades.OrderBy(s => s.SortId).ThenBy(s => s.MarkingPeriodName).ToList();

                                        effortGradeDetailsViewModel.effortGradeItemDetails.Add(effortGradeItemDetails);

                                    }
                                    studentsReportCardViewModel.effortGradeList.Add(effortGradeDetailsViewModel);
                                }

                                var effortGradeScale = this.context?.EffortGradeScale.Where(x => x.TenantId == reportCardViewModel.TenantId && x.SchoolId == reportCardViewModel.SchoolId).Select(s => new EffortGradeScale { GradeScaleValue = s.GradeScaleValue, GradeScaleComment = s.GradeScaleComment }).ToList();
                                if (effortGradeScale?.Any() == true)
                                {
                                    studentsReportCardViewModel.effortGradeScales = effortGradeScale;
                                }
                            }

                            reportCardView.studentsReportCardViewModelList.Add(studentsReportCardViewModel);
                        }
                    }
                }
                else
                {
                    reportCardView._failure = true;
                    reportCardView._message = "Select Student Please";
                }
            }
            catch (Exception es)
            {
                reportCardView._failure = true;
                reportCardView._message = es.Message;
            }
            return reportCardView;
        }

    }
}

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
using opensis.data.ViewModels.StudentEnrollmentCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
    public class StudentEnrollmentCodeRepository: IStudentEnrollmentCodeRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public StudentEnrollmentCodeRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Add Student Enrollment Code
        /// </summary>
        /// <param name="studentEnrollmentCodeAddViewModel"></param>
        /// <returns></returns>
        public StudentEnrollmentCodeAddViewModel AddStudentEnrollmentCode(StudentEnrollmentCodeAddViewModel studentEnrollmentCodeAddViewModel)
        {

            //int? MasterEnrollmentCode = Utility.GetMaxPK(this.context, new Func<StudentEnrollmentCode, int>(x => x.EnrollmentCode));
            if (studentEnrollmentCodeAddViewModel.studentEnrollmentCode != null)
            {
                int? MasterEnrollmentCode = 1;

                var EnrollmentCodeData = this.context?.StudentEnrollmentCode.Where(x => x.SchoolId == studentEnrollmentCodeAddViewModel.studentEnrollmentCode.SchoolId && x.TenantId == studentEnrollmentCodeAddViewModel.studentEnrollmentCode.TenantId).OrderByDescending(x => x.EnrollmentCode).FirstOrDefault();

                if (EnrollmentCodeData != null)
                {
                    MasterEnrollmentCode = EnrollmentCodeData.EnrollmentCode + 1;
                }

                studentEnrollmentCodeAddViewModel.studentEnrollmentCode.EnrollmentCode = (int)MasterEnrollmentCode;

                studentEnrollmentCodeAddViewModel.studentEnrollmentCode.AcademicYear = Utility.GetCurrentAcademicYear(this.context!, studentEnrollmentCodeAddViewModel.studentEnrollmentCode.TenantId, studentEnrollmentCodeAddViewModel.studentEnrollmentCode.SchoolId);

                //var studentEnrollmentCodeData = this.context?.StudentEnrollmentCode.Where(x => x.TenantId == studentEnrollmentCodeAddViewModel.studentEnrollmentCode.TenantId && x.SchoolId == studentEnrollmentCodeAddViewModel.studentEnrollmentCode.SchoolId && x.Type.ToLower() == studentEnrollmentCodeAddViewModel.studentEnrollmentCode.Type.ToLower() && x.Type.ToLower() != "DROP".ToLower() && x.Type.ToLower() != "Add".ToLower()).ToList();

                var studentEnrollmentCodeData = this.context?.StudentEnrollmentCode.AsEnumerable().Where(x => x.TenantId == studentEnrollmentCodeAddViewModel.studentEnrollmentCode.TenantId && x.SchoolId == studentEnrollmentCodeAddViewModel.studentEnrollmentCode.SchoolId && String.Compare(x.Type, studentEnrollmentCodeAddViewModel.studentEnrollmentCode.Type, true) == 0 && String.Compare(x.Type, "DROP", true) == 0 && String.Compare(x.Type, "Add", true) == 0).ToList();

                if (studentEnrollmentCodeData!=null && studentEnrollmentCodeData.Any())
                {
                    studentEnrollmentCodeAddViewModel._failure = true;
                    studentEnrollmentCodeAddViewModel._message = "You can't add any enrollment code in this type";
                    return studentEnrollmentCodeAddViewModel;
                }
                studentEnrollmentCodeAddViewModel.studentEnrollmentCode.CreatedOn = DateTime.UtcNow;
                this.context?.StudentEnrollmentCode.Add(studentEnrollmentCodeAddViewModel.studentEnrollmentCode);
                //context!.Entry(studentEnrollmentCodeAddViewModel.studentEnrollmentCode.SchoolMaster).State = EntityState.Unchanged;
                this.context?.SaveChanges();
                studentEnrollmentCodeAddViewModel._failure = false;
                studentEnrollmentCodeAddViewModel._message = "Enrollment Code added successfully";
            }
            return studentEnrollmentCodeAddViewModel;
        }

        /// <summary>
        /// Get Student Enrollment Code
        /// </summary>
        /// <param name="studentEnrollmentCodeAddViewModel"></param>
        /// <returns></returns>

        public StudentEnrollmentCodeAddViewModel ViewStudentEnrollmentCode(StudentEnrollmentCodeAddViewModel studentEnrollmentCodeAddViewModel)
        {
            StudentEnrollmentCodeAddViewModel studentEnrollmentCodeView = new StudentEnrollmentCodeAddViewModel();
            try
            {
                if (studentEnrollmentCodeAddViewModel.studentEnrollmentCode != null)
                {

                    var studentEnrollmentCodeData = this.context?.StudentEnrollmentCode.FirstOrDefault(x => x.TenantId == studentEnrollmentCodeAddViewModel.studentEnrollmentCode.TenantId && x.SchoolId == studentEnrollmentCodeAddViewModel.studentEnrollmentCode.SchoolId && x.EnrollmentCode == studentEnrollmentCodeAddViewModel.studentEnrollmentCode.EnrollmentCode);
                    if (studentEnrollmentCodeData != null)
                    {
                        studentEnrollmentCodeView.studentEnrollmentCode = studentEnrollmentCodeData;
                    }
                    else
                    {
                        studentEnrollmentCodeView._failure = true;
                        studentEnrollmentCodeView._message = NORECORDFOUND;
                    }
                }
            }
            catch (Exception es)
            {
                studentEnrollmentCodeView._failure = true;
                studentEnrollmentCodeView._message = es.Message;
            }
            return studentEnrollmentCodeView;
        }

        /// <summary>
        /// Delete Student Enrollment Code
        /// </summary>
        /// <param name="studentEnrollmentCodeAddViewModel"></param>
        /// <returns></returns>
        public StudentEnrollmentCodeAddViewModel DeleteStudentEnrollmentCode(StudentEnrollmentCodeAddViewModel studentEnrollmentCodeAddViewModel)
        {
            try
            {
                if (studentEnrollmentCodeAddViewModel.studentEnrollmentCode != null)
                {
                    var studentEnrollmentCodeDelete = this.context?.StudentEnrollmentCode.FirstOrDefault(x => x.TenantId == studentEnrollmentCodeAddViewModel.studentEnrollmentCode.TenantId && x.SchoolId == studentEnrollmentCodeAddViewModel.studentEnrollmentCode.SchoolId && x.EnrollmentCode == studentEnrollmentCodeAddViewModel.studentEnrollmentCode.EnrollmentCode);

                    if (studentEnrollmentCodeDelete != null)
                    {
                        //if (studentEnrollmentCodeDelete.Type?.ToLower() == "Roll over".ToLower() || studentEnrollmentCodeDelete.Type?.ToLower() == "Enroll (Transfer)".ToLower() || studentEnrollmentCodeDelete.Type?.ToLower() == "Drop (Transfer)".ToLower())
                        if (String.Compare(studentEnrollmentCodeDelete.Type, "Roll over", true) == 0 || String.Compare(studentEnrollmentCodeDelete.Type, "Enroll (Transfer)",true) == 0 || String.Compare(studentEnrollmentCodeDelete.Type, "Drop (Transfer)",true) == 0)
                        {
                            studentEnrollmentCodeAddViewModel._failure = true;
                            studentEnrollmentCodeAddViewModel._message = "Cannot delete because it is not deletable.";
                        }
                        else
                        {
                            var studentEnrollmentExits = this.context?.StudentEnrollment.FirstOrDefault(x => x.TenantId == studentEnrollmentCodeDelete.TenantId && x.SchoolId == studentEnrollmentCodeDelete.SchoolId && (x.EnrollmentCode == studentEnrollmentCodeDelete.Title || x.ExitCode == studentEnrollmentCodeDelete.Title));
                            if (studentEnrollmentExits != null)
                            {
                                studentEnrollmentCodeAddViewModel._failure = true;
                                studentEnrollmentCodeAddViewModel._message = "Cannot delete because enrollment codes are associated.";
                            }
                            else
                            {
                                this.context?.StudentEnrollmentCode.Remove(studentEnrollmentCodeDelete);
                                this.context?.SaveChanges();
                                studentEnrollmentCodeAddViewModel._failure = false;
                                studentEnrollmentCodeAddViewModel._message = "Enrollment Code deleted successfullyy";
                            }
                        }
                    }
                }
            }
            catch (Exception es)
            {
                studentEnrollmentCodeAddViewModel._failure = true;
                studentEnrollmentCodeAddViewModel._message = es.Message;
            }
            return studentEnrollmentCodeAddViewModel;
        }

        /// <summary>
        /// Update Student Enrollment Code
        /// </summary>
        /// <param name="studentEnrollmentCodeAddViewModel"></param>
        /// <returns></returns>
        public StudentEnrollmentCodeAddViewModel UpdateStudentEnrollmentCode(StudentEnrollmentCodeAddViewModel studentEnrollmentCodeAddViewModel)
        {
            try
            {
                if (studentEnrollmentCodeAddViewModel.studentEnrollmentCode != null)
                {
                    var studentEnrollmentCodeUpdate = this.context?.StudentEnrollmentCode.FirstOrDefault(x => x.TenantId == studentEnrollmentCodeAddViewModel.studentEnrollmentCode.TenantId && x.SchoolId == studentEnrollmentCodeAddViewModel.studentEnrollmentCode.SchoolId && x.EnrollmentCode == studentEnrollmentCodeAddViewModel.studentEnrollmentCode.EnrollmentCode);

                    if (studentEnrollmentCodeUpdate != null)
                    {
                        //var sameTitleExits = this.context?.StudentEnrollment.Where(x => x.SchoolId == studentEnrollmentCodeUpdate.SchoolId && x.TenantId == studentEnrollmentCodeUpdate.TenantId && (x.EnrollmentCode.ToLower() == studentEnrollmentCodeUpdate.Title.ToLower() || x.ExitCode.ToLower() == studentEnrollmentCodeUpdate.Title.ToLower())).ToList();
                        var sameTitleExits = this.context?.StudentEnrollment.AsEnumerable().Where(x => x.SchoolId == studentEnrollmentCodeUpdate.SchoolId && x.TenantId == studentEnrollmentCodeUpdate.TenantId && String.Compare(x.EnrollmentCode, studentEnrollmentCodeUpdate.Title, true) == 0 && String.Compare(x.ExitCode, studentEnrollmentCodeUpdate.Title, true) == 0 && x.AcademicYear == studentEnrollmentCodeUpdate.AcademicYear).ToList();

                        if (sameTitleExits != null && sameTitleExits.Any())
                        {
                            foreach (var title in sameTitleExits)
                            {
                                //if (title.EnrollmentCode != null && title.EnrollmentCode.ToLower() == studentEnrollmentCodeUpdate?.Title?.ToLower())
                                if (title.EnrollmentCode != null && String.Compare(title.EnrollmentCode, studentEnrollmentCodeUpdate.Title, true) == 0)
                                {
                                    title.EnrollmentCode = studentEnrollmentCodeAddViewModel.studentEnrollmentCode.Title;
                                }
                                // if(title.ExitCode != null && title.ExitCode.ToLower() == studentEnrollmentCodeUpdate?.Title?.ToLower())
                                if (title.ExitCode != null && String.Compare(title.ExitCode, studentEnrollmentCodeUpdate.Title, true) == 0)
                                {
                                    title.ExitCode = studentEnrollmentCodeAddViewModel.studentEnrollmentCode.Title;
                                }
                            }
                            this.context?.StudentEnrollment.UpdateRange(sameTitleExits);
                        }

                        if (studentEnrollmentCodeAddViewModel.studentEnrollmentCode.Type?.ToLower() != studentEnrollmentCodeUpdate?.Type?.ToLower())
                        {
                            studentEnrollmentCodeAddViewModel._message = "Can't edit Type because it is not editable";
                        }
                        else
                        {
                            studentEnrollmentCodeAddViewModel.studentEnrollmentCode.AcademicYear = studentEnrollmentCodeUpdate!.AcademicYear;
                            studentEnrollmentCodeAddViewModel.studentEnrollmentCode.UpdatedOn = DateTime.UtcNow;
                            studentEnrollmentCodeAddViewModel.studentEnrollmentCode.CreatedOn = studentEnrollmentCodeUpdate.CreatedOn;
                            studentEnrollmentCodeAddViewModel.studentEnrollmentCode.CreatedBy = studentEnrollmentCodeUpdate.CreatedBy;
                            this.context?.Entry(studentEnrollmentCodeUpdate).CurrentValues.SetValues(studentEnrollmentCodeAddViewModel.studentEnrollmentCode);
                            this.context?.SaveChanges();
                            studentEnrollmentCodeAddViewModel._failure = false;
                            studentEnrollmentCodeAddViewModel._message = "Enrollment Code Updated Successfully";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                studentEnrollmentCodeAddViewModel.studentEnrollmentCode = null;
                studentEnrollmentCodeAddViewModel._failure = true;
                studentEnrollmentCodeAddViewModel._message = ex.Message;
            }
            return studentEnrollmentCodeAddViewModel;
        }

        /// <summary>
        /// Get All Student Enrollment Code
        /// </summary>
        /// <param name="studentEnrollmentCodeListView"></param>
        /// <returns></returns>
        public StudentEnrollmentCodeListViewModel GetAllStudentEnrollmentCode(StudentEnrollmentCodeListViewModel studentEnrollmentCodeListView)
        {
            StudentEnrollmentCodeListViewModel studentEnrollmentCodeList = new StudentEnrollmentCodeListViewModel();
            try
            {

                var StudentEnrollmentCodeAll = this.context?.StudentEnrollmentCode.Where(x => x.TenantId == studentEnrollmentCodeListView.TenantId && x.SchoolId == studentEnrollmentCodeListView.SchoolId && x.AcademicYear == studentEnrollmentCodeListView.AcademicYear).ToList();

                if (StudentEnrollmentCodeAll!=null && StudentEnrollmentCodeAll.Any())
                {
                    if (studentEnrollmentCodeListView.IsListView == true)
                    {
                        StudentEnrollmentCodeAll.ForEach(c =>
                        {
                            c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, studentEnrollmentCodeListView.TenantId, c.CreatedBy);
                            c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, studentEnrollmentCodeListView.TenantId, c.UpdatedBy);
                        });
                    }
                }

                studentEnrollmentCodeList.studentEnrollmentCodeList = StudentEnrollmentCodeAll ?? new List<StudentEnrollmentCode>();
                studentEnrollmentCodeList._tenantName = studentEnrollmentCodeListView._tenantName;
                studentEnrollmentCodeList._token = studentEnrollmentCodeListView._token;

                if (StudentEnrollmentCodeAll!=null && StudentEnrollmentCodeAll.Any())
                {                
                    studentEnrollmentCodeList._failure = false;
                }
                else
                {
                    studentEnrollmentCodeList._failure = true;
                    studentEnrollmentCodeList._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                studentEnrollmentCodeList._message = es.Message;
                studentEnrollmentCodeList._failure = true;
                studentEnrollmentCodeList._tenantName = studentEnrollmentCodeListView._tenantName;
                studentEnrollmentCodeList._token = studentEnrollmentCodeListView._token;
            }
            return studentEnrollmentCodeList;

        }

        /// <summary>
        /// UpdateStudentEnrollmentCodeSortOrder
        /// </summary>
        /// <param name="studentEnrollmentCodeSortOrderViewModel"></param>
        /// <returns></returns>
        public StudentEnrollmentCodeSortOrderViewModel UpdateStudentEnrollmentCodeSortOrder(StudentEnrollmentCodeSortOrderViewModel studentEnrollmentCodeSortOrderViewModel)
        {
            try
            {
                var studentEnrollmentCodeList = this.context?.StudentEnrollmentCode.Where(a => a.TenantId == studentEnrollmentCodeSortOrderViewModel.TenantId && a.SchoolId == studentEnrollmentCodeSortOrderViewModel.SchoolId && a.AcademicYear == studentEnrollmentCodeSortOrderViewModel._academicYear);

                if (studentEnrollmentCodeList?.Any() == true)
                {
                    var sortOrderValues = studentEnrollmentCodeSortOrderViewModel.SortOrderValues.OrderBy(c => c.Id);
                    foreach (var sortOrderValue in sortOrderValues)
                    {
                        var studentEnrollmentCodeData = studentEnrollmentCodeList.FirstOrDefault(d => d.TenantId == studentEnrollmentCodeSortOrderViewModel.TenantId && d.SchoolId == studentEnrollmentCodeSortOrderViewModel.SchoolId && d.EnrollmentCode == sortOrderValue.Id);

                        if (studentEnrollmentCodeData != null)
                        {
                            var oldStudentEnrollmentCodeData = studentEnrollmentCodeData;
                            studentEnrollmentCodeData.SortOrder = sortOrderValue.SortOrder;
                            studentEnrollmentCodeData.UpdatedOn = DateTime.UtcNow;
                            studentEnrollmentCodeData.UpdatedBy = studentEnrollmentCodeSortOrderViewModel.UpdatedBy;
                            this.context?.Entry(oldStudentEnrollmentCodeData).CurrentValues.SetValues(studentEnrollmentCodeData);
                        }
                    }
                    this.context?.SaveChanges();
                    studentEnrollmentCodeSortOrderViewModel._failure = false;
                    studentEnrollmentCodeSortOrderViewModel._message = "Sort order updated successfully";
                }
                else
                {
                    studentEnrollmentCodeSortOrderViewModel._failure = true;
                    studentEnrollmentCodeSortOrderViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                studentEnrollmentCodeSortOrderViewModel._message = es.Message;
                studentEnrollmentCodeSortOrderViewModel._failure = true;
            }
            return studentEnrollmentCodeSortOrderViewModel;
        }
    }
}

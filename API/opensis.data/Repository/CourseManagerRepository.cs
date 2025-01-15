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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.CourseManager;

namespace opensis.data.Repository
{
    public class CourseManagerRepository : ICourseManagerRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public CourseManagerRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }
        /// <summary>
        /// Add Program
        /// </summary>
        /// <param name="programAddViewModel"></param>
        /// <returns></returns>
        //public ProgramAddViewModel AddProgram(ProgramAddViewModel programAddViewModel)
        //{
        //    //int? ProgramId = Utility.GetMaxPK(this.context, new Func<Programs, int>(x => x.ProgramId));

        //    int? ProgramId = 0;

        //    var programData = this.context?.Programs.Where(x => x.SchoolId == programAddViewModel.programs.SchoolId && x.TenantId == programAddViewModel.programs.TenantId).OrderByDescending(x => x.ProgramId).FirstOrDefault();

        //    if (programData != null)
        //    {
        //        ProgramId = programData.ProgramId + 1;
        //    }
        //    else
        //    {
        //        ProgramId = 1;
        //    }

        //    programAddViewModel.programs.ProgramId = (int)ProgramId;
        //    programAddViewModel.programs.CreatedOn = DateTime.UtcNow;
        //    this.context?.Programs.Add(programAddViewModel.programs);
        //    this.context?.SaveChanges();
        //    programAddViewModel._failure = false;

        //    return programAddViewModel;
        //}

        /// <summary>
        /// Get All Program
        /// </summary>
        /// <param name="programListViewModel"></param>
        /// <returns></returns>
        public ProgramListViewModel GetAllProgram(ProgramListViewModel programListViewModel)
        {
            ProgramListViewModel programListModel = new ProgramListViewModel();
            try
            {

                var programList = this.context?.Programs.Where(x => x.TenantId == programListViewModel.TenantId && x.SchoolId == programListViewModel.SchoolId).ToList();

               
                programListModel._tenantName = programListViewModel._tenantName;
                programListModel._token = programListViewModel._token;

                if (programList!=null && programList.Count > 0)
                {
                    programListModel.ProgramList = programList;
                    programListModel._failure = false;
                }
                else
                {
                    programListModel._failure = true;
                    programListModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                programListModel._message = es.Message;
                programListModel._failure = true;
                programListModel._tenantName = programListViewModel._tenantName;
                programListModel._token = programListViewModel._token;
            }
            return programListModel;
        }

        /// <summary>
        /// Add/Update Program
        /// </summary>
        /// <param name="programAddViewModel"></param>
        /// <returns></returns>
        public ProgramListViewModel AddEditProgram(ProgramListViewModel programListViewModel)
        {
            ProgramListViewModel programUpdateModel = new ProgramListViewModel();
            try
            {
                foreach (var programLists in programListViewModel.ProgramList)
                {
                    if (programLists.ProgramId > 0)
                    {
                        var programUpdate = this.context?.Programs.FirstOrDefault(x => x.TenantId == programLists.TenantId && x.SchoolId == programLists.SchoolId && x.ProgramId == programLists.ProgramId);
                        if (programUpdate != null)
                        {
                            var program = this.context?.Programs.AsEnumerable().FirstOrDefault(x => x.TenantId == programLists.TenantId && x.SchoolId == programLists.SchoolId && x.ProgramId != programLists.ProgramId && String.Compare(x.ProgramName,programLists.ProgramName,true)==0);
                            if (program != null)
                            {
                                programUpdateModel._message = "Program Name already exists";
                                programUpdateModel._failure = true;
                                return programUpdateModel;
                            }
                            else
                            {
                                var courseList = this.context?.Course.AsEnumerable().Where(x => x.TenantId == programUpdate.TenantId && x.SchoolId == programUpdate.SchoolId && String.Compare(x.CourseProgram,programUpdate.ProgramName,true)==0).ToList();

                                programLists.CreatedBy = programUpdate.CreatedBy;
                                programLists.CreatedOn = programUpdate.CreatedOn;
                                programLists.UpdatedOn = DateTime.UtcNow;
                                this.context?.Entry(programUpdate).CurrentValues.SetValues(programLists);
                                this.context?.SaveChanges();

                                if (courseList!=null && courseList.Any())
                                {
                                    courseList.ForEach(x => x.CourseProgram = programLists.ProgramName);
                                }

                                programUpdateModel._message = "Program Updated Successfully";
                            }
                        }
                    }
                    else
                    {
                        int? ProgramId = 1;

                        var programData = this.context?.Programs.Where(x => x.SchoolId == programLists.SchoolId && x.TenantId == programLists.TenantId).OrderByDescending(x => x.ProgramId).FirstOrDefault();

                        if (programData != null)
                        {
                            ProgramId = programData.ProgramId + 1;
                        }

                        var program = this.context?.Programs.AsEnumerable().Where(x => x.SchoolId == programLists.SchoolId && x.TenantId == programLists.TenantId && String.Compare(x.ProgramName, programLists.ProgramName,true)==0).FirstOrDefault();
                        if (program != null)
                        {
                            programUpdateModel._failure = true;
                            programUpdateModel._message = "Program Name already exists";
                            return programUpdateModel;
                        }
                        programLists.ProgramId = (int)ProgramId;
                        programLists.CreatedOn = DateTime.UtcNow;
                        this.context?.Programs.AddRange(programLists);
                        programUpdateModel._message = "Program added successfully";
                    }
                }
                this.context?.SaveChanges();
                programUpdateModel._failure = false;
            }
            catch (Exception es)
            {

                programUpdateModel._message = es.Message;
                programUpdateModel._failure = true;
                programUpdateModel._tenantName = programListViewModel._tenantName;
                programUpdateModel._token = programListViewModel._token;
            }
            return programUpdateModel;
        }

        /// <summary>
        /// Delete Program
        /// </summary>
        /// <param name="programAddViewModel"></param>
        /// <returns></returns>
        public ProgramAddViewModel DeleteProgram(ProgramAddViewModel programAddViewModel)
        {
            try
            {
                var programDelete = this.context?.Programs.FirstOrDefault(x => x.TenantId == programAddViewModel.Programs!.TenantId && x.SchoolId == programAddViewModel.Programs.SchoolId && x.ProgramId == programAddViewModel.Programs.ProgramId);
                if(programDelete != null)
                {

                
                var CourseList = this.context?.Course.AsEnumerable().Where(e => String.Compare(e.CourseProgram,programDelete.ProgramName,true)==0 && e.SchoolId == programDelete.SchoolId && e.TenantId == programDelete.TenantId).ToList();

                if (CourseList!=null && CourseList.Any())
                {
                    programAddViewModel._message = "It Has Associationship";
                    programAddViewModel._failure = true;
                }
                else
                {
                    this.context?.Programs.Remove(programDelete);
                    this.context?.SaveChanges();
                    programAddViewModel._failure = false;
                    programAddViewModel._message = "Program deleted successfullyy";
                }
                }
            }
            catch (Exception es)
            {
                programAddViewModel._failure = true;
                programAddViewModel._message = es.Message;
            }
            return programAddViewModel;
        }

        /// <summary>
        /// Add Subject
        /// </summary>
        /// <param name="subjectAddViewModel"></param>
        /// <returns></returns>
        //public SubjectAddViewModel AddSubject(SubjectAddViewModel subjectAddViewModel)
        //{
        //    try
        //    {
        //        //int? MasterSubjectId = Utility.GetMaxPK(this.context, new Func<Subject, int>(x => x.SubjectId));
        //        int? MasterSubjectId = 1;

        //        var subjectData = this.context?.Subject.Where(x => x.SchoolId == subjectAddViewModel.subject.SchoolId && x.TenantId == subjectAddViewModel.subject.TenantId).OrderByDescending(x => x.SubjectId).FirstOrDefault();

        //        if (subjectData != null)
        //        {
        //            MasterSubjectId = subjectData.SubjectId + 1;
        //        }
        //        subjectAddViewModel.subject.SubjectId = (int)MasterSubjectId;
        //        subjectAddViewModel.subject.CreatedOn = DateTime.UtcNow;
        //        this.context?.Subject.Add(subjectAddViewModel.subject);
        //        this.context?.SaveChanges();
        //        subjectAddViewModel._failure = false;
        //    }
        //    catch (Exception es)
        //    {
        //        subjectAddViewModel._failure = true;
        //        subjectAddViewModel._message = es.Message;
        //    }
        //    return subjectAddViewModel;
        //}

        /// <summary>
        /// Add/Update Subject
        /// </summary>
        /// <param name="subjectListViewModel"></param>
        /// <returns></returns>
        public SubjectListViewModel AddEditSubject(SubjectListViewModel subjectListViewModel)
        {
            try
            {
                foreach (var subject in subjectListViewModel.SubjectList)
                {
                    if (subject.SubjectId > 0)
                    {
                        var SubjectUpdate = this.context?.Subject.FirstOrDefault(x => x.TenantId == subject.TenantId && x.SchoolId == subject.SchoolId && x.SubjectId == subject.SubjectId);

                        if (SubjectUpdate != null)
                        {
                            var subjectName = this.context?.Subject.AsEnumerable().FirstOrDefault(x => x.TenantId == subject.TenantId && x.SchoolId == subject.SchoolId && String.Compare(x.SubjectName,subject.SubjectName,true)==0 && x.SubjectId != subject.SubjectId && x.AcademicYear == SubjectUpdate.AcademicYear);

                            if (subjectName == null)
                            {
                                var sameSubjectNameExits = this.context?.Course.AsEnumerable().Where(x => x.SchoolId == SubjectUpdate.SchoolId && x.TenantId == SubjectUpdate.TenantId && String.Compare(x.CourseSubject,SubjectUpdate.SubjectName,true)==0).ToList();

                                if (sameSubjectNameExits!=null && sameSubjectNameExits.Any())
                                {
                                    sameSubjectNameExits.ForEach(x => x.CourseSubject = subject.SubjectName);
                                }

                                var subjectNameUsedInStaff = this.context?.StaffMaster.AsEnumerable().Where(x => x.SchoolId == subject.SchoolId && x.TenantId == subject.TenantId && String.Compare(x.PrimarySubjectTaught,SubjectUpdate.SubjectName,true)==0).ToList();

                                if (subjectNameUsedInStaff!=null && subjectNameUsedInStaff.Any())
                                {
                                    subjectNameUsedInStaff.ForEach(x => x.PrimarySubjectTaught = subject.SubjectName);
                                }

                                var StaffData = this.context?.StaffMaster.Where(x => x.SchoolId == subject.SchoolId && x.TenantId == subject.TenantId && (x.OtherSubjectTaught??"").ToLower().Contains((SubjectUpdate.SubjectName??"").ToLower())).ToList();

                                if (StaffData!=null && StaffData.Any())
                                {
                                    foreach (var staff in StaffData)
                                    {
                                        var otherSubjectTaught = staff.OtherSubjectTaught!.Split(",");
                                        otherSubjectTaught = otherSubjectTaught.Where(w => w != SubjectUpdate.SubjectName).ToArray();
                                        var newOtherSubjectTaught = string.Join(",", otherSubjectTaught);
                                        newOtherSubjectTaught = newOtherSubjectTaught + "," + subject.SubjectName;
                                        staff.OtherSubjectTaught = newOtherSubjectTaught;
                                    }
                                }

                                subject.CreatedBy = SubjectUpdate.CreatedBy;
                                subject.CreatedOn = SubjectUpdate.CreatedOn;
                                subject.UpdatedOn = DateTime.UtcNow;
                                this.context?.Entry(SubjectUpdate).CurrentValues.SetValues(subject);
                                this.context?.SaveChanges();
                                subjectListViewModel._message = "Subject Updated Successfully";
                            }
                            else
                            {
                                subjectListViewModel._failure = true;
                                subjectListViewModel._message = "Subject Name Already Exits";
                                return subjectListViewModel;
                            }
                        }
                        else
                        {
                            subjectListViewModel._failure = true;
                            subjectListViewModel._message = NORECORDFOUND;
                        }
                    }
                    else
                    {
                        subject.AcademicYear = Utility.GetCurrentAcademicYear(this.context!, subject.TenantId, subject.SchoolId);

                        var subjectName = this.context?.Subject.AsEnumerable().FirstOrDefault(x => x.TenantId == subject.TenantId && x.SchoolId == subject.SchoolId && String.Compare(x.SubjectName,subject.SubjectName,true)==0 && x.AcademicYear == subject.AcademicYear);

                        if (subjectName == null)
                        {
                            int? SubjectId = 1;

                            var subjectData = this.context?.Subject.Where(x => x.SchoolId == subject.SchoolId && x.TenantId == subject.TenantId).OrderByDescending(x => x.SubjectId).FirstOrDefault();

                            if (subjectData != null)
                            {
                                SubjectId = subjectData.SubjectId + 1;
                            }


                            subject.SubjectId = (int)SubjectId;
                            subject.CreatedOn = DateTime.UtcNow;
                            this.context?.Subject.Add(subject);
                        }
                        else
                        {
                            subjectListViewModel._failure = true;
                            subjectListViewModel._message = "Subject Name Already Exits";
                            return subjectListViewModel;
                        }

                        subjectListViewModel._message = "Subject added successfully";
                    }
                }
                this.context?.SaveChanges();
                subjectListViewModel._failure = false;
            }
            catch (Exception es)
            {
                subjectListViewModel._failure = true;
                subjectListViewModel._message = es.Message;
            }
            return subjectListViewModel;
        }

        /// <summary>
        /// Get All Subject List
        /// </summary>
        /// <param name="subjectListViewModel"></param>
        /// <returns></returns>
        public SubjectListViewModel GetAllSubjectList(SubjectListViewModel subjectListViewModel)
        {
            SubjectListViewModel subjectList = new SubjectListViewModel();
            try
            {
                var Subjectdata = this.context?.Subject.Where(x => x.TenantId == subjectListViewModel.TenantId && x.SchoolId == subjectListViewModel.SchoolId && x.AcademicYear == subjectListViewModel.AcademicYear).ToList();
                
               
                subjectList._tenantName = subjectListViewModel._tenantName;
                subjectList._token = subjectListViewModel._token;

                if (Subjectdata != null && Subjectdata.Any())
                {
                    subjectList.SubjectList = Subjectdata;
                    subjectList._failure = false;
                }
                else
                {
                    subjectList._failure = true;
                    subjectList._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                subjectList._message = es.Message;
                subjectList._failure = true;
                subjectList._tenantName = subjectListViewModel._tenantName;
                subjectList._token = subjectListViewModel._token;
            }
            return subjectList;
        }

        /// <summary>
        /// Delete Subject
        /// </summary>
        /// <param name="subjectAddViewModel"></param>
        /// <returns></returns>
        public SubjectAddViewModel DeleteSubject(SubjectAddViewModel subjectAddViewModel)
        {
            try
            {
                var subjectDelete = this.context?.Subject.FirstOrDefault(x => x.TenantId == subjectAddViewModel.Subject!.TenantId && x.SchoolId == subjectAddViewModel.Subject.SchoolId && x.SubjectId == subjectAddViewModel.Subject.SubjectId);

                if (subjectDelete != null)
                {
                    var courseExits = this.context?.Course.FirstOrDefault(x => x.TenantId == subjectDelete.TenantId && x.SchoolId == subjectDelete.SchoolId && x.CourseSubject == subjectDelete.SubjectName);
                    if (courseExits != null)
                    {
                        subjectAddViewModel._failure = true;
                        subjectAddViewModel._message = "Cannot delete because it has association.";
                    }
                    else
                    {
                        this.context?.Subject.Remove(subjectDelete);
                        this.context?.SaveChanges();
                        subjectAddViewModel._failure = false;
                        subjectAddViewModel._message = "Subject deleted successfullyy";
                    }
                }
            }
            catch (Exception es)
            {
                subjectAddViewModel._failure = true;
                subjectAddViewModel._message = es.Message;
            }
            return subjectAddViewModel;
        }

        /// <summary>
        /// Add Course
        /// </summary>
        /// <param name="courseAddViewModel"></param>
        /// <returns></returns>
        public CourseAddViewModel AddCourse(CourseAddViewModel courseAddViewModel)
        {
            try
            {
                if (courseAddViewModel.ProgramId == 0)
                {
                    int? ProgramId = 1;

                    var programData = this.context?.Programs.Where(x => x.TenantId == courseAddViewModel.Course!.TenantId && x.SchoolId == courseAddViewModel.Course.SchoolId).OrderByDescending(x => x.ProgramId).FirstOrDefault();

                    if (programData != null)
                    {
                        ProgramId = programData.ProgramId + 1;
                    }

                    var programName = this.context?.Programs.AsEnumerable().FirstOrDefault(x => x.SchoolId == courseAddViewModel.Course!.SchoolId && x.TenantId == courseAddViewModel.Course!.TenantId && String.Compare(x.ProgramName,courseAddViewModel.Course.CourseProgram,true)==0);
                    if (programName != null)
                    {
                        courseAddViewModel._failure = true;
                        courseAddViewModel._message = "Program Name already exists";
                        return courseAddViewModel;
                    }
                    var programAdd = new Programs() { TenantId = courseAddViewModel.Course!.TenantId, SchoolId = courseAddViewModel.Course.SchoolId, ProgramId = (int)ProgramId, ProgramName = courseAddViewModel.Course.CourseProgram, CreatedOn = DateTime.UtcNow, CreatedBy = courseAddViewModel.Course.CreatedBy };
                    this.context?.Programs.Add(programAdd);
                }
                if (courseAddViewModel.SubjectId == 0)
                {
                    int? SubjectId = 1;

                    var subjectData = this.context?.Subject.Where(x => x.TenantId == courseAddViewModel.Course!.TenantId && x.SchoolId == courseAddViewModel.Course.SchoolId).OrderByDescending(x => x.SubjectId).FirstOrDefault();

                    if (subjectData != null)
                    {
                        SubjectId = subjectData.SubjectId + 1;
                    }

                    var subjectName = this.context?.Subject.AsEnumerable().FirstOrDefault(x => x.SchoolId == courseAddViewModel.Course!.SchoolId && x.TenantId == courseAddViewModel.Course.TenantId && String.Compare(x.SubjectName,courseAddViewModel.Course.CourseSubject,true)==0);
                    if (subjectName != null)
                    {
                        courseAddViewModel._failure = true;
                        courseAddViewModel._message = "Subject Name already exists";
                        return courseAddViewModel;
                    }
                    var subjectAdd = new Subject() { TenantId = courseAddViewModel.Course!.TenantId, SchoolId = courseAddViewModel.Course.SchoolId, SubjectId = (int)SubjectId, SubjectName = courseAddViewModel.Course.CourseSubject, CreatedOn = DateTime.UtcNow, CreatedBy = courseAddViewModel.Course.CreatedBy, AcademicYear = Utility.GetCurrentAcademicYear(this.context!, courseAddViewModel.Course.TenantId, courseAddViewModel.Course.SchoolId)};
                    this.context?.Subject.Add(subjectAdd);
                }

                var courseTitle = this.context?.Course.AsEnumerable().FirstOrDefault(x => x.TenantId == courseAddViewModel.Course!.TenantId && x.SchoolId == courseAddViewModel.Course.SchoolId && String.Compare(x.CourseTitle,courseAddViewModel.Course.CourseTitle,true)==0);

                if (courseTitle == null)
                {
                    int? CourseId = 1;

                    var courseData = this.context?.Course.Where(x => x.TenantId == courseAddViewModel.Course!.TenantId && x.SchoolId == courseAddViewModel.Course.SchoolId).OrderByDescending(x => x.CourseId).FirstOrDefault();

                    if (courseData != null)
                    {
                        CourseId = courseData.CourseId + 1;
                    }
                    courseAddViewModel.Course!.CourseId = (int)CourseId;
                    courseAddViewModel.Course.CreatedOn = DateTime.UtcNow;
                    courseAddViewModel.Course.IsCourseActive = true;
                    courseAddViewModel.Course.AcademicYear = Utility.GetCurrentAcademicYear(this.context!, courseAddViewModel.Course.TenantId, courseAddViewModel.Course.SchoolId);

                    if (courseAddViewModel.Course.CourseStandard.ToList().Any())
                    {
                        courseAddViewModel.Course.CourseStandard.ToList().ForEach(x => x.CreatedOn = DateTime.UtcNow);
                    }

                    this.context?.Course.Add(courseAddViewModel.Course);
                }
                else
                {
                    courseAddViewModel._failure = true;
                    courseAddViewModel._message = "Course Title Already Exits";
                    return courseAddViewModel;
                }
                this.context?.SaveChanges();
                courseAddViewModel._failure = false;
                courseAddViewModel._message = "Course added successfully";
            }
            catch (Exception es)
            {
                courseAddViewModel._failure = true;
                courseAddViewModel._message = es.Message;
            }
            return courseAddViewModel;
        }

        /// <summary>
        /// Update Course
        /// </summary>
        /// <param name="courseAddViewModel"></param>
        /// <returns></returns>
        public CourseAddViewModel UpdateCourse(CourseAddViewModel courseAddViewModel)
        {
            try
            {
                if (courseAddViewModel.ProgramId == 0)
                {
                    int? ProgramId = 1;

                    var programData = this.context?.Programs.Where(x => x.TenantId == courseAddViewModel.Course!.TenantId && x.SchoolId == courseAddViewModel.Course.SchoolId).OrderByDescending(x => x.ProgramId).FirstOrDefault();

                    if (programData != null)
                    {
                        ProgramId = programData.ProgramId + 1;
                    }

                    var programName = this.context?.Programs.AsEnumerable().FirstOrDefault(x => x.SchoolId == courseAddViewModel.Course!.SchoolId && x.TenantId == courseAddViewModel.Course.TenantId && String.Compare(x.ProgramName,courseAddViewModel.Course.CourseProgram,true)==0);

                    if (programName != null)
                    {
                        courseAddViewModel._failure = true;
                        courseAddViewModel._message = "Program Name already exists";
                        return courseAddViewModel;
                    }

                    var programAdd = new Programs() { TenantId = courseAddViewModel.Course!.TenantId, SchoolId = courseAddViewModel.Course.SchoolId, ProgramId = (int)ProgramId, ProgramName = courseAddViewModel.Course.CourseProgram, CreatedOn = DateTime.UtcNow, CreatedBy = courseAddViewModel.Course.CreatedBy };
                    this.context?.Programs.Add(programAdd);
                }

                if (courseAddViewModel.SubjectId == 0)
                {
                    int? SubjectId = 1;

                    var subjectData = this.context?.Subject.Where(x => x.TenantId == courseAddViewModel.Course!.TenantId && x.SchoolId == courseAddViewModel.Course.SchoolId).OrderByDescending(x => x.SubjectId).FirstOrDefault();

                    if (subjectData != null)
                    {
                        SubjectId = subjectData.SubjectId + 1;
                    }

                    var subjectName = this.context?.Subject.AsEnumerable().FirstOrDefault(x => x.SchoolId == courseAddViewModel.Course!.SchoolId && x.TenantId == courseAddViewModel.Course.TenantId && String.Compare(x.SubjectName,courseAddViewModel.Course.CourseSubject,true)==0);
                    if (subjectName != null)
                    {
                        courseAddViewModel._failure = true;
                        courseAddViewModel._message = "Subject Name already exists";
                        return courseAddViewModel;
                    }

                    var subjectAdd = new Subject() { TenantId = courseAddViewModel.Course!.TenantId, SchoolId = courseAddViewModel.Course.SchoolId, SubjectId = (int)SubjectId, SubjectName = courseAddViewModel.Course.CourseSubject, CreatedOn = DateTime.UtcNow, CreatedBy = courseAddViewModel.Course.CreatedBy, AcademicYear = Utility.GetCurrentAcademicYear(this.context!, courseAddViewModel.Course.TenantId, courseAddViewModel.Course.SchoolId) };
                    this.context?.Subject.Add(subjectAdd);
                }

                var courseUpdate = this.context?.Course.Include(x => x.CourseStandard).FirstOrDefault(x => x.TenantId == courseAddViewModel.Course!.TenantId && x.SchoolId == courseAddViewModel.Course.SchoolId && x.CourseId == courseAddViewModel.Course.CourseId);

                if (courseUpdate != null)
                {
                    var courseTitle = this.context?.Course.AsEnumerable().FirstOrDefault(x => x.TenantId == courseAddViewModel.Course!.TenantId && x.SchoolId == courseAddViewModel.Course.SchoolId && String.Compare(x.CourseTitle,courseAddViewModel.Course.CourseTitle,true)==0 && x.CourseId != courseAddViewModel.Course.CourseId);

                    if (courseTitle == null)
                    {
                        courseAddViewModel.Course!.CreatedBy = courseUpdate.CreatedBy;
                        courseAddViewModel.Course.CreatedOn = courseUpdate.CreatedOn;
                        courseAddViewModel.Course.UpdatedOn = DateTime.UtcNow;
                        courseAddViewModel.Course.AcademicYear = courseUpdate.AcademicYear;
                        this.context?.Entry(courseUpdate).CurrentValues.SetValues(courseAddViewModel.Course);
                        courseAddViewModel._message = "Course Updated Successfully";

                        if (courseAddViewModel.Course.CourseStandard.ToList().Count > 0)
                        {
                            this.context?.CourseStandard.RemoveRange(courseUpdate.CourseStandard);
                            courseAddViewModel.Course.CourseStandard.ToList().ForEach(x => x.UpdatedOn = DateTime.UtcNow);
                            this.context?.CourseStandard.AddRange(courseAddViewModel.Course.CourseStandard);
                        }
                        if (courseUpdate.CourseStandard.Count > 0 && courseAddViewModel.Course.CourseStandard.ToList().Count == 0)
                        {
                            this.context?.CourseStandard.RemoveRange(courseUpdate.CourseStandard);
                        }
                    }
                    else
                    {
                        courseAddViewModel._failure = true;
                        courseAddViewModel._message = "Course Title Already Exits";
                        return courseAddViewModel;
                    }
                }
                this.context?.SaveChanges();
                courseAddViewModel._failure = false;
            }
            catch (Exception es)
            {
                courseAddViewModel._failure = true;
                courseAddViewModel._message = es.Message;
            }
            return courseAddViewModel;
        }

        /// <summary>
        /// Delete Course
        /// </summary>
        /// <param name="courseAddViewModel"></param>
        /// <returns></returns>
        public CourseAddViewModel DeleteCourse(CourseAddViewModel courseAddViewModel)
        {
            try
            {
                var courseDelete = this.context?.Course.Include(x => x.CourseStandard).Include(x => x.CourseSection).FirstOrDefault(x => x.TenantId == courseAddViewModel.Course!.TenantId && x.SchoolId == courseAddViewModel.Course.SchoolId && x.CourseId == courseAddViewModel.Course.CourseId);
                var courseCommentCategory = this.context?.CourseCommentCategory.Where(x => x.TenantId == courseAddViewModel.Course!.TenantId && x.SchoolId == courseAddViewModel.Course.SchoolId && x.CourseId == courseAddViewModel.Course.CourseId).ToList();

                if (courseDelete != null)
                {
                    if (courseDelete.CourseSection.Any() || (courseCommentCategory!=null && courseCommentCategory.Any()))
                    {
                        courseAddViewModel._failure = true;
                        courseAddViewModel._message = "Course Can't be deleted because it has association";
                        return courseAddViewModel;
                    }
                    this.context?.CourseStandard.RemoveRange(courseDelete.CourseStandard);
                    this.context?.Course.Remove(courseDelete);
                    this.context?.SaveChanges();
                    courseAddViewModel._failure = false;
                    courseAddViewModel._message = "deleted successfullyy";
                }
                else
                {
                    courseAddViewModel._failure = true;
                    courseAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                courseAddViewModel._failure = true;
                courseAddViewModel._message = es.Message;
            }
            return courseAddViewModel;
        }

        /// <summary>
        /// Get All Course List
        /// </summary>
        /// <param name="courseListViewModel"></param>
        /// <returns></returns>
        public CourseListViewModel GetAllCourseList(CourseListViewModel courseListViewModel)
        {
            CourseListViewModel courseListModel = new CourseListViewModel();

            if (courseListViewModel.ScheduleReport != true)
            {
                try
                {
                    var courseRecords = this.context?.Course.Include(x => x.CourseSection).Include(e => e.CourseStandard).ThenInclude(c => c.GradeUsStandard).Where(x => x.TenantId == courseListViewModel.TenantId && x.SchoolId == courseListViewModel.SchoolId && x.AcademicYear == courseListViewModel.AcademicYear).ToList();

                    if (courseRecords != null && courseRecords.Any())
                    {
                        foreach (var course in courseRecords)
                        {
                            CourseViewModel courseViewModel = new CourseViewModel { Course = course, CourseSectionCount = course.CourseSection.Count };
                            courseListModel.CourseViewModelList.Add(courseViewModel);
                        }
                        courseListModel.CourseCount = courseRecords.Count;

                        courseListModel._failure = false;
                    }
                    else
                    {
                        courseListModel.CourseCount = null;
                        courseListModel._failure = true;
                        courseListModel._message = NORECORDFOUND;
                    }
                    courseListModel._tenantName = courseListViewModel._tenantName;
                    courseListModel._token = courseListViewModel._token;
                }
                catch (Exception es)
                {
                    courseListModel.CourseViewModelList = new();
                    courseListModel._message = es.Message;
                    courseListModel._failure = true;
                    courseListModel._tenantName = courseListViewModel._tenantName;
                    courseListModel._token = courseListViewModel._token;
                }
            }
            else
            {
                try
                {

                    var courseRecords = this.context?.Course.Include(x => x.CourseSection).Include(e => e.CourseStandard).ThenInclude(c => c.GradeUsStandard).Where(x => x.TenantId == courseListViewModel.TenantId && x.SchoolId == courseListViewModel.SchoolId && x.AcademicYear == courseListViewModel.AcademicYear).ToList();

                    if (courseRecords != null && courseRecords.Any())
                    {
                        foreach (var course in courseRecords)
                        {
                            int? totalSeats = 0;
                            int? totalScheduleStudent = 0;

                            foreach (var cs in course.CourseSection)
                            {
                                var seats = cs.Seats;

                                var scheduleStudents = this.context?.StudentCoursesectionSchedule.Where(x => x.TenantId == courseListViewModel.TenantId && x.SchoolId == courseListViewModel.SchoolId && x.CourseId == cs.CourseId && x.CourseSectionId == cs.CourseSectionId && x.IsDropped != true).ToList().Count;
                                totalSeats = totalSeats + seats;
                                totalScheduleStudent = totalScheduleStudent + scheduleStudents;
                            }
                            var availableSeats = totalSeats - totalScheduleStudent;
                            var totalCourseSection = course.CourseSection.Count();

                            course.CourseSection = new HashSet<CourseSection>();
                            course.CourseStandard = new HashSet<CourseStandard>();

                            CourseViewModel courseViewModel = new CourseViewModel { Course = course, CourseSectionCount = totalCourseSection, ScheduleStudent = totalScheduleStudent, AvailableSeats = availableSeats, TotalSeats = totalSeats };
                            courseListModel.CourseViewModelList.Add(courseViewModel);
                        }
                       
                        courseListModel.CourseCount = courseRecords.Count;

                        courseListModel._failure = false;
                    }
                    else
                    {
                        courseListModel.CourseCount = null;
                        courseListModel._failure = true;
                        courseListModel._message = NORECORDFOUND;
                    }
                    courseListModel._tenantName = courseListViewModel._tenantName;
                    courseListModel._token = courseListViewModel._token;
                }
                catch (Exception es)
                {
                    courseListModel.CourseViewModelList = new();
                    courseListModel._message = es.Message;
                    courseListModel._failure = true;
                    courseListModel._tenantName = courseListViewModel._tenantName;
                    courseListModel._token = courseListViewModel._token;
                }
            }

            return courseListModel;
        }

        /// <summary>
        /// Add Course Section
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <returns></returns>
        public CourseSectionAddViewModel AddCourseSection(CourseSectionAddViewModel courseSectionAddViewModel)
        {
            using var transaction = this.context?.Database.BeginTransaction();
            try
            {
                decimal? academicYear = null;

                academicYear = Utility.GetCurrentAcademicYear(this.context!, courseSectionAddViewModel.CourseSection!.TenantId, courseSectionAddViewModel.CourseSection.SchoolId);

                var CalenderData = this.context?.SchoolCalendars.FirstOrDefault(x => x.CalenderId == courseSectionAddViewModel.CourseSection!.CalendarId && x.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && x.TenantId == courseSectionAddViewModel.CourseSection.TenantId);

                if (CalenderData != null)
                {
                    //academicYear = (decimal)CalenderData.AcademicYear;

                    if (!(bool)courseSectionAddViewModel.CourseSection?.DurationBasedOnPeriod!)
                    {
                        if (CalenderData.StartDate > courseSectionAddViewModel.CourseSection.DurationStartDate || CalenderData.EndDate < courseSectionAddViewModel.CourseSection.DurationEndDate)
                        {
                            courseSectionAddViewModel._message = "Start Date And End Date of Course Section Should Be Between Start Date And End Date of School Calender";
                            courseSectionAddViewModel._failure = true;
                            return courseSectionAddViewModel;
                        }
                    }
                    var checkCourseSectionName = this.context?.CourseSection.AsEnumerable().Where(x => x.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && x.TenantId == courseSectionAddViewModel.CourseSection.TenantId && String.Compare(x.CourseSectionName ,courseSectionAddViewModel.CourseSection.CourseSectionName,true)==0 && x.AcademicYear == academicYear).FirstOrDefault();

                    if (checkCourseSectionName != null)
                    {
                        courseSectionAddViewModel._failure = true;
                        courseSectionAddViewModel._message = "Course Section Name already exists";
                        return courseSectionAddViewModel;
                    }

                    int? CourseSectionId = 1;

                    var CourseSectionData = this.context?.CourseSection.Where(x => x.TenantId == courseSectionAddViewModel.CourseSection.TenantId && x.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId /*&& x.AcademicYear== academicYear*/).OrderByDescending(x => x.CourseSectionId).FirstOrDefault();


                    if (CourseSectionData != null)
                    {
                        CourseSectionId = CourseSectionData.CourseSectionId + 1;
                    }

                    if (courseSectionAddViewModel.MarkingPeriodId != null)
                    {
                        string[] markingPeriodID = courseSectionAddViewModel.MarkingPeriodId.Split("_");

                        if (markingPeriodID.First() == "0")
                        {
                            courseSectionAddViewModel.CourseSection.YrMarkingPeriodId = Convert.ToInt32(markingPeriodID[1]);
                        }

                        if (markingPeriodID.First() == "1")
                        {
                            courseSectionAddViewModel.CourseSection.SmstrMarkingPeriodId = Convert.ToInt32(markingPeriodID[1]);
                        }

                        if (markingPeriodID.First() == "2")
                        {
                            courseSectionAddViewModel.CourseSection.QtrMarkingPeriodId = Convert.ToInt32(markingPeriodID[1]);
                        }

                        if (markingPeriodID.First() == "3")
                        {
                            courseSectionAddViewModel.CourseSection.PrgrsprdMarkingPeriodId = Convert.ToInt32(markingPeriodID[1]);
                        }
                    }
                    courseSectionAddViewModel._message = "Course Section added successfully.";

                    switch ((courseSectionAddViewModel.CourseSection.ScheduleType ?? "").ToLower())
                    {
                        case "fixedschedule":

                            if (AddFixedSchedule(courseSectionAddViewModel, CourseSectionId, academicYear)._failure)
                            {
                                return courseSectionAddViewModel;
                            }
                            break;

                        case "variableschedule":

                            if (AddVariableSchedule(courseSectionAddViewModel, CourseSectionId, academicYear)._failure)
                            {
                                return courseSectionAddViewModel;
                            }
                            break;

                        case "calendarschedule":

                            if (AddCalendarSchedule(courseSectionAddViewModel, CourseSectionId, academicYear)._failure)
                            {
                                return courseSectionAddViewModel;
                            }
                            break;

                        case "blockschedule":

                            if (AddBlockSchedule(courseSectionAddViewModel, CourseSectionId, academicYear)._failure)
                            {
                                return courseSectionAddViewModel;
                            }
                            break;

                        default:
                            courseSectionAddViewModel._failure = true;
                            courseSectionAddViewModel._message = "Please Provide a Valid Schedule Type.";
                            return courseSectionAddViewModel;
                    }

                    courseSectionAddViewModel.CourseSection.CourseSectionId = (int)CourseSectionId;
                    courseSectionAddViewModel.CourseSection.AcademicYear = academicYear;
                    courseSectionAddViewModel.CourseSection.CreatedOn = DateTime.UtcNow;
                    this.context?.CourseSection.Add(courseSectionAddViewModel.CourseSection);
                    this.context?.SaveChanges();

                    courseSectionAddViewModel._failure = false;
                    transaction?.Commit();
                }
                else
                {
                    courseSectionAddViewModel._failure = true;
                    courseSectionAddViewModel._message = "School Calender does not exist";
                }
            }
            catch (Exception es)
            {
                transaction?.Rollback();
                courseSectionAddViewModel._message = es.Message;
                courseSectionAddViewModel._failure = true;
            }
            return courseSectionAddViewModel;
        }

        /// <summary>
        /// Get All CourseSection
        /// </summary>
        /// <param name="courseSectionViewModel"></param>
        /// <returns></returns>
        public CourseSectionViewModel GetAllCourseSection(CourseSectionViewModel courseSectionViewModel)
        {
            CourseSectionViewModel courseSectionView = new ();
            try
            {
                var courseSectionData = this.context?.CourseSection.Include(x => x.Course).Include(x => x.AttendanceCodeCategories).Include(x => x.GradeScale).Include(x => x.SchoolCalendars).Include(x => x.SchoolYears).Include(x => x.Semesters).Include(x => x.Quarters).Include(x => x.ProgressPeriods).Where(x => x.TenantId == courseSectionViewModel.TenantId && x.SchoolId == courseSectionViewModel.SchoolId && x.CourseId == courseSectionViewModel.CourseId && x.AcademicYear == courseSectionViewModel.AcademicYear).ToList().Select(cs => new CourseSection
                {
                    TenantId = cs.TenantId,
                    SchoolId = cs.SchoolId,
                    CourseId = cs.CourseId,
                    CourseSectionId = cs.CourseSectionId,
                    GradeScaleId = cs.GradeScaleId,
                    CourseSectionName = cs.CourseSectionName,
                    CalendarId = cs.CalendarId,
                    AttendanceCategoryId = cs.AttendanceCategoryId,
                    CreditHours = cs.CreditHours,
                    Seats = cs.Seats,
                    IsWeightedCourse = cs.IsWeightedCourse,
                    AffectsClassRank = cs.AffectsClassRank,
                    AffectsHonorRoll = cs.AffectsHonorRoll,
                    OnlineClassRoom = cs.OnlineClassRoom,
                    OnlineClassroomUrl = cs.OnlineClassroomUrl,
                    OnlineClassroomPassword = cs.OnlineClassroomPassword,
                    StandardGradeScaleId = cs.StandardGradeScaleId,
                    UseStandards = cs.UseStandards,
                    DurationBasedOnPeriod = cs.DurationBasedOnPeriod,
                    DurationStartDate = cs.DurationStartDate,
                    DurationEndDate = cs.DurationEndDate,
                    ScheduleType = cs.ScheduleType,
                    MeetingDays = cs.MeetingDays,
                    AttendanceTaken = cs.AttendanceTaken,
                    IsActive = cs.IsActive,
                    CreatedBy = cs.CreatedBy ,
                    CreatedOn = cs.CreatedOn,
                    UpdatedBy = cs.UpdatedBy, 
                    UpdatedOn = cs.UpdatedOn,
                    QtrMarkingPeriodId = cs.QtrMarkingPeriodId,
                    SmstrMarkingPeriodId = cs.SmstrMarkingPeriodId,
                    YrMarkingPeriodId = cs.YrMarkingPeriodId,
                    PrgrsprdMarkingPeriodId= cs.PrgrsprdMarkingPeriodId,
                    AcademicYear = cs.AcademicYear,
                    GradeScaleType = cs.GradeScaleType,
                    AllowStudentConflict = cs.AllowStudentConflict,
                    AllowTeacherConflict = cs.AllowTeacherConflict,
                    AllowRoomConflict = cs.AllowRoomConflict,
                    Course = new Course { CourseTitle = cs.Course.CourseTitle, CourseProgram = cs.Course.CourseProgram, CourseSubject = cs.Course.CourseSubject },
                    GradeScale = cs.GradeScale != null ? new GradeScale { GradeScaleName = cs.GradeScale.GradeScaleName } : null,
                    AttendanceCodeCategories = cs.AttendanceCodeCategories != null ? new AttendanceCodeCategories { Title = cs.AttendanceCodeCategories.Title } : null,
                    SchoolCalendars = cs.SchoolCalendars != null ? new SchoolCalendars { TenantId = cs.SchoolCalendars.TenantId, SchoolId = cs.SchoolCalendars.SchoolId, CalenderId = cs.SchoolCalendars.CalenderId, Title = cs.SchoolCalendars.Title, StartDate = cs.SchoolCalendars.StartDate, EndDate = cs.SchoolCalendars.EndDate, AcademicYear = cs.SchoolCalendars.AcademicYear, DefaultCalender = cs.SchoolCalendars.DefaultCalender, Days = cs.SchoolCalendars.Days, RolloverId = cs.SchoolCalendars.RolloverId, VisibleToMembershipId = cs.SchoolCalendars.VisibleToMembershipId, UpdatedOn = cs.SchoolCalendars.UpdatedOn, UpdatedBy = cs.UpdatedBy,
                        CreatedBy = cs.CreatedBy,
                        CreatedOn = cs.CreatedOn,
                    } : null,
                    ProgressPeriods = cs.ProgressPeriods != null ? new ProgressPeriods { Title = cs.ProgressPeriods.Title, StartDate = cs.ProgressPeriods.StartDate, EndDate = cs.ProgressPeriods.EndDate, } : null,
                    Quarters = cs.Quarters != null ? new Quarters { Title = cs.Quarters.Title, StartDate = cs.Quarters.StartDate, EndDate = cs.Quarters.EndDate, } : null,
                    Semesters = cs.Semesters != null ? new Semesters { Title = cs.Semesters.Title, StartDate = cs.Semesters.StartDate, EndDate = cs.Semesters.EndDate, } : null,
                    SchoolYears = cs.SchoolYears != null ? new SchoolYears { Title = cs.SchoolYears.Title, StartDate = cs.SchoolYears.StartDate, EndDate = cs.SchoolYears.EndDate, } : null
                });


                if (courseSectionData!=null &&  courseSectionData.Any())
                {
                    string? markingId = null;
                    string? standardGradeScaleName = null;

                    foreach (var courseSection in courseSectionData)
                    {
                        List<DateTime> holidayList = new ();
                        int? totalStaff = null;
                        int? totalStudent = null;
                        string? staffFullName = null;
                        if (courseSection.UseStandards == true)
                        {
                            //var gradeUsStandardData = this.context?.GradeUsStandard.Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.GradeStandardId == courseSection.StandardGradeScaleId).FirstOrDefault();
                            //if (gradeUsStandardData != null)
                            //{
                            //    standardRefNo = gradeUsStandardData.StandardRefNo;
                            //}

                            var StandardData = this.context?.GradeScale.Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.GradeScaleId == courseSection.StandardGradeScaleId).FirstOrDefault();
                            if (StandardData != null)
                            {
                                standardGradeScaleName = StandardData.GradeScaleName;
                            }
                        }
                        else
                        {
                            standardGradeScaleName = null;
                        }
                        if (courseSection.YrMarkingPeriodId != null)
                        {
                            markingId = "0" + "_" + courseSection.YrMarkingPeriodId;
                        }
                        if (courseSection.SmstrMarkingPeriodId != null)
                        {
                            markingId = "1" + "_" + courseSection.SmstrMarkingPeriodId;
                        }
                        if (courseSection.QtrMarkingPeriodId != null)
                        {
                            markingId = "2" + "_" + courseSection.QtrMarkingPeriodId;
                        }
                        if (courseSection.PrgrsprdMarkingPeriodId != null)
                        {
                            markingId = "3" + "_" + courseSection.PrgrsprdMarkingPeriodId;
                        }
                        if (courseSection.QtrMarkingPeriodId == null && courseSection.SmstrMarkingPeriodId == null && courseSection.YrMarkingPeriodId == null && courseSection.PrgrsprdMarkingPeriodId == null)
                        {
                            markingId = null;
                        }

                        //totalStaff = this.context.StaffCoursesectionSchedule.Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseId == courseSection.CourseId && x.CourseSectionId == courseSection.CourseSectionId && x.IsDropped != false).ToList().Count();

                        //totalStudent = this.context.StudentCoursesectionSchedule.Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseId == courseSection.CourseId && x.CourseSectionId == courseSection.CourseSectionId && x.IsDropped != true ).ToList().Count;

                        totalStudent = this.context?.StudentCoursesectionSchedule.Include(s => s.StudentMaster).Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseId == courseSection.CourseId && x.CourseSectionId == courseSection.CourseSectionId && x.IsDropped != true /*&& (x.EffectiveDropDate == null || x.EffectiveDropDate.Value.Date >= DateTime.Today.Date)*/&& x.StudentMaster.IsActive == true).ToList().Count;                        

                        var staffData = this.context?.StaffCoursesectionSchedule.Include(x => x.StaffMaster).Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseId == courseSection.CourseId && x.CourseSectionId == courseSection.CourseSectionId && x.IsDropped != true).ToList();
                        if (staffData!=null && staffData.Any())
                        {
                          
                            staffFullName = $"{staffData!.FirstOrDefault()!.StaffMaster.FirstGivenName} { (staffData!.FirstOrDefault()!.StaffMaster.MiddleName == null ? " " : $"{staffData!.FirstOrDefault()!.StaffMaster.MiddleName} ")}{staffData!.FirstOrDefault()!.StaffMaster.LastFamilyName}";
                            totalStaff = staffData.Count;
                        }

                        //Count Holiday between courseSection date range
                        var CalendarEventsData = this.context?.CalendarEvents.Where(e => e.TenantId == courseSection.TenantId && e.CalendarId == courseSection.CalendarId && (e.StartDate >= courseSection.DurationStartDate && e.StartDate <= courseSection.DurationEndDate || e.EndDate >= courseSection.DurationStartDate && e.EndDate <= courseSection.DurationEndDate) && e.IsHoliday == true && (e.SchoolId == courseSection.SchoolId || e.ApplicableToAllSchool == true)).ToList();
                        if (CalendarEventsData!=null && CalendarEventsData.Any())
                        {
                            foreach (var calender in CalendarEventsData)
                            {
                                if(calender.EndDate!=null && calender.StartDate != null)
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
                            holidayList.Distinct();
                        }


                        if (String.Compare(courseSection.ScheduleType,"Fixed Schedule (1)",true)==0)
                        {
                            var fixedScheduleData = this.context?.CourseFixedSchedule.Include(f => f.Rooms).Include(f => f.BlockPeriod).Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseId == courseSection.CourseId && x.CourseSectionId == courseSection.CourseSectionId).Select(s => new CourseFixedSchedule { TenantId = s.TenantId, SchoolId = s.SchoolId, CourseId = s.CourseId, CourseSectionId = s.CourseSectionId, GradeScaleId = s.GradeScaleId, Serial = s.Serial, RoomId = s.RoomId, PeriodId = s.PeriodId, BlockId = s.BlockId, CreatedBy = s.CreatedBy, CreatedOn = s.CreatedOn, UpdatedBy = s.UpdatedBy, UpdatedOn = s.UpdatedOn, Rooms = new Rooms { Title = s.Rooms!.Title }, BlockPeriod = new BlockPeriod { PeriodTitle = s.BlockPeriod!.PeriodTitle } }).FirstOrDefault();
                            //if (fixedScheduleData != null)
                            //{                         
                            courseSection.ScheduleType = "Fixed Schedule";
                            GetCourseSectionForView getFixedSchedule = new GetCourseSectionForView
                            {
                                CourseFixedSchedule = fixedScheduleData,
                                CourseSection = courseSection,
                                MarkingPeriod = markingId,
                                StandardGradeScaleName = standardGradeScaleName,
                                TotalStaffSchedule = totalStaff,
                                TotalStudentSchedule = totalStudent,
                                AvailableSeat = courseSection.Seats - totalStudent,
                                StaffName = staffFullName,
                                HolidayList= holidayList
                            };
                            courseSectionView.GetCourseSectionForView.Add(getFixedSchedule);
                            //}
                        }

                        if (String.Compare(courseSection.ScheduleType,"Variable Schedule (2)",true)==0)
                        {
                            var variableScheduleData = this.context?.CourseVariableSchedule.Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseId == courseSection.CourseId && x.CourseSectionId == courseSection.CourseSectionId).Include(f => f.Rooms).Include(f => f.BlockPeriod).Select(s => new CourseVariableSchedule { TenantId = s.TenantId, SchoolId = s.SchoolId, CourseId = s.CourseId, CourseSectionId = s.CourseSectionId, GradeScaleId = s.GradeScaleId, Serial = s.Serial, Day = s.Day, RoomId = s.RoomId, TakeAttendance = s.TakeAttendance, PeriodId = s.PeriodId, BlockId = s.BlockId, CreatedBy = s.CreatedBy, CreatedOn = s.CreatedOn, UpdatedBy = s.UpdatedBy, UpdatedOn = s.UpdatedOn, Rooms = new Rooms { Title = s.Rooms!.Title }, BlockPeriod = new BlockPeriod { PeriodTitle = s.BlockPeriod!.PeriodTitle } }).ToList();

                            //if (variableScheduleData.Count > 0)
                            //{
                            courseSection.ScheduleType = "Variable Schedule";
                            GetCourseSectionForView getVariableSchedule = new GetCourseSectionForView
                            {
                                CourseVariableSchedule = variableScheduleData??null!,
                                CourseSection = courseSection,
                                MarkingPeriod = markingId,
                                StandardGradeScaleName = standardGradeScaleName,
                                TotalStaffSchedule = totalStaff,
                                TotalStudentSchedule = totalStudent,
                                AvailableSeat = courseSection.Seats - totalStudent,
                                StaffName = staffFullName,
                                HolidayList = holidayList
                            };
                            courseSectionView.GetCourseSectionForView.Add(getVariableSchedule);
                            //}
                        }

                        if (String.Compare(courseSection.ScheduleType, "Calendar Schedule (3)",true)==0)
                        {
                            var calendarScheduleData = this.context?.CourseCalendarSchedule.Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseId == courseSection.CourseId && x.CourseSectionId == courseSection.CourseSectionId).Include(f => f.Rooms).Include(f => f.BlockPeriod).Select(s => new CourseCalendarSchedule { TenantId = s.TenantId, SchoolId = s.SchoolId, CourseId = s.CourseId, CourseSectionId = s.CourseSectionId, GradeScaleId = s.GradeScaleId, Serial = s.Serial, Date = s.Date, RoomId = s.RoomId, TakeAttendance = s.TakeAttendance, PeriodId = s.PeriodId, BlockId = s.BlockId, CreatedBy = s.CreatedBy, CreatedOn = s.CreatedOn, UpdatedBy = s.UpdatedBy, UpdatedOn = s.UpdatedOn, Rooms = new Rooms { Title = s.Rooms!.Title }, BlockPeriod = new BlockPeriod { PeriodTitle = s.BlockPeriod!.PeriodTitle } }).ToList();
                            //if (calendarScheduleData.Count > 0)
                            //{
                            courseSection.ScheduleType = "Calendar Schedule";
                            GetCourseSectionForView getCalendarSchedule = new GetCourseSectionForView
                            {
                                CourseCalendarSchedule = calendarScheduleData??null!,
                                CourseSection = courseSection,
                                MarkingPeriod = markingId,
                                StandardGradeScaleName = standardGradeScaleName,
                                TotalStaffSchedule = totalStaff,
                                TotalStudentSchedule = totalStudent,
                                AvailableSeat = courseSection.Seats - totalStudent,
                                StaffName = staffFullName,
                                HolidayList = holidayList
                            };
                            courseSectionView.GetCourseSectionForView.Add(getCalendarSchedule);
                            //}
                        }

                        if (String.Compare(courseSection.ScheduleType,"Block Schedule (4)",true)==0)
                        {
                            var blockScheduleData = this.context?.CourseBlockSchedule.Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseId == courseSection.CourseId && x.CourseSectionId == courseSection.CourseSectionId).Include(f => f.Rooms).Include(f => f.BlockPeriod).Include(f => f.Block).Select(s => new CourseBlockSchedule { TenantId = s.TenantId, SchoolId = s.SchoolId, CourseId = s.CourseId, CourseSectionId = s.CourseSectionId, GradeScaleId = s.GradeScaleId, Serial = s.Serial, RoomId = s.RoomId, PeriodId = s.PeriodId, BlockId = s.BlockId, TakeAttendance = s.TakeAttendance, CreatedBy = s.CreatedBy, CreatedOn = s.CreatedOn, UpdatedBy = s.UpdatedBy, UpdatedOn = s.UpdatedOn, Rooms = new Rooms { Title = s.Rooms!.Title }, BlockPeriod = new BlockPeriod { PeriodTitle = s.BlockPeriod!.PeriodTitle }, Block = new Block { BlockTitle = s.Block!.BlockTitle } }).ToList();
                            //if (blockScheduleData.Count > 0)
                            //{
                            courseSection.ScheduleType = "Block/Rotating Schedule";

                            var bellScheduleList = new List<BellSchedule>();
                            if (blockScheduleData != null && blockScheduleData.Any())
                            {
                                foreach (var block in blockScheduleData)
                                {
                                    var bellScheduleData = this.context?.BellSchedule.Where(c => c.SchoolId == courseSection.SchoolId && c.TenantId == courseSection.TenantId && c.BlockId == block.BlockId && c.BellScheduleDate >= courseSection.DurationStartDate && c.BellScheduleDate <= courseSection.DurationEndDate).ToList();
                                    if (bellScheduleData != null && bellScheduleData.Any())
                                        bellScheduleList.AddRange(bellScheduleData);
                                }

                                GetCourseSectionForView getBlockSchedule = new GetCourseSectionForView
                                {
                                    CourseBlockSchedule = blockScheduleData,
                                    CourseSection = courseSection,
                                    MarkingPeriod = markingId,
                                    StandardGradeScaleName = standardGradeScaleName,
                                    TotalStaffSchedule = totalStaff,
                                    TotalStudentSchedule = totalStudent,
                                    AvailableSeat = courseSection.Seats - totalStudent,
                                    StaffName = staffFullName,
                                    HolidayList = holidayList,
                                    BellScheduleList = bellScheduleList
                                };
                                courseSectionView.GetCourseSectionForView.Add(getBlockSchedule);
                                //}
                            }
                        }
                    }

                    //this block for fetch school details
                    if (courseSectionViewModel.schoolDetails == true)
                    {
                        var SchoolDetailsData = this.context?.SchoolMaster.Include(y => y.SchoolDetail).Where(x => x.TenantId == courseSectionViewModel.TenantId && x.SchoolId == courseSectionViewModel.SchoolId).FirstOrDefault();

                        if (SchoolDetailsData != null)
                        {
                            courseSectionView.SchoolName = SchoolDetailsData.SchoolName;
                            courseSectionView.StreetAddress1 = SchoolDetailsData.StreetAddress1;
                            courseSectionView.StreetAddress2 = SchoolDetailsData.StreetAddress2;
                            courseSectionView.Country = SchoolDetailsData.Country;
                            courseSectionView.City = SchoolDetailsData.City;
                            courseSectionView.State = SchoolDetailsData.State;
                            courseSectionView.District = SchoolDetailsData.District;
                            courseSectionView.Zip = SchoolDetailsData.Zip;
                            //courseSectionView.SchoolLogo = SchoolDetailsData.SchoolDetail.FirstOrDefault()?.SchoolLogo;
                            courseSectionView.SchoolLogo = SchoolDetailsData.SchoolDetail.FirstOrDefault()?.SchoolThumbnailLogo;
                        }
                    }

                    courseSectionView.TenantId = courseSectionViewModel.TenantId;
                    courseSectionView.SchoolId = courseSectionViewModel.SchoolId;
                    courseSectionView.CourseId = courseSectionViewModel.CourseId;
                    courseSectionView.AcademicYear = courseSectionViewModel.AcademicYear;
                    courseSectionView._tenantName = courseSectionViewModel._tenantName;
                    courseSectionView._token = courseSectionViewModel._token;
                    courseSectionView._failure = false;
                }
                else
                {
                    courseSectionView._failure = true;
                    courseSectionView._message = NORECORDFOUND;
                    courseSectionView._tenantName = courseSectionViewModel._tenantName;
                    courseSectionView._token = courseSectionViewModel._token;
                }
            }
            catch (Exception es)
            {
                courseSectionView.GetCourseSectionForView = null!;
                courseSectionView._failure = true;
                courseSectionView._message = es.Message;
            }
            return courseSectionView;
        }

        /// <summary>
        /// Add Fixed Course Section Schedule
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <param name="CourseSectionId"></param>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        private CourseSectionAddViewModel AddFixedSchedule(CourseSectionAddViewModel courseSectionAddViewModel, int? CourseSectionId, decimal? academicYear)
        {
            try
            {
                if (courseSectionAddViewModel.CourseFixedSchedule != null)
                {
                    if (courseSectionAddViewModel.CourseSection!.AllowRoomConflict != true)
                    {
                        var courseSectionList = this.context?.AllCourseSectionView.AsEnumerable().Where(c => c.TenantId == courseSectionAddViewModel!.CourseSection!.TenantId && c.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId &&
                       ((c.FixedPeriodId != null && (c.FixedRoomId == courseSectionAddViewModel.CourseFixedSchedule.RoomId && c.FixedPeriodId == courseSectionAddViewModel.CourseFixedSchedule.PeriodId && Regex.IsMatch((courseSectionAddViewModel.CourseSection.MeetingDays ?? "").ToLower(), (c.FixedDays ?? "").ToLower(), RegexOptions.IgnoreCase))) ||
                        (c.VarPeriodId != null && (c.VarRoomId == courseSectionAddViewModel.CourseFixedSchedule.RoomId && c.VarPeriodId == courseSectionAddViewModel.CourseFixedSchedule.PeriodId && (courseSectionAddViewModel.CourseSection.MeetingDays ?? "").ToLower().Contains((c.VarDay ?? "").ToLower()))) ||
                        (c.CalPeriodId != null && (c.CalRoomId == courseSectionAddViewModel.CourseFixedSchedule.RoomId && c.CalPeriodId == courseSectionAddViewModel.CourseFixedSchedule.PeriodId && (courseSectionAddViewModel.CourseSection.MeetingDays ?? "").ToLower().Contains((c.CalDay ?? "").ToLower()))))
                       && c.AcademicYear == academicYear && c.DurationEndDate > courseSectionAddViewModel.CourseSection.DurationStartDate).ToList();

                        if (courseSectionList != null && courseSectionList.Any())
                        {
                            courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Scheduling.";
                            courseSectionAddViewModel._failure = true;
                            return courseSectionAddViewModel;
                        }

                    }
                    //string[] meetingDays = courseSectionAddViewModel.courseSection.MeetingDays.Split("|");
                    //string message = null;

                    //foreach (var meetingDay in meetingDays)
                    //{ 
                    //   var courseSectionList = this.context?.CourseSection.
                    //                Join(this.context?.CourseFixedSchedule,
                    //                cs => cs.CourseSectionId, cf => cf.CourseSectionId,
                    //                (cs, cfs) => new { cs, cfs }).Where(c=>c.cs.TenantId==courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId==courseSectionAddViewModel.courseSection.SchoolId && c.cfs.RoomId== courseSectionAddViewModel.courseFixedSchedule.RoomId && c.cfs.PeriodId==courseSectionAddViewModel.courseFixedSchedule.PeriodId && c.cs.MeetingDays.Contains(meetingDay) && c.cs.AcademicYear== academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();


                    //    if (courseSectionList.Count>0)
                    //    {
                    //        courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Fixed Scheduling";
                    //        courseSectionAddViewModel._failure = true;
                    //        return courseSectionAddViewModel;
                    //    }

                    //    Days days = new Days();
                    //    var Day = Enum.GetName(days.GetType(), Convert.ToInt32(meetingDay));

                    //    var variableScheduleDataList = this.context?.CourseSection.
                    //                Join(this.context?.CourseVariableSchedule,
                    //                cs => cs.CourseSectionId, cf => cf.CourseSectionId,
                    //                (cs, cvs) => new { cs, cvs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cvs.RoomId == courseSectionAddViewModel.courseFixedSchedule.RoomId && c.cvs.PeriodId == courseSectionAddViewModel.courseFixedSchedule.PeriodId && c.cvs.Day.ToLower()==Day.ToLower() && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                    //    if (variableScheduleDataList.Count > 0)
                    //    {
                    //        courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Variable Scheduling";
                    //        courseSectionAddViewModel._failure = true;
                    //        return courseSectionAddViewModel;
                    //    }

                    //    var calenderScheduleList = this.context?.CourseSection.
                    //                Join(this.context?.CourseCalendarSchedule,
                    //                cs => cs.CourseSectionId, ccs => ccs.CourseSectionId,
                    //                (cs, ccs) => new { cs, ccs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.ccs.RoomId == courseSectionAddViewModel.courseFixedSchedule.RoomId && c.ccs.PeriodId == courseSectionAddViewModel.courseFixedSchedule.PeriodId  && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                    //    if (calenderScheduleList.Count > 0)
                    //    {
                    //        foreach (var calenderSchedule in calenderScheduleList)
                    //        {
                    //            var calenderDay = calenderSchedule.ccs.Date.Value.DayOfWeek.ToString();

                    //            if (calenderDay != null)
                    //            {
                    //                int dayValue = (int)Enum.Parse(typeof(Days), calenderDay);                                  

                    //                if (dayValue.ToString() == meetingDay)
                    //                {
                    //                    if (message != null)
                    //                    {
                    //                        message = calenderSchedule.ccs.Date.Value.Date.ToString("yyyy-MM-dd") + "(" + calenderDay + ")" + "," + message;
                    //                    }
                    //                    else
                    //                    {
                    //                        message = calenderSchedule.ccs.Date.Value.Date.ToString("yyyy-MM-dd") + "(" + calenderDay + ")";
                    //                    }
                    //                    courseSectionAddViewModel._message = message + "is associate with Course Calender Schedule";
                    //                    courseSectionAddViewModel._failure = true;
                    //                    return courseSectionAddViewModel;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    var roomCapacity = this.context?.Rooms.FirstOrDefault(e => e.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && e.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && e.RoomId == courseSectionAddViewModel.CourseFixedSchedule.RoomId)?.Capacity;

                    if (roomCapacity == null || roomCapacity < courseSectionAddViewModel.CourseSection!.Seats)
                    {
                        courseSectionAddViewModel._message = "Invalid Seat Capacity";
                        courseSectionAddViewModel._failure = true;
                        return courseSectionAddViewModel;
                    }
                    else
                    {
                        int? fixedscheduleSerial = 1;

                        var CourseSectionfixedscheduleData = this.context?.CourseFixedSchedule.Where(x => x.TenantId == courseSectionAddViewModel.CourseSection.TenantId && x.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId).OrderByDescending(x => x.Serial).FirstOrDefault();

                        if (CourseSectionfixedscheduleData != null)
                        {
                            fixedscheduleSerial = CourseSectionfixedscheduleData.Serial + 1;
                        }

                        var courseFixedSchedule = new CourseFixedSchedule()
                        {
                            TenantId = courseSectionAddViewModel.CourseSection.TenantId,
                            SchoolId = courseSectionAddViewModel.CourseSection.SchoolId,
                            CourseId = courseSectionAddViewModel.CourseSection.CourseId,
                            CourseSectionId = (int)CourseSectionId!,
                            GradeScaleId = courseSectionAddViewModel.CourseSection.GradeScaleId,
                            Serial = (int)fixedscheduleSerial,
                            RoomId = courseSectionAddViewModel.CourseFixedSchedule.RoomId,
                            PeriodId = courseSectionAddViewModel.CourseFixedSchedule.PeriodId,
                            CreatedBy = courseSectionAddViewModel.CourseFixedSchedule.CreatedBy,
                            CreatedOn = DateTime.UtcNow,
                            BlockId = courseSectionAddViewModel.CourseFixedSchedule.BlockId
                        };

                        this.context?.CourseFixedSchedule.Add(courseFixedSchedule);
                        courseSectionAddViewModel.CourseSection.ScheduleType = "Fixed Schedule (1)";
                        courseSectionAddViewModel._failure = false;
                    }
                }
            }
            catch (Exception es)
            {
                courseSectionAddViewModel._failure = true;
                courseSectionAddViewModel._message = es.Message;
            }
            return courseSectionAddViewModel;
        }

        /// <summary>
        /// Add Variable Course Section Schedule
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <param name="CourseSectionId"></param>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        private CourseSectionAddViewModel AddVariableSchedule(CourseSectionAddViewModel courseSectionAddViewModel, int? CourseSectionId, decimal? academicYear)
        {
            try
            {
                List<CourseVariableSchedule> courseVariableScheduleList = new List<CourseVariableSchedule>();
                //string message = null;

                if (courseSectionAddViewModel.CourseVariableScheduleList.Count > 0)
                {
                    int? variablescheduleSerial = 1;

                    var CourseSectionvariablescheduleData = this.context?.CourseVariableSchedule.Where(x => x.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && x.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId).OrderByDescending(x => x.Serial).FirstOrDefault();

                    if (CourseSectionvariablescheduleData != null)
                    {
                        variablescheduleSerial = CourseSectionvariablescheduleData.Serial + 1;
                        //variablescheduleSerial = CourseSectionvariablescheduleData.cvs.Serial + 1;
                    }

                    foreach (var courseVariableSchedules in courseSectionAddViewModel.CourseVariableScheduleList)
                    {
                        if (courseSectionAddViewModel.CourseSection!.AllowRoomConflict != true)
                        {
                            var courseSectionList = this.context?.AllCourseSectionView.Where(c => c.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && c.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId &&
                            ((c.FixedPeriodId != null && c.FixedRoomId == courseVariableSchedules.RoomId && c.FixedPeriodId == courseVariableSchedules.PeriodId && (c.FixedDays ?? "").Contains(courseVariableSchedules!.Day!)) ||
                            (c.VarPeriodId != null && (c.VarRoomId == courseVariableSchedules.RoomId && c.VarPeriodId == courseVariableSchedules.PeriodId && c.VarDay == courseVariableSchedules.Day)) ||
                            (c.CalPeriodId != null && (c.CalRoomId == courseVariableSchedules.RoomId && c.CalPeriodId == courseVariableSchedules.PeriodId && c.CalDay == courseVariableSchedules.Day)))
                            && c.AcademicYear == academicYear && c.DurationEndDate > courseSectionAddViewModel.CourseSection.DurationStartDate).ToList();

                            if (courseSectionList != null && courseSectionList.Any())
                            {
                                courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Scheduling.";
                                courseSectionAddViewModel._failure = true;
                                return courseSectionAddViewModel;
                            }
                        }
                        //Days days = new Days();

                        //var Day = Enum.GetName(days.GetType(), Convert.ToInt32(courseVariableSchedules.Day));                        

                        //var variableScheduleDataList = this.context?.CourseSection.
                        //            Join(this.context?.CourseVariableSchedule,
                        //            cs => cs.CourseSectionId, cvs => cvs.CourseSectionId,
                        //            (cs, cvs) => new { cs, cvs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cvs.RoomId == courseVariableSchedules.RoomId && c.cvs.PeriodId == courseVariableSchedules.PeriodId && c.cvs.Day.ToLower() == Day.ToLower() && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();


                        //if (variableScheduleDataList.Count > 0)
                        //{
                        //    courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Variable Scheduling";
                        //    courseSectionAddViewModel._failure = true;
                        //    return courseSectionAddViewModel;
                        //}

                        //var courseSectionList = this.context?.CourseSection.
                        //            Join(this.context?.CourseFixedSchedule,
                        //            cs => cs.CourseSectionId, cf => cf.CourseSectionId,
                        //            (cs, cf) => new { cs, cf }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cf.RoomId == courseVariableSchedules.RoomId && c.cf.PeriodId == courseVariableSchedules.PeriodId && c.cs.MeetingDays.Contains(courseVariableSchedules.Day) && c.cs.AcademicYear==academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //if (courseSectionList.Count > 0)
                        //{
                        //    courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Fixed Scheduling";
                        //    courseSectionAddViewModel._failure = true;
                        //    return courseSectionAddViewModel;
                        //}

                        //    var calenderScheduleList = this.context?.CourseSection.
                        //            Join(this.context?.CourseCalendarSchedule,
                        //            cs => cs.CourseSectionId, ccs => ccs.CourseSectionId,
                        //            (cs, ccs) => new { cs, ccs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.ccs.RoomId == courseVariableSchedules.RoomId && c.ccs.PeriodId == courseVariableSchedules.PeriodId && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();


                        //    if (calenderScheduleList.Count>0)
                        //    {

                        //        foreach (var calenderSchedule in calenderScheduleList)
                        //        {
                        //            var calenderDay = calenderSchedule.ccs.Date.Value.DayOfWeek.ToString();

                        //            if (calenderDay != null)
                        //            {
                        //                int dayValue = (int)Enum.Parse(typeof(Days), calenderDay);

                        //                if (dayValue.ToString() == courseVariableSchedules.Day)
                        //                {
                        //                    if (message != null)
                        //                    {
                        //                        message = calenderSchedule.ccs.Date.Value.Date.ToString("yyyy-MM-dd") + "(" + calenderDay + ")" + "," + message;
                        //                    }
                        //                    else
                        //                    {
                        //                        message = calenderSchedule.ccs.Date.Value.Date.ToString("yyyy-MM-dd") + "(" + calenderDay + ")";
                        //                    }
                        //                    courseSectionAddViewModel._message = message + "is associate with Course Calender Schedule";
                        //                    courseSectionAddViewModel._failure = true;
                        //                    return courseSectionAddViewModel;
                        //                }
                        //            }
                        //        }

                        //    }

                        var courseeVariableSchedule = new CourseVariableSchedule()
                        {
                            TenantId = courseSectionAddViewModel.CourseSection!.TenantId,
                            SchoolId = courseSectionAddViewModel.CourseSection.SchoolId,
                            CourseId = courseSectionAddViewModel.CourseSection.CourseId,
                            CourseSectionId = (int)CourseSectionId!,
                            GradeScaleId = courseSectionAddViewModel.CourseSection.GradeScaleId,
                            Serial = (int)variablescheduleSerial,
                            Day = courseVariableSchedules.Day,
                            PeriodId = courseVariableSchedules.PeriodId,
                            RoomId = courseVariableSchedules.RoomId,
                            TakeAttendance = courseVariableSchedules.TakeAttendance,
                            CreatedBy = courseSectionAddViewModel.CourseSection.CreatedBy,
                            CreatedOn = DateTime.UtcNow,
                            BlockId = courseVariableSchedules.BlockId

                        };
                        courseVariableScheduleList.Add(courseeVariableSchedule);
                        variablescheduleSerial++;
                    }
                    this.context?.CourseVariableSchedule.AddRange(courseVariableScheduleList);
                    courseSectionAddViewModel.CourseSection!.ScheduleType = "Variable Schedule (2)";
                    courseSectionAddViewModel._failure = false;
                }
            }
            catch (Exception es)
            {
                courseSectionAddViewModel._failure = true;
                courseSectionAddViewModel._message = es.Message;
            }
            return courseSectionAddViewModel;
        }

        /// <summary>
        /// Add Calendar Course Section Schedule
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <param name="CourseSectionId"></param>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        private CourseSectionAddViewModel AddCalendarSchedule(CourseSectionAddViewModel courseSectionAddViewModel, int? CourseSectionId, decimal? academicYear)
        {
            try
            {
                List<CourseCalendarSchedule> courseCalendarScheduleList = new List<CourseCalendarSchedule>();

                if (courseSectionAddViewModel.CourseCalendarScheduleList.Count > 0)
                {
                    int? calendarscheduleSerial = 1;

                    var CourseSectioncalendarscheduleData = this.context?.CourseCalendarSchedule.Where(x => x.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && x.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId).OrderByDescending(x => x.Serial).FirstOrDefault();

                    if (CourseSectioncalendarscheduleData != null)
                    {
                        calendarscheduleSerial = CourseSectioncalendarscheduleData.Serial + 1;
                        //calendarscheduleSerial = CourseSectioncalendarscheduleData.ccs.Serial + 1;
                    }

                    foreach (var courseCalendarSchedule in courseSectionAddViewModel.CourseCalendarScheduleList)
                    {
                        if (courseSectionAddViewModel.CourseSection!.AllowRoomConflict != true)
                        {
                            var calenderDay = courseCalendarSchedule.Date!.Value.DayOfWeek.ToString();

                            var courseSectionList = this.context?.AllCourseSectionView.Where(c => c.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && c.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId &&
                            ((c.FixedRoomId != null && (c.FixedRoomId == courseCalendarSchedule.RoomId && c.FixedPeriodId == courseCalendarSchedule.PeriodId && (c.FixedDays ?? "").Contains(calenderDay))) ||
                            (c.VarPeriodId != null && (c.VarRoomId == courseCalendarSchedule.RoomId && c.VarPeriodId == courseCalendarSchedule.PeriodId && c.VarDay == calenderDay)) ||
                            (c.CalPeriodId != null && (c.CalRoomId == courseCalendarSchedule.RoomId && c.CalPeriodId == courseCalendarSchedule.PeriodId && c.CalDay == calenderDay)))
                            && c.AcademicYear == academicYear && c.DurationEndDate > courseSectionAddViewModel.CourseSection.DurationStartDate).ToList();

                            if (courseSectionList != null && courseSectionList.Any())
                            {
                                courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Scheduling.";
                                courseSectionAddViewModel._failure = true;
                                return courseSectionAddViewModel;
                            }
                        }

                        //    var calenderDay = courseCalendarSchedule.Date.Value.DayOfWeek.ToString();

                        //    if (calenderDay != null)
                        //    {
                        //        int dayValue = (int)Enum.Parse(typeof(Days), calenderDay);

                        //        var courseSectionList = this.context?.CourseSection.
                        //            Join(this.context?.CourseFixedSchedule,
                        //            cs => cs.CourseSectionId, cf => cf.CourseSectionId,
                        //            (cs, cf) => new { cs, cf }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cf.RoomId == courseCalendarSchedule.RoomId && c.cf.PeriodId == courseCalendarSchedule.PeriodId && c.cs.MeetingDays.Contains(dayValue.ToString()) && c.cs.AcademicYear==academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //    if (courseSectionList.Count > 0)
                        //    {                            
                        //        courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Fixed Scheduling";
                        //        courseSectionAddViewModel._failure = true;
                        //        return courseSectionAddViewModel;
                        //    }
                        //}

                        //    var variableScheduleDataList = this.context?.CourseSection.
                        //                Join(this.context?.CourseVariableSchedule,
                        //                cs => cs.CourseSectionId, cvs => cvs.CourseSectionId,
                        //                (cs, cvs) => new { cs, cvs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cvs.RoomId == courseCalendarSchedule.RoomId && c.cvs.PeriodId == courseCalendarSchedule.PeriodId && c.cvs.Day.ToLower() == calenderDay.ToLower() && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //    if (variableScheduleDataList.Count > 0)
                        //        {                            
                        //            courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Variable Scheduling";
                        //            courseSectionAddViewModel._failure = true;
                        //            return courseSectionAddViewModel;
                        //        }                        

                        //    var calenderScheduleDataList = this.context?.CourseSection.
                        //                Join(this.context?.CourseCalendarSchedule,
                        //                cs => cs.CourseSectionId, ccs => ccs.CourseSectionId,
                        //                (cs, ccs) => new { cs, ccs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.ccs.RoomId == courseCalendarSchedule.RoomId && c.ccs.PeriodId == courseCalendarSchedule.PeriodId && c.ccs.Date == courseCalendarSchedule.Date && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //    if (calenderScheduleDataList.Count > 0)
                        //        {
                        //            courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Calendar Scheduling.";
                        //            courseSectionAddViewModel._failure = true;
                        //            return courseSectionAddViewModel;
                        //        }

                        var courseCalenderSchedule = new CourseCalendarSchedule()
                        {
                            TenantId = courseSectionAddViewModel.CourseSection!.TenantId,
                            SchoolId = courseSectionAddViewModel.CourseSection.SchoolId,
                            CourseId = courseSectionAddViewModel.CourseSection.CourseId,
                            CourseSectionId = (int)CourseSectionId!,
                            GradeScaleId = courseSectionAddViewModel.CourseSection.GradeScaleId,
                            Serial = (int)calendarscheduleSerial,
                            BlockId = courseCalendarSchedule.BlockId,
                            Date = courseCalendarSchedule.Date,
                            PeriodId = courseCalendarSchedule.PeriodId,
                            RoomId = courseCalendarSchedule.RoomId,
                            TakeAttendance = courseCalendarSchedule.TakeAttendance,
                            CreatedBy = courseSectionAddViewModel.CourseSection.CreatedBy,
                            CreatedOn = DateTime.UtcNow
                        };
                        courseCalendarScheduleList.Add(courseCalenderSchedule);
                        calendarscheduleSerial++;
                    }
                    this.context?.CourseCalendarSchedule.AddRange(courseCalendarScheduleList);
                    courseSectionAddViewModel.CourseSection!.ScheduleType = "Calendar Schedule (3)";
                    courseSectionAddViewModel._failure = false;
                }
            }
            catch (Exception es)
            {
                courseSectionAddViewModel._failure = true;
                courseSectionAddViewModel._message = es.Message;
            }
            return courseSectionAddViewModel;
        }

        /// <summary>
        /// Add Block Course Section Schedule
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <param name="CourseSectionId"></param>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        private CourseSectionAddViewModel AddBlockSchedule(CourseSectionAddViewModel courseSectionAddViewModel, int? CourseSectionId, decimal? academicYear)
        {
            try
            {
                List<CourseBlockSchedule> courseBlockScheduleList = new List<CourseBlockSchedule>();

                if (courseSectionAddViewModel.CourseBlockScheduleList.Count > 0)
                {

                    int? blockscheduleSerial = 1;

                    var CourseSectionblockscheduleData = this.context?.CourseBlockSchedule.Where(x => x.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && x.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId).OrderByDescending(x => x.Serial).FirstOrDefault();

                    if (CourseSectionblockscheduleData != null)
                    {
                        blockscheduleSerial = CourseSectionblockscheduleData.Serial + 1;
                    }

                    foreach (var courseBlockSchedules in courseSectionAddViewModel.CourseBlockScheduleList)
                    {
                        if (courseSectionAddViewModel.CourseSection!.AllowRoomConflict != true)
                        {
                            var blockScheduleData = this.context?.AllCourseSectionView.Where(c => c.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && c.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && c.BlockRoomId == courseBlockSchedules.RoomId && c.BlockPeriodId == courseBlockSchedules.PeriodId && c.BlockId == courseBlockSchedules.BlockId && c.AcademicYear == academicYear && c.DurationEndDate > courseSectionAddViewModel.CourseSection.DurationStartDate).ToList();

                            if (blockScheduleData != null && blockScheduleData.Any())
                            {
                                courseSectionAddViewModel._failure = true;
                                courseSectionAddViewModel._message = "Room is not available for this block and period";
                                return courseSectionAddViewModel;
                            }
                        }

                        var courseBlockSchedule = new CourseBlockSchedule()
                        {
                            TenantId = courseSectionAddViewModel.CourseSection!.TenantId,
                            SchoolId = courseSectionAddViewModel.CourseSection.SchoolId,
                            CourseId = courseSectionAddViewModel.CourseSection.CourseId,
                            CourseSectionId = (int)CourseSectionId!,
                            GradeScaleId = courseSectionAddViewModel.CourseSection.GradeScaleId,
                            Serial = (int)blockscheduleSerial,
                            BlockId = courseBlockSchedules.BlockId,
                            PeriodId = courseBlockSchedules.PeriodId,
                            RoomId = courseBlockSchedules.RoomId,
                            TakeAttendance = courseBlockSchedules.TakeAttendance,
                            CreatedBy = courseSectionAddViewModel.CourseSection.CreatedBy,
                            CreatedOn = DateTime.UtcNow
                        };
                        courseBlockScheduleList.Add(courseBlockSchedule);
                        blockscheduleSerial++;

                    }
                    this.context?.CourseBlockSchedule.AddRange(courseBlockScheduleList);
                    courseSectionAddViewModel.CourseSection!.ScheduleType = "Block Schedule (4)";
                    courseSectionAddViewModel._failure = false;
                }
            }
            catch (Exception es)
            {
                courseSectionAddViewModel._failure = true;
                courseSectionAddViewModel._message = es.Message;
            }
            return courseSectionAddViewModel;
        }

        /// <summary>
        /// Update Course Section
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <returns></returns>
        public CourseSectionAddViewModel UpdateCourseSection(CourseSectionAddViewModel courseSectionAddViewModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    var studentScheduleData = this.context?.StudentCoursesectionSchedule.FirstOrDefault(e => e.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && e.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && e.CourseSectionId == courseSectionAddViewModel.CourseSection.CourseSectionId && (e.IsDropped == null || e.IsDropped == false));

                    var staffScheduleData = this.context?.StaffCoursesectionSchedule.FirstOrDefault(e => e.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && e.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && e.CourseSectionId == courseSectionAddViewModel.CourseSection.CourseSectionId);

                    if (studentScheduleData != null || staffScheduleData != null)
                    {
                        courseSectionAddViewModel._failure = true;
                        courseSectionAddViewModel._message = "This Course Section could not be updated. It has association";
                    }
                    else
                    {
                        decimal? academicYear = null;

                        var courseSectionUpdate = this.context?.CourseSection.FirstOrDefault(x => x.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && x.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && x.CourseId == courseSectionAddViewModel.CourseSection.CourseId && x.CourseSectionId == courseSectionAddViewModel.CourseSection.CourseSectionId);

                        if (courseSectionUpdate != null)
                        {
                            academicYear = courseSectionUpdate.AcademicYear;

                            var CalenderData = this.context?.SchoolCalendars.FirstOrDefault(x => x.CalenderId == courseSectionAddViewModel.CourseSection!.CalendarId && x.TenantId == courseSectionAddViewModel.CourseSection.TenantId && x.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId);

                            if (CalenderData != null)
                            {
                                //academicYear = CalenderData.AcademicYear;

                                if (!(bool)courseSectionAddViewModel.CourseSection!.DurationBasedOnPeriod!)
                                {
                                    if (CalenderData.StartDate > courseSectionAddViewModel.CourseSection.DurationStartDate || CalenderData.EndDate < courseSectionAddViewModel.CourseSection.DurationEndDate)
                                    {
                                        courseSectionAddViewModel._message = "Start Date And End Date of Course Section Should Be Between Start Date And End Date of School Calender";
                                        courseSectionAddViewModel._failure = true;
                                        return courseSectionAddViewModel;
                                    }
                                }

                                var courseSectionNameExists = this.context?.CourseSection.AsEnumerable().FirstOrDefault(x => x.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && x.TenantId == courseSectionAddViewModel.CourseSection.TenantId && x.CourseSectionId != courseSectionAddViewModel.CourseSection.CourseSectionId && String.Compare(x.CourseSectionName,courseSectionAddViewModel.CourseSection.CourseSectionName,true)==0 && x.AcademicYear == academicYear);

                                if (courseSectionNameExists != null)
                                {
                                    courseSectionAddViewModel._failure = true;
                                    courseSectionAddViewModel._message = "Course Section Name already exists";
                                    return courseSectionAddViewModel;
                                }


                                courseSectionAddViewModel._message = "Course Section Updated Successfully";

                                if (courseSectionAddViewModel.CourseSection.ScheduleType != null)
                                {
                                    switch (courseSectionAddViewModel.CourseSection.ScheduleType.ToLower())
                                    {
                                        case "fixedschedule":
                                            if (UpdateFixedSchedule(courseSectionAddViewModel, academicYear)._failure)
                                            {
                                                return courseSectionAddViewModel;
                                            }
                                            break;

                                        case "variableschedule":
                                            if (UpdateVariableSchedule(courseSectionAddViewModel, academicYear)._failure)
                                            {
                                                return courseSectionAddViewModel;
                                            }
                                            break;

                                        case "calendarschedule":
                                            if (UpdateCalendarSchedule(courseSectionAddViewModel, academicYear)._failure)
                                            {
                                                return courseSectionAddViewModel;
                                            }
                                            break;

                                        case "blockschedule":
                                            if (UpdateBlockSchedule(courseSectionAddViewModel, academicYear)._failure)
                                            {
                                                return courseSectionAddViewModel;
                                            }
                                            break;
                                        default:
                                            courseSectionAddViewModel._failure = true;
                                            courseSectionAddViewModel._message = "Please Provide a Valid Schedule Type.";
                                            return courseSectionAddViewModel;
                                    }

                                    if (courseSectionAddViewModel.MarkingPeriodId != null)
                                    {
                                        var markingPeriodid = courseSectionAddViewModel.MarkingPeriodId.Split("_", StringSplitOptions.RemoveEmptyEntries);

                                        if (markingPeriodid.First() == "3")
                                        {
                                            courseSectionAddViewModel.CourseSection.PrgrsprdMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                                        }
                                        if (markingPeriodid.First() == "2")
                                        {
                                            courseSectionAddViewModel.CourseSection.QtrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                                        }
                                        if (markingPeriodid.First() == "1")
                                        {
                                            courseSectionAddViewModel.CourseSection.SmstrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                                        }
                                        if (markingPeriodid.First() == "0")
                                        {
                                            courseSectionAddViewModel.CourseSection.YrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                                        }
                                    }
                                    courseSectionAddViewModel.CourseSection.CreatedBy = courseSectionUpdate.CreatedBy;
                                    courseSectionAddViewModel.CourseSection.CreatedOn = courseSectionUpdate.CreatedOn;
                                    courseSectionAddViewModel.CourseSection.UpdatedOn = DateTime.UtcNow;
                                    courseSectionAddViewModel.CourseSection.AcademicYear = academicYear;
                                    this.context?.Entry(courseSectionUpdate).CurrentValues.SetValues(courseSectionAddViewModel.CourseSection);
                                    this.context?.SaveChanges();
                                    courseSectionAddViewModel._failure = false;
                                }
                            }
                            else
                            {
                                courseSectionAddViewModel._failure = true;
                                courseSectionAddViewModel._message = "School Calender does not exist";
                                return courseSectionAddViewModel;
                            }
                        }
                        else
                        {
                            courseSectionAddViewModel._failure = true;
                            courseSectionAddViewModel._message = NORECORDFOUND;
                        }
                        transaction?.Commit();
                    }
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    courseSectionAddViewModel._failure = true;
                    courseSectionAddViewModel._message = es.Message;
                }
            }
            return courseSectionAddViewModel;
        }

        /// <summary>
        /// Update Fixed Course Section Schedule
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        private CourseSectionAddViewModel UpdateFixedSchedule(CourseSectionAddViewModel courseSectionAddViewModel, decimal? academicYear)
        {
            try
            {
                if (courseSectionAddViewModel.CourseFixedSchedule != null)
                {
                    var fixedScheduleDataUpdate = this.context?.CourseFixedSchedule.Where(x => x.TenantId == courseSectionAddViewModel.CourseFixedSchedule.TenantId && x.SchoolId == courseSectionAddViewModel.CourseFixedSchedule.SchoolId && x.CourseId == courseSectionAddViewModel.CourseFixedSchedule.CourseId && x.CourseSectionId == courseSectionAddViewModel.CourseFixedSchedule.CourseSectionId && x.Serial == courseSectionAddViewModel.CourseFixedSchedule.Serial).FirstOrDefault();

                    if (fixedScheduleDataUpdate != null)
                    {
                        var roomCapacity = this.context?.Rooms.FirstOrDefault(e => e.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && e.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && e.RoomId == courseSectionAddViewModel.CourseFixedSchedule.RoomId && e.IsActive == true)?.Capacity;

                        if (roomCapacity == null || roomCapacity < courseSectionAddViewModel.CourseSection!.Seats)
                        {
                            courseSectionAddViewModel._message = "Invalid Seat Capacity";
                            courseSectionAddViewModel._failure = true;
                            return courseSectionAddViewModel;
                        }

                        // string[] meetingDays = courseSectionAddViewModel.courseSection.MeetingDays.Split("|");

                        //foreach (var meetingDay in meetingDays)
                        //{
                        if (courseSectionAddViewModel.CourseSection!.AllowRoomConflict != true)
                        {
                            var courseSectionList = this.context?.AllCourseSectionView.AsEnumerable().Where(c => c.TenantId == courseSectionAddViewModel.CourseSection.TenantId && c.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && c.CourseSectionId != courseSectionAddViewModel.CourseSection.CourseSectionId &&
                        ((c.FixedPeriodId != null && (c.FixedRoomId == courseSectionAddViewModel.CourseFixedSchedule.RoomId && c.FixedPeriodId == courseSectionAddViewModel.CourseFixedSchedule.PeriodId && (Regex.IsMatch((courseSectionAddViewModel.CourseSection.MeetingDays ?? "").ToLower(), (c.FixedDays ?? "").ToLower(), RegexOptions.IgnoreCase)))) ||
                         (c.VarPeriodId != null && (c.VarRoomId == courseSectionAddViewModel.CourseFixedSchedule.RoomId && c.VarPeriodId == courseSectionAddViewModel.CourseFixedSchedule.PeriodId && (courseSectionAddViewModel.CourseSection.MeetingDays ?? "").ToLower().Contains((c.VarDay ?? "").ToLower()))) ||
                         (c.CalPeriodId != null && (c.CalRoomId == courseSectionAddViewModel.CourseFixedSchedule.RoomId && c.CalPeriodId == courseSectionAddViewModel.CourseFixedSchedule!.PeriodId && (courseSectionAddViewModel.CourseSection.MeetingDays ?? "").ToLower().Contains((c.CalDay ?? "").ToLower()))))
                        && c.AcademicYear == academicYear && ((courseSectionAddViewModel.CourseSection.DurationStartDate >= c.DurationStartDate && courseSectionAddViewModel.CourseSection.DurationStartDate <= c.DurationEndDate) || (courseSectionAddViewModel.CourseSection.DurationEndDate >= c.DurationStartDate && courseSectionAddViewModel.CourseSection.DurationEndDate <= c.DurationEndDate))).ToList();

                            if (courseSectionList != null && courseSectionList.Any())
                            {
                                courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Scheduling.";
                                courseSectionAddViewModel._failure = true;
                                return courseSectionAddViewModel;
                            }
                        }
                        //}


                        //string[] meetingDays = courseSectionAddViewModel.courseSection.MeetingDays.Split("|");
                        //string message = null;

                        //foreach (var meetingDay in meetingDays)
                        //{
                        //    var courseSectionList = this.context?.CourseSection.
                        //    Join(this.context?.CourseFixedSchedule,
                        //     cs => cs.CourseSectionId, cfs => cfs.CourseSectionId,
                        //    (cs, cfs) => new { cs, cfs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cfs.RoomId == courseSectionAddViewModel.courseFixedSchedule.RoomId && c.cfs.PeriodId == courseSectionAddViewModel.courseFixedSchedule.PeriodId && c.cs.CourseSectionId != courseSectionAddViewModel.courseSection.CourseSectionId && c.cfs.Serial != courseSectionAddViewModel.courseFixedSchedule.Serial && c.cs.MeetingDays.Contains(meetingDay) && c.cs.AcademicYear==academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //    if (courseSectionList.Count > 0)
                        //    {
                        //        courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Fixed Scheduling.";
                        //        courseSectionAddViewModel._failure = true;
                        //        return courseSectionAddViewModel;
                        //    }

                        //    Days days = new Days();
                        //    var Day = Enum.GetName(days.GetType(), Convert.ToInt32(meetingDay));

                        //    var variableScheduleDataList = this.context?.CourseSection.
                        //    Join(this.context?.CourseVariableSchedule,
                        //     cs => cs.CourseSectionId, cvs => cvs.CourseSectionId,
                        //    (cs, cvs) => new { cs, cvs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cvs.RoomId == courseSectionAddViewModel.courseFixedSchedule.RoomId && c.cvs.PeriodId == courseSectionAddViewModel.courseFixedSchedule.PeriodId && c.cvs.Day.ToLower() == Day.ToLower() && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //    if (variableScheduleDataList.Count > 0)
                        //    {
                        //        courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Variable Scheduling.";
                        //        courseSectionAddViewModel._failure = true;
                        //        return courseSectionAddViewModel;
                        //    }

                        //    var calenderScheduleList = this.context?.CourseSection.
                        //    Join(this.context?.CourseCalendarSchedule,
                        //     cs => cs.CourseSectionId, ccs => ccs.CourseSectionId,
                        //    (cs, ccs) => new { cs, ccs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.ccs.RoomId == courseSectionAddViewModel.courseFixedSchedule.RoomId && c.ccs.PeriodId == courseSectionAddViewModel.courseFixedSchedule.PeriodId && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //    if (calenderScheduleList.Count > 0)
                        //    {
                        //        foreach (var calenderSchedule in calenderScheduleList)
                        //        {
                        //            var calenderDay = calenderSchedule.ccs.Date.Value.DayOfWeek.ToString();

                        //            if (calenderDay != null)
                        //            {
                        //                int calenderDayValue = (int)Enum.Parse(typeof(Days), calenderDay);
                        //                if (calenderDayValue.ToString() == meetingDay)
                        //                {
                        //                    if (message != null)
                        //                    {
                        //                        message = calenderSchedule.ccs.Date.Value.Date.ToString("yyyy-MM-dd") + "(" + calenderDay + ")" + "," + message;
                        //                    }
                        //                    else
                        //                    {
                        //                        message = calenderSchedule.ccs.Date.Value.Date.ToString("yyyy-MM-dd") + "(" + calenderDay + ")";
                        //                    }
                        //                    courseSectionAddViewModel._message = message + "is associate with Course Calender Schedule";
                        //                    courseSectionAddViewModel._failure = true;
                        //                    return courseSectionAddViewModel;
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        courseSectionAddViewModel.CourseFixedSchedule.CreatedBy = fixedScheduleDataUpdate.CreatedBy;
                        courseSectionAddViewModel.CourseFixedSchedule.CreatedOn = fixedScheduleDataUpdate.CreatedOn;
                        courseSectionAddViewModel.CourseFixedSchedule.UpdatedOn = DateTime.UtcNow;
                        this.context?.Entry(fixedScheduleDataUpdate).CurrentValues.SetValues(courseSectionAddViewModel.CourseFixedSchedule);
                    }
                    courseSectionAddViewModel._failure = false;
                    courseSectionAddViewModel.CourseSection!.ScheduleType = "Fixed Schedule (1)";
                }
            }
            catch (Exception es)
            {
                courseSectionAddViewModel._failure = true;
                courseSectionAddViewModel._message = es.Message;
            }
            return courseSectionAddViewModel;
        }

        /// <summary>
        /// Update Variable Course Section Schedule
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        private CourseSectionAddViewModel UpdateVariableSchedule(CourseSectionAddViewModel courseSectionAddViewModel, decimal? academicYear)
        {
            try
            {
                if (courseSectionAddViewModel.CourseVariableScheduleList.Count > 0)
                {
                    var variableScheduleDataUpdate = this.context?.CourseVariableSchedule.Where(x => x.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && x.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && x.CourseId == courseSectionAddViewModel.CourseSection.CourseId && x.CourseSectionId == courseSectionAddViewModel.CourseSection.CourseSectionId).ToList();

                    if (variableScheduleDataUpdate != null)
                    {
                        this.context?.CourseVariableSchedule.RemoveRange(variableScheduleDataUpdate);
                        this.context?.SaveChanges();
                    }

                    int? variablescheduleSerial = 1;

                    var CourseSectionvariablescheduleData = this.context?.CourseVariableSchedule.Where(x => x.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && x.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId).OrderByDescending(x => x.Serial).FirstOrDefault();

                    if (CourseSectionvariablescheduleData != null)
                    {
                        variablescheduleSerial = CourseSectionvariablescheduleData.Serial + 1;
                    }

                    foreach (var courseVariableSchedules in courseSectionAddViewModel.CourseVariableScheduleList)
                    {

                        List<CourseVariableSchedule> courseVariableScheduleList = new List<CourseVariableSchedule>();

                        if (courseSectionAddViewModel.CourseSection!.AllowRoomConflict != true)
                        {
                            var courseSectionList = this.context?.AllCourseSectionView.Where(c => c.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && c.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId &&
                        ((c.FixedPeriodId != null && (c.FixedRoomId == courseVariableSchedules.RoomId && c.FixedPeriodId == courseVariableSchedules.PeriodId && (c.FixedDays ?? "").Contains(courseVariableSchedules!.Day!))) ||
                        (c.VarPeriodId != null && (c.VarRoomId == courseVariableSchedules.RoomId && c.VarPeriodId == courseVariableSchedules.PeriodId && c.VarDay == courseVariableSchedules.Day)) ||
                        (c.CalPeriodId != null && (c.CalRoomId == courseVariableSchedules.RoomId && c.CalPeriodId == courseVariableSchedules.PeriodId && c.CalDay == courseVariableSchedules.Day)))
                        && c.AcademicYear == academicYear && ((courseSectionAddViewModel.CourseSection.DurationStartDate >= c.DurationStartDate && courseSectionAddViewModel.CourseSection.DurationStartDate <= c.DurationEndDate) || (courseSectionAddViewModel.CourseSection.DurationEndDate >= c.DurationStartDate && courseSectionAddViewModel.CourseSection.DurationEndDate <= c.DurationEndDate))).ToList();

                            if (courseSectionList != null && courseSectionList.Any())
                            {
                                courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Scheduling.";
                                courseSectionAddViewModel._failure = true;
                                return courseSectionAddViewModel;
                            }
                        }

                        //string message = null;

                        //Days days = new Days();

                        //var Day = Enum.GetName(days.GetType(), Convert.ToInt32(courseVariableSchedules.Day));

                        //var variableScheduleDataList = this.context?.CourseSection.
                        //            Join(this.context?.CourseVariableSchedule,
                        //            cs => cs.CourseSectionId, cvs => cvs.CourseSectionId,
                        //            (cs, cvs) => new { cs, cvs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cvs.RoomId == courseVariableSchedules.RoomId && c.cvs.PeriodId == courseVariableSchedules.PeriodId && c.cvs.Day.ToLower() == Day.ToLower() && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //if (variableScheduleDataList.Count > 0)
                        //{
                        //    courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Variable Scheduling";
                        //    courseSectionAddViewModel._failure = true;
                        //    return courseSectionAddViewModel;
                        //}

                        //var courseSectionList = this.context?.CourseSection.
                        //            Join(this.context?.CourseFixedSchedule,
                        //            cs => cs.CourseSectionId, cf => cf.CourseSectionId,
                        //            (cs, cf) => new { cs, cf }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cf.RoomId == courseVariableSchedules.RoomId && c.cf.PeriodId == courseVariableSchedules.PeriodId && c.cs.MeetingDays.Contains(courseVariableSchedules.Day) && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //if (courseSectionList.Count > 0)
                        //{
                        //    courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Fixed Scheduling";
                        //    courseSectionAddViewModel._failure = true;
                        //    return courseSectionAddViewModel;
                        //}

                        //var calenderScheduleList = this.context?.CourseSection.
                        //        Join(this.context?.CourseCalendarSchedule,
                        //        cs => cs.CourseSectionId, ccs => ccs.CourseSectionId,
                        //        (cs, ccs) => new { cs, ccs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.ccs.RoomId == courseVariableSchedules.RoomId && c.ccs.PeriodId == courseVariableSchedules.PeriodId && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //if (calenderScheduleList.Count > 0)
                        //{
                        //    foreach (var calenderSchedule in calenderScheduleList)
                        //    {
                        //        var calenderDay = calenderSchedule.ccs.Date.Value.DayOfWeek.ToString();

                        //        if (calenderDay != null)
                        //        {
                        //            int dayValue = (int)Enum.Parse(typeof(Days), calenderDay);

                        //            if (dayValue.ToString() == courseVariableSchedules.Day)
                        //            {
                        //                if (message != null)
                        //                {
                        //                    message = calenderSchedule.ccs.Date.Value.Date.ToString("yyyy-MM-dd") + "(" + calenderDay + ")" + "," + message;
                        //                }
                        //                else
                        //                {
                        //                    message = calenderSchedule.ccs.Date.Value.Date.ToString("yyyy-MM-dd") + "(" + calenderDay + ")";
                        //                }
                        //                courseSectionAddViewModel._message = message + "is associate with Course Calender Schedule";
                        //                courseSectionAddViewModel._failure = true;
                        //                return courseSectionAddViewModel;
                        //            }
                        //        }
                        //    }
                        //}

                        var courseVariableScheduleAdd = new CourseVariableSchedule()
                        {
                            TenantId = courseSectionAddViewModel.CourseSection!.TenantId,
                            SchoolId = courseSectionAddViewModel.CourseSection.SchoolId,
                            CourseId = courseSectionAddViewModel.CourseSection.CourseId,
                            CourseSectionId = courseSectionAddViewModel.CourseSection.CourseSectionId,
                            GradeScaleId = courseSectionAddViewModel.CourseSection.GradeScaleId,
                            Serial = (int)variablescheduleSerial,
                            BlockId = courseVariableSchedules.BlockId,
                            Day = courseVariableSchedules.Day,
                            PeriodId = courseVariableSchedules.PeriodId,
                            RoomId = courseVariableSchedules.RoomId,
                            TakeAttendance = courseVariableSchedules.TakeAttendance,
                            CreatedBy = courseSectionAddViewModel.CourseSection.CreatedBy,
                            UpdatedBy = courseSectionAddViewModel.CourseSection.UpdatedBy,
                            UpdatedOn = DateTime.UtcNow
                        };
                        courseVariableScheduleList.Add(courseVariableScheduleAdd);
                        variablescheduleSerial++;
                        this.context?.CourseVariableSchedule.AddRange(courseVariableScheduleList);
                    }
                }
                //}
                courseSectionAddViewModel._failure = false;
                courseSectionAddViewModel.CourseSection!.ScheduleType = "Variable Schedule (2)";

            }
            catch (Exception es)
            {
                courseSectionAddViewModel._failure = true;
                courseSectionAddViewModel._message = es.Message;
            }
            return courseSectionAddViewModel;
        }

        /// <summary>
        /// Update Calendar Course Section Schedule
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        private CourseSectionAddViewModel UpdateCalendarSchedule(CourseSectionAddViewModel courseSectionAddViewModel, decimal? academicYear)
        {
            try
            {
                if (courseSectionAddViewModel.CourseCalendarScheduleList.Count > 0)
                {
                    var calenderScheduleDataUpdate = this.context?.CourseCalendarSchedule.Where(x => x.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && x.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && x.CourseId == courseSectionAddViewModel.CourseSection.CourseId && x.CourseSectionId == courseSectionAddViewModel.CourseSection.CourseSectionId).ToList();

                    if (calenderScheduleDataUpdate != null)
                    {
                        this.context?.CourseCalendarSchedule.RemoveRange(calenderScheduleDataUpdate);
                        this.context?.SaveChanges();
                    }

                    int? calendarscheduleSerial = 1;

                    var CourseSectioncalendarscheduleData = this.context?.CourseCalendarSchedule.Where(x => x.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && x.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId).OrderByDescending(x => x.Serial).FirstOrDefault();

                    if (CourseSectioncalendarscheduleData != null)
                    {
                        calendarscheduleSerial = CourseSectioncalendarscheduleData.Serial + 1;
                    }

                    foreach (var courseCalendarSchedule in courseSectionAddViewModel.CourseCalendarScheduleList)
                    {
                        List<CourseCalendarSchedule> courseCalendarScheduleList = new List<CourseCalendarSchedule>();

                        if (courseSectionAddViewModel.CourseSection!.AllowRoomConflict != true)
                        {
                            var calenderDay = courseCalendarSchedule.Date!.Value.DayOfWeek.ToString();

                            var courseSectionList = this.context?.AllCourseSectionView.Where(c => c.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && c.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId &&
                            ((c.FixedRoomId != null && (c.FixedRoomId == courseCalendarSchedule.RoomId && c.FixedPeriodId == courseCalendarSchedule.PeriodId && (c.FixedDays ?? "").Contains(calenderDay))) ||
                            (c.VarPeriodId != null && (c.VarRoomId == courseCalendarSchedule.RoomId && c.VarPeriodId == courseCalendarSchedule.PeriodId && c.VarDay == calenderDay)) ||
                            (c.CalPeriodId != null && (c.CalRoomId == courseCalendarSchedule.RoomId && c.CalPeriodId == courseCalendarSchedule.PeriodId && c.CalDay == calenderDay)))
                            && c.AcademicYear == academicYear && ((courseSectionAddViewModel.CourseSection.DurationStartDate >= c.DurationStartDate && courseSectionAddViewModel.CourseSection.DurationStartDate <= c.DurationEndDate) || (courseSectionAddViewModel.CourseSection.DurationEndDate >= c.DurationStartDate && courseSectionAddViewModel.CourseSection.DurationEndDate <= c.DurationEndDate))).ToList();

                            if (courseSectionList != null && courseSectionList.Any())
                            {
                                courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Scheduling.";
                                courseSectionAddViewModel._failure = true;
                                return courseSectionAddViewModel;
                            }
                        }

                        //var calenderDay = courseCalendarSchedule.Date.Value.DayOfWeek.ToString();

                        //if (calenderDay != null)
                        //{
                        //    int dayValue = (int)Enum.Parse(typeof(Days), calenderDay);

                        //    var courseSectionList = this.context?.CourseSection.
                        //        Join(this.context?.CourseFixedSchedule,
                        //        cs => cs.CourseSectionId, cf => cf.CourseSectionId,
                        //        (cs, cf) => new { cs, cf }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cf.RoomId == courseCalendarSchedule.RoomId && c.cf.PeriodId == courseCalendarSchedule.PeriodId && c.cs.MeetingDays.Contains(dayValue.ToString()) && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //    if (courseSectionList.Count > 0)
                        //    {
                        //        courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Fixed Scheduling";
                        //        courseSectionAddViewModel._failure = true;
                        //        return courseSectionAddViewModel;
                        //    }
                        //}

                        //var variableScheduleDataList = this.context?.CourseSection.
                        //        Join(this.context?.CourseVariableSchedule,
                        //        cs => cs.CourseSectionId, cvs => cvs.CourseSectionId,
                        //        (cs, cvs) => new { cs, cvs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cvs.RoomId == courseCalendarSchedule.RoomId && c.cvs.PeriodId == courseCalendarSchedule.PeriodId && c.cvs.Day.ToLower() == calenderDay.ToLower() && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //if (variableScheduleDataList.Count > 0)
                        //{
                        //    courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Variable Scheduling";
                        //    courseSectionAddViewModel._failure = true;
                        //    return courseSectionAddViewModel;
                        //}

                        //var calenderScheduleDataList = this.context?.CourseSection.
                        //        Join(this.context?.CourseCalendarSchedule,
                        //        cs => cs.CourseSectionId, ccs => ccs.CourseSectionId,
                        //        (cs, ccs) => new { cs, ccs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.ccs.RoomId == courseCalendarSchedule.RoomId && c.ccs.PeriodId == courseCalendarSchedule.PeriodId && c.ccs.Date == courseCalendarSchedule.Date && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //if (calenderScheduleDataList.Count > 0)
                        //{
                        //    courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Calendar Scheduling.";
                        //    courseSectionAddViewModel._failure = true;
                        //    return courseSectionAddViewModel;
                        //}
                        var courseCalenderSchedule = new CourseCalendarSchedule()
                        {
                            TenantId = courseSectionAddViewModel.CourseSection!.TenantId,
                            SchoolId = courseSectionAddViewModel.CourseSection.SchoolId,
                            CourseId = courseSectionAddViewModel.CourseSection.CourseId,
                            CourseSectionId = courseSectionAddViewModel.CourseSection.CourseSectionId,
                            GradeScaleId = courseSectionAddViewModel.CourseSection.GradeScaleId,
                            Serial = (int)calendarscheduleSerial,
                            BlockId = courseCalendarSchedule.BlockId,
                            Date = courseCalendarSchedule.Date,
                            PeriodId = courseCalendarSchedule.PeriodId,
                            RoomId = courseCalendarSchedule.RoomId,
                            TakeAttendance = courseCalendarSchedule.TakeAttendance,
                            CreatedBy = courseSectionAddViewModel.CourseSection.CreatedBy,
                            UpdatedBy = courseSectionAddViewModel.CourseSection.UpdatedBy,
                            UpdatedOn = DateTime.UtcNow
                        };
                        courseCalendarScheduleList.Add(courseCalenderSchedule);
                        calendarscheduleSerial++;
                        this.context?.CourseCalendarSchedule.AddRange(courseCalendarScheduleList);
                    }
                }
                courseSectionAddViewModel.CourseSection!.ScheduleType = "Calendar Schedule (3)";
                courseSectionAddViewModel._failure = false;
            }
            catch (Exception es)
            {
                courseSectionAddViewModel._failure = true;
                courseSectionAddViewModel._message = es.Message;
            }
            return courseSectionAddViewModel;
        }

        /// <summary>
        /// Update Block Course Section Schedule
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        private CourseSectionAddViewModel UpdateBlockSchedule(CourseSectionAddViewModel courseSectionAddViewModel, decimal? academicYear)
        {
            try
            {
                if (courseSectionAddViewModel.CourseBlockScheduleList.Count > 0)
                {
                    var blockScheduleDataUpdate = this.context?.CourseBlockSchedule.Where(x => x.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && x.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && x.CourseId == courseSectionAddViewModel.CourseSection.CourseId && x.CourseSectionId == courseSectionAddViewModel.CourseSection.CourseSectionId).ToList();

                    if (blockScheduleDataUpdate!=null && blockScheduleDataUpdate.Any())
                    {
                        this.context?.CourseBlockSchedule.RemoveRange(blockScheduleDataUpdate);
                        this.context?.SaveChanges();
                    }

                    int? blockscheduleSerial = 1;

                    var CourseSectionblockscheduleData = this.context?.CourseBlockSchedule.Where(x => x.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && x.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId).OrderByDescending(x => x.Serial).FirstOrDefault();

                    if (CourseSectionblockscheduleData != null)
                    {
                        blockscheduleSerial = CourseSectionblockscheduleData.Serial + 1;
                    }

                    foreach (var courseBlockSchedule in courseSectionAddViewModel.CourseBlockScheduleList)
                    {
                        List<CourseBlockSchedule> courseBlockScheduleList = new List<CourseBlockSchedule>();

                        if (courseSectionAddViewModel.CourseSection!.AllowRoomConflict != true)
                        {
                            var blockScheduleData = this.context?.AllCourseSectionView.Where(c => c.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && c.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && c.BlockRoomId == courseBlockSchedule.RoomId && c.BlockPeriodId == courseBlockSchedule.PeriodId && c.BlockId == courseBlockSchedule.BlockId && c.AcademicYear == academicYear && ((courseSectionAddViewModel.CourseSection.DurationStartDate >= c.DurationStartDate && courseSectionAddViewModel.CourseSection.DurationStartDate <= c.DurationEndDate) || (courseSectionAddViewModel.CourseSection.DurationEndDate >= c.DurationStartDate && courseSectionAddViewModel.CourseSection.DurationEndDate <= c.DurationEndDate))).ToList();

                            if (blockScheduleData != null && blockScheduleData.Any())
                            {
                                courseSectionAddViewModel._failure = true;
                                courseSectionAddViewModel._message = "Room is not available for this block and period";
                                return courseSectionAddViewModel;
                            }
                        }

                        var courseBlockScheduleAdd = new CourseBlockSchedule()
                        {
                            TenantId = courseSectionAddViewModel.CourseSection!.TenantId,
                            SchoolId = courseSectionAddViewModel.CourseSection.SchoolId,
                            CourseId = courseSectionAddViewModel.CourseSection.CourseId,
                            CourseSectionId = courseSectionAddViewModel.CourseSection.CourseSectionId,
                            GradeScaleId = courseSectionAddViewModel.CourseSection.GradeScaleId,
                            Serial = (int)blockscheduleSerial,
                            BlockId = courseBlockSchedule.BlockId,
                            PeriodId = courseBlockSchedule.PeriodId,
                            RoomId = courseBlockSchedule.RoomId,
                            TakeAttendance = courseBlockSchedule.TakeAttendance,
                            CreatedBy = courseSectionAddViewModel.CourseSection.CreatedBy,
                            UpdatedBy = courseSectionAddViewModel.CourseSection.UpdatedBy,
                            UpdatedOn = DateTime.UtcNow
                        };

                        courseBlockScheduleList.Add(courseBlockScheduleAdd);
                        blockscheduleSerial++;
                        this.context?.CourseBlockSchedule.AddRange(courseBlockScheduleList);
                    }
                }
                courseSectionAddViewModel!.CourseSection!.ScheduleType = "Block Schedule (4)";
                courseSectionAddViewModel._failure = false;
            }
            catch (Exception es)
            {
                courseSectionAddViewModel._failure = true;
                courseSectionAddViewModel._message = es.Message;
            }
            return courseSectionAddViewModel;
        }

        /// <summary>
        /// Delete Course Section
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <returns></returns>
        public CourseSectionAddViewModel DeleteCourseSection(CourseSectionAddViewModel courseSectionAddViewModel)
        {
            using var transaction = this.context?.Database.BeginTransaction();
            {
                try
                {
                    var studentScheduleData = this.context?.StudentCoursesectionSchedule.FirstOrDefault(e => e.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && e.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && e.CourseSectionId == courseSectionAddViewModel.CourseSection.CourseSectionId && (e.IsDropped == null || e.IsDropped == false));

                    var staffScheduleData = this.context?.StaffCoursesectionSchedule.FirstOrDefault(e => e.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && e.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && e.CourseSectionId == courseSectionAddViewModel.CourseSection.CourseSectionId && (e.IsDropped == null || e.IsDropped == false));

                    var gradebookGradeData = this.context?.GradebookGrades.FirstOrDefault(e => e.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && e.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && e.CourseSectionId == courseSectionAddViewModel.CourseSection.CourseSectionId);

                    var studentAttendanceData = this.context?.StudentAttendance.FirstOrDefault(e => e.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && e.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && e.CourseSectionId == courseSectionAddViewModel.CourseSection.CourseSectionId);

                    var studentFinalGradeData = this.context?.StudentFinalGrade.FirstOrDefault(e => e.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && e.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && e.CourseSectionId == courseSectionAddViewModel.CourseSection.CourseSectionId);

                    var studentEffortGradeData = this.context?.StudentEffortGradeMaster.FirstOrDefault(e => e.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && e.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && e.CourseSectionId == courseSectionAddViewModel.CourseSection.CourseSectionId);

                    var assignmentTypeDataData = this.context?.AssignmentType.FirstOrDefault(e => e.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && e.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && e.CourseSectionId == courseSectionAddViewModel.CourseSection.CourseSectionId);

                    if (studentScheduleData != null || staffScheduleData != null || gradebookGradeData != null || studentAttendanceData != null || studentFinalGradeData != null || studentEffortGradeData != null || assignmentTypeDataData != null)
                    {
                        courseSectionAddViewModel._failure = true;
                        courseSectionAddViewModel._message = "This Course Section could not be deleted. It has association";
                    }
                    else
                    {
                        var courseSectionDelete = this.context?.CourseSection.Include(x => x.StudentCoursesectionSchedule).Include(y => y.StaffCoursesectionSchedule).Include(z => z.GradebookConfiguration).FirstOrDefault(e => e.TenantId == courseSectionAddViewModel.CourseSection!.TenantId && e.SchoolId == courseSectionAddViewModel.CourseSection.SchoolId && e.CourseSectionId == courseSectionAddViewModel.CourseSection.CourseSectionId);

                        if (courseSectionDelete != null)
                        {
                            if (courseSectionDelete.ScheduleType == "Fixed Schedule (1)")
                            {
                                var courseFixedScheduleDelete = this.context?.CourseFixedSchedule.FirstOrDefault(x => x.TenantId == courseSectionDelete.TenantId && x.SchoolId == courseSectionDelete.SchoolId && x.CourseSectionId == courseSectionDelete.CourseSectionId);

                                if (courseFixedScheduleDelete != null)
                                {
                                    this.context?.CourseFixedSchedule.Remove(courseFixedScheduleDelete);
                                }
                            }

                            if (courseSectionDelete.ScheduleType == "Variable Schedule (2)")
                            {
                                var courseVariableScheduleDelete = this.context?.CourseVariableSchedule.Where(x => x.TenantId == courseSectionDelete.TenantId && x.SchoolId == courseSectionDelete.SchoolId && x.CourseSectionId == courseSectionDelete.CourseSectionId).ToList();

                                if (courseVariableScheduleDelete != null)
                                {
                                    this.context?.CourseVariableSchedule.RemoveRange(courseVariableScheduleDelete);
                                }
                            }

                            if (courseSectionDelete.ScheduleType == "Calendar Schedule (3)")
                            {
                                var courseCalendarScheduleDelete = this.context?.CourseCalendarSchedule.Where(x => x.TenantId == courseSectionDelete.TenantId && x.SchoolId == courseSectionDelete.SchoolId && x.CourseSectionId == courseSectionDelete.CourseSectionId).ToList();

                                if (courseCalendarScheduleDelete != null)
                                {
                                    this.context?.CourseCalendarSchedule.RemoveRange(courseCalendarScheduleDelete);
                                }
                            }

                            if (courseSectionDelete.ScheduleType == "Block Schedule (4)")
                            {
                                var courseBlockScheduleDelete = this.context?.CourseBlockSchedule.Where(x => x.TenantId == courseSectionDelete.TenantId && x.SchoolId == courseSectionDelete.SchoolId && x.CourseSectionId == courseSectionDelete.CourseSectionId).ToList();

                                if (courseBlockScheduleDelete != null)
                                {
                                    this.context?.CourseBlockSchedule.RemoveRange(courseBlockScheduleDelete);
                                }
                            }

                            if (courseSectionDelete.StudentCoursesectionSchedule?.Any() == true)
                            {
                                this.context?.StudentCoursesectionSchedule.RemoveRange(courseSectionDelete.StudentCoursesectionSchedule);
                            }

                            if (courseSectionDelete.StaffCoursesectionSchedule?.Any() == true)
                            {
                                this.context?.StaffCoursesectionSchedule.RemoveRange(courseSectionDelete.StaffCoursesectionSchedule);
                            }

                            if (courseSectionDelete.GradebookConfiguration?.Any() == true)
                            {
                                var gradebookConfigurationData = this.context?.GradebookConfiguration.Include(a => a.GradebookConfigurationGradescale).Include(b => b.GradebookConfigurationYear).Include(c => c.GradebookConfigurationSemester).Include(d => d.GradebookConfigurationQuarter).Include(e => e.GradebookConfigurationProgressPeriods).FirstOrDefault(x => x.TenantId == courseSectionDelete.TenantId && x.SchoolId == courseSectionDelete.SchoolId && x.CourseSectionId == courseSectionDelete.CourseSectionId);

                                if (gradebookConfigurationData != null)
                                {
                                    if (gradebookConfigurationData.GradebookConfigurationGradescale?.Any() == true)
                                    {
                                        this.context?.GradebookConfigurationGradescale.RemoveRange(gradebookConfigurationData.GradebookConfigurationGradescale);
                                    }

                                    if (gradebookConfigurationData.GradebookConfigurationYear?.Any() == true)
                                    {
                                        this.context?.GradebookConfigurationYear.RemoveRange(gradebookConfigurationData.GradebookConfigurationYear);
                                    }

                                    if (gradebookConfigurationData.GradebookConfigurationSemester?.Any() == true)
                                    {
                                        this.context?.GradebookConfigurationSemester.RemoveRange(gradebookConfigurationData.GradebookConfigurationSemester);
                                    }

                                    if (gradebookConfigurationData.GradebookConfigurationQuarter?.Any() == true)
                                    {
                                        this.context?.GradebookConfigurationQuarter.RemoveRange(gradebookConfigurationData.GradebookConfigurationQuarter);
                                    }

                                    if (gradebookConfigurationData.GradebookConfigurationProgressPeriods.Any() == true)
                                    {
                                        this.context?.GradebookConfigurationProgressPeriods.RemoveRange(gradebookConfigurationData.GradebookConfigurationProgressPeriods);
                                    }
                                }

                                this.context?.GradebookConfiguration.RemoveRange(courseSectionDelete.GradebookConfiguration);
                            }


                            this.context?.CourseSection.Remove(courseSectionDelete);
                            this.context?.SaveChanges();

                            courseSectionAddViewModel._failure = false;
                            courseSectionAddViewModel._message = "Course Section deleted successfullyy";
                        }
                        else
                        {
                            courseSectionAddViewModel._failure = true;
                            courseSectionAddViewModel._message = NORECORDFOUND;
                        }
                    }

                    transaction?.Commit();
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    courseSectionAddViewModel._message = es.Message;
                    courseSectionAddViewModel._failure = true;
                }
                return courseSectionAddViewModel;
            }
        }

        /// <summary>
        /// Get All Course Standard For Course
        /// </summary>
        /// <param name="courseStandardForCourseViewModel"></param>
        /// <returns></returns>
        public CourseStandardForCourseViewModel GetAllCourseStandardForCourse(CourseStandardForCourseViewModel courseStandardForCourseViewModel)
        {
            CourseStandardForCourseViewModel courseStandardForCourse = new CourseStandardForCourseViewModel();
            try
            {
                courseStandardForCourse.TenantId = courseStandardForCourseViewModel.TenantId;
                courseStandardForCourse.SchoolId = courseStandardForCourseViewModel.SchoolId;
                courseStandardForCourse.CourseId = courseStandardForCourseViewModel.CourseId;
                courseStandardForCourse._tenantName = courseStandardForCourseViewModel._tenantName;
                courseStandardForCourse._token = courseStandardForCourseViewModel._token;
                courseStandardForCourse._userName = courseStandardForCourseViewModel._userName;

                var courseStandarddata = this.context?.CourseStandard.Include(x => x.GradeUsStandard).Where(x => x.TenantId == courseStandardForCourseViewModel.TenantId && x.SchoolId == courseStandardForCourseViewModel.SchoolId && x.CourseId == courseStandardForCourseViewModel.CourseId).ToList();
                
                if (courseStandarddata!=null && courseStandarddata.Any())
                {
                    courseStandardForCourse.CourseStandards = courseStandarddata;
                }
                else
                {
                    courseStandardForCourse._failure = true;
                    courseStandardForCourse._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                courseStandardForCourse.CourseStandards = null!;
                courseStandardForCourse._failure = true;
                courseStandardForCourse._message = es.Message;
            }
            return courseStandardForCourse;
        }

        ///// <summary>
        ///// Delete Course Section  For Specific Schedule Type
        ///// </summary>
        ///// <param name="deleteScheduleViewModel"></param>
        ///// <returns></returns>
        //public DeleteScheduleViewModel DeleteSchedule(DeleteScheduleViewModel deleteScheduleViewModel)
        //{
        //    try
        //    {
        //        if (deleteScheduleViewModel.ScheduleType.ToLower() == "variableschedule")
        //        {
        //            var variableScheduleData = this.context?.CourseVariableSchedule.FirstOrDefault(x => x.TenantId == deleteScheduleViewModel.TenantId && x.SchoolId == deleteScheduleViewModel.SchoolId && x.CourseId == deleteScheduleViewModel.CourseId && x.CourseSectionId == deleteScheduleViewModel.CourseSectionId && x.Serial == deleteScheduleViewModel.Serial);

        //            if (variableScheduleData != null)
        //            {
        //                this.context?.CourseVariableSchedule.Remove(variableScheduleData);
        //            }
        //        }

        //        if (deleteScheduleViewModel.ScheduleType.ToLower() == "blockschedule")
        //        {
        //            var blockScheduleData = this.context?.CourseBlockSchedule.FirstOrDefault(x => x.TenantId == deleteScheduleViewModel.TenantId && x.SchoolId == deleteScheduleViewModel.SchoolId && x.CourseId == deleteScheduleViewModel.CourseId && x.CourseSectionId == deleteScheduleViewModel.CourseSectionId && x.Serial == deleteScheduleViewModel.Serial);

        //            if (blockScheduleData != null)
        //            {
        //                this.context?.CourseBlockSchedule.Remove(blockScheduleData);
        //            }
        //        }

        //        if (deleteScheduleViewModel.ScheduleType.ToLower() == "calendarschedule")
        //        {
        //            var calendarScheduleData = this.context?.CourseCalendarSchedule.FirstOrDefault(x => x.TenantId == deleteScheduleViewModel.TenantId && x.SchoolId == deleteScheduleViewModel.SchoolId && x.CourseId == deleteScheduleViewModel.CourseId && x.CourseSectionId == deleteScheduleViewModel.CourseSectionId && x.Serial == deleteScheduleViewModel.Serial);

        //            if (calendarScheduleData != null)
        //            {
        //                this.context?.CourseCalendarSchedule.Remove(calendarScheduleData);
        //            }
        //        }

        //        this.context?.SaveChanges();
        //        deleteScheduleViewModel._message = "Schedule deleted successfullyy";
        //        deleteScheduleViewModel._failure = false;

        //    }
        //    catch (Exception es)
        //    {
        //        deleteScheduleViewModel._message = es.Message;
        //        deleteScheduleViewModel._failure = true;
        //    }
        //    return deleteScheduleViewModel;
        //}

        /// <summary>
        /// Search Course Section For Schedule
        /// </summary>
        /// <param name="searchCourseSectionViewModel"></param>
        /// <returns></returns>
        public SearchCourseSectionViewModel SearchCourseSectionForSchedule(SearchCourseSectionViewModel searchCourseSectionViewModel)
        {
            SearchCourseSectionViewModel searchCourseSection = new SearchCourseSectionViewModel();
            try
            {
                searchCourseSection.TenantId = searchCourseSectionViewModel.TenantId;
                searchCourseSection.SchoolId = searchCourseSectionViewModel.SchoolId;
                searchCourseSection._token = searchCourseSectionViewModel._token;
                searchCourseSection._tenantName = searchCourseSectionViewModel._tenantName;

                int? YrmarkingPeriodId = 0;
                int? SmtrmarkingPeriodId = 0;
                int? QtrmarkingPeriodId = 0;
                int? PrgrsprdmarkingPeriodId = 0;
                if (searchCourseSectionViewModel.MarkingPeriodId != null)
                {
                    var markingPeriod = searchCourseSectionViewModel.MarkingPeriodId.Split("_", StringSplitOptions.RemoveEmptyEntries);

                    if (markingPeriod.First() == "0")
                    {
                        YrmarkingPeriodId = Int32.Parse(markingPeriod.Last());
                    }
                    if (markingPeriod.First() == "1")
                    {
                        SmtrmarkingPeriodId = Int32.Parse(markingPeriod.Last());
                    }
                    if (markingPeriod.First() == "2")
                    {
                        QtrmarkingPeriodId = Int32.Parse(markingPeriod.Last());
                    }
                    if (markingPeriod.First() == "3")
                    {
                        PrgrsprdmarkingPeriodId = Int32.Parse(markingPeriod.Last());
                    }
                }
                //var todayDate = DateTime.UtcNow;

                var coursedata = this.context?.AllCourseSectionView.Where(x => x.TenantId == searchCourseSectionViewModel.TenantId && x.SchoolId == searchCourseSectionViewModel.SchoolId && (searchCourseSectionViewModel.MarkingPeriodStartDate == null || (x.DurationStartDate >= searchCourseSectionViewModel.MarkingPeriodStartDate || x.DurationEndDate >= searchCourseSectionViewModel.MarkingPeriodStartDate)) && (searchCourseSectionViewModel.CourseId == null || x.CourseId == searchCourseSectionViewModel.CourseId) && (searchCourseSectionViewModel.CourseSubject == null || x.CourseSubject == searchCourseSectionViewModel.CourseSubject) && (searchCourseSectionViewModel.CourseProgram == null || x.CourseProgram == searchCourseSectionViewModel.CourseProgram) && (searchCourseSectionViewModel.MarkingPeriodId == null || x.YrMarkingPeriodId == YrmarkingPeriodId || x.SmstrMarkingPeriodId == SmtrmarkingPeriodId || x.QtrMarkingPeriodId == QtrmarkingPeriodId || x.PrgrsprdMarkingPeriodId == PrgrsprdmarkingPeriodId));

                if (coursedata != null && coursedata.Any())
                {

                    var distinctCourseData = coursedata.Select(s => new AllCourseSectionView { TenantId = s.TenantId, SchoolId = s.SchoolId, CourseId = s.CourseId, CourseTitle = s.CourseTitle, CourseProgram = s.CourseProgram, CourseSubject = s.CourseSubject, AcademicYear = s.AcademicYear, CourseSectionId = s.CourseSectionId, CourseSectionName = s.CourseSectionName, YrMarkingPeriodId = s.YrMarkingPeriodId, SmstrMarkingPeriodId = s.SmstrMarkingPeriodId, QtrMarkingPeriodId = s.QtrMarkingPeriodId, PrgrsprdMarkingPeriodId = s.PrgrsprdMarkingPeriodId, IsActive = s.IsActive, DurationStartDate = s.DurationStartDate, DurationEndDate = s.DurationEndDate, Seats = s.Seats, CourseGradeLevel = s.CourseGradeLevel, GradeScaleId = s.GradeScaleId, AllowStudentConflict = s.AllowStudentConflict, AllowTeacherConflict = s.AllowTeacherConflict, ScheduleType = s.ScheduleType }).Distinct().ToList();

                    if (searchCourseSectionViewModel.ForStaff == true)
                    {
                        foreach (var CourseData in distinctCourseData)
                        {
                            var staffSchedule = this.context?.StaffCoursesectionSchedule.Include(x => x.StaffMaster).Where(x => x.TenantId == searchCourseSectionViewModel.TenantId && x.SchoolId == searchCourseSectionViewModel.SchoolId && x.CourseSectionId == CourseData.CourseSectionId && x.IsDropped != true).ToList();
                            if (staffSchedule != null && staffSchedule.Any())
                            {
                                foreach (var staff in staffSchedule)
                                {
                                    var staffName = staff.StaffMaster.FirstGivenName + " " + staff.StaffMaster.MiddleName + " " + staff.StaffMaster.LastFamilyName;
                                    CourseData.StaffName = CourseData.StaffName != null ? CourseData.StaffName + "|" + staffName : staffName;
                                }
                            }
                        }
                    }

                    if (searchCourseSectionViewModel.ForStudent == true)
                    {
                        foreach (var CourseData in distinctCourseData)
                        {
                            int? studentSchedule = null;
                            studentSchedule = this.context?.StudentCoursesectionSchedule.Where(x => x.TenantId == searchCourseSectionViewModel.TenantId && x.SchoolId == searchCourseSectionViewModel.SchoolId && x.CourseSectionId == CourseData.CourseSectionId && x.IsDropped != true).ToList().Count;

                            CourseData.AvailableSeat = CourseData.Seats - studentSchedule;

                            var staffScheduleData = this.context?.StaffCoursesectionSchedule.Include(x => x.StaffMaster).Where(x => x.TenantId == searchCourseSectionViewModel.TenantId && x.SchoolId == searchCourseSectionViewModel.SchoolId && x.CourseSectionId == CourseData.CourseSectionId && x.IsDropped != true).ToList();
                            if (staffScheduleData != null && staffScheduleData.Any())
                            {
                                foreach (var staff in staffScheduleData)
                                {
                                    var staffName = staff.StaffMaster.FirstGivenName + " " + staff.StaffMaster.MiddleName + " " + staff.StaffMaster.LastFamilyName;
                                    CourseData.StaffName = CourseData.StaffName != null ? CourseData.StaffName + "|" + staffName : staffName;
                                }
                            }


                            if (CourseData.ScheduleType == "Fixed Schedule (1)")
                            {
                                CourseData.FixedDays = coursedata.Where(x => x.CourseSectionId == CourseData.CourseSectionId).FirstOrDefault()?.FixedDays;
                            }
                            else if (CourseData.ScheduleType == "Variable Schedule (2)")
                            {
                                var variabaleScheduleData = coursedata.Where(x => x.CourseSectionId == CourseData.CourseSectionId).ToList();

                                foreach (var variabaleSchedule in variabaleScheduleData)
                                {
                                    var varDay = variabaleSchedule.VarDay;
                                    CourseData.VarDay = CourseData.VarDay != null ? CourseData.VarDay + "|" + varDay : varDay;
                                }
                            }

                            if (CourseData.PrgrsprdMarkingPeriodId != null)
                            {
                                CourseData.MarkingPeriod = this.context?.ProgressPeriods.FirstOrDefault(x => x.TenantId == searchCourseSectionViewModel.TenantId && x.SchoolId == searchCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseData.PrgrsprdMarkingPeriodId)?.Title;
                            }
                            else if (CourseData.QtrMarkingPeriodId != null)
                            {
                                CourseData.MarkingPeriod = this.context?.Quarters.FirstOrDefault(x => x.TenantId == searchCourseSectionViewModel.TenantId && x.SchoolId == searchCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseData.QtrMarkingPeriodId)?.Title;
                            }
                            else if (CourseData.SmstrMarkingPeriodId != null)
                            {
                                CourseData.MarkingPeriod = this.context?.Semesters.FirstOrDefault(x => x.TenantId == searchCourseSectionViewModel.TenantId && x.SchoolId == searchCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseData.SmstrMarkingPeriodId)?.Title;
                            }
                            else if (CourseData.YrMarkingPeriodId != null)
                            {
                                CourseData.MarkingPeriod = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == searchCourseSectionViewModel.TenantId && x.SchoolId == searchCourseSectionViewModel.SchoolId && x.MarkingPeriodId == CourseData.YrMarkingPeriodId)?.Title;
                            }
                        }
                    }
                    searchCourseSection.AllCourseSectionViewList = distinctCourseData;
                    searchCourseSection._failure = false;
                }
                else
                {
                    searchCourseSection._message = NORECORDFOUND;
                    searchCourseSection._failure = true;
                }
            }
            catch (Exception ex)
            {
                searchCourseSection.AllCourseSectionViewList = null!;
                searchCourseSection._failure = true;
                searchCourseSection._message = ex.Message;
            }
            return searchCourseSection;
        }

        /// <summary>
        /// Get All Staff Schedule In Course Section & Course
        /// </summary>
        /// <param name="staffListViewModel"></param>
        /// <returns></returns>
        public StaffListViewModel GetAllStaffScheduleInCourseSection(StaffListViewModel staffListViewModel)
        {
            StaffListViewModel staffListView = new StaffListViewModel();
            try
            {
                staffListView.TenantId = staffListViewModel.TenantId;
                staffListView.SchoolId = staffListViewModel.SchoolId;
                staffListView.CourseId = staffListViewModel.CourseId;
                staffListView.CourseSectionId = staffListViewModel.CourseSectionId;
                staffListView._token = staffListViewModel._token;
                staffListView._tenantName = staffListViewModel._tenantName;

                var staffSchedule = this.context?.CourseSection.Include(x => x.SchoolYears).Include(x => x.Semesters).Include(x => x.Quarters).Include(x => x.StaffCoursesectionSchedule).ThenInclude(x => x.StaffMaster).Where(x => x.TenantId == staffListViewModel.TenantId && x.SchoolId == staffListViewModel.SchoolId && x.CourseId == staffListViewModel.CourseId && (staffListViewModel.CourseSectionId == null || x.CourseSectionId == staffListViewModel.CourseSectionId)).AsNoTracking().Select(e => new CourseSection
                {
                    TenantId = e.TenantId,
                    SchoolId = e.SchoolId,
                    CourseId = e.CourseId,
                    CourseSectionId = e.CourseSectionId,
                    CourseSectionName = e.CourseSectionName,
                    ScheduleType = e.ScheduleType,
                    YrMarkingPeriodId = e.YrMarkingPeriodId,
                    SmstrMarkingPeriodId = e.SmstrMarkingPeriodId,
                    QtrMarkingPeriodId = e.QtrMarkingPeriodId,
                    DurationStartDate = e.DurationStartDate,
                    DurationEndDate = e.DurationEndDate,
                    ProgressPeriods = e.ProgressPeriods != null ? new ProgressPeriods { Title = e.ProgressPeriods.Title, StartDate = e.ProgressPeriods.StartDate, EndDate = e.ProgressPeriods.EndDate, ShortName = e.ProgressPeriods.ShortName } : null,
                    Quarters = e.Quarters != null ? new Quarters { Title = e.Quarters.Title, StartDate = e.Quarters.StartDate, EndDate = e.Quarters.EndDate, ShortName = e.Quarters.ShortName } : null,
                    Semesters = e.Semesters != null ? new Semesters { Title = e.Semesters.Title, StartDate = e.Semesters.StartDate, EndDate = e.Semesters.EndDate, ShortName = e.Semesters.ShortName } : null,
                    SchoolYears = e.SchoolYears != null ? new SchoolYears { Title = e.SchoolYears.Title, StartDate = e.SchoolYears.StartDate, EndDate = e.SchoolYears.EndDate, ShortName = e.SchoolYears.ShortName } : null,
                    StaffCoursesectionSchedule = e.StaffCoursesectionSchedule.Where(d => d.IsDropped != true).Select(s => new StaffCoursesectionSchedule
                    {
                        TenantId = s.TenantId,
                        SchoolId = s.SchoolId,
                        StaffId = s.StaffId,
                        StaffGuid = s.StaffGuid,
                        CourseId = s.CourseId,
                        CourseSectionId = s.CourseSectionId,
                        CourseSectionName = s.CourseSectionName,
                        IsDropped = s.IsDropped,
                        MeetingDays = s.MeetingDays,
                        IsPrimaryStaff = s.IsPrimaryStaff,
                        StaffMaster = new StaffMaster
                        {
                            TenantId = s.StaffMaster.TenantId,
                            SchoolId = s.StaffMaster.SchoolId,
                            StaffId = s.StaffMaster.StaffId,
                            FirstGivenName = s.StaffMaster.FirstGivenName,
                            MiddleName = s.StaffMaster.MiddleName,
                            LastFamilyName = s.StaffMaster.LastFamilyName,
                            StaffInternalId = s.StaffMaster.StaffInternalId,
                            StaffPhoto = staffListViewModel.CourseSectionId != null ? s.StaffMaster.StaffPhoto : null,
                            FirstLanguage = s.StaffMaster.FirstLanguage,
                            SecondLanguage = s.StaffMaster.SecondLanguage,
                            ThirdLanguage = s.StaffMaster.ThirdLanguage,
                            FirstLanguageNavigation = s.StaffMaster.FirstLanguageNavigation,
                            SecondLanguageNavigation = s.StaffMaster.SecondLanguageNavigation,
                            ThirdLanguageNavigation = s.StaffMaster.ThirdLanguageNavigation,
                            JobTitle = s.StaffMaster.JobTitle,
                            PrimaryGradeLevelTaught = s.StaffMaster.PrimaryGradeLevelTaught,
                            OtherGradeLevelTaught = s.StaffMaster.OtherGradeLevelTaught,
                            PrimarySubjectTaught = s.StaffMaster.PrimarySubjectTaught,
                            OtherSubjectTaught = s.StaffMaster.OtherSubjectTaught
                        }
                    }).ToList()
                }).ToList();

                if (staffSchedule!=null && staffSchedule.Any())
                {
                    staffListView.CourseSectionsList = staffSchedule;
                    staffListView._failure = false;
                }
                else
                {
                    staffListView._failure = true;
                    staffListView._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                staffListView.CourseSectionsList = null!;
                staffListView._failure = true;
                staffListView._message = es.Message;
            }
            return staffListView;
        }

        /// <summary>
        /// Search Course For Schedule
        /// </summary>
        /// <param name="searchCourseScheduleViewModel"></param>
        /// <returns></returns>
        public SearchCourseScheduleViewModel SearchCourseForSchedule(SearchCourseScheduleViewModel searchCourseScheduleViewModel)
        {
            SearchCourseScheduleViewModel searchCourseSchedule = new SearchCourseScheduleViewModel();
            try
            {
                searchCourseSchedule.TenantId = searchCourseScheduleViewModel.TenantId;
                searchCourseSchedule.SchoolId = searchCourseScheduleViewModel.SchoolId;
                searchCourseSchedule._token = searchCourseScheduleViewModel._token;
                searchCourseSchedule._tenantName = searchCourseScheduleViewModel._tenantName;

                var courseData = this.context?.Course.Where(x => x.TenantId == searchCourseScheduleViewModel.TenantId && x.SchoolId == searchCourseScheduleViewModel.SchoolId && x.AcademicYear == searchCourseScheduleViewModel.AcademicYear && (searchCourseScheduleViewModel.CourseSubject == null || x.CourseSubject == searchCourseScheduleViewModel.CourseSubject) && (x.CourseProgram == searchCourseScheduleViewModel.CourseProgram || searchCourseScheduleViewModel.CourseProgram == null));

                if (courseData != null)
                {
                    var resultData = courseData.Select(s => new Course
                    {
                        TenantId = s.TenantId,
                        SchoolId = s.SchoolId,
                        CourseId = s.CourseId,
                        CourseProgram = s.CourseProgram,
                        CourseSubject = s.CourseSubject,
                        CourseGradeLevel = s.CourseGradeLevel,
                        CourseTitle = s.CourseTitle,
                        CourseCategory = s.CourseCategory,
                        CreditHours = s.CreditHours
                    }).ToList();

                    searchCourseSchedule.Course = resultData;
                    searchCourseSchedule._failure = false;
                }
                else
                {
                    searchCourseSchedule._message = NORECORDFOUND;
                    searchCourseSchedule._failure = true;
                }
            }
            catch (Exception ex)
            {
                searchCourseSchedule.Course = null;
                searchCourseSchedule._failure = true;
                searchCourseSchedule._message = ex.Message;
            }
            return searchCourseSchedule;
        }

        /// <summary>
        /// Add/Edit BellSchedule
        /// </summary>
        /// <param name="programAddViewModel"></param>
        /// <returns></returns>
        public BellScheduleAddViewModel AddEditBellSchedule(BellScheduleAddViewModel bellScheduleAddViewModel)
        {
            try
            {
                var bellScheduleData = this.context?.BellSchedule.Where(x => x.TenantId == bellScheduleAddViewModel.BellSchedule!.TenantId && x.SchoolId == bellScheduleAddViewModel.BellSchedule.SchoolId && x.BellScheduleDate == bellScheduleAddViewModel.BellSchedule.BellScheduleDate).FirstOrDefault();

                if (bellScheduleData != null)
                {
                    bellScheduleAddViewModel.BellSchedule!.CreatedBy = bellScheduleData.CreatedBy;
                    bellScheduleAddViewModel.BellSchedule.CreatedOn = bellScheduleData.CreatedOn;
                    bellScheduleAddViewModel.BellSchedule.UpdatedOn = DateTime.UtcNow;
                    bellScheduleAddViewModel.BellSchedule.AcademicYear = bellScheduleData.AcademicYear;
                    this.context?.Entry(bellScheduleData).CurrentValues.SetValues(bellScheduleAddViewModel.BellSchedule);

                    bellScheduleAddViewModel!._message = "Bell Schedule Updated Successfully";
                }
                else
                {
                    bellScheduleAddViewModel.BellSchedule!.AcademicYear = Utility.GetCurrentAcademicYear(this.context!, bellScheduleAddViewModel.BellSchedule.TenantId, bellScheduleAddViewModel.BellSchedule.SchoolId) ?? 0;
                    bellScheduleAddViewModel.BellSchedule!.CreatedOn = DateTime.UtcNow;
                    this.context?.BellSchedule.Add(bellScheduleAddViewModel.BellSchedule);

                    bellScheduleAddViewModel._message = "Bell Schedule added successfully";
                }
                this.context?.SaveChanges();
                bellScheduleAddViewModel._failure = false;
            }
            catch (Exception es)
            {
                bellScheduleAddViewModel._message = es.Message;
                bellScheduleAddViewModel._failure = true;
            }
            return bellScheduleAddViewModel;
        }

        /// <summary>
        /// Get All Bell Schedule
        /// </summary>
        /// <param name="bellScheduleListViewModel"></param>
        /// <returns></returns>
        public BellScheduleListViewModel GetAllBellSchedule(BellScheduleListViewModel bellScheduleListViewModel)
        {
            BellScheduleListViewModel bellScheduleView = new BellScheduleListViewModel();
            try
            {
                bellScheduleView._tenantName = bellScheduleListViewModel._tenantName;
                bellScheduleView._token = bellScheduleListViewModel._token;
                bellScheduleView.TenantId = bellScheduleListViewModel.TenantId;
                bellScheduleView.SchoolId = bellScheduleListViewModel.SchoolId;
                bellScheduleView.AcademicYear = bellScheduleListViewModel.AcademicYear;

                var bellScheduleData = this.context?.BellSchedule.Where(x => x.TenantId == bellScheduleListViewModel.TenantId && x.SchoolId == bellScheduleListViewModel.SchoolId && x.AcademicYear == bellScheduleListViewModel.AcademicYear).ToList();

                if (bellScheduleData!=null && bellScheduleData.Any())
                {
                    bellScheduleView.BellScheduleList = bellScheduleData;
                    bellScheduleView._failure = false;
                }
                else
                {
                    bellScheduleView._failure = true;
                    bellScheduleView._message = NORECORDFOUND;
                }              
            }
            catch (Exception es)
            {
                bellScheduleView._message = es.Message;
                bellScheduleView._failure = true;             
            }
            return bellScheduleView;
        }

        /// <summary>
        /// Get Course Catelog
        /// </summary>
        /// <param name="courseCatelogViewModel"></param>
        /// <returns></returns>
        public CourseCatelogViewModel GetCourseCatelog(CourseCatelogViewModel courseCatelogViewModel)
        {
            CourseCatelogViewModel courseCatelog = new CourseCatelogViewModel();
            try
            {
                courseCatelog.TenantId = courseCatelogViewModel.TenantId;
                courseCatelog.SchoolId = courseCatelogViewModel.SchoolId;
                courseCatelog._token = courseCatelogViewModel._token;
                courseCatelog._tenantName = courseCatelogViewModel._tenantName;
                courseCatelog._userName = courseCatelogViewModel._userName;

                int? YrmarkingPeriodId = 0;
                int? SmtrmarkingPeriodId = 0;
                int? QtrmarkingPeriodId = 0;
                int? PrgrsprdmarkingPeriodId = 0;
                String? markingPeriodTitle = null;
                if (!string.IsNullOrEmpty(courseCatelogViewModel.MarkingPeriodId))
                {
                    var markingPeriod = courseCatelogViewModel.MarkingPeriodId.Split("_", StringSplitOptions.RemoveEmptyEntries);

                    if (markingPeriod.First() == "0")
                    {
                        YrmarkingPeriodId = Int32.Parse(markingPeriod.Last());
                        markingPeriodTitle = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == courseCatelogViewModel.TenantId && x.SchoolId == courseCatelogViewModel.SchoolId && x.MarkingPeriodId == YrmarkingPeriodId)?.Title;
                    }
                    if (markingPeriod.First() == "1")
                    {
                        SmtrmarkingPeriodId = Int32.Parse(markingPeriod.Last());
                        markingPeriodTitle = this.context?.Semesters.FirstOrDefault(x => x.TenantId == courseCatelogViewModel.TenantId && x.SchoolId == courseCatelogViewModel.SchoolId && x.MarkingPeriodId == SmtrmarkingPeriodId)?.Title;
                    }
                    if (markingPeriod.First() == "2")
                    {
                        QtrmarkingPeriodId = Int32.Parse(markingPeriod.Last());
                        markingPeriodTitle = this.context?.Quarters.FirstOrDefault(x => x.TenantId == courseCatelogViewModel.TenantId && x.SchoolId == courseCatelogViewModel.SchoolId && x.MarkingPeriodId == QtrmarkingPeriodId)?.Title;
                    }
                    if (markingPeriod.First() == "3")
                    {
                        PrgrsprdmarkingPeriodId = Int32.Parse(markingPeriod.Last());
                        markingPeriodTitle = this.context?.ProgressPeriods.FirstOrDefault(x => x.TenantId == courseCatelogViewModel.TenantId && x.SchoolId == courseCatelogViewModel.SchoolId && x.MarkingPeriodId == PrgrsprdmarkingPeriodId)?.Title;
                    }
                }

                var courseSearchData = this.context?.AllCourseSectionView.Where(x => x.TenantId == courseCatelogViewModel.TenantId && x.SchoolId == courseCatelogViewModel.SchoolId && x.AcademicYear == courseCatelogViewModel.AcademicYear && (courseCatelogViewModel.CourseId == null || x.CourseId == courseCatelogViewModel.CourseId) && (string.IsNullOrEmpty(courseCatelogViewModel.CourseSubject) || x.CourseSubject == courseCatelogViewModel.CourseSubject) && (string.IsNullOrEmpty(courseCatelogViewModel.GradeLevel) || x.CourseGradeLevel == courseCatelogViewModel.GradeLevel) && (string.IsNullOrEmpty(courseCatelogViewModel.MarkingPeriodId) || x.YrMarkingPeriodId == YrmarkingPeriodId || x.SmstrMarkingPeriodId == SmtrmarkingPeriodId || x.QtrMarkingPeriodId == QtrmarkingPeriodId || x.PrgrsprdMarkingPeriodId == PrgrsprdmarkingPeriodId) && (courseCatelogViewModel.RoomId == null || x.FixedRoomId == courseCatelogViewModel.RoomId || x.VarRoomId == courseCatelogViewModel.RoomId || x.CalRoomId == courseCatelogViewModel.RoomId || x.BlockRoomId == courseCatelogViewModel.RoomId) && (courseCatelogViewModel.PeriodId == null || x.FixedPeriodId == courseCatelogViewModel.PeriodId || x.VarPeriodId == courseCatelogViewModel.PeriodId || x.CalPeriodId == courseCatelogViewModel.PeriodId || x.BlockPeriodId == courseCatelogViewModel.PeriodId));

                if (courseSearchData != null && courseSearchData.Any())
                {
                    var schoolMasterData = this.context?.SchoolMaster.Include(x => x.SchoolCalendars).FirstOrDefault(x => x.TenantId == courseCatelogViewModel.TenantId && x.SchoolId == courseCatelogViewModel.SchoolId);
                    if (schoolMasterData != null)
                    {
                        var schoolCalendarsData = schoolMasterData.SchoolCalendars.FirstOrDefault(x => x.SessionCalendar == true && x.AcademicYear == courseCatelogViewModel.AcademicYear);

                        courseCatelog.SchoolName = schoolMasterData.SchoolName;
                        courseCatelog.StreetAddress1 = schoolMasterData.StreetAddress1;
                        courseCatelog.StreetAddress2 = schoolMasterData.StreetAddress2;
                        courseCatelog.City = schoolMasterData.City;
                        courseCatelog.District = schoolMasterData.District;
                        courseCatelog.State = schoolMasterData.State;
                        courseCatelog.Country = schoolMasterData.Country;
                        courseCatelog.Zip = schoolMasterData.Zip;

                        courseCatelog.SchoolYearStartDate = schoolCalendarsData != null ? schoolCalendarsData.StartDate!.Value.Year.ToString() : null;
                        courseCatelog.SchoolYearEndDate = schoolCalendarsData != null ? schoolCalendarsData.EndDate!.Value.Year.ToString() : null;

                        courseCatelog.MarkingPeriodTitle = markingPeriodTitle;
                        courseCatelog.GradeLevelTitle = (string.IsNullOrEmpty(courseCatelogViewModel.GradeLevel)) ? null : courseCatelogViewModel.GradeLevel;
                    }

                    var allCourseData = this.context?.Course.Include(x => x.CourseSection).Where(x => x.TenantId == courseCatelogViewModel.TenantId && x.SchoolId == courseCatelogViewModel.SchoolId && x.AcademicYear == courseCatelogViewModel.AcademicYear);

                    var courseIds = courseSearchData.Select(s => s.CourseId).Distinct().ToList();
                    var courseSectionIds = courseSearchData.Select(s => s.CourseSectionId).Distinct().ToList();

                    foreach (var courseId in courseIds)
                    {
                        CourseWithCourseSectionDetailsViewModel courseWithCourseSectionDetails = new();
                        var courseData = allCourseData != null ? allCourseData.Where(x => x.CourseId == courseId).FirstOrDefault() : null!;

                        if (courseData != null && courseData.CourseSection.Any())
                        {
                            foreach (var courseSection in courseData.CourseSection)
                            {
                                if (courseSectionIds.Contains(courseSection.CourseSectionId))
                                {
                                    GetCourseSectionForView getCourseSection = new();
                                    getCourseSection.CourseSection = courseSection;

                                    //this block for fetch CourseSection's MarkingPeriod Title
                                    if(courseSection.YrMarkingPeriodId!=null)
                                    {
                                        getCourseSection.MarkingPeriod = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == courseCatelogViewModel.TenantId && x.SchoolId == courseCatelogViewModel.SchoolId && x.MarkingPeriodId == courseSection.YrMarkingPeriodId)?.Title;
                                    }
                                    else if(courseSection.SmstrMarkingPeriodId != null)
                                    {
                                        getCourseSection.MarkingPeriod = this.context?.Semesters.FirstOrDefault(x => x.TenantId == courseCatelogViewModel.TenantId && x.SchoolId == courseCatelogViewModel.SchoolId && x.MarkingPeriodId == courseSection.SmstrMarkingPeriodId)?.Title;
                                    }
                                    else if (courseSection.QtrMarkingPeriodId != null)
                                    {
                                        getCourseSection.MarkingPeriod = this.context?.Quarters.FirstOrDefault(x => x.TenantId == courseCatelogViewModel.TenantId && x.SchoolId == courseCatelogViewModel.SchoolId && x.MarkingPeriodId == courseSection.QtrMarkingPeriodId)?.Title;
                                    }
                                    else if (courseSection.PrgrsprdMarkingPeriodId != null)
                                    {
                                        getCourseSection.MarkingPeriod = this.context?.ProgressPeriods.FirstOrDefault(x => x.TenantId == courseCatelogViewModel.TenantId && x.SchoolId == courseCatelogViewModel.SchoolId && x.MarkingPeriodId == courseSection.PrgrsprdMarkingPeriodId)?.Title;
                                    }
                                    else
                                    {
                                        getCourseSection.MarkingPeriod = "Custom Date Range";
                                    }

                                    getCourseSection.CourseSection.Course = new();
                                    getCourseSection.CourseSection.SchoolMaster = new();
                                    getCourseSection.CourseSection.SchoolCalendars = null;
                                    getCourseSection.CourseSection.SchoolYears = null;
                                    getCourseSection.CourseSection.Semesters = null;
                                    getCourseSection.CourseSection.Quarters = null;
                                    getCourseSection.CourseSection.ProgressPeriods = null;

                                    if (String.Compare(courseSection.ScheduleType, "Fixed Schedule (1)", true) == 0)
                                    {
                                        var courseFixedScheduleData = this.context?.CourseFixedSchedule.Include(c => c.BlockPeriod).Include(b => b.Rooms).FirstOrDefault(x => x.TenantId == courseCatelogViewModel.TenantId && x.SchoolId == courseCatelogViewModel.SchoolId && x.CourseSectionId == courseSection.CourseSectionId);
                                        if (courseFixedScheduleData != null)
                                        {
                                            courseFixedScheduleData.BlockPeriod!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                            courseFixedScheduleData.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                            courseFixedScheduleData.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                            courseFixedScheduleData.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                            courseFixedScheduleData.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>();
                                            courseFixedScheduleData.BlockPeriod.SchoolMaster = new();
                                            courseFixedScheduleData.Rooms!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                            courseFixedScheduleData.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                            courseFixedScheduleData.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                            courseFixedScheduleData.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                            getCourseSection.CourseFixedSchedule = courseFixedScheduleData;
                                        }
                                    }

                                    if (String.Compare(courseSection.ScheduleType, "Variable Schedule (2)", true) == 0)
                                    {
                                        var courseVariableScheduleData = this.context?.CourseVariableSchedule.Include(c => c.BlockPeriod).Include(b => b.Rooms).Where(x => x.TenantId == courseCatelogViewModel.TenantId && x.SchoolId == courseCatelogViewModel.SchoolId && x.CourseSectionId == courseSection.CourseSectionId).ToList();

                                        if (courseVariableScheduleData != null && courseVariableScheduleData.Any())
                                        {
                                            courseVariableScheduleData.ForEach(x =>
                                            { /*x.BlockPeriod!.CourseFixedSchedule = null!; x.BlockPeriod.CourseVariableSchedule = null!; x.BlockPeriod.CourseCalendarSchedule = null!; x.BlockPeriod.CourseBlockSchedule = null!; x.Rooms!.CourseFixedSchedule = null!; x.Rooms.CourseVariableSchedule = null!; x.Rooms.CourseCalendarSchedule = null!; x.Rooms.CourseBlockSchedule = null!;*/
                                                x.BlockPeriod!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                                x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                                x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                                x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                                x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>();
                                                x.BlockPeriod.SchoolMaster = new();
                                                x.Rooms!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                                x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                                x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                                x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                            });

                                            getCourseSection.CourseVariableSchedule = courseVariableScheduleData;
                                        }
                                    }

                                    if (String.Compare(courseSection.ScheduleType, "Calendar Schedule (3)", true) == 0)
                                    {
                                        var courseCalenderScheduleData = this.context?.CourseCalendarSchedule.Include(c => c.BlockPeriod).Include(b => b.Rooms).Where(x => x.TenantId == courseCatelogViewModel.TenantId && x.SchoolId == courseCatelogViewModel.SchoolId && x.CourseSectionId == courseSection.CourseSectionId).ToList();

                                        if (courseCalenderScheduleData != null && courseCalenderScheduleData.Any())
                                        {
                                            courseCalenderScheduleData.ForEach(x =>
                                            { /*x.BlockPeriod!.CourseFixedSchedule = null!; x.BlockPeriod.CourseVariableSchedule = null!; x.BlockPeriod.CourseCalendarSchedule = null!; x.BlockPeriod.CourseBlockSchedule = null!; x.Rooms!.CourseFixedSchedule = null!; x.Rooms.CourseVariableSchedule = null!; x.Rooms.CourseCalendarSchedule = null!; x.Rooms.CourseBlockSchedule = null!;*/
                                                x.BlockPeriod!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                                x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                                x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                                x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                                x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>();
                                                x.BlockPeriod.SchoolMaster = new();
                                                x.Rooms!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                                x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                                x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                                x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                            });

                                            getCourseSection.CourseCalendarSchedule = courseCalenderScheduleData;
                                        }
                                    }

                                    if (String.Compare(courseSection.ScheduleType, "Block Schedule (4)", true) == 0)
                                    {
                                        var courseBlockScheduleData = this.context?.CourseBlockSchedule.Include(c => c.BlockPeriod).Include(b => b.Rooms).Where(x => x.TenantId == courseCatelogViewModel.TenantId && x.SchoolId == courseCatelogViewModel.SchoolId && x.CourseSectionId == courseSection.CourseSectionId).ToList();

                                        if (courseBlockScheduleData != null && courseBlockScheduleData.Any())
                                        {
                                            courseBlockScheduleData.ForEach(x =>
                                            { /*x.BlockPeriod!.CourseFixedSchedule = null!; x.BlockPeriod.CourseVariableSchedule = null!; x.BlockPeriod.CourseCalendarSchedule = null!; x.BlockPeriod.CourseBlockSchedule = null!; x.Rooms!.CourseFixedSchedule = null!; x.Rooms.CourseVariableSchedule = null!; x.Rooms.CourseCalendarSchedule = null!; x.Rooms.CourseBlockSchedule = null!;*/
                                                x.BlockPeriod!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                                x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                                x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                                x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                                x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>();
                                                x.BlockPeriod.SchoolMaster = new();
                                                x.Rooms!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                                x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                                x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                                x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                            });

                                            getCourseSection.CourseBlockSchedule = courseBlockScheduleData;

                                            var bellScheduleList = new List<BellSchedule>();
                                            foreach (var block in courseBlockScheduleData)
                                            {
                                                var bellScheduleData = this.context?.BellSchedule.Where(c => c.SchoolId == courseCatelogViewModel.SchoolId && c.TenantId == courseCatelogViewModel.TenantId && c.BlockId == block.BlockId && c.BellScheduleDate >= courseSection.DurationStartDate && c.BellScheduleDate <= courseSection.DurationEndDate).ToList();
                                                if (bellScheduleData != null && bellScheduleData.Any())
                                                    bellScheduleList.AddRange(bellScheduleData);
                                            }
                                            getCourseSection.BellScheduleList = bellScheduleList;
                                        }
                                    }
                                    courseWithCourseSectionDetails.GetCourseSectionForView.Add(getCourseSection);
                                }
                            }
                        }
                        courseWithCourseSectionDetails.TenantId = courseData!.TenantId;
                        courseWithCourseSectionDetails.SchoolId = courseData.SchoolId;
                        courseWithCourseSectionDetails.CourseId = courseData.CourseId;
                        courseWithCourseSectionDetails.CourseTitle = courseData.CourseTitle;
                        courseWithCourseSectionDetails.CourseProgram = courseData.CourseProgram;
                        courseWithCourseSectionDetails.CourseSubject = courseData.CourseSubject;
                        courseWithCourseSectionDetails.CourseGradeLevel = courseData.CourseGradeLevel;
                        courseWithCourseSectionDetails.CourseShortName = courseData.CourseShortName;
                        courseWithCourseSectionDetails.CreditHours = courseData.CreditHours;
                        courseCatelog.CourseWithCourseSectionDetailsViewModels.Add(courseWithCourseSectionDetails);
                    }
                }
                else
                {
                    courseCatelog._message = NORECORDFOUND;
                    courseCatelog._failure = true;
                }
            }
            catch (Exception es)
            {
                courseCatelog._message = es.Message;
                courseCatelog._failure = true;
            }
            return courseCatelog;
        }

        /// <summary>
        /// Get subjrct,course,CourseSection By Staff
        /// </summary>
        /// <param name="subjectsViewModel"></param>
        /// <returns></returns>
        public SubjectsViewModel GetCourseSectionByStaff(SubjectsViewModel subjectsViewModel)
        {
            SubjectsViewModel subjectView = new SubjectsViewModel();
            subjectView.TenantId = subjectsViewModel.TenantId;
            subjectView.SchoolId = subjectsViewModel.SchoolId;
            subjectView.StaffId = subjectsViewModel.StaffId;
            subjectView.AcademicYear = subjectsViewModel.AcademicYear;
            subjectView._tenantName = subjectsViewModel._tenantName;
            subjectView._token = subjectsViewModel._token;

            try
            {
                var staffCoursesectionScheduleData = this.context?.StaffCoursesectionSchedule.
                                    Join(this.context.Course,
                                    scss => scss.CourseId, c => c.CourseId,
                                    (scss, c) => new { scss, c }).Where(x => x.scss.TenantId == subjectsViewModel.TenantId && x.c.TenantId == subjectsViewModel.TenantId && x.scss.SchoolId == subjectsViewModel.SchoolId && x.c.SchoolId == subjectsViewModel.SchoolId && x.scss.StaffId == subjectsViewModel.StaffId && x.c.AcademicYear == subjectsViewModel.AcademicYear).ToList();

                if (staffCoursesectionScheduleData?.Any()==true)
                {
                    var subjectWiseData = staffCoursesectionScheduleData.GroupBy(x => x.c.CourseSubject).ToList();
                    var subjectData = this.context?.Subject.Where(x => x.TenantId == subjectsViewModel.TenantId && x.SchoolId == subjectsViewModel.SchoolId && x.AcademicYear == subjectsViewModel.AcademicYear);

                    //List<SubjectViewModel> subjectList = new();

                    //this loop for subject
                    foreach (var sub in subjectWiseData)
                    {
                        SubjectViewModel subject = new();
                        subject.SubjectName = sub.Key;
                        subject.SubjectId = subjectData?.FirstOrDefault(x => (x.SubjectName ?? "").ToLower() == (sub.Key ?? "").ToLower())?.SubjectId;

                        var courseForSub = sub.Select(s => s.c).Where(s => s.CourseSubject == sub.Key).Distinct().ToList();

                        //this loop for course
                        foreach (var course in courseForSub)
                        {
                            CoursesViewModel courseView = new();
                            courseView.CourseName = course.CourseTitle;
                            courseView.CourseId = course.CourseId;

                            var courseSectionbyCourse = sub.Select(s => s.scss).Where(x => x.CourseId == course.CourseId).ToList();

                            //this loop for courseSection
                            foreach (var cs in courseSectionbyCourse)
                            {
                                CourseSectionsViewModel courseSectionView = new();
                                courseSectionView.CourseSectionName = cs.CourseSectionName;
                                courseSectionView.CourseSectionId = cs.CourseSectionId;
                                courseView.courseSectionsViewModels.Add(courseSectionView);
                            }
                            subject.coursesViewModels.Add(courseView);
                        }
                        subjectView.subjectViewModels.Add(subject);
                    }
                }
                else
                {
                    subjectView._failure = true;
                    subjectView._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                subjectView._failure = true;
                subjectView._message = es.Message;
            }
            return subjectView;
        }

        /// <summary>
        /// GetCourseSectionAssignmentByStaff
        /// </summary>
        /// <param name="courseSectionAssignmentViewModel"></param>
        /// <returns></returns>
        public CourseSectionAssignmentViewModel GetCourseSectionAssignmentByStaff(CourseSectionAssignmentViewModel courseSectionAssignmentViewModel)
        {
            CourseSectionAssignmentViewModel courseSectionAssignment = new CourseSectionAssignmentViewModel();
            courseSectionAssignment.TenantId = courseSectionAssignmentViewModel.TenantId;
            courseSectionAssignment.SchoolId = courseSectionAssignmentViewModel.SchoolId;
            courseSectionAssignment.StaffId = courseSectionAssignmentViewModel.StaffId;
            courseSectionAssignment.AcademicYear = courseSectionAssignmentViewModel.AcademicYear;
            courseSectionAssignment._tenantName = courseSectionAssignmentViewModel._tenantName;
            courseSectionAssignment._token = courseSectionAssignmentViewModel._token;

            try
            {
                var staffCoursesectionScheduleData = this.context?.StaffCoursesectionSchedule.
                                    Join(this.context.AssignmentType.Include(x => x.Assignment),
                                    scss => scss.CourseSectionId, a => a.CourseSectionId,
                                    (scss, a) => new { scss, a }).Where(x => x.scss.TenantId == courseSectionAssignmentViewModel.TenantId && x.a.TenantId == courseSectionAssignmentViewModel.TenantId && x.scss.SchoolId == courseSectionAssignmentViewModel.SchoolId && x.a.SchoolId == courseSectionAssignmentViewModel.SchoolId && x.scss.StaffId == courseSectionAssignmentViewModel.StaffId && x.a.AcademicYear == courseSectionAssignmentViewModel.AcademicYear && x.scss.IsDropped != true).ToList();

                if (staffCoursesectionScheduleData?.Any() == true)
                {
                    var courseSectionWiseData = staffCoursesectionScheduleData.GroupBy(x => x.a.CourseSectionId).ToList();

                    foreach (var courseSection in courseSectionWiseData)
                    {
                        CourseSectionDetails courseSectionDetails = new();

                        courseSectionDetails.CourseSectionId = courseSection.Key;
                        courseSectionDetails.CourseSectionName = courseSection.First().scss.CourseSectionName;

                        //select assignmentType for this course section & add view model
                        courseSectionDetails.assignmentTypes = courseSection.Select(s => s.a).ToList();
                        courseSectionAssignment.courseSectionList.Add(courseSectionDetails);
                    }
                }
                else
                {
                    courseSectionAssignment._failure = true;
                    courseSectionAssignment._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                courseSectionAssignment._failure = true;
                courseSectionAssignment._message = es.Message;
            }
            return courseSectionAssignment;
        }
    }
}

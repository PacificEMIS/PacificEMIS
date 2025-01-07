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

import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { AddCourseSectionComponent } from './add-course-section/add-course-section.component';
import icInfo from '@iconify/icons-ic/twotone-info';
import icCheckCircle from '@iconify/icons-ic/check-circle';
import icDeleteForever from '@iconify/icons-ic/twotone-delete-forever';
import { StudentScheduleService } from 'src/app/services/student-schedule.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CourseManagerService } from 'src/app/services/course-manager.service';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { SharedFunction } from '../../shared/shared-function';
import { PageRolesPermission } from 'src/app/common/page-roles-permissions.service';
import { LoaderService } from 'src/app/services/loader.service';
import { CommonService } from 'src/app/services/common.service';
import { GetAllCourseListModel, GetAllProgramModel, GetAllSubjectModel } from 'src/app/models/course-manager.model';
import { GetMarkingPeriodTitleListModel } from 'src/app/models/marking-period.model';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { GetUnassociatedStudentListByCourseSectionModel, ScheduledStudentDeleteModel } from 'src/app/models/student-schedule.model';
import { Permissions } from '../../../models/roll-based-access.model';
import { MatTableDataSource } from '@angular/material/table';
import { FormControl } from '@angular/forms';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { MatCheckbox } from '@angular/material/checkbox';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import { RemoveStaffCourseSectionSchedule, ScheduledStaffForCourseSection } from 'src/app/models/course-section.model';
import { CourseSectionService } from 'src/app/services/course-section.service';
import { TeacherScheduleService } from 'src/app/services/teacher-schedule.service';
import { MatSort, Sort } from '@angular/material/sort';

@Component({
  selector: 'vex-group-delete',
  templateUrl: './group-delete.component.html',
  styleUrls: ['./group-delete.component.scss']
})
export class GroupDeleteComponent implements OnInit, AfterViewInit, OnDestroy {

  icInfo = icInfo;
  icCheckCircle = icCheckCircle;
  icDeleteForever = icDeleteForever;
  showScheduledStudents: boolean = true;

  displayedColumns: string[] = ['studentSelected', 'name', 'studentId', 'alternateId', 'grade', 'phone'];
  displayedStaffColumns: string[] = ['staffSelected', 'firstGivenName', 'staffId', 'profile', 'jobTitle', 'schoolEmail', 'mobilePhone'];
  studentDetails: MatTableDataSource<any>;
  programList = [];
  subjectList = [];
  courseList = [];
  permissions: Permissions;
  markingPeriodList = [];
  courseSectionData;
  loading: boolean;
  destroySubject$: Subject<void> = new Subject();
  totalCount: number = 0;
  pageNumber: number;
  pageSize: number;
  searchCtrl: FormControl;
  isVisible: boolean = false;
  isStaffVisible: boolean = false;
  isGroupDelete: boolean = false;
  isGroupStaffDelete: boolean = false;
  showCourseSectionName: boolean = false;
  showErrorMessage: string = '';
  showStaffErrorMessage: string = '';
  listOfStudents = [];
  selectedStudents = [];
  listOfStaffs = [];
  selectedStaffs = [];
  staffCoursesectionSchedule: MatTableDataSource<any>;
  totalStaffCount: number = 0;
  getAllProgramModel: GetAllProgramModel = new GetAllProgramModel();
  getAllSubjectModel: GetAllSubjectModel = new GetAllSubjectModel();
  getAllCourseListModel: GetAllCourseListModel = new GetAllCourseListModel();
  getMarkingPeriodTitleListModel: GetMarkingPeriodTitleListModel = new GetMarkingPeriodTitleListModel();
  scheduleStudentListViewModel: GetUnassociatedStudentListByCourseSectionModel = new GetUnassociatedStudentListByCourseSectionModel();
  scheduledStudentDeleteModel: ScheduledStudentDeleteModel = new ScheduledStudentDeleteModel();
  scheduledTeacher:ScheduledStaffForCourseSection = new ScheduledStaffForCourseSection();
  removeStaffDetails : RemoveStaffCourseSectionSchedule = new RemoveStaffCourseSectionSchedule();
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild('masterCheckBox') masterCheckBox: MatCheckbox;
  @ViewChild('masterCheckBoxStaff') masterCheckBoxStaff: MatCheckbox;
  @ViewChild(MatSort) sort: MatSort;

  constructor(private dialog: MatDialog,
    public translateService: TranslateService,
    private studentScheduleService: StudentScheduleService,
    private snackbar: MatSnackBar,
    private courseManagerService: CourseManagerService,
    private markingPeriodService: MarkingPeriodService,
    public defaultService: DefaultValuesService,
    private commonFunction: SharedFunction,
    private pageRolePermissions: PageRolesPermission,
    private loaderService: LoaderService,
    private paginatorObj: MatPaginatorIntl,
    private commonService: CommonService,
    private courseSectionService:CourseSectionService,
    private teacherScheduleService:TeacherScheduleService,
  ) {
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission();
    this.searchCtrl = new FormControl();
    this.getAllCourse();
    this.getAllSubjectList();
    this.getAllProgramList();
    this.getAllMarkingPeriodList();
  }

  ngAfterViewInit(): void {
    this.searchInStudentList();
  }

  announceSortChange(sortState: Sort) {
      this.staffCoursesectionSchedule.sort = this.sort;
  }

  // For searching
  searchInStudentList() {
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term.trim().length > 0) {
        let filterParams = [
          {
            columnName: null,
            filterValue: term,
            filterOption: 3
          }
        ];
        Object.assign(this.scheduleStudentListViewModel, { filterParams: filterParams });
        this.scheduleStudentListViewModel.pageNumber = 1;
        this.paginator.pageIndex = 0;
        this.scheduleStudentListViewModel.pageSize = this.pageSize;
        this.getUnassociatedStudentListByCourseSection(this.courseSectionData.courseSectionId);
      }
      else {
        Object.assign(this.scheduleStudentListViewModel, { filterParams: null });
        this.scheduleStudentListViewModel.pageNumber = this.paginator.pageIndex + 1;
        this.scheduleStudentListViewModel.pageSize = this.pageSize;
        this.getUnassociatedStudentListByCourseSection(this.courseSectionData.courseSectionId);
      }
    });
  }

  // For open select Course Section dialog
  selectCourseSection() {
    if (this.defaultService.checkAcademicYear() && (this?.subjectList?.length > 0 && this.courseList?.length > 0 )) {
      this.studentDetails = new MatTableDataSource([]);
      this.scheduleStudentListViewModel = new GetUnassociatedStudentListByCourseSectionModel();
      this.searchCtrl = new FormControl();
      this.searchInStudentList();
      this.listOfStudents = [];
      this.selectedStudents = [];
      this.listOfStaffs = [];
      this.selectedStaffs = [];
      this.isVisible = false;
      this.isStaffVisible = false;
      this.isGroupDelete = false;
      this.isGroupStaffDelete = false;
      this.showCourseSectionName = false;
      this.showErrorMessage = '';
      this.showStaffErrorMessage = '';
      this.dialog.open(AddCourseSectionComponent, {
        width: '900px',
        data: {
          markingPeriods: this.getMarkingPeriodTitleListModel.getMarkingPeriodView,
          courseList: this.courseList,
          subjectList: this.subjectList,
          programList: this.programList
        }
      }).afterClosed().subscribe(res => {
        this.courseSectionData = res;
        if (this.courseSectionData) {
          this.showCourseSectionName = true;
          this.getUnassociatedStudentListByCourseSection(this.courseSectionData.courseSectionId);
          this.getScheduledTeachers();
        } else {
          this.showCourseSectionName = false;
        }
      });
    }
  }

  /********** Calling Dropdown API's Start **********/
  getAllProgramList() {
    this.courseManagerService.GetAllProgramsList(this.getAllProgramModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.programList = [];
          if (!data.programList) {
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
        } else {
          this.programList = data.programList;
        }
      } else {
        this.snackbar.open(this.defaultService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  getAllSubjectList() {
    this.courseManagerService.GetAllSubjectList(this.getAllSubjectModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.subjectList = [];
          if (!data.subjectList) {
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
        } else {
          this.subjectList = data.subjectList;
        }
      } else {
        this.snackbar.open(this.defaultService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  getAllMarkingPeriodList() {
    this.getMarkingPeriodTitleListModel.schoolId = this.defaultService.getSchoolID();
    this.getMarkingPeriodTitleListModel.academicYear = this.defaultService.getAcademicYear();
    this.markingPeriodService.getAllMarkingPeriodList(this.getMarkingPeriodTitleListModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.getMarkingPeriodTitleListModel.getMarkingPeriodView = [];
          if (!this.getMarkingPeriodTitleListModel?.getMarkingPeriodView) {
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
        } else {
          this.getMarkingPeriodTitleListModel.getMarkingPeriodView = data.getMarkingPeriodView;
        }
      } else {
        this.snackbar.open(this.defaultService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  getAllCourse() {
    this.courseManagerService.GetAllCourseList(this.getAllCourseListModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.courseList = [];
          if (!data.courseViewModelList) {
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
        } else {
          this.courseList = data.courseViewModelList;
        }
      } else {
        this.snackbar.open(this.defaultService.getHttpError(), '', {
          duration: 10000
        });
      }
    })
  }

  getScheduledTeachers() {
    this.scheduledTeacher.courseId = this.courseSectionData.courseId;
    this.scheduledTeacher.courseSectionId = this.courseSectionData.courseSectionId;
    this.courseSectionService.getUnassociatedStaffListByCourseSection(this.scheduledTeacher).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.snackbar.open('Teacher schedule failed ' + this.defaultService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
        if (res._failure) {
          this.staffCoursesectionSchedule = new MatTableDataSource([]);
          this.totalStaffCount = 0;
          this.isStaffVisible = false;
          this.showStaffErrorMessage = res._message;
        } else {
          this.isStaffVisible = true;
          this.scheduledTeacher.courseSectionsList = res.courseSectionsList;
          this.totalStaffCount = res.courseSectionsList[0].staffCoursesectionSchedule?.length;
          if(this.totalStaffCount > 0){
            res.courseSectionsList[0].staffCoursesectionSchedule.forEach((staff) => {
              staff.checked = false;
            });
            this.listOfStaffs = res.courseSectionsList[0].staffCoursesectionSchedule.map((item) => {
              this.selectedStaffs.map((selectedUser) => {
                if (item.staffId === selectedUser.staffId) {
                  item.checked = true;
                  return item;
                }
              });
              return item;
            });
            this.masterCheckBox.checked = this.listOfStaffs.every((item) => {
              return item.checked;
            });
            res.courseSectionsList[0]?.staffCoursesectionSchedule.map( (item: any) => {
              item.firstGivenName = item?.staffMaster?.firstGivenName;
              item.lastFamilyName = item?.staffMaster?.lastFamilyName;
              item.staffInternalId = item?.staffMaster?.staffInternalId;
              item.profile = item?.staffMaster?.profile;
              item.jobTitle = item?.staffMaster?.jobTitle;
              item.schoolEmail = item?.staffMaster?.schoolEmail;
              item.mobilePhone = item?.staffMaster?.mobilePhone;
            })
            this.staffCoursesectionSchedule = new MatTableDataSource(res.courseSectionsList[0]?.staffCoursesectionSchedule);
            this.staffCoursesectionSchedule.sort = this.sort;
            this.showStaffErrorMessage = '';
          }
          else{
            this.isStaffVisible = false;
            this.showStaffErrorMessage = this.defaultService.translateKey('noStaffFound');
          }
        }
      }
    })
  }
  /********** Calling Dropdown API's End **********/

  // For call unassociated student list API
  getUnassociatedStudentListByCourseSection(courseSectionId) {
    this.scheduleStudentListViewModel.sortingModel = null;
    this.scheduleStudentListViewModel.courseSectionId = courseSectionId;
    this.studentScheduleService.getUnassociatedStudentListByCourseSection(this.scheduleStudentListViewModel).subscribe((res) => {
      if (res) {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          //if(!this.searchCtrl.value)
          this.showErrorMessage = this.defaultService.translateKey(res._message);
          this.isVisible = false;
          this.studentDetails = new MatTableDataSource([]);
          this.totalCount = null;
        } else {
          this.showErrorMessage = '';
          this.isVisible = true;
          this.totalCount = res.totalCount;
          this.pageNumber = res.pageNumber;
          this.pageSize = res._pageSize;
          res.scheduleStudentForView.forEach((student) => {
            student.checked = false;
          });
          this.listOfStudents = res.scheduleStudentForView.map((item) => {
            this.selectedStudents.map((selectedUser) => {
              if (item.studentId === selectedUser.studentId) {
                item.checked = true;
                return item;
              }
            });
            return item;
          });
          this.masterCheckBox.checked = this.listOfStudents.every((item) => {
            return item.checked;
          });
          this.studentDetails = new MatTableDataSource(res.scheduleStudentForView);
        }
      } else {
        this.snackbar.open(this.defaultService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  // For server side pagination
  getPageEvent(event) {
    if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
      let filterParams = [
        {
          columnName: null,
          filterValue: this.searchCtrl.value,
          filterOption: 3
        }
      ]
      Object.assign(this.scheduleStudentListViewModel, { filterParams: filterParams });
    }
    this.scheduleStudentListViewModel.pageNumber = event.pageIndex + 1;
    this.scheduleStudentListViewModel.pageSize = event.pageSize;
    this.getUnassociatedStudentListByCourseSection(this.courseSectionData.courseSectionId);
  }

  someComplete(): boolean {
    let indetermine = false;
    for (let user of this.listOfStudents) {
      for (let selectedUser of this.selectedStudents) {
        if (user.studentId === selectedUser.studentId) {
          indetermine = true;
        }
      }
    }
    if (indetermine) {
      this.masterCheckBox.checked = this.listOfStudents.every((item) => {
        return item.checked;
      })
      if (this.masterCheckBox.checked) {
        return false;
      } else {
        return true;
      }
    }
  }

  setAll(event) {
    this.listOfStudents.forEach(user => { user.checked = event; });
    this.studentDetails = new MatTableDataSource(this.listOfStudents);
    this.decideCheckUncheck();
  }

  onChangeSelection(eventStatus: boolean, studentId) {
    for (let item of this.listOfStudents) {
      if (item.studentId === studentId) {
        item.checked = eventStatus;
        break;
      }
    }
    this.studentDetails = new MatTableDataSource(this.listOfStudents);
    this.masterCheckBox.checked = this.listOfStudents.every((item) => {
      return item.checked;
    });

    this.decideCheckUncheck();
  }

  decideCheckUncheck() {
    this.listOfStudents.map((item) => {
      let isIdIncludesInSelectedList = false;
      if (item.checked) {
        for (let selectedUser of this.selectedStudents) {
          if (item.studentId === selectedUser.studentId) {
            isIdIncludesInSelectedList = true;
            break;
          }
        }
        if (!isIdIncludesInSelectedList) {
          this.selectedStudents.push(item);
        }
      }
      else {
        for (let selectedUser of this.selectedStudents) {
          if (item.studentId === selectedUser.studentId) {
            this.selectedStudents = this.selectedStudents.filter((user) => user.studentId !== item.studentId);
            break;
          }
        }
      }
      isIdIncludesInSelectedList = false;
    });
    this.selectedStudents = this.selectedStudents.filter((item) => item.checked);
  }

  // For open deleteGroupStudents confirmation dialog.
  confirmDeleteGroupStudents() {
    let selectedStudentsStaffsLength = this.selectedStudents.length + this.selectedStaffs.length;
    if (selectedStudentsStaffsLength === 0) {
      this.snackbar.open('Please Select a Student and/or Teacher', '', {
        duration: 5000
      });
      return;
    }
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
        title: this.defaultService.translateKey('areYouSure'),
        message: selectedStudentsStaffsLength > 1 ? this.defaultService.translateKey('areYouSureYouWantToDeleteTheSelectedStudentsFromTheCourseSection?') : this.defaultService.translateKey('areYouSureYouWantToDeleteTheSelectedStudentFromTheCourseSection?')
      }
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult) {
        this.deleteGroupStudents();
      }
    });
  }

  // For call groupDeleteForScheduledStudent API.
  deleteGroupStudents() {
    this.scheduledStudentDeleteModel.courseSectionId = this.courseSectionData.courseSectionId;
    this.scheduledStudentDeleteModel.studentIds = this.selectedStudents.map(item => {
      return item.studentId;
    });
    this.scheduledStudentDeleteModel.staffIds = this.selectedStaffs.map(item => {
      return item.staffId;
    });
    this.studentScheduleService.groupDeleteForScheduledStudent(this.scheduledStudentDeleteModel).subscribe((res) => {
      if (res) {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        } else {
          this.showErrorMessage = '';
          this.isVisible = false;
          this.showStaffErrorMessage = '';
          this.isStaffVisible = false;
          this.isGroupDelete = true;
          this.showCourseSectionName = false;
          // this.checkShowCourseSectionName();
        }
      } else {
        this.snackbar.open(this.defaultService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  confirmDeleteScheduledStaff () {
    if (!this.selectedStaffs.length) {
      this.snackbar.open(this.defaultService.translateKey('selectStaffOrGroupStaff'), '', {
        duration: 5000
      });
      return;
    }
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
        title: this.selectedStaffs.length > 1 ? this.defaultService.translateKey('areYouSureWantToDeleteScheduledTeachers') : this.defaultService.translateKey('areYouSureWantToDeleteScheduledTeacher'),
        message: ''
      }
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult) {
        this.removeStaffDetails.staffIds = this.selectedStaffs.map(item => {
          return item.staffId;
        });
        this.removeStaffDetails.courseSectionId = this.courseSectionData.courseSectionId;
        this.teacherScheduleService.removeStaffCourseSectionSchedule(this.removeStaffDetails).subscribe(res => {
          if (res._failure) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
          else {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
            this.isGroupStaffDelete = true;
            this.isStaffVisible = false;
            this.showStaffErrorMessage = '';
            this.checkShowCourseSectionName();
            this.getScheduledTeachers();
          }
        })
      }
    });
  }

  someStaffComplete(): boolean {
    let indetermine = false;
    for (let user of this.listOfStaffs) {
      for (let selectedUser of this.selectedStaffs) {
        if (user.staffId === selectedUser.staffId) {
          indetermine = true;
        }
      }
    }
    if (indetermine) {
      this.masterCheckBoxStaff.checked = this.listOfStaffs.every((item) => {
        return item.checked;
      })
      if (this.masterCheckBoxStaff.checked) {
        return false;
      } else {
        return true;
      }
    }
  }

  setStaffAll(event) {
    this.listOfStaffs.forEach(user => { user.checked = event; });
    this.staffCoursesectionSchedule = new MatTableDataSource(this.listOfStaffs);
    this.decideStaffCheckUncheck();
  }

  onChangeStaffSelection(eventStatus: boolean, staffId) {
    for (let item of this.listOfStaffs) {
      if (item.staffId === staffId) {
        item.checked = eventStatus;
        break;
      }
    }
    this.staffCoursesectionSchedule = new MatTableDataSource(this.listOfStaffs);
    this.masterCheckBoxStaff.checked = this.listOfStaffs.every((item) => {
      return item.checked;
    });

    this.decideStaffCheckUncheck();
  }

  decideStaffCheckUncheck() {
    this.listOfStaffs.map((item) => {
      let isIdIncludesInSelectedList = false;
      if (item.checked) {
        for (let selectedUser of this.selectedStaffs) {
          if (item.staffId === selectedUser.staffId) {
            isIdIncludesInSelectedList = true;
            break;
          }
        }
        if (!isIdIncludesInSelectedList) {
          this.selectedStaffs.push(item);
        }
      }
      else {
        for (let selectedUser of this.selectedStaffs) {
          if (item.staffId === selectedUser.staffId) {
            this.selectedStaffs = this.selectedStaffs.filter((user) => user.staffId !== item.staffId);
            break;
          }
        }
      }
      isIdIncludesInSelectedList = false;
    });
    this.selectedStaffs = this.selectedStaffs.filter((item) => item.checked);
  }

  checkShowCourseSectionName() {
    if (this.totalStaffCount === 0) {
      if (this.isGroupDelete)
        this.showCourseSectionName = false;
    }
    else {
      if (this.isGroupStaffDelete && this.isGroupDelete) {
        this.showCourseSectionName = false;
      }
    }
  }

  // For destroy the isLoading subject.
  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}

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

import { Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import { fadeInRight400ms } from '../../../../../@vex/animations/fade-in-right.animation';
import icAdd from '@iconify/icons-ic/baseline-add';
import icClear from '@iconify/icons-ic/baseline-clear';
import { SchoolCreate } from '../../../../enums/school-create.enum';
import { RollingOptionsEnum } from '../../../../enums/rolling-retention-option.enum';
import { CalendarListModel, GetCalendarAndHolidayListModel } from '../../../../models/calendar.model';
import { CalendarService } from '../../../../services/calendar.service';
import { StudentEnrollmentDetails, StudentEnrollmentModel, StudentEnrollmentSchoolListModel } from '../../../../models/student.model';
import { StudentService } from '../../../../services/student.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import icEdit from '@iconify/icons-ic/edit';
import icCheckbox from '@iconify/icons-ic/baseline-check-box';
import icCheckboxOutline from '@iconify/icons-ic/baseline-check-box-outline-blank';
import icPromoted from '@iconify/icons-ic/baseline-north';
import icExternal from '@iconify/icons-ic/baseline-undo';
import icRetained from '@iconify/icons-ic/baseline-replay';
import icHomeSchool from '@iconify/icons-ic/baseline-home';
import icExpand from '@iconify/icons-ic/baseline-arrow-drop-up';
import icCollapse from '@iconify/icons-ic/baseline-arrow-drop-down';
import icTrasnferIn from '@iconify/icons-ic/baseline-call-received';
import icTrasnferOut from '@iconify/icons-ic/baseline-call-made';
import icDrop from '@iconify/icons-ic/vertical-align-bottom';
import { EnrollmentCodeListView } from '../../../../models/enrollment-code.model';
import { Router } from '@angular/router';
import { SharedFunction } from '../../../shared/shared-function';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../../models/roll-based-access.model';
import { CryptoService } from '../../../../services/Crypto.service';
import { GetAllSectionModel } from '../../../../models/section.model';
import { SectionService } from '../../../../services/section.service';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { DefaultValuesService } from '../../../../common/default-values.service';
import { PageRolesPermission } from '../../../../common/page-roles-permissions.service';
import { CommonService } from 'src/app/services/common.service';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from 'src/app/pages/shared-module/confirm-dialog/confirm-dialog.component';
import { Module } from 'src/app/enums/module.enum';
@Component({
  selector: 'vex-student-enrollmentinfo',
  templateUrl: './student-enrollmentinfo.component.html',
  styleUrls: ['./student-enrollmentinfo.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms,
    fadeInRight400ms
  ],
})
export class StudentEnrollmentinfoComponent implements OnInit, OnDestroy {
  icAdd = icAdd;
  icClear = icClear;
  icEdit = icEdit;
  icCheckbox = icCheckbox;
  icCheckboxOutline = icCheckboxOutline;
  icPromoted = icPromoted;
  icExternal = icExternal;
  icHomeSchool = icHomeSchool;
  icExpand = icExpand;
  icCollapse = icCollapse;
  icRetained = icRetained;
  icTrasnferIn = icTrasnferIn;
  icTrasnferOut = icTrasnferOut;
  icDrop = icDrop;
  membershipType;
  studentCreate = SchoolCreate;
  studentCreateMode: SchoolCreate;
  studentDetailsForViewAndEdit;
  rollingOptions = Object.keys(RollingOptionsEnum);
  exitCodes = [];
  calendarListModel: CalendarListModel = new CalendarListModel();
  studentEnrollmentModel: StudentEnrollmentModel = new StudentEnrollmentModel();
  studentEnrollmentViewModel: StudentEnrollmentModel = new StudentEnrollmentModel();
  schoolListWithGrades: StudentEnrollmentSchoolListModel = new StudentEnrollmentSchoolListModel();
  enrollmentCodeListView: EnrollmentCodeListView = new EnrollmentCodeListView();
  @ViewChild('form') currentForm: NgForm;
  minExitDate = new Date();
  divCount = [1];
  studentName: string;
  schoolListWithGradeLevelsAndEnrollCodes = [];
  selectedSchoolIndex = [];
  selectedTransferredSchoolIndex = [];
  calendarNameInViewMode = '-';
  filteredEnrollmentInfoInViewMode;
  expandEnrollmentHistory = true;
  selectedExitCodes = [];
  selectedEnrollmentCodes = [];
  disableEditDueToActiveExitCode = false;
  cloneEnrollmentForCancel;
  cloneOfCloneEnrollmentForCancel;
  cloneStudentModel;
  studentCloneModel;
  cloneStudentEnrollment: StudentEnrollmentModel = new StudentEnrollmentModel();

  sectionList: GetAllSectionModel = new GetAllSectionModel();
  destroySubject$: Subject<void> = new Subject();
  sectionNameInViewMode = '-';
  studentActiveStatus = true;
  permissions: Permissions;
  module = Module.STUDENT;
  categoryId = 1;
  customValid=false;
  getAllCalendarHoliday: GetCalendarAndHolidayListModel = new GetCalendarAndHolidayListModel();
  constructor(
    private dialog: MatDialog,
    private calendarService: CalendarService,
    private studentService: StudentService,
    private snackbar: MatSnackBar,
    private cryptoService: CryptoService,
    private router: Router,
    private defaultValuesService: DefaultValuesService,
    private pageRolePermissions: PageRolesPermission,
    private commonFunction: SharedFunction,
    private sectionService: SectionService,
    public defaultValueService: DefaultValuesService,
    private commonService: CommonService,
  ) {
    this.defaultValuesService.checkAcademicYear() && !this.studentService.getStudentId() ? this.studentService.redirectToGeneralInfo() : !this.defaultValuesService.checkAcademicYear() && !this.studentService.getStudentId() ? this.studentService.redirectToStudentList() : '';
  }

  ngOnInit(): void {
    this.studentService.studentCreatedMode.subscribe((res) => {
      this.studentCreateMode = res;

    });
    this.studentService.studentDetailsForViewedAndEdited.subscribe((res) => {
      this.studentDetailsForViewAndEdit = res;
      this.studentName = this.studentDetailsForViewAndEdit?.studentMaster?.firstGivenName + ' ' + this.studentDetailsForViewAndEdit?.studentMaster?.lastFamilyName
      if (res?.fieldsCategoryList) {
        if (this.studentCreateMode === this.studentCreate.VIEW) {
          // this.getAllSchoolListWithGradeLevels();
          this.getAllStudentEnrollments();
          this.studentService.changePageMode(this.studentCreateMode);
        }
      }
    });

    this.permissions = this.pageRolePermissions.checkPageRolePermission();

    if (this.studentCreateMode === this.studentCreate.EDIT) {
      this.studentCreateMode = this.studentCreate.ADD;
    }
    if (this.studentCreateMode === this.studentCreate.ADD) {
      // this.getAllCalendar();
      this.getAllStudentEnrollments();
      // this.studentService.changePageMode(this.studentCreateMode);
    }
    this.membershipType = this.defaultValueService.getUserMembershipType();
  }

  cmpare(index) {
    return index;
  }

  onSchoolChange(schoolId, indexOfDynamicRow) {
    const index = this.schoolListWithGradeLevelsAndEnrollCodes.findIndex((x) => {
      return x.schoolId === +schoolId;
    });
    this.selectedSchoolIndex[indexOfDynamicRow] = index;

    this.cloneStudentEnrollment.studentEnrollments[indexOfDynamicRow].schoolName = this.schoolListWithGradeLevelsAndEnrollCodes[index].schoolName;
    this.getCalendarAndHolidayList(schoolId);
  }

  getCalendarAndHolidayList(schoolId: any) {
    this.getAllCalendarHoliday._tenantName = this.defaultValuesService.getDefaultTenant();
    this.getAllCalendarHoliday._token = this.defaultValuesService.getToken();
    this.getAllCalendarHoliday._userName = this.defaultValuesService.getUserName();
    this.getAllCalendarHoliday.schoolCalendar.academicYear = this.defaultValuesService.getAcademicYear();
    this.getAllCalendarHoliday.schoolCalendar.schoolId = schoolId;
    this.calendarService.GetCalendarAndHolidayList(this.getAllCalendarHoliday).subscribe((res: any) => {
      if (res._failure) {
      }
      else {
        this.schoolListWithGradeLevelsAndEnrollCodes.forEach(item => {
          if (item.schoolId == schoolId) {
            item.exitStartDate = new Date(res?.schoolCalendar?.startDate);
            item.exitEndDate = new Date(res?.schoolCalendar?.endDate);
          }
        });
      }
    })
  }


  onTransferredSchoolChange(transferredSchoolId, indexOfDynamicRow) {
    const index = this.schoolListWithGradeLevelsAndEnrollCodes.findIndex((x) => {
      return x.schoolId === +transferredSchoolId;
    });
    this.selectedTransferredSchoolIndex[indexOfDynamicRow] = index;
    this.cloneStudentEnrollment.studentEnrollments[indexOfDynamicRow].schoolTransferred = this.schoolListWithGradeLevelsAndEnrollCodes[index].schoolName;

  }

  onEnrollmentCodeChange(enrollmentCode, indexOfDynamicRow) {

  }

  onExitCodeChange(value, indexOfDynamicRow) {
    this.schoolListWithGradeLevelsAndEnrollCodes[this.selectedSchoolIndex[indexOfDynamicRow]].studentEnrollmentCode?.map((item) => {
      if (item.enrollmentCode === +value) {
        this.selectedExitCodes[indexOfDynamicRow] = item.type;
      }
    })

  }

  addMoreEnrollments() {
    this.cloneStudentEnrollment.studentEnrollments.push({
      tenantId: this.defaultValueService.getTenantID(),
      schoolId: null,
      studentId: null,
      academicYear: null,
      enrollmentId: 0,
      calenderId: null,
      rollingOption: null,
      schoolName: null,
      gradeId: null,
      gradeLevelTitle: null,
      enrollmentDate: null,
      rolloverId: null,
      enrollmentCode: null,
      exitDate: null,
      exitCode: null,
      exitType: null,
      type: null,
      transferredSchoolId: null,
      schoolTransferred: null,
      transferredGrade: null,
      enrollmentType: null,
      createdBy: null,
      createdOn: null,
      updatedOn: null,
      updatedBy: this.defaultValueService.getUserGuidId(),
      studentGuid: null,
      startYear: null,
      endYear: null,
      isActive: null,
      showDrop: null

    })
    this.divCount.push(2); // Why 2? We have to fill up the divCount, It could be anything.
  }

  deleteDynamicRow(indexOfDynamicRow) {
    this.divCount.splice(indexOfDynamicRow, 1);
    this.cloneStudentEnrollment.studentEnrollments.splice(indexOfDynamicRow, 1);
    this.selectedSchoolIndex.splice(indexOfDynamicRow, 1);
    this.selectedTransferredSchoolIndex.splice(indexOfDynamicRow, 1);
  }

  getAllCalendar() {
    this.calendarService.getAllCalendar(this.calendarListModel).subscribe((res) => {
      const allCalendarsInCurrentSchool = res.calendarList;
      this.calendarListModel.calendarList = allCalendarsInCurrentSchool.filter((x) => {
        return (x.academicYear === +this.defaultValueService.getAcademicYear() && x.defaultCalender);
      });
      this.manipulateModelInEditMode();
    });

  }

  getAllSchoolListWithGradeLevelsAndEnrollCodes() {
    this.studentService.studentEnrollmentSchoolList(this.schoolListWithGrades).subscribe(res => {
      if (res) {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
        } else {
          this.schoolListWithGradeLevelsAndEnrollCodes = res.schoolMaster.filter(item => item);
          this.schoolListWithGradeLevelsAndEnrollCodes.forEach(item => {
            if (item.schoolId == this.defaultValuesService.getSchoolID()) {
              item.exitStartDate = new Date(this.defaultValuesService.getFullYearStartDate());
              item.exitEndDate = new Date(this.defaultValuesService.getFullYearEndDate());
            }
          });
          for (let i = 0; i < this.cloneStudentEnrollment.studentEnrollments.length; i++) {
            this.selectedSchoolIndex[i] = this.schoolListWithGradeLevelsAndEnrollCodes.findIndex((x) => {
              return x.schoolId === +this.cloneStudentEnrollment.studentEnrollments[i].schoolId;
            });
            this.selectedTransferredSchoolIndex[i] = this.schoolListWithGradeLevelsAndEnrollCodes.findIndex((x) => {
              return x.schoolId === +this.cloneStudentEnrollment.studentEnrollments[i].transferredSchoolId;
            });
          }
          this.findEnrollmentCodeIdByName();
          this.findExitCodeIdByName();
        }
      }
    });
  }

  editEnrollmentInfo() {
    this.divCount.length = this.studentCloneModel?.studentEnrollments?.length;
    if (this.studentCloneModel.studentEnrollments !== null) {
      for (let i = 0; i < this.studentCloneModel.studentEnrollments?.length; i++) {
        this.divCount[i] = 2;
      }
    } else {
      this.cloneStudentEnrollment = new StudentEnrollmentModel();
    }
    this.studentCreateMode = this.studentCreate.EDIT;
    this.getAllSchoolListWithGradeLevelsAndEnrollCodes();
    this.studentService.changePageMode(this.studentCreateMode);
  }

  cancelEdit() {
    // if(this.studentEnrollmentModel!==this.cloneEnrollmentForCancel){
    //   this.studentEnrollmentModel=JSON.parse(this.cloneEnrollmentForCancel);
    // }
    // if(this.cloneStudentEnrollment!==this.cloneOfCloneEnrollmentForCancel){
    //   this.cloneStudentEnrollment=JSON.parse(this.cloneOfCloneEnrollmentForCancel);
    // }
    // debugger;
    // this.cloneStudentEnrollment.studentEnrollments = this.cloneEnrollmentForCancel.studentEnrollmentListForView.filter((item) => {
    //   return item.exitCode == null;
    // });
    // for (let i = 0; i < this.cloneStudentEnrollment.studentEnrollments.length; i++) {
    //   this.selectedExitCodes[i] = null;
    //   this.cloneStudentEnrollment.studentEnrollments[i].schoolId = this.cloneEnrollmentForCancel.studentEnrollmentListForView[i].schoolId?.toString();
    //   this.cloneStudentEnrollment.studentEnrollments[i].gradeId = this.cloneEnrollmentForCancel.studentEnrollmentListForView[i].gradeId?.toString();
    // }

    if (this.cloneStudentEnrollment.studentEnrollments !== null) {
      for (let i = 0; i < this.cloneStudentEnrollment?.studentEnrollments?.length; i++) {
        if (this.divCount[i] === 2) {
          this.divCount.splice(i, 1);
        }
      }
    } else {
      this.cloneStudentEnrollment = new StudentEnrollmentModel();
    }
    // if (this.cloneStudentEnrollment !== JSON.parse(this.cloneStudentModel)) {
    //   this.cloneStudentEnrollment = JSON.parse(this.cloneStudentModel);
    //   this.studentCloneModel = JSON.parse(this.cloneStudentModel);
    //   this.divCount.length = this.cloneStudentEnrollment.studentEnrollments.length;
    // }
    this.getAllStudentEnrollments();
    this.studentCreateMode = this.studentCreate.VIEW;
    this.replaceEnrollmentCodeWithName();
    this.studentService.changePageMode(this.studentCreateMode);
  }
  replaceEnrollmentCodeWithName() {
    for (let i = 0; i < this.studentEnrollmentModel.studentEnrollmentListForView?.length; i++) {
      const index = this.schoolListWithGradeLevelsAndEnrollCodes.findIndex((x) => {
        return x.schoolId == +this.studentEnrollmentModel.studentEnrollmentListForView[i].schoolId;
      });
      for (let j = 0; j < this.schoolListWithGradeLevelsAndEnrollCodes[index].studentEnrollmentCode?.length; j++) {
        if (this.studentEnrollmentModel.studentEnrollmentListForView[i].enrollmentCode == this.schoolListWithGradeLevelsAndEnrollCodes[index].studentEnrollmentCode[j].enrollmentCode.toString()) {
          this.studentEnrollmentModel.studentEnrollmentListForView[i].enrollmentCode = this.schoolListWithGradeLevelsAndEnrollCodes[index].studentEnrollmentCode[j].title;
          break;
        }
      }
    }
  }


  toggleEnrollmentHistory() {
    this.expandEnrollmentHistory = !this.expandEnrollmentHistory;
  }

  //checking for dropped of student
  confirmDroppedOut() {
    this.customValid=false;
    this.currentForm.form.markAllAsTouched();
    this.defaultValuesService.customFieldsCheckParentComp.next(true);
    if (this.currentForm.form.invalid) {
      return;
    }
    delete this.studentEnrollmentModel.studentEnrollmentListForView;
    this.studentEnrollmentModel.studentEnrollments = this.cloneStudentEnrollment.studentEnrollments.filter((item) => {
      return item.gradeId !== null;
    });

    const dropEnrollDetails = this.studentEnrollmentModel.studentEnrollments.filter((item) => {
      return item.exitCode === '2';
    });
    if (dropEnrollDetails.length > 0) {
      const dialogRef = this.dialog.open(ConfirmDialogComponent, {
        maxWidth: '400px',
        data: {
          title: 'Are you sure?',
          message: 'You are about to drop ' + this.studentName + ' from ' + dropEnrollDetails[0].schoolName + '.'
        }
      });
      dialogRef.afterClosed().subscribe(dialogResult => {
        if (dialogResult) {
          if(this.customValid) this.updateStudentEnrollment();
        }
      });
    }
    else {
      if(this.customValid) this.updateStudentEnrollment();
    }

  }

  getAllStudentEnrollments() {
    this.studentEnrollmentViewModel = new StudentEnrollmentModel();
    const details = this.studentService.getStudentDetails();
    if (details != null) {
      this.studentEnrollmentModel.studentGuid = details.studentMaster.studentGuid;
    } else {
      this.studentEnrollmentModel.studentGuid = this.studentDetailsForViewAndEdit.studentMaster.studentGuid;
    }
    this.studentEnrollmentModel.studentId = this.studentService.getStudentId();
    this.studentEnrollmentModel.tenantId = this.defaultValueService.getTenantID();
    this.studentEnrollmentModel.schoolId = this.defaultValueService.getSchoolID();
    this.studentEnrollmentModel.academicYear = this.defaultValueService.getAcademicYear()?.toString();
    if (this.studentCreateMode === this.studentCreate.VIEW) {
      this.studentEnrollmentModel.studentGuid = this.studentDetailsForViewAndEdit.studentMaster.studentGuid;
    }
    this.studentService.getAllStudentEnrollment(this.studentEnrollmentModel).subscribe(res => {
      if (res) {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          if (!res.studentEnrollmentListForView) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        } else {
          this.studentEnrollmentModel = res;
          this.dropRowAdded();
          this.cloneEnrollmentForCancel = JSON.parse(JSON.stringify(res));
          this.getAllSection();
          this.findSectionNameById();
          for (let i = 0; i < this.studentEnrollmentViewModel.studentEnrollmentListForView?.length; i++) {

            //if(this.studentEnrollmentViewModel.studentEnrollmentListForView?.length -1 <= (i+1)){
              if (this.studentEnrollmentViewModel.studentEnrollmentListForView[i].enrollmentCode === "Dropped Out" && this.studentEnrollmentViewModel.studentEnrollmentListForView[i + 1]?.exitCode === "Dropped Out" 
                && this.studentEnrollmentViewModel.studentEnrollmentListForView[i].enrollmentDate ===this.studentEnrollmentViewModel.studentEnrollmentListForView[i + 1]?.exitDate) {
                this.studentEnrollmentViewModel.studentEnrollmentListForView.splice((i+1),1);
              }
            //}
          }
          this.cloneStudentEnrollment.studentEnrollments = res.studentEnrollmentListForView.filter((item) => item.isActive === true);
          this.cloneStudentModel = JSON.stringify(this.cloneStudentEnrollment);
          this.studentCloneModel = JSON.parse(this.cloneStudentModel);
          // this.cloneEnrollmentForCancel = JSON.stringify(this.studentEnrollmentModel);
          // this.cloneOfCloneEnrollmentForCancel = JSON.stringify(this.cloneStudentEnrollment);
          // for (let i = 0; i < this.cloneStudentEnrollment.studentEnrollments?.length; i++) {
          //   this.divCount[i] = i;
          // }
          if (this.studentCreateMode === this.studentCreate.ADD) {
            this.getAllSchoolListWithGradeLevelsAndEnrollCodes();
          }
          this.getAllCalendar();
        }
      }
      else {
        this.snackbar.open(this.defaultValueService.translateKey('studentEnrollmentsfailedtofetch') + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  // show duplicate data for exittype drop out
  dropRowAdded() {
    for (let i = 0; i < this.studentEnrollmentModel.studentEnrollmentListForView?.length; i++) {

      if (this.studentEnrollmentModel.studentEnrollmentListForView[i].exitType === "Drop") {

        this.studentEnrollmentViewModel.studentEnrollmentListForView.push(JSON.parse(JSON.stringify(this.studentEnrollmentModel.studentEnrollmentListForView[i])));
        this.studentEnrollmentViewModel.studentEnrollmentListForView[this.studentEnrollmentViewModel.studentEnrollmentListForView.length - 1].showDrop = true;
        this.studentEnrollmentViewModel.studentEnrollmentListForView.push(JSON.parse(JSON.stringify(this.studentEnrollmentModel.studentEnrollmentListForView[i])));
        this.studentEnrollmentViewModel.studentEnrollmentListForView[this.studentEnrollmentViewModel.studentEnrollmentListForView.length - 1].showDrop = false;
      }
      else {
        // if (this.studentEnrollmentViewModel.studentEnrollmentListForView?.length > 1) {
        //   this.studentEnrollmentViewModel.studentEnrollmentListForView.push(new StudentEnrollmentDetails());
        // }
        //let lastIndex = this.studentEnrollmentViewModel.studentEnrollmentListForView?.length - 1;
        this.studentEnrollmentViewModel.studentEnrollmentListForView.push(this.studentEnrollmentModel.studentEnrollmentListForView[i]); 
      }

    }
    if (this.studentEnrollmentViewModel.studentEnrollmentListForView[0]?.schoolId === undefined) {
      this.studentEnrollmentViewModel.studentEnrollmentListForView.splice(0, 1);
    }
  }

  findEnrollmentCodeIdByName() {
    for (let i = 0; i < this.studentEnrollmentModel.studentEnrollmentListForView?.length; i++) {
      const index = this.schoolListWithGradeLevelsAndEnrollCodes.findIndex((x) => {
        return x.schoolId == +this.studentEnrollmentModel.studentEnrollmentListForView[i].schoolId;
      });
      for (let j = 0; j < this.schoolListWithGradeLevelsAndEnrollCodes[index].studentEnrollmentCode?.length; j++) {
        if (this.studentEnrollmentModel.studentEnrollmentListForView[i].enrollmentCode == this.schoolListWithGradeLevelsAndEnrollCodes[index].studentEnrollmentCode[j].title) {
          this.studentEnrollmentModel.studentEnrollmentListForView[i].enrollmentCode = this.schoolListWithGradeLevelsAndEnrollCodes[index].studentEnrollmentCode[j].enrollmentCode.toString();
          break;
        }
      }
    }
  }

  findExitCodeIdByName() {
    for (let i = 0; i < this.studentEnrollmentModel.studentEnrollmentListForView?.length; i++) {
      const index = this.schoolListWithGradeLevelsAndEnrollCodes.findIndex((x) => {
        return x.schoolId == +this.studentEnrollmentModel.studentEnrollmentListForView[i].schoolId;
      });
      for (let j = 0; j < this.schoolListWithGradeLevelsAndEnrollCodes[index].studentEnrollmentCode?.length; j++) {
        if (this.studentEnrollmentModel.studentEnrollmentListForView[i].exitCode == this.schoolListWithGradeLevelsAndEnrollCodes[index].studentEnrollmentCode[j].title) {
          this.studentEnrollmentModel.studentEnrollmentListForView[i].exitCode = this.schoolListWithGradeLevelsAndEnrollCodes[index].studentEnrollmentCode[j].enrollmentCode.toString();
          break;
        }
      }
    }
  }

  findCalendarNameById() {
    if (this.calendarListModel.calendarList != null) {
      for (let i = 0; i < this.calendarListModel.calendarList?.length; i++) {
        if (this.studentEnrollmentModel.calenderId != null && this.calendarListModel.calendarList[i].calenderId.toString() == this.studentEnrollmentModel.calenderId.toString()) {
          this.calendarNameInViewMode = this.calendarListModel.calendarList[i].title;
        } else {
          this.calendarNameInViewMode = '-';
        }
      }
    }
  }

  manipulateModelInEditMode() {
    this.filteredEnrollmentInfoInViewMode = this.studentEnrollmentModel.studentEnrollmentListForView.filter((x) => {
      return x.exitCode == null;
    });
    this.filteredEnrollmentInfoInViewMode = this.filteredEnrollmentInfoInViewMode.reverse();
    if (this.filteredEnrollmentInfoInViewMode?.length == 1) {
      this.filteredEnrollmentInfoInViewMode[0]?.enrollmentType == 'Drop' ? this.studentActiveStatus = false : ''
    }
    if (this.studentEnrollmentModel.calenderId != null) {
      for (let i = 0; i < this.studentEnrollmentModel.studentEnrollmentListForView?.length; i++) {
        this.studentEnrollmentModel.calenderId = this.studentEnrollmentModel.calenderId.toString();
      }
    }
    for (let i = 0; i < this.cloneStudentEnrollment.studentEnrollments.length; i++) {
      this.selectedExitCodes[i] = null;
      // this.cloneStudentEnrollment.studentEnrollments[i].schoolId = this.studentEnrollmentModel.studentEnrollmentListForView[i].schoolId?.toString();
      // this.cloneStudentEnrollment.studentEnrollments[i].gradeId = this.studentEnrollmentModel.studentEnrollmentListForView[i].gradeId?.toString();
      this.cloneStudentEnrollment.studentEnrollments[i].enrollmentCodeName = this.studentEnrollmentModel.studentEnrollmentListForView[i]?.enrollmentCode?.toString();
      this.cloneStudentEnrollment.studentEnrollments[i].exitCodeName = this.studentEnrollmentModel.studentEnrollmentListForView[i]?.exitCode?.toString();
      this.cloneStudentEnrollment.studentEnrollments[i].schoolId = this.studentEnrollmentModel.studentEnrollmentListForView[i].schoolId?.toString();
      this.cloneStudentEnrollment.studentEnrollments[i].gradeId = this.studentEnrollmentModel.studentEnrollmentListForView[i].gradeId?.toString();
      this.cloneStudentEnrollment.studentEnrollments[i].programId = this.studentEnrollmentModel.studentEnrollmentListForView[i].programId?.toString();
      this.cloneStudentEnrollment.studentEnrollments[i].transferredProgramId = this.studentEnrollmentModel.studentEnrollmentListForView[i].transferredProgramId?.toString();
      this.cloneStudentEnrollment.studentEnrollments[i].enrollmentCode = this.studentEnrollmentModel.studentEnrollmentListForView[i]?.enrollmentCodeId?.toString();
      this.cloneStudentEnrollment.studentEnrollments[i].exitCode = this.studentEnrollmentModel.studentEnrollmentListForView[i]?.exitCodeId?.toString();
      
    }
    this.findCalendarNameById();
  }

  updateStudentEnrollment() {
    const details = this.studentService.getStudentDetails();
    this.studentEnrollmentModel.fieldsCategoryList = this.studentDetailsForViewAndEdit.fieldsCategoryList;

    if (this.studentEnrollmentModel.fieldsCategoryList?.length > 0) {
      this.studentEnrollmentModel.selectedCategoryId = + this.studentEnrollmentModel.fieldsCategoryList[this.categoryId]?.categoryId;
      for (const studentCustomField of this.studentEnrollmentModel.fieldsCategoryList[this.categoryId]?.customFields) {
        if (studentCustomField.type === 'Multiple SelectBox' && this.studentService.getStudentMultiselectValue() !== undefined) {
          studentCustomField.customFieldsValue[0].customFieldValue = this.studentService.getStudentMultiselectValue().toString().replaceAll(',', '|');
        }
      }
    }

    for (let i = 0; i < this.studentEnrollmentModel.studentEnrollments?.length; i++) {
      this.studentEnrollmentModel.studentEnrollments[i].studentId = this.studentEnrollmentModel.studentEnrollments[i].studentId ? this.studentEnrollmentModel.studentEnrollments[i].studentId : this.studentService.getStudentId();
      this.studentEnrollmentModel.studentEnrollments[i].academicYear = +this.defaultValueService.getAcademicYear();
      this.studentEnrollmentModel.studentEnrollments[i].enrollmentDate = this.commonFunction.formatDateSaveWithoutTime(this.studentEnrollmentModel.studentEnrollments[i].enrollmentDate)
      this.studentEnrollmentModel.studentEnrollments[i].exitDate = this.commonFunction.formatDateSaveWithoutTime(this.studentEnrollmentModel.studentEnrollments[i].exitDate)
      if (details != null) {
        this.studentEnrollmentModel.studentEnrollments[i].studentGuid = details.studentMaster.studentGuid;
      } else {
        this.studentEnrollmentModel.studentEnrollments[i].studentGuid = this.studentDetailsForViewAndEdit.studentMaster.studentGuid;

      }
    }
    if (details != null) {
      this.studentEnrollmentModel.studentGuid = details.studentMaster.studentGuid;
    } else {
      this.studentEnrollmentModel.studentGuid = this.studentDetailsForViewAndEdit.studentMaster.studentGuid;
    }
    this.studentEnrollmentModel.estimatedGradDate = this.commonFunction.formatDateSaveWithoutTime(this.studentEnrollmentModel.estimatedGradDate)

    this.studentEnrollmentModel.academicYear = this.defaultValueService.getAcademicYear()?.toString();
    this.studentEnrollmentModel.schoolId = +this.defaultValueService.getSchoolID();
    this.studentEnrollmentModel._userName = this.defaultValueService.getUserName();

    this.studentEnrollmentModel.studentEnrollments.map(item => item.updatedBy = this.defaultValuesService.getUserGuidId());

    this.studentService.updateStudentEnrollment(this.studentEnrollmentModel).subscribe((res) => {
      if (res) {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        } else {
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
          let isStudentInactive = false;
          res.studentEnrollments.map((item) => {
            // if (res.rollingOption === RollingOptionsEnum['Do not enroll after this school year'] && +item.exitCode === 2){
            //   this.router.navigate(['school/students']);
            //   isStudentInactive=true;
            // }

            const index = this.schoolListWithGradeLevelsAndEnrollCodes.findIndex((x) => {
              return x.schoolId == +item.schoolId;
            });
            const dropType = this.schoolListWithGradeLevelsAndEnrollCodes[index].studentEnrollmentCode?.find(x => x.enrollmentCode === +item.exitCode)?.type.toString();
            if (dropType === 'Drop') {
              this.router.navigate(['school/students']);
              isStudentInactive = true;
            }
          });
          this.studentEnrollmentModel.studentEnrollments.map((item) => {
            if (item.schoolTransferred) {
              this.router.navigate(['school/students']);
              isStudentInactive = true;
            }
          })
          if (!isStudentInactive) {
            this.getAllStudentEnrollments();
            this.studentCreateMode = this.studentCreate.VIEW;
            this.studentService.changePageMode(this.studentCreateMode);
          }
          // this.studentEnrollmentModel=res;
          // this.replaceEnrollmentCodeWithName();

        }
      }
      else {
        this.snackbar.open(this.defaultValueService.translateKey('enrollmentUpdatefailed') + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  getAllSection() {
    if (!this.sectionList.isSectionAvailable) {
      this.sectionList.isSectionAvailable = true;
      this.sectionService.GetAllSection(this.sectionList).pipe(takeUntil(this.destroySubject$)).subscribe(res => {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          if (!res.tableSectionsList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        }
        else {
          this.sectionList.tableSectionsList = res.tableSectionsList;
          if (this.studentCreateMode === this.studentCreate.VIEW) {
            this.findSectionNameById();
          }
        }
      });
    }
  }

  findSectionNameById() {
    this.sectionList?.tableSectionsList?.map((item) => {
      if (item.sectionId === +this.studentEnrollmentModel.sectionId) {
        this.sectionNameInViewMode = item.name;
      }
    })
  }

  onGradeLevelChange(indexOfDynamicRow) {
    const indexOfGradeLevel = this.schoolListWithGradeLevelsAndEnrollCodes[this.selectedSchoolIndex[indexOfDynamicRow]]?.gradelevels.findIndex((item) => {
      return item.gradeId === +this.cloneStudentEnrollment.studentEnrollments[indexOfDynamicRow].gradeId;
    });
    this.cloneStudentEnrollment.studentEnrollments[indexOfDynamicRow].gradeLevelTitle = this.schoolListWithGradeLevelsAndEnrollCodes[this.selectedSchoolIndex[indexOfDynamicRow]]?.gradelevels[indexOfGradeLevel].title;
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
    
  }

}

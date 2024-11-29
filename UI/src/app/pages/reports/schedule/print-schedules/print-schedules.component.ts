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
import { TranslateService } from '@ngx-translate/core';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { StudentListModel } from 'src/app/models/student.model';
import { ScheduleStudentListViewModel } from 'src/app/models/student-schedule.model';
import { FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged, switchMap, takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LoaderService } from 'src/app/services/loader.service';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';
import { MarkingPeriodListModel } from 'src/app/models/marking-period.model';
import { ProfilesTypes } from 'src/app/enums/profiles.enum';
import { GetPrintScheduleReportModel } from 'src/app/models/report.model';
import { SharedFunction } from 'src/app/pages/shared/shared-function';
import { StudentService } from 'src/app/services/student.service';
import { StudentScheduleService } from 'src/app/services/student-schedule.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatCheckbox } from '@angular/material/checkbox';
import { AdvancedSearchExpansionModel } from 'src/app/models/common.model';
import { fadeInRight400ms } from 'src/@vex/animations/fade-in-right.animation';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { ReportService } from 'src/app/services/report.service';

@Component({
  selector: 'vex-print-schedules',
  templateUrl: './print-schedules.component.html',
  styleUrls: ['./print-schedules.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class PrintSchedulesComponent implements OnInit, AfterViewInit, OnDestroy {
  // displayedColumns: string[] = ['studentCheck', 'studentName', 'studentId', 'alternateId', 'grade', 'section', 'phone'];
  columns = [
    { label: '', property: 'studentCheck', type: 'text', visible: true },
    { label: 'Name', property: 'studentName', type: 'text', visible: true },
    { label: 'Student ID', property: 'studentId', type: 'text', visible: true },
    { label: 'Alternate ID', property: 'alternateId', type: 'text', visible: true },
    { label: 'Grade Level', property: 'grade', type: 'text', visible: true },
    { label: 'section', property: 'section', type: 'text', visible: true },
    { label: 'Telephone', property: 'phone', type: 'text', visible: true },
    { label: 'Status', property: 'status', type: 'text', visible: false }
  ];
  studentList: MatTableDataSource<any>;
  selectOptions = [];
  getAllStudentModel: StudentListModel = new StudentListModel();
  scheduleStudentListViewModel: ScheduleStudentListViewModel = new ScheduleStudentListViewModel();
  markingPeriodListModel: MarkingPeriodListModel = new MarkingPeriodListModel();
  getPrintScheduleReportModel: GetPrintScheduleReportModel = new GetPrintScheduleReportModel();
  advancedSearchExpansionModel: AdvancedSearchExpansionModel = new AdvancedSearchExpansionModel();
  searchCtrl: FormControl;
  loading: boolean;
  destroySubject$: Subject<void> = new Subject();
  selectedReportBy;
  isAdmin: boolean;
  profiles = ProfilesTypes;
  totalCount: number = 0;
  pageNumber: number;
  pageSize: number;
  isFromAdvancedSearch: boolean = false;
  showAdvanceSearchPanel: boolean = false;
  searchCount;
  searchValue;
  toggleValues;
  listOfStudents = [];
  selectedStudents = [];
  today: Date = new Date();
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild('masterCheckBox') masterCheckBox: MatCheckbox;
  studentListSubject$: Subject<void> = new Subject();
  studentListByTeacherSubject$: Subject<void> = new Subject();
  printScheduleReportData;

  constructor(
    public translateService: TranslateService,
    public defaultValuesService: DefaultValuesService,
    private snackbar: MatSnackBar,
    private loaderService: LoaderService,
    private markingPeriodService: MarkingPeriodService,
    private commonFunction: SharedFunction,
    private studentService: StudentService,
    private studentScheduleService: StudentScheduleService,
    private reportService: ReportService
  ) {
    this.advancedSearchExpansionModel.accessInformation = false;
    this.advancedSearchExpansionModel.enrollmentInformation = true;
    this.advancedSearchExpansionModel.searchAllSchools = false;
    this.defaultValuesService.setReportCompoentTitle.next("Print Schedules");
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
    (this.defaultValuesService.getUserMembershipType() === this.profiles.Teacher || this.defaultValuesService.getUserMembershipType() === this.profiles.HomeroomTeacher) ? this.isAdmin = false : this.isAdmin = true;
    this.selectedReportBy = this.defaultValuesService.getMarkingPeriodTitle();
    this.getPrintScheduleReportModel.markingPeriodStartDate = this.commonFunction.formatDateSaveWithoutTime(this.defaultValuesService.getMarkingPeriodStartDate());
    this.getPrintScheduleReportModel.markingPeriodEndDate = this.commonFunction.formatDateSaveWithoutTime(this.defaultValuesService.getMarkingPeriodEndDate());
  }

  ngOnInit(): void {
    this.getAllStudentModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.scheduleStudentListViewModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.searchCtrl = new FormControl();
    this.getMarkingPeriod();
    this.callAllStudent();
  }

  ngAfterViewInit(): void {
    if (this.isAdmin) {
      // For searching
      this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
        if (term.trim().length > 0) {
          let filterParams = [
            {
              columnName: null,
              filterValue: term,
              filterOption: 3
            }
          ];
          Object.assign(this.getAllStudentModel, { filterParams: filterParams });
          this.getAllStudentModel.pageNumber = 1;
          this.paginator.pageIndex = 0;
          this.getAllStudentModel.pageSize = this.pageSize;
          if (this.getAllStudentModel.sortingModel?.sortColumn === "") {
            this.getAllStudentModel.sortingModel = null;
          }
          this.studentListSubject$.next();
        }
        else {
          Object.assign(this.getAllStudentModel, { filterParams: null });
          this.getAllStudentModel.pageNumber = this.paginator.pageIndex + 1;
          this.getAllStudentModel.pageSize = this.pageSize;
          if (this.getAllStudentModel.sortingModel?.sortColumn === "") {
            this.getAllStudentModel.sortingModel = null;
          }
          this.studentListSubject$.next();
        }
      });
    } else {
      // For searching
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
          this.scheduleStudentListViewModel.staffId = this.defaultValuesService.getUserId();
          this.scheduleStudentListViewModel.academicYear = this.defaultValuesService.getAcademicYear();
          if (this.scheduleStudentListViewModel.sortingModel?.sortColumn == "") {
            this.scheduleStudentListViewModel.sortingModel = null;
          }
          this.studentListByTeacherSubject$.next();
        }
        else {
          Object.assign(this.scheduleStudentListViewModel, { filterParams: null });
          this.scheduleStudentListViewModel.pageNumber = this.paginator.pageIndex + 1;
          this.scheduleStudentListViewModel.pageSize = this.pageSize;
          this.scheduleStudentListViewModel.staffId = this.defaultValuesService.getUserId();
          this.scheduleStudentListViewModel.academicYear = this.defaultValuesService.getAcademicYear();
          if (this.scheduleStudentListViewModel.sortingModel?.sortColumn == "") {
            this.scheduleStudentListViewModel.sortingModel = null;
          }
          this.studentListByTeacherSubject$.next();
        }
      });
    }
    this.callAllStudentBySearch();
  }

  // For get all marking period
  getMarkingPeriod() {
    this.markingPeriodService.GetMarkingPeriod(this.markingPeriodListModel).subscribe((data: any) => {
      if (data._failure) {

      } else {
        for (let i = 0; i < data.schoolYearsView.length; i++) {
          this.selectOptions.push({
            title: data.schoolYearsView[i]?.title,
            startDate: data.schoolYearsView[i]?.startDate,
            endDate: data.schoolYearsView[i]?.endDate,
            subTitle: data.schoolYearsView[i]?.title
          });
          if (data.schoolYearsView[i].children.length > 0) {
            data.schoolYearsView[i].children.map((item: any) => {
              this.selectOptions.push({
                title: item?.title,
                startDate: item?.startDate,
                endDate: item?.endDate,
                subTitle: item?.title
              });
              if (item.children.length > 0) {
                item.children.map((subItem: any) => {
                  this.selectOptions.push({
                    title: subItem?.title,
                    startDate: subItem?.startDate,
                    endDate: subItem?.endDate,
                    subTitle: subItem?.title
                  });
                  if (subItem.children.length > 0) {
                    subItem.children.map((subOfSubItem: any) => {
                      this.selectOptions.push({
                        title: subOfSubItem?.title,
                        startDate: subOfSubItem?.startDate,
                        endDate: subOfSubItem?.endDate,
                        subTitle: subOfSubItem?.title
                      });
                    });
                  }
                });
              }
            });
          }
        }
      }
    });
  }

  // For get all student list from API
  callAllStudentBySearch() {
    this.studentListSubject$.pipe(switchMap(() => this.studentService.GetAllStudentList(this.getAllStudentModel))).subscribe(res => {
      if (res) {
        if (res._failure) {
          if (res.studentListViews === null) {
            this.totalCount = this.isFromAdvancedSearch ? 0 : null;
            this.searchCount = this.isFromAdvancedSearch ? 0 : null;
            this.studentList = new MatTableDataSource([]);
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
            this.isFromAdvancedSearch = false;
          } else {
            this.studentList = new MatTableDataSource([]);
            this.totalCount = this.isFromAdvancedSearch ? 0 : null;
            this.searchCount = this.isFromAdvancedSearch ? 0 : null;
            this.isFromAdvancedSearch = false;
          }
        } else {
          this.totalCount = res.totalCount;
          this.searchCount = res.totalCount;
          this.pageNumber = res.pageNumber;
          this.pageSize = res._pageSize;
          res.studentListViews.forEach((student) => {
            student.checked = false;
          });
          this.listOfStudents = res.studentListViews.map((item) => {
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
          this.studentList = new MatTableDataSource(res.studentListViews);
          this.isFromAdvancedSearch = false;
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });

    this.studentListByTeacherSubject$.pipe(switchMap(() => this.studentScheduleService.searchScheduledStudentForGroupDrop(this.scheduleStudentListViewModel))).subscribe(data => {
      if (data) {
        if (data._failure) {
          if (data.scheduleStudentForView === null) {
            this.totalCount = this.isFromAdvancedSearch ? 0 : null;
            this.searchCount = this.isFromAdvancedSearch ? 0 : null;
            this.studentList = new MatTableDataSource([]);
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
            this.isFromAdvancedSearch = false;
          } else {
            this.studentList = new MatTableDataSource([]);
            this.totalCount = this.isFromAdvancedSearch ? 0 : null;
            this.searchCount = this.isFromAdvancedSearch ? 0 : null;
            this.isFromAdvancedSearch = false;
          }
        } else {
          this.totalCount = data.totalCount;
          this.searchCount = data.totalCount ? data.totalCount : null;
          this.pageNumber = data.pageNumber;
          this.pageSize = data._pageSize;
          data.scheduleStudentForView.forEach((student) => {
            student.checked = false;
          });
          this.listOfStudents = data.scheduleStudentForView.map((item) => {
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
          this.studentList = new MatTableDataSource(data.scheduleStudentForView);
          this.isFromAdvancedSearch = false;
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  // For get all student list from API
  callAllStudent() {
    if (this.isAdmin) {
      if (this.getAllStudentModel.sortingModel?.sortColumn === "") {
        this.getAllStudentModel.sortingModel = null;
      }
      this.studentService.GetAllStudentList(this.getAllStudentModel).subscribe(res => {
        if (res) {
          if (res._failure) {
            if (res.studentListViews === null) {
              this.totalCount = this.isFromAdvancedSearch ? 0 : null;
              this.searchCount = this.isFromAdvancedSearch ? 0 : null;
              this.studentList = new MatTableDataSource([]);
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
              this.isFromAdvancedSearch = false;
            } else {
              this.studentList = new MatTableDataSource([]);
              this.totalCount = this.isFromAdvancedSearch ? 0 : null;
              this.searchCount = this.isFromAdvancedSearch ? 0 : null;
              this.isFromAdvancedSearch = false;
            }
          } else {
            this.totalCount = res.totalCount;
            this.searchCount = res.totalCount;
            this.pageNumber = res.pageNumber;
            this.pageSize = res._pageSize;
            res.studentListViews.forEach((student) => {
              student.checked = false;
            });
            this.listOfStudents = res.studentListViews.map((item) => {
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
            this.studentList = new MatTableDataSource(res.studentListViews);
            this.isFromAdvancedSearch = false;
          }
        } else {
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      });
    } else {
      this.scheduleStudentListViewModel.staffId = this.defaultValuesService.getUserId();
      this.scheduleStudentListViewModel.academicYear = this.defaultValuesService.getAcademicYear();
      if (this.scheduleStudentListViewModel.sortingModel?.sortColumn == "") {
        this.scheduleStudentListViewModel.sortingModel = null;
      }
      this.studentScheduleService.searchScheduledStudentForGroupDrop(this.scheduleStudentListViewModel).subscribe(data => {
        if (data) {
          if (data._failure) {
            if (data.scheduleStudentForView === null) {
              this.totalCount = this.isFromAdvancedSearch ? 0 : null;
              this.searchCount = this.isFromAdvancedSearch ? 0 : null;
              this.studentList = new MatTableDataSource([]);
              this.snackbar.open(data._message, '', {
                duration: 10000
              });
              this.isFromAdvancedSearch = false;
            } else {
              this.studentList = new MatTableDataSource([]);
              this.totalCount = this.isFromAdvancedSearch ? 0 : null;
              this.searchCount = this.isFromAdvancedSearch ? 0 : null;
              this.isFromAdvancedSearch = false;
            }
          } else {
            this.totalCount = data.totalCount;
            this.searchCount = data.totalCount ? data.totalCount : null;
            this.pageNumber = data.pageNumber;
            this.pageSize = data._pageSize;
            data.scheduleStudentForView.forEach((student) => {
              student.checked = false;
            });
            this.listOfStudents = data.scheduleStudentForView.map((item) => {
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
            this.studentList = new MatTableDataSource(data.scheduleStudentForView);
            this.isFromAdvancedSearch = false;
          }
        } else {
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      });
    }
  }

  // For server side pagination
  getPageEvent(event) {
    if (this.isAdmin) {
      if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
        let filterParams = [
          {
            columnName: null,
            filterValue: this.searchCtrl.value,
            filterOption: 3
          }
        ]
        Object.assign(this.getAllStudentModel, { filterParams: filterParams });
      }
      this.getAllStudentModel.pageNumber = event.pageIndex + 1;
      this.getAllStudentModel.pageSize = event.pageSize;
      this.defaultValuesService.setPageSize(event.pageSize);
      this.callAllStudent();
    } else {
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
      this.defaultValuesService.setPageSize(event.pageSize);
      this.callAllStudent();
    }
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
    this.studentList = new MatTableDataSource(this.listOfStudents);
    this.decideCheckUncheck();
  }

  onChangeSelection(eventStatus: boolean, studentId) {
    for (let item of this.listOfStudents) {
      if (item.studentId === studentId) {
        item.checked = eventStatus;
        break;
      }
    }
    this.studentList = new MatTableDataSource(this.listOfStudents);
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

  /* This is for get all data from the Advanced Search component and then call the API in this page 
  NOTE: We just get the filterParams Array from Search component
  */
  filterData(res) {
    this.isFromAdvancedSearch = true;
    this.getAllStudentModel = new StudentListModel();
    this.scheduleStudentListViewModel = new ScheduleStudentListViewModel();
    this.getAllStudentModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.scheduleStudentListViewModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    if (res) {
      if (this.defaultValuesService.getUserMembershipType() === this.profiles.HomeroomTeacher || this.defaultValuesService.getUserMembershipType() === this.profiles.Teacher) {
        this.scheduleStudentListViewModel.filterParams = res.filterParams;
        this.scheduleStudentListViewModel.includeInactive = res.inactiveStudents;
        this.scheduleStudentListViewModel.dobStartDate = this.commonFunction.formatDateSaveWithoutTime(res.dobStartDate);
        this.scheduleStudentListViewModel.dobEndDate = this.commonFunction.formatDateSaveWithoutTime(res.dobEndDate);
        this.callAllStudent();
      } else {
        this.getAllStudentModel.filterParams = res.filterParams;
        this.getAllStudentModel.includeInactive = res.inactiveStudents;
        this.getAllStudentModel.searchAllSchool = res.searchAllSchool;
        this.getAllStudentModel.dobStartDate = this.commonFunction.formatDateSaveWithoutTime(res.dobStartDate);
        this.getAllStudentModel.dobEndDate = this.commonFunction.formatDateSaveWithoutTime(res.dobEndDate);
        this.defaultValuesService.sendIncludeInactiveFlag(res.inactiveStudents);
        this.defaultValuesService.sendAllSchoolFlag(res.searchAllSchool);
        this.callAllStudent();
      }
    }
  }

  getToggleValues(event) {
    this.toggleValues = event;
    if (event.inactiveStudents === true) {
      this.columns[7].visible = true;
    }
    else if (event.inactiveStudents === false) {
      this.columns[7].visible = false;
    }
  }

  hideAdvanceSearch(event) {
    this.showAdvanceSearchPanel = false;
  }

  getSearchInput(event) {
    this.searchValue = event;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  getReportBy(event) {
    if (event.value) {
      const selectedOption = this.selectOptions.filter(x => x.subTitle === event.value);
      this.getPrintScheduleReportModel.markingPeriodStartDate = this.commonFunction.formatDateSaveWithoutTime(selectedOption[0].startDate);
      this.getPrintScheduleReportModel.markingPeriodEndDate = this.commonFunction.formatDateSaveWithoutTime(selectedOption[0].endDate);
    } else {
      this.getPrintScheduleReportModel.markingPeriodStartDate = null;
      this.getPrintScheduleReportModel.markingPeriodEndDate = null;
    }
  }

  getPrintScheduleReport() {
    return new Promise((resolve, reject) => {
      this.reportService.getPrintScheduleReport(this.getPrintScheduleReportModel).subscribe(res => {
        if (res) {
          if (res._failure) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          } else {
            resolve(res);
          }
        } else {
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      });
    });
  }

  generateReport() {
    if (!this.selectedReportBy) {
      this.getPrintScheduleReportModel.markingPeriodStartDate = this.getPrintScheduleReportModel.markingPeriodStartDate ? this.commonFunction.formatDateSaveWithoutTime(this.getPrintScheduleReportModel.markingPeriodStartDate) : null;
      this.getPrintScheduleReportModel.markingPeriodEndDate = this.getPrintScheduleReportModel.markingPeriodEndDate ? this.commonFunction.formatDateSaveWithoutTime(this.getPrintScheduleReportModel.markingPeriodEndDate) : null;
      if (this.getPrintScheduleReportModel.markingPeriodStartDate && this.getPrintScheduleReportModel.markingPeriodEndDate) {
        if (this.getPrintScheduleReportModel.markingPeriodStartDate > this.getPrintScheduleReportModel.markingPeriodEndDate) {
          this.snackbar.open("To date value should be greater than from date value", "", {
            duration: 10000
          });
          return;
        }
      } else if (!this.getPrintScheduleReportModel.markingPeriodStartDate && !this.getPrintScheduleReportModel.markingPeriodEndDate) {
        this.snackbar.open("Choose from date and to date", "", {
          duration: 10000
        });
        return;
      } else if ((this.getPrintScheduleReportModel.markingPeriodStartDate && !this.getPrintScheduleReportModel.markingPeriodEndDate) || (!this.getPrintScheduleReportModel.markingPeriodStartDate && this.getPrintScheduleReportModel.markingPeriodEndDate)) {
        this.snackbar.open("Choose both from date and to date", "", {
          duration: 10000,
        });
        return;
      }
    }

    if (!this.selectedStudents.length) {
      this.snackbar.open(this.defaultValuesService.translateKey('selectAtLeastOneStudent'), '', {
        duration: 3000
      });
      return;
    }

    this.getPrintScheduleReportModel.studentGuids = this.selectedStudents.map(item => item.studentGuid);

    if (this.defaultValuesService.getUserMembershipType() !== this.profiles.SuperAdmin && this.defaultValuesService.getUserMembershipType() !== this.profiles.SchoolAdmin && this.defaultValuesService.getUserMembershipType() !== this.profiles.AdminAssitant) {
      this.getPrintScheduleReportModel.staffId = +this.defaultValuesService.getUserId();
    }

    this.getPrintScheduleReport().then((res: GetPrintScheduleReportModel) => {
      res.studentDetailsViewModelList.map(studentDetails => {
        studentDetails?.courseDetailsViewModelList?.map(courseDetails => {
          courseDetails.modifiedDataList = [];
          courseDetails?.courseSectionDetailsViewModelList?.map(courseSectionDetails => {
            courseSectionDetails?.dayDetailsViewModelList?.map(dayDetails => {
              dayDetails?.datePeriodRoomDetailsViewModelList?.map((datePeriodRoomDetails, datePeriodRoomDetailsIndex) => {
                courseDetails.modifiedDataList.push({ date: datePeriodRoomDetails?.date, periodName: datePeriodRoomDetails?.periodName, roomName: datePeriodRoomDetails?.roomName, dayName: dayDetails?.dayName, courseSectionName: courseSectionDetails?.courseSectionName, staffName: courseSectionDetails?.staffName, courseName: courseDetails?.courseName, isFirst: datePeriodRoomDetailsIndex === 0 ? true : false, length: dayDetails?.datePeriodRoomDetailsViewModelList?.length });
              });
            });
          });
        });
      });

      this.printScheduleReportData = res;

      setTimeout(() => {
        this.generatePDF();
      }, 100 * this.printScheduleReportData.studentDetailsViewModelList.length);
    });
  }

  // For open the print window
  generatePDF() {
    let printContents, popupWin;
    printContents = document.getElementById('printSectionId').innerHTML;
    document.getElementById('printSectionId').className = 'block';
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    if (popupWin === null || typeof (popupWin) === 'undefined') {
      document.getElementById('printSectionId').className = 'hidden';
      this.snackbar.open("User needs to allow the popup from the browser", '', {
        duration: 10000
      });
    } else {
      popupWin.document.open();
      popupWin.document.write(`
      <html>
        <head>
          <title>Print tab</title>
          <style>
          h1,
          h2,
          h3,
          h4,
          h5,
          h6,
          p {
            margin: 0;
          }
          body {
            -webkit-print-color-adjust: exact;
            font-family: Arial;
            background-color: #fff;
          }
          table {
            border-collapse: collapse;
            width: 100%;
          }
          .student-information-report {
              width: 1024px;
              margin: auto;
          }
          .float-left {
            float: left;
          }
          .text-center {
            text-align: center;
          }
          .text-right {
            text-align: right;
          }
          .inline-block {
              display: inline-block;
          }
          .border-table {
              border: 1px solid #000;
              border-top: none;
          }
          .clearfix::after {
              display: block;
              clear: both;
              content: "";
            }
          .report-header {
              padding: 20px 0;
              border-bottom: 2px solid #000;
          }
          .school-logo {
              width: 80px;
              height: 80px;
              border-radius: 50%;
              border: 2px solid #cacaca;
              margin-right: 20px;
              text-align: center;
              overflow: hidden;
          }
          .school-logo img {
              width: 100%;
              overflow: hidden;
          }
          .report-header td {
              padding: 20px;
              padding-bottom: 10px;
          }
          .report-header td.generate-date {
              padding: 0;
          }
          .report-header .information h4 {
              font-size: 20px;
              font-weight: 600;
              padding: 10px 0;
          }
          .report-header .information p, .header-right p {
              font-size: 16px;
          }
          .header-right div {
              background-color: #000;
              color: #fff;
              font-size: 20px;
              padding: 5px 20px;
              font-weight: 600;
              margin-bottom: 8px;
          }
          .student-logo {
              padding: 20px;
          }
          .student-logo div {
              width: 100%;
              height: 100%;
              border: 1px solid rgb(136, 136, 136);
              border-radius: 3px;
          }
          .student-logo img {
              width: 100%;
          }
          .student-details {
              padding: 20px;
              vertical-align: top;
          }
          .student-details h4 {
              font-size: 22px;
              font-weight: 600;
              margin-bottom: 10px;
          }
          .student-details span {
              color: #817e7e;
              padding: 0 15px;
              font-size: 20px;
          }
          .student-details p {
              color: #121212;
              font-size: 16px;
          }
          .student-details table {
              border-collapse: separate;
              border-spacing:0;
              border-radius: 10px;
          }
          .student-details table td, .student-details table th {
              border-left: 1px solid #000;
              border-top: 1px solid #000;
              padding: 8px 10px;
              text-align: left;
          }
          .information.student-details table td {
              width: 33.33%;
          }
          .information.student-details .border-table {
            border-top: 1px solid #000;
          }
          .information.student-details table tr:first-child td {
              border-top: none;
          }
          .student-details table td b, .student-details table span {
              color: #000;
              font-size: 16px;
          }
          .student-details table td b {
              font-weight: 600;
          }
          .student-details table td:first-child, .student-details table th:first-child {
              border-left: none;
          }
          .student-details table tr:first-child td {
              border-bottom: none;
          }
          .student-details table th:first-child {
              border-top-left-radius: 10px;
          }
          .student-details table th:last-child {
              border-top-right-radius: 10px;
          }
          .p-b-8 {
              padding-bottom: 8px;
          }
          .width-160 {
              width: 160px;
          }
          .m-b-15 {
              margin-bottom: 15px;
          }
          .bg-black {
              background-color: #000;
          }
          .bg-slate {
              background-color: #E5E5E5;
          }
          .information-table td {
              font-size: 16px;
          }

          table td {
              vertical-align: middle;
          }

          .report-header .header-left {
            width: 65%;
          }
          .report-header .header-right {
            width: 35%;
          }
          .report-header .information {
            width: calc(100% - 110px);
          }
          </style>
        </head>
        <body onload="window.print()">${printContents}</body>
      </html>`
      );
      popupWin.document.close();
      document.getElementById('printSectionId').className = 'hidden';
      return;
    }
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
    this.studentListSubject$.unsubscribe();
    this.studentListByTeacherSubject$.unsubscribe();
  }

}

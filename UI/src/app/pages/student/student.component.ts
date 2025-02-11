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

import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { fadeInRight400ms } from '../../../@vex/animations/fade-in-right.animation';
import { fadeInUp400ms } from '../../../@vex/animations/fade-in-up.animation';
import { stagger40ms } from '../../../@vex/animations/stagger.animation';
import icMoreVert from '@iconify/icons-ic/twotone-more-vert';
import icAdd from '@iconify/icons-ic/baseline-add';
import icSearch from '@iconify/icons-ic/search';
import icFilterList from '@iconify/icons-ic/filter-list';
import icRestore from '@iconify/icons-ic/twotone-restore';
import icDelete from '@iconify/icons-ic/twotone-delete-forever';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { SelectionModel } from '@angular/cdk/collections';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { FormControl, NgForm } from '@angular/forms';
import { debounceTime, distinctUntilChanged, skip, takeUntil } from 'rxjs/operators';
import icImpersonate from '@iconify/icons-ic/twotone-account-circle';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { RolePermissionListViewModel, RolePermissionViewModel } from '../../models/roll-based-access.model';
import { RollBasedAccessService } from '../../services/roll-based-access.service';
import { CryptoService } from '../../services/Crypto.service';
import { SearchFilter, SearchFilterAddViewModel, SearchFilterListViewModel } from '../../models/search-filter.model';
import { ModuleIdentifier } from '../../enums/module-identifier.enum';
import { SchoolCreate } from '../../enums/school-create.enum';
import { LoaderService } from '../../services/loader.service';
import { ImageCropperService } from '../../services/image-cropper.service';
import { ExcelService } from '../../services/excel.service';
import { CommonService } from '../../services/common.service';
import { StudentListModel, DeleteStudentModel } from '../../models/student.model';
import { StudentService } from '../../services/student.service';
import { ConfirmDialogComponent } from '../shared-module/confirm-dialog/confirm-dialog.component';
import { SaveFilterComponent } from './save-filter/save-filter.component';
import { DefaultValuesService } from '../../common/default-values.service';
import { DataEditInfoComponent } from '../shared-module/data-edit-info/data-edit-info.component';
import { PageRolesPermission } from '../../common/page-roles-permissions.service';
import { Permissions } from '../../models/roll-based-access.model';
import { StudentScheduleService } from '../../services/student-schedule.service';
import { ScheduleStudentListViewModel } from '../../models/student-schedule.model';
import { ProfilesTypes } from '../../enums/profiles.enum';
import { AdvancedSearchExpansionModel } from 'src/app/models/common.model';
import { SharedFunction } from '../shared/shared-function';
import { MatCheckbox } from '@angular/material/checkbox';


@Component({
  selector: 'vex-student',
  templateUrl: './student.component.html',
  styleUrls: ['./student.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class StudentComponent implements OnInit, OnDestroy {
  columns = [
    { label: 'studentCheck', property: 'selectedStudent', type: 'text', visible: true },
    { label: 'name', property: 'lastFamilyName', type: 'text', visible: true },
    { label: 'studentId', property: 'studentInternalId', type: 'text', visible: true },
    { label: 'gender', property: 'gender', type: 'text', visible: true },
    { label: 'dob', property: 'dob', type: 'text', visible: true },
    { label: 'gradeLevel', property: 'gradeLevelTitle', type: 'text', visible: true },
    { label: 'section', property: 'sectionId', type: 'text', visible: true },
    { label: 'alternateId', property: 'alternateId', type: 'text', visible: false },
    { label: 'email', property: 'schoolEmail', type: 'text', visible: false },
    { label: 'telephone', property: 'homePhone', type: 'text', visible: false },
    { label: 'schoolName', property: 'schoolName', type: 'text', visible: false },
    { label: 'status', property: 'status', type: 'text', visible: false },
    { label: 'action', property: 'action', type: 'text', visible: true },
  ];
  icImpersonate = icImpersonate;
  icRestore = icRestore;
  selection = new SelectionModel<any>(true, []);
  totalCount: number = 0;
  pageNumber: number;
  pageSize: number;
  searchCtrl: FormControl;
  icMoreVert = icMoreVert;
  icAdd = icAdd;
  icSearch = icSearch;
  icFilterList = icFilterList;
  icDelete = icDelete;
  fapluscircle = "fa-plus-circle";
  tenant = "";
  filterValue: number;
  searchFilter: SearchFilter = new SearchFilter();
  filterJsonParams;
  loading: boolean;
  staffMembershipType: string;
  showSaveFilter: boolean = false;
  allStudentList = [];
  isAdvance:boolean;
  filterParameters = [];
  destroySubject$: Subject<void> = new Subject();
  getAllStudent: StudentListModel = new StudentListModel();
  scheduleStudentListViewModel: ScheduleStudentListViewModel = new ScheduleStudentListViewModel();
  searchFilterAddViewModel: SearchFilterAddViewModel = new SearchFilterAddViewModel();
  searchFilterListViewModel: SearchFilterListViewModel = new SearchFilterListViewModel();
  deleteStudentModel: DeleteStudentModel = new DeleteStudentModel();
  StudentModelList: MatTableDataSource<any>;
  showAdvanceSearchPanel: boolean = false;
  moduleIdentifier = ModuleIdentifier;
  createMode = SchoolCreate;
  editPermission = false;
  deletePermission = false;
  addPermission = false;
  searchValue: any = null;
  toggleValues: any = null;
  searchCount: number = null;
  isFromAdvancedSearch: boolean = false;
  permissionListViewModel: RolePermissionListViewModel = new RolePermissionListViewModel();
  advancedSearchExpansionModel: AdvancedSearchExpansionModel = new AdvancedSearchExpansionModel();
  permissionGroup: RolePermissionViewModel = new RolePermissionViewModel();
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild('masterCheckBox') masterCheckBox: MatCheckbox;
  listOfStudents = [];
  selectedStudents = [];
  showLoadFilter = true;
  profiles=ProfilesTypes;
  categories = [
    {
      categoryId: 3,
      title: 'General Info'
    },
    {
      categoryId: 4,
      title: 'Enrollment Info'
    },
    {
      categoryId: 5,
      title: 'Address & Contact'
    },
    {
      categoryId: 6,
      title: 'Family Info'
    },
    {
      categoryId: 7,
      title: 'Medical Info'
    },
    {
      categoryId: 8,
      title: 'Documents'
    }
  ];
  permissions: Permissions;
  constructor(
    
    private studentService: StudentService,
    private snackbar: MatSnackBar,
    private router: Router,
    private loaderService: LoaderService,
    private imageCropperService: ImageCropperService,
    private excelService: ExcelService,
    private cryptoService: CryptoService,
    public translateService: TranslateService,
    public rollBasedAccessService: RollBasedAccessService,
    private dialog: MatDialog,
    private commonService: CommonService,
    private studentScheduleService: StudentScheduleService,
    private pageRolePermission: PageRolesPermission,
    public defaultValuesService: DefaultValuesService,
    private paginatorObj: MatPaginatorIntl,
    private commonFunction: SharedFunction
  ) {
    this.defaultValuesService.sendAllSchoolFlag(false);
    this.defaultValuesService.sendIncludeInactiveFlag(false);
    this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    //translateService.use('en');
    this.getAllStudent.filterParams = null;
    
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
    if (this.defaultValuesService.getUserMembershipType() === this.profiles.HomeroomTeacher || this.defaultValuesService.getUserMembershipType() === this.profiles.Teacher) {
      this.callAllStudentForTeacher();
    }
    else {
      this.callAllStudent();
    }
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    
  }

  ngOnInit(): void {
    this.searchCtrl = new FormControl();
    this.staffMembershipType = this.defaultValuesService.getUserMembershipType();
    this.permissions = this.pageRolePermission.checkPageRolePermission('/school/students/student-generalinfo')
    this.getAllSearchFilter();
    this.defaultValuesService.sendAllSchoolFlagSubject.subscribe(data=>{
      this.isAdvance=data;
    })
    
  }

  /* This is for get all data from the Advanced Search component and then call the API in this page 
  NOTE: We just get the filterParams Array from Search component
  */
  filterData(res) {
    this.filterParameters = res.filterParams;
    this.isFromAdvancedSearch = true;
    this.getAllStudent = new StudentListModel();
    this.scheduleStudentListViewModel = new ScheduleStudentListViewModel();
    this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.scheduleStudentListViewModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    if (res) {
      if (this.defaultValuesService.getUserMembershipType() === this.profiles.HomeroomTeacher || this.defaultValuesService.getUserMembershipType() === this.profiles.Teacher) {
        this.scheduleStudentListViewModel.filterParams = res.filterParams;
        this.scheduleStudentListViewModel.includeInactive = res.inactiveStudents;
        this.scheduleStudentListViewModel.dobStartDate = this.commonFunction.formatDateSaveWithoutTime(res.dobStartDate);
        this.scheduleStudentListViewModel.dobEndDate = this.commonFunction.formatDateSaveWithoutTime(res.dobEndDate);
        this.callAllStudentForTeacher();
        this.showSaveFilter = true;
      } else {
        this.getAllStudent.filterParams = res.filterParams;
        this.getAllStudent.includeInactive = res.inactiveStudents;
        this.getAllStudent.searchAllSchool = res.searchAllSchool;
        this.getAllStudent.dobStartDate = this.commonFunction.formatDateSaveWithoutTime(res.dobStartDate);
        this.getAllStudent.dobEndDate = this.commonFunction.formatDateSaveWithoutTime(res.dobEndDate);
        this.defaultValuesService.sendIncludeInactiveFlag(res.inactiveStudents);
        this.defaultValuesService.sendAllSchoolFlag(res.searchAllSchool);
        this.callAllStudent();
        this.showSaveFilter = true;
      }
    }
  }

  getSearchResult(res) {
    if (res.totalCount) {
      this.searchCount = res.totalCount;
      this.totalCount = res.totalCount;
    }
    else {
      this.searchCount = 0;
      this.totalCount = 0;
    }
    this.showSaveFilter = true;
    this.pageNumber = res.pageNumber;
    this.pageSize = res._pageSize;
    if (this.defaultValuesService.getUserMembershipType() === this.profiles.HomeroomTeacher || this.defaultValuesService.getUserMembershipType() === this.profiles.Teacher) {
      this.StudentModelList = new MatTableDataSource(res.scheduleStudentForView);
      this.scheduleStudentListViewModel = new ScheduleStudentListViewModel();
    }
    else{
      this.StudentModelList = new MatTableDataSource(res.studentListViews);
      this.getAllStudent = new StudentListModel();
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
  getSearchInput(event) {
    this.searchValue = event;
  }
  resetStudentList() {
    this.searchCount = null;
    this.searchValue = null;
    if (this.defaultValuesService.getUserMembershipType() === this.profiles.HomeroomTeacher || this.defaultValuesService.getUserMembershipType() === this.profiles.Teacher) {
      this.scheduleStudentListViewModel = new ScheduleStudentListViewModel();
      this.scheduleStudentListViewModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
      this.callAllStudentForTeacher();
    }
    else {
      this.getAllStudent = new StudentListModel();
      this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
      this.callAllStudent();
    }
  }

  ngAfterViewInit() {
    if (this.defaultValuesService.getUserMembershipType() === this.profiles.HomeroomTeacher || this.defaultValuesService.getUserMembershipType() === this.profiles.Teacher) {

      //  Sorting
      this.scheduleStudentListViewModel = new ScheduleStudentListViewModel();
      this.sort.sortChange.subscribe((res) => {
        this.scheduleStudentListViewModel.pageNumber = this.pageNumber
        this.scheduleStudentListViewModel.pageSize = this.pageSize;

        this.scheduleStudentListViewModel.sortingModel.sortColumn = res.active;
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
        if (res.direction == "") {
          this.scheduleStudentListViewModel.sortingModel = null;

          this.callAllStudentForTeacher();

          this.scheduleStudentListViewModel = new ScheduleStudentListViewModel();
          this.scheduleStudentListViewModel.sortingModel = null;
        } else {
          this.scheduleStudentListViewModel.sortingModel.sortDirection = res.direction;

          this.callAllStudentForTeacher();

        }
      });
      //  Searching
      this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
        if (term != '') {
          let filterParams = [
            {
              columnName: null,
              filterValue: term,
              filterOption: 3
            }
          ]
          if (this.sort.active != undefined && this.sort.direction != "") {
            this.scheduleStudentListViewModel.sortingModel.sortColumn = this.sort.active;
            this.scheduleStudentListViewModel.sortingModel.sortDirection = this.sort.direction;
          }
          Object.assign(this.scheduleStudentListViewModel, { filterParams: filterParams });
          this.scheduleStudentListViewModel.pageNumber = 1;
          this.paginator.pageIndex = 0;
          this.scheduleStudentListViewModel.pageSize = this.pageSize;

          this.callAllStudentForTeacher();

        }
        else {
          Object.assign(this.scheduleStudentListViewModel, { filterParams: null });
          this.scheduleStudentListViewModel.pageNumber = this.paginator.pageIndex + 1;
          this.scheduleStudentListViewModel.pageSize = this.pageSize;
          if (this.sort.active != undefined && this.sort.direction != "") {
            this.scheduleStudentListViewModel.sortingModel.sortColumn = this.sort.active;
            this.scheduleStudentListViewModel.sortingModel.sortDirection = this.sort.direction;
          }

          this.callAllStudentForTeacher();

        }
      })

    }
    else {
      //  Sorting
      this.getAllStudent = new StudentListModel();
      this.sort.sortChange.subscribe((res) => {
        this.getAllStudent.pageNumber = this.pageNumber
        this.getAllStudent.pageSize = this.pageSize;
        this.getAllStudent.sortingModel.sortColumn = res.active;
        if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
          let filterParams = [
            {
              columnName: null,
              filterValue: this.searchCtrl.value,
              filterOption: 3
            }
          ]
          Object.assign(this.getAllStudent, { filterParams: filterParams });
        }
        if (res.direction == "") {
          this.getAllStudent.sortingModel = null;

          this.callAllStudent();

          this.getAllStudent = new StudentListModel();
          this.getAllStudent.sortingModel = null;
        } else {
          this.getAllStudent.sortingModel.sortDirection = res.direction;

          this.callAllStudent();

        }
      });
      //  Searching
      this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
        if (term != '') {
          let filterParams = [
            {
              columnName: null,
              filterValue: term,
              filterOption: 3
            }
          ]
          if (this.sort.active != undefined && this.sort.direction != "") {
            this.getAllStudent.sortingModel.sortColumn = this.sort.active;
            this.getAllStudent.sortingModel.sortDirection = this.sort.direction;
          }
          Object.assign(this.getAllStudent, { filterParams: filterParams });
          this.getAllStudent.pageNumber = 1;
          this.paginator.pageIndex = 0;
          this.getAllStudent.pageSize = this.pageSize;

          this.callAllStudent();

        }
        else {
          Object.assign(this.getAllStudent, { filterParams: null });
          this.getAllStudent.pageNumber = this.paginator.pageIndex + 1;
          this.getAllStudent.pageSize = this.pageSize;
          if (this.sort.active != undefined && this.sort.direction != "") {
            this.getAllStudent.sortingModel.sortColumn = this.sort.active;
            this.getAllStudent.sortingModel.sortDirection = this.sort.direction;
          }

          this.callAllStudent();

        }
      })
    }
  }

  goToAdd() {
    this.router.navigate(["school/students/student-generalinfo"]);
    this.imageCropperService.enableUpload({ module: this.moduleIdentifier.STUDENT, upload: true, mode: this.createMode.ADD });
  }

  // For open the data chage history dialog
  openDataEdit(element) {
    this.dialog.open(DataEditInfoComponent, {
      width: '500px',
      data: { createdBy: element.createdBy, createdOn: element.createdOn, modifiedBy: element.updatedBy, modifiedOn: element.updatedOn }
    })
  }

  navigateToSetting() {
    //this.defaultValuesService.setPageId('Student Bulk Data Import');
    this.router.navigate(["school/tools/student-bulk-data-import"]);
  }

  saveFilter() {
    this.dialog.open(SaveFilterComponent, {
      width: '500px',
      data: null
    }).afterClosed().subscribe(data => {
      if (data === 'submited') {
        this.showSaveFilter = false;
        this.showLoadFilter = false;
        this.getAllSearchFilter();
      }
    });
  }

  getAllSearchFilter() {
    this.searchFilterListViewModel.module = 'Student';
    this.commonService.getAllSearchFilter(this.searchFilterListViewModel).subscribe((res) => {
      if (typeof (res) === 'undefined') {
        this.snackbar.open('Filter list failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.searchFilterListViewModel.searchFilterList = []
          if (!res.searchFilterList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        }
        else {
          this.searchFilterListViewModel = res;
          let filterData = this.searchFilterListViewModel.searchFilterList.filter(x => x.filterId == this.searchFilter.filterId);
          if (filterData.length > 0) {
            this.searchFilter.jsonList = filterData[0].jsonList;
          }
          if (this.filterJsonParams == null) {
            this.searchFilter = this.searchFilterListViewModel.searchFilterList[this.searchFilterListViewModel.searchFilterList.length - 1];
          }
        }
      }
    }
    );
  }

  editFilter() {
    this.showAdvanceSearchPanel = true;
    this.filterJsonParams = this.searchFilter;
    this.showSaveFilter = false;
    this.showLoadFilter = false;
  }

  deleteFilter() {

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
        title: 'Are you sure?',
        message: 'You are about to delete ' + this.searchFilter.filterName + '.'
      }
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult) {
        this.deleteFilterdata(this.searchFilter);
      }
    });
  }

  deleteFilterdata(filterData) {
    this.searchFilterAddViewModel.searchFilter = filterData;
    this.commonService.deleteSearchFilter(this.searchFilterAddViewModel).subscribe(
      (res: SearchFilterAddViewModel) => {
        if (typeof (res) === 'undefined') {
          this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
        else {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open('' + res._message, '', {
              duration: 10000
            });
          }
          else {
            this.getAllSearchFilter();
            if (this.defaultValuesService.getUserMembershipType() === this.profiles.HomeroomTeacher || this.defaultValuesService.getUserMembershipType() === this.profiles.Teacher) {
              this.scheduleStudentListViewModel.filterParams = null;
              this.callAllStudentForTeacher();
            }
            else {
              this.getAllStudent.filterParams = null;
              this.callAllStudent();
            }
            this.searchFilter = new SearchFilter();
            this.showLoadFilter = true;
          }
        }
      }
    );
  }

  searchByFilterName(filter) {
    this.searchFilter = filter;
    this.showLoadFilter = false;
    this.showSaveFilter = false;

    if (this.defaultValuesService.getUserMembershipType() === this.profiles.HomeroomTeacher || this.defaultValuesService.getUserMembershipType() === this.profiles.Teacher) {
      this.scheduleStudentListViewModel.staffId = this.defaultValuesService.getUserId();
      this.scheduleStudentListViewModel.academicYear = this.defaultValuesService.getAcademicYear();
      this.scheduleStudentListViewModel.filterParams = JSON.parse(filter.jsonList);
      this.scheduleStudentListViewModel.sortingModel = null;
      this.studentScheduleService.searchScheduledStudentForGroupDrop(this.scheduleStudentListViewModel).subscribe(data => {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          if (data.scheduleStudentForView === null) {
            this.StudentModelList = new MatTableDataSource([]);
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          } else {
            this.StudentModelList = new MatTableDataSource([]);
          }
        } else {
          this.totalCount = data.totalCount;
          this.pageNumber = data.pageNumber;
          this.pageSize = data._pageSize;
          data.scheduleStudentForView.forEach((student) => {
            student.checked = false;
            student.isCurrentSchool = student.schoolId === this.defaultValuesService.getSchoolID() ? true : false;
          });
          this.listOfStudents = data.scheduleStudentForView.map((item) => {
            this.selectedStudents.map((selectedUser) => {
              if (item.studentGuid === selectedUser.studentGuid) {
                item.checked = true;
                return item;
              }
            });
            return item;
          });
          this.masterCheckBox.checked = this.listOfStudents.every((item) => {
            return item.checked;
          });
          this.StudentModelList = new MatTableDataSource(data.scheduleStudentForView);
          this.scheduleStudentListViewModel = new ScheduleStudentListViewModel();
        }
      });
    }
    else{
      this.getAllStudent.filterParams = JSON.parse(filter.jsonList);
      this.getAllStudent.sortingModel = null;
      this.getAllStudent.pageSize = this.pageSize;
      this.studentService.GetAllStudentList(this.getAllStudent).subscribe(data => {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          if (data.studentListViews === null) {
            this.StudentModelList = new MatTableDataSource([]);
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          } else {
            this.StudentModelList = new MatTableDataSource([]);
          }
        } else {
          this.totalCount = data.totalCount;
          this.pageNumber = data.pageNumber;
          this.pageSize = data._pageSize;
          data.studentListViews.forEach((student) => {
            student.checked = false;
            student.isCurrentSchool = student.schoolId === this.defaultValuesService.getSchoolID() ? true : false;
          });
          this.listOfStudents = data.studentListViews.map((item) => {
            this.selectedStudents.map((selectedUser) => {
              if (item.studentGuid === selectedUser.studentGuid) {
                item.checked = true;
                return item;
              }
            });
            return item;
          });
          this.masterCheckBox.checked = this.listOfStudents.every((item) => {
            return item.checked;
          });
          this.StudentModelList = new MatTableDataSource(data.studentListViews);
          this.getAllStudent = new StudentListModel();
        }
      });
    }
    
  }


  viewStudentDetails(data) {
    this.imageCropperService.enableUpload({ module: this.moduleIdentifier.STUDENT, upload: true, mode: this.createMode.VIEW });
    this.studentService.setStudentId(data.studentId);
    if(data.schoolId === this.defaultValuesService.getSchoolID()) {
      this.defaultValuesService.setSchoolID(data.schoolId, true);
        this.checkPermissionAndRoute();
      } else {
      this.defaultValuesService.setSchoolID(data.schoolId, true);
        this.getPermissionForStudent();
      }
  }

  getPermissionForStudent() {
    let rolePermissionListView: RolePermissionListViewModel = new RolePermissionListViewModel();
    rolePermissionListView.permissionList = [];
    this.rollBasedAccessService.getAllRolePermission(rolePermissionListView).subscribe((res: RolePermissionListViewModel) => {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
      } else{
        this.checkPermissionAndRoute(res);
      }
    });
  }

  checkPermissionAndRoute(res?) {
    let permittedDetails = this.pageRolePermission.getPermittedSubCategories('/school/students', res);
    if (permittedDetails.length) {
      this.studentService.setCategoryId(0);
      this.studentService.setCategoryTitle(permittedDetails[0].title);
      this.router.navigate([permittedDetails[0].path], {state: {permissions: res}});
    } else {
      this.defaultValuesService.setSchoolID(undefined);
      this.snackbar.open('Student didnot have permission to view details.', '', {
        duration: 10000
      });
    }
  }

 

  // checkViewPermission(){
  //   let categoryId;
  //   let categoryName;
  //    for (const permission of this.permissionListViewModel.permissionList[2].permissionGroup.permissionCategory[0].permissionSubcategory){
  //     if(permission.rolePermission[0].canView){
  //      categoryName=permission.permissionSubcategoryName;       
  //      let index;
  //      index=this.categories.findIndex((item)=>item.title.toLowerCase()===categoryName.toLowerCase());
  //      if(index!=-1){
  //       categoryId=this.categories[index].categoryId;
  //      }
  //      break;
  //     }
  //   }
  //   if(categoryId){
  //     this.checkCurrentCategoryAndRoute(categoryId,categoryName);
  //   }
  // }

  // checkCurrentCategoryAndRoute(categoryId,categoryName) {
  //   this.studentService.setCategoryTitle(categoryName);
  //   this.studentService.setCategoryId(0);
  //   this.commonService.setModuleName('Student');
  //   if(categoryId === 3) {
  //     this.router.navigate(['/school', 'students', 'student-generalinfo']);
  //   } else if(categoryId === 4 ) {
  //     this.router.navigate(['/school', 'students', 'student-enrollmentinfo']);
  //   } else if(categoryId === 5 ) {
  //       this.router.navigate(['/school', 'students', 'student-address-contact']);
  //   } else if(categoryId === 6 ) {
  //     this.router.navigate(['/school', 'students', 'student-familyinfo']);
  //   } else if(categoryId === 7 ) {
  //     this.router.navigate(['/school', 'students', 'student-medicalinfo']);
  //   }
  //    else if(categoryId === 8 ) {
  //     this.router.navigate(['/school', 'students', 'student-comments']);
  //   } else if(categoryId === 9 ) {
  //     this.router.navigate(['/school', 'students', 'student-documents']);
  //   } else if(categoryId === 100 ) {
  //     this.router.navigate(['/school', 'students', 'student-course-schedule']);
  //   }  else if(categoryId === 101 ) {
  //     this.router.navigate(['/school', 'students', 'student-attendance']);
  //   }  else if(categoryId === 102 ) {
  //     this.router.navigate(['/school', 'students', 'student-transcript']);
  //   }  else if(categoryId === 103 ) {
  //     this.router.navigate(['/school', 'students', 'student-report-card']);
  //   }
  //   else if(categoryId > 9 ) {
  //     this.router.navigate(['/school', 'students', 'custom', categoryName.trim().toLowerCase().split(' ').join('-')]);
  //   }

  // }

  getPageEvent(event) {
    if(this.defaultValuesService.getUserMembershipType() === this.profiles.HomeroomTeacher || this.defaultValuesService.getUserMembershipType() === this.profiles.Teacher){
      if (this.sort.active != undefined && this.sort.direction != "") {
        this.scheduleStudentListViewModel.sortingModel.sortColumn = this.sort.active;
        this.scheduleStudentListViewModel.sortingModel.sortDirection = this.sort.direction;
      }
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
      this.callAllStudentForTeacher();
    }
    else{
      if (this.sort.active != undefined && this.sort.direction != "") {
        this.getAllStudent.sortingModel.sortColumn = this.sort.active;
        this.getAllStudent.sortingModel.sortDirection = this.sort.direction;
      }
      if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
        let filterParams = [
          {
            columnName: null,
            filterValue: this.searchCtrl.value,
            filterOption: 3
          }
        ]
        Object.assign(this.getAllStudent, { filterParams: filterParams });
      }
      this.getAllStudent.pageNumber = event.pageIndex + 1;
      this.getAllStudent.pageSize = event.pageSize;
      this.getAllStudent.filterParams = this.filterParameters;
      this.defaultValuesService.setPageSize(event.pageSize);
      this.callAllStudent();
    }
  }

  callAllStudent() {
    if (this.getAllStudent.sortingModel?.sortColumn == "") {
      this.getAllStudent.sortingModel = null;
    }
    this.studentService.GetAllStudentList(this.getAllStudent).subscribe(data => {
     if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
        if (data.studentListViews === null) {
          this.totalCount = this.isFromAdvancedSearch ? 0 : null;
          this.searchCount = this.isFromAdvancedSearch ? 0 : null;
          this.StudentModelList = new MatTableDataSource([]);
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
          this.isFromAdvancedSearch = false;
        } else {
          this.StudentModelList = new MatTableDataSource([]);
          this.totalCount = this.isFromAdvancedSearch ? 0 : null;
          this.searchCount = this.isFromAdvancedSearch ? 0 : null;
          this.isFromAdvancedSearch = false;
        }
      } else {
        this.totalCount = data.totalCount;
        this.searchCount = data.totalCount;
        this.pageNumber = data.pageNumber;
        this.pageSize = data._pageSize;
        data.studentListViews.forEach((student) => {
          student.checked = false;
          student.isCurrentSchool = student.schoolId === this.defaultValuesService.getSchoolID() ? true : false;
        });
        this.listOfStudents = data.studentListViews.map((item) => {
          this.selectedStudents.map((selectedUser) => {
            if (item.studentGuid === selectedUser.studentGuid) {
              item.checked = true;
              return item;
            }
          });
          return item;
        });
        this.masterCheckBox.checked = this.listOfStudents.every((item) => {
          return item.checked;
        });
        this.StudentModelList = new MatTableDataSource(data.studentListViews);
        this.getAllStudent = new StudentListModel();
        this.isFromAdvancedSearch = false;
      }
    });
  }


  callAllStudentForTeacher() {
    this.scheduleStudentListViewModel.staffId = this.defaultValuesService.getUserId();
    this.scheduleStudentListViewModel.academicYear = this.defaultValuesService.getAcademicYear();
    if (this.scheduleStudentListViewModel.sortingModel?.sortColumn == "") {
      this.scheduleStudentListViewModel.sortingModel = null;
    }
    this.studentScheduleService.searchScheduledStudentForGroupDrop(this.scheduleStudentListViewModel).subscribe(data => {
     if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
        if (data.scheduleStudentForView === null) {
          this.totalCount = this.isFromAdvancedSearch ? 0 : null;
          this.searchCount = this.isFromAdvancedSearch ? 0 : null;
          this.StudentModelList = new MatTableDataSource([]);
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
          this.isFromAdvancedSearch = false;
        } else {
          this.StudentModelList = new MatTableDataSource([]);
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
          student.isCurrentSchool = student.schoolId === this.defaultValuesService.getSchoolID() ? true : false;
        });
        this.listOfStudents = data.scheduleStudentForView.map((item) => {
          this.selectedStudents.map((selectedUser) => {
            if (item.studentGuid === selectedUser.studentGuid) {
              item.checked = true;
              return item;
            }
          });
          return item;
        });
        this.masterCheckBox.checked = this.listOfStudents.every((item) => {
          return item.checked;
        });
        this.StudentModelList = new MatTableDataSource(data.scheduleStudentForView);
        this.scheduleStudentListViewModel = new ScheduleStudentListViewModel();
        this.isFromAdvancedSearch = false;
      }
    });
  }

  exportStudentListToExcel() {
    if (this.defaultValuesService.getUserMembershipType() === this.profiles.HomeroomTeacher || this.defaultValuesService.getUserMembershipType() === this.profiles.Teacher) {
      const scheduleStudentListViewModel: ScheduleStudentListViewModel = new ScheduleStudentListViewModel();
      scheduleStudentListViewModel.pageNumber = 0;
      scheduleStudentListViewModel.pageSize = 0;
      scheduleStudentListViewModel.sortingModel = null;
      scheduleStudentListViewModel.staffId = this.defaultValuesService.getUserId();
      scheduleStudentListViewModel.academicYear = this.defaultValuesService.getAcademicYear();
      this.studentScheduleService.searchScheduledStudentForGroupDrop(scheduleStudentListViewModel).subscribe(res => {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open('Failed to Export Student List.' + res._message, '', {
            duration: 10000
          });
        } else {
          if (res.scheduleStudentForView.length > 0) {
            let studentList;
            studentList = res.scheduleStudentForView?.map((x) => {
              const middleName = x.middleName == null ? ' ' : ' ' + x.middleName + ' ';
              return {
                [this.defaultValuesService.translateKey('name')]: x.firstGivenName + middleName + x.lastFamilyName,
                [this.defaultValuesService.translateKey('studentID')]: x.studentInternalId,
                [this.defaultValuesService.translateKey('alternateID')]: x.alternateId,
                [this.defaultValuesService.translateKey('gradeLevel')]: x.gradeLevel,
                [this.defaultValuesService.translateKey('email')]: x.schoolEmail,
                [this.defaultValuesService.translateKey('telephone')]: x.homePhone
              };
            });
            this.excelService.exportAsExcelFile(studentList, 'Students_List_')
          } else {
            this.snackbar.open('No Records Found. Failed to Export Students List', '', {
              duration: 5000
            });
          }
        }
      });
   }
   else{
    const getAllStudent: StudentListModel = new StudentListModel();
    getAllStudent.pageNumber = 0;
    getAllStudent.pageSize = 0;
    getAllStudent.sortingModel = null;
    this.studentService.GetAllStudentList(getAllStudent).subscribe(res => {
    if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        this.snackbar.open('Failed to Export Student List.' + res._message, '', {
          duration: 10000
        });
      } else {
        if (res.studentListViews.length > 0) {
          let studentList;
          studentList = res.studentListViews?.map((x) => {
            const middleName = x.middleName == null ? ' ' : ' ' + x.middleName + ' ';
            return {
              [this.defaultValuesService.translateKey('name')]: x.firstGivenName + middleName + x.lastFamilyName,
              [this.defaultValuesService.translateKey('studentID')]: x.studentInternalId,
              [this.defaultValuesService.translateKey('alternateID')]: x.alternateId,
              [this.defaultValuesService.translateKey('gradeLevel')]: x.gradeLevelTitle,
              [this.defaultValuesService.translateKey('email')]: x.schoolEmail,
              [this.defaultValuesService.translateKey('telephone')]: x.homePhone,
              [this.defaultValuesService.translateKey('schoolName')]: x.schoolName
            };
          });
          this.excelService.exportAsExcelFile(studentList, 'Students_List_')
        } else {
          this.snackbar.open('No Records Found. Failed to Export Students List', '', {
            duration: 5000
          });
        }
      }
    });
   }
  }

  showAdvanceSearch() {
    this.showAdvanceSearchPanel = true;
    this.filterJsonParams = null;
  }

  hideAdvanceSearch(event) {
    this.showSaveFilter = event.showSaveFilter;
    this.showAdvanceSearchPanel = false;
    if (event.showSaveFilter == false) {
      this.getAllSearchFilter();
    }
  }


  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  onFilterChange(value: string) {
    if (!this.StudentModelList) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    this.StudentModelList.filter = value;
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  deleteSelectedStudents() {
    if(this.selectedStudents?.length < 1){
      this.snackbar.open('Select students to delete', '', {
        duration: 5000
      });
    } else {
      const dialogRef = this.dialog.open(ConfirmDialogComponent, {
        maxWidth: '400px',
        data: {
          title: this.defaultValuesService.translateKey('areYouSure'),
          message: this.defaultValuesService.translateKey('youWantToDeleteSelectedStudents') + '.'
        }
      });
      dialogRef.afterClosed().subscribe(dialogResult => {
        if (dialogResult) {
          this.loading = true;
          this.deleteStudentModel.studentListViews = this.selectedStudents;
          this.studentService.DeleteStudent(this.deleteStudentModel).subscribe((res) => {
            if(res._failure){
              this.snackbar.open(res._message, '', {
                duration: 5000
              });
              this.loading = false;
            } else {
              this.snackbar.open(res._message, '', {
                duration: 5000
              });
              this.loading = false;
              this.getAllStudent.pageNumber = 1;
              this.getAllStudent.pageSize = this.pageSize;
              if (this.sort.active != undefined && this.sort.direction != "") {
                this.getAllStudent.sortingModel.sortColumn = this.sort.active;
                this.getAllStudent.sortingModel.sortDirection = this.sort.direction;
              } else {
                this.getAllStudent.sortingModel = null;
              }
              if(this.filterParameters){
                this.scheduleStudentListViewModel.filterParams = this.filterParameters;
              } else {
                this.scheduleStudentListViewModel.filterParams = null;
              }
              this.callAllStudent();
            }
          })
        }
      });
    }
  }

  someComplete(): boolean {
    let indetermine = false;
    for (let user of this.listOfStudents) {
      for (let selectedUser of this.selectedStudents) {
        if (user.studentGuid === selectedUser.studentGuid) {
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
    this.StudentModelList = new MatTableDataSource(this.listOfStudents);
    this.decideCheckUncheck();
  }

  onChangeSelection(eventStatus: boolean, studentGuid) {
    for (let item of this.listOfStudents) {
      if (item.studentGuid === studentGuid) {
        item.checked = eventStatus;
        break;
      }
    }
    this.StudentModelList = new MatTableDataSource(this.listOfStudents);
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
          if (item.studentGuid === selectedUser.studentGuid) {
            isIdIncludesInSelectedList = true;
            break;
          }
        }
        if (!isIdIncludesInSelectedList) {
          if (item.isCurrentSchool && item.isActive) this.selectedStudents.push(item);
        }
      } else {
        for (let selectedUser of this.selectedStudents) {
          if (item.studentGuid === selectedUser.studentGuid) {
            this.selectedStudents = this.selectedStudents.filter((user) => user.studentGuid !== item.studentGuid);
            break;
          }
        }
      }
      isIdIncludesInSelectedList = false;
    });
    this.selectedStudents = this.selectedStudents.filter((item) => item.checked);    
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
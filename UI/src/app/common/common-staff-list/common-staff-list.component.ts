import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { SearchFilter, SearchFilterAddViewModel, SearchFilterListViewModel } from 'src/app/models/search-filter.model';
import { GetAllStaffModel, StaffAddModel, StaffMasterModel, DeleteStaffModel } from 'src/app/models/staff.model';
import { FinalGradeService } from 'src/app/services/final-grade.service';
import { LoaderService } from 'src/app/services/loader.service';
import { StaffService } from 'src/app/services/staff.service';
import { DefaultValuesService } from '../default-values.service';
import { PageRolesPermission } from '../page-roles-permissions.service';
import { Permissions } from '../../models/roll-based-access.model';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { fadeInRight400ms } from 'src/@vex/animations/fade-in-right.animation';
import { CommonService } from 'src/app/services/common.service';
import moment from 'moment';
import icAdd from '@iconify/icons-ic/baseline-add';
import icFilterList from '@iconify/icons-ic/filter-list';
import icDelete from '@iconify/icons-ic/twotone-delete-forever';
import { ConfirmDialogComponent } from 'src/app/pages/shared-module/confirm-dialog/confirm-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { ImageCropperService } from 'src/app/services/image-cropper.service';
import { ModuleIdentifier } from '../../enums/module-identifier.enum';
import { SchoolCreate } from '../../enums/school-create.enum';
import { SaveFilterComponent } from 'src/app/pages/staff/staffinfo/save-filter/save-filter.component';
import { ExcelService } from 'src/app/services/excel.service';
import icRestore from '@iconify/icons-ic/twotone-restore';
import icImpersonate from '@iconify/icons-ic/twotone-account-circle';
import { ImpersonateServices } from 'src/app/services/impersonate.service';
import { DataEditInfoComponent } from 'src/app/pages/shared-module/data-edit-info/data-edit-info.component';
import { ProfilesTypes } from 'src/app/enums/profiles.enum';
import { SharedFunction } from 'src/app/pages/shared/shared-function';
import { StudentAttendanceService } from 'src/app/services/student-attendance.service';

@Component({
  selector: 'vex-common-staff-list',
  templateUrl: './common-staff-list.component.html',
  styleUrls: ['./common-staff-list.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class CommonStaffListComponent implements OnInit {
  @Input() parentComponent;
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort
  getAllStaff: GetAllStaffModel = new GetAllStaffModel();
  staffList: MatTableDataSource<any>;
  totalCount: number = 0;
  pageNumber: number;
  pageSize: number;
  permissions: Permissions;
  loading: boolean;
  pageInit = 1;
  searchCtrl: FormControl;
  icAdd = icAdd;
  searchFilterAddViewModel: SearchFilterAddViewModel = new SearchFilterAddViewModel();
  searchFilterListViewModel: SearchFilterListViewModel = new SearchFilterListViewModel();
  deleteStaffModel: DeleteStaffModel = new DeleteStaffModel();
  searchFilter: SearchFilter = new SearchFilter();
  displayedColumns: string[] = ['lastFamilyName', 'staffInternalId', 'profile', 'jobTitle', 'schoolEmail', 'mobilePhone', 'status'];
  icFilterList = icFilterList;
  icDelete = icDelete;
  showAdvanceSearchPanel: boolean = false;
  showSaveFilter: boolean = false;
  filterJsonParams;
  filterParameters = [];
  showLoadFilter = true;
  toggleValues: any = null;
  moduleIdentifier = ModuleIdentifier;
  createMode = SchoolCreate;
  inactiveStaff = false;
  columns = [
    { label: 'name', property: 'lastFamilyName', type: 'text', visible: true },
    { label: 'staffId', property: 'staffInternalId', type: 'text', visible: true },
    { label: 'profile', property: 'profile', type: 'text', visible: true },
    { label: 'jobTitle', property: 'jobTitle', type: 'text', visible: true },
    { label: 'schoolEmail', property: 'schoolEmail', type: 'text', visible: true },
    { label: 'mobilePhone', property: 'mobilePhone', type: 'number', visible: true },
    { label: 'schoolName', property: 'schoolName', type: 'text', visible: false },
    { label: 'status', property: 'status', type: 'text', visible: false },
    { label: 'actions', property: 'actions', type: 'text', visible: true }
  ];
  icImpersonate = icImpersonate;
  icRestore = icRestore;
  staffAddModel: StaffAddModel = new StaffAddModel();
  profile = [
    'School Administrator',
    'Admin Assistant',
    'Teacher',
    'Homeroom Teacher'
  ];
  profiles = ProfilesTypes;
  searchCount: number = null;
  searchValue: any = null;
  isFromAdvancedSearch: boolean = false;

  constructor(
    private staffService: StaffService,
    private snackbar: MatSnackBar,
    private finalGradeService: FinalGradeService,
    private router: Router,
    private loaderService: LoaderService,
    private pageRolePermissions: PageRolesPermission,
    private paginatorObj: MatPaginatorIntl,
    public translateService: TranslateService,
    public defaultValuesService: DefaultValuesService,
    private commonService: CommonService,
    private dialog: MatDialog,
    private imageCropperService: ImageCropperService,
    private excelService: ExcelService,
    private impersonateServices: ImpersonateServices,
    private commonFunction: SharedFunction,
    private studentAttendanceService:StudentAttendanceService
  ) {
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');

    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.getAllStaff.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.columns.map((x, index) => {
      if (this.parentComponent === 'takeAttendence') {
        if (x.label === "actions")
          this.columns.splice(index, 1)
        if (x.label === "status")
          this.columns[index].visible = true
      }
    })

    this.callStaffList();
    this.searchCtrl = new FormControl();
    this.permissions = this.pageRolePermissions.checkPageRolePermission()
    if (this.parentComponent === "staffList")
      this.getAllSearchFilter();

  }

  ngAfterViewInit() {
    //  Sorting
    this.getAllStaff = new GetAllStaffModel();
    this.sort.sortChange.subscribe((res) => {
      this.getAllStaff.pageNumber = this.pageNumber
      this.getAllStaff.pageSize = this.pageSize;
      this.getAllStaff.sortingModel.sortColumn = res.active;
      if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
        let filterParams = [
          {
            columnName: null,
            filterValue: this.searchCtrl.value,
            filterOption: 4
          }
        ]
        Object.assign(this.getAllStaff, { filterParams: filterParams });
      }
      if (res.direction == "") {
        this.getAllStaff.sortingModel = null;
        this.callStaffList();
        this.getAllStaff = new GetAllStaffModel();
        this.getAllStaff.sortingModel = null;
      } else {
        this.getAllStaff.sortingModel.sortDirection = res.direction;
        this.callStaffList();
      }
    });

    //  Searching
    this.searchCtrl?.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term != '') {
        this.callWithFilterValue(term);
      } else {
        this.callWithoutFilterValue()
      }
    });
  }

  callStaffList() {
    if (this.getAllStaff.sortingModel?.sortColumn == "") {
      this.getAllStaff.sortingModel = null
    }
    if (this.parentComponent === "inputEffortGrade")
      this.getAllStaff.isHomeRoomTeacher = true;
    this.staffService.getAllStaffList(this.getAllStaff).subscribe(res => {
      if (res._failure) {

        this.staffList = new MatTableDataSource([]);
        this.totalCount = this.isFromAdvancedSearch ? 0 : null;
        this.searchCount = this.isFromAdvancedSearch ? 0 : null;
        if (!res.staffMaster) {
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        }
        this.isFromAdvancedSearch = false;
      } else {
        this.totalCount = res.totalCount;
        this.searchCount = res.totalCount;
        this.pageNumber = res.pageNumber;
        this.pageSize = res._pageSize;
        this.staffList = new MatTableDataSource(res.staffMaster);
        if (this.getAllStaff.searchAllSchool === true) {
          for (let staff of this.staffList.filteredData) {
            for (let schoolList of staff.staffSchoolInfo) {
              staff.schoolName = schoolList.schoolAttachedName;
              if (schoolList.endDate === null || moment(new Date()).isBetween(schoolList.startDate, schoolList.endDate)) {
                staff.status = 'active';
                break;
              } else
                staff.status = 'inactive';
            }
          }
        }
        else {
          for (let staff of this.staffList.filteredData) {
            staff.staffSchoolInfo.map(schoolList => {
              if (this.defaultValuesService.getSchoolID() == schoolList.schoolAttachedId) {
                staff.schoolName = schoolList.schoolAttachedName;
                staff.status = this.checkStaffActiveOrInactive(schoolList.startDate,schoolList.endDate)
              }
            })
          }
        }
        this.getAllStaff = new GetAllStaffModel();
        this.isFromAdvancedSearch = false;
      }
    });
  }

  checkStaffActiveOrInactive(startDate, endDate) {
    if (!endDate) {
      if (moment(this.commonFunction.formatDateSaveWithoutTime(new Date())).isSameOrAfter(this.commonFunction.formatDateSaveWithoutTime(startDate)))
        return 'active';
      else
        return 'inactive';
    } else {
      if (moment(this.commonFunction.formatDateSaveWithoutTime(new Date())).isSameOrAfter(this.commonFunction.formatDateSaveWithoutTime(startDate)) &&
        moment(this.commonFunction.formatDateSaveWithoutTime(new Date())).isSameOrBefore(this.commonFunction.formatDateSaveWithoutTime(endDate)))
        return 'active';
      else
        return 'inactive';
    }
  }
  
  viewStaffDetails(element, checkFlag?) {
    if (checkFlag) {
      this.imageCropperService.enableUpload({ module: this.moduleIdentifier.STAFF, upload: true, mode: this.createMode.VIEW });
      this.staffService.setStaffId(element.staffId);
      this.getPermissionForStaff();
    } else {
      if (element.status === "active") {
        if (!this.permissions?.edit) return;
        const staffFullName = `${element.firstGivenName} ${element.middleName ? element.middleName + ' ' : ''}${element.lastFamilyName}`;
        if(this.parentComponent === "inputFinalGrade" || this.parentComponent === "inputEffortGrade" || this.parentComponent === "gradebookGrades")
          this.finalGradeService.setStaffDetails({ staffId: element.staffId, staffFullName });
        if (this.parentComponent === "inputFinalGrade")
          this.router.navigate(['/school', 'staff', 'teacher-functions', 'input-final-grade', 'final-grade-details']);
        else if (this.parentComponent === "inputEffortGrade")
          this.router.navigate(['/school', 'staff', 'teacher-functions', 'input-effort-grade', 'effort-grade-details']);
        else if (this.parentComponent === "gradebookGrades")
          this.router.navigate(['/school', 'staff', 'teacher-functions', 'gradebook-grades', 'gradebook-grade-list']);
        else if (this.parentComponent === "takeAttendence"){
          this.studentAttendanceService.setStaffDetails({staffId: element.staffId, staffFullName});
          this.router.navigate(['/school', 'staff', 'teacher-functions', 'take-attendance', 'course-section']);
        }
        this.pageInit = 2;
      }
      else
        this.snackbar.open('Inactive staff does not have any access to enter the next page', '', {
          duration: 10000
        })
    }
  }

  getPermissionForStaff() {
    let permittedDetails = this.pageRolePermissions.getPermittedSubCategories('/school/staff');
    if (permittedDetails.length) {
      this.staffService.setCategoryId(0);
      this.staffService.setCategoryTitle(permittedDetails[0].title);
      this.router.navigateByUrl(permittedDetails[0].path, { state: { type: SchoolCreate.VIEW } });
    } else {
      this.defaultValuesService.setSchoolID(undefined);
      this.snackbar.open('Saff didnot have permission to view details.', '', {
        duration: 10000
      });
    }
  }

  getPageEvent(event) {
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.getAllStaff.sortingModel.sortColumn = this.sort.active;
      this.getAllStaff.sortingModel.sortDirection = this.sort.direction;
    }
    if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
      let filterParams = [
        {
          columnName: null,
          filterValue: this.searchCtrl.value,
          filterOption: 3
        }
      ]
      Object.assign(this.getAllStaff, { filterParams: filterParams });
    }
    this.getAllStaff.pageNumber = event.pageIndex + 1;
    this.getAllStaff.pageSize = event.pageSize;
    this.getAllStaff.filterParams = this.filterParameters;
    this.callStaffList();
  }

  callWithFilterValue(term) {
    let filterParams = [
      {
        columnName: null,
        filterValue: term,
        filterOption: 4
      }
    ]
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.getAllStaff.sortingModel.sortColumn = this.sort.active;
      this.getAllStaff.sortingModel.sortDirection = this.sort.direction;
    }
    Object.assign(this.getAllStaff, { filterParams: filterParams });
    this.getAllStaff.pageNumber = 1;
    this.paginator.pageIndex = 0;
    this.getAllStaff.pageSize = this.pageSize;
    this.callStaffList();
  }

  callWithoutFilterValue() {
    Object.assign(this.getAllStaff, { filterParams: null });
    this.getAllStaff.pageNumber = this.paginator.pageIndex + 1;
    this.getAllStaff.pageSize = this.pageSize;
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.getAllStaff.sortingModel.sortColumn = this.sort.active;
      this.getAllStaff.sortingModel.sortDirection = this.sort.direction;
    }
    this.callStaffList();
  }

  includeInactiveStaff(event) {
    if (event.checked) {
      this.inactiveStaff = true;
      this.defaultValuesService.sendIncludeInactiveFlag(true);
      this.callStaffList();
    }
    else {
      this.inactiveStaff = false;
      this.defaultValuesService.sendIncludeInactiveFlag(false);
      this.callStaffList();
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
          if (res._failure) {

            this.snackbar.open('' + res._message, '', {
              duration: 10000
            });
          }
          else {
            this.searchFilter = new SearchFilter();
            this.showLoadFilter = true;
            this.getAllStaff.filterParams = null;
            this.getAllSearchFilter();
            this.callStaffList();
          }
        }
      }
    );
  }

  getAllSearchFilter() {
    this.searchFilterListViewModel.module = 'Staff';
    this.commonService.getAllSearchFilter(this.searchFilterListViewModel).subscribe((res) => {
      if (typeof (res) === 'undefined') {
        this.snackbar.open('Filter list failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
        if (res._failure) {

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

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  searchByFilterName(filter) {
    this.searchFilter = filter;
    this.showLoadFilter = false;
    this.showSaveFilter = false;
    this.getAllStaff.filterParams = JSON.parse(filter.jsonList);
    this.getAllStaff.sortingModel = null;
    this.staffService.getAllStaffList(this.getAllStaff).subscribe(data => {
      if (data._failure) {

        if (data.staffMaster === null) {
          this.staffList = new MatTableDataSource([]);
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        } else {
          this.staffList = new MatTableDataSource([]);
        }
      } else {
        this.totalCount = data.totalCount;
        this.pageNumber = data.pageNumber;
        this.pageSize = data._pageSize;
        this.staffList = new MatTableDataSource(data.staffMaster);
        this.getAllStaff = new GetAllStaffModel();
      }
    });
  }

  navigateToSetting() {
    this.defaultValuesService.setPageId('Staff Bulk Data Import');
    this.router.navigate(["school/tools/staff-bulk-data-import"]);
  }

  goToAdd() {
    this.staffService.setStaffId(null);
    this.router.navigate(['/school', 'staff', 'staff-generalinfo']);
    this.imageCropperService.enableUpload({ module: this.moduleIdentifier.STAFF, upload: true, mode: this.createMode.ADD });

  }

  openSaveFilter() {
    this.dialog.open(SaveFilterComponent, {
      width: '500px',
      data: null
    }).afterClosed().subscribe(res => {
      if (res) {
        this.showSaveFilter = false;
        this.showLoadFilter = false;
        this.getAllSearchFilter();
      }
    });
  }

  exportStaffListToExcel() {
    let getAllStaff: GetAllStaffModel = new GetAllStaffModel();
    getAllStaff.pageNumber = 0;
    getAllStaff.pageSize = 0;
    getAllStaff.sortingModel = null;
    this.staffService.getAllStaffList(getAllStaff).subscribe(res => {
      if (res._failure) {

        this.snackbar.open('Failed to Export Staff List.' + res._message, '', {
          duration: 10000
        });
      } else {
        if (res.staffMaster.length > 0) {
          let staffList = res.staffMaster?.map((x: StaffMasterModel) => {
            let middleName = x.middleName == null ? ' ' : ' ' + x.middleName + ' ';
            return {
              Name: x.firstGivenName + middleName + x.lastFamilyName,
              'Staff ID': x.staffInternalId,
              Profile: x.profile,
              'Job Title': x.jobTitle,
              'School Email': x.schoolEmail,
              'Mobile Phone': x.mobilePhone
            }
          });

          this.excelService.exportAsExcelFile(staffList, 'Staffs_List_')
        } else {
          this.snackbar.open('No Records Found. Failed to Export Staff List', '', {
            duration: 5000
          });
        }
      }
    });
  }

  impersonateAsStaff(staffId: any) {
    this.impersonateServices.impersonateStoreData();
    /** call viwe staff api for getting basic staff details */
    this.staffAddModel.staffMaster.staffId = staffId;
    this.staffService.viewStaff(this.staffAddModel).subscribe((res: any) => {
      if (res._failure) {

      } else {
        this.defaultValuesService.setUserGuidId(res.staffMaster.staffGuid);
        this.defaultValuesService.setUserPhoto(res.staffMaster.staffPhoto);
        this.defaultValuesService.setUserId(res.staffMaster.staffId);
        this.profile.map((value, index) => {
          if (value == res.staffMaster.profile)
            this.defaultValuesService.setUserMembershipID((index + 2).toString());
        })
        this.defaultValuesService.setUserMembershipType(res.staffMaster.profile);
        let middleName = res.staffMaster.middleName ? ' ' + res.staffMaster.middleName + ' ' : ' ';
        let fullName = res.staffMaster.firstGivenName + middleName + res.staffMaster.lastFamilyName;
        this.defaultValuesService.setFullUserName(fullName);
        this.defaultValuesService.setEmailId(res.staffMaster.loginEmailAddress);
        this.defaultValuesService.setuserMembershipName(res.staffMaster.profile);
        this.impersonateServices.callRolePermissions(true);
      }
    });
  }

  openDataEdit(element) {
    this.dialog.open(DataEditInfoComponent, {
      width: '500px',
      data: { createdBy: element.createdBy, createdOn: element.createdOn, modifiedBy: element.updatedBy, modifiedOn: element.updatedOn }
    })
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  resetStaffList() {
    this.searchCount = null;
    this.searchValue = null;
    this.getAllStaff = new GetAllStaffModel();
    this.getAllStaff.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.callStaffList();
  }

  getToggleValues(event) {
    this.toggleValues = event;
    if(this.parentComponent !== "takeAttendence"){
      if (event.inactiveStaff === true)
        this.columns[7].visible = true;
      else
        this.columns[7].visible = false;
    }

    if (event.searchAllSchool === true)
      this.columns[6].visible = true;
    else
      this.columns[6].visible = false;
  }

  getSearchInput(event) {
    this.searchValue = event;
    // this.columns[6].visible = event.inactiveStaff;
  }

  /* This is for get all data from the Advanced Search component and then call the API in this page 
  NOTE: We just get the filterParams Array from Search component
  */
  filterData(res) {
    this.filterParameters = res.filterParams;
    this.isFromAdvancedSearch = true;
    this.getAllStaff = new GetAllStaffModel();
    this.getAllStaff.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    if (res) {
      this.getAllStaff.filterParams = res.filterParams;
      this.getAllStaff.includeInactive = res.inactiveStaff;
      this.getAllStaff.searchAllSchool = res.searchAllSchool;
      this.getAllStaff.dobStartDate = this.commonFunction.formatDateSaveWithoutTime(res.dobStartDate);
      this.getAllStaff.dobEndDate = this.commonFunction.formatDateSaveWithoutTime(res.dobEndDate);
      this.defaultValuesService.sendIncludeInactiveFlag(res.inactiveStaff);
      this.defaultValuesService.sendAllSchoolFlag(res.searchAllSchool);
      this.callStaffList();
      this.showSaveFilter = true;
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
    this.staffList = new MatTableDataSource(res.staffMaster);
    this.getAllStaff = new GetAllStaffModel();
  }

  deleteStaff(item, index) {
    /*if(this.defaultValuesService.getUserMembershipType() === this.profiles.SuperAdmin){
      this.confirmDelete(item, index);
    } else {
      this.checkIFExternalSchoolWithoutCategory(item).then((res:any) => {
        this.confirmDelete(item, index);
      });
    }*/
      this.checkIFExternalSchoolWithoutCategory(item).then((res:any) => {
        this.confirmDelete(item, index);
      });
  }

  confirmDelete(item, index) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
        title: this.defaultValuesService.translateKey('areYouSure'),
        message: this.defaultValuesService.translateKey('youWantToDeleteStaff') + ' ' + item.firstGivenName + ' ' + item.lastFamilyName
      }
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult) {
        this.staffList.filteredData[index].loading = true;
        this.deleteStaffModel.staffMasters = [{...item}];
        this.staffService.deleteStaff(this.deleteStaffModel).subscribe((res) => {
          if(res._failure){
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          } else {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
            // set pagination
            this.getAllStaff.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
            this.getAllStaff.pageNumber = 1;

            // set search filterparams
            if(this.filterParameters){
              this.getAllStaff.filterParams = this.filterParameters;
            }

            // set search filter
            if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
              let filterParams = [
                {
                  columnName: null,
                  filterValue: this.searchCtrl.value,
                  filterOption: 3
                }
              ]
              Object.assign(this.getAllStaff, { filterParams: filterParams });
            }

            // set sorting (if any)
            if (this.sort.active !== undefined && this.sort.direction !== "") {
              this.getAllStaff.sortingModel.sortColumn = this.sort.active;
              this.getAllStaff.sortingModel.sortDirection = this.sort.direction;
            }
            this.callStaffList();
          }
        })
      }
    });
  }

  checkIFExternalSchoolWithoutCategory(model) {
    return new Promise((resolve, reject) => {
      let defaultSchool = model.staffSchoolInfo.find(x => x.schoolId === x.schoolAttachedId);
      let enxternalSchools = model.staffSchoolInfo.filter(x => x.schoolId !== x.schoolAttachedId);
      if (enxternalSchools?.length > 0) {
        const index = enxternalSchools.findIndex(x => x.schoolAttachedId === this.defaultValuesService.getSchoolID())
        if (index >= 0) {
          this.snackbar.open(
            `This staff is associated to ${defaultSchool.schoolAttachedName}. Please go to ${defaultSchool.schoolAttachedName} for any changes`, '', 
            { duration: 10000 }
          );
        } else {
          resolve('');
        }
      } else {
        resolve('');
      }
    });
  }

  ngOnDestroy(){
    // this.getAllStaff.includeInactive=false;
    // this.getAllStaff.searchAllSchool=false;
    this.defaultValuesService.sendIncludeInactiveFlag(false);
    this.defaultValuesService.sendAllSchoolFlag(false);
  }
}


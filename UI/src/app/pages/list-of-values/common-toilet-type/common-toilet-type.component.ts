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

import { Component, OnInit, Input,ViewChild } from '@angular/core';
import icMoreVert from '@iconify/icons-ic/twotone-more-vert';
import icAdd from '@iconify/icons-ic/baseline-add';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icSearch from '@iconify/icons-ic/search';
import icFilterList from '@iconify/icons-ic/filter-list';
import icImpersonate from '@iconify/icons-ic/twotone-account-circle';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router} from '@angular/router';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { stagger40ms } from '../../../../@vex/animations/stagger.animation';
import { TranslateService } from '@ngx-translate/core';
import { EditCommonToiletTypeComponent } from './edit-common-toilet-type/edit-common-toilet-type.component';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { LoaderService } from './../../../services/loader.service';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import { LovList, LovAddView, LoVSortOrderValuesModel, UpdateLovSortingModel } from '../../../models/lov.model';
import { ExcelService } from '../../../services/excel.service';
import { SharedFunction } from '../../shared/shared-function';
import { CommonService } from './../../../services/common.service';
import { RolePermissionListViewModel, RolePermissionViewModel } from 'src/app/models/roll-based-access.model';
import { CryptoService } from '../../../services/Crypto.service';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { Permissions } from '../../../models/roll-based-access.model';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { CdkDragDrop } from '@angular/cdk/drag-drop';

@Component({
  selector: 'vex-common-toilet-type',
  templateUrl: './common-toilet-type.component.html',
  styleUrls: ['./common-toilet-type.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class CommonToiletTypeComponent implements OnInit {
  columns = [
    { label: 'sort', property: 'lovId', type: 'text', visible: true },
    { label: 'title', property: 'lovColumnValue', type: 'text', visible: true },
    { label: 'createdBy', property: 'createdBy', type: 'text', visible: true },
    { label: 'createDate', property: 'createdOn', type: 'text', visible: true },
    { label: 'updatedBy', property: 'updatedBy', type: 'text', visible: true },
    { label: 'updateDate', property: 'updatedOn', type: 'text', visible: true },
    { label: 'actions', property: 'actions', type: 'text', visible: true }
  ];


  icMoreVert = icMoreVert;
  icAdd = icAdd;
  icEdit = icEdit;
  icDelete = icDelete;
  icSearch = icSearch;
  icImpersonate = icImpersonate;
  icFilterList = icFilterList;
  loading:Boolean;
  searchKey:string;
  lovAddView:LovAddView= new LovAddView();
  lovList:LovList= new LovList();
  commonToiletTypeListForExcel =[];
  lovName="Common Toilet Type";
  commonToiletTypeList: MatTableDataSource<any>;
  @ViewChild(MatSort) sort: MatSort;
  listCount;
  editPermission = false;
  deletePermission = false;
  addPermission = false;
  permissionListViewModel: RolePermissionListViewModel = new RolePermissionListViewModel();
  permissionGroup: RolePermissionViewModel = new RolePermissionViewModel();
  updateLovSortingModel: UpdateLovSortingModel = new UpdateLovSortingModel();
  permissions: Permissions;
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  constructor(
    private router: Router,
    private dialog: MatDialog,
    public translateService:TranslateService,
    private snackbar: MatSnackBar,
    private commonService:CommonService,
    private loaderService:LoaderService,
    private excelService:ExcelService,
    public commonfunction:SharedFunction,
    private pageRolePermissions: PageRolesPermission,
    private defaultValuesService: DefaultValuesService,
    private paginatorObj: MatPaginatorIntl,
    ) {
      paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    //translateService.use('en');
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    }); 
  }

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/settings/lov-settings/common-toilet-type')
    this.getAllCommonToiletType();
  }

  getPageEvent(event){    
    // this.getAllSchool.pageNumber=event.pageIndex+1;
    // this.getAllSchool.pageSize=event.pageSize;
    // this.callAllSchool(this.getAllSchool);
  }
  onSearchClear(){
    this.searchKey="";
    this.applyFilter();
  }
  applyFilter(){
    this.commonToiletTypeList.filter = this.searchKey.trim().toLowerCase()
  }
  goToAdd(){
    this.dialog.open(EditCommonToiletTypeComponent, {
      width: '500px'
    }).afterClosed().subscribe((data)=>{
      if(data==='submited'){
        this.getAllCommonToiletType();
      }
    })
  }
  goToEdit(element){
    this.dialog.open(EditCommonToiletTypeComponent,{
      data: element,
      width:'500px'
    }).afterClosed().subscribe((data)=>{
      if(data==='submited'){
        this.getAllCommonToiletType()
      }
    })
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }
  deleteCommonToiletTypedata(element){
    this.lovAddView.dropdownValue.id=element.id
    this.commonService.deleteDropdownValue(this.lovAddView).subscribe(
      (res:LovAddView)=>{
        if(typeof(res)=='undefined'){
          this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
        else{
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open('' + res._message, '', {
              duration: 10000
            });
          } 
          else { 
            this.snackbar.open('' + res._message, '', {
              duration: 10000
            });
            this.getAllCommonToiletType()
           
          }
        }
      }
    )
  }
  confirmDelete(element){
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: "400px",
      data: {
          title: "Are you sure?",
          message: "You are about to delete "+element.lovColumnValue+"."}
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if(dialogResult){
        this.deleteCommonToiletTypedata(element);
      }
   });
  }
 
  getAllCommonToiletType(){
    this.lovList.lovName=this.lovName;
    this.lovList.isListView = true;
    this.commonService.getAllDropdownValues(this.lovList).subscribe(
      (res:LovList)=>{
        if(typeof(res)=='undefined'){
          this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
        else{
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message); 
            if (res.dropdownList == null) {
              this.commonToiletTypeList= new MatTableDataSource(null);
              this.listCount=this.commonToiletTypeList.data;
              this.snackbar.open( res._message, '', {
                duration: 10000
              });
            } else {
              this.commonToiletTypeList= new MatTableDataSource(null);
              this.listCount=this.commonToiletTypeList.data;
            }
          } 
          else { 
            this.commonToiletTypeList=new MatTableDataSource(res.dropdownList) ;
            this.commonToiletTypeListForExcel = res.dropdownList;
            this.commonToiletTypeList.sort=this.sort;  
            this.commonToiletTypeList.paginator=this.paginator;  
            this.listCount=this.commonToiletTypeList.data.length;   
          }
        }
      }
    );
  }

  translateKey(key) {
    let trnaslateKey;
   this.translateService.get(key).subscribe((res: string) => {
       trnaslateKey = res;
    });
    return trnaslateKey;
  }

  exportCommonToiletTypeListToExcel(){
    if(this.commonToiletTypeListForExcel.length!=0){
      let commonToiletType=this.commonToiletTypeListForExcel?.map((item)=>{
        return{
          [this.translateKey('title')]: item.lovColumnValue,
          [this.translateKey('createdBy')]: item.createdBy ? item.createdBy: '-',
          [this.translateKey('createDate')]: this.commonfunction.transformDateWithTime(item.createdOn),
          [this.translateKey('updatedBy')]: item.updatedBy ? item.updatedBy: '-',
          [this.translateKey('updateDate')]:  this.commonfunction.transformDateWithTime(item.updatedOn)
        }
      });
      this.excelService.exportAsExcelFile(commonToiletType,'Common_Toilet_Type_List_')
     }
     else{
    this.snackbar.open('No Records Found. Failed to Export Common Toilet Type List','', {
      duration: 5000
    });
  }
}

sortLovList(event: CdkDragDrop<string[]>) {
  if (event.currentIndex > event.previousIndex) {
    this.commonToiletTypeList.filteredData[event.currentIndex].sortOrder = Number(this.commonToiletTypeList.filteredData[event.currentIndex].sortOrder) - 1;
  }
  else if (event.currentIndex < event.previousIndex) {
    this.commonToiletTypeList.filteredData[event.currentIndex].sortOrder = Number(this.commonToiletTypeList.filteredData[event.currentIndex].sortOrder) + 1;
  }
  this.commonToiletTypeList.filteredData[event.previousIndex].sortOrder = Number(event.currentIndex) + 1;

  let dropdownListMod = this.commonToiletTypeList.filteredData?.sort((a, b) => a.sortOrder < b.sortOrder ? -1 : 1);

  let sortOrderValues = [];

  dropdownListMod.forEach((oneLov, idxLov) => {
    let thisItemSort = new LoVSortOrderValuesModel();
    thisItemSort.id = oneLov.id;
    thisItemSort.sortOrder = Number(idxLov) + 1;

    sortOrderValues.push(thisItemSort);
  })

  this.updateLovSortingModel.sortOrderValues = sortOrderValues;
  this.updateLovSortingModel.tenantId = this.defaultValuesService.getTenantID();
  this.updateLovSortingModel.schoolId = this.defaultValuesService.getSchoolID();
  this.updateLovSortingModel.updatedBy = this.defaultValuesService.getUserGuidId();
  this.updateLovSortingModel.lovName = this.lovName

  this.commonService.updateDropdownValueSortOrder(this.updateLovSortingModel).subscribe((res) => {
    if (res) {
      if (res._failure) {
        this.snackbar.open(res._message, '', {
          duration: 3000
        });
      }
      else {
        this.snackbar.open(res._message, '', {
          duration: 3000
        });
        this.getAllCommonToiletType();
      }
    }
    else {
      this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
        duration: 3000
      });
    }
  })
}
}

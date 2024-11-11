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

import { Component, OnInit, Input, ViewChild } from '@angular/core';
import icMoreVert from '@iconify/icons-ic/twotone-more-vert';
import icAdd from '@iconify/icons-ic/baseline-add';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icSearch from '@iconify/icons-ic/search';
import icFilterList from '@iconify/icons-ic/filter-list';
import icImpersonate from '@iconify/icons-ic/twotone-account-circle';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { stagger40ms } from '../../../../@vex/animations/stagger.animation';
import { TranslateService } from '@ngx-translate/core';
import { EditRaceComponent } from './edit-race/edit-race.component';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import { LovAddView, LovList, LoVSortOrderValuesModel, UpdateLovSortingModel } from '../../../models/lov.model';
import { LoaderService } from '../../../services/loader.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { CommonService } from '../../../services/common.service';
import { ExcelService } from '../../../services/excel.service';
import { SharedFunction } from '../../shared/shared-function';
import { RolePermissionListViewModel, RolePermissionViewModel } from 'src/app/models/roll-based-access.model';
import { CryptoService } from '../../../services/Crypto.service';
import { Permissions } from '../../../models/roll-based-access.model';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { CdkDragDrop } from '@angular/cdk/drag-drop';

@Component({
  selector: 'vex-race',
  templateUrl: './race.component.html',
  styleUrls: ['./race.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class RaceComponent implements OnInit {
  columns = [
    { label: 'sort', property: 'lovId', type: 'text', visible: true },
    { label: 'title', property: 'lovColumnValue', type: 'text', visible: true },
    { label: 'createdBy', property: 'createdBy', type: 'text', visible: true },
    { label: 'createDate', property: 'createdOn', type: 'text', visible: true },
    { label: 'updatedBy', property: 'updatedBy', type: 'text', visible: true },
    { label: 'updateDate', property: 'updatedOn', type: 'text', visible: true },
    { label: 'actions', property: 'actions', type: 'text', visible: true }
  ];
 
  raceListViewModel: LovList = new LovList();
  lovAddView:LovAddView= new LovAddView();
  icMoreVert = icMoreVert;
  icAdd = icAdd;
  icEdit = icEdit;
  icDelete = icDelete;
  icSearch = icSearch;
  icImpersonate = icImpersonate;
  icFilterList = icFilterList;
  loading: Boolean;
  searchKey: string;
  listCount;
  raceListForExcel=[];
  raceListModel: MatTableDataSource<any>;
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  editPermission = false;
  deletePermission = false;
  addPermission = false;
  permissionListViewModel: RolePermissionListViewModel = new RolePermissionListViewModel();
  permissionGroup: RolePermissionViewModel = new RolePermissionViewModel();
  updateLovSortingModel: UpdateLovSortingModel = new UpdateLovSortingModel();
  permissions: Permissions;
  lovName = "Race";
  constructor(private router: Router, private dialog: MatDialog,
    public translateService: TranslateService,
    private loaderService: LoaderService,
    private snackbar: MatSnackBar,
    private commonService: CommonService,
    private excelService:ExcelService,
    public commonfunction:SharedFunction,
    private pageRolePermissions: PageRolesPermission,
    private defaultValuesService: DefaultValuesService
    ) {
    //translateService.use('en');

    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });
  }


  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/settings/lov-settings/race')
    this.getAllRace()
  }


  getPageEvent(event) {
    // this.getAllSchool.pageNumber=event.pageIndex+1;
    // this.getAllSchool.pageSize=event.pageSize;
    // this.callAllSchool(this.getAllSchool);
  }

  openAddNew() {
    this.dialog.open(EditRaceComponent, {
      data: null,
      width: '500px'
    }).afterClosed().subscribe(data => {
      if (data === 'submited') {
        this.getAllRace();
      }
    });
  }

  openEditdata(element) {
    this.dialog.open(EditRaceComponent, {
      data: element,
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if (data === 'submited') {
        this.getAllRace();
      }
    })
  }

  getAllRace() {
    this.raceListViewModel.lovName = "Race";
    this.raceListViewModel.isListView = true;
    this.commonService.getAllDropdownValues(this.raceListViewModel).subscribe(
      (res: LovList) => {
        if (typeof (res) == 'undefined') {
          this.snackbar.open('Race List failed. ' + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
        else {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            if (res.dropdownList == null) {
              this.raceListModel= new MatTableDataSource(null);
              this.listCount=this.raceListModel.data;
              this.snackbar.open( res._message, '', {
                duration: 10000
              });
            } else {
              this.raceListModel= new MatTableDataSource(null);
              this.listCount=this.raceListModel.data;
            }
          }
          else {
            this.raceListModel = new MatTableDataSource(res.dropdownList);
            this.raceListForExcel =res.dropdownList;
            this.listCount=this.raceListModel.data.length;
            this.raceListModel.sort = this.sort;
            this.raceListModel.paginator = this.paginator;
          }
        }
      })
  }

  translateKey(key) {
    let trnaslateKey;
   this.translateService.get(key).subscribe((res: string) => {
       trnaslateKey = res;
    });
    return trnaslateKey;
  }

  exportRaceListToExcel(){
    if(this.raceListForExcel.length!=0){
      let raceList=this.raceListForExcel?.map((item)=>{
        return{
          [this.translateKey('title')]: item.lovColumnValue,
          [this.translateKey('createdBy')]: item.createdBy ? item.createdBy: '-',
          [this.translateKey('createDate')]: this.commonfunction.transformDateWithTime(item.createdOn),
          [this.translateKey('updatedBy')]: item.updatedBy ? item.updatedBy: '-',
          [this.translateKey('updateDate')]:  this.commonfunction.transformDateWithTime(item.updatedOn)
        }
      });
      this.excelService.exportAsExcelFile(raceList,'Race_List_')
     }
     else{
    this.snackbar.open('No Records Found. Failed to Export Race List','', {
      duration: 5000
    });
  }
}
  onSearchClear(){
    this.searchKey="";
    this.applyFilter();
  }

  applyFilter(){
    this.raceListModel.filter = this.searchKey.trim().toLowerCase()
  }

  deleteRaceData(element){
    this.lovAddView.dropdownValue.id=element.id
    this.commonService.deleteDropdownValue(this.lovAddView).subscribe(
      (res:LovAddView)=>{
        if(typeof(res)=='undefined'){
          this.snackbar.open('Race Deletion failed. ' + this.defaultValuesService.getHttpError(), '', {
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
            this.getAllRace()
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
        this.deleteRaceData(element);
      }
   });
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  sortLovList(event: CdkDragDrop<string[]>) {
    if (event.currentIndex > event.previousIndex) {
      this.raceListModel.filteredData[event.currentIndex].sortOrder = Number(this.raceListModel.filteredData[event.currentIndex].sortOrder) - 1;
    }
    else if (event.currentIndex < event.previousIndex) {
      this.raceListModel.filteredData[event.currentIndex].sortOrder = Number(this.raceListModel.filteredData[event.currentIndex].sortOrder) + 1;
    }
    this.raceListModel.filteredData[event.previousIndex].sortOrder = Number(event.currentIndex) + 1;

    let dropdownListMod = this.raceListModel.filteredData?.sort((a, b) => a.sortOrder < b.sortOrder ? -1 : 1);

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
          this.getAllRace();
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

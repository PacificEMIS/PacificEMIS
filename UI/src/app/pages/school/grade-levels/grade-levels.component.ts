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

import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../@vex/animations/stagger.animation';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icSearch from '@iconify/icons-ic/twotone-search';
import icAdd from '@iconify/icons-ic/twotone-add';
import icFilterList from '@iconify/icons-ic/twotone-filter-list';
import { EditGradeLevelsComponent } from '../grade-levels/edit-grade-levels/edit-grade-levels.component';
import { TranslateService } from '@ngx-translate/core';
import { AddGradeLevelModel, GelAllGradeEquivalencyModel, GetAllGradeLevelsModel } from '../../../models/grade-level.model';
import { MatTableDataSource } from '@angular/material/table';
import { GradeLevelService } from '../../../services/grade-level.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { LoaderService } from '../../../services/loader.service';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../models/roll-based-access.model';
import { CryptoService } from '../../../services/Crypto.service';
import { CommonService } from '../../../services/common.service';
import { AgeRangeList, EducationalStage } from '../../../models/common.model';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { LoVSortOrderValuesModel, UpdateLovSortingModel } from 'src/app/models/lov.model';

@Component({
  selector: 'vex-grade-levels',
  templateUrl: './grade-levels.component.html',
  styleUrls: ['./grade-levels.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms
  ]
})
export class GradeLevelsComponent implements OnInit {
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort

  icEdit = icEdit;
  icDelete = icDelete;
  icSearch = icSearch;
  icAdd = icAdd;
  icFilterList = icFilterList;
  getAllGradeLevels: GetAllGradeLevelsModel = new GetAllGradeLevelsModel();
  // gradeLevelList: MatTableDataSource<GetAllGradeLevelsModel>;
  gradeLevelList: MatTableDataSource<any>;
  getGradeEquivalencyList: GelAllGradeEquivalencyModel = new GelAllGradeEquivalencyModel();
  ageRangeList:AgeRangeList = new AgeRangeList();
  educationalStageList:EducationalStage = new EducationalStage();
  sendGradeLevelsToDialog: [];
  editMode: boolean;
  sendDetailsToEditComponent: [];
  loading: boolean = false;
  searchKey: string;
  deleteGradeLevelData: AddGradeLevelModel = new AddGradeLevelModel();
  updateLovSortingModel: UpdateLovSortingModel = new UpdateLovSortingModel();

  columns = [
    { label: 'sort', property: 'lovId', type: 'text', visible: true },
    { label: 'title', property: 'title', type: 'text', visible: true },
    { label: 'shortName', property: 'shortName', type: 'text', visible: true },
    { label: 'sortOrder', property: 'sortOrder', type: 'number', visible: true },
    { label: 'gradeLevel Equivalency', property: 'gradeLevelEquivalency', type: 'text', visible: true },
   // { label: 'Age Range', property: 'ageRange', type: 'text', visible: false},
  //  { label: 'Educational Stage', property: 'educationalStage', type: 'text', visible: false},
    { label: 'nextGrade', property: 'nextGrade', type: 'text', visible: true },
    { label: 'action', property: 'action', type: 'text', visible: true }
  ];
  permissions: Permissions
  constructor(private dialog: MatDialog,
    public translateService: TranslateService,
    private gradeLevelService: GradeLevelService,
    private loaderService: LoaderService,
    private snackbar: MatSnackBar,
    private pageRolePermissions: PageRolesPermission,
    private commonService:CommonService,
    public defaultValuesService: DefaultValuesService) {
    //translateService.use('en');
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });
  }
  ngOnInit(): void {
    this.permissions=this.pageRolePermissions.checkPageRolePermission('/school/settings/school-settings/grade-levels')
    this.getGradeEquivalency();
    this.getAgeRangeList();
    this.getEducationalStageList();
    this.getAllGradeLevel();
  }

 
  getGradeEquivalency() {
    this.gradeLevelService.getAllGradeEquivalency(this.getGradeEquivalencyList).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.snackbar.open('Grade Equivalency List failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.getGradeEquivalencyList= new GelAllGradeEquivalencyModel();
          if (!res.gradeEquivalencyList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        }
        else {
          this.getGradeEquivalencyList = res;

        }
      }
    })
  }
  getAgeRangeList(){
    this.commonService.getAllGradeAgeRange(this.ageRangeList).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.snackbar.open('Grade Age Range List failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.ageRangeList= new AgeRangeList();
          if (!res.gradeAgeRangeList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          } 
        }
        else {
          this.ageRangeList = res;
        }
      }
    })
  }
  getEducationalStageList(){
    this.commonService.getAllGradeEducationalStage(this.educationalStageList).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.snackbar.open('Grade Educational Stage List failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.educationalStageList= new EducationalStage();
          if (!res.gradeEducationalStageList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        }
        else {
          this.educationalStageList = res;
        }
      }
    })
  }

  openAddNew() {
    this.editMode = false;
    this.dialog.open(EditGradeLevelsComponent, {
      data: {
        editMode: this.editMode,
        gradeLevels: this.sendGradeLevelsToDialog,
        gradeLevelEquivalencyList: this.getGradeEquivalencyList,
        ageRangeList: this.ageRangeList,
        educationalStage:this.educationalStageList
      },
      width: '600px'
    }).afterClosed().subscribe((res) => {
      if (res) {
        this.getAllGradeLevel();
      }
    });
  }

  openEdit(editDetails) {
    this.editMode = true;
    this.dialog.open(EditGradeLevelsComponent, {
      data: {
        editMode: this.editMode,
        editDetails: editDetails,
        gradeLevels: this.sendGradeLevelsToDialog,
        gradeLevelEquivalencyList: this.getGradeEquivalencyList,
        ageRangeList: this.ageRangeList,
        educationalStage:this.educationalStageList
      },
      width: '600px'
    }).afterClosed().subscribe((res) => {
      if (res) {
        this.getAllGradeLevel();
      }
    });
  }


  getAllGradeLevel() {
    this.getAllGradeLevels.schoolId =this.defaultValuesService.getSchoolID();
    this.getAllGradeLevels._tenantName = this.defaultValuesService.getTenantName();
    this.getAllGradeLevels._token = this.defaultValuesService.getToken();
    this.getAllGradeLevels.isListView=true;
    this.gradeLevelService.getAllGradeLevels(this.getAllGradeLevels).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.snackbar.open('Grade Level List failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          if (res.tableGradelevelList === null) {
            this.gradeLevelList = new MatTableDataSource([]);
            this.gradeLevelList.sort = this.sort;
            this.sendGradeLevelsToDialog = [];
            this.snackbar.open('' + res._message, '', {
              duration: 10000
            });
          } else {
            this.gradeLevelList = new MatTableDataSource([]);
            this.gradeLevelList.sort = this.sort;
            this.sendGradeLevelsToDialog = [];
          }

        }
        else {
          this.gradeLevelList = new MatTableDataSource(res.tableGradelevelList);
          this.gradeLevelList.sort = this.sort;
          this.sendGradeLevelsToDialog = res.tableGradelevelList;
          this.gradeLevelList.paginator = this.paginator;
        }
      }
    })
  }

  onSearchClear() {
    this.searchKey = "";
    this.applyFilter();
  }

  applyFilter() {
    this.gradeLevelList.filter = this.searchKey.trim().toLowerCase()
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  confirmDelete(deleteDetails) {
    // call our modal window
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: "400px",
      data: {
        title: "Are you sure?",
        message: "You are about to delete " + deleteDetails.title + "."
      }
    });
    // listen to response
    dialogRef.afterClosed().subscribe(dialogResult => {
      // if user pressed yes dialogResult will be true, 
      // if user pressed no - it will be false
      if (dialogResult) {
        this.deleteGradeLevel(deleteDetails);
      }
    });
  }

  deleteGradeLevel(deleteDetails) {
    this.deleteGradeLevelData.tblGradelevel.schoolId = deleteDetails.schoolId;
    this.deleteGradeLevelData.tblGradelevel.gradeId = deleteDetails.gradeId;
    this.deleteGradeLevelData._tenantName = this.defaultValuesService.getTenantName();
    this.deleteGradeLevelData._token = this.defaultValuesService.getToken();
    this.gradeLevelService.deleteGradelevel(this.deleteGradeLevelData).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.snackbar.open('Grade Level Deletion failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      } else if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        this.snackbar.open(res._message, '', {
          duration: 10000
        });
      } else {
        this.snackbar.open('' + res._message, '', {
          duration: 10000
        });
        this.getAllGradeLevel();
      }
    })
  }

  sortLovList(event: CdkDragDrop<string[]>) {

    if (event.currentIndex > event.previousIndex) {
      this.gradeLevelList.filteredData[event.currentIndex].sortOrder = Number(this.gradeLevelList.filteredData[event.currentIndex].sortOrder) - 1;
    }
    else if (event.currentIndex < event.previousIndex) {
      this.gradeLevelList.filteredData[event.currentIndex].sortOrder = Number(this.gradeLevelList.filteredData[event.currentIndex].sortOrder) + 1;
    }
    this.gradeLevelList.filteredData[event.previousIndex].sortOrder = Number(event.currentIndex) + 1;

    let dropdownListMod = this.gradeLevelList.filteredData?.sort((a, b) => a.sortOrder < b.sortOrder ? -1 : 1);

    let sortOrderValues = [];

    dropdownListMod.forEach((oneLov, idxLov) => {
      let thisItemSort = new LoVSortOrderValuesModel();
      thisItemSort.id = oneLov.gradeId;
      thisItemSort.sortOrder = Number(idxLov) + 1;

      sortOrderValues.push(thisItemSort);
    })

    this.updateLovSortingModel.sortOrderValues = sortOrderValues;
    this.updateLovSortingModel.tenantId = this.defaultValuesService.getTenantID();
    this.updateLovSortingModel.schoolId = this.defaultValuesService.getSchoolID();
    this.updateLovSortingModel.updatedBy = this.defaultValuesService.getUserGuidId();

    this.gradeLevelService.updateGradeLevelSortOrder(this.updateLovSortingModel).subscribe((res) => {
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
          this.getAllGradeLevel();
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

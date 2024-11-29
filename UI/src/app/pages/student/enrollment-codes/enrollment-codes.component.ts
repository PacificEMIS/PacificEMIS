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
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { stagger40ms } from '../../../../@vex/animations/stagger.animation';
import { MatSort } from '@angular/material/sort';
import { LoaderService } from '../../../services/loader.service';
import { MatTableDataSource } from '@angular/material/table';
import { EditEnrollmentCodeComponent } from '../enrollment-codes/edit-enrollment-code/edit-enrollment-code.component';
import { EnrollmentCodesService } from '../../../services/enrollment-codes.service';
import { EnrollmentCodeAddView, EnrollmentCodeListView } from '../../../models/enrollment-code.model';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import { TranslateService } from '@ngx-translate/core';
import { ExcelService } from '../../../services/excel.service';
import { CryptoService } from '../../../services/Crypto.service';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../models/roll-based-access.model';
import { DefaultValuesService } from '../../../common/default-values.service';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { CommonService } from 'src/app/services/common.service';
import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { LoVSortOrderValuesModel, UpdateLovSortingModel } from 'src/app/models/lov.model';

@Component({
  selector: 'vex-enrollment-codes',
  templateUrl: './enrollment-codes.component.html',
  styleUrls: ['./enrollment-codes.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class EnrollmentCodesComponent implements OnInit {
  columns = [
    { label: 'sort', property: 'lovId', type: 'text', visible: true },
    { label: 'title', property: 'title', type: 'text', visible: true },
    { label: 'shortName', property: 'shortName', type: 'text', visible: true },
    { label: 'sortOrder', property: 'sortOrder', type: 'text', visible: true },
    { label: 'type', property: 'type', type: 'text', visible: true },
    { label: 'action', property: 'action', type: 'text', visible: true }
  ];

  icMoreVert = icMoreVert;
  icAdd = icAdd;
  icEdit = icEdit;
  icDelete = icDelete;
  icSearch = icSearch;
  icFilterList = icFilterList;
  loading: boolean;
  searchKey: string;
  enrollmentCodelistView: EnrollmentCodeListView = new EnrollmentCodeListView();
  enrollmentAddView: EnrollmentCodeAddView = new EnrollmentCodeAddView();
  updateLovSortingModel: UpdateLovSortingModel = new UpdateLovSortingModel();
  enrollmentListForExcel: EnrollmentCodeListView;
  permissions: Permissions
  constructor(
    private router: Router,
    private dialog: MatDialog,
    private snackbar: MatSnackBar,
    private enrollmentCodeService: EnrollmentCodesService,
    private loaderService: LoaderService,
    private translateService: TranslateService,
    private excelService: ExcelService,
    public defaultValuesService: DefaultValuesService,
    private pageRolePermissions: PageRolesPermission,
    private commonService: CommonService,
  ) {
    //translateService.use('en');
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });

  }
  enrollmentList: MatTableDataSource<any>;
  @ViewChild(MatSort) sort: MatSort;

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/settings/student-settings/enrollment-codes')
    this.getAllStudentEnrollmentCode();
  }
  getAllStudentEnrollmentCode() {
    this.enrollmentCodelistView.isListView=true;
    this.enrollmentCodeService.getAllStudentEnrollmentCode(this.enrollmentCodelistView).subscribe(
      (res: EnrollmentCodeListView) => {
        if (res) {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.enrollmentList = new MatTableDataSource([]);
            this.enrollmentList.sort = this.sort;
            if (!res.studentEnrollmentCodeList) {
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }
          }
          else {
            this.enrollmentList = new MatTableDataSource(res.studentEnrollmentCodeList);
            this.enrollmentListForExcel = res;
            this.enrollmentList.sort = this.sort;
          }
        }
        else{
          this.snackbar.open(this.defaultValuesService.translateKey('enrollmentCodeListFailed') + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
  }

  exportEnrollmentCodesToExcel() {
    if (this.enrollmentListForExcel.studentEnrollmentCodeList.length > 0) {
      const enrollmentList = this.enrollmentListForExcel.studentEnrollmentCodeList?.map((item) => {
        return {
          [(this.defaultValuesService.translateKey('title')).toUpperCase()]: item.title,
          [(this.defaultValuesService.translateKey('shortName')).toUpperCase()]: item.shortName,
          [(this.defaultValuesService.translateKey('sortOrder')).toUpperCase()]: item.sortOrder,
          [(this.defaultValuesService.translateKey('type')).toUpperCase()]: item.type ? item.type : '-'
        };
      });
      this.excelService.exportAsExcelFile(enrollmentList, 'Enrollment_List_');
    } else {
      this.snackbar.open(this.defaultValuesService.translateKey('noRecordsFoundFailedtoExportEnrollmentList'), '', {
        duration: 5000
      });
    }
  }

  goToAdd() {
    this.dialog.open(EditEnrollmentCodeComponent, {
      width: '600px'
    }).afterClosed().subscribe(
      result => {
        if (result === 'submited') {
          this.getAllStudentEnrollmentCode();
        }
      }
    );
  }

  getPageEvent(event) {
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }
  openEditdata(element) {
    this.dialog.open(EditEnrollmentCodeComponent, {
      data: element,
      width: '600px'
    }).afterClosed().subscribe(
      result => {
        if (result === 'submited') {
          this.getAllStudentEnrollmentCode();
        }
      }
    );
  }
  deleteEnrollmentCode(element) {
    this.enrollmentAddView.studentEnrollmentCode = element;
    this.enrollmentCodeService.deleteStudentEnrollmentCode(this.enrollmentAddView).subscribe(
      (res: EnrollmentCodeAddView) => {
        if (res) {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
          else {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
            this.getAllStudentEnrollmentCode();
          }
        }
        else {
          this.snackbar.open(this.defaultValuesService.translateKey('enrollmentCodeDeleteFailed')
          + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
  }
  confirmDelete(element) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
        title: this.defaultValuesService.translateKey('areYouSure'),
        message: this.defaultValuesService.translateKey('youAreAboutToDelete') + element.title + '.'
      }
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult) {
        this.deleteEnrollmentCode(element);
      }
    });
  }

  onSearchClear() {
    this.searchKey = '';
    this.applyFilter();
  }
  applyFilter() {
    this.enrollmentList.filter = this.searchKey.trim().toLowerCase();
  }

  sortLovList(event: CdkDragDrop<string[]>) {
    
    if (event.currentIndex > event.previousIndex) {
      this.enrollmentList.filteredData[event.currentIndex].sortOrder = Number(this.enrollmentList.filteredData[event.currentIndex].sortOrder) - 1;
    }
    else if (event.currentIndex < event.previousIndex) {
      this.enrollmentList.filteredData[event.currentIndex].sortOrder = Number(this.enrollmentList.filteredData[event.currentIndex].sortOrder) + 1;
    }
    this.enrollmentList.filteredData[event.previousIndex].sortOrder = Number(event.currentIndex) + 1;

    let dropdownListMod = this.enrollmentList.filteredData?.sort((a, b) => a.sortOrder < b.sortOrder ? -1 : 1);

    let sortOrderValues = [];

    dropdownListMod.forEach((oneLov, idxLov) => {
      let thisItemSort = new LoVSortOrderValuesModel();
      thisItemSort.id = oneLov.enrollmentCode;
      thisItemSort.sortOrder = Number(idxLov) + 1;

      sortOrderValues.push(thisItemSort);
    })

    this.updateLovSortingModel.sortOrderValues = sortOrderValues;
    this.updateLovSortingModel.tenantId = this.defaultValuesService.getTenantID();
    this.updateLovSortingModel.schoolId = this.defaultValuesService.getSchoolID();
    this.updateLovSortingModel.updatedBy = this.defaultValuesService.getUserGuidId();

    this.enrollmentCodeService.updateStudentEnrollmentCodeSortOrder(this.updateLovSortingModel).subscribe((res) => {
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
          this.getAllStudentEnrollmentCode();
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

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

import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AddStudentComponent } from './add-student/add-student.component';
import { AddCourseSectionComponent } from './add-course-section/add-course-section.component';
import { TranslateService } from '@ngx-translate/core';
import { StudentScheduleService } from '../../../services/student-schedule.service';
import { StudentCourseSectionScheduleAddViewModel, StudentScheduleReportViewModel } from '../../../models/student-schedule.model';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { LoaderService } from '../../../services/loader.service';
import { MatTableDataSource } from '@angular/material/table';
import { ExcelService } from '../../../services/excel.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TitleCasePipe } from '@angular/common';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { Permissions } from '../../../models/roll-based-access.model';
import { CommonService } from '../../../services/common.service';
import { DefaultValuesService } from '../../../common/default-values.service';
import { GetAllSectionModel } from '../../../models/section.model';
import { GetAllGradeLevelsModel } from '../../../models/grade-level.model';
import { GradeLevelService } from '../../../services/grade-level.service';
import { LoginService } from '../../../services/login.service';
import { LanguageModel } from '../../../models/language.model';
import { SectionService } from '../../../services/section.service';
import { MarkingPeriodService } from '../../../services/marking-period.service';
import { CourseManagerService } from '../../../services/course-manager.service';
import { GetAllCourseListModel, GetAllProgramModel, GetAllSubjectModel } from '../../../models/course-manager.model';
import { GetMarkingPeriodTitleListModel } from '../../../models/marking-period.model';
import { weeks } from 'src/app/common/static-data';
import { uniqueColors } from "../../../common/static-data";
import {FormControl} from '@angular/forms';
import {MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS} from '@angular/material-moment-adapter';
import {DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE} from '@angular/material/core';
import * as _moment from 'moment';
import {default as _rollupMoment} from 'moment';

const moment = _rollupMoment || _moment;

export const MY_FORMATS = {
  parse: {
    dateInput: 'LL',
  },
  display: {
    dateInput: 'LL',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY',
  },
};
@Component({
  selector: 'vex-schedule-student',
  templateUrl: './schedule-student.component.html',
  styleUrls: ['./schedule-student.component.scss'],
  providers: [
    TitleCasePipe,
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS],
    },

    {provide: MAT_DATE_FORMATS, useValue: MY_FORMATS},
  ]
})
export class ScheduleStudentComponent implements OnInit, OnDestroy {
  studentList = [];
  languageList;
  sectionList = [];
  programList = [];
  subjectList = [];
  courseList = [];
  markingPeriodList = [];
  gradeLevelList = [];
  viewReport: boolean = false;
  failedScheduling: boolean = false
  showReportTable: boolean = false;
  courseSectionList = [];
  destroySubject$: Subject<void> = new Subject();
  languages: LanguageModel = new LanguageModel();
  getAllSubjectModel: GetAllSubjectModel = new GetAllSubjectModel();
  getAllCourseListModel: GetAllCourseListModel = new GetAllCourseListModel();
  getAllProgramModel: GetAllProgramModel = new GetAllProgramModel();
  getAllGradeLevelsModel: GetAllGradeLevelsModel = new GetAllGradeLevelsModel();
  getMarkingPeriodTitleListModel: GetMarkingPeriodTitleListModel = new GetMarkingPeriodTitleListModel();
  studentCourseSectionScheduleAddViewModel: StudentCourseSectionScheduleAddViewModel = new StudentCourseSectionScheduleAddViewModel();
  studentScheduleReportViewModel: StudentScheduleReportViewModel = new StudentScheduleReportViewModel();
  loading: boolean;
  showCard: boolean = false;
  scheduleReport: MatTableDataSource<any>;
  displayedColumns: string[];
  permissions: Permissions;
  date = new FormControl(moment());
  weekArray = ['S', 'M', 'T', 'W', 'T', 'F', 'S'];
  constructor(private dialog: MatDialog, public translateService: TranslateService,
    private studentScheduleService: StudentScheduleService,
    private loaderService: LoaderService,
    private excelService: ExcelService,
    private gradeLevelService: GradeLevelService,
    private loginService: LoginService,
    private sectionService: SectionService,
    private snackbar: MatSnackBar,
    private markingPeriodService: MarkingPeriodService,
    private courseManagerService: CourseManagerService,
    private titlecasePipe: TitleCasePipe,
    private pageRolePermissions: PageRolesPermission,
    private commonService: CommonService,
    private defaultValuesService: DefaultValuesService
  ) {
    //translateService.use('en');
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission();
    this.getAllLanguage();
    this.getAllSection();
    this.getAllGradeLevelList();
    this.getAllCourse();
    this.getAllSubjectList();
    this.getAllProgramList();
    this.getAllMarkingPeriodList();
  }

  getAllLanguage() {
    this.languages._tenantName = this.defaultValuesService.getTenantName();
    this.loginService.getAllLanguage(this.languages).pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.languageList = [];
      }
      else {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          if (res.tableLanguage) {
            this.languageList = []
          } else {
            this.languageList = []
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        } else {
          this.languageList = res.tableLanguage?.sort((a, b) => { return a.locale < b.locale ? -1 : 1; })
        }
      }
    })
  }

  getAllSection() {
    let section: GetAllSectionModel = new GetAllSectionModel();
    this.sectionService.GetAllSection(section).pipe(takeUntil(this.destroySubject$)).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        if (!data.tableSectionsList) {
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        }
      }
      else {
        this.sectionList = data.tableSectionsList;
      }

    });
  }


  getAllGradeLevelList() {
    this.gradeLevelService.getAllGradeLevels(this.getAllGradeLevelsModel).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
      }
      else {
        this.gradeLevelList = data.tableGradelevelList;
      }

    });
  }

  getAllProgramList() {
    this.courseManagerService.GetAllProgramsList(this.getAllProgramModel).subscribe(data => {
      if(data){
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.programList=[];
          if(!data.programList){
            this.snackbar.open(data._message, '', {
              duration: 1000
            }); 
          }
        }else{
          this.programList=data.programList;
        }
      }else{
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 1000
        }); 
      }  
    });
  }
  getAllSubjectList() {
    this.courseManagerService.GetAllSubjectList(this.getAllSubjectModel).subscribe(data => {
      if(data){
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.subjectList=[];
          if(!data.subjectList){
            this.snackbar.open(data._message, '', {
              duration: 1000
            }); 
          }
        }else{
          this.subjectList=data.subjectList;
        }
      }else{
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 1000
        }); 
      } 

    });
  }

  getAllMarkingPeriodList() {
    this.getMarkingPeriodTitleListModel.schoolId = this.defaultValuesService.getSchoolID();
    this.getMarkingPeriodTitleListModel.academicYear = this.defaultValuesService.getAcademicYear();
    this.markingPeriodService.getAllMarkingPeriodList(this.getMarkingPeriodTitleListModel).subscribe(data => {
     if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
        this.getMarkingPeriodTitleListModel.getMarkingPeriodView = [];
        if(!this.getMarkingPeriodTitleListModel?.getMarkingPeriodView){
          this.snackbar.open(data._message, '', {
            duration: 1000
          }); 
        }
      } else {
        this.getMarkingPeriodTitleListModel.getMarkingPeriodView = data.getMarkingPeriodView;
      }
    });
  }

  getAllCourse() {
    this.courseManagerService.GetAllCourseList(this.getAllCourseListModel).subscribe(data => {
      if (data) {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.courseList = [];
          if (!data.courseViewModelList) {
            this.snackbar.open(data._message, '', {
              duration: 1000
            });
          }
        } else {
          this.courseList = data.courseViewModelList;
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    })
  }

  viewScheduledReport() {
    this.studentScheduleService.studentScheduleReport(this.studentScheduleReportViewModel).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);

      }
      else {
        if (data.scheduleReport.length > 0) {
          this.showReportTable = true;
        }
        data.scheduleReport.map((item) => {
          for (const key in item) {
            if (item.hasOwnProperty(key)) {
              if (key === 'studentId') {
                delete item[key];
              }
            }
          }
        });
        this.displayedColumns = Object.keys(data.scheduleReport[0]);
        this.scheduleReport = new MatTableDataSource(data.scheduleReport);
      }

    });
  }

  selectStudent() {
    if(this.defaultValuesService.checkAcademicYear()){
    this.dialog.open(AddStudentComponent, {
      width: '900px',
      data:{
        sectionList: this.sectionList,
        languageList: this.languageList,
        gradeLevelList: this.gradeLevelList
      }
    }).afterClosed().subscribe((data) => {
      this.studentList = data;
      this.showCard = false;
      this.viewReport = false;
      this.showReportTable = false;
    });
  }
  }

  selectCourseSection() {
    if(this.defaultValuesService.checkAcademicYear()){
    this.dialog.open(AddCourseSectionComponent, {
      width: '900px',
      data:{
        courseList: this.courseList,
        subjectList: this.subjectList,
        programList: this.programList,
        markingPeriodList: this.getMarkingPeriodTitleListModel.getMarkingPeriodView
      }
    }).afterClosed().subscribe((data) => {
      if(data)
        this.courseSectionList=this.createTableDataset(data);
      this.showCard = false;
      this.viewReport = false;
      this.showReportTable = false;
    });
    }
  }

  changeDateEvent(getValue) {
    getValue.durationStartDate=moment(getValue.durationStartDate).format('YYYY-MM-DD');
  }

  createTableDataset(courseSectionList) {
    courseSectionList.map((courseSection: any) => {
      courseSection.courseDurationStartDate=courseSection.durationStartDate=moment(courseSection.durationStartDate).format('YYYY-MM-DD');

      if (moment(new Date()).isBetween(courseSection.courseDurationStartDate, courseSection.durationEndDate)) {
        // courseSection.durationStartDate = moment(new Date()).format('YYYY-MM-DD');
        courseSection.durationStartDate = moment(courseSection.durationStartDate).format('YYYY-MM-DD');
      } // for checking current date is in between or not  

      if (courseSection?.staffName)                                                                            // splitting all staff names 
        courseSection.staffNameList = this.cerateTeacherListArray(courseSection?.staffName.split("|"));
      if (courseSection?.scheduleType.includes('Fixed Schedule'))
        courseSection.meetingDays = this.findDaysBasedOnName(courseSection?.fixedDays.split("|"));
      else if (courseSection?.scheduleType.includes('Variable Schedule'))
        courseSection.meetingDays = this.findDaysBasedOnName(courseSection?.varDay.split("|"));
      else
        courseSection.meetingDays = false;
    })
    return courseSectionList;
  }

  findDaysBasedOnName(days: any) {
    let cloneMeetingDays = "";
    days.map((day) => {
      for (const [i, weekDay] of weeks.entries()) {
        if (weekDay.name.toLowerCase() == day.trim().toLowerCase()) {
          cloneMeetingDays += weekDay.id;
          break;
        }
      }
    });
    return cloneMeetingDays;
  }

  cerateTeacherListArray(staffName: any) {
    let fitstAndLastCharArray = []
    let teachersName = ""
    staffName?.map((names: any, index: any) => {
      if (index < 3) {
        for (var i = 0; i < names.length; i++)
          if (names?.charAt(i) === " " && names?.charAt(i + 1) !== " ")
            fitstAndLastCharArray.push({
              fullName: names,
              firstNameChar: names?.substring(0, 1).toUpperCase(),
              lastNameChar: names?.charAt(i + 1).toUpperCase(),
              bgColor: uniqueColors[Math.floor(Math.random() * 9)].backgroundColor
            });
      } else
        teachersName += `${names} \n`;
    })
    if (teachersName)
      fitstAndLastCharArray.push({ fullName: teachersName });
    return fitstAndLastCharArray;
  }

  translateKey(key) {
    let convertedKey;
    this.translateService.get(key).subscribe((res: string) => {
      convertedKey = res;
    });
    return this.titlecasePipe.transform(convertedKey);
  }

  scheduleStudent() {
    this.showCard = true;
    this.studentCourseSectionScheduleAddViewModel.courseSectionList = this.courseSectionList;
    this.studentCourseSectionScheduleAddViewModel.studentMasterList = this.studentList;
    this.studentCourseSectionScheduleAddViewModel.createdBy = this.defaultValuesService.getUserName();
    this.studentScheduleService.addStudentCourseSectionSchedule(this.studentCourseSectionScheduleAddViewModel).pipe(takeUntil(this.destroySubject$)).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        this.studentCourseSectionScheduleAddViewModel.conflictMessage = 'Failed to schedule student(s) to course section(s)';
        this.failedScheduling = true;
      }
      else {
        this.studentCourseSectionScheduleAddViewModel = data;
        this.studentCourseSectionScheduleAddViewModel.conflictMessage = data.conflictMessage;
        this.viewReport = true;
      }
    });
  }

  refreshAll() {
    this.studentList = [];
    this.courseSectionList = [];
    this.showCard = false;
    this.viewReport = false;
    this.showReportTable = false;
  }

  viewExcelReport() {

    this.studentScheduleService.studentScheduleReport(this.studentScheduleReportViewModel).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);

      }
      else {
        let modifiedReportData = [];
        data.scheduleReport.map((item) => {
          let obj = {};
          for (const key in item) {
            if (item.hasOwnProperty(key)) {
              if (key === 'studentId') {
                delete item[key];
              } else {
                Object.assign(obj, { [key === 'studentInternalId' ? 'Student ID' : this.translateKey(key)]: item[key] })
              }
            }
          }
          modifiedReportData.push(obj);
        })
        for (let report of modifiedReportData) {
          for (let key in report) {
            if (report.hasOwnProperty(key)) {
              if (report[key]?.split('|')[0].includes('False')) {
                report[key] = report[key]?.split('|')[1].trim()
              } else if (report[key]?.split('|')[0].includes('True')) {
                report[key] = report[key]?.split('|')[0].trim()
              }
            }
          }
        }

        if (modifiedReportData.length > 0) {
          this.excelService.exportAsExcelFile(modifiedReportData, 'Schedule_Report_List_')
        }
        else {
          this.snackbar.open('No Records Found. Failed to Export Schedule Report', '', {
            duration: 5000
          });
        }
      }

    });
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}

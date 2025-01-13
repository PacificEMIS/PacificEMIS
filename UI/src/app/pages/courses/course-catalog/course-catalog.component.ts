import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DefaultValuesService } from '../../../common/default-values.service';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { AddCourseModel, CourseCatelogViewModel, CourseWithCourseSectionDetailsViewModel, GetAllCourseListModel, GetAllSubjectModel } from '../../../models/course-manager.model';
import { GetAllGradeLevelsModel } from '../../../models/grade-level.model';
import { GetMarkingPeriodTitleListModel } from '../../../models/marking-period.model';
import { CommonService } from '../../../services/common.service';
import { CourseManagerService } from '../../../services/course-manager.service';
import { GradeLevelService } from '../../../services/grade-level.service';
import { GradesService } from '../../../services/grades.service';
import { LoaderService } from '../../../services/loader.service';
import { MarkingPeriodService } from '../../../services/marking-period.service';
import { weeks } from "../../../common/static-data";
import { scheduleType } from 'src/app/enums/takeAttendanceList.enum';
import { CourseSectionViewModel, ScheduleDetailsModel } from 'src/app/models/staff.model';
import { Transform24to12Pipe } from '../../shared-module/user-define-pipe/transform-24to12.pipe';
import { BlockListViewModel } from '../../../models/school-period.model';
import { SchoolPeriodService } from '../../../services/school-period.service';
import { FixedSchedulingCourseSectionAddModel, OutputEmitDataFormat } from '../../../models/course-section.model';
import { RoomService } from '../../../services/room.service';
import { RoomListViewModel } from '../../../models/room.model';
@Component({
  selector: 'vex-course-catalog',
  templateUrl: './course-catalog.component.html',
  styleUrls: ['./course-catalog.component.scss']
})
export class CourseCatalogComponent implements OnInit {

  getAllCourseListModel: GetAllCourseListModel = new GetAllCourseListModel();
  addCourseModel: AddCourseModel = new AddCourseModel();
  getAllSubjectModel: GetAllSubjectModel = new GetAllSubjectModel();
  getAllGradeLevelsModel: GetAllGradeLevelsModel = new GetAllGradeLevelsModel();
  getMarkingPeriodTitleListModel: GetMarkingPeriodTitleListModel = new GetMarkingPeriodTitleListModel();
  courseWithCourseSectionDetailsViewModel: CourseWithCourseSectionDetailsViewModel = new CourseWithCourseSectionDetailsViewModel();
  courseCatelogViewModel: CourseCatelogViewModel = new CourseCatelogViewModel();
  roomListViewModel:RoomListViewModel=new RoomListViewModel();
  courseList = [];
  subjectList = [];
  periodStartTime;
  periodEndTime;
  gradeLevelList = [];
  markingPeriodList = [];
  courseWithCourseSectionDetails = [];
  SCHEDULE_TYPE = scheduleType;
  staffCourseReport = null;
  loading: boolean;
  courseSectionData=[];
  courseSectionDetailsList=[];
  courseCatalogDetailsForPDF;
  courseWithCourseSectionDetailsForPDF;
  courseSectionDetailsListForPDF;
  weekArray = ['S', 'M', 'T', 'W', 'T', 'F', 'S'];
  mDays: any;
  newCourseDataSet:any=[];
  periodList:any = [];
  roomList:any = [];

  constructor(
    private gradesService: GradesService,
    private gradeLevelService: GradeLevelService,
    private courseManager: CourseManagerService,
    private snackbar: MatSnackBar,
    private markingPeriodService: MarkingPeriodService,
    private defaultValuesService: DefaultValuesService,
    private pageRolePermissions: PageRolesPermission,
    private loaderService: LoaderService,
    private commonService: CommonService,
    private schoolPeriodService:SchoolPeriodService,
    private roomService:RoomService,
  ) {
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.getAllCourse();
    this.getAllSubjectList();
    this.getAllGradeLevelList();
    this.getAllMarkingPeriodList();
    this.getCourseCatelog();
    this.getAllBlockList();
    this.getAllRooms();
  }
  
  getAllCourse() {
    this.courseManager.GetAllCourseList(this.getAllCourseListModel).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        this.courseList = [];
        this.snackbar.open(data._message, '', {
          duration: 1000
        });
      } else {
        this.courseList = data.courseViewModelList;
      }
      console.log('courseList',this.courseList);
    });
  }

  getAllMarkingPeriodList() {
    this.getMarkingPeriodTitleListModel.academicYear = this.defaultValuesService.getAcademicYear();
    this.markingPeriodService.getAllMarkingPeriodList(this.getMarkingPeriodTitleListModel).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        this.markingPeriodList = [];
        if (!data.getMarkingPeriodView) {
          this.snackbar.open(data._message, '', {
            duration: 1000
          });
        }
      } else {
        this.markingPeriodList = data.getMarkingPeriodView;
      }
    });
  }

  getAllSubjectList() {
    this.courseManager.GetAllSubjectList(this.getAllSubjectModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.subjectList = [];
        } else {
          this.subjectList = data.subjectList;
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 1000
        });
      }
    });
  }
  getAllGradeLevelList() {
    this.gradeLevelService.getAllGradeLevels(this.getAllGradeLevelsModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.gradeLevelList = [];
          if (!data.tableGradelevelList) {
            this.snackbar.open(data._message, '', {
              duration: 1000
            });
          }
        } else {
          this.gradeLevelList = data.tableGradelevelList;
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 1000
        });
      }

    });
  }

  getCourseCatalogForPDF() {
    return new Promise((resolve, reject) => {
      this.courseSectionData = [];
      this.courseManager.getCourseCatelog(this.courseCatelogViewModel).subscribe(data => {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.courseCatalogDetailsForPDF = [];
          this.courseWithCourseSectionDetailsForPDF = [];
          this.courseSectionDetailsListForPDF = [];
          this.snackbar.open(data._message, '', {
            duration: 1000
          });
        } else {
          resolve(data);
        }
      });
    });
  }

  printCourseCatalog() {
    this.getCourseCatalogForPDF().then((res: any) => {
      this.courseCatalogDetailsForPDF = res;
      this.courseWithCourseSectionDetailsForPDF = res.courseWithCourseSectionDetailsViewModels;
      for (let course of this.courseWithCourseSectionDetailsForPDF) {
        this.courseSectionDetailsListForPDF = this.createTableDataset(course.getCourseSectionForView);
      }
      setTimeout(() => {
        this.generatePDF();
      }, 100 * this.courseWithCourseSectionDetailsForPDF?.courseWithCourseSectionDetailsViewModels?.length);
    });
  }

  generatePDF() {
    let printContents, popupWin;
    printContents = document.getElementById('printSectionId').innerHTML;
    document.getElementById('printSectionId').className = 'block';
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    if(popupWin === null || typeof(popupWin)==='undefined'){
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
            width: 900px;
            margin: auto;
            font-family: "Roboto", "Helvetica Neue";
            background: #fff;
          
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
              margin: 0;
              font-size: 14px;
            }
          
            table {
              border-collapse: collapse;
              width: 100%;
            }
          
            .course-catalog-header {
              border-bottom: 2px solid #000;
            }
          
            .course-catalog-header td, .course-catalog-heading {
              padding: 10px;
            }
          
            .header-left h2 {
              font-weight: 400;
              font-size: 25px;
              margin: 0;
            }
          
            .header-left p {
              margin: 5px 0;
              font-size: 15px;
            }
          
            .header-right {
              background: #000;
              color: #fff;
              text-align: center;
              padding: 6px 10px;
              border-radius: 3px;
            }
          
            .header-right h3 {
              margin: 0;
              font-size: 16px;
            }
          
            .header-right p {
              margin: 5px 0 0;
            } 
          
            .course-catalog-heading p {
              font-size: 20px;
              font-weight: 600;
            }
          
            .catalog-table {
              padding: 0 10px 20px;
            }
          
            .catalog-table h4, .course-catalog-heading h4 {
              font-weight: normal;
              margin: 0;
              font-size: 14px;
            }
          
            .catalog-table table {
              border: 1px solid #000;
            } 
          
            .catalog-table th {
              text-align: left;
              font-weight: normal;
              background-color: #e5e5e5;
              vertical-align: top;
            }
          
            .catalog-table th:last-child {
              border-right: 1px solid #000;
            }
            .catalog-table th {
              border-bottom: 1px solid #000;
              padding: 5px 10px;
            }
          
            .catalog-table td {
              padding: 8px 0;
            }
          
            .catalog-table td:not(.inner-table td, .inner-table.bordered-table td){
              border-bottom: 1px solid #000;
            }
          
            .catalog-table th p, .catalog-table .inner-table p {
              font-weight: 600;
              font-size: 14px;
              margin: 5px 0;
            }
          
            .catalog-table .inner-table {
              border: none;
            }
          
            .catalog-table .inner-table td {
              padding: 0px 10px 8px;
            }
          
            .catalog-table .inner-table caption {
              text-align: left;
              padding: 0 10px;
            }
          
            .catalog-table .inner-table caption h1 {
              margin-bottom: 0;
              font-size: 18px;
            }
          
            .catalog-table .inner-table caption p {
              font-size: 16px;
              font-weight: normal;
              margin-top: 8px;
            }
          
            .catalog-table .inner-table.bordered-table th {
              background-color: transparent;
              font-weight: 600;
              font-size: 16px;
              border-right: none;
            }
          
            .catalog-table .inner-table.bordered-table caption, .catalog-table .inner-table caption p {
              margin-bottom: 15px;
            }
          
            .catalog-table .inner-table.bordered-table td {
              border-bottom: 1px solid #000;
              padding: 8px 10px;
              font-size: 16px;
            }
          
            .catalog-table .inner-table.bordered-table {
              margin-bottom: 5px;
            }
            .catalog-table td.schedule-table {
              border-bottom: none !important;
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

  getCourseCatelog() {
    this.courseSectionData = [];
    this.newCourseDataSet = [];
    this.courseManager.getCourseCatelog(this.courseCatelogViewModel).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        this.courseWithCourseSectionDetails = [];
        this.snackbar.open(data._message, '', {
          duration: 1000
        });
      } else {
        this.courseWithCourseSectionDetails = data.courseWithCourseSectionDetailsViewModels;
        for (let course of this.courseWithCourseSectionDetails) {
          this.courseSectionDetailsList =this.createTableDataset(course.getCourseSectionForView);
          let item=[]
          this.courseSectionDetailsList.map(x=>{
           if(x.courseId===course.courseId){
            item.push(x);
           }
          })
          this.newCourseDataSet.push(item)
        }
      }
    });
  }

  createTableDataset(courseSectionList: CourseSectionViewModel[]) {
    courseSectionList.map((courseSection) => {
      if (courseSection.courseSection.scheduleType.includes('Fixed Schedule')) {
        courseSection.cloneMeetingDays = courseSection.courseSection.meetingDays;
        const days = courseSection.cloneMeetingDays.split("|");
        courseSection.cloneMeetingDays = "";
        days.map((day) => {
          for (const [i, weekDay] of weeks.entries()) {
            if (weekDay.name.toLowerCase() == day.trim().toLowerCase()) {
              courseSection.cloneMeetingDays =
                courseSection.cloneMeetingDays + weekDay.id;
              break;
            }
          }
        });
        this.courseSectionData.push({
          scheduleType: this.SCHEDULE_TYPE.FixedSchedule,
          weekDays: courseSection.weekDays,
          courseSection: courseSection.courseSection.courseSectionName,
          courseId: courseSection.courseSection.courseId,
          period: courseSection.courseFixedSchedule.blockPeriod.periodTitle,
          time:
            Transform24to12Pipe.prototype.transform(
              courseSection.courseFixedSchedule.blockPeriod.periodStartTime
            ) +
            " to " +
            Transform24to12Pipe.prototype.transform(
              courseSection.courseFixedSchedule.blockPeriod.periodEndTime
            ),
          markingPeriod: courseSection.markingPeriod,
          room: courseSection.courseFixedSchedule.rooms.title,
          meetingDays: courseSection.cloneMeetingDays,
          meetingDaysForPDF: courseSection.courseSection.meetingDays
        });
      } else if (
        courseSection.courseSection.scheduleType.includes('Variable Schedule')
      ) {
        let scheduleDetails: ScheduleDetailsModel[] = [];
        courseSection.courseVariableSchedule?.map((varSchedule) => {
          scheduleDetails.push({
            day: varSchedule.day.toString(),
            period: varSchedule.blockPeriod.periodTitle,
            room: varSchedule.rooms.title,
            time:
              Transform24to12Pipe.prototype.transform(
                varSchedule.blockPeriod.periodStartTime
              ) +
              " to " +
              Transform24to12Pipe.prototype.transform(
                varSchedule.blockPeriod.periodEndTime
              ),
          });
        });
        this.courseSectionData.push({
          scheduleType: this.SCHEDULE_TYPE.variableSchedule,
          rowExpand: true,
          courseSection: courseSection.courseSection.courseSectionName,
          courseId: courseSection.courseSection.courseId,
          period: "-",
          time: "-",
          markingPeriod: courseSection.markingPeriod,
          room: "-",
          scheduleDetails,
        });
      } else if (
        courseSection.courseSection.scheduleType.includes('Calendar Schedule')
      ) {
        let scheduleDetails: ScheduleDetailsModel[] = [];
        courseSection.courseCalendarSchedule?.map((calSchedule) => {
          scheduleDetails.push({
            date: calSchedule.date,
            day: this.dayToName(new Date(calSchedule.date).getDay()),
            period: calSchedule.blockPeriod.periodTitle,
            room: calSchedule.rooms.title,
            time:
              Transform24to12Pipe.prototype.transform(
                calSchedule.blockPeriod.periodStartTime
              ) +
              " to " +
              Transform24to12Pipe.prototype.transform(
                calSchedule.blockPeriod.periodEndTime
              ),
          });
        });
        this.courseSectionData.push({
          scheduleType: this.SCHEDULE_TYPE.calendarSchedule,
          rowExpand: true,
          courseSection: courseSection.courseSection.courseSectionName,
          courseId: courseSection.courseSection.courseId,
          period: "-",
          time: "-",
          markingPeriod: courseSection.markingPeriod,
          room: "-",
          scheduleDetails,
        });
      } else if(courseSection.courseSection.scheduleType.includes('Block Schedule')) {
        let scheduleDetails = [];
        courseSection.bellScheduleList?.map((bellSchedule) => {
          let block;
          courseSection.courseBlockSchedule.map((subItem)=>{
            if(subItem.blockId === bellSchedule.blockId) {
              block = subItem;
            }
          })
          scheduleDetails.push({
            date: bellSchedule.bellScheduleDate,
            day: this.dayToName(new Date(bellSchedule.bellScheduleDate).getDay()),
            period: block.blockPeriod.periodTitle,
            room: block.rooms.title,
            time:
              Transform24to12Pipe.prototype.transform(
                block.blockPeriod.periodStartTime
              ) +
              " to " +
              Transform24to12Pipe.prototype.transform(
                block.blockPeriod.periodEndTime
              ),
          });
        });

        this.courseSectionData.push({
          scheduleType: this.SCHEDULE_TYPE.blockSchedule,
          rowExpand: true,
          courseSection: courseSection.courseSection.courseSectionName,
          courseId: courseSection.courseSection.courseId,
          period: "-",
          time: "-",
          markingPeriod: courseSection.markingPeriod,
          room: "-",
          scheduleDetails,
        });
      }
    });
    
    return this.courseSectionData;
  }
    
  

  dayToName(day) {
    for (let weekDay of weeks) {
      if (day === weekDay.id) {
        return weekDay.name;
      }
    }
    return null;
  }

    getAllBlockList() {
      let blockListViewModel:BlockListViewModel= new BlockListViewModel();
      blockListViewModel.tenantId = this.defaultValuesService.getTenantID();
      blockListViewModel.schoolId = this.defaultValuesService.getSchoolID();
      blockListViewModel.isListView = true;
      this.schoolPeriodService.getAllBlockList(blockListViewModel).subscribe(
        (res: BlockListViewModel) => {
          if(typeof(res)=='undefined'){
            this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
              duration: 10000
            });
          }
          else{
          if(res._failure){
            this.periodList = [];
          this.commonService.checkTokenValidOrNot(res._message);    
              this.snackbar.open(res._message, '', {
                duration: 10000
              }); 
            } 
            else{
              this.periodList = res.getBlockListForView[0]?.blockPeriod;
            }
          }
          
        }
      );
    }

    getAllRooms(){
      this.roomListViewModel.isListView = true;
        this.roomService.getAllRoom(this.roomListViewModel).subscribe(
          (res:RoomListViewModel)=>{
            if(typeof(res)=='undefined'){
              this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
                duration: 10000
              });
            }
            else{
            if(res._failure){
            this.roomList= [];
            this.commonService.checkTokenValidOrNot(res._message);    
                if(!res.tableroomList){
                  this.snackbar.open(res._message, '', {
                    duration: 10000
                  });
                }
              } 
              else{
                this.roomListViewModel=res;
                this.roomList = res?.tableroomList;
              }
            }
          }
        )
      }
    
  

  
}

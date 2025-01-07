import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { classColor } from 'src/app/common/static-data';
import { ScheduledCourseSectionViewModel } from 'src/app/models/dashboard.model';
import { CommonService } from 'src/app/services/common.service';
import { DasboardService } from 'src/app/services/dasboard.service';
import { LoaderService } from 'src/app/services/loader.service';
import icHowToReg from '@iconify/icons-ic/outline-how-to-reg';
@Component({
  selector: 'vex-my-classes',
  templateUrl: './my-classes.component.html',
  styleUrls: ['./my-classes.component.scss']
})
export class MyClassesComponent implements OnInit {
  loading: boolean;
  periodTitle: string;
  icHowToReg = icHowToReg;
  periodStartTime: string;
  takeAttendance: boolean;
  roomTitle: string;
  scheduledCourseSectionViewModel: ScheduledCourseSectionViewModel = new ScheduledCourseSectionViewModel();
  classCount:number=0;
  noticeCount:number=0;
  notificationsList;
  weeks = [
    { name: 'Sunday', id: 0 },
    { name: 'Monday', id: 1 },
    { name: 'Tuesday', id: 2 },
    { name: 'Wednesday', id: 3 },
    { name: 'Thursday', id: 4 },
    { name: 'Friday', id: 5 },
    { name: 'Saturday', id: 6 }
  ];

  constructor(
    private loaderService: LoaderService,
    private defaultValuesService: DefaultValuesService,
    private dasboardService: DasboardService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
  ) {
      this.loaderService.isLoading.subscribe((val) => {
        this.loading = val;
      });
      this.scheduledCourseSectionViewModel.allCourse= true;
      this.getDashboardViewForStaff()
   }

  ngOnInit(): void {
  }

  getDashboardViewForStaff() {
    this.scheduledCourseSectionViewModel.staffId = this.defaultValuesService.getUserId();
    this.scheduledCourseSectionViewModel.membershipId=this.defaultValuesService.getuserMembershipID();
    this.dasboardService.getDashboardViewForStaff(this.scheduledCourseSectionViewModel).subscribe((res) => {
      if (res) {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.classCount=0;
          this.noticeCount=0;
        }
        else {
          this.scheduledCourseSectionViewModel = res;
          this.scheduledCourseSectionViewModel.courseSectionViewList = this.findMeetingDays(this.scheduledCourseSectionViewModel.courseSectionViewList);
          this.classCount= this.scheduledCourseSectionViewModel.courseSectionViewList.length;
          this.noticeCount=this.scheduledCourseSectionViewModel.noticeList?.length;
          this.notificationsList=res.notificationList;
          this.notificationsList.forEach((element, index, arr) => {
              arr[index] = element.slice(0,39)
          });  
        }
      }
      else {
        this.snackbar.open('Dashboard View failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
        this.classCount=0;
        this.noticeCount=0;
      }
    });
  }

  findMeetingDays(courseSectionList) {
    courseSectionList = courseSectionList.map((item) => {
      let random = Math.floor((Math.random() * 7) + 0);
      if (item.scheduleType === 'Fixed Schedule' || item.scheduleType=='Variable Schedule') {
        let days = item.meetingDays.split('|')
        days.map((day) => {
          for (let [i, weekDay] of this.weeks.entries()) {
            if (weekDay.name == day.trim()) {
              item.meetingDays = item.meetingDays + weekDay.id;
              item.attendanceDays = item.attendanceDays? item.attendanceDays +  weekDay.id.toString() : weekDay.id.toString();
              break;
            }
          }
        })
        // item.text=classColor[random].text;
        // item.borderColor=classColor[random].borderColor;
        if (item.attendanceTaken) {
          item.text='text-green';
          item.borderColor='border-green';  
        }
        else {
          item.text='text-red';
          item.borderColor='border-red';
        }
      } else if (item.scheduleType === 'Calendar Schedule') {
        item.attendanceDays = [];
        item.courseCalendarSchedule.map((calendarSchedule) => {
          item.attendanceDays.push(calendarSchedule.date);
        });
        // item.text = classColor[random].text;
        // item.borderColor = classColor[random].borderColor;
        if (item.attendanceTaken) {
          item.text='text-green';
          item.borderColor='border-green';  
        }
        else {
          item.text='text-red';
          item.borderColor='border-red';
        }
      } else if (item.scheduleType === 'Block Schedule') {
        item.attendanceDays = [];
        item.bellScheduleList.map((blockSchedule) => {
          item.attendanceDays.push(blockSchedule.bellScheduleDate);
        });
        // item.text = classColor[random].text;
        // item.borderColor = classColor[random].borderColor;
        if (item.attendanceTaken) {
          item.text='text-green';
          item.borderColor='border-green';  
        }
        else {
          item.text='text-red';
          item.borderColor='border-red';
        }
      }
      return item;

    });
    return courseSectionList;
  }

  selectCourseSection(courseSection) {
    this.dasboardService.selectedCourseSection(courseSection);
    let courseSectionId = courseSection.courseSectionId;
    this.defaultValuesService.setCourseSectionId(courseSectionId);
    this.defaultValuesService.setSelectedCourseSection(courseSection);
    if(this.defaultValuesService.getCourseSectionForAttendance()){
      sessionStorage.removeItem("courseSectionForAttendance");
    }
    
  }

  getPeriodStartTime(courseSection) {
    if (courseSection.scheduleType === "Fixed Schedule") {
      this.periodStartTime = new Date("1900-01-01T" + courseSection?.courseFixedSchedule?.blockPeriod?.periodStartTime).toString();

    }
    return this.periodStartTime;
  }

  getAttendanceForPeriod(courseSection) {
    if (courseSection.scheduleType === "Fixed Schedule") {
      this.takeAttendance = courseSection.takeAttendanceForFixedSchedule ? true : false;
    }
    return this.takeAttendance;
  }

  getPeriodTitle(courseSection) {
    if (courseSection.scheduleType === "Fixed Schedule") {
      this.periodTitle = courseSection?.courseFixedSchedule?.blockPeriod?.periodTitle;

    }

    return this.periodTitle;
  }

  getRoomTitle(courseSection) {
    if (courseSection.scheduleType === "Fixed Schedule") {
      this.roomTitle = courseSection?.courseFixedSchedule?.rooms?.title;

    }
    return this.roomTitle;
  }
}

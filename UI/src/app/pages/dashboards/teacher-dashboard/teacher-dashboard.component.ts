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

import { Component, OnDestroy, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icMissingAttendance from '@iconify/icons-ic/twotone-alarm-off';
import icAssessment from '@iconify/icons-ic/outline-assessment';
import icWarning from '@iconify/icons-ic/warning';
import { CalendarEvent, CalendarMonthViewBeforeRenderEvent, CalendarView } from 'angular-calendar';
import icHowToReg from '@iconify/icons-ic/outline-how-to-reg';
import { DasboardService } from '../../../services/dasboard.service';
import { DashboardViewModel, ScheduledCourseSectionViewModel } from '../../../models/dashboard.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable, Subject } from 'rxjs';
import { CalendarEventModel } from 'src/app/models/calendar-event.model';
import { CalendarModel } from 'src/app/models/calendar.model';
import { map, shareReplay, takeUntil, tap } from 'rxjs/operators';
import { SchoolService } from 'src/app/services/school.service';
import { classColor } from "../../../common/static-data";
import { CommonService } from 'src/app/services/common.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { PageRolesPermission } from 'src/app/common/page-roles-permissions.service';
import { Router } from '@angular/router';
import moment from 'moment';

@Component({
  selector: 'vex-teacher-dashboard',
  templateUrl: './teacher-dashboard.component.html',
  styleUrls: ['./teacher-dashboard.component.scss'],
  styles: [
    `
     .cal-month-view .bg-aqua,
      .cal-week-view .cal-day-columns .bg-aqua,
      .cal-day-view .bg-aqua {
        background-color: #ffdee4 !important;
      }
    `,
  ],
})
export class TeacherDashboardComponent implements OnInit,OnDestroy {
  view: CalendarView = CalendarView.Month;
  viewDate: Date = new Date();
  events: CalendarEvent[] = [];
  events$: Observable<CalendarEvent<{ calendar: CalendarEventModel }>[]>;
  calendars: CalendarModel;
  activeDayIsOpen = true;
  weekendDays: number[];
  filterDays = [];
  icMissingAttendance = icMissingAttendance;
  icAssessment = icAssessment;
  icWarning = icWarning;
  panelOpenState = false;
  icHowToReg = icHowToReg;
  scheduledCourseSectionViewModel: ScheduledCourseSectionViewModel;
  dashboardViewModel: DashboardViewModel = new DashboardViewModel();
  calendarTitle: string;
  noticeBody: string;
  refresh: Subject<any> = new Subject();
  destroySubject$: Subject<void> = new Subject();
  cssClass = 'bg-aqua';
  showCalendarView: boolean = false;
  noticeHide: boolean = true;
  periodTitle: string;
  roomTitle: string;
  periodStartTime: string;
  takeAttendance: boolean;
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
  allCourseFlag: boolean;
  listOfAllClasses = [];
  listOfTodaysClasses = [];
  dayOfWeek: string;
  eventCount = 0;

  constructor(
    public translateService: TranslateService,
    private snackbar: MatSnackBar,
    private schoolService: SchoolService,
    private dasboardService: DasboardService,
    private commonService: CommonService,
    private router: Router,
    private pageRolePermission: PageRolesPermission,
    private defaultValuesService: DefaultValuesService
    ) {
    // translateService.use("en");
  }

  ngOnInit(): void {
    this.dasboardService.markingPeriodTriggeredData.pipe(takeUntil(this.destroySubject$)).subscribe(flag=>{
      if(flag.markingPeriodLoaded || flag.markingPeriodChanged) {
        if(this.defaultValuesService.getMarkingPeriodStartDate()!==null && this.defaultValuesService.getMarkingPeriodEndDate()!==null){
          this.getDashboardViewForStaff();
          this.getDashboardViewForCalendarView();
        }
      }
    })
  }

  getDashboardViewForStaff() {
    this.allCourseFlag = false;
    this.scheduledCourseSectionViewModel = new ScheduledCourseSectionViewModel();
    this.scheduledCourseSectionViewModel.staffId = this.defaultValuesService.getUserId();
    this.scheduledCourseSectionViewModel.membershipId=this.defaultValuesService.getuserMembershipID();
    this.scheduledCourseSectionViewModel.allCourse= true;
    this.dasboardService.getDashboardViewForStaff(this.scheduledCourseSectionViewModel).subscribe((res) => {
      if (res) {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.classCount=0;
          this.noticeCount=0;
          this.listOfAllClasses = [];
          this.listOfTodaysClasses = [];
        }
        else {
          this.scheduledCourseSectionViewModel = res;
          this.listOfAllClasses = this.scheduledCourseSectionViewModel.courseSectionViewList;
          this.listOfTodaysClasses = this.filterTodaysClassesFromAllClasses(this.listOfAllClasses);
          this.scheduledCourseSectionViewModel.courseSectionViewList = this.findMeetingDays(this.listOfTodaysClasses);
          this.classCount= this.scheduledCourseSectionViewModel.courseSectionViewList.length;
          this.noticeCount=this.scheduledCourseSectionViewModel.noticeList?.length;
          this.notificationsList=res.notificationList;
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

  filterTodaysClassesFromAllClasses(allClassesData) {
    let listOfClasses = [];
    for(let course of allClassesData) {
      if(course.scheduleType==="Fixed Schedule" || course.scheduleType==="Variable Schedule") {      
        let days = course.meetingDays.split("|");
        for(let x of this.weeks) {
          if(x.id === new Date().getDay()) {
            this.dayOfWeek = x.name;
          }
        }
        if(days.includes(this.dayOfWeek) && moment(new Date()).isBetween(course.durationStartDate, course.durationEndDate)) {
          listOfClasses.push(course);
        }
        
      } else if(course.scheduleType==="Block Schedule") {
          if(moment(new Date()).isBetween(course.durationStartDate, course.durationEndDate)) {
            listOfClasses.push(course);
          }
      } else if(course.scheduleType==="Calendar Schedule") {
          if(moment(new Date()).isBetween(course.durationStartDate, course.durationEndDate)) {
            listOfClasses.push(course);
          }
      }
    }
    return listOfClasses;
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

  getAttendanceForPeriod(courseSection) {
    if (courseSection.scheduleType === "Fixed Schedule" || courseSection.scheduleType === "Calendar Schedule" || courseSection.scheduleType === "Block Schedule" || courseSection.scheduleType === "Variable Schedule") {
      this.takeAttendance = courseSection.takeAttendanceForFixedSchedule ? true : false;
    }
    return this.takeAttendance;
  }

  //for rendar weekends
  beforeMonthViewRender(renderEvent: CalendarMonthViewBeforeRenderEvent): void {
    renderEvent.body.forEach((day) => {
      const dayOfMonth = day.date.getDay();
      if (this.filterDays.includes(dayOfMonth)) {
        day.cssClass = this.cssClass;
      }
    });
  }

  getHoliDay(event) {
    let events = [];
    events = event;
    if (events.filter(x => x?.meta?.calendar?.isHoliday).length > 0) {
      return true;
    } else {
      return false;
    }
  }

  getEventColor(event){
    let events = [];
    events = event;
    return events.filter(x => x?.meta?.calendar?.isHoliday)[0].meta.calendar.eventColor;
  }

  getPeriodTitle(courseSection) {
    if (courseSection.scheduleType === "Fixed Schedule") {
      this.periodTitle = courseSection?.courseFixedSchedule?.blockPeriod?.periodTitle;
    } else if (courseSection.scheduleType === "Calendar Schedule") {
      this.periodTitle = courseSection?.courseCalendarSchedule[0]?.blockPeriod?.periodTitle;
    } else if (courseSection.scheduleType === "Block Schedule") {
      this.periodTitle = courseSection?.courseBlockSchedule[0]?.blockPeriod?.periodTitle;
    } else if (courseSection.scheduleType === "Variable Schedule") {
      this.periodTitle = courseSection?.courseVariableSchedule[0]?.blockPeriod?.periodTitle;
    }

    return this.periodTitle;
  }

  todaysClasses(){
    this.allCourseFlag = false;
    this.classCount = this.listOfTodaysClasses.length;
    this.scheduledCourseSectionViewModel.courseSectionViewList = this.findMeetingDays(this.listOfTodaysClasses);
  }

  allClasses(){
    this.allCourseFlag = true;
    this.classCount = this.listOfAllClasses.length;
    this.scheduledCourseSectionViewModel.courseSectionViewList = this.findMeetingDays(this.listOfAllClasses);
  }

  getPeriodStartTime(courseSection) {
    if (courseSection.scheduleType === "Fixed Schedule") {
      this.periodStartTime = new Date("1900-01-01T" + courseSection?.courseFixedSchedule?.blockPeriod?.periodStartTime).toString();
    } else if (courseSection.scheduleType === "Calendar Schedule") {
      this.periodStartTime = new Date("1900-01-01T" + courseSection?.courseCalendarSchedule[0]?.blockPeriod?.periodStartTime).toString();
    } else if (courseSection.scheduleType === "Block Schedule") {
      this.periodStartTime = new Date("1900-01-01T" + courseSection?.courseBlockSchedule[0]?.blockPeriod?.periodStartTime).toString();
    } else if (courseSection.scheduleType === "Variable Schedule") {
      this.periodStartTime = new Date("1900-01-01T" + courseSection?.courseVariableSchedule[0]?.blockPeriod?.periodStartTime).toString();
    }
    return this.periodStartTime;
  }

  getRoomTitle(courseSection) {
    if (courseSection.scheduleType === "Fixed Schedule") {
      this.roomTitle = courseSection?.courseFixedSchedule?.rooms?.title;
    } else if (courseSection.scheduleType === "Calendar Schedule") {
      this.roomTitle = courseSection?.courseCalendarSchedule[0]?.rooms?.title;
    } else if (courseSection.scheduleType === "Block Schedule") {
      this.roomTitle = courseSection?.courseBlockSchedule[0]?.rooms?.title;
    } else if (courseSection.scheduleType === "Variable Schedule") {
      this.roomTitle = courseSection?.courseVariableSchedule[0]?.rooms?.title;
    }
    return this.roomTitle;
  }

  getDashboardViewForCalendarView() {
    this.dashboardViewModel.membershipId=this.defaultValuesService.getuserMembershipID();
    this.events$ = this.dasboardService.getDashboardViewForCalendarView(this.dashboardViewModel).pipe(shareReplay(), tap((res) => {
      if (res) {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        }
        else {
          this.calendars = res.schoolCalendar;
          this.showCalendarView = false;
          if (this.calendars !== null) {
            this.calendarTitle = res.schoolCalendar.title;
            this.showCalendarView = true;
            this.getDays(this.calendars.days);
          }
        }
      }
      else {
        this.snackbar.open('Dashboard View failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    }),
      map(({ calendarEventList }: { calendarEventList: CalendarEventModel[] }) => {
        if (calendarEventList !== null) {
          let eventList = calendarEventList.map((calendar: CalendarEventModel) => {

            return {
              id: calendar.eventId,
              title: calendar.title,
              start: new Date(calendar.startDate),
              end: new Date(calendar.endDate),
              allDay: true,
              meta: {
                calendar,
              },
              draggable: true
            };
          });
          this.eventCount = eventList.length;
          return eventList.sort((n1, n2) => {
            if (n1.start > n2.start) {
              return 1;
            }

            if (n1.start < n2.start) {
              return -1;
            }

            return 0;
          });
        }

      })
    );
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

  getDays(days: string) {
    const calendarDays = days;
    let allDays = [0, 1, 2, 3, 4, 5, 6];
    let splitDays = calendarDays.split('').map(x => +x);
    this.filterDays = allDays.filter(f => !splitDays.includes(f));
    this.weekendDays = this.filterDays;
    this.cssClass = 'bg-aqua';
    this.refresh.next();
  }

  goToCalendar() {
    if (this.pageRolePermission.checkPageRolePermission('/school/schoolcalendars').view) {
      this.router.navigate(['school/schoolcalendars']);
    } else {
      this.snackbar.open(`You don't have permission to view Calendar`, '', {
        duration: 10000
      });
    }
  }

  goTeacherMissingAttendance(){
    this.router.navigate(['school/attendance/teacher-missing-attendance']);
  }

  ngOnDestroy(): void {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}

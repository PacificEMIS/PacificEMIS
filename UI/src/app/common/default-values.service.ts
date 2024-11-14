import { Injectable, OnInit } from '@angular/core';
import { CommonField } from '../models/common-field.model';
import { TranslateService } from '@ngx-translate/core';
import { LoginService } from '../services/login.service';
import { Router } from '@angular/router';
import { CryptoService } from '../services/Crypto.service';
import { BehaviorSubject, Subject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import * as moment from "moment";
import { SharedFunction } from '../pages/shared/shared-function';
@Injectable({
  providedIn: 'root',
})
export class DefaultValuesService {
  commonModel: CommonField = new CommonField();
  public newSubject = new Subject<string>();
  private photoChange = new Subject<string>();
  photoChanged = this.photoChange.asObservable();
  customFieldsCheckParentComp = new BehaviorSubject<boolean>(false);
  customFieldsCheckParentCompObs = this.customFieldsCheckParentComp.asObservable();
  TenantId: string = '';
  schoolID: number;
  academicYear: number;
  markingPeriodStartDate: string;
  permissionListKeyName: string;
  public sendAllSchoolFlagSubject = new Subject<boolean>();
  public sendIncludeFlagSubject = new Subject<boolean>();
  public setReportCompoentTitle = new Subject<String>()
  constructor(
    public translateService: TranslateService,
    private cryptoService: CryptoService,
    private commonFunction:SharedFunction,
  ) {
  }

  setDefaultTenant() {
    const url = window.location.href;

    let tenant = '';
    if (url.includes('localhost')) {
      // sessionStorage.setItem('tenant', JSON.stringify('opensisv2_ef6'));
      // tenant = 'opensisv2_ef6';
      sessionStorage.setItem('tenant', JSON.stringify('110'));
      tenant = '110';
    } else {
      let startIndex = url.indexOf('//');
      let endIndex = url.indexOf('.');
      let tenantName = url.substr(startIndex + 2, endIndex - (startIndex + 2));

      sessionStorage.setItem('tenant', JSON.stringify(tenantName));
      tenant = tenantName;
    }
    this.commonModel._tenantName = tenant;
  }

  getDefaultTenant() {
    if (
      this.commonModel._tenantName === '' ||
      this.commonModel._tenantName === null ||
      typeof this.commonModel._tenantName == 'undefined'
    ) {
      this.setDefaultTenant();
    }
    return this.commonModel._tenantName;
  }

  getUserName() {
    let user = JSON.parse(sessionStorage.getItem('user'));
    this.commonModel._userName = user ? user : "";
    return this.commonModel._userName;
  }
  getEmailId() {
    let email = JSON.parse(sessionStorage.getItem('email'));
    return email;
  }
  getUserGuidId() {
    return JSON.parse(sessionStorage.getItem('UserGuidId'));
  }
  setToken(token: string) {
    return new Promise((resolve, reject) => {
      sessionStorage.setItem('token', JSON.stringify(token));
      resolve('');
    })
  }

  getToken() {
    this.commonModel._token = JSON.parse(sessionStorage.getItem('token'));
    return JSON.parse(sessionStorage.getItem('token'));
  }

  setTenantID() {
    if (this.TenantId?.trim().length > 0) {
      this.TenantId = JSON.parse(sessionStorage.getItem('tenantId'));
    }
  }
  getTenantID() {
    return JSON.parse(sessionStorage.getItem('tenantId'));
  }

  setSchoolID(schoolId?: string, setScholIdForStudent?: boolean) {
    if (schoolId && !setScholIdForStudent) {
      sessionStorage.setItem("selectedSchoolId", JSON.stringify(schoolId)); //JSON.Stringify
      this.schoolID = JSON.parse(sessionStorage.getItem("selectedSchoolId"));
      return;
    }
    if (setScholIdForStudent) {
      this.schoolID = +schoolId;
      return;
    }
    if (this.schoolID === null || typeof (schoolId) === "undefined") {
      this.schoolID = JSON.parse(sessionStorage.getItem("selectedSchoolId"));
    }
  }

  setFullAcademicYear(fulltAcademicYear: string) {
    sessionStorage.setItem("fullAcademicYear", JSON.stringify(fulltAcademicYear));
  }

  getFullAcademicYear() {
    return JSON.parse(sessionStorage.getItem("fullAcademicYear"));
  }

  setCourseSectionName(selectedCourseSectionName: string) {
    sessionStorage.setItem("selectedCourseSectionName", JSON.stringify(selectedCourseSectionName));
  }

  getCourseSectionName() {
    return JSON.parse(sessionStorage.getItem("selectedCourseSectionName"));
  }

  setAcademicYear(academicYear?, setScholIdForStudent?: boolean) {
    if(setScholIdForStudent) {
      this.academicYear =  academicYear;
    } else {
      sessionStorage.setItem("academicyear", JSON.stringify(academicYear))
    }
  }

  getAcademicYear() {
    if(this.academicYear) {
      return this.academicYear;
    } else {
      return JSON.parse(sessionStorage.getItem("academicyear"));
    }
  }

  getSchoolID() {
    if (!this.schoolID) {
      this.setSchoolID();
    }
    return this.schoolID;
  }

  // setFinYear(finYear?: string) {
  //   this.commonModel.finYear = sessionStorage.getItem('FinancialYear');
  // }

  // getFinYear() {
  //   this.commonModel.finYear = sessionStorage.getItem('FinancialYear');
  //   return this.commonModel.finYear;
  // }

  getAllMandatoryVariable(obj: any) {
    obj._tenantName = this.getDefaultTenant();
    obj._userName = this.getUserName();
    obj._token = this.getToken();
    obj._academicYear = this.getAcademicYear();
    obj.tenantId = this.getTenantID();
    obj.schoolId = this.getSchoolID();
    return obj;
  }

  getTenent() {
    return JSON.parse(sessionStorage.getItem('tenant'));
  }

  getuserMembershipID() {
    return JSON.parse(sessionStorage.getItem('userMembershipID'));
  }

  getuserMembershipName() {
    return JSON.parse(sessionStorage.getItem('membershipName'));
  }


  getUserMembershipType() {
    return JSON.parse(sessionStorage.getItem('membershipType'));
  }

  setUserMembershipType(membershipType?: string) {

    if (membershipType) {
      sessionStorage.setItem("membershipType", JSON.stringify(membershipType));
    }
  }

  setAll(token: string) {
    this.setDefaultTenant();


    this.setToken(token);
    this.setTenantID();


  }

  translateKey(key) {
    let trnaslateKey;
    this.translateService.get(key).subscribe((res: string) => {
      trnaslateKey = res;
    });
    return trnaslateKey;
  }

  setPageSize(data: number) {
    sessionStorage.setItem('pageSize', JSON.stringify(data));
  }

  getPageSize(): number {
    return JSON.parse(sessionStorage.getItem('pageSize'));
  }

  setMarkingPeriodStartDate(mpStartDate?: string) {
    sessionStorage.setItem("markingPeriodStartDate", JSON.stringify(mpStartDate));
  }

  getMarkingPeriodStartDate() {
    return JSON.parse(sessionStorage.getItem("markingPeriodStartDate"));
  }

  setMarkingPeriodEndDate(mpEndDate?: string) {
    sessionStorage.setItem("markingPeriodEndDate", JSON.stringify(mpEndDate));
  }

  getMarkingPeriodEndDate() {
    return JSON.parse(sessionStorage.getItem("markingPeriodEndDate"));
  }

  createPermissionKeyName() {
    let membershipId = this.getuserMembershipID();
    this.permissionListKeyName = `permissions${membershipId}`;
    return membershipId;
  }

  setPermissionList(permissionList) {
    let membershipId = this.createPermissionKeyName();
    localStorage.setItem(`permissions${membershipId}`, this.cryptoService.dataEncrypt(JSON.stringify(permissionList)))
  }

  getPermissionList() {
    this.createPermissionKeyName();
    if (localStorage.getItem(this.permissionListKeyName)) {
      let permissionList = JSON.parse(this.cryptoService.dataDecrypt(localStorage.getItem(this.permissionListKeyName)));
      return permissionList;
    }
    return null;
  }

  setMarkingPeriodId(id: string) {
    sessionStorage.setItem("markingPeriodId", JSON.stringify(id));
  }

  setMarkingPeriodTitle(title: string) {
    sessionStorage.setItem("markingPeriodTitle", JSON.stringify(title));
  }

  getMarkingPeriodTitle() {
    return JSON.parse(sessionStorage.getItem("markingPeriodTitle"));
  }

  getMarkingPeriodId() {
    return JSON.parse(sessionStorage.getItem("markingPeriodId"));
  }

  //new changes for get------------------------------------------------
  getUserId() {
    return JSON.parse(sessionStorage.getItem('userId'));
  }
  getHttpError() {
    return JSON.parse(sessionStorage.getItem("httpError"));
  }
  getTenantName() {
    return JSON.parse(sessionStorage.getItem('tenant'));
  }
  getuserPhoto() {
    return JSON.parse(sessionStorage.getItem('userPhoto'));
  }
  getLanguage() {
    return JSON.parse(sessionStorage.getItem("language"));
  }
  getBuildVersion() {
    return JSON.parse(sessionStorage.getItem('buildVersion'));
  }
  getCourseSectionForAttendance() {
    return JSON.parse(sessionStorage.getItem("courseSectionForAttendance"));
  }
  getFullYearStartDate() {
    return JSON.parse(sessionStorage.getItem("schoolYearStartDate"));
  }
  getFullYearEndDate() {
    return JSON.parse(sessionStorage.getItem("schoolYearEndDate"));
  }
  getSchoolOpened() {
    return JSON.parse(sessionStorage.getItem('schoolOpened'));
  }
  getSchoolClosed() {
    return JSON.parse(sessionStorage.getItem('schoolClosed'));
  }
  getFullUserName() {
    return JSON.parse(sessionStorage.getItem('fullUserName'));
  }
  getSelectedCourseSection() {
    return JSON.parse(sessionStorage.getItem('selectedCourseSection'));
  }
  //new changes for set---------------------------------
  setFullUserName(fullUserName: string) {
    sessionStorage.setItem("fullUserName", JSON.stringify(fullUserName));
  }
  setErrorMessage(errorMessage: string) {
    sessionStorage.setItem("httpError", JSON.stringify(errorMessage));
  }
  setTenant(tenant: string) {
    sessionStorage.setItem("tenant", JSON.stringify(tenant));
  }
  setSchoolOpened(schoolOpened: string) {
    sessionStorage.setItem("schoolOpened", JSON.stringify(schoolOpened));
  }
  setSchoolClosed(schoolClosed: string) {
    sessionStorage.setItem("schoolClosed", JSON.stringify(schoolClosed));
  }
  setFullYearStartDate(fullYearStartDate: string) {
    sessionStorage.setItem("schoolYearStartDate", JSON.stringify(fullYearStartDate));
  }
  setFullYearEndDate(fullYearEndDate: string) {
    sessionStorage.setItem("schoolYearEndDate", JSON.stringify(fullYearEndDate));
  }
  setTenantIdVal(tenant: string) { //find out the uses
    sessionStorage.setItem("tenantId", JSON.stringify(tenant));
  }
  setEmailId(email: string) {
    sessionStorage.setItem("email", JSON.stringify(email));
  }
  setUserGuidId(email: string) {
    sessionStorage.setItem("UserGuidId", JSON.stringify(email));
  }
  setUserName(user: string) {
    sessionStorage.setItem("user", JSON.stringify(user));
  }
  setUserId(userId: string) {
    sessionStorage.setItem("userId", JSON.stringify(userId));
  }
  setUserPhoto(userPhoto: string) {
    sessionStorage.setItem("userPhoto", JSON.stringify(userPhoto));
  }
  setUserMembershipID(userMembershipID: string) {
    sessionStorage.setItem("userMembershipID", JSON.stringify(userMembershipID));
  }
  setuserMembershipName(membershipName: string) {
    sessionStorage.setItem("membershipName", JSON.stringify(membershipName));
  }
  setLanguage(language: string) {
    sessionStorage.setItem("language", JSON.stringify(language));
  }
  setSelectedCourseSection(selectedCourseSection: any) {
    sessionStorage.setItem("selectedCourseSection", JSON.stringify(selectedCourseSection));
  }

  //new changes foe localStorage get
  getPageId() {
    return JSON.parse(localStorage.getItem("pageId"));
  }
  getCourseSectionId() {
    return JSON.parse(localStorage.getItem('courseSectionId'));
  }
  getCollapseValue() {
    return JSON.parse(localStorage.getItem("collapseValue"));
  }
  getSchoolCount() {
    return JSON.parse(localStorage.getItem("schoolCount"));
  }
  //new changes foe localStorage set

  setPageId(pageId: any) {
    localStorage.setItem("pageId", JSON.stringify(pageId));
  }
  setCollapseValue(collapseValue: boolean) {
    localStorage.setItem("collapseValue", JSON.stringify(collapseValue));
  }
  setCourseSectionId(courseSectionId: string) {
    localStorage.setItem("courseSectionId", JSON.parse(courseSectionId));
  }
  setSchoolCount(schoolCount: any) {
    localStorage.setItem("schoolCount", JSON.stringify(schoolCount));
  }
  setPhotoAndFooter(data) {
     sessionStorage.setItem('photoAndFooter', JSON.stringify(data));
  }
  getPhotoAndFooter() {
    return JSON.parse(sessionStorage.getItem('photoAndFooter'));
  }

  //useing of RxJs
  sendName(data) {
    this.newSubject.next(data);
  }
  sendAllSchoolFlag(data) {
    this.sendAllSchoolFlagSubject.next(data);
  }
  sendIncludeInactiveFlag(data) {
    this.sendIncludeFlagSubject.next(data);
  }
  sendPhoto(data: string) {
    this.photoChange.next(data);
  }


  checkAcademicYear() {
    return moment(this.commonFunction.formatDateSaveWithoutTime(new Date())).isSameOrBefore(this.getFullYearEndDate());
  }
}

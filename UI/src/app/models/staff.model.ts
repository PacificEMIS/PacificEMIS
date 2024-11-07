import { AttendanceCodeCategories } from './attendance-code.model';
import { CalendarModel } from './calendar.model';
import { CertificateModel } from './certificate.model';
import { CommonField } from './common-field.model';
import { CourseBlockSchedule, CourseCalendarSchedule, CourseFixedSchedule, CourseSection, CourseVariableSchedule, markingPeriodTitle } from './course-section.model';
import { FieldsCategoryModel } from './fields-category.model';


export class StaffMasterModel {
    public tenantId: string;
    public schoolId: number;
    public staffId: number;
    public staffPhoto: string;
    public staffThumbnailPhoto: string;
    public staffInternalId: string;
    public alternateId: string;
    public districtId: string;
    public stateId: string;
    public salutation: string;
    public firstGivenName: string;
    public middleName: string;
    public lastFamilyName: string;
    public suffix: string;
    public preferredName: string;
    public previousName: string;
    public socialSecurityNumber: string;
    public otherGovtIssuedNumber: string;
    public studentPhoto: string;
    public dob: string;
    public gender: string;
    public race: string;
    public ethnicity: string;
    public maritalStatus: string;
    public countryOfBirth: number;
    public nationality: number;
    public firstLanguage: number;
    public secondLanguage: number;
    public thirdLanguage: number;
    public physicalDisability: boolean;
    public disabilityDescription: string;
    public portalAccess: boolean;
    public loginEmailAddress: string;
    public profile: string;
    public jobTitle: string;
    public joiningDate: string;
    public endDate: string;
    public isActive: boolean;
    public homeroomTeacher: boolean;
    public primaryGradeLevelTaught: string;
    public primarySubjectTaught: string;
    public otherGradeLevelTaught: string;
    public otherSubjectTaught: string;
    public homePhone: string;
    public mobilePhone: string;
    public officePhone: string;
    public personalEmail: string;
    public schoolEmail: string;
    public twitter: string;
    public facebook: string;
    public instagram: string;
    public youtube: string;
    public linkedin: string;
    public homeAddressLineOne: string;
    public homeAddressLineTwo: string;
    public homeAddressCity: string;
    public homeAddressState: string;
    public homeAddressCountry: string | number;
    public homeAddressZip: string;
    public mailingAddressSameToHome: boolean;
    public mailingAddressLineOne: string;
    public mailingAddressLineTwo: string;
    public mailingAddressCity: string;
    public mailingAddressState: string;
    public mailingAddressCountry: string | number;
    public status: string;
    public mailingAddressZip: string;
    public busNo: string;
    public busPickup : boolean;
    public busDropoff : boolean;
    public emergencyFirstName: string;
    public emergencyLastName: string;
    public relationshipToStaff: string;
    public emergencyHomePhone: string;
    public emergencyWorkPhone: string;
    public emergencyMobilePhone: string;
    public emergencyEmail: string;
    public createdBy: string;
    public createdOn: string;
    public updatedBy: string;
    public updatedOn: string;
    constructor() {
        
    }
}

export class StaffAddModel extends CommonField {
    public staffMaster: StaffMasterModel;
    public fieldsCategoryList: FieldsCategoryModel[];
    public selectedCategoryId: number;
    public passwordHash: string;
    public ExternalSchoolId: number;
    public externalSchoolIds: any[];

    constructor() {
        super();
        this.staffMaster = new StaffMasterModel();
    }
}

export class DeleteStaffModel extends CommonField {
    public staffMasters: [staffMasterCloneModel];
    public tenantId: string;
    public schoolId: string | number;
    public updatedBy: string;
    constructor(){
        super();
    }
}


export class CheckStaffInternalIdViewModel extends CommonField {
    public tenantId: string;
    public staffInternalId: string;
    public isValidInternalId: boolean;
    constructor() {
        super();
    }
}

export class StaffListModel {
    tenantId: string;
    staffId: number;
    firstGivenName: string;
    middleName: string;
    lastFamilyName: string;
    staffInternalId: string;
    profile: string;
    jobTitle: string;
    schoolEmail: string;
    mobilePhone: string;
}
export class staffMasterCloneModel extends StaffMasterModel{
    staffSchoolInfo: [StaffSchoolInfoListModel];
    checked: boolean;
    schoolName: string;
}
export class GetAllStaffModel {
    getStaffListForView: [StaffListModel];
    staffMaster: [staffMasterCloneModel];
    missingAttendanceDateList=[]; // for getting first mising attendance data
    staffId: number;
    courseSectionId:number; // Use for teacher missing attendance
    tenantId: string;
    schoolId: number;
    includeInactive: boolean;
    pageNumber: number;
    pageSize: number;
    _pageSize: number;
    totalCount: number;
    sortingModel: sorting;
    filterParams: filterParams[];
    searchAllSchool:boolean;
    _tenantName: string;
    _token: string;
    _failure: boolean;
    _message: string;
    _userName: string;
    academicYear: number;
    isHomeRoomTeacher: boolean;
    markingPeriodStartDate:string;
    markingPeriodEndDate:string;
    public emailAddress: string;
    public dobStartDate : Date|string;
    public dobEndDate : Date|string;
    public fullName:string;
    public profilePhoto:boolean;
    constructor() {
        this.pageNumber = 1;
        this.pageSize = 10;
        this.sortingModel = new sorting();
        this.filterParams = [];
        this._failure = false;
        this._message = "";
        this.includeInactive = false;
    }
}

export class GetAllStaffReportModel {
    getStaffListForView: [StaffListModel];
    staffMaster: [staffMasterCloneModel];
    missingAttendanceDateList=[]; // for getting first mising attendance data
    staffId: number;
    courseSectionId:number; // Use for teacher missing attendance
    tenantId: string;
    schoolId: number;
    includeInactive: boolean;
    pageNumber: number;
    pageSize: number;
    _pageSize: number;
    totalCount: number;
    sortingModel: sorting;
    filterParams: filterParams[];
    searchAllSchool:boolean;
    _tenantName: string;
    _token: string;
    _failure: boolean;
    _message: string;
    _userName: string;
    academicYear: number;
    isHomeRoomTeacher: boolean;
    markingPeriodStartDate:string;
    markingPeriodEndDate:string;
    public emailAddress: string;
    public dobStartDate : Date|string;
    public dobEndDate : Date|string;
    public fullName:string;
    public profilePhoto:boolean;
    constructor() {
        this.pageNumber = 1;
        this.pageSize = 10;
        this.sortingModel = new sorting();
        this.filterParams = [];
        this._failure = false;
        this._message = "";
    }
}

class sorting {
    sortColumn: string;
    sortDirection: string;
    constructor() {
        this.sortColumn = "";
        this.sortDirection = "";
    }
}

export class filterParams {
    columnName: string;
    filterValue: string;
    filterOption: number;
    constructor() {
        this.columnName = null;
        this.filterOption = 3;
        this.filterValue = null;
    }
}

//Staff Certificate Models 
export class StaffCertificateModel extends CommonField {
    staffCertificateInfo: CertificateModel;
    fieldsCategoryList;
    selectedCategoryId;
    constructor() {
        super();
        this.staffCertificateInfo = new CertificateModel();
    }
}
export class StaffCertificateListModel extends CommonField {
    staffCertificateInfoList: [CertificateModel];
    tenantId: string;
    schoolId: number;
    staffId: number;
    pageNumber: number;
    pageSize: number;
    filterParams: filterParams;
    constructor() {
        super();
        this.pageNumber = 1;
        this.pageSize = 10;
        this.filterParams = null;
    }
}

export class StaffSchoolInfoListModel extends CommonField {
    id: number;
    tenantId: string;
    schoolId: number;
    staffId: number;
    schoolAttachedId: number | string;
    schoolAttachedName: string;
    staffMaster: {};
    profile: string;
    startDate: Date | string;
    endDate: Date | string;
    hide: boolean; //This is not a backend key, Its only for frontend check.
    createdOn: string;
    createdBy: string;
    updatedOn: string;
    updatedBy: string;
    membershipId: number;
    membership: any;
    constructor() {
        super();
        this._failure = false;
    }
}
export class StaffSchoolInfoModel extends CommonField {
    schoolId: number;
    tenantId: string;
    staffId: number;
    profile: string;
    jobTitle: string;
    joiningDate: string;
    endDate: string;
    homeroomTeacher: boolean;
    primaryGradeLevelTaught: string;
    primarySubjectTaught: string;
    otherGradeLevelTaught: string;
    otherSubjectTaught: string;
    fieldsCategoryList;
    selectedCategoryId;
    staffSchoolInfoList: [StaffSchoolInfoListModel];
    isActive : boolean;
    constructor() {
        super();
        this.homeroomTeacher = false;
        this.staffSchoolInfoList = [new StaffSchoolInfoListModel];
        this._failure = false;
    }
}

export class StaffMasterSearchModel {
    public tenantId: string;
    public schoolId: number;
    public staffId: number;
    public staffPhoto: string;
    public staffInternalId: string;
    public alternateId: string;
    public districtId: string;
    public stateId: string;
    public salutation: string;
    public firstGivenName: string;
    public middleName: string;
    public lastFamilyName: string;
    public suffix: string;
    public preferredName: string;
    public previousName: string;
    public socialSecurityNumber: string;
    public otherGovtIssuedNumber: string;
    public studentPhoto: string;
    public dob: string;
    public gender: string;
    public race: string;
    public ethnicity: string;
    public maritalStatus: string;
    public countryOfBirth: number;
    public nationality: number;
    public firstLanguage: number;
    public secondLanguage: number;
    public thirdLanguage: number;
    public physicalDisability: boolean;
    public disabilityDescription: string;
    public portalAccess: boolean;
    public loginEmailAddress: string;
    public profile: string;
    public jobTitle: string;
    public joiningDate: string;
    public endDate: string;
    public homeroomTeacher: boolean;
    public primaryGradeLevelTaught: string;
    public primarySubjectTaught: string;
    public otherGradeLevelTaught: string;
    public otherSubjectTaught: string;
    public homePhone: string;
    public mobilePhone: string;
    public officePhone: string;
    public personalEmail: string;
    public schoolEmail: string;
    public twitter: string;
    public facebook: string;
    public instagram: string;
    public youtube: string;
    public linkedin: string;
    public homeAddressLineOne: string;
    public homeAddressLineTwo: string;
    public homeAddressCity: string;
    public homeAddressState: string;
    public homeAddressCountry: string | number;
    public homeAddressZip: string;
    public mailingAddressSameToHome: boolean;
    public mailingAddressLineOne: string;
    public mailingAddressLineTwo: string;
    public mailingAddressCity: string;
    public mailingAddressState: string;
    public mailingAddressCountry: string | number;
    public mailingAddressZip: string;
    public busNo: string;
    public busPickup : boolean;
    public busDropoff : boolean;
    public emergencyFirstName: string;
    public emergencyLastName: string;
    public relationshipToStaff: string;
    public emergencyHomePhone: string;
    public emergencyWorkPhone: string;
    public emergencyMobilePhone: string;
    public emergencyEmail: string;
    public lastUpdatedBy: string;
    public lastUpdated: string;
    
}

export class StaffImportModel extends CommonField{
    staffAddViewModelList:StaffMasterImportModel[]
    conflictIndexNo:string;
    createdBy: string;
    constructor(){
        super();
        this.staffAddViewModelList=[]
    }
}

export class StaffMasterImportModel {
    staffMaster:any;
    countryOfBirthName: string;
    nationalityName: string;
    firstLanguageName: string;
    secondLanguageName: string;
    thirdLanguageName: string;
    profile: string;
    dob: string;
    joiningDate: string;
    startDate: string;
    _message:string;
    fieldsCategoryList:FieldsCategoryModel[];
    loginEmail?: string;
    passwordHash?: string;
    constructor(){
        this.staffMaster=null;
        this.fieldsCategoryList=[new FieldsCategoryModel];
        this.loginEmail = null;
        this.passwordHash = null;
    }
}

export class AfterImportStatus{
    totalStaffsSent:number;
    totalStaffsImported:number;
    totalStaffsImportedInPercentage:number;
}

export class ScheduledCourseSectionsForStaffModel extends CommonField{
    courseSectionViewList:CourseSectionViewModel[];
    staffId: number;
    courseSectionId: number;
    allCourse: boolean;
    missingAttendanceCount: number;
    pageNumber: number;
    _pageSize: number;
    academicYear: number;
    constructor(){
        super();
    }
}

export class CourseSectionViewModel{
    public tenantId: string;
    public schoolId: number;
    public courseId: number;
    public courseSectionId: number;
    public gradeScaleId: number;
    public gradeScaleType: string;
    public courseSectionName: string;
    public calendarId: number;
    public attendanceCategoryId: number;
    public creditHours: number;
    public academicYear: number;
    public seats: number;
    public allowStudentConflict: boolean;
    public allowTeacherConflict: boolean;
    public isWeightedCourse: boolean;
    public affectsClassRank: boolean;
    public affectsHonorRoll: boolean;
    public onlineClassRoom: boolean;
    public onlineClassroomUrl: string;
    public onlineClassroomPassword: string;
    public useStandards: boolean;
    public standardGradeScaleId: number;
    public durationBasedOnPeriod: boolean;
    public yrMarkingPeriodId: number;
    public smstrMarkingPeriodId: number;
    public qtrMarkingPeriodId: number;
    public durationStartDate: string;
    public durationEndDate: string;
    public scheduleType: string;
    public meetingDays: string;
    public attendanceTaken: boolean;
    public weekDays: string;
    public isActive: boolean;
    public createdBy: string;
    public createdOn: string;
    public updatedBy: string;
    public updatedOn: string;
    public attendanceCodeCategories: AttendanceCodeCategories;
    public course: string;
    public schoolCalendars: CalendarModel;
    public schoolMaster: string;
    public quarters:markingPeriodTitle;
    public schoolYears:markingPeriodTitle;
    public semesters:markingPeriodTitle;
    public markingPeriod: string;
    public mpTitle:string; //[marking period title]This key is only used for front end view to extract mp title, this is not related to backend.
    public mpStartDate:string;
    public mpEndDate:string;
    public cloneMeetingDays: string;
    public courseSection: CourseSection;
    public courseFixedSchedule: CourseFixedSchedule;
    public courseVariableSchedule: CourseVariableSchedule[];
    public courseCalendarSchedule: CourseCalendarSchedule[];
    public courseBlockSchedule: CourseBlockSchedule[];
    public bellScheduleList;
}

export class ScheduledCourseSectionTableModel{
    courseSection: string;
    period: string;
    weekDays?: string;
    markingPeriod: string;
    time: string;
    room: string;
    meetingDays?:string;
    scheduleType: string;
    rowExpand?: boolean;
    scheduleDetails?: ScheduleDetailsModel[];
}
export class ScheduleDetailsModel{
    date?: string;
    day: string;
    period: string;
    time: string;
    room: string;
}

export class GetStaffModel {
    staffId: number;
    tenantId: string;
    schoolId: number;
    _tenantName: string;
    _token: string;
    _failure: boolean;
    _message: string;
    _userName: string;
    academicYear: number;
    public emailAddress: string;
    public DobStartDate : Date|string;
    public DobEndDate : Date|string;
    public fullName:string;
    public profilePhoto:boolean;
    constructor() { 
        this._failure = false;
        this._message = "";
    }
}


import { CommonField } from "./common-field.model";
import { FieldsCategoryModel } from './fields-category.model';
import { SchoolMasterModel } from "./school-master.model";

export class StudentMasterModel {
    public tenantId: string;
    public schoolId: number;
    public studentId: number;
    public studentGuid: string;
    public studentInternalId: string;
    public studentPortalId: string;
    public alternateId: string;
    public districtId: number;
    public stateId: number;
    public admissionNumber: string;
    public rollNumber: string;
    public salutation: string;
    public firstGivenName: string;
    public middleName: string;
    public lastFamilyName: string;
    public suffix: string;
    public preferredName: string;
    public gradeLevelTitle: string;
    public previousName: string;
    public socialSecurityNumber: string;
    public otherGovtIssuedNumber: string;
    public studentPhoto: string;
    public studentThumbnailPhoto: string;
    public dob: string;
    public displayAge: string;
    public gender: string;
    public race: string;
    public checked: boolean;
    public isActive: boolean;
    public ethnicity: string;
    public maritalStatus: string;
    public countryOfBirth: number;
    public nationality: number;
    public firstLanguageId: number;
    public firstLanguageName: string;
    public secondLanguageId: number;
    public thirdLanguageId: number;
    public sectionId: number;
    public sectionName: string;
    public estimatedGradDate: string;
    public eligibility504: boolean;
    public economicDisadvantage: boolean;
    public freeLunchEligibility: boolean;
    public specialEducationIndicator: boolean;
    public lepIndicator: boolean;
    public homePhone: string;
    public mobilePhone: string;
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
    public homeAddressZip: string;
    public busNo: string;
    public schoolBusPickUp: boolean;
    public schoolBusDropOff: boolean;
    public mailingAddressSameToHome: boolean;
    public mailingAddressLineOne: string;
    public mailingAddressLineTwo: string;
    public mailingAddressCity: string;
    public mailingAddressState: string;
    public mailingAddressZip: string;
    public primaryContactRelationship: string;
    public primaryContactFirstname: string;
    public primaryContactLastname: string;
    public primaryContactHomePhone: string;
    public primaryContactWorkPhone: string;
    public primaryContactMobile: string;
    public primaryContactEmail: string;
    public isPrimaryCustodian: boolean;
    public isPrimaryPortalUser: boolean;
    public primaryPortalUserId: string;
    public primaryContactStudentAddressSame: boolean;
    public primaryContactAddressLineOne: string;
    public primaryContactAddressLineTwo: string;
    public primaryContactCity: string;
    public primaryContactState: string;
    public primaryContactZip: string;
    public secondaryContactRelationship: string;
    public secondaryContactFirstname: string;
    public secondaryContactLastname: string;
    public secondaryContactHomePhone: string;
    public secondaryContactWorkPhone: string;
    public secondaryContactMobile: string;
    public secondaryContactEmail: string;
    public isSecondaryCustodian: boolean;
    public isSecondaryPortalUser: boolean;
    public secondaryPortalUserId: string;
    public secondaryContactStudentAddressSame: boolean;
    public secondaryContactAddressLineOne: string;
    public secondaryContactAddressLineTwo: string;
    public secondaryContactCity: string;
    public secondaryContactState: string;
    public studentEnrollment: [StudentEnrollmentDetails];
    public secondaryContactZip: string;
    public homeAddressCountry: number;
    public mailingAddressCountry: number;
    public address: string;
    public schoolName: string;
    public criticalAlert: string;
    public alertDescription: string;
    public primaryCarePhysician: string;
    public primaryCarePhysicianPhone: string;
    public medicalFacility: string;
    public medicalFacilityPhone: string;
    public insuranceCompany: string;
    public insuranceCompanyPhone: string;
    public policyNumber: string;
    public policyHolder: string;
    public dentist: string;
    public dentistPhone: string;
    public createdBy: string;
    public createdOn: string;
    public updatedBy: string;
    public updatedOn: string;
    public vision: string;
    public visionPhone: string;
    public schoolMaster: Object;
    public academicYear: string;
    public studentMedicalAlert: StudentMedicalModel[];
    constructor() {
        this.academicYear = JSON.parse(sessionStorage.getItem("academicyear"));
        this.tenantId = JSON.parse(sessionStorage.getItem("tenantId"));
        // this.schoolId = JSON.parse(sessionStorage.getItem("selectedSchoolId"));
    }
}

export class StudentAddModel extends CommonField {
    public studentMaster: StudentMasterModel;
    public schoolMaster: {};
    public fieldsCategoryList: FieldsCategoryModel[];
    public selectedCategoryId: number;
    public loginEmail: string;
    public passwordHash: string;
    public portalAccess: boolean;
    public currentGradeLevel: string;
    public studentEnrollment: {};
    public academicYear:string;
    public allowDuplicate:boolean;
    public checkDuplicate:number;
    constructor() {
        super();
        this.studentMaster = new StudentMasterModel();
        this.schoolMaster = null;
        this.studentEnrollment = null;
    }
}

export class StudentAddForGroupAssignModel extends CommonField {
    public studentMaster: StudentMasterModel;
    public schoolMaster: {};
    public fieldsCategoryList: FieldsCategoryModel[];
    public selectedCategoryId: number;
    public loginEmail: string;
    public passwordHash: string;
    public portalAccess: boolean;
    public currentGradeLevel: string;
    public studentEnrollment: {};
    public academicYear:string;
    public studentIds: any;

    constructor() {
        super();
        this.studentIds = [];
        this.studentMaster = new StudentMasterModel();
        this.schoolMaster = null;
        this.studentEnrollment = null;
    }
}

export class StudentEnrollmentForGroupAssignModel extends CommonField {
    studentEnrollments: StudentEnrollmentDetailsForGroupAssignModel;
    tenantId: string;
    studentId: number;
    calenderId: number | string;
    rollingOption: string;
    schoolId: number;
    academicYear: string;
    studentGuid: string;
    sectionId: number;
    estimatedGradDate: string;
    eligibility504: boolean;
    economicDisadvantage: boolean;
    freeLunchEligibility: boolean;
    specialEducationIndicator: boolean;
    lepIndicator: boolean;
    public studentIds: any;
    constructor() {
        super();
        this.studentIds = [];
        this.studentEnrollments = new StudentEnrollmentDetailsForGroupAssignModel();
    }
}

export class StudentEnrollmentDetailsForGroupAssignModel {
    tenantId: string;
    schoolId: number;
    calenderId: number
    rollingOption: string;
    gradeId: number;
    gradeLevelTitle: string;
    updatedBy: string;
    enrollmentDate?: string;
}



export class AddEditStudentMedicalProviderForGroupAssignModel extends CommonField{
    studentMedicalProvider: StudentMedicalModel;
    public fieldsCategoryList: FieldsCategoryModel[];
    selectedCategoryId: number;
    public studentIds: any;
    
    constructor(){
        super();
        this.studentIds = [];
        this.studentMedicalProvider = new StudentMedicalModel();
        this.studentMedicalProvider.id = 0;
    }
}

export class CheckStudentInternalIdViewModel extends CommonField {
    public tenantId: string;
    public schoolId: number;
    public studentInternalId: string;
    public isValidInternalId: boolean;
}

export class DeleteStudentModel extends CommonField {    
    public studentListViews: StudentListView[];
    public tenantId: string;
    public schoolId: string | number;
    public updatedBy: string;
    constructor() {
        super();
    }
}

class studentList {
    tenantId: string;
    schoolId: number;
    studentId: number;
    alternateId: number;
    studentInternalId: number;
    mobilePhone: string;
    homePhone: string
    firstGivenName: string;
    middleName: string;
    lastFamilyName: string;
    personalEmail: string;
    gradeLevelTitle: string;
    enrollmentDate: string
}

export class StudentResponseListModel {
    public getStudentListForViews: [studentList];
    public studentMaster: [StudentMasterModel];
    public studentListViews:StudentListView[];
    public tenantId: string;
    public schoolId: number;
    public totalCount: number;
    public pageNumber: number;
    public _pageSize: number;
    public _tenantName: string;
    public _token: string;
    public _failure: string;
    public _message: string;
}

export class StudentListModel extends CommonField {
    public studentMaster: StudentMasterModel[];
    public studentListViews: StudentListView[];
    public totalCount: number;
    public tenantId: string;
    public schoolId: number;
    public courseSectionId: number;
    public staffId: number;
    public includeInactive: boolean;
    public searchAllSchool: boolean;
    public pageNumber: number;
    public pageSize: number;
    public profilePhoto: boolean;
    public sortingModel: sorting;
    public filterParams: filterParams[];
    public dobStartDate: string;
    public enrollmentCode: number;
    public gradeLevelTitle: string;
    public gradeId: number;
    public enrollmentDate: string;
    public updatedBy: string;
    public academicYear: number;
    public dobEndDate: string;
    public fullName: string;
    public emailAddress: string;
    constructor() {
        super();
        this.pageNumber = 1;
        this.pageSize = 10;
        this.dobStartDate = null;
        this.dobEndDate = null;
        this.sortingModel = new sorting();
        this.filterParams = [];
    }
}

export class StudentListByDateRangeModel extends CommonField {
    public includeInactive: boolean;
    public searchAllSchool: boolean;
    public pageNumber: number;
    public pageSize: number;
    public sortingModel: sorting;
    public filterParams: filterParams[];
    public markingPeriodStartDate: string;
    public markingPeriodEndDate: string;
    public academicYear: number;
    public EmailAddress: string;

    constructor() {
        super();
        this.pageNumber = 1;
        this.pageSize = 10;
        this.sortingModel = new sorting();
        this.filterParams = [];
    }
}

export class StudentListView{
    public tenantId: string;
    public schoolId: number;
    public studentId: number;
    public studentGuid: string;
    public studentInternalId: string;
    public studentPortalId: string;
    public alternateId: string;
    public districtId: number;
    public stateId: number;
    public admissionNumber: string;
    public rollNumber: string;
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
    public displayAge: string;
    public gender: string;
    public race: string;
    public checked: boolean;
    public ethnicity: string;
    public maritalStatus: string;
    public countryOfBirth: number;
    public nationality: number;
    public firstLanguageId: number;
    public firstLanguageName: string;
    public secondLanguageId: number;
    public thirdLanguageId: number;
    public sectionId: number;
    public sectionName: string;
    public estimatedGradDate: string;
    public eligibility504: boolean;
    public economicDisadvantage: boolean;
    public freeLunchEligibility: boolean;
    public specialEducationIndicator: boolean;
    public lepIndicator: boolean;
    public homePhone: string;
    public mobilePhone: string;
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
    public homeAddressZip: string;
    public busNo: string;
    public schoolBusPickUp: boolean;
    public schoolBusDropOff: boolean;
    public mailingAddressSameToHome: boolean;
    public mailingAddressLineOne: string;
    public mailingAddressLineTwo: string;
    public mailingAddressCity: string;
    public mailingAddressState: string;
    public mailingAddressZip: string;
    public enrollmentType:string;
    public isActive:boolean;
    public lastUpdated: string;
    public updatedBy: string;
    public enrollmentId: number;
    public enrollmentDate: string;
    public enrollmentCode: string;
    public calenderId:number;
    public gradeId: number;
    public gradeLevelTitle: string;
    public rollingOption: string;
    public primaryContactRelationship: string;
    public primaryContactFirstname: string;
    public primaryContactLastname: string;
    public primaryContactHomePhone: string;
    public primaryContactWorkPhone: string;
    public primaryContactMobile: string;
    public primaryContactEmail: string;
    public isPrimaryCustodian: boolean;
    public isPrimaryPortalUser: boolean;
    public primaryPortalUserId: string;
    public primaryContactStudentAddressSame: boolean;
    public primaryContactAddressLineOne: string;
    public primaryContactAddressLineTwo: string;
    public primaryContactCity: string;
    public primaryContactState: string;
    public primaryContactZip: string;
    public secondaryContactRelationship: string;
    public secondaryContactFirstname: string;
    public secondaryContactLastname: string;
    public secondaryContactHomePhone: string;
    public secondaryContactWorkPhone: string;
    public secondaryContactMobile: string;
    public secondaryContactEmail: string;
    public isSecondaryCustodian: boolean;
    public isSecondaryPortalUser: boolean;
    public secondaryPortalUserId: string;
    public secondaryContactStudentAddressSame: boolean;
    public secondaryContactAddressLineOne: string;
    public secondaryContactAddressLineTwo: string;
    public secondaryContactCity: string;
    public secondaryContactState: string;
    public studentEnrollment: [StudentEnrollmentDetails];
    public secondaryContactZip: string;
    public homeAddressCountry: number;
    public mailingAddressCountry: number;
    public address: string;
    public schoolName: string;
    public criticalAlert: string;
    public alertDescription: string;
    public primaryCarePhysician: string;
    public primaryCarePhysicianPhone: string;
    public medicalFacility: string;
    public medicalFacilityPhone: string;
    public insuranceCompany: string;
    public insuranceCompanyPhone: string;
    public policyNumber: string;
    public policyHolder: string;
    public dentist: string;
    public dentistPhone: string;
    public vision: string;
    public visionPhone: string;
    public isCurrentSchool?: boolean;
}
class sorting {
    sortColumn: string;
    sortDirection: string;
    constructor() {
        this.sortColumn = '';
        this.sortDirection = '';
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
export class StudentDocumentListModel {
    public tenantId: string;
    public schoolId: number;
    public studentId: number;
    public documentId: number;
    public fileUploaded: string;
    public uploadedOn: string;
    public uploadedBy: string;
    public studentMaster: {};
}

export class GetAllStudentDocumentsList extends CommonField {
    public studentDocumentsList: [];
    public tenantId: string;
    public schoolId: number;
    public studentId: number;
}

export class StudentDocumentAddModel extends CommonField {
    public studentDocuments: Array<string>;
    public tenantId: string;
    public schoolId: number;
    public studentId: number;
    public documentId: number;
    public fileUploaded: string;
    public uploadedOn: string;
    public uploadedBy: string;
    public studentMaster: {};
}

export class StudentDocumentAddForGroupAssignModel extends CommonField {
    public studentDocuments: Array<string>;
    public tenantId: string;
    public schoolId: number;
    public studentId: number;
    public documentId: number;
    public fileUploaded: string;
    public uploadedOn: string;
    public uploadedBy: string;
    public studentMaster: {};
    public studentIds: any;

    constructor() {
        super();
        this.studentIds = [];
    }
}

export class StudentSiblingSearch extends CommonField {
    getStudentForView: [StudentMasterModel]
    tenantId: string;
    schoolId: number;
    firstGivenName: string;
    lastFamilyName: string;
    gradeLevel: string;
    studentInternalId: string;
    dob: string;
    gradeLevelTitle: string;
}

export class StudentViewSibling extends CommonField {
    studentMaster: StudentMasterModel[]
    tenantId: string;
    schoolId: number;
    studentId: number;
}

export class StudentSiblingAssociation extends CommonField {
    studentMaster: StudentMasterModel;
    schoolId: number;
    studentId: number;
    constructor() {
        super();
        this.studentMaster = new StudentMasterModel()
        this.studentMaster.tenantId = JSON.parse(sessionStorage.getItem("tenantId"));
    }
}

export class StudentEnrollmentSchoolListModel extends CommonField {
    public schoolMaster: SchoolMasterModel[]
    public tenantId: string;
    constructor() {
        super();
        this.schoolMaster = [new SchoolMasterModel];
    }
}

export class StudentEnrollmentDetails {
    tenantId: string;
    schoolId: number | string;
    studentId: number;
    academicYear: number;
    enrollmentId: number;
    rolloverId: number;
    calenderId: number;
    rollingOption: string;
    schoolName: string;
    gradeId: number | string;
    gradeLevelTitle: string;
    enrollmentDate: string;
    enrollmentCode: string;
    exitDate: string;
    exitCode: string;
    exitType: string;
    type: string;
    transferredSchoolId: number | string;
    schoolTransferred: string;
    transferredGrade: string;
    enrollmentType: string;
    createdBy: string;
    createdOn: string;
    updatedOn: string;
    updatedBy: string;
    studentGuid: string;
    startYear: string;
    endYear: string;
    isActive: boolean;
    showDrop:boolean;
    exitReason : string;
}
export class StudentEnrollmentModel extends CommonField {
    studentEnrollments: Array<StudentEnrollmentDetails>;
    studentEnrollmentListForView: StudentEnrollmentDetails[];
    tenantId: string;
    studentId: number;
    calenderId: number | string;
    rollingOption: string;
    schoolId: number;
    academicYear: string;
    studentGuid: string;
    sectionId: number;
    estimatedGradDate: string;
    eligibility504: boolean;
    economicDisadvantage: boolean;
    freeLunchEligibility: boolean;
    specialEducationIndicator: boolean;
    lepIndicator: boolean;
    fieldsCategoryList;
    selectedCategoryId;
    transferredSchoolId;
    transferredSchoolName;
    enrollOtherSchoolId;
    statusCode;
    constructor() {
        super();
        this.studentEnrollments = [new StudentEnrollmentDetails];
        this.studentEnrollmentListForView = [new StudentEnrollmentDetails];
    }
}

export class StudentMasterSearchModel {

    public studentInternalId: string;
    public studentPortalId: string;
    public alternateId: string;
    public districtId: number;
    public stateId: number;
    public gradeId: number;
    public admissionNumber: string;
    public courseSection: number;
    public rollNumber: string;
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
    public rollingOption: string;
    public enrollmentDate: string;
    public enrollmentCode: string;
    public exitDate: string;
    public exitCode: string;
    public displayAge: string;
    public gender: string;
    public race: string;
    public ethnicity: string;
    public maritalStatus: string;
    public countryOfBirth: number;
    public nationality: number;
    public firstLanguageId: number;
    public secondLanguageId: number;
    public thirdLanguageId: number;
    public sectionId: number;
    public estimatedGradDate: string;
    public eligibility504: boolean;
    public economicDisadvantage: boolean;
    public freeLunchEligibility: boolean;
    public specialEducationIndicator: boolean;
    public lepIndicator: boolean;
    public homePhone: string;
    public mobilePhone: string;
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
    public homeAddressZip: string;
    public busNo: string;
    public schoolBusPickUp: boolean;
    public schoolBusDropOff: boolean;
    public mailingAddressSameToHome: boolean;
    public mailingAddressLineOne: string;
    public mailingAddressLineTwo: string;
    public mailingAddressCity: string;
    public mailingAddressState: string;
    public mailingAddressZip: string;
    public primaryContactRelationship: string;
    public primaryContactFirstname: string;
    public primaryContactLastname: string;
    public primaryContactHomePhone: string;
    public primaryContactWorkPhone: string;
    public primaryContactMobile: string;
    public primaryContactEmail: string;
    public isPrimaryCustodian: boolean;
    public isPrimaryPortalUser: boolean;
    public primaryPortalUserId: string;
    public primaryContactStudentAddressSame: boolean;
    public primaryContactAddressLineOne: string;
    public primaryContactAddressLineTwo: string;
    public primaryContactCity: string;
    public primaryContactState: string;
    public primaryContactZip: string;
    public secondaryContactRelationship: string;
    public secondaryContactFirstname: string;
    public secondaryContactLastname: string;
    public secondaryContactHomePhone: string;
    public secondaryContactWorkPhone: string;
    public secondaryContactMobile: string;
    public secondaryContactEmail: string;
    public isSecondaryCustodian: boolean;
    public isSecondaryPortalUser: boolean;
    public secondaryPortalUserId: string;
    public secondaryContactStudentAddressSame: boolean;
    public secondaryContactAddressLineOne: string;
    public secondaryContactAddressLineTwo: string;
    public secondaryContactCity: string;
    public secondaryContactState: string;
    public studentEnrollment: [StudentEnrollmentDetails];
    public secondaryContactZip: string;
    public homeAddressCountry: number;
    public mailingAddressCountry: number;
    public address: string;
    public schoolName: string;
    public criticalAlert: string;
    public alertDescription: string;
    public primaryCarePhysician: string;
    public primaryCarePhysicianPhone: string;
    public medicalFacility: string;
    public medicalFacilityPhone: string;
    public insuranceCompany: string;
    public insuranceCompanyPhone: string;
    public policyNumber: string;
    public policyHolder: string;
    public dentist: string;
    public dentistPhone: string;
    public vision: string;
    public visionPhone: string;
    public schoolMaster: Object;
    public alertType: string;
    public noteDate: string;
    public medicalNote: string;
    public immunizationType: string;
    public immunizationDate: string;
    public immunizationComment: string;
    public nurseVisitDate: string;
    public reason: string;
    public result: string;
    public nurseComment: string;
}

export class StudentImportModel extends CommonField {
    studentAddViewModelList: StudentMasterImportModel[]
    conflictIndexNo: string;
    createdBy: string;
    constructor() {
        super();
        this.studentAddViewModelList = []
    }
}

export class StudentMasterImportModel {
    studentMaster: any;
    countryOfBirthName: string;
    nationalityName: string;
    firstLanguageName: string;
    secondLanguageName: string;
    thirdLanguageName: string;
    sectionName: string;
    currentGradeLevel: string;
    enrollmentDate: string;
    dob: string;
    estimatedGradDate: string;
    _message: string;
    fieldsCategoryList: FieldsCategoryModel[];
    loginEmail?:string;
    passwordHash?: string;
    constructor() {
        this.studentMaster = null;
        this.fieldsCategoryList = [new FieldsCategoryModel];
        this.loginEmail=null;
        this.passwordHash=null;
    }
}

export class AfterImportStatus {
    totalStudentsSent: number;
    totalStudentsImported: number;
    totalStudentsImportedInPercentage: number;
}

export class StudentName {
    public firstGivenName: string;
    public middleName: string;
    public lastFamilyName: string;
}
export class StudentMedicalModel{
    tenantId: string;
    schoolId: number;
    studentId: number;
    id: number;
    alertType: string;
    noteDate: string;
    medicalNote: string;
    alertDescription: string;
    immunizationType: string;
    immunizationDate: string;
    nurseVisitDate: string;
    timeIn: string;
    timeOut: string;
    reason: string;
    result: string;
    comment: string;
    primaryCarePhysician: string;
    primaryCarePhysicianPhone: string;
    preferredMedicalFacility: string;
    preferredMedicalFacilityPhone: string;
    insuranceCompany: string;
    insuranceCompanyPhone: string;
    policyNumber: string;
    policyHolderName: string;
    dentistName: string;
    dentistPhone: string;
    visionName: string;
    visionProviderPhone: string;
    createdBy: string;
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
    constructor(){
        this.id = 0;
        this.primaryCarePhysician = null;
        this.primaryCarePhysicianPhone = null;
        this.preferredMedicalFacility = null;
        this.preferredMedicalFacilityPhone = null;
        this.insuranceCompany = null;
        this.insuranceCompanyPhone = null;
        this.policyHolderName = null;
        this.policyNumber = null;
        this.dentistName = null;
        this.dentistPhone = null;
        this.visionName = null;
        this.visionProviderPhone = null;
    }
}
export class AddEditStudentMedicalAlertModel extends CommonField {
    studentMedicalAlert: StudentMedicalModel;
    constructor(){
        super();
        this.studentMedicalAlert = new StudentMedicalModel();
    }
}
export class AddEditStudentMedicalNoteModel extends CommonField{
    studentMedicalNote: StudentMedicalModel;
    constructor(){
        super();
        this.studentMedicalNote = new StudentMedicalModel();
    }
}
export class AddEditStudentMedicalImmunizationModel extends CommonField {
    studentMedicalImmunization: StudentMedicalModel;
    constructor(){
        super();
        this.studentMedicalImmunization = new StudentMedicalModel();
    }
}

export class AddEditStudentMedicalNurseVisitModel extends CommonField{
    studentMedicalNurseVisit: StudentMedicalModel;
    constructor(){
        super();
        this.studentMedicalNurseVisit = new StudentMedicalModel();
    }
}

export class AddEditStudentMedicalProviderModel extends CommonField{
    studentMedicalProvider: StudentMedicalModel;
    public fieldsCategoryList: [FieldsCategoryModel];
    selectedCategoryId: number;
    constructor(){
        super();
        this.studentMedicalProvider = new StudentMedicalModel();
        this.studentMedicalProvider.id = 0;
    }
}



export class StudentMedicalInfoListModel extends CommonField{
    public studentMedicalAlertList: [StudentMedicalModel];
    public studentMedicalImmunizationList: [StudentMedicalModel];
    public studentMedicalNoteList: [StudentMedicalModel];
    public studentMedicalNurseVisitList: [StudentMedicalModel];
    public studentMedicalProviderList: [StudentMedicalModel];
    public fieldsCategoryList: [FieldsCategoryModel];
    studentId: number;
    constructor(){
        super();
    }
}

export class OtherStudentTabs{
    courseSchedule: boolean;
    attendance: boolean;
    reportCard: boolean;
    transcript: boolean;
    progressReport: boolean;
    constructor(){
        this.courseSchedule=false;
        this.attendance = false;
        this.reportCard = false;
        this.transcript = false;
        this.progressReport = false;
    }
}

import { AddEditStudentMedicalProviderForGroupAssignModel, AddEditStudentMedicalProviderModel, StudentAddForGroupAssignModel, StudentDocumentAddForGroupAssignModel, StudentEnrollmentForGroupAssignModel, StudentListByDateRangeModel } from './../models/student.model';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import {
  StudentAddModel,
  StudentListModel,
  StudentResponseListModel,
  GetAllStudentDocumentsList,
  StudentDocumentAddModel,
  StudentSiblingSearch,
  StudentSiblingAssociation,
  StudentViewSibling,
  CheckStudentInternalIdViewModel,
  StudentEnrollmentModel,
  StudentEnrollmentSchoolListModel,
  StudentImportModel,
  StudentName,
  StudentMedicalInfoListModel,
  DeleteStudentModel
} from '../models/student.model';
import { StudentCommentsAddForGroupAssign, StudentCommentsAddView, StudentCommentsListViewModel } from '../models/student-comments.model';
import { BehaviorSubject, Subject } from 'rxjs';
import { CryptoService } from './Crypto.service';
import { DefaultValuesService } from '../common/default-values.service';
import { SchoolCreate } from '../enums/school-create.enum';
import { AddEditStudentMedicalAlertModel, AddEditStudentMedicalImmunizationModel, AddEditStudentMedicalNoteModel, AddEditStudentMedicalNurseVisitModel } from '../models/student.model';
import { PageRolesPermission } from '../common/page-roles-permissions.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
@Injectable({
  providedIn: 'root'
})
export class StudentService {
  searchAllSchool:boolean;
  includeInactive:boolean;
  studentCreate = SchoolCreate;
  selectedSchoolId: number;
  apiUrl: string = environment.apiURL;
  private currentYear = new BehaviorSubject(false);
  currentY = this.currentYear.asObservable();
  
  private studentCreateMode = new BehaviorSubject(this.studentCreate.ADD);
  studentCreatedMode = this.studentCreateMode.asObservable();

  private studentDetailsForViewAndEdit = new BehaviorSubject(null);
  studentDetailsForViewedAndEdited = this.studentDetailsForViewAndEdit.asObservable();
  
  private categoryId = new BehaviorSubject(null);
  categoryIdSelected = this.categoryId.asObservable();

  private categoryTitle = new BehaviorSubject(null);
  selectedCatgoryTitle = this.categoryTitle.asObservable();

  private dataAfterSavingGeneralInfo = new BehaviorSubject(0);
  dataAfterSavingGeneralInfoChanged = this.dataAfterSavingGeneralInfo.asObservable();

  private criticalAlertInMedicalInfo = new Subject();
  criticalAlertStatus = this.criticalAlertInMedicalInfo.asObservable();

  studentName: StudentName;
  private isFirstView: boolean = true;
  httpOptions: { headers: any; };
  constructor(
    private http: HttpClient,
    private cryptoService: CryptoService,
    private defaultValuesService: DefaultValuesService,
    private pageRolePermission: PageRolesPermission,
    private router: Router,
    private snackbar: MatSnackBar) {
      this.httpOptions = {
        headers: new HttpHeaders({
          'Cache-Control': 'no-cache',
          'Pragma': 'no-cache',
        })
      }
     }

  redirectToGeneralInfo() {
    let permission = this.pageRolePermission.checkPageRolePermission('/school/students/student-generalinfo', null, true);
    if (!permission.add) {
      this.router.navigate(['/school', 'students']);
      this.snackbar.open('You did not have permission to add student details.', '', {
        duration: 10000
      });
    } else {
      this.router.navigate(['/school', 'students', 'student-generalinfo']);
    }
  }

  redirectToStudentList() {
    this.router.navigate(['/school', 'students']);
    this.snackbar.open(`Didn't have any permisssion to add student in selected academic year.`, '', {
      duration: 30000
    });
  }

  AddStudent(obj: StudentAddModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMaster.tenantId = this.defaultValuesService.getTenantID();
    obj.studentMaster.schoolId = this.defaultValuesService.getSchoolID();
    obj.studentMaster.createdBy = this.defaultValuesService.getUserGuidId();
    obj.passwordHash = this.cryptoService.encrypt(obj.passwordHash);
    const apiurl = this.apiUrl + obj._tenantName + '/Student/addStudent';
    obj.studentMaster.studentPhoto = this.studentImage;
    return this.http.post<StudentAddModel>(apiurl, obj,this.httpOptions);
  }

  AddStudentForGroupAssign(obj: StudentAddForGroupAssignModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMaster.tenantId = this.defaultValuesService.getTenantID();
    obj.studentMaster.schoolId = this.defaultValuesService.getSchoolID();
    obj.studentMaster.createdBy = this.defaultValuesService.getUserGuidId();
    obj.passwordHash = this.cryptoService.encrypt(obj.passwordHash);
    const apiurl = this.apiUrl + obj._tenantName + '/Student/assignGeneralInfoForStudents';
    obj.studentMaster.studentPhoto = this.studentImage;
    return this.http.post<StudentAddForGroupAssignModel>(apiurl, obj,this.httpOptions);
  }

  viewStudent(obj: StudentAddModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMaster.tenantId = this.defaultValuesService.getTenantID();
    obj.studentMaster.schoolId = this.defaultValuesService.getSchoolID();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/viewStudent';
    return this.http.post<StudentAddModel>(apiurl, obj,this.httpOptions);
  }

  UpdateStudent(obj: StudentAddModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMaster.tenantId = this.defaultValuesService.getTenantID();
    obj.studentMaster.schoolId = this.defaultValuesService.getSchoolID();
    obj.studentMaster.updatedBy = this.defaultValuesService.getUserGuidId();
    obj.passwordHash = this.cryptoService.encrypt(obj.passwordHash);
    const apiurl = this.apiUrl + obj._tenantName + '/Student/updateStudent';
    obj.studentMaster.studentPhoto = this.studentImage;
    return this.http.put<StudentAddModel>(apiurl, obj,this.httpOptions);
  }

  DeleteStudent(obj: DeleteStudentModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.tenantId = this.defaultValuesService.getTenantID();
    obj.schoolId = this.defaultValuesService.getSchoolID();
    obj.updatedBy = this.defaultValuesService.getUserGuidId();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/deleteStudent';
    return this.http.post<DeleteStudentModel>(apiurl, obj,this.httpOptions);
  }

  GetAllStudentList(obj: StudentListModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.emailAddress=this.defaultValuesService.getEmailId();
    this.getAllSchoolAndInactive();
    obj.searchAllSchool=this.searchAllSchool;
    obj.includeInactive=this.includeInactive;
    const apiurl = this.apiUrl + obj._tenantName + '/Student/getAllStudentList';
    return this.http.post<StudentResponseListModel>(apiurl, obj,this.httpOptions);
  }

  getAllStudentListByDateRange(obj: StudentListByDateRangeModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.academicYear = this.defaultValuesService.getAcademicYear();
    obj.EmailAddress = this.defaultValuesService.getEmailId();
    obj.markingPeriodStartDate =  this.defaultValuesService.getMarkingPeriodStartDate();
    obj.markingPeriodEndDate =  this.defaultValuesService.getMarkingPeriodEndDate();

    this.getAllSchoolAndInactive();
    obj.searchAllSchool = this.searchAllSchool;
    obj.includeInactive = this.includeInactive;
    const apiurl = this.apiUrl + obj._tenantName + '/Student/getAllStudentListByDateRange';
    return this.http.post<StudentResponseListModel>(apiurl, obj,this.httpOptions);
  }

  checkStudentInternalId(obj: CheckStudentInternalIdViewModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/Student/checkStudentInternalId';
    return this.http.post<CheckStudentInternalIdViewModel>(apiurl, obj,this.httpOptions);
  }

  getAllSchoolAndInactive(){
    this.defaultValuesService.sendAllSchoolFlagSubject.subscribe(data=>{
      this.searchAllSchool=data;
    })
    this.defaultValuesService.sendIncludeFlagSubject.subscribe(data=>{
      this.includeInactive=data;
    })
  }

  private category = new Subject;
  categoryToSend = this.category.asObservable();

  changeCategory(category: number) {
    this.category.next(category);
  }

  private studentDetails;
  setStudentDetails(data) {
    this.studentDetails = data;
  }
  getStudentDetails() {
    return this.studentDetails;
  }

  private studentId: number;
  setStudentId(id: number) {
    this.studentId = id;
  }
  getStudentId() {
    return this.studentId;
  }

  private studentGuid: string;
  setStudentGuid(id: string) {
    this.studentGuid = id;
  }
  getStudentGuid() {
    return this.studentGuid;
  }

  private studentMultiselectValue: any;
  setStudentMultiselectValue(value: any) {
    this.studentMultiselectValue = value;
  }
  getStudentMultiselectValue() {
    return this.studentMultiselectValue;
  }

  private studentImage;
  setStudentImage(imageInBase64) {
    this.studentImage = imageInBase64;
  }

  private studentThumbnailImage;
  setStudentThumbnailImage(imageInBase64) {
    this.studentThumbnailImage = imageInBase64;
  }

  // Update Mode in Student
  public pageMode = new Subject;
  modeToUpdate = this.pageMode.asObservable();

  changePageMode(mode: number) {
    this.pageMode.next(mode);
  }

  // for cancel after Student photo added
  public cloneStudentImage;
  setStudentCloneImage(image) {
    this.cloneStudentImage = image;
  }
  getStudentCloneImage() {
    return this.cloneStudentImage;
  }

  // Student comment
  addStudentComment(obj: StudentCommentsAddView) {
    obj.studentComments.tenantId = this.defaultValuesService.getTenantID();
    obj.studentComments.schoolId = this.defaultValuesService.getSchoolID();
    obj.studentComments.createdBy = this.defaultValuesService.getUserGuidId();
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/Student/addStudentComment';
    return this.http.post<StudentCommentsAddView>(apiurl, obj,this.httpOptions);
  }
  updateStudentComment(obj: StudentCommentsAddView) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentComments.tenantId = this.defaultValuesService.getTenantID();
    obj.studentComments.schoolId = this.defaultValuesService.getSchoolID();
    obj.studentComments.updatedBy = this.defaultValuesService.getUserGuidId();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/updateStudentComment';
    return this.http.put<StudentCommentsAddView>(apiurl, obj,this.httpOptions);
  }
  deleteStudentComment(obj: StudentCommentsAddView) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentComments.tenantId = this.defaultValuesService.getTenantID();
    obj.studentComments.schoolId = this.defaultValuesService.getSchoolID();
    obj.studentComments.updatedBy = this.defaultValuesService.getUserGuidId();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/deleteStudentComment';
    return this.http.post<StudentCommentsAddView>(apiurl, obj,this.httpOptions);
  }
  getAllStudentCommentsList(obj: StudentCommentsListViewModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/Student/getAllStudentCommentsList';
    return this.http.post<StudentCommentsListViewModel>(apiurl, obj,this.httpOptions);
  }

  addStudentCommentForGroupAssign(obj: StudentCommentsAddForGroupAssign) {
    obj.studentComments.tenantId = this.defaultValuesService.getTenantID();
    obj.studentComments.schoolId = this.defaultValuesService.getSchoolID();
    obj.studentComments.updatedBy = this.defaultValuesService.getUserName();
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/Student/assignCommentForStudents';
    return this.http.post<StudentCommentsAddForGroupAssign>(apiurl, obj,this.httpOptions);
  }

  // to Update student details in General for first time.
  private studentDetailsForGeneralInfo = new Subject();
  getStudentDetailsForGeneral = this.studentDetailsForGeneralInfo.asObservable();

  sendDetails(studentDetailsForGeneralInfo) {
    this.studentDetailsForGeneralInfo.next(studentDetailsForGeneralInfo);
  }

  GetAllStudentDocumentsList(obj: GetAllStudentDocumentsList) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/Student/getAllStudentDocumentsList';
    return this.http.post<GetAllStudentDocumentsList>(apiurl, obj,this.httpOptions);
  }
  AddStudentDocument(obj: StudentDocumentAddModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/Student/addStudentDocument';
    return this.http.post<StudentDocumentAddModel>(apiurl, obj,this.httpOptions);
  }
  updateStudentDocument(obj: StudentDocumentAddModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/Student/updateStudentDocument';
    return this.http.put<StudentDocumentAddModel>(apiurl, obj,this.httpOptions);
  }
  DeleteStudentDocument(obj: StudentDocumentAddModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/Student/deleteStudentDocument';
    return this.http.post<StudentDocumentAddModel>(apiurl, obj,this.httpOptions);
  }

  AddStudentDocumentForGroupAssign(obj: StudentDocumentAddForGroupAssignModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/Student/assignDocumentForStudents';
    return this.http.post<StudentDocumentAddForGroupAssignModel>(apiurl, obj,this.httpOptions);
  }

  // Student Sibling
  siblingSearch(searchDetails: StudentSiblingSearch,schoolId) {
    searchDetails= this.defaultValuesService.getAllMandatoryVariable(searchDetails);
    searchDetails.schoolId= schoolId;
    const apiurl = this.apiUrl + searchDetails._tenantName + '/Student/siblingSearch';
    return this.http.post<StudentSiblingSearch>(apiurl, searchDetails,this.httpOptions);
  }
  associationSibling(studentDetails: StudentSiblingAssociation) {
    studentDetails = this.defaultValuesService.getAllMandatoryVariable(studentDetails);
    studentDetails.studentMaster.tenantId = this.defaultValuesService.getTenantID();
    const apiurl = this.apiUrl + studentDetails._tenantName + '/Student/associationSibling';
    return this.http.post<StudentSiblingAssociation>(apiurl, studentDetails,this.httpOptions);
  }
  viewSibling(studentDetails: StudentViewSibling) {
    studentDetails = this.defaultValuesService.getAllMandatoryVariable(studentDetails);
    const apiurl = this.apiUrl + studentDetails._tenantName + '/Student/viewSibling';
    return this.http.post<StudentViewSibling>(apiurl, studentDetails,this.httpOptions);
  }
  removeSibling(studentDetails: StudentSiblingAssociation) {
    studentDetails = this.defaultValuesService.getAllMandatoryVariable(studentDetails);
    studentDetails.studentMaster.tenantId = this.defaultValuesService.getTenantID();
    const apiurl = this.apiUrl + studentDetails._tenantName + '/Student/removeSibling';
    return this.http.post<StudentSiblingAssociation>(apiurl, studentDetails,this.httpOptions);
  }

  // Student Enrollment

  updateStudentEnrollment(studentDetails: StudentEnrollmentModel) {
    studentDetails = this.defaultValuesService.getAllMandatoryVariable(studentDetails);
    const apiurl = this.apiUrl + studentDetails._tenantName + '/Student/updateStudentEnrollment';
    return this.http.put<StudentEnrollmentModel>(apiurl, studentDetails,this.httpOptions);
  }

  studentEnrollmentForGroupAssign(studentDetails: StudentEnrollmentForGroupAssignModel) {
    studentDetails = this.defaultValuesService.getAllMandatoryVariable(studentDetails);
    const apiurl = this.apiUrl + studentDetails._tenantName + '/Student/AssignEnrollmentInfoForStudents';
    return this.http.post<StudentEnrollmentForGroupAssignModel>(apiurl, studentDetails,this.httpOptions);
  }

  getAllStudentEnrollment(studentDetails: StudentEnrollmentModel) {
    studentDetails = this.defaultValuesService.getAllMandatoryVariable(studentDetails);
    const apiurl = this.apiUrl + studentDetails._tenantName + '/Student/getAllStudentEnrollment';
    return this.http.post<StudentEnrollmentModel>(apiurl, studentDetails,this.httpOptions);
  }

  studentEnrollmentSchoolList(schoolDetails: StudentEnrollmentSchoolListModel) {
    schoolDetails = this.defaultValuesService.getAllMandatoryVariable(schoolDetails);
    const apiurl = this.apiUrl + schoolDetails._tenantName + '/School/studentEnrollmentSchoolList';
    return this.http.post<StudentEnrollmentSchoolListModel>(apiurl, schoolDetails,this.httpOptions);
  }

  addUpdateStudentPhoto(obj: StudentAddModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMaster.studentId = this.getStudentId();
    obj.studentMaster.schoolId = this.defaultValuesService.getSchoolID();
    obj.studentMaster.updatedBy = this.defaultValuesService.getUserGuidId();
    obj.studentMaster.studentPhoto = this.studentImage;
    obj.studentMaster.studentThumbnailPhoto = this.studentThumbnailImage;
    
    const apiurl = this.apiUrl + obj._tenantName + '/Student/addUpdateStudentPhoto';
    return this.http.put<StudentAddModel>(apiurl, obj,this.httpOptions);
  }

  searchStudentListForReenroll(obj: StudentListModel, schoolId) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.schoolId = +schoolId;
    const apiurl = this.apiUrl + obj._tenantName + '/Student/searchStudentListForReenroll';
    return this.http.post<StudentResponseListModel>(apiurl, obj,this.httpOptions);
  }

  reenrollmentForStudent(obj: StudentListModel) {
    const schoolId = this.selectedSchoolId;
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.schoolId = schoolId;
    obj.updatedBy = this.defaultValuesService.getUserGuidId();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/reenrollmentForStudent';
    return this.http.post<StudentListModel>(apiurl, obj,this.httpOptions);
  }
  addStudentList(obj: StudentImportModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.createdBy = this.defaultValuesService.getUserGuidId();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/addStudentList';
    return this.http.post<StudentImportModel>(apiurl, obj,this.httpOptions);
  }
  addStudentMedicalAlert(obj: AddEditStudentMedicalAlertModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMedicalAlert.tenantId = this.defaultValuesService.getTenantID();
    obj.studentMedicalAlert.schoolId = this.defaultValuesService.getSchoolID();
    obj.studentMedicalAlert.createdBy = this.defaultValuesService.getUserGuidId();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/addStudentMedicalAlert';
    return this.http.post<AddEditStudentMedicalAlertModel>(apiurl, obj,this.httpOptions);
  }
  updateStudentMedicalAlert(obj: AddEditStudentMedicalAlertModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMedicalAlert.tenantId = this.defaultValuesService.getTenantID();
    obj.studentMedicalAlert.schoolId = this.defaultValuesService.getSchoolID();
    obj.studentMedicalAlert.updatedBy = this.defaultValuesService.getUserGuidId();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/updateStudentMedicalAlert';
    return this.http.put<AddEditStudentMedicalAlertModel>(apiurl, obj,this.httpOptions);
  }
  deleteStudentMedicalAlert(obj: AddEditStudentMedicalAlertModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMedicalAlert.tenantId = this.defaultValuesService.getTenantID();
    obj.studentMedicalAlert.schoolId = this.defaultValuesService.getSchoolID();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/deleteStudentMedicalAlert';
    return this.http.post<AddEditStudentMedicalAlertModel>(apiurl, obj,this.httpOptions);
  }
  addStudentMedicalNote(obj: AddEditStudentMedicalNoteModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMedicalNote.tenantId = this.defaultValuesService.getTenantID();
    obj.studentMedicalNote.schoolId = this.defaultValuesService.getSchoolID();
    obj.studentMedicalNote.createdBy = this.defaultValuesService.getUserGuidId();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/addStudentMedicalNote';
    return this.http.post<AddEditStudentMedicalNoteModel>(apiurl, obj,this.httpOptions);
  }
  updateStudentMedicalNote(obj: AddEditStudentMedicalNoteModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMedicalNote.tenantId = this.defaultValuesService.getTenantID();
    obj.studentMedicalNote.schoolId = this.defaultValuesService.getSchoolID();
    obj.studentMedicalNote.updatedBy = this.defaultValuesService.getUserGuidId();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/updateStudentMedicalNote';
    return this.http.put<AddEditStudentMedicalNoteModel>(apiurl, obj,this.httpOptions);
  }
  deleteStudentMedicalNote(obj: AddEditStudentMedicalNoteModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMedicalNote.tenantId = this.defaultValuesService.getTenantID();
    obj.studentMedicalNote.schoolId = this.defaultValuesService.getSchoolID();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/deleteStudentMedicalNote';
    return this.http.post<AddEditStudentMedicalNoteModel>(apiurl, obj,this.httpOptions);
  }
  addStudentMedicalImmunization(obj: AddEditStudentMedicalImmunizationModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMedicalImmunization.tenantId = this.defaultValuesService.getTenantID();
    obj.studentMedicalImmunization.schoolId = this.defaultValuesService.getSchoolID();
    obj.studentMedicalImmunization.createdBy = this.defaultValuesService.getUserGuidId();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/addStudentMedicalImmunization';
    return this.http.post<AddEditStudentMedicalImmunizationModel>(apiurl, obj,this.httpOptions);
  }
  updateStudentMedicalImmunization(obj: AddEditStudentMedicalImmunizationModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMedicalImmunization.tenantId = this.defaultValuesService.getTenantID();
    obj.studentMedicalImmunization.schoolId = this.defaultValuesService.getSchoolID();
    obj.studentMedicalImmunization.updatedBy = this.defaultValuesService.getUserGuidId();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/updateStudentMedicalImmunization';
    return this.http.put<AddEditStudentMedicalImmunizationModel>(apiurl, obj,this.httpOptions);
  }
  deleteStudentMedicalImmunization(obj: AddEditStudentMedicalImmunizationModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMedicalImmunization.tenantId = this.defaultValuesService.getTenantID();
    obj.studentMedicalImmunization.schoolId = this.defaultValuesService.getSchoolID();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/deleteStudentMedicalImmunization';
    return this.http.post<AddEditStudentMedicalImmunizationModel>(apiurl, obj,this.httpOptions);
  }
  addStudentMedicalNurseVisit(obj: AddEditStudentMedicalNurseVisitModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMedicalNurseVisit.tenantId = this.defaultValuesService.getTenantID();
    obj.studentMedicalNurseVisit.schoolId = this.defaultValuesService.getSchoolID();
    obj.studentMedicalNurseVisit.createdBy = this.defaultValuesService.getUserGuidId();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/addStudentMedicalNurseVisit';
    return this.http.post<AddEditStudentMedicalNurseVisitModel>(apiurl, obj,this.httpOptions);
  }
  updateStudentMedicalNurseVisit(obj: AddEditStudentMedicalNurseVisitModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMedicalNurseVisit.tenantId = this.defaultValuesService.getTenantID();
    obj.studentMedicalNurseVisit.schoolId = this.defaultValuesService.getSchoolID();
    obj.studentMedicalNurseVisit.updatedBy = this.defaultValuesService.getUserGuidId();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/updateStudentMedicalNurseVisit';
    return this.http.put<AddEditStudentMedicalNurseVisitModel>(apiurl, obj,this.httpOptions);
  }
  deleteStudentMedicalNurseVisit(obj: AddEditStudentMedicalNurseVisitModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMedicalNurseVisit.tenantId = this.defaultValuesService.getTenantID();
    obj.studentMedicalNurseVisit.schoolId = this.defaultValuesService.getSchoolID();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/deleteStudentMedicalNurseVisit';
    return this.http.post<AddEditStudentMedicalNurseVisitModel>(apiurl, obj,this.httpOptions);
  }
  addStudentMedicalProvider(obj: AddEditStudentMedicalProviderModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMedicalProvider.tenantId = this.defaultValuesService.getTenantID();
    obj.studentMedicalProvider.schoolId = this.defaultValuesService.getSchoolID();
    obj.studentMedicalProvider.createdBy = this.defaultValuesService.getUserGuidId();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/addStudentMedicalProvider';
    return this.http.post<AddEditStudentMedicalProviderModel>(apiurl, obj,this.httpOptions);
  }
  addStudentMedicalProviderForGroupAssign(obj: AddEditStudentMedicalProviderForGroupAssignModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMedicalProvider.tenantId = this.defaultValuesService.getTenantID();
    obj.studentMedicalProvider.schoolId = this.defaultValuesService.getSchoolID();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/AssignMedicalInfoForStudents';
    return this.http.post<AddEditStudentMedicalProviderForGroupAssignModel>(apiurl, obj,this.httpOptions);
  }
  updateStudentMedicalProvider(obj: AddEditStudentMedicalProviderModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMedicalProvider.tenantId = this.defaultValuesService.getTenantID();
    obj.studentMedicalProvider.schoolId = this.defaultValuesService.getSchoolID();
    obj.studentMedicalProvider.updatedBy = this.defaultValuesService.getUserGuidId();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/updateStudentMedicalProvider';
    return this.http.put<AddEditStudentMedicalProviderModel>(apiurl, obj,this.httpOptions);
  }
  deleteStudentMedicalProvider(obj: AddEditStudentMedicalProviderModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentMedicalProvider.tenantId = this.defaultValuesService.getTenantID();
    obj.studentMedicalProvider.schoolId = this.defaultValuesService.getSchoolID();
    const apiurl = this.apiUrl + obj._tenantName + '/Student/deleteStudentMedicalProvider';
    return this.http.post<AddEditStudentMedicalProviderModel>(apiurl, obj,this.httpOptions);
  }
  getAllStudentMedicalInfo(obj: StudentMedicalInfoListModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/Student/getAllStudentMedicalInfo';
    return this.http.post<StudentMedicalInfoListModel>(apiurl, obj,this.httpOptions);
  }

  setStudentName(studentName) {
    this.studentName = {
      firstGivenName: studentName.split('|')[0],
      middleName: studentName.split('|')[1],
      lastFamilyName: studentName.split('|')[2]
    };
  }
  getStudentName() {
    return this.studentName;
  }

  setStudentFirstView(status){
    this.isFirstView = status;
  }
  getStudentFirstView(){
    return this.isFirstView;
  }

  setSelectedSchoolId(data: number) {
    this.selectedSchoolId = data;
  }

  setStudentCreateMode(data) {
    this.studentCreateMode.next(data);
  }

  setStudentDetailsForViewAndEdit(data) {
    this.studentDetailsForViewAndEdit.next(data);
  }

  setCategoryId(data) {
    this.categoryId.next(data);
  }

  setCategoryTitle(title: string){
    this.categoryTitle.next(title);
  }
  setDataAfterSavingGeneralInfo(data) {
    this.dataAfterSavingGeneralInfo.next(data);
  }

  setStudentCriticalInfo(data) {
    this.criticalAlertInMedicalInfo.next(data);
  }

}

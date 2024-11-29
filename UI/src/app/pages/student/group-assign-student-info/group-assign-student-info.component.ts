import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icRemoveCircle from '@iconify/icons-ic/twotone-remove-circle';
import { ControlContainer, FormBuilder, FormControl, FormGroup, NgForm, Validators } from '@angular/forms';
import { AddEditStudentMedicalProviderForGroupAssignModel, StudentAddForGroupAssignModel, StudentDocumentAddForGroupAssignModel, StudentEnrollmentForGroupAssignModel, StudentListModel } from 'src/app/models/student.model';
import { StudentService } from '../../../services/student.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DefaultValuesService } from '../../../common/default-values.service';
import { LoaderService } from '../../../services/loader.service';
import { debounceTime, distinctUntilChanged, take, takeUntil } from 'rxjs/operators';
import { ReplaySubject, Subject } from 'rxjs';
import { MatSort } from '@angular/material/sort';
import { ScheduleStudentListViewModel } from '../../../models/student-schedule.model';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms, stagger60ms } from 'src/@vex/animations/stagger.animation';
import { fadeInRight400ms } from 'src/@vex/animations/fade-in-right.animation';
import { SearchFilter, SearchFilterListViewModel } from '../../../models/search-filter.model';
import { CommonService } from '../../../services/common.service';
import { MatSelect } from '@angular/material/select';
import { LanguageModel } from '../../../models/language.model';
import { LoginService } from '../../../services/login.service';
import { SchoolCreate } from '../../../enums/school-create.enum';
import { MiscModel } from '../../../models/misc-data-student.model';
import { CountryModel } from '../../../models/country.model';
import { CalendarService } from '../../../services/calendar.service';
import { CalendarListModel } from '../../../models/calendar.model';
import { RollingOptionsEnum } from '../../../enums/rolling-retention-option.enum';
import { GetAllSectionModel } from '../../../models/section.model';
import { SectionService } from '../../../services/section.service';
import { StudentCommentsAddForGroupAssign } from '../../../models/student-comments.model';
import { EditorChangeContent, EditorChangeSelection } from 'ngx-quill';
import { CommonLOV } from '../../shared-module/lov/common-lov';
import { SharedFunction } from '../../shared/shared-function';
import { ImageCropperService } from '../../../services/image-cropper.service';
import { ModuleIdentifier } from '../../../enums/module-identifier.enum';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { FieldsCategoryListView, FieldsCategoryModel } from '../../../models/fields-category.model';
import { CustomFieldService } from '../../../services/custom-field.service';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { MY_FORMATS } from '../../shared/format-datepicker';
import { CustomFieldsValueModel } from '../../../models/custom-fields-value.model';
import { StudentScheduleService } from '../../../services/student-schedule.service';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { MatCheckbox } from '@angular/material/checkbox';
import { Module } from 'src/app/enums/module.enum';
import { ProfilesTypes } from '../../../enums/profiles.enum';
import { GradeLevelService } from 'src/app/services/grade-level.service';
import { GetAllGradeLevelsModel } from 'src/app/models/grade-level.model';
import { AdvancedSearchExpansionModel } from 'src/app/models/common.model';

export interface StudentList {
  selectStudent: boolean;
  studentName: string;
  studentId: number;
  alternateId: string;
  grade: string;
  section: string;
  email: string;
}

export interface SelectedStudentList {
  remove: string;
  studentName: string;
  studentId: number;
  alternateId: string;
  gradeLevel: string;
  section: string;
  email: string;
}



@Component({
  selector: 'vex-group-assign-student-info',
  templateUrl: './group-assign-student-info.component.html',
  styleUrls: ['./group-assign-student-info.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms,
    stagger60ms
  ],
  
})

export class GroupAssignStudentInfoComponent implements OnInit, OnDestroy{

  icRemoveCircle = icRemoveCircle;
  currentTab: string = 'generalInfo';
  form: FormGroup;
  groupAssignStudents = true;
  showSelectedStudent = false;
  isShow = false;
  getAllStudent: StudentListModel = new StudentListModel();
  selectedStudentListModel;
  selectedStudentList: MatTableDataSource<any>;
  @ViewChild('f') currentForm: NgForm;
  f: NgForm;
  totalCount: number = 0;
  StudentModelList: MatTableDataSource<any>;
  pageNumber: number;
  pageSize: number;
  searchCount: number = null;
  searchValue: any = null;
  destroySubject$: Subject<void> = new Subject();
  @ViewChild(MatSort) sort: MatSort;
  scheduleStudentListViewModel: ScheduleStudentListViewModel = new ScheduleStudentListViewModel();

  searchCtrl: FormControl;
  showAdvanceSearchPanel: boolean = false;
  filterJsonParams;
  showSaveFilter: boolean = false;
  searchFilterListViewModel: SearchFilterListViewModel = new SearchFilterListViewModel();
  searchFilter: SearchFilter = new SearchFilter();
  showLoadFilter = true;
  toggleValues: any = null;
  studentDocument = [];
  fieldsCategoryListView = new FieldsCategoryListView();
  // @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild('studentsPaginator') paginator: MatPaginator;

  @ViewChild('selectedStudentsPaginator') selectedStudentsPaginator: MatPaginator;
  @ViewChild('masterCheckBox') masterCheckBox: MatCheckbox;


  columns = [
    { label: 'selectStudent', property: 'selectStudent', type: 'text', visible: true },
    { label: 'Name', property: 'firstGivenName', type: 'text', visible: true },
    { label: 'Student ID', property: 'studentInternalId', type: 'text', visible: true },
    { label: 'Alternate ID', property: 'alternateId', type: 'text', visible: true },
    { label: 'Grade', property: 'gradeLevelTitle', type: 'text', visible: true },
    { label: 'Section', property: 'section', type: 'text', visible: true },
    { label: 'Email', property: 'schoolEmail', type: 'text', visible: true },
  ];
  displayedColumnsStudents: string[] = ['remove', 'firstGivenName', 'studentInternalId', 'alternateId', 'gradeLevelTitle', 'section', 'schoolEmail'];
  loading: boolean;
  studentAddForGroupAssignModel: StudentAddForGroupAssignModel = new StudentAddForGroupAssignModel();
  today = new Date();
  nationalityFilterCtrl: FormControl = new FormControl();
  protected _onDestroy = new Subject<void>();
  countryOfBirthFilterCtrl: FormControl = new FormControl();
  countryListArr = [];
  public filteredNationality: ReplaySubject<any> = new ReplaySubject<any>(1);
  public filteredcountryOfBirth: ReplaySubject<any> = new ReplaySubject<any>(1);
  @ViewChild('singleSelect') singleSelect: MatSelect;
  languages: LanguageModel = new LanguageModel();
  languageList;
  studentCreateMode;
  studentCreate = SchoolCreate;
  data;
  nameOfMiscValuesForView: MiscModel = new MiscModel(); // This Object contains Section Name, Nationality, Country, languages for View Mode.
  countryModel: CountryModel = new CountryModel();
  studentEnrollmentForGroupAssignModel: StudentEnrollmentForGroupAssignModel =  new StudentEnrollmentForGroupAssignModel();
  calendarListModel: CalendarListModel = new CalendarListModel();
  rollingOptions = Object.keys(RollingOptionsEnum);
  sectionList: GetAllSectionModel = new GetAllSectionModel();
  AddEditStudentMedicalProviderForGroupAssignModel: AddEditStudentMedicalProviderForGroupAssignModel =  new AddEditStudentMedicalProviderForGroupAssignModel();
  StudentCommentsAddForGroupAssign: StudentCommentsAddForGroupAssign = new StudentCommentsAddForGroupAssign();
  StudentDocumentAddForGroupAssignModel: StudentDocumentAddForGroupAssignModel = new StudentDocumentAddForGroupAssignModel();
  getAllGradeLevelsModel: GetAllGradeLevelsModel = new GetAllGradeLevelsModel();
  advancedSearchExpansionModel: AdvancedSearchExpansionModel = new AdvancedSearchExpansionModel();
  gradeLevelList = [];
  moduleIdentifier = ModuleIdentifier;
  profiles=ProfilesTypes;
  body: string;
  cloneFiles: File[] = [];
  filesName = [];
  filesType = [];
  base64Arr = [];
  files: File[] = [];
  genderList = [];
  raceList = [];
  suffixList = [];
  salutationList = [];
  maritalStatusList = [];
  ethnicityList = [];
  countryOfBirthCtrl: FormControl = new FormControl();
  nationalityCtrl: FormControl = new FormControl();
  fieldsCategory = [];
  studentCustomFields=[];
  studentMedicalCustomFields=[];
  studentMultiSelectValue;
  isFromAdvancedSearch: boolean = false
  filterParameters = [];

  constructor(
    public translateService: TranslateService,
    private studentService: StudentService,
    private snackbar: MatSnackBar,
    private defaultValuesService: DefaultValuesService,
    private loaderService: LoaderService,
    private commonService: CommonService,
    private loginService: LoginService,
    private calendarService: CalendarService,
    private sectionService: SectionService,
    private fb: FormBuilder,
    private commonLOV: CommonLOV,
    private commonFunction: SharedFunction,
    private imageCropperService: ImageCropperService,
    private pageRolePermission: PageRolesPermission,
    private customFieldservice: CustomFieldService,
    private studentScheduleService: StudentScheduleService,
    private gradeLevelService: GradeLevelService,
    private paginatorObj: MatPaginatorIntl,
    ) {
      this.advancedSearchExpansionModel.searchAllSchools = false;
      paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
      this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
      //translateService.use('en');
      this.getAllStudent.filterParams = null;
      
      this.studentCreateMode = this.studentCreate.EDIT
      this.selectedStudentListModel = [];
      this.selectedStudentList = new MatTableDataSource([]);
      this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
        this.loading = val;
      });
    // translateService.use('en');
    this.callAllStudent();
    this.GetAllLanguage();
    this.getAllCountry();
    this.getAllCalendar();
    this.getAllSection();
    this.getAllGradeLevelList();
    this.getAllFieldsCategory();

    this.form = fb.group({
      commentId: [0],
      Body: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.searchCtrl = new FormControl();
    this.commonLOV.getLovByName('Gender').pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.genderList = res;
    });
    this.commonLOV.getLovByName('Race').pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.raceList = res;
    });
    this.commonLOV.getLovByName('Ethnicity').pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.ethnicityList = res;
    });
    this.commonLOV.getLovByName('Marital Status').pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.maritalStatusList = res;
    });
    // this.currentTab = 'generalInfo';
    // this.form = new FormGroup({
    //   'editor': new FormControl(null)
    // })

    this.getAllSearchFilter();
    
  }


  ngAfterViewInit() {

    this.countryOfBirthValueChange();
    this.nationalityValueChange();

    this.selectedStudentList.paginator = this.selectedStudentsPaginator;

      //  Sorting
      this.getAllStudent = new StudentListModel();
      this.sort.sortChange.subscribe((res) => {
        this.getAllStudent.pageNumber = this.pageNumber
        this.getAllStudent.pageSize = this.pageSize;

        this.getAllStudent.sortingModel.sortColumn = res.active;
        if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
          let filterParams = [
            {
              columnName: null,
              filterValue: this.searchCtrl.value,
              filterOption: 3
            }
          ]
          Object.assign(this.getAllStudent, { filterParams: filterParams });
        }
        if (res.direction == "") {
          this.getAllStudent.sortingModel = null;

          this.callAllStudent();

          this.getAllStudent = new StudentListModel();
          this.getAllStudent.sortingModel = null;
        } else {
          this.getAllStudent.sortingModel.sortDirection = res.direction;

          this.callAllStudent();

        }
      });
      //  Searching
      this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
        if (term.trim().length > 0) {
          let filterParams = [
            {
              columnName: null,
              filterValue: term,
              filterOption: 3
            }
          ]
          if (this.sort.active != undefined && this.sort.direction != "") {
            this.getAllStudent.sortingModel.sortColumn = this.sort.active;
            this.getAllStudent.sortingModel.sortDirection = this.sort.direction;
          }
          Object.assign(this.getAllStudent, { filterParams: filterParams });
          this.getAllStudent.pageNumber = 1;
          this.paginator.pageIndex = 0;
          this.getAllStudent.pageSize = this.pageSize;

          this.callAllStudent();

        }
        else {
          Object.assign(this.getAllStudent, { filterParams: null });
          this.getAllStudent.pageNumber = this.paginator.pageIndex + 1;
          this.getAllStudent.pageSize = this.pageSize;
          if (this.sort.active != undefined && this.sort.direction != "") {
            this.getAllStudent.sortingModel.sortColumn = this.sort.active;
            this.getAllStudent.sortingModel.sortDirection = this.sort.direction;
          }

          this.callAllStudent();

        }
      })
  }

  changedEditor(event: EditorChangeContent | EditorChangeSelection) {
    if (event.source === 'user') {
      this.body = document.querySelector('.ql-editor').innerHTML;
    }
  }

  submitComment(){
    if (this.form.controls.Body.hasError('required')){
      this.form.controls.Body.markAllAsTouched();
    }
    if (this.form.valid){
      if (this.form.controls.commentId.value === 0){
        // this.StudentCommentsAddForGroupAssign.studentComments.studentId = this.data.studentId;
        this.StudentCommentsAddForGroupAssign.studentComments.commentId = this.form.controls.commentId.value;
        this.StudentCommentsAddForGroupAssign.studentComments.comment = this.form.controls.Body.value;

        this.studentService.addStudentCommentForGroupAssign(this.StudentCommentsAddForGroupAssign).subscribe(
          (res: StudentCommentsAddForGroupAssign) => {
            if (res){
            if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
                this.snackbar.open(res._message, '', {
                  duration: 10000
                });
              }
              else {
                this.snackbar.open('Information has been saved for selected students.', '', {
                  duration: 10000
                });
              }
            }
            else{
              this.snackbar.open(this.defaultValuesService.translateKey('commentFailed') + this.defaultValuesService.getHttpError(), '', {
                duration: 10000
              });
            }
          }
        );
      }
    }
  }



  onSelect(event) {
    this.files.push(...event.addedFiles);

    this.cloneFiles.push(...event.addedFiles)
    for(let i=0; i<this.cloneFiles.length;i++){
      this.filesName.push(this.cloneFiles[i].name);
      this.filesType.push(this.cloneFiles[i].type);
      const reader = new FileReader();
      reader.readAsDataURL(this.cloneFiles[i]);
      reader.onload = () => {
        this.HandleReaderLoaded(reader.result);
      };
    }
    this.cloneFiles = [];
  }

  uploadFile(){
    this.base64Arr.forEach((value, index) => {
        let obj = {};
        obj = {
            tenantId: this.defaultValuesService.getTenantID(),
            schoolId: this.defaultValuesService.getSchoolID(),
            documentId: 0,
            filename: this.filesName[index],
            filetype: this.filesType[index],
            fileUploaded: value,
            uploadedBy: this.defaultValuesService.getUserGuidId(),
            studentMaster: null
          };
        this.studentDocument.push(obj);
      });

    if (this.studentDocument.length > 0){

        this.StudentDocumentAddForGroupAssignModel.studentDocuments = this.studentDocument;
        // return;

        this.studentService.AddStudentDocumentForGroupAssign(this.StudentDocumentAddForGroupAssignModel).subscribe(data => {
          if (data){
           if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
              this.snackbar.open( data._message, '', {
                duration: 10000
              });
            } else {
              this.snackbar.open('Information has been saved for selected students.', '', {
                duration: 10000
              }).afterOpened().subscribe(() => {
                this.base64Arr = [];
                this.studentDocument = [];
                this.filesName = [];
                this.filesType = [];
                this.files = [];
              });
            }
          }else{
            this.snackbar.open(this.defaultValuesService.translateKey('studentDocumentUploadfailed') + this.defaultValuesService.getHttpError(), '', {
              duration: 10000
            });
          }
        });
      }
      // else{
      //   this.snackbar.open(this.defaultValuesService.translateKey('pleaseSelectFile'), '', {
      //     duration: 1000
      //   });
      // }

  }

  submitMedicalInfo() {
    this.studentService.addStudentMedicalProviderForGroupAssign(this.AddEditStudentMedicalProviderForGroupAssignModel).subscribe(
      (res) => {
        if (res){
           if (res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.studentCreateMode = this.studentCreate.VIEW;
          }else{
            this.studentCreateMode = this.studentCreate.VIEW;
            this.snackbar.open('Information has been saved for selected students.', '', {
              duration: 10000
            })
          }
        }
        else{
          this.snackbar.open( this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
  }

  onRemove(event) {
    this.files.splice(this.files.indexOf(event), 1);
    this.filesName.splice(this.files.indexOf(event), 1);
    this.filesType.splice(this.files.indexOf(event), 1);
    this.base64Arr.splice(this.files.indexOf(event), 1);
  }

  HandleReaderLoaded(e) {
    const str = e.substr(e.indexOf(',') + 1);
    this.base64Arr.push(str);
  }

  getAllCalendar() {
    this.calendarService.getAllCalendar(this.calendarListModel).subscribe((res) => {
      const allCalendarsInCurrentSchool = res.calendarList;
      this.calendarListModel.calendarList = allCalendarsInCurrentSchool.filter((x) => {
        return (x.academicYear === +this.defaultValuesService.getAcademicYear() && x.defaultCalender);
      });
      // this.manipulateModelInEditMode();
    });

  }

  getAllSection() {
    if (!this.sectionList.isSectionAvailable) {
      this.sectionList.isSectionAvailable = true;
      this.sectionService.GetAllSection(this.sectionList).pipe(takeUntil(this.destroySubject$)).subscribe(res => {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          if (!res.tableSectionsList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        }
        else {
          this.sectionList.tableSectionsList = res.tableSectionsList;
          if (this.studentCreateMode === this.studentCreate.VIEW) {
          }
        }
      });
    }
  }

  selectGrade(grade) {
    this.studentEnrollmentForGroupAssignModel.studentEnrollments.gradeLevelTitle = grade.title;
  }

  // For get all grade level list
  getAllGradeLevelList() {
    this.gradeLevelService.getAllGradeLevels(this.getAllGradeLevelsModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.gradeLevelList = [];
          if (!data.tableGradelevelList) {
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
        } else {
          this.gradeLevelList = data.tableGradelevelList;
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }
  

  nationalityValueChange(){
    this.nationalityFilterCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe((res) => {
        this.filterNationalitis();
      });
  }
  countryOfBirthValueChange(){
    this.countryOfBirthFilterCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe((res) => {
        this.filterCountryOfBirth();
      });
  }

  filterNationalitis(){
    if (!this.countryListArr) {
      return;
    }
    let search = this.nationalityFilterCtrl.value;
    if (!search) {
      this.filteredNationality.next(this.countryListArr.slice());
      return;
    }
    else {
      search = search.toLowerCase();
    }
    this.filteredNationality.next(
      this.countryListArr.filter(country => country.name.toLowerCase().indexOf(search) > -1)
    );
  }
  filterCountryOfBirth(){
    if (!this.countryListArr) {
      return;
    }
    let search = this.countryOfBirthFilterCtrl.value;
    if (!search) {
      this.filteredcountryOfBirth.next(this.countryListArr.slice());
      return;
    }
    else {
      search = search.toLowerCase();
    }
    this.filteredcountryOfBirth.next(
      this.countryListArr.filter(country => country.name.toLowerCase().indexOf(search) > -1)
    );
  }

  protected setInitialValue() {
    this.filteredNationality
    .pipe(take(1), takeUntil(this._onDestroy))
    .subscribe(() => {
      this.singleSelect.compareWith = (a, b) => a && b && a.name === b.name;
    });
    this.filteredcountryOfBirth
    .pipe(take(1), takeUntil(this._onDestroy))
    .subscribe(() => {
      this.singleSelect.compareWith = (a, b) => a && b && a.name === b.name;
    });
  }

  GetAllLanguage() {
    if (!this.languages.isLanguageAvailable) {
      this.languages.isLanguageAvailable = true;
      this.languages._tenantName = this.defaultValuesService.getTenantName();
      this.loginService.getAllLanguage(this.languages).pipe(takeUntil(this.destroySubject$)).subscribe(
        (res) => {
          if (res) {
           if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
              this.languageList = [];
              if(!res.tableLanguage){
                this.snackbar.open(res._message, '', {
                  duration: 10000
                });
              }
            }else{
            this.languageList = res.tableLanguage?.sort((a, b) => a.locale < b.locale ? -1 : 1);
            if (this.studentCreateMode === this.studentCreate.VIEW) {
              // this.findLanguagesById();
            }
          }
          }
          else {
            this.languageList = [];
          }
        }
      );
    }
  }

  findLanguagesById() {
    this.languageList.map((val) => {
      const firstLanguageId = + this.data.firstLanguageId;
      const secondLanguageId = + this.data.secondLanguageId;
      const thirdLanguageId = + this.data.thirdLanguageId;

      if (val.langId === firstLanguageId) {
        this.nameOfMiscValuesForView.firstLanguage = val.locale;
      }
      if (val.langId === secondLanguageId) {
        this.nameOfMiscValuesForView.secondLanguage = val.locale;
      }
      if (val.langId === thirdLanguageId) {
        this.nameOfMiscValuesForView.thirdLanguage = val.locale;
      }
    });
  }

  getAllCountry() {
    if (!this.countryModel.isCountryAvailable) {
      this.countryModel.isCountryAvailable = true;
      this.commonService.GetAllCountry(this.countryModel).pipe(takeUntil(this.destroySubject$)).subscribe(data => {
        if (data) {
         if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
            this.countryListArr = [];
            if(!data.tableCountry){
              this.snackbar.open(data._message, '', {
                duration: 10000
              });
            }
          } else {
            // this.nationalityCtrl.setValue(data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1));
            this.filteredNationality.next(data.tableCountry?.slice());
            // this.countryOfBirthCtrl.setValue(data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1));
            this.filteredcountryOfBirth.next(data.tableCountry?.slice());
            this.countryListArr = data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1);
            if (this.studentCreateMode === this.studentCreate.VIEW) {
              // this.findCountryNationalityById();
            }
          }
        }
        else {
          this.countryListArr = [];
        }
      });
    }
  }

  findCountryNationalityById() {
    this.countryListArr.map((val) => {
      const countryInNumber = +this.data.countryOfBirth;
      const nationality = +this.data.nationality;
      if (val.id === countryInNumber) {
        this.nameOfMiscValuesForView.countryName = val.name;
      }
      if (val.id === nationality) {
        this.nameOfMiscValuesForView.nationality = val.name;
      }
    });
  }


  changeTab(status) {
    this.currentTab = status;
  }

  goToselectedStudent(){
    if(this.selectedStudentListModel.length) {
      this.selectedStudentList.paginator = this.selectedStudentsPaginator;
      this.groupAssignStudents = false;
      this.isShow = true;
      this.showSelectedStudent = true;
      this.checkStudentCustomValue();
      this.checkStudentMedicalCustomValue();
    } else {
      this.snackbar.open('Please Select any student.', '', {
        duration: 10000
      });
    }
   
  }

  backToselectedStudent(){
    this.groupAssignStudents = true;
    this.isShow = true;
    this.showSelectedStudent = false;
    this.StudentModelList.filteredData.map(user => {
      user.selectStudent = false;
      this.selectedStudentListModel = [];
      this.selectedStudentList = new MatTableDataSource([]);
      this.studentEnrollmentForGroupAssignModel.studentIds = [];
      this.studentAddForGroupAssignModel.studentIds = [];
      this.AddEditStudentMedicalProviderForGroupAssignModel.studentIds = [];
      this.StudentCommentsAddForGroupAssign.studentIds = [];
      this.StudentDocumentAddForGroupAssignModel.studentIds = [];
      this.callAllStudent();
    });
  }

  toggleStudentList() {
    this.isShow = !this.isShow;
  }


  getPageEvent(event) {
    if(this.defaultValuesService.getUserMembershipType() === this.profiles.HomeroomTeacher || this.defaultValuesService.getUserMembershipType() === this.profiles.Teacher){
      if (this.sort.active != undefined && this.sort.direction != "") {
        this.scheduleStudentListViewModel.sortingModel.sortColumn = this.sort.active;
        this.scheduleStudentListViewModel.sortingModel.sortDirection = this.sort.direction;
      }
      if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
        let filterParams = [
          {
            columnName: null,
            filterValue: this.searchCtrl.value,
            filterOption: 3
          }
        ]
        Object.assign(this.scheduleStudentListViewModel, { filterParams: filterParams });
      }
      this.scheduleStudentListViewModel.pageNumber = event.pageIndex + 1;
      this.scheduleStudentListViewModel.pageSize = event.pageSize;
      this.defaultValuesService.setPageSize(event.pageSize);
    }
    else{
      if (this.sort.active != undefined && this.sort.direction != "") {
        this.getAllStudent.sortingModel.sortColumn = this.sort.active;
        this.getAllStudent.sortingModel.sortDirection = this.sort.direction;
      }
      if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
        let filterParams = [
          {
            columnName: null,
            filterValue: this.searchCtrl.value,
            filterOption: 3
          }
        ]
        Object.assign(this.getAllStudent, { filterParams: filterParams });
      }
      this.getAllStudent.pageNumber = event.pageIndex + 1;
      this.getAllStudent.pageSize = event.pageSize;
      this.getAllStudent.filterParams = this.filterParameters;
      this.defaultValuesService.setPageSize(event.pageSize);
      this.callAllStudent();
    }
  }

  callAllStudent() {
    if (this.getAllStudent.sortingModel?.sortColumn == "") {
      this.getAllStudent.sortingModel = null;
    }
    this.studentService.GetAllStudentList(this.getAllStudent).subscribe(data => {
     if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
        if (data.studentListViews === null) {
          this.totalCount = this.isFromAdvancedSearch ? 0 : null;
          this.searchCount = this.isFromAdvancedSearch ? 0 : null;
          this.StudentModelList = new MatTableDataSource([]);
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
          this.isFromAdvancedSearch = false;
        } else {
          this.StudentModelList = new MatTableDataSource([]);
          this.totalCount = this.isFromAdvancedSearch ? 0 : null;
          this.searchCount = this.isFromAdvancedSearch ? 0 : null;
          this.isFromAdvancedSearch = false;
        }
      } else {
        this.totalCount = data.totalCount;
        this.searchCount = data.totalCount;
        this.pageNumber = data.pageNumber;
        this.pageSize = data._pageSize;
        data.studentListViews.forEach((student) => {
          student.checked = false;
        });

        this.StudentModelList = new MatTableDataSource(data.studentListViews);
        this.getAllStudent = new StudentListModel();
        this.StudentModelList.filteredData = data.studentListViews.map((item: any) => {
          this.studentEnrollmentForGroupAssignModel.studentIds.map((selectedUser) => {
            if (item.studentId == selectedUser) {
              item.selectStudent = true;
              return item;
            }
          });
          return item;
        });
        this.masterCheckBox.checked = this.StudentModelList.filteredData.every((item) => {
          return item.checked;
        })
        this.isFromAdvancedSearch = false;
      }
    });
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  resetStudentList() {
    this.getAllStudent = new StudentListModel();
    this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.searchCount = null;
    this.searchValue = null;
      this.callAllStudent();
  }

    editFilter() {
    this.showAdvanceSearchPanel = true;
    this.filterJsonParams = this.searchFilter;
    this.showSaveFilter = false;
    this.showLoadFilter = false;
  }

  /* This is for get all data from the Advanced Search component and then call the API in this page 
  NOTE: We just get the filterParams Array from Search component
  */
  filterData(res) {
    this.filterParameters = res.filterParams;
    this.isFromAdvancedSearch = true;
    this.getAllStudent = new StudentListModel();
    this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    if (res) {
      this.getAllStudent.filterParams = res.filterParams;
      this.getAllStudent.includeInactive = res.inactiveStudents;
      this.getAllStudent.searchAllSchool = res.searchAllSchool;
      this.getAllStudent.dobStartDate = this.commonFunction.formatDateSaveWithoutTime(res.dobStartDate);
      this.getAllStudent.dobEndDate = this.commonFunction.formatDateSaveWithoutTime(res.dobEndDate);
      this.defaultValuesService.sendIncludeInactiveFlag(res.inactiveStudents);
      this.defaultValuesService.sendAllSchoolFlag(res.searchAllSchool);
      this.callAllStudent();
      this.showSaveFilter = true;
    }
  }

  getSearchResult(res) {
    if (res.totalCount) {
      this.searchCount = res.totalCount;
      this.totalCount = res.totalCount;
    }
    else {
      this.searchCount = 0;
      this.totalCount = 0;
    }
    this.showSaveFilter = true;
    this.pageNumber = res.pageNumber;
    this.pageSize = res._pageSize;
    if (this.defaultValuesService.getUserMembershipType() === this.profiles.HomeroomTeacher || this.defaultValuesService.getUserMembershipType() === this.profiles.Teacher) {
      this.StudentModelList = new MatTableDataSource(res.scheduleStudentForView);
      this.scheduleStudentListViewModel = new ScheduleStudentListViewModel();
    }
    else{
      this.StudentModelList = new MatTableDataSource(res.studentListViews);
      if (res.studentListViews) {
      this.StudentModelList.filteredData = res.studentListViews.map((item: any) => {
        this.studentEnrollmentForGroupAssignModel.studentIds.map((selectedUser) => {
          if (item.studentId == selectedUser) {
            item.selectStudent = true;
            return item;
          }
        });
        return item;
      });
      }
      this.masterCheckBox.checked = this.StudentModelList.filteredData.every((item) => {
        return item.checked;
      })

      this.getAllStudent = new StudentListModel();
    }
  }

  getAllSearchFilter() {
    this.searchFilterListViewModel.module = 'Student';
    this.commonService.getAllSearchFilter(this.searchFilterListViewModel).subscribe((res) => {
      if (typeof (res) === 'undefined') {
        this.snackbar.open('Filter list failed. ' +this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.searchFilterListViewModel.searchFilterList = []
          if (!res.searchFilterList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        }
        else {
          this.searchFilterListViewModel = res;
          let filterData = this.searchFilterListViewModel.searchFilterList.filter(x => x.filterId == this.searchFilter.filterId);
          if (filterData.length > 0) {
            this.searchFilter.jsonList = filterData[0].jsonList;
          }
          if (this.filterJsonParams == null) {
            this.searchFilter = this.searchFilterListViewModel.searchFilterList[this.searchFilterListViewModel.searchFilterList.length - 1];
          }
        }
      }
    }
    );
  }

  getToggleValues(event) {
    this.toggleValues = event;
  }

  hideAdvanceSearch(event) {
    this.showSaveFilter = event.showSaveFilter;
    this.showAdvanceSearchPanel = false;
    if (event.showSaveFilter == false) {
      this.getAllSearchFilter();
    }
  }

  getSearchInput(event) {
    this.searchValue = event;
  }

  
  showAdvanceSearch() {
    this.showAdvanceSearchPanel = true;
    this.filterJsonParams = null;
  }

  submitEnrollment() {
    this.studentEnrollmentForGroupAssignModel.estimatedGradDate = this.commonFunction.formatDateSaveWithoutTime(this.studentEnrollmentForGroupAssignModel.estimatedGradDate)

    this.studentEnrollmentForGroupAssignModel.academicYear = this.defaultValuesService.getAcademicYear()?.toString();
    this.studentEnrollmentForGroupAssignModel.schoolId = +this.defaultValuesService.getSchoolID();
    this.studentEnrollmentForGroupAssignModel._userName = this.defaultValuesService.getUserName();

    this.studentService.studentEnrollmentForGroupAssign(this.studentEnrollmentForGroupAssignModel).subscribe((res) => {
      if (res){
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open( res._message, '', {
            duration: 10000
          });
        } else {
          this.snackbar.open('Information has been saved for selected students.', '', {
            duration: 10000
          });
        }
      }
      else{
        this.snackbar.open( this.defaultValuesService.translateKey('enrollmentUpdatefailed') + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  addStudentGeneralInfo() {
    this.studentAddForGroupAssignModel.academicYear = this.defaultValuesService.getAcademicYear().toString();
    this.studentAddForGroupAssignModel.studentMaster.dob = this.commonFunction.formatDateSaveWithoutTime(this.studentAddForGroupAssignModel.studentMaster.dob);
    this.studentService.AddStudentForGroupAssign(this.studentAddForGroupAssignModel).pipe(takeUntil(this.destroySubject$)).subscribe(data => {
      if (data) {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        } else {
          this.snackbar.open('Information has been saved for selected students.', '', {
            duration: 10000
          });
          // this.studentService.setStudentId(data.studentMaster.studentId);
          // this.studentService.setStudentCloneImage(data.studentMaster.studentPhoto);
          // this.studentService.setStudentDetails(data);
          // this.studentService.setDataAfterSavingGeneralInfo(data);
          // this.studentCreateMode = this.studentCreate.EDIT;
          // this.studentService.changePageMode(this.studentCreateMode);
          this.countryOfBirthCtrl.setValue(data.studentMaster.countryOfBirth);
          this.nationalityCtrl.setValue(data.studentMaster.nationality);
          // this.dataAfterSavingGeneralInfo.emit(data);
          // this.imageCropperService.enableUpload({module: this.moduleIdentifier.STUDENT, upload: true, mode: this.studentCreate.EDIT});
        }
      }
      else {
        this.snackbar.open(this.defaultValuesService.translateKey('studentSaveFailed') + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  finalSubmit() {
      this.studentAddForGroupAssignModel.fieldsCategoryList[0].customFields = this.studentCustomFields;
      this.AddEditStudentMedicalProviderForGroupAssignModel.fieldsCategoryList[4].customFields = this.studentMedicalCustomFields;

      this.addStudentGeneralInfo();
      this.submitEnrollment();
      this.submitComment();
      this.uploadFile();
      this.submitMedicalInfo();
  }

  setSelectedStudentList(event, element: any) {
    if(event.checked) {
      this.selectedStudentListModel.push(element);
      this.selectedStudentList = new MatTableDataSource(this.selectedStudentListModel);
      this.selectedStudentList.paginator = this.selectedStudentsPaginator;

      this.studentAddForGroupAssignModel.studentIds.push(element.studentId);
      this.studentEnrollmentForGroupAssignModel.studentIds.push(element.studentId);
      this.AddEditStudentMedicalProviderForGroupAssignModel.studentIds.push(element.studentId);
      this.StudentCommentsAddForGroupAssign.studentIds.push(element.studentId);
      this.StudentDocumentAddForGroupAssignModel.studentIds.push(element.studentId);
    } else {
      const index = this.selectedStudentListModel.findIndex(x => x.studentId === element.studentId);
      this.selectedStudentListModel.splice(index, 1);
      this.selectedStudentList = new MatTableDataSource(this.selectedStudentListModel);
      this.selectedStudentList.paginator = this.selectedStudentsPaginator;

      this.studentAddForGroupAssignModel.studentIds.splice(index, 1);
      this.studentEnrollmentForGroupAssignModel.studentIds.splice(index, 1);
      this.AddEditStudentMedicalProviderForGroupAssignModel.studentIds.splice(index, 1);
      this.StudentCommentsAddForGroupAssign.studentIds.splice(index, 1);
      this.StudentDocumentAddForGroupAssignModel.studentIds.splice(index, 1);
    }
  }

  checkViewPermission(category) {
    let permittedTabs =this.pageRolePermission.getPermittedSubCategories('/school/students')
    let filteredCategory: FieldsCategoryModel[] = [];
    for(const item of category){
      for (const permission of permittedTabs) {
        if (
          item.title.toLowerCase() ===
          permission.title.toLowerCase()
        ) {
            filteredCategory.push(item)
        }
      }
    }
    // this.currentCategory = filteredCategory[0]?.categoryId;
    return filteredCategory;
  }

  getAllFieldsCategory() {
    this.fieldsCategoryListView.module = Module.STUDENT;
    this.customFieldservice
      .getAllFieldsCategory(this.fieldsCategoryListView)
      .subscribe((res) => {
        if (res) {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            if (!res.fieldsCategoryList) {
              this.snackbar.open(res._message, "", {
                duration: 10000,
              });
            }
          } else {
            this.studentAddForGroupAssignModel.fieldsCategoryList = this.checkViewPermission(
              res.fieldsCategoryList
            );

            this.studentAddForGroupAssignModel.fieldsCategoryList.map((item)=>{
              item.customFields =  item.customFields.filter(x=> !x.systemField && !x.hide);
            })

            this.AddEditStudentMedicalProviderForGroupAssignModel.fieldsCategoryList = this.checkViewPermission(
              res.fieldsCategoryList
            );

            this.AddEditStudentMedicalProviderForGroupAssignModel.fieldsCategoryList.map((item)=>{
              item.customFields = item.customFields.filter(x=> !x.systemField && !x.hide);
            })

            // this.studentAddModel.fieldsCategoryList = this.checkViewPermission(
            //   res.fieldsCategoryList
            // );
    // this.studentService.setStudentDetailsForViewAndEdit(this.studentAddModel);

            // this.studentService.sendDetails(this.studentAddModel);
          }
        } else {
          this.snackbar.open(
            this.defaultValuesService.translateKey("categoryListFailed") +
            this.defaultValuesService.getHttpError(),
            "",
            {
              duration: 10000,
            }
          );
        }
      });
  }

  checkStudentCustomValue() {
    if (this.studentAddForGroupAssignModel?.fieldsCategoryList?.length>0) {
        this.studentCustomFields = this.studentAddForGroupAssignModel?.fieldsCategoryList[0]?.customFields.filter(x=> !x.systemField && !x.hide);
        if(this.studentCustomFields?.length>0){

        for (let studentCustomField of this.studentCustomFields) {
          if (studentCustomField?.customFieldsValue.length == 0) {

            studentCustomField?.customFieldsValue.push(new CustomFieldsValueModel());
            if(studentCustomField.type==='Checkbox'){
              studentCustomField.customFieldsValue[0].customFieldValue= studentCustomField.defaultSelection === 'true' ? true : false;
            }
            else{
              studentCustomField.customFieldsValue[0].customFieldValue= studentCustomField.defaultSelection;
            }
          }
          else {
            if (studentCustomField?.type === 'Multiple SelectBox') {
              this.studentMultiSelectValue = studentCustomField?.customFieldsValue[0].customFieldValue.split('|');

            }
          }
        }
      } 
    }
  }

  searchByFilterName(filter) {
    this.searchFilter = filter;
    this.showLoadFilter = false;
    this.showSaveFilter = false;

    if (this.defaultValuesService.getUserMembershipType() === this.profiles.HomeroomTeacher || this.defaultValuesService.getUserMembershipType() === this.profiles.Teacher) {
      this.scheduleStudentListViewModel.staffId = this.defaultValuesService.getUserId();
      this.scheduleStudentListViewModel.academicYear = this.defaultValuesService.getAcademicYear();
      this.scheduleStudentListViewModel.filterParams = JSON.parse(filter.jsonList);
      this.scheduleStudentListViewModel.sortingModel = null;
      this.studentScheduleService.searchScheduledStudentForGroupDrop(this.scheduleStudentListViewModel).subscribe(data => {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          if (data.scheduleStudentForView === null) {
            this.StudentModelList = new MatTableDataSource([]);
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          } else {
            this.StudentModelList = new MatTableDataSource([]);
          }
        } else {
          this.totalCount = data.totalCount;
          this.pageNumber = data.pageNumber;
          this.pageSize = data._pageSize;
          this.StudentModelList = new MatTableDataSource(data.scheduleStudentForView);
          this.scheduleStudentListViewModel = new ScheduleStudentListViewModel();
        }
      });
    }
    else{
      this.getAllStudent.filterParams = JSON.parse(filter.jsonList);
      this.getAllStudent.sortingModel = null;
      this.studentService.GetAllStudentList(this.getAllStudent).subscribe(data => {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          if (data.studentListViews === null) {
            this.StudentModelList = new MatTableDataSource([]);
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          } else {
            this.StudentModelList = new MatTableDataSource([]);
          }
        } else {
          this.totalCount = data.totalCount;
          this.pageNumber = data.pageNumber;
          this.pageSize = data._pageSize;
          this.StudentModelList = new MatTableDataSource(data.studentListViews);
          this.getAllStudent = new StudentListModel();
        }
      });
    }
    
  }

  
  someComplete(): boolean {
    let indetermine = false;
    if(this.StudentModelList) {
    for (let user of this.StudentModelList.filteredData) {
      for (let selectedUser of this.studentEnrollmentForGroupAssignModel.studentIds) {
        if (user.studentId == selectedUser) {
          indetermine = true;
        }
      }
    }
    if (indetermine && this.masterCheckBox) {
      this.masterCheckBox.checked = this.StudentModelList.filteredData.every((item) => {
        return item.selectStudent;
      })
      if (this.masterCheckBox.checked) {
        return false;
      } else {
        return true;
      }
    }
  }


  }

  
  setAll(event) {
    if(this.StudentModelList) {
    this.StudentModelList.filteredData.map(user => {
      user.selectStudent = event;
      if(event) {
        let isIdIncludesInSelectedList = false;
        if (this.selectedStudentListModel) {
          this.selectedStudentListModel.map((element) => {
            if (element.studentId === user.studentId) {
              isIdIncludesInSelectedList = true;
              return;
            }
          });
          if (!isIdIncludesInSelectedList) {
            this.selectedStudentListModel.push(user);
            this.selectedStudentList = new MatTableDataSource(this.selectedStudentListModel);
            this.selectedStudentList.paginator = this.selectedStudentsPaginator;

            this.studentEnrollmentForGroupAssignModel.studentIds.push(user.studentId);
            this.studentAddForGroupAssignModel.studentIds.push(user.studentId);
            this.AddEditStudentMedicalProviderForGroupAssignModel.studentIds.push(user.studentId);
            this.StudentCommentsAddForGroupAssign.studentIds.push(user.studentId);
            this.StudentDocumentAddForGroupAssignModel.studentIds.push(user.studentId);
          }
        } else {
          this.selectedStudentListModel.push(user);
          this.selectedStudentList = new MatTableDataSource(this.selectedStudentListModel);
          this.selectedStudentList.paginator = this.selectedStudentsPaginator;

          this.studentEnrollmentForGroupAssignModel.studentIds.push(user.studentId);
          this.studentAddForGroupAssignModel.studentIds.push(user.studentId);
          this.AddEditStudentMedicalProviderForGroupAssignModel.studentIds.push(user.studentId);
          this.StudentCommentsAddForGroupAssign.studentIds.push(user.studentId);
          this.StudentDocumentAddForGroupAssignModel.studentIds.push(user.studentId);
        }
      } else {
        this.selectedStudentListModel = [];
        this.selectedStudentList = new MatTableDataSource([]);
        
        this.studentEnrollmentForGroupAssignModel.studentIds = [];
        this.studentAddForGroupAssignModel.studentIds = [];
        this.AddEditStudentMedicalProviderForGroupAssignModel.studentIds = [];
        this.StudentCommentsAddForGroupAssign.studentIds = [];
        this.StudentDocumentAddForGroupAssignModel.studentIds = [];
      }
      });
    }
  }

  decideCheckUncheck() {
    this.StudentModelList.filteredData.map((item) => {
      let isIdIncludesInSelectedList = false;
      if (item.checked) {
        for (let selectedUser of this.studentEnrollmentForGroupAssignModel.studentIds) {
          if (item.studentId == selectedUser) {
            isIdIncludesInSelectedList = true;
            break;
          }
        }
        if (!isIdIncludesInSelectedList) {
          this.studentEnrollmentForGroupAssignModel.studentIds.push(item);
        }
      } else {
        for (let selectedUser of this.studentEnrollmentForGroupAssignModel.studentIds) {
          if (item.studentId == selectedUser) {
            this.studentEnrollmentForGroupAssignModel.studentIds = this.StudentModelList.filteredData.filter((user) => user != item.studentId);
            break;
          }
        }
      }
      isIdIncludesInSelectedList = false;

    });
    this.studentEnrollmentForGroupAssignModel.studentIds = this.StudentModelList.filteredData.filter((item) => item.checked);
  }

  checkStudentMedicalCustomValue() {
    if (this.AddEditStudentMedicalProviderForGroupAssignModel?.fieldsCategoryList?.length>0) {
        this.studentMedicalCustomFields = this.AddEditStudentMedicalProviderForGroupAssignModel?.fieldsCategoryList[4]?.customFields.filter(x=> !x.systemField && !x.hide);
        if(this.studentMedicalCustomFields?.length>0){

        for (let studentCustomField of this.studentMedicalCustomFields) {
          if (studentCustomField?.customFieldsValue.length == 0) {

            studentCustomField?.customFieldsValue.push(new CustomFieldsValueModel());
            if(studentCustomField.type==='Checkbox'){
              studentCustomField.customFieldsValue[0].customFieldValue= studentCustomField.defaultSelection==='true' ? true : false;
            }
            else{
              studentCustomField.customFieldsValue[0].customFieldValue= studentCustomField.defaultSelection;
            }
          }
          else {
            if (studentCustomField?.type === 'Multiple SelectBox') {
              this.studentMultiSelectValue = studentCustomField?.customFieldsValue[0].customFieldValue.split('|');

            }
          }
        }
      } 
    }
  }

  

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}

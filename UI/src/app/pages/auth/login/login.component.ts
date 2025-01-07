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

import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import icVisibility from '@iconify/icons-ic/twotone-visibility';
import icVisibilityOff from '@iconify/icons-ic/twotone-visibility-off';
import icLanguage from '@iconify/icons-ic/twotone-language';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { UserViewModel } from '../../../models/user.model';
import { LoginService } from '../../../services/login.service';
import { Observable, timer } from 'rxjs';
import { map } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import { LoaderService } from '../../../services/loader.service';
import { ValidationService } from '../../shared/validation.service';
import { LanguageModel } from '../../../models/language.model';
import { CookieService } from 'ngx-cookie-service';
import { CryptoService } from '../../../services/Crypto.service';
import { SchoolService } from '../../../services/school.service';
import { DefaultValuesService } from '../../../common/default-values.service';
import { CommonService } from '../../../services/common.service';
import { RolePermissionListViewModel } from '../../../models/roll-based-access.model';
import { RollBasedAccessService } from '../../../services/roll-based-access.service';
import { ProfilesTypes } from '../../../enums/profiles.enum';
import { AvailableTenantViewModel } from '../../../models/available-tenant';
import { CatalogDbService } from 'src/app/services/catalog-db.service';
@Component({
  selector: 'vex-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  animations: [
    fadeInUp400ms
  ]
})
export class LoginComponent implements OnInit {

  form: FormGroup;
  private date = new Date();
  time: Observable<Date> = timer(0, 1000).pipe(map(() => new Date()));
  inputType = 'password';
  visible = false;
  icVisibility = icVisibility;
  icVisibilityOff = icVisibilityOff;
  icLanguage = icLanguage;
  public tenant = "";
  UserModel: UserViewModel = new UserViewModel();
  loading: boolean;
  languages: LanguageModel = new LanguageModel();
  listOfLanguageCode;
  selectedLanguage:string=null;
  languageList;
  expiredDate
  setValue = false;
  forceLoaderToStop:boolean;
  buildVersion:string;
  profiles= ProfilesTypes;
  ipAdd:any;
  tenantPhoto: string;
  tenantName: any;
  tenantFooter: string;
  favIcon: HTMLLinkElement = document.querySelector('#appFavicon');
  rememberMeFlag: boolean = false;
  constructor(
    private router: Router,
    private Activeroute: ActivatedRoute,
    private fb: FormBuilder,
    private cd: ChangeDetectorRef,
    private snackbar: MatSnackBar,
    private loginService: LoginService,
    public translate: TranslateService,
    private cookieService: CookieService,
    private loaderService: LoaderService,
    private cryptoService:CryptoService,
    private schoolService:SchoolService,
    private defaultValuesService: DefaultValuesService,
    private commonService: CommonService,
    private rollBasedAccessService: RollBasedAccessService,
    private catalogDB: CatalogDbService,
  ) {
    translate.use("en");

    this.commonService.checkAndRoute();
    // this.Activeroute.params.subscribe(params => { this.tenant = params.id || 'opensisv2'; });
    this.tenant = this.defaultValuesService.getDefaultTenant();
    this.translate.addLangs(['en', 'fr']);
    this.translate.setDefaultLang('en');
    this.defaultValuesService.setTenant(this.tenant);

    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });
     /***10_8 */
     sessionStorage.clear();
     this.checkValidTenant();
     /**10_8 */
    this.GetAllLanguage();
    this.form = this.fb.group({
      email: ['', [Validators.required]],
      password: ['', Validators.required],
      language:['en', Validators.required]
    });
  }
  get f() { return this.form.controls; }

  ngOnInit() {
    
    if (this.cookieService.get('userDetails') !== null && this.cookieService.get('userDetails') !== "") {
      this.setValue = true;
      this.form.patchValue({ email: JSON.parse(this.cookieService.get('userDetails')).email});
      this.form.patchValue({ password: JSON.parse(this.cookieService.get('userDetails')).password });
      this.form.markAsDirty();
    }
    this.buildVersion= this.defaultValuesService.getBuildVersion();
    this.getIpAdressFromExternal();
  }

  checkValidTenant() {
    let tenant = this.defaultValuesService.getDefaultTenant();
    let model: AvailableTenantViewModel = new AvailableTenantViewModel();
    model.tenant.tenantName = tenant;
    this.catalogDB.validTenant(model).subscribe(
      (data) => {
        
        if (data.failure) {
          //this.router.navigateByUrl('/invalidtenant');
        } else {          
          sessionStorage.setItem('tenant', JSON.stringify(data.tenant.tenantName));
          this.defaultValuesService.setTenantID();
          this.tenantPhoto = data.tenant.tenantLogo;
          this.tenantName = data.tenant.tenantName;
          this.tenantFooter = data.tenant.tenantFooter;
          
          this.defaultValuesService.setPhotoAndFooter(data.tenant);
          this.favIcon.href = 'data:image/jpeg;base64,'+ data.tenant.tenantFavIcon;
          //this.router.navigateByUrl("/invalidtenant");
        }
      },
      (error) => {
        //this.router.navigateByUrl('/invalidtenant');
      }
    );
  }

  rememberMe(event) {
    if (!event) {
      this.rememberMeFlag = true;
    }
    else {
      this.rememberMeFlag = false;
    }

  }
  getIpAdressFromExternal() {
    this.commonService.getIpAddress().subscribe((res)=>{
     this.ipAdd=res;
    })
  }
  send() {
    this.getIpAdressFromExternal();
    this.form.markAsTouched();
    if (this.form.dirty && this.form.valid) {
      this.UserModel._tenantName = this.tenant;
      this.UserModel.password = this.form.value.password;
      this.UserModel.email = this.form.value.email.trim().toLowerCase();;
      this.UserModel.userAccessLog.ipaddress = (this.ipAdd.ip ? this.ipAdd.ip : null);
      this.UserModel.schoolId=this.defaultValuesService.getSchoolID();
      this.loginService.ValidateLogin(this.UserModel).subscribe(data => {
        if (typeof (data) == 'undefined') {
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
        else {
         if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          } else {
            if(data.lastUsedSchoolId){
              this.defaultValuesService.setSchoolID(data.lastUsedSchoolId.toString());
              }
            else{
            this.defaultValuesService.setSchoolID(data.schoolId.toString());
             }
             this.defaultValuesService.setToken(data._token)
            let middleName = data.middleName ? ' ' + data.middleName + ' ' : ' ';
            let fullName = data.firstGivenName + middleName + data.lastFamilyName;
            this.defaultValuesService.setFullUserName(fullName);
            this.defaultValuesService.setUserMembershipType(data.membershipType);
            this.defaultValuesService.setTenantIdVal(data.tenantId);
            this.defaultValuesService.setEmailId(data.email);
            this.defaultValuesService.setUserGuidId(data.userGuid);

            this.defaultValuesService.setUserName(data.name);
            this.defaultValuesService.setUserId(data.userId.toString());
            this.defaultValuesService.setUserPhoto(data.userPhoto);
            this.defaultValuesService.setUserMembershipID(data.membershipId.toString());
            this.defaultValuesService.setuserMembershipName(data.membershipName);
            this.callRolePermissions();
            
            if (this.rememberMeFlag) {
              this.expiredDate = new Date();
              this.expiredDate.setDate(this.expiredDate.getDate() + 7);
              this.cookieService.set('userDetails', JSON.stringify(this.form.value), this.expiredDate,null,null,true);
            }
            else {
              if (this.cookieService.get('userDetails') !== null && this.cookieService.get('userDetails') !== "") {
                this.cookieService.delete('userDetails');
              }
            }
          }
        }
      })
    }
  }

  callRolePermissions(){
    let rolePermissionListView: RolePermissionListViewModel = new RolePermissionListViewModel();
    rolePermissionListView.permissionList = [];
        this.rollBasedAccessService.getAllRolePermission(rolePermissionListView).subscribe((res: RolePermissionListViewModel) => {
          if(res){
            if(!res._failure){
              this.defaultValuesService.setPermissionList(res);
            this.schoolService.changeSchoolListStatus({schoolLoaded:false,schoolChanged:false,dataFromUserLogin:true,academicYearChanged:false,academicYearLoaded:false});
            if(this.defaultValuesService.getUserMembershipType() === this.profiles.HomeroomTeacher || this.defaultValuesService.getUserMembershipType() === this.profiles.Teacher){
              this.router.navigateByUrl("/school/teacher/dashboards").then(()=>{
                this.commonService.setUserActivity(true);
              });
            }
            else{
              this.router.navigateByUrl("/school/dashboards").then(()=>{
                this.commonService.setUserActivity(true);
              });
            }
            }
          }else{
              this.commonService.checkTokenValidOrNot(res._message);
          }
        });
  }

  GetAllLanguage() {
    this.forceLoaderToStop=false;
    this.languages._tenantName = this.tenant;
    this.loginService.getAllLanguageForLogin(this.languages).subscribe((res) => {
     if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        this.form.patchValue({
          language:null
        })
      }else{
        this.languageList = res.tableLanguage;
        this.forceLoaderToStop=true;
        // **Below commented lines(4) will be uncommented when we will have multilingual feature.
        //  this.listOfLanguageCode = this.languageList.map(a=>a.languageCode);
        //   this.translate.addLangs(this.listOfLanguageCode);
        //   this.translate.setDefaultLang('en');
        let checkPreviousPreference = this.defaultValuesService.getLanguage();
        if (checkPreviousPreference == null) {
          this.defaultValuesService.setLanguage('en');
          this.selectedLanguage = this.defaultValuesService.getLanguage();
        } else {
          this.selectedLanguage = this.defaultValuesService.getLanguage();
        }
      }
    })
  }

  toggleVisibility() {
    if (this.visible) {
      this.inputType = 'password';
      this.visible = false;
      this.cd.markForCheck();
    } else {
      this.inputType = 'text';
      this.visible = true;
      this.cd.markForCheck();
    }
  }

  switchLang(lang: string) {
    this.defaultValuesService.setLanguage(lang);
    this.selectedLanguage = lang;
    this.commonService.triggerLanguageChange(lang);
    // this.translate.use(lang)
  }

}
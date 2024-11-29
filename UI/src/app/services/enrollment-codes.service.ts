import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import {EnrollmentCodeAddView,EnrollmentCodeListView} from '../models/enrollment-code.model'
import { UpdateLovSortingModel } from '../models/lov.model';

@Injectable({
  providedIn: 'root'
})
export class EnrollmentCodesService {

  apiUrl:string = environment.apiURL;
  httpOptions: { headers: any; };
  constructor(private http: HttpClient,
    private defaultValuesService: DefaultValuesService) {
      this.httpOptions = {
        headers: new HttpHeaders({
          'Cache-Control': 'no-cache',
          'Pragma': 'no-cache',
        })
      }
     }

     addStudentEnrollmentCode(obj:EnrollmentCodeAddView){
      obj = this.defaultValuesService.getAllMandatoryVariable(obj);
      obj.studentEnrollmentCode = this.defaultValuesService.getAllMandatoryVariable(obj.studentEnrollmentCode);
      // obj.studentEnrollmentCode.academicYear = this.defaultValuesService.getAcademicYear();
      obj.studentEnrollmentCode.createdBy = this.defaultValuesService.getUserGuidId();
      let apiurl=this.apiUrl+obj._tenantName+"/StudentEnrollmentCode/addStudentEnrollmentCode";  
      return this.http.post<EnrollmentCodeAddView>(apiurl,obj,this.httpOptions)
    }
    getAllStudentEnrollmentCode(obj:EnrollmentCodeListView){
      obj = this.defaultValuesService.getAllMandatoryVariable(obj);
      obj.academicYear = this.defaultValuesService.getAcademicYear();
      let apiurl=this.apiUrl+obj._tenantName+"/StudentEnrollmentCode/getAllStudentEnrollmentCode";
      return this.http.post<EnrollmentCodeListView>(apiurl,obj,this.httpOptions)
    }
    updateStudentEnrollmentCode(obj:EnrollmentCodeAddView){
      obj = this.defaultValuesService.getAllMandatoryVariable(obj);
      obj.studentEnrollmentCode = this.defaultValuesService.getAllMandatoryVariable(obj.studentEnrollmentCode);
      // obj.studentEnrollmentCode.academicYear = this.defaultValuesService.getAcademicYear();
      obj.studentEnrollmentCode.updatedBy = this.defaultValuesService.getUserGuidId();
      let apiurl=this.apiUrl+obj._tenantName+"/StudentEnrollmentCode/updateStudentEnrollmentCode";
      return this.http.put<EnrollmentCodeAddView>(apiurl,obj,this.httpOptions)
    }
  deleteStudentEnrollmentCode(obj:EnrollmentCodeAddView){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl=this.apiUrl+obj._tenantName+"/StudentEnrollmentCode/deleteStudentEnrollmentCode";
    return this.http.post<EnrollmentCodeAddView>(apiurl,obj,this.httpOptions)
  }

  updateStudentEnrollmentCodeSortOrder(obj: UpdateLovSortingModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName+ "/StudentEnrollmentCode/updateStudentEnrollmentCodeSortOrder";
    return this.http.put<UpdateLovSortingModel>(apiurl,obj,this.httpOptions)
  }

}

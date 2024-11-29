import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { AddGradeLevelModel, GelAllGradeEquivalencyModel, GetAllGradeLevelsModel } from '../models/grade-level.model';
import { BehaviorSubject } from 'rxjs';
import { DefaultValuesService } from '../common/default-values.service';
import { UpdateLovSortingModel } from '../models/lov.model';

@Injectable({
  providedIn: 'root'
})
export class GradeLevelService {

  apiUrl:string = environment.apiURL;
  httpOptions: { headers: any; };
  constructor(private http: HttpClient,private defaultValuesService: DefaultValuesService) { 
    this.httpOptions = {
      headers: new HttpHeaders({
        'Cache-Control': 'no-cache',
        'Pragma': 'no-cache',
      })
    }
  }

  getAllGradeLevels(obj: GetAllGradeLevelsModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName+ "/Gradelevel/getAllGradeLevels";   
    return this.http.post<GetAllGradeLevelsModel>(apiurl,obj,this.httpOptions)
  }

  addGradelevel(obj:AddGradeLevelModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.tblGradelevel.schoolId= this.defaultValuesService.getSchoolID();
    obj.tblGradelevel.tenantId= this.defaultValuesService.getTenantID();
    obj.tblGradelevel.createdBy= this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName+ "/Gradelevel/addGradelevel";
    return this.http.post<AddGradeLevelModel>(apiurl,obj,this.httpOptions)
  }

  updateGradelevel(obj:AddGradeLevelModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.tblGradelevel.schoolId= this.defaultValuesService.getSchoolID();
    obj.tblGradelevel.tenantId= this.defaultValuesService.getTenantID();
    obj.tblGradelevel.updatedBy= this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName+ "/Gradelevel/updateGradelevel";
    return this.http.put<AddGradeLevelModel>(apiurl,obj,this.httpOptions)
  }

  deleteGradelevel(obj:AddGradeLevelModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.tblGradelevel.schoolId= this.defaultValuesService.getSchoolID();
    obj.tblGradelevel.tenantId= this.defaultValuesService.getTenantID()
    let apiurl = this.apiUrl + obj._tenantName+ "/Gradelevel/deleteGradelevel";
    return this.http.post<AddGradeLevelModel>(apiurl,obj,this.httpOptions)
  }

  getAllGradeEquivalency(obj:GelAllGradeEquivalencyModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName+ "/Gradelevel/getAllGradeEquivalency";
    return this.http.post<GelAllGradeEquivalencyModel>(apiurl,obj,this.httpOptions)
  }

  updateGradeLevelSortOrder(obj: UpdateLovSortingModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName+ "/Gradelevel/updateGradeLevelSortOrder";
    return this.http.put<UpdateLovSortingModel>(apiurl,obj,this.httpOptions)
  }
}

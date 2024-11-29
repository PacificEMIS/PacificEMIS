import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { GetAllSectionModel , SectionAddModel} from 'src/app/models/section.model';
import { DefaultValuesService } from '../common/default-values.service';
import { UpdateLovSortingModel } from '../models/lov.model';

@Injectable({
  providedIn: 'root'
})
export class SectionService {

  apiUrl: string = environment.apiURL;
  httpOptions: { headers: any; };
  constructor(private http: HttpClient, private defaultValuesService: DefaultValuesService) { 
    this.httpOptions = {
      headers: new HttpHeaders({
        'Cache-Control': 'no-cache',
        'Pragma': 'no-cache',
      })
    }
  }

  GetAllSection(obj: GetAllSectionModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + '/Section/getAllSection';
    return this.http.post<GetAllSectionModel>(apiurl, obj,this.httpOptions);
  }
  SaveSection(obj: SectionAddModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.tableSections.tenantId = this.defaultValuesService.getTenantID();
    obj.tableSections.createdBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName + '/Section/addSection';
    return this.http.post<SectionAddModel>(apiurl, obj,this.httpOptions);
  }
  UpdateSection(obj: SectionAddModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.tableSections.tenantId = this.defaultValuesService.getTenantID();
    obj.tableSections.updatedBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName + '/Section/updateSection';
    return this.http.put<SectionAddModel>(apiurl, obj,this.httpOptions);
  }

  deleteSection(obj: SectionAddModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.tableSections.tenantId = this.defaultValuesService.getTenantID();
    let apiurl = this.apiUrl + obj._tenantName + '/Section/deleteSection';
    return this.http.post<SectionAddModel>(apiurl, obj,this.httpOptions);
  }

  updateSectionSortOrder(obj: UpdateLovSortingModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName+ "/Section/updateSectionSortOrder";
    return this.http.put<UpdateLovSortingModel>(apiurl,obj,this.httpOptions)
  }
}

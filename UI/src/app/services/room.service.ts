import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import { RoomAddView, RoomListViewModel } from '../models/room.model';
import { UpdateLovSortingModel } from '../models/lov.model';

@Injectable({
  providedIn: 'root'
})
export class RoomService {
  apiUrl: string = environment.apiURL;
  httpOptions: { headers: any; };
  constructor(private http: HttpClient,private defaultValuesService: DefaultValuesService,) { 
    this.httpOptions = {
      headers: new HttpHeaders({
        'Cache-Control': 'no-cache',
        'Pragma': 'no-cache',
      })
    }
  }

  addRoom(Obj: RoomAddView){
    Obj = this.defaultValuesService.getAllMandatoryVariable(Obj);
    Obj.tableRoom.schoolId = this.defaultValuesService.getSchoolID();
    Obj.tableRoom.tenantId = this.defaultValuesService.getTenantID();
    Obj.tableRoom.createdBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + Obj._tenantName + '/Room/addRoom';
    return this.http.post<RoomAddView>(apiurl, Obj,this.httpOptions);
  }
  updateRoom(Obj: RoomAddView){
    Obj = this.defaultValuesService.getAllMandatoryVariable(Obj);
    Obj.tableRoom.schoolId = this.defaultValuesService.getSchoolID();
    Obj.tableRoom.tenantId = this.defaultValuesService.getTenantID();
    Obj.tableRoom.updatedBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + Obj._tenantName + '/Room/updateRoom';
    return this.http.put<RoomAddView>(apiurl, Obj,this.httpOptions);
  }
  deleteRoom(Obj: RoomAddView){
    Obj = this.defaultValuesService.getAllMandatoryVariable(Obj);
    Obj.tableRoom.schoolId = this.defaultValuesService.getSchoolID();
    Obj.tableRoom.tenantId = this.defaultValuesService.getTenantID();
    let apiurl = this.apiUrl + Obj._tenantName + '/Room/deleteRoom';
    return this.http.post<RoomAddView>(apiurl, Obj,this.httpOptions);
  }
  getAllRoom(obj: RoomListViewModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.academicYear = this.defaultValuesService.getAcademicYear();
    let apiurl = this.apiUrl + obj._tenantName + '/Room/getAllRoom';
    return this.http.post<RoomListViewModel>(apiurl, obj,this.httpOptions);
  }

  updateRoomSortOrder(obj: UpdateLovSortingModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName+ "/Room/updateRoomSortOrder";
    return this.http.put<UpdateLovSortingModel>(apiurl,obj,this.httpOptions)
  }
}

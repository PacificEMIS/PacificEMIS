import { CommonField } from './common-field.model';


export class lovModel{
    id: number;
    tenantId: string;
    schoolId: number;
    lovName: string;
    lovColumnValue: string;
    createdBy: string;
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
    constructor(){
        this.id=0;
        this.createdOn=null;
        this.updatedOn=null;
    }
}

export class LovAddView extends CommonField{
    dropdownValue:lovModel;
    constructor(){
        super();
        this.dropdownValue = new lovModel();
    }
}

export class LovList extends CommonField{
    public dropdownList:[lovModel];
    public isListView: boolean;
    public schoolId: number;
    public lovName: string;
    public tenantId: string;
    public isHasSchoolId:boolean;
}

export class UpdateLovSortingModel extends CommonField {
    public tenantId: string;
    public schoolId: number;
    public lovName: string;
    public updatedBy: string;
    public sortOrderValues: LoVSortOrderValuesModel[]
}

export class LoVSortOrderValuesModel {
    id: number;
    sortOrder: number;
}
import {IPermissionResponse} from "./ipermission-response";

export class PermissionModel implements IPermissionResponse {
  id: number;
  user_Id: number;
  type: string;
  value: string;

  static get lookupArray(): { key: string, value: string }[] {
    let array = [];
    let table = PermissionModel.lookupTable;

    for (const item in table) {
      array.push({key: item, value: table[item]});
    }

    return array;
  }

  private static _lookupTable: { [ key: string]: string } = {
    "http://EventSystem/Claims/Security/Role|0": "Dienstleistungsarten Administrator",
    "http://EventSystem/Claims/Security/Role|1": "System Administrator"
  };

  static get lookupTable(): { [p: string]: string } {
    return {...PermissionModel._lookupTable};
  }

  get isInternal(): boolean {
    return this.displayName == null;
  }

  private _displayName: string;

  get displayName(): string {
    return this._displayName;
  }

  static parse(data: IPermissionResponse): PermissionModel {
    let model = new PermissionModel();

    model.id = data.id;
    model.user_Id = data.user_Id;
    model.type = data.type;
    model.value = data.value;

    model._displayName = PermissionModel._lookupTable[model.type + "|" + model.value];

    return model;
  }
}

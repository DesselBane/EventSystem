/**
 * Created by hgx on 19.11.17.
 */
import {IEventService} from './ievent-service';

export class EventServiceModel implements IEventService {

  id: number;
  salary: number;
  profile: string;
  locationId: number;
  typeId: number; // ServiceTypeId
  personId: number;

  constructor() {

  }

  static parse(data: IEventService): EventServiceModel {
    const model = new EventServiceModel();
    model.id = data.id;
    model.locationId = data.locationId;
    model.salary = data.salary;
    model.profile = data.profile;
    model.typeId = data.typeId;
    model.personId = data.personId;

    return model;
  }
}

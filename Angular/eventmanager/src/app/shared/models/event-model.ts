import {HostModel} from './host-model';
import {IEventResponse} from './ievent-response';

export class EventModel implements IEventResponse {
  locationId: number;
  description: string;
  id: number;
  start: Date;
  end: Date;
  name: string;
  budget: number;
  hostId: number;
  host: HostModel;

  constructor(id?: number,
              startTime?: Date,
              endTime?: Date,
              name?: string,
              budget?: number,
              hostId?: number,
              host?: HostModel) {
    this.id = id;
    this.start = startTime;
    this.end = endTime;
    this.name = name;
    this.budget = budget;
    this.hostId = hostId;
    this.host = host;
  }

  static parse(data: IEventResponse): EventModel {
    const model = new EventModel();
    model.id = data.id;
    model.start = new Date(data.start);
    model.end = new Date(data.end);
    model.name = data.name;
    model.budget = data.budget;
    model.hostId = data.hostId;
    model.locationId = data.locationId;
    model.description = data.description;

    return model;
  }
}

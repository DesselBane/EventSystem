import {IAttendeeResponse} from './iattendee-response';

export class AttendeeModel implements IAttendeeResponse {
  eventId: number;
  personId: number;
  type: number;

  constructor(eventId?: number,
              personId?: number,
              type?: number) {
    this.eventId = eventId;
    this.personId = personId;
    this.type = type;
  }

  static parse(data: IAttendeeResponse): AttendeeModel {
    return new AttendeeModel(data.eventId, data.personId, data.type);
  }
}

/**
 * Created by hgx on 07.12.17.
 */

import {IEventServiceAgreementResponse} from './ievent-service-agreement-response';
import {EventModel} from './event-model';

export class EventServiceAgreementModel implements IEventServiceAgreementResponse {
  eventId: number;
  serviceSlotId: number;
  eventServiceModelId: number;
  cost: number;
  start: Date;
  end: Date;
  comment: string;
  state: number;
  event: EventModel;


  constructor(eventId?: number,
              serviceSlotId?: number,
              eventServiceModelId?: number,
              cost?: number,
              start?: Date,
              end?: Date,
              comment?: string,
              state?: number) {
    this.eventId = eventId;
    this.serviceSlotId = serviceSlotId;
    this.eventServiceModelId = eventServiceModelId;
    this.cost = cost;
    this.start = start;
    this.end = end;
    this.comment = comment;
    this.state = state;
  }

  static parse(data: IEventServiceAgreementResponse): EventServiceAgreementModel {
    return new EventServiceAgreementModel(
      data.eventId,
      data.serviceSlotId,
      data.eventServiceModelId,
      data.cost,
      new Date(data.start),
      new Date(data.end),
      data.comment,
      data.state
    );
  }
}



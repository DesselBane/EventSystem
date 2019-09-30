import {IServiceSlotResponse} from './iservice-slot-response';

export class ServiceSlotModel implements IServiceSlotResponse {

  budgetTarget: number;
  start: Date;
  eventId: number;
  typeId: number;
  id: number;
  end: Date;

  constructor(id?: number,
              budgetTarget?: number,
              start?: Date,
              end?: Date,
              eventId?: number,
              typeId?: number) {
    this.id = id;
    this.budgetTarget = budgetTarget;
    this.start = start;
    this.end = end;
    this.eventId = eventId;
    this.typeId = typeId;
  }

  static parse(data: IServiceSlotResponse): ServiceSlotModel {
    return new ServiceSlotModel(
      data.id,
      data.budgetTarget,
      new Date(data.start),
      new Date(data.end),
      data.eventId,
      data.typeId
    );
  }
}

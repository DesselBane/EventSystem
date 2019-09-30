/**
 * Created by hgx on 07.12.17.
 */
export interface IEventServiceAgreementResponse {
  eventId : number;
  serviceSlotId: number;
  eventServiceModelId : number;
  cost : number;
  start : Date;
  end : Date;
  comment : string;
  state : number;
}

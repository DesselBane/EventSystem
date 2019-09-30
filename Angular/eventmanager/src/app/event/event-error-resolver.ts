import {HttpErrorResponse} from '@angular/common/http';
import {EventErrorCodes} from '../shared/ErrorCodes/event-errorCodes';
import {HttpErrorModel} from '../shared/models/http-error-model';

export class EventErrorResolver {
  static resolve(error: HttpErrorResponse) {
    const parsedError = HttpErrorModel.parse(error);

    switch (parsedError.ErrorCode.toUpperCase()) {
      case EventErrorCodes.START_DATE_INVALID:
        return 'Bitte geben Sie ein korrektes Startdatum an!';
      case EventErrorCodes.END_DATE_INVALID:
        return 'Bitte geben Sie ein korrektes Enddatum an!';
      case EventErrorCodes.BUDGET_GREATER_OR_EQUAL_ZERO:
        return 'Wenn ein Budget angegeben wird, muss dieses positiv oder null sein!';
      case EventErrorCodes.NAME_REQUIRED:
        return 'Bitte geben Sie einen Titel an!';
      case EventErrorCodes.START_MUST_BE_BEFORE_END:
        return 'Der Startzeitpunkt muss vor den Endzeitpunkt sein!';
      case EventErrorCodes.NO_DELETE_PERMISSIONS:
        return 'Sie haben nicht die nötige Berechtigung, um dieses Event zu löschen!';
      case EventErrorCodes.EVENT_NOT_FOUND:
        return 'Das ausgewählte Eevnt existiert nicht!';
      case EventErrorCodes.NO_GET_PERMISSIONS:
        return 'Sie haben nicht die nötige Berechtigung, um dieses Event anzusehen!';
      case EventErrorCodes.NO_UPDATE_PERMISSIONS:
        return 'Sie haben nicht die nötige Berechtigung, um dieses Event zu ändern!';
      case EventErrorCodes.SERVICE_SLOT_NOT_FOUND:
        return 'Der Service Slot wurde nicht gefunden!';
      case EventErrorCodes.SERVICE_NOT_FOUND:
        return 'Das Event wurde nicht gefunden!';
      case EventErrorCodes.SERVICE_SLOT_ALREADY_CONTAINS_AGREEMENT:
        return 'Der Anbieter wurde dem Service Slot bereits hinzugefügt!';
      case EventErrorCodes.NO_UPDATE_PERMISSIONS_ON_SPS:
        return 'Sie haben nicht die nötige Berechtigung, um diesen Dienstleistungsslot zu ändern!';
      case EventErrorCodes.SERVICE_TYPE_DOESNT_EXIST:
        return 'Der Service Typ existiert nicht!';
      case EventErrorCodes.START_TIME_MUS_BE_BEFORE_END:
        return 'Der Startzeitpunkt muss vor dem Endzeitpunkt liegen!';
      case EventErrorCodes.NoServiceAgreementFoundForServiceSlot:
        return 'No Agreement'
    }
  }
}

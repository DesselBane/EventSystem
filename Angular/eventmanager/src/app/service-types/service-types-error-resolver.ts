import {HttpErrorResponse} from '@angular/common/http';
import {ServiceTypesErrorCodes} from '../shared/ErrorCodes/service-types-errorCodes';
import {HttpErrorModel} from '../shared/models/http-error-model';

export module ServiceTypesErrorResolver {
  export function resolve(error: HttpErrorResponse) {
    const parsedError = HttpErrorModel.parse(error);


    if (parsedError == null) {
      console.log(error);
      return 'Unknown Error Occured';
    }

    switch (parsedError.ErrorCode.toUpperCase()) {
      case ServiceTypesErrorCodes.NO_PERMISSION_TO_DELETE_SERVICE_TYPE:
        return 'Sie verfügen nicht über die nötigen Rechte, um diesen Dienstleister-Typ zu löschen!';
      case ServiceTypesErrorCodes.NO_PERMISSION_TO_UPDATE_SERVICE_TYPE:
        return 'Sie verfügen nicht über die nötigen Rechte, um diesen Dienstleister-Typ zu bearbeiten!';
      case ServiceTypesErrorCodes.SERVICE_TYPE_NOT_FOUND:
        return 'Der Dienstleister-Typ existiert nicht!';
      case ServiceTypesErrorCodes.NO_PERMISSION_TO_CREATE_SERVICE_TYPE:
        return 'Sie verfügen nicht über die nötigen Rechte, um einen Dienstleister-Typ zu erstellen!';
      case ServiceTypesErrorCodes.SERVICE_TYPE_ALREADY_EXISTS:
        return 'Der Typ ist schon vorhanden!';
      case ServiceTypesErrorCodes.TYPE_NAME_TOO_LONG:
        return 'Der Name darf nicht länger als 50 Zeichen sein!';
      case ServiceTypesErrorCodes.TYPE_NAME_MUST_BE_SET:
        return 'Bitte geben Sie einen Namen an!';

      default:
        throw error;
    }
  }
}

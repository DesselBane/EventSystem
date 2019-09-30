import {HttpErrorResponse} from '@angular/common/http';
import {PermissionErrorCodes} from '../shared/ErrorCodes/permission-error-codes';
import {HttpErrorModel} from '../shared/models/http-error-model';

export module PermissionErrorResolver {
  export function resolve(error: HttpErrorResponse): string {
    const parsedError = HttpErrorModel.parse(error);

    if (parsedError == null) {
      throw error;
    }

    console.log(error);

    switch (parsedError.ErrorCode.toUpperCase()) {
      case PermissionErrorCodes.CLAIM_ALREADY_CREATED:
        return 'Der Benutzer besitzt diese Berechtigung bereits!';
      case PermissionErrorCodes.CLAIM_NOT_FOUND:
        return 'Berechtigung konnte nicht gefunden werden!';
      case PermissionErrorCodes.INVALID_CLAIM_TYPE:
        return 'Dieser Berechtigungs Typ ist nicht zulässig!';
      case PermissionErrorCodes.INVALID_CLAIM_VALUE:
        return 'Dieser Berechtigungs Wert ist nicht zulässig!';
      case PermissionErrorCodes.NO_DELETE_PERMISSION:
        return 'Unzureichende Berechtigungen vorhanden um diese Berechtigung zu löschen!';
      case PermissionErrorCodes.NO_GET_PERMISSION:
        return 'Unzureichende Berechtigungen vorhanden um diese Berechtigung anzusehen!';
      case PermissionErrorCodes.NO_UPDATE_PERMISSION:
        return 'Unzureichende Berechtigungen vorhanden um diese Berechtigung zu verändern!';
      default:
        return 'Unknown Error occured. ErrorCode: ' + parsedError.ErrorCode;
    }
  }

}

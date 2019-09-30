import {HttpErrorResponse} from '@angular/common/http';
import {UserErrorCodes} from '../shared/ErrorCodes/user-error-codes';
import {HttpErrorModel} from '../shared/models/http-error-model';

export module UserErrorResolver {
  export function resolve(error: HttpErrorResponse): string {
    const parsedError = HttpErrorModel.parse(error);

    if (parsedError == null) {
      console.log(error);
      return 'Unknown Error Occured';
    }

    switch (parsedError.ErrorCode) {
      case UserErrorCodes.USER_NOT_FOUND:
        return 'Benutzer konnte nicht gefunden werden!';
      case UserErrorCodes.NO_GET_PERMISSIONS:
        return 'Keine Berechtigungen diesen Benutzer anzusehen!';
      default:
        return 'Undefined Error';
    }
  }
}

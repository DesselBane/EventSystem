import {HttpErrorResponse} from '@angular/common/http';
import {PersonErrorCodes} from '../shared/ErrorCodes/person-error-codes';
import {HttpErrorModel} from '../shared/models/http-error-model';

export module PersonErrorResolver {
  export function resolve(error: HttpErrorResponse): string {
    const parsedError = HttpErrorModel.parse(error);

    if (parsedError == null) {
      console.log(error);
      return 'Unknown Error Occured';
    }

    switch (parsedError.ErrorCode.toUpperCase()) {
      case PersonErrorCodes.PERSON_NOT_FOUND:
        return 'Diese Person existiert nicht!';
      case PersonErrorCodes.INVALID_SEARCH_TERM:
        return 'Dies ist keine valide Suchanfrage!';
      default:
        return 'Undefined Error';
    }
  }
}

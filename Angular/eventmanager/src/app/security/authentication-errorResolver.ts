import {HttpErrorResponse} from '@angular/common/http';
import {AuthenticationErrorCodes} from '../shared/ErrorCodes/authentication-errorCodes';
import {HttpErrorModel} from '../shared/models/http-error-model';

export module AuthenticationErrorResolver {

  export function resolve(error: HttpErrorResponse) {
    const errorParsed = HttpErrorModel.parse(error);

    switch (errorParsed.ErrorCode.toUpperCase()) {
      case AuthenticationErrorCodes.INVALID_USERNAME_OR_PASSWORD:
        return 'Ungültiger Benutzername oder ungültiges Passwort!';
      case AuthenticationErrorCodes.USERACCOUNT_NOT_ACTIVATED:
        return 'Der Account wurde noch nicht aktiviert!';
      case AuthenticationErrorCodes.RESET_HAST_DOESNT_EXIST:
        return 'Der Link zum Zurücksetzen des Passwortes ist ungültig!';
      case AuthenticationErrorCodes.USER_NOT_FOUND:
        return 'Den Benutzer gibt es nicht!';
      case AuthenticationErrorCodes.USERNAME_EXISTS_ALREADY:
        return 'Dieser Benutzername existiert bereits!';
      case AuthenticationErrorCodes.USERNAME_NO_EMAIL:
        return 'Bitte gib eine EMail Adresse ein!';

      default:
        console.log(`Could not resolve error ${errorParsed.ErrorCode}`);
        throw error;
    }
  }
}

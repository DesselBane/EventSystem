import {HttpErrorResponse} from '@angular/common/http';

export class HttpErrorModel {
  ErrorCode: string;
  ErrorReason: string;

  constructor(ErrorCode: string, ErrorReason: string) {
    this.ErrorCode = ErrorCode;
    this.ErrorReason = ErrorReason;
  }

  static parse(error: HttpErrorResponse): HttpErrorModel {
    const parsed = JSON.parse(error.error);
    return new HttpErrorModel(parsed.ErrorCode, parsed.ErrorReason);
  }
}

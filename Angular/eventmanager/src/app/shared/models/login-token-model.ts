import {ILoginTokenResponse} from "./ilogin-token-response";
import {JwtHelper} from "angular2-jwt";

export class LoginTokenModel implements ILoginTokenResponse{

  accessToken: string;
  accessToken_ValidUntil: Date;
  refreshToken: string;
  refreshToken_ValidUntil: Date;

  private _parsedToken: any;

  constructor(accessToken: string, accessTokenValidUntil: Date, refreshToken: string, refreshTokenValidUntil: Date) {
    this.accessToken = accessToken;
    this.accessToken_ValidUntil = accessTokenValidUntil;
    this.refreshToken = refreshToken;
    this.refreshToken_ValidUntil = refreshTokenValidUntil;
    this._parsedToken = new JwtHelper().decodeToken(accessToken);
  }

  isValid(): boolean {
    return new Date() < this.accessToken_ValidUntil;
  }

  static parse(data: ILoginTokenResponse): LoginTokenModel
  {
    return new LoginTokenModel(data.accessToken,new Date(data.accessToken_ValidUntil),data.refreshToken,new Date(data.refreshToken_ValidUntil));
  }

  getClaimValue(claimType: string): any {
    return this._parsedToken[claimType];
  }

  isServiceTypeAdmin(): boolean {
    let claimValue = this.getClaimValue('http://EventSystem/Claims/Security/Role');

    if (claimValue instanceof Array)
      return claimValue.includes("0") || claimValue.includes(0);
    else
      return claimValue == "0";
  }

  isSystemAdmin(): boolean {
    let claimValue = this.getClaimValue('http://EventSystem/Claims/Security/Role');

    if (claimValue instanceof Array)
      return claimValue.includes(1) || claimValue.includes("1");
    else
      return claimValue == 1;
  }
}

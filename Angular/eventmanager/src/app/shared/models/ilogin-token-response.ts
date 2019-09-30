export interface ILoginTokenResponse{
  readonly accessToken: string;
  readonly accessToken_ValidUntil: Date;
  readonly refreshToken: string;
  readonly refreshToken_ValidUntil: Date;
}

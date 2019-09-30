export class LocationModel {
  private _street: string;
  private _zipCode: string;
  private _state: string;
  private _country: string;
  private _id: number;

  constructor(id: number, street: string, zipCode: string, state: string, country: string) {
    this._id = id;
    this._street = street;
    this._zipCode = zipCode;
    this._state = state;
    this._country = country;

  }

  get id(): number {
    return this._id;
  }
}

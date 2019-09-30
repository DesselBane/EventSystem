import {Observable} from 'rxjs/Observable';

export class EventServiceStub {

}

export class PersonServiceStub {

}

export class RouterStub {

}

export class LocationStub {
  // method is onyl stub
  subscribe() {
  };

  // method is only stub
  path() {

  };
}

export class ActivatedRouteStub {
  // method is only stub
  params = Observable.of({id: 1});
}

export class ProfileServiceStub {

}

export class AuthenticationServiceStub {

}

export class ServiceTypesServiceStub {

}

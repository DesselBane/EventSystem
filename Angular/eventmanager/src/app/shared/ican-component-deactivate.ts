import {Observable} from 'rxjs/Observable';

export interface ICanComponentDeactivate {
  canDeactivate(): Observable<boolean>;
}

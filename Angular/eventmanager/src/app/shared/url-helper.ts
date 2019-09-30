import {ActivatedRoute} from '@angular/router';

export class UrlHelper {
  GetUrlQueryString(route: ActivatedRoute, queryString: string): number {
    let queryValue = null;
    route.params.subscribe(params => {
      queryValue = params[queryString];
    });
    return queryValue;
  }
}

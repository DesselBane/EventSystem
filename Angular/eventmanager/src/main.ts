import { enableProdMode } from '@angular/core';

import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';
import { getTranslationProviders } from './app/i18n-providers';


import '../node_modules/hammerjs';

if (environment.production) {
  enableProdMode();
}

/*
platformBrowserDynamic().bootstrapModule(AppModule);
*/
getTranslationProviders().then(providers => {
  const options = { providers };
  platformBrowserDynamic().bootstrapModule(AppModule, options);
});

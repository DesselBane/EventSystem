import {BrowserModule} from '@angular/platform-browser';
import {CUSTOM_ELEMENTS_SCHEMA, NgModule} from '@angular/core';
import {FlexLayoutModule} from '@angular/flex-layout';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {AppComponent} from './app.component';
import {AuthGuard} from './shared/auth.guard';
import {SharedModule} from './shared/shared.module';
import {AppRoutingModule} from './app-routing.module';
import {ImageUploadModule} from 'angular2-image-upload';
import {HttpModule} from '@angular/http';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {AuthenticationInterceptor} from './shared/AuthenticationInterceptor';
import {HttpClientErrorInterceptor} from './shared/HttpClientErrorInterceptor';
import {MaterialMetaModule} from "./material-meta/material-meta.module";

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    BrowserModule,
    FlexLayoutModule,
    BrowserAnimationsModule,
    SharedModule,
    HttpModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    ImageUploadModule,
    MaterialMetaModule,
    // always last one!
    AppRoutingModule
  ],
  providers: [
    AuthGuard,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpClientErrorInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthenticationInterceptor,
      multi: true
    }
    // providers used to create fake backend
    // fakeBackendProvider,
    // MockBackend,
    // BaseRequestOptions
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  bootstrap: [AppComponent]
})
export class AppModule {
}

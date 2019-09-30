import {async, ComponentFixture, fakeAsync, inject, TestBed, tick} from '@angular/core/testing';
import {APP_BASE_HREF, Location} from '@angular/common';
import {ResetPasswordComponent} from './resetPassword.component';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {AuthenticationService} from '../../shared/authentication.service';
import {AuthenticationServiceStub, LocationStub, RouterStub} from '../../shared/TestCommons/stubs';
import {Router} from '@angular/router';

describe('PasswordReset.Routing', () => {
  let component: ResetPasswordComponent;
  let fixture: ComponentFixture<ResetPasswordComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ResetPasswordComponent],
      providers: [
        {provide: APP_BASE_HREF, useValue: '/'},
        {provide: AuthenticationService, useClass: AuthenticationServiceStub},
        {provide: Router, useClass: RouterStub},
        {provide: Location, useClass: LocationStub},
      ],
      schemas: [NO_ERRORS_SCHEMA],
    })
      .overrideComponent(ResetPasswordComponent, {
        set: {
          template: '<a routerLink="/auth/login" id="login">Login</a>'
          + '<router-outlet></router-outlet>'
        }
      })
      .compileComponents()
      .then(() => {
        fixture = TestBed.createComponent(ResetPasswordComponent);
        component = fixture.componentInstance;
      });
  }));

  xit('Reset password should redirect to login',
    fakeAsync(inject([Location], (location: Location) => {
      // arrange
      const passwordResetLink = fixture.debugElement.nativeElement.querySelector('#login');
      fixture.detectChanges();
      tick();

      // act
      passwordResetLink.click();
      tick();

      // assert
      fixture.whenStable().then(() => {
        expect(location.path()).toBe('/auth/login');
      })
    })));
});

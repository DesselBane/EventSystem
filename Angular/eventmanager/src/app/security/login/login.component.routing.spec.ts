/**
 * Created by Christian on 12.05.2017.
 */
import {async, ComponentFixture, fakeAsync, inject, TestBed, tick} from '@angular/core/testing';
import {APP_BASE_HREF, Location} from '@angular/common';
import {LoginComponent} from './login.component';
import {NO_ERRORS_SCHEMA} from '@angular/core';

describe('LoginComponent.Routing', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [LoginComponent],
      providers: [
        {provide: APP_BASE_HREF, useValue: '/'}
      ],
      schemas: [NO_ERRORS_SCHEMA],
    })
      .overrideComponent(LoginComponent, {
        set: {
          template: '<a class="noLinkStyle" routerLink="/auth/resetPassword" id="resetPassword">Passwort vergessen?</a>'
          + '<router-outlet></router-outlet>'
        }
      })
      .compileComponents()
      .then(() => {
        fixture = TestBed.createComponent(LoginComponent);
        component = fixture.componentInstance;
      });
  }));

  xit('Click on "Passwort vergessen?" should navigate to resetPassword',
    fakeAsync(inject([Location], (location: Location) => {
      // arrange
      const passwordResetLink = fixture.debugElement.nativeElement.querySelector('#resetPassword');
      fixture.detectChanges();
      tick();

      // act
      passwordResetLink.click();
      tick();

      // assert
      fixture.whenStable().then(() => {
        expect(location.path()).toBe('/auth/resetPassword');
      })
    })));
});

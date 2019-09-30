import {async, ComponentFixture, fakeAsync, TestBed, tick} from '@angular/core/testing';

import {ResetPasswordComponent} from './resetPassword.component';
import {APP_BASE_HREF} from '@angular/common';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {AuthenticationService} from '../../shared/authentication.service';
import {AuthenticationServiceStub, RouterStub} from '../../shared/TestCommons/stubs';
import {Router} from '@angular/router';

describe('ResetPasswordComponent', () => {
  let component: ResetPasswordComponent;
  let fixture: ComponentFixture<ResetPasswordComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [FormsModule],
      declarations: [ResetPasswordComponent],
      providers: [
        {provide: APP_BASE_HREF, useValue: '/'},
        {provide: AuthenticationService, useClass: AuthenticationServiceStub},
        {provide: Router, useClass: RouterStub},
      ],
      schemas: [NO_ERRORS_SCHEMA],
    })
      .compileComponents()
      .then(() => {
        fixture = TestBed.createComponent(ResetPasswordComponent);
        component = fixture.componentInstance;
      });
  }));

  it('ResetPasswordComponent creation should succeed', () => {
    // assert
    expect(component).toBeTruthy();
  });

  xit('Request password reset for existing user should succeed', () => {
    // act

    // assert
    expect(component.error).toBe('');
  });

  xit('Request password reset for non existing user should return expected error', () => {
    // act

    // assert
    expect(component.error).toBe('Den Benutzer gibt es nicht!');
  });

  it('Input an email into the html should update the property', fakeAsync(() => {
    // arrange
    const usernameInput = fixture.debugElement.nativeElement.querySelector('#username');
    fixture.detectChanges();
    tick();

    // act
    usernameInput.value = 'an email';
    usernameInput.dispatchEvent(new Event('input'));
    tick();

    // assert
    expect(component.username).toBe('an email');
  }));
});

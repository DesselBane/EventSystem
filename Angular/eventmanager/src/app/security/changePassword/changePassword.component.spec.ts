import {async, ComponentFixture, fakeAsync, TestBed, tick} from '@angular/core/testing';
import {APP_BASE_HREF} from '@angular/common';
import {ChangePasswordComponent} from './changePassword.component';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {ActivatedRouteStub, AuthenticationServiceStub, RouterStub} from '../../shared/TestCommons/stubs';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthenticationService} from '../../shared/authentication.service';

describe('ChangePasswordComponent', () => {
  let component: ChangePasswordComponent;
  let fixture: ComponentFixture<ChangePasswordComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [FormsModule],
      declarations: [ChangePasswordComponent],
      providers: [
        {provide: APP_BASE_HREF, useValue: '/'},
        {provide: Router, useClass: RouterStub},
        {provide: ActivatedRoute, useClass: ActivatedRouteStub},
        {provide: AuthenticationService, useClass: AuthenticationServiceStub},
      ],
      schemas: [NO_ERRORS_SCHEMA],
    })
      .compileComponents()
      .then(() => {
        fixture = TestBed.createComponent(ChangePasswordComponent);
        component = fixture.componentInstance;
      });
  }));

  xit('ChangePasswordComponent creation should succeed', () => {
    // assert
    expect(component).toBeTruthy();
  });

  xit('Request password change with valid link should succeed', () => {
    // act

    // assert
    expect(component.error).toBe('');
  });

  xit('Request password change with invalid link should return expected error', () => {
    // act

    // assert
    expect(component.error).toBe('Der Link zum Zurücksetzen des Passwortes ist ungültig!');
  });

  xit('Request password change with not matching password and passwordRepeat should return expected error', () => {
    // arrange
    component.password = 'asdf';
    component.passwordRepeat = 'asdfg';

    // act
    component.changePassword();

    // assert
    expect(component.error).toBe('Die Passwörter stimmen nicht überein!');
  });

  xit('Input a password into the html should update the property', fakeAsync(() => {
    // arrange
    const passwordInput = fixture.debugElement.nativeElement.querySelector('#password');
    fixture.detectChanges();
    tick();

    // act
    passwordInput.value = 'password';
    passwordInput.dispatchEvent(new Event('input'));
    tick();

    // assert
    expect(component.password).toBe('password');
  }));

  xit('Input a passwordRepeat into the html should update the property', fakeAsync(() => {
    // arrange
    const passwordRepeatInput = fixture.debugElement.nativeElement.querySelector('#passwordRepeat');
    fixture.detectChanges();
    tick();

    // act
    passwordRepeatInput.value = 'password';
    passwordRepeatInput.dispatchEvent(new Event('input'));
    tick();

    // assert
    expect(component.passwordRepeat).toBe('password');
  }));
});

import {async, ComponentFixture, fakeAsync, TestBed, tick} from '@angular/core/testing';
import {APP_BASE_HREF} from '@angular/common';
import {LoginComponent} from './login.component';
import {routes} from '../../app-routing.module';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {FormsModule} from '@angular/forms';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [FormsModule],
      declarations: [LoginComponent],
      providers: [
        {provide: APP_BASE_HREF, useValue: '/'}
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents()
      .then(() => {
        fixture = TestBed.createComponent(LoginComponent);
        component = fixture.componentInstance;
      });
  }));

  xit('LoginComponent creation should succeed', () => {
    // assert
    expect(component).toBeTruthy();
  });

  xit('Login with valid user-password-combination an activated account should succeed', () => {
    // act

    // assert
    expect(component.error).toBe('');
  });

  xit('Login with invalid user-password-combination should return expected error', () => {
    // act

    // assert
    expect(component.error).toBe('Ungültiger Benutzername oder ungültiges Passwort!');
  });

  xit('Login with not activated account should return expected error', () => {
    // act

    // assert
    expect(component.error).toBe('Der Account wurde noch nicht aktiviert!');
  });

  xit('Input a username into the html should update the property', fakeAsync(() => {
    // arrange
    const usernameInput = fixture.debugElement.nativeElement.querySelector('#username');
    fixture.detectChanges();
    tick();

    // act
    usernameInput.value = 'username';
    usernameInput.dispatchEvent(new Event('input'));
    tick();

    // assert
    expect(component.username).toBe('username');
  }));

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

  xit('Html link for password reset should get expected route from the property', () => {
    // arrange
    const resetPasswordRoute = routes.find(r => r.path === 'resetPassword');
    const resetPasswordLink = fixture.debugElement.nativeElement.querySelector('#resetPasswordLink');
    fixture.detectChanges();

    // assert
    expect(resetPasswordLink.href).toContain(resetPasswordRoute.path);
  });

  xit('ResetPasswordRoute should be expected route', () => {
    // arrange
    const resetPasswordRoute = routes.find(r => r.path === 'resetPassword');

    // assert
    expect(component.resetPasswordRoute).toEqual('/' + resetPasswordRoute.path);
  });
});

import {async, ComponentFixture, fakeAsync, TestBed, tick} from '@angular/core/testing';
import {APP_BASE_HREF} from '@angular/common';
import {RegisterComponent} from './register.component';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {AuthenticationService} from '../../shared/authentication.service';
import {AuthenticationServiceStub, RouterStub} from '../../shared/TestCommons/stubs';
import {Router} from '@angular/router';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [FormsModule],
      declarations: [RegisterComponent],
      providers: [
        {provide: APP_BASE_HREF, useValue: '/'},
        {provide: AuthenticationService, useValue: AuthenticationServiceStub},
        {provide: Router, useValue: RouterStub},
      ],
      schemas: [NO_ERRORS_SCHEMA],
    })
      .compileComponents()
      .then(() => {
        fixture = TestBed.createComponent(RegisterComponent);
        component = fixture.componentInstance;
      });
  }));

  it('RegisterComponent creation should succeed', () => {
    // assert
    expect(component).toBeTruthy();
  });

  it('Register with unused and valid email should succeed', () => {
    // act

    // assert
    expect(component.errorMessage).toBe('');
  });

  xit('Register with invalid email should return expected error', () => {
    // act

    // assert
    expect(component.errorMessage).toBe('Die eingegebene Adresse ist keine korrekte E-Mail-Adresse!');
  });

  xit('Register with already used email should return expected error', () => {
    // act

    // assert
    expect(component.errorMessage).toBe('Die eingegebene E-Mail-Adresse wird bereits verwendet!');
  });

  it('Input an email into the html should update the property', fakeAsync(() => {
    // arrange
    const emailInput = fixture.debugElement.nativeElement.querySelector('#email');
    fixture.detectChanges();
    tick();

    // act
    emailInput.value = 'an email';
    emailInput.dispatchEvent(new Event('input'));
    tick();

    // assert
    expect(component.email).toBe('an email');
  }));
});

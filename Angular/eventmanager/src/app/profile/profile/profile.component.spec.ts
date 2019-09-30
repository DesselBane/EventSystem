import {async, ComponentFixture, TestBed} from '@angular/core/testing';

import {ProfileComponent} from './profile.component';
import {APP_BASE_HREF} from '@angular/common';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {ProfileService} from '../profile.service';
import {ProfileServiceStub} from '../../shared/TestCommons/stubs';
import {MdDialogModule} from '@angular/material';
import {PersonService} from "../../person/person.service";

describe('ProfileComponent', () => {
  let component: ProfileComponent;
  let fixture: ComponentFixture<ProfileComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [MdDialogModule],
      providers: [
        {provide: APP_BASE_HREF, useValue: '/'},
        {provide: ProfileService, usceClass: ProfileServiceStub},
        {provide: PersonService}
      ],
      declarations: [ProfileComponent],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents()
      .then(() => {
        fixture = TestBed.createComponent(ProfileComponent);
        component = fixture.componentInstance;
      });
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

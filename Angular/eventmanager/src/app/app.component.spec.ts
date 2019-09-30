import {async, ComponentFixture, TestBed} from '@angular/core/testing';
import {APP_BASE_HREF} from '@angular/common';
import {AppComponent} from './app.component';
import {NO_ERRORS_SCHEMA} from '@angular/core';

describe('AppComponent', () => {
  let app: AppComponent;
  let fixture: ComponentFixture<AppComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [AppComponent],
      providers: [
        {provide: APP_BASE_HREF, useValue: '/'}
      ],
      schemas: [NO_ERRORS_SCHEMA],
    })
      .compileComponents()
      .then(() => {
        fixture = TestBed.createComponent(AppComponent);
        app = fixture.debugElement.componentInstance;
      });
  }));

  it('should create the app', async(() => {
    // assert
    expect(app).toBeTruthy();
  }));
});

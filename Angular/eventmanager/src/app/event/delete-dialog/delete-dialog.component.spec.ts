import {async, ComponentFixture, TestBed} from '@angular/core/testing';

import {DeleteDialogComponent} from './delete-dialog.component';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {APP_BASE_HREF} from '@angular/common';

describe('DeleteDialogComponent', () => {
  let component: DeleteDialogComponent;
  let fixture: ComponentFixture<DeleteDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [],
      declarations: [DeleteDialogComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [{provide: APP_BASE_HREF, useValue: ''}],
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeleteDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});

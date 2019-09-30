import {async, ComponentFixture, TestBed} from '@angular/core/testing';

import {UpdateServiceSlotComponent} from './update-service-slot.component';
import {RouterStub} from '../../shared/TestCommons/stubs';
import {Router} from '@angular/router';

describe('UpdateServiceSlotComponent', () => {
  let component: UpdateServiceSlotComponent;
  let fixture: ComponentFixture<UpdateServiceSlotComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [UpdateServiceSlotComponent],
      providers: [
        {provide: Router, useClass: RouterStub},
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UpdateServiceSlotComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

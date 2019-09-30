import {inject, TestBed} from '@angular/core/testing';
import {EventService} from './event.service';
import {AppModule} from '../app.module';
import {APP_BASE_HREF} from '@angular/common';

describe('EventServiceModel', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        EventService,
        {provide: APP_BASE_HREF, useValue: '/'},
        {provide: EventService, useValue: EventServiceStub},
      ],
      imports: [
        AppModule
      ]
    });
  });

  it('should ...', inject([EventService], (service: EventService) => {
    expect(service).toBeTruthy();
  }));
});

class EventServiceStub {
}

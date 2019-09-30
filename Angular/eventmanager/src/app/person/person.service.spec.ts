import {inject, TestBed} from '@angular/core/testing';

import {PersonService} from './person.service';

describe('PersonService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [PersonService]
    });
  });

  xit('should ...', inject([PersonService], (service: PersonService) => {
    expect(service).toBeTruthy();
  }));
});

import {inject, TestBed} from '@angular/core/testing';

import {ServiceTypesService} from './service-types.service';

describe('ServiceTypesService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ServiceTypesService]
    });
  });

  xit('should ...', inject([ServiceTypesService], (service: ServiceTypesService) => {
    expect(service).toBeTruthy();
  }));
});

import {Component, OnInit} from '@angular/core';
import {ServiceTypeModel} from '../../shared/models/service-types-model';
import {ServiceTypesService} from '../service-types.service';
import {ServiceTypesErrorResolver} from '../service-types-error-resolver';

@Component({
  selector: 'app-service-types-overview',
  templateUrl: './service-types-overview.component.html',
  styleUrls: ['./service-types-overview.component.css']
})
export class ServiceTypesOverviewComponent implements OnInit {
  serviceTypes: ServiceTypeModel[];
  error: string;

  constructor(private serviceTypesService: ServiceTypesService) {
  }

  ngOnInit() {
    this.loadServiceTypes();
  }

  loadServiceTypes() {
    this.serviceTypesService.getServiceTypes()
      .subscribe((result: ServiceTypeModel[]) => {
        this.serviceTypes = result;
      });
  }

  submitDelete(id: number) {
    this.serviceTypesService.deleteServiceType(id)
      .subscribe(() => {
        this.loadServiceTypes();
      }, (error) => {
        this.loadServiceTypes();
        this.error = ServiceTypesErrorResolver.resolve(error);
      });
  }

}

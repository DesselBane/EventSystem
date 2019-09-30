import {Component, OnInit} from '@angular/core';
import {ServiceTypeModel} from '../../shared/models/service-types-model';
import {ServiceTypesService} from '../service-types.service';
import {ServiceTypesErrorResolver} from '../service-types-error-resolver';
import {Router} from '@angular/router';

@Component({
  selector: 'app-create-service-types',
  templateUrl: './create-service-type.component.html',
  styleUrls: ['./create-service-type.component.css']
})
export class CreateServiceTypeComponent implements OnInit {
  newServiceType = new ServiceTypeModel();
  error: string;

  constructor(private serviceTypesService: ServiceTypesService, private router: Router) {
  }

  ngOnInit() {
  }

  create() {
    this.serviceTypesService.createServiceType(this.newServiceType)
      .subscribe(() => {
        this.router.navigate([`serviceTypes`]);
      }, (error) => {
        this.error = ServiceTypesErrorResolver.resolve(error);
      });
  }
}

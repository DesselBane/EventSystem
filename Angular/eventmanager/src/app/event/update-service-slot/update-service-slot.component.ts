import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

@Component({
  selector: 'app-update-service-slot',
  templateUrl: './update-service-slot.component.html',
  styleUrls: ['./update-service-slot.component.css']
})
export class UpdateServiceSlotComponent implements OnInit {
  private router: Router;

  constructor(router: Router) {
    this.router = router;
  }

  ngOnInit() {
  }

}

import {Component, HostListener, OnInit, ViewChild} from '@angular/core';
import {AuthenticationService} from './shared/authentication.service';
import {MdSidenav} from '@angular/material';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {
  constructor() { }

  @ViewChild('sidenav') sidenav: MdSidenav;
  navMode = 'side';

  ngOnInit() {
    if (window.innerWidth < 768) {
      this.navMode = 'over';
    }
  }

  @HostListener('window:resize', ['$event'])
  onResize(event) {
    if (event.target.innerWidth < 768) {
      this.navMode = 'over';
      this.sidenav.close();
    }
    if (event.target.innerWidth > 768) {
      this.navMode = 'side';
      this.sidenav.open();
    }
  }



  navigationItems = [
    {
      label: 'Register',
      route: 'auth/register'
    }, {
      label: 'Login',
      route: 'auth/login'
    },
    {
      label: 'Profil',
      route: 'profile',
      needsAuthentication: true
    },
    {
      label: 'Events',
      route: 'events',
      needsAuthentication: true
    },
    {
      label: 'Service anbieten',
      route: 'provideEventService',
      needsAuthentication: true
    },
    {
      label: 'Serviceanfragen',
      route: 'provideEventService/eventServiceAgreements',
      needsAuthentication: true
    },
    {
      label: 'Admin',
      route: 'serviceTypes',
      needsAuthentication: true,
      hasPrivilege: () => {
        let token = AuthenticationService.loginToken;

        if (token != null)
          return token.isSystemAdmin() || token.isServiceTypeAdmin();
        else
          return false;
      },
      children: [
        {
          label: 'Servicetypen',
          route: 'serviceTypes',
          needsAuthentication: true,
          hasPrivilege: () => {
            let token = AuthenticationService.loginToken;

            if (token != null)
              return token.isServiceTypeAdmin();
            else
              return false;
          }
        },
        {
          label: 'Benutzer',
          route: 'userManagement',
          needsAuthentication: true,
          hasPrivilege: () => {
            let token = AuthenticationService.loginToken;

            if (token != null)
              return token.isSystemAdmin();
            else
              return false;
          }
        }
      ]
    }
  ];

  // needed for Angular frontend but Webstorm doesnt get it
  // noinspection JSMethodCanBeStatic
  isLoggedIn() {
    return AuthenticationService.isLoggedIn();
  }

}



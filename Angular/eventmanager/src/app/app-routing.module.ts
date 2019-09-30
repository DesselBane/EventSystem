import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';

export const routes: Routes = [
  {path: 'auth', loadChildren: './security/security.module#SecurityModule'},
  {path: 'profile', loadChildren: './profile/profile.module#ProfileModule'},
  {path: 'events', loadChildren: './event/event.module#EventModule'},
  {path: 'serviceTypes', loadChildren: './service-types/service-types.module#ServiceTypesModule'},
  {path: 'userManagement', loadChildren: './user-management/user-management.module#UserMangementModule'},
  {path: 'provideEventService', loadChildren: './event-service/event-service.module#EventServiceModule'},

  // otherwise redirect to events
  {path: '**', redirectTo: 'events'}
];

@NgModule({
  imports: [
    // tracing only for debug purposes  ,{enableTracing: true}
    RouterModule.forRoot(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class AppRoutingModule {
}

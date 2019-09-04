import { Route } from '@angular/router';
import { HomeComponent } from './home/home.component';

import {
  PersonComponents,
  PersonRoutes
} from './person';

export const RouteComponents = [
  HomeComponent,
  ...PersonComponents
];

export const Routes: Route[] = [
  { path: 'home', component: HomeComponent },
  ...PersonRoutes,
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: '**', redirectTo: 'home', pathMatch: 'full' }
];

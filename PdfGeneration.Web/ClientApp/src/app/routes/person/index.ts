import { Route } from '@angular/router';

import { PeopleComponent } from './people.component';
import { PersonComponent } from './person.component';
import { PersonSideComponent } from './person-side.component';

import { PersonAssociatesComponent } from './children/person-associates.component';

export const PersonComponents = [
  PeopleComponent,
  PersonComponent,
  PersonSideComponent,
  PersonAssociatesComponent,
];

export const PersonRoutes: Route[] = [
  { path: 'people', component: PeopleComponent },
  { path: 'person/:id', component: PersonComponent, children: [
    { path: 'associates', component: PersonAssociatesComponent },
    { path: '', redirectTo: 'associates', pathMatch: 'prefix' },
    { path: '**', redirectTo: 'associates', pathMatch: 'prefix' }
  ]},
  { path: 'person/:id', component: PersonSideComponent, outlet: 'side' }
];

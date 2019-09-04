import { Person } from '../../models/person';

import {
  Component,
  Input,
  Output,
  EventEmitter
} from '@angular/core';

@Component({
  selector: 'person-overview',
  templateUrl: 'person-overview.component.html'
})
export class PersonOverviewComponent {
  @Input() person: Person;
  @Output() view = new EventEmitter<Person>();
  @Output() pin = new EventEmitter<Person>();
}

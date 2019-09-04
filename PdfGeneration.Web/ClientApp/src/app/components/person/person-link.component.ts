import {
  Component,
  Input,
  Output,
  EventEmitter
} from '@angular/core';

import { Person } from '../../models';

@Component({
  selector: 'person-link',
  templateUrl: 'person-link.component.html'
})
export class PersonLinkComponent {
  @Input() person: Person;
  @Input() width = 320;
  @Output() selected = new EventEmitter<Person>();
}

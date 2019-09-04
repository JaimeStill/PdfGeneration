import { Person } from '../../models';

import {
  Component,
  Input,
  Output,
  EventEmitter
} from '@angular/core';

@Component({
  selector: 'person-card',
  templateUrl: 'person-card.component.html'
})
export class PersonCardComponent {
  @Input() person: Person;
  @Input() closable = false;
  @Input() viewable = false;
  @Input() pinnable = true;
  @Input() editable = true;
  @Input() removable = true;
  @Input() getPDF = true;
  @Input() actionLayout = 'space-evenly center';
  @Output() close = new EventEmitter();
  @Output() view = new EventEmitter<Person>();
  @Output() pin = new EventEmitter<Person>();
  @Output() edit = new EventEmitter<Person>();
  @Output() remove = new EventEmitter<Person>();
  @Output() getPdf = new EventEmitter<Person>();
}

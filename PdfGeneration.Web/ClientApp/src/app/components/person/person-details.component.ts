import {
  Component,
  Input
} from '@angular/core';

@Component({
  selector: 'person-details',
  templateUrl: 'person-details.component.html'
})
export class PersonDetailsComponent {
  @Input() personId: string;
}

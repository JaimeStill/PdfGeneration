import {
  Component,
  OnInit
} from '@angular/core';

import {
  ActivatedRoute,
  Router
} from '@angular/router';

import { PersonService } from '../../../services';
import { Person } from '../../../models';

@Component({
  selector: 'person-associates-route',
  templateUrl: 'person-associates.component.html',
  providers: [ PersonService ]
})
export class PersonAssociatesComponent implements OnInit {
  constructor(
    public service: PersonService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit() {
    this.route.parent.paramMap.subscribe(async map => {
      if (map.has('id')) {
        const id = parseInt(map.get('id'));

        if (id) {
          const res = await this.service.getPersonAssociates(id);
          !res && this.navigate();
        } else {
          this.navigate();
        }
      } else {
        this.navigate();
      }
    })
  }

  private navigate = () => this.router.navigate(['/people']);

  viewPerson = (person: Person) => this.router.navigate(['person', person.id]);
  pinPerson = (person: Person) => this.router.navigate([{ outlets: { side: `person/${person.id}`}}]);
}

import {
  Component,
  OnInit
} from '@angular/core';

import {
  ActivatedRoute,
  Router
} from '@angular/router';

import { MatDialog } from '@angular/material';
import { PersonService } from '../../services';
import { Person } from '../../models';

import {
  ConfirmDialog,
  PersonDialog
} from '../../dialogs';

@Component({
  selector: 'person-route',
  templateUrl: 'person.component.html',
  providers: [ PersonService ]
})
export class PersonComponent implements OnInit {
  personId: number;

  constructor(
    public service: PersonService,
    private route: ActivatedRoute,
    private router: Router,
    private dialog: MatDialog
  ) { }

  ngOnInit() {
    this.route.paramMap.subscribe(async map => {
      if (map.has('id')) {
        this.personId = parseInt(map.get('id'));

        if (this.personId) {
          const res = await this.service.getPerson(this.personId);
          !res && this.navigate();
        } else {
          this.navigate();
        }
      } else {
        this.navigate();
      }
    });
  }

  private navigate = () => this.router.navigate(['/people']);

  pinPerson = (person: Person) => this.router.navigate([{ outlets: { side: `person/${person.id}`}}]);

  editPerson = (person: Person) => this.dialog.open(PersonDialog, {
    data: { isAssociate: false, person: Object.assign({} as Person, person) },
    width: '800px',
    disableClose: true
  })
  .afterClosed()
  .subscribe(res => res && this.service.getPerson(this.personId));

  removePerson = (person: Person) => this.dialog.open(ConfirmDialog)
    .afterClosed()
    .subscribe(async res => {
      if (res) {
        const result = await this.service.togglePersonDeleted(person);
        result && this.navigate();
      }
    });

    getPdf = (person: Person) => this.dialog.open(ConfirmDialog)
    .afterClosed()
    .subscribe(async result => {
      if (result) {
        const res = await this.service.getPdf(person, person.id);
        res && this.navigate();
      }
    });
}

import {
  Component,
  OnInit
} from '@angular/core';

import { Router } from '@angular/router';
import { MatDialog } from '@angular/material';
import { PersonService } from '../../services';
import { Person } from '../../models';
import { PersonDialog } from '../../dialogs';

@Component({
  selector: 'people-route',
  templateUrl: 'people.component.html',
  providers: [ PersonService ]
})
export class PeopleComponent implements OnInit {
  constructor(
    public dialog: MatDialog,
    public router: Router,
    public service: PersonService
  ) { }

  ngOnInit() {
    this.service.getPeople();
  }

  viewPerson = (person: Person) => person && this.router.navigate(['/person', person.id]);

  addPerson = () => this.dialog.open(PersonDialog, {
    data: { isAssociate: false, person: {} as Person },
    width: '800px',
    disableClose: true
  })
  .afterClosed()
  .subscribe(res => res && this.service.getPeople());
}

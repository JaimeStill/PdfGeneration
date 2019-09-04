import { PersonService } from '../../services';
import { Person } from '../../models';

import {
  Component,
  Inject,
  OnInit
} from '@angular/core';

import {
  MatDialogRef,
  MAT_DIALOG_DATA
} from '@angular/material';

@Component({
  selector: 'person-dialog',
  templateUrl: 'person.dialog.html',
  providers: [ PersonService ]
})
export class PersonDialog implements OnInit {
  dialogTitle = 'Add Person';
  person: Person;

  constructor(
    public dialogRef: MatDialogRef<PersonDialog>,
    @Inject(MAT_DIALOG_DATA) public data: {
      isAssociate: boolean,
      person: Person,
      personId: number
    },
    public service: PersonService
  ) { }

  ngOnInit() {
    this.person = this.data && this.data.person ?
      this.data.person :
      {} as Person;

    this.dialogTitle = this.data.isAssociate ? 'Add Associate' :
      this.person.id ? 'Update Person' : 'Add Person';
  }

  async savePerson() {
    const res = this.data.isAssociate ?
      await this.service.addPersonAssociate(this.data.personId, this.person) :
      this.person.id ?
        await this.service.updatePerson(this.person) :
        await this.service.addPerson(this.person);

    res && this.dialogRef.close(true);
  }
}

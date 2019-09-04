import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { SnackerService } from '../snacker.service';

import {
  Person,
  PersonAssociate
} from '../../models';

@Injectable()
export class PersonService {
  private people = new BehaviorSubject<Person[]>(null);
  private truePeople = new BehaviorSubject<Person[]>(null);
  private associates = new BehaviorSubject<Person[]>(null);
  private person = new BehaviorSubject<Person>(null);
  private truePersonId = new BehaviorSubject<number>(null);
  private truePerson = new BehaviorSubject<Person>(null);

  // pdf: FormData;
  people$ = this.people.asObservable();
  truePeople$ = this.truePeople.asObservable();
  associates$ = this.associates.asObservable();

  person$ = this.person.asObservable();
  truePersonId$ = this.truePersonId.asObservable();
  truePerson$ = this.truePerson.asObservable();

  constructor(
    private http: HttpClient,
    private snacker: SnackerService,
  ) { }

  // public upload: UploadService;

  getPeople = () => this.http.get<Person[]>('/api/person/getPeople')
    .subscribe(
      data => this.people.next(data),
      err => this.snacker.sendErrorMessage(err.error)
    );

  getDeletedPeople = () => this.http.get<Person[]>('/api/person/getDeletedPeople')
    .subscribe(
      data => this.people.next(data),
      err => this.snacker.sendErrorMessage(err.error)
    );

  getTruePeople = () => this.http.get<Person[]>('/api/person/getTruePeople')
    .subscribe(
      data => this.truePeople.next(data),
      err => this.snacker.sendErrorMessage(err.error)
    );

  getDeletedTruePeople = () => this.http.get<Person[]>('/api/person/getDeletedTruePeople')
    .subscribe(
      data => this.truePeople.next(data),
      err => this.snacker.sendErrorMessage(err.error)
    );

  getAssociates = () => this.http.get<Person[]>('/api/person/getAssociates')
    .subscribe(
      data => this.associates.next(data),
      err => this.snacker.sendErrorMessage(err.error)
    );

  getDeletedAssociates = () => this.http.get<Person[]>('/api/person/getDeletedAssociates')
    .subscribe(
      data => this.associates.next(data),
      err => this.snacker.sendErrorMessage(err.error)
    );

  searchPeople = (search: string) => this.http.get<Person[]>(`/api/person/searchPeople/${search}`)
    .subscribe(
      data => this.people.next(data),
      err => this.snacker.sendErrorMessage(err.error)
    );

  searchDeletedPeople = (search: string) => this.http.get<Person[]>(`/api/person/searchDeletedPeople/${search}`)
    .subscribe(
      data => this.people.next(data),
      err => this.snacker.sendErrorMessage(err.error)
    );

  getPerson = (id: number): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.get<Person>(`/api/person/getPerson/${id}`)
        .subscribe(
          data => {
            this.person.next(data);
            resolve(true);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });

  addPerson = (person: Person): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.post('/api/person/addPerson', person)
        .subscribe(
          () => {
            this.snacker.sendSuccessMessage(`${person.firstName} ${person.lastName} successfully created`);
            resolve(true);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });

  updatePerson = (person: Person): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.post('/api/person/updatePerson', person)
        .subscribe(
          () => {
            this.snacker.sendSuccessMessage(`${person.firstName} ${person.lastName} successfully updated`);
            resolve(true);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });

  togglePersonDeleted = (person: Person): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.post('/api/person/togglePersonDeleted', person)
        .subscribe(
          () => {
            const message = person.isDeleted ?
              `${person.firstName} ${person.lastName} successfully restored` :
              `${person.firstName} ${person.lastName} successfully deleted`;

            this.snacker.sendSuccessMessage(message);
            resolve(true);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });

  removePerson = (person: Person): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.post('/api/person/removePerson', person)
        .subscribe(
          () => {
            this.snacker.sendSuccessMessage(`${person.firstName} ${person.lastName} permanently deleted`);
            resolve(true);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });

  getPersonAssociates = (personId: number): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.get<Person[]>(`/api/person/getPersonAssociates/${personId}`)
      .subscribe(
        data => {
          this.associates.next(data);
          resolve(true);
        },
        err => {
          this.snacker.sendErrorMessage(err.error);
          resolve(false);
        }
      );
    })

  getDeletedPersonAssociates = (personId: number) =>
    this.http.get<Person[]>(`/api/person/getDeletedPersonAssociates/${personId}`)
      .subscribe(
        data => this.associates.next(data),
        err => this.snacker.sendErrorMessage(err.error)
      );

  getTruePersonId = (personId: number): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.get<number>(`/api/person/getTruePersonId/${personId}`)
        .subscribe(
          data => {
            this.truePersonId.next(data);
            resolve(true);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });

  getTruePerson = (personId: number): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.get<Person>(`/api/person/getTruePerson/${personId}`)
        .subscribe(
          data => {
            this.truePerson.next(data);
            resolve(true);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });

  getAssociatePerson = (personId: number): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.get<Person>(`/api/person/getAssociatePerson/${personId}`)
        .subscribe(
          data => {
            this.truePerson.next(data);
            resolve(true);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });

  addAssociateToPerson = (personId: number, associate: Person): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.post(`/api/person/addAssociateToPerson/${personId}`, associate)
        .subscribe(
          () => {
            this.snacker.sendSuccessMessage(`${associate.firstName} ${associate.lastName} successfully assigned to person`);
            resolve(true);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });

  addPersonAssociate = (personId: number, associate: Person): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.post(`/api/person/addPersonAssociate/${personId}`, associate)
        .subscribe(
          () => {
            this.snacker.sendSuccessMessage(`${associate.firstName} ${associate.lastName} successfully created and assigned to person`);
            resolve(true);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });

  removePersonAssociate = (personAssociate: PersonAssociate): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.post('/api/person/removePersonAssociate', personAssociate)
        .subscribe(
          () => {
            this.snacker.sendSuccessMessage('Associate successfully removed from person');
            resolve(true);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });

    // setPDF(pdf: FormData) {
    //   this.pdf = pdf;
    // }

    getPdf = (person: Person, id: number): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.post<Person>(`/api/person/getPdf`, person)
        .subscribe(
          data => {
            this.person.next(data);
            resolve(true);
            this.snacker.sendSuccessMessage('Downloaded');
            // this.upload.uploadFiles(this.pdf, 3);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });
}

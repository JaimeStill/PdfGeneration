import { Person } from './person';

export interface PersonAssociate {
  id: number;
  associateId: number;
  personId: number;

  associate: Person;
  person: Person;
}

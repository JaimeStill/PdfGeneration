import { Upload } from './upload';

export interface User {
  id: number;
  guid: string;
  firstName: string;
  lastName: string;
  username: string;
  email: string;
  socketName: string;
  theme: string;
  isDeleted: boolean;

  uploads: Upload[];
}

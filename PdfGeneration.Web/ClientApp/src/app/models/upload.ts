import { User } from './user';

export interface Upload {
  id: number;
  userId: number;
  url: string;
  path: string;
  file: string;
  name: string;
  fileType: string;
  size: number;
  uploadDate: Date;
  isDeleted: boolean;

  user: User;
}

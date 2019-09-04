import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { SnackerService } from './snacker.service';

import {
  AdUser,
  User
} from '../models';

@Injectable()
export class IdentityService {
  private domainUsers = new BehaviorSubject<AdUser[]>(null);
  private users = new BehaviorSubject<User[]>(null);

  private user = new BehaviorSubject<User>(null);
  private currentUserId = new BehaviorSubject<number>(null);
  private currentUser = new BehaviorSubject<User>(null);

  domainUsers$ = this.domainUsers.asObservable();
  users$ = this.users.asObservable();

  user$ = this.user.asObservable();
  currentUserId$ = this.currentUserId.asObservable();
  currentUser$ = this.currentUser.asObservable();

  constructor(
    private http: HttpClient,
    private snacker: SnackerService
  ) { }

  getDomainUsers = () => this.http.get<AdUser[]>('/api/identity/getDomainUsers')
    .subscribe(
      data => this.domainUsers.next(data),
      err => this.snacker.sendErrorMessage(err.error)
    );

  findDomainUser = (search: string) => this.http.get<AdUser[]>(`/api/identity/findDomainUser/${search}`)
    .subscribe(
      data => this.domainUsers.next(data),
      err => this.snacker.sendErrorMessage(err.error)
    );

  getUsers = () => this.http.get<User[]>('/api/identity/getUsers')
    .subscribe(
      data => this.users.next(data),
      err => this.snacker.sendErrorMessage(err.error)
    );

  getDeletedUsers = () => this.http.get<User[]>('/api/identity/getDeletedUsers')
    .subscribe(
      data => this.users.next(data),
      err => this.snacker.sendErrorMessage(err.error)
    );

  searchUsers = (search: string) => this.http.get<User[]>(`/api/identity/searchUsers/${search}`)
    .subscribe(
      data => this.users.next(data),
      err => this.snacker.sendErrorMessage(err.error)
    );

  searchDeletedUsers = (search: string) => this.http.get<User[]>(`/api/identity/searchDeletedUsers/${search}`)
    .subscribe(
      data => this.users.next(data),
      err => this.snacker.sendErrorMessage(err.error)
    );

  getUser = (id: number): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.get<User>(`/api/identity/getUser/${id}`)
        .subscribe(
          data => {
            this.user.next(data);
            resolve(true);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });

  getCurrentUserId = (guid: string): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.get<number>(`/api/identity/getCurrentUserId/${guid}`)
        .subscribe(
          data => {
            this.currentUserId.next(data);
            resolve(true);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });

  syncUser = () => this.http.get<User>(`/api/identity/syncUser`)
    .subscribe(
      data => this.currentUser.next(data),
      err => this.snacker.sendErrorMessage(err.error)
    );

  addUser = (user: User): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.post('/api/identity/addUser', user)
        .subscribe(
          () => {
            this.snacker.sendSuccessMessage(`${user.username} successfully added`);
            resolve(true);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });

  updateUser = (user: User): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.post('/api/identity/updateUser', user)
        .subscribe(
          () => {
            this.snacker.sendSuccessMessage(`${user.username} successfully updated`);
            resolve(true);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });

  toggleUserDeleted = (user: User): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.post('/api/identity/toggleUserDeleted', user)
        .subscribe(
          () => {
            const message = user.isDeleted ?
              `${user.username} successfully restored` :
              `${user.username} successfully deleted`;

            this.snacker.sendSuccessMessage(message);
            resolve(true);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });

  removeUser = (user: User): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.post('/api/identity/removeUser', user)
        .subscribe(
          () => {
            this.snacker.sendSuccessMessage(`${user.username} permanently deleted`);
            resolve(true);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });
}

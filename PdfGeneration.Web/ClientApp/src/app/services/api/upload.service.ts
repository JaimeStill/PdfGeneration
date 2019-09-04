import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { CoreService } from '../core.service';
import { SnackerService } from '../snacker.service';
import { Upload } from '../../models';

@Injectable()
export class UploadService {
  private uploads = new BehaviorSubject<Upload[]>(null);
  private upload = new BehaviorSubject<Upload>(null);

  uploads$ = this.uploads.asObservable();
  upload$ = this.upload.asObservable();

  constructor(
    private http: HttpClient,
    private snacker: SnackerService,
    private core: CoreService
  ) { }

  getUploads = () => this.http.get<Upload[]>('/api/upload/getUploads')
    .subscribe(
      data => this.uploads.next(data),
      err => this.snacker.sendErrorMessage(err.error)
    );

  getDeletedUploads = () => this.http.get<Upload[]>('/api/upload/getDeletedUploads')
    .subscribe(
      data => this.uploads.next(data),
      err => this.snacker.sendErrorMessage(err.error)
    );

  searchUploads = (search: string) => this.http.get<Upload[]>(`/api/upload/searchUploads/${search}`)
    .subscribe(
      data => this.uploads.next(data),
      err => this.snacker.sendErrorMessage(err.error)
    );

  searchDeletedUploads = (search: string) => this.http.get<Upload[]>(`/api/upload/searchDeletedUploads/${search}`)
    .subscribe(
      data => this.uploads.next(data),
      err => this.snacker.sendErrorMessage(err.error)
    );

  getUpload = (id: number): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.get<Upload>(`/api/upload/getUpload/${id}`)
        .subscribe(
          data => {
            this.upload.next(data);
            resolve(true);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });

  uploadFiles = (formData: FormData, userId: number): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.post(
        `/api/upload/uploadFiles/${userId}`,
        formData,
        { headers: this.core.getUploadOptions() }
      )
      .subscribe(
        () => {
          this.snacker.sendSuccessMessage('Uploads successfully processed');
          resolve(true);
        },
        err => {
          this.snacker.sendErrorMessage(err.error);
          resolve(false);
        }
      );
    });

  toggleUploadDeleted = (upload: Upload): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.post('/api/upload/toggleUploadDeleted', upload)
        .subscribe(
          () => {
            const message = upload.isDeleted ?
              `${upload.file} successfully restored` :
              `${upload.file} successfully deleted`;

            this.snacker.sendSuccessMessage(message);
            resolve(true);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });

  removeUpload = (upload: Upload): Promise<boolean> =>
    new Promise((resolve) => {
      this.http.post('/api/upload/removeUpload', upload)
        .subscribe(
          () => {
            this.snacker.sendSuccessMessage(`${upload.file} permanently deleted`);
            resolve(true);
          },
          err => {
            this.snacker.sendErrorMessage(err.error);
            resolve(false);
          }
        );
    });
}

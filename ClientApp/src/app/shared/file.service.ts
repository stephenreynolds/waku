import { Injectable } from '@angular/core';
import { HttpRequest, HttpEventType, HttpClient, HttpHeaders } from '@angular/common/http';
import { UserService } from './user.service';
import { map } from 'rxjs/operators';

@Injectable()
export class FileService {

  public message: string;
  public filename: string;  // File name returned from upload controller.

  constructor(private http: HttpClient, private userService: UserService) {

  }

  public delete(filename: string) {
    return this.http.delete(`api/file/${filename}`, {
      headers: new HttpHeaders().set('Authorization', `Bearer ${this.userService.getToken()}`)
    });
  }

  public upload(files) {
    if (files.length === 0) {
      return;
    }

    const formData = new FormData();

    for (const file of files) {
      formData.append(file.name, file);
    }

    return this.http.post('api/file', formData, {
      headers: new HttpHeaders().set('Authorization', `Bearer ${this.userService.getToken()}`)
    }).pipe(
      map((data: any) => {
        this.filename = data.filename;
        return true;
      })
    );
  }
}

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BlogPost } from './blogPost';
import { UserService } from './user.service';

@Injectable()
export class BlogService {

  public count: number;
  public posts: BlogPost[] = [];

  constructor(private http: HttpClient, private userService: UserService) {}

  public getCount(): Observable<boolean> {
    return this.http.get('api/blog/count').pipe(
      map((data: any) => {
        this.count = data;
        return true;
      })
    );
  }

  public loadPosts(model: { page: number; size: number }): Observable<boolean> {
    return this.http.post('api/blog/getpage', model).pipe(
      map((data: any[]) => {
        this.posts = data;
        return true;
      })
    );
  }

  public createPost(model: { title: string; subtitle: string; thumbnail: string; content: string; }) {
    return this.http.post('api/blog/createpost', model, {
      headers: new HttpHeaders().set('Authorization', `Bearer ${this.userService.getToken()}`)
    });
  }
}

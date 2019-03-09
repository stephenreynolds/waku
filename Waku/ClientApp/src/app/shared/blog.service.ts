import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { BlogPost } from './blogPost';

@Injectable()
export class BlogService {

  public posts: BlogPost[] = [];

  constructor(private http: HttpClient) {}

  public loadPosts(model: { page: number; size: number }): Observable<boolean> {
    const start = (model.page - 1) * model.size;
    const end = start * model.size;
    return this.http.get(`/api/blog?start=${start}&end=${end}`)
      .pipe(
        map((data: any[]) => {
          this.posts = data;
          return true;
        })
      );
  }
}

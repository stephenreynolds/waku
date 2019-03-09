import { Component, OnInit } from '@angular/core';
import { BlogPost } from '../shared/blogPost';
import { BlogService } from '../shared/blog.service';

@Component({
  selector: 'app-blog',
  templateUrl: './blog.component.html'
})
export class BlogComponent implements OnInit {
  public posts: BlogPost[];
  public itemsPerPage: number;
  public totalItems: any;
  public page: any;
  public previousPagee: any;

  constructor(private blogService: BlogService) {

  }

  public loadPage(page: number) {
    if (page !== this.previousPagee) {
      this.previousPagee = page;
      this.loadData();
    }
  }

  public ngOnInit() {
    this.loadPage(1);
  }

  private loadData() {
    this.blogService
      .loadPosts({
        page: this.page - 1,
        size: this.itemsPerPage
      })
      .subscribe(success => {
        this.posts = this.blogService.posts;
      });
  }
}

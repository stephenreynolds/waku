import { Component, OnInit } from '@angular/core';
import { BlogPost } from '../shared/blogPost';
import { BlogService } from '../shared/blog.service';

@Component({
  selector: 'app-blog',
  templateUrl: './blog.component.html',
  styleUrls: ['./blog.component.scss']
})
export class BlogComponent implements OnInit {

  public itemsPerPage: number = 9;
  public page: any;
  public posts: BlogPost[];
  public previousPage: any;
  public totalItems: any;

  constructor(private blogService: BlogService) {}

  public loadPage(page: number) {
    if (page !== this.previousPage) {
      this.previousPage = page;
      this.page = page;
      this.loadData();
    }
  }

  public ngOnInit() {
    this.loadPage(1);
  }

  private loadData() {
    this.blogService
      .loadPosts({
        page: this.page,
        size: this.itemsPerPage
      })
      .subscribe(success => {
        this.posts = this.blogService.posts;
      });
  }
}

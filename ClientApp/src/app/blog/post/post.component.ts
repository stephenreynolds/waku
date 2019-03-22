import { Component, OnInit } from '@angular/core';
import { BlogService } from 'src/app/shared/blog.service';
import { BlogPost } from 'src/app/shared/blogPost';
import { UserService } from 'src/app/shared/user.service';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';

@Component({
  selector: 'app-blog-post',
  templateUrl: './post.component.html',
  styleUrls: ['./post.component.scss']
})
export class PostComponent implements OnInit {

  public post: BlogPost;

  constructor(private blogService: BlogService, public userService: UserService, public route: ActivatedRoute, private router: Router) {

  }

  public ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    this.blogService.getPost(id).subscribe((data) => {
      if (data) {
        this.post = data;
      }
      else {
        this.router.navigate(['/error/notfound']);
      }
    });
  }
}

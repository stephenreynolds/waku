import { BlogService } from '../shared/blog.service';
import { Component } from '@angular/core';

@Component({
  selector: 'app-blog-createpost',
  templateUrl: './createpost.component.html'
})
export class CreatePostComponent {

  constructor(private blogService: BlogService) {

  }

  private onSubmit() {

  }
}

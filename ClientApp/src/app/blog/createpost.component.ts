import { BlogService } from '../shared/blog.service';
import { Component } from '@angular/core';

@Component({
  selector: 'app-blog-createpost',
  templateUrl: './createpost.component.html'
})
export class CreatePostComponent {

  constructor(private blogService: BlogService) {

  }

  private onFileChange(event) {
    const reader = new FileReader();

    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      reader.readAsDataURL(file);

      reader.onload = () => {
        // See https://medium.com/@amcdnl/file-uploads-with-angular-reactive-forms-960fd0b34cb5
      };
    }
  }

  private onSubmit() {

  }
}

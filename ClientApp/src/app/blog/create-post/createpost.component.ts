import { BlogService } from '../../shared/blog.service';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FileService } from '../../shared/file.service';
import { Md5 } from 'ts-md5/dist/md5';

@Component({
  selector: 'app-blog-createpost',
  templateUrl: './createpost.component.html',
  styleUrls: ['createpost.component.scss']
})
export class CreatePostComponent {

  public post = {
    title: '',
    subtitle: '',
    thumbnail: '',
    content: ''
  };

  public thumbnailPreview: string = '';
  public thumbnailFile;
  public errorMessage: string = '';

  constructor(private blogService: BlogService, public fileService: FileService, private router: Router) {}

  public getDate(): Date {
    return new Date();
  }

  public onFileChange(files) {
    if (files && files.length) {
      // Read the image.
      const [file] = files;
      const reader = new FileReader();
      reader.readAsDataURL(file);

      // Set thumbnail preview.
      reader.onload = () => {
        this.thumbnailPreview = reader.result.toString();
      };

      this.thumbnailFile = files;
    }
  }

  public onSubmit() {
    this.fileService.upload(this.thumbnailFile).subscribe(
      uploaded => {
        this.post.thumbnail = this.fileService.filename;

        this.blogService.createPost(this.post).subscribe(
          success => {
            if (success) {
              this.router.navigate(['/blog']);
            }
          },
          err => (this.errorMessage = 'Failed to create new post.')
        );
      },
      err => (this.errorMessage = 'Failed to upload thumbnail.')
    );
  }

  public onCancel() {
    this.fileService.delete(this.post.thumbnail);
  }
}

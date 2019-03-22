import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { UserService } from './shared/user.service';
import { BlogComponent } from './blog/blog.component';
import { BlogService } from './shared/blog.service';
import { CreatePostComponent } from './blog/create-post/createpost.component';
import { FileService } from './shared/file.service';
import { PostComponent } from './blog/post/post.component';
import { NotFoundComponent } from './error/notfound/notfound.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    LoginComponent,
    BlogComponent,
    CreatePostComponent,
    PostComponent,
    NotFoundComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    NgbModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'login', component: LoginComponent },
      { path: 'blog', component: BlogComponent },
      { path: 'blog/createpost', component: CreatePostComponent },
      { path: 'blog/post/:id', component: PostComponent },
      { path: '**', component: NotFoundComponent }
    ])
  ],
  providers: [
    BlogService,
    FileService,
    UserService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

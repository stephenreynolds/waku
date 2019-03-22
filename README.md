[![Build Status](https://travis-ci.com/stephenreynolds/waku.svg?branch=master)](https://travis-ci.com/stephenreynolds/waku)

# stephenreynolds.me

stephenreynolds.me (aka Waku) is my personal website implemented in ASP.NET Core and Angular. It features a basic CMS capable of managing my resume, projects, and blog posts.

## Building

Install the required NPM packages using `npm install` within `./ClientApp/`.

Rename `appsettings.Development.Sample.json` to `appsettings.Development.json`. Within this file make sure a valid database connection string, token key/issuer, and default user credentials are provided.

You can then use `dotnet run` from the root of the project to build and run the server (on `localhost:5000` by default). Use `dotnet build` to build without running.

## Roadmap

- [ ] User accounts
  - [x] Default user account
  - [x] Login/logout
  - [ ] Ability to change username and password
- [ ] File Management
  - [x] Upload/download through API
  - [ ] Upload/download UI
  - [ ] File browser
- [ ] Blogging
  - [x] Basic post creation
  - [ ] Make posts viewable
  - [ ] WYSIWYG content (markdown)
  - [ ] Post editing and deletion
- [ ] Projects
  - [ ] Project creation
  - [ ] GitHub integration
- [ ] Resume
  - [ ] Ability to edit resume
- [ ] Commenting (likely using Disqus)
- [ ] Search
- [ ] Social network integration
- [ ] Theme support

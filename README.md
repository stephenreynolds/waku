[![Build Status](https://travis-ci.com/stephenreynolds/waku.svg?branch=master)](https://travis-ci.com/stephenreynolds/waku)

# stephenreynolds.me

stephenreynolds.me (aka Waku) is my personal website implemented in ASP.NET Core and Angular.
It features a basic CMS capable of managing my resume, projects, and blog posts.

I named my CMS "Waku" because it represents what my site is supposed to be and how I want users to feel when using my site.
It is Japanese for "excited" (沸く), "framework" (枠), and "to feel emotions form" (湧く).

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
  - [x] Make posts viewable
  - [ ] WYSIWYG content (markdown)
  - [ ] Post editing and deletion
- [ ] Projects
  - [ ] Project creation
  - [ ] GitHub integration
- [ ] Resume
  - [ ] Ability to edit resume
- [x] Commenting (likely using Disqus)
- [ ] Search
- [ ] Social network integration
- [ ] Theme support

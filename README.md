Git Dependency Resolver (DEVELOP)
===

**NOTE: This branch is for development purposes only.**  
**NOTE: To use a released package, see [Releases page](https://github.com/mob-sakai/GitDependencyResolverForUnity/releases) or [default branch](https://github.com/mob-sakai/GitDependencyResolverForUnity).**

<br><br><br>

## How to contribute this repository

See [CONTRIBUTING.md](https://github.com/mob-sakai/GitDependencyResolverForUnity/blob/develop/CONTRIBUTING.md) and [CODE_OF_CONDUCT.md](https://github.com/mob-sakai/GitDependencyResolverForUnity/blob/develop/CODE_OF_CONDUCT.md).

<br><br><br>

## How to develop

1. Fork this repository.
1. Clone the forked repository to local.
1. Create your branch from `develop` branch.
1. Develop the package.
1. Commit with a message based on [Conventional Commits](https://www.conventionalcommits.org/).
1. Fill out the description, link any related issues and submit your pull request.  
   **NOTE: Create a pull request to merge into `develop` branch**

### Branch strategy

| Branch         | Description                | Manual Push        | Pull request<br>(For developing) | Pull request<br>(For releasing) | Unity project      |
| -------------- | -------------------------- | ------------------ | -------------------------------- | ------------------------------- | ------------------ |
| upm (default)  | For publishing             | :x:                | :x:                              | :x:                             | :x:                |
| develop        | For developing the package | :white_check_mark: | :white_check_mark:               | :x:                             | :white_check_mark: |
| preview        | For preview releasing      | :x:                | :x:                              | :white_check_mark:              | :white_check_mark: |
| main           | For main releasing         | :x:                | :x:                              | :white_check_mark:              | :white_check_mark: |
| vN.x (eg. 1.x) | For maintainance releasing | :x:                | :x:                              | :white_check_mark:              | :white_check_mark: |

- Develop packages in the `develop` branch
- "The development pull request" will be merged into the `develop` branch
- "The release pull request" will be merged into the `preivew` or `main` branch
  - These are the "pointer" branches for releasing packages

### Committed messages in the most common cases

| Case | Commit message|
| -- | -- |
| Added a new feature | feat: add new feature |
| Added a suggested feature #999 | feat: add new feature<br>Close #999 |
| Fixed a bug | fix: a problem |
| Fixed a reported bug #999 | fix: a problem<br>Close #999 |
| Added features that include breaking changes | feat: add new feature<br><br>BREAKING CHANGE: Details of the changes |

<br><br><br>

## How to release

**NOTE: The contributor does not need to perform a release operation.**  

When you push to `preview`, `master` or `vN.x` (eg. `v1.x`, `v2.x`) branch, this package is automatically released by GitHub Action.  
Internally, a npm tool [semantic-release](https://semantic-release.gitbook.io/semantic-release/) is used to release.  

The new version will be determined based on [Conventional Commits](https://www.conventionalcommits.org/).  
Committers should follow it.

- fix: a commit of the type fix patches a bug in your codebase (this correlates with PATCH in semantic versioning).
- feat: a commit of the type feat introduces a new feature to the codebase (this correlates with MINOR in semantic versioning).
- BREAKING CHANGE: a commit that has a footer BREAKING CHANGE:, or appends a ! after the type/scope, introduces a breaking API change (correlating with MAJOR in semantic versioning). A BREAKING CHANGE can be part of commits of any type.

### Main Release

[Create pull request to main release](https://github.com/mob-sakai/GitDependencyResolverForUnity/compare/main...develop?expand=1)

- Release the package as a stable version.
- The associated issue or pull request will be tagged with a `released` tag.

### Preview Release

[Create pull request to preview release](https://github.com/mob-sakai/GitDependencyResolverForUnity/compare/preview...develop?expand=1)

- Release the package as a **unstable** version.
- The version will have a `-preview.x` suffix
  - eg. `1.0.0-preview.1`
- The associated issue or pull request will be tagged with a `released on @preview` tag.

### Maintainance Release

- Branches like `v.1.x`, `v.2.x`, etc. are for maintenance releases.
- Used for releases that are not the latest version.
- The associated issue or pull request will be tagged with a `released` tag.

### Alternative way

You can release it manually with the following command:

```bash
$ npm run release -- --no-ci
```


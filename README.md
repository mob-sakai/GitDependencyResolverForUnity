Git Dependency Resolver For Unity
===

This plugin resolves git-based dependencies in the package for Unity Package Manager.  
You can use git repositories url as a package dependencies! :+1:

![logo](https://user-images.githubusercontent.com/12690315/57779067-636a7e00-7760-11e9-8f4a-06bbaee402e8.png)

[![](https://img.shields.io/npm/v/com.coffee.git-dependency-resolver?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.coffee.git-dependency-resolver/)
[![](https://img.shields.io/github/v/release/mob-sakai/GitDependencyResolverForUnity?include_prereleases)](https://github.com/mob-sakai/GitDependencyResolverForUnity/releases)
[![](https://img.shields.io/github/release-date/mob-sakai/GitDependencyResolverForUnity.svg)](https://github.com/mob-sakai/GitDependencyResolverForUnity/releases)
![](https://img.shields.io/badge/unity-2018.3%20or%20later-green.svg)
[![](https://img.shields.io/github/license/mob-sakai/GitDependencyResolverForUnity.svg)](https://github.com/mob-sakai/GitDependencyResolverForUnity/blob/upm/LICENSE.txt)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-orange.svg)](http://makeapullrequest.com)
[![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai)

<< [Description](#Description) | [Install](#install) | [Usage](#usage) | [Development Note](#development-note) | [Contributing](#contributing) | [Change log][CL] >>

[CL]: https://github.com/mob-sakai/GitDependencyResolverForUnity/blob/upm/CHANGELOG.md



<br><br><br><br>

## Description

In Unity 2018.3, [the Unity Package Manager (UPM) supported Git](https://forum.unity.com/threads/git-support-on-package-manager.573673/). :)

This update allows us to quickly install packages on code hosting services such as GitHub.

However, UPM does not support git-based dependencies in the package. :(

```
[ package-a/package.json ]
{
  "name": "com.coffee.package-a",
  "version": "0.1.0",
  "dependencies": {
    "com.coffee.core-a" : "https://github.com/mob-sakai/GitPackageTest#core-a-0.1.0"
  }
}
```
When the above package is installed, the following error occur.
![console](https://user-images.githubusercontent.com/12690315/57829436-e84daa00-77e9-11e9-84af-f5e46b1f0f02.png)

**Git-based dependencies in packages** feature is on the roadmap for 2020, [but no specific ETA](https://forum.unity.com/threads/custom-package-with-git-dependencies.628390/#post-5367033).

<br>

This plugin resolves git-based dependencies in the package.  
You can use git repositories url as a package dependencies! :+1:


### Features

* Easy to use: just install
* Resolve git-based dependencies in packages
* Automatically uninstall unused packages that is installed by this plugin
* Support GitHub, Bitbucket, GitLab, etc.
* Support private repository
* Support Unity 2018.3 or later
* Support .Net 3.5 and 4.x
* Update package with a specific tag/branch/hash
* Deterministic package installation
* Support CI environment
* Support [path query parameter (sub-directory)](https://forum.unity.com/threads/some-feedback-on-package-manager-git-support.743345/) even Unity 2019.2 or earlier
  * path must be a relative path to the root of the repository.
  * path query parameter **must** be placed **before** the revision anchor. The reverse order will fail.
  * A package manifest (package.json) is expected in the specified path.
  * e.g. With Path query parameter: `https://github.com/user/repo.git?path=/example/folder`
  * e.g. With revision anchor and path query parameter: `https://github.com/user/repo.git?path=/example/folder#v1.2.3`


### Notes

From: https://forum.unity.com/threads/git-support-on-package-manager.573673/page-3#post-4552084

> There is no conflict detection and/or resolution algorithm.
> The lastest package found with the same name is used.
> This is not how the package manager resolve dependency (See https://docs.unity3d.com/Manual/upm-conflicts-auto.html).

In Unity's algorithm, package conflicts are resolved by "dependency-level from root".  
The all packages resolved by this plugin are "dependency-level=1".  
Therefore, in some cases, the package of the intended version may not be installed.

For example, in the case of a project with a dependency graph like this:

```
project (root)
 ├ package A: 1.0.0
 │  └ package X: 2.0.0
 └ package B: 1.0.0
    └ package C: 2.0.0
       └ package X: 2.0.1
```

**This plugin's algorithm**

Install -> A: 1.0.0, B: 1.0.0, C: 2.0.0, X: **2.0.1**

**Unity's algorithm**

Install -> A: 1.0.0, B: 1.0.0, C: 2.0.0, X: **2.0.0**



<br><br><br><br>

## Installation

### Requirement

* Unity 2018.3 or later

### Using OpenUPM

This package is available on [OpenUPM](https://openupm.com). 
You can install it via [openupm-cli](https://github.com/openupm/openupm-cli).
```
openupm add com.coffee.git-dependency-resolver
```

### Using Git

Find the `manifest.json` file in the `Packages` directory in your project and edit it as follows:
```
{
  "dependencies": {
    "com.coffee.git-dependency-resolver": "https://github.com/mob-sakai/GitDependencyResolverForUnity.git",
    ...
  },
}
```
To update the package, change suffix `#{version}` to the target version.

* e.g. `"com.coffee.git-dependency-resolver": "https://github.com/mob-sakai/GitDependencyResolverForUnity.git#1.2.0",`

Or, use [UpmGitExtension](https://github.com/mob-sakai/UpmGitExtension) to install and update the package.



<br><br><br><br>

## Usage

### For package user

* Install this plugin.
  * See [installation section](#installation).
* If the dependencies are not resolved successfully, reopen the project. If that does not work, try the following:
   1. Close the project.
   2. Delete `Library/ScriptAssemblies` directory in the project.
   3. Open the project.
* When `Unity Package Manager Error` window is opens, click `Continue`.  
![window](https://user-images.githubusercontent.com/12690315/57823865-08726e80-77d4-11e9-8203-46bf22d504d9.png)
* Add `Packages/.*` to `.gitignore` to hide auto-installed package. 

### For package developer

* Find the `package.json` file in your package and edit it as follows:
```
{
  ...
  "gitDependencies": {
    "your.package": "https://github.com/yourname/yourpackage.git#v1.2.3",
    ...
  }
}
```
* You can use [path query parameter (sub-directory)](https://forum.unity.com/threads/some-feedback-on-package-manager-git-support.743345/) even Unity 2019.2 or earlier.
  * e.g. `"your.package": "https://github.com/yourname/yourpackage.git?path=/pkg/dir#v1.2.3"`
  * path must be a relative path to the root of the repository.
  * path query parameter **must** be placed **before** the revision anchor. The reverse order will fail.
  * A package manifest (package.json) is expected in the specified path.
* You **must** use `gitDependencies` instead of `dependencies` to define git-based dependencies for the package.
  * This plugin also supports `dependencies` to resolve git-based dependencies, but if `dependencies` include packages that UPM can't resolve, it will fail to start Unity in CI environment.
* You **must** announce to your package users that they must install `com.coffee.git-dependency-resolver`.
  * See [installation section](#installation).
* It is recommended to use [SemVer](https://semver.org/) as a tag or branch name.  
  * e.g. `1.0.0`, `0.5.0-preview10`, `0.1.0-alpha+daily5`




<br><br><br><br>

## Contributing

### Issues

Issues are very valuable to this project.

- Ideas are a valuable source of contributions others can make
- Problems show where this project is lacking
- With a question you show where contributors can improve the user experience

### Pull Requests

Pull requests are, a great way to get your ideas into this repository.  
See [CONTRIBUTING.md](/../../blob/develop/CONTRIBUTING.md).

### Support

This is an open source project that I am developing in my spare time.  
If you like it, please support me.  
With your support, I can spend more time on development. :)

[![](https://user-images.githubusercontent.com/12690315/50731629-3b18b480-11ad-11e9-8fad-4b13f27969c1.png)](https://www.patreon.com/join/mob_sakai?)  
[![](https://user-images.githubusercontent.com/12690315/66942881-03686280-f085-11e9-9586-fc0b6011029f.png)](https://github.com/users/mob-sakai/sponsorship)




<br><br><br><br>

## License

* MIT
* [MiniJson](https://gist.github.com/darktable/1411710) by Calvin Rien
* [SemVer](https://github.com/maxhauser/semver) by Max Hauser



## Author

[mob-sakai](https://github.com/mob-sakai)
[![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai)



## See Also

* GitHub page : https://github.com/mob-sakai/GitDependencyResolverForUnity
* Releases : https://github.com/mob-sakai/GitDependencyResolverForUnity/releases
* Issue tracker : https://github.com/mob-sakai/GitDependencyResolverForUnity/issues
* Change log : https://github.com/mob-sakai/GitDependencyResolverForUnity/blob/upm/CHANGELOG.md

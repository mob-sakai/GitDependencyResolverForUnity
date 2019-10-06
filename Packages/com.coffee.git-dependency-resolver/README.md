Git Dependency Resolver For Unity
===

This plugin resolves git url dependencies in the package for Unity Package Manager.  
You can use a git url as a package dependency!

![logo](https://user-images.githubusercontent.com/12690315/57779067-636a7e00-7760-11e9-8f4a-06bbaee402e8.png)

[![](https://img.shields.io/github/release/mob-sakai/GitDependencyResolverForUnity.svg?label=latest%20version)](https://github.com/mob-sakai/GitDependencyResolverForUnity/releases)
[![](https://img.shields.io/github/release-date/mob-sakai/GitDependencyResolverForUnity.svg)](https://github.com/mob-sakai/GitDependencyResolverForUnity/releases)
![](https://img.shields.io/badge/unity-2018%20or%20later-green.svg)
[![](https://img.shields.io/github/license/mob-sakai/GitDependencyResolverForUnity.svg)](https://github.com/mob-sakai/GitDependencyResolverForUnity/blob/upm/LICENSE.txt)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-orange.svg)](http://makeapullrequest.com)
[![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai)

<< [Description](#Description) | [Install](#install) | [Usage](#usage) | [Demo](#demo) | [Development Note](#development-note) >>

### What's new? [See changelog ![](https://img.shields.io/github/release-date/mob-sakai/GitDependencyResolverForUnity.svg?label=last%20updated)](https://github.com/mob-sakai/GitDependencyResolverForUnity/blob/upm/CHANGELOG.md)
### Do you want to receive notifications for new releases? [Watch this repo ![](https://img.shields.io/github/watchers/mob-sakai/GitDependencyResolverForUnity.svg?style=social&label=Watch)](https://github.com/mob-sakai/GitDependencyResolverForUnity/subscription)
### Support me on Patreon! [![become_a_patron](https://user-images.githubusercontent.com/12690315/50731629-3b18b480-11ad-11e9-8fad-4b13f27969c1.png)](https://www.patreon.com/join/2343451?)



<br><br><br><br>
## Description

In Unity 2018.3, the Unity Package Manager (UPM) supported Git. :)  
https://forum.unity.com/threads/git-support-on-package-manager.573673/

This update allows us to quickly install packages on code hosting services such as GitHub.

However, UPM does not support git urls as dependencies in the package. :(

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
![console](https://user-images.githubusercontent.com/12690315/57829436-e84daa00-77e9-11e9-84af-f5e46b1f0f02.png)


<br>
This plugin resolves git url dependencies in the package for Unity Package Manager.

You can use a git url as a package dependency!


#### Features

* Easy to use: just install
* Resolve git url dependencies in packages
* Uninstall unused packages that is installed by this plugin
* Support GitHub, Bitbucket, GitLab, etc.
* Support private repository
* Support Unity 2019.1+
* Support .Net 3.5 & 4.x
* Update package with a specific tag/branch
* Deterministic package installation
* Refer to no files from the Library folder


#### Notes

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

Install -> A: 1.0.0, B: 1.0.0, C: 2.0.0, X: 2.0.1

**Unity's algorithm**
Install -> A: 1.0.0, B: 1.0.0, C: 2.0.0, X: **2.0.0**



<br><br><br><br>
## Install

Find `Packages/manifest.json` in your project and edit it to look like this:
```js
{
  "dependencies": {
    "com.coffee.git-dependency-resolver": "https://github.com/mob-sakai/GitDependencyResolverForUnity.git#1.1.3",
    ...
  }
}
```
To update the package, change `#{version}` to the target version.  
Or, use [UpmGitExtension](https://github.com/mob-sakai/UpmGitExtension.git) to install or update the package.


##### Requirement

* Unity 2018.3 or later



<br><br><br><br>
## Usage

* If dependencies are not resolved successfully, try the following:
    * Reopen the project.
    * Delete `Library` directory in the project.  
![library](https://user-images.githubusercontent.com/12690315/57830868-690ea500-77ee-11e9-9e47-4a9794d77da8.png)
* When `Unity Package Manager Error` window is opens, click `Continue`.  
![window](https://user-images.githubusercontent.com/12690315/57823865-08726e80-77d4-11e9-8203-46bf22d504d9.png)
* Use [SemVer](https://semver.org/) as a tag or branch name.  
eg. `1.0.0`, `0.5.0-preview10`, `0.1.0-alpha+daily5`   



<br><br><br><br>
## Demo

https://github.com/mob-sakai/UnityGitDependencyTest



<br><br><br><br>
## Development Note

#### Develop a package for UPM

The branching strategy when I develop a package for UPM is as follows.

|Branch|Description|'Assets' directory|
|-|-|-|
|develop|Development, Testing|Included|
|upm(default)|Subtree to publish for UPM|Excluded|
|{tags}|Tags to install using UPM|Excluded|

**Steps to release a package:**
1. Develop your package project on `develop` branch and update version in `package.json`.
2. Split subtree into `ump` branch.  
`git subtree split --prefix=Assets/YOUR/PACKAGE/DIRECTRY --branch upm`
3. Tag on `ump` branch as new version.
4. That's all. :)

For details, see https://www.patreon.com/posts/25070968.



<br><br><br><br>
## License

* MIT
* [MiniJson](https://gist.github.com/darktable/1411710) by Calvin Rien
* [SemVer](https://github.com/maxhauser/semver) by Max Hauser



## Author

[mob-sakai](https://github.com/mob-sakai)
[![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai)  
[![become_a_patron](https://user-images.githubusercontent.com/12690315/50731615-ce9db580-11ac-11e9-964f-e0423533dc69.png)](https://www.patreon.com/join/2343451?)



## See Also

* GitHub page : https://github.com/mob-sakai/GitDependencyResolverForUnity
* Releases : https://github.com/mob-sakai/GitDependencyResolverForUnity/releases
* Issue tracker : https://github.com/mob-sakai/GitDependencyResolverForUnity/issues
* Current project : https://github.com/mob-sakai/GitDependencyResolverForUnity/projects/1
* Change log : https://github.com/mob-sakai/GitDependencyResolverForUnity/blob/upm/CHANGELOG.md

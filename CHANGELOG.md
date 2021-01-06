# [2.0.0](https://github.com/mob-sakai/GitDependencyResolverForUnity/compare/1.1.3...2.0.0) (2021-01-06)


### Bug Fixes

* fix for ci ([f9c3772](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/f9c3772ff26a6d304c3ad65bb38fe1f1f5c2678c))
* fix unused package detection ([8ba02b9](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/8ba02b919f29505ce61e3a74525fb6b604500120))
* ignore empty directories in Packages/ ([2822ffd](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/2822ffdf6eb3f2cdee76025976b5d41d4f85c35a))


### Features

* Add copy directory feature to DirUtils ([6503db9](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/6503db9c571b9515cdc43bfff4ec845e87acf0c2))
* add log for  symbol ([fc3e9a3](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/fc3e9a3f9a467b9a0d717030db01ffedf70a8b7b))
* deterministic package installation ([3f60d80](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/3f60d802396964b30074b3f296d6ee3ec8623fe4))
* iterative package resolution ([c2a3871](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/c2a38715b1ac88c630d638070cefa2aaa4ac44fd))
* support  non semver ([e711602](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/e711602a5b6e2e9d30b47c6c979a3237cfd37810))
* support docker image such as unityui/editor ([e009fcc](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/e009fcc19dd07917c66e13b56208c416b19ecda7))
* support gitDependencies in package.json ([52b280d](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/52b280d0a26c2710d802da853c7acc359e721997))
* support path query parameter (sub-directory) even Unity 2019.2 or earlier ([0dfde55](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/0dfde55619e63895d00b76259c921b28e2e3cfef))


### BREAKING CHANGES

* Must use gitDependencies instead of dependencies to define git-based dependencies for the package.

# [2.0.0-preview.2](https://github.com/mob-sakai/GitDependencyResolverForUnity/compare/v2.0.0-preview.1...v2.0.0-preview.2) (2021-01-06)


### Bug Fixes

* Copy and deletes temporary package directory instead of moving it ([e04c5f1](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/e04c5f11f3f4d17951116d0da83d9469a60e632c))
* fix for ci ([ba3f246](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/ba3f2467728d48306534f009be9d8565383a4016))


### Features

* Add copy directory feature to DirUtils ([5e86160](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/5e86160eea0e0e1d811222c21886ca05e09b5da9))
* support docker image such as unityui/editor ([67ab523](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/67ab523d0293652ba9205b8c5d9c5cc628420c8c))

# [2.0.0-preview.1](https://github.com/mob-sakai/GitDependencyResolverForUnity/compare/v1.1.3...v2.0.0-preview.1) (2020-08-28)


### Bug Fixes

* fix unused package detection ([b057892](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/b0578920f7425a3f7bfed90247454fcb2621980e))
* ignore empty directories in Packages/ ([abb0c9f](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/abb0c9fc4e0281da3885ca9efd2f4b5a4d97fea0))


### Features

* add log for `GDR_LOG` symbol ([e5dacba](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/e5dacba880e308a9797ad9ca51874783c69d5865))
* deterministic package installation ([5485ff9](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/5485ff927a2e803acffcb4c1bdf6a2d96d2f552c))
* iterative package resolution ([85a756c](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/85a756c3bd893dae8f83f070ab86fd81dab366c0))
* support  non semver ([44a5d91](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/44a5d910294ff3d4594a0fa7bd10d4e3f8dc5dc6))
* support `gitDependencies` in `package.json` ([8961111](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/89611118fa28e3bbcb1785f04c4604cbbbd9936a))
* support path query parameter (sub-directory) even Unity 2019.2 or earlier ([fa365cc](https://github.com/mob-sakai/GitDependencyResolverForUnity/commit/fa365cc61a95cf82dfe093daf99526dd6503fefe)), closes [example/folder#v1](https://github.com/example/folder/issues/v1)


### BREAKING CHANGES

* Must use `gitDependencies` instead of `dependencies` to define git-based dependencies for the package.
This plugin also supports `dependencies` to resolve git-based dependencies, but if `dependencies` include packages that UPM can't resolve, it will fail to start Unity in CI environment.

# Changelog

## [1.1.3](https://github.com/mob-sakai/GitDependencyResolverForUnity/tree/1.1.3) (2019-10-06)

[Full Changelog](https://github.com/mob-sakai/GitDependencyResolverForUnity/compare/1.1.2...1.1.3)

- Some files in GitDependencyResolverForUnity conflicts with UpmGitExtension [\#16](https://github.com/mob-sakai/GitDependencyResolverForUnity/issues/16)

## [1.1.2](https://github.com/mob-sakai/GitDependencyResolverForUnity/tree/1.1.2) (2019-08-05)

[Full Changelog](https://github.com/mob-sakai/GitDependencyResolverForUnity/compare/1.1.1...1.1.2)

## [1.1.1](https://github.com/mob-sakai/GitDependencyResolverForUnity/tree/1.1.1) (2019-08-05)

[Full Changelog](https://github.com/mob-sakai/GitDependencyResolverForUnity/compare/1.1.0...1.1.1)

**Fixed bugs:**

- guids for git-dependency-resolver conflict with upm-git-extension [\#15](https://github.com/mob-sakai/GitDependencyResolverForUnity/issues/15)

## [1.1.0](https://github.com/mob-sakai/GitDependencyResolverForUnity/tree/1.1.0) (2019-06-11)

[Full Changelog](https://github.com/mob-sakai/GitDependencyResolverForUnity/compare/1.0.0...1.1.0)

**Implemented enhancements:**

- Add notes on using this package [\#13](https://github.com/mob-sakai/GitDependencyResolverForUnity/issues/13)
- Refer to no files from the Library folder [\#12](https://github.com/mob-sakai/GitDependencyResolverForUnity/issues/12)
- Deterministic package installation [\#10](https://github.com/mob-sakai/GitDependencyResolverForUnity/issues/10)

## [1.0.0](https://github.com/mob-sakai/GitDependencyResolverForUnity/tree/1.0.0) (2019-05-16)

[Full Changelog](https://github.com/mob-sakai/GitDependencyResolverForUnity/compare/96d11551ce2e670f5c991a254ac3dd4fb4b67c02...1.0.0)

This plugin resolves git url dependencies in the package for Unity Package Manager.

You can use a git url as a package dependency as the following!

```js
{
  "name": "com.coffee.package-a",
  "version": "0.1.0",
  "dependencies": {
    "com.coffee.core-a": "https://github.com/mob-sakai/GitPackageTest.git#core-a-0.1.0"
  }
}
```

**Implemented enhancements:**

- Update package with a specific tag/branch [\#7](https://github.com/mob-sakai/GitDependencyResolverForUnity/issues/7)
- Support .Net 3.5 & 4.x [\#6](https://github.com/mob-sakai/GitDependencyResolverForUnity/issues/6)
- Support Unity 2019.1+ [\#5](https://github.com/mob-sakai/GitDependencyResolverForUnity/issues/5)
- Support private repository [\#4](https://github.com/mob-sakai/GitDependencyResolverForUnity/issues/4)
- Support GitHub, Bitbucket, GitLab, etc. [\#3](https://github.com/mob-sakai/GitDependencyResolverForUnity/issues/3)
- Uninstall unused packages that is installed by this plugin [\#2](https://github.com/mob-sakai/GitDependencyResolverForUnity/issues/2)
- Resolve git url dependencies in packages [\#1](https://github.com/mob-sakai/GitDependencyResolverForUnity/issues/1)



\* *This Changelog was automatically generated by [github_changelog_generator](https://github.com/github-changelog-generator/github-changelog-generator)*

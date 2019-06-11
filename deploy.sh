#!/bin/sh

# 1. << Input release version >>
echo ">> Start Github Release:"
PACKAGE_NAME=`node -pe 'require("./package.json").name'`
echo ">> Package name: ${PACKAGE_NAME}"
CURRENT_VERSION=`grep -o -e "\"version\".*$" package.json | sed -e "s/\"version\": \"\(.*\)\".*$/\1/"`
UNITY_PACKAGE_SRC=`node -pe 'require("./package.json").src'`
echo ">> Package source: ${UNITY_PACKAGE_SRC}"
[ ! -d "$UNITY_PACKAGE_SRC" ] && echo -e "\n>> Error : $UNITY_PACKAGE_SRC is not exist." && exit 1

read -p "[? (1/7) Input release version (for current: ${CURRENT_VERSION}): " RELEASE_VERSION
[ -z "${RELEASE_VERSION}" ] && exit 1

read -p "[? Are the issues on this release closed all? (y/N):" yn
case "$yn" in [yY]*) ;; *) exit 1;; esac

read -p "[? Is package for UnityPackageManager? (y/N):" yn
case "$yn" in [yY]*) UNITY_PACKAGE_MANAGER=true;; *) ;; esac

echo ">> OK"



# 2. << Update version in package.json >>
echo "\n>> (2/7) Update version... package.json"
sed -i '' -e "s/\"version\": \(.*\)/\"version\": \"${RELEASE_VERSION}\",/g" package.json
echo ">> OK"



# 3. << Generate change log >>
CHANGELOG_GENERATOR_ARG=`grep -o -e ".*git\"$" package.json | sed -e "s/^.*\/\([^\/]*\)\/\([^\/]*\).git.*$/--user \1 --project \2/"`
CHANGELOG_GENERATOR_ARG="--future-release ${RELEASE_VERSION} ${CHANGELOG_GENERATOR_ARG}"
echo "\n>> (3/7) Generate change log... ${CHANGELOG_GENERATOR_ARG}"
github_changelog_generator ${CHANGELOG_GENERATOR_ARG}

git diff -- CHANGELOG.md
read -p "[? Is the change log correct? (y/N):" yn
case "$yn" in [yY]*) ;; *) exit 1;; esac
echo ">> OK"



# 4. << Commit release documents >>
echo "\n>> (4/7) Commit release documents..."
cp -f package.json CHANGELOG.md README.md "$UNITY_PACKAGE_SRC"
git add -u
git commit -m "update documents for $RELEASE_VERSION"
echo ">> OK"



#  5. << Split to upm >>
if [ "$UNITY_PACKAGE_MANAGER" == "true" ]; then
  echo "\n>> (5/7) Split to upm..."
  git fetch
  git show-ref --quiet refs/remotes/origin/upm && git branch -f upm origin/upm
  ./git-subtree-split-squash.sh --prefix="$UNITY_PACKAGE_SRC" --message="$RELEASE_VERSION" --branch=upm
  git push origin upm
fi



# 6. << Push to remote >>
echo "\n>> (6/7) Push to remote..."
git push origin
echo ">> OK"



# 7. << Release on Github >>
echo "\n>> (7/7) Release on Github..."
GH_RELEASE_ARG="--name $RELEASE_VERSION --tag_name $RELEASE_VERSION"
[ "$UNITY_PACKAGE_MANAGER" == "true" ] && GH_RELEASE_ARG="$GH_RELEASE_ARG --target_commitish upm"
gh-release $GH_RELEASE_ARG
[ "$?" == "0" ] && echo "\n\n>> $PACKAGE_NAME $RELEASE_VERSION has been successfully released!\n" && exit 1
echo ">> OK"

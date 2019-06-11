#!/bin/sh

if test $# -eq 0
then
	set -- -h
fi

OPTS_SPEC="\
git-subtree-split-squash   --prefix=<prefix> --message=<message> --branch=<branch>
git-subtree-split-squash   --p <prefix> --m <message> --b <branch>
--
h,help        show the help
p,prefix=     the name of the subdir to split out
m,message=    commit message
b,branch=     branch for split to
"
eval "$(echo "$OPTS_SPEC" | git rev-parse --parseopt -- "$@" || echo exit $?)"

# exit
while test $# -gt 0
do
	opt="$1"
  shift
	case "$opt" in
	-p)
		prefix="$1" && shift;;
	-m)
		message="$1" && shift;;
	-b)
		branch="$1" && shift;;
	-h)
    echo ${OPTS_SPEC} && exit 0;;
	--)
		break;;
	*)
		echo "Unexpected option: $opt" && exit 1;;
	esac
done

[ -z "$prefix" ] && echo "You must provide the --prefix option." && exit 1
[ -z "$branch" ] && echo "You must provide the --branch option." && exit 1


# 1. << Stash before task >>
current_branch=`git rev-parse --abbrev-ref HEAD`
changed=`git status --porcelain`
[ -n "${changed}" ] && git add -A && git stash


# 2. << Take a snapshot of the files >>
tmp=$(mktemp -d)
mv -f ${prefix}/* ${tmp}


# 3. << Switch branch and clean >>
git show-ref --quiet refs/heads/${branch} && git checkout -f ${branch} || git checkout --orphan ${branch}
git reset --hard
git clean -fd
git rm -rf --ignore-unmatch *


# 4. << restore the snapshot >>
mv -f ${tmp}/* .


# 5. << commit changes >>
git add -A
[ -n "${message}" ] && git commit -m "${message}" || git commit


# 6. << Stash pop after task >>
git checkout -f "$current_branch"
[ -n "${changed}" ] && git stash pop && git reset HEAD .

#!/bin/bash

# UNITY_APP=/Applications/Unity/Hub/Editor/`cat ProjectSettings/ProjectVersion.txt | cut -d ' '  -f 2`/Unity.app/Contents/MacOS/Unity
# UNITY_APP=/Applications/Unity/Hub/Editor/2018.4.22f1/Unity.app/Contents/MacOS/Unity
# UNITY_APP=/Applications/Unity/Hub/Editor/2019.4.4f1/Unity.app/Contents/MacOS/Unity
UNITY_APP=$1

git --no-pager diff -U1 -- ./Packages/com.coffee.git-dependency-resolver

cd `dirname $0`
testDir=`mktemp -d`
cp -rf  Assets $testDir/Assets
cp -rf  ProjectSettings $testDir/ProjectSettings
cp -rf  Packages $testDir/Packages
cd $testDir

run_test () {
  rm -rf success_*
  echo $1
  $UNITY_APP -quit -batchmode -logFile $1 -projectPath . -executeMethod Coffee.Ugd.Runtime.Execute

  [ -e success_compile ] && echo "  success compile" || echo "  ! failed compile !"
  [ -e success_execute ] && echo "  success execute" || echo "  ! failed execute !"
  echo "  log > ${testDir}/$1"
  echo ""
} 

echo "testDir > ${testDir}"
echo ""

# 1. run init
run_test 1_run_init

# 1-2. run again
run_test 1-2_run_again

# 3. run after remove auto-installed packages
rm -rf ./Packages/.com*
run_test 3_run_after_remove_auto_installed_packages

# 3-2. run again
run_test 3-2_run_again

# # 4. run after remove Library/ScriptAssemblies packages
# rm -rf success_* Library/ScriptAssemblies
# run_test 4_run_after_remove_library_assemblies

# # 4-2. run again
# run_test 4-2_run_again

# # 2. run after remove library
# rm -rf ./Library
# run_test 2_run_after_remove_library

# # 2-2. run again
# run_test 2-2_run_again
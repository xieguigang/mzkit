@echo off

REM git remote add local http://git.biodeep.cn/xieguigang/mzkit.git

git pull gitlink HEAD
git pull gitee HEAD
git pull local HEAD

git push gitlink HEAD
git push gitee HEAD
git push local HEAD

echo synchronization of this code repository job done!
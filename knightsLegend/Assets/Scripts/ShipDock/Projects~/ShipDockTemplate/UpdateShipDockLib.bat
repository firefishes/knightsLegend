@echo off

::类库dll
set lib_name=ShipDockFSM
set unity_ver=2018_4

set dll=%lib_name%.dll
set pdb=%lib_name%.pdb
set to_path=..\..\ShipDockPlugins\Framework_

echo Update %lib_name% dll...
::移动文件
set move_from=bin\Debug\%dll%
xcopy %move_from% %to_path%%unity_ver%\

set move_from=bin\Debug\%pdb%
xcopy %move_from% %to_path%%unity_ver%\

echo Finished..
pause
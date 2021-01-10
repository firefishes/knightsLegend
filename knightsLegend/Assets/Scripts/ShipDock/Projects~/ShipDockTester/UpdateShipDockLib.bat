@echo off

::类库dll
set lib_name=ShipDockTester
set unity_ver=2018_4

set dll=%lib_name%.dll
set pdb=%lib_name%.pdb
set to_path=..\..\ShipDockPlugins\Framework_

echo Update %lib_name% dll...
::复制文件
set move_from=bin\Debug\%dll%
copy %move_from% %to_path%%unity_ver%\ >nul

set move_from=bin\Debug\%pdb%
copy %move_from% %to_path%%unity_ver%\ >nul

echo Finished..
pause
@echo off

::保存当前目录路径
set root=%~dp0

::定义即将移动到的Unity工程dll
set move_from=..\..\..\Temp\bin\Debug\Assembly-CSharp.dll

echo Update main project dll...
::移动热更文件
xcopy %move_from% %root%\bin\Debug

echo Finished..
pause
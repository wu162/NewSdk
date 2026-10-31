@echo off
java -Djava.awt.headless=true -cp "%~dp0fla2swf.jar;%~dp0lib\*" newsdk.fla.Main %*
exit /b %errorlevel%

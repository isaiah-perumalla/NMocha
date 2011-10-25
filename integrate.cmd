@echo off
if exist "%PROGRAMFILES%\Microsoft Visual Studio 9.0\VC\vcvarsall.bat" call "%PROGRAMFILES%\Microsoft Visual Studio 9.0\VC\vcvarsall.bat"
tools\nant-0.85\bin\nant integrate %*
pause

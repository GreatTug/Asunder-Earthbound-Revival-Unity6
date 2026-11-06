@echo off
REM Asunder: Earthbound — Quick Batch Builder
REM Usage: build.bat [All|PC|VR|Audit]

set TARGET=%1
if "%TARGET%"=="" set TARGET=All

powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0build.ps1" -Target %TARGET%
exit /b %ERRORLEVEL%

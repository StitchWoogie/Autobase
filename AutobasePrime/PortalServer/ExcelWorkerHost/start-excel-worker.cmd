@echo off
setlocal

set "WORKER_EXE=%~dp0bin\Debug\ExcelWorkerHost.exe"
set "WORK_DIR=D:\EXE\AUTOBASE.Prime\WebServer\AutoWeb\Project"
set "BASE_URL=http://localhost"
set "RUNTIME_DIR=D:\EXE\AUTOBASE.Prime\WebServer\AutoWeb\Runtime"
set "WORKER_NAME=%COMPUTERNAME%"
set "POLL_SECONDS=5"

if not exist "%WORKER_EXE%" (
    echo ExcelWorkerHost.exe 파일을 찾을 수 없습니다.
    echo 경로: %WORKER_EXE%
    exit /b 1
)

echo Excel Worker 시작
echo   EXE     : %WORKER_EXE%
echo   WORKDIR : %WORK_DIR%
echo   BASEURL : %BASE_URL%
echo   RUNTIME : %RUNTIME_DIR%
echo   WORKER  : %WORKER_NAME%
echo   POLL    : %POLL_SECONDS%

"%WORKER_EXE%" --workdir "%WORK_DIR%" --baseurl "%BASE_URL%" --runtimedir "%RUNTIME_DIR%" --worker "%WORKER_NAME%" --poll %POLL_SECONDS%

endlocal

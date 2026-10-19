@echo off
setlocal
set "ENDPOINT_DOTNET=dotnet"
if exist "%USERPROFILE%\.dotnet\dotnet.exe" (
    "%USERPROFILE%\.dotnet\dotnet.exe" --list-sdks | findstr /R /B "9\." >nul
    if not errorlevel 1 (
        set "ENDPOINT_DOTNET=%USERPROFILE%\.dotnet\dotnet.exe"
        set "DOTNET_ROOT=%USERPROFILE%\.dotnet"
    )
)
"%ENDPOINT_DOTNET%" run --project "%~dp0EndpointResolver\EndpointResolver.csproj" %*
exit /b %errorlevel%

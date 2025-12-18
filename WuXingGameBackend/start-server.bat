@echo off
echo Starting WuXing Game Backend Server...
echo Make sure MongoDB is running on port 27018 before starting the server.
echo If MongoDB is not running, the server will still start but MongoDB features will be disabled.
echo.
echo Press any key to start the server...
pause >nul

cd /d "%~dp0"
dotnet run

echo.
echo Server stopped.
pause
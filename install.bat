@echo off
echo ========================================
echo INSTALACIJA TROŠKOVI RADA APLIKACIJE
echo ========================================
echo.

REM Provjera .NET 6.0
echo Provjera .NET 6.0...
dotnet --list-sdks | findstr "6.0" >nul
if %errorlevel% neq 0 (
    echo .NET 6.0 nije instaliran!
    echo Preuzmite sa: https://dotnet.microsoft.com/download/dotnet/6.0
    pause
    exit /b 1
)
echo ✓ .NET 6.0 je instaliran

REM Provjera PostgreSQL
echo.
echo Provjera PostgreSQL...
where psql >nul 2>nul
if %errorlevel% neq 0 (
    echo PostgreSQL nije instaliran!
    echo Preuzmite sa: https://www.postgresql.org/download/
    echo.
    echo Nakon instalacije, pokrenite ove naredbe u psql:
    echo CREATE DATABASE troskovirada;
    echo CREATE USER troskovirada_user WITH PASSWORD 'TroskoviRada123!';
    echo GRANT ALL PRIVILEGES ON DATABASE troskovirada TO troskovirada_user;
    pause
    exit /b 1
)
echo ✓ PostgreSQL je instaliran

REM Kreiranje baze podataka
echo.
echo Kreiranje baze podataka...
echo Unesite PostgreSQL admin lozinku:
set /p PG_PASSWORD=
psql -U postgres -c "CREATE DATABASE troskovirada;" 2>nul
psql -U postgres -c "CREATE USER troskovirada_user WITH PASSWORD 'TroskoviRada123!';" 2>nul
psql -U postgres -c "GRANT ALL PRIVILEGES ON DATABASE troskovirada TO troskovirada_user;" 2>nul
echo ✓ Baza podataka kreirana

REM Konfiguracija appsettings.json
echo.
echo Konfiguriranje aplikacije...
cd ..\App
copy appsettings.json appsettings.json.backup 2>nul

REM Kreiranje novog appsettings.json
(
echo {
echo   "Logging": {
echo     "LogLevel": {
echo       "Default": "Information",
echo       "Microsoft.AspNetCore": "Warning"
echo     }
echo   },
echo   "AllowedHosts": "*",
echo   "ConnectionStrings": {
echo     "DefaultConnection": "Host=localhost;Database=troskovirada;Username=troskovirada_user;Password=TroskoviRada123!"
echo   }
echo }
) > appsettings.json

echo ✓ Konfiguracija postavljena

REM Pokretanje migracija
echo.
echo Pokretanje migracija baze podataka...
dotnet ef database update
if %errorlevel% neq 0 (
    echo Pokrećem migracije alternativnim načinom...
    dotnet tool install --global dotnet-ef
    dotnet ef database update
)
echo ✓ Migracije završene

REM Pokretanje aplikacije
echo.
echo Pokretanje aplikacije...
echo Aplikacija će se pokrenuti na: http://localhost:5000 i https://localhost:5001
echo.
echo Pritisnite Ctrl+C za zaustavljanje aplikacije
echo.
dotnet run

pause
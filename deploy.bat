@echo off
echo ========================================
echo DEPLOY TROŠKOVI RADA APLIKACIJE
echo ========================================
echo.
echo Odaberite opciju:
echo 1 - Kompletna instalacija (baza + aplikacija)
echo 2 - Samo pokretanje aplikacije
echo 3 - Samo kreiranje baze podataka
echo 4 - Povezivanje na GitHub repository
echo.
set /p choice="Unesite broj opcije: "

if "%choice%"=="1" (
    echo.
    echo Pokrećem kompletnu instalaciju...
    call Scripts\install.bat
) else if "%choice%"=="2" (
    echo.
    echo Pokrećem aplikaciju...
    cd App
    echo Otvaram browser...
    start http://localhost:5232
    timeout /t 2
    dotnet run
) else if "%choice%"=="3" (
    echo.
    echo Kreiranje baze podataka...
    echo Pokrenite create_database.sql u psql alatu:
    echo psql -U postgres -f Scripts\create_database.sql
    pause
) else if "%choice%"=="4" (
    echo.
    echo Povezivanje na GitHub repository...
    echo Git Repository: https://github.com/[VAŠ_USERNAME]/TroskoviRada
    echo.
    echo Za kloniranje pokrenite:
    echo git clone https://github.com/[VAŠ_USERNAME]/TroskoviRada.git
    pause
) else (
    echo Nepoznata opcija!
    pause
)
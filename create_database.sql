-- SQL skripta za kreiranje baze podataka za Troškove Rada aplikaciju

-- Kreiranje baze ako ne postoji
SELECT 'CREATE DATABASE troskovirada'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'troskovirada')\gexec

-- Povezivanje na bazu
\c troskovirada;

-- Kreiranje korisnika ako ne postoji
DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'troskovirada_user') THEN
        CREATE USER troskovirada_user WITH PASSWORD 'TroskoviRada123!';
    END IF;
END
$$;

-- Dodjeljivanje privilegija
GRANT ALL PRIVILEGES ON DATABASE troskovirada TO troskovirada_user;
GRANT CREATE ON SCHEMA public TO troskovirada_user;

-- Informacije za korisnika
\echo '======================================='
\echo 'BAZA PODATAKA USPJEŠNO KREIRANA'
\echo '======================================='
\echo 'Baza: troskovirada'
\echo 'Korisnik: troskovirada_user'
\echo 'Lozinka: TroskoviRada123!'
\echo '======================================='
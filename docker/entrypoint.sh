#!/bin/sh
set -e

echo "Instalando certificado da CA..."

cp /certificates/root-ca.crt \
   /usr/local/share/ca-certificates/plataforma-root-ca.crt

update-ca-certificates

echo "Certificado instalado."
echo "Iniciando ${APP_DLL}..."

exec su -s /bin/sh app -c \
  "exec dotnet ${APP_DLL}"
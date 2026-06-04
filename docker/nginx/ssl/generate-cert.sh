#!/bin/bash
# Generate self-signed SSL certificate for development
mkdir -p "$(dirname "$0")"
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
    -keyout "$(dirname "$0")/key.pem" \
    -out "$(dirname "$0")/cert.pem" \
    -subj "/C=BR/ST=SP/L=Sao Paulo/O=Folha360/CN=localhost"
echo "SSL certificate generated for localhost"

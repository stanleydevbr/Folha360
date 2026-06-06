#!/bin/bash
# =============================================================================
# PostgreSQL Init Script
# Executado na primeira inicialização do container
# =============================================================================
set -e

echo "=== Folha360: Inicializando PostgreSQL ==="

# Ativar extensão pgvector
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE EXTENSION IF NOT EXISTS vector;
    CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
    CREATE EXTENSION IF NOT EXISTS pg_trgm;
EOSQL

echo "pgvector extension activated."

# Criar schema 'documentos' para armazenamento de documentos/arquivos
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE SCHEMA IF NOT EXISTS documentos;
    GRANT ALL ON SCHEMA documentos TO $POSTGRES_USER;
EOSQL

echo "Schema 'documentos' created."

echo "=== Folha360: PostgreSQL initialization complete ==="

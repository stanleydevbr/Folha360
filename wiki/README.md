# Como Publicar esta Wiki no GitHub

## Estrutura

```
Folha360/
├── wiki/                          ← Arquivos específicos da Wiki
│   ├── Home.md                    ← Página inicial
│   ├── _Sidebar.md                ← Barra lateral de navegação
│   ├── _Footer.md                 ← Rodapé
│   └── README.md                  ← Este guia
├── docs/outputs/arquitetura/      ← Artefatos de arquitetura
│   ├── layered-architecture.md
│   ├── component-boundaries.md
│   ├── integration-boundaries.md
│   ├── deployment-view.md
│   ├── runtime-view.md
│   ├── quality-attribute-scenarios.md
│   ├── architecture-risk-register.md
│   ├── tradeoff-matrix.md
│   ├── architecture-options.md
│   ├── adr-001-monolito-modular.md
│   ├── adr-002-rabbitmq-message-broker.md
│   ├── adr-003-schema-por-tenant.md
│   ├── adr-004-processamento-assincrono-folha.md
│   └── adr-005-redis-cache-tabelas.md
└── mkdocs.yml                     ← Configuração MkDocs
```

## Opção 1: Publicar via GitHub Web

1. Acesse: `https://github.com/<seu-usuario>/Folha360/wiki`
2. Clique em **"Create the first page"**
3. Cole o conteúdo de `wiki/Home.md` como página **Home**
4. Clique em **"Save"**
5. Para cada artefato, clique em **"New Page"**:
   - Título = nome do arquivo sem `.md` (ex.: `layered-architecture`)
   - Conteúdo = copiar do arquivo correspondente em `docs/outputs/arquitetura/`
6. Vá em **"Settings"** → selecione `_Sidebar.md` e cole o conteúdo
7. Vá em **"Settings"** → selecione `_Footer.md` e cole o conteúdo

## Opção 2: Publicar via Git (recomendado)

```bash
# 1. Clonar a wiki como repositório Git separado
git clone https://github.com/<seu-usuario>/Folha360.wiki.git
cd Folha360.wiki

# 2. Copiar todos os artefatos como páginas
cp ../Folha360/docs/outputs/arquitetura/*.md .

# 3. Copiar arquivos especiais da wiki
cp ../Folha360/wiki/Home.md .
cp ../Folha360/wiki/_Sidebar.md .
cp ../Folha360/wiki/_Footer.md .

# 4. Commit e push
git add .
git commit -m "docs: publish architecture wiki"
git push origin master
```

## Opção 3: Servir localmente com MkDocs (recomendado para dev)

```bash
# Instalar MkDocs + Material theme
pip install mkdocs mkdocs-material

# Gerar mkdocs.yml na raiz do projeto
cd Folha360
```

Crie `mkdocs.yml`:

```yaml
site_name: Folha360 - Arquitetura
site_description: Documentação de Arquitetura do Sistema Folha360
theme:
  name: material
  features:
    - navigation.sections
    - navigation.expand
    - toc.integrate
nav:
  - Home: docs/outputs/arquitetura/wiki/Home.md
  - Visões Arquiteturais:
      - Visão em Camadas: docs/outputs/arquitetura/layered-architecture.md
      - Fronteiras de Componentes: docs/outputs/arquitetura/component-boundaries.md
      - Fronteiras de Integração: docs/outputs/arquitetura/integration-boundaries.md
      - Visão de Deployment: docs/outputs/arquitetura/deployment-view.md
      - Visão de Runtime: docs/outputs/arquitetura/runtime-view.md
  - ADRs:
      - ADR-001 Monólito Modular: docs/outputs/arquitetura/adr-001-monolito-modular.md
      - ADR-002 RabbitMQ: docs/outputs/arquitetura/adr-002-rabbitmq-message-broker.md
      - ADR-003 Schema por Tenant: docs/outputs/arquitetura/adr-003-schema-por-tenant.md
      - ADR-004 Processamento Assíncrono: docs/outputs/arquitetura/adr-004-processamento-assincrono-folha.md
      - ADR-005 Redis Cache: docs/outputs/arquitetura/adr-005-redis-cache-tabelas.md
  - Análises:
      - Cenários de Qualidade: docs/outputs/arquitetura/quality-attribute-scenarios.md
      - Registro de Riscos: docs/outputs/arquitetura/architecture-risk-register.md
      - Matriz de Tradeoffs: docs/outputs/arquitetura/tradeoff-matrix.md
      - Opções Arquiteturais: docs/outputs/arquitetura/architecture-options.md
```

```bash
# Servir localmente
mkdocs serve
# Acessar http://localhost:8000
```

## Mapeamento de Páginas

| Página na Wiki | Arquivo Fonte |
|---|---|
| `Home` | `wiki/Home.md` |
| `layered-architecture` | `layered-architecture.md` |
| `component-boundaries` | `component-boundaries.md` |
| `integration-boundaries` | `integration-boundaries.md` |
| `deployment-view` | `deployment-view.md` |
| `runtime-view` | `runtime-view.md` |
| `quality-attribute-scenarios` | `quality-attribute-scenarios.md` |
| `architecture-risk-register` | `architecture-risk-register.md` |
| `tradeoff-matrix` | `tradeoff-matrix.md` |
| `architecture-options` | `architecture-options.md` |
| `adr-001-monolito-modular` | `adr-001-monolito-modular.md` |
| `adr-002-rabbitmq-message-broker` | `adr-002-rabbitmq-message-broker.md` |
| `adr-003-schema-por-tenant` | `adr-003-schema-por-tenant.md` |
| `adr-004-processamento-assincrono-folha` | `adr-004-processamento-assincrono-folha.md` |
| `adr-005-redis-cache-tabelas` | `adr-005-redis-cache-tabelas.md` |

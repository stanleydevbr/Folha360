## Contexto
Criar a infraestrutura de um aplicativo frontend de gestão de folha de pagamento, permitindo que usuários visualizem dados de pagamento, acessem relatórios e realizem operações de cadastro com consistência e segurança.

## Objetivo
Definir a base técnica de um frontend responsivo, escalável e modular para o sistema de folha de pagamento.
O frontend deve contemplar os seguintes elementos:
- Tela de login segura e intuitiva
- Tela principal com navegação estruturada
    - Header com identificação do usuário, título da página e ações rápidas
    - Sidebar de menu com acesso às principais seções do sistema
    - Footer com informações institucionais e links de apoio
    - Área de conteúdo principal para dashboards, listas e formulários

## Requisitos de infraestrutura
- Estrutura de projeto configurada para React + TypeScript, com modularização de páginas e componentes
- Suporte a roteamento de páginas e navegação interna
- Configuração de tema global, tokens de estilo e layout padrão
- Suporte a tema claro e escuro com alternância de modo e persistência da preferência do usuário
- Infra de autenticação básica no frontend (formulário, validação, persistência de sessão)
- Integração com biblioteca de gerenciamento de estado ou query client para dados remotos
- Controle de responsividade e acessibilidade desde o começo

## Estrutura de pastas

A organização do projeto segue **monorepo Turborepo + pnpm workspaces**, com separação clara entre aplicações e pacotes compartilhados, aplicando princípios **SOLID**, **Clean Architecture** e **feature-first design**.

```
frontend/
├── apps/
│   ├── admin/                          # Dashboard administrativo
│   │   ├── src/
│   │   │   ├── routes/                 # Páginas organizadas por domínio (feature-first)
│   │   │   │   ├── cadastros/          #   Módulo F02 — empresas, funcionários, cargos
│   │   │   │   │   ├── EmpresasPage.tsx
│   │   │   │   │   ├── FuncionariosPage.tsx
│   │   │   │   │   └── components/     #   Sub-componentes específicos do módulo
│   │   │   │   │       ├── EmpresaForm.tsx
│   │   │   │   │       └── FuncionarioTable.tsx
│   │   │   │   ├── eventos/            #   Módulo F03 — admissões, desligamentos, afastamentos
│   │   │   │   ├── processamento/      #   Módulo F04 — processamento de folha
│   │   │   │   ├── fiscais/            #   Módulo F05 — obrigações fiscais
│   │   │   │   ├── relatorios/         #   Módulo F06 — relatórios e exportações
│   │   │   │   ├── esocial/            #   Módulo F07 — integração eSocial
│   │   │   │   ├── dashboard/          #   Visão geral com indicadores
│   │   │   │   └── auth/              #   Login, recuperação de senha
│   │   │   ├── layouts/               # Layouts reutilizáveis (default, auth, blank)
│   │   │   │   ├── AdminLayout.tsx     #   Header + Sidebar + Footer + conteúdo
│   │   │   │   └── AuthLayout.tsx      #   Layout da tela de login
│   │   │   ├── providers/             # Providers de contexto (tema, auth, etc.)
│   │   │   │   ├── ThemeProvider.tsx
│   │   │   │   └── AuthProvider.tsx
│   │   │   ├── hooks/                 # Hooks de UI local (não de dados)
│   │   │   ├── lib/                   # Configurações de cliente (axios, router)
│   │   │   ├── App.tsx
│   │   │   └── main.tsx
│   │   ├── index.html
│   │   ├── vite.config.ts
│   │   ├── tsconfig.json
│   │   └── package.json
│   └── portal/                         # Portal do funcionário (fase futura)
├── packages/
│   ├── ui/                             # Design System — componentes visuais puros
│   │   ├── src/
│   │   │   ├── components/            # Componentes atômicos e moleculares
│   │   │   │   ├── DataTable/         #   Tabela padronizada (ordenação, paginação, filtros)
│   │   │   │   ├── Form/             #   Formulários, campos, validação
│   │   │   │   ├── Button/
│   │   │   │   ├── Modal/
│   │   │   │   ├── Card/
│   │   │   │   ├── Sidebar/
│   │   │   │   └── …                 #   Demais componentes compartilhados
│   │   │   ├── tokens/               # Tokens de design (cores, tipografia, spacing)
│   │   │   │   ├── colors.ts         #   Paletas claro e escuro
│   │   │   │   └── typography.ts
│   │   │   ├── hooks/                # Hooks internos de UI (useBreakpoint, useTheme)
│   │   │   └── index.ts
│   │   ├── tsconfig.json
│   │   └── package.json
│   ├── api/                            # Camada de dados — TanStack Query + tipos DTO
│   │   ├── src/
│   │   │   ├── clients/              # Cliente HTTP configurado (axios/fetch)
│   │   │   │   └── apiClient.ts
│   │   │   ├── modules/              # Hooks e DTOs por domínio
│   │   │   │   ├── cadastros/        #   useEmpresas, useFuncionarios, EmpresaDto, …
│   │   │   │   ├── eventos/
│   │   │   │   ├── processamento/
│   │   │   │   ├── fiscais/
│   │   │   │   ├── relatorios/
│   │   │   │   ├── esocial/
│   │   │   │   └── auth/            #   useLogin, useSession
│   │   │   ├── hooks/               # Hooks genéricos (usePaginatedQuery, useInfiniteScroll)
│   │   │   └── index.ts
│   │   ├── tsconfig.json
│   │   └── package.json
│   └── utils/                          # Funções puras — sem dependência React
│       ├── src/
│       │   ├── formatters/           # formatCnpj, formatCpf, formatCurrency, …
│       │   ├── validators/           # validateCnpj, validateCpf, validateEmail, …
│       │   ├── masks/               # maskCnpj, maskCpf, maskPhone, …
│       │   └── index.ts
│       ├── tsconfig.json
│       └── package.json
├── pnpm-workspace.yaml
├── turbo.json
└── tsconfig.json
```

### Princípios aplicados na estrutura

| Princípio | Como é aplicado |
|-----------|----------------|
| **Single Responsibility** | Cada pacote tem uma única responsabilidade: `ui` só renderiza, `api` só acessa dados, `utils` só contém funções puras |
| **Dependency Inversion (DIP)** | `apps/*` depende de abstrações dos pacotes (`@folha360/api`, `@folha360/ui`); o fluxo de dados é unidirecional |
| **Interface Segregation (ISP)** | Componentes do `packages/ui` expõem props enxutas e específicas; hooks do `packages/api` retornam apenas o necessário |
| **Open/Closed (OCP)** | `DataTable` e `Form` são extensíveis via props de customização sem exigir modificação interna |
| **Liskov Substitution (LSP)** | Componentes genéricos (`DataTable<T>`) aceitam qualquer tipo de dado que implemente a interface esperada |
| **Separation of Concerns** | UI, dados, lógica de negócio e utilidades vivem em camadas independentes e com regras rígidas de dependência |
| **Composition over Inheritance** | Páginas compõem componentes menores via props; não há herança de componentes |
| **Feature-First** | Dentro de `apps/admin`, o código é agrupado por domínio de negócio (cadastros, eventos, etc.) e não por tipo técnico |

### Regras de colocalização

| Tamanho do componente | Onde colocar |
|-----------------------|-------------|
| < 80 linhas, usado só em uma página | No mesmo arquivo da página |
| 80–150 linhas, usado só em um módulo | `routes/<modulo>/components/Nome.tsx` |
| > 150 linhas ou usado em 2+ módulos | Extrair para `packages/ui` |

## Requisitos de componentes compartilhados
O frontend deve prover uma biblioteca de componentes reutilizáveis que suportem:
- Tabelas padronizadas
    - Ordenação por colunas
    - Paginação configurável
    - Filtros de texto e selects
    - Ajuste de quantidade de itens por página
    - Navegação de páginas e status de total de registros
    - Ações por linha: edição, exclusão, seleção em massa
    - Renderização customizável de células e colunas
- Formulários padronizados
    - Validação de campos e layout consistente
    - Suporte a criação, edição e visualização de registros
    - Comportamento de submissão e tratamento de erros
    - Ações de cancelamento, reset e impressão/exportação quando aplicável
    - Personalização de campos, grupos e botões sem duplicação de lógica
- Controles de interface adicionais
    - Botões com variantes de estilo e estados desabilitado/carregando
    - Cards e painéis para dashboards e resumos
    - Modais e alertas para confirmação de ações
    - Badges, tags e indicadores de status

## Features
- Dashboard com visualização de informações de pagamento e indicadores principais
- Navegação clara e consistente entre as seções do sistema
- Layout responsivo, preparado para expansão de funcionalidades futuras
- Biblioteca de componentes compartilhados para facilitar manutenção e evolução
- Base de projeto adequada para integração com APIs e evolução incremental
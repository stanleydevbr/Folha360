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
- **Sistema de presets de cores** inspirado no Berry: mínimo de 5 variações de tema primário (azul, azul marinho, verde, roxo, cinza) com troca em runtime
- **Painel customizador de tema** (tipo slide-out drawer): alternar layout (vertical/horizontal), cores, fontes, modo escuro, sidebar colapsada
- **3 opções de font family**: Roboto (padrão), Inter, Poppins — configuráveis via token CSS e alternáveis no customizador
- **Suporte a RTL** (Right-to-Left) preparado para futura internacionalização
- Infra de autenticação básica no frontend (formulário, validação, persistência de sessão)
- **Padrão de autenticação JWT**: interceptor HTTP que anexa token, tratamento automático de 401 com redirect para login, AuthGuard como `<ProtectedRoute>`
- Integração com biblioteca de gerenciamento de estado ou query client para dados remotos
- **Breadcrumb automático** gerado a partir da árvore de rotas/navegação (referência: `BreadcrumbComponent` do Berry)
- **Indicador de carregamento de navegação** (progress bar no topo) durante transições de rota
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
│   │   │   │   ├── AdminLayout.tsx     #   Header + Sidebar + Footer + conteúdo (ref: Berry AdminComponent)
│   │   │   │   ├── AuthLayout.tsx      #   Layout da tela de login (ref: Berry GuestComponent)
│   │   │   │   ├── Header.tsx          #   Barra superior: breadcrumb, notificações, perfil (ref: Berry NavBar)
│   │   │   │   ├── Sidebar.tsx         #   Menu lateral colapsável (ref: Berry NavigationComponent)
│   │   │   │   ├── Footer.tsx          #   Rodapé institucional
│   │   │   │   └── ThemeConfigPanel.tsx #  Painel customizador slide-out (ref: Berry ConfigurationComponent)
│   │   │   ├── providers/             # Providers de contexto (tema, auth, etc.)
│   │   │   │   ├── ThemeProvider.tsx    #   Tema: preset, dark/light, fonte, layout, RTL (ref: Berry ThemeService)
│   │   │   │   └── AuthProvider.tsx     #   Autenticação JWT: login, logout, token, user (ref: Berry AuthenticationService)
│   │   │   ├── hooks/                 # Hooks de UI local (não de dados)
│   │   │   │   ├── useBreadcrumbs.ts   #   Gera breadcrumb da rota ativa + NavItem[]
│   │   │   │   ├── useNavigationItems.ts #  Itens de menu tipados (ref: Berry navigation.ts)
│   │   │   │   └── useNavigationProgress.ts # Barra de progresso na transição de rotas
│   │   │   ├── lib/                   # Configurações de cliente (axios, router)
│   │   │   │   ├── apiClient.ts        #   Axios com interceptors (ref: Berry BasicAuthInterceptor + ErrorInterceptor)
│   │   │   │   └── router.ts           #   React Router config
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
│   │   │   │   ├── Breadcrumb/        #   Breadcrumb automático (ref: Berry BreadcrumbComponent)
│   │   │   │   ├── Navigation/        #   Menu hierárquico: NavGroup, NavCollapse, NavItem (ref: Berry nav-content/)
│   │   │   │   └── …                 #   Demais componentes compartilhados
│   │   │   ├── tokens/               # Tokens de design (cores, tipografia, spacing)
│   │   │   │   ├── colors.ts         #   Paletas claro e escuro + presets (ref: Berry color-variables.scss)
│   │   │   │   ├── typography.ts     #   Roboto, Inter, Poppins (ref: Berry font-family.scss)
│   │   │   │   └── layout.ts         #   Dimensões: header, sidebar, container (ref: Berry theme-variables.scss)
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
## Projeto de referencia

O template **Berry Angular ng-bootstrap** (`C:\projetos\templates\berry-angular-ng-bootstrap`) — versão 3.5.0 por CodedThemes — serve como referência arquitetural e visual. Embora seja Angular 18 + Bootstrap 5, seus padrões de layout, tema, autenticação e navegação devem ser reinterpretados na stack React do Folha360 (React 19 + TypeScript 5 + Vite 6 + Tailwind CSS 4 + shadcn/ui).

### Padrões extraídos do template Berry

| Padrão | Como o Berry implementa | Como adaptar para o Folha360 (React) |
|--------|------------------------|--------------------------------------|
| **Layout shell** | `AdminComponent` standalone com `<app-navigation>` (sidebar) + `<app-nav-bar>` (header) + `<router-outlet>` (conteúdo) + footer | `AdminLayout.tsx` com `<Sidebar />` + `<Header />` + `<Outlet />` + `<Footer />` |
| **Layout público** | `GuestComponent` standalone para login, landing, manutenção | `AuthLayout.tsx` com estrutura centrada e minimalista |
| **Sidebar colapsável** | `NavigationComponent` com toggle que alterna entre `260px` (expandido) e `80px` (colapsado, ícones apenas) | Sidebar com estado `collapsed` via contexto/hook, transições CSS |
| **Menu como dados** | `NavigationItem[]` tipado com `type: 'group' \| 'collapse' \| 'item'` em `navigation.ts` | Array tipado `NavItem[]` com `children` recursivo, renderizado por componente `NavContent` |
| **Header fixo** | `NavBarComponent` com altura `80px`, dividido em `nav-logo`, `nav-left` (mobile toggle), `nav-right` (notificações, perfil, logout) | `Header.tsx` com `h-20`, logo, botão mobile, dropdown de usuário |
| **Breadcrumb automático** | `BreadcrumbComponent` que assina `Router.events` e percorre a árvore de `NavigationItems` | Hook `useBreadcrumbs()` que analisa `useLocation()` + `NavItem[]` |
| **7 temas de cor predefinidos** | Presets 1-7 com `--bs-primary`, `--bs-secondary` e variantes via SCSS `@each` | Tokens CSS custom properties no `@layer base` do Tailwind; preset switcher via contexto |
| **Modo escuro** | Classe `body.berry-dark` sobrescreve todas as custom properties | Classe `dark` no `<html>` com Tailwind dark mode (`class` strategy) |
| **3 font families** | Roboto (default), Inter, Poppins — via classe no `<body>` | Configuração no `tailwind.config.ts` com `fontFamily` extendido |
| **RTL** | Classe `body.berry-rtl` + `style-rtl.scss` | Suporte via Tailwind RTL (logical properties) + direção no `<html dir="rtl">` |
| **Customizador de tema** | `ConfigurationComponent` — painel slide-out que altera layout, cor, fonte, modo escuro em runtime via `Renderer2` | `ThemeConfigPanel` — drawer/sheet com controles que disparam ações no `ThemeProvider` |
| **Spinner de navegação** | `SpinnerComponent` exibido em `NavigationStart`/`NavigationEnd` | `NavigationProgress` — barra de progresso no topo via `useNavigation()` do React Router |
| **JWT + HttpOnly** | `BasicAuthInterceptor` anexa `Authorization: Bearer <token>`; `ErrorInterceptor` faz logout em 401 | `apiClient` (axios) com interceptor que lê token do `AuthProvider` e redireciona em 401 |
| **AuthGuard** | `CanActivate` que verifica `currentUserValue` do `AuthenticationService` | Componente `<ProtectedRoute>` que verifica `useAuth()` e redireciona para `/login` |
| **i18n** | `@ngx-translate/core` com 4 idiomas (en, fr, ro, cn) em JSON | `react-i18next` ou `next-intl` com arquivos JSON por locale |
| **Notificações toast** | `ngx-toastr` + `SweetAlert2` para confirmações | `sonner` (toast) + shadcn/ui `AlertDialog` para confirmações |
| **Scrollbar customizada** | `ngx-scrollbar` | CSS `scrollbar-width: thin` + `scrollbar-color` ou `overlay-scrollbar` |
| **Footer** | `pc-footer` com copyright e links | `<Footer />` com informações institucionais e links de apoio |

### Dimensões de layout de referência (extraídas do Berry)

| Elemento | Berry (Angular) | Folha360 (React + Tailwind) |
|----------|-----------------|----------------------------|
| Header altura | `80px` ($header-height) | `h-20` (80px) |
| Sidebar largura expandida | `260px` ($sidebar-width) | `w-[260px]` |
| Sidebar largura colapsada | `80px` ($sidebar-collapsed-width) | `w-[80px]` |
| Sidebar expandida (compact mode) | `300px` | `w-[300px]` |
| Container border-radius | `8px` | `rounded-lg` |
| Container background | `$gray-100` | `bg-muted` / `bg-gray-100` |

### Estrutura visual do layout admin (referência Berry)

```
┌──────────────────────────────────────────────────────────┐
│ Sidebar (260px / 80px colapsado)  │ Header (80px fixo)   │
│ ┌──────────────────────┐          │ ┌───────────────────┐│
│ │ Logo + Toggle        │          │ │ Breadcrumb        ││
│ │                      │          │ │ Notificações      ││
│ │ NavGroup             │          │ │ User Menu/Logout   ││
│ │  ├─ NavCollapse      │          │ └───────────────────┘│
│ │  │   ├─ NavItem      │          │ Conteúdo (com scroll)│
│ │  │   └─ NavItem      │          │ ┌───────────────────┐│
│ │  └─ NavItem          │          │ │ <Outlet />        ││
│ │                      │          │ │                   ││
│ │ Version              │          │ └───────────────────┘│
│ └──────────────────────┘          │ Footer               │
│                                   │ ┌───────────────────┐│
│                                   │ │ Copyright │ Links ││
│                                   │ └───────────────────┘│
└──────────────────────────────────────────────────────────┘
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

## Sistema de Tema e Customização (referência: Berry)

Inspirado no painel `ConfigurationComponent` do Berry, o Folha360 deve prover um sistema de customização visual em runtime.

### Presets de cores (mínimo 5)

| Preset | Cor primária | Cor secundária | Referência Berry |
|--------|-------------|----------------|------------------|
| preset-1 (Blue) | `#2196f3` | `#673ab7` | preset-1 |
| preset-2 (Navy) | `#0a2342` | `#2ca58d` | preset-6 |
| preset-3 (Dark Teal) | `#16595a` | `#c77e23` | preset-4 |
| preset-4 (Blue Grey) | `#607d8b` | `#009688` | preset-2 |
| preset-5 (Dark Cyan) | `#173e43` | `#3fb0ac` | preset-5 |

Cada preset gera tokens CSS custom properties aplicados no `:root`:
- `--color-primary`, `--color-primary-light`, `--color-primary-dark`
- `--color-secondary`, `--color-secondary-light`, `--color-secondary-dark`
- `--color-sidebar-bg`, `--color-sidebar-text`, `--color-sidebar-active`
- `--color-header-bg`, `--color-header-text`

### Customizador de tema (ThemeConfigPanel)

Painel slide-out (sheet/drawer) acessível por botão na sidebar ou header, contendo:
- **Layout mode**: Vertical (sidebar esquerda) / Horizontal (navbar topo) — referência Berry `layout` config
- **Sidebar**: Expandida / Colapsada (ícones) / Captions visíveis ou ocultas
- **Color preset**: Seletor visual dos 5 presets
- **Dark/Light mode**: Toggle com ícone sol/lua
- **Font family**: Roboto / Inter / Poppins
- **Container**: Boxed (largura máxima) / Full-width
- **RTL**: Toggle de direção de texto

Estado persistido em `localStorage` e aplicado via `ThemeProvider` (React Context).

## Features
- Dashboard com visualização de informações de pagamento e indicadores principais
- Navegação clara e consistente entre as seções do sistema
- Layout responsivo, preparado para expansão de funcionalidades futuras
- **Três modos de layout** (referência Berry): vertical (sidebar esquerda), horizontal (navbar topo), compact (sidebar com ícones)
- **Sidebar com menu hierárquico**: grupos (com caption), colapsáveis (submenu animado) e itens simples — definidos como array tipado de `NavItem[]`
- Biblioteca de componentes compartilhados para facilitar manutenção e evolução
- **Customizador de tema visual** em runtime (cores, fontes, layout, modo escuro) com persistência
- **Breadcrumb automático** baseado na rota ativa e árvore de navegação
- **Indicador de progresso de navegação** (barra superior estilo NProgress)
- Base de projeto adequada para integração com APIs e evolução incremental
# Architecture Options — Folha360

## Contexto
Este documento registra opções arquiteturais que estão em aberto ou que podem ser reavaliadas no futuro. As decisões já tomadas estão documentadas na [Tradeoff Matrix](./tradeoff-matrix.md) e nos [ADRs](./).

---

## Opção 1: Estratégia de Autenticação e Autorização

### Context
- **Problem**: Sistema multi-tenant com diferentes perfis de acesso (admin, operador, contador, consulta). Dados sensíveis de RH exigem controle de acesso rigoroso.
- **Scope**: Todos os módulos (APIs + frontend)
- **Constraints**: .NET 10; React SPA; compatível com LGPD

### Option A: Identity Server (Duende IdentityServer / Keycloak)
- **Shape**: Servidor de identidade dedicado (OAuth2/OIDC). APIs validam JWT; frontend usa PKCE flow.
- **Strengths**: Padrão de mercado; SSO; suporte a múltiplos clients; refresh token; revogação
- **Weaknesses**: +1 infra para gerenciar; complexidade adicional; curva de aprendizado
- **Best fit when**: Múltiplos sistemas compartilham autenticação; necessidade de SSO

### Option B: ASP.NET Core Identity + JWT (escolhido para início)
- **Shape**: Autenticação integrada no monólito modular. Identity Server no próprio app. JWT emitido e validado localmente.
- **Strengths**: Simples de implementar; sem infra extra; integração nativa com EF Core
- **Weaknesses**: Sem SSO nativo; acoplamento com o monólito; migração futura mais difícil
- **Best fit when**: Sistema único; time pequeno; sem necessidade de SSO

### Option C: Autenticação via Certificado Digital (ICP-Brasil)
- **Shape**: Login com certificado digital (e-CPF/e-CNPJ) além de senha. Integração com cadeia ICP-Brasil.
- **Strengths**: Máxima segurança; conformidade com gov.br; elimina senhas
- **Weaknesses**: Complexidade alta; depende de middleware de certificado; nem todos usuários têm certificado
- **Best fit when**: Exigência de compliance governamental; sistema de alto risco

### Recommendation
- **Chosen option**: **Option B** (ASP.NET Core Identity + JWT) para o MVP
- **Why now**: Simplicidade; atende aos requisitos iniciais; time pequeno
- **What must be watched**: Preparar para migrar para Option A (Keycloak) se surgir necessidade de SSO; Option C pode ser adicionada como 2FA

---

## Opção 2: Estratégia de Geração de XML para e-Social

### Context
- **Problem**: Gerar XML no formato exato exigido pelos schemas XSD do e-Social v. S-1.3 para cada tipo de evento (S-1200, S-2200, S-5001, etc.)
- **Scope**: Módulos de Eventos Trabalhistas, Cálculo da Folha, Obrigações Fiscais
- **Constraints**: Schemas XSD fornecidos pelo governo; validação estrita; namespaces específicos

### Option A: Serialização via XmlSerializer + Classes Geradas (XSD.exe)
- **Shape**: Gerar classes C# a partir dos XSD usando `xsd.exe` ou `dotnet-xsd`. Serializar objetos para XML com `XmlSerializer`.
- **Strengths**: Tipagem forte; IntelliSense; validação em tempo de compilação
- **Weaknesses**: Classes grandes e complexas; difícil de manter quando schemas mudam; namespaces confusos
- **Best fit when**: Schemas estáveis; muitos tipos de eventos

### Option B: Geração via Template (Razor/Liquid/Scriban) + Validação XSD
- **Shape**: Templates de XML para cada tipo de evento. Preencher com dados e validar contra XSD.
- **Strengths**: Templates legíveis; fácil de ajustar quando schema muda; separação clara entre dados e formato
- **Weaknesses**: Sem validação em tempo de compilação; erros de sintaxe XML em runtime
- **Best fit when**: Schemas mudam com frequência; equipe familiarizada com templates

### Option C: Biblioteca Especializada (TecnoSpeed / eSocial SDK)
- **Shape**: Usar biblioteca de terceiros que abstrai a geração de XML e comunicação com e-Social.
- **Strengths**: Menos código; atualizações de layout fornecidas pelo vendor; suporte
- **Weaknesses**: Dependência de vendor; custo de licenciamento; possível limitação de customização
- **Best fit when**: Prazo curto; equipe pequena; orçamento para licença

### Recommendation
- **Chosen option**: **Option B** (Templates + Validação XSD) como estratégia principal, com fallback para Option A em eventos complexos
- **Why now**: Flexibilidade para mudanças de layout (NTs); menor acoplamento com classes geradas; custo zero
- **What must be watched**: Criar suíte de testes com XML de exemplo do governo; CI/CD valida todos os templates contra XSD

---

## Opção 3: Estratégia de Frontend — React SPA vs Next.js

### Context
- **Problem**: Escolher o framework frontend para o dashboard administrativo do Folha360
- **Scope**: Interface web (dashboard, gestão de funcionários, relatórios)
- **Constraints**: React (definido no contexto); integração com APIs .NET

### Option A: React SPA (Vite + React Router) — escolhido
- **Shape**: Single Page Application com Vite. React Router para navegação. Consome APIs REST.
- **Strengths**: Simples; build rápido (Vite); ampla comunidade; sem dependência de Node.js server
- **Weaknesses**: Sem SSR/SSG; SEO ruim (não relevante para sistema interno); bundle inicial grande
- **Best fit when**: Sistema interno; sem necessidade de SEO; time familiarizado com React

### Option B: Next.js (App Router)
- **Shape**: Next.js com App Router. SSR/SSG para páginas estáticas; API Routes como BFF.
- **Strengths**: Performance (SSR/SSG); SEO; BFF layer; roteamento avançado
- **Weaknesses**: Complexidade adicional; Node.js server necessário; curva de aprendizado
- **Best fit when**: Necessidade de SEO; páginas públicas; BFF para agregar APIs

### Recommendation
- **Chosen option**: **Option A** (React SPA com Vite) para o MVP
- **Why now**: Sistema interno não precisa de SEO; simplicidade; time familiarizado
- **What must be watched**: Se surgir necessidade de portal público (ex.: consulta de holerite pelo funcionário), migrar para Next.js

---

## Opção 4: Estratégia de Log e Observabilidade

### Context
- **Problem**: Sistema distribuído (6 módulos) precisa de logging centralizado, métricas e tracing para debugging e auditoria (LGPD).
- **Scope**: Todos os módulos
- **Constraints**: .NET 10; on-premise ou cloud privada; custo controlado

### Option A: Seq + Prometheus + Grafana (escolhido)
- **Shape**: Serilog → Seq (logs); Prometheus (métricas); Grafana (dashboards). Open-source, self-hosted.
- **Strengths**: Custo zero (open-source); integração nativa com .NET (Serilog); dashboards customizáveis
- **Weaknesses**: Self-hosted (exige manutenção); escala limitada
- **Best fit when**: On-premise; orçamento limitado; equipe de infra disponível

### Option B: Elastic Stack (Elasticsearch + Kibana + APM)
- **Shape**: Elasticsearch para logs; Kibana para visualização; APM para tracing distribuído.
- **Strengths**: Full-text search poderoso; APM integrado; escala horizontal
- **Weaknesses**: Pesado (RAM/CPU); complexidade operacional; custo de licenciamento (Elastic License)
- **Best fit when**: Grande volume de logs; necessidade de search avançado; tracing distribuído

### Option C: Azure Application Insights / AWS CloudWatch
- **Shape**: Serviço gerenciado de observabilidade na nuvem.
- **Strengths**: Zero manutenção; integração nativa; alertas inteligentes
- **Weaknesses**: Custo variável; vendor lock-in; exige cloud
- **Best fit when**: Cloud-native; equipe sem infra; orçamento para serviços gerenciados

### Recommendation
- **Chosen option**: **Option A** (Seq + Prometheus + Grafana)
- **Why now**: Custo zero; adequado ao porte do sistema; equipe pode gerenciar
- **What must be watched**: Se volume de logs crescer exponencialmente, migrar para Elastic Stack

---

## Summary de Opções em Aberto

| # | Decisão | Escolha Atual | Reavaliar Quando |
|---|---|---|---|
| O1 | Autenticação | ASP.NET Core Identity + JWT | Necessidade de SSO ou certificado digital |
| O2 | Geração XML e-Social | Templates + Validação XSD | Schemas muito complexos ou muitos eventos |
| O3 | Frontend | React SPA (Vite) | Necessidade de portal público ou SEO |
| O4 | Observabilidade | Seq + Prometheus + Grafana | Volume de logs > 10GB/dia |

## Recommended Next Skill
`adr-writer` — para criar ADRs formais das decisões arquiteturais já tomadas (mensageria, multi-tenant, processamento assíncrono).

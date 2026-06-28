# PRD G14 — Módulo Sistema (Cross-Cutting)

## Visão Geral

O módulo Sistema agrupa as funcionalidades transversais do Folha360: autenticação JWT, 
gerenciamento de tenants (multi-empresa), auditoria imutável e health checks.

**Problema resolvido**: O sistema precisa de uma camada de segurança robusta (JWT com refresh 
token), isolamento de dados por tenant (schema-per-tenant), rastreabilidade completa de 
alterações (audit_log) e monitoramento de saúde dos serviços.

**Público-alvo**: Admin (gestão de tenants e usuários), todos os perfis (login).

## Objetivos

- **Segurança**: Autenticação JWT com refresh token e perfis de acesso (Admin, Operador, Contador, Consulta)
- **Isolamento**: Multi-tenancy com schema por tenant (PostgreSQL)
- **Rastreabilidade**: Auditoria imutável de todas as operações de escrita
- **Monitoramento**: Health checks para todos os serviços de infraestrutura

## Histórias de Usuário

1. **Como qualquer usuário**, quero fazer login com e-mail e senha para acessar o sistema.
2. **Como qualquer usuário**, quero que meu token seja renovado automaticamente via refresh 
   token para não perder a sessão.
3. **Como Admin**, quero gerenciar tenants (criar, ativar, inativar) para isolar dados 
   de diferentes empresas.
4. **Como Admin**, quero gerenciar usuários com perfis de acesso (Admin, Operador, Contador, 
   Consulta) para controle de permissões.
5. **Como Admin**, quero consultar o log de auditoria para rastrear quem fez qual alteração 
   e quando.
6. **Como Admin**, quero verificar a saúde do sistema (health check) para monitorar 
   disponibilidade dos serviços.

## Funcionalidades Principais

### F1. Autenticação
**Requisitos funcionais**:
- RF01: O sistema DEVE autenticar usuários com e-mail e senha (hash)
- RF02: O sistema DEVE retornar JWT access token + refresh token
- RF03: O sistema DEVE permitir renovação de token via refresh token
- RF04: O sistema DEVE retornar dados do usuário (nome, e-mail, roles) e tenants disponíveis
- RF05: O sistema DEVE suportar perfis: Admin, Operador, Contador, Consulta

### F2. Tenants
**Requisitos funcionais**:
- RF06: O sistema DEVE isolar dados por schema PostgreSQL (schema-per-tenant)
- RF07: O sistema DEVE gerenciar status do tenant: Ativo, Inativo, Excluído

### F3. Auditoria
**Requisitos funcionais**:
- RF08: O sistema DEVE registrar todas as operações de escrita com: schema, tabela, 
  registro ID, ação, dados anteriores, dados novos, usuário, data/hora
- RF09: O sistema DEVE manter audit_log imutável (apenas insert, sem update/delete)

### F4. Health Check
**Requisitos funcionais**:
- RF10: O sistema DEVE expor endpoints de health check: `/health`, `/health/ready`, `/health/live`
- RF11: O sistema DEVE verificar status de todos os serviços: PostgreSQL, Redis, RabbitMQ, MinIO

## Experiência do Usuário

**Login**: Tela limpa com e-mail, senha e botão "Entrar". Feedback claro em caso de erro 
(credenciais inválidas, conta bloqueada).

**Health Check**: Endpoints para monitoramento (Prometheus/Grafana), não possui interface visual.

## Restrições Técnicas de Alto Nível

- **Autenticação**: JWT com refresh token; senhas com hash (bcrypt/argon2)
- **Multi-tenancy**: Schema-per-tenant no PostgreSQL; tenant resolvido via claims do JWT
- **Auditoria**: Tabela `audit_log` imutável; triggers ou interceptors para captura automática
- **Monitoramento**: Health checks compatíveis com Kubernetes liveness/readiness probes

## Fora de Escopo

- Cadastro de usuários (gerenciado via seed/admin — sem self-registration)
- Single Sign-On (SSO) e integração com LDAP/AD (feature futura)
- Rate limiting e proteção contra brute force no login (feature futura — F10 Segurança)

---

## Anexo A — Referência de Endpoints

| Funcionalidade | Controller | Base Path |
|---------------|-----------|-----------|
| Autenticação | `AuthController` | `/api/auth` |
| Health Check | `HealthController` | `/health` |

Ver detalhes em: `docs/outputs/agrupamento/agrupamento-cadastros-processos.md` — Seção 14.

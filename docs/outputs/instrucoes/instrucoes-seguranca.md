# 🛡️ Boas Práticas — Segurança

> **Escopo**: LGPD, OWASP, criptografia, autenticação, autorização  
> **Referências**: [Modelo de Banco](./database-model), [ADR-003](./adr-003-schema-por-tenant)  
> **Fonte completa**: [`docs/instrucoes/07-seguranca.md`](../instrucoes/07-seguranca.md)

---

## OWASP Top 10

| Risco | Prevenção no Folha360 |
|---|---|
| A01 — Broken Access Control | Resource-based auth por tenant, policy-based por role |
| A03 — Injection | EF Core parametrizado, NUNCA concatenar SQL |
| A05 — Security Misconfiguration | Error messages genéricos, headers de segurança |
| A07 — XSS | React escapa por padrão, CSP headers |
| A08 — Data Integrity | `dotnet audit`, `npm audit`, Trivy scan |

## Autenticação

- **JWT** com claims: `tenant_id`, `role` (admin, operador, contador, consulta)
- **Refresh token rotation**: antigo invalidado após uso
- **Blacklist no Redis**: logout invalida token até expiração
- **Rate limiting**: 5 tentativas de login por 15 minutos

## Autorização

| Role | Permissões |
|---|---|
| **admin** | Todas as 28 telas, CRUD total, e-Social |
| **operador** | Cadastros, Eventos, Folha |
| **contador** | Obrigações Fiscais, Relatórios |
| **consulta** | Históricos, Holerites (somente leitura) |

## LGPD — Dados Sensíveis

| Dado | Proteção |
|---|---|
| CPF, CTPS, PIS | AES-256 em repouso |
| Salário líquido | AES-256 em repouso |
| Documentos | AES-256 + MinIO criptografado |

### Direito ao Esquecimento

- Soft delete → anonimização em 7 dias
- Hard delete: `DROP SCHEMA tenant_XXX CASCADE`
- `audit_log` mantém registro da exclusão (imutável)

## Criptografia

| Contexto | Algoritmo | Uso |
|---|---|---|
| Dados em repouso | AES-256-GCM | CPF, salários, documentos |
| Dados em trânsito | TLS 1.3 | Todas as comunicações |
| Senhas | Argon2id | Hashing com salt |
| e-Social | Certificado A1 (ICP-Brasil) | Assinatura XML |
| API | JWT (HMAC-SHA256) | Autenticação |

## Headers de Segurança

```
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
Strict-Transport-Security: max-age=31536000; includeSubDomains
Content-Security-Policy: default-src 'self'; script-src 'self'
Referrer-Policy: strict-origin-when-cross-origin
```

## Anti-Patterns de Segurança

| ❌ Não Fazer | Consequência |
|---|---|
| Log de dados sensíveis (`_log.Info(cpf)`) | Violação LGPD |
| Secrets em variáveis de ambiente plain | Exposição em logs |
| SQL concatenado (`$"WHERE id = '{id}'"`) | SQL injection |
| Senha em plain text | Vazamento massivo |
| CORS `*` (allow all) | CSRF/XSS cross-origin |
| Certificado e-Social no código-fonte | Exposição em git |

---

**Fonte completa com exemplos de código**: [`docs/instrucoes/07-seguranca.md`](../instrucoes/07-seguranca.md)

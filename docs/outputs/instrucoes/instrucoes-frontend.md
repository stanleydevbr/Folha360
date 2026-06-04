# ⚛️ Boas Práticas — Frontend React

> **Escopo**: React 18, TypeScript, Tailwind CSS, TanStack Query  
> **Referências**: [Protótipo Frontend](./frontend-prototype), [ADR-004](./adr-004-processamento-assincrono-folha)  
> **Fonte completa**: [`docs/instrucoes/02-frontend-react.md`](../instrucoes/02-frontend-react.md)

---

## Estrutura de Pastas (Feature-First)

```
src/
├── features/
│   ├── cadastros/       # Components, hooks, api, types, pages
│   ├── folha/
│   ├── fiscais/
│   ├── eventos/
│   ├── relatorios/
│   └── esocial/
├── shared/              # Componentes reutilizados em 3+ features
├── layouts/             # AppLayout, Header, Sidebar
└── routes/              # React Router config
```

## TypeScript

- **Strict mode** obrigatório, zero `any`
- Interfaces para API contracts
- Discriminated unions para estado de loading

## Estado

| Tipo | Solução | Quando |
|---|---|---|
| Server state | **TanStack Query** | Dados da API (funcionários, folhas) |
| URL state | `useSearchParams` | Filtros, paginação, aba ativa |
| Client state | **Zustand** (mínimo) | Sidebar colapsada, tema |

## Formulários

- React Hook Form + Zod para validação
- Field-level validation com máscaras (CPF, CTPS)

## Acessibilidade (WCAG 2.1 AA)

- aria-labels em ações iconográficas
- Keyboard navigation + focus management
- Contraste mínimo 4.5:1 (texto)
- Skip navigation link

## Processamento Assíncrono (ADR-004)

- SignalR para progresso em tempo real
- Polling a cada 5s como fallback
- `202 Accepted` + barra de progresso

## Anti-Patterns

| ❌ Não Fazer | Consequência |
|---|---|
| Estado duplicado (server + client) | Inconsistência, bugs |
| `useEffect` para buscar dados sem TanStack Query | Cache inexistente, loading manual |
| Qualquer tipo como `any` | Perde type-safety |
| Organizar por tipo (components/, hooks/ na raiz) | Difícil localizar feature |

---

**Fonte completa com exemplos de código**: [`docs/instrucoes/02-frontend-react.md`](../instrucoes/02-frontend-react.md)

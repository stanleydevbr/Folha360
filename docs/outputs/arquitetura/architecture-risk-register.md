# Architecture Risk Register — Folha360

## Summary
Registro de riscos arquiteturais do sistema Folha360, identificados a partir da análise das visões de camadas, componentes, integração, deployment e runtime. Cada risco é classificado por probabilidade e impacto para gerar severidade e ações de mitigação.

---

## Escala de Classificação

| Nível | Probabilidade | Impacto |
|---|---|---|
| 1 — Baixo | < 20% de ocorrer no projeto | Atraso < 1 dia; afeta 1 módulo |
| 2 — Médio | 20-50% de ocorrer | Atraso 1-5 dias; afeta 2-3 módulos |
| 3 — Alto | 50-80% de ocorrer | Atraso 1-2 semanas; afeta sistema parcialmente |
| 4 — Crítico | > 80% de ocorrer | Atraso > 2 semanas; sistema indisponível |

**Severidade** = Probabilidade × Impacto (1-16)

---

## Registro de Riscos

| # | Risco | Trigger | Impacto | Prob. | Imp. | Sev. | Mitigação | Experimento/Evidência |
|---|---|---|---|---|---|---|---|---|
| **R1** | **Indisponibilidade do e-Social no prazo de envio** | Portal gov.br fora do ar no dia do fechamento | Multas por atraso; eventos não enviados | 3 | 4 | **12** | Retry 24h com backoff; dead-letter queue; alerta operacional; procedimento de contingência manual | Teste de indisponibilidade simulada (chaos engineering) |
| **R2** | **Vazamento de dados sensíveis de funcionários** | Ataque interno/externo; configuração incorreta de segurança | Multa LGPD (até 2% faturamento); dano reputacional; processos judiciais | 2 | 4 | **8** | Criptografia AES-256 em repouso; TLS everywhere; audit log imutável; pentest semestral; network policies K8s | Pentest externo; revisão de código de segurança |
| **R3** | **Processamento da folha excede 2 horas** | Volume de funcionários > 100K; consultas lentas; gargalo de CPU | Atraso no fechamento; insatisfação do cliente; multas por atraso em obrigações | 3 | 3 | **9** | Batch processing com paralelismo; cache Redis; read replica para consultas; HPA no K8s; particionamento por empresa | Teste de carga com 150K funcionários simulados |
| **R4** | **Certificado digital A1 expirado** | Falha na renovação; falta de monitoramento | Impossibilidade de enviar eventos ao e-Social; multas; obrigações não cumpridas | 2 | 4 | **8** | Monitor de expiração com alerta 30, 15, 7, 1 dias antes; procedimento documentado de renovação; certificado backup A3 | Verificação mensal de validade no CI/CD |
| **R5** | **Inconsistência de dados entre módulos** | Falha em mensageria; evento perdido; bug de concorrência | Cálculo incorreto da folha; divergência fiscal; retrabalho | 3 | 3 | **9** | Eventos de domínio imutáveis; reconciliação batch diária; idempotência em todos os consumidores; saga pattern com compensação | Teste de integração com injeção de falhas no RMQ |
| **R6** | **Single point of failure no PostgreSQL Primary** | Falha de hardware; corrupção de dados; desastre no datacenter | Sistema inteiro indisponível; perda de dados (RPO) | 2 | 4 | **8** | Patroni + etcd para auto-failover; backups a cada 1h; WAL shipping para réplica; disaster recovery documentado | Simulação de failover trimestral |
| **R7** | **Alteração de layout e-Social (nova NT) com prazo curto** | Governo publica NT com 30 dias para adequação | Multas; envios rejeitados; sistema fora de conformidade | 3 | 3 | **9** | Schemas XSD versionados; CI/CD monitora portal e-Social; equipe dedicada para atualização; testes automatizados de validação XSD | Verificação semanal de novas NTs no portal |
| **R8** | **Sobrecarga no fechamento mensal (todos processam ao mesmo tempo)** | 50+ operadores iniciam fechamento simultâneo | APIs sobrecarregadas; timeouts; experiência ruim | 3 | 2 | **6** | Rate limiting; fila de processamento (uma empresa por vez); agendamento de fechamento; HPA agressivo no período | Teste de carga com 50 usuários simultâneos |
| **R9** | **Falha na validação XSD de eventos e-Social** | Schema desatualizado; bug no gerador XML; campo obrigatório ausente | Lote rejeitado; retrabalho; atraso no envio | 3 | 2 | **6** | Validação XSD pré-envio; testes unitários para cada tipo de evento; CI/CD valida schemas; isolamento de evento com erro | Testes automatizados com XML de exemplo do governo |
| **R10** | **Perda de eventos na fila RabbitMQ** | Crash do nó RMQ; configuração sem persistência; fila cheia (overflow) | Eventos de domínio perdidos; inconsistência entre módulos; cálculo incorreto | 2 | 3 | **6** | Mensagens persistentes; publisher confirm; cluster RMQ; dead-letter queue; reconciliação batch | Teste de kill do nó RMQ e recuperação |
| **R11** | **Complexidade de implantação multi-módulo** | Deploy de 6 APIs + frontend + infra; dependências entre versões | Deploy lento; rollback complexo; ambiente instável | 3 | 2 | **6** | Docker Compose para dev; Helm charts para K8s; CI/CD pipeline por módulo; smoke tests pós-deploy; versionamento de APIs | Pipeline de deploy automatizado testado |
| **R12** | **Mudança de regime tributário da empresa** | Empresa muda de Lucro Real para Simples Nacional (ou vice-versa) | Regras de cálculo fiscal incorretas; apuração errada; risco fiscal para o cliente | 2 | 3 | **6** | Strategy pattern para regimes tributários; configuração por empresa; testes com diferentes regimes | Testes unitários com cenários de regimes diferentes |
| **R13** | **Latência de replicação PostgreSQL impacta relatórios** | Lag de replicação > 30s por carga pesada no primary | Relatórios mostram dados desatualizados; decisões erradas | 2 | 2 | **4** | Monitor de lag; alerta se > 10s; fallback para primary em consultas críticas | Monitoramento contínuo de lag |
| **R14** | **Equipe não familiarizada com a stack proposta** | Contratação difícil; curva de aprendizado alta | Atraso no desenvolvimento; bugs por falta de domínio | 3 | 2 | **6** | Documentação interna; pair programming; treinamento inicial; padrões documentados | Avaliação de senioridade da equipe |
| **R15** | **Fórmula NCalc com loop infinito ou acesso indevido** | Usuário cadastra fórmula maliciosa ou com erro de sintaxe | Crash do motor de cálculo; funcionário não processado; lentidão generalizada | 3 | 3 | **9** | Sandbox com timeout 100ms e memory limit; whitelist de funções; validador de sintaxe pré-execução; fórmula marcada como `formula_erro` e isolada | Teste com fórmulas maliciosas conhecidas |
| **R16** | **Ciclo em composição hierárquica de rubricas** | Usuário cria composição circular (A → B → A) | Loop infinito no cálculo; stack overflow; processo abortado | 2 | 3 | **6** | Detecção de ciclos via DFS no momento do cadastro; validação pré-cálculo; alerta visual no editor drag-and-drop | Testes com grafos cíclicos de rubricas |
| **R17** | **Tabela progressiva desatualizada (IRRF/INSS)** | Mudança anual nas tabelas oficiais não aplicada a tempo | Cálculos fiscais incorretos; divergência com governo; multas | 3 | 3 | **9** | Versionamento anual de tabelas; script de seed com tabelas oficiais; alerta de vigência; endpoint `GET /api/rubricas/conformidade` | Verificação trimestral de tabelas oficiais |
| **R18** | **Inconsistência entre rubricas locais e Tabela 03 e-Social** | Rubrica cadastrada sem `tipo_esocial` ou com código incorreto | Evento S-1010 rejeitado; retrabalho; atraso no envio | 3 | 3 | **9** | Validação de `tipo_esocial` contra catálogo oficial; endpoint de conformidade; dashboard de rubricas não mapeadas; CI/CD monitora portal e-Social | Testes de validação XSD para S-1010 |

---

## Heatmap de Riscos

|   | Impacto 1 | Impacto 2 | Impacto 3 | Impacto 4 |
|---|---|---|---|---|
| **Prob. 4** | — | — | — | — |
| **Prob. 3** | — | R8, R9, R11 | R3, R5, R7, R15, R17, R18 | R1 |
| **Prob. 2** | — | R13 | R10, R12, R14, R16 | R2, R4, R6 |
| **Prob. 1** | — | — | — | — |

---

## Top 5 Riscos por Severidade

| # | Risco | Severidade | Ação Imediata |
|---|---|---|---|
| **R1** | Indisponibilidade e-Social | 12 | Implementar retry robusto + dead-letter queue + alerta |
| **R3** | Processamento > 2h | 9 | Teste de carga com 150K; otimizar batch; HPA |
| **R5** | Inconsistência entre módulos | 9 | Idempotência; reconciliação batch; saga pattern |
| **R7** | Novo layout e-Social | 9 | CI/CD monitora portal; schemas versionados |
| **R15** | Fórmula NCalc maliciosa/loop | 9 | Sandbox com timeout 100ms; whitelist de funções |
| **R17** | Tabela progressiva desatualizada | 9 | Versionamento anual; alerta de vigência; seed data |
| **R18** | Inconsistência rubricas × Tabela 03 | 9 | Validação contínua; dashboard de conformidade |

## Evidence vs Assumptions

**Evidências**:
- e-Social historicamente tem instabilidades em prazos de entrega (fim de mês)
- LGPD prevê multas de até 2% do faturamento para vazamentos
- Certificados digitais ICP-Brasil têm validade de 1-3 anos

**Assumptions**:
- Volume de 100K funcionários é realista para o cliente alvo
- Equipe tem experiência com .NET e PostgreSQL, mas não necessariamente com e-Social
- Kubernetes estará disponível no ambiente de produção

## Recommended Next Skill
`tradeoff-analysis-writer` — para analisar tradeoffs das principais decisões arquiteturais (monólito vs modular, mensageria, cache).

# Quality Attribute Scenarios — Folha360: Domínios de Cálculo Fiscal e Social

## Summary

Este documento complementa o artefato principal [`quality-attribute-scenarios.md`](./quality-attribute-scenarios.md), que já cobre 15 cenários distribuídos entre Performance (P1-P3), Availability (A1-A3), Security (S1-S3), Scalability (SC1-SC2), Maintainability (M1-M3) e Usability (U1).

O foco aqui são **cenários específicos dos domínios de cálculo fiscal e social** que impactam diretamente a precisão e a conformidade legal do processamento da folha de pagamento:

- **Vínculos Sindicais**: contribuição sindical, assistencial e confederativa definidas por Convenção Coletiva de Trabalho (CCT), com vigência, tetos e múltiplos sindicatos por estabelecimento.
- **Planos de Saúde**: reajustes de mensalidade por faixa etária ou sinistralidade, inclusão/exclusão de dependentes, coparticipação e portabilidade.
- **Bases de Cálculo Governamentais**: tabelas progressivas de IRRF e INSS versionadas anualmente, teto do Salário-Família, alíquota de FGTS (8% ou 11,2% para menor aprendiz), deduções legais (dependentes, pensão alimentícia, previdência complementar).
- **11 Tipos de Cálculo**: mensal, férias, 13º salário, rescisão, dissídio coletivo, complementar, auxílio-doença, salário-maternidade, acordo trabalhista, estágio e RPA — cada um com regras fiscais e sociais distintas.
- **Integração Eventos Trabalhistas → Cálculo**: impacto automático de admissões (S-2200), afastamentos (S-2230) e desligamentos (S-2299) na folha corrente e em recálculos retroativos.

Cada cenário segue o formato ISO 25010 / SEI com **medidas quantitativas e verificáveis**, priorizados por criticidade considerando impacto legal/fiscal, frequência de ocorrência e esforço de mitigação.

---

## Performance

### Cenário P4: Recálculo Retroativo por Evento Trabalhista (S-2230)

- **Attribute**: Performance (latency + throughput)
- **Source/stimulus**: RH registra afastamento por auxílio-doença (S-2230) com data retroativa de 15 dias, impactando 3 competências já fechadas para 1 funcionário
- **Environment**: Produção, durante o dia útil, com folha do mês corrente em andamento para outros funcionários
- **Artifact**: Módulo de Cálculo da Folha (motor de recálculo) + Fila RMQ (evento `FuncionarioAfastado`)
- **Response**: Sistema recalcula automaticamente as 3 competências afetadas, ajusta bases de INSS e IRRF, gera diferenças de vencimentos/descontos e emite eventos retificadores (S-1200/S-1210) para o período
- **Response measure**: Recálculo de 3 competências para 1 funcionário < 30 segundos; diferenças apuradas com precisão de R$ 0,01; nenhuma competência de outros funcionários afetada
- **Priority**: Crítica

### Cenário P5: Processamento dos 11 Tipos de Cálculo em Lote

- **Attribute**: Performance (throughput)
- **Source/stimulus**: No fechamento de dezembro, RH processa simultaneamente: folha mensal (100.000 func.), 13º salário (100.000 func.), e férias proporcionais para 5.000 funcionários com desligamento programado
- **Environment**: Produção, último dia útil do ano, pico de carga
- **Artifact**: Módulo de Cálculo da Folha (orquestrador de fechamento) + PostgreSQL + Redis
- **Response**: Sistema paraleliza os 3 tipos de cálculo em lotes independentes de 1.000 funcionários, consolida totais por empresa e gera holerites distintos para cada tipo
- **Response measure**: Tempo total combinado < 3 horas; throughput sustentado > 30 funcionários/segundo por tipo de cálculo; uso de CPU < 85% nos nós de processamento
- **Priority**: Crítica

### Cenário P6: Cálculo de Férias com Múltiplas Rubricas Compostas

- **Attribute**: Performance (latency)
- **Source/stimulus**: RH processa férias de 1 funcionário com: adicional de 1/3 constitucional, abono pecuniário (10 dias), média de horas extras (12 meses), adicional noturno, adicional de insalubridade e terço constitucional sobre todas as verbas
- **Environment**: Produção, operação individual no portal do RH
- **Artifact**: Módulo de Cálculo da Folha (fase de vencimentos → bases) + NCalc engine
- **Response**: Sistema resolve a árvore hierárquica de rubricas (6 rubricas com dependências entre si), calcula bases de INSS e IRRF específicas para férias, e apresenta o demonstrativo
- **Response measure**: Tempo de cálculo < 5 segundos; todas as rubricas calculadas com precisão de R$ 0,01; fórmula de média de horas extras considera exatamente 12 meses anteriores
- **Priority**: Alta

### Cenário P7: Simulação de Impacto de Nova Rubrica em Larga Escala

- **Attribute**: Performance (throughput)
- **Source/stimulus**: Analista de RH simula o impacto financeiro de uma nova rubrica de "Auxílio Home Office — R$ 300,00" para 50.000 funcionários antes do fechamento
- **Environment**: Produção, ambiente de simulação (sandbox) isolado do cálculo oficial
- **Artifact**: Módulo de Cálculo da Folha (motor de simulação) + Redis (cache de bases)
- **Response**: Sistema executa simulação em modo dry-run, aplicando a nova rubrica sobre a base de cálculo do mês corrente, gerando relatório de impacto por centro de custo
- **Response measure**: Simulação concluída < 5 minutos para 50.000 funcionários; impacto financeiro total com precisão de ±0,5%; zero impacto nos dados oficiais da folha
- **Priority**: Média

---

## Disponibilidade (Availability)

### Cenário A4: Indisponibilidade do Motor NCalc Durante Cálculo

- **Attribute**: Availability (fault tolerance)
- **Source/stimulus**: Durante o processamento da folha de 100.000 funcionários, o serviço de avaliação de fórmulas NCalc apresenta timeout ou falha intermitente em 5% das requisições
- **Environment**: Produção, fechamento da folha (dia 28)
- **Artifact**: Módulo de Cálculo da Folha (FormulaEvaluationService) + RabbitMQ (DLQ)
- **Response**: Sistema realiza retry com exponential backoff (3 tentativas, intervalo 2s/4s/8s); falhas persistentes são enviadas para Dead Letter Queue; lote problemático é isolado; funcionários com fórmula não resolvida são marcados como "pendente de cálculo manual"
- **Response measure**: 99,9% das fórmulas resolvidas em até 3 tentativas; < 0,1% dos funcionários encaminhados para cálculo manual; processamento total não excede 2,5 horas mesmo com degradação
- **Priority**: Alta

### Cenário A5: Inconsistência de Dados Entre Eventos Trabalhistas e Cálculo

- **Attribute**: Availability (data integrity)
- **Source/stimulus**: Evento S-2200 (admissão) é processado com sucesso, mas a mensagem RMQ para o módulo de Cálculo é perdida por crash do consumer antes do commit
- **Environment**: Produção, durante operação normal de admissão
- **Artifact**: Módulo de Eventos Trabalhistas → RabbitMQ (persistent messages + publisher confirm) → Módulo de Cálculo
- **Response**: Sistema detecta a ausência do evento de admissão na competência corrente via job de reconciliação diário; reenvia o evento a partir do log de eventos trabalhistas; recalcula a folha do funcionário admitido
- **Response measure**: Inconsistência detectada em < 24 horas (job diário); reconciliação automática para 100% dos eventos perdidos; tempo de recuperação < 5 minutos por funcionário
- **Priority**: Alta

### Cenário A6: Falha na Integração com Operadora de Plano de Saúde

- **Attribute**: Availability (graceful degradation)
- **Source/stimulus**: API da operadora de plano de saúde fica indisponível durante a janela de importação mensal de descontos (coparticipação, reajuste de mensalidade) para 20.000 funcionários
- **Environment**: Produção, dia 20 do mês (fechamento da folha se aproximando)
- **Artifact**: Módulo de Cálculo da Folha (HealthPlanIntegrationService) + RabbitMQ (retry queue)
- **Response**: Sistema utiliza última tabela de descontos cached (Redis, TTL 72h) com flag de "valores provisórios"; notifica operadores de RH sobre a indisponibilidade; retoma importação automaticamente quando API volta
- **Response measure**: Cálculo prossegue com valores cached para 100% dos funcionários afetados; divergência entre valor cached e valor real < 3% (variação mensal típica); importação definitiva concluída em < 1 hora após restabelecimento
- **Priority**: Média

---

## Segurança (Security)

### Cenário S4: Injeção de Fórmula Maliciosa no Motor NCalc

- **Attribute**: Security (integrity)
- **Source/stimulus**: Usuário com permissão de edição de rubricas tenta injetar código malicioso via fórmula NCalc (ex.: `File.ReadAllText("/etc/passwd")` ou `DROP TABLE`)
- **Environment**: Produção, interface de cadastro de rubricas
- **Artifact**: Módulo de Cadastros (RubricaValidator + NCalc Sandbox)
- **Response**: Sandbox de fórmulas bloqueia acesso a namespaces perigosos (System.IO, System.Diagnostics, System.Reflection, System.Data, System.Net); validador rejeita a fórmula antes da persistência; evento de segurança é registrado em audit log
- **Response measure**: 100% das funções não-whitelistadas são bloqueadas; whitelist restrita a funções matemáticas (Abs, Ceiling, Floor, Round, Max, Min), condicionais (If) e referências a rubricas do sistema; tentativa de injeção gera alerta de segurança em < 1 minuto
- **Priority**: Crítica

### Cenário S5: Vazamento de Dados de Contribuição Sindical Entre Empresas

- **Attribute**: Security (confidentiality — multi-tenant isolation)
- **Source/stimulus**: Operador de RH da Empresa A tenta consultar valores de contribuição sindical de funcionários da Empresa B via API manipulando `tenant_id` no token JWT
- **Environment**: Produção, consulta de relatório sindical
- **Artifact**: Auth Gateway (JWT + TenantResolutionStrategy) + PostgreSQL (schema-per-tenant + Row-Level Security)
- **Response**: Sistema valida `tenant_id` do token JWT contra o schema do banco; Row-Level Security no PostgreSQL bloqueia qualquer leitura cross-schema; tentativa é registrada em audit log com IP e timestamp
- **Response measure**: 100% das tentativas de acesso cross-tenant bloqueadas; latência adicional por validação RLS < 5ms por query; alerta de segurança emitido para 3+ tentativas do mesmo IP em 5 minutos
- **Priority**: Crítica

### Cenário S6: Manipulação Indevida de Tabelas Progressivas (INSS/IRRF)

- **Attribute**: Security (integrity + non-repudiation)
- **Source/stimulus**: Usuário interno com acesso administrativo tenta alterar retroativamente a tabela de INSS vigente para reduzir descontos de um grupo de funcionários
- **Environment**: Produção, interface de manutenção de tabelas fiscais
- **Artifact**: Módulo de Cadastros (TabelaProgressivaService) + Audit Log imutável
- **Response**: Sistema versiona tabelas progressivas por ano com registro imutável de criação; alterações em tabelas já vigentes exigem aprovação dual (4-eyes principle); todas as alterações são registradas em audit log com hash do conteúdo anterior
- **Response measure**: 100% das alterações em tabelas vigentes requerem dupla aprovação; audit log armazena diff completo (antes/depois) com hash SHA-256; tabelas de anos anteriores são imutáveis (read-only após virada do ano fiscal)
- **Priority**: Crítica

### Cenário S7: Conformidade LGPD — Anonimização de Base de Cálculo para Relatórios

- **Attribute**: Security (privacy by design)
- **Source/stimulus**: Contador terceirizado precisa acessar relatório de bases de cálculo de INSS por centro de custo, mas não pode visualizar CPF ou matrícula individual dos funcionários
- **Environment**: Produção, geração de relatório gerencial
- **Artifact**: Módulo de Relatórios (DataMaskingService) + PostgreSQL (views anonimizadas)
- **Response**: Sistema aplica máscara de dados em CPF (ex.: `***.456.789-**`) e substitui matrícula por hash não-reversível; agrega valores por centro de custo quando grupo tem < 3 funcionários (k-anonymity); mantém rastreabilidade apenas para auditoria fiscal
- **Response measure**: 100% dos relatórios para perfil "contador terceirizado" têm CPF mascarado; grupos com < 3 funcionários têm valores agregados; dados originais preservados apenas no schema de auditoria (acesso restrito)
- **Priority**: Alta

---

## Manutenibilidade (Maintainability)

### Cenário M4: Alteração de Alíquota de Contribuição Sindical por Convenção Coletiva

- **Attribute**: Maintainability (modifiability)
- **Source/stimulus**: Nova Convenção Coletiva de Trabalho (CCT) é homologada em 15 de março, alterando a contribuição assistencial de 1% para 1,5% do salário-base, com teto de R$ 150,00, vigência retroativa a 1º de março
- **Environment**: Produção, durante o mês corrente, com folha ainda não fechada
- **Artifact**: Módulo de Cadastros (subsistema de vínculos sindicais) + Módulo de Cálculo
- **Response**: Usuário cadastra nova alíquota com vigência retroativa; sistema recalcula automaticamente a contribuição para todos os funcionários do sindicato afetado na competência corrente; gera relatório de diferenças para conferência
- **Response measure**: Cadastro da nova alíquota < 10 minutos; recálculo automático para até 50.000 funcionários do sindicato < 15 minutos; sem necessidade de deploy ou alteração de código; rastreabilidade completa da CCT (número, data de homologação, sindicato)
- **Priority**: Alta

### Cenário M5: Inclusão de Nova Rubrica Legal Obrigatória (Ex.: Nova Contribuição Previdenciária)

- **Attribute**: Maintainability (evolvability)
- **Source/stimulus**: Governo federal publica MP criando nova contribuição previdenciária complementar de 0,5% sobre a folha, com incidência específica (apenas sobre verbas de natureza remuneratória acima de R$ 5.000,00), vigência em 90 dias
- **Environment**: Desenvolvimento → Homologação → Produção (janela de 90 dias)
- **Artifact**: Módulo de Cálculo da Folha (motor de rubricas + bases) + Módulo de Obrigações Fiscais
- **Response**: Equipe configura a nova rubrica como "desconto legal" com fórmula condicional (`If({BASE_REMUNERATORIA} > 5000, {BASE_REMUNERATORIA} * 0.005, 0)`), vincula à base de cálculo apropriada, adiciona ao evento S-5001 (totais de contribuição) e testa em sandbox
- **Response measure**: Configuração completa < 8 horas de trabalho; 100% parametrizável (sem alteração de código-fonte); coberta por 20+ cenários de teste automatizados; implantação em produção possível em qualquer dia do mês
- **Priority**: Alta

### Cenário M6: Versionamento e Coexistência de Tabelas Progressivas (IRRF/INSS)

- **Attribute**: Maintainability (versioning)
- **Source/stimulus**: Receita Federal publica nova tabela progressiva de IRRF com vigência a partir de 1º de janeiro; folha de dezembro (competência anterior) ainda precisa usar tabela antiga para o cálculo do 13º salário
- **Environment**: Transição de ano fiscal (dezembro/janeiro)
- **Artifact**: Módulo de Cálculo da Folha (TabelaProgressivaService + estratégia de versionamento)
- **Response**: Sistema mantém ambas as tabelas ativas simultaneamente; seleciona a tabela correta por competência (não por data de processamento); 13º salário de dezembro usa tabela do ano corrente; folha de janeiro usa nova tabela automaticamente
- **Response measure**: Seleção de tabela 100% correta por competência; zero erros de aplicação de tabela errada em testes de regressão; transição de ano fiscal não requer deploy (tabelas pré-cadastradas)
- **Priority**: Alta

### Cenário M7: Reajuste de Plano de Saúde por Faixa Etária com Diferentes Operadoras

- **Attribute**: Maintainability (configurability)
- **Source/stimulus**: 3 operadoras de plano de saúde diferentes aplicam reajustes distintos por faixa etária (Operadora A: +8% para 40-49 anos; Operadora B: +12% para 45-54 anos; Operadora C: reajuste por sinistralidade de +6,5% global), todos com vigência em meses diferentes
- **Environment**: Produção, durante o ano, múltiplas competências
- **Artifact**: Módulo de Cadastros (subsistema de planos de saúde) + Módulo de Cálculo
- **Response**: Usuário cadastra tabelas de reajuste por operadora, faixa etária e vigência; sistema aplica automaticamente o reajuste correto na competência de vigência; funcionários que mudam de faixa etária no mês têm desconto recalculado
- **Response measure**: Cadastro de reajuste por operadora < 5 minutos; aplicação automática para 100% dos funcionários afetados na competência correta; relatório de impacto financeiro do reajuste disponível antes do fechamento
- **Priority**: Média

### Cenário M8: Alteração de Regra de Cálculo de Rescisão por Mudança na CLT

- **Attribute**: Maintainability (modifiability)
- **Source/stimulus**: Reforma trabalhista altera regra de cálculo de aviso prévio proporcional (de 3 dias/ano para 5 dias/ano), impactando o tipo de cálculo "Rescisão"
- **Environment**: Desenvolvimento → Homologação → Produção (vacatio legis de 60 dias)
- **Artifact**: Módulo de Cálculo da Folha (estratégia de cálculo de rescisão — Strategy Pattern)
- **Response**: Equipe altera a estratégia `AvisoPrevioProporcionalStrategy` isoladamente; testes de regressão específicos para rescisão são executados; demais tipos de cálculo não são afetados
- **Response measure**: Alteração de código isolada em 1 classe (Strategy Pattern); cobertura de testes de regressão > 95% para o tipo "Rescisão"; deploy em produção < 2 horas; zero impacto nos outros 10 tipos de cálculo
- **Priority**: Média

---

## Escalabilidade (Scalability)

### Cenário SC3: Processamento de Dissídio Coletivo para 100.000 Funcionários

- **Attribute**: Scalability (elasticity)
- **Source/stimulus**: Dissídio coletivo é homologado com reajuste de 5% retroativo a 3 meses, exigindo recálculo de 3 competências para 100.000 funcionários, com geração de diferenças salariais e recolhimento complementar de INSS/FGTS
- **Environment**: Produção, fora do período de fechamento mensal
- **Artifact**: Módulo de Cálculo da Folha (motor de dissídio) + RabbitMQ (particionamento por lote)
- **Response**: Sistema divide o recálculo em 100 lotes de 1.000 funcionários, processando 4 lotes em paralelo (4 consumers RMQ); cada lote recalcula 3 competências, gera diferenças e emite eventos retificadores; barra de progresso consolidada no dashboard
- **Response measure**: Recálculo total < 4 horas; throughput > 20 funcionários/segundo por consumer; escala linear com adição de consumers (até 10 consumers simultâneos); zero impacto em outras operações do sistema
- **Priority**: Alta

### Cenário SC4: Expansão de Vínculos Sindicais (Múltiplos Sindicatos por Estabelecimento)

- **Attribute**: Scalability (data volume)
- **Source/stimulus**: Empresa com 50 estabelecimentos em 20 estados diferentes passa a ter 45 sindicatos distintos (por categoria e região), cada um com CCT própria, alíquotas e tetos diferentes
- **Environment**: Produção, crescimento orgânico ao longo de 3 anos
- **Artifact**: Módulo de Cadastros (subsistema de vínculos sindicais) + Módulo de Cálculo
- **Response**: Sistema gerencia matriz de vínculo sindical (estabelecimento × categoria × sindicato); cálculo da folha aplica alíquotas corretas por sindicato sem degradação de performance; relatórios sindicais (contribuição patronal + empregado) são gerados por sindicato
- **Response measure**: Tempo de cálculo com 45 sindicatos < 110% do tempo com 1 sindicato; isolamento de regras por sindicato sem interferência; cadastro de novo sindicato < 30 minutos
- **Priority**: Média

---

## Usabilidade (Usability)

### Cenário U2: Validação de Fórmula de Rubrica com Feedback Imediato em Sandbox

- **Attribute**: Usability (error prevention)
- **Source/stimulus**: Analista de RH (usuário não-técnico) cria nova rubrica de "Adicional de Periculosidade" com fórmula `{SALARIO_BASE} * 0.30` e quer testar o impacto antes de ativar
- **Environment**: Produção, interface de cadastro de rubricas (React)
- **Artifact**: Frontend (RubricaSandbox) + API de simulação
- **Response**: Sistema oferece editor de fórmulas com autocomplete de rubricas disponíveis, validação sintática em tempo real (< 500ms), destaque de erros (ex.: referência circular), e botão "Testar com funcionário real" que simula o cálculo para 5 funcionários de perfis diferentes e exibe o demonstrativo
- **Response measure**: Validação sintática < 500ms após cada tecla; simulação para 5 funcionários < 10 segundos; 100% dos erros de sintaxe detectados antes da persistência; zero rubricas inválidas ativadas em produção
- **Priority**: Alta

### Cenário U3: Alerta Proativo de Vencimento de Convenção Coletiva

- **Attribute**: Usability (proactive notification)
- **Source/stimulus**: CCT do Sindicato dos Metalúrgicos (35.000 funcionários) vence em 15 dias e ainda não há renovação cadastrada
- **Environment**: Produção, dashboard do operador de RH
- **Artifact**: Frontend (NotificationService) + Job de verificação diário
- **Response**: Sistema exibe alerta no dashboard com 30, 15 e 5 dias de antecedência; lista funcionários impactados; ao vencer sem renovação, mantém última alíquota com flag "CCT vencida — aguardando renovação" e notifica por e-mail os gestores de RH
- **Response measure**: Alertas exibidos com 30 dias de antecedência; lista de impacto (funcionários × sindicato) disponível em < 3 segundos; zero cálculos com alíquota zerada por CCT vencida (mantém último valor com ressalva)
- **Priority**: Média

---

## Matriz de Prioridades

| ID | Cenário | Atributo | Prioridade |
|---|---|---|---|
| P4 | Recálculo retroativo S-2230 < 30s | Performance | **Crítica** |
| P5 | 11 tipos de cálculo em lote < 3h | Performance | **Crítica** |
| S4 | Bloqueio injeção fórmula NCalc 100% | Security | **Crítica** |
| S5 | Isolamento multi-tenant sindical 100% | Security | **Crítica** |
| S6 | Imutabilidade tabelas fiscais (4-eyes) | Security | **Crítica** |
| P6 | Cálculo férias complexas < 5s | Performance | Alta |
| A4 | Tolerância falha NCalc 99,9% | Availability | Alta |
| A5 | Reconciliação eventos perdidos < 24h | Availability | Alta |
| S7 | Anonimização relatórios LGPD 100% | Security | Alta |
| M4 | Alteração CCT retroativa < 15min | Maintainability | Alta |
| M5 | Nova rubrica legal < 8h (zero código) | Maintainability | Alta |
| M6 | Coexistência tabelas IRRF/INSS 100% | Maintainability | Alta |
| SC3 | Dissídio 100K func. < 4h | Scalability | Alta |
| U2 | Sandbox fórmulas < 500ms | Usability | Alta |
| P7 | Simulação rubrica 50K func. < 5min | Performance | Média |
| A6 | Fallback plano de saúde cached | Availability | Média |
| M7 | Reajuste multi-operadora configurável | Maintainability | Média |
| M8 | Alteração regra rescisão isolada | Maintainability | Média |
| SC4 | 45 sindicatos sem degradação | Scalability | Média |
| U3 | Alerta vencimento CCT 30 dias | Usability | Média |

---

## Evidence vs Assumptions

### Evidências (fundamentação legal e normativa)

| Evidência | Fonte | Cenários impactados |
|---|---|---|
| e-Social exige eventos retificadores (S-1200/S-1210) quando há recálculo por evento trabalhista superveniente | Manual de Orientação do e-Social v. S-1.3, Seção 5.4 | P4, A5 |
| CLT art. 611-A e 611-B definem prevalência do negociado sobre o legislado para contribuição sindical | CLT, Título VI | M4, U3 |
| IRRF — Tabela Progressiva Mensal é publicada anualmente pela RFB (IN RFB) | IN RFB vigente | M6, P6 |
| INSS — Teto e alíquotas são reajustados por Portaria Interministerial (MF/MPS) anualmente | Portaria Interministerial vigente | M6 |
| LGPD art. 18 — direito à anonimização/eliminação de dados pessoais; art. 7º, X — retenção para cumprimento de obrigação legal (fiscal: 5 anos) | Lei 13.709/2018 | S7 |
| FGTS — alíquota de 8% (geral) e 11,2% (menor aprendiz); Lei 8.036/90 | Lei 8.036/90, art. 15 | M5 |
| NCalc permite execução de funções arbitrárias se não configurado sandbox adequado | NCalc documentation — Security Considerations | S4 |
| Convenção Coletiva de Trabalho tem vigência máxima de 2 anos (CLT art. 614, §3º) | CLT art. 614, §3º | U3 |

### Premissas (assumptions)

| Premissa | Justificativa | Risco se inválida |
|---|---|---|
| 100.000 funcionários é o teto de escala nos próximos 5 anos | Projeção de mercado para médias/grandes empresas no Brasil | Cenários de escalabilidade (SC3, SC4) podem exigir redesign |
| 3 horas é o SLA aceitável para fechamento combinado (mensal + 13º + férias) | Baseado no benchmark de sistemas concorrentes (Sênior, ADP, TOTVS) | Insatisfação do cliente; necessidade de otimização de infra |
| NCalc é o motor de fórmulas escolhido e será mantido na stack | Decisão arquitetural já tomada no PRD F03 | Cenários S4 e A4 precisam de mitigação adicional |
| Schema-per-tenant com RLS é suficiente para isolamento multi-tenant | Padrão definido na F01 — Fundação & Infraestrutura | S5 precisaria de revisão se migrar para database-per-tenant |
| Operadoras de plano de saúde mantêm APIs com disponibilidade > 99% | SLA típico de mercado para operadoras de saúde | A6 exigiria fallback manual mais frequente |
| Equipe de 3-5 desenvolvedores por módulo com conhecimento em legislação trabalhista | Dimensionamento definido no PRD F02 | Prazos de manutenibilidade (M4-M8) podem ser impactados |

---

## Relação com o Artefato Principal

Os cenários deste documento **complementam** (não substituem) os cenários do artefato principal [`quality-attribute-scenarios.md`](./quality-attribute-scenarios.md). O mapeamento de dependências é:

| Cenário Principal | Cenário Complementar | Relação |
|---|---|---|
| P1 (Folha 100K < 2h) | P5 (11 tipos < 3h) | P5 estende P1 para múltiplos tipos simultâneos |
| M1 (IRRF < 4h) | M6 (Coexistência tabelas) | M6 detalha o versionamento de M1 |
| M3 (Customização rubrica) | M4 (Alteração CCT) | M4 é caso específico de M3 com retroatividade |
| A2 (e-Social indisponível) | A5 (Eventos perdidos) | A5 cobre falha interna (não externa como A2) |
| S1 (Acesso não autorizado) | S5 (Cross-tenant sindical) | S5 é especialização de S1 para domínio sindical |
| U1 (Dashboard progresso) | U2 (Sandbox fórmulas) | U2 é especialização de U1 para edição de rubricas |

---

## Recommended Next Skills

1. **`architecture-risk-assessor`** — Para classificar os riscos arquiteturais derivados destes cenários, especialmente:
   - Risco de injeção de fórmulas NCalc (S4) — impacto: corrupção de dados da folha
   - Risco de inconsistência Eventos → Cálculo (A5) — impacto: divergência fiscal
   - Risco de vazamento cross-tenant (S5) — impacto: LGPD + multa ANPD

2. **`scalability-hotspot-detector`** — Para identificar gargalos de escalabilidade nos cenários SC3 (dissídio 100K) e P5 (11 tipos simultâneos), validando se a arquitetura de particionamento por lote + RMQ suporta o throughput projetado.

3. **`tradeoff-analysis-writer`** — Para analisar tradeoffs como:
   - Performance (P5: 3h) vs Consistência (A5: reconciliação em 24h)
   - Segurança (S4: sandbox restrito) vs Flexibilidade (M3: fórmulas customizadas)
   - Disponibilidade (A6: fallback cached) vs Precisão (A6: divergência < 3%)

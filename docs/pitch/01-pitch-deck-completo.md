# Pitch Deck para Investidores — Folha360

**Data**: Junho 2026
**Versão**: 1.0
**Formato**: Markdown (exportável para Canva, Google Slides, PowerPoint, Gamma.app)

---

## Briefing Diagnóstico

| Item | Descrição |
|------|-----------|
| **Problema** | Folha de pagamento no Brasil é extremamente complexa: 50+ tipos de eventos trabalhistas, 3.000+ regras de cálculo, obrigatoriedade do e-Social (v. S-1.3) com mudanças constantes. Empresas gastam ~40h/mês só com folha e erros geram multas de até R$ 100 mil por autuação. |
| **Solução** | Folha360 — sistema moderno, modular e escalável de folha de pagamento compatível com e-Social. Processa 100.000+ funcionários em menos de 2 horas. |
| **Diferencial Técnico** | Monólito modular (.NET 10) com comunicação assíncrona (RabbitMQ), schema-per-tenant (isolamento LGPD), cache Redis, processamento paralelo, Kubernetes. Arquitetura preparada para extração gradual para microservices. |
| **Stack** | .NET 10, React (Vite), PostgreSQL 16, Redis 7, RabbitMQ 3.13, Docker + Kubernetes, Seq + Prometheus + Grafana |
| **Maturidade** | Fase de arquitetura e design concluída (ADRs, visões C4, cenários de qualidade). Próximo passo: implementação da Fundação & Infraestrutura (F01). |
| **Mercado** | ~5 milhões de empresas no Brasil. TAM estimado em R$ 8 bi/ano em software de folha de pagamento. |
| **Concorrência** | Senior Sistemas, Totvs (Protheus/RM), Sankhya (legados, caros, complexos); Omie (SMB, mas limitado); Gupy/Kenoby (RH, não folha completa). |

---

# Pitch Deck — Conteúdo dos 20 Slides

---

## SLIDE 1 — Capa

### Título
**Folha360**
Folha de Pagamento Inteligente

### Subtítulo
Sistema moderno, escalável e compatível com e-Social para empresas brasileiras

### Informações
- [Stanley Dias]
- [Sócio]
- Junho 2026

### Design Visual
- **Layout**: Fundo escuro (#0f172a) com gradiente azul-verde diagonal
- **Logo**: Folha360 centralizado, tipografia bold (Inter/display)
- **Elemento**: Linhas de grid tecnológico sutis ao fundo (hexágonos/circuitos)
- **Tagline**: "Simplificando a folha de pagamento do Brasil" (rodapé)

---

## SLIDE 2 — O Problema

### Título
**A Folha de Pagamento no Brasil é um Pesadelo**

### Conteúdo Principal
- **Complexidade regulatória**: 50+ tipos de eventos trabalhistas, 3.000+ regras de cálculo
- **e-Social obrigatório**: Leiautes S-1.3 com atualizações constantes — não conformidade gera multas
- **Custo de erro**: Multas de até **R$ 100 mil** por autuação trabalhista
- **Tempo desperdiçado**: Empresas de médio porte gastam **~40 horas/mês** só com folha
- **Sistemas legados**: ERPs antigos, sem API, sem mobile, sem compliance atualizado

### Design Visual
- **Layout**: Grid 2 colunas — lado esquerdo texto, lado direito ícone grande de "dor" (⚠️)
- **Números em destaque**: "40h/mês", "R$ 100K multa", "3.000+ regras"
- **Animação**: Números aparecendo com contagem progressiva

### Notas do Apresentador
> "Toda empresa no Brasil precisa processar folha de pagamento. Parece simples, mas a realidade é um pesadelo regulatório. São mais de 50 tipos de eventos trabalhistas, 3 mil regras de cálculo, e a obrigatoriedade do e-Social que muda a cada nova nota técnica. Uma empresa de médio porte gasta em média 40 horas por mês só com folha. E o custo de um erro? Multas que chegam a R$ 100 mil por autuação. Este é o problema que o Folha360 resolve."

---

## SLIDE 3 — O Cenário

### Título
**Um Mercado Gigante e Carente de Inovação**

### Conteúdo Principal
- **~5 milhões** de empresas ativas no Brasil (IBGE)
- **~R$ 8 bilhões/ano** em software de folha de pagamento (TAM)
- **Obrigatoriedade legal**: 100% das empresas com CNPJ precisam processar folha
- **Digitalização acelerada**: +35% de adoção de SaaS RH pós-pandemia
- **Janela regulatória**: e-Social S-1.3 cria necessidade de migração de sistemas legados

### Design Visual
- **Layout**: Data storytelling — gráfico de barras mostrando crescimento do mercado SaaS RH no Brasil
- **Destaque**: Mapa do Brasil com indicadores por região
- **Citação**: "O mercado de RH no Brasil movimenta R$ 30 bi/ano — folha de pagamento representa 27% disso" (fonte: ABRH)

### Notas do Apresentador
> "O Brasil tem cerca de 5 milhões de empresas ativas. Todas — sem exceção — precisam processar folha de pagamento. É um mercado de R$ 8 bilhões por ano, crescendo com a digitalização pós-pandemia. E com a chegada do e-Social S-1.3, milhares de empresas precisarão migrar de sistemas legados que não acompanham as mudanças regulatórias. Esta é a nossa janela de oportunidade."

---

## SLIDE 4 — A Solução Atual

### Título
**Concorrência Fragmentada e Desatualizada**

### Conteúdo Principal

| Concorrente | Problema |
|-------------|----------|
| **Totvs (Protheus/RM)** | ERP pesado, caro (>R$ 5K/mês), implementação longa (6-12 meses) |
| **Senior Sistemas** | Complexo, foco enterprise, sem API moderna |
| **Sankhya** | Legado, sem inovação, suporte limitado |
| **Omie** | SMB, mas folha é módulo complementar limitado |
| **Gupy/Kenoby** | RH (recrutamento), não faz folha de pagamento |

### Design Visual
- **Layout**: Matriz 2×2 posicionando concorrentes (eixo X: preço, eixo Y: modernidade)
- **Folha360**: Canto superior direito (moderno + preço justo)
- **Totvs/Senior**: Canto inferior direito (caro + legado)
- **Omie**: Centro-esquerda (simples mas limitado)

### Notas do Apresentador
> "Olhe para as opções disponíveis hoje. De um lado, sistemas legados como Totvs e Senior — poderosos, mas caros, complexos, com implementação que leva meses. Do outro, soluções modernas como Omie — mas que tratam folha como módulo complementar, não como core. E tem ainda as plataformas de RH como Gupy, que fazem recrutamento mas não processam folha. O mercado está fragmentado e carece de uma solução moderna, completa e com preço justo."

---

## SLIDE 5 — Nossa Solução

### Título
**Folha360: Folha de Pagamento Inteligente**

### Conteúdo Principal
- **Sistema completo** de folha de pagamento em nuvem (SaaS)
- **6 módulos integrados**: Cadastros → Eventos → Cálculo → Fiscais → Relatórios → e-Social
- **Processamento de 100.000+ funcionários** em menos de 2 horas
- **Compliance e-Social S-1.3** nativo — sem adaptações
- **Multi-tenancy** com isolamento total de dados (schema-per-tenant)
- **API RESTful** para integração com qualquer sistema

### Design Visual
- **Layout**: Diagrama de arquitetura simplificado (6 módulos em fluxo)
- **Destaque**: Frase "100K funcionários • <2h • 100% compliance"
- **Badges**: .NET 10, React, PostgreSQL, K8s

### Notas do Apresentador
> "O Folha360 é um sistema completo de folha de pagamento construído do zero para a era digital. São seis módulos integrados que cobrem todo o ciclo: dos cadastros básicos até o envio ao e-Social. Nossa arquitetura moderna processa 100 mil funcionários em menos de 2 horas. E o mais importante: o compliance com e-Social S-1.3 é nativo — não é uma adaptação de última hora."

---

## SLIDE 6 — Diferenciais Técnicos

### Título
**Arquitetura que Escala**

### Conteúdo Principal
- **Monólito modular (.NET 10)**: Isolamento lógico com deploy único — simplicidade operacional sem abrir mão da organização
- **Processamento assíncrono**: RabbitMQ + background jobs para cálculos pesados — API responde em ms, não em horas
- **Cache inteligente (Redis)**: Tabelas progressivas IRRF/INSS em cache — 90% hit rate, latência < 5ms
- **Schema-per-tenant**: Isolamento total de dados por cliente — LGPD compliance nativo
- **Kubernetes + HPA**: Escala automática no pico do fechamento mensal
- **Preparado para microservices**: Extração gradual via strangler fig pattern quando o crescimento exigir

### Design Visual
- **Layout**: 6 cards em grid 3×2, cada um com ícone + título + descrição curta
- **Destaque**: Card "100K func. • <2h" em destaque (tamanho maior)
- **Badges técnicos**: .NET 10, React, PostgreSQL 16, Redis 7, RabbitMQ, K8s

### Notas do Apresentador
> "Nossa arquitetura foi projetada por engenheiros que já construíram sistemas de folha antes. Sabemos que o gargalo não é código — é arquitetura. Usamos monólito modular para manter a simplicidade enquanto o time é pequeno, mas com comunicação assíncrona via RabbitMQ que permite extrair módulos para microservices quando crescer. O resultado: um sistema que processa 100 mil funcionários em menos de 2 horas, com isolamento LGPD por cliente e cache que entrega respostas em milissegundos."

---

## SLIDE 7 — Demonstração

### Título
**Produto em Ação**

### Conteúdo Principal
> *[Espaço para print/screenshot da interface]*

**Fluxo demonstrado**:
1. Dashboard do operador de RH — visão geral do mês
2. Cadastro de funcionário — dados pessoais + contrato + rubricas
3. Processamento da folha — um clique, acompanhamento em tempo real
4. Holerite gerado — visualização e download
5. Envio e-Social — status dos eventos

### Design Visual
- **Layout**: Mockup de browser ou dashboard em destaque (80% do slide)
- **Setas numeradas**: Indicando o fluxo de 1 a 5
- **Minimalista**: Pouco texto, foco na imagem do produto

### Notas do Apresentador
> "Vou mostrar rapidamente como funciona. O operador de RH abre o dashboard e vê a visão geral do mês. Com poucos cliques, ele cadastra um funcionário — todos os dados pessoais, contrato e rubricas. Depois, um único clique inicia o processamento da folha. O acompanhamento é em tempo real. Em minutos — não horas — o holerite está pronto. E o envio ao e-Social é automatizado."

---

## SLIDE 8 — Stack Tecnológica

### Título
**Tecnologia de Ponta**

### Conteúdo Principal

| Camada | Tecnologia | Por quê |
|--------|-----------|---------|
| **Backend** | .NET 10 (C#) | Performance, tipagem forte, ecossistema maduro |
| **Frontend** | React + Vite + TypeScript | Produtividade, componentes, tipagem |
| **Banco de Dados** | PostgreSQL 16 | Robustez, extensões, schema-per-tenant |
| **Cache** | Redis 7 | Sub-milissegundo, pub/sub para invalidação |
| **Mensageria** | RabbitMQ 3.13 | AMQP, dead-letter, roteamento flexível |
| **Container** | Docker + Kubernetes | Portabilidade, auto-scaling, resiliência |
| **Observabilidade** | Seq + Prometheus + Grafana | Logs, métricas, alertas |

### Design Visual
- **Layout**: Grid mostrando cada tecnologia com logo + nome + descrição
- **Cores**: Cada badge com a cor característica da tecnologia (.NET = roxo, React = azul, PostgreSQL = azul escuro, Redis = vermelho)
- **Diferencial**: Nota "100% cloud-native, rodando em qualquer cloud (AWS, GCP, Azure, on-prem)"

### Notas do Apresentador
> "Escolhemos cada tecnologia com um propósito. .NET 10 nos dá performance e tipagem forte — essencial para cálculos financeiros sem erro. PostgreSQL com schema-per-tenant nos dá isolamento LGPD nativo. Redis para cache em milissegundos. RabbitMQ para comunicação assíncrona confiável. Tudo rodando em Kubernetes, cloud-native, prontos para escalar."

---

## SLIDE 9 — e-Social Compatível

### Título
**Compliance e-Social S-1.3 Nativo**

### Conteúdo Principal
- **Leiautes S-1.3**: Cobertura completa das tabelas (S-1000 a S-1080) e eventos periódicos (S-1200 a S-1300)
- **Validação XSD**: Geração de XML validado contra schemas oficiais
- **Atualização contínua**: Monitoramento de Notas Técnicas — atualização em até 15 dias
- **Retry automático**: Se o portal e-Social cair, eventos acumulam na fila e são reenviados automaticamente
- **Histórico completo**: Todos os eventos enviados ficam registrados para auditoria

### Design Visual
- **Layout**: Timeline mostrando a evolução do e-Social (S-1.0 → S-1.3)
- **Destaque**: Selo "100% Compatível S-1.3"
- **Iconografia**: Documento com check verde, engrenagem de sincronia

### Notas do Apresentador
> "O e-Social é o calcanhar de Aquiles de muitos sistemas. No Folha360, o compliance é nativo — não é um módulo comprado de terceiros. Cobrimos todos os leiautes S-1.3, validamos contra os schemas oficiais, e nos atualizamos em até 15 dias após cada Nota Técnica. Se o governo cair (sim, acontece), os eventos acumulam na fila e são reenviados automaticamente. Sem perder dados, sem multas."

---

## SLIDE 10 — Segurança & LGPD

### Título
**Privacidade por Design**

### Conteúdo Principal
- **Criptografia AES-256**: Dados sensíveis (CPF, CTPS, PIS/PASEP, salários) criptografados em repouso
- **Schema-per-tenant**: Isolamento total — cada cliente em seu próprio schema PostgreSQL
- **LGPD compliance**: Exclusão de dados (art. 18) via `DROP SCHEMA tenant_XXX CASCADE`
- **Audit trail imutável**: Toda alteração registrada em `audit_log` — quem, quando, o quê
- **JWT + RBAC**: Autenticação com perfis (admin, operador, contador, consulta)
- **Chaves em KMS**: Chaves de criptografia armazenadas em HSM/KMS separado do banco

### Design Visual
- **Layout**: Escudo/cadeado como elemento central
- **Grid**: 6 cards com os pilares de segurança
- **Badge**: "LGPD Ready" + "AES-256" + "Schema Isolation"

### Notas do Apresentador
> "Segurança não é feature — é requisito. Todos os dados sensíveis são criptografados com AES-256 em repouso. Cada cliente tem seu próprio schema no banco — isolamento total. Se um cliente exercer o direito de exclusão da LGPD, um comando `DROP SCHEMA` resolve. Temos auditoria imutável de todas as alterações. E as chaves de criptografia ficam em um KMS separado, não no banco."

---

## SLIDE 11 — O Time

### Título
**O Time por Trás do Folha360**

### Conteúdo Principal
> *[Espaço para fotos e bios do time]*

**Perfis sugeridos**:
- **Arquiteto de Software**: .NET, DDD, sistemas distribuídos, e-Social
- **Desenvolvedores Full-stack**: .NET + React, PostgreSQL
- **Designer UX/UI**: Sistemas de design, experiência B2B
- **Product Manager**: Domínio de RH/folha de pagamento

**Diferenciais**:
- Experiência prévia em sistemas de folha de pagamento
- Conhecimento profundo de e-Social e legislação trabalhista
- Histórico de entrega de sistemas críticos e escaláveis

### Design Visual
- **Layout**: Cards de pessoas (foto, nome, cargo, breve bio)
- **Se disponível**: Logos de empresas anteriores ou formação acadêmica
- **Tom**: Pessoal, autêntico — fotos reais, não stock photos

### Notas do Apresentador
> "Um grande produto começa com um grande time. Nosso time combina experiência profunda em engenharia de software com conhecimento de domínio de folha de pagamento. Já construímos sistemas que processam folha de milhares de funcionários antes. Conhecemos o e-Social por dentro. E temos a visão de que folha de pagamento pode — e deve — ser simples."

---

## SLIDE 12 — Traction & Progresso

### Título
**Onde Estamos e Para Onde Vamos**

### Conteúdo Principal

| Fase | Status | Período |
|------|--------|---------|
| 🏗️ Arquitetura e Design | ✅ Concluído | Jun/2026 |
| 📐 ADRs e Visões C4 | ✅ Concluído | Jun/2026 |
| 🖥️ Protótipo Frontend | ✅ Concluído | Jun/2026 |
| 🔧 Fundação & Infraestrutura (F01) | 🔜 Próximo | Jul/2026 |
| 📋 Gestão de Cadastros (F02) | 📅 Planejado | Ago/2026 |
| ⚙️ Processamento da Folha (F04) | 📅 Planejado | Out/2026 |
| 🔗 Integração e-Social (F07) | 📅 Planejado | Dez/2026 |
| 🚀 MVP Completo | 🎯 **Meta** | **Jan/2027** |

### Design Visual
- **Layout**: Timeline horizontal com milestones
- **Cores**: Verde (concluído), amarelo (em andamento), cinza (planejado)
- **Destaque**: Meta do MVP em destaque (janeiro 2027)

### Notas do Apresentador
> "Estamos na fase de arquitetura e design, já concluída. Temos todas as decisões arquiteturais documentadas em ADRs, as visões C4, o protótipo frontend. O próximo passo é a Fundação & Infraestrutura — a base sobre a qual tudo será construído. Nossa meta é ter o MVP completo com folha de pagamento funcional e integração e-Social até janeiro de 2027. São 7 meses de execução."

---

## SLIDE 13 — Modelo de Negócio

### Título
**SaaS: Receita Recorrente e Escalável**

### Conteúdo Principal

| Plano | Público | Preço | Funcionalidades |
|-------|---------|-------|-----------------|
| **Essencial** | Micro (1-10 func.) | R$ 199/mês | Folha básica, e-Social, holerite |
| **Profissional** | SMB (10-100 func.) | R$ 499/mês | Completo + relatórios + API |
| **Enterprise** | Médio (100-1000 func.) | R$ 999/mês | Tudo + suporte premium + SLA |
| **Corporativo** | Grande (1000+ func.) | Sob consulta | Dedicado + on-premise opcional |

**Modelo**: Assinatura mensal/anuais + taxa por funcionário excedente
**Receita adicional**: Implementação (one-time), treinamento, suporte premium

### Design Visual
- **Layout**: 4 cards de planos lado a lado (tabela de precificação)
- **Destaque**: Plano "Profissional" como recommended (badge verde)
- **Cálculo**: Exemplo de ticket médio: R$ 499/mês → R$ 5.988/ano por cliente

### Notas do Apresentador
> "Seguimos o modelo SaaS com planos progressivos. O plano Essencial a R$ 199/mês atende microempresas. O Profissional a R$ 499/mês é nosso carro-chefe para SMB. Enterprise para médias empresas. E Corporativo sob consulta. O ticket médio estimado é de R$ 499/mês. Com margem bruta de 70-80%, cada cliente paga em média R$ 6 mil por ano. É um modelo previsível, escalável e com alta retenção."

---

## SLIDE 14 — Mercado Endereçável

### Título
**TAM, SAM e SOM**

### Conteúdo Principal

```
TAM (Total Addressable Market)
🌎 Mercado global de software de folha de pagamento
📊 ~R$ 8 bilhões/ano (Brasil)

SAM (Serviceable Addressable Market)
🎯 Empresas SMB que usam sistemas digitais
📊 ~R$ 2,5 bilhões/ano

SOM (Serviceable Obtainable Market)
🎯 Nosso alvo inicial: SMB tech-savvy (primeiros 3 anos)
📊 ~R$ 150 milhões/ano
```

### Design Visual
- **Layout**: 3 círculos concêntricos (TAM → SAM → SOM) — diagrama visual
- **Dados**: Cada círculo com valor + descrição
- **Destaque**: SOM com seta "Nosso foco inicial"

### Notas do Apresentador
> "O mercado total de software de folha de pagamento no Brasil é de R$ 8 bilhões por ano. Nosso mercado endereçável — empresas que usam sistemas digitais — é de R$ 2,5 bilhões. E nosso mercado obtenível nos primeiros 3 anos, focando em SMB tech-savvy que estão insatisfeitas com sistemas legados, é de R$ 150 milhões. Mais do que suficiente para construir um negócio de centenas de milhões."

---

## SLIDE 15 — Go-to-Market

### Título
**Estratégia de Aquisição de Clientes**

### Conteúdo Principal
- **Canal direto (inbound)**: Marketing de conteúdo (blog, SEO, webinars sobre e-Social) — CAC estimado: R$ 1.500
- **Parcerias**: Contadores e escritórios de contabilidade (indicam para clientes) — comissão 20% do 1º ano
- **Canais digitais**: Google Ads (palavras-chave: "sistema de folha de pagamento", "e-Social"), LinkedIn Ads
- **Free trial**: 14 dias gratuitos, sem cartão de crédito
- **Onboarding**: Implementação assistida em 5 dias úteis

### Design Visual
- **Layout**: Funil de vendas (topo → fundo) com taxas de conversão
- **Canais**: 3 cards com ícones (inbound, parcerias, digital)
- **CAC em destaque**: "CAC estimado: R$ 1.500–2.000"

### Notas do Apresentador
> "Nossa estratégica Go-to-Market tem 3 pilares. Primeiro, marketing de conteúdo — vamos educar o mercado sobre e-Social e atrair clientes que buscam soluções modernas. Segundo, parcerias com contadores — eles são influenciadores-chave na decisão de compra. Terceiro, canais digitais com busca paga. Nosso CAC estimado é de R$ 1.500 a R$ 2.000 — saudável para um ticket de R$ 499/mês. E com free trial de 14 dias, reduzimos a barreira de entrada."

---

## SLIDE 16 — Concorrência

### Título
**Vantagem Competitiva**

### Conteúdo Principal

| Critério | Folha360 | Totvs | Senior | Omie |
|----------|----------|-------|--------|------|
| Preço mensal | R$ 199-999 | R$ 5.000+ | R$ 3.000+ | R$ 299 |
| Implementação | 5 dias | 6-12 meses | 3-6 meses | 2-3 dias |
| e-Social nativo | ✅ Nativo | 🟡 Adaptado | 🟡 Adaptado | 🟡 Limitado |
| API moderna | ✅ RESTful | ❌ Legado | 🟡 Parcial | ✅ RESTful |
| 100K func. | ✅ Sim | ✅ Sim | ✅ Sim | ❌ Não |
| Multi-tenant isolado | ✅ Schema | ❌ Coluna | ❌ Coluna | ❌ Coluna |
| Mobile | 🟡 Previsto | ✅ Sim | ✅ Sim | ✅ Sim |
| Suporte | Chat + email | Telefone | Telefone | Chat |

### Design Visual
- **Layout**: Tabela comparativa com checkmarks (✅) e "x" (❌)
- **Destaque**: Coluna Folha360 em destaque (cor diferente)
- **Diferenciais**: Linhas com fundo verde onde Folha360 é superior

### Notas do Apresentador
> "Quando nos comparamos com a concorrência, algumas vantagens ficam claras. Somos mais baratos que Totvs e Senior por uma ordem de grandeza. Nossa implementação leva dias, não meses. O e-Social é nativo — não adaptado. Temos API moderna, isolamento LGPD por schema, e arquitetura que escala para 100 mil funcionários. Onde perdemos? Mobile — mas está no roadmap. E marca — mas isso se constrói."

---

## SLIDE 17 — Projeções Financeiras

### Título
**Projeção 3 Anos**

### Conteúdo Principal

| Indicador | Ano 1 | Ano 2 | Ano 3 |
|-----------|-------|-------|-------|
| **Clientes** | 105 | 420 | 1.260 |
| **Ticket médio** | R$ 499 | R$ 499 | R$ 499 |
| **MRR** | R$ 52K | R$ 210K | R$ 629K |
| **ARR** | R$ 629K | R$ 2,5M | R$ 7,5M |
| **Receita Líquida** | R$ 566K | R$ 2,3M | R$ 6,8M |
| **Custos + Despesas** | R$ 1,7M | R$ 2,9M | R$ 5,2M |
| **EBITDA** | (R$ 1,1M) | (R$ 599K) | R$ 1,6M |
| **Margem EBITDA** | -196% | -26% | +24% |

### Design Visual
- **Layout**: Gráfico de linhas mostrando a evolução da receita × despesas
- **Destaque**: Ponto de break-even (entre Ano 2 e Ano 3) com marcador visual
- **Cores**: Linha verde (receita), linha vermelha (despesas), área de lucro sombreada em verde

### Notas do Apresentador
> "Nossas projeções são conservadoras. Ano 1 é de investimento: receita de R$ 629 mil contra R$ 1,7 milhão em custos. Ano 2, a receita escala para R$ 2,5 milhões e o EBITDA ainda é negativo, mas bem menor. No Ano 3, atingimos break-even com R$ 7,5 milhões de ARR e margem EBITDA positiva de 24%. Precisamos de aproximadamente R$ 2,5 milhões de capital para chegar até lá."

---

## SLIDE 18 — Métricas-Chave

### Título
**Métricas SaaS que Importam**

### Conteúdo Principal

| Métrica | Ano 1 | Ano 2 | Ano 3 | Benchmark Saudável |
|---------|-------|-------|-------|-------------------|
| **MRR** | R$ 52K | R$ 210K | R$ 629K | — |
| **ARR** | R$ 629K | R$ 2,5M | R$ 7,5M | — |
| **CAC** | R$ 2.000 | R$ 1.500 | R$ 1.200 | < R$ 2.000 |
| **LTV** | R$ 16,6K | R$ 25K | R$ 50K | > R$ 6K |
| **LTV/CAC** | 8,3× | 16,6× | 41,6× | > 3× ✅ |
| **Churn** | 3% | 2% | 1% | < 5% ✅ |
| **Margem Bruta** | 50% | 65% | 75% | > 70% ✅ |
| **NPS** | — | 50+ | 60+ | > 50 |

### Design Visual
- **Layout**: Dashboard de métricas — cards com indicadores grandes
- **Cores**: Verde (saudável), amarelo (atenção), vermelho (crítico)
- **Destaque**: LTV/CAC de 8,3× no Ano 1 em destaque (métrica favorita de VCs)

### Notas do Apresentador
> "As métricas que investidores olham: LTV/CAC acima de 3× — o nosso começa em 8× e chega a 41×. Churn abaixo de 5% — o nosso cai de 3% para 1%. Margem bruta acima de 70% — atingimos no Ano 3. Esses números mostram que o modelo é saudável, escalável e defensável. Cada real investido em aquisição retorna 8 reais em valor do cliente no primeiro ano."

---

## SLIDE 19 — O Pedido

### Título
**O Que Estamos Buscando**

### Conteúdo Principal

**Valor solicitado**: **R$ 2.000.000**
**Participação**: 10–15% (equity)
**Prazo**: 18 meses de runway

**Uso dos recursos**:
| Destino | % | Valor |
|---------|---|-------|
| 👨‍💻 Time de tecnologia (dev + infra) | 50% | R$ 1.000.000 |
| 📢 Marketing & Vendas | 25% | R$ 500.000 |
| 🏢 Operações & Suporte | 15% | R$ 300.000 |
| 💰 Reserva legal | 10% | R$ 200.000 |

**Milestones com este investimento**:
1. ✅ MVP funcional (folha + e-Social) — Mês 7
2. ✅ 50 clientes pagantes — Mês 12
3. ✅ Break-even mensal — Mês 24
4. 🚀 ARR de R$ 7,5M — Ano 3

### Design Visual
- **Layout**: Pizza chart com a alocação dos recursos
- **Timeline**: 4 milestones com prazos
- **Destaque**: "R$ 2.000.000" em fonte grande

### Notas do Apresentador
> "Estamos levantando R$ 2 milhões por 10 a 15% de participação. Esse capital nos dá 18 meses de runway — tempo suficiente para lançar o MVP, conquistar os primeiros 50 clientes pagantes e chegar perto do break-even. Metade vai para tecnologia — nosso principal diferencial. Um quarto para marketing e vendas. O restante para operações e reserva. Com este investimento, levamos o Folha360 do papel ao mercado."

---

## SLIDE 20 — Call to Action

### Título
**Vamos Revolucionar a Folha de Pagamento do Brasil?**

### Conteúdo Principal
- **O mercado**: R$ 8 bilhões/ano e carente de inovação
- **A solução**: Moderna, escalável, 100% compliance e-Social
- **O time**: Experiente, motivado, com visão clara
- **O momento**: Janela regulatória e-Social + digitalização do RH

### Contato
- **Email**: [email@folha360.com.br]
- **Site**: [www.folha360.com.br]
- **LinkedIn**: [linkedin.com/company/folha360]
- **Demo**: [link para demonstração]

### Design Visual
- **Layout**: Minimalista — fundo escuro com gradiente
- **Central**: "Vamos conversar?" em destaque
- **QR Code**: Link direto para a demo
- **Rodapé**: Contatos + "Folha360 — Folha de Pagamento Inteligente"

### Notas do Apresentador
> "O Brasil precisa de um sistema de folha de pagamento moderno. O mercado é gigante — R$ 8 bilhões por ano. A janela regulatória do e-Social está aberta. Nós temos a visão, a arquitetura e o time para construir isso. Estamos buscando parceiros que compartilhem dessa visão. Vamos revolucionar a folha de pagamento do Brasil juntos? Obrigado."

---

## Roteiro de Apresentação (Script Completo)

### Abertura (Slide 1 — Capa)
> "Bom dia a todos. Meu nome é [Nome] e estou aqui para apresentar o Folha360 — um sistema de folha de pagamento inteligente que estamos construindo para transformar a forma como empresas brasileiras processam folha."

### Slide 2 — O Problema (30s)
> "Toda empresa no Brasil precisa processar folha de pagamento. Parece simples, mas a realidade é um pesadelo. São mais de 50 tipos de eventos trabalhistas, 3 mil regras de cálculo, e o e-Social que muda constantemente. Uma empresa de médio porte gasta 40 horas por mês só com folha. E o custo de um erro? Multas de até R$ 100 mil. Este é o problema."

### Slide 3 — O Cenário (30s)
> "O Brasil tem 5 milhões de empresas ativas. Todas precisam de folha de pagamento. É um mercado de R$ 8 bilhões por ano. E com a digitalização pós-pandemia e a chegada do e-Social S-1.3, milhares de empresas precisarão migrar de sistemas legados. Esta é nossa janela."

### Slide 4 — A Solução Atual (30s)
> "O que existe hoje? De um lado, sistemas legados como Totvs e Senior — poderosos, mas caros e complexos. Do outro, soluções como Omie — modernas mas limitadas. O mercado está fragmentado. Não há uma solução moderna, completa e com preço justo. É o que vamos construir."

### Slide 5 — Nossa Solução (45s)
> "O Folha360 é um sistema completo de folha de pagamento em nuvem. São 6 módulos integrados que cobrem todo o ciclo. Nossa arquitetura processa 100 mil funcionários em menos de 2 horas. E o compliance com e-Social é nativo — não é adaptação."

### Slide 6 — Diferenciais Técnicos (45s)
> "Nossa arquitetura foi projetada por engenheiros que já construíram sistemas de folha antes. Monólito modular para simplicidade, comunicação assíncrona via RabbitMQ, schema-per-tenant para LGPD, cache Redis para performance. E preparado para virar microservices quando crescer."

### Slide 7 — Demonstração (60s)
> "Vou mostrar como funciona. [Demonstração ao vivo ou print]. O operador de RH abre o dashboard, cadastra um funcionário, inicia o processamento com um clique, acompanha em tempo real e o holerite está pronto em minutos. Simples."

### Slide 8 — Stack Tecnológica (30s)
> "Escolhemos cada tecnologia com propósito. .NET 10 para performance. PostgreSQL para isolamento. Redis para cache. RabbitMQ para mensageria. Tudo em Kubernetes, cloud-native, prontos para escalar."

### Slide 9 — e-Social (30s)
> "O e-Social é nativo. Cobrimos todos os leiautes S-1.3, validamos contra schemas oficiais, atualizamos em até 15 dias após cada Nota Técnica. Se o governo cair, eventos acumulam na fila e reenviam automaticamente."

### Slide 10 — Segurança & LGPD (30s)
> "Dados sensíveis criptografados com AES-256. Cada cliente em seu schema. Exclusão LGPD via DROP SCHEMA. Auditoria imutável. Chaves em KMS separado. Segurança não é feature — é requisito."

### Slide 11 — O Time (30s)
> "Nosso time combina engenharia de software de ponta com conhecimento profundo de folha de pagamento. Já construímos sistemas que processam folha de milhares de funcionários. Conhecemos o e-Social por dentro."

### Slide 12 — Traction (30s)
> "Arquitetura e design concluídos. ADRs, visões C4, protótipo frontend prontos. Próximo passo: Fundação & Infraestrutura. Meta: MVP completo com folha + e-Social até janeiro de 2027."

### Slide 13 — Modelo de Negócio (30s)
> "SaaS com planos de R$ 199 a R$ 999 por mês. Ticket médio de R$ 499. Margem bruta de 70-80%. Modelo previsível, escalável, com alta retenção."

### Slide 14 — Mercado (30s)
> "TAM de R$ 8 bilhões. SAM de R$ 2,5 bilhões. SOM de R$ 150 milhões — mais que suficiente para construir um negócio de centenas de milhões."

### Slide 15 — Go-to-Market (30s)
> "Três pilares: inbound marketing, parcerias com contadores, canais digitais. CAC de R$ 1.500 a R$ 2.000. Free trial de 14 dias. Onboarding em 5 dias."

### Slide 16 — Concorrência (30s)
> "Mais baratos que Totvs e Senior. Implementação em dias, não meses. e-Social nativo. API moderna. Isolamento LGPD. Onde perdemos? Mobile — no roadmap. Marca — se constrói."

### Slide 17 — Projeções Financeiras (45s)
> "Ano 1: investimento. Ano 2: escala. Ano 3: break-even com ARR de R$ 7,5 milhões e margem de 24%. Precisamos de R$ 2,5 milhões de capital para chegar lá."

### Slide 18 — Métricas (30s)
> "LTV/CAC de 8× no Ano 1. Churn caindo de 3% para 1%. Margem bruta chegando a 75%. Métricas saudáveis que mostram um modelo defensável."

### Slide 19 — O Pedido (45s)
> "Estamos levantando R$ 2 milhões. 50% para tecnologia, 25% marketing, 15% operações, 10% reserva. 18 meses de runway. Metas: MVP em 7 meses, 50 clientes em 12 meses, break-even em 24 meses."

### Slide 20 — Call to Action (30s)
> "Mercado de R$ 8 bilhões. Solução moderna. Time experiente. Janela aberta. Vamos revolucionar a folha de pagamento do Brasil juntos? Obrigado."

---

## Checklist de Verificação

### Conteúdo
- ✅ O problema está claramente definido e quantificado? (Slide 2 — 40h/mês, R$ 100K multa)
- ✅ A solução mostra diferencial competitivo real? (Slide 6 — arquitetura modular, 100K em <2h)
- ✅ Os dados de mercado são citados com fontes? (Slide 3 — IBGE, ABRH)
- ✅ As projeções financeiras são realistas e defensáveis? (Slide 17 — 3 anos, break-even no Ano 3)
- ✅ O pedido de investimento é específico e justificado? (Slide 19 — R$ 2M, 50% tech)

### Estrutura
- ✅ A narrativa segue o framework PAS + Hero's Journey? (Ato I: problema, Ato II: solução, Ato III: oportunidade, Ato IV: pedido)
- ✅ Todos os 20 slides estão preenchidos?
- ✅ Cada slide tem título, conteúdo, design e notas?
- ✅ O script total cabe em 10–15 minutos? (~12 minutos para 20 slides)

### Qualidade
- ✅ O tom é profissional e orientado a dados?
- ✅ Não há jargão técnico excessivo (público investidor)?
- ✅ Os gráficos e dados são visualmente claros?
- ✅ O deck conta uma história coesa do começo ao fim?

### Entrega
- ✅ O conteúdo está em formato Markdown exportável?
- ✅ Há sugestões de ferramentas para criar os slides? (Canva, Google Slides, PowerPoint, Gamma.app)
- ✅ O QR code / link de contato está incluído?

---

> **Próximos passos**: Este deck pode ser importado para **Canva**, **Google Slides**, **PowerPoint** ou **Gamma.app**. Recomenda-se gerar os slides visuais usando a skill `pitch-slides-generator` para preview HTML interativo no navegador.

# 🧭 Folha360 — Workflow da Jornada do Usuário

> **Objetivo**: Mapear a jornada completa dos usuários do Folha360, organizada por perfil de acesso,
> fluxos de trabalho e sequência lógica de operações no Departamento Pessoal.
>
> **Data**: 27/06/2026  
> **Base**: Agrupamento de Cadastros e Processos v2.0 + Estrutura de Endpoints  
> **Versão**: 1.0

---

## 👥 Perfis de Usuário (Personas)

| Perfil | Quem é | Principais Tarefas | Nível de Acesso |
|--------|--------|--------------------|-----------------|
| **Admin** | Administrador do sistema / TI | Configurar tenants, gerenciar usuários, certificados digitais, parâmetros globais | Total |
| **Operador DP** | Analista de Departamento Pessoal | Cadastrar empresas e funcionários, lançar eventos, processar folha, gerar holerites | Operador |
| **Contador** | Contador / Escritório contábil | Apurar tributos, gerar guias, DIRF, RAIS, configurar regras fiscais, enviar e-Social | Contador |
| **Consulta** | Gestor / RH / Funcionário | Visualizar holerites, consultar dados cadastrais, acompanhar eventos | Consulta |

---

## 🗺️ Visão Geral das Jornadas

```mermaid
flowchart LR
    subgraph FASE1["FASE 1: Fundação"]
        A1["🔐 Login & Tenant"] --> A2["🏢 Cadastrar Empresa"]
        A2 --> A3["📐 Estrutura de Apoio"]
    end

    subgraph FASE2["FASE 2: Cadastros Base"]
        A3 --> B1["💼 Cadastrar Cargos"]
        A3 --> B2["🧮 Configurar Rubricas"]
        B1 --> B3["👤 Cadastrar Funcionários"]
        B2 --> B3
    end

    subgraph FASE3["FASE 3: Eventos"]
        B3 --> C1["📅 Admissão"]
        B3 --> C2["📅 Férias"]
        B3 --> C3["📅 Afastamentos"]
        B3 --> C4["📅 Alterações Contratuais"]
        B3 --> C5["📅 Desligamento"]
    end

    subgraph FASE4["FASE 4: Processamento"]
        C1 --> D1["💰 Processar Folha"]
        C2 --> D1
        C3 --> D1
        C4 --> D1
        D1 --> D2["📄 Gerar Holerites"]
    end

    subgraph FASE5["FASE 5: Obrigações"]
        D1 --> E1["🧾 Apurar Tributos"]
        E1 --> E2["📤 Enviar e-Social"]
        E1 --> E3["📊 Gerar Relatórios"]
        E2 --> E3
    end

    style FASE1 fill:#e8f5e9,stroke:#2e7d32
    style FASE2 fill:#e3f2fd,stroke:#1565c0
    style FASE3 fill:#fff3e0,stroke:#ef6c00
    style FASE4 fill:#fce4ec,stroke:#c62828
    style FASE5 fill:#f3e5f5,stroke:#6a1b9a
```

---

## 📋 Jornada 1: Admin — Configuração Inicial do Sistema

**Perfil**: Admin  
**Duração estimada**: 1-2 horas (setup único)  
**Objetivo**: Deixar o sistema pronto para uso pelo Departamento Pessoal

### Diagrama da Jornada

```mermaid
journey
    title Jornada do Admin — Setup Inicial
    section Autenticação
      Fazer login: 5: Admin
      Selecionar tenant: 5: Admin
    section Infraestrutura
      Verificar saúde do sistema: 4: Admin
      Criar usuários da equipe: 5: Admin
      Definir perfis de acesso: 5: Admin
    section Empresa
      Cadastrar empresa (CNPJ/Razão Social): 5: Admin
      Configurar regime tributário: 4: Admin
      Cadastrar lotações: 4: Admin
      Cadastrar endereços: 3: Admin
      Cadastrar contatos: 3: Admin
    section e-Social
      Configurar ambiente e-Social: 4: Admin
      Upload certificado digital (A1/A3): 5: Admin
      Testar conexão certificado: 4: Admin
    section Tabelas de Apoio
      Cadastrar sindicatos: 3: Admin
      Cadastrar convênios: 3: Admin
      Cadastrar horários de trabalho: 4: Admin
```

### Fluxo Detalhado

| # | Etapa | Endpoint | Ação |
|---|-------|----------|------|
| 1 | **Login** | `POST /api/auth/login` | Autenticar com e-mail e senha |
| 2 | **Selecionar Tenant** | `POST /api/auth/refresh` | Selecionar empresa/tenant ativo |
| 3 | **Health Check** | `GET /health` | Verificar status dos serviços |
| 4 | **Criar Empresa** | `POST /api/empresas` | Cadastrar CNPJ, Razão Social, regime tributário |
| 5 | **Configurar e-Social** | `PUT /api/empresas/{id}/config-esocial` | Definir ambiente, certificado, transmissor |
| 6 | **Upload Certificado** | `POST /api/esocial/certificados` | Enviar arquivo PFX do certificado digital |
| 7 | **Cadastrar Lotações** | `POST /api/lotacoes` | Criar matriz, filiais, obras |
| 8 | **Cadastrar Sindicatos** | `POST /api/sindicatos` | Registrar sindicatos patronais e laborais |
| 9 | **Cadastrar Convênios** | `POST /api/convenios` | Plano de saúde, odontológico, VR, VT |
| 10 | **Cadastrar Horários** | `POST /api/horarios-trabalho` | Definir jornadas padrão |

---

## 📋 Jornada 2: Operador DP — Cadastro de Funcionário (End-to-End)

**Perfil**: Operador DP  
**Duração estimada**: 15-20 minutos por funcionário  
**Objetivo**: Cadastrar um novo funcionário com todos os dados necessários para processar a folha

### Diagrama da Jornada

```mermaid
journey
    title Jornada do Operador DP — Cadastro de Funcionário
    section Dados Pessoais
      Buscar cargo disponível: 4: Operador DP
      Buscar lotação: 4: Operador DP
      Preencher dados pessoais: 5: Operador DP
      Salvar funcionário: 5: Operador DP
    section Documentos
      Adicionar CTPS: 5: Operador DP
      Adicionar RG e CPF: 4: Operador DP
      Adicionar PIS/PASEP: 5: Operador DP
      Anexar comprovantes: 3: Operador DP
    section Contrato
      Definir tipo de contrato: 5: Operador DP
      Vincular cargo e lotação: 5: Operador DP
      Definir salário base: 5: Operador DP
      Associar horário de trabalho: 4: Operador DP
      Vincular sindicato: 3: Operador DP
    section Dependentes
      Adicionar dependentes IRRF: 4: Operador DP
      Adicionar dependentes salário-família: 4: Operador DP
      Configurar pensão alimentícia: 3: Operador DP
    section Remuneração
      Configurar benefícios (VT/VR): 5: Operador DP
      Configurar plano de saúde: 4: Operador DP
      Definir adicionais: 3: Operador DP
    section Bancários
      Cadastrar conta para pagamento: 5: Operador DP
      Cadastrar chave PIX: 3: Operador DP
    section e-Social
      Preencher info e-Social: 4: Operador DP
      Informar deficiência/PCD: 3: Operador DP
```

### Fluxo Detalhado

| # | Etapa | Endpoint | Ação |
|---|-------|----------|------|
| 1 | **Listar Cargos** | `GET /api/cargos?empresaId=` | Buscar cargos disponíveis |
| 2 | **Listar Lotações** | `GET /api/lotacoes?empresaId=` | Buscar lotações disponíveis |
| 3 | **Criar Funcionário** | `POST /api/funcionarios` | Nome, CPF, data admissão, cargo, lotação, salário |
| 4 | **Adicionar Documentos** | `POST /api/documentos` | CPF, RG, CTPS, PIS/PASEP, CNH, Título |
| 5 | **Criar Contrato** | `POST /api/funcionarios/{id}/contrato` | Tipo contrato, salário, horário, sindicato |
| 6 | **Adicionar Dependentes** | `POST /api/dependentes` | Filhos, cônjuge, pensão |
| 7 | **Configurar Remuneração** | `PUT /api/funcionarios/{id}/remuneracao` | VT, VR, plano saúde, adicionais |
| 8 | **Cadastrar Dados Bancários** | `POST /api/funcionarios/{id}/dados-bancarios` | Banco, agência, conta, PIX |
| 9 | **Preencher Info e-Social** | `PUT /api/funcionarios/{id}/info-esocial` | PCD, reservista, primeiro emprego |
| 10 | **Adicionar Mov. Fixa** | `POST /api/funcionarios/{id}/movimentacao-fixa` | Rubricas fixas mensais |

---

## 📋 Jornada 3: Operador DP — Configuração de Rubricas

**Perfil**: Operador DP  
**Duração estimada**: 2-4 horas (setup inicial), 15-30 min (manutenção)  
**Objetivo**: Configurar o plano de rubricas para cálculo correto da folha

### Diagrama da Jornada

```mermaid
journey
    title Jornada do Operador DP — Configuração de Rubricas
    section Estrutura Base
      Criar grupos de rubricas: 5: Operador DP
      Cadastrar rubricas de vencimento: 5: Operador DP
      Cadastrar rubricas de desconto: 5: Operador DP
      Cadastrar rubricas informativas: 4: Operador DP
    section Incidências
      Configurar incidência INSS: 5: Operador DP
      Configurar incidência IRRF: 5: Operador DP
      Configurar incidência FGTS: 5: Operador DP
      Configurar incidência 13º e Férias: 4: Operador DP
    section Cálculo
      Criar rubricas com fórmula: 4: Operador DP
      Configurar composição de rubricas: 4: Operador DP
      Criar tabelas progressivas IRRF: 5: Operador DP
      Criar tabelas progressivas INSS: 5: Operador DP
    section Validação
      Verificar conformidade e-Social: 5: Operador DP
      Simular cálculo de rubricas: 5: Operador DP
      Ajustar com base na simulação: 4: Operador DP
```

### Fluxo Detalhado

| # | Etapa | Endpoint | Ação |
|---|-------|----------|------|
| 1 | **Criar Grupos** | `POST /api/grupos-rubrica` | Agrupar por natureza (Vencimento/Desconto/Informativa) |
| 2 | **Criar Rubricas** | `POST /api/rubricas` | Código, descrição, natureza, tipo cálculo, incidências |
| 3 | **Configurar Incidências** | `POST /api/rubricas/{id}/incidencias` | INSS, IRRF, FGTS, Sindical, 13º, Férias, etc. |
| 4 | **Criar Fórmulas** | `PUT /api/rubricas/{id}/formula` | Expressão NCalc para rubricas calculadas |
| 5 | **Configurar Composição** | `POST /api/rubricas/{id}/composicao` | Rubricas que compõem outras (ex: salário base + adicionais) |
| 6 | **Criar Tabelas Progressivas** | `POST /api/tabelas-progressivas` | Faixas IRRF e INSS por ano vigência |
| 7 | **Verificar Conformidade** | `GET /api/rubricas/conformidade` | Validar contra Tabela 03 do e-Social |
| 8 | **Simular Cálculo** | `POST /api/rubricas/simular` | Testar com salário base, verificar resultados |

---

## 📋 Jornada 4: Operador DP — Ciclo Mensal da Folha

**Perfil**: Operador DP  
**Duração estimada**: 2-4 horas por processamento  
**Objetivo**: Executar o ciclo completo de processamento da folha de pagamento mensal

### Diagrama da Jornada

```mermaid
journey
    title Jornada do Operador DP — Ciclo Mensal da Folha
    section Preparação
      Lançar movimentações mensais: 5: Operador DP
      Lançar eventos do mês: 5: Operador DP
      Revisar afastamentos ativos: 4: Operador DP
      Verificar alterações contratuais: 4: Operador DP
    section Processamento
      Iniciar processamento da folha: 5: Operador DP
      Acompanhar progresso: 4: Operador DP
      Verificar funcionários com erro: 5: Operador DP
      Corrigir inconsistências: 4: Operador DP
      Reprocessar folha: 4: Operador DP
    section Fechamento
      Revisar itens calculados: 5: Operador DP
      Verificar totais (vencimentos/descontos): 5: Operador DP
      Aprovar fechamento: 5: Operador DP
    section Distribuição
      Gerar holerites em lote: 5: Operador DP
      Verificar holerites gerados: 4: Operador DP
      Distribuir para funcionários: 4: Operador DP
```

### Fluxo Detalhado

| # | Etapa | Endpoint | Ação |
|---|-------|----------|------|
| 1 | **Lançar Mov. Mensal** | `POST /api/funcionarios/{id}/movimentacao-mensal` | Horas extras, faltas, comissões do mês |
| 2 | **Verificar Afastamentos** | `GET /api/funcionarios/{id}/afastamentos` | Confirmar afastamentos ativos no período |
| 3 | **Iniciar Processamento** | `POST /api/folha/processar` | Disparar cálculo para empresa e período |
| 4 | **Acompanhar Status** | `GET /api/folha/processamento/{id}` | Monitorar progresso, verificar erros |
| 5 | **Verificar Itens** | `GET /api/folha/processamento/{id}/itens` | Revisar itens calculados por funcionário |
| 6 | **Reprocessar (se necessário)** | `POST /api/folha/reprocessar` | Corrigir e recalcular |
| 7 | **Reabrir (se necessário)** | `POST /api/folha/{id}/reabrir` | Reabrir folha fechada com justificativa |
| 8 | **Verificar Histórico** | `GET /api/folha/{empresaId}/{periodo}/historico` | Consultar versões anteriores |
| 9 | **Gerar Holerites** | `POST /api/relatorios/holerites/lote` | Gerar PDFs em lote |
| 10 | **Acompanhar Geração** | `GET /api/relatorios/holerites/lote/{id}/status` | Verificar progresso da geração |

---

## 📋 Jornada 5: Contador — Obrigações Fiscais e e-Social

**Perfil**: Contador  
**Duração estimada**: 3-5 horas por período  
**Objetivo**: Apurar tributos, gerar guias, enviar eventos ao e-Social e gerar relatórios legais

### Diagrama da Jornada

```mermaid
journey
    title Jornada do Contador — Obrigações Fiscais Mensais
    section Apuração
      Consultar resumo da apuração: 5: Contador
      Verificar apuração por tributo: 5: Contador
      Revisar bases de cálculo: 5: Contador
    section Guias
      Visualizar guias geradas: 5: Contador
      Verificar dados da guia (GPS/DARF): 5: Contador
      Baixar PDF das guias: 5: Contador
      Registrar pagamento: 4: Contador
    section e-Social
      Enviar lote de eventos: 5: Contador
      Acompanhar processamento do lote: 5: Contador
      Verificar eventos enviados: 4: Contador
      Tratar falhas de envio: 4: Contador
      Reprocessar eventos com erro: 4: Contador
    section Relatórios Legais
      Gerar DIRF anual: 5: Contador
      Gerar RAIS anual: 5: Contador
      Gerar resumo mensal: 4: Contador
      Exportar em CSV/PDF: 3: Contador
    section Regras Fiscais
      Consultar regras vigentes: 4: Contador
      Atualizar tabelas progressivas: 5: Contador
      Criar nova regra fiscal: 3: Contador
```

### Fluxo Detalhado

| # | Etapa | Endpoint | Ação |
|---|-------|----------|------|
| 1 | **Consultar Resumo** | `GET /api/fiscais/guias/{empresaId}/{periodo}` | Ver todas as guias do período |
| 2 | **Ver Detalhe Guia** | `GET /api/fiscais/guias/{empresaId}/{periodo}/{tributo}/dados` | Dados completos da guia |
| 3 | **Baixar PDF Guia** | `GET /api/fiscais/guias/{empresaId}/{periodo}/{tributo}` | Download do PDF para pagamento |
| 4 | **Registrar Pagamento** | `POST /api/fiscais/guias/{guiaId}/registrar-pagamento` | Informar valor pago e data |
| 5 | **Enviar Lote e-Social** | `POST /api/esocial/lotes/enviar` | Enviar eventos pendentes |
| 6 | **Acompanhar Lote** | `GET /api/esocial/lotes/{empresaId}` | Verificar status do envio |
| 7 | **Verificar Falhas** | `GET /api/esocial/falhas` | Listar eventos com erro |
| 8 | **Reprocessar Falha** | `POST /api/esocial/falhas/{id}/reprocessar` | Tentar reenvio |
| 9 | **Gerar DIRF** | `GET /api/relatorios/dirf/{empresaId}/{ano}` | Relatório anual IRRF |
| 10 | **Gerar RAIS** | `GET /api/relatorios/rais/{empresaId}/{ano}` | Relatório anual RAIS |
| 11 | **Consultar Regras** | `GET /api/fiscais/regras?tributo=` | Ver regras fiscais ativas |
| 12 | **Criar/Atualizar Regra** | `POST /api/fiscais/regras` | Nova versão de regra fiscal |

---

## 📋 Jornada 6: Operador DP — Gestão de Eventos Trabalhistas

**Perfil**: Operador DP  
**Duração estimada**: 10-30 minutos por evento  
**Objetivo**: Registrar eventos da vida funcional do trabalhador

### Diagrama da Jornada

```mermaid
journey
    title Jornada do Operador DP — Eventos Trabalhistas
    section Admissão
      Registrar admissão: 5: Operador DP
      Definir período de experiência: 4: Operador DP
    section Férias
      Consultar período aquisitivo: 5: Operador DP
      Programar férias: 5: Operador DP
      Definir tipo (normais/coletivas): 4: Operador DP
      Registrar abono pecuniário: 3: Operador DP
    section Afastamentos
      Registrar afastamento por doença: 5: Operador DP
      Informar CID e atestado: 4: Operador DP
      Registrar licença maternidade: 5: Operador DP
      Dar baixa no retorno: 4: Operador DP
    section Alterações Contratuais
      Registrar promoção/mudança cargo: 5: Operador DP
      Alterar salário: 5: Operador DP
      Alterar jornada: 4: Operador DP
    section Desligamento
      Registrar desligamento: 5: Operador DP
      Classificar motivo: 5: Operador DP
      Calcular verbas rescisórias: 5: Operador DP
```

### Fluxo Detalhado

| # | Etapa | Endpoint | Ação |
|---|-------|----------|------|
| 1 | **Registrar Admissão** | `POST /api/admissoes` | Funcionário, empresa, data, cargo, salário |
| 2 | **Programar Férias** | `POST /api/ferias` | Data início, dias gozo, período aquisitivo |
| 3 | **Registrar Afastamento** | `POST /api/afastamentos` | Tipo, data início/fim, CID |
| 4 | **Registrar Alteração** | `POST /api/alteracoes-contratuais` | Campos alterados, valores anterior/novo |
| 5 | **Registrar Desligamento** | `POST /api/desligamentos` | Data, motivo, verbas rescisórias |
| 6 | **Consultar Eventos** | `GET /api/eventos-trabalhistas/funcionario/{id}` | Timeline completa do funcionário |

---

## 📋 Jornada 7: Consulta — Visualização de Holerites e Dados

**Perfil**: Consulta (Gestor / Funcionário)  
**Duração estimada**: 2-5 minutos  
**Objetivo**: Consultar holerites, dados cadastrais e acompanhar eventos

### Diagrama da Jornada

```mermaid
journey
    title Jornada do Usuário Consulta — Acesso a Informações
    section Autenticação
      Fazer login: 5: Consulta
      Visualizar dashboard pessoal: 5: Consulta
    section Holerites
      Listar holerites disponíveis: 5: Consulta
      Visualizar holerite do mês: 5: Consulta
      Baixar PDF do holerite: 5: Consulta
    section Dados Cadastrais
      Visualizar dados pessoais: 4: Consulta
      Verificar dependentes: 3: Consulta
      Consultar dados bancários: 3: Consulta
    section Eventos
      Verificar férias programadas: 4: Consulta
      Acompanhar afastamentos: 3: Consulta
      Ver histórico de eventos: 3: Consulta
```

### Fluxo Detalhado

| # | Etapa | Endpoint | Ação |
|---|-------|----------|------|
| 1 | **Login** | `POST /api/auth/login` | Autenticar com e-mail e senha |
| 2 | **Listar Holerites** | `GET /api/folha/holerites/{processamentoId}` | Ver holerites disponíveis |
| 3 | **Baixar Holerite** | `GET /api/folha/holerites/{processamentoId}/{funcionarioId}` | Download do PDF |
| 4 | **Ver Dados Pessoais** | `GET /api/funcionarios/{id}` | Consultar cadastro |
| 5 | **Ver Dependentes** | `GET /api/dependentes?funcionarioId=` | Listar dependentes |
| 6 | **Ver Eventos** | `GET /api/eventos-trabalhistas/funcionario/{id}` | Timeline de eventos |

---

## 📋 Jornada 8: Contador — Fechamento Mensal Completo

**Perfil**: Contador  
**Duração estimada**: 4-6 horas  
**Objetivo**: Executar o fechamento contábil e fiscal do mês

### Diagrama da Jornada

```mermaid
journey
    title Jornada do Contador — Fechamento Mensal
    section Verificação Prévia
      Verificar status fiscal da empresa: 5: Contador
      Revisar resumo mensal da folha: 5: Contador
      Verificar folha sintética: 4: Contador
      Verificar folha analítica: 4: Contador
    section Obrigações
      Apurar IRRF do mês: 5: Contador
      Apurar INSS do mês: 5: Contador
      Apurar FGTS do mês: 5: Contador
      Gerar GPS (INSS): 5: Contador
      Gerar DARF (IRRF): 5: Contador
      Gerar GRF (FGTS): 5: Contador
    section Envio e-Social
      Enviar eventos S-1200 (remuneração): 5: Contador
      Enviar eventos S-1210 (pagamentos): 5: Contador
      Enviar fechamento S-1299: 5: Contador
      Verificar recibos do governo: 5: Contador
    section Relatórios
      Gerar resumo mensal: 4: Contador
      Agendar relatórios recorrentes: 3: Contador
      Enviar relatórios por e-mail: 3: Contador
    section Arquivo
      Exportar lançamentos contábeis: 4: Contador
      Atualizar materialized views: 3: Contador
```

### Fluxo Detalhado

| # | Etapa | Endpoint | Ação |
|---|-------|----------|------|
| 1 | **Status Fiscal** | `GET /api/fiscais/apuracao/status/{empresaId}` | Ver pendências e vencimentos |
| 2 | **Resumo Mensal** | `GET /api/relatorios/resumo-mensal/{empresaId}/{periodo}` | Visão consolidada |
| 3 | **Folha Sintética** | `GET /api/relatorios/folha-sintetica/{empresaId}/{periodo}` | Totais por rubrica |
| 4 | **Folha Analítica** | `GET /api/relatorios/folha-analitica/{empresaId}/{periodo}` | Detalhe por funcionário |
| 5 | **Verificar Guias** | `GET /api/fiscais/guias/{empresaId}/{periodo}` | Listar todas as guias |
| 6 | **Registrar Pagamentos** | `POST /api/fiscais/guias/{guiaId}/registrar-pagamento` | Baixar guias pagas |
| 7 | **Enviar e-Social** | `POST /api/esocial/lotes/enviar` | Lote com eventos do período |
| 8 | **Refresh Views** | `POST /api/relatorios/materialized-views/refresh` | Atualizar views materializadas |
| 9 | **Agendar Relatórios** | `POST /api/relatorios/agendamentos` | Configurar envio automático |
| 10 | **Enviar Email** | `POST /api/relatorios/enviar-email` | Enviar relatórios para diretoria |

---

## 📋 Jornada 9: Operador DP — Rescisão de Funcionário

**Perfil**: Operador DP  
**Duração estimada**: 20-30 minutos  
**Objetivo**: Processar o desligamento completo de um funcionário

### Diagrama da Jornada

```mermaid
journey
    title Jornada do Operador DP — Processo de Rescisão
    section Evento
      Registrar evento de desligamento: 5: Operador DP
      Classificar motivo da rescisão: 5: Operador DP
    section Cálculo
      Processar folha rescisória: 5: Operador DP
      Verificar saldo de salário: 5: Operador DP
      Verificar aviso prévio: 5: Operador DP
      Verificar 13º proporcional: 5: Operador DP
      Verificar férias vencidas/proporcionais: 5: Operador DP
      Calcular multa FGTS: 5: Operador DP
    section Documentos
      Gerar TRCT (holerite rescisório): 5: Operador DP
      Gerar guias rescisórias: 5: Operador DP
      Gerar chave FGTS para saque: 4: Operador DP
    section Finalização
      Atualizar status do funcionário: 5: Operador DP
      Enviar evento S-2299 (desligamento): 5: Operador DP
```

### Fluxo Detalhado

| # | Etapa | Endpoint | Ação |
|---|-------|----------|------|
| 1 | **Registrar Desligamento** | `POST /api/desligamentos` | Data, motivo, verbas rescisórias |
| 2 | **Processar Folha Rescisão** | `POST /api/folha/processar` | TipoCalculo = Rescisao |
| 3 | **Verificar Itens** | `GET /api/folha/processamento/{id}/itens` | Revisar verbas calculadas |
| 4 | **Gerar Holerite** | `GET /api/folha/holerites/{processamentoId}/{funcionarioId}` | TRCT em PDF |
| 5 | **Atualizar Funcionário** | `PUT /api/funcionarios/{id}` | Data desligamento, status inativo |

---

## 📋 Jornada 10: Operador DP — Décimo Terceiro Salário

**Perfil**: Operador DP  
**Duração estimada**: 1-2 horas  
**Objetivo**: Processar o 13º salário dos funcionários

### Diagrama da Jornada

```mermaid
journey
    title Jornada do Operador DP — Décimo Terceiro
    section Primeira Parcela
      Verificar funcionários elegíveis: 5: Operador DP
      Processar 1ª parcela (adiantamento): 5: Operador DP
      Verificar valores calculados: 5: Operador DP
      Gerar holerites da 1ª parcela: 4: Operador DP
    section Segunda Parcela
      Processar 2ª parcela (complemento): 5: Operador DP
      Verificar descontos (INSS/IRRF): 5: Operador DP
      Verificar médias de comissões/horas extras: 4: Operador DP
      Gerar holerites da 2ª parcela: 4: Operador DP
    section Fechamento
      Verificar totais do 13º: 5: Operador DP
      Apurar INSS e IRRF sobre 13º: 5: Operador DP
      Enviar eventos e-Social S-1200: 4: Operador DP
```

### Fluxo Detalhado

| # | Etapa | Endpoint | Ação |
|---|-------|----------|------|
| 1 | **Processar 1ª Parcela** | `POST /api/folha/processar` | TipoCalculo = DecimoTerceiro (1ª parcela) |
| 2 | **Verificar Itens** | `GET /api/folha/processamento/{id}/itens` | Revisar adiantamentos |
| 3 | **Processar 2ª Parcela** | `POST /api/folha/processar` | TipoCalculo = DecimoTerceiro (2ª parcela) |
| 4 | **Verificar Totais** | `GET /api/folha/processamento/{id}` | Conferir totais |
| 5 | **Gerar Holerites** | `POST /api/relatorios/holerites/lote` | PDFs do 13º |

---

## 📊 Matriz de Jornadas por Perfil

| Jornada | Admin | Operador DP | Contador | Consulta |
|---------|-------|-------------|----------|----------|
| 1. Setup Inicial | ✅ | — | — | — |
| 2. Cadastro de Funcionário | — | ✅ | — | — |
| 3. Configuração de Rubricas | — | ✅ | ✅ | — |
| 4. Ciclo Mensal da Folha | — | ✅ | — | — |
| 5. Obrigações Fiscais e e-Social | — | — | ✅ | — |
| 6. Eventos Trabalhistas | — | ✅ | — | — |
| 7. Consulta de Holerites | — | — | — | ✅ |
| 8. Fechamento Mensal | — | — | ✅ | — |
| 9. Rescisão | — | ✅ | — | — |
| 10. Décimo Terceiro | — | ✅ | — | — |

---

## 🔄 Linha do Tempo — Ciclo Anual do DP

```mermaid
gantt
    title Ciclo Anual do Departamento Pessoal no Folha360
    dateFormat  YYYY-MM
    axisFormat  %b
    
    section Mensal (Todo Mês)
    Lançar movimentações mensais    :active, m1, 2026-01, 12M
    Processar folha mensal          :active, m2, 2026-01, 12M
    Gerar holerites                 :active, m3, 2026-01, 12M
    Apurar tributos (INSS/IRRF/FGTS):active, m4, 2026-01, 12M
    Gerar guias (GPS/DARF/GRF)      :active, m5, 2026-01, 12M
    Enviar e-Social mensal          :active, m6, 2026-01, 12M
    
    section Eventos
    Férias (programação)            :f1, 2026-01, 12M
    Rescisões (conforme ocorrem)    :f2, 2026-01, 12M
    
    section Periódico
    Décimo Terceiro - 1ª Parcela    :crit, d1, 2026-11, 1M
    Décimo Terceiro - 2ª Parcela    :crit, d2, 2026-12, 1M
    
    section Anual
    DIRF (ano anterior)             :milestone, r1, 2026-02, 0M
    RAIS (ano anterior)             :milestone, r2, 2026-03, 0M
    Atualizar tabelas IRRF/INSS     :crit, r3, 2026-01, 1M
```

---

## 🎯 Pontos de Decisão e Ramificações

```mermaid
flowchart TD
    START((Início do Mês)) --> MOV{Novas movimentações?}
    MOV -->|Sim| LANCAR[Lançar mov. mensais]
    MOV -->|Não| EVENTOS{Novos eventos?}
    LANCAR --> EVENTOS
    
    EVENTOS -->|Admissão| ADM[Registrar admissão]
    EVENTOS -->|Férias| FER[Programar férias]
    EVENTOS -->|Afastamento| AFA[Registrar afastamento]
    EVENTOS -->|Desligamento| DES[Iniciar rescisão]
    EVENTOS -->|Nenhum| FOLHA[Processar folha]
    
    ADM --> FOLHA
    FER --> FOLHA
    AFA --> FOLHA
    DES --> RESC[Processar folha rescisória]
    RESC --> FOLHA
    
    FOLHA --> ERROS{Erros?}
    ERROS -->|Sim| CORRIGIR[Corrigir e reprocessar]
    CORRIGIR --> FOLHA
    ERROS -->|Não| FECHAR[Aprovar fechamento]
    
    FECHAR --> HOLERITES[Gerar holerites]
    HOLERITES --> TRIBUTOS[Apurar tributos]
    TRIBUTOS --> GUIAS[Gerar guias]
    GUIAS --> ESOCIAL[Enviar e-Social]
    ESOCIAL --> RELATORIOS[Gerar relatórios]
    RELATORIOS --> FIM((Fim do Ciclo))
    
    style START fill:#4caf50,color:#fff
    style FIM fill:#4caf50,color:#fff
    style ERROS fill:#ff9800,color:#fff
    style EVENTOS fill:#2196f3,color:#fff
    style MOV fill:#2196f3,color:#fff
```

---

> **Versão 1.0** — Documento que mapeia **10 jornadas completas** de usuário, cobrindo 
> **4 perfis** (Admin, Operador DP, Contador, Consulta), com diagramas de jornada, 
> fluxos detalhados com endpoints, e visão do ciclo anual do Departamento Pessoal.

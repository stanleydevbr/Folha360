# Runbook de Operação — F05 Obrigações Fiscais

## Visão Geral
Este runbook documenta os procedimentos operacionais para o módulo de Obrigações Fiscais (F05) do Folha360. Destina-se a administradores de sistema e contadores que operam o módulo.

---

## 1. Como Reprocessar Apuração Fiscal Manualmente

**Cenário**: Uma apuração fiscal falhou ou precisa ser refeita após correção de dados.

**Procedimento**:
1. Verificar se a folha foi reprocessada (F04) — a apuração fiscal depende dos dados da folha
2. Chamar o endpoint de apuração manual:
   ```
   POST /api/fiscais/apuracao/{empresaId}/{periodo}/reprocessar
   ```
3. Acompanhar o status via:
   ```
   GET /api/fiscais/apuracao/{empresaId}/{periodo}
   ```
4. Verificar que o status mudou para `Concluido` e as guias foram geradas

**Rollback**: Se necessário, usar o endpoint de reversão (ver seção 2).

---

## 2. Como Reverter Guias em Caso de Erro

**Cenário**: Guias foram geradas com valores incorretos e precisam ser canceladas.

**Procedimento**:
1. Identificar o `empresaId` e `periodo` afetados
2. Chamar o endpoint de reversão:
   ```
   POST /api/fiscais/guias/{empresaId}/{periodo}/reverter
   ```
3. Verificar que todas as guias do período estão com status `Cancelada`:
   ```
   GET /api/fiscais/guias/{empresaId}/{periodo}
   ```
4. Após correção da causa raiz, reprocessar a apuração (seção 1)

**Atenção**: A reversão é acionada automaticamente pela Saga de Reabertura do F04 quando uma folha é reaberta. O procedimento manual só é necessário em cenários excepcionais.

---

## 3. Como Cadastrar Novas Regras Fiscais (Virada de Ano)

**Cenário**: Novo ano-calendário (ex.: 2027) requer novas alíquotas e faixas.

**Procedimento**:
1. Obter as alíquotas e faixas oficiais para o novo ano (fonte: Receita Federal, INSS, legislação municipal)
2. Para cada tributo, cadastrar a nova versão:
   ```
   POST /api/fiscais/regras
   {
     "tributo": "IRRF",
     "versao": 2027,
     "vigenciaInicio": "2027-01-01",
     "vigenciaFim": "2027-12-31",
     "parametros": "{...}",
     "codigoReceita": "0561"
   }
   ```
3. Verificar que a nova regra aparece na listagem:
   ```
   GET /api/fiscais/regras?tributo=IRRF
   ```
4. **Importante**: Períodos de 2026 continuarão usando as regras de 2026 automaticamente

**Formato dos parâmetros por tributo**:

### IRRF
```json
{
  "faixas": [
    {"limite": 2259.20, "aliquota": 0, "deducao": 0},
    {"limite": 2826.65, "aliquota": 0.075, "deducao": 169.44}
  ],
  "deducaoDependente": 189.59
}
```

### INSS
```json
{
  "aliquotaPatronal": 0.20,
  "rat": 0.02,
  "fap": 0.01,
  "terceiros": 0.058,
  "faixasEmpregado": [
    {"limite": 1412.00, "aliquota": 0.075},
    {"limite": 2666.68, "aliquota": 0.09}
  ],
  "teto": 7786.02
}
```

### ISS (com alíquotas por município)
```json
{
  "aliquotaPadrao": 0.05,
  "aliquotasPorMunicipio": [
    {"municipio": "São Paulo", "aliquota": 0.05},
    {"municipio": "Belo Horizonte", "aliquota": 0.03}
  ]
}
```

---

## 4. Como Diagnosticar Falhas

### Logs (Seq)
- Acessar o Seq em `http://seq:5341`
- Filtrar por `SourceContext ~ Fiscais` para ver logs do módulo
- Níveis de log:
  - `Information`: Apuração iniciada/concluída, guias geradas
  - `Warning`: Regra fiscal não encontrada, guia vencida
  - `Error`: Falha na apuração, falha na geração de PDF

### Métricas (Prometheus/Grafana)
- Dashboard: `Folha360 > Fiscais`
- Métricas principais:
  - `fiscais_apuracao_duration_seconds`: Tempo de apuração por tributo
  - `fiscais_guias_geradas_total`: Contador de guias geradas
  - `fiscais_erros_total`: Contador de erros
- Alertas configurados:
  - Apuração > 30 min para 100K funcionários
  - Erro rate > 1%
  - Guias vencidas não pagas

### Dead-Letter Queue (RabbitMQ)
- Acessar o RabbitMQ Management em `http://rabbitmq:15672`
- Verificar fila `fiscais_dead_letter`
- Mensagens nesta fila indicam falhas após 3 tentativas de retry

---

## 5. Como Escalar

### Aumentar paralelismo
A apuração fiscal processa tributos sequencialmente dentro do consumer. Para escalar:
- Aumentar `ConcurrencyLimit` do MassTransit no `appsettings.json`:
  ```json
  "MassTransit": {
    "ConcurrencyLimit": 10
  }
  ```

### Cache Redis
- Regras fiscais são cacheadas por 1 hora
- Em caso de alta latência, verificar conexão Redis:
  ```
  redis-cli -h redis ping
  ```
- Para invalidar cache manualmente:
  ```
  redis-cli -h redis DEL cache:regra_fiscal:IRRF:2026
  ```

### Banco de Dados
- Tabela `apuracao_fiscal` é particionada por período (mensal)
- Para arquivar dados antigos (> 5 anos), usar o script de archival:
  ```sql
  SELECT archival.archive_apuracao_fiscal('2021-01-01');
  ```

---

## 6. Contatos e Procedimentos de Emergência

### Emergência: Guias não geradas até o vencimento
1. Verificar logs no Seq para identificar a causa
2. Se a apuração falhou, reprocessar manualmente (seção 1)
3. Se o problema persistir, contatar o desenvolvedor responsável
4. **Prazo crítico**: GPS vence dia 20, GRF vence dia 7

### Emergência: Regra fiscal incorreta
1. NÃO alterar a regra existente — isso afetaria períodos já processados
2. Cadastrar nova versão da regra com `vigenciaInicio` retroativo (se necessário)
3. Reprocessar a apuração do período afetado

### Contatos
- Desenvolvedor responsável: Time Folha360
- Consultor contábil: [a definir]
- Suporte técnico: [a definir]

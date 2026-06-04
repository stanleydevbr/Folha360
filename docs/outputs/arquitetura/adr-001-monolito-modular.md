# ADR-001: Monólito Modular como Arquitetura de Deploy

## Status
**Accepted** (Junho 2026)

## Context
O sistema Folha360 é composto por 6 módulos de domínio (Cadastros, Eventos Trabalhistas, Cálculo da Folha, Obrigações Fiscais, Relatórios, Integração e-Social). A equipe estimada é de 5-15 desenvolvedores. Precisamos decidir entre implantar como serviços independentes (microservices) ou como um monólito modular (deploy único com isolamento lógico).

## Decision Drivers
1. **Complexidade operacional**: Time pequeno não tem capacidade para gerenciar orquestração de microservices
2. **Performance**: Comunicação entre módulos é intensa (cálculo da folha consulta cadastros e eventos constantemente)
3. **Custo de infraestrutura**: Orçamento inicial limitado
4. **Evolução futura**: Possibilidade de extrair módulos específicos quando necessário
5. **LGPD**: Isolamento de dados sensíveis entre módulos

## Options Considered

| Opção | Prós | Contras |
|---|---|---|
| **Microservices puro** | Escala independente; deploy independente; isolamento de falha | Complexidade operacional alta; latência de rede; custo de infra; exige times autônomos |
| **Monólito modular (escolhido)** | Deploy simples; baixa latência; menor custo; adequado ao time | Escala tudo junto; crash afeta todos; deploy tudo ou nada |
| **Monólito tradicional** | Mais simples ainda | Sem isolamento entre módulos; acoplamento inevitável; difícil de manter |

## Decision
**Monólito Modular**: Todos os módulos são implantados como um único artefato (.NET assembly), mas com separação clara em projetos/namespaces, respeitando bounded contexts DDD. A comunicação entre módulos é feita via interfaces (em processo) e eventos de domínio (RabbitMQ), permitindo extração gradual para microservices no futuro (strangler fig pattern).

## Consequences

### Positive
- Deploy único simplifica CI/CD e operação
- Comunicação em processo elimina latência de rede entre módulos
- Menor custo de infraestrutura (menos containers, menos RAM)
- Debugging mais simples (um processo)
- Adequado ao tamanho da equipe

### Negative
- Escala horizontal escala todos os módulos juntos (desperdício de recursos)
- Crash em um módulo pode derrubar todo o sistema
- Deploy é tudo ou nada (não é possível deploy independente)
- Requer disciplina para não criar acoplamento entre módulos

### Mitigações
- Health checks por módulo para identificar rapidamente a origem de falhas
- HPA (Horizontal Pod Autoscaler) mitiga desperdício de recursos
- Contratos entre módulos são definidos como se fossem remotos (facilita extração futura)
- Testes de integração por módulo garantem isolamento

## Follow-up Actions
- [ ] Definir projetos .NET por módulo (src/Cadastros, src/Eventos, src/Folha, etc.)
- [ ] Estabelecer contratos de interface entre módulos
- [ ] Configurar health checks por módulo
- [ ] Revisar esta decisão em 12 meses ou quando a equipe crescer para 20+ desenvolvedores

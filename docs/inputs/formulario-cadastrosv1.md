## Formulário de Cadastro

### 1. Objetivo
Definir o novo layout e comportamento dos formulários de cadastro (criação e edição de registros) no painel administrativo do Folha360, substituindo o modelo atual de drawer lateral por uma experiência em tela cheia ou modal centralizado.

---

### 2. Tipos de Formulário

#### 2.1 Formulário de Página Inteira (Full Page)
- **Onde usar:** Cadastros complexos ou que exigem maior espaço para preenchimento (ex: cadastro de empresa, funcionário, tabelas de eventos).
- **Comportamento:** O formulário substitui completamente o conteúdo do painel principal, ocupando 100% da área disponível.
- **Navegação:** Ao salvar ou cancelar, o usuário retorna à tela anterior (geralmente a listagem do módulo).

#### 2.2 Formulário Modal Centralizado
- **Onde usar:** Cadastros simples ou rápidos (ex: cadastro de banco, centro de custo, cargo).
- **Comportamento:** Modal exibido centralizado na tela, sobreposto ao conteúdo do painel principal.
- **Dimensões:** Largura máxima de 720px (ou 80% da viewport, o que for menor). Altura máxima de 85% da viewport com scroll interno.
- **Fechamento:** Clique no backdrop (overlay) **não** deve fechar o modal para evitar perda acidental de dados. Apenas o botão de fechar (X), "Cancelar" ou "Salvar" fecham o modal.

---

### 3. Estrutura do Cabeçalho (Obrigatório para ambos os tipos)

Todo formulário deve conter um cabeçalho fixo com:

| Elemento | Descrição |
|---|---|
| **Título da tela** | Nome do cadastro + ação (ex: "Novo Funcionário", "Editar Empresa"). Deve ser claro e descritivo. |
| **Botão Fechar (X)** | Ícone de "X" no canto superior direito. Deve perguntar "Deseja descartar as alterações?" se houver dados não salvos. |
| **Botão Salvar** | Posicionado no lado direito do cabeçalho. Deve validar os campos obrigatórios antes de submeter. |
| **Botão Cancelar** | Posicionado ao lado do botão Salvar. Deve retornar à tela anterior sem salvar (com confirmação se houver dados alterados). |

> **Nota:** No formulário Full Page, o cabeçalho deve ser **fixo** (sticky) no topo. No modal, o cabeçalho deve estar no topo do modal e não scrolla com o conteúdo.

---

### 4. Comportamento e Regras de Negócio

- **Proteção contra perda de dados:** Se o usuário tentar fechar/formulário com campos preenchidos sem salvar, exibir um diálogo de confirmação: "Deseja descartar as alterações não salvas?"
- **Validação em tempo real:** Campos obrigatórios devem ser validados ao perder o foco (onBlur). A validação completa deve ocorrer ao clicar em "Salvar".
- **Scroll:** Formulários Full Page devem scrollar naturalmente com a página. Modais devem ter scroll interno no corpo do modal.
- **Responsividade:** Em viewports menores que 768px, o modal deve se comportar como Full Page (ocupar toda a tela).
- **Estado de loading:** Ao submeter, exibir indicador de carregamento no botão Salvar e desabilitar interações para evitar dupla submissão.
- **Feedback visual:** Exibir notificação de sucesso (toast/snackbar) após salvar com sucesso e notificação de erro em caso de falha.

---

### 5. Aparência Visual (Design)

#### Cabeçalho
- Fundo: `--color-surface` (tema claro) / `--color-surface-dark` (tema escuro)
- Borda inferior: 1px sólida `--color-border`
- Padding: `16px 24px`
- Altura mínima: 56px
- Título: Tipografia Heading 5 (H5), peso semibold

#### Botões no Cabeçalho
- **Salvar:** Botão primário (`--color-primary`) com ícone de check ou texto "Salvar"
- **Cancelar:** Botão secundário/outline com texto "Cancelar"
- **Fechar:** Ícone X sem borda, apenas hover com fundo sutil

#### Corpo do Formulário (Full Page)
- Padding: `24px 32px`
- Largura máxima do conteúdo: 960px (centralizado na página)
- Espaçamento entre campos: `24px` vertical

#### Corpo do Modal
- Padding interno: `24px`
- Largura máxima: 720px
- Border-radius: `12px`
- Sombra: `--shadow-lg`
- Overlay: Fundo preto com 50% de opacidade

---

### 6. Estados e Transições

| Estado | Comportamento |
|---|---|
| **Padrão** | Formulário exibido com campos vazios (novo) ou preenchidos (edição) |
| **Validando** | Campos com erro exibem borda vermelha e mensagem de erro abaixo do campo |
| **Submetendo** | Botão Salvar desabilitado com spinner; campos desabilitados |
| **Sucesso** | Fechar formulário e exibir toast de sucesso |
| **Erro** | Toast de erro com mensagem descritiva; campos permanecem preenchidos |

---

### 7. Exemplos de Uso

**Full Page:** Cadastro de Funcionário (múltiplas seções: dados pessoais, endereço, contato, documentos, dependentes)

**Modal:** Cadastro de Centro de Custo (apenas código, nome e descrição)

---

### 8. Critérios de Aceitação

- [ ] Formulários não são mais exibidos em drawer lateral
- [ ] Full Page ocupa 100% da área do painel principal
- [ ] Modal aparece centralizado com overlay
- [ ] Cabeçalho fixo com título, fechar, salvar e cancelar
- [ ] Proteção contra perda de dados não salvos
- [ ] Validação visual clara de campos obrigatórios
- [ ] Responsivo: modal vira full page em mobile
- [ ] Estados de loading visíveis durante submissão 
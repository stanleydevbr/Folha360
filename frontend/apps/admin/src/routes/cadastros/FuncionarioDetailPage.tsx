import { useState } from 'react'
import { useParams, useNavigate } from 'react-router'
import { useAuth } from '@/providers/AuthProvider'
import {
  useFuncionario,
  useUpdateFuncionario,
  useDocumentos,
  useCreateDocumento,
  useDeleteDocumento,
  useContrato,
  useCreateContrato,
  useUpdateContrato,
  useDependentes,
  useCreateDependente,
  useDeleteDependente,
  useRemuneracao,
  useUpdateRemuneracao,
  useDadosBancarios,
  useCreateDadosBancarios,
  useDeleteDadosBancarios,
  useMovimentacaoFixa,
  useCreateMovimentacaoFixa,
  useDeleteMovimentacaoFixa,
  useMovimentacaoMensal,
  useCreateMovimentacaoMensal,
  useAfastamentosFuncionario,
  useCreateAfastamentoFuncionario,
  useInfoESocialFuncionario,
  useUpdateInfoESocialFuncionario,
  useCargos,
  useLotacoes,
  useSindicatos,
  useHorariosTrabalho,
  useRubricas,
  useBancos,
} from '@folha360/api'
import type {
  FuncionarioDto,
  Documento,
  ContratoTrabalho,
  Dependente,
  RemuneracaoBeneficio,
  DadosBancariosFuncionario,
  MovimentacaoFixa,
  MovimentacaoMensal,
  AfastamentoFuncionario,
  InfoESocialFuncionario,
} from '@folha360/api'
import {
  DataTable,
  FormContainer,
  FormInput,
  FormSelect,
  FormGrid,
  FormCheckbox,
  FormTextarea,
  FuncionarioFormFields,
  DocumentoFormFields,
  ContratoFormFields,
  DependenteFormFields,
  RemuneracaoFormFields,
  DadosBancariosFormFields,
  AfastamentoFormFields,
  InfoESocialFormFields,
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  Button,
  Badge,
  Skeleton,
} from '@folha360/ui'
import type { Column } from '@folha360/ui'
import { formatCpf, formatCurrency, formatDate } from '@folha360/utils'
import {
  ArrowLeft,
  Plus,
  Pencil,
  Trash2,
  User,
  FileText,
  Briefcase,
  Users,
  Wallet,
  Landmark,
  Clock,
  Heart,
  Shield,
  DollarSign,
} from 'lucide-react'
import { toast } from 'sonner'

type Tab = 'dados' | 'documentos' | 'contrato' | 'dependentes' | 'remuneracao' | 'bancarios' | 'movfixa' | 'afastamentos' | 'esocial'

const TABS: { id: Tab; label: string; icon: React.ReactNode }[] = [
  { id: 'dados', label: 'Dados Pessoais', icon: <User className="h-4 w-4" /> },
  { id: 'documentos', label: 'Documentos', icon: <FileText className="h-4 w-4" /> },
  { id: 'contrato', label: 'Contrato', icon: <Briefcase className="h-4 w-4" /> },
  { id: 'dependentes', label: 'Dependentes', icon: <Users className="h-4 w-4" /> },
  { id: 'remuneracao', label: 'Remuneração', icon: <Wallet className="h-4 w-4" /> },
  { id: 'bancarios', label: 'Bancário', icon: <Landmark className="h-4 w-4" /> },
  { id: 'movfixa', label: 'Mov. Fixa', icon: <DollarSign className="h-4 w-4" /> },
  { id: 'afastamentos', label: 'Afastamentos', icon: <Heart className="h-4 w-4" /> },
  { id: 'esocial', label: 'e-Social', icon: <Shield className="h-4 w-4" /> },
]

export default function FuncionarioDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { apiClient } = useAuth()
  const [tab, setTab] = useState<Tab>('dados')

  const { data: funcionario, isLoading } = useFuncionario(apiClient, id)

  if (isLoading || !funcionario) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-8 w-64" />
        <Skeleton className="h-64 w-full" />
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-4">
        <Button variant="ghost" size="sm" onClick={() => navigate('/cadastros/funcionarios')}>
          <ArrowLeft className="mr-1 h-4 w-4" /> Voltar
        </Button>
        <div className="flex-1">
          <h1 className="text-2xl font-bold">{funcionario.nome}</h1>
          <p className="text-sm text-muted-foreground">
            CPF: {funcionario.cpfMascarado} · Status:{' '}
            <Badge variant={funcionario.status === 'Ativo' ? 'default' : 'secondary'} className="ml-1">
              {funcionario.status}
            </Badge>
          </p>
        </div>
      </div>

      {/* Tab Navigation */}
      <div className="flex gap-1 border-b overflow-x-auto">
        {TABS.map((t) => (
          <button
            key={t.id}
            onClick={() => setTab(t.id)}
            className={`flex items-center gap-1.5 px-4 py-2.5 text-sm font-medium whitespace-nowrap transition-colors border-b-2 -mb-px ${
              tab === t.id
                ? 'border-primary text-primary'
                : 'border-transparent text-muted-foreground hover:text-foreground'
            }`}
          >
            {t.icon}
            {t.label}
          </button>
        ))}
      </div>

      {/* Tab Content */}
      <div className="min-h-[400px]">
        {tab === 'dados' && <DadosTab funcionario={funcionario} />}
        {tab === 'documentos' && <DocumentosTab funcionarioId={funcionario.id} />}
        {tab === 'contrato' && <ContratoTab funcionarioId={funcionario.id} empresaId={funcionario.empresaId} />}
        {tab === 'dependentes' && <DependentesTab funcionarioId={funcionario.id} />}
        {tab === 'remuneracao' && <RemuneracaoTab funcionarioId={funcionario.id} />}
        {tab === 'bancarios' && <BancariosTab funcionarioId={funcionario.id} />}
        {tab === 'movfixa' && <MovFixaTab funcionarioId={funcionario.id} />}
        {tab === 'afastamentos' && <AfastamentosTab funcionarioId={funcionario.id} />}
        {tab === 'esocial' && <ESocialTab funcionarioId={funcionario.id} />}
      </div>
    </div>
  )
}

// ---- Dados Pessoais Tab ----

function DadosTab({ funcionario }: { funcionario: FuncionarioDto }) {
  const { apiClient } = useAuth()
  const updateMutation = useUpdateFuncionario(apiClient)
  const { data: cargosData } = useCargos(apiClient)
  const { data: lotacoesData } = useLotacoes(apiClient)
  const [editing, setEditing] = useState(false)
  const [form, setForm] = useState({
    nome: funcionario.nome,
    dataNascimento: funcionario.dataNascimento ?? '',
    sexo: funcionario.sexo ?? '',
    estadoCivil: funcionario.estadoCivil ?? '',
    nacionalidade: funcionario.nacionalidade ?? '',
    nomeMae: funcionario.nomeMae ?? '',
    nomePai: funcionario.nomePai ?? '',
    cargoId: funcionario.cargoId,
    lotacaoId: funcionario.lotacaoId,
    salarioBase: String(funcionario.salarioBase),
    tipoContrato: funcionario.tipoContrato ?? '',
    jornadaHorasSemanais: funcionario.jornadaHorasSemanais ? String(funcionario.jornadaHorasSemanais) : '',
    enderecoLogradouro: '',
    enderecoNumero: '',
    enderecoComplemento: '',
    enderecoBairro: '',
    enderecoCep: '',
    enderecoMunicipio: '',
    enderecoUf: '',
    telefone: '',
    email: '',
  })

  const cargoOptions = [{ value: '', label: 'Selecione...' }, ...(cargosData?.items ?? []).map((c) => ({ value: c.id, label: `${c.nome} (CBO: ${c.cbo})` }))]
  const lotacaoOptions = [{ value: '', label: 'Selecione...' }, ...(lotacoesData?.items ?? []).map((l) => ({ value: l.id, label: `${l.codigo} - ${l.descricao}` }))]

  if (!editing) {
    return (
      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle className="text-base">Dados Pessoais</CardTitle>
            <Button size="sm" variant="outline" onClick={() => setEditing(true)}><Pencil className="mr-1 h-3 w-3" />Editar</Button>
          </CardHeader>
          <CardContent className="grid grid-cols-2 gap-3">
            <Info label="Nome" value={funcionario.nome} />
            <Info label="CPF" value={funcionario.cpfMascarado} />
            <Info label="Data Nasc." value={funcionario.dataNascimento ? formatDate(funcionario.dataNascimento) : '—'} />
            <Info label="Sexo" value={funcionario.sexo || '—'} />
            <Info label="Estado Civil" value={funcionario.estadoCivil || '—'} />
            <Info label="Nome da Mãe" value={funcionario.nomeMae || '—'} />
          </CardContent>
        </Card>
        <Card>
          <CardHeader><CardTitle className="text-base">Dados Contratuais</CardTitle></CardHeader>
          <CardContent className="grid grid-cols-2 gap-3">
            <Info label="Admissão" value={formatDate(funcionario.dataAdmissao)} />
            <Info label="Status" value={funcionario.status} />
            <Info label="Salário Base" value={formatCurrency(funcionario.salarioBase)} />
            <Info label="Tipo Contrato" value={funcionario.tipoContrato || '—'} />
            <Info label="Jornada Semanal" value={funcionario.jornadaHorasSemanais ? `${funcionario.jornadaHorasSemanais}h` : '—'} />
          </CardContent>
        </Card>
      </div>
    )
  }

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle className="text-base">Editar Dados</CardTitle>
        <Button size="sm" variant="outline" onClick={() => setEditing(false)}>Cancelar</Button>
      </CardHeader>
      <CardContent>
        <FormContainer mode="edit" onSubmit={(e) => { e.preventDefault(); updateMutation.mutateAsync({ id: funcionario.id, data: { ...form, salarioBase: Number(form.salarioBase) || 0, jornadaHorasSemanais: form.jornadaHorasSemanais ? Number(form.jornadaHorasSemanais) : undefined } }).then(() => { toast.success('Atualizado!'); setEditing(false) }).catch(() => toast.error('Erro.')) }} isSubmitting={updateMutation.isPending} submitLabel="Salvar">
          <FuncionarioFormFields
            data={{ ...form, cpf: funcionario.cpfMascarado, dataAdmissao: funcionario.dataAdmissao }}
            onChange={setForm}
            mode="edit"
            cargos={cargoOptions}
            lotacoes={lotacaoOptions}
          />
        </FormContainer>
      </CardContent>
    </Card>
  )
}

function Info({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <span className="text-xs text-muted-foreground">{label}</span>
      <p className="text-sm font-medium">{value}</p>
    </div>
  )
}

// ---- Documentos Tab ----

function DocumentosTab({ funcionarioId }: { funcionarioId: string }) {
  const { apiClient } = useAuth()
  const { data, isLoading } = useDocumentos(apiClient, funcionarioId)
  const createMutation = useCreateDocumento(apiClient)
  const deleteMutation = useDeleteDocumento(apiClient)
  const [adding, setAdding] = useState(false)
  const [form, setForm] = useState<Documento>({
    id: '', funcionarioId, tipo: '', numero: '', dataEmissao: '', dataValidade: '', orgaoEmissor: '', ufEmissor: '',
  })

  const columns: Column<Documento>[] = [
    { key: 'tipo', header: 'Tipo', render: (item) => <Badge variant="outline">{item.tipo}</Badge> },
    { key: 'numero', header: 'Número' },
    { key: 'orgaoEmissor', header: 'Órgão Emissor', render: (item) => item.orgaoEmissor || '—' },
    { key: 'dataEmissao', header: 'Emissão', render: (item) => item.dataEmissao ? formatDate(item.dataEmissao) : '—' },
    { key: 'dataValidade', header: 'Validade', render: (item) => item.dataValidade ? formatDate(item.dataValidade) : '—' },
  ]

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle className="text-base">Documentos</CardTitle>
        <Button size="sm" onClick={() => setAdding(!adding)}><Plus className="mr-1 h-4 w-4" />Novo</Button>
      </CardHeader>
      <CardContent className="space-y-4">
        {adding && (
          <div className="rounded-lg border p-4">
            <FormContainer mode="create" onSubmit={(e) => { e.preventDefault(); createMutation.mutateAsync(form).then(() => { toast.success('Documento adicionado!'); setAdding(false) }).catch(() => toast.error('Erro.')) }} onCancel={() => setAdding(false)} isSubmitting={createMutation.isPending} submitLabel="Adicionar">
              <DocumentoFormFields data={form as any} onChange={(d) => setForm(d as Documento)} mode="create" />
            </FormContainer>
          </div>
        )}
        <DataTable columns={columns} rows={data ?? []} isLoading={isLoading} emptyMessage="Nenhum documento."
          actions={(item) => (
            <Button variant="ghost" size="sm" onClick={() => { if (confirm('Remover?')) deleteMutation.mutateAsync(item.id).then(() => toast.success('Removido!')).catch(() => toast.error('Erro.')) }}>
              <Trash2 className="h-4 w-4 text-destructive" />
            </Button>
          )} />
      </CardContent>
    </Card>
  )
}

// ---- Contrato Tab ----

function ContratoTab({ funcionarioId, empresaId }: { funcionarioId: string; empresaId: string }) {
  const { apiClient } = useAuth()
  const { data, isLoading } = useContrato(apiClient, funcionarioId)
  const createMutation = useCreateContrato(apiClient)
  const updateMutation = useUpdateContrato(apiClient)
  const { data: sindicatos } = useSindicatos(apiClient)
  const { data: horarios } = useHorariosTrabalho(apiClient)
  const [form, setForm] = useState<ContratoTrabalho>({
    id: '', funcionarioId, empresaId, tipoContrato: '', tipoSalario: '', salarioBase: 0, dataAdmissao: '', status: 'Ativo',
  })

  if (isLoading) return <Skeleton className="h-48 w-full" />
  if (data && form.id !== data.id) setForm(data)

  const sindOptions = [{ value: '', label: 'Nenhum' }, ...(sindicatos ?? []).map((s) => ({ value: s.id, label: s.nome }))]
  const horarioOptions = [{ value: '', label: 'Nenhum' }, ...(horarios ?? []).map((h) => ({ value: h.id, label: `${h.codigo} - ${h.descricao}` }))]

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    const action = data ? updateMutation.mutateAsync({ funcionarioId, data: form }) : createMutation.mutateAsync({ funcionarioId, data: form })
    action.then(() => toast.success('Contrato salvo!')).catch(() => toast.error('Erro.'))
  }

  return (
    <Card>
      <CardHeader><CardTitle className="text-base">Contrato de Trabalho</CardTitle></CardHeader>
      <CardContent>
        <FormContainer mode="edit" onSubmit={handleSubmit} isSubmitting={createMutation.isPending || updateMutation.isPending} submitLabel="Salvar">
          <ContratoFormFields
            data={form as any}
            onChange={(d: any) => setForm({ ...form, ...d, salarioBase: Number(d.salarioBase) || 0, cargaHorariaSemanal: d.cargaHorariaSemanal ? Number(d.cargaHorariaSemanal) : undefined })}
            mode="edit"
            sindicatos={sindOptions}
            horarios={horarioOptions}
          />
        </FormContainer>
      </CardContent>
    </Card>
  )
}

// ---- Dependentes Tab ----

function DependentesTab({ funcionarioId }: { funcionarioId: string }) {
  const { apiClient } = useAuth()
  const { data, isLoading } = useDependentes(apiClient, funcionarioId)
  const createMutation = useCreateDependente(apiClient)
  const deleteMutation = useDeleteDependente(apiClient)
  const [adding, setAdding] = useState(false)
  const [form, setForm] = useState<Dependente>({
    id: '', funcionarioId, nome: '', cpf: '', dataNascimento: '', tipo: '', dependenteIrrf: false, dependenteSalarioFamilia: false,
  })

  const columns: Column<Dependente>[] = [
    { key: 'nome', header: 'Nome' },
    { key: 'tipo', header: 'Tipo', render: (item) => <Badge variant="outline">{item.tipo}</Badge> },
    { key: 'dataNascimento', header: 'Nascimento', render: (item) => formatDate(item.dataNascimento) },
    { key: 'dependenteIrrf', header: 'IRRF', render: (item) => item.dependenteIrrf ? <Badge>Sim</Badge> : '—' },
    { key: 'dependenteSalarioFamilia', header: 'Sal. Família', render: (item) => item.dependenteSalarioFamilia ? <Badge>Sim</Badge> : '—' },
  ]

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle className="text-base">Dependentes</CardTitle>
        <Button size="sm" onClick={() => setAdding(!adding)}><Plus className="mr-1 h-4 w-4" />Novo</Button>
      </CardHeader>
      <CardContent className="space-y-4">
        {adding && (
          <div className="rounded-lg border p-4">
            <FormContainer mode="create" onSubmit={(e) => { e.preventDefault(); createMutation.mutateAsync(form).then(() => { toast.success('Dependente adicionado!'); setAdding(false) }).catch(() => toast.error('Erro.')) }} onCancel={() => setAdding(false)} isSubmitting={createMutation.isPending} submitLabel="Adicionar">
              <DependenteFormFields data={form as any} onChange={(d) => setForm(d as unknown as Dependente)} mode="create" />
            </FormContainer>
          </div>
        )}
        <DataTable columns={columns} rows={data ?? []} isLoading={isLoading} emptyMessage="Nenhum dependente."
          actions={(item) => (
            <Button variant="ghost" size="sm" onClick={() => { if (confirm('Remover?')) deleteMutation.mutateAsync(item.id).then(() => toast.success('Removido!')).catch(() => toast.error('Erro.')) }}>
              <Trash2 className="h-4 w-4 text-destructive" />
            </Button>
          )} />
      </CardContent>
    </Card>
  )
}

// ---- Remuneração Tab ----

function RemuneracaoTab({ funcionarioId }: { funcionarioId: string }) {
  const { apiClient } = useAuth()
  const { data, isLoading } = useRemuneracao(apiClient, funcionarioId)
  const updateMutation = useUpdateRemuneracao(apiClient)
  const [form, setForm] = useState<RemuneracaoBeneficio>({
    id: '', funcionarioId, salarioBase: 0, valeTransporte: false, valeRefeicao: false, planoSaude: false, planoOdontologico: false, seguroVida: false, previdenciaPrivada: false,
  })

  if (isLoading) return <Skeleton className="h-48 w-full" />
  if (data && form.id !== data.id) setForm(data)

  return (
    <Card>
      <CardHeader><CardTitle className="text-base">Remuneração e Benefícios</CardTitle></CardHeader>
      <CardContent>
        <FormContainer mode="edit" onSubmit={(e) => { e.preventDefault(); updateMutation.mutateAsync({ funcionarioId, data: form }).then(() => toast.success('Salvo!')).catch(() => toast.error('Erro.')) }} isSubmitting={updateMutation.isPending} submitLabel="Salvar">
          <RemuneracaoFormFields
            data={form as any}
            onChange={(d) => setForm(d as unknown as RemuneracaoBeneficio)}
            mode="edit"
          />
        </FormContainer>
      </CardContent>
    </Card>
  )
}

// ---- Bancários Tab ----

function BancariosTab({ funcionarioId }: { funcionarioId: string }) {
  const { apiClient } = useAuth()
  const { data, isLoading } = useDadosBancarios(apiClient, funcionarioId)
  const { data: bancosData } = useBancos(apiClient)
  const createMutation = useCreateDadosBancarios(apiClient)
  const deleteMutation = useDeleteDadosBancarios(apiClient)
  const [adding, setAdding] = useState(false)
  const [form, setForm] = useState<DadosBancariosFuncionario>({
    id: '', funcionarioId, bancoId: '', agencia: '', conta: '', tipoConta: '', contaPrincipal: false,
  })

  const bancoOptions = [{ value: '', label: 'Selecione...' }, ...(bancosData ?? []).map((b) => ({ value: b.id, label: `${b.codigo} - ${b.nome}` }))]

  const columns: Column<DadosBancariosFuncionario>[] = [
    { key: 'bancoId', header: 'Banco' },
    { key: 'agencia', header: 'Agência' },
    { key: 'conta', header: 'Conta' },
    { key: 'tipoConta', header: 'Tipo', render: (item) => <Badge variant="outline">{item.tipoConta}</Badge> },
    { key: 'contaPrincipal', header: 'Principal', render: (item) => item.contaPrincipal ? <Badge>Sim</Badge> : '—' },
  ]

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle className="text-base">Dados Bancários</CardTitle>
        <Button size="sm" onClick={() => setAdding(!adding)}><Plus className="mr-1 h-4 w-4" />Nova</Button>
      </CardHeader>
      <CardContent className="space-y-4">
        {adding && (
          <div className="rounded-lg border p-4">
            <FormContainer mode="create" onSubmit={(e) => { e.preventDefault(); createMutation.mutateAsync({ funcionarioId, data: form }).then(() => { toast.success('Conta adicionada!'); setAdding(false) }).catch(() => toast.error('Erro.')) }} onCancel={() => setAdding(false)} isSubmitting={createMutation.isPending} submitLabel="Adicionar">
              <DadosBancariosFormFields data={form as any} onChange={(d) => setForm(d as DadosBancariosFuncionario)} mode="create" bancos={bancoOptions} />
            </FormContainer>
          </div>
        )}
        <DataTable columns={columns} rows={data ?? []} isLoading={isLoading} emptyMessage="Nenhuma conta bancária."
          actions={(item) => (
            <Button variant="ghost" size="sm" onClick={() => { if (confirm('Remover?')) deleteMutation.mutateAsync({ funcionarioId, id: item.id }).then(() => toast.success('Removida!')).catch(() => toast.error('Erro.')) }}>
              <Trash2 className="h-4 w-4 text-destructive" />
            </Button>
          )} />
      </CardContent>
    </Card>
  )
}

// ---- Movimentação Fixa Tab ----

function MovFixaTab({ funcionarioId }: { funcionarioId: string }) {
  const { apiClient } = useAuth()
  const { data, isLoading } = useMovimentacaoFixa(apiClient, funcionarioId)
  const { data: rubricasData } = useRubricas(apiClient, { pageSize: 200 })
  const createMutation = useCreateMovimentacaoFixa(apiClient)
  const deleteMutation = useDeleteMovimentacaoFixa(apiClient)
  const [adding, setAdding] = useState(false)
  const [form, setForm] = useState<MovimentacaoFixa>({
    id: '', funcionarioId, rubricaId: '', valor: 0,
  })

  const rubricaOptions = [{ value: '', label: 'Selecione...' }, ...(rubricasData?.items ?? []).map((r) => ({ value: r.id, label: `${r.codigo} - ${r.descricao}` }))]

  const columns: Column<MovimentacaoFixa>[] = [
    { key: 'rubricaId', header: 'Rubrica' },
    { key: 'descricao', header: 'Descrição', render: (item) => item.descricao || '—' },
    { key: 'quantidade', header: 'Qtd', render: (item) => item.quantidade ? String(item.quantidade) : '—' },
    { key: 'valor', header: 'Valor', render: (item) => formatCurrency(item.valor) },
  ]

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle className="text-base">Movimentação Fixa (Rubricas Recorrentes)</CardTitle>
        <Button size="sm" onClick={() => setAdding(!adding)}><Plus className="mr-1 h-4 w-4" />Nova</Button>
      </CardHeader>
      <CardContent className="space-y-4">
        {adding && (
          <div className="rounded-lg border p-4">
            <FormContainer mode="create" onSubmit={(e) => { e.preventDefault(); createMutation.mutateAsync({ funcionarioId, data: form }).then(() => { toast.success('Adicionada!'); setAdding(false) }).catch(() => toast.error('Erro.')) }} onCancel={() => setAdding(false)} isSubmitting={createMutation.isPending} submitLabel="Adicionar">
              <FormGrid cols={2}>
                <FormSelect id="rubricaId" label="Rubrica" value={form.rubricaId} onChange={(e) => setForm({ ...form, rubricaId: e.target.value })} options={rubricaOptions} required />
                <FormInput id="valor" label="Valor (R$)" type="number" value={String(form.valor)} onChange={(e) => setForm({ ...form, valor: Number(e.target.value) || 0 })} min="0" step="0.01" required />
                <FormInput id="quantidade" label="Quantidade" type="number" value={form.quantidade ? String(form.quantidade) : ''} onChange={(e) => setForm({ ...form, quantidade: e.target.value ? Number(e.target.value) : undefined })} min="0" step="0.01" />
                <FormInput id="descricao" label="Descrição" value={form.descricao ?? ''} onChange={(e) => setForm({ ...form, descricao: e.target.value })} />
              </FormGrid>
            </FormContainer>
          </div>
        )}
        <DataTable columns={columns} rows={data ?? []} isLoading={isLoading} emptyMessage="Nenhuma movimentação fixa."
          actions={(item) => (
            <Button variant="ghost" size="sm" onClick={() => { if (confirm('Remover?')) deleteMutation.mutateAsync({ funcionarioId, id: item.id }).then(() => toast.success('Removida!')).catch(() => toast.error('Erro.')) }}>
              <Trash2 className="h-4 w-4 text-destructive" />
            </Button>
          )} />
      </CardContent>
    </Card>
  )
}

// ---- Afastamentos Tab ----

function AfastamentosTab({ funcionarioId }: { funcionarioId: string }) {
  const { apiClient } = useAuth()
  const { data, isLoading } = useAfastamentosFuncionario(apiClient, funcionarioId)
  const createMutation = useCreateAfastamentoFuncionario(apiClient)
  const [adding, setAdding] = useState(false)
  const [form, setForm] = useState<AfastamentoFuncionario>({
    id: '', funcionarioId, tipo: '', dataInicio: '',
  })

  const columns: Column<AfastamentoFuncionario>[] = [
    { key: 'tipo', header: 'Tipo', render: (item) => <Badge variant="outline">{item.tipo}</Badge> },
    { key: 'dataInicio', header: 'Início', render: (item) => formatDate(item.dataInicio) },
    { key: 'dataFimPrevista', header: 'Fim Previsto', render: (item) => item.dataFimPrevista ? formatDate(item.dataFimPrevista) : '—' },
    { key: 'dataFimEfetiva', header: 'Fim Efetivo', render: (item) => item.dataFimEfetiva ? formatDate(item.dataFimEfetiva) : '—' },
    { key: 'numeroAtestadoCid', header: 'CID', render: (item) => item.numeroAtestadoCid || '—' },
  ]

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle className="text-base">Afastamentos</CardTitle>
        <Button size="sm" onClick={() => setAdding(!adding)}><Plus className="mr-1 h-4 w-4" />Novo</Button>
      </CardHeader>
      <CardContent className="space-y-4">
        {adding && (
          <div className="rounded-lg border p-4">
            <FormContainer mode="create" onSubmit={(e) => { e.preventDefault(); createMutation.mutateAsync({ funcionarioId, data: form }).then(() => { toast.success('Afastamento registrado!'); setAdding(false) }).catch(() => toast.error('Erro.')) }} onCancel={() => setAdding(false)} isSubmitting={createMutation.isPending} submitLabel="Registrar">
              <AfastamentoFormFields data={form as any} onChange={(d) => setForm(d as AfastamentoFuncionario)} mode="create" />
            </FormContainer>
          </div>
        )}
        <DataTable columns={columns} rows={data ?? []} isLoading={isLoading} emptyMessage="Nenhum afastamento." />
      </CardContent>
    </Card>
  )
}

// ---- e-Social Tab ----

function ESocialTab({ funcionarioId }: { funcionarioId: string }) {
  const { apiClient } = useAuth()
  const { data, isLoading } = useInfoESocialFuncionario(apiClient, funcionarioId)
  const updateMutation = useUpdateInfoESocialFuncionario(apiClient)
  const [form, setForm] = useState<InfoESocialFuncionario>({
    id: '', funcionarioId, indicadorDeficiencia: false, reservista: false, primeiroEmprego: false, trabalhadorAposentado: false,
  })

  if (isLoading) return <Skeleton className="h-48 w-full" />
  if (data && form.id !== data.id) setForm(data)

  return (
    <Card>
      <CardHeader><CardTitle className="text-base">Informações e-Social</CardTitle></CardHeader>
      <CardContent>
        <FormContainer mode="edit" onSubmit={(e) => { e.preventDefault(); updateMutation.mutateAsync({ funcionarioId, data: form }).then(() => toast.success('Salvo!')).catch(() => toast.error('Erro.')) }} isSubmitting={updateMutation.isPending} submitLabel="Salvar">
          <InfoESocialFormFields data={form as any} onChange={(d) => setForm(d as InfoESocialFuncionario)} mode="edit" />
        </FormContainer>
      </CardContent>
    </Card>
  )
}

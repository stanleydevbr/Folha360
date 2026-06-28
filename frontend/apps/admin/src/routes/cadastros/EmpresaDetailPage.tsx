import { useState, useCallback } from 'react'
import { useParams, useNavigate } from 'react-router'
import { useAuth } from '@/providers/AuthProvider'
import {
  useEmpresa,
  useUpdateEmpresa,
  useEnderecosEmpresa,
  useCreateEnderecoEmpresa,
  useDeleteEnderecoEmpresa,
  useContatosEmpresa,
  useCreateContatoEmpresa,
  useDeleteContatoEmpresa,
  useConfigESocial,
  useUpdateConfigESocial,
  useConfigBancariaEmpresa,
  useCreateConfigBancariaEmpresa,
  useDeleteConfigBancariaEmpresa,
  useConfiguracoesGerais,
  useUpdateConfiguracoesGerais,
  useProcessosAdministrativos,
  useCreateProcessoAdministrativo,
  useDeleteProcessoAdministrativo,
  useBancos,
} from '@folha360/api'
import type {
  EmpresaDto,
  AtualizarEmpresaCommand,
  EnderecoEmpresa,
  ContatoEmpresa,
  ConfiguracaoESocial,
  ConfiguracaoBancariaEmpresa,
  ProcessoAdministrativo,
} from '@folha360/api'
import {
  DataTable,
  FormContainer,
  FormInput,
  FormSelect,
  FormGrid,
  FormCheckbox,
  FormTextarea,
  EmpresaFormFields,
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  Button,
  Badge,
  Skeleton,
} from '@folha360/ui'
import type { Column } from '@folha360/ui'
import { formatCnpj, formatDate, formatCurrency } from '@folha360/utils'
import {
  ArrowLeft,
  Plus,
  Pencil,
  Trash2,
  Building2,
  MapPin,
  Phone,
  Shield,
  Landmark,
  Settings,
  Scale,
} from 'lucide-react'
import { toast } from 'sonner'

type Tab = 'dados' | 'enderecos' | 'contatos' | 'esocial' | 'bancarios' | 'config' | 'processos'

const TABS: { id: Tab; label: string; icon: React.ReactNode }[] = [
  { id: 'dados', label: 'Dados Gerais', icon: <Building2 className="h-4 w-4" /> },
  { id: 'enderecos', label: 'Endereços', icon: <MapPin className="h-4 w-4" /> },
  { id: 'contatos', label: 'Contatos', icon: <Phone className="h-4 w-4" /> },
  { id: 'esocial', label: 'e-Social', icon: <Shield className="h-4 w-4" /> },
  { id: 'bancarios', label: 'Bancário', icon: <Landmark className="h-4 w-4" /> },
  { id: 'config', label: 'Configurações', icon: <Settings className="h-4 w-4" /> },
  { id: 'processos', label: 'Processos', icon: <Scale className="h-4 w-4" /> },
]

export default function EmpresaDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { apiClient } = useAuth()
  const [tab, setTab] = useState<Tab>('dados')
  const [editing, setEditing] = useState(false)

  const { data: empresa, isLoading } = useEmpresa(apiClient, id)
  const updateMutation = useUpdateEmpresa(apiClient)

  if (isLoading || !empresa) {
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
        <Button variant="ghost" size="sm" onClick={() => navigate('/cadastros/empresas')}>
          <ArrowLeft className="mr-1 h-4 w-4" /> Voltar
        </Button>
        <div className="flex-1">
          <h1 className="text-2xl font-bold">{empresa.razaoSocial}</h1>
          <p className="text-sm text-muted-foreground">
            {formatCnpj(empresa.cnpj)} · {empresa.regimeTributario}
          </p>
        </div>
        <Button onClick={() => setEditing(!editing)} variant="outline">
          <Pencil className="mr-2 h-4 w-4" />
          {editing ? 'Cancelar' : 'Editar'}
        </Button>
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
        {tab === 'dados' && (
          <DadosTab empresa={empresa} editing={editing} onSave={(data) => updateMutation.mutateAsync({ id: empresa.id, data }).then(() => { toast.success('Empresa atualizada!'); setEditing(false) }).catch(() => toast.error('Erro ao salvar.'))} isSaving={updateMutation.isPending} />
        )}
        {tab === 'enderecos' && <EnderecosTab empresaId={empresa.id} />}
        {tab === 'contatos' && <ContatosTab empresaId={empresa.id} />}
        {tab === 'esocial' && <ESocialTab empresaId={empresa.id} />}
        {tab === 'bancarios' && <BancariosTab empresaId={empresa.id} />}
        {tab === 'config' && <ConfigTab empresaId={empresa.id} />}
        {tab === 'processos' && <ProcessosTab empresaId={empresa.id} />}
      </div>
    </div>
  )
}

// ---- Dados Gerais Tab ----

function DadosTab({
  empresa,
  editing,
  onSave,
  isSaving,
}: {
  empresa: EmpresaDto
  editing: boolean
  onSave: (data: AtualizarEmpresaCommand) => void
  isSaving: boolean
}) {
  const [form, setForm] = useState({
    razaoSocial: empresa.razaoSocial,
    nomeFantasia: empresa.nomeFantasia ?? '',
    cnae: empresa.cnae ?? '',
    regimeTributario: empresa.regimeTributario,
    fpas: empresa.fpas ?? '',
    codigoTerceiros: empresa.codigoTerceiros ?? '',
    classificacaoTributaria: empresa.classificacaoTributaria ?? '',
    matrizFilial: empresa.matrizFilial ?? '',
    cnpjMatriz: empresa.cnpjMatriz ?? '',
    enderecoLogradouro: empresa.enderecoLogradouro ?? '',
    enderecoNumero: empresa.enderecoNumero ?? '',
    enderecoComplemento: empresa.enderecoComplemento ?? '',
    enderecoBairro: empresa.enderecoBairro ?? '',
    enderecoCep: empresa.enderecoCep ?? '',
    enderecoMunicipio: empresa.enderecoMunicipio ?? '',
    enderecoUf: empresa.enderecoUf ?? '',
    telefone: empresa.telefone ?? '',
    email: empresa.email ?? '',
  })

  if (!editing) {
    return (
      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
        <Card>
          <CardHeader><CardTitle className="text-base">Identificação</CardTitle></CardHeader>
          <CardContent className="grid grid-cols-1 gap-3 sm:grid-cols-2">
            <Info label="CNPJ" value={formatCnpj(empresa.cnpj)} />
            <Info label="Regime Tributário" value={empresa.regimeTributario} />
            <Info label="Razão Social" value={empresa.razaoSocial} />
            <Info label="Nome Fantasia" value={empresa.nomeFantasia || '—'} />
            <Info label="CNAE" value={empresa.cnae || '—'} />
            <Info label="Matriz/Filial" value={empresa.matrizFilial || '—'} />
          </CardContent>
        </Card>
        <Card>
          <CardHeader><CardTitle className="text-base">Endereço & Contato</CardTitle></CardHeader>
          <CardContent className="grid grid-cols-1 gap-3 sm:grid-cols-2">
            <Info label="Logradouro" value={empresa.enderecoLogradouro || '—'} />
            <Info label="Número" value={empresa.enderecoNumero || '—'} />
            <Info label="Bairro" value={empresa.enderecoBairro || '—'} />
            <Info label="CEP" value={empresa.enderecoCep || '—'} />
            <Info label="Município" value={empresa.enderecoMunicipio || '—'} />
            <Info label="UF" value={empresa.enderecoUf || '—'} />
            <Info label="Telefone" value={empresa.telefone || '—'} />
            <Info label="E-mail" value={empresa.email || '—'} />
          </CardContent>
        </Card>
      </div>
    )
  }

  return (
    <Card>
      <CardHeader><CardTitle className="text-base">Editar Dados da Empresa</CardTitle></CardHeader>
      <CardContent>
        <FormContainer mode="edit" onSubmit={(e) => { e.preventDefault(); onSave(form) }} onCancel={() => window.location.reload()} isSubmitting={isSaving} submitLabel="Salvar Alterações">
          <EmpresaFormFields data={{ ...form, cnpj: empresa.cnpj }} onChange={setForm} mode="edit" />
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

// ---- Endereços Tab ----

function EnderecosTab({ empresaId }: { empresaId: string }) {
  const { apiClient } = useAuth()
  const { data, isLoading } = useEnderecosEmpresa(apiClient, empresaId)
  const createMutation = useCreateEnderecoEmpresa(apiClient)
  const deleteMutation = useDeleteEnderecoEmpresa(apiClient)
  const [adding, setAdding] = useState(false)
  const [form, setForm] = useState<EnderecoEmpresa>({
    id: '', empresaId, tipo: '', logradouro: '', numero: '', complemento: '', bairro: '', cep: '', uf: '', estrangeiro: false,
  })

  const columns: Column<EnderecoEmpresa>[] = [
    { key: 'tipo', header: 'Tipo', render: (item) => <Badge variant="outline">{item.tipo}</Badge> },
    { key: 'logradouro', header: 'Logradouro' },
    { key: 'numero', header: 'Nº', render: (item) => item.numero || '—' },
    { key: 'bairro', header: 'Bairro', render: (item) => item.bairro || '—' },
    { key: 'cep', header: 'CEP', render: (item) => item.cep || '—' },
    { key: 'uf', header: 'UF', render: (item) => item.uf || '—' },
  ]

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle className="text-base">Endereços</CardTitle>
        <Button size="sm" onClick={() => setAdding(!adding)}><Plus className="mr-1 h-4 w-4" />Novo</Button>
      </CardHeader>
      <CardContent className="space-y-4">
        {adding && (
          <div className="rounded-lg border p-4">
            <FormContainer mode="create" onSubmit={(e) => { e.preventDefault(); createMutation.mutateAsync({ empresaId, data: form }).then(() => { toast.success('Endereço adicionado!'); setAdding(false); setForm({ id: '', empresaId, tipo: '', logradouro: '', numero: '', complemento: '', bairro: '', cep: '', uf: '', estrangeiro: false }) }).catch(() => toast.error('Erro.')) }} onCancel={() => setAdding(false)} isSubmitting={createMutation.isPending} submitLabel="Adicionar">
              <FormGrid cols={2}>
                <FormSelect id="tipo" label="Tipo" value={form.tipo} onChange={(e) => setForm({ ...form, tipo: e.target.value })} options={[{ value: '', label: 'Selecione...' }, { value: 'Principal', label: 'Principal' }, { value: 'Fiscal', label: 'Fiscal' }, { value: 'Cobranca', label: 'Cobrança' }, { value: 'Entrega', label: 'Entrega' }, { value: 'Obra', label: 'Obra' }]} required />
                <FormInput id="logradouro" label="Logradouro" value={form.logradouro} onChange={(e) => setForm({ ...form, logradouro: e.target.value })} required />
                <FormInput id="numero" label="Número" value={form.numero ?? ''} onChange={(e) => setForm({ ...form, numero: e.target.value })} />
                <FormInput id="complemento" label="Complemento" value={form.complemento ?? ''} onChange={(e) => setForm({ ...form, complemento: e.target.value })} />
                <FormInput id="bairro" label="Bairro" value={form.bairro ?? ''} onChange={(e) => setForm({ ...form, bairro: e.target.value })} />
                <FormInput id="cep" label="CEP" value={form.cep ?? ''} onChange={(e) => setForm({ ...form, cep: e.target.value })} />
                <FormInput id="uf" label="UF" value={form.uf ?? ''} onChange={(e) => setForm({ ...form, uf: e.target.value })} maxLength={2} />
              </FormGrid>
            </FormContainer>
          </div>
        )}
        <DataTable
          columns={columns}
          rows={data ?? []}
          isLoading={isLoading}
          emptyMessage="Nenhum endereço cadastrado."
          actions={(item) => (
            <Button variant="ghost" size="sm" onClick={() => { if (confirm('Remover este endereço?')) deleteMutation.mutateAsync({ empresaId, id: item.id }).then(() => toast.success('Removido!')).catch(() => toast.error('Erro.')) }}>
              <Trash2 className="h-4 w-4 text-destructive" />
            </Button>
          )}
        />
      </CardContent>
    </Card>
  )
}

// ---- Contatos Tab ----

function ContatosTab({ empresaId }: { empresaId: string }) {
  const { apiClient } = useAuth()
  const { data, isLoading } = useContatosEmpresa(apiClient, empresaId)
  const createMutation = useCreateContatoEmpresa(apiClient)
  const deleteMutation = useDeleteContatoEmpresa(apiClient)
  const [adding, setAdding] = useState(false)
  const [form, setForm] = useState<ContatoEmpresa>({
    id: '', empresaId, tipo: '', nome: '', cpf: '', cargo: '', email: '', telefone: '', celular: '', contatoPrincipal: false, ativo: true,
  })

  const columns: Column<ContatoEmpresa>[] = [
    { key: 'nome', header: 'Nome' },
    { key: 'tipo', header: 'Tipo', render: (item) => <Badge variant="outline">{item.tipo}</Badge> },
    { key: 'cargo', header: 'Cargo', render: (item) => item.cargo || '—' },
    { key: 'email', header: 'E-mail', render: (item) => item.email || '—' },
    { key: 'telefone', header: 'Telefone', render: (item) => item.telefone || item.celular || '—' },
    { key: 'contatoPrincipal', header: 'Principal', render: (item) => item.contatoPrincipal ? <Badge>Sim</Badge> : '—' },
  ]

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle className="text-base">Contatos</CardTitle>
        <Button size="sm" onClick={() => setAdding(!adding)}><Plus className="mr-1 h-4 w-4" />Novo</Button>
      </CardHeader>
      <CardContent className="space-y-4">
        {adding && (
          <div className="rounded-lg border p-4">
            <FormContainer mode="create" onSubmit={(e) => { e.preventDefault(); createMutation.mutateAsync({ empresaId, data: form }).then(() => { toast.success('Contato adicionado!'); setAdding(false) }).catch(() => toast.error('Erro.')) }} onCancel={() => setAdding(false)} isSubmitting={createMutation.isPending} submitLabel="Adicionar">
              <FormGrid cols={2}>
                <FormSelect id="tipo" label="Tipo" value={form.tipo} onChange={(e) => setForm({ ...form, tipo: e.target.value })} options={[{ value: '', label: 'Selecione...' }, { value: 'Diretor', label: 'Diretor' }, { value: 'Gerente', label: 'Gerente' }, { value: 'Socio', label: 'Sócio' }, { value: 'Contador', label: 'Contador' }, { value: 'RH', label: 'RH' }, { value: 'TI', label: 'TI' }, { value: 'Preposto', label: 'Preposto' }]} required />
                <FormInput id="nome" label="Nome" value={form.nome} onChange={(e) => setForm({ ...form, nome: e.target.value })} required />
                <FormInput id="cpf" label="CPF" value={form.cpf ?? ''} onChange={(e) => setForm({ ...form, cpf: e.target.value })} placeholder="000.000.000-00" maxLength={14} />
                <FormInput id="cargo" label="Cargo" value={form.cargo ?? ''} onChange={(e) => setForm({ ...form, cargo: e.target.value })} />
                <FormInput id="email" label="E-mail" type="email" value={form.email ?? ''} onChange={(e) => setForm({ ...form, email: e.target.value })} />
                <FormInput id="telefone" label="Telefone" value={form.telefone ?? ''} onChange={(e) => setForm({ ...form, telefone: e.target.value })} placeholder="(00) 0000-0000" />
                <FormInput id="celular" label="Celular" value={form.celular ?? ''} onChange={(e) => setForm({ ...form, celular: e.target.value })} placeholder="(00) 00000-0000" />
                <FormCheckbox id="contatoPrincipal" label="Contato Principal" checked={form.contatoPrincipal} onChange={(v) => setForm({ ...form, contatoPrincipal: v })} />
              </FormGrid>
            </FormContainer>
          </div>
        )}
        <DataTable columns={columns} rows={data ?? []} isLoading={isLoading} emptyMessage="Nenhum contato cadastrado."
          actions={(item) => (
            <Button variant="ghost" size="sm" onClick={() => { if (confirm('Remover este contato?')) deleteMutation.mutateAsync({ empresaId, id: item.id }).then(() => toast.success('Removido!')).catch(() => toast.error('Erro.')) }}>
              <Trash2 className="h-4 w-4 text-destructive" />
            </Button>
          )} />
      </CardContent>
    </Card>
  )
}

// ---- e-Social Tab ----

function ESocialTab({ empresaId }: { empresaId: string }) {
  const { apiClient } = useAuth()
  const { data, isLoading } = useConfigESocial(apiClient, empresaId)
  const updateMutation = useUpdateConfigESocial(apiClient)
  const [form, setForm] = useState<ConfiguracaoESocial>({
    id: '', empresaId, ambiente: '', certificadoDigitalTipo: '', versaoLayout: '', codigoTransmissor: '', grupoEsocial: '',
  })

  if (isLoading) return <Skeleton className="h-48 w-full" />

  // Sync form when data loads
  if (data && form.id !== data.id) setForm(data)

  return (
    <Card>
      <CardHeader><CardTitle className="text-base">Configuração e-Social</CardTitle></CardHeader>
      <CardContent>
        <FormContainer mode="edit" onSubmit={(e) => { e.preventDefault(); updateMutation.mutateAsync({ empresaId, data: form }).then(() => toast.success('Configuração salva!')).catch(() => toast.error('Erro.')) }} isSubmitting={updateMutation.isPending} submitLabel="Salvar">
          <FormGrid cols={2}>
            <FormSelect id="ambiente" label="Ambiente" value={form.ambiente} onChange={(e) => setForm({ ...form, ambiente: e.target.value })} options={[{ value: '', label: 'Selecione...' }, { value: 'Producao', label: 'Produção' }, { value: 'ProducaoRestrita', label: 'Produção Restrita' }]} required />
            <FormSelect id="certificadoDigitalTipo" label="Tipo Certificado" value={form.certificadoDigitalTipo ?? ''} onChange={(e) => setForm({ ...form, certificadoDigitalTipo: e.target.value })} options={[{ value: '', label: 'Selecione...' }, { value: 'A1', label: 'A1 (Arquivo)' }, { value: 'A3', label: 'A3 (Token)' }]} />
            <FormInput id="versaoLayout" label="Versão Layout" value={form.versaoLayout ?? ''} onChange={(e) => setForm({ ...form, versaoLayout: e.target.value })} />
            <FormInput id="codigoTransmissor" label="Código Transmissor" value={form.codigoTransmissor ?? ''} onChange={(e) => setForm({ ...form, codigoTransmissor: e.target.value })} />
            <FormInput id="grupoEsocial" label="Grupo e-Social" value={form.grupoEsocial ?? ''} onChange={(e) => setForm({ ...form, grupoEsocial: e.target.value })} />
          </FormGrid>
        </FormContainer>
      </CardContent>
    </Card>
  )
}

// ---- Bancários Tab ----

function BancariosTab({ empresaId }: { empresaId: string }) {
  const { apiClient } = useAuth()
  const { data, isLoading } = useConfigBancariaEmpresa(apiClient, empresaId)
  const { data: bancosData } = useBancos(apiClient)
  const createMutation = useCreateConfigBancariaEmpresa(apiClient)
  const deleteMutation = useDeleteConfigBancariaEmpresa(apiClient)
  const [adding, setAdding] = useState(false)
  const [form, setForm] = useState<ConfiguracaoBancariaEmpresa>({
    id: '', empresaId, bancoId: '', agencia: '', agenciaDv: '', conta: '', contaDv: '', tipoConta: '', chavePix: '', finalidade: '', ativa: true,
  })

  const bancoOptions = [{ value: '', label: 'Selecione...' }, ...(bancosData ?? []).map((b) => ({ value: b.id, label: `${b.codigo} - ${b.nome}` }))]

  const columns: Column<ConfiguracaoBancariaEmpresa>[] = [
    { key: 'bancoId', header: 'Banco' },
    { key: 'agencia', header: 'Agência' },
    { key: 'conta', header: 'Conta' },
    { key: 'tipoConta', header: 'Tipo', render: (item) => <Badge variant="outline">{item.tipoConta}</Badge> },
    { key: 'finalidade', header: 'Finalidade', render: (item) => <Badge variant="outline">{item.finalidade}</Badge> },
    { key: 'ativa', header: 'Ativa', render: (item) => item.ativa ? <Badge>Sim</Badge> : <Badge variant="secondary">Não</Badge> },
  ]

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle className="text-base">Configurações Bancárias</CardTitle>
        <Button size="sm" onClick={() => setAdding(!adding)}><Plus className="mr-1 h-4 w-4" />Nova</Button>
      </CardHeader>
      <CardContent className="space-y-4">
        {adding && (
          <div className="rounded-lg border p-4">
            <FormContainer mode="create" onSubmit={(e) => { e.preventDefault(); createMutation.mutateAsync({ empresaId, data: form }).then(() => { toast.success('Conta adicionada!'); setAdding(false) }).catch(() => toast.error('Erro.')) }} onCancel={() => setAdding(false)} isSubmitting={createMutation.isPending} submitLabel="Adicionar">
              <FormGrid cols={2}>
                <FormSelect id="bancoId" label="Banco" value={form.bancoId} onChange={(e) => setForm({ ...form, bancoId: e.target.value })} options={bancoOptions} required />
                <FormSelect id="tipoConta" label="Tipo Conta" value={form.tipoConta} onChange={(e) => setForm({ ...form, tipoConta: e.target.value })} options={[{ value: '', label: 'Selecione...' }, { value: 'Corrente', label: 'Corrente' }, { value: 'Poupanca', label: 'Poupança' }]} required />
                <FormInput id="agencia" label="Agência" value={form.agencia} onChange={(e) => setForm({ ...form, agencia: e.target.value })} required />
                <FormInput id="agenciaDv" label="DV Agência" value={form.agenciaDv ?? ''} onChange={(e) => setForm({ ...form, agenciaDv: e.target.value })} maxLength={1} />
                <FormInput id="conta" label="Conta" value={form.conta} onChange={(e) => setForm({ ...form, conta: e.target.value })} required />
                <FormInput id="contaDv" label="DV Conta" value={form.contaDv ?? ''} onChange={(e) => setForm({ ...form, contaDv: e.target.value })} maxLength={1} />
                <FormSelect id="finalidade" label="Finalidade" value={form.finalidade} onChange={(e) => setForm({ ...form, finalidade: e.target.value })} options={[{ value: '', label: 'Selecione...' }, { value: 'Folha', label: 'Folha' }, { value: 'Tributos', label: 'Tributos' }, { value: 'Fornecedor', label: 'Fornecedor' }, { value: 'Geral', label: 'Geral' }]} required />
                <FormInput id="chavePix" label="Chave PIX" value={form.chavePix ?? ''} onChange={(e) => setForm({ ...form, chavePix: e.target.value })} />
                <FormCheckbox id="ativa" label="Ativa" checked={form.ativa} onChange={(v) => setForm({ ...form, ativa: v })} />
              </FormGrid>
            </FormContainer>
          </div>
        )}
        <DataTable columns={columns} rows={data ?? []} isLoading={isLoading} emptyMessage="Nenhuma conta bancária."
          actions={(item) => (
            <Button variant="ghost" size="sm" onClick={() => { if (confirm('Remover?')) deleteMutation.mutateAsync({ empresaId, id: item.id }).then(() => toast.success('Removida!')).catch(() => toast.error('Erro.')) }}>
              <Trash2 className="h-4 w-4 text-destructive" />
            </Button>
          )} />
      </CardContent>
    </Card>
  )
}

// ---- Configurações Gerais Tab ----

function ConfigTab({ empresaId }: { empresaId: string }) {
  const { apiClient } = useAuth()
  const { data, isLoading } = useConfiguracoesGerais(apiClient, empresaId)
  const updateMutation = useUpdateConfiguracoesGerais(apiClient)
  const [values, setValues] = useState<Record<string, string>>({})

  if (isLoading) return <Skeleton className="h-48 w-full" />

  // Sync from server
  const configMap: Record<string, string> = {}
  data?.forEach((c) => { configMap[c.chave] = c.valor })
  if (Object.keys(values).length === 0 && Object.keys(configMap).length > 0) setValues(configMap)

  const defaultKeys = ['DiaPagamento', 'PercentualAdiantamento', 'PercentualVT', 'AbonoPecuniario', 'ToleranciaPonto']

  return (
    <Card>
      <CardHeader><CardTitle className="text-base">Configurações Gerais</CardTitle></CardHeader>
      <CardContent>
        <FormContainer mode="edit" onSubmit={(e) => { e.preventDefault(); updateMutation.mutateAsync({ empresaId, data: values }).then(() => toast.success('Salvo!')).catch(() => toast.error('Erro.')) }} isSubmitting={updateMutation.isPending} submitLabel="Salvar">
          <FormGrid cols={2}>
            {defaultKeys.map((key) => (
              <FormInput
                key={key}
                id={key}
                label={key.replace(/([A-Z])/g, ' $1').trim()}
                value={values[key] ?? configMap[key] ?? ''}
                onChange={(e) => setValues({ ...values, [key]: e.target.value })}
              />
            ))}
          </FormGrid>
        </FormContainer>
      </CardContent>
    </Card>
  )
}

// ---- Processos Tab ----

function ProcessosTab({ empresaId }: { empresaId: string }) {
  const { apiClient } = useAuth()
  const { data, isLoading } = useProcessosAdministrativos(apiClient, empresaId)
  const createMutation = useCreateProcessoAdministrativo(apiClient)
  const deleteMutation = useDeleteProcessoAdministrativo(apiClient)
  const [adding, setAdding] = useState(false)
  const [form, setForm] = useState<ProcessoAdministrativo>({
    id: '', empresaId, numeroProcesso: '', tipo: '', orgao: '', observacao: '',
  })

  const columns: Column<ProcessoAdministrativo>[] = [
    { key: 'numeroProcesso', header: 'Nº Processo' },
    { key: 'tipo', header: 'Tipo', render: (item) => <Badge variant="outline">{item.tipo}</Badge> },
    { key: 'orgao', header: 'Órgão', render: (item) => item.orgao || '—' },
    { key: 'dataInicio', header: 'Início', render: (item) => item.dataInicio ? formatDate(item.dataInicio) : '—' },
  ]

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle className="text-base">Processos Administrativos / Judiciais</CardTitle>
        <Button size="sm" onClick={() => setAdding(!adding)}><Plus className="mr-1 h-4 w-4" />Novo</Button>
      </CardHeader>
      <CardContent className="space-y-4">
        {adding && (
          <div className="rounded-lg border p-4">
            <FormContainer mode="create" onSubmit={(e) => { e.preventDefault(); createMutation.mutateAsync(form).then(() => { toast.success('Processo adicionado!'); setAdding(false) }).catch(() => toast.error('Erro.')) }} onCancel={() => setAdding(false)} isSubmitting={createMutation.isPending} submitLabel="Adicionar">
              <FormGrid cols={2}>
                <FormInput id="numeroProcesso" label="Número do Processo" value={form.numeroProcesso} onChange={(e) => setForm({ ...form, numeroProcesso: e.target.value })} required />
                <FormSelect id="tipo" label="Tipo" value={form.tipo} onChange={(e) => setForm({ ...form, tipo: e.target.value })} options={[{ value: '', label: 'Selecione...' }, { value: 'Administrativo', label: 'Administrativo' }, { value: 'Judicial', label: 'Judicial' }]} required />
                <FormInput id="orgao" label="Órgão" value={form.orgao ?? ''} onChange={(e) => setForm({ ...form, orgao: e.target.value })} />
                <FormTextarea id="observacao" label="Observações" value={form.observacao ?? ''} onChange={(e) => setForm({ ...form, observacao: e.target.value })} className="sm:col-span-2" />
              </FormGrid>
            </FormContainer>
          </div>
        )}
        <DataTable columns={columns} rows={data ?? []} isLoading={isLoading} emptyMessage="Nenhum processo cadastrado."
          actions={(item) => (
            <Button variant="ghost" size="sm" onClick={() => { if (confirm('Remover?')) deleteMutation.mutateAsync(item.id).then(() => toast.success('Removido!')).catch(() => toast.error('Erro.')) }}>
              <Trash2 className="h-4 w-4 text-destructive" />
            </Button>
          )} />
      </CardContent>
    </Card>
  )
}

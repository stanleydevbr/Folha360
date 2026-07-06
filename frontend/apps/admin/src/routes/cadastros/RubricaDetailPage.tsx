import { useState } from 'react'
import { useParams, useNavigate } from 'react-router'
import { useAuth } from '@/providers/AuthProvider'
import {
  useRubrica,
  useUpdateRubrica,
  useGruposRubrica,
  useCreateGrupoRubrica,
  useComposicaoRubrica,
  useAddComponente,
  useFormulaRubrica,
  useUpdateFormulaRubrica,
  useTabelasProgressivas,
  useCreateFaixaProgressiva,
  useIncidenciasRubrica,
  useAddIncidencia,
  useSimularRubrica,
} from '@folha360/api'
import type {
  RubricaDto,
  CriarRubricaCommand,
  GrupoRubricaDto,
  CriarGrupoRubricaCommand,
  RubricaComposicaoDto,
  AdicionarComponenteCommand,
  RubricaFormulaDto,
  AtualizarRubricaFormulaCommand,
  RubricaTabelaProgressivaDto,
  CriarFaixaProgressivaCommand,
  RubricaIncidenciaDto,
  AdicionarIncidenciaCommand,
  SimularRubricaCommand,
  SimulacaoResultadoDto,
} from '@folha360/api'
import {
  DataTable,
  FormContainer,
  FormInput,
  FormSelect,
  FormGrid,
  FormCheckbox,
  FormTextarea,
  RubricaFormFields,
  GrupoRubricaFormFields,
  FaixaProgressivaFormFields,
  SimulacaoRubricaFormFields,
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  Button,
  Badge,
  Skeleton,
  TIPO_INCIDENCIA_OPTIONS,
} from '@folha360/ui'
import type { Column } from '@folha360/ui'
import { formatCurrency, formatDate } from '@folha360/utils'
import {
  ArrowLeft,
  Plus,
  Trash2,
  Calculator,
  Layers,
  Sigma,
  BarChart3,
  Shield,
  FlaskConical,
  Pencil,
} from 'lucide-react'
import { toast } from 'sonner'

type Tab = 'dados' | 'grupos' | 'composicao' | 'formulas' | 'tabelas' | 'incidencias' | 'simulacao'

const TABS: { id: Tab; label: string; icon: React.ReactNode }[] = [
  { id: 'dados', label: 'Dados Gerais', icon: <Pencil className="h-4 w-4" /> },
  { id: 'grupos', label: 'Grupos', icon: <Layers className="h-4 w-4" /> },
  { id: 'composicao', label: 'Composição', icon: <Calculator className="h-4 w-4" /> },
  { id: 'formulas', label: 'Fórmulas', icon: <Sigma className="h-4 w-4" /> },
  { id: 'tabelas', label: 'Tabelas Progressivas', icon: <BarChart3 className="h-4 w-4" /> },
  { id: 'incidencias', label: 'Incidências', icon: <Shield className="h-4 w-4" /> },
  { id: 'simulacao', label: 'Simulação', icon: <FlaskConical className="h-4 w-4" /> },
]

// ---- dtoToForm / formToDto helpers ----

type FormData = {
  empresaId: string; grupoRubricaId: string; codigo: string; descricao: string; descricaoAbreviada: string
  natureza: string; tipoEsocial: string; enviarEsocial: boolean; tipoCalculo: string
  formulaCalculo: string; valorFixo: string; percentual: string; rubricaBaseId: string
  ordemCalculo: string; ordemExibicao: string; prioridadeDesconto: string
  tetoMaximo: string; pisoMinimo: string; ativo: boolean; dataInicioVigencia: string; dataFimVigencia: string; observacao: string
  incideInss: boolean; incideIrrf: boolean; incideFgts: boolean
  incideContribuicaoSindical: boolean; incideDecimoTerceiro: boolean; incideFerias: boolean
  incideAvisoPrevio: boolean; incideRescisao: boolean; incideDissidio: boolean
  incideSalarioMaternidade: boolean; incideAuxilioDoenca: boolean; incideAdiantamento: boolean
}

function dtoToForm(item: RubricaDto): FormData {
  return {
    empresaId: item.empresaId, grupoRubricaId: item.grupoRubricaId ?? '', codigo: item.codigo,
    descricao: item.descricao, descricaoAbreviada: item.descricaoAbreviada ?? '',
    natureza: item.natureza, tipoEsocial: item.tipoEsocial ?? '', enviarEsocial: item.enviarEsocial,
    tipoCalculo: item.tipoCalculo, formulaCalculo: item.formulaCalculo ?? '',
    valorFixo: item.valorFixo ? String(item.valorFixo) : '', percentual: item.percentual ? String(item.percentual) : '',
    rubricaBaseId: item.rubricaBaseId ?? '',
    ordemCalculo: String(item.ordemCalculo), ordemExibicao: String(item.ordemExibicao),
    prioridadeDesconto: item.prioridadeDesconto ? String(item.prioridadeDesconto) : '',
    tetoMaximo: item.tetoMaximo ? String(item.tetoMaximo) : '', pisoMinimo: item.pisoMinimo ? String(item.pisoMinimo) : '',
    ativo: item.ativo, dataInicioVigencia: item.dataInicioVigencia ?? '', dataFimVigencia: item.dataFimVigencia ?? '',
    observacao: item.observacao ?? '',
    incideInss: item.incideInss, incideIrrf: item.incideIrrf, incideFgts: item.incideFgts,
    incideContribuicaoSindical: item.incideContribuicaoSindical, incideDecimoTerceiro: item.incideDecimoTerceiro,
    incideFerias: item.incideFerias, incideAvisoPrevio: item.incideAvisoPrevio,
    incideRescisao: item.incideRescisao, incideDissidio: item.incideDissidio,
    incideSalarioMaternidade: item.incideSalarioMaternidade, incideAuxilioDoenca: item.incideAuxilioDoenca,
    incideAdiantamento: item.incideAdiantamento,
  }
}

function formToDto(form: FormData): CriarRubricaCommand {
  return {
    ...form,
    valorFixo: form.valorFixo ? Number(form.valorFixo) : undefined,
    percentual: form.percentual ? Number(form.percentual) : undefined,
    tetoMaximo: form.tetoMaximo ? Number(form.tetoMaximo) : undefined,
    pisoMinimo: form.pisoMinimo ? Number(form.pisoMinimo) : undefined,
    ordemCalculo: Number(form.ordemCalculo),
    ordemExibicao: Number(form.ordemExibicao),
    prioridadeDesconto: form.prioridadeDesconto ? Number(form.prioridadeDesconto) : undefined,
  }
}

export default function RubricaDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { apiClient } = useAuth()
  const [tab, setTab] = useState<Tab>('dados')

  const { data: rubrica, isLoading } = useRubrica(apiClient, id)

  if (isLoading || !rubrica) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-8 w-64" />
        <Skeleton className="h-64 w-full" />
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Button variant="ghost" size="sm" onClick={() => navigate('/cadastros/rubricas')}>
          <ArrowLeft className="mr-1 h-4 w-4" /> Voltar
        </Button>
        <div className="flex-1">
          <h1 className="text-2xl font-bold">{rubrica.codigo} — {rubrica.descricao}</h1>
          <p className="text-sm text-muted-foreground">
            Natureza: <Badge variant="outline" className="ml-1">{rubrica.natureza}</Badge>
            {' · '}Tipo Cálculo: {rubrica.tipoCalculo}
            {' · '}{rubrica.ativo ? 'Ativo' : 'Inativo'}
          </p>
        </div>
      </div>

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

      <div className="min-h-[400px]">
        {tab === 'dados' && <DadosTab rubrica={rubrica} />}
        {tab === 'grupos' && <GruposTab empresaId={rubrica.empresaId} />}
        {tab === 'composicao' && <ComposicaoTab rubricaId={rubrica.id} />}
        {tab === 'formulas' && <FormulasTab rubricaId={rubrica.id} />}
        {tab === 'tabelas' && <TabelasTab rubricaId={rubrica.id} />}
        {tab === 'incidencias' && <IncidenciasTab rubricaId={rubrica.id} />}
        {tab === 'simulacao' && <SimulacaoTab empresaId={rubrica.empresaId} />}
      </div>
    </div>
  )
}

// ---- Dados Gerais Tab ----

function DadosTab({ rubrica }: { rubrica: RubricaDto }) {
  const { apiClient } = useAuth()
  const updateMutation = useUpdateRubrica(apiClient)
  const [editing, setEditing] = useState(false)
  const [form, setForm] = useState<FormData>(dtoToForm(rubrica))

  if (!editing) {
    return (
      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle className="text-base">Dados Básicos</CardTitle>
            <Button size="sm" variant="outline" onClick={() => setEditing(true)}><Pencil className="mr-1 h-3 w-3" />Editar</Button>
          </CardHeader>
          <CardContent className="grid grid-cols-2 gap-3">
            <Info label="Código" value={rubrica.codigo} />
            <Info label="Descrição" value={rubrica.descricao} />
            <Info label="Natureza" value={rubrica.natureza} />
            <Info label="Tipo Cálculo" value={rubrica.tipoCalculo} />
            <Info label="Ordem Cálculo" value={String(rubrica.ordemCalculo)} />
            <Info label="Ordem Exibição" value={String(rubrica.ordemExibicao)} />
            <Info label="Ativo" value={rubrica.ativo ? 'Sim' : 'Não'} />
          </CardContent>
        </Card>
        <Card>
          <CardHeader><CardTitle className="text-base">Valores e Incidências</CardTitle></CardHeader>
          <CardContent className="grid grid-cols-2 gap-3">
            <Info label="Valor Fixo" value={rubrica.valorFixo ? formatCurrency(rubrica.valorFixo) : '—'} />
            <Info label="Percentual" value={rubrica.percentual ? `${rubrica.percentual}%` : '—'} />
            <Info label="Teto Máximo" value={rubrica.tetoMaximo ? formatCurrency(rubrica.tetoMaximo) : '—'} />
            <Info label="Piso Mínimo" value={rubrica.pisoMinimo ? formatCurrency(rubrica.pisoMinimo) : '—'} />
            <Info label="INSS" value={rubrica.incideInss ? 'Sim' : 'Não'} />
            <Info label="IRRF" value={rubrica.incideIrrf ? 'Sim' : 'Não'} />
            <Info label="FGTS" value={rubrica.incideFgts ? 'Sim' : 'Não'} />
            <Info label="13º" value={rubrica.incideDecimoTerceiro ? 'Sim' : 'Não'} />
          </CardContent>
        </Card>
      </div>
    )
  }

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle className="text-base">Editar Rubrica</CardTitle>
        <Button size="sm" variant="outline" onClick={() => setEditing(false)}>Cancelar</Button>
      </CardHeader>
      <CardContent>
        <FormContainer mode="edit" onSubmit={(e) => { e.preventDefault(); updateMutation.mutateAsync({ id: rubrica.id, data: formToDto(form) }).then(() => { toast.success('Atualizada!'); setEditing(false) }).catch(() => toast.error('Erro.')) }} isSubmitting={updateMutation.isPending} submitLabel="Salvar">
          <RubricaFormFields data={form as any} onChange={(d: any) => setForm(d as FormData)} mode="edit" grupos={[]} rubricasBase={[]} />
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

// ---- Grupos Tab ----

function GruposTab({ empresaId }: { empresaId: string }) {
  const { apiClient } = useAuth()
  const { data, isLoading } = useGruposRubrica(apiClient, empresaId)
  const createMutation = useCreateGrupoRubrica(apiClient)
  const [adding, setAdding] = useState(false)
  const [form, setForm] = useState<CriarGrupoRubricaCommand>({
    empresaId, codigo: '', descricao: '', natureza: '', ordemExibicao: 0,
  })

  const columns: Column<GrupoRubricaDto>[] = [
    { key: 'codigo', header: 'Código' },
    { key: 'descricao', header: 'Descrição' },
    { key: 'natureza', header: 'Natureza', render: (item) => <Badge variant="outline">{item.natureza}</Badge> },
    { key: 'ordemExibicao', header: 'Ordem', render: (item) => String(item.ordemExibicao) },
  ]

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle className="text-base">Grupos de Rubrica</CardTitle>
        <Button size="sm" onClick={() => setAdding(!adding)}><Plus className="mr-1 h-4 w-4" />Novo Grupo</Button>
      </CardHeader>
      <CardContent className="space-y-4">
        {adding && (
          <div className="rounded-lg border p-4">
            <FormContainer mode="create" onSubmit={(e) => { e.preventDefault(); createMutation.mutateAsync(form).then(() => { toast.success('Grupo criado!'); setAdding(false) }).catch(() => toast.error('Erro.')) }} onCancel={() => setAdding(false)} isSubmitting={createMutation.isPending} submitLabel="Criar Grupo">
              <GrupoRubricaFormFields data={{ ...form, ordemExibicao: String(form.ordemExibicao) }} onChange={(d) => setForm({ ...d, empresaId, ordemExibicao: Number(d.ordemExibicao) || 0 })} mode="create" />
            </FormContainer>
          </div>
        )}
        <DataTable columns={columns} rows={data?.items ?? []} isLoading={isLoading} emptyMessage="Nenhum grupo de rubrica." />
      </CardContent>
    </Card>
  )
}

// ---- Composição Tab ----

function ComposicaoTab({ rubricaId }: { rubricaId: string }) {
  const { apiClient } = useAuth()
  const { data, isLoading } = useComposicaoRubrica(apiClient, rubricaId)
  const addMutation = useAddComponente(apiClient)
  const [adding, setAdding] = useState(false)
  const [form, setForm] = useState<AdicionarComponenteCommand>({
    rubricaPrincipalId: rubricaId, rubricaComponenteId: '', operador: '+', ordem: 0, obrigatorio: true,
  })

  const columns: Column<RubricaComposicaoDto>[] = [
    { key: 'rubricaComponenteCodigo', header: 'Componente', render: (item) => item.rubricaComponenteCodigo || item.rubricaComponenteId },
    { key: 'operador', header: 'Operador', render: (item) => <Badge variant="outline">{item.operador}</Badge> },
    { key: 'percentualComposicao', header: '%', render: (item) => item.percentualComposicao ? `${item.percentualComposicao}%` : '—' },
    { key: 'ordem', header: 'Ordem', render: (item) => String(item.ordem) },
    { key: 'obrigatorio', header: 'Obrigatório', render: (item) => item.obrigatorio ? <Badge>Sim</Badge> : '—' },
  ]

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle className="text-base">Composição da Rubrica</CardTitle>
        <Button size="sm" onClick={() => setAdding(!adding)}><Plus className="mr-1 h-4 w-4" />Adicionar Componente</Button>
      </CardHeader>
      <CardContent className="space-y-4">
        {adding && (
          <div className="rounded-lg border p-4">
            <FormContainer mode="create" onSubmit={(e) => { e.preventDefault(); addMutation.mutateAsync({ rubricaId, data: form }).then(() => { toast.success('Componente adicionado!'); setAdding(false) }).catch(() => toast.error('Erro.')) }} onCancel={() => setAdding(false)} isSubmitting={addMutation.isPending} submitLabel="Adicionar">
              <FormGrid cols={2}>
                <FormInput id="rubricaComponenteId" label="ID da Rubrica Componente" value={form.rubricaComponenteId} onChange={(e) => setForm({ ...form, rubricaComponenteId: e.target.value })} required />
                <FormSelect id="operador" label="Operador" value={form.operador} onChange={(e) => setForm({ ...form, operador: e.target.value })} options={[{ value: '+', label: '+ (Soma)' }, { value: '-', label: '- (Subtrai)' }]} />
                <FormInput id="ordem" label="Ordem" type="number" value={String(form.ordem)} onChange={(e) => setForm({ ...form, ordem: Number(e.target.value) || 0 })} min="0" required />
                <FormInput id="percentualComposicao" label="% Composição" type="number" value={form.percentualComposicao ? String(form.percentualComposicao) : ''} onChange={(e) => setForm({ ...form, percentualComposicao: e.target.value ? Number(e.target.value) : undefined })} min="0" max="100" step="0.01" />
                <FormCheckbox id="obrigatorio" label="Obrigatório" checked={form.obrigatorio ?? false} onChange={(v) => setForm({ ...form, obrigatorio: v })} />
              </FormGrid>
            </FormContainer>
          </div>
        )}
        <DataTable columns={columns} rows={data ?? []} isLoading={isLoading} emptyMessage="Nenhum componente na composição." />
      </CardContent>
    </Card>
  )
}

// ---- Fórmulas Tab ----

function FormulasTab({ rubricaId }: { rubricaId: string }) {
  const { apiClient } = useAuth()
  const { data, isLoading } = useFormulaRubrica(apiClient, rubricaId)
  const updateMutation = useUpdateFormulaRubrica(apiClient)
  const [form, setForm] = useState<AtualizarRubricaFormulaCommand>({
    rubricaId, expressao: '', parametros: '', descricaoFormal: '',
  })

  if (isLoading) return <Skeleton className="h-48 w-full" />
  if (data && form.expressao !== data.expressao) setForm({ rubricaId, expressao: data.expressao, parametros: data.parametros ?? '', descricaoFormal: data.descricaoFormal ?? '' })

  return (
    <Card>
      <CardHeader><CardTitle className="text-base">Fórmula de Cálculo (NCalc)</CardTitle></CardHeader>
      <CardContent>
        {data ? (
          <FormContainer mode="edit" onSubmit={(e) => { e.preventDefault(); updateMutation.mutateAsync({ rubricaId, data: form }).then(() => toast.success('Fórmula salva!')).catch(() => toast.error('Erro.')) }} isSubmitting={updateMutation.isPending} submitLabel="Salvar Fórmula">
            <div className="space-y-3">
              <FormTextarea id="expressao" label="Expressão NCalc" value={form.expressao} onChange={(e) => setForm({ ...form, expressao: e.target.value })} placeholder="Ex: SalarioBase * 0.08" required />
              <FormInput id="parametros" label="Parâmetros" value={form.parametros ?? ''} onChange={(e) => setForm({ ...form, parametros: e.target.value })} placeholder="SalarioBase, PercentualINSS" />
              <FormTextarea id="descricaoFormal" label="Descrição Formal" value={form.descricaoFormal ?? ''} onChange={(e) => setForm({ ...form, descricaoFormal: e.target.value })} placeholder="Fórmula: SB × 8%" />
              {data.versao > 0 && <p className="text-xs text-muted-foreground">Versão atual: {data.versao}</p>}
            </div>
          </FormContainer>
        ) : (
          <div className="text-center py-8 text-muted-foreground">
            <p>Nenhuma fórmula configurada.</p>
            <Button variant="outline" size="sm" className="mt-2" onClick={() => setForm({ rubricaId, expressao: '', parametros: '', descricaoFormal: '' })}>
              <Plus className="mr-1 h-4 w-4" /> Criar Fórmula
            </Button>
          </div>
        )}
      </CardContent>
    </Card>
  )
}

// ---- Tabelas Progressivas Tab ----

function TabelasTab({ rubricaId }: { rubricaId: string }) {
  const { apiClient } = useAuth()
  const { data, isLoading } = useTabelasProgressivas(apiClient, rubricaId)
  const createMutation = useCreateFaixaProgressiva(apiClient)
  const [adding, setAdding] = useState(false)
  const [form, setForm] = useState({
    rubricaId, anoVigencia: String(new Date().getFullYear()), faixaDe: '', faixaAte: '', aliquota: '', deducao: '', ordem: '0',
  })

  const columns: Column<RubricaTabelaProgressivaDto>[] = [
    { key: 'anoVigencia', header: 'Ano', render: (item) => String(item.anoVigencia) },
    { key: 'faixaDe', header: 'De (R$)', render: (item) => formatCurrency(item.faixaDe) },
    { key: 'faixaAte', header: 'Até (R$)', render: (item) => item.faixaAte ? formatCurrency(item.faixaAte) : '—' },
    { key: 'aliquota', header: 'Alíquota (%)', render: (item) => `${item.aliquota}%` },
    { key: 'deducao', header: 'Dedução (R$)', render: (item) => formatCurrency(item.deducao) },
    { key: 'ordem', header: 'Ordem', render: (item) => String(item.ordem) },
  ]

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle className="text-base">Tabelas Progressivas</CardTitle>
        <Button size="sm" onClick={() => setAdding(!adding)}><Plus className="mr-1 h-4 w-4" />Nova Faixa</Button>
      </CardHeader>
      <CardContent className="space-y-4">
        {adding && (
          <div className="rounded-lg border p-4">
            <FormContainer mode="create" onSubmit={(e) => { e.preventDefault(); createMutation.mutateAsync({ rubricaId, anoVigencia: Number(form.anoVigencia), faixaDe: Number(form.faixaDe), faixaAte: form.faixaAte ? Number(form.faixaAte) : undefined, aliquota: Number(form.aliquota), deducao: Number(form.deducao), ordem: Number(form.ordem) }).then(() => { toast.success('Faixa criada!'); setAdding(false) }).catch(() => toast.error('Erro.')) }} onCancel={() => setAdding(false)} isSubmitting={createMutation.isPending} submitLabel="Criar Faixa">
              <FaixaProgressivaFormFields data={form as any} onChange={(d: any) => setForm(d)} mode="create" />
            </FormContainer>
          </div>
        )}
        <DataTable columns={columns} rows={data ?? []} isLoading={isLoading} emptyMessage="Nenhuma faixa configurada." />
      </CardContent>
    </Card>
  )
}

// ---- Incidências Tab ----

function IncidenciasTab({ rubricaId }: { rubricaId: string }) {
  const { apiClient } = useAuth()
  const { data, isLoading } = useIncidenciasRubrica(apiClient, rubricaId)
  const addMutation = useAddIncidencia(apiClient)
  const [adding, setAdding] = useState(false)
  const [form, setForm] = useState<AdicionarIncidenciaCommand>({
    rubricaId, tipoIncidencia: '',
  })

  const columns: Column<RubricaIncidenciaDto>[] = [
    { key: 'tipoIncidencia', header: 'Tipo', render: (item) => <Badge variant="outline">{item.tipoIncidencia}</Badge> },
  ]

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle className="text-base">Incidências</CardTitle>
        <Button size="sm" onClick={() => setAdding(!adding)}><Plus className="mr-1 h-4 w-4" />Adicionar</Button>
      </CardHeader>
      <CardContent className="space-y-4">
        {adding && (
          <div className="rounded-lg border p-4">
            <FormContainer mode="create" onSubmit={(e) => { e.preventDefault(); addMutation.mutateAsync({ rubricaId, data: form }).then(() => { toast.success('Incidência adicionada!'); setAdding(false) }).catch(() => toast.error('Erro.')) }} onCancel={() => setAdding(false)} isSubmitting={addMutation.isPending} submitLabel="Adicionar">
              <FormSelect id="tipoIncidencia" label="Tipo de Incidência" value={form.tipoIncidencia} onChange={(e) => setForm({ ...form, tipoIncidencia: e.target.value })} options={TIPO_INCIDENCIA_OPTIONS} required />
            </FormContainer>
          </div>
        )}
        <DataTable columns={columns} rows={data ?? []} isLoading={isLoading} emptyMessage="Nenhuma incidência configurada." />
      </CardContent>
    </Card>
  )
}

// ---- Simulação Tab ----

function SimulacaoTab({ empresaId }: { empresaId: string }) {
  const { apiClient } = useAuth()
  const simulateMutation = useSimularRubrica(apiClient)
  const [form, setForm] = useState({
    empresaId, salarioBase: '', tipoContrato: '', quantidadeHoras: '', quantidadeDias: '', rubricasIds: [] as string[],
  })
  const [result, setResult] = useState<SimulacaoResultadoDto | null>(null)

  const handleSimulate = (e: React.FormEvent) => {
    e.preventDefault()
    const payload: SimularRubricaCommand = {
      empresaId: form.empresaId,
      salarioBase: Number(form.salarioBase),
      tipoContrato: form.tipoContrato || undefined,
      quantidadeHoras: form.quantidadeHoras ? Number(form.quantidadeHoras) : undefined,
      quantidadeDias: form.quantidadeDias ? Number(form.quantidadeDias) : undefined,
      rubricasIds: form.rubricasIds,
    }
    simulateMutation.mutateAsync(payload).then(setResult).catch(() => toast.error('Erro na simulação.'))
  }

  return (
    <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
      <Card>
        <CardHeader><CardTitle className="text-base">Parâmetros da Simulação</CardTitle></CardHeader>
        <CardContent>
          <FormContainer mode="create" onSubmit={handleSimulate} isSubmitting={simulateMutation.isPending} submitLabel="Simular">
            <SimulacaoRubricaFormFields data={form as any} onChange={(d: any) => setForm(d)} />
          </FormContainer>
        </CardContent>
      </Card>

      {result && (
        <Card>
          <CardHeader><CardTitle className="text-base">Resultado</CardTitle></CardHeader>
          <CardContent className="space-y-3">
            <div className="grid grid-cols-2 gap-3">
              <Info label="Total Vencimentos" value={formatCurrency(result.totalVencimentos)} />
              <Info label="Total Descontos" value={formatCurrency(result.totalDescontos)} />
              <Info label="Líquido" value={formatCurrency(result.liquido)} />
              <Info label="Base INSS" value={formatCurrency(result.baseInss)} />
              <Info label="Base IRRF" value={formatCurrency(result.baseIrrf)} />
              <Info label="Base FGTS" value={formatCurrency(result.baseFgts)} />
            </div>
            {result.erros.length > 0 && (
              <div className="rounded-lg border border-destructive/50 bg-destructive/10 p-3">
                <p className="text-sm font-medium text-destructive">Erros:</p>
                <ul className="mt-1 list-disc pl-4 text-xs text-destructive">
                  {result.erros.map((err, i) => <li key={i}>{err}</li>)}
                </ul>
              </div>
            )}
          </CardContent>
        </Card>
      )}
    </div>
  )
}

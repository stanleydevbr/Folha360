import { useState, useCallback } from 'react'
import { useAuth } from '@/providers/AuthProvider'
import {
  useAdmissoes, useCreateAdmissao,
  useDesligamentos, useCreateDesligamento,
  useFerias, useCreateFerias,
  useAfastamentos, useCreateAfastamento,
  useAlteracoesContratuais, useCreateAlteracaoContratual,
  useFuncionarios, useCargos,
} from '@folha360/api'
import type {
  AdmissaoDto, CriarAdmissaoCommand,
  DesligamentoDto, CriarDesligamentoCommand,
  FeriasDto, CriarFeriasCommand,
  AfastamentoDto, CriarAfastamentoCommand,
  AlteracaoContratualDto, CriarAlteracaoContratualCommand,
} from '@folha360/api'
import {
  DataTable,
  FormContainer,
  AdmissaoFormFields,
  DesligamentoFormFields,
  FeriasFormFields,
  AfastamentoEventoFormFields,
  AlteracaoContratualFormFields,
  Card,
  CardContent,
  Button,
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  Badge,
} from '@folha360/ui'
import type { Column } from '@folha360/ui'
import { formatDate, formatCurrency } from '@folha360/utils'
import { Plus } from 'lucide-react'
import { toast } from 'sonner'

type Tab = 'admissoes' | 'desligamentos' | 'ferias' | 'afastamentos' | 'alteracoes'

export default function EventosPage() {
  const { apiClient } = useAuth()
  const [tab, setTab] = useState<Tab>('admissoes')
  const [sheetOpen, setSheetOpen] = useState(false)

  const { data: funcionariosData } = useFuncionarios(apiClient, { pageSize: 500 })
  const { data: cargosData } = useCargos(apiClient)

  const funcionarioOptions = (funcionariosData?.items ?? []).map((f) => ({ value: f.id, label: f.nome }))
  const cargoOptions = (cargosData?.items ?? []).map((c) => ({ value: c.id, label: `${c.nome} (CBO: ${c.cbo})` }))

  // ---- Admissões ----
  const { data: admissoesData, isLoading: admLoading } = useAdmissoes(apiClient)
  const createAdmissao = useCreateAdmissao(apiClient)
  const [admForm, setAdmForm] = useState({
    funcionarioId: '', empresaId: '', dataAdmissao: '', cargoId: '', salarioInicial: '0', tipoContrato: '', periodoExperienciaMeses: '',
  })

  const admColumns: Column<AdmissaoDto>[] = [
    { key: 'dataAdmissao', header: 'Data', render: (item) => formatDate(item.dataAdmissao) },
    { key: 'tipoContrato', header: 'Tipo Contrato', render: (item) => <Badge variant="outline">{item.tipoContrato}</Badge> },
    { key: 'salarioInicial', header: 'Salário Inicial', render: (item) => formatCurrency(item.salarioInicial) },
  ]

  // ---- Desligamentos ----
  const { data: desligData, isLoading: desLoading } = useDesligamentos(apiClient)
  const createDeslig = useCreateDesligamento(apiClient)
  const [desForm, setDesForm] = useState<CriarDesligamentoCommand>({
    funcionarioId: '', empresaId: '', dataDesligamento: '', motivoDesligamento: '', verbasRescisorias: '',
  })

  const desColumns: Column<DesligamentoDto>[] = [
    { key: 'dataDesligamento', header: 'Data', render: (item) => formatDate(item.dataDesligamento) },
    { key: 'motivoDesligamento', header: 'Motivo', render: (item) => <Badge variant="outline">{item.motivoDesligamento}</Badge> },
  ]

  // ---- Férias ----
  const { data: feriasData, isLoading: ferLoading } = useFerias(apiClient)
  const createFeriasMutation = useCreateFerias(apiClient)
  const [ferForm, setFerForm] = useState({
    funcionarioId: '', empresaId: '', dataInicio: '', diasGozo: '30', periodoAquisitivoInicio: '', periodoAquisitivoFim: '', tipoFerias: '',
  })

  const ferColumns: Column<FeriasDto>[] = [
    { key: 'dataInicio', header: 'Início', render: (item) => formatDate(item.dataInicio) },
    { key: 'diasGozo', header: 'Dias', render: (item) => String(item.diasGozo) },
    { key: 'tipoFerias', header: 'Tipo', render: (item) => <Badge variant="outline">{item.tipoFerias}</Badge> },
  ]

  // ---- Afastamentos ----
  const { data: afastData, isLoading: afaLoading } = useAfastamentos(apiClient)
  const createAfast = useCreateAfastamento(apiClient)
  const [afaForm, setAfaForm] = useState<CriarAfastamentoCommand>({
    funcionarioId: '', empresaId: '', dataInicio: '', dataFimPrevista: '', tipoAfastamento: '', cid: '',
  })

  const afaColumns: Column<AfastamentoDto>[] = [
    { key: 'dataInicio', header: 'Início', render: (item) => formatDate(item.dataInicio) },
    { key: 'dataFimPrevista', header: 'Fim Previsto', render: (item) => formatDate(item.dataFimPrevista) },
    { key: 'tipoAfastamento', header: 'Tipo', render: (item) => <Badge variant="outline">{item.tipoAfastamento}</Badge> },
  ]

  // ---- Alterações Contratuais ----
  const { data: altData, isLoading: altLoading } = useAlteracoesContratuais(apiClient)
  const createAlt = useCreateAlteracaoContratual(apiClient)
  const [altForm, setAltForm] = useState<CriarAlteracaoContratualCommand>({
    funcionarioId: '', empresaId: '', dataAlteracao: '', camposAlterados: '', valorAnterior: '', valorNovo: '',
  })

  const altColumns: Column<AlteracaoContratualDto>[] = [
    { key: 'dataAlteracao', header: 'Data', render: (item) => formatDate(item.dataAlteracao) },
    { key: 'camposAlterados', header: 'Campos Alterados', render: (item) => item.camposAlterados || '—' },
  ]

  const openSheet = useCallback(() => setSheetOpen(true), [])
  const closeSheet = useCallback(() => setSheetOpen(false), [])

  const handleAdmissaoSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      await createAdmissao.mutateAsync({
        funcionarioId: admForm.funcionarioId,
        empresaId: admForm.empresaId,
        dataAdmissao: admForm.dataAdmissao,
        cargoId: admForm.cargoId,
        salarioInicial: Number(admForm.salarioInicial) || 0,
        tipoContrato: admForm.tipoContrato,
        periodoExperienciaMeses: admForm.periodoExperienciaMeses ? Number(admForm.periodoExperienciaMeses) : undefined,
      })
      toast.success('Admissão registrada!')
      closeSheet()
    } catch { toast.error('Erro ao registrar.') }
  }

  const handleDesligSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try { await createDeslig.mutateAsync(desForm); toast.success('Desligamento registrado!'); closeSheet() }
    catch { toast.error('Erro ao registrar.') }
  }

  const handleFeriasSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      await createFeriasMutation.mutateAsync({
        funcionarioId: ferForm.funcionarioId,
        empresaId: ferForm.empresaId,
        dataInicio: ferForm.dataInicio,
        diasGozo: Number(ferForm.diasGozo),
        periodoAquisitivoInicio: ferForm.periodoAquisitivoInicio,
        periodoAquisitivoFim: ferForm.periodoAquisitivoFim,
        tipoFerias: ferForm.tipoFerias,
      })
      toast.success('Férias programadas!')
      closeSheet()
    }
    catch { toast.error('Erro ao programar.') }
  }

  const handleAfastSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try { await createAfast.mutateAsync(afaForm); toast.success('Afastamento registrado!'); closeSheet() }
    catch { toast.error('Erro ao registrar.') }
  }

  const handleAltSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try { await createAlt.mutateAsync(altForm); toast.success('Alteração registrada!'); closeSheet() }
    catch { toast.error('Erro ao registrar.') }
  }

  const tabLabels: Record<Tab, string> = {
    admissoes: 'Admissões',
    desligamentos: 'Desligamentos',
    ferias: 'Férias',
    afastamentos: 'Afastamentos',
    alteracoes: 'Alt. Contratuais',
  }

  return (
    <div className="space-y-6">
      <div><h1 className="text-2xl font-bold">Eventos Trabalhistas</h1><p className="text-sm text-muted-foreground">Registre os eventos da vida funcional dos trabalhadores</p></div>

      {/* Tab Navigation */}
      <div className="flex gap-1 border-b">
        {(Object.keys(tabLabels) as Tab[]).map((t) => (
          <button
            key={t}
            onClick={() => setTab(t)}
            className={`px-4 py-2 text-sm font-medium transition-colors border-b-2 -mb-px ${
              tab === t ? 'border-primary text-primary' : 'border-transparent text-muted-foreground hover:text-foreground'
            }`}
          >
            {tabLabels[t]}
          </button>
        ))}
      </div>

      <div className="flex justify-end">
        <Button onClick={openSheet}><Plus className="mr-2 h-4 w-4" />Novo Registro</Button>
      </div>

      <Card>
        <CardContent className="pt-6">
          {tab === 'admissoes' && <DataTable columns={admColumns} rows={admissoesData?.items ?? []} isLoading={admLoading} />}
          {tab === 'desligamentos' && <DataTable columns={desColumns} rows={desligData?.items ?? []} isLoading={desLoading} />}
          {tab === 'ferias' && <DataTable columns={ferColumns} rows={feriasData?.items ?? []} isLoading={ferLoading} />}
          {tab === 'afastamentos' && <DataTable columns={afaColumns} rows={afastData?.items ?? []} isLoading={afaLoading} />}
          {tab === 'alteracoes' && <DataTable columns={altColumns} rows={altData?.items ?? []} isLoading={altLoading} />}
        </CardContent>
      </Card>

      <Sheet open={sheetOpen} onOpenChange={setSheetOpen}>
        <SheetContent side="right" className="w-full sm:max-w-lg overflow-y-auto">
          <SheetHeader>
            <SheetTitle>
              {tab === 'admissoes' && 'Nova Admissão'}
              {tab === 'desligamentos' && 'Novo Desligamento'}
              {tab === 'ferias' && 'Programar Férias'}
              {tab === 'afastamentos' && 'Novo Afastamento'}
              {tab === 'alteracoes' && 'Nova Alteração Contratual'}
            </SheetTitle>
          </SheetHeader>
          <div className="mt-6">
            {tab === 'admissoes' && (
              <FormContainer mode="create" onSubmit={handleAdmissaoSubmit} onCancel={closeSheet} isSubmitting={createAdmissao.isPending}>
                <AdmissaoFormFields data={admForm as any} onChange={(d: any) => setAdmForm(d)} mode="create" funcionarios={[{ value: '', label: 'Selecione...' }, ...funcionarioOptions]} cargos={[{ value: '', label: 'Selecione...' }, ...cargoOptions]} />
              </FormContainer>
            )}
            {tab === 'desligamentos' && (
              <FormContainer mode="create" onSubmit={handleDesligSubmit} onCancel={closeSheet} isSubmitting={createDeslig.isPending}>
                <DesligamentoFormFields data={desForm as any} onChange={(d: any) => setDesForm(d)} mode="create" funcionarios={[{ value: '', label: 'Selecione...' }, ...funcionarioOptions]} />
              </FormContainer>
            )}
            {tab === 'ferias' && (
              <FormContainer mode="create" onSubmit={handleFeriasSubmit} onCancel={closeSheet} isSubmitting={createFeriasMutation.isPending}>
                <FeriasFormFields data={ferForm as any} onChange={(d: any) => setFerForm(d)} mode="create" funcionarios={[{ value: '', label: 'Selecione...' }, ...funcionarioOptions]} />
              </FormContainer>
            )}
            {tab === 'afastamentos' && (
              <FormContainer mode="create" onSubmit={handleAfastSubmit} onCancel={closeSheet} isSubmitting={createAfast.isPending}>
                <AfastamentoEventoFormFields data={afaForm as any} onChange={(d: any) => setAfaForm(d)} mode="create" funcionarios={[{ value: '', label: 'Selecione...' }, ...funcionarioOptions]} />
              </FormContainer>
            )}
            {tab === 'alteracoes' && (
              <FormContainer mode="create" onSubmit={handleAltSubmit} onCancel={closeSheet} isSubmitting={createAlt.isPending}>
                <AlteracaoContratualFormFields data={altForm as any} onChange={(d: any) => setAltForm(d)} mode="create" funcionarios={[{ value: '', label: 'Selecione...' }, ...funcionarioOptions]} />
              </FormContainer>
            )}
          </div>
        </SheetContent>
      </Sheet>
    </div>
  )
}

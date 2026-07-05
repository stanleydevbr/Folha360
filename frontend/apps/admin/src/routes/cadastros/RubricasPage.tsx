import { useState, useCallback } from 'react'
import { useNavigate } from 'react-router'
import { useAuth } from '@/providers/AuthProvider'
import { useRubricas, useCreateRubrica, useUpdateRubrica, useDeleteRubrica, useGruposRubrica } from '@folha360/api'
import type { RubricaDto, CriarRubricaCommand } from '@folha360/api'
import {
  DataTable,
  FormContainer,
  RubricaFormFields,
  Card,
  CardContent,
  Button,
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetDescription,
  Badge,
} from '@folha360/ui'
import type { Column } from '@folha360/ui'
import { Plus, Pencil, Trash2, ExternalLink } from 'lucide-react'
import { toast } from 'sonner'

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

const EMPTY_FORM: FormData = {
  empresaId: '', grupoRubricaId: '', codigo: '', descricao: '', descricaoAbreviada: '',
  natureza: '', tipoEsocial: '', enviarEsocial: true, tipoCalculo: '',
  formulaCalculo: '', valorFixo: '', percentual: '', rubricaBaseId: '',
  ordemCalculo: '0', ordemExibicao: '0', prioridadeDesconto: '',
  tetoMaximo: '', pisoMinimo: '', ativo: true, dataInicioVigencia: '', dataFimVigencia: '', observacao: '',
  incideInss: false, incideIrrf: false, incideFgts: false,
  incideContribuicaoSindical: false, incideDecimoTerceiro: false, incideFerias: false,
  incideAvisoPrevio: false, incideRescisao: false, incideDissidio: false,
  incideSalarioMaternidade: false, incideAuxilioDoenca: false, incideAdiantamento: false,
}

const COLUMNS: Column<RubricaDto>[] = [
  { key: 'codigo', header: 'Código' },
  { key: 'descricao', header: 'Descrição' },
  { key: 'natureza', header: 'Natureza', render: (item) => <Badge variant="outline">{item.natureza}</Badge> },
  { key: 'tipoCalculo', header: 'Tipo Cálculo' },
  { key: 'ativo', header: 'Ativo', render: (item) => item.ativo ? <Badge>Sim</Badge> : <Badge variant="secondary">Não</Badge> },
]

// ---- Conversão DTO ↔ FormData ----

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

export default function RubricasPage() {
  const { apiClient } = useAuth()
  const navigate = useNavigate()
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [sheetOpen, setSheetOpen] = useState(false)
  const [editId, setEditId] = useState<string | null>(null)
  const [formData, setFormData] = useState<FormData>(EMPTY_FORM)

  const { data, isLoading } = useRubricas(apiClient, { page, pageSize: 20, descricao: search || undefined })
  const { data: gruposData } = useGruposRubrica(apiClient)
  const { data: rubricasData } = useRubricas(apiClient, { pageSize: 200 })

  const createMutation = useCreateRubrica(apiClient)
  const updateMutation = useUpdateRubrica(apiClient)
  const deleteMutation = useDeleteRubrica(apiClient)

  const grupoOptions = [{ value: '', label: 'Nenhum' }, ...(gruposData?.items ?? []).map((g) => ({ value: g.id, label: g.descricao }))]
  const rubricaBaseOptions = [{ value: '', label: 'Nenhuma' }, ...(rubricasData?.items ?? []).map((r) => ({ value: r.id, label: `${r.codigo} - ${r.descricao}` }))]

  const openCreate = useCallback(() => { setEditId(null); setFormData(EMPTY_FORM); setSheetOpen(true) }, [])
  const openEdit = useCallback((item: RubricaDto) => {
    setEditId(item.id)
    setFormData(dtoToForm(item))
    setSheetOpen(true)
  }, [])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      const payload = formToDto(formData)
      if (editId) { await updateMutation.mutateAsync({ id: editId, data: payload }); toast.success('Rubrica atualizada!') }
      else { await createMutation.mutateAsync(payload); toast.success('Rubrica criada!') }
      setSheetOpen(false)
    } catch { toast.error('Erro ao salvar.') }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div><h1 className="text-2xl font-bold">Rubricas</h1><p className="text-sm text-muted-foreground">Gerencie o plano de rubricas para cálculo da folha</p></div>
        <Button onClick={openCreate}><Plus className="mr-2 h-4 w-4" />Nova Rubrica</Button>
      </div>
      <Card><CardContent className="pt-6">
        <DataTable columns={COLUMNS} rows={data?.items ?? []} isLoading={isLoading} totalCount={data?.totalCount ?? 0} page={page} pageSize={20} onPageChange={setPage}
          searchValue={search}
          onSearchChange={setSearch}
          searchPlaceholder="Buscar por descrição..."
          onRowClick={(item) => navigate(`/cadastros/rubricas/${item.id}`)}
          actions={(item) => (
            <div className="flex gap-1">
              <Button variant="ghost" size="sm" onClick={(e) => { e.stopPropagation(); navigate(`/cadastros/rubricas/${item.id}`) }}><ExternalLink className="h-4 w-4" /></Button>
              <Button variant="ghost" size="sm" onClick={(e) => { e.stopPropagation(); openEdit(item) }}><Pencil className="h-4 w-4" /></Button>
              <Button variant="ghost" size="sm" onClick={(e) => { e.stopPropagation(); deleteMutation.mutateAsync(item.id).then(() => toast.success('Excluída!')).catch(() => toast.error('Erro!')) }}><Trash2 className="h-4 w-4 text-destructive" /></Button>
            </div>
          )} />
      </CardContent></Card>
      <Sheet open={sheetOpen} onOpenChange={setSheetOpen}>
        <SheetContent side="right" className="w-full sm:max-w-3xl lg:max-w-4xl overflow-y-auto">
          <SheetHeader><SheetTitle>{editId ? 'Editar Rubrica' : 'Nova Rubrica'}</SheetTitle><SheetDescription>Configure a rubrica para cálculo da folha.</SheetDescription></SheetHeader>
          <div className="mt-6"><FormContainer mode={editId ? 'edit' : 'create'} onSubmit={handleSubmit} onCancel={() => setSheetOpen(false)} isSubmitting={createMutation.isPending || updateMutation.isPending}><RubricaFormFields data={formData as any} onChange={(d: any) => setFormData(d as FormData)} mode={editId ? 'edit' : 'create'} grupos={grupoOptions} rubricasBase={rubricaBaseOptions} /></FormContainer></div>
        </SheetContent>
      </Sheet>
    </div>
  )
}

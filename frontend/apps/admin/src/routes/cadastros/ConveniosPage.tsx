import { useState, useCallback } from 'react'
import { useAuth } from '@/providers/AuthProvider'
import { useConvenios, useCreateConvenio, useUpdateConvenio, useDeleteConvenio, useEmpresas } from '@folha360/api'
import type { Convenio } from '@folha360/api'
import {
  DataTable,
  ConvenioFormFields,
  Card,
  CardContent,
  Button,
  FormPanel,
  Badge,
} from '@folha360/ui'
import type { Column } from '@folha360/ui'
import { formatCurrency } from '@folha360/utils'
import { Plus, Pencil, Trash2 } from 'lucide-react'
import { toast } from 'sonner'

type FormData = {
  id: string
  empresaId: string
  nome: string
  tipo: string
  operadora: string
  valorMensal: string
  percentualEmpresa: string
  percentualFuncionario: string
}

const EMPTY_FORM: Omit<FormData, 'id' | 'empresaId'> = {
  nome: '', tipo: '', operadora: '',
  valorMensal: '0', percentualEmpresa: '0', percentualFuncionario: '0',
}

const COLUMNS: Column<Convenio>[] = [
  { key: 'nome', header: 'Nome' },
  { key: 'tipo', header: 'Tipo', render: (item) => <Badge variant="outline">{item.tipo}</Badge> },
  { key: 'operadora', header: 'Operadora', render: (item) => item.operadora || '—' },
  { key: 'valorMensal', header: 'Valor Mensal', render: (item) => formatCurrency(item.valorMensal) },
]

export default function ConveniosPage() {
  const { apiClient } = useAuth()
  const [sheetOpen, setSheetOpen] = useState(false)
  const [editId, setEditId] = useState<string | null>(null)
  const [editItem, setEditItem] = useState<Convenio | null>(null)
  const [formData, setFormData] = useState<FormData>({ id: '', empresaId: '', ...EMPTY_FORM })

  const { data, isLoading } = useConvenios(apiClient)
  const { data: empresasData } = useEmpresas(apiClient)
  const createMutation = useCreateConvenio(apiClient)
  const updateMutation = useUpdateConvenio(apiClient)
  const deleteMutation = useDeleteConvenio(apiClient)
  const primeiraEmpresaId = empresasData?.items?.[0]?.id

  const openCreate = useCallback(() => {
    setEditId(null)
    setEditItem(null)
    setFormData({ id: '', empresaId: primeiraEmpresaId ?? '', ...EMPTY_FORM })
    setSheetOpen(true)
  }, [primeiraEmpresaId])
  const openEdit = useCallback((item: Convenio) => {
    setEditId(item.id)
    setEditItem(item)
    setFormData({
      id: item.id, empresaId: item.empresaId, nome: item.nome, tipo: item.tipo,
      operadora: item.operadora ?? '',
      valorMensal: String(item.valorMensal),
      percentualEmpresa: String(item.percentualEmpresa),
      percentualFuncionario: String(item.percentualFuncionario),
    })
    setSheetOpen(true)
  }, [])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      if (!formData.empresaId) { toast.error('Nenhuma empresa encontrada. Crie uma empresa primeiro.'); return }
      const payload: Convenio = { ...formData, valorMensal: Number(formData.valorMensal), percentualEmpresa: Number(formData.percentualEmpresa), percentualFuncionario: Number(formData.percentualFuncionario), ...(editItem ? { createdAt: editItem.createdAt, updatedAt: editItem.updatedAt } : {}) }
      if (editId) { await updateMutation.mutateAsync({ id: editId, data: payload }); toast.success('Convênio atualizado!') }
      else { const { id: _, ...createPayload } = payload; await createMutation.mutateAsync(createPayload as Convenio); toast.success('Convênio criado!') }
      setSheetOpen(false)
    } catch { toast.error('Erro ao salvar.') }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div><h1 className="text-2xl font-bold">Convênios</h1><p className="text-sm text-muted-foreground">Gerencie convênios e benefícios</p></div>
        <Button onClick={openCreate}><Plus className="mr-2 h-4 w-4" />Novo Convênio</Button>
      </div>
      <Card><CardContent className="pt-6">
        <DataTable columns={COLUMNS} rows={data ?? []} isLoading={isLoading}
          actions={(item) => (
            <div className="flex gap-1">
              <Button variant="ghost" size="sm" onClick={() => openEdit(item)}><Pencil className="h-4 w-4" /></Button>
              <Button variant="ghost" size="sm" onClick={() => deleteMutation.mutateAsync(item.id).then(() => toast.success('Excluído!')).catch(() => toast.error('Erro!'))}><Trash2 className="h-4 w-4 text-destructive" /></Button>
            </div>
          )} />
      </CardContent></Card>
      <FormPanel
        open={sheetOpen}
        onClose={() => setSheetOpen(false)}
        title={editId ? 'Editar Convênio' : 'Novo Convênio'}
        subtitle="Preencha os dados do convênio."
        onSubmit={handleSubmit}
        isSubmitting={createMutation.isPending || updateMutation.isPending}
        saveLabel={editId ? 'Salvar' : 'Criar Convênio'}
      >
        <fieldset disabled={createMutation.isPending || updateMutation.isPending} className="flex flex-col gap-4">
          <ConvenioFormFields data={formData as any} onChange={(d: any) => setFormData(d as FormData)} mode={editId ? 'edit' : 'create'} />
        </fieldset>
      </FormPanel>
    </div>
  )
}

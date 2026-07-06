import { useState, useCallback } from 'react'
import { useAuth } from '@/providers/AuthProvider'
import { useLotacoes, useCreateLotacao, useUpdateLotacao, useDeleteLotacao } from '@folha360/api'
import type { LotacaoDto, CriarLotacaoCommand } from '@folha360/api'
import {
  DataTable,
  LotacaoFormFields,
  Card,
  CardContent,
  Button,
  FormPanel,
} from '@folha360/ui'
import type { Column } from '@folha360/ui'
import { Plus, Pencil, Trash2 } from 'lucide-react'
import { toast } from 'sonner'

const COLUMNS: Column<LotacaoDto>[] = [
  { key: 'codigo', header: 'Código' },
  { key: 'descricao', header: 'Descrição' },
  { key: 'tipoEsocial', header: 'Tipo e-Social', render: (item) => item.tipoEsocial || '—' },
]

export default function LotacoesPage() {
  const { apiClient } = useAuth()
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [sheetOpen, setSheetOpen] = useState(false)
  const [editId, setEditId] = useState<string | null>(null)
  const [formData, setFormData] = useState({ empresaId: '', codigo: '', descricao: '', tipoEsocial: '' })

  const { data, isLoading } = useLotacoes(apiClient, { page, pageSize: 20, descricao: search || undefined })
  const createMutation = useCreateLotacao(apiClient)
  const updateMutation = useUpdateLotacao(apiClient)
  const deleteMutation = useDeleteLotacao(apiClient)

  const openCreate = useCallback(() => { setEditId(null); setFormData({ empresaId: '', codigo: '', descricao: '', tipoEsocial: '' }); setSheetOpen(true) }, [])
  const openEdit = useCallback((item: LotacaoDto) => {
    setEditId(item.id)
    setFormData({ empresaId: item.empresaId, codigo: item.codigo, descricao: item.descricao, tipoEsocial: item.tipoEsocial ?? '' })
    setSheetOpen(true)
  }, [])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      const payload: CriarLotacaoCommand = { ...formData }
      if (editId) { await updateMutation.mutateAsync({ id: editId, data: payload }); toast.success('Lotação atualizada!') }
      else { await createMutation.mutateAsync(payload); toast.success('Lotação criada!') }
      setSheetOpen(false)
    } catch { toast.error('Erro ao salvar.') }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div><h1 className="text-2xl font-bold">Lotações</h1><p className="text-sm text-muted-foreground">Gerencie as lotações (matriz, filiais, obras)</p></div>
        <Button onClick={openCreate}><Plus className="mr-2 h-4 w-4" />Nova Lotação</Button>
      </div>
      <Card><CardContent className="pt-6">
        <DataTable columns={COLUMNS} rows={data?.items ?? []} isLoading={isLoading} totalCount={data?.totalCount ?? 0} page={page} pageSize={20} onPageChange={setPage} searchValue={search} onSearchChange={setSearch} searchPlaceholder="Buscar..."
          actions={(item) => (
            <div className="flex gap-1">
              <Button variant="ghost" size="sm" onClick={() => openEdit(item)}><Pencil className="h-4 w-4" /></Button>
              <Button variant="ghost" size="sm" onClick={() => deleteMutation.mutateAsync(item.id).then(() => toast.success('Excluída!')).catch(() => toast.error('Erro!'))}><Trash2 className="h-4 w-4 text-destructive" /></Button>
            </div>
          )} />
      </CardContent></Card>
      <FormPanel
        open={sheetOpen}
        onClose={() => setSheetOpen(false)}
        title={editId ? 'Editar Lotação' : 'Nova Lotação'}
        subtitle="Preencha os dados da lotação."
        onSubmit={handleSubmit}
        isSubmitting={createMutation.isPending || updateMutation.isPending}
        saveLabel={editId ? 'Salvar' : 'Criar Lotação'}
      >
        <fieldset disabled={createMutation.isPending || updateMutation.isPending} className="flex flex-col gap-4">
          <LotacaoFormFields data={formData as any} onChange={(d: any) => setFormData(d as typeof formData)} mode={editId ? 'edit' : 'create'} />
        </fieldset>
      </FormPanel>
    </div>
  )
}

import { useState, useCallback } from 'react'
import { useAuth } from '@/providers/AuthProvider'
import { useCargos, useCreateCargo, useUpdateCargo, useDeleteCargo } from '@folha360/api'
import type { CargoDto, CriarCargoCommand } from '@folha360/api'
import {
  DataTable,
  CargoFormFields,
  Card,
  CardContent,
  Button,
  FormPanel,
} from '@folha360/ui'
import type { Column } from '@folha360/ui'
import { formatCurrency, formatDate } from '@folha360/utils'
import { Plus, Pencil, Trash2 } from 'lucide-react'
import { toast } from 'sonner'

const COLUMNS: Column<CargoDto>[] = [
  { key: 'nome', header: 'Nome' },
  { key: 'cbo', header: 'CBO' },
  { key: 'descricao', header: 'Descrição', render: (item) => item.descricao || '—' },
  {
    key: 'salarioBaseMinimo',
    header: 'Sal. Mín.',
    render: (item) => (item.salarioBaseMinimo ? formatCurrency(item.salarioBaseMinimo) : '—'),
  },
  {
    key: 'salarioBaseMaximo',
    header: 'Sal. Máx.',
    render: (item) => (item.salarioBaseMaximo ? formatCurrency(item.salarioBaseMaximo) : '—'),
  },
]

type FormData = {
  empresaId: string
  nome: string
  cbo: string
  descricao: string
  salarioBaseMinimo: string
  salarioBaseMaximo: string
}

const EMPTY_FORM: FormData = {
  empresaId: '',
  nome: '',
  cbo: '',
  descricao: '',
  salarioBaseMinimo: '',
  salarioBaseMaximo: '',
}

export default function CargosPage() {
  const { apiClient } = useAuth()
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [sheetOpen, setSheetOpen] = useState(false)
  const [editId, setEditId] = useState<string | null>(null)
  const [formData, setFormData] = useState<FormData>(EMPTY_FORM)
  const [formErrors, setFormErrors] = useState<Record<string, string>>({})

  const { data, isLoading } = useCargos(apiClient, { page, pageSize: 20, nome: search || undefined })
  const createMutation = useCreateCargo(apiClient)
  const updateMutation = useUpdateCargo(apiClient)
  const deleteMutation = useDeleteCargo(apiClient)

  const openCreate = useCallback(() => {
    setEditId(null)
    setFormData(EMPTY_FORM)
    setFormErrors({})
    setSheetOpen(true)
  }, [])

  const openEdit = useCallback((item: CargoDto) => {
    setEditId(item.id)
    setFormData({
      empresaId: item.empresaId,
      nome: item.nome,
      cbo: item.cbo,
      descricao: item.descricao ?? '',
      salarioBaseMinimo: item.salarioBaseMinimo ? String(item.salarioBaseMinimo) : '',
      salarioBaseMaximo: item.salarioBaseMaximo ? String(item.salarioBaseMaximo) : '',
    })
    setFormErrors({})
    setSheetOpen(true)
  }, [])

  const validate = (): boolean => {
    const errors: Record<string, string> = {}
    if (!formData.nome.trim()) errors.nome = 'Nome é obrigatório'
    if (!formData.cbo || formData.cbo.replace(/\D/g, '').length !== 6) errors.cbo = 'CBO deve ter 6 dígitos'
    setFormErrors(errors)
    return Object.keys(errors).length === 0
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!validate()) return
    try {
      const payload: CriarCargoCommand = {
        ...formData,
        salarioBaseMinimo: formData.salarioBaseMinimo ? Number(formData.salarioBaseMinimo) : undefined,
        salarioBaseMaximo: formData.salarioBaseMaximo ? Number(formData.salarioBaseMaximo) : undefined,
      }
      if (editId) {
        await updateMutation.mutateAsync({ id: editId, data: payload })
        toast.success('Cargo atualizado!')
      } else {
        await createMutation.mutateAsync(payload)
        toast.success('Cargo criado!')
      }
      setSheetOpen(false)
    } catch {
      toast.error('Erro ao salvar cargo.')
    }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Excluir este cargo?')) return
    try {
      await deleteMutation.mutateAsync(id)
      toast.success('Cargo excluído!')
    } catch {
      toast.error('Erro ao excluir cargo.')
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Cargos</h1>
          <p className="text-sm text-muted-foreground">Gerencie os cargos e funções</p>
        </div>
        <Button onClick={openCreate}><Plus className="mr-2 h-4 w-4" />Novo Cargo</Button>
      </div>

      <Card>
        <CardContent className="pt-6">
          <DataTable
            columns={COLUMNS}
            rows={data?.items ?? []}
            isLoading={isLoading}
            totalCount={data?.totalCount ?? 0}
            page={page}
            pageSize={20}
            onPageChange={setPage}
            searchValue={search}
            onSearchChange={setSearch}
            searchPlaceholder="Buscar por nome..."
            actions={(item) => (
              <div className="flex gap-1">
                <Button variant="ghost" size="sm" onClick={() => openEdit(item)}><Pencil className="h-4 w-4" /></Button>
                <Button variant="ghost" size="sm" onClick={() => handleDelete(item.id)}><Trash2 className="h-4 w-4 text-destructive" /></Button>
              </div>
            )}
          />
        </CardContent>
      </Card>

      <FormPanel
        open={sheetOpen}
        onClose={() => setSheetOpen(false)}
        title={editId ? 'Editar Cargo' : 'Novo Cargo'}
        subtitle="Preencha os dados do cargo."
        onSubmit={handleSubmit}
        isSubmitting={createMutation.isPending || updateMutation.isPending}
        saveLabel={editId ? 'Salvar' : 'Criar Cargo'}
      >
        <fieldset disabled={createMutation.isPending || updateMutation.isPending} className="flex flex-col gap-4">
          <CargoFormFields data={formData as any} onChange={(d) => setFormData(d as FormData)} errors={formErrors as any} mode={editId ? 'edit' : 'create'} />
        </fieldset>
      </FormPanel>
    </div>
  )
}

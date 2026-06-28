import { useState, useCallback } from 'react'
import { useAuth } from '@/providers/AuthProvider'
import { useSindicatos, useCreateSindicato, useUpdateSindicato, useDeleteSindicato } from '@folha360/api'
import type { Sindicato } from '@folha360/api'
import {
  DataTable,
  FormContainer,
  SindicatoFormFields,
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
import { Plus, Pencil, Trash2 } from 'lucide-react'
import { toast } from 'sonner'

type FormData = {
  id: string
  empresaId: string
  codigo: string
  nome: string
  cnpj: string
  tipo: string
  contribuicaoSindicalPercentual: string
  contribuicaoAssistencialPercentual: string
}

const EMPTY_FORM: FormData = {
  id: '',
  empresaId: '',
  codigo: '',
  nome: '',
  cnpj: '',
  tipo: '',
  contribuicaoSindicalPercentual: '0',
  contribuicaoAssistencialPercentual: '0',
}

const COLUMNS: Column<Sindicato>[] = [
  { key: 'codigo', header: 'Código' },
  { key: 'nome', header: 'Nome' },
  { key: 'cnpj', header: 'CNPJ', render: (item) => item.cnpj || '—' },
  { key: 'tipo', header: 'Tipo', render: (item) => item.tipo ? <Badge variant="outline">{item.tipo}</Badge> : '—' },
]

export default function SindicatosPage() {
  const { apiClient } = useAuth()
  const [sheetOpen, setSheetOpen] = useState(false)
  const [editId, setEditId] = useState<string | null>(null)
  const [formData, setFormData] = useState<FormData>(EMPTY_FORM)

  const { data, isLoading } = useSindicatos(apiClient)
  const createMutation = useCreateSindicato(apiClient)
  const updateMutation = useUpdateSindicato(apiClient)
  const deleteMutation = useDeleteSindicato(apiClient)

  const openCreate = useCallback(() => {
    setEditId(null)
    setFormData(EMPTY_FORM)
    setSheetOpen(true)
  }, [])

  const openEdit = useCallback((item: Sindicato) => {
    setEditId(item.id)
    setFormData({
      id: item.id, empresaId: item.empresaId, codigo: item.codigo, nome: item.nome,
      cnpj: item.cnpj ?? '', tipo: item.tipo ?? '',
      contribuicaoSindicalPercentual: String(item.contribuicaoSindicalPercentual),
      contribuicaoAssistencialPercentual: String(item.contribuicaoAssistencialPercentual),
    })
    setSheetOpen(true)
  }, [])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      const payload: Sindicato = {
        ...formData,
        contribuicaoSindicalPercentual: Number(formData.contribuicaoSindicalPercentual),
        contribuicaoAssistencialPercentual: Number(formData.contribuicaoAssistencialPercentual),
      }
      if (editId) {
        await updateMutation.mutateAsync({ id: editId, data: payload })
        toast.success('Sindicato atualizado!')
      } else {
        await createMutation.mutateAsync(payload)
        toast.success('Sindicato criado!')
      }
      setSheetOpen(false)
    } catch {
      toast.error('Erro ao salvar.')
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Sindicatos</h1>
          <p className="text-sm text-muted-foreground">Gerencie os sindicatos patronais e laborais</p>
        </div>
        <Button onClick={openCreate}><Plus className="mr-2 h-4 w-4" />Novo Sindicato</Button>
      </div>

      <Card>
        <CardContent className="pt-6">
          <DataTable
            columns={COLUMNS}
            rows={data ?? []}
            isLoading={isLoading}
            actions={(item) => (
              <div className="flex gap-1">
                <Button variant="ghost" size="sm" onClick={() => openEdit(item)}><Pencil className="h-4 w-4" /></Button>
                <Button variant="ghost" size="sm" onClick={() => deleteMutation.mutateAsync(item.id).then(() => toast.success('Excluído!')).catch(() => toast.error('Erro!'))}>
                  <Trash2 className="h-4 w-4 text-destructive" />
                </Button>
              </div>
            )}
          />
        </CardContent>
      </Card>

      <Sheet open={sheetOpen} onOpenChange={setSheetOpen}>
        <SheetContent side="right" className="w-full sm:max-w-lg overflow-y-auto">
          <SheetHeader>
            <SheetTitle>{editId ? 'Editar Sindicato' : 'Novo Sindicato'}</SheetTitle>
            <SheetDescription>Preencha os dados do sindicato.</SheetDescription>
          </SheetHeader>
          <div className="mt-6">
            <FormContainer
              mode={editId ? 'edit' : 'create'}
              onSubmit={handleSubmit}
              onCancel={() => setSheetOpen(false)}
              isSubmitting={createMutation.isPending || updateMutation.isPending}
            >
              <SindicatoFormFields data={formData as any} onChange={(d: any) => setFormData(d as FormData)} mode={editId ? 'edit' : 'create'} />
            </FormContainer>
          </div>
        </SheetContent>
      </Sheet>
    </div>
  )
}

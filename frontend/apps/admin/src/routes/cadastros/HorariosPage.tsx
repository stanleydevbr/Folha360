import { useState, useCallback } from 'react'
import { useAuth } from '@/providers/AuthProvider'
import { useHorariosTrabalho, useCreateHorarioTrabalho, useUpdateHorarioTrabalho, useDeleteHorarioTrabalho } from '@folha360/api'
import type { HorarioTrabalho } from '@folha360/api'
import {
  DataTable,
  FormContainer,
  HorarioTrabalhoFormFields,
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
  descricao: string
  tipo: string
  cargaHorariaDiaria: string
  cargaHorariaSemanal: string
  inicioJornada: string
  fimJornada: string
  inicioIntervalo: string
  fimIntervalo: string
  toleranciaAtrasoMinutos: string
}

const EMPTY_FORM: FormData = {
  id: '', empresaId: '', codigo: '', descricao: '', tipo: '',
  cargaHorariaDiaria: '480', cargaHorariaSemanal: '2400',
  inicioJornada: '08:00', fimJornada: '17:00', inicioIntervalo: '', fimIntervalo: '',
  toleranciaAtrasoMinutos: '10',
}

const COLUMNS: Column<HorarioTrabalho>[] = [
  { key: 'codigo', header: 'Código' },
  { key: 'descricao', header: 'Descrição' },
  { key: 'tipo', header: 'Tipo', render: (item) => <Badge variant="outline">{item.tipo}</Badge> },
  { key: 'inicioJornada', header: 'Início' },
  { key: 'fimJornada', header: 'Fim' },
]

export default function HorariosPage() {
  const { apiClient } = useAuth()
  const [sheetOpen, setSheetOpen] = useState(false)
  const [editId, setEditId] = useState<string | null>(null)
  const [formData, setFormData] = useState<FormData>(EMPTY_FORM)

  const { data, isLoading } = useHorariosTrabalho(apiClient)
  const createMutation = useCreateHorarioTrabalho(apiClient)
  const updateMutation = useUpdateHorarioTrabalho(apiClient)
  const deleteMutation = useDeleteHorarioTrabalho(apiClient)

  const openCreate = useCallback(() => { setEditId(null); setFormData(EMPTY_FORM); setSheetOpen(true) }, [])
  const openEdit = useCallback((item: HorarioTrabalho) => {
    setEditId(item.id)
    setFormData({
      id: item.id, empresaId: item.empresaId, codigo: item.codigo, descricao: item.descricao, tipo: item.tipo,
      cargaHorariaDiaria: String(item.cargaHorariaDiaria),
      cargaHorariaSemanal: String(item.cargaHorariaSemanal),
      inicioJornada: item.inicioJornada,
      fimJornada: item.fimJornada,
      inicioIntervalo: item.inicioIntervalo ?? '',
      fimIntervalo: item.fimIntervalo ?? '',
      toleranciaAtrasoMinutos: String(item.toleranciaAtrasoMinutos),
    })
    setSheetOpen(true)
  }, [])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      const payload: HorarioTrabalho = { ...formData, cargaHorariaDiaria: Number(formData.cargaHorariaDiaria), cargaHorariaSemanal: Number(formData.cargaHorariaSemanal), toleranciaAtrasoMinutos: Number(formData.toleranciaAtrasoMinutos) }
      if (editId) { await updateMutation.mutateAsync({ id: editId, data: payload }); toast.success('Horário atualizado!') }
      else { await createMutation.mutateAsync(payload); toast.success('Horário criado!') }
      setSheetOpen(false)
    } catch { toast.error('Erro ao salvar.') }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div><h1 className="text-2xl font-bold">Horários de Trabalho</h1><p className="text-sm text-muted-foreground">Gerencie as jornadas de trabalho</p></div>
        <Button onClick={openCreate}><Plus className="mr-2 h-4 w-4" />Novo Horário</Button>
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
      <Sheet open={sheetOpen} onOpenChange={setSheetOpen}>
        <SheetContent side="right" className="w-full sm:max-w-lg overflow-y-auto">
          <SheetHeader><SheetTitle>{editId ? 'Editar Horário' : 'Novo Horário'}</SheetTitle><SheetDescription>Configure a jornada de trabalho.</SheetDescription></SheetHeader>
          <div className="mt-6"><FormContainer mode={editId ? 'edit' : 'create'} onSubmit={handleSubmit} onCancel={() => setSheetOpen(false)} isSubmitting={createMutation.isPending || updateMutation.isPending}><HorarioTrabalhoFormFields data={formData as any} onChange={(d: any) => setFormData(d as FormData)} mode={editId ? 'edit' : 'create'} /></FormContainer></div>
        </SheetContent>
      </Sheet>
    </div>
  )
}

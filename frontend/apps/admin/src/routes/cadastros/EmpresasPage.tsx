import { useState, useCallback } from 'react'
import { useNavigate } from 'react-router'
import { useAuth } from '@/providers/AuthProvider'
import { useEmpresas, useCreateEmpresa, useDeleteEmpresa } from '@folha360/api'
import type { EmpresaDto, CriarEmpresaCommand } from '@folha360/api'
import {
  DataTable,
  FormContainer,
  EmpresaFormFields,
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
import { formatCnpj, formatDate } from '@folha360/utils'
import { Plus, Trash2, ExternalLink } from 'lucide-react'
import { toast } from 'sonner'

const EMPTY_FORM: CriarEmpresaCommand = {
  cnpj: '',
  razaoSocial: '',
  nomeFantasia: '',
  cnae: '',
  regimeTributario: '',
  fpas: '',
  codigoTerceiros: '',
  classificacaoTributaria: '',
  matrizFilial: '',
  cnpjMatriz: '',
  enderecoLogradouro: '',
  enderecoNumero: '',
  enderecoComplemento: '',
  enderecoBairro: '',
  enderecoCep: '',
  enderecoMunicipio: '',
  enderecoUf: '',
  telefone: '',
  email: '',
}

const COLUMNS: Column<EmpresaDto>[] = [
  { key: 'cnpj', header: 'CNPJ', render: (item) => formatCnpj(item.cnpj) },
  { key: 'razaoSocial', header: 'Razão Social' },
  { key: 'nomeFantasia', header: 'Nome Fantasia', render: (item) => item.nomeFantasia || '—' },
  {
    key: 'regimeTributario',
    header: 'Regime',
    render: (item) => <Badge variant="secondary">{item.regimeTributario}</Badge>,
  },
  { key: 'createdAt', header: 'Criado em', render: (item) => formatDate(item.createdAt) },
]

export default function EmpresasPage() {
  const { apiClient } = useAuth()
  const navigate = useNavigate()
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [sheetOpen, setSheetOpen] = useState(false)
  const [formData, setFormData] = useState<CriarEmpresaCommand>(EMPTY_FORM)
  const [formErrors, setFormErrors] = useState<Record<string, string>>({})

  const { data, isLoading } = useEmpresas(apiClient) // Remove unused params for simplicity
  const createMutation = useCreateEmpresa(apiClient)
  const deleteMutation = useDeleteEmpresa(apiClient)

  const openCreate = useCallback(() => {
    setFormData(EMPTY_FORM)
    setFormErrors({})
    setSheetOpen(true)
  }, [])

  const validate = (): boolean => {
    const errors: Record<string, string> = {}
    if (!formData.cnpj || formData.cnpj.replace(/\D/g, '').length !== 14)
      errors.cnpj = 'CNPJ inválido (14 dígitos)'
    if (!formData.razaoSocial.trim()) errors.razaoSocial = 'Razão Social é obrigatória'
    if (!formData.regimeTributario) errors.regimeTributario = 'Regime Tributário é obrigatório'
    setFormErrors(errors)
    return Object.keys(errors).length === 0
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!validate()) return

    try {
      await createMutation.mutateAsync(formData)
      toast.success('Empresa criada com sucesso!')
      setSheetOpen(false)
    } catch {
      toast.error('Erro ao salvar empresa.')
    }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Tem certeza que deseja excluir esta empresa?')) return
    try {
      await deleteMutation.mutateAsync(id)
      toast.success('Empresa excluída!')
    } catch {
      toast.error('Erro ao excluir empresa.')
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Empresas</h1>
          <p className="text-sm text-muted-foreground">
            Gerencie as empresas cadastradas. Clique em uma empresa para ver detalhes.
          </p>
        </div>
        <Button onClick={openCreate}>
          <Plus className="mr-2 h-4 w-4" />
          Nova Empresa
        </Button>
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
            searchPlaceholder="Buscar por Razão Social..."
            onRowClick={(item) => navigate(`/cadastros/empresas/${item.id}`)}
            actions={(item) => (
              <div className="flex gap-1">
                <Button variant="ghost" size="sm" onClick={(e) => { e.stopPropagation(); navigate(`/cadastros/empresas/${item.id}`) }}>
                  <ExternalLink className="h-4 w-4" />
                </Button>
                <Button variant="ghost" size="sm" onClick={(e) => { e.stopPropagation(); handleDelete(item.id) }}>
                  <Trash2 className="h-4 w-4 text-destructive" />
                </Button>
              </div>
            )}
          />
        </CardContent>
      </Card>

      {/* Create-only Sheet (quick create) */}
      <Sheet open={sheetOpen} onOpenChange={setSheetOpen}>
        <SheetContent side="right" className="w-full sm:max-w-xl overflow-y-auto">
          <SheetHeader>
            <SheetTitle>Nova Empresa</SheetTitle>
            <SheetDescription>
              Preencha os dados para cadastrar uma nova empresa. Após criar, acesse a empresa para adicionar endereços, contatos e demais configurações.
            </SheetDescription>
          </SheetHeader>
          <div className="mt-6">
            <FormContainer
              mode="create"
              onSubmit={handleSubmit}
              onCancel={() => setSheetOpen(false)}
              isSubmitting={createMutation.isPending}
              submitLabel="Criar Empresa"
            >
              <EmpresaFormFields
                data={formData as any}
                onChange={(d) => setFormData(d as CriarEmpresaCommand)}
                errors={formErrors}
                mode="create"
              />
            </FormContainer>
          </div>
        </SheetContent>
      </Sheet>
    </div>
  )
}

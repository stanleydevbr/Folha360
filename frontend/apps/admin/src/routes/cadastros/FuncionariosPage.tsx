import { useState, useCallback } from 'react'
import { useNavigate } from 'react-router'
import { useAuth } from '@/providers/AuthProvider'
import {
  useFuncionarios,
  useCreateFuncionario,
  useDeleteFuncionario,
  useCargos,
  useLotacoes,
} from '@folha360/api'
import type { FuncionarioDto, CriarFuncionarioCommand } from '@folha360/api'
import {
  DataTable,
  FormContainer,
  FuncionarioFormFields,
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
import { formatCurrency, formatDate } from '@folha360/utils'
import { Plus, Trash2, ExternalLink } from 'lucide-react'
import { toast } from 'sonner'

const EMPTY_FORM: CriarFuncionarioCommand = {
  empresaId: '',
  nome: '',
  cpf: '',
  dataAdmissao: '',
  cargoId: '',
  lotacaoId: '',
  salarioBase: 0,
}

const COLUMNS: Column<FuncionarioDto>[] = [
  { key: 'nome', header: 'Nome' },
  { key: 'cpfMascarado', header: 'CPF' },
  {
    key: 'status',
    header: 'Status',
    render: (item) => (
      <Badge variant={item.status === 'Ativo' ? 'default' : 'secondary'}>{item.status}</Badge>
    ),
  },
  {
    key: 'salarioBase',
    header: 'Salário Base',
    render: (item) => formatCurrency(item.salarioBase),
  },
  { key: 'dataAdmissao', header: 'Admissão', render: (item) => formatDate(item.dataAdmissao) },
]

export default function FuncionariosPage() {
  const { apiClient } = useAuth()
  const navigate = useNavigate()
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [sheetOpen, setSheetOpen] = useState(false)
  const [formData, setFormData] = useState<CriarFuncionarioCommand>(EMPTY_FORM)
  const [formErrors, setFormErrors] = useState<Record<string, string>>({})

  const { data, isLoading } = useFuncionarios(apiClient)
  const { data: cargosData } = useCargos(apiClient)
  const { data: lotacoesData } = useLotacoes(apiClient)
  const createMutation = useCreateFuncionario(apiClient)
  const deleteMutation = useDeleteFuncionario(apiClient)

  const cargoOptions = [{ value: '', label: 'Selecione...' }, ...(cargosData?.items ?? []).map((c) => ({ value: c.id, label: `${c.nome} (CBO: ${c.cbo})` }))]
  const lotacaoOptions = [{ value: '', label: 'Selecione...' }, ...(lotacoesData?.items ?? []).map((l) => ({ value: l.id, label: `${l.codigo} - ${l.descricao}` }))]

  const openCreate = useCallback(() => {
    setFormData(EMPTY_FORM)
    setFormErrors({})
    setSheetOpen(true)
  }, [])

  const validate = (): boolean => {
    const errors: Record<string, string> = {}
    if (!formData.nome.trim()) errors.nome = 'Nome é obrigatório'
    if (!formData.cpf || formData.cpf.replace(/\D/g, '').length !== 11) errors.cpf = 'CPF inválido'
    if (!formData.dataAdmissao) errors.dataAdmissao = 'Data de admissão é obrigatória'
    if (!formData.cargoId) errors.cargoId = 'Cargo é obrigatório'
    if (!formData.lotacaoId) errors.lotacaoId = 'Lotação é obrigatória'
    if (!formData.salarioBase || formData.salarioBase <= 0) errors.salarioBase = 'Salário base é obrigatório'
    setFormErrors(errors)
    return Object.keys(errors).length === 0
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!validate()) return
    try {
      await createMutation.mutateAsync(formData)
      toast.success('Funcionário criado com sucesso!')
      setSheetOpen(false)
    } catch {
      toast.error('Erro ao salvar funcionário.')
    }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Tem certeza que deseja excluir este funcionário?')) return
    try {
      await deleteMutation.mutateAsync(id)
      toast.success('Funcionário excluído!')
    } catch {
      toast.error('Erro ao excluir funcionário.')
    }
  }

  // Convert form data for the form component
  const formComponentData = {
    ...formData,
    salarioBase: String(formData.salarioBase),
    dataNascimento: '',
    sexo: '',
    estadoCivil: '',
    nacionalidade: '',
    nomeMae: '',
    nomePai: '',
    tipoContrato: '',
    jornadaHorasSemanais: '',
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

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Funcionários</h1>
          <p className="text-sm text-muted-foreground">
            Gerencie os funcionários. Clique em um funcionário para ver detalhes.
          </p>
        </div>
        <Button onClick={openCreate}>
          <Plus className="mr-2 h-4 w-4" />
          Novo Funcionário
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
            searchPlaceholder="Buscar por nome..."
            onRowClick={(item) => navigate(`/cadastros/funcionarios/${item.id}`)}
            actions={(item) => (
              <div className="flex gap-1">
                <Button variant="ghost" size="sm" onClick={(e) => { e.stopPropagation(); navigate(`/cadastros/funcionarios/${item.id}`) }}>
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

      {/* Create-only Sheet */}
      <Sheet open={sheetOpen} onOpenChange={setSheetOpen}>
        <SheetContent side="right" className="w-full sm:max-w-xl overflow-y-auto">
          <SheetHeader>
            <SheetTitle>Novo Funcionário</SheetTitle>
            <SheetDescription>
              Preencha os dados básicos. Após criar, acesse o funcionário para adicionar documentos, contrato, dependentes e demais dados.
            </SheetDescription>
          </SheetHeader>
          <div className="mt-6">
            <FormContainer
              mode="create"
              onSubmit={handleSubmit}
              onCancel={() => setSheetOpen(false)}
              isSubmitting={createMutation.isPending}
              submitLabel="Criar Funcionário"
            >
              <FuncionarioFormFields
                data={formComponentData}
                onChange={(d) => setFormData({ ...formData, ...d, salarioBase: Number(d.salarioBase) || 0, jornadaHorasSemanais: d.jornadaHorasSemanais ? Number(d.jornadaHorasSemanais) : undefined })}
                errors={formErrors}
                mode="create"
                cargos={cargoOptions}
                lotacoes={lotacaoOptions}
              />
            </FormContainer>
          </div>
        </SheetContent>
      </Sheet>
    </div>
  )
}

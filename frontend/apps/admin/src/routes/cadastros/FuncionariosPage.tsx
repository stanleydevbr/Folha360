import {useState} from 'react';
import {DataTable, Button, Card, Modal, ModalHeader, ModalBody, ModalFooter} from '@folha360/ui';
import {useFuncionarios, useDeleteFuncionario} from '@folha360/api';
import type {FuncionarioDto} from '@folha360/api';
import {formatCpf, formatCurrency} from '@folha360/utils';
import {Plus, Pencil, Trash2} from 'lucide-react';

export function FuncionariosPage() {const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(20);
    const [search, setSearch] = useState('');
    const [modalOpen, setModalOpen] = useState(false);
    const [editingId, setEditingId] = useState<string | null>(null);
    const [deleteId, setDeleteId] = useState<string | null>(null);

    const {data, isLoading} = useFuncionarios({page, pageSize, search});
    const deleteFuncionario = useDeleteFuncionario();

    const openCreate = () =>{setEditingId(null); setModalOpen(true); };
    const openEdit = (id: string) =>{setEditingId(id); setModalOpen(true); };
    const closeModal = () =>{setModalOpen(false); setEditingId(null); };

    return (
        <div>
        <div className="flex items-center justify-between mb-6">
        <h1 className="text-xl font-medium text-ink">Funcionarios</h1>
            <Button variant="primary" size="sm" onClick={openCreate} >
                <Plus className="h-4 w-4"/>Novo Funcionario</Button>
                    </div>

                    <Card variant="default" padding="none">
                        <DataTable
                    columns={
        [
            {key: 'nome', header: 'Nome', sortable: true},
            {key: 'cpf', header: 'CPF', render: (f: FuncionarioDto) =>formatCpf(f.cpf) },
            {key: 'cargo', header: 'Cargo' },
            {key: 'salario', header: 'Salario', sortable: true, render: (f: FuncionarioDto) =>formatCurrency(f.salario) },
            {key: 'situacao', header: 'Situacao',
                render: (f: FuncionarioDto) =>(
                    <span className= {`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium ${f.situacao === 'ativo' ? 'bg-primary/10 text-primary' : 'bg-hairline-cool text-ink-mute'}`
            }>{f.situacao}</span>)
                        },
                    ]
}
data={data?.items ?? []}
totalCount={data?.totalCount ?? 0}
page={page}
pageSize={pageSize}
onPageChange={setPage}
onPageSizeChange={(s) =>{setPageSize(s); setPage(1); }}
onSearch={setSearch}
isLoading={isLoading}
emptyMessage="Nenhum funcionario encontrado"
actions={(f: FuncionarioDto) =>(
    <>
    <Button variant="ghost" size="sm" onClick={() =>openEdit(f.id)}>
        <Pencil className="h-4 w-4"/>
            </Button>
            <Button variant="ghost" size="sm" onClick={() =>setDeleteId(f.id)}>
                <Trash2 className="h-4 w-4 text-accent-tomato"/>
                    </Button>
                    </>)}/>
    </Card>

    <Modal open={modalOpen} onClose={closeModal} >
        <ModalHeader>{editingId? 'Editar Funcionario': 'Novo Funcionario' }</ModalHeader>
        <ModalBody >
        <p className="text-sm text-ink-mute">Formulario de funcionario</p>
            </ModalBody>
            <ModalFooter >
            <Button variant="outline" size="sm" onClick={closeModal}>Cancelar</Button>
                <Button variant="primary" size="sm">Salvar</Button>
                    </ModalFooter>
                    </Modal>

                    <Modal open={!!deleteId} onClose={() =>setDeleteId(null)}>
                        <ModalHeader>Confirmar Exclusao</ModalHeader>
                            <ModalBody >
                            <p className="text-sm text-ink-mute">Tem certeza que deseja excluir este funcionario ?</p>
                                </ModalBody>
                                <ModalFooter >
                                <Button variant="outline" size="sm" onClick={() =>setDeleteId(null)}>Cancelar</Button>
                                    <Button variant="danger" size="sm" onClick={async() =>{if (deleteId) await deleteFuncionario.mutateAsync(deleteId); setDeleteId(null); }}>Excluir</Button>
                                        </ModalFooter>
                                        </Modal>
                                        </div>);
}

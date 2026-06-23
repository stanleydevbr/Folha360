import {useState} from 'react';
import {DataTable, Button, Card, Modal, ModalHeader, ModalBody, ModalFooter, FormInput} from '@folha360/ui';
import {useEmpresas, useCreateEmpresa, useUpdateEmpresa, useDeleteEmpresa} from '@folha360/api';
import type {EmpresaDto} from '@folha360/api';
import {formatCnpj} from '@folha360/utils';
import {Plus, Pencil, Trash2} from 'lucide-react';

export function EmpresasPage() {const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(20);
    const [search, setSearch] = useState('');
    const [modalOpen, setModalOpen] = useState(false);
    const [editingId, setEditingId] = useState<string | null>(null);
    const [deleteId, setDeleteId] = useState<string | null>(null);

    const {data, isLoading} = useEmpresas({page, pageSize, search});
    const createEmpresa = useCreateEmpresa();
    const updateEmpresa = useUpdateEmpresa();
    const deleteEmpresa = useDeleteEmpresa();

    const openCreate = () =>{setEditingId(null); setModalOpen(true); };
    const openEdit = (id: string) =>{setEditingId(id); setModalOpen(true); };
    const closeModal = () =>{setModalOpen(false); setEditingId(null); };

    return (
        <div>
        <div className="flex items-center justify-between mb-6">
        <h1 className="text-xl font-medium text-ink">Empresas</h1>
            <Button variant="primary" size="sm" onClick={openCreate} >
                <Plus className="h-4 w-4"/>Nova Empresa</Button>
                    </div>

                    <Card variant="default" padding="none">
                        <DataTable
                    columns={
        [
            {key: 'cnpj', header: 'CNPJ', render: (e: EmpresaDto) =>formatCnpj(e.cnpj) },
            {key: 'razaoSocial', header: 'Razao Social', sortable: true},
            {key: 'nomeFantasia', header: 'Nome Fantasia' },
            {key: 'ativa', header: 'Situacao',
                render: (e: EmpresaDto) =>(
                    <span className= {`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium ${e.ativa ? 'bg-primary/10 text-primary' : 'bg-hairline-cool text-ink-mute'}`
            }>{e.ativa ? 'Ativa' : 'Inativa' }</span>)
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
emptyMessage="Nenhuma empresa encontrada"
actions={(e: EmpresaDto) =>(
    <>
    <Button variant="ghost" size="sm" onClick={() =>openEdit(e.id)}>
        <Pencil className="h-4 w-4"/>
            </Button>
            <Button variant="ghost" size="sm" onClick={() =>setDeleteId(e.id)}>
                <Trash2 className="h-4 w-4 text-accent-tomato"/>
                    </Button>
                    </>)}/>
    </Card>{/* Create / Edit Modal */}
<EmpresaFormModal
                open={modalOpen}
editingId={editingId}
onClose={closeModal}
onSave={async(data) =>{if (editingId) await updateEmpresa.mutateAsync({id: editingId, ...data});
    else await createEmpresa.mutateAsync(data);
    closeModal();
}}/>{/* Delete Confirmation */}
<Modal open={ !!deleteId} onClose={() =>setDeleteId(null)}>
    <ModalHeader>Confirmar Exclusao</ModalHeader>
        <ModalBody >
        <p className="text-sm text-ink-mute">Tem certeza que deseja excluir esta empresa ? Esta acao nao pode ser desfeita.</p>
            </ModalBody>
            <ModalFooter >
            <Button variant="outline" size="sm" onClick={() =>setDeleteId(null)}>Cancelar</Button>
                <Button variant="danger" size="sm" onClick={async() =>{if (deleteId) await deleteEmpresa.mutateAsync(deleteId); setDeleteId(null); }}>Excluir</Button>
                    </ModalFooter>
                    </Modal>
                    </div>);
}

function EmpresaFormModal({open, editingId, onClose, onSave}: {open: boolean; editingId: string | null; onClose: () =>void; onSave: (data: {cnpj: string; razaoSocial: string; nomeFantasia: string}) =>Promise<void>}) {const [cnpj, setCnpj] = useState('');
    const [razaoSocial, setRazaoSocial] = useState('');
    const [nomeFantasia, setNomeFantasia] = useState('');
    const [saving, setSaving] = useState(false);

    const handleSave = async () =>{setSaving(true);
        try {await onSave({cnpj, razaoSocial, nomeFantasia}); } finally {setSaving(false); }
    };

    return (
        <Modal open= {open} onClose={onClose} >
            <ModalHeader>{editingId? 'Editar Empresa': 'Nova Empresa' }</ModalHeader>
            <ModalBody >
            <div className="space-y-4">
                <FormInput label="CNPJ" name="cnpj" value={cnpj} onChange={(e) =>setCnpj(e.target.value)
} placeholder="00.000.000/0000-00" required/>
    <FormInput label="Razao Social" name="razaoSocial" value={razaoSocial} onChange={(e) =>setRazaoSocial(e.target.value)} placeholder="Razao Social" required/>
        <FormInput label="Nome Fantasia" name="nomeFantasia" value={nomeFantasia} onChange={(e) =>setNomeFantasia(e.target.value)} placeholder="Nome Fantasia" required/>
            </div>
            </ModalBody>
            <ModalFooter >
            <Button variant="outline" size="sm" onClick={onClose}>Cancelar</Button>
                <Button variant="primary" size="sm" onClick={handleSave} loading={saving}>{editingId? 'Salvar': 'Criar' }</Button>
                    </ModalFooter>
                    </Modal>);
}

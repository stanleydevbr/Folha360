import { http, HttpResponse, type JsonValue } from 'msw';

// ─── Types ────────────────────────────────────────────────────
export interface MockEmpresa { id: string; cnpj: string; razaoSocial: string; nomeFantasia: string; ativa: boolean; }
export interface MockFuncionario { id: string; nome: string; cpf: string; cargo: string; salario: number; dataAdmissao: string; situacao: string; }

let empresas: MockEmpresa[] = [
    { id: '1', cnpj: '11222333000181', razaoSocial: 'Empresa ABC Ltda', nomeFantasia: 'ABC', ativa: true },
    { id: '2', cnpj: '99888777000155', razaoSocial: 'XYZ Comercio Ltda', nomeFantasia: 'XYZ', ativa: true },
    { id: '3', cnpj: '55666444000199', razaoSocial: 'Tech Solutions SA', nomeFantasia: 'TechSol', ativa: false },
];

let funcionarios: MockFuncionario[] = [
    { id: '1', nome: 'Ana Silva', cpf: '52998224725', cargo: 'Analista RH', salario: 5000, dataAdmissao: '2024-01-15', situacao: 'ativo' },
    { id: '2', nome: 'Bruno Costa', cpf: '11122233344', cargo: 'Dev', salario: 8000, dataAdmissao: '2024-03-01', situacao: 'ativo' },
    { id: '3', nome: 'Carla Dias', cpf: '55566677788', cargo: 'Analista Contabil', salario: 6000, dataAdmissao: '2024-06-10', situacao: 'ativo' },
];

function paginate<T>(items: T[], page: number, pageSize: number) {
    const start = (page - 1) * pageSize;
    return { items: items.slice(start, start + pageSize), totalCount: items.length, page, pageSize, totalPages: Math.ceil(items.length / pageSize) };
}

// ─── Handlers ──────────────────────────────────────────────────
export const handlers = [
    // ── Auth ──
    http.post('/api/Auth/login', async ({ request }) => {
        const body = await request.json() as JsonValue & { email?: string; password?: string; refreshToken?: string };
        // Support refresh via login endpoint (temporary): if refreshToken present, return refreshed tokens
        if (body?.refreshToken && !body?.email && !body?.password) {
            return HttpResponse.json({
                accessToken: 'mock-jwt-token-refreshado', refreshToken: 'mock-refresh-token',
            });
        }
        if (body?.email === 'admin@folha360.com.br' && body?.password === '123456') {
            return HttpResponse.json({
                accessToken: 'mock-jwt-token', refreshToken: 'mock-refresh-token',
                expiresAt: '2026-12-31T23:59:59Z',
                user: { id: '1', nome: 'Admin', email: body.email, roles: ['Admin'] },
                tenants: [{ id: 't1', nome: 'Empresa ABC', slug: 'abc' }],
            });
        }
        return HttpResponse.json({ message: 'Credenciais invalidas' }, { status: 401 });
    }),

    // ── Dashboard ──
    http.get('/api/dashboard/indicadores', () => {
        return HttpResponse.json({ totalFuncionarios: 150, valorTotalFolha: 450000, eventosPendentes: 12, obrigacoesAVencer: 3 });
    }),

    // ── Empresas ──
    http.get('/api/Empresas', ({ request }) => {
        const url = new URL(request.url);
        const page = parseInt(url.searchParams.get('page') || '1');
        const pageSize = parseInt(url.searchParams.get('pageSize') || '20');
        return HttpResponse.json(paginate(empresas, page, pageSize));
    }),

    http.post('/api/Empresas', async ({ request }) => {
        const data = await request.json() as MockEmpresa;
        const nova = { id: String(empresas.length + 1), ...data, ativa: true };
        empresas.push(nova);
        return HttpResponse.json(nova, { status: 201 });
    }),

    http.put('/api/Empresas/:id', async ({ params, request }) => {
        const data = await request.json() as Partial<MockEmpresa>;
        const idx = empresas.findIndex((e) => e.id === params.id);
        if (idx >= 0) { empresas[idx] = { ...empresas[idx]!, ...data }; return HttpResponse.json(empresas[idx]); }
        return HttpResponse.json({ message: 'Nao encontrada' }, { status: 404 });
    }),

    http.delete('/api/Empresas/:id', ({ params }) => {
        empresas = empresas.filter((e) => e.id !== params.id);
        return HttpResponse.json({});
    }),

    // ── Funcionarios ──
    http.get('/api/Funcionarios', ({ request }) => {
        const url = new URL(request.url);
        const page = parseInt(url.searchParams.get('page') || '1');
        const pageSize = parseInt(url.searchParams.get('pageSize') || '20');
        return HttpResponse.json(paginate(funcionarios, page, pageSize));
    }),

    http.post('/api/Funcionarios', async ({ request }) => {
        const data = await request.json() as MockFuncionario;
        const novo = { id: String(funcionarios.length + 1), ...data };
        funcionarios.push(novo);
        return HttpResponse.json(novo, { status: 201 });
    }),

    http.put('/api/Funcionarios/:id', async ({ params, request }) => {
        const data = await request.json() as Partial<MockFuncionario>;
        const idx = funcionarios.findIndex((f) => f.id === params.id);
        if (idx >= 0) { funcionarios[idx] = { ...funcionarios[idx]!, ...data }; return HttpResponse.json(funcionarios[idx]); }
        return HttpResponse.json({ message: 'Nao encontrado' }, { status: 404 });
    }),

    http.delete('/api/Funcionarios/:id', ({ params }) => {
        funcionarios = funcionarios.filter((f) => f.id !== params.id);
        return HttpResponse.json({});
    }),
];

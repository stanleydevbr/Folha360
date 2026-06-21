import { test as base, type Page } from '@playwright/test';

// Mock data
const mockData = {
    loginSuccess: {
        accessToken: 'mock-jwt-token', refreshToken: 'mock-refresh-token',
        expiresAt: '2026-12-31T23:59:59Z',
        user: { id: '1', nome: 'Admin', email: 'admin@folha360.com.br', roles: ['Admin'] },
        tenants: [{ id: 't1', nome: 'Empresa ABC', slug: 'abc' }],
    },
    empresas: [
        { id: '1', cnpj: '11222333000181', razaoSocial: 'Empresa ABC Ltda', nomeFantasia: 'ABC', ativa: true },
        { id: '2', cnpj: '99888777000155', razaoSocial: 'XYZ Comercio Ltda', nomeFantasia: 'XYZ', ativa: true },
    ],
    funcionarios: [
        { id: '1', nome: 'Ana Silva', cpf: '52998224725', cargo: 'Analista RH', salario: 5000, dataAdmissao: '2024-01-15', situacao: 'ativo' },
        { id: '2', nome: 'Bruno Costa', cpf: '11122233344', cargo: 'Dev', salario: 8000, dataAdmissao: '2024-03-01', situacao: 'ativo' },
    ],
    dashboard: { totalFuncionarios: 150, valorTotalFolha: 450000, eventosPendentes: 12, obrigacoesAVencer: 3 },
};

function paginate(arr: unknown[], page = 1, pageSize = 20) {
    const start = (page - 1) * pageSize;
    return { items: arr.slice(start, start + pageSize), totalCount: arr.length, page, pageSize, totalPages: Math.ceil(arr.length / pageSize) };
}

export async function setupApiMocks(page: Page) {
    // Auth
    await page.route('**/api/Auth/login', async (route) => {
        const body = route.request().postDataJSON();
        if (body?.refreshToken && !body?.email && !body?.password) {
            await route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify({ accessToken: 'new-token', refreshToken: 'new-refresh' }) });
            return;
        }
        if (body?.email === 'admin@folha360.com.br' && body?.password === '123456') {
            await route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(mockData.loginSuccess) });
        } else {
            await route.fulfill({ status: 401, contentType: 'application/json', body: JSON.stringify({ message: 'Credenciais invalidas' }) });
        }
    });

    // Dashboard
    await page.route('**/api/dashboard/indicadores', async (route) => {
        await route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(mockData.dashboard) });
    });

    // Empresas
    await page.route('**/api/Empresas', async (route) => {
        if (route.request().method() === 'GET') {
            const url = new URL(route.request().url());
            const pageN = parseInt(url.searchParams.get('page') || '1');
            const ps = parseInt(url.searchParams.get('pageSize') || '20');
            await route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(paginate(mockData.empresas, pageN, ps)) });
        } else if (route.request().method() === 'POST') {
            await route.fulfill({ status: 201, contentType: 'application/json', body: JSON.stringify({ id: '3', ...route.request().postDataJSON() }) });
        }
    });

    await page.route('**/api/Empresas/*', async (route) => {
        if (route.request().method() === 'DELETE') {
            await route.fulfill({ status: 200, contentType: 'application/json', body: '{}' });
        }
    });

    // Funcionarios
    await page.route('**/api/Funcionarios', async (route) => {
        if (route.request().method() === 'GET') {
            const url = new URL(route.request().url());
            const pageN = parseInt(url.searchParams.get('page') || '1');
            const ps = parseInt(url.searchParams.get('pageSize') || '20');
            await route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(paginate(mockData.funcionarios, pageN, ps)) });
        } else if (route.request().method() === 'POST') {
            await route.fulfill({ status: 201, contentType: 'application/json', body: JSON.stringify({ id: '3', ...route.request().postDataJSON() }) });
        }
    });

    await page.route('**/api/Funcionarios/*', async (route) => {
        if (route.request().method() === 'DELETE') {
            await route.fulfill({ status: 200, contentType: 'application/json', body: '{}' });
        }
    });
}

// Test fixture with auto-mocked API
export const test = base.extend({
    page: async ({ page }, use) => {
        await setupApiMocks(page);
        await use(page);
    },
});

export { expect } from '@playwright/test';

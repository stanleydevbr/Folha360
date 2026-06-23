import { test, expect } from '../fixtures';

test.describe('CRUD Empresas', () => {
    test.beforeEach(async ({ page }) => {
        await page.goto('/login');
        await page.fill('input[type="email"]', 'admin@folha360.com.br');
        await page.fill('input[type="password"]', '123456');
        await page.click('button[type="submit"]');
        await expect(page).toHaveURL(/\/dashboard/);
        await page.click('text=Empresas');
    });

    test('lista empresas na tabela', async ({ page }) => {
        await expect(page.locator('table')).toBeVisible();
        await expect(page.locator('text=Empresa ABC Ltda')).toBeVisible();
    });

    test('cria nova empresa', async ({ page }) => {
        await page.click('text=Nova Empresa');
        await page.fill('input[name="cnpj"]', '66777888000122');
        await page.fill('input[name="razaoSocial"]', 'Nova Empresa Ltda');
        await page.fill('input[name="nomeFantasia"]', 'Nova');
        await page.click('button:has-text("Criar")');
        await expect(page.locator('text=Nova Empresa Ltda')).toBeVisible();
    });

    test('exclui empresa', async ({ page }) => {
        await page.click('button[aria-label*="Selecionar item"]');
        // Click delete button for first row
        const deleteBtns = page.locator('button:has(svg[class*="lucide-trash2"])');
        await deleteBtns.first().click();
        await expect(page.locator('text=Confirmar Exclusao')).toBeVisible();
        await page.click('button:has-text("Excluir")');
    });
});

test.describe('CRUD Funcionarios', () => {
    test.beforeEach(async ({ page }) => {
        await page.goto('/login');
        await page.fill('input[type="email"]', 'admin@folha360.com.br');
        await page.fill('input[type="password"]', '123456');
        await page.click('button[type="submit"]');
        await expect(page).toHaveURL(/\/dashboard/);
        await page.click('text=Cadastros');
        await page.click('text=Funcionarios');
    });

    test('lista funcionarios na tabela', async ({ page }) => {
        await expect(page.locator('table')).toBeVisible();
        await expect(page.locator('text=Ana Silva')).toBeVisible();
    });

    test('exclui funcionario', async ({ page }) => {
        const deleteBtns = page.locator('button:has(svg[class*="lucide-trash2"])');
        await deleteBtns.first().click();
        await expect(page.locator('text=Confirmar Exclusao')).toBeVisible();
        await page.click('button:has-text("Excluir")');
    });
});

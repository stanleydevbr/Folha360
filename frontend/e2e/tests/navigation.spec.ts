import { test, expect } from '../fixtures';

test.describe('Navegacao', () => {
    test.beforeEach(async ({ page }) => {
        await page.goto('/login');
        await page.fill('input[type="email"]', 'admin@folha360.com.br');
        await page.fill('input[type="password"]', '123456');
        await page.click('button[type="submit"]');
        await expect(page).toHaveURL(/\/dashboard/);
    });

    test('sidebar mostra todas as secoes', async ({ page }) => {
        await expect(page.locator('nav[aria-label="Navegacao principal"]')).toBeVisible();
        const secoes = ['Dashboard', 'Cadastros', 'Eventos', 'Processamento', 'Fiscais', 'Relatorios', 'eSocial'];
        for (const secao of secoes) {
            await expect(page.locator(`text=${secao}`).first()).toBeVisible();
        }
    });

    test('navega para cadastros/empresas', async ({ page }) => {
        await page.click('text=Empresas');
        await expect(page).toHaveURL(/\/cadastros\/empresas/);
        await expect(page.locator('h1')).toContainText('Empresas');
    });

    test('navega para cadastros/funcionarios', async ({ page }) => {
        await page.click('text=Cadastros');
        await page.click('text=Funcionarios');
        await expect(page).toHaveURL(/\/cadastros\/funcionarios/);
        await expect(page.locator('h1')).toContainText('Funcionarios');
    });

    test('sidebar alterna entre colapsado e expandido', async ({ page }) => {
        const toggleBtn = page.locator('button[aria-label="Recolher sidebar"]');
        await toggleBtn.click();
        // After collapsing, sidebar should be narrower
        await expect(toggleBtn).toBeVisible();
        // Toggle back
        await page.locator('button[aria-label="Expandir sidebar"]').click();
    });
});

import { test, expect } from '../fixtures';

test.describe('Autenticacao', () => {
    test('login com credenciais validas redireciona para dashboard', async ({ page }) => {
        await page.goto('/login');
        await page.fill('input[type="email"]', 'admin@folha360.com.br');
        await page.fill('input[type="password"]', '123456');
        await page.click('button[type="submit"]');
        await expect(page).toHaveURL(/\/dashboard/);
        await expect(page.locator('text=Dashboard')).toBeVisible();
    });

    test('login com credenciais invalidas mostra erro', async ({ page }) => {
        await page.goto('/login');
        await page.fill('input[type="email"]', 'admin@folha360.com.br');
        await page.fill('input[type="password"]', 'senha-errada');
        await page.click('button[type="submit"]');
        await expect(page.locator('[role="alert"]')).toContainText(/invalido/i);
    });

    test('usuario logado e redirecionado ao acessar login', async ({ page }) => {
        // First login
        await page.goto('/login');
        await page.fill('input[type="email"]', 'admin@folha360.com.br');
        await page.fill('input[type="password"]', '123456');
        await page.click('button[type="submit"]');
        await expect(page).toHaveURL(/\/dashboard/);
        // Then try to go to login
        await page.goto('/login');
        await expect(page).toHaveURL(/\/dashboard/);
    });
});

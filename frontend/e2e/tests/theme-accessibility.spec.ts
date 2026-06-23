import { test, expect } from '../fixtures';
import AxeBuilder from '@axe-core/playwright';

test.describe('Tema', () => {
    test.beforeEach(async ({ page }) => {
        await page.goto('/login');
        await page.fill('input[type="email"]', 'admin@folha360.com.br');
        await page.fill('input[type="password"]', '123456');
        await page.click('button[type="submit"]');
        await expect(page).toHaveURL(/\/dashboard/);
    });

    test('alterna para modo escuro', async ({ page }) => {
        const temaBtn = page.locator('button[aria-label="Modo escuro"]');
        await temaBtn.click();
        await expect(page.locator('html')).toHaveClass(/dark/);
        // O icone deve mudar para sol
        await expect(page.locator('button[aria-label="Modo claro"]')).toBeVisible();
    });

    test('alterna para modo claro', async ({ page }) => {
        // First go dark
        await page.locator('button[aria-label="Modo escuro"]').click();
        await expect(page.locator('html')).toHaveClass(/dark/);
        // Then back to light
        await page.locator('button[aria-label="Modo claro"]').click();
        await expect(page.locator('html')).not.toHaveClass(/dark/);
    });
});

test.describe('Responsividade', () => {
    test('layout funciona em viewport desktop', async ({ page }) => {
        await page.setViewportSize({ width: 1280, height: 720 });
        await page.goto('/login');
        await page.fill('input[type="email"]', 'admin@folha360.com.br');
        await page.fill('input[type="password"]', '123456');
        await page.click('button[type="submit"]');
        await expect(page).toHaveURL(/\/dashboard/);
        await expect(page.locator('nav[aria-label="Navegacao principal"]')).toBeVisible();
    });

    test('layout funciona em viewport tablet', async ({ page }) => {
        await page.setViewportSize({ width: 768, height: 1024 });
        await page.goto('/login');
        await page.fill('input[type="email"]', 'admin@folha360.com.br');
        await page.fill('input[type="password"]', '123456');
        await page.click('button[type="submit"]');
        await expect(page).toHaveURL(/\/dashboard/);
    });
});

test.describe('Acessibilidade', () => {
    test('nao tem violacoes de acessibilidade na pagina de login', async ({ page }) => {
        await page.goto('/login');
        await page.waitForLoadState('networkidle');
        const results = await new AxeBuilder({ page }).analyze();
        expect(results.violations.length).toBe(0);
    });

    test('nao tem violacoes de acessibilidade no dashboard', async ({ page }) => {
        await page.goto('/login');
        await page.fill('input[type="email"]', 'admin@folha360.com.br');
        await page.fill('input[type="password"]', '123456');
        await page.click('button[type="submit"]');
        await expect(page).toHaveURL(/\/dashboard/);
        await page.waitForLoadState('networkidle');
        const results = await new AxeBuilder({ page }).analyze();
        expect(results.violations.length).toBe(0);
    });
});

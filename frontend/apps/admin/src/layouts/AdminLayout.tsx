import { useState, useCallback } from 'react';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import { Sidebar, Header, Footer, type NavItem } from '@folha360/ui';
import { useAuth } from '../providers/AuthProvider';
import { useTheme } from '../providers/ThemeProvider';
import {
  LayoutDashboard,
  Users,
  Calendar,
  Calculator,
  FileText,
  BarChart3,
  Upload,
} from 'lucide-react';

const navItems: NavItem[] = [
  {
    label: 'Dashboard',
    path: '/dashboard',
    icon: <LayoutDashboard className="h-5 w-5" />,
  },
  {
    label: 'Cadastros',
    path: '/cadastros',
    icon: <Users className="h-5 w-5" />,
    children: [
      { label: 'Empresas', path: '/cadastros/empresas', icon: <Users className="h-4 w-4" /> },
      { label: 'Funcionários', path: '/cadastros/funcionarios', icon: <Users className="h-4 w-4" /> },
    ],
  },
  { label: 'Eventos', path: '/eventos', icon: <Calendar className="h-5 w-5" /> },
  { label: 'Processamento', path: '/processamento', icon: <Calculator className="h-5 w-5" /> },
  { label: 'Fiscais', path: '/fiscais', icon: <FileText className="h-5 w-5" /> },
  { label: 'Relatórios', path: '/relatorios', icon: <BarChart3 className="h-5 w-5" /> },
  { label: 'eSocial', path: '/esocial', icon: <Upload className="h-5 w-5" /> },
];

/**
 * AdminLTE v4 layout wrapper:
 *   .layout-wrapper > .main-sidebar + .content-wrapper
 *   .content-wrapper > .main-header + .content-header? + .content + .main-footer
 */
export function AdminLayout() {
  const [sidebarCollapsed, setSidebarCollapsed] = useState(() => {
    try {
      return localStorage.getItem('folha360_sidebar') === 'collapsed';
    } catch {
      return false;
    }
  });
  const navigate = useNavigate();
  const location = useLocation();
  const { user, logout } = useAuth();
  const { theme, setTheme } = useTheme();

  const toggleSidebar = useCallback(() => {
    setSidebarCollapsed((prev) => {
      const next = !prev;
      try {
        localStorage.setItem('folha360_sidebar', next ? 'collapsed' : 'expanded');
      } catch {
        /** ignore */
      }
      return next;
    });
  }, []);

  const handleNavigate = useCallback((path: string) => navigate(path), [navigate]);

  // Gera breadcrumbs baseados na rota atual (AdminLTE .content-header style)
  const pathParts = location.pathname.split('/').filter(Boolean);
  const breadcrumbs = pathParts.map((part, i) => ({
    label: part.charAt(0).toUpperCase() + part.slice(1),
    path: i < pathParts.length - 1 ? '/' + pathParts.slice(0, i + 1).join('/') : undefined,
  }));

  return (
    <div className="layout-wrapper flex h-screen bg-[#f4f6f9]">
      {/* .main-sidebar */}
      <Sidebar
        items={navItems}
        activePath={location.pathname}
        onNavigate={handleNavigate}
        collapsed={sidebarCollapsed}
        onToggle={toggleSidebar}
        title="Folha360"
        logo={
          <div className="w-8 h-8 rounded-lg bg-[#3ecf8e] flex items-center justify-center text-[#171717] text-xs font-bold">
            F
          </div>
        }
      />

      {/* .content-wrapper */}
      <div className="content-wrapper flex-1 flex flex-col min-w-0">
        {/* .main-header (navbar) */}
        <Header
          onToggleSidebar={toggleSidebar}
          sidebarCollapsed={sidebarCollapsed}
          breadcrumbs={breadcrumbs}
          user={user ? { name: user.nome, email: user.email } : null}
          theme={theme === 'dark' ? 'dark' : 'light'}
          onThemeToggle={() => setTheme(theme === 'dark' ? 'light' : 'dark')}
          onLogout={logout}
        />

        {/* .content (main area) */}
        <main className="content flex-1 overflow-y-auto p-6 bg-[#f4f6f9]">
          <div className="container-fluid mx-auto">
            <Outlet />
          </div>
        </main>

        {/* .main-footer */}
        <Footer
          copyright={`© ${new Date().getFullYear()} Folha360`}
          version="0.1.0"
          links={[
            { label: 'Termos', href: '#' },
            { label: 'Suporte', href: '#' },
          ]}
        />
      </div>
    </div>
  );
}

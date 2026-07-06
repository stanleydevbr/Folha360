import { useNavigate, useLocation } from 'react-router'
import { Navigation } from '@folha360/ui'
import { useTheme } from '../providers/ThemeProvider'
import { useNavigationItems } from '../hooks/useNavigationItems'
import { ThemeConfigPanel } from './ThemeConfigPanel'

export function Sidebar() {
  const { theme } = useTheme()
  const navigate = useNavigate()
  const location = useLocation()
  const navItems = useNavigationItems()
  const collapsed = theme.sidebarCollapsed

  return (
    <aside
      className="hidden shrink-0 flex-col border-r transition-all duration-300 lg:flex"
      style={{
        width: collapsed ? '80px' : '260px',
        backgroundColor: 'var(--color-sidebar-bg)',
        color: 'var(--color-sidebar-text)',
      }}
    >
      <div className="flex h-20 items-center justify-center border-b px-4">
        {collapsed ? (
          <span className="text-lg font-bold">F3</span>
        ) : (
          <span className="text-lg font-bold">Folha360</span>
        )}
      </div>

      <div className="flex-1 overflow-y-auto">
        <Navigation
          items={navItems}
          currentPath={location.pathname}
          collapsed={collapsed}
          onNavigate={(route: string) => navigate(route)}
        />
      </div>

      <div className="border-t p-3">
        <ThemeConfigPanel collapsed={collapsed} />
      </div>
    </aside>
  )
}

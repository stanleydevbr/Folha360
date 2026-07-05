import { useNavigate } from 'react-router'
import {
  Menu,
  PanelLeftClose,
  PanelLeftOpen,
  Bell,
  User,
  Settings,
  LogOut,
} from 'lucide-react'
import { Button } from '@folha360/ui'
import {
  DropdownMenu,
  DropdownMenuTrigger,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuLabel,
  DropdownMenuGroup,
} from '@folha360/ui'
import { PageBreadcrumb } from '@folha360/ui'
import { useTheme } from '../providers/ThemeProvider'
import { useAuth } from '../providers/AuthProvider'
import { useNavigationItems } from '../hooks/useNavigationItems'
import { useBreadcrumbs } from '../hooks/useBreadcrumbs'

function UserAvatar({ nome, size = 'md' }: { nome: string; size?: 'sm' | 'md' }) {
  const initials = nome
    .split(' ')
    .map((n) => n[0])
    .slice(0, 2)
    .join('')
    .toUpperCase()

  const sizeClasses = size === 'sm' ? 'h-8 w-8 text-xs' : 'h-10 w-10 text-sm'

  return (
    <div
      className={`${sizeClasses} flex shrink-0 items-center justify-center rounded-full bg-primary font-medium text-primary-foreground`}
    >
      {initials}
    </div>
  )
}

export function Header() {
  const { theme, toggleSidebar } = useTheme()
  const { user, logout } = useAuth()
  const navigate = useNavigate()
  const navItems = useNavigationItems()
  const breadcrumbItems = useBreadcrumbs(navItems)

  return (
    <header
      className="flex h-20 shrink-0 items-center justify-between border-b px-6"
      style={{
        backgroundColor: 'var(--color-header-bg)',
        color: 'var(--color-header-text)',
      }}
    >
      <div className="flex items-center gap-4">
        <Button
          variant="ghost"
          size="icon"
          onClick={toggleSidebar}
          className="hidden lg:flex"
        >
          {theme.sidebarCollapsed ? (
            <PanelLeftOpen className="h-5 w-5" />
          ) : (
            <PanelLeftClose className="h-5 w-5" />
          )}
        </Button>
        <Button variant="ghost" size="icon" className="lg:hidden">
          <Menu className="h-5 w-5" />
        </Button>
        <PageBreadcrumb
          items={breadcrumbItems}
          onNavigate={(route: string) => navigate(route)}
        />
      </div>

      <div className="flex items-center gap-3">
        {/* Notifications */}
        <Button variant="ghost" size="icon" className="relative">
          <Bell className="h-5 w-5" />
          <span className="absolute -top-0.5 -right-0.5 flex h-4 w-4 items-center justify-center rounded-full bg-destructive text-[10px] font-bold text-destructive-foreground">
            3
          </span>
        </Button>

        {/* User dropdown */}
        <DropdownMenu>
          <DropdownMenuTrigger>
            <span className="flex items-center gap-2 rounded-full p-1 transition-colors hover:bg-muted/50">
              <UserAvatar nome={user?.nome ?? 'U'} />
              <div className="hidden text-left md:block">
                <p className="text-sm font-medium leading-tight">{user?.nome}</p>
                <p className="text-xs text-muted-foreground">{user?.email}</p>
              </div>
            </span>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-56">
            <DropdownMenuGroup>
              <DropdownMenuLabel className="font-normal">
                <div className="flex flex-col gap-1">
                  <p className="text-sm font-medium">{user?.nome}</p>
                  <p className="text-xs text-muted-foreground">{user?.email}</p>
                  <div className="mt-1 flex gap-1">
                    {user?.roles?.map((role) => (
                      <span
                        key={role}
                        className="rounded-full bg-primary/10 px-2 py-0.5 text-[10px] font-medium text-primary"
                      >
                        {role}
                      </span>
                    ))}
                  </div>
                </div>
              </DropdownMenuLabel>
            </DropdownMenuGroup>
            <DropdownMenuSeparator />
            <DropdownMenuItem onClick={() => navigate('/perfil')}>
              <User className="mr-2 h-4 w-4" />
              Editar Perfil
            </DropdownMenuItem>
            <DropdownMenuItem onClick={() => navigate('/configuracoes')}>
              <Settings className="mr-2 h-4 w-4" />
              Configurações
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <DropdownMenuItem onClick={logout} className="text-destructive">
              <LogOut className="mr-2 h-4 w-4" />
              Sair
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </header>
  )
}

import { createContext, useContext, useState, useCallback, type ReactNode } from 'react'
import { createApiClient, type ApiClient, type UserDto, type TenantDto } from '@folha360/api'

interface AuthState {
  user: UserDto | null
  token: string | null
  tenant: TenantDto | null
  isAuthenticated: boolean
  isLoading: boolean
}

interface AuthContextValue extends AuthState {
  login: (token: string, user: UserDto, tenants: TenantDto[]) => void
  logout: () => void
  setTenant: (tenant: TenantDto) => void
  apiClient: ApiClient
}

const AuthContext = createContext<AuthContextValue | null>(null)

function loadAuth(): { token: string | null; user: UserDto | null; tenant: TenantDto | null } {
  try {
    const token = localStorage.getItem('folha360-token')
    const user = JSON.parse(localStorage.getItem('folha360-user') || 'null')
    const tenant = JSON.parse(localStorage.getItem('folha360-tenant') || 'null')
    return { token, user, tenant }
  } catch {
    return { token: null, user: null, tenant: null }
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<AuthState>(() => {
    const { token, user, tenant } = loadAuth()
    return {
      user,
      token,
      tenant,
      isAuthenticated: !!token && !!user,
      isLoading: false,
    }
  })

  const apiClient = createApiClient({
    baseURL: import.meta.env.VITE_API_URL || '',
    getToken: () => state.token,
    onUnauthorized: () => {
      setState((s) => ({ ...s, token: null, user: null, tenant: null, isAuthenticated: false }))
      localStorage.removeItem('folha360-token')
      localStorage.removeItem('folha360-user')
      localStorage.removeItem('folha360-tenant')
    },
  })

  const login = useCallback((token: string, user: UserDto, tenants: TenantDto[]) => {
    localStorage.setItem('folha360-token', token)
    localStorage.setItem('folha360-user', JSON.stringify(user))
    const defaultTenant = tenants[0] || null
    if (defaultTenant) {
      localStorage.setItem('folha360-tenant', JSON.stringify(defaultTenant))
    }
    setState({ token, user, tenant: defaultTenant, isAuthenticated: true, isLoading: false })
  }, [])

  const logout = useCallback(() => {
    localStorage.removeItem('folha360-token')
    localStorage.removeItem('folha360-user')
    localStorage.removeItem('folha360-tenant')
    setState({ token: null, user: null, tenant: null, isAuthenticated: false, isLoading: false })
  }, [])

  const setTenant = useCallback((tenant: TenantDto) => {
    localStorage.setItem('folha360-tenant', JSON.stringify(tenant))
    setState((s) => ({ ...s, tenant }))
  }, [])

  return (
    <AuthContext.Provider value={{ ...state, login, logout, setTenant, apiClient }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}

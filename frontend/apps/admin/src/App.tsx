import { Routes, Route, Navigate } from 'react-router'
import { lazy, Suspense } from 'react'
import { AdminLayout } from './layouts/AdminLayout'
import { AuthLayout } from './layouts/AuthLayout'
import { ProtectedRoute } from './components/ProtectedRoute'
import { LoginPage } from './routes/auth/LoginPage'
import { NavigationProgress } from './components/NavigationProgress'

const DashboardPage = lazy(() => import('./routes/dashboard/DashboardPage'))
const EmpresasPage = lazy(() => import('./routes/cadastros/EmpresasPage'))
const FuncionariosPage = lazy(() => import('./routes/cadastros/FuncionariosPage'))
const EventosPage = lazy(() => import('./routes/eventos/EventosPage'))
const ProcessamentoPage = lazy(() => import('./routes/processamento/ProcessamentoPage'))
const FiscaisPage = lazy(() => import('./routes/fiscais/FiscaisPage'))
const RelatoriosPage = lazy(() => import('./routes/relatorios/RelatoriosPage'))
const EsocialPage = lazy(() => import('./routes/esocial/EsocialPage'))
const PerfilPage = lazy(() => import('./routes/perfil/PerfilPage'))

function PageLoader() {
  return (
    <div className="flex items-center justify-center py-20">
      <div className="h-8 w-8 animate-spin rounded-full border-4 border-primary border-t-transparent" />
    </div>
  )
}

export function App() {
  return (
    <>
      <NavigationProgress />
      <Suspense fallback={<PageLoader />}>
        <Routes>
          <Route element={<AuthLayout />}>
            <Route path="/login" element={<LoginPage />} />
          </Route>
          <Route element={<ProtectedRoute />}>
            <Route element={<AdminLayout />}>
              <Route path="/dashboard" element={<DashboardPage />} />
              <Route path="/cadastros/empresas" element={<EmpresasPage />} />
              <Route path="/cadastros/funcionarios" element={<FuncionariosPage />} />
              <Route path="/eventos" element={<EventosPage />} />
              <Route path="/processamento" element={<ProcessamentoPage />} />
              <Route path="/fiscais" element={<FiscaisPage />} />
              <Route path="/relatorios" element={<RelatoriosPage />} />
              <Route path="/esocial" element={<EsocialPage />} />
              <Route path="/perfil" element={<PerfilPage />} />
            </Route>
          </Route>
          <Route path="*" element={<Navigate to="/dashboard" replace />} />
        </Routes>
      </Suspense>
    </>
  )
}

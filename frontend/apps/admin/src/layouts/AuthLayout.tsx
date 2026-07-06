import { Outlet } from 'react-router'

export function AuthLayout() {
  return (
    <div className="flex min-h-screen items-center justify-center bg-muted">
      <div className="w-full max-w-md rounded-lg border bg-card p-8 shadow-sm">
        <div className="mb-6 text-center">
          <h1 className="text-2xl font-bold">Folha360</h1>
          <p className="text-sm text-muted-foreground">Sistema de Gestão de Folha de Pagamento</p>
        </div>
        <Outlet />
      </div>
    </div>
  )
}

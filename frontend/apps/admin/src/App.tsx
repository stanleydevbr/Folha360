import { Routes, Route, Navigate } from "react-router-dom";
import { ThemeProvider } from "./providers/ThemeProvider";
import { AdminLayout } from "./layouts/AdminLayout";
import { PrivateRoute, PublicRoute } from "./lib/PrivateRoute";
import { LoginPage } from "./routes/auth/LoginPage";
import { SelectTenantPage } from "./routes/auth/SelectTenantPage";
import { EmpresasPage } from "./routes/cadastros/EmpresasPage";
import { FuncionariosPage } from "./routes/cadastros/FuncionariosPage";
import { DashboardPage } from "./routes/dashboard/DashboardPage";

export function App() {
  return (
    <ThemeProvider>
      <Routes>
        <Route path="/login" element={<PublicRoute><LoginPage /></PublicRoute>} />
        <Route path="/select-tenant" element={<PrivateRoute><SelectTenantPage /></PrivateRoute>} />
        <Route path="/" element={<PrivateRoute><AdminLayout /></PrivateRoute>}>
          <Route index element={<Navigate to="/dashboard" replace />} />
          <Route path="dashboard" element={<DashboardPage />} />
          <Route path="cadastros/empresas" element={<EmpresasPage />} />
          <Route path="cadastros/funcionarios" element={<FuncionariosPage />} />
          <Route path="eventos" element={<div className="p-8"><h1 className="text-2xl font-medium">Eventos</h1></div>} />
          <Route path="processamento" element={<div className="p-8"><h1 className="text-2xl font-medium">Processamento</h1></div>} />
          <Route path="fiscais" element={<div className="p-8"><h1 className="text-2xl font-medium">Fiscais</h1></div>} />
          <Route path="relatorios" element={<div className="p-8"><h1 className="text-2xl font-medium">Relatorios</h1></div>} />
          <Route path="esocial" element={<div className="p-8"><h1 className="text-2xl font-medium">eSocial</h1></div>} />
        </Route>
        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </ThemeProvider>
  );
}
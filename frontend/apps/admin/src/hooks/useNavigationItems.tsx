import { useMemo } from 'react'
import {
  LayoutDashboard,
  Building2,
  Users,
  Briefcase,
  MapPin,
  Clock,
  HeartHandshake,
  ShieldCheck,
  Calculator,
  FileText,
  BarChart3,
  FileSpreadsheet,
  CalendarClock,
  DollarSign,
  Settings,
  ChevronRight,
} from 'lucide-react'
import type { NavItem } from '@folha360/ui'

export function useNavigationItems(): NavItem[] {
  return useMemo(
    () => [
      {
        id: 'dashboard',
        type: 'item',
        title: 'Dashboard',
        route: '/dashboard',
        icon: <LayoutDashboard className="h-5 w-5" />,
      },
      {
        id: 'cadastros',
        type: 'collapse',
        title: 'Cadastros',
        icon: <Building2 className="h-5 w-5" />,
        children: [
          { id: 'empresas', type: 'item', title: 'Empresas', route: '/cadastros/empresas' },
          { id: 'funcionarios', type: 'item', title: 'Funcionários', route: '/cadastros/funcionarios' },
          { id: 'cargos', type: 'item', title: 'Cargos', route: '/cadastros/cargos' },
          { id: 'lotacoes', type: 'item', title: 'Lotações', route: '/cadastros/lotacoes' },
          { id: 'sindicatos', type: 'item', title: 'Sindicatos', route: '/cadastros/sindicatos' },
          { id: 'convenios', type: 'item', title: 'Convênios', route: '/cadastros/convenios' },
          { id: 'horarios', type: 'item', title: 'Horários', route: '/cadastros/horarios' },
        ],
      },
      {
        id: 'rubricas',
        type: 'item',
        title: 'Rubricas',
        route: '/cadastros/rubricas',
        icon: <DollarSign className="h-5 w-5" />,
      },
      {
        id: 'eventos',
        type: 'item',
        title: 'Eventos',
        route: '/eventos',
        icon: <CalendarClock className="h-5 w-5" />,
      },
      {
        id: 'processamento',
        type: 'item',
        title: 'Processamento',
        route: '/processamento',
        icon: <Calculator className="h-5 w-5" />,
      },
      {
        id: 'fiscais',
        type: 'item',
        title: 'Fiscais',
        route: '/fiscais',
        icon: <FileText className="h-5 w-5" />,
      },
      {
        id: 'relatorios',
        type: 'collapse',
        title: 'Relatórios',
        icon: <BarChart3 className="h-5 w-5" />,
        children: [
          { id: 'relatorios-folha', type: 'item', title: 'Folha', route: '/relatorios/folha' },
          { id: 'relatorios-exportacoes', type: 'item', title: 'Exportações', route: '/relatorios/exportacoes' },
        ],
      },
      {
        id: 'esocial',
        type: 'item',
        title: 'eSocial',
        route: '/esocial',
        icon: <FileSpreadsheet className="h-5 w-5" />,
      },
    ],
    [],
  )
}

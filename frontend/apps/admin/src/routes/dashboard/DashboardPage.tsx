import { useDashboardIndicadores } from '@folha360/api';
import { Users, DollarSign, AlertCircle, Calendar, TrendingUp, TrendingDown } from 'lucide-react';
import { formatCurrency } from '@folha360/utils';

const indicators = [
  {
    label: 'Total de Funcionários',
    icon: Users,
    format: (v: number) => v.toLocaleString('pt-BR'),
    bgColor: 'bg-indigo-100',
    iconColor: 'text-indigo-600',
    trend: 12,
    trendUp: true,
  },
  {
    label: 'Folha do Mês',
    icon: DollarSign,
    format: (v: number) => formatCurrency(v),
    bgColor: 'bg-emerald-100',
    iconColor: 'text-emerald-600',
    trend: 3.5,
    trendUp: false,
  },
  {
    label: 'Eventos Pendentes',
    icon: AlertCircle,
    format: (v: number) => v.toLocaleString('pt-BR'),
    bgColor: 'bg-amber-100',
    iconColor: 'text-amber-600',
    trend: 8,
    trendUp: true,
  },
  {
    label: 'Obrigações a Vencer',
    icon: Calendar,
    format: (v: number) => v.toLocaleString('pt-BR'),
    bgColor: 'bg-rose-100',
    iconColor: 'text-rose-600',
    trend: 2,
    trendUp: false,
  },
];

/**
 * Dashboard com cards estilo AdminLTE v4 — .card com card-header, card-body,
 * indicadores com trend, tabelas de vencimentos e eventos.
 */
export function DashboardPage() {
  const { data, isLoading } = useDashboardIndicadores();

  return (
    <div className="space-y-6">
      {/* Info cards row — AdminLTE small-box style */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        {indicators.map((item) => {
          const Icon = item.icon;
          const value = data
            ? item.format(
                data[
                  item.label === 'Total de Funcionários'
                    ? 'totalFuncionarios'
                    : item.label === 'Folha do Mês'
                      ? 'valorTotalFolha'
                      : item.label === 'Eventos Pendentes'
                        ? 'eventosPendentes'
                        : ('obrigacoesAVencer' as any)
                ],
              )
            : '-';
          return (
            <div
              key={item.label}
              className="bg-white rounded-lg border border-gray-200 shadow-sm"
            >
              {/* Card body */}
              <div className="p-5">
                <div className="flex items-start justify-between">
                  <div className="space-y-1">
                    <p className="text-sm font-medium text-gray-500">{item.label}</p>
                    {isLoading ? (
                      <div className="h-7 w-24 bg-gray-200 rounded animate-pulse" />
                    ) : (
                      <p className="text-2xl font-bold text-gray-800">{value}</p>
                    )}
                    <div className="flex items-center gap-1 pt-1">
                      {item.trendUp ? (
                        <TrendingUp className="h-3.5 w-3.5 text-emerald-500" />
                      ) : (
                        <TrendingDown className="h-3.5 w-3.5 text-rose-500" />
                      )}
                      <span
                        className={`text-xs font-medium ${item.trendUp ? 'text-emerald-600' : 'text-rose-600'}`}
                      >
                        {item.trend}%
                      </span>
                      <span className="text-xs text-gray-400">vs. mês anterior</span>
                    </div>
                  </div>
                  <div className={`p-3 rounded-lg ${item.bgColor}`}>
                    <Icon className={`h-6 w-6 ${item.iconColor}`} />
                  </div>
                </div>
              </div>
            </div>
          );
        })}
      </div>

      {/* Bottom row — AdminLTE .card style */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
        {/* Card: Próximos Vencimentos */}
        <div className="card bg-white rounded-lg border border-gray-200 shadow-sm">
          <div className="card-header px-5 py-3 border-b border-gray-200">
            <h3 className="text-base font-semibold text-gray-800">Próximos Vencimentos</h3>
          </div>
          <div className="card-body p-5">
            <div className="space-y-3">
              {[
                { nome: 'INSS', data: '05/07/2026', valor: 'R$ 32.450,00' },
                { nome: 'FGTS', data: '07/07/2026', valor: 'R$ 18.920,00' },
                { nome: 'IRRF', data: '10/07/2026', valor: 'R$ 8.340,00' },
              ].map((item) => (
                <div
                  key={item.nome}
                  className="flex items-center justify-between py-2 border-b border-gray-100 last:border-0"
                >
                  <div>
                    <p className="text-sm font-medium text-gray-700">{item.nome}</p>
                    <p className="text-xs text-gray-400">Vencimento: {item.data}</p>
                  </div>
                  <span className="text-sm font-semibold text-gray-800">{item.valor}</span>
                </div>
              ))}
            </div>
          </div>
        </div>

        {/* Card: Últimos Eventos */}
        <div className="card bg-white rounded-lg border border-gray-200 shadow-sm">
          <div className="card-header px-5 py-3 border-b border-gray-200">
            <h3 className="text-base font-semibold text-gray-800">Últimos Eventos</h3>
          </div>
          <div className="card-body p-5">
            <div className="space-y-3">
              {[
                { tipo: 'Admissão', funcionario: 'Carlos Silva', data: '20/06/2026' },
                { tipo: 'Férias', funcionario: 'Maria Souza', data: '18/06/2026' },
                { tipo: 'Desligamento', funcionario: 'João Pereira', data: '15/06/2026' },
              ].map((item, i) => (
                <div
                  key={i}
                  className="flex items-center justify-between py-2 border-b border-gray-100 last:border-0"
                >
                  <div className="flex items-center gap-3">
                    <span
                      className={`px-2 py-0.5 rounded text-xs font-medium ${
                        item.tipo === 'Admissão'
                          ? 'bg-emerald-100 text-emerald-700'
                          : item.tipo === 'Férias'
                            ? 'bg-amber-100 text-amber-700'
                            : 'bg-rose-100 text-rose-700'
                      }`}
                    >
                      {item.tipo}
                    </span>
                    <p className="text-sm text-gray-700">{item.funcionario}</p>
                  </div>
                  <span className="text-xs text-gray-400">{item.data}</span>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

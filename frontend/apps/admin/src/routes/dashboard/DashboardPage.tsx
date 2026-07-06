import { Card, CardContent, CardHeader, CardTitle } from '@folha360/ui'
import { Users, Calculator, CalendarClock, AlertTriangle } from 'lucide-react'

const metrics = [
  { title: 'Funcionários Ativos', value: '1.247', change: '+12', icon: Users },
  { title: 'Folha do Mês', value: 'R$ 2.847.350', change: '+3.2%', icon: Calculator },
  { title: 'Eventos Pendentes', value: '23', change: '-5', icon: CalendarClock },
  { title: 'Vencimentos Fiscais', value: '4', change: 'Esta semana', icon: AlertTriangle },
]

export default function DashboardPage() {
  return (
    <div>
      <h1 className="mb-6 text-2xl font-bold">Dashboard</h1>
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        {metrics.map((m) => (
          <Card key={m.title}>
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">
                {m.title}
              </CardTitle>
              <m.icon className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{m.value}</div>
              <p className="text-xs text-muted-foreground">{m.change}</p>
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  )
}

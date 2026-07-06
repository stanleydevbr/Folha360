import { Card, CardContent, CardHeader, CardTitle, Button, FormInput } from '@folha360/ui'
import { useAuth } from '../../providers/AuthProvider'
import { useState } from 'react'

export default function PerfilPage() {
  const { user } = useAuth()
  const [nome, setNome] = useState(user?.nome ?? '')
  const [email, setEmail] = useState(user?.email ?? '')

  const initials = (user?.nome ?? 'U')
    .split(' ')
    .map((n) => n[0])
    .slice(0, 2)
    .join('')
    .toUpperCase()

  return (
    <div className="max-w-2xl">
      <h1 className="mb-6 text-2xl font-bold">Meu Perfil</h1>

      <Card>
        <CardHeader>
          <div className="flex items-center gap-4">
            <div className="flex h-20 w-20 items-center justify-center rounded-full bg-primary text-2xl font-bold text-primary-foreground">
              {initials}
            </div>
            <div>
              <CardTitle>{user?.nome}</CardTitle>
              <p className="text-sm text-muted-foreground">{user?.email}</p>
              <div className="mt-2 flex gap-1">
                {user?.roles?.map((role) => (
                  <span
                    key={role}
                    className="rounded-full bg-primary/10 px-2 py-0.5 text-xs font-medium text-primary"
                  >
                    {role}
                  </span>
                ))}
              </div>
            </div>
          </div>
        </CardHeader>
        <CardContent className="flex flex-col gap-4">
          <FormInput
            label="Nome"
            id="nome"
            value={nome}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) => setNome(e.target.value)}
          />
          <FormInput
            label="Email"
            id="email"
            type="email"
            value={email}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) => setEmail(e.target.value)}
            disabled
          />
          <div className="flex gap-2 pt-2">
            <Button>Salvar Alterações</Button>
            <Button variant="outline">Alterar Senha</Button>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}

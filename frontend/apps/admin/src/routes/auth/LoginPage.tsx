import { useState } from 'react'
import { useNavigate } from 'react-router'
import { Button, Input } from '@folha360/ui'
import { useAuth } from '../../providers/AuthProvider'
import { useLogin } from '@folha360/api'

export function LoginPage() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const { login: authLogin, apiClient } = useAuth()
  const navigate = useNavigate()
  const loginMutation = useLogin(apiClient)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError('')

    if (!email || !password) {
      setError('Preencha todos os campos.')
      return
    }

    try {
      const result = await loginMutation.mutateAsync({ email, password })
      authLogin(result.accessToken, result.user, result.tenants)
      navigate('/dashboard', { replace: true })
    } catch (err: any) {
      const message = err?.response?.data?.detail || 'Erro ao fazer login. Tente novamente.'
      setError(message)
    }
  }

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-4">
      <div>
        <label htmlFor="email" className="text-sm font-medium">
          Email
        </label>
        <Input
          id="email"
          type="email"
          value={email}
          onChange={(e: React.ChangeEvent<HTMLInputElement>) => setEmail(e.target.value)}
          placeholder="seu@email.com"
          className="mt-1"
        />
      </div>
      <div>
        <label htmlFor="password" className="text-sm font-medium">
          Senha
        </label>
        <Input
          id="password"
          type="password"
          value={password}
          onChange={(e: React.ChangeEvent<HTMLInputElement>) => setPassword(e.target.value)}
          placeholder="Sua senha"
          className="mt-1"
        />
      </div>

      {error && (
        <p className="text-sm text-destructive">{error}</p>
      )}

      <Button type="submit" disabled={loginMutation.isPending} className="w-full">
        {loginMutation.isPending ? 'Entrando...' : 'Entrar'}
      </Button>
    </form>
  )
}

import {useState} from 'react';
import {useAuth} from '../../providers/AuthProvider';

export function LoginPage() {const {login} = useAuth();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) =>{e.preventDefault();
        setError('');

        if (!email.trim()) {setError('Informe o e-mail.'); return; }
        if (!password) {setError('Informe a senha.'); return; }

        setLoading(true);
        const user = await login({email, password});
        setLoading(false);

        if (!user) {setError('E-mail ou senha inválidos.');
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-canvas-soft px-4">
        <div className="w-full max-w-sm">
            <div className="text-center mb-8">
                <h1 className="text-2xl font-medium text-ink">Folha360</h1>
                    <p className="text-sm text-ink-mute mt-1">Sistema de Gestão de Folha de Pagamento</p>
                        </div>

                        <form onSubmit={handleSubmit} className="bg-canvas border border-hairline-cool rounded-xl p-6 shadow-sm space-y-4">
                            <h2 className="text-lg font-medium text-ink">Entrar</h2>

                                <div >
                                <label htmlFor="email" className="block text-sm font-medium text-ink mb-1">E-mail</label>
                                    <input
    id="email"
    type="email"
    value={email}
    onChange={(e) =>setEmail(e.target.value)
}
placeholder="seu@email.com"
className="w-full h-10 px-3 rounded-md border border-hairline-strong bg-canvas text-ink text-sm placeholder:text-ink-mute-2 focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors"
autoFocus/>
    </div>

    <div >
    <label htmlFor="password" className="block text-sm font-medium text-ink mb-1">Senha</label>
        <input
id="password"
type="password"
value={password}
onChange={(e) =>setPassword(e.target.value)}
placeholder="Sua senha"
className="w-full h-10 px-3 rounded-md border border-hairline-strong bg-canvas text-ink text-sm placeholder:text-ink-mute-2 focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-colors"/>
    </div>{error && (
    <p className="text-sm text-accent-tomato" role="alert">{error}</p>)}

<button
                        type="submit"
disabled={loading}
className="w-full h-10 flex items-center justify-center rounded-md bg-primary text-on-primary text-sm font-medium hover:bg-primary-deep transition-colors disabled:opacity-50 disabled:cursor-not-allowed cursor-pointer">{loading?(
                            <span className="inline-block h-4 w-4 border-2 border-on-primary border-t-transparent rounded-full animate-spin"/>): (
                'Entrar'
            )
}</button>
    </form>
    </div>
    </div>);
}

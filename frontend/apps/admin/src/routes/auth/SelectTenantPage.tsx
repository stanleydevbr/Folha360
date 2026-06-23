import {useLocation, useNavigate} from 'react-router-dom';
import {useAuth} from '../../providers/AuthProvider';
import type {TenantDto} from '@folha360/api';

interface LocationState {tenants?: TenantDto[]; }

export function SelectTenantPage() {const {switchTenant} = useAuth();
    const navigate = useNavigate();
    const location = useLocation();
    const state = location.state as LocationState | null;
    const tenants = state?.tenants ?? [];

    if (tenants.length === 0) {navigate('/login');
        return null;
    }

    return (
        <div className="min-h-screen flex items-center justify-center bg-canvas-soft px-4">
        <div className="w-full max-w-sm">
            <div className="text-center mb-8">
                <h1 className="text-2xl font-medium text-ink">Folha360</h1>
                    <p className="text-sm text-ink-mute mt-1">Selecione a empresa</p>
                        </div>

                        <div className="space-y-2">{tenants.map((tenant) =>(
                                <button
                            key= {tenant.id}
                            onClick={() =>switchTenant(tenant)}
    className="w-full flex items-center gap-3 p-4 bg-canvas border border-hairline-cool rounded-lg hover:border-primary hover:shadow-sm transition-all text-left cursor-pointer"
        >
        <div className="w-10 h-10 rounded-full bg-primary/10 text-primary flex items-center justify-center text-sm font-medium shrink-0">{tenant.nome.charAt(0).toUpperCase() }</div>
            <div >
            <p className="text-sm font-medium text-ink">{tenant.nome}</p>
                <p className="text-xs text-ink-mute">{tenant.slug}</p>
                    </div>
                    </button>))}</div >
                    </div>
                    </div>);
}

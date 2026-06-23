import {createContext, useContext, useState, useCallback, useEffect, type ReactNode} from 'react';
import {useNavigate} from 'react-router-dom';
import {useLogin, useLogout, restoreSession, configureAuth, setActiveTenantId, getActiveTenantId, getAccessToken} from '@folha360/api';
import type {LoginCommand, UserDto, TenantDto} from '@folha360/api';

interface AuthContextValue {user: UserDto | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    activeTenant: TenantDto | null;
    availableTenants: TenantDto[];
    login: (credentials: LoginCommand) =>Promise<UserDto | null>;
    logout: () =>void;
    switchTenant: (tenant: TenantDto) =>void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function useAuth() {const ctx = useContext(AuthContext);
    if (!ctx) throw new Error('useAuth must be within AuthProvider');
    return ctx;
}

export function AuthProvider({children}: {children: ReactNode}) {const [user, setUser] = useState<UserDto | null>(null);
    const [availableTenants, setAvailableTenants] = useState<TenantDto[]>([]);
    const [activeTenant, setActiveTenant] = useState<TenantDto | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const navigate = useNavigate();
    const loginMutation = useLogin();
    const performLogout = useLogout();

    // Configure auth callbacks for apiClient
    useEffect(() =>{configureAuth(() =>{performLogout();
            setUser(null);
            setActiveTenant(null);
            navigate('/login');
        });
    }, [performLogout, navigate]);

    // Restore session on mount
    useEffect(() =>{const token = getAccessToken();
        if (!token) {setIsLoading(false);
            return;
        }
        restoreSession()
            .then((data) =>{setUser(data.user);
                setAvailableTenants(data.tenants);
                const lastId = getActiveTenantId();
                const tenant = data.tenants.find((t: TenantDto) =>t.id === lastId) || data.tenants[0];
                if (tenant) {setActiveTenant(tenant);
                    setActiveTenantId(tenant.id);
                }
            })
            .catch(() =>{performLogout();
            })
            .finally(() =>setIsLoading(false));
    }, [performLogout]);

    const login = useCallback(async (credentials: LoginCommand): Promise<UserDto | null>=>{try {const result = await loginMutation.mutateAsync(credentials);
            setUser(result.user);
            setAvailableTenants(result.tenants);
            if (result.tenants.length === 1) {const tenant = result.tenants[0]!;
                setActiveTenant(tenant);
                setActiveTenantId(tenant.id);
                navigate('/dashboard');
            } else {navigate('/select-tenant', {state: {tenants: result.tenants} });
            }
            return result.user;
        } catch {return null;
        }
    }, [loginMutation, navigate]);

    const logout = useCallback(() =>{performLogout();
        setUser(null);
        setActiveTenant(null);
        setAvailableTenants([]);
        navigate('/login');
    }, [performLogout, navigate]);

    const switchTenant = useCallback((tenant: TenantDto) =>{setActiveTenant(tenant);
        setActiveTenantId(tenant.id);
        navigate('/dashboard');
    }, [navigate]);

    return (
        <AuthContext.Provider value={{user, isAuthenticated: !!user, isLoading, activeTenant, availableTenants, login, logout, switchTenant}
}>{children}</AuthContext.Provider>);
}

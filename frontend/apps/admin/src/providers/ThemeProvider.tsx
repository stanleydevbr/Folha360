import {createContext, useContext, useState, useEffect, useCallback, type ReactNode} from 'react';

type Theme = 'light' | 'dark' | 'system';

interface ThemeContextValue {theme: Theme;
    resolvedTheme: 'light' | 'dark';
    setTheme: (theme: Theme) =>void;
}

const ThemeContext = createContext<ThemeContextValue | null>(null);

export function useTheme() {const ctx = useContext(ThemeContext);
    if (!ctx) throw new Error('useTheme must be within ThemeProvider');
    return ctx;
}

function getStoredTheme(): Theme {try {const t = localStorage.getItem('folha360-theme'); if (t === 'light' || t === 'dark' || t === 'system') return t; } catch {/** ignore */ }
    return 'system';
}

function getSystemTheme(): 'light' | 'dark' {if (typeof window !== 'undefined' && window.matchMedia('(prefers-color-scheme: dark)').matches) return 'dark';
    return 'light';
}

function applyTheme(resolved: 'light' | 'dark') {if (resolved === 'dark') document.documentElement.classList.add('dark');
    else document.documentElement.classList.remove('dark');
}

export function ThemeProvider({children}: {children: ReactNode}) {const [theme, setThemeState] = useState<Theme>(getStoredTheme);
    const [resolvedTheme, setResolved] = useState<'light' | 'dark'>(() =>{const t = getStoredTheme();
        return t === 'system' ? getSystemTheme() : t;
    });

    const setTheme = useCallback((newTheme: Theme) =>{setThemeState(newTheme);
        try {localStorage.setItem('folha360-theme', newTheme); } catch {/** ignore */ }
        const resolved = newTheme === 'system' ? getSystemTheme() : newTheme;
        setResolved(resolved);
        applyTheme(resolved);
    }, []);

    // Listen to system theme changes
    useEffect(() =>{if (theme !== 'system') return;
        const mq = window.matchMedia('(prefers-color-scheme: dark)');
        const handler = () =>{const resolved = mq.matches ? 'dark' : 'light';
            setResolved(resolved);
            applyTheme(resolved);
        };
        mq.addEventListener('change', handler);
        return () =>mq.removeEventListener('change', handler);
    }, [theme]);

    // Initial apply
    useEffect(() =>{applyTheme(resolvedTheme); }, [resolvedTheme]);

    return <ThemeContext.Provider value={ {theme, resolvedTheme, setTheme} }>{children}</ThemeContext.Provider>;
}

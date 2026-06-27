import { createContext, useContext, useState, useEffect, useCallback, type ReactNode } from 'react'
import { presets, fontFamilies, type FontFamily } from '@folha360/ui'

export interface ThemeState {
  preset: string
  mode: 'light' | 'dark'
  fontFamily: FontFamily
  layout: 'vertical' | 'horizontal'
  sidebarCollapsed: boolean
  direction: 'ltr' | 'rtl'
  container: 'boxed' | 'full'
}

interface ThemeContextValue {
  theme: ThemeState
  setPreset: (preset: string) => void
  toggleMode: () => void
  setFontFamily: (font: FontFamily) => void
  setLayout: (layout: 'vertical' | 'horizontal') => void
  toggleSidebar: () => void
  setDirection: (dir: 'ltr' | 'rtl') => void
  setContainer: (container: 'boxed' | 'full') => void
}

const defaultTheme: ThemeState = {
  preset: 'preset-1',
  mode: 'light',
  fontFamily: 'roboto',
  layout: 'vertical',
  sidebarCollapsed: false,
  direction: 'ltr',
  container: 'boxed',
}

const ThemeContext = createContext<ThemeContextValue | null>(null)

function loadTheme(): ThemeState {
  try {
    const stored = localStorage.getItem('folha360-theme')
    if (stored) return { ...defaultTheme, ...JSON.parse(stored) }
  } catch { /* ignore */ }
  return defaultTheme
}

function applyTheme(theme: ThemeState) {
  const root = document.documentElement
  const preset = presets[theme.preset] ?? presets['preset-1']
  const isDark = theme.mode === 'dark'

  root.style.setProperty('--color-primary', preset.primary)
  root.style.setProperty('--color-primary-light', preset.primaryLight)
  root.style.setProperty('--color-primary-dark', preset.primaryDark)
  root.style.setProperty('--color-secondary', preset.secondary)
  root.style.setProperty('--color-secondary-light', preset.secondaryLight)
  root.style.setProperty('--color-secondary-dark', preset.secondaryDark)

  // Sidebar: dark colors in dark mode, light colors in light mode
  root.style.setProperty('--color-sidebar-bg', isDark ? preset.sidebarBg : '#f8f9fa')
  root.style.setProperty('--color-sidebar-text', isDark ? preset.sidebarText : '#4a5568')
  root.style.setProperty('--color-sidebar-active', preset.sidebarActive)

  // Header: use preset dark header in dark mode, white in light mode
  root.style.setProperty('--color-header-bg', isDark ? preset.headerBg : '#ffffff')
  root.style.setProperty('--color-header-text', isDark ? preset.headerText : '#1a202c')

  root.style.setProperty('--font-family', fontFamilies[theme.fontFamily])

  root.classList.toggle('dark', isDark)
  root.dir = theme.direction
}

export function ThemeProvider({ children }: { children: ReactNode }) {
  const [theme, setTheme] = useState<ThemeState>(loadTheme)

  useEffect(() => {
    applyTheme(theme)
    localStorage.setItem('folha360-theme', JSON.stringify(theme))
  }, [theme])

  const setPreset = useCallback((preset: string) => setTheme((t) => ({ ...t, preset })), [])
  const toggleMode = useCallback(() => setTheme((t) => ({ ...t, mode: t.mode === 'light' ? 'dark' : 'light' })), [])
  const setFontFamily = useCallback((font: FontFamily) => setTheme((t) => ({ ...t, fontFamily: font })), [])
  const setLayout = useCallback((layout: 'vertical' | 'horizontal') => setTheme((t) => ({ ...t, layout })), [])
  const toggleSidebar = useCallback(() => setTheme((t) => ({ ...t, sidebarCollapsed: !t.sidebarCollapsed })), [])
  const setDirection = useCallback((dir: 'ltr' | 'rtl') => setTheme((t) => ({ ...t, direction: dir })), [])
  const setContainer = useCallback((container: 'boxed' | 'full') => setTheme((t) => ({ ...t, container })), [])

  return (
    <ThemeContext.Provider value={{ theme, setPreset, toggleMode, setFontFamily, setLayout, toggleSidebar, setDirection, setContainer }}>
      {children}
    </ThemeContext.Provider>
  )
}

export function useTheme() {
  const ctx = useContext(ThemeContext)
  if (!ctx) throw new Error('useTheme must be used within ThemeProvider')
  return ctx
}

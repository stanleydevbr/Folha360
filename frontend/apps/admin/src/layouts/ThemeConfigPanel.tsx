import { useState } from 'react'
import { Settings, Sun, Moon } from 'lucide-react'
import {
  Button,
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from '@folha360/ui'
import { useTheme } from '../providers/ThemeProvider'
import { presets, type FontFamily, type ColorPreset } from '@folha360/ui'

export function ThemeConfigPanel({ collapsed }: { collapsed: boolean }) {
  const [open, setOpen] = useState(false)
  const { theme, setPreset, toggleMode, setFontFamily, setLayout, toggleSidebar, setDirection, setContainer } = useTheme()

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <SheetTrigger
        className="inline-flex w-full items-center gap-2 rounded-md px-3 py-2 text-sm transition-colors hover:bg-muted/50"
        style={{ color: 'var(--color-sidebar-text)' }}
      >
        <Settings className="h-4 w-4" />
        {!collapsed && 'Customizar'}
      </SheetTrigger>
      <SheetContent side="right" className="w-80">
        <SheetHeader>
          <SheetTitle>Customizar Tema</SheetTitle>
        </SheetHeader>
        <div className="mt-6 flex flex-col gap-6">
          {/* Presets */}
          <div>
            <label className="text-sm font-medium">Cores</label>
            <div className="mt-2 flex flex-wrap gap-2">
              {Object.entries(presets).map(([key, preset]: [string, ColorPreset]) => (
                <button
                  key={key}
                  onClick={() => setPreset(key)}
                  className="flex h-8 w-8 items-center justify-center rounded-full border-2 transition-all"
                  style={{
                    backgroundColor: preset.primary,
                    borderColor: theme.preset === key ? preset.secondary : 'transparent',
                  }}
                  title={preset.name}
                />
              ))}
            </div>
          </div>

          {/* Dark/Light */}
          <div>
            <label className="text-sm font-medium">Modo</label>
            <Button
              variant="outline"
              size="sm"
              onClick={toggleMode}
              className="mt-2 w-full gap-2"
            >
              {theme.mode === 'dark' ? <Sun className="h-4 w-4" /> : <Moon className="h-4 w-4" />}
              {theme.mode === 'dark' ? 'Claro' : 'Escuro'}
            </Button>
          </div>

          {/* Font */}
          <div>
            <label className="text-sm font-medium">Fonte</label>
            <select
              value={theme.fontFamily}
              onChange={(e) => setFontFamily(e.target.value as FontFamily)}
              className="mt-2 w-full rounded-md border px-3 py-2 text-sm"
            >
              <option value="roboto">Roboto</option>
              <option value="inter">Inter</option>
              <option value="poppins">Poppins</option>
            </select>
          </div>

          {/* Layout */}
          <div>
            <label className="text-sm font-medium">Layout</label>
            <select
              value={theme.layout}
              onChange={(e) => setLayout(e.target.value as 'vertical' | 'horizontal')}
              className="mt-2 w-full rounded-md border px-3 py-2 text-sm"
            >
              <option value="vertical">Vertical</option>
              <option value="horizontal">Horizontal</option>
            </select>
          </div>

          {/* Sidebar */}
          <div>
            <label className="text-sm font-medium">Sidebar</label>
            <Button
              variant="outline"
              size="sm"
              onClick={toggleSidebar}
              className="mt-2 w-full"
            >
              {theme.sidebarCollapsed ? 'Expandir' : 'Colapsar'}
            </Button>
          </div>

          {/* RTL */}
          <div>
            <label className="text-sm font-medium">Direção</label>
            <Button
              variant="outline"
              size="sm"
              onClick={() => setDirection(theme.direction === 'ltr' ? 'rtl' : 'ltr')}
              className="mt-2 w-full"
            >
              {theme.direction === 'ltr' ? 'RTL' : 'LTR'}
            </Button>
          </div>

          {/* Container */}
          <div>
            <label className="text-sm font-medium">Container</label>
            <Button
              variant="outline"
              size="sm"
              onClick={() => setContainer(theme.container === 'boxed' ? 'full' : 'boxed')}
              className="mt-2 w-full"
            >
              {theme.container === 'boxed' ? 'Full-width' : 'Boxed'}
            </Button>
          </div>
        </div>
      </SheetContent>
    </Sheet>
  )
}

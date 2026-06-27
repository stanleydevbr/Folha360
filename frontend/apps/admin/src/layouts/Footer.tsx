export function Footer() {
  return (
    <footer className="flex h-12 shrink-0 items-center justify-between border-t px-6 text-xs text-muted-foreground">
      <span>&copy; {new Date().getFullYear()} Folha360. Todos os direitos reservados.</span>
      <span>v1.0.0</span>
    </footer>
  )
}

import { useMemo } from 'react'
import { useLocation } from 'react-router'
import { deriveBreadcrumbs, type NavItem, type PageBreadcrumbItem } from '@folha360/ui'

export function useBreadcrumbs(navItems: NavItem[]): PageBreadcrumbItem[] {
  const location = useLocation()
  return useMemo(
    () => deriveBreadcrumbs(navItems, location.pathname),
    [navItems, location.pathname],
  )
}

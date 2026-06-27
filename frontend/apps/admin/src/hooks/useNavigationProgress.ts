import { useEffect, useRef, useState } from 'react'
import { useLocation } from 'react-router'

export function useNavigationProgress() {
  const location = useLocation()
  const barRef = useRef<HTMLDivElement>(null)
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    setLoading(true)
    const bar = barRef.current
    if (bar) {
      bar.style.width = '0%'
      bar.style.opacity = '1'
      requestAnimationFrame(() => {
        bar.style.width = '70%'
      })
    }

    // Simulate completion after a short delay (navigation is instant with client-side routing)
    const timer = setTimeout(() => {
      if (bar) {
        bar.style.width = '100%'
        setTimeout(() => {
          bar.style.opacity = '0'
          bar.style.width = '0%'
          setLoading(false)
        }, 200)
      }
    }, 300)

    return () => clearTimeout(timer)
  }, [location.pathname])

  return barRef
}

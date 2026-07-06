import { useNavigationProgress } from '../hooks/useNavigationProgress'

export function NavigationProgress() {
  const barRef = useNavigationProgress()

  return (
    <div
      ref={barRef}
      className="fixed top-0 left-0 z-50 h-0.5 bg-primary transition-all duration-300"
      style={{ width: '0%', opacity: 0 }}
    />
  )
}

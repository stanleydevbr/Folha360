import {Navigate} from 'react-router-dom';
import {useAuth} from '../providers/AuthProvider';

export function PrivateRoute({children}: {children: React.ReactNode}) {const {isAuthenticated, isLoading} = useAuth();

    if (isLoading) {return (
            <div className="min-h-screen flex items-center justify-center bg-canvas">
            <div className="h-8 w-8 border-2 border-primary border-t-transparent rounded-full animate-spin"/>
                </div>);
    }

    if (!isAuthenticated) return <Navigate to="/login" replace/>;
    return <>{children}</>;
}

export function PublicRoute({children}: {children: React.ReactNode}) {const {isAuthenticated, isLoading} = useAuth();

    if (isLoading) {return (
            <div className="min-h-screen flex items-center justify-center bg-canvas">
            <div className="h-8 w-8 border-2 border-primary border-t-transparent rounded-full animate-spin"/>
                </div>);
    }

    if (isAuthenticated) return <Navigate to="/dashboard" replace/>;
    return <>{children}</>;
}

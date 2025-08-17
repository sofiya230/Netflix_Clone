
import { Navigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';

function RequireProfile({ children }) {
    const { currentProfile, loading } = useAuth();

    if (loading) return null;

    if (!currentProfile) {
        return <Navigate to="/select-profile" replace />;
    }

    return children;
}

export default RequireProfile;

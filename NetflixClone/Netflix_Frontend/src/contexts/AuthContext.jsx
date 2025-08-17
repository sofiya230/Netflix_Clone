import { createContext, useContext, useEffect, useMemo, useState } from 'react';

const AuthContext = createContext(null);

export function useAuth() {
    return useContext(AuthContext);
}

export function AuthProvider({ children }) {
    const [authToken, setAuthToken] = useState(null);
    const [currentUser, setCurrentUser] = useState(null);
    const [currentRole, setCurrentRole] = useState(null);
    const [currentProfile, setCurrentProfile] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        try {
            const token = localStorage.getItem('auth_token');
            const userRaw = localStorage.getItem('user_data');
            const role = localStorage.getItem('user_role');
            const profileRaw = localStorage.getItem('current_profile');

            if (token) setAuthToken(token);
            if (userRaw) {
                const parsedUser = JSON.parse(userRaw);
                setCurrentUser(parsedUser);
            }
            if (role) setCurrentRole(role);
            if (profileRaw) setCurrentProfile(JSON.parse(profileRaw));
        } catch (e) {
        } finally {
            setLoading(false);
        }
    }, []);

    const isAuthenticated = useMemo(() => !!authToken, [authToken]);

    const isProfileSelected = useMemo(() => !!currentProfile, [currentProfile]);

    const login = (userData, token, roleFromBody) => {
        localStorage.setItem('auth_token', token);
        localStorage.setItem('user_data', JSON.stringify(userData));

        const role = roleFromBody ?? userData?.role ?? null;
        if (role) localStorage.setItem('user_role', role);

        setAuthToken(token);
        setCurrentUser(userData);
        setCurrentRole(role);
    };

    const logout = () => {
        localStorage.removeItem('auth_token');
        localStorage.removeItem('user_data');
        localStorage.removeItem('user_role');
        localStorage.removeItem('current_profile');

        setAuthToken(null);
        setCurrentUser(null);
        setCurrentRole(null);
        setCurrentProfile(null);
    };

    const selectProfile = (profile) => {
        setCurrentProfile(profile);
        localStorage.setItem('current_profile', JSON.stringify(profile));
    };

    const value = {
        authToken,
        currentUser,
        currentRole,
        currentProfile,
        isAuthenticated,
        isProfileSelected,
        loading,
        login,
        logout,
        selectProfile,
    };

    return (
        <AuthContext.Provider value={value}>
            {!loading && children}
        </AuthContext.Provider>
    );
}

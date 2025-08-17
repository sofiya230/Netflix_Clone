import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';

import Browse from './pages/Browse';
import ContentDetail from './pages/ContentDetail';
import Login from './components/auth/Login';
import ProfileSelection from './components/auth/ProfileSelection';
import WatchPage from './pages/WatchPage';
import EpisodesPage from './pages/EpisodesPage';
import MyListPage from './pages/MyListPage';
import SeriesPage from './pages/SeriesPage';
import SeriesListPage from './pages/SeriesListPage';
import MoviesListPage from './pages/MoviesListPage';
import NewReleasesPage from './pages/NewReleasesPage';
import WatchHistory from './pages/WatchHistory';
import AddProfile from './pages/AddProfile';
import WatchAgainPage from './pages/WatchAgainPage';
import AdminContents from './pages/AdminContents';
import AdminEpisodes from './pages/AdminEpisodes';
import UserSettings from './pages/UserSettings';
import EditProfile from './pages/EditProfile';
import HelpCenter from './pages/HelpCenter';
import ForgotPassword from './components/auth/ForgotPassword';
import ResetPassword from './components/auth/ResetPassword';
import TwoFactorSettings from './components/auth/TwoFactorSettings';

const ProtectedRoute = ({ children, requireProfile = true }) => {
    const { isAuthenticated, isProfileSelected, currentProfile, loading } = useAuth();

    if (loading) return <div className="text-white p-10">Loading...</div>;

    const authed =
        typeof isAuthenticated === 'function' ? isAuthenticated() : !!isAuthenticated;

    const hasProfile =
        typeof isProfileSelected === 'function'
            ? isProfileSelected()
            : !!currentProfile;

    if (!authed) return <Navigate to="/login" replace />;
    if (requireProfile && !hasProfile) return <Navigate to="/profiles" replace />;

    return children;
};

function AppRoutes() {
    return (
        <Routes>
            <Route path="/login" element={<Login />} />
            <Route path="/profiles" element={<ProfileSelection />} />
            <Route path="/profiles/add" element={<AddProfile />} />
            <Route path="/profiles/edit/:id" element={<EditProfile />} />
            <Route path="/forgot-password" element={<ForgotPassword />} />
            <Route path="/reset-password" element={<ResetPassword />} />
            <Route path="/two-factor-settings" element={<TwoFactorSettings />} />

            <Route path="/admin/contents" element={<AdminContents />} />
            <Route path="/admin/episodes/:contentId" element={<AdminEpisodes />} />

            <Route path="/" element={<Navigate to="/home" replace />} />

            <Route
                path="/browse"
                element={
                    <ProtectedRoute>
                        <Browse />
                    </ProtectedRoute>
                }
            />
            <Route
                path="/home"
                element={
                    <ProtectedRoute>
                        <Browse />
                    </ProtectedRoute>
                }
            />

            <Route
                path="/content/:id"
                element={
                    <ProtectedRoute>
                        <ContentDetail />
                    </ProtectedRoute>
                }
            />
            <Route
                path="/watch/:id"
                element={
                    <ProtectedRoute>
                        <WatchPage />
                    </ProtectedRoute>
                }
            />
            <Route
                path="/episodes/:seriesId"
                element={
                    <ProtectedRoute>
                        <EpisodesPage />
                    </ProtectedRoute>
                }
            />

            <Route
                path="/my-list"
                element={
                    <ProtectedRoute>
                        <MyListPage />
                    </ProtectedRoute>
                }
            />
            <Route
                path="/watch-again"
                element={
                    <ProtectedRoute>
                        <WatchAgainPage />
                    </ProtectedRoute>
                }
            />
            <Route
                path="/history"
                element={
                    <ProtectedRoute>
                        <WatchHistory />
                    </ProtectedRoute>
                }
            />

            <Route
                path="/series"
                element={
                    <ProtectedRoute>
                        <SeriesListPage />
                    </ProtectedRoute>
                }
            />
            <Route
                path="/series/:id"
                element={
                    <ProtectedRoute>
                        <SeriesPage />
                    </ProtectedRoute>
                }
            />

            <Route
                path="/movies"
                element={
                    <ProtectedRoute>
                        <MoviesListPage />
                    </ProtectedRoute>
                }
            />

            <Route
                path="/new"
                element={
                    <ProtectedRoute>
                        <NewReleasesPage />
                    </ProtectedRoute>
                }
            />

            <Route
                path="/settings"
                element={
                    <ProtectedRoute requireProfile={false}>
                        <UserSettings />
                    </ProtectedRoute>
                }
            />

            <Route
                path="/help"
                element={
                    <ProtectedRoute requireProfile={false}>
                        <HelpCenter />
                    </ProtectedRoute>
                }
            />
        </Routes>
    );
}

function App() {
    return (
        <Router>
            <AuthProvider>
                <AppRoutes />
            </AuthProvider>
        </Router>
    );
}

export default App;

import { useState, useEffect, useRef } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import SearchBar from './SearchBar';

function Navbar({ transparent = false }) {
    const [isScrolled, setIsScrolled] = useState(false);
    const [showMobileMenu, setShowMobileMenu] = useState(false);
    const [showProfileMenu, setShowProfileMenu] = useState(false);
    const [showAdminMenu, setShowAdminMenu] = useState(false);

    const { currentUser, currentProfile, logout, currentRole } = useAuth();
    const navigate = useNavigate();
    const location = useLocation();
    const adminRef = useRef(null);

    useEffect(() => {
        const handleScroll = () => setIsScrolled(window.scrollY > 0);
        window.addEventListener('scroll', handleScroll);
        return () => window.removeEventListener('scroll', handleScroll);
    }, []);

    
    useEffect(() => {
        function onDocClick(e) {
            if (adminRef.current && !adminRef.current.contains(e.target)) {
                setShowAdminMenu(false);
            }
        }
        function onEsc(e) {
            if (e.key === 'Escape') setShowAdminMenu(false);
        }
        document.addEventListener('mousedown', onDocClick);
        document.addEventListener('keydown', onEsc);
        return () => {
            document.removeEventListener('mousedown', onDocClick);
            document.removeEventListener('keydown', onEsc);
        };
    }, []);

    const handleProfileSelect = () => {
        setShowProfileMenu(false);
        navigate('/profiles');
    };

    const handleLogout = () => {
        setShowProfileMenu(false);
        logout();
    };

    
    const isActive = (path) => location.pathname === path;
    const startsWith = (prefix) => location.pathname.startsWith(prefix);

    useEffect(() => {

    }, [currentRole]);

    return (
        <nav
            className={`fixed top-0 z-40 w-full transition duration-500 ${isScrolled || !transparent
                    ? 'bg-black bg-opacity-90'
                    : 'bg-gradient-to-b from-black/70 to-transparent'
                }`}
        >
            <div className="px-4 md:px-16 py-5 flex items-center justify-between">
                {/* Left: Logo + Links */}
                <div className="flex items-center gap-8">
                    <Link to="/home" className="text-red-600 text-2xl font-bold">
                        NetflixClone
                    </Link>

                    {/* Desktop Navigation */}
                    <div className="hidden md:flex gap-6">
                        <Link
                            to="/home"
                            className={`text-sm font-medium ${isActive('/home') ? 'text-white' : 'text-gray-300 hover:text-white transition'
                                }`}
                        >
                            Home
                        </Link>

                        <Link
                            to="/watch-again"
                            className={`text-sm font-medium ${isActive('/watch-again') ? 'text-white' : 'text-gray-300 hover:text-white transition'
                                }`}
                        >
                            Watch Again
                        </Link>

                        {/* Series list (active for /series and /series/:id) */}
                        <Link
                            to="/series"
                            className={`text-sm font-medium ${startsWith('/series') ? 'text-white' : 'text-gray-300 hover:text-white transition'
                                }`}
                        >
                            Series
                        </Link>

                        {/* Movies list */}
                        <Link
                            to="/movies"
                            className={`text-sm font-medium ${startsWith('/movies') ? 'text-white' : 'text-gray-300 hover:text-white transition'
                                }`}
                        >
                            Movies
                        </Link>

                        <Link
                            to="/new"
                            className={`text-sm font-medium ${isActive('/new') ? 'text-white' : 'text-gray-300 hover:text-white transition'
                                }`}
                        >
                            New & Popular
                        </Link>

                        <Link
                            to="/my-list"
                            className={`text-sm font-medium ${isActive('/my-list') ? 'text-white' : 'text-gray-300 hover:text-white transition'
                                }`}
                        >
                            My List
                        </Link>

                        {/* --- ADMIN DROPDOWN (click-to-toggle, no hover gap) --- */}
                        {currentRole === 'Admin' && (
                            <div ref={adminRef} className="relative">
                                <button
                                    type="button"
                                    onClick={() => setShowAdminMenu(v => !v)}
                                    className={`text-sm font-medium ${showAdminMenu ? 'text-white' : 'text-gray-300 hover:text-white transition'} px-2`}
                                    aria-expanded={showAdminMenu ? 'true' : 'false'}
                                    aria-haspopup="menu"
                                >
                                    Admin Panel ▾
                                </button>


                                {showAdminMenu && (
                                    <div
                                        role="menu"
                                        className="absolute left-0 mt-2 bg-gray-900 border border-gray-800 rounded shadow-lg min-w-[190px] z-50"
                                    >
                                        <button
                                            role="menuitem"
                                            className="w-full text-left block px-4 py-2 text-gray-200 hover:bg-gray-800"
                                            onClick={() => {
                                                setShowAdminMenu(false);
                                                navigate('/admin/contents');
                                            }}
                                        >
                                            Manage Content
                                        </button>
                                    </div>
                                )}
                            </div>
                        )}
                    </div>

                    {/* Mobile Menu Toggle */}
                    <div className="md:hidden">
                        <button
                            onClick={() => setShowMobileMenu((s) => !s)}
                            className="text-white hover:text-gray-300 transition"
                        >
                            <span className="sr-only">Menu</span>
                            {showMobileMenu ? (
                                <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    className="h-6 w-6"
                                    viewBox="0 0 24 24"
                                    stroke="currentColor"
                                    fill="none"
                                >
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                                </svg>
                            ) : (
                                <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    className="h-6 w-6"
                                    viewBox="0 0 24 24"
                                    stroke="currentColor"
                                    fill="none"
                                >
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
                                </svg>
                            )}
                        </button>
                    </div>
                </div>

                {/* Right: Search + Profile */}
                <div className="flex items-center gap-4">
                    <div className="hidden md:block">
                        <SearchBar />
                    </div>

                    {/* Profile Dropdown (NO AVATAR IMAGE) */}
                    <div className="relative">
                        <button
                            onClick={() => setShowProfileMenu((s) => !s)}
                            className="flex items-center gap-2 cursor-pointer"
                        >
                            {/* Just a simple user icon instead of an image */}
                            <svg
                                xmlns="http://www.w3.org/2000/svg"
                                className="h-8 w-8 text-gray-300"
                                viewBox="0 0 24 24"
                                fill="none"
                                stroke="currentColor"
                            >
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
                                    d="M5.121 17.804A7 7 0 0112 15a7 7 0 016.879 2.804M15 10a3 3 0 11-6 0 3 3 0 016 0z" />
                            </svg>

                            <span className="hidden md:inline text-gray-300 text-sm">
                                {currentProfile?.name || 'Profile'}
                            </span>
                            <svg
                                xmlns="http://www.w3.org/2000/svg"
                                className={`h-4 w-4 text-gray-300 transition-transform ${showProfileMenu ? 'rotate-180' : ''}`}
                                viewBox="0 0 24 24"
                                stroke="currentColor"
                                fill="none"
                            >
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                            </svg>
                        </button>

                        {showProfileMenu && (
                            <div className="absolute right-0 mt-2 w-56 bg-black border border-gray-700 rounded shadow-lg py-2 z-50">
                                {(currentUser?.profiles || []).map((profile) => (
                                    <button
                                        key={profile.id}
                                        onClick={() => setShowProfileMenu(false)}
                                        className="flex items-center w-full px-4 py-2 text-sm text-gray-200 hover:bg-gray-800"
                                    >
                                        {/* Name only — no image */}
                                        <span className="truncate">{profile.name}</span>
                                    </button>
                                ))}
                                <div className="border-t border-gray-700 my-1" />
                                <button
                                    onClick={handleProfileSelect}
                                    className="w-full text-left px-4 py-2 text-sm text-gray-200 hover:bg-gray-800"
                                >
                                    Manage Profiles
                                </button>
                                <Link
                                    to="/settings"
                                    className="block px-4 py-2 text-sm text-gray-200 hover:bg-gray-800"
                                    onClick={() => setShowProfileMenu(false)}
                                >
                                    Account Settings
                                </Link>
                                <button
                                    onClick={handleLogout}
                                    className="w-full text-left px-4 py-2 text-sm text-gray-200 hover:bg-gray-800"
                                >
                                    Sign out of Netflix
                                </button>
                            </div>
                        )}
                    </div>
                </div>
            </div>

            {/* Mobile Menu */}
            {showMobileMenu && (
                <div className="bg-black bg-opacity-90 md:hidden py-4 px-4 border-t border-gray-800">
                    <ul className="flex flex-col space-y-4">
                        <li>
                            <Link to="/home" className="text-sm text-gray-200 hover:text-white" onClick={() => setShowMobileMenu(false)}>
                                Home
                            </Link>
                        </li>
                        <li>
                            <Link to="/series" className="text-sm text-gray-200 hover:text-white" onClick={() => setShowMobileMenu(false)}>
                                Series
                            </Link>
                        </li>
                        <li>
                            <Link to="/movies" className="text-sm text-gray-200 hover:text-white" onClick={() => setShowMobileMenu(false)}>
                                Movies
                            </Link>
                        </li>
                        <li>
                            <Link to="/new" className="text-sm text-gray-200 hover:text-white" onClick={() => setShowMobileMenu(false)}>
                                New & Popular
                            </Link>
                        </li>
                        <li>
                            <Link to="/my-list" className="text-sm text-gray-200 hover:text-white" onClick={() => setShowMobileMenu(false)}>
                                My List
                            </Link>
                        </li>

                        {/* Admin (mobile) */}
                        {currentRole === 'Admin' && (
                            <>
                                <li>
                                    <Link
                                        to="/admin/contents"
                                        className="text-sm text-yellow-300 hover:text-yellow-200"
                                        onClick={() => setShowMobileMenu(false)}
                                    >
                                        Admin Content
                                    </Link>
                                </li>
                                <li>
                                    <Link
                                        to="/admin/episodes/1"
                                        className="text-sm text-yellow-300 hover:text-yellow-200"
                                        onClick={() => setShowMobileMenu(false)}
                                    >
                                        Admin Episodes
                                    </Link>
                                </li>
                            </>
                        )}
                    </ul>
                </div>
            )}
        </nav>
    );
}

export default Navbar;

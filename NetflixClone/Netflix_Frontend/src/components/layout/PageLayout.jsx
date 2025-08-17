import { useAuth } from '../../contexts/AuthContext';
import { useLocation } from 'react-router-dom';
import Navbar from './Navbar';
import Footer from './Footer';

function PageLayout({ children, transparentNav = false, showFooter = true }) {
    const { loading } = useAuth();
    const location = useLocation();

    if (loading) {
        return (
            <div className="flex items-center justify-center h-screen text-white">
                <div className="w-12 h-12 border-t-4 border-red-500 border-solid rounded-full animate-spin"></div>
            </div>
        );
    }

    
    
    const mainTopPadding = transparentNav ? 'pt-0' : 'pt-20 md:pt-24';

    return (
        <div className="min-h-screen flex flex-col bg-black">
            <Navbar transparent={transparentNav} />
            <main className={`flex-grow ${mainTopPadding}`}>{children}</main>
            {showFooter && <Footer />}
        </div>
    );
}

export default PageLayout;

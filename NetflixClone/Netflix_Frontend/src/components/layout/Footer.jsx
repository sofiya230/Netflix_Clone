import React from 'react';
import { Link } from 'react-router-dom';

const Footer = () => {
    return (
        <footer className="bg-black text-gray-400 py-8">
            <div className="max-w-7xl mx-auto px-4">
                <div className="text-center">
                    <div className="mb-6">
                        <Link to="/help" className="text-gray-400 hover:text-white text-sm hover:underline">
                            Help Center
                        </Link>
                    </div>
                </div>
            </div>
        </footer>
    );
};

export default Footer;

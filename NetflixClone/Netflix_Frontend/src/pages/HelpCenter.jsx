import React from 'react';
import { Link } from 'react-router-dom';

const HelpCenter = () => {
    return (
        <div className="min-h-screen bg-black text-white px-4 py-10">
            <div className="max-w-4xl mx-auto">
                {/* Header */}
                <div className="text-center mb-12">
                    <h1 className="text-4xl font-bold mb-4">Help Center</h1>
                    <p className="text-gray-400 text-lg">How can we help you with NetflixClone?</p>
                </div>

                {/* Help Categories */}
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-12">
                    <div className="bg-gray-900 p-6 rounded-lg">
                        <h3 className="text-xl font-semibold mb-3">Account & Profiles</h3>
                        <ul className="space-y-2 text-gray-400">
                            <li>• Create and manage user accounts</li>
                            <li>• Set up multiple profiles</li>
                            <li>• Change subscription plans</li>
                            <li>• Reset forgotten passwords</li>
                            <li>• Enable/disable two-factor authentication</li>
                        </ul>
                    </div>

                    <div className="bg-gray-900 p-6 rounded-lg">
                        <h3 className="text-xl font-semibold mb-3">Watching Content</h3>
                        <ul className="space-y-2 text-gray-400">
                            <li>• Browse movies and TV series</li>
                            <li>• Watch episodes with video player</li>
                            <li>• Continue watching from where you left off</li>
                            <li>• View watch history</li>
                            <li>• Search for specific content</li>
                        </ul>
                    </div>

                    <div className="bg-gray-900 p-6 rounded-lg">
                        <h3 className="text-xl font-semibold mb-3">My List & Organization</h3>
                        <ul className="space-y-2 text-gray-400">
                            <li>• Add content to your personal list</li>
                            <li>• Remove items from your list</li>
                            <li>• View all your saved content</li>
                            <li>• Organize by movies or series</li>
                        </ul>
                    </div>

                    <div className="bg-gray-900 p-6 rounded-lg">
                        <h3 className="text-xl font-semibold mb-3">Content Management</h3>
                        <ul className="space-y-2 text-gray-400">
                            <li>• Browse by categories (Movies, Series, New Releases)</li>
                            <li>• Filter content by genre</li>
                            <li>• View content details and descriptions</li>
                            <li>• Access episode lists for series</li>
                        </ul>
                    </div>
                </div>

                {/* Getting Started */}
                <div className="bg-gray-900 p-6 rounded-lg mb-8">
                    <h3 className="text-xl font-semibold mb-4">Getting Started</h3>
                    <div className="text-gray-300 space-y-2">
                        <p>1. <strong>Sign Up:</strong> Create your account with email and password</p>
                        <p>2. <strong>Create Profile:</strong> Set up your viewing profile with name and avatar</p>
                        <p>3. <strong>Browse Content:</strong> Explore movies and TV series</p>
                        <p>4. <strong>Start Watching:</strong> Click on any content to begin streaming</p>
                        <p>5. <strong>Build Your List:</strong> Add favorites to My List for easy access</p>
                    </div>
                </div>

                {/* Back to Home */}
                <div className="text-center">
                    <Link
                        to="/home"
                        className="inline-flex items-center bg-red-600 hover:bg-red-700 px-6 py-3 rounded text-white font-semibold transition-colors"
                    >
                        ← Back to Home
                    </Link>
                </div>
            </div>
        </div>
    );
};

export default HelpCenter;

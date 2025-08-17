import { useState } from 'react';
import { Link } from 'react-router-dom';
import { authService } from '../../services/api';

function ForgotPassword() {
    const [email, setEmail] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');
    const [response, setResponse] = useState(null);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setIsLoading(true);
        setMessage('');
        setError('');
        setResponse(null);

        try {
            const response = await authService.forgotPassword({ email });
            setResponse(response);
            
            if (response.success) {
                setMessage('Password reset code has been sent to your email. Please check your inbox and use the code to reset your password.');
            } else {
                setError(response.message || 'Failed to generate reset code. Please try again.');
            }
        } catch (err) {
            setError(err.message || 'Failed to generate reset code. Please try again.');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-black flex items-center justify-center px-4">
            <div className="max-w-md w-full">
                {/* Logo */}
                <div className="text-center mb-8">
                    <Link to="/" className="text-red-600 text-4xl font-bold">
                        NetflixClone
                    </Link>
                </div>

                {/* Form */}
                <div className="bg-black border border-gray-800 rounded-lg p-8">
                    <h1 className="text-white text-2xl font-bold mb-6">Forgot Password</h1>
                    
                    <p className="text-gray-400 mb-6">
                        Enter your email address and we'll send you a 6-digit reset code to reset your password.
                    </p>

                    <form onSubmit={handleSubmit} className="space-y-6">
                        <div>
                            <label htmlFor="email" className="block text-sm font-medium text-gray-300 mb-2">
                                Email Address
                            </label>
                            <input
                                type="email"
                                id="email"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                required
                                className="w-full px-3 py-3 bg-gray-800 border border-gray-700 rounded-md text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent"
                                placeholder="Enter your email"
                            />
                        </div>

                        {message && (
                            <div className="p-4 bg-green-900 border border-green-700 rounded-md">
                                <p className="text-green-300 text-sm">{message}</p>
                                <div className="mt-3">
                                    <Link 
                                        to="/reset-password" 
                                        className="inline-block bg-red-600 text-white py-2 px-4 rounded-md text-sm hover:bg-red-700 transition-colors"
                                    >
                                        Enter Reset Code
                                    </Link>
                                </div>
                            </div>
                        )}

                        {error && (
                            <div className="p-4 bg-red-900 border border-red-700 rounded-md">
                                <p className="text-red-300 text-sm">{error}</p>
                            </div>
                        )}

                        <button
                            type="submit"
                            disabled={isLoading}
                            className="w-full bg-red-600 text-white py-3 px-4 rounded-md font-semibold hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-2 focus:ring-offset-black disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                        >
                            {isLoading ? 'Sending...' : 'Send Reset Code'}
                        </button>
                    </form>

                    <div className="mt-6 text-center">
                        <Link 
                            to="/login" 
                            className="text-gray-400 hover:text-white text-sm transition-colors"
                        >
                            ← Back to Login
                        </Link>
                    </div>
                </div>

                {/* Additional Info */}
                <div className="mt-6 text-center text-gray-500 text-sm">
                    <p>Don't have an account?{' '}
                        <Link to="/register" className="text-red-600 hover:text-red-500">
                            Sign up
                        </Link>
                    </p>
                </div>
            </div>
        </div>
    );
}

export default ForgotPassword;

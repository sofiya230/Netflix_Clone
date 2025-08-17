import { useState, useEffect } from 'react';
import { authService } from '../../services/api';

function TwoFactorSettings() {
    const [email, setEmail] = useState('');
    const [isEnabled, setIsEnabled] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');
    const [showPassword, setShowPassword] = useState(false);
    const [password, setPassword] = useState('');

    useEffect(() => {

        const userData = localStorage.getItem('user_data');
        if (userData) {
            const user = JSON.parse(userData);
            setEmail(user.email);
            checkTwoFactorStatus(user.email);
        }
    }, []);

    const checkTwoFactorStatus = async (userEmail) => {
        try {
            const response = await authService.getTwoFactorStatus(userEmail);

            if (response.Success || response.success) {
                setIsEnabled(response.IsEnabled || response.isEnabled);
            }
        } catch (err) {
        }
    };

    const handleEnableTwoFactor = async () => {
        if (!email) {
            setError('Email is required');
            return;
        }

        setIsLoading(true);
        setError('');
        setMessage('');

        try {
            const response = await authService.enableTwoFactor({ email });

            
            if (response.Success || response.success) {
                setIsEnabled(true);
                setMessage(response.Message || response.message);
            } else {
                setError(response.Message || response.message || 'Failed to enable two-factor authentication');
            }
        } catch (err) {
            setError(err.message || 'Failed to enable two-factor authentication');
        } finally {
            setIsLoading(false);
        }
    };

    const handleDisableTwoFactor = async () => {
        if (!password) {
            setError('Password is required to disable 2FA');
            return;
        }

        setIsLoading(true);
        setError('');
        setMessage('');

        try {
            const response = await authService.disableTwoFactor({ email, password });

            
            if (response.Success || response.success) {
                setIsEnabled(false);
                setMessage(response.Message || response.message);
                setPassword('');
            } else {
                setError(response.Message || response.message || 'Failed to disable two-factor authentication');
            }
        } catch (err) {
            setError(err.message || 'Failed to disable two-factor authentication');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-black flex items-center justify-center px-4">
            <div className="max-w-md w-full">
                {/* Logo */}
                <div className="text-center mb-8">
                    <h1 className="text-red-600 text-4xl font-bold">
                        NetflixClone
                    </h1>
                </div>

                {/* Settings Form */}
                <div className="bg-black border border-gray-800 rounded-lg p-8">
                    <h2 className="text-white text-2xl font-bold mb-6 text-center">Two-Factor Authentication</h2>
                    
                    <div className="space-y-6">
                        {/* Current Status */}
                        <div className="text-center">
                            <div className={`inline-flex items-center px-4 py-2 rounded-full text-sm font-medium ${
                                isEnabled 
                                    ? 'bg-green-900 text-green-300 border border-green-700' 
                                    : 'bg-gray-800 text-gray-300 border border-gray-700'
                            }`}>
                                <span className={`w-2 h-2 rounded-full mr-2 ${
                                    isEnabled ? 'bg-green-400' : 'bg-gray-400'
                                }`}></span>
                                {isEnabled ? 'Enabled' : 'Disabled'}
                            </div>
                        </div>

                        {/* Enable/Disable Buttons */}
                        {!isEnabled ? (
                            <button
                                onClick={handleEnableTwoFactor}
                                disabled={isLoading}
                                className="w-full bg-green-600 text-white py-3 px-4 rounded-md font-semibold hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-offset-2 focus:ring-offset-black disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                            >
                                {isLoading ? 'Enabling...' : 'Enable Two-Factor Authentication'}
                            </button>
                        ) : (
                            <div className="space-y-4">
                                <div>
                                    <label htmlFor="password" className="block text-sm font-medium text-gray-300 mb-2">
                                        Confirm Password to Disable
                                    </label>
                                    <div className="relative">
                                        <input
                                            type={showPassword ? "text" : "password"}
                                            id="password"
                                            value={password}
                                            onChange={(e) => setPassword(e.target.value)}
                                            required
                                            className="w-full px-3 py-3 pr-12 bg-gray-800 border border-gray-700 rounded-md text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent"
                                            placeholder="Enter your password"
                                        />
                                        <button
                                            type="button"
                                            onClick={() => setShowPassword(!showPassword)}
                                            className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-white"
                                        >
                                            {showPassword ? (
                                                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.878 9.878L3 3m6.878 6.878L21 21" />
                                                </svg>
                                            ) : (
                                                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                                                </svg>
                                            )}
                                        </button>
                                    </div>
                                </div>
                                
                                <button
                                    onClick={handleDisableTwoFactor}
                                    disabled={isLoading || !password}
                                    className="w-full bg-red-600 text-white py-3 px-4 rounded-md font-semibold hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-2 focus:ring-offset-black disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                                >
                                    {isLoading ? 'Disabling...' : 'Disable Two-Factor Authentication'}
                                </button>
                            </div>
                        )}

                        {/* Messages */}
                        {message && (
                            <div className="p-4 bg-green-900 border border-green-700 rounded-md">
                                <p className="text-green-300 text-sm">{message}</p>
                            </div>
                        )}

                        {error && (
                            <div className="p-4 bg-red-900 border border-red-700 rounded-md">
                                <p className="text-red-300 text-sm">{error}</p>
                            </div>
                        )}

                        {/* Information */}
                        <div className="bg-gray-800 p-4 rounded-md">
                            <h3 className="text-white font-semibold mb-2">How it works:</h3>
                            <ul className="text-gray-300 text-sm space-y-1">
                                <li>• <strong>Enabling 2FA:</strong> Sets up security (no code sent)</li>
                                <li>• <strong>During Login:</strong> You'll receive a 6-digit code via email</li>
                                <li>• <strong>Enter the code:</strong> To complete your login</li>
                                <li>• <strong>Security:</strong> Codes expire after 10 minutes</li>
                                <li>• <strong>Disable:</strong> You can turn off 2FA anytime with your password</li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default TwoFactorSettings;


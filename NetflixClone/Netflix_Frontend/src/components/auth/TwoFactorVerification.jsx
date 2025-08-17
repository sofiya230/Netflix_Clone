import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { authService } from '../../services/api';

function TwoFactorVerification({ email, onVerificationSuccess }) {
    const [verificationCode, setVerificationCode] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState('');
    const [showPassword, setShowPassword] = useState(false);

    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        if (verificationCode.length !== 6) {
            setError('Please enter the 6-digit verification code');
            return;
        }

        setIsLoading(true);
        setError('');

        try {
            const response = await authService.verifyTwoFactor({ email, code: verificationCode });

            
            if (response.Success || response.success) {

                onVerificationSuccess(response);
            } else {
                setError(response.Message || response.message || 'Invalid verification code');
            }
        } catch (err) {
            setError(err.message || 'Failed to verify code. Please try again.');
        } finally {
            setIsLoading(false);
        }
    };

    const handleResendCode = async () => {
        setIsLoading(true);
        setError('');

        try {
            const response = await authService.resendTwoFactorCode({ email });

            
            if (response.Success || response.success) {
                setError('');
                alert('Verification code has been resent to your email');
            } else {
                setError(response.Message || response.message || 'Failed to resend code');
            }
        } catch (err) {
            setError(err.message || 'Failed to resend code');
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

                {/* Form */}
                <div className="bg-black border border-gray-800 rounded-lg p-8">
                    <h2 className="text-white text-2xl font-bold mb-6 text-center">Two-Factor Authentication</h2>
                    
                    <p className="text-gray-400 mb-6 text-center">
                        We've sent a 6-digit verification code to your email. Please enter it below.
                    </p>

                    <form onSubmit={handleSubmit} className="space-y-6">
                        <div>
                            <label htmlFor="verificationCode" className="block text-sm font-medium text-gray-300 mb-2">
                                Verification Code
                            </label>
                            <input
                                type="text"
                                id="verificationCode"
                                value={verificationCode}
                                onChange={(e) => setVerificationCode(e.target.value)}
                                required
                                maxLength={6}
                                pattern="[0-9]{6}"
                                className="w-full px-3 py-3 bg-gray-800 border border-gray-700 rounded-md text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent text-center text-2xl tracking-widest"
                                placeholder="000000"
                            />
                            <p className="text-xs text-gray-500 mt-1 text-center">Enter the 6-digit code sent to your email</p>
                        </div>

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
                            {isLoading ? 'Verifying...' : 'Verify Code'}
                        </button>

                        <div className="text-center">
                            <button
                                type="button"
                                onClick={handleResendCode}
                                disabled={isLoading}
                                className="text-gray-400 hover:text-white text-sm transition-colors disabled:opacity-50"
                            >
                                Didn't receive the code? Resend
                            </button>
                        </div>
                        

                    </form>

                    <div className="mt-6 text-center">
                        <button
                            onClick={() => {
                
                                localStorage.removeItem('pending_2fa_email');
                                navigate('/login');
                            }}
                            className="text-gray-400 hover:text-white text-sm transition-colors"
                        >
                            ← Back to Login
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default TwoFactorVerification;


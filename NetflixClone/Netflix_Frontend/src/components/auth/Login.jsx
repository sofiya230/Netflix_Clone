import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { authService } from '../../services/api';
import { Link } from 'react-router-dom';
import TwoFactorVerification from './TwoFactorVerification';

function Login() {
    const [mode, setMode] = useState('login');

    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [showPassword, setShowPassword] = useState(false);

    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);
    const [dob, setDob] = useState('');

    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const [requiresTwoFactor, setRequiresTwoFactor] = useState(false);
    const navigate = useNavigate();
    const { login } = useAuth();

    useEffect(() => {
        localStorage.removeItem('pending_2fa_login');
    }, []);

    useEffect(() => {
        return () => {
            if (!requiresTwoFactor) {
                localStorage.removeItem('pending_2fa_email');
            }
        };
    }, [requiresTwoFactor]);

    const isLogin = mode === 'login';

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        try {
            if (isLogin) {
                const response = await authService.login({ email, password });
                
                if (response.requiresTwoFactor) {
                    localStorage.setItem('pending_2fa_email', response.email);
                    setRequiresTwoFactor(true);
                    setError('');
                } else if (response.isSuccessful) {
                    const { token, user, role } = response;
                    login(user, token, role);
                    navigate('/profiles');
                } else {
                    setError(response.message || 'Login failed');
                }
            } else {
                if (!firstName || !lastName) {
                    throw new Error('Please enter first and last name.');
                }
                if (!dob) {
                    throw new Error('Please select your date of birth.');
                }
                if (password.length < 6) {
                    throw new Error('Password must be at least 6 characters.');
                }
                if (password !== confirmPassword) {
                    throw new Error('Passwords do not match.');
                }
                const dobIso = new Date(dob).toISOString();

                const payload = {
                    email,
                    password,
                    confirmPassword,
                    firstName,
                    lastName,
                    dateOfBirth: dobIso,
                };

                const resp = await authService.register(payload);
                

                if (resp?.isSuccessful === false) {

                    setError(resp.errorMessage || resp.message || 'Registration failed. Please try again.');
                    return;
                }
                

                
                if (resp?.token && resp?.user) {
                    login(resp.user, resp.token, resp.role);
                } else {

                    const { token, user, role } = await authService.login({ email, password });
                    login(user, token, role);
                }
                navigate('/profiles');
            }
        } catch (err) {
            setError(err?.message || 'Something went wrong.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div
            className="min-h-screen flex flex-col bg-black bg-cover bg-center relative"
            style={{ backgroundImage: "url('/images/Netflix.jpg')" }}
        >
            {/* Show 2FA verification if required */}
            {requiresTwoFactor ? (
                <TwoFactorVerification 
                    email={email}
                    onVerificationSuccess={(verificationResponse) => {
        
                        setRequiresTwoFactor(false);
                        
                        if (verificationResponse.token && verificationResponse.user) {
                            login(verificationResponse.user, verificationResponse.token, verificationResponse.role);
                            localStorage.removeItem('pending_2fa_email');
                            navigate('/profiles');
                        } else {
                            alert('Login failed after 2FA verification. Please try again.');
                            navigate('/login');
                        }
                    }}
                />
            ) : (
                <>
                    {/* Overlay */}
                    <div className="absolute inset-0 bg-black bg-opacity-60"></div>

                    {/* Header */}
                    <header className="relative z-10 px-4 py-6">
                        <svg
                            viewBox="0 0 111 30"
                            className="h-8 md:h-10 text-red-600 fill-current"
                            aria-hidden="true"
                            focusable="false"
                        >
                            <g id="netflix-logo">
                                <path d="M105.06233,14.2806261 L110.999156,30 C109.249227,29.7497422 107.500234,29.4366857 105.718437,29.1554972 L102.374168,20.4686475 L98.9371075,28.4375293 C97.2499766,28.1563408 95.5928391,28.061674 93.9057081,27.8432843 L99.9372012,14.0931671 L94.4680851,-5.68434189e-14 L99.5313525,-5.68434189e-14 L102.593495,7.87421502 L105.874965,-5.68434189e-14 L110.999156,-5.68434189e-14 L105.06233,14.2806261 Z M90.4686475,-5.68434189e-14 L85.8749649,-5.68434189e-14 L85.8749649,27.2499766 C87.3746368,27.3437061 88.9371075,27.4055675 90.4686475,27.5930265 L90.4686475,-5.68434189e-14 Z M81.9055207,26.93692 C77.7186241,26.6557316 73.5307901,26.4064111 69.250164,26.3117443 L69.250164,-5.68434189e-14 L73.9366389,-5.68434189e-14 L73.9366389,21.8745899 C76.6248008,21.9373887 79.3120255,22.1557784 81.9055207,22.2804387 L81.9055207,26.93692 Z M64.2496954,10.6561065 L64.2496954,15.3435186 L57.8442216,15.3435186 L57.8442216,25.9996251 L53.2186709,25.9996251 L53.2186709,-5.68434189e-14 L66.3436123,-5.68434189e-14 L66.3436123,4.68741213 L57.8442216,4.68741213 L57.8442216,10.6561065 Z M45.3435186,4.68741213 L45.3435186,26.2498828 C43.7810479,26.2498828 42.1876465,26.2498828 40.6561065,26.3117443 L40.6561065,4.68741213 L35.8121661,4.68741213 L35.8121661,-5.68434189e-14 L50.2183897,-5.68434189e-14 L50.2183897,4.68741213 L45.3435186,4.68741213 Z M30.749836,15.5928391 C28.687787,15.5928391 26.2498828,15.5928391 24.4999531,15.6875059 L24.4999531,22.6562939 C27.2499766,22.4678976 30,22.2495079 32.7809542,22.1557784 L32.7809542,26.6557316 L19.812541,27.6876933 L19.812541,-5.68434189e-14 L32.7809542,-5.68434189e-14 L32.7809542,4.68741213 L24.4999531,4.68741213 L24.4999531,10.9991564 C26.3126816,10.9991564 29.0936358,10.9054269 30.749836,10.9054269 L30.749836,15.5928391 Z M4.78114163,12.9684132 L4.78114163,29.3429562 C3.09401069,29.5313525 1.59340144,29.7497422 0,30 L0,-5.68434189e-14 L4.4690224,-5.68434189e-14 L10.562377,17.0315868 L10.562377,-5.68434189e-14 L15.2497891,-5.68434189e-14 L15.2497891,28.061674 C13.5935889,28.3437998 11.906458,28.4375293 10.1246602,28.6868498 L4.78114163,12.9684132 Z"></path>
                            </g>
                        </svg>
                    </header>

                    {/* Form card */}
                    <div className="flex-1 flex items-center justify-center z-10 px-4">
                        <div className="bg-black bg-opacity-70 p-8 md:p-12 rounded-md max-w-md w-full">
                            <h1 className="text-white text-3xl font-bold mb-6">
                                {isLogin ? 'Sign In' : 'Sign Up'}
                            </h1>

                            {error && (
                                <div className="bg-red-600 bg-opacity-70 text-white p-3 rounded mb-4">
                                    {error}
                                </div>
                            )}

                            <form onSubmit={handleSubmit}>
                                {/* Register-only fields */}
                                {!isLogin && (
                                    <>
                                        <div className="mb-4 grid grid-cols-1 md:grid-cols-2 gap-3">
                                            <input
                                                type="text"
                                                value={firstName}
                                                onChange={(e) => setFirstName(e.target.value)}
                                                placeholder="First name"
                                                required
                                                className="w-full p-4 rounded bg-gray-800 text-white border border-gray-600 focus:outline-none focus:border-gray-500"
                                            />
                                            <input
                                                type="text"
                                                value={lastName}
                                                onChange={(e) => setLastName(e.target.value)}
                                                placeholder="Last name"
                                                required
                                                className="w-full p-4 rounded bg-gray-800 text-white border border-gray-600 focus:outline-none focus:border-gray-500"
                                            />
                                        </div>

                                        <div className="mb-4">
                                            <input
                                                type="date"
                                                value={dob}
                                                onChange={(e) => setDob(e.target.value)}
                                                placeholder="Date of birth"
                                                required
                                                className="w-full p-4 rounded bg-gray-800 text-white border border-gray-600 focus:outline-none focus:border-gray-500"
                                            />
                                        </div>
                                    </>
                                )}


                                <div className="mb-4">
                                    <input
                                        type="email"
                                        value={email}
                                        onChange={(e) => setEmail(e.target.value)}
                                        placeholder="Email"
                                        required
                                        className="w-full p-4 rounded bg-gray-800 text-white border border-gray-600 focus:outline-none focus:border-gray-500"
                                    />
                                </div>

                                <div className="mb-4 relative">
                                    <input
                                        type={showPassword ? "text" : "password"}
                                        value={password}
                                        onChange={(e) => setPassword(e.target.value)}
                                        placeholder="Password"
                                        required
                                        className="w-full p-4 pr-12 rounded bg-gray-800 text-white border border-gray-600 focus:outline-none focus:border-gray-500"
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


                                {!isLogin && (
                                    <div className="mb-6 relative">
                                        <input
                                            type={showConfirmPassword ? "text" : "password"}
                                            value={confirmPassword}
                                            onChange={(e) => setConfirmPassword(e.target.value)}
                                            placeholder="Confirm Password"
                                            required
                                            className="w-full p-4 pr-12 rounded bg-gray-800 text-white border border-gray-600 focus:outline-none focus:border-gray-500"
                                        />
                                        <button
                                            type="button"
                                            onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                                            className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-white"
                                        >
                                            {showConfirmPassword ? (
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
                                )}

                                <button
                                    type="submit"
                                    disabled={loading}
                                    className="w-full bg-red-600 p-4 rounded font-semibold text-white hover:bg-red-700 transition disabled:opacity-70"
                                >
                                    {loading ? 'Please wait…' : isLogin ? 'Sign In' : 'Sign Up'}
                                </button>
                            </form>

                            <div className="mt-4 flex flex-col">
                                <div className="flex items-center text-sm text-gray-500">
                                    <input type="checkbox" id="remember" className="mr-2" />
                                    <label htmlFor="remember">Remember me</label>
                                    {isLogin && (
                                        <Link 
                                            to="/forgot-password" 
                                            className="ml-auto text-gray-500 hover:text-white hover:underline cursor-pointer"
                                        >
                                            Forgot password?
                                        </Link>
                                    )}
                                </div>

                                <div className="mt-6 text-gray-500">
                                    {isLogin ? 'New to Netflix?' : 'Already have an account?'}
                                    <button
                                        onClick={() => setMode(isLogin ? 'register' : 'login')}
                                        className="text-white ml-1 hover:underline"
                                    >
                                        {isLogin ? 'Sign up now' : 'Sign in'}
                                    </button>
                                </div>


                            </div>
                        </div>
                    </div>
                </>
            )}
        </div>
    );
}

export default Login;

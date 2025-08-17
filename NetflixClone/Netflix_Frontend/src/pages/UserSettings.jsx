

import { userService } from '../services/api';
import { FaEye, FaEyeSlash, FaShieldAlt } from 'react-icons/fa';
import React, { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { Link, useNavigate } from 'react-router-dom';


const UserSettings = () => {
    const { currentUser, logout } = useAuth();
    const navigate = useNavigate();

    const [userData, setUserData] = useState(null);
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [currentPassword, setCurrentPassword] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [subscriptionPlan, setSubscriptionPlan] = useState('');
    const [showPasswords, setShowPasswords] = useState({
        current: false,
        new: false,
        confirm: false,
    });
    const [isChangingPassword, setIsChangingPassword] = useState(false);

    useEffect(() => {
        const fetchUser = async () => {
            try {
                const data = await userService.getUser(currentUser.id);
                setUserData(data);
                setFirstName(data.firstName || '');
                setLastName(data.lastName || '');
                setSubscriptionPlan(data.subscriptionPlan || '');
                    } catch (error) {
            }
        };

        if (currentUser?.id) fetchUser();
    }, [currentUser]);

    if (!currentUser?.id || !userData) {
        return <div className="text-white p-4">Loading user settings...</div>;
    }

    const handleUpdateProfile = async () => {
        try {
            await userService.updateUser(currentUser.id, {
                firstName,
                lastName
            });
            alert('✅ Profile updated!');
        } catch (error) {
            alert('❌ Failed to update profile.');
        }
    };

    const handleChangePassword = async () => {

        if (!currentPassword.trim()) {
            alert('❌ Please enter your current password');
            return;
        }


        if (!newPassword.trim()) {
            alert('❌ Please enter a new password');
            return;
        }

        if (newPassword.length < 6) {
            alert('❌ New password must be at least 6 characters long');
            return;
        }


        if (currentPassword === newPassword) {
            alert('❌ New password must be different from current password');
            return;
        }


        if (newPassword !== confirmPassword) {
            alert('❌ New passwords do not match');
            return;
        }

        setIsChangingPassword(true);
        try {
            await userService.changePassword(currentUser.id, {
                currentPassword,
                newPassword,
                confirmNewPassword: confirmPassword
            });
            alert('✅ Password changed successfully!');
            

            setCurrentPassword('');
            setNewPassword('');
            setConfirmPassword('');
        } catch (error) {
            if (error.message) {
                alert(`❌ Failed to change password: ${error.message}`);
            } else {
                alert('❌ Failed to change password. Please check your current password.');
            }
        } finally {
            setIsChangingPassword(false);
        }
    };

    const handleChangeSubscription = async () => {
        try {
            await userService.changeSubscription(currentUser.id, {
                NewSubscriptionPlan: subscriptionPlan
            });
            alert('✅ Subscription updated!');
        } catch (error) {
            alert('❌ Failed to change subscription.');
        }
    };

    const handleDelete = async () => {
        try {
            await userService.deleteUser(currentUser.id);
            alert('✅ Account deleted successfully!');
            

            logout();
            navigate('/login');
        } catch (error) {
            alert('❌ Failed to delete account.');
        }
    };

    const togglePasswordVisibility = (field) => {
        setShowPasswords(prev => ({
            ...prev,
            [field]: !prev[field]
        }));
    };

    return (
        <div className="min-h-screen bg-black text-white px-4 py-10">
            <div className="max-w-2xl mx-auto space-y-8">
                <h1 className="text-3xl font-bold mb-6 border-b border-gray-600 pb-2">Account Settings</h1>

                {/* Basic Info */}
                <div className="bg-gray-900 p-6 rounded-lg shadow space-y-4">
                    <h2 className="text-xl font-semibold border-b border-gray-700 pb-2">Basic Info</h2>
                    <input
                        type="text"
                        className="w-full p-2 rounded bg-gray-800 text-white border border-gray-700"
                        placeholder="First Name"
                        value={firstName}
                        onChange={(e) => setFirstName(e.target.value)}
                    />
                    <input
                        type="text"
                        className="w-full p-2 rounded bg-gray-800 text-white border border-gray-700"
                        placeholder="Last Name"
                        value={lastName}
                        onChange={(e) => setLastName(e.target.value)}
                    />
                    <button
                        onClick={handleUpdateProfile}
                        className="bg-red-600 hover:bg-red-700 px-4 py-2 rounded text-white"
                    >
                        Update Profile
                    </button>
                </div>

                {/* Password Change */}
                <div className="bg-gray-900 p-6 rounded-lg shadow space-y-4">
                    <h2 className="text-xl font-semibold border-b border-gray-700 pb-2">Change Password</h2>
                    {[
                        { label: 'Current Password', state: currentPassword, field: 'current', setter: setCurrentPassword },
                        { label: 'New Password', state: newPassword, field: 'new', setter: setNewPassword },
                        { label: 'Confirm New Password', state: confirmPassword, field: 'confirm', setter: setConfirmPassword }
                    ].map(({ label, state, setter, field }) => (
                        <div key={field} className="relative">
                            <input
                                type={showPasswords[field] ? 'text' : 'password'}
                                className="w-full p-2 pr-10 rounded bg-gray-800 text-white border border-gray-700"
                                placeholder={label}
                                value={state}
                                onChange={(e) => setter(e.target.value)}
                                required
                            />
                            <span
                                className="absolute right-3 top-2.5 cursor-pointer text-gray-400 hover:text-white"
                                onClick={() => togglePasswordVisibility(field)}
                                title={showPasswords[field] ? 'Hide password' : 'Show password'}
                            >
                                {showPasswords[field] ? <FaEyeSlash /> : <FaEye />}
                            </span>
                        </div>
                    ))}
                    
                    {/* Password strength indicator */}
                    {newPassword && (
                        <div className="text-sm">
                            <div className="flex items-center space-x-2 mb-1">
                                <span className="text-gray-400">Password strength:</span>
                                <div className="flex space-x-1">
                                    {[
                                        { condition: newPassword.length >= 6, color: 'bg-red-500' },
                                        { condition: newPassword.length >= 8, color: 'bg-yellow-500' },
                                        { condition: newPassword.length >= 10, color: 'bg-green-500' },
                                        { condition: /[A-Z]/.test(newPassword), color: 'bg-blue-500' },
                                        { condition: /[0-9]/.test(newPassword), color: 'bg-purple-500' }
                                    ].map((strength, index) => (
                                        <div
                                            key={index}
                                            className={`w-2 h-2 rounded-full ${strength.condition ? strength.color : 'bg-gray-600'}`}
                                        />
                                    ))}
                                </div>
                            </div>
                            <div className="text-xs text-gray-500 space-y-1">
                                <div>• At least 6 characters</div>
                                <div>• At least 8 characters (recommended)</div>
                                <div>• At least 10 characters (strong)</div>
                                <div>• Include uppercase letter</div>
                                <div>• Include number</div>
                            </div>
                        </div>
                    )}
                    <button
                        onClick={handleChangePassword}
                        disabled={isChangingPassword}
                        className={`px-4 py-2 rounded text-white transition-colors ${
                            isChangingPassword 
                                ? 'bg-gray-600 cursor-not-allowed' 
                                : 'bg-blue-600 hover:bg-blue-700'
                        }`}
                    >
                        {isChangingPassword ? 'Changing Password...' : 'Change Password'}
                    </button>
                </div>

                {/* Subscription Plan */}
                <div className="bg-gray-900 p-6 rounded-lg shadow space-y-4">
                    <h2 className="text-xl font-semibold border-b border-gray-700 pb-2">Subscription Plan</h2>
                    <select
                        value={subscriptionPlan}
                        onChange={(e) => setSubscriptionPlan(e.target.value)}
                        className="w-full p-2 rounded bg-gray-800 text-white border border-gray-700"
                    >
                        <option value="Basic">Basic</option>
                        <option value="Standard">Standard</option>
                        <option value="Premium">Premium</option>
                    </select>
                    <button
                        onClick={handleChangeSubscription}
                        className="bg-yellow-500 hover:bg-yellow-600 px-4 py-2 rounded text-black font-bold"
                    >
                        Change Subscription
                    </button>
                </div>

                {/* Two-Factor Authentication */}
                <div className="bg-gray-900 p-6 rounded-lg shadow space-y-4">
                    <h2 className="text-xl font-semibold border-b border-gray-700 pb-2 flex items-center">
                        <FaShieldAlt className="mr-2" />
                        Two-Factor Authentication
                    </h2>
                    <p className="text-gray-400 text-sm">
                        Add an extra layer of security to your account by enabling two-factor authentication. 
                        <br /><span className="text-yellow-400">Note: Verification codes are only sent during login, not when enabling 2FA.</span>
                    </p>
                    <Link
                        to="/two-factor-settings"
                        className="inline-flex items-center bg-green-600 hover:bg-green-700 px-4 py-2 rounded text-white transition-colors"
                    >
                        <FaShieldAlt className="mr-2" />
                        Manage 2FA Settings
                    </Link>
                </div>

                {/* Account Actions */}
                <div className="bg-gray-900 p-6 rounded-lg shadow space-y-4">
                    <h2 className="text-xl font-semibold border-b border-gray-700 pb-2">Account</h2>
                    <button
                        onClick={handleDelete}
                        className="bg-red-800 hover:bg-red-900 px-4 py-2 rounded text-white"
                    >
                        Delete Account
                    </button>
                </div>
            </div>
        </div>
    );
};

export default UserSettings;


import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from '../contexts/AuthContext';
import { profileService } from "../services/api";

function AddProfile() {
    const { currentUser } = useAuth();
    const navigate = useNavigate();
    const [form, setForm] = useState({
        name: "",
        avatarUrl: "",
        isKidsProfile: false,
        language: "en-US",
        maturityLevel: "Adult"
    });
    const [error, setError] = useState("");

    const handleChange = (e) => {
        const { name, value, type, checked } = e.target;
        setForm((prev) => ({
            ...prev,
            [name]: type === "checkbox" ? checked : value
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");

        try {
            await profileService.createProfile(currentUser.id, form);
            navigate("/profiles");
        } catch (err) {
            setError("Failed to create profile.");
        }
    };

    return (
        <div className="min-h-screen bg-black text-white flex justify-center items-center">
            <form
                onSubmit={handleSubmit}
                className="bg-gray-900 p-8 rounded shadow max-w-md w-full"
            >
                <h2 className="text-2xl font-bold mb-6">Add Profile</h2>

                {error && <p className="text-red-500 mb-4">{error}</p>}

                <input
                    type="text"
                    name="name"
                    placeholder="Profile Name"
                    value={form.name}
                    onChange={handleChange}
                    required
                    className="w-full p-3 mb-4 bg-gray-800 rounded"
                />

                <input
                    type="text"
                    name="avatarUrl"
                    placeholder="Avatar URL (optional)"
                    value={form.avatarUrl}
                    onChange={handleChange}
                    className="w-full p-3 mb-4 bg-gray-800 rounded"
                />

                <label className="flex items-center mb-4">
                    <input
                        type="checkbox"
                        name="isKidsProfile"
                        checked={form.isKidsProfile}
                        onChange={handleChange}
                        className="mr-2"
                    />
                    Is Kids Profile
                </label>

                <select
                    name="language"
                    value={form.language}
                    onChange={handleChange}
                    className="w-full p-3 mb-4 bg-gray-800 rounded"
                >
                    <option value="en-US">English</option>
                    <option value="hi-IN">Hindi</option>
                    <option value="es-ES">Spanish</option>
                </select>

                {!form.isKidsProfile && (
                    <select
                        name="maturityLevel"
                        value={form.maturityLevel}
                        onChange={handleChange}
                        className="w-full p-3 mb-4 bg-gray-800 rounded"
                    >
                        <option value="Teen">Teen</option>
                        <option value="Adult">Adult</option>
                    </select>
                )}

                <button
                    type="submit"
                    className="w-full bg-red-600 hover:bg-red-700 p-3 rounded font-semibold"
                >
                    Create Profile
                </button>
            </form>
        </div>
    );
}

export default AddProfile;

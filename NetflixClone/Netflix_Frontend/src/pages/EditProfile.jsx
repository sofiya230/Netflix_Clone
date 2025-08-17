
import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { profileService } from "../services/api";
function EditProfile() {
    const { id } = useParams();
    const navigate = useNavigate();
    const [form, setForm] = useState({
        name: "",
        avatarUrl: "",
        language: "en-US",
        maturityLevel: "Adult"
    });
    const [isKidsProfile, setIsKidsProfile] = useState(false);
    const [error, setError] = useState("");

    useEffect(() => {
        const loadProfile = async () => {
            try {
                const profile = await profileService.getProfileById(id);
                setForm({
                    name: profile.name,
                    avatarUrl: profile.avatarUrl,
                    language: profile.language,
                    maturityLevel: profile.maturityLevel
                });
                setIsKidsProfile(profile.isKidsProfile);
            } catch (err) {
                setError("Failed to load profile.");
            }
        };

        loadProfile();
    }, [id]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");

        try {
            await profileService.updateProfile(id, form);
            navigate("/profiles");
        } catch (err) {
            setError("Failed to update profile.");
        }
    };

    return (
        <div className="min-h-screen bg-black text-white flex justify-center items-center">
            <form
                onSubmit={handleSubmit}
                className="bg-gray-900 p-8 rounded shadow max-w-md w-full"
            >
                <h2 className="text-2xl font-bold mb-6">Edit Profile</h2>

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
                    placeholder="Avatar URL"
                    value={form.avatarUrl}
                    onChange={handleChange}
                    className="w-full p-3 mb-4 bg-gray-800 rounded"
                />

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

                {!isKidsProfile && (
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
                    Update Profile
                </button>
            </form>
        </div>
    );
}

export default EditProfile;

import { useEffect, useState } from "react";
import { contentService } from "../services/api";
import { useAuth } from "../contexts/AuthContext";
import { useNavigate } from "react-router-dom";


   export default function AdminContents() {
    const { currentRole } = useAuth();
    const [contents, setContents] = useState([]);
    const [form, setForm] = useState({
        title: "", description: "", releaseYear: "", maturityRating: "", duration: "",
        totalSeasons: "", thumbnailUrl: "", coverImageUrl: "", trailerUrl: "",
        contentType: "", genre: "", director: "", cast: ""
    });
    const [editingId, setEditingId] = useState(null);
    const [loading, setLoading] = useState(true);
    const [err, setErr] = useState("");

    const navigate = useNavigate();

    const fetchData = async () => {
        setLoading(true);
        try {
            const all = await contentService.getAllContents();
    
            setContents(all);
        } catch (e) {

            setErr("Failed to load content");
        }
        setLoading(false);
    };

    useEffect(() => {
        fetchData();
    }, []);

    if (!currentRole || currentRole !== "Admin") {
        return (
            <div className="text-red-500 p-10">
                You must be an admin to access this page.
            </div>
        );
    }

    const handleChange = (e) => {
        setForm(f => ({
            ...f,
            [e.target.name]: e.target.value
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setErr("");
        try {
            if (!form.title || !form.description || !form.contentType || !form.genre) {
                setErr("Please fill in all required fields.");
                return;
            }

            if (editingId) {
                await contentService.updateContent(editingId, form);
            } else {
                await contentService.createContent(form);
            }

            setForm({
                title: "", description: "", releaseYear: "", maturityRating: "", duration: "",
                totalSeasons: "", thumbnailUrl: "", coverImageUrl: "", trailerUrl: "",
                contentType: "", genre: "", director: "", cast: ""
            });
            setEditingId(null);
            await fetchData();
        } catch (error) {
            setErr("Failed to save. " + (error.message || ""));
        }
    };

    const handleEdit = (content) => {
        setForm({ ...content });
        setEditingId(content.id);
    };

    const handleDelete = async (id) => {
        if (!window.confirm("Delete this content?")) return;
        try {
            await contentService.deleteContent(id);
            await fetchData();
        } catch (error) {
            setErr("Failed to delete. " + (error.message || ""));
        }
    };

    return (
        <div className="p-8 max-w-4xl mx-auto text-white">
            <h1 className="text-2xl font-bold mb-6">Admin: Manage Content</h1>
            {err && <div className="text-red-400 mb-4">{err}</div>}
            <form onSubmit={handleSubmit} className="space-y-4 bg-gray-800 p-4 rounded mb-8">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <input name="title" value={form.title} onChange={handleChange} className="p-2 rounded bg-gray-700" placeholder="Title" required />
                    <input name="description" value={form.description} onChange={handleChange} className="p-2 rounded bg-gray-700" placeholder="Description" required />
                    <input name="releaseYear" value={form.releaseYear} onChange={handleChange} className="p-2 rounded bg-gray-700" placeholder="Release Year" required />
                    <input name="maturityRating" value={form.maturityRating} onChange={handleChange} className="p-2 rounded bg-gray-700" placeholder="Maturity Rating" required />
                    <input name="duration" value={form.duration} onChange={handleChange} className="p-2 rounded bg-gray-700" placeholder="Duration" />
                    <input name="totalSeasons" value={form.totalSeasons} onChange={handleChange} className="p-2 rounded bg-gray-700" placeholder="Total Seasons" />
                    <input name="thumbnailUrl" value={form.thumbnailUrl} onChange={handleChange} className="p-2 rounded bg-gray-700" placeholder="Thumbnail URL" />
                    <input name="coverImageUrl" value={form.coverImageUrl} onChange={handleChange} className="p-2 rounded bg-gray-700" placeholder="Cover Image URL" />
                    <input name="trailerUrl" value={form.trailerUrl} onChange={handleChange} className="p-2 rounded bg-gray-700" placeholder="Trailer URL" />
                    <input name="contentType" value={form.contentType} onChange={handleChange} className="p-2 rounded bg-gray-700" placeholder="Content Type (Movie/Series)" required />
                    <input name="genre" value={form.genre} onChange={handleChange} className="p-2 rounded bg-gray-700" placeholder="Genre" required />
                    <input name="director" value={form.director} onChange={handleChange} className="p-2 rounded bg-gray-700" placeholder="Director" />
                    <input name="cast" value={form.cast} onChange={handleChange} className="p-2 rounded bg-gray-700" placeholder="Cast" />
                </div>
                <button type="submit" className="bg-blue-500 px-6 py-2 rounded font-semibold hover:bg-blue-700">
                    {editingId ? "Update Content" : "Add Content"}
                </button>
                {editingId && (
                    <button
                        type="button"
                        className="ml-4 bg-gray-600 px-4 py-2 rounded"
                        onClick={() => {
                            setEditingId(null);
                            setForm({
                                title: "", description: "", releaseYear: "", maturityRating: "", duration: "",
                                totalSeasons: "", thumbnailUrl: "", coverImageUrl: "", trailerUrl: "",
                                contentType: "", genre: "", director: "", cast: ""
                            });
                        }} >
                        Cancel Edit
                    </button>
                )}
            </form>

            <h2 className="text-xl font-semibold mb-2">All Content</h2>
            {loading ? <div>Loading...</div> : (
                <table className="w-full text-sm bg-gray-900 rounded overflow-x-auto">
                    <thead>
                        <tr>
                            <th className="p-2">Title</th>
                            <th className="p-2">Year</th>
                            <th className="p-2">Type</th>
                            <th className="p-2">Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {contents.map(content => (
                            <tr key={content.id}>
                                <td className="p-2">{content.title}</td>
                                <td className="p-2">{content.releaseYear}</td>
                                <td className="p-2">{content.contentType}</td>
                                <td className="p-2 flex gap-2">
                                    <button className="bg-green-600 px-2 py-1 rounded text-white" onClick={() => handleEdit(content)}>Edit</button>
                                    

                                    {(content.contentType?.toLowerCase().includes('series') || content.contentType?.toLowerCase().includes('tv show')) && (
                                        <button 
                                            className="bg-blue-600 px-3 py-1 rounded text-white font-medium hover:bg-blue-700" 
                                            onClick={() => {
                                        
                                                navigate(`/admin/episodes/${content.id}`);
                                            }}
                                        >
                                            📺 Manage Episodes
                                        </button>
                                    )}
                                    
                                    <button className="bg-red-600 px-2 py-1 rounded text-white" onClick={() => handleDelete(content.id)}>Delete</button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}

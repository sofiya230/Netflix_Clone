import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { contentService, episodeService } from "../services/api";

export default function AdminEpisodes() {
    const { contentId } = useParams();
    const navigate = useNavigate();
    const [episodes, setEpisodes] = useState([]);
    const [allContents, setAllContents] = useState([]);
    const [selectedContentId, setSelectedContentId] = useState(contentId);
    const [form, setForm] = useState({ title: "", description: "", seasonNumber: 1, episodeNumber: 1, duration: "", thumbnailUrl: "", videoUrl: "" });
    const [editingId, setEditingId] = useState(null);
    const [err, setErr] = useState("");
    const [content, setContent] = useState(null);

    const fetchEpisodes = async () => {
        if (!selectedContentId) return;
        const eps = await contentService.getEpisodes(selectedContentId, form.seasonNumber);
        setEpisodes(eps);
    };

    const fetchAllContents = async () => {
        try {
            const contents = await contentService.getAllContents();
            setAllContents(contents);
        } catch (error) {
        }
    };

    useEffect(() => {
        fetchAllContents();
    }, []);

    useEffect(() => {
        if (selectedContentId) {
            const loadContent = async () => {
                setContent(await contentService.getContentById(selectedContentId));
                fetchEpisodes();
            };
            loadContent();
        }
    }, [selectedContentId]);

    const handleContentChange = (e) => {
        const newContentId = e.target.value;
        setSelectedContentId(newContentId);
        if (newContentId) {
            navigate(`/admin/episodes/${newContentId}`);
        }
    };

    const handleChange = (e) => setForm(f => ({ ...f, [e.target.name]: e.target.value }));

    const handleSubmit = async (e) => {
        e.preventDefault();
        setErr("");
        try {
            if (editingId) {
                await episodeService.updateEpisode(editingId, form);
            } else {
                await episodeService.createEpisode({ ...form, contentId: parseInt(contentId) });
            }
            setForm({ title: "", description: "", seasonNumber: 1, episodeNumber: 1, duration: "", thumbnailUrl: "", videoUrl: "" });
            setEditingId(null);
            fetchEpisodes();
        } catch (error) {
            setErr("Failed to save. " + error.message);
        }
    };

    const handleEdit = (ep) => {
        setForm({ ...ep });
        setEditingId(ep.id);
    };

    const handleDelete = async (id) => {
        if (!window.confirm("Delete this episode?")) return;
        await episodeService.deleteEpisode(id);
        fetchEpisodes();
    };

    return (
        <div className="p-8 max-w-3xl mx-auto">
            <div className="mb-6">
                <h2 className="text-2xl text-white mb-4">Manage Episodes</h2>
                <div className="mb-4">
                    <label className="block text-white mb-2">Select Content:</label>
                    <select 
                        value={selectedContentId || ''} 
                        onChange={handleContentChange}
                        className="p-2 rounded bg-gray-700 text-white min-w-[300px]"
                    >
                        <option value="">-- Select Content --</option>
                        {allContents.map(content => (
                            <option key={content.id} value={content.id}>
                                {content.title} ({content.contentType})
                            </option>
                        ))}
                    </select>
                </div>
                {content && (
                    <h3 className="text-lg text-white mb-2">Episodes for: <span className="font-bold">{content.title}</span></h3>
                )}
            </div>
            
            {!selectedContentId ? (
                <div className="text-gray-400 text-center py-8">
                    Please select a content item to manage its episodes
                </div>
            ) : (
                <>
                    {err && <div className="text-red-500 mb-2">{err}</div>}
                    <form onSubmit={handleSubmit} className="bg-gray-900 p-4 rounded-lg mb-6 grid gap-2">
                        <h3 className="text-white mb-2">{editingId ? "Edit Episode" : "Add Episode"}</h3>
                        <div className="grid grid-cols-2 gap-3">
                            <input className="p-2 rounded" name="title" value={form.title} onChange={handleChange} placeholder="Title" required />
                            <input className="p-2 rounded" name="seasonNumber" value={form.seasonNumber} onChange={handleChange} type="number" placeholder="Season" min={1} required />
                            <input className="p-2 rounded" name="episodeNumber" value={form.episodeNumber} onChange={handleChange} type="number" placeholder="Episode #" min={1} required />
                            <input className="p-2 rounded" name="duration" value={form.duration} onChange={handleChange} placeholder="Duration" required />
                        </div>
                        <input className="p-2 rounded" name="thumbnailUrl" value={form.thumbnailUrl} onChange={handleChange} placeholder="Thumbnail URL" />
                        <input className="p-2 rounded" name="videoUrl" value={form.videoUrl} onChange={handleChange} placeholder="YouTube Video URL" />
                        <textarea className="p-2 rounded" name="description" value={form.description} onChange={handleChange} placeholder="Description" required rows={2} />
                        <div className="flex gap-3 mt-2">
                            <button type="submit" className="bg-blue-600 text-white px-4 py-2 rounded">{editingId ? "Update" : "Add"}</button>
                            {editingId && <button type="button" className="bg-gray-600 text-white px-4 py-2 rounded" onClick={() => { setForm({ title: "", description: "", seasonNumber: 1, episodeNumber: 1, duration: "", thumbnailUrl: "", videoUrl: "" }); setEditingId(null); }}>Cancel</button>}
                        </div>
                    </form>
                    <table className="w-full text-white text-sm">
                        <thead>
                            <tr className="bg-gray-800">
                                <th className="p-2">ID</th>
                                <th>Title</th>
                                <th>Season</th>
                                <th>Episode</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            {episodes.map(ep => (
                                <tr key={ep.id} className="border-b border-gray-700">
                                    <td className="p-2">{ep.id}</td>
                                    <td>{ep.title}</td>
                                    <td>{ep.seasonNumber}</td>
                                    <td>{ep.episodeNumber}</td>
                                    <td>
                                        <button className="text-blue-400 mr-2" onClick={() => handleEdit(ep)}>Edit</button>
                                        <button className="text-red-400" onClick={() => handleDelete(ep.id)}>Delete</button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </>
            )}
        </div>
    );
}

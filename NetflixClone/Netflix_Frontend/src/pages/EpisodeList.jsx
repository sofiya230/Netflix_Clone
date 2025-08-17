import { useEffect, useState } from 'react';
import { contentService } from "../services/api";

function EpisodeList({ contentId, totalSeasons = 1 }) {
    const [season, setSeason] = useState(1);
    const [episodes, setEpisodes] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchEpisodes = async () => {
            setLoading(true);
            try {
            
                const data = await contentService.getEpisodes(contentId, season);
                setEpisodes(data);
            } catch (err) {
                setEpisodes([]);
            } finally {
                setLoading(false);
            }
        };
        if (contentId) fetchEpisodes();
    }, [contentId, season]);

    return (
        <div className="bg-gray-900 p-4 rounded-lg mt-6">
            <div className="flex items-center justify-between mb-4">
                <h2 className="text-white text-lg font-semibold">Episodes</h2>
                {totalSeasons > 1 && (
                    <select
                        className="bg-gray-800 text-white px-3 py-1 rounded"
                        value={season}
                        onChange={(e) => setSeason(parseInt(e.target.value))}
                    >
                        {Array.from({ length: totalSeasons }, (_, i) => i + 1).map((s) => (
                            <option key={s} value={s}>Season {s}</option>
                        ))}
                    </select>
                )}
            </div>
            {loading ? (
                <p className="text-gray-400">Loading...</p>
            ) : episodes.length === 0 ? (
                <p className="text-gray-400">No episodes found for this season.</p>
            ) : (
                <ul className="space-y-4">
                    {episodes.map((ep) => (
                        <li key={ep.id} className="flex items-center bg-gray-800 rounded p-2 shadow">
                            {/* Thumbnail if exists */}
                            {ep.thumbnailUrl && (
                                <img
                                    src={ep.thumbnailUrl}
                                    alt={ep.title}
                                    className="w-24 h-14 rounded mr-4 object-cover"
                                />
                            )}
                            <div className="flex-1">
                                <div className="flex items-center">
                                    <strong className="text-white mr-2">
                                        Ep {ep.episodeNumber}:
                                    </strong>
                                    <span className="text-white">{ep.title}</span>
                                    <span className="ml-3 text-gray-400 text-sm">
                                        {ep.duration} min
                                    </span>
                                </div>
                                <p className="text-gray-300 text-sm">{ep.description}</p>
                                {/* Play on YouTube button */}
                                {ep.videoUrl && (
                                    <a
                                        href={ep.videoUrl}
                                        target="_blank"
                                        rel="noopener noreferrer"
                                        className="inline-block mt-1 text-blue-400 hover:underline text-xs"
                                    >
                                        ▶️ Watch on YouTube
                                    </a>
                                )}
                            </div>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
}

export default EpisodeList;

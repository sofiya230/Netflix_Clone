import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { contentService, episodeService, watchHistoryService } from '../services/api';
import { useAuth } from '../contexts/AuthContext';

function EpisodesPage() {
    const { id } = useParams();
    const [episodes, setEpisodes] = useState([]);
    const [seriesInfo, setSeriesInfo] = useState(null);
    const navigate = useNavigate();
    const { currentProfile } = useAuth();

    useEffect(() => {
        const fetchEpisodes = async () => {
            try {
                const series = await contentService.getContentById(id);
                setSeriesInfo(series);

                const episodeList = await episodeService.getEpisodesByContent(id);
                setEpisodes(episodeList);
                    } catch (error) {
        }
        };

        fetchEpisodes();
    }, [id]);

    const handleWatch = async (episode) => {
        if (!currentProfile) return;

        try {
            await watchHistoryService.updateWatchProgress(currentProfile.id, id, {
                                    episodeId: episode.id,
                watchedDuration: 0,
                totalDuration: episode.duration || 100,
                completed: false
            });

            navigate(`/watch/${id}?s=${episode.seasonNumber}&e=${episode.episodeNumber}`);
        } catch (err) {
        }
    };

    return (
        <div className="p-4 text-white">
            <h1 className="text-3xl font-bold mb-4">{seriesInfo?.title}</h1>
            <p className="text-sm mb-6 text-gray-300">{seriesInfo?.description}</p>

            <div className="grid gap-4 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4">
                {episodes.map((episode) => (
                    <div key={episode.id} className="bg-gray-800 rounded p-3 shadow">
                        <img
                            src={episode.thumbnailUrl}
                            alt={episode.title}
                            className="w-full h-40 object-cover rounded mb-2"
                        />
                        <h2 className="font-semibold text-lg mb-1">{episode.title}</h2>
                        <p className="text-sm text-gray-400 mb-2">{episode.duration}</p>
                        <button
                            onClick={() => handleWatch(episode)}
                            className="text-sm bg-white text-black px-4 py-1 rounded hover:bg-opacity-90"
                        >
                            Watch
                        </button>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default EpisodesPage;

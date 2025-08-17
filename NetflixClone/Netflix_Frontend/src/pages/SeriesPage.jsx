import { useEffect, useState } from 'react';
import { useParams, useNavigate, useSearchParams } from 'react-router-dom';
import PageLayout from '../components/layout/PageLayout';
import { contentService, episodeService, watchHistoryService } from '../services/api';
import { useAuth } from '../contexts/AuthContext';

function SeriesPage() {
    const { id } = useParams();
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const [content, setContent] = useState(null);
    const [season, setSeason] = useState(Number(searchParams.get('s')) || 1);
    const [episodes, setEpisodes] = useState([]);
    const [loading, setLoading] = useState(true);
    const { currentProfile } = useAuth();


    useEffect(() => {
        let cancelled = false;
        const load = async () => {
            setLoading(true);
            try {
                const c = await contentService.getContentById(id);
                if (!cancelled) setContent(c);

                const eps = await episodeService.getEpisodes(id, season);
                if (!cancelled) setEpisodes(eps || []);
                    } catch (err) {
        } finally {
                if (!cancelled) setLoading(false);
            }
        };
        load();
        return () => { cancelled = true; };
    }, [id, season]);

    const handleSeasonChange = async (e) => {
        const s = Number(e.target.value);
        setSeason(s);
        const eps = await episodeService.getEpisodes(id, s);
        setEpisodes(eps || []);
    };

    if (loading) return <div className="text-white p-4">Loading...</div>;
    if (!content) return <div className="text-white p-4">Series not found.</div>;

    return (
        <PageLayout>
            <div className="text-white p-6">
                <h1 className="text-3xl font-bold mb-2">{content.title}</h1>
                <p className="text-gray-400 mb-4">{content.description}</p>

                <div className="mb-4">
                    <label className="mr-2">Select Season:</label>
                    <select
                        value={season}
                        onChange={handleSeasonChange}
                        className="bg-gray-800 text-white p-2 rounded"
                    >
                        {Array.from({ length: content.totalSeasons || 1 }).map((_, idx) => (
                            <option key={idx + 1} value={idx + 1}>Season {idx + 1}</option>
                        ))}
                    </select>
                </div>

                <div className="space-y-4">
                    {episodes.map(ep => (
                        <button
                            key={ep.id}
                            onClick={async () => {
                                try {
                                    if (currentProfile?.id) {
                                        await watchHistoryService.updateWatchProgress(currentProfile.id, id, {
                                            episodeId: ep.id,
                                            watchedDuration: 0,
                                            watchedPercentage: 0,
                                            completed: false
                                        });
                                    }
                                        } catch (err) {
        }
                                navigate(`/watch/${id}?s=${ep.seasonNumber}&e=${ep.episodeNumber}`);
                            }}
                            className="w-full text-left border border-gray-700 p-4 rounded hover:bg-gray-800"
                        >
                            <h2 className="text-xl font-semibold">{ep.title}</h2>
                            <p className="text-sm text-gray-400">{ep.description}</p>
                            <p className="text-sm text-gray-500">Duration: {ep.duration}</p>
                        </button>
                    ))}
                </div>
            </div>
        </PageLayout>
    );
}

export default SeriesPage;

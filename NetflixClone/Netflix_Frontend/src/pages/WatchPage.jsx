import { useEffect, useMemo, useState } from 'react';
import { useParams, useNavigate, useSearchParams } from 'react-router-dom';
import { contentService, episodeService, watchHistoryService, myListService } from "../services/api";
import YouTubePlayer from '../components/player/YouTubePlayer';
import { useAuth } from '../contexts/AuthContext';

function WatchPage() {
    const { id } = useParams();
    const navigate = useNavigate();
    const [searchParams, setSearchParams] = useSearchParams();
    const { currentProfile } = useAuth();

    const startSeason = Number(searchParams.get('s')) || 1;
    const startEpisode = Number(searchParams.get('e')) || 1;

    const [content, setContent] = useState(null);
    const [selectedSeason, setSelectedSeason] = useState(startSeason);
    const [episodes, setEpisodes] = useState([]);
    const [selectedEpisode, setSelectedEpisode] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [isInMyList, setIsInMyList] = useState(false);

    useEffect(() => {
        if (content && currentProfile?.id) {
            myListService.isContentInMyList(currentProfile.id, id)
                .then(result => setIsInMyList(result))
                .catch(() => setIsInMyList(false));
        }
    }, [content, currentProfile?.id, id]);

    const handleAddToMyList = async () => {
        if (!currentProfile?.id) return;
        
        try {
            if (isInMyList) {
                await myListService.removeFromMyList(currentProfile.id, id);
                setIsInMyList(false);
            } else {
                await myListService.addToMyList(currentProfile.id, id);
                setIsInMyList(true);
            }
        } catch (err) {
        }
    };

    useEffect(() => {
        let cancelled = false;

        const load = async () => {
            setLoading(true);
            try {
                const c = await contentService.getContentById(id);
                if (cancelled) return;
                setContent(c);

                const eps = await episodeService.getEpisodes(id, selectedSeason);
                if (cancelled) return;
                setEpisodes(eps || []);

                const ep = (eps || []).find(e => e.episodeNumber === startEpisode) || (eps || [])[0] || null;
                setSelectedEpisode(ep);
            } catch (err) {
                if (!cancelled) setError('Failed to load content. Please try again later.');
            } finally {
                if (!cancelled) setLoading(false);
            }
        };

        load();
        return () => { cancelled = true; };
    }, [id, selectedSeason]);

    const onSeasonChange = async (s) => {
        setSelectedSeason(s);
        setSearchParams({ s: String(s) });
        const eps = await episodeService.getEpisodes(id, s);
        setEpisodes(eps || []);
        setSelectedEpisode((eps || [])[0] || null);
    };

    const onClickEpisode = async (ep) => {
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
        
        setSelectedEpisode(ep);
        setSearchParams({ s: String(ep.seasonNumber), e: String(ep.episodeNumber) });
    };

    const videoId = useMemo(() => {
        const url = selectedEpisode?.videoUrl || content?.videoUrl;
        if (!url) return null;
        const rx = /(?:youtube\.com\/(?:watch\?v=|embed\/|v\/)|youtu\.be\/)([a-zA-Z0-9_-]{11})/;
        const m = url.match(rx);
        return m ? m[1] : null;
    }, [selectedEpisode, content]);

    const hasVideo = !!videoId;

    if (loading) {
        return (
            <div className="min-h-screen bg-black flex items-center justify-center">
                <p className="text-white text-xl">Loading content...</p>
            </div>
        );
    }

    if (error) {
        return (
            <div className="min-h-screen bg-black flex flex-col items-center justify-center p-4">
                <p className="text-red-500 text-xl mb-4">{error}</p>
                <button className="bg-red-600 text-white px-4 py-2 rounded" onClick={() => navigate(-1)}>
                    Go Back
                </button>
            </div>
        );
    }

    const seasonNumbers = Array.from({ length: content?.totalSeasons || 1 }).map((_, i) => i + 1);

    return (
        <div className="min-h-screen bg-black">
            <div className="container mx-auto px-4 py-8 flex flex-col md:flex-row gap-10 items-start">
                {/* Left: Poster and Info */}
                <div className="w-full md:w-1/3 flex flex-col items-center">
                    <img
                        src={content?.thumbnailUrl || content?.coverImageUrl}
                        alt={content?.title}
                        className="rounded-lg shadow-lg mb-6 w-full object-cover"
                        style={{ maxHeight: 420 }}
                    />
                    <div className="bg-gray-900 p-6 rounded-lg shadow w-full">
                        <h1 className="text-white text-3xl font-bold mb-2">{content?.title}</h1>
                        <div className="flex flex-wrap items-center gap-4 mb-4">
                            <span className="text-green-500">{content?.releaseYear}</span>
                            {content?.maturityRating && (
                                <span className="border border-gray-500 px-2 py-0.5 text-sm text-white">
                                    {content.maturityRating}
                                </span>
                            )}
                            {content?.duration && <span className="text-white">{content.duration}</span>}
                            {content?.genre && <span className="text-gray-400">{content.genre}</span>}
                        </div>
                        {content?.description && <p className="text-white mb-4">{content.description}</p>}
                    </div>
                </div>

                {/* Right: Player + Episodes */}
                <div className="w-full md:w-2/3 flex flex-col">
                    <button
                        className="mb-4 text-gray-400 hover:text-white bg-gray-800 px-3 py-1 rounded self-start md:self-end"
                        onClick={() => navigate(-1)}
                    >
                        ← Back
                    </button>

                    {hasVideo ? (
                        <div>
                            <YouTubePlayer
                                videoId={videoId}
                                title={selectedEpisode?.title || content?.title}
                            />
                            {/* Action Buttons */}
                            <div className="mt-4 flex gap-4">
                                <button
                                    onClick={handleAddToMyList}
                                    className={`flex items-center ${
                                        isInMyList
                                            ? 'bg-gray-600 bg-opacity-70 text-white'
                                            : 'bg-white bg-opacity-20 text-white hover:bg-opacity-30'
                                    } py-2 px-6 rounded font-semibold transition-colors`}
                                >
                                    {isInMyList ? '✓ My List' : '+ My List'}
                                </button>
                                <button
                                    onClick={async () => {
                                        if (currentProfile?.id) {
                                            try {
                                                await watchHistoryService.updateWatchProgress(currentProfile.id, id, {
                                                    watchedDuration: content?.duration || 0,
                                                    watchedPercentage: 100,
                                                    completed: true
                                                });
                                                alert('Marked as completed! This will now appear in your Watch Again section.');
                                            } catch (err) {
                                                alert('Failed to mark as completed. Please try again.');
                                            }
                                        }
                                    }}
                                    className="hidden bg-green-600 hover:bg-green-700 text-white px-6 py-3 rounded-lg font-semibold transition-colors"
                                >
                                    ✅ Mark as Completed
                                </button>
                            </div>
                        </div>
                    ) : (
                        <div className="w-full max-w-4xl mx-auto aspect-video my-8 rounded-xl overflow-hidden shadow-lg border border-gray-800 bg-gray-900 flex items-center justify-center">
                            <div className="text-center text-white p-8">
                                <div className="text-6xl mb-4">🎬</div>
                                <h3 className="text-xl font-semibold mb-2">Video Not Available</h3>
                                <p className="text-gray-400 mb-4">
                                    {selectedEpisode ? 
                                        `Episode "${selectedEpisode.title}" doesn't have a video URL.` :
                                        `"${content?.title}" doesn't have a video URL.`
                                    }
                                </p>
                                <p className="text-sm text-gray-500">
                                    Please check with an administrator to add video URLs to this content.
                                </p>
                            </div>
                        </div>
                    )}

                    {(episodes && episodes.length > 0) && (
                        <div className="bg-gray-900 mt-6 rounded-lg p-4">
                            <h2 className="text-white text-2xl font-bold mb-3">Episodes</h2>

                            {seasonNumbers.length > 1 && (
                                <div className="mb-4">
                                    <select
                                        className="bg-gray-800 text-white px-3 py-1 rounded"
                                        value={selectedSeason}
                                        onChange={(e) => onSeasonChange(Number(e.target.value))}
                                    >
                                        {seasonNumbers.map((s) => (
                                            <option key={s} value={s}>Season {s}</option>
                                        ))}
                                    </select>
                                </div>
                            )}

                            <div className="flex flex-col gap-3">
                                {episodes.map(ep => (
                                    <div
                                        key={ep.id}
                                        className={`flex items-center gap-4 p-2 rounded cursor-pointer hover:bg-gray-700 transition
                      ${selectedEpisode?.id === ep.id ? "bg-gray-700 border-l-4 border-blue-400" : ""}`
                                        }
                                        onClick={() => onClickEpisode(ep)}
                                    >
                                        {ep.thumbnailUrl && (
                                            <img src={ep.thumbnailUrl} alt={ep.title} className="w-24 h-16 object-cover rounded" />
                                        )}
                                        <div className="flex flex-col">
                                            <span className="text-white font-bold">{ep.title}</span>
                                            {ep.description && (
                                                <span className="text-gray-400 text-sm">
                                                    {ep.description.length > 60 ? ep.description.slice(0, 60) + '…' : ep.description}
                                                </span>
                                            )}
                                        </div>
                                    </div>
                                ))}
                            </div>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}

export default WatchPage;

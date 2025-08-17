import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import PageLayout from '../components/layout/PageLayout';
import ContentRow from '../components/browse/ContentRow';
import { contentService, episodeService, myListService, watchHistoryService } from "../services/api";
import { useAuth } from '../contexts/AuthContext';

function ContentDetail() {
    const { id } = useParams();
    const navigate = useNavigate();
    const { currentProfile } = useAuth();

    const [content, setContent] = useState(null);
    const [similarContent, setSimilarContent] = useState([]);
    const [episodes, setEpisodes] = useState([]);
    const [selectedSeason, setSelectedSeason] = useState(1);
    const [loading, setLoading] = useState(true);
    const [showAllDescription, setShowAllDescription] = useState(false);
    const [isInMyList, setIsInMyList] = useState(false);

    const profileId = currentProfile?.id || localStorage.getItem('active_profile_id') || null;

    useEffect(() => {
        if (content && profileId) {
            myListService.isContentInMyList(profileId, id)
                .then(result => setIsInMyList(result))
                .catch(() => setIsInMyList(false));
        }
    }, [content, profileId, id]);

    useEffect(() => {
        let cancelled = false;

        const fetchContent = async () => {
            try {
                const contentData = await contentService.getContentById(id);
                if (cancelled) return;
                setContent(contentData);

                const similar = await contentService.getSimilarContent(id);
                if (!cancelled) setSimilarContent(similar || []);

                const type = contentData?.contentType?.toLowerCase();
                if ((type === 'tv show' || type === 'series') && (contentData.totalSeasons > 0)) {
                    const eps = await episodeService.getEpisodes(id, selectedSeason);
                    if (!cancelled) setEpisodes(eps || []);
                } else {
                    if (!cancelled) setEpisodes([]);
                }
                    } catch (error) {
            } finally {
                if (!cancelled) setLoading(false);
            }
        };

        fetchContent();
        return () => { cancelled = true; };
    }, [id, selectedSeason]);

    const handleSeasonChange = (e) => setSelectedSeason(parseInt(e.target.value, 10));

    const handleAddToMyList = async () => {
        if (!profileId) return;
        try {
            if (isInMyList) {
                await myListService.removeFromMyList(profileId, id);
                setIsInMyList(false);
                alert('Removed from My List');
            } else {
                await myListService.addToMyList(profileId, id);
                setIsInMyList(true);
                alert('Added to My List');
            }
        } catch {
            alert('Failed to update My List');
        }
    };

    const handlePlay = async () => {
        if (!content) return;

        try {
            if (profileId) {
                const isSeries = ['tv show', 'series'].includes(content.contentType?.toLowerCase() || '');
                const firstEpisode = episodes.length > 0 ? episodes[0] : null;
                
                await watchHistoryService.updateWatchProgress(profileId, id, {
                    episodeId: isSeries && firstEpisode ? firstEpisode.id : null,
                    watchedDuration: 0,
                    watchedPercentage: 0,
                    completed: false
                });
            }
        } catch (err) {
        }

        const isSeries = ['tv show', 'series'].includes(content.contentType?.toLowerCase() || '');
        const query = isSeries && episodes.length ? `?s=${selectedSeason}&e=1` : '';
        navigate(`/watch/${id}${query}`);
    };

    if (loading) {
        return (
            <PageLayout>
                <div className="flex items-center justify-center h-screen">
                    <div className="w-16 h-16 border-t-4 border-red-600 border-solid rounded-full animate-spin"></div>
                </div>
            </PageLayout>
        );
    }

    if (!content) {
        return (
            <PageLayout>
                <div className="flex flex-col items-center justify-center h-screen">
                    <h1 className="text-white text-2xl mb-4">Content Not Found</h1>
                    <a href="/" className="text-red-600 hover:underline">Back to Browse</a>
                </div>
            </PageLayout>
        );
    }

    const castText = Array.isArray(content.cast)
        ? content.cast.join(', ')
        : (content.cast || 'Not available');

    const hasVideo = !!(content?.videoUrl);

    return (
        <PageLayout>
            <div className="relative w-full h-[50vw] max-h-[60vh]">
                <img
                    src={content.coverImageUrl || content.thumbnailUrl}
                    alt={content.title}
                    className="absolute inset-0 w-full h-full object-cover"
                />
                <div className="absolute inset-0 bg-gradient-to-t from-black to-transparent"></div>

                <div className="absolute bottom-0 left-0 w-full p-6 md:p-16">
                    <h1 className="text-white text-3xl md:text-5xl font-bold mb-4">{content.title}</h1>
                    <div className="flex items-center gap-4 mb-6">
                        <button
                            onClick={handlePlay}
                            disabled={!hasVideo}
                            className={`flex items-center py-2 px-6 rounded font-semibold transition ${
                                hasVideo 
                                    ? 'bg-white text-black hover:bg-opacity-90' 
                                    : 'bg-gray-500 text-gray-300 cursor-not-allowed'
                            }`}
                            title={hasVideo ? 'Play content' : 'No video available'}
                        >
                            {hasVideo ? '▶️ Play' : '⏸️ No Video'}
                        </button>
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
                                if (profileId) {
                                    try {
                                        await watchHistoryService.updateWatchProgress(profileId, id, {
                                            watchedDuration: content.duration || 0,
                                            watchedPercentage: 100,
                                            completed: true
                                        });
                                        alert('Marked as completed! This will now appear in your Watch Again section.');
                                    } catch (err) {
                                        alert('Failed to mark as completed. Please try again.');
                                    }
                                }
                            }}
                            className="hidden flex items-center bg-green-600 bg-opacity-70 text-white py-2 px-6 rounded font-semibold hover:bg-opacity-60 transition-colors"
                        >
                            ✅ Mark Completed
                        </button>
                    </div>
                </div>
            </div>

            <div className="px-6 md:px-16 py-8 bg-black">
                <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
                    <div className="md:col-span-2">
                        <div className="flex items-center mb-4">
                            <span className="text-green-500 font-semibold mr-2">{content.releaseYear}</span>
                            <span className="text-white border px-1 text-xs mr-2">{content.maturityRating}</span>
                            <span className="text-white mr-4">
                                {['tv show', 'series'].includes(content.contentType?.toLowerCase() || '')
                                    ? `${content.totalSeasons} Seasons`
                                    : content.duration}
                            </span>
                            <span className="text-white px-1 border border-white">HD</span>
                        </div>
                        <div className="mb-6">
                            <p className={`text-white ${!showAllDescription ? 'line-clamp-3' : ''}`}>{content.description}</p>
                            {content.description?.length > 150 && (
                                <button
                                    onClick={() => setShowAllDescription(!showAllDescription)}
                                    className="text-gray-400 text-sm mt-1 hover:text-white"
                                >
                                    {showAllDescription ? 'Show less' : 'Show more'}
                                </button>
                            )}
                        </div>
                    </div>

                    <div>
                        <p className="text-gray-400 mb-1"><span className="text-gray-500">Cast:</span> {castText}</p>
                        <p className="text-gray-400 mb-1"><span className="text-gray-500">Genres:</span> {content.genre}</p>
                        {Array.isArray(content.tags) && (
                            <p className="text-gray-400 mb-1"><span className="text-gray-500">Tags:</span> {content.tags.join(', ')}</p>
                        )}
                    </div>
                </div>

                {['tv show', 'series'].includes(content.contentType?.toLowerCase() || '') && episodes.length > 0 && (
                    <div className="mt-8 border-t border-gray-800 pt-8">
                        <div className="flex items-center justify-between mb-6">
                            <h2 className="text-white text-2xl font-semibold">Episodes</h2>
                            {(content.totalSeasons || 0) > 1 && (
                                <select
                                    value={selectedSeason}
                                    onChange={handleSeasonChange}
                                    className="bg-black text-white border border-gray-600 py-1 px-3 rounded"
                                >
                                    {Array.from({ length: content.totalSeasons || 1 }, (_, i) => (
                                        <option key={i} value={i + 1}>Season {i + 1}</option>
                                    ))}
                                </select>
                            )}
                        </div>
                        <div className="space-y-4">
                            {episodes.map((ep) => (
                                <button
                                    key={ep.id}
                                    onClick={() => navigate(`/watch/${id}?s=${ep.seasonNumber}&e=${ep.episodeNumber}`)}
                                    className="flex space-x-4 p-4 hover:bg-gray-900 rounded-lg transition text-left w-full"
                                >
                                    <div className="w-32 md:w-40 flex-shrink-0">
                                        <img src={ep.thumbnailUrl} alt={ep.title} className="w-full h-auto rounded" />
                                    </div>
                                    <div className="flex-grow">
                                        <div className="flex justify-between">
                                            <h3 className="text-white font-semibold">{ep.episodeNumber}. {ep.title}</h3>
                                            <span className="text-gray-400">{ep.duration}</span>
                                        </div>
                                        <p className="text-gray-400 mt-2">{ep.description}</p>
                                    </div>
                                </button>
                            ))}
                        </div>
                    </div>
                )}

                {similarContent.length > 0 && (
                    <div className="mt-8 border-t border-gray-800 pt-8">
                        <ContentRow title="More Like This" contents={similarContent} />
                    </div>
                )}
            </div>
        </PageLayout>
    );
}

export default ContentDetail;

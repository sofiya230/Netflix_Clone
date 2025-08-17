import { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { myListService } from '../../services/api';

function HeroBanner({ content }) {
    const [adding, setAdding] = useState(false);
    const [inList, setInList] = useState(false);
    const navigate = useNavigate();
    const { currentProfile } = useAuth();

    useEffect(() => {
        let cancelled = false;
        async function check() {
            if (!currentProfile?.id || !content?.id) return;
            try {
                const res = await myListService.isContentInMyList(currentProfile.id, content.id);
                if (!cancelled) setInList(!!res || res?.isInList === true);
            } catch {
                if (!cancelled) setInList(false);
            }
        }
        check();
        return () => { cancelled = true; };
    }, [currentProfile?.id, content?.id]);

    const toggleMyList = useCallback(async () => {
        if (!currentProfile?.id || !content?.id || adding) return;
        setAdding(true);
        try {
            if (inList) {
                await myListService.removeFromMyList(currentProfile.id, content.id);
                setInList(false);
            } else {
                await myListService.addToMyList(currentProfile.id, content.id);
                setInList(true);
            }
        } catch (e) {
        } finally {
            setAdding(false);
        }
    }, [currentProfile?.id, content?.id, inList, adding]);

    if (!content) return null;

    const isSeries = content.contentType === 'TV Show' || content.type === 'series';
    const detailUrl = isSeries ? `/series/${content.id}` : `/content/${content.id}`;

    return (
        <div className="relative w-full h-[50vw] max-h-[65vh] overflow-hidden mt-0">
            <div className="absolute inset-0">
                <img
                    src={content.coverImageUrl || content.coverUrl || "/fallback-cover.jpg"}
                    alt={content.title}
                    className="w-11/12 h-full object-cover mx-auto"
                />
                <div className="absolute inset-0 bg-gradient-to-t from-black to-transparent opacity-80" />
                <div className="absolute inset-y-0 left-0 w-1/4 bg-gradient-to-r from-black to-transparent" />
            </div>

            <div className="absolute bottom-[15%] md:bottom-[20%] left-0 w-full p-4 md:p-16">
                <div className="max-w-xl">
                    <h1 className="text-white text-2xl md:text-5xl font-bold mb-4">{content.title}</h1>

                    <div className="flex items-center space-x-2 mb-4">
                        {content.releaseYear && (
                            <span className="text-green-500 font-semibold">{content.releaseYear}</span>
                        )}
                        {content.maturityRating && (
                            <span className="text-white border px-1">{content.maturityRating}</span>
                        )}
                        <span className="text-white">
                            {isSeries
                                ? (content.totalSeasons ? `${content.totalSeasons} Seasons` : 'Limited Series')
                                : content.duration}
                        </span>
                    </div>

                    {content.description && (
                        <p className="text-white mb-6 line-clamp-3 md:line-clamp-none">
                            {content.description}
                        </p>
                    )}

                    <div className="flex flex-wrap gap-3">
                        <button
                            onClick={() => navigate(detailUrl)}
                            className="flex items-center justify-center bg-white text-black py-2 px-6 rounded font-semibold hover:bg-opacity-90 transition-colors"
                        >
                            <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5 mr-2" viewBox="0 0 20 20" fill="currentColor">
                                <path fillRule="evenodd" d="M6 4l10 6-10 6V4z" clipRule="evenodd" />
                            </svg>
                            Details
                        </button>

                        <button
                            onClick={toggleMyList}
                            disabled={adding}
                            className="flex items-center justify-center bg-gray-600 bg-opacity-70 text-white py-2 px-6 rounded font-semibold hover:bg-opacity-60 transition-colors"
                            title={inList ? 'Remove from My List' : 'Add to My List'}
                        >
                            {inList ? '✓ In My List' : '+ My List'}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default HeroBanner;

import { useState, useEffect, useCallback } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { myListService } from '../../services/api';

function ContentCard({ content, expanded = false, hoverEffect = true }) {
    const [isHovered, setIsHovered] = useState(false);
    const [inList, setInList] = useState(false);
    const [busy, setBusy] = useState(false);
    const { currentProfile } = useAuth();

    const displayDuration = () => {
        const isSeries =
            content?.contentType?.toLowerCase() === 'tv show' ||
            content?.contentType?.toLowerCase() === 'series' ||
            content?.type?.toLowerCase() === 'series';

        if (isSeries) {
            return content?.totalSeasons ? `${content.totalSeasons} Seasons` : content?.duration;
        }
        return content?.duration;
    };

    const hasVideo = !!(content?.videoUrl);
    const isValidYear = !!content?.releaseYear && content.releaseYear !== 0;
    const isValidRating = !!content?.maturityRating && String(content.maturityRating).toLowerCase() !== 'string';
    const isValidGenre = !!content?.genre && String(content.genre).toLowerCase() !== 'string';

    const isSeriesType =
        content?.contentType?.toLowerCase() === 'tv show' ||
        content?.contentType?.toLowerCase() === 'series' ||
        content?.type?.toLowerCase() === 'series';

    const linkUrl = isSeriesType ? `/series/${content.id}` : `/content/${content.id}`;

    useEffect(() => {
        let cancelled = false;
        const check = async () => {
            if (!currentProfile?.id || !content?.id) return;
            try {
                const res = await myListService.isContentInMyList(currentProfile.id, content.id);
                if (!cancelled) setInList(!!res || res?.isInList === true);
            } catch (error) {
                if (!cancelled) setInList(false);
            }
        };
        check();
        return () => { cancelled = true; };
    }, [currentProfile?.id, content?.id]);

    const toggleMyList = useCallback(async () => {
        if (!currentProfile?.id || !content?.id || busy) return;
        setBusy(true);
        try {
            if (inList) {
                await myListService.removeFromMyList(currentProfile.id, content.id);
                setInList(false);
            } else {
                await myListService.addToMyList(currentProfile.id, content.id);
                setInList(true);
            }
        } catch (error) {

            alert(`Failed to ${inList ? 'remove from' : 'add to'} My List: ${error.message || 'Unknown error'}`);
        } finally {
            setBusy(false);
        }
    }, [currentProfile?.id, content?.id, inList, busy]);

    return (
        <div
            className={`relative ${hoverEffect ? 'content-card' : ''} ${expanded ? 'w-full' : ''}`}
            onMouseEnter={() => setIsHovered(true)}
            onMouseLeave={() => setIsHovered(false)}
        >
            <div className="relative aspect-[2/3] w-full overflow-hidden rounded-sm">

                <Link
                    to={linkUrl}
                    className="absolute inset-0 block"
                    aria-label={content?.title || 'Open'}
                >
                    <img
                        src={content?.thumbnailUrl || '/fallback-thumbnail.png'}
                        alt={content?.title || ''}
                        className="h-full w-full object-cover"
                        loading="lazy"
                        draggable={false}
                    />
                </Link>

                <button
                    type="button"
                    onClick={(e) => { e.preventDefault(); e.stopPropagation(); toggleMyList(); }}
                    disabled={busy}
                    className="absolute top-2 right-2 bg-black/70 text-white text-xs px-2 py-1 rounded hover:bg-black/85 pointer-events-auto"
                    title={inList ? 'Remove from My List' : 'Add to My List'}
                >
                    {inList ? '✓' : '+'}
                </button>



                {hoverEffect && isHovered && (
                    <div className="pointer-events-none absolute inset-0 bg-black/70 flex flex-col justify-end p-3">
                        <h3 className="text-white font-bold text-sm line-clamp-2">{content?.title}</h3>

                        <div className="flex items-center mt-1 text-xs space-x-2">
                            {isValidYear && <span className="text-green-500 font-semibold">{content.releaseYear}</span>}
                            {isValidRating && <span className="text-white border px-1 text-[10px]">{content.maturityRating}</span>}
                            {content?.duration && <span className="text-white">{displayDuration()}</span>}
                        </div>

                        <div className="mt-1 text-xs text-gray-200 space-y-0.5">
                            {content?.director && <p>Director: {content.director}</p>}
                            {content?.cast && <p>Cast: {content.cast}</p>}
                        </div>

                        {isValidGenre && (
                            <div className="mt-1 flex flex-wrap gap-1">
                                <span className="text-white text-xs">{content.genre}</span>
                            </div>
                        )}

                        {hasVideo && (
                            <div className="mt-2 flex items-center gap-2">
                                <span className="bg-red-600 text-white text-xs px-2 py-1 rounded">▶</span>
                                <span className="text-green-400 text-xs font-semibold">Video Available</span>
                            </div>
                        )}
                    </div>
                )}
            </div>

            {expanded && (
                <div className="mt-2">
                    <h3 className="text-white font-bold">{content?.title}</h3>
                    <div className="flex items-center mt-1 text-xs space-x-2">
                        {isValidYear && <span className="text-green-500 font-semibold">{content.releaseYear}</span>}
                        {isValidRating && <span className="text-white border px-1 text-[10px]">{content.maturityRating}</span>}
                        {content?.duration && <span className="text-white">{displayDuration()}</span>}
                    </div>
                    {content?.description && (
                        <p className="text-white text-sm mt-2 line-clamp-3">{content.description}</p>
                    )}
                    <div className="text-sm text-gray-300 mt-1 space-y-0.5">
                        {content?.director && <p><strong>Director:</strong> {content.director}</p>}
                        {content?.cast && <p><strong>Cast:</strong> {content.cast}</p>}
                        {content?.genre && <p><strong>Genre:</strong> {content.genre}</p>}
                    </div>
                </div>
            )}
        </div>
    );
}

export default ContentCard;

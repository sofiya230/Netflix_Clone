import { useEffect, useState } from 'react';
import { watchHistoryService } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import ContentCard from '../components/browse/ContentCard';
import PageLayout from '../components/layout/PageLayout';

function WatchAgainPage() {
    const { currentProfile } = useAuth();
    const [watchedContent, setWatchedContent] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        if (!currentProfile?.id) return;

        const fetchWatched = async () => {
            setLoading(true);
            setError(null);
            try {

                const result = await watchHistoryService.getByProfile(currentProfile.id);

                setWatchedContent(result || []);
            } catch (err) {
                setError('Failed to load watch again content. Please try again.');
            } finally {
                setLoading(false);
            }
        };

        fetchWatched();
    }, [currentProfile?.id]);

    const handleRemoveFromHistory = async (contentId) => {
        try {
            
            const historyItem = watchedContent.find(item => item.contentId === contentId);
            if (historyItem?.id) {
                await watchHistoryService.delete(historyItem.id);

                setWatchedContent(prev => prev.filter(item => item.contentId !== contentId));
            }
        } catch (err) {
            alert('Failed to remove item from watch history. Please try again.');
        }
    };

    if (!currentProfile?.id) {
        return (
            <PageLayout>
                <div className="p-6 text-white">
                    <h1 className="text-2xl font-bold mb-6">Watch Again</h1>
                    <p className="text-gray-300">Please select a profile to view your watch again content.</p>
                </div>
            </PageLayout>
        );
    }

    return (
        <PageLayout>
            <div className="p-6 text-white">
                <h1 className="text-2xl font-bold mb-6">Watch Again</h1>
                <p className="text-gray-400 mb-6">Content you've started watching. Click the X to remove items you don't want to see again.</p>
                
                {loading ? (
                    <div className="flex items-center justify-center py-12">
                        <div className="w-8 h-8 border-t-2 border-red-600 border-solid rounded-full animate-spin"></div>
                        <span className="ml-3 text-gray-300">Loading your watched content...</span>
                    </div>
                ) : error ? (
                    <div className="text-center py-12">
                        <p className="text-red-400 mb-4">{error}</p>
                        <button 
                            onClick={() => window.location.reload()} 
                            className="bg-red-600 hover:bg-red-700 px-4 py-2 rounded text-white transition-colors"
                        >
                            Try Again
                        </button>
                    </div>
                ) : watchedContent.length === 0 ? (
                    <div className="text-center py-12">
                        <p className="text-gray-300 text-lg mb-4">You haven't watched any content yet.</p>
                        <p className="text-gray-400">Start watching movies and TV shows to see them here!</p>
                    </div>
                ) : (
                    <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
                        {watchedContent.map(item => (
                            <div key={`${item.contentId}-${item.episodeId || 'movie'}`} className="relative">
                                <ContentCard 
                                    content={item.content || item} 
                                    hoverEffect 
                                />
                                {/* Remove button */}
                                <button
                                    onClick={() => handleRemoveFromHistory(item.contentId)}
                                    className="absolute top-2 right-2 bg-red-600 hover:bg-red-700 text-white rounded-full w-6 h-6 flex items-center justify-center text-xs font-bold transition-colors z-10"
                                    title="Remove from watch history"
                                >
                                    ×
                                </button>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </PageLayout>
    );
}

export default WatchAgainPage;
import { useEffect, useState } from 'react';
import { watchHistoryService } from '../services/api';
import PageLayout from '../components/layout/PageLayout';
import { Link } from 'react-router-dom';

function WatchHistory() {
    const [history, setHistory] = useState([]);
    const [loading, setLoading] = useState(true);
    const [profileId, setProfileId] = useState(null);

    useEffect(() => {
        const activeProfileId = localStorage.getItem('active_profile_id');
        if (!activeProfileId) return;

        setProfileId(activeProfileId);

        const fetchHistory = async () => {
            try {
                const data = await watchHistoryService.getByProfile(activeProfileId);
                setHistory(data);
                    } catch (err) {
        } finally {
                setLoading(false);
            }
        };

        fetchHistory();
    }, []);

    return (
        <PageLayout>
            <div className="px-6 md:px-16 py-10 bg-black min-h-screen text-white">
                <h1 className="text-3xl font-bold mb-6">Watch History</h1>
                {loading ? (
                    <p className="text-gray-400">Loading...</p>
                ) : history.length === 0 ? (
                    <p className="text-gray-400">You haven�t watched anything yet.</p>
                ) : (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                        {history.map((item) => {
                            const percentage = item.watchedPercentage || 0;

                            return (
                                <div key={item.id} className="bg-gray-800 rounded-lg overflow-hidden shadow-md">
                                    <Link to={`/content/${item.contentId}`}>
                                        <img
                                            src={item.content?.thumbnailUrl || '/placeholder.jpg'}
                                            alt={item.content?.title || 'Content'}
                                            className="w-full h-40 object-cover"
                                        />
                                    </Link>
                                    <div className="p-4">
                                        <h3 className="text-lg font-semibold">
                                            {item.content?.title || 'Unknown Content'}
                                        </h3>
                                        <p className="text-sm text-gray-400">
                                            Watched: {item.watchedDuration || 0} min
                                        </p>
                                        <div className="w-full bg-gray-600 h-2 rounded mt-2">
                                            <div
                                                className="bg-red-600 h-2 rounded"
                                                style={{ width: `${percentage}%` }}
                                            ></div>
                                        </div>
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                )}
            </div>
        </PageLayout>
    );
}

export default WatchHistory;

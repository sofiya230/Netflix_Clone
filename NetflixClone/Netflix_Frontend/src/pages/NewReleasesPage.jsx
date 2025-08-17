import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import PageLayout from '../components/layout/PageLayout';
import { contentService } from '../services/api';

export default function NewReleasesPage() {
    const [items, setItems] = useState([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        let cancelled = false;
        (async () => {
            try {
                const data = await contentService.getNewReleases();
                if (!cancelled) setItems(Array.isArray(data) ? data : []);
                    } catch (err) {
        } finally {
                if (!cancelled) setLoading(false);
            }
        })();
        return () => { cancelled = true; };
    }, []);

    if (loading) return <div className="text-white p-6">Loading...</div>;

    return (
        <PageLayout>
            <div className="p-6 text-white">
                <h1 className="text-2xl mb-4">New & Popular</h1>

                {!items.length ? (
                    <div className="opacity-80">No new releases right now.</div>
                ) : (
                    <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-4">
                        {items.map(item => (
                            <button
                                key={item.id}
                                className="bg-neutral-800 rounded-lg overflow-hidden text-left hover:opacity-90 transition focus:outline-none focus:ring-2 focus:ring-neutral-500"
                                onClick={() => navigate(`/content/${item.id}`)}
                            >
                                <img
                                    src={item.thumbnailUrl || '/images/placeholder.jpg'}
                                    alt={item.title}
                                    className="w-full h-40 object-cover"
                                />
                                <div className="p-3">
                                    <div className="font-semibold line-clamp-1">{item.title}</div>
                                    <div className="text-sm opacity-70">{item.releaseYear ?? ''}</div>
                                </div>
                            </button>
                        ))}
                    </div>
                )}
            </div>
        </PageLayout>
    );
}

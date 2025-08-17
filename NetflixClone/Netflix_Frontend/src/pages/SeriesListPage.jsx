import { useEffect, useState } from 'react';
import PageLayout from '../components/layout/PageLayout';
import { contentService } from '../services/api';
import ContentCard from '../components/browse/ContentCard';

export default function SeriesListPage() {
    const [series, setSeries] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        let cancelled = false;
        (async () => {
            try {
                const res = await contentService.getContentsByType('Series');
                if (!cancelled) setSeries(res || []);
            } finally {
                if (!cancelled) setLoading(false);
            }
        })();
        return () => { cancelled = true; };
    }, []);

    return (
        <PageLayout>
            <div className="px-6 pt-8">
                <h1 className="text-white text-3xl font-bold mb-6">All Series</h1>
                {loading ? (
                    <div className="text-white">Loading...</div>
                ) : (
                    <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-4">
                        {series.map(item => (
                            <ContentCard key={item.id} content={item} hoverEffect />
                        ))}
                    </div>
                )}
            </div>
        </PageLayout>
    );
}

import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import PageLayout from '../components/layout/PageLayout';
import { contentService } from '../services/api';
import ContentCard from '../components/browse/ContentCard';

export default function MoviesListPage() {
    const [movies, setMovies] = useState([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        let cancelled = false;
        (async () => {
            try {
                const data = await contentService.getContentsByType('Movie');
                if (!cancelled) setMovies(Array.isArray(data) ? data : []);
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
                <h1 className="text-2xl mb-4">Movies</h1>
                {!movies.length ? (
                    <div className="opacity-80">No movies found.</div>
                ) : (
                    <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-4">
                        {movies.map(item => (
                            <ContentCard key={item.id} content={item} hoverEffect />
                        ))}
                    </div>
                )}
            </div>
        </PageLayout>
    );
}

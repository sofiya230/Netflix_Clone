import { useEffect, useMemo, useState } from 'react';
import { useLocation } from 'react-router-dom';
import PageLayout from '../components/layout/PageLayout';
import HeroBanner from '../components/browse/HeroBanner';
import ContentRow from '../components/browse/ContentRow';
import { useAuth } from '../contexts/AuthContext';
import { contentService } from '../services/api';
import RequireProfile from '../components/auth/RequireProfile';

function useQuery() {
    const { search } = useLocation();
    return useMemo(() => new URLSearchParams(search), [search]);
}

export default function Browse() {
    const qs = useQuery();
    const { currentProfile } = useAuth();

    const query = (qs.get('query') || '').trim();

    const [featuredContent, setFeaturedContent] = useState(null);
    const [contents, setContents] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        let cancelled = false;

        (async () => {
            setLoading(true);
            try {
                let data = [];
                if (query) {
                    data = await contentService.searchContent(query);
                } else {
                    data = await contentService.getTrendingContent();
                }

                if (!cancelled) {
                    const list = Array.isArray(data) ? data : [];
                    setContents(list);
                    setFeaturedContent(list.length ? list[Math.floor(Math.random() * list.length)] : null);
                }
                    } catch (err) {
        } finally {
                if (!cancelled) setLoading(false);
            }
        })();

        return () => { cancelled = true; };
    }, [query, currentProfile]);

    const title = query ? `Search Results${query ? ` for “${query}”` : ''}` : 'Trending Now';

    if (loading) {
        return (
            <PageLayout>
                <div className="flex items-center justify-center h-screen">
                    <div className="w-16 h-16 border-t-4 border-red-600 border-solid rounded-full animate-spin"></div>
                </div>
            </PageLayout>
        );
    }

    return (
        <RequireProfile>
            <PageLayout transparentNav={false}>
                {featuredContent && <HeroBanner content={featuredContent} />}
                <div className="px-4 md:px-16 relative z-10 pb-16">
                    <ContentRow title={title} contents={contents} />
                </div>
            </PageLayout>
        </RequireProfile>
    );
}

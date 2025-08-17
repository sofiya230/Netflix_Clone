import { useEffect, useState } from 'react';
import { watchHistoryService } from "../../services/api";
import { useAuth } from "../../contexts/AuthContext";
import ContentCard from "./ContentCard";

function ContinueWatchingRow() {
    const { currentProfile } = useAuth();
    const [watchHistory, setWatchHistory] = useState([]);

    useEffect(() => {
        if (!currentProfile?.id) return;

        const fetchHistory = async () => {
            try {
                const response = await watchHistoryService.getByProfile(currentProfile.id);
                setWatchHistory(response);
                    } catch (err) {
        }
        };

        fetchHistory();
    }, [currentProfile]);

    if (!currentProfile || watchHistory.length === 0) return null;

    return (
        <section className="mt-8 px-4">
            <h2 className="text-white text-xl font-bold mb-4">Continue Watching</h2>
            <div className="flex overflow-x-auto gap-4 pb-2">
                {watchHistory.map((item) => (
                    <ContentCard key={item.id} content={item} />
                ))}
            </div>
        </section>
    );
}

export default ContinueWatchingRow;

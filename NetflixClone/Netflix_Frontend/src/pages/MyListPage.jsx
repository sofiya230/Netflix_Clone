import { useEffect, useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { myListService } from '../services/api';
import ContentCard from '../components/browse/ContentCard';

function MyListPage() {
    const { currentProfile, loading, isProfileSelected } = useAuth();
    const [myList, setMyList] = useState([]);
    const [isLoading, setIsLoading] = useState(true);

    if (!loading && !isProfileSelected) {
        return (
            <div className="p-4">
                <h1 className="text-white text-2xl font-bold mb-6">My List</h1>
                <div className="text-center py-16">
                    <div className="text-6xl mb-4">👤</div>
                    <h2 className="text-white text-xl font-semibold mb-4">Profile Required</h2>
                    <p className="text-gray-400 mb-6">
                        Please select a profile to view your My List.
                    </p>
                    <a 
                        href="/profiles" 
                        className="bg-red-600 text-white px-6 py-3 rounded-lg hover:bg-red-700 transition-colors"
                    >
                        Select Profile
                    </a>
                </div>
            </div>
        );
    }

    useEffect(() => {
        const fetchMyList = async () => {
            if (!currentProfile?.id) return;

            setIsLoading(true);

            try {
                const response = await myListService.getMyList(currentProfile.id);
                setMyList(response || []);
            } catch (error) {

                            setMyList([]);
                alert(`Failed to load My List: ${error.message || 'Unknown error'}`);
            } finally {
                setIsLoading(false);
            }
        };

        fetchMyList();
    }, [currentProfile?.id]);

    if (loading) {
        return <p className="text-white p-4">Loading...</p>;
    }

    return (
        <div className="p-4">
            <h1 className="text-white text-2xl font-bold mb-6">My List</h1>
            
            {isLoading ? (
                <p className="text-gray-300">Loading your saved content...</p>
            ) : myList.length === 0 ? (
                <p className="text-gray-300">Your list is empty.</p>
            ) : (
                <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
                    {myList.map((item) => (
                        <ContentCard key={item.id} content={item.content} />
                    ))}
                </div>
            )}
        </div>
    );
}

export default MyListPage;

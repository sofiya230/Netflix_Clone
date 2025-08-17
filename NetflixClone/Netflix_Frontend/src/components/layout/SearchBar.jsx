import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

function SearchBar() {
    const [query, setQuery] = useState('');
    const navigate = useNavigate();

    const handleSubmit = (e) => {
        e.preventDefault();

        if (query.trim()) {
            navigate(`/browse?category=search&query=${encodeURIComponent(query.trim())}`);
            setQuery('');
        }
    };

    return (
        <form onSubmit={handleSubmit} className="flex items-center">
            <input
                type="text"
                placeholder="Search movies, shows..."
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                className="bg-gray-800 text-white rounded px-3 py-1 text-sm focus:outline-none focus:ring focus:ring-red-500"
            />
            <button
                type="submit"
                className="ml-2 text-white text-sm bg-red-600 px-3 py-1 rounded hover:bg-red-700"
            >
                Search
            </button>
        </form>
    );
}

export default SearchBar;

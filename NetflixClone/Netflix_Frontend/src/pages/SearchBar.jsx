
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

export default function SearchBar() {
    const [text, setText] = useState('');
    const navigate = useNavigate();

    const onSubmit = (e) => {
        e.preventDefault();
        const q = text.trim();
        if (!q) return;
        navigate(`/browse?query=${encodeURIComponent(q)}`);
    };

    return (
        <form onSubmit={onSubmit} className="flex items-center gap-2">
            <input
                value={text}
                onChange={(e) => setText(e.target.value)}
                placeholder="Search movies, shows..."
                className="bg-neutral-800 text-white px-3 py-2 rounded outline-none"
                aria-label="Search"
            />
            <button type="submit" className="bg-red-600 text-white px-3 py-2 rounded">
                Search
            </button>
        </form>
    );
}

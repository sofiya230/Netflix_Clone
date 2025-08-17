import { useState, useRef } from 'react';
import ContentCard from './ContentCard';

function ContentRow({ title, contents }) {
    const [showLeftArrow, setShowLeftArrow] = useState(false);
    const [showRightArrow, setShowRightArrow] = useState(true);
    const rowRef = useRef(null);

    const handleScroll = () => {
        if (rowRef.current) {
            const { scrollLeft, scrollWidth, clientWidth } = rowRef.current;
            setShowLeftArrow(scrollLeft > 0);
            setShowRightArrow(scrollLeft < scrollWidth - clientWidth - 10);
        }
    };

    const scroll = (direction) => {
        if (rowRef.current) {
            const scrollAmount = rowRef.current.clientWidth * 0.75;
            rowRef.current.scrollBy({
                left: direction === 'left' ? -scrollAmount : scrollAmount,
                behavior: 'smooth'
            });
        }
    };

    return (
        <div className="my-8 relative">
            <h2 className="text-white text-xl md:text-2xl font-bold mb-4 px-4 md:px-0">{title}</h2>

            <div className="relative group">
                {showLeftArrow && (
                    <button
                        className="absolute left-0 top-0 bottom-0 z-10 bg-black bg-opacity-50 text-white flex items-center justify-center w-12 opacity-0 group-hover:opacity-100 transition-opacity"
                        onClick={() => scroll('left')}
                    >
                        <svg xmlns="http://www.w3.org/2000/svg" className="h-8 w-8" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
                        </svg>
                    </button>
                )}

                <div
                    ref={rowRef}
                    className="flex space-x-2 overflow-x-auto scrollbar-hide px-4 md:px-0"
                    onScroll={handleScroll}
                    style={{ scrollbarWidth: 'none', msOverflowStyle: 'none' }}
                >
                    {contents.map((content) => (
                        <div key={content.id} className="flex-shrink-0 w-[180px] md:w-[220px]">
                            <ContentCard content={content} />
                        </div>
                    ))}
                </div>

                {showRightArrow && (
                    <button
                        className="absolute right-0 top-0 bottom-0 z-10 bg-black bg-opacity-50 text-white flex items-center justify-center w-12 opacity-0 group-hover:opacity-100 transition-opacity"
                        onClick={() => scroll('right')}
                    >
                        <svg xmlns="http://www.w3.org/2000/svg" className="h-8 w-8" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                        </svg>
                    </button>
                )}
            </div>
        </div>
    );
}

export default ContentRow;

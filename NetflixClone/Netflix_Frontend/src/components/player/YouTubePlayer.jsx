import React from "react";

const YouTubePlayer = ({ videoId, title }) => {
    if (!videoId) return null;

    return (
        <div className="w-full max-w-4xl mx-auto aspect-video my-8 rounded-xl overflow-hidden shadow-lg border border-gray-800">
            <iframe
                className="w-full h-full"
                src={`https://www.youtube.com/embed/${videoId}?autoplay=1&rel=0&showinfo=0&modestbranding=1`}
                title={title || "YouTube video player"}
                frameBorder="0"
                allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                allowFullScreen
                style={{ minHeight: 360, background: "#000" }}
            />
        </div>
    );
};

export default YouTubePlayer;

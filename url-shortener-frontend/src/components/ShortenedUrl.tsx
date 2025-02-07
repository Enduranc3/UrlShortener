import React from "react";

interface ShortenedUrlProps {
    baseUrl: string;
    shortUrl: string;
}

const ShortenedUrl: React.FC<ShortenedUrlProps> = ({baseUrl, shortUrl}) => {
    const fullUrl = `${baseUrl}/${shortUrl}`;
    return (
        <div>
            <p>
                Shortened URL: <a href={fullUrl}>{fullUrl}</a>
            </p>
        </div>
    );
};

export default ShortenedUrl;

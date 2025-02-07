import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { Card } from "react-bootstrap";
import UrlService from "../services/UrlService";
import { useAuth } from "../contexts/AuthContext"; // Add this import

interface UrlInfo {
  shortUrl: string;
  originalUrl: string;
  description: string;
  createdByUserId: number;
  createdDate: string;
}

const UrlInfoView = () => {
  const { shortUrl } = useParams<{ shortUrl: string }>();
  const [urlInfo, setUrlInfo] = useState<UrlInfo | null>(null);
  const [error, setError] = useState("");
  const { isAdmin } = useAuth(); // Add this line

  useEffect(() => {
    const fetchUrlInfo = async () => {
      try {
        if (shortUrl) {
          const urlService = new UrlService();
          const data = await urlService.getUrlInfo(shortUrl);
          setUrlInfo(data);
        }
      } catch (error: any) {
        setError("Failed to fetch URL information: " + error.message);
      }
    };

    fetchUrlInfo();
  }, [shortUrl]);

  if (error) return <div className="alert alert-danger">{error}</div>;
  if (!urlInfo) return <div>Loading...</div>;

  return (
    <div className="container">
      <h2>URL Information</h2>
      <Card>
        <Card.Body>
          <Card.Text>Original URL: {urlInfo.originalUrl}</Card.Text>
          <Card.Text>Short URL: {urlInfo.shortUrl}</Card.Text>
          <Card.Text>
            Created Date: {new Date(urlInfo.createdDate).toLocaleString()}
          </Card.Text>
          <Card.Text>Description: {urlInfo.description}</Card.Text>
          {isAdmin && (
            <Card.Text>Created By User ID: {urlInfo.createdByUserId}</Card.Text>
          )}
        </Card.Body>
      </Card>
    </div>
  );
};

export default UrlInfoView;

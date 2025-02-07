import React, { useState, useEffect, useCallback } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import { Table, Form, Button } from "react-bootstrap";
import UrlService from "../services/UrlService";

interface UrlData {
  shortUrl: string;
  originalUrl: string;
  description: string;
  createdByUserId: number;
  createdDate: string;
}

const UrlTableView = () => {
  const [urls, setUrls] = useState<UrlData[]>([]);
  const [newUrl, setNewUrl] = useState("");
  const [description, setDescription] = useState("");
  const [error, setError] = useState("");
  const { user, isAdmin } = useAuth();
  const urlService = React.useMemo(() => new UrlService(), []);
  const [shortenedUrls, setShortenedUrls] = useState<{ [key: string]: string }>(
    {}
  );
  const baseUrl = "https://mydomain.com/";

  const fetchUrls = useCallback(async () => {
    try {
      const data = isAdmin
        ? await urlService.getAllUrls()
        : await urlService.getMyUrls();
      setUrls(data);
    } catch (error) {
      setError("Failed to fetch URLs");
    }
  }, [urlService, isAdmin]);

  const shorten = async (e: any) => {
    e.preventDefault();
    setError(""); // Clear previous errors
    try {
      const shortened = await urlService.shortenUrl({
        originalUrl: newUrl,
        description: description,
      });
      setShortenedUrls((prev) => ({ ...prev, [newUrl]: shortened.shortUrl }));
      setNewUrl("");
      setDescription("");
      fetchUrls();
    } catch (error: any) {
      console.log(error.response.data);
      setError(error.response.data);
    }
  };

  const handleDelete = async (shortUrlIdentifier: string) => {
    try {
      await urlService.deleteUrl(shortUrlIdentifier);
      fetchUrls();
    } catch (error: any) {
      setError(error.response.data);
    }
  };

  useEffect(() => {
    setError("");
    fetchUrls();
  }, [fetchUrls]);

  return (
    <div className="container">
      <h2>URL Shortener</h2>
      {error && <div className="alert alert-danger">{error}</div>}

      {user && (
        <Form onSubmit={shorten}>
          <Form.Control
            type="url"
            placeholder="Enter URL to shorten"
            className="mr-sm-2 mb-3"
            value={newUrl}
            onChange={(e) => setNewUrl(e.target.value)}
          />
          <Form.Control
            type="text"
            placeholder="Enter URL description"
            className="mr-sm-2 mb-3"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
          <Button variant="primary" className="mt-3 mb-3" type="submit">
            Shorten URL
          </Button>
        </Form>
      )}
      {newUrl && shortenedUrls[newUrl] && (
        <p>
          Shortened URL:
          <a href={`${baseUrl}${shortenedUrls[newUrl]}`}>
            {`${baseUrl}${shortenedUrls[newUrl]}`}
          </a>
        </p>
      )}

      <Table striped bordered hover>
        <thead>
          <tr>
            <th>Original URL</th>
            <th>Short URL</th>
            {isAdmin && <th>Created By User ID</th>}
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {urls.map((url) => {
            const shortUrlIdentifier = url.shortUrl.replace(
              "mydomain.com/",
              ""
            );
            return (
              <tr key={url.shortUrl}>
                <td>{url.originalUrl}</td>
                <td>{url.shortUrl}</td>
                {isAdmin && <td>{url.createdByUserId}</td>}
                <td>
                  {user && (
                    <div
                      style={{
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "space-between",
                      }}
                    >
                      <Link to={`/urls/${shortUrlIdentifier}`}>
                        View Details
                      </Link>
                      <Button
                        className="btn btn-danger"
                        onClick={() => handleDelete(shortUrlIdentifier)}
                      >
                        Delete
                      </Button>
                    </div>
                  )}
                </td>
              </tr>
            );
          })}
        </tbody>
      </Table>
    </div>
  );
};

export default UrlTableView;

import React, {useEffect, useState} from "react";
import {useAuth} from "../contexts/AuthContext";
import {Button, Form} from "react-bootstrap";

const AboutView = () => {
    const [content, setContent] = useState("");
    const [isEditing, setIsEditing] = useState(false);
    const {isAdmin} = useAuth();

    useEffect(() => {
        fetchContent();
    }, []);

    const fetchContent = async () => {
        setContent(
            "URL Shortener uses a base62 encoding algorithm to generate short URLs..."
        );
    };

    return (
        <div className="container">
            <h2>About URL Shortener</h2>
            {isEditing && isAdmin ? (
                <Form.Control
                    as="textarea"
                    rows={3}
                    value={content}
                    onChange={(e) => setContent(e.target.value)}
                />
            ) : (
                <p>{content}</p>
            )}
            {isAdmin && (
                <Button onClick={() => setIsEditing(!isEditing)}>
                    {isEditing ? "Save" : "Edit"}
                </Button>
            )}
        </div>
    );
};

export default AboutView;

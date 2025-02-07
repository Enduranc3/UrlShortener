import React, {useState} from "react";
import {useNavigate} from "react-router-dom";
import {Button, Form} from "react-bootstrap";
import AuthService from "../services/AuthService";
import {useAuth} from "../contexts/AuthContext";

const RegistrationView = () => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [username, setUserame] = useState("");
    const [error, setError] = useState("");
    const navigate = useNavigate();
    const authService = new AuthService();
    const {login} = useAuth();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            const emailRegistered = await authService.register({
                email,
                password,
                username,
            });
            await login(emailRegistered, password);
            navigate("/urls");
        } catch (error: any) {
            setError(error.message || "Registration failed");
        }
    };

    return (
        <div className="container">
            <h2>Register</h2>
            {error && <div className="alert alert-danger">{error}</div>}
            <Form onSubmit={handleSubmit}>
                <Form.Group controlId="formBasicFirstName">
                    <Form.Label>Username:</Form.Label>
                    <Form.Control
                        type="text"
                        placeholder="Enter username"
                        value={username}
                        onChange={(e) => setUserame(e.target.value)}
                        required
                    />
                </Form.Group>

                <Form.Group controlId="formBasicEmail">
                    <Form.Label>Email:</Form.Label>
                    <Form.Control
                        type="email"
                        placeholder="Enter email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                </Form.Group>

                <Form.Group controlId="formBasicPassword">
                    <Form.Label>Password:</Form.Label>
                    <Form.Control
                        type="password"
                        placeholder="Password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </Form.Group>

                <Button variant="primary" type="submit" className="mt-3">
                    Register
                </Button>
            </Form>
        </div>
    );
};

export default RegistrationView;

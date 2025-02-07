import React from "react";
import {Link, Outlet} from "react-router-dom";
import {useAuth} from "../contexts/AuthContext";
import {Button, Nav, Navbar} from "react-bootstrap";

const Layout = () => {
    const {user, logout} = useAuth();

    return (
        <div>
            <Navbar bg="grey" expand="lg">
                <Navbar.Brand style={{marginLeft: 10}}>URL Shortener</Navbar.Brand>
                <Navbar.Toggle aria-controls="basic-navbar-nav"/>
                <Navbar.Collapse id="basic-navbar-nav" className="ms-auto">
                    <Nav className="mr-auto">
                        {user ? (
                            <>
                                <Nav.Link as={Link} to="/urls">
                                    URLs
                                </Nav.Link>
                                <Nav.Link as={Link} to="/about">
                                    About
                                </Nav.Link>
                                <Button
                                    variant="outline-danger"
                                    onClick={() => {
                                        logout();
                                        window.location.href = "/login";
                                    }}
                                >
                                    Logout
                                </Button>
                            </>
                        ) : (
                            <>
                                <Nav.Link as={Link} to="/about">
                                    About
                                </Nav.Link>
                                <Nav.Link as={Link} to="/login">
                                    Login
                                </Nav.Link>
                                <Nav.Link as={Link} to="/register">
                                    Register
                                </Nav.Link>
                            </>
                        )}
                    </Nav>
                </Navbar.Collapse>
            </Navbar>
            <hr className="mt-0 mb-3"/>
            <main className="container">
                <Outlet/>
            </main>
        </div>
    );
};

export default Layout;

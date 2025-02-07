// @ts-ignore
import React from "react";
import {BrowserRouter, Navigate, Route, Routes} from "react-router-dom";
import {AuthProvider} from "./contexts/AuthContext";
import Layout from "./components/Layout";
import LoginView from "./views/LoginView";
import UrlTableView from "./views/UrlTableView";
import UrlInfoView from "./views/UrlInfoView";
import AboutView from "./views/AboutView";
import ProtectedRoute from "./components/ProtectedRoute";
import "bootstrap/dist/css/bootstrap.min.css";
import RegistrationView from "./views/RegistrationView";

function App() {
    return (
        <AuthProvider>
            <BrowserRouter>
                <Routes>
                    <Route path="/" element={<Layout/>}>
                        <Route index element={<Navigate to="/urls"/>}/>
                        <Route path="login" element={<LoginView/>}/>
                        <Route path="register" element={<RegistrationView/>}/>
                        <Route path="urls" element={<UrlTableView/>}/>
                        <Route
                            path="urls/:shortUrl"
                            element={
                                <ProtectedRoute>
                                    <UrlInfoView/>
                                </ProtectedRoute>
                            }
                        />
                        <Route path="about" element={<AboutView/>}/>
                    </Route>
                </Routes>
            </BrowserRouter>
        </AuthProvider>
    );
}

export default App;

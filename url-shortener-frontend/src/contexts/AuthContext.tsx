import React, { createContext, useContext, useEffect, useState } from "react";
import AuthService from "../services/AuthService";
import TokenService from "../services/TokenService";

interface User {
  id: string;
  email: string;
  username: string;
  role: string; // Add this line
}

interface AuthContextType {
  user: User | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
  isAdmin: boolean;
}

const AuthContext = createContext<AuthContextType | null>(null);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [user, setUser] = useState<User | null>(null);
  const [isAdmin, setIsAdmin] = useState(false);
  const [loading, setLoading] = useState(true);
  const authService = React.useMemo(() => new AuthService(), []);

  useEffect(() => {
    const initializeAuth = async () => {
      if (TokenService.hasToken()) {
        try {
          const userData = await authService.getCurrentUser();
          setUser(userData);
          setIsAdmin(userData.role === "Admin");
        } catch (error) {
          TokenService.removeToken();
          setUser(null);
          setIsAdmin(false);
        }
      }
      setLoading(false);
    };

    initializeAuth();
  }, [authService]);

  const login = async (email: string, password: string) => {
    try {
      const token = await authService.login({ email, password });
      TokenService.saveToken(token);
      const user = await authService.getCurrentUser();
      setUser(user);
      setIsAdmin(user.role === "Admin");
    } catch (error: any) {
      throw new Error(error.response.data || error.response.statusText);
    }
  };

  const logout = () => {
    authService.logout();
    setUser(null);
    setIsAdmin(false);
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  return (
    <AuthContext.Provider value={{ user, login, logout, isAdmin }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};

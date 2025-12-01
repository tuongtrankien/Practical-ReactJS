import React, { createContext, useContext, useEffect, useState } from "react";
import { api } from "../api/axios";
import { User } from "../models/User";
import { useNavigate } from "react-router-dom";
type Ctx = {
  user: User;
  login: (e: string, p: string) => Promise<void>;
  logout: () => void;
};

const AuthContext = createContext<Ctx>(null!);
export const useAuth = () => useContext(AuthContext);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({children}) => {

    const [user, setUser] = useState<User>(null);
    const navigate = useNavigate();

    // On mount, attempt to get current user from server using cookie-based auth
    useEffect(() => {
        let mounted = true;
        const fetchMe = async () => {
            try {
                const { data } = await api.get('/auth/me');
                if (!mounted) return;
                setUser({ email: data.email, roles: data.roles });
            } catch (err) {
                if (!mounted) return;
                setUser(null);
            }
        };
        fetchMe();
        return () => { mounted = false; };
    }, []);


    const login = async (email: string, password: string) => {
        await api.post("/auth/login", { email, password });
        const { data } = await api.get('/auth/me');
        setUser({ email: data.email, roles: data.roles });
        navigate("/");
    };

    const logout = () => {
        api.post('/auth/logout').catch(() => {});
        setUser(null);
        navigate("/");
    };

    return (
        <AuthContext.Provider value={{ user, login, logout }}>{children}</AuthContext.Provider>
    );
};

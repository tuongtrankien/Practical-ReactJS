import React, { createContext, useContext, useEffect, useState } from "react";
import { api, setAccessToken } from "../api/axios";
import { isTokenExpired, parseJwt } from "../utils/helper";
import { User } from "../models/User";
import { useNavigate } from "react-router-dom";
import { jwtDecode } from "jwt-decode";


type Ctx = {
  user: User;
  login: (e: string, p: string) => Promise<void>;
  logout: () => void;
};

const AuthContext = createContext<Ctx>(null!);
export const useAuth = () => useContext(AuthContext);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({children}) => {

    const getInitialUser = (): User => {
        const accessToken = localStorage.getItem("accessToken");
        if (!accessToken) return null;
        const isExpired = isTokenExpired(accessToken);
        if (isExpired) return null;
        return parseJwt(accessToken);
    };

    const [user, setUser] = useState<User>(getInitialUser);
    const navigate = useNavigate();

    useEffect(() => {
        const token = localStorage.getItem("accessToken");
        if (!token || isTokenExpired(token)) {
            logout();
            return;
        }

        const { exp } = jwtDecode<{ exp: number }>(token);
        const expiryTime = exp * 1000;
        const now = Date.now();

        const timeout = setTimeout(() => {
            logout();
        }, expiryTime - now);

        return () => clearTimeout(timeout);
    }, [user]);


    const login = async (email: string, password: string) => {
        const { data } = await api.post("/auth/login", { email, password });
        setAccessToken(data.accessToken);
        setUser(parseJwt(data.accessToken));
        navigate("/");
    };

    const logout = () => {
        setAccessToken(null);
        setUser(null);
        navigate("/");
    };

    return (
        <AuthContext.Provider value={{ user, login, logout }}>{children}</AuthContext.Provider>
    );
};

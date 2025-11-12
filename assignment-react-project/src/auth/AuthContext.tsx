import React, { createContext, useContext, useState } from "react";
import { api, setAccessToken } from "../api/axios";
import { parseJwt } from "../utils/helper";
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

    const getInitialUser = (): User => {
        const accessToken = localStorage.getItem("accessToken");
        if (!accessToken) return null;
        return parseJwt(accessToken);
    };

    const [user, setUser] = useState<User>(getInitialUser);
    const navigate = useNavigate();

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

import axios from "axios";

export const api = axios.create({
  baseURL: 'https://localhost:7050/api'
});

export const setAccessToken = (t: string | null) => {
  if (t) localStorage.setItem("accessToken", t);
  else localStorage.removeItem("accessToken");
};

// Add JWT token to request headers
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("accessToken");
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

// 401 -> redirect to login
api.interceptors.response.use(
  (res) => res,
  (error) => {
    if (error.response?.status === 401) {
      console.log("Unauthorized! Redirecting to login...");
      localStorage.removeItem("accessToken");
    //   window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

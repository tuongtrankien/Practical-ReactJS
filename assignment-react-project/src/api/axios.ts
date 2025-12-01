import axios from "axios";

export const api = axios.create({
  baseURL: 'https://localhost:7050/api',
  withCredentials: true // send cookies (jwt) with requests
});

// Response interceptor: optionally handle global 401s
api.interceptors.response.use(
  (res) => res,
  (error) => {
    if (error.response?.status === 401) {
      console.log("Unauthorized (401)");
    }
    return Promise.reject(error);
  }
);

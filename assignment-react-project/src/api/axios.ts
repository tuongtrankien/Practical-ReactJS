import axios from "axios";

export const api = axios.create({
  baseURL: 'https://localhost:7050/api',
  withCredentials: true // send cookies (jwt) with requests
});

// Response interceptor: optionally handle global 401s
api.interceptors.response.use(
  (res) => res,
  async (error) => {
    const originalRequest = error.config;
    if (error.response?.status === 401 && !originalRequest?._retry) {
      // Try to refresh the access token using the refresh token cookie
      try {
        originalRequest._retry = true;
        await api.post('/auth/refresh');
        return api(originalRequest);
      } catch (err) {
        // Refresh failed -> fallthrough to reject and let app handle (e.g., redirect to login)
        console.log('Refresh failed', err);
      }
    }
    return Promise.reject(error);
  }
);

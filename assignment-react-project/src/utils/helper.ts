import { jwtDecode } from "jwt-decode";

export const parseJwt = (token: string) => {
  try {
    const payload: any = jwtDecode(token);
    const roles = Array.isArray(payload["role"])
      ? payload["role"]
      : payload["role"]
      ? [payload["role"]]
      : [];
    return { email: payload["email"] ?? "", roles };
  } catch {
    return { email: "", roles: [] };
  }
};

export const isTokenExpired = (token: string): boolean => {
  try {
    const decoded: { exp?: number } = jwtDecode(token);

    if (!decoded.exp || typeof decoded.exp !== 'number') {
      return true;
    }

    return Date.now() >= decoded.exp * 1000;
  } catch (e) {
    return true;
  }
}
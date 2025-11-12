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
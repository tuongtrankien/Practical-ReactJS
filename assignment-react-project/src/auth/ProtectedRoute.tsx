import { Navigate } from "react-router-dom";
import { useAuth } from "./AuthContext";

export const ProtectedRoute: React.FC<{children: React.ReactNode, roles?: string[]}> =
({children, roles}) => {
  const { user } = useAuth();
  if (!user) return <Navigate to="/login" replace />;
  if (roles?.length && !roles.some(r => user.roles.includes(r))) {
    return <Navigate to="/403" replace />;
  }
  return children;
};
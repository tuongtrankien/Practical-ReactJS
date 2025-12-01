import { Route, Routes } from "react-router-dom";
import LoginPage from "../pages/auth/LoginPage";
import { ProtectedRoute } from "../auth/ProtectedRoute";
import HomePage from "../pages/HomePage";
import ProductPage from "../pages/product/ProductPage";

const AppRoutes = () => {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />

      <Route
        path="/"
        element={<HomePage />}
      />

      <Route path="/products" element={
        <ProtectedRoute>
            <ProductPage />
        </ProtectedRoute>
      }
      />

      <Route path="*" element={<div>404</div>} />
    </Routes>
  );
};

export default AppRoutes;

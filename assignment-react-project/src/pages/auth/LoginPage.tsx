import React, { FormEvent, useState } from "react";
import { useAuth } from "../../auth/AuthContext";
import TextField from "@mui/material/TextField";
import Button from "@mui/material/Button";
import Box from "@mui/material/Box";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import Typography from "@mui/material/Typography";
import { api } from "../../api/axios";

const LoginPage = () => {
  const { login } = useAuth();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [switchMode, setSwitchMode] = useState({
    isLogin: true,
    isRegister: false
  });

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    try {
      setLoading(true);
      if (switchMode.isLogin) {
        await login(email, password);
      }
      else {
        await api.post("/auth/register", { email, password, firstName, lastName });
        await login(email, password);
      }
    } catch (error) {
      console.log(error);
      setError("An error occurred during processing.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box
      sx={{
        height: "100vh",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        bgcolor: "#f5f5f5",
        px: 2
      }}
    >
      <Card sx={{ width: 380, p: 2, boxShadow: 3, borderRadius: 2 }}>
        <CardContent>
          <Typography
            variant="h5"
            textAlign="center"
            sx={{ fontWeight: 600, mb: 3 }}
          >
            {switchMode.isLogin ? "Login" : "Register"}
          </Typography>
          {error && (
            <Typography color="error" sx={{ mb: 2 }}>
              {error}
            </Typography>
          )}

          <form onSubmit={onSubmit}>
            <TextField
              fullWidth
              label="Email"
              variant="outlined"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              sx={{ mb: 2 }}
            />

            {switchMode.isRegister && (
              <>
                <TextField
                  fullWidth
                  label="First Name"
                  variant="outlined"
                  value={firstName}
                  onChange={(e) => setFirstName(e.target.value)}
                  sx={{ mb: 2 }}
                />
                <TextField
                  fullWidth
                  label="Last Name"
                  variant="outlined"
                  value={lastName}
                  onChange={(e) => setLastName(e.target.value)}
                  sx={{ mb: 2 }}
                />
              </>
            )}

            <TextField
              fullWidth
              label="Password"
              type="password"
              variant="outlined"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              sx={{ mb: 3 }}
            />

            <Button
              fullWidth
              variant="contained"
              type="submit"
              size="large"
              sx={{ py: 1.2 }}
              loading={loading}
            >
              {switchMode.isLogin ? "Login" : "Register"}
            </Button>
          </form>
          <Button
            fullWidth
            variant="outlined"
            size="large"
            sx={{ mt: 2, py: 1.2 }}
            onClick={() => setSwitchMode({ isLogin: !switchMode.isLogin, isRegister: !switchMode.isRegister })}
          >
            Switch to {switchMode.isLogin ? "Register" : "Login"}
          </Button>
        </CardContent>
      </Card>
    </Box>
  );
};

export default LoginPage;

// Import Roboto font weights
import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';

import { BrowserRouter } from 'react-router-dom';
import { AuthProvider } from './auth/AuthContext';
import AppRoutes from './routes/AppRoutes';
import ResponsiveAppBar from './components/ResponsiveAppBar';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <ResponsiveAppBar />
        <AppRoutes />
      </AuthProvider>
    </BrowserRouter>
  )
}

export default App

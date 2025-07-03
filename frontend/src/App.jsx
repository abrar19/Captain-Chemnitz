import { BrowserRouter as Router, Routes, Route, useLocation } from 'react-router-dom';
import MapView from './components/MapView/MapView';
import LocationDetails from './components/LocationDetails/LocationDetails';

import './App.css'
import Login from './components/Login/Login';
import ProtectedRoute from './ProtectedRoute';
import Register from './components/Register/Register';
import Favorites from './components/FavoritesPage/Favorites';
import EditProfile from './components/EditProfilePage/EditProfile';
import Header from './components/Header/Header';
import AdminDashboard from './components/AdminDashboard/AdminDashboard';
import AdminUserDashboard from "./components/AdminDashboard/AdminUserDashboard.jsx";
import AdminCultureSites from "./components/AdminDashboard/AdminCultureSites.jsx";

function AppWrapper() {
  const location = useLocation();

  // Hide header on login and register pages
  const hideHeaderPaths = ['/login', '/register', '/admin'];
  const shouldShowHeader = !hideHeaderPaths.includes(location.pathname);

  return (
    <>
      {shouldShowHeader && <Header />}

      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/" element={<MapView />} />
        <Route
          path="/favorites"
          element={
            <ProtectedRoute>
              <Favorites />
            </ProtectedRoute>
          }
        />
        <Route
          path="/profile"
          element={
            <ProtectedRoute>
              <EditProfile />
            </ProtectedRoute>
          }
        />
        <Route path="/location/:id" element={<LocationDetails />} />
        <Route path="/admin" element={<AdminDashboard />} />
          <Route path="/admin/users" element={<AdminUserDashboard />} />
          <Route path="/admin/culturalsites" element={<AdminCultureSites />} />
      </Routes>
    </>
  );
}

function App() {
  return (
    <Router>
      <AppWrapper />
    </Router>
  );
}

export default App;

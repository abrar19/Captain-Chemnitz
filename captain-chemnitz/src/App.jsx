import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import MapView from './components/MapView/MapView';
import LocationDetails from './components/LocationDetails/LocationDetails';

import './App.css'
import Login from './components/Login/Login';
import ProtectedRoute from './ProtectedRoute';
import Register from './components/Register/Register';
import Favorites from './components/FavoritesPage/Favorites';
import EditProfile from './components/EditProfilePage/EditProfile';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route
          path="/"
          element={
            <ProtectedRoute>
              <MapView />
            </ProtectedRoute>
          }
        />
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
      </Routes>
    </Router>
  );
}

export default App;

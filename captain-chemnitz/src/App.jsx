import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import MapView from './components/MapView/MapView';
import LocationDetails from './components/LocationDetails/LocationDetails';

import './App.css'
import Login from './components/Login/Login';
import ProtectedRoute from './ProtectedRoute';
import Register from './components/Register/Register';

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
        <Route path="/location/:id" element={<LocationDetails />} />
      </Routes>
    </Router>
  );
}

export default App;

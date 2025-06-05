import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import MapView from './components/MapView/MapView';
import LocationDetails from './components/LocationDetails/LocationDetails';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<MapView />} />
        <Route path="/location/:id" element={<LocationDetails />} />
      </Routes>
    </Router>
  );
}

export default App;

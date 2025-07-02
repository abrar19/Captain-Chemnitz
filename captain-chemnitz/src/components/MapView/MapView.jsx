import { useRef, useEffect, useState } from 'react'
import { Link, useNavigate} from 'react-router-dom';
import Sidebar from '../Sidebar/Sidebar';
import { useSelector, useDispatch } from 'react-redux';
import { setFeatures } from '../../uiSlice';

import mapboxgl from 'mapbox-gl'

import * as turf from '@turf/turf';

import 'mapbox-gl/dist/mapbox-gl.css';
import './mapView.css'

function MapView() {

  const navigate = useNavigate();

  const mapRef = useRef();
  const mapContainerRef = useRef();
  const markersRef = useRef([])
  const routeLayerIdRef = useRef(null);
  const isochroneLayerIdRef = useRef(null);



  const features = useSelector((state) => state.ui.features);  
  const searchTerm = useSelector((state) => state.ui.searchTerm);
  const dispatch = useDispatch();

  const [userLocation, setUserLocation] = useState(null);
  const [message, setMessage] = useState('');


  const extractCategory = (properties) => {
    // Order matters: check common keys from most to least specific
    if (properties.tourism) return properties.tourism;
    if (properties.amenity) return properties.amenity;
    if (properties.shop) return properties.shop;
    if (properties.leisure) return properties.leisure;
    // Add more keys as needed
    return 'unknown';
  };
  

  const getMarkerEmoji = (type) => {
    switch (type) {
      case 'museum':
        return '🏛️';
      case 'restaurant':
        return '🍽️';
      case 'cafe':
        return '☕';
      case 'hotel':
        return '🏨';
      case 'supermarket':
        return '🛒';
      case 'park':
      case 'playground':
        return '🌳';
      default:
        return '📍';
    }
  };
  

  //User location
  useEffect(() => {
    mapboxgl.accessToken = import.meta.env.VITE_MAPBOX_TOKEN;
    mapRef.current = new mapboxgl.Map({
      container: mapContainerRef.current,
      center: [12.92530, 50.83219],
      zoom: 13.00
    });

    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          const userLng = position.coords.longitude;
          const userLat = position.coords.latitude;
    
          setUserLocation([userLng, userLat]); // ✅ moved inside
    
          const userMarker = new mapboxgl.Marker({
            color: 'blue'
          })
            .setLngLat([userLng, userLat])
            .setPopup(new mapboxgl.Popup().setText('Your Location'))
            .addTo(mapRef.current);
    
          // center the map to user location
          mapRef.current.setCenter([userLng, userLat]);
        },
        (error) => {
          console.warn('Error fetching geolocation:', error.message);
        },
        { enableHighAccuracy: true }
      );
    }
    
    

  // Fetch the geojson
  fetch("/Chemnitz.geojson")
      .then((res) => res.json())
      .then((geojsonData) => {
        dispatch(setFeatures(geojsonData.features));
    });

    return () => {
      mapRef.current.remove()
    }
  }, [])

  // Filter logic
  const normalize = (str) =>
    str?.toLowerCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "");
  
  const filteredFeatures = features.filter((f) => {
    const name = normalize(f.properties?.name);
    const term = normalize(searchTerm);
    return name && name.includes(term);
  });

  //route removal logic for route cleanup on search cleanup
  const removeRoute = () => {
    const map = mapRef.current;
    const layerId = routeLayerIdRef.current;
  
    if (layerId && map.getLayer(layerId)) {
      map.removeLayer(layerId);
    }
    if (layerId && map.getSource(layerId)) {
      map.removeSource(layerId);
    }
  
    routeLayerIdRef.current = null;
  };
  

  // Update map markers when search changes
  useEffect(() => {

    if (!searchTerm.trim()) {
      // Remove all existing markers
      markersRef.current.forEach(marker => marker.remove());
      markersRef.current = [];
      removeRoute(); // If no markers will be shown, remove the route
      return;
    }


    // Remove previous markers
    markersRef.current.forEach(marker => marker.remove());
    markersRef.current = [];
    
    // Add filtered markers
    const newMarkers = [];
  
    filteredFeatures.forEach((feature) => {
      const { coordinates } = feature.geometry;
      const { name, website } = feature.properties;
      const category = extractCategory(feature.properties);
  
      const popup = new mapboxgl.Popup({ offset: 25 }).setHTML(
        `<strong>${name}</strong><br/><a href="${website}" target="_blank">Website</a>`
      );
  
      const el = document.createElement('div');
      el.className = 'custom-marker';
      el.textContent = getMarkerEmoji(category);
      el.style.fontSize = '1.25rem';
      el.style.cursor = 'pointer';
  
      const marker = new mapboxgl.Marker({ element: el })
        .setLngLat(coordinates)
        .setPopup(popup)
        .addTo(mapRef.current);
  
      newMarkers.push(marker);
    });
  
    markersRef.current = newMarkers;
  }, [filteredFeatures]);
  
  //Handle Isochrone (locations within 10 mins of distance)
  const handleShowIsochrone = async () => {
    if (!userLocation) {
      alert("User location not available");
      return;
    }
  
    const [lng, lat] = userLocation;
  
    const isochroneUrl = `https://api.mapbox.com/isochrone/v1/mapbox/driving-traffic/${lng},${lat}?contours_minutes=10&polygons=true&access_token=${import.meta.env.VITE_MAPBOX_TOKEN}`;
  
    try {
      const res = await fetch(isochroneUrl);
      const data = await res.json();
  
      const map = mapRef.current;
  
      // Remove existing isochrone if present
      if (isochroneLayerIdRef.current && map.getLayer(isochroneLayerIdRef.current)) {
        map.removeLayer(isochroneLayerIdRef.current);
        map.removeSource(isochroneLayerIdRef.current);
      }
  
      const newLayerId = `isochrone-${Date.now()}`;
  
      map.addSource(newLayerId, {
        type: 'geojson',
        data: data
      });
  
      map.addLayer({
        id: newLayerId,
        type: 'fill',
        source: newLayerId,
        layout: {},
        paint: {
          'fill-color': '#5a3fc0',
          'fill-opacity': 0.3
        }
      });
  
      isochroneLayerIdRef.current = newLayerId;
  
      // Zoom to isochrone bounds
      const bounds = new mapboxgl.LngLatBounds();
      data.features[0].geometry.coordinates[0].forEach(coord => bounds.extend(coord));
      map.fitBounds(bounds, { padding: 40 });

      // 🌟 Filter locations within isochrone
      const isochronePolygon = turf.polygon(data.features[0].geometry.coordinates);

      const locationsWithinIsochrone = features.filter(feature => {
        const point = turf.point(feature.geometry.coordinates);
        return turf.booleanPointInPolygon(point, isochronePolygon);
      });

      console.log("Locations within isochrone:", locationsWithinIsochrone);

      // Optional: Update your markers to only show these
      markersRef.current.forEach(marker => marker.remove());
      markersRef.current = [];

      const newMarkers = [];

      locationsWithinIsochrone.forEach((feature) => {
        const { coordinates } = feature.geometry;
        const { name, website } = feature.properties;
        const category = extractCategory(feature.properties);

        const popup = new mapboxgl.Popup({ offset: 25 }).setHTML(
          `<strong>${name}</strong><br/><a href="${website}" target="_blank">Website</a>`
        );

        const el = document.createElement('div');
        el.className = 'custom-marker';
        el.textContent = getMarkerEmoji(category);
        el.style.fontSize = '1.25rem';
        el.style.cursor = 'pointer';

        const marker = new mapboxgl.Marker({ element: el })
          .setLngLat(coordinates)
          .setPopup(popup)
          .addTo(map);

        newMarkers.push(marker);
      });

      markersRef.current = newMarkers;

  
    } catch (err) {
      console.error("Failed to fetch isochrone:", err);
      alert("Failed to fetch walking zone.");
    }
  };
  
  //Add to favorites
  const addToFavorites = async (place) => {
    const tokenData = localStorage.getItem('token');
  
    if (!tokenData) {
      console.error('User not authenticated');
      return;
    }
  
    try {
      const { token, userId } = JSON.parse(tokenData);
  
      const response = await fetch(`http://localhost:5029/api/user/${userId}/favorites`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}` // Optional if backend checks token
        },
        body: JSON.stringify({
          name: place.properties.name,
          locationId: place.id 
        })
      });
  
      if (response.ok) {
        setMessage(`Added ${place.properties.name} to favorites`);
      } else if (response.status === 409) {
        setMessage(`⚠️ "${place.properties.name}" is already in your favorites.`);
      } else {
        const errorText = await response.text();
        setMessage('Failed to add favorite:', errorText);
      }
    } catch (err) {
      setMessage('Error adding favorite:', err);
    }

    setTimeout(() => setMessage(''), 2000);

  };
  


  const handleShowDirections = async (destinationCoords) => {
    if (!userLocation) {
      alert("User location not available");
      return;
    }
  
    const [userLng, userLat] = userLocation;
    const [destLng, destLat] = destinationCoords;
  
    const directionsUrl = `https://api.mapbox.com/directions/v5/mapbox/walking/${userLng},${userLat};${destLng},${destLat}?geometries=geojson&access_token=${import.meta.env.VITE_MAPBOX_TOKEN}`;
  
    try {
      const res = await fetch(directionsUrl);
      const data = await res.json();
      const route = data.routes[0].geometry;
  
      const map = mapRef.current;
      removeRoute(); // ✅ clear existing route before adding new
  
      const newLayerId = `route-${Date.now()}`;
      map.addSource(newLayerId, {
        type: 'geojson',
        data: {
          type: 'Feature',
          geometry: route,
        }
      });
  
      map.addLayer({
        id: newLayerId,
        type: 'line',
        source: newLayerId,
        layout: {
          'line-join': 'round',
          'line-cap': 'round',
        },
        paint: {
          'line-color': '#3b82f6',
          'line-width': 4,
        }
      });
  
      routeLayerIdRef.current = newLayerId; 
  
      // Zoom to route bounds
      const coords = route.coordinates;
      const bounds = coords.reduce((b, coord) => b.extend(coord), new mapboxgl.LngLatBounds(coords[0], coords[0]));
      map.fitBounds(bounds, { padding: 60 });
  
    } catch (err) {
      console.error('Error fetching directions:', err);
      alert("Could not fetch directions.");
    }
  };

  //Logout
  const handleLogout = () => {
    localStorage.removeItem("token"); // or localStorage.clear() if you want to wipe all
    navigate("/login"); // Redirect to login page
  };
  
  

  return (
    <div className="app-container">
      {/* Sidebar */}
      <Sidebar 
        message={message}
        addToFavorites={addToFavorites}
        handleShowDirections={handleShowDirections}
        markersRef={markersRef}
        mapRef={mapRef}
        filteredFeatures={filteredFeatures}
      />
      
      {/* Floating Circular Dropdown Menu */}
      <div className="floating-menu">
        <input type="checkbox" id="menu-toggle" className="menu-toggle" />
        <label htmlFor="menu-toggle" className="menu-button">☰</label>
        <div className="menu-items">
          <Link to="/favorites" className="menu-item" title="Favorites">⭐</Link>
          <Link to="/profile" className="menu-item" title="Edit Profile">📝</Link>
          <button className="menu-item logout-button" onClick={handleLogout} title="Logout">🚪</button>
          <button className="menu-item" onClick={handleShowIsochrone} title="10-Min Walk Zone">🚶‍♂️ 10 Min Zone</button>
        </div>
      </div>

      {/* Map container */}
      <div id="map-container" ref={mapContainerRef} />

    </div>
  )
}

export default MapView

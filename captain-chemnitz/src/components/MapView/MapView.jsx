import { useRef, useEffect, useState } from 'react'
import { Link } from 'react-router-dom';

import mapboxgl from 'mapbox-gl'

import 'mapbox-gl/dist/mapbox-gl.css';
import './mapView.css'

function MapView() {

  const mapRef = useRef();
  const mapContainerRef = useRef();


  const [features, setFeatures] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [markerMap, setMarkerMap] = useState({});

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
    
          const userMarker = new mapboxgl.Marker({
            color: 'blue'
          })
            .setLngLat([userLng, userLat])
            .setPopup(new mapboxgl.Popup().setText('Your Location'))
            .addTo(mapRef.current);
    
          // Optional: center the map to user location
          mapRef.current.setCenter([userLng, userLat]);
        },
        (error) => {
          console.warn('Error fetching geolocation:', error.message);
        },
        { enableHighAccuracy: true }
      );
    }
    

  // Fetch the geojson
  fetch("/Chemnitz.geojson") // assuming it's in /public folder
  .then((res) => res.json())
  .then((geojsonData) => {
    setFeatures(geojsonData.features);
    const markers = {};

    geojsonData.features.forEach((feature) => {
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

      markers[feature.id] = marker;
    });

    setMarkerMap(markers); 
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


  return (
    <div className="app-container">
      {/* Sidebar */}
      <div className="sidebar-panel">
        <h3 className="sidebar-title">Locations</h3>
        <input
          type="text"
          placeholder="Search by name..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="search-input"
        />
        <ul className="location-list">
          {filteredFeatures.map((feature) => (
            <li 
              key={feature.id} 
              className="location-item"
              onClick={() => {
                const marker = markerMap[feature.id];
                if (marker) {
                  mapRef.current.flyTo({
                    center: feature.geometry.coordinates,
                    zoom: 15,
                    speed: 0.8,
                  });
                  marker.togglePopup(); // open popup
                }
              }}
            >
              <strong>{feature.properties.name}</strong><br />
              <a href={feature.properties.website} target="_blank" rel="noreferrer">Website</a>
              <Link to={`/location/${encodeURIComponent(feature.id)}`} className="details-link">View Details</Link>

            </li>
          ))}
        </ul>
      </div>

      {/* Map container */}
      <div id="map-container" ref={mapContainerRef} />
    </div>
  )
}

export default MapView

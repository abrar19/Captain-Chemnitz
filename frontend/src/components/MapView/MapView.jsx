import { useRef, useEffect, useState } from 'react'
import { Link, useNavigate} from 'react-router-dom';

import mapboxgl from 'mapbox-gl'

import 'mapbox-gl/dist/mapbox-gl.css';
import './mapView.css'
import {APIEndpoints} from "../../constants/APIEndpoints.js";
import AddReviewModal from "../AddReview/AddReviewModal.jsx";
import Button from "react-bootstrap/Button";
import Modal from "react-bootstrap/Modal";
import Form from "react-bootstrap/Form";
import ReviewViewerModal from "../ViewReviews/ReviewViewerModal.jsx";


function MapView() {

  const navigate = useNavigate();

  const mapRef = useRef();
  const mapContainerRef = useRef();
  const markersRef = useRef([])
  const routeLayerIdRef = useRef(null);

  const [favorites, setFavorites] = useState([]);
  const [showFavoritesOnly, setShowFavoritesOnly] = useState(false);
  const [showMuseumsOnly, setShowMuseumsOnly] = useState(false);
  const [showHotelsOnly, setShowHotelsOnly] = useState(false);
  const [showRestaurantsOnly, setShowRestaurantsOnly] = useState(false);
  const [features, setFeatures] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [markerMap, setMarkerMap] = useState({});
  const [userLocation, setUserLocation] = useState(null);
  const [message, setMessage] = useState('');






  const [showReviews, setShowReviews] = useState(false);






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
    
          setUserLocation([userLng, userLat]);
    
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

    if(isLoggedIn()) {
      var tokenData = localStorage.getItem('token');
      const { token } = JSON.parse(tokenData);
      fetch(APIEndpoints.getMyFavoriteSites, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}` // Optional if backend checks token
        }
      }).then(async res => {
        if (!res.ok) {
          throw new Error('Failed to fetch cultural sites');
        }
        var data= await res.json();
        setFavorites(data);
      });
    }
    
    

  // Fetch the geojson
    /*fetch(APIEndpoints.login) // it's in /public folder
        .then((res) => res.json())
        .then((geojsonData) => {
          setFeatures(geojsonData.features);
        });*/

     fetch(APIEndpoints.culturalSites, {
      method: 'GET',
    }).then(async res => {
       if (!res.ok) {
         throw new Error('Failed to fetch cultural sites');
       }
       var data= await res.json();
       setFeatures(data);
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
    const matchesSearch = name && name.includes(term);

    const isFavorite = favorites.some(fav => fav.culturalSite.culturalSiteId === f.culturalSiteId);
    const category = extractCategory(f.properties);

    const matchesMuseum = !showMuseumsOnly || category === 'museum';
    const matchesHotel = !showHotelsOnly || category === 'hotel';
    const matchesRestaurant = !showRestaurantsOnly || category === 'restaurant';

    return (
      matchesSearch &&
      (!showFavoritesOnly || isFavorite) &&
      matchesMuseum &&
      matchesHotel &&
      matchesRestaurant
    );
    
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

    // if (!searchTerm.trim()) {
    //   // Remove all existing markers
    //   markersRef.current.forEach(marker => marker.remove());
    //   markersRef.current = [];
    //   removeRoute(); // If no markers will be shown, remove the route
    //   return;
    // }


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


  const getAddress = (properties) => {
    const { addrStreet, addrHousenumber, addrPostcode, addrCity } = properties;
    return [addrStreet, addrHousenumber, addrPostcode, addrCity]
        .filter(Boolean)
        .join(', ');
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
  
      const response = await fetch(APIEndpoints.addFavoriteSite, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}` // Optional if backend checks token
        },
        body: JSON.stringify({
          culturalSiteId: place.culturalSiteId,
        })
      });
  
      if (response.ok) {
        alert(`✅ "${place.properties.name}" added to your favorites!`);
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
    navigate("/"); // Redirect to login page
  };

  //Logged in check
  const isLoggedIn = () => !!localStorage.getItem("token");




  

  return (
    <div className="app-container">
      {/* Sidebar */}
      <div className="sidebar-panel">
        <h3 className="sidebar-title">Locations</h3>
        <div className="favorites-toggle">
          <label>
            <input
                type="checkbox"
                checked={showFavoritesOnly}
                onChange={() => setShowFavoritesOnly(!showFavoritesOnly)}
            />
            Show Favorites Only
          </label>
        </div>

        <div className="museum-toggle">
          <label>
            <input
              type="checkbox"
              checked={showMuseumsOnly}
              onChange={() => setShowMuseumsOnly(!showMuseumsOnly)}
            />
            Show Museums Only
          </label>
        </div>
        <div className="type-toggle">
          <label>
            <input
              type="checkbox"
              checked={showHotelsOnly}
              onChange={() => setShowHotelsOnly(!showHotelsOnly)}
            />
            Show Hotels Only
          </label>
        </div>
        <div className="type-toggle">
          <label>
            <input
              type="checkbox"
              checked={showRestaurantsOnly}
              onChange={() => setShowRestaurantsOnly(!showRestaurantsOnly)}
            />
            Show Restaurants Only
          </label>
        </div>

        {message && (
          <div className="message-box">
            {message}
          </div>
        )}
        <div className="search-wrapper">
          <input
            type="text"
            placeholder="Search by name..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="search-input"
          />
          {searchTerm && (
            <button
              className="clear-button"
              onClick={() => setSearchTerm('')}
              aria-label="Clear search"
            >
              <svg width="16" height="16" viewBox="0 0 24 24">
                <path fill="currentColor" d="M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 
                12 13.41 17.59 19 19 17.59 13.41 12z"/>
              </svg>
            </button>
          )}
        </div>
        <ul className="location-list">
          {(searchTerm.trim() || showFavoritesOnly) &&  filteredFeatures.map((feature) => (
            <li
              key={feature.culturalSiteId}
              className="location-item"
              onClick={() => {
                const featureCoords = feature.geometry.coordinates;

                const matchedMarker = markersRef.current.find((marker) => {
                  const lngLat = marker.getLngLat();
                  return lngLat.lng === featureCoords[0] && lngLat.lat === featureCoords[1];
                });

                if (matchedMarker) {
                  mapRef.current.flyTo({
                    center: featureCoords,
                    zoom: 15,
                    speed: 0.8,
                  });
                  matchedMarker.togglePopup();
                }
              }}
            >
              <strong>{feature.properties.name}</strong><br />
              <span style={{ fontSize: '1em'  }}>
                      {getMarkerEmoji(extractCategory(feature.properties))}
                    </span>
              <span style={{ marginLeft: '10px' }}>
                      {extractCategory(feature.properties).toString().toUpperCase()}
                    </span>
              {
                //reviews
                feature.reviews.averageRating && feature.reviews.totalReviews > 0 && (
                  <div style={{ color: 'gray', fontSize: '0.9em' }}>
                   <a onClick={(event) => {
                     event.stopPropagation(); // Prevent map zoom trigger
                     setShowReviews(true)
                   }}> {feature.reviews.totalReviews} review{feature.reviews.averageRating > 1 ? 's' : ''}</a>
                    {' '}-  Rating: {feature.reviews.averageRating.toFixed(1)} ⭐
                  </div>
                )
              }
              {
                // Opening hours
                feature.properties.openingHours && (
                  <div style={{ color: 'gray', fontSize: '0.9em' }}>
                    {feature.properties.openingHours}
                  </div>
                )
              }

              {
                  getAddress(feature.properties) && (
                      <div style={{ color: 'gray', fontSize: '0.9em' }}>
                        {getAddress(feature.properties)}
                      </div>
                  )
              }

              {
                //website
                feature.properties.website && (
                  <div style={{ color: 'gray', fontSize: '0.9em' }}>
                    <a href={feature.properties.website} target="_blank" rel="noopener noreferrer">Website</a>
                  </div>
                )
              }

              {localStorage.getItem("token") && !favorites.some(fav => fav.culturalSite.culturalSiteId === feature.culturalSiteId) && (


                <button
                  className="favorite-button"
                  onClick={(e) => {
                    e.stopPropagation(); // Prevent map zoom trigger
                    addToFavorites(feature);
                  }}
                >
                  ⭐ Add to Favorites
                </button>
              )}
              {
                localStorage.getItem("token") && favorites.some(fav => fav.culturalSite.culturalSiteId === feature.culturalSiteId) && (
                  <span style={{
                    color: 'red',
                    marginRight: '10px',
                    cursor: 'pointer',
                    display: 'inline-block'
                  }}>❤️
                  </span>
                )

              }

              <br/>


              <button
                  style={{ marginTop: '10px', marginBottom: '10px' }}
                className="direction-button"
                onClick={() => handleShowDirections(feature.geometry.coordinates)}
              >
                🧭 Directions
              </button>

              {
                localStorage.getItem("token") && (
                    <AddReviewModal culturalSiteId={feature.culturalSiteId}></AddReviewModal>
                    )
              }
              {
                <ReviewViewerModal
                    show={showReviews}
                    handleClose={() => setShowReviews(false)}
                    culturalSiteId={feature.culturalSiteId}
                />
              }





            </li>
          ))}
        </ul>

      </div>

      {/* Map container */}
      <div id="map-container" ref={mapContainerRef} />



    </div>
  )
}



//add Reviews







export default MapView

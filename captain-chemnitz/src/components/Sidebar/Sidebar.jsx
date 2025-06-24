import { useSelector, useDispatch } from 'react-redux';
import { setSearchTerm, setSelectedFeature } from '../../uiSlice';
import { Link } from 'react-router-dom';

const Sidebar = ({ message, addToFavorites, handleShowDirections, markersRef, mapRef, filteredFeatures }) => {
  const dispatch = useDispatch();
  const searchTerm = useSelector(state => state.ui.searchTerm);
  const features = useSelector(state => state.ui.features);

  return (
    <div className="sidebar-panel">
      <h3 className="sidebar-title">Locations</h3>
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
          onChange={(e) => dispatch(setSearchTerm(e.target.value))}
          className="search-input"
        />
        {searchTerm && (
          <button
            className="clear-button"
            onClick={() => dispatch(setSearchTerm(''))}
            aria-label="Clear search"
          >
            <svg width="16" height="16" viewBox="0 0 24 24">
              <path fill="currentColor" d="M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 
              10.59 12 5 17.59 6.41 19 12 13.41 
              17.59 19 19 17.59 13.41 12z"/>
            </svg>
          </button>
        )}
      </div>

      <ul className="location-list">
        {searchTerm.trim() && filteredFeatures.map((feature) => (
          <li 
            key={feature.id} 
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

              dispatch(setSelectedFeature(feature));
            }}
          >
            <strong>{feature.properties.name}</strong><br />
            <a href={feature.properties.website} target="_blank" rel="noreferrer">Website</a>
            <Link to={`/location/${encodeURIComponent(feature.id)}`} className="details-link">View Details</Link>
            <button
              className="favorite-button"
              onClick={(e) => {
                e.stopPropagation(); 
                addToFavorites(feature);
              }}
            >
              ⭐ Add to Favorites
            </button>
            <button 
              className="direction-button"
              onClick={() => handleShowDirections(feature.geometry.coordinates)}
            >
              🧭 Directions
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default Sidebar;

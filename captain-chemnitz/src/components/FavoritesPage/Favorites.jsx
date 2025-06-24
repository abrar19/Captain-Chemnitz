import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import './favorite.css';

function Favorites() {
  const [favorites, setFavorites] = useState([]);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchFavorites = async () => {
      try {
        const tokenData = localStorage.getItem('token');
        if (!tokenData) {
          setError('Not logged in');
          return;
        }

        const { token, userId } = JSON.parse(tokenData);

        const response = await fetch(`http://localhost:5029/api/user/${userId}/favorites`, {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });
        
        if (response.status === 401) {
          // Token invalid or expired
          localStorage.removeItem('token');  // clear token
          setError('Session expired, please login again.');
          return;
        }
        
        if (response.ok) {
          const data = await response.json();
          setFavorites(data);
        } else {
          const errText = await response.text();
          setError(`Failed to load favorites: ${errText}`);
        }
      } catch (err) {
        setError('Error fetching favorites');
      }
    };

    fetchFavorites();
  }, []);

  const handleRemoveFavorite = async (favoriteId) => {
    const tokenData = localStorage.getItem('token');
    if (!tokenData) {
      setError('Not logged in');
      return;
    }
  
    const { token, userId } = JSON.parse(tokenData);
  
    try {
      const response = await fetch(`http://localhost:5029/api/user/${userId}/favorites/${favoriteId}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
  
      if (response.ok) {
        // Update state after delete
        setFavorites(prev => prev.filter(fav => fav.id !== favoriteId));
      } else {
        const errText = await response.text();
        setError(`Failed to remove favorite: ${errText}`);
      }
    } catch (err) {
      setError('Error removing favorite');
    }
  };
  

  return (
    <div className="favorites-page">
      <h2 className="favorites-title">⭐ My Favorite Places</h2>

      {error && <p className="favorites-error">{error}</p>}

      {favorites.length === 0 && !error && (
        <p className="favorites-empty">You haven’t added any favorites yet.</p>
      )}

      <ul className="favorites-list">
        {favorites.map(fav => (
          <li key={fav.id} className="favorite-item">
            <Link 
              to={`/location/${encodeURIComponent(fav.locationId)}`} 
              className="favorite-link"
            >
              {fav.name}
            </Link>
            <button 
              className="remove-button" 
              onClick={() => handleRemoveFavorite(fav.id)}
            >
              ❌ Remove
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
}

export default Favorites;

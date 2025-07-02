import { useEffect, useState } from 'react';

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

  return (
    <div className="favorites-container">
      <h2>My Favorite Places</h2>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <ul>
        {favorites.map(fav => (
          <li key={fav.id}>
            <strong>{fav.name}</strong><br />
            <span>{fav.description}</span>
          </li>
        ))}
        {favorites.length === 0 && !error && <p>No favorites yet.</p>}
      </ul>
    </div>
  );
}

export default Favorites;

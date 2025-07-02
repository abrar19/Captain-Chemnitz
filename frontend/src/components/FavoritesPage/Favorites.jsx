import { useEffect, useState } from 'react';
import {APIEndpoints} from "../../constants/APIEndpoints.js";
import {data} from "react-router-dom";
import Card from 'react-bootstrap/Card';
import {Button} from "react-bootstrap";

function Favorites() {
  const [favorites, setFavorites] = useState([]);
  const [error, setError] = useState('');

  const fetchFavorites = async () => {
    try {
      const tokenData = localStorage.getItem('token');
      if (!tokenData) {
        setError('Not logged in');
        return;
      }

      const { token } = JSON.parse(tokenData);

      console.log(token);

      fetch(APIEndpoints.getMyFavoriteSites, {
        method: 'GET',
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json',
          'Authorization': 'Bearer ' + token,

        }
      }).then(async res => {
        if (!res.ok) {
          throw new Error('Failed to fetch cultural sites');
        }
        var data= await res.json();
        console.log(data);
        setFavorites(data);
      });


      // const response = await fetch(APIEndpoints.getMyFavoriteSites,
      //
      //     {
      //       method: 'GET',
      //   headers: {
      //     'Authorization': `Bearer ${token}`
      //   }
      // });
      //
      // if (response.ok) {
      //   const data = await response.json();
      //   setFavorites(data);
      // } else {
      //   const errText = await response.text();
      //   setError(`Failed to load favorites: ${errText}`);
      // }
    } catch (err) {
      console.log(err);
      setError('Error fetching favorites');
    }
  };

  useEffect(() => {


    fetchFavorites().then(r => {
        console.log('Favorites fetched successfully');
    });
  }, []);

  return (
    <div  style={ {margin : 10 , padding : 10, backgroundColor : '#f8f9fa', borderRadius : 5}}>
      <h2>My Favorite Places</h2>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <ul style={ {listStyleType : 'none', padding : 10}} >
        {favorites.map(fav =>

          (
              <Card key={fav.favoriteSiteId} style={{ marginTop : 10,textAlign : 'left'}}>
                <Card.Body>
                  <Card.Title style={{fontSize : '2em'}}>{fav.culturalSite.properties.name}</Card.Title>
                  <div style={{color: 'black', fontSize: '1.2em'}}>
                       <span style={{ fontSize: '1em'  }}>
                      {getMarkerEmoji(extractCategory(fav.culturalSite.properties))}
                    </span>
                    <span style={{ marginLeft: '10px' }}>
                      {extractCategory(fav.culturalSite.properties).toString().toUpperCase()}
                    </span>
                    {
                      // "addrCity": "",
                      // "addrHousenumber": "",
                      // "addrPostcode": "",
                      // "addrStreet": "",
                        getAddress(fav.culturalSite.properties) && (
                            <div style={{ color: 'gray', fontSize: '0.9em' }}>
                              {getAddress(fav.culturalSite.properties)}
                            </div>
                        )

                    }

                    {fav.culturalSite.properties.openingHours && (
                        <div style={{  color: 'gray', fontSize: '0.9em' }}>
                          {fav.culturalSite.properties.openingHours}
                        </div>
                    )}

                    {fav.culturalSite.properties.website && (
                        <div style={{  color: 'gray', fontSize: '0.9em'  }}>
                          <a href={fav.culturalSite.properties.website}>Website</a>
                        </div>
                    )}

                    <br/>
                    <Button variant="primary" onClick={() => handleRemoveFromFavorites(fav.favoriteSiteId)}>Remove From Favorite</Button>
                  </div>

                </Card.Body>
              </Card>
          )

        )}
        {favorites.length === 0 && !error && <p>No favorites yet.</p>}
      </ul>
    </div>
  );
}

var handleRemoveFromFavorites = async (favoriteId) => {
    const tokenData = localStorage.getItem('token');
    if (!tokenData) {
        alert('Not logged in');
        return;
    }

    const { token } = JSON.parse(tokenData);

    try {
        const response = await fetch(`${APIEndpoints.deleteFavoriteSite(favoriteId)}`, {
        method: 'DELETE',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        }
        });

        if (response.ok) {


        alert('Removed from favorites successfully');
        } else {
        const errText = await response.text();
        alert(`Failed to remove favorite: ${errText}`);
        }
    } catch (err) {
        console.error(err);
        alert('Error removing favorite');
    }
}

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

const extractCategory = (properties) => {
  // Order matters: check common keys from most to least specific
  if (properties.tourism) return properties.tourism;
  if (properties.amenity) return properties.amenity;
  if (properties.shop) return properties.shop;
  if (properties.leisure) return properties.leisure;
  // Add more keys as needed
  return 'unknown';
};


const getAddress = (properties) => {
  const { addrStreet, addrHousenumber, addrPostcode, addrCity } = properties;
  return [addrStreet, addrHousenumber, addrPostcode, addrCity]
    .filter(Boolean)
    .join(', ');
};


export default Favorites;

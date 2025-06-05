import { useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';

function LocationDetails() {
  const { id } = useParams();
  const [location, setLocation] = useState(null);
  const [reviews, setReviews] = useState([]);
  const [newReview, setNewReview] = useState('');

  useEffect(() => {
    fetch('/Chemnitz.geojson')
      .then((res) => res.json())
      .then((data) => {
        const found = data.features.find(f => f.id === id);
        setLocation(found);
      });
  }, [id]);

  const handleAddReview = () => {
    if (newReview.trim()) {
      setReviews([...reviews, newReview.trim()]);
      setNewReview('');
    }
  };

  if (!location) return <p>Loading...</p>;

  const { name, website } = location.properties;

  return (
    <div className="details-page">
      <h2>{name}</h2>
      <p><a href={website} target="_blank" rel="noreferrer">Official Website</a></p>

      <hr />

      <h3>User Reviews</h3>
      <ul>
        {reviews.map((r, idx) => (
          <li key={idx}>{r}</li>
        ))}
      </ul>

      <textarea
        value={newReview}
        onChange={(e) => setNewReview(e.target.value)}
        rows={3}
        placeholder="Write a review..."
      />
      <br />
      <button onClick={handleAddReview}>Submit Review</button>
    </div>
  );
}

export default LocationDetails;

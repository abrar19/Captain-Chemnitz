import React, { useEffect, useState } from 'react';
import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';
import ListGroup from 'react-bootstrap/ListGroup';
import {APIEndpoints} from "../../constants/APIEndpoints.js";

function ReviewViewerModal({ show, handleClose, culturalSiteId }) {
    const [reviews, setReviews] = useState([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (show && culturalSiteId) {
            fetchReviews(culturalSiteId);
        }
    }, [show, culturalSiteId]);

    const fetchReviews = async (siteId) => {
        setLoading(true);
        try {
            var tokenData = localStorage.getItem('token');
            var token = JSON.parse(tokenData).token;
            const response = await fetch(APIEndpoints.getReviews(siteId), {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            if (!response.ok) {
                throw new Error('Failed to fetch reviews');
            }

            const data = await response.json();



            console.log(data.length);
            setReviews(data);
        } catch (error) {
            console.error(error);
            setReviews([]);
        } finally {
            setLoading(false);
        }
    };

    return (
        <Modal show={show} onHide={handleClose} size="lg">
            <Modal.Header closeButton>
                <Modal.Title>Reviews</Modal.Title>
            </Modal.Header>

            <Modal.Body>
                {loading ? (
                    <p>Loading reviews...</p>
                ) :  (
                    <ListGroup   >
                        <div >
                            {reviews.map((review) => (
                                <ListGroup.Item key={review.id} style={{ marginTop: '30px' }}>
                                    <div>
                                        <strong>{review.firstName} {review.lastName}</strong> —
                                        <span style={{ marginLeft: '10px' }}>⭐ {review.rating}/5</span>
                                    </div>
                                    <div style={{ fontSize: '0.9em', color: '#666' }}>
                                        {new Date(review.createdAt).toLocaleString()}
                                    </div>
                                    <p style={{ marginTop: '8px' }}>{review.reviewText}</p>
                                </ListGroup.Item>
                            ))}
                        </div>
                    </ListGroup>
                ) }
            </Modal.Body>

        </Modal>
    );
}

export default ReviewViewerModal;
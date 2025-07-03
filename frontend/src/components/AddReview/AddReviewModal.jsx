import { useState } from 'react';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import Modal from 'react-bootstrap/Modal';
import {APIEndpoints} from "../../constants/APIEndpoints.js";

function AddReviewModal({ culturalSiteId }) {
    const [show, setShow] = useState(false);
    const [reviewText, setReviewText] = useState('');
    const [reviewStars, setReviewStars] = useState(5);
    const [loading, setLoading] = useState(false);

    const handleClose = () => {
        setShow(false);
        setReviewText('');
        setReviewStars(5);
    };

    const handleShow = () => setShow(true);

    const handleSubmit = async () => {
        setLoading(true);
        try {
            var tokenData = localStorage.getItem('token');
            var token = JSON.parse(tokenData).token;
            const response = await fetch(APIEndpoints.addReview, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({
                    culturalSiteId: culturalSiteId,
                    reviewText: reviewText,
                    rating: reviewStars,
                }),
            });

            if (!response.ok) {
                const errorData = await response.json();
                alert('Failed to submit review: ' + errorData.message);

            }else {
                alert('Review submitted!');
                handleClose();
            }


        } catch (error) {
            console.error(error);
            alert('There was an error submitting your review.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <>
            <Button variant="success" onClick={(e) => {
                e.stopPropagation(); // Prevent map zoom trigger
                handleShow();
            }}
             disabled={!localStorage.getItem('token')}>
                ➕ Add Review
            </Button>

            <Modal show={show} onHide={handleClose}>
                <Modal.Header closeButton>
                    <Modal.Title>Add Your Review</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form>
                        <Form.Group className="mb-3" controlId="reviewText">
                            <Form.Label>Review</Form.Label>
                            <Form.Control
                                as="textarea"
                                rows={3}
                                placeholder="Write your review..."
                                value={reviewText}
                                onChange={(e) => setReviewText(e.target.value)}
                            />
                        </Form.Group>

                        <Form.Group className="mb-3" controlId="reviewStars">
                            <Form.Label>Rating (1 to 5)</Form.Label>
                            <Form.Control
                                type="number"
                                min={1}
                                max={5}
                                value={reviewStars}
                                onChange={(e) => setReviewStars(Number(e.target.value))}
                            />
                        </Form.Group>
                    </Form>
                </Modal.Body>

                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose} disabled={loading}>
                        Cancel
                    </Button>
                    <Button variant="primary" onClick={handleSubmit} disabled={loading}>
                        {loading ? 'Saving...' : 'Submit Review'}
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
}

export default AddReviewModal;
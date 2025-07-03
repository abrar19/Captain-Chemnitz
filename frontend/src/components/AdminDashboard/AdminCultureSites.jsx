import React, {useState} from 'react';
import Card from "react-bootstrap/Card";
import {APIEndpoints} from "../../constants/APIEndpoints.js";
import {jwtDecode} from "jwt-decode";
import {Table} from "react-bootstrap";
import Form from "react-bootstrap/Form";
import Button from "react-bootstrap/Button";



function AdminCultureSites() {


    const isTokenExpired = (token) => {
        try {
            const { exp } = jwtDecode(token);
            return Date.now() >= exp * 1000;
        } catch {
            return true;
        }
    };

    const isAdmin = () => {
        const token = JSON.parse(localStorage.getItem('token'))?.token;
        if (!token || isTokenExpired(token)) {
            return false;
        }
        try {
            const decodedToken = jwtDecode(token);
            return decodedToken.role === 'Admin';

        } catch (error) {
            console.error('Error decoding token:', error);
            return false;
        }
    };


    const [cultureSites, setCultureSites] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [isSyncing, setIsSyncing] = useState(false);

    const syncWithOverPass = async () => {
        try {
            setIsSyncing(true);
            const tokenData = JSON.parse(localStorage.getItem('token'));
            const token = tokenData?.token;

            const response = await fetch(APIEndpoints.syncWithOverPass, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error('Failed to fetch users');
            }
            alert("Cultural Sites Synced Successfully");
        } catch (error) {
            console.error('Error syncing cultural sites:', error);
        } finally {
            setIsSyncing(false);
        }
    }






    const fetchUsers = async () => {
        try {
            setIsLoading(true);
            const tokenData = JSON.parse(localStorage.getItem('token'));
            const token = tokenData?.token;

            const response = await fetch(APIEndpoints.culturalSites, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error('Failed to fetch users');
            }

            const data = await response.json();
            setCultureSites(data);
        } catch (error) {
            console.error('Error fetching users:', error);
        } finally {
            setIsLoading(false);
        }
    }
    useState(() => {
        if (!isAdmin()) {
            alert("You are not authorized to view this page");
            window.location.href = '/';
            localStorage.removeItem('token');
            return;
        }

        fetchUsers();
    }, []);



    return(
        <div className="dashboard">
            <aside className="sidebar">
                <h2 className="sidebar-title text-white">Admin Panel</h2>
                <nav>
                    <ul className="sidebar-nav">
                        <li><a href="/admin">Dashboard</a></li>
                        <li><a href="/admin/users">Users</a></li>
                        <li><a href="/admin/culturalsites">CulturalSites</a></li>
                    </ul>
                </nav>
            </aside>


            <main className="main-content">
                <h2>Culture Sites</h2>
                {isLoading ? (
                    <div className="loading">
                        <p>Loading...</p>
                    </div>
                ) : (
                    <div className="user-list">

                        <Card className="mb-3">
                            <Card.Body>
                                <Card.Title>Total Culture Sites: {cultureSites.length}</Card.Title>
                                <Card.Text>
                                    <Button isSyncing variant="primary"  onClick={

                                    () => {
                                        if (isSyncing) return;
                                        syncWithOverPass();
                                    }
                                   }>{isSyncing? "Syncing":"Sync With OverPass"}</Button>
                                </Card.Text>
                            </Card.Body>
                        </Card>

                        <Table striped bordered hover>
                            <thead>
                            <tr>
                                <th>#</th>
                                <th>Site Name</th>
                                <th>Site Type</th>
                                <th>Address </th>
                            </tr>
                            </thead>
                            <tbody>

                            {cultureSites.map((site, index) => (
                                <tr key={index}>
                                    <td>{index + 1}</td>
                                    <td>{site.properties.name}</td>
                                    <td>{extractCategory(site.properties)}</td>
                                    <td>{getAddress(site.properties)}</td>
                                </tr>
                            ))}
                            </tbody>
                        </Table>

                    </div>
                )}
            </main>
        </div>

    )
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

export default AdminCultureSites;
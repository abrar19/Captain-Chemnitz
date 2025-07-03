import React, {useEffect, useState} from 'react';
import './AdminDashboard.css';
import {jwtDecode} from "jwt-decode";
import {useNavigate} from "react-router-dom";
import {APIEndpoints} from "../../constants/APIEndpoints.js";
import Card from "react-bootstrap/Card";
import Button from "react-bootstrap/Button";
import {Tab, Tabs} from "react-bootstrap";









function AdminDashboard() {


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

    const [adminDashBoardData, setAdminDashBoardData] = useState([]);

    const [isLoading, setIsLoading] = useState(true);

    const navigate = useNavigate();

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/');
    };


    const fetchAdminDashboardData = async () => {

        try {
            setIsLoading(true);
            var tokenData = JSON.parse(localStorage.getItem('token'));
            var token = tokenData?.token;





            fetch(APIEndpoints.getAdminDashboardData, {
                method: 'GET',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + token,

                }
            }).then(async res => {
                if (!res.ok) {
                    throw new Error('Failed to fetch  data');
                }
                var data= await res.json();
                console.log(data);
                setIsLoading(false);
                setAdminDashBoardData(data);

            });
        }catch (error) {
            setIsLoading(false);
            console.error('Error fetching admin dashboard data:', error);
        }

    }

    useEffect(() => {
        fetchAdminDashboardData().then(
            () => console.log('Admin dashboard data fetched successfully'),
        )


    }, []);



    if (!isAdmin()) {
    return (
      <div className="unauthorized">
        <h1>Unauthorized Access</h1>
        <p>You do not have permission to view this page.</p>

      </div>
    );
    }else

  return (
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
        <header className="dashboard-header">
          <h1>Welcome, Admin</h1>
          <button onClick={handleLogout} className="logout-btn">Logout</button>
        </header>

      {        isLoading && (
            <div className="loading">
                <p>Loading dashboard data...</p>
            </div>
      )}

      {!isLoading &&(
          <section className="stats-cards">
              <div className="card">
                  <h3>Total Users</h3>
                  <p>{
                      adminDashBoardData.totalUsers || '0'
                  }</p>
              </div>
              <div className="card">
                  <h3>Deleted User</h3>
                  <p>{
                        adminDashBoardData.totalInactiveUsers || '0'
                  }</p>
              </div>
              <div className="card">
                  <h3>Total Cultural Sites</h3>
                  <p>{
                        adminDashBoardData.totalCulturalSites || '0'
                  }</p>
              </div>
          </section>
      )}

      {!isLoading && (
          <section className="recent-activity">
              <h2>Recent Activity</h2>
              <ul>
                  {adminDashBoardData.recentReviews && adminDashBoardData.recentReviews.length > 0 ? (
                  adminDashBoardData.recentReviews.map((recentReview, index) => (
                  <li key={index}>
                      <Card key={index} style={{   }} >
                          <Card.Header>{recentReview.culturalSiteName}</Card.Header>
                          <Card.Body style={{  fontSize: '1.2em', textAlign: 'left'}}>
                              <Card.Text style={{
                                  fontSize: '1.2em' }}>
                                    <div className={
                                        "text-body text-wrap"
                                    }><a>Review:</a> {recentReview.reviewText}<br /></div>
                                    <a>Rating:</a> {recentReview.rating}<br />
                                    <a>User:</a> {recentReview.firstName}
                              </Card.Text>
                          </Card.Body>
                      </Card>
                      </li>
                  ))
                  ) : (
                      <li>No recent activity found.</li>
                  )}
                  {/* add your dynamic activity logs here */}
              </ul>
          </section>
          )}
      </main>
    </div>
  );
}

export default AdminDashboard;

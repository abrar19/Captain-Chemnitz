import React from 'react';
import './AdminDashboard.css';
import {jwtDecode} from "jwt-decode";
import {useNavigate} from "react-router-dom";


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




function AdminDashboard() {
    const navigate = useNavigate();

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/');
    };

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
        <h2 className="sidebar-title">Admin Panel</h2>
        <nav>
          <ul className="sidebar-nav">
            <li><a href="/dashboard">Dashboard</a></li>
            <li><a href="/users">Users</a></li>
            <li><a href="/favorites">Favorites</a></li>
            <li><a href="/settings">Settings</a></li>
            <li><a href="/logs">Logs</a></li>
          </ul>
        </nav>
      </aside>

      <main className="main-content">
        <header className="dashboard-header">
          <h1>Welcome, Admin</h1>
          <button onClick={handleLogout} className="logout-btn">Logout</button>
        </header>

        <section className="stats-cards">
          <div className="card">
            <h3>Total Users</h3>
            <p>1,234</p>
          </div>
          <div className="card">
            <h3>Active Sessions</h3>
            <p>123</p>
          </div>
          <div className="card">
            <h3>New Signups</h3>
            <p>45</p>
          </div>
        </section>

        <section className="recent-activity">
          <h2>Recent Activity</h2>
          <ul>
            <li>User John added a new favorite.</li>
            <li>Settings updated by Admin.</li>
            <li>User Jane registered.</li>
            {/* add your dynamic activity logs here */}
          </ul>
        </section>
      </main>
    </div>
  );
}

export default AdminDashboard;

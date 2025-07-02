import { Link, useNavigate } from 'react-router-dom';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';
import './header.css';

function Header() {
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/');
  };

  const isLoggedIn = !!localStorage.getItem('token');

  return (
    <header className="header">
      <div className="header-left">
          <h1>Captain Chemnitz</h1>


      </div>

      <div className="header-right">
        {!isLoggedIn && (
          <>
            <Link to="/login" className="header-button">Login</Link>
            <Link to="/register" className="header-button register-button">Register</Link>
          </>
        )}

        {isLoggedIn && (
          <div className="floating-menu">
            <input type="checkbox" id="menu-toggle" className="menu-toggle" />
            <label htmlFor="menu-toggle" className="menu-button" title="Menu">☰</label>
            <div className="menu-items">
              <Link to="/favorites" className="menu-item" title="Favorites">⭐ Favorites</Link>
              <Link to="/profile" className="menu-item" title="Edit Profile">📝 Profile</Link>
              <button
                className="menu-item logout-button"
                onClick={handleLogout}
                title="Logout"
              >
                🚪 Logout
              </button>
            </div>
          </div>
        )}
      </div>
    </header>
  );
}

export default Header;

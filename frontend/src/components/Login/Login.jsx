import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import './login.css';
import {APIEndpoints} from "../../constants/APIEndpoints.js";

function Login() {
  const navigate = useNavigate();
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const handleLogin = async (e) => {
    e.preventDefault();
  
    try {

      const response = await fetch(APIEndpoints.login, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
      // body:
        // {
      //   "email": "string",
      //     "password": "string"
      // }
        body: JSON.stringify({ "email": username, "password": password })
      });
  
      if (response.ok) {
        const data = await response.json(); // parse JSON (token + userId)
  
        // Save token + userId in localStorage
        localStorage.setItem('token', JSON.stringify({
          token: data.token,
          firstName: data.firstName,
          lastName: data.lastName,
          email: data.email,
          role: data.role,
        }));
  
        navigate('/');
      }else {
        const data = await response.json();
        setError(`Login failed: ${data.message || 'Something went wrong'}`);

      }
    } catch (err) {
      setError('Error connecting to server');
    }
  };
  

  return (
    <div className="login-container">
      <form onSubmit={handleLogin} className="login-form">
        <h2>Login</h2>
        {error && <p className="error-text">{error}</p>}
        <input
          type="text"
          placeholder="Email"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          required
        />
        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        <button type="submit">Login</button>
      </form>
      <p className="login-footer">
        Don’t have an account? <Link to="/register">Register here</Link>
      </p>
    </div>
  );
}

export default Login;

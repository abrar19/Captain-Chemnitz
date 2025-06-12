import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import './login.css';

function Login() {
  const navigate = useNavigate();
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const handleLogin = (e) => {
    e.preventDefault();

    // Mock auth logic (replace with real backend logic)
    const storedUser = JSON.parse(localStorage.getItem('registeredUser'));

    if (
      (username === 'admin' && password === 'password') ||
      (storedUser?.email === username && storedUser?.password === password)
    ) {
      localStorage.setItem('isAuthenticated', 'true');
      navigate('/'); // redirect to map page
    } else {
      setError('Invalid username or password');
    }
  };

  return (
    <div className="login-container">
      <form onSubmit={handleLogin} className="login-form">
        <h2>Login</h2>
        {error && <p className="error-text">{error}</p>}
        <input
          type="text"
          placeholder="Username or Email"
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

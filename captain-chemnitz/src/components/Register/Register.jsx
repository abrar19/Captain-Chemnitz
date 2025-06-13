import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';

function Register() {

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [message, setMessage] = useState('');

  const navigate = useNavigate();

  const handleRegister = async (e) => {
    e.preventDefault();

    try {
      const response = await fetch('http://localhost:5029/api/user/register', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ username: email, password })
      });

      if (response.ok) {
        setMessage('Registration successful!');
        navigate('/login');
      } else {
        const errorText = await response.text();
        setMessage(`Registration failed: ${errorText}`);
      }
    } catch (err) {
      setMessage('Error connecting to server');
    }

  };

  return (
    <div className="login-container">
      <h2>Register</h2>
      {message && <p>{message}</p>}
      <form onSubmit={handleRegister} className="login-form">
        <input 
          type="email" 
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required 
        />
        <input 
          type="password" 
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required 
        />
        <input 
          type="password" 
          placeholder="Confirm Password"
          value={confirmPassword}
          onChange={(e) => setConfirmPassword(e.target.value)}
          required 
        />
        <button type="submit">Register</button>
      </form>
      <p className="login-footer">
        Already have an account? <Link to="/login">Login here</Link>
      </p>
    </div>
  );
}

export default Register;

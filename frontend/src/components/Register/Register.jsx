import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import {APIEndpoints as ApiEndpoints} from "../../constants/APIEndpoints.js";

function Register() {

  const [email, setEmail] = useState('');
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [password, setPassword] = useState('');
  const [message, setMessage] = useState('');

  const navigate = useNavigate();

  const handleRegister = async (e) => {
    e.preventDefault();

    try {
      const response = await fetch(ApiEndpoints.register, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ "email": email, "firstName": firstName, "lastName": lastName, "password": password })
      });

      if (response.ok) {
        setMessage('Registration successful!');
        navigate('/login');
      } else {
        const data = await response.json();
        setMessage(`Login failed: ${data.message || 'Something went wrong'}`);
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
            type="text"
            placeholder="First Name"
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
            required
        />

        <input
            type="text"
            placeholder="Last Name"
            value={lastName}
            onChange={(e) => setLastName(e.target.value)}
            required
        />



        <input 
          type="password" 
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
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

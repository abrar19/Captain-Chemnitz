import { Navigate } from 'react-router-dom';

function ProtectedRoute({ children }) {
  const tokenString = localStorage.getItem('token');
  let token = null;

  try {
    const parsed = JSON.parse(tokenString);
    token = parsed.token;
  } catch (e) {
    // Invalid JSON or null, treat as unauthenticated
  }

  return token === 'dummy-token' ? children : <Navigate to="/login" replace />;
}

export default ProtectedRoute;

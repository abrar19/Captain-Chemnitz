


const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:8080/api';

export const APIEndpoints = {
    register: `${API_BASE_URL}/User/Registration`,
    login: `${API_BASE_URL}/User/Login`,

}



const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:8080/api';

export const APIEndpoints = {
    register: `${API_BASE_URL}/User/Registration`,
    login: `${API_BASE_URL}/User/Login`,
    culturalSites: `${API_BASE_URL}/CulturalSite/GetCulturalSites`,
    getMyFavoriteSites: `${API_BASE_URL}/FavoriteSite/GetFavoriteSites`,
    deleteFavoriteSite:(id) => `${API_BASE_URL}/FavoriteSite/removeFromFavorites/${id}`,
    

}
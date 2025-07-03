


const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:8080/api';

export const APIEndpoints = {
    register: `${API_BASE_URL}/User/Registration`,
    login: `${API_BASE_URL}/User/Login`,
    culturalSites: `${API_BASE_URL}/CulturalSite/GetCulturalSites`,
    getMyFavoriteSites: `${API_BASE_URL}/FavoriteSite/GetFavoriteSites`,
    deleteFavoriteSite:(id) => `${API_BASE_URL}/FavoriteSite/removeFromFavorites/${id}`,
    addFavoriteSite: `${API_BASE_URL}/FavoriteSite/addToFavorites`,
    getUserProfile: `${API_BASE_URL}/User/GetUserProfile`,
    updateUserProfile: `${API_BASE_URL}/User/UpdateProfile`,
    updateUserPassword: `${API_BASE_URL}/User/UpdatePassword`,
    addReview: `${API_BASE_URL}/Review/addReview`,
    getReviews: (id) => `${API_BASE_URL}/Review/getReviews?culturalSiteId=${id}`,
    deleteUser: `${API_BASE_URL}/User/DeleteUser`,
    getAdminDashboardData: `${API_BASE_URL}/Admin/GetDashboardStatistics`,
    getAllUsers: `${API_BASE_URL}/Admin/GetAllUsers`,
    getActiveUsers: `${API_BASE_URL}/Admin/getAllActiveUsers`,
    getAllInactiveUsers: `${API_BASE_URL}/Admin/getAllInactiveUsers`,


}
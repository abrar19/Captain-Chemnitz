# 🌍 Chemnitz Map Explorer

A full-stack web application that displays geo-located points of interest on an interactive Mapbox map of Chemnitz. Users can search locations, view details, and get walking directions. Includes a secure login system powered by a .NET Core Web API.

---

## 🧭 Features

- 🔍 Search locations with live filtering
- 📍 Interactive Mapbox GL map with custom markers and popups
- 🧭 Walking directions from user location
- 🔐 Login & authentication via JWT (backend in .NET Core)
- 📁 Supports GeoJSON-based location data
- 🚀 Simple and extensible UI with React

---

## 🛠️ Tech Stack

### Frontend
- [React](https://reactjs.org/)
- [React Router](https://reactrouter.com/)
- [Mapbox GL JS](https://docs.mapbox.com/mapbox-gl-js/)
- [Vite](https://vitejs.dev/) (for fast dev builds)

### Backend
- [.NET 6+ Web API](https://learn.microsoft.com/en-us/aspnet/core/web-api/)
- [JWT Authentication](https://jwt.io/)

---

## 🔧 Setup Instructions
 1. Clone the repository
```bash
    git clone url-to-repo


```
 2. Run the Database
```bash
    docker-compose up mssql_db
```
 3. Configure the backend and frontend


4. Run the Backend and Frontend
```bash
    docker-compose up backend frontend



# 🌍 Chemnitz Map Explorer

A full-stack web application that displays geo-located points of interest on an interactive Mapbox map of Chemnitz. Users can search locations, view details, and get walking directions. Includes a secure login system powered by a .NET Core Web API.

---

## 🧭 Features

- 🔍 Search locations with live filtering
- 📍 Interactive Mapbox GL map with custom markers and popups
- 🧭 Walking directions from user location
- 🔐 Login & authentication page
- 📁 Supports GeoJSON-based location data
- 🚀 Simple and extensible UI with React

---

## 🛠️ Tech Stack

### Frontend
- [React](https://reactjs.org/)
- [React Router](https://reactrouter.com/)
- [Mapbox GL JS](https://docs.mapbox.com/mapbox-gl-js/)
- [Vite](https://vitejs.dev/) (for fast dev builds)

## 🔧 Setup Instructions

### 🖥️ Frontend

```bash
cd frontend
npm install
npm run dev

## Environment
VITE_MAPBOX_TOKEN=your_mapbox_token_here

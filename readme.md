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
- [.NET 9 Web API](https://learn.microsoft.com/en-us/aspnet/core/web-api/)
- [JWT Authentication](https://jwt.io/)

---

## 🔧 Setup Instructions
 1. Clone the repository
```bash
    git clone url-to-repo


```
//add .evn file with the following content
    DB_CONNECTION_STRING=Server=mssql_db;Database=ChemnitzMapExplorer;User Id=sa;Password=yourStrong(!)Password;
    JWT_SECRET_KEY=your_jwt_secret_key


 2. Add a `.env` file at the root of the project with the following content:
```env
    VITE_MAPBOX_TOKEN=XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
```

 3. Update application.json in the backend project with your secret information
```json
"UserSettings" : {
"UserEmail": "Admin email",
"UserPassword": "Admin password",
},
"ApplicationSettings": {
"SeedDataFile": "./Data/seed-data.json",
"JWT_Secret": "32_character_long_secret_key",
}
```
4. make sure you have Docker and Docker Compose installed on your machine.



5. Run the Database first
```bash
    docker-compose up mssql_db
```
6.  run the following command to create the database into backend folder

```bash
    dotnet ef database update
    
```




7. Run the Backend and Frontend
```bash
    docker-compose up backend frontend

8. Open your browser and navigate to `http://localhost:3000` to view the application and `http://localhost:8080` to access the backend API.




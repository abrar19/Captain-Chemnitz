﻿FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["backend.csproj", "./"]
RUN dotnet restore "backend.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY Data/Chemnitz.geojson ./Data/Chemnitz.geojson
COPY Data/Sachsen.geojson ./Data/Sachsen.geojson

#COPY wait-for-mssql.sh /wait-for-mssql.sh


#ENTRYPOINT ["/wait-for-mssql.sh", "dotnet", "backend.dll"]
ENTRYPOINT ["dotnet", "backend.dll"]

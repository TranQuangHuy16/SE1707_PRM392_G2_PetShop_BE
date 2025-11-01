# --- Stage 1: Build ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy toàn bộ solution
COPY ["SE1707_PRM392_G2_PetShop_BE.sln", "./"]
COPY ["PetShop.API/PetShop.API.csproj", "PetShop.API/"]
COPY ["PetShop.Repositories/PetShop.Repositories.csproj", "PetShop.Repositories/"]
COPY ["PetShop.Services/PetShop.Services.csproj", "PetShop.Services/"]

# Restore dependencies
RUN dotnet restore "PetShop.API/PetShop.API.csproj"

# Copy toàn bộ source code
COPY . .

WORKDIR /src/PetShop.API
RUN dotnet publish -c Release -o /app/publish

# --- Stage 2: Run ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Copy .env file (optional, for debug)
COPY ../.env .env

EXPOSE 8080
ENTRYPOINT ["dotnet", "PetShop.API.dll"]

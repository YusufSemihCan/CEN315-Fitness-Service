# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copy the Project File and Restore Dependencies
# This layer is cached to make future builds faster
COPY ["Fitness_Service_API/Fitness_Service_API.csproj", "Fitness_Service_API/"]
RUN dotnet restore "Fitness_Service_API/Fitness_Service_API.csproj"

# 2. Copy the Rest of the Code
COPY . .

# 3. Build and Publish
WORKDIR "/src/Fitness_Service_API"
RUN dotnet build "Fitness_Service_API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Fitness_Service_API.csproj" -c Release -o /app/publish

# Stage 2: Create the Runtime Image (Smaller & Faster)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Fitness_Service_API.dll"]
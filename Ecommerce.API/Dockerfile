# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj và restore
COPY Ecommerce.API/*.csproj ./Ecommerce.API/
RUN dotnet restore Ecommerce.API/Ecommerce.API.csproj

# Copy toàn bộ solution
COPY . .
WORKDIR /app/Ecommerce.API
RUN dotnet publish -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out ./
ENTRYPOINT ["dotnet", "Ecommerce.API.dll"]

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj và restore cho AppHost
COPY /Ecommerce.AppHost/*.csproj ./Ecommerce.AppHost/
RUN dotnet restore Ecommerce.AppHost/Ecommerce.AppHost.csproj

# Copy toàn bộ source code
COPY . .
WORKDIR /app/Ecommerce.AppHost
RUN dotnet publish -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /Ecommerce/out ./
ENTRYPOINT ["dotnet", "Ecommerce.AppHost.dll"]

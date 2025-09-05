# See https://docs.microsoft.com/aspnet/core/host-and-deploy/docker/building-net-docker-images
# for more information about .NET Docker images.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PropertyManagementSystem.Web/PropertyManagementSystem.Web.csproj", "PropertyManagementSystem.Web/"]
COPY ["PropertyManagementSystem.Core/PropertyManagementSystem.Core.csproj", "PropertyManagementSystem.Core/"]
COPY ["PropertyManagementSystem.Data/PropertyManagementSystem.Data.csproj", "PropertyManagementSystem.Data/"]
RUN dotnet restore "PropertyManagementSystem.Web/PropertyManagementSystem.Web.csproj"
COPY . .
WORKDIR "/src/PropertyManagementSystem.Web"
RUN dotnet build "PropertyManagementSystem.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PropertyManagementSystem.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PropertyManagementSystem.Web.dll"]

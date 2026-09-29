#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
ENV \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

RUN apt-get update && apt-get install -y \
    libicu-dev \
    tzdata \
 && rm -rf /var/lib/apt/lists/*
USER app
WORKDIR /app
EXPOSE 8080
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DMS.API/DMS.API.csproj", "DMS.API/"]
COPY ["DMS.Library/DMS.Library.csproj", "DMS.Library/"]
COPY ["DMS.Shared/DMS.Shared.csproj", "DMS.Shared/"]
COPY ["DLMS.Tenants/DLMS.Tenants.csproj", "DLMS.Tenants/"]
COPY Directory.Packages.props Directory.Packages.props
RUN dotnet restore "./DMS.API/DMS.API.csproj"
COPY . .
WORKDIR "/src/DMS.API"
RUN dotnet build "./DMS.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DMS.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DMS.API.dll", "--urls", "http://0.0.0.0:8080"]

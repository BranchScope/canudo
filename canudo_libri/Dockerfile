﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

ENV CONTAINERIZE_THESE_NUTS=true

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["canudo_libri.csproj", "./"]
RUN dotnet restore "canudo_libri.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "canudo_libri.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "canudo_libri.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY strings.json /app/strings.json
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "canudo_libri.dll"]

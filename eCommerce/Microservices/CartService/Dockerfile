FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./eCommerce/Microservices/CartService/CartService.csproj", "./eCommerce/Microservices/CartService/"]
COPY ["./eCommerce/Shared/MonitoringService/MonitoringService.csproj", "./eCommerce/Shared/MonitoringService/"]
COPY ["./eCommerce/Shared/Cache/Cache.csproj", "./eCommerce/Shared/Cache/"]
RUN dotnet restore "./eCommerce/Microservices/CartService/CartService.csproj"
COPY . .
WORKDIR "/src/eCommerce/Microservices/CartService"
RUN dotnet build "CartService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CartService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CartService.dll"]

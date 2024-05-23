FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./eCommerce/Microservices/ProductService/ProductService.csproj", "./eCommerce/Microservices/ProductService/"]
COPY ["./eCommerce/Shared/MonitoringService/MonitoringService.csproj", "./eCommerce/Shared/MonitoringService/"]
COPY ["./eCommerce/Shared/AppSettings/AppSettings.csproj", "./eCommerce/Shared/AppSettings/"]
COPY ["./eCommerce/Shared/Cache/Cache.csproj", "./eCommerce/Shared/Cache/"]
COPY ["./eCommerce/Shared/Messaging/Messaging.csproj", "./eCommerce/Shared/Messaging/"]

RUN dotnet restore "./eCommerce/Microservices/ProductService/ProductService.csproj"
COPY . .
WORKDIR "/src/eCommerce/Microservices/ProductService"
RUN dotnet build "ProductService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProductService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductService.dll"]

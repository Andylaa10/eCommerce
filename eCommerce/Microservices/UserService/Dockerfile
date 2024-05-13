FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./eCommerce/Microservices/UserService/UserService.csproj", "./eCommerce/Microservices/UserService/"]
COPY ["./eCommerce/Shared/Cache/Cache.csproj", "./eCommerce/Shared/Cache/"]
RUN dotnet restore "./eCommerce/Microservices/UserService/UserService.csproj"
COPY . .
WORKDIR "/src/eCommerce/Microservices/UserService"
RUN dotnet build "UserService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UserService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.dll"]

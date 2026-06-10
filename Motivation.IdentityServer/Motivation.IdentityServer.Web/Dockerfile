FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем csproj файлы
COPY ["Motivation.IdentityServer.Web/Motivation.IdentityServer.Web.csproj", "Motivation.IdentityServer.Web/"]
COPY ["../Motivation.IdentityServer.Domain/Motivation.IdentityServer.Domain.csproj", "Motivation.IdentityServer.Domain/"]
COPY ["../Motivation.IdentityServer.Infrastructure/Motivation.IdentityServer.Infrastructure.csproj", "Motivation.IdentityServer.Infrastructure/"]

RUN dotnet restore "Motivation.IdentityServer.Web/Motivation.IdentityServer.Web.csproj"

COPY . .

WORKDIR "/src/Motivation.IdentityServer.Web"
RUN dotnet build "Motivation.IdentityServer.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "Motivation.IdentityServer.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Motivation.IdentityServer.Web.dll"]
# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Motivation.KpiService/Motivation.KpiService.csproj", "Motivation.KpiService/"]
COPY ["Motivation.Application/Motivation.Application.csproj", "Motivation.Application/"]
COPY ["Motivation.Contracts/Motivation.Contracts.csproj", "Motivation.Contracts/"]
COPY ["Motivation.Domain/Motivation.Domain.csproj", "Motivation.Domain/"]
COPY ["Motivation.IdentityServer/Motivation.IdentityServer.Domain/Motivation.IdentityServer.Domain.csproj", "Motivation.IdentityServer/Motivation.IdentityServer.Domain/"]
COPY ["Motivation.IdentityServer/Motivation.IdentityServer.Infrastructure/Motivation.IdentityServer.Infrastructure.csproj", "Motivation.IdentityServer/Motivation.IdentityServer.Infrastructure/"]
COPY ["Motivation.Infrastructure/Motivation.Infrastructure.csproj", "Motivation.Infrastructure/"]
RUN dotnet restore "./Motivation.KpiService/Motivation.KpiService.csproj"
COPY . .
WORKDIR "/src/Motivation.KpiService"
RUN dotnet build "./Motivation.KpiService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Motivation.KpiService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Motivation.KpiService.dll"]
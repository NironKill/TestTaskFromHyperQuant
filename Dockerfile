FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Presentation/CryptoManager.WebApplication/CryptoManager.WebApplication.csproj", "Presentation/CryptoManager.WebApplication/"]
COPY ["Infrastructure/CryptoManager.Infrastructure/CryptoManager.Infrastructure.csproj", "Infrastructure/CryptoManager.Infrastructure/"]
COPY ["Core/CryptoManager.Domain/CryptoManager.Domain.csproj", "Core/CryptoManager.Domain/"]
COPY ["Core/CryptoManager.Application/CryptoManager.Application.csproj", "Core/CryptoManager.Application/"]
RUN dotnet restore "Presentation/CryptoManager.WebApplication/CryptoManager.WebApplication.csproj"

COPY . .
WORKDIR "/src/Presentation/CryptoManager.WebApplication"
RUN dotnet build "CryptoManager.WebApplication.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CryptoManager.WebApplication.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CryptoManager.WebApplication.dll"]
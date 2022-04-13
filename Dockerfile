#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0-focal-arm64v8 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal-arm64v8 AS build
WORKDIR /src
COPY ["Un1ver5e.Bot.III.csproj", "."]
RUN dotnet restore "./Un1ver5e.Bot.III.csproj" -r linux-arm64
COPY . .
WORKDIR "/src/."
RUN dotnet build "Un1ver5e.Bot.III.csproj" -c Release -o /app/build -r linux-arm64

FROM build AS publish
RUN dotnet publish "Un1ver5e.Bot.III.csproj" -c Release -o /app/publish -r linux-arm64

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Un1ver5e.Bot.III.dll"]
COPY ["DB.db3", "DB.db3"]

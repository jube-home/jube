FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /
COPY ["Jube.App/Jube.App.csproj", "Jube.App/"]
COPY . .
RUN dotnet restore "Jube.App/Jube.App.csproj"
WORKDIR "/Jube.App"
RUN dotnet build "Jube.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Jube.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Jube.App.dll"]
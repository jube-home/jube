FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Jube.App/Jube.App.csproj", "Jube.App/"]
COPY ["Jube.Data/Jube.Data.csproj", "Jube.Data/"]
COPY ["Accord.Statistics/Accord.Statistics.csproj", "Accord.Statistics/"]
COPY ["Accord.Math/Accord.Math.csproj", "Accord.Math/"]
COPY ["Accord.Math.Core/Accord.Math.Core.csproj", "Accord.Math.Core/"]
COPY ["Accord.Core/Accord.Core.csproj", "Accord.Core/"]
COPY ["Jube.Extensions/Jube.Extensions.csproj", "Jube.Extensions/"]
COPY ["Jube.DynamicEnvironment/Jube.DynamicEnvironment.csproj", "Jube.DynamicEnvironment/"]
COPY ["Jube.Engine/Jube.Engine.csproj", "Jube.Engine/"]
COPY ["Accord.Genetic/Accord.Genetic.csproj", "Accord.Genetic/"]
COPY ["Accord.MachineLearning/Accord.MachineLearning.csproj", "Accord.MachineLearning/"]
COPY ["Accord.Neuro/Accord.Neuro.csproj", "Accord.Neuro/"]
COPY ["Jube.Parser/Jube.Parser.csproj", "Jube.Parser/"]
COPY ["Jube.Migrations/Jube.Migrations.csproj", "Jube.Migrations/"]
COPY ["Jube.Service/Jube.Service.csproj", "Jube.Service/"]
COPY ["Jube.Validations/Jube.Validations.csproj", "Jube.Validations/"]
RUN dotnet restore "Jube.App/Jube.App.csproj"
COPY . .
WORKDIR "/src/Jube.App"
RUN dotnet build "Jube.App.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Jube.App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Jube.App.dll"]

#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
# When Visual Studio handles the building of the image, the image seens to work only when Visual Studio run a container from it
# When I tried to build the image from Dockerfile without any changes, I could run it with no problems:
# My docker build command (run from the solution folder): docker build -f Yaba.WebApi/Dockerfile -t yaba-tito .
# My docker run command: docker run --rm -p 5000:8080 -p 5001:8081 -e "ASPNETCORE_ENVIRONMENT=Development" <image-id>
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Yaba.WebApi/Yaba.WebApi.csproj", "Yaba.WebApi/"]
COPY ["Yaba.Application/Yaba.Application.csproj", "Yaba.Application/"]
COPY ["Yaba.Domain/Yaba.Domain.csproj", "Yaba.Domain/"]
COPY ["Yaba.Infrastructure.DTO/Yaba.Infrastructure.DTO.csproj", "Yaba.Infrastructure.DTO/"]
COPY ["Yaba.Infrastructure.AzureStorageQueue/Yaba.Infrastructure.AzureStorageQueue.csproj", "Yaba.Infrastructure.AzureStorageQueue/"]
COPY ["Yaba.Infrastructure.Persistence/Yaba.Infrastructure.Persistence.csproj", "Yaba.Infrastructure.Persistence/"]
COPY ["Yaba.Infrastructure.Security/Yaba.Infrastructure.Security.csproj", "Yaba.Infrastructure.Security/"]
COPY ["Yaba.Infrastructure.IoC/Yaba.Infrastructure.IoC.csproj", "Yaba.Infrastructure.IoC/"]
RUN dotnet restore "./Yaba.WebApi/Yaba.WebApi.csproj"
COPY . .
WORKDIR "/src/Yaba.WebApi"
RUN dotnet build "./Yaba.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Yaba.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Yaba.WebApi.dll"]
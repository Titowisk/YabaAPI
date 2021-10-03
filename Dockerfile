FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env

# Copy code to src/ folder in case I need the source code
WORKDIR /usr/src

COPY ./Yaba.Application ./Yaba.Application
COPY ./Yaba.Domain ./Yaba.Domain
COPY ./Yaba.Infrastructure.AzureStorageQueue ./Yaba.Infrastructure.AzureStorageQueue
COPY ./Yaba.Infrastructure.DTO ./Yaba.Infrastructure.DTO
COPY ./Yaba.Infrastructure.IoC ./Yaba.Infrastructure.IoC
COPY ./Yaba.Infrastructure.Logger ./Yaba.Infrastructure.Logger
COPY ./Yaba.Infrastructure.Persistence ./Yaba.Infrastructure.Persistence
COPY ./Yaba.Infrastructure.Security ./Yaba.Infrastructure.Security
COPY ./Yaba.WebApi ./Yaba.WebApi

# RUN tests

# Publish /p:EnviromentName=Development
RUN dotnet restore ./Yaba.WebApi/Yaba.WebApi.csproj
RUN dotnet publish ./Yaba.WebApi/Yaba.WebApi.csproj -v n -o /usr/app -c debug --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /usr/app
COPY --from=build-env /usr/app .
ENV ASPNETCORE_ENVIRONMENT="Development"
ENV ASPNETCORE_URLS=http://+:5000
CMD ["dotnet", "Yaba.WebApi.dll"] 

# RUN in release mode 
# dotnet run --project ./Yaba.WebApi/Yaba.WebApi.csproj --launch-profile YabaAPI -v n
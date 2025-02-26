FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

ENV NUGET_PACKAGES=/root/.nuget/packages

RUN mkdir -p $NUGET_PACKAGES && chmod -R 777 $NUGET_PACKAGES

RUN dotnet nuget locals all --clear

COPY ["UserPermissions.API/UserPermissions.API.csproj", "UserPermissions.API/"]
COPY ["UserPermissions.Application/UserPermissions.Application.csproj", "UserPermissions.Application/"]
COPY ["UserPermissions.Domain/UserPermissions.Domain.csproj", "UserPermissions.Domain/"]
COPY ["UserPermissions.Infrastructure/UserPermissions.Infrastructure.csproj", "UserPermissions.Infrastructure/"]

RUN dotnet restore --force --no-cache "UserPermissions.API/UserPermissions.API.csproj"

COPY . .

RUN dotnet build --no-restore -c Release
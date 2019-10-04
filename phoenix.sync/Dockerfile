FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./phoenix.sync/phoenix.sync.csproj ./phoenix.sync/
COPY ./phoenix.core/phoenix.core.csproj ./phoenix.core/
COPY ./phoenix.requests/phoenix.requests.csproj ./phoenix.requests/

RUN dotnet restore phoenix.core \
    && dotnet restore phoenix.requests \
    && dotnet restore phoenix.sync

# Copy everything else and build
COPY . ./
WORKDIR /app/phoenix.sync
RUN dotnet publish --no-restore -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app/phoenix.sync
COPY --from=build-env /app/phoenix.sync/out .
ENTRYPOINT ["dotnet", "./phoenix.sync.dll"]

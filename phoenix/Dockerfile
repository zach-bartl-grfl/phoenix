FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./phoenix/phoenix.csproj ./phoenix/
COPY ./phoenix.core/phoenix.core.csproj ./phoenix.core/
COPY ./phoenix.requests/phoenix.requests.csproj ./phoenix.requests/

RUN dotnet restore phoenix.core \
    && dotnet restore phoenix.requests \
    && dotnet restore phoenix

# Copy everything else and build
COPY . ./
WORKDIR /app/phoenix
RUN dotnet publish --no-restore -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app/phoenix
COPY --from=build-env /app/phoenix/out .
ENTRYPOINT ["dotnet", "./phoenix.dll"]

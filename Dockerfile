FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln ./
COPY AspireBudgetApi/*.csproj ./AspireBudgetApi/
COPY TestWebApi/*.csproj ./TestWebApi/
RUN dotnet restore

# Copy everything else and build
COPY . ./
WORKDIR /app/TestWebApi
RUN dotnet publish -c Release -o ../out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "TestWebApi.dll"]
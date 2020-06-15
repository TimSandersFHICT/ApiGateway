
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /UserAPI

ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["UserAPI.csproj", "./"]

RUN dotnet restore "UserAPI.csproj"
COPY . .

RUN dotnet build "UserAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserAPI.csproj" -c Release -o /app/publish

FROM base AS final

WORKDIR /UserAPI
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserAPI.dll"]

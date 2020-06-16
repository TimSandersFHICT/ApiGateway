FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
EXPOSE 8080

WORKDIR /KweetAPI


FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["KweetAPI.csproj", "./"]

RUN dotnet restore "KweetAPI.csproj"
COPY . .

RUN dotnet build "KweetAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KweetAPI.csproj" -c Release -o /app/publish

FROM base AS final

WORKDIR /KweetAPI

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "KweetAPI.dll"]

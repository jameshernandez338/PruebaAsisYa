FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Provider.Api/Provider.Api.csproj", "Provider.Api/"]
RUN dotnet restore "Provider.Api/Provider.Api.csproj"
COPY . .
WORKDIR "/src/Provider.Api"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "Provider.Api.dll"]

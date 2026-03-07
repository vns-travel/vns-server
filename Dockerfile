FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Presentation/Presentation.csproj", "Presentation/"]
COPY ["BLL/BLL.csproj", "BLL/"]
COPY ["DAL/DAL.csproj", "DAL/"]
RUN dotnet restore "Presentation/Presentation.csproj"

COPY . .
RUN dotnet publish "Presentation/Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 10000

ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-10000} dotnet Presentation.dll"]

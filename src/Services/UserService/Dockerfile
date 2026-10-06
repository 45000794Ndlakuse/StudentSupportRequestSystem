# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["UserService.csproj", "./"]

RUN dotnet restore "UserService.csproj"

COPY . .

RUN dotnet publish "UserService.csproj" -c Release -o /app/publish /p:UseAppHost=false


# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5263

ENTRYPOINT ["dotnet", "UserService.dll"]
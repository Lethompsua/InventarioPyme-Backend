FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore solo el csproj primero (cache de dependencias)
COPY InventarioPyme.Api/InventarioPyme.Api.csproj ./InventarioPyme.Api/
RUN dotnet restore ./InventarioPyme.Api/InventarioPyme.Api.csproj

# Copiar el resto y publicar
COPY . .
RUN dotnet publish ./InventarioPyme.Api/InventarioPyme.Api.csproj \
    -c Release -o /app/publish --no-restore

# Imagen final (solo runtime, más liviana)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

# Railway inyecta PORT como variable de entorno
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "InventarioPyme.Api.dll"]

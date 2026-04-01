# =============================
# Etapa 1: Build
# =============================
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./

RUN dotnet publish -c Release -o out

# =============================
# Etapa 2: Runtime
# =============================
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

COPY --from=build /app/out .

COPY certs /app/certs

EXPOSE 5000

ENTRYPOINT ["dotnet", "ServiceA.dll"]
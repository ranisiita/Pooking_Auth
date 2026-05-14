FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY src/ ./src/
COPY Microservicio.Pooking.Auth.slnx ./

RUN dotnet restore src/Microservicio.Pooking.Auth.Api/Microservicio.Pooking.Auth.Api.csproj

RUN dotnet publish src/Microservicio.Pooking.Auth.Api/Microservicio.Pooking.Auth.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Microservicio.Pooking.Auth.Api.dll"]

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build

ENV ASPNETCORE_ENVIRONMENT Docker
ENV ASPNETCORE_URLS "https://*;http://*"

EXPOSE 80
EXPOSE 443

WORKDIR /api/
COPY ./ .
RUN dotnet build
RUN dotnet publish Sample.HealthCheck.UI/Sample.HealthCheck.UI.csproj -c Release -o /api/publish/

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /api
COPY --from=build /api/publish/ .
RUN ls /api
ENTRYPOINT [ "dotnet" , "Sample.HealthCheck.UI.dll" ]

FROM mcr.microsoft.com/dotnet/core/sdk:latest as build

ENV ASPNETCORE_ENVIRONMENT Docker
ENV ASPNETCORE_URLS "https://*:443;http://*:80"

EXPOSE 80
EXPOSE 443

WORKDIR /api/
COPY ./ .
RUN dotnet build
RUN dotnet publish -c Release -o /api/publish/

FROM mcr.microsoft.com/dotnet/core/aspnet:latest
WORKDIR /api
COPY --from=build /api/publish/ .
ENTRYPOINT [ "dotnet" , "Sample.Api.dll" ]

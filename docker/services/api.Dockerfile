FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build

ENV ASPNETCORE_ENVIRONMENT Docker
ENV ASPNETCORE_URLS "https://api:443;http://api:80"

EXPOSE 80
EXPOSE 443

WORKDIR /api/
COPY ./ .
RUN dotnet build Sample.Api/Sample.Api.csproj -c Release
RUN dotnet publish Sample.Api/Sample.Api.csproj -c Release -o /api/publish/

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /api
COPY --from=build /api/publish/ .
RUN ls /api
ENTRYPOINT [ "dotnet" , "Sample.Api.dll" ]

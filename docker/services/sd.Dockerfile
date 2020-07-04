FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build

ENV ASPNETCORE_ENVIRONMENT Docker
ENV ASPNETCORE_URLS "https://sd:443;http://sd:80"

EXPOSE 80
EXPOSE 443

WORKDIR /sd/
COPY ./ .
RUN dotnet build Sample.ApiGateway/Sample.ApiGateway.csproj -c Release
RUN dotnet publish Sample.ApiGateway/Sample.ApiGateway.csproj -c Release -o /sd/publish/

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /sd
COPY --from=build /sd/publish/ .
RUN ls /sd
ENTRYPOINT [ "dotnet" , "Sample.ApiGateway.dll" ]

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build

ENV ASPNETCORE_ENVIRONMENT Docker

EXPOSE 80

WORKDIR /api/
COPY ./ .
RUN dotnet build Sample.MessageRelay/Sample.MessageRelay.csproj -c Release
RUN dotnet publish Sample.MessageRelay/Sample.MessageRelay.csproj -c Release -o /api/publish/

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /api
COPY --from=build /api/publish/ .
RUN ls /api
ENTRYPOINT [ "dotnet" , "Sample.MessageRelay.dll" ]

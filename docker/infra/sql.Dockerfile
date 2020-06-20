FROM mcr.microsoft.com/mssql/server:2019-GDR1-ubuntu-16.04
ENV SA_PASSWORD P@assw0rd
ENV ACCEPT_EULA Y

EXPOSE 1433
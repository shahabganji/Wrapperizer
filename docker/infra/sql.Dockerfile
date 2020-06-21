FROM mcr.microsoft.com/mssql/server:2019-CU3-ubuntu-18.04
ENV SA_PASSWORD P@assw0rd
ENV ACCEPT_EULA Y

EXPOSE 1433

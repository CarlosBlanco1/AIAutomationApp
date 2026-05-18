FROM mcr.microsoft.com/dotnet/sdk:latest

COPY . .

EXPOSE 5257

RUN dotnet restore

CMD [ "dotnet run" ]
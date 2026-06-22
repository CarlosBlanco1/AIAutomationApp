FROM mcr.microsoft.com/dotnet/sdk:10.0 as build

WORKDIR /src

COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o ./app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 as final

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT [ "dotnet", "app-api.dll" ]
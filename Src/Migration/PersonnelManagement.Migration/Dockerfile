FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Src/Migration/PersonnelManagement.Migration/PersonnelManagement.Migration.csproj", "PersonnelManagement.Migration/"]
RUN dotnet restore "Src/Migration/PersonnelManagement.Migration/PersonnelManagement.Migration.csproj"
COPY . .
WORKDIR "/src/PersonnelManagement.Migration"
RUN dotnet build "PersonnelManagement.Migration.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PersonnelManagement.Migration.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PersonnelManagement.Migration.dll"]

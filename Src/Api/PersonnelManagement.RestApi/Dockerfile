FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Src/Api/PersonnelManagement.RestApi/PersonnelManagement.RestApi.csproj", "PersonnelManagement.RestApi/"]
RUN dotnet restore "Src/Api/PersonnelManagement.RestApi/PersonnelManagement.RestApi.csproj"
COPY . .
WORKDIR "/src/PersonnelManagement.RestApi"
RUN dotnet build "PersonnelManagement.RestApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PersonnelManagement.RestApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PersonnelManagement.RestApi.dll"]

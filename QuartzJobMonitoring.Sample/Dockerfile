FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["QuartzJobMonitoring.Sample/QuartzJobMonitoring.Sample.csproj", "QuartzJobMonitoring.Sample/"]
RUN dotnet restore "QuartzJobMonitoring.Sample/QuartzJobMonitoring.Sample.csproj"
COPY . .
WORKDIR "/src/QuartzJobMonitoring.Sample"
RUN dotnet build "QuartzJobMonitoring.Sample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QuartzJobMonitoring.Sample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuartzJobMonitoring.Sample.dll"]
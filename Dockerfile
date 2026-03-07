FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY ["Presentation/Randevu365.Api/Randevu365.Api.csproj", "Presentation/Randevu365.Api/"]
COPY ["Core/Randevu365.Application/Randevu365.Application.csproj", "Core/Randevu365.Application/"]
COPY ["Core/Randevu365.Domain/Randevu365.Domain.csproj", "Core/Randevu365.Domain/"]
COPY ["Infrastructure/Randevu365.Infrastructure/Randevu365.Infrastructure.csproj", "Infrastructure/Randevu365.Infrastructure/"]
COPY ["Infrastructure/Randevu365.Persistence/Randevu365.Persistence.csproj", "Infrastructure/Randevu365.Persistence/"]
RUN dotnet restore "Presentation/Randevu365.Api/Randevu365.Api.csproj"

COPY . .
WORKDIR "/src/Presentation/Randevu365.Api"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Randevu365.Api.dll"]

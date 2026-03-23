FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos de projeto da pasta 'api'
COPY ["api/ControleGastos.API/ControleGastos.API.csproj", "api/ControleGastos.API/"]
COPY ["api/ControleGastos.Infrastructure/ControleGastos.Infrastructure.csproj", "api/ControleGastos.Infrastructure/"]
COPY ["api/ControleGastos.Application/ControleGastos.Application.csproj", "api/ControleGastos.Application/"]
COPY ["api/ControleGastos.Domain/ControleGastos.Domain.csproj", "api/ControleGastos.Domain/"]

# Restaura as dependências
RUN dotnet restore "api/ControleGastos.API/ControleGastos.API.csproj"

# Copia todo o resto
COPY . .

# Muda o diretório de trabalho para onde está o projeto principal da API
WORKDIR "/src/api/ControleGastos.API"
RUN dotnet publish "ControleGastos.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Cria a pasta para persistência
RUN mkdir -p /app/data

# Copia os arquivos publicados
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "ControleGastos.API.dll"]
# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia todo o código fonte para a imagem
COPY . .

# Restaura as dependências e publica a aplicação
RUN dotnet restore "src/Infnet.PesqMgm.Api/Infnet.PesqMgm.Api.csproj"
RUN dotnet publish "src/Infnet.PesqMgm.Api/Infnet.PesqMgm.Api.csproj" -c Release -o /app/publish

# Estágio de Runtime (Imagem mais leve apenas para rodar)
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Configura a porta que o Railway espera (padrão .NET container é 8080)
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Comando de entrada
ENTRYPOINT ["dotnet", "Infnet.PesqMgm.Api.dll"]
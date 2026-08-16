FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

USER root

WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/services/GestaoFinanceira/PlataformaEducacao.GestaoFinanceira.Api/PlataformaEducacao.GestaoFinanceira.Api.csproj", "src/services/GestaoFinanceira/PlataformaEducacao.GestaoFinanceira.Api/"]
COPY ["src/building-blocks/PlataformaEducacao.Core/PlataformaEducacao.Core.csproj", "src/building-blocks/PlataformaEducacao.Core/"]
COPY ["src/building-blocks/PlataformaEducacao.MessageBus/PlataformaEducacao.MessageBus.csproj", "src/building-blocks/PlataformaEducacao.MessageBus/"]
COPY ["src/building-blocks/PlataformaEducacao.WebApi.Core/PlataformaEducacao.WebApi.Core.csproj", "src/building-blocks/PlataformaEducacao.WebApi.Core/"]

RUN dotnet restore "src/services/GestaoFinanceira/PlataformaEducacao.GestaoFinanceira.Api/PlataformaEducacao.GestaoFinanceira.Api.csproj"

COPY . .

WORKDIR "/src/src/services/GestaoFinanceira/PlataformaEducacao.GestaoFinanceira.Api"
FROM build AS publish
RUN dotnet publish "PlataformaEducacao.GestaoFinanceira.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

ARG BUILD_DATE="unknown"
ARG VCS_REF="unknown"
ARG VERSION="dev"
LABEL org.opencontainers.image.title="gestao-financeira-api" \
      org.opencontainers.image.description="Gestão Financeira API (.NET 8)" \
      org.opencontainers.image.created=$BUILD_DATE \
      org.opencontainers.image.revision=$VCS_REF \
      org.opencontainers.image.version=$VERSION

COPY --from=publish /app/publish .

COPY docker/entrypoint.sh /entrypoint.sh

RUN chmod +x /entrypoint.sh \
    && mkdir -p /app/logs \
    && chown -R app:app /app

ENTRYPOINT ["/entrypoint.sh"]

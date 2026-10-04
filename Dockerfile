# Combined image: runs the .NET API, the SvelteKit frontend and the Angular backoffice
# in a single container. The API is internal-only, reached over loopback (127.0.0.1:8080).

# ---------- Backend build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend-build
WORKDIR /src
COPY Backend/src/ src/
RUN dotnet restore "src/EstudaKi.slnx"
WORKDIR /src/src/Web/Estudaki.Api
RUN dotnet publish "Estudaki.Api.csproj" -c Release -o /app/backend --no-restore

# ---------- Frontend build ----------
FROM node:22-bookworm-slim AS frontend-build
WORKDIR /app
COPY Frontend/package.json Frontend/package-lock.json ./
RUN npm ci
COPY Frontend/ .
# Baked in at build time: $env/static/private is inlined by Vite, not readable at runtime.
ENV API_URL=http://127.0.0.1:8080
RUN npm run build && npm prune --omit=dev

# ---------- Backoffice build ----------
FROM node:22-bookworm-slim AS backoffice-build
WORKDIR /app
COPY Backoffice/package.json Backoffice/package-lock.json ./
# npm ci: Angular's Vite/Rolldown toolchain pulls in OS-specific optional native
# bindings that aren't always fully captured in a lockfile generated on Windows.
RUN npm install
COPY Backoffice/ .
RUN npm run build && npm prune --omit=dev

# ---------- Final ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
# Node.js runtime, needed to run the SvelteKit frontend alongside the API.
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates curl gnupg \
    && mkdir -p /etc/apt/keyrings \
    && curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg \
    && echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_22.x nodistro main" > /etc/apt/sources.list.d/nodesource.list \
    && apt-get update \
    && apt-get install -y --no-install-recommends nodejs \
    && apt-get purge -y gnupg && apt-get autoremove -y \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=backend-build /app/backend ./backend
COPY --from=frontend-build /app/build ./frontend/build
COPY --from=frontend-build /app/node_modules ./frontend/node_modules
COPY --from=backoffice-build /app/dist/estudaki-backoffice ./backoffice
COPY --from=backoffice-build /app/node_modules ./backoffice/node_modules
COPY entrypoint.sh ./entrypoint.sh
RUN chmod +x ./entrypoint.sh && chown -R $APP_UID:$APP_UID /app

USER $APP_UID
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    NODE_ENV=production \
    API_URL=http://127.0.0.1:8080

# Only the frontend (3000) and backoffice (4000) ports are published;
# the API is only reachable via loopback inside the container.
EXPOSE 3000 4000
ENTRYPOINT ["./entrypoint.sh"]

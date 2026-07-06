# Stage 1: Build
FROM node:22-alpine AS build
WORKDIR /app
RUN corepack enable && corepack prepare pnpm@10 --activate

COPY frontend/pnpm-workspace.yaml frontend/pnpm-lock.yaml ./
COPY frontend/apps/admin/package.json apps/admin/
COPY frontend/packages/ui/package.json packages/ui/
COPY frontend/packages/api/package.json packages/api/
COPY frontend/packages/utils/package.json packages/utils/

RUN pnpm install --frozen-lockfile

COPY frontend/ .
RUN pnpm --filter admin build

# Stage 2: Serve via nginx
FROM nginx:alpine
COPY --from=build /app/apps/admin/dist /usr/share/nginx/html
COPY docker/nginx/frontend.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

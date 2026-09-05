
FROM node:22-alpine AS build
WORKDIR /src

COPY clinical-appointment-app/package.json clinical-appointment-app/package-lock.json ./
RUN npm ci

COPY clinical-appointment-app/ ./
RUN npm run build -- --configuration production

FROM nginx:alpine AS runtime

COPY docker/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /src/dist/clinical-appointment-app/browser /usr/share/nginx/html

EXPOSE 80

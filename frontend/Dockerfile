FROM node:20-alpine AS build
WORKDIR /app

COPY package.json .
RUN npm install

COPY tsconfig.json .
COPY ts/ ./ts/
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html/dist
COPY css/ /usr/share/nginx/html/css/
COPY pages/ /usr/share/nginx/html/pages/
COPY nginx.conf /etc/nginx/conf.d/default.conf

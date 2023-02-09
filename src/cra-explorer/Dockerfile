FROM node:lts-alpine as build

WORKDIR /usr/src/app
COPY package.json package-lock.json ./
RUN npm install
COPY . ./
RUN npm run build

FROM nginx:stable-alpine as base
COPY --from=build /usr/src/app/build /usr/share/nginx/html

RUN rm /etc/nginx/conf.d/default.conf
COPY nginx/nginx.conf /etc/nginx/conf.d

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

FROM node:20-alpine3.22 AS builder
RUN mkdir -p /project
WORKDIR /project
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
RUN npx ng build --configuration=production

FROM nginx:alpine3.22-perl
COPY ./nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=builder /project/dist/browser /usr/share/nginx/html
COPY ./entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh
ENTRYPOINT [ "/entrypoint.sh" ]

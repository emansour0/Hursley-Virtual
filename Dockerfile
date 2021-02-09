FROM nginx:alpine

WORKDIR /webgl
COPY WebGL/WebGL .

WORKDIR /etc/nginx/conf.d
RUN rm default.conf
COPY webgl.conf webgl.conf
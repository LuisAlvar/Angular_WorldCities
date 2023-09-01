# syntax=docker/dockerfile:1.4

FROM node as builder
WORKDIR /app
EXPOSE 90
RUN npm install -g @angular/cli@13.0.1

COPY package.json ./
COPY package-lock.json ./
RUN npm ci
COPY  . .
CMD ["node","aspnetcore-https"]
CMD ["ng","serve","--ssl","-ssl-cert", "%APPDATA%\\ASP.NET\\https\\%npm_package_name%.pem", "--ssl-key","%APPDATA%\\ASP.NET\\https\\%npm_package_name%.key","--host=90"]

FROM builder as dev-envs
RUN apt-get update && \
  apt-get install -y --no-install-recommends git && \
  useradd -s /bin/bash -m vscode &&\
  groupadd docker && \
  usermod -aG docker vscode

COPY --from=gloursdocker/docker / /

CMD ["npm", "start"]

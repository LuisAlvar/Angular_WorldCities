FROM node:16.14 AS client
EXPOSE 4200 49153
USER node

RUN mkdir /home/node/.npm-global
ENV PATH=/home/node/.npm-global/bin:$PATH
ENV NPM_CONFIG_PREFIX=/home/node/.npm-global

RUN npm install -g @angular/cli@13.0.1

WORKDIR /app
CMD ["ng","serve", "--port", "4200", "--host","0.0.0.0","--disable-host-check","--poll","2000"]

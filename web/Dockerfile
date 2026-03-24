FROM node:20-alpine

WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .

EXPOSE 5173

# O host 0.0.0.0 é vital para o Docker conseguir expor a porta
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]
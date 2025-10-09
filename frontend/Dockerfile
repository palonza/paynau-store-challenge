# ===========================
# Build stage
# ===========================
FROM node:20-alpine AS build
WORKDIR /app

# Copiar definición de dependencias
COPY package*.json ./

# Instalar dependencias exactas según package-lock.json
RUN npm ci

# Copiar el resto del proyecto
COPY . .

# Construir la aplicación (VITE_API_BASE_URL se inyecta desde el entorno)
RUN npm run build

# ===========================
# Runtime stage
# ===========================
FROM nginx:alpine AS runtime

# Copiar archivos compilados desde el build anterior
COPY --from=build /app/dist /usr/share/nginx/html

# Exponer el puerto del servidor
EXPOSE 3001

# Configurar NGINX para escuchar en el puerto 3001
RUN sed -i 's/listen       80;/listen 3001;/' /etc/nginx/conf.d/default.conf

# Iniciar el servidor NGINX
CMD ["nginx", "-g", "daemon off;"]

# Paynau Frontend

Frontend del proyecto Paynau Store construido con React + Vite + TypeScript + Material UI.

## 🎯 Tecnologías

- **React 19** con TypeScript
- **Vite** para desarrollo y build
- **Material UI (MUI)** para componentes de UI
- **React Router** para navegación
- **Cliente TypeScript** generado desde OpenAPI

## 📋 Requisitos Previos

- Node.js 20+
- npm o yarn
- Backend Paynau corriendo en `http://localhost:5001`

## 🚀 Instalación y Ejecución

### 1. Instalar dependencias

```bash
npm install
```

### 2. Configurar variables de entorno

Crear archivo `.env` en la raíz del proyecto:

```env
VITE_API_BASE_URL=http://localhost:5001
VITE_APP_ENV=development
```

### 3. Ejecutar en modo desarrollo

```bash
npm run dev
```

La aplicación estará disponible en `http://localhost:5173`

### 4. Build para producción

```bash
npm run build
```

### 5. Preview del build

```bash
npm run preview
```

Esto levantará un servidor en `http://localhost:4173`

## 📁 Estructura del Proyecto

```
src/
├── api/
│   └── client/           # Cliente TypeScript generado desde OpenAPI
├── components/           # Componentes presentacionales (UI pura)
│   ├── common/
│   ├── orders/
│   └── products/
├── containers/           # Containers (lógica + estado)
│   ├── OrdersContainer.tsx
│   └── ProductsContainer.tsx
├── hooks/                # Custom hooks
│   └── useToast.tsx
├── pages/                # Páginas principales
│   ├── HomePage.tsx
│   ├── OrdersPage.tsx
│   └── ProductsPage.tsx
├── routes/               # Configuración de rutas
│   └── AppRoutes.tsx
├── theme/                # Configuración de tema MUI
│   └── ThemeContext.tsx
├── types/                # Tipos TypeScript extendidos
│   └── extended.ts
├── main.tsx              # Entry point
└── App.tsx               # Componente raíz
```

## 🎨 Patrón de Arquitectura

El proyecto sigue el patrón **Containers & Components**:

- **Components**: Componentes presentacionales puros (props in → UI out)
- **Containers**: Orquestan datos, estado y llamadas a API
- **Pages**: Páginas que renderizan containers

## 🔌 Conexión con el Backend

El frontend consume la API REST del backend a través de:

- Cliente TypeScript auto-generado desde `openapi.json`
- Base URL configurable vía `VITE_API_BASE_URL`
- Endpoints principales:
  - `GET/POST /api/Products`
  - `GET/PUT/DELETE /api/Products/{id}`
  - `GET/POST /api/Orders`

## ✅ Funcionalidades Implementadas

### Productos
- ✅ Listar productos con paginación (5 por página)
- ✅ Crear nuevos productos
- ✅ Editar productos existentes
- ✅ Eliminar productos
- ✅ Ver detalle de producto
- ✅ Validaciones en formulario
- ✅ Manejo de estados (loading/error/success)
- ✅ Feedback con toasts

### Órdenes
- ✅ Listar órdenes con paginación (5 por página)
- ✅ Crear nuevas órdenes
- ✅ Ver detalle de orden
- ✅ Validación de stock disponible
- ✅ Actualización automática de stock
- ✅ Manejo de estados (loading/error/success)
- ✅ Feedback con toasts

## 🧪 Scripts Disponibles

```json
{
  "dev": "vite",                    // Modo desarrollo
  "build": "tsc -b && vite build",  // Build producción
  "lint": "eslint .",               // Linter
  "preview": "vite preview"         // Preview del build
}
```

## 🎨 Temas

La aplicación soporta modo claro/oscuro mediante el toggle en el header.
El tema se persiste en `localStorage`.

## 🔒 Seguridad

- El backend genera JWT por request
- No se requiere Authorization header desde el frontend
- Todas las validaciones se replican en cliente y servidor

## 🐳 Docker

El proyecto está preparado para correr con Docker:

```bash
docker-compose up
```

El frontend estará disponible en el puerto `3001`.

## 📝 Notas Importantes

- Todas las validaciones de formularios se ejecutan solo al enviar
- Los mensajes de error están en español
- La paginación es fija de 5 elementos por página
- No se requieren confirmaciones para acciones críticas (feedback posterior)
- El cliente API usa tipado fuerte (sin `any`)

## 🤝 Contribución

1. Mantener el patrón Containers & Components
2. No modificar componentes presentacionales existentes
3. Usar el cliente API generado (no `fetch` manual)
4. Mantener mensajes en español
5. Tipado fuerte en todo el código
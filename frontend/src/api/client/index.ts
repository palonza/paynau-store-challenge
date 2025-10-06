// Cliente TypeScript generado automáticamente desde openapi.json

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5001';

// ==================== DTOs ====================

export interface ProductDto {
  id: number;
  name: string;
  description: string;
  price: number;
  stock: number;
}

export interface CreateProductDto {
  name: string;
  description: string;
  price: number;
  stock: number;
}

export interface UpdateProductDto {
  name: string;
  description: string;
  price: number;
  stock: number;
}

export interface OrderDto {
  id: number;
  productId: number;
  quantity: number;
  total: number;
  createdAt: string;
}

export interface CreateOrderDto {
  productId: number;
  quantity: number;
}

export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
}

// ==================== Errores personalizados ====================

export class ApiError extends Error {
  status: number;
  statusText: string;
  problemDetails?: ProblemDetails;

  constructor(status: number, statusText: string, problemDetails?: ProblemDetails) {
    super(problemDetails?.detail || statusText);
    this.name = 'ApiError';
    this.status = status;
    this.statusText = statusText;
    this.problemDetails = problemDetails;
  }
}

// ==================== Utilidades ====================

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    let problemDetails: ProblemDetails | undefined;
    
    try {
      const contentType = response.headers.get('content-type');
      if (contentType && contentType.includes('application/json')) {
        problemDetails = await response.json();
      }
    } catch {
      // Si no se puede parsear el JSON, continuamos sin problemDetails
    }

    throw new ApiError(response.status, response.statusText, problemDetails);
  }

  // 204 No Content no tiene body
  if (response.status === 204) {
    return undefined as T;
  }

  const contentType = response.headers.get('content-type');
  if (contentType && contentType.includes('application/json')) {
    return response.json();
  }

  return undefined as T;
}

async function request<T>(
  endpoint: string,
  options?: RequestInit
): Promise<T> {
  const url = `${API_BASE_URL}${endpoint}`;
  
  const defaultHeaders: HeadersInit = {
    'Content-Type': 'application/json',
  };

  const config: RequestInit = {
    ...options,
    headers: {
      ...defaultHeaders,
      ...options?.headers,
    },
  };

  const response = await fetch(url, config);
  return handleResponse<T>(response);
}

// ==================== Clientes de API ====================

export const productsApi = {
  async getAll(): Promise<ProductDto[]> {
    return request<ProductDto[]>('/api/Products', {
      method: 'GET',
    });
  },

  async getById(id: number): Promise<ProductDto> {
    return request<ProductDto>(`/api/Products/${id}`, {
      method: 'GET',
    });
  },

  async create(data: CreateProductDto): Promise<ProductDto> {
    return request<ProductDto>('/api/Products', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  },

  async update(id: number, data: UpdateProductDto): Promise<ProductDto> {
    return request<ProductDto>(`/api/Products/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  },

  async delete(id: number): Promise<void> {
    return request<void>(`/api/Products/${id}`, {
      method: 'DELETE',
    });
  },
};

export const ordersApi = {
  async getAll(): Promise<OrderDto[]> {
    return request<OrderDto[]>('/api/Orders', {
      method: 'GET',
    });
  },

  async create(data: CreateOrderDto): Promise<OrderDto> {
    return request<OrderDto>('/api/Orders', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  },
};
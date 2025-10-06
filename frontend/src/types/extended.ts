// src/types/extended.ts

// --- Tipos de dominio base ---
export interface ProductDto {
  id: number;
  name: string;
  description: string;
  price: number;
  stock: number;
}

// --- Formularios ---
export interface ProductFormData {
  name: string;
  description: string;
  price: number;
  stock: number;
}

// --- Órdenes ---
export type OrderStatus = 'pending' | 'completed' | 'cancelled';

export interface OrderViewDto {
  id: number;
  productId: number;
  productName?: string;
  quantity: number;
  total: number;
  createdAt: string;
  status: OrderStatus;
}
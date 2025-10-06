import { OrderDto, ProductDto } from '../api/client';

export type OrderStatus = 'pending' | 'completed' | 'cancelled';

export interface OrderViewDto extends OrderDto {
  status: OrderStatus;
  productName?: string; // Opcional, por si el container resuelve el nombre del producto
}

export interface ProductFormData {
  name: string;
  description: string;
  price: number;
  stock: number;
}
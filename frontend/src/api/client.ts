// src/api/client.ts
export interface ProductDto {
  id: number;
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
  date: string;
}

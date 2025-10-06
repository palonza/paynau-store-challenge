import React, { useState, useEffect, useCallback } from 'react';
import { Box, Button, Dialog, DialogTitle, DialogContent, Typography } from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import { OrdersTable } from '../components/orders/OrdersTable';
import { OrderForm } from '../components/orders/OrderForm';
import { useToast } from '../hooks/useToast';
import { ordersApi, productsApi, ApiError } from '../api/client';
import type { ProductDto } from '../api/client';
import type { OrderViewDto } from '../types/extended';

export const OrdersContainer: React.FC = () => {
  const [orders, setOrders] = useState<OrderViewDto[]>([]);
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(0);
  const [rowsPerPage] = useState(5);
  const [cancellingIds, setCancellingIds] = useState<number[]>([]);

  // Modal de formulario
  const [formOpen, setFormOpen] = useState(false);
  const [formLoading, setFormLoading] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);

  // Modal de vista detalle
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [viewingOrder, setViewingOrder] = useState<OrderViewDto | null>(null);

  const { showToast, ToastComponent } = useToast();

  // Cargar órdenes y productos
  const fetchOrders = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const [ordersData, productsData] = await Promise.all([
        ordersApi.getAll(),
        productsApi.getAll(),
      ]);

      // Convertir OrderDto a OrderViewDto con información del producto
      const ordersWithProducts: OrderViewDto[] = ordersData.map((order) => {
        const product = productsData.find((p) => p.id === order.productId);
        return {
          ...order,
          productName: product?.name,
          status: 'pending' as const, // Por defecto, las órdenes están pendientes
        };
      });

      setOrders(ordersWithProducts);
      setProducts(productsData);
    } catch (err) {
      const message = err instanceof ApiError
        ? err.problemDetails?.detail || 'Error al cargar órdenes'
        : 'Error al cargar órdenes';
      setError(message);
      showToast(message, 'error');
    } finally {
      setLoading(false);
    }
  }, [showToast]);

  useEffect(() => {
    fetchOrders();
  }, [fetchOrders]);

  // Handlers de tabla
  const handlePageChange = useCallback((_event: unknown, newPage: number) => {
    setPage(newPage);
  }, []);

  const handleRowsPerPageChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
    setPage(0);
  }, []);

  const handleView = useCallback((order: OrderViewDto) => {
    setViewingOrder(order);
    setViewDialogOpen(true);
  }, []);

  const handleCancel = useCallback(async (orderId: number) => {
    setCancellingIds((prev) => [...prev, orderId]);

    try {
      // Nota: El backend actual no tiene endpoint de cancelación,
      // por lo que esta implementación es preparatoria para cuando se agregue
      showToast('Funcionalidad de cancelación pendiente de implementación en el backend.', 'info');
      
      // await ordersApi.cancel(orderId);
      // showToast('Orden cancelada correctamente.', 'success');
      // await fetchOrders();
    } catch (err) {
      const message = err instanceof ApiError
        ? err.problemDetails?.detail || 'No se pudo cancelar la orden'
        : 'No se pudo cancelar la orden';
      showToast(message, 'error');
    } finally {
      setCancellingIds((prev) => prev.filter((id) => id !== orderId));
    }
  }, [showToast]);

  // Handlers de formulario
  const handleOpenCreateForm = useCallback(() => {
    setFormError(null);
    setFormOpen(true);
  }, []);

  const handleCloseForm = useCallback(() => {
    if (!formLoading) {
      setFormOpen(false);
      setFormError(null);
    }
  }, [formLoading]);

  const handleSubmitForm = useCallback(
    async (data: { productId: number; quantity: number }) => {
      setFormLoading(true);
      setFormError(null);

      try {
        await ordersApi.create(data);
        showToast('Orden creada correctamente.', 'success');
        setFormOpen(false);
        await fetchOrders();
      } catch (err) {
        const message = err instanceof ApiError
          ? err.problemDetails?.detail || 'Error al crear la orden'
          : 'Error al crear la orden';
        setFormError(message);
        showToast(message, 'error');
      } finally {
        setFormLoading(false);
      }
    },
    [fetchOrders, showToast]
  );

  // Paginación
  const paginatedOrders = orders.slice(
    page * rowsPerPage,
    page * rowsPerPage + rowsPerPage
  );

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h4" component="h1" fontWeight="bold">
          Gestión de Órdenes
        </Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={handleOpenCreateForm}
          disabled={loading || products.length === 0}
        >
          Crear Orden
        </Button>
      </Box>

      <OrdersTable
        orders={paginatedOrders}
        loading={loading}
        error={error}
        page={page}
        rowsPerPage={rowsPerPage}
        totalCount={orders.length}
        onPageChange={handlePageChange}
        onRowsPerPageChange={handleRowsPerPageChange}
        onView={handleView}
        onCancel={handleCancel}
        cancellingIds={cancellingIds}
      />

      <OrderForm
        open={formOpen}
        onClose={handleCloseForm}
        onSubmit={handleSubmitForm}
        loading={formLoading}
        error={formError}
        products={products}
      />

      <Dialog
        open={viewDialogOpen}
        onClose={() => setViewDialogOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Detalle de la Orden</DialogTitle>
        <DialogContent>
          {viewingOrder && (
            <Box sx={{ pt: 2 }}>
              <Typography variant="body1" gutterBottom>
                <strong>ID:</strong> {viewingOrder.id}
              </Typography>
              <Typography variant="body1" gutterBottom>
                <strong>Producto:</strong> {viewingOrder.productName || `#${viewingOrder.productId}`}
              </Typography>
              <Typography variant="body1" gutterBottom>
                <strong>Cantidad:</strong> {viewingOrder.quantity}
              </Typography>
              <Typography variant="body1" gutterBottom>
                <strong>Total:</strong> ${viewingOrder.total.toFixed(2)}
              </Typography>
              <Typography variant="body1" gutterBottom>
                <strong>Fecha:</strong>{' '}
                {new Date(viewingOrder.createdAt).toLocaleString('es-AR')}
              </Typography>
              <Typography variant="body1" gutterBottom>
                <strong>Estado:</strong> {viewingOrder.status === 'pending' ? 'Pendiente' : viewingOrder.status === 'completed' ? 'Completada' : 'Cancelada'}
              </Typography>
            </Box>
          )}
        </DialogContent>
      </Dialog>

      <ToastComponent />
    </Box>
  );
};
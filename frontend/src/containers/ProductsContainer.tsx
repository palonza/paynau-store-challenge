// src/containers/ProductsContainer.tsx
import React, { useState, useEffect, useCallback } from 'react';
import { Box, Button, Dialog, DialogTitle, DialogContent, Typography } from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import { ProductsTable } from '../components/products/ProductsTable';
import { ProductForm } from '../components/products/ProductForm';
import { productsApi, ApiError } from '../api/client';
import type { ProductDto } from '../api/client';
import type { ProductFormData } from '../types/extended';
import { useToast } from '../hooks/useToast';

export const ProductsContainer: React.FC = () => {
  console.log("🟢 ProductsContainer montado"); // 👈 log inmediato
  
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(0);
  const [rowsPerPage] = useState(5);
  const [deletingIds, setDeletingIds] = useState<number[]>([]);
  
  // Modal de formulario
  const [formOpen, setFormOpen] = useState(false);
  const [formLoading, setFormLoading] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [editingProduct, setEditingProduct] = useState<ProductDto | null>(null);
  
  // Modal de vista detalle
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [viewingProduct, setViewingProduct] = useState<ProductDto | null>(null);

  const { showToast, ToastComponent } = useToast();

  // Cargar productos
  const fetchProducts = useCallback(async () => {
    console.log("API base URL:", import.meta.env.VITE_API_BASE_URL);

    try {
      setLoading(true);
      setError(null);
      const data = await productsApi.getAll();
      setProducts(data);
    } catch (err) {
      const message = err instanceof ApiError 
        ? err.problemDetails?.detail || 'Error al cargar productos'
        : 'Error al cargar productos';
      setError(message);
      showToast(message, 'error');
    } finally {
      setLoading(false);
    }
  }, [showToast]);

  useEffect(() => {
    fetchProducts();
  }, [fetchProducts]);

  // Handlers de tabla
  const handlePageChange = useCallback((_event: unknown, newPage: number) => {
    setPage(newPage);
  }, []);

  const handleRowsPerPageChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
    setPage(0);
  }, []);

  const handleView = useCallback((product: ProductDto) => {
    setViewingProduct(product);
    setViewDialogOpen(true);
  }, []);

  const handleEdit = useCallback((product: ProductDto) => {
    setEditingProduct(product);
    setFormError(null);
    setFormOpen(true);
  }, []);

  const handleDelete = useCallback(async (productId: number) => {
    setDeletingIds((prev) => [...prev, productId]);
    
    try {
      await productsApi.delete(productId);
      showToast('Producto eliminado correctamente.', 'success');
      await fetchProducts();
    } catch (err) {
      const message = err instanceof ApiError
        ? err.problemDetails?.detail || 'Error al eliminar el producto'
        : 'Error al eliminar el producto';
      showToast(message, 'error');
    } finally {
      setDeletingIds((prev) => prev.filter((id) => id !== productId));
    }
  }, [fetchProducts, showToast]);

  // Handlers de formulario
  const handleOpenCreateForm = useCallback(() => {
    setEditingProduct(null);
    setFormError(null);
    setFormOpen(true);
  }, []);

  const handleCloseForm = useCallback(() => {
    if (!formLoading) {
      setFormOpen(false);
      setEditingProduct(null);
      setFormError(null);
    }
  }, [formLoading]);

  const handleSubmitForm = useCallback(async (data: ProductFormData) => {
    setFormLoading(true);
    setFormError(null);

    try {
      if (editingProduct) {
        // Actualizar
        await productsApi.update(editingProduct.id, data);
        showToast('Producto actualizado con éxito.', 'success');
      } else {
        // Crear
        await productsApi.create(data);
        showToast('Producto guardado con éxito.', 'success');
      }
      
      setFormOpen(false);
      setEditingProduct(null);
      await fetchProducts();
    } catch (err) {
      const message = err instanceof ApiError
        ? err.problemDetails?.detail || 'Error al guardar el producto'
        : 'Error al guardar el producto';
      setFormError(message);
      showToast(message, 'error');
    } finally {
      setFormLoading(false);
    }
  }, [editingProduct, fetchProducts, showToast]);

  // Paginación
  const paginatedProducts = products.slice(
    page * rowsPerPage,
    page * rowsPerPage + rowsPerPage
  );

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h4" component="h1" fontWeight="bold">
          Gestión de Productos
        </Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={handleOpenCreateForm}
          disabled={loading}
        >
          Crear Producto
        </Button>
      </Box>

      <ProductsTable
        rows={paginatedProducts}
        loading={loading}
        error={error}
        page={page}
        rowsPerPage={rowsPerPage}
        totalCount={products.length}
        onPageChange={handlePageChange}
        onRowsPerPageChange={handleRowsPerPageChange}
        onView={handleView}
        onEdit={handleEdit}
        onDelete={handleDelete}
        deletingIds={deletingIds}
      />

      <ProductForm
        open={formOpen}
        onClose={handleCloseForm}
        onSubmit={handleSubmitForm}
        loading={formLoading}
        error={formError}
        product={editingProduct}
      />

      <Dialog
        open={viewDialogOpen}
        onClose={() => setViewDialogOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Detalle del Producto</DialogTitle>
        <DialogContent>
          {viewingProduct && (
            <Box sx={{ pt: 2 }}>
              <Typography variant="body1" gutterBottom>
                <strong>Nombre:</strong> {viewingProduct.name}
              </Typography>
              <Typography variant="body1" gutterBottom>
                <strong>Descripción:</strong> {viewingProduct.description}
              </Typography>
              <Typography variant="body1" gutterBottom>
                <strong>Precio:</strong> ${viewingProduct.price.toFixed(2)}
              </Typography>
              <Typography variant="body1" gutterBottom>
                <strong>Stock:</strong> {viewingProduct.stock}
              </Typography>
            </Box>
          )}
        </DialogContent>
      </Dialog>

      <ToastComponent />
    </Box>
  );
};
import React, { useState, useEffect } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Button,
  Box,
  Alert,
  MenuItem,
  FormControl,
  InputLabel,
  Select,
  FormHelperText,
} from '@mui/material';
import type { SelectChangeEvent } from '@mui/material';
import type { ProductDto } from '../../api/client';

interface OrderFormProps {
  open: boolean;
  onClose: () => void;
  onSubmit: (data: { productId: number; quantity: number }) => void;
  loading: boolean;
  error: string | null;
  products: ProductDto[];
}

interface FormErrors {
  productId?: string;
  quantity?: string;
}

export const OrderForm: React.FC<OrderFormProps> = ({
  open,
  onClose,
  onSubmit,
  loading,
  error,
  products,
}) => {
  const [formData, setFormData] = useState({
    productId: 0,
    quantity: 1,
  });

  const [errors, setErrors] = useState<FormErrors>({});
  const [touched, setTouched] = useState<Record<string, boolean>>({});

  const selectedProduct = products.find(p => p.id === formData.productId);

  useEffect(() => {
    if (!open) {
      setFormData({ productId: 0, quantity: 1 });
      setErrors({});
      setTouched({});
    }
  }, [open]);

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};

    if (formData.productId === 0) {
      newErrors.productId = 'Debe seleccionar un producto';
    }

    if (formData.quantity <= 0) {
      newErrors.quantity = 'La cantidad debe ser mayor que 0';
    }

    if (selectedProduct && formData.quantity > selectedProduct.stock) {
      newErrors.quantity = `La cantidad no puede exceder el stock disponible (${selectedProduct.stock})`;
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    setTouched({
      productId: true,
      quantity: true,
    });

    if (validateForm()) {
      onSubmit(formData);
    }
  };

  const handleProductChange = (e: SelectChangeEvent<number>) => {
    const productId = Number(e.target.value);
    setFormData(prev => ({ ...prev, productId }));
  };

  const handleQuantityChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const quantity = parseInt(e.target.value) || 0;
    setFormData(prev => ({ ...prev, quantity }));
  };

  const handleBlur = (field: string) => () => {
    setTouched(prev => ({ ...prev, [field]: true }));
  };

  const handleClose = () => {
    if (!loading) {
      onClose();
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit}>
        <DialogTitle>Crear Orden</DialogTitle>
        <DialogContent>
          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
            <FormControl
              fullWidth
              error={touched.productId && Boolean(errors.productId)}
              disabled={loading}
            >
              <InputLabel id="product-select-label">Producto</InputLabel>
              <Select
                labelId="product-select-label"
                value={formData.productId}
                onChange={handleProductChange}
                onBlur={handleBlur('productId')}
                label="Producto"
                required
                inputProps={{
                  'aria-label': 'Seleccionar producto',
                }}
              >
                <MenuItem value={0} disabled>
                  Seleccione un producto
                </MenuItem>
                {products.map((product) => (
                  <MenuItem key={product.id} value={product.id}>
                    {product.name} - Stock: {product.stock} - ${product.price.toFixed(2)}
                  </MenuItem>
                ))}
              </Select>
              {touched.productId && errors.productId && (
                <FormHelperText>{errors.productId}</FormHelperText>
              )}
            </FormControl>

            <TextField
              label="Cantidad"
              type="number"
              value={formData.quantity}
              onChange={handleQuantityChange}
              onBlur={handleBlur('quantity')}
              error={touched.quantity && Boolean(errors.quantity)}
              helperText={touched.quantity && errors.quantity}
              required
              fullWidth
              disabled={loading || formData.productId === 0}
              inputProps={{
                step: '1',
                min: '1',
                max: selectedProduct?.stock || undefined,
                'aria-label': 'Cantidad de productos',
              }}
            />

            {selectedProduct && formData.quantity > 0 && (
              <Alert severity="info">
                Total: ${(selectedProduct.price * formData.quantity).toFixed(2)}
              </Alert>
            )}
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} disabled={loading}>
            Cancelar
          </Button>
          <Button
            type="submit"
            variant="contained"
            disabled={loading}
          >
            {loading ? 'Creando...' : 'Crear Orden'}
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
};
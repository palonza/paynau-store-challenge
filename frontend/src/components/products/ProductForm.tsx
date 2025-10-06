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
} from '@mui/material';
import { ProductDto } from '../../api/client';
import { ProductFormData } from '../../types/extended';

interface ProductFormProps {
  open: boolean;
  onClose: () => void;
  onSubmit: (data: ProductFormData) => void;
  loading: boolean;
  error: string | null;
  product?: ProductDto | null;
}

interface FormErrors {
  name?: string;
  description?: string;
  price?: string;
  stock?: string;
}

export const ProductForm: React.FC<ProductFormProps> = ({
  open,
  onClose,
  onSubmit,
  loading,
  error,
  product,
}) => {
  const isEditMode = Boolean(product);

  const [formData, setFormData] = useState<ProductFormData>({
    name: '',
    description: '',
    price: 0,
    stock: 0,
  });

  const [errors, setErrors] = useState<FormErrors>({});
  const [touched, setTouched] = useState<Record<string, boolean>>({});

  useEffect(() => {
    if (product) {
      setFormData({
        name: product.name || '',
        description: product.description || '',
        price: product.price,
        stock: product.stock,
      });
    } else {
      setFormData({
        name: '',
        description: '',
        price: 0,
        stock: 0,
      });
    }
    setErrors({});
    setTouched({});
  }, [product, open]);

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};

    // Validar name
    if (!formData.name.trim()) {
      newErrors.name = 'El nombre es requerido';
    } else if (formData.name.trim().length < 3) {
      newErrors.name = 'El nombre debe tener al menos 3 caracteres';
    } else if (formData.name.trim().length > 100) {
      newErrors.name = 'El nombre no puede exceder 100 caracteres';
    }

    // Validar description
    if (!formData.description.trim()) {
      newErrors.description = 'La descripción es requerida';
    }

    // Validar price
    if (formData.price <= 0) {
      newErrors.price = 'El precio debe ser mayor que 0';
    }

    // Validar stock
    if (formData.stock < 0) {
      newErrors.stock = 'El stock no puede ser negativo';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    // Marcar todos los campos como tocados
    setTouched({
      name: true,
      description: true,
      price: true,
      stock: true,
    });

    if (validateForm()) {
      onSubmit(formData);
    }
  };

  const handleChange = (field: keyof ProductFormData) => (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    const value = field === 'price' || field === 'stock' 
      ? parseFloat(e.target.value) || 0 
      : e.target.value;

    setFormData(prev => ({ ...prev, [field]: value }));
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
        <DialogTitle>
          {isEditMode ? 'Editar Producto' : 'Crear Producto'}
        </DialogTitle>
        <DialogContent>
          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
            <TextField
              label="Nombre"
              value={formData.name}
              onChange={handleChange('name')}
              onBlur={handleBlur('name')}
              error={touched.name && Boolean(errors.name)}
              helperText={touched.name && errors.name}
              required
              fullWidth
              disabled={loading}
              inputProps={{
                'aria-label': 'Nombre del producto',
              }}
            />
            <TextField
              label="Descripción"
              value={formData.description}
              onChange={handleChange('description')}
              onBlur={handleBlur('description')}
              error={touched.description && Boolean(errors.description)}
              helperText={touched.description && errors.description}
              required
              fullWidth
              multiline
              rows={3}
              disabled={loading}
              inputProps={{
                'aria-label': 'Descripción del producto',
              }}
            />
            <TextField
              label="Precio"
              type="number"
              value={formData.price}
              onChange={handleChange('price')}
              onBlur={handleBlur('price')}
              error={touched.price && Boolean(errors.price)}
              helperText={touched.price && errors.price}
              required
              fullWidth
              disabled={loading}
              inputProps={{
                step: '0.01',
                min: '0',
                'aria-label': 'Precio del producto',
              }}
            />
            <TextField
              label="Stock"
              type="number"
              value={formData.stock}
              onChange={handleChange('stock')}
              onBlur={handleBlur('stock')}
              error={touched.stock && Boolean(errors.stock)}
              helperText={touched.stock && errors.stock}
              required
              fullWidth
              disabled={loading}
              inputProps={{
                step: '1',
                min: '0',
                'aria-label': 'Stock del producto',
              }}
            />
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
            {loading ? 'Guardando...' : isEditMode ? 'Actualizar' : 'Crear'}
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
};
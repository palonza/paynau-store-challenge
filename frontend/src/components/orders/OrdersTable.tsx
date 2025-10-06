import React from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  IconButton,
  Tooltip,
  TablePagination,
  Skeleton,
  Alert,
  Box,
  Typography,
  Chip,
} from '@mui/material';
import {
  Visibility as VisibilityIcon,
  Cancel as CancelIcon,
} from '@mui/icons-material';
import type { OrderViewDto, OrderStatus } from '../../types/extended';

interface OrdersTableProps {
  orders: OrderViewDto[];
  loading: boolean;
  error: string | null;
  page: number;
  rowsPerPage: number;
  totalCount: number;
  onPageChange: (event: unknown, newPage: number) => void;
  onRowsPerPageChange: (event: React.ChangeEvent<HTMLInputElement>) => void;
  onView: (order: OrderViewDto) => void;
  onCancel: (orderId: number) => void;
  cancellingIds?: number[];
}

const getStatusLabel = (status: OrderStatus): string => {
  const labels: Record<OrderStatus, string> = {
    pending: 'Pendiente',
    completed: 'Completada',
    cancelled: 'Cancelada',
  };
  return labels[status];
};

const getStatusColor = (status: OrderStatus): 'warning' | 'success' | 'default' => {
  const colors: Record<OrderStatus, 'warning' | 'success' | 'default'> = {
    pending: 'warning',
    completed: 'success',
    cancelled: 'default',
  };
  return colors[status];
};

export const OrdersTable: React.FC<OrdersTableProps> = ({
  orders,
  loading,
  error,
  page,
  rowsPerPage,
  totalCount,
  onPageChange,
  onRowsPerPageChange,
  onView,
  onCancel,
  cancellingIds = [],
}) => {
  if (loading) {
    return (
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>Producto</TableCell>
              <TableCell align="right">Cantidad</TableCell>
              <TableCell align="right">Total</TableCell>
              <TableCell>Fecha</TableCell>
              <TableCell>Estado</TableCell>
              <TableCell align="right">Acciones</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {[...Array(5)].map((_, index) => (
              <TableRow key={index}>
                <TableCell><Skeleton variant="text" /></TableCell>
                <TableCell><Skeleton variant="text" /></TableCell>
                <TableCell><Skeleton variant="text" /></TableCell>
                <TableCell><Skeleton variant="text" /></TableCell>
                <TableCell><Skeleton variant="text" /></TableCell>
                <TableCell><Skeleton variant="text" /></TableCell>
                <TableCell><Skeleton variant="text" /></TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    );
  }

  if (error) {
    return (
      <Alert severity="error" sx={{ mt: 2 }}>
        {error}
      </Alert>
    );
  }

  if (orders.length === 0) {
    return (
      <Box sx={{ textAlign: 'center', py: 8 }}>
        <Typography variant="h6" color="text.secondary">
          No hay órdenes registradas
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
          Crea tu primera orden para comenzar
        </Typography>
      </Box>
    );
  }

  return (
    <>
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>Producto</TableCell>
              <TableCell align="right">Cantidad</TableCell>
              <TableCell align="right">Total</TableCell>
              <TableCell>Fecha</TableCell>
              <TableCell>Estado</TableCell>
              <TableCell align="right">Acciones</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {orders.map((order) => {
              const isCancelling = cancellingIds.includes(order.id);
              const canCancel = order.status === 'pending';
              const showCancelButton = order.status !== 'cancelled';

              const getCancelTooltip = (): string => {
                if (order.status === 'completed') {
                  return 'No se puede cancelar una orden completada';
                }
                return 'Cancelar orden';
              };

              return (
                <TableRow key={order.id} hover>
                  <TableCell>{order.id}</TableCell>
                  <TableCell>
                    {order.productName || `Producto #${order.productId}`}
                  </TableCell>
                  <TableCell align="right">{order.quantity}</TableCell>
                  <TableCell align="right">${order.total.toFixed(2)}</TableCell>
                  <TableCell>
                    {new Date(order.createdAt).toLocaleDateString('es-AR', {
                      year: 'numeric',
                      month: '2-digit',
                      day: '2-digit',
                    })}
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={getStatusLabel(order.status)}
                      color={getStatusColor(order.status)}
                      size="small"
                    />
                  </TableCell>
                  <TableCell align="right">
                    <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: 0.5 }}>
                      <Tooltip title="Ver detalles de la orden">
                        <IconButton
                          size="small"
                          onClick={() => onView(order)}
                          aria-label="Ver detalles de la orden"
                          disabled={isCancelling}
                        >
                          <VisibilityIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                      {showCancelButton && (
                        <Tooltip title={getCancelTooltip()}>
                          <span>
                            <IconButton
                              size="small"
                              onClick={() => onCancel(order.id)}
                              aria-label="Cancelar orden"
                              disabled={!canCancel || isCancelling}
                            >
                              <CancelIcon fontSize="small" />
                            </IconButton>
                          </span>
                        </Tooltip>
                      )}
                    </Box>
                  </TableCell>
                </TableRow>
              );
            })}
          </TableBody>
        </Table>
      </TableContainer>
      <TablePagination
        component="div"
        count={totalCount}
        page={page}
        onPageChange={onPageChange}
        rowsPerPage={rowsPerPage}
        onRowsPerPageChange={onRowsPerPageChange}
        rowsPerPageOptions={[5]}
        labelRowsPerPage="Órdenes por página:"
        labelDisplayedRows={({ from, to, count }) =>
          `${from}-${to} de ${count !== -1 ? count : `más de ${to}`}`
        }
      />
    </>
  );
};
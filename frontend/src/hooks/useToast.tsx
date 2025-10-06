import { useState, useCallback } from 'react';
import { Snackbar, Alert} from '@mui/material';
import type { AlertColor } from '@mui/material';

interface ToastState {
  open: boolean;
  message: string;
  severity: AlertColor;
}

export const useToast = () => {
  const [toast, setToast] = useState<ToastState>({
    open: false,
    message: '',
    severity: 'success',
  });

  const showToast = useCallback((message: string, severity: AlertColor = 'success') => {
    setToast({
      open: true,
      message,
      severity,
    });
  }, []);

  const hideToast = useCallback(() => {
    setToast((prev) => ({
      ...prev,
      open: false,
    }));
  }, []);

  const ToastComponent = useCallback(() => (
    <Snackbar
      open={toast.open}
      autoHideDuration={4000}
      onClose={hideToast}
      anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
    >
      <Alert onClose={hideToast} severity={toast.severity} sx={{ width: '100%' }}>
        {toast.message}
      </Alert>
    </Snackbar>
  ), [toast, hideToast]);

  return {
    showToast,
    hideToast,
    ToastComponent,
  };
};
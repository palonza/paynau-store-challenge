// import React from "react";
import { AppBar, Toolbar, Typography, Button, IconButton, Tooltip } from "@mui/material";
import HomeIcon from "@mui/icons-material/Home";
import Brightness4Icon from "@mui/icons-material/Brightness4";
import Brightness7Icon from "@mui/icons-material/Brightness7";
import { Link, useLocation } from "react-router-dom";
import { useThemeMode } from "../../theme/ThemeContext";

export default function PageHeader() {
  const location = useLocation();
  const { mode, toggleMode } = useThemeMode();

  // Determina el título dinámicamente según la ruta
  const getTitle = () => {
    if (location.pathname.startsWith("/productos")) return "Productos";
    if (location.pathname.startsWith("/ordenes")) return "Órdenes";
    return "Paynau Store";
  };

  return (
    <AppBar position="static" color="primary" elevation={1}>
      <Toolbar
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
        }}
      >
        <Typography variant="h6" fontWeight="bold">
          {getTitle()}
        </Typography>

        <div>
            <Tooltip title="Ir al inicio">
                <Button
                component={Link}
                to="/"
                color="inherit"
                startIcon={<HomeIcon />}
                >
                Inicio
                </Button>
            </Tooltip>

            <Tooltip title={`Cambiar a modo ${mode === "light" ? "oscuro" : "claro"}`}>
                <IconButton color="inherit" onClick={toggleMode}>
                    {mode === "light" ? <Brightness4Icon /> : <Brightness7Icon />}
                </IconButton>
            </Tooltip>
        </div>
      </Toolbar>
    </AppBar>
  );
}

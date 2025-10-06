import React from "react";
import { AppBar, Toolbar, Typography, Button } from "@mui/material";
import HomeIcon from "@mui/icons-material/Home";
import { Link, useLocation } from "react-router-dom";

export default function PageHeader() {
  const location = useLocation();

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

        <Button
          component={Link}
          to="/"
          color="inherit"
          startIcon={<HomeIcon />}
        >
          Inicio
        </Button>
      </Toolbar>
    </AppBar>
  );
}

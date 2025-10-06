import { BrowserRouter, Routes, Route } from "react-router-dom";
import HomePage from "../pages/HomePage";
// import { ProductsTable } from "../components/products/ProductsTable";
// import { OrdersTable } from "../components/orders/OrdersTable";
import {ProductsContainer} from "../containers/ProductsContainer";
import {OrdersContainer} from "../containers/OrdersContainer";
import { Box } from "@mui/material";
import PageHeader from "../components/common/PageHeader";

export default function AppRoutes() {
  return (
    <BrowserRouter>
        <PageHeader />
        <Box sx={{ p: 4 }}>

        <Routes>
          <Route path="/" element={<HomePage />} />

          {/* Productos */}
          <Route path="/productos" element={<ProductsContainer />} />

          {/* Órdenes */}
          <Route path="/ordenes" element={<OrdersContainer />} />
        </Routes>

        </Box>
    </BrowserRouter>
  );
}

import { BrowserRouter, Routes, Route } from "react-router-dom";
import HomePage from "../pages/HomePage";
import { ProductsTable } from "../components/products/ProductsTable";
import { OrdersTable } from "../components/orders/OrdersTable";
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
                <Route
                path="/productos"
                element={
                    <div style={{ padding: 24 }}>
                    <ProductsTable
                        rows={[]}
                        loading={false}
                        error={null}
                        page={0}
                        rowsPerPage={5}
                        totalCount={0}
                        onPageChange={() => {}}
                        onRowsPerPageChange={() => {}}
                        onView={() => {}}
                        onEdit={() => {}}
                        onDelete={() => {}}
                    />
                    </div>
                }
                />

                {/* Órdenes */}
                <Route
                path="/ordenes"
                element={
                    <div style={{ padding: 24 }}>
                    <OrdersTable
                        orders={[]}
                        loading={false}
                        error={null}
                        page={0}
                        rowsPerPage={5}
                        totalCount={0}
                        onPageChange={() => {}}
                        onRowsPerPageChange={() => {}}
                        onView={() => {}}
                        onCancel={() => {}}
                    />
                    </div>
                }
                />
            </Routes>
        </Box>
    </BrowserRouter>
  );
}

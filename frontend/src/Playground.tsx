import { ProductsTable } from "./components/products/ProductsTable";
import { ProductForm } from "./components/products/ProductForm";
import { OrderForm } from "./components/orders/OrderForm";
import { OrdersTable } from "./components/orders/OrdersTable";

export default function Playground() {
  const sampleProducts = [
    { id: 1, name: "Producto demo", description: "Solo prueba", price: 10, stock: 5 },
    { id: 2, name: "Producto 2", description: "Otro", price: 20, stock: 10 },
  ];

  return (
    <div style={{ padding: 24, display: "grid", gap: 24 }}>
      <ProductsTable
        rows={sampleProducts}
        loading={false}
        error={null}
        page={0}
        rowsPerPage={5}
        totalCount={sampleProducts.length}
        onPageChange={() => {}}
        onRowsPerPageChange={() => {}}
        onView={(p) => console.log("Ver", p)}
        onEdit={(p) => console.log("Editar", p)}
        onDelete={(id) => console.log("Eliminar", id)}
      />

      <ProductForm
        open={true}
        onClose={() => console.log("Cerrar formulario")}
        onSubmit={(data) => console.log("Crear producto", data)}
        loading={false}
        error={null}
      />

      <OrderForm
        open={true}
        onClose={() => console.log("Cerrar orden")}
        onSubmit={(o) => console.log("Orden creada", o)}
        loading={false}
        error={null}
        products={sampleProducts}
      />

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
  );
}

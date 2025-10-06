USE PaynauDb;

SET FOREIGN_KEY_CHECKS=0;
TRUNCATE TABLE PaynauDb.Orders;
TRUNCATE TABLE PaynauDb.Products;
SET FOREIGN_KEY_CHECKS=1;

INSERT INTO Products (Id, Name, Description, Price, Stock) VALUES
(1, 'Wireless Mouse', 'Ergonomic wireless mouse with adjustable DPI.', 25.99, 150),
(2, 'Mechanical Keyboard', 'RGB backlit mechanical keyboard with blue switches.', 89.50, 80),
(3, 'HD Monitor', '27-inch full HD monitor with ultra-thin bezel.', 179.99, 45),
(4, 'USB-C Hub', 'Multiport adapter with HDMI, USB 3.0, and SD card reader.', 49.00, 100),
(5, 'External SSD', '1TB portable external SSD with high-speed USB 3.2.', 139.99, 60),
(6, 'Noise Cancelling Headphones', 'Wireless over-ear headphones with active noise cancelling.', 199.99, 30);

INSERT INTO Orders (Id, ProductId, Quantity, Total) VALUES
(1, 1, 2, 25.99 * 2),
(2, 2, 1, 89.50 * 1),
(3, 4, 5, 49.00 * 5),
(4, 5, 1, 139.99 * 1);

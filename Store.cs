using System;

namespace OrderManagement
{
    class Store
    {
        private Product[] products;
        private Order[] orders;
        private int productCount;
        private int orderCount;
        private int nextOrderId;

        public int OrderCount
        {
            get { return orderCount; }
        }

        public Store(int maxProducts, int maxOrders)
        {
            products = new Product[maxProducts];
            orders = new Order[maxOrders];
            productCount = 0;
            orderCount = 0;
            nextOrderId = 1001;
        }

        public int FindProductIndex(int productId)
        {
            for (int i = 0; i < productCount; i++)
            {
                if (products[i].Id == productId)
                {
                    return i;
                }
            }

            return -1;
        }

        public int FindOrderIndex(int orderId)
        {
            for (int i = 0; i < orderCount; i++)
            {
                if (orders[i].OrderId == orderId)
                {
                    return i;
                }
            }

            return -1;
        }

        public bool AddProduct(Product product)
        {
            if (productCount == products.Length)
            {
                Console.WriteLine("Ошибка: массив товаров заполнен");
                return false;
            }

            if (FindProductIndex(product.Id) != -1)
            {
                Console.WriteLine($"Ошибка: товар с ID {product.Id} уже существует");
                return false;
            }

            products[productCount] = product;
            productCount++;
            return true;
        }

        public void ShowProducts()
        {
            if (productCount == 0)
            {
                Console.WriteLine("Товаров пока нет");
                return;
            }

            Console.WriteLine("Список товаров:");

            for (int i = 0; i < productCount; i++)
            {
                Console.WriteLine(products[i]);
            }
        }

        public bool ShowProductForOrder(int productId)
        {
            int index = FindProductIndex(productId);

            if (index == -1)
            {
                Console.WriteLine($"Ошибка: товар с ID {productId} не найден");
                return false;
            }

            Console.WriteLine($"Товар: {products[index].Name}");
            Console.WriteLine($"Цена: {products[index].Price} KZT");
            Console.WriteLine($"Доступное количество: {products[index].Quantity}");
            return true;
        }

        public bool CreateOrder(string customerName, int productId, int quantity)
        {
            if (orderCount == orders.Length)
            {
                Console.WriteLine("Ошибка: массив заказов заполнен");
                return false;
            }

            if (string.IsNullOrWhiteSpace(customerName))
            {
                Console.WriteLine("Ошибка: ФИО покупателя не может быть пустым");
                return false;
            }

            int productIndex = FindProductIndex(productId);

            if (productIndex == -1)
            {
                Console.WriteLine($"Ошибка: товар с ID {productId} не найден");
                return false;
            }

            if (quantity <= 0)
            {
                Console.WriteLine("Ошибка: количество должно быть больше нуля");
                return false;
            }

            if (quantity > products[productIndex].Quantity)
            {
                Console.WriteLine($"Ошибка: на складе только {products[productIndex].Quantity} шт. товара \"{products[productIndex].Name}\", а заказано {quantity}");
                return false;
            }

            decimal totalAmount = products[productIndex].Price * quantity;

            orders[orderCount] = new Order(nextOrderId, customerName, productId, quantity, totalAmount);

            Console.WriteLine("Заказ успешно создан!");
            Console.WriteLine($"Номер заказа: {nextOrderId}");
            Console.WriteLine($"Сумма: {totalAmount} KZT");
            Console.WriteLine($"Статус: {orders[orderCount].Status}");

            orderCount++;
            nextOrderId++;
            return true;
        }

        public void MoveOrder(int orderId, OrderStatus newStatus)
        {
            int index = FindOrderIndex(orderId);

            if (index == -1)
            {
                Console.WriteLine($"Ошибка: заказ №{orderId} не найден");
                return;
            }

            switch (newStatus)
            {
                case OrderStatus.Confirmed:
                    ConfirmOrder(index);
                    break;
                case OrderStatus.Cancelled:
                    CancelOrder(index);
                    break;
                default:
                    orders[index].ChangeStatus(newStatus);
                    break;
            }
        }

        private void ConfirmOrder(int index)
        {
            OrderStatus status = orders[index].Status;

            if (status == OrderStatus.Cancelled)
            {
                Console.WriteLine($"Ошибка: заказ №{orders[index].OrderId} отменён, подтвердить его нельзя");
                return;
            }

            if (status != OrderStatus.Created)
            {
                Console.WriteLine($"Ошибка: заказ №{orders[index].OrderId} уже подтверждён");
                return;
            }

            int productIndex = FindProductIndex(orders[index].ProductId);

            if (productIndex == -1)
            {
                Console.WriteLine("Ошибка: товар из заказа не найден");
                return;
            }

            if (orders[index].Quantity > products[productIndex].Quantity)
            {
                Console.WriteLine($"Ошибка: подтвердить нельзя, на складе только {products[productIndex].Quantity} шт. товара \"{products[productIndex].Name}\", а в заказе {orders[index].Quantity}");
                return;
            }

            orders[index].ChangeStatus(OrderStatus.Confirmed);
            products[productIndex].DecreaseQuantity(orders[index].Quantity);

            Console.WriteLine($"Со склада списано {orders[index].Quantity} шт. Остаток товара \"{products[productIndex].Name}\": {products[productIndex].Quantity}");
        }

        private void CancelOrder(int index)
        {
            OrderStatus oldStatus = orders[index].Status;

            if (oldStatus == OrderStatus.Cancelled)
            {
                Console.WriteLine($"Ошибка: заказ №{orders[index].OrderId} уже отменён");
                return;
            }

            bool changed = orders[index].ChangeStatus(OrderStatus.Cancelled);

            if (changed && oldStatus != OrderStatus.Created)
            {
                int productIndex = FindProductIndex(orders[index].ProductId);

                if (productIndex != -1)
                {
                    products[productIndex].IncreaseQuantity(orders[index].Quantity);

                    Console.WriteLine($"На склад возвращено {orders[index].Quantity} шт. Остаток товара \"{products[productIndex].Name}\": {products[productIndex].Quantity}");
                }
            }
        }

        private void PrintOrder(int index)
        {
            int productIndex = FindProductIndex(orders[index].ProductId);

            if (productIndex != -1)
            {
                Console.WriteLine($"Товар: {products[productIndex].Name}");
            }

            orders[index].DisplayInfo();
        }

        public void ShowOrder(int orderId)
        {
            int index = FindOrderIndex(orderId);

            if (index == -1)
            {
                Console.WriteLine($"Ошибка: заказ №{orderId} не найден");
                return;
            }

            PrintOrder(index);
        }

        public void ShowAllOrders()
        {
            if (orderCount == 0)
            {
                Console.WriteLine("Заказов пока нет");
                return;
            }

            Console.WriteLine("Список всех заказов:");
            Console.WriteLine();

            for (int i = 0; i < orderCount; i++)
            {
                PrintOrder(i);
                Console.WriteLine();
            }
        }

        public void ShowOrdersByStatus(OrderStatus status)
        {
            Console.WriteLine($"Заказы со статусом {status}:");
            Console.WriteLine();

            int found = 0;

            for (int i = 0; i < orderCount; i++)
            {
                if (orders[i].Status == status)
                {
                    PrintOrder(i);
                    Console.WriteLine();
                    found++;
                }
            }

            if (found == 0)
            {
                Console.WriteLine("Заказов с таким статусом нет");
            }
        }

        public decimal GetTotalOrdersAmount()
        {
            decimal total = 0;

            for (int i = 0; i < orderCount; i++)
            {
                total += orders[i].TotalAmount;
            }

            return total;
        }

        public void ShowStatusCounts()
        {
            Console.WriteLine("Количество заказов по статусам:");

            foreach (OrderStatus status in Enum.GetValues(typeof(OrderStatus)))
            {
                int count = 0;

                for (int i = 0; i < orderCount; i++)
                {
                    if (orders[i].Status == status)
                    {
                        count++;
                    }
                }

                Console.WriteLine($"{status}: {count}");
            }
        }
    }
}

using System;

namespace OrderManagement
{
    struct Order
    {
        public int OrderId { get; }
        public string CustomerName { get; }
        public int ProductId { get; }
        public int Quantity { get; }
        public decimal TotalAmount { get; }
        public OrderStatus Status { get; private set; }

        public Order(int orderId, string customerName, int productId, int quantity, decimal totalAmount)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Количество должно быть больше нуля");
            }

            if (totalAmount <= 0)
            {
                throw new ArgumentException("Сумма заказа должна быть больше нуля");
            }

            OrderId = orderId;
            CustomerName = customerName;
            ProductId = productId;
            Quantity = quantity;
            TotalAmount = totalAmount;
            Status = OrderStatus.Created;
        }

        public bool ChangeStatus(OrderStatus newStatus)
        {
            bool allowed = false;

            switch (Status)
            {
                case OrderStatus.Created:
                    allowed = newStatus == OrderStatus.Confirmed || newStatus == OrderStatus.Cancelled;
                    break;
                case OrderStatus.Confirmed:
                    allowed = newStatus == OrderStatus.Processing || newStatus == OrderStatus.Cancelled;
                    break;
                case OrderStatus.Processing:
                    allowed = newStatus == OrderStatus.Shipped || newStatus == OrderStatus.Cancelled;
                    break;
                case OrderStatus.Shipped:
                    allowed = newStatus == OrderStatus.Delivered;
                    break;
                case OrderStatus.Delivered:
                case OrderStatus.Cancelled:
                    Console.WriteLine($"Ошибка: заказ №{OrderId} уже {Status}, статус изменить нельзя");
                    return false;
            }

            if (allowed)
            {
                Console.WriteLine($"Заказ №{OrderId}: статус изменён {Status} -> {newStatus}");
                Status = newStatus;
                return true;
            }

            Console.WriteLine($"Ошибка: заказ №{OrderId} нельзя перевести из {Status} в {newStatus}");
            return false;
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"Номер заказа: {OrderId}");
            Console.WriteLine($"Покупатель: {CustomerName}");
            Console.WriteLine($"ID товара: {ProductId}");
            Console.WriteLine($"Количество: {Quantity}");
            Console.WriteLine($"Сумма: {TotalAmount} KZT");
            Console.WriteLine($"Статус: {Status}");
        }
    }
}

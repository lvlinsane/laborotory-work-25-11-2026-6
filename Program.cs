using System;
using System.Globalization;

namespace OrderManagement
{
    class Program
    {
        static int ReadInt(string text)
        {
            while (true)
            {
                Console.Write(text);
                string input = Console.ReadLine();
                int value;

                if (int.TryParse(input, out value))
                {
                    return value;
                }

                Console.WriteLine("Ошибка: введите целое число");
            }
        }

        static decimal ReadDecimal(string text)
        {
            while (true)
            {
                Console.Write(text);
                string input = Console.ReadLine();
                decimal value;

                if (input != null)
                {
                    input = input.Replace(',', '.');
                }

                if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                {
                    return value;
                }

                Console.WriteLine("Ошибка: введите число");
            }
        }

        static string ReadString(string text)
        {
            while (true)
            {
                Console.Write(text);
                string input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input.Trim();
                }

                Console.WriteLine("Ошибка: строка не может быть пустой");
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("Система управления заказами");
            Console.WriteLine("1. Просмотреть список товаров");
            Console.WriteLine("2. Добавить товар");
            Console.WriteLine("3. Создать заказ");
            Console.WriteLine("4. Просмотреть список заказов");
            Console.WriteLine("5. Найти заказ по номеру");
            Console.WriteLine("6. Изменить статус заказа");
            Console.WriteLine("7. Отменить заказ");
            Console.WriteLine("8. Просмотреть заказы по статусу");
            Console.WriteLine("9. Статистика заказов");
            Console.WriteLine("0. Выход");
        }

        static void ShowProductsAction(Store store)
        {
            store.ShowProducts();
        }

        static void AddProductAction(Store store)
        {
            int id = ReadInt("Введите ID товара: ");

            if (id <= 0)
            {
                Console.WriteLine("Ошибка: ID должен быть больше нуля");
                return;
            }

            if (store.FindProductIndex(id) != -1)
            {
                Console.WriteLine($"Ошибка: товар с ID {id} уже существует");
                return;
            }

            string name = ReadString("Введите название: ");

            decimal price = ReadDecimal("Введите цену: ");

            if (price < 0)
            {
                Console.WriteLine("Ошибка: цена не может быть отрицательной");
                return;
            }

            int quantity = ReadInt("Введите количество: ");

            if (quantity < 0)
            {
                Console.WriteLine("Ошибка: количество не может быть отрицательным");
                return;
            }

            ProductCategory[] categories = (ProductCategory[])Enum.GetValues(typeof(ProductCategory));

            Console.WriteLine("Выберите категорию:");

            for (int i = 0; i < categories.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i]}");
            }

            int categoryNumber = ReadInt("Номер категории: ");

            if (categoryNumber < 1 || categoryNumber > categories.Length)
            {
                Console.WriteLine("Ошибка: нет такой категории");
                return;
            }

            Product product = new Product(id, name, price, quantity, categories[categoryNumber - 1]);

            if (store.AddProduct(product))
            {
                Console.WriteLine("Товар успешно добавлен!");
            }
        }

        static void CreateOrderAction(Store store)
        {
            int productId = ReadInt("Введите ID товара: ");

            if (!store.ShowProductForOrder(productId))
            {
                return;
            }

            int quantity = ReadInt("Введите количество: ");
            string customerName = ReadString("Введите ФИО покупателя: ");

            store.CreateOrder(customerName, productId, quantity);
        }

        static void ShowOrdersAction(Store store)
        {
            store.ShowAllOrders();
        }

        static void FindOrderAction(Store store)
        {
            int orderId = ReadInt("Введите номер заказа: ");
            Console.WriteLine();
            store.ShowOrder(orderId);
        }

        static void ChangeStatusAction(Store store)
        {
            int orderId = ReadInt("Введите номер заказа: ");

            if (store.FindOrderIndex(orderId) == -1)
            {
                Console.WriteLine($"Ошибка: заказ №{orderId} не найден");
                return;
            }

            Console.WriteLine("Выберите новый статус:");
            Console.WriteLine("1. Confirmed");
            Console.WriteLine("2. Processing");
            Console.WriteLine("3. Shipped");
            Console.WriteLine("4. Delivered");

            int choice = ReadInt("Номер статуса: ");
            OrderStatus newStatus;

            switch (choice)
            {
                case 1:
                    newStatus = OrderStatus.Confirmed;
                    break;
                case 2:
                    newStatus = OrderStatus.Processing;
                    break;
                case 3:
                    newStatus = OrderStatus.Shipped;
                    break;
                case 4:
                    newStatus = OrderStatus.Delivered;
                    break;
                default:
                    Console.WriteLine("Ошибка: нет такого статуса");
                    return;
            }

            store.MoveOrder(orderId, newStatus);
        }

        static void CancelOrderAction(Store store)
        {
            int orderId = ReadInt("Введите номер заказа: ");
            store.MoveOrder(orderId, OrderStatus.Cancelled);
        }

        static void ShowOrdersByStatusAction(Store store)
        {
            OrderStatus[] statuses = (OrderStatus[])Enum.GetValues(typeof(OrderStatus));

            Console.WriteLine("Выберите статус:");

            for (int i = 0; i < statuses.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {statuses[i]}");
            }

            int choice = ReadInt("Номер статуса: ");

            if (choice < 1 || choice > statuses.Length)
            {
                Console.WriteLine("Ошибка: нет такого статуса");
                return;
            }

            Console.WriteLine();
            store.ShowOrdersByStatus(statuses[choice - 1]);
        }

        static void ShowStatisticsAction(Store store)
        {
            Console.WriteLine($"Всего заказов: {store.OrderCount}");
            Console.WriteLine($"Общая стоимость всех заказов: {store.GetTotalOrdersAmount()} KZT");
            Console.WriteLine();
            store.ShowStatusCounts();
        }

        static void Main(string[] args)
        {
            Store store = new Store(50, 100);

            store.AddProduct(new Product(101, "Ноутбук", 250000, 5, ProductCategory.Electronics));
            store.AddProduct(new Product(102, "Футболка", 7000, 40, ProductCategory.Clothing));
            store.AddProduct(new Product(103, "Хлеб", 150, 100, ProductCategory.Food));
            store.AddProduct(new Product(104, "Учебник C#", 12000, 15, ProductCategory.Books));
            store.AddProduct(new Product(105, "Рюкзак", 18000, 12, ProductCategory.Other));

            bool running = true;

            while (running)
            {
                ShowMenu();
                int choice = ReadInt("Выберите действие: ");
                Console.WriteLine();

                switch (choice)
                {
                    case 1:
                        ShowProductsAction(store);
                        break;
                    case 2:
                        AddProductAction(store);
                        break;
                    case 3:
                        CreateOrderAction(store);
                        break;
                    case 4:
                        ShowOrdersAction(store);
                        break;
                    case 5:
                        FindOrderAction(store);
                        break;
                    case 6:
                        ChangeStatusAction(store);
                        break;
                    case 7:
                        CancelOrderAction(store);
                        break;
                    case 8:
                        ShowOrdersByStatusAction(store);
                        break;
                    case 9:
                        ShowStatisticsAction(store);
                        break;
                    case 0:
                        running = false;
                        Console.WriteLine("Выход из программы");
                        break;
                    default:
                        Console.WriteLine("Ошибка: нет такого пункта меню");
                        break;
                }

                Console.WriteLine();
            }
        }
    }
}

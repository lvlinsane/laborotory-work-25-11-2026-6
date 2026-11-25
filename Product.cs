using System;

namespace OrderManagement
{
    struct Product
    {
        public int Id { get; }
        public string Name { get; }
        public decimal Price { get; }
        public int Quantity { get; private set; }
        public ProductCategory Category { get; }

        public Product(int id, string name, decimal price, int quantity, ProductCategory category)
        {
            if (price < 0)
            {
                throw new ArgumentException("Цена не может быть отрицательной");
            }

            if (quantity < 0)
            {
                throw new ArgumentException("Количество не может быть отрицательным");
            }

            Id = id;
            Name = name;
            Price = price;
            Quantity = quantity;
            Category = category;
        }

        public decimal GetTotalValue()
        {
            return Price * Quantity;
        }

        public void DecreaseQuantity(int amount)
        {
            if (amount <= 0 || amount > Quantity)
            {
                throw new ArgumentException("Некорректное количество для списания со склада");
            }

            Quantity -= amount;
        }

        public void IncreaseQuantity(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Некорректное количество для возврата на склад");
            }

            Quantity += amount;
        }

        public override string ToString()
        {
            return $"ID: {Id} | {Name} | Цена: {Price} KZT | Количество: {Quantity} | Категория: {Category} | Стоимость остатков: {GetTotalValue()} KZT";
        }
    }
}

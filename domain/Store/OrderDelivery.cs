namespace Store
{
    public class OrderDelivery
    {
        public string Name { get; }
        public string Description { get; }
        public decimal Price { get; }
        public IReadOnlyDictionary<string, string> Parameters { get; }

        public OrderDelivery(string name,
                             string description,
                             decimal price,
                             IReadOnlyDictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException(nameof(description));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            Name = name;
            Description = description;
            Price = price;
            Parameters = parameters;
        }
    }
}

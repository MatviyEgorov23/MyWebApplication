namespace WebApplication_AuthenticationSystem_.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string UserEmail { get; set; }

        public string PostalCode { get; set; }

        public string PaymentMethod { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
    }
}

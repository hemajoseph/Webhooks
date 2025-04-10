namespace Webhooks.Api.Models
{
    public sealed record Student
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Grade { get; set; }
        
    }
}

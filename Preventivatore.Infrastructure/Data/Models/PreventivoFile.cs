namespace Preventivatore.Infrastructure.Data.Models
{
    public class PreventivoFile
    {
        public int Id { get; set; }
        public Guid PreventivoId { get; set; }
        public string BlobName { get; set; } = null!;
        public string Url { get; set; } = null!;
        public DateTime UploadedAt { get; set; }
    }
}

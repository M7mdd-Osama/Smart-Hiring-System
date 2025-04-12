namespace SmartHiring.APIs.DTOs
{
    public class NoteDto
    {
        public int Id { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UserId { get; set; }
        public bool IsSeen { get; set; }
        public int PostId { get; set; }
    }
}

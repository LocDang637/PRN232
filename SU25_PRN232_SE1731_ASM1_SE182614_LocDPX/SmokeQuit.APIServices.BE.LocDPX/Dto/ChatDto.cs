using System.ComponentModel.DataAnnotations;

namespace SmokeQuit.APIServices.BE.LocDPX.Dto
{
    public class ChatDto
    {
        [Required(ErrorMessage = "User ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be greater than 0")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Coach ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Coach ID must be greater than 0")]
        public int CoachId { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Message must be between 1 and 1000 characters")]
        public string Message { get; set; }

        [Required(ErrorMessage = "SentBy is required")]
        [RegularExpression("^(user|coach)$", ErrorMessage = "SentBy must be either 'user' or 'coach'")]
        public string SentBy { get; set; }

        [Required(ErrorMessage = "MessageType is required")]
        [RegularExpression("^(text|image|file)$", ErrorMessage = "MessageType must be 'text', 'image', or 'file'")]
        public string MessageType { get; set; }

        public bool IsRead { get; set; } = false;

        [Url(ErrorMessage = "Attachment URL must be a valid URL")]
        [StringLength(255, ErrorMessage = "Attachment URL cannot exceed 255 characters")]
        public string? AttachmentUrl { get; set; }

        public DateTime? ResponseTime { get; set; }

        // Note: CreatedAt is handled automatically by the system, not included in DTO
        // This prevents client from setting creation time
    }
}
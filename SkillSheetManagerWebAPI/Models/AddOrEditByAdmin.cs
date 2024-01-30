namespace SkillSheetWebAPI.Models
{
    /// <summary>
    /// Model for Add Or Edit By Admin
    /// </summary>
    public class AddOrEditByAdmin
    {
        /// <summary>
        /// User Name
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}

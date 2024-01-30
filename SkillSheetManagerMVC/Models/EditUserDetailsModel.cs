namespace SkillSheetMVC.Models
{
    /// <summary>
    /// MOdel for Edit User Partial View
    /// </summary>
    public class EditUserDetailsModel
    {
        /// <summary>
        /// Name
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

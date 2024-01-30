namespace SkillSheetMVC.Models
{
    /// <summary>
    /// Model for Add User Partial View
    /// </summary>
    public class AddNewUserModel
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

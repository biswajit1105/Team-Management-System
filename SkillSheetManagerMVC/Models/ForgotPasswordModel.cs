namespace SkillSheetMVC.Models
{
    /// <summary>
    /// Model for Change Password
    /// </summary>
    public class ForgotPasswordModel
    {
        /// <summary>
        /// User Name
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// New Password
        /// </summary>
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// Confirm Password
        /// </summary>
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

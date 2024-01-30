using System.ComponentModel.DataAnnotations;

namespace SkillSheetMVC.Models
{
    /// <summary>
    /// Model for Login Page
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Group Name
        /// </summary>
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        /// USer Name
        /// </summary>
        [Key]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}

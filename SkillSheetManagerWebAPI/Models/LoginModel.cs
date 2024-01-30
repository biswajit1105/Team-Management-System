using System.ComponentModel.DataAnnotations;

namespace SkillSheetWebAPI.Models
{
    /// <summary>
    /// Login Model
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Group Name
        /// </summary>
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        /// User Name
        /// </summary>
        [Key]   
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}

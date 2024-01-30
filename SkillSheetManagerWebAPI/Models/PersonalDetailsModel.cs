using System.ComponentModel.DataAnnotations;

namespace SkillSheetWebAPI.Models
{
    /// <summary>
    /// Personal Details Model
    /// </summary>
    public class PersonalDetailsModel
    {
        /// <summary>
        /// Name
        /// </summary>
        [Key]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Date of Birth
        /// </summary>
        [DataType(DataType.Date), DisplayFormat(DataFormatString = APIConstants.DateFormat, ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Gender
        /// </summary>
        public string? Gender { get; set; }

        /// <summary>
        /// Date of Joining
        /// </summary>
        [DataType(DataType.Date), DisplayFormat(DataFormatString = APIConstants.DateFormat, ApplyFormatInEditMode = true)]
        public DateTime? JoiningDate { get; set; }

        /// <summary>
        /// Worked in Japan
        /// </summary>
        public bool? WorkedInJapan { get; set; }

        /// <summary>
        /// Qualification
        /// </summary>
        public string? Qualification { get; set; }

        /// <summary>
        /// Computer Skills
        /// </summary>
        public string? ComputerSkills { get; set; }

        /// <summary>
        /// Languages
        /// </summary>
        public string? Languages { get; set; }

        /// <summary>
        /// Database
        /// </summary>
        public string? Database { get; set; }

        /// <summary>
        /// Image File Name
        /// </summary>
        public string? ImageFileName { get; set; }

        /// <summary>
        /// Image File Path
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// Image in Binary
        /// </summary>
        public byte[]? Image { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}

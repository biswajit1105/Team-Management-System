using Microsoft.AspNetCore.Mvc;
using SkillSheetWebAPI.Data;
using SkillSheetWebAPI.Models;

namespace SkillSheetWebAPI.Controllers
{
    /// <summary>
    /// Personal Details Controller
    /// </summary>
    [ApiController]
    [Route("api/personalDetails")]
    public class PersonalDetailsController : Controller
    {
        /// <summary>
        /// Property for Initializing DB context for Personal Details Table 
        /// </summary>
        private APIDbContext PersonalDBContext { get; set; }

        /// <summary>
        /// Constructor for Personal Details Controller
        /// </summary>
        /// <param name="personalDetailsDBContext">Db Context of Personal Details Page</param>
        public PersonalDetailsController(APIDbContext personalDetailsDBContext)
        {
            this.PersonalDBContext = personalDetailsDBContext;
        }

        #region Public Methods

        /// <summary>
        /// Action Method to Get the Personal Details of a paticular User
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <returns>Personal Details of a paticular User</returns>
        [HttpGet("GetPersonalDetails{userName}")]
        public async Task<IActionResult> GetPersonalDetails([FromRoute] string userName)
        {
            if (string.IsNullOrWhiteSpace(userName) == true)
            {
                return BadRequest(APIConstants.ErrEmptyUserName);
            }

            try
            {
                var personalDetails = await PersonalDBContext.PersonalDetails.FindAsync(userName);

                // Check if the user exists
                if (personalDetails == null)
                {
                    return BadRequest(APIConstants.ErrUserNotFound);
                }

                // Checks if Image File Path Exists
                if (string.IsNullOrWhiteSpace(personalDetails.Path) == false)
                {
                    personalDetails.Image = ReadImage(personalDetails.Path);
                }

                return Ok(personalDetails);
            }
            catch (Exception ex) { return BadRequest(APIConstants.ErrMsgErrorOccured + ex.Message); }
        }

        /// <summary>
        /// Action Method to Update Personal Details of a User
        /// </summary>
        /// <param name="newPersonalDetails">Modified Personal details of User</param>
        /// <returns>Result of Updating Personal Details of User</returns>
        [HttpPut("UpdatePersonalDetails")]
        public async Task<IActionResult> UpdatePersonalDetails([FromBody] PersonalDetailsModel newPersonalDetails)
        {
            if (newPersonalDetails == null || string.IsNullOrWhiteSpace(newPersonalDetails.Name) == true)
            {
                return BadRequest(APIConstants.ErrEmptyPersonalDetails);
            }

            try
            {
                // Get Existing Personal Details of User
                var personaldetails = await PersonalDBContext.PersonalDetails.FindAsync(newPersonalDetails.Name);

                if (personaldetails == null)
                {
                    return BadRequest(APIConstants.ErrPersonalDetailsNotfound);
                }

                // Converts Binary Image into File and Save it in a Folder
                string? imgFilePath = null;

                if (newPersonalDetails.ImageFileName != null && newPersonalDetails.Image != null)
                {
                    imgFilePath = SaveImage(newPersonalDetails.ImageFileName, newPersonalDetails.Image, out string error);
                }

                personaldetails.Name = newPersonalDetails.Name;
                personaldetails.BirthDate = newPersonalDetails.BirthDate;
                personaldetails.Gender = newPersonalDetails.Gender;
                personaldetails.JoiningDate = newPersonalDetails.JoiningDate;
                personaldetails.Languages = newPersonalDetails.Languages;
                personaldetails.WorkedInJapan = newPersonalDetails.WorkedInJapan;
                personaldetails.Qualification = newPersonalDetails.Qualification;
                personaldetails.ComputerSkills = newPersonalDetails.ComputerSkills;
                personaldetails.Database = newPersonalDetails.Database;
                personaldetails.Path = imgFilePath ?? personaldetails.Path;
                personaldetails.Email = newPersonalDetails.Email;

                await PersonalDBContext.SaveChangesAsync();
                return Ok(personaldetails);
            }
            catch (Exception ex) { return BadRequest(APIConstants.ErrMsgErrorOccured + ex.Message); }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Methods to read a Image and store in a Byte Array
        /// </summary>
        /// <param name="filePath">File Path of Image</param>
        /// <returns>Byte Array of Image</returns>
        private static byte[]? ReadImage(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) == true)
            {
                return null;
            }

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    byte[] binaryImage = memoryStream.ToArray();
                    return binaryImage;
                }
            }
        }

        /// <summary>
        /// Method to convert Binary Image into an Image file and Save in a folder
        /// </summary>
        /// <param name="fileName">Image File Name</param>
        /// <param name="image">Image in byte array</param>
        /// <param name="error">Error Messages</param>
        /// <returns>File Path</returns>
        private static string SaveImage(string fileName, byte[] image, out string error)
        {
            error = string.Empty;

            try
            {
                if (fileName == null || image == null || image.Length < 0)
                {
                    error = APIConstants.ErrImageNotFound;
                    return string.Empty;
                }

                string uploadFolder = "..\\SkillSheetManagerWebAPI\\Image\\";
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    stream.Write(image);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                error = APIConstants.ErrMsgErrorOccured + ex.Message;
                return string.Empty;
            }
        }

        #endregion
    }
}
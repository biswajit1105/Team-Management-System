using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSheetWebAPI.Data;
using SkillSheetWebAPI.Models;

namespace SkillSheetWebAPI.Controllers
{
    /// <summary>
    /// Common Controller
    /// </summary>
    [ApiController]
    [Route("api/common")]
    public class CommonController : Controller
    {
        /// <summary>
        /// Property for Initializing DB context
        /// </summary>
        private APIDbContext DbContext { get; }

        /// <summary>
        /// Constructor for Common Controller
        /// </summary>
        /// <param name="dbContext">Db Context</param>
        public CommonController(APIDbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        #region Public Methods

        /// <summary>
        /// Method to Get List of All User's Name and Email for Admin View
        /// </summary>
        /// <returns>List of All User's Name and Email</returns>
        [HttpGet("GetAllLoginDetails")]
        public async Task<IActionResult> GetAllLoginDetails()
        {
            try
            {
                List<PersonalDetailsModel> personalDetailsList = await DbContext.PersonalDetails
                .Select(p => new PersonalDetailsModel { Name = p.Name, Email = p.Email }).ToListAsync();
                return Ok(personalDetailsList);
            }
            catch (Exception ex) { return BadRequest(APIConstants.ErrMsgErrorOccured + ex.Message); }
        }

        /// <summary>
        /// Method to Add New User
        /// </summary>
        /// <param name="newUserDetails">New User Details</param>
        /// <returns>Result of New User Addition Process</returns>
        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser([FromBody] AddOrEditByAdmin newUserDetails)
        {
            if (newUserDetails == null || string.IsNullOrWhiteSpace(newUserDetails.UserName) == true ||
                string.IsNullOrWhiteSpace(newUserDetails.Password) == true ||
                string.IsNullOrWhiteSpace(newUserDetails.Email) == true)
            {
                return BadRequest(APIConstants.ErrEmptyNewUserDetails);
            }

            try
            {
                // Checks if User Name is already present in the database
                PersonalDetailsModel? personalDetails = await DbContext.PersonalDetails.FindAsync(newUserDetails.UserName);

                if (personalDetails != null)
                {
                    return NotFound();
                }

                // Login Model Object to Add User in the Login Table
                LoginModel newLoginDetails = new()
                {
                    GroupName = APIConstants.GroupUser,
                    UserName = newUserDetails.UserName,
                    Password = Encryption.ComputeHash(newUserDetails.Password, out string error),
                };

                // Check for error during Password Hashing
                if (error.Equals(string.Empty) == false)
                {
                    return BadRequest(APIConstants.ErrPasswordHashingFailed + error);
                }

                // Login Model Object to Add User in the Personal Details Table
                PersonalDetailsModel newPersonalDetails = new()
                {
                    Name = newUserDetails.UserName,
                    Email = newUserDetails.Email
                };

                await DbContext.Login.AddAsync(newLoginDetails);
                await DbContext.PersonalDetails.AddAsync(newPersonalDetails);
                await DbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex) { return BadRequest(APIConstants.ErrMsgErrorOccured + ex.Message); }
        }

        /// <summary>
        /// Method to Get User Details for "Edit User" Option by Admin
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <returns>User Details for Edit User Option</returns>
        [HttpGet("GetUserDetails/{userName}")]
        public async Task<IActionResult> GetUserDetails([FromRoute] string userName)
        {
            if (string.IsNullOrEmpty(userName)) { return BadRequest(APIConstants.ErrEmptyUserName); }

            try
            {
                // Get UserName and Email from Personal Details Table
                PersonalDetailsModel? userPersonalDetails = await DbContext.PersonalDetails.FindAsync(userName);

                if (userPersonalDetails == null || string.IsNullOrWhiteSpace(userPersonalDetails.Name) == true ||
                    string.IsNullOrWhiteSpace(userPersonalDetails.Email) == true)
                {
                    return BadRequest(APIConstants.ErrEmptyUserDetails);
                }

                AddOrEditByAdmin userDetails = new()
                {
                    UserName = userPersonalDetails.Name,
                    Email = userPersonalDetails.Email
                };

                return Ok(userDetails);
            }
            catch (Exception ex) { return BadRequest(APIConstants.ErrMsgErrorOccured + ex.Message); }
        }

        /// <summary>
        /// Method to Update modified Details of User by Admin
        /// </summary>
        /// <param name="updatedDetails">Updated User Details</param>
        /// <returns>Result for Edit User Process</returns>
        [HttpPut("EditUserDetails")]
        public async Task<IActionResult> EditUserDetails([FromBody] AddOrEditByAdmin updatedDetails)
        {
            if (updatedDetails == null || string.IsNullOrWhiteSpace(updatedDetails.UserName) == true ||
                 string.IsNullOrWhiteSpace(updatedDetails.Password) == true ||
                 string.IsNullOrWhiteSpace(updatedDetails.Email) == true)
            {
                return BadRequest(APIConstants.ErrEmptyEditedDetails);
            }

            try
            {
                // Get Old Details of User From both Login and Personal Details Table
                var userLoginDetails = await DbContext.Login.FindAsync(updatedDetails.UserName);
                var userPersonaldetails = await DbContext.PersonalDetails.FindAsync(updatedDetails.UserName);

                if (userLoginDetails == null || userPersonaldetails == null)
                {
                    return BadRequest(APIConstants.ErrUserNotFound);
                }

                // Hash New Modified Password 
                string newHashedPassword = Encryption.ComputeHash(updatedDetails.Password, out string error);

                if (error.Equals(string.Empty) == false)
                {
                    return BadRequest(APIConstants.ErrPasswordHashingFailed + error);
                }

                // If Modified Details are same as before then return as Not Found
                if (updatedDetails.UserName.Equals(userLoginDetails.UserName) == true &&
                    newHashedPassword.Equals(userLoginDetails.Password) == true &&
                    updatedDetails.Email.Equals(userPersonaldetails.Email) == true)
                {
                    return NotFound();
                }

                userLoginDetails.Password = newHashedPassword;
                userPersonaldetails.Email = updatedDetails.Email;
                await DbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex) { return BadRequest(APIConstants.ErrMsgErrorOccured + ex.Message); }
        }

        /// <summary>
        /// Method to Delete User by Admin
        /// </summary>
        /// <param name="userNames">User Names</param>
        /// <returns>Result for User Deletion Process</returns>
        [HttpDelete("DeleteUser{userNames}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string userNames)
        {
            if (string.IsNullOrWhiteSpace(userNames) == true)
            {
                return BadRequest(APIConstants.ErrUserNotFound);
            }

            try
            {
                // Converts string of userNames to an array of userNames
                string[] userNamesArray = ConvertUsersStringToArray(userNames);

                if (userNamesArray.Length <= 0)
                {
                    return BadRequest(APIConstants.ErrUserNamesNotFound);
                }

                foreach (var names in userNamesArray)
                {
                    var userLoginDetails = await DbContext.Login.FindAsync(names.Trim());
                    var userPersonalDetails = await DbContext.PersonalDetails.FindAsync(names.Trim());

                    if (userLoginDetails == null || userPersonalDetails == null)
                    {
                        continue;
                    }

                    DbContext.PersonalDetails.Remove(userPersonalDetails);
                    DbContext.Login.Remove(userLoginDetails);
                    await DbContext.SaveChangesAsync();
                }

                return Ok();
            }
            catch (Exception ex) { return BadRequest(APIConstants.ErrMsgErrorOccured + ex.Message); }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method to convert string of userNames to an array of userNames
        /// </summary>
        /// <param name="usersNames">string of User Names</param>
        /// <returns>Array of UserNames</returns>
        private static string[] ConvertUsersStringToArray(string usersNames)
        {
            string[] userNamesArray;
            char[] splitChar = { ',' };
            userNamesArray = usersNames.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < userNamesArray.Length; i++)
            {
                userNamesArray[i] = userNamesArray[i].Trim('\"');
            }

            return userNamesArray;
        }

        #endregion
    };
}
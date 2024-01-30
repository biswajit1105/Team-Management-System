using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SkillSheetWebAPI.Data;
using SkillSheetWebAPI.Models;

namespace SkillSheetWebAPI.Controllers
{
    /// <summary>
    /// Login Controller
    /// </summary>
    [ApiController]
    [Route("api/login")]
    public class LoginController : Controller
    {
        /// <summary>
        /// Property for Initializing DB context for Login Table 
        /// </summary>
        private APIDbContext LoginDbContext { get; }

        /// <summary>
        /// Constructor for Login Controller
        /// </summary>
        /// <param name="dbContext">Db Context of Login Page</param>
        public LoginController(APIDbContext dbContext)
        {
            this.LoginDbContext = dbContext;
        }

        #region Public Methods

        /// <summary>
        /// Action Method to Get All Group Names and User Names for Login Page
        /// </summary>
        /// <returns>Group Name and User Name of all Users</returns>
        [HttpGet("GetUserDetailsOfAll")]
        public async Task<IActionResult> GetUserDetailsOfAll()
        {
            try
            {
                List<LoginModel> myLoginList = await LoginDbContext.Login.ToListAsync();
                return Ok(myLoginList);
            }
            catch (Exception ex) { return BadRequest(APIConstants.ErrMsgErrorOccured + ex.Message); }
        }

        /// <summary>
        /// Action Method to Change Password using Forgot Password Option
        /// </summary>
        /// <param name="newPasswordDetails">User Details for changing Password</param>
        /// <returns>Result of changing Password using Forgot Passsword Option</returns>
        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] ForgotPasswordModel newPasswordDetails)
        {
            if (newPasswordDetails == null || string.IsNullOrWhiteSpace(newPasswordDetails.UserName) == true ||
                 string.IsNullOrWhiteSpace(newPasswordDetails.Password) == true ||
                 string.IsNullOrWhiteSpace(newPasswordDetails.NewPassword) == true ||
                 string.IsNullOrWhiteSpace(newPasswordDetails.ConfirmPassword) == true)
            {
                return BadRequest(APIConstants.ErrNullImput);
            }

            // If New Password and Confirm Password are not Equal then return Bad Request
            if (newPasswordDetails.NewPassword.Equals(newPasswordDetails.ConfirmPassword) == false)
            {
                return BadRequest(APIConstants.ErrPasswordNotMatch);
            }

            try
            {
                // Get Old Password Details Saved in Database
                var oldPasswordDetails = await LoginDbContext.Login.FindAsync(newPasswordDetails.UserName);

                if (oldPasswordDetails == null)
                {
                    return BadRequest(APIConstants.ErrUserNotFound);
                }

                // Checks for Old Password Match
                bool? isPasswordMatch = Encryption.VerifyHash(newPasswordDetails.Password, oldPasswordDetails.Password, out string error);

                if (error.Equals(string.Empty) == false)
                {
                    return BadRequest(APIConstants.ErrPasswordHashingFailed + error);
                }

                // Check for Wrong password Entered by user
                if (isPasswordMatch == false)
                {
                    return NotFound();
                }

                // Hash New Password
                string newPassword = Encryption.ComputeHash(newPasswordDetails.NewPassword, out error);

                if (string.IsNullOrWhiteSpace(newPassword) == true)
                {
                    return BadRequest(APIConstants.ErrPasswordHashingFailed + error);
                }

                oldPasswordDetails.Password = newPassword;
                await LoginDbContext.SaveChangesAsync();
                return Ok(oldPasswordDetails);
            }
            catch (Exception ex) { return BadRequest(APIConstants.ErrMsgErrorOccured + ex.Message); }
        }

        /// <summary>
        /// Action Method to Match Password of User
        /// </summary>
        /// <param name="loginInput">User Name</param>
        /// <returns>Result for Saving Password</returns>
        [HttpGet("CheckPasswordMatch{loginInput}")]
        public async Task<IActionResult> CheckPasswordMatch([FromRoute] string loginInput)
        {
            if (string.IsNullOrEmpty(loginInput) == true) { return BadRequest(APIConstants.ErrNullImput); }

            try
            {
                // Get User's Details by UserName for Forgot Password Page
                LoginModel? userLoginDetails = await LoginDbContext.Login.FindAsync(loginInput);

                if (userLoginDetails != null)
                {
                    return Ok(userLoginDetails);
                }

                // Get User's Input details in Login Page
                LoginModel? userLoginDetail = JsonConvert.DeserializeObject<LoginModel>(loginInput);

                if (userLoginDetail == null || string.IsNullOrEmpty(userLoginDetail.GroupName) == true ||
                    string.IsNullOrEmpty(userLoginDetail.UserName) == true ||
                    string.IsNullOrEmpty(userLoginDetail.Password) == true)
                {
                    return BadRequest(APIConstants.ErrNullImput);
                }

                // Get User Details to Match Password for Login Page
                LoginModel? userDetail = await LoginDbContext.Login.FindAsync(userLoginDetail.UserName);

                if (userDetail == null || string.IsNullOrEmpty(userDetail.GroupName) == true ||
                    string.IsNullOrEmpty(userDetail.UserName) == true ||
                    string.IsNullOrEmpty(userDetail.Password) == true)
                {
                    return BadRequest(APIConstants.ErrUserNotFound);
                }

                // Check for Password Match
                bool isPasswordMatch = Encryption.VerifyHash(userLoginDetail.Password, userDetail.Password, out string error);

                if (error.Equals(string.Empty) == false)
                {
                    return BadRequest(APIConstants.ErrPasswordHashingFailed + error);
                }

                if (isPasswordMatch == false)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception ex) { return BadRequest(APIConstants.ErrMsgErrorOccured + ex.Message); }
        }

        #endregion
    }
}
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SkillSheetMVC.EnumsAndConstants;
using SkillSheetMVC.Models;
using System.Text;

namespace SkillSheetMVC.Controllers
{
    /// <summary>
    /// Forgot Password Controller
    /// </summary>
    public class ForgotPasswordController : Controller
    {
        /// <summary>
        /// Object representation for URI
        /// </summary>
        private readonly Uri _baseAddress;

        /// <summary>
        /// Property that provides a class for HTTP Request and HTTP Response by URI
        /// </summary>
        private readonly HttpClient _client;

        /// <summary>
        /// Constructor for Forgot Password Controller
        /// </summary>
        public ForgotPasswordController(IConfiguration configuration)
        {
            string apiUrl = configuration["AppSettings:ApiUrl"] ?? throw new InvalidOperationException(MVCConatants.ErrApiUrlNotConfigured);

            _baseAddress = new Uri(apiUrl);

            _client = new HttpClient
            {
                BaseAddress = _baseAddress
            };
        }

        /// <summary>
        /// Action Method to Get User Details for Forgot Password Page
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <param name="groupName">group Name</param>
        /// <returns>View for Forgot Password Page</returns>
        public IActionResult Index(string userName, EnumGroupNames groupName)
        {
            bool hasSession = HttpContext.Session.Keys.Contains(userName);

            // Check if user is logged in
            if (hasSession == false)
            {
                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrSessionExpired;
                return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
            }

            if (string.IsNullOrWhiteSpace(userName) == true)
            {
                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUnableToProcess;
                return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
            }

            TempData[MVCConatants.GroupName] = groupName;
            ViewBag.GroupName = groupName.ToString();
            ViewBag.UserName = userName;
            ForgotPasswordModel? userDetails;

            try
            {
                HttpResponseMessage response = _client.GetAsync(_baseAddress + $"/login/CheckPasswordMatch{userName}").Result;

                if (response.IsSuccessStatusCode == true)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    userDetails = JsonConvert.DeserializeObject<ForgotPasswordModel>(data);
                }
                else if (string.Equals(response.ReasonPhrase, MVCConatants.ErrMsgBadRequest, StringComparison.OrdinalIgnoreCase) == true)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUnableToProcess;
                    return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
                }
                else
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrMsgSomethingWentWrong;
                    return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
                }
            }
            catch (Exception ex)
            {
                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrMsgErrorOccured + ex.Message;
                return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
            }

            return View(userDetails);
        }

        /// <summary>
        /// Action Method for Changing Password
        /// </summary>
        /// <param name="changePassword">User Details provided by the user for changing Password</param>
        /// <returns>View for Admin Page, Personal Details Page or Forgot Password Page</returns>
        public IActionResult ChangePassword(ForgotPasswordModel changePassword)
        {
            var group = TempData[MVCConatants.GroupName];

            // If any Input for passwords is null
            if (changePassword == null || string.IsNullOrWhiteSpace(changePassword.UserName) == true ||
                string.IsNullOrWhiteSpace(changePassword.Password) == true ||
                string.IsNullOrWhiteSpace(changePassword.NewPassword) == true ||
                string.IsNullOrWhiteSpace(changePassword.ConfirmPassword) == true || group == null)
            {
                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrEmptyInput;
                return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
            }

            EnumGroupNames groupName = (EnumGroupNames)group;
            string userName = changePassword.UserName;

            try
            {
                // If New Password and Confirm Password does not Matches
                if (changePassword.NewPassword.Equals(changePassword.ConfirmPassword) == false)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrPasswordNotMatch;
                    return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.ForgotPasswordController, new { userName, groupName });
                }

                string json = JsonConvert.SerializeObject(changePassword);
                var content = new StringContent(json, Encoding.UTF8, MVCConatants.MediaType);
                HttpResponseMessage response = _client.PutAsync(_baseAddress + "/login/UpdatePassword", content).Result;

                if (response.IsSuccessStatusCode == true)
                {
                    if (groupName.Equals(EnumGroupNames.User) == true)
                    {
                        TempData[MVCConatants.MsgAlertMessage] = MVCConatants.MsgPasswordChangedSucess;
                        return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.PersonalDetailsController, new { userName });
                    }
                    else if (groupName.Equals(EnumGroupNames.Admin) == true)
                    {
                        TempData[MVCConatants.MsgAlertMessage] = MVCConatants.MsgPasswordChangedSucess;
                        var adminName = userName;
                        return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.AdminController, new { adminName });
                    }
                }
                else if (string.Equals(response.ReasonPhrase, MVCConatants.ErrMsgNotFound, StringComparison.OrdinalIgnoreCase) == true)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrWrongPassword;
                    return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.ForgotPasswordController, new { userName, groupName });
                }
                else if (string.Equals(response.ReasonPhrase, MVCConatants.ErrMsgBadRequest, StringComparison.OrdinalIgnoreCase) == true)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUnableToChangePassword;
                    return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.ForgotPasswordController, new { userName, groupName });
                }
            }
            catch (Exception ex)
            {
                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrMsgErrorOccured + ex.Message;
                return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.ForgotPasswordController, new { userName, groupName });
            }

            TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrMsgSomethingWentWrong;
            return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.ForgotPasswordController, new { userName, groupName });
        }
    }
}
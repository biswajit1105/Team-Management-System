using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SkillSheetMVC.EnumsAndConstants;
using SkillSheetMVC.Models;

namespace SkillSheetMVC.Controllers
{
    /// <summary>
    /// Login Controller
    /// </summary>
    public class LoginController : Controller
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
        /// Constructor for Login Controller
        /// </summary>
        public LoginController(IConfiguration configuration)
        {
            string apiUrl = configuration["AppSettings:ApiUrl"] ?? throw new InvalidOperationException(MVCConatants.ErrApiUrlNotConfigured);

            _baseAddress = new Uri(apiUrl);

            _client = new HttpClient
            {
                BaseAddress = _baseAddress
            };
        }

        /// <summary>
        /// Action Method to Get List of Group Names and User Names
        /// </summary>
        /// <param name="logout_Clicked">Login Button is clicked or not</param>
        /// <param name="username">User Name</param>
        /// <returns>View for Login details Page</returns>
        public IActionResult Index(bool logout_Clicked = false, string? username = null)
        {
            List<LoginViewModel>? viewModels = new();
            bool hasSession = HttpContext.Session.Keys.Contains(username);

            // Clear Session when User Logout
            if (logout_Clicked == true && hasSession == true && username != null)
            {
                HttpContext.Session.Remove(username);
            }

            try
            {
                HttpResponseMessage response = _client.GetAsync(_baseAddress + "/login/GetUserDetailsOfAll").Result;

                if (response.IsSuccessStatusCode == true)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    viewModels = (JsonConvert.DeserializeObject<List<LoginViewModel>>(data));
                }
                else if (string.Equals(response.ReasonPhrase, MVCConatants.ErrMsgBadRequest, StringComparison.OrdinalIgnoreCase) == true)
                {
                    var error = response.Content.ReadAsStringAsync();
                    TempData[MVCConatants.ErrFailedMessage] = error.Result.ToString();
                }
            }
            catch (Exception ex)
            {
                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrMsgErrorOccured + ex.Message;
            }

            return View(viewModels);
        }

        /// <summary>
        /// Action Method to Get Login Details Entered by a User for Password Verification
        /// </summary>
        /// <param name="userLoginInput">Login Details Entered by a user for login attempt</param>
        /// <returns>View of Login Page or Admin Page</returns>
        public IActionResult LoginSubmit(LoginModel userLoginInput)
        {
            if (userLoginInput == null)
            {
                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUnableToGetLoginDetails;
                return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
            }

            if (string.IsNullOrEmpty(userLoginInput.GroupName) == true ||
                string.IsNullOrEmpty(userLoginInput.UserName) == true ||
                string.IsNullOrEmpty(userLoginInput.Password) == true)
            {
                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUnableToGetLoginDetails;
                return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
            }

            TempData[MVCConatants.GroupName] = userLoginInput.GroupName;
            TempData[MVCConatants.UserName] = userLoginInput.UserName;

            try
            {
                string loginInput = JsonConvert.SerializeObject(userLoginInput); 
                HttpResponseMessage response = _client.GetAsync(_baseAddress + $"/login/CheckPasswordMatch{loginInput}").Result;

                if (response.IsSuccessStatusCode == true)
                {
                    // Set Session of User
                    HttpContext.Session.SetString(userLoginInput.UserName, "");
                    TempData[MVCConatants.GroupName] = null;
                    TempData[MVCConatants.UserName] = null;

                    if (userLoginInput.GroupName.Equals(EnumGroupNames.User.ToString()) == true)
                    {
                        string userName = userLoginInput.UserName;
                        return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.PersonalDetailsController, new { userName });
                    }
                    else if (userLoginInput.GroupName.Equals(EnumGroupNames.Admin.ToString()) == true)
                    {
                        string adminName = userLoginInput.UserName;
                        return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.AdminController, new { adminName });
                    }
                }
                else if (string.Equals(response.ReasonPhrase, MVCConatants.ErrMsgBadRequest, StringComparison.OrdinalIgnoreCase) == true)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrLoginFailed;
                    return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
                }
                else if (string.Equals(response.ReasonPhrase, MVCConatants.ErrMsgNotFound, StringComparison.OrdinalIgnoreCase) == true)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrWrongPassword;
                    return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
                }

                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrMsgSomethingWentWrong;
                return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
            }
            catch (Exception ex)
            {
                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrMsgErrorOccured + ex.Message;
                return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
            }
        }
    }
}
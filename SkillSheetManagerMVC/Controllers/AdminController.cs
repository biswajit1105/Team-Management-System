using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SkillSheetMVC.EnumsAndConstants;
using SkillSheetMVC.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace SkillSheetMVC.Controllers
{
    /// <summary>
    /// Admin Controller
    /// </summary>
    public class AdminController : Controller
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
        /// Constructor for Admin Controller
        /// </summary>
        public AdminController(IConfiguration configuration)
        {
            string apiUrl = configuration["AppSettings:ApiUrl"] ?? throw new InvalidOperationException(MVCConatants.ErrApiUrlNotConfigured);

            _baseAddress = new Uri(apiUrl);

            _client = new HttpClient
            {
                BaseAddress = _baseAddress
            };
        }

        /// <summary>
        /// Action Method to Get list of all UserNames and Email to Show in Admin Page
        /// </summary>
        /// <param name="adminName">Admin Name</param>
        /// <returns>View of Admin Page</returns>
        public IActionResult Index(string adminName)
        {
            bool hasSession = HttpContext.Session.Keys.Contains(adminName);

            // Check if user is logged in
            if (hasSession == false)
            {
                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrSessionExpired;
                return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
            } 

            if (string.IsNullOrWhiteSpace(adminName) == true)
            {
                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUnableToGetAdminDetails;
                return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
            }

            ViewBag.UserName = adminName;
            List<AdminModel>? adminPage;

            try
            {
                HttpResponseMessage response = _client.GetAsync(_baseAddress + $"/common/GetAllLoginDetails").Result;

                if (response.IsSuccessStatusCode == true)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    adminPage = JsonConvert.DeserializeObject<List<AdminModel>>(data);
                }
                else if (string.Equals(response.ReasonPhrase, MVCConatants.ErrMsgBadRequest, StringComparison.OrdinalIgnoreCase) == true)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUnableToGetUserDetails;
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

            return View(adminPage);
        }

        /// <summary>
        /// Action Method for Partial View to Add New User
        /// </summary>
        /// <param name="adminName">Admin Name</param>
        /// <returns>Partial view of Add New User Page</returns>
        public IActionResult AddNewUser(string adminName)
        {
            if (string.IsNullOrWhiteSpace(adminName) == true)
            {
                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUnableToGetAdminDetails;
                return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
            }

            ViewBag.AdminName = adminName;
            AddNewUserModel newUser = new();
            return PartialView(MVCConatants.AddUserPartialView, newUser);
        }

        /// <summary>
        /// Action Method for Adding New User
        /// </summary>
        /// <param name="adminName">Admin Name</param>
        /// <param name="newUser">User Details on New User</param>
        /// <returns>View of Admin Page</returns>
        public IActionResult UpdateNewUser(string adminName, AddNewUserModel newUser)
        {
            if (string.IsNullOrWhiteSpace(adminName) == true)
            {
                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUnableToGetAdminDetails;
                return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
            }

            try
            {
                if (newUser == null || string.IsNullOrEmpty(newUser.UserName) == true ||
                    string.IsNullOrEmpty(newUser.Password) == true ||
                    string.IsNullOrEmpty(newUser.Email) == true)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrEmptyInput;
                    return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.AdminController, new { adminName });
                }

                // Check for valid Email 
                string email = newUser.Email;
                Regex regex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
                Match match = regex.Match(email);

                if (match.Success == false)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrInvalidEmailFormat;
                    return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.AdminController, new { adminName });
                }

                HttpResponseMessage response = _client.PostAsJsonAsync(_baseAddress + $"/common/AddUser", newUser).Result;

                if (response.IsSuccessStatusCode == true)
                {
                    TempData[MVCConatants.MsgAlertMessage] = MVCConatants.MsgUserAdded;
                }
                else if (string.Equals(response.ReasonPhrase, MVCConatants.ErrMsgNotFound, StringComparison.OrdinalIgnoreCase) == true)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUserAlreadyPresent;
                }
                else if (string.Equals(response.ReasonPhrase, MVCConatants.ErrMsgBadRequest, StringComparison.OrdinalIgnoreCase) == true)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUnableToAddUser;
                }
                else
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrMsgSomethingWentWrong;
                }
            }
            catch (Exception ex) { TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrMsgErrorOccured + ex.Message; }
            return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.AdminController, new { adminName });
        }

        /// <summary>
        /// Action Method for Getting Details of User For edit
        /// </summary>
        /// <param name="usernames">User Name</param>
        /// <param name="adminName">Admin Name</param>
        /// <returns>Partial View of Edit User Page</returns>
        public IActionResult EditUserDetails(string usernames, string adminName)
        {
            ViewBag.AdminName = adminName;
            usernames = usernames.Trim();
            AddNewUserModel? editUser = new();

            if (string.IsNullOrWhiteSpace(adminName) == true)
            {
                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUnableToGetAdminDetails;
                return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
            }

            if (string.IsNullOrWhiteSpace(usernames) == true)
            {
                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUnableToGetUserDetails;
                return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.AdminController, new { adminName });
            }

            try
            {
                HttpResponseMessage response = _client.GetAsync(_baseAddress + $"/common/GetUserDetails/{usernames}").Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    editUser = JsonConvert.DeserializeObject<AddNewUserModel>(data);
                }
                else if (string.Equals(response.ReasonPhrase, MVCConatants.ErrMsgBadRequest, StringComparison.OrdinalIgnoreCase) == true)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUnableToGetUserDetails;
                }
                else
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrMsgSomethingWentWrong;
                }
            }
            catch (Exception ex)
            {
                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrMsgErrorOccured + ex.Message;
                RedirectToAction(MVCConatants.IndexMethod, MVCConatants.AdminController);
            }

            return PartialView(MVCConatants.EditUserPartialView, editUser);
        }

        /// <summary>
        /// Action Method for Updating Existing User Details
        /// </summary>
        /// <param name="adminName">Admin Name</param>
        /// <param name="userDetails">Edited USer Details Filled by User</param>
        /// <returns>View for Admin Page</returns>
        public IActionResult UpdateExistingUser(string adminName, EditUserDetailsModel userDetails)
        {
            if (string.IsNullOrWhiteSpace(adminName) == true)
            {
                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUnableToGetAdminDetails;
                return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
            }

            if (userDetails == null || string.IsNullOrWhiteSpace(userDetails.UserName) == true ||
                string.IsNullOrWhiteSpace(userDetails.Password) == true ||
                string.IsNullOrWhiteSpace(userDetails.Email) == true)
            {
                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrEmptyInput;
                return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.AdminController, new { adminName });
            }

            try
            {
                // Check for valid Email 
                string email = userDetails.Email;
                Regex regex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
                Match match = regex.Match(email);

                if (match.Success == false)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrInvalidEmailFormat;
                    return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.AdminController, new { adminName });
                }

                string json = JsonConvert.SerializeObject(userDetails);
                var content = new StringContent(json, Encoding.UTF8, MVCConatants.MediaType);
                HttpResponseMessage response = _client.PutAsync(_baseAddress + "/common/EditUserDetails", content).Result;

                if (response.IsSuccessStatusCode == true)
                {
                    TempData[MVCConatants.MsgAlertMessage] = MVCConatants.MsgUpdatedUser;
                }
                else if (string.Equals(response.ReasonPhrase, MVCConatants.ErrMsgBadRequest, StringComparison.OrdinalIgnoreCase) == true)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUnableToEdit;
                }
                else if (string.Equals(response.ReasonPhrase, MVCConatants.ErrMsgNotFound, StringComparison.OrdinalIgnoreCase) == true)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrSameEditedDetails;
                }
                else
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrMsgSomethingWentWrong;
                }
            }
            catch (Exception ex) { TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrMsgErrorOccured + ex.Message; }

            return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.AdminController, new { adminName });
        }

        /// <summary>
        /// Action Method for Deleting User
        /// </summary>
        /// <param name="usernames">List of User Names</param>
        /// <returns>Result of Deleting User Process</returns>
        public IActionResult DeleteUser(string usernames)
        {
            try
            {
                string userNames = JsonConvert.SerializeObject(usernames);
                HttpResponseMessage response = _client.DeleteAsync(_baseAddress + $"/common/DeleteUser{userNames}").Result;

                if (response.IsSuccessStatusCode == true)
                {
                    return Ok();
                }
                else { return NotFound(); }
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}

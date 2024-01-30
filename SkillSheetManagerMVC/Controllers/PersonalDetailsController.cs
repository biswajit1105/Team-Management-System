using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SkillSheetMVC.EnumsAndConstants;
using SkillSheetMVC.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace SkillSheetMVC.Controllers
{
    /// <summary>
    /// Personal Details Controller
    /// </summary>
    public class PersonalDetailsController : Controller
    {
        /// <summary>
        /// Object representation for URI
        /// </summary>
        private readonly Uri _baseAddress;

        /// <summary>
        /// Property that provides a class for HTTP Request and HTTP Response by URI
        /// </summary>
        private readonly HttpClient _client;

        #region Public Method

        /// <summary>
        /// Constructor for Personal Details Controller
        /// </summary>
        public PersonalDetailsController(IConfiguration configuration)
        {
            string apiUrl = configuration["AppSettings:ApiUrl"] ?? throw new InvalidOperationException(MVCConatants.ErrApiUrlNotConfigured);

            _baseAddress = new Uri(apiUrl);

            _client = new HttpClient
            {
                BaseAddress = _baseAddress
            };
        }

        /// <summary>
        /// Action Method to Get Personal Details Page Info
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <returns>View For Personal Details Page</returns>
        public IActionResult Index(string userName)
        {
            ViewBag.UserName = userName;
            PersonalDetailsModel? personalDetailsModel;
            bool hasSession = HttpContext.Session.Keys.Contains(userName);

            // Check if user is logged in
            if (hasSession == false)
            {
                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrSessionExpired;
                return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
            }

            if (string.IsNullOrWhiteSpace(userName) == true)
            {
                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrEmptyUserName;
                return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
            }

            try
            {
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + $"/personalDetails/GetPersonalDetails{userName}").Result;

                if (response.IsSuccessStatusCode == true)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    personalDetailsModel = JsonConvert.DeserializeObject<PersonalDetailsModel>(data);
                }
                else if (string.Equals(response.ReasonPhrase, MVCConatants.ErrMsgBadRequest, StringComparison.OrdinalIgnoreCase) == true)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUnableToGetPersonalDetails;
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

            return View(personalDetailsModel);
        }

        /// <summary>
        /// Action Method for Updating Personal Details
        /// </summary>
        /// <param name="modifiedPersonalDetails">Modified Personal Details of a User</param>
        /// <returns>View to Personal Details Page or Index Page</returns>
        [HttpPost]
        public IActionResult PersonalDetailsUpdate(PersonalDetailsModel modifiedPersonalDetails)
        {
            try
            {
                if (modifiedPersonalDetails == null)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUnableToGetModifiedPersonalDetails;
                    return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.LoginController);
                }

                if (string.IsNullOrWhiteSpace(modifiedPersonalDetails.Name) == true)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrEmptyUserName;
                    return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.PersonalDetailsController, new { userName = modifiedPersonalDetails.Name });
                }

                // Checks if Email, Birthdate or Joining Date is Invalid
                bool isPersonalDataValid = CheckValidPersonalData(modifiedPersonalDetails, out string invalidParameter);

                if (isPersonalDataValid == false)
                {
                    TempData[MVCConatants.Name] = modifiedPersonalDetails.Name;
                    TempData[MVCConatants.Gender] = modifiedPersonalDetails.Gender;
                    TempData[MVCConatants.Languages] = modifiedPersonalDetails.Languages;
                    TempData[MVCConatants.Database] = modifiedPersonalDetails.Database;
                    TempData[MVCConatants.Qualification] = modifiedPersonalDetails.Qualification;
                    TempData[MVCConatants.Email] = modifiedPersonalDetails.Email;
                    TempData[MVCConatants.BirthDate] = modifiedPersonalDetails.BirthDate;
                    TempData[MVCConatants.JoiningDate] = modifiedPersonalDetails.JoiningDate;

                    if (invalidParameter.Equals(MVCConatants.Email) == true)
                    {
                        TempData[MVCConatants.Email] = null;
                        TempData[MVCConatants.ErrMsgInvalid] = MVCConatants.ErrInvalidEmailFormat;
                    }
                    else if (invalidParameter.Equals(MVCConatants.BirthDate) == true)
                    {
                        TempData[MVCConatants.BirthDate] = null;
                        TempData[MVCConatants.ErrMsgInvalid] = MVCConatants.ErrInvalidBirthDate;
                    }
                    else if (invalidParameter.Equals(MVCConatants.JoiningDate) == true)
                    {
                        TempData[MVCConatants.JoiningDate] = null;
                        TempData[MVCConatants.ErrMsgInvalid] = MVCConatants.ErrInvalidJoiningDate;
                    }

                    string userName = modifiedPersonalDetails.Name;
                    return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.PersonalDetailsController, new { userName });
                }

                // Checks if Image is updated or not
                if (modifiedPersonalDetails.IFormFilePhoto != null)
                {
                    byte[]? binaryImage = ConvertImageToBinary(modifiedPersonalDetails.IFormFilePhoto, out string? error);

                    if (string.IsNullOrEmpty(error) == false)
                    {
                        TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrMsgErrorOccured + error;
                        string userName = modifiedPersonalDetails.Name;
                        return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.PersonalDetailsController, new { userName });
                    }

                    if (binaryImage != default && binaryImage.Length > 0)
                    {
                        modifiedPersonalDetails.ImageFileName = modifiedPersonalDetails.IFormFilePhoto.FileName;
                        modifiedPersonalDetails.Image = binaryImage;
                    }
                }

                string json = JsonConvert.SerializeObject(modifiedPersonalDetails);
                var content = new StringContent(json, Encoding.UTF8, MVCConatants.MediaType);
                HttpResponseMessage response = _client.PutAsync(_baseAddress + "/personalDetails/UpdatePersonalDetails", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    string userName = modifiedPersonalDetails.Name;
                    TempData[MVCConatants.MsgAlertMessage] = MVCConatants.MsgUpdatedUserDetails;
                    return RedirectToAction(MVCConatants.IndexMethod, MVCConatants.PersonalDetailsController, new { userName });
                }
                else if (string.Equals(response.ReasonPhrase, MVCConatants.ErrMsgBadRequest, StringComparison.OrdinalIgnoreCase) == true)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUpdateUserFailed;
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
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method to convert IFormFile Image to Byte Array
        /// </summary>
        /// <param name="image">Image</param>
        /// <param name="error">Error Message</param>
        /// <returns>Byte Array</returns>
        private static byte[]? ConvertImageToBinary(IFormFile? image, out string error)
        {
            error = string.Empty;

            try
            {
                if (image == null)
                {
                    error = MVCConatants.ErrMsgImageNull;
                    return default;
                }

                using (var memoryStream = new MemoryStream())
                {
                    image.CopyTo(memoryStream);
                    byte[] binaryImage = memoryStream.ToArray();
                    return binaryImage;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return default;
            }
        }

        /// <summary>
        /// Method to Check Valid Input Data such as Email, BirthDate and Joining Date 
        /// </summary>
        /// <param name="modifiedPersonalDetails">Modified Personal Details of a User</param>
        /// <param name="invalidParameter">Parameter which is invalid</param>
        /// <returns>True is data is valid else false</returns>
        private static bool CheckValidPersonalData(PersonalDetailsModel modifiedPersonalDetails, out string invalidParameter)
        {
            invalidParameter = string.Empty;
            // Check for valid Email 
            string email = modifiedPersonalDetails.Email;
            Regex regex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
            Match match = regex.Match(email);

            if (match.Success == false)
            {
                invalidParameter = MVCConatants.Email;
                return false;
            }

            // Check for Valid BirthDate
            DateTime? birthDate = modifiedPersonalDetails.BirthDate;

            if (birthDate != null)
            {
                int age = DateTime.Now.Year - birthDate.Value.Year;

                if (age <= 0 || 120 < age)
                {
                    invalidParameter = MVCConatants.BirthDate;
                    return false;
                }
            }

            // Check for valid Joining Date
            DateTime? joiningDate = modifiedPersonalDetails.JoiningDate;

            if (birthDate != null && joiningDate != null)
            {
                int JoiningAtAge = Math.Abs(joiningDate.Value.Year - birthDate.Value.Year);
                bool isJoiningGreaterToday = joiningDate > DateTime.Now;

                if (joiningDate != null && (JoiningAtAge < 16 || isJoiningGreaterToday == true))
                {
                    invalidParameter = MVCConatants.JoiningDate;
                    return false;
                }
            }

            // Check if Joining Date is Greater than today's Date
            if (joiningDate != null)
            {
                if (joiningDate > DateTime.Now == true)
                {
                    invalidParameter = MVCConatants.JoiningDate;
                    return false;
                }
            }

            return true;
        }

        #endregion 
    }
}
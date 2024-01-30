namespace SkillSheetMVC.EnumsAndConstants
{
    /// <summary>
    /// Constant class for Skill Sheet MVC 
    /// </summary>
    public class MVCConatants
    {
        #region Controller, Method, View, Group Names

        /// <summary>
        /// Login Controller Name
        /// </summary>
        public const string LoginController = "Login";
        
        /// <summary>
        /// Personal details Controller Name
        /// </summary>
        public const string PersonalDetailsController = "PersonalDetails";
        
        /// <summary>
        /// Admin Controller Name
        /// </summary>
        public const string AdminController = "Admin";
        
        /// <summary>
        /// Forgot Password Controller Name
        /// </summary>
        public const string ForgotPasswordController = "ForgotPassword";
        
        /// <summary>
        /// Index Method
        /// </summary>
        public const string IndexMethod = "Index";
        
        /// <summary>
        /// Group Name
        /// </summary>
        public const string GroupName = "GroupName";
        
        /// <summary>
        /// User Name
        /// </summary>
        public const string UserName = "UserName";
        
        /// <summary>
        /// Delete User Method Name
        /// </summary>
        public const string DeleteUserMethod = "DeleteUser";

        /// <summary>
        /// Edit User Partial View
        /// </summary>
        public const string EditUserPartialView = "EditUserDetails";
        
        /// <summary>
        /// Add User Partial view
        /// </summary>
        public const string AddUserPartialView = "AddNewUser";
        
        /// <summary>
        /// User
        /// </summary>
        public const string GroupUser = "User";
       
        /// <summary>
        /// Admin
        /// </summary>
        public const string GroupAdmin = "Admin";
        
        /// <summary>
        /// Title
        /// </summary>
        public const string Title = "Title";

        /// <summary>
        /// Media Type to exchanging JSON data between clients and servers.
        /// </summary>
        public const string MediaType = "application/json";

        #endregion

        #region Error Messages

        /// <summary>
        /// Error Message Unable to edit User Details
        /// </summary>
        public const string ErrUnableToEdit = "Unable to Edit User Details...!";

        /// <summary>
        /// Error Message Edited Details Cannot be same as Before
        /// </summary>
        public const string ErrSameEditedDetails = "Edited Details Cannot be same as Before...!";

        /// <summary>
        /// Error Message Image is null
        /// </summary>
        public const string ErrMsgImageNull = "Image is null";

        /// <summary>
        /// Error Message Invalid
        /// </summary>
        public const string ErrMsgInvalid = "Invalid";

        /// <summary>
        /// Error Message InvalidParameter
        /// </summary>
        public const string ErrMsgInvalidParameter = "InvalidParameter";

        /// <summary>
        /// Error Message Invalid Birth Date
        /// </summary>
        public const string ErrInvalidBirthDate = "Invalid Birth Date...!";

        /// <summary> 
        /// Error Message Invalid Joining Date
        /// </summary>
        public const string ErrInvalidJoiningDate = "Invalid Joining Date...!";

        /// <summary>
        /// Error Message Email Should end with the format @abc.xyz
        /// </summary>
        public const string ErrInvalidEmailFormat = "Invalid Email...! Email Should end with the format @abc.xyz";

        /// <summary>
        /// Error Message Unable to Get User's Personal Details
        /// </summary>
        public const string ErrUnableToGetPersonalDetails = "Unable to Get User's Personal Details";

        /// <summary>
        /// Error Message Unable to Get User's Modified Personal Details
        /// </summary>
        public const string ErrUnableToGetModifiedPersonalDetails = "Unable to Get User's Modified Personal Details";

        /// <summary>
        /// Error Message Unable to Change Password
        /// </summary>
        public const string ErrUnableToChangePassword = "Unable to Change Password...!";

        /// <summary>
        /// Error Message Wrong Password
        /// </summary>
        public const string ErrWrongPassword = "Wrong Password...!";

        /// <summary>
        /// Error Message Unable to Process Request
        /// </summary>
        public const string ErrUnableToProcess = "Unable to Process Request...!";

        /// <summary>
        /// Error Message API URL is not configured
        /// </summary>
        public const string ErrApiUrlNotConfigured = "API URL is not configured...!";

        /// <summary>
        /// Error Message Unable to get login details
        /// </summary>
        public const string ErrUnableToGetLoginDetails = "Unable to get login details..!";

        /// <summary> 
        /// Error Message Unable to Update User Details
        /// </summary>
        public const string ErrUpdateUserFailed = "Unable to Update User Details..! ";

        /// <summary>
        /// Error Message Unale to get User Details
        /// </summary>
        public const string ErrUnableToGetUserDetails = "Unale to get User Details";

        /// <summary>
        /// Error Message Unale to get Admin Details
        /// </summary>
        public const string ErrUnableToGetAdminDetails = "Unale to get Admin Details";

        /// <summary>
        /// Error Message Input Cannot be Empty
        /// </summary> 
        public const string ErrEmptyInput = "Input Cannot be Empty...!";

        /// <summary>
        /// Error Message User Name is Empty
        /// </summary>
        public const string ErrEmptyUserName = "User Name is Empty...!";

        /// <summary>
        /// Error Message Something Went Wrong
        /// </summary>
        public const string ErrMsgSomethingWentWrong = "Something Went Wrong...!";

        /// <summary>
        /// Error Message Error Occured
        /// </summary>
        public const string ErrMsgErrorOccured = "Error Occured: ";

        /// <summary>
        /// Error Failed Message
        /// </summary>
        public const string ErrFailedMessage = "FailedMessage";

        /// <summary>
        /// Error Session Expired
        /// </summary>
        public const string ErrSessionExpired = "Session Expired...!";

        /// <summary>
        /// Error Not Found
        /// </summary>
        public const string ErrMsgNotFound = "Not Found";

        /// <summary>
        /// Error Bad Request
        /// </summary>
        public const string ErrMsgBadRequest = "Bad Request";

        /// <summary>
        /// Error Login Failed
        /// </summary>
        public const string ErrLoginFailed = "Login Failed...!";

        /// <summary>
        /// Error Message User Already Present
        /// </summary>
        public const string ErrUserAlreadyPresent  = "User Already Present...!";

        /// <summary>
        /// Error Message Unable to Add New User
        /// </summary>
        public const string ErrUnableToAddUser = "Unable to Add New User...!";

        /// <summary>
        /// Error Message New Passwrod and Old Password Doesnot Match
        /// </summary>
        public const string ErrPasswordNotMatch = "New Password and Confirm Password Does not Match...!";

        #endregion

        #region Success Messages

        /// <summary>
        /// Success Message
        /// </summary>
        public const string MsgUpdatedUser = "User Updated Sucessfully...!";

        /// <summary>
        /// Success Message
        /// </summary>
        public const string MsgUserAdded = "User Added Sucessfully...!";

        /// <summary>
        /// Success Message
        /// </summary>
        public const string MsgUpdatedUserDetails = "User Details Updated Successfully...!";

        /// <summary>
        /// Success Message
        /// </summary>
        public const string MsgAlertMessage = "AlertMessage";

        /// <summary>
        /// Success Message
        /// </summary>
        public const string MsgPasswordChangedSucess = "Password Changed Sucessfully...!";

        #endregion

        #region Model Element

        /// <summary>
        /// Name
        /// </summary>
        public const string Name = "Name";

        /// <summary>
        /// Gender
        /// </summary>
        public const string Gender = "Gender";

        /// <summary>
        /// Languages
        /// </summary>
        public const string Languages = "Languages";

        /// <summary>
        /// Database
        /// </summary>
        public const string Database = "Database";

        /// <summary>
        /// Qualification
        /// </summary>
        public const string Qualification = "Qualification";

        /// <summary>
        /// Email
        /// </summary>
        public const string Email = "Email";

        /// <summary>
        /// BirthDate
        /// </summary>
        public const string BirthDate = "BirthDate";

        /// <summary>
        /// JoiningDate
        /// </summary>
        public const string JoiningDate = "JoiningDate";

        #endregion
    }
}
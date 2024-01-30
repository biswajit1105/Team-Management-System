namespace SkillSheetWebAPI
{
    /// <summary>
    /// Constants for Web API
    /// </summary>
    public class APIConstants
    {
        #region Errros

        /// <summary>
        /// Error Message New User Datails are Empty
        /// </summary>
        public const string ErrEmptyNewUserDetails = "New User Details are Empty...!";

        /// <summary>
        /// Error Message Error Occured
        /// </summary>
        public const string ErrMsgErrorOccured = "Error Occured: ";

        /// <summary>
        /// Error Message Password Does not Match
        /// </summary>
        public const string ErrPasswordHashingFailed = "Error Occured in Password Hashing: ";

        /// <summary>
        /// Error Message User Name is Empty
        /// </summary>
        public const string ErrEmptyUserName = "User Name is Empty...!";

        /// <summary>
        /// Errro Message User Details are Empty
        /// </summary>
        public const string ErrEmptyUserDetails = "User Details are Empty...!";

        /// <summary>
        /// Error Message Edited User Details Are Empty
        /// </summary>
        public const string ErrEmptyEditedDetails = "Edited Details are Empty...!";

        /// <summary>
        /// Error Message Unable to find User
        /// </summary>
        public const string ErrUserNotFound = "Unable to Find User...!";

        /// <summary>
        /// Error Message User Name are Not Found For Delete Operation
        /// </summary>
        public const string ErrUserNamesNotFound = "Unable to get User Names for Delete operation...!";

        /// <summary>
        /// Error User Input Details Cannot be Null
        /// </summary>
        public const string ErrNullImput = "Input Details cannot be null...!";

        /// <summary>
        /// Error Message New Passwrod and Old Password Doesnot Match
        /// </summary>
        public const string ErrPasswordNotMatch = "New Password and Confirm Password Does not Match...!";

        /// <summary>
        /// Errro Message Personal Details Are empty
        /// </summary>
        public const string ErrEmptyPersonalDetails = "Personal Details is Empty";

        /// <summary>
        /// Error Message Personal Details not Found
        /// </summary>
        public const string ErrPersonalDetailsNotfound = "Unable to get User's Personal Details";

        /// <summary>
        /// Error Message Image Not Found
        /// </summary>
        public const string ErrImageNotFound = "Image Not Found";

        #endregion

        #region Group Name and Date Format

        /// <summary>
        /// Group Name User
        /// </summary>
        public const string GroupUser = "User";

        /// <summary>
        /// Date Format for Joining Date and Birth Date
        /// </summary>
        public const string DateFormat = "{0:dd/MM/yyyy}";

        #endregion
    }
}
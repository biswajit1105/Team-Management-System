using System.Security.Cryptography;
using System.Text;

namespace SkillSheetWebAPI.Controllers
{
    /// <summary>
    /// Encryption Class
    /// </summary>
    internal class Encryption
    {
        /// <summary>
        /// Method to compute Hash Password
        /// </summary>
        /// <param name="input">Input Password</param>
        /// <param name="error">Error Message</param>
        /// <returns>Hashed Password</returns>
        internal static string ComputeHash(string input, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(input) == true)
            {
                return string.Empty;
            }

            try
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                    byte[] hashBytes = sha256.ComputeHash(inputBytes);
                    string hashedInput = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
                    return hashedInput;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return string.Empty;
            }
        }

        /// <summary>
        /// Method to Match Password
        /// </summary>
        /// <param name="input">User Entered Password</param>
        /// <param name="hash">Old Hashed Password</param>
        /// <param name="error">Error Message</param>
        /// <returns>True if password Matches else false</returns>
        internal static bool VerifyHash(string input, string hash, out string error)
        {
            try
            {
                string inputHash = ComputeHash(input, out error);
                bool isMatch = inputHash.Equals(hash, StringComparison.OrdinalIgnoreCase);
                return isMatch;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}

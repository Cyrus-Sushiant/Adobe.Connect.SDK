using AdobeConnectSDK.Model;
using System;

namespace AdobeConnectSDK.Extensions
{
    /// <summary>
    /// User management extensions
    /// </summary>
    public static class UserManagement
    {
        /// <summary>
        /// Changes a user’s password.
        /// <para>A password can be changed in either of these cases:</para>
        /// <para>- By an Administrator logged in to the account, with or without the user’s old password</para>
        /// <para>- By any Adobe Connect Server user, with the user’s principal-id number, login name, and old password</para>
        /// An Administrator can create rules for valid passwords on the server.
        /// These rules might include, for example, the number and types of characters a password must contain.
        /// If a user submits a new password that does not adhere to the rules, Adobe Connect would throw an error showing that the new password is invalid.
        /// <para>From <seealso href="https://helpx.adobe.com/adobe-connect/webservices/user-update-pwd.html">here</seealso>.</para>
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="oldPassword"></param>
        /// <param name="password"></param>
        /// <returns>
        ///     <see cref="ApiStatus" />
        /// </returns>
        public static ApiStatus UpdatePassword(this AdobeConnectXmlAPI adobeConnectXmlApi, String userId, String password)
        {
            // Password verify will probably be validated on the ui or another class before reaching this method.
            // Having that in mind, i'll send the password-verify equal to password
            var parameters = String.Format("user-id={0}&password={1}&password-verify={1}", userId, password);

            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("user-update-pwd", parameters);

            return s;
        }

        /// <summary>
        /// Changes a user’s password.
        /// <para>A password can be changed in either of these cases:</para>
        /// <para>- By an Administrator logged in to the account, with or without the user’s old password</para>
        /// <para>- By any Adobe Connect Server user, with the user’s principal-id number, login name, and old password</para>
        /// An Administrator can create rules for valid passwords on the server.
        /// These rules might include, for example, the number and types of characters a password must contain.
        /// If a user submits a new password that does not adhere to the rules, Adobe Connect would throw an error showing that the new password is invalid.
        /// <para>From <seealso href="https://helpx.adobe.com/adobe-connect/webservices/user-update-pwd.html">here</seealso>.</para>
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="oldPassword"></param>
        /// <param name="password"></param>
        /// <returns>
        ///     <see cref="ApiStatus" />
        /// </returns>
        public static ApiStatus UpdatePassword(this AdobeConnectXmlAPI adobeConnectXmlApi, String userId, String oldPassword, String password)
        {
            // Password verify will probably be validated on the ui or another class before reaching this method.
            // Having that in mind, i'll send the password-verify equal to password
            var parameters = String.Format("user-id={0}&password-old={1}&password={2}&password-verify={2}", userId, oldPassword, password);

            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("user-update-pwd", parameters);

            return s;
        }
    }
}

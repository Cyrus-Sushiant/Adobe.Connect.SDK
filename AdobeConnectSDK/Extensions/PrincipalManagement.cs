using AdobeConnectSDK.Common;
using AdobeConnectSDK.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace AdobeConnectSDK.Extensions
{
    /// <summary>
    /// Principal management extensions.
    /// </summary>
    public static class PrincipalManagement
    {
        /// <summary>
        /// Provides information about one principal, either a user or a group.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="principalId">The principal identifier.</param>
        /// <returns>
        ///   <see cref="PrincipalInfo" />
        /// </returns>
        /// <exception cref="System.ArgumentNullException">principalId</exception>
        public static PrincipalInfoStatus GetPrincipalInfo(this AdobeConnectXmlAPI adobeConnectXmlApi, string principalId)
        {
            if (String.IsNullOrEmpty(principalId))
                throw new ArgumentNullException("principalId");

            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("principal-info", String.Format("principal-id={0}", principalId));

            var principalInfoStatus = Helpers.WrapBaseStatusInfo<PrincipalInfoStatus>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return null;
            }

            var principalInfo = new PrincipalInfo();

            try
            {
                XElement contactData = s.ResultDocument.XPathSelectElement("//contact");

                if (contactData != null)
                {
                    principalInfo.Contact = XmlSerializerHelpersGeneric.FromXML<Contact>(contactData.CreateReader());
                }

                XElement preferencesData = s.ResultDocument.XPathSelectElement("//preferences");

                if (preferencesData != null)
                {
                    principalInfo.Preferences = XmlSerializerHelpersGeneric.FromXML<Preferences>(preferencesData.CreateReader());
                }

                XElement principalData = s.ResultDocument.XPathSelectElement("//principal");

                if (principalData != null)
                {
                    principalInfo.PrincipalData = XmlSerializerHelpersGeneric.FromXML<Principal>(principalData.CreateReader());
                }

            }
            catch (Exception ex)
            {
                throw;
            }

            principalInfoStatus.Result = principalInfo;

            return principalInfoStatus;
        }

        /// <summary>
        /// Creates or updates a user or group. The user or group (that is, the principal) is created or
        /// updated in the same account as the user making the call.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="principalSetup"><see cref="PrincipalSetup" /></param>
        /// <param name="principal"><see cref="Principal" /></param>
        /// <returns>
        ///   <see cref="ApiStatus" />
        /// </returns>
        public static ApiStatus PrincipalUpdate(this AdobeConnectXmlAPI adobeConnectXmlApi, PrincipalSetup principalSetup, out Principal principal)
        {
            string cmdParams = Helpers.StructToQueryString(principalSetup, true);

            principal = null;

            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("principal-update", cmdParams);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return s;
            }

            principal = XmlSerializerHelpersGeneric.FromXML<Principal>(s.ResultDocument.XPathSelectElement("//principal").CreateReader());

            return s;
        }

        /// <summary>
        /// Removes one or more principals, either users or groups.
        /// To delete principals, you must have Administrator privilege.
        /// To delete multiple principals, specify multiple principal-id parameters. All of the principals
        /// you specify will be deleted.
        /// The principal-id can identify either a user or group. If you specify a user, the user is
        /// removed from any groups the user belongs to. If you specify a group, the group is deleted, but
        /// the users who belong to it are not.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="principalId">The principal identifier.</param>
        /// <returns>
        ///   <see cref="ApiStatus" />
        /// </returns>
        public static ApiStatus PrincipalDelete(this AdobeConnectXmlAPI adobeConnectXmlApi, string[] principalId)
        {
            for (int i = 0; i < principalId.Length; i++)
            {
                principalId[i] = "principal-id=" + principalId[i];
            }

            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("principals-delete", String.Join("&", principalId));

            return s;
        }

        /// <summary>
        /// Changes a user’s password. A password can be changed in either of these cases:
        /// By an Administrator logged in to the account, with or without the user’s old password
        /// By any Connect Enterprise user, with the user’s principal-id number, Login Name, and old password
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="passwordOld">The old password.</param>
        /// <param name="password">The new password.</param>
        /// <returns>
        ///   <see cref="ApiStatus" />
        /// </returns>
        public static ApiStatus PrincipalUpdatePassword(this AdobeConnectXmlAPI adobeConnectXmlApi, string userId, string passwordOld, string password)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("user-update-pwd", String.Format("user-id={0}&password-old={1}&password={2}", userId, passwordOld, password));

            return s;
        }

        /// <summary>
        /// Adds one or more principals to a group, or removes one or more principals from a group.
        /// To update multiple principals and groups, specify multiple trios of group-id, principal-id,
        /// and is-member parameters.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="groupId">The ID of the group in which you want to add or change members.</param>
        /// <param name="principalId">The ID of the principal whose membership status you want to update. Returned by principal-info.</param>
        /// <param name="isMember">Whether the principal is added to (true) or deleted from (false) the group.</param>
        /// <returns>
        ///   <see cref="ApiStatus" />
        /// </returns>
        public static ApiStatus PrincipalGroupMembershipUpdate(this AdobeConnectXmlAPI adobeConnectXmlApi, string groupId, string principalId, bool isMember)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("group-membership-update", String.Format("group-id={0}&principal-id={1}&is-member={2}", groupId, principalId, isMember ? 1 : 0));

            return s;
        }

        /// <summary>
        /// Provides a complete list of users and groups, including primary groups.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="filterBy">Optional filtering parameter.</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<PrincipalListItem> GetPrincipalList(this AdobeConnectXmlAPI adobeConnectXmlApi, string groupId, string filterBy)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("principal-list", String.Format("group-id={0}&{1}", groupId, filterBy));

            var resultStatus = Helpers.WrapBaseStatusInfo<EnumerableResultStatus<PrincipalListItem>>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return resultStatus;
            }

            IEnumerable<XElement> list;

            try
            {
                IEnumerable<XElement> nodeData = s.ResultDocument.XPathSelectElements("//principal-list/principal");
                list = nodeData as IList<XElement> ?? nodeData;
            }
            catch (Exception ex)
            {
                throw;
            }

            resultStatus.Result = list.Select(itemInfo => XmlSerializerHelpersGeneric.FromXML<PrincipalListItem>(itemInfo.CreateReader()));

            return resultStatus;
        }

        /// <summary>
        /// Provides a complete list of users and groups, including primary groups.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<PrincipalListItem> GetPrincipalList(this AdobeConnectXmlAPI adobeConnectXmlApi)
        {
            return adobeConnectXmlApi.GetPrincipalList(String.Empty, String.Empty);
        }

        /// <summary>
        /// Returns true if user is Admin
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="aclId">acl_id of the current user</param>
        /// <returns><see cref="bool"/> bool : user us admin ? true : false</returns>
        public static bool IsAdmin(AdobeConnectXmlAPI adobeConnectXmlApi, string aclId)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("principal-list", "filter-type=admins");

            var principalItem = s.ResultDocument.XPathSelectElements("//principal-list/principal").FirstOrDefault();

            var groupId = principalItem.Attribute("principal-id").Value;

            ApiStatus apiStatus = adobeConnectXmlApi.ProcessApiRequest("principal-list", String.Format("group-id={0}&filter-is-member=true", groupId));

            return (apiStatus.ResultDocument.Descendants("principal").Where(a => a.Attribute("principal-id").Value == aclId).Count() == 1);
        }

        /// <summary>
        /// Returns the list of principals (users or groups) who have permissions to act on a SCO,
        /// principal, or account.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="aclId">*Required.
        /// The ID of a SCO, account, or principal
        /// that a principal has permission to act
        /// on. The acl-id is a sco-id, principalid,
        /// or account-id in other calls.</param>
        /// <param name="principalId">Optional.
        /// The ID of a user or group who has a
        /// permission (even if Denied or not set) to
        /// act on a SCO, an account, or another principal.</param>
        /// <param name="filter">Optional filtering parameter.</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<PermissionInfo> GetPermissionsInfo(this AdobeConnectXmlAPI adobeConnectXmlApi, string aclId, string principalId, string filter)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("permissions-info", String.Format("acl-id={0}&principal-id={1}&filter-definition={2}", aclId, principalId, filter));

            var resultStatus = Helpers.WrapBaseStatusInfo<EnumerableResultStatus<PermissionInfo>>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return resultStatus;
            }

            IEnumerable<XElement> list;

            try
            {
                IEnumerable<XElement> nodeData = s.ResultDocument.XPathSelectElements("//permissions/principal");
                list = nodeData as IList<XElement> ?? nodeData;
            }
            catch (Exception ex)
            {
                throw;
            }

            resultStatus.Result = list.Select(itemInfo => XmlSerializerHelpersGeneric.FromXML<PermissionInfo>(itemInfo.CreateReader()));

            return resultStatus;
        }

        /// <summary>
        /// Resets all permissions any principals have on a SCO to the permissions of its parent SCO. If
        /// the parent has no permissions set, the child SCO will also have no permissions.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="aclId">*Required.
        /// The ID of a SCO, account, or principal
        /// that a principal has permission to act
        /// on. The acl-id is a sco-id, principalid,
        /// or account-id in other calls.</param>
        /// <returns>
        ///   <see cref="ApiStatus" />
        /// </returns>
        public static ApiStatus PermissionsReset(this AdobeConnectXmlAPI adobeConnectXmlApi, string aclId)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("permissions-reset", String.Format("acl-id={0}", aclId));

            return s;
        }

        /// <summary>
        /// Updates the permissions a principal has to access a SCO, using a trio of principal-id, aclid,
        /// and permission-id. To update permissions for multiple principals or objects, specify
        /// multiple trios. You can update more than 200 permissions in a single call to permissionsupdate.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="aclId">*Required.
        /// The ID of a SCO, account, or principal
        /// that a principal has permission to act
        /// on. The acl-id is a sco-id, principalid,
        /// or account-id in other calls.</param>
        /// <param name="principalId">*Required.
        /// The ID of a user or group who has a
        /// permission (even if Denied or not set) to
        /// act on a SCO, an account, or another principal.</param>
        /// <param name="permissionId">*Required. <see cref="PermissionId" />.</param>
        /// <returns>
        ///   <see cref="ApiStatus" />
        /// </returns>
        public static ApiStatus PermissionsUpdate(this AdobeConnectXmlAPI adobeConnectXmlApi, string aclId, string principalId, PermissionId permissionId)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("permissions-update", String.Format("acl-id={0}&principal-id={1}&permission-id={2}", aclId, principalId, Helpers.EnumToString(permissionId)));

            return s;
        }

        /// <summary>
        /// The server defines a special principal, public-access, which combines with values of permission-id to create special access permissions to meetings.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="aclId">*Required.
        /// The ID of a SCO, account, or principal
        /// that a principal has permission to act
        /// on. The acl-id is a sco-id, principalid,
        /// or account-id in other calls.</param>
        /// <param name="permissionId">*Required. <see cref="SpecialPermissionId" />.</param>
        /// <returns>
        ///   <see cref="ApiStatus" />
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static ApiStatus SpecialPermissionsUpdate(this AdobeConnectXmlAPI adobeConnectXmlApi, string aclId, SpecialPermissionId permissionId)
        {
            switch (permissionId)
            {
                case SpecialPermissionId.Denied:
                    return PermissionsUpdate(adobeConnectXmlApi, aclId, "public-access", PermissionId.Denied);
                case SpecialPermissionId.Remove:
                    return PermissionsUpdate(adobeConnectXmlApi, aclId, "public-access", PermissionId.Remove);
                case SpecialPermissionId.ViewHidden:
                    return PermissionsUpdate(adobeConnectXmlApi, aclId, "public-access", PermissionId.ViewHidden);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Subscribes specific participant to specific Course/event
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="courseSco">The course sco.</param>
        /// <param name="principalId">principal/participant id</param>
        /// <returns>
        ///   <see cref="ApiStatus" />
        /// </returns>
        public static ApiStatus ParticipantSubscribe(this AdobeConnectXmlAPI adobeConnectXmlApi, string courseSco, string principalId)
        {
            return PermissionsUpdate(adobeConnectXmlApi, courseSco, principalId, PermissionId.View);
        }

        /// <summary>
        /// Subscribes specific participant to specific Course/event
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="courseSco">The course sco.</param>
        /// <param name="principalId">principal/participant id</param>
        /// <param name="permissionId">*Required. <see cref="PermissionId" />.</param>
        /// <returns>
        ///   <see cref="ApiStatus" />
        /// </returns>
        public static ApiStatus ParticipantSubscribe(this AdobeConnectXmlAPI adobeConnectXmlApi, string courseSco, string principalId, PermissionId permissionId)
        {
            return PermissionsUpdate(adobeConnectXmlApi, courseSco, principalId, permissionId);
        }

        /// <summary>
        /// UnSubscribes specific participant from specific Course/event
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="courseSco">The course sco.</param>
        /// <param name="principalId">principal/participant id</param>
        /// <returns>
        ///   <see cref="ApiStatus" />
        /// </returns>
        public static ApiStatus ParticipantUnsubscribe(this AdobeConnectXmlAPI adobeConnectXmlApi, string courseSco, string principalId)
        {
            return PermissionsUpdate(adobeConnectXmlApi, courseSco, principalId, PermissionId.Remove);
        }
    }
}

using System;
using System.Xml.Serialization;

namespace AdobeConnectSDK.Model
{
    /// <summary>
    /// PrincipalInfo structure
    /// </summary>
    [Serializable]
    public class PrincipalInfo
    {
        public Preferences Preferences;
        public Principal PrincipalData;
        public Contact Contact;
    }

    [Serializable]
    [XmlRoot("contact")]
    public class Contact
    {
        [XmlElement("email")]
        public string Email;

        [XmlElement("first-name")]
        public string FirstName;

        [XmlElement("last-name")]
        public string LastName;
    }

    /// <summary>
    /// Principal structure
    /// </summary>
    [Serializable]
    [XmlRoot("principal")]
    public class Principal
    {
        [XmlAttribute("account-id")]
        public string AccountId;

        [XmlAttribute("principal-id")]
        public string PrincipalId;

        [XmlAttribute("has-children")]
        public bool HasChildren;

        [XmlAttribute("is-hidden")]
        public bool IsHidden;

        [XmlAttribute("is-primary")]
        public bool IsPrimary;

        [XmlElement("ext-Login")]
        public string ExtLogin;

        [XmlElement("login")]
        public string Login;

        [XmlElement("name")]
        public string Name;

        [XmlElement("email")]
        public string Email;

        [XmlElement("first-name")]
        public string FirstName;

        [XmlElement("last-name")]
        public string LastName;
    }


    /// <summary>
    /// Preferences structure
    /// </summary>
    [Serializable]
    [XmlRoot("preferences")]
    public class Preferences
    {
        [XmlAttribute("acl-id")]
        public string AclId;

        [XmlAttribute("lang")]
        public string Language;

        [XmlAttribute("time-zone-id")]
        public string TimeZoneId;

    }


    [Serializable]
    [XmlRoot("principal")]
    public class PrincipalListItem
    {
        [XmlAttribute("account-id")]
        public string AccountId;

        [XmlAttribute("principal-id")]
        public string PrincipalId;

        [XmlAttribute("has-children")]
        public bool HasChildren;

        [XmlAttribute("is-hidden")]
        public bool IsHidden;

        [XmlAttribute("is-primary")]
        public bool IsPrimary;

        [XmlElement("login")]
        public string Login;

        [XmlElement("name")]
        public string Name;

        [XmlElement("email")]
        public string Email;

        [XmlElement("display-uid")]
        public string DisplayUid;
    }

    public enum PrincipalTypes
    {
        admins,
        authors,
        course_admins,
        event_admins,
        event_group,
        everyone,
        external_group,
        external_user,
        group,
        guest,
        learners,
        live_admins,
        seminar_admins,
        user
    }

    /// <summary>
    /// PrincipalSetup structure
    /// </summary>
    public class PrincipalSetup
    {
        /// <summary>
        /// The Type of principal. Use only when creating a new principal
        /// </summary>
        [XmlElement("type")]
        public PrincipalTypes PrincipalType;

        /// <summary>
        /// The principal's new Login Name, usually
        /// the principal's e-mail address. Must be
        /// unique on the server. Required to create
        /// or update a user. Do not use with groups.
        /// </summary>
        public string Login;

        /// <summary>
        /// The new group's Name. Use only when
        /// creating a new group. Required to create
        /// a group.
        /// </summary>
        public string Name;

        /// <summary>
        /// The user's new first Name. Use only with
        /// users, not with groups. Required to create a user
        /// </summary>
        [XmlElement("first-name")]
        public string FirstName;

        /// <summary>
        /// The new last Name to assign to the user.
        /// Required to create a user. Do not use with groups.
        /// </summary>
        [XmlElement("last-name")]
        public string LastName;

        /// <summary>
        /// The user's e-mail address. Can be
        /// different from the Login. Be sure to
        /// specify a value if you use sendemail=true.
        /// </summary>
        public string Email;

        /// <summary>
        /// The new user's password. Use only when creating a new user.
        /// </summary>
        public string Password;

        /// <summary>
        /// The new group's Description. Use only when creating a new group.
        /// </summary>
        public string Description;

        /// <summary>
        /// Whether the principal has children. If the
        /// principal is a group, use 1 or true. If the
        /// principal is a user, use 0 or false.
        /// </summary>
        [XmlElement("has-children")]
        public bool HasChildren;

        /// <summary>
        /// The ID of the principal that has
        /// information you want to update. Required
        /// to update a user or group, but do not use
        /// to create either.
        /// </summary>
        [XmlAttribute("principal-id")]
        public string PrincipalId;

        /// <summary>
        /// A flag indicating whether the server
        /// should send an e-mail to the principal with
        /// account and Login information.
        /// </summary>
        [XmlElement("send-email")]
        public bool SendEmail;
    }
}

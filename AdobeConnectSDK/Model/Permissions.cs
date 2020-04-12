using AdobeConnectSDK.Common;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AdobeConnectSDK.Model
{

    /// <summary>
    /// PermissionInfo structure
    /// </summary>
    [Serializable]
    [XmlRoot("principal")]
    public class PermissionInfo
    {
        [XmlAttribute("principal-id")]
        public string PrincipalId;

        [XmlAttribute("has-children")]
        public bool HasChildren;

        [XmlAttribute("is-primary")]
        public bool IsPrimary;

        [XmlIgnore]
        public PermissionId PermissionId;

        [XmlAttribute("permission-id")]
        internal string PermissionIdRaw
        {
            get { return Helpers.EnumToString(this.PermissionId); }
            set
            {
                this.PermissionId = Helpers.ReflectEnum<PermissionId>(value);
            }
        }

        [XmlAttribute("training-group-id")]
        public string TrainingGroupId;

        [XmlElement("login")]
        public string Login;

        [XmlElement("name")]
        public string Name;

        [XmlElement("description")]
        public string Description;
    }

    public enum SpecialPermissionId
    {
        /// <summary>
        /// The Me is public, and anyone who has the URL for the Me can enter the room.
        /// </summary>
        ViewHidden,

        /// <summary>
        /// The Me is protected, and only registered users and accepted guests can enter the room.
        /// </summary>
        Remove,

        /// <summary>
        /// The Me is private, and only registered users and participants can enter the room.
        /// </summary>
        Denied,
    }

    public enum PermissionId
    {
        None,
        /// <summary>
        /// The principal has full access to an account and can create users, View any
        /// Folder, or launch any SCO. However, the principal cannot Publish Content or
        /// act as Host of an Acrobat Connect Professional Me.
        /// </summary>
        Admin,
        Author,
        Learner,
        /// <summary>
        /// The principal can View, but cannot modify, the SCO. The principal can take a
        /// Course, attend a Me as participant, or View a Folder's Content.
        /// </summary>
        View,
        /// <summary>
        /// Available for meetings only. The principal is Host of a Me and can
        /// create the Me or act as presenter, even without View permission on the
        /// Me's parent Folder.
        /// </summary>
        ViewHidden,
        /// <summary>
        /// The Me is public, and anyone who has the URL for the Me can enter the room.
        /// </summary>
        PublicAccess,
        /// <summary>
        /// Public, equivalent to Anyone who has the URL for the Me can enter the room.
        /// </summary>
        Host,
        /// <summary>
        /// Available for meetings only. The principal is presenter of a Me and
        /// can present Content, share a screen, send text messages, moderate
        /// questions, create text notes, broadcast audio and video, and push Content
        /// from web links.
        /// </summary>

        //mini-host is the correct value to be sent
        [Description("mini-host")]
        MiniHost,
        /// <summary>
        /// Available for meetings only. The principal does not have participant,
        /// presenter or Host permission to attend the Me. If a user is already
        /// attending a live Me, the user is not removed from the Me until the
        /// Session times out.
        /// </summary>
        Remove,
        /// <summary>
        /// Available for SCOs other than meetings. The principal can Publish or
        /// update the SCO. The Publish permission includes View and allows the
        /// principal to View reports related to the SCO. On a Folder, Publish does not
        /// allow the principal to create new subfolders or set permissions.
        /// </summary>
        Publish,
        /// <summary>
        /// Available for SCOs other than meetings or courses. The principal can
        /// View, delete, move, edit, or set permissions on the SCO. On a Folder, the
        /// principal can create subfolders or View reports on Folder Content.
        /// </summary>
        Manage,
        /// <summary>
        /// Available for SCOs other than meetings. The principal cannot View,
        /// access, or Manage the SCO.
        /// </summary>
        Denied,
    }
}

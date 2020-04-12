using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;

namespace AdobeConnectSDK.Model
{
    [Serializable]
    public abstract class BaseStatusInfo
    {
        public StatusCodes Code { get; internal set; }

        public StatusSubCodes SubCode { get; internal set; }

        public string InvalidField { get; internal set; }

        public string Exception { get; internal set; }

        public Exception InnerException { get; internal set; }

        public string SessionInfo { get; internal set; }
    }

    [Serializable]
    public class PrincipalInfoStatus : BaseStatusInfo
    {
        public PrincipalInfo Result { get; internal set; }
    }

    [Serializable]
    public class UserInfoStatus : BaseStatusInfo
    {
        public UserInfo Result { get; internal set; }
    }

    /// <summary>
    /// API operation status information, and result document reference.
    /// </summary>
    [Serializable]
    public class ApiStatus : BaseStatusInfo
    {
        public XDocument ResultDocument { get; internal set; }
    }

    [Serializable]
    public class LoginStatus : BaseStatusInfo
    {
        public bool Result { get; internal set; }
    }

    [Serializable]
    public class MeetingDetailStatus : BaseStatusInfo
    {
        public MeetingDetail Result { get; internal set; }
    }

    [Serializable]
    public class QuotaInfoStatus : BaseStatusInfo
    {
        public QuotaInfo Result { get; internal set; }
    }

    [Serializable]
    public class EnumerableResultStatus<T> : BaseStatusInfo
    {
        public IEnumerable<T> Result { get; internal set; }
    }

    /// <summary>
    /// API operation status codes.
    /// </summary>
    public enum StatusCodes
    {
        NotSet,

        /// <summary>
        /// Indicates that the action has completed successfully.
        /// </summary>
        OK,

        /// <summary>
        /// Indicates that a call is invalid in some way. The Invalid element provides more detail.
        /// </summary>
        [Description("invalid")]
        Invalid,

        /// <summary>
        /// Indicates that you don�t have permission to call the action. The subcode
        /// attribute provides more details.
        /// </summary>
        [Description("no-access")]
        NoAccess,

        /// <summary>
        /// Indicates that there is no data available (in response to an action that
        /// would normally result in returning data). Usually indicates that there is
        /// no item with the ID you specified. To resolve the error, change the
        /// specified ID to that of an item that exists.
        /// </summary>
        [Description("no-data")]
        NoData,

        /// <summary>
        /// Indicates that the action should have returned a single result but is
        /// actually returning multiple results. For example, if there are multiple
        /// users with the same user Name and password, and you call the Login
        /// action using that user Name and password as parameters, the system
        /// cannot determine which user to log you in as, so it returns a too-muchdata error.
        /// </summary>
        [Description("too-much-data")]
        TooMuchData,

        [Description("internal-error")]
        InternalError
    }

    /// <summary>
    /// API operation sub codes.
    /// </summary>
    public enum StatusSubCodes
    {
        NotSet,

        /// <summary>
        /// The customer account has Expired.
        /// </summary>
        [Description("account-expired")]
        AccountExpired,

        /// <summary>
        /// Based on the supplied credentials, you don�t have permission to call the action.
        /// </summary>
        [Description("Denied")]
        Denied,

        /// <summary>
        /// The user is not logged in. To resolve the error, log in (using the Login action) before you make the call. For more information, see Login.
        /// </summary>
        [Description("no-Login")]
        NoLogin,

        /// <summary>
        /// The account limits have been reached or exceeded.
        /// </summary>
        [Description("no-quota")]
        NoQuota,

        /// <summary>
        /// The required resource is unavailable.
        /// </summary>
        [Description("not-available")]
        NotAvailable,

        /// <summary>
        /// You must use SSL to call this action.
        /// </summary>
        [Description("not-secure")]
        NotSecure,

        /// <summary>
        /// The account is not yet activated.
        /// </summary>
        [Description("pending-activation")]
        PendingActivation,

        /// <summary>
        /// The account�s license agreement has not been settled.
        /// </summary>
        [Description("pending-license")]
        PendingLicense,

        /// <summary>
        /// The Course or tracking Content has Expired.
        /// </summary>
        [Description("sco-expired")]
        ScoExpired,

        /// <summary>
        /// The Me or Course has not started.
        /// </summary>
        [Description("sco-not-started")]
        ScoNotStarted,

        //--------------------
        /// <summary>
        /// The call attempted to add a Duplicate item in a context where
        /// uniqueness is required.
        /// </summary>
        [Description("duplicate")]
        Duplicate,

        /// <summary>
        /// The requested operation violates integrity rules (for example, moving
        /// a Folder into itself).
        /// </summary>
        [Description("illegal-operation")]
        IllegalOperation,

        /// <summary>
        /// The requested information does not exist.
        /// </summary>
        [Description("no-such-item")]
        NoSuchItem,

        /// <summary>
        /// The value is outside the permitted Range of values.
        /// </summary>
        [Description("range")]
        Range,

        /// <summary>
        /// A required parameter is Missing.
        /// </summary>
        [Description("missing")]
        Missing,

        /// <summary>
        /// A passed parameter had the wrong Format.
        /// </summary>
        [Description("format")]
        Format
    }
}

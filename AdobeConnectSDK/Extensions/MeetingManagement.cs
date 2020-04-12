using AdobeConnectSDK.Common;
using AdobeConnectSDK.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace AdobeConnectSDK.Extensions
{
    /// <summary>
    /// Meeting extensions.
    /// </summary>
    public static class MeetingManagement
    {
        /// <summary>
        /// List all meetings on the server
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<MeetingItem> GetAllMeetings(this AdobeConnectXmlAPI adobeConnectXmlApi)
        {
            return GetAllMeetings(adobeConnectXmlApi, null);
        }

        /// <summary>
        /// List all meetings on the server
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="likeName">filter like the Name of the Meeting</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<MeetingItem> GetAllMeetings(this AdobeConnectXmlAPI adobeConnectXmlApi, string likeName)
        {
            string filterName = String.Empty;

            if (!String.IsNullOrEmpty(likeName))
                filterName = @"&filter-like-name=" + likeName;

            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("report-bulk-objects", "filter-type=meeting" + filterName);

            var resultStatus = Helpers.WrapBaseStatusInfo<EnumerableResultStatus<MeetingItem>>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null || s.ResultDocument.Root == null)
            {
                return resultStatus;
            }

            IEnumerable<XElement> list;

            try
            {
                IEnumerable<XElement> meetingsData = s.ResultDocument.XPathSelectElements("//report-bulk-objects/row");
                list = meetingsData as IList<XElement> ?? meetingsData;
            }
            catch (Exception ex)
            {
                throw;
            }

            resultStatus.Result = adobeConnectXmlApi.PreProcessMeetingItems(list, new XmlRootAttribute("row"));

            return resultStatus;
        }

        /// <summary>
        /// Returns a list of SCOs within another SCO. The enclosing SCO can be a Folder, Meeting, or
        /// Curriculum.
        /// In general, the contained SCOs can be of any type meetings, courses, curriculums, Content,
        /// events, folders, trees, or links (see the list in Type). However, the Type of the contained SCO
        /// needs to be valid for the enclosing SCO. For example, courses are contained within
        /// curriculums, and Meeting Content is contained within meetings.
        /// Because folders are SCOs, the returned list includes SCOs and subfolders at the next
        /// hierarchical level, but not the contents of the subfolders. To include the subfolder contents,
        /// call sco-expanded-contents.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="scoId">Room/Folder ID</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<MeetingItem> GetMeetingsInRoom(this AdobeConnectXmlAPI adobeConnectXmlApi, string scoId)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("sco-contents", String.Format("sco-id={0}", scoId));

            var resultStatus = Helpers.WrapBaseStatusInfo<EnumerableResultStatus<MeetingItem>>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return resultStatus;
            }

            IEnumerable<XElement> list;

            try
            {
                IEnumerable<XElement> meetingsData = s.ResultDocument.XPathSelectElements("//sco");
                list = meetingsData as IList<XElement> ?? meetingsData;
            }
            catch (Exception ex)
            {
                throw;
            }

            resultStatus.Result = adobeConnectXmlApi.PreProcessMeetingItems(list, new XmlRootAttribute("sco"));

            return resultStatus;
        }

        /// <summary>
        /// Provides information about a SCO on Connect Enterprise. The object can have any valid
        /// SCO Type. See Type for a list of the allowed SCO types.
        /// The response includes the account the SCO belongs to, the dates it was created and last
        /// modified, the owner, the URL that reaches it, and other data. For some types of SCOs, the
        /// response also includes information about a template from which this SCO was created.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="scoId">Meeting id</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static MeetingDetailStatus GetMeetingDetail(this AdobeConnectXmlAPI adobeConnectXmlApi, string scoId)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("sco-info", String.Format("sco-id={0}", scoId));

            var resultStatus = Helpers.WrapBaseStatusInfo<MeetingDetailStatus>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return resultStatus;
            }

            try
            {
                XElement nodeData = s.ResultDocument.XPathSelectElement("//sco");

                if (nodeData == null || !nodeData.HasElements)
                {
                    return null;
                }

                resultStatus.Result = XmlSerializerHelpersGeneric.FromXML<MeetingDetail>(nodeData.CreateReader(), new XmlRootAttribute("sco"));
                resultStatus.Result.FullUrl = adobeConnectXmlApi.ResolveFullUrl(resultStatus.Result.UrlPath);

                return resultStatus;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Creates a new Meeting.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="meetingUpdateItem"><see cref="MeetingUpdateItem" /></param>
        /// <param name="meetingDetail"><see cref="MeetingDetail" /></param>
        /// <returns>
        ///   <see cref="ApiStatus" />
        /// </returns>
        public static ApiStatus MeetingCreate(this AdobeConnectXmlAPI adobeConnectXmlApi, MeetingUpdateItem meetingUpdateItem, out MeetingDetail meetingDetail)
        {
            meetingDetail = null;
            if (meetingUpdateItem == null)
                return null;

            if (String.IsNullOrEmpty(meetingUpdateItem.FolderId))
            {
                return Helpers.WrapStatusException(StatusCodes.Invalid, StatusSubCodes.Format, new ArgumentNullException("MeetingItem", "FolderID must be set to create new item"));
            }

            if (meetingUpdateItem.MeetingItemType == SCOtype.NotSet)
            {
                return Helpers.WrapStatusException(StatusCodes.Invalid, StatusSubCodes.Format, new ArgumentNullException("MeetingItem", "SCOtype must be set"));
            }

            meetingUpdateItem.ScoId = null;

            return adobeConnectXmlApi.ScoUpdate(meetingUpdateItem, out meetingDetail);
        }

        /// <summary>
        /// Updates the Meeting.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="meetingUpdateItem"><see cref="MeetingUpdateItem" /></param>
        /// <returns>
        ///   <see cref="ApiStatus" />
        /// </returns>
        public static ApiStatus MeetingUpdate(this AdobeConnectXmlAPI adobeConnectXmlApi, MeetingUpdateItem meetingUpdateItem)
        {

            if (meetingUpdateItem == null)
                return null;

            if (String.IsNullOrEmpty(meetingUpdateItem.ScoId))
            {
                return Helpers.WrapStatusException(StatusCodes.Invalid, StatusSubCodes.Format, new ArgumentNullException("MeetingItem", "ScoId must be set to update existing item"));
            }

            meetingUpdateItem.FolderId = null;

            return adobeConnectXmlApi.ScoUpdate(meetingUpdateItem, out MeetingDetail meetingDetail);
        }

        /// <summary>
        /// Returns a list of SCOs within another SCO. The enclosing SCO can be a Folder, Meeting, or
        /// Curriculum.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="scoId">Room/Folder ID</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<XElement> GetMeetingsInRoomRaw(this AdobeConnectXmlAPI adobeConnectXmlApi, string scoId)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("sco-contents", String.Format("sco-id={0}", scoId));

            var resultStatus = Helpers.WrapBaseStatusInfo<EnumerableResultStatus<XElement>>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return resultStatus;
            }

            resultStatus.Result = s.ResultDocument.XPathSelectElements("//sco");

            return resultStatus;
        }

        /// <summary>
        /// Provides result from calling GetSCOshortcuts(),
        /// with conditional filtering applied: scoItem.Type.Equals("meetings")
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<ScoShortcut> GetMeetingShortcuts(this AdobeConnectXmlAPI adobeConnectXmlApi)
        {
            EnumerableResultStatus<ScoShortcut> itemData = GetSCOshortcuts(adobeConnectXmlApi);

            IEnumerable<ScoShortcut> meetingShortcuts = itemData.Result.Where(shortcut => shortcut.Type == "meetings");
            itemData.Result = meetingShortcuts;
            return itemData;
        }

        /// <summary>
        /// Provides result from calling GetSCOshortcuts(),
        /// with conditional filtering applied: scoItem.Type.Equals("meetings")
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<ScoShortcut> GeSharedtMeetingShortcuts(this AdobeConnectXmlAPI adobeConnectXmlApi)
        {
            EnumerableResultStatus<ScoShortcut> itemData = GetSCOshortcuts(adobeConnectXmlApi);

            IEnumerable<ScoShortcut> meetingShortcuts = itemData.Result.Where(shortcut => shortcut.Type == "meetings");
            itemData.Result = meetingShortcuts;
            return itemData;
        }
        /// <summary>
        /// Provides information about all Acrobat Connect meetings for which the user is a Host, invited
        /// participant, or registered guest. The Meeting can be scheduled in the past, present, or future.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<MeetingItem> GetMyMeetings(this AdobeConnectXmlAPI adobeConnectXmlApi)
        {
            return GetMyMeetings(adobeConnectXmlApi, null);
        }

        /// <summary>
        /// Provides information about all Acrobat Connect meetings for which the user is a Host, invited
        /// participant, or registered guest. The Meeting can be scheduled in the past, present, or future.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="likeName">filter like the Name of the Meeting</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<MeetingItem> GetMyMeetings(this AdobeConnectXmlAPI adobeConnectXmlApi, string likeName)
        {
            string filterName = null;

            if (!String.IsNullOrEmpty(likeName))
                filterName = @"filter-like-name=" + likeName;

            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("report-my-meetings", filterName);

            var resultStatus = Helpers.WrapBaseStatusInfo<EnumerableResultStatus<MeetingItem>>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return resultStatus;
            }

            IEnumerable<XElement> list;

            try
            {
                IEnumerable<XElement> meetingsData = s.ResultDocument.XPathSelectElements("//my-meetings/meeting");
                list = meetingsData as IList<XElement> ?? meetingsData;
            }
            catch (Exception ex)
            {
                throw;
            }

            resultStatus.Result = adobeConnectXmlApi.PreProcessMeetingItems(list, new XmlRootAttribute("meeting"));

            return resultStatus;
        }

        /// <summary>
        /// Method is intented to retrieve data from AC 'Content' Folder. E.g.: Quizzes
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="scoId">The sco identifier.</param>
        /// <returns>
        ///   <see cref="ApiStatus" />, containing ResultDocument if status code is OK and result document is not null; otherwise, null.
        /// </returns>
        public static ApiStatus GetQuizzesInRoom(this AdobeConnectXmlAPI adobeConnectXmlApi, string scoId)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("sco-contents", String.Format("sco-id={0}", scoId));

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return null;
            }

            return s;
        }

        /// <summary>
        /// Provides information about the folders relevant to the current user. These include a Folder for
        /// the user’s current meetings, a Folder for the user’s Content, as well as folders above them in the
        /// navigation hierarchy.
        /// To determine the URL of a SCO, concatenate the url-path returned by sco-info, scocontents,
        /// or sco-expanded-contents with the domain-Name returned by sco-shortcuts.
        /// For example, you can concatenate these two strings:
        /// - http://test.server.com (the domain-Name returned by sco-shortcuts)
        /// - /f2006123456/ (the url-path returned by sco-info, sco-contents, or scoexpanded-contents)
        /// The result is this URL: http://test.server.com/f2006123456/
        /// You can also call sco-contents with the sco-id of a Folder returned by sco-shortcuts to
        /// see the contents of the Folder.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<ScoShortcut> GetSCOshortcuts(this AdobeConnectXmlAPI adobeConnectXmlApi)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("sco-shortcuts", null);

            var resultStatus = Helpers.WrapBaseStatusInfo<EnumerableResultStatus<ScoShortcut>>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return resultStatus;
            }

            IEnumerable<XElement> list;

            try
            {
                IEnumerable<XElement> nodeData = s.ResultDocument.XPathSelectElements("//shortcuts/sco");
                list = nodeData as IList<XElement> ?? nodeData;
            }
            catch (Exception ex)
            {
                throw;
            }

            resultStatus.Result = list.Select(itemInfo => XmlSerializerHelpersGeneric.FromXML<ScoShortcut>(itemInfo.CreateReader()));

            return resultStatus;
        }

        /// <summary>
        /// Returns the list of all rooms
        /// </summary>
        /// <remarks This function facilates the need to return the list of all 
        /// urls/rooms for admin view
        /// <returns><see cref="List<List<bool>>"/>List of List of strings {}</returns>
        public static EnumerableResultStatus<MeetingItem> GetSharedList(AdobeConnectXmlAPI adobeConnectXmlApi)
        {

            var sharedMeetings = adobeConnectXmlApi.GetMeetingShortcuts().Result.FirstOrDefault<ScoShortcut>();

            var scoId = (sharedMeetings as ScoShortcut).ScoId;

            var expandedContents = adobeConnectXmlApi.ProcessApiRequest("sco-expanded-contents", String.Format("sco-id={0}&filter-type=meeting", scoId));

            var resultStatus = Helpers.WrapBaseStatusInfo<EnumerableResultStatus<MeetingItem>>(expandedContents);

            var folder = expandedContents.ResultDocument.Descendants("sco").Where(p => p.Attribute("folder-id").Value == scoId);

            resultStatus.Result = folder.Select(meeting => XmlSerializerHelpersGeneric.FromXML<MeetingItem>(meeting.CreateReader(), new XmlRootAttribute("sco")));

            return resultStatus;
        }

    }
}

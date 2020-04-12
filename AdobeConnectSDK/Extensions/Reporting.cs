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
    /// Reporting extensions.
    /// </summary>
    public static class Reporting
    {
        /// <summary>
        /// Provides information about each event the current user has attended or is scheduled to attend.
        /// The user can be either a Host or a participant in the event. The events returned are those in the
        /// user’s my-events Folder.
        /// To obtain information about all events on your Enterprise Server or in your Enterprise Hosted
        /// account, call sco-shortcuts to get the sco-id of the events Folder. Then, call scocontents
        /// with the sco-id to list all events.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<EventInfo> ReportMyEvents(this AdobeConnectXmlAPI adobeConnectXmlApi)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("report-my-events", null);

            var resultStatus = Helpers.WrapBaseStatusInfo<EnumerableResultStatus<EventInfo>>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return resultStatus;
            }

            IEnumerable<XElement> list;

            try
            {
                IEnumerable<XElement> nodeData = s.ResultDocument.XPathSelectElements("//my-events/event");
                list = nodeData as IList<XElement> ?? nodeData;
            }
            catch (Exception ex)
            {
                throw;
            }

            resultStatus.Result = list.Select(itemInfo => XmlSerializerHelpersGeneric.FromXML<EventInfo>(itemInfo.CreateReader(), new XmlRootAttribute("event")));

            return resultStatus;
        }

        /// <summary>
        /// Returns the number of concurrent Meeting participants you can have is determined by your Adobe Connect license.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <returns>
        ///   <see cref="QuotaInfoStatus" />
        /// </returns>
        public static QuotaInfoStatus Report_Quotas(this AdobeConnectXmlAPI adobeConnectXmlApi)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("report-quotas", String.Empty);

            var resultStatus = Helpers.WrapBaseStatusInfo<QuotaInfoStatus>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return resultStatus;
            }

            try
            {
                resultStatus.Result = XmlSerializerHelpersGeneric.FromXML<QuotaInfo>(s.ResultDocument.Descendants("report-quotas").FirstOrDefault().CreateReader());
                return resultStatus;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns bulk questions information
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="filterBy">optional 'filter by' params</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<XElement> Report_BulkQuestions(this AdobeConnectXmlAPI adobeConnectXmlApi, string filterBy)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("report-bulk-questions", String.Format("{0}", filterBy));

            var resultStatus = Helpers.WrapBaseStatusInfo<EnumerableResultStatus<XElement>>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return resultStatus;
            }

            resultStatus.Result = s.ResultDocument.XPathSelectElements("//row");

            return resultStatus;
        }

        /// <summary>
        /// Returns information about principal-to-SCO transactions on your server or in your hosted account.
        /// A transaction is an instance of one principal visiting one SCO. The SCO can be an Acrobat
        /// Connect Professional Meeting, Course, document, or any Content on the server.
        /// Note: this call to report-bulk-consolidated-transactions, with filter-Type=Meeting, returns only
        /// users who logged in to the Meeting as participants, not users who entered the Meeting as guests.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="filterBy">optional 'filter by' params</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<TransactionInfo> Report_ConsolidatedTransactions(this AdobeConnectXmlAPI adobeConnectXmlApi, string filterBy)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("report-bulk-consolidated-transactions", filterBy);

            var resultStatus = Helpers.WrapBaseStatusInfo<EnumerableResultStatus<TransactionInfo>>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return resultStatus;
            }

            IEnumerable<XElement> list;

            try
            {
                IEnumerable<XElement> nodeData = s.ResultDocument.XPathSelectElements("//row");
                list = nodeData as IList<XElement> ?? nodeData;
            }
            catch (Exception ex)
            {
                throw;
            }

            resultStatus.Result = list.Select(itemInfo => XmlSerializerHelpersGeneric.FromXML<TransactionInfo>(itemInfo.CreateReader(), new XmlRootAttribute("row")));

            return resultStatus;
        }

        /// <summary>
        /// Provides information about all the interactions users have had with a certain quiz. An
        /// interaction identifies all answers one user makes to one quiz question. If a user answers the
        /// same question more than once, all answers are part of the same interaction and have the same interaction-id.
        /// This report provides information about every answer that any user has ever given to questions
        /// on a quiz. You can filter the response to make it more meaningful, using any allowed filters.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="scoId">The sco identifier.</param>
        /// <param name="filterBy">optional 'filter by' params</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<XElement> Report_QuizInteractions(this AdobeConnectXmlAPI adobeConnectXmlApi, string scoId, string filterBy)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("report-quiz-interactions", String.Format("sco-id={0}&{1}", scoId, filterBy));

            var resultStatus = Helpers.WrapBaseStatusInfo<EnumerableResultStatus<XElement>>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return resultStatus;
            }

            resultStatus.Result = s.ResultDocument.XPathSelectElements("//row");

            return resultStatus;
        }

        /// <summary>
        /// Returns information about the number of users who chose a specific answer to a quiz
        /// question. The combination of one quiz question and all of one user’s answers to it is called an
        /// interaction. If the user answers the question more than once, all answers are part of the same
        /// interaction and have the same interaction-id
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="scoId">The sco identifier.</param>
        /// <param name="filterBy">optional 'filter by' params</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<XElement> Report_QuizQuestionAnswers(this AdobeConnectXmlAPI adobeConnectXmlApi, string scoId, string filterBy)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("report-quiz-question-answer-distribution", String.Format("sco-id={0}&{1}", scoId, filterBy));

            var resultStatus = Helpers.WrapBaseStatusInfo<EnumerableResultStatus<XElement>>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return resultStatus;
            }

            resultStatus.Result = s.ResultDocument.XPathSelectElements("//row");

            return resultStatus;
        }

        /// <summary>
        /// Returns information about the number of correct and incorrect answers to the questions on a
        /// quiz. This call can help you determine how a group responded to a quiz question overall.
        /// Because this call returns information about all the questions on a quiz, you may want to filter
        /// the results for a specific question or group of questions.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="scoId">The sco identifier.</param>
        /// <param name="filterBy">optional 'filter by' params</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<XElement> Report_QuizQuestionDistribution(this AdobeConnectXmlAPI adobeConnectXmlApi, string scoId, string filterBy)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("report-quiz-question-distribution", String.Format("sco-id={0}&{1}", scoId, filterBy));

            var resultStatus = Helpers.WrapBaseStatusInfo<EnumerableResultStatus<XElement>>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return resultStatus;
            }

            resultStatus.Result = s.ResultDocument.XPathSelectElements("//row");

            return resultStatus;
        }

        /// <summary>
        /// Provides a list of answers that users have given to questions on a quiz.
        /// Without filtering, this action returns all answers from any user to any question on the quiz.
        /// However, you can filter the response for a specific user, interaction, or answer (see the filter
        /// syntax at filter-definition).
        /// An interaction is a combination of one user and one question. If the user answers the same
        /// question more than once, all answers are part of the same interaction and have the same interaction-id.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="scoId">The sco identifier.</param>
        /// <param name="filterBy">optional 'filter by' params</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<XElement> Report_QuizQuestionResponse(this AdobeConnectXmlAPI adobeConnectXmlApi, string scoId, string filterBy)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("report-quiz-question-response", String.Format("sco-id={0}&{1}", scoId, filterBy));

            var resultStatus = Helpers.WrapBaseStatusInfo<EnumerableResultStatus<XElement>>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return resultStatus;
            }

            resultStatus.Result = s.ResultDocument.XPathSelectElements("//row");

            return resultStatus;
        }

        /// <summary>
        /// Provides a summary of data about a quiz, including the number of times the quiz has been
        /// taken, average, high, and low scores, and other information.
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="scoId">The sco identifier.</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<XElement> Report_QuizSummary(this AdobeConnectXmlAPI adobeConnectXmlApi, string scoId)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("report-quiz-summary", String.Format("sco-id={0}", scoId));

            var resultStatus = Helpers.WrapBaseStatusInfo<EnumerableResultStatus<XElement>>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return resultStatus;
            }

            resultStatus.Result = s.ResultDocument.XPathSelectElements("//row");

            return resultStatus;
        }

        /// <summary>
        /// Provides information about all users who have taken a quiz in a training. Use a sco-id to
        /// identify the quiz.
        /// To reduce the volume of the response, use any allowed filter or pass a Type parameter to
        /// return information about just one Type of SCO (courses, presentations, or meetings).
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="scoId">The sco identifier.</param>
        /// <param name="filterBy">optional 'filter by' params</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<XElement> Report_QuizTakers(this AdobeConnectXmlAPI adobeConnectXmlApi, string scoId, string filterBy)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("report-quiz-takers", String.Format("sco-id={0}&{1}", scoId, filterBy));

            var resultStatus = Helpers.WrapBaseStatusInfo<EnumerableResultStatus<XElement>>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return resultStatus;
            }

            resultStatus.Result = s.ResultDocument.XPathSelectElements("//row");

            return resultStatus;
        }

        /// <summary>
        /// Returns a list of users who attended an Acrobat Connect Meeting. The data is returned in row
        /// elements, one for each person who attended. If the Meeting hasn’t started or had no attendees,
        /// the response contains no rows.The response does not include Meeting hosts or users who were
        /// invited but did not attend
        /// </summary>
        /// <param name="adobeConnectXmlApi">The adobe connect XML API.</param>
        /// <param name="scoId">Meeting ID</param>
        /// <param name="filterBy">optional 'filter by' params</param>
        /// <returns>
        ///   <see cref="EnumerableResultStatus{T}" />
        /// </returns>
        public static EnumerableResultStatus<XElement> Report_MeetingAttendance(this AdobeConnectXmlAPI adobeConnectXmlApi, string scoId, string filterBy)
        {
            ApiStatus s = adobeConnectXmlApi.ProcessApiRequest("report-meeting-attendance", String.Format("sco-id={0}&{1}", scoId, filterBy));

            var resultStatus = Helpers.WrapBaseStatusInfo<EnumerableResultStatus<XElement>>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return resultStatus;
            }

            resultStatus.Result = s.ResultDocument.XPathSelectElements("//row");

            return resultStatus;
        }
    }
}

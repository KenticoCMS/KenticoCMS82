using System;
using System.Linq;
using System.Text;

namespace CMS.Controls
{
    /// <summary>
    /// Message boards transformation methods
    /// </summary>
    public partial class CMSTransformation
    {
        /// <summary>
        /// Returns the count of messages in given message board.
        /// </summary>
        /// <param name="boardWebpartName">Name of the messageboard webpart.</param> 
        /// <param name="type">Type of messageboard: 'document', 'user' or 'group'.</param>
        public int GetBoardMessagesCount(string boardWebpartName, string type)
        {
            // Get messages count
            return MessageBoardFunctions.GetBoardMessagesCount(EvalInteger("DocumentID"), boardWebpartName, type);
        }


        /// <summary>
        /// Returns the count of messages in given document related message board.
        /// </summary>
        /// <param name="boardWebpartName">Name of the messageboard webpart.</param> 
        public int GetBoardMessagesCount(string boardWebpartName)
        {
            // Get messages count
            return GetBoardMessagesCount(boardWebpartName, "document");
        }
    }
}
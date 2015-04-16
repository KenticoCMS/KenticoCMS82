using System;
using System.Data;
using System.Text;
using System.Xml;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.Synchronization;
using CMS.UIControls;

public partial class CMSModules_Staging_Tools_Controls_ViewTask : CMSAdminEditControl
{
    #region "Variables"

    private int mTaskId = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the ID of the task.
    /// </summary>
    public int TaskId
    {
        get
        {
            return mTaskId;
        }
        set
        {
            mTaskId = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        StagingTaskInfo ti = StagingTaskInfoProvider.GetTaskInfo(TaskId);
        // Set edited object
        EditedObject = ti;

        if (ti != null)
        {
            ((CMSDeskPage)Page).PageTitle.TitleText += " (" + HTMLHelper.HTMLEncode(ti.TaskTitle) + ")";

            // Prepare task description
            StringBuilder sbTaskInfo = new StringBuilder();
            sbTaskInfo.Append("<table>");
            sbTaskInfo.Append("<tr><td class=\"Title Grid\" style=\"width:135px\">" + GetString("staging.tasktype") + "</td><td>" + ti.TaskType.ToString() + "</td></tr>");
            sbTaskInfo.Append("<tr><td class=\"Title Grid\">" + GetString("staging.tasktime") + "</td><td>" + ti.TaskTime.ToString() + "</td></tr>");
            sbTaskInfo.Append("<tr><td class=\"Title Grid\">" + GetString("staging.taskprocessedby") + "</td><td>" + DataHelper.GetNotEmpty(ti.TaskServers.Trim(';').Replace(";", ", "), "-") + "</td></tr>");
            sbTaskInfo.Append("</table>");

            string objectType = ti.TaskObjectType;
            if (ti.TaskNodeID > 0)
            {
                objectType = TreeNode.OBJECT_TYPE;
            }
            viewDataSet.ObjectType = objectType;
            viewDataSet.DataSet = GetDataSet(ti.TaskData, ti.TaskType, ti.TaskObjectType);
            viewDataSet.AdditionalContent = sbTaskInfo.ToString();
        }
    }


    /// <summary>
    /// Returns the dataset loaded from the given document data.
    /// </summary>
    /// <param name="documentData">Document data to make the dataset from</param>
    /// <param name="taskType">Task type</param>
    /// <param name="taskObjectType">Task object type</param>
    protected virtual DataSet GetDataSet(string documentData, TaskTypeEnum taskType, string taskObjectType)
    {
        var man = SyncManager.GetInstance();
        man.OperationType = OperationTypeEnum.Synchronization;
        string className = DocumentHierarchyHelper.GetNodeClassName(documentData, ExportFormatEnum.XML);
        DataSet ds = man.GetSynchronizationTaskDataSet(taskType, className, taskObjectType);

        XmlParserContext xmlContext = new XmlParserContext(null, null, null, XmlSpace.None);
        XmlReader reader = new XmlTextReader(documentData, XmlNodeType.Element, xmlContext);

        return DataHelper.ReadDataSetFromXml(ds, reader, null, null);
    }
}
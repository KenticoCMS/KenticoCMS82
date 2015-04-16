using System;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DataEngine;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.OnlineForms;
using CMS.UIControls;


public partial class CMSAdminControls_UI_Development_GeneratedCodePreview : CMSAdminControl
{
    private string mCode;
    private string mClassName;
    private const string DOWNLOAD_FILE = "DownloadFile";
    private BaseInfo mBaseInfo;


    private BaseInfo BaseInfo
    {
        get
        {
            return mBaseInfo ?? (mBaseInfo = UIContext.EditedObject as BaseInfo);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (BaseInfo == null)
        {
            ShowError(GetString("codegenerator.objectwasnotloaded"));
            txtCode.Visible = false;
            return;
        }

        DataClassInfo dci = GetDataClassInfo();
        if (dci != null)
        {
            mClassName = dci.ClassName;
            string formDefinition = dci.ClassFormDefinition;
            var fi = new FormInfo(formDefinition);
            mCode = GetCode(fi);
            txtCode.Text = mCode;
            HeaderActions.AddAction(new HeaderAction()
            {
                Text = GetString("codegenerator.downloadfile"),
                CommandName = DOWNLOAD_FILE,
                OnClientClick = "window.noProgress = true;"
            });

            HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        }
    }


    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (e.CommandName == DOWNLOAD_FILE)
        {
            Response.Clear();
            Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"", HTTPHelper.GetDispositionFilename(GetFileName()) + ".cs"));
            Response.ContentType = "text/plain";
            Response.Write(mCode);
            Response.End();
        }
    }


    private DataClassInfo GetDataClassInfo()
    {
        switch (BaseInfo.TypeInfo.ObjectType)
        {
            case BizFormInfo.OBJECT_TYPE:
                int classID = ValidationHelper.GetInteger(BaseInfo.GetValue("FormClassID"), 0);
                return DataClassInfoProvider.GetDataClassInfo(classID);

            case DataClassInfo.OBJECT_TYPE_DOCUMENTTYPE:
            case DataClassInfo.OBJECT_TYPE_CUSTOMTABLE:
                string className = ValidationHelper.GetString(BaseInfo.GetValue("ClassName"), String.Empty);
                return DataClassInfoProvider.GetDataClassInfo(className);
        }

        return null;
    }


    private string GetCode(FormInfo formInfo)
    {
        switch (BaseInfo.TypeInfo.ObjectType)
        {
            case BizFormInfo.OBJECT_TYPE:
                return FormInfoClassGenerator.GetOnlineForm(mClassName, formInfo);

            case DataClassInfo.OBJECT_TYPE_DOCUMENTTYPE:
                return FormInfoClassGenerator.GetDocumentType(mClassName, formInfo);

            case DataClassInfo.OBJECT_TYPE_CUSTOMTABLE:
                return FormInfoClassGenerator.GetCustomTable(mClassName, formInfo);
        }

        return null;
    }


    private string GetFileName()
    {
        string fileName = mClassName;

        int dotIndex = fileName.LastIndexOfCSafe('.');
        if (dotIndex >= 0)
        {
            fileName = fileName.Substring(dotIndex + 1);
        }

        fileName = ValidationHelper.GetIdentifier(fileName, String.Empty);
        fileName = fileName[0].ToString().ToUpperCSafe() + fileName.Substring(1);

        switch (BaseInfo.TypeInfo.ObjectType)
        {
            case BizFormInfo.OBJECT_TYPE:
            case DataClassInfo.OBJECT_TYPE_CUSTOMTABLE:
                fileName += "Item";
                break;
        }

        return fileName + ".generated";
    }
}

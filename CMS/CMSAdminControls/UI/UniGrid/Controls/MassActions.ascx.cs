using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Helpers;
using CMS.UIControls;


/// <summary>
/// Control for displaying and handling the UniGrid mass actions.
/// </summary>
public partial class CMSAdminControls_UI_UniGrid_Controls_MassActions : CMSUserControl, ICallbackEventHandler, IExtensibleMassActions
{
    private readonly List<MassActionItem> mMassActions = new List<MassActionItem>();


    /// <summary>
    /// Client ID of hidden input containing the selected items.
    /// </summary>
    /// <remarks>
    /// Items are separated by pipe ('|'). UniGrid automatically creates this hidden field. Its ClientID can be obtained by GetSelectionFieldClientID() method.
    /// </remarks>
    public string SelectedItemsClientID
    {
        get;
        set;
    }


    /// <summary>
    /// If mass action is called, Value of this Lazy object will be passed to the CreateUrl callback. If this property is null, nothing is passed to the callback.
    /// </summary>
    public Lazy<object> AdditionalParameters
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or set the resource string which will be used as 'Selected items' item in the drop down list. If not set, the default text (Selected items) will be displayed.
    /// </summary>
    public string SelectedItemsResourceString
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or set the resource string which will be used as 'All items' item in the drop down list. If not set, the default text (All items) will be displayed.
    /// </summary>
    public string AllItemsResourceString
    {
        get;
        set;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        if (string.IsNullOrEmpty(SelectedItemsClientID))
        {
            throw new InvalidOperationException("SelectedItemsClientID property has to be set to pass information about selected items");
        }

        FillDropdowns();
        
        var moduleParams = new
        {
            CallbackTargetUniqueID = UniqueID,
            ButtonClientID = btnOk.ClientID,
            ScopeDropDownClientID = drpScope.ClientID,
            ActionDropDownClientID = drpAction.ClientID,
            SelectedItemsClientID = SelectedItemsClientID,
            MessagePlaceHolderClientID = divMessages.ClientID,
            NoItemsSelectedMessage = GetString("documents.selectdocuments"),
            NoActionSelectedMessage = GetString("massaction.selectsomeaction")
        };

        if (!RequestHelper.IsCallback())
        {
            ScriptHelper.RegisterModule(this, "CMS/MassActions", moduleParams);
        }
    }


    /// <summary>
    /// Inserts items into dropdowns.
    /// </summary>
    private void FillDropdowns()
    {
        if (!RequestHelper.IsPostBack())
        {
            drpScope.Items.Add(new ListItem(GetString(SelectedItemsResourceString ?? "general.SelectedItems"), Convert.ToInt32(MassActionScopeEnum.SelectedItems).ToString()));
            drpScope.Items.Add(new ListItem(GetString(AllItemsResourceString ?? "general.AllItems"), Convert.ToInt32(MassActionScopeEnum.AllItems).ToString()));
        }

        drpAction.Items.Add(new ListItem(GetString("general.SelectAction"), "##noaction##"));

        foreach (var massAction in mMassActions)
        {
            drpAction.Items.Add(new ListItem(GetString(massAction.DisplayNameResourceString), massAction.CodeName));
        }
    }


    /// <summary>
    /// Adds new mass action to the underlying collection.
    /// </summary>
    /// <param name="massActionItem">Mass action to be added</param>
    /// <exception cref="ArgumentNullException"><paramref name="massActionItem"/> is null</exception>
    public void AddMassAction(MassActionItem massActionItem)
    {
        if (massActionItem == null)
        {
            throw new ArgumentNullException("massActionItem");
        }

        mMassActions.Add(massActionItem);
    }


    #region "Callbacks handling (ICallbackEventHandler members and helping fields and types)"

    /// <summary>
    /// Wrapper for arguments needed when performing callback.
    /// </summary>
    private class CallbackArguments
    {
        /// <summary>
        /// Code name of action, used as the unique identifier of mass action.
        /// </summary>
        public string ActionCodeName
        {
            get;
            set;
        }


        /// <summary>
        /// Determines whether action should be performed only on selected items or on all items which satisfies filter condition.
        /// </summary>
        public MassActionScopeEnum Scope
        {
            get;
            set;
        }


        /// <summary>
        /// Collection containing IDs of selected items.
        /// </summary>
        public List<int> SelectedItems
        {
            get;
            set;
        }
    }


    private CallbackArguments mCallbackArguments;


    /// <summary>
    /// Processes a callback event that targets a control.
    /// </summary>
    /// <param name="eventArgument">A string that represents an event argument to pass to the event handler.</param>
    /// <exception cref="ArgumentNullException"><paramref name="eventArgument"/> is null</exception>
    public void RaiseCallbackEvent(string eventArgument)
    {
        if (eventArgument == null)
        {
            throw new ArgumentNullException("eventArgument");
        }

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        mCallbackArguments = serializer.Deserialize<CallbackArguments>(eventArgument);

        if (mCallbackArguments == null)
        {
            throw new InvalidOperationException("[MassActions.RaiseCallbackEvent]: callback arguments cannot be deserialized");
        }

        if (mCallbackArguments.Scope == MassActionScopeEnum.AllItems)
        {
            mCallbackArguments.SelectedItems = null;
        }
    }


    /// <summary>
    /// Returns the results of a callback event that targets a control.
    /// </summary>
    /// <returns>The result of the callback.</returns>
    public string GetCallbackResult()
    {
        var selectedAction = mMassActions.SingleOrDefault(action => action.CodeName == mCallbackArguments.ActionCodeName);

        if (selectedAction == null)
        {
            throw new GeneralCMSException(string.Format("[MassActions.GetCallbackResult]: Specified mass action ({0}) was not found. Mass actions has to be added on every request (event postback).", mCallbackArguments.ActionCodeName));
        }

        var additionalParameters = AdditionalParameters == null ? null : AdditionalParameters.Value;

        var result = new
        {
            url = selectedAction.CreateUrl(mCallbackArguments.Scope, mCallbackArguments.SelectedItems, additionalParameters),
            type = selectedAction.ActionType, // serializes enum to its internal representation (0, 1)
        };

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        return serializer.Serialize(result);
    }

    #endregion
}
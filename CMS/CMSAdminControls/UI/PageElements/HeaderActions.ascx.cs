using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;

using CMS;
using CMS.Helpers;
using CMS.Base;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;
using CMS.FormControls;

using AbstractUserControl = CMS.ExtendedControls.AbstractUserControl;


public partial class CMSAdminControls_UI_PageElements_HeaderActions : HeaderActions, IPostBackEventHandler
{
    #region "Variables"

    private List<HeaderAction> mActionsList = new List<HeaderAction>();
    private string[,] mActions;
    private bool? mUseSmallIcons;
    private bool mEnabled = true;
    private HeaderAction shortcutAction;
    private bool mPerformFullPostBack = true;
    private List<AbstractUserControl> mAdditionalControls;
    private List<CMSButton> mProcessedBaseButtons;

    #endregion


    #region "Properties"

    /// <summary>
    /// Component name
    /// </summary>
    public override string ComponentName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["ComponentName"], base.ComponentName);
        }
        set
        {
            base.ComponentName = value;
            ViewState["ComponentName"] = value;
        }
    }


    /// <summary>
    /// Indicates if control is enabled
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
        }
    }


    /// <summary>
    /// Update panel
    /// </summary>
    public override CMSUpdatePanel UpdatePanel
    {
        get
        {
            return pnlUp;
        }
    }


    /// <summary>
    /// Indicates if the actions should perform full post-back
    /// </summary>
    public override bool PerformFullPostBack
    {
        get
        {
            return mPerformFullPostBack;
        }
        set
        {
            mPerformFullPostBack = value;
        }
    }


    /// <summary>
    /// Gets or sets the array of actions. The meaning of the indexes:
    /// (0,0): Type of the link (HyperLink or LinkButton), 
    /// (0,1): Text of the action link, 
    /// (0,2): JavaScript command to be performed OnClick action, 
    /// (0,3): NavigationUrl of the HyperLink (or PostBackUrl of LinkButton), 
    /// (0,4): Tooltip of the action link, 
    /// (0,5): Action image URL, 
    /// (0,6): CommandName of the LinkButton Command event, 
    /// (0,7): CommandArgument of the LinkButton Command event.
    /// (0,8): Register shortcut action (TRUE/FALSE)
    /// (0,9): Enabled state of the action (TRUE/FALSE)
    /// (0,10): Visibility of the action (TRUE/FALSE)
    /// (0,11): Hyperlink target (only if type is hyperlink).
    /// (0,12): Use ImageButton instead of Image (TRUE/FALSE).
    /// At least first two arguments must be defined.
    /// </summary>
    [Obsolete("Use List<HeaderAction> ActionsList instead.")]
    public override string[,] Actions
    {
        get
        {
            // Initialize actions from list for backward compatibility
            if ((mActions == null) && (ActionsList != null) && (ActionsList.Count > 0))
            {
                mActions = GetActionsFromList(ActionsList);
            }

            return mActions;
        }
        set
        {
            mActions = value;
        }
    }


    /// <summary>
    /// List of actions
    /// </summary>
    public override List<HeaderAction> ActionsList
    {
        get
        {
            return mActionsList;
        }
        set
        {
            mActionsList = value;
        }
    }


    /// <summary>
    /// Gets or sets CssClass of the panel where all the actions are placed.
    /// </summary>
    public override string PanelCssClass
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if small icons should be used for actions
    /// </summary>
    public override bool UseSmallIcons
    {
        get
        {
            if (mUseSmallIcons == null)
            {
                mUseSmallIcons = !UseBasicStyles && !IsLiveSite;
            }

            return mUseSmallIcons.Value;
        }
        set
        {
            mUseSmallIcons = value;
        }
    }


    /// <summary>
    /// Additional controls list
    /// </summary>
    public override List<AbstractUserControl> AdditionalControls
    {
        get
        {
            return mAdditionalControls ?? (mAdditionalControls = new List<AbstractUserControl>());
        }
    }


    /// <summary>
    /// List of processed base buttons
    /// </summary>
    protected List<CMSButton> ProcessedBaseButtons
    {
        get
        {
            return mProcessedBaseButtons ?? (mProcessedBaseButtons = new List<CMSButton>());
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page init 
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        ReloadAdditionalControls();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Get type safe configuration
        InitializeActionsList(mActions);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ReloadData();

        // Hide processed base buttons
        ProcessedBaseButtons.ForEach(b => b.Visible = false);

        // Add shadow below header actions
        ScriptHelper.RegisterModule(Page, "CMS/HeaderShadow");

        // Hide menu if not visible
        plcMenu.Visible = HasAnyVisibleAction();
    }


    protected override void Render(HtmlTextWriter writer)
    {
        if ((Context != null) && RenderContainer)
        {
            string cssClass = (UseBasicStyles || IsLiveSite) ? "PageHeaderLine" : "cms-edit-menu";
            writer.Write("<div class\"{0}\">", cssClass);
        }

        base.Render(writer);

        if ((Context != null) && RenderContainer)
        {
            writer.Write("</div>");
        }

        if (shortcutAction != null)
        {
            PostBackOptions opt = new PostBackOptions(this, shortcutAction.CommandArgument)
            {
                PerformValidation = true,
                ValidationGroup = shortcutAction.ValidationGroup
            };

            Page.ClientScript.RegisterForEventValidation(opt);
        }
    }


    /// <summary>
    /// Initializes actions list
    /// </summary>
    /// <param name="actions">Actions</param>
    private void InitializeActionsList(string[,] actions)
    {
        // Convert array of actions
        if ((actions != null) && (ActionsList.Count == 0))
        {
            ActionsList = new List<HeaderAction>();

            // Get the number of actions
            int actionCount = actions.GetUpperBound(0) + 1;

            // Get the array size (number of arguments)
            int arraySize = actions.GetUpperBound(1) + 1;

            // Exit if nothing about the action is specified
            if (arraySize < 2)
            {
                return;
            }

            // Generate the actions
            for (int i = 0; i < actionCount; i++)
            {
                string type = (actions[i, 0] != null) ? actions[i, 0].ToLowerCSafe() : null;
                HeaderAction action = (type == TYPE_SAVEBUTTON) ? new SaveAction(Page) : new HeaderAction { IsLiveSite = IsLiveSite };

                // Get the action parameters
                action.Text = (actions[i, 1]);
                action.Visible = (!(arraySize > 10) || ValidationHelper.GetBoolean(actions[i, 10], true));
                action.OnClientClick = ((arraySize > 2) ? actions[i, 2] : null);
                action.RedirectUrl = ((arraySize > 3) ? actions[i, 3] : null);
                action.Tooltip = ((arraySize > 4) ? actions[i, 4] : null);
                action.CommandName = ((arraySize > 6) ? actions[i, 6] : (arraySize > 13) ? actions[i, 13] : null);
                action.CommandArgument = ((arraySize > 7) ? actions[i, 7] : null);
                action.RegisterShortcutScript = ((arraySize > 8) && ValidationHelper.GetBoolean(actions[i, 8], false));
                action.Enabled = (!(arraySize > 9) || ValidationHelper.GetBoolean(actions[i, 9], true));
                action.Target = ((arraySize > 11) ? ValidationHelper.GetString(actions[i, 11], string.Empty) : string.Empty);

                ActionsList.Add(action);
            }
        }
    }


    /// <summary>
    /// Initializes additional controls list
    /// </summary>
    public override void ReloadAdditionalControls()
    {
        plcAdditionalControls.Controls.Clear();
        foreach (AbstractUserControl ctrl in AdditionalControls)
        {
            plcAdditionalControls.Controls.Add(ctrl);
        }
    }


    /// <summary>
    /// Gets actions from actions list
    /// </summary>
    /// <param name="actionsList">Actions list</param>
    private string[,] GetActionsFromList(List<HeaderAction> actionsList)
    {
        int actionCount = actionsList.Count;
        string[,] actions = new string[actionCount, 14];

        // Generate the actions
        for (int i = 0; i < actionCount; i++)
        {
            actions[i, 1] = actionsList[i].Text;
            actions[i, 2] = actionsList[i].OnClientClick;
            actions[i, 3] = actionsList[i].RedirectUrl;
            actions[i, 4] = actionsList[i].Tooltip;
            actions[i, 5] = "";
            actions[i, 6] = actionsList[i].CommandName;
            actions[i, 7] = actionsList[i].CommandArgument;
            actions[i, 8] = actionsList[i].RegisterShortcutScript.ToString();
            actions[i, 9] = actionsList[i].Enabled.ToString();
            actions[i, 10] = actionsList[i].Visible.ToString();
            actions[i, 11] = actionsList[i].Target;
            actions[i, 12] = false.ToString();
            actions[i, 13] = actionsList[i].CommandName;
            actions[i, 0] = TYPE_HYPERLINK;
        }

        return actions;
    }


    /// <summary>
    /// Indicates if the menu has any action to display
    /// </summary>
    public bool HasAnyVisibleAction()
    {
        return (ActionsList.Any(action => action.IsVisible()));
    }


    /// <summary>
    /// Indicates if the menu has content to display
    /// </summary>
    public override bool IsVisible()
    {
        return HasAnyVisibleAction() && Visible;
    }


    /// <summary>
    /// Reloads the actions.
    /// </summary>
    public override void ReloadData()
    {
        // Get type safe configuration
        InitializeActionsList(mActions);

        int actionsCount = ActionsList.Count;
        if (actionsCount > 0)
        {
            pnlActions.Controls.Clear();

            if (!String.IsNullOrEmpty(PanelCssClass))
            {
                pnlActions.CssClass = PanelCssClass;
            }

            CreateActions(ActionsList, pnlActions);
        }

        pnlAdditionalControls.CssClass += " " + AdditionalControlsCssClass;
    }


    /// <summary>
    /// Adds action.
    /// </summary>
    /// <param name="action">Action</param>
    public override void AddAction(HeaderAction action)
    {
        if (action == null)
        {
            return;
        }

        // Make sure the Save action is set only once
        string key = string.Format("HeaderActionsSaveSet_{0}_{1}", action.CommandArgument, ClientID);
        bool saveSet = ValidationHelper.GetBoolean(RequestStockHelper.GetItem(key), false);
        if (!(action is SaveAction) || !saveSet)
        {
            bool added = false;

            // Ensure correct index
            if (action.Index == -1)
            {
                action.Index = ActionsList.Count;
            }
            else
            {
                // Post processing of action attribute
                for (int i = 0; i < ActionsList.Count; i++)
                {
                    if (ActionsList[i].Index == action.Index)
                    {
                        // Replace action with the same index
                        ActionsList[i] = action;

                        // Button added
                        added = true;
                        break;
                    }
                }
            }

            // If action with the same index was not found, add it to the list
            if (!added)
            {
                ActionsList.Add(action);
            }

            // Keep flag
            if (action is SaveAction)
            {
                RequestStockHelper.Add(key, (action.BaseButton == null) || action.BaseButton.Visible);
            }
        }

        // Store base buttons
        if ((action.BaseButton != null) && !ProcessedBaseButtons.Contains(action.BaseButton))
        {
            ProcessedBaseButtons.Add(action.BaseButton);
        }
    }


    /// <summary>
    /// Inserts action at specified index.
    /// </summary>
    /// <param name="index">Index</param>
    /// <param name="action">Action</param>
    public override void InsertAction(int index, HeaderAction action)
    {
        if (action == null)
        {
            return;
        }

        action.Index = index;
        AddAction(action);
    }


    private void CreateActions(List<HeaderAction> actions, Control parent)
    {
        int actionsCount = actions.Count;
        // Sort actions by index to be sure the order is ensured for multiple actions
        if (actionsCount > 1)
        {
            // At least one action has index
            if (actions.Exists(a => (a.Index != -1)))
            {
                // Sort the actions
                actions.Sort((a1, a2) => a1.Index.CompareTo(a2.Index));
            }
        }

        // Generate the actions
        for (int i = 0; i < actionsCount; ++i)
        {
            var action = actions[i];

            // If the text is not specified or visibility is false, skip the action
            var formButton = action.BaseButton as FormSubmitButton;
            if (!action.IsVisible() || ((action.BaseButton != null) && (!action.BaseButton.Visible || ((formButton != null) && !formButton.RegisterHeaderAction))))
            {
                // Skip empty action
                action.Visible = false;
                continue;
            }

            // Check permission if action is enabled
            if (action.Enabled)
            {
                action.Enabled = CheckPermissions(action.ResourceName, action.Permission);
            }

            // Set live site flag for resource strings
            action.IsLiveSite = IsLiveSite;

            // Get the action parameters
            string ctrlId = String.Concat(ID, "_HA_", i);
            Control actionControl = null;

            HeaderActionControlCreatedEventArgs args = new HeaderActionControlCreatedEventArgs()
            {
                Action = action,
            };

            // Start the ActionControlCreated event
            using (var actionCreated = ActionControlCreated.StartEvent(args))
            {
                if (actionCreated.CanContinue())
                {
                    // Ensure correct HeaderAction instance is used
                    action = args.Action;

                    // Use multi button when action contains alternative actions
                    if ((action.AlternativeActions != null) && action.AlternativeActions.Any())
                    {
                        // Get main action
                        var controlActions = new List<CMSButtonAction>
                        {
                            GetControlAction(action)
                        };

                        if (action.RegisterShortcutScript)
                        {
                            RegisterSaveShortcutScript(action, ctrlId);
                        }

                        // Get other actions
                        for (int j = 0; j < action.AlternativeActions.Count; j++)
                        {
                            var alternativeAction = action.AlternativeActions[j];

                            controlActions.Add(GetControlAction(alternativeAction));
                            if (action.RegisterShortcutScript)
                            {
                                RegisterSaveShortcutScript(action, ctrlId + "_" + j);
                            }
                        }

                        var button = action.Inactive ? (CMSMultiButtonBase)new CMSToggleButton() : new CMSMoreOptionsButton();

                        button.Enabled = Enabled;
                        button.ID = ctrlId;
                        button.Actions = controlActions;

                        actionControl = button;
                    }
                    // Use classic button
                    else
                    {
                        var controlAction = GetControlAction(action);
                        var button = new CMSButton
                        {
                            ButtonStyle = action.ButtonStyle,
                            ID = ctrlId,
                            Enabled = controlAction.Enabled,
                            Text = controlAction.Text,
                            OnClientClick = controlAction.OnClientClick,
                            ToolTip = action.Tooltip,
                            UseSubmitBehavior = false
                        };

                        if (action.RegisterShortcutScript)
                        {
                            RegisterSaveShortcutScript(action, ctrlId);
                        }

                        actionControl = button;
                    }

                    if ((action.CssClass != null) && (actionControl is WebControl))
                    {
                        ((WebControl)actionControl).AddCssClass(action.CssClass);
                    }

                    args.ActionControl = actionControl;
                }

                // Finish the AcrtionControlCreated event
                actionCreated.FinishEvent();
            }

            actionControl = args.ActionControl;

            // Add control to the panel
            if (actionControl != null)
            {
                parent.Controls.Add(actionControl);
            }
        }
    }


    private CMSButtonAction GetControlAction(HeaderAction headerAction)
    {
        var controlAction = new CMSButtonAction();

        controlAction.Text = headerAction.Text;
        controlAction.Enabled = headerAction.Enabled && Enabled;
        controlAction.ToolTip = headerAction.Tooltip;

        // Register script only when action is active
        if (Enabled && headerAction.Enabled && !headerAction.Inactive)
        {
            // Wrap script from OnClick property into anonymous function so it won't cancel the following script in case this property script returns true. 
            // The execution of following script is canceled only when anonymous function returns false.
            if (!String.IsNullOrEmpty(headerAction.OnClientClick))
            {
                string onClickScript = "var onClickWrapper = function(sender) { " + headerAction.OnClientClick + "}; if (onClickWrapper(this) === false) { return false; }";
                controlAction.OnClientClick = onClickScript;
            }

            string commandName = !string.IsNullOrEmpty(headerAction.CommandName) ? headerAction.CommandName : headerAction.EventName;

            // Perform post-back
            if (!String.IsNullOrEmpty(commandName) || !String.IsNullOrEmpty(headerAction.CommandArgument))
            {
                string argument = string.Join(";", new[]
                {
                    commandName,
                    headerAction.CommandArgument
                });

                var opt = new PostBackOptions(this, argument)
                {
                    PerformValidation = true,
                    ValidationGroup = headerAction.ValidationGroup
                };

                string postbackScript = ControlsHelper.GetPostBackEventReference(this, opt, false, !PerformFullPostBack);
                controlAction.OnClientClick += postbackScript + ";";
            }
            else
            {
                // Use URL only for standard link
                if (!String.IsNullOrEmpty(headerAction.RedirectUrl))
                {
                    var target = headerAction.Target ?? "_self";

                    var url = ScriptHelper.ResolveUrl(headerAction.RedirectUrl);

                    if (headerAction.OpenInDialog)
                    {
                        url = URLHelper.AddParameterToUrl(url, "dialog", "1");
                        url = UIContextHelper.AppendDialogHash(url);

                        ScriptHelper.RegisterDialogScript(Page);

                        controlAction.OnClientClick = ScriptHelper.GetModalDialogScript(url, "action" + headerAction.Index, headerAction.DialogWidth, headerAction.DialogHeight, false);
                    }
                    else
                    {
                        controlAction.OnClientClick += "window.open('" + url + "','" + target + "');";
                    }
                }
            }

            // Stop automatic postback rendered by asp button 
            controlAction.OnClientClick += " return false;";
        }

        return controlAction;
    }


    /// <summary>
    /// Register the CRTL+S shortcut.
    /// </summary>
    /// <param name="action">Save header action</param>
    /// <param name="scriptID">Id of </param>
    private void RegisterSaveShortcutScript(HeaderAction action, string scriptID)
    {
        // Register script only when action is active
        if (Enabled && action.Enabled && !action.Inactive)
        {
            string commandName = !string.IsNullOrEmpty(action.CommandName) ? action.CommandName : action.EventName;

            string script = null;

            // Perform post-back
            if (!String.IsNullOrEmpty(commandName) || !String.IsNullOrEmpty(action.CommandArgument))
            {
                // Register encapsulation function for OnClientClick in shortcut. 
                if (!string.IsNullOrEmpty(action.OnClientClick))
                {
                    script = "if (PerfAction_" + scriptID + "() === false) { return false; }";

                    string scriptFunction = "function PerfAction_" + scriptID + "() { " + action.OnClientClick + "}";

                    ScriptHelper.RegisterStartupScript(Page, typeof(string), "PerfAction_" + scriptID, scriptFunction, true);
                }

                // Store action information for shortcut event validation registration
                shortcutAction = new HeaderAction
                {
                    IsLiveSite = IsLiveSite,
                    CommandArgument = action.CommandArgument,
                    ValidationGroup = action.ValidationGroup
                };


                string argument = string.Join(";", new[]
                {
                    commandName,
                    action.CommandArgument
                });

                var opt = new PostBackOptions(this, argument)
                {
                    PerformValidation = true,
                    ValidationGroup = action.ValidationGroup
                };

                string postbackScript = ControlsHelper.GetPostBackEventReference(this, opt, false, !PerformFullPostBack);

                // Prepare action script
                script = String.Concat(script, " ", postbackScript, ";");
            }
            else
            {
                script = action.OnClientClick;
            }

            ScriptHelper.RegisterSaveShortcut(Page, script);
        }
    }


    /// <summary>
    /// Returns true if user has permission for given resource.
    /// Returns true for empty resource name or permission.
    /// </summary>
    /// <param name="resourceName">Resource name</param>
    /// <param name="permission">Permission name to check</param>
    private bool CheckPermissions(string resourceName, string permission)
    {
        // Check only if both filled
        if (!String.IsNullOrEmpty(resourceName) && !String.IsNullOrEmpty(permission))
        {
            return CurrentUser.IsAuthorizedPerResource(resourceName, permission, SiteContext.CurrentSiteName);
        }

        return true;
    }


    /// <summary>
    /// Clears content rendered by header actions control.
    /// </summary>
    public void Clear()
    {
        pnlActions.Controls.Clear();
    }

    #endregion


    #region "IPostBackEventHandler Members"

    /// <summary>
    /// Raise post-back event
    /// </summary>
    /// <param name="eventArgument">Argument</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        if (!string.IsNullOrEmpty(eventArgument))
        {
            string[] argValues = eventArgument.Split(';');
            if (argValues.Length == 2)
            {
                CommandEventArgs args = new CommandEventArgs(argValues[0], argValues[1]);
                RaiseActionPerformed(this, args);

                // Update parent update panel in conditional mode
                var up = ControlsHelper.GetUpdatePanel(this);
                if ((up != null) && (up.UpdateMode == UpdatePanelUpdateMode.Conditional))
                {
                    up.Update();
                }
            }
        }
    }

    #endregion
}
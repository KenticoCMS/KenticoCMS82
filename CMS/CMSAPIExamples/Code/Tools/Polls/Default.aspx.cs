using System;
using System.Data;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Polls;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;

[CheckLicence(FeatureEnum.Polls)]
public partial class CMSAPIExamples_Code_Tools_Polls_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Poll
        apiCreatePoll.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreatePoll);
        apiGetAndUpdatePoll.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdatePoll);
        apiGetAndBulkUpdatePolls.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdatePolls);
        apiDeletePoll.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeletePoll);

        // Answer
        apiCreateAnswer.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateAnswer);
        apiGetAndUpdateAnswer.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateAnswer);
        apiGetAndBulkUpdateAnswers.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateAnswers);
        apiDeleteAnswer.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteAnswer);

        // Poll on site
        apiAddPollToSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(AddPollToSite);
        apiRemovePollFromSite.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(RemovePollFromSite);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Poll
        apiCreatePoll.Run();
        apiGetAndUpdatePoll.Run();
        apiGetAndBulkUpdatePolls.Run();

        // Answer
        apiCreateAnswer.Run();
        apiGetAndUpdateAnswer.Run();
        apiGetAndBulkUpdateAnswers.Run();

        // Poll on site
        apiAddPollToSite.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Poll on site
        apiRemovePollFromSite.Run();

        // Answer
        apiDeleteAnswer.Run();

        // Poll
        apiDeletePoll.Run();
    }

    #endregion


    #region "API examples - Poll"

    /// <summary>
    /// Creates poll. Called when the "Create poll" button is pressed.
    /// </summary>
    private bool CreatePoll()
    {
        // Create new poll object
        PollInfo newPoll = new PollInfo();

        // Set the properties
        newPoll.PollDisplayName = "My new poll";
        newPoll.PollCodeName = "MyNewPoll";
        newPoll.PollTitle = "My title";
        newPoll.PollQuestion = "My question";
        newPoll.PollResponseMessage = "My response message.";
        newPoll.PollAllowMultipleAnswers = false;
        newPoll.PollAccess = 0;

        // Save the poll
        PollInfoProvider.SetPollInfo(newPoll);

        return true;
    }


    /// <summary>
    /// Gets and updates poll. Called when the "Get and update poll" button is pressed.
    /// Expects the CreatePoll method to be run first.
    /// </summary>
    private bool GetAndUpdatePoll()
    {
        // Get the poll
        PollInfo updatePoll = PollInfoProvider.GetPollInfo("MyNewPoll", SiteContext.CurrentSiteID);

        if (updatePoll != null)
        {
            // Update the properties
            updatePoll.PollDisplayName = updatePoll.PollDisplayName.ToLower();

            // Save the changes
            PollInfoProvider.SetPollInfo(updatePoll);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates polls. Called when the "Get and bulk update polls" button is pressed.
    /// Expects the CreatePoll method to be run first.
    /// </summary>
    private bool GetAndBulkUpdatePolls()
    {
        // Prepare the parameters
        string where = "PollCodeName LIKE N'MyNewPoll%'";

        // Get the data
        DataSet polls = PollInfoProvider.GetPolls(where, null);

        if (!DataHelper.DataSourceIsEmpty(polls))
        {
            // Loop through the individual items
            foreach (DataRow pollDr in polls.Tables[0].Rows)
            {
                // Create object from DataRow
                PollInfo modifyPoll = new PollInfo(pollDr);

                // Update the properties
                modifyPoll.PollDisplayName = modifyPoll.PollDisplayName.ToUpper();

                // Save the changes
                PollInfoProvider.SetPollInfo(modifyPoll);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes poll. Called when the "Delete poll" button is pressed.
    /// Expects the CreatePoll method to be run first.
    /// </summary>
    private bool DeletePoll()
    {
        // Get the poll
        PollInfo deletePoll = PollInfoProvider.GetPollInfo("MyNewPoll", SiteContext.CurrentSiteID);

        // Delete the poll
        PollInfoProvider.DeletePollInfo(deletePoll);

        return (deletePoll != null);
    }

    #endregion


    #region "API examples - Answer"

    /// <summary>
    /// Creates answer. Called when the "Create answer" button is pressed.
    /// </summary>
    private bool CreateAnswer()
    {
        // Get the poll
        PollInfo poll = PollInfoProvider.GetPollInfo("MyNewPoll", SiteContext.CurrentSiteID);

        if (poll != null)
        {
            // Create new answer object
            PollAnswerInfo newAnswer = new PollAnswerInfo();

            // Set the properties
            newAnswer.AnswerPollID = poll.PollID;
            newAnswer.AnswerText = "My new answer";
            newAnswer.AnswerEnabled = true;
            newAnswer.AnswerCount = 0;

            // Save the answer
            PollAnswerInfoProvider.SetPollAnswerInfo(newAnswer);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and updates answer. Called when the "Get and update answer" button is pressed.
    /// Expects the CreateAnswer method to be run first.
    /// </summary>
    private bool GetAndUpdateAnswer()
    {
        // Get the answer
        PollInfo updatePoll = PollInfoProvider.GetPollInfo("MyNewPoll", SiteContext.CurrentSiteID);

        if (updatePoll != null)
        {
            DataSet answers = PollAnswerInfoProvider.GetAnswers(updatePoll.PollID, 1, null);

            if (!DataHelper.DataSourceIsEmpty(answers))
            {
                PollAnswerInfo updateAnswer = new PollAnswerInfo(answers.Tables[0].Rows[0]);

                // Update the properties
                updateAnswer.AnswerText = updateAnswer.AnswerText.ToLower();

                // Save the changes
                PollAnswerInfoProvider.SetPollAnswerInfo(updateAnswer);

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates answers. Called when the "Get and bulk update answers" button is pressed.
    /// Expects the CreateAnswer method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateAnswers()
    {
        PollInfo updatePoll = PollInfoProvider.GetPollInfo("MyNewPoll", SiteContext.CurrentSiteID);

        if (updatePoll != null)
        {
            // Get the data
            DataSet answers = PollAnswerInfoProvider.GetAnswers(updatePoll.PollID);
            if (!DataHelper.DataSourceIsEmpty(answers))
            {
                // Loop through the individual items
                foreach (DataRow answerDr in answers.Tables[0].Rows)
                {
                    // Create object from DataRow
                    PollAnswerInfo modifyAnswer = new PollAnswerInfo(answerDr);

                    // Update the properties
                    modifyAnswer.AnswerText = modifyAnswer.AnswerText.ToUpper();

                    // Save the changes
                    PollAnswerInfoProvider.SetPollAnswerInfo(modifyAnswer);
                }

                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Deletes answer. Called when the "Delete answer" button is pressed.
    /// Expects the CreateAnswer method to be run first.
    /// </summary>
    private bool DeleteAnswer()
    {
        // Get the poll
        PollInfo updatePoll = PollInfoProvider.GetPollInfo("MyNewPoll", SiteContext.CurrentSiteID);

        if (updatePoll != null)
        {
            // Get the answer
            DataSet answers = PollAnswerInfoProvider.GetAnswers(updatePoll.PollID, 1, null);

            if (!DataHelper.DataSourceIsEmpty(answers))
            {
                PollAnswerInfo deleteAnswer = new PollAnswerInfo(answers.Tables[0].Rows[0]);

                // Delete the answer
                PollAnswerInfoProvider.DeletePollAnswerInfo(deleteAnswer);

                return (deleteAnswer != null);
            }
        }

        return false;
    }

    #endregion


    #region "API examples - Poll on site"

    /// <summary>
    /// Adds poll to site. Called when the "Add poll to site" button is pressed.
    /// Expects the CreatePoll method to be run first.
    /// </summary>
    private bool AddPollToSite()
    {
        int siteId = SiteContext.CurrentSiteID;

        // Get the poll
        PollInfo poll = PollInfoProvider.GetPollInfo("MyNewPoll", siteId);

        if (poll != null)
        {
            int pollId = poll.PollID;

            // Save the binding
            PollSiteInfoProvider.AddPollToSite(pollId, siteId);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes poll from site. Called when the "Remove poll from site" button is pressed.
    /// Expects the AddPollToSite method to be run first.
    /// </summary>
    private bool RemovePollFromSite()
    {
        int siteId = SiteContext.CurrentSiteID;

        // Get the poll
        PollInfo removePoll = PollInfoProvider.GetPollInfo("MyNewPoll", siteId);

        if (removePoll != null)
        {
            // Remove poll from site
            PollSiteInfoProvider.RemovePollFromSite(removePoll.PollID, siteId);

            return true;
        }

        return false;
    }

    #endregion
}
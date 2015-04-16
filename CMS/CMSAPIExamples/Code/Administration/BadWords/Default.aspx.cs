using System;
using System.Collections;
using System.Data;

using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.Protection;

public partial class CMSAPIExamples_Code_Administration_BadWords_Default : CMSAPIExamplePage
{
    #region "Initialization"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Bad word
        apiCreateBadWord.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CreateBadWord);
        apiGetAndUpdateBadWord.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndUpdateBadWord);
        apiGetAndBulkUpdateBadWords.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(GetAndBulkUpdateBadWords);
        apiDeleteBadWord.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(DeleteBadWord);

        // Performing bad word checks
        apiCheckSingleBadWord.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CheckSingleBadWord);
        apiCheckAllBadWords.RunExample += new CMSAPIExamples_Controls_APIExample.OnRunExample(CheckAllBadWords);
    }

    #endregion


    #region "Mass actions"

    /// <summary>
    /// Runs all creating and managing examples.
    /// </summary>
    public override void RunAll()
    {
        base.RunAll();

        // Bad word
        apiCreateBadWord.Run();
        apiGetAndUpdateBadWord.Run();
        apiGetAndBulkUpdateBadWords.Run();

        // Performing bad word checks
        apiCheckSingleBadWord.Run();
        apiCheckAllBadWords.Run();
    }


    /// <summary>
    /// Runs all cleanup examples.
    /// </summary>
    public override void CleanUpAll()
    {
        base.CleanUpAll();

        // Bad word
        apiDeleteBadWord.Run();
    }

    #endregion


    #region "API examples - Bad word"

    /// <summary>
    /// Creates a "testbadword" bad word. Called when the "Create word" button is pressed.
    /// </summary>
    private bool CreateBadWord()
    {
        // Create new bad word object
        BadWordInfo newWord = new BadWordInfo();

        // Set the properties
        newWord.WordExpression = "testbadword";
        newWord.WordAction = BadWordActionEnum.ReportAbuse;
        newWord.WordIsGlobal = true;
        newWord.WordIsRegularExpression = false;

        // Save the bad word
        BadWordInfoProvider.SetBadWordInfo(newWord);

        return true;
    }


    /// <summary>
    /// Gets and updates the first single bad word matching the condition. Called when the "Get and update bad word" button is pressed.
    /// Expects the CreateBadWord method to be run first.
    /// </summary>
    private bool GetAndUpdateBadWord()
    {
        // Prepare the parameters
        string where = "[WordExpression] = 'testbadword'";

        // Get the data
        DataSet words = BadWordInfoProvider.GetBadWords(where, null);

        if (!DataHelper.DataSourceIsEmpty(words))
        {
            // Get the bad word's data row
            DataRow wordDr = words.Tables[0].Rows[0];

            // Create object from DataRow
            BadWordInfo modifyWord = new BadWordInfo(wordDr);

            // Update the properties
            modifyWord.WordAction = BadWordActionEnum.Replace;
            modifyWord.WordReplacement = "testpoliteword";

            // Save the changes
            BadWordInfoProvider.SetBadWordInfo(modifyWord);

            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets and bulk updates bad words created by the CreateBadWord method. Called when the "Get and bulk update bad words" button is pressed.
    /// Expects the CreateBadWord method to be run first.
    /// </summary>
    private bool GetAndBulkUpdateBadWords()
    {
        // Prepare the parameters
        string where = "[WordExpression] = 'testbadword'";

        // Get the data
        DataSet words = BadWordInfoProvider.GetBadWords(where, null);
        if (!DataHelper.DataSourceIsEmpty(words))
        {
            // Loop through the individual items
            foreach (DataRow wordDr in words.Tables[0].Rows)
            {
                // Create object from DataRow
                BadWordInfo modifyWord = new BadWordInfo(wordDr);

                // Update the properties
                modifyWord.WordAction = BadWordActionEnum.Replace;
                modifyWord.WordReplacement = "testpoliteword";

                // Save the changes
                BadWordInfoProvider.SetBadWordInfo(modifyWord);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Deletes all "testbadword" bad words. Called when the "Delete bad word(s)" button is pressed.
    /// Expects the CreateBadWord method to be run first.
    /// </summary>
    private bool DeleteBadWord()
    {
        // Prepare the parameters
        string where = "[WordExpression] = 'testbadword' ";

        // Get the data
        DataSet words = BadWordInfoProvider.GetBadWords(where, null);

        if (!DataHelper.DataSourceIsEmpty(words))
        {
            foreach (DataRow wordDr in words.Tables[0].Rows)
            {
                // Create object from DataRow
                BadWordInfo deleteWord = new BadWordInfo(wordDr);

                // Delete the bad word
                BadWordInfoProvider.DeleteBadWordInfo(deleteWord);
            }

            return true;
        }

        return false;
    }

    #endregion


    #region "API Examples - Performing bad word checks"

    /// <summary>
    /// Checks the declared custom string for presence of the 'testbadword' bad word created by the first code example on this page.
    /// Expects the CreateBadWord method to be run first.
    /// </summary>
    private bool CheckSingleBadWord()
    {
        // Prepare parameters for selection of the checked bad word
        string where = "[WordExpression] = 'testbadword' ";

        // Get the data
        DataSet words = BadWordInfoProvider.GetBadWords(where, null);

        if (!DataHelper.DataSourceIsEmpty(words))
        {
            // Get DataRow with bad word
            DataRow wordDr = words.Tables[0].Rows[0];

            // Get BadWordInfo object
            BadWordInfo badWord = new BadWordInfo(wordDr);

            // String to be checked for presence of the bad word
            string text = "This is a string containing the sample testbadword, which can be created by the first code example on this page.";

            // Hashtable that will contain found bad words
            Hashtable foundWords = new Hashtable();

            // Modify the string according to found bad words and return which action should be performed
            BadWordActionEnum action = BadWordInfoProvider.CheckBadWord(badWord, null, null, ref text, foundWords, 0);

            if (foundWords.Count != 0)
            {
                switch (action)
                {
                    case BadWordActionEnum.Deny:
                        // Some additional actions performed here ...
                        break;

                    case BadWordActionEnum.RequestModeration:
                        // Some additional actions performed here ...
                        break;

                    case BadWordActionEnum.Remove:
                        // Some additional actions performed here ...
                        break;

                    case BadWordActionEnum.Replace:
                        // Some additional actions performed here ...
                        break;

                    case BadWordActionEnum.ReportAbuse:
                        // Some additional actions performed here ...
                        break;

                    case BadWordActionEnum.None:
                        // Some additional actions performed here ...
                        break;
                }

                return true;
            }

            apiCheckSingleBadWord.ErrorMessage = "Bad word 'testbadword' is not present in the checked string.";
            return false;
        }

        return false;
    }


    /// <summary>
    /// Checks the declared custom string for presence of any defined bad word.
    /// </summary>
    private bool CheckAllBadWords()
    {
        // String to be checked for presence of bad words
        string text = "This is a string containing the sample testbadword, which can be created by the first code example on this page. It also contains the word asshole, which is one of the default bad words defined in the system.";

        // Hashtable that will contain found bad words
        Hashtable foundWords = new Hashtable();

        // Modify the string according to found bad words and return which action should be performed
        BadWordActionEnum action = BadWordInfoProvider.CheckAllBadWords(null, null, ref text, foundWords);

        if (foundWords.Count != 0)
        {
            switch (action)
            {
                case BadWordActionEnum.Deny:
                    // Some additional actions performed here ...
                    break;

                case BadWordActionEnum.RequestModeration:
                    // Some additional actions performed here ...
                    break;

                case BadWordActionEnum.Remove:
                    // Some additional actions performed here ...
                    break;

                case BadWordActionEnum.Replace:
                    // Some additional actions performed here ...
                    break;

                case BadWordActionEnum.ReportAbuse:
                    // Some additional actions performed here ...
                    break;

                case BadWordActionEnum.None:
                    // Some additional actions performed here ...
                    break;
            }

            return true;
        }

        return false;
    }

    #endregion
}
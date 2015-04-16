using System;
using System.Net;
using System.Threading;
using System.Web.Security;

using CMS.Base;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_System_Debug_System_DebugLoad : CMSDebugPage
{
    #region "Variables"

    private static bool mCancel = true;
    private static bool mRun;
    private static int mSuccessRequests;
    private static int mErrors;
    private static int mCurrentThreads;

    private static string mUserName = "";
    private static string mDuration = "";
    private static string mIterations = "1000";
    private static string mInterval = "";
    private static string mThreads = "10";
    private static string mUserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.2; .NET4.0C; .NET4.0E; MS-RTC LM 8)";
    private static string mURLs = "~/Home.aspx";
    private static bool mSplitURLs;

    protected static string mLastError = null;

    #endregion


    #region "Request loader"

    /// <summary>
    /// Request loader class.
    /// </summary>
    protected class RequestLoader
    {
        public string[] URLs = null;
        public DateTime RunUntil = DateTimeHelper.ZERO_TIME;
        public int NumberOfIterations = -1;
        public int WaitInterval = 0;
        public string UserAgent = null;
        public string UserName = null;


        /// <summary>
        /// Returns true if the loader is canceled (exceeds the execution time, exceeds an allowed number of iterations or is forcibly canceled).
        /// </summary>
        protected bool IsCanceled()
        {
            return mCancel || ((RunUntil != DateTimeHelper.ZERO_TIME) && (DateTime.Now > RunUntil)) || ((NumberOfIterations != -1) && (NumberOfIterations == 0));
        }


        /// <summary>
        /// Runs the load to the URLs.
        /// </summary>
        public void Run()
        {
            mCurrentThreads++;

            // Prepare the client
            WebClient client = new WebClient();

            // Authenticate specified user
            if (!string.IsNullOrEmpty(UserName))
            {
                client.Headers.Add("Cookie", ".ASPXFORMSAUTH=" + FormsAuthentication.GetAuthCookie(UserName, false).Value);
            }

            // Add user agent header
            if (!string.IsNullOrEmpty(UserAgent))
            {
                client.Headers.Add("user-agent", UserAgent);
            }

            while (!IsCanceled())
            {
                // Run the list of URLs
                foreach (string url in URLs)
                {
                    if (!string.IsNullOrEmpty(url))
                    {
                        if (IsCanceled())
                        {
                            break;
                        }

                        // Wait if some interval specified
                        if (WaitInterval > 0)
                        {
                            Thread.Sleep(WaitInterval);
                        }

                        try
                        {
                            // Get the page
                            client.DownloadData(url);

                            mSuccessRequests++;
                        }
                        catch (Exception ex)
                        {
                            mLastError = ex.Message;
                            mErrors++;
                        }
                    }
                }

                // Decrease number of iterations
                if (NumberOfIterations > 0)
                {
                    NumberOfIterations--;
                }
            }

            // Dispose the client
            client.Dispose();

            mCurrentThreads--;
        }
    }

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EnsureScriptManager();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!URLHelper.IsPostback())
        {
            if ((mCurrentThreads > 0) || (mSuccessRequests > 0) || (mErrors > 0))
            {
                txtDuration.Text = mDuration;
                txtInterval.Text = mInterval;
                txtIterations.Text = mIterations;
                txtThreads.Text = mThreads;
                txtURLs.Text = mURLs;
                txtUserAgent.Text = mUserAgent;
                userElem.Value = mUserName;
                chkSplitUrls.Checked = mSplitURLs;
            }
        }
        else
        {
            if (mRun && (mCurrentThreads == 0))
            {
                // Enable the form when the load finished
                mRun = false;
                EnableAll();
                pnlBody.Update();
            }
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        lblInfo.Text = String.Format(GetString("DebugLoad.Info"), mCurrentThreads, mSuccessRequests, mErrors);
        if (!String.IsNullOrEmpty(mLastError))
        {
            ShowError(mLastError);
        }

        btnStart.Text = GetString("DebugLoad.Generate");
        btnStop.Text = GetString("DebugLoad.Stop");
        btnReset.Text = GetString("DebugLoad.Reset");

        if (mCurrentThreads > 0)
        {
            DisableAll();
        }
        btnStop.Enabled = (mCurrentThreads > 0);
    }


    protected void btnReset_Click(object sender, EventArgs e)
    {
        mSuccessRequests = 0;
        mErrors = 0;
        mLastError = "";
    }


    protected void btnStop_Click(object sender, EventArgs e)
    {
        mRun = false;
        mCancel = true;
        while (mCurrentThreads > 0)
        {
            Thread.Sleep(100);
        }
        mCurrentThreads = 0;

        EnableAll();
        btnStop.Enabled = false;

        mLastError = "";
    }


    protected void btnStart_Click(object sender, EventArgs e)
    {
        mLastError = "";
        mCancel = false;
        mSuccessRequests = 0;
        mErrors = 0;
        mRun = true;

        mDuration = txtDuration.Text.Trim();
        mInterval = txtInterval.Text.Trim();
        mIterations = txtIterations.Text.Trim();
        mThreads = txtThreads.Text.Trim();
        mURLs = txtURLs.Text.Trim();
        mUserAgent = txtUserAgent.Text.Trim();
        mUserName = ValidationHelper.GetString(userElem.Value, "");
        mSplitURLs = chkSplitUrls.Checked;

        if (!String.IsNullOrEmpty(txtURLs.Text))
        {
            // Prepare the parameters
            string[] urls = txtURLs.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < urls.Length; i++)
            {
                urls[i] = URLHelper.GetAbsoluteUrl(urls[i]);
            }

            int newThreads = ValidationHelper.GetInteger(txtThreads.Text, 0);
            if (newThreads > 0)
            {
                int duration = ValidationHelper.GetInteger(txtDuration.Text, 0);
                int interval = ValidationHelper.GetInteger(txtInterval.Text, 0);
                int iterations = ValidationHelper.GetInteger(txtIterations.Text, 0);
                bool splitUrls = ValidationHelper.GetBoolean(chkSplitUrls.Checked, false);

                DateTime runUntil = DateTime.Now.AddSeconds(duration);

                // Divide URLs between threads
                string[][] partUrls = null;
                if (splitUrls)
                {
                    // Do not run more threads than URLs
                    newThreads = Math.Min(urls.Length, newThreads);

                    partUrls = new string[newThreads][];

                    int size = (int)Math.Ceiling((double)urls.Length / newThreads);
                    for (int i = 0; i < newThreads; i++)
                    {
                        size = Math.Min(size, urls.Length - i * size);
                        partUrls[i] = new string[size];
                        for (int j = 0; j < size; j++)
                        {
                            partUrls[i][j] = urls[i * size + j];
                        }
                    }
                }

                // Run specified number of threads
                for (int i = 0; i < newThreads; i++)
                {
                    // Prepare the loader object
                    RequestLoader loader = new RequestLoader();
                    loader.URLs = (splitUrls ? partUrls[i] : urls);
                    loader.WaitInterval = interval;
                    loader.UserAgent = txtUserAgent.Text.Trim();
                    loader.UserName = ValidationHelper.GetString(userElem.Value, "").Trim();
                    if (duration > 0)
                    {
                        loader.RunUntil = runUntil;
                    }
                    if (iterations > 0)
                    {
                        loader.NumberOfIterations = iterations;
                    }

                    // Start new thread
                    CMSThread newThread = new CMSThread(loader.Run);
                    newThread.Start();
                }

                DisableAll();
                btnStop.Enabled = true;
                btnReset.Enabled = true;
            }
        }
    }


    private void EnableAll()
    {
        txtDuration.Enabled = true;
        txtInterval.Enabled = true;
        txtIterations.Enabled = true;
        txtThreads.Enabled = true;
        txtURLs.Enabled = true;
        txtUserAgent.Enabled = true;
        userElem.Enabled = true;
        chkSplitUrls.Enabled = true;
        btnStart.Enabled = true;
    }


    private void DisableAll()
    {
        txtDuration.Enabled = false;
        txtInterval.Enabled = false;
        txtIterations.Enabled = false;
        txtThreads.Enabled = false;
        txtURLs.Enabled = false;
        txtUserAgent.Enabled = false;
        userElem.Enabled = false;
        chkSplitUrls.Enabled = false;
        btnStart.Enabled = false;
    }
}
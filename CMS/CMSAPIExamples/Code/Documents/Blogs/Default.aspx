<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSAPIExamples_Code_Documents_Blogs_Default" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Blog --%>
    <cms:LocalizedHeading ID="headCreateBlog" runat="server" Text="Blog" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateBlog" runat="server" ButtonText="Create blog" InfoMessage="Blog 'My new blog' was created." ErrorMessage="Root page '/' was not found." />
    <cms:APIExample ID="apiGetAndUpdateBlog" runat="server" ButtonText="Get and update blog" APIExampleType="ManageAdditional" InfoMessage="Blog 'My new blog' was updated." ErrorMessage="Blog 'My new blog' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateBlogs" runat="server" ButtonText="Get and bulk update blogs" APIExampleType="ManageAdditional" InfoMessage="All blogs matching the condition were updated." ErrorMessage="Blogs matching the condition were not found." />
    <%-- Blog post --%>
    <cms:LocalizedHeading ID="headCreateBlogPost" runat="server" Text="Blog post" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateBlogPost" runat="server" ButtonText="Create post" InfoMessage="Post 'My new post' was created." ErrorMessage="Blog 'My new blog' was not found." />
    <cms:APIExample ID="apiGetAndUpdateBlogPost" runat="server" ButtonText="Get and update post" APIExampleType="ManageAdditional" InfoMessage="Post 'My new post' was updated." ErrorMessage="Post 'My new post' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateBlogPosts" runat="server" ButtonText="Get and bulk update posts" APIExampleType="ManageAdditional" InfoMessage="All posts matching the condition were updated." ErrorMessage="Posts matching the condition were not found." />
    <%-- Blog comment --%>
    <cms:LocalizedHeading ID="headCreateBlogComment" runat="server" Text="Blog comment" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateBlogComment" runat="server" ButtonText="Create comment" InfoMessage="Comment 'My new comment' was created." ErrorMessage="Post 'My new blog post' was not found." />
    <cms:APIExample ID="apiGetAndUpdateBlogComment" runat="server" ButtonText="Get and update comment" APIExampleType="ManageAdditional" InfoMessage="Comment 'My new comment' was updated." ErrorMessage="Comment 'My new comment' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateBlogComments" runat="server" ButtonText="Get and bulk update comments" APIExampleType="ManageAdditional" InfoMessage="All comments matching the condition were updated." ErrorMessage="Comments matching the condition were not found." />
    <%-- Blog post subscription --%>
    <cms:LocalizedHeading ID="headCreateBlogPostSubscription" runat="server" Text="Blog post subscription" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiCreateBlogPostSubscription" runat="server" ButtonText="Create subscription" InfoMessage="Subscription 'My new subscription' was created." ErrorMessage="Comment 'My new comment' was not found." />
    <cms:APIExample ID="apiGetAndUpdateBlogPostSubscription" runat="server" ButtonText="Get and update subscription" APIExampleType="ManageAdditional" InfoMessage="Subscription 'My new subscription' was updated." ErrorMessage="Subscription 'My new subscription' was not found." />
    <cms:APIExample ID="apiGetAndBulkUpdateBlogPostSubscriptions" runat="server" ButtonText="Get and bulk update subscriptions" APIExampleType="ManageAdditional" InfoMessage="All subscriptions matching the condition were updated." ErrorMessage="Subscriptions matching the condition were not found." />
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Blog post subscription --%>
    <cms:LocalizedHeading ID="headDeleteBlogPostSubscription" runat="server" Text="Blog post subscription" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteBlogPostSubscription" runat="server" ButtonText="Delete subscription" APIExampleType="CleanUpMain" InfoMessage="Subscription 'My new subscription' and all its dependencies were deleted." ErrorMessage="Subscription 'My new subscription' was not found." />
    <%-- Blog comment --%>
    <cms:LocalizedHeading ID="headDeleteBlogComment" runat="server" Text="Blog comment" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteBlogComment" runat="server" ButtonText="Delete comment" APIExampleType="CleanUpMain" InfoMessage="Comment 'My new comment' and all its dependencies were deleted." ErrorMessage="Comment 'My new comment' was not found." />
    <%-- Blog post --%>
    <cms:LocalizedHeading ID="headDeleteBlogPost" runat="server" Text="Blog post" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteBlogPost" runat="server" ButtonText="Delete post" APIExampleType="CleanUpMain" InfoMessage="Post 'My new post' and all its dependencies were deleted." ErrorMessage="Post 'My new post' was not found." />
    <%-- Blog --%>
    <cms:LocalizedHeading ID="headDeleteBlog" runat="server" Text="Blog" Level="4" EnableViewState="false" />
    <cms:APIExample ID="apiDeleteBlog" runat="server" ButtonText="Delete blog" APIExampleType="CleanUpMain" InfoMessage="Blog 'My new blog' and all its dependencies were deleted." ErrorMessage="Blog 'My new blog' was not found." />
</asp:Content>

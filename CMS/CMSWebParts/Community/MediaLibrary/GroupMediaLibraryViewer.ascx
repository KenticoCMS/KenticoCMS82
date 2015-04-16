<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Community_MediaLibrary_GroupMediaLibraryViewer" CodeFile="~/CMSWebParts/Community/MediaLibrary/GroupMediaLibraryViewer.ascx.cs" %>
<%@ Register Assembly="CMS.MediaLibrary" Namespace="CMS.MediaLibrary" TagPrefix="cms" %>
<cms:BasicRepeater ID="repLibraries" runat="server" />
<cms:MediaLibraryDataSource ID="srcLib" runat="server" />
<div class="Pager">
    <cms:UniPager ID="pagerElem" runat="server" />
</div>

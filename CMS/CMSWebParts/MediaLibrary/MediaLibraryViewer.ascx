<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_MediaLibrary_MediaLibraryViewer" CodeFile="~/CMSWebParts/MediaLibrary/MediaLibraryViewer.ascx.cs" %>
<%@ Register Assembly="CMS.MediaLibrary" Namespace="CMS.MediaLibrary" TagPrefix="cms" %>
<cms:BasicRepeater ID="repMediaLib" runat="server" />
<cms:MediaLibraryDataSource ID="srcMediaLib" runat="server" />
<div class="Pager">
    <cms:UniPager ID="pagerElem" runat="server" />
</div>

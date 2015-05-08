<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ObjectViewModel>" %>

<!DOCTYPE html>
<html>
<head runat="server">
	<title><%: Model.Title %></title>
	<link rel="stylesheet" type="text/css" href="<%: Url.Content("~/Scripts/jqwidgets/styles/jqx.base.css") %>" />
	<link rel="stylesheet" type="text/css" href="<%: Url.Content("~/Scripts/jqwidgets/styles/jqx.darkblue.css") %>" />
	<link rel="stylesheet" type="text/css" href="<%: Url.Content("~/Scripts/jqwidgets/styles/jqx.office.css") %>" />
	<link rel="stylesheet" type="text/css" href="<%: Url.Content("~/Scripts/jqwidgets/styles/jqx.orange.css") %>" />
	<link rel="stylesheet" type="text/css" href="<%: Url.Content("~/Scripts/jqwidgets/styles/jqx.metro.css") %>" />
	<script src="<%: Url.Content("~/Scripts/jquery-1.11.1.min.js") %>"></script>
	<script src="<%: Url.Content("~/Scripts/jqwidgets/jqxcore.js") %>"></script>
	<script src="<%: Url.Content("~/Scripts/jqwidgets/jqx-all.js") %>"></script>
	<script src="<%: Url.Content("~/Scripts/FormValidation.js") %>"></script>
	<script src="<%: Url.Content("~/Scripts/Master.js") %>"></script>
	
	<link rel="stylesheet" type="text/css" href="<%: Url.Content("~/Content/Master.css") %>">
</head>
<body oncontextmenu="return false;">
	<div class="page">
		<div class="menu">
			<%: Html.Partial(Model.Menu) %>
		</div>
		<div class="page-content">
			<%: Html.Partial(Model) %>
		</div>
		
		<%: Html.Partial("Messages") %>
	</div>
</body>
</html>
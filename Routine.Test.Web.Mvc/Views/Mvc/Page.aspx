<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<% var model = Model as ObjectViewModel; %>
<!DOCTYPE html>
<html>
<head runat="server">
	<title><%= (Model as ObjectViewModel).Title %></title>
	<link rel="stylesheet" type="text/css" href="~/Scripts/jqwidgets/styles/jqx.base.css" />
    <link rel="stylesheet" type="text/css" href="~/Scripts/jqwidgets/styles/jqx.darkblue.css" />
	<link rel="stylesheet" type="text/css" href="~/Content/Master.css">
	<script src="/Scripts/jquery-1.11.1.min.js"></script>
	<script src="/Scripts/jqwidgets/jqxcore.js"></script>
	<script src="/Scripts/jqwidgets/jqx-all.js"></script>
	<script src="/Scripts/Master.js"></script>
</head>
<body oncontextmenu="return false;">
	<div class="segment main">
		<div class="wrapper">
			<% model.Render(Html); %>
		</div>
	</div>
	<div class="segment menu">
		<div class="wrapper">
			<a href="<%= Url.Action("Index", "App") %>"><img runat="server" src="~/Content/Images/top_logo.png" /></a>
			<% model.Application.Menu.Render(Html); %>
		</div>
	</div>
	<footer>
		<div class="wrapper">
			routineframework.org &copy; 2014
		</div>
	</footer>
</body>


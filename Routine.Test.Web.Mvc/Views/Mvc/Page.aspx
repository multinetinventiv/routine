<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<% var model = Model as ObjectViewModel; %>
<!DOCTYPE html>
<html>
<head runat="server">
	<title><%= (Model as ObjectViewModel).Title %></title>
	<link rel="stylesheet" type="text/css" href="~/Content/ui/ui.css">
	<link rel="stylesheet" type="text/css" href="~/Content/ui/ui.grid.css">
	<link rel="stylesheet" type="text/css" href="~/Content/Master.css">
	<script src="http://code.jquery.com/jquery-1.9.1.min.js"></script>
	<script src="/Scripts/ui.js"></script>
	<script src="/Scripts/ui.grid.js"></script>
	<script src="/Scripts/ui.numeric.js"></script>
	<script src="/Scripts/Master.js"></script>
</head>
<body>
	<header>
		<div class="wrapper">
			<h1>Sample Mvc Application</h1>
			<a href="<%= Url.Action("Index", "App") %>"><img runat="server" src="~/Content/Images/top_logo.png" /></a>
		</div>
	</header>
	<div class="segment menu">
		<div class="wrapper">
			<% model.Application.Menu.Render(Html); %>
		</div>
	</div>
	<div class="segment main">
		<div class="wrapper">
			<% model.Render(Html); %>
		</div>
	</div>
	<footer>
		<div class="wrapper">
			routineframework.org &copy; 2014
		</div>
	</footer>
</body>


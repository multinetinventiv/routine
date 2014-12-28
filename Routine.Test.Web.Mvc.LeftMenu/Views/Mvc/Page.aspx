<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<% var model = Model as ObjectViewModel; %>
<!DOCTYPE html>
<html>
<head runat="server">
	<title><%= (Model as ObjectViewModel).Title %></title>
	<link rel="stylesheet" type="text/css" href="~/Scripts/jqwidgets/styles/jqx.base.css" />
	<link rel="stylesheet" type="text/css" href="~/Scripts/jqwidgets/styles/jqx.darkblue.css" />
	<link rel="stylesheet" type="text/css" href="~/Scripts/jqwidgets/styles/jqx.office.css" />
	<link rel="stylesheet" type="text/css" href="~/Scripts/jqwidgets/styles/jqx.orange.css" />
	<link rel="stylesheet" type="text/css" href="~/Scripts/jqwidgets/styles/jqx.metro.css" />
	<script src="/Scripts/jquery-1.11.1.min.js"></script>
	<script src="/Scripts/jqwidgets/jqxcore.js"></script>
	<script src="/Scripts/jqwidgets/jqx-all.js"></script>
	<script src="/Scripts/FormValidation.js"></script>
	<script src="/Scripts/Master.js"></script>
	
	<link rel="stylesheet" type="text/css" href="~/Content/Master.css">
</head>
<body oncontextmenu="return false;">
	<div class="page">
		<div class="menu">
			<% model.Menu.Render(Html); %>
		</div>
		<div class="page-content">
				<% model.Render(Html); %>
		</div>
		
		<% if (Context.Items["messages"] != null && ((List<string>)Context.Items["messages"]).Count > 0)
		{ %>
			<div class="messages">
				<div class="wrapper">
					<% foreach (var message in ((List<string>)Context.Items["messages"]))
					{ %>
						<div class="message">
							<%= message %>
						</div>
					<% } %>
				</div>
			</div>
		<% } %>
	</div>
</body>

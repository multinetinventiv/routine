<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title>Error</title>
</head>
<body>
	<div>
		<% if (Context.Items["messages"] != null && ((List<string>)Context.Items["messages"]).Count >0) { %>
			<% foreach (var message in ((List<string>)Context.Items["messages"])) { %>
				<h2><%= message %></h2>  
			<% } %>
		<% } %>
	</div>
</body>


<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% var model = Model as MenuViewModel; %>
<% var first = true; %>
<% foreach (var link in model.Links.Where(l => l.MarkedAs("Module")))
   { %>
<div class="menu-item">
	<div class="parent <%=" "+link.Title%>">
		<% link.RenderAs(Html, "Link"); %>
	</div>
	<div class="children<%=" "+link.Title%>">
			<% var firstChild = true; %>
			<% foreach(var sublink in model.Links.Where(l => l.MarkedAs("Search") && l.GetOperations(OperationTypes.Search).Any())) { %>
				<% if(sublink.Module == link.Module) { %>
					<% if(firstChild) { %>
						<% firstChild = false; %>
					<% } %>
					<% sublink.RenderAs(Html, "Link", "text",sublink.Title); %>
				<% } %>
			<% } %>
		</div>
	<% first = false; %>
	</div>
<% } %>

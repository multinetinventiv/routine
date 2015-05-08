<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MenuViewModel>" %>

<% foreach (var link in Model.Links.Where(l => l.MarkedAs("Module"))) { %>
<div class="menu-item">
	<div class="parent <%: " " + link.Title %>">
		<%: Html.Partial(link, "link") %>
	</div>
	<div class="children<%: " " + link.Title %>">
			<% var firstChild = true; %>
			<% foreach(var sublink in Model.Links.Where(l => l.MarkedAs("Search") && l.GetOperations(OperationTypes.Search).Any())) { %>
				<% if(sublink.Module == link.Module) { %>
					<% if(firstChild) { %>
						<% firstChild = false; %>
					<% } %>
					<%: Html.Partial(sublink, "link", new { text = sublink.Title }) %>
				<% } %>
			<% } %>
		</div>
	</div>
<% } %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MenuViewModel>" %>

<% var first = true; %>
<% foreach (var link in Model.Links.Where(l => l.MarkedAs("Module")))
   { %>
	<div class="parent">
		<% if(!first) { %>
			<span class="menu-separator">&bull;</span>
		<% } %>
		<%: Html.Partial(link, "link") %>
		<div class="children<%: first?"":" children-after-first"%>">
			<% var firstChild = true; %>
			<% foreach(var sublink in Model.Links.Where(l => l.MarkedAs("Search") && l.GetOperations(OperationTypes.Search).Any())) { %>
				<% if(sublink.Module == link.Module) { %>
					<% if(firstChild) { %>
						<% firstChild = false; %>
					<% } else { %>
						<span class="menu-separator">&bull;</span>
					<% } %>
					<%: Html.Partial(sublink, "link", new { text = "Find " + sublink.Title }) %>
				<% } %>
			<% } %>
		</div>
	</div>
	<% first = false; %>
<% } %>

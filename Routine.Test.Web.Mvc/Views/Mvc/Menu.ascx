<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% var model = Model as MenuViewModel; %>
<% var first = true; %>
<% foreach (var link in model.Links.Where(l => l.MarkedAs("Module")))
   { %>
	<div class="parent">
		<% if(!first) { %>
			<span class="menu-separator">&bull;</span>
		<% } %>
		<% link.RenderAs(Html, "Link"); %>
		<div class="children<%= first?"":" children-after-first"%>">
			<% var firstChild = true; %>
			<% foreach(var sublink in model.Links.Where(l => l.MarkedAs("Search") && l.OperationMenu.Any(o => !o.IsSeparator && o.ReturnsList && o.HasParameter))) { %>
				<% if(sublink.Module == link.Module) { %>
					<% if(firstChild) { %>
						<% firstChild = false; %>
					<% } else { %>
						<span class="menu-separator">&bull;</span>
					<% } %>
					<% sublink.RenderAs(Html, "Link", "text", "Find " + sublink.Title); %>
				<% } %>
			<% } %>
		</div>
	</div>
	<% first = false; %>
<% } %>

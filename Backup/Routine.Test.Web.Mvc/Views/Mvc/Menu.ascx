<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% var model = Model as MenuViewModel; %>
<% foreach(var link in model.Links) { %>
	<span class="menu-separator">&bull;</span> <% link.RenderAs(Html, "Link"); %>
<% } %>

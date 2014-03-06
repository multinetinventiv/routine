<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
				
<% var model = Model as MemberViewModel; %>
<span>
	<span class="member-value-data">
		<% foreach(var obj in model.List) { %>
			<span class="member-value-data-item"><% obj.RenderAs(Html, "Link"); %></span>
		<% } %>
	</span>
</span>

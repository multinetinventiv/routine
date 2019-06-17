<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DataViewModel>" %>
				
<span>
	<span class="member-value-data">
		<% foreach(var obj in Model.List) { %>
			<span class="member-value-data-item"><%: Html.Partial(obj, "link") %></span>
		<% } %>
	</span>
</span>

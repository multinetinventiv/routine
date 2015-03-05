<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MemberViewModel>" %>
				
<span>
	<span class="member-value-data">
		<% foreach(var obj in Model.List) { %>
			<img src="https://pbs.twimg.com/profile_images/1137602966/nd-tw-profile8_normal.png"/><span class="member-value-data-item"><%: Html.Partial(obj, "link") %></span>
		<% } %>
	</span>
</span>
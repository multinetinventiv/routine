<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
				
<% var model = Model as MemberViewModel; %>
<table>
	<% bool headerOk = false; %>
	<% foreach(var obj in model.List) { %>
		<% if(!headerOk) { %>
			<% headerOk = true; %>
			<tr>
				<% foreach(var col in obj.SimpleMembers) { %>
					<th><%= col.Text %>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</th>
				<% } %>
				<% foreach(var op in obj.SimpleOperations) { %>
					<th><%= op.Text %>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</th>
				<% } %>
				<th class="action">Details&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</th>
			</tr>
		<% } %>
		<tr>
			<% foreach(var col in obj.SimpleMembers) { %>
				<td><% col.Render(Html); %></td>
			<% } %>
			<% foreach(var op in obj.SimpleOperations) { %>
				<td class="action"><% op.Render(Html); %></td>
			<% } %>
			<td class="action"><% obj.RenderAs(Html, "Link", "text", "Open"); %></td>
		</tr>
	<% } %>
</table>

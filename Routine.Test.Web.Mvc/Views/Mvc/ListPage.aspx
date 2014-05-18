<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
				
<% var model = Model as VariableViewModel; %>
<table>
<% bool headerOk = false; %>
<% foreach(var obj in model.List) { %>
	<% if(!headerOk) { %>
		<% headerOk = true; %>
	<thead>
		<tr>
		<% foreach(var col in obj.SimpleMembers) { %>
			<th class="data-column"><%= col.Text %></th>
		<% } %>
		</tr>
	</thead>
	<tbody>
	<% } %>
		<tr>
	<% foreach(var col in obj.SimpleMembers) { %>
		<td><% col.Render(Html); %></td>
	<% } %>
		</tr>
<% } %>
	</tbody>
</table>
<div class="context-menus">
<% var i = 0; %>
<% foreach(var obj in model.List) { %>
	<div class="context-menu context-menu-<%= i %>">
		<ul>
			<li><b><% obj.RenderAs(Html, "Link", "text", "Open"); %></b></li>
		</ul>
	</div>
	<% i++; %>
<% } %>
</div>

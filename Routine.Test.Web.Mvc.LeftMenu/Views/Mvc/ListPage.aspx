<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
				
<% var model = Model as VariableViewModel; %>
<table>
<% bool headerOk = false; %>
<% foreach(var obj in model.List) { %>
	<% if(!headerOk) { %>
		<% headerOk = true; %>
	<thead>
		<tr>
		<% foreach(var col in obj.GetMembers(MemberTypes.TableColumn)) { %>
			<th class="data-column"><%= col.Text %></th>
		<% } %>
		</tr>
	</thead>
	<tbody>
	<% } %>
		<tr>
	<% foreach(var col in obj.GetMembers(MemberTypes.TableColumn)) { %>
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
			<li>
				<b>
					<% if(Request["mode"] == "select") {  %>
						<input class="enable-double-click" type="button" value="Select" onclick="selectRow(this);"/>
						<input type="hidden" name="id" value="<%= obj.Option.Id %>" />
						<input type="hidden" name="value" value="<%= obj.Option.Value %>" />
					<% } else {
						obj.RenderAs(Html, "Link", "text", "Open"); 
					} %>
				</b>
			</li>
		</ul>
	</div>
	<% i++; %>
<% } %>
</div>

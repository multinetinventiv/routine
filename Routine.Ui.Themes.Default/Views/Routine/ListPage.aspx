<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<VariableViewModel>" %>
				
<table>
<% bool headerOk = false; %>
<% foreach(var obj in Model.List) { %>
	<% var columns = obj.GetMembers(MemberTypes.TableColumn); %>
	<% if(!headerOk) { %>
		<% headerOk = true; %>
	<thead>
		<tr>
		<% foreach(var column in columns) { %>
			<th class="data-column"><%: column.Text %></th>
		<% } %>
		</tr>
	</thead>
	<tbody>
	<% } %>
		<tr>
	<% foreach(var column in columns) { %>
			<td><%: Html.Partial(column) %></td>
	<% } %>
		</tr>
<% } %>
	</tbody>
</table>
<div class="context-menus">
<% var i = 0; %>
<% foreach(var obj in Model.List) { %>
	<div class="context-menu context-menu-<%: i %>">
		<ul>
			<li>
				<b>
					<% if(Request["mode"] == "select") {  %>
						<input class="enable-double-click" type="button" value="Select" onclick="selectRow(this);"/>
						<input type="hidden" name="id" value="<%: obj.Option.Id %>" />
						<input type="hidden" name="value" value="<%: obj.Option.Value %>" />
					<% } else { %>
						<%: Html.Partial(obj, "link", new {text = "Open"}) %>
					<% } %>
				</b>
			</li>
		</ul>
	</div>
	<% i++; %>
<% } %>
</div>

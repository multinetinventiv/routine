<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DataViewModel>" %>
				
<table>
<% bool headerOk = false; %>
<% foreach(var obj in Model.List) { %>
	<% var columns = obj.GetDatas(DataLocations.TableColumn); %>
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
			<li><b><%: Html.Partial(obj, "link", new {text = "Open"}) %></b></li>
	<% foreach(var op in obj.GetOperations(OperationTypes.Table)) { %>
			<li>
				<%: Html.Partial(op) %>
			</li>
	<% } %>
		</ul>
	</div>
	<% i++; %>
<% } %>
</div>

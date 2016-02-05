<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<VariableViewModel>" %>
				
<table>
<% bool headerOk = false; %>
<% foreach(var obj in Model.List) { %>
	<% if(!headerOk) { %>
		<% headerOk = true; %>
	<thead>
		<tr>
		<% foreach(var col in obj.GetDatas(DataLocations.TableColumn)) { %>
			<th class="data-column"><%: col.Text %></th>
		<% } %>
		</tr>
	</thead>
	<tbody>
	<% } %>
		<tr>
	<% foreach(var col in obj.GetDatas(DataLocations.TableColumn)) { %>
		<td><%: Html.Partial(col) %></td>
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
						<input type="button" value="Select" onclick="selectRow(this);"/>
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

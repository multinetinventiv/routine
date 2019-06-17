<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ObjectViewModel>" %>

<% var operations = Model.GetOperations(OperationTypes.Page); %>
<% var nameValueMembers = Model.GetDatas(DataLocations.PageNameValue); %>
<% var tableMembers = Model.GetDatas(DataLocations.PageTable); %>
<fieldset class="object-data">
	<legend class="object-data"><%: Model.Title %></legend>
	<% if(operations.Any()) { %>
		<div class="operation-tabs<%: Model.HasData?" collapsible-tabs":"" %>">
			<ul>
				<% foreach(var operation in operations) { %>
					<li><%: operation.Text %></li>
				<% } %>
			</ul>
			<% foreach(var operation in operations) { %>
				<div><%: Html.Partial(operation) %></div>
			<% } %>	
		</div>
	<% } %>
	<% if(nameValueMembers.Any()) { %>
		<dl class="data-list<%: operations.Any()?"":" no-operation"%>">
		<% foreach(var member in nameValueMembers) { %>
			<dt class="single-value"><%: member.Text %></dt>
			<dd class="single-value">
				<%: Html.Partial(member) %>
			</dd>
		<% } %>	
		</dl>
	<% } %>	
	<% if(tableMembers.Any()) { %>
		<div class="data-tabs tabs">
			<ul>
				<% foreach(var table in tableMembers) { %>
					<li><%: table.Text %></li>
				<% } %>
			</ul>
			<% foreach(var table in tableMembers) { %>
				<div><%: Html.Partial(table, "table") %></div>
			<% } %>	
		</div>
	<% } %>	
</fieldset>
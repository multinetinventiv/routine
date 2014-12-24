<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% var model = Model as ObjectViewModel; %>
<% var operations = model.GetOperations(OperationTypes.Page); %>
<% var nameValueMembers = model.GetMembers(MemberTypes.PageNameValue); %>
<% var tableMembers = model.GetMembers(MemberTypes.PageTable); %>
<fieldset class="object-data">
	<legend class="object-data"><%= model.Title %></legend>
	<% if(operations.Any()) { %>
		<div class="operation-tabs<%= model.HasMember?" collapsible-tabs":"" %>">
			<ul>
				<% foreach(var operation in operations) { %>
					<li><%= operation.Text %></li>
				<% } %>
			</ul>
			<% foreach(var operation in operations) { %>
				<div><% operation.Render(Html); %></div>
			<% } %>	
		</div>
	<% } %>
	<% if(nameValueMembers.Any()) { %>
		<dl class="data-list<%= operations.Any()?"":" no-operation"%>">
		<% foreach(var member in nameValueMembers) { %>
			<dt class="single-value"><%= member.Text %></dt>
			<dd class="single-value">
				<% member.Render(Html); %>
			</dd>
		<% } %>	
		</dl>
	<% } %>	
	<% if(tableMembers.Any()) { %>
		<div class="data-tabs tabs">
			<ul>
				<% foreach(var table in tableMembers) { %>
					<li><%= table.Text %></li>
				<% } %>
			</ul>
			<% foreach(var table in tableMembers) { %>
				<div><% table.RenderAs(Html, "Table"); %></div>
			<% } %>	
		</div>
	<% } %>	
</fieldset>
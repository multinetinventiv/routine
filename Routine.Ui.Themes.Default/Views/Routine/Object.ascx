<%@ Control Language="C#" Inherits="ViewUserControl<ObjectViewModel>" %>

<fieldset class="object-data">
    <legend class="object-data"><%: Model.Title %></legend>
	
	<% var operations = Model.GetOperations(OperationTypes.Page); %>
    <% if (operations.Any()) { %>
    <div class="operation-buttons">
        <% foreach (var operation in operations) { %>
			<%: Html.Partial(operation) %>
        <% } %>
    </div>
    <% } %>
	
	<% var nameValueMembers = Model.GetMembers(MemberTypes.PageNameValue); %>
    <% if (nameValueMembers.Any()) { %>
    <dl class="data-list<%: operations.Any()?"":" no-operation" %>">
        <% foreach (var member in nameValueMembers) { %>
        <dt class="single-value"><%: member.Text %></dt>
        <dd class="single-value">
            <%: Html.Partial(member) %>
        </dd>
        <% } %>
    </dl>
    <% } %>
	
	<% var tableMembers = Model.GetMembers(MemberTypes.PageTable); %>
    <% if (tableMembers.Any()) { %>
    <div class="data-tabs tabs">
        <ul>
            <% foreach (var table in tableMembers) { %>
            <li><%: table.Text.After("Get ") %></li>
            <% } %>
        </ul>
        <% foreach (var table in tableMembers) { %>
        <div><%: Html.Partial(table, "table") %></div>
        <% } %>
    </div>
    <% } %>
</fieldset>

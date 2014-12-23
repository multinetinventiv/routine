<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% var model = Model as ObjectViewModel; %>
<% var operations = model.GetOperations(OperationTypes.Search); %>
<fieldset class="search-data">
	<legend class="search-data"><%= model.Title %></legend>
	<div class="operation-tabs tabs">
			<ul>
				<% foreach(var operation in operations) { %>
					<li><%= operation.Text %></li>
				<% } %>
			</ul>
			<% foreach (var operation in operations) { %>
				<div><% operation.Render(Html, "text", "Search", "cancel", "false"); %></div>
			<% } %>	
	</div>
	<div class="search-result" />
</fieldset>
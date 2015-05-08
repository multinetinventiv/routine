<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ObjectViewModel>" %>

<% var operations = Model.GetOperations(OperationTypes.Search); %>
<fieldset class="search-data">
	<legend class="search-data"><%: Model.Title %></legend>
	<div class="operation-tabs tabs">
			<ul>
				<% foreach(var operation in operations) { %>
					<li><%: operation.Text %></li>
				<% } %>
			</ul>
			<% foreach (var operation in operations) { %>
				<div><%: Html.Partial(operation, new { text = "Search", cancel = "false" }) %></div>
			<% } %>	
	</div>
	<div class="search-result" />
</fieldset>
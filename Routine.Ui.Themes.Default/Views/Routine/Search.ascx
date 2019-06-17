<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ObjectViewModel>" %>

<fieldset class="search-data">
	<legend class="search-data"><%: Model.Title %></legend>
	<div class="operation-tabs tabs">
			<ul>
				<% foreach(var operation in Model.GetOperations(OperationTypes.Search)) { %>
					<li><%: operation.Text %></li>
				<% } %>
			</ul>
			<% foreach(var operation in Model.GetOperations(OperationTypes.Search)) { %>
				<div><%: Html.Partial(operation, new {text = "Search", mode = "inline"}) %></div>
			<% } %>	
	</div>
	<div class="search-result" />
</fieldset>
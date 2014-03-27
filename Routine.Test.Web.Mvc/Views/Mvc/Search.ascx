<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% var model = Model as ObjectViewModel; %>
<fieldset class="search-data">
	<legend class="search-data"><%= model.Title %></legend>
	<% foreach(var operation in model.OperationMenu.Where(op => op.IsSeparator || op.ReturnsList)) { %>
		<% operation.Render(Html); %>
	<% } %>
	<div class="search-result">
		<!-- TODO ajax setup to perform search operation -->
		<!-- TODO clear current result, create table from result & render it as grid -->
	</div>
</fieldset>
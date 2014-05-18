<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% var model = Model as ObjectViewModel; %>
<fieldset class="search-data">
	<legend class="search-data"><%= model.Title %></legend>
	<div class="operation-tabs tabs">
			<ul>
				<% foreach(var operation in model.OperationMenu.Where(o => !o.IsSeparator && o.ReturnsList && o.HasParameter)) { %>
					<li><%= operation.Text %></li>
				<% } %>
			</ul>
			<% foreach(var operation in model.OperationMenu.Where(o => !o.IsSeparator && o.ReturnsList && o.HasParameter)) { %>
				<div><% operation.Render(Html, "text", "Search", "cancel", "false"); %></div>
			<% } %>	
	</div>
	<div class="search-result">
		<!-- TODO ajax setup to perform search operation -->
		<!-- TODO clear current result, create table from result & render it as grid -->
	</div>
</fieldset>
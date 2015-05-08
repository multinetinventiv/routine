<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OperationViewModel>" %>

<% var mode = ViewData["mode"] as string ?? "menu"; %>
<% var text = ViewData["text"] as string ?? "OK"; %>
<% var cancel = ViewData["cancel"] as string ?? "true"; %>

<% if(Model.IsAvailable) { %>
	<form action="<%: Url.Route(Model) + (Request["modal"]=="true" ? "?mode=select":"") %>" class="operation-form" method="post">
	<% if(mode == "menu")  { %>
		<fieldset>
			<div class="content">
		<% if(!Model.HasParameter) { %>
				<div class="confirm">Are you sure?</div>
		<% } else { %>
				<dl class="parameter-list">
			<% int i = 0; %>
			<% foreach(var parameter in Model.Parameters) { %>
					<dt><%: parameter.Text %></dt>
					<dd>
						<%: Html.Partial(parameter, new { index = i }) %>
					</dd>
					<% i++; %>
			<% } %>
				</dl>
		<% } %>
		<% if(cancel == "true") { %>
				<input type="button" value="Cancel"/>
		<% } %>
				<input type="submit" value="<%: text %>"/>
			</div>
		</fieldset>
	<% } else if(mode == "table"){ %>
		<input type="submit" value="<%: Model.Text %>"/>
	<% } %>
	</form>
<% } else { %>
	<input type="submit" value="<%: Model.Text %>" class="disabled"/>
<% } %>
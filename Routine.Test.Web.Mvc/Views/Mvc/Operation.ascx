<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Routine.Ui" %>

<% var model = Model as OperationViewModel; %>
<% var mode = ViewData["mode"] as string??"menu"; %>
<% var text = ViewData["text"] as string??"OK"; %>
<% var cancel = ViewData["cancel"] as string??"true"; %>

<% if(model.IsAvailable) { %>
	<form action="<%= Url.Route(model) + (Request["modal"]=="true" ? "?mode=select":"") %>" class="operation-form" method="post">
	<% if(mode == "menu")  { %>
		<fieldset>
			<div class="content">
		<% if(!model.HasParameter) { %>
				<div class="confirm">Are you sure?</div>
		<% } else { %>
				<dl class="parameter-list">
			<% int i = 0; %>
			<% foreach(var parameter in model.Parameters) { %>
					<dt><%= parameter.Text %></dt>
					<dd>
						<% parameter.Render(Html, "index", i); %>
					</dd>
					<% i++; %>
			<% } %>
				</dl>
		<% } %>
			<% if(cancel == "true") { %>
				<input type="button" value="Cancel"/>
			<% } %>
				<input type="submit" value="<%= text %>"/>
			</div>
		</fieldset>
	<% } else if(mode == "table"){ %>
		<input type="submit" value="<%= model.Text %>"/>
	<% } %>
	</form>
<% } else { %>
	<input type="submit" value="<%= model.Text %>" class="disabled"/>
<% } %>
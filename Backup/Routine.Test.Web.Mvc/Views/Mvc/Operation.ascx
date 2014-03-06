<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% var model = Model as OperationViewModel; %>

<% if(model.IsSeparator) { %>
	<span class="operation-separator">|</span>
<% } else if(model.IsAvailable) { %>
	<form action="<%= Url.Route(model) %>" class="operation-form" method="post">
	<% if(!model.HasParameter) { %>
		<input type="submit" value="<%= model.Text %>" class="needs-confirm"/>
	<% } else { %>
		<fieldset>
			<legend><%= model.Text %></legend>
			<div class="content">
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
				<input type="submit" value="<%= model.Text %>"/>
			</div>
		</fieldset>
	<% } %>
	</form>
<% } else if(!model.IsAvailable) { %>
	<input type="submit" value="<%= model.Text %>" class="disabled"/>
<% } %>
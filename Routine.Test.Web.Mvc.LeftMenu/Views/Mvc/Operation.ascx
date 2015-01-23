<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% var model = Model as OperationViewModel; %>
<% var dataModalId = model.Object.Id + model.Id + "-div"; %>
<% var text = ViewData["text"] as string??model.Text; %>
<% var mode = ViewData["mode"] as string ?? ((model.HasParameter || model.ConfirmationRequired) ? "modal" : "inline"); %>
<% if(mode == "modal")  { %>
	<input data-modal-id="<%= dataModalId %>" type="button" class="operation-button" value="<%= model.Text %>"/>
    <div id="<%= dataModalId %>" class="operation-form-div">
	    <div class="header"><%= model.Text %></div>
	    <div class="modal-content">
			<% text = model.HasParameter ? model.Text : "OK"; %>
<% } %>

<% if(model.IsAvailable) { %>
	<form action="<%= Url.Route(model) + (Request["modal"]=="true" ? "?mode=select":"") %>" class="operation-form" method="post">
<% } else if(!model.IsAvailable) { %>
	<form action="<%= Url.Route(model) + (Request["modal"]=="true" ? "?mode=select":"") %>" class="operation-form disabled" style="display: none" method="post">
<% } %>
		<% if(model.HasParameter || model.ConfirmationRequired) { %>
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
			
			<% if (mode == "modal") { %>
			<input type="button" value="Cancel" data-modal-id="<%= dataModalId %>"/>
			<% } %>
		<% } %>

			<input type="submit" value="<%= text %>" <%= (!model.HasParameter && !model.ConfirmationRequired)?"class=\"operation-button\"":"" %>/>
		<% if(model.HasParameter || model.ConfirmationRequired) { %>
			</div>
		</fieldset>
		<% } %>
	</form>

<% if(mode == "modal")  { %>
	    </div>
    </div>
<% } %>
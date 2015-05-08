<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OperationViewModel>" %>

<% var dataModalId = Model.Object.Id + Model.Id + "-div"; %>
<% var text = ViewData["text"] as string ?? Model.Text; %>
<% var mode = ViewData["mode"] as string ?? ((Model.HasParameter || Model.ConfirmationRequired) ? "modal" : "inline"); %>
<% if(mode == "modal")  { %>
	<input data-modal-id="<%: dataModalId %>" type="button" class="operation-button" value="<%: Model.Text %>"/>
    <div id="<%: dataModalId %>" class="operation-form-div">
	    <div class="header"><%: Model.Text %></div>
	    <div class="modal-content">
			<% text = Model.HasParameter ? Model.Text : "OK"; %>
<% } %>

<% if(Model.IsAvailable) { %>
	<form action="<%: Url.Route(Model) + (Request["modal"]=="true" ? "?mode=select":"") %>" class="operation-form" method="post">
<% } else if(!Model.IsAvailable) { %>
	<form action="<%: Url.Route(Model) + (Request["modal"]=="true" ? "?mode=select":"") %>" class="operation-form disabled" style="display: none" method="post">
<% } %>
		<% if(Model.HasParameter || Model.ConfirmationRequired) { %>
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
			
			<% if (mode == "modal") { %>
			<input type="button" value="Cancel" data-modal-id="<%: dataModalId %>"/>
			<% } %>
		<% } %>

			<input type="submit" value="<%: text %>" <%: (!Model.HasParameter && !Model.ConfirmationRequired)?"class=\"operation-button\"":"" %>/>
		<% if(Model.HasParameter || Model.ConfirmationRequired) { %>
			</div>
		</fieldset>
		<% } %>
	</form>

<% if(mode == "modal")  { %>
	    </div>
    </div>
<% } %>
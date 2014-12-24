<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% var model = Model as ParameterViewModel; %>
<% var index = ViewData["index"]; %>
<input type="hidden" name="parameters[<%= index %>].key" value="<%= model.Id %>" />
<% if(!model.HasOptions) { %>
	<% if(model.IsValue) { %>
		<input type="text" name="parameters[<%= index %>].value" 
			data-type="<%= model.DataType %>" value="<%= model.DefaultValue %>" />
	<% } else { %>
		<input type="hidden" name="parameters[<%= index %>].value" />
		<input type="button" value="Search" class="modal-search" data-type="modal" data-route="<%= Url.Route(model.GetSearcher()) %>?modal=true"/>
	<% } %>
<% } else { %>
	<% if(model.DataType == "s-boolean") { %>
		<input type="checkbox" name="parameters[<%= index %>].value" 
			   data-type="<%= model.DataType %>" value="True"  />
		<input type="hidden" name="parameters[<%= index %>].value"
			   data-type="<%= model.DataType %>" value="False"  />
	<% } else { %>
	    <select name="parameters[<%= index %>].value">
		    <% foreach(var option in model.Options) { %>
			    <option value="<%= option.Id %>" <%= option.Value==model.DefaultValue?"selected=\"selected\"":"" %> ><%= option.Value %></option>
		    <% } %>
	    </select>
	<% } %>
<% } %>

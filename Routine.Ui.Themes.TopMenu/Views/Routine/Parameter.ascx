<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ParameterViewModel>" %>

<% var index = ViewData["index"]; %>
<input type="hidden" name="parameters[<%: index %>].key" value="<%: Model.Id %>" />
<% if(!Model.HasOptions) { %>
	<% if(Model.IsValue) { %>
		<input type="text" name="parameters[<%: index %>].value" 
			data-type="<%: Model.DataType %>" value="<%: Model.DefaultValue %>" />
	<% } else { %>
		<input type="hidden" name="parameters[<%: index %>].value" />
		<input type="button" value="Search" class="modal-search" data-type="modal" data-route="<%: Url.Route(Model.GetSearcher()) %>?modal=true"/>
	<% } %>
<% } else { %>
	<% if(Model.DataType == "s-boolean") { %>
		<input type="checkbox" name="parameters[<%: index %>].value" 
			   data-type="<%: Model.DataType %>" value="True"  />
		<input type="hidden" name="parameters[<%: index %>].value"
			   data-type="<%: Model.DataType %>" value="False"  />
	<% } else { %>
	    <select name="parameters[<%: index %>].value">
		    <% foreach(var option in Model.Options) { %>
			    <option value="<%: option.Id %>" <%: option.Value == Model.DefaultValue ? "selected=\"selected\"" : "" %> ><%: option.Value %></option>
		    <% } %>
	    </select>
	<% } %>
<% } %>

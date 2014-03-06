<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% var model = Model as ObjectViewModel; %>
<fieldset class="object-data">
	<legend class="object-data"><%= model.Title %></legend>
	<% if(model.HasOperation) { %>
		<% foreach(var operation in model.OperationMenu) { %>
			<% operation.Render(Html); %>
		<% } %>
		<div class="operation-list-placeholder"></div>
	<% } %>
	<% if(model.HasSimpleMember) { %>
		<dl class="data-list<%= model.HasOperation?"":" no-operation"%>">
		<% foreach(var member in model.SimpleMembers) { %>
			<dt class="single-value"><%= member.Text %></dt>
			<dd class="single-value">
				<% member.Render(Html); %>
			</dd>
		<% } %>	
		</dl>
	<% } %>	
	<% if(model.HasTableMember) { %>
		<div class="data-tabs tabs">
			<ul>
				<% foreach(var member in model.TableMembers) { %>
					<li><a href="#<%= member.Id %>"><%= member.Text %></a></li>
				<% } %>
			</ul>
			<% foreach(var member in model.TableMembers) { %>
				<div id="<%= member.Id %>"><% member.RenderAs(Html, "Table"); %></div>
			<% } %>	
		</div>
	<% } %>	
</fieldset>
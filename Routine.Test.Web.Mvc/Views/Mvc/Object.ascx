<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% var model = Model as ObjectViewModel; %>
<fieldset class="object-data">
	<legend class="object-data"><%= model.Title %></legend>
	<% if(model.HasOperation) { %>
		<div class="operation-tabs<%= model.HasSimpleMember || model.HasTableMember?" collapsible-tabs":"" %>">
			<ul>
				<% foreach(var operation in model.OperationMenu.Where(m => !m.IsSeparator)) { %>
					<li><%= operation.Text %></li>
				<% } %>
			</ul>
			<% foreach(var operation in model.OperationMenu.Where(m => !m.IsSeparator)) { %>
				<div><% operation.Render(Html); %></div>
			<% } %>	
		</div>
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
					<li><%= member.Text %></li>
				<% } %>
			</ul>
			<% foreach(var member in model.TableMembers) { %>
				<div><% member.RenderAs(Html, "Table"); %></div>
			<% } %>	
		</div>
	<% } %>	
</fieldset>
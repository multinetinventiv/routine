<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% var messages = Context.Items["messages"] as List<string>; %>
<% if (messages != null && messages.Count > 0) { %>
	<div class="messages">
		<div class="wrapper">
			<% foreach (var message in messages) { %>
				<div class="message">
					<%: message %>
				</div>
			<% } %>
		</div>
	</div>
<% } %>

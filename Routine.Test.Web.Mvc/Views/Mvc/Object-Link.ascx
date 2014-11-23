<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Routine.Ui" %>

<% var model = Model as ObjectViewModel; %>
<% var text = ViewData["text"] as string??model.Title; %>
<% if(model.HasDetail) { %>
	<a href="<%= Url.Route(model) %>" data-module="<%= model.Module %>" class="reference-link"><%= text %></a>
<% } else if(model.ViewModelId == "s-boolean"){ %>
	<span><%= text == "True"?"&#10004;":"&#10005" %></span>
<% } else { %>
	<span><%= text %></span>
<% } %>
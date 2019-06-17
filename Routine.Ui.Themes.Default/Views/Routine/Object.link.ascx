<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ObjectViewModel>" %>

<% var text = ViewData["text"] as string ?? Model.Title; %>
<% if(Model.HasDetail) { %>
	<a href="<%: Url.Route(Model) %>" data-module="<%: Model.Module %>" class="reference-link"><%: text %></a>
<% } else if(Model.ViewModelId == "s-boolean"){ %>
	<span><%: text == "True" ? "&#10004;" : "&#10005" %></span>
<% } else { %>
	<span><%: text %></span>
<% } %>
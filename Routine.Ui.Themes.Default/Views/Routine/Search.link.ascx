<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ObjectViewModel>" %>

<% var text = ViewData["text"] as string ?? Model.Title; %>
<a href="<%: Url.Route(Model) %>" data-module="<%: Model.Module %>" class="search-link"><%: text %></a>

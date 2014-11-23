﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Routine.Ui" %>

<% var model = Model as ObjectViewModel; %>
<% var text = ViewData["text"] as string??model.Title; %>
<a href="<%= Url.Route(model) %>" data-module="<%= model.Module %>" class="search-link"><%= text %></a>

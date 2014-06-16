<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<% var model = Model as ObjectViewModel; %>

<% model.Render(Html); %>


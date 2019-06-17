<%@ Control Language="C#" Inherits="ViewUserControl<ObjectViewModel>" %>
	
<a href="<%: Url.Route(Model) %>">Custom link: <%: Model.Title %></a>
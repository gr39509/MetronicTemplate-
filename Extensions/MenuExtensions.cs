// namespace NovaAccounts.Extensions;
// using System;
// using System.Collections.Generic;
// using System.Collections.Specialized;
// using System.Linq;
// using System.Web;
// using System.Web.Mvc;
//     public static class menuExtensions
//     {
//
//
//         public static MvcHtmlString MenuFirstLevelOpen(this HtmlHelper html, string text, string icon, List<string> lstControllers)
//         {
//             var context = html.ViewContext;
//             if (context.Controller.ControllerContext.IsChildAction)
//                 context = html.ViewContext.ParentActionViewContext;
//             var routeValues = context.RouteData.Values;
//             var currentAction = routeValues["action"].ToString();
//             var currentController = routeValues["controller"].ToString();
//             var isController = lstControllers.Find(c => c.Equals(currentController)) != null;
//             var clOne = isController ? "class='nav-item active open'" : " class='nav-item'";
//             var clIsSelected = isController ? "class='selected'" : string.Empty;
//             var clIsArrowOpen = isController ? "class='arrow open'" : "class='arrow'";
//             var str = $"<li {clOne}> <a href='javascript:;' class='nav-link nav-toggle'> <i class='{icon}'></i> <span class='title'> {text} </span><span {clIsSelected}></span> <span {clIsArrowOpen}></span> </a> <ul class='sub-menu'>";
//             return new MvcHtmlString(str);
//         }
//         public static MvcHtmlString MenuLevelClose(this HtmlHelper html)
//         {
//             var str = $"</ul></li>";
//             return new MvcHtmlString(str);
//         }
//         public static MvcHtmlString MenuSecondLevelOpen(this HtmlHelper html, string text, string icon, List<string> lstControllers)
//         {
//             var context = html.ViewContext;
//             if (context.Controller.ControllerContext.IsChildAction)
//                 context = html.ViewContext.ParentActionViewContext;
//             var routeValues = context.RouteData.Values;
//             var currentAction = routeValues["action"].ToString();
//             var currentController = routeValues["controller"].ToString();
//             var isController = lstControllers.Find(c => c.Equals(currentController)) != null;
//             var clOne = isController ? "class='nav-item active open'" : " class='nav-item'";
//             var clIsSelected = isController ? "class='selected'" : string.Empty;
//             var clIsArrowOpen = isController ? "class='arrow open'" : "class='arrow'";
//             var str = $"<li {clOne}> <a href='javascript:;' class='nav-link nav-toggle'> <i class='{icon}'></i> <span class='title'>{text} </span><span {clIsSelected}></span><span {clIsArrowOpen}></span> </a> <ul class='sub-menu'>";
//             return new MvcHtmlString(str);
//         }
//         public static MvcHtmlString MenuThirdLevel(this HtmlHelper html, string text, string action, string controller, string icon)
//         {
//             var context = html.ViewContext;
//             if (context.Controller.ControllerContext.IsChildAction)
//                 context = html.ViewContext.ParentActionViewContext;
//             var routeValues = context.RouteData.Values;
//             var currentAction = routeValues["action"].ToString();
//             var currentController = routeValues["controller"].ToString();
//             var isCurrentAction = currentController.Equals(controller, StringComparison.InvariantCultureIgnoreCase) && currentAction.Equals(action, StringComparison.InvariantCultureIgnoreCase) ? "class='nav-item active open'" : "class='nav-item'";
//
//             var str = $"<li {isCurrentAction}> <a href='/{controller}/{action}' class='nav-link'> <i class='{icon}'></i> {text} </a></li>";
//             return new MvcHtmlString(str);
//         }
//         public static MvcHtmlString LiActionLinkFirst(this HtmlHelper html, string text, string action, string controller, string icon)
//         {
//             var context = html.ViewContext;
//             if (context.Controller.ControllerContext.IsChildAction)
//                 context = html.ViewContext.ParentActionViewContext;
//             var routeValues = context.RouteData.Values;
//             var currentAction = routeValues["action"].ToString();
//             var currentController = routeValues["controller"].ToString();
//             var str = String.Format("<li {4}><a href='/{1}/{0}' class='nav-link'><i class='{2}'></i><span class='title'>{3}</span></a></li>",
//                 action, controller, icon, text,
//                 currentController.Equals(controller, StringComparison.InvariantCultureIgnoreCase) ?
//                 " class=\"nav-item start active\"" :
//                 " class=\"nav-item start\""
//             );
//             return new MvcHtmlString(str);
//         }
//         public static MvcHtmlString NovaSubstring(this HtmlHelper html, string text)
//         {
//             string str = text;
//             if (text.ToLower().Contains("m"))
//             {
//                 str = text.Length >= 14 ? $"<span class='tooltips' data-container='body' data-placement='bottom' data-original-title='{text}'>{text.Substring(0, 11)}...</span>" : text;
//             }
//             else
//             {
//                 str = text.Length >= 14 ? $"<span class='tooltips' data-container='body' data-placement='bottom' data-original-title='{text}'>{text.Substring(0, 13)}...</span>" : text;
//             }
//             return new MvcHtmlString(str);
//         }
//         public static MvcHtmlString LiActionInventoryHeader(this HtmlHelper html, string text, string icon)
//         {
//             var context = html.ViewContext;
//             if (context.Controller.ControllerContext.IsChildAction)
//                 context = html.ViewContext.ParentActionViewContext;
//             var routeValues = context.RouteData.Values;
//             var currentAction = routeValues["action"].ToString();
//             var currentController = routeValues["controller"].ToString();
//             List<string> lstControllers = new List<string> { "inventories" };
//             var isController = lstControllers.Find(c => c.Equals(currentController)) != null;
//             var str = String.Format("<li {2}> <a href='javascript:;' class='nav-link nav-toggle'><i class='{0}'></i><span class='title'>{1}</span><span {3}></span><span {4}></span> </a><ul class='sub-menu'>",
//                 icon, text, isController ?
//                 " class=\"nav-item  active open\"" :
//                  " class=\"nav-item\"",
//                 isController ?
//                 "class=\"selected\"" :
//                 string.Empty,
//                 isController ?
//                 "class=\"arrow open\"" :
//                 "class=\"arrow\""
//             );
//             return new MvcHtmlString(str);
//         }
//         public static MvcHtmlString LiActionPayrollHeader(this HtmlHelper html, string text, string icon)
//         {
//             var context = html.ViewContext;
//             if (context.Controller.ControllerContext.IsChildAction)
//                 context = html.ViewContext.ParentActionViewContext;
//             var routeValues = context.RouteData.Values;
//             var currentAction = routeValues["action"].ToString();
//             var currentController = routeValues["controller"].ToString();
//             List<string> lstControllers = new List<string> { "Payrolls", "Payscales" };
//             var isController = lstControllers.Find(c => c.Equals(currentController)) != null;
//             var str = String.Format("<li {2}> <a href='javascript:;' class='nav-link nav-toggle'><i class='{0}'></i><span class='title'>{1}</span><span {3}></span><span {4}></span> </a><ul class='sub-menu'>",
//                 icon, text, isController ?
//                 " class=\"nav-item  active open\"" :
//                  " class=\"nav-item\"",
//                 isController ?
//                 "class=\"selected\"" :
//                 string.Empty,
//                 isController ?
//                 "class=\"arrow open\"" :
//                 "class=\"arrow\""
//             );
//             return new MvcHtmlString(str);
//         }
//         public static MvcHtmlString LiActionMpaccHeader(this HtmlHelper html, string text, string icon)
//         {
//             var context = html.ViewContext;
//             if (context.Controller.ControllerContext.IsChildAction)
//                 context = html.ViewContext.ParentActionViewContext;
//             var routeValues = context.RouteData.Values;
//             var currentAction = routeValues["action"].ToString();
//             var currentController = routeValues["controller"].ToString();
//             List<string> lstControllers = new List<string> { "mpac", "Clients", "DepositOnAccount" };
//             var isController = lstControllers.Find(c => c.ToLower().Equals(currentController.ToLower())) != null;
//             var str = String.Format("<li {2}> <a href='javascript:;' class='nav-link nav-toggle'><i class='{0}'></i><span class='title'>{1}</span><span {3}></span><span {4}></span> </a><ul class='sub-menu'>",
//                 icon, text, isController ?
//                 " class=\"nav-item  active open\"" :
//                  " class=\"nav-item\"",
//                 isController ?
//                 "class=\"selected\"" :
//                 string.Empty,
//                 isController ?
//                 "class=\"arrow open\"" :
//                 "class=\"arrow\""
//             );
//             return new MvcHtmlString(str);
//         }
//
//         public static MvcHtmlString LiActionLinkHasSub(this HtmlHelper html, string text, string icon)
//         {
//             var context = html.ViewContext;
//             if (context.Controller.ControllerContext.IsChildAction)
//                 context = html.ViewContext.ParentActionViewContext;
//             var routeValues = context.RouteData.Values;
//             var currentAction = routeValues["action"].ToString();
//             var currentController = routeValues["controller"].ToString();
//             List<string> lstControllers = new List<string> { "GLDG", "cashiers" };
//             var isController = lstControllers.Find(c => c.ToLower().Equals(currentController.ToLower())) != null;
//             var str = String.Format("<li {2}> <a href='javascript:;' class='nav-link nav-toggle'><i class='{0}'></i><span class='title'>{1}</span><span {3}></span><span {4}></span> </a><ul class='sub-menu'>",
//                 icon, text, isController ?
//                 " class=\"nav-item  active open\"" :
//                  " class=\"nav-item\"",
//                 isController ?
//                 "class=\"selected\"" :
//                 string.Empty,
//                 isController ?
//                 "class=\"arrow open\"" :
//                 "class=\"arrow\""
//             );
//             return new MvcHtmlString(str);
//         }
//
//         public static MvcHtmlString LiActionLinkHasSubAdmin(this HtmlHelper html, string text, string icon)
//         {
//             var context = html.ViewContext;
//             if (context.Controller.ControllerContext.IsChildAction)
//                 context = html.ViewContext.ParentActionViewContext;
//             var routeValues = context.RouteData.Values;
//             var currentAction = routeValues["action"].ToString();
//             var currentController = routeValues["controller"].ToString();
//             List<string> lstControllers = new List<string> { "Account", "Manage", "Users", "Privileges", "Engines", "Terminals", "OBalances", "companydetails" };
//             var isController = lstControllers.Find(c => c.Equals(currentController)) != null;
//             var str = String.Format("<li {2}> <a href='javascript:;' class='nav-link nav-toggle'><i class='{0}'></i><span class='title'>{1}</span><span {3}></span><span {4}></span> </a><ul class='sub-menu'>",
//                 icon, text, isController ?
//                 " class=\"nav-item  active open\"" :
//                  " class=\"nav-item\"",
//                 isController ?
//                 "class=\"selected\"" :
//                 string.Empty,
//                 isController ?
//                 "class=\"arrow open\"" :
//                 "class=\"arrow\""
//             );
//             return new MvcHtmlString(str);
//         }
//         public static MvcHtmlString subLiActionLink(this HtmlHelper html, string text, string action, string controller)
//         {
//             var context = html.ViewContext;
//             if (context.Controller.ControllerContext.IsChildAction)
//                 context = html.ViewContext.ParentActionViewContext;
//             var routeValues = context.RouteData.Values;
//             var currentAction = routeValues["action"].ToString();
//             var currentController = routeValues["controller"].ToString();
//
//             var str = String.Format("<li {3}><a href='/{1}/{0}' class='nav-link '><span class='title'>{2}</span><span {4}></span></a></li>",
//                 action, controller, text,
//                 currentController.ToLower().Equals(controller.ToLower(), StringComparison.InvariantCulture) && currentAction.ToLower().Equals(action.ToLower(), StringComparison.InvariantCulture) ?
//                 " class=\"nav-item  active open\"" :
//                 " class=\"nav-item\"",
//                 currentController.ToLower().Equals(controller.ToLower(), StringComparison.InvariantCulture) && currentAction.Equals(action.ToLower(), StringComparison.InvariantCulture) ?
//                 " class=\"selected\"" :
//                 String.Empty
//             );
//             return new MvcHtmlString(str);
//         }
//         public static MvcHtmlString subLiActionLinkLast(this HtmlHelper html, string text, string action, string controller)
//         {
//             var context = html.ViewContext;
//             if (context.Controller.ControllerContext.IsChildAction)
//                 context = html.ViewContext.ParentActionViewContext;
//             var routeValues = context.RouteData.Values;
//             var currentAction = routeValues["action"].ToString();
//             var currentController = routeValues["controller"].ToString();
//
//             var str = String.Format("<li {3}><a href='/{1}/{0}' class='nav-link '><span class='title'>{2}</span><span {4}></span></a></li></ul>",
//                 action, controller, text,
//                 currentController.Equals(controller, StringComparison.InvariantCulture) && currentAction.Equals(action, StringComparison.InvariantCulture) ?
//                 " class=\"nav-item  active open\"" :
//                 " class=\"nav-item\"",
//                 currentController.Equals(controller, StringComparison.InvariantCulture) && currentAction.Equals(action, StringComparison.InvariantCulture) ?
//                 " class=\"selected\"" :
//                 String.Empty
//             );
//             return new MvcHtmlString(str);
//         }
//
//
//
//
//         //*CBS MENUS*//
//         public static MvcHtmlString LiActioncbsClients(this HtmlHelper html, string text, string icon)
//         {
//             var context = html.ViewContext;
//             if (context.Controller.ControllerContext.IsChildAction)
//                 context = html.ViewContext.ParentActionViewContext;
//             var routeValues = context.RouteData.Values;
//             var currentAction = routeValues["action"].ToString();
//             var currentController = routeValues["controller"].ToString();
//             List<string> lstControllers = new List<string> { "cbsClients" };
//             var isController = lstControllers.Find(c => c.ToLower().Equals(currentController.ToLower())) != null;
//             var str = String.Format("<li {2}> <a href='javascript:;' class='nav-link nav-toggle'><i class='{0}'></i><span class='title'>{1}</span><span {3}></span><span {4}></span> </a><ul class='sub-menu'>",
//                 icon, text, isController ?
//                 " class=\"nav-item  active open\"" :
//                  " class=\"nav-item\"",
//                 isController ?
//                 "class=\"selected\"" :
//                 string.Empty,
//                 isController ?
//                 "class=\"arrow open\"" :
//                 "class=\"arrow\""
//             );
//             return new MvcHtmlString(str);
//         }
//     }

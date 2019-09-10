using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using TwittSquare.ASP.Utils.Extensions;

namespace TwittSquare.ASP.Utils {
    public class TwittSquareAuthorizationFilter : IAuthorizationFilter {

        public void OnAuthorization(AuthorizationFilterContext context) {
            var token = context.HttpContext.CheckLogin();

            var action = context.ActionDescriptor as ControllerActionDescriptor;
            if(AcceptAnonymous(action.ControllerTypeInfo) || AcceptAnonymous(action.MethodInfo)) {
                return;
            }

            if((IsApi(action.ControllerTypeInfo) || IsApi(action.MethodInfo)) && token == null) {
                context.Result = new JsonResult(new { error = "not authorization" });
                return;
            }

            if(token == null) {
                context.Result = new RedirectToActionResult("Index","Home",null);
            }
        }

        private bool AcceptAnonymous(ICustomAttributeProvider o) {
            return o?.IsDefined(typeof(AllowAnonymousAttribute),true) ?? false;
        }

        private bool IsApi(ICustomAttributeProvider o) {
            return o?.IsDefined(typeof(ApiAttribute),true) ?? false;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,Inherited = false,AllowMultiple = false)]
    sealed class ApiAttribute : Attribute {

        public ApiAttribute() { }
    }
}

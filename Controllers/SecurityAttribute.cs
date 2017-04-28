using System.Web;
using System.Web.Mvc;

public class SecurityAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorized = base.AuthorizeCore(httpContext);
            if (!authorized) {
                return false;
            }
        
        return true;
        }
    }

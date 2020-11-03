using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auction.Constraints
{
	public class IdConstraint : IRouteConstraint
	{
		public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
		{
            try
            {
                if (System.Convert.ToInt32(values[routeKey]) < 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                return true;
            }

		}
	}
}

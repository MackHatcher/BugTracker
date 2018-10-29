using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlString DisplayDate(this HtmlHelper helper, DateTimeOffset? date)
        {
            var returnData = "";

            if (!date.HasValue)
            {
                returnData = "-";
            }
            else
            {
                returnData = date.Value.ToString("yyyy-MM-dd HH:mm");
            }

            return new HtmlString(returnData);
        }
    }
}

#region Using Namespaces

using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Caching;
using System.Collections.Generic;
using System.Data;
using BrewingCoffee.Helpers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

#endregion

namespace BrewingCoffee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoffeeController : ControllerBase
    {
        #region Internal Members

        private ISysTime _sysTime;
        private IMemoryCache _cache;
        private const string _message            = "Your piping hot coffee is ready";
        private const string _dateFormat_ISO8601 = "yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz";

        #endregion

        #region Constructors

        public CoffeeController(IMemoryCache cache, ISysTime sysTime)
        {
            _cache      = cache;
            _sysTime    = sysTime;
        }

        #endregion

        #region Anonymous Methods

        [HttpGet]
        [Route("~/brew-coffee")]
        public async Task<IActionResult> BrewCoffeeAsync()
        {
            DateTime nowDateTime = _sysTime.GetCurrentTime();

            // Look for cache key.
            if (!_cache.TryGetValue("RequestNo", out int requestNo))
            {
                // Key not in cache, so get data.
                requestNo = 1;

                // Save data in cache.
                _cache.Set("RequestNo", requestNo);
            }
            else
            {
                // Key in cache, increment by one.
                requestNo++;

                // Save data in cache.
                _cache.Set("RequestNo", requestNo);
            }

            // On every fifth call to the endpoint, the endpoint should return 503 Service
            // Unavailable with an empty response body, to signify that the coffee machine is out of coffee.
            if (requestNo % 5 == 0)
                return await Task.Run(() => StatusCode((int)HttpStatusCode.ServiceUnavailable, string.Empty));

            // If the date is April 1st, then all calls to the endpoint should return 418 I’m a teapot instead,
            // with an empty response body, to signify that the endpoint is not brewing coffee today
            if (nowDateTime.Month == 4 && nowDateTime.Day == 1)
                return await Task.Run(() => StatusCode(418, string.Empty));

            return await Task.Run(() => Ok(new
            {
                message = _message,
                prepared = nowDateTime.ToString(_dateFormat_ISO8601)
            }));
        }

        #endregion
    }
}
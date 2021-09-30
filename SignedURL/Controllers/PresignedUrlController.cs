using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SignedURL.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignedURL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PresignedUrlController : ControllerBase
    {
        private readonly ILogger<PresignedUrlController> _logger;
        private readonly AWS _options;

        public PresignedUrlController(ILogger<PresignedUrlController> logger, IOptions<AWS> options)
        {
            _logger = logger;
            _options = new AWS
            {
                region = options.Value.region,
                accessKey = options.Value.accessKey,
                secretKey = options.Value.secretKey
            };
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_options);
        }
    }
}

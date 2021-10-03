using AWSServerless.IServices;
using AWSServerless;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AWSServerless.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PresignedUrlController : ControllerBase
    {
        IPresignedUrlService _presignedUrlService;
        public PresignedUrlController(IPresignedUrlService presignedUrlService)
        {
            _presignedUrlService = presignedUrlService;
        }

        [HttpPost]
        public ActionResult<string> GetPresignedUrl()
        {
            return _presignedUrlService.GenerateUrl();
        }

       
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSServerless.IServices
{
    public interface IPresignedUrlService
    {
        string GenerateUrl();
    }
}

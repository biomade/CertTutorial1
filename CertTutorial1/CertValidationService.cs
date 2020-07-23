using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CertTutorial1
{
    public class CertValidationService
    {
        //3: add certificate validation
        public bool ValidateCertificate(X509Certificate2 clientCertificate, IWebHostEnvironment env)
        {
            //here would be where we check against the thumbprint in the database
            ;
            var cert = new X509Certificate2(Path.Combine(env.ContentRootPath, "root_localhost.pfx"), "1234");
            if (clientCertificate.Thumbprint == cert.Thumbprint)
            {
                return true;
            }

            return false;
        }

    }
}

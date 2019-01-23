/*
Copyright (C) 2019-Present Pivotal Software, Inc. All rights reserved. 

This program and the accompanying materials are made available under the terms of the 
under the Apache License, Version 2.0 (the "License”); you may not use this file except 
in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the 
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
either express or implied. See the License for the specific language governing permissions 
and limitations under the License.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace NET_framework4_6_1_win_valid_certificate.Controllers
{
  public class ValuesController : ApiController
	{
		private CloudFoundryApplicationOptions _appOptions { get; set; }
		private CloudFoundryServicesOptions _serviceOptions { get; set; }
		private static ILogger<HomeController> _logger { get; set; }
		private static X509Certificate2 _rootCert { get; set; }

		public ValuesController()
		{
			_appOptions = ApplicationConfig.CloudFoundryApplication;
			_serviceOptions = ApplicationConfig.CloudFoundryServices;
			_logger = LoggingConfig.LoggerFactory.CreateLogger<HomeController>();

			LoadCertificate();
		}

		private void LoadCertificate()
		{
			string base64String = _serviceOptions.Services["user-provided"]
																.First(q => q.Name.Equals("root-certificate"))
																.Credentials["base64-certificate"].Value;

			byte[] bytes = Convert.FromBase64String(base64String);

			try {
				_rootCert = new X509Certificate2(bytes, "", X509KeyStorageFlags.PersistKeySet);
			} catch (System.Security.Cryptography.CryptographicException ex) {
				//Did the Base64 string get cut off? Or was the header/footer not removed?
				throw new Exception("An error occurred converting to X509Certificate2 object. " + ex.Message);
			}
		}

		// GET api/values
		[HttpGet]
		public IEnumerable<string> Get()
		{
			if (_rootCert == null)
				throw new Exception("No root certificate was provided");

			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://54.236.42.173");
			request.Method = "GET";
			request.ServerCertificateValidationCallback = CertificateValidationCallBack;

			string ret = "";
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
				using (StreamReader sr = new StreamReader(response.GetResponseStream())) {
					ret = sr.ReadToEnd();
				}
			}

			return new string[] { ret };
		}

		private static bool CertificateValidationCallBack(
			object sender,
			System.Security.Cryptography.X509Certificates.X509Certificate certificate,
			System.Security.Cryptography.X509Certificates.X509Chain chain,
			System.Net.Security.SslPolicyErrors sslPolicyErrors)
		{
			// If the certificate is a valid, signed certificate, return true.
			if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None) {
				_logger.LogDebug("======= It's ok");
				return true;
			}

			_logger.LogDebug("======= Compare root certs");
			X509Certificate2 certificate2 = certificate as X509Certificate2 ?? new X509Certificate2(certificate);
			bool isValidRoot = (certificate2.PublicKey.Key == _rootCert.PublicKey.Key);
			_logger.LogDebug("Trusted root certificate: {0}", isValidRoot.ToString());
			return isValidRoot;
		}
		// GET api/values/5
		[HttpGet]
		public string Get(int id)
		{
			return "value";
    }

    // POST api/values
    public void Post([FromBody]string value)
    {
    }

    // PUT api/values/5
    public void Put(int id, [FromBody]string value)
    {
    }

    // DELETE api/values/5
    public void Delete(int id)
    {
    }
  }
}

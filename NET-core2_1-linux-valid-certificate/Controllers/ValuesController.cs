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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Net;

namespace NET_core2_1_linux_valid_certificate.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ValuesController : ControllerBase
	{
		private CloudFoundryApplicationOptions _appOptions { get; set; }
		private CloudFoundryServicesOptions _serviceOptions { get; set; }
		private ILogger _logger { get; set; }
		private X509Certificate2Collection certificates { get; set; }

		public ValuesController(IOptions<CloudFoundryApplicationOptions> appOptions,
												IOptions<CloudFoundryServicesOptions> serviceOptions,
												ILogger<ValuesController> logger)
		{
			_appOptions = appOptions.Value;
			_serviceOptions = serviceOptions.Value;
			_logger = logger;

			LoadCertificate();
		}

		private void LoadCertificate()
		{
			string base64String = _serviceOptions.Services["user-provided"]
																			.First(q => q.Name.Equals("root-certificate"))
																			.Credentials["base64-certificate"].Value;
			X509Certificate2 xcert;
			byte[] bytes = Convert.FromBase64String(base64String);

			try {
				xcert = new X509Certificate2(bytes, "", X509KeyStorageFlags.PersistKeySet);
			} catch (System.Security.Cryptography.CryptographicException ex) {
				//Did the Base64 string get cut off? Or was the header/footer not removed?
				throw new Exception("An error occurred converting to X509Certificate2 object. " + ex.Message);
			}

			using (X509Store certStore = new X509Store(StoreName.Root, StoreLocation.CurrentUser)) {
				certStore.Open(OpenFlags.ReadWrite);
				certStore.Add(xcert);
				certStore.Close();
			}

			return;
		}

		// GET api/values
		/*[HttpGet]
		public ActionResult<string> Get(int id)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://35.170.55.36");
			request.Method = "GET";
			//request.ServerCertificateValidationCallback = CertificateValidationCallBack;

			string ret = "";
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
				using (StreamReader sr = new StreamReader(response.GetResponseStream())) {
					ret = sr.ReadToEnd();
				}
			}

			return ret;
		}*/
		[HttpGet]
		public async Task<ActionResult<string>> Get(int id)
		{
			string ret = "";

			//THIS IS AN EXAMPLE ONLY. USE HTTPFACTORY IN REAL CODE.
			//https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
			//https://github.com/SteeltoeOSS/Samples/blob/1ae78bda3dca13bbab2f7c8a9ff092463807d7ca/Discovery/src/AspDotNetCore/Fortune-Teller-UI/Startup.cs#L29
			using (HttpClient client = new HttpClient())
			{
				// Call asynchronous network methods in a try/catch block to handle exceptions
				try{
					ret = await client.GetStringAsync("https://somewhere");
				}catch (HttpRequestException e){
					_logger.LogError(e, e.Message);
					throw;
				}
			}

			return ret;
		}

		private static bool CertificateValidationCallBack(
		 object sender,
		 System.Security.Cryptography.X509Certificates.X509Certificate certificate,
		 System.Security.Cryptography.X509Certificates.X509Chain chain,
		 System.Net.Security.SslPolicyErrors sslPolicyErrors)
		{
			// If the certificate is a valid, signed certificate, return true.
			if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None) {
				Console.WriteLine("It's ok");
				return true;
			}

			// If there are errors in the certificate chain, look at each error to determine the cause.
			if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0) {
				if (chain != null && chain.ChainStatus != null) {
					foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus) {
						if ((certificate.Subject == certificate.Issuer) &&
							 (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot)) {
							// Self-signed certificates with an untrusted root are valid.
							Console.WriteLine("Untrusted root certificate");
							continue;
						} else {
							if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError) {

								Console.WriteLine("Another error");
								// If there are any other errors in the certificate chain, the certificate is invalid,
								// so the method returns false.
								return false;
							}
						}
					}
				}

				// When processing reaches this line, the only errors in the certificate chain are 
				// untrusted root errors for self-signed certificates. These certificates are valid
				// for default Exchange server installations, so return true.
				Console.WriteLine("Everything seems ok");
				return true;
			} else {
				Console.WriteLine("All other cases");
				// In all other cases, return false.
				return false;
			}
		}
		/*
		 // POST api/values
		 [HttpPost]
		 public void Post([FromBody] string value)
		 {
		 }

		 // PUT api/values/5
		 [HttpPut("{id}")]
		 public void Put(int id, [FromBody] string value)
		 {
		 }

		 // DELETE api/values/5
		 [HttpDelete("{id}")]
		 public void Delete(int id)
		 {
		 }*/
	}
}

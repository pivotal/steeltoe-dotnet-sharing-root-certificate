﻿/*
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Steeltoe.Extensions.Logging;

namespace NET_core2_1_linux_valid_certificate
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseCloudFoundryHosting()
				.AddCloudFoundry()
				.UseStartup<Startup>()
				.ConfigureLogging((builderContext, loggingBuilder) => {
					// Add Steeltoe Dynamic Logging provider
					loggingBuilder.AddDynamicConsole();
				});

	}
}

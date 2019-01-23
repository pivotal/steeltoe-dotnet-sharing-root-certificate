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
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace NET_framework4_6_1_win_valid_certificate
{
  public class WebApiApplication : System.Web.HttpApplication
  {
    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();
      GlobalConfiguration.Configure(WebApiConfig.Register);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);

      // Create applications configuration
      ApplicationConfig.Configure("development");

			// Create logging system using configuration
			LoggingConfig.Configure(ApplicationConfig.Configuration);

			// Add management endpoints to application
			ManagementConfig.ConfigureManagementActuators(
					ApplicationConfig.Configuration,
					LoggingConfig.LoggerProvider,
					GlobalConfiguration.Configuration.Services.GetApiExplorer(),
					LoggingConfig.LoggerFactory);

			// Start the management endpoints
			ManagementConfig.Start();
		}
		protected void Application_Stop()
		{
			ManagementConfig.Stop();
		}
	}
}

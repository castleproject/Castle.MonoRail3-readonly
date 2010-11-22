﻿//  Copyright 2004-2010 Castle Project - http://www.castleproject.org/
//  
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// 
namespace Castle.MonoRail
{
	using System;
	using System.Web;
	using Hosting.Mvc;
	using Hosting.Mvc;
	using Primitives.Mvc;
	using Hosting.Mvc.Typed;
	using Hosting.Mvc.ViewEngines;

    public class ViewResult : ActionResult
	{
		private readonly string viewName;
		private readonly string layout;

		public ViewResult(string viewName, string layout = null, dynamic data = null)
		{
			Data = data;
			this.viewName = viewName;
			this.layout = layout;
		}

		public dynamic Data { get; set; }

		public override void Execute(ActionResultContext context, IMonoRailServices services)
		{
			var viewEngines = services.ViewEngines;

			var result = viewEngines.ResolveView(viewName, layout, new ViewResolutionContext(context));

			if (result.Successful)
			{
				try
				{
					//TODO: needs a better way to resolve the HttpContext
					var httpContext = new HttpContextWrapper(HttpContext.Current);
					var viewContext = new ViewContext(httpContext, httpContext.Response.Output) {Data = Data};

					result.View.Process(viewContext, httpContext.Response.Output);
				}
				finally
				{
					result.ViewEngine.Release(result.View);
				}
			}
			else
			{
				throw new Exception("Could not find view " + viewName +
					". Searched at " + string.Join(", ", result.SearchedLocations));
			}
		}
	}
}
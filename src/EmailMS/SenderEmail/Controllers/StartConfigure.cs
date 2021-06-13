﻿using ConfigureMS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SenderEmail.Controllers
{
    public class StartConfigureController : Controller
    {
        private readonly StartConfigurationMS config;
        private readonly ILogger<StartConfigureController> _logger;

        public StartConfigureController(StartConfigurationMS config, ILogger<StartConfigureController> logger)
        {
            this.config = config;
            _logger = logger;

        }
        public async Task<IActionResult> Index([FromServices]IWebHostEnvironment webHostEnvironment)
        {
            var pluginsFolder = Path.Combine(webHostEnvironment.WebRootPath, "plugins");
            await foreach (var item in config.StartFinding(pluginsFolder))
            {
                ModelState.AddModelError(item.MemberNames.FirstOrDefault() ?? "error", item.ErrorMessage);
            };
            return View();
        }
    }
}
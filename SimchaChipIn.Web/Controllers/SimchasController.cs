using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimchaChipIn.Data;
using SimchaChipIn.Web.Models;

namespace SimchaChipIn.Web.Controllers
{
    public class SimchasController : Controller
    {
        private string _connectionString =
          @"Data Source=.\sqlexpress;Initial Catalog=SimchaChipIn;Integrated Security=true;";
        public IActionResult Index()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            var manager = new SimchaChipInManager(_connectionString);
            var vm = new SimchasIndexViewModel();
            vm.TotalContributorCount = manager.GetContributorCount();
            vm.Simchas = manager.GetSimchas();
            return View(vm);
        }
        [HttpPost]
        public IActionResult NewSimcha(Simcha simcha)
        {
            var manager = new SimchaChipInManager(_connectionString);
            manager.AddSimcha(simcha);
            TempData["Message"] = $"New Simcha Created! Id: {simcha.Id}";
            return RedirectToAction("index");
        }
        public IActionResult Contributions(int simchaId)
        {
            var manager = new SimchaChipInManager(_connectionString);
            var vm = new ContributionsViewModel();
            vm.Simcha = manager.GetSimchaById(simchaId);
            vm.Contributors = manager.GetSimchaContributors(simchaId);
            return View(vm);
        }
        [HttpPost]
        public IActionResult UpdateContributions(IEnumerable<ContributionInclusion> contributors,int simchaId)
        {
            var manager = new SimchaChipInManager(_connectionString);
            manager.UpdateSimchaContributions(simchaId, contributors);
            TempData["Message"] = "Simcha updated successfully";
            return RedirectToAction("index");
        }
    }
}
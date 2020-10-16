using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimchaChipIn.Data;
using SimchaChipIn.Web.Models;

namespace SimchaChipIn.Web.Controllers
{
    public class ContributorsController : Controller
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
            var vm = new ContributorsIndexViewModel();
            vm.Contributors = manager.GetContributors();
            return View(vm);
        }
        [HttpPost]
        public IActionResult New(Contributor contributor, decimal initialDeposit)
        {
            var manager = new SimchaChipInManager(_connectionString);
            manager.AddContributor(contributor);
            var deposit = new Deposit
            {
                Amount = initialDeposit,
                ContributorId = contributor.Id,
                Date = contributor.Date
            };
            manager.AddDeposit(deposit);
            return RedirectToAction("index");
        }
        [HttpPost]
        public IActionResult Edit(Contributor contributor)
        {
            var manager = new SimchaChipInManager(_connectionString);
            manager.UpdateContributor(contributor);
            return RedirectToAction("index");
        }
        public IActionResult ShowHistory(int contributorId)
        {
            var manager = new SimchaChipInManager(_connectionString);
            IEnumerable<Deposit> deposits = manager.GetDepositsById(contributorId);
            IEnumerable<Contribution> contributions = manager.GetContributionsById(contributorId);
            List<Transaction> transactions = new List<Transaction>();
            foreach(Deposit d in deposits)
            {
                transactions.Add(new Transaction
                {
                    Action = "Deposit",
                    Date = d.Date,
                    Amount = d.Amount
                });
            }
            foreach(Contribution c in contributions)
            {
                transactions.Add(new Transaction
                {
                    Action = $"Contribution for the {c.SimchaName} Simcha",
                    Date = c.Date,
                    Amount = c.Amount
                });
            }
             transactions.OrderByDescending(t => t.Date);
            var vm = new ContributorHistoryViewModel();
            vm.Transactions = transactions;
            return View(vm);
        }
        [HttpPost]
        public IActionResult Deposit(Deposit deposit)
        {
            var manager = new SimchaChipInManager(_connectionString);
            manager.AddDeposit(deposit);
            TempData["message"] = "Deposit successfully recorded";
            return RedirectToAction("Index");
        }
    }
}
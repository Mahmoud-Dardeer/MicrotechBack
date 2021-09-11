using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using MicroTechTask.ViewModel;

namespace MicroTechTask.Controllers
{
    [Route("api/accounts")]
    [EnableCors("AllowOrigin")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountsController()
        {
            // its not better to use context here 
            var optionsBuilder = new DbContextOptionsBuilder<TestDevModel>();
            optionsBuilder.UseSqlServer(@"Data Source=.;Initial Catalog=TestDev;Integrated Security=True;Persist Security Info=True");

            var context = new TestDevModel(optionsBuilder.Options);

            _accountRepository = new AccountRepository(context);
        }

        [HttpGet("get-total-balances")]
        public async Task<IActionResult> GetTotalBalances()
        {
            try
            {
                var accounts = await _accountRepository.GetTopLevelAccounts();

                // Its better to do this logic in service layer

                var result = new List<AccountViewModel>();

                foreach (var account in accounts)
                {
                    decimal tempBalance = 0;

                    foreach (var childAccountLevel1 in account.ChildAccounts)
                    {
                        if (childAccountLevel1.Balance.HasValue && childAccountLevel1.Balance.Value > 0)
                        {
                            tempBalance += childAccountLevel1.Balance.Value;
                            continue;
                        }
                        else
                        {
                            foreach (var childAccountLevel2 in childAccountLevel1.ChildAccounts)
                            {
                                if (childAccountLevel2.Balance.HasValue && childAccountLevel2.Balance.Value > 0)
                                {
                                    tempBalance += childAccountLevel2.Balance.Value;
                                    continue;
                                }
                                else
                                {
                                    foreach (var childAccountLevel3 in childAccountLevel2.ChildAccounts)
                                    {
                                        if (childAccountLevel3.Balance.HasValue && childAccountLevel3.Balance.Value > 0)
                                        {
                                            tempBalance += childAccountLevel3.Balance.Value;
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    result.Add(new AccountViewModel()
                    {
                        TopLevelAccount = account.AccNumber,
                        TotalBalance = tempBalance
                    });
                }

                //var result = accounts.Select(a => new AccountViewModel()
                //{
                //    TopLevelAccount = a.AccNumber,
                //    TotalBalance = a.ChildAccounts.Sum(aa => aa.Balance ?? 0 + aa.ChildAccounts.Sum(aaa => aaa.Balance ?? 0 + aaa.ChildAccounts.Sum(aaaa => aaaa.Balance ?? 0)))
                //}).ToList();

                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Get account details to show in front
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        [HttpGet("get-account-details/{accountNumber}")]
        public async Task<IActionResult> GetAccountDetails( [FromRoute] string accountNumber)
        {
            try
            {
                var account = await _accountRepository.GetAccountDetails(accountNumber);

                var result = new List<AccountDetailsViewModel>();


                foreach (var childAccountLevel1 in account.ChildAccounts)
                {
                    var tempListString = new List<string>();

                    tempListString.Add(account.AccNumber);
                    tempListString.Add(childAccountLevel1.AccNumber);

                    var tempAccount = new AccountDetailsViewModel()
                    {
                        AccountNumbers = tempListString,
                    };

                    if (childAccountLevel1.Balance.HasValue && childAccountLevel1.Balance.Value > 0)
                    {
                        tempAccount.Balance = childAccountLevel1.Balance.Value;
                    }
                    else
                    {
                        tempAccount.AccountNumbers.Add(childAccountLevel1.ChildAccounts[0].AccNumber);

                        if (childAccountLevel1.ChildAccounts[0].Balance.HasValue && childAccountLevel1.ChildAccounts[0].Balance.Value > 0)
                        {
                            tempAccount.Balance = childAccountLevel1.ChildAccounts[0].Balance.Value;
                        }
                        else
                        {
                            tempAccount.AccountNumbers.Add(childAccountLevel1.ChildAccounts[0].ChildAccounts[0].AccNumber);

                            if (childAccountLevel1.ChildAccounts[0].ChildAccounts[0].Balance.HasValue && childAccountLevel1.ChildAccounts[0].ChildAccounts[0].Balance.Value > 0)
                            {
                                tempAccount.Balance = childAccountLevel1.ChildAccounts[0].ChildAccounts[0].Balance.Value;
                            }
                        }
                    }

                    result.Add(tempAccount);

                }

                return Ok(result);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Entity Developer tool using the template for generating Repositories and a Unit of Work for EF Core model.
// Code is generated on: 9/10/2021 12:23:33 AM
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
using System;
using System.Linq;
using System.Collections.Generic;

namespace MicroTechTask
{
    public partial class AccountRepository : EntityFrameworkRepository<Account>, IAccountRepository
    {
        public AccountRepository(TestDevModel context) : base(context)
        {
        }

        public virtual ICollection<Account> GetAll()
        {
            return objectSet.ToList();
        }

        public virtual Account GetByKey(string _AccNumber)
        {
            return objectSet.SingleOrDefault(e => e.AccNumber == _AccNumber);
        }

        public new TestDevModel Context 
        {
            get 
            {
                return (TestDevModel)base.Context;
            }
        }
    }
}

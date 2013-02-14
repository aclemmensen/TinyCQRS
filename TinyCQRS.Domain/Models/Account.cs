using System;
using TinyCQRS.Contracts;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Domain.Models
{
	public class AccountCoordinator :
		IHandle<CreateAccount>
	{
		private readonly IRepository<Account> _accounts;

		public AccountCoordinator(IRepository<Account> accounts)
		{
			_accounts = accounts;
		}

		public void Handle(CreateAccount command)
		{
			var account = new Account(command.AggregateId, command.Name);
			_accounts.Save(account);
		}
	}

	public class Account : Saga,
		IApply<AccountCreated>
	{
		private Guid _accountId;

		public Account() { }

		public Account(Guid id, string name)
		{
			ApplyChange(new AccountCreated(id, name));
		}

		public void Apply(AccountCreated @event)
		{
			_id = @event.AggregateId;
			_accountId = @event.AccountId;
		}
	}
	
	public class CreateAccount : Command
	{
		public string Name { get; set; }

		public CreateAccount(Guid accountId, string name) : base(accountId)
		{
			Name = name;
		}
	}

	public class AccountCreated : Event
	{
		public Guid AccountId { get; set; }
		public string Name { get; set; }

		public AccountCreated(Guid accountId, string name) : base(accountId)
		{
			AccountId = accountId;
			Name = name;
		}
	}
}
using System;
using TinyCQRS.Domain.Services;

namespace TinyCQRS.Infrastructure.Services
{
	public class FakeSpellcheckerFactory : ISpellcheckerFactory
	{
		public ISpellchecker CreateFor(string primaryLanguageKey, string secondaryLanguageKey)
		{
			return new FakeSpellchecker();
		}
	}

	public class FakeSpellchecker : ISpellchecker
	{
		public Spellcheck Check(string text)
		{
			var result = new Spellcheck();

			var words = text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var word in words)
			{
				if (word.Length > 12)
				{
					result.Misspellings.Add(word);
				}
				else if (word.Length > 8)
				{
					result.PotentialMisspellings.Add(word);
				}
			}

			return result;
		}
	}
}

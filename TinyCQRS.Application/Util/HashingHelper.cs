using System.Security.Cryptography;
using System.Text;

namespace TinyCQRS.Application.Util
{
	public class HashingHelper
	{
		public static string Hash(string input)
		{
			var algorithm = new SHA256Managed();
			
			var rawBytes = Encoding.UTF8.GetBytes(input);
			var hashedBytes = algorithm.ComputeHash(rawBytes);
			var sb = new StringBuilder(64);

			foreach (var x in hashedBytes)
			{
				sb.AppendFormat("{0:x2}", x);
			}

			return sb.ToString();
		}
	}
}
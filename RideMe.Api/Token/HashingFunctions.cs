using System.Security.Cryptography;
using System.Text;

namespace RideMe.Api.Token
{
	public class HashingFunctions
	{

		// hashing the password using the SHA256
		public string HashPassword(string data)
		{
			byte[] dataBytes = Encoding.UTF8.GetBytes(data);

			string hashedData;

			using (var sha256 = SHA256.Create())
			{
				byte[] hash = sha256.ComputeHash(dataBytes);
				// Convert hash to a string representation (e.g., hexadecimal)
				hashedData = Convert.ToHexString(hash);
			}
			return hashedData;
		}


		// Compare the new hashed data with the hashed password
		public bool verifyPassword(string hashedData, string Data2)
		{
			string hashedData2 = HashPassword(Data2);

			bool areHashesEqual = true;
			for (int i = 0; i < hashedData.Length; i++)
			{
				if (hashedData[i] != hashedData2[i])
				{
					areHashesEqual = false;
					break;
				}
			}
			return areHashesEqual;
		}
	}
}

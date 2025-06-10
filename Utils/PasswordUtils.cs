using System.Linq;
using System.Security.Cryptography;
using System.Text;
// References https://dotnettutorials.net/lesson/how-to-store-password-in-hash-format-in-asp-net-core-web-api/
namespace Inventory_Mgmt_System.Utils
{
    public static class PasswordHasher
    {
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Debug output
                Console.WriteLine($"Salt: {Convert.ToBase64String(passwordSalt)}");
                Console.WriteLine($"Hash: {Convert.ToBase64String(passwordHash)}");
                string hi = Convert.ToBase64String(passwordSalt);
                Console.WriteLine(hi);
            }

        }
        public static bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            // Add null checks
            if (storedHash == null || storedSalt == null)
            {
                Console.WriteLine("Stored hash or salt is null");
                return false;
            }

            try
            {
                // Convert the stored salt and hash from Base64 strings to byte arrays
                byte[] saltBytes = Convert.FromBase64String(storedSalt);
                byte[] hashBytes = Convert.FromBase64String(storedHash);

                using (var hmac = new HMACSHA512(saltBytes))
                {
                    byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                    // Compare the computed hash with the stored hash
                    return computedHash.SequenceEqual(hashBytes);
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid Base64 format in stored hash or salt");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying password hash: {ex.Message}");
                return false;
            }
        }
    }
    }


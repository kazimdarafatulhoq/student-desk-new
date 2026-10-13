using System;
using System.Security.Cryptography;
using System.Text;

namespace StudentManagement.BusinessLayer.Security
{
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));

            var salt = new byte[16];
            RandomNumberGenerator.Fill(salt);
            var saltB64 = Convert.ToBase64String(salt);
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(saltB64 + password));
            return $"SHA256${saltB64}${Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(storedHash) || string.IsNullOrWhiteSpace(password))
                return false;

            var parts = storedHash.Split('$');
            if (parts.Length != 3 || parts[0] != "SHA256")
                return false;

            var salt = parts[1];
            var expected = parts[2];
            using var sha = SHA256.Create();
            var actual = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(salt + password)));
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(actual),
                Encoding.UTF8.GetBytes(expected));
        }
    }

    public static class AgeCalculator
    {
        public static string Format(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var years = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-years)) years--;
            var months = today.Month - dateOfBirth.Month;
            if (today.Day < dateOfBirth.Day) months--;
            if (months < 0) { years--; months += 12; }
            return $"{years} yrs {months} mos";
        }
    }
}

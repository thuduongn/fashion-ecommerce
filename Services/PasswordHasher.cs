namespace fashion.Services
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            // Tạo salt ngẫu nhiên
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Tạo hash của mật khẩu và salt bằng PBKDF2
            byte[] hash = PBKDF2(password, salt, 10000, 256 / 8);

            // Kết hợp salt và hash thành một chuỗi và trả về
            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string hashedPassword, string password)
        {
            // Tách salt và hash từ chuỗi đã hash
            string[] parts = hashedPassword.Split(':');
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] hash = Convert.FromBase64String(parts[1]);

            // Tạo hash mới từ mật khẩu và salt đã lưu, so sánh với hash đã lưu
            byte[] testHash = PBKDF2(password, salt, 10000, hash.Length);
            return SlowEquals(hash, testHash);
        }

        private static byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                return pbkdf2.GetBytes(outputBytes);
            }
        }

        // So sánh hai mảng byte một cách chậm chạp để tránh tấn công dò mật khẩu
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }
    }

}

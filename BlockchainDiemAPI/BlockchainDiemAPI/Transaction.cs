using BlockchainDiemAPI.Cryptography;
using BlockchainDiemAPI.Interfaces;
using System.Text;

namespace BlockchainDiemAPI
{
    public class Transaction : ITransaction
    {
        public string MaSinhVien { get; set; }
        public string MaMonHoc { get; set; }
        public double Diem { get; set; }
        public int LanThi { get; set; }
        public DateTime NgayLuu { get; set; }
        public Transaction()
        {
            
        }
        public Transaction(string maSinhVien, string maMonHoc, double diem, int lanThi, DateTime ngayLuu)
        {
            MaSinhVien = maSinhVien;
            MaMonHoc = maMonHoc;
            Diem = diem;
            LanThi = lanThi;
            NgayLuu = ngayLuu;
        }
        public string CalculateTransactionHash()
        {
            string combined = $"{MaSinhVien}{MaMonHoc}{Diem}{LanThi}{NgayLuu}";
            return Convert.ToBase64String(HashData.ComputeHashSha256(Encoding.UTF8.GetBytes(combined)));
        }
    }
}

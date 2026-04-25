using BlockchainDiemAPI.Cryptography;
using BlockchainDiemAPI.Interfaces;
using System.Text;

namespace BlockchainDiemAPI
{
    public class Transaction : ITransaction
    {
        public string MaSinhVien { get; set; }
        public string HoTen { get; set; }
        public DateTime NgayTotNghiep { get; set; }
        public double DiemTongKet { get; set; }
        public string LoaiTotNghiep { get; set; }
        public string NganhHoc { get; set; }
        public DateTime NgayLuu { get; set; }
        public string StoredHash { get; set; }

        public string Signature { get; set; }

        public Transaction() { }

        public Transaction(string maSinhVien, string hoTen, DateTime ngayTotNghiep,double diemTongKet, string loaiTotNghiep, string nganhHoc)
        {
            MaSinhVien = maSinhVien;
            HoTen = hoTen;
            NgayTotNghiep = ngayTotNghiep;
            DiemTongKet = diemTongKet;
            LoaiTotNghiep = loaiTotNghiep;
            NganhHoc = nganhHoc;
            NgayLuu = DateTime.UtcNow;

            StoredHash = CalculateTransactionHash();
        }

        public string CalculateTransactionHash()
        {
            string combined = $"{MaSinhVien}{HoTen}{NgayTotNghiep:yyyyMMdd}{DiemTongKet}{LoaiTotNghiep}{NganhHoc}";
            return Convert.ToBase64String(HashData.ComputeHashSha256(Encoding.UTF8.GetBytes(combined)));
        }

        public void Sign(DigitalSignature ds)
        {
            byte[] hashBytes = HashData.ComputeHashSha256(Encoding.UTF8.GetBytes(CalculateTransactionHash()));
            Signature = Convert.ToBase64String(ds.SignData(hashBytes));
        }

        public bool VerifySignature(DigitalSignature ds)
        {
            if (string.IsNullOrEmpty(Signature)) return false;
            byte[] hashBytes = HashData.ComputeHashSha256(Encoding.UTF8.GetBytes(CalculateTransactionHash()));
            return ds.VerifySignature(hashBytes, Convert.FromBase64String(Signature));
        }
    }
}
namespace BlockchainDiemAPI.Interfaces
{
    public interface ITransaction
    {
         string MaSinhVien { get; set;}
         string HoTen { get; set; }
         DateTime NgayTotNghiep { get; set; }
         double DiemTongKet { get; set; }
         string LoaiTotNghiep { get; set; }
         string NganhHoc { get; set; }
         DateTime NgayLuu { get; set; }

         string StoredHash { get; set; }

         string Signature { get; set; }

        string CalculateTransactionHash();
    }
}

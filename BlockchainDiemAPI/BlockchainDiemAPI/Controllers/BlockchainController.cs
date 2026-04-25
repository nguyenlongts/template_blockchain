using Microsoft.AspNetCore.Mvc;
using BlockchainDiemAPI.Services;

namespace BlockchainDiemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlockchainController : ControllerBase
    {
        private readonly BlockchainService _svc;
        public BlockchainController(BlockchainService svc) => _svc = svc;

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _svc.GetBlocks().Select(b => new {
                b.BlockNumber,
                b.BlockHash,
                b.PreviousBlockHash,
                b.MerkleRoot,
                CreatedDate = b.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss"),
                TxCount = b.Transaction.Count,
                Transactions = b.Transaction.Select((t, i) => new {
                    Index         = i,
                    t.MaSinhVien,
                    t.HoTen,
                    NgayTotNghiep = t.NgayTotNghiep.ToString("dd/MM/yyyy"),
                    t.DiemTongKet,
                    t.LoaiTotNghiep,
                    t.NganhHoc,
                    NgayLuu       = t.NgayLuu.ToString("dd/MM/yyyy HH:mm:ss"),
                    Hash          = t.CalculateTransactionHash(),
                    t.Signature
                })
            });
            return Ok(result);
        }

        [HttpPost("transaction")]
        public IActionResult Add([FromBody] AddTxRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.MaSinhVien) ||
                string.IsNullOrWhiteSpace(req.HoTen) ||
                string.IsNullOrWhiteSpace(req.LoaiTotNghiep) ||
                string.IsNullOrWhiteSpace(req.NganhHoc))
                return BadRequest(new { message = "Vui lòng nhập đầy đủ thông tin." });

            var txn = new Transaction(
                req.MaSinhVien,
                req.HoTen,
                req.NgayTotNghiep,
                req.DiemTongKet,
                req.LoaiTotNghiep,
                req.NganhHoc
            );

            var r = _svc.AddTransaction(txn);
            return Ok(new {
                r.BlockCreated,
                r.PendingCount,
                message = r.BlockCreated
                    ? "Block mới được tạo!"
                    : $"Đã thêm ({r.PendingCount}/{4})"
            });
        }

        [HttpGet("pending")]
        public IActionResult GetPending()
        {
            var result = _svc.GetPending().Select((t, i) => new {
                Index         = i,
                t.MaSinhVien,
                t.HoTen,
                NgayTotNghiep = t.NgayTotNghiep.ToString("dd/MM/yyyy"),
                t.DiemTongKet,
                t.LoaiTotNghiep,
                t.NganhHoc,
                Hash          = t.CalculateTransactionHash(),
                t.Signature
            });
            return Ok(result);
        }

        [HttpGet("verify")]
        public IActionResult Verify()
        {
            _svc.Verify(out var chainErrors);
            var blocks = _svc.GetBlocks();

            var blockResults = blocks.Select(block => {
                var tamperedIdx = block.GetTamperedTransactions();
                bool blockValid = tamperedIdx.Count == 0;

                return new {
                    blockNumber  = block.BlockNumber,
                    isValid      = blockValid,
                    status       = blockValid ? "Hợp lệ" : "Có transaction bị thay đổi",
                    transactions = block.Transaction.Select((t, i) => new {
                        index         = i,
                        t.MaSinhVien,
                        t.HoTen,
                        t.DiemTongKet,
                        t.LoaiTotNghiep,
                        isValid       = !tamperedIdx.Contains(i),
                        status        = tamperedIdx.Contains(i) ? "Đã bị thay đổi" : "Hợp lệ"
                    })
                };
            }).ToList();

            bool chainValid = chainErrors.Count == 0 && blockResults.All(b => b.isValid);

            return Ok(new {
                isValid = chainValid,
                message = chainValid ? "Blockchain hợp lệ" : "Phát hiện dữ liệu bị thay đổi!",
                details = chainErrors,
                blocks  = blockResults
            });
        }

        [HttpPost("save")]
        public IActionResult Save() { _svc.Save(); return Ok(new { message = "Đã lưu" }); }
    }

    public record AddTxRequest(
        string MaSinhVien,
        string HoTen,
        DateTime NgayTotNghiep,
        double DiemTongKet,
        string LoaiTotNghiep,
        string NganhHoc
    );
}
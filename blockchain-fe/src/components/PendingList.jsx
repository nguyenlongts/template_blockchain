import { useState } from "react";
import { api } from "../api/api";

export default function PendingList() {
  const [txs, setTxs] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const load = async () => {
    setLoading(true);
    setError(null);
    try {
      setTxs(await api.getPending());
    } catch (e) {
      setError("Không thể tải dữ liệu pending.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="card">
      <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: "0.75rem" }}>
        <span className="card-title">Giao dịch chờ xử lý</span>
        <button className="btn" onClick={load} disabled={loading}>
          {loading ? "Đang tải..." : "Tải lại"}
        </button>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      {txs === null && !error && (
        <div className="empty">Nhấn "Tải lại" để xem giao dịch đang chờ</div>
      )}

      {txs !== null && txs.length === 0 && (
        <div className="empty">Không có giao dịch nào đang chờ</div>
      )}

      {txs && txs.length > 0 && (
        <>
          <div className="pending-label">
            <span>Mã SV</span>
            <span>Môn học</span>
            <span>Điểm</span>
            <span>Lần</span>
            <span>Ngày</span>
          </div>
          {txs.map((t, i) => (
            <div key={i} className="pending-row">
              <span>{t.maSinhVien}</span>
              <span>{t.maMonHoc}</span>
              <span className="diem-badge">{t.diem}</span>
              <span>{t.lanThi}</span>
              <span style={{ color: "#9ca3af", fontSize: 12 }}>
                {t.ngayLuu ? new Date(t.ngayLuu).toLocaleString("vi-VN") : "—"}
              </span>
            </div>
          ))}
        </>
      )}
    </div>
  );
}

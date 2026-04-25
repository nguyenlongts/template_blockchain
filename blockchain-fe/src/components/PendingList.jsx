import { useState } from "react";
import { api } from "../api/api";

export default function PendingList() {
  const [txs, setTxs] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [showHash, setShowHash] = useState({});

  const load = async () => {
    setLoading(true);
    setError(null);
    try {
      setTxs(await api.getPending());
    } catch {
      setError("Không thể tải dữ liệu pending.");
    } finally {
      setLoading(false);
    }
  };

  const toggleHash = (i) => setShowHash((h) => ({ ...h, [i]: !h[i] }));

  return (
    <div className="card">
      <div
        style={{
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          marginBottom: "0.75rem",
        }}
      >
        <span className="card-title">
          Transaction chờ lưu
          {txs && (
            <span
              style={{
                fontSize: 12,
                fontWeight: 400,
                color: "#6b7280",
                marginLeft: 8,
              }}
            >
              ({txs.length}/4)
            </span>
          )}
        </span>
        <button className="btn" onClick={load} disabled={loading}>
          {loading ? "Đang tải..." : "Tải lại"}
        </button>
      </div>

      {error && <div className="alert alert-error">{error}</div>}
      {txs === null && !error && (
        <div className="empty">Nhấn "Tải lại" để xem transaction đang chờ</div>
      )}
      {txs?.length === 0 && (
        <div className="empty">Không có transaction nào đang chờ</div>
      )}

      {txs &&
        txs.length > 0 &&
        txs.map((t, i) => (
          <div
            key={i}
            style={{
              background: "#f9fafb",
              borderRadius: 8,
              padding: "10px 12px",
              marginBottom: 8,
              border: "1px solid #e5e7eb",
            }}
          >
            <div
              style={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
              }}
            >
              <div>
                <span style={{ fontWeight: 500, fontSize: 13 }}>
                  {t.maSinhVien}
                </span>
                <span style={{ color: "#6b7280", fontSize: 12, marginLeft: 8 }}>
                  {t.hoTen}
                </span>
                <span style={{ marginLeft: 8 }} className="diem-badge">
                  {t.diemTongKet}
                </span>
                <span style={{ color: "#6b7280", fontSize: 12, marginLeft: 8 }}>
                  {t.loaiTotNghiep} · {t.nganhHoc}
                </span>
              </div>
              <button
                className="btn"
                style={{ fontSize: 11 }}
                onClick={() => toggleHash(i)}
              >
                {showHash[i] ? "Ẩn ▲" : "Hash ▼"}
              </button>
            </div>
            {showHash[i] && (
              <div
                style={{
                  marginTop: 8,
                  fontSize: 11,
                  fontFamily: "monospace",
                  wordBreak: "break-all",
                  color: "#1d4ed8",
                }}
              >
                {t.hash}
              </div>
            )}
          </div>
        ))}
    </div>
  );
}

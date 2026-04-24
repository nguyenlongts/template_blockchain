import { useState } from "react";
import { api } from "../api/api";

export default function VerifyPanel() {
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);

  const verify = async () => {
    setLoading(true);
    setResult(null);
    try {
      setResult(await api.verify());
    } catch (e) {
      setResult({ isValid: false, message: "Lỗi kết nối server." });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="card">
      <span className="card-title">Xác minh chuỗi</span>
      <p style={{ color: "#6b7280", fontSize: 13, margin: "0.5rem 0 1rem" }}>
        Kiểm tra tính toàn vẹn của toàn bộ blockchain.
      </p>
      <button className="btn-primary" onClick={verify} disabled={loading}>
        {loading ? "Đang xác minh..." : "Xác minh ngay"}
      </button>

      {result && (
        <div className={`verify-result ${result.isValid !== false ? "verify-ok" : "verify-fail"}`}>
          <span>{result.isValid !== false ? "✓" : "✗"}</span>
          <span>{result.message}</span>
        </div>
      )}
    </div>
  );
}

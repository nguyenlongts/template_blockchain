import { useState } from "react";
import { api } from "../api/api";
import Alert from "./Alert";

const LOA_TOT_NGHIEP = ["Xuất sắc", "Giỏi", "Khá", "Trung bình"];

export default function AddTransactionForm({ onAdded }) {
  const [form, setForm] = useState({
    MaSinhVien: "",
    HoTen: "",
    NgayTotNghiep: "",
    DiemTongKet: "",
    LoaiTotNghiep: "Khá",
    NganhHoc: "",
  });
  const [msg, setMsg] = useState(null);
  const [loading, setLoading] = useState(false);

  const set = (k, v) => setForm((f) => ({ ...f, [k]: v }));

  const handleSubmit = async () => {
    if (
      !form.MaSinhVien ||
      !form.HoTen ||
      !form.NgayTotNghiep ||
      form.DiemTongKet === "" ||
      !form.NganhHoc
    ) {
      setMsg({ text: "Vui lòng nhập đầy đủ thông tin.", type: "error" });
      return;
    }
    if (parseFloat(form.DiemTongKet) < 0 || parseFloat(form.DiemTongKet) > 10) {
      setMsg({ text: "Điểm tổng kết phải từ 0 đến 10.", type: "error" });
      return;
    }

    setLoading(true);
    try {
      const data = await api.addTransaction({
        ...form,
        DiemTongKet: parseFloat(form.DiemTongKet),
        NgayTotNghiep: new Date(form.NgayTotNghiep).toISOString(),
      });
      setMsg({ text: data.message, type: "success" });
      setForm({
        MaSinhVien: "",
        HoTen: "",
        NgayTotNghiep: "",
        DiemTongKet: "",
        LoaiTotNghiep: "Khá",
        NganhHoc: "",
      });
      onAdded();
    } catch (e) {
      setMsg({ text: e.message, type: "error" });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="card">
      <span className="card-title">Thêm thông tin tốt nghiệp</span>

      <div className="form-grid">
        <div>
          <label>Mã sinh viên</label>
          <input
            className="input"
            placeholder="VD: SV001"
            value={form.MaSinhVien}
            onChange={(e) => set("MaSinhVien", e.target.value)}
          />
        </div>
        <div>
          <label>Họ tên sinh viên</label>
          <input
            className="input"
            placeholder="VD: Nguyễn Văn A"
            value={form.HoTen}
            onChange={(e) => set("HoTen", e.target.value)}
          />
        </div>
        <div>
          <label>Ngành học</label>
          <input
            className="input"
            placeholder="VD: Công nghệ thông tin"
            value={form.NganhHoc}
            onChange={(e) => set("NganhHoc", e.target.value)}
          />
        </div>
        <div>
          <label>Ngày tốt nghiệp</label>
          <input
            className="input"
            type="date"
            value={form.NgayTotNghiep}
            onChange={(e) => set("NgayTotNghiep", e.target.value)}
          />
        </div>
        <div>
          <label>Điểm tổng kết (0–10)</label>
          <input
            className="input"
            type="number"
            min="0"
            max="10"
            step="0.01"
            placeholder="VD: 8.5"
            value={form.DiemTongKet}
            onChange={(e) => set("DiemTongKet", e.target.value)}
          />
        </div>
        <div>
          <label>Loại tốt nghiệp</label>
          <select
            className="input"
            value={form.LoaiTotNghiep}
            onChange={(e) => set("LoaiTotNghiep", e.target.value)}
          >
            {LOA_TOT_NGHIEP.map((l) => (
              <option key={l}>{l}</option>
            ))}
          </select>
        </div>
      </div>

      <button className="btn-primary" onClick={handleSubmit} disabled={loading}>
        {loading ? "Đang xử lý..." : "Thêm transaction"}
      </button>

      {msg && <Alert msg={msg.text} type={msg.type} />}
    </div>
  );
}

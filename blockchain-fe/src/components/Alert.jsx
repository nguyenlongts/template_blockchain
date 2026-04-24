export default function Alert({ msg, type }) {
  if (!msg) return null;
  return (
    <div className={`alert ${type === "success" ? "alert-success" : "alert-error"}`}>
      {msg}
    </div>
  );
}

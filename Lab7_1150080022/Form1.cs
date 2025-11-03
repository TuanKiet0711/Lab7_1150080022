using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

public class Form1 : Form
{
    private const string CONN_STR =
     @"Data Source=(LocalDB)\MSSQLLocalDB;
      AttachDbFilename=E:\TaiLieuNopBai\Lab7_1150080022\Lab7_1150080022\QLBanHang.mdf; 
      Integrated Security=True;
      Connect Timeout=30";


    private TextBox txtMa, txtTen, txtDiaChi;
    private Button btnHienThi, btnThem, btnSua, btnXoa;
    private DataGridView dgv;

    public Form1()
    {
        Text = "NXB - Hiển thị/Thêm/Sửa/Xóa";
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(720, 480);

        var lblMa = new Label { Text = "MaXB", Location = new Point(20, 20), AutoSize = true };
        var lblTen = new Label { Text = "TenNXB", Location = new Point(20, 55), AutoSize = true };
        var lblDc = new Label { Text = "DiaChi", Location = new Point(20, 90), AutoSize = true };

        txtMa = new TextBox { Location = new Point(90, 16), Width = 220, MaxLength = 10 };
        txtTen = new TextBox { Location = new Point(90, 51), Width = 220, MaxLength = 100 };
        txtDiaChi = new TextBox { Location = new Point(90, 86), Width = 400, MaxLength = 500 };

        btnHienThi = new Button { Text = "Hiển thị", Location = new Point(520, 15), Width = 80 };
        btnHienThi.Click += (s, e) => LoadData();

        btnThem = new Button { Text = "Thêm", Location = new Point(520, 50), Width = 80 };
        btnThem.Click += (s, e) => Them();

        btnSua = new Button { Text = "Sửa", Location = new Point(520, 85), Width = 80 };
        btnSua.Click += (s, e) => Sua();

        btnXoa = new Button { Text = "Xóa", Location = new Point(610, 85), Width = 70 };
        btnXoa.Click += (s, e) => Xoa();

        dgv = new DataGridView
        {
            Location = new Point(20, 130),
            Size = new Size(660, 300),
            ReadOnly = true,
            AllowUserToAddRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };
        dgv.CellClick += (s, e) =>
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgv.Rows.Count)
            {
                var r = dgv.Rows[e.RowIndex];
                txtMa.Text = (r.Cells["MaXB"].Value?.ToString() ?? "").Trim();
                txtTen.Text = r.Cells["TenNXB"].Value?.ToString() ?? "";
                txtDiaChi.Text = r.Cells["DiaChi"].Value?.ToString() ?? "";
            }
        };

        Controls.AddRange(new Control[] { lblMa, lblTen, lblDc, txtMa, txtTen, txtDiaChi, btnHienThi, btnThem, btnSua, btnXoa, dgv });
        LoadData();
    }

    private void LoadData()
    {
        using (var conn = new SqlConnection(CONN_STR))
        using (var da = new SqlDataAdapter("SELECT MaXB, TenNXB, DiaChi FROM NhaXuatBan ORDER BY MaXB", conn))
        {
            var tbl = new DataTable();
            try { da.Fill(tbl); dgv.DataSource = tbl; }
            catch (Exception ex) { MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message); }
        }
    }

    private bool KiemTraNhap(bool forEdit = false)
    {
        if (string.IsNullOrWhiteSpace(txtMa.Text)) { MessageBox.Show("MaXB không được trống"); txtMa.Focus(); return false; }
        if (string.IsNullOrWhiteSpace(txtTen.Text)) { MessageBox.Show("TenNXB không được trống"); txtTen.Focus(); return false; }
        if (string.IsNullOrWhiteSpace(txtDiaChi.Text)) { MessageBox.Show("DiaChi không được trống"); txtDiaChi.Focus(); return false; }
        return true;
    }

    private void Them()
    {
        if (!KiemTraNhap()) return;
        using (var conn = new SqlConnection(CONN_STR))
        using (var cmd = new SqlCommand("INSERT INTO NhaXuatBan (MaXB, TenNXB, DiaChi) VALUES (@MaXB, @TenNXB, @DiaChi)", conn))
        {
            cmd.Parameters.Add("@MaXB", SqlDbType.Char, 10).Value = txtMa.Text.Trim();
            cmd.Parameters.Add("@TenNXB", SqlDbType.NVarChar, 100).Value = txtTen.Text.Trim();
            cmd.Parameters.Add("@DiaChi", SqlDbType.NVarChar, 500).Value = txtDiaChi.Text.Trim();
            try { conn.Open(); cmd.ExecuteNonQuery(); MessageBox.Show("Thêm thành công"); LoadData(); ClearInputs(); }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) { MessageBox.Show("MaXB đã tồn tại"); }
            catch (Exception ex) { MessageBox.Show("Lỗi thêm: " + ex.Message); }
        }
    }

    private void Sua()
    {
        if (!KiemTraNhap(forEdit: true)) return;
        using (var conn = new SqlConnection(CONN_STR))
        using (var cmd = new SqlCommand("UPDATE NhaXuatBan SET TenNXB=@TenNXB, DiaChi=@DiaChi WHERE MaXB=@MaXB", conn))
        {
            cmd.Parameters.Add("@TenNXB", SqlDbType.NVarChar, 100).Value = txtTen.Text.Trim();
            cmd.Parameters.Add("@DiaChi", SqlDbType.NVarChar, 500).Value = txtDiaChi.Text.Trim();
            cmd.Parameters.Add("@MaXB", SqlDbType.Char, 10).Value = txtMa.Text.Trim();
            try { conn.Open(); var n = cmd.ExecuteNonQuery(); if (n > 0) { MessageBox.Show("Sửa thành công"); LoadData(); } else MessageBox.Show("Không tìm thấy MaXB để sửa"); }
            catch (Exception ex) { MessageBox.Show("Lỗi sửa: " + ex.Message); }
        }
    }

    private void Xoa()
    {
        if (string.IsNullOrWhiteSpace(txtMa.Text)) { MessageBox.Show("Chọn MaXB cần xóa"); return; }
        if (MessageBox.Show("Xóa bản ghi này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;
        using (var conn = new SqlConnection(CONN_STR))
        using (var cmd = new SqlCommand("DELETE FROM NhaXuatBan WHERE MaXB=@MaXB", conn))
        {
            cmd.Parameters.Add("@MaXB", SqlDbType.Char, 10).Value = txtMa.Text.Trim();
            try
            {
                conn.Open();
                var n = cmd.ExecuteNonQuery();
                if (n > 0) { MessageBox.Show("Xóa thành công"); LoadData(); ClearInputs(); }
                else MessageBox.Show("Không tìm thấy MaXB để xóa");
            }
            catch (SqlException ex) when (ex.Number == 547) { MessageBox.Show("Không thể xóa: MaXB đang được dùng ở bảng Sach."); }
            catch (Exception ex) { MessageBox.Show("Lỗi xóa: " + ex.Message); }
        }
    }

    private void ClearInputs()
    {
        txtMa.Text = "";
        txtTen.Text = "";
        txtDiaChi.Text = "";
        txtMa.Focus();
    }
}

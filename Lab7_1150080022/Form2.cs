using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

public class Form2 : Form
{
    private const string CONN_STR =
        @"Data Source=(LocalDB)\MSSQLLocalDB;
          AttachDbFilename=|DataDirectory|\QLBanHang.mdf;
          Integrated Security=True;
          Connect Timeout=30";

    private TabControl tabs;
    private TextBox mhMa, mhTen, mhDonVi, mhDonGia, mhGhiChu, mhSearch;
    private DateTimePicker mhNSX, mhNHH;
    private Button mhShow, mhAdd, mhEdit, mhDel;
    private DataGridView mhGrid;

    private TextBox nccMa, nccTen, nccDiaChi, nccMST, nccTK, nccDT, nccSearch;
    private Button nccShow, nccAdd, nccEdit, nccDel;
    private DataGridView nccGrid;

    private TextBox hnMaSP, hnMaNCC, hnSoHD, hnSoLuong, hnDonGia, hnSearch;
    private DateTimePicker hnNgayGH;
    private Button hnShow, hnAdd, hnEdit, hnDel;
    private DataGridView hnGrid;

    public Form2()
    {
        AppDomain.CurrentDomain.SetData("DataDirectory", Application.StartupPath);
        Text = "Quản lý QLBanHang";
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(1040, 640);
        BackColor = Color.White;
        Font = new Font("Segoe UI", 10);
        tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(BuildMatHangTab());
        tabs.TabPages.Add(BuildNhaCCTab());
        tabs.TabPages.Add(BuildHangNhapTab());
        Controls.Add(tabs);
        LoadMatHang();
        LoadNhaCC();
        LoadHangNhap();
    }

    private TabPage BuildMatHangTab()
    {
        var page = new TabPage("Mặt hàng");
        var top = new Panel { Dock = DockStyle.Top, Height = 160, Padding = new Padding(16) };
        var gridWrap = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16, 0, 16, 16) };
        page.Controls.Add(gridWrap);
        page.Controls.Add(top);

        var lblMa = new Label { Text = "MaSP", AutoSize = true, Location = new Point(0, 8) };
        var lblTen = new Label { Text = "TenSP", AutoSize = true, Location = new Point(0, 44) };
        var lblNSX = new Label { Text = "NgaySX", AutoSize = true, Location = new Point(0, 80) };
        var lblNHH = new Label { Text = "NgayHH", AutoSize = true, Location = new Point(260, 80) };
        var lblDonVi = new Label { Text = "DonVi", AutoSize = true, Location = new Point(520, 8) };
        var lblDonGia = new Label { Text = "DonGia", AutoSize = true, Location = new Point(520, 44) };
        var lblGhiChu = new Label { Text = "GhiChu", AutoSize = true, Location = new Point(520, 80) };

        mhMa = new TextBox { Location = new Point(70, 4), Width = 170, MaxLength = 5 };
        mhTen = new TextBox { Location = new Point(70, 40), Width = 420, MaxLength = 30 };
        mhNSX = new DateTimePicker { Location = new Point(70, 76), Width = 170, Format = DateTimePickerFormat.Short };
        mhNHH = new DateTimePicker { Location = new Point(330, 76), Width = 160, Format = DateTimePickerFormat.Short };
        mhDonVi = new TextBox { Location = new Point(590, 4), Width = 170, MaxLength = 10 };
        mhDonGia = new TextBox { Location = new Point(590, 40), Width = 170 };
        mhGhiChu = new TextBox { Location = new Point(590, 76), Width = 300, MaxLength = 200 };

        mhShow = NiceBtn("Hiển thị", new Point(910, 4), (s, e) => LoadMatHang());
        mhAdd = NiceBtn("Thêm", new Point(910, 40), (s, e) => MH_Add());
        mhEdit = NiceBtn("Sửa", new Point(910, 76), (s, e) => MH_Edit());
        mhDel = NiceBtn("Xóa", new Point(910, 112), (s, e) => MH_Del());

        var lblSearch = new Label { Text = "Tìm kiếm", AutoSize = true, Location = new Point(0, 116) };
        mhSearch = new TextBox { Location = new Point(70, 112), Width = 820 };
        mhSearch.TextChanged += (s, e) => FilterGrid(mhGrid, mhSearch.Text, new[] { "MaSP", "TenSP", "DonVi", "GhiChu" });

        top.Controls.AddRange(new Control[] { lblMa, lblTen, lblNSX, lblNHH, lblDonVi, lblDonGia, lblGhiChu,
            mhMa, mhTen, mhNSX, mhNHH, mhDonVi, mhDonGia, mhGhiChu, mhShow, mhAdd, mhEdit, mhDel, lblSearch, mhSearch });

        mhGrid = MakeGrid();
        mhGrid.CellClick += (s, e) =>
        {
            if (e.RowIndex < 0 || e.RowIndex >= mhGrid.Rows.Count) return;
            var r = mhGrid.Rows[e.RowIndex];
            mhMa.Text = (r.Cells["MaSP"].Value?.ToString() ?? "").Trim();
            mhTen.Text = r.Cells["TenSP"].Value?.ToString() ?? "";
            DateTime d1; DateTime.TryParse(r.Cells["NgaySX"].Value?.ToString(), out d1);
            DateTime d2; DateTime.TryParse(r.Cells["NgayHH"].Value?.ToString(), out d2);
            mhNSX.Value = d1 == default(DateTime) ? DateTime.Today : d1;
            mhNHH.Value = d2 == default(DateTime) ? DateTime.Today : d2;
            mhDonVi.Text = r.Cells["DonVi"].Value?.ToString() ?? "";
            mhDonGia.Text = r.Cells["DonGia"].Value?.ToString() ?? "";
            mhGhiChu.Text = r.Cells["GhiChu"].Value?.ToString() ?? "";
        };
        gridWrap.Controls.Add(mhGrid);
        return page;
    }

    private TabPage BuildNhaCCTab()
    {
        var page = new TabPage("Nhà cung cấp");
        var top = new Panel { Dock = DockStyle.Top, Height = 160, Padding = new Padding(16) };
        var gridWrap = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16, 0, 16, 16) };
        page.Controls.Add(gridWrap);
        page.Controls.Add(top);

        var lblMa = new Label { Text = "MaNhaCC", AutoSize = true, Location = new Point(0, 8) };
        var lblTen = new Label { Text = "TenNhaCC", AutoSize = true, Location = new Point(0, 44) };
        var lblDiaChi = new Label { Text = "DiaChi", AutoSize = true, Location = new Point(0, 80) };
        var lblMST = new Label { Text = "MaSoThue", AutoSize = true, Location = new Point(520, 8) };
        var lblTK = new Label { Text = "TaiKhoan", AutoSize = true, Location = new Point(520, 44) };
        var lblDT = new Label { Text = "DienThoai", AutoSize = true, Location = new Point(520, 80) };

        nccMa = new TextBox { Location = new Point(100, 4), Width = 180, MaxLength = 5 };
        nccTen = new TextBox { Location = new Point(100, 40), Width = 390, MaxLength = 50 };
        nccDiaChi = new TextBox { Location = new Point(100, 76), Width = 390, MaxLength = 200 };
        nccMST = new TextBox { Location = new Point(600, 4), Width = 170, MaxLength = 15 };
        nccTK = new TextBox { Location = new Point(600, 40), Width = 170, MaxLength = 15 };
        nccDT = new TextBox { Location = new Point(600, 76), Width = 170, MaxLength = 11 };

        nccShow = NiceBtn("Hiển thị", new Point(910, 4), (s, e) => LoadNhaCC());
        nccAdd = NiceBtn("Thêm", new Point(910, 40), (s, e) => NCC_Add());
        nccEdit = NiceBtn("Sửa", new Point(910, 76), (s, e) => NCC_Edit());
        nccDel = NiceBtn("Xóa", new Point(910, 112), (s, e) => NCC_Del());

        var lblSearch = new Label { Text = "Tìm kiếm", AutoSize = true, Location = new Point(0, 116) };
        nccSearch = new TextBox { Location = new Point(100, 112), Width = 790 };
        nccSearch.TextChanged += (s, e) => FilterGrid(nccGrid, nccSearch.Text, new[] { "MaNhaCC", "TenNhaCC", "DiaChi", "MaSoThue", "TaiKhoan", "DienThoai" });

        top.Controls.AddRange(new Control[] { lblMa, lblTen, lblDiaChi, lblMST, lblTK, lblDT,
            nccMa, nccTen, nccDiaChi, nccMST, nccTK, nccDT, nccShow, nccAdd, nccEdit, nccDel, lblSearch, nccSearch });

        nccGrid = MakeGrid();
        nccGrid.CellClick += (s, e) =>
        {
            if (e.RowIndex < 0 || e.RowIndex >= nccGrid.Rows.Count) return;
            var r = nccGrid.Rows[e.RowIndex];
            nccMa.Text = (r.Cells["MaNhaCC"].Value?.ToString() ?? "").Trim();
            nccTen.Text = r.Cells["TenNhaCC"].Value?.ToString() ?? "";
            nccDiaChi.Text = r.Cells["DiaChi"].Value?.ToString() ?? "";
            nccMST.Text = r.Cells["MaSoThue"].Value?.ToString() ?? "";
            nccTK.Text = r.Cells["TaiKhoan"].Value?.ToString() ?? "";
            nccDT.Text = r.Cells["DienThoai"].Value?.ToString() ?? "";
        };
        gridWrap.Controls.Add(nccGrid);
        return page;
    }

    private TabPage BuildHangNhapTab()
    {
        var page = new TabPage("Hàng nhập");
        var top = new Panel { Dock = DockStyle.Top, Height = 200, Padding = new Padding(16) };
        var gridWrap = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16, 0, 16, 16) };
        page.Controls.Add(gridWrap);
        page.Controls.Add(top);

        var lblMaSP = new Label { Text = "MaSP", AutoSize = true, Location = new Point(0, 8) };
        var lblMaNCC = new Label { Text = "MaNhaCC", AutoSize = true, Location = new Point(0, 44) };
        var lblSoHD = new Label { Text = "SoHD", AutoSize = true, Location = new Point(0, 80) };
        var lblSL = new Label { Text = "SoLuong", AutoSize = true, Location = new Point(280, 8) };
        var lblDG = new Label { Text = "DonGia", AutoSize = true, Location = new Point(280, 44) };
        var lblNGH = new Label { Text = "NgayGH", AutoSize = true, Location = new Point(280, 80) };

        hnMaSP = new TextBox { Location = new Point(80, 4), Width = 170, MaxLength = 5 };
        hnMaNCC = new TextBox { Location = new Point(80, 40), Width = 170, MaxLength = 5 };
        hnSoHD = new TextBox { Location = new Point(80, 76), Width = 170, MaxLength = 10 };
        hnSoLuong = new TextBox { Location = new Point(360, 4), Width = 140 };
        hnDonGia = new TextBox { Location = new Point(360, 40), Width = 140 };
        hnNgayGH = new DateTimePicker { Location = new Point(360, 76), Width = 160, Format = DateTimePickerFormat.Short };

        hnShow = NiceBtn("Hiển thị", new Point(910, 4), (s, e) => LoadHangNhap());
        hnAdd = NiceBtn("Thêm", new Point(910, 40), (s, e) => HN_Add());
        hnEdit = NiceBtn("Sửa", new Point(910, 76), (s, e) => HN_Edit());
        hnDel = NiceBtn("Xóa", new Point(910, 112), (s, e) => HN_Del());

        var lblSearch = new Label { Text = "Tìm kiếm", AutoSize = true, Location = new Point(0, 120) };
        hnSearch = new TextBox { Location = new Point(80, 116), Width = 820 };
        hnSearch.TextChanged += (s, e) => FilterGrid(hnGrid, hnSearch.Text, new[] { "MaSP", "MaNhaCC", "SoHD" });

        top.Controls.AddRange(new Control[] { lblMaSP, lblMaNCC, lblSoHD, lblSL, lblDG, lblNGH,
            hnMaSP, hnMaNCC, hnSoHD, hnSoLuong, hnDonGia, hnNgayGH, hnShow, hnAdd, hnEdit, hnDel, lblSearch, hnSearch });

        hnGrid = MakeGrid();
        hnGrid.CellClick += (s, e) =>
        {
            if (e.RowIndex < 0 || e.RowIndex >= hnGrid.Rows.Count) return;
            var r = hnGrid.Rows[e.RowIndex];
            hnMaSP.Text = (r.Cells["MaSP"].Value?.ToString() ?? "").Trim();
            hnMaNCC.Text = (r.Cells["MaNhaCC"].Value?.ToString() ?? "").Trim();
            hnSoHD.Text = r.Cells["SoHD"].Value?.ToString() ?? "";
            hnSoLuong.Text = r.Cells["SoLuong"].Value?.ToString() ?? "";
            hnDonGia.Text = r.Cells["DonGia"].Value?.ToString() ?? "";
            DateTime d; DateTime.TryParse(r.Cells["NgayGH"].Value?.ToString(), out d);
            hnNgayGH.Value = d == default(DateTime) ? DateTime.Today : d;
        };
        gridWrap.Controls.Add(hnGrid);
        return page;
    }

    private DataGridView MakeGrid()
    {
        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            RowHeadersVisible = false,
            EnableHeadersVisualStyles = false,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            GridColor = Color.FromArgb(255, 224, 178)
        };
        grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.FromArgb(255, 145, 0),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        grid.DefaultCellStyle = new DataGridViewCellStyle
        {
            Font = new Font("Segoe UI", 10),
            SelectionBackColor = Color.FromArgb(255, 183, 77),
            SelectionForeColor = Color.Black
        };
        grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.FromArgb(255, 248, 240)
        };
        return grid;
    }

    private Button NiceBtn(string text, Point location, EventHandler onClick)
    {
        var b = new Button
        {
            Text = text,
            Location = location,
            Width = 100,
            Height = 30,
            BackColor = Color.FromArgb(35, 110, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        b.FlatAppearance.BorderSize = 0;
        b.Click += onClick;
        return b;
    }

    private void FilterGrid(DataGridView grid, string keyword, string[] cols)
    {
        if (grid != null && grid.DataSource is DataTable)
        {
            var dt = (DataTable)grid.DataSource;
            var k = (keyword ?? "").Replace("'", "''");
            if (string.IsNullOrWhiteSpace(k)) { dt.DefaultView.RowFilter = ""; return; }
            var parts = new System.Collections.Generic.List<string>();
            foreach (var c in cols) parts.Add(string.Format("CONVERT([{0}], System.String) LIKE '%{1}%'", c, k));
            dt.DefaultView.RowFilter = string.Join(" OR ", parts.ToArray());
        }
    }

    private void LoadMatHang()
    {
        using (var conn = new SqlConnection(CONN_STR))
        using (var da = new SqlDataAdapter("SELECT MaSP, TenSP, NgaySX, NgayHH, DonVi, DonGia, GhiChu FROM tblMatHang ORDER BY MaSP", conn))
        {
            var tbl = new DataTable();
            try { da.Fill(tbl); mhGrid.DataSource = tbl; }
            catch (Exception ex) { MessageBox.Show("Lỗi tải Mặt hàng: " + ex.Message); }
        }
    }

    private void LoadNhaCC()
    {
        using (var conn = new SqlConnection(CONN_STR))
        using (var da = new SqlDataAdapter("SELECT MaNhaCC, TenNhaCC, DiaChi, MaSoThue, TaiKhoan, DienThoai FROM tblNhaCC ORDER BY MaNhaCC", conn))
        {
            var tbl = new DataTable();
            try { da.Fill(tbl); nccGrid.DataSource = tbl; }
            catch (Exception ex) { MessageBox.Show("Lỗi tải Nhà cung cấp: " + ex.Message); }
        }
    }

    private void LoadHangNhap()
    {
        using (var conn = new SqlConnection(CONN_STR))
        using (var da = new SqlDataAdapter("SELECT MaSP, MaNhaCC, SoLuong, DonGia, SoHD, NgayGH FROM tblHangNhap ORDER BY NgayGH DESC, SoHD", conn))
        {
            var tbl = new DataTable();
            try { da.Fill(tbl); hnGrid.DataSource = tbl; }
            catch (Exception ex) { MessageBox.Show("Lỗi tải Hàng nhập: " + ex.Message); }
        }
    }

    private bool TryParseDouble(string s, out double v) { return double.TryParse((s ?? "").Replace(",", "").Trim(), out v); }
    private bool TryParseInt(string s, out int v) { return int.TryParse((s ?? "").Trim(), out v); }

    private void MH_Add()
    {
        if (string.IsNullOrWhiteSpace(mhMa.Text)) { MessageBox.Show("MaSP không được trống"); mhMa.Focus(); return; }
        if (string.IsNullOrWhiteSpace(mhTen.Text)) { MessageBox.Show("TenSP không được trống"); mhTen.Focus(); return; }
        double donGia; if (!TryParseDouble(mhDonGia.Text, out donGia)) { MessageBox.Show("DonGia không hợp lệ"); mhDonGia.Focus(); return; }

        using (var conn = new SqlConnection(CONN_STR))
        using (var cmd = new SqlCommand("sp_ThemMatHang", conn) { CommandType = CommandType.StoredProcedure })
        {
            cmd.Parameters.Add("@MaSP", SqlDbType.NChar, 5).Value = mhMa.Text.Trim();
            cmd.Parameters.Add("@TenSP", SqlDbType.NVarChar, 30).Value = mhTen.Text.Trim();
            cmd.Parameters.Add("@NgaySX", SqlDbType.Date).Value = mhNSX.Value.Date;
            cmd.Parameters.Add("@NgayHH", SqlDbType.Date).Value = mhNHH.Value.Date;
            cmd.Parameters.Add("@DonVi", SqlDbType.NVarChar, 10).Value = mhDonVi.Text.Trim();
            cmd.Parameters.Add("@DonGia", SqlDbType.Float).Value = donGia;
            cmd.Parameters.Add("@GhiChu", SqlDbType.NVarChar, 200).Value = mhGhiChu.Text.Trim();
            try { conn.Open(); cmd.ExecuteNonQuery(); MessageBox.Show("Thêm mặt hàng thành công"); LoadMatHang(); }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) { MessageBox.Show("MaSP đã tồn tại"); }
            catch (Exception ex) { MessageBox.Show("Lỗi thêm mặt hàng: " + ex.Message); }
        }
    }

    private void MH_Edit()
    {
        if (string.IsNullOrWhiteSpace(mhMa.Text)) { MessageBox.Show("Chọn MaSP để sửa"); return; }
        if (string.IsNullOrWhiteSpace(mhTen.Text)) { MessageBox.Show("TenSP không được trống"); mhTen.Focus(); return; }
        double donGia; if (!TryParseDouble(mhDonGia.Text, out donGia)) { MessageBox.Show("DonGia không hợp lệ"); mhDonGia.Focus(); return; }

        using (var conn = new SqlConnection(CONN_STR))
        using (var cmd = new SqlCommand("sp_SuaMatHang", conn) { CommandType = CommandType.StoredProcedure })
        {
            cmd.Parameters.Add("@MaSP", SqlDbType.NChar, 5).Value = mhMa.Text.Trim();
            cmd.Parameters.Add("@TenSP", SqlDbType.NVarChar, 30).Value = mhTen.Text.Trim();
            cmd.Parameters.Add("@NgaySX", SqlDbType.Date).Value = mhNSX.Value.Date;
            cmd.Parameters.Add("@NgayHH", SqlDbType.Date).Value = mhNHH.Value.Date;
            cmd.Parameters.Add("@DonVi", SqlDbType.NVarChar, 10).Value = mhDonVi.Text.Trim();
            cmd.Parameters.Add("@DonGia", SqlDbType.Float).Value = donGia;
            cmd.Parameters.Add("@GhiChu", SqlDbType.NVarChar, 200).Value = mhGhiChu.Text.Trim();
            try { conn.Open(); int n = cmd.ExecuteNonQuery(); if (n > 0) { MessageBox.Show("Sửa mặt hàng thành công"); LoadMatHang(); } else MessageBox.Show("Không tìm thấy MaSP"); }
            catch (Exception ex) { MessageBox.Show("Lỗi sửa mặt hàng: " + ex.Message); }
        }
    }

    private void MH_Del()
    {
        if (string.IsNullOrWhiteSpace(mhMa.Text)) { MessageBox.Show("Chọn MaSP để xóa"); return; }
        if (MessageBox.Show("Xóa mặt hàng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;

        using (var conn = new SqlConnection(CONN_STR))
        using (var cmd = new SqlCommand("sp_XoaMatHang", conn) { CommandType = CommandType.StoredProcedure })
        {
            cmd.Parameters.Add("@MaSP", SqlDbType.NChar, 5).Value = mhMa.Text.Trim();
            try { conn.Open(); int n = cmd.ExecuteNonQuery(); if (n > 0) { MessageBox.Show("Xóa mặt hàng thành công"); LoadMatHang(); } else MessageBox.Show("Không tìm thấy MaSP"); }
            catch (SqlException ex) when (ex.Number == 547) { MessageBox.Show("Không thể xóa: đang có Hàng nhập tham chiếu MaSP này."); }
            catch (Exception ex) { MessageBox.Show("Lỗi xóa mặt hàng: " + ex.Message); }
        }
    }

    private void NCC_Add()
    {
        if (string.IsNullOrWhiteSpace(nccMa.Text)) { MessageBox.Show("MaNhaCC không được trống"); nccMa.Focus(); return; }
        if (string.IsNullOrWhiteSpace(nccTen.Text)) { MessageBox.Show("TenNhaCC không được trống"); nccTen.Focus(); return; }

        using (var conn = new SqlConnection(CONN_STR))
        using (var cmd = new SqlCommand("sp_ThemNhaCC", conn) { CommandType = CommandType.StoredProcedure })
        {
            cmd.Parameters.Add("@MaNhaCC", SqlDbType.NChar, 5).Value = nccMa.Text.Trim();
            cmd.Parameters.Add("@TenNhaCC", SqlDbType.NVarChar, 50).Value = nccTen.Text.Trim();
            cmd.Parameters.Add("@DiaChi", SqlDbType.NVarChar, 200).Value = nccDiaChi.Text.Trim();
            cmd.Parameters.Add("@MaSoThue", SqlDbType.NVarChar, 15).Value = nccMST.Text.Trim();
            cmd.Parameters.Add("@TaiKhoan", SqlDbType.NVarChar, 15).Value = nccTK.Text.Trim();
            cmd.Parameters.Add("@DienThoai", SqlDbType.NVarChar, 11).Value = nccDT.Text.Trim();
            try { conn.Open(); cmd.ExecuteNonQuery(); MessageBox.Show("Thêm nhà cung cấp thành công"); LoadNhaCC(); }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) { MessageBox.Show("MaNhaCC đã tồn tại"); }
            catch (Exception ex) { MessageBox.Show("Lỗi thêm nhà cung cấp: " + ex.Message); }
        }
    }

    private void NCC_Edit()
    {
        if (string.IsNullOrWhiteSpace(nccMa.Text)) { MessageBox.Show("Chọn MaNhaCC để sửa"); return; }

        using (var conn = new SqlConnection(CONN_STR))
        using (var cmd = new SqlCommand("sp_SuaNhaCC", conn) { CommandType = CommandType.StoredProcedure })
        {
            cmd.Parameters.Add("@MaNhaCC", SqlDbType.NChar, 5).Value = nccMa.Text.Trim();
            cmd.Parameters.Add("@TenNhaCC", SqlDbType.NVarChar, 50).Value = nccTen.Text.Trim();
            cmd.Parameters.Add("@DiaChi", SqlDbType.NVarChar, 200).Value = nccDiaChi.Text.Trim();
            cmd.Parameters.Add("@MaSoThue", SqlDbType.NVarChar, 15).Value = nccMST.Text.Trim();
            cmd.Parameters.Add("@TaiKhoan", SqlDbType.NVarChar, 15).Value = nccTK.Text.Trim();
            cmd.Parameters.Add("@DienThoai", SqlDbType.NVarChar, 11).Value = nccDT.Text.Trim();
            try { conn.Open(); int n = cmd.ExecuteNonQuery(); if (n > 0) { MessageBox.Show("Sửa nhà cung cấp thành công"); LoadNhaCC(); } else MessageBox.Show("Không tìm thấy MaNhaCC"); }
            catch (Exception ex) { MessageBox.Show("Lỗi sửa nhà cung cấp: " + ex.Message); }
        }
    }

    private void NCC_Del()
    {
        if (string.IsNullOrWhiteSpace(nccMa.Text)) { MessageBox.Show("Chọn MaNhaCC để xóa"); return; }
        if (MessageBox.Show("Xóa nhà cung cấp này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;

        using (var conn = new SqlConnection(CONN_STR))
        using (var cmd = new SqlCommand("sp_XoaNhaCC", conn) { CommandType = CommandType.StoredProcedure })
        {
            cmd.Parameters.Add("@MaNhaCC", SqlDbType.NChar, 5).Value = nccMa.Text.Trim();
            try { conn.Open(); int n = cmd.ExecuteNonQuery(); if (n > 0) { MessageBox.Show("Xóa nhà cung cấp thành công"); LoadNhaCC(); } else MessageBox.Show("Không tìm thấy MaNhaCC"); }
            catch (SqlException ex) when (ex.Number == 547) { MessageBox.Show("Không thể xóa: đang có Hàng nhập tham chiếu nhà cung cấp này."); }
            catch (Exception ex) { MessageBox.Show("Lỗi xóa nhà cung cấp: " + ex.Message); }
        }
    }

    private void HN_Add()
    {
        if (string.IsNullOrWhiteSpace(hnMaSP.Text) || string.IsNullOrWhiteSpace(hnMaNCC.Text) || string.IsNullOrWhiteSpace(hnSoHD.Text))
        { MessageBox.Show("MaSP, MaNhaCC, SoHD không được trống"); return; }
        int sl; if (!TryParseInt(hnSoLuong.Text, out sl) || sl < 0) { MessageBox.Show("SoLuong không hợp lệ"); hnSoLuong.Focus(); return; }
        double dg; if (!TryParseDouble(hnDonGia.Text, out dg) || dg < 0) { MessageBox.Show("DonGia không hợp lệ"); hnDonGia.Focus(); return; }

        using (var conn = new SqlConnection(CONN_STR))
        using (var cmd = new SqlCommand("sp_ThemHangNhap", conn) { CommandType = CommandType.StoredProcedure })
        {
            cmd.Parameters.Add("@MaSP", SqlDbType.NChar, 5).Value = hnMaSP.Text.Trim();
            cmd.Parameters.Add("@MaNhaCC", SqlDbType.NChar, 5).Value = hnMaNCC.Text.Trim();
            cmd.Parameters.Add("@SoLuong", SqlDbType.Int).Value = sl;
            cmd.Parameters.Add("@DonGia", SqlDbType.Float).Value = dg;
            cmd.Parameters.Add("@SoHD", SqlDbType.NVarChar, 10).Value = hnSoHD.Text.Trim();
            cmd.Parameters.Add("@NgayGH", SqlDbType.Date).Value = hnNgayGH.Value.Date;
            try { conn.Open(); cmd.ExecuteNonQuery(); MessageBox.Show("Thêm hàng nhập thành công"); LoadHangNhap(); }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) { MessageBox.Show("Bản ghi đã tồn tại (trùng MaSP + MaNhaCC + SoHD)"); }
            catch (SqlException ex) when (ex.Number == 547) { MessageBox.Show("FK lỗi: MaSP/MaNhaCC không tồn tại trong danh mục"); }
            catch (Exception ex) { MessageBox.Show("Lỗi thêm hàng nhập: " + ex.Message); }
        }
    }

    private void HN_Edit()
    {
        if (string.IsNullOrWhiteSpace(hnMaSP.Text) || string.IsNullOrWhiteSpace(hnMaNCC.Text) || string.IsNullOrWhiteSpace(hnSoHD.Text))
        { MessageBox.Show("Chọn MaSP, MaNhaCC, SoHD để sửa"); return; }
        int sl; if (!TryParseInt(hnSoLuong.Text, out sl) || sl < 0) { MessageBox.Show("SoLuong không hợp lệ"); return; }
        double dg; if (!TryParseDouble(hnDonGia.Text, out dg) || dg < 0) { MessageBox.Show("DonGia không hợp lệ"); return; }

        using (var conn = new SqlConnection(CONN_STR))
        using (var cmd = new SqlCommand("sp_SuaHangNhap", conn) { CommandType = CommandType.StoredProcedure })
        {
            cmd.Parameters.Add("@MaSP", SqlDbType.NChar, 5).Value = hnMaSP.Text.Trim();
            cmd.Parameters.Add("@MaNhaCC", SqlDbType.NChar, 5).Value = hnMaNCC.Text.Trim();
            cmd.Parameters.Add("@SoHD", SqlDbType.NVarChar, 10).Value = hnSoHD.Text.Trim();
            cmd.Parameters.Add("@SoLuong", SqlDbType.Int).Value = sl;
            cmd.Parameters.Add("@DonGia", SqlDbType.Float).Value = dg;
            cmd.Parameters.Add("@NgayGH", SqlDbType.Date).Value = hnNgayGH.Value.Date;
            try { conn.Open(); int n = cmd.ExecuteNonQuery(); if (n > 0) { MessageBox.Show("Sửa hàng nhập thành công"); LoadHangNhap(); } else MessageBox.Show("Không tìm thấy bản ghi để sửa"); }
            catch (Exception ex) { MessageBox.Show("Lỗi sửa hàng nhập: " + ex.Message); }
        }
    }

    private void HN_Del()
    {
        if (string.IsNullOrWhiteSpace(hnMaSP.Text) || string.IsNullOrWhiteSpace(hnMaNCC.Text) || string.IsNullOrWhiteSpace(hnSoHD.Text))
        { MessageBox.Show("Chọn MaSP, MaNhaCC, SoHD để xóa"); return; }
        if (MessageBox.Show("Xóa bản ghi hàng nhập này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;

        using (var conn = new SqlConnection(CONN_STR))
        using (var cmd = new SqlCommand("sp_XoaHangNhap", conn) { CommandType = CommandType.StoredProcedure })
        {
            cmd.Parameters.Add("@MaSP", SqlDbType.NChar, 5).Value = hnMaSP.Text.Trim();
            cmd.Parameters.Add("@MaNhaCC", SqlDbType.NChar, 5).Value = hnMaNCC.Text.Trim();
            cmd.Parameters.Add("@SoHD", SqlDbType.NVarChar, 10).Value = hnSoHD.Text.Trim();
            try { conn.Open(); int n = cmd.ExecuteNonQuery(); if (n > 0) { MessageBox.Show("Xóa hàng nhập thành công"); LoadHangNhap(); } else MessageBox.Show("Không tìm thấy bản ghi để xóa"); }
            catch (Exception ex) { MessageBox.Show("Lỗi xóa hàng nhập: " + ex.Message); }
        }
    }
}

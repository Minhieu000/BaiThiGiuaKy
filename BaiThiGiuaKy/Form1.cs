using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BaiThiGiuaKy;


namespace BaiThiGiuaKy

{
    public partial class FrmSanpham : Form
    {
        public FrmSanpham()
        {
            InitializeComponent();
        }
        private bool isAdding = false;


        private void FillCombobox(List<LoaiSP> listLoaiSP)
        {
            this.cboLoaiSP.DataSource = listLoaiSP;
            this.cboLoaiSP.DisplayMember = "TenLoai";
            this.cboLoaiSP.ValueMember = "MaLoai";

        }

        private void BindGrid(List<SanPham> listSP)
        {
            dgvSanpham.Rows.Clear();
            foreach (var item in listSP)
            {
                int index = dgvSanpham.Rows.Add();
                dgvSanpham.Rows[index].Cells[0].Value = item.MaSP;
                dgvSanpham.Rows[index].Cells[1].Value = item.TenSP;
                var LoaiSP = cboLoaiSP.Items.Cast<LoaiSP>()
            .FirstOrDefault(f => f.MaLoai == item.MaLoai);
                if (LoaiSP != null)
                {
                    dgvSanpham.Rows[index].Cells[3].Value = LoaiSP.TenLoai;
                }
                dgvSanpham.Rows[index].Cells[2].Value = item.NgayNhap;
            }
        }


        private void HandleException(Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }



        private void LoadFacultyComboBox()
        {
            try
            {
                using (var context = new Model1())
                {
                    var faculties = context.LoaiSPs.ToList();
                    cboLoaiSP.DataSource = faculties;
                    cboLoaiSP.DisplayMember = "TenLoai";
                    cboLoaiSP.ValueMember = "MaLoai";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách khoa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void dgvSanpham_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0) // Đảm bảo không click vào tiêu đề
                {
                    txtMaSP.Text = dgvSanpham.Rows[e.RowIndex].Cells[0].Value?.ToString();
                    txtTenSP.Text = dgvSanpham.Rows[e.RowIndex].Cells[1].Value?.ToString();
                    dtNgaynhap.Text = dgvSanpham.Rows[e.RowIndex].Cells[2].Value?.ToString();
                    cboLoaiSP.Text = dgvSanpham.Rows[e.RowIndex].Cells[3].Value?.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi chọn dòng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FrmSanpham_Load(object sender, EventArgs e)
        {
            try
            {
                Model1 context = new Model1();
                List<LoaiSP> listLoaiSP = context.LoaiSPs.ToList();
                List<SanPham> listSP = context.SanPhams.ToList();
                FillCombobox(listLoaiSP);
                BindGrid(listSP);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btSua_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(txtMaSP.Text) ||
                    string.IsNullOrWhiteSpace(txtTenSP.Text) ||
                    !DateTime.TryParse(dtNgaynhap.Text, out DateTime ngayNhap))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ và hợp lệ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var context = new Model1())
                {
                    // Tìm sản phẩm cần sửa theo mã sản phẩm
                    string MaSP = txtMaSP.Text;
                    var productToUpdate = context.SanPhams.FirstOrDefault(s => s.MaSP == MaSP);

                    if (productToUpdate != null)
                    {
                        // Cập nhật thông tin sản phẩm
                        productToUpdate.TenSP = txtTenSP.Text;
                        productToUpdate.NgayNhap = ngayNhap;
                        productToUpdate.MaLoai = cboLoaiSP.SelectedValue.ToString();
                        // Lưu thay đổi
                        context.SaveChanges();

                        // Hiển thị lại danh sách
                        BindGrid(context.SanPhams.ToList());

                        MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sản phẩm!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaSP.Text) ||
                    string.IsNullOrWhiteSpace(txtTenSP.Text) ||
                    !DateTime.TryParse(dtNgaynhap.Text, out DateTime ngayNhap))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ và hợp lệ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var context = new Model1())
                {
                    if (context.SanPhams.Any(s => s.MaSP == txtMaSP.Text))
                    {
                        MessageBox.Show("Mã sản phẩm đã tồn tại!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var newSanpham = new SanPham
                    {
                        MaSP = txtMaSP.Text,
                        TenSP = txtTenSP.Text,
                        NgayNhap = ngayNhap,
                        MaLoai = cboLoaiSP.SelectedValue.ToString()
                    };
                    context.SanPhams.Add(newSanpham);
                    context.SaveChanges();
                    MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    BindGrid(context.SanPhams.ToList());
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void btXoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra nếu chưa chọn sản phẩm
                if (string.IsNullOrWhiteSpace(txtMaSP.Text))
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Hiển thị hộp thoại xác nhận
                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này?",
                                                    "Xác nhận xóa",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Question);
                if (confirmResult == DialogResult.Yes)
                {
                    using (var context = new Model1())
                    {
                        // Tìm sản phẩm cần xóa
                        string MaSP = txtMaSP.Text;
                        var productToDelete = context.SanPhams.FirstOrDefault(s => s.MaSP == MaSP);

                        if (productToDelete != null)
                        {
                            // Xóa sản phẩm khỏi cơ sở dữ liệu
                            context.SanPhams.Remove(productToDelete);
                            context.SaveChanges();

                            // Hiển thị lại danh sách sản phẩm
                            BindGrid(context.SanPhams.ToList());

                            MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy sản phẩm cần xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btLuu_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(txtMaSP.Text) ||
                    string.IsNullOrWhiteSpace(txtTenSP.Text) ||
                    !DateTime.TryParse(dtNgaynhap.Text, out DateTime ngayNhap))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ và hợp lệ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var context = new Model1())
                {
                    string MaSP = txtMaSP.Text;
                    // Kiểm tra sản phẩm có tồn tại trong cơ sở dữ liệu không
                    var existingProduct = context.SanPhams.FirstOrDefault(s => s.MaSP == MaSP);

                    if (existingProduct != null)
                    {
                        // Nếu sản phẩm đã tồn tại, cập nhật thông tin
                        existingProduct.TenSP = txtTenSP.Text;
                        existingProduct.NgayNhap = ngayNhap;
                        existingProduct.MaLoai = cboLoaiSP.SelectedValue.ToString();
                        MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                    else
                    {
                        // Nếu sản phẩm chưa tồn tại, thêm mới
                        var newProduct = new SanPham
                        {
                            MaSP = MaSP,
                            TenSP = txtTenSP.Text,
                            NgayNhap = ngayNhap,
                            MaLoai = cboLoaiSP.SelectedValue.ToString()
                        };
                        context.SanPhams.Add(newProduct);
                        MessageBox.Show("Thêm mới sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // Lưu thay đổi
                    context.SaveChanges();

                    // Làm mới danh sách hiển thị
                    BindGrid(context.SanPhams.ToList());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btKLuu_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra nếu có dòng được chọn trong DataGridView
                if (dgvSanpham.SelectedRows.Count > 0)
                {
                    // Lấy dữ liệu của dòng được chọn
                    var selectedRow = dgvSanpham.SelectedRows[0];
                    txtMaSP.Text = selectedRow.Cells[0].Value?.ToString();
                    txtTenSP.Text = selectedRow.Cells[1].Value?.ToString();
                    dtNgaynhap.Text = selectedRow.Cells[2].Value?.ToString();
                    cboLoaiSP.Text = selectedRow.Cells[3].Value?.ToString();
                }
                else
                {
                    // Nếu không có dòng nào được chọn, xóa toàn bộ ô nhập liệu
                    txtMaSP.Clear();
                    txtTenSP.Clear();
                    dtNgaynhap.Value = DateTime.Now; // Đặt lại ngày hiện tại
                    cboLoaiSP.SelectedIndex = -1; // Bỏ chọn combobox
                }

                // Hiển thị thông báo
                MessageBox.Show("Dữ liệu đã được khôi phục!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thực hiện khôi phục: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btThoat_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn thoát chương trình?",
                                "Xác nhận Thoát",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);

            // Nếu người dùng chọn "Yes", thoát ứng dụng
            if (confirmResult == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

    }
}
    
    




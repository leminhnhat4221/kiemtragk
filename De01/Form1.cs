using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using De01.QuanlySV;

namespace De01
{
    public partial class frmSinhVien : Form
    {
        public frmSinhVien()
        {
            InitializeComponent();
        }

        private void btnCLose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmSinhVien_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show("Bạn có muốn thoát chương trình hay không", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }
        }

        private void frmSinhVien_Load(object sender, EventArgs e)
        {
            try
            {
                //cmbFac.SelectedIndex = 0;
                cmbClass.Items.Add("22DTHA1");
                cmbClass.Items.Add("22DTHA2");
                cmbClass.Items.Add("22DTHA3");
                cmbClass.Items.Add("22DTHA4");
                cmbClass.Items.Add("22DTHA5");
                cmbClass.SelectedItem = "22DTHA1";

                Model1 context = new Model1();
                List<LOP> lops = context.LOPs.ToList();
                List<SINHVIEN> sinhviens = context.SINHVIENs.ToList();
                BindGrid(sinhviens);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void BindGrid(List<SINHVIEN> sinhviens)
        {
            dgvSinhVien.Rows.Clear();
            foreach (var item in sinhviens)
            {
                int index = dgvSinhVien.Rows.Add();
                dgvSinhVien.Rows[index].Cells[0].Value = item.MaSV;
                dgvSinhVien.Rows[index].Cells[1].Value = item.HoTenSV;
                dgvSinhVien.Rows[index].Cells[2].Value = item.NgaySinh;
                dgvSinhVien.Rows[index].Cells[3].Value = item.LOP.TenLop;

            }
        }

        private void dgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow row = dgvSinhVien.Rows[e.RowIndex];
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                txtID.Text = row.Cells[0].Value.ToString();
                txtName.Text = row.Cells[1].Value.ToString();
                dateTimePicker1.Text = cmbClass.Text = row.Cells[2].Value.ToString();
                cmbClass.Text = row.Cells[3].Value.ToString();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(txtID.Text) || string.IsNullOrEmpty(txtName.Text) || cmbClass.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string studentID = txtID.Text.Trim();
                string fullName = txtName.Text.Trim();
                string className = cmbClass.SelectedItem.ToString();
                string dateOfBirth = dateTimePicker1.Value.ToString("yyyy-MM-dd");

                // Kiểm tra xem sinh viên đã tồn tại hay chưa
                if (IsStudentIDExist(studentID))
                {
                    MessageBox.Show("Mã sinh viên đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Thêm sinh viên vào DataGridView
                AddStudent(studentID, fullName, dateOfBirth, className);

                // Thêm sinh viên vào cơ sở dữ liệu
                using (Model1 context = new Model1())
                {
                    SINHVIEN newStudent = new SINHVIEN
                    {
                        MaSV = studentID,
                        HoTenSV = fullName,
                        NgaySinh = dateTimePicker1.Value,
                        MaLop = className
                    };
                    context.SINHVIENs.Add(newStudent);
                    context.SaveChanges();
                }

                MessageBox.Show("Thêm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData(); // Làm mới DataGridView
                ClearInputFields(); // Xóa các trường nhập liệu
            }
            catch (Exception ex)
            {
            
            }
        }

        private void AddStudent(string studentID, string fullName, string date, string lop)
        {
            dgvSinhVien.Rows.Add(studentID, fullName, date, lop);
            MessageBox.Show("Thông tin sinh viên đã được thêm mới", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool IsStudentIDExist(string studentID) //Kiểm tra MSSV
        {
            foreach (DataGridViewRow row in dgvSinhVien.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == studentID)
                {
                    return true;
                }
            }
            return false;
        }

        private void btnFix_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(txtID.Text) || string.IsNullOrEmpty(txtName.Text) || cmbClass.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string studentID = txtID.Text.Trim();
                string fullName = txtName.Text.Trim();
                string className = cmbClass.SelectedItem.ToString();
                DateTime dateOfBirth = dateTimePicker1.Value;

                using (Model1 context = new Model1())
                {
                    // Tìm sinh viên trong cơ sở dữ liệu
                    var existingStudent = context.SINHVIENs.FirstOrDefault(s => s.MaSV == studentID);

                    if (existingStudent != null)
                    {
                        // Cập nhật thông tin sinh viên
                        existingStudent.HoTenSV = fullName;
                        existingStudent.NgaySinh = dateOfBirth;
                        existingStudent.MaLop = className;

                        // Lưu thay đổi vào cơ sở dữ liệu
                        context.SaveChanges();

                        // Cập nhật thông tin trên DataGridView
                        foreach (DataGridViewRow row in dgvSinhVien.Rows)
                        {
                            if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == studentID)
                            {
                                row.Cells[1].Value = fullName;
                                row.Cells[2].Value = dateOfBirth.ToString("yyyy-MM-dd");
                                row.Cells[3].Value = className;
                                break;
                            }
                        }

                        MessageBox.Show("Thông tin sinh viên đã được cập nhật!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Sinh viên không tồn tại trong hệ thống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {

            // Kiểm tra xem có ô nào được chọn không
            if (dgvSinhVien.CurrentCell != null)
            {
                int rowIndex = dgvSinhVien.CurrentCell.RowIndex; // Lấy chỉ số hàng của ô đang chọn

                // Kiểm tra chỉ số hợp lệ và không phải hàng mới
                if (rowIndex >= 0 && !dgvSinhVien.Rows[rowIndex].IsNewRow)
                {
                    // Xác nhận trước khi xóa
                    var result = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này không?",
                                                 "Xác nhận",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        dgvSinhVien.Rows.RemoveAt(rowIndex); // Xóa hàng tại chỉ số rowIndex
                        MessageBox.Show("Đã xóa sinh viên thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Không thể xóa hàng trống hoặc không hợp lệ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một ô để xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSeach_Click(object sender, EventArgs e)
        {
            
        }

        private void btnNotSave_Click(object sender, EventArgs e)
        {
            ClearInput();   
        }
        private void ClearInput()
        {
            txtID.Clear();
            txtName.Clear();
            dateTimePicker1.Value = DateTime.Now;
            cmbClass.SelectedIndex = -1;
        }

        private void ClearInputFields()
        {
            txtID.Text = "";
            txtName.Text = "";
            dateTimePicker1.Value = DateTime.Now;
            cmbClass.SelectedIndex = -1;
        }

        private void LoadData()
        {
            try
            {
                using (Model1 context = new Model1())
                {
                    List<SINHVIEN> sinhviens = context.SINHVIENs.ToList();
                    List<LOP> lops = context.LOPs.ToList();

                    //FillFacultyComboBox(lops);
                    BindGrid(sinhviens);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(txtID.Text) || string.IsNullOrEmpty(txtName.Text) || cmbClass.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (Model1 context = new Model1())
                {
                    string maSV = txtID.Text.Trim();

                    // Kiểm tra xem sinh viên đã tồn tại hay chưa
                    var existingSV = context.SINHVIENs.FirstOrDefault(s => s.MaSV == maSV);

                    if (existingSV != null)
                    {
                        // Nếu đã tồn tại, cập nhật thông tin
                        existingSV.HoTenSV = txtName.Text.Trim();
                        existingSV.NgaySinh = dateTimePicker1.Value;
                        existingSV.MaLop = cmbClass.SelectedValue.ToString();

                        MessageBox.Show("Đã cập nhật thông tin sinh viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Nếu chưa tồn tại, thêm mới sinh viên
                        SINHVIEN newSV = new SINHVIEN
                        {
                            MaSV = maSV,
                            HoTenSV = txtName.Text.Trim(),
                            NgaySinh = dateTimePicker1.Value,
                            MaLop = cmbClass.SelectedValue.ToString()
                        };

                        context.SINHVIENs.Add(newSV);
                        MessageBox.Show("Đã thêm sinh viên mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    context.SaveChanges();

                    // Làm mới giao diện
                    LoadData();
                    ClearInputFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            try
            {
                // Kiểm tra điều kiện nhập liệu
                if (string.IsNullOrEmpty(txtID.Text) && string.IsNullOrEmpty(txtName.Text))
                {
                    MessageBox.Show("Vui lòng nhập mã sinh viên hoặc tên sinh viên để tìm kiếm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string searchID = txtID.Text.Trim();
                string searchName = txtName.Text.Trim().ToLower();

                using (Model1 context = new Model1())
                {
                    // Lấy danh sách sinh viên từ cơ sở dữ liệu
                    List<SINHVIEN> sinhviens = context.SINHVIENs.ToList();

                    // Lọc danh sách theo mã sinh viên hoặc tên sinh viên
                    var filteredList = sinhviens.Where(s =>
                        (!string.IsNullOrEmpty(searchID) && s.MaSV.Contains(searchID)) ||
                        (!string.IsNullOrEmpty(searchName) && s.HoTenSV.ToLower().Contains(searchName))
                    ).ToList();

                    // Kiểm tra kết quả
                    if (filteredList.Any())
                    {
                        BindGrid(filteredList); // Cập nhật DataGridView với kết quả tìm kiếm
                        MessageBox.Show($"Tìm thấy {filteredList.Count} sinh viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên nào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        BindGrid(new List<SINHVIEN>()); // Làm trống DataGridView
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

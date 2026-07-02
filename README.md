<div align="center">
  
  # 💄 L'Univers Beauté
  
  **Hệ thống Quản lý Cửa hàng Mỹ phẩm Toàn diện**
  
  [![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)]()
  [![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)]()
  [![SQL Server](https://img.shields.io/badge/SQLServer-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)]()
  [![Inno Setup](https://img.shields.io/badge/Inno_Setup-004B87?style=for-the-badge)]()
</div>

---

## 📖 Giới thiệu
**L'Univers Beauté** là phần mềm quản lý cửa hàng mỹ phẩm được phát triển nhằm tối ưu hóa quy trình bán hàng, quản lý kho, và chăm sóc khách hàng. Giao diện trực quan, dễ sử dụng cùng kiến trúc phần mềm chặt chẽ giúp hệ thống vận hành ổn định và hiệu quả.

---

## ✨ Tính năng nổi bật (Trọng tâm)
- 🛒 **Quản lý Bán hàng:** Tạo hóa đơn nhanh chóng, hỗ trợ tính toán chiết khấu, khuyến mãi.
- 📦 **Quản lý Kho hàng:** Theo dõi số lượng mỹ phẩm, cảnh báo sắp hết hàng, quản lý nhà cung cấp.
- 👥 **Quản lý Nhân sự & Phân quyền:** Quản lý thông tin nhân viên, phân quyền truy cập theo vai trò (Quản lý, Thu ngân, Kho...).
- 🤝 **Chăm sóc Khách hàng:** Lưu trữ thông tin khách hàng, hạng thẻ thành viên, tích điểm.
- 📊 **Báo cáo & Thống kê:** Báo cáo doanh thu, lợi nhuận, top sản phẩm bán chạy theo ngày/tháng/năm trực quan.

---

## 🏗 Kiến trúc Hệ thống
Dự án được xây dựng theo **Mô hình 3 Lớp (3-Tier Architecture)** giúp dễ dàng bảo trì và phát triển:
1. **GUI (Giao diện người dùng):** Nơi tương tác trực tiếp với người dùng.
2. **BUS (Business Logic Layer):** Xử lý các nghiệp vụ logic của phần mềm.
3. **DAL (Data Access Layer):** Giao tiếp trực tiếp với cơ sở dữ liệu (thêm, sửa, xóa, lấy dữ liệu).
4. **DTO (Data Transfer Object):** Các đối tượng trung chuyển dữ liệu giữa các lớp.

---

## 🚀 Hướng dẫn Cài đặt & Chạy dự án

### Yêu cầu hệ thống
- Visual Studio (Khuyến nghị bản 2019 hoặc mới hơn)
- SQL Server (Để chạy cơ sở dữ liệu)

### Các bước thiết lập
1. **Clone dự án về máy:**
   ```bash
   git clone https://github.com/Minh-Nha/LUnivers-Beaute.git
   ```
2. **Cơ sở dữ liệu (Đã được Host trên Server):**
   - Hiện tại hệ thống đã được kết nối sẵn với cơ sở dữ liệu trên server riêng.
   - Bạn không cần cài đặt SQL Server local, chỉ cần có mạng Internet là chạy được ngay.
   - *(Tùy chọn)* Nếu bạn muốn tự chạy Database ở máy cá nhân (Local): Hãy chạy script SQL trong thư mục `Database/` và sửa lại chuỗi kết nối trong file `config.json` (hoặc trong source code lớp DAL).
4. **Chạy dự án:**
   - Mở file `.sln` bằng Visual Studio.
   - Nhấn `Start` (hoặc `F5`) để build và chạy ứng dụng.

> 💡 **Lưu ý:** Dự án có đính kèm file `LUniversBeaute_Installer.iss`. Bạn có thể sử dụng Inno Setup để đóng gói phần mềm thành file `.exe` cài đặt cực kỳ tiện lợi.

---

## 🤝 Đóng góp
Nếu bạn muốn đóng góp cho dự án, vui lòng tạo một `Pull Request` hoặc mở một `Issue` để thảo luận.
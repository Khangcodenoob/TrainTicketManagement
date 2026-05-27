# 🚂 Hệ Thống Quản Lý Vé Tàu

Một hệ thống quản lý vé tàu toàn diện được xây dựng bằng **.NET 8.0** với ba ứng dụng: Web MVC, API REST, và ứng dụng Desktop WinForms.

---

## 📁 Cây Thư Mục

```
Duong Chi Khang/
│
├── 📄 README.md
├── 📄 Program.cs                          # ASP.NET Core Web App entry point
├── 📄 appsettings.json
├── 📄 appsettings.Development.json
├── 📄 Database.sql
├── 📄 Duong Chi Khang.csproj
├── 📄 Duong Chi Khang.csproj.user
├── 📄 Duong Chi Khang.sln                 # Solution file
│
├── 📂 Controllers/                        # MVC Controllers
│   └── HomeController.cs
│
├── 📂 Models/                             # Domain Models
│   └── ErrorViewModel.cs
│
├── 📂 Properties/                         # Project Properties
│   └── launchSettings.json                # Port: 5250 (HTTP), 7252 (HTTPS)
│
├── 📂 Views/                              # MVC Views
│   ├── _ViewStart.cshtml
│   ├── _ViewImports.cshtml
│   ├── Home/
│   └── Shared/
│
├── 📂 wwwroot/                            # Static files
│   ├── css/
│   ├── js/
│   └── lib/
│
├── 📂 bin/                                # Compiled binaries
├── 📂 obj/                                # Build outputs
│
├── 📂 TrainTicketApi/                     # ASP.NET Core API Backend
│   │
│   ├── 📄 Program.cs
│   ├── 📄 appsettings.json
│   ├── 📄 appsettings.Development.json
│   ├── 📄 TrainTicketApi.csproj
│   ├── 📄 TrainTicketApi.csproj.user
│   ├── 📄 TrainTicketApi.http             # HTTP test file
│   │
│   ├── 📂 Controllers/                    # API Endpoints
│   │   ├── AuthController.cs              # Authentication
│   │   ├── CustomersController.cs         # Customer CRUD
│   │   ├── DashboardController.cs         # Dashboard statistics
│   │   ├── RoutesController.cs            # Train routes management
│   │   ├── TicketsController.cs           # Tickets management
│   │   └── TrainTripsController.cs        # Train trips management
│   │
│   ├── 📂 Data/                           # Database Context
│   │   ├── AppDbContext.cs                # EF Core DbContext
│   │   └── DbSeeder.cs                    # Database seed data
│   │
│   ├── 📂 DTOs/                           # Data Transfer Objects
│   │   ├── Auth/
│   │   ├── Customers/
│   │   ├── Dashboard/
│   │   ├── Routes/
│   │   ├── Tickets/
│   │   └── TrainTrips/
│   │
│   ├── 📂 Migrations/                     # EF Core Migrations
│   │   ├── 20260505082112_InitTrainTicketManagement.cs
│   │   ├── 20260505082112_InitTrainTicketManagement.Designer.cs
│   │   ├── 20260505094110_AddIdentityAndAdvancedFeatures.cs
│   │   ├── 20260505094110_AddIdentityAndAdvancedFeatures.Designer.cs
│   │   ├── 20260505114721_UpdateTicketManagement.cs
│   │   ├── 20260505114721_UpdateTicketManagement.Designer.cs
│   │   └── AppDbContextModelSnapshot.cs
│   │
│   ├── 📂 Models/                         # Database Models
│   │   ├── AuditLog.cs
│   │   ├── Customer.cs
│   │   ├── Ticket.cs
│   │   ├── TrainRoute.cs
│   │   └── TrainTrip.cs
│   │
│   ├── 📂 Repositories/                   # Repository pattern (DAL)
│   │   └── Interfaces/
│   │
│   ├── 📂 Services/                       # Business logic
│   │   └── Interfaces/
│   │
│   ├── 📂 Properties/
│   │   └── launchSettings.json            # Port: 5216 (HTTP), 7282 (HTTPS)
│   │
│   └── 📂 bin/, obj/
│
├── 📂 Database/
│   └── CreateTrainTicketManagementDB.sql

└── 📂 TrainTicketWinForms/                # Windows Forms Desktop App
    │
    ├── 📄 Program.cs
    ├── 📄 TrainTicketWinForms.csproj
    ├── 📄 TrainTicketWinForms.csproj.user
    │
    ├── 📄 Form1.cs
    ├── 📄 Form1.Designer.cs
    ├── 📄 Form1.resx
    ├── 📄 Form1.GridIcons.cs              # Grid icons handling
    ├── 📄 Form1.Logic.cs                  # Business logic
    ├── 📄 Form1.Stations.cs               # Station management
    ├── 📄 Form1.Views.cs                  # UI views
    │
    ├── 📄 LoginForm.cs
    ├── 📄 LoginForm.resx
    │
    ├── 📄 ConfirmDialog.cs                # Confirmation dialogs
    ├── 📄 ShadowPanel.cs                  # Custom UI components
    ├── 📄 UiTheme.cs                      # Theme styling
    │
    ├── 📂 Models/
    ├── 📂 Services/
    ├── 📂 bin/, obj/

---

## ✨ Tính Năng Chính (Features)

### 1. **Quản Lý Tuyến Tàu (Train Routes)**
   - Tạo, sửa, xóa tuyến tàu
   - Quản lý ga đi/đến
   - Theo dõi khoảng cách

### 2. **Quản Lý Chuyến Tàu (Train Trips)**
   - Tạo chuyến tàu mới
   - Quản lý chỗ ngồi (tổng, còn trống)
   - Thiết lập giá vé cơ bản
   - Quản lý thời gian xuất phát/đến

### 3. **Quản Lý Khách Hàng (Customer Management)**
   - Thêm khách hàng mới
   - Cập nhật thông tin
   - Xem lịch sử đặt vé

### 4. **Quản Lý Vé Tàu (Ticket Management)**
   - Đặt vé tàu
   - Hủy vé
   - Xem lịch sử vé
   - Tính giá động

### 5. **Dashboard & Báo Cáo (Dashboard & Reports)**
   - Thống kê doanh thu
   - Thống kê khách hàng
   - Thống kê chuyến tàu
   - Xuất báo cáo

### 6. **Xác Thực & Phân Quyền (Authentication & Authorization)**
   - Đăng nhập bằng JWT
   - Role-based access control
   - Chỉ có `POST /api/auth/login`
   - Audit logging

### 7. **Ghi Nhật Ký (Audit Logging)**
   - Theo dõi các thay đổi
   - Lịch sử truy cập
   - Báo cáo bảo mật

---

## 🛠️ Công Nghệ Sử Dụng (Technology Stack)

| Layer | Technology | Version |
|-------|-----------|---------|
| **Framework** | .NET | 8.0 |
| **Web Framework** | ASP.NET Core | 8.0.11 |
| **ORM** | Entity Framework Core | 8.0.11 |
| **Database** | SQL Server | 2019+ (SQLEXPRESS) |
| **Authentication** | ASP.NET Identity | 8.0.11 |
| **Authorization** | JWT Bearer Tokens | - |
| **API Documentation** | Swagger/Swashbuckle | 6.4.0 |
| **Desktop UI** | Windows Forms | net8.0-windows |
| **JSON Serialization** | System.Text.Json | - |
| **HTTP Client** | Newtonsoft.Json | 13.0.3 |

---

## 🚀 Cách Chạy Project (HOW TO RUN) - QUAN TRỌNG NHẤT

### 📋 Yêu Cầu Tiên Quyết

- **.NET 8.0 SDK** hoặc cao hơn - [Tải về](https://dotnet.microsoft.com/download)
- **SQL Server 2019+** hoặc **SQL Server Express** - [Tải về](https://www.microsoft.com/sql-server/sql-server-downloads)
- **Visual Studio 2022** hoặc **Visual Studio Code**
- **Git** (tùy chọn)

### ✅ Cấu Hình Cơ Sở Dữ Liệu

#### Bước 1: Kiểm tra chuỗi kết nối
Mở file [TrainTicketApi/appsettings.json](TrainTicketApi/appsettings.json) và kiểm tra:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=KhangDC\\SQLEXPRESS;Database=TrainTicketManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

**Thay đổi:**
- `KhangDC\\SQLEXPRESS` → Thay bằng tên SQL Server instance của bạn
- Nếu sử dụng SQL Server local: `Server=(local)\\SQLEXPRESS` hoặc `.\\SQLEXPRESS`

#### Bước 2: Chạy Migration (Tự động tạo cơ sở dữ liệu)

```bash
# Di chuyển đến thư mục TrainTicketApi
cd TrainTicketApi

# Chạy migrations để tạo database
dotnet ef database update

# Hoặc từ Package Manager Console trong Visual Studio
Update-Database
```

**Kiểm tra:** Mở SQL Server Management Studio → Database → `TrainTicketManagementDB` có tồn tại

### 🎯 Chạy Ứng Dụng

#### **Cách 1: Chạy từ Command Line**

```bash
# Đảm bảo bạn ở thư mục gốc (Duong Chi Khang)

# 1️⃣ Chạy API Backend (Terminal 1)
cd TrainTicketApi
dotnet run

# ✅ API sẽ chạy tại:
# - HTTP: http://localhost:5216
# - HTTPS: https://localhost:7282
# - Swagger UI: https://localhost:7282/swagger

# 2️⃣ Chạy Web MVC (Terminal 2)
cd ..
dotnet run

# ✅ Web MVC sẽ chạy tại:
# - HTTP: http://localhost:5250
# - HTTPS: https://localhost:7252

# 3️⃣ Chạy Desktop App (Terminal 3)
cd TrainTicketWinForms
dotnet run
```

#### **Cách 2: Chạy từ Visual Studio 2022**

1. **Mở Solution:** `Duong Chi Khang.sln`
2. **Set Multiple Startup Projects:**
   - Chuột phải vào Solution → Properties
   - Chọn "Multiple startup projects"
   - Set:
     - `TrainTicketApi` → **Start**
     - `Duong Chi Khang` → **Start**
     - `TrainTicketWinForms` → (Tùy chọn)
3. **Nhấn F5** hoặc **Ctrl+F5** để chạy

#### **Cách 3: Chạy từ Visual Studio Code**

1. Mở folder `g:\TrainTicketManagement` trong VS Code
2. Cài đặt extension: **C# Dev Kit** & **REST Client**
3. Mở **Terminal** → Chạy:

```bash
# Terminal 1 - API
cd TrainTicketApi && dotnet run

# Terminal 2 (mới) - Web MVC
dotnet run
```

### 📱 Truy Cập Ứng Dụng

| Ứng Dụng | URL | Port | Ghi Chú |
|----------|-----|------|--------|
| **API (HTTP)** | http://localhost:5216 | 5216 | REST API |
| **API (HTTPS)** | https://localhost:7282 | 7282 | REST API (Bảo mật) |
| **Swagger UI** | https://localhost:7282/swagger | 7282 | API Documentation |
| **Web MVC (HTTP)** | http://localhost:5250 | 5250 | Web Interface |
| **Web MVC (HTTPS)** | https://localhost:7252 | 7252 | Web Interface (Bảo mật) |
| **Desktop App** | N/A | N/A | Windows Forms Application |

---

## 👤 Tài Khoản Test

### 📚 Khởi Tạo Dữ Liệu
Khi cơ sở dữ liệu được tạo lần đầu, các tài khoản test sẽ được tự động tạo thông qua [DbSeeder.cs](TrainTicketApi/Data/DbSeeder.cs).

### 🔓 Thông Tin Đăng Nhập

| Vai Trò | Tên Người Dùng | Mật Khẩu | Ghi Chú |
|---------|----------------|----------|--------|
| **Quản Trị Viên** | admin | admin123 | Quản trị viên hệ thống |
| **Nhân Viên** | staff | staff123 | Nhân viên |

> Hiện tại không có tài khoản khách hàng `customer` để đăng nhập. Khách hàng chỉ là dữ liệu trong bảng `Customers`.

### 🔑 Cách Đăng Nhập

#### **API (Postman / REST Client)**
```http
POST https://localhost:7282/api/auth/login
Content-Type: application/json

{
  "userName": "admin",
  "password": "admin123"
}
```

**Phản Hồi:**
```json
{
  "message": "Đăng nhập thành công.",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAtUtc": "2026-05-27T12:34:56Z",
    "userName": "admin",
    "role": "Admin"
  }
}
```

#### **Web MVC**
- Truy cập: http://localhost:5250
- Nhấp vào **Đăng Nhập**
- Nhập Tên Người Dùng & Mật Khẩu

#### **Ứng Dụng Desktop**
- Chạy ứng dụng
- Hộp thoại đăng nhập sẽ hiển thị
- Nhập thông tin đăng nhập

---

## 🔌 API Endpoints

### 📌 URL Cơ Bản
```
https://localhost:7282/api
```

### 🔐 Xác Thực

```http
POST /api/auth/login
```

### 🛣️ Tuyến Tàu

```http
GET    /api/routes                    # Lấy tất cả tuyến
POST   /api/routes                    # Tạo tuyến mới
GET    /api/routes/{id}               # Lấy tuyến theo ID
PUT    /api/routes/{id}               # Cập nhật tuyến
DELETE /api/routes/{id}               # Xóa tuyến
```

### 🚂 Chuyến Tàu

```http
GET    /api/train-trips
GET    /api/train-trips/search?departureStation=&arrivalStation=&departureDate=
GET    /api/train-trips/{id}
POST   /api/train-trips
PUT    /api/train-trips/{id}
DELETE /api/train-trips/{id}
GET    /api/train-trips/{id}/available-seats
```

### 👥 Khách Hàng

```http
GET    /api/customers                 # Lấy tất cả khách
POST   /api/customers                 # Tạo khách mới
GET    /api/customers/{id}            # Lấy khách theo ID
PUT    /api/customers/{id}            # Cập nhật khách
DELETE /api/customers/{id}            # Xóa khách
```

### 🎫 Vé Tàu

```http
GET    /api/tickets
GET    /api/tickets/search?keyword=&ticketStatus=&paymentStatus=
GET    /api/tickets/{id}
POST   /api/tickets
PUT    /api/tickets/{id}
PUT    /api/tickets/{id}/cancel
POST   /api/tickets/{id}/pay
DELETE /api/tickets/{id}
```

### 📊 Bảng Điều Khiển

```http
GET    /api/dashboard/stats
GET    /api/dashboard/seat-map/{trainTripId}
GET    /api/dashboard/reports/export?fromDate=&toDate=
GET    /api/dashboard/audit-logs
```

---

## 🏗️ Kiến Trúc

### 📐 Mô Hình Kiến Trúc

```
┌─────────────────────────────────────────────────────────────┐
│                    Tầng Trình Bày (UI)                       │
├──────────────────────┬──────────────────────┬───────────────┤
│   Web MVC (Razor)    │   REST API (JSON)    │  Ứng Dụng     │
│   Port: 5250         │   Port: 5216         │  Desktop      │
│                      │                      │  (WinForms)   │
└──────────────────────┴──────────────────────┴───────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                   Tầng Logic Kinh Doanh                      │
│  ┌─────────────────────────────────────────────────────┐    │
│  │          Services (Interfaces)                      │    │
│  │  ├─ AuthService        ├─ TicketService            │    │
│  │  ├─ CustomerService    ├─ RouteService             │    │
│  │  ├─ TrainTripService   └─ DashboardService         │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                            ▼
┌───────────────Tầng Truy Cập Dữ Liệu──────────────────────────┐
│                  Data Access Layer (DAL)                    │
│  ┌─────────────────────────────────────────────────────┐    │
│  │          Repositories (Interfaces)                  │    │
│  │  ├─ RouteRepository    ├─ TicketRepository         │    │
│  │  ├─ CustomerRepository ├─ AuditLogRepository       │    │
│  │  └─ TrainTripRepository                            │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│              Entity Framework Core DbContext                │
│             (AppDbContext)                                   │
└─────────────────────────────────────────────────────────────┘
                            ▼
┌───────────────────Cơ Sở Dữ Liệu SQL Server───────────────────────┐
│                   SQL Server Database                       │
│  TrainTicketManagementDB                                    │
│  ├─ Routes          ├─ Tickets       ├─ AspNetUsers        │
│  ├─ TrainTrips      ├─ Customers     ├─ AspNetRoles        │
│  └─ AuditLogs       └─ Migrations                           │
└─────────────────────────────────────────────────────────────┘
```
Luồng Dữ Liệu

```
Yêu Cầu Người Dùng
    ▼
[Controller] → Xác Thực Đầu Vào
    ▼
[Service] → Logic Kinh Doanh
    ▼
[Repository] → Truy Vấn Cơ Sở Dữ Liệu
    ▼
[DbContext] → Ánh Xạ Entity Framework
    ▼
[SQL Server] → Thực Thi Truy Vấn
    ▼
Phản Hồi (DTO) → Trả Lại Người Dùng
Response (DTO) → Return to User
```

---
Chi Tiết

### **TrainTicketApi - Cấu Trúc Backend**

#### Controllers/ (Các Bộ Điều Khiển)
#### Controllers/
```
AuthController.cs          → Xử lý đăng nhập, JWT
CustomersController.cs     → CRUD khách hàng
DashboardController.cs     → API thống kê
RoutesController.cs        → Quản lý tuyến tàu
TicketsController.cs       → Quản lý vé tàu
TrainTripsController.cs    → Quản lý chuyến tàu
```
 (Dữ Liệu)
```
AppDbContext.cs           → Entity Framework Core context
DbSeeder.cs               → Tạo dữ liệu mẫu, tài khoản test
```

#### Models/ (Mô Hình)
#### Models/
```
TrainRoute.cs             → Mô hình tuyến tàu
TrainTrip.cs              → Mô hình chuyến tàu
Customer.cs               → Mô hình khách hàng
Ticket.cs                 → Mô hình vé tàu
AuditLog.cs               → Mô hình nhật ký
```
 (Đối Tượng Truyền Dữ Liệu)
```
Auth/                     → DTOs xác thực
Customers/                → DTOs khách hàng
Dashboard/                → DTOs bảng điều khiển
Routes/                   → DTOs tuyến tàu
Tickets/                  → DTOs vé tàu
TrainTrips/               → DTOs chuyến tàu
```

#### Repositories/ (Kho Lưu Trữ)
```
Interfaces/               → Interface các repository
[Implementations]         → Các lớp repository cụ thể
```

#### Services/ (Các Dịch Vụ)
```
Interfaces/               → Interface các service
[Implementations]         → Các lớp service cụ thể
```

#### Migrations/ (Di Chuyển)
```
[Timestamp]_*.cs          → Tệp di chuyển Entity Framework
AppDbContextModelSnapshot.cs → Ảnh chụp schema database
AppDbContextModelSnapshot.cs → Database schema snapshot
```

---
Sơ Đồ Cơ Sở Dữ Liệu

### **Bảng & Mối Quan Hệ
### **Tables & Relationships**

```sql
┌─────────────────────┐ (Tuyến Tàu)
├─────────────────────┤
│ RouteId (PK)        │
│ DepartureStation    │ (Ga Đi)
│ ArrivalStation      │ (Ga Đến)
│ DistanceKm          │ (Khoảng Cách)
│ Status              │ (Trạng Thái)
│ CreatedAt           │ (Tạo Lúc)
│ UpdatedAt           │ (Cập Nhật Lúc)
│ UpdatedAt           │
└─────────────────────┘
         │
         ▼
┌─────────────────────┐ (Chuyến Tàu)
├─────────────────────┤
│ TrainTripId (PK)    │
│ TrainCode           │ (Mã Tàu)
│ RouteId (FK)        │
│ DepartureTime       │ (Thời Gian Xuất Phát)
│ ArrivalTime         │ (Thời Gian Đến)
│ TotalSeats          │ (Tổng Chỗ)
│ AvailableSeats      │ (Chỗ Còn Trống)
│ BaseTicketPrice     │ (Giá Vé Cơ Bản)
│ Status              │ (Trạng Thái)
│ Status              │
└─────────────────────┘
         │
         ▼
┌─────────────────────┐ (Vé Tàu)
├─────────────────────┤
│ TicketId (PK)       │
│ TrainTripId (FK)    │
│ CustomerId (FK)     │
│ SeatNumber          │ (Số Chỗ)
│ Price               │ (Giá)
│ BookingDate         │ (Ngày Đặt)
│ Status              │ (Trạng Thái)
│ Status              │
└─────────────────────┘
         ▲
         │
┌─────────────────────┐ (Khách Hàng)
├─────────────────────┤
│ CustomerId (PK)     │
│ UserId (FK)         │
│ FullName            │ (Tên Đầy Đủ)
│ Phone               │ (Điện Thoại)
│ Email               │ (Email)
│ Address             │ (Địa Chỉ)
│ CreatedAt           │ (Tạo Lúc)
│ CreatedAt           │
└─────────────────────┘

┌─────────────────────┐ (Nhật Ký)
├─────────────────────┤
│ AuditLogId (PK)     │
│ UserId (FK)         │
│ Action              │ (Hành Động)
│ TableName           │ (Tên Bảng)
│ RecordId            │ (ID Bản Ghi)
│ OldValues           │ (Giá Trị Cũ)
│ NewValues           │ (Giá Trị Mới)
│ Timestamp           │ (Thời Điểm)
│ Timestamp           │
└─────────────────────┘

┌─────────────────────┐Người Dùng - ASP.NET Identity)
├─────────────────────┤
│ Id (PK)             │
│ UserName            │ (Tên Người Dùng)
│ Email               │ (Email)
│ PasswordHash        │ (Hash Mật Khẩu)
│ SecurityStamp       │ (Dấu Bảo Mật)
└─────────────────────┘

┌─────────────────────┐
│    AspNetRoles      │ (Vai Trò - ASP.NET Identity)
├─────────────────────┤
│ Id (PK)             │
│ Name                │ (Tên Vai Trò)
│ NormalizedName      │ (Tên Chuẩn Hóa)
│ NormalizedName      │
└─────────────────────┘
```

---
Bảo Mật & Xác Thực

### **Cấu Hình JWT
### **JWT Configuration**
```json
"Jwt": {
  "Key": "TrainTicketManagementSuperSecretKey2026!",
  "Issuer": "TrainTicketApi",
  "Audience": "TrainTicketWinForms",
  "ExpireMinutes": 480
}
```
Chính Sách Mật Khẩu**
- Độ dài tối thiểu: 6 ký tự
- Không yêu cầu số, chữ hoa, ký tự đặc biệt
- Có thể điều chỉnh trong `Program.cs` → Cấu hình Identity

### **Hết Hạn Token**
- **480 phút** (8 giờ)
- Tự động làm mới sau khi hết hạn

### **CORS & HTTPS**
- HTTPS bắt buộc trong Production
- Tin Tưởng Chứng Chỉg Production
- Trust Server Certificate được bật cho development

---
Khắc Phục Sự Cố
## 🐛 Troubleshooting (Khắc Phục Sự Cố)

### ❌ **Lỗi: "Connection string not found"**
```
✅ Kiểm tra: TrainTicketApi/appsettings.json
✅ Thay đổi connection string theo tên SQL Server instance của bạn
```

### ❌ **Lỗi: "Database does not exist"**
```bash
✅ Chạy: cd TrainTicketApi && dotnet ef database update
```

### ❌ **Lỗi: "Port already in use"**
```bash
✅ Thay đổi port trong Properties/launchSettings.json
```

### ❌ **Lỗi: "SSL Certificate Invalid"**
```bash
✅ Thêm trong appsettings.json: "TrustServerCertificate=True"
```

### ❌ **Lỗi: "CORS policy"**
```
✅ Kiểm tra CORS configurcác origin được phépgram.cs
✅ Thêm frontend URL vào allowed origins
```

---


| Công Cụ | Mục Đích | Liên Kết |
|---------|---------|---------|
| **Swagger UI** | Tài Liệu API | https://localhost:7282/swagger |
| **SQL Server Management Studio** | Quản Lý Cơ Sở Dữ Liệu | https://ssms.info |
| **Visual Studio 2022** | Môi Trường Phát Triển | https://visualstudio.microsoft.com |
| **Postman** | Kiểm Tra API | https://postman.com |
| **Entity Framework Core Docs** | Tài Liệu ORM | https://docs.microsoft.com/ef/core |
| **.NET Documentation** | Tài Liệu Frameworkcumentation | https://docs.microsoft.com/ef/core |
| **.NET Documentation** | Framework Docs | https://docs.microsoft.com/dotnet |

---

## 📝 Ghi Chú Quan Trọng
Cơ Sở Dữ Liệu**: Sử dụng SQL Server Express cục bộ
2. **Xác Thực**: JWT Bearer tokens cho API
3. **CORS**: Cấu hình cho các URL frontend
4. **Migrations**: Được quản lý bằng Entity Framework Core
5. **Ghi Nhật Ký**: Console logging được kích hoạt trong Development
6. **Seed Data**: Tự động tạo người dùng test khi cơ sở dữ liệu được khởi tạo

---

## 👨‍💻 Hỗ Trợ & Liên Hệ

Nếu gặp sự cố:
1. Kiểm tra lại chuỗi kết nối cơ sở dữ liệu
2. Chạy `dotnet ef database update` để tạo cơ sở dữ liệu
3. Xem SQL Server logs
4. Kiểm tra cài đặt tường lửa

---

**Lần Cập Nhật Cuối Cùng:** 6 Tháng 5, 2026
**Phiên Bản:** 1.0.0
**Trạng Thái:** Đang Phát Triển Tích Cực
**Status:** Active Development ✅

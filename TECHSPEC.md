# EzChat 기술 스펙 문서

## 기술 스택 개요

### 백엔드 프레임워크
- **ASP.NET Web Forms**
  - .NET Framework 4.8
  - 페이지 기반 개발 모델 (.aspx / .aspx.cs)

### 데이터베이스
- **Microsoft SQL Server**
  - 버전: SQL Server 2019+ / LocalDB (개발용)
  - ADO.NET (SqlConnection, SqlCommand, SqlParameter)

### 인증/인가
- **Forms Authentication**
  - 역할 기반 접근 제어
  - 비밀번호 해싱: PBKDF2

---

## 프로젝트 구조

```
EzChat/
├── EzChat.sln
├── EzChat.Web/
│   ├── App_Code/
│   │   ├── BLL/
│   │   │   ├── SecurityHelper.cs
│   │   │   ├── UserBLL.cs
│   │   │   ├── BoardBLL.cs
│   │   │   ├── ChatBLL.cs
│   │   │   └── IpBanBLL.cs
│   │   ├── DAL/
│   │   │   └── DatabaseHelper.cs
│   │   └── Models/
│   │       └── User.cs
│   ├── Admin/
│   │   ├── Default.aspx(.cs)
│   │   ├── Users.aspx(.cs)
│   │   ├── IpBans.aspx(.cs)
│   │   ├── Rooms.aspx(.cs)
│   │   └── AuditLogs.aspx(.cs)
│   ├── Account/
│   │   ├── Login.aspx(.cs)
│   │   └── Register.aspx(.cs)
│   ├── Board/
│   │   ├── Default.aspx(.cs)
│   │   ├── Detail.aspx(.cs)
│   │   ├── Create.aspx(.cs)
│   │   └── Edit.aspx(.cs)
│   ├── Chat/
│   │   ├── Default.aspx(.cs)
│   │   ├── Room.aspx(.cs)
│   │   └── Create.aspx(.cs)
│   ├── MasterPages/
│   │   └── Site.Master(.cs)
│   ├── Content/
│   │   ├── css/site.css
│   │   └── js/site.js
│   ├── Default.aspx(.cs)
│   ├── Web.config
│   ├── Global.asax(.cs)
│   └── EzChat.Web.csproj
└── 문서/
```

---

## 데이터베이스 스키마

### 테이블 구조

#### Users
- Id (PK, INT, IDENTITY)
- Email (NVARCHAR(100), UNIQUE)
- PasswordHash (NVARCHAR(256))
- DisplayName (NVARCHAR(50))
- IsAdmin (BIT)
- IsActive (BIT)
- CreatedAt (DATETIME)
- LastLoginAt (DATETIME, NULL)

#### ChatRooms
- Id (PK, INT, IDENTITY)
- Name (NVARCHAR(100))
- Description (NVARCHAR(500))
- CreatedById (FK -> Users)
- CreatedAt (DATETIME)
- IsActive (BIT)
- MaxUsers (INT)

#### ChatMessages
- Id (PK, INT, IDENTITY)
- RoomId (FK -> ChatRooms)
- UserId (FK -> Users)
- Content (NVARCHAR(2000))
- SentAt (DATETIME)
- IsDeleted (BIT)

#### BoardPosts
- Id (PK, INT, IDENTITY)
- Title (NVARCHAR(200))
- Content (NVARCHAR(MAX))
- AuthorId (FK -> Users)
- CreatedAt (DATETIME)
- UpdatedAt (DATETIME, NULL)
- ViewCount (INT)
- IsDeleted (BIT)

#### Comments
- Id (PK, INT, IDENTITY)
- PostId (FK -> BoardPosts)
- AuthorId (FK -> Users)
- Content (NVARCHAR(1000))
- CreatedAt (DATETIME)
- IsDeleted (BIT)

#### IpBans
- Id (PK, INT, IDENTITY)
- IpAddress (NVARCHAR(45))
- Reason (NVARCHAR(500))
- BannedById (FK -> Users)
- BannedAt (DATETIME)
- ExpiresAt (DATETIME, NULL)
- IsActive (BIT)

#### AuditLogs
- Id (PK, INT, IDENTITY)
- AdminId (FK -> Users)
- Action (NVARCHAR(100))
- TargetType (NVARCHAR(50))
- TargetId (NVARCHAR(100))
- Details (NVARCHAR(1000))
- Timestamp (DATETIME)
- IpAddress (NVARCHAR(45))

---

## 개발 환경 설정

### 필수 도구
- Visual Studio 2022
- .NET Framework 4.8
- SQL Server 2019+ / LocalDB
- Git

### 연결 문자열 (Web.config)
```xml
<connectionStrings>
  <add name="EzChatConnection"
       connectionString="Server=(localdb)\mssqllocaldb;Database=EzChatDb;Trusted_Connection=True;"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

---

## 페이지 구조

### 인증
- `/Account/Login.aspx` - 로그인
- `/Account/Register.aspx` - 회원가입

### 관리자
- `/Admin/Default.aspx` - 대시보드
- `/Admin/Users.aspx` - 사용자 관리
- `/Admin/IpBans.aspx` - IP 차단 관리
- `/Admin/Rooms.aspx` - 채팅방 관리
- `/Admin/AuditLogs.aspx` - 감사 로그

### 게시판
- `/Board/Default.aspx` - 게시글 목록
- `/Board/Detail.aspx?id=` - 게시글 상세
- `/Board/Create.aspx` - 게시글 작성
- `/Board/Edit.aspx?id=` - 게시글 수정

### 채팅
- `/Chat/Default.aspx` - 채팅방 목록
- `/Chat/Room.aspx?id=` - 채팅방
- `/Chat/Create.aspx` - 채팅방 생성

---

## 보안 설정 (Web.config)

### Forms Authentication
```xml
<authentication mode="Forms">
  <forms loginUrl="~/Account/Login.aspx" timeout="30" protection="All" />
</authentication>
```

### 폴더별 권한
```xml
<location path="Admin">
  <system.web>
    <authorization>
      <allow roles="Admin" />
      <deny users="*" />
    </authorization>
  </system.web>
</location>
```

### 보안 헤더
```xml
<httpProtocol>
  <customHeaders>
    <add name="X-Content-Type-Options" value="nosniff" />
    <add name="X-Frame-Options" value="DENY" />
    <add name="X-XSS-Protection" value="1; mode=block" />
  </customHeaders>
</httpProtocol>
```

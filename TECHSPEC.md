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
│   │   │   └── BoardBLL.cs
│   │   ├── DAL/
│   │   │   └── DatabaseHelper.cs
│   │   └── Models/
│   │       └── User.cs
│   ├── Admin/
│   │   └── Default.aspx(.cs)
│   ├── Account/
│   │   ├── Login.aspx(.cs)
│   │   └── Register.aspx(.cs)
│   ├── Board/
│   │   ├── Default.aspx(.cs)
│   │   ├── Detail.aspx(.cs)
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
| 컬럼명 | 타입 | 제약 |
|--------|------|------|
| UserID | INT | PK, IDENTITY |
| Username | NVARCHAR(50) | NOT NULL |
| UserLoginID | NVARCHAR(100) | NOT NULL, UNIQUE |
| PasswordHash | NVARCHAR(256) | NOT NULL |
| IsAdmin | BIT | DEFAULT 0 |

#### Posts
| 컬럼명 | 타입 | 제약 |
|--------|------|------|
| PostID | INT | PK, IDENTITY |
| UserID | INT | FK → Users.UserID |
| Title | NVARCHAR(200) | NOT NULL |
| Content | NVARCHAR(MAX) | NOT NULL |
| CreatedAt | DATETIME | DEFAULT GETUTCDATE() |

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
- `/Admin/Default.aspx` - 관리자 페이지 (사용자/게시글 탭)

### 게시판
- `/Board/Default.aspx` - 게시글 목록
- `/Board/Detail.aspx?id=` - 게시글 상세/수정
- `/Board/Create.aspx` - 게시글 작성

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

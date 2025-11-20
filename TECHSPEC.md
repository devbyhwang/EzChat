# EzChat 기술 스펙 문서

## 기술 스택 개요

### 백엔드 프레임워크
- **ASP.NET MVC 5** / **ASP.NET Core MVC**
  - 선택: ASP.NET Core 8.0 (최신 LTS)
  - 이유: 보안 패치 지원, 크로스 플랫폼, 성능 향상

### 데이터베이스
- **Microsoft SQL Server**
  - 버전: SQL Server 2019+ / LocalDB (개발용)
  - ORM: Entity Framework Core 8.0
  - 마이그레이션: EF Core Migrations

### 실시간 통신
- **SignalR**
  - 채팅 기능을 위한 WebSocket 기반 실시간 통신
  - 자동 폴백 지원 (Long Polling)

### 인증/인가
- **ASP.NET Core Identity**
  - 역할 기반 접근 제어 (RBAC)
  - 비밀번호 해싱: PBKDF2 (Identity 기본값)

---

## 프로젝트 구조

```
EzChat/
├── EzChat.sln
├── src/
│   └── EzChat.Web/
│       ├── Controllers/
│       │   ├── AccountController.cs
│       │   ├── AdminController.cs
│       │   ├── BoardController.cs
│       │   ├── ChatController.cs
│       │   └── HomeController.cs
│       ├── Models/
│       │   ├── ApplicationUser.cs
│       │   ├── ChatRoom.cs
│       │   ├── ChatMessage.cs
│       │   ├── BoardPost.cs
│       │   ├── Comment.cs
│       │   └── IpBan.cs
│       ├── ViewModels/
│       ├── Views/
│       ├── Hubs/
│       │   └── ChatHub.cs
│       ├── Data/
│       │   └── ApplicationDbContext.cs
│       ├── Services/
│       │   ├── IAuthService.cs
│       │   ├── IAdminService.cs
│       │   ├── IBoardService.cs
│       │   └── IChatService.cs
│       ├── Middleware/
│       │   └── IpBanMiddleware.cs
│       ├── wwwroot/
│       │   ├── css/
│       │   ├── js/
│       │   └── lib/
│       ├── appsettings.json
│       └── Program.cs
├── docs/
│   ├── PLAN.md
│   ├── TECHSPEC.md
│   ├── SECURITY.md
│   └── AGENT.md
└── tests/
    └── EzChat.Tests/
```

---

## NuGet 패키지

### 필수 패키지
```xml
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
<PackageReference Include="Microsoft.AspNetCore.SignalR" Version="1.1.0" />
```

### 보안 패키지
```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.Cookies" Version="2.2.0" />
<PackageReference Include="HtmlSanitizer" Version="8.0.0" />
```

---

## 데이터베이스 스키마

### 테이블 구조

#### AspNetUsers (Identity 확장)
- Id (PK)
- UserName
- Email
- PasswordHash
- IsAdmin
- CreatedAt
- LastLoginAt
- IsActive

#### ChatRooms
- Id (PK)
- Name
- CreatedBy (FK -> AspNetUsers)
- CreatedAt
- IsActive

#### ChatMessages
- Id (PK)
- RoomId (FK -> ChatRooms)
- UserId (FK -> AspNetUsers)
- Content
- SentAt

#### BoardPosts
- Id (PK)
- Title
- Content
- AuthorId (FK -> AspNetUsers)
- CreatedAt
- UpdatedAt
- ViewCount

#### Comments
- Id (PK)
- PostId (FK -> BoardPosts)
- AuthorId (FK -> AspNetUsers)
- Content
- CreatedAt

#### IpBans
- Id (PK)
- IpAddress
- Reason
- BannedBy (FK -> AspNetUsers)
- BannedAt
- ExpiresAt (nullable)

---

## 개발 환경 설정

### 필수 도구
- Visual Studio 2022 / VS Code
- .NET 8.0 SDK
- SQL Server 2019+ / LocalDB
- Git

### 로컬 개발 설정
```bash
# 프로젝트 생성
dotnet new mvc -n EzChat.Web -o src/EzChat.Web

# 패키지 설치
cd src/EzChat.Web
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools

# 데이터베이스 마이그레이션
dotnet ef migrations add InitialCreate
dotnet ef database update

# 실행
dotnet run
```

### 연결 문자열 (개발용)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EzChatDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

---

## API 엔드포인트

### 인증
- `POST /Account/Login` - 로그인
- `POST /Account/Logout` - 로그아웃
- `POST /Account/Register` - 회원가입

### 관리자
- `GET /Admin/Dashboard` - 대시보드
- `POST /Admin/BanIp` - IP 차단
- `POST /Admin/UnbanIp` - IP 차단 해제
- `DELETE /Admin/DeleteUser/{id}` - 사용자 삭제
- `DELETE /Admin/DeleteRoom/{id}` - 채팅방 삭제

### 게시판
- `GET /Board` - 게시글 목록
- `GET /Board/Detail/{id}` - 게시글 상세
- `POST /Board/Create` - 게시글 작성
- `PUT /Board/Edit/{id}` - 게시글 수정
- `DELETE /Board/Delete/{id}` - 게시글 삭제

### 채팅
- `GET /Chat` - 채팅방 목록
- `GET /Chat/Room/{id}` - 채팅방 입장
- `POST /Chat/CreateRoom` - 채팅방 생성

---

## 버전 관리
- Git Flow 전략 사용
- 메인 브랜치: main
- 개발 브랜치: develop
- 기능 브랜치: feature/*

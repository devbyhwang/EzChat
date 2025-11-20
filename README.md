# EzChat

ASP.NET Web Forms 기반의 커뮤니티 플랫폼으로, 실시간 채팅과 게시판 기능을 제공합니다.

## 주요 기능

- **사용자 인증**: 회원가입, 로그인, 세션 관리
- **채팅**: 채팅방 생성/삭제, 실시간 메시지, 메시지 기록
- **게시판**: 글 작성/수정/삭제, 댓글, 검색, 페이지네이션
- **관리자 패널**: 대시보드, 사용자 관리, IP 차단, 감사 로그

## 기술 스택

| 구분 | 기술 |
|------|------|
| Backend | ASP.NET Web Forms (.NET Framework 4.8), C# |
| Database | SQL Server / LocalDB |
| Data Access | ADO.NET (파라미터화된 쿼리) |
| Authentication | Forms Authentication |
| Password | PBKDF2 해싱 (10,000 iterations) |
| Frontend | ASPX, Custom CSS, Vanilla JavaScript |

## 시작하기

### 필수 조건

- Visual Studio 2022 (또는 이후 버전)
- .NET Framework 4.8
- SQL Server 2019+ 또는 LocalDB

### 설치 및 실행

1. **저장소 클론**
   ```bash
   git clone https://github.com/devbyhwang/EzChat.git
   cd EzChat
   ```

2. **솔루션 열기**
   ```
   Visual Studio에서 EzChat.sln 열기
   ```

3. **빌드**
   ```
   Build > Build Solution (Ctrl+Shift+B)
   ```

4. **실행**
   ```
   F5 키를 눌러 IIS Express로 실행
   ```

5. **데이터베이스**
   - 애플리케이션 첫 실행 시 자동으로 초기화됩니다
   - 연결 문자열은 `Web.config`에서 설정

### 기본 관리자 계정

- **Email**: `admin@ezchat.local`
- **Password**: `Admin@123!`

> 프로덕션 환경에서는 반드시 기본 비밀번호를 변경하세요.

## 프로젝트 구조

```
EzChat/
├── EzChat.sln                    # 솔루션 파일
└── EzChat.Web/                   # 메인 웹 애플리케이션
    ├── Account/                  # 인증 (Login, Register)
    ├── Admin/                    # 관리자 패널
    ├── Board/                    # 게시판
    ├── Chat/                     # 채팅
    ├── App_Code/                 # 서버 로직
    │   ├── BLL/                  # 비즈니스 로직
    │   ├── DAL/                  # 데이터 접근
    │   └── Models/               # 데이터 모델
    ├── Content/                  # CSS, JS
    ├── MasterPages/              # 마스터 페이지
    └── Web.config                # 설정 파일
```

## 아키텍처

3계층 아키텍처를 따릅니다:

```
Presentation Layer (ASPX Pages)
         ↓
Business Logic Layer (BLL)
         ↓
Data Access Layer (DAL)
         ↓
SQL Server Database
```

## 보안

- **SQL 인젝션 방지**: 모든 쿼리에 `SqlParameter` 사용
- **XSS 방지**: `SecurityHelper.SanitizeInput()` 적용
- **비밀번호 보안**: PBKDF2 해싱 (SHA256, 10,000 iterations)
- **인증**: Forms Authentication with encrypted tickets
- **권한 관리**: 역할 기반 접근 제어 (Admin, User)

## 데이터베이스 스키마

| 테이블 | 설명 |
|--------|------|
| Users | 사용자 계정 |
| ChatRooms | 채팅방 |
| ChatMessages | 채팅 메시지 |
| BoardPosts | 게시글 |
| Comments | 댓글 |
| IpBans | IP 차단 목록 |
| AuditLogs | 감사 로그 |

## 설정

### Web.config 주요 설정

```xml
<!-- 연결 문자열 -->
<connectionStrings>
  <add name="EzChatConnection"
       connectionString="Server=(localdb)\mssqllocaldb;Database=EzChatDb;..."
       providerName="System.Data.SqlClient" />
</connectionStrings>

<!-- 앱 설정 -->
<appSettings>
  <add key="DefaultAdminEmail" value="admin@ezchat.local" />
  <add key="SessionTimeout" value="30" />
</appSettings>
```

## 문서

- [TECHSPEC.md](TECHSPEC.md) - 기술 사양
- [SECURITY.md](SECURITY.md) - 보안 정책
- [PLAN.md](PLAN.md) - 개발 계획
- [AGENT.md](AGENT.md) - AI 개발 가이드라인

## 라이선스

이 프로젝트는 MIT 라이선스를 따릅니다. 자세한 내용은 [LICENSE](LICENSE) 파일을 참조하세요.

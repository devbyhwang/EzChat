# CLAUDE.md

이 파일은 Claude Code가 EzChat 프로젝트를 이해하는 데 필요한 컨텍스트를 제공합니다.

## 프로젝트 개요

EzChat은 ASP.NET Web Forms 기반의 커뮤니티 플랫폼으로, 실시간 채팅과 게시판 기능을 제공합니다.

## 기술 스택

- **Backend**: ASP.NET Web Forms (.NET Framework 4.8), C#
- **Database**: SQL Server / LocalDB (ADO.NET, 파라미터화된 쿼리)
- **Authentication**: Forms Authentication (PBKDF2 비밀번호 해싱)
- **Frontend**: ASPX, Custom CSS, Vanilla JavaScript

## 빌드 및 실행 명령

```bash
# Visual Studio에서 솔루션 열기
# EzChat.sln

# 빌드 (Visual Studio)
Build > Build Solution (Ctrl+Shift+B)

# 또는 명령줄에서
msbuild EzChat.sln

# 실행
# Visual Studio에서 F5 (IIS Express로 실행)
```

### 기본 관리자 계정
- Email: `admin@ezchat.local`
- Password: `Admin@123!`

## 프로젝트 구조

```
EzChat/
├── EzChat.sln                    # 솔루션 파일
└── EzChat.Web/                   # 메인 웹 애플리케이션
    ├── Account/                  # 인증 페이지 (Login, Register)
    ├── Admin/                    # 관리자 패널 (역할 제한)
    ├── Board/                    # 게시판 기능
    ├── Chat/                     # 채팅 기능
    ├── App_Code/                 # 서버 로직
    │   ├── BLL/                  # 비즈니스 로직 레이어
    │   ├── DAL/                  # 데이터 접근 레이어
    │   └── Models/               # 데이터 모델
    ├── Content/                  # 정적 리소스 (CSS, JS)
    ├── MasterPages/              # 마스터 페이지
    ├── Web.config                # 애플리케이션 설정
    └── Global.asax               # 애플리케이션 생명주기
```

## 아키텍처

3계층 아키텍처:
- **Presentation Layer**: ASPX 페이지 + Code-Behind
- **Business Logic Layer**: `App_Code/BLL/*.cs` (정적 메서드)
- **Data Access Layer**: `App_Code/DAL/DatabaseHelper.cs`

## 주요 기능

1. **인증 시스템**: 회원가입, 로그인, 세션 관리
2. **채팅**: 채팅방 생성/삭제, 메시지 기록
3. **게시판**: CRUD, 댓글, 검색, 페이지네이션
4. **관리자 패널**: 대시보드, 사용자/IP 관리, 감사 로그

## 코딩 컨벤션

- **클래스/메서드**: PascalCase (`UserBLL`, `GetUserByEmail`)
- **변수**: camelCase (`userName`, `roomId`)
- **컨트롤 ID**: 접두사 + 이름 (`btnLogin`, `txtEmail`, `lblError`)
- **모든 SQL**: 파라미터화된 쿼리 사용 필수

## 보안 가이드라인

- SQL 인젝션 방지: 항상 `SqlParameter` 사용
- XSS 방지: `SecurityHelper.SanitizeInput()` 사용
- 비밀번호: PBKDF2 해싱 (10,000 iterations)
- 인증: Forms Authentication with encrypted tickets

## 데이터베이스

8개 테이블: Users, ChatRooms, ChatMessages, BoardPosts, Comments, IpBans, AuditLogs

데이터베이스는 애플리케이션 첫 실행 시 자동 초기화됩니다 (`Global.asax.cs`).

## 설정 파일

- `Web.config`: 연결 문자열, 앱 설정, 인증 설정
- 연결 문자열 이름: `EzChatConnection`

## 관련 문서

- `TECHSPEC.md`: 기술 사양
- `SECURITY.md`: 보안 정책
- `PLAN.md`: 개발 계획
- `AGENT.md`: AI 개발 가이드라인

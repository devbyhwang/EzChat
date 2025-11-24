# EzChat - ASP.NET Web Forms 게시판 애플리케이션

ASP.NET Web Forms (.NET Framework 4.8) 기반의 게시판 애플리케이션입니다.
사용자 인증, 게시판, 관리자 기능을 포함합니다.

---

## 주요 기능

- **회원 관리**: 회원가입, 로그인, 로그아웃
- **게시판**: 게시글 작성, 수정, 삭제, 검색, 페이지네이션
- **관리자**: 사용자 관리, 게시글 관리 (탭 방식)
- **보안**: PBKDF2 비밀번호 해싱, SQL Injection 방지, XSS 방지

---

## 기술 스택

| 구분 | 기술 |
|------|------|
| 프레임워크 | ASP.NET Web Forms (.NET Framework 4.8) |
| 데이터베이스 | Microsoft SQL Server / LocalDB |
| 인증 | Forms Authentication |
| 데이터 접근 | ADO.NET (SqlParameter) |

---

## Visual Studio로 실행하기

### 1. 필수 요구사항

- **Visual Studio 2022** (또는 2019)
  - ASP.NET 및 웹 개발 워크로드 설치 필요
- **.NET Framework 4.8** 개발자 팩
- **SQL Server 2019+** 또는 **LocalDB** (Visual Studio와 함께 설치됨)

### 2. 프로젝트 열기

1. Visual Studio를 실행합니다.
2. **파일** → **열기** → **프로젝트/솔루션** 선택
3. `EzChat.sln` 파일을 선택하여 엽니다.

```
EzChat/
└── EzChat.sln  ← 이 파일 선택
```

### 3. 연결 문자열 확인

`EzChat.Web/Web.config` 파일에서 데이터베이스 연결 문자열을 확인합니다:

```xml
<connectionStrings>
  <add name="EzChatConnection"
       connectionString="Server=(localdb)\mssqllocaldb;Database=EzChatDb;Trusted_Connection=True;MultipleActiveResultSets=true"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

> **참고**: 기본값은 LocalDB를 사용합니다. 별도의 SQL Server를 사용하려면 연결 문자열을 수정하세요.

### 4. 프로젝트 실행

#### 방법 1: F5 (디버그 모드)
- **F5** 키를 눌러 디버그 모드로 실행
- 브레이크포인트 설정 및 디버깅 가능

#### 방법 2: Ctrl+F5 (디버그 없이 실행)
- **Ctrl+F5** 키를 눌러 디버그 없이 실행
- 더 빠르게 시작됨

#### 방법 3: 메뉴 사용
- **디버그** → **디버깅 시작** (또는 **디버깅하지 않고 시작**)

### 5. 브라우저에서 확인

실행 후 브라우저가 자동으로 열리며 기본 URL은 다음과 같습니다:
```
http://localhost:[포트번호]/
```

---

## 초기 설정

### 데이터베이스 자동 생성

애플리케이션 첫 실행 시 `Global.asax`의 `Application_Start`에서 데이터베이스와 테이블이 자동으로 생성됩니다.

### 기본 관리자 계정

| 항목 | 값 |
|------|------|
| 아이디 | `admin` |
| 비밀번호 | `Admin@123!` |

> **보안 주의**: 프로덕션 환경에서는 반드시 관리자 비밀번호를 변경하세요!

관리자 계정 설정은 `Web.config`에서 변경 가능합니다:
```xml
<appSettings>
  <add key="AdminLoginID" value="admin" />
  <add key="AdminPassword" value="Admin@123!" />
</appSettings>
```

---

## 프로젝트 구조

```
EzChat/
├── EzChat.sln                    # 솔루션 파일
├── EzChat.Web/
│   ├── App_Code/
│   │   ├── BLL/                  # 비즈니스 로직 계층
│   │   │   ├── SecurityHelper.cs # 보안 유틸리티
│   │   │   ├── UserBLL.cs        # 사용자 비즈니스 로직
│   │   │   └── BoardBLL.cs       # 게시판 비즈니스 로직
│   │   ├── DAL/                  # 데이터 접근 계층
│   │   │   └── DatabaseHelper.cs # DB 헬퍼
│   │   └── Models/               # 데이터 모델
│   │       └── User.cs           # User, Post 모델
│   ├── Admin/                    # 관리자 페이지
│   │   └── Default.aspx          # 사용자/게시글 관리 (탭)
│   ├── Account/                  # 인증 페이지
│   │   ├── Login.aspx            # 로그인
│   │   └── Register.aspx         # 회원가입
│   ├── Board/                    # 게시판 페이지
│   │   ├── Default.aspx          # 게시글 목록
│   │   ├── Detail.aspx           # 게시글 상세/수정
│   │   └── Create.aspx           # 게시글 작성
│   ├── MasterPages/
│   │   └── Site.Master           # 마스터 페이지 (레이아웃)
│   ├── Content/
│   │   ├── css/site.css          # 스타일시트
│   │   └── js/site.js            # JavaScript
│   ├── Default.aspx              # 메인 페이지
│   ├── Web.config                # 웹 설정
│   └── Global.asax               # 애플리케이션 이벤트
├── PLAN.md                       # 개발 계획
├── TECHSPEC.md                   # 기술 스펙
└── SECURITY.md                   # 보안 정책
```

---

## 페이지 URL

| 페이지 | URL | 설명 |
|--------|-----|------|
| 홈 | `/` | 메인 페이지 |
| 로그인 | `/Account/Login.aspx` | 로그인 |
| 회원가입 | `/Account/Register.aspx` | 회원가입 |
| 게시판 | `/Board/Default.aspx` | 게시글 목록 |
| 게시글 상세 | `/Board/Detail.aspx?id=1` | 게시글 보기/수정 |
| 게시글 작성 | `/Board/Create.aspx` | 새 게시글 작성 |
| 관리자 | `/Admin/Default.aspx` | 관리자 페이지 |

---

## 문제 해결

### LocalDB 연결 오류

1. Visual Studio Installer에서 **SQL Server Express LocalDB** 설치 확인
2. 명령 프롬프트에서 LocalDB 상태 확인:
   ```cmd
   sqllocaldb info mssqllocaldb
   ```
3. LocalDB 인스턴스 시작:
   ```cmd
   sqllocaldb start mssqllocaldb
   ```

### .NET Framework 4.8 오류

1. [.NET Framework 4.8 개발자 팩](https://dotnet.microsoft.com/download/dotnet-framework/net48) 다운로드 및 설치
2. Visual Studio 재시작

### 포트 충돌

`EzChat.Web/Properties/launchSettings.json` 또는 프로젝트 속성에서 포트 번호 변경

---

## 보안 참고사항

- **비밀번호**: PBKDF2 (100,000 iterations) 해싱 사용
- **SQL Injection**: ADO.NET SqlParameter 사용으로 방지
- **XSS**: Server.HtmlEncode()로 출력 인코딩
- **접근 제어**: Forms Authentication + 역할 기반 권한

자세한 내용은 [SECURITY.md](SECURITY.md)를 참조하세요.

---

## 라이선스

이 프로젝트는 학습 및 개인 프로젝트 목적으로 제작되었습니다.

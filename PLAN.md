# EzChat 개발 계획서

## 프로젝트 개요
ASP.NET Web Forms (.NET Framework 4.8) 기반의 게시판 애플리케이션으로, 관리자 패널을 포함합니다.

---

## 전체 할일 목록

### Phase 1: 프로젝트 초기 설정
- [x] 프로젝트 문서화 (PLAN.md, TECHSPEC.md, SECURITY.md)
- [x] ASP.NET Web Forms 프로젝트 구조 생성
- [x] SQL Server 데이터베이스 스키마 설계
- [x] 데이터 접근 계층 (DAL) 구현

### Phase 2: 인증 시스템
- [x] 사용자 모델 생성
- [x] 로그인/로그아웃 기능 구현 (Forms Authentication)
- [x] 비밀번호 해싱 (PBKDF2)
- [x] 세션 관리 (30분 타임아웃)

### Phase 3: 관리자 기능
- [x] 관리자 역할 기반 접근 제어
- [x] 사용자 계정 삭제
- [x] 게시글 삭제
- [x] 관리자 대시보드 (탭 방식)

### Phase 4: 게시판 기능
- [x] 게시글 CRUD
- [x] 페이지네이션
- [x] 검색 기능
- [x] XSS 방지 처리 (HtmlEncode)

### Phase 5: 보안 강화
- [x] SQL Injection 방지 (매개변수화된 쿼리)
- [x] XSS 방지 (HtmlEncode)
- [x] 입력 유효성 검사
- [x] 보안 헤더 설정

---

## 현재 작업 상태

### 개발 완료 (간소화 버전)

#### 완료된 기능
- **인증 시스템**: Forms Authentication 기반 로그인/회원가입
- **관리자 기능**: 사용자 관리, 게시글 관리 (탭 방식)
- **게시판**: 글 작성/수정/삭제, 검색, 페이지네이션

---

## 로컬 실행 방법

### 필수 요구사항
- Visual Studio 2022
- .NET Framework 4.8
- SQL Server 2019+ 또는 LocalDB

### 설정 단계
1. Visual Studio에서 `EzChat.sln` 열기
2. `Web.config`에서 연결 문자열 확인/수정
3. F5로 실행 (IIS Express)

### 기본 관리자 계정
- 아이디: admin
- 비밀번호: Admin@123!

**중요**: 프로덕션 환경에서는 반드시 기본 관리자 비밀번호를 변경하세요!

---

## 프로젝트 구조
```
EzChat/
├── EzChat.sln
├── EzChat.Web/
│   ├── App_Code/
│   │   ├── BLL/          # 비즈니스 로직
│   │   │   ├── SecurityHelper.cs
│   │   │   ├── UserBLL.cs
│   │   │   └── BoardBLL.cs
│   │   ├── DAL/          # 데이터 접근
│   │   │   └── DatabaseHelper.cs
│   │   └── Models/       # 모델
│   │       └── User.cs
│   ├── Admin/            # 관리자 페이지 (.aspx)
│   │   └── Default.aspx  # 탭 방식 관리 페이지
│   ├── Account/          # 인증 페이지 (.aspx)
│   │   ├── Login.aspx
│   │   └── Register.aspx
│   ├── Board/            # 게시판 페이지 (.aspx)
│   │   ├── Default.aspx  # 게시글 목록
│   │   ├── Detail.aspx   # 게시글 상세 (수정 포함)
│   │   └── Create.aspx   # 게시글 작성
│   ├── MasterPages/      # 마스터 페이지
│   │   └── Site.Master
│   ├── Content/          # CSS, JS
│   ├── Default.aspx      # 메인 페이지
│   ├── Web.config
│   └── Global.asax
└── 문서 (PLAN.md, TECHSPEC.md, SECURITY.md)
```

---

## 데이터베이스 구조

### Users 테이블
| 컬럼명 | 타입 | 설명 |
|--------|------|------|
| UserID | INT (PK) | 사용자 ID |
| Username | NVARCHAR(50) | 사용자 이름 |
| UserLoginID | NVARCHAR(100) | 로그인 ID (UNIQUE) |
| PasswordHash | NVARCHAR(256) | 비밀번호 해시 |
| IsAdmin | BIT | 관리자 여부 |

### Posts 테이블
| 컬럼명 | 타입 | 설명 |
|--------|------|------|
| PostID | INT (PK) | 게시글 ID |
| UserID | INT (FK) | 작성자 ID |
| Title | NVARCHAR(200) | 제목 |
| Content | NVARCHAR(MAX) | 내용 |
| CreatedAt | DATETIME | 작성일 |

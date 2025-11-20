# EzChat 개발 계획서

## 프로젝트 개요
ASP.NET Core 8.0 기반의 채팅 애플리케이션으로, 관리자 패널과 게시판 기능을 포함합니다.

---

## 전체 할일 목록

### Phase 1: 프로젝트 초기 설정
- [x] 프로젝트 문서화 (PLAN.md, TECHSPEC.md, SECURITY.md, AGENT.md)
- [x] ASP.NET MVC 프로젝트 구조 생성
- [x] SQL Server 데이터베이스 스키마 설계
- [x] Entity Framework 설정

### Phase 2: 인증 시스템
- [x] 사용자 모델 생성 (ApplicationUser)
- [x] 로그인/로그아웃 기능 구현
- [x] 비밀번호 해싱 (PBKDF2 - Identity 기본)
- [x] 세션 관리 (30분 타임아웃)
- [x] CSRF 토큰 구현

### Phase 3: 관리자 기능
- [x] 관리자 역할 기반 접근 제어
- [x] IP 차단 기능
- [x] 사용자 계정 삭제/비활성화
- [x] 채팅방 삭제
- [x] 관리자 대시보드
- [x] 감사 로그

### Phase 4: 게시판 기능
- [x] 게시글 CRUD
- [x] 댓글 기능
- [x] 페이지네이션
- [x] 검색 기능
- [x] XSS 방지 처리 (HtmlSanitizer)

### Phase 5: 채팅 기능
- [x] SignalR 실시간 통신 설정
- [x] 채팅방 생성/삭제
- [x] 메시지 전송/수신
- [x] 사용자 입장/퇴장 알림
- [x] 채팅 기록 저장

### Phase 6: 보안 강화
- [x] SQL Injection 방지 (Entity Framework 매개변수화 쿼리)
- [x] XSS 방지 (HtmlSanitizer, Razor 기본 인코딩)
- [x] CSRF 방지 (ValidateAntiForgeryToken)
- [x] 입력 유효성 검사 (Data Annotations)
- [x] 보안 헤더 설정
- [x] 로깅 및 감사

### Phase 7: 테스트 및 마무리
- [ ] 단위 테스트
- [ ] 통합 테스트
- [ ] 보안 테스트
- [x] 문서화 완료

---

## 현재 작업 상태

### 1차 개발 완료

#### 완료된 기능
- **인증 시스템**: 로그인, 회원가입, 로그아웃
- **관리자 기능**: IP 차단, 사용자 관리, 채팅방 관리, 감사 로그
- **게시판**: 글 작성/수정/삭제, 댓글, 검색, 페이지네이션
- **채팅**: 실시간 채팅, 채팅방 생성/삭제

#### 다음 단계 (2차 개발)
- [ ] 단위 테스트 작성
- [ ] UI/UX 개선
- [ ] 프로필 기능
- [ ] 알림 기능
- [ ] 배포 환경 설정

---

## 작업 기록

### 2024-11-20
- 1차 개발 완료
- 프로젝트 구조 생성
- 모든 핵심 기능 구현
  - 인증 시스템 (ASP.NET Core Identity)
  - 관리자 패널 (대시보드, 사용자/IP/채팅방 관리)
  - 게시판 (CRUD, 댓글, 검색)
  - 실시간 채팅 (SignalR)
- 보안 기능 적용
  - XSS 방지 (HtmlSanitizer)
  - CSRF 토큰
  - SQL Injection 방지
  - 보안 헤더

---

## 로컬 실행 방법

```bash
# 프로젝트 디렉토리로 이동
cd src/EzChat.Web

# 패키지 복원
dotnet restore

# 데이터베이스 마이그레이션 생성
dotnet ef migrations add InitialCreate

# 데이터베이스 업데이트
dotnet ef database update

# 실행
dotnet run
```

### 기본 관리자 계정
- 이메일: admin@ezchat.local
- 비밀번호: Admin@123!

**중요**: 프로덕션 환경에서는 반드시 기본 관리자 비밀번호를 변경하세요!

---

## 우선순위
1. **완료**: 인증 시스템, 관리자 기능, 게시판, 채팅
2. **다음**: 테스트, UI 개선
3. **향후**: 배포, 추가 기능

## 예상 일정
- Phase 1-6: 완료 (1차 개발)
- Phase 7: 2차 개발 예정

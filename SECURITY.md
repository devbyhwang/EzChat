# EzChat 보안 정책 문서

## 보안 개요

이 문서는 EzChat 프로젝트의 보안 정책과 구현 가이드라인을 정의합니다.
OWASP Top 10을 기준으로 주요 취약점을 방지합니다.

---

## OWASP Top 10 대응 전략

### 1. SQL Injection 방지 (A03:2021)

#### 매개변수화된 쿼리 사용
- **필수**: ADO.NET의 SqlParameter 사용
- **금지**: 문자열 연결을 통한 SQL 쿼리 생성

```csharp
// 안전한 방법
string query = "SELECT * FROM Users WHERE UserLoginID = @UserLoginID";
DatabaseHelper.ExecuteQuery(query, new SqlParameter("@UserLoginID", loginId));

// 위험한 방법 (사용 금지)
string query = $"SELECT * FROM Users WHERE UserLoginID = '{loginId}'";
```

---

### 2. 인증 보안 (A07:2021)

#### 비밀번호 정책
- 최소 8자 이상
- 대문자, 소문자, 숫자, 특수문자 중 3가지 이상 포함
- PBKDF2 해싱

```csharp
public static string HashPassword(string password)
{
    using (var deriveBytes = new Rfc2898DeriveBytes(password, 16, 100000))
    {
        byte[] salt = deriveBytes.Salt;
        byte[] hash = deriveBytes.GetBytes(32);
        // ...
    }
}
```

#### 세션 관리
- 세션 타임아웃: 30분
- 로그인 시 세션 정보 저장
- 로그아웃 시 세션 완전 파기

---

### 3. XSS 방지 (A03:2021)

#### 출력 인코딩
- Server.HtmlEncode() 사용
- 사용자 입력 표시 시 항상 인코딩

```csharp
litContent.Text = Server.HtmlEncode(post.Content).Replace("\n", "<br/>");
```

#### 입력 검증
```csharp
public static string SanitizeInput(string input)
{
    if (string.IsNullOrEmpty(input)) return input;
    return HttpUtility.HtmlEncode(input.Trim());
}
```

---

### 4. 접근 제어 (A01:2021)

#### 역할 기반 접근 제어
```xml
<!-- Web.config -->
<location path="Admin">
  <system.web>
    <authorization>
      <allow roles="Admin" />
      <deny users="*" />
    </authorization>
  </system.web>
</location>
```

#### 리소스 소유권 검증
```csharp
// 작성자 또는 관리자만 수정/삭제 가능
if (post.UserID == userId || isAdmin)
{
    btnEdit.Visible = true;
    btnDelete.Visible = true;
}
```

---

### 5. 보안 헤더 설정 (A05:2021)

```xml
<httpProtocol>
  <customHeaders>
    <add name="X-Content-Type-Options" value="nosniff" />
    <add name="X-Frame-Options" value="DENY" />
    <add name="X-XSS-Protection" value="1; mode=block" />
    <add name="Referrer-Policy" value="strict-origin-when-cross-origin" />
  </customHeaders>
</httpProtocol>
```

---

## 관리자 보안

### 관리자 계정 보호
- 기본 관리자 계정 비밀번호 변경 필수
- 관리자 계정은 별도 생성 (회원가입으로 생성 불가)
- 관리자만 사용자 삭제 및 게시글 삭제 가능

### 기본 관리자 계정
- 아이디: admin
- 비밀번호: Admin@123!

**주의**: 프로덕션 환경에서는 반드시 비밀번호를 변경하세요!

---

## 입력 유효성 검사

### 서버 측 검증 필수
```csharp
public static bool Register(string userLoginId, string password, string username, out string errorMessage)
{
    if (string.IsNullOrWhiteSpace(userLoginId) || userLoginId.Length < 4)
    {
        errorMessage = "아이디는 4자 이상이어야 합니다.";
        return false;
    }

    if (!ValidatePasswordPolicy(password, out errorMessage))
    {
        return false;
    }
    // ...
}
```

---

## 보안 체크리스트

### 배포 전 확인사항
- [x] 모든 입력 검증 구현
- [x] XSS 방지 처리
- [x] SQL Injection 방지 확인
- [x] 적절한 HTTP 보안 헤더 설정
- [x] 관리자 접근 제어 확인
- [ ] HTTPS 강제 (프로덕션 환경)
- [ ] 에러 메시지에 민감 정보 노출 금지

---

## 참고 자료
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [ASP.NET Security Best Practices](https://docs.microsoft.com/en-us/aspnet/web-forms/overview/security/)

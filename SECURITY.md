# EzChat 보안 정책 문서

## 보안 개요

이 문서는 EzChat 프로젝트의 보안 정책과 구현 가이드라인을 정의합니다.
OWASP Top 10을 기준으로 모든 주요 취약점을 방지합니다.

---

## OWASP Top 10 대응 전략

### 1. Injection (A03:2021)

#### SQL Injection 방지
- **필수**: Entity Framework Core의 매개변수화된 쿼리만 사용
- **금지**: 문자열 연결을 통한 SQL 쿼리 생성

```csharp
// 안전한 방법
var user = await _context.Users
    .Where(u => u.Email == email)
    .FirstOrDefaultAsync();

// 위험한 방법 (사용 금지)
var query = $"SELECT * FROM Users WHERE Email = '{email}'";
```

#### Command Injection 방지
- 사용자 입력을 시스템 명령어에 직접 전달 금지
- 필요시 화이트리스트 검증 사용

---

### 2. Broken Authentication (A07:2021)

#### 비밀번호 정책
- 최소 8자 이상
- 대문자, 소문자, 숫자, 특수문자 중 3가지 이상 포함
- PBKDF2 해싱 (ASP.NET Core Identity 기본값)

```csharp
services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
});
```

#### 세션 관리
- 세션 타임아웃: 30분
- 로그인 시 세션 재생성
- 로그아웃 시 세션 완전 파기

#### 계정 잠금
- 5회 로그인 실패 시 15분 잠금
- 잠금 해제 후 실패 횟수 초기화

```csharp
options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
options.Lockout.MaxFailedAccessAttempts = 5;
options.Lockout.AllowedForNewUsers = true;
```

---

### 3. Cross-Site Scripting (XSS) (A03:2021)

#### 출력 인코딩
- Razor 뷰에서 기본 HTML 인코딩 사용 (`@Model.Content`)
- `@Html.Raw()` 사용 금지 (불가피한 경우 HtmlSanitizer 사용)

#### 입력 검증
```csharp
// HtmlSanitizer 사용
var sanitizer = new HtmlSanitizer();
var sanitizedContent = sanitizer.Sanitize(userInput);
```

#### Content Security Policy
```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy",
        "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'");
    await next();
});
```

---

### 4. Cross-Site Request Forgery (CSRF) (A01:2021)

#### Anti-Forgery Token
- 모든 POST/PUT/DELETE 요청에 CSRF 토큰 필수

```csharp
// Controller
[ValidateAntiForgeryToken]
[HttpPost]
public async Task<IActionResult> Create(PostViewModel model)

// View
<form method="post">
    @Html.AntiForgeryToken()
    ...
</form>
```

#### SameSite Cookie
```csharp
services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
```

---

### 5. Security Misconfiguration (A05:2021)

#### 환경별 설정 분리
- 개발/프로덕션 설정 분리
- 민감한 정보는 환경 변수 또는 Secret Manager 사용

```csharp
// 개발 환경에서만 상세 에러 표시
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
```

#### HTTP 보안 헤더
```csharp
app.UseHsts();
app.UseHttpsRedirection();

// 추가 보안 헤더
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});
```

---

### 6. Vulnerable Components (A06:2021)

#### 의존성 관리
- 정기적인 NuGet 패키지 업데이트
- `dotnet list package --vulnerable` 명령으로 취약점 검사
- 사용하지 않는 패키지 제거

---

### 7. Broken Access Control (A01:2021)

#### 역할 기반 접근 제어
```csharp
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    // 관리자만 접근 가능
}
```

#### 리소스 소유권 검증
```csharp
public async Task<IActionResult> Edit(int id)
{
    var post = await _context.Posts.FindAsync(id);

    // 작성자 또는 관리자만 수정 가능
    if (post.AuthorId != User.GetUserId() && !User.IsInRole("Admin"))
    {
        return Forbid();
    }

    return View(post);
}
```

---

## 관리자 보안

### 관리자 계정 보호
- 기본 관리자 계정 비밀번호 변경 필수
- 관리자 계정은 별도 생성 (회원가입으로 생성 불가)
- 관리자 작업 모든 로깅

### 관리자 기능 감사 로그
```csharp
public class AdminAuditLog
{
    public int Id { get; set; }
    public string AdminId { get; set; }
    public string Action { get; set; }
    public string TargetType { get; set; }
    public string TargetId { get; set; }
    public string Details { get; set; }
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; }
}
```

---

## IP 차단 시스템

### 구현
```csharp
public class IpBanMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        if (await _banService.IsIpBanned(ipAddress))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Access Denied");
            return;
        }

        await _next(context);
    }
}
```

### IP 차단 정보
- 차단 IP 주소
- 차단 사유
- 차단한 관리자
- 차단 일시
- 만료 일시 (영구 차단 시 null)

---

## 데이터 보호

### 민감 데이터 암호화
- 비밀번호: PBKDF2 해싱
- 연결 문자열: 환경 변수 또는 Azure Key Vault

### 데이터 백업
- 정기적인 데이터베이스 백업
- 백업 암호화

---

## 입력 유효성 검사

### 서버 측 검증 필수
```csharp
public class LoginViewModel
{
    [Required(ErrorMessage = "이메일은 필수입니다.")]
    [EmailAddress(ErrorMessage = "올바른 이메일 형식이 아닙니다.")]
    [StringLength(100)]
    public string Email { get; set; }

    [Required(ErrorMessage = "비밀번호는 필수입니다.")]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; }
}
```

### 파일 업로드 검증
- 파일 확장자 화이트리스트
- 파일 크기 제한
- MIME 타입 검증
- 파일 내용 검사

---

## 로깅 및 모니터링

### 보안 이벤트 로깅
- 로그인 성공/실패
- 권한 거부
- 관리자 작업
- 의심스러운 활동

### 로그 보호
- 민감 정보 마스킹 (비밀번호, 토큰 등)
- 로그 파일 접근 제한

```csharp
_logger.LogWarning("Failed login attempt for user {Email} from IP {IpAddress}",
    email, ipAddress);
```

---

## 보안 체크리스트

### 배포 전 확인사항
- [ ] 모든 입력 검증 구현
- [ ] CSRF 토큰 적용
- [ ] XSS 방지 처리
- [ ] SQL Injection 방지 확인
- [ ] 적절한 HTTP 보안 헤더 설정
- [ ] HTTPS 강제
- [ ] 에러 메시지에 민감 정보 노출 금지
- [ ] 관리자 접근 제어 확인
- [ ] 로깅 구현
- [ ] 의존성 취약점 검사

---

## 보안 업데이트 이력

| 날짜 | 내용 | 담당자 |
|------|------|--------|
| 2024-XX-XX | 초기 보안 정책 수립 | - |

---

## 참고 자료
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [ASP.NET Core Security](https://docs.microsoft.com/en-us/aspnet/core/security/)
- [OWASP Cheat Sheet Series](https://cheatsheetseries.owasp.org/)

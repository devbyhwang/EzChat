# EzChat 개발 에이전트 지침

## 개요

이 문서는 ASP.NET Core 개발을 위한 AI 에이전트 지침을 정의합니다.
코드 생성, 리뷰, 문제 해결 시 이 지침을 따릅니다.

---

## 프로젝트 컨텍스트

### 기본 정보
- **프로젝트명**: EzChat
- **프레임워크**: ASP.NET Core 8.0
- **데이터베이스**: Microsoft SQL Server
- **아키텍처**: MVC 패턴
- **개발 유형**: 1인 개발, 로컬 환경

### 주요 기능
1. 사용자 인증 (로그인/로그아웃)
2. 관리자 패널 (IP 차단, 사용자/방 관리)
3. 게시판
4. 실시간 채팅 (SignalR)

---

## 코드 작성 지침

### 일반 원칙
1. **보안 우선**: OWASP Top 10 취약점 방지
2. **SOLID 원칙 준수**
3. **DRY (Don't Repeat Yourself)**
4. **명확한 네이밍**: 의미 있는 변수/메서드명
5. **한국어 주석**: 복잡한 로직에 한국어 주석 추가

### 네이밍 규칙
```csharp
// 클래스: PascalCase
public class ChatMessage { }

// 메서드: PascalCase
public async Task SendMessageAsync() { }

// 변수: camelCase
var userName = "홍길동";

// 상수: UPPER_SNAKE_CASE
public const int MAX_MESSAGE_LENGTH = 1000;

// 프라이빗 필드: _camelCase
private readonly ILogger _logger;
```

### 비동기 프로그래밍
```csharp
// async/await 사용
public async Task<IActionResult> GetPostAsync(int id)
{
    var post = await _context.Posts.FindAsync(id);
    return View(post);
}

// 메서드명에 Async 접미사
public async Task<User> GetUserByIdAsync(int id);
```

---

## 보안 코드 패턴

### 필수 패턴

#### 1. 매개변수화된 쿼리 (Entity Framework)
```csharp
// 올바른 방법
var user = await _context.Users
    .FirstOrDefaultAsync(u => u.Email == email);

// 잘못된 방법 (절대 사용 금지)
var sql = $"SELECT * FROM Users WHERE Email = '{email}'";
```

#### 2. 입력 검증
```csharp
public class CreatePostViewModel
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; }

    [Required]
    [StringLength(10000)]
    public string Content { get; set; }
}
```

#### 3. 출력 인코딩
```csharp
// Razor에서 자동 인코딩 (기본)
<p>@Model.Content</p>

// HTML 직접 출력 필요시 (HtmlSanitizer 사용)
@Html.Raw(sanitizedContent)
```

#### 4. CSRF 보호
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(PostViewModel model)
{
    // ...
}
```

#### 5. 권한 검사
```csharp
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    public async Task<IActionResult> DeleteUser(string id)
    {
        // 관리자만 접근 가능
    }
}
```

---

## 아키텍처 패턴

### 계층 구조
```
Controllers/  - HTTP 요청 처리
Services/     - 비즈니스 로직
Models/       - 데이터 모델
ViewModels/   - 뷰 데이터 전송
Data/         - 데이터베이스 컨텍스트
```

### 의존성 주입
```csharp
// Program.cs
builder.Services.AddScoped<IBoardService, BoardService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IAdminService, AdminService>();

// Controller
public class BoardController : Controller
{
    private readonly IBoardService _boardService;

    public BoardController(IBoardService boardService)
    {
        _boardService = boardService;
    }
}
```

### 서비스 인터페이스 패턴
```csharp
public interface IBoardService
{
    Task<IEnumerable<Post>> GetPostsAsync(int page, int pageSize);
    Task<Post> GetPostByIdAsync(int id);
    Task<Post> CreatePostAsync(CreatePostDto dto, string userId);
    Task UpdatePostAsync(int id, UpdatePostDto dto);
    Task DeletePostAsync(int id);
}
```

---

## Entity Framework 패턴

### DbContext 설정
```csharp
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<ChatRoom> ChatRooms { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<BoardPost> BoardPosts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<IpBan> IpBans { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // 인덱스 설정
        builder.Entity<IpBan>()
            .HasIndex(b => b.IpAddress);
    }
}
```

### 마이그레이션
```bash
# 마이그레이션 생성
dotnet ef migrations add AddChatTables

# 데이터베이스 업데이트
dotnet ef database update
```

---

## SignalR 패턴

### Hub 구현
```csharp
public class ChatHub : Hub
{
    public async Task SendMessage(int roomId, string message)
    {
        var userId = Context.User.GetUserId();
        var sanitizedMessage = _sanitizer.Sanitize(message);

        // 메시지 저장
        await _chatService.SaveMessageAsync(roomId, userId, sanitizedMessage);

        // 방에 브로드캐스트
        await Clients.Group($"room_{roomId}")
            .SendAsync("ReceiveMessage", userId, sanitizedMessage);
    }

    public async Task JoinRoom(int roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{roomId}");
    }
}
```

---

## 에러 처리

### 전역 예외 처리
```csharp
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionFeature != null)
        {
            _logger.LogError(exceptionFeature.Error, "Unhandled exception");
        }

        context.Response.Redirect("/Home/Error");
    });
});
```

### 서비스 레벨 예외 처리
```csharp
public async Task<Post> GetPostByIdAsync(int id)
{
    var post = await _context.Posts.FindAsync(id);

    if (post == null)
    {
        throw new NotFoundException($"Post with id {id} not found");
    }

    return post;
}
```

---

## 테스트 지침

### 단위 테스트
```csharp
public class BoardServiceTests
{
    [Fact]
    public async Task CreatePost_WithValidData_ReturnsPost()
    {
        // Arrange
        var service = new BoardService(_mockContext.Object);
        var dto = new CreatePostDto { Title = "테스트", Content = "내용" };

        // Act
        var result = await service.CreatePostAsync(dto, "user1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("테스트", result.Title);
    }
}
```

---

## 문서화 지침

### 코드 주석
```csharp
/// <summary>
/// 게시글을 생성합니다.
/// </summary>
/// <param name="dto">게시글 생성 정보</param>
/// <param name="userId">작성자 ID</param>
/// <returns>생성된 게시글</returns>
/// <exception cref="ArgumentNullException">dto가 null인 경우</exception>
public async Task<Post> CreatePostAsync(CreatePostDto dto, string userId)
{
    // 입력 검증
    if (dto == null) throw new ArgumentNullException(nameof(dto));

    // 게시글 생성 로직
    var post = new Post
    {
        Title = dto.Title,
        Content = _sanitizer.Sanitize(dto.Content), // XSS 방지
        AuthorId = userId,
        CreatedAt = DateTime.UtcNow
    };

    _context.Posts.Add(post);
    await _context.SaveChangesAsync();

    return post;
}
```

---

## 개발 워크플로우

### 기능 개발 순서
1. 모델/엔티티 정의
2. DbContext에 DbSet 추가
3. 마이그레이션 생성 및 적용
4. 서비스 인터페이스 정의
5. 서비스 구현
6. ViewModel 정의
7. 컨트롤러 구현
8. 뷰 구현
9. 테스트 작성

### 커밋 메시지 형식
```
feat: 게시판 CRUD 기능 구현
fix: 로그인 시 세션 갱신 버그 수정
refactor: 채팅 서비스 구조 개선
docs: README 업데이트
test: 게시판 서비스 단위 테스트 추가
security: XSS 취약점 수정
```

---

## 금지 사항

### 절대 하지 말아야 할 것
1. **문자열 연결 SQL 쿼리** - SQL Injection 위험
2. **@Html.Raw() 무분별 사용** - XSS 위험
3. **하드코딩된 비밀번호/키** - 보안 취약점
4. **CSRF 토큰 생략** - CSRF 공격 위험
5. **try-catch로 예외 무시** - 디버깅 어려움
6. **동기 데이터베이스 호출** - 성능 저하

---

## 참고 자료

### 공식 문서
- [ASP.NET Core 문서](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/)

### 보안 가이드
- [OWASP ASP.NET Core Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/DotNet_Security_Cheat_Sheet.html)
- [ASP.NET Core Security Best Practices](https://docs.microsoft.com/en-us/aspnet/core/security/)

---

## 버전 이력

| 버전 | 날짜 | 변경 내용 |
|------|------|-----------|
| 1.0 | 2024-XX-XX | 초기 버전 |

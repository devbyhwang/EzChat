using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EzChat.Web.Models;

namespace EzChat.Web.Data;

/// <summary>
/// 애플리케이션 데이터베이스 컨텍스트
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ChatRoom> ChatRooms { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<BoardPost> BoardPosts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<IpBan> IpBans { get; set; }
    public DbSet<AdminAuditLog> AdminAuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // IpBan 인덱스 설정
        builder.Entity<IpBan>()
            .HasIndex(b => b.IpAddress);

        builder.Entity<IpBan>()
            .HasIndex(b => b.IsActive);

        // BoardPost 인덱스 설정
        builder.Entity<BoardPost>()
            .HasIndex(p => p.CreatedAt);

        builder.Entity<BoardPost>()
            .HasIndex(p => p.IsDeleted);

        // ChatMessage 인덱스 설정
        builder.Entity<ChatMessage>()
            .HasIndex(m => m.SentAt);

        // ChatRoom 인덱스 설정
        builder.Entity<ChatRoom>()
            .HasIndex(r => r.IsActive);

        // AdminAuditLog 인덱스 설정
        builder.Entity<AdminAuditLog>()
            .HasIndex(l => l.Timestamp);

        builder.Entity<AdminAuditLog>()
            .HasIndex(l => l.AdminId);

        // 관계 설정 - Cascade delete 제한
        builder.Entity<ChatMessage>()
            .HasOne(m => m.User)
            .WithMany(u => u.Messages)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ChatMessage>()
            .HasOne(m => m.Room)
            .WithMany(r => r.Messages)
            .HasForeignKey(m => m.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<BoardPost>()
            .HasOne(p => p.Author)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Comment>()
            .HasOne(c => c.Author)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Comment>()
            .HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ChatRoom>()
            .HasOne(r => r.CreatedBy)
            .WithMany(u => u.CreatedRooms)
            .HasForeignKey(r => r.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<IpBan>()
            .HasOne(b => b.BannedBy)
            .WithMany()
            .HasForeignKey(b => b.BannedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<AdminAuditLog>()
            .HasOne(l => l.Admin)
            .WithMany()
            .HasForeignKey(l => l.AdminId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

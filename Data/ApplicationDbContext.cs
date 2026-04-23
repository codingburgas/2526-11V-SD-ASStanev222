using Microsoft.EntityFrameworkCore;
using ProjectLMS.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ProjectLMS.Data;

/// <summary>
/// Entity Framework Core DbContext for the LMS application.
/// Configures all entities, relationships, and database constraints.
/// Follows code-first approach with fluent API configuration.
/// </summary>
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets for all entities
    public DbSet<Course> Courses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Test> Tests { get; set; }
    public DbSet<TestQuestion> TestQuestions { get; set; }
    public DbSet<TestAnswer> TestAnswers { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<StudentTestAttempt> StudentTestAttempts { get; set; }
    public DbSet<StudentAnswer> StudentAnswers { get; set; }

    /// <summary>
    /// Configures model relationships and constraints.
    /// - One-to-Many: Course → Lessons, Tests
    /// - Many-to-Many: User ↔ Course (via Enrollment)
    /// - Composite unique constraint: UserId + CourseId on Enrollment
    /// - Cascade delete policies for data integrity
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== USER - COURSE RELATIONSHIP (Many-to-Many via Enrollment) =====
        modelBuilder.Entity<Enrollment>()
            .HasKey(e => new { e.UserId, e.CourseId });

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // ===== COURSE - LESSON RELATIONSHIP (One-to-Many) =====
        modelBuilder.Entity<Lesson>()
            .HasOne(l => l.Course)
            .WithMany(c => c.Lessons)
            .HasForeignKey(l => l.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // ===== COURSE - TEST RELATIONSHIP (One-to-Many) =====
        modelBuilder.Entity<Test>()
            .HasOne(t => t.Course)
            .WithMany(c => c.Tests)
            .HasForeignKey(t => t.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // ===== TEST - QUESTION RELATIONSHIP (One-to-Many) =====
        modelBuilder.Entity<TestQuestion>()
            .HasOne(q => q.Test)
            .WithMany(t => t.Questions)
            .HasForeignKey(q => q.TestId)
            .OnDelete(DeleteBehavior.Cascade);

        // ===== QUESTION - ANSWER RELATIONSHIP (One-to-Many) =====
        modelBuilder.Entity<TestAnswer>()
            .HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // ===== TEST - STUDENT ATTEMPT RELATIONSHIP (One-to-Many) =====
        modelBuilder.Entity<StudentTestAttempt>()
            .HasOne(sta => sta.Test)
            .WithMany(t => t.StudentAttempts)
            .HasForeignKey(sta => sta.TestId)
            .OnDelete(DeleteBehavior.Cascade);

        // ===== STUDENT ANSWER RELATIONSHIPS =====
        modelBuilder.Entity<StudentAnswer>()
            .HasOne(sa => sa.TestAttempt)
            .WithMany(sta => sta.Answers)
            .HasForeignKey(sa => sa.TestAttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StudentAnswer>()
            .HasOne(sa => sa.Question)
            .WithMany(q => q.StudentAnswers)
            .HasForeignKey(sa => sa.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentAnswer>()
            .HasOne(sa => sa.SelectedAnswer)
            .WithMany()
            .HasForeignKey(sa => sa.SelectedAnswerId)
            .OnDelete(DeleteBehavior.Restrict);

        // ===== INDEXES FOR PERFORMANCE =====
        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => e.UserId)
            .HasDatabaseName("IX_Enrollment_UserId");

        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => e.CourseId)
            .HasDatabaseName("IX_Enrollment_CourseId");

        modelBuilder.Entity<Course>()
            .HasIndex(c => c.CreatedByUserId)
            .HasDatabaseName("IX_Course_CreatedByUserId");

        modelBuilder.Entity<Lesson>()
            .HasIndex(l => l.CourseId)
            .HasDatabaseName("IX_Lesson_CourseId");

        modelBuilder.Entity<Test>()
            .HasIndex(t => t.CourseId)
            .HasDatabaseName("IX_Test_CourseId");

        modelBuilder.Entity<StudentTestAttempt>()
            .HasIndex(sta => new { sta.UserId, sta.TestId })
            .HasDatabaseName("IX_StudentTestAttempt_UserId_TestId");
    }
}

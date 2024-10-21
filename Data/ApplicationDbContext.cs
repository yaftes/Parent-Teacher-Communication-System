using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,string> {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options) { }

    public DbSet<Course> Course {get;set;}
    public DbSet<Class> Class {get;set;}
    public DbSet<CourseStudent> CourseStudent {get;set;}
    public DbSet<CourseReport> CourseReport {get;set;}
    public DbSet<StudentPerformanceReport> StudentPerformanceReport {get;set;}
    public DbSet<Announcement> Announcement {get;set;} 
    public DbSet<ClassTeacher> ClassTeacher {get;set;}
    public DbSet<ChatMessage> ChatMessage {get;set;}


    protected override void OnModelCreating(ModelBuilder model){

         base.OnModelCreating(model);
         
         model.Entity<StudentPerformanceReport>()
        .HasOne(s => s.ApplicationUser)
        .WithMany(a => a.StudentPerformanceReport)
        .HasForeignKey(s => s.StudentId)
        .OnDelete(DeleteBehavior.Cascade);

        model.Entity<ClassTeacher>().HasKey( ct => new {ct.ClassId,ct.TeacherId});

        model.Entity<ClassTeacher>()
        .HasOne(ct => ct.Class)
        .WithMany( ct => ct.ClassTeacher)
        .HasForeignKey( ct => ct.ClassId)
        .OnDelete(DeleteBehavior.Cascade);

        model.Entity<ClassTeacher>()
        .HasOne(ct => ct.Teacher)
        .WithMany(ct => ct.ClassTeacher)
        .HasForeignKey(ct => ct.TeacherId)
        .OnDelete(DeleteBehavior.Cascade);

        model.Entity<Course>()
        .HasMany(cr => cr.CourseReports)
        .WithOne(c => c.Course)
        .HasForeignKey(c => c.CourseId)
        .OnDelete(DeleteBehavior.Cascade);


        
    }
    

}
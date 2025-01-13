using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using opensis.data.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.Models
{
    public class CRMContext : DbContext
    {
        private readonly DbContextOptions? contextOptions;


        public CRMContext() : base()
        {
        }

        public CRMContext(DbContextOptions options) : base(options)
        {
            this.contextOptions = options;
        }

        public virtual DbSet<AllCourseSectionView> AllCourseSectionView { get; set; } = null!;
        public virtual DbSet<ApiControllerKeyMapping> ApiControllerKeyMapping { get; set; } = null!;
        public virtual DbSet<ApiControllerList> ApiControllerList { get; set; } = null!;
        public virtual DbSet<ApiKeysMaster> ApiKeysMaster { get; set; } = null!;
        public virtual DbSet<Assignment> Assignment { get; set; } = null!;
        public virtual DbSet<AssignmentType> AssignmentType { get; set; } = null!;
        public virtual DbSet<AttendanceCode> AttendanceCode { get; set; } = null!;
        public virtual DbSet<AttendanceCodeCategories> AttendanceCodeCategories { get; set; } = null!;
        public virtual DbSet<BellSchedule> BellSchedule { get; set; } = null!;
        public virtual DbSet<Block> Block { get; set; } = null!;
        public virtual DbSet<BlockPeriod> BlockPeriod { get; set; } = null!;
        public virtual DbSet<CalendarEvents> CalendarEvents { get; set; } = null!;
        public virtual DbSet<City> City { get; set; } = null!;
        public virtual DbSet<Country> Country { get; set; } = null!;
        public virtual DbSet<Course> Course { get; set; } = null!;
        public virtual DbSet<CourseBlockSchedule> CourseBlockSchedule { get; set; } = null!;
        public virtual DbSet<CourseCalendarSchedule> CourseCalendarSchedule { get; set; } = null!;
        public virtual DbSet<CourseCommentCategory> CourseCommentCategory { get; set; } = null!;
        public virtual DbSet<CourseFixedSchedule> CourseFixedSchedule { get; set; } = null!;
        public virtual DbSet<CourseSection> CourseSection { get; set; } = null!;
        public virtual DbSet<CourseStandard> CourseStandard { get; set; } = null!;
        public virtual DbSet<CourseVariableSchedule> CourseVariableSchedule { get; set; } = null!;
        public virtual DbSet<CustomFields> CustomFields { get; set; } = null!;
        public virtual DbSet<CustomFieldsValue> CustomFieldsValue { get; set; } = null!;
        public virtual DbSet<DpdownValuelist> DpdownValuelist { get; set; } = null!;
        public virtual DbSet<EffortGradeLibraryCategory> EffortGradeLibraryCategory { get; set; } = null!;
        public virtual DbSet<EffortGradeLibraryCategoryItem> EffortGradeLibraryCategoryItem { get; set; } = null!;
        public virtual DbSet<EffortGradeScale> EffortGradeScale { get; set; } = null!;
        public virtual DbSet<FieldsCategory> FieldsCategory { get; set; } = null!;
        public virtual DbSet<Grade> Grade { get; set; } = null!;
        public virtual DbSet<GradeAgeRange> GradeAgeRange { get; set; } = null!;
        public virtual DbSet<GradeEducationalStage> GradeEducationalStage { get; set; } = null!;
        public virtual DbSet<GradeEquivalency> GradeEquivalency { get; set; } = null!;
        public virtual DbSet<GradeScale> GradeScale { get; set; } = null!;
        public virtual DbSet<GradeUsStandard> GradeUsStandard { get; set; } = null!;
        public virtual DbSet<GradebookConfiguration> GradebookConfiguration { get; set; } = null!;
        public virtual DbSet<GradebookConfigurationGradescale> GradebookConfigurationGradescale { get; set; } = null!;
        public virtual DbSet<GradebookConfigurationProgressPeriod> GradebookConfigurationProgressPeriods { get; set; } = null!;
        public virtual DbSet<GradebookConfigurationQuarter> GradebookConfigurationQuarter { get; set; } = null!;
        public virtual DbSet<GradebookConfigurationSemester> GradebookConfigurationSemester { get; set; } = null!;
        public virtual DbSet<GradebookConfigurationYear> GradebookConfigurationYear { get; set; } = null!;
        public virtual DbSet<GradebookGrades> GradebookGrades { get; set; } = null!;
        public virtual DbSet<Gradelevels> Gradelevels { get; set; } = null!;
        public virtual DbSet<HistoricalCreditTransfer> HistoricalCreditTransfer { get; set; } = null!;
        public virtual DbSet<HistoricalGrade> HistoricalGrade { get; set; } = null!;
        public virtual DbSet<HistoricalMarkingPeriod> HistoricalMarkingPeriod { get; set; } = null!;
        public virtual DbSet<HonorRolls> HonorRolls { get; set; } = null!;
        public virtual DbSet<Language> Language { get; set; } = null!;
        public virtual DbSet<LoginSession> LoginSession { get; set; } = null!;
        public virtual DbSet<Membership> Membership { get; set; } = null!;
        public virtual DbSet<Notice> Notice { get; set; } = null!;
        public virtual DbSet<ParentAddress> ParentAddress { get; set; } = null!;
        public virtual DbSet<ParentAssociationship> ParentAssociationship { get; set; } = null!;
        public virtual DbSet<ParentInfo> ParentInfo { get; set; } = null!;
        public virtual DbSet<ParentListView> ParentListView { get; set; } = null!;
        public virtual DbSet<PermissionCategory> PermissionCategory { get; set; } = null!;
        public virtual DbSet<PermissionGroup> PermissionGroup { get; set; } = null!;
        public virtual DbSet<PermissionSubcategory> PermissionSubcategory { get; set; } = null!;
        public virtual DbSet<Plans> Plans { get; set; } = null!;
        public virtual DbSet<Programs> Programs { get; set; } = null!;
        public virtual DbSet<ProgressPeriods> ProgressPeriods { get; set; } = null!;
        public virtual DbSet<Quarters> Quarters { get; set; } = null!;
        public virtual DbSet<ReleaseNumber> ReleaseNumber { get; set; } = null!;
        public virtual DbSet<RolePermission> RolePermission { get; set; } = null!;
        public virtual DbSet<Rooms> Rooms { get; set; } = null!;
        public virtual DbSet<ScheduledJob> ScheduledJobs { get; set; } = null!;
        public virtual DbSet<ScheduledJobHistory> ScheduledJobHistories { get; set; } = null!;
        public virtual DbSet<SchoolCalendars> SchoolCalendars { get; set; } = null!;
        public virtual DbSet<SchoolDetail> SchoolDetail { get; set; } = null!;
        public virtual DbSet<SchoolMaster> SchoolMaster { get; set; } = null!;
        public virtual DbSet<SchoolPreference> SchoolPreference { get; set; } = null!;
        public virtual DbSet<SchoolRollover> SchoolRollover { get; set; } = null!;
        public virtual DbSet<SchoolYears> SchoolYears { get; set; } = null!;
        public virtual DbSet<SearchFilter> SearchFilter { get; set; } = null!;
        public virtual DbSet<Sections> Sections { get; set; } = null!;
        public virtual DbSet<Semesters> Semesters { get; set; } = null!;
        public virtual DbSet<StaffCertificateInfo> StaffCertificateInfo { get; set; } = null!;
        public virtual DbSet<StaffCoursesectionSchedule> StaffCoursesectionSchedule { get; set; } = null!;
        public virtual DbSet<StaffMaster> StaffMaster { get; set; } = null!;
        public virtual DbSet<StaffScheduleView> StaffScheduleView { get; set; } = null!;
        public virtual DbSet<StaffSchoolInfo> StaffSchoolInfo { get; set; } = null!;
        public virtual DbSet<State> State { get; set; } = null!;
        public virtual DbSet<StudentAttendance> StudentAttendance { get; set; } = null!;
        public virtual DbSet<StudentAttendanceComments> StudentAttendanceComments { get; set; } = null!;
        public virtual DbSet<StudentAttendanceHistory> StudentAttendanceHistory { get; set; } = null!;
        public virtual DbSet<StudentComments> StudentComments { get; set; } = null!;
        public virtual DbSet<StudentCoursesectionSchedule> StudentCoursesectionSchedule { get; set; } = null!;
        public virtual DbSet<StudentDailyAttendance> StudentDailyAttendance { get; set; } = null!;
        public virtual DbSet<StudentDocuments> StudentDocuments { get; set; } = null!;
        public virtual DbSet<StudentEffortGradeDetail> StudentEffortGradeDetail { get; set; } = null!;
        public virtual DbSet<StudentEffortGradeMaster> StudentEffortGradeMaster { get; set; } = null!;
        public virtual DbSet<StudentEnrollment> StudentEnrollment { get; set; } = null!;
        public virtual DbSet<StudentEnrollmentCode> StudentEnrollmentCode { get; set; } = null!;
        public virtual DbSet<StudentFinalGrade> StudentFinalGrade { get; set; } = null!;
        public virtual DbSet<StudentFinalGradeComments> StudentFinalGradeComments { get; set; } = null!;
        public virtual DbSet<StudentFinalGradeStandard> StudentFinalGradeStandard { get; set; } = null!;
        public virtual DbSet<StudentListView> StudentListView { get; set; } = null!;
        public virtual DbSet<StudentMedicalListView> StudentMedicalListViews { get; set; } = null!;
        public virtual DbSet<StudentMaster> StudentMaster { get; set; } = null!;
        public virtual DbSet<StudentMedicalAlert> StudentMedicalAlert { get; set; } = null!;
        public virtual DbSet<StudentMedicalImmunization> StudentMedicalImmunization { get; set; } = null!;
        public virtual DbSet<StudentMedicalNote> StudentMedicalNote { get; set; } = null!;
        public virtual DbSet<StudentMedicalNurseVisit> StudentMedicalNurseVisit { get; set; } = null!;
        public virtual DbSet<StudentMedicalProvider> StudentMedicalProvider { get; set; } = null!;
        public virtual DbSet<StudentMissingAttendance> StudentMissingAttendances { get; set; } = null!;
        public virtual DbSet<StudentReportCardDetail> StudentReportCardDetail { get; set; } = null!;
        public virtual DbSet<StudentReportCardMaster> StudentReportCardMaster { get; set; } = null!;
        public virtual DbSet<StudentScheduleView> StudentScheduleView { get; set; } = null!;
        public virtual DbSet<StudentTranscriptDetail> StudentTranscriptDetail { get; set; } = null!;
        public virtual DbSet<StudentTranscriptMaster> StudentTranscriptMaster { get; set; } = null!;
        public virtual DbSet<Subject> Subject { get; set; } = null!;
        public virtual DbSet<UserAccessLog> UserAccessLog { get; set; } = null!;
        public virtual DbSet<UserMaster> UserMaster { get; set; } = null!;
        public virtual DbSet<UserSecretQuestions> UserSecretQuestions { get; set; } = null!;

       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_general_ci");
            modelBuilder.Entity<AllCourseSectionView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("all_course_section_view");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.AllowStudentConflict).HasColumnName("allow_student_conflict");

                entity.Property(e => e.AllowTeacherConflict).HasColumnName("allow_teacher_conflict");

                entity.Property(e => e.AttendanceCategoryId).HasColumnName("attendance_category_id");

                entity.Property(e => e.AttendanceTaken).HasColumnName("attendance_taken");

                entity.Property(e => e.BlockId).HasColumnName("block_id");

                entity.Property(e => e.BlockPeriodId).HasColumnName("block_period_id");

                entity.Property(e => e.BlockRoomId).HasColumnName("block_room_id");

                entity.Property(e => e.CalDate)
                    .HasColumnType("date")
                    .HasColumnName("cal_date");

                entity.Property(e => e.CalDay)
                    .HasMaxLength(30)
                    .HasColumnName("cal_day");

                entity.Property(e => e.CalPeriodId).HasColumnName("cal_period_id");

                entity.Property(e => e.CalRoomId).HasColumnName("cal_room_id");

                entity.Property(e => e.CalendarId).HasColumnName("calendar_id");

                entity.Property(e => e.CourseGradeLevel)
                    .HasMaxLength(50)
                    .HasColumnName("course_grade_level");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseProgram)
                    .HasMaxLength(100)
                    .HasColumnName("course_program");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.CourseSectionName)
                    .HasMaxLength(200)
                    .HasColumnName("course_section_name");

                entity.Property(e => e.CourseSubject)
                    .HasMaxLength(100)
                    .HasColumnName("course_subject");

                entity.Property(e => e.CourseTitle).HasColumnName("course_title");

                entity.Property(e => e.DurationEndDate)
                    .HasColumnType("date")
                    .HasColumnName("duration_end_date");

                entity.Property(e => e.DurationStartDate)
                    .HasColumnType("date")
                    .HasColumnName("duration_start_date");

                entity.Property(e => e.FixedDays)
                    .HasMaxLength(100)
                    .HasColumnName("fixed_days");

                entity.Property(e => e.FixedPeriodId).HasColumnName("fixed_period_id");

                entity.Property(e => e.FixedRoomId).HasColumnName("fixed_room_id");

                entity.Property(e => e.GradeScaleId).HasColumnName("grade_scale_id");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.QtrMarkingPeriodId).HasColumnName("qtr_marking_period_id");

                entity.Property(e => e.ScheduleType)
                    .HasMaxLength(25)
                    .HasColumnName("schedule_type");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.Seats).HasColumnName("seats");

                entity.Property(e => e.SmstrMarkingPeriodId).HasColumnName("smstr_marking_period_id");

                entity.Property(e => e.TakeAttendanceBlock).HasColumnName("take_attendance_block");

                entity.Property(e => e.TakeAttendanceCalendar).HasColumnName("take_attendance_calendar");

                entity.Property(e => e.TakeAttendanceVariable).HasColumnName("take_attendance_variable");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.VarDay)
                    .HasMaxLength(15)
                    .HasColumnName("var_day");

                entity.Property(e => e.VarPeriodId).HasColumnName("var_period_id");

                entity.Property(e => e.VarRoomId).HasColumnName("var_room_id");

                entity.Property(e => e.YrMarkingPeriodId).HasColumnName("yr_marking_period_id");

                entity.Property(e => e.PrgrsprdMarkingPeriodId).HasColumnName("prgrsprd_marking_period_id");
            });

            modelBuilder.Entity<ApiControllerKeyMapping>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.KeyId, e.ControllerId })
                    .HasName("PK_api_controller_key_mapping_tenant_id");

                entity.ToTable("api_controller_key_mapping");

                entity.HasIndex(e => new { e.TenantId, e.ControllerId }, "IX_api_controller_key_mapping_tenant_id_controller");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();
                   

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.KeyId).HasColumnName("key_id");

                entity.Property(e => e.ControllerId).HasColumnName("controller_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.ApiControllerList)
                    .WithMany(p => p.ApiControllerKeyMapping)
                    .HasForeignKey(d => new { d.TenantId, d.ControllerId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("api_controller_key_mapping$FK_api_controller_list");

                entity.HasOne(d => d.ApiKeysMaster)
                    .WithMany(p => p.ApiControllerKeyMapping)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.KeyId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("api_controller_key_mapping$FK_api_keys_master");
            });

            modelBuilder.Entity<ApiControllerList>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.ControllerId })
                    .HasName("PK_api_controller_list_tenant_id");

                entity.ToTable("api_controller_list");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength(); ;

                entity.Property(e => e.ControllerId).HasColumnName("controller_id");

                entity.Property(e => e.ApiTitle)
                    .HasMaxLength(250)
                    .HasColumnName("api_title");

                entity.Property(e => e.ControllerPath)
                    .HasMaxLength(500)
                    .HasColumnName("controller_path");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.Module)
                    .HasMaxLength(50)
                    .HasColumnName("module");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<ApiKeysMaster>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.KeyId })
                    .HasName("PK_api_keys_master_tenant_id");

                entity.ToTable("api_keys_master");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength(); ;

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.KeyId).HasColumnName("key_id");

                entity.Property(e => e.ApiKey).HasColumnName("api_key");

                entity.Property(e => e.ApiTitle)
                    .HasMaxLength(150)
                    .HasColumnName("api_title");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Emailaddress)
                    .HasMaxLength(150)
                    .HasColumnName("emailaddress");

                entity.Property(e => e.Expires)
                    .HasColumnType("date")
                    .HasColumnName("expires");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.Revoked).HasColumnName("revoked");

                entity.Property(e => e.Scopes).HasColumnName("scopes");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.AssignmentId })
                    .HasName("PK_assignment_tenant_id");

                entity.ToTable("assignment");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.AssignmentTypeId }, "IX_assignment_tenant_id_school_id_assignment_type");

                entity.HasIndex(e => new { e.TenantId, e.StaffId }, "IX_assignment_tenant_id_staff_id");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");

                entity.Property(e => e.AssignmentDate)
                    .HasColumnType("date")
                    .HasColumnName("assignment_date");

                entity.Property(e => e.AssignmentDescription).HasColumnName("assignment_description");

                entity.Property(e => e.AssignmentTitle)
                    .HasMaxLength(500)
                    .HasColumnName("assignment_title");

                entity.Property(e => e.AssignmentTypeId).HasColumnName("assignment_type_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.DueDate)
                    .HasColumnType("date")
                    .HasColumnName("due_date");

                entity.Property(e => e.Points).HasColumnName("points");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.StaffMaster)
                    .WithMany(p => p.Assignment)
                    .HasForeignKey(d => new { d.TenantId, d.StaffId })
                    .HasConstraintName("assignment$FK_assignment_staff_master");

                entity.HasOne(d => d.AssignmentType)
                    .WithMany(p => p.Assignment)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.AssignmentTypeId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("assignment$FK_assignment_assignment_type");
            });

            modelBuilder.Entity<AssignmentType>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.AssignmentTypeId })
                    .HasName("PK_assignment_type_tenant_id");

                entity.ToTable("assignment_type");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.AssignmentTypeId).HasColumnName("assignment_type_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");
                entity.Property(e => e.PrgrsprdMarkingPeriodId).HasColumnName("prgrsprd_marking_period_id");

                entity.Property(e => e.QtrMarkingPeriodId).HasColumnName("qtr_marking_period_id");

                entity.Property(e => e.SmstrMarkingPeriodId).HasColumnName("smstr_marking_period_id");

                entity.Property(e => e.YrMarkingPeriodId).HasColumnName("yr_marking_period_id");


                entity.Property(e => e.Title)
                    .HasMaxLength(250)
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.Weightage).HasColumnName("weightage");

                entity.HasOne(d => d.ProgressPeriod)
                    .WithMany(p => p.AssignmentTypes)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.PrgrsprdMarkingPeriodId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_assignment_type_progress_periods");

                entity.HasOne(d => d.Quarter)
                    .WithMany(p => p.AssignmentTypes)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.QtrMarkingPeriodId })
                    .HasConstraintName("FK_assignment_type_quarters");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.AssignmentTypes)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.SmstrMarkingPeriodId })
                    .HasConstraintName("FK_assignment_type_semesters");

                entity.HasOne(d => d.SchoolYear)
                    .WithMany(p => p.AssignmentTypes)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.YrMarkingPeriodId })
                    .HasConstraintName("FK_assignment_type_school_years");
            });

            modelBuilder.Entity<AttendanceCode>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.AttendanceCategoryId, e.AttendanceCode1 })
                    .HasName("PK_attendance_code_tenant_id");

                entity.ToTable("attendance_code");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.AttendanceCategoryId).HasColumnName("attendance_category_id");

                entity.Property(e => e.AttendanceCode1).HasColumnName("attendance_code");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.AllowEntryBy)
                    .HasMaxLength(50)
                    .HasColumnName("allow_entry_by");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.DefaultCode).HasColumnName("default_code");
                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.ShortName)
                    .HasMaxLength(10)
                    .HasColumnName("short_name");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.StateCode)
                    .HasMaxLength(8)
                    .HasColumnName("state_code");

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .HasColumnName("title");

                entity.Property(e => e.Type)
                    .HasMaxLength(10)
                    .HasColumnName("type");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.AttendanceCodeCategories)
                    .WithMany(p => p.AttendanceCode)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.AttendanceCategoryId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("attendance_code$FK_categories");
            });

            modelBuilder.Entity<AttendanceCodeCategories>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.AttendanceCategoryId })
                    .HasName("PK_attendance_code_categories_tenant_id");

                entity.ToTable("attendance_code_categories");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.AttendanceCategoryId).HasColumnName("attendance_category_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Title)
                    .HasMaxLength(150)
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.AttendanceCodeCategories)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("attendance_code_categories$FK_school");
            });

            modelBuilder.Entity<BellSchedule>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.AcademicYear, e.BellScheduleDate })
                    .HasName("PK_bell_schedule_tenant_id");

                entity.ToTable("bell_schedule");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.BellScheduleDate)
                    .HasColumnType("date")
                    .HasColumnName("bell_schedule_date");

                entity.Property(e => e.BlockId).HasColumnName("block_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<Block>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.BlockId })
                    .HasName("PK_block_tenant_id");

                entity.ToTable("block");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.BlockId).HasColumnName("block_id");
                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");


                entity.Property(e => e.BlockSortOrder).HasColumnName("block_sort_order");

                entity.Property(e => e.BlockTitle)
                    .HasMaxLength(200)
                    .HasColumnName("block_title");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.FullDayMinutes).HasColumnName("full_day_minutes");

                entity.Property(e => e.HalfDayMinutes).HasColumnName("half_day_minutes");
                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.Block)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("block$FK_block_school_master");
            });

            modelBuilder.Entity<BlockPeriod>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.BlockId, e.PeriodId })
                    .HasName("PK_block_period_tenant_id");

                entity.ToTable("block_period");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.BlockId).HasColumnName("block_id");

                entity.Property(e => e.PeriodId).HasColumnName("period_id");
                entity.Property(e => e.AcademicYear)
                   .HasColumnType("decimal(4, 0)")
                   .HasColumnName("academic_year");

                entity.Property(e => e.CalculateAttendance).HasColumnName("calculate_attendance");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.PeriodEndTime)
                    .HasMaxLength(8)
                    .HasColumnName("period_end_time")
                    .IsFixedLength();

                entity.Property(e => e.PeriodShortName)
                    .HasMaxLength(50)
                    .HasColumnName("period_short_name");

                entity.Property(e => e.PeriodSortOrder).HasColumnName("period_sort_order");

                entity.Property(e => e.PeriodStartTime)
                    .HasMaxLength(8)
                    .HasColumnName("period_start_time")
                    .IsFixedLength();

                entity.Property(e => e.PeriodTitle)
                    .HasMaxLength(200)
                    .HasColumnName("period_title");
                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.BlockPeriod)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("block_period$FK_block_period_school_master");
            });

            modelBuilder.Entity<CalendarEvents>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.CalendarId, e.EventId })
                    .HasName("PK_calendar_events_tenant_id");

                entity.ToTable("calendar_events");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.CalendarId).HasColumnName("calendar_id");

                entity.Property(e => e.EventId).HasColumnName("event_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.ApplicableToAllSchool).HasColumnName("applicable_to_all_school");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("end_date");

                entity.Property(e => e.EventColor)
                    .HasMaxLength(7)
                    .HasColumnName("event_color");

                entity.Property(e => e.IsHoliday).HasColumnName("is_holiday");

                entity.Property(e => e.SchoolDate)
                    .HasColumnType("date")
                    .HasColumnName("school_date");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_date");

                entity.Property(e => e.SystemWideEvent).HasColumnName("system_wide_event");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.VisibleToMembershipId)
                    .HasMaxLength(30)
                    .HasColumnName("visible_to_membership_id");
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("city");

                entity.HasIndex(e => e.StateId, "IX_city_stateid");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.StateId).HasColumnName("stateid");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.City)
                    .HasForeignKey(d => d.StateId)
                    .HasConstraintName("city$FK_city_state");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("country");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CountryCode)
                    .HasMaxLength(5)
                    .HasColumnName("countrycode");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.CourseId })
                    .HasName("PK_course_tenant_id");

                entity.ToTable("course");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseCategory)
                    .HasMaxLength(8)
                    .HasColumnName("course_category");

                entity.Property(e => e.CourseDescription).HasColumnName("course_description");

                entity.Property(e => e.CourseGradeLevel)
                    .HasMaxLength(50)
                    .HasColumnName("course_grade_level");

                entity.Property(e => e.CourseProgram)
                    .HasMaxLength(100)
                    .HasColumnName("course_program");

                entity.Property(e => e.CourseShortName)
                    .HasMaxLength(50)
                    .HasColumnName("course_short_name");

                entity.Property(e => e.CourseSubject)
                    .HasMaxLength(100)
                    .HasColumnName("course_subject");

                entity.Property(e => e.CourseTitle).HasColumnName("course_title");

                entity.Property(e => e.AcademicYear)
                   .HasColumnType("decimal(4, 0)")
                   .HasColumnName("academic_year");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.CreditHours).HasColumnName("credit_hours");

                entity.Property(e => e.IsCourseActive).HasColumnName("is_course_active");
                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.Standard)
                    .HasMaxLength(50)
                    .HasColumnName("standard");

                entity.Property(e => e.StandardRefNo)
                    .HasMaxLength(50)
                    .HasColumnName("standard_ref_no");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<CourseBlockSchedule>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.CourseId, e.CourseSectionId, e.Serial })
                    .HasName("PK_course_block_schedule_tenant_id");

                entity.ToTable("course_block_schedule");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.BlockId, e.PeriodId }, "IX_course_block_schedule_tenant_id_school_id_block");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.RoomId }, "IX_course_block_schedule_tenant_id_school_id_room");


                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.Serial).HasColumnName("serial");

                entity.Property(e => e.BlockId).HasColumnName("block_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.GradeScaleId).HasColumnName("grade_scale_id");

                entity.Property(e => e.PeriodId).HasColumnName("period_id");

                entity.Property(e => e.RoomId).HasColumnName("room_id");

                entity.Property(e => e.TakeAttendance).HasColumnName("take_attendance");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.Block)
                    .WithMany(p => p.CourseBlockSchedule)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.BlockId })
                    .HasConstraintName("course_block_schedule$FK_block");

                entity.HasOne(d => d.Rooms)
                    .WithMany(p => p.CourseBlockSchedule)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.RoomId })
                    .HasConstraintName("course_block_schedule$FK_rooms");

                entity.HasOne(d => d.BlockPeriod)
                    .WithMany(p => p.CourseBlockSchedule)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.BlockId, d.PeriodId })
                    .HasConstraintName("course_block_schedule$FK_periods");
            });

            modelBuilder.Entity<CourseCalendarSchedule>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.CourseId, e.CourseSectionId, e.Serial })
                    .HasName("PK_course_calendar_schedule_tenant_id");

                entity.ToTable("course_calendar_schedule");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.BlockId, e.PeriodId }, "IX_course_calendar_schedule_tenant_id_school_id_bl");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.RoomId }, "IX_course_calendar_schedule_tenant_id_school_id_ro");


                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.Serial).HasColumnName("serial");

                entity.Property(e => e.BlockId).HasColumnName("block_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .HasColumnName("date");

                entity.Property(e => e.GradeScaleId).HasColumnName("grade_scale_id");

                entity.Property(e => e.PeriodId).HasColumnName("period_id");

                entity.Property(e => e.RoomId).HasColumnName("room_id");

                entity.Property(e => e.TakeAttendance).HasColumnName("take_attendance");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.Rooms)
                    .WithMany(p => p.CourseCalendarSchedule)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.RoomId })
                    .HasConstraintName("course_calendar_schedule$FK_rooms");

                entity.HasOne(d => d.BlockPeriod)
                    .WithMany(p => p.CourseCalendarSchedule)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.BlockId, d.PeriodId })
                    .HasConstraintName("course_calendar_schedule$FK_periods");
            });

            modelBuilder.Entity<CourseCommentCategory>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.CourseCommentId })
                    .HasName("PK_course_comment_category_tenant_id");

                entity.ToTable("course_comment_category");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.CourseCommentId).HasColumnName("course_comment_id");

                entity.Property(e => e.ApplicableAllCourses).HasColumnName("applicable_all_courses");

                entity.Property(e => e.Comments)
                    .HasMaxLength(500)
                    .HasColumnName("comments")
                    .HasDefaultValueSql("(N'')");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.AcademicYear)
                   .HasColumnType("decimal(4, 0)")
                   .HasColumnName("academic_year");

                entity.Property(e => e.CourseName)
                    .HasMaxLength(250)
                    .HasColumnName("course_name");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");
                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<CourseFixedSchedule>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.CourseId, e.CourseSectionId, e.Serial })
                    .HasName("PK_course_fixed_schedule_tenant_id");

                entity.ToTable("course_fixed_schedule");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.BlockId, e.PeriodId }, "IX_course_fixed_schedule_tenant_id_school_id_block");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.RoomId }, "IX_course_fixed_schedule_tenant_id_school_id_room_");


                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.Serial).HasColumnName("serial");

                entity.Property(e => e.BlockId).HasColumnName("block_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.GradeScaleId).HasColumnName("grade_scale_id");

                entity.Property(e => e.PeriodId).HasColumnName("period_id");

                entity.Property(e => e.RoomId).HasColumnName("room_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.Rooms)
                    .WithMany(p => p.CourseFixedSchedule)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.RoomId })
                    .HasConstraintName("course_fixed_schedule$FK_rooms");

                entity.HasOne(d => d.BlockPeriod)
                    .WithMany(p => p.CourseFixedSchedule)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.BlockId, d.PeriodId })
                    .HasConstraintName("course_fixed_schedule$FK_periods");
            });

            modelBuilder.Entity<CourseSection>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.CourseId, e.CourseSectionId })
                    .HasName("PK_course_section_tenant_id");

                entity.ToTable("course_section");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.AttendanceCategoryId }, "IX_course_section_tenant_id_school_id_attendance_c");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.CalendarId }, "IX_course_section_tenant_id_school_id_calendar_id");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.GradeScaleId }, "IX_course_section_tenant_id_school_id_grade_scale_");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.QtrMarkingPeriodId }, "IX_course_section_tenant_id_school_id_qtr_marking_");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.SmstrMarkingPeriodId }, "IX_course_section_tenant_id_school_id_smstr_markin");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.YrMarkingPeriodId }, "IX_course_section_tenant_id_school_id_yr_marking_p");


                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.AffectsClassRank).HasColumnName("affects_class_rank");

                entity.Property(e => e.AffectsHonorRoll).HasColumnName("affects_honor_roll");

                entity.Property(e => e.AllowStudentConflict).HasColumnName("allow_student_conflict");

                entity.Property(e => e.AllowTeacherConflict).HasColumnName("allow_teacher_conflict");

                entity.Property(e => e.AllowRoomConflict).HasColumnName("allow_room_conflict");

                entity.Property(e => e.AttendanceCategoryId).HasColumnName("attendance_category_id");

                entity.Property(e => e.AttendanceTaken).HasColumnName("attendance_taken");

                entity.Property(e => e.CalendarId).HasColumnName("calendar_id");

                entity.Property(e => e.CourseSectionName)
                    .HasMaxLength(200)
                    .HasColumnName("course_section_name");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.CreditHours)
                    .HasColumnType("decimal(8, 3)")
                    .HasColumnName("credit_hours");

                entity.Property(e => e.DurationBasedOnPeriod).HasColumnName("duration_based_on_period");

                entity.Property(e => e.DurationEndDate)
                    .HasColumnType("date")
                    .HasColumnName("duration_end_date");

                entity.Property(e => e.DurationStartDate)
                    .HasColumnType("date")
                    .HasColumnName("duration_start_date");

                entity.Property(e => e.GradeScaleId).HasColumnName("grade_scale_id");

                entity.Property(e => e.GradeScaleType)
                    .HasMaxLength(13)
                    .HasColumnName("grade_scale_type");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.IsCustomMarkingPeriod).HasColumnName("is_custom_marking_period");


                entity.Property(e => e.IsWeightedCourse).HasColumnName("is_weighted_course");

                entity.Property(e => e.MeetingDays)
                    .HasMaxLength(100)
                    .HasColumnName("meeting_days");

                entity.Property(e => e.OnlineClassRoom).HasColumnName("online_class_room");

                entity.Property(e => e.OnlineClassroomPassword)
                    .HasMaxLength(20)
                    .HasColumnName("online_classroom_password");

                entity.Property(e => e.OnlineClassroomUrl)
                    .HasMaxLength(250)
                    .HasColumnName("online_classroom_url");

                entity.Property(e => e.PrgrsprdMarkingPeriodId).HasColumnName("prgrsprd_marking_period_id");

                entity.Property(e => e.QtrMarkingPeriodId).HasColumnName("qtr_marking_period_id");

                entity.Property(e => e.ScheduleType)
                    .HasMaxLength(25)
                    .HasColumnName("schedule_type");

                entity.Property(e => e.Seats).HasColumnName("seats");

                entity.Property(e => e.SmstrMarkingPeriodId).HasColumnName("smstr_marking_period_id");

                entity.Property(e => e.StandardGradeScaleId).HasColumnName("standard_grade_scale_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.UseStandards).HasColumnName("use_standards");

                entity.Property(e => e.YrMarkingPeriodId).HasColumnName("yr_marking_period_id");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.CourseSection)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("course_section$FK_course_section_school_master");

                entity.HasOne(d => d.AttendanceCodeCategories)
                    .WithMany(p => p.CourseSection)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.AttendanceCategoryId })
                    .HasConstraintName("course_section$FK_categories");

                entity.HasOne(d => d.SchoolCalendars)
                    .WithMany(p => p.CourseSection)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.CalendarId })
                    .HasConstraintName("course_section$FK_course_section_school_calendars");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.CourseSection)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.CourseId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("course_section$FK_course_section_course");

                entity.HasOne(d => d.GradeScale)
                    .WithMany(p => p.CourseSection)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.GradeScaleId })
                    .HasConstraintName("course_section$FK_course_section_grade_scale");

                entity.HasOne(d => d.Quarters)
                    .WithMany(p => p.CourseSection)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.QtrMarkingPeriodId })
                    .HasConstraintName("course_section$FK_course_section_quarters");

                entity.HasOne(d => d.Semesters)
                    .WithMany(p => p.CourseSection)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.SmstrMarkingPeriodId })
                    .HasConstraintName("course_section$FK_course_section_semesters");

                entity.HasOne(d => d.SchoolYears)
                    .WithMany(p => p.CourseSection)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.YrMarkingPeriodId })
                    .HasConstraintName("course_section$FK_course_section_school_years");

                entity.HasOne(d => d.ProgressPeriods)
                    .WithMany(p => p.CourseSections)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.PrgrsprdMarkingPeriodId })
                    .HasConstraintName("FK_course_section_progress_periods");

            });

            modelBuilder.Entity<CourseStandard>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.CourseId, e.StandardRefNo, e.GradeStandardId })
                    .HasName("PK_course_standard_tenant_id");

                entity.ToTable("course_standard");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StandardRefNo, e.GradeStandardId }, "IX_course_standard_tenant_id_school_id_standard_re");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.StandardRefNo)
                    .HasMaxLength(50)
                    .HasColumnName("standard_ref_no");

                entity.Property(e => e.GradeStandardId).HasColumnName("grade_standard_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.CourseStandard)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.CourseId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("course_standard$FK_course");

                entity.HasOne(d => d.GradeUsStandard)
                    .WithMany(p => p.CourseStandard)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StandardRefNo, d.GradeStandardId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("course_standard$FK_us_standard");
            });

            modelBuilder.Entity<CourseVariableSchedule>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.CourseId, e.CourseSectionId, e.Serial })
                    .HasName("PK_course_variable_schedule_tenant_id");

                entity.ToTable("course_variable_schedule");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.BlockId, e.PeriodId }, "IX_course_variable_schedule_tenant_id_school_id_bl");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.RoomId }, "IX_course_variable_schedule_tenant_id_school_id_ro");


                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.Serial).HasColumnName("serial");

                entity.Property(e => e.BlockId).HasColumnName("block_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Day)
                    .HasMaxLength(15)
                    .HasColumnName("day");

                entity.Property(e => e.GradeScaleId).HasColumnName("grade_scale_id");

                entity.Property(e => e.PeriodId).HasColumnName("period_id");

                entity.Property(e => e.RoomId).HasColumnName("room_id");

                entity.Property(e => e.TakeAttendance).HasColumnName("take_attendance");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.Rooms)
                    .WithMany(p => p.CourseVariableSchedule)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.RoomId })
                    .HasConstraintName("course_variable_schedule$FK_rooms");

                entity.HasOne(d => d.BlockPeriod)
                    .WithMany(p => p.CourseVariableSchedule)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.BlockId, d.PeriodId })
                    .HasConstraintName("course_variable_schedule$FK_block_periods");
            });

            modelBuilder.Entity<CustomFields>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.CategoryId, e.FieldId })
                    .HasName("PK_custom_fields_tenant_id");

                entity.ToTable("custom_fields");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.Title }, "IX_custom_fields");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.FieldId).HasColumnName("field_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.DefaultSelection)
                    .HasMaxLength(100)
                    .HasColumnName("default_selection");

                entity.Property(e => e.FieldName)
                    .HasMaxLength(50)
                    .HasColumnName("field_name");

                entity.Property(e => e.Hide).HasColumnName("hide");

                entity.Property(e => e.IsSystemWideField).HasColumnName("is_system_wide_field");

                entity.Property(e => e.Module)
                    .HasMaxLength(10)
                    .HasColumnName("module")
                    .HasDefaultValueSql("(N'')")
                    .IsFixedLength();

                entity.Property(e => e.Required).HasColumnName("required");

                entity.Property(e => e.Search).HasColumnName("search");

                entity.Property(e => e.SelectOptions).HasColumnName("select_options");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.SystemField).HasColumnName("system_field");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .HasColumnName("type");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.CustomFields)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("custom_fields$FK_custom_fields_school_master");

                entity.HasOne(d => d.FieldsCategory)
                    .WithMany(p => p.CustomFields)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.CategoryId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("custom_fields$FK_custom_fields_fields_category");
            });

            modelBuilder.Entity<CustomFieldsValue>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.CategoryId, e.FieldId, e.TargetId, e.Module })
                    .HasName("PK_custom_fields_value_tenant_id");

                entity.ToTable("custom_fields_value");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.FieldId).HasColumnName("field_id");

                entity.Property(e => e.TargetId).HasColumnName("target_id");

                entity.Property(e => e.Module)
                    .HasMaxLength(10)
                    .HasColumnName("module")
                    .IsFixedLength();

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.CustomFieldTitle)
                    .HasMaxLength(50)
                    .HasColumnName("custom_field_title");

                entity.Property(e => e.CustomFieldType)
                    .HasMaxLength(50)
                    .HasColumnName("custom_field_type");

                entity.Property(e => e.CustomFieldValue).HasColumnName("custom_field_value");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdateOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.CustomFields)
                    .WithMany(p => p.CustomFieldsValue)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.CategoryId, d.FieldId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("custom_fields_value$FK_fields");
            });

            modelBuilder.Entity<DpdownValuelist>(entity =>
            {
                entity.ToTable("dpdown_valuelist");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId }, "IX_dpdown_valuelist_tenant_id_school_id");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.LovCode)
                    .HasMaxLength(250)
                    .HasColumnName("lov_code");

                entity.Property(e => e.LovColumnValue).HasColumnName("lov_column_value");

                entity.Property(e => e.LovName)
                    .HasMaxLength(50)
                    .HasColumnName("lov_name");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.DpdownValuelist)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .HasConstraintName("dpdown_valuelist$FK_dpdown_valuelist_school_master");
            });

            modelBuilder.Entity<EffortGradeLibraryCategory>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.EffortCategoryId })
                    .HasName("PK_effort_grade_library_category_tenant_id");

                entity.ToTable("effort_grade_library_category");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.EffortCategoryId).HasColumnName("effort_category_id");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(200)
                    .HasColumnName("category_name");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<EffortGradeLibraryCategoryItem>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.EffortCategoryId, e.EffortItemId })
                    .HasName("PK_effort_grade_library_category_item_tenant_id");

                entity.ToTable("effort_grade_library_category_item");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.EffortCategoryId).HasColumnName("effort_category_id");

                entity.Property(e => e.EffortItemId).HasColumnName("effort_item_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.EffortItemTitle)
                    .HasMaxLength(200)
                    .HasColumnName("effort_item_title");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.EffortGradeLibraryCategory)
                    .WithMany(p => p.EffortGradeLibraryCategoryItem)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.EffortCategoryId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("library_category_item$FK_category");
            });

            modelBuilder.Entity<EffortGradeScale>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.EffortGradeScaleId })
                    .HasName("PK_effort_grade_scale_tenant_id");

                entity.ToTable("effort_grade_scale");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.EffortGradeScaleId).HasColumnName("effort_grade_scale_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.GradeScaleComment)
                    .HasMaxLength(200)
                    .HasColumnName("grade_scale_comment");

                entity.Property(e => e.GradeScaleValue).HasColumnName("grade_scale_value");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<FieldsCategory>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.CategoryId })
                    .HasName("PK_fields_category_tenant_id");

                entity.ToTable("fields_category");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Hide).HasColumnName("hide");

                entity.Property(e => e.IsSystemCategory).HasColumnName("is_system_category");

                entity.Property(e => e.IsSystemWideCategory).HasColumnName("is_system_wide_category");

                entity.Property(e => e.Module)
                    .HasMaxLength(10)
                    .HasColumnName("module")
                    .IsFixedLength();

                entity.Property(e => e.Required).HasColumnName("required");

                entity.Property(e => e.Search).HasColumnName("search");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.FieldsCategory)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fields_category$FK_custom_category_school_master");
            });

            modelBuilder.Entity<Grade>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.GradeScaleId, e.GradeId })
                    .HasName("PK_grade_tenant_id");

                entity.ToTable("grade");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.GradeScaleId).HasColumnName("grade_scale_id");

                entity.Property(e => e.GradeId).HasColumnName("grade_id");

                entity.Property(e => e.Breakoff).HasColumnName("breakoff");

                entity.Property(e => e.Comment).HasColumnName("comment");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.Title).HasColumnName("title");

                entity.Property(e => e.UnweightedGpValue)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("unweighted_gp_value");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.WeightedGpValue)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("weighted_gp_value");

                entity.HasOne(d => d.GradeScale)
                    .WithMany(p => p.Grade)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.GradeScaleId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("grade$FK_grade_grade_scale");
            });

            modelBuilder.Entity<GradeAgeRange>(entity =>
            {
                entity.HasKey(e => e.AgeRangeId)
                    .HasName("PK_grade_age_range_age_range_id");

                entity.ToTable("grade_age_range");

                entity.Property(e => e.AgeRangeId)
                    .ValueGeneratedNever()
                    .HasColumnName("age_range_id");

                entity.Property(e => e.AgeRange)
                    .HasMaxLength(10)
                    .HasColumnName("age_range");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<GradeEducationalStage>(entity =>
            {
                entity.HasKey(e => e.IscedCode)
                    .HasName("PK_grade_educational_stage_isced_code");

                entity.ToTable("grade_educational_stage");

                entity.Property(e => e.IscedCode)
                    .ValueGeneratedNever()
                    .HasColumnName("isced_code");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.EducationalStage)
                    .HasMaxLength(250)
                    .HasColumnName("educational_stage");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<GradeEquivalency>(entity =>
            {
                entity.HasKey(e => e.EquivalencyId)
                    .HasName("PK_grade_equivalency_equivalency_id");

                entity.ToTable("grade_equivalency");

                entity.Property(e => e.EquivalencyId)
                    .ValueGeneratedNever()
                    .HasColumnName("equivalency_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.GradeLevelEquivalency)
                    .HasMaxLength(250)
                    .HasColumnName("grade_level_equivalency");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<GradeScale>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.GradeScaleId })
                    .HasName("PK_grade_scale_tenant_id");

                entity.ToTable("grade_scale");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.GradeScaleId).HasColumnName("grade_scale_id");
                entity.Property(e => e.AcademicYear)
                   .HasColumnType("decimal(4, 0)")
                   .HasColumnName("academic_year");

                entity.Property(e => e.CalculateGpa).HasColumnName("calculate_gpa");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.GradeScaleComment).HasColumnName("grade_scale_comment");

                entity.Property(e => e.GradeScaleName)
                    .HasMaxLength(100)
                    .HasColumnName("grade_scale_name");

                entity.Property(e => e.GradeScaleValue)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("grade_scale_value");
                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.UseAsStandardGradeScale).HasColumnName("use_as_standard_grade_scale");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.GradeScale)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("grade_scale$FK_grade_scale_school_master");
            });

            modelBuilder.Entity<GradeUsStandard>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StandardRefNo, e.GradeStandardId })
                    .HasName("PK_grade_us_standard_tenant_id");

                entity.ToTable("grade_us_standard");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StandardRefNo)
                    .HasMaxLength(50)
                    .HasColumnName("standard_ref_no");

                entity.Property(e => e.GradeStandardId).HasColumnName("grade_standard_id");

                entity.Property(e => e.Course)
                    .HasMaxLength(100)
                    .HasColumnName("course");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Domain)
                    .HasMaxLength(100)
                    .HasColumnName("domain");

                entity.Property(e => e.GradeLevel)
                    .HasMaxLength(50)
                    .HasColumnName("grade_level");

                entity.Property(e => e.IsSchoolSpecific).HasColumnName("is_school_specific");

                entity.Property(e => e.StandardDetails).HasColumnName("standard_details");

                entity.Property(e => e.Subject)
                    .HasMaxLength(50)
                    .HasColumnName("subject")
                    .IsFixedLength();

                entity.Property(e => e.Topic).HasColumnName("topic");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<GradebookConfiguration>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.CourseId, e.CourseSectionId, e.AcademicYear, e.GradebookConfigurationId })
                    .HasName("PK_gradebook_configuration_tenant_id");

                entity.ToTable("gradebook_configuration");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.GradebookConfigurationId).HasColumnName("gradebook_configuration_id");

                entity.Property(e => e.AssignmentSorting)
                    .HasMaxLength(100)
                    .HasColumnName("assignment_sorting");
                entity.Property(e => e.ConfigUpdateFlag).HasColumnName("config_update_flag");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.General)
                    .HasMaxLength(250)
                    .HasColumnName("general");

                entity.Property(e => e.MaxAnomalousGrade).HasColumnName("max_anomalous_grade");

                entity.Property(e => e.ScoreRounding)
                    .HasMaxLength(100)
                    .HasColumnName("score_rounding");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.UpgradedAssignmentGradeDays).HasColumnName("upgraded_assignment_grade_days");

                entity.HasOne(d => d.CourseSection)
                    .WithMany(p => p.GradebookConfiguration)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.CourseId, d.CourseSectionId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("grd_conf_course_sec");
            });

            modelBuilder.Entity<GradebookConfigurationGradescale>(entity =>
            {
                entity.ToTable("gradebook_configuration_gradescale");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.CourseId, e.CourseSectionId, e.AcademicYear, e.GradebookConfigurationId }, "IX_gradebook_configuration_gradescale");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.BreakoffPoints).HasColumnName("breakoff_points");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.GradeId).HasColumnName("grade_id");

                entity.Property(e => e.GradeScaleId).HasColumnName("grade_scale_id");

                entity.Property(e => e.GradebookConfigurationId).HasColumnName("gradebook_configuration_id");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.GradebookConfiguration)
                    .WithMany(p => p.GradebookConfigurationGradescale)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.CourseId, d.CourseSectionId, d.AcademicYear, d.GradebookConfigurationId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("gradebook_conf_gradescale");
            });

            modelBuilder.Entity<GradebookConfigurationProgressPeriod>(entity =>
            {
                entity.ToTable("gradebook_configuration_progressPeriod");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.ExamPercentage).HasColumnName("exam_percentage");

                entity.Property(e => e.GradebookConfigurationId).HasColumnName("gradebook_configuration_id");

                entity.Property(e => e.GradingPercentage).HasColumnName("grading_percentage");

                entity.Property(e => e.PrgrsprdMarkingPeriodId).HasColumnName("prgrsprd_marking_period_id");

                entity.Property(e => e.QtrMarkingPeriodId).HasColumnName("qtr_marking_period_id");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.ProgressPeriod)
                    .WithMany(p => p.GradebookConfigurationProgressPeriods)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.PrgrsprdMarkingPeriodId })
                    .HasConstraintName("FK_gradebook_conf_progress_periods");

                entity.HasOne(d => d.GradebookConfiguration)
                    .WithMany(p => p.GradebookConfigurationProgressPeriods)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.CourseId, d.CourseSectionId, d.AcademicYear, d.GradebookConfigurationId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_gb_conf_progPeriod_conf");
            });

            modelBuilder.Entity<GradebookConfigurationQuarter>(entity =>
            {
                entity.ToTable("gradebook_configuration_quarter");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.QtrMarkingPeriodId }, "IX_gradebook_conf_qtr_tenant_id_school");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.CourseId, e.CourseSectionId, e.AcademicYear, e.GradebookConfigurationId }, "IX_gradebook_configuration_quarter_tenant_id_schoo");


                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.ExamPercentage).HasColumnName("exam_percentage");

                entity.Property(e => e.GradebookConfigurationId).HasColumnName("gradebook_configuration_id");

                entity.Property(e => e.GradingPercentage).HasColumnName("grading_percentage");

                entity.Property(e => e.PrgrsprdMarkingPeriodId).HasColumnName("prgrsprd_marking_period_id");

                entity.Property(e => e.QtrMarkingPeriodId).HasColumnName("qtr_marking_period_id");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.Quarters)
                    .WithMany(p => p.GradebookConfigurationQuarter)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.QtrMarkingPeriodId })
                    .HasConstraintName("gradebook_configuration_quarter$FK_quarters");

                


                entity.HasOne(d => d.GradebookConfiguration)
                    .WithMany(p => p.GradebookConfigurationQuarter)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.CourseId, d.CourseSectionId, d.AcademicYear, d.GradebookConfigurationId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("gradebook_configuration_quarter$FK_configuration");
            });

            modelBuilder.Entity<GradebookConfigurationSemester>(entity =>
            {
                entity.ToTable("gradebook_configuration_semester");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.SmstrMarkingPeriodId }, "IX_grade_conf_sem_t_id_school");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.QtrMarkingPeriodId }, "IX_gradebook_conf_sem_school");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.CourseId, e.CourseSectionId, e.AcademicYear, e.GradebookConfigurationId }, "IX_gradebook_configuration_semester_tenant_id_scho");


                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.ExamPercentage).HasColumnName("exam_percentage");

                entity.Property(e => e.GradebookConfigurationId).HasColumnName("gradebook_configuration_id");

                entity.Property(e => e.GradingPercentage).HasColumnName("grading_percentage");

                entity.Property(e => e.QtrMarkingPeriodId).HasColumnName("qtr_marking_period_id");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.SmstrMarkingPeriodId).HasColumnName("smstr_marking_period_id");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.Quarters)
                    .WithMany(p => p.GradebookConfigurationSemester)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.QtrMarkingPeriodId })
                    .HasConstraintName("gradebook_configuration_semester$FK_quarters");

                entity.HasOne(d => d.Semesters)
                    .WithMany(p => p.GradebookConfigurationSemester)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.SmstrMarkingPeriodId })
                    .HasConstraintName("gradebook_config_semester$FK_semester");

                entity.HasOne(d => d.GradebookConfiguration)
                    .WithMany(p => p.GradebookConfigurationSemester)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.CourseId, d.CourseSectionId, d.AcademicYear, d.GradebookConfigurationId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("gradebook_configuration_semester$FK_semester");
            });

            modelBuilder.Entity<GradebookConfigurationYear>(entity =>
            {
                entity.ToTable("gradebook_configuration_year");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.SmstrMarkingPeriodId }, "IX_grade_conf_yr_t_id_school_i");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.YrMarkingPeriodId }, "IX_grade_conf_yr_tenant_id_school_i");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.CourseId, e.CourseSectionId, e.AcademicYear, e.GradebookConfigurationId }, "IX_gradebook_configuration_year_tenant_id_school_i");


                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.ExamPercentage).HasColumnName("exam_percentage");

                entity.Property(e => e.GradebookConfigurationId).HasColumnName("gradebook_configuration_id");

                entity.Property(e => e.GradingPercentage).HasColumnName("grading_percentage");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.SmstrMarkingPeriodId).HasColumnName("smstr_marking_period_id");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.YrMarkingPeriodId).HasColumnName("yr_marking_period_id");

                entity.HasOne(d => d.Semesters)
                    .WithMany(p => p.GradebookConfigurationYear)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.SmstrMarkingPeriodId })
                    .HasConstraintName("gradebook_config_year$FK_sem");

                entity.HasOne(d => d.SchoolYears)
                    .WithMany(p => p.GradebookConfigurationYear)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.YrMarkingPeriodId })
                    .HasConstraintName("gradebook_config_year$FK_school_years");

                entity.HasOne(d => d.GradebookConfiguration)
                    .WithMany(p => p.GradebookConfigurationYear)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.CourseId, d.CourseSectionId, d.AcademicYear, d.GradebookConfigurationId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("gradebook_config_year$FK_config");
            });

            modelBuilder.Entity<GradebookGrades>(entity =>
            {
            
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.AcademicYear, e.CourseSectionId, e.AssignmentTypeId, e.AssignmentId });

                entity.ToTable("gradebook_grades");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.AssignmentId }, "IX_gradebook_grades_tenant_id_school_id_assignment");


                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.YrMarkingPeriodId).HasColumnName("yr_marking_period_id");
                entity.Property(e => e.SmstrMarkingPeriodId).HasColumnName("smstr_marking_period_id");
                entity.Property(e => e.QtrMarkingPeriodId).HasColumnName("qtr_marking_period_id");
                entity.Property(e => e.PrgrsprdMarkingPeriodId).HasColumnName("prgrsprd_marking_period_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.AssignmentTypeId).HasColumnName("assignment_type_id");

                entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");

                entity.Property(e => e.AllowedMarks)
                    .HasMaxLength(5)
                    .HasColumnName("allowed_marks")
                    .IsFixedLength();

                entity.Property(e => e.Comment)
                    .HasMaxLength(500)
                    .HasColumnName("comment");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.LetterGrade)
                    .HasMaxLength(5)
                    .HasColumnName("letter_grade")
                    .IsFixedLength();

                entity.Property(e => e.Percentage).HasColumnName("percentage");

                entity.Property(e => e.RunningAvg)
                    .HasMaxLength(10)
                    .HasColumnName("running_avg")
                    .IsFixedLength();

                entity.Property(e => e.RunningAvgGrade)
                    .HasMaxLength(5)
                    .HasColumnName("running_avg_grade")
                    .IsFixedLength();

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.Assignment)
                    .WithMany(p => p.GradebookGrades)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.AssignmentId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("gradebook_grades$FK_assignment");

                entity.HasOne(d => d.StudentMaster)
                    .WithMany(p => p.GradebookGrades)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("gradebook_grades$FK_studmast");
            });

            modelBuilder.Entity<Gradelevels>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.GradeId })
                    .HasName("PK_gradelevels_tenant_id");

                entity.ToTable("gradelevels");

                entity.HasIndex(e => e.AgeRangeId, "IX_gradelevels_age_range_id");

                entity.HasIndex(e => e.EquivalencyId, "IX_gradelevels_equivalency_id");

                entity.HasIndex(e => e.IscedCode, "IX_gradelevels_isced_code");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.GradeId).HasColumnName("grade_id");

                entity.Property(e => e.AgeRangeId).HasColumnName("age_range_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.EquivalencyId).HasColumnName("equivalency_id");

                entity.Property(e => e.IscedCode).HasColumnName("isced_code");

                entity.Property(e => e.NextGradeId).HasColumnName("next_grade_id");

                entity.Property(e => e.ShortName)
                    .HasMaxLength(5)
                    .HasColumnName("short_name");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.AgeRange)
                    .WithMany(p => p.Gradelevels)
                    .HasForeignKey(d => d.AgeRangeId)
                    .HasConstraintName("gradelevels$FK_gradelevels_grade_age_range");

                entity.HasOne(d => d.Equivalency)
                    .WithMany(p => p.Gradelevels)
                    .HasForeignKey(d => d.EquivalencyId)
                    .HasConstraintName("gradelevels$FK_gradelevels_grade_equivalency");

                entity.HasOne(d => d.IscedCodeNavigation)
                    .WithMany(p => p.Gradelevels)
                    .HasForeignKey(d => d.IscedCode)
                    .HasConstraintName("gradelevels$FK_gradelevels_grade_educational_stage");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.Gradelevels)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("gradelevels$FK_gradelevels_school_master");
            });

            modelBuilder.Entity<HistoricalCreditTransfer>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.HistGradeId, e.HistMarkingPeriodId, e.CreditTransferId })
                    .HasName("PK_historical_credit_transfer_tenant_id");

                entity.ToTable("historical_credit_transfer");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.HistGradeId).HasColumnName("hist_grade_id");

                entity.Property(e => e.HistMarkingPeriodId).HasColumnName("hist_marking_period_id");

                entity.Property(e => e.CreditTransferId).HasColumnName("credit_transfer_id");

                entity.Property(e => e.CalculateGpa).HasColumnName("calculate_gpa");

                entity.Property(e => e.CourseCode)
                    .HasMaxLength(15)
                    .HasColumnName("course_code");

                entity.Property(e => e.CourseName)
                    .HasMaxLength(150)
                    .HasColumnName("course_name");

                entity.Property(e => e.CourseType)
                    .HasMaxLength(150)
                    .HasColumnName("course_type");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.CreditAttempted)
                    .HasColumnType("decimal(4, 2)")
                    .HasColumnName("credit_attempted");

                entity.Property(e => e.CreditEarned)
                    .HasColumnType("decimal(4, 2)")
                    .HasColumnName("credit_earned");

                entity.Property(e => e.GpValue)
                    .HasColumnType("decimal(4, 2)")
                    .HasColumnName("gp_value");

                entity.Property(e => e.GradeScale)
                    .HasColumnType("decimal(4, 2)")
                    .HasColumnName("grade_scale");

                entity.Property(e => e.LetterGrade)
                    .HasMaxLength(2)
                    .HasColumnName("letter_grade")
                    .IsFixedLength();

                entity.Property(e => e.Percentage)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("percentage");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.HistoricalGrade)
                    .WithMany(p => p.HistoricalCreditTransfer)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId, d.HistGradeId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("hist_credit_trf$FK_hist_grade");
            });

            modelBuilder.Entity<HistoricalGrade>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.HistGradeId })
                    .HasName("PK_historical_grade_tenant_id");

                entity.ToTable("historical_grade");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.HistGradeId).HasColumnName("hist_grade_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.EquivalencyId).HasColumnName("equivalency_id");

                entity.Property(e => e.HistMarkingPeriodId).HasColumnName("hist_marking_period_id");

                entity.Property(e => e.SchoolName)
                    .HasMaxLength(100)
                    .HasColumnName("school_name");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<HistoricalMarkingPeriod>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.HistMarkingPeriodId })
                    .HasName("PK_historical_marking_period_tenant_id");

                entity.ToTable("historical_marking_period");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.HistMarkingPeriodId, e.AcademicYear, e.Title }, "historical_marking_period$IX_historical_marking_pe")
                     .IsUnique();

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.HistMarkingPeriodId).HasColumnName("hist_marking_period_id");

                entity.Property(e => e.AcademicYear)
                    .HasMaxLength(10)
                    .HasColumnName("academic_year");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.DoesComments).HasColumnName("does_comments");

                entity.Property(e => e.DoesExam).HasColumnName("does_exam");

                entity.Property(e => e.DoesGrades).HasColumnName("does_grades");

                entity.Property(e => e.GradePostDate)
                    .HasColumnType("date")
                    .HasColumnName("grade_post_date");

                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<HonorRolls>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.HonorRollId })
                    .HasName("PK_honor_rolls_tenant_id");

                entity.ToTable("honor_rolls");

                


                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.HonorRollId).HasColumnName("honor_roll_id");
                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.Breakoff).HasColumnName("breakoff");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.HonorRoll)
                    .HasMaxLength(50)
                    .HasColumnName("honor_roll");

                
                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

              
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.HasKey(e => e.LangId)
                    .HasName("PK_language_lang_id");

                entity.ToTable("language");

                entity.Property(e => e.LangId)
                    .ValueGeneratedNever()
                    .HasColumnName("lang_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.LanguageCode)
                    .HasMaxLength(10)
                    .HasColumnName("language_code")
                    .IsFixedLength();

                entity.Property(e => e.Lcid)
                    .HasMaxLength(10)
                    .HasColumnName("lcid")
                    .IsFixedLength();

                entity.Property(e => e.Locale)
                    .HasMaxLength(50)
                    .HasColumnName("locale")
                    .IsFixedLength();

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<LoginSession>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.TenantId, e.SchoolId, e.EmailAddress })
                    .HasName("PK_login_session_id");

                entity.ToTable("login_session");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.EmailAddress)
                    .HasMaxLength(150)
                    .HasColumnName("emailaddress");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Ipaddress)
                    .HasMaxLength(150)
                    .HasColumnName("ipaddress");

                entity.Property(e => e.IsExpired).HasColumnName("is_expired");

                entity.Property(e => e.LoginTime)
                    .HasPrecision(0)
                    .HasColumnName("login_time");

                entity.Property(e => e.Token).HasColumnName("token");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<Membership>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.MembershipId })
                    .HasName("PK_membership_tenant_id");

                entity.ToTable("membership");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.MembershipId).HasColumnName("membership_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .HasColumnName("description");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.IsSuperadmin).HasColumnName("is_superadmin");

                entity.Property(e => e.IsSystem).HasColumnName("is_system");

                entity.Property(e => e.Profile)
                    .HasMaxLength(30)
                    .HasColumnName("profile");

                entity.Property(e => e.ProfileType)
                    .HasMaxLength(30)
                    .HasColumnName("profile_type");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.Membership)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("membership$fk_table_membership_table_school_master");
            });

            modelBuilder.Entity<Notice>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.NoticeId })
                    .HasName("PK_notice_tenant_id");

                entity.ToTable("notice");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.NoticeId, e.CreatedOn, e.SortOrder }, "IX_notice");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.NoticeId).HasColumnName("notice_id");

                entity.Property(e => e.Body).HasColumnName("body");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Isactive).HasColumnName("isactive");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.TargetMembershipIds)
                    .HasMaxLength(50)
                    .HasColumnName("target_membership_ids");

                entity.Property(e => e.Title).HasColumnName("title");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.ValidFrom)
                    .HasColumnType("date")
                    .HasColumnName("valid_from");

                entity.Property(e => e.ValidTo)
                    .HasColumnType("date")
                    .HasColumnName("valid_to");

                entity.Property(e => e.VisibleToAllSchool).HasColumnName("visible_to_all_school");
            });

            modelBuilder.Entity<ParentAddress>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.ParentId, e.StudentId })
                    .HasName("PK_parent_address_tenant_id");

                entity.ToTable("parent_address");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.AddressLineOne)
                    .HasMaxLength(200)
                    .HasColumnName("address_line_one");

                entity.Property(e => e.AddressLineTwo)
                    .HasMaxLength(200)
                    .HasColumnName("address_line_two");

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .HasColumnName("city");

                entity.Property(e => e.Country)
                    .HasMaxLength(50)
                    .HasColumnName("country");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.State)
                    .HasMaxLength(50)
                    .HasColumnName("state");

                entity.Property(e => e.StudentAddressSame).HasColumnName("student_address_same");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.Zip)
                    .HasMaxLength(20)
                    .HasColumnName("zip");

                entity.HasOne(d => d.ParentInfo)
                    .WithMany(p => p.ParentAddress)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.ParentId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("parent_address$FK_parent_address_parent_info");
            });

            modelBuilder.Entity<ParentAssociationship>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.ParentId, e.StudentId })
                    .HasName("PK_parent_associationship_tenant_id");

                entity.ToTable("parent_associationship");

                entity.HasIndex(e => new { e.TenantId, e.ParentId, e.Associationship }, "IX_parent_associationship_tenant_id_parent_id_asso");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentId, e.Associationship }, "IX_parent_associationship_tenant_id_school_id_student_id_asso");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.Associationship).HasColumnName("associationship");

                entity.Property(e => e.ContactType)
                    .HasMaxLength(9)
                    .HasColumnName("contact_type");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.IsCustodian).HasColumnName("is_custodian");

                entity.Property(e => e.Relationship)
                    .HasMaxLength(30)
                    .HasColumnName("relationship");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<ParentInfo>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.ParentId })
                    .HasName("PK_parent_info_tenant_id");

                entity.ToTable("parent_info");
                entity.HasIndex(e => new { e.TenantId, e.ParentId }, "IX_parent_info_tenant_id_parent_id");
                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.BusDropoff).HasColumnName("bus_dropoff");

                entity.Property(e => e.BusNo)
                    .HasMaxLength(15)
                    .HasColumnName("bus_No");

                entity.Property(e => e.BusPickup).HasColumnName("bus_pickup");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Firstname)
                    .HasMaxLength(50)
                    .HasColumnName("firstname");

                entity.Property(e => e.HomePhone)
                    .HasMaxLength(30)
                    .HasColumnName("home_phone");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.IsPortalUser).HasColumnName("is_portal_user");

                entity.Property(e => e.Lastname)
                    .HasMaxLength(50)
                    .HasColumnName("lastname");

                entity.Property(e => e.LoginEmail)
                    .HasMaxLength(150)
                    .HasColumnName("login_email");

                entity.Property(e => e.Middlename)
                    .HasMaxLength(50)
                    .HasColumnName("middlename");

                entity.Property(e => e.Mobile)
                    .HasMaxLength(30)
                    .HasColumnName("mobile");

                entity.Property(e => e.ParentGuid)
                    .HasMaxLength(36)
                    .HasColumnName("parent_guid")
                    .HasDefaultValueSql("(N'00000000-0000-0000-0000-000000000000')")
                    .IsFixedLength();

                entity.Property(e => e.ParentPhoto).HasColumnName("parent_photo");

                entity.Property(e => e.ParentThumbnailPhoto).HasColumnName("parent_thumbnail_photo");

                entity.Property(e => e.PersonalEmail)
                    .HasMaxLength(150)
                    .HasColumnName("personal_email");

                entity.Property(e => e.Salutation)
                    .HasMaxLength(20)
                    .HasColumnName("salutation");

                entity.Property(e => e.Suffix)
                    .HasMaxLength(20)
                    .HasColumnName("suffix");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.UserProfile)
                    .HasMaxLength(50)
                    .HasColumnName("user_profile");

                entity.Property(e => e.WorkEmail)
                    .HasMaxLength(150)
                    .HasColumnName("work_email");

                entity.Property(e => e.WorkPhone)
                    .HasMaxLength(30)
                    .HasColumnName("work_phone");
            });

            modelBuilder.Entity<ParentListView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("parent_list_view");

                entity.Property(e => e.AddressLineOne)
                    .HasMaxLength(200)
                    .HasColumnName("address_line_one");

                entity.Property(e => e.AddressLineTwo)
                    .HasMaxLength(200)
                    .HasColumnName("address_line_two");

                entity.Property(e => e.Associationship).HasColumnName("associationship");

                entity.Property(e => e.BusDropoff).HasColumnName("bus_dropoff");

                entity.Property(e => e.BusNo)
                    .HasMaxLength(15)
                    .HasColumnName("bus_No");

                entity.Property(e => e.BusPickup).HasColumnName("bus_pickup");

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .HasColumnName("city");

                entity.Property(e => e.Country)
                    .HasMaxLength(50)
                    .HasColumnName("country");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.FirstGivenName)
                    .HasMaxLength(50)
                    .HasColumnName("first_given_name");

                entity.Property(e => e.Firstname)
                    .HasMaxLength(50)
                    .HasColumnName("firstname");

                entity.Property(e => e.HomePhone)
                    .HasMaxLength(30)
                    .HasColumnName("home_phone");

                entity.Property(e => e.IsPortalUser).HasColumnName("is_portal_user");

                entity.Property(e => e.LastFamilyName)
                    .HasMaxLength(50)
                    .HasColumnName("last_family_name");

                entity.Property(e => e.Lastname)
                    .HasMaxLength(50)
                    .HasColumnName("lastname");

                entity.Property(e => e.LoginEmail)
                    .HasMaxLength(150)
                    .HasColumnName("login_email");

                entity.Property(e => e.Middlename)
                    .HasMaxLength(50)
                    .HasColumnName("middlename");

                entity.Property(e => e.Mobile)
                    .HasMaxLength(30)
                    .HasColumnName("mobile");

                entity.Property(e => e.ParentGuid)
                    .HasMaxLength(36)
                    .HasColumnName("parent_guid")
                    .IsFixedLength();

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.ParentPhoto).HasColumnName("parent_photo");

                entity.Property(e => e.PersonalEmail)
                    .HasMaxLength(150)
                    .HasColumnName("personal_email");

                entity.Property(e => e.Relationship)
                    .HasMaxLength(30)
                    .HasColumnName("relationship");

                entity.Property(e => e.Salutation)
                    .HasMaxLength(20)
                    .HasColumnName("salutation");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.State)
                    .HasMaxLength(50)
                    .HasColumnName("state");

                entity.Property(e => e.StudentAddressSame).HasColumnName("student_address_same");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.StudentMiddleName)
                    .HasMaxLength(50)
                    .HasColumnName("student_middle_name");

                entity.Property(e => e.Suffix)
                    .HasMaxLength(20)
                    .HasColumnName("suffix");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.UserProfile)
                    .HasMaxLength(50)
                    .HasColumnName("user_profile");

                entity.Property(e => e.WorkEmail)
                    .HasMaxLength(150)
                    .HasColumnName("work_email");

                entity.Property(e => e.WorkPhone)
                    .HasMaxLength(30)
                    .HasColumnName("work_phone");

                entity.Property(e => e.Zip)
                    .HasMaxLength(20)
                    .HasColumnName("zip");
            });

            modelBuilder.Entity<PermissionCategory>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.PermissionCategoryId })
                    .HasName("PK_permission_category_tenant_id");

                entity.ToTable("permission_category");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.PermissionGroupId }, "IX_permission_category_tenant_id_school_id_permiss");


                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.PermissionCategoryId).HasColumnName("permission_category_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.EnableAdd).HasColumnName("enable_add");

                entity.Property(e => e.EnableDelete).HasColumnName("enable_delete");

                entity.Property(e => e.EnableEdit).HasColumnName("enable_edit");

                entity.Property(e => e.EnableView).HasColumnName("enable_view");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.Path)
                    .HasMaxLength(255)
                    .HasColumnName("path");

                entity.Property(e => e.PermissionCategoryName)
                    .HasMaxLength(50)
                    .HasColumnName("permission_category_name");

                entity.Property(e => e.PermissionGroupId).HasColumnName("permission_group_id");

                entity.Property(e => e.ShortCode)
                    .HasMaxLength(50)
                    .HasColumnName("short_code");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .HasColumnName("type");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.PermissionGroup)
                    .WithMany(p => p.PermissionCategory)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.PermissionGroupId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("permission_category$FK_group");
            });

            modelBuilder.Entity<PermissionGroup>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.PermissionGroupId })
                    .HasName("PK_permission_group_tenant_id");

                entity.ToTable("permission_group");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.PermissionGroupId).HasColumnName("permission_group_id");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.BadgeType)
                    .HasMaxLength(50)
                    .HasColumnName("badgeType");

                entity.Property(e => e.BadgeValue)
                    .HasMaxLength(50)
                    .HasColumnName("badgeValue");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Icon)
                    .HasMaxLength(50)
                    .HasColumnName("icon");

                entity.Property(e => e.IconType)
                    .HasMaxLength(50)
                    .HasColumnName("icon_type");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.IsSystem).HasColumnName("is_system");

                entity.Property(e => e.Path)
                    .HasMaxLength(255)
                    .HasColumnName("path");

                entity.Property(e => e.PermissionGroupName)
                    .HasMaxLength(50)
                    .HasColumnName("permission_group_name");

                entity.Property(e => e.ShortName)
                    .HasMaxLength(50)
                    .HasColumnName("short_name");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .HasColumnName("type");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.PermissionGroup)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("permission_group$FK_permission_group_school_master");
            });

            modelBuilder.Entity<PermissionSubcategory>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.PermissionSubcategoryId })
                    .HasName("PK_permission_subcategory_tenant_id");

                entity.ToTable("permission_subcategory");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.PermissionCategoryId }, "IX_permission_subcategory_tenant_id_school_id_perm");


                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.PermissionSubcategoryId).HasColumnName("permission_subcategory_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.EnableAdd).HasColumnName("enable_add");

                entity.Property(e => e.EnableDelete).HasColumnName("enable_delete");

                entity.Property(e => e.EnableEdit).HasColumnName("enable_edit");

                entity.Property(e => e.EnableView).HasColumnName("enable_view");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.IsSystem).HasColumnName("is_system");

                entity.Property(e => e.Path)
                    .HasMaxLength(255)
                    .HasColumnName("path");

                entity.Property(e => e.PermissionCategoryId).HasColumnName("permission_category_id");

                entity.Property(e => e.PermissionGroupId).HasColumnName("permission_group_id");

                entity.Property(e => e.PermissionSubcategoryName)
                    .HasMaxLength(50)
                    .HasColumnName("permission_subcategory_name");

                entity.Property(e => e.ShortCode)
                    .HasMaxLength(50)
                    .HasColumnName("short_code");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .HasColumnName("type");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.PermissionCategory)
                    .WithMany(p => p.PermissionSubcategory)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.PermissionCategoryId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("permission_subcategory$FK_category");
            });

            modelBuilder.Entity<Plans>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.PlanId })
                    .HasName("PK_plans_tenant_id");

                entity.ToTable("plans");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.PlanId).HasColumnName("plan_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Features).HasColumnName("features");

                entity.Property(e => e.MaxApiChecks).HasColumnName("max_api_checks");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<Programs>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.ProgramId })
                    .HasName("PK_programs_tenant_id");

                entity.ToTable("programs");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.ProgramId).HasColumnName("program_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.ProgramName)
                    .HasMaxLength(100)
                    .HasColumnName("program_name");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<ProgressPeriods>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.MarkingPeriodId })
                   .HasName("PK_progress_periods_tenant_id");

                entity.ToTable("progress_periods");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.QuarterId }, "IX_progress_periods_tenant_id_school_id_quarter_id");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.MarkingPeriodId).HasColumnName("marking_period_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.QuarterId).HasColumnName("quarter_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.DoesComments).HasColumnName("does_comments");

                entity.Property(e => e.DoesExam).HasColumnName("does_exam");

                entity.Property(e => e.DoesGrades).HasColumnName("does_grades");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("end_date");

                entity.Property(e => e.PostEndDate)
                    .HasColumnType("date")
                    .HasColumnName("post_end_date");

                entity.Property(e => e.PostStartDate)
                    .HasColumnType("date")
                    .HasColumnName("post_start_date");

                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.ShortName)
                    .HasMaxLength(10)
                    .HasColumnName("short_name");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_date");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.Quarters)
                     .WithMany(p => p.ProgressPeriods)
                     .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.QuarterId })
                     .HasConstraintName("progress_periods$FK_quarters");
            });

            modelBuilder.Entity<Quarters>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.MarkingPeriodId })
                    .HasName("PK_quarters_tenant_id");

                entity.ToTable("quarters");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.SemesterId }, "IX_quarters_tenant_id_school_id_semester_id");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.MarkingPeriodId).HasColumnName("marking_period_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.DoesComments).HasColumnName("does_comments");

                entity.Property(e => e.DoesExam).HasColumnName("does_exam");

                entity.Property(e => e.DoesGrades).HasColumnName("does_grades");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("end_date");

                entity.Property(e => e.PostEndDate)
                    .HasColumnType("date")
                    .HasColumnName("post_end_date");

                entity.Property(e => e.PostStartDate)
                    .HasColumnType("date")
                    .HasColumnName("post_start_date");

                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.SemesterId).HasColumnName("semester_id");

                entity.Property(e => e.ShortName)
                    .HasMaxLength(10)
                    .HasColumnName("short_name");

                entity.Property(e => e.SortOrder)
                    .HasColumnType("decimal(10, 0)")
                    .HasColumnName("sort_order");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_date");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.Quarters)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("quarters$FK_quarters_school_master");

                entity.HasOne(d => d.Semesters)
                    .WithMany(p => p.Quarters)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.SemesterId })
                    .HasConstraintName("quarters$FK_quarters_semesters");
            });

            modelBuilder.Entity<ReleaseNumber>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.ReleaseNumber1 })
                    .HasName("PK_release_number_tenant_id");

                entity.ToTable("release_number");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.ReleaseNumber1)
                    .HasMaxLength(9)
                    .HasColumnName("release_number");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.ReleaseDate)
                    .HasColumnType("date")
                    .HasColumnName("release_date");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.RolePermissionId })
                    .HasName("PK_role_permission_tenant_id");

                entity.ToTable("role_permission");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.MembershipId }, "IX_role_permission_tenant_id_school_id_membership_");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.PermissionCategoryId }, "IX_role_permission_tenant_id_school_id_permission_");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.PermissionGroupId }, "role_permission_tenant_id_school_id_permission");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.PermissionSubcategoryId }, "role_permission_tenant_id_school_id_permission_id");


                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.RolePermissionId).HasColumnName("role_permission_id");

                entity.Property(e => e.CanAdd).HasColumnName("can_add");

                entity.Property(e => e.CanDelete).HasColumnName("can_delete");

                entity.Property(e => e.CanEdit).HasColumnName("can_edit");

                entity.Property(e => e.CanView).HasColumnName("can_view");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.MembershipId).HasColumnName("membership_id");

                entity.Property(e => e.PermissionCategoryId).HasColumnName("permission_category_id");

                entity.Property(e => e.PermissionGroupId).HasColumnName("permission_group_id");

                entity.Property(e => e.PermissionSubcategoryId).HasColumnName("permission_subcategory_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.Membership)
                    .WithMany(p => p.RolePermission)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.MembershipId })
                    .HasConstraintName("role_permission$FK_membership");

                entity.HasOne(d => d.PermissionCategory)
                    .WithMany(p => p.RolePermission)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.PermissionCategoryId })
                    .HasConstraintName("role_permission$FK_category");

                entity.HasOne(d => d.PermissionGroup)
                    .WithMany(p => p.RolePermission)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.PermissionGroupId })
                    .HasConstraintName("role_permission$FK_groupId");

                entity.HasOne(d => d.PermissionSubcategory)
                    .WithMany(p => p.RolePermission)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.PermissionSubcategoryId })
                    .HasConstraintName("role_permission$FK_subcategory");
            });

            modelBuilder.Entity<Rooms>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.RoomId })
                    .HasName("PK_rooms_tenant_id");

                entity.ToTable("rooms");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.RoomId).HasColumnName("room_id");
                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.Capacity).HasColumnName("capacity");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.IsActive).HasColumnName("isactive");
                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });
            modelBuilder.Entity<ScheduledJob>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.JobId });

                entity.ToTable("scheduled_job");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.JobId).HasColumnName("job_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.ApiTitle)
                    .HasMaxLength(250)
                    .HasColumnName("api_title");

                entity.Property(e => e.ControllerPath)
                    .HasMaxLength(500)
                    .HasColumnName("controller_path");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.JobScheduleDate)
                    .HasColumnType("date")
                    .HasColumnName("job_schedule_date");

                entity.Property(e => e.JobTitle)
                    .HasMaxLength(150)
                    .HasColumnName("job_title");

                entity.Property(e => e.LastRunStatus).HasColumnName("last_run_status");

                entity.Property(e => e.LastRunTime)
                    .HasColumnType("datetime")
                    .HasColumnName("last_run_time");

                entity.Property(e => e.TaskJson).HasColumnName("task_json");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<ScheduledJobHistory>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.JobId, e.JobRunId });

                entity.ToTable("scheduled_job_history");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.JobId).HasColumnName("job_id");

                entity.Property(e => e.JobRunId).HasColumnName("job_run_id");

                entity.Property(e => e.JobStatus).HasColumnName("job_status");

                entity.Property(e => e.RunTime)
                    .HasColumnType("datetime")
                    .HasColumnName("run_time");

                entity.Property(e => e.ScheduledDate)
                    .HasColumnType("date")
                    .HasColumnName("scheduled_date");

                entity.HasOne(d => d.ScheduledJob)
                    .WithMany(p => p.ScheduledJobHistories)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.JobId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_job_history_job");
            });
            modelBuilder.Entity<SchoolCalendars>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.CalenderId })
                    .HasName("PK_school_calendars_tenant_id");

                entity.ToTable("school_calendars");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.CalenderId).HasColumnName("calender_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Days)
                    .HasMaxLength(7)
                    .HasColumnName("days");

                entity.Property(e => e.DefaultCalender).HasColumnName("default_calender");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("end_date");

                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.SessionCalendar).HasColumnName("session_calendar");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_date");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.VisibleToMembershipId)
                    .HasMaxLength(50)
                    .HasColumnName("visible_to_membership_id");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.SchoolCalendars)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("school_calendars$FK_school_calendars_school_master");
            });

            modelBuilder.Entity<SchoolDetail>(entity =>
            {
                entity.ToTable("school_detail");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId }, "IX_school_detail_tenant_id_school_id");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Affiliation).HasColumnName("affiliation");

                entity.Property(e => e.Associations).HasColumnName("associations");

                entity.Property(e => e.CommonToiletAccessibility).HasColumnName("common_toilet_accessibility");

                entity.Property(e => e.ComonToiletType).HasColumnName("comon_toilet_type");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.CurrentlyAvailable).HasColumnName("currently_available");

                entity.Property(e => e.DateSchoolClosed)
                    .HasColumnType("date")
                    .HasColumnName("date_school_closed");

                entity.Property(e => e.DateSchoolOpened)
                    .HasColumnType("date")
                    .HasColumnName("date_school_opened");

                entity.Property(e => e.Electricity).HasColumnName("electricity");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email")
                    .IsFixedLength();

                entity.Property(e => e.Facebook)
                    .HasMaxLength(100)
                    .HasColumnName("facebook")
                    .IsFixedLength();

                entity.Property(e => e.Fax)
                    .HasMaxLength(20)
                    .HasColumnName("fax")
                    .IsFixedLength();

                entity.Property(e => e.FemaleToiletAccessibility).HasColumnName("female_toilet_accessibility");

                entity.Property(e => e.FemaleToiletType).HasColumnName("female_toilet_type");

                entity.Property(e => e.Gender)
                    .HasMaxLength(15)
                    .HasColumnName("gender")
                    .IsFixedLength();

                entity.Property(e => e.HandwashingAvailable).HasColumnName("handwashing_available");

                entity.Property(e => e.HighestGradeLevel)
                    .HasMaxLength(100)
                    .HasColumnName("highest_grade_level")
                    .IsFixedLength();

                entity.Property(e => e.HygeneEducation)
                    .HasMaxLength(50)
                    .HasColumnName("hygene_education")
                    .IsFixedLength();

                entity.Property(e => e.Instagram)
                    .HasMaxLength(100)
                    .HasColumnName("instagram")
                    .IsFixedLength();

                entity.Property(e => e.Internet).HasColumnName("internet");

                entity.Property(e => e.LinkedIn)
                    .HasMaxLength(100)
                    .HasColumnName("linkedin")
                    .IsFixedLength();

                entity.Property(e => e.Locale)
                    .HasMaxLength(100)
                    .HasColumnName("locale")
                    .IsFixedLength();

                entity.Property(e => e.LowestGradeLevel)
                    .HasMaxLength(100)
                    .HasColumnName("lowest_grade_level")
                    .IsFixedLength();

                entity.Property(e => e.MainSourceOfDrinkingWater)
                    .HasMaxLength(100)
                    .HasColumnName("main_source_of_drinking_water")
                    .IsFixedLength();

                entity.Property(e => e.MaleToiletAccessibility).HasColumnName("male_toilet_accessibility");

                entity.Property(e => e.MaleToiletType).HasColumnName("male_toilet_type");

                entity.Property(e => e.NameOfAssistantPrincipal)
                    .HasMaxLength(100)
                    .HasColumnName("name_of_assistant_principal")
                    .IsFixedLength();

                entity.Property(e => e.NameOfPrincipal)
                    .HasMaxLength(100)
                    .HasColumnName("name_of_principal")
                    .IsFixedLength();

                entity.Property(e => e.RunningWater).HasColumnName("running_water");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.SchoolLogo).HasColumnName("school_logo");

                entity.Property(e => e.SchoolThumbnailLogo).HasColumnName("school_thumbnail_logo");

                entity.Property(e => e.SoapAndWaterAvailable).HasColumnName("soap_and_water_available");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Telephone)
                    .HasMaxLength(30)
                    .HasColumnName("telephone")
                    .IsFixedLength();

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.TotalCommonToilets).HasColumnName("total_common_toilets");

                entity.Property(e => e.TotalCommonToiletsUsable).HasColumnName("total_common_toilets_usable");

                entity.Property(e => e.TotalFemaleToilets).HasColumnName("total_female_toilets");

                entity.Property(e => e.TotalFemaleToiletsUsable).HasColumnName("total_female_toilets_usable");

                entity.Property(e => e.TotalMaleToilets).HasColumnName("total_male_toilets");

                entity.Property(e => e.TotalMaleToiletsUsable).HasColumnName("total_male_toilets_usable");

                entity.Property(e => e.Twitter)
                    .HasMaxLength(100)
                    .HasColumnName("twitter")
                    .IsFixedLength();

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.Website)
                    .HasMaxLength(150)
                    .HasColumnName("website")
                    .IsFixedLength();

                entity.Property(e => e.Youtube)
                    .HasMaxLength(100)
                    .HasColumnName("youtube")
                    .IsFixedLength();

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.SchoolDetail)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .HasConstraintName("school_detail$FK_school_detail_school_master");
            });

            modelBuilder.Entity<SchoolMaster>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId })
                    .HasName("PK_school_master_tenant_id");

                entity.ToTable("school_master");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.PlanId }, "IX_school_master_tenant_id_school_id_plan_id");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.AlternateName)
                    .HasMaxLength(100)
                    .HasColumnName("alternate_name");

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .HasColumnName("city")
                    .IsFixedLength();

                entity.Property(e => e.Country)
                    .HasMaxLength(50)
                    .HasColumnName("country")
                    .IsFixedLength();

                entity.Property(e => e.County)
                    .HasMaxLength(50)
                    .HasColumnName("county")
                    .IsFixedLength();

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.CurrentPeriodEnds)
                    .HasPrecision(0)
                    .HasColumnName("current_period_ends");

                entity.Property(e => e.District)
                    .HasMaxLength(50)
                    .HasColumnName("district")
                    .IsFixedLength();

                entity.Property(e => e.Division)
                    .HasMaxLength(50)
                    .HasColumnName("division")
                    .IsFixedLength();

                entity.Property(e => e.Features).HasColumnName("features");

                entity.Property(e => e.Latitude).HasColumnName("latitude");

                entity.Property(e => e.Longitude).HasColumnName("longitude");

                entity.Property(e => e.MaxApiChecks).HasColumnName("max_api_checks");

                entity.Property(e => e.PlanId).HasColumnName("plan_id");

                entity.Property(e => e.SchoolAltId)
                    .HasMaxLength(50)
                    .HasColumnName("school_alt_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolClassification)
                    .HasMaxLength(50)
                    .HasColumnName("school_classification")
                    .IsFixedLength();

                entity.Property(e => e.SchoolDistrictId)
                    .HasMaxLength(50)
                    .HasColumnName("school_district_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolGuid)
                    .HasMaxLength(36)
                    .HasColumnName("school_guid")
                    .HasDefaultValueSql("(N'00000000-0000-0000-0000-000000000000')")
                    .IsFixedLength();

                entity.Property(e => e.SchoolInternalId)
                    .HasMaxLength(50)
                    .HasColumnName("school_internal_id");

                entity.Property(e => e.SchoolLevel)
                    .HasMaxLength(50)
                    .HasColumnName("school_level")
                    .IsFixedLength();

                entity.Property(e => e.SchoolName)
                    .HasMaxLength(100)
                    .HasColumnName("school_name");

                entity.Property(e => e.SchoolStateId)
                    .HasMaxLength(50)
                    .HasColumnName("school_state_id")
                    .IsFixedLength();

                entity.Property(e => e.State)
                    .HasMaxLength(50)
                    .HasColumnName("state")
                    .IsFixedLength();

                entity.Property(e => e.StreetAddress1)
                    .HasMaxLength(150)
                    .HasColumnName("street_address_1");

                entity.Property(e => e.StreetAddress2)
                    .HasMaxLength(150)
                    .HasColumnName("street_address_2");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.Zip)
                    .HasMaxLength(20)
                    .HasColumnName("zip")
                    .IsFixedLength();

                entity.HasOne(d => d.Plans)
                    .WithMany(p => p.SchoolMaster)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.PlanId })
                    .HasConstraintName("school_master$FK_school_master_plans");
            });

            modelBuilder.Entity<SchoolPreference>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.SchoolPreferenceId })
                    .HasName("PK_school_preference_tenant_id");

                entity.ToTable("school_preference");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.SchoolPreferenceId).HasColumnName("school_preference_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.FullDayMinutes).HasColumnName("full_day_minutes");

                entity.Property(e => e.HalfDayMinutes).HasColumnName("half_day_minutes");

                entity.Property(e => e.MaxInactivityDays).HasColumnName("max_inactivity_days");

                entity.Property(e => e.MaxLoginFailure).HasColumnName("max_login_failure");

                entity.Property(e => e.SchoolAltId)
                    .HasMaxLength(50)
                    .HasColumnName("school_alt_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolGuid)
                    .HasMaxLength(36)
                    .HasColumnName("school_guid")
                    .IsFixedLength();

                entity.Property(e => e.SchoolInternalId)
                    .HasMaxLength(50)
                    .HasColumnName("school_internal_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.SchoolPreference)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("school_preference$FK_school");
            });

            modelBuilder.Entity<SchoolRollover>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.RolloverId })
                    .HasName("PK_school_rollover_tenant_id");

                entity.ToTable("school_rollover");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.ReenrollmentDate)
                    .HasColumnType("date")
                    .HasColumnName("reenrollment_date");

                entity.Property(e => e.RolloverContent).HasColumnName("rollover_content");

                entity.Property(e => e.RolloverStatus).HasColumnName("rollover_status");

                entity.Property(e => e.SchoolBeginDate)
                    .HasColumnType("date")
                    .HasColumnName("school_begin_date");

                entity.Property(e => e.SchoolEndDate)
                    .HasColumnType("date")
                    .HasColumnName("school_end_date");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.SchoolRollover)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("school_rollover$FK_school_rollover_school_master");
            });

            modelBuilder.Entity<SchoolYears>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.MarkingPeriodId })
                    .HasName("PK_school_years_tenant_id");

                entity.ToTable("school_years");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.MarkingPeriodId).HasColumnName("marking_period_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.DoesComments).HasColumnName("does_comments");

                entity.Property(e => e.DoesExam).HasColumnName("does_exam");

                entity.Property(e => e.DoesGrades).HasColumnName("does_grades");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("end_date");

                entity.Property(e => e.PostEndDate)
                    .HasColumnType("date")
                    .HasColumnName("post_end_date");

                entity.Property(e => e.PostStartDate)
                    .HasColumnType("date")
                    .HasColumnName("post_start_date");

                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.ShortName)
                    .HasMaxLength(10)
                    .HasColumnName("short_name");

                entity.Property(e => e.SortOrder)
                    .HasColumnType("decimal(10, 0)")
                    .HasColumnName("sort_order");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_date");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.SchoolYears)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("school_years$FK_school_years_school_master");
            });

            modelBuilder.Entity<SearchFilter>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.Module, e.FilterId })
                    .HasName("PK_search_filter_tenant_id");

                entity.ToTable("search_filter");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.FilterName }, "IX_search_filter");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.Emailaddress }, "IX_search_filter_tenant_id_school_id_emailaddress");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.Module)
                    .HasMaxLength(15)
                    .HasColumnName("module")
                    .IsFixedLength();

                entity.Property(e => e.FilterId).HasColumnName("filter_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by")
                    .IsFixedLength();

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Emailaddress)
                    .HasMaxLength(150)
                    .HasColumnName("emailaddress");

                entity.Property(e => e.FilterName)
                    .HasMaxLength(50)
                    .HasColumnName("filter_name")
                    .IsFixedLength();

                entity.Property(e => e.JsonList).HasColumnName("json_list");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by")
                    .IsFixedLength();

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.SearchFilter)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("search_filter$FK_search_filter_school_master");

                entity.HasOne(d => d.UserMaster)
                    .WithMany(p => p.SearchFilter)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.Emailaddress })
                    .HasConstraintName("search_filter$FK_search_filter_user_master");
            });

            modelBuilder.Entity<Sections>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.SectionId })
                    .HasName("PK_sections_tenant_id");

                entity.ToTable("sections");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.SectionId).HasColumnName("section_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<Semesters>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.MarkingPeriodId })
                    .HasName("PK_semesters_tenant_id");

                entity.ToTable("semesters");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.YearId }, "IX_semesters_tenant_id_school_id_year_id");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.MarkingPeriodId).HasColumnName("marking_period_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.DoesComments).HasColumnName("does_comments");

                entity.Property(e => e.DoesExam).HasColumnName("does_exam");

                entity.Property(e => e.DoesGrades).HasColumnName("does_grades");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("end_date");

                entity.Property(e => e.PostEndDate)
                    .HasColumnType("date")
                    .HasColumnName("post_end_date");

                entity.Property(e => e.PostStartDate)
                    .HasColumnType("date")
                    .HasColumnName("post_start_date");

                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.ShortName)
                    .HasMaxLength(10)
                    .HasColumnName("short_name");

                entity.Property(e => e.SortOrder)
                    .HasColumnType("decimal(10, 0)")
                    .HasColumnName("sort_order");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_date");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.YearId).HasColumnName("year_id");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.Semesters)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("semesters$FK_semesters_school_master");

                entity.HasOne(d => d.SchoolYears)
                    .WithMany(p => p.Semesters)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.YearId })
                    .HasConstraintName("semesters$FK_semesters_school_years");
            });

            modelBuilder.Entity<StaffCertificateInfo>(entity =>
            {
                entity.ToTable("staff_certificate_info");

                entity.HasIndex(e => new { e.TenantId, e.StaffId }, "IX_staff_certificate_info_tenant_id_staff_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CertificationCode)
                    .HasMaxLength(20)
                    .HasColumnName("certification_code");

                entity.Property(e => e.CertificationDate)
                    .HasColumnType("date")
                    .HasColumnName("certification_date");

                entity.Property(e => e.CertificationDescription).HasColumnName("certification_description");

                entity.Property(e => e.CertificationExpiryDate)
                    .HasColumnType("date")
                    .HasColumnName("certification_expiry_date");

                entity.Property(e => e.CertificationName)
                    .HasMaxLength(150)
                    .HasColumnName("certification_name");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.PrimaryCertification).HasColumnName("primary_certification");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.ShortName)
                    .HasMaxLength(50)
                    .HasColumnName("short_name");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.StaffMaster)
                    .WithMany(p => p.StaffCertificateInfo)
                    .HasForeignKey(d => new { d.TenantId, d.StaffId })
                   .HasConstraintName("staff_certificate_info$FK_staff");
            });

            modelBuilder.Entity<StaffCoursesectionSchedule>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StaffId, e.CourseId, e.CourseSectionId })
                    .HasName("PK_staff_coursesection_schedule_tenant_id");

                entity.ToTable("staff_coursesection_schedule");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.CourseId, e.CourseSectionId }, "IX_staff_coursesection_schedule_tenant_id_school_i");

                entity.HasIndex(e => new { e.TenantId, e.StaffId }, "IX_staff_coursesection_schedule_tenant_id_staff_id");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.QtrMarkingPeriodId }, "staff_coursesection_schedule_tenant_id_1");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.SmstrMarkingPeriodId }, "staff_coursesection_schedule_tenant_id_2");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.YrMarkingPeriodId }, "staff_coursesection_schedule_tenant_id_3");


                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.CourseSectionName)
                    .HasMaxLength(200)
                    .HasColumnName("course_section_name");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.DurationEndDate)
                    .HasColumnType("date")
                    .HasColumnName("duration_end_date");

                entity.Property(e => e.DurationStartDate)
                    .HasColumnType("date")
                    .HasColumnName("duration_start_date");

                entity.Property(e => e.EffectiveDropDate)
                    .HasPrecision(0)
                    .HasColumnName("effective_drop_date");

                entity.Property(e => e.IsAssigned).HasColumnName("is_assigned");

                entity.Property(e => e.IsDropped).HasColumnName("is_dropped");

                entity.Property(e => e.IsPrimaryStaff).HasColumnName("is_primary_staff");

                entity.Property(e => e.MeetingDays)
                    .HasMaxLength(100)
                    .HasColumnName("meeting_days");

                entity.Property(e => e.PrgrsprdMarkingPeriodId).HasColumnName("prgrsprd_marking_period_id");

                entity.Property(e => e.QtrMarkingPeriodId).HasColumnName("qtr_marking_period_id");

                entity.Property(e => e.SmstrMarkingPeriodId).HasColumnName("smstr_marking_period_id");

                entity.Property(e => e.StaffGuid)
                    .HasMaxLength(36)
                    .HasColumnName("staff_guid")
                    .IsFixedLength();

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.YrMarkingPeriodId).HasColumnName("yr_marking_period_id");

                entity.HasOne(d => d.StaffMaster)
                    .WithMany(p => p.StaffCoursesectionSchedule)
                    .HasForeignKey(d => new { d.TenantId, d.StaffId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("staff_coursesection_schedule$FK_staff_master");

                entity.HasOne(d => d.Quarter)
                    .WithMany(p => p.StaffCoursesectionSchedule)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.QtrMarkingPeriodId })
                    .HasConstraintName("staff_coursesection_schedule$FK_quarters");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.StaffCoursesectionSchedule)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.SmstrMarkingPeriodId })
                   .HasConstraintName("staff_coursesection_schedule$FK_semesters");

                entity.HasOne(d => d.SchoolYear)
                    .WithMany(p => p.StaffCoursesectionSchedule)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.YrMarkingPeriodId })
                    .HasConstraintName("staff_coursesection_schedule$FK_years");

                entity.HasOne(d => d.CourseSection)
                    .WithMany(p => p.StaffCoursesectionSchedule)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.CourseId, d.CourseSectionId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("staff_coursesection_schedule$FK_section");

                entity.HasOne(d => d.ProgressPeriod)
                         .WithMany(p => p.StaffCoursesectionSchedules)
                         .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.PrgrsprdMarkingPeriodId })
                         .HasConstraintName("FK_staff_cs_sch_progress_periods");
            });

            modelBuilder.Entity<StaffMaster>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.StaffId })
                    .HasName("PK_staff_master_tenant_id");

                entity.ToTable("staff_master");

                entity.HasIndex(e => e.FirstLanguage, "IX_staff_master_first_language");

                entity.HasIndex(e => e.SecondLanguage, "IX_staff_master_second_language");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId }, "IX_staff_master_tenant_id_school_id");

                entity.HasIndex(e => e.ThirdLanguage, "IX_staff_master_third_language");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.StaffId).HasColumnName("staff_id");

                entity.Property(e => e.AlternateId)
                    .HasMaxLength(50)
                    .HasColumnName("alternate_id");

                entity.Property(e => e.BusDropoff).HasColumnName("bus_dropoff");

                entity.Property(e => e.BusNo)
                    .HasMaxLength(15)
                    .HasColumnName("bus_no");

                entity.Property(e => e.BusPickup).HasColumnName("bus_pickup");

                entity.Property(e => e.CountryOfBirth).HasColumnName("country_of_birth");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.DisabilityDescription)
                    .HasMaxLength(200)
                    .HasColumnName("disability_description");

                entity.Property(e => e.DistrictId)
                    .HasMaxLength(50)
                    .HasColumnName("district_id");

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("dob");

                entity.Property(e => e.EmergencyEmail)
                    .HasMaxLength(150)
                    .HasColumnName("emergency_email");

                entity.Property(e => e.EmergencyFirstName)
                    .HasMaxLength(50)
                    .HasColumnName("emergency_first_name");

                entity.Property(e => e.EmergencyHomePhone)
                    .HasMaxLength(30)
                    .HasColumnName("emergency_home_phone");

                entity.Property(e => e.EmergencyLastName)
                    .HasMaxLength(50)
                    .HasColumnName("emergency_last_name");

                entity.Property(e => e.EmergencyMobilePhone)
                    .HasMaxLength(30)
                    .HasColumnName("emergency_mobile_phone");

                entity.Property(e => e.EmergencyWorkPhone)
                    .HasMaxLength(30)
                    .HasColumnName("emergency_work_phone");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("end_date");

                entity.Property(e => e.Ethnicity)
                    .HasMaxLength(50)
                    .HasColumnName("ethnicity");

                entity.Property(e => e.Facebook).HasColumnName("facebook");

                entity.Property(e => e.FirstGivenName)
                    .HasMaxLength(50)
                    .HasColumnName("first_given_name");

                entity.Property(e => e.FirstLanguage).HasColumnName("first_language");

                entity.Property(e => e.Gender)
                    .HasMaxLength(6)
                    .HasColumnName("gender");

                entity.Property(e => e.HomeAddressCity)
                    .HasMaxLength(50)
                    .HasColumnName("home_address_city");

                entity.Property(e => e.HomeAddressCountry)
                    .HasMaxLength(50)
                    .HasColumnName("home_address_country");

                entity.Property(e => e.HomeAddressLineOne)
                    .HasMaxLength(200)
                    .HasColumnName("home_address_line_one");

                entity.Property(e => e.HomeAddressLineTwo)
                    .HasMaxLength(200)
                    .HasColumnName("home_address_line_two");

                entity.Property(e => e.HomeAddressState)
                    .HasMaxLength(50)
                    .HasColumnName("home_address_state");

                entity.Property(e => e.HomeAddressZip)
                    .HasMaxLength(20)
                    .HasColumnName("home_address_zip");

                entity.Property(e => e.HomePhone)
                    .HasMaxLength(30)
                    .HasColumnName("home_phone");

                entity.Property(e => e.HomeroomTeacher).HasColumnName("homeroom_teacher");

                entity.Property(e => e.Instagram).HasColumnName("instagram");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.JobTitle)
                    .HasMaxLength(50)
                    .HasColumnName("job_title");

                entity.Property(e => e.JoiningDate)
                    .HasColumnType("date")
                    .HasColumnName("joining_date");

                entity.Property(e => e.LastFamilyName)
                    .HasMaxLength(50)
                    .HasColumnName("last_family_name");

                entity.Property(e => e.Linkedin).HasColumnName("linkedin");

                entity.Property(e => e.LoginEmailAddress)
                    .HasMaxLength(150)
                    .HasColumnName("login_email_address");

                entity.Property(e => e.MailingAddressCity)
                    .HasMaxLength(50)
                    .HasColumnName("mailing_address_city");

                entity.Property(e => e.MailingAddressCountry)
                    .HasMaxLength(50)
                    .HasColumnName("mailing_address_country");

                entity.Property(e => e.MailingAddressLineOne)
                    .HasMaxLength(200)
                    .HasColumnName("mailing_address_line_one");

                entity.Property(e => e.MailingAddressLineTwo)
                    .HasMaxLength(200)
                    .HasColumnName("mailing_address_line_two");

                entity.Property(e => e.MailingAddressSameToHome).HasColumnName("mailing_address_same_to_home");

                entity.Property(e => e.MailingAddressState)
                    .HasMaxLength(50)
                    .HasColumnName("mailing_address_state");

                entity.Property(e => e.MailingAddressZip)
                    .HasMaxLength(20)
                    .HasColumnName("mailing_address_zip");

                entity.Property(e => e.MaritalStatus)
                    .HasMaxLength(10)
                    .HasColumnName("marital_status");

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(50)
                    .HasColumnName("middle_name");

                entity.Property(e => e.MobilePhone)
                    .HasMaxLength(30)
                    .HasColumnName("mobile_phone");

                entity.Property(e => e.Nationality).HasColumnName("nationality");

                entity.Property(e => e.OfficePhone)
                    .HasMaxLength(30)
                    .HasColumnName("office_phone");

                entity.Property(e => e.OtherGovtIssuedNumber)
                    .HasMaxLength(50)
                    .HasColumnName("other_govt_issued_number");

                entity.Property(e => e.OtherGradeLevelTaught)
                    .HasMaxLength(50)
                    .HasColumnName("other_grade_level_taught");

                entity.Property(e => e.OtherSubjectTaught)
                    .HasMaxLength(50)
                    .HasColumnName("other_subject_taught");

                entity.Property(e => e.PersonalEmail).HasColumnName("personal_email");

                entity.Property(e => e.PhysicalDisability).HasColumnName("physical_disability");

                entity.Property(e => e.PortalAccess).HasColumnName("portal_access");

                entity.Property(e => e.PreferredName)
                    .HasMaxLength(50)
                    .HasColumnName("preferred_name");

                entity.Property(e => e.PreviousName)
                    .HasMaxLength(50)
                    .HasColumnName("previous_name");

                entity.Property(e => e.PrimaryGradeLevelTaught)
                    .HasMaxLength(50)
                    .HasColumnName("primary_grade_level_taught");

                entity.Property(e => e.PrimarySubjectTaught)
                    .HasMaxLength(50)
                    .HasColumnName("primary_subject_taught");

                entity.Property(e => e.Profile)
                    .HasMaxLength(50)
                    .HasColumnName("profile");

                entity.Property(e => e.Race)
                    .HasMaxLength(50)
                    .HasColumnName("race");

                entity.Property(e => e.RelationshipToStaff)
                    .HasMaxLength(50)
                    .HasColumnName("relationship_to_staff");

                entity.Property(e => e.Salutation)
                    .HasMaxLength(50)
                    .HasColumnName("salutation");

                entity.Property(e => e.SchoolEmail).HasColumnName("school_email");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.SecondLanguage).HasColumnName("second_language");

                entity.Property(e => e.SocialSecurityNumber)
                    .HasMaxLength(50)
                    .HasColumnName("social_security_number");

                entity.Property(e => e.StaffGuid)
                    .HasMaxLength(36)
                    .HasColumnName("staff_guid")
                    .HasDefaultValueSql("(N'00000000-0000-0000-0000-000000000000')")
                    .IsFixedLength();

                entity.Property(e => e.StaffInternalId)
                    .HasMaxLength(50)
                    .HasColumnName("staff_internal_id");

                entity.Property(e => e.StaffPhoto).HasColumnName("staff_photo");

                entity.Property(e => e.StaffThumbnailPhoto).HasColumnName("staff_thumbnail_photo");

                entity.Property(e => e.StateId)
                    .HasMaxLength(50)
                    .HasColumnName("state_id");

                entity.Property(e => e.Suffix)
                    .HasMaxLength(50)
                    .HasColumnName("suffix");

                entity.Property(e => e.ThirdLanguage).HasColumnName("third_language");

                entity.Property(e => e.Twitter).HasColumnName("twitter");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.Youtube).HasColumnName("youtube");

                entity.HasOne(d => d.FirstLanguageNavigation)
                    .WithMany(p => p.StaffMasterFirstLanguageNavigation)
                    .HasForeignKey(d => d.FirstLanguage)
                    .HasConstraintName("staff_master$FK_staff_master_language");

                entity.HasOne(d => d.SecondLanguageNavigation)
                    .WithMany(p => p.StaffMasterSecondLanguageNavigation)
                    .HasForeignKey(d => d.SecondLanguage)
                    .HasConstraintName("staff_master$FK_staff_master_language1");

                entity.HasOne(d => d.ThirdLanguageNavigation)
                    .WithMany(p => p.StaffMasterThirdLanguageNavigation)
                    .HasForeignKey(d => d.ThirdLanguage)
                    .HasConstraintName("staff_master$FK_staff_master_language2");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.StaffMaster)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("staff_master$FK_staff_master_school_master");
            });

            modelBuilder.Entity<StaffScheduleView>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StaffId, e.CourseId, e.CourseSectionId })
                    .HasName("PK_staff_schedule_view_tenant_id");

                entity.ToTable("staff_schedule_view");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.ConflictComment)
                    .HasMaxLength(300)
                    .HasColumnName("conflict_comment");

                entity.Property(e => e.CourseSectionName)
                    .HasMaxLength(200)
                    .HasColumnName("course_section_name");

                entity.Property(e => e.CourseShortName)
                    .HasMaxLength(50)
                    .HasColumnName("course_short_name");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Scheduled).HasColumnName("scheduled");

                entity.Property(e => e.StaffInternalId)
                    .HasMaxLength(50)
                    .HasColumnName("staff_internal_id");

                entity.Property(e => e.StaffName)
                    .HasMaxLength(250)
                    .HasColumnName("staff_name");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<StaffSchoolInfo>(entity =>
            {
                entity.ToTable("staff_school_info");

                entity.HasIndex(e => new { e.TenantId, e.StaffId }, "IX_staff_school_info_tenant_id_staff_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("end_date");
                entity.Property(e => e.MembershipId).HasColumnName("membership_id");

                entity.Property(e => e.Profile)
                    .HasMaxLength(50)
                    .HasColumnName("profile");

                entity.Property(e => e.SchoolAttachedId).HasColumnName("school_attached_id");

                entity.Property(e => e.SchoolAttachedName)
                    .HasMaxLength(100)
                    .HasColumnName("school_attached_name");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_date");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.StaffMaster)
                    .WithMany(p => p.StaffSchoolInfo)
                    .HasForeignKey(d => new { d.TenantId, d.StaffId })
                    .HasConstraintName("staff_school_info$FK_master");
                entity.HasOne(d => d.Membership)
                   .WithMany(p => p.StaffSchoolInfos)
                   .HasForeignKey(d => new { d.TenantId, d.SchoolAttachedId, d.MembershipId })
                   .HasConstraintName("staff_school_info$FK_membership");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.ToTable("state");

                entity.HasIndex(e => e.CountryId, "IX_state_countryid");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CountryId).HasColumnName("countryid");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("CreatedBy");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(6)
                    .HasColumnName("CreatedOn");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.State)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("state$FK_state_country");
            });

            modelBuilder.Entity<StudentAttendance>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.StaffId, e.CourseId, e.CourseSectionId, e.AttendanceDate, e.BlockId, e.PeriodId })
                    .HasName("PK_student_attendance_tenant_id");

                entity.ToTable("student_attendance");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentId }, "IX_student_attendance");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.CourseSectionId }, "IX_student_attendance_1");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StaffId }, "IX_student_attendance_2");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentId, e.AttendanceDate }, "IX_student_attendance_3");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.AttendanceCategoryId, e.AttendanceCode }, "IX_student_attendance_tenant_id_school_id_attendan");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.BlockId, e.PeriodId }, "IX_student_attendance_tenant_id_school_id_block_id");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.MembershipId }, "IX_student_attendance_tenant_id_school_id_membersh");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StaffId, e.CourseId, e.CourseSectionId }, "IX_student_attendance_tenant_id_school_id_staff_id");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentId, e.CourseId, e.CourseSectionId }, "IX_student_attendance_tenant_id_school_id_student_");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentId, e.StudentAttendanceId }, "student_attendance$AK_student_attendance_tenant_id")
                    .IsUnique();

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentId, e.StudentAttendanceId }, "student_attendance$student_attendance_id_idx")
                    .IsUnique();

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.AttendanceDate)
                    .HasColumnType("date")
                    .HasColumnName("attendance_date");

                entity.Property(e => e.BlockId).HasColumnName("block_id");

                entity.Property(e => e.PeriodId).HasColumnName("period_id");

                entity.Property(e => e.AttendanceCategoryId).HasColumnName("attendance_category_id");

                entity.Property(e => e.AttendanceCode).HasColumnName("attendance_code");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.MembershipId).HasColumnName("membership_id");

                entity.Property(e => e.StudentAttendanceId).HasColumnName("student_attendance_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.Membership)
                    .WithMany(p => p.StudentAttendance)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.MembershipId })
                    .HasConstraintName("student_attendance$FK_membership");

                entity.HasOne(d => d.AttendanceCodeNavigation)
                    .WithMany(p => p.StudentAttendance)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.AttendanceCategoryId, d.AttendanceCode })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("student_attendance$FK_code");

                entity.HasOne(d => d.BlockPeriod)
                    .WithMany(p => p.StudentAttendance)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.BlockId, d.PeriodId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("student_attendance$FKperiod");

                entity.HasOne(d => d.StaffCoursesectionSchedule)
                    .WithMany(p => p.StudentAttendance)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StaffId, d.CourseId, d.CourseSectionId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("student_attendance$FK_staff_cs_sch");

                entity.HasOne(d => d.StudentCoursesectionSchedule)
                    .WithMany(p => p.StudentAttendance)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId, d.CourseId, d.CourseSectionId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("student_attd_coursesec_sch");
            });

            modelBuilder.Entity<StudentAttendanceComments>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.StudentAttendanceId, e.CommentId })
                    .HasName("PK_student_attendance_comments_tenant_id");

                entity.ToTable("student_attendance_comments");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.MembershipId }, "IX_student_attendance_comments_tenant_id_school_id");


                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.StudentAttendanceId).HasColumnName("student_attendance_id");

                entity.Property(e => e.CommentId).HasColumnName("comment_id");

                entity.Property(e => e.Comment)
                    .HasMaxLength(500)
                    .HasColumnName("comment");

                entity.Property(e => e.CommentTimestamp)
                    .HasPrecision(0)
                    .HasColumnName("comment_timestamp");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.MembershipId).HasColumnName("membership_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.Membership)
                    .WithMany(p => p.StudentAttendanceComments)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.MembershipId })
                   .HasConstraintName("student_attendance_comments$FK_membership");

                entity.HasOne(d => d.StudentAttendance)
                    .WithMany(p => p.StudentAttendanceComments)
                    .HasPrincipalKey(p => new { p.TenantId, p.SchoolId, p.StudentId, p.StudentAttendanceId })
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId, d.StudentAttendanceId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("std_attd_comments$FK_std_atd");
            });

            modelBuilder.Entity<StudentAttendanceHistory>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.AttendanceHistoryId })
                    .HasName("PK_student_attendance_history_tenant_id");

                entity.ToTable("student_attendance_history");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.AttendanceHistoryId).HasColumnName("attendance_history_id");

                entity.Property(e => e.AttendanceCategoryId).HasColumnName("attendance_category_id");

                entity.Property(e => e.AttendanceCode).HasColumnName("attendance_code");

                entity.Property(e => e.AttendanceDate)
                    .HasColumnType("date")
                    .HasColumnName("attendance_date");

                entity.Property(e => e.BlockId).HasColumnName("block_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.MembershipId).HasColumnName("membership_id");

                entity.Property(e => e.ModificationTimestamp)
                    .HasPrecision(0)
                    .HasColumnName("modification_timestamp");

                entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");

                entity.Property(e => e.PeriodId).HasColumnName("period_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.StudentMaster)
                    .WithMany(p => p.StudentAttendanceHistory)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("student_attendance_history$FK_student");
            });

            modelBuilder.Entity<StudentComments>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.CommentId })
                    .HasName("PK_student_comments_tenant_id");

                entity.ToTable("student_comments");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.CommentId).HasColumnName("comment_id");

                entity.Property(e => e.Comment).HasColumnName("comment");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.StudentMaster)
                    .WithMany(p => p.StudentComments)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("student_comments$FK_std_master");
            });

            modelBuilder.Entity<StudentCoursesectionSchedule>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.CourseId, e.CourseSectionId })
                    .HasName("PK_student_coursesection_schedule_tenant_id");

                entity.ToTable("student_coursesection_schedule");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentId }, "IX_student_coursesection_schedule");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.CourseId, e.CourseSectionId }, "IX_student_coursesection_schedule_1");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.AlternateId)
                    .HasMaxLength(50)
                    .HasColumnName("alternate_id");

                entity.Property(e => e.CalendarId).HasColumnName("calendar_id");

                entity.Property(e => e.CourseSectionName)
                    .HasMaxLength(200)
                    .HasColumnName("course_section_name");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.EffectiveDropDate)
                    .HasPrecision(0)
                    .HasColumnName("effective_drop_date");

                entity.Property(e => e.EffectiveStartDate)
                    .HasPrecision(0)
                    .HasColumnName("effective_start_date");

                entity.Property(e => e.FirstGivenName)
                    .HasMaxLength(50)
                    .HasColumnName("first_given_name");

                entity.Property(e => e.FirstLanguageId).HasColumnName("first_language_id");

                entity.Property(e => e.GradeId).HasColumnName("grade_id");

                entity.Property(e => e.GradeScaleId).HasColumnName("grade_scale_id");

                entity.Property(e => e.IsDropped).HasColumnName("is_dropped");

                entity.Property(e => e.LastFamilyName)
                    .HasMaxLength(50)
                    .HasColumnName("last_family_name");

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(50)
                    .HasColumnName("middle_name");

                entity.Property(e => e.StudentGuid)
                    .HasMaxLength(36)
                    .HasColumnName("student_guid")
                    .IsFixedLength();

                entity.Property(e => e.StudentInternalId)
                    .HasMaxLength(50)
                    .HasColumnName("student_internal_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.StudentCoursesectionSchedule)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("sch_mast_std_cs_sch");


                entity.HasOne(d => d.StudentMaster)
                    .WithMany(p => p.StudentCoursesectionSchedule)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("std_mast_cs_sch");


                entity.HasOne(d => d.CourseSection)
                    .WithMany(p => p.StudentCoursesectionSchedule)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.CourseId, d.CourseSectionId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("std_cs_cs_sch");
            });

            modelBuilder.Entity<StudentDailyAttendance>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.AttendanceDate })
                    .HasName("PK_student_daily_attendance_tenant_id");

                entity.ToTable("student_daily_attendance");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.GradeScaleId }, "IX_student_daily_attendance_tenant_id_school_id_gr");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.SectionId }, "IX_student_daily_attendance_tenant_id_school_id_se");


                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.AttendanceDate)
                    .HasColumnType("date")
                    .HasColumnName("attendance_date");

                entity.Property(e => e.AttendanceCode)
                    .HasMaxLength(20)
                    .HasColumnName("attendance_code");

                entity.Property(e => e.AttendanceComment)
                    .HasMaxLength(250)
                    .HasColumnName("attendance_comment");

                entity.Property(e => e.AttendanceMinutes).HasColumnName("attendance_minutes");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.GradeId).HasColumnName("grade_id");

                entity.Property(e => e.GradeScaleId).HasColumnName("grade_scale_id");

                entity.Property(e => e.SectionId).HasColumnName("section_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.GradeScale)
                    .WithMany(p => p.StudentDailyAttendance)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.GradeScaleId })
                   .HasConstraintName("student_daily_attendance$FK_grade_scale");

                entity.HasOne(d => d.Sections)
                    .WithMany(p => p.StudentDailyAttendance)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.SectionId })
                    .HasConstraintName("student_daily_attendance$FK_sections");

                entity.HasOne(d => d.StudentMaster)
                    .WithMany(p => p.StudentDailyAttendance)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("student_daily_attendance$FK_master");
            });

            modelBuilder.Entity<StudentDocuments>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.DocumentId })
                    .HasName("PK_student_documents_tenant_id");

                entity.ToTable("student_documents");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.DocumentId).HasColumnName("document_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.FileUploaded).HasColumnName("file_uploaded");

                entity.Property(e => e.Filename)
                    .HasMaxLength(100)
                    .HasColumnName("filename");

                entity.Property(e => e.Filetype)
                    .HasMaxLength(100)
                    .HasColumnName("filetype");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.UploadedBy)
                    .HasMaxLength(100)
                    .HasColumnName("uploaded_by");

                entity.Property(e => e.UploadedOn)
                    .HasPrecision(0)
                    .HasColumnName("uploaded_on");

                entity.HasOne(d => d.StudentMaster)
                    .WithMany(p => p.StudentDocuments)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("student_documents$FK_master");
            });

            modelBuilder.Entity<StudentEffortGradeDetail>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.StudentEffortGradeSrlno, e.Id })
                    .HasName("PK_student_effort_grade_detail_tenant_id");

                entity.ToTable("student_effort_grade_detail");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.StudentEffortGradeSrlno).HasColumnName("student_effort_grade_srlno");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.EffortCategoryId).HasColumnName("effort_category_id");

                entity.Property(e => e.EffortGradeScaleId).HasColumnName("effort_grade_scale_id");

                entity.Property(e => e.EffortItemId).HasColumnName("effort_item_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.StudentEffortGradeMaster)
                    .WithMany(p => p.StudentEffortGradeDetail)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId, d.StudentEffortGradeSrlno })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("std_effort_master_detail");
            });

            modelBuilder.Entity<StudentEffortGradeMaster>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.StudentEffortGradeSrlno })
                    .HasName("PK_student_effort_grade_master_tenant_id");

                entity.ToTable("student_effort_grade_master");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.SmstrMarkingPeriodId }, "IX_std_effort_gd_master_tid_school_id");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.QtrMarkingPeriodId }, "IX_std_effort_grade_master_tid_school_id");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.YrMarkingPeriodId }, "IX_std_effort_grade_mster_tid_school_id");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.CalendarId }, "IX_student_effort_grade_master_tenant_id_school_id");


                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.StudentEffortGradeSrlno).HasColumnName("student_effort_grade_srlno");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.CalendarId).HasColumnName("calendar_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.IsCustomMarkingPeriod).HasColumnName("is_custom_marking_period");

                entity.Property(e => e.IsExamGrade).HasColumnName("is_exam_grade");

                entity.Property(e => e.PrgrsprdMarkingPeriodId).HasColumnName("prgrsprd_marking_period_id");

                entity.Property(e => e.QtrMarkingPeriodId).HasColumnName("qtr_marking_period_id");

                entity.Property(e => e.SmstrMarkingPeriodId).HasColumnName("smstr_marking_period_id");

                entity.Property(e => e.TeacherComment)
                    .HasMaxLength(500)
                    .HasColumnName("teacher_comment");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.YrMarkingPeriodId).HasColumnName("yr_marking_period_id");

                entity.HasOne(d => d.SchoolCalendars)
                    .WithMany(p => p.StudentEffortGradeMaster)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.CalendarId })
                    .HasConstraintName("student_effort_grade_calendars");

                entity.HasOne(d => d.ProgressPeriod)
                   .WithMany(p => p.StudentEffortGradeMasters)
                   .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.PrgrsprdMarkingPeriodId })
                   .HasConstraintName("FK_student_effort_grade_master_progress_periods");


                entity.HasOne(d => d.Quarters)
                    .WithMany(p => p.StudentEffortGradeMaster)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.QtrMarkingPeriodId })
                    .HasConstraintName("student_effort_grade_quarters");

                entity.HasOne(d => d.Semesters)
                    .WithMany(p => p.StudentEffortGradeMaster)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.SmstrMarkingPeriodId })
                    .HasConstraintName("student_effort_grade_semesters");

                entity.HasOne(d => d.SchoolYears)
                    .WithMany(p => p.StudentEffortGradeMaster)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.YrMarkingPeriodId })
                   .HasConstraintName("student_effort_grade_years");

              
            });

            modelBuilder.Entity<StudentEnrollment>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.EnrollmentId })
                    .HasName("PK_student_enrollment_tenant_id");

                entity.ToTable("student_enrollment");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.GradeId }, "IX_student_enrollment_tenant_id_school_id_grade_id");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentGuid }, "IX_student_enrollment_tenant_id_school_id_student_");


                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.EnrollmentId).HasColumnName("enrollment_id");

                entity.Property(e => e.CalenderId).HasColumnName("calender_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.EnrollmentCode)
                    .HasMaxLength(50)
                    .HasColumnName("enrollment_code");

                entity.Property(e => e.EnrollmentDate)
                    .HasColumnType("date")
                    .HasColumnName("enrollment_date");

                entity.Property(e => e.ExitCode)
                    .HasMaxLength(50)
                    .HasColumnName("exit_code");

                entity.Property(e => e.ExitDate)
                    .HasColumnType("date")
                    .HasColumnName("exit_date");

                entity.Property(e => e.GradeId).HasColumnName("grade_id");

                entity.Property(e => e.GradeLevelTitle)
                    .HasMaxLength(50)
                    .HasColumnName("grade_level_title");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.RollingOption)
                    .HasMaxLength(50)
                    .HasColumnName("rolling_option");

                entity.Property(e => e.SchoolName)
                    .HasMaxLength(200)
                    .HasColumnName("school_name");

                entity.Property(e => e.SchoolTransferred)
                    .HasMaxLength(200)
                    .HasColumnName("school_transferred");

                entity.Property(e => e.StudentGuid)
                    .HasMaxLength(36)
                    .HasColumnName("student_guid")
                    .HasDefaultValueSql("(N'00000000-0000-0000-0000-000000000000')")
                    .IsFixedLength();

                entity.Property(e => e.TransferredGrade)
                    .HasMaxLength(50)
                    .HasColumnName("transferred_grade");
                entity.Property(e => e.EnrollOtherSchoolId).HasColumnName("enroll_other_school_id");

                entity.Property(e => e.TransferredSchoolId).HasColumnName("transferred_school_id");

                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.Gradelevels)
                    .WithMany(p => p.StudentEnrollment)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.GradeId })
                    .HasConstraintName("student_enrollment$FK_gradelevels");
            });

            modelBuilder.Entity<StudentEnrollmentCode>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.EnrollmentCode })
                    .HasName("PK_student_enrollment_code_tenant_id");

                entity.ToTable("student_enrollment_code");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.EnrollmentCode).HasColumnName("enrollment_code");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.ShortName)
                    .HasMaxLength(10)
                    .HasColumnName("short_name")
                    .IsFixedLength();

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .HasColumnName("title");

                entity.Property(e => e.Type)
                    .HasMaxLength(20)
                    .HasColumnName("type");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.StudentEnrollmentCode)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("student_enrollment_code$FK_master");
            });

            modelBuilder.Entity<StudentFinalGrade>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.StudentFinalGradeSrlno })
                    .HasName("PK_student_final_grade_tenant_id");

                entity.ToTable("student_final_grade");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.QtrMarkingPeriodId }, "IX_student_final_grade_tenant_id_school_id_qtr_mar");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.SmstrMarkingPeriodId }, "IX_student_final_grade_tenant_id_school_id_smstr_m");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.YrMarkingPeriodId }, "IX_student_final_grade_tenant_id_school_id_yr_mark");


                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.StudentFinalGradeSrlno).HasColumnName("student_final_grade_srlno");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.BasedOnStandardGrade).HasColumnName("based_on_standard_grade");

                entity.Property(e => e.CalendarId).HasColumnName("calendar_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.CreditAttempted)
                    .HasColumnType("decimal(8, 3)")
                    .HasColumnName("creditattempted");

                entity.Property(e => e.CreditEarned)
                    .HasColumnType("decimal(8, 3)")
                    .HasColumnName("creditearned");

                entity.Property(e => e.GradeId).HasColumnName("grade_id");

                entity.Property(e => e.GradeObtained)
                    .HasMaxLength(5)
                    .HasColumnName("grade_obtained")
                    .IsFixedLength();
               
                entity.Property(e => e.GradeScaleId).HasColumnName("grade_scale_id");

                entity.Property(e => e.IsCustomMarkingPeriod).HasColumnName("is_custom_marking_period");

                entity.Property(e => e.IsExamGrade).HasColumnName("is_exam_grade");


                entity.Property(e => e.IsPercent).HasColumnName("is_percent");

                entity.Property(e => e.PercentMarks)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("percent_marks");

                entity.Property(e => e.PrgrsprdMarkingPeriodId).HasColumnName("prgrsprd_marking_period_id");

                entity.Property(e => e.QtrMarkingPeriodId).HasColumnName("qtr_marking_period_id");

                entity.Property(e => e.SmstrMarkingPeriodId).HasColumnName("smstr_marking_period_id");

                entity.Property(e => e.TeacherComment)
                    .HasMaxLength(500)
                    .HasColumnName("teacher_comment");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.YrMarkingPeriodId).HasColumnName("yr_marking_period_id");

                entity.HasOne(d => d.ProgressPeriod)
                    .WithMany(p => p.StudentFinalGrades)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.PrgrsprdMarkingPeriodId })
                    .HasConstraintName("FK_student_final_grade_progress_periods");


                entity.HasOne(d => d.Quarters)
                    .WithMany(p => p.StudentFinalGrade)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.QtrMarkingPeriodId })
                    .HasConstraintName("student_final_grade$FK_quarters");


                entity.HasOne(d => d.Semesters)
                    .WithMany(p => p.StudentFinalGrade)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.SmstrMarkingPeriodId })
                    .HasConstraintName("student_final_grade$FK_semesters");

                entity.HasOne(d => d.StudentMaster)
                    .WithMany(p => p.StudentFinalGrade)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("student_final_grade$FK_master");

                entity.HasOne(d => d.SchoolYears)
                    .WithMany(p => p.StudentFinalGrade)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.YrMarkingPeriodId })
                    .HasConstraintName("student_final_grade$FK_years");

            });

            modelBuilder.Entity<StudentFinalGradeComments>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.StudentFinalGradeSrlno, e.CourseCommentId })
                    .HasName("PK_student_final_grade_comments_tenant_id");

                entity.ToTable("student_final_grade_comments");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.CourseCommentId }, "IX_student_final_grade_comments_tenant_id_school_i");


                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.StudentFinalGradeSrlno).HasColumnName("student_final_grade_srlno");

                entity.Property(e => e.CourseCommentId).HasColumnName("course_comment_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.CourseCommentCategory)
                    .WithMany(p => p.StudentFinalGradeComments)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.CourseCommentId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("student_final_grade_comments$FK_comment_category");

                entity.HasOne(d => d.StudentFinalGrade)
                    .WithMany(p => p.StudentFinalGradeComments)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId, d.StudentFinalGradeSrlno })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("std_final_grade_cmnts$FK_final_grade");
            });

            modelBuilder.Entity<StudentFinalGradeStandard>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.Id })
                    .HasName("PK_student_final_grade_standard_tenant_id");

                entity.ToTable("student_final_grade_standard");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentId, e.StudentFinalGradeSrlno }, "IX_student_final_grade_standard_tenant_id_school_i");


                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.CalendarId).HasColumnName("calendar_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.GradeObtained).HasColumnName("grade_obtained");

                entity.Property(e => e.PrgrsprdMarkingPeriodId).HasColumnName("prgrsprd_marking_period_id");

                entity.Property(e => e.QtrMarkingPeriodId).HasColumnName("qtr_marking_period_id");

                entity.Property(e => e.SmstrMarkingPeriodId).HasColumnName("smstr_marking_period_id");

                entity.Property(e => e.StandardGradeScaleId).HasColumnName("standard_grade_scale_id");

                entity.Property(e => e.StudentFinalGradeSrlno).HasColumnName("student_final_grade_srlno");

                entity.Property(e => e.TeacherComment)
                    .HasMaxLength(500)
                    .HasColumnName("teacher_comment");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.YrMarkingPeriodId).HasColumnName("yr_marking_period_id");

                entity.HasOne(d => d.StudentFinalGrade)
                    .WithMany(p => p.StudentFinalGradeStandard)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId, d.StudentFinalGradeSrlno })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("std_final_grade_std$FK_final_grade");
            });

            modelBuilder.Entity<StudentListView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("student_list_view");

                entity.Property(e => e.AdmissionNumber)
                    .HasMaxLength(50)
                    .HasColumnName("admission_number");

                entity.Property(e => e.AlternateId)
                    .HasMaxLength(50)
                    .HasColumnName("alternate_id");

                entity.Property(e => e.BusNo)
                    .HasMaxLength(15)
                    .HasColumnName("bus_no");

                entity.Property(e => e.CalenderId).HasColumnName("calender_id");

                entity.Property(e => e.CountryOfBirth).HasColumnName("country_of_birth");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.DistrictId)
                    .HasMaxLength(50)
                    .HasColumnName("district_id");

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("dob");

                entity.Property(e => e.EconomicDisadvantage).HasColumnName("economic_disadvantage");

                entity.Property(e => e.Eligibility504).HasColumnName("eligibility_504");

                entity.Property(e => e.EnrollmentCode)
                    .HasMaxLength(50)
                    .HasColumnName("enrollment_code");

                entity.Property(e => e.EnrollmentDate)
                    .HasColumnType("date")
                    .HasColumnName("enrollment_date");

                entity.Property(e => e.EnrollmentId).HasColumnName("enrollment_id");

                entity.Property(e => e.EnrollmentType)
                    .HasMaxLength(8)
                    .HasColumnName("enrollment_type")
                    .IsFixedLength();

                entity.Property(e => e.EstimatedGradDate)
                    .HasColumnType("date")
                    .HasColumnName("estimated_grad_date");

                entity.Property(e => e.Ethnicity)
                    .HasMaxLength(50)
                    .HasColumnName("ethnicity");

                entity.Property(e => e.Facebook).HasColumnName("facebook");

                entity.Property(e => e.FirstGivenName)
                    .HasMaxLength(50)
                    .HasColumnName("first_given_name");

                entity.Property(e => e.FirstLanguageId).HasColumnName("first_language_id");

                entity.Property(e => e.FreeLunchEligibility).HasColumnName("free_lunch_eligibility");

                entity.Property(e => e.Gender)
                    .HasMaxLength(6)
                    .HasColumnName("gender");

                entity.Property(e => e.GradeId).HasColumnName("grade_id");

                entity.Property(e => e.GradeLevelTitle)
                    .HasMaxLength(50)
                    .HasColumnName("grade_level_title");

                entity.Property(e => e.HomeAddressCity)
                    .HasMaxLength(50)
                    .HasColumnName("home_address_city");

                entity.Property(e => e.HomeAddressCountry)
                    .HasMaxLength(50)
                    .HasColumnName("home_address_country");

                entity.Property(e => e.HomeAddressLineOne)
                    .HasMaxLength(200)
                    .HasColumnName("home_address_line_one");

                entity.Property(e => e.HomeAddressLineTwo)
                    .HasMaxLength(200)
                    .HasColumnName("home_address_line_two");

                entity.Property(e => e.HomeAddressState)
                    .HasMaxLength(50)
                    .HasColumnName("home_address_state");

                entity.Property(e => e.HomeAddressZip)
                    .HasMaxLength(20)
                    .HasColumnName("home_address_zip");

                entity.Property(e => e.HomePhone)
                    .HasMaxLength(30)
                    .HasColumnName("home_phone");

                entity.Property(e => e.Instagram).HasColumnName("instagram");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.LastFamilyName)
                    .HasMaxLength(50)
                    .HasColumnName("last_family_name");

                entity.Property(e => e.LepIndicator).HasColumnName("lep_indicator");

                entity.Property(e => e.Linkedin).HasColumnName("linkedin");

                entity.Property(e => e.MailingAddressCity)
                    .HasMaxLength(50)
                    .HasColumnName("mailing_address_city");

                entity.Property(e => e.MailingAddressCountry)
                    .HasMaxLength(50)
                    .HasColumnName("mailing_address_country");

                entity.Property(e => e.MailingAddressLineOne)
                    .HasMaxLength(200)
                    .HasColumnName("mailing_address_line_one");

                entity.Property(e => e.MailingAddressLineTwo)
                    .HasMaxLength(200)
                    .HasColumnName("mailing_address_line_two");

                entity.Property(e => e.MailingAddressSameToHome).HasColumnName("mailing_address_same_to_home");

                entity.Property(e => e.MailingAddressState)
                    .HasMaxLength(50)
                    .HasColumnName("mailing_address_state");

                entity.Property(e => e.MailingAddressZip)
                    .HasMaxLength(20)
                    .HasColumnName("mailing_address_zip");

                entity.Property(e => e.MaritalStatus)
                    .HasMaxLength(10)
                    .HasColumnName("marital_status");

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(50)
                    .HasColumnName("middle_name");

                entity.Property(e => e.MobilePhone)
                    .HasMaxLength(30)
                    .HasColumnName("mobile_phone");

                entity.Property(e => e.Nationality).HasColumnName("nationality");

                entity.Property(e => e.OtherGovtIssuedNumber)
                    .HasMaxLength(50)
                    .HasColumnName("other_govt_issued_number");

                entity.Property(e => e.PersonalEmail).HasColumnName("personal_email");

                entity.Property(e => e.PreferredName)
                    .HasMaxLength(50)
                    .HasColumnName("preferred_name");

                entity.Property(e => e.PreviousName)
                    .HasMaxLength(50)
                    .HasColumnName("previous_name");

                entity.Property(e => e.Race)
                    .HasMaxLength(50)
                    .HasColumnName("race");

                entity.Property(e => e.RollNumber)
                    .HasMaxLength(50)
                    .HasColumnName("roll_number");

                entity.Property(e => e.RollingOption)
                    .HasMaxLength(50)
                    .HasColumnName("rolling_option");

                entity.Property(e => e.Salutation)
                    .HasMaxLength(50)
                    .HasColumnName("salutation");

                entity.Property(e => e.SchoolBusDropOff).HasColumnName("school_bus_drop_off");

                entity.Property(e => e.SchoolBusPickUp).HasColumnName("school_bus_pick_up");

                entity.Property(e => e.SchoolEmail).HasColumnName("school_email");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.SchoolName)
                    .HasMaxLength(200)
                    .HasColumnName("school_name");

                entity.Property(e => e.SecondLanguageId).HasColumnName("second_language_id");

                entity.Property(e => e.SectionId).HasColumnName("section_id");

                entity.Property(e => e.SectionName)
                    .HasMaxLength(255)
                    .HasColumnName("section_name");

                entity.Property(e => e.SocialSecurityNumber)
                    .HasMaxLength(50)
                    .HasColumnName("social_security_number");

                entity.Property(e => e.SpecialEducationIndicator).HasColumnName("special_education_indicator");

                entity.Property(e => e.StateId)
                    .HasMaxLength(50)
                    .HasColumnName("state_id");

                entity.Property(e => e.StudentGuid)
                    .HasMaxLength(36)
                    .HasColumnName("student_guid")
                    .IsFixedLength();

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.StudentInternalId)
                    .HasMaxLength(50)
                    .HasColumnName("student_internal_id");

                entity.Property(e => e.StudentPortalId)
                    .HasMaxLength(50)
                    .HasColumnName("student_portal_id");

                entity.Property(e => e.Suffix)
                    .HasMaxLength(50)
                    .HasColumnName("suffix");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.ThirdLanguageId).HasColumnName("third_language_id");

                entity.Property(e => e.Twitter).HasColumnName("twitter");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.Youtube).HasColumnName("youtube");
            });

            modelBuilder.Entity<StudentMedicalListView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("student_medical_list_view");

                entity.Property(e => e.AlertDescription).HasColumnName("alert_description");

                entity.Property(e => e.AlertType)
                    .HasMaxLength(100)
                    .HasColumnName("alert_type");

                entity.Property(e => e.ImmunizationComment).HasColumnName("immunization_comment");

                entity.Property(e => e.ImmunizationDate)
                    .HasColumnType("date")
                    .HasColumnName("immunization_date");

                entity.Property(e => e.ImmunizationType)
                    .HasMaxLength(150)
                    .HasColumnName("immunization_type");

                entity.Property(e => e.MedicalNote).HasColumnName("medical_note");

                entity.Property(e => e.NoteDate)
                    .HasColumnType("date")
                    .HasColumnName("note_date");

                entity.Property(e => e.NurseComment).HasColumnName("nurse_comment");

                entity.Property(e => e.NurseVisitDate)
                    .HasColumnType("date")
                    .HasColumnName("nurse_visit_date");

                entity.Property(e => e.Reason)
                    .HasMaxLength(250)
                    .HasColumnName("reason");

                entity.Property(e => e.Result)
                    .HasMaxLength(250)
                    .HasColumnName("result");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.StudentGuid).HasColumnName("student_guid");
            });

            modelBuilder.Entity<StudentMaster>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId })
                    .HasName("PK_student_master_tenant_id");

                entity.ToTable("student_master");

                entity.HasIndex(e => e.FirstLanguageId, "IX_student_master_first_language_id");

                entity.HasIndex(e => e.SecondLanguageId, "IX_student_master_second_language_id");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.SectionId }, "IX_student_master_tenant_id_school_id_section_id");

                entity.HasIndex(e => e.ThirdLanguageId, "IX_student_master_third_language_id");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentGuid }, "student_master$AK_student_master_tenant_id_school_")
                    .IsUnique();

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentGuid }, "student_master$IX_student_master")
                    .IsUnique();

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.AdmissionNumber)
                    .HasMaxLength(50)
                    .HasColumnName("admission_number");

                entity.Property(e => e.AlertDescription).HasColumnName("alert_description");

                entity.Property(e => e.AlternateId)
                    .HasMaxLength(50)
                    .HasColumnName("alternate_id");

                entity.Property(e => e.Associationship).HasColumnName("associationship");

                entity.Property(e => e.BusNo)
                    .HasMaxLength(15)
                    .HasColumnName("bus_no");

                entity.Property(e => e.CountryOfBirth).HasColumnName("country_of_birth");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.CriticalAlert)
                    .HasMaxLength(200)
                    .HasColumnName("critical_alert");

                entity.Property(e => e.Dentist)
                    .HasMaxLength(100)
                    .HasColumnName("dentist");

                entity.Property(e => e.DentistPhone)
                    .HasMaxLength(50)
                    .HasColumnName("dentist_phone");

                entity.Property(e => e.DistrictId)
                    .HasMaxLength(50)
                    .HasColumnName("district_id");

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("dob");

                entity.Property(e => e.EconomicDisadvantage).HasColumnName("economic_disadvantage");

                entity.Property(e => e.Eligibility504).HasColumnName("eligibility_504");

                entity.Property(e => e.EnrollmentType)
                    .HasMaxLength(8)
                    .HasColumnName("enrollment_type")
                    .IsFixedLength();

                entity.Property(e => e.EstimatedGradDate)
                    .HasColumnType("date")
                    .HasColumnName("estimated_grad_date");

                entity.Property(e => e.Ethnicity)
                    .HasMaxLength(50)
                    .HasColumnName("ethnicity");

                entity.Property(e => e.Facebook).HasColumnName("facebook");

                entity.Property(e => e.FirstGivenName)
                    .HasMaxLength(50)
                    .HasColumnName("first_given_name");

                entity.Property(e => e.FirstLanguageId).HasColumnName("first_language_id");

                entity.Property(e => e.FreeLunchEligibility).HasColumnName("free_lunch_eligibility");

                entity.Property(e => e.Gender)
                    .HasMaxLength(6)
                    .HasColumnName("gender");

                entity.Property(e => e.HomeAddressCity)
                    .HasMaxLength(50)
                    .HasColumnName("home_address_city");

                entity.Property(e => e.HomeAddressCountry)
                    .HasMaxLength(50)
                    .HasColumnName("home_address_country");

                entity.Property(e => e.HomeAddressLineOne)
                    .HasMaxLength(200)
                    .HasColumnName("home_address_line_one");

                entity.Property(e => e.HomeAddressLineTwo)
                    .HasMaxLength(200)
                    .HasColumnName("home_address_line_two");

                entity.Property(e => e.HomeAddressState)
                    .HasMaxLength(50)
                    .HasColumnName("home_address_state");

                entity.Property(e => e.HomeAddressZip)
                    .HasMaxLength(20)
                    .HasColumnName("home_address_zip");

                entity.Property(e => e.HomePhone)
                    .HasMaxLength(30)
                    .HasColumnName("home_phone");

                entity.Property(e => e.Instagram).HasColumnName("instagram");

                entity.Property(e => e.InsuranceCompany)
                    .HasMaxLength(200)
                    .HasColumnName("insurance_company");

                entity.Property(e => e.InsuranceCompanyPhone)
                    .HasMaxLength(50)
                    .HasColumnName("insurance_company_phone");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.LastFamilyName)
                    .HasMaxLength(50)
                    .HasColumnName("last_family_name");

                entity.Property(e => e.LepIndicator).HasColumnName("lep_indicator");

                entity.Property(e => e.Linkedin).HasColumnName("linkedin");

                entity.Property(e => e.MailingAddressCity)
                    .HasMaxLength(50)
                    .HasColumnName("mailing_address_city");

                entity.Property(e => e.MailingAddressCountry)
                    .HasMaxLength(50)
                    .HasColumnName("mailing_address_country");

                entity.Property(e => e.MailingAddressLineOne)
                    .HasMaxLength(200)
                    .HasColumnName("mailing_address_line_one");

                entity.Property(e => e.MailingAddressLineTwo)
                    .HasMaxLength(200)
                    .HasColumnName("mailing_address_line_two");

                entity.Property(e => e.MailingAddressSameToHome).HasColumnName("mailing_address_same_to_home");

                entity.Property(e => e.MailingAddressState)
                    .HasMaxLength(50)
                    .HasColumnName("mailing_address_state");

                entity.Property(e => e.MailingAddressZip)
                    .HasMaxLength(20)
                    .HasColumnName("mailing_address_zip");

                entity.Property(e => e.MaritalStatus)
                    .HasMaxLength(10)
                    .HasColumnName("marital_status");

                entity.Property(e => e.MedicalFacility)
                    .HasMaxLength(100)
                    .HasColumnName("medical_facility");

                entity.Property(e => e.MedicalFacilityPhone)
                    .HasMaxLength(50)
                    .HasColumnName("medical_facility_phone");

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(50)
                    .HasColumnName("middle_name");

                entity.Property(e => e.MobilePhone)
                    .HasMaxLength(30)
                    .HasColumnName("mobile_phone");

                entity.Property(e => e.Nationality).HasColumnName("nationality");

                entity.Property(e => e.OtherGovtIssuedNumber)
                    .HasMaxLength(50)
                    .HasColumnName("other_govt_issued_number");

                entity.Property(e => e.PersonalEmail).HasColumnName("personal_email");

                entity.Property(e => e.PolicyHolder)
                    .HasMaxLength(100)
                    .HasColumnName("policy_holder");

                entity.Property(e => e.PolicyNumber)
                    .HasMaxLength(50)
                    .HasColumnName("policy_number");

                entity.Property(e => e.PreferredName)
                    .HasMaxLength(50)
                    .HasColumnName("preferred_name");

                entity.Property(e => e.PreviousName)
                    .HasMaxLength(50)
                    .HasColumnName("previous_name");

                entity.Property(e => e.PrimaryCarePhysician)
                    .HasMaxLength(200)
                    .HasColumnName("primary_care_physician");

                entity.Property(e => e.PrimaryCarePhysicianPhone)
                    .HasMaxLength(50)
                    .HasColumnName("primary_care_physician_phone");

                entity.Property(e => e.Race)
                    .HasMaxLength(50)
                    .HasColumnName("race");

                entity.Property(e => e.RollNumber)
                    .HasMaxLength(50)
                    .HasColumnName("roll_number");

                entity.Property(e => e.Salutation)
                    .HasMaxLength(50)
                    .HasColumnName("salutation");

                entity.Property(e => e.SchoolBusDropOff).HasColumnName("school_bus_drop_off");

                entity.Property(e => e.SchoolBusPickUp).HasColumnName("school_bus_pick_up");

                entity.Property(e => e.SchoolEmail).HasColumnName("school_email");

                entity.Property(e => e.SecondLanguageId).HasColumnName("second_language_id");

                entity.Property(e => e.SectionId).HasColumnName("section_id");

                entity.Property(e => e.SocialSecurityNumber)
                    .HasMaxLength(50)
                    .HasColumnName("social_security_number");

                entity.Property(e => e.SpecialEducationIndicator).HasColumnName("special_education_indicator");

                entity.Property(e => e.StateId)
                    .HasMaxLength(50)
                    .HasColumnName("state_id");

                entity.Property(e => e.StudentGuid)
                    .HasMaxLength(36)
                    .HasColumnName("student_guid")
                    .HasDefaultValueSql("(N'00000000-0000-0000-0000-000000000000')")
                    .IsFixedLength();

                entity.Property(e => e.StudentInternalId)
                    .HasMaxLength(50)
                    .HasColumnName("student_internal_id");

                entity.Property(e => e.StudentPhoto).HasColumnName("student_photo");

                entity.Property(e => e.StudentThumbnailPhoto).HasColumnName("student_thumbnail_photo");

                entity.Property(e => e.StudentPortalId)
                    .HasMaxLength(50)
                    .HasColumnName("student_portal_id");

                entity.Property(e => e.Suffix)
                    .HasMaxLength(50)
                    .HasColumnName("suffix");

                entity.Property(e => e.ThirdLanguageId).HasColumnName("third_language_id");

                entity.Property(e => e.Twitter).HasColumnName("twitter");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.Vision)
                    .HasMaxLength(100)
                    .HasColumnName("vision");

                entity.Property(e => e.VisionPhone)
                    .HasMaxLength(50)
                    .HasColumnName("vision_phone");

                entity.Property(e => e.Youtube).HasColumnName("youtube");

                entity.HasOne(d => d.FirstLanguage)
                    .WithMany(p => p.StudentMasterFirstLanguage)
                    .HasForeignKey(d => d.FirstLanguageId)
                    .HasConstraintName("student_master$FK_student_master_language");

                entity.HasOne(d => d.SecondLanguage)
                    .WithMany(p => p.StudentMasterSecondLanguage)
                    .HasForeignKey(d => d.SecondLanguageId)
                    .HasConstraintName("student_master$FK_student_master_language1");

                entity.HasOne(d => d.ThirdLanguage)
                    .WithMany(p => p.StudentMasterThirdLanguage)
                    .HasForeignKey(d => d.ThirdLanguageId)
                    .HasConstraintName("student_master$FK_student_master_language2");

                entity.HasOne(d => d.SchoolMaster)
                    .WithMany(p => p.StudentMaster)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("student_master$FK_student_master_school_master");

                entity.HasOne(d => d.Sections)
                    .WithMany(p => p.StudentMaster)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.SectionId })
                    .HasConstraintName("student_master$FK_student_master_sections");
            });

            modelBuilder.Entity<StudentMedicalAlert>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.Id })
                    .HasName("PK_student_medical_alert_tenant_id");

                entity.ToTable("student_medical_alert");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AlertDescription).HasColumnName("alert_description");

                entity.Property(e => e.AlertType)
                    .HasMaxLength(100)
                    .HasColumnName("alert_type");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.StudentMaster)
                    .WithMany(p => p.StudentMedicalAlert)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("student_medical_alert$FK_master");
            });

            modelBuilder.Entity<StudentMedicalImmunization>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.Id })
                    .HasName("PK_student_medical_immunization_tenant_id");

                entity.ToTable("student_medical_immunization");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Comment).HasColumnName("comment");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.ImmunizationDate)
                    .HasColumnType("date")
                    .HasColumnName("immunization_date");

                entity.Property(e => e.ImmunizationType)
                    .HasMaxLength(150)
                    .HasColumnName("immunization_type");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.StudentMaster)
                    .WithMany(p => p.StudentMedicalImmunization)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("student_medical_immunization$FK_master");
            });

            modelBuilder.Entity<StudentMedicalNote>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.Id })
                    .HasName("PK_student_medical_note_tenant_id");

                entity.ToTable("student_medical_note");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.MedicalNote).HasColumnName("medical_note");

                entity.Property(e => e.NoteDate)
                    .HasColumnType("date")
                    .HasColumnName("note_date");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.StudentMaster)
                    .WithMany(p => p.StudentMedicalNote)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("student_medical_note$FK_master");
            });

            modelBuilder.Entity<StudentMedicalNurseVisit>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.Id })
                    .HasName("PK_student_medical_nurse_visit_tenant_id");

                entity.ToTable("student_medical_nurse_visit");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Comment).HasColumnName("comment");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.NurseVisitDate)
                    .HasColumnType("date")
                    .HasColumnName("nurse_visit_date");

                entity.Property(e => e.Reason)
                    .HasMaxLength(250)
                    .HasColumnName("reason");

                entity.Property(e => e.Result)
                    .HasMaxLength(250)
                    .HasColumnName("result");

                entity.Property(e => e.TimeIn)
                    .HasPrecision(0)
                    .HasColumnName("time_in");

                entity.Property(e => e.TimeOut)
                    .HasPrecision(0)
                    .HasColumnName("time_out");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.StudentMaster)
                    .WithMany(p => p.StudentMedicalNurseVisit)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("student_medical_nurse_visit$FK_master");
            });

            modelBuilder.Entity<StudentMedicalProvider>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.Id })
                    .HasName("PK_student_medical_provider_tenant_id");

                entity.ToTable("student_medical_provider");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.DentistName)
                    .HasMaxLength(150)
                    .HasColumnName("dentist_name");

                entity.Property(e => e.DentistPhone)
                    .HasMaxLength(50)
                    .HasColumnName("dentist_phone");

                entity.Property(e => e.InsuranceCompany)
                    .HasMaxLength(200)
                    .HasColumnName("insurance_company");

                entity.Property(e => e.InsuranceCompanyPhone)
                    .HasMaxLength(50)
                    .HasColumnName("insurance_company_phone");

                entity.Property(e => e.PolicyHolderName)
                    .HasMaxLength(100)
                    .HasColumnName("policy_holder_name");

                entity.Property(e => e.PolicyNumber)
                    .HasMaxLength(50)
                    .HasColumnName("policy_number");

                entity.Property(e => e.PreferredMedicalFacility)
                    .HasMaxLength(150)
                    .HasColumnName("preferred_medical_facility");

                entity.Property(e => e.PreferredMedicalFacilityPhone)
                    .HasMaxLength(50)
                    .HasColumnName("preferred_medical_facility_phone");

                entity.Property(e => e.PrimaryCarePhysician)
                    .HasMaxLength(150)
                    .HasColumnName("primary_care_physician");

                entity.Property(e => e.PrimaryCarePhysicianPhone)
                    .HasMaxLength(20)
                    .HasColumnName("primary_care_physician_phone");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.VisionName)
                    .HasMaxLength(150)
                    .HasColumnName("vision_name");

                entity.Property(e => e.VisionProviderPhone)
                    .HasMaxLength(50)
                    .HasColumnName("vision_provider_phone");

                entity.HasOne(d => d.StudentMaster)
                    .WithMany(p => p.StudentMedicalProvider)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("student_medical_provider$FK_master");
            });

            modelBuilder.Entity<StudentMissingAttendance>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.MissingAttendanceId })
                    .HasName("PK_student_missing_attendance_1");

                entity.ToTable("student_missing_attendance");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.MissingAttendanceId).HasColumnName("missing_attendance_id");

                entity.Property(e => e.AttendanceCategoryId).HasColumnName("attendance_category_id");

                entity.Property(e => e.AttendanceCode).HasColumnName("attendance_code");

                entity.Property(e => e.BlockId)
                    .HasColumnName("block_id")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CourseId)
                    .HasColumnName("course_id")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CourseSectionId)
                    .HasColumnName("course_section_id")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.MissingAttendanceDate)
                    .HasColumnType("date")
                    .HasColumnName("missing_attendance_date")
                    .HasDefaultValueSql("('0001-01-01')");

                entity.Property(e => e.PeriodId)
                    .HasColumnName("period_id")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.StaffMaster)
                    .WithMany(p => p.StudentMissingAttendances)
                    .HasForeignKey(d => new { d.TenantId, d.StaffId })
                    .HasConstraintName("FK_missing_attendance_staff");

                entity.HasOne(d => d.BlockPeriod)
                    .WithMany(p => p.StudentMissingAttendances)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.BlockId, d.PeriodId })
                    .HasConstraintName("FK_missing_attendance_block_period");

                entity.HasOne(d => d.StaffCoursesectionSchedule)
                    .WithMany(p => p.StudentMissingAttendances)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StaffId, d.CourseId, d.CourseSectionId })
                    .HasConstraintName("FK_missing_attendance_cs");
            });
            modelBuilder.Entity<StudentReportCardDetail>(entity =>
            {
                entity.ToTable("student_report_card_detail");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentId, e.SchoolYear, e.MarkingPeriodTitle }, "IX_student_report_card_detail_tenant_id_school_id_");


                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Comments)
                    .HasMaxLength(50)
                    .HasColumnName("comments");

                entity.Property(e => e.CourseName)
                    .HasMaxLength(250)
                    .HasColumnName("course_name");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Gpa)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("gpa");

                entity.Property(e => e.Grade)
                    .HasMaxLength(20)
                    .HasColumnName("grade");

                entity.Property(e => e.GradeTitle).HasColumnName("grade_title");

                entity.Property(e => e.MarkingPeriodTitle)
                    .HasMaxLength(50)
                    .HasColumnName("marking_period_title")
                    .IsFixedLength();

                entity.Property(e => e.OverallTeacherComments)
                    .HasMaxLength(250)
                    .HasColumnName("overall_teacher_comments");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.SchoolYear)
                    .HasMaxLength(9)
                    .HasColumnName("school_year");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.Teacher)
                    .HasMaxLength(250)
                    .HasColumnName("teacher");

                entity.Property(e => e.TeacherComments)
                    .HasMaxLength(50)
                    .HasColumnName("teacher_comments");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.StudentReportCardMaster)
                    .WithMany(p => p.StudentReportCardDetail)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId, d.SchoolYear, d.MarkingPeriodTitle })
                   .HasConstraintName("student_report_card_detail$FK_master");
            });

            modelBuilder.Entity<StudentReportCardMaster>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.SchoolYear, e.MarkingPeriodTitle })
                    .HasName("PK_student_report_card_master_tenant_id");

                entity.ToTable("student_report_card_master");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.SchoolYear)
                    .HasMaxLength(9)
                    .HasColumnName("school_year");

                entity.Property(e => e.MarkingPeriodTitle)
                    .HasMaxLength(50)
                    .HasColumnName("marking_period_title")
                    .HasDefaultValueSql("(N'')")
                    .IsFixedLength();

                entity.Property(e => e.Absences).HasColumnName("absences");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.ExcusedAbsences).HasColumnName("excused_absences");

                entity.Property(e => e.GradeTitle).HasColumnName("grade_title");

                entity.Property(e => e.ReportGenerationDate)
                    .HasPrecision(0)
                    .HasColumnName("report_generation_date");

                entity.Property(e => e.StudentInternalId)
                    .HasMaxLength(50)
                    .HasColumnName("student_internal_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.YodAbsence).HasColumnName("yod_absence");

                entity.Property(e => e.YodAttendance)
                    .HasMaxLength(4)
                    .HasColumnName("yod_attendance");

                entity.HasOne(d => d.StudentMaster)
                    .WithMany(p => p.StudentReportCardMaster)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("student_report_card_master$FK_master");
            });

            modelBuilder.Entity<StudentScheduleView>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.CourseId, e.CourseSectionId })
                    .HasName("PK_student_schedule_view_tenant_id");

                entity.ToTable("student_schedule_view");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.ConflictComment)
                    .HasMaxLength(300)
                    .HasColumnName("conflict_comment");

                entity.Property(e => e.CourseSectionName)
                    .HasMaxLength(200)
                    .HasColumnName("course_section_name");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Scheduled).HasColumnName("scheduled");

                entity.Property(e => e.StudentInternalId)
                    .HasMaxLength(50)
                    .HasColumnName("student_internal_id");

                entity.Property(e => e.StudentName)
                    .HasMaxLength(250)
                    .HasColumnName("student_name");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<StudentTranscriptDetail>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.TenantId, e.SchoolId, e.StudentId })
                    .HasName("PK_student_transcript_detail_id");

                entity.ToTable("student_transcript_detail");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentId, e.GradeTitle }, "IX_student_transcript_detail_tenant_id_school_id_s");


                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.CourseCode)
                    .HasMaxLength(50)
                    .HasColumnName("course_code");

                entity.Property(e => e.CourseName)
                    .HasMaxLength(250)
                    .HasColumnName("course_name");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.CreditEarned)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("credit_earned");

                entity.Property(e => e.CreditHours)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("credit_hours");

                entity.Property(e => e.GpValue)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("gp_value");

                entity.Property(e => e.Grade)
                    .HasMaxLength(10)
                    .HasColumnName("grade")
                    .IsFixedLength();

                entity.Property(e => e.GradeTitle)
                    .HasMaxLength(200)
                    .HasColumnName("grade_title")
                    .HasDefaultValueSql("(N'')");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.StudentTranscriptMaster)
                    .WithMany(p => p.StudentTranscriptDetail)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId, d.GradeTitle })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("student_transcript_detail$FK_transcript");
            });

            modelBuilder.Entity<StudentTranscriptMaster>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.GradeTitle })
                    .HasName("PK_student_transcript_master_tenant_id");

                entity.ToTable("student_transcript_master");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.GradeTitle)
                    .HasMaxLength(200)
                    .HasColumnName("grade_title");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.CreditAttempted)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("credit_attempted");

                entity.Property(e => e.CumulativeGpa)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("cumulative_gpa");

                entity.Property(e => e.GeneratedOn)
                    .HasPrecision(0)
                    .HasColumnName("generated_on");

                entity.Property(e => e.Gpa)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("gpa");

                entity.Property(e => e.SchoolName)
                    .HasMaxLength(100)
                    .HasColumnName("school_name");

                entity.Property(e => e.SchoolYear)
                    .HasMaxLength(9)
                    .HasColumnName("school_year");

                entity.Property(e => e.StudentInternalId)
                    .HasMaxLength(50)
                    .HasColumnName("student_internal_id");

                entity.Property(e => e.TotalCreditAttempted)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("total_credit_attempted");

                entity.Property(e => e.TotalCreditEarned)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("total_credit_earned");

                entity.Property(e => e.TotalGradeCreditEarned)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("total_grade_credit_earned");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.StudentMaster)
                    .WithMany(p => p.StudentTranscriptMaster)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("student_transcript_master$FK_master");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.SubjectId })
                    .HasName("PK_subject_tenant_id");

                entity.ToTable("subject");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.SubjectId).HasColumnName("subject_id");
                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");
                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.SubjectName)
                    .HasMaxLength(100)
                    .HasColumnName("subject_name");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<UserAccessLog>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.TenantId, e.SchoolId, e.LoginAttemptDate })
                    .HasName("PK_user_access_log_id");

                entity.ToTable("user_access_log");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.LoginAttemptDate)
                    .HasPrecision(0)
                    .HasColumnName("login_attempt_date");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Emailaddress)
                    .HasMaxLength(150)
                    .HasColumnName("emailaddress");

                entity.Property(e => e.Ipaddress)
                    .HasMaxLength(150)
                    .HasColumnName("ipaddress");

                entity.Property(e => e.LoginFailureCount).HasColumnName("login_failure_count");

                entity.Property(e => e.LoginStatus).HasColumnName("login_status");

                entity.Property(e => e.MembershipId).HasColumnName("membership_id");

                entity.Property(e => e.Profile)
                    .HasMaxLength(30)
                    .HasColumnName("profile");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.UserName)
                    .HasMaxLength(150)
                    .HasColumnName("user_name");
            });

            modelBuilder.Entity<UserMaster>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.EmailAddress })
                    .HasName("PK_user_master_tenant_id");

                entity.ToTable("user_master");

                entity.HasIndex(e => e.LangId, "IX_user_master_lang_id");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.MembershipId }, "IX_user_master_tenant_id_school_id_membership_id");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.EmailAddress)
                    .HasMaxLength(150)
                    .HasColumnName("emailaddress");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .HasColumnName("description");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.IsTenantadmin).HasColumnName("is_tenantadmin");

                entity.Property(e => e.LangId)
                    .HasColumnName("lang_id")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.LastUsedSchoolId).HasColumnName("last_used_school_id");

                entity.Property(e => e.LoginAttemptDate)
                    .HasColumnType("date")
                    .HasColumnName("login_attempt_date");

                entity.Property(e => e.LoginFailureCount).HasColumnName("login_failure_count");

                entity.Property(e => e.MembershipId).HasColumnName("membership_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(150)
                    .HasColumnName("name");

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(256)
                    .HasColumnName("passwordhash");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Lang)
                    .WithMany(p => p.UserMaster)
                    .HasForeignKey(d => d.LangId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_master$FK_user_master_language");

                entity.HasOne(d => d.Membership)
                    .WithMany(p => p.UserMaster)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.MembershipId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_master$FK_user_master_membership");
            });

            modelBuilder.Entity<UserSecretQuestions>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.Emailaddress })
                    .HasName("PK_user_secret_questions_tenant_id");

                entity.ToTable("user_secret_questions");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.Emailaddress)
                    .HasMaxLength(150)
                    .HasColumnName("emailaddress");

                entity.Property(e => e.Book)
                    .HasMaxLength(100)
                    .HasColumnName("book");

                entity.Property(e => e.Cartoon)
                    .HasMaxLength(100)
                    .HasColumnName("cartoon");

                entity.Property(e => e.City)
                    .HasMaxLength(100)
                    .HasColumnName("city");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Hero)
                    .HasMaxLength(100)
                    .HasColumnName("hero");

                entity.Property(e => e.Movie)
                    .HasMaxLength(100)
                    .HasColumnName("movie");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.UserMaster)
                    .WithOne(p => p.UserSecretQuestions)
                    .HasForeignKey<UserSecretQuestions>(d => new { d.TenantId, d.SchoolId, d.Emailaddress })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_secret_questions$FK_master");
            });


            //    /**************Seeding Data**********/
            LanguageSeedData(modelBuilder);
            CountrySeedData(modelBuilder);
            GradeEquivalencySeedData(modelBuilder);
            GradeEducationalStageSeedData(modelBuilder);
            GradeAgeRangeSeedData(modelBuilder);

        }


        private static void LanguageSeedData(ModelBuilder mb)
        {
            mb.Entity<Language>().HasData(new Language { LangId = 1, Lcid = "af", Locale = "Afrikaans", LanguageCode = "af" },
                new Language { LangId = 2, Lcid = "sq", Locale = "Albanian", LanguageCode = "sq" },
                new Language { LangId = 3, Lcid = "am", Locale = "Amharic", LanguageCode = "am" },
                new Language { LangId = 4, Lcid = "ar-dz", Locale = "Arabic - Algeria", LanguageCode = "ar" },
                new Language { LangId = 5, Lcid = "ar-bh", Locale = "Arabic - Bahrain", LanguageCode = "ar" },
                new Language { LangId = 6, Lcid = "ar-eg", Locale = "Arabic - Egypt", LanguageCode = "ar" },
                new Language { LangId = 7, Lcid = "ar-iq", Locale = "Arabic - Iraq", LanguageCode = "ar" },
                new Language { LangId = 8, Lcid = "ar-jo", Locale = "Arabic - Jordan", LanguageCode = "ar" },
                new Language { LangId = 9, Lcid = "ar-kw", Locale = "Arabic - Kuwait", LanguageCode = "ar" },
                new Language { LangId = 10, Lcid = "ar-lb", Locale = "Arabic - Lebanon", LanguageCode = "ar" },
                new Language { LangId = 11, Lcid = "ar-ly", Locale = "Arabic - Libya", LanguageCode = "ar" },
                new Language { LangId = 12, Lcid = "ar-ma", Locale = "Arabic - Morocco", LanguageCode = "ar" },
                new Language { LangId = 13, Lcid = "ar-om", Locale = "Arabic - Oman", LanguageCode = "ar" },
                new Language { LangId = 14, Lcid = "ar-qa", Locale = "Arabic - Qatar", LanguageCode = "ar" },
                new Language { LangId = 15, Lcid = "ar-sa", Locale = "Arabic - Saudi Arabia", LanguageCode = "ar" },
                new Language { LangId = 16, Lcid = "ar-sy", Locale = "Arabic - Syria", LanguageCode = "ar" },
                new Language { LangId = 17, Lcid = "ar-tn", Locale = "Arabic - Tunisia", LanguageCode = "ar" },
                new Language { LangId = 18, Lcid = "ar-ae", Locale = "Arabic - United Arab Emirates", LanguageCode = "ar" },
                new Language { LangId = 19, Lcid = "ar-ye", Locale = "Arabic - Yemen", LanguageCode = "ar" },
                new Language { LangId = 20, Lcid = "hy", Locale = "Armenian", LanguageCode = "hy" },
                new Language { LangId = 21, Lcid = "as", Locale = "Assamese", LanguageCode = "as" },
                new Language { LangId = 22, Lcid = "az-az", Locale = "Azeri - Cyrillic", LanguageCode = "az" },
                new Language { LangId = 23, Lcid = "az-az", Locale = "Azeri - Latin", LanguageCode = "az" },
                new Language { LangId = 24, Lcid = "eu", Locale = "Basque", LanguageCode = "eu" },
                new Language { LangId = 25, Lcid = "be", Locale = "Belarusian", LanguageCode = "be" },
                new Language { LangId = 26, Lcid = "bn", Locale = "Bengali - Bangladesh", LanguageCode = "bn" },
                new Language { LangId = 27, Lcid = "bn", Locale = "Bengali - India", LanguageCode = "bn" },
                new Language { LangId = 28, Lcid = "bs", Locale = "Bosnian", LanguageCode = "bs" },
                new Language { LangId = 29, Lcid = "bg", Locale = "Bulgarian", LanguageCode = "bg" },
                new Language { LangId = 30, Lcid = "my", Locale = "Burmese", LanguageCode = "my" },
                new Language { LangId = 31, Lcid = "ca", Locale = "Catalan", LanguageCode = "ca" },
                new Language { LangId = 32, Lcid = "zh-cn", Locale = "Chinese - China", LanguageCode = "zh" },
                new Language { LangId = 33, Lcid = "zh-hk", Locale = "Chinese - Hong Kong SAR", LanguageCode = "zh" },
                new Language { LangId = 34, Lcid = "zh-mo", Locale = "Chinese - Macau SAR", LanguageCode = "zh" },
                new Language { LangId = 35, Lcid = "zh-sg", Locale = "Chinese - Singapore", LanguageCode = "zh" },
                new Language { LangId = 36, Lcid = "zh-tw", Locale = "Chinese - Taiwan", LanguageCode = "zh" },
                new Language { LangId = 37, Lcid = "hr", Locale = "Croatian", LanguageCode = "hr" },
                new Language { LangId = 38, Lcid = "cs", Locale = "Czech", LanguageCode = "cs" },
                new Language { LangId = 39, Lcid = "da", Locale = "Danish", LanguageCode = "da" },
                new Language { LangId = 40, Lcid = "Maldivian", Locale = "Divehi", LanguageCode = "Dhivehi" },
                new Language { LangId = 41, Lcid = "nl-be", Locale = "Dutch - Belgium", LanguageCode = "nl" },
                new Language { LangId = 42, Lcid = "nl-nl", Locale = "Dutch - Netherlands", LanguageCode = "nl" },
                new Language { LangId = 43, Lcid = "en-au", Locale = "English - Australia", LanguageCode = "en" },
                new Language { LangId = 44, Lcid = "en-bz", Locale = "English - Belize", LanguageCode = "en" },
                new Language { LangId = 45, Lcid = "en-ca", Locale = "English - Canada", LanguageCode = "en" },
                new Language { LangId = 46, Lcid = "en-cb", Locale = "English - Caribbean", LanguageCode = "en" },
                new Language { LangId = 47, Lcid = "en-gb", Locale = "English - Great Britain", LanguageCode = "en" },
                new Language { LangId = 48, Lcid = "en-in", Locale = "English - India", LanguageCode = "en" },
                new Language { LangId = 49, Lcid = "en-ie", Locale = "English - Ireland", LanguageCode = "en" },
                new Language { LangId = 50, Lcid = "en-jm", Locale = "English - Jamaica", LanguageCode = "en" },
                new Language { LangId = 51, Lcid = "en-nz", Locale = "English - New Zealand", LanguageCode = "en" },
                new Language { LangId = 52, Lcid = "en-ph", Locale = "English - Philippines", LanguageCode = "en" },
                new Language { LangId = 53, Lcid = "en-za", Locale = "English - Southern Africa", LanguageCode = "en" },
                new Language { LangId = 54, Lcid = "en-tt", Locale = "English - Trinidad", LanguageCode = "en" },
                new Language { LangId = 55, Lcid = "en-us", Locale = "English - United States", LanguageCode = "en" },
                new Language { LangId = 56, Lcid = "et", Locale = "Estonian", LanguageCode = "et" },
                new Language { LangId = 57, Lcid = "mk", Locale = "FYRO Macedonia", LanguageCode = "mk" },
                new Language { LangId = 58, Lcid = "fo", Locale = "Faroese", LanguageCode = "fo" },
                new Language { LangId = 59, Lcid = "fa", Locale = "Farsi - Persian", LanguageCode = "fa" },
                new Language { LangId = 60, Lcid = "fi", Locale = "Finnish", LanguageCode = "fi" },
                new Language { LangId = 61, Lcid = "fr-be", Locale = "French - Belgium", LanguageCode = "fr" },
                new Language { LangId = 62, Lcid = "fr-ca", Locale = "French - Canada", LanguageCode = "fr" },
                new Language { LangId = 63, Lcid = "fr-fr", Locale = "French - France", LanguageCode = "fr" },
                new Language { LangId = 64, Lcid = "fr-lu", Locale = "French - Luxembourg", LanguageCode = "fr" },
                new Language { LangId = 65, Lcid = "fr-ch", Locale = "French - Switzerland", LanguageCode = "fr" },
                new Language { LangId = 66, Lcid = "gd-ie", Locale = "Gaelic - Ireland", LanguageCode = "gd" },
                new Language { LangId = 67, Lcid = "gd", Locale = "Gaelic - Scotland", LanguageCode = "gd" },
                new Language { LangId = 68, Lcid = "de-at", Locale = "German - Austria", LanguageCode = "de" },
                new Language { LangId = 69, Lcid = "de-de", Locale = "German - Germany", LanguageCode = "de" },
                new Language { LangId = 70, Lcid = "de-li", Locale = "German - Liechtenstein", LanguageCode = "de" },
                new Language { LangId = 71, Lcid = "de-lu", Locale = "German - Luxembourg", LanguageCode = "de" },
                new Language { LangId = 72, Lcid = "de-ch", Locale = "German - Switzerland", LanguageCode = "de" },
                new Language { LangId = 73, Lcid = "el", Locale = "Greek", LanguageCode = "el" },
                new Language { LangId = 74, Lcid = "gn", Locale = "Guarani - Paraguay", LanguageCode = "gn" },
                new Language { LangId = 75, Lcid = "gu", Locale = "Gujarati", LanguageCode = "gu" },
                new Language { LangId = 76, Lcid = "he", Locale = "Hebrew", LanguageCode = "he" },
                new Language { LangId = 77, Lcid = "hi", Locale = "Hindi", LanguageCode = "hi" },
                new Language { LangId = 78, Lcid = "hu", Locale = "Hungarian", LanguageCode = "hu" },
                new Language { LangId = 79, Lcid = "is", Locale = "Icelandic", LanguageCode = "is" },
                new Language { LangId = 80, Lcid = "id", Locale = "Indonesian", LanguageCode = "id" },
                new Language { LangId = 81, Lcid = "it-it", Locale = "Italian - Italy", LanguageCode = "it" },
                new Language { LangId = 82, Lcid = "it-ch", Locale = "Italian - Switzerland", LanguageCode = "it" },
                new Language { LangId = 83, Lcid = "ja", Locale = "Japanese", LanguageCode = "ja" },
                new Language { LangId = 84, Lcid = "kn", Locale = "Kannada", LanguageCode = "kn" },
                new Language { LangId = 85, Lcid = "ks", Locale = "Kashmiri", LanguageCode = "ks" },
                new Language { LangId = 86, Lcid = "kk", Locale = "Kazakh", LanguageCode = "kk" },
                new Language { LangId = 87, Lcid = "km", Locale = "Khmer", LanguageCode = "km" },
                new Language { LangId = 88, Lcid = "ko", Locale = "Korean", LanguageCode = "ko" },
                new Language { LangId = 89, Lcid = "lo", Locale = "Lao", LanguageCode = "lo" },
                new Language { LangId = 90, Lcid = "la", Locale = "Latin", LanguageCode = "la" },
                new Language { LangId = 91, Lcid = "lv", Locale = "Latvian", LanguageCode = "lv" },
                new Language { LangId = 92, Lcid = "lt", Locale = "Lithuanian", LanguageCode = "lt" },
                new Language { LangId = 93, Lcid = "ms-bn", Locale = "Malay - Brunei", LanguageCode = "ms" },
                new Language { LangId = 94, Lcid = "ms-my", Locale = "Malay - Malaysia", LanguageCode = "ms" },
                new Language { LangId = 95, Lcid = "ml", Locale = "Malayalam", LanguageCode = "ml" },
                new Language { LangId = 96, Lcid = "mt", Locale = "Maltese", LanguageCode = "mt" },
                new Language { LangId = 97, Lcid = "mi", Locale = "Maori", LanguageCode = "mi" },
                new Language { LangId = 98, Lcid = "mr", Locale = "Marathi", LanguageCode = "mr" },
                new Language { LangId = 99, Lcid = "mn", Locale = "Mongolian", LanguageCode = "mn" },
                new Language { LangId = 100, Lcid = "mn", Locale = "Mongolian", LanguageCode = "mn" },
                new Language { LangId = 101, Lcid = "ne", Locale = "Nepali", LanguageCode = "ne" },
                new Language { LangId = 102, Lcid = "no-no", Locale = "Norwegian - Bokml", LanguageCode = "nb" },
                new Language { LangId = 103, Lcid = "no-no", Locale = "Norwegian - Nynorsk", LanguageCode = "nn" },
                new Language { LangId = 104, Lcid = "or", Locale = "Oriya", LanguageCode = "or" },
                new Language { LangId = 105, Lcid = "pl", Locale = "Polish", LanguageCode = "pl" },
                new Language { LangId = 106, Lcid = "pt-br", Locale = "Portuguese - Brazil", LanguageCode = "pt" },
                new Language { LangId = 107, Lcid = "pt-pt", Locale = "Portuguese - Portugal", LanguageCode = "pt" },
                new Language { LangId = 108, Lcid = "pa", Locale = "Punjabi", LanguageCode = "pa" },
                new Language { LangId = 109, Lcid = "rm", Locale = "Raeto-Romance", LanguageCode = "rm" },
                new Language { LangId = 110, Lcid = "ro-mo", Locale = "Romanian - Moldova", LanguageCode = "ro" },
                new Language { LangId = 111, Lcid = "ro", Locale = "Romanian - Romania", LanguageCode = "ro" },
                new Language { LangId = 112, Lcid = "ru", Locale = "Russian", LanguageCode = "ru" },
                new Language { LangId = 113, Lcid = "ru-mo", Locale = "Russian - Moldova", LanguageCode = "ru" },
                new Language { LangId = 114, Lcid = "sa", Locale = "Sanskrit", LanguageCode = "sa" },
                new Language { LangId = 115, Lcid = "sr-sp", Locale = "Serbian - Cyrillic", LanguageCode = "sr" },
                new Language { LangId = 116, Lcid = "sr-sp", Locale = "Serbian - Latin", LanguageCode = "sr" },
                new Language { LangId = 117, Lcid = "tn", Locale = "Setsuana", LanguageCode = "tn" },
                new Language { LangId = 118, Lcid = "sd", Locale = "Sindhi", LanguageCode = "sd" },
                new Language { LangId = 119, Lcid = "si", Locale = "Sinhala", LanguageCode = "Sinhalese" },
                new Language { LangId = 120, Lcid = "sk", Locale = "Slovak", LanguageCode = "sk" },
                new Language { LangId = 121, Lcid = "sl", Locale = "Slovenian", LanguageCode = "sl" },
                new Language { LangId = 122, Lcid = "so", Locale = "Somali", LanguageCode = "so" },
                new Language { LangId = 123, Lcid = "sb", Locale = "Sorbian", LanguageCode = "sb" },
                new Language { LangId = 124, Lcid = "es-ar", Locale = "Spanish - Argentina", LanguageCode = "es" },
                new Language { LangId = 125, Lcid = "es-bo", Locale = "Spanish - Bolivia", LanguageCode = "es" },
                new Language { LangId = 126, Lcid = "es-cl", Locale = "Spanish - Chile", LanguageCode = "es" },
                new Language { LangId = 127, Lcid = "es-co", Locale = "Spanish - Colombia", LanguageCode = "es" },
                new Language { LangId = 128, Lcid = "es-cr", Locale = "Spanish - Costa Rica", LanguageCode = "es" },
                new Language { LangId = 129, Lcid = "es-do", Locale = "Spanish - Dominican Republic", LanguageCode = "es" },
                new Language { LangId = 130, Lcid = "es-ec", Locale = "Spanish - Ecuador", LanguageCode = "es" },
                new Language { LangId = 131, Lcid = "es-sv", Locale = "Spanish - El Salvador", LanguageCode = "es" },
                new Language { LangId = 132, Lcid = "es-gt", Locale = "Spanish - Guatemala", LanguageCode = "es" },
                new Language { LangId = 133, Lcid = "es-hn", Locale = "Spanish - Honduras", LanguageCode = "es" },
                new Language { LangId = 134, Lcid = "es-mx", Locale = "Spanish - Mexico", LanguageCode = "es" },
                new Language { LangId = 135, Lcid = "es-ni", Locale = "Spanish - Nicaragua", LanguageCode = "es" },
                new Language { LangId = 136, Lcid = "es-pa", Locale = "Spanish - Panama", LanguageCode = "es" },
                new Language { LangId = 137, Lcid = "es-py", Locale = "Spanish - Paraguay", LanguageCode = "es" },
                new Language { LangId = 138, Lcid = "es-pe", Locale = "Spanish - Peru", LanguageCode = "es" },
                new Language { LangId = 139, Lcid = "es-pr", Locale = "Spanish - Puerto Rico", LanguageCode = "es" },
                new Language { LangId = 140, Lcid = "es-es", Locale = "Spanish - Spain (Traditional)", LanguageCode = "es" },
                new Language { LangId = 141, Lcid = "es-uy", Locale = "Spanish - Uruguay", LanguageCode = "es" },
                new Language { LangId = 142, Lcid = "es-ve", Locale = "Spanish - Venezuela", LanguageCode = "es" },
                new Language { LangId = 143, Lcid = "sw", Locale = "Swahili", LanguageCode = "sw" },
                new Language { LangId = 144, Lcid = "sv-fi", Locale = "Swedish - Finland", LanguageCode = "sv" },
                new Language { LangId = 145, Lcid = "sv-se", Locale = "Swedish - Sweden", LanguageCode = "sv" },
                new Language { LangId = 146, Lcid = "tg", Locale = "Tajik", LanguageCode = "tg" },
                new Language { LangId = 147, Lcid = "ta", Locale = "Tamil", LanguageCode = "ta" },
                new Language { LangId = 148, Lcid = "tt", Locale = "Tatar", LanguageCode = "tt" },
                new Language { LangId = 149, Lcid = "te", Locale = "Telugu", LanguageCode = "te" },
                new Language { LangId = 150, Lcid = "th", Locale = "Thai", LanguageCode = "th" },
                new Language { LangId = 151, Lcid = "bo", Locale = "Tibetan", LanguageCode = "bo" },
                new Language { LangId = 152, Lcid = "ts", Locale = "Tsonga", LanguageCode = "ts" },
                new Language { LangId = 153, Lcid = "tr", Locale = "Turkish", LanguageCode = "tr" },
                new Language { LangId = 154, Lcid = "tk", Locale = "Turkmen", LanguageCode = "tk" },
                new Language { LangId = 155, Lcid = "uk", Locale = "Ukrainian", LanguageCode = "uk" },
                new Language { LangId = 157, Lcid = "ur", Locale = "Urdu", LanguageCode = "ur" },
                new Language { LangId = 158, Lcid = "uz-uz", Locale = "Uzbek - Cyrillic", LanguageCode = "uz" },
                new Language { LangId = 159, Lcid = "uz-uz", Locale = "Uzbek - Latin", LanguageCode = "uz" },
                new Language { LangId = 160, Lcid = "vi", Locale = "Vietnamese", LanguageCode = "vi" },
                new Language { LangId = 161, Lcid = "cy", Locale = "Welsh", LanguageCode = "cy" },
                new Language { LangId = 162, Lcid = "xh", Locale = "Xhosa", LanguageCode = "xh" },
                new Language { LangId = 163, Lcid = "yi", Locale = "Yiddish", LanguageCode = "yi" },
                new Language { LangId = 164, Lcid = "zu", Locale = "Zulu", LanguageCode = "zu" }
                );
          }




        private static void CountrySeedData(ModelBuilder mb)
        {
            mb.Entity<Country>().HasData(new Country { Id = 1, Name = "Afghanistan", CountryCode = "AF" },
                new Country { Id = 2, Name = "Albania", CountryCode = "AL" },
                new Country { Id = 3, Name = "Algeria", CountryCode = "DZ" },
                new Country { Id = 4, Name = "American Samoa", CountryCode = "AS" },
                new Country { Id = 5, Name = "Andorra", CountryCode = "AD" },
                new Country { Id = 6, Name = "Angola", CountryCode = "AO" },
                new Country { Id = 7, Name = "Anguilla", CountryCode = "AI" },
                new Country { Id = 8, Name = "Antarctica", CountryCode = "AQ" },
                new Country { Id = 9, Name = "Antigua And Barbuda", CountryCode = "AG" },
                new Country { Id = 10, Name = "Argentina", CountryCode = "AR" },
                new Country { Id = 11, Name = "Armenia", CountryCode = "AM" },
                new Country { Id = 12, Name = "Aruba", CountryCode = "AW" },
                new Country { Id = 13, Name = "Australia", CountryCode = "AU" },
                new Country { Id = 14, Name = "Austria", CountryCode = "AT" },
                new Country { Id = 15, Name = "Azerbaijan", CountryCode = "AZ" },
                new Country { Id = 16, Name = "Bahamas The", CountryCode = "BS" },
                new Country { Id = 17, Name = "Bahrain", CountryCode = "BH" },
                new Country { Id = 18, Name = "Bangladesh", CountryCode = "BD" },
                new Country { Id = 19, Name = "Barbados", CountryCode = "BB" },
                new Country { Id = 20, Name = "Belarus", CountryCode = "BY" },
                new Country { Id = 21, Name = "Belgium", CountryCode = "BE" },
                new Country { Id = 22, Name = "Belize", CountryCode = "BZ" },
                new Country { Id = 23, Name = "Benin", CountryCode = "BJ" },
                new Country { Id = 24, Name = "Bermuda", CountryCode = "BM" },
                new Country { Id = 25, Name = "Bhutan", CountryCode = "BT" },
                new Country { Id = 26, Name = "Bolivia", CountryCode = "BO" },
                new Country { Id = 27, Name = "Bosnia and Herzegovina", CountryCode = "BA" },
                new Country { Id = 28, Name = "Botswana", CountryCode = "BW" },
                new Country { Id = 29, Name = "Bouvet Island", CountryCode = "BV" },
                new Country { Id = 30, Name = "Brazil", CountryCode = "BR" },
                new Country { Id = 31, Name = "British Indian Ocean Territory", CountryCode = "IO" },
                new Country { Id = 32, Name = "Brunei", CountryCode = "BN" },
                new Country { Id = 33, Name = "Bulgaria", CountryCode = "BG" },
                new Country { Id = 34, Name = "Burkina Faso", CountryCode = "BF" },
                new Country { Id = 35, Name = "Burundi", CountryCode = "BI" },
                new Country { Id = 36, Name = "Cambodia", CountryCode = "KH" },
                new Country { Id = 37, Name = "Cameroon", CountryCode = "CM" },
                new Country { Id = 38, Name = "Canada", CountryCode = "CA" },
                new Country { Id = 39, Name = "Cape Verde", CountryCode = "CV" },
                new Country { Id = 40, Name = "Cayman Islands", CountryCode = "KY" },
                new Country { Id = 41, Name = "Central African Republic", CountryCode = "CF" },
                new Country { Id = 42, Name = "Chad", CountryCode = "TD" },
                new Country { Id = 43, Name = "Chile", CountryCode = "CL" },
                new Country { Id = 44, Name = "China", CountryCode = "CN" },
                new Country { Id = 45, Name = "Christmas Island", CountryCode = "CX" },
                new Country { Id = 46, Name = "Cocos (Keeling) Islands", CountryCode = "CC" },
                new Country { Id = 47, Name = "Colombia", CountryCode = "CO" },
                new Country { Id = 48, Name = "Comoros", CountryCode = "KM" },
                new Country { Id = 49, Name = "Congo", CountryCode = "CG" },
                new Country { Id = 50, Name = "Congo The Democratic Republic Of The", CountryCode = "CD" },
                new Country { Id = 51, Name = "Cook Islands", CountryCode = "CK" },
                new Country { Id = 52, Name = "Costa Rica", CountryCode = "CR" },
                new Country { Id = 53, Name = "Cote D'Ivoire (Ivory Coast)", CountryCode = "CI" },
                new Country { Id = 54, Name = "Croatia (Hrvatska)", CountryCode = "HR" },
                new Country { Id = 55, Name = "Cuba", CountryCode = "CU" },
                new Country { Id = 56, Name = "Cyprus", CountryCode = "CY" },
                new Country { Id = 57, Name = "Czech Republic", CountryCode = "CZ" },
                new Country { Id = 58, Name = "Denmark", CountryCode = "DK" },
                new Country { Id = 59, Name = "Djibouti", CountryCode = "DJ" },
                new Country { Id = 60, Name = "Dominica", CountryCode = "DM" },
                new Country { Id = 61, Name = "Dominican Republic", CountryCode = "DO" },
                new Country { Id = 62, Name = "East Timor", CountryCode = "TP" },
                new Country { Id = 63, Name = "Ecuador", CountryCode = "EC" },
                new Country { Id = 64, Name = "Egypt", CountryCode = "EG" },
                new Country { Id = 65, Name = "El Salvador", CountryCode = "SV" },
                new Country { Id = 66, Name = "Equatorial Guinea", CountryCode = "GQ" },
                new Country { Id = 67, Name = "Eritrea", CountryCode = "ER" },
                new Country { Id = 68, Name = "Estonia", CountryCode = "EE" },
                new Country { Id = 69, Name = "Ethiopia", CountryCode = "ET" },
                new Country { Id = 70, Name = "External Territories of Australia", CountryCode = "XA" },
                new Country { Id = 71, Name = "Falkland Islands", CountryCode = "FK" },
                new Country { Id = 72, Name = "Faroe Islands", CountryCode = "FO" },
                new Country { Id = 73, Name = "Fiji Islands", CountryCode = "FJ" },
                new Country { Id = 74, Name = "Finland", CountryCode = "FI" },
                new Country { Id = 75, Name = "France", CountryCode = "FR" },
                new Country { Id = 76, Name = "French Guiana", CountryCode = "GF" },
                new Country { Id = 77, Name = "French Polynesia", CountryCode = "PF" },
                new Country { Id = 78, Name = "French Southern Territories", CountryCode = "TF" },
                new Country { Id = 79, Name = "Gabon", CountryCode = "GA" },
                new Country { Id = 80, Name = "Gambia The", CountryCode = "GM" },
                new Country { Id = 81, Name = "Georgia", CountryCode = "GE" },
                new Country { Id = 82, Name = "Germany", CountryCode = "DE" },
                new Country { Id = 83, Name = "Ghana", CountryCode = "GH" },
                new Country { Id = 84, Name = "Gibraltar", CountryCode = "GI" },
                new Country { Id = 85, Name = "Greece", CountryCode = "GR" },
                new Country { Id = 86, Name = "Greenland", CountryCode = "GL" },
                new Country { Id = 87, Name = "Grenada", CountryCode = "GD" },
                new Country { Id = 88, Name = "Guadeloupe", CountryCode = "GP" },
                new Country { Id = 89, Name = "Guam", CountryCode = "GU" },
                new Country { Id = 90, Name = "Guatemala", CountryCode = "GT" },
                new Country { Id = 91, Name = "Guernsey and Alderney", CountryCode = "XU" },
                new Country { Id = 92, Name = "Guinea", CountryCode = "GN" },
                new Country { Id = 93, Name = "Guinea-Bissau", CountryCode = "GW" },
                new Country { Id = 94, Name = "Guyana", CountryCode = "GY" },
                new Country { Id = 95, Name = "Haiti", CountryCode = "HT" },
                new Country { Id = 96, Name = "Heard and McDonald Islands", CountryCode = "HM" },
                new Country { Id = 97, Name = "Honduras", CountryCode = "HN" },
                new Country { Id = 98, Name = "Hong Kong S.A.R.", CountryCode = "HK" },
                new Country { Id = 99, Name = "Hungary", CountryCode = "HU" },
                new Country { Id = 100, Name = "Iceland", CountryCode = "IS" },
                new Country { Id = 101, Name = "India", CountryCode = "IN" },
                new Country { Id = 102, Name = "Indonesia", CountryCode = "ID" },
                new Country { Id = 103, Name = "Iran", CountryCode = "IR" },
                new Country { Id = 104, Name = "Iraq", CountryCode = "IQ" },
                new Country { Id = 105, Name = "Ireland", CountryCode = "IE" },
                new Country { Id = 106, Name = "Israel", CountryCode = "IL" },
                new Country { Id = 107, Name = "Italy", CountryCode = "IT" },
                new Country { Id = 108, Name = "Jamaica", CountryCode = "JM" },
                new Country { Id = 109, Name = "Japan", CountryCode = "JP" },
                new Country { Id = 110, Name = "Jersey", CountryCode = "XJ" },
                new Country { Id = 111, Name = "Jordan", CountryCode = "JO" },
                new Country { Id = 112, Name = "Kazakhstan", CountryCode = "KZ" },
                new Country { Id = 113, Name = "Kenya", CountryCode = "KE" },
                new Country { Id = 114, Name = "Kiribati", CountryCode = "KI" },
                new Country { Id = 115, Name = "Korea North", CountryCode = "KP" },
                new Country { Id = 116, Name = "Korea South", CountryCode = "KR" },
                new Country { Id = 117, Name = "Kuwait", CountryCode = "KW" },
                new Country { Id = 118, Name = "Kyrgyzstan", CountryCode = "KG" },
                new Country { Id = 119, Name = "Laos", CountryCode = "LA" },
                new Country { Id = 120, Name = "Latvia", CountryCode = "LV" },
                new Country { Id = 121, Name = "Lebanon", CountryCode = "LB" },
                new Country { Id = 122, Name = "Lesotho", CountryCode = "LS" },
                new Country { Id = 123, Name = "Liberia", CountryCode = "LR" },
                new Country { Id = 124, Name = "Libya", CountryCode = "LY" },
                new Country { Id = 125, Name = "Liechtenstein", CountryCode = "LI" },
                new Country { Id = 126, Name = "Lithuania", CountryCode = "LT" },
                new Country { Id = 127, Name = "Luxembourg", CountryCode = "LU" },
                new Country { Id = 128, Name = "Macau S.A.R.", CountryCode = "MO" },
                new Country { Id = 129, Name = "Macedonia", CountryCode = "MK" },
                new Country { Id = 130, Name = "Madagascar", CountryCode = "MG" },
                new Country { Id = 131, Name = "Malawi", CountryCode = "MW" },
                new Country { Id = 132, Name = "Malaysia", CountryCode = "MY" },
                new Country { Id = 133, Name = "Maldives", CountryCode = "MV" },
                new Country { Id = 134, Name = "Mali", CountryCode = "ML" },
                new Country { Id = 135, Name = "Malta", CountryCode = "MT" },
                new Country { Id = 136, Name = "Man (Isle of)", CountryCode = "XM" },
                new Country { Id = 137, Name = "Marshall Islands", CountryCode = "MH" },
                new Country { Id = 138, Name = "Martinique", CountryCode = "MQ" },
                new Country { Id = 139, Name = "Mauritania", CountryCode = "MR" },
                new Country { Id = 140, Name = "Mauritius", CountryCode = "MU" },
                new Country { Id = 141, Name = "Mayotte", CountryCode = "YT" },
                new Country { Id = 142, Name = "Mexico", CountryCode = "MX" },
                new Country { Id = 143, Name = "Micronesia", CountryCode = "FM" },
                new Country { Id = 144, Name = "Moldova", CountryCode = "MD" },
                new Country { Id = 145, Name = "Monaco", CountryCode = "MC" },
                new Country { Id = 146, Name = "Mongolia", CountryCode = "MN" },
                new Country { Id = 147, Name = "Montserrat", CountryCode = "MS" },
                new Country { Id = 148, Name = "Morocco", CountryCode = "MA" },
                new Country { Id = 149, Name = "Mozambique", CountryCode = "MZ" },
                new Country { Id = 150, Name = "Myanmar", CountryCode = "MM" },
                new Country { Id = 151, Name = "Namibia", CountryCode = "NA" },
                new Country { Id = 152, Name = "Nauru", CountryCode = "NR" },
                new Country { Id = 153, Name = "Nepal", CountryCode = "NP" },
                new Country { Id = 154, Name = "Netherlands Antilles", CountryCode = "AN" },
                new Country { Id = 155, Name = "Netherlands The", CountryCode = "NL" },
                new Country { Id = 156, Name = "New Caledonia", CountryCode = "NC" },
                new Country { Id = 157, Name = "New Zealand", CountryCode = "NZ" },
                new Country { Id = 158, Name = "Nicaragua", CountryCode = "NI" },
                new Country { Id = 159, Name = "Niger", CountryCode = "NE" },
                new Country { Id = 160, Name = "Nigeria", CountryCode = "NG" },
                new Country { Id = 161, Name = "Niue", CountryCode = "NU" },
                new Country { Id = 162, Name = "Norfolk Island", CountryCode = "NF" },
                new Country { Id = 163, Name = "Northern Mariana Islands", CountryCode = "MP" },
                new Country { Id = 164, Name = "Norway", CountryCode = "NO" },
                new Country { Id = 165, Name = "Oman", CountryCode = "OM" },
                new Country { Id = 166, Name = "Pakistan", CountryCode = "PK" },
                new Country { Id = 167, Name = "Palau", CountryCode = "PW" },
                new Country { Id = 168, Name = "Palestinian Territory Occupied", CountryCode = "PS" },
                new Country { Id = 169, Name = "Panama", CountryCode = "PA" },
                new Country { Id = 170, Name = "Papua new Guinea", CountryCode = "PG" },
                new Country { Id = 171, Name = "Paraguay", CountryCode = "PY" },
                new Country { Id = 172, Name = "Peru", CountryCode = "PE" },
                new Country { Id = 173, Name = "Philippines", CountryCode = "PH" },
                new Country { Id = 174, Name = "Pitcairn Island", CountryCode = "PN" },
                new Country { Id = 175, Name = "Poland", CountryCode = "PL" },
                new Country { Id = 176, Name = "Portugal", CountryCode = "PT" },
                new Country { Id = 177, Name = "Puerto Rico", CountryCode = "PR" },
                new Country { Id = 178, Name = "Qatar", CountryCode = "QA" },
                new Country { Id = 179, Name = "Reunion", CountryCode = "RE" },
                new Country { Id = 180, Name = "Romania", CountryCode = "RO" },
                new Country { Id = 181, Name = "Russia", CountryCode = "RU" },
                new Country { Id = 182, Name = "Rwanda", CountryCode = "RW" },
                new Country { Id = 183, Name = "Saint Helena", CountryCode = "SH" },
                new Country { Id = 184, Name = "Saint Kitts And Nevis", CountryCode = "KN" },
                new Country { Id = 185, Name = "Saint Lucia", CountryCode = "LC" },
                new Country { Id = 186, Name = "Saint Pierre and Miquelon", CountryCode = "PM" },
                new Country { Id = 187, Name = "Saint Vincent And The Grenadines", CountryCode = "VC" },
                new Country { Id = 188, Name = "Samoa", CountryCode = "WS" },
                new Country { Id = 189, Name = "San Marino", CountryCode = "SM" },
                new Country { Id = 190, Name = "Sao Tome and Principe", CountryCode = "ST" },
                new Country { Id = 191, Name = "Saudi Arabia", CountryCode = "SA" },
                new Country { Id = 192, Name = "Senegal", CountryCode = "SN" },
                new Country { Id = 193, Name = "Serbia", CountryCode = "RS" },
                new Country { Id = 194, Name = "Seychelles", CountryCode = "SC" },
                new Country { Id = 195, Name = "Sierra Leone", CountryCode = "SL" },
                new Country { Id = 196, Name = "Singapore", CountryCode = "SG" },
                new Country { Id = 197, Name = "Slovakia", CountryCode = "SK" },
                new Country { Id = 198, Name = "Slovenia", CountryCode = "SI" },
                new Country { Id = 199, Name = "Smaller Territories of the UK", CountryCode = "XG" },
                new Country { Id = 200, Name = "Solomon Islands", CountryCode = "SB" },
                new Country { Id = 201, Name = "Somalia", CountryCode = "SO" },
                new Country { Id = 202, Name = "South Africa", CountryCode = "ZA" },
                new Country { Id = 203, Name = "South Georgia", CountryCode = "GS" },
                new Country { Id = 204, Name = "South Sudan", CountryCode = "SS" },
                new Country { Id = 205, Name = "Spain", CountryCode = "ES" },
                new Country { Id = 206, Name = "Sri Lanka", CountryCode = "LK" },
                new Country { Id = 207, Name = "Sudan", CountryCode = "SD" },
                new Country { Id = 208, Name = "Suriname", CountryCode = "SR" },
                new Country { Id = 209, Name = "Svalbard And Jan Mayen Islands", CountryCode = "SJ" },
                new Country { Id = 210, Name = "Swaziland", CountryCode = "SZ" },
                new Country { Id = 211, Name = "Sweden", CountryCode = "SE" },
                new Country { Id = 212, Name = "Switzerland", CountryCode = "CH" },
                new Country { Id = 213, Name = "Syria", CountryCode = "SY" },
                new Country { Id = 214, Name = "Taiwan", CountryCode = "TW" },
                new Country { Id = 215, Name = "Tajikistan", CountryCode = "TJ" },
                new Country { Id = 216, Name = "Tanzania", CountryCode = "TZ" },
                new Country { Id = 217, Name = "Thailand", CountryCode = "TH" },
                new Country { Id = 218, Name = "Togo", CountryCode = "TG" },
                new Country { Id = 219, Name = "Tokelau", CountryCode = "TK" },
                new Country { Id = 220, Name = "Tonga", CountryCode = "TO" },
                new Country { Id = 221, Name = "Trinidad And Tobago", CountryCode = "TT" },
                new Country { Id = 222, Name = "Tunisia", CountryCode = "TN" },
                new Country { Id = 223, Name = "Turkey", CountryCode = "TR" },
                new Country { Id = 224, Name = "Turkmenistan", CountryCode = "TM" },
                new Country { Id = 225, Name = "Turks And Caicos Islands", CountryCode = "TC" },
                new Country { Id = 226, Name = "Tuvalu", CountryCode = "TV" },
                new Country { Id = 227, Name = "Uganda", CountryCode = "UG" },
                new Country { Id = 228, Name = "Ukraine", CountryCode = "UA" },
                new Country { Id = 229, Name = "United Arab Emirates", CountryCode = "AE" },
                new Country { Id = 230, Name = "United Kingdom", CountryCode = "GB" },
                new Country { Id = 231, Name = "United States", CountryCode = "US" },
                new Country { Id = 232, Name = "United States Minor Outlying Islands", CountryCode = "UM" },
                new Country { Id = 233, Name = "Uruguay", CountryCode = "UY" },
                new Country { Id = 234, Name = "Uzbekistan", CountryCode = "UZ" },
                new Country { Id = 235, Name = "Vanuatu", CountryCode = "VU" },
                new Country { Id = 236, Name = "Vatican City State (Holy See)", CountryCode = "VA" },
                new Country { Id = 237, Name = "Venezuela", CountryCode = "VE" },
                new Country { Id = 238, Name = "Vietnam", CountryCode = "VN" },
                new Country { Id = 239, Name = "Virgin Islands (British)", CountryCode = "VG" },
                new Country { Id = 240, Name = "Virgin Islands (US)", CountryCode = "VI" },
                new Country { Id = 241, Name = "Wallis And Futuna Islands", CountryCode = "WF" },
                new Country { Id = 242, Name = "Western Sahara", CountryCode = "EH" },
                new Country { Id = 243, Name = "Yemen", CountryCode = "YE" },
                new Country { Id = 244, Name = "Yugoslavia", CountryCode = "YU" },
                new Country { Id = 245, Name = "Zambia", CountryCode = "ZM" },
                new Country { Id = 246, Name = "Zimbabwe", CountryCode = "ZW" }

            );
        }

        //private void StateSeedData(ModelBuilder mb)
        //{
        //    var statejsonList = StateData.StateJSON;
        //    var states = JsonConvert.DeserializeObject<List<State>>(statejsonList);
        //    mb.Entity<State>().HasData(states);
        //}

        //private void CitySeedData(ModelBuilder mb)
        //{
        //    //var dataText = System.IO.File.ReadAllText(@"weatherdataseed.json");
        //    var cityjsonList= CityData.CityJSON;
        //    var cities = JsonConvert.DeserializeObject<List<City>>(cityjsonList);
        //    mb.Entity<City>().HasData(cities);
        //}


        private static void GradeEquivalencySeedData(ModelBuilder mb)
        {

            mb.Entity<GradeEquivalency>().HasData(
                new GradeEquivalency { EquivalencyId = -1, GradeLevelEquivalency = "Pre-Kindergarten" },
                new GradeEquivalency { EquivalencyId = 0, GradeLevelEquivalency = "Kindergarten" },
                new GradeEquivalency { EquivalencyId = 1, GradeLevelEquivalency = "1st Grade" },
                new GradeEquivalency { EquivalencyId = 2, GradeLevelEquivalency = "2nd Grade" },
                new GradeEquivalency { EquivalencyId = 3, GradeLevelEquivalency = "3rd Grade" },
                new GradeEquivalency { EquivalencyId = 4, GradeLevelEquivalency = "4th Grade" },
                new GradeEquivalency { EquivalencyId = 5, GradeLevelEquivalency = "5th Grade" },
                new GradeEquivalency { EquivalencyId = 6, GradeLevelEquivalency = "6th Grade" },
                new GradeEquivalency { EquivalencyId = 7, GradeLevelEquivalency = "7th Grade" },
                new GradeEquivalency { EquivalencyId = 8, GradeLevelEquivalency = "8th Grade" },
                new GradeEquivalency { EquivalencyId = 9, GradeLevelEquivalency = "9th Grade" },
                new GradeEquivalency { EquivalencyId = 10, GradeLevelEquivalency = "10th Grade" },
                new GradeEquivalency { EquivalencyId = 11, GradeLevelEquivalency = "11th Grade" },
                new GradeEquivalency { EquivalencyId = 12, GradeLevelEquivalency = "12th Grade" },
                new GradeEquivalency { EquivalencyId = 13, GradeLevelEquivalency = "1st Year College" },
                new GradeEquivalency { EquivalencyId = 14, GradeLevelEquivalency = "2nd Year College" },
                new GradeEquivalency { EquivalencyId = 15, GradeLevelEquivalency = "3rd Year College" },
                new GradeEquivalency { EquivalencyId = 16, GradeLevelEquivalency = "4th Year College" },
                new GradeEquivalency { EquivalencyId = 17, GradeLevelEquivalency = "5th Year College" },
                new GradeEquivalency { EquivalencyId = 18, GradeLevelEquivalency = "6th Year College" },
                new GradeEquivalency { EquivalencyId = 19, GradeLevelEquivalency = "7th Year College" },
                new GradeEquivalency { EquivalencyId = 20, GradeLevelEquivalency = "8th Year College" }
                );
        }

        private static void GradeEducationalStageSeedData(ModelBuilder mb)
        {

            mb.Entity<GradeEducationalStage>().HasData(
                new GradeEducationalStage { IscedCode = 0, EducationalStage = "Early childhood Education" },
                new GradeEducationalStage { IscedCode = 1, EducationalStage = "Primary education" },
                new GradeEducationalStage { IscedCode = 2, EducationalStage = "Lower secondary education" },
                new GradeEducationalStage { IscedCode = 3, EducationalStage = "Upper secondary education" },
                new GradeEducationalStage { IscedCode = 4, EducationalStage = "Post-secondary non-tertiary education" },
                new GradeEducationalStage { IscedCode = 5, EducationalStage = "Short-cycle tertiary education" },
                new GradeEducationalStage { IscedCode = 6, EducationalStage = "Bachelor's degree or equivalent" },
                new GradeEducationalStage { IscedCode = 7, EducationalStage = "Master's degree or equivalent" },
                new GradeEducationalStage { IscedCode = 8, EducationalStage = "Doctoral degree or equivalent" }
                );
        }

        private static void GradeAgeRangeSeedData(ModelBuilder mb)
        {

            mb.Entity<GradeAgeRange>().HasData(
               new GradeAgeRange { AgeRangeId = 0, AgeRange = "Below 5" },
               new GradeAgeRange { AgeRangeId = 1, AgeRange = "5–6" },
               new GradeAgeRange { AgeRangeId = 2, AgeRange = "6–7" },
               new GradeAgeRange { AgeRangeId = 3, AgeRange = "7–8" },
               new GradeAgeRange { AgeRangeId = 4, AgeRange = "8–9" },
               new GradeAgeRange { AgeRangeId = 5, AgeRange = "9–10" },
               new GradeAgeRange { AgeRangeId = 6, AgeRange = "10–11" },
               new GradeAgeRange { AgeRangeId = 7, AgeRange = "11–12" },
               new GradeAgeRange { AgeRangeId = 8, AgeRange = "12–13" },
               new GradeAgeRange { AgeRangeId = 9, AgeRange = "13–14" },
               new GradeAgeRange { AgeRangeId = 10, AgeRange = "14–15" },
               new GradeAgeRange { AgeRangeId = 11, AgeRange = "15–16" },
               new GradeAgeRange { AgeRangeId = 12, AgeRange = "16–17" },
               new GradeAgeRange { AgeRangeId = 13, AgeRange = "17–18" },
               new GradeAgeRange { AgeRangeId = 14, AgeRange = "18+" }
                );
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
               
            }
        }
    }
}

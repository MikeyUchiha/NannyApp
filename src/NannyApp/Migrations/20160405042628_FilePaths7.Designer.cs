using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using NannyApp.Models;

namespace NannyApp.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20160405042628_FilePaths7")]
    partial class FilePaths7
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasAnnotation("Relational:Name", "RoleNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasAnnotation("Relational:TableName", "AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasAnnotation("Relational:TableName", "AspNetUserRoles");
                });

            modelBuilder.Entity("NannyApp.Models.Activity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ActitivityNotes");

                    b.Property<string>("Category")
                        .IsRequired();

                    b.Property<int?>("ChildId");

                    b.Property<DateTime?>("EndTime");

                    b.Property<string>("Group")
                        .IsRequired();

                    b.Property<bool>("HasEndTime");

                    b.Property<bool>("HasStartTime");

                    b.Property<bool>("HasSubCategory");

                    b.Property<DateTime?>("StartTime");

                    b.Property<string>("SubCategory");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("NannyApp.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<DateTime>("BirthDate");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Country");

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<DateTime>("EmailLinkDate");

                    b.Property<string>("FirstName");

                    b.Property<DateTime>("JoinDate");

                    b.Property<DateTime>("LastLoginDate");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasAnnotation("Relational:Name", "EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .HasAnnotation("Relational:Name", "UserNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetUsers");
                });

            modelBuilder.Entity("NannyApp.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("GroupId");

                    b.Property<string>("Name");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("NannyApp.Models.Child", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Allergies");

                    b.Property<string>("ChildNotes");

                    b.Property<int?>("ChildPhotoId");

                    b.Property<string>("Color");

                    b.Property<int?>("FamilyId");

                    b.Property<int>("Gender");

                    b.Property<string>("Name");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("NannyApp.Models.Connection", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ConnectionType");

                    b.Property<int>("FamilyId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("NannyApp.Models.Family", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("FamilyPhotoId");

                    b.Property<string>("GeneralNotes");

                    b.Property<string>("Name");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("NannyApp.Models.FilePath", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ActivityId");

                    b.Property<DateTime>("DateUploaded");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("FileName");

                    b.Property<int>("FileType");

                    b.Property<string>("FileUrl");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:DiscriminatorProperty", "Discriminator");

                    b.HasAnnotation("Relational:DiscriminatorValue", "FilePath");
                });

            modelBuilder.Entity("NannyApp.Models.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("UserId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("NannyApp.Models.SubCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CategoryId");

                    b.Property<string>("Name");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("NannyApp.Models.ActivityPhoto", b =>
                {
                    b.HasBaseType("NannyApp.Models.FilePath");

                    b.Property<int?>("ActivityId1");

                    b.HasAnnotation("Relational:DiscriminatorValue", "ActivityPhoto");
                });

            modelBuilder.Entity("NannyApp.Models.ChildPhoto", b =>
                {
                    b.HasBaseType("NannyApp.Models.FilePath");

                    b.Property<int?>("ChildId");

                    b.HasAnnotation("Relational:DiscriminatorValue", "ChildPhoto");
                });

            modelBuilder.Entity("NannyApp.Models.FamilyPhoto", b =>
                {
                    b.HasBaseType("NannyApp.Models.FilePath");

                    b.Property<int?>("FamilyId");

                    b.HasAnnotation("Relational:DiscriminatorValue", "FamilyPhoto");
                });

            modelBuilder.Entity("NannyApp.Models.ProfilePhoto", b =>
                {
                    b.HasBaseType("NannyApp.Models.FilePath");


                    b.HasAnnotation("Relational:DiscriminatorValue", "ProfilePhoto");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("NannyApp.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("NannyApp.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("NannyApp.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("NannyApp.Models.Activity", b =>
                {
                    b.HasOne("NannyApp.Models.Child")
                        .WithMany()
                        .HasForeignKey("ChildId");
                });

            modelBuilder.Entity("NannyApp.Models.Category", b =>
                {
                    b.HasOne("NannyApp.Models.Group")
                        .WithMany()
                        .HasForeignKey("GroupId");
                });

            modelBuilder.Entity("NannyApp.Models.Child", b =>
                {
                    b.HasOne("NannyApp.Models.FilePath")
                        .WithMany()
                        .HasForeignKey("ChildPhotoId");

                    b.HasOne("NannyApp.Models.Family")
                        .WithMany()
                        .HasForeignKey("FamilyId");
                });

            modelBuilder.Entity("NannyApp.Models.Connection", b =>
                {
                    b.HasOne("NannyApp.Models.Family")
                        .WithMany()
                        .HasForeignKey("FamilyId");

                    b.HasOne("NannyApp.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("NannyApp.Models.Family", b =>
                {
                    b.HasOne("NannyApp.Models.FilePath")
                        .WithMany()
                        .HasForeignKey("FamilyPhotoId");
                });

            modelBuilder.Entity("NannyApp.Models.FilePath", b =>
                {
                    b.HasOne("NannyApp.Models.Activity")
                        .WithMany()
                        .HasForeignKey("ActivityId");

                    b.HasOne("NannyApp.Models.ApplicationUser")
                        .WithOne()
                        .HasForeignKey("NannyApp.Models.FilePath", "UserId");
                });

            modelBuilder.Entity("NannyApp.Models.Group", b =>
                {
                    b.HasOne("NannyApp.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("NannyApp.Models.SubCategory", b =>
                {
                    b.HasOne("NannyApp.Models.Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");
                });

            modelBuilder.Entity("NannyApp.Models.ActivityPhoto", b =>
                {
                    b.HasOne("NannyApp.Models.Activity")
                        .WithMany()
                        .HasForeignKey("ActivityId1");
                });

            modelBuilder.Entity("NannyApp.Models.ChildPhoto", b =>
                {
                    b.HasOne("NannyApp.Models.Child")
                        .WithMany()
                        .HasForeignKey("ChildId");
                });

            modelBuilder.Entity("NannyApp.Models.FamilyPhoto", b =>
                {
                    b.HasOne("NannyApp.Models.Family")
                        .WithMany()
                        .HasForeignKey("FamilyId");
                });

            modelBuilder.Entity("NannyApp.Models.ProfilePhoto", b =>
                {
                });
        }
    }
}

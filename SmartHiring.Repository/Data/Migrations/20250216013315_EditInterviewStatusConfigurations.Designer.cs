﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartHiring.Repository.Data;

#nullable disable

namespace SmartHiring.Repository.Data.Migrations
{
    [DbContext(typeof(SmartHiringDbContext))]
    [Migration("20250216013315_EditInterviewStatusConfigurations")]
    partial class EditInterviewStatusConfigurations
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.36")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("SmartHiring.Core.Entities.Admin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Admins", (string)null);
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.Agency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("ApplicantPool")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.ToTable("Agencies", (string)null);
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.AgencyApplicant", b =>
                {
                    b.Property<int>("AgencyId")
                        .HasColumnType("int");

                    b.Property<int>("ApplicantId")
                        .HasColumnType("int");

                    b.HasKey("AgencyId", "ApplicantId");

                    b.HasIndex("ApplicantId");

                    b.ToTable("AgencyApplicants", (string)null);
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.Applicant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("date");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("FName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("LName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Skills")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.ToTable("Applicants", (string)null);
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.ApplicantAddress", b =>
                {
                    b.Property<int>("ApplicantId")
                        .HasColumnType("int");

                    b.Property<string>("Country")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ZIP")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("ApplicantId", "Country");

                    b.ToTable("Applicant_Addresses", (string)null);
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.ApplicantPhone", b =>
                {
                    b.Property<int>("ApplicantId")
                        .HasColumnType("int");

                    b.Property<string>("Phone")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("ApplicantId", "Phone");

                    b.ToTable("Applicant_Phones", (string)null);
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.Application", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("AgencyId")
                        .HasColumnType("int");

                    b.Property<int>("ApplicantId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ApplicationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CV_Link")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("IsShortlisted")
                        .HasColumnType("bit");

                    b.Property<int>("PostId")
                        .HasColumnType("int");

                    b.Property<double>("RankScore")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("AgencyId");

                    b.HasIndex("ApplicantId");

                    b.HasIndex("PostId");

                    b.ToTable("Applications", (string)null);
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.CandidateList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CompanyId")
                        .HasColumnType("int");

                    b.Property<DateTime>("GeneratedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PostId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("PostId");

                    b.ToTable("CandidateLists", (string)null);
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.CandidateListApplicant", b =>
                {
                    b.Property<int>("CandidateListId")
                        .HasColumnType("int");

                    b.Property<int>("ApplicantId")
                        .HasColumnType("int");

                    b.HasKey("CandidateListId", "ApplicantId");

                    b.HasIndex("ApplicantId");

                    b.ToTable("CandidateListApplicants", (string)null);
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AdminId")
                        .HasColumnType("int");

                    b.Property<string>("BusinessEmail")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Industry")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<int>("ManagerId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.HasIndex("AdminId");

                    b.HasIndex("ManagerId")
                        .IsUnique();

                    b.ToTable("Companies", (string)null);
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.CompanyPhone", b =>
                {
                    b.Property<int>("CompanyId")
                        .HasColumnType("int");

                    b.Property<string>("Phone")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("CompanyId", "Phone");

                    b.ToTable("Company_Phones", (string)null);
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.HR", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CompanyId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("HRs", (string)null);
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.Interview", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("ApplicantId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<int>("HRId")
                        .HasColumnType("int");

                    b.Property<int>("InterviewStatus")
                        .HasMaxLength(50)
                        .HasColumnType("int");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("PostId")
                        .HasColumnType("int");

                    b.Property<double>("Score")
                        .HasColumnType("float");

                    b.Property<TimeSpan>("Time")
                        .HasColumnType("time");

                    b.HasKey("Id");

                    b.HasIndex("ApplicantId");

                    b.HasIndex("HRId");

                    b.HasIndex("PostId");

                    b.ToTable("Interviews", (string)null);
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.Manager", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Managers", (string)null);
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("CompanyId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Deadline")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("HRId")
                        .HasColumnType("int");

                    b.Property<decimal>("JobSalary")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("JobStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JobTitle")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<DateTime>("PostDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Requirements")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("HRId");

                    b.ToTable("Posts", (string)null);
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.AgencyApplicant", b =>
                {
                    b.HasOne("SmartHiring.Core.Entities.Agency", "Agency")
                        .WithMany("AgencyApplicants")
                        .HasForeignKey("AgencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartHiring.Core.Entities.Applicant", "Applicant")
                        .WithMany()
                        .HasForeignKey("ApplicantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Agency");

                    b.Navigation("Applicant");
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.ApplicantAddress", b =>
                {
                    b.HasOne("SmartHiring.Core.Entities.Applicant", "Applicant")
                        .WithMany("ApplicantAddresses")
                        .HasForeignKey("ApplicantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Applicant");
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.ApplicantPhone", b =>
                {
                    b.HasOne("SmartHiring.Core.Entities.Applicant", "Applicant")
                        .WithMany("ApplicantPhones")
                        .HasForeignKey("ApplicantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Applicant");
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.Application", b =>
                {
                    b.HasOne("SmartHiring.Core.Entities.Agency", "Agency")
                        .WithMany("Applications")
                        .HasForeignKey("AgencyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SmartHiring.Core.Entities.Applicant", "Applicant")
                        .WithMany("Applications")
                        .HasForeignKey("ApplicantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartHiring.Core.Entities.Post", "Post")
                        .WithMany("Applications")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Agency");

                    b.Navigation("Applicant");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.CandidateList", b =>
                {
                    b.HasOne("SmartHiring.Core.Entities.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("SmartHiring.Core.Entities.Post", "Post")
                        .WithMany("CandidateLists")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Company");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.CandidateListApplicant", b =>
                {
                    b.HasOne("SmartHiring.Core.Entities.Applicant", "Applicant")
                        .WithMany("CandidateListApplicants")
                        .HasForeignKey("ApplicantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartHiring.Core.Entities.CandidateList", "CandidateList")
                        .WithMany("CandidateListApplicants")
                        .HasForeignKey("CandidateListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Applicant");

                    b.Navigation("CandidateList");
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.Company", b =>
                {
                    b.HasOne("SmartHiring.Core.Entities.Admin", "Admin")
                        .WithMany("Companies")
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("SmartHiring.Core.Entities.Manager", "Manager")
                        .WithOne("Company")
                        .HasForeignKey("SmartHiring.Core.Entities.Company", "ManagerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Admin");

                    b.Navigation("Manager");
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.CompanyPhone", b =>
                {
                    b.HasOne("SmartHiring.Core.Entities.Company", "Company")
                        .WithMany("CompanyPhones")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.HR", b =>
                {
                    b.HasOne("SmartHiring.Core.Entities.Company", "Company")
                        .WithMany("HRs")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.Interview", b =>
                {
                    b.HasOne("SmartHiring.Core.Entities.Applicant", "Applicant")
                        .WithMany()
                        .HasForeignKey("ApplicantId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("SmartHiring.Core.Entities.HR", "HR")
                        .WithMany("Interviews")
                        .HasForeignKey("HRId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("SmartHiring.Core.Entities.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Applicant");

                    b.Navigation("HR");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.Post", b =>
                {
                    b.HasOne("SmartHiring.Core.Entities.Company", null)
                        .WithMany("Posts")
                        .HasForeignKey("CompanyId");

                    b.HasOne("SmartHiring.Core.Entities.HR", "HR")
                        .WithMany("Posts")
                        .HasForeignKey("HRId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("HR");
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.Admin", b =>
                {
                    b.Navigation("Companies");
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.Agency", b =>
                {
                    b.Navigation("AgencyApplicants");

                    b.Navigation("Applications");
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.Applicant", b =>
                {
                    b.Navigation("ApplicantAddresses");

                    b.Navigation("ApplicantPhones");

                    b.Navigation("Applications");

                    b.Navigation("CandidateListApplicants");
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.CandidateList", b =>
                {
                    b.Navigation("CandidateListApplicants");
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.Company", b =>
                {
                    b.Navigation("CompanyPhones");

                    b.Navigation("HRs");

                    b.Navigation("Posts");
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.HR", b =>
                {
                    b.Navigation("Interviews");

                    b.Navigation("Posts");
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.Manager", b =>
                {
                    b.Navigation("Company")
                        .IsRequired();
                });

            modelBuilder.Entity("SmartHiring.Core.Entities.Post", b =>
                {
                    b.Navigation("Applications");

                    b.Navigation("CandidateLists");
                });
#pragma warning restore 612, 618
        }
    }
}

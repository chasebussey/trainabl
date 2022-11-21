﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Trainabl.Server;

#nullable disable

namespace Trainabl.Server.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20221121210805_AddWorkoutNotes")]
    partial class AddWorkoutNotes
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Trainabl.Shared.Models.ClientProfile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TrainerProfileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UserSettingsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TrainerProfileId");

                    b.HasIndex("UserSettingsId");

                    b.ToTable("ClientProfiles");
                });

            modelBuilder.Entity("Trainabl.Shared.Models.Metric", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ClientProfileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Unit")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Value")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("ClientProfileId");

                    b.ToTable("Metrics");
                });

            modelBuilder.Entity("Trainabl.Shared.Models.Movement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("RequiresEquipment")
                        .HasColumnType("bit");

                    b.Property<string>("TargetMuscleGroup")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Movements");
                });

            modelBuilder.Entity("Trainabl.Shared.Models.TrainerProfile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserSettingsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserSettingsId");

                    b.ToTable("TrainerProfiles");
                });

            modelBuilder.Entity("Trainabl.Shared.Models.UserSettings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("PreferLightMode")
                        .HasColumnType("bit");

                    b.Property<bool>("PreferMiniDrawer")
                        .HasColumnType("bit");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("UserSettings");
                });

            modelBuilder.Entity("Trainabl.Shared.Models.Workout", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ClientProfileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Exercises")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDraft")
                        .HasColumnType("bit");

                    b.Property<bool>("IsTemplate")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TrainerProfileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("WorkoutType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ClientProfileId");

                    b.HasIndex("TrainerProfileId");

                    b.ToTable("Workouts");
                });

            modelBuilder.Entity("Trainabl.Shared.Models.WorkoutNote", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDateUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExerciseNotes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("WorkoutId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("WorkoutId");

                    b.ToTable("WorkoutNotes");
                });

            modelBuilder.Entity("Trainabl.Shared.Models.ClientProfile", b =>
                {
                    b.HasOne("Trainabl.Shared.Models.TrainerProfile", "TrainerProfile")
                        .WithMany("ClientProfiles")
                        .HasForeignKey("TrainerProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Trainabl.Shared.Models.UserSettings", "UserSettings")
                        .WithMany()
                        .HasForeignKey("UserSettingsId");

                    b.Navigation("TrainerProfile");

                    b.Navigation("UserSettings");
                });

            modelBuilder.Entity("Trainabl.Shared.Models.Metric", b =>
                {
                    b.HasOne("Trainabl.Shared.Models.ClientProfile", null)
                        .WithMany("Metrics")
                        .HasForeignKey("ClientProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Trainabl.Shared.Models.TrainerProfile", b =>
                {
                    b.HasOne("Trainabl.Shared.Models.UserSettings", "UserSettings")
                        .WithMany()
                        .HasForeignKey("UserSettingsId");

                    b.Navigation("UserSettings");
                });

            modelBuilder.Entity("Trainabl.Shared.Models.Workout", b =>
                {
                    b.HasOne("Trainabl.Shared.Models.ClientProfile", "ClientProfile")
                        .WithMany("Workouts")
                        .HasForeignKey("ClientProfileId");

                    b.HasOne("Trainabl.Shared.Models.TrainerProfile", "TrainerProfile")
                        .WithMany("Workouts")
                        .HasForeignKey("TrainerProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ClientProfile");

                    b.Navigation("TrainerProfile");
                });

            modelBuilder.Entity("Trainabl.Shared.Models.WorkoutNote", b =>
                {
                    b.HasOne("Trainabl.Shared.Models.Workout", null)
                        .WithMany("WorkoutNotes")
                        .HasForeignKey("WorkoutId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Trainabl.Shared.Models.ClientProfile", b =>
                {
                    b.Navigation("Metrics");

                    b.Navigation("Workouts");
                });

            modelBuilder.Entity("Trainabl.Shared.Models.TrainerProfile", b =>
                {
                    b.Navigation("ClientProfiles");

                    b.Navigation("Workouts");
                });

            modelBuilder.Entity("Trainabl.Shared.Models.Workout", b =>
                {
                    b.Navigation("WorkoutNotes");
                });
#pragma warning restore 612, 618
        }
    }
}

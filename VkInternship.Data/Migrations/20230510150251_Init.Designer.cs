﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VkInternship.Data;

#nullable disable

namespace VkInternship.Data.Migrations
{
    [DbContext(typeof(UsersContext))]
    [Migration("20230510150251_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("content")
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("VkInternship.Data.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<int>("GroupId")
                        .HasColumnType("integer")
                        .HasColumnName("group_id");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("login");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.Property<int>("StateId")
                        .HasColumnType("integer")
                        .HasColumnName("state_id");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("GroupId")
                        .HasDatabaseName("ix_users_group_id");

                    b.HasIndex("StateId")
                        .HasDatabaseName("ix_users_state_id");

                    b.HasIndex("Id", "StateId")
                        .HasDatabaseName("ix_users_id_state_id");

                    b.ToTable("users", "content");
                });

            modelBuilder.Entity("VkInternship.Data.Entities.UserGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.HasKey("Id")
                        .HasName("pk_user_groups");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasDatabaseName("ix_user_groups_code");

                    b.ToTable("user_groups", "content");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Code = "Admin",
                            Description = ""
                        },
                        new
                        {
                            Id = 2,
                            Code = "User",
                            Description = ""
                        });
                });

            modelBuilder.Entity("VkInternship.Data.Entities.UserState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.HasKey("Id")
                        .HasName("pk_user_states");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasDatabaseName("ix_user_states_code");

                    b.ToTable("user_states", "content");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Code = "Active",
                            Description = ""
                        },
                        new
                        {
                            Id = 2,
                            Code = "Blocked",
                            Description = ""
                        });
                });

            modelBuilder.Entity("VkInternship.Data.Entities.User", b =>
                {
                    b.HasOne("VkInternship.Data.Entities.UserGroup", "Group")
                        .WithMany("Users")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_users_user_groups_group_id");

                    b.HasOne("VkInternship.Data.Entities.UserState", "State")
                        .WithMany("Users")
                        .HasForeignKey("StateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_users_user_states_state_id");

                    b.Navigation("Group");

                    b.Navigation("State");
                });

            modelBuilder.Entity("VkInternship.Data.Entities.UserGroup", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("VkInternship.Data.Entities.UserState", b =>
                {
                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}

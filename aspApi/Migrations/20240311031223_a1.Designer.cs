﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using aspApi.Data;

#nullable disable

namespace aspApi.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20240311031223_a1")]
    partial class a1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TodoApi.Models.TodoItem", b =>
                {
                    b.Property<long>("TodoItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("TodoItemId"));

                    b.Property<bool>("IsComplete")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TeamId")
                        .HasColumnType("int");

                    b.HasKey("TodoItemId");

                    b.HasIndex("TeamId");

                    b.ToTable("TodoItems");
                });

            modelBuilder.Entity("aspApi.Models.Team", b =>
                {
                    b.Property<int>("TeamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TeamId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TeamId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("aspApi.Models.TeamUser", b =>
                {
                    b.Property<int>("TeamId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<bool>("IsLeader")
                        .HasColumnType("bit");

                    b.HasKey("TeamId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("TeamUsers");
                });

            modelBuilder.Entity("aspApi.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TeamId")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.HasIndex("TeamId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("TodoApi.Models.TodoItem", b =>
                {
                    b.HasOne("aspApi.Models.Team", null)
                        .WithMany("TodoItems")
                        .HasForeignKey("TeamId");
                });

            modelBuilder.Entity("aspApi.Models.TeamUser", b =>
                {
                    b.HasOne("aspApi.Models.Team", "Team")
                        .WithMany("TeamUsers")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("aspApi.Models.User", "User")
                        .WithMany("TeamUsers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Team");

                    b.Navigation("User");
                });

            modelBuilder.Entity("aspApi.Models.User", b =>
                {
                    b.HasOne("aspApi.Models.Team", null)
                        .WithMany("Users")
                        .HasForeignKey("TeamId");
                });

            modelBuilder.Entity("aspApi.Models.Team", b =>
                {
                    b.Navigation("TeamUsers");

                    b.Navigation("TodoItems");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("aspApi.Models.User", b =>
                {
                    b.Navigation("TeamUsers");
                });
#pragma warning restore 612, 618
        }
    }
}

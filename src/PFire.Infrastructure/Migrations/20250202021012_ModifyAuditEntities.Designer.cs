﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PFire.Infrastructure.Services;

#nullable disable

namespace PFire.Infrastructure.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20250202021012_ModifyAuditEntities")]
    partial class ModifyAuditEntities
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.1");

            modelBuilder.Entity("PFire.Infrastructure.Entities.Friend", b =>
                {
                    b.Property<int>("MeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ThemId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("TEXT");

                    b.Property<string>("Message")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<bool>("Pending")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("MeId", "ThemId");

                    b.HasIndex("ThemId");

                    b.HasIndex("MeId", "ThemId")
                        .IsUnique();

                    b.ToTable("Friend", (string)null);
                });

            modelBuilder.Entity("PFire.Infrastructure.Entities.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<int>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Group");
                });

            modelBuilder.Entity("PFire.Infrastructure.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("TEXT");

                    b.Property<string>("Nickname")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("Username");

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("PFire.Infrastructure.Entities.Friend", b =>
                {
                    b.HasOne("PFire.Infrastructure.Entities.User", "Me")
                        .WithMany("MyFriends")
                        .HasForeignKey("MeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PFire.Infrastructure.Entities.User", "Them")
                        .WithMany("FriendsOf")
                        .HasForeignKey("ThemId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Me");

                    b.Navigation("Them");
                });

            modelBuilder.Entity("PFire.Infrastructure.Entities.Group", b =>
                {
                    b.HasOne("PFire.Infrastructure.Entities.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("PFire.Infrastructure.Entities.User", b =>
                {
                    b.Navigation("FriendsOf");

                    b.Navigation("MyFriends");
                });
#pragma warning restore 612, 618
        }
    }
}

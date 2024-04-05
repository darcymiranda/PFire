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
    [Migration("20240401151159_UserPrefs")]
    partial class UserPrefs
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.1");

            modelBuilder.Entity("PFire.Infrastructure.Entities.Friend", b =>
                {
                    b.Property<int>("MeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ThemId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateModified")
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

            modelBuilder.Entity("PFire.Infrastructure.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("TEXT");

                    b.Property<string>("Nickname")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("PlaySoundInChatrooms")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("PlaySoundOnNewMessages")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("PlaySoundOnScreenshots")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("PlaySoundOnVoicecalls")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("PlaySoundsOnLogOn")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("PlaySoundsOnNewMessagesInGame")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("ShowFriendsOfFriends")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ShowGameDataOnProfile")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ShowGameServerData")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ShowGameStatusToFriends")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ShowNicknames")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ShowOfflineFriends")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ShowTimeStampInChat")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ShowTooltipOnDownload")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ShowTooltipOnLogOn")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ShowTyping")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ShowVoiceChatServer")
                        .HasColumnType("INTEGER");

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

            modelBuilder.Entity("PFire.Infrastructure.Entities.User", b =>
                {
                    b.Navigation("FriendsOf");

                    b.Navigation("MyFriends");
                });
#pragma warning restore 612, 618
        }
    }
}

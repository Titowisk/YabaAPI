﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Yaba.Infrastructure.Persistence.Context;

namespace Yaba.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20200620211632_BankAccount_Code")]
    partial class BankAccount_Code
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Yaba.Domain.Models.BankAccounts.BankAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("BK_Id")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Agency")
                        .IsRequired()
                        .HasColumnName("BK_Agency")
                        .HasColumnType("nvarchar(max)");

                    b.Property<short>("Code")
                        .HasColumnName("BK_Code")
                        .HasColumnType("smallint");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasColumnName("BK_Number")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("BankAccounts");
                });

            modelBuilder.Entity("Yaba.Domain.Models.Transactions.Transaction", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("TR_Id")
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnName("TR_Amount")
                        .HasColumnType("decimal(12,2)");

                    b.Property<int?>("BankAccountId")
                        .HasColumnType("int");

                    b.Property<short?>("Category")
                        .HasColumnName("TR_Category")
                        .HasColumnType("smallint");

                    b.Property<DateTime>("Date")
                        .HasColumnName("TR_Date")
                        .HasColumnType("date");

                    b.Property<string>("Origin")
                        .HasColumnName("TR_Origin")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("BankAccountId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Yaba.Domain.Models.Transactions.Transaction", b =>
                {
                    b.HasOne("YabaAPI.Models.BankAccount", "BankAccount")
                        .WithMany("Transactions")
                        .HasForeignKey("BankAccountId");
                });
#pragma warning restore 612, 618
        }
    }
}

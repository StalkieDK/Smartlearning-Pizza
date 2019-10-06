﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pizza.API.Data;

namespace Pizza.API.Migrations
{
    [DbContext(typeof(PizzaAPIContext))]
    [Migration("20191005135431_Number")]
    partial class Number
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Pizza.API.Models.MenuCategoryModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("MenuCategoryModel");
                });

            modelBuilder.Entity("Pizza.API.Models.MenuItemModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CategoryID");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<string>("Number");

                    b.Property<decimal>("Price")
                        .HasColumnType("MONEY");

                    b.HasKey("ID");

                    b.HasIndex("CategoryID");

                    b.ToTable("MenuItemModel");
                });

            modelBuilder.Entity("Pizza.API.Models.MenuItemModel", b =>
                {
                    b.HasOne("Pizza.API.Models.MenuCategoryModel", "Category")
                        .WithMany("MenuItems")
                        .HasForeignKey("CategoryID");
                });
#pragma warning restore 612, 618
        }
    }
}

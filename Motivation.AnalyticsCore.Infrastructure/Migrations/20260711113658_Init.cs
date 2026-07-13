using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Motivation.AnalyticsCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "customer_group",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentGroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_group", x => x.Id);
                    table.ForeignKey(
                        name: "FK_customer_group_customer_group_ParentGroupId",
                        column: x => x.ParentGroupId,
                        principalTable: "customer_group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Content = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Error = table.Column<string>(type: "character varying(8192)", maxLength: 8192, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "product_group",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentGroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_group", x => x.Id);
                    table.ForeignKey(
                        name: "FK_product_group_product_group_ParentGroupId",
                        column: x => x.ParentGroupId,
                        principalTable: "product_group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "warehouses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CustomerGroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    MasterCustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Manager = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_customers_customer_group_CustomerGroupId",
                        column: x => x.CustomerGroupId,
                        principalTable: "customer_group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_customers_customers_MasterCustomerId",
                        column: x => x.MasterCustomerId,
                        principalTable: "customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Supplier = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Purchaser = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Status = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductGroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_products_product_group_ProductGroupId",
                        column: x => x.ProductGroupId,
                        principalTable: "product_group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_products_warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "customer_transaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    Responsible = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AsOfDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_transaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_customer_transaction_companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_customer_transaction_customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "receivables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    AsOfDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_receivables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_receivables_companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_receivables_customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inventory_balances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    AsOfDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_balances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_inventory_balances_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_inventory_balances_warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "warehouse_transaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: true),
                    AsOfDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouse_transaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_warehouse_transaction_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_warehouse_transaction_warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "customer_transaction_products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CustomerTransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_transaction_products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_customer_transaction_products_customer_transaction_Customer~",
                        column: x => x.CustomerTransactionId,
                        principalTable: "customer_transaction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_customer_transaction_products_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_companies_Code",
                table: "companies",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_companies_Title",
                table: "companies",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_customer_group_Code",
                table: "customer_group",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_customer_group_ParentGroupId",
                table: "customer_group",
                column: "ParentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_customer_group_Title",
                table: "customer_group",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_customer_transaction_Code",
                table: "customer_transaction",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_customer_transaction_CompanyId",
                table: "customer_transaction",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_customer_transaction_CustomerId",
                table: "customer_transaction",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_customer_transaction_Responsible",
                table: "customer_transaction",
                column: "Responsible");

            migrationBuilder.CreateIndex(
                name: "IX_customer_transaction_Type",
                table: "customer_transaction",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_customer_transaction_products_CustomerTransactionId",
                table: "customer_transaction_products",
                column: "CustomerTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_customer_transaction_products_ProductId",
                table: "customer_transaction_products",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_customers_Code",
                table: "customers",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_customers_CustomerGroupId",
                table: "customers",
                column: "CustomerGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_customers_FullName",
                table: "customers",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_customers_MasterCustomerId",
                table: "customers",
                column: "MasterCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_balances_ProductId",
                table: "inventory_balances",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_balances_WarehouseId",
                table: "inventory_balances",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_product_group_Code",
                table: "product_group",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_product_group_ParentGroupId",
                table: "product_group",
                column: "ParentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_product_group_Title",
                table: "product_group",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_products_Code",
                table: "products",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_products_ProductGroupId",
                table: "products",
                column: "ProductGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_products_WarehouseId",
                table: "products",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_receivables_CompanyId",
                table: "receivables",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_receivables_CustomerId",
                table: "receivables",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_warehouse_transaction_Code",
                table: "warehouse_transaction",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_warehouse_transaction_ProductId",
                table: "warehouse_transaction",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_warehouse_transaction_Type",
                table: "warehouse_transaction",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_warehouse_transaction_WarehouseId",
                table: "warehouse_transaction",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_warehouses_Code",
                table: "warehouses",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customer_transaction_products");

            migrationBuilder.DropTable(
                name: "inventory_balances");

            migrationBuilder.DropTable(
                name: "outbox_messages");

            migrationBuilder.DropTable(
                name: "receivables");

            migrationBuilder.DropTable(
                name: "warehouse_transaction");

            migrationBuilder.DropTable(
                name: "customer_transaction");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "companies");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "product_group");

            migrationBuilder.DropTable(
                name: "warehouses");

            migrationBuilder.DropTable(
                name: "customer_group");
        }
    }
}

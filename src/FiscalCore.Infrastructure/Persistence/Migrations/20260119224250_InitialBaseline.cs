using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiscalCore.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ErrorLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(sysdatetime())"),
                    MethodName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ExceptionMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExceptionStackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExceptionString = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SatCatalogRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CatalogCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemKey = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AppliesToCatalog = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AppliesToKey = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsAllowed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SatCatalogRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SatCatalogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CfdiVersion = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SatCatalogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SatCatalogItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    SatCatalogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeyCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SatCatalogItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SatCatalogItems_SatCatalogs",
                        column: x => x.SatCatalogId,
                        principalTable: "SatCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(sysdatetime())"),
                    EventType = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logs_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    CertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rfc = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CertificateType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    ValidTo = table.Column<DateOnly>(type: "date", nullable: false),
                    CerFile = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    KeyFile = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    EncryptedKeyPassword = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.CertificateId);
                    table.ForeignKey(
                        name: "FK_Certificates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CfdiBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CfdiBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CfdiBatches_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cfdis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RfcEmisor = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    RfcReceptor = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cfdis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cfdis_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MassDownloadRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "date", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MassDownloadRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MassDownloadRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StampBalances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalStamps = table.Column<int>(type: "int", nullable: false),
                    UsedStamps = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StampBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StampBalances_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BatchItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CfdiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchItems_CfdiBatches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "CfdiBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatchItems_Cfdis_CfdiId",
                        column: x => x.CfdiId,
                        principalTable: "Cfdis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CfdiPdfs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CfdiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CfdiPdfs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CfdiPdfs_Cfdis_CfdiId",
                        column: x => x.CfdiId,
                        principalTable: "Cfdis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CfdiStatusHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CfdiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CfdiStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CfdiStatusHistories_Cfdis_CfdiId",
                        column: x => x.CfdiId,
                        principalTable: "Cfdis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CfdiXmls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CfdiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    XmlContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CfdiXmls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CfdiXmls_Cfdis_CfdiId",
                        column: x => x.CfdiId,
                        principalTable: "Cfdis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CfdiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Recipient = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailLogs_Cfdis_CfdiId",
                        column: x => x.CfdiId,
                        principalTable: "Cfdis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MassDownloadFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MassDownloadFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MassDownloadFiles_MassDownloadRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "MassDownloadRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StampMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    StampBalanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CfdiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StampMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StampMovements_Cfdis_CfdiId",
                        column: x => x.CfdiId,
                        principalTable: "Cfdis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StampMovements_StampBalances_StampBalanceId",
                        column: x => x.StampBalanceId,
                        principalTable: "StampBalances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_UserId",
                table: "ActivityLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthTokens_Token",
                table: "AuthTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthTokens_UserId",
                table: "AuthTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchItems_BatchId",
                table: "BatchItems",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchItems_CfdiId",
                table: "BatchItems",
                column: "CfdiId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_SerialNumber",
                table: "Certificates",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_UserId",
                table: "Certificates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CfdiBatches_UserId",
                table: "CfdiBatches",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CfdiPdfs_CfdiId",
                table: "CfdiPdfs",
                column: "CfdiId");

            migrationBuilder.CreateIndex(
                name: "UX_CfdiPdfs_CfdiId_Version",
                table: "CfdiPdfs",
                columns: new[] { "CfdiId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cfdis_RfcEmisor_RfcReceptor",
                table: "Cfdis",
                columns: new[] { "RfcEmisor", "RfcReceptor" });

            migrationBuilder.CreateIndex(
                name: "IX_Cfdis_UserId",
                table: "Cfdis",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cfdis_Uuid",
                table: "Cfdis",
                column: "Uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CfdiStatusHistories_CfdiId",
                table: "CfdiStatusHistories",
                column: "CfdiId");

            migrationBuilder.CreateIndex(
                name: "IX_CfdiStatusHistories_CfdiId_ChangedAt",
                table: "CfdiStatusHistories",
                columns: new[] { "CfdiId", "ChangedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CfdiXmls_CfdiId",
                table: "CfdiXmls",
                column: "CfdiId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_CfdiId",
                table: "EmailLogs",
                column: "CfdiId");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_LogDate",
                table: "ErrorLogs",
                column: "LogDate");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_MethodName",
                table: "ErrorLogs",
                column: "MethodName");

            migrationBuilder.CreateIndex(
                name: "IX_MassDownloadFiles_RequestId",
                table: "MassDownloadFiles",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_MassDownloadFiles_Uuid",
                table: "MassDownloadFiles",
                column: "Uuid");

            migrationBuilder.CreateIndex(
                name: "IX_MassDownloadRequests_UserId",
                table: "MassDownloadRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SatCatalogItems_SatCatalogId",
                table: "SatCatalogItems",
                column: "SatCatalogId");

            migrationBuilder.CreateIndex(
                name: "UX_SatCatalogItems_Catalog_Key",
                table: "SatCatalogItems",
                columns: new[] { "SatCatalogId", "KeyCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SatCatalogRules_Validation",
                table: "SatCatalogRules",
                columns: new[] { "CatalogCode", "ItemKey", "AppliesToCatalog", "AppliesToKey" });

            migrationBuilder.CreateIndex(
                name: "IX_SatCatalogs_CfdiVersion",
                table: "SatCatalogs",
                column: "CfdiVersion");

            migrationBuilder.CreateIndex(
                name: "IX_SatCatalogs_Code",
                table: "SatCatalogs",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StampBalances_UserId",
                table: "StampBalances",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StampMovements_CfdiId",
                table: "StampMovements",
                column: "CfdiId");

            migrationBuilder.CreateIndex(
                name: "IX_StampMovements_StampBalanceId",
                table: "StampMovements",
                column: "StampBalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "AuthTokens");

            migrationBuilder.DropTable(
                name: "BatchItems");

            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "CfdiPdfs");

            migrationBuilder.DropTable(
                name: "CfdiStatusHistories");

            migrationBuilder.DropTable(
                name: "CfdiXmls");

            migrationBuilder.DropTable(
                name: "EmailLogs");

            migrationBuilder.DropTable(
                name: "ErrorLogs");

            migrationBuilder.DropTable(
                name: "MassDownloadFiles");

            migrationBuilder.DropTable(
                name: "SatCatalogItems");

            migrationBuilder.DropTable(
                name: "SatCatalogRules");

            migrationBuilder.DropTable(
                name: "StampMovements");

            migrationBuilder.DropTable(
                name: "CfdiBatches");

            migrationBuilder.DropTable(
                name: "MassDownloadRequests");

            migrationBuilder.DropTable(
                name: "SatCatalogs");

            migrationBuilder.DropTable(
                name: "Cfdis");

            migrationBuilder.DropTable(
                name: "StampBalances");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

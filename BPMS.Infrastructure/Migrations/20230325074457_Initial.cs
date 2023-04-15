using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BPMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllFlowsWithDelayLogs",
                columns: table => new
                {
                    FlowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlowStaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestStaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlowStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FlowStaff = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestNo = table.Column<long>(type: "bigint", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubProcessName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowDetailTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestStaff = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PredictedTime = table.Column<int>(type: "int", nullable: false),
                    TimeTodo = table.Column<int>(type: "int", nullable: true),
                    DelayHour = table.Column<int>(type: "int", nullable: false),
                    PredictedTimeAllToThisStep = table.Column<int>(type: "int", nullable: false),
                    TimeTodoAllToThisStep = table.Column<int>(type: "int", nullable: false),
                    DelayHourToThisStep = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllFlowsWithDelayLogs", x => x.FlowId);
                });

            migrationBuilder.CreateTable(
                name: "AverageRequestProcessingTimeLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkFlowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvgTimeToDo = table.Column<int>(type: "int", nullable: true),
                    AvgTimeDone = table.Column<int>(type: "int", nullable: true),
                    Min = table.Column<int>(type: "int", nullable: true),
                    Max = table.Column<int>(type: "int", nullable: true),
                    Count = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedTime = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AverageRequestProcessingTimeLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    NationalNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FromDsr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CellPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Dsr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Avtive = table.Column<bool>(type: "bit", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OrganizationPost = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EconomicCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ShortName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    WebSite = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FullAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Dsr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegisterDate = table.Column<long>(type: "bigint", nullable: false),
                    NationalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RegistrationNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RegisterTime = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentsAccessTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormClassificationAccessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsInProcess = table.Column<bool>(type: "bit", nullable: false),
                    IsExpired = table.Column<bool>(type: "bit", nullable: false),
                    CanCreate = table.Column<bool>(type: "bit", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    CanRemove = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentsAccessTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SmtpServerUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PortNumber = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SslRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieverEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentDate = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    EmailText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentStatus = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Exceptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exceptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormClassificationRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MainId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SecondaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormClassificationRelations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormLookUp2N",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title2 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormLookUp2N", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LookUps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Aux = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Aux2 = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookUps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dsr = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleLifeTimeLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleType = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Elapsed = table.Column<TimeSpan>(type: "time", nullable: false),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleLifeTimeLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleType = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTaskLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkFlowTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkFlowVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkFlowDetailTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceTaskObjName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<int>(type: "int", nullable: false),
                    CreatedTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalApiDataJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTaskLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmsLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieverNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentDate = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    SmsText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentStatus = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SmsSendType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmsProviderConfiges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApiKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProviderNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Uri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SmsSendType = table.Column<int>(type: "int", nullable: false),
                    GsmPort = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GsmPortRate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GsmPortReadTimeout = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GsmPortWriteTimeout = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsProviderConfiges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThirdParties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordExpires = table.Column<bool>(type: "bit", nullable: false),
                    ExpireDate = table.Column<int>(type: "int", nullable: true),
                    IPAddresses = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThirdParties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsefulLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsExternalLink = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsefulLinks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowDetailPatterns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDetailPatterns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowFormLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlowFormLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Charts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhpId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Charts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Charts_Charts_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Charts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Charts_LookUps_ChartLevelId",
                        column: x => x.ChartLevelId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExternalApis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseStructute = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Headers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UseInGrid = table.Column<bool>(type: "bit", nullable: false),
                    ContentType = table.Column<int>(type: "int", nullable: false),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    AuthorizationType = table.Column<int>(type: "int", nullable: false),
                    LookUpId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LookUpId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalApis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalApis_LookUps_LookUpId",
                        column: x => x.LookUpId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExternalApis_LookUps_LookUpId1",
                        column: x => x.LookUpId1,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FormClassifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FormTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EditDate = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    StandardTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EditNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AccessTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkFlowLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConfidentialLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RecordEditDate = table.Column<int>(type: "int", nullable: true),
                    RegisterDate = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<int>(type: "int", nullable: true),
                    Dsr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Counter = table.Column<int>(type: "int", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Parent = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsImplementedInBpms = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormClassifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormClassifications_LookUps_AccessTypeId",
                        column: x => x.AccessTypeId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FormClassifications_LookUps_ConfidentialLevelId",
                        column: x => x.ConfidentialLevelId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FormClassifications_LookUps_FormStatusId",
                        column: x => x.FormStatusId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FormClassifications_LookUps_FormTypeId",
                        column: x => x.FormTypeId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FormClassifications_LookUps_StandardTypeId",
                        column: x => x.StandardTypeId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FormClassifications_LookUps_WorkFlowLookupId",
                        column: x => x.WorkFlowLookupId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Holydays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<int>(type: "int", nullable: false),
                    Dsr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HolydayTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holydays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Holydays_LookUps_HolydayTypeId",
                        column: x => x.HolydayTypeId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaskTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<int>(type: "int", nullable: false),
                    EndDate = table.Column<int>(type: "int", nullable: false),
                    RunTime = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    IsDaily = table.Column<bool>(type: "bit", nullable: false),
                    SaturDay = table.Column<bool>(type: "bit", nullable: false),
                    SunDay = table.Column<bool>(type: "bit", nullable: false),
                    MonDay = table.Column<bool>(type: "bit", nullable: false),
                    TuesDay = table.Column<bool>(type: "bit", nullable: false),
                    WednesDay = table.Column<bool>(type: "bit", nullable: false),
                    ThursDay = table.Column<bool>(type: "bit", nullable: false),
                    Friday = table.Column<bool>(type: "bit", nullable: false),
                    IsRunExpireTrigger = table.Column<bool>(type: "bit", nullable: false),
                    DailyInterval = table.Column<byte>(type: "tinyint", nullable: false),
                    HourlyInterval = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    RegisterDate = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_LookUps_TaskTypeId",
                        column: x => x.TaskTypeId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Staffs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonalCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    FName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StaffTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EngTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocalPhone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    BuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InsuranceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmploymentDate = table.Column<int>(type: "int", nullable: true),
                    AgreementEndDate = table.Column<int>(type: "int", nullable: true),
                    Gender = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staffs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Staffs_LookUps_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Staffs_LookUps_EngTypeId",
                        column: x => x.EngTypeId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Staffs_LookUps_StaffTypeId",
                        column: x => x.StaffTypeId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoleActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Controller = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleActions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoleMapPostTitles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostTitleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMapPostTitles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleMapPostTitles_LookUps_PostTitleId",
                        column: x => x.PostTitleId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoleMapPostTitles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoleMapPostTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMapPostTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleMapPostTypes_LookUps_PostTypeId",
                        column: x => x.PostTypeId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoleMapPostTypes_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowDetailPatternItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    LookupOrganizationPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowDetailPatternId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDetailPatternItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowDetailPatternItems_LookUps_LookupOrganizationPostId",
                        column: x => x.LookupOrganizationPostId,
                        principalTable: "LookUps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkflowDetailPatternItems_WorkflowDetailPatterns_WorkflowDetailPatternId",
                        column: x => x.WorkflowDetailPatternId,
                        principalTable: "WorkflowDetailPatterns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleMapCharts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMapCharts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleMapCharts_Charts_ChartId",
                        column: x => x.ChartId,
                        principalTable: "Charts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoleMapCharts_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FormClassificationAccesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormClassificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormClassificationAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormClassificationAccesses_FormClassifications_FormClassificationId",
                        column: x => x.FormClassificationId,
                        principalTable: "FormClassifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assingnments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResponseTypeGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assingnments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assingnments_LookUps_ResponseTypeGroupId",
                        column: x => x.ResponseTypeGroupId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assingnments_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FormClassificationCreators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormClassificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatorTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormClassificationCreators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormClassificationCreators_FormClassifications_FormClassificationId",
                        column: x => x.FormClassificationId,
                        principalTable: "FormClassifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormClassificationCreators_LookUps_CreatorTypeId",
                        column: x => x.CreatorTypeId,
                        principalTable: "LookUps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormClassificationCreators_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganiztionInfoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhpId = table.Column<int>(type: "int", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganiztionPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Priority = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganiztionInfoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganiztionInfoes_Charts_ChartId",
                        column: x => x.ChartId,
                        principalTable: "Charts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrganiztionInfoes_LookUps_OrganiztionPostId",
                        column: x => x.OrganiztionPostId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrganiztionInfoes_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LDAPUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LDAPDomainName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNumber = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TwoStepVerification = table.Column<bool>(type: "bit", nullable: false),
                    TwoStepVerificationByEmail = table.Column<bool>(type: "bit", nullable: false),
                    TwoStepVerificationByGoogleAuthenticator = table.Column<bool>(type: "bit", nullable: false),
                    GoogleAuthKey = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowConfermentAuthorities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDate = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlowConfermentAuthorities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkFlowConfermentAuthorities_LookUps_RequestTypeId",
                        column: x => x.RequestTypeId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkFlowConfermentAuthorities_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowForms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Jquery = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    AdditionalCssStyleCode = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDate = table.Column<int>(type: "int", nullable: false),
                    RegisterTime = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    ModifiedId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifideDate = table.Column<int>(type: "int", nullable: true),
                    ModifideTime = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    DocumentCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrginalVersion = table.Column<int>(type: "int", nullable: false),
                    SecondaryVersion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlowForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkFlowForms_Staffs_ModifiedId",
                        column: x => x.ModifiedId,
                        principalTable: "Staffs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkFlowForms_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Workflows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestGroupTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RemoteId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDate = table.Column<int>(type: "int", nullable: false),
                    RegisterTime = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    ModifiedId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifideDate = table.Column<int>(type: "int", nullable: true),
                    ModifideTime = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    OrginalVersion = table.Column<int>(type: "int", nullable: false),
                    SecondaryVersion = table.Column<int>(type: "int", nullable: false),
                    Dsr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    About = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyWords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    FlowTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Owner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubProcessId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workflows_LookUps_FlowTypeId",
                        column: x => x.FlowTypeId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Workflows_LookUps_RequestGroupTypeId",
                        column: x => x.RequestGroupTypeId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Workflows_LookUps_RequestTypeId",
                        column: x => x.RequestTypeId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Workflows_Staffs_ModifiedId",
                        column: x => x.ModifiedId,
                        principalTable: "Staffs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Workflows_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Workflows_Workflows_SubProcessId",
                        column: x => x.SubProcessId,
                        principalTable: "Workflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoleAccesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleAccesses_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoleAccesses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemSettings_Users_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserLoginOuts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginOutTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    Ip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoginOuts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLoginOuts_LookUps_LoginOutTypeId",
                        column: x => x.LoginOutTypeId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserLoginOuts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettingTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettings_LookUps_SettingTypeId",
                        column: x => x.SettingTypeId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowConfermentAuthorityDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConfermentAuthorityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromDate = table.Column<int>(type: "int", nullable: false),
                    ToDate = table.Column<int>(type: "int", nullable: false),
                    OnlyOwnRequest = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlowConfermentAuthorityDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkFlowConfermentAuthorityDetails_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkFlowConfermentAuthorityDetails_WorkFlowConfermentAuthorities_ConfermentAuthorityId",
                        column: x => x.ConfermentAuthorityId,
                        principalTable: "WorkFlowConfermentAuthorities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Expersion = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    RegisterDate = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PrintFileName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_Staffs_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Staffs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Workflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDate = table.Column<int>(type: "int", nullable: false),
                    RegisterTime = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    RequestNo = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkFlowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationPostTitleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    IgnoreOrgInfChange = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requests_LookUps_OrganizationPostTitleId",
                        column: x => x.OrganizationPostTitleId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Requests_LookUps_RequestStatusId",
                        column: x => x.RequestStatusId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Requests_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Requests_Workflows_WorkFlowId",
                        column: x => x.WorkFlowId,
                        principalTable: "Workflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StartTimerEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSequential = table.Column<bool>(type: "bit", nullable: false),
                    IntervalHours = table.Column<int>(type: "int", nullable: true),
                    HasExpireDate = table.Column<bool>(type: "bit", nullable: false),
                    ExpireDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkFlowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastRunTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StartTimerEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StartTimerEvents_Workflows_WorkFlowId",
                        column: x => x.WorkFlowId,
                        principalTable: "Workflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkFlowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganizationPostTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganizationPostTitleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkFlowFormId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResponseGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CallProcessId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkflowDetailPatternId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Step = table.Column<int>(type: "int", nullable: true),
                    ViewName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExiteMethod = table.Column<int>(type: "int", nullable: true),
                    IsMultiConfirmReject = table.Column<bool>(type: "bit", nullable: false),
                    RequesterAccept = table.Column<bool>(type: "bit", nullable: false),
                    IsOrLogic = table.Column<bool>(type: "bit", nullable: false),
                    NoReject = table.Column<bool>(type: "bit", nullable: false),
                    BusinessAcceptor = table.Column<bool>(type: "bit", nullable: false),
                    SelectAcceptor = table.Column<bool>(type: "bit", nullable: false),
                    SelectFirstPostPattern = table.Column<bool>(type: "bit", nullable: false),
                    SelectAllPostPattern = table.Column<bool>(type: "bit", nullable: false),
                    WaitingTime = table.Column<int>(type: "int", nullable: true),
                    BusinessAcceptorMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScriptTaskMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditableFields = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HiddenFields = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WaithingTImeForAct = table.Column<int>(type: "int", nullable: true),
                    Act = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    PrintFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dsr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAdHoc = table.Column<bool>(type: "bit", nullable: false),
                    AdHocWorkflowDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsServiceTask = table.Column<bool>(type: "bit", nullable: false),
                    ServiceTaskApiResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalApiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsManualTask = table.Column<bool>(type: "bit", nullable: false),
                    IsScriptTask = table.Column<bool>(type: "bit", nullable: false),
                    Info = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasSaveableForm = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlowDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkFlowDetails_ExternalApis_ExternalApiId",
                        column: x => x.ExternalApiId,
                        principalTable: "ExternalApis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkFlowDetails_LookUps_OrganizationPostTitleId",
                        column: x => x.OrganizationPostTitleId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkFlowDetails_LookUps_OrganizationPostTypeId",
                        column: x => x.OrganizationPostTypeId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkFlowDetails_LookUps_ResponseGroupId",
                        column: x => x.ResponseGroupId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkFlowDetails_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkFlowDetails_WorkFlowForms_WorkFlowFormId",
                        column: x => x.WorkFlowFormId,
                        principalTable: "WorkFlowForms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkFlowDetails_WorkflowDetailPatterns_WorkflowDetailPatternId",
                        column: x => x.WorkflowDetailPatternId,
                        principalTable: "WorkflowDetailPatterns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkFlowDetails_Workflows_WorkFlowId",
                        column: x => x.WorkFlowId,
                        principalTable: "Workflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DynamicCharts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WidgetTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DataSetting = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicCharts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DynamicCharts_LookUps_WidgetTypeId",
                        column: x => x.WidgetTypeId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DynamicCharts_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DynamicCharts_Staffs_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Staffs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmployementCertificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestIntention = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dsr = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployementCertificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployementCertificates_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Flows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationPostTitleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlowStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResponseDate = table.Column<int>(type: "int", nullable: true),
                    ResponseTime = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    DelayDate = table.Column<int>(type: "int", nullable: true),
                    DelayTime = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    IsBalloon = table.Column<bool>(type: "bit", nullable: false),
                    Dsr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkFlowDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreviousWorkFlowDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PreviousFlowId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConfermentAuthorityStaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsEnd = table.Column<bool>(type: "bit", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CallActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DynamicWaitingTime = table.Column<int>(type: "int", nullable: true),
                    Value = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Flows_Flows_PreviousFlowId",
                        column: x => x.PreviousFlowId,
                        principalTable: "Flows",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Flows_LookUps_FlowStatusId",
                        column: x => x.FlowStatusId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Flows_LookUps_OrganizationPostTitleId",
                        column: x => x.OrganizationPostTitleId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Flows_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Flows_Staffs_ConfermentAuthorityStaffId",
                        column: x => x.ConfermentAuthorityStaffId,
                        principalTable: "Staffs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Flows_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Flows_WorkFlowDetails_WorkFlowDetailId",
                        column: x => x.WorkFlowDetailId,
                        principalTable: "WorkFlowDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowBoundaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoundaryId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Info = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlowBoundaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkFlowBoundaries_WorkFlowDetails_WorkflowDetailId",
                        column: x => x.WorkflowDetailId,
                        principalTable: "WorkFlowDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowIndicators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WidgetTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlowstatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CalcCriterionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Warning = table.Column<int>(type: "int", nullable: false),
                    Crisis = table.Column<int>(type: "int", nullable: false),
                    RegisterDate = table.Column<int>(type: "int", nullable: false),
                    RegisterTime = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlowIndicators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkFlowIndicators_LookUps_CalcCriterionId",
                        column: x => x.CalcCriterionId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkFlowIndicators_LookUps_DurationId",
                        column: x => x.DurationId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkFlowIndicators_LookUps_FlowstatusId",
                        column: x => x.FlowstatusId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkFlowIndicators_LookUps_RequestTypeId",
                        column: x => x.RequestTypeId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkFlowIndicators_LookUps_WidgetTypeId",
                        column: x => x.WidgetTypeId,
                        principalTable: "LookUps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkFlowIndicators_WorkFlowDetails_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "WorkFlowDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowNextSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromWfdId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToWfdId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BoundaryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Esb = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gateway = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FlowLine = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Method = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Evt = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlowNextSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkFlowNextSteps_WorkFlowDetails_FromWfdId",
                        column: x => x.FromWfdId,
                        principalTable: "WorkFlowDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkFlowNextSteps_WorkFlowDetails_ToWfdId",
                        column: x => x.ToWfdId,
                        principalTable: "WorkFlowDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowEsbs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Info = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowNextStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowEsbs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowEsbs_WorkFlowNextSteps_WorkflowNextStepId",
                        column: x => x.WorkflowNextStepId,
                        principalTable: "WorkFlowNextSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlowEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkFlowEsbId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    GatewayEventBase = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlowEvents_Flows_FlowId",
                        column: x => x.FlowId,
                        principalTable: "Flows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlowEvents_WorkflowEsbs_WorkFlowEsbId",
                        column: x => x.WorkFlowEsbId,
                        principalTable: "WorkflowEsbs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assingnments_ResponseTypeGroupId",
                table: "Assingnments",
                column: "ResponseTypeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Assingnments_StaffId",
                table: "Assingnments",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_AverageRequestProcessingTimeLogs_WorkFlowId",
                table: "AverageRequestProcessingTimeLogs",
                column: "WorkFlowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Charts_ChartLevelId",
                table: "Charts",
                column: "ChartLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Charts_ParentId",
                table: "Charts",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicCharts_CreatorId",
                table: "DynamicCharts",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicCharts_ReportId",
                table: "DynamicCharts",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicCharts_WidgetTypeId",
                table: "DynamicCharts",
                column: "WidgetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployementCertificates_RequestId",
                table: "EmployementCertificates",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalApis_LookUpId",
                table: "ExternalApis",
                column: "LookUpId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalApis_LookUpId1",
                table: "ExternalApis",
                column: "LookUpId1");

            migrationBuilder.CreateIndex(
                name: "IX_FlowEvents_FlowId",
                table: "FlowEvents",
                column: "FlowId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowEvents_WorkFlowEsbId",
                table: "FlowEvents",
                column: "WorkFlowEsbId");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_ConfermentAuthorityStaffId",
                table: "Flows",
                column: "ConfermentAuthorityStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_FlowStatusId",
                table: "Flows",
                column: "FlowStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_OrganizationPostTitleId",
                table: "Flows",
                column: "OrganizationPostTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_PreviousFlowId",
                table: "Flows",
                column: "PreviousFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_RequestId",
                table: "Flows",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_StaffId",
                table: "Flows",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_WorkFlowDetailId",
                table: "Flows",
                column: "WorkFlowDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_FormClassificationAccesses_FormClassificationId",
                table: "FormClassificationAccesses",
                column: "FormClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_FormClassificationCreators_CreatorTypeId",
                table: "FormClassificationCreators",
                column: "CreatorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FormClassificationCreators_FormClassificationId",
                table: "FormClassificationCreators",
                column: "FormClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_FormClassificationCreators_StaffId",
                table: "FormClassificationCreators",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_FormClassifications_AccessTypeId",
                table: "FormClassifications",
                column: "AccessTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FormClassifications_ConfidentialLevelId",
                table: "FormClassifications",
                column: "ConfidentialLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_FormClassifications_FormStatusId",
                table: "FormClassifications",
                column: "FormStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_FormClassifications_FormTypeId",
                table: "FormClassifications",
                column: "FormTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FormClassifications_StandardTypeId",
                table: "FormClassifications",
                column: "StandardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FormClassifications_WorkFlowLookupId",
                table: "FormClassifications",
                column: "WorkFlowLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_Holydays_HolydayTypeId",
                table: "Holydays",
                column: "HolydayTypeId");

            migrationBuilder.CreateIndex(
                name: "XI_Code_Type",
                table: "LookUps",
                columns: new[] { "Code", "Type" },
                unique: true,
                filter: "[Type] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrganiztionInfoes_ChartId",
                table: "OrganiztionInfoes",
                column: "ChartId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganiztionInfoes_OrganiztionPostId",
                table: "OrganiztionInfoes",
                column: "OrganiztionPostId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganiztionInfoes_StaffId",
                table: "OrganiztionInfoes",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_CreatorId",
                table: "Reports",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_WorkflowId",
                table: "Reports",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_OrganizationPostTitleId",
                table: "Requests",
                column: "OrganizationPostTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestStatusId",
                table: "Requests",
                column: "RequestStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_StaffId",
                table: "Requests",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_WorkFlowId",
                table: "Requests",
                column: "WorkFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleAccesses_UserId",
                table: "RoleAccesses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleId_UserId",
                table: "RoleAccesses",
                columns: new[] { "RoleId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Controller_Action_RoleId",
                table: "RoleActions",
                columns: new[] { "Controller", "Action", "RoleId" },
                unique: true,
                filter: "[Controller] IS NOT NULL AND [Action] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RoleActions_RoleId",
                table: "RoleActions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleId_ChartId",
                table: "RoleMapCharts",
                columns: new[] { "RoleId", "ChartId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleMapCharts_ChartId",
                table: "RoleMapCharts",
                column: "ChartId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleId_LookupId",
                table: "RoleMapPostTitles",
                columns: new[] { "RoleId", "PostTitleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleMapPostTitles_PostTitleId",
                table: "RoleMapPostTitles",
                column: "PostTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleId_LookupId",
                table: "RoleMapPostTypes",
                columns: new[] { "RoleId", "PostTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleMapPostTypes_PostTypeId",
                table: "RoleMapPostTypes",
                column: "PostTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_TaskTypeId",
                table: "Schedules",
                column: "TaskTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_BuildingId",
                table: "Staffs",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_EngTypeId",
                table: "Staffs",
                column: "EngTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_PersonalCode",
                table: "Staffs",
                column: "PersonalCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_StaffTypeId",
                table: "Staffs",
                column: "StaffTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StartTimerEvents_WorkFlowId",
                table: "StartTimerEvents",
                column: "WorkFlowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_CreatorUserId",
                table: "SystemSettings",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginOuts_LoginOutTypeId",
                table: "UserLoginOuts",
                column: "LoginOutTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginOuts_UserId",
                table: "UserLoginOuts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StaffId",
                table: "Users",
                column: "StaffId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_SettingTypeId",
                table: "UserSettings",
                column: "SettingTypeId");

            migrationBuilder.CreateIndex(
                name: "XI_User_SettingType",
                table: "UserSettings",
                columns: new[] { "UserId", "SettingTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowBoundaries_WorkflowDetailId",
                table: "WorkFlowBoundaries",
                column: "WorkflowDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowConfermentAuthorities_RequestTypeId",
                table: "WorkFlowConfermentAuthorities",
                column: "RequestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowConfermentAuthorities_StaffId",
                table: "WorkFlowConfermentAuthorities",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowConfermentAuthorityDetails_ConfermentAuthorityId",
                table: "WorkFlowConfermentAuthorityDetails",
                column: "ConfermentAuthorityId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowConfermentAuthorityDetails_StaffId",
                table: "WorkFlowConfermentAuthorityDetails",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDetailPatternItems_LookupOrganizationPostId",
                table: "WorkflowDetailPatternItems",
                column: "LookupOrganizationPostId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDetailPatternItems_WorkflowDetailPatternId",
                table: "WorkflowDetailPatternItems",
                column: "WorkflowDetailPatternId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowDetails_ExternalApiId",
                table: "WorkFlowDetails",
                column: "ExternalApiId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowDetails_OrganizationPostTitleId",
                table: "WorkFlowDetails",
                column: "OrganizationPostTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowDetails_OrganizationPostTypeId",
                table: "WorkFlowDetails",
                column: "OrganizationPostTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowDetails_ResponseGroupId",
                table: "WorkFlowDetails",
                column: "ResponseGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowDetails_StaffId",
                table: "WorkFlowDetails",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowDetails_WorkflowDetailPatternId",
                table: "WorkFlowDetails",
                column: "WorkflowDetailPatternId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowDetails_WorkFlowFormId",
                table: "WorkFlowDetails",
                column: "WorkFlowFormId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowDetails_WorkFlowId",
                table: "WorkFlowDetails",
                column: "WorkFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowEsbs_WorkflowNextStepId",
                table: "WorkflowEsbs",
                column: "WorkflowNextStepId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowForms_ModifiedId",
                table: "WorkFlowForms",
                column: "ModifiedId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowForms_StaffId",
                table: "WorkFlowForms",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowIndicators_ActivityId",
                table: "WorkFlowIndicators",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowIndicators_CalcCriterionId",
                table: "WorkFlowIndicators",
                column: "CalcCriterionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowIndicators_DurationId",
                table: "WorkFlowIndicators",
                column: "DurationId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowIndicators_FlowstatusId",
                table: "WorkFlowIndicators",
                column: "FlowstatusId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowIndicators_RequestTypeId",
                table: "WorkFlowIndicators",
                column: "RequestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowIndicators_WidgetTypeId",
                table: "WorkFlowIndicators",
                column: "WidgetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowNextSteps_FromWfdId",
                table: "WorkFlowNextSteps",
                column: "FromWfdId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowNextSteps_ToWfdId",
                table: "WorkFlowNextSteps",
                column: "ToWfdId");

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_FlowTypeId",
                table: "Workflows",
                column: "FlowTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_ModifiedId",
                table: "Workflows",
                column: "ModifiedId");

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_RequestGroupTypeId",
                table: "Workflows",
                column: "RequestGroupTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_RequestTypeId",
                table: "Workflows",
                column: "RequestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_StaffId",
                table: "Workflows",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_SubProcessId",
                table: "Workflows",
                column: "SubProcessId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllFlowsWithDelayLogs");

            migrationBuilder.DropTable(
                name: "Assingnments");

            migrationBuilder.DropTable(
                name: "AverageRequestProcessingTimeLogs");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "DocumentsAccessTypes");

            migrationBuilder.DropTable(
                name: "DynamicCharts");

            migrationBuilder.DropTable(
                name: "EmailConfigs");

            migrationBuilder.DropTable(
                name: "EmailLogs");

            migrationBuilder.DropTable(
                name: "EmployementCertificates");

            migrationBuilder.DropTable(
                name: "Exceptions");

            migrationBuilder.DropTable(
                name: "FlowEvents");

            migrationBuilder.DropTable(
                name: "FormClassificationAccesses");

            migrationBuilder.DropTable(
                name: "FormClassificationCreators");

            migrationBuilder.DropTable(
                name: "FormClassificationRelations");

            migrationBuilder.DropTable(
                name: "FormLookUp2N");

            migrationBuilder.DropTable(
                name: "Holydays");

            migrationBuilder.DropTable(
                name: "OrganiztionInfoes");

            migrationBuilder.DropTable(
                name: "RoleAccesses");

            migrationBuilder.DropTable(
                name: "RoleActions");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "RoleMapCharts");

            migrationBuilder.DropTable(
                name: "RoleMapPostTitles");

            migrationBuilder.DropTable(
                name: "RoleMapPostTypes");

            migrationBuilder.DropTable(
                name: "ScheduleLifeTimeLogs");

            migrationBuilder.DropTable(
                name: "ScheduleLogs");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "ServiceTaskLogs");

            migrationBuilder.DropTable(
                name: "SmsLogs");

            migrationBuilder.DropTable(
                name: "SmsProviderConfiges");

            migrationBuilder.DropTable(
                name: "StartTimerEvents");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "ThirdParties");

            migrationBuilder.DropTable(
                name: "UsefulLinks");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLoginOuts");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "WorkFlowBoundaries");

            migrationBuilder.DropTable(
                name: "WorkFlowConfermentAuthorityDetails");

            migrationBuilder.DropTable(
                name: "WorkflowDetailPatternItems");

            migrationBuilder.DropTable(
                name: "WorkFlowFormLists");

            migrationBuilder.DropTable(
                name: "WorkFlowIndicators");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Flows");

            migrationBuilder.DropTable(
                name: "WorkflowEsbs");

            migrationBuilder.DropTable(
                name: "FormClassifications");

            migrationBuilder.DropTable(
                name: "Charts");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WorkFlowConfermentAuthorities");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "WorkFlowNextSteps");

            migrationBuilder.DropTable(
                name: "WorkFlowDetails");

            migrationBuilder.DropTable(
                name: "ExternalApis");

            migrationBuilder.DropTable(
                name: "WorkFlowForms");

            migrationBuilder.DropTable(
                name: "WorkflowDetailPatterns");

            migrationBuilder.DropTable(
                name: "Workflows");

            migrationBuilder.DropTable(
                name: "Staffs");

            migrationBuilder.DropTable(
                name: "LookUps");
        }
    }
}

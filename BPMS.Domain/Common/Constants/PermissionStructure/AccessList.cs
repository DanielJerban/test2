using BPMS.Domain.Common.ViewModels;

namespace BPMS.Domain.Common.Constants.PermissionStructure;

public static class AccessList
{

    public static List<AccessListItem> GetAccessList()
    {

        return new List<AccessListItem>
        {
            new AccessListItem
            {
                ParentId =null,
                Id=21,
                Title="Dashboard",
                PersianName="داشبورد مدیریت",
                IsParent=true,
                HasChild = true,
                Icon = "icon-dashbord",
                Datatest = "63548eaf-38a7-44e6-8b00-b5dcc03c13b2"


            },
            new AccessListItem
            {
                ParentId =21,
                Id=4002,
                Title="Main",
                PersianName="داشبورد من",
                ClaimGuid=DashboardPermission.Main.Manage,
                IsParent=false,
                HasChild = true,
                ParentTitle="Dashboard",
                Icon = "",
                Datatest = "e84b82bf-cbb7-4fa8-9cf5-d0e009a6caf5"
            },
            new AccessListItem
            {
                ParentId =21,
                Id=4001,
                Title="EditDashboard",
                PersianName="تنظیمات داشبورد",
                ClaimGuid=DashboardPermission.EditDashboard.Manage,
                IsParent=false,
                HasChild = true,
                ParentTitle="Dashboard",
                Icon = "",
                Datatest = ""
            },
            new AccessListItem
            {
                ParentId =null,
                Id = 20,
                Title="CartbotIndex",
                PersianName="کارتابل فرآیندها",
                ClaimGuid= CartbotPermission.CartbotIndex.Manage,
                HasChild=false,
                IsParent=false,
                ParentTitle="Cartbot",
                Icon = "icon-kartabl",
                Datatest = "2ea6c1b2-05f8-4fc2-9ca8-50b11de928e6"

            },
            new AccessListItem
            {
                ParentId =null,
                Id = 22,
                Title="CartbotAssignment",
                PersianName="تفویض اختیار کارتابل",
                ClaimGuid= CartbotPermission.CartbotAssignment.Manage,
                HasChild=false,
                IsParent=false,
                ParentTitle="Cartbot",
                Icon = "icon-tafviz",
                Datatest = "38f42b14-2ad3-46de-b001-d12f7119c22d"
            },
            new AccessListItem
            {
                ParentId = null,
                Id = 3,
                Title = "Cartbot",
                PersianName = "فرآیندها",
                IsParent = true,
                HasChild = true,
                Icon = "icon-farayandha",
                Datatest = "1691abc0-aad4-4661-a932-85a1e81c946a"

            },
            new AccessListItem
            {
                ParentId = 3,
                Id = 313100,
                Title = "CreateDiagram",
                PersianName = "ایجاد فرایند ",
                ClaimGuid = CartbotPermission.CreateDiagram.Manage,
                HasChild = false,
                ConsiderInMenue = false,
                IsParent = false,
                ParentTitle = "Cartbot",
                Icon = "",
                Datatest = "e858f63b-f816-4bef-9b93-310e95adbc60"

            },
            new AccessListItem
            {
                ParentId = 3,
                Id =  313200,
                Title = "PreviewDiagram",
                PersianName = "مدیریت فرآیندها",
                ClaimGuid = CartbotPermission.PreviewDiagram.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "Cartbot",
                Icon = "",
                Datatest = "99fca125-31a9-4983-9201-2f57b8937019"

            },
            new AccessListItem
            {
                ParentId = 3,
                Id=8012,
                Title="RequestManagement",
                PersianName="مدیریت درخواست ها",
                ClaimGuid=ReportPermission.RequestManagement.Manage,
                IsParent=false,
                HasChild = false,
                ParentTitle="Report",
                Icon = "",
                Datatest = "6acd3336-f385-4a93-be85-3cdc0d087587"

            },
            new AccessListItem
            {
                ParentId = 3,
                Id = 3400,
                Title = "ChangeFlow",
                PersianName = "تغییر انجام دهنده کار",
                ClaimGuid = CartbotPermission.ChangeFlow.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "Cartbot",
                Icon = "",
                Datatest = "b4105c21-1404-4990-b170-bc6968a3b30f"
            },
            new AccessListItem
            {
                ParentId = 3,
                Id = 3500,
                Title = "ChangeRequest",
                PersianName = "تغییر درخواست دهنده کار",
                ClaimGuid = CartbotPermission.ChangeRequest.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "Cartbot",
                Icon = "",
                Datatest = "de86f1b6-d108-4327-b1a5-94a89720dd06"

            },
            new AccessListItem
            {
                ParentId = 3,
                Id = 3600,
                Title = "ChangeSubUsersRequest",
                PersianName = "تغییر درخواست دهنده کار پرسنل زیر مجموعه",
                ClaimGuid = CartbotPermission.ChangeSubUsersRequest.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "Cartbot",
                Icon = "",
                Datatest = "87d0348d-93fa-4b36-82f3-fbcf30cff254"

            },

            new AccessListItem
            {
                ParentId = 3,
                Id = 3700,
                Title = "BaseInfo",
                PersianName = "اطلاعات پایه فرایند",
                ClaimGuid = CartbotPermission.BaseInfo.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "Cartbot",
                Icon = "",
                Datatest = "e317f8a8-8bc0-4177-abb4-1dc35d8f404b"

            },
            new AccessListItem
            {
                ParentId = 3700,
                Id = 313300,
                Title = "CreateResposeGroup",
                PersianName = "تعریف گروه پاسخ دهنده",
                ClaimGuid =CartbotPermission.CreateResposeGroup.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "Cartbot",
                Icon = "",
                Datatest = "48d4e819-f26b-444f-a3fc-ea7ea58ce85a"

            },
            new AccessListItem
            {
                ParentId = 3700,
                Id =  3800,
                Title = "RecieverPattern",
                PersianName = "تعریف الگوی پاسخ دهنده ها",
                ClaimGuid = CartbotPermission.RecieverPattern.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "Cartbot",
                Icon = "",
                Datatest = "44fsa125-m6a9-4973-5201-2s57b4937016"

            },
            new AccessListItem
            {
                ParentId = 3700,
                Id = 6005,
                Title = "ExternalApiIndex",
                PersianName = "تعریف Api",
                ClaimGuid = LookUpsPermission.ExternalApiIndex.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "LookUps",
                Icon = "",
                Datatest = "c5df8477-ba22-421f-a15e-b33c28a4e82e"

            },
            new AccessListItem
            {
                ParentId = 3700,
                Id = 313500,
                Title = "CreateProccessGroup",
                PersianName = "گروه فرآیند ",
                ClaimGuid =LookUpsPermission.ProcessGroup.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "LookUps",
                Icon = "",
                Datatest = "18d4e844-f26b-404f-s3fc-ss7ea58ce85a"

            },
            new AccessListItem
            {
                ParentId = null,
                Id = 12,
                Title = "WorkFlowTutorial",
                PersianName = "راهنمای فرآیند ها",
                ClaimGuid = WorkFlowPermission.WorkFlowTutorial.Manage,
                IsParent = true,
                HasChild = true,
                ParentTitle = "WorkFlow",
                Icon = "icon-rahnama",
                Datatest = "f63221bf-43ac-41a8-a65f-b2f02198c174",
            },
            new AccessListItem
            {
                ParentId = null,
                Id = 4,
                Title = "Cartbot",
                PersianName = "فرم ها",
                IsParent = true,
                HasChild = true,
                Icon = "icon-form",
                Datatest = "0c297d9c-6dcd-45e5-bbb9-f7c9c07509fd"

            },
            new AccessListItem
            {
                ParentId = 4,
                Id =3232100,
                Title = "CreateForm",
                PersianName = "ایجاد فرم ",
                ClaimGuid = CartbotPermission.CreateForm.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "Cartbot",
                ConsiderInMenue = false,
                Icon = "",
                Datatest = "d80732f2-4a75-436a-9cc7-174ed9fe8211"

            },
            new AccessListItem
            {
                ParentId = 4,
                Id =3232200,
                Title = "PreviewForms",
                PersianName = "مدیریت فرم ها",
                ClaimGuid =CartbotPermission.PreviewForms.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "Cartbot",
                Icon = "",
                Datatest = "9803f627-b6fc-40e7-9589-c2acb60b9647"
            },

            new AccessListItem
            {
                ParentId = 4,
                Id = 3232300,
                Title = "BaseInfo",
                PersianName = "اطلاعات پایه فرم",
                ClaimGuid =CartbotPermission.PreviewForms.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "",
                Icon = "",
                Datatest = "f609b200-51e2-4621-b279-c3b042fa8971"
            },

            new AccessListItem
            {
                ParentId = 3232300,
                Id =3232400,
                Title = "CreateList",
                PersianName = "ایجاد لیست",
                ClaimGuid =CartbotPermission.CreateList.Manage,
                ConsiderInMenue = false,
                HasChild = false,
                IsParent = false,
                ParentTitle = "Cartbot",
                Icon = "",
                Datatest = "353136fa-74c7-4b00-8ce9-c57496d23ec7"

            },
            new AccessListItem
            {
                ParentId = 3232300,
                Id = 3232500,
                Title = "PreviewLists",
                PersianName = "مدیریت لیست ها",
                ClaimGuid =CartbotPermission.PreviewLists.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "Cartbot",
                Icon = "",
                Datatest = "0546d39e-7700-42b5-a9e6-1cbf40743f7c"
            },
            new AccessListItem
            {
                ParentId =3232300,
                Id =3232600,
                Title = "CreateLookUp2N",
                PersianName = "مدیریت انتخاب گر دو مرحله ای",
                ClaimGuid = CartbotPermission.CreateLookUp2N.Manage,
                HasChild = false,
                IsParent = false,

                ParentTitle = "Cartbot",
                Icon = "",
                Datatest = "0d4d8ada-b3dd-4065-b1c3-8e6e799d4465"
            },
                
            //new AccessListItem
            //               {
            //                   ParentId =3232300,
            //                   Id = 3232700,
            //                   Title = "ShowLookUp2N",
            //                   PersianName = "مدیریت انتخاب گر دو مرحله ای",
            //                   ClaimGuid = CartbotPermission.ShowLookUp2N.Manage,
            //                   HasChild = false,
            //                   IsParent = false,
            //                   ConsiderInMenue = false,
            //                   ParentTitle = "Cartbot",
            //                   Icon = "",
            //                   Datatest = "5161e8fe-5119-4a61-a892-6ff8cea5d306"

            //               },
            new AccessListItem
            {
                ParentId =3232300,
                Id =3232800,
                Title = "CreateTable",
                PersianName = "ایجاد انتخاب گر ",
                ClaimGuid = CartbotPermission.CreateTable.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "Cartbot",
                Icon = "",
                Datatest = "91d04dc2-6d6d-42f3-970b-71759c4c4b8a"

            },
            new AccessListItem
            {
                ParentId =3232300,
                Id = 3232900,
                Title = "FormsBaseInfo",
                PersianName = "اصلاح انتخابگر",
                ClaimGuid = CartbotPermission.FormsBaseInfo.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "Cartbot",
                Icon = "",
                Datatest = "34a77531-bc54-4798-a4cf-44fb05f1a51e"
            },
            new AccessListItem
            {
                ParentId = 3232300,
                Id = 6002,
                Title = "CreateClient",
                PersianName = " ثبت افراد خارج سازمان",
                ClaimGuid = LookUpsPermission.CreateClient.Manage,
                IsParent = false,
                ParentTitle = "LookUps",
                Icon = "",
                Datatest = "733ffed1-d63a-419a-9c6f-3e3c6f08c900"
            },
            new AccessListItem
            {
                ParentId = 3232300,
                Id = 6003,
                Title = "CreateCompany",
                PersianName = " ثبت شرکت",
                ClaimGuid = LookUpsPermission.CreateCompany.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "LookUps",
                Icon = "",
                Datatest = "7cc12ffb-8578-49c9-b410-fdd6d76654f6"
            },
            new AccessListItem
            {
                ParentId =3232300,
                Id = 3232800,
                Title = "UploadFile",
                PersianName = "بارگذاری فایل",
                ClaimGuid = CartbotPermission.UploadFile.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "Cartbot",
                Icon = "",
                Datatest = "7eeb65cc-be1d-4769-88fb-86916bd987c6"
            },
            new AccessListItem
            {
                ParentId = null,
                Id=8,
                Title="Report",
                PersianName="گزارشات",
                IsParent=true,
                HasChild = true,
                Icon = "icon-gozareshat",
                Datatest = "e6ac588c-4590-4ad8-af02-85d40e2f783c"

            },
            new AccessListItem
            {
                ParentId = 8,
                Id=8013,
                Title="ProcessStatus",
                PersianName="گزارش وضعیت درخواست ها",
                ClaimGuid=ReportPermission.ProcessStatus.Manage,
                IsParent=false,
                HasChild = false,
                ParentTitle="Report",
                Icon = "",
                Datatest = "3692d646-7279-4576-84c7-5aba25d18dce"
            },
            new AccessListItem
            {
                ParentId = 8,
                Id=8007,
                Title="AllNoActionRequestReports",
                PersianName="گزارش درخواست های اقدام نشده",
                ClaimGuid=ReportPermission.AllNoActionRequestReports.Manage,
                IsParent=false,
                HasChild = false,
                ParentTitle="Report",
                Icon = "",
                Datatest = "ac1b69a2-bd39-47b3-a3eb-97e2da54e7e1"
            },
            new AccessListItem
            {
                ParentId = 8,
                Id=8008,
                Title="AllRequestReports",
                PersianName="گزارش تمامی درخواست ها",
                ClaimGuid=ReportPermission.AllRequestReports.Manage,
                IsParent=false,
                HasChild = false,
                ParentTitle="Report",
                Icon = "",
                Datatest = "73b71f04-ced8-4fc2-b6f6-00fd1af0fa80"
            },
            new AccessListItem
            {
                ParentId = 8,
                Id=8010,
                Title="AllRequestByUserReport",
                PersianName="گزارش تمامی درخواست ها به تفکیک پرسنل زیر مجموعه",
                ClaimGuid=ReportPermission.AllRequestByUserReport.Manage,
                IsParent=false,
                HasChild = false,
                ParentTitle="Report",
                Icon = "",
                Datatest = "3b56a6a0-dc5a-4ee0-bf59-ff0944a14493"
            },
            new AccessListItem
            {
                ParentId = 8,
                Id=8011,
                Title="AllRequestByUserDelayReport",
                PersianName="گزارش تمامی درخواست های تاخیر دار به تفکیک پرسنل زیر مجموعه",
                ClaimGuid=ReportPermission.AllRequestByUserDelayReport.Manage,
                IsParent=false,
                HasChild = false,
                ParentTitle="Report",
                Icon="",
                Datatest = "1dd80997-1dc3-4f70-a1d8-6ec9a906db48"

            },
            new AccessListItem
            {
                ParentId = 8,
                Id=8009,
                Title="AllHolooRequestReports",
                PersianName="گزارش درخواست های شرکت هلو",
                ClaimGuid= ReportPermission.AllHolooRequestReports.Manage,
                IsParent=false,
                HasChild = false,
                ParentTitle="Report",
                Icon="",
                Datatest = "61b2b099-188c-4120-b7f7-a6d5b2681f92"
            },
            new AccessListItem
            {
                ParentId = 8,
                Id=8014,
                Title="GeneralProcessStatus",
                PersianName="گزارش میانگین زمان انجام درخواست ها",
                ClaimGuid=ReportPermission.GeneralProcessStatus.Manage,
                IsParent=false,
                HasChild = false,
                ParentTitle="Report",
                Icon = "",
                Datatest = "137f4482-d422-495a-befc-71c437117cf4"
            },
            new AccessListItem
            {
                ParentId = 8,
                Id=8004,
                Title="DynamicReport",
                PersianName="گزارش های ایجاد شده",
                ClaimGuid= ReportPermission.DynamicReport.Manage,
                IsParent=false,
                HasChild = false,
                ParentTitle="Report",
                Icon = "",
                Datatest = "7a64bbdb-860b-4062-8dfd-5e5c66c00926"
            },
            new AccessListItem
            {
                ParentId = 8,
                Id=8002,
                Title="WorkflowFormList",
                PersianName="گزارش لیست ها",
                ClaimGuid= ReportPermission.WorkflowFormList.Manage,
                IsParent=false,
                HasChild = false,
                ParentTitle="Report",
                Icon = "",
                Datatest = "a92be904-1a9a-445a-8e2e-7ec27fe8454f"

            },
            new AccessListItem
            {
                ParentId = null,
                Id=13,
                Title="Report",
                PersianName="گزارش ساز",
                IsParent=true,
                HasChild = true,
                Icon = "icon-gozareshsaz",
                Datatest = "CA05CC5B-521F-4C82-BBD3-785EDFE66457"

            },
            new AccessListItem
            {
                ParentId = 13,
                Id=8003,
                Title="ReportGenerator",
                PersianName="ایجاد گزارش جدید",
                ClaimGuid= ReportPermission.ReportGenerator.Manage,
                IsParent=false,
                HasChild = false,
                ParentTitle="Report",
                Icon = "",
                Datatest = "82a7147f-3276-4204-9f60-471dc06331d4"

            },
            new AccessListItem
            {
                ParentId = 13,
                Id=8005,
                Title="DynamicChart",
                PersianName="ایجاد نمودار گزارش",
                ClaimGuid=ReportPermission.DynamicChart.Manage,
                IsParent=false,
                HasChild = false,
                ConsiderInMenue=false,
                ParentTitle="Report",
                Icon = "",
                Datatest = "57a21236-5ea6-4475-a492-e8722ed6a68f"

            },
            new AccessListItem
            {
                ParentId = 13,
                Id=8006,
                Title="DynamicChartPrevious",
                PersianName="مدیریت نمودارهای گزارش",
                ClaimGuid=ReportPermission.DynamicChartPrevious.Manage,
                IsParent=false,
                HasChild = false,
                ParentTitle="Report",
                Icon = "",
                Datatest = "d74b2840-dfb3-40d7-a8ce-861c8370fa98"

            },
            new AccessListItem
            {
                ParentId = 13,
                Id = 3300,
                Title = "CreateWorkFlowIndicator",
                PersianName = "تعریف شاخص",
                ClaimGuid = CartbotPermission.CreateWorkFlowIndicator.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "Cartbot",
                Icon = "",
                Datatest = "2ef92a8c-6db5-4ba4-ba91-c3ab762891f6"

            },
            new AccessListItem
            {
                ParentId = null,
                Id = 5,
                Title = "FormClassification",
                PersianName = "مدارک و مستندات",
                IsParent = true,
                HasChild = true,
                Icon="icon-madarek",
                Datatest = "3a99d1f9-f426-479c-9139-117870565b2f"

            },
            new AccessListItem
            {
                ParentId = 5,
                Id = 5002,
                Title = "Create",
                PersianName = "ثبت مدارک",
                ClaimGuid = FormClassificationPermission.Create.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "FormClassification",
                Icon="",
                Datatest = "3f31dda9-c3aa-4be1-95a2-b38fdbfc4eaf"


            },
            new AccessListItem
            {
                ParentId = 5,
                Id = 5004,
                Title = "DocumentsList",
                PersianName = "لیست مدارک",
                ClaimGuid =FormClassificationPermission.Report.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "FormClassification",
                Icon = "",
                Datatest = "0273dd5c-8dfe-4893-8dc3-7559a2eea0e8"

            },
            new AccessListItem
            {
                ParentId = 5,
                Id = 5005,
                Title = "DocumentsAccess",
                PersianName = "مدیریت دسترسی اسناد",
                ClaimGuid =FormClassificationPermission.Access.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "FormClassification",
                Icon = "",
                Datatest = "0273dd5c-8dfe-4893-8dc3-7559a2eea0e8"

            },
            new AccessListItem
            {
                ParentId = 5,
                Id = 5001,
                Title = "BaseInfo",
                PersianName = "اطلاعات پایه مدارک",
                ClaimGuid = FormClassificationPermission.BaseInfo.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "FormClassification",
                Icon = "",
                Datatest = "78cbeae0-1a72-4278-9962-be13a8b0b0aa"
            },
            new AccessListItem
            {
                ParentId = null,
                Id = 10,
                Title = "Staffs",
                PersianName = "مدیریت کاربران",
                IsParent = true,
                HasChild = true,
                Icon = "icon-karbaran",
                Datatest = "2c9c7005-0880-4b2c-b884-abf978009d9c"

            },
            new AccessListItem
            {
                ParentId = 10,
                Id = 10001,
                Title = "CreateNewStaff",
                PersianName = "ایجاد کاربر",
                ClaimGuid =  StaffsPermission.CreateNewStaff.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "Staffs",
                ConsiderInMenue = false,
                Icon = "",
                Datatest = "4f89837d-3379-429a-8044-4d4952de10d7"

            },
            new AccessListItem
            {
                ParentId = 10,
                Id = 10002,
                Title = "PersonelListEdit",
                PersianName = "مدیریت کاربر",
                ClaimGuid = StaffsPermission.PersonelListEdit.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "Staffs",
                Icon = "",
                Datatest = "eff6146a-3ed4-4186-8384-e79164c78085"

            },
            new AccessListItem
            {
                ParentId = 10,
                Id = 10006,
                Title = "CreateChart",
                PersianName = "ایجاد چارت سازمانی",
                ClaimGuid = StaffsPermission.CreateChart.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "Staffs",
                Icon = "",
                Datatest = "54ae0519-b5ab-4e7c-8230-49d72aa2cb09"

            },
            new AccessListItem
            {
                ParentId = 10,
                Id = 10009,
                Title = "ChartDiagram",
                PersianName = "نمودار چارت سازمانی",
                ClaimGuid = StaffsPermission.ChartDiagram.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "Staffs",
                Icon = "",
                Datatest = "47e1fe72-968b-4bb8-97f5-be8bcca0e828"

            },
            new AccessListItem
            {
                ParentId = 10,
                Id = 10008,
                Title = "CreatePostsData",
                PersianName = "ایجاد پست سازمانی",
                ClaimGuid = StaffsPermission.CreatePostsData.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "Staffs",
                Icon = "",
                Datatest = "3b75a698-e692-44c0-80fa-4739acd061e9"

            },
            new AccessListItem
            {
                ParentId = 10,
                Id=10011,
                Title="StaffList",
                PersianName="تخصیص پست سازمانی",
                ClaimGuid=StaffsPermission.StaffList.Manage,
                IsParent=false,
                HasChild = false,
                ParentTitle="Staffs",
                Icon = "",
                Datatest = "62566802-2c02-460a-a518-b8831a90d1c1"

            },
            new AccessListItem
            {
                ParentId = 10,
                Id = 10005,
                Title = "PersonelListPerChart",
                PersianName = "نمایش کاربران به تفکیک چارت سازمانی",
                ClaimGuid = StaffsPermission.PersonelListPerChart.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "Staffs",
                Icon = "",
                Datatest = "cd5b0c3d-4de7-48b7-afb6-e1ad1a3f6f3f"

            },
            new AccessListItem
            {
                ParentId = 10,
                Id = 10010,
                Title = "Holyday",
                PersianName = "تعطیلات و ساعت کاری",
                ClaimGuid = StaffsPermission.Holyday.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "Staffs",
                Icon = "",
                Datatest = "84127426-401a-4d83-b5b0-9cfebd6da9db"
            },

            new AccessListItem
            {
                ParentId = 10,
                Id = 12001,
                Title = "StaffAndUsersReport",
                PersianName = "گزارش کاربران سیستم",
                ClaimGuid = UsersPermission.StaffAndUsersReport.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "Users",
                Icon = "",
                Datatest = "b4c4f873-fe56-4ec1-82b7-cc8ab44bff67"

            },
            new AccessListItem
            {
                ParentId = 10,
                Id = 10007,
                Title = "BaseInfo",
                PersianName = "اطلاعات پایه کاربران",
                ClaimGuid =StaffsPermission.BaseInfo.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "Staffs",
                Icon = "",
                Datatest = "0d47d9ae-a3a5-4f70-a469-c429db5106ba"


            },
            new AccessListItem
            {
                ParentId = null,
                Id = 6,
                Title = "LookUps",
                PersianName = "امور سیستم",
                IsParent = true,
                HasChild = true,
                Icon = "icon-system",
                Datatest = "4bede8b7-095c-42ba-8ad0-25b82700b7ab"

            },
            new AccessListItem
            {
                ParentId = 6,
                Id = 6006,
                Title = "ThirdParty",
                PersianName = "اتصال سیستم های ثانویه",
                ClaimGuid = LookUpsPermission.ThirdParty.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "LookUps",
                Icon = "",
                Datatest = "5ad75c1f-36b4-4589-a354-2ac0aed46606"

            },
            new AccessListItem
            {
                ParentId = 6,
                Id = 6001,
                Title = "CreateType",
                PersianName = "ساخت جدول مجازی",
                ClaimGuid = LookUpsPermission.CreateType.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "LookUps",
                Icon = "",
                Datatest = "24d66426-3502-4174-b214-251c45f3b2b8"
            },
            new AccessListItem
            {
                ParentId = 6,
                Id = 6004,
                Title = "BaseInfo",
                PersianName = "اطلاعات پایه امور سیستم",
                ClaimGuid =  LookUpsPermission.BaseInfo.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "LookUps",
                Icon = "",
                Datatest = "5ad75c1f-36b4-4589-a354-2ac0aed46606"
            },
            new AccessListItem
            {
                ParentId = null,
                Id = 1,
                Title = "Role",
                PersianName = "  دسترسی ها",
                IsParent = true,
                HasChild = true,
                Icon = "icon-Shield-Done fa-fw",
                Datatest = "19d19909-eae9-4a62-97b0-318f6795a097"
            },
            new AccessListItem
            {
                ParentId = 1,
                Id = 1001,
                Title = "CreateRole",
                PersianName = "مدیریت گروه",
                ClaimGuid =  RolePermission.CreateRole.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "Role",
                Icon = "",
                Datatest = "421fae86-d538-408d-8353-69247cc20cfd"
            },
            new AccessListItem
            {
                ParentId = 1,
                Id = 1002,
                Title = "CreateAccess2",
                PersianName = "تخصیص دسترسی",
                ClaimGuid =RolePermission.CreateAccess2.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "Role",
                Icon = "",
                Datatest = "b761d6c6-9360-4e1f-adde-8455f2bb26b2"
            },
            new AccessListItem
            {
                ParentId = 1,
                Id = 1003,
                Title = "AccessesReport",
                PersianName = " گزارش دسترسی ها",
                ClaimGuid = RolePermission.AccessesReport.Manage,
                HasChild = false,
                IsParent = false,
                ParentTitle = "Role",
                Icon = "",
                Datatest = "a7c77797-f07e-495b-96a2-78de941c8282"

            },
            new AccessListItem
            {
                ParentId = null,
                Id = 9,
                Title = "Schedule",
                PersianName = "زمان بندی",
                IsParent = true,
                HasChild = true,
                Icon = "ti-timer fa-fw",
                Datatest = "a3275aef-7627-4505-9d7c-910a28f47f18"
            },
            new AccessListItem
            {
                ParentId = 9,
                Id = 9001,
                Title = "CreateSchedule",
                PersianName = "تعریف زمان بندی",
                ClaimGuid = SchedulePermission.CreateSchedule.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "Schedule",
                Icon = "",
                Datatest = "60cfbba0-76c4-458b-9941-26f0f0ac2177"

            },

            new AccessListItem
            {
                ParentId = null,
                Id = 11,
                Title = "SystemLogs",
                PersianName = "لاگ های سیستمی",
                IsParent = true,
                HasChild = true,
                Icon = "icon-log",
                Datatest = "3f4d6e83-157e-4372-a471-b91b775b4f4d",

            },
            new AccessListItem
            {
                ParentId = 11,
                Id = 11001,
                Title = "GetErrorLogs",
                PersianName = "لیست خطاهای سیستمی",
                ClaimGuid = SystemLogsPermission.GetErrorLogs.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "SystemLogs",
                Icon = "",
                Datatest = "60cfbba0-76c4-458b-9941-26f0f0ac2177"

            },
            new AccessListItem
            {
                ParentId = 11,
                Id = 11011,
                Title = "UserLoginOuts",
                PersianName = "گزارش ورود و خروج کاربران",
                ClaimGuid = UsersPermission.UserLoginOuts.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "Users",
                Icon = "",
                Datatest = "4461faf1-dd92-4d07-a353-0382f686456a",

            },
            new AccessListItem
            {
                ParentId = 11,
                Id = 11002,
                Title = "GetJiraLogs",
                PersianName = "لیست لاگ های جیرا",
                ClaimGuid = SystemLogsPermission.GetJiraLogs.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "SystemLogs",
                Icon = "",
                Datatest = "ea7655eb-2cb2-4531-bcbc-32ce399069e9"

            },
            new AccessListItem
            {
                ParentId = 11,
                Id = 7005,
                Title = "GetSmsLogs",
                PersianName = "لیست گزارشات ارسال پیامک",
                ClaimGuid = SystemLogsPermission.GetSmsLogs.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "SystemLogs",
                Icon = "",
                Datatest = "5F46DB53-5F25-481C-A4A7-27307C4CEDFF",
            },
            new AccessListItem
            {
                ParentId = 11,
                Id = 7004,
                Title = "GetEmailLogs",
                PersianName = "لیست گزارشات ارسال ایمیل",
                ClaimGuid = SystemLogsPermission.GetEmailLogs.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "SystemLogs",
                Icon = "",
                Datatest = "97803CC1-9BB3-49B6-96FA-97BF9C074ECD"
            },
            new AccessListItem
            {
                ParentId = 11,
                Id = 7008,
                Title = "GetScheduleLogs",
                PersianName = "گزارش زمانبندها",
                ClaimGuid = SystemLogsPermission.GetAllScheduleLogs.Manage,
                IsParent = false,
                HasChild = false,
                ParentTitle = "SystemLogs",
                Icon = "",
                Datatest = "CC8F0574-F969-4B12-8E62-A6CB172341E0"
            },
            new AccessListItem
            {
                ParentId =null,
                Id = 200,
                Title="MyUserLoginOuts",
                PersianName="گزارش ورود و خروج من",
                ClaimGuid= UsersPermission.MyUserLoginOuts.Manage,
                HasChild=false,
                IsParent=false,
                ParentTitle="Users",
                Icon = "icon-tafviz",
                Datatest = "2ea6c1b2-05g9-4fh2-9dr8-50b11de928f6"

            },
            new AccessListItem
            {
                ParentId = null,
                Id = 23,
                Title = "ManageUsefulLinks",
                PersianName = "مدیریت لینک های پر کاربرد",
                ClaimGuid =ManageUsefulLinksPermission.ManageUsefulLinks.Manage,
                IsParent = true,
                HasChild = false,
                Icon = "icon-link",
                Datatest = "f2b5193c-e49a-49bb-835c-25bf72bbaf85",
                ParentTitle = "ManageUsefulLinks"
            },
            new AccessListItem
            {
                ParentId = null,
                Id = 22,
                Title = "ManageSetting",
                PersianName = "تنظیمات",
                ClaimGuid =SettingPermission.ManageSetting.Setting,
                IsParent = true,
                HasChild = false,
                Icon = "icon-tanzimat",
                Datatest = "2B96E3D9-03C4-4042-9EB6-7B26C1750F28",
                ParentTitle = "Setting"
            },
            new AccessListItem
            {
                ParentId = null,
                Id = 24,
                Title = "QueekLogin",
                PersianName = "سامانه پشتیبانی",
                ClaimGuid =SupportPermission.LoginPermissin.SupportLogin,
                IsParent = true,
                HasChild = false,
                Icon = "fa fa-phone-square supportLink",
                Datatest = "4A15E624-4A94-497D-940B-7A032588D94F",
                ParentTitle = "Support"
            },
        };
    }
}
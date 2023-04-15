using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using BPMS.Domain.Common.Helpers;

namespace BPMS.Application.Repositories;

public class FormClassificationRepository : Repository<FormClassification>, IFormClassificationRepository
{
    public FormClassificationRepository(BpmsDbContext context) : base(context)
    {
    }
    public void SaveFormFiles(string id, Dictionary<string, MemoryStream> documents, string webRootPath)
    {
        foreach (var dictItem in documents)
        {
            var fileName = dictItem.Key;
            var item = dictItem.Value;

            if (item.Length > 20 * 1024 * 1024)
                throw new ArgumentException("حجم فایل آپلود شده نمیتوان بیش از 20 مگابایت باشد.");

            var extention = Path.GetExtension(fileName)?.Replace(".", "").Trim();
            var targetfolder = Path.Combine(webRootPath, "Images/upload/formClassification/Document/" + id.ToLower());
            if (Directory.Exists(targetfolder) == false)
                Directory.CreateDirectory(targetfolder);
            var path = Path.Combine(targetfolder, fileName);

            using (var fileStream = File.Open(path, FileMode.CreateNew))
            {
                item.Seek(0, SeekOrigin.Begin);
                item.CopyTo(fileStream);
                fileStream.Flush();
            }
        }
    }
    public void UpdateFormClassificationRelations(Guid currentFormClassificationId, List<Guid> otherFormClassificationIds)
    {
        var formClassificationRelations = Context.FormClassificationRelations.Where(c => c.MainId == currentFormClassificationId).ToList();
        Context.FormClassificationRelations.RemoveRange(formClassificationRelations);

        var formClassification = Context.FormClassifications.Single(c => c.Id == currentFormClassificationId);
        if (otherFormClassificationIds != null)
        {
            foreach (var id in otherFormClassificationIds)
            {
                Context.FormClassificationRelations.Add(new FormClassificationRelation()
                {
                    MainId = formClassification.Id,
                    SecondaryId = id
                });
            }
        }

        Context.SaveChanges();
    }
    public void Create(FormClassificationViewModel formClassification, Dictionary<string, MemoryStream> documents, string username, string webRootPath)
    {
        var user = Context.Users.Single(c => c.UserName == username);
        var creatorId = user.Id;

        formClassification.Id = Guid.NewGuid();
        if (FormExists(formClassification.FormNo, formClassification.FormTypeId))
            throw new ArgumentException(" سندی با این شناسه قبلا ثبت شده است.");

        if (formClassification.Creators == null)
            throw new ArgumentException("شما تهیه کننده ای انتخاب نکردید.");
        if (formClassification.Verifiers == null)
            throw new ArgumentException("شما تایید کننده ای انتخاب نکردید.");
        Guid formStatusId = Context.LookUps.FirstOrDefault(f => f.Type == "FormStatus" && f.Title == "مصوب").Id;
        if (formClassification.FormStatusId == formStatusId)
        {
            if (formClassification.Ratifiers == null)
                throw new ArgumentException("شما تصویب کننده ای انتخاب نکردید.");
        }
        // اضافه شدن مشخصات برگه
        FormClassification form = FormClassificationMapper.MapToModel(formClassification, 0);
        Context.FormClassifications.Add(form);


        Guid CreatorsId = Context.LookUps.FirstOrDefault(f => f.Type == "CreatorType" && f.Title == "تهیه کننده").Id;
        Guid VerifiersId = Context.LookUps.FirstOrDefault(f => f.Type == "CreatorType" && f.Title == "تایید کننده").Id;
        Guid RatifiersId = Context.LookUps.FirstOrDefault(f => f.Type == "CreatorType" && f.Title == "تصویب کننده").Id;

        // اضافه شدن تهیه کننده
        foreach (string item in formClassification.Creators)
        {
            Guid userId = Guid.Parse(item);
            Context.FormClassificationCreators.Add(new FormClassificationCreators()
            {
                CreatorTypeId = CreatorsId,
                FormClassificationId = form.Id,
                Id = Guid.NewGuid(),
                StaffId = Context.Users.Find(userId) == null ? userId : Context.Users.Find(userId).StaffId
            });
        }

        // اضافه شدن تایید کننده
        foreach (string item in formClassification.Verifiers)
        {
            Guid userId = Guid.Parse(item);
            Context.FormClassificationCreators.Add(new FormClassificationCreators()
            {
                CreatorTypeId = VerifiersId,
                FormClassificationId = form.Id,
                Id = Guid.NewGuid(),
                StaffId = Context.Users.Find(userId) == null ? userId : Context.Users.Find(userId).StaffId
            });
        }

        // اضافه شدن تصویب کننده
        if (formClassification.Ratifiers != null)
        {
            foreach (string item in formClassification.Ratifiers)
            {
                Guid userId = Guid.Parse(item);
                Context.FormClassificationCreators.Add(new FormClassificationCreators()
                {
                    CreatorTypeId = RatifiersId,
                    FormClassificationId = form.Id,
                    Id = Guid.NewGuid(),
                    StaffId = Context.Users.Find(userId) == null ? userId : Context.Users.Find(userId).StaffId
                });
            }
        }

        // اضافه شدن پیوست ها
        if (documents != null && documents.Count > 0)
            SaveFormFiles(formClassification.Id.ToString(), documents, webRootPath);


        // اضافه شدن مدارک مرتبط
        if (formClassification.RelatedFormClassificationIds != null)
        {
            foreach (var item in formClassification.RelatedFormClassificationIds)
            {
                if (item == form.Id)
                {
                    continue;
                }

                Context.FormClassificationRelations.Add(new FormClassificationRelation()
                {
                    MainId = form.Id,
                    SecondaryId = item
                });
            }
        }

        // اضافه شدن دسترسی
        GiveAccessToDocument(form.Id, new List<Guid>() { creatorId });
    }

    private void GiveAccessToDocument(Guid formClassificationId, List<Guid> userIds = null, List<Guid> organizationPostTypeIds = null, List<Guid> organizationPostTitleIds = null, List<Guid> chartIds = null)
    {
        if (userIds != null)
        {
            foreach (var userId in userIds)
            {
                Context.FormClassificationAccess.Add(new FormClassificationAccess()
                {
                    AccessId = userId,
                    FormClassificationId = formClassificationId,
                    Type = "UserAccess"
                });
            }
        }
        if (organizationPostTypeIds != null)
        {
            foreach (var organizationPostTypeId in organizationPostTypeIds)
            {
                Context.FormClassificationAccess.Add(new FormClassificationAccess()
                {
                    AccessId = organizationPostTypeId,
                    FormClassificationId = formClassificationId,
                    Type = "OrganizationPostType"
                });
            }
        }
        if (organizationPostTitleIds != null)
        {
            foreach (var organizationPostTitleId in organizationPostTitleIds)
            {
                Context.FormClassificationAccess.Add(new FormClassificationAccess()
                {
                    AccessId = organizationPostTitleId,
                    FormClassificationId = formClassificationId,
                    Type = "OrganizationPostTitle"
                });
            }
        }
        if (chartIds != null)
        {
            foreach (var chartId in chartIds)
            {
                Context.FormClassificationAccess.Add(new FormClassificationAccess()
                {
                    AccessId = chartId,
                    FormClassificationId = formClassificationId,
                    Type = "Chart"
                });
            }
        }

    }
    public void CreateNewVersion(FormClassificationViewModel formClassification, Dictionary<string, MemoryStream> documents, string username, string webRootPath)
    {
        var user = Context.Users.Single(c => c.UserName == username);
        var creatorId = user.Id;

        var formId = formClassification.Id.ToString();
        formClassification.Id = Guid.NewGuid();
        if (formClassification.Creators == null)
            throw new ArgumentException("شما تهیه کننده ای انتخاب نکردید.");
        if (formClassification.Verifiers == null)
            throw new ArgumentException("شما تایید کننده ای انتخاب نکردید.");
        Guid formStatusId = Context.LookUps.FirstOrDefault(f => f.Type == "FormStatus" && f.Title == "مصوب").Id;
        if (formClassification.FormStatusId == formStatusId)
        {
            if (formClassification.Ratifiers == null)
                throw new ArgumentException("شما تصویب کننده ای انتخاب نکردید.");
        }
        // اضافه شدن مشخصات برگه
        var orgForm = Context.FormClassifications.Find(formClassification.ParentId);
        orgForm.RecordEditDate = int.Parse(DateTime.Now.ToString().Split(' ')[0].Replace("/", ""));
        Context.Set<FormClassification>().Update(orgForm);


        FormClassification form = FormClassificationMapper.MapToModel(formClassification, 0);
        Context.FormClassifications.Add(form);


        Guid CreatorsId = Context.LookUps.FirstOrDefault(f => f.Type == "CreatorType" && f.Title == "تهیه کننده").Id;
        Guid VerifiersId = Context.LookUps.FirstOrDefault(f => f.Type == "CreatorType" && f.Title == "تایید کننده").Id;
        Guid RatifiersId = Context.LookUps.FirstOrDefault(f => f.Type == "CreatorType" && f.Title == "تصویب کننده").Id;

        // اضافه شدن تهیه کننده
        foreach (string item in formClassification.Creators)
        {
            Guid userId = Guid.Parse(item);
            Context.FormClassificationCreators.Add(new FormClassificationCreators()
            {
                CreatorTypeId = CreatorsId,
                FormClassificationId = form.Id,
                Id = Guid.NewGuid(),
                StaffId = Context.Users.Find(userId) == null ? userId : Context.Users.Find(userId).StaffId
            });
        }

        // اضافه شدن تایید کننده
        foreach (string item in formClassification.Verifiers)
        {
            Guid userId = Guid.Parse(item);
            Context.FormClassificationCreators.Add(new FormClassificationCreators()
            {
                CreatorTypeId = VerifiersId,
                FormClassificationId = form.Id,
                Id = Guid.NewGuid(),
                StaffId = Context.Users.Find(userId) == null ? userId : Context.Users.Find(userId).StaffId
            });
        }

        // اضافه شدن تصویب کننده
        if (formClassification.Ratifiers != null)
        {
            foreach (string item in formClassification.Ratifiers)
            {
                Guid userId = Guid.Parse(item);
                Context.FormClassificationCreators.Add(new FormClassificationCreators()
                {
                    CreatorTypeId = RatifiersId,
                    FormClassificationId = form.Id,
                    Id = Guid.NewGuid(),
                    StaffId = Context.Users.Find(userId) == null ? userId : Context.Users.Find(userId).StaffId
                });
            }
        }

        //اضافه شدن پیوست ها
        if (documents != null && documents.Count > 0)
        {
            SaveFormFiles(formClassification.Id.ToString(), documents,webRootPath);
        }
        if (formClassification.Files != null)
        {
            foreach (string item in formClassification.Files)
            {
                var fileName = item;
                var fromFolder = Path.Combine(webRootPath, "Images/upload/formClassification/Document/" + formId.ToLower());
                var destFolder = Path.Combine(webRootPath, "/Images/upload/formClassification/Document/" + form.Id.ToString().ToLower());
                if (Directory.Exists(destFolder) == false)
                    Directory.CreateDirectory(destFolder);
                var fromPath = Path.Combine(fromFolder, fileName);
                var destPath = Path.Combine(destFolder, fileName);
                File.Copy(fromPath, destPath);
            }
        }

        // اضافه شدن مدارک مرتبط
        if (formClassification.RelatedFormClassificationIds != null)
        {
            foreach (var item in formClassification.RelatedFormClassificationIds)
            {
                if (item == form.Id)
                {
                    continue;
                }

                Context.FormClassificationRelations.Add(new FormClassificationRelation()
                {
                    MainId = form.Id,
                    SecondaryId = item
                });
            }
        }

        // اضافه شدن دسترسی
        GiveAccessToDocument(form.Id, new List<Guid>() { creatorId });
    }
    public void CheckClassificationId(string FormNo, string EditNo)
    {
        Context.FormClassifications.Where(c => c.FormNo == FormNo).Where(c => c.EditNo == EditNo).SingleOrDefault();
    }

    private bool FormExists(string FormNo, Guid formTypeId)
    {
        return Context.FormClassifications.Any(c => c.FormNo == FormNo && c.FormTypeId == formTypeId);
    }
    public bool FormNumberExists(FormClassificationViewModel form)
    {
        return Context.FormClassifications.Any(c => c.FormNo == form.FormNo && c.FormTypeId == form.FormTypeId && c.EditNo == form.EditNo && c.Id != form.Id);
    }

    public List<FormClassificationViewModel> Report_ReadAll()
    {
        List<FormClassificationViewModel> forms = new List<FormClassificationViewModel>();
        var data = Context.FormClassifications.Where(f => f.Parent == null).OrderByDescending(c => c.RegisterDate).ThenByDescending(c => c.EditNo).Distinct().ToList();
        data.ForEach(x =>
        {
            int totalDownloadCount = 0;
            var currentItem = Context.FormClassifications.Where(c => c.Id == x.Id || c.Parent == x.Id).OrderByDescending(c => c.RegisterDate).ThenByDescending(c => c.EditNo).First();
            var formClassificationVM = FormClassificationMapper.MapToViewModel(currentItem);
            var childsAndParent = Context.FormClassifications.Where(c => c.Parent == currentItem.Id || c.Id == currentItem.Id).ToList();
            childsAndParent.ForEach(c =>
            {
                totalDownloadCount += c.Counter;
            });
            formClassificationVM.TotalDownloadCount = totalDownloadCount;
            forms.Add(formClassificationVM);
        });
        return forms;
    }

    public List<FormClassificationViewModel> Report_Read(string username)
    {
        var user = Context.Users.Single(c => c.UserName == username);
        var StaffId = user.StaffId;
        var userId = user.Id;

        var ChartIdAccess = Context.OrganiztionInfos.Where(f => f.StaffId == StaffId).Select(d => d.ChartId).ToList();
        var OrganizationPostTitle = Context.OrganiztionInfos.Where(f => f.StaffId == StaffId).Select(d => d.OrganiztionPostId).ToList();
        var OrganizationPostType = Context.OrganiztionInfos.Where(f => f.StaffId == StaffId).Select(d => d.OrganiztionPost.Aux).ToList();

        var UserAccess = Context.FormClassificationAccess.Where(w => w.AccessId == userId && w.Type == "UserAccess").ToList();

        List<FormClassificationAccess> AccessList = new List<FormClassificationAccess>();
        var AllCharts = Context.FormClassificationAccess.Where(f => f.Type == "Chart");

        ChartIdAccess.ForEach(item =>
        {
            AllCharts.Where(f => f.AccessId == item).ToList().ForEach(p =>
            {
                AccessList.Add(p);
            });
        });

        var AllOrganizationPostTitle = Context.FormClassificationAccess.Where(f => f.Type == "OrganizationPostTitle");
        OrganizationPostTitle.ForEach(item =>
        {
            AllOrganizationPostTitle.Where(f => f.AccessId == item).ToList().ForEach(x =>
            {
                AccessList.Add(x);
            });
        });

        var AllOrganizationPostType = Context.FormClassificationAccess.Where(f => f.Type == "OrganizationPostType");
        OrganizationPostType.ForEach(item =>
        {
            Guid guid = Guid.Parse(item);
            AllOrganizationPostType.Where(f => f.AccessId == guid).ToList().ForEach(item2 =>
            {
                AccessList.Add(item2);
            });

        });

        UserAccess.ForEach(item =>
        {
            AccessList.Add(item);
        });
        List<FormClassification> allForms = new List<FormClassification>();
        AccessList.ForEach(item =>
        {
            var frm = Context.FormClassifications.FirstOrDefault(x => x.Id == item.FormClassificationId);
            allForms.Add(frm);

        });

        // کاربرد این خط چیه ؟ چرا باید فقط مدارک مصوب نمایش داده شه؟ 
        // allForms = allForms.Where(a => a.FormStatus.Code == 1).ToList();
        List<FormClassificationViewModel> forms = new List<FormClassificationViewModel>();
        allForms.ForEach(item => forms.Add(MapToFormClassificationViewModel(item)));
        return forms.OrderByDescending(i => i.RegisterDate).ToList();
    }

    public List<FormClassificationViewModel> GetAllDocument()
    {
        var allForms = Context.FormClassifications;
        List<FormClassificationViewModel> forms = new List<FormClassificationViewModel>();
        foreach (var item in allForms)
        {
            forms.Add(MapToFormClassificationViewModel(item));
        }
        return forms.OrderByDescending(t => t.RegisterDate).ToList();
    }

    private FormClassificationViewModel MapToFormClassificationViewModel(FormClassification model)
    {

        string aux = Context.LookUps.FirstOrDefault(i => i.Id == model.FormTypeId)?.Aux;

        string formNu = string.IsNullOrEmpty(aux) ? " " + model.FormNo + " - " + model.EditNo : aux + "" + " - " + model.FormNo + " - " + model.EditNo;

        FormClassificationViewModel vm = new FormClassificationViewModel()
        {
            Id = model.Id,
            AccessTypeId = model.AccessTypeId,
            //AccessType = model.AccessType,
            WorkFlowLookupId = model.WorkFlowLookupId,
            WorkFlowLookup_Title = model.WorkFlowLookup?.Title,
            ConfidentialLevelId = model.ConfidentialLevelId,
            Dsr = model.Dsr,
            EditDate = model.EditDate == null ? null : HelperBs.MakeDate(model.EditDate.ToString()),
            CreatedDate = model.CreatedDate == null ? null : HelperBs.MakeDate(model.CreatedDate.ToString()),
            EditNo = model.EditNo,
            RecordEditDate = model.RecordEditDate == null ? null : HelperBs.MakeDate(model.RecordEditDate.ToString()),
            FormNo = formNu,
            FormStatusId = model.FormStatusId,
            //FormStatus = model.AccessType,
            StandardTypeId = model.StandardTypeId,
            StandardType_Title = model.StandardType.Title,
            Title = model.Title,
            IsActive = model.IsActive,
            FormTypeId = model.FormTypeId,
            FormType_Title = model.FormType.Title,
            FormStatus_Title = model.FormStatus.Title,
            RegisterDate = HelperBs.MakeDate(model.RegisterDate.ToString()),
            Counter = model.Counter,
            Tags = model.Tags,
            ParentId = model.Parent,
            //FormTypeAux = aux,
            FormTypeAux = "",
            //FormTypeAuxAndNumber = aux + " - " + model.FormNo + " - " + model.EditNo,
            FormTypeAuxAndNumber = "" + " - " + model.FormNo + " - " + model.EditNo,
            IsImplementedInBpms = model.IsImplementedInBpms
        };

        return vm;
    }
}
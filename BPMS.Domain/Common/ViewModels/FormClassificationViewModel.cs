using BPMS.Domain.Common.Helpers;
using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Entities;
using Kendo.Mvc.UI;

namespace BPMS.Domain.Common.ViewModels;

public class FormClassificationViewModel
{
    public Guid FormClassificationId { get; set; }

    [Display(Name = "شناسه")]
    public Guid Id { get; set; }
    [Display(Name = "عنوان مدرک")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public string Title { get; set; }
    [MaxLength(30, ErrorMessage = "مقدار {0} باید کمتر از {1} کاراکتر باشد")]
    [Display(Name = "شناسه مدرک")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public string FormNo { get; set; }
    [Display(Name = "نوع مدرک")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public Guid FormTypeId { get; set; }
    public LookUp FormType { get; set; }
    public string FormType_Title { get; set; }
    [Display(Name = "وضعیت")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public Guid FormStatusId { get; set; }
    public LookUp FormStatus { get; set; }
    public string FormStatus_Title { get; set; }
    [Display(Name = "تاریخ ویرایش")]
    public string EditDate { get; set; }
    [Display(Name = "وضعیت فعال")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public bool IsActive { get; set; } = true;
    [Display(Name = "نوع استاندارد")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public Guid StandardTypeId { get; set; }
    public LookUp StandardType { get; set; }
    public string StandardType_Title { get; set; }
    [MaxLength(10, ErrorMessage = "مقدار {0} باید کمتر از {1} کاراکتر باشد")]
    [Display(Name = "شماره ویرایش")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public string EditNo { get; set; }
    [Display(Name = "نام فرآیند")]
    public Guid? WorkFlowLookupId { get; set; }
    public virtual LookUp WorkFlowLookupType { get; set; }
    public string WorkFlowLookup_Title { get; set; }
    [Display(Name = "سطح محرمانگی")]
    public Guid? ConfidentialLevelId { get; set; }
    public LookUp ConfidentialLevel { get; set; }
    [Display(Name = "نوع درسترسی")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public Guid AccessTypeId { get; set; }
    public LookUp AccessType { get; set; }
    public string AccessType_Title { get; set; }
    [Display(Name = "تاریخ ویرایش رکورد")]
    public string RecordEditDate { get; set; }
    [Display(Name = "تاریخ ثبت")]
    public string RegisterDate { get; set; }
    [Display(Name = "تاریخ ثبت سند")]
    public string CreatedDate { get; set; }
    [Display(Name = "خلاصه تغییرات")]
    public string Dsr { get; set; }
    [Display(Name = "تعداد بازدید")]
    public int Counter { get; set; }
    public string[] Creators { get; set; }
    public string[] Verifiers { get; set; }
    public string[] Ratifiers { get; set; }
    public string[] Files { get; set; }
    public string ChangeMode { get; set; }
    [Display(Name = "کلمات کلیدی")]
    public string Tags { get; set; }
    public IEnumerable<TreeViewItemModel> ChartsTreeView { get; set; }
    public IEnumerable<Guid> Charts { get; set; }
    public IEnumerable<Guid> OrganizationPostTitle { get; set; }
    public IEnumerable<Guid> OrganizationPostType { get; set; }
    public IEnumerable<Guid> UserAccess { get; set; }
    public Guid? ParentId { get; set; }

    public string FormTypeAux { get; set; }
    public string FormTypeAuxAndNumber { get; set; }

    [Display(Name = "در BPMS پیاده سازی شده است")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public bool IsImplementedInBpms { get; set; }

    // Total count of downloads of all child and this form classification
    [Display(Name = "دفعات کل دانلود")]
    public int? TotalDownloadCount { get; set; }

    public List<Guid> RelatedFormClassificationIds { get; set; }
}
public static class FormClassificationMapper
{
    public static FormClassification MapToModel(FormClassificationViewModel model, int counter)
    {
        int? EditDate = null;
        if (model.EditDate != null)
            EditDate = int.Parse(model.EditDate.Replace("/", ""));

        int? RecordEditDate = null;
        if (model.RecordEditDate != null)
            RecordEditDate = int.Parse(model.RecordEditDate.Replace("/", ""));

        int? createdDate = null;
        if (!string.IsNullOrEmpty(model.CreatedDate))
        {
            createdDate = int.Parse(model.CreatedDate.Replace("/", ""));
        }

        return new FormClassification()
        {
            Id = model.Id == Guid.Empty ? Guid.NewGuid() : model.Id,
            AccessTypeId = model.AccessTypeId,
            ConfidentialLevelId = model.ConfidentialLevelId,
            WorkFlowLookupId = model.WorkFlowLookupId,
            Dsr = model.Dsr,
            EditDate = EditDate,
            EditNo = model.EditNo,
            RecordEditDate = RecordEditDate,
            FormNo = model.FormNo,
            FormStatusId = model.FormStatusId,
            StandardTypeId = model.StandardTypeId,
            Title = model.Title,
            IsActive = model.IsActive,
            FormTypeId = model.FormTypeId,
            RegisterDate = int.Parse(DateTime.Now.ToString().Split(' ')[0].Replace("/", "")),
            Counter = counter,
            Tags = model.Tags,
            Parent = model.ParentId,
            IsImplementedInBpms = model.IsImplementedInBpms,
            CreatedDate = createdDate
        };
    }
    public static FormClassificationViewModel MapToViewModel(FormClassification model)
    {
        // todo
        //IUnitOfWork uow = new UnitOfWork(new DatabaseContext());
        //string aux = uow.LookUps.Get(model.FormTypeId).Aux;

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
            FormNo = model.FormNo,
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
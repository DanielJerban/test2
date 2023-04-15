using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class FormLookUp2NRepository : Repository<FormLookUp2N>, IFormLookUp2NRepository
{
    public FormLookUp2NRepository(BpmsDbContext context) : base(context)
    {
    }

    public void CreateNew(FormLookup2NViewModel model)
    {
        if (model.Type1.Trim() == model.Type2.Trim())
            throw new ArgumentException($"نوع اول با نوع دوم برابر است.");
        var look = Context.LookUps.Where(d => d.Type.Trim() == model.Type1.Trim());
        var look2 = Context.LookUps.Where(d => d.Type.Trim() == model.Type2.Trim());
        var form = Context.FormLookUp2N.Where(
            d => d.Type1.Trim() == model.Type1.Trim()
                 || d.Type1.Trim() == model.Type2.Trim()
                 || d.Type2.Trim() == model.Type1.Trim()
                 || d.Type2.Trim() == model.Type2.Trim());

        if (model.Id == Guid.Empty)
        {
            if (form.Any())
                throw new ArgumentException($"نوع تکراری وارد شده است.");
            model.Id = Guid.NewGuid();

            if (look.Any())
                throw new ArgumentException($"نوع '{model.Type1}' تکراری وارد شده است.");

            if (look2.Any())
                throw new ArgumentException($"نوع '{model.Type2}' تکراری وارد شده است.");
        }
        else
        {
            var f = form.FirstOrDefault();
            if (f != null && f.Id != model.Id)
                throw new ArgumentException($"نوع تکراری وارد شده است.");

            var first = look.FirstOrDefault();
            if (first != null && first.Type.Trim() != model.Type1.Trim())
                throw new ArgumentException($"نوع '{model.Type1}' تکراری وارد شده است.");
            var second = look2.FirstOrDefault();
            if (second != null && second.Type.Trim() != model.Type2.Trim())
                throw new ArgumentException($"نوع '{model.Type2}' تکراری وارد شده است.");
        }

        Context.FormLookUp2N.Update(new FormLookUp2N
        {
            Id = model.Id,
            Type1 = model.Type1,
            Title1 = model.Title1,
            Type2 = model.Type2,
            Title2 = model.Title2,
            Title = model.Title
        });
    }

    public void RemoveFormLookUp2N(Guid id)
    {
        var formLookup = Context.FormLookUp2N.Find(id);
        if (formLookup == null)
            throw new ArgumentException("رکورد مورد نظر یافت نشد.");
        var look = Context.LookUps.Where(d =>
            d.Type.Trim() == formLookup.Type1.Trim() || d.Type.Trim() == formLookup.Type2.Trim());
        if (look.Any())
            throw new ArgumentException("به دلیل داشتن رکورد در جدول مجازی امکان حذف وجود ندارد.");
        Context.FormLookUp2N.Remove(formLookup);
    }
}
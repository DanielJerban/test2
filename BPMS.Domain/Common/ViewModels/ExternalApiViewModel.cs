using BPMS.Domain.Common.Dtos;
using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.Enums;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BPMS.Domain.Common.ViewModels;

public class ExternalApiViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "عنوان")]
    public string Title { get; set; }

    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "آدرس")]
    public string Url { get; set; }

    [Display(Name = "نام کاربری")]
    public string Username { get; set; }

    [Display(Name = "کلمه عبور")]
    public string Password { get; set; }

    [Display(Name = "توکن")]
    public string Token { get; set; }

    [Display(Name = "ورودی های ثابت")]
    public string Content { get; set; }
    public string ResponseStructute { get; set; }

    [Display(Name = "استفاده در جدول")]
    public bool UseInGrid { get; set; }

    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "نوع محتوا")]
    public ContentTypeEnum ContentType { get; set; }
    public string ContentTypeString { get; set; }

    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "نوع متد")]
    public ActionType ActionType { get; set; }
    public string ActionTypeString { get; set; }

    [Required]
    [Display(Name = "نوع احراز هویت")]
    public AuthorizationType AuthorizationType { get; set; }
    public string AuthorizationTypeString { get; set; }

    public string FormDataText { get; set; }
    public string FormDataFileText { get; set; }

    public List<ExternalApiHeaderDto> Headers { get; set; }
    public string HeadersJson { get; set; }

    public List<FormDataFileDto> FormDataFiles { get; set; }

    public static ExternalApiViewModel Map(ExternalApi model)
    {
        return new ExternalApiViewModel()
        {
            Id = model.Id,
            Content = model.Content,
            Token = model.Token,
            Url = model.Url,
            Title = model.Title,
            UseInGrid = model.UseInGrid,
            AuthorizationType = model.AuthorizationType,
            AuthorizationTypeString = model.AuthorizationType.ToDisplay(),
            Username = model.Username,
            ActionType = model.ActionType,
            ActionTypeString = model.ActionType.ToDisplay(),
            Password = model.Password,
            ContentType = model.ContentType,
            ContentTypeString = model.ContentType.ToDisplay(),
            ResponseStructute = model.ResponseStructute,
            Headers = JsonConvert.DeserializeObject<List<ExternalApiHeaderDto>>(model.Headers),
            HeadersJson = model.Headers
        };
    }
}

public class FormDataFileDto
{
    public string FileKey { get; set; }
    public IFormFile File { get; set; }
}
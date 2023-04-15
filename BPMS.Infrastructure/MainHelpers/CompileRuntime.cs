using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.CSharp;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace BPMS.Infrastructure.MainHelpers;

public static class CompileRuntime
{
    // todo: uncomment later 
    //public static dynamic CodeDomGenerateClass(string name, string model)
    //{
    //    var props = JsonConvert.DeserializeObject<IDictionary<string, string>>(model);

    //    var compilerOptions = new Dictionary<string, string> { { "CompilerVersion", "v4.0" } };

    //    var csc = new CSharpCodeProvider(compilerOptions);
    //    var parameters = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll" }, "BPMS.Core.Models.dll",
    //        false)
    //    {
    //        GenerateExecutable = false,
    //        GenerateInMemory = true
    //    };

    //    var compileUnit = new CodeCompileUnit();
    //    var ns = new CodeNamespace("BPMS.Core.Models");
    //    compileUnit.Namespaces.Add(ns);
    //    ns.Imports.Add(new CodeNamespaceImport("System"));

    //    var classType = new CodeTypeDeclaration(name)
    //    {
    //        Attributes = MemberAttributes.Public
    //    };
    //    ns.Types.Add(classType);

    //    foreach (var prop in props)
    //    {
    //        var fieldName = "_" + prop.Key;
    //        var field = new CodeMemberField(prop.Value.GetType(), fieldName);
    //        classType.Members.Add(field);

    //        var property = new CodeMemberProperty
    //        {
    //            Attributes = MemberAttributes.Public | MemberAttributes.Final,
    //            Type = new CodeTypeReference(prop.Value.GetType()),
    //            Name = prop.Key
    //        };
    //        property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)));
    //        property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName), new CodePropertySetValueReferenceExpression()));
    //        classType.Members.Add(property);
    //    }

    //    var results = csc.CompileAssemblyFromDom(parameters, compileUnit);
    //    results.Errors.Cast<CompilerError>().ToList().ForEach(error => Console.WriteLine(error.ErrorText));

    //    var type = results.CompiledAssembly.GetType("BPMS.Core.Models." + name);

    //    return JsonConvert.DeserializeObject(model, type);


    //}
    //public static bool BpmsRuleEngine(dynamic work, SystemRuleDto bpmsRule)
    //{

    //    var engine = new MRE();

    //    var fake = typeof(MRE).GetMethod("CompileRule").MakeGenericMethod(work.GetType())
    //        .Invoke(engine, new object[] { bpmsRule }) as dynamic;


    //    return fake(work);


    //}
    //public static Type RoslynGenerateClass(string model)
    //{
    //    return GetTypeClass(model, null, false);

    //}
    //public static IEnumerable<dynamic> FillJsonArrayInDynamicClass(string model)
    //{
    //    var type = GetTypeClass(model, null, false);
    //    var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
    //    return (IEnumerable<dynamic>)JsonConvert.DeserializeObject(model, list.GetType());
    //}
    //public static dynamic RoslynGenerateClass(string model, string method)
    //{
    //    var type = GetTypeClass(model, method, true);
    //    return JsonConvert.DeserializeObject(model, type);

    //}
    //private static Type GetTypeClass(string model, string method, bool pascal)
    //{

    //    var className = "RootObject";
    //    var @class = Prepare(model, className, 1, false, pascal, "None", true);
    //    try
    //    {
    //        @class = @class?.Trim();
    //    }
    //    catch (Exception e)
    //    {
    //        Console.WriteLine(e);
    //        throw;
    //    }
    //    if (!string.IsNullOrWhiteSpace(method) && method != "null")
    //    {
    //        @class = @class.Remove(@class.Length - 1);
    //        @class = @class + method + " }";
    //    }

    //    var regex = new Regex(Regex.Escape("#=param#"));
    //    @class = regex.Replace(@class, "dynamic param = JsonConvert.DeserializeObject(work);", 1);

    //    var code = @"
    //        using System;
    //        using System.Linq;
    //        using System.Collections.Generic;
    //        using System.Data.Entity;
    //        using Newtonsoft.Json;
    //        using System.Globalization;
    //        using BPMS.Infrastructure.MainHelpers;
    //        using RestSharp;
    //        using BPMS.Infrastructure;
    //        namespace BPMS.Domain.Entities
    //        {
    //          " + @class + @"
    //        }";


    //    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);


    //    string assemblyName = Path.GetRandomFileName();
    //    MetadataReference[] references = {
    //        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(IUnitOfWork).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(IList<>).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(Microsoft.EntityFrameworkCore.DbSet<>).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(IListSource).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(CSharpArgumentInfo).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(JsonConvert).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(RestClient).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(Util).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(DateTimeStyles).Assembly.Location)

    //    };

    //    CSharpCompilation compilation = CSharpCompilation.Create(
    //        assemblyName,
    //        syntaxTrees: new[] { syntaxTree },
    //        references: references,
    //        options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));


    //    var ms = new MemoryStream();

    //    EmitResult result = compilation.Emit(ms);

    //    if (!result.Success)
    //    {
    //        IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
    //            diagnostic.IsWarningAsError ||
    //            diagnostic.Severity == DiagnosticSeverity.Error);

    //        foreach (Diagnostic diagnostic in failures)
    //        {
    //            //Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
    //            throw new ArgumentException($"{diagnostic.Id}: {diagnostic.GetMessage()}");

    //        }
    //    }
    //    ms.Seek(0, SeekOrigin.Begin);
    //    var assembly = Assembly.Load(ms.ToArray());

    //    //تست اجرای متد
    //    var type = assembly.GetType("BPMS.Domain.Entities." + className);
    //    return type;
    //}

    //public static void ExecuteVoidMethod(string code)
    //{
    //    try
    //    {
    //        code = code.Trim();
    //    }
    //    catch (Exception e)
    //    {
    //        Console.WriteLine(e);
    //        throw;
    //    }

    //    var myCode = RoslynHelper.GetScheduleClass(code);


    //    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(myCode);


    //    string assemblyName = Path.GetRandomFileName();
    //    MetadataReference[] references = {
    //        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(IUnitOfWork).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(IJobService).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(IList<>).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(Microsoft.EntityFrameworkCore.DbSet<>).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(IListSource).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(CSharpArgumentInfo).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(JsonConvert).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(RestClient).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(Util).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(UnicodeEncoding).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(HttpContext).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(FlowParam).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(DateTimeStyles).Assembly.Location),
    //        //MetadataReference.CreateFromFile(typeof(LookUpService).Assembly.Location)
    //    };

    //    CSharpCompilation compilation = CSharpCompilation.Create(
    //        assemblyName,
    //        syntaxTrees: new[] { syntaxTree },
    //        references: references,
    //        options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));


    //    var ms = new MemoryStream();

    //    EmitResult result = compilation.Emit(ms);

    //    if (!result.Success)
    //    {
    //        IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
    //            diagnostic.IsWarningAsError ||
    //            diagnostic.Severity == DiagnosticSeverity.Error);

    //        foreach (Diagnostic diagnostic in failures)
    //        {
    //            //Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
    //            throw new ArgumentException($"{diagnostic.Id}: {diagnostic.GetMessage()}");

    //        }
    //    }
    //    ms.Seek(0, SeekOrigin.Begin);
    //    var assembly = Assembly.Load(ms.ToArray());

    //    //تست اجرای متد
    //    var type = assembly.GetType("BPMS.Core.Models.MyClass");
    //    var obj = Activator.CreateInstance(type);
    //    type.InvokeMember("MyMethod",
    //        BindingFlags.Default | BindingFlags.InvokeMethod,
    //        null,
    //        obj,
    //        new object[] { });

    //}
    //public static string Prepare(string JSON, string classname, int language, bool nest, bool pascal, string propertyAttribute, bool hasGetSet = false)
    //{
    //    if (string.IsNullOrEmpty(JSON))
    //    {
    //        return null;
    //    }

    //    ICodeWriter writer;

    //    if (language == 1)
    //        writer = new CSharpCodeWriter();
    //    else if (language == 2)
    //        writer = new VisualBasicCodeWriter();
    //    else if (language == 7)
    //        writer = new TypeScriptCodeWriter();
    //    //else if (language == 4)
    //    //    writer = new SqlCodeWriter();
    //    else/* if (language == 5)*/
    //        writer = new JavaCodeWriter();
    //    //else
    //    //    writer = new PhpCodeWriter();

    //    var gen = new JsonClassGenerator();
    //    gen.Example = JSON;
    //    gen.InternalVisibility = false;
    //    gen.CodeWriter = writer;
    //    gen.ExplicitDeserialization = false;
    //    if (nest)
    //        gen.Namespace = "JSONUtils";
    //    else
    //        gen.Namespace = null;

    //    gen.NoHelperClass = false;
    //    gen.SecondaryNamespace = null;
    //    gen.UseProperties = (language != 5 && language != 6) || hasGetSet;
    //    gen.MainClass = classname;
    //    gen.UsePascalCase = pascal;
    //    //gen.PropertyAttribute = propertyAttribute;

    //    gen.UseNestedClasses = nest;
    //    gen.ApplyObfuscationAttributes = false;
    //    gen.SingleFile = true;
    //    gen.ExamplesInDocumentation = false;

    //    gen.TargetFolder = null;
    //    gen.SingleFile = true;

    //    using (var sw = new StringWriter())
    //    {
    //        gen.OutputStream = sw;
    //        gen.GenerateClasses();
    //        sw.Flush();

    //        return sw.ToString();
    //    }
    //}
    //public static List<Tuple<PropertyInfo, string>> GetProperties(string aux2)
    //{
    //    var code = @"
    //        using System;
    //        using System.Linq;
    //        using BPMS.Domain.Common.ViewModels;
    //        using System.Reflection;
    //        using BPMS.Filters;
    //        using System.Collections;
    //        using System.Collections.Generic;
    //        namespace BPMS.Domain.Common.ViewModels
    //        {
    //           public class myClass
    //            {
    //                public List<Tuple<PropertyInfo, string>> PropertyInfos;
    //                public Stack<string> Name;
    //                public List<Tuple<PropertyInfo,string>> GetProp()
    //                {
    //                    var type1 = typeof(" + aux2 + @");
    //                    PropertyInfos = new List<Tuple<PropertyInfo, string>>();
    //                    Name = new Stack<string>();
    //                    Get(type1);
    //                    return PropertyInfos;
    //                }
    //                public void Get(Type type1)
    //                {
    //                    var prop = type1.GetProperties().ToList().Where(p => Attribute.IsDefined(p, typeof(ExpersionAttribute)));
    //                    foreach (var propertyInfo in prop)
    //                    {

    //                        var type = propertyInfo.PropertyType;
    //                        if (type.FullName.Contains(""BPMS.Core.ViewModel""))
    //                        {
    //                            Name.Push(type.Name + ""."");
    //                            Get(type);
    //                            Name.Pop();
    //                        }
    //                        else
    //                        {
    //                            PropertyInfos.Add(new Tuple<PropertyInfo, string>(propertyInfo, ConvertStackToString(Name)));
    //                        }
    //                     }
    //                }
    //                private string ConvertStackToString(Stack<string> name)
    //                {
    //                    var name2 = new Stack<string>(name.ToArray());
    //                    var p = name2.Aggregate("""", (current, item) => current + item );
    //                    return p.Length > 0 ? p.Substring(0, p.Length) : null;
    //                }
    //            }
    //        }";
    //    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
    //    string assemblyName = Path.GetRandomFileName();
    //    MetadataReference[] references = {
    //        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(IEnumerable<>).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(OverTimeViewModel).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(PropertyInfo).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(DbSet<>).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(Stack<>).Assembly.Location)
    //    };
    //    CSharpCompilation compilation = CSharpCompilation.Create(
    //        assemblyName,
    //        syntaxTrees: new[] { syntaxTree },
    //        references: references,
    //        options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    //    var ms = new MemoryStream();
    //    EmitResult result = compilation.Emit(ms);


    //    if (!result.Success)
    //    {
    //        IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
    //            diagnostic.IsWarningAsError ||
    //            diagnostic.Severity == DiagnosticSeverity.Error);

    //        foreach (Diagnostic diagnostic in failures)
    //        {
    //            //Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
    //            throw new ArgumentException($"{diagnostic.Id}: {diagnostic.GetMessage()}");

    //        }
    //    }
    //    ms.Seek(0, SeekOrigin.Begin);
    //    var assembly = Assembly.Load(ms.ToArray());
    //    //تست اجرای متد
    //    var type = assembly.GetType("BPMS.Domain.Common.ViewModels.myClass");
    //    var m = type.GetMembers();
    //    var obj = Activator.CreateInstance(type);
    //    var d = type.InvokeMember("GetProp",
    //        BindingFlags.Default | BindingFlags.InvokeMethod,
    //        null,
    //        obj,
    //        new object[] { });
    //    return d as List<Tuple<PropertyInfo, string>>;
    //}

    //public static string RoslynChangeJsonVaule(FlowParam param, string scriptTaskMethod)
    //{
    //    string code = GenerateCodeScriptTaskMethod(scriptTaskMethod);
    //    Assembly assembly = CompileCode(code);

    //    //تست اجرای متد
    //    var type = assembly.GetType("BPMS.Domain.Entities.myClass");
    //    var obj = Activator.CreateInstance(type);
    //    var d = type.InvokeMember("ChangeJsonValue",
    //        BindingFlags.Default | BindingFlags.InvokeMethod,
    //        null,
    //        obj,
    //        new object[] { param });

    //    return d as string;

    //}

    //private static string GenerateCodeScriptTaskMethod(string scriptTaskMethod)
    //{
    //    var regex = new Regex(Regex.Escape("#=paramwork#"));
    //    scriptTaskMethod = regex.Replace(scriptTaskMethod, "param.Work = JsonConvert.DeserializeObject(param.Work);", 1);
    //    var code = @"
    //               using System;
    //            using System.Text;
    //            using System.Linq;
    //            using Newtonsoft.Json;
    //            using System.Collections.Generic;
    //            using BPMS.Infrastructure.Data;
    //            using System.Data.Entity;
    //            using BPMS.Domain.Entities;
    //            using Newtonsoft.Json;
    //            using BPMS.Domain.Common.ViewModels;
    //            using BPMS.Infrastructure;
    //            using System.Web.Mvc;
    //        namespace BPMS.Domain.Entities
    //        {

                 
    //            public class myClass
    //            {
    //                private static readonly IUnitOfWork unitOfWork = DependencyResolver.Current.GetService<IUnitOfWork>();
    //               " + scriptTaskMethod + @"
    //            }
    //        }";
    //    return code;
    //}

    //private static Assembly CompileCode(string code)
    //{
    //    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
    //    string assemblyName = Path.GetRandomFileName();
    //    MetadataReference[] references = {
    //        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(IUnitOfWork).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(IUnitOfWork).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(IList<>).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(DbSet<>).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(IListSource).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(CSharpArgumentInfo).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(JsonConvert).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(FlowParam).Assembly.Location)


    //    };

    //    CSharpCompilation compilation = CSharpCompilation.Create(
    //        assemblyName,
    //        syntaxTrees: new[] { syntaxTree },
    //        references: references,
    //        options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    //    var ms = new MemoryStream();

    //    EmitResult result = compilation.Emit(ms);

    //    if (!result.Success)
    //    {
    //        IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
    //            diagnostic.IsWarningAsError ||
    //            diagnostic.Severity == DiagnosticSeverity.Error);

    //        foreach (Diagnostic diagnostic in failures)
    //        {
    //            //Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
    //            throw new ArgumentException($"{diagnostic.Id}: {diagnostic.GetMessage()}");

    //        }
    //    }
    //    ms.Seek(0, SeekOrigin.Begin);
    //    var assembly = Assembly.Load(ms.ToArray());

    //    return assembly;
    //}

    //public static IEnumerable<Guid> RoslynGenerateBusinnesAcceptor(FlowParam param, string businessAcceptorMethod)
    //{

    //    var regex = new Regex(Regex.Escape("#=paramwork#"));
    //    businessAcceptorMethod = regex.Replace(businessAcceptorMethod, "param.Work = JsonConvert.DeserializeObject(param.Work);", 1);
    //    var code = @"
    //        using System;
    //        using System.Linq;
    //        using System.Collections.Generic;
    //        using System.Data.Entity;
    //        using BPMS.Core.Models;
    //        using Newtonsoft.Json;
    //        using BPMS.Domain.Common.ViewModels;
    //        using BPMS.Core;
    //        using System.Web.Mvc;
    //        using BPMS.Infrastructure;
    //        namespace BPMS.Core.Models
    //        {

                 
    //            public class myClass
    //            {
    //                private static readonly IUnitOfWork unitOfWork = DependencyResolver.Current.GetService<IUnitOfWork>();
    //               " + businessAcceptorMethod + @"
    //            }
    //        }";


    //    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);


    //    string assemblyName = Path.GetRandomFileName();
    //    MetadataReference[] references = {
    //        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
    //        //MetadataReference.CreateFromFile(typeof(UnitOfWork).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(IUnitOfWork).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(IList<>).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(Microsoft.EntityFrameworkCore.DbSet<>).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(IListSource).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(CSharpArgumentInfo).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(JsonConvert).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(FlowParam).Assembly.Location)


    //    };

    //    CSharpCompilation compilation = CSharpCompilation.Create(
    //        assemblyName,
    //        syntaxTrees: new[] { syntaxTree },
    //        references: references,
    //        options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));


    //    var ms = new MemoryStream();

    //    EmitResult result = compilation.Emit(ms);

    //    if (!result.Success)
    //    {
    //        IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
    //            diagnostic.IsWarningAsError ||
    //            diagnostic.Severity == DiagnosticSeverity.Error);

    //        foreach (Diagnostic diagnostic in failures)
    //        {
    //            //Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
    //            throw new ArgumentException($"{diagnostic.Id}: {diagnostic.GetMessage()}");

    //        }
    //    }
    //    ms.Seek(0, SeekOrigin.Begin);
    //    var assembly = Assembly.Load(ms.ToArray());

    //    //تست اجرای متد
    //    var type = assembly.GetType("BPMS.Core.Models.myClass");
    //    var obj = Activator.CreateInstance(type);
    //    var d = type.InvokeMember("GetStaffIds",
    //        BindingFlags.Default | BindingFlags.InvokeMethod,
    //        null,
    //        obj,
    //        new object[] { param });

    //    return d as IEnumerable<Guid>;

    //}
    //public static IEnumerable<Diagnostic> CheckCode(string method, int status)
    //{
    //    if (status != 1)
    //    {

    //        if (method.Contains("Complete"))
    //        {
    //            throw new ArgumentException("نمی توانید از کلمه Complete در متد استفاده کنید.");
    //        }

    //        if (method.Contains("SaveChanges"))
    //        {
    //            throw new ArgumentException("نمی توانید از کلمه SaveChanges در متد استفاده کنید.");
    //        }

    //        if (method.Contains("Add"))
    //        {
    //            // throw new ArgumentException("نمی توانید از کلمه Add در متد استفاده کنید.");
    //        }

    //        if (method.Contains("Update"))
    //        {
    //            throw new ArgumentException("نمی توانید از کلمه Update در متد استفاده کنید.");
    //        }

    //        if (method.Contains("Remove"))
    //        {
    //            throw new ArgumentException("نمی توانید از کلمه Remove در متد استفاده کنید.");
    //        }

    //        var regex = new Regex(Regex.Escape("#=param#"));
    //        var regex2 = new Regex(Regex.Escape("#=paramwork#"));
    //        method = regex.Replace(method, "dynamic param = JsonConvert.DeserializeObject(work);", 1);
    //        method = regex2.Replace(method, "param.Work = JsonConvert.DeserializeObject(param.Work);", 1);
    //    }

    //    var code = RoslynHelper.GetScheduleCheckClass(method);

    //    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);


    //    string assemblyName = Path.GetRandomFileName();
    //    MetadataReference[] references = {
    //        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
    //        //MetadataReference.CreateFromFile(typeof(UnitOfWork).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(IUnitOfWork).Assembly.Location),
    //        // MetadataReference.CreateFromFile(typeof(JobService).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(IJobService).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(IList<>).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(DbSet<>).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(IListSource).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(CSharpArgumentInfo).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(JsonConvert).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(FlowParam).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(RestClient).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(Util).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(UnicodeEncoding).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(HttpContext).Assembly.Location),
    //        MetadataReference.CreateFromFile(typeof(DateTimeStyles).Assembly.Location),
    //        //MetadataReference.CreateFromFile(typeof(LookUpService).Assembly.Location)
    //    };

    //    CSharpCompilation compilation = CSharpCompilation.Create(
    //        assemblyName,
    //        syntaxTrees: new[] { syntaxTree },
    //        references: references,
    //        options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));


    //    var ms = new MemoryStream();

    //    EmitResult result = compilation.Emit(ms);

    //    if (!result.Success)
    //    {
    //        var failures = result.Diagnostics.Where(diagnostic =>
    //            diagnostic.IsWarningAsError ||
    //            diagnostic.Severity == DiagnosticSeverity.Error);
    //        return failures;

    //    }
    //    return new List<Diagnostic>();
    //}
}